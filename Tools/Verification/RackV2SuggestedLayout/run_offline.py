#!/usr/bin/env python3
"""Compile the production suggested-layout foundation and run its detached geometry checks.

No Unity process, network package or repository build output. Uses the installed Unity CoreModule
as a type reference only; the tested API path calls no Unity native functions. ForSave's unused save
record type is stubbed in OfflineCheck.cs. The live-world/serialization gate remains mandatory.
"""
import argparse
import os
from pathlib import Path
import re
import subprocess
from xml.sax.saxutils import escape

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", type=Path, default=Path("/media/jer/Files/CCF/Verification/RackV2SuggestedLayoutOffline"))
    parser.add_argument("--core-module", type=Path, default=Path("/media/jer/ZX20/Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll"))
    args = parser.parse_args()
    source = ROOT / "Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs"
    text = source.read_text()
    forbidden = r"\b(AddRack|UpdateRack|DeleteRack|ConfirmRackPlan|MarkRackEntrance|ClearRackEntranceMark)\s*\("
    if re.search(forbidden, text) or "UnityEngine.UI" in text or "ScenarioOneUiRoot" in text:
        raise SystemExit("layout getter depends on UI or calls a rack mutation API")
    if not args.core_module.is_file():
        raise SystemExit("missing installed Unity type reference: " + str(args.core_module))
    args.out.mkdir(parents=True, exist_ok=True)
    sources = [source, ROOT / "Assets/ForestPrototype/Racks/RackGeometry.cs",
               ROOT / "Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs",
               ROOT / "Assets/ForestPrototype/RackPlanningModel.cs",
               ROOT / "Assets/ForestPrototype/StandGeometryModel.cs", HERE / "OfflineCheck.cs"]
    items = "\n".join('<Compile Include="' + escape(str(p)) + '" />' for p in sources)
    project = '''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion>
<Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems>
</PropertyGroup><ItemGroup>''' + items + '<Reference Include="UnityEngine.CoreModule"><HintPath>' + escape(str(args.core_module)) + '</HintPath></Reference></ItemGroup></Project>'
    (args.out / "check.csproj").write_text(project)
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    for cmd in (["dotnet", "build", "check.csproj", "-v", "q", "-c", "Release"],
                ["dotnet", "run", "--project", "check.csproj", "--no-build", "-c", "Release"]):
        result = subprocess.run(cmd, cwd=args.out, env=env, capture_output=True, text=True)
        print(result.stdout + result.stderr, end="")
        if result.returncode:
            return result.returncode
    print("SUGGESTED_LAYOUT_STATIC_PASS no UI dependency or rack mutation call")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

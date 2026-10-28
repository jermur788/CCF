#!/usr/bin/env python3
"""Compile production rack geometry and spacing rules with OfflineCheck.cs and run on plain .NET.

No Unity process and nothing is written inside the repository. It proves the pure geometry/validation/snapping rules only; the live
plan, the manager facade, the map and the pointer are the Unity gate's job (RackV2EditingVerification.cs).

  run_offline.py [--out DIR]      exit 0 and RACK_SPACING_OFFLINE_PASS on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path
from xml.sax.saxutils import escape

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", type=Path, default=Path(tempfile.gettempdir()) / "ccf-rack-editing-offline")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    sources = [ROOT / "Assets/ForestPrototype/Racks/RackGeometry.cs", ROOT / "Assets/ForestPrototype/Racks/RackEditing.cs", HERE / "OfflineCheck.cs"]
    for source in sources:
        if not source.is_file():
            sys.exit("missing source " + str(source))
    text = sources[1].read_text()
    if "using UnityEngine" in text or "MonoBehaviour" in text:
        sys.exit("RackEditing.cs must stay free of UnityEngine")
    args.out.mkdir(parents=True, exist_ok=True)
    items = "\n".join('<Compile Include="%s" />' % escape(str(p)) for p in sources)
    (args.out / "check.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
        '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>OfflineCheck</StartupObject></PropertyGroup>'
        "<ItemGroup>" + items + "</ItemGroup></Project>")
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    for command in ([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], [dotnet, "run", "--no-build", "-c", "Release"]):
        result = subprocess.run(command, cwd=args.out, env=env, capture_output=True, text=True)
        sys.stdout.write(result.stdout)
        sys.stderr.write(result.stderr)
        if result.returncode:
            return result.returncode
    return 0


if __name__ == "__main__":
    sys.exit(main())

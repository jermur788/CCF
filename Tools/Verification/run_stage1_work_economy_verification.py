#!/usr/bin/env python3
"""Build/run the pure production files and fixtures with an existing .NET 8 SDK.

No downloaded packages. All generated project/build output stays in ignored Build/.
The Unity editor entry point additionally verifies Unity's actual JsonUtility.
"""
import argparse
from pathlib import Path
import shutil
import subprocess
import sys


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dotnet", default=shutil.which("dotnet"))
    parser.add_argument("--repeat", type=int, default=2)
    args = parser.parse_args()
    if not args.dotnet or args.repeat < 1:
        parser.error("Supply an existing .NET 8 dotnet executable and positive --repeat.")
    root = Path(__file__).resolve().parents[2]
    output = root / "Build" / "Stage1WorkEconomyVerification"
    output.mkdir(parents=True, exist_ok=True)
    project = output / "Stage1WorkEconomyVerification.csproj"
    project.write_text('''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion><EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors><NuGetAudit>false</NuGetAudit>
    <RestoreSources></RestoreSources><ImplicitUsings>disable</ImplicitUsings><Nullable>disable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="../../Assets/ForestPrototype/WorkEconomy/*.cs" />
    <Compile Include="../../Tools/Verification/Stage1WorkEconomyFoundationVerification.cs" />
  </ItemGroup>
</Project>
''', encoding="utf-8")
    subprocess.run([args.dotnet, "build", str(project), "--configuration", "Release", "--nologo"], check=True, cwd=root)
    assembly = output / "bin" / "Release" / "net8.0" / "Stage1WorkEconomyVerification.dll"
    previous = None
    for index in range(args.repeat):
        result = subprocess.run([args.dotnet, str(assembly)], cwd=root, text=True, capture_output=True)
        print(result.stdout, end="")
        if result.stderr:
            print(result.stderr, end="", file=sys.stderr)
        result.check_returncode()
        marker = next((line for line in result.stdout.splitlines() if line.startswith("STAGE1_ECONOMY_VERIFY_PASS")), None)
        if marker is None or (previous is not None and marker != previous):
            raise RuntimeError("Missing pass marker or different evidence between independent runs.")
        previous = marker
    print(f"STAGE1_ECONOMY_REPEAT_PASS runs={args.repeat}")


if __name__ == "__main__":
    main()

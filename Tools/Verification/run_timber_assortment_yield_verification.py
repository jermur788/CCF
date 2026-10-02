#!/usr/bin/env python3
"""Compile pure yield + existing WorkEconomy sources using an installed .NET 8 SDK.

No package dependencies. Repeat evidence hashes; timings are diagnostic only.
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
        parser.error("Supply an installed .NET 8 executable and positive repeat count.")
    root = Path(__file__).resolve().parents[2]
    output = root / "Build" / "TimberAssortmentYieldVerification"
    output.mkdir(parents=True, exist_ok=True)
    project = output / "TimberAssortmentYieldVerification.csproj"
    project.write_text('''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems><TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <NuGetAudit>false</NuGetAudit><RestoreSources></RestoreSources><ImplicitUsings>disable</ImplicitUsings><Nullable>disable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="../../Assets/ForestPrototype/TimberYield/*.cs" />
    <Compile Include="../../Assets/ForestPrototype/WorkEconomy/*.cs" />
    <Compile Include="../../Tools/Verification/TimberAssortmentYieldVerification.cs" />
  </ItemGroup>
</Project>
''', encoding="utf-8")
    subprocess.run([args.dotnet, "build", str(project), "-c", "Release", "--nologo"], check=True, cwd=root)
    assembly = output / "bin" / "Release" / "net8.0" / "TimberAssortmentYieldVerification.dll"
    previous = None
    for index in range(args.repeat):
        run = subprocess.run([args.dotnet, str(assembly)], cwd=root, text=True, capture_output=True)
        (output / f"standalone-{index + 1}.log").write_text(run.stdout + run.stderr, encoding="utf-8")
        print(run.stdout, end="")
        if run.stderr:
            print(run.stderr, file=sys.stderr, end="")
        run.check_returncode()
        marker = next((line for line in run.stdout.splitlines() if line.startswith("TIMBER_YIELD_VERIFY_PASS")), None)
        if marker is None or (previous is not None and marker != previous):
            raise RuntimeError("Missing pass or non-deterministic evidence.")
        previous = marker
    print(f"TIMBER_YIELD_REPEAT_PASS runs={args.repeat}")


if __name__ == "__main__":
    main()

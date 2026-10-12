#!/usr/bin/env python3
"""Build and run the three-species offline logic check on the plain .NET SDK.

Compiles the production Assets/ForestPrototype/SitkaGrowthModel.cs and
MixedDensityModel.cs (neither uses UnityEngine) together with
MixedDensityOfflineCheck.cs in a temporary project OUTSIDE the repository,
then runs it. No Unity Editor, no Library, nothing written inside the repo.

This proves pure arithmetic and decision rules only. It does not prove
controller wiring, scene state or any Unity anchor hash.

Usage: run_offline_check.py [--out DIR]
Exit code 0 and THREE_SPECIES_OFFLINE_PASS on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", "..", ".."))
PRODUCTION = ["SitkaGrowthModel.cs", "MixedDensityModel.cs"]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>MixedDensityOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-three-species-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    os.makedirs(args.out, exist_ok=True)
    sources = [os.path.join(ROOT, "Assets", "ForestPrototype", name) for name in PRODUCTION]
    sources.append(os.path.join(HERE, "MixedDensityOfflineCheck.cs"))
    for source in sources:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    items = "\n".join('    <Compile Include="%s" />' % s for s in sources)
    with open(os.path.join(args.out, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())

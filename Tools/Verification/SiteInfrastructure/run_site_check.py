#!/usr/bin/env python3
"""Build and run the offline check for the dormant Oak/Beech site infrastructure v1 on the plain .NET SDK.

It compiles ONLY the three production data files

  Assets/ForestPrototype/ForestSiteProfile.cs
  Assets/ForestPrototype/SpeciesSiteResponse.cs
  Assets/ForestPrototype/ScenarioSiteProfiles.cs

together with OfflineLogic/SiteInfrastructureOfflineCheck.cs in a temporary project OUTSIDE the repository. A
successful build therefore proves the data files need no UnityEngine, Sitka, species-asset, growth or save code.
No Unity Editor, no Library, nothing written inside the repo.

This proves pure data rules and (by reading source text) that nothing in production or the other verification
tools calls the new types and that the save version is still 23 (the check asserts 23; it was 20 when this runner was
written). It does not prove Unity import/compile, any
anchor hash, or that any descriptor is ecologically correct.

This is a separate runner from ThreeSpecies/OfflineLogic/run_offline_check.py on purpose: that file is edited by
the density lane, and this packet must not collide with it.

Usage: run_site_check.py [--out DIR]
Exit code 0 and a SITE_INFRASTRUCTURE_PASS line on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")

SOURCES = [
    os.path.join(FP, "ForestSiteProfile.cs"),
    os.path.join(FP, "SpeciesSiteResponse.cs"),
    os.path.join(FP, "ScenarioSiteProfiles.cs"),
    os.path.join(HERE, "OfflineLogic", "SiteInfrastructureOfflineCheck.cs"),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>SiteInfrastructureOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-site-infrastructure-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    for source in SOURCES:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    os.makedirs(args.out, exist_ok=True)
    items = "\n".join('    <Compile Include="%s" />' % s for s in SOURCES)
    with open(os.path.join(args.out, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    if "warning" in build.stdout.lower():
        sys.stdout.write(build.stdout)
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())

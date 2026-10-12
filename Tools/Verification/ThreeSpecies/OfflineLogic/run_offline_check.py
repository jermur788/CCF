#!/usr/bin/env python3
"""Build and run the three-species offline logic checks on the plain .NET SDK.

Suites (default: all):
  density  MixedDensityModel (growth model 2) against the original Sitka route.
  timber   Oak/Beech assortment ladders, prices and valuation against the real allocator and work
           calculator, plus proof that Sitka allocation is identical to the unmodified base commit.

Each suite compiles PRODUCTION sources unchanged (none of them uses UnityEngine) together with its check
file in a temporary project OUTSIDE the repository, then runs it. No Unity Editor, no Library, nothing
written inside the repo.

This proves pure arithmetic and decision rules only. It does not prove controller wiring, scene state, the
Unity-side economy adapter or any Unity anchor hash.

Usage: run_offline_check.py [--suite density|timber|all] [--out DIR]
Exit code 0 and *_PASS tokens on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import glob
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")

SUITES = {
    "density": dict(
        startup="MixedDensityOfflineCheck",
        sources=[os.path.join(FP, "SitkaGrowthModel.cs"), os.path.join(FP, "MixedDensityModel.cs"),
                 os.path.join(HERE, "MixedDensityOfflineCheck.cs")],
        args=[ROOT]),
    "timber": dict(
        startup="BroadleafTimberOfflineCheck",
        sources=sorted(glob.glob(os.path.join(FP, "TimberYield", "*.cs")) + glob.glob(os.path.join(FP, "WorkEconomy", "*.cs")))
                + [os.path.join(HERE, "SitkaYieldFixture.cs"), os.path.join(HERE, "BroadleafTimberOfflineCheck.cs")],
        args=[]),
}

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>{startup}</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def run_suite(name, out, dotnet):
    suite = SUITES[name]
    work = os.path.join(out, name)
    os.makedirs(work, exist_ok=True)
    for source in suite["sources"]:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    items = "\n".join('    <Compile Include="%s" />' % s for s in suite["sources"])
    with open(os.path.join(work, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(startup=suite["startup"], items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--"] + suite["args"], cwd=work, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--suite", choices=["density", "timber", "all"], default="all")
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-three-species-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    names = ["density", "timber"] if args.suite == "all" else [args.suite]
    status = 0
    for name in names:
        code = run_suite(name, args.out, dotnet)
        status = status or code
    return status


if __name__ == "__main__":
    sys.exit(main())

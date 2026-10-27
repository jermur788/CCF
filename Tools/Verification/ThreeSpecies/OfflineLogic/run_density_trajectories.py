#!/usr/bin/env python3
"""Build and run the reduced stand simulator (DensityTrajectorySim.cs / DensityCalibrationSearch.cs) on the plain .NET SDK.

It compiles the PRODUCTION density sources unchanged with the two tools files in a temporary project outside the repository.
Nothing is written inside the repo and no Unity is involved. See the header of DensityCalibrationSearch.cs for the commands.

Usage: run_density_trajectories.py [--out DIR] <command> [arguments...]
Examples:
  run_density_trajectories.py hindcast <dir with longrun-E80-300y.csv and longrun-L40-300y.csv>
  run_density_trajectories.py search
  run_density_trajectories.py compare 0.35 0.40 2
  run_density_trajectories.py diagnose
  run_density_trajectories.py dump B_BEECH E80 v3 0.35 0.40 2
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")
SOURCES = [os.path.join(FP, n) for n in ("SitkaGrowthModel.cs", "MixedDensityModel.cs", "MixedDensityModel.Composite.cs", "SitkaSubsetDensitySignal.cs")] + \
          [os.path.join(HERE, "DensityTrajectorySim.cs"), os.path.join(HERE, "DensityCalibrationSearch.cs")]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>DensityCalibrationSearch</StartupObject>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser(add_help=False)
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-density-trajectories"))
    known, rest = parser.parse_known_args()
    dotnet = shutil.which("dotnet") or sys.exit("dotnet SDK not found")
    os.makedirs(known.out, exist_ok=True)
    with open(os.path.join(known.out, "sim.csproj"), "w") as handle:
        handle.write(PROJECT.format(items="\n".join('    <Compile Include="%s" />' % s for s in SOURCES)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1",
               CCF_RECRUITMENT_SERIES=os.path.join(HERE, "recruitment-series-v2.csv"))
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=known.out, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout); sys.stderr.write(build.stderr)
        return build.returncode
    return subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--"] + rest, cwd=known.out, env=env).returncode


if __name__ == "__main__":
    sys.exit(main())

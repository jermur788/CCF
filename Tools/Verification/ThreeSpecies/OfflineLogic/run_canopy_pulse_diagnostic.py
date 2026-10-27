#!/usr/bin/env python3
"""Build and run the canopy-pulse decomposition (CanopyPulseDiagnostic.cs) on the plain .NET SDK.

Compiles the PRODUCTION density sources unchanged, plus the reduced stand simulator, in a temporary project OUTSIDE the
repository. No Unity, nothing written inside the repo.

Usage: run_canopy_pulse_diagnostic.py [--out DIR] one  <fixture> <L40|E80> <smooth|pulse:P|gap:A-B|none> [seed] [top N] [csv]
       run_canopy_pulse_diagnostic.py [--out DIR] scan <L40|E80> <fixture,..> <mode,..> <seeds N>
Examples: run_canopy_pulse_diagnostic.py one B_BEECH_DENSE L40 pulse:5
          run_canopy_pulse_diagnostic.py scan E80 B_BEECH_DENSE,F_BEECH_YOUNG smooth,pulse:5 6
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
          [os.path.join(HERE, "DensityTrajectorySim.cs"), os.path.join(HERE, "DensityCalibrationSearch.cs"), os.path.join(HERE, "CanopyPulseDiagnostic.cs")]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>CanopyPulseDiagnostic</StartupObject>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser(add_help=False)
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-canopy-pulse"))
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

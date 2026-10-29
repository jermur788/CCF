#!/usr/bin/env python3
"""MFM-03 coverage calibration: build the measurement-only exporter, run it, then run the analysis.

Measurement tooling only. It compiles the UNMODIFIED production geometry/competition sources together with runner/CoverageExport.cs
and the existing offline ForestEcologyController stand-in, in a temporary project OUTSIDE the repository, exports per-tree facts
for the frozen MFM-02 V2 fixture stand, and then runs analyse_coverage.py and write_tables.py (Python standard library only). No Unity, no Library,
nothing written inside the repository. Durable output goes to the Files CCF Verification tree.

Usage: run_coverage_calibration.py [--out DIR]     Exit 0 and MFM03_EXPORT_PASS / MFM03_ANALYSIS_PASS on success. Needs dotnet 8+.
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
GAME = os.path.join(ROOT, "Assets", "ForestPrototype")

# Unmodified production sources (all free of UnityEngine) plus the existing offline Hegyi stand-in, whose parity with production the
# exporter re-checks against the real source text.
SOURCES = [
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackSampleGeometry.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackBay.cs"),
    os.path.join(GAME, "ScenarioOne", "CompetitionShare.cs"),
    os.path.join(GAME, "Racks", "RackGeometry.cs"),
    os.path.join(ROOT, "Tools", "Verification", "ScenarioOneNonUi", "OfflineLogic", "OfflineStubs.cs"),
    os.path.join(HERE, "runner", "CoverageExport.cs"),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>CoverageExport</StartupObject>
    <NoWarn>CS0649;CS0169;CS0414</NoWarn>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default="/media/jer/Files/CCF/Verification/MFM03-CoverageCalibration-20261029")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    for source in SOURCES:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    os.makedirs(args.out, exist_ok=True)
    export = os.path.join(args.out, "export.json")
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build_dir = tempfile.mkdtemp(prefix="mfm03-export-")
    try:
        with open(os.path.join(build_dir, "export.csproj"), "w") as handle:
            handle.write(PROJECT.format(items="\n".join('    <Compile Include="%s" />' % s for s in SOURCES)))
        build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=build_dir, env=env, capture_output=True, text=True)
        if build.returncode != 0:
            sys.stdout.write(build.stdout)
            sys.stderr.write(build.stderr)
            return build.returncode
        run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT, os.path.join(HERE, "inputs", "fixture-stand.csv"), export],
                             cwd=build_dir, env=env, capture_output=True, text=True)
        sys.stdout.write(run.stdout)
        sys.stderr.write(run.stderr)
        if run.returncode != 0:
            return run.returncode
    finally:
        shutil.rmtree(build_dir, ignore_errors=True)
    analysis = subprocess.run([sys.executable, "-I", "-B", os.path.join(HERE, "analyse_coverage.py"), export, args.out, ROOT])
    if analysis.returncode != 0:
        return analysis.returncode
    tables = subprocess.run([sys.executable, "-I", "-B", os.path.join(HERE, "write_tables.py"), args.out])
    return tables.returncode


if __name__ == "__main__":
    sys.exit(main())

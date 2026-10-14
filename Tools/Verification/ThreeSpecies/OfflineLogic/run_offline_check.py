#!/usr/bin/env python3
"""Build and run the three-species offline logic checks on the plain .NET SDK.

Suites (default: all):
  density  Growth model 2 density routes: pure-Sitka routing and legacy-route fingerprints, the dimensionless
           species-aware occupancy model (pure Oak, pure Beech, mixed fixtures, generic mortality, calibration),
           plus composite continuity/long-run checks and two isolated compiles that prove arithmetic-module isolation:
             density-isolated        MixedDensityModel.cs + its composite planner WITHOUT SitkaGrowthModel.cs
             density-sitka-isolated  SitkaGrowthModel.cs WITHOUT MixedDensityModel.cs
  history  Derived Scenario One management history (S1-C) against the real persisted record classes.
  timber   Oak/Beech assortment ladders, prices and valuation against the real allocator and work
           calculator, plus proof that Sitka allocation is identical to the unmodified base commit.

Each suite compiles PRODUCTION sources unchanged (none of them uses UnityEngine) together with its check
file in a temporary project OUTSIDE the repository, then runs it. No Unity Editor, no Library, nothing
written inside the repo.

This proves pure arithmetic and decision rules only. It does not prove controller wiring, scene state, the
Unity-side economy adapter or any Unity anchor hash.

Usage: run_offline_check.py [--suite density|density-composite|density-isolated|density-sitka-isolated|timber|history|all] [--out DIR]
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
    "density-composite": dict(startup="CompositeDensityOfflineCheck", sources=[os.path.join(FP,"SitkaGrowthModel.cs"),os.path.join(FP,"MixedDensityModel.cs"),os.path.join(FP,"MixedDensityModel.Composite.cs"),os.path.join(FP,"SitkaSubsetDensitySignal.cs"),os.path.join(HERE,"CompositeDensityOfflineCheck.cs")],args=[]),
    "density": dict(
        startup="MixedDensityOfflineCheck",
        sources=[os.path.join(FP, "SitkaGrowthModel.cs"), os.path.join(FP, "MixedDensityModel.cs"),
                 os.path.join(HERE, "MixedDensityOfflineCheck.cs")],
        args=[ROOT]),
    "density-isolated": dict(
        startup="MixedDensityIsolatedCheck",
        sources=[os.path.join(FP, "MixedDensityModel.cs"), os.path.join(FP,"MixedDensityModel.Composite.cs"), os.path.join(HERE, "MixedDensityIsolatedCheck.cs")],
        args=[]),
    "density-sitka-isolated": dict(
        startup="SitkaIsolatedCheck",
        sources=[os.path.join(FP, "SitkaGrowthModel.cs"), os.path.join(HERE, "SitkaIsolatedCheck.cs")],
        args=[]),
    "history": dict(
        startup="ScenarioHistoryOfflineCheck",
        sources=[os.path.join(FP, "ScenarioOne", "ScenarioInterventionHistory.cs"), os.path.join(FP, "ScenarioOne", "ManagementHistoryText.cs"),
                 os.path.join(FP, "ScenarioOne", "ScenarioManagementEvent.cs"), os.path.join(FP, "ScenarioOne", "ScenarioEcologicalSnapshot.cs"),
                 os.path.join(FP, "ScenarioOne", "ScenarioDeadwoodRecord.cs"), os.path.join(FP, "ScenarioOne", "ScenarioOneWorkOrder.cs"),
                 os.path.join(FP, "ForestSaveData.cs")]
                + sorted(glob.glob(os.path.join(FP, "WorkEconomy", "*.cs")))
                + [os.path.join(HERE, "UnityShim.cs"), os.path.join(HERE, "HistoryOfflineStubs.cs"), os.path.join(HERE, "ScenarioHistoryOfflineCheck.cs")],
        args=[]),
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
{packages}  </ItemGroup>
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
        packages = "".join('    <PackageReference Include="%s" Version="%s" />\n' % pkg for pkg in suite.get("packages", []))
        handle.write(PROJECT.format(startup=suite["startup"], items=items, packages=packages))
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
    parser.add_argument("--suite", choices=["density", "density-composite", "density-isolated", "density-sitka-isolated", "timber", "history", "all"], default="all")
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-three-species-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    # "density" always includes its two compile-time isolation proofs.
    groups = {"density": ["density", "density-composite", "density-isolated", "density-sitka-isolated"],
              "all": ["density", "density-composite", "density-isolated", "density-sitka-isolated", "timber", "history"]}
    names = groups.get(args.suite, [args.suite])
    status = 0
    for name in names:
        code = run_suite(name, args.out, dotnet)
        status = status or code
    return status


if __name__ == "__main__":
    sys.exit(main())

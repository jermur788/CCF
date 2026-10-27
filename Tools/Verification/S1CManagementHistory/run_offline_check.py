#!/usr/bin/env python3
"""Build and run the S1-C per-year management history offline check on the plain .NET SDK.

Compiles PRODUCTION sources unchanged (none of them needs the Unity runtime):
  ScenarioManagementHistory.cs (the T-01 query layer), ScenarioInterventionHistory.cs, the real persisted record
  classes, the real StormModel.cs/SimulationRandom.cs and the WorkEconomy enums, together with this folder's Unity
  shim, save-schema stubs and the check itself, in a temporary project OUTSIDE the repository. No Unity Editor, no
  Library, nothing written inside the repo.

This proves the pure query/derivation rules only. It does not prove manager wiring, UI or any Unity anchor hash.

Usage: run_offline_check.py [--out DIR]
Exit code 0 and the S1C_HISTORY_QUERY_PASS token on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import glob
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")
ONE = os.path.join(FP, "ScenarioOne")

SOURCES = [
    os.path.join(ONE, "ScenarioManagementHistory.cs"),
    os.path.join(ONE, "ScenarioInterventionHistory.cs"),
    os.path.join(ONE, "ManagementHistoryText.cs"),
    os.path.join(ONE, "ScenarioManagementEvent.cs"),
    os.path.join(ONE, "ScenarioEcologicalSnapshot.cs"),
    os.path.join(ONE, "ScenarioDeadwoodRecord.cs"),
    os.path.join(ONE, "ScenarioOneWorkOrder.cs"),
    os.path.join(FP, "ForestSaveData.cs"),
    os.path.join(FP, "StormModel.cs"),
    os.path.join(FP, "SimulationRandom.cs"),
] + sorted(glob.glob(os.path.join(FP, "WorkEconomy", "*.cs"))) + [
    os.path.join(HERE, "UnityShim.cs"),
    os.path.join(HERE, "HistoryOfflineStubs.cs"),
    os.path.join(HERE, "S1CPerYearHistoryOfflineCheck.cs"),
]

# Generated compile-time bridge, written into the temporary project (never the repository).
# post-rack, ScenarioOneWorkOrder.cs labels map squares through ScenarioOneUiFacts.SquareLabel, which needs a live
# ForestEcologyController and UI Toolkit and is not in the source list. With no ecology production returns its "#N"
# fallback, and an offline run has none, so shim exactly that. The assert keeps the shim honest: it fails if production
# changes the signature or the fallback (same approach as Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py).
def bridge_ui_facts():
    path = os.path.join(FP, "UI", "ScenarioOneUiFacts.cs")
    with open(path) as handle:
        facts = handle.read()
    if "public static string SquareLabel(int cellIndex)" not in facts or '"#" + cellIndex' not in facts:
        sys.exit("ScenarioOneUiFacts.SquareLabel signature or no-ecology fallback changed in " + path + "; update the offline shim")
    return "UiFactsBridge.cs", "public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => \"#\" + cellIndex; }\n"


PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>S1CPerYearHistoryOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s1c-history-query-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")

    work = args.out
    os.makedirs(work, exist_ok=True)
    for source in SOURCES:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    sources = list(SOURCES)
    filename, text = bridge_ui_facts()
    bridge_path = os.path.join(work, filename)
    with open(bridge_path, "w") as handle:
        handle.write(text)
    sources.append(bridge_path)

    items = "\n".join('    <Compile Include="%s" />' % s for s in sources)
    with open(os.path.join(work, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=items))

    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=ROOT)
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())

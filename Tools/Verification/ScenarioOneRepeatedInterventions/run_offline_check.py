#!/usr/bin/env python3
"""Build and run the S1-D T-02 advisory cycle-state offline check on the plain .NET SDK.

Compiles PRODUCTION sources unchanged (none of them needs the Unity runtime):
  ScenarioCycleState.cs, ScenarioInterventionHistory.cs, ScenarioOneCompletionRules.cs, the real persisted record classes
  and the WorkEconomy enums, together with the S1-C offline folder's Unity shim and save-schema stubs and this folder's
  check, in a temporary project OUTSIDE the repository. No Unity Editor, no Library, nothing written inside the repo.

This proves the pure derivation rules only. It does not prove manager wiring beyond the source-text checks, UI, or any Unity
behaviour.

--mutations additionally compiles deliberately broken copies of ScenarioCycleState.cs (also outside the repo) and requires the
check to FAIL for each one, so a green run is shown to be able to go red.

Usage: run_offline_check.py [--out DIR] [--mutations]
Exit code 0 and the CYCLE_STATE_OFFLINE_PASS token on success. Needs `dotnet` (SDK 8+).
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
S1C = os.path.join(HERE, "..", "S1CManagementHistory")
CYCLE_SOURCE = os.path.join(ONE, "ScenarioCycleState.cs")

SOURCES = [
    "{cycle}",
    os.path.join(ONE, "ScenarioInterventionHistory.cs"),
    os.path.join(ONE, "ScenarioOneCompletionRules.cs"),
    os.path.join(ONE, "ScenarioManagementEvent.cs"),
    os.path.join(ONE, "ScenarioEcologicalSnapshot.cs"),
    os.path.join(ONE, "ScenarioDeadwoodRecord.cs"),
    os.path.join(ONE, "ScenarioOneWorkOrder.cs"),
    os.path.join(FP, "ForestSaveData.cs"),
    os.path.join(FP, "StormModel.cs"),
    os.path.join(FP, "SimulationRandom.cs"),
] + sorted(glob.glob(os.path.join(FP, "WorkEconomy", "*.cs"))) + [
    os.path.join(S1C, "UnityShim.cs"),
    os.path.join(S1C, "HistoryOfflineStubs.cs"),
    os.path.join(HERE, "CycleStateOfflineCheck.cs"),
]

# (name, text to find in ScenarioCycleState.cs, replacement). Each must make the check fail.
MUTATIONS = [
    ("ack ignores event order",
     "&& e.year >= interventionYear && e.eventId > lastResolution)", "&& e.year >= interventionYear)"),
    ("ack ignores intervention year",
     "&& e.year >= interventionYear && e.eventId > lastResolution)", "&& e.eventId > lastResolution)"),
    ("hard-coded three-cycle cap",
     "state.LatestInterventionOrdinal = latest.Ordinal;", "state.LatestInterventionOrdinal = Math.Min(latest.Ordinal, 3);"),
    ("a fixed-interval verdict appears",
     "public int ResolvedInterventionCount;", "public int ResolvedInterventionCount;\n    public bool ShouldIntervene => YearsSinceLatestIntervention >= 3;"),
    ("approved and pending are merged",
     "var pending = open.Where(o => o.status == ScenarioWorkStatus.Pending).ToList();", "var pending = open;"),
    ("a state with no explanation",
     'state.YearsSince.Reason = "No intervention yet, so there is nothing to count years from.";', 'state.YearsSince.Reason = "";'),
    ("the check mutates an input",
     "var open = orders.Where(o => o != null && o.IsOpen).OrderBy(o => o.workOrderId).ToList();",
     "var open = orders.Where(o => o != null && o.IsOpen).OrderBy(o => o.workOrderId).ToList();\n        foreach (var o in open) if (o.createdYear < currentYear - 5) o.status = ScenarioWorkStatus.Failed;"),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>CycleStateOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


# post-rack, ScenarioOneWorkOrder.cs labels map squares through ScenarioOneUiFacts.SquareLabel, which needs a live ecology
# controller. Shim exactly the no-ecology fallback and assert production still has that signature (as the S1-C check does).
def bridge_ui_facts():
    path = os.path.join(FP, "UI", "ScenarioOneUiFacts.cs")
    with open(path) as handle:
        facts = handle.read()
    if "public static string SquareLabel(int cellIndex)" not in facts or '"#" + cellIndex' not in facts:
        sys.exit("ScenarioOneUiFacts.SquareLabel signature or no-ecology fallback changed in " + path + "; update the offline shim")
    return "UiFactsBridge.cs", "public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => \"#\" + cellIndex; }\n"


def build_and_run(dotnet, work, cycle_source):
    os.makedirs(work, exist_ok=True)
    sources = [cycle_source if s == "{cycle}" else s for s in SOURCES]
    for source in sources:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    filename, text = bridge_ui_facts()
    bridge_path = os.path.join(work, filename)
    with open(bridge_path, "w") as handle:
        handle.write(text)
    sources.append(bridge_path)
    items = "\n".join('    <Compile Include="%s" />' % s for s in sources)
    with open(os.path.join(work, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=ROOT, CCF_CYCLE_SOURCE=cycle_source)
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        return "BUILD_FAILED", build.stdout + build.stderr
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    return ("OK" if run.returncode == 0 else "CHECK_FAILED"), run.stdout + run.stderr


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s1d-cycle-state-offline"))
    parser.add_argument("--mutations", action="store_true", help="also prove the check fails against deliberately broken copies")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")

    status, output = build_and_run(dotnet, args.out, CYCLE_SOURCE)
    sys.stdout.write(output)
    if status != "OK":
        return 1
    if not args.mutations:
        return 0

    with open(CYCLE_SOURCE) as handle:
        original = handle.read()
    survived = []
    for index, (name, find, replace) in enumerate(MUTATIONS):
        if original.count(find) != 1:
            sys.exit("mutation '%s': expected exactly one match in ScenarioCycleState.cs, found %d" % (name, original.count(find)))
        mutant_dir = os.path.join(args.out, "mutant%d" % index)
        os.makedirs(mutant_dir, exist_ok=True)
        mutant_source = os.path.join(mutant_dir, "ScenarioCycleState.cs")
        with open(mutant_source, "w") as handle:
            handle.write(original.replace(find, replace))
        mutant_status, mutant_output = build_and_run(dotnet, mutant_dir, mutant_source)
        failing = [line for line in mutant_output.splitlines() if line.startswith("FAIL")]
        if mutant_status == "CHECK_FAILED":
            print("MUTATION CAUGHT   %-38s %d failing check(s), e.g. %s" % (name, len(failing), failing[0][:110] if failing else "?"))
        else:
            print("MUTATION SURVIVED %-38s (%s)" % (name, mutant_status))
            survived.append(name)
    print("CYCLE_STATE_MUTATIONS_%s caught=%d survived=%d" % ("PASS" if not survived else "FAIL", len(MUTATIONS) - len(survived), len(survived)))
    return 0 if not survived else 1


if __name__ == "__main__":
    sys.exit(main())

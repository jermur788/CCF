#!/usr/bin/env python3
"""Build and run the S1-D T-06 management-event ID integrity offline check on the plain .NET SDK.

Compiles PRODUCTION sources unchanged (ForestSaveValidation*.cs and the real save-record classes) with the shared
Unity shim and N3 save-neighbour stubs, in a temporary project OUTSIDE the repository. No Unity Editor, no Library,
nothing written inside the repo. Needs `dotnet` (SDK 8+) and the Newtonsoft.Json 13.0.3 NuGet package.

--mutations additionally compiles deliberately broken copies of ForestSaveValidation.cs (also outside the repo) and
requires the check to FAIL for each one, so a green run is shown to be able to go red.

This proves the validator rules and the controller's single-seam wiring only. It does not prove Unity behaviour,
CanLoad/LoadData at runtime, disk bytes or world non-mutation: see SaveEventIdIntegrityVerification.cs.

Usage: run_event_id_integrity_offline.py [--out DIR] [--mutations]
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
ONE = os.path.join(FP, "ScenarioOne")
VERIFICATION = os.path.join(ROOT, "Tools", "Verification")
N3 = os.path.join(VERIFICATION, "ScenarioOneCompletion", "N3", "OfflineLogic")
VALIDATION_SOURCE = os.path.join(FP, "ForestSaveValidation.cs")

SOURCES = [
    "{validation}",
    os.path.join(FP, "ForestSaveValidation.Geometry.cs"),
    os.path.join(FP, "ForestSaveValidation.Storms.cs"),
    os.path.join(FP, "ForestSaveValidation.RackV2.cs"),
    os.path.join(ONE, "ScenarioManagementEvent.cs"),
    os.path.join(ONE, "ScenarioOneWorkOrder.cs"),
    os.path.join(ONE, "ScenarioEcologicalSnapshot.cs"),
    os.path.join(ONE, "ScenarioDeadwoodRecord.cs"),
    os.path.join(FP, "ForestSaveData.cs"),
    os.path.join(FP, "RackPlanningModel.cs"),
    os.path.join(FP, "StandGeometryModel.cs"),
    os.path.join(FP, "RegenerationModel.cs"),
    os.path.join(FP, "SitkaGrowthModel.cs"),
] + sorted(glob.glob(os.path.join(FP, "WorkEconomy", "*.cs"))) + [
    os.path.join(N3, "SaveNeighbours.cs"),
    os.path.join(HERE, "EventIdIntegrityOfflineCheck.cs"),
]

# (name, text to find in ForestSaveValidation.cs, replacement). Each must make the check fail.
MUTATIONS = [
    ("zero id allowed", "if (entry.eventId <= 0)", "if (entry.eventId < 0)"),
    ("negative id allowed", "if (entry.eventId <= 0)", "if (entry.eventId == 0)"),
    ("duplicate id allowed",
     "            if (firstRecordForId.TryGetValue(entry.eventId, out int first))\n"
     "                return $\"management event ID {entry.eventId} appears twice (records {first} and {i})\";\n"
     "            firstRecordForId.Add(entry.eventId, i);",
     "            firstRecordForId[entry.eventId] = i;"),
    ("rule never wired into Validate", "return ValidateManagementEventIds(data.scenarioOne);", "return null;"),
    ("rule runs before older checks", "        string geometryProblem = ValidateGeometry(data);",
     "        string earlyIds = ValidateManagementEventIds(data.scenarioOne);\n"
     "        if (earlyIds != null) return earlyIds;\n        string geometryProblem = ValidateGeometry(data);"),
    ("null list rejected", "if (scenario == null || scenario.managementEvents == null) return null;",
     "if (scenario == null) return null;\n        if (scenario.managementEvents == null) return \"no event list\";"),
    ("repairs by sorting the list", "firstRecordForId.Add(entry.eventId, i);",
     "firstRecordForId.Add(entry.eventId, i); scenario.managementEvents.Sort((a, b) => a.eventId.CompareTo(b.eventId));"),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>EventIdIntegrityOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
{items}
  </ItemGroup>
</Project>
"""


def write_shim(work):
    # Same shim as the N3 offline runner: the three-species Unity shim plus the Mathf rounding helpers.
    shim = open(os.path.join(VERIFICATION, "ThreeSpecies", "OfflineLogic", "UnityShim.cs")).read()
    needle = "public static class Mathf\n    {"
    if needle not in shim:
        sys.exit("UnityShim.cs layout changed; update the Mathf patch in this runner")
    shim = shim.replace(needle, needle + "\n        public static int RoundToInt(float v)=>(int)Math.Round(v);\n"
                        "        public static int CeilToInt(float v)=>(int)Math.Ceiling(v);\n"
                        "        public static int FloorToInt(float v)=>(int)Math.Floor(v);")
    path = os.path.join(work, "UnityShim.cs")
    with open(path, "w") as handle:
        handle.write(shim)
    # ScenarioOneWorkOrder.cs labels squares through ScenarioOneUiFacts.SquareLabel, which needs a live ecology controller.
    # Shim exactly its no-ecology fallback, and refuse to run if production stops matching it.
    facts = open(os.path.join(FP, "UI", "ScenarioOneUiFacts.cs")).read()
    if "public static string SquareLabel(int cellIndex)" not in facts or '"#" + cellIndex' not in facts:
        sys.exit("ScenarioOneUiFacts.SquareLabel signature or no-ecology fallback changed; update the offline shim")
    bridge = os.path.join(work, "UiFactsBridge.cs")
    with open(bridge, "w") as handle:
        handle.write('public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => "#" + cellIndex; }\n')
    return [path, bridge]


def build_and_run(dotnet, work, validation_source):
    os.makedirs(work, exist_ok=True)
    sources = [validation_source if s == "{validation}" else s for s in SOURCES]
    for source in sources:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    sources += write_shim(work)
    items = "\n".join('    <Compile Include="%s" />' % s for s in sources)
    with open(os.path.join(work, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=ROOT)
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        return "BUILD_FAILED", build.stdout + build.stderr
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    return ("OK" if run.returncode == 0 else "CHECK_FAILED"), run.stdout + run.stderr


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s1d-event-id-offline"))
    parser.add_argument("--mutations", action="store_true", help="also prove the check fails against deliberately broken copies")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")

    status, output = build_and_run(dotnet, args.out, VALIDATION_SOURCE)
    sys.stdout.write(output)
    if status != "OK":
        return 1
    if not args.mutations:
        return 0

    with open(VALIDATION_SOURCE) as handle:
        original = handle.read()
    survived = []
    for index, (name, find, replace) in enumerate(MUTATIONS):
        if original.count(find) != 1:
            sys.exit("mutation '%s': expected exactly one match in ForestSaveValidation.cs, found %d" % (name, original.count(find)))
        mutant_dir = os.path.join(args.out, "mutant%d" % index)
        os.makedirs(mutant_dir, exist_ok=True)
        mutant_source = os.path.join(mutant_dir, "ForestSaveValidation.cs")
        with open(mutant_source, "w") as handle:
            handle.write(original.replace(find, replace))
        mutant_status, mutant_output = build_and_run(dotnet, mutant_dir, mutant_source)
        failing = [line for line in mutant_output.splitlines() if line.startswith("FAIL")]
        if mutant_status == "CHECK_FAILED":
            print("MUTATION CAUGHT   %-34s %d failing check(s), e.g. %s" % (name, len(failing), failing[0][:100] if failing else "?"))
        else:
            print("MUTATION SURVIVED %-34s (%s)" % (name, mutant_status))
            survived.append(name)
    print("EVENT_ID_MUTATIONS_%s caught=%d survived=%d" % ("PASS" if not survived else "FAIL", len(MUTATIONS) - len(survived), len(survived)))
    return 0 if not survived else 1


if __name__ == "__main__":
    sys.exit(main())

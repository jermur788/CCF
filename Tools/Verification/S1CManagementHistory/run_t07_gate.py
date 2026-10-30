#!/usr/bin/env python3
"""Run the S1-C T-07 data-contract stability gate offline on the plain .NET SDK.

Compiles PRODUCTION sources unchanged (the per-year history reader, the real persisted record classes, the real
ScenarioOneSaveMigration, the real ForestSaveValidation with the accepted S1-D T-06 event-ID rule, and the real
StormModel) together with this folder's shims and the T-07 check, in a temporary project OUTSIDE the repository.
No Unity Editor, no Library, nothing written inside the repo.

The old save is a genuine pre-S1-C v20-y5 world copied from existing durable evidence; the current save is the genuine
recorded v22 year-3 save from the S1-C T-05 evidence. Both digests are pinned below, so a differently-labelled file
cannot silently replace them.

--mutations additionally compiles deliberately broken copies of the history reader and the event-ID validator, and
requires the T-07 check to FAIL for every one. A green run is therefore shown to be able to go red.

Usage:
  run_t07_gate.py --old-save PATH --current-save PATH --out DIR [--mutations]
Exit code 0 and the S1C_T07_OFFLINE_PASS token on success. Needs `dotnet` (SDK 8+); the Newtonsoft.Json package is
restored by the existing T-06 offline workflow.
"""
import argparse
import glob
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import time

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
FP = ROOT / "Assets" / "ForestPrototype"
ONE = FP / "ScenarioOne"

OLD_SHA256 = "af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00"
CURRENT_SHA256 = "95e5dd37d7f546d78cb94cda8e5af1641b576c925cc37313ce88aa0a54167b9e"

HISTORY = ONE / "ScenarioManagementHistory.cs"
INTERVENTION = ONE / "ScenarioInterventionHistory.cs"
VALIDATION = FP / "ForestSaveValidation.cs"

SOURCES = [
    "{history}",
    str(INTERVENTION),
    str(ONE / "ScenarioManagementEvent.cs"),
    str(ONE / "ScenarioEcologicalSnapshot.cs"),
    str(ONE / "ScenarioDeadwoodRecord.cs"),
    str(ONE / "ScenarioOneWorkOrder.cs"),
    str(ONE / "ScenarioOneSaveMigration.cs"),
    str(FP / "ForestSaveData.cs"),
    "{validation}",
    str(FP / "ForestSaveValidation.Geometry.cs"),
    str(FP / "ForestSaveValidation.Storms.cs"),
    str(FP / "ForestSaveValidation.RackV2.cs"),
    str(FP / "RackPlanningModel.cs"),
    str(FP / "StandGeometryModel.cs"),
    str(FP / "RegenerationModel.cs"),
    str(FP / "SitkaGrowthModel.cs"),
    str(FP / "StormModel.cs"),
    str(FP / "SimulationRandom.cs"),
] + sorted(glob.glob(str(FP / "WorkEconomy" / "*.cs"))) + [
    "UnityShim.cs",
    "UiFactsBridge.cs",
    str(HERE / "T07OfflineStubs.cs"),
    str(HERE / "S1CDataContractStabilityOfflineCheck.cs"),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>S1CDataContractStabilityOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
{items}
  </ItemGroup>
</Project>
"""

# (name, target file, exact text to find, replacement). Every mutation must make the T-07 check fail.
MUTATIONS = [
    ("reader renumbers existing events", "history",
     "            view.Events.Add(entry);",
     "            entry.eventId = view.Events.Count + 1;\n            view.Events.Add(entry);"),
    ("reader re-keys work orders", "history",
     "if (order != null && !orderById.ContainsKey(order.workOrderId)) orderById.Add(order.workOrderId, order);",
     "if (order != null && !orderById.ContainsKey(order.workOrderId)) orderById.Add(orderById.Count + 1000, order);"),
    ("cancellation fabricates a replacement ID", "history",
     "                    WorkOrderId = entry.workOrderId,",
     "                    WorkOrderId = entry.workOrderId + 1000000,"),
    ("harvestJobId treated as a unique job identity", "history",
     "            bool grouped = !record.OrderStillExists || record.Order.harvestJobId == view.Year\n"
     "                || record.Order.harvestJobId < 0;",
     "            bool grouped = record.OrderStillExists && record.Order.harvestJobId == record.Event.workOrderId;"),
    ("grouped harvest invents per-order money", "history",
     "        if (succeeded && !harvest)",
     "        if (succeeded)"),
    ("query mutates an input order", "history",
     "            if (order != null && order.resolvedYear == year) view.ResolvedOrders.Add(order);",
     "            if (order != null && order.resolvedYear == year) { order.validationMessage = \"read-touched\"; view.ResolvedOrders.Add(order); }"),
    ("harvest label becomes the order identity", "history",
     "            ResolutionYearLabel = view.Year,",
     "            ResolutionYearLabel = settlement.Event.workOrderId,"),
    ("duplicate event ID accepted", "validation",
     "            if (firstRecordForId.TryGetValue(entry.eventId, out int first))\n"
     "                return $\"management event ID {entry.eventId} appears twice (records {first} and {i})\";\n"
     "            firstRecordForId.Add(entry.eventId, i);",
     "            firstRecordForId[entry.eventId] = i;"),
]


def sha256(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def write_shim(work):
    shim = (HERE / "UnityShim.cs").read_text()
    needle = "public static class Mathf\n    {"
    if needle not in shim:
        raise SystemExit("UnityShim.cs layout changed; update the Mathf patch in run_t07_gate.py")
    shim = shim.replace(needle, needle + "\n        public static int RoundToInt(float v)=>(int)Math.Round(v);\n"
                        "        public static int CeilToInt(float v)=>(int)Math.Ceiling(v);\n"
                        "        public static int FloorToInt(float v)=>(int)Math.Floor(v);")
    (work / "UnityShim.cs").write_text(shim)


def write_bridge(work):
    facts = (FP / "UI" / "ScenarioOneUiFacts.cs").read_text()
    if "public static string SquareLabel(int cellIndex)" not in facts or '"#" + cellIndex' not in facts:
        raise SystemExit("ScenarioOneUiFacts.SquareLabel signature or no-ecology fallback changed; update the offline shim")
    (work / "UiFactsBridge.cs").write_text(
        "public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => \"#\" + cellIndex; }\n")


def build_and_run(dotnet, work, history_source, validation_source):
    work.mkdir(parents=True, exist_ok=True)
    write_shim(work)
    write_bridge(work)
    sources = [s.replace("{history}", str(history_source)).replace("{validation}", str(validation_source)) for s in SOURCES]
    for source in sources:
        candidate = Path(source) if Path(source).is_absolute() else work / source
        if not candidate.is_file():
            return "SOURCE_MISSING", "missing source " + str(candidate)
    items = "\n".join('    <Compile Include="%s" />' % s for s in sources)
    (work / "check.csproj").write_text(PROJECT.format(items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=str(ROOT))
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=str(work), env=env,
                           capture_output=True, text=True)
    if build.returncode != 0:
        return "BUILD_FAILED", build.stdout + build.stderr
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=str(work), env=env,
                         capture_output=True, text=True)
    output = run.stdout + run.stderr
    return ("OK" if run.returncode == 0 else "CHECK_FAILED"), output


def parse_main(output, status):
    token = "S1C_T07_OFFLINE_PASS" if status == "OK" and "S1C_T07_OFFLINE_PASS" in output else "S1C_T07_OFFLINE_FAIL"
    checks = failures = 0
    for line in output.splitlines():
        if line.startswith("S1C_T07_OFFLINE_PASS") or line.startswith("S1C_T07_OFFLINE_FAIL"):
            for part in line.split():
                if part.startswith("checks="):
                    checks = int(part.split("=", 1)[1])
                if part.startswith("failures="):
                    failures = int(part.split("=", 1)[1])
    return {
        "status": status,
        "token": token,
        "checks": checks,
        "failures": failures,
        "pass": status == "OK" and token == "S1C_T07_OFFLINE_PASS" and checks > 0,
    }


def fixture_record(name, path, expected):
    actual = sha256(path)
    return {
        "name": name,
        "path": str(path.resolve()),
        "bytes": Path(path).stat().st_size,
        "sha256": actual,
        "expected_sha256": expected,
        "digest_matches": actual == expected,
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--old-save", type=Path, required=True, help="genuine pre-S1-C v20 rendered save")
    parser.add_argument("--current-save", type=Path, required=True, help="genuine recorded v22 S1-C save")
    parser.add_argument("--out", type=Path, required=True, help="durable evidence folder outside the repository")
    parser.add_argument("--build-dir", type=Path, default=None, help="temporary build folder (default: mkdtemp under /tmp)")
    parser.add_argument("--mutations", action="store_true", help="also prove the check fails against deliberately broken copies")
    parser.add_argument("--keep-build", action="store_true", help="keep the temporary build folder")
    args = parser.parse_args()

    root = ROOT.resolve()
    out = args.out.resolve()
    if out == root or root in out.parents:
        raise SystemExit("Evidence folder must be outside the repository")
    if not args.old_save.is_file():
        raise SystemExit("missing old save: " + str(args.old_save))
    if not args.current_save.is_file():
        raise SystemExit("missing current save: " + str(args.current_save))
    dotnet = shutil.which("dotnet")
    if not dotnet:
        raise SystemExit("dotnet SDK not found")

    old = fixture_record("old-v20-y5", args.old_save, OLD_SHA256)
    current = fixture_record("current-v22-y3", args.current_save, CURRENT_SHA256)
    if not old["digest_matches"]:
        raise SystemExit("old-save digest mismatch: got %s want %s; refusing a differently-labelled fixture" % (old["sha256"], OLD_SHA256))
    if not current["digest_matches"]:
        raise SystemExit("current-save digest mismatch: got %s want %s; refusing a differently-labelled fixture" % (current["sha256"], CURRENT_SHA256))

    os.environ["CCF_T07_OLD_SAVE"] = str(args.old_save.resolve())
    os.environ["CCF_T07_CURRENT_SAVE"] = str(args.current_save.resolve())
    out.mkdir(parents=True, exist_ok=True)
    work = args.build_dir or Path(tempfile.mkdtemp(prefix="ccf-s1c-t07-"))
    work = work.resolve()
    lane_before = subprocess.check_output(["git", "-C", str(root), "status", "--porcelain"], text=True).strip()
    started = time.time()
    print("S1-C T-07 offline gate: building production sources", flush=True)
    status, output = build_and_run(dotnet, work, HISTORY, VALIDATION)
    (out / "t07-offline-stdout.txt").write_text(output)
    main_result = parse_main(output, status)
    print(output, end="" if output.endswith("\n") else "\n", flush=True)

    mutation_results = []
    if args.mutations:
        originals = {"history": HISTORY.read_text(), "validation": VALIDATION.read_text()}
        for index, (name, target, find, replace) in enumerate(MUTATIONS):
            original = originals[target]
            if original.count(find) != 1:
                raise SystemExit("mutation '%s': expected exactly one match in %s, found %d" %
                                 (name, target, original.count(find)))
            mutant_dir = work / ("mutant%d" % index)
            mutant_dir.mkdir(parents=True, exist_ok=True)
            mutant_path = mutant_dir / (HISTORY.name if target == "history" else VALIDATION.name)
            mutant_path.write_text(original.replace(find, replace))
            mutant_status, mutant_output = build_and_run(
                dotnet, mutant_dir,
                mutant_path if target == "history" else HISTORY,
                mutant_path if target == "validation" else VALIDATION)
            failing = [line for line in mutant_output.splitlines() if line.startswith("FAIL")]
            caught = mutant_status == "CHECK_FAILED" and len(failing) > 0
            mutation_results.append({
                "name": name,
                "target": target,
                "status": mutant_status,
                "caught": caught,
                "failing_count": len(failing),
                "failing_example": failing[0] if failing else "",
            })
            verdict = "MUTATION_CAUGHT" if caught else "MUTATION_NOT_CAUGHT"
            print("%-14s %-42s %s (%s, %d failing check(s))" % (
                verdict, name, mutant_status, failing[0][:90] if failing else "no failing check", len(failing)), flush=True)

    lane_after = subprocess.check_output(["git", "-C", str(root), "status", "--porcelain"], text=True).strip()
    head = subprocess.check_output(["git", "-C", str(root), "rev-parse", "HEAD"], text=True).strip()
    branch = subprocess.check_output(["git", "-C", str(root), "rev-parse", "--abbrev-ref", "HEAD"], text=True).strip()
    inspected = [
        HISTORY, INTERVENTION, VALIDATION,
        ONE / "ScenarioManagementEvent.cs", ONE / "ScenarioOneWorkOrder.cs",
        FP / "ForestSaveData.cs", ONE / "ScenarioOneSaveMigration.cs",
        HERE / "S1CDataContractStabilityOfflineCheck.cs", HERE / "T07OfflineStubs.cs",
        HERE / "run_t07_gate.py",
    ]
    all_mutations_caught = not args.mutations or all(item["caught"] for item in mutation_results)
    pass_result = (main_result["pass"] and all_mutations_caught and lane_before == lane_after)
    result = {
        "task": "S1-C T-07 data-contract stability",
        "generated_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
        "head": head,
        "branch": branch,
        "worktree": str(root),
        "seconds": round(time.time() - started, 2),
        "fixtures": {"old": old, "current": current},
        "source_sha256": {str(p.relative_to(root)): sha256(p) for p in inspected},
        "main": main_result,
        "mutations_requested": args.mutations,
        "mutations": mutation_results,
        "lane_status_before": lane_before,
        "lane_status_after": lane_after,
        "pass": pass_result,
    }
    (out / "t07-offline-result.json").write_text(json.dumps(result, indent=2) + "\n")
    print("S1C_T07_OFFLINE_%s checks=%d failures=%d mutations=%d caught=%d evidence=%s" % (
        "PASS" if pass_result else "FAIL",
        main_result["checks"], main_result["failures"], len(mutation_results),
        sum(1 for item in mutation_results if item["caught"]), out / "t07-offline-result.json"), flush=True)
    if args.keep_build:
        print("build folder kept: " + str(work), flush=True)
    elif args.build_dir is None:
        shutil.rmtree(work, ignore_errors=True)
    return 0 if pass_result else 1


if __name__ == "__main__":
    sys.exit(main())

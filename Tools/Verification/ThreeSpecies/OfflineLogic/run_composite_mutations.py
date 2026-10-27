#!/usr/bin/env python3
"""Mutation tests for the component-faithful density kernel (density v3).

Each mutant changes ONE thing in a copy of the production sources, compiles the copy with the unchanged offline suite
(CompositeDensityOfflineCheck.cs and the reduced simulator) and must be KILLED: the suite must show a FAIL line that the
unmutated control does not show, or crash. The control itself need not be fully green: on the integrated Scenario One candidate
it still carries known reduced-model R2 failures (single-year all-stem loss 0.407-0.423 against 0.40); those names are read from
the control run and are not counted as kills. A survivor is a hole in the suite and stops the run. Nothing is written inside the repository.

Usage: run_composite_mutations.py [--out DIR]
"""
import argparse
import json
import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
FP = ROOT / "Assets/ForestPrototype"
SOURCES = ["SitkaGrowthModel.cs", "MixedDensityModel.cs", "MixedDensityModel.Composite.cs", "SitkaSubsetDensitySignal.cs"]
TOOLS = ["DensityTrajectorySim.cs", "DensityCalibrationSearch.cs", "CompositeDensityOfflineCheck.cs"]

# name -> (file, exact old text, new text, what the mutant stands for)
MUTANTS = {
    "discard-the-sitka-component": ("SitkaSubsetDensitySignal.cs",
        "if (probability > 0.0 && roll(stems[i].TreeId) < probability) result.HazardVictimIndices.Add(i);\n            else survivors.Add(i);",
        "survivors.Add(i);",
        "the Sitka density process demands nothing (the deaths vanish)"),
    "remove-the-sitka-boundary-stage": ("SitkaSubsetDensitySignal.cs",
        "            result.BoundaryVictimIndices.Add(i);", "",
        "the Sitka subset is never pulled back to its line"),
    "broadleaf-enters-the-sitka-density": ("SitkaSubsetDensitySignal.cs",
        "if (stems[i].Demand == GrowingSpaceDemandClass.Reference)\n                subset.Add(i);", "subset.Add(i);",
        "Oak and Beech counted in the Sitka relative density and eligible as Sitka victims"),
    "sitka-debt-dropped-from-the-union": ("MixedDensityModel.Composite.cs",
        "        plan.SitkaVictimIndices.AddRange(sitkaHazardVictims);\n        plan.SitkaVictimIndices.AddRange(sitkaBoundaryVictims);", "",
        "the Sitka victims are computed but never killed"),
    "generic-may-kill-a-sitka-victim-twice": ("MixedDensityModel.Composite.cs",
        "                if (taken[i]) continue;\n", "",
        "the generic draw ignores the stems the Sitka component already took"),
    "untapered-generic-pressure": ("MixedDensityModel.Composite.cs",
        "plan.GenericLossFraction = plan.GenericResponse * plan.NonReferenceShare;", "plan.GenericLossFraction = plan.GenericResponse;",
        "the generic response no longer vanishes with the broadleaf share"),
    "no-generic-mortality": ("MixedDensityModel.Composite.cs",
        "return calibration.MaximumLoss * (1.0 - Math.Exp(-calibration.Strength * Math.Pow(excess, calibration.Exponent)));", "return 0.0;",
        "broadleaves never die of density"),
    "species-biased-allocation": ("MixedDensityModel.Composite.cs",
        "double weight = totalVulnerability > 0.0 ? Vulnerability(Suppression(stems[i].CompetitionIndex)) : 1.0;",
        "double weight = (totalVulnerability > 0.0 ? Vulnerability(Suppression(stems[i].CompetitionIndex)) : 1.0) * Weight(stems[i].Demand);",
        "a class multiplier in the victim choice"),
    "quota-rounded-up": ("MixedDensityModel.Composite.cs",
        "int quota = (int)Math.Floor(expectedCount + 1e-10);\n        double remainder = Math.Max(0.0, expectedCount - quota);\n        if (remainder > 0.0 && genericRoll(GenericQuotaRollKey) < remainder) quota++;",
        "int quota = (int)Math.Ceiling(expectedCount - 1e-10);",
        "any positive expected count always kills a stem (a one-stem step at the first broadleaf)"),
    "response-reverted-to-v2-strength": ("MixedDensityModel.Composite.cs",
        "public const double GenericResponseStrength = 1.60;\n    public const double GenericMaximumAnnualLoss = 0.50;",
        "public const double GenericResponseStrength = 0.20;\n    public const double GenericMaximumAnnualLoss = 0.25;",
        "the weak v2 generic response (pure Beech keeps climbing)"),
    "budget-removed": ("MixedDensityModel.Composite.cs",
        "if (spent + basalArea > budget + 1e-12) { plan.GenericBudgetPassedOver++; continue; }", "",
        "no basal-area budget: the recruit shortage falls on the canopy again"),
    "budget-never-stall-rule-removed": ("MixedDensityModel.Composite.cs",
        "            if (plan.GenericVictimIndices.Count == 0)\n            {\n                plan.GenericVictimIndices.Add(eligible[0]);\n                spent = StemBasalArea(stems[eligible[0]]);\n            }\n", "",
        "a stand of a few large trees is frozen: the generic component takes nobody when no stem fits"),
    "budget-first-stem-always-allowed": ("MixedDensityModel.Composite.cs",
        "if (spent + basalArea > budget + 1e-12) { plan.GenericBudgetPassedOver++; continue; }",
        "if (plan.GenericVictimIndices.Count > 0 && spent + basalArea > budget + 1e-12) { plan.GenericBudgetPassedOver++; continue; }",
        "the first stem may exceed the budget even when a smaller one fits (the looser first version of the rule)"),
    "budget-stops-at-the-first-stem-that-does-not-fit": ("MixedDensityModel.Composite.cs",
        "{ plan.GenericBudgetPassedOver++; continue; }", "{ plan.GenericBudgetPassedOver++; break; }",
        "a large stem ends the step instead of being passed over (the budget is under-used)"),
    "budget-weighted-by-class": ("MixedDensityModel.Composite.cs",
        "double basalArea = StemBasalArea(stems[i]);\n                if (spent", "double basalArea = StemBasalArea(stems[i]) * Weight(stems[i].Demand);\n                if (spent",
        "a class multiplier in the budget"),
    "budget-constant-loosened": ("MixedDensityModel.Composite.cs",
        "public const double GenericAnnualBasalAreaBudgetShare = 0.125;", "public const double GenericAnnualBasalAreaBudgetShare = 0.25;",
        "a budget so loose that the 25 % pulses are allowed again"),
    "budget-measured-on-the-wrong-total": ("MixedDensityModel.Composite.cs",
        "foreach (int i in order) standBasalArea += StemBasalArea(stems[i]);", "foreach (int i in order) standBasalArea += StemBasalArea(stems[i]) * 0.5;",
        "the budget is computed against half the stand's basal area"),
    "response-capped-at-an-occupancy": ("MixedDensityModel.Composite.cs",
        "double excess = Math.Max(0.0, occupancy - OnsetOccupancy);", "double excess = Math.Min(2.0, Math.Max(0.0, occupancy - OnsetOccupancy));",
        "a hidden maximum occupancy beyond which nothing changes"),
}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-density-v3-mutations"))
    parser.add_argument("--only", action="append", default=[], help="run only the named mutant(s) (after the control)")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet") or sys.exit("dotnet SDK not found")
    root = Path(args.out)
    root.mkdir(parents=True, exist_ok=True)
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_RECRUITMENT_SERIES=str(HERE / "recruitment-series-v2.csv"), CCF_REPO_ROOT=str(ROOT))
    results = []
    survivors = []
    # the unmutated control must pass first, or "killed" means nothing
    control = run_one("control (no mutation)", None, root / "control", dotnet, env, expect_pass=True)
    results.append(control)
    known = set(control.get("failing_names", []))
    for name, (file, old, new, meaning) in MUTANTS.items():
        if args.only and name not in args.only:
            continue
        record = run_one(name, (file, old, new), root / name, dotnet, env, expect_pass=False, known=known)
        record["meaning"] = meaning
        results.append(record)
        if not record["killed"]:
            survivors.append(name)
    (root / "results.json").write_text(json.dumps(results, indent=2))
    ran = len(args.only) if args.only else len(MUTANTS)
    print(json.dumps({"mutants": ran, "killed": ran - len(survivors), "survivors": survivors, "control_passed": control.get("control_passed", False),
                      "control_known_failures": sorted(known)}))
    return 0 if not survivors and control.get("control_ran", False) else 1


def run_one(name, mutation, work, dotnet, env, expect_pass, known=frozenset()):
    if work.exists():
        shutil.rmtree(work)
    work.mkdir(parents=True)
    files = []
    for f in SOURCES:
        text = (FP / f).read_text()
        if mutation and mutation[0] == f:
            assert mutation[1] in text, ("mutation target not found", name)
            text = text.replace(mutation[1], mutation[2], 1)
        (work / f).write_text(text)
        files.append(work / f)
    for f in TOOLS:
        shutil.copyfile(HERE / f, work / f)
        files.append(work / f)
    items = "".join('<Compile Include="%s" />' % f for f in files)
    (work / "mutant.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType>'
        '<LangVersion>9.0</LangVersion><EnableDefaultCompileItems>false</EnableDefaultCompileItems><InvariantGlobalization>true</InvariantGlobalization>'
        '<StartupObject>CompositeDensityOfflineCheck</StartupObject></PropertyGroup><ItemGroup>' + items + '</ItemGroup></Project>')
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        print(name, "DID NOT COMPILE", build.stdout[-400:], flush=True)
        return {"mutant": name, "killed": False, "compiled": False, "control_passed": False}
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    (work / "result.txt").write_text(run.stdout + run.stderr)
    failing = [l for l in run.stdout.splitlines() if l.startswith("FAIL ")]
    names = sorted({l.split()[1] for l in failing})
    finished = "COMPOSITE_DENSITY_" in run.stdout
    if expect_pass:
        ok = run.returncode == 0 and "COMPOSITE_DENSITY_PASS" in run.stdout
        print(name, "PASS" if ok else ("RAN WITH KNOWN FAILURES " + str(names) if finished else "DID NOT RUN"), flush=True)
        return {"mutant": name, "control_passed": ok, "control_ran": finished, "failing_names": names, "killed": False}
    new_failures = [n for n in names if n not in known]
    crashed = bool(run.stderr.strip()) and not finished
    killed = bool(new_failures) or crashed
    print(name, "KILLED" if killed else "SURVIVED", f"new_failing_assertions={len(new_failures)}" + (" (by exception)" if crashed else ""), flush=True)
    return {"mutant": name, "killed": killed, "failing_assertions": [l for l in failing if l.split()[1] in new_failures][:12], "by_exception": crashed}

if __name__ == "__main__":
    sys.exit(main())

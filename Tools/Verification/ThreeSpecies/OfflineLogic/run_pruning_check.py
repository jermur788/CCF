#!/usr/bin/env python3
"""PARITY-06 bounded executable pruning/adapter check; no Unity launch.

Compiles the production definition, economy adapter, yield/work modules unchanged.
Extracts the EXACT ForestTree pruning block + restore method into a fixture with
inert visual hooks. Compiles the base block alongside it for differential legacy
checks. Stubs model only Unity data/attributes and unrelated collaborators; this
does NOT verify manager execution, Unity serialization, visuals or lifecycle hashes.
Artifacts go to the required external --out directory. --mutations exercises
negative controls in generated copies only; never edits production or baselines.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
FP = ROOT / "Assets/ForestPrototype"
BASE = "72dbee8b8eab66f58046abfd49e77ead4c29d357"


def base_text(path):
    return subprocess.check_output(["git", "show", f"{BASE}:{path.relative_to(ROOT)}"], cwd=ROOT).decode()


def method(source, signature):
    start = source.index(signature)
    brace = source.index("{", start)
    depth = 1
    end = brace + 1
    while depth:
        depth += (source[end] == "{") - (source[end] == "}")
        end += 1
    return source[start:end]


def fixture(source, name):
    start = source.index("[SerializeField, Min(0)] private int pruningLifts;")
    end = source.index("// Biological timber interface:", start)
    block = source[start:end]
    restore = method(source, "public void RestorePruningHistory(")
    return f"""using UnityEngine;
public sealed class {name} {{
    public TreeSpeciesDefinition species;
    public TreeSpeciesDefinition Species => species;
    public string TreeId = "fixture";
    public bool CanChop = true, IsLiving = true, IsBiologicallyDead = false;
    public string MortalityCause = "";
    public float Height = 24f, Diameter = 60f, crownRadiusMeters = 5f;
    public float BiologicalStemVolumeM3 => Diameter * Diameter * 0.00007854f * Height * 0.5f;
    public float CrownRadius => crownRadiusMeters;
    private void RefreshVisuals() {{ }}
    {restore}
    {block}
}}
"""


def preservation():
    protected = ["SitkaGrowthModel.cs", "MixedDensityModel.cs", "MixedDensityModel.Composite.cs",
                 "SitkaSubsetDensitySignal.cs", "ForestEcologyController.cs", "ForestSaveData.cs",
                 "ForestSaveController.cs", "ForestSaveValidation.cs", "Species/SitkaSpruce.asset",
                 "Species/SessileOak.asset", "Species/BeechSpecies.asset",
                 "TimberYield/TimberYieldCalculator.cs", "TimberYield/TimberYieldEconomyAdapter.cs",
                 "ScenarioOne/ScenarioOneEconomyAdapter.cs", "ScenarioOne/ScenarioOne.asset",
                 "ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes",
                 "ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json"]
    result = {}
    for relative in protected:
        path = FP / relative
        old = subprocess.check_output(["git", "show", f"{BASE}:{path.relative_to(ROOT)}"], cwd=ROOT)
        digest = hashlib.sha256(path.read_bytes()).hexdigest()
        assert digest == hashlib.sha256(old).hexdigest(), f"protected bytes moved: {path}"
        result[relative] = digest
    # Existing Sitka configuration and legacy target arithmetic are byte-exact too.
    for path, signature in [(FP / "TimberYield/TimberYieldConfiguration.cs", "public static TimberYieldConfiguration CreateSitka("),
                            (FP / "WorkEconomy/Stage1EconomyDefaults.cs", "public static ForestryPriceBook Create("),
                            (FP / "ScenarioOne/ScenarioOneDefinition.cs", "public float NextPruningTargetHeightM(int currentLifts)")]:
        assert method(path.read_text(), signature) == method(base_text(path), signature), signature
    # Every production manager pruning consumer must use the model gate; no other manager body may change.
    manager = FP / "ScenarioOne/ScenarioOneManager.cs"
    text = manager.read_text()
    replacements = {
        "definition.NextPruningTargetHeightM(tree.PruningLifts)": "definition.NextPruningTargetHeightM(tree, BroadleafMarketActive)",
        "tree.CanPrune(target, CurrentYear + 1)": "tree.CanPrune(target, CurrentYear + 1, BroadleafMarketActive)",
        "tree.CanPrune(definition.NextPruningTargetHeightM(tree, BroadleafMarketActive), CurrentYear + 1)": "tree.CanPrune(definition.NextPruningTargetHeightM(tree, BroadleafMarketActive), CurrentYear + 1, BroadleafMarketActive)",
        "tree.CanPrune(targetHeight, CurrentYear + 1)": "tree.CanPrune(targetHeight, CurrentYear + 1, BroadleafMarketActive)",
        "tree.TryPrune(targetHeight, report.year)": "tree.TryPrune(targetHeight, report.year, BroadleafMarketActive)",
        "tree.CanPrune(PruningTarget(order), CurrentYear + 1)": "tree.CanPrune(PruningTarget(order), CurrentYear + 1, BroadleafMarketActive)"}
    expected = base_text(manager)
    for old, new in replacements.items():
        assert old in expected, old
        expected = expected.replace(old, new)
    assert text == expected, "manager diff exceeds the approved pruning call sites"
    tree = FP / "ForestTree.cs"
    scrub = tree.read_text()
    old_tree = base_text(tree)
    for signature in ["public string CanPrune(", "public string TryPrune("]:
        scrub = scrub.replace(method(scrub, signature), method(old_tree, signature))
    assert scrub == old_tree, "ForestTree diff exceeds CanPrune/TryPrune"
    print(f"PARITY06_PRESERVATION_PASS protected_files={len(result)} manager_callsites=exact legacy_methods=exact")
    return result


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--out", required=True, type=Path)
    ap.add_argument("--mutations", action="store_true")
    args = ap.parse_args()
    out = args.out.resolve()
    if out == ROOT or ROOT in out.parents:
        ap.error("--out must be outside the repository")
    out.mkdir(parents=True, exist_ok=True)
    hashes = preservation()
    (out / "protected-sha256.json").write_text(json.dumps(hashes, indent=2) + "\n")
    tree_path = FP / "ForestTree.cs"
    tree = fixture(tree_path.read_text(), "ForestTree")
    legacy = fixture(base_text(tree_path), "LegacyForestTree")
    sources = sorted((FP / "TimberYield").glob("*.cs")) + sorted((FP / "WorkEconomy").glob("*.cs"))
    sources += [FP / "ScenarioOne" / p for p in ["ScenarioOneDefinition.cs", "ScenarioOneEconomyAdapter.cs", "ScenarioOneWorkOrder.cs", "ScenarioDeadwoodRecord.cs"]]
    sources += [HERE / "PruningOfflineStubs.cs", HERE / "BroadleafPruningOfflineCheck.cs"]
    mutants = {
        "interval-two": ("broadleaf ? 3 : MinimumYearsBetweenLifts", "broadleaf ? 2 : MinimumYearsBetweenLifts"),
        "interval-four": ("broadleaf ? 3 : MinimumYearsBetweenLifts", "broadleaf ? 4 : MinimumYearsBetweenLifts"),
        "third-lift": ("broadleaf ? 2 : MaxPruningLifts", "broadleaf ? 3 : MaxPruningLifts"),
        "no-species-gate": ('species.SpeciesId == "sessile-oak" || species.SpeciesId == "beech"', 'species.SpeciesId != ""'),
        "no-model-gate": ("speciesAware && species != null", "species != null"),
        "wrong-target": ("pruningLifts == 0 ? 3.5f : 6f", "pruningLifts == 0 ? 2.5f : 6f"),
        "wrong-crown-loss": ("CrownRadiusReductionPerLift = 0.08f", "CrownRadiusReductionPerLift = 0.10f"),
        "height-cap": ("treeHeight * 0.6f", "treeHeight * 0.7f")}

    def run(name, current):
        work = out / name
        work.mkdir(exist_ok=True)
        # Two compilation units because each extract starts with a using directive.
        (work / "TreePruningExtract.cs").write_text(current)
        (work / "LegacyTreePruningExtract.cs").write_text(legacy)
        items = sources + [work / "TreePruningExtract.cs", work / "LegacyTreePruningExtract.cs"]
        project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType><EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>9.0</LangVersion></PropertyGroup><ItemGroup>'
        project += "".join(f'<Compile Include="{p}" />' for p in items) + '</ItemGroup></Project>'
        (work / "check.csproj").write_text(project)
        build = subprocess.run(["dotnet", "build", "-v", "q", "-c", "Release"], cwd=work, capture_output=True, text=True)
        (work / "build.log").write_text(build.stdout + build.stderr)
        if build.returncode:
            raise RuntimeError(f"{name} compilation failed (never counts as a killed mutant): {build.stdout}")
        result = subprocess.run(["dotnet", "run", "--no-build", "-c", "Release"], cwd=work, capture_output=True, text=True)
        (work / "run.log").write_text(result.stdout + result.stderr)
        return result

    result = run("candidate", tree)
    print(result.stdout, end="")
    if result.returncode:
        print(result.stderr)
        return result.returncode
    summary = {"scope": "OFFLINE ONLY — NOT UNITY VERIFIED", "base": BASE,
               "candidate_output": result.stdout, "protected_files": len(hashes), "killed_mutants": []}
    if args.mutations:
        for name, (old, new) in mutants.items():
            assert tree.count(old) == 1, (name, old)
            result = run(name, tree.replace(old, new))
            assert result.returncode != 0 and "PARITY06_PRUNING_FAIL" in result.stdout, f"survived: {name}"
            print("PARITY06_MUTANT_KILLED " + name)
            summary["killed_mutants"].append(name)
        print(f"PARITY06_MUTATIONS_PASS killed={len(mutants)}/{len(mutants)}")
    (out / "results.json").write_text(json.dumps(summary, indent=2) + "\n")
    return 0


if __name__ == "__main__":
    sys.exit(main())

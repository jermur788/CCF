#!/usr/bin/env python3
# PARITY-05 static conformance check: early-life / browse / protection, current accepted behaviour.
# Reads the repository only (species assets, production source text, scene, docs, harness text, git diff).
# It runs no Unity code and proves nothing about runtime behaviour; the Unity harnesses it points at do that.
#
#   python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_early_life_conformance.py --out <dir> [--base <rev>]
#
# --base <rev> additionally proves the diff from <rev> to the working tree touches only allowed documentation and
# verification files, never species assets / scenes / production C#, and only adds harness lines (nothing weakened).
import argparse, json, re, subprocess, sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
SPECIES = {"sitka": "Assets/ForestPrototype/Species/SitkaSpruce.asset",
           "oak": "Assets/ForestPrototype/Species/SessileOak.asset",
           "beech": "Assets/ForestPrototype/Species/BeechSpecies.asset"}
ALLOWED_CHANGES = {
    "Docs/BeechSpeciesParameters.md", "Assets/ForestPrototype/Docs/SessileOakV1.md", "Docs/BrowsingProtectionV1.md",
    "Docs/Project/unity-project-overview.md",
    "Tools/Verification/JuvenileMortalityFoundationVerification.cs", "Tools/Verification/BrowsingProtectionVerification.cs",
    "Tools/Verification/RegenerationBudget/RegenerationModelVerification.cs",
    "Tools/Verification/ThreeSpecies/OfflineLogic/check_early_life_conformance.py",
    "Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/HANDOFF.md",
}
HARNESSES = [p for p in ALLOWED_CHANGES if p.startswith("Tools/Verification/") and p.endswith(".cs")]
results = []


def check(name, ok, detail=""):
    results.append((name, bool(ok), detail))
    print(("PASS " if ok else "FAIL ") + name + (" :: " + detail if detail and not ok else ""))


def read(rel):
    return (ROOT / rel).read_text(encoding="utf-8")


def parse_asset(rel):
    """Top-level 'key: value' scalars and '- v' lists of a Unity MonoBehaviour asset (no nested objects needed)."""
    scalars, lists, current = {}, {}, None
    for line in read(rel).splitlines():
        m = re.match(r"^  (\w+):\s*(.*)$", line)
        if m:
            key, value = m.groups()
            if value == "":
                lists[key], current = [], key
            elif value == "[]":
                lists[key], current = [], None
            else:
                scalars[key], current = value, None
            continue
        m = re.match(r"^  - (.*)$", line)
        if m and current:
            lists[current].append(m.group(1))
        else:
            current = None
    return scalars, lists


def num(scalars, key, default=None):
    return float(scalars[key]) if key in scalars else default


def git(*args):
    return subprocess.run(["git", *args], cwd=ROOT, capture_output=True, text=True, check=True).stdout


def method_body(text, signature):
    start = text.index(signature)
    depth, i = 0, text.index("{", start)
    begin = i
    while True:
        depth += (text[i] == "{") - (text[i] == "}")
        i += 1
        if depth == 0:
            return text[begin:i]


def assets():
    parsed = {k: parse_asset(v) for k, v in SPECIES.items()}

    # M1: selection. Oak distinct; Beech (FALLBACK-CONSERVATIVE) and Sitka combined.
    check("OAK_USES_DISTINCT_JUVENILE_CURVES", parsed["oak"][0].get("useDistinctJuvenileLightResponses") == "1"
          and len(parsed["oak"][1]["survivalResponseFactor"]) >= 2 and len(parsed["oak"][1]["establishmentResponseFactor"]) >= 2)
    for k in ("beech", "sitka"):
        s, l = parsed[k]
        check(f"{k.upper()}_USES_COMBINED_FALLBACK_NOT_DISTINCT",
              s.get("useDistinctJuvenileLightResponses", "0") == "0"
              and not l.get("survivalResponseFactor") and not l.get("establishmentResponseFactor"))

    # M1: shared juvenile growth / promotion / density calibration, identical for all three; no new scalar.
    shared = {"regenHeightGrowthMPerYear": 0.35, "promotionHeightM": 3.5, "regenDensityPerEstablishment": 0.5,
              "regenDensityMax": 1.5, "regenMortalityUnderPoorLight": 0.2, "regenPoorLightThreshold": 0.15}
    for k, (s, _) in parsed.items():
        check(f"{k.upper()}_SHARED_JUVENILE_CALIBRATION_EXACT", all(num(s, f) == v for f, v in shared.items()),
              str({f: s.get(f) for f in shared}))
    check("INITIAL_HEIGHT_AND_PROMOTION_LIGHT_FLOOR_EXACT",
          num(parsed["sitka"][0], "regenInitialHeightM") == 0.15 and num(parsed["beech"][0], "regenInitialHeightM") == 0.15
          and num(parsed["oak"][0], "regenInitialHeightM") == 0.18
          and num(parsed["sitka"][0], "promotionMinimumLight", 0.0) == 0.0 and num(parsed["beech"][0], "promotionMinimumLight", 0.0) == 0.0
          and num(parsed["oak"][0], "promotionMinimumLight") == 0.2)
    check("OAK_DISTINCT_CURVE_ANCHORS_EXACT",
          [float(x) for x in parsed["oak"][1]["survivalResponseFactor"]] == [0.2, 0.3, 0.55, 0.75, 0.9, 1, 1, 1]
          and [float(x) for x in parsed["oak"][1]["survivalResponseLight"]] == [0, 0.01, 0.05, 0.1, 0.15, 0.2, 0.3, 1]
          and [float(x) for x in parsed["oak"][1]["establishmentResponseFactor"]] == [0.75, 0.8, 0.95, 1, 1]
          and [float(x) for x in parsed["oak"][1]["establishmentResponseLight"]] == [0, 0.01, 0.05, 0.1, 1])
    check("BEECH_GROWTH_LIGHT_CURVE_EXACT",
          [float(x) for x in parsed["beech"][1]["lightResponseLight"]] == [0, 0.02, 0.05, 0.1, 0.2, 0.35, 1]
          and [float(x) for x in parsed["beech"][1]["lightResponseFactor"]] == [0, 0, 0.15, 0.35, 0.65, 1, 1])

    # M2: browse. Current absolute palatabilities, ordering, and unchanged escape heights.
    pal = {k: num(parsed[k][0], "browsePalatability") for k in parsed}
    check("BROWSE_PALATABILITY_CURRENT_VALUES", pal == {"sitka": 0.15, "beech": 0.5, "oak": 1.0}, str(pal))
    check("BROWSE_ORDERING_OAK_GT_BEECH_GT_SITKA", pal["oak"] > pal["beech"] > pal["sitka"])
    check("BROWSE_HEIGHTS_UNCHANGED",
          (num(parsed["sitka"][0], "browseFullVulnerabilityHeightM"), num(parsed["sitka"][0], "browseEscapeHeightM")) == (0.5, 0.8)
          and all((num(parsed[k][0], "browseFullVulnerabilityHeightM"), num(parsed[k][0], "browseEscapeHeightM")) == (1.2, 1.8)
                  for k in ("oak", "beech")))
    return parsed


def production_source():
    rules = read("Assets/ForestPrototype/JuvenileEcologyRules.cs")
    survival = method_body(rules, "public static double SurvivalResponse(TreeSpeciesDefinition species, float light)")
    check("SURVIVAL_SELECTION_DISTINCT_ELSE_COMBINED_FALLBACK",
          "species.UsesDistinctJuvenileLightResponses" in survival and "DistinctJuvenileSurvivalResponse(light)" in survival
          and "LightResponse(species, light) < species.RegenPoorLightThreshold" in survival
          and "1d - species.RegenMortalityUnderPoorLight : 1d" in survival)
    check("BROWSE_CONSTANTS_UNCHANGED", all(s in rules for s in (
        "BrowseProbabilityCap = 0.95f", "BrowseHeightIncrementLoss = 0.9f", "BrowseExtraMortality = 0.04f")))
    assess = method_body(rules, "public static BrowseAssessment AssessBrowse(")
    check("BROWSE_FORMULA_IS_PRODUCT_WITH_PROTECTION_ACCESS",
          "assessment.BackgroundPressure * assessment.Palatability * assessment.HeightVulnerability" in assess
          and "* assessment.VegetationExposure * assessment.ProtectionAccess" in assess
          and "Mathf.Min(BrowseProbabilityCap, Mathf.Max(0f, effective))" in assess)

    conditions = read("Assets/ForestPrototype/BrowsingConditions.cs")
    protection_at = method_body(conditions, "public BrowseProtectionState ProtectionAt(")
    assigned = re.findall(r"\baccess\s*=\s*([^;]+);", protection_at)
    check("PROTECTION_ACCESS_IS_BINARY_0_OR_1", assigned and set(assigned) <= {"0f", "1f"} and {"0f", "1f"} <= set(assigned), str(assigned))
    check("INTACT_FENCE_AND_EFFECTIVE_SHELTER_ACCESS_0",
          re.search(r"IsEffective\(year\)\)\s*\{\s*access = 0f;\s*return BrowseProtectionState\.EffectiveShelter", protection_at) is not None
          and re.search(r"ExcludesDeer\(year\)\)\s*\{\s*access = 0f;\s*return BrowseProtectionState\.InsideIntactFence", protection_at) is not None)

    definition = read("Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs")
    check("SCENARIO_ONE_0_2_IS_BACKGROUND_PRESSURE",
          "private float backgroundBrowsePressure = 0.2f" in definition)
    manager = read("Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs")
    check("SCENARIO_ONE_0_2_COPIED_ONLY_TO_BACKGROUND_PRESSURE",
          re.search(r"Browsing\.BackgroundPressure\s*=\s*Definition\.BackgroundBrowsePressure", manager) is not None
          or re.search(r"Browsing\.BackgroundPressure\s*=\s*\w*[Dd]efinition\w*\.BackgroundBrowsePressure", manager) is not None)

    controller = read("Assets/ForestPrototype/ForestEcologyController.cs")
    check("STOCK_DEFAULTS_SHARED_3_YEARS_0_6_M_DENSITY_1",
          "private int plantedJuvenileAgeYears = 3;" in controller and "private float plantedJuvenileHeightM = 0.6f;" in controller
          and "private float plantedJuvenileDensity = 1f;" in controller)
    scene = read("Assets/Scenes/ForestTest.unity")
    check("SCENE_STOCK_VALUES_SHARED",
          all(s in scene for s in ("plantedJuvenileAgeYears: 3", "plantedJuvenileHeightM: 0.6", "plantedJuvenileDensity: 1")))
    # Known stale production wording. Recorded, deliberately NOT edited by PARITY-05 (production file).
    stale = 'Header("Planting (Beech v1)")' in controller
    print("INFO production_stock_header_still_says_beech_v1=" + str(stale) + " (follow-up, not edited by PARITY-05)")
    return stale


def rejected_concepts_absent():
    tokens = re.compile(r"establishmentRatePerYear|establishmentSurvival\b|juvenileAnnualSurvival|"
                        r"protect\w*Multiplier|residualAccess|partialProtection|browseSusceptibilityMultiplier", re.I)
    hits = []
    for pattern in ("Assets/**/*.cs", "Assets/ForestPrototype/Species/*.asset"):
        for path in ROOT.glob(pattern):
            if tokens.search(path.read_text(encoding="utf-8", errors="ignore")):
                hits.append(str(path.relative_to(ROOT)))
    check("PACKET_ONLY_EARLY_LIFE_AND_PROTECTION_MULTIPLIERS_ABSENT_FROM_PRODUCTION", not hits, str(hits))


def docs_and_harness():
    beech = read("Docs/BeechSpeciesParameters.md")
    check("DOC_BEECH_FALLBACK_CONSERVATIVE_LABEL", beech.count("FALLBACK-CONSERVATIVE") >= 2 and "Early-life response and planting stock" in beech)
    check("DOC_BEECH_BROADLEAF_STOCK_ABSTRACTION", "Broadleaf stock abstraction v1" in beech)
    oak = read("Assets/ForestPrototype/Docs/SessileOakV1.md")
    check("DOC_OAK_DISTINCT_AND_BEECH_FALLBACK_LABEL", "FALLBACK-CONSERVATIVE" in oak and "keeps its distinct establishment and" in oak
          and "broadleaf stock abstraction v1" in oak)
    browse = read("Docs/BrowsingProtectionV1.md")
    check("DOC_BROWSE_VALUES_ORDERING_AND_BINARY_PROTECTION",
          "Sitka **0.15**, Beech **0.5**, Oak **1.0**" in browse and "Oak > Beech > Sitka" in browse
          and "Protection is binary access" in browse)
    check("DOC_0_2_IS_BACKGROUND_PRESSURE_NOT_PROTECTION",
          "Scenario One 0.2 is background browse pressure, not a protection factor" in browse)
    overview = read("Docs/Project/unity-project-overview.md")
    check("DOC_OVERVIEW_CURRENT_STATE_AND_PARITY_08_FOLLOW_UP",
          "Current early-life and browse model (PARITY-05" in overview and "FALLBACK-CONSERVATIVE" in overview and "PARITY-08" in overview)

    browse_h = read("Tools/Verification/BrowsingProtectionVerification.cs")
    block = method_body(browse_h, "private void VerifyEarlyLifeBrowseConformance(")
    check("HARNESS_BROWSE_CONFORMANCE_PRESENT_AND_HASH_SAFE",
          "VerifyEarlyLifeBrowseConformance(sitka, oak, beech);" in browse_h and "BROWSE_EARLY_LIFE_CONFORMANCE_PASS" in block
          and "Emit(" not in block, "the new block must not call Emit: Emit feeds BROWSE_MEASUREMENT_HASH")
    juvenile_h = read("Tools/Verification/JuvenileMortalityFoundationVerification.cs")
    block = method_body(juvenile_h, "private static void VerifyEarlyLifeConformance(")
    check("HARNESS_JUVENILE_CONFORMANCE_PRESENT_AND_READ_ONLY",
          "VerifyEarlyLifeConformance(ecology, spawner);" in juvenile_h and "EARLY_LIFE_CONFORMANCE_VERIFY_PASS" in block
          and not re.search(r"\.(Restore|Add|RestoreEcologyState|LoadData)\(|\bnew ForestSaveData", block))


def diff_proof(base):
    names = git("diff", "--name-status", base).splitlines()
    untracked = git("ls-files", "--others", "--exclude-standard").splitlines()
    changed = {line.split("\t", 1)[1].strip(): line.split("\t", 1)[0] for line in names}
    changed.update({p: "A" for p in untracked})
    outside = sorted(set(changed) - ALLOWED_CHANGES)
    check("DIFF_TOUCHES_ONLY_ALLOWED_FILES", not outside, str(outside))
    forbidden = re.compile(r"\.(asset|unity|prefab|mat|meta|cs)$|^(ProjectSettings|Packages)/")
    protected = [p for p in changed if forbidden.search(p) and p not in HARNESSES]
    check("ZERO_SPECIES_ASSET_SCENE_PREFAB_SETTINGS_OR_PRODUCTION_CS_CHANGE", not protected, str(protected))
    # Harness edits must only add lines, so no existing assertion or expected value can have been weakened.
    removed = {}
    for p in HARNESSES:
        for line in git("diff", "--unified=0", base, "--", p).splitlines():
            if line.startswith("-") and not line.startswith("---"):
                removed.setdefault(p, []).append(line)
    check("HARNESS_EDITS_ARE_ADDITIVE_ONLY", not removed, str(removed))
    hexed = [l for p in HARNESSES + sorted(ALLOWED_CHANGES - set(HARNESSES))
             for l in git("diff", "--unified=0", base, "--", p).splitlines()
             if l[:1] in "+-" and l[:3] not in ("+++", "---") and re.search(r"\b[0-9A-F]{16}\b", l)]
    check("NO_PROTECTED_FINGERPRINT_TOKEN_ADDED_OR_REMOVED", not hexed, str(hexed))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--out", type=Path, required=True)
    ap.add_argument("--base")
    args = ap.parse_args()
    args.out.mkdir(parents=True, exist_ok=True)
    assets()
    stale = production_source()
    rejected_concepts_absent()
    docs_and_harness()
    if args.base:
        diff_proof(args.base)
    failed = [r for r in results if not r[1]]
    summary = {"passed": len(results) - len(failed), "failed": [r[0] for r in failed],
               "production_stock_header_still_stale": stale, "base": args.base, "unity_verified": False}
    (args.out / "early_life_conformance.json").write_text(json.dumps({"results": results, "summary": summary}, indent=1))
    print(("EARLY_LIFE_CONFORMANCE_STATIC_PASS" if not failed else "EARLY_LIFE_CONFORMANCE_STATIC_FAIL")
          + f" checks={len(results)} NOT_UNITY_VERIFIED")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())

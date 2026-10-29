#!/usr/bin/env python3
"""PARITY-07 static copy check: FR-9 required truths present, forbidden wording absent.

Reads UI C# source and the spec only. NOT UNITY-VERIFIED: it proves the strings are in the source,
not that they render, fit at 1280x720/1920x1080 or are reachable in play.
Run: python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_player_facing_truth.py [--repo ROOT]
"""
import argparse, re, sys
from pathlib import Path

ap = argparse.ArgumentParser()
ap.add_argument("--repo", default=str(Path(__file__).resolve().parents[4]))
root = Path(ap.parse_args().repo)
ui = root / "Assets/ForestPrototype/UI"
spec = (root / "specs/scenario-one-three-species-parity-p0/spec.md").read_text(encoding="utf-8")

passed = failed = 0
def check(name, ok):
    global passed, failed
    if ok: passed += 1
    else:
        failed += 1; print("FAIL:", name)

def src(name): return (ui / name).read_text(encoding="utf-8")

# Forbidden player-facing wording (FR-9 / review N2): measured form/defect, guaranteed premium Oak, stale pruning premium claim.
FORBIDDEN = {
    "straight enough": r"straight enough",
    "large, straight log": r"large,\s*straight",
    "size and form": r"size and form",
    "form-quality claim": r"(?i)\bgood form\b|\bwell[- ]formed\b|\bstraight stems?\b",
    "guaranteed premium": r"(?i)guarantee[ds]?\b[^\".]{0,40}(?:premium|veneer|oak)|(?:premium|veneer|oak)[^\".]{0,40}\bguarantee|will (?:reach|become) (?:premium|veneer)",
    "stale no-pruning-premium": r"applies no timber-price premium for pruning",
    "stale 3-lift for all species": r"This game's configured lift heights are 2\.5 m",
}
def forbidden_hits(text):
    return [k for k, rx in FORBIDDEN.items() if re.search(rx, text)]

ui_files = sorted(ui.rglob("*.cs"))
all_ui = {f.name: f.read_text(encoding="utf-8") for f in ui_files}
for fname, text in all_ui.items():
    check(f"forbidden wording absent in {fname}: {forbidden_hits(text)}", not forbidden_hits(text))

facts = src("ScenarioOneUiFacts.cs"); insp = src("TreeInspectionView.cs")
work = src("WorkPlanView.cs"); cur = src("LearningObjectivesView.Curriculum.cs")

def const(name):
    m = re.search(rf'const string {name} = "([^"]*)"', facts)
    return m.group(1) if m else ""
q, p, s = const("BroadleafQualityLimit"), const("PremiumOakNote"), const("SharedSpeciesRulesNote")

# Required truths, each mapped to a spec FR-9 bullet.
check("FR-9a quality = size + pruning history", "size and pruning history" in q)
check("FR-9a fork/defect not modelled", "fork and defect shape is not modelled" in q)
check("FR-9b spec bullet present", "size and pruning history in this version; fork/defect shape is not modelled" in spec)
check("FR-9c site limit stated", "Site suitability" in s and "not calibrated per species" in s)
check("FR-9c canopy limit stated", "canopy shading" in s)
check("FR-9d wind shared, not species-calibrated", "wind exposure" in s and "not calibrated per species" in s)
check("FR-9d storms-off wording in inspection", "Storm events are off in this forest." in insp and "StormModel.None" in insp)
check("FR-9d spec wind bullet present", "storms are off/not modelled" in spec)
check("FR-9e premium Oak possible+late", "possible late outcome" in p)
check("FR-9e premium Oak conditions", "favourably grown" in p and "little competition" in p and "pruned" in p)
check("FR-9e premium Oak not promised/required", "not promised or required" in p)
check("FR-9e spec bullet present", "possible late, favourable, low-competition and pruning-dependent outcome" in spec)

# Wiring: the shared strings are actually used by each surface.
check("inspection uses quality constant", "ScenarioOneUiFacts.BroadleafQualityLimit" in insp)
check("inspection uses premium constant", "ScenarioOneUiFacts.PremiumOakNote" in insp)
check("inspection uses shared-rules constant", "ScenarioOneUiFacts.SharedSpeciesRulesNote" in insp)
check("Oak pruned line says eligible, not guaranteed", "can become eligible for first-grade veneer/planking once they are large enough" in insp)
check("Work Plan harvest uses quality constant", "ScenarioOneUiFacts.BroadleafQualityLimit" in work)
check("Work Plan harvest uses premium constant", "ScenarioOneUiFacts.PremiumOakNote" in work)
check("Work Plan pruning says fork/defect not modelled", "Fork and defect shape is not modelled" in work)
check("teaching pruning says fork/defect not modelled", "fork or defect shape is not modelled" in cur)

# Pruning teaching matches the production broadleaf rule in ForestTree.CanPrune.
tree = (root / "Assets/ForestPrototype/ForestTree.cs").read_text(encoding="utf-8")
check("production broadleaf 2 lifts / 3 years / 3.5 / 6", all(t in tree for t in ["broadleaf ? 2 :", "broadleaf ? 3 :", "pruningLifts == 0 ? 3.5f : 6f"]))
check("teaching states 3.5 m then 6.0 m, 3 years, two lifts",
      "two lifts, to 3.5 m then 6.0 m, at least three years apart" in cur)
check("teaching Sitka schedule unchanged 2.5/5/6.5, 3 lifts, 5 years",
      "2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart)" in cur)

# Negative controls: the forbidden-wording detector must catch each injected phrase.
for k, bad in [("straight enough", "if they are large and straight enough"), ("large, straight log", "needs a large, straight log"),
               ("size and form", "depends on size and form"), ("stale no-pruning-premium", "applies no timber-price premium for pruning")]:
    check(f"negative control detects '{k}'", k in forbidden_hits(bad))
check("negative control detects guaranteed premium", "guaranteed premium" in forbidden_hits("a guaranteed premium Oak outcome"))

print(f"PARITY07_COPY {'PASS' if not failed else 'FAIL'} checks={passed + failed} passed={passed} failed={failed} NOT_UNITY_VERIFIED")
sys.exit(1 if failed else 0)

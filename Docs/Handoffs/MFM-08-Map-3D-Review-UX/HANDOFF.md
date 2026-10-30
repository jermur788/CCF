# MFM-08 — Stand Map / 3D Review UX — Offline Candidate Handoff

**Claim:** #156 · **Parent:** #91 / #106 · **Mode:** offline/static only (no Unity run in this phase)
**Provider/model:** Claude Code, Sonnet 5.5 (medium)
**Branch:** `feature/mfm-08-map-3d-review-ux-v1` · **Worktree:** `/media/jer/Videos/CCF-MFM08`
**Exact start (Manager repin `6511551513`):** `cfdc93bf3943125d21570bc5749a9338f52d1cce`
**Production candidate SHA (all code):** `d55ec86a3f0bf13820f2080197b6b0f43d832e06` (first slice `4c1e25f2`). This HANDOFF is a docs-only successor commit on top; its SHA is the branch tip and is posted on #156.

## Verdict
Offline/static portion **passes**. The candidate is **not Unity-verified**: no compile in the Editor, no rendering, no interaction was run. Unity Tier-2 qualification (1280×720 and 1920×1080, interaction, regressions) is still required and is scheduled separately by #91.

## Changed files (12 vs start; nothing else)
Production:
- `Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs` (new, pure, no UnityEngine): wording, closed-reason text, area rows, `Session` (draft choices, rejections, candidate cursor, active prescription, 3D spot-check latch), approval gate, 3D tree context.
- `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs` (new): the Treatment review side card, map-layer feed, and the 3D companion (`OnInspected`, `ContextFor`).
- `Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs` (new, pure): per-state outline/hatch patterns, candidate and proposal glyph shapes.
- `Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs` (new): its own element over the map surface; paints the plan and prints an area tag. The shared canvas, draw plan, model and HUD minimap are untouched.
- `Assets/ForestPrototype/UI/StandMapView.cs` (+23/−3): `Layer.Review` tab "Treatment review" (only when `RackPlanningV2Active`), layer element, side-card branch, `SetWaypointAtWorld`, state key.
- `Assets/ForestPrototype/UI/TreeInspectionView.cs` (+16/−1): calls `OnInspected`, adds a candidate/proposal chip and line, and the contextual key hint.
- four `.meta` files for the new scripts.
Verification: `Tools/Verification/MfmTreatmentReview/` (runner + `OfflineLogic/TreatmentReviewOfflineCheck.cs`).
**Not touched:** `ScenarioOneUiRoot.cs`, `AnnualReviewView.cs`/history, `ScenarioOneManager*`, coverage/calibration/world adapter, save/schema/validator/hash/reference, Work Plan code, learning/curriculum, USS, scenes/prefabs, Windows build.

## APIs consumed
- Coverage: `ScenarioOneManager.ReadCoverage(out string problem, IReadOnlyCollection<string> manualUnitIds = null)` returning `ScenarioOneCoverage.Result`. Called with **no** manual ids, so classification stays pure and the Include/Leave-manual choice is a separate draft (otherwise "Leave manual" would turn Covered into Manual). Two call sites: the review read and the once-per-newly-inspected-tree spot-check read. No `Classify`, masks or coverage-world code in MFM-08 files (asserted).
- Candidates: `ScenarioOneCropTreeCandidates.Suggest`, `TryGetInspectionTarget`, `TryMoveInspectionTarget`.
- Prescription: `CreatePrescription`, `PreviewPrescription`, `ApprovePrescription` (each called once). `CheckPrescriptionLink` is not needed and not used.

## Implemented contract
- **Source sample vs targets:** sample row "Sample — demonstrated treatment", never a target; map pattern is a heavy outline with crossed diagonal hatch.
- **Four states, readable reasons:** `COVERED`/`NOT COVERED`/`EXCLUDED`/`MANUAL` with plain-language text for every closed `Reason`; `NaturalRegeneration`/`PlayerCropTrees` are informational notes, not exclusions. Remainders listed as Manual.
- **Covered = comparable, not committed:** meaning line "Comparable — available to include" plus a note. Classification and the draft choice ("Apply sample treatment here" / "Leave manual") are separate; only Covered targets are offered the choice (the model also refuses it for others).
- **Wording:** "Apply your sample treatment to comparable areas." "operator selection" is not used.
- **Candidates:** card with "CANDIDATE — inspection prompt" and "Spacing identifies where to inspect. Judge the tree in 3D before confirming it as a Crop Tree." Previous/Next (non-wrapping; Previous only from a selected candidate), Inspect in forest (sets the existing waypoint; no teleport). The map has no confirm and there is no bulk action. In 3D the inspection card shows the candidate chip; **[C]** is the normal Crop Tree mark (confirmation), **[N]** rejects/reopens (session-only, no mark). Confirmed state is derived from the live Crop Tree mark.
- **Proposals:** from read-only `PreviewPrescription`; each labelled "PROPOSAL — not a Fell mark and not yet work" with tree, unit, served Crop Tree, reason and share/basal area; conflicts ("CONFLICT — held, not proposed") use the production conflict kinds. Plain data only, never marks.
- **Prepare / approve:** "Prepare prescription for review" calls `CreatePrescription` (snapshot only). "Approve prescription" is enabled only with an active snapshot, a valid preview with proposals, and a 3D spot-check; the gate is re-evaluated in the click handler before `ApprovePrescription`. Success text: "Prescription approved. Pending tasks were added to the Work Plan. They are not approved for execution yet." The Work Plan is never opened or approved from here.
- **3D spot-check:** recorded only in `OnInspected` when the player actually inspects a tree standing in an accepted, still Covered unit of the active prescription. Waypoint, selection and map movement never set it. Not persisted; reset when the active snapshot is replaced.
- **Form, not colour:** areas differ by outline dash/weight and hatch (sample cross-diagonal; covered single diagonal; not covered dashed, no hatch; excluded square grid; manual dotted) and carry a printed tag. Candidate = hollow **square**, proposal = hollow **triangle** (conflict adds a bar); existing marks are rings on the map and paint bands in the forest. Violet/pink are bounded presentation choices.
- **Fail closed:** an invalid `ReadCoverage` shows the production problem and builds no action.

## Offline evidence (all run on this candidate)
- `python3 -B Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py` → `MFM08_BUILD_PASS` (14 sources, warnings as errors), `MFM08_TREATMENT_REVIEW_PASS checks=106 failed=0`, `MFM08_REPEAT_PASS` (two processes, byte-identical), `MFM08_BOUNDARY_PASS` (only owned files changed since the start).
- Regression, unchanged: MFM-04 core `240/0`, MFM-05 candidates `95/0`, MFM-07 approval boundary `125/0`, each with repeat PASS.
- Type-check of the Unity-facing files: `Tools/Verification/CloudTypeCheck/run_typecheck.py` (2021.3 approximation) with a throwaway `Painter2D` stub outside the repo, bodies confirmed to bind by a negative control. No errors in `StandMapTreatmentReview`, `ScenarioOneTreatmentReview`, `StandMapView`, `TreeInspectionView` or the plan. The one new entry vs baseline is the same `MeshGenerationContext.painter2D` 2021.3 gap `StandMapCanvas` and others already have (Painter2D exists in Unity 6). **This is not Unity's compiler.**
- Evidence logs: `/media/jer/Files/CCF/Verification/MFM08-TreatmentReview-final/`, `MFM08-regress-*`.

## Decisions to review
1. **Footprints and candidate stems are drawn from public pure data, not a coverage adapter.** Area polygons come from `ScenarioOneRackSampleGeometry.Derive(ConfirmedRacks, half)` (the same pure function production uses; matched to coverage areas by `UnitIndex`+`RackId`, otherwise not drawn). The candidate batch needs a stem list, so the card reads living `ForestTree`s (position, species, Crop/Fell mark). Coverage classification still comes only from `ReadCoverage`. If the Manager prefers a thin manager accessor for these, that is a separate API request.
2. **[N]** for reject/reopen: free of existing bindings; it is in the contextual hint only, not a global shortcut.
3. The tab is shown only when `RackPlanningV2Active`.

## Unity-readiness checklist (for the separately scheduled gate)
1. Import/compile clean in Unity 6000.6.0f1 (new files have `.meta`; Painter2D/`ScrollViewMode` usage untested).
2. Render Treatment review at 1280×720 and 1920×1080: sample + four states + tags + legend text; the card scrolls rather than overflowing (height capped at `MapSidePx − 40`); confirm area tags and hatching are legible and do not hide the map squares.
3. Covered vs Include/Manual is distinct on screen.
4. Candidate Previous/Next/Inspect; walk; confirm one with [C]; reject another with [N]; no bulk action; reopen works.
5. Candidate/proposal glyphs distinguishable from Fell/Crop/Rack marks by shape/text (rings, bands, rack lines).
6. Prepare prescription creates no order or mark; preview shows proposals; inspect one in 3D (chip + line, no Fell mark).
7. Waypoint alone leaves "3D spot-check not done"; actually inspecting a tree in an accepted unit completes it; Approve stays disabled before.
8. Approve creates ordinary Pending work with provenance and the Work Plan stays unapproved; double-approve blocked.
9. Conflict/held presentation via fixture (manual Fell mark, open work, unit no longer covered).
10. Rerun Stand Map/Rack V2, MFM-04/05/06/07 and UI regressions; Reference Future v1 hashes unchanged; no Windows build.
Watch for: `maxHeight`-bounded `ScrollView` inside the `map-side` card; keyboard focus (buttons are `focusable=false` via `UiKit`); the review layer sitting under the square buttons.

## Next action
#91 schedules the Unity Tier-2 gate on the exact SHA posted on #156 (fresh lock/process/free-space/priority preflight). Do not integrate before then.

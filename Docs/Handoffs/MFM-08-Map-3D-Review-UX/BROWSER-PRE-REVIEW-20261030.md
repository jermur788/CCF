# MFM-08 — Browser Static Pre-Review

**Claim:** #178  
**Feature claim:** #156  
**Parent:** #91 / #106  
**Browser lane:** #162  
**Date:** 2026-10-30  
**Exact production candidate reviewed:** `d55ec86a3f0bf13820f2080197b6b0f43d832e06`  
**Worker docs/handoff tip reviewed:** `d70cec1781c104794a6f225c8609847c55df1510`  
**Exact implementation start:** `cfdc93bf3943125d21570bc5749a9338f52d1cce`

## Verdict

**APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED**

This verdict is deliberately limited. The source/diff, accepted API boundaries and pushed offline evidence are consistent with the approved MFM-08 contract. It does **not** claim Unity import/compile, interaction correctness, rendered legibility, input behavior, geometry alignment in a live scene, runtime provenance, or Reference Future preservation on this candidate. Those remain mandatory Tier-2 evidence before integration.

No production edit, local worktree operation, Unity execution, runtime/rendered claim, integration or Windows build was performed by this review.

## 1. Changed-file scope — PASS

Git comparison from exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce` to production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06` is exactly two commits ahead with 12 changed files:

Production presentation:

- `Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs` — new pure review/session presentation model;
- `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs` — new Stand Map treatment-review card/layer controller and 3D companion;
- `Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs` — new pure review shape/pattern plan;
- `Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs` — new UI Toolkit paint layer;
- `Assets/ForestPrototype/UI/StandMapView.cs` — bounded Treatment review tab/layer/navigation hookup;
- `Assets/ForestPrototype/UI/TreeInspectionView.cs` — bounded candidate/proposal inspection context;
- four matching `.meta` files.

Focused verification:

- `Tools/Verification/MfmTreatmentReview/OfflineLogic/TreatmentReviewOfflineCheck.cs`;
- `Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py`.

The candidate does **not** change `ScenarioOneUiRoot`, Annual Review/history, generic S1-D Work Plan presentation, `ScenarioOneManager*`, MFM coverage/calibration/world-adapter logic, save/schema/validator/hash/reference state, MFM-09 curriculum, learning IDs, scenes/prefabs, growth/ecology/economy or Windows build material. This matches the MFM-08 packet ownership boundary.

The docs tip `d70cec1781c104794a6f225c8609847c55df1510` is a handoff-wording-only successor; it does not alter the reviewed production candidate.

## 2. Authoritative coverage use — PASS

The approved MFM-08 contract required the final #129 `ScenarioOneManager.ReadCoverage` façade to remain the sole coverage-classification source. #129 had already been Manager-approved and integrated with the exact semantics required for that façade: same authoritative world read and `ScenarioOneCoverage.Classify` path used by production approval, side-effect-free, no cache, persistence, hidden fallback or alternate UI classifier.

The candidate complies:

- the review read is `manager.ReadCoverage(out coverageProblem)`;
- the 3D spot-check re-reads current coverage through `ui.Manager.ReadCoverage(out _)`;
- no MFM-08 file calls `ScenarioOneCoverage.Classify`, rebuilds masks/features, invokes a coverage-world adapter or copies coverage calibration;
- invalid coverage fails closed and exposes the production problem instead of inventing fallback coverage;
- state/reason/flags shown to the player come from `ScenarioOneCoverage.Result`.

### Rack-sample polygon question — source-resolved, live alignment still a Tier-2 check

The UI does derive **drawing geometry only** with the same public pure Rack V2 function, `ScenarioOneRackSampleGeometry.Derive(ui.Manager.ConfirmedRacks, half)`. It does not use that geometry to classify coverage. A drawn polygon is accepted only when it matches the authoritative coverage area by `UnitIndex` and `RackId`; an unmatched footprint is omitted rather than guessed.

That is an acceptable presentation read and does not create a second coverage authority. A new manager accessor is therefore **not required for static approval**. Tier-2 must still prove the rendered footprints align with the production coverage state in the real Enlarged80 scene, after ordinary load/state transitions.

### Living-tree stem list question — source-resolved, runtime completeness still a Tier-2 check

`StandMapTreatmentReview.ReadStems()` reads the existing live `ForestTree` objects for position, species and current Crop/Fell state, then passes those plain facts into the already-authoritative `ScenarioOneCropTreeCandidates` rule. It neither advances biology nor substitutes a second tree model, and candidate ordering/eligibility remains owned by MFM-05.

A thin manager accessor is therefore **not required for static approval**. Tier-2 should still prove the scene read remains complete/current across confirmation, rejection, load/reference transitions and map reopening.

## 3. Source sample / coverage semantics — PASS

The source sample remains explicitly separate from targets:

- `Sample — demonstrated treatment`;
- source is not an extrapolation target;
- `Covered` means `Comparable — available to include`, not selected or committed;
- only Covered non-source areas can receive the draft `Apply sample treatment here` / `Leave manual` choice;
- `Not Covered`, `Excluded` and `Manual` remain distinct states;
- remainders are surfaced as Manual rather than silently assigned;
- coverage reasons use the closed production vocabulary, including the explicit non-Sitka V1 reason;
- `NaturalRegeneration` and `PlayerCropTrees` remain informational flags rather than exclusions.

The draft choice is session/presentation state and is pruned if a unit stops being Covered. It does not rewrite classification.

## 4. Crop Tree candidate semantics — PASS

Candidate semantics remain the accepted MFM-05 / MFM V1 contract:

- candidate wording says `CANDIDATE — inspection prompt`;
- the player is explicitly told that spacing identifies **where to inspect**, while 3D judgement determines Crop Tree suitability;
- Previous/Next navigation is non-wrapping;
- the review session exposes no confirm-all or bulk confirmation operation;
- the map never confirms a candidate;
- confirmation is inferred only from the live ordinary Crop Tree mark, using the normal `[C]` path in 3D;
- `[N]` rejection/reopen is session-only review state and creates no second persistent tree mark.

This does not claim the `[C]` / `[N]` input path works correctly in Unity; that remains a Tier-2 interaction check.

## 5. Proposal and commitment semantics — PASS

Proposal state remains non-authoritative review data until explicit prescription approval:

- proposal rows come from read-only `PreviewPrescription`;
- every proposal is labelled `PROPOSAL — not a Fell mark and not yet work`;
- held/conflicting proposals are surfaced rather than silently applied;
- no MFM-08 source calls a Fell-mark mutation, felling-order creation, Work Plan approval or execution path;
- `Prepare prescription for review` calls `CreatePrescription` only, creating the accepted snapshot boundary but no work/mark;
- prescription approval is gated on an active snapshot, valid current preview with proposals and a real 3D spot-check;
- the approval click re-evaluates the gate and then calls the existing MFM-07 `ApprovePrescription` boundary once;
- success copy states that Pending tasks were added to the Work Plan and are **not approved for execution yet**;
- the Work Plan remains the separate commitment gate.

The 3D spot-check is also correctly bounded in source: selecting an area, moving the map or setting a waypoint cannot satisfy it. `RecordSpotCheck` is reached from the actual inspected-tree path and requires an accepted unit that is still Covered in a fresh authoritative coverage read.

## 6. Visual/semantic distinction — PASS by source design; rendering still unproven

The implementation does not depend on hue alone:

- sample: heavy outline + crossed diagonal hatch;
- Covered: solid outline + single diagonal hatch;
- Not Covered: dashed outline;
- Excluded: square-grid hatch;
- Manual: dotted outline;
- candidate: hollow square, with separate rejected/confirmed/current forms;
- proposal: hollow triangle;
- held conflict: triangle plus bar.

Area tags and card text carry the meaning in words. Existing authoritative Fell/Crop marks remain rings on the map / paint bands in the forest; Rack V2 retains its own line/entrance-mark grammar. Open candidate/proposal state does not reuse those authoritative mark shapes.

This is sufficient static separation, but **actual legibility and collision avoidance are not yet proven**. The chosen violet/pink colours are presentation choices only; Tier-2 must judge the rendered result by form + text/pattern at both required resolutions.

## 7. Open implementation questions classified

| Question | Static classification | Required follow-up |
|---|---|---|
| Rack-sample geometry read | **Source-resolved / no API blocker.** Same pure Rack V2 geometry is used only to draw, then matched to authoritative coverage identity; it does not classify. | Render/live-scene alignment in Tier-2. |
| Living-tree stem list | **Source-resolved / no API blocker.** Reads authoritative live tree facts and delegates candidate logic to MFM-05; no second biological state. | Runtime completeness/current-state witness in Tier-2. |
| Card `maxHeight` / internal scrolling | **Genuine rendered residual.** Source bounds the ScrollView to the map frame but cannot prove layout. | 1280×720 and 1920×1080 screenshots plus interaction/readability. |
| `Painter2D` / `ScrollViewMode` Unity-6 path | **Genuine Unity residual.** Cloud type-check is explicitly not the Unity compiler. | Unity 6000.6.0f1 import/compile + rendered paint witness. |
| Review overlay ordering / map square interaction | **Genuine Unity/rendered residual.** Layer is `PickingMode.Ignore` and source ordering is reasonable, but actual stacking/legibility is runtime UI behavior. | Verify area hatching/tags do not block or obscure required map interaction/marks. |
| Treatment-review tab placement | **Source-resolved product placement; rendered residual remains.** It is inside the existing Stand Map tab row and only appears for Rack V2. | Confirm tab/readability/focus at both resolutions. |
| `[N]` candidate reject/reopen | **Source-resolved semantics; runtime input residual.** Contextual only, no persistent mark. | Confirm no binding/focus/help collision and correct reopen behavior in Unity. |

None of these requires a product/spec/API change before the Tier-2 run on the evidence currently available.

## 8. Offline evidence assessment — PASS for this phase

The pushed handoff reports the exact candidate was checked with:

- `MFM08_BUILD_PASS`;
- `MFM08_TREATMENT_REVIEW_PASS checks=106 failed=0`;
- `MFM08_REPEAT_PASS` with two-process byte-identical output;
- `MFM08_BOUNDARY_PASS`;
- MFM-04 core `240/0`;
- MFM-05 candidates `95/0`;
- MFM-07 approval boundary `125/0`;
- repeated regression passes;
- a bounded external type-check of the Unity-facing files, explicitly caveated as **not Unity's compiler**.

The focused test source substantively checks the important static boundaries: no second coverage classifier, no map confirmation/bulk confirmation, proposal-not-mark semantics, waypoint-not-spot-check semantics, approval gating, no Work Plan approval, and pattern/shape distinction.

I accept this evidence for the **static/offline phase only**. I do not promote the external type-check or offline UI model into a Unity compile/render claim.

## 9. Mandatory Tier-2 residuals before integration

The #156 Unity gate must still prove, on the exact handed-off candidate:

1. clean Unity 6000.6.0f1 import/compile, including `Painter2D`, `ScrollViewMode` and all new `.meta` imports;
2. Treatment review rendered at 1280×720 and 1920×1080 with sample/four states/reasons/tags/legend readable and internal scrolling usable;
3. coverage footprints/hatching/tags align with the authoritative production coverage in the live stand and do not interfere with map squares, Rack V2 or existing marks;
4. candidate Previous/Next/Inspect, authoritative `[C]` confirmation and contextual `[N]` reject/reopen work without bulk action or input collision;
5. candidate/proposal states remain unmistakable from Fell/Crop/Rack marks by **form and text**, not merely colour;
6. Prepare creates no mark/order; preview creates no Fell mark; waypoint alone does not satisfy the spot-check; actual 3D tree inspection in an accepted still-Covered unit does;
7. prescription approval creates ordinary Pending work with provenance while the Work Plan remains unapproved, and repeated approval is blocked;
8. deterministic conflict/held presentation for manual Fell/open work/no-longer-covered cases;
9. affected Stand Map/Rack V2 and MFM-04/05/06/07 regressions remain green;
10. Reference Future v1/protected compatibility evidence remains unchanged under the existing applicable gates;
11. verification lane/save state is clean and the Unity lock is released normally;
12. no Windows build.

A real production defect found by Tier-2 is a stop/report condition under the current packet; do not weaken tests or convert this static approval into permission to integrate.

## 10. Manager disposition

The exact production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06` is **approved to proceed to its already-required Unity Tier-2 qualification**. It is **not approved for integration** on static/offline evidence alone.

Current serialized priority remains the existing #151 Unity gate. #156 may take the Unity lane only after #151 releases it and fresh coordination/lock/storage/process preflight is clean.

**Verdict:** `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`

**User action required:** none.

**Dashboard sync:** GitHub claim/handoff state is the authoritative sync target for this browser review; no external/generated dashboard ingestion is asserted.

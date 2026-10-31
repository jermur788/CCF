# MFM-08 Successor Tier-2 — Manager Review — 2026-10-31

**Parent claim:** #156  
**Returned verification head:** `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727`  
**Exact repaired production candidate:** `e5fb424635f16182748efcb0539e8a49539495a9`  
**Original feature start / worker baseline:** `cfdc93bf3943125d21570bc5749a9338f52d1cce`  
**Current Scenario One integration:** `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

## Verdict

**NEEDS CHANGES — VERIFICATION CLOSURE ONLY.**

The repaired MFM-08 feature behavior is accepted as qualified on its exact candidate, but #156 is not yet eligible for integration because one explicitly required Rack V2 regression was not run and the disclosed red Rack gates were compared only to the old feature start, which is 29 commits behind the current Scenario One integration lineage.

No MFM-08 production repair is requested by this review.

## Accepted evidence

The Manager accepts the following evidence from the returned handoff:

- exact candidate `e5fb4246...` executed in a clean detached registered Unity lane under the authoritative lock, with normal release and clean return;
- unmodified successor MFM-08 gate PASS at **353/353**;
- verification-only open-work conflict addition raises the gate to **373 PASS** without deleting or weakening existing assertions;
- repaired Low/Centre/High footprints, fixed teaching/sample footprint, non-stacked tags and area-specific Inspect-in-forest centres pass at both **1280×720** and **1920×1080**;
- Previous/Next bounds, real `[C]`/`[N]`, no bulk confirm, Prepare/preview noncommitment, 3D spot-check, Pending approval provenance, repeated-approval refusal and Work Plan commitment boundary pass;
- manual-Fell and open-work scenarios preserve the player's marks/orders and hold the affected tree out of proposals;
- MFM-06 save, MFM-07 approval, MFM-08A coverage, Reference Future v1/protected, save hardening, Scenario One completion and interaction pass;
- Rack V2 field-marking, runtime, save, Legacy40 rack authoring and confirm/minimap pass;
- no production file changed during qualification; no Windows build occurred.

The added open-work harness delta is bounded to `Tools/Verification/MfmTreatmentReviewUnity/MfmTreatmentReviewUnityGate.cs` plus the handoff. It creates an ordinary open work order through the existing mark → `AddMarkedTreesToWorkPlan()` path, verifies the review does not overwrite/add to that work, removes it through the ordinary path, and proves the original proposal signature returns.

The progress runner discrepancy is classified as **verification-runner policy debt**, not an MFM-08 gameplay failure: the gate itself emitted `SCENARIO_ONE_PROGRESS_VERIFY_PASS`, while the wrapper marked failure because an isolated `.bak` save was written; host saves were reported unchanged.

## Rack V2 red-gate disposition

The returned evidence is sufficient to show that MFM-08 did **not introduce** the observed `guided-v2` and `combined-v2` failures: they reproduce identically on `cfdc93bf...`, the exact feature start. `combined-legacy` is also not worsened by MFM-08 (candidate 440/8 versus parent 416/32).

However, `cfdc93bf...` is not the current integration baseline. Git ancestry shows current Scenario One integration `ce673714...` is **29 commits ahead** of `cfdc93bf...`, and that later lineage includes changes in relevant verification/teaching surfaces including `Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs`. Therefore the red gates must be dispositioned against `ce673714...` before the MFM-08 delta is merged onto that lineage.

## Blocking verification gap

The #156 release explicitly required Rack V2 authoritative map/runtime/field regressions. `RackV2MapPlanning` was not run. This is material because MFM-08 changes `StandMapView`, the same player-facing map surface.

This is the sole direct missing MFM-08 qualification gate identified by Manager review.

## Closure task

Bounded verification claim **#195 — MFM-08 Tier-2 Closure 01** is released.

It must:

1. run authoritative `RackV2MapPlanning` on exact MFM-08 candidate `e5fb4246...`;
2. run `guided-v2`, `combined-v2`, and `combined-legacy` diagnostically on current integration `ce673714...` with current repository gate sources;
3. identify whether the later integration lineage already contains the relevant correction or whether a separate current-lineage Rack V2 repair is required;
4. make no production/test weakening changes;
5. return lane/Editor/lock clean; no Windows build.

The already-green MFM-08 373-check run does not need repetition unless the closure worker finds a material ambiguity.

## Integration status

- MFM-08 feature behavior: **QUALIFIED ON `e5fb4246...`**.
- #156 Manager verdict: **NEEDS CHANGES — VERIFICATION CLOSURE ONLY**.
- MFM-08 production integration: **NOT YET AUTHORIZED**.
- MFM-09: remains blocked on accepted/integrated MFM-08.
- Windows: not authorized.

If #195 passes and current-lineage Rack failures are either green or cleanly separated into an independent pre-existing defect, Manager may upgrade #156 to APPROVED and perform the planned fresh-overlap integration onto the exact current Scenario One integration lineage.

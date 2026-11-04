# PARITY-08 — Save / Deterministic Compatibility — Integration Handoff

**Claim:** #126  
**Independent review:** #128 — APPROVED  
**Production candidate:** `3eb9c954c363659a1b012e06ce58a35ab87f4303`  
**Previous integration tip:** `b6b96cb3a51e80bd6b61df7dc73b1c1237950a32`  
**Integration merge:** `7d2049e862df5c9311748167423b981e713d1fa9`  
**Verdict:** **APPROVED / INTEGRATED**

## Merge construction

The integration merge has two parents: the current Scenario One integration lineage `b6b96cb3...` and the independently reviewed PARITY-08 candidate `3eb9c954...`.

Relative to the previous integration tip, the merge adds exactly the six reviewed candidate paths:

- `Assets/ForestPrototype/ForestSaveController.cs`
- `Assets/ForestPrototype/ForestSaveValidation.cs`
- `Tools/Verification/SaveHardeningVerification.cs`
- `Tools/Verification/Parity08Save/Parity08SpeciesOfflineCheck.cs`
- `Tools/Verification/Parity08Save/run_offline_check.py`
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md`

Relative to the PARITY-08 candidate, the merge preserves only the already-approved #165 verification/integration-side changes: the v24 historical gate expectation repairs, the accepted `progress`/`density-save` isolated-save runner allowances, and #165 handoff/integration documentation. No PARITY-08 production file was altered during integration.

## Accepted evidence

The independent review at `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md` approved the exact candidate after reviewing:

- pre-mutation unknown-species rejection shared by `CanLoad` and `LoadData`;
- offline species validation 23/23 plus mutation controls;
- Unity SaveHardening unknown-species/no-mutation witness;
- current v24 save/provenance and teaching-latch round trips;
- model-1 and model-2 compatibility gates;
- byte-identical protected hash dump;
- frozen Reference Future v1 load/preview/continuation;
- protected Legacy40 density anchors;
- protected Sitka timber anchor: fixtures 120, assertions 3007, evidence SHA-256 `88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.

No parity-specific save field or schema bump was introduced. Save schema remains v24. No Reference Future resource or protected anchor was rebaselined.

## Recorded non-blocking residual

The `completion-model0` diagnostic does not reproduce historical completion anchor `6F84AF319D301F87`. Claim #167 ran the identical verifier and runner against accepted pre-PARITY-08 base `185200f57140c65ed4d540ff0c4d173451170d14` and reproduced the candidate failure exactly, including completion hash `046551EB29730CE1`, v16 layout hash `20B85B0152EA747A`, and the same v17/v18 compatibility hashes. This is therefore pre-existing D-047/D-048 verification debt, not a regression introduced by #126. The historical anchor remains unchanged.

Positive old-model-0 save compatibility is independently exercised by the frozen Reference Future v1 save-v12 load and continuation path; current `LoadData` explicitly restores pre-v17 saves with `GrowthModel.Legacy`.

## Status

PARITY-08 is **IMPLEMENTED + UNITY-QUALIFIED FOR ITS ACCEPTANCE MATRIX + INDEPENDENTLY REVIEWED + INTEGRATED**.

This does not constitute PARITY-09, final Tier-3, milestone acceptance or user playtest acceptance. The merge itself has not been separately Unity-run; its production delta is exactly the already-qualified candidate and the other parent contributes only already-approved verification/documentation changes.

No Windows build.

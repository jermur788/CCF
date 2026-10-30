# PARITY-08 Independent Review — Final

**Claim:** #128  
**Target:** #126 PARITY-08 exact production candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`  
**Reviewer:** Overall Manager / OpenAI GPT-5.6 Sol, independent from Claude Opus 5.5 implementation  
**Verdict:** **APPROVED**

## Scope reviewed

Reviewed the exact `185200f5 -> 3eb9c954` diff, the implementation handoff, v24 release amendment, Unity qualification from #126/#165, residual #166 evidence, and baseline discriminator #167.

The production change is limited to:

- `Assets/ForestPrototype/ForestSaveValidation.cs`
- `Assets/ForestPrototype/ForestSaveController.cs`

plus focused verification/handoff files. No parity-specific save field or schema bump was introduced; save schema remains v24. No Reference Future resource, protected hash/anchor file, scene, prefab, package or ProjectSettings change is part of the candidate.

## Findings

### Unknown-species rejection / mutation ordering — PASS

`ForestSaveValidation.ValidateSpecies` rejects unknown non-empty species IDs on the authoritative Scenario One P0 paths used to build or execute state: trees, positive-density regeneration cohorts, planted juveniles, and Pending/Approved work orders. Empty IDs retain their historical default-species meaning and pre-field save versions retain their previous semantics.

Both `CanLoad` and `LoadData` call the same `FindLoadProblem` path. The species validation runs there before `LoadData` reaches its first world mutation (`ApplyStandGeometry`). `ForestTreeSpawner.ResolveSpecies` returns null for unknown or non-simulation-ready species, so those IDs are refused rather than silently treated as Sitka.

The Unity SaveHardening witness passed with the new unknown-species cases and no-mutation checks.

### Save/model compatibility — PASS

- current v24 save/provenance Unity gate: PASS, 159 checks;
- v24 teaching-completion latch round trip: PASS, 60 checks;
- pre-v23 historical save paths: covered by the save/provenance and frozen-reference gates;
- model 1 completion: clean PASS after #165 reconciliation;
- model 2 density/save path: clean PASS after #165 reconciliation;
- old model 0 save compatibility: accepted from the frozen Reference Future v1 contract. The archive is save v12, and current `LoadData` explicitly restores every pre-v17 save with `GrowthModel.Legacy` (0). The Reference Future v1 archive/load/preview/continuation gate is green on the exact candidate.

### Protected identity / anchors — PASS

- MFM hash dump: 18/18 lines byte-identical to the accepted baseline; no rebaseline;
- frozen Reference Future v1: PASS;
- protected Legacy40 density anchors: PASS after #165 reconciliation;
- protected Sitka timber anchor: PASS in #166 with exact match `fixtures=120`, `assertions=3007`, `evidenceSha256=88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.

No protected anchor or frozen reference was rewritten.

## Model-0 completion replay residual

The added `completion-model0` diagnostic does **not** reproduce historical completion anchor `6F84AF319D301F87`; however #167 reran the identical harness/runner against the accepted pre-PARITY-08 base `185200f57140c65ed4d540ff0c4d173451170d14` and reproduced the failure byte-for-byte:

- same Year-25 non-completion despite all eight objectives reporting ok;
- completion hash `046551EB29730CE1` on both;
- v16 layout hash `20B85B0152EA747A` on both;
- v17/v18 compatibility hashes identical on both.

Therefore this is **pre-existing D-047/D-048 verification debt, not a regression introduced by PARITY-08**. The historical anchor is not changed or rebaselined. The hypothesis that the diagnostic lacks the pre-calibration Oak/Beech reconstruction remains unverified and is not accepted as fact.

This residual does not block PARITY-08 because its approved scope is old-model save compatibility plus protected deterministic/reference anchors. Positive old-model-0 save/load behavior is independently exercised by the green frozen v12 Reference Future continuation, while the candidate-vs-base discriminator proves the failed completion diagnostic is not attributable to #126.

## Final disposition

**APPROVED.** Candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303` satisfies PARITY-08 for integration.

Status distinction:

- IMPLEMENTED: yes;
- UNITY-QUALIFIED for PARITY-08 acceptance matrix: yes, with the model-0 completion replay recorded separately as pre-existing verification debt;
- INDEPENDENTLY REVIEWED: yes;
- INTEGRATED: not by this review document;
- ACCEPTED: PARITY-08 feature acceptance approved for integration; milestone/player-test acceptance remains separate.

No Windows build.

Success token:

`PARITY-08 INDEPENDENT REVIEW APPROVED — READY FOR PARITY-09 DEPENDENCY CHECK`

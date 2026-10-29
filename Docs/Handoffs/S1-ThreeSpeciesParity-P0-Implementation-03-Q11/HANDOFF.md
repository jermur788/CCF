# HANDOFF — PARITY-03 Q11 evidence-record repair — 2026-10-28

**Task:** PARITY-03 Q11 — repair stale mixed-density verification records and evidence-label mapping  
**Claim:** #81  
**Parent follow-up:** #74  
**Executor:** Overall Manager / GPT-5.6 Sol / OpenAI  
**Lane:** GitHub/browser docs-only  
**Unity:** not used  
**Windows build:** not run

## Result

**PARITY-03 Q11 EVIDENCE RECORDS REPAIRED — DOCS-ONLY COMPLETE**

The non-canonical historical records under `Docs/Verification/ThreeSpeciesProgramme/` are now explicitly bounded so they cannot be mistaken for current parity authority.

## Changes

1. Added `Docs/Verification/ThreeSpeciesProgramme/PARITY-03-Q11-EvidenceRecordCorrections.md`.
   - marks old “growth model 2 is not the default” statements as superseded historical status;
   - records that old V3 Unity measurements remain evidence for their exact older tested lineage only and are not current-base requalification evidence;
   - maps legacy density tags into FR-12 without inventing unsupported equivalence:
     - `[ORD]` → `ORDINAL REPRESENTATION`;
     - `[C]` → `GAMEPLAY CALIBRATION`;
     - `[B]` → `TRANSFER` only where the record explicitly means transferred British Sitka evidence;
     - `[D]` → no automatic FR-12 mapping; reclassify each occurrence from its source/claim context;
   - records the density-specific interpretation for Route A and Route B;
   - keeps the core honesty claims intact: no Oak/Beech SDI, no biological m²/ha carrying-capacity claim, no DEC/DMC/mixture bonus;
   - explicitly leaves Q10 continuity-strengthening and verification-scanner policy open.

2. Updated `Docs/Verification/ThreeSpeciesProgramme/README.md`.
   - adds a prominent historical/non-canonical warning and points readers to the correction note;
   - labels old check counts as historical snapshots, not current acceptance totals;
   - marks the old phase-status table as historical programme state rather than current milestone status;
   - prevents later density/canopy records from being treated as current-base evidence unless a later accepted handoff adopts them.

## Authority/evidence checked

- Approved `specs/scenario-one-three-species-parity-p0/spec.md`, FR-10 and FR-12.
- Accepted independent PARITY-03 static review Q11 finding.
- Current source still sets `ScenarioOneManager.NewGameGrowthModel = GrowthModel.SpeciesAware`; old “model 2 not default” wording is therefore historical, not current.

## No production or verification change

No production C#, species assets, scenes, save/schema, density arithmetic, verification harness, Rack, MFM, S1-C or S1-D files were changed. No test was weakened, no anchor was rebaselined and no new Unity claim is made.

## Remaining PARITY-03 follow-up

Q10 remains open under #74. It should strengthen the continuity verification only:

- Legacy40 + Enlarged80;
- Intermediate + High replacements;
- heterogeneous DBH/CI in a realistic low-loss regime;
- meaningful relative tolerance;
- controller-level 0→1 non-Reference continuity measurement.

Existing PARITY-03 production and its accepted Unity qualification remain valid; Q10 is evidence-strengthening, not a production retune.

## Disposition

- Q11 docs repair: **IMPLEMENTED / REVIEWED / INTEGRATED** (Manager direct docs lane).
- Unity verification: **NOT APPLICABLE** to this docs-only correction.
- Q10 continuity-strengthening: **OPEN / WAITING**.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**

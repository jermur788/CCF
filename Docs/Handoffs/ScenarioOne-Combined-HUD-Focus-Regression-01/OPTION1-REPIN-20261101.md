# Scenario One Combined HUD / Focus Regression Repair 01 — Option 1 Exact-Candidate Repin — 2026-11-01

**Claim:** #199  
**Parent:** #91 / browser #200 / local orchestration #201/#203  
**Base continuation:** `OPTION1-CONTINUATION-PACKET.md`  
**Prior repin:** `OPTION1-REPIN-20261031.md`  
**Status:** **REPINNED / READY — QUEUED PRE-TIER-3; NOT CURRENT UNITY PRIORITY**

MFM-09 has been Manager-approved and the published Scenario One integration has moved. This amendment supersedes the `34e360f8...` exact-start pin in the prior repin; all Option 1 scope/protected assertions remain binding.

## Exact start

Future #199 production work must start from:

**`6784ab17cac922a16815f4898a30737482101018`**

Do not branch production work from stale partial `42742a8a`, `ce673714...`, or `34e360f8...`.

## MFM-09 overlap note

MFM-09 changed only:

- `ScenarioOneTeachingSequence.cs`
- `LearningObjectivesView.cs`
- `TeachingOverlayView.cs`
- `WorkPlanView.cs`

It did not change `ScenarioOneUiRoot.cs` or `TreeInspectionView.cs`, the principal expected Option 1 production surfaces. Therefore no direct production-file overlap was found, but #199 must preserve the integrated MFM-09 teaching/Work Plan behavior and must run affected regressions on the new lineage.

MFM-08 still owns accepted `TreeInspectionView` treatment-review behavior from the earlier repin. Start fresh from the new integration head and do not cherry-pick stale production hunks.

## Binding Option 1 contract

Preserve:

1. ordinary estimate inspection keeps the minimap visible;
2. accepted PARITY-07 estimate information/copy;
3. central-prompt/detail-column clearance;
4. protected Combined minimap/focus assertions;
5. repair of reproduced 1920 V2 competitor-list clipping if still present;
6. responsive/reflow solution rather than copy deletion or assertion relaxation;
7. Rack V2, Legacy/model-0, MFM-08 treatment review and MFM-09 teaching/Work Plan behavior.

No Rack geometry/marking/persistence, MFM semantics, save/schema/anchors/reference, ecology or economy changes.

## Scheduling

#199 remains READY / QUEUED and required before Tier-3. MFM-10 #211 is the current preferred programme task. Do not create overlapping UI ownership; refresh live coordination immediately before starting #199.

Independent review remains mandatory after production repair.

Success token remains:
`SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`.

No Windows build. #194 remains NO ACTION.

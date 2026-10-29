# MFM-08A source-authority review — browser result

**Claim:** #146  
**Date:** 2026-10-29  
**Lane:** browser/GitHub only  
**Verdict:** **APPROVED AS ADMINISTRATIVE CLOSURE — DIRECT MANAGER DISPOSITION IS BINDING**

## Scope and authority

This result does **not** reopen or replace the MFM-08A source-authority decisions. The substantive ambiguity was already disposed directly by the Overall Manager on #129 before this stale browser claim completed. This file makes that existing authority durable in the expected #146 result location and closes the administrative gap only.

Inspected:
- `Docs/Project/coordination/active-tasks.md`;
- #129 and Manager comments `6509104711` and `6509324739`;
- #129 pushed worker handoff on `feature/mfm-v1-coverage-world-adapter-v1`, `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/HANDOFF.md`;
- `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/TASK-PACKET.md`;
- `Docs/Handoffs/MFM-08A-Source-Authority-Review-20261029/BROWSER-HANDOFF.md`;
- #146 and #148.

## Binding source mapping already decided

| Mask/source | Binding authority | Geometry / semantic rule | #129 permitted surface |
|---|---|---|---|
| `Headland` | existing public `ScenarioOneHeadlandGeometry` authority; not part of the disputed Manager ambiguity | use the existing authoritative headland geometry; do not invent alternate calibration | read only |
| `Track` | Manager #129 comment `6509104711` | authoritative Dirt Path geometry + existing `ForestStartingStand.pathCorridorRadiusMeters` | minimal read-only accessor allowed; no reflection/copied literal |
| `Clearing` | Manager #129 comment `6509104711` | authoritative `Forest Clearing` position + existing `ForestStartingStand.clearingRadiusMeters`; visual scale is not authority | minimal read-only accessor allowed |
| `Site` | Manager #129 comment `6509104711` | durable buildable/structure site positions + existing `clearingRadiusMeters` | read existing authoritative structure positions + accessor |
| player-start portion of `Site` | Manager #129 comment `6509104711` | **excluded from runtime MFM coverage**; moving player transform is not a durable start anchor | no spawn/save field; no moving-player mask |

The Manager also approved `ScenarioOneManager.ReadCoverage` only as a thin side-effect-free read-only façade over the same production world-read + `ScenarioOneCoverage.Classify` path, with no cache, persistence, fallback or alternate coverage semantics (`6509324739`). Protected-area polygon sampling was rejected; current Scenario One emits zero `ProtectedArea` features and must fail closed if an authoritative polygon appears until a separately authorised mapping exists. Shelter/protection objects are included by physical current-world existence, not by current ecological-effect expiry.

## Product/spec impact

No Decision Log, feature spec, plan, calibration, save/schema, Reference Future or product-semantic change is required. These are already-recorded Manager implementation-source dispositions within the accepted MFM contract.

## #129 status at this reconciliation

#129 has since published an exact pushed worker handoff:
- gated production candidate `716486a21588e76a482d3eaea4fed12069f75ffd`;
- docs-only handoff successor `0b33c3288ebc78ab7361d0cdb3fcee537324dadb`;
- offline gates reported green;
- exact Unity evidence reported for compile, coverage-world, MFM-07 approval, MFM-06 save, protected hash comparison and Reference Future;
- success token `MFM COVERAGE-WORLD ADAPTER COMPLETE — MFM-08 UX MAY RELY ON PRODUCTION COVERAGE`.

That is a **worker handoff**, not Manager approval or integration. #129 may be closed as `[HANDED-OFF]`; #156 remains blocked until Manager review/APPROVAL, integration and repin of the exact accepted lineage.

## #148 disposition

#148's planned independent source-authority Manager review has no remaining independent decision to make: the Overall Manager directly issued the binding source-authority dispositions on #129 before #146 produced a result. #148 is therefore **SUPERSEDED** and should close without issuing a duplicate APPROVED/NEEDS CHANGES/REJECTED semantic verdict.

## User action

None.

No C#/Unity/save/schema/spec/Windows-build work was performed by this browser closure.

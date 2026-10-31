# Scenario One Combined HUD / Focus Regression Repair 01 — Option 1 Exact-Candidate Repin — 2026-10-31

**Claim:** #199  
**Parent:** #91 / browser #200 / local orchestration #201/#203  
**Base continuation:** `OPTION1-CONTINUATION-PACKET.md`  
**Status:** **REPinned / READY — QUEUED PRE-TIER-3; NOT CURRENT UNITY PRIORITY**

The post-MFM-08 start blocker is cleared. This amendment supersedes the stale `ce673714...` base assumption in the continuation packet.

## Exact start

Future #199 production work must start from current published Scenario One integration:

**`34e360f85835afd15f5323f287372e0f2ee83dd9`**

Do not branch production work from stale partial `42742a8a` or old `ce673714...`.

## MFM-08 overlap note

MFM-08 integration changed `Assets/ForestPrototype/UI/TreeInspectionView.cs`, one of the likely #199 presentation surfaces. Therefore:

- start from fresh `34e360f8...`;
- inspect current `TreeInspectionView` before edits and preserve MFM-08 treatment-review/3D candidate behavior;
- do **not** cherry-pick stale production hunks from the old branch;
- the accepted verification-fixture correction in `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` may be replayed only after checking it remains applicable on the new lineage;
- if satisfying Option 1 requires changing MFM-08 behavior rather than presentation composition, STOP for Manager disposition.

## Binding Option 1 contract

Preserve both accepted contracts:

1. ordinary estimate inspection keeps the minimap visible;
2. accepted PARITY-07 estimate information/copy is retained;
3. PARITY-07 central-prompt/detail-column clearance remains green;
4. protected Combined minimap/focus assertions retain their meaning;
5. the reproduced 1920 V2 competitor-list clipping is repaired if still present;
6. prefer responsive/reflow layout rather than copy deletion or assertion relaxation;
7. preserve Rack V2 and model-0/Legacy compatibility.

No Rack geometry/marking/persistence, MFM semantics, save/schema/anchors/reference, ecology or economy changes.

## Scheduling

#199 is READY/QUEUED but is not the preferred next serialized Unity action. #206 S1-C T-08 retains that slot. #203 may schedule #199 later when ownership/Unity serialization permits without displacing the established S1-C/S1-D critical path.

Independent review remains mandatory after production repair.

Success token remains:
`SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`.

Dispatch when scheduled:

`Resume #199 on exact published integration 34e360f85835afd15f5323f287372e0f2ee83dd9 using OPTION1-CONTINUATION-PACKET.md plus OPTION1-REPIN-20261031.md. Start fresh rather than cherry-picking stale production hunks; preserve MFM-08 TreeInspection behavior, protected Combined minimap/focus assertions and PARITY-07 estimate/prompt/detail requirements. Restore ordinary-inspection minimap visibility and repair the 1920 competitor clipping through bounded responsive/reflow changes, run both combined modes at both resolutions plus PARITY-07 and affected regressions, obtain independent review, and return the exact success token or smallest contract blocker. No Windows build.`

# 2D Phase 1 — Parallel Work Admission Assessment and Draft Spec Handoff

**Author:** CCF Overall Manager (ChatGPT browser GitHub connector), 2026-11-24.  
**Verdict:** Parallel planning YES; parallel map-disjoint source tranche POSSIBLE ONLY AFTER spec approval and exclusive lane admission. No production code has been edited, built, tested, reviewed, merged or owner-accepted by this audit.

## Owner request
Start 2D Phase 1 (save/load; advance one year or suggested management review; decision and outcome history) while 2D Multi map integration remains in progress; keep map changes independent. Phase 1 precedes the Option 3 forest-maker/standard five-rack 0.64 ha economy/labour calibration.

## Verified GitHub context
- Phase2 candidate `feat/multi-phase2-oak-beech-20261124-v1` @ `cf6fe7a52aa096e824827c568f43fe05724dcce9`; closed in bounded Core qualification, not asserted merged into `main`.
- Map Pass1 `feat/multi-map-integration-pass1-20261124-v1` descends from Phase2, 5 commits ahead in observed comparison, implementation handoff says READY FOR INDEPENDENT REVIEW, NOT INTEGRATED/ACCEPTED. It changed Core `MultiAreaContinuation.cs`, `MultiAreaContracts.cs`, `MultiAreaEcology.cs`, `MultiAreaExperiment.cs`, `MultiAreaUiReadout.cs`, `MultiAreaWorkPlanAdapter.cs`, plus UI, server, test registration and smoke files. Therefore 'not touching the map screen' is not a reliable file-ownership separation.
- Live `main/Docs/Project/coordination/active-tasks.md` consulted, but it does not establish current unreported local physical leases or a map Pass2 grant; any writer admission must refresh both live coordination and local Ops records.
- The `MultiAreaRoot` in Phase2 has year/revision/areas/events/executed keys/pending and approved orders/annual history and optional Stage4/Stage5/Phase2 roots. `Advance` does a genuine shared annual step and may cancel outstanding orders only with *explicit* consent. A skip must not silently supply that consent.

## Proposed safe split
1. **1A0 NOW AFTER APPROVAL**: a new branch/worktree and new-file-only `src/Core/Phase1/` save envelope/strict round-trip/validator/storage; non-overlapping test files. No alterations to any existing source, map, UI, source registration or legacy fixtures. Separate Git history, no merge onto map writer's branch. Declare unrun tests where existing registration is blocked.
2. **1A1 AFTER MAP FILES RELEASED**: integrate exact-root capture/restore, session fence reset, history persistence and genuine multi-tick controller into shared Core + API with single writer; test against a settled map baseline.
3. **1B AFTER MAP REVIEW**: controls in current `/multi`, paged history with size budget, review/test/owner playtest.

## Authority and open decisions
The user previously selected Phase 1 before forest creation and requested parallelism; this does **not** approve a technical source spec or auto-apply 3D Scenario One D-077 to 2D. The four-file quartet under `specs/multi-2d-phase1-save-time-history-v1/` is DRAFT. Six A/B decisions are at its `spec.md` §7 (slot style, migration, skip target, history retention, delivery split, approved-plan reload). Recommended A in all six; awaiting explicit owner choice or adoption of defaults. No implementation task, file claim, physical Ops reservation or agent dispatch created by the draft.

## Next actions
1. Owner reviews/approves draft and decisions.
2. Manager rechecks current `active-tasks.md`, map Pass2 scope and physical Ops lease; appoints ONE qualified implementation writer and grants a new separate worktree/branch with exact base and new-file-only allowlist. Register task RUNNING only when actually admitted.
3. Implement 1A0, independently review and hand off. Return for 1A1/1B only after the map's overlapping file ownership is released and G4 integration is authorised.

**Task-ready constraints:** SPEC REQUIRED, single source writer, separate worktree, new files under `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**` and new tests/evidence only, no existing code, no `Assets/**`, no map UI, no `main` merge, no Windows build. Stop if a shared existing file is needed; deliver partial isolated component and report blocker. This is a proposed packet, **not a live claim**.

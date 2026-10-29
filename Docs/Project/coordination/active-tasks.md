# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol after #129 Manager approval/integration and #159 browser reconciliation. The #159 result was correct when published but became stale immediately afterward because #129 then completed Manager review and integration. This ledger reflects the later authoritative state.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- MFM-07 remains integrated/verified through `c8cfae8c4025cec6e960a050edf4bfde2a981de5` (save schema v23).
- MFM-08A #129 is **MANAGER-APPROVED AND INTEGRATED**. Reviewed production candidate: `716486a21588e76a482d3eaea4fed12069f75ffd`; worker docs successor: `0b33c3288ebc78ab7361d0cdb3fcee537324dadb`; Manager review: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/MANAGER-REVIEW.md`; integration production merge: `e19f026d0fa4978e89836caa5f0ec94cba61cd59`; integration handoff/docs successor: `182c2c480465a34b269a07ba654d98669c8d8436`. #129 is closed and owns nothing.
- #129 qualification evidence accepted by Manager: offline MFM coverage-world/MFM-04/05/07 green; Unity compile, coverage-world 56, approval 106, MFM-06 save 159, protected hash dump identical 18/18, Reference Future v1 PASS. Accepted residuals are recorded in the worker handoff.
- Storage remediation #152 is complete/closed. Historical handoff restored ZX20 above 20 GiB; every Unity task must still fresh-check current free space.
- S1-C T-06 #147: exact production candidate `d959e0397c80bc7a9d7a707c8d4f437f946231f4`, docs successor `769ac28a08d47acb7dce727d1188025e56a2b3b8`; **offline Manager-approved**. Two-resolution rendered/waypoint Unity qualification and affected Unity regressions remain.
- D-066 repair #145 and independent Manager review #158 are complete/approved.
- S1-D T-04P #143 is **RUNNING — OFFLINE IMPLEMENTATION**. Executor recorded: T3 Code / Codex harness / OpenAI gpt-6.1-sol high; branch `feature/s1-d-t04-teaching-persistence-v1`; worktree `/media/jer/Videos/CCF-S1D-T04-Persistence`; exact start `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; v24 was confirmed free at task start. #143 is sole bounded save/schema/hash writer for the one teaching latch.
- MFM-08 #156 is now **RELEASED FOR NON-UNITY IMPLEMENTATION** from exact integrated start `182c2c480465a34b269a07ba654d98669c8d8436`; its Unity/rendered qualification waits behind #147. It owns no Annual Review/history/save/schema/MFM-09 curriculum seams.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | orchestration/integration | orchestration + child lanes | orchestration only | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING** |
| **S1-C T-06 prior-treatment guidance** (#147) | T3 Code / Unity | `feature/s1-c-t06-prior-treatment-guidance-v1`; `/media/jer/Videos/CCF-S1C-T06` | history/review presentation seam | **OFFLINE APPROVED — NEXT UNITY PRIORITY** |
| **S1-D T-04P teaching latch** (#143) | T3 Code / save | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | sole bounded save/schema/hash writer for one latch | **RUNNING — OFFLINE** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | released Stand Map / bounded inspection UI after worker start record | **READY FOR OFFLINE IMPLEMENTATION — UNITY WAITS ON #147** |
| **S1-D T-05 previous vs proposed** (#151) | local UI after #147 | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | none while blocked | **BLOCKED ON #147 FINAL HANDOFF** |
| **S1-D T-04 guide monotonicity** (#124) | curriculum after #143 | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | none while blocked | **BLOCKED ON ACCEPTED/INTEGRATED #143** |
| **PARITY-08 compatibility** (#126) | save verification later | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | none while blocked | **BLOCKED ON S1-D LINEAGE** |
| **PARITY-08 independent review** (#128) | review only | future exact #126 candidate | none | **BLOCKED ON #126** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING UNITY FOLLOW-UP** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## Single-writer boundaries

### #143 / #124 / #126

#143 owns only the minimum v24 one-way historical teaching-completion latch and required capture/restore/hash-exclusion plumbing. No cycle keys/counters/lists, J/K/session detail, retrospective reconstruction or hash rebaseline. The latch writes false→true only at the genuine pre-latch `response → recruitment` transition after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. Full saves round-trip it; every world/reference/protected identity projection strips it. #124 and #126 remain blocked until #143 is qualified, independently reviewed, Manager-approved and integrated.

### #147 / #151

#147 remains the sole Annual Review/history presentation writer until final qualification/handoff. #151 owns nothing until #147 releases that seam and Manager repins its exact lineage.

### #156

#156 may start from `182c2c480465a34b269a07ba654d98669c8d8436` after fresh ownership check and a `[RUNNING — OFFLINE]` claim record. Expected ownership is Stand Map plus the smallest MFM-specific/inspection presentation helpers. Prefer no `ScenarioOneUiRoot.cs`; if required, stop for Manager overlap assignment. No Annual Review/history, S1-D Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk confirmation changes. Unity waits behind #147.

## Dependency-ready sequence

1. **#147 Unity qualification now** → Manager final review/integration → release #151.
2. **#143 continues offline in parallel** → later Unity + independent implementation review → Manager review/integration → release #124.
3. **#156 may start offline in parallel now** from exact integrated #129 lineage; Unity waits behind #147.
4. After #143 + #124, repin/release #126 PARITY-08, then #128 independent review.
5. MFM-09 follows accepted MFM-08 plus accepted S1-D T-04/T-05.
6. #74 Q10 is evidence-strengthening only and stays behind critical-path Unity work.
7. Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate; then user personal playtest. Windows build only after explicit post-playtest authorisation.

## Durable current records

- #129 worker handoff: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/HANDOFF.md`
- #129 Manager review: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/MANAGER-REVIEW.md`
- #129 integration handoff: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/INTEGRATION-HANDOFF.md`
- #143 execution handoff: `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/EXECUTION-HANDOFF-20261029.md`
- #153 MFM-08 readiness: `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`
- #159 coordination reconciliation: `Docs/Handoffs/Browser-Coordination-Reconciliation-20261029/BROWSER-RESULT.md` (historical snapshot; superseded where #129 later advanced)

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
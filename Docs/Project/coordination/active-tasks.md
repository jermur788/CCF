# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol after S1-C T-06 #147 Manager approval/integration and S1-D T-05 #151 release.

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
- S1-C T-06 #147 is **MANAGER-APPROVED, INTEGRATED AND CLOSED**. Reviewed production merge: `d959e0397c80bc7a9d7a707c8d4f437f946231f4`; qualified successor `769ac28a08d47acb7dce727d1188025e56a2b3b8`; final worker handoff `5a0fc5190e2d47a02997706c68e38958a3a3bb3f`; exact integration production merge `625e0adf5e7fd71184cb074e85e1901b417ec02d`; integration handoff/docs successor **`cfdc93bf3943125d21570bc5749a9338f52d1cce`**. Accepted qualification: 1,224 Unity checks, 66 captures across 1280×720 and 1920×1080, faithful recorded-location waypoint and missing-location/no-invented-waypoint cases, read-only earlier-year browsing, and affected T-02/T-03/T-04 regressions. #147 owns nothing.
- Storage remediation #152 is complete/closed. Historical handoff restored ZX20 above 20 GiB; every Unity task must still fresh-check current free space.
- D-066 repair #145 and independent Manager review #158 are complete/approved.
- S1-D T-04P #143 is **RUNNING — OFFLINE IMPLEMENTATION**. Executor recorded: T3 Code / Codex harness / OpenAI gpt-6.1-sol high; branch `feature/s1-d-t04-teaching-persistence-v1`; worktree `/media/jer/Videos/CCF-S1D-T04-Persistence`; exact start `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; v24 was confirmed free at task start. #143 is sole bounded save/schema/hash writer for the one teaching latch.
- MFM-08 #156 is **RELEASED FOR NON-UNITY IMPLEMENTATION** from exact integrated start `182c2c480465a34b269a07ba654d98669c8d8436`. #147 no longer blocks its Unity phase, but every Unity attempt still requires fresh machine/priority preflight. It owns no Annual Review/history/save/schema/MFM-09 curriculum seams.
- S1-D T-05 #151 is **RELEASED FOR NON-UNITY IMPLEMENTATION** from exact integrated start `cfdc93bf3943125d21570bc5749a9338f52d1cce`; no production ownership exists until a worker records `[RUNNING — OFFLINE]` after fresh ownership checks.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | orchestration/integration | orchestration + child lanes | orchestration only | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING** |
| **S1-D T-04P teaching latch** (#143) | T3 Code / save | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | sole bounded save/schema/hash writer for one latch | **RUNNING — OFFLINE** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | released Stand Map / bounded inspection UI after worker start record | **READY FOR OFFLINE IMPLEMENTATION** |
| **S1-D T-05 previous vs proposed** (#151) | local UI | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | none until worker start record | **READY FOR OFFLINE IMPLEMENTATION** |
| **S1-D T-04 guide monotonicity** (#124) | curriculum after #143 | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | none while blocked | **BLOCKED ON ACCEPTED/INTEGRATED #143** |
| **PARITY-08 compatibility** (#126) | save verification later | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | none while blocked | **BLOCKED ON S1-D LINEAGE** |
| **PARITY-08 independent review** (#128) | review only | future exact #126 candidate | none | **BLOCKED ON #126** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING UNITY FOLLOW-UP** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## Single-writer boundaries

### #143 / #124 / #126

#143 owns only the minimum v24 one-way historical teaching-completion latch and required capture/restore/hash-exclusion plumbing. No cycle keys/counters/lists, J/K/session detail, retrospective reconstruction or hash rebaseline. The latch writes false→true only at the genuine pre-latch `response → recruitment` transition after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. Full saves round-trip it; every world/reference/protected identity projection strips it. #124 and #126 remain blocked until #143 is qualified, independently reviewed, Manager-approved and integrated.

### #151 / S1-C history

#147 is complete and releases the Annual Review/history seam. #151 may now start from `cfdc93bf3943125d21570bc5749a9338f52d1cce` after fresh ownership checks and a `[RUNNING — OFFLINE]` record. Primary ownership is `WorkPlanView.cs`; reuse the integrated S1-C exact-year/history navigation. Prefer no `AnnualReviewView.cs` edit. If a missing tiny navigation API is proven, stop for Manager overlap disposition before touching that file. No save/schema/history-store duplication or MFM-09 curriculum ownership.

### #156

#156 may start from `182c2c480465a34b269a07ba654d98669c8d8436` after fresh ownership check and a `[RUNNING — OFFLINE]` claim record. Expected ownership is Stand Map plus the smallest MFM-specific/inspection presentation helpers. Prefer no `ScenarioOneUiRoot.cs`; if required, stop for Manager overlap assignment. No Annual Review/history, S1-D Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk confirmation changes.

## Dependency-ready sequence

1. **#143 continues offline** → later Unity + mandatory independent implementation review → Manager review/integration → release #124.
2. **#151 may start offline now** from exact post-S1-C integration `cfdc93bf...`; qualify/review/integrate its bounded repeated-intervention Work Plan presentation.
3. **#156 may start offline now** from exact post-MFM-08A integrated lineage; its Tier-2 Unity/rendered qualification runs when fresh machine priority permits.
4. After #143 + #124, repin/release #126 PARITY-08, then #128 independent review.
5. MFM-09 follows accepted MFM-08 plus accepted S1-D T-04/T-05.
6. #74 Q10 is evidence-strengthening only and stays behind critical-path Unity work.
7. Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate; then user personal playtest. Windows build only after explicit post-playtest authorisation.

## Durable current records

- #129 worker handoff: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/HANDOFF.md`
- #129 Manager review: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/MANAGER-REVIEW.md`
- #129 integration handoff: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/INTEGRATION-HANDOFF.md`
- #147 worker handoff: `Docs/Handoffs/S1-C-Implementation-06/HANDOFF.md`
- #147 Manager review: `Docs/Handoffs/S1-C-Implementation-06/MANAGER-REVIEW.md`
- #147 integration handoff: `Docs/Handoffs/S1-C-Implementation-06/INTEGRATION-HANDOFF.md`
- #143 execution handoff: `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/EXECUTION-HANDOFF-20261029.md`
- #151 release handoff: `Docs/Handoffs/S1-D-Implementation-05/RELEASE-HANDOFF-20261029.md`
- #153 MFM-08 readiness: `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`
- #159 coordination reconciliation: `Docs/Handoffs/Browser-Coordination-Reconciliation-20261029/BROWSER-RESULT.md` (historical snapshot; superseded where later tasks advanced)

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
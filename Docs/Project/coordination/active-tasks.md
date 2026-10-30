# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30T00:50Z by DeepSeek V4.1 Flash overnight orchestrator after integrating #143, approving #161 and dispatching #124/#151/#156 offline. Live claims and handoffs outrank this table.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- Browser-capable phases must be separated from local implementation/Unity work wherever practical; #162 is the overnight browser lane.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- MFM-07 remains integrated/verified through `c8cfae8c4025cec6e960a050edf4bfde2a981de5` (save schema v23).
- MFM-08A #129 is **MANAGER-APPROVED AND INTEGRATED**. Reviewed production candidate `716486a21588e76a482d3eaea4fed12069f75ffd`; integration production merge `e19f026d0fa4978e89836caa5f0ec94cba61cd59`; integration handoff/docs successor `182c2c480465a34b269a07ba654d98669c8d8436`. #129 is closed and owns nothing.
- S1-C T-06 #147 is **MANAGER-APPROVED, INTEGRATED AND CLOSED**. Reviewed production merge `d959e0397c80bc7a9d7a707c8d4f437f946231f4`; final worker handoff `5a0fc5190e2d47a02997706c68e38958a3a3bb3f`; exact integration production merge `625e0adf5e7fd71184cb074e85e1901b417ec02d`; integration handoff/docs successor **`cfdc93bf3943125d21570bc5749a9338f52d1cce`**. Accepted qualification: 1,224 Unity checks and 66 captures across 1280×720 and 1920×1080. #147 owns nothing.
- Storage remediation #152 is complete/closed. Historical handoff restored ZX20 above 20 GiB; every Unity task must still fresh-check current free space.
- D-066 repair #145 and independent Manager review #158 are complete/approved.
- S1-D T-04P #143 is **INTEGRATED**, not yet combined-Tier-3 qualified. Independent #161 review **APPROVED** exact production `b9bf11aaf8f6fd7ea43e33e39258560408b88b12` (docs `d8168926`); integration production merge **`2f476a52`**; integration docs successor **`bccd2b90b3eafc3728a9f29ac421b2719d042d72`**. Six production files are byte-identical to the candidate; only a verification-tool save allowlist conflict was resolved as a deliberate union (`coverage-world` + `teaching-latch`). Post-merge runtime-source compile: 184 sources, 0 errors. Save schema v24. No hash/reference rebaseline. #143 owns nothing now.
- **#124 S1-D T-04 guide monotonicity is RUNNING — OFFLINE** from `bccd2b90`, branch `feature/s1-d-t04-guide-monotonicity-v2`, worktree `/media/jer/Videos/CCF-S1D-T04`, provider DeepSeek V4.1 Flash. It consumes only the integrated historical-completion read; no save/schema/hash ownership. Old `b9d204c` is evidence only.
- MFM-08 #156 is **RUNNING — OFFLINE** from exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce`, branch `feature/mfm-08-map-3d-review-ux-v1`, worktree `/media/jer/Videos/CCF-MFM08`, provider Claude B Sonnet 5.5 medium. Unity waits for its pushed candidate and a fresh serialized machine preflight.
- S1-D T-05 #151 is **RUNNING — OFFLINE** from exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce`, branch `feature/s1-d-t05-previous-proposed-v1`, worktree `/media/jer/Videos/CCF-S1D-T05`, provider OpenAI B gpt-6.1-sol high. Unity waits for its pushed candidate and the serialized lane.
- #124 is ACCEPTED/INTEGRATED at `185200f5`: offline review APPROVED; Unity guide 93 checks; #164 reconciled stale harnesses with all gates exit 0 (TeachingCopy 61, RackLesson 36, teaching-latch 60 x3, S1-C history 279, T-04 529, T-06 170); production unchanged; compile 184/0.
- #126 PARITY-08 is RUNNING — OFFLINE on Claude Opus 5.5 Low from `185200f5`; save schema v24; no parity schema bump/rebaseline. #128 remains blocked on #126.
- #151 is offline-approved and Unity-pending; #156 is offline-candidate and Unity-pending. Unity is free but reserved for #126 qualification priority. Intermittent Unity Editor exit hang remains a rerun risk (2/5 teaching-latch launches), never waived.
- #151 was cut off by OpenAI B subscription exhaustion after pushing production `98ac98b5`; an uncommitted verification directory is preserved. DeepSeek resumed the same branch.
- #156 offline candidate is production `d55ec86a` / docs `d70cec17`; Unity Tier-2 remains pending and was not run.
- Unity is currently free (no Editor/process observed); ZX20 free space was 20 GiB at reconciliation, at the practical floor; every acquisition must fresh-check >=15 GiB mandatory / >=20 GiB preferred and stop otherwise.
- Overnight continuation handoff: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-CONTINUATION-20261030.md`, publication commit `4112718310cc842d9ba2d677bc55c4cb450e8bcb`.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration | orchestration + child lanes | orchestration only | **RUNNING** |
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING** |
| **S1-D T-04P teaching latch** (#143) | save/Unity | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | none (integrated, stopped) | **INTEGRATED at `2f476a52` / docs `bccd2b90`; combined Tier-3 pending** |
| **#143 independent implementation review** (#161) | browser-first independent review | exact candidate `b9bf11aa` + docs `d8168926` | read-only | **APPROVED** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **OFFLINE CANDIDATE `d55ec86a`; UNITY PENDING** |
| **S1-D T-05 previous vs proposed** (#151) | local UI | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | `WorkPlanView.cs` + smallest S1-C nav hook | **OFFLINE APPROVED; UNITY PENDING** |
| **S1-D T-04 guide monotonicity** (#124) | curriculum after #143 | `feature/s1-d-t04-guide-monotonicity-v2`; `/media/jer/Videos/CCF-S1D-T04` | none (accepted/integrated) | **ACCEPTED / INTEGRATED at `185200f5`** |
| **S1-D v24 harness reconciliation** (#164) | verification/Unity | `verify/s1d-v24-harness-reconciliation-v1`; `/media/jer/Videos/CCF-S1D-HarnessRecon` | none (integrated) | **PASS / INTEGRATED at `185200f5`** |
| **PARITY-08 compatibility** (#126) | save verification | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | save validation/determinism after start record | **RUNNING — OFFLINE (Claude Opus 5.5 Low)** |
| **PARITY-08 independent review** (#128) | review only | future exact #126 candidate | none | **BLOCKED ON #126** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING UNITY FOLLOW-UP** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## Single-writer boundaries

### #143 / #124 / #126

#143 is integrated and owns nothing. Its v24 one-way latch is the only new saved fact; all world/reference/hash projections strip it; anchors are unchanged. #161 approved it. **#124** now owns only the monotone post-latch guide consumption and focused verification; it must not write/validate/reconstruct the latch, add persistence, or touch save/schema/hash. **#126** remains blocked until #124 is accepted/integrated and then must be browser-repinned to the actual post-#143/#124 lineage before any save work.

### #151 / S1-C history

#147 is complete and releases the Annual Review/history seam. #151 may start from `cfdc93bf3943125d21570bc5749a9338f52d1cce` after fresh ownership checks and a `[RUNNING — OFFLINE]` record. Primary ownership is `WorkPlanView.cs`; reuse the integrated S1-C exact-year/history navigation. Prefer no `AnnualReviewView.cs` edit. If a missing tiny navigation API is proven, stop for overlap disposition before touching that file. No save/schema/history-store duplication or MFM-09 curriculum ownership.

### #156

#156 may start from `cfdc93bf3943125d21570bc5749a9338f52d1cce` after fresh ownership checks and a `[RUNNING — OFFLINE]` claim record. Expected ownership is Stand Map plus the smallest MFM-specific/inspection presentation helpers. Prefer no `ScenarioOneUiRoot.cs`; if required, stop for Manager overlap assignment. No Annual Review/history, S1-D Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk confirmation changes.

### Browser lane #162

#162 owns no production files. It is the overnight route for GitHub inspection, static/independent review, coordination, packets, claim reconciliation, downstream repinning and safe browser-side integration preparation. Every substantive feature/review still uses its own child claim. Mixed tasks must be split rather than consuming a local lane for browser-only phases.

## Dependency-ready sequence

1. **DONE:** #143 qualified → #161 APPROVED → integrated at `2f476a52` / docs `bccd2b90`.
2. **RUNNING:** #124 offline from `bccd2b90`; then serialized Unity qualification, independent review and integration.
3. **RUNNING:** #151 offline from `cfdc93bf...`; browser review/prep through #162 once a candidate is pushed; Unity queues behind the serialized lane.
4. **RUNNING:** #156 offline from `cfdc93bf...`; browser review/prep through #162 once a candidate is pushed; Unity queues behind the serialized lane.
5. After accepted #143 + #124, use #162 to repin/release **#126 PARITY-08** to the actual post-#124 lineage, then run #126 and independent #128.
6. MFM-09 follows accepted/integrated MFM-08 plus accepted/integrated S1-D T-04/T-05; prepare it in browser before local implementation.
7. #74 Q10 is evidence-strengthening only and stays behind critical-path Unity work.
8. Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate; then user personal playtest. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Overnight continuation: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-CONTINUATION-20261030.md`
- #129 worker/Manager/integration handoffs: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/`
- #147 worker/Manager/integration handoffs: `Docs/Handoffs/S1-C-Implementation-06/`
- #143 execution handoff: `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/EXECUTION-HANDOFF-20261029.md`
- #143 current worker handoff: `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md` on its pushed candidate branch
- #151 release handoff: `Docs/Handoffs/S1-D-Implementation-05/RELEASE-HANDOFF-20261029.md`
- #153 MFM-08 readiness: `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
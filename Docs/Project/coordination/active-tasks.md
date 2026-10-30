# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after S1-C T-07 #187 Manager approval/closure and OPS Unity-lock repair #189 Manager approval/closure.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- Browser-capable phases should be separated from local implementation/Unity wherever practical; #162 is the browser lane.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` remains **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`**. T-07 and OPS #189 introduced no production change.
- S1-D T-05 #151 is **APPROVED / INTEGRATED / CLOSED**.
- PARITY-08 #126 is **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**.
- Rack V2 / D-064 is **VERIFIED for feature scope**. Final G4 review remains `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md`.
- S1-C T-07 #187 is **MANAGER APPROVED / CLOSED**. Accepted verification head `e64b337efd3b758fe6bc21636822ad178f0df0ca`; worker handoff `8228a26651fe01107f68e0aa6f2022d6d26fd3e9`; 304/304 checks and 8/8 mutation controls. Canonical Manager review: `Docs/Handoffs/S1-C-Implementation-07/MANAGER-REVIEW-20261030.md`. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- OPS Unity-lock repair #189 is **MANAGER APPROVED / CLOSED**. The live machine-local config repair changed only `storage.device` `/dev/sda2` -> `/dev/sdd2` for ZX20 UUID `18D6-896E`; normal `unity-lock status` returned explicit free at handoff. Canonical Manager review: `Docs/Handoffs/OPS-UNITY-LOCK-01/MANAGER-REVIEW.md`. That prior free receipt does not reserve the lane or prove it remains free later.
- MFM-08 #156 is **STATIC/OFFLINE PRE-REVIEW APPROVED; UNITY TIER-2 RELEASED**. Production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06`; docs tip `d70cec1781c104794a6f225c8609847c55df1510`. The lock-governance blocker is cleared; #156 is authorised to repeat its full fresh preflight and may post `[RUNNING — UNITY]` only after a new authoritative acquire.
- S1-C T-08 is **DEPENDENCY-CLEARED ON T-07 BUT NOT RELEASED TO UNITY YET**. Canonical packet: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`. Current preference is the first history qualification slot immediately after #156, subject to fresh candidate/ownership/lock preflight. Premature issue #191 was withdrawn before execution and grants no ownership.
- S1-D T-08 remains **RELEASED** but serialized. Because T-07 is accepted before the post-#156 slot, S1-C T-08 is currently preferred before S1-D T-08 so S1-D can reuse final history evidence.
- S1-D T-09 remains prepared and waits for S1-D T-08 PASS.
- MFM-09 remains prepared/not released; it waits for Manager-approved/integrated #156.
- MFM-10 remains prepared/not released; it waits for accepted/integrated MFM-09.
- PARITY-09 remains blocked on MFM-10 `SPECIES GUARD QUALIFIED`.
- MFM-11 remains prepared/not released; it waits for MFM-10 and must resolve the MFM-03 live/cross-year residual.
- MFM-12 remains prepared/not released; it waits for MFM-11 plus the applicable accepted integrated lineage.
- Scenario One Tier-3/player-test candidate remains browser-prepared/not released. No Windows build is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **UNITY TIER-2 RELEASED — OPS CLEAR; FRESH RE-PREFLIGHT AUTHORISED; NOT YET RUNNING** |
| **OPS-UNITY-LOCK-01** (#189) | machine-local ops | `/media/jer/ZX20/CCF-Ops` | authoritative lock governance repair | **APPROVED / CLOSED** |
| **S1-C T-07 data-contract stability** (#187) | offline verification | `verify/s1-c-t07-data-contract-v1`; `/media/jer/Videos/CCF-S1C-T07` | verification/docs only | **MANAGER APPROVED / CLOSED** |
| **S1-C T-08 final history qualification** | local Tier-2 after #156 | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; NOT RELEASED; NEXT HISTORY SLOT AFTER #156** |
| **S1-D T-08 multi-cycle witness** | local Tier-2 after preferred S1-C T-08 slot | `verify/s1-d-multicycle-witness-v1`; `/media/jer/Videos/CCF-S1D-T08` | qualification only; no production repair by default | **RELEASED; WAITING SERIALIZED UNITY** |
| **S1-D T-09 independent conformance review** | browser/read-only independent review | no production lane | review only | **PREPARED; WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | browser prep then local | prepared only; repin at release | curriculum/integration after accepted #156 | **WAITING ACCEPTED/INTEGRATED #156** |
| **MFM-10 species-parity guard** | local qualification after MFM-09 | prepared only; repin at release | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | local qualification + independent review | prepared only; repin at release | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | local integrated qualification + independent review | prepared only; repin at release | verification/review | **WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | browser repin then local Tier-3 | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight and acquire through the normal authoritative tool.

## Single-writer boundaries

### #156 MFM-08

Owns Stand Map plus the smallest MFM-specific/inspection presentation helpers. No Annual Review/history, S1-D generic Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal-to-Fell-mark or bulk-confirmation changes. Tier-2 must prove Unity compile/render, live footprint/layer/input behaviour, candidate/proposal distinction, `[C]/[N]`, real 3D spot-check, prescription/Work Plan boundary, conflicts, regressions and protected compatibility. Genuine production defect stops for separate ownership; no test weakening.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. Do not start before #156 releases the first restored Unity slot, current integration is freshly repinned, live coordination grants this slot, and the authoritative lock is reacquired normally. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. It must not take Unity while #156, S1-C T-08 or another live owner holds it. Exact released candidate remains `ce673714...` unless Manager explicitly republishes a later inspected descendant.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work.

## Dependency-ready sequence

1. **Now:** #156 repeats full fresh preflight using the restored authoritative lock tool; only a successful live acquire permits `[RUNNING — UNITY]`.
2. When #156 returns, browser/Manager reviews exact runtime/rendered evidence and integrates only if APPROVED.
3. After #156 releases Unity, fresh-repin/release S1-C T-08 on the exact current integration candidate and create/mark its substantive execution claim only at actual start.
4. After S1-C T-08 final qualification/Manager acceptance, proceed with S1-D T-08, then S1-D T-09 independent review and Manager feature verification.
5. After accepted/integrated #156, repin/release MFM-09.
6. MFM-09 accepted/integrated -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
7. MFM-10 green -> PARITY-09 and MFM-11 become eligible subject to ownership/Unity serialization.
8. MFM-11 accepted + applicable accepted Rack/parity/S1-C/S1-D lineage -> MFM-12 integrated qualification.
9. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 accepted/integrated on one exact candidate -> Scenario One Tier-3.
10. Tier-3 success -> user personally playtests the same exact candidate.
11. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Rack G4: `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md`
- S1-C T-07: `Docs/Handoffs/S1-C-Implementation-07/` including `MANAGER-REVIEW-20261030.md`
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- OPS Unity lock: `Docs/Handoffs/OPS-UNITY-LOCK-01/`
- MFM-08: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/`
- S1-D T-08: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09: `Docs/Handoffs/S1-D-Implementation-09/`
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.

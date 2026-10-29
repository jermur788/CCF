# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol / OpenAI browser lane #110 closeout.

## Coordination rules

- Overall Manager or an explicitly assigned integrator maintains this file.
- Every worker with GitHub access refreshes this file and its task claim at substantial task start and before claiming a lock is missing.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- If file/subsystem ownership overlaps, stop before editing and return to the Manager for ownership/API disposition.
- A GitHub task claim records execution presence; it **does not grant ownership** and never overrides this file.
- Heavy Unity execution is serialized machine-wide. Verify candidate HEAD, branch/worktree and ownership before acquiring the Unity lock. Never bypass another Unity owner or kill its process.
- No Windows build before the user personally playtests the Scenario One candidate and explicitly authorises it.

## Repository / integration publication state

GitHub currently publishes `integration/scenario-one-player-test-ready-v1` at **`aca74acacc565f72ab802177cf69ca6ebb54d637`**. Claim #103 reports a later local T3 merge to `22ba45849ccb6bcd535d0625fa3fe0fb42e376b6`, but that commit is not currently fetchable from GitHub and the published integration ref has not advanced. Therefore `22ba458...` is **local execution evidence, not canonical published integration truth**, until it is pushed and independently refreshable.

The same rule applies to later local child SHAs: IMPLEMENTED / REVIEWED / locally merged is not published INTEGRATED merely because an issue comment reports it.

## Machine-wide Unity queue

The most recent T3 #106 preflight reported `/media/jer/ZX20` at **21 GiB free**, above the **15 GiB Unity-lock floor**, with the Unity lock free and no Editor PIDs. This clears the earlier standing storage blocker for that observed preflight only. **Every Unity task must refresh free space, lock ownership and process state immediately before launch.**

Current #91/#106 order:

1. **#104 S1-C T-05 — v22 objective/progression history closure**: cross-provider save review/fixes, pin reconciliation, Unity save/hash/Reference/rendered gates; integrate only after exact candidate publication.
2. **#102 PARITY-07 — player-facing species truth**: reported wording fix is local-only; publish exact fix, then Unity compile + 720p/1080p rendered qualification.
3. **#93 + #100 MFM-04/05**: reported reviewed fixes/transplants are local-only; publish exact candidates, then focused Unity compile/non-mutation gates.
4. Remaining dependency-aware evidence such as #74 PARITY-03 Q10 only when #91 releases it; Q10 remains evidence strengthening, not a retroactive blocker.

Do not infer a later item owns Unity until its child claim is RUNNING and the lock is acquired normally.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01 — orchestrate Scenario One to personal-playtest-ready candidate** (#91) | **DeepSeek orchestration/integration lead**. Claude Sonnet 5.5 Medium default; Opus 5.5 Low only with recorded reason; OpenAI 6.1 Sol for suitable work/cross-provider review. | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, locks, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly only at parent level. | Preserve child ownership; no product invention; high-risk save/reference work requires independent review; no Windows build. | **RUNNING**. |
| **PLAYER-TEST-T3-CODE-LANE-01 — local C#/Unity execution** (#106) | DeepSeek-dispatched local lane using user routing and provider independence | Child task worktrees; no blanket subsystem branch | C#/verification edits, save/version work, Unity/runtime/rendered gates and integration through matching child claims | Publish exact candidates/handoffs and pushed integration refs before claiming canonical integration. | **RUNNING**. |
| **S1-C T-05 — objective/progression history with v22 save** (#104) | #106 local lane; Claude implementation + OpenAI 6.1 Sol save review | feature + bounded qualification descendant | S1-C T-05 objective-history schema/UI/verification and v22 closure only | Preserve Reference Future v1/protected hashes/old-save evidence. | **RUNNING** — reported fix `2424f31`; re-review/pin/Unity/rendered closure outstanding; not published-integrated. |
| **S1-D T-03 — teaching-footprint cap decoupling** (#103) | #106 local lane | production `f629be3`; verification reconciliation `217595f`; reported local merge `22ba458` | T-03 production + bounded RackLesson verification reconciliation | No tolerance broadening/production geometry change. | **RUNNING — PUBLICATION RECONCILIATION REQUIRED**. |
| **PARITY-07 — player-facing species truth** (#102) | #106 local lane; OpenAI review + Claude Sonnet bounded copy fix | feature branch; reported fix `478bd84` | Player-facing species truth/copy and dedicated qualification | No ecology/economy/save retune. | **RUNNING** — exact fix not yet GitHub-resolvable; Unity/rendered qualification outstanding. |
| **MFM-04 — deterministic prescription core** (#93) | #106 local lane; OpenAI 6.1 Sol independent review | reported current-lineage transplant `a8d3ccd` | Pure deterministic derivation/fit/coverage/safeguard core | No Work Plan mutation, Fell proposals, hidden quality ranking or broadleaf release. | **RUNNING** — reported `1b6ba3e` fix/review PASS and 240/240, but fix/transplant not yet GitHub-resolvable; Unity gate outstanding. |
| **MFM-05 — spacing-only Crop Tree candidate system** (#100) | #106 local lane; Claude Sonnet independent review of OpenAI implementation | reported transplant `01a2836` | Spacing-only individual inspection targets/navigation | No designation, bulk confirmation, Work Plan/save mutation or quality ranking. | **RUNNING** — original review PASS; transplant not yet GitHub-resolvable; Unity gate outstanding. |
| **PARITY-03 Q10 — mixed-density continuity evidence strengthening** (#74) | Waiting verifier when released | `verify/parity-03-density-continuity-q10-v1`; reviewed static candidate `3383e763e8ccff945dd00250366b7f334fe57331` | Verification only | Static Manager PASS; controller-level Unity evidence only if released; no production retune/rebaseline. | **WAITING**. |

## Prepared but not dispatched — no production ownership

S1-D:

- T-04 packet: `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md` — start after published T-03 + #102 teaching ownership release; mandatory OpenAI 6.1 Sol guide review.
- T-05 packet: `Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md` — start after #104 releases shared S1-C Work Plan/Annual Review/history ownership.
- T-08 packet: `Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md` — blocked until T-02..T-06 are all published-integrated.
- T-09 packet: `Docs/Handoffs/S1-D-Implementation-09/TASK-PACKET.md` — blocked until T-08 PASS; independent review only.

MFM:

- MFM-06 packet: `Docs/Handoffs/MFM-Implementation-06/TASK-PACKET.md` — start after MFM-04/05 publication+Unity qualification and #104 published integration; use actual next available save version.
- MFM-07 packet: `Docs/Handoffs/MFM-Implementation-07/TASK-PACKET.md` — after MFM-06; proposals never Fell; Work Plan remains commitment gate.
- MFM-08 packet: `Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md` — after MFM-07; individual 3D-grounded review and rendered qualification.

Packet preparation grants no ownership. Create substantive child claims only when dispatch gates are actually satisfied.

## Recently released / completed coordination relevant to current work

- **PLAYER-TEST-BROWSER-LANE-02 (#110):** completed live-state reconciliation, returned-work review, S1-D DAG audit and next-packet preparation. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-02-RESULT.md`.
- **PLAYER-TEST-BROWSER-LANE-01 (#105):** completed earlier T-05/T-03/PARITY-07/MFM-04/05 browser triage. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-RESULT.md`.
- **RACK-V2 programme through RACK-V2-14:** execution claims/handoffs report field marking, teaching, persistence, functional/rendered qualification and Rack conformance completion; no live Rack ownership retained here.
- **S1-D T-06:** independent review and Unity qualification claims closed; no live writer.
- **PARITY-05:** later parity work treats it as qualified; no live writer.
- **MFM-03 calibration:** accepted input to MFM-04/05; no live writer.
- **S1-C T-03:** current T-05 claim treats T-01/T-02/T-03 as integrated on the published `aca74ac` lineage; no separate S1-C T-03 writer retained here.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only. It grants no ownership and blocks no current feature.

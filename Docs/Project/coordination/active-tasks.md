# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. It is intentionally limited to current execution, waiting lanes with reserved ownership, and machine-wide sequencing. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol / OpenAI browser lane #110.

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

GitHub currently publishes `integration/scenario-one-player-test-ready-v1` at **`aca74acacc565f72ab802177cf69ca6ebb54d637`**. Claim #103 later reports a local T3 merge to `22ba45849ccb6bcd535d0625fa3fe0fb42e376b6`, but that commit is not currently fetchable from GitHub and the published integration ref has not advanced. Therefore `22ba458...` is **local execution evidence, not canonical GitHub integration truth**, until it is pushed and the integration ref can be independently refreshed.

This distinction applies to any later local T3 candidate SHA: IMPLEMENTED / REVIEWED / locally merged does not become published INTEGRATED merely from an issue comment.

## Machine-wide Unity queue

The most recent T3 #106 preflight reported `/media/jer/ZX20` at **21 GiB free**, above the **15 GiB Unity-lock floor**, with the Unity lock free and no Editor PIDs. That clears the earlier storage blocker for that observed preflight only. **Every Unity task must refresh free space, lock ownership and process state before launch.**

Current #91/#106 order is:

1. **#104 S1-C T-05 — v22 objective/progression history closure**: mandatory cross-provider save review/fixes, pin reconciliation, Unity save/hash/Reference/rendered gates; integrate only after all gates are green and the exact candidate is published.
2. **#102 PARITY-07 — player-facing species truth**: bounded wording fix is in local execution; Unity compile and 720p/1080p rendered qualification remain.
3. **#93 + #100 MFM-04/05 — deterministic prescription core + spacing-only candidate system**: independently reviewed/transplanted locally; focused Unity compile/non-mutation gates remain.
4. Remaining dependency-aware Unity evidence such as #74 PARITY-03 Q10 controller measurement only when #91 releases it; Q10 remains evidence strengthening and is not allowed to become a retroactive blocker for accepted parity work.

Do not infer a later queue item owns Unity until its child claim is actively RUNNING and the lock is acquired normally.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01 — orchestrate Scenario One to a personal-playtest-ready candidate** (#91) | **DeepSeek orchestration/integration lead**. Latest user routing: Claude Sonnet 5.5 Medium by default; Claude Opus 5.5 Low only with a recorded reason; OpenAI 6.1 Sol for suitable work and cross-provider review. | `orchestration/scenario-one-player-test-ready-v1`; production work stays on child branches/worktrees | Coordination/integration only at parent level: claims, locks, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly. | Preserve child ownership; no product invention; high-risk save/reference work requires independent review; no Windows build. | **RUNNING** — umbrella programme active. |
| **PLAYER-TEST-BROWSER-LANE-02 — live reconciliation, returned-work review and next packets** (#110) | Overall Manager / GPT-5.6 Sol / OpenAI browser lane | GitHub/browser only; no production worktree | Repository/GitHub inspection, source/static review, verdicts, claims/coordination, dependency reconciliation, task-packet publication | No C#/save-version/Unity/rendered/integration execution. | **RUNNING** — user explicitly requested all available browser-safe work. |
| **PLAYER-TEST-T3-CODE-LANE-01 — local C#/Unity execution** (#106) | DeepSeek-dispatched local lane using user model routing and cross-provider independence | Child task worktrees; no blanket subsystem branch | C#/verification edits, save/version work, Unity/runtime/rendered gates and local integration only through matching child claims | Must publish exact candidates/handoffs to GitHub; no blanket ownership; no Windows build. | **RUNNING** — local execution has started and has returned multiple child results. |
| **S1-C T-05 — objective/progression history with v22 save** (#104) | #106 local lane; Claude implementation with OpenAI 6.1 Sol independent save review | `feature/s1-c-t05-objective-history-v1` plus bounded qualification descendant | S1-C T-05 objective-history schema/UI/verification and v22 closure only | Preserve Reference Future v1, protected hashes and older-save evidence; no unrelated schema changes. | **RUNNING** — OpenAI review found two blockers; bounded fix `2424f31` reported with 137 offline checks; re-review/pin/Unity/rendered closure outstanding. Not published-integrated. |
| **S1-D T-03 — teaching-footprint cap decoupling** (#103) | #106 local lane | production `f629be3`; RackLesson verification reconciliation `217595f`; reported local merge `22ba458` | T-03 production + bounded verification-only RackLesson expectation reconciliation | Production unchanged by RackLesson fix; no tolerance broadening. | **RUNNING — PUBLICATION RECONCILIATION REQUIRED** — local Unity gates reported green and a local integration merge is claimed, but GitHub integration still resolves to `aca74ac`; do not call it published-integrated yet. |
| **PARITY-07 — player-facing species truth** (#102) | #106 local lane; OpenAI 6.1 Sol review + Claude Sonnet bounded copy fix | `feature/parity-07-player-facing-species-truth-v1`; reported wording fix `478bd84` | Player-facing species truth/copy and dedicated qualification only | No ecology/economy/save retune. Must keep species crown-geometry representation and market gating honest. | **RUNNING** — wording fix reported 83/83 + timber 73; Unity compile/rendered qualification outstanding. |
| **MFM-04 — deterministic prescription core** (#93) | #106 local lane; OpenAI 6.1 Sol independent review | bounded current-lineage transplant reported at `a8d3ccd` | Pure deterministic derivation/fit/coverage/safeguard core | No Work Plan mutation, Fell proposals, hidden quality ranking or broadleaf release. | **RUNNING** — fail-closed safeguard fix `1b6ba3e` independently re-reviewed PASS; transplant reports 240/240; Unity compile/non-mutation gate outstanding. |
| **MFM-05 — spacing-only Crop Tree candidate system** (#100) | #106 local lane; Claude Sonnet 5.5 Medium independent review of OpenAI implementation | reported transplant on reviewed MFM-04 descendant at `01a2836` | Spacing-only individual inspection targets/navigation | No designation, bulk confirmation, Work Plan/save mutation or quality ranking. MFM-06/07/08 remain separate. | **RUNNING** — independent review PASS and transplant reported; Unity compile/non-mutation gate outstanding. |
| **PARITY-03 Q10 — mixed-density continuity evidence strengthening** (#74) | Waiting verifier when released | `verify/parity-03-density-continuity-q10-v1`; reviewed static candidate `3383e763e8ccff945dd00250366b7f334fe57331` | Verification only | Manager static PASS; controller-level Unity measurement remains optional evidence strengthening under #91 sequencing; no production retune/rebaseline. | **WAITING** — no active writer/Unity ownership. |

## Recently released / completed coordination relevant to current work

- **PLAYER-TEST-BROWSER-LANE-01 (#105):** completed browser inspection/triage of T-05, T-03, PARITY-07 and MFM-04/05; durable closeout `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-RESULT.md`.
- **RACK-V2 programme through RACK-V2-14:** execution claims/handoffs report field marking, teaching, persistence, functional and rendered qualification, and Rack integration/spec-conformance completion. Closed claims no longer hold live ownership.
- **S1-D T-06:** independent review and Unity qualification claims are closed; no live ownership remains here.
- **PARITY-05 focused qualification/harness reconciliation:** later parity work treats PARITY-05 as qualified; no live ownership remains here.
- **MFM-03 calibration:** accepted calibration evidence is an input to MFM-04/05; no live writer remains on MFM-03.
- **S1-C T-03:** the current T-05 claim treats T-01/T-02/T-03 as integrated on the `aca74ac` lineage; no separate T-03 writer is retained here.

## Near-term authorised work not yet holding production ownership

- **S1-D:** T-04 guide monotonicity; T-05 previous-versus-proposed presentation; T-08 three-cycle witness after T-02..T-06; T-09 independent spec-conformance review after T-08.
- **MFM:** MFM-06 save/provenance foundation; MFM-07 proposal/approval boundary; MFM-08 map/3D review UX; then MFM-09..12 in approved dependency order.
- These items receive substantive child claims when actually dispatched. Packet preparation does not grant production ownership.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only. It grants no ownership and blocks no current feature.

## Claim hygiene note

The Manager reconciles claim titles to `[RUNNING]` when a handed-off/static claim is actively resumed by #106. That status reflects execution presence only and does not broaden subsystem ownership beyond the row above.

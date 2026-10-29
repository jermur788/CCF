# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. It is intentionally limited to current execution, waiting lanes with reserved ownership, and machine-wide sequencing. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol / OpenAI browser lane.

## Coordination rules

- Overall Manager or an explicitly assigned integrator maintains this file.
- Every worker with GitHub access refreshes this file and its task claim at substantial task start and before claiming a lock is missing.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- If file/subsystem ownership overlaps, stop before editing and return to the Manager for ownership/API disposition.
- A GitHub task claim records execution presence; it **does not grant ownership** and never overrides this file.
- Heavy Unity execution is serialized machine-wide. Verify candidate HEAD, branch/worktree and ownership before acquiring the Unity lock. Never bypass another Unity owner or kill its process.
- No Windows build before the user personally playtests the Scenario One candidate and explicitly authorises it.

## Machine-wide Unity queue

**Current priority:** RACK-V2-08 / claim #49 is the next authorised Rack critical-path task. It is currently **DISPATCHED**, not proven RUNNING; no Unity lock is implied until its worker refreshes this file/claim, records actual execution identity and acquires the lock normally.

Waiting Unity work (#19 S1-C T-03 rendered qualification, #70 PARITY-05 Unity qualification, the controller-level phase of #74 PARITY-03 Q10, #39 MFM-03 measurement where Unity is required, and the eventual Unity qualification following #85 S1-D T-06 independent review) must not bypass #49 unless the Overall Manager explicitly reroutes the queue.

The RACK-V2-08 Combined HUD layout repair (#59) is complete and integrated through PR #80 / merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1`; its Unity lock is released. That completed repair is not a current owner.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **RACK-V2-08 — remove competing first-person rack planning and make Stand Map authoritative** (claim #49) | Existing T3 Code / OpenAI Codex lane; DeepSeek watcher where already assigned | `feature/scenario-one-rack-v2-no-competing-author-v1`; `/media/jer/ZX20/CCF-RackV2-08` | Bounded RACK-V2-08 production/UI and its Guided/Combined verification per packet/G3 disposition | Must bring repaired current `main` containing PR #80 into the lane. Preserve Legacy40/model-0 first-person compatibility and ordinary Fell/Crop interaction. Full model-0 + V2 Guided/Sequential/Combined HUD qualification required. Seven prior real-input failures are not waived. Machine Unity lock before Unity. | **DISPATCHED / READY TO RESUME** — Manager continuation `Docs/Handoffs/RACK-V2-08/RESUME-AFTER-LAYOUT-REPAIR.md`; not RUNNING until worker posts execution identity. |
| **S1-D T-06 Independent Review — verify management-event ID save hardening and repair stale U4 synthetic contract** (claim #85) | Target: OpenAI Codex / strongest available Sol-class review model; must be independent of Claude Sonnet 5.5 / Anthropic implementer | suggested `review/s1-d-management-event-id-integrity-v1`; dedicated clean review worktree based on `55787c8` | Production is read-only for review; bounded verification-only U4 repair; review handoff | Manager static disposition accepts production for independent review only. U4 duplicate-ID synthetic fixture is not a compatibility STOP; it must be changed to expect pre-mutation load refusal without weakening N3/rules-level duplicate chronology coverage. No production edit during review. Unity waits behind #49. | **DISPATCHED** — packet `Docs/Handoffs/S1-D-Implementation-06-Review/TASK-PACKET.md`; implementation claim #83 handed off/closed. |
| **PARITY-03 Q10 — strengthen mixed-density continuity evidence** (claim #74) | Target: OpenAI Codex / strong Sol-class local verification model; Claude Sonnet-class acceptable | suggested `verify/parity-03-density-continuity-q10-v1`; dedicated verification worktree to be reported at start | `Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs`; bounded `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` and narrow runners/reports only | No production/density retune/anchor rebaseline. Static/offline harness work may run in parallel with Rack; controller-level Unity measurement waits behind #49 unless Manager reroutes. Existing PARITY-03 qualification stays approved. | **DISPATCHED — STATIC FIRST** — packet `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/TASK-PACKET.md`; not RUNNING until worker records execution identity. |
| **PROJECT CONTEXT REFRESH — publish reconciled Scenario One authority snapshot** (claim #84) | Target: SOL/CODEX infrastructure lane; bounded OpenCode acceptable | suggested `ops/context-refresh-20261029-reconciled-state`; dedicated clean worktree | `Tools/ProjectContext` generation execution and generated outputs; permanent Drive mirror publication; platform attachment bundle; context handoff | Publication only. `active-tasks.md` remains live/excluded. No gameplay/Unity/save/schema/mechanic change; no manual edits to generated composites/dashboard state; no account/privacy/billing changes. | **DISPATCHED** — packet `Docs/Handoffs/ContextRefresh-20261029-ReconciledState/TASK-PACKET.md`; old 2026-10-27 packet explicitly superseded. |
| **S1-C T-03 — intervention drill-down and money honesty, rendered Tier-2 qualification** (claim #19) | Existing T3 Code / Claude Code branch retained; no active writer while waiting | `feature/s1-c-history-drilldown-v1`; `/media/jer/ZX20/CCF-S1C-Impl03` | `UI/AnnualReviewView.cs`; additive T-03 `ScenarioManagementHistory.cs`; T-03 verification only when released | Static candidate `859f74e` Manager-accepted for qualification, but not integrated/Unity-verified/accepted. Reuse same claim/branch/worktree. Wait behind #49 unless Manager reroutes. | **WAITING** — rendered continuation packet published; no Unity ownership. |
| **PARITY-05 — early-life growth and browsing focused Unity qualification** (claim #70) | Unassigned local Unity-capable qualifier until released | verification lane to be selected from current main | PARITY-05 verification execution only | PR #69 already integrated. No production change/retune/rebaseline. Protected hashes must not move. Wait behind #49 unless Manager reroutes. | **WAITING** — integrated implementation/docs patch; Unity qualification outstanding. |
| **MFM-03 — measure and calibrate representative-management coverage** (claim #39) | SOL/CODEX strongest local reasoning/measurement lane when released | recommended `research/mfm-coverage-calibration-v1`; dedicated clean worktree | Measurement tooling/analysis for rack-served unit variation and candidate-spacing workload | Measurement/calibration only; no prescription/proposal/save/Work Plan/UI implementation. Any Unity use waits behind #49 unless Manager reroutes. | **WAITING** — valid authorised task, not currently executing. |

## Recently released / completed coordination relevant to current work

- **S1-D T-06 implementation — management-event ID save hardening:** static implementation handed off at production `8e05b5a` / published tip `55787c8`; Manager disposition accepts it for independent review only. Claim #83 is closed; claim #85 owns the review. Unity qualification remains outstanding.
- **Project-status reconciliation — current milestone/live queue cleanup:** completed by Overall Manager browser lane; handoff `Docs/Handoffs/Project-Status-Reconciliation-20261028/HANDOFF.md`; no live ownership remains.
- **RACK-V2-08 Layout Repair — fix Stand Map/minimap Combined HUD regressions:** Manager-approved and integrated through PR #80 / `b118d8e63f60d54c9f9a3bed6de40af83b35efc1`; claim #59 closed; Unity lock released.
- **RACK-V2-07 — confirm/reopen Rack Plan and minimap presentation:** qualified, Manager-approved and integrated through PR #48 / `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9`; no live ownership.
- **PARITY-03 Q11 — repair stale mixed-density verification records and evidence mapping:** completed by browser lane; claim #81 closed; only Q10 remains under #74.
- **PARITY-05 static patch — align early-life/browse evidence and strengthened assertions:** Manager-reviewed and integrated through PR #69; no live production ownership; #70 tracks remaining Unity qualification.
- **S1-D T-02 — advisory cycle/readiness state:** Manager-accepted bounded slice and integrated through PR #76 / `9fcb5ee7065950584bf98c88ca5ce95a4a455ca8`; no live writer. Real Unity import/compile observation remains for a later applicable S1-C/S1-D gate.
- **ARM-010 — T3 Adaptive contracts/persistence skeleton:** cross-repo implementation claim #63 is closed COMPLETE and remains separate from CCF gameplay/Unity ownership.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, is backlog only. It grants no ownership and blocks no current feature.

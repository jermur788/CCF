# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. It is intentionally limited to current execution, waiting lanes with reserved ownership, and machine-wide sequencing. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-28 by Overall Manager / GPT-5.6 Sol / OpenAI browser lane.

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

Waiting Unity work (#19 S1-C T-03 rendered qualification, #70 PARITY-05 Unity qualification, #74 PARITY-03 Q10 controller-level continuity measurement, and #39 MFM-03 measurement where Unity is required) must not bypass #49 unless the Overall Manager explicitly reroutes the queue.

The RACK-V2-08 Combined HUD layout repair (#59) is complete and integrated through PR #80 / merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1`; its Unity lock is released. That completed repair is not a current owner.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PROJECT-STATUS-RECON-20261028 — reconcile milestone, live coordination and queue wording** (claim #82) | Overall Manager / GPT-5.6 Sol / OpenAI browser lane | `main`; GitHub/browser only | `Docs/Project/current-milestone.md`; this coordination file; targeted issue metadata; reconciliation handoff | Docs/metadata only. No production, Unity, verification-harness implementation, save/schema, Rack/MFM mechanics or dashboard-state edits. | **RUNNING** — current milestone and open-claim queue wording being reconciled; this row clears when handoff is published. |
| **RACK-V2-08 — remove competing first-person rack planning and make Stand Map authoritative** (claim #49) | Existing T3 Code / OpenAI Codex lane; DeepSeek watcher where already assigned | `feature/scenario-one-rack-v2-no-competing-author-v1`; `/media/jer/ZX20/CCF-RackV2-08` | Bounded RACK-V2-08 production/UI and its Guided/Combined verification per packet/G3 disposition | Must bring repaired current `main` containing PR #80 into the lane. Preserve Legacy40/model-0 first-person compatibility and ordinary Fell/Crop interaction. Full model-0 + V2 Guided/Sequential/Combined HUD qualification required. Seven prior real-input failures are not waived. Machine Unity lock before Unity. | **DISPATCHED / READY TO RESUME** — Manager continuation `Docs/Handoffs/RACK-V2-08/RESUME-AFTER-LAYOUT-REPAIR.md`; not RUNNING until worker posts execution identity. |
| **S1-C T-03 — intervention drill-down and money honesty, rendered Tier-2 qualification** (claim #19) | Existing T3 Code / Claude Code branch retained; no active writer while waiting | `feature/s1-c-history-drilldown-v1`; `/media/jer/ZX20/CCF-S1C-Impl03` | `UI/AnnualReviewView.cs`; additive T-03 `ScenarioManagementHistory.cs`; T-03 verification only when released | Static candidate `859f74e` Manager-accepted for qualification, but not integrated/Unity-verified/accepted. Reuse same claim/branch/worktree. Wait behind #49 unless Manager reroutes. | **WAITING** — rendered continuation packet published; no Unity ownership. |
| **PARITY-05 — early-life growth and browsing focused Unity qualification** (claim #70) | Unassigned local Unity-capable qualifier until released | verification lane to be selected from current main | PARITY-05 verification execution only | PR #69 already integrated. No production change/retune/rebaseline. Protected hashes must not move. Wait behind #49 unless Manager reroutes. | **WAITING** — integrated implementation/docs patch; Unity qualification outstanding. |
| **PARITY-03 Q10 — strengthen mixed-density continuity evidence** (claim #74) | Unassigned verification-only lane | branch/worktree to be selected; no current writer | Continuity verification harness and evidence only | Q11 docs repair is complete via claim #81. No density arithmetic/retune/anchor rebaseline. Static harness preparation may proceed only with explicit ownership; controller-level measurement requiring Unity waits behind #49. Existing PARITY-03 qualification remains approved. | **WAITING** — evidence-strengthening follow-up only. |
| **MFM-03 — measure and calibrate representative-management coverage** (claim #39) | SOL/CODEX strongest local reasoning/measurement lane when released | recommended `research/mfm-coverage-calibration-v1`; dedicated clean worktree | Measurement tooling/analysis for rack-served unit variation and candidate-spacing workload | Measurement/calibration only; no prescription/proposal/save/Work Plan/UI implementation. Any Unity use waits behind #49 unless Manager reroutes. | **WAITING** — valid authorised task, not currently executing. |

## Recently released / completed coordination relevant to current work

- **RACK-V2-08 Layout Repair — fix Stand Map/minimap Combined HUD regressions:** Manager-approved and integrated through PR #80 / `b118d8e63f60d54c9f9a3bed6de40af83b35efc1`; claim #59 closed; Unity lock released.
- **RACK-V2-07 — confirm/reopen Rack Plan and minimap presentation:** qualified, Manager-approved and integrated through PR #48 / `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9`; no live ownership.
- **PARITY-03 Q11 — repair stale mixed-density verification records and evidence mapping:** completed by browser lane; claim #81 closed; only Q10 remains under #74.
- **PARITY-05 static patch — align early-life/browse evidence and strengthened assertions:** Manager-reviewed and integrated through PR #69; no live production ownership; #70 tracks remaining Unity qualification.
- **S1-D T-02 — advisory cycle/readiness state:** Manager-accepted bounded slice and integrated through PR #76 / `9fcb5ee7065950584bf98c88ca5ce95a4a455ca8`; no live writer. Real Unity import/compile observation remains for a later applicable S1-C/S1-D gate.
- **ARM-010 — T3 Adaptive contracts/persistence skeleton:** cross-repo implementation claim #63 is closed COMPLETE and remains separate from CCF gameplay/Unity ownership.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, is backlog only. It grants no ownership and blocks no current feature.

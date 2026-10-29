# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol after MFM-06 #113 integration and browser #120 readiness approval.

## Coordination rules

- Overall Manager or an explicitly assigned integrator maintains this file.
- Every worker with GitHub access refreshes this file and its task claim at substantial task start and before claiming a lock is missing.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- If file/subsystem ownership overlaps, stop before editing and return to the Manager for ownership/API disposition.
- A GitHub task claim records execution presence; it **does not grant ownership** and never overrides this file.
- Heavy Unity execution is serialized machine-wide. Verify candidate HEAD, branch/worktree and ownership before acquiring the Unity lock. Never bypass another Unity owner or kill its process.
- No Windows build before the user personally playtests the Scenario One candidate and explicitly authorises it.

## Current integration truth

MFM-06 #113 is **HANDED-OFF / CLOSED — INTEGRATED / VERIFIED for its bounded gate**.

- Pre-integration umbrella: `ac876a64969752a3864034521f42559671264b30`.
- Exact reviewed MFM-06 input: `13fd85452b8d3a55946fcb459af8b00e0326a579`.
- Exact merged and Unity-qualified production umbrella: **`b971f4da0540e606bfee9702b0c99f81a0a89355`**.
- Published `integration/scenario-one-player-test-ready-v1` head: **`7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`**, a documentation-only successor of the tested production merge.
- Integration handoff: `Docs/Handoffs/MFM-06/INTEGRATION-HANDOFF.md`.
- Save schema is v23. Frozen Reference and protected hash compatibility passed without rebaseline.

Browser #120 reviewed #113 and returned **APPROVED — MFM-07 READY FOR CODE EXECUTION**. #120 is closed/released.

## Immediate execution order

1. **#121 MFM-07 — proposal state and approval boundary** is **DISPATCHED / RELEASED**, not yet assumed RUNNING. Production work must root from exact qualified candidate `b971f4da0540e606bfee9702b0c99f81a0a89355` unless a later Manager-approved docs-only descendant is explicitly selected.
2. Before edits, #121 refreshes this file, #91/#106/#113/#120/#121, worktrees, dirty state and Unity lock; then marks itself `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD.
3. **Claude Code is authorised. Preferred route: Claude Sonnet 5.5 Ultra Code** for this 5/5 task, retaining one primary production writer. Escalate to Opus 5.5 Low only if architecture/conflict reasoning requires it.
4. No downstream task gains production ownership merely because a packet exists.

There is no standing assertion here that the Unity lock is free. The active worker must refresh lock/process/free-space state immediately before Unity.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | Child worktrees only | Local implementation/qualification through child claims | Exact candidate/handoff required; Unity serialized | **RUNNING** |
| **MFM-07 proposal/approval boundary** (#121) | Claude Code; Sonnet 5.5 Ultra Code preferred | `feature/mfm-v1-proposal-approval-boundary-v1`; suggested `/media/jer/Videos/CCF-MFM-07` | Smallest proposal → validated pending-work surface required by MFM-07 | Root from `b971f4d`; no schema bump/save-validator edit; no MFM-08/09; no broadleaf quantitative extrapolation | **DISPATCHED / RELEASED — mark RUNNING at actual start** |
| **PARITY-03 Q10 evidence strengthening** (#74) | Waiting verifier when released | existing verification lane | Verification only | Non-blocking evidence strengthening | **WAITING** |

## MFM-07 single-writer boundary

Claim #121 and `Docs/Handoffs/MFM-07/TASK-PACKET.md` are the current execution authority.

When #121 starts, its writer owns only the smallest proposal→pending-work integration surface required by the published MFM-06 API:

- proposals remain separate from `TreeMarkType.Fell` and ordinary manual marks;
- prescription approval creates pending ordinary work only;
- Work Plan approval remains a separate commitment gate;
- manual player state wins conflicts and conflicts are surfaced rather than overwritten;
- **before each prescription-generated order is written, its referenced prescription, coverage unit and Crop Tree must validate; mismatch/staleness fails closed with no partial order/world mutation**;
- MFM-06 v23 IDs/provenance are consumed, not duplicated;
- no schema/version bump or save-validator edit is authorised; if required, STOP and return to Manager;
- no broadleaf quantitative extrapolation.

Likely hot/shared surfaces include `ScenarioOneManager`, ordinary work-order creation APIs and Work Plan integration hooks. Exact files must be recorded before edits. Any overlap with another active writer means stop before editing.

## Downstream sequencing — prepared only, no ownership yet

### Save / validation

- **PARITY-08** may start only under a separate live claim after MFM-06 integration. It owns save validation/determinism while active. Do not overlap it with any unexpected MFM-07 save-path edit.
- Final Tier-3 reruns the combined save/history chain on one exact final candidate.

### S1-C review/history

- **S1-C T-04**: provenance labelling on Annual Review/history.
- **S1-C T-06**: prior-treatment guidance/history navigation.
- **S1-C T-07**: stable identity/history contract verification.
- **S1-C T-08**: feature qualification after T-04/T-06/T-07.
- These may run beside MFM-07 only with explicitly non-overlapping live files. Shared `ScenarioOneManager`, save-validation, work-order or history-helper edits force serialization/Manager review.

### S1-D / teaching / Work Plan

- **S1-D T-04** owns teaching-sequence/current-state files and must land before MFM-09.
- **S1-D T-05** waits until the MFM-07 pending-work/provenance API is stable; it then owns the bounded previous-versus-proposed Work Plan presentation seam.
- **S1-D T-08** waits for T-02..T-06 acceptance.
- **S1-D T-09** waits for T-08 PASS and is independent review only.

### MFM continuation

- **MFM-08** follows accepted/integrated MFM-07 and owns bounded map/3D candidate/proposal review UX after overlapping writers are released.
- **MFM-09** follows MFM-07/08 plus S1-D T-04/T-05.
- **MFM-10** follows MFM-09 + PARITY-08 and retains the Sitka-only species guard.
- **MFM-11** follows stable parity/repeated-intervention/player-facing flow.
- **MFM-12** follows MFM-11 plus parity/S1-C/S1-D qualification and provides integrated MFM verification/review.

### Three-species parity

- **PARITY-08**: save/deterministic compatibility after MFM-06, subject to single-writer serialization above.
- **PARITY-09**: integrated parity qualification after PARITY-08 + MFM-10.

### Programme endgame

- Final Tier-3: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md` — one exact final candidate; mandatory fresh S1-C T-05 save/history rerun.
- Final independent review: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW-TASK-PACKET.md`.
- User-playtest template: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF-TEMPLATE.md` — template only until final qualification passes.

## Released/completed claims relevant to current state

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #112 superseded review; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.

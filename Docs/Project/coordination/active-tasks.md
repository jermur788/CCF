# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol under browser lane #122. MFM-07 #121 remains published as DISPATCHED/RELEASED until its local executor records actual start. S1-C T-04 #123 and S1-D T-04 #124 are also DISPATCHED/RELEASED; neither owns production files until its executor marks RUNNING and live ownership is refreshed.

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

1. **#121 MFM-07 — proposal state and approval boundary** is **DISPATCHED / RELEASED**, not yet assumed RUNNING. Root from exact qualified `b971f4d...` unless Manager explicitly approves another production base.
2. **#123 S1-C T-04 — provenance labelling** is **DISPATCHED / RELEASED** on the same exact base. It is file-separable from intended MFM-07 and may start when its worker records live ownership.
3. **#124 S1-D T-04 — guide monotonicity** is **DISPATCHED / RELEASED** on the same exact base. It is file-separable from intended MFM-07 and must land before MFM-09.
4. Each worker refreshes this file and its claim immediately before edits, verifies exact branch/worktree/HEAD and conflicts, then marks its claim `[RUNNING]` with actual model/provider/lane. The live coordination row is updated at that moment.
5. Unity qualification remains serialized even when code-file ownership is disjoint.

There is no standing assertion here that the Unity lock is free. The active worker must refresh lock/process/free-space state immediately before Unity.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | Child worktrees only | Local implementation/qualification through child claims | Exact candidate/handoff required; Unity serialized | **RUNNING** |
| **PLAYER-TEST-BROWSER-LANE-05** (#122) | Overall Manager / OpenAI GPT-5.6 Sol browser lane | GitHub/browser only; no production worktree | Readiness analysis, packet/claim publication, MFM-07 return review, coordination updates | No production C#/Unity/save execution/integration/rendered qualification | **RUNNING** |
| **MFM-07 proposal/approval boundary** (#121) | Claude Code; Sonnet 5.5 Ultra Code preferred | `feature/mfm-v1-proposal-approval-boundary-v1`; suggested `/media/jer/Videos/CCF-MFM-07` | Smallest proposal → validated pending-work surface required by MFM-07, only after actual start is registered | Root `b971f4d`; no schema/save-validator edit; no MFM-08/09; no broadleaf extrapolation | **DISPATCHED / RELEASED — no production ownership until RUNNING** |
| **S1-C T-04 provenance labelling** (#123) | Claude Sonnet 5.5 Medium or stronger Code lane | `feature/s1-c-t04-provenance-labels-v1`; suggested `/media/jer/Videos/CCF-S1C-T04` | `AnnualReviewView.cs`, `ManagementHistoryText.cs`, minimum history-label helper only after actual start is registered | Root `b971f4d`; no save/schema/validator, MFM proposal/work-order or simulation edits | **DISPATCHED / RELEASED — no production ownership until RUNNING** |
| **S1-D T-04 guide monotonicity** (#124) | Claude Sonnet 5.5 Ultra Code preferred | `feature/s1-d-t04-guide-monotonicity-v1`; suggested `/media/jer/Videos/CCF-S1D-T04` | `ScenarioOneTeachingSequence.cs`, `LearningObjectivesView.Sequence.cs`, minimum pure helper only after actual start is registered | Root `b971f4d`; no schema/validator/MFM; must land before MFM-09 | **DISPATCHED / RELEASED — no production ownership until RUNNING** |
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

## Parallel-safe boundaries for #123 / #124

### #123 S1-C T-04

- History/review presentation labels only.
- Must not edit save/load/validation, `ScenarioOneManager` work-order creation, MFM proposal/approval logic or prescription persistence.
- If a required provenance label needs one of those surfaces, STOP and return to Manager.

### #124 S1-D T-04

- Teaching/current-state monotonicity only.
- Must not edit save/load/validation, completion rules, Rack lifecycle or MFM logic.
- If MFM-07 unexpectedly touches the teaching files, STOP and return to Manager rather than reconciling independently.

## Post-MFM-07 sequencing — prepared only, no ownership yet

### Save / validation

- **PARITY-08** packet is prepared at `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/TASK-PACKET.md` but remains **WAITING** for the actual MFM-07 return review.
- If #121's diff confirms no unauthorised save/validator/hash/reference changes, create/release the PARITY-08 child claim immediately on the Manager-approved post-MFM-07 base.
- While PARITY-08 runs it is the sole save-validation/determinism writer.
- Final Tier-3 reruns the combined save/history chain on one exact final candidate.

### S1-C review/history

- **T-04 #123** is released now.
- **T-06** follows accepted T-04 on the same S1-C history/Annual Review seam; serialize overlapping files.
- **T-07** stable identity/history contract verification must not create a second identity/history store.
- **T-08** feature qualification follows T-04/T-06/T-07.

### S1-D / teaching / Work Plan

- **T-04 #124** is released now and must land before MFM-09.
- **T-05** packet is prepared but waits for the accepted MFM-07 pending-work/provenance API; it then owns the bounded Work Plan previous-vs-proposed grouping.
- **T-08** waits for T-02..T-06 acceptance.
- **T-09** waits for T-08 PASS and is independent review only.

### MFM continuation

- **MFM-08** follows accepted/integrated MFM-07 and owns bounded map/3D candidate/proposal review UX.
- **MFM-09** follows MFM-07/08 plus accepted S1-D T-04/T-05 and consumes their existing teaching/Work Plan seams.
- **MFM-10** follows MFM-09 + PARITY-08 and retains the Sitka-only species guard.
- **MFM-11** follows stable parity/repeated-intervention/player-facing flow.
- **MFM-12** follows MFM-11 plus parity/S1-C/S1-D qualification.

### Three-species parity

- **PARITY-08**: waiting only for MFM-07 return/ownership reconciliation.
- **PARITY-09**: after PARITY-08 + MFM-10.

### Programme endgame

- Final Tier-3: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md` — one exact final candidate; mandatory fresh S1-C T-05 save/history rerun.
- Final independent review: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW-TASK-PACKET.md`.
- User-playtest template: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF-TEMPLATE.md` — template only until final qualification passes.

## Released/completed claims relevant to current state

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #112 superseded review; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.

# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol / OpenAI browser lane #120.

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

GitHub `main` was refreshed by browser lane #120 at **`3864c0634e59d5fb1f392dc651114d1d06a89849`** before this coordination update.

GitHub currently publishes `integration/scenario-one-player-test-ready-v1` at **`aca74acacc565f72ab802177cf69ca6ebb54d637`**. Parent #91 / T3 #106 previously reported later umbrella candidate **`ac876a64969752a3864034521f42559671264b30`** as clean, local and unpushed. Treat that SHA and descendants as local execution evidence only until #113 publishes a GitHub-resolvable post-MFM-06 integration ref.

MFM-06 implementation/qualification claim #111 is **HANDED-OFF / CLOSED**. Exact reviewed production candidate: **`13fd85452b8d3a55946fcb459af8b00e0326a579`** on `feature/mfm-v1-save-provenance-v23`. Independent Claude Opus 5.5 Low round-2 review is accepted by the Manager for this integration gate after two defects were repaired. Extra cross-provider claim #112 is **SUPERSEDED / CLOSED**.

No browser-side statement may convert locally qualified MFM-06 into canonical integration. That requires #113's exact pre/post SHAs plus post-merge gates and a pushed integration ref.

## Machine-wide Unity queue and immediate dependency order

There is no standing browser-side assertion that the Unity lock is free. Every T3 task must refresh free space, lock ownership and process state immediately before launch.

Current order:

1. **#113 MFM-06 integration/post-merge qualification** is DISPATCHED. It has not yet marked itself RUNNING in GitHub at this reconciliation, so no browser claim is made that it currently owns Unity or the integration worktree. Once it starts normally, it owns only bounded MFM-06 integration/post-merge verification until handoff.
2. **#120 browser lane** is RUNNING now with **no production ownership**. It audits MFM-07 readiness, dependencies and single-writer boundaries and will review #113's returned handoff.
3. **#121 MFM-07 proposal state / approval boundary** is PREPARED and BLOCKED until #113 returns `MFM-06 V23 INTEGRATED — READY FOR MFM-07` and #120 issues an explicit APPROVED readiness verdict with the exact post-#113 umbrella base.
4. Downstream implementation must follow the single-writer order in `Docs/Handoffs/PlayerTestReadiness-Overnight-01/CROSS-FEATURE-OWNERSHIP-AND-SEQUENCING.md`, as refined below.

Do not infer a later item owns Unity until its child claim is RUNNING and the lock is acquired normally.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01 — orchestrate Scenario One to personal-playtest-ready candidate** (#91) | **DeepSeek orchestration/integration lead** | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, locks, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly only at parent level | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01 — local C#/Unity execution** (#106) | DeepSeek-dispatched local lane | Child task worktrees; no blanket subsystem branch | C#/verification edits, Unity/runtime/rendered gates and integration only through matching child claims | Publish exact candidates/handoffs and pushed integration refs before claiming canonical integration | **RUNNING** |
| **PLAYER-TEST-BROWSER-LANE-04 — MFM-07 readiness / #113 review** (#120) | OpenAI GPT-5.6 Sol / browser Manager | GitHub/browser only | Coordination, spec/packet review, handoff verdicts, task/claim hygiene | **No C#/Unity/save/integration production ownership.** Review #113 when returned and pin exact MFM-07 base | **RUNNING** |
| **MFM-06 integration gate** (#113) | ChatGPT Code / OpenAI 6.1 Sol preferred by packet | Current umbrella integration lineage, refreshed at actual start | Bounded MFM-06 integration and post-merge qualification only after claim start | Must integrate exact reviewed `13fd854...`; preserve reviewed repairs; publish exact pre/post SHAs and gate evidence | **DISPATCHED — NOT YET RUNNING AT LAST REFRESH** |
| **PARITY-03 Q10 — mixed-density continuity evidence strengthening** (#74) | Waiting verifier when released | `verify/parity-03-density-continuity-q10-v1`; reviewed static candidate `3383e763e8ccff945dd00250366b7f334fe57331` | Verification only | Non-blocking evidence strengthening | **WAITING** |

## MFM-07 single-writer boundary after #113

Claim #121 and `Docs/Handoffs/MFM-07/TASK-PACKET.md` are the current MFM-07 execution authority once released. The older `Docs/Handoffs/MFM-Implementation-07/TASK-PACKET.md` is historical/preparation material and must not be dispatched instead of #121.

When #120 releases #121, the MFM-07 writer owns the smallest proposal→pending-work integration surface required by the published MFM-06 API. In particular:

- proposals remain separate from `TreeMarkType.Fell` and ordinary manual marks;
- prescription approval creates pending ordinary work only; Work Plan approval remains a separate commitment gate;
- manual player state wins conflicts and conflicts are surfaced rather than overwritten;
- **before each prescription-generated order is written, its prescription, coverage unit and Crop Tree linkage must be validated against the referenced prescription; mismatch/staleness fails closed with no partial order/world mutation**;
- MFM-06 v23 IDs/provenance are consumed, not duplicated;
- no schema/version bump or save-validation edit is permitted unless the task stops and returns to Manager ownership review;
- no broadleaf quantitative extrapolation.

Likely hot/shared surfaces include `ScenarioOneManager` / ordinary work-order creation APIs and Work Plan integration hooks. Exact files must be recorded from the post-#113 API before edits. If MFM-07 discovers it needs a file actively owned by another lane, it stops before editing.

## Downstream single-writer reconciliation

These tasks remain packet-prepared and have **no production ownership** until a live claim is created and recorded here.

### Save / validation

- **PARITY-08** starts only after accepted MFM-06 integration. It owns save validation/determinism compatibility while active and must not overlap a MFM-07 edit to save classes/validation. MFM-07 is expected to consume v23 without a schema bump; if it needs save-path edits, serialize with PARITY-08 rather than running both writers.
- Final Tier-3 reruns the combined save/history chain on one exact final candidate.

### S1-C review/history

- **S1-C T-04** owns provenance labelling on the Annual Review/history presentation surface after the post-MFM-06 base is known.
- **S1-C T-06** follows/partitions with T-04 and owns bounded prior-treatment guidance/history navigation.
- **S1-C T-07** verifies the stable identity/history contract and must not create a second identity/history store or edit MFM provenance semantics in parallel with MFM-07.
- S1-C work may proceed beside MFM-07 only when the live claims record non-overlapping files. Any need for shared `ScenarioOneManager`, save-validation, work-order, or shared history-helper edits forces serialization/Manager review.

### S1-D / teaching / Work Plan

- **S1-D T-04** owns `ScenarioOneTeachingSequence.cs`, `LearningObjectivesView.Sequence.cs` and only the minimum history read seam; it must land before MFM-09 and must not expand into MFM logic.
- **S1-D T-05** must start **after MFM-07's pending-work/provenance API is stable** and before MFM-09. Its expected primary production file is `Assets/ForestPrototype/UI/WorkPlanView.cs`; Annual Review/history internals remain S1-C-owned unless explicitly repartitioned.
- **MFM-09** then consumes the stable MFM-07 pending-work API plus S1-D T-04/T-05 presentation/teaching seams. It must not invent parallel grouping/history semantics.

### MFM continuation

- **MFM-08** follows accepted/integrated MFM-07 and owns the bounded map/3D candidate/proposal review UX after overlapping Stand Map/teaching UI writers are released.
- **MFM-09** follows MFM-07/08 plus S1-D T-04/T-05.
- **MFM-10** follows MFM-09 + PARITY-08 and retains the Sitka-only species guard.
- **PARITY-09** follows PARITY-08 + MFM-10.
- **MFM-11** follows stable repeated-intervention/player-facing flow; **MFM-12** follows MFM-11 plus parity/S1-C/S1-D qualification.

## Released execution claims with publication caveat

The following execution claims are closed `[HANDED-OFF]` and own no files. Parent #91 reports their results locally integrated/verified on the historical unpushed umbrella lineage; do not describe them as GitHub-published integration until #113 (or a later integrator) publishes the umbrella ref:

- #103 — S1-D T-03 teaching-footprint cap decoupling + RackLesson reconciliation.
- #104 — S1-C T-05 objective/progression history v22.
- #93 — MFM-04 deterministic prescription core.
- #100 — MFM-05 spacing-only Crop Tree candidate system.
- #102 — PARITY-07 player-facing species truth/rendered qualification.
- #111 — MFM-06 v23 save/provenance implementation/qualification; exact reviewed production candidate `13fd854...`; not integrated by this closure.
- #112 — superseded extra cross-provider review; closed not-planned.

Browser Lane 03 (#118) is released/closed. Browser Lane 04 (#120) is the current browser-only Manager task.

## Prepared / blocked packets — no production ownership

### S1-D

- T-04: `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md` + sequencing amendment; before MFM-09.
- T-05: `Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md` + sequencing amendment; after MFM-07 API, before MFM-09.
- T-08: `Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md`; blocked until T-02..T-06 accepted.
- T-09: `Docs/Handoffs/S1-D-Implementation-09/TASK-PACKET.md`; blocked until T-08 PASS.

### S1-C

- T-04: `Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md`.
- T-06: `Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md`.
- T-07: `Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md`.
- T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md` after T-04/T-06/T-07.

### Multi-Scale Forestry Management V1

- MFM-06 integration: `Docs/Handoffs/MFM-06/INTEGRATION-TASK-PACKET.md` / #113.
- **MFM-07 current execution packet:** `Docs/Handoffs/MFM-07/TASK-PACKET.md` / #121; blocked on #113 + #120 approval.
- MFM-08: `Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md` after MFM-07.
- MFM-09: `Docs/Handoffs/MFM-Implementation-09/TASK-PACKET.md` after MFM-07/08 + S1-D T-04/T-05.
- MFM-10: `Docs/Handoffs/MFM-Implementation-10/TASK-PACKET.md` after MFM-09 + PARITY-08.
- MFM-11: `Docs/Handoffs/MFM-Implementation-11/TASK-PACKET.md` after stable parity/repeated-intervention flow.
- MFM-12: `Docs/Handoffs/MFM-Implementation-12/TASK-PACKET.md` integrated end-to-end verification/review.

### Three-species parity

- PARITY-08: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/TASK-PACKET.md` after MFM-06 integration; high-risk save/determinism.
- PARITY-09: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md` after PARITY-08 + MFM-10.

### Programme endgame

- Final Tier-3: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md` — one exact final candidate; mandatory fresh S1-C T-05 save/history rerun.
- Final independent review: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW-TASK-PACKET.md`.
- User-playtest structure: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF-TEMPLATE.md` — template only, not readiness.

## Recently released / completed coordination relevant to current work

- **PLAYER-TEST-BROWSER-LANE-03 (#118):** completed earlier live-state reconciliation, cross-feature sequencing, packet preparation and endgame preparation. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-03-RESULT.md`.
- **RACK-V2 programme through RACK-V2-14:** no live Rack ownership retained here.
- **S1-D T-06**, **PARITY-05**, **MFM-03**, **S1-C T-03:** no live writer retained here.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only. It grants no ownership and blocks no current feature.

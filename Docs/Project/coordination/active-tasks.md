# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / GPT-5.6 Sol / OpenAI browser lane #118 closeout.

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

GitHub `main` was refreshed at **`16f1b4fba7b435f398850d4ac0d60883b1335619`** when browser lane #118 started. Browser-lane coordination/handoff commits after that point advance `main` but do not by themselves advance the Scenario One integration candidate.

GitHub currently publishes `integration/scenario-one-player-test-ready-v1` at **`aca74acacc565f72ab802177cf69ca6ebb54d637`**.

Parent #91 and T3 #106 report a later umbrella candidate **`ac876a64969752a3864034521f42559671264b30`** containing S1-D T-03, S1-C T-05 v22, MFM-04/05 and PARITY-07. That candidate is explicitly reported **clean, local and unpushed**. It is valid local execution/orchestration evidence, but it is **not browser-inspected GitHub integration truth** until its exact SHA/ref is pushed and independently refreshable.

The same rule applies to descendants of `ac876a6`: IMPLEMENTED / REVIEWED / locally merged / Unity-qualified is not GitHub-published INTEGRATED merely because an issue comment reports it.

## Machine-wide Unity queue

There is no standing browser-side assertion that the Unity lock is free. Every T3 task must refresh free space, lock ownership and process state immediately before launch.

Current dependency order:

1. **#111 MFM-06 — save/provenance foundation** is the active local production task. It started from reported local `ac876a6`; its worker confirmed locally that `ForestSaveData.CurrentVersion == 22`, so this task owns v23 on that exact local lineage. Browser cannot independently inspect that unpushed starting SHA.
2. **#112 MFM-06 independent review** waits for #111's published exact candidate/handoff. Because #111 is actually being implemented by Claude Sonnet 5.5 / Anthropic, the required independent save/reference reviewer is now **OpenAI 6.1 Sol** to preserve cross-provider independence. If Sol remains unavailable, #112 waits; do not silently substitute another Claude reviewer.
3. **#113 MFM-06 integration gate** remains blocked until #111 is qualified/published and #112 returns APPROVED. Integration must refresh the actual umbrella head and push a GitHub-resolvable integration ref before canonical INTEGRATED status is claimed.
4. Only after accepted MFM-06 integration may the downstream packet DAG begin. Use `Docs/Handoffs/PlayerTestReadiness-Overnight-01/CROSS-FEATURE-OWNERSHIP-AND-SEQUENCING.md` for the single-writer order.

Do not infer a later item owns Unity until its child claim is RUNNING and the lock is acquired normally.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01 — orchestrate Scenario One to personal-playtest-ready candidate** (#91) | **DeepSeek orchestration/integration lead** | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, locks, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly only at parent level | Preserve child ownership; no product invention; high-risk save/reference work requires independent cross-provider review; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01 — local C#/Unity execution** (#106) | DeepSeek-dispatched local lane using user routing and provider independence | Child task worktrees; no blanket subsystem branch | C#/verification edits, save/version work, Unity/runtime/rendered gates and integration through matching child claims | Publish exact candidates/handoffs and pushed integration refs before claiming canonical integration | **RUNNING** |
| **MFM-06 — save/provenance foundation** (#111) | #106 local lane; actual worker Claude Sonnet 5.5 / Anthropic | `feature/mfm-v1-save-provenance-v23`; `/media/jer/Videos/CCF-MFM-06`; reported start `ac876a6` | MFM-06 v23 save/provenance schema/domain/verification only | No MFM-07 behaviour; preserve v22 history, Legacy40, Reference Future, pre-mutation rejection; publish exact candidate before review | **RUNNING** |
| **MFM-06 independent review** (#112) | OpenAI 6.1 Sol, cross-provider from Claude implementation | Review only | Exact #111 diff/handoff/evidence; no production ownership | Wait for GitHub-resolvable candidate and complete handoff; if Sol unavailable remain WAITING | **WAITING** |
| **MFM-06 integration gate** (#113) | #106 integrator only after review | Current umbrella integration lineage, refreshed at start | Bounded MFM-06 integration only | Requires #111 qualified/published + #112 APPROVED + clean ownership + post-merge checks | **WAITING / BLOCKED** |
| **PARITY-03 Q10 — mixed-density continuity evidence strengthening** (#74) | Waiting verifier when released | `verify/parity-03-density-continuity-q10-v1`; reviewed static candidate `3383e763e8ccff945dd00250366b7f334fe57331` | Verification only | Static Manager PASS; controller-level Unity evidence only if released; non-blocking evidence strengthening | **WAITING** |

## Released execution claims with publication caveat

The following execution claims are closed `[HANDED-OFF]` and no longer own files. Parent #91 reports their results locally integrated/verified on the unpushed `ac876a6` umbrella lineage, so those local SHAs must not be described as GitHub-published integration until the umbrella ref is pushed:

- #103 — S1-D T-03 teaching-footprint cap decoupling + RackLesson reconciliation.
- #104 — S1-C T-05 objective/progression history v22.
- #93 — MFM-04 deterministic prescription core.
- #100 — MFM-05 spacing-only Crop Tree candidate system.
- #102 — PARITY-07 player-facing species truth/rendered qualification.

Browser Lane 03 (#118) is also released/closed. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-03-RESULT.md`.

## Prepared but not dispatched — no production ownership

Packet preparation grants no ownership. Create substantive child claims only when the packet's start gate is true and the exact current base can be recorded.

### S1-D

- T-04: `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md` + `SEQUENCING-AMENDMENT-20261029.md`; must land before MFM-09; current default Claude Sonnet 5.5 Medium + OpenAI 6.1 Sol independent review.
- T-05: `Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md` + `SEQUENCING-AMENDMENT-20261029.md`; start after MFM-07 API and land before MFM-09.
- T-08: `Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md` + `ROUTING-AMENDMENT-20261029.md`; blocked until T-02..T-06 accepted; Claude Sonnet 5.5 Medium default executor.
- T-09: `Docs/Handoffs/S1-D-Implementation-09/TASK-PACKET.md` + `ROUTING-AMENDMENT-20261029.md`; blocked until T-08 PASS; OpenAI 6.1 Sol independent review.

### S1-C

- T-04: `Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md` — provenance labels / old-save honesty.
- T-06: `Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md` — bounded prior-treatment guidance.
- T-07: `Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md` — stable identity/history contract.
- T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md` — feature qualification after T-04/T-06/T-07.

### Multi-Scale Forestry Management V1

- MFM-06 current packet: `Docs/Handoffs/MFM-06/TASK-PACKET.md`; review routing amended by `Docs/Handoffs/MFM-06/REVIEW-ROUTING-AMENDMENT-20261029.md`.
- MFM-07: `Docs/Handoffs/MFM-Implementation-07/TASK-PACKET.md` — after accepted MFM-06 integration; proposals never Fell; Work Plan remains commitment gate.
- MFM-08: `Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md` — after MFM-07; individual 3D-grounded review and rendered qualification.
- MFM-09: `Docs/Handoffs/MFM-Implementation-09/TASK-PACKET.md` — after MFM-07/08 and S1-D T-04/T-05; stable teaching IDs + Work Plan integration.
- MFM-10: `Docs/Handoffs/MFM-Implementation-10/TASK-PACKET.md` — after MFM-09 + PARITY-08; Sitka-only species guard.
- MFM-11: `Docs/Handoffs/MFM-Implementation-11/TASK-PACKET.md` — calibration-impact comparison after parity/repeated-intervention flow is stable; independent OpenAI review.
- MFM-12: `Docs/Handoffs/MFM-Implementation-12/TASK-PACKET.md` — integrated end-to-end verification and independent review.

### Three-species parity

- PARITY-08: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/TASK-PACKET.md` — after MFM-06; high-risk save/determinism, Claude Opus 5.5 Low + OpenAI 6.1 Sol independent review.
- PARITY-09: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md` — after PARITY-08 + MFM-10; integrated parity qualification.

### Programme endgame

- Final Tier-3: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md` — one exact final candidate; mandatory S1-C T-05 save/history rerun.
- Final independent review: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW-TASK-PACKET.md` — OpenAI 6.1 Sol after Tier-3 PASS.
- User-playtest structure: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF-TEMPLATE.md` — template only; not a readiness verdict.

## Recently released / completed coordination relevant to current work

- **PLAYER-TEST-BROWSER-LANE-03 (#118):** completed live-state reconciliation, claim hygiene, cross-feature sequencing, all remaining T3 packet preparation and programme endgame preparation. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-03-RESULT.md`.
- **PLAYER-TEST-BROWSER-LANE-02 (#110):** completed live-state reconciliation, returned-work review, S1-D DAG audit and initial next-packet preparation. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-02-RESULT.md`.
- **PLAYER-TEST-BROWSER-LANE-01 (#105):** completed earlier T-05/T-03/PARITY-07/MFM-04/05 browser triage. Closeout: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-RESULT.md`.
- **RACK-V2 programme through RACK-V2-14:** execution claims/handoffs report field marking, teaching, persistence, functional/rendered qualification and Rack conformance completion; no live Rack ownership retained here.
- **S1-D T-06:** independent review and Unity qualification claims closed; no live writer.
- **PARITY-05:** later parity work treats it as qualified; no live writer.
- **MFM-03 calibration:** accepted input to MFM-04/05; no live writer.
- **S1-C T-03:** later S1-C work treats T-01/T-02/T-03 as integrated on the earlier published lineage; no separate S1-C T-03 writer retained here.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only. It grants no ownership and blocks no current feature.

# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md` rather than remaining as a live lock.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol under browser lane #127.

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

- Exact Unity-qualified MFM-06 production umbrella: **`b971f4da0540e606bfee9702b0c99f81a0a89355`**.
- Published `integration/scenario-one-player-test-ready-v1` head at the #127 refresh: **`7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`**, a documentation-only successor of the tested MFM-06 production merge.
- MFM-07 #121 is **HANDED-OFF / CLOSED — MANAGER APPROVED as an implementation candidate**, exact production candidate **`d4188b33e58b0fe67504a87167692d01b226c84e`**; docs-only successor `01755c47d537202e9cf479545be527d43121298f`.
- MFM-07 is **not yet integrated** into the umbrella. #125 owns that future integration/qualification when it actually starts.
- Therefore there is **no post-MFM-07 integrated umbrella SHA to pin yet**. Downstream tasks that require that SHA remain blocked/prepared only.
- Accepted MFM-07 residual: its production prescription-world adapter passes empty infrastructure-mask and management-feature arrays into the already-capable coverage core. Browser #127 determined that this requires a bounded prerequisite before shipped MFM-08 coverage UX; see #129.

## Immediate execution order

1. **#125 MFM-07 integration** — PREPARED FOR DISPATCH. Integrate exact approved `d4188b3...` onto the refreshed umbrella and publish exact merged/qualified SHA. No production ownership exists until the executor marks #125 `[RUNNING]` and refreshes this file.
2. **#123 S1-C T-04 provenance labelling** — PREPARED FOR DISPATCH, file-separable from #125. Root from its packet base unless Manager updates it after integration. No ownership until `[RUNNING]` is registered here.
3. **#124 S1-D T-04 guide monotonicity** — PREPARED FOR DISPATCH, file-separable from #125 and must land before MFM-09. No ownership until `[RUNNING]` is registered here.
4. **#126 PARITY-08 save/deterministic compatibility** — PREPARED FOR DISPATCH on exact approved MFM-07 candidate `d4188b3...`; it is the sole future save/validator/hash/reference writer when running. Mandatory independent review is pre-coordinated as blocked review claim #128.
5. Unity qualification remains serialized even when code-file ownership is disjoint.

There is no standing assertion here that the Unity lock is free. The active worker must refresh lock/process/free-space state immediately before Unity.

## Live tasks / reserved lanes

| Task | Primary / lane | Branch / worktree | Owned files or subsystem while active | Restrictions / dependencies | Live status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production stays on child lanes | Claims, dependency DAG, handoff triage, integration decisions, final Tier-3 assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | Child worktrees only | Local implementation/qualification through child claims | Exact candidate/handoff required; Unity serialized | **RUNNING** |
| **PLAYER-TEST-BROWSER-LANE-06** (#127) | Overall Manager / OpenAI GPT-5.6 Sol | GitHub/browser only; no production worktree | Handoff review, readiness/API inspection, claim/packet/coordination publication | No production C#/Unity/save execution/integration/rendered/Tier-3 execution | **RUNNING** |
| **MFM-07 integration** (#125) | Claude Sonnet 5.5 Medium or OpenAI local Code lane when started | integration worktree `/media/jer/Videos/CCF-PLAYER-TEST-READY` | MFM-07 merge/qualification only when actually started | Exact approved input `d4188b3...`; no MFM-08/09; no schema/save-validator edit | **PREPARED FOR DISPATCH — no ownership yet** |
| **S1-C T-04 provenance labelling** (#123) | Claude Sonnet 5.5 Medium or equivalent when started | `feature/s1-c-t04-provenance-labels-v1`; `/media/jer/Videos/CCF-S1C-T04` | `AnnualReviewView.cs`, `ManagementHistoryText.cs`, minimal history-label helper only after start | No save/schema/validator, MFM proposal/work-order or simulation edits | **PREPARED FOR DISPATCH — no ownership yet** |
| **S1-D T-04 guide monotonicity** (#124) | Claude Sonnet 5.5 Ultra Code preferred when started | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | teaching-sequence files/minimal pure helper only after start | No schema/validator/MFM; must land before MFM-09 | **PREPARED FOR DISPATCH — no ownership yet** |
| **PARITY-08 save/deterministic compatibility** (#126) | Claude Opus 5.5 Low preferred primary writer when started | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | Save validation/determinism/hash/reference compatibility only after start | Sole writer; exact candidate in claim; no rebaseline/schema bump; independent review required | **PREPARED FOR DISPATCH — no ownership yet** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane after #126 handoff | exact handed-off candidate; review-only | No production ownership | Blocked until #126 publishes exact candidate/handoff | **BLOCKED — REVIEW ONLY** |
| **MFM-08A production coverage-world adapter** (#129) | OpenAI Sol/Codex or equivalent local Code lane after unblocked | suggested `feature/mfm-v1-coverage-world-adapter-v1`; `/media/jer/Videos/CCF-MFM-08A` | smallest coverage-world adapter surface only after exact base is pinned | Blocked on #125; do not invent missing infrastructure geometry; no UI/teaching/Work Plan/save/calibration | **BLOCKED — NO OWNERSHIP** |
| **PARITY-03 Q10 evidence strengthening** (#74) | Waiting verifier when released | existing verification lane | Verification only | Non-blocking evidence strengthening | **WAITING** |

## Ownership boundaries

### #125 MFM-07 integration

- Integration only: exact approved MFM-07 candidate plus its published docs/handoff lineage.
- Preserve proposal != mark/order, prescription approval creates Pending work only, Work Plan remains commitment gate, and fresh pre-write prescription/unit/Crop-Tree validation with rollback.
- No MFM-08 UX, MFM-09 teaching, save/schema/validator/hash/reference edit or product decision.

### #123 S1-C T-04

- History/review provenance labels only.
- Must not edit save/load/validation, `ScenarioOneManager` work-order creation, MFM proposal/approval logic or prescription persistence.

### #124 S1-D T-04

- Teaching/current-state monotonicity only.
- Must not edit save/load/validation, completion rules, Rack lifecycle or MFM logic.

### #126 / #128 PARITY-08

- #126 is the only prospective writer for save validation/determinism/hash/reference compatibility.
- #128 is strictly independent read-only review; repairs return to #126 or a newly assigned repair claim.
- Never allow #128 to become a second save writer.

### #129 MFM-08A prerequisite

- Existing pure coverage rules already accept masks/features; this task must only bridge authoritative live state into that contract.
- Known authoritative manager state includes planted juveniles, clearance patches, shelters, deadwood and work orders. Inspect authoritative infrastructure geometry before mapping mask kinds.
- If a required headland/track/clearing/site mask lacks an authoritative source/API, stop and return the missing source to Manager. Do not invent coordinates/radii or silently omit accepted exclusions.
- No MFM-08 map/3D UX may rely on production coverage until this prerequisite is completed/verified or Manager explicitly disposes it another way.

## Dependency-ready sequencing

### S1-C review/history

- T-04 = #123, prepared.
- T-06 follows accepted T-04 with no overlapping history writer.
- T-07 stable identity/history contract waits for save/identity ownership to be free and one exact current integration lineage.
- T-08 feature qualification waits for T-01..T-07 accepted on one exact candidate.

### S1-D / Work Plan / teaching

- T-04 = #124, prepared; must precede MFM-09.
- T-05 remains blocked until #125 publishes the exact post-MFM-07 integrated umbrella and pending-work/provenance API. It owns only previous-vs-proposed Work Plan presentation; it must reuse S1-C history and must not absorb MFM-09 teaching.
- T-08 waits for T-02..T-06 acceptance; T-09 is independent spec review after T-08 PASS.

### MFM

- MFM-08A prerequisite = #129, blocked on #125 exact integrated SHA.
- MFM-08 map/3D review UX follows #129 + integrated MFM-07 and available UI ownership.
- MFM-09 follows MFM-08 plus accepted S1-D T-04/T-05; teaching ownership stays in MFM-09 only.
- MFM-10 follows MFM-09 + PARITY-08 and retains Sitka-only quantitative scope.
- MFM-11 follows stable parity/repeated-intervention/player-facing flow.
- MFM-12 follows MFM-11 plus parity/S1-C/S1-D qualification.

### Three-species parity

- PARITY-08 = #126, with mandatory independent review #128.
- PARITY-09 waits for PARITY-08 PASS + independent review APPROVED + MFM-10.

### Programme endgame

- Final Tier-3 uses one exact final candidate only after Rack/parity/S1-C/S1-D/MFM dependencies are green.
- User personally playtests the qualified candidate next.
- Windows build remains prohibited until explicit post-playtest user authorisation.

## Released/completed claims relevant to current state

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04; #121 MFM-07 implementation; #122 browser lane 05.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.

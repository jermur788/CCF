# CCF — MFM-12 Local Integrated Qualification Handoff

Date: 2026-11-02
Owner/orchestrator: #203 local orchestration
Claim: #225

## Purpose

Execute the final integrated qualification for Multi-Scale Forestry Management V1 on the exact post-#223/#224 Scenario One integration lineage. This is a qualification and independent-review task, not a production-repair task.

## Difficulty

**High.** This crosses persistence, Rack-derived sample/coverage, candidate/proposal/player-judgement boundaries, Work Plan commitment, ordinary economy, rendered UX, history/provenance, repeated management cycles and complete-loop Scenario One completion. A defect found during qualification must be returned rather than repaired in place.

## Best-suited provider/model

- Primary local lane: **SOL/CODEX high-reasoning** if available.
- If the requested lane is unavailable, #203 may route to the strongest suitable local implementation/qualification model, but must record the actual provider/model honestly and preserve all stop conditions.
- Independent reviewer: a separate cross-provider reviewer, preferably **Claude Sonnet 5.5** or another capable reviewer independent of the qualification thread.

## Binding authority

Read first:

- `Docs/Handoffs/MFM-12-Integrated-Qualification/TASK-PACKET.md`
- `specs/multiscale-forestry-management-v1/spec.md`
- `specs/multiscale-forestry-management-v1/tasks.md`
- `specs/multiscale-forestry-management-v1/checklists/requirements.md`
- `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/MANAGER-REVIEW-20261102.md`
- `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/MANAGER-REVIEW-20261102.md`

## Exact lane

Repository: `jermur788/CCF`

Exact Scenario One integration candidate:

`508b3e8124b5d9c477a4d2471cd56ebf6608f197`

Branch:

`qual/mfm-12-integrated-v1`

Worktree:

Use one dedicated clean worktree assigned by #203 and record its exact path in #225 before execution.

## Claim / ownership preflight

Before any Unity or qualification work:

1. refresh `Docs/Project/coordination/active-tasks.md` from live GitHub;
2. verify #225 is READY/REPinned and that no conflicting owner holds the relevant lane;
3. verify the exact starting HEAD is `508b3e8124b5d9c477a4d2471cd56ebf6608f197`;
4. verify clean worktree;
5. check current Editor processes and the machine-wide Unity lock;
6. update #225 to `[RUNNING]` and record provider/model, branch, worktree and starting HEAD.

Heavy Unity remains serialized machine-wide. A historical free receipt is not sufficient.

## Required qualification

Use `checklists/requirements.md` as the binding evidence matrix. For each applicable requirement A–P, provide exact evidence or explicitly justified inherited evidence.

At minimum qualify:

- authoritative individual tree/ecology-cell state and absence of parallel stand biology;
- Rack V2 persistent geometry/world identity and Rack-derived sample/coverage;
- teaching sample semantics and separation of 8 m spacing from 8 m Hegyi radius;
- 30% first release and 40% cumulative basal-area safeguards;
- coverage states/reasons and rack-corridor exclusions;
- deterministic prescription from player-executed sample decisions;
- no hidden quality/vigour/crown ranking;
- spacing-only outside-sample Crop Tree candidates;
- candidate/proposal/player-judgement boundary, including usable manual alternative;
- Sitka-only quantitative logic not applied to Oak/Beech;
- proposal != Fell mark and no premature Work Plan leakage;
- prescription approval creates pending ordinary work only;
- Work Plan approval remains the commitment gate;
- ordinary contractor/landowner/economy path remains authoritative;
- manual-state precedence and durable prescription provenance;
- save/load persistence, old-save compatibility, malformed-data rejection before mutation;
- Legacy40 compatibility and exact frozen Reference Future v1 preservation;
- deterministic continuation after save/load;
- rendered 1280×720 and 1920×1080 distinctions among sample, coverage, candidate, proposal, approved work and executed result;
- map coverage/reasons, 3D navigation/confirm/reject and proposal inspection;
- #199 minimap/focus contracts;
- guided-v2 remaining green after #223/#224;
- MFM-11 calibration boundary and ordinary economy coherence;
- prescription/work-order provenance through Pending → Work Plan approval → execution → Annual Review/history;
- read-only history behavior;
- coexistence with S1-C review/history and S1-D repeated interventions across at least two management cycles.

## Current regression obligations

Confirm current exact-candidate results for at least:

- BroadleafTimberEconomyVerification — Legacy40 and Enlarged80;
- MixedDensityVerification;
- pruning verification;
- ThreeSpecies history suite;
- guided-v2;
- PARITY-07 / Combined / Rack rendered regressions as relevant.

The accepted three pure-Beech R2 misses remain a C-level limitation against the unchanged 0.40 criterion. Do not retune or weaken that criterion.

The accepted MFM-11 post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED** unless an explicitly authorised new diagnostic actually proves it. Do not promote that claim by inference.

## Complete-loop witness

Produce one explicit exact-candidate witness covering:

`initial forest → assess/access → sample teaching → extend treatment → review/confirm player judgement → prescription approval → Work Plan approval → execute → advance → Annual Review/history → later intervention → adapt/repeat → completion`

Record intervention years, material stand/economy/ecology transitions, species introductions required by completion, and final completion/objective state.

## Independent review

After qualification evidence is complete, obtain one fresh independent cross-provider review of the exact candidate and evidence package.

Required verdict:

`APPROVED / NEEDS CHANGES / REJECTED`

Reviewer must inspect exact ancestry, checklist coverage, runtime/rendered evidence, persistence/compatibility, protected anchors, player-judgement boundary, absence of hidden automation, and whether any PASS claim exceeds the evidence.

## Stop conditions

Stop and return to Browser Overall before any production repair if:

- any production/spec defect is found;
- any production edit would be needed;
- Reference Future v1, Legacy40 or protected pure-Sitka anchors move unexpectedly;
- required current-product rendered gates remain red;
- malformed/old saves mutate before required rejection;
- player judgement can be bypassed;
- unapproved proposals reach committed work;
- complete-loop Scenario One completion is unreachable;
- the exact candidate cannot be maintained;
- a blocking independent-review finding appears.

Do not weaken tests, retune production, silently rebaseline, or absorb unrelated dirty work.

## Protected boundaries

- No production repair under #225.
- No Windows build.
- #194 remains NO ACTION.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- No new automation that bypasses player judgement.

## Durable return

Publish exactly:

- `Docs/Handoffs/MFM-12-Integrated-Qualification/HANDOFF.md`
- `Docs/Handoffs/MFM-12-Integrated-Qualification/INDEPENDENT-REVIEW.md`

Return must include exact start/final HEAD, worktree, commands/results, gate counts, screenshots manifest, save/reference fingerprints, complete-loop witness, A–P checklist matrix, Unity lock lifecycle, cleanup state, independent reviewer identity/verdict/findings, and explicit unresolved limitations.

Successful qualification return should state:

`MFM-12 QUALIFICATION PASS / INDEPENDENT REVIEW APPROVED / READY FOR MANAGER ACCEPTANCE`

Do **not** self-issue the product token:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

That token remains Browser Overall's acceptance decision after review.

## User action

Paste the Browser Overall dispatch containing the direct GitHub URL to this handoff into the CCF local orchestrator chat. No other user action is required until the result returns.

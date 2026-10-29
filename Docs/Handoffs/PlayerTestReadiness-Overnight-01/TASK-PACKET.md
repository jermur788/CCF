# PLAYER-TEST-OVERNIGHT-01 — DeepSeek orchestration to Scenario One personal-playtest-ready candidate

## Task identity

- **Parent claim:** #91
- **Task class:** SPEC REQUIRED umbrella orchestration over already-approved feature specs; this packet creates no new gameplay requirement.
- **Orchestrator / integrator:** **DeepSeek agent**, Manager-assigned for this umbrella task.
- **Mandatory first delegate:** **Claude A = Claude Opus, medium**.
- **Other authorised delegate pools:** **Claude B** and **OpenAI 6.1 Sao/Sol**, using the strongest available configuration appropriate to each bounded task.
- **Observed starting `main`:** `775947a52f66be4b9f1a202a77edebf484a05d0b`.
- **Context commit:** `d05b27ad376ce02748ee4703429ad3ff66a65542`.
- **Suggested orchestration branch:** `orchestration/scenario-one-player-test-ready-v1`.
- **Suggested non-Unity orchestration worktree:** `/media/jer/Videos/CCF-Orchestrator-PlayerTest` or another clean non-ZX20 path. Do **not** consume scarce ZX20 space for the parent coordination lane.

Refresh `origin/main`, this packet, claim #91, all referenced child claims, and live `Docs/Project/coordination/active-tasks.md` before acting. If `main` has advanced, use the newer state unless it creates a real authority/dependency conflict; record the actual starting SHA.

## One outcome

Continue the accepted Scenario One programme until there is **one exact, integrated, Tier-3-qualified candidate ready for the user's personal player test**.

Parent success token:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

This means the game/repository candidate is ready for the user to personally test. It does **not** mean user acceptance, external-test readiness, native-Windows qualification, or permission to build Windows.

## Non-negotiable release boundary

**NO WINDOWS BUILD.** The user must personally playtest the Scenario One candidate and explicitly authorise a Windows build first.

Do not create, refresh or distribute a Windows candidate under this task.

## Authority

Use the existing hierarchy:

1. latest direct user instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`;
5. approved task-level specs below.

Implementation truth is inspected repo / verified Unity state, then `Docs/Project/unity-project-overview.md`.

Governing approved specs:

- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`
- `specs/scenario-one-three-species-parity-p0/`
- `specs/scenario-one-review-history-v1/`
- `specs/scenario-one-repeated-interventions-v1/`
- `specs/multiscale-forestry-management-v1/`

Do not invent product behaviour to keep the overnight run moving. If a genuine unresolved product/design decision is exposed, stop the affected branch and continue only independent safe work that does not depend on that decision.

## Orchestration authority and limits

DeepSeek is the **orchestration/integration lead for this parent task**, not a replacement product owner. Within already-approved specs it may:

- maintain the execution DAG;
- create/reuse child claims;
- create branches/worktrees;
- assign bounded workers/reviewers;
- update live `active-tasks.md` for this umbrella task as the assigned integrator;
- acquire/release the Unity lock through the normal mechanism;
- inspect child handoffs and issue technical `APPROVED / NEEDS CHANGES / REJECTED` integration decisions against accepted requirements;
- integrate approved child work one lane at a time;
- run/assign post-integration qualification;
- update implementation-status documentation with verified facts;
- continue automatically when the next step is already determined by the approved specs.

DeepSeek may **not** change accepted product direction, weaken a mandatory gate, silently rebaseline protected anchors, modify Reference Future v1, bypass ownership, or infer success from chat text alone.

## Mandatory model sequence

### First substantive delegate — required

After DeepSeek performs only minimal preflight (repo/claim/lock availability), it must delegate the first substantive assessment to:

**Claude A — Claude Opus, medium**.

Claude A's first task is a read-only programme/readiness audit. It must inspect:

- current `main` and live active tasks;
- this umbrella packet and claim #91;
- the five approved specs/task lists;
- existing child claims/handoffs (#90, #88, #19, #70, #74, #39);
- current worktrees/branches/dirty state;
- current Unity lock/process state;
- current free space on `/media/jer/ZX20`;
- the remaining dependency graph and likely overlapping file ownership.

Claude A publishes:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/CLAUDE-A-START-REVIEW.md`

with actual model/provider, inspected SHA, storage result, dependency/ownership risks, and verdict:

`OVERNIGHT PROGRAMME READY TO EXECUTE`

or

`OVERNIGHT PROGRAMME BLOCKED — MANAGER DECISION REQUIRED`.

**Do not silently substitute another model for this first delegate.** If Claude A / Opus / medium is unavailable, mark #91 `[BLOCKED]` and stop before substantive implementation.

### Later model routing

After the Claude A start review passes, DeepSeek may use:

- **Claude A / Opus medium:** difficult architecture, save/reference review, independent spec-conformance review, final candidate review.
- **Claude B:** bounded implementation, UI/copy, verification or secondary review where its available capability fits.
- **OpenAI 6.1 Sao/Sol:** strong code/Unity implementation, qualification, debugging and cross-provider independent review.
- **DeepSeek:** orchestration, task splitting, evidence triage, integration and bounded pure-code implementation only where no independence conflict is created.

For high-risk save/reference/determinism work, prefer cross-provider independence: if Claude implements, OpenAI reviews; if OpenAI implements, Claude A reviews. A helper/child of an implementing thread does not count as the independent reviewer for that same change.

## Parent claim and child claims

Claim #91 is the single user-facing umbrella task. It **does not replace child execution claims** required by AgentWorkflow.

Reuse existing claims when their scope still matches:

- **#90 — RACK-V2-09:** physically mark confirmed rack entrance trees in orange.
- **#88 — S1-D T-06:** management-event ID save-hardening Unity qualification.
- **#19 — S1-C T-03:** intervention drill-down and money-honesty rendered qualification.
- **#70 — PARITY-05:** early-life growth/browsing focused Unity qualification.
- **#74 — PARITY-03 Q10:** non-blocking mixed-density continuity evidence strengthening.
- **#39 — MFM-03:** representative-management coverage calibration.

Create one new child claim for each later substantive dispatched task that lacks one. Every child claim must follow `[DISPATCHED] → [RUNNING]/[BLOCKED] → [HANDED-OFF]` with actual model/provider, branch/worktree and starting HEAD.

## Machine/storage precondition

The last known state had `/media/jer/ZX20` at about **11.5 GiB free**, below the **15 GiB Unity-lock floor**. No heavy Unity launch is allowed until fresh preflight proves the floor is restored.

### Authorised storage remediation

The orchestrator may restore the floor using only **safe, reversible relocation** of inactive material after ownership verification. Preferred target is `/media/jer/Videos/CCF-ZX20-overflow/`.

Allowed, after inspection:

- relocate **clean, inactive, completed/merged** worktrees from ZX20 to the Videos overflow area;
- byte/checksum-verify the copy before removing the old location;
- use `git worktree repair` after relocation;
- relocate inactive disposable scratch/verification/cache directories that are demonstrably regenerable and unowned;
- prefer restoring at least **20 GiB free** for practical headroom, but the canonical lock floor remains 15 GiB.

Forbidden:

- touching a dirty worktree;
- deleting/stashing/resetting/cleaning user or unrelated work;
- touching active task worktrees without the owning task's explicit handoff/release;
- touching user saves/backups or evidence archives;
- force-removing a worktree merely because its branch is old;
- bypassing the Unity storage-floor check.

If the 15 GiB floor cannot be restored safely under these rules, mark #91 `[BLOCKED — STORAGE]`, publish the exact inventory/blocker, and do not launch Unity.

## Execution strategy

This is one programme, not one giant branch. Use bounded child branches/worktrees and one primary writer per subsystem. Parallelise only non-overlapping offline/static work. Heavy Unity remains serialized machine-wide.

Where a spec task list still says TODO despite already-published implementation evidence, inspect the actual repository/handoff first and **do not redo completed work**. Reconcile status from evidence.

### Phase A — Finish Rack V2 / D-064

Current known state: RACK-V2-07 and RACK-V2-08 are integrated; RACK-V2-08 merged via PR #89 at `d925e9591f2fffb9419881a0e77031cefccf9aeb`.

Proceed through the remaining approved Rack tasks and dependencies:

1. **RACK-V2-09 — field marking** using claim #90.
2. **RACK-V2-10 — Help/teaching**.
3. **RACK-V2-11 — persistence/save compatibility** using the next available schema/version exactly as the approved spec requires.
4. **RACK-V2-12 — functional end-to-end qualification**.
5. **RACK-V2-13 — rendered review at 1280×720 and 1920×1080**.
6. **RACK-V2-14 — integration/spec conformance**.

Protected behaviour: Enlarged80 map authoring; five-rack Scenario One calibration; orange first-entrance-tree marking; exclusive mark semantics; Legacy40 path; frozen Reference Future v1; no second removal mechanism; Confirm still creates pending work before approval/execution.

Rack exit required before MFM production coding:

`INTEGRATED CANDIDATE READY`

for the Rack spec, with persisted confirmed rack geometry available to MFM.

### Phase B — Clear already-waiting Unity gates

Once Rack's Unity-critical dependency is satisfied, clear the existing qualified waiting slices in dependency-aware order:

- #88 **S1-D T-06 management-event ID save-hardening Unity qualification**, then integrate only after the runtime no-mutation/save-hardening/cell-index/U4/import gates pass.
- #19 **S1-C T-03 intervention drill-down/money honesty rendered qualification**, then integrate if accepted.
- #70 **PARITY-05 early-life/browse focused Unity qualification**.
- controller-level Unity portion of #74 **PARITY-03 Q10** if still useful; Q10 is evidence strengthening and must not become an invented blocker for already-accepted PARITY-03.

Static/offline work that does not overlap may run while Unity is occupied.

### Phase C — Complete three-species parity P0

Do not redo PARITY-01/02 or accepted PARITY-03/04 work. Reconcile live evidence first.

Complete the remaining approved parity programme:

- finish PARITY-05 focused qualification;
- **PARITY-06 — Broadleaf timber/economy/pruning gap closure**;
- **PARITY-07 — player-facing species truth**;
- **PARITY-08 — save/deterministic compatibility, including unknown-species pre-mutation rejection**;
- **PARITY-09 — integrated parity qualification + independent review**.

Required parity exit:

`THREE-SPECIES PARITY P0 VERIFIED`

No fabricated empirical precision, no Sitka anchor rebaseline, no DEC/DMC/mixture productivity bonus.

### Phase D — Complete S1-C review/history

Known integrated: T-01 and T-02. Known static accepted but initially waiting: T-03.

After T-03 qualification/integration, complete only the still-unmet approved tasks:

- **T-04 provenance labelling**;
- **T-05 objective/progression history** using the next available save schema at the actual integration base;
- **T-06 prior-treatment guidance**;
- **T-07 S1-D data contract/stability checks**;
- **T-08 feature qualification/spec conformance**.

Coordinate `AnnualReviewView`, `WorkPlanView`, history helper and shared save/version surfaces with S1-D/MFM writers. Do not create a second history UI.

Required S1-C result: complete checklist PASS and integrated accepted feature slice.

### Phase E — Complete S1-D repeated interventions

Known complete/integrated: T-00, T-01, T-02. T-06 has independent source/executable review PASS and requires the Unity/integration gate above. T-07 is cancelled.

Complete still-unmet authorised tasks:

- **T-03 — decouple teaching-footprint cap after lesson completion**;
- **T-04 — guide monotonicity**, sequenced with MFM curriculum ownership;
- **T-05 — previous versus proposed presentation**, integrated through S1-C surfaces rather than duplicated;
- **T-06 — integrate only after #88 passes**;
- **T-08 — three-cycle witness + compatibility/solvency**;
- **T-09 — independent spec-conformance review**.

Required S1-D result: repeated management cycles remain reachable, non-prescriptive and compatible, and the spec checklist receives an independent PASS.

### Phase F — Complete Multi-Scale Forestry Management V1 / D-065

Known integrated: MFM-02. Existing claim #39 owns MFM-03.

After persisted Rack V2 geometry is integrated and ownership is clear:

- **MFM-03 — coverage calibration measurement**;
- **MFM-04 — deterministic prescription core**;
- **MFM-05 — spacing-only Crop Tree candidate system**;
- **MFM-06 — save/provenance foundation** using the actual next available schema after Rack/S1-C sequencing;
- **MFM-07 — proposal state and approval boundary**;
- **MFM-08 — map/3D review UX**;
- **MFM-09 — teaching sequence/Work Plan integration**, coordinated with S1-D guide monotonicity;
- **MFM-10 — species-parity integration guard** after parity is stable;
- **MFM-11 — calibration impact qualification** with independent review;
- **MFM-12 — integrated verification and independent review**.

Critical invariants:

- proposals are never `TreeMarkType.Fell`;
- player judgement remains required;
- no bulk Crop Tree confirmation;
- prescription approval creates pending ordinary work, not committed work;
- Work Plan approval remains the commitment gate;
- manual player state wins conflicts;
- V1 quantitative treatment is Sitka-adult only unless the spec is explicitly amended;
- Legacy40 and Reference Future v1 remain unaffected.

Required MFM exit:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

### Phase G — Complete-loop candidate and Tier-3 qualification

After Rack, parity, S1-C, S1-D and MFM are integrated on one candidate lineage:

1. create one clean candidate branch/worktree from the integrated head (suggested `candidate/scenario-one-player-test-ready-20261029`);
2. verify clean import/compile;
3. run a **Tier-3 full integrated Scenario One qualification** appropriate to the accepted milestone;
4. prove the player can start from the initial forest, assess, plan/manage, execute, advance, review, make second/later interventions, repeat and reach understandable completion;
5. exercise three-species parity, Rack V2 planning/marking/persistence, S1-C history, S1-D repetition, MFM sample→coverage→proposal→approval→Work Plan flow, save/load and protected compatibility;
6. include required rendered states at 1280×720 and 1920×1080;
7. compare protected Reference/Legacy/determinism anchors required by the integrated features;
8. run completion/solvency witness(es) sufficient to answer the stated uncertainty; do not multiply long simulations without a reason;
9. independently review the final integrated candidate with a model/provider that did not implement the dominant final fixes — prefer Claude A Opus medium if available and independent;
10. leave the user a clean exact candidate with simple launch instructions for personal playtest.

Do not mark the user playtest itself complete.

## Final candidate acceptance conditions

The parent task may emit the success token only when all are true:

- required approved-spec work is integrated on one exact candidate SHA;
- no mandatory child claim remains required for personal-playtest readiness;
- Rack V2 complete flow is functional, persisted and visually qualified;
- three-species parity P0 is VERIFIED;
- S1-C complete review/history slice is qualified/integrated;
- S1-D repeated-intervention slice is independently reviewed and qualified;
- MFM V1 reaches its MFM-12 integrated/user-playtest-ready gate;
- full Scenario One completion remains reachable and understandable;
- Tier-3 integrated qualification is green;
- protected Legacy40 / Reference Future v1 / save / determinism contracts are preserved or any authorised changes are explicitly evidenced;
- candidate branch/worktree is clean and exact SHA is published;
- launch instructions for the user's personal test are published;
- **no Windows build occurred**.

## Final handoff

Publish:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF.md`

It must include:

- DeepSeek orchestrator identity/version/provider;
- every delegated worker/reviewer, including Claude A first-step evidence;
- exact final candidate SHA, branch and worktree;
- integrated PRs/commits;
- child claim table and final states;
- specs/checklists and conformance verdicts;
- Tier-3 commands/gates/results;
- protected anchor/save/reference evidence;
- known limitations clearly separated into blockers/non-blockers;
- exact user launch steps;
- confirmation that no Windows build was made;
- `USER ACTION REQUIRED: personally playtest this exact candidate`;
- dashboard sync status; if no publisher exists, record `DASHBOARD SYNC: NOT AVAILABLE` rather than fabricating success.

Then mark #91 `[HANDED-OFF]` and close it with:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

## Hard stop conditions

Stop the affected work and publish a blocker instead of inventing a fix if any of these occur:

- a genuine product/design choice not settled by user/Decision Log/spec;
- two active owners need to write the same subsystem and cannot be cleanly sequenced;
- storage cannot be restored above the Unity floor without risking unrelated/dirty/user work;
- accepted historical save/Reference data conflicts with a proposed compatibility rule;
- protected anchor movement has no authorised explanation;
- a required schema version conflicts with another integrated/active owner;
- completion becomes unreachable without changing accepted ecology/economy/objective coefficients;
- a mandatory review/verification gate cannot be run with available tools;
- the mandatory first Claude A / Opus-medium delegate is unavailable.

A blocker in one lane does not require abandoning independent safe work in other lanes, but the parent success token is forbidden until every required blocker is resolved.

## Copy/paste launch line

`Orchestrate PLAYER-TEST-OVERNIGHT-01 from Docs/Handoffs/PlayerTestReadiness-Overnight-01/TASK-PACKET.md using parent claim #91. You are the DeepSeek orchestration/integration lead for this umbrella task. Refresh main, active-tasks and all child claims first. Your first substantive delegate MUST be Claude A using Claude Opus on medium for the read-only programme/readiness audit and CLAUDE-A-START-REVIEW.md; do not substitute another model for that first step. After its PASS, continue autonomously through the accepted Rack V2, three-species parity, S1-C, S1-D and MFM V1 task graphs, reusing #90/#88/#19/#70/#74/#39 and creating bounded child claims/branches/worktrees for later tasks. You may delegate to Claude B and OpenAI 6.1 Sao/Sol as available, using cross-provider independent review for high-risk save/reference/determinism work. Restore the ZX20 >=15 GiB Unity floor only through safe reversible relocation of clean inactive material; never touch dirty/user/active work. Serialize heavy Unity, integrate only qualified work, run the final Tier-3 complete-loop qualification, and stop only for a genuine authority/safety/mandatory-gate blocker. Final exit is SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED with exact candidate SHA/worktree/launch steps. NO WINDOWS BUILD.`

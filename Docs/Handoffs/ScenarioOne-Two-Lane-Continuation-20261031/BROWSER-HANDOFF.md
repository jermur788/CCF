# Scenario One Browser / GitHub Manager Lane 03 — Handoff

**Claim:** #200  
**Parent:** #91  
**Role:** browser/GitHub Manager lane  
**Prepared:** 2026-10-31  
**Context snapshot:** `d05b27ad376ce02748ee4703429ad3ff66a65542`  
**Main at split:** `8bd03bca142bbea6d379ad5f0a506f3c0922d2ec`  
**Published Scenario One integration at split:** `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

## Purpose

Take **all work that can safely be done in the browser/GitHub lane** out of the long-running local orchestration task. This chat is the Overall Manager/browser counterpart to #201 and should keep moving whenever a local worker is running, waiting on Unity, or has returned a handoff.

This lane owns Manager review, repository inspection, routing/packet preparation, claim/coordination maintenance and browser-safe canonical documentation. It does **not** own local C#/Unity implementation or runtime verification.

## Authority

Use current project authority in this order:

1. latest direct user instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`;
5. approved task-level specs within their scope.

Implementation truth remains inspected repository / verified Unity evidence, then `Docs/Project/unity-project-overview.md`.

Always refresh `Docs/Project/coordination/active-tasks.md` from live GitHub at substantial task start. This handoff is not a substitute for that live file.

## Two-lane ownership rule

- **#200 — this browser lane:** sole primary writer for `Docs/Project/coordination/active-tasks.md` during this split; Manager decisions; GitHub issue/claim status; browser reviews; packet/repin preparation; canonical status/docs updates; safe GitHub-only administration.
- **#201 — long-running deep orchestration:** all non-browser execution: local worktrees, C#/Unity implementation, tooling, compiler/Editor/runtime verification, local integration and long-running worker routing.
- Child task claims remain mandatory and record execution presence only.
- The two umbrellas do not grant file ownership. Refresh live ownership before every child starts.
- Do not create two writers for the same branch/file set. If #201 reports overlap, resolve it here before production edits continue.

## Browser lane responsibilities

### 1. Review every returned handoff promptly

For each substantive return:

- inspect the exact GitHub handoff, changed files, candidate SHA and ancestry;
- inspect relevant specs/decisions/current integration;
- distinguish IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED;
- issue canonical `APPROVED / NEEDS CHANGES / REJECTED` where a Manager decision is due;
- update/close the execution claim correctly;
- publish a Manager-review Markdown record when substantive;
- create/repin the next executable child packet when the next step is already clear;
- update `active-tasks.md` and #91/#200/#201 routing surfaces after material state changes.

Do not accept Unity/runtime/rendered claims from chat text alone. Require the pushed handoff and actual evidence receipts.

### 2. Perform browser-capable diagnosis and review

Use GitHub/browser inspection for:

- commit/branch ancestry and scoped diff review;
- test/harness contract inspection;
- source-level root-cause classification before authorising production repair;
- save/reference/spec conformance review when pushed evidence is sufficient;
- independent read-only review where local execution is not required;
- identifying stale verification contracts versus production regressions;
- checking that a proposed repair does not weaken a protected gate.

When a question requires fresh Unity/runtime evidence, issue a bounded child to #201 rather than inferring the result.

### 3. Prepare and release exact packets

This lane owns browser-side dependency transitions:

- repin task packets to the exact accepted combined SHA;
- create claims before substantive dispatch;
- record exact handoff path, purpose, difficulty/reason, provider/model recommendation, lane/branch/worktree, claim/status, dispatch line and user action;
- maintain the approved spec/task chain without silently changing product intent.

### 4. Safe integration management

Browser inspection may approve and prepare integration, but local code integration/import/compile/runtime work belongs to #201 when those checks matter.

Before any integration decision:

- inspect exact candidate and target lineage;
- check overlapping files/subsystems and active ownership;
- keep verification-only commits distinct from production commits where possible;
- preserve frozen Reference Future v1, Legacy40 and other protected anchors;
- require post-integration verification proportional to risk.

### 5. Canonical/status maintenance

Keep current state accurate in:

- `Docs/Project/coordination/active-tasks.md`;
- relevant issue bodies/comments/titles;
- #91 orchestration status;
- current milestone / overview only when their canonical meaning genuinely changes;
- dashboard/status publication where the project workflow uses it.

Do not manually edit generated dashboard state.

## Current exact programme state

### MFM-08

- #156 remains **NEEDS CHANGES — verification closure only**.
- Exact repaired production candidate: `e5fb424635f16182748efcb0539e8a49539495a9`.
- Existing MFM-08 evidence remains: unmodified gate 353/353 PASS; strengthened open-work-conflict gate 373 PASS at 1280×720 and 1920×1080.
- #196 is the **only MFM-specific closure blocker**: state-aware RackV2MapPlanning navigation-tab reconciliation plus full rerun on exact `e5fb4246...`.
- First browser trigger: when #196 returns, review it immediately. If the full gate passes under a non-weakened contract and no new MFM-specific failure appears, decide whether #156 can be upgraded and prepare the exact safe integration step.

### Separate current-lineage verification/UI debt

#197 is APPROVED/CLOSED and classified the old red gates:

- #198 — `guided-v2` v24 / D-066 verification-contract reconciliation. Verification-only first.
- #199 — real current-lineage Scenario One Combined HUD/focus regression repair. Production UI/focus repair is allowed only within its bounded packet; protected Combined HUD assertions must not be weakened.

These are pre-Tier-3 debt. They do **not** automatically block MFM-08 approval unless new evidence directly threatens accepted Rack V2/D-064 behaviour required by MFM-08.

### Remaining Scenario One chain

Subject to current live board and returned evidence:

1. #196 → Manager review here → #156 approval/integration if warranted.
2. Fresh repin/release MFM-09 after accepted/integrated MFM-08.
3. MFM-09 → MFM-10; MFM-10 must return `SPECIES GUARD QUALIFIED`.
4. MFM-10 green → fresh PARITY-09 repin/qualification and MFM-11 eligibility.
5. S1-C T-08 final history qualification remains dependency-cleared on T-07 and needs a serialized Unity slot.
6. S1-D T-08 multi-cycle witness → S1-D T-09 independent browser review.
7. MFM-11 → MFM-12.
8. #198/#199 must be resolved before final Scenario One Tier-3 where their protected gates are part of integrated qualification.
9. Required Rack V2 + PARITY + S1-C + S1-D + MFM state accepted/integrated on one exact candidate → final Tier-3.
10. Tier-3 success token remains `SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`.
11. User personally playtests that same candidate.
12. **No Windows build before explicit post-playtest user authorisation.**

The live board, not this numbered summary, decides exact serialization when state changes.

## Current browser work that can run while #201 is busy

- inspect any pushed child branch/handoff as soon as it appears;
- prepare final Manager-review checklists for #196/#198/#199 and downstream work;
- inspect candidate/target overlaps before local integration;
- keep downstream packets exact but do not release dependency-gated production early;
- perform read-only source/lineage diagnosis for unexpected failures before opening a production repair claim;
- reconcile stale issue wording and dependency surfaces after durable evidence;
- prepare S1-D T-09 browser independent review when T-08 passes;
- maintain #91/#200/#201 status and live coordination.

## Hard boundaries

Do **not** from this lane:

- claim local worktree cleanliness or Unity-lock state without local evidence;
- edit or qualify production C#/Unity merely because GitHub write access exists;
- run or claim compiler, Editor, runtime, rendered or native-input evidence;
- weaken tests/thresholds/anchors to obtain green;
- merge high-risk production merely from worker self-assessment without the required Manager/review gates;
- create a Windows build.

### Explicit research-source restriction

The user previously instructed the Manager **not to act on** `Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md` / #194. Treat that work as informational context only unless the user explicitly lifts the restriction. Do not use this two-lane split as permission to resume source cleanup, deletion, bundling, Drive reorganisation or Project-source replacement.

## Coordination protocol with #201

For each material child dispatch/return:

1. #201 records execution facts and routing evidence on its orchestration claim/ledger and the child claim.
2. #200 reads the pushed evidence and performs any Manager/browser gate.
3. #200 updates live coordination and issues the next exact release/repin when authorised.
4. #201 does not self-authorise a product/scope/integration decision reserved to the Manager.
5. If #200 is temporarily unavailable, #201 may continue only dependency-independent work already authorised by existing packets; it must not infer approval.

## Handoff output expected from this chat

Every substantive browser review/decision should leave a durable GitHub Markdown record with:

- exact reviewed candidate/head;
- evidence inspected;
- verdict;
- what passed/failed;
- implication for the programme;
- next exact action;
- child claim/status;
- whether dashboard/status sync was completed;
- user action required or none.

## Difficulty / best-suited model

**Difficulty: Medium/High.** The work is mostly repository/browser management, but it spans several interacting specs and protected verification contracts. Best suited to the Game Dev Overall Manager / strong browser reasoning model with GitHub write access. It should not consume a local Unity worker for work that is safely browser-capable.

## Copy/paste dispatch line

`Take Browser / GitHub Manager Lane 03 under claim #200 using Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-HANDOFF.md. Refresh live active-tasks.md and current issues first. Own all browser-capable Manager review, GitHub inspection, packet/repin/claim/status work and active-tasks writes; do no local C#/Unity/runtime/build work. Coordinate with long-running orchestration #201, review every returned handoff promptly, keep protected gates and Reference Future/Legacy compatibility intact, and continue automatically when the next browser action is clear. Do not act on research-source handoff #194 unless the user explicitly lifts that restriction.`

**User action required:** open a second chat in this Game Dev project and give it the dispatch line above (or simply point it to claim #200 and this handoff path).
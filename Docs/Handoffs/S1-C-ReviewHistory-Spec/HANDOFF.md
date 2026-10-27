# HANDOFF — S1-C Scenario One Review / Management History Specification

**Exit statement: `S1-C REVIEW/HISTORY SPEC DRAFT READY FOR MANAGER REVIEW`**

## IMPLEMENTATION HANDOFF

- **Task:** Lane C — Scenario One Review / Management History specification (S1-C)
- **Worker:** Primary specification author (T3 Code / opencode-go mimo-v2.6-pro)
- **Role:** Primary (SPEC REQUIRED — specification only)
- **Branch:** `spec/scenario-one-review-history-v1`
- **Base:** `3b499aa459b92212f7bb06897836bae5914b5230`
- **Context commit:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` (as declared by the packet; canonical product docs read from the live set and checked against the packet's scope)
- **HEAD:** see delivery record at the end of this file
- **Packet:** `Docs/Handoffs/S1-C-ReviewHistory-Spec/TASK-PACKET.md` (packet commit `8177cf6f9df454bc25274bc4ada4942c251ec35b`)

### WHAT CHANGED

- Created `specs/scenario-one-review-history-v1/spec.md` (DRAFT): product intent, goals/non-goals, player workflows, mandatory L1–L4 content-layer separation, FR-1…FR-16, S1-D data contract, save/compatibility and write-back rules, AC-1…AC-11, stop conditions, OPEN-1…OPEN-4.
- Created `specs/scenario-one-review-history-v1/plan.md` (DRAFT): verified current-state map with file:line citations, proposed approach (extend Annual Review + read-only history query layer), rejected alternatives, integration boundaries, affected files, ownership/concurrency notes, migration/save implications, verification strategy, risks, implementation sequence.
- Created `specs/scenario-one-review-history-v1/tasks.md` (DRAFT): bounded tasks T-01…T-08, all TODO, not dispatchable until the spec is `APPROVED`.
- Created `specs/scenario-one-review-history-v1/checklists/requirements.md` (DRAFT): FR/AC/compatibility conformance checklist for the later implementation.
- Created this handoff.

### FILES CHANGED

- `specs/scenario-one-review-history-v1/spec.md` (new)
- `specs/scenario-one-review-history-v1/plan.md` (new)
- `specs/scenario-one-review-history-v1/tasks.md` (new)
- `specs/scenario-one-review-history-v1/checklists/requirements.md` (new)
- `Docs/Handoffs/S1-C-ReviewHistory-Spec/HANDOFF.md` (new)

### SERIALIZED UNITY FILES TOUCHED

None.

### PLAYER-FACING RESULT

None yet — specification only. Intended result (for the later implementation): after advancing time and reviewing the latest year, the player can look back through every completed year of management history, inspect recorded interventions (what/when/where/type/result/status), see recorded versus derived versus session-only content labelled honestly, and open a bounded "what earlier treatment matters now" view before the next decision.

### SIMULATION / SAVE IMPACT

None implemented. Specified save implications only (see "Save implications" below).

### ASSETS ADDED OR MODIFIED

None.

### MUST-NOT-CHANGE HASHES / ANCHORS

None changed. Spec explicitly preserves Reference Future v1 (D-042) and existing save-compatibility anchors; any future anchor movement from the additive save field is flagged as an explicit decision (G3), never a silent re-baseline.

### INTENTIONALLY CHANGED HASHES / ANCHORS

None.

### VERIFICATION RUN

Documentation/spec review only (packet verification clause):

- Draft checked against `Docs/Project/decision-log.md` (D-004, D-010, D-042, D-056, D-057, D-060, D-063, D-064, D-065), `Docs/Project/current-milestone.md` (S1-C PRE-DEMO; pre-demo sequence step 3), `Docs/Project/game-brief.md` (core loop; decades-long stand history), `Docs/Project/AgentWorkflow.md` (SDD lifecycle, spec authority, verification tiers).
- Implementation facts inspected read-only in the repository and recorded with file:line citations in `plan.md` §1: Annual Review (`UI/AnnualReviewView.cs`), Work Plan surfaces (`UI/WorkPlanView.cs`, `UI/WorkPlanOverview.cs`), work-order lifecycle (`ScenarioOne/ScenarioOneWorkOrder.cs`), management-event log (`ScenarioOne/ScenarioManagementEvent.cs`), year advancement (`ScenarioOneManager.AdvanceYear`), economy/history records (`ForestSaveData.cs` `ScenarioAnnualReport`/`ScenarioTimberSale`, `WorkEconomy/` transient ledger), objective evaluation (`ScenarioOne/ScenarioOneObjectives.cs`), save schema v20 and identity keys (`ForestSaveData.cs`, `ForestStartingStand.cs`, `StandGeometryModel.cs`), Reference Future behaviour (`ScenarioOne/ScenarioReferenceArchive.cs`, `Docs/ReferenceFutureContract.md`), session-only facts (`PlantedJuvenile.cs`, `ForestEcologyController.annualDbhGrowth`, `RegenerationDiagnosis.cs`).
- No Unity run; no source or asset change; no integration.

### VERIFICATION RESULTS

- Spec review: PASS (internal consistency; every FR traces to a verified gap or an accepted constraint; every open product question is marked OPEN rather than silently resolved).
- Repository-truth check: PASS (quoted field names/enums/line references verified against the tree at packet commit `8177cf6`; derivations feasibility checked against `ScenarioOneObjectives.Evaluate` inputs).
- Authority check: PASS (S1-C scope matches D-060 pre-demo requirement; no conflict with D-064/D-065 independence clauses; Reference Future and old-save constraints upheld).

### SPEC-DRIVEN HANDOFF (SPEC REQUIRED)

- **SPEC ID / STATUS:** SPEC-scenario-one-review-history-v1-V1 / **DRAFT** (not approved; confers no implementation authority)
- **TASKS COMPLETED:** Draft spec, plan, task list, requirements checklist, this handoff (the packet's full deliverable set).
- **SPEC DEVIATIONS:** None. The packet's deliverable list is complete. The four artifacts use the `specs/_template/` structure with additions the packet requires (S1-D contract, content layers, open-decision register).
- **UNRESOLVED REQUIREMENTS:** OPEN-1…OPEN-4 (below) await Manager/user decision; T-03/T-05/T-06 detail choices depend on them.
- **ACCEPTANCE CRITERIA TESTED:** Not applicable at draft stage; AC-1…AC-11 are written to be checkable against the later implementation (checklist prepared).
- **SPEC CONFORMANCE:** NOT VERIFIED (no implementation exists; conformance is a later gate).

### REMAINING VERIFICATION GATES

1. Manager review of the DRAFT → `APPROVED` (or `NEEDS CHANGES` with reasons).
2. Resolution of OPEN-1…OPEN-4 (or explicit deferral with an approved default).
3. Later implementation: T-01…T-08 per `tasks.md`, then the requirements checklist with evidence; G2/G3/G4 for the additive save field; Tier 2 feature qualification per `plan.md` §7.

### KNOWN LIMITATIONS

- Findings are static code reading at the packet commit; no runtime behaviour was exercised (this is consistent with a documentation-only packet).
- The draft deliberately leaves four product choices open rather than resolving them silently (packet instruction).
- S1-D itself is out of scope by packet; the contract in `spec.md` §9 is the handoff surface for that later work.

### UNRELATED WORK PRESERVED

- CCF-Primary working tree carries other agents' uncommitted work (`AGENTS.md`, `Docs/Project/coordination/active-tasks.md` modified; untracked `Docs/Operations/StorageArchitecture.md`, `Docs/ScenarioTwoPreProductionAudit.md`) — untouched. All work was done in a separate worktree `/media/jer/ZX20/CCF-Spec-ReviewHistory` on the packet's branch; no other lane, branch or worktree was modified.

### DECISIONS NEEDED

See "Open decisions" below. Also: Manager decision on whether the additive `objectiveAchievements` schema (FR-10, `spec.md` §11) is accepted with the conventional v21 bump.

### WORKER ASSESSMENT

- **Ready for review: Yes** — the draft answers the packet's product questions against verified repository truth, marks genuine product questions as OPEN, and defines the S1-D data contract without implementing anything.

### DASHBOARD SYNC

- **DASHBOARD SYNC: NOT AVAILABLE**
- **PUBLISHED DASHBOARD REFLECTS THIS TASK: NO**
- **DASHBOARD SYNC FOLLOW-UP:** No dashboard publisher/CLI is available in this session and the authoritative coordination record (`Docs/Project/coordination/active-tasks.md`) is the Manager's single-writer file on `ops/workflow-foundation-v1`. The Manager should record this Lane C handoff there and run the dashboard ingestion/publication step per AgentWorkflow; this worker did not edit generated dashboard state.

### RECOMMENDED NEXT STEP

Manager reviews the DRAFT (`specs/scenario-one-review-history-v1/spec.md`), decides OPEN-1…OPEN-4, marks the spec `APPROVED` (or returns changes), then dispatches T-01… per `tasks.md` through ordinary packets.

---

## Implementation facts discovered (summary)

Full detail with citations is in `plan.md` §1. The load-bearing facts for review:

1. **The history record layer already exists and is durable:** append-only `managementEvents` (all plan/execute/cancel/purchase/year/outcome events with year, targets, money deltas and `ecologicalTreatment`), permanently retained `workOrders` (incl. `createdYear`/`resolvedYear`), per-year `annualReports`, per-year `ecologicalSnapshots`, `stormEvents` (exact D-050 shape), `deadwoodRecords` (`fallenYear`), per-tree history fields, `outcomeYear`, and the once-built `centuryReview`. Save schema v20.
2. **The player cannot browse it:** `AnnualReviewView` renders only the newest report (plus 12-year cash/canopy trend strips); there is no multi-year or intervention-history surface.
3. **Precision limits exist and are currently unlabelled in play:** grouped-harvest settlement lands on one `WorkResolved` record (`SettleHarvestJob` overwrite) and `harvestJobId` is a resolution-year label; storm victim counts are reconstructed from live records; last-year browse assessments and "DBH growth last year" are session-only and vanish after load; mark apply/remove times are not recorded (current `markType` only); cancelled orders survive only as events; objective results have no achieved year.
4. **Objective achievement years can be recorded at first achievement cheaply**, and can be *derived with labels* for pre-feature saves from year-tagged snapshots/events (`ScenarioOneObjectives.Evaluate` is a pure function of saved inputs).
5. **Reference Future v1 review behaviour is a separate, frozen surface** (read-only milestone preview + century comparison semantics frozen-vs-aspirational) and must remain untouched; its archive integrity depends on original embedded JSON hashes.

## Proposed boundaries

- Extend the existing Annual Review; add a read-only history query layer; no second history store, no map-history cartography, no analytics product (FR-15).
- May touch `UI/AnnualReviewView.cs`, a new read-only `ScenarioOne/ScenarioManagementHistory.cs`, and (only if FR-10 is approved) one additive save field plus `EvaluateProgress` recording.
- Must not touch ecology/economy rules, objective definitions, Reference Future archive/preview semantics, existing record shapes, or the approved professional map.
- S1-D contract (stable IDs + append-only read surfaces) is fixed in `spec.md` §9 so later intervention work cannot invent competing identity schemes.

## Open decisions (`spec.md` §15, repeated for triage)

| ID | Narrow question | Recommendation |
|---|---|---|
| OPEN-1 | Pre-feature saves: derive labelled objective-achievement years from recorded per-year records, or show "not recorded"? | Derive + mandatory label |
| OPEN-2 | Persist session-only "what happened last year" facts (browse, DBH growth), or label them session-only? | Label; revisit after playtest |
| OPEN-3 | Intervention detail default: aggregate-first with drill-down to recorded targets, or always list every target? | Aggregate-first with drill-down |
| OPEN-4 | Is a recorded trail of decisions (mark apply/remove, retain-here) required for S1-C, or is the commitment trail sufficient? | Commitment trail sufficient; keep an extension point for S1-D / Multi-Scale |

## Save implications

- Default path: **no schema change** — the whole history requirement is served by records that already persist at v20.
- If FR-10's recorded-achievement path is approved: additive `ScenarioOneSaveData.objectiveAchievements` list `{objectiveId, achievedYear, targetValueAtAchievement}`, appended once per objective at first achievement; missing/empty = "not recorded"; no retrospective writes; conventional save-version bump to v21; v20 and older unchanged; Reference Future v1 frozen (`saveVersion` 12) and never migrated (D-042).

## Why the draft is ready for Manager review

- Every requirement traces to either a verified implementation gap (cited) or an accepted constraint (decision log / packet / milestone).
- Genuine product questions are isolated as OPEN-1…OPEN-4 with narrow statements and recommendations, so approval can be quick and explicit.
- Compatibility constraints (Reference Future v1, old saves, append-only history, no causal over-claim) are written as checkable rules with acceptance criteria.
- The S1-D data contract is specified now, so later repeated-intervention work can proceed without rediscovering identity/persistence rules.

## Delivery record

- Worktree: `/media/jer/ZX20/CCF-Spec-ReviewHistory`
- Branch: `spec/scenario-one-review-history-v1` (from packet commit `8177cf6f9df454bc25274bc4ada4942c251ec35b`)
- Delivered HEAD: the commit that contains this handoff and the four spec artifacts (branch tip at delivery).

---

# MANAGER-AMENDMENT ADDENDUM

**Exit statement: `S1-C REVIEW/HISTORY SPEC APPROVED — READY FOR IMPLEMENTATION PACKETS`**

- **Addendum date:** 2026-10-27
- **Worker:** same S1-C specification lane (Primary specification author)
- **Task class:** specification amendment only (packet `Docs/Handoffs/S1-C-ReviewHistory-Spec/AMENDMENT-PACKET.md`)
- **Manager review commit:** `65d354a6654b7ffd25b73cb04c9691f4aea127ea` (verdict `NEEDS CHANGES — NARROW MANAGER AMENDMENT ONLY`; reviewed worker HEAD `9b96f57`)
- **Amendment packet commit:** `b7193a5`
- **Exact amended/approved spec HEAD:** `1b1bef7f23104eedcc6a1b009bca2a4e93f34492` (`specs: approve S1-C review/history spec (R-1–R-4; save-version sequencing)`)
- **Published branch HEAD:** the commit containing this addendum, pushed as the branch tip of `spec/scenario-one-review-history-v1` (see publication record at the end of this addendum)

## Decisions applied

| Register | Manager decision | Applied as |
|---|---|---|
| R-1 (was OPEN-1) | Labelled derived achievement year **only** where exact first-achievement is recomputable solely from persisted year-tagged L1 records + accepted objective logic; otherwise "not recorded"; never approximate-as-fact | `spec.md` FR-4/FR-10/§11/AC-4/§15 R-1; `plan.md` §2.5, rejected-alternative note, §6, §8; `tasks.md` T-05; checklist FR-4/FR-10/AC-4 |
| R-2 (was OPEN-2) | Session-only browse / DBH-growth facts are **not persisted** in S1-C; labelled session-only after load | `spec.md` FR-13/§15 R-2; `tasks.md` T-04 exclusions; checklist FR-13 |
| R-3 (was OPEN-3) | Aggregate-first with drill-down to recorded targets | `spec.md` FR-8/§15 R-3; `tasks.md` T-03; checklist FR-8 |
| R-4 (was OPEN-4) | Existing commitment/result trail is sufficient; mark/retain decision history is a future extension point (S1-D / Multi-Scale), not an S1-C requirement | `spec.md` §9 consumer rules, §15 R-4 |

## Save-version correction applied

All fixed `v21` claims removed from the S1-C package. Required rule now stated in `spec.md` §11, `plan.md` §3/§6/§8 and `tasks.md` T-05: inspect the actual integration base immediately before the objective-achievement persistence change; use the **next available save-schema version**; never reuse a version owned by an accepted/qualified concurrent feature. Rack Planning & Field Marking V2 currently reserves/implements **v21**, so if Rack V2 integrates first, S1-C is expected to use **v22**. Old-save defaulting and frozen Reference Future v1 are preserved exactly; no retrospective write into old saves. T-05 requires recording the integration-base version check in its implementation handoff.

## Status change

- `spec.md` → **APPROVED** (implementation authority for this scope).
- `plan.md`, `tasks.md`, `checklists/requirements.md` → reference the approved spec state; decisions R-1…R-4 binding.
- T-01…T-08 retained as implementation tasks, all still **TODO**; no implementation task marked complete.
- `spec.md` §15 retained as a resolved decision register for traceability (per the amendment packet).

## Files changed by this amendment

- `specs/scenario-one-review-history-v1/spec.md` (status APPROVED; R-1…R-4; save-version sequencing)
- `specs/scenario-one-review-history-v1/plan.md` (approved status; boundaries/§6/§8/§9 updated)
- `specs/scenario-one-review-history-v1/tasks.md` (OPEN dependencies removed; T-05 version-check requirement)
- `specs/scenario-one-review-history-v1/checklists/requirements.md` (approved status; R-1 conditional old-save rule; save-version check)
- `Docs/Handoffs/S1-C-ReviewHistory-Spec/HANDOFF.md` (this addendum)

## Production / Unity confirmation

No production code, Unity asset, scene, prefab, package, ProjectSettings or save file changed. The amendment touched documentation only. Branch remains a clean descendant of the reviewed S1-C branch (`8177cf6` packet → `9b96f57` draft package → `65d354a` Manager review → `b7193a5` amendment packet → `1b1bef7` approved spec → this addendum).

## Verification performed (documentation-only)

- No remaining unresolved OPEN-1…OPEN-4 dependency in the task graph (T-01…T-08 dependencies and summary table checked; only the resolved register mentions the former IDs).
- No remaining unconditional/fixed S1-C `v21` bump (remaining `v21` mentions are the Rack V2 collision warning and the "Rack V2 reserves v21" sequencing rule).
- No contradiction found between `spec.md`, `plan.md`, `tasks.md` and the checklist (R-1…R-4 and the save-version rule applied consistently).
- No production/source/Unity/save file changed (`git status`: only the four spec artifacts and this handoff).
- Branch is a clean descendant of the reviewed S1-C branch (fast-forward lineage only).

## Publication record

- Amended/approved spec HEAD: `1b1bef7f23104eedcc6a1b009bca2a4e93f34492`
- Published to `origin/spec/scenario-one-review-history-v1` (see push record of this session; resulting branch HEAD is the addendum commit).

## Next step

No further product review is required unless implementation exposes a new material ambiguity. The Manager may integrate the approved specification and dispatch T-01…T-08 through ordinary implementation packets.

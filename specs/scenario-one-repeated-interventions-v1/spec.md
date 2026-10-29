# Scenario One Second and Subsequent Interventions (S1-D) Specification

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** APPROVED — Overall Manager decisions recorded in `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/MANAGER-REVIEW.md` (review commit `457461b9b92da2e96101f5b0eb9940b4e92fec0a`), applied by `AMENDMENT-PACKET.md` (`771620113487894bd8e09c69309318cc603ae063`), and amended by D-066 / browser claim #136 on 2026-10-29. Was `DRAFT` at `666cb67cfc3e17fc22df5607ac4043c8fbd7ee7d`.
- **Implementation authority:** task-level, within scope and within each task's stated dependencies only (`tasks.md`).
- **Owner:** Overall Manager (ChatGPT Game Dev)
- **Created:** 2026-10-27
- **Last updated:** 2026-10-29
- **Decision log references:** D-004, D-008, D-009, D-010 (amended in part by D-065), D-013, D-014, D-042, D-056, D-060, D-063, D-064, D-065, **D-066**; resolved decisions RD-1…RD-12 (§12)
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness; this is the **S1-D** pre-demo item (D-060)
- **Related evidence:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/HANDOFF.md`, `MANAGER-REVIEW.md`; S1-C spec `specs/scenario-one-review-history-v1/`; `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`; `specs/multiscale-forestry-management-v1/`; `Docs/Handoffs/S1-D-T04-Persistence-Amendment-20261029/BROWSER-RESULT.md`
- **Supersedes / superseded by:** D-066 supersedes only the earlier S1-D V1 statements that prohibited every new persisted field; the broad T-07 cycle-state expansion remains cancelled.
- **Packet:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/TASK-PACKET.md` — base `3b499aa459b92212f7bb06897836bae5914b5230`, context `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
- **Task class:** SPEC REQUIRED. This package is specification only: no production code, Unity edit, coefficient or Windows build.

## How to read this spec

Every requirement carries an **Authority** tag:

- `[D-nnn]`, `[Milestone]`, `[Brief]` — settled by accepted project authority;
- `[RD-n]` — settled by an Overall Manager decision recorded in `MANAGER-REVIEW.md` and §12;
- `[CODE c6342f7]` — a historical inspected implementation fact (see §3 for its limits);
- `[SPEC]` — a requirement introduced by this spec and approved with it.

D-066 is later authority for the narrow persistence amendment described in §§10.B, 10.F, 14 and 18. It does not reopen other settled scope.

## 1. Product intent

The player must be able to repeat

`assess → decide/mark/plan → Work Plan → execute → advance time → observe/review → decide again`

without resetting the scenario, fabricating a new forest, or treating the first intervention as a one-shot tutorial that blocks later management. This is the accepted loop `decide → advance time → walk → observe → understand why → decide again` [D-004] applied repeatedly, and it is a **pre-demo requirement** because the first external demo contains the complete Scenario One, in which the player makes second and later interventions, repeats the management cycle and reaches completion [D-060, Milestone].

## 2. Problem

Accepted authority states the repeated-intervention requirement (D-060). Inspection showed that the underlying management actions repeat, but first-cycle teaching assumptions can still block or regress re-entry. The later T-04 attempt then exposed one additional fact: the release/review teaching boundary is not faithfully reconstructible from existing persisted records in every legitimate state. The user therefore accepted D-066: preserve full FR-B1/AC-3 by adding the **smallest durable aggregate teaching-sample/review-complete record**, without persisting individual `[J]/[K]` choices or creating a broad cycle-state blob.

## 3. Inspection basis — read this before relying on any finding

The original spec inspection used the historical Scenario One integration lineage at `c6342f79a3e89c040fadd232017a937132779385`. Those findings remain provenance for F-1…F-12, not current implementation truth.

The D-066 browser amendment separately refreshed the live programme on 2026-10-29. The exact post-MFM-07 Unity-qualified production umbrella was `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, whose `ForestSaveData.CurrentVersion` is **23**. No active production save/schema writer owned the required surface when browser claim #136 inspected it; PARITY-08 #126 was blocked with no ownership. This establishes an **expected** next version of v24 for the bounded prerequisite only if the local worker's task-start refresh still confirms v23 and no competing writer. It is not permission to reuse v24 after the lineage changes.

Implementation facts must be re-verified at the actual implementation base before production edits. Browser/GitHub inspection does not prove Unity compilation or runtime behaviour.

## 4. Verified first-cycle assumption register

Class key: **B** can block Approve/Advance or a management action; **R** can regress or mislead progression state; **G** design gap; **I** informational dependency.

| ID | Cls | Historical finding | Treatment |
|---|---|---|---|
| F-1 | **B** | `LearningObjectivesView.RackPlanProblem()` made the teaching-footprint cap a lifetime Approve/Advance block from cumulative historical removal. | RD-5 / FR-A3: hard only while its owning activity is active; advisory afterwards. |
| F-2 | R | The guide could regress when current Crop Tree count or release-share facts changed. | RD-7 + D-066 / FR-B1. |
| F-3 | R | Later in-footprint felling could reopen the release/review lesson from mutable current facts. | RD-7 + D-066 / FR-B1. |
| F-4 | R | `ScenarioOneRackReview` `[J]/[K]` decisions were session-only; loading could return the guide to the review lesson. | D-066: keep individual `[J]/[K]` non-persistent; persist only minimum aggregate sample/review completion history. |
| F-5 | G | Only the first Annual Review acknowledgement gates play. | RD-3: later acknowledgement advisory only. |
| F-6 | G | Readiness existed as text only. | RD-4: derived component/reason advisory state. |
| F-7 | G | Marks carry no designation time; orders/events do carry years/IDs. | RD-6: derive previous/proposed presentation; no designation-year field or dated map layer. |
| F-8 | R | Learning practice steps use device-wide `PlayerPrefs`. | RD-7: never progression/cycle authority after historical completion. |
| F-9 | I | Completion chronology assumes unique positive management-event IDs; old validation did not enforce that. | RD-12 / FR-E3: reject ambiguity before mutation. |
| F-10 | I | Economy already distinguishes previous thinning history. | Unchanged. |
| F-11 | I | `Completed` does not stop management before the existing horizon; `Failed` is terminal. | RD-8 / FR-E4. |
| F-12 | I | Rack V2 persistence and Objective 1 teaching are separate concerns. | RD-9 / FR-G2. |

### 4.1 Already repeatable baseline behaviour

- Marking, Work Plan, pruning, planting, clearance and salvage create ordinary orders with no intended cycle-count lock.
- Open orders may persist and be revalidated; nothing is automatically approved or cancelled.
- Intervention history is derived from persisted management records.
- Advance-without-approved-work remains possible after warning.

These are implementation findings to recheck, not product decisions.

## 5. Definitions

- **Management cycle** `[SPEC]`: one pass of the §1 loop. A cycle may legitimately end with **no** intervention.
- **Intervention (cycle level)**: a resolved work plan / accepted grouping exposed by the integrated S1-C history contract; order-level records are its drill-down.
- **Previous work** `[SPEC]`: executed/resolved work with year and outcome.
- **Current commitment** `[SPEC]`: approved orders awaiting the next advance.
- **Proposed work** `[SPEC]`: pending orders and Fell marks not yet in the Work Plan. Crop Tree designations are standing designations, not work proposals.
- **Re-entry** `[SPEC]`: beginning any cycle after the first in the same save, including after save/load and idle years.
- **Readiness** `[RD-4]`: advisory derived state reporting components and reasons; never a gate or timetable.
- **Teaching-cycle key** `[D-066]`: the stable persisted key used to associate one aggregate teaching-sample/review-completion record with the management/teaching cycle it describes. The implementation may choose the concrete representation, but it must be deterministic, unique within the save, and validation-safe.
- **Aggregate teaching-history record** `[D-066]`: append-only/monotone historical evidence containing only sample/review completion and the minimum aggregate counters/outcomes required to distinguish incomplete from completed teaching progression. It is not a snapshot of current advice, marks, tree choices or a whole management-cycle state.

## 6. Goals

1. A player who has completed the first intervention can re-enter the cycle in the same forest until the scenario's management horizon.
2. The player can tell previous work from current proposals and review earlier treatment through the S1-C contract.
3. Completion remains reachable and understandable after repeated cycles.
4. Current recommendation/readiness state remains derived from authoritative records; **only the D-066 minimum aggregate teaching-completion history is newly persisted**.
5. Management after the first guided sequence stays player-directed, with advisory guidance only.
6. Save/load cannot erase a legitimately completed teaching sample/review, and cannot fabricate one for a pre-amendment save.

## 7. Non-goals

- Production code, Unity or scene/prefab edits in this specification/browser amendment.
- New ecology, economy, objective-target or calibration coefficients; changing the eight objective predicates or D-057 targets.
- A fixed intervention interval, minimum observation period or correct second treatment.
- A new prescriptive objective for every cycle.
- **Broad persisted management-cycle/readiness state** such as `ScenarioOneManagementCycleState`; T-07 remains cancelled.
- Persisting individual `[J]/[K]` choices, inspected-tree choices, `PlayerPrefs` practice credits or Crop Tree designation year.
- A dated past-work map layer.
- A second intervention/history store, competing identity scheme or duplicated S1-C history UI.
- Rack reopening/additional-rack authoring, a second MFM prescription, Scenario Two or sandbox work.

## 8. Player workflows

### 8.1 Repeat cycle (target flow)

| Step | Player does | S1-D concern |
|---|---|---|
| 1. Assess | Walk, inspect, map layers, read history | Previous-versus-new clarity |
| 2. Decide/mark/plan | Fell/Crop marks, nursery, planting, pruning, clearance | Not lesson-gated |
| 3. Work Plan | Review cost/residual stand, choose executor/material, approve | Teaching cap cannot become a lifetime gate |
| 4. Execute and advance | Advance one year; approved work resolves | Idle advance remains legitimate |
| 5. Observe/review | Annual Review, history, walk changed forest | Later acknowledgement advisory |
| 6. Decide again | Return to step 1 | Guide must be monotone after durable completion |

### 8.2 Entry conditions for the next cycle

The next cycle is available as soon as the advance completes while the scenario remains manageable under existing terminal/horizon rules. No recurring acknowledgement, observation interval or cleared-order requirement is introduced [RD-3, RD-4].

## 9. Interface to S1-C (Review / Management History)

S1-D consumes the S1-C history capability; it does not define a competing intervention-history authority.

### 9.1 Accepted S1-C boundaries S1-D relies on

1. One read-only history capability over authoritative records.
2. Stable IDs and append-only/read-only history surfaces are compatibility contracts.
3. Recorded fact, current observed state, derived comparison and explanatory teaching remain distinguishable.
4. Aggregate-first intervention detail with drill-down; no default per-tree dumps.
5. Session-only facts remain session-only and are labelled after load.
6. Pre-feature historical dates are shown only when reconstruction is evidence-complete; otherwise `not recorded`.
7. Full mark/retain decision provenance is not required.

### 9.2 Consumed interfaces

S1-D uses stable tree/order/event/juvenile/deadwood identity; persisted `managementEvents`, `workOrders`, annual reports, ecological snapshots, storm/deadwood records and outcome fields; the S1-C year-browsable review/history surface; Annual Review acknowledgements; and existing completion/next-step presentation. S1-D must not rewrite those records merely to cache advisory state.

### 9.3 D-066 boundary against S1-C

The D-066 aggregate teaching-history record is permitted because the T-04 review found that the release/review teaching boundary is not faithfully reconstructible from the existing S1-C/intervention records in all legitimate states. The new record therefore stores only the missing **teaching completion evidence**. It must not duplicate intervention details already held by S1-C, and must not become a second general history UI/store.

### 9.4 Cycle-state adapter

S1-D may add one small read-only, `UnityEngine`-free cycle-state adapter over authoritative records. It holds no advisory/readiness cache. The D-066 teaching-history record is separate historical completion evidence and does not authorise persistence of the adapter's current recommendation state.

## 10. Functional requirements

Priority words follow RFC 2119.

### A. Re-entry and action availability

- **FR-A1** After the first resolved intervention the player MUST be able to begin further cycles in the same save without scenario reset, forest regeneration, or loss of earlier marks/orders/events/reports/snapshots/deadwood records. `[D-060][D-004]`
- **FR-A2** Management actions available in the first cycle MUST remain available in later manageable years and MUST NOT depend on lesson, `PlayerPrefs` or session-only state. Rack reopening/additional-rack authoring remain Rack V2 concerns. `[D-060][RD-9]`
- **FR-A3** The teaching-footprint safeguard MUST remain a hard constraint only while the lesson/sample activity that owns it is active. Once the **durable D-066 teaching-history completion condition** for that activity is established, exceeding the historical footprint threshold MAY be advisory but MUST NOT disable Approve Work Plan or Advance. This does not weaken an MFM-owned prescription ceiling. `[RD-5][D-066]`

### B. Progression state

- **FR-B1** The guide / active-objective state MUST be **monotone** across legitimate later management and save/load. Current/recomputable facts MAY remain derived from authoritative history/world state, but the release/review teaching boundary that cannot be faithfully reconstructed MUST use the D-066 aggregate teaching-history record. At minimum, for each applicable teaching/management cycle, the durable record MUST preserve: (a) a stable cycle key; (b) whether the teaching sample is complete; (c) whether the required review is complete; and (d) only the aggregate counters/outcomes needed to decide incomplete versus completed progression. Once a record establishes completion it MUST NOT regress. Repeated reads or repeated calls in the same state MUST NOT consume an additional sample. Before completion is recorded, existing in-lesson behaviour remains. `[RD-7][D-066]`
- **FR-B2** Stable saved IDs and lesson/objective/event/order identities MUST NOT be renamed or reused; new states/lessons use new IDs. Visible lesson numbering is not persistent identity.
- **FR-B3** Individual `[J]/[K]` inspection choices, inspected-tree selections and device-wide `PlayerPrefs` MUST NOT be progression/cycle authority after historical completion and MUST NOT be persisted by S1-D. `[RD-7][D-066]`

### C. Previous work versus current proposals

- **FR-C1** Work presentation MUST distinguish Executed, Approved, Pending, Pending carried over, and Fell marks not yet in the Work Plan, derived from existing orders/events/history.
- **FR-C2** Carried-over orders MUST be revalidated against the present forest before approval and surfaced when stale; nothing is auto-cancelled or auto-approved.
- **FR-C3** Executed work MUST NOT be presented as a current proposal; failed tasks remain visible with reason.
- **FR-C4** Where persisted records lack a designation time, the UI MUST NOT state or imply one.
- **FR-C5** S1-D V1 MUST NOT add Crop Tree designation-year persistence or a dated past-work map layer.

### D. Cycle state, readiness and review

- **FR-D1** The game MUST provide an explainable **advisory** cycle state from authoritative records, reporting components/reasons rather than one prescriptive yes/no. It MUST distinguish approved work awaiting execution, pending/carried-over work, stale/invalid orders, whether latest resolved intervention has been reviewed, and years since latest intervention as descriptive context.
- **FR-D2** Readiness MUST NEVER be a gate or timetable. No minimum observation interval is introduced; wait-and-observe remains valid.
- **FR-D3** After the first advance, outstanding Annual Review acknowledgement is advisory and MUST NOT block Approve/Advance. The protected completion chronology may still require reviewed cycles.
- **FR-D4** Later interventions remain ordinary player-directed management with advisory guidance and existing global objectives; no prescriptive objective per cycle.

### E. Completion and validation

- **FR-E1** Completion MUST remain reachable after repeated cycles; no transient teaching-only state may be required after durable historical completion is recorded.
- **FR-E2** Protected behaviour in §10.1 MUST NOT change.
- **FR-E3** A loaded save with duplicate/non-positive management-event IDs MUST be rejected **before any world or disk mutation** with clear error; no silent repair/renumbering/reinterpretation. The same pre-mutation principle applies to malformed or ambiguous D-066 aggregate teaching-history data in the schema version that owns it. If a shipped/accepted fixture is ambiguous, STOP and return to the Manager.
- **FR-E4** The protected minimum of two reviewed successful intervention cycles in distinct years remains unchanged; continued management after `Completed` is accepted until existing terminal/horizon conditions.

### F. Save and history re-entry

- **FR-F1** After save/load, history, current advisory cycle state, previous/proposed classification and next-step guidance MUST reproduce correctly. Reading/displaying them MUST NOT write fields. The D-066 aggregate teaching-history record changes only when authoritative teaching sample/review completion is recorded, not when UI is read.
- **FR-F2** S1-D V1 MAY add **only** the D-066 minimum aggregate teaching-history persistence slice. It MUST NOT add a broad mutable management-cycle/readiness state blob. The implementation MUST use the next available schema version on the actual integration base and MUST NOT reuse a version owned by another feature. On the browser-inspected post-MFM-07 base the expected next version is v24 because `CurrentVersion = 23`; this expectation is void if live inspection changes. T-07 remains cancelled.
- **FR-F3** Reference Future v1, Legacy40 anchors, pre-v20 compatibility and MFM-06/07 prescription/provenance compatibility MUST remain unchanged.
- **FR-F4** For pre-amendment saves with no D-066 record, absence means **not recorded / incomplete for the durable teaching latch**. The loader MUST NOT fabricate completed teaching samples from `PlayerPrefs`, session state, current marks/geometry, existing intervention events or heuristic reconstruction.
- **FR-F5** For the new owning schema version, malformed or ambiguous aggregate teaching-history data MUST be rejected before mutation and MUST NOT be silently repaired. Valid records MUST survive save → load → continue exactly and monotonically.

### G. Neighbouring work and S1-C

- **FR-G1** MFM-generated Work Plan orders are ordinary orders. S1-D contains no second-prescription logic.
- **FR-G2** Objective 1 rack teaching MUST NOT automatically repeat in every cycle; rack reopening/additional-rack authoring remain Rack V2 decisions.
- **FR-G3** S1-D MUST NOT assume Sitka-only or add species coefficients.
- **FR-G4** S1-D consumes the S1-C history contract and MUST NOT create a competing intervention-history store, identity scheme or duplicate history UI. The D-066 aggregate record is narrowly teaching-completion evidence only.

### H. Compatibility

- **FR-H1** Geometry-independent derived presentation/re-entry features MAY work for Legacy40 where required records exist. Enlarged80-only rack/sample/curriculum features MUST NOT be migrated into Legacy40. Reference Future v1 remains frozen.
- **FR-H2** Pre-amendment Enlarged80 saves MUST load without fabricated teaching completion, history, cycle state or provenance.

### 10.1 Protected behaviour

Unchanged by D-066:

- `ScenarioOneCompletionRules` normalised source and its pinned N3 hash;
- eight objective predicates/targets and D-057 geometry-aware effective targets;
- economy/ecology coefficients, work-economy values and thinning-tier schedules;
- Reference Future v1 and Legacy40/Enlarged80 protected anchors;
- no Windows build before user playtest authorisation;
- no fixed intervention interval or automatic treatment choice;
- MFM-06/07 prescription/provenance semantics and save round-trip contract.

## 11. Authority / source of truth

Accepted product intent: Decision Log through D-066, Current Milestone, Game Brief and the approved S1-D spec package. Implementation truth remains the exact inspected repository/Unity state. Supporting handoffs and candidate commits such as `b9d204c` are evidence only until integrated and qualified.

## 12. Resolved decision register

The original Manager decisions RD-1…RD-12 remain in force except where D-066 narrowly supersedes the no-new-persistence portion of RD-7.

| Id | Topic | Decision / current effect |
|---|---|---|
| RD-1 | Candidate-lineage baseline | Historical derived history, protected two-cycle rule, completion guidance and guide architecture were partial implementation baseline only; no feature-completion inference. |
| RD-2 | Later-cycle mode | Bounded hybrid; later interventions are player-directed with advisory guidance. |
| RD-3 | Recurring Annual Review acknowledgement | Advisory, not a recurring hard gate. |
| RD-4 | Readiness | Advisory derived components/reasons; never a gate/timetable. |
| RD-5 | Teaching-footprint cap | Hard only during owning activity; advisory/non-blocking afterwards. |
| RD-6 | Previous/new provenance | Derived from existing orders/events/history; no designation-year field or dated past-work map layer. |
| RD-7, narrowed by D-066 | Guide/progression authority | Current/recomputable guide facts remain derived where faithful; individual `[J]/[K]` and `PlayerPrefs` are not progression authority. **D-066 supersedes only the blanket prohibition on a new save field and authorises the minimum aggregate teaching-completion record required by FR-B1/AC-3.** |
| RD-8 | Completion/post-completion | Two reviewed successful cycles minimum unchanged; management may continue after `Completed`. |
| RD-9 | Racks later | Rack reopening/additional racks deferred to Rack V2; Objective 1 teaching does not automatically repeat. |
| RD-10 | Legacy40 | Geometry-independent derived features may work where records exist; no Enlarged80-only migration; Reference frozen. |
| RD-11 | Second MFM prescription | Not part of S1-D V1. |
| RD-12 | Ambiguous management-event IDs | Reject before mutation; no silent repair/renumbering. |
| D-066 amendment | Teaching persistence | Persist only minimum aggregate sample/review completion history per stable teaching/management cycle; individual `[J]/[K]` non-persistent; T-07 broad state remains cancelled. |

## 13. Evidence classification

- **Accepted authority:** D-060 repeated-loop requirement; D-066 persistence amendment; RD-1…RD-12 except the narrowed RD-7 clause.
- **Implementation evidence:** exact repository/Unity state only; `b9d204c` is unqualified evidence, not authority.
- **Model derivation:** current readiness/history comparisons computed from authoritative records.
- **Gameplay calibration:** inherited stale-order thresholds, teaching release/cap values and two-cycle completion minimum; D-066 adds no ecological coefficient.
- **Simulation/product design:** the aggregate teaching-history persistence structure and old-save fallback are product/technical rules, not empirical forestry claims.

## 14. Save / backward compatibility

D-066 is narrowly additive.

1. **Current inspected lineage.** Post-MFM-07 qualified production umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5` uses save schema v23. The local prerequisite may use v24 only after task-start refresh confirms v23 remains current/free and no competing save writer exists.
2. **New persisted content.** Add only the aggregate teaching-history evidence required by FR-B1: stable cycle key, sample/review completion and minimum aggregate counters/outcomes needed for incomplete/completed determination. Concrete C# names are implementation details.
3. **No individual choices.** `[J]/[K]`, inspected-tree choices and `PlayerPrefs` remain non-persistent.
4. **No broad cycle blob.** T-07 remains cancelled; current readiness/recommendation state continues to recompute.
5. **Pre-amendment saves.** Missing field/list means no durable teaching completion recorded. Do not infer completion heuristically. Existing game progress remains loadable; the teaching guide may require the lesson again until new durable completion is earned/recorded.
6. **Validation.** New-version duplicate cycle keys, impossible completion combinations, negative/invalid aggregate counts or other ambiguity defined by the implementation contract must be rejected before world/disk mutation; no repair.
7. **Compatibility.** Preserve Reference Future v1, Legacy40, pre-v20 paths, objective-achievement history and MFM-06/07 prescription/provenance state. No protected archive/hash rebaseline.

## 15. Security / write-back

Single-player local save; no external trust boundary. Load validation is nevertheless a hard integrity boundary. Event-ID and D-066 malformed-data rejection must occur before world/disk mutation, with CanLoad/LoadData parity and no silent repair.

## 16. Acceptance criteria

- **AC-1** Three real cycles including idle years produce expected events/orders/reports/history and later management actions remain available.
- **AC-2** After durable completion of the owning teaching activity, an exceeded historical footprint threshold is advisory/non-blocking; during the activity the original hard safeguard remains.
- **AC-3** Once the D-066 aggregate record establishes teaching sample/review completion, the guide does not regress after (i) a fourth Crop Tree, (ii) Crop Tree loss, (iii) later in-footprint felling, (iv) executed-share drift, or (v) save/load at later lessons. Before durable completion existing in-lesson behaviour remains. **Repeated calls/reads in one state do not consume another sample. Individual `[J]/[K]` choices are not persisted.**
- **AC-4** The same valid save loaded twice yields identical intervention history, durable teaching aggregate, advisory cycle state and next-step text; read/display paths mutate nothing.
- **AC-5** Executed, approved, pending, carried-over-pending and unplanned Fell-mark work remain visibly distinct; no unrecorded designation time or dated map layer is invented.
- **AC-6** A scripted ≥3-cycle path completes and stays solvent; objective predicates, D-057 targets and N3 normalised-source hash remain unchanged.
- **AC-7** Duplicate/non-positive management-event IDs are rejected before mutation, with no repair.
- **AC-8** Reference Future v1, Legacy40/Enlarged80 protected anchors and existing protected fingerprints remain unchanged; the S1-D save diff is limited to the D-066 aggregate teaching-history slice plus required version/validation plumbing. MFM-06/07 save/provenance round-trip remains green.
- **AC-9** Readiness reports components/reasons, never blocks play or prescribes an interval; later unreviewed state does not disable Approve/Advance.
- **AC-10** Independent review finds no hidden automation or causal overclaim.
- **AC-11** There is one intervention/history authority. The D-066 teaching aggregate is not a competing general history store; the advisory cycle-state adapter is read-only and `UnityEngine`-free.
- **AC-12** Objective 1 rack teaching does not re-trigger automatically; no rack-reopening/additional-rack behaviour is added.
- **AC-13** Legacy40/pre-amendment saves preserve valid compatibility; no Enlarged80-only feature is migrated; Reference unaffected.
- **AC-14** Management can continue after `Completed` until existing terminal/horizon conditions.
- **AC-15** MFM-generated orders behave as ordinary orders; no second-prescription logic is introduced.
- **AC-16** A pre-amendment v23 (and representative earlier supported) save with no D-066 record loads successfully with durable teaching completion treated as `not recorded / incomplete`, not fabricated complete.
- **AC-17** Malformed/ambiguous new-version teaching-history records reject before mutation; CanLoad/LoadData agree; valid save → load → continue preserves completion monotonically.

## 17. Stop conditions

Return to the Manager rather than assuming if:

- the implementation baseline or live ownership cannot be established;
- another active writer owns save/schema/validator surfaces required by T-04P;
- `ForestSaveData.CurrentVersion` has advanced from 23 before T-04P starts, or expected v24 is otherwise consumed;
- the implementation would need persistence broader than D-066's aggregate teaching-history slice;
- a shipped/accepted save fixture has ambiguous management-event IDs or malformed teaching data that would require a new compatibility policy;
- a fix requires changing `ScenarioOneCompletionRules`, objective predicates/targets, economy/ecology values, protected reference archives/hashes, Rack V2 lifecycle or MFM prescription semantics;
- the integrated base contains competing intervention/history authorities or duplicated history UI;
- the teaching cap can trap a player indefinitely under the shared completion semantics;
- repeated cycles make completion unreachable/economically unrecoverable without coefficient change;
- the work would add species coefficients, a fixed intervention interval, automatic treatment choice or another authoritative management-cycle state machine.

## 18. D-066 amendment and execution order — 2026-10-29

D-066 preserves the full intent of FR-B1 / AC-3 rather than weakening it. It clarifies that the earlier wording "advisory persistence not required" applies to **current recommendation/readiness state**, which remains recomputable; it does not prohibit the deliberately accepted aggregate historical teaching-completion record.

The required order is:

1. browser claim #136 publishes the amendment and packet;
2. **T-04P / claim #143** implements and qualifies the minimum durable aggregate teaching-history persistence against the exact qualified post-MFM-07 production umbrella, with live schema/ownership refresh;
3. #143 is integrated before curriculum replay;
4. **#124 / T-04** rebases/replays the existing guide work against that integrated prerequisite; `b9d204c` remains unqualified evidence only;
5. #124 completes offline + Unity qualification and Manager review;
6. **PARITY-08 #126** is re-released only after that lineage is exact; it must not be a concurrent save writer;
7. mandatory independent **#128** review follows the #126 handoff.

No Windows build is authorised by this amendment.
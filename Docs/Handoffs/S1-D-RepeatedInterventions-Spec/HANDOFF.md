# S1-D — Scenario One Second and Subsequent Interventions — SPEC HANDOFF

- **Task:** S1-D Repeated-Interventions Specification (packet `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/TASK-PACKET.md`)
- **Class:** SPEC REQUIRED — specification only. No production code, Unity edit, coefficient or build.
- **Branch:** `spec/scenario-one-repeated-interventions-v1`
- **Packet commit:** `8eb7a996613ec4ec9b9ff7befe064796cc700100` (parent = base `3b499aa459b92212f7bb06897836bae5914b5230`)
- **Context lock:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`. Verified by `git diff`: the canonical context set (`Docs/Project/`, `AGENTS.md`, `CLAUDE.md`, `Tools/ProjectContext/`, `specs/`) is byte-identical between that commit and the packet tree I read; only the live-coordination file `active-tasks.md` differs, which is excluded from context by definition.
- **Spec status:** **`APPROVED`** — Manager review `457461b9b92da2e96101f5b0eb9940b4e92fec0a`; amendment packet `771620113487894bd8e09c69309318cc603ae063`; see §12 addendum.
- **Deliverables:** `specs/scenario-one-repeated-interventions-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}` and this file.

> **Reading this handoff.** §1–§11 are the original draft handoff, retained as the review record. Statements that a decision is `OPEN`, that approval is blocked, or that persistence is conditional are **superseded by §12** and by the resolved decision register in spec §12. For the avoidance of doubt, every `OPEN-n` reference in §1–§11 maps to the corresponding `RD-n` in §12, and **no item remains open**. §12 is authoritative for the amended, approved package.

## 1. Read these three things first

1. **The packet base does not contain the code being specified.** `main` at `3b499aa` has save v20 and no lesson curriculum (no `rack.approve`/`rack.result`, no sequential lessons, no management-history or completion-chronology code). That code is on the Scenario One integration lineage, which is not an ancestor of `main` (merge-base `c1564b8`). I inspected its current tip, `c6342f79a3e89c040fadd232017a937132779385` (save v21; the BASE of the registered Rack V2 lanes; RACK-V2-03B reports a Unity gate-chain PASS there). Every implementation fact in the spec is tagged `[CODE c6342f7]`: **read statically from Git, not run in Unity, not integrated into `main`**. Re-verify at the real implementation base (task T-01).
2. **Canonical documents and the candidate lineage disagree about whether S1-C/S1-D exist.** The Milestone (context `eb218ea`) says both are "not implemented". The candidate lineage already carries derived management history (`7a023eb`), a **reviewed two-cycle completion rule** in save v20 (`7840872`) whose normalised source is hash-pinned by the N3 harness, completion guidance (`b87b52e`), and a derived eight-lesson guide (`420fcb6`). Its own design note says "No new S1-C/S1-D development needed" and its Phase 7–9 record lists canonical updates "required (not made)". I did **not** resolve this. It is `OPEN-1`, and it conditions the whole plan.
3. **There is one genuine blocker for a second intervention.** `RackPlanProblem` disables Approve Work Plan and Advance whenever a cumulative 40 % trunk-area share of the teaching footprint is exceeded. It has no lesson-stage condition and counts all completed felling in that footprint across the whole game. It was introduced as a lesson safeguard ("interactive planning safeguard only") and works as a lifetime ceiling. Whether that is intended is `OPEN-5`. Everything else I found is regression risk or a design gap, not a hard block.

## 2. Method and limits

- Authority read in full: Decision Log, Current Milestone, Game Brief, Unity Project Overview, `specs/README.md`, the spec templates and the MFM spec. Read in the relevant parts: AgentWorkflow (authority, SDD, context, capability, safety, locks, gates, verification tiers, packet) and the Rack V2 spec (purpose/authority, confirm and edit, teaching flow, persistence, counters, protected behaviour, and the product decisions OPEN-1..3).
- Code read from Git objects at `c6342f7` via `git show`/`git grep`: curriculum and teaching sequence, rack-bay analysis and rack review, completion rules and guidance, intervention history and its text, the manager's review/outcome/advance/mark-import/approve paths, work-order and event records, save data and migration, Stand Map layers, the economy adapter's thinning tiers.
- **Not done:** no Unity, no compile, no gate run, no rendered check. No claim of runtime behaviour is made. Findings are static reading of source and comments.
- Not read in depth (so not asserted): Annual Review and Work Plan view internals beyond their gating lines, tree inspection, the Rack V2 spec's geometry, mouse-interaction and copy sections (§§5–16, §§20–33), and pruning constants (the lesson text alone is cited for lift limits).
- The S1-C branch `spec/scenario-one-review-history-v1` (`8177cf6`) contained only its task packet. The history interface (spec §9) is therefore inferred from that packet and the existing derived-history code, and **must be reconciled with the S1-C draft**.

## 3. Inspected first-cycle assumptions (summary; full register is spec §4)

| ID | Class | Assumption found |
|---|---|---|
| F-1 | **Blocker** | Teaching-footprint 40 % cumulative cap hard-blocks Approve and Advance for the whole game (UI layer) |
| F-2 | Regression | Guide returns to lesson 2 if the footprint has ≠ 3 valid Crop Trees; executed-release measure can drift below threshold over years |
| F-3 | Regression | Any later in-footprint felling re-opens lesson 5 |
| F-4 | Regression | Lesson 5 `[J]/[K]` review is session-only, so the guide returns to lesson 5 after every load |
| F-5 | Gap | Only the first Annual Review acknowledgement gates play; later ones are optional events the completion rule reads |
| F-6 | Gap | "Ready for next intervention" exists only as text hints |
| F-7 | Gap | Marks carry no time or provenance; Fell marks are consumed on import; no dated past-work map layer |
| F-8 | Regression | Learning practice steps live in device-wide `PlayerPrefs` |
| F-9 | Info | Completion needs all eight objectives plus two successful plans in distinct years with an acknowledgement between; monotone; assumes unique positive event IDs that no validator checks |
| F-10 | Info | Harvest cost tier is keyed to distinct felling years (First/Second/Later) |
| F-11 | Info | Completed does not stop management; Failed is terminal (including cash exactly 0); horizon year 100 |
| F-12 | Info | Rack V2 persistence exists at v21; reopening after work is Rack V2 spec §39.7 OPEN-3 |

**Verified already repeatable (no change needed):** Fell-mark → Work Plan → approve → advance; Crop Tree designation; nursery/planting/shelters; clearance; pruning; salvage; stale-order surfacing and revalidation; history derivation with no two-cycle special case; the unapproved-advance warning. Marking and Work Plan code has **no reference to lesson state**, so first-cycle assumptions are in the UI teaching layer, not the simulation.

## 4. Proposed repeated-cycle architecture (plan §2)

Derived-first, additive, no new persisted state by default:

1. Decouple teaching safeguards from Approve/Advance once their lesson is over (`OPEN-5`).
2. Make the guide monotone by latching on history-only facts, freezing sample facts, or (only if needed) a small persisted record (`OPEN-7`).
3. A pure, `UnityEngine`-free `ScenarioCycleState` beside `ScenarioInterventionHistory` that names where the player is in a cycle and why — advisory, never an instruction or an interval (`OPEN-3`, `OPEN-4`).
4. Previous-versus-proposed presentation using existing `createdYear`, status and the stale-order rule (`OPEN-6`).
5. A pre-mutation check for ambiguous management-event IDs (G5).
6. Persistence only if the Manager chooses it.

Not touched: the completion rule, objective predicates and targets, economy, ecology, Reference Future v1, rack runtime.

## 5. Dependencies

- **Review/History (S1-C):** S1-D relies on seven items (EI-1..EI-7, spec §9), all present as derived code at `c6342f7`. EI-5 (per-year acknowledgement) has no accessor yet; whichever spec lands first should own it.
- **Rack V2:** the Objective 1 workflow must not repeat per cycle. 03C owns the rack runtime and may edit lesson-sequence code; rack availability after Objective 1 is `OPEN-9` and Rack V2 spec §39.7 OPEN-3. Cross-lane files are out of bounds (**CROSS-LANE API REQUIRED**).
- **MFM (D-065):** its "Extend your treatment" lesson sits after Lesson 5 and edits the same curriculum files and `Current()` as S1-D guide work. MFM contributes ordinary pending orders; it does not satisfy S1-D. Recommended order: Rack V2 → S1-C → S1-D guide/gating → MFM insertion (Manager to confirm; plan §5).
- **Three-species parity:** later cycles may involve Oak/Beech; S1-D adds no species logic or coefficients.

## 6. Open product decisions (original draft — all resolved in §12)

All marked `OPEN — MANAGER/USER DECISION REQUIRED`; recommendations are non-binding. **Superseded:** every item below is resolved as RD-1…RD-11 in spec §12 and §12 of this handoff.

| ID | Narrow question | Blocking |
|---|---|---|
| OPEN-1 | Accept the candidate-lineage derived history, two-cycle rule and derived guide as the baseline? | **Yes** |
| OPEN-2 | Later cycles: free, objective-driven, or bounded hybrid? | No |
| OPEN-3 | Is a recurring Annual Review acknowledgement a gate or advisory? | No |
| OPEN-4 | What does "ready for next intervention" mean (advisory/gate/none; any interval)? | **Yes** (FR-D) |
| OPEN-5 | Does the 40 % footprint cap stay a hard block after the lesson? | No (blocks T-03) |
| OPEN-6 | Previous-vs-new for Crop Trees: derived only, or persisted designation year; dated map layer? | No (blocks T-05/T-07) |
| OPEN-7 | Guide stability: derive from history, or persist a record; persist `[J]/[K]`? | **Yes** (T-04) |
| OPEN-8 | Is two reviewed cycles the intended completion minimum; is continued management after Completed accepted? | No |
| OPEN-9 | May racks be planned or reopened in later cycles? | No (defer to Rack V2 spec §39.7 OPEN-3) |
| OPEN-10 | Do derived re-entry features apply to Legacy40? | No |
| OPEN-11 | Is a second MFM prescription part of S1-D? | No |

## 7. Save implications

**Superseded by §12:** S1-D V1 adds **no persisted field** and changes no schema version. The conditional options below were not selected.

- **Default: none.** History and cycle state are derived from records already saved (events, reports, snapshots, orders, deadwood, outcome). `CurrentVersion` is neither assumed nor changed.
- **Conditional (original draft — not selected):** a per-save progress record (`OPEN-7`) or a Crop Tree designation year (`OPEN-6`). Each would have needed the next free schema version on the actual integration base, a documented default, no fabricated state in old saves, the world-hash compatibility path, and G5.
- **One robustness risk:** a save with duplicate or non-positive management-event IDs loads (no validator checks them) but the completion chronology silently becomes empty, so the scenario can never complete. Proposed `FR-E3`; touches shared load validation, so G5.
- Legacy40, Reference Future v1 and pre-feature Enlarged80 saves: unaffected, no fabricated state.

## 8. Verification performed

Documentation/spec review only (as the packet specifies). Each proposed behaviour is tagged to accepted authority, an inspected code fact, or `[PROPOSED]`; product choices are `OPEN`. No test, compile, gate or Unity run was performed and none is claimed. The checklist verdict is **NOT VERIFIED** by design.

## 9. Repository and coordination record

- Work done in a dedicated worktree `/media/jer/Files/CCF/Operations/CCF-S1D-Spec` on the task branch, clean at the packet commit before editing. Location follows the existing non-Unity worktree convention and the storage policy (ZX20 is for Unity working sets); the Manager may relocate or remove it.
- `/media/jer/ZX20/CCF-Primary` (branch `ops/workflow-foundation-v1`) was **not modified**. It already carries unrelated uncommitted work that was preserved untouched: modified `AGENTS.md` and `Docs/Project/coordination/active-tasks.md`; untracked `Docs/Operations/StorageArchitecture.md` and `Docs/ScenarioTwoPreProductionAudit.md`.
- Git operations that touched the shared store: fetched `origin/spec/scenario-one-repeated-interventions-v1`, `origin/main` and `origin/spec/scenario-one-review-history-v1` (objects and remote-tracking refs only); created one worktree and one local branch tracking the remote task branch.
- **Live locks:** `origin/main` `active-tasks.md` at `3b499aa` has no row for S1-D or S1-C, so there was no ownership conflict. This lane writes only new paths (`specs/scenario-one-repeated-interventions-v1/`, `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/HANDOFF.md`). I did **not** edit `active-tasks.md` (Manager single-writer); the Manager may wish to register this lane.
- No Unity lock was needed or taken.

## 10. Canonical updates recommended (not made)

`decision-log.md`: record the `OPEN-1` outcome (status of the derived history, two-cycle rule and guide) and the "intervention = resolved work plan" presentation convention. `current-milestone.md`: correct the S1-C/S1-D "not implemented" wording to whatever `OPEN-1` decides. `unity-project-overview.md`: record verified candidate-lineage facts only after they are integrated and verified.

## 11. Readiness for Manager review (original draft — superseded by §12)

**Ready for review; not ready for approval** *(at the draft stage)*. The draft answers each question the packet lists or marks it `OPEN`:

| Packet question | Where |
|---|---|
| when the next cycle becomes available | spec §8.2, F-5, `OPEN-3`/`OPEN-4` |
| which lessons/objectives stay completed; which actions are reusable | §4 (R-1, R-2, F-2..F-4), FR-A2, FR-B1, `OPEN-7` |
| objective-driven, free or hybrid | `OPEN-2` |
| previous versus new marks/orders/results | FR-C1..C5, F-7, `OPEN-6` |
| what "ready" means | §5, FR-D, `OPEN-4` |
| completion reachable after repeated cycles | F-9, FR-E1..E4, `OPEN-8` |
| saved state and history for re-entry | §9, §14, FR-F |
| previous versus current proposed work for the player | FR-C |
| Legacy40 and Reference Future v1 compatibility | FR-H1/H2, FR-F3, `OPEN-10` |

*(Original draft stage: approval was blocked on `OPEN-1`, `OPEN-4` and `OPEN-7`. All are resolved in §12.)*

*(Original draft exit, superseded by §12.3.)* S1-D REPEATED-INTERVENTION SPEC DRAFT READY FOR MANAGER REVIEW

## 12. Amendment addendum — APPROVED

**Status:** **`APPROVED`** under the Overall Manager review `457461b9b92da2e96101f5b0eb9940b4e92fec0a` (`MANAGER-REVIEW.md`) and its amendment packet `771620113487894bd8e09c69309318cc603ae063` (`AMENDMENT-PACKET.md`). This addendum supersedes the draft-stage statements above. No production task was dispatched.

- **Reviewed worker HEAD:** `666cb67cfc3e17fc22df5607ac4043c8fbd7ee7d`
- **Amended package HEAD:** `AMENDED_HEAD_PLACEHOLDER` (contains the amended `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` and this addendum)
- **Returned HEAD:** the commit that records the amended HEAD SHA (child of the package HEAD; no other change)

### 12.1 Amendments applied

1. **Decisions resolved.** `OPEN-1`…`OPEN-11` are replaced by resolved decisions RD-1…RD-11, plus RD-12 for ambiguous management-event IDs, in spec §12. No blocking product decision remains.
2. **S1-C contract reconciled.** Spec §9 / EI-1…EI-7 now consume the accepted S1-C boundaries: one read-only history capability over existing authoritative records; stable IDs and append-only/read-only surfaces; recorded fact / current observed state / derived comparison / teaching kept distinguishable; aggregate-first intervention detail with drill-down; absent provenance not invented; session-only browse/DBH facts left session-only; labelled-derived objective dates only when evidence-complete, otherwise `not recorded`; full mark/retain provenance not required. S1-D may add only a small read-only, `UnityEngine`-free cycle-state adapter, never a competing history store or duplicated history UI.
3. **Tasks amended.** T-00 is **COMPLETE**; T-01 remains the **mandatory first execution task** on the actual integrated Rack V2 + approved S1-C contract base; T-02/T-03/T-04 are unblocked by product decisions but remain dependent on T-01 (and ownership/sequencing where stated); T-05 remains dependent on T-01/T-02 and S1-C implementation/UI ownership; T-06 is approved in principle with **reject-before-mutation** and no repair; T-07 is **CANCELLED for S1-D V1**; T-08/T-09 remain downstream. No task was dispatched from this lane.
4. **Checklist amended.** `checklists/requirements.md` now tests every resolved decision, the functional requirements and AC-1…AC-15. Every implementation item remains **NOT STARTED**; no verification is claimed.
5. **Save policy.** S1-D V1 adds **no new persisted field** and names no fixed future schema version. Any later schema work must use the next available version on the actual integration base. T-07 is cancelled.
6. **Protected behaviour retained.** `ScenarioOneCompletionRules` normalised source and its N3-pinned hash, the eight objective predicates/targets, economy/ecology coefficients, Reference Future v1, no Windows build, no fixed intervention interval, no automatic treatment choice and no hidden automation replacing player judgement.

### 12.2 Verification (documentation only)

- `git diff 666cb67..AMENDED_HEAD_PLACEHOLDER` contains only the S1-D spec package and the handoff/review/amendment documents; **no production source, Unity asset, scene, prefab, save, package or ProjectSettings change**.
- No unresolved blocking `OPEN-*` remains in the S1-D package.
- No fixed future save-version number is named for S1-D.
- T-07 is CANCELLED.
- S1-C interface wording matches the Manager review.
- Completion rule and hash are explicitly protected.
- Status is `APPROVED` across spec, plan, tasks, checklist and this handoff.

No Unity, compile or runtime gate applies to this documentation amendment, and none was run. Rack V2, S1-C, MFM and three-species parity work are untouched. **No production files changed.**

### 12.3 Exit

`S1-D REPEATED-INTERVENTION SPEC APPROVED — READY FOR T-01 BASELINE RE-VERIFICATION AFTER RACK V2 CORE INTEGRATION / S1-C CONTRACT APPROVAL`

Return the amended HEAD to the Overall Manager. T-01 is not started from this lane.

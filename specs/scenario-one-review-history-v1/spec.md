# Scenario One Review / Management History — Specification

- **Spec ID:** SPEC-scenario-one-review-history-v1-V1
- **Feature ID:** `SCENARIO-ONE-REVIEW-HISTORY-V1` (S1-C)
- **Status:** APPROVED — implementation authority for this scope (Manager review `65d354a`, amendments per `Docs/Handoffs/S1-C-ReviewHistory-Spec/AMENDMENT-PACKET.md`)
- **Owner:** Overall Manager (ChatGPT Game Dev); specification drafted by the Lane C primary specification author
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27 (approved; open decisions resolved — see §15)
- **Decision log references:** D-060 (S1-C is PRE-DEMO), D-004 (core loop), D-010 (Work Plan commitment model), D-042 / D-019 (Reference Future v1 frozen), D-056 / D-057 (Enlarged80 geometry and calibration), D-063 (spec-driven development), D-064 (Rack V2 rack-plan provenance, independent lane), D-065 (Multi-Scale V1, later provenance contributor)
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness (`Docs/Project/current-milestone.md`; pre-demo sequence step 3)
- **Related evidence:** `Docs/Handoffs/S1-C-ReviewHistory-Spec/TASK-PACKET.md` (authorising packet); `Docs/Project/unity-project-overview.md`; `Docs/ReferenceFutureContract.md`; inspected repository state recorded in `specs/scenario-one-review-history-v1/plan.md` §1
- **Supersedes / superseded by:** none

## 1. Product intent

Scenario One is a complete-loop demo (D-060): the player must be able to start from the initial forest, assess it, make an intervention, advance time, observe consequences, **understand and review previous management**, make second and later interventions, and reach scenario completion.

This feature (S1-C) supplies the *review / management history* half of that loop. After management work and time advancement, the player must be able to understand:

- what work was carried out;
- when it happened;
- what trees / area / work type it affected, at an appropriate level;
- what immediate work result occurred;
- what forest and economic consequences are now visible;
- what earlier treatment should inform the next decision.

The result serves the core loop `decide → advance time → walk → observe → understand why → decide again` (D-004). It is **not** a spreadsheet-style analytics product.

S1-C is the history/review capability the later repeated-intervention system (S1-D) will consume. This spec defines that capability and its data contract; it does not implement the second/subsequent intervention loop.

## 2. Problem

The simulation already records substantial management history, but the player cannot look back through it.

Verified gaps (full evidence in `plan.md` §1):

1. **Only the latest year is reviewable.** `UI/AnnualReviewView.cs:55` renders `AnnualReports[Count - 1]` only. Earlier years' reports, although saved, are unreachable in play.
2. **No intervention history view.** Past work is reconstructible from saved `managementEvents` and resolved `workOrders`, but no surface answers "what did I do in year 5, where, and what came of it?".
3. **No objective/progression history.** `ScenarioObjectiveResult` (`ScenarioOneObjectives.cs:14-21`) has no achieved-year; only the scenario-level `outcomeYear` is recorded.
4. **Precision limits are not surfaced honestly.** Grouped-harvest money lands on one settlement record; storm review counts are reconstructed live; "DBH growth last year" and last-year browse assessments are session-only and vanish after load. A player cannot tell recorded fact from reconstruction.
5. **Nothing links prior treatment to the next decision.** The Work Plan looks forward; the Annual Review looks one year back; neither answers "given what I already did here, what should I consider now?".
6. **Second and later interventions (S1-D) have no agreed history contract**, so a later loop could invent competing records or lose identity linkage.

## 3. Goals

- Let the player review **every completed year** of management history, not only the most recent.
- Show, for recorded past work, **what / when / where / work type / immediate result / status** at an appropriate level of detail.
- Keep **recorded historical fact**, **current observed state**, **derived comparison** and **explanatory teaching** visibly separate.
- State honestly when a displayed value is recorded, derived, reconstructed or not recorded — and never fabricate a past fact from current state when provenance is absent.
- Connect prior treatment to the next decision in bounded teaching language that does not claim causal precision the simulation lacks.
- Define the **stable identity and data contract** later intervention systems (S1-D, later Multi-Scale provenance) may rely on.
- Preserve Reference Future v1 and old-save compatibility.

## 4. Non-goals

- Production code or Unity asset changes **in this task** (this spec is the deliverable).
- Implementing the second/subsequent intervention loop (S1-D) or any new intervention mechanism.
- A spreadsheet-style analytics product, KPI dashboards, export/reporting suites or comparative benchmarking.
- New ecology/economy coefficients, or any change to simulation rules.
- Multi-Scale Forestry Management (D-065) implementation; S1-C must not depend on unimplemented Multi-Scale state.
- Three-species parity implementation (D-059/D-061/D-062); S1-C must remain correct as that work lands.
- A map-history overlay or new cartography (the approved professional map stays frozen; see §8).
- Editing, rewriting or "correcting" recorded history; history is append-only.
- Windows build work.

## 5. User / player workflows

### 5.1 Review the year just completed (existing flow, retained)

1. The player advances one year from the Work Plan.
2. The Annual Review opens (existing gate: `ScenarioOneUiRoot.NeedsAnnualReview` forces acknowledgement before the next advance).
3. The player reads what was done, money, forest change, storm (if any), trends and objectives.
4. The player acknowledges and returns to the forest / Work Plan.

### 5.2 Look back at earlier management (new)

1. From the review surface the player selects an earlier completed year (or browses years in order).
2. The surface shows that year's **recorded** work, money and forest-change record: work carried out, when, which trees/area/work type (aggregate first; recorded targets on demand), the immediate recorded result, and status including failures and cancellations.
3. The player can move between "what I did", "what the year recorded", and "what the stand looks like now", with each labelled by content layer (§6).

### 5.3 Understand why / prepare the next decision (new, bounded)

1. From a reviewed year or intervention, the player can open a short explanatory view: earlier treatment that bears on the next decision, grounded only in recorded history plus current observed state.
2. Where useful, the player can go and look: reuse of the existing waypoint mechanism (as the storm card already does) so consequences stay walkable rather than purely textual.

### 5.4 Edge flows

- A year with no approved work still appears (it has an annual report and a `YearAdvanced` event).
- A pre-feature save has no objective-achievement records; the history surface shows "not recorded" unless the exact first-achievement year is evidence-complete under the FR-10 derivation rule, in which case it shows a labelled derived year. Never a fabricated date.
- After loading a save, session-only facts show "not recorded this session" rather than stale or invented values.

## 6. Content layers (mandatory separation)

Every item shown in history/review surfaces MUST belong to exactly one layer, and the presentation MUST make the layer distinguishable (label, section or equivalent):

- **L1 Recorded historical fact** — persisted at the time the fact occurred: `managementEvents`, `workOrders` (including resolved/failed), `annualReports`, `ecologicalSnapshots`, `stormEvents`, `deadwoodRecords`, per-tree history fields (`pruningLifts`, `crownBaseHeightM`, `lastPruningYear`, `mortalityCause`, `mortalityYear`, `equivalentSuppressedYears`), `outcome`/`outcomeYear`/`outcomeReason`, `centuryReview`.
- **L2 Current observed state** — the live forest and its inspection: now-visible canopy, regeneration, deadwood, browsing, tree condition, current cash, open orders.
- **L3 Derived comparison** — computed from L1/L2 at render time: year deltas, trend strips, century comparisons, retrospective objective-achievement derivation, reconstructed storm summaries.
- **L4 Explanatory teaching** — bounded guidance copy connecting treatment to consideration ("you opened the canopy here in year 6; regeneration response is recorded in these cells"), never a new authoritative fact and never a claim of causal precision the simulation does not contain.

## 7. Functional requirements

### History access and content

- **FR-1 Year-browsable review history.** The player MUST be able to open any completed year's review record, not only the latest. This extends the existing Annual Review surface; it does not create a second, parallel history system.
- **FR-2 Intervention record.** For each recorded intervention (order created / approved / cancelled / resolved), the player MUST be able to see: work type, year, what it targeted (tree IDs and/or cell/area at the level of FR-8), the immediate recorded result (ecological treatment, and where recorded volume/revenue/cost/stock), execution method (contractor / landowner), and status including failure reason where recorded.
- **FR-3 Year record.** For each completed year the player MUST see the recorded annual report content (work counts, money, volume, deadwood, owner minutes, closing cash) plus that year's ecological snapshot change against the previous snapshot where a previous exists.
- **FR-4 Provenance fidelity.** Historical displays MUST represent authoritative recorded past events. Where a displayed value is derived (L3) or reconstructed from current state (e.g. storm victim lists), it MUST be labelled as such. Where provenance is absent, the display MUST show "not recorded" rather than fabricating a past fact from current state; a derived value is acceptable only when it is exact under the FR-10 derivation rule and is labelled derived.
- **FR-5 Layer separation.** See §6. MUST be implementable as distinct sections/labels in every history surface.
- **FR-6 Consequence framing without over-claiming causality.** "What is now visible" MAY be linked to past treatment only where the simulation records the causal account (e.g. `ecologicalTreatment` on the event, `mortalityCause`/`mortalityYear` on trees, `fallenYear` on deadwood, `stormEvents`). Otherwise the display must present co-occurrence ("recorded in the same period") rather than causation.
- **FR-7 Prior-treatment guidance for the next decision.** From a reviewed year/intervention the player MUST be able to reach a short bounded teaching view (L4) naming earlier treatment that matters for the next decision (e.g. previous thinning/release in an area, retained deadwood choices, planting and protection history, storm losses). This view MUST be grounded in L1 records plus L2 observation and MUST NOT introduce new simulation facts or prescriptions. It MAY use the existing waypoint mechanism so the player can walk to see.
- **FR-8 Appropriate level of tree/area detail.** Intervention detail MUST lead with aggregate content (work type, counts, area/cells affected, species) and allow drill-down to the recorded targets (tree IDs, cell indices, positions). Full per-tree dumps are not the default presentation (Manager decision 2026-10-27: aggregate-first with drill-down).
- **FR-9 Money-history honesty.** Per-year money authority is the `ScenarioAnnualReport` (including `timberSales`, `harvestMinimumAdjustmentCents`, `closingCashCents`). For a grouped harvest, the settlement is recorded on one `WorkResolved` event (`SettleHarvestJob` overwrite, `ScenarioOneManager.cs:1301-1304`), so per-order money attribution inside a grouped job MUST NOT be presented as authoritative per-order fact; show it as the grouped settlement or attribute to the job. `harvestJobId` is a resolution year, not a unique sequence; do not present it as a unique job key.
- **FR-10 Objective / progression history.** The player MUST be able to see, per scenario objective, when it was achieved. New games MUST record achievement at the time it first occurs (FR-12). For pre-feature saves, an achievement year may be derived **only where the exact first-achievement year can be recomputed solely from persisted year-tagged L1 records (`ecologicalSnapshots`, `managementEvents`, resolved `workOrders`) and the objective's accepted logic**; any derived year MUST be labelled derived. Where exact reconstruction is not possible (e.g. because historical open-order or session state is absent), display "not recorded". A knowingly approximate year MUST NEVER be presented as an exact historical fact. Scenario completion (`outcomeYear`) remains the authoritative completion record.
- **FR-11 Storm / disturbance history.** `stormEvents` (`year`, `severity`, `directionDegrees`, `cropTreesLost`) are the recorded aggregate. Victim identities/dimensions are recovered from tree mortality and deadwood records; any count or list built that way MUST be labelled reconstructed (this matches the existing `StormDamageSummary` comment). Storm severity bands remain game bands, not measured wind speeds.
- **FR-12 Append-only history and recording at the time.** History records MUST NOT be rewritten by the review feature. Where FR-10 requires a new durable fact (objective achievement year), it MUST be written at the moment it first becomes true during normal simulation evaluation, not reconstructed into the past on save.
- **FR-13 Session-only fact labelling.** Facts that do not survive save/load (`PlantedJuvenile.lastYearBrowsed` / `lastBrowseAssessmentYear`, `annualDbhGrowth` "DBH growth last year", `RegenerationDiagnostics.LastYearBrowsed`) MUST be labelled session-only ("not recorded after load") wherever shown in history/review context. These facts are **not persisted in S1-C** (Manager decision 2026-10-27); revisit only if later playtesting demonstrates a player need strong enough to justify extra durable state.
- **FR-14 S1-D data/API contract.** See §9. The history capability MUST expose a stable, read-only query surface the later repeated-intervention system can rely on, and MUST NOT introduce a competing identity scheme for trees, orders, events, juveniles, deadwood or years.
- **FR-15 Bounded UI.** History surfaces MUST stay readable and walkable: no sortable analytics grids, no export/report suite, no comparative scoreboards. Multi-year presentation is limited to simple trend context (the existing trend-strip pattern may be extended within this bound).
- **FR-16 Independence from unimplemented systems.** The basic repeated-management history requirement MUST work with records that exist today. Rack V2 rack-plan provenance (D-064) and Multi-Scale prescription provenance (D-065) MAY later extend history content through their own recorded events; S1-C MUST NOT require them.

## 8. Authority / source of truth

- **Accepted product intent:** latest user instruction; `Docs/Project/decision-log.md` (D-060, D-004, D-010, D-042, D-056, D-057, D-063, D-064, D-065); `Docs/Project/current-milestone.md` (S1-C PRE-DEMO); `Docs/Project/game-brief.md` (core loop; history of the stand across decades).
- **Implementation facts:** inspected repository state on `spec/scenario-one-review-history-v1` (base `3b499aa`), recorded with file/line citations in `plan.md` §1; `Docs/Project/unity-project-overview.md`.
- **Supporting evidence:** `Docs/ReferenceFutureContract.md` (archive/preview/continuation semantics); CCF book guidance referenced by the Rack V2 spec is *management guidance*, not measured fact, and may inform L4 wording only with that label.
- **Spec authority:** when `APPROVED`, task-level implementation authority for this scope, subordinate to the project authority hierarchy above. `DRAFT` status confers no implementation authority.

## 9. S1-D data contract (what later intervention work may rely on)

The later repeated-intervention system (S1-D) may rely on the following, which S1-C treats as a compatibility surface:

**Stable identity keys (must be preserved):**

| Key | Form | Notes |
|---|---|---|
| Tree | `P{rr}{cc}`, `PO{rr}{cc}`, `T###`, `PL-PJ####`, `PL<year>-…`, `R<year>-…` | `TreeSaveData.treeId`; load matches by ID |
| Work order | monotonic `int` (`nextWorkOrderId`) | persists incl. resolved/failed |
| Management event | monotonic `int` (`nextManagementEventId`) | append-only log |
| Planted juvenile | `PJ####` (`nextJuvenileId`) | promotion keeps `PL-PJ####` linkage |
| Deadwood | `DW####` (`nextDeadwoodId`) | `fallenYear` recorded |
| Shelter | `S-J{juvenileId}` | |
| Cell | `index` relative to the save's `standGeometryModel` | not stable across geometry change |
| Storm event | `year` | one resolved event record per storm year |

**Read surfaces (append-only or immutable-by-convention):** `managementEvents` (`ScenarioManagementEvent`), `workOrders` (`ScenarioOneWorkOrder`, incl. `createdYear`/`resolvedYear`), `annualReports` (`ScenarioAnnualReport`), `ecologicalSnapshots` (`ScenarioEcologicalSnapshot`), `stormEvents` (`StormEventRecord`), `deadwoodRecords` (`ScenarioDeadwoodRecord`), per-tree history fields, `outcome`/`outcomeYear`/`outcomeReason`, `centuryReview`.

**Rules for consumers:** never rewrite these records; append new record types rather than repurposing existing fields; do not depend on Multi-Scale prescription provenance (D-065) or Rack V2 plan objects for the basic history requirement; treat `harvestJobId` as a resolution-year grouping label only. Note (R-4): S1-C deliberately does not record mark apply/remove or retain/release decision provenance — the commitment/result trail above is the contract; later systems may **append** new record types for decision provenance.

## 10. Evidence classification

This feature is presentation and record-keeping over existing simulation output; it introduces no ecological or economic coefficients. Where L4 teaching copy draws on forestry practice (e.g. release/thinning follow-up), that copy is **management guidance** (as already labelled in the Rack V2 / CCF book lineage), not measured evidence, and MUST NOT be worded as a universal prescription (Game Brief: no single correct prescription). Simulation-derived numbers shown in L1/L3 keep their existing evidence labels (e.g. storm severity = game band [C]; aspirational century targets = design targets [D], not forecasts).

## 11. Save / backward compatibility

- **Default: no mandatory schema change.** All core history content is served by records that already persist at save v20.
- **Additive schema for FR-10's "record at the time" path (accepted):** `ScenarioOneSaveData` gains `List<ScenarioObjectiveAchievement> objectiveAchievements`, each `{ string objectiveId; int achievedYear; float targetValueAtAchievement; }`, appended once per objective when it first becomes achieved during `EvaluateProgress`. Semantics: missing field or empty list = **not recorded**; no automatic migration and no retrospective writing into old saves; duplicates prevented by "first achievement only"; `achievedYear` is the ecological year in which evaluation first observed achievement. **Save-version sequencing (no fixed number):** inspect the actual integration base immediately before this persistence change and use the **next available save-schema version**; never reuse a version already owned by an accepted/qualified concurrent feature. Rack Planning & Field Marking V2 currently reserves/implements **v21**, so if Rack V2 integrates first, S1-C is expected to use **v22**. Old saves load unchanged (missing field defaults); Reference Future v1 stays frozen at its archive `saveVersion` 12 and is never migrated, resized or reinterpreted (D-042).
- **Rejected for this spec:** rewriting or synthesising history into old saves; changing any existing record shape; changing `workOrders` retention semantics.
- Derived objective-achievement years for pre-feature saves follow the FR-10 rule: exact recomputation from year-tagged L1 records only (`ecologicalSnapshots`, `managementEvents`, resolved `workOrders`) plus accepted objective logic, labelled derived; otherwise "not recorded".

## 12. Security / write-back

Not a networked or untrusted-input feature. Trust boundaries that do matter:

- **History integrity:** review surfaces are read-only over saved records. No write-back, no "fix my history", no re-serialization of frozen Reference archive content (hash integrity depends on original embedded JSON; `ScenarioReferenceArchive.Load` deliberately re-hashes the original bytes).
- **Save writes:** the only planned write is the additive FR-10 record during ordinary simulation evaluation; it follows existing save validation and old-save defaulting.
- **Failure handling:** a corrupt/missing history list must fail toward "not recorded" display, never toward fabricated content.

## 13. Acceptance criteria

- **AC-1 (what work / when / where / type / result):** From the review surface the player can open any completed year and see recorded work of that year with work type, year, affected trees/area at the approved detail level, immediate recorded result, execution method and status/failures — sourced from `managementEvents` / `workOrders` / `annualReports`, with grouped-harvest money shown per FR-9. Evidence: rendered review of at least one felling year, one planting year, one cancelled/failed order and one no-work year.
- **AC-2 (look back across years):** Years before the latest are reachable in play after a multi-year session; the latest-year content that exists today is preserved. Evidence: two-resolution rendered check (1280x720 and 1920x1080) after ≥ 3 advanced years.
- **AC-3 (layer separation):** In every history surface, L1/L2/L3/L4 content is distinguishable and labelled; reconstructed storm counts and derived deltas carry their labels; session-only facts show "not recorded after load". Evidence: rendered review with one example per layer.
- **AC-4 (no fabricated past):** A save from before the feature shows no invented history: an objective-achievement year appears only where exact first-achievement is evidence-complete from year-tagged L1 records and is labelled derived; otherwise it renders "not recorded". A knowingly approximate year never appears as fact. Evidence: genuine old-save load (v20 pre-feature save) review, including at least one objective that is exactly derivable and one that is not.
- **AC-5 (understand why / next decision):** From a reviewed intervention the player can reach bounded L4 guidance naming earlier treatment relevant to the next decision, grounded in recorded history/current state, with no new simulation claims; where a waypoint is offered, it leads to the real recorded location. Evidence: rendered walkthrough of the guidance view on ≥ 2 distinct interventions.
- **AC-6 (objective history):** In a new game, each objective's achieved year is recorded when it first becomes achieved and is shown in history; `outcomeYear` remains consistent. Evidence: runtime fixture completing ≥ 2 objectives across different years, save/load round-trip of the recorded years.
- **AC-7 (money honesty):** Grouped-harvest settlement is never displayed as authoritative per-order money; annual-report totals are used for per-year money. Evidence: code-level check plus rendered review of one grouped harvest year.
- **AC-8 (append-only / round-trip):** Opening history does not mutate saved records; save/load round-trip of a history-bearing save is byte-stable in its history lists. Evidence: before/after save comparison in a verification harness.
- **AC-9 (old saves and Reference Future):** v20 and older saves load with unchanged behaviour; Reference Future v1 preview/continuation and archive hashes unchanged. Evidence: existing save-compatibility gates re-run; archive hash check unchanged.
- **AC-10 (S1-D contract):** The identity/data contract in §9 is documented in code-facing form and covered by a stability check (no competing IDs; append-only), so S1-D can rely on it. Evidence: contract doc + verification assertions over ID formats and record retention.
- **AC-11 (bounded UI):** No analytics-grid/export/scoreboard surface ships; multi-year presentation is at most simple trend context. Evidence: rendered review.

## 14. Stop conditions

Stop and return to the Manager/spec (do not resolve by implementation assumption) if:

- satisfying FR-1–FR-8 requires changing ecology/economy rules, objective definitions/targets, or completion semantics;
- a required display cannot be sourced from recorded history and the honest answer would be "not recorded", but product wants a filled value anyway (never satisfy this by inventing or approximating history);
- implementation proposes a second authoritative history store, a competing identity scheme, or a rewrite of existing records;
- Reference Future v1, old-save loading, or save validation would change beyond the additive FR-10 field;
- Multi-Scale or Rack V2 unimplemented state becomes a hard dependency of the basic history requirement;
- the review surface grows beyond FR-15's bound (analytics product direction).

## 15. Resolved decision register (Manager review `65d354a`, 2026-10-27)

The four product questions raised in the draft are **resolved**; the register is retained for traceability. The decisions below are binding requirements, not recommendations.

- **R-1 (was OPEN-1) — Pre-feature saves and objective-achievement dates.** Derive and label an achievement year **only where the exact first-achievement year can be recomputed solely from persisted year-tagged L1 records and the objective's accepted logic**; otherwise show "not recorded". Never present a knowingly approximate year as exact fact. (Implemented as the FR-10 derivation rule.)
- **R-2 (was OPEN-2) — Session-only "what happened last year" facts.** Do **not** persist last-year browse assessments or per-tree "DBH growth last year" in S1-C. Keep them session-only and label them honestly after load (FR-13). Revisit only if later playtesting demonstrates a player need strong enough to justify extra durable state.
- **R-3 (was OPEN-3) — Intervention detail depth.** **Aggregate-first with drill-down to recorded targets** (FR-8). Full per-tree target dumps are not the default.
- **R-4 (was OPEN-4) — Decision provenance.** The existing commitment/result trail (`OrderCreated` / `OrderApproved` / `OrderCancelled` / `WorkResolved` plus retained resolved orders) is **sufficient** for S1-C. A complete historical trail of mark application/removal and retain/release decisions is **not required** here. Preserve an extension point for later repeated-intervention (S1-D) and Multi-Scale (D-065) provenance; do not build it now.

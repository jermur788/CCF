# Scenario One Review / Management History — Bounded Tasks

- **Spec ID:** SPEC-scenario-one-review-history-v1-V1
- **Spec status:** DRAFT (tasks may be drafted now; implementation must not be dispatched until the spec is `APPROVED`)
- **Task list owner:** Overall Manager
- **Last updated:** 2026-10-27

Tasks are implementation units, not new product decisions. Each task stays inside the approved spec and plan. If a task exposes a material product ambiguity, stop and return it to the Manager/spec (`spec.md` §14). OPEN-1–OPEN-4 must be resolved (or explicitly deferred with an approved default) before T-03/T-05/T-06 detail choices are locked.

## T-01 — History query layer (read-only)

- **Task ID:** T-01
- **Status:** TODO
- **Goal:** A read-only query helper over existing saved records that returns per-year history views with layer classification (L1 recorded / L3 derived).
- **Scope:** New `ScenarioOne/ScenarioManagementHistory.cs` (or equivalent); per-year views combining `annualReports`, `managementEvents` (all `Order*`/`WorkResolved`/`StockPurchased`/`YearAdvanced`), resolved `workOrders` by `resolvedYear`, `ecologicalSnapshots` deltas, `stormEvents`, `deadwoodRecords`; stable ID pass-through; grouped-harvest attribution metadata (job-level, per FR-9); no mutation of any record.
- **Exclusions:** UI; save schema changes; objective-achievement recording (T-05); ecology/economy code.
- **Owner / routing:** TBD (SOL/CODEX class implementation)
- **Dependencies:** —
- **Verification:** Offline fixture tests over a multi-year fixture save: per-year counts match raw records; cancelled orders only via events; grouped-harvest money reported at job level; round-trip read does not dirty the save structures.
- **Stop conditions:** Requires rewriting any existing record; cannot classify a value's layer honestly.
- **Completion state:** Tests pass; layer classification documented in code; no production record shape changed.

## T-02 — Year browsing in the Annual Review

- **Task ID:** T-02
- **Status:** TODO
- **Goal:** The player can open any completed year's review from the existing Annual Review surface.
- **Scope:** `UI/AnnualReviewView.cs` year navigation (previous/next and/or year list); same card structure per year built from that year's records; latest-year-first default preserved; `annualReviewSeen` acknowledgement gate semantics unchanged; trends remain simple strips (FR-15).
- **Exclusions:** New history system/panel; map overlay; analytics tables.
- **Owner / routing:** TBD
- **Dependencies:** T-01
- **Verification:** Unity Tier 2: session with ≥ 3 advanced years; navigate to an earlier year and back; acknowledgement gate still blocks advancement until reviewed; rendered check at 1280x720 and 1920x1080.
- **Stop conditions:** Gate semantics would change; ownership conflict on `AnnualReviewView.cs`/`ScenarioOneUiRoot.cs` unresolved.
- **Completion state:** AC-2 evidence collected; no regression to latest-year content.

## T-03 — Intervention drill-down and money honesty

- **Task ID:** T-03
- **Status:** TODO
- **Goal:** From a year's Work card, the player can inspect a recorded intervention at the approved detail level (OPEN-3), with honest money attribution.
- **Scope:** Expandable intervention rows: work type, year, aggregate (counts/cells/species) first, recorded targets on demand (tree IDs/cell/position), execution method, status/failure reason, result fields; grouped harvest shown as job settlement (FR-9); `harvestJobId` presented as a year label only.
- **Exclusions:** Per-tree analytics dumps as default; ledger-category persistence.
- **Owner / routing:** TBD
- **Dependencies:** T-01, T-02, OPEN-3 decision
- **Verification:** Offline harness on attribution rules; rendered review of one felling (grouped) year, one planting year, one cancelled and one failed order.
- **Stop conditions:** Product wants authoritative per-order money for grouped jobs (contradicts records; return to spec).
- **Completion state:** AC-1, AC-7 evidence collected.

## T-04 — Provenance labelling pass

- **Task ID:** T-04
- **Status:** TODO
- **Goal:** Every history/review item is visibly classified: recorded / current / derived / reconstructed / session-only / not recorded.
- **Scope:** Labels on storm reconstructed counts (FR-11), derived deltas and trends (L3), session-only browse and DBH-growth facts (FR-13), "not recorded" states for absent provenance (FR-4); wording review so no label implies causality the simulation lacks (FR-6).
- **Exclusions:** Persisting session-only facts (OPEN-2 default out of scope).
- **Owner / routing:** TBD
- **Dependencies:** T-02, T-03
- **Verification:** Rendered review with one example per layer; old-save load shows "not recorded"/labelled derivation, never invented values.
- **Stop conditions:** —
- **Completion state:** AC-3, AC-4 evidence collected.

## T-05 — Objective / progression history

- **Task ID:** T-05
- **Status:** TODO
- **Goal:** Each scenario objective shows when it was achieved; new games record achievement at the time it first occurs.
- **Scope:** Additive `objectiveAchievements` record per spec §11 (append in `EvaluateProgress` on first achievement; clone round-trip; save version bump to v21; old-save default empty); history display of achieved years; OPEN-1 behaviour for pre-feature saves (labelled derivation from year-tagged records, or "not recorded", per decision); `outcomeYear` unchanged.
- **Exclusions:** Changing objective definitions/targets/semantics; retrospective writes into old saves.
- **Owner / routing:** TBD
- **Dependencies:** T-02, OPEN-1 decision; G2/G3/G4 gates (save surface)
- **Verification:** Runtime fixture completing ≥ 2 objectives in different years; save/load round-trip preserves recorded years; old-save behaviour matches OPEN-1; protected save/reference gates re-run; no anchor silently re-baselined.
- **Stop conditions:** Any requirement to alter completion semantics or objective targets; anchor movement without explicit decision.
- **Completion state:** AC-6 evidence collected; save-compatibility evidence attached.

## T-06 — Prior-treatment guidance ("understand why")

- **Task ID:** T-06
- **Status:** TODO
- **Goal:** From a reviewed intervention/year, a short bounded L4 view names earlier treatment relevant to the next decision and can lead the player to look.
- **Scope:** Guidance card composed only from L1 records + L2 observation templates; optional reuse of the existing waypoint mechanism for recorded locations; copy rules per FR-6/FR-7 (no new simulation facts, no universal prescriptions).
- **Exclusions:** New lessons/curriculum; prescriptive decision support; simulation changes.
- **Owner / routing:** TBD
- **Dependencies:** T-02, T-03; L4 wording reviewed against spec §10 evidence rules
- **Verification:** Rendered walkthrough on ≥ 2 distinct interventions; waypoint (where offered) leads to the recorded location; copy audit for causal over-claim.
- **Stop conditions:** Guidance would need facts the simulation does not record.
- **Completion state:** AC-5 evidence collected.

## T-07 — S1-D data contract and stability checks

- **Task ID:** T-07
- **Status:** TODO
- **Goal:** The §9 identity/data contract is documented in code-facing form and guarded by checks so later intervention work can rely on it.
- **Scope:** Contract notes alongside the history helper; offline assertions: ID formats and counters stay stable, resolved orders/events are retained (append-only), no competing identity scheme introduced; note `harvestJobId` limitation where the API exposes it.
- **Exclusions:** Implementing S1-D itself; Multi-Scale provenance.
- **Owner / routing:** TBD
- **Dependencies:** T-01
- **Verification:** Offline contract checks pass on fixture saves (including a genuine old save).
- **Stop conditions:** —
- **Completion state:** AC-10 evidence collected.

## T-08 — Feature qualification and spec conformance

- **Task ID:** T-08
- **Status:** TODO
- **Goal:** Bounded qualification of the complete slice and a spec-conformance verdict.
- **Scope:** Tier 2 Unity feature gate (multi-year session, two-resolution rendered review, genuine v20 old-save load, save/load round-trip, grouped-harvest year, no-work year); protected gates as required by the adopted save change; run `checklists/requirements.md`; report deviations.
- **Exclusions:** Tier 3 escalation without a trigger; Windows build.
- **Owner / routing:** TBD
- **Dependencies:** T-01…T-07
- **Verification:** All ACs evidenced; checklist verdict recorded in the implementation handoff.
- **Stop conditions:** Any AC fails; spec deviation discovered.
- **Completion state:** Checklist complete with evidence; verdict PASS (or NEEDS CHANGES with reasons).

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T-01 | TODO | TBD | — |
| T-02 | TODO | TBD | T-01 |
| T-03 | TODO | TBD | T-01, T-02, OPEN-3 |
| T-04 | TODO | TBD | T-02, T-03 |
| T-05 | TODO | TBD | T-02, OPEN-1 (G2/G3/G4) |
| T-06 | TODO | TBD | T-02, T-03 |
| T-07 | TODO | TBD | T-01 |
| T-08 | TODO | TBD | T-01…T-07 |

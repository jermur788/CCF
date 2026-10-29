# S1-D IMPLEMENTATION 06 — TASK PACKET

## Task

**S1-D T-06 — Management-event ID integrity: reject ambiguous event IDs before load mutation.**

**Spec authority:** `specs/scenario-one-repeated-interventions-v1/` — T-06 / FR-E3 / AC-7.  
**Starting main for dispatch:** `e4bb1625e1b3ec7651a71e5bdab803c448357b88` or a clean descendant after refresh.  
**Recommended branch:** `feature/s1-d-management-event-id-integrity-v1`.  
**Recommended executor:** OpenAI Codex / strongest available Sol-class local code model. Claude Sonnet-class implementation is an acceptable fallback, but the mandatory independent review must use a different provider/model where practical.  
**Unity:** not required for production implementation. Any Editor save-hardening qualification must respect the machine-wide queue and may not bypass RACK-V2-08 claim #49.  
**Windows build:** prohibited.

## Why this task exists

Repeated-intervention history depends on stable management-event identity. Current `ForestSaveValidation.ValidateScenarioCellIndices` checks management-event record presence and cell-index validity but does not reject non-positive or duplicate `ScenarioManagementEvent.eventId` values. Both `CanLoad` and `LoadData` already converge through `ForestSaveController.FindLoadProblem` → `ForestSaveValidation.Validate` before the first world mutation, so the integrity rule must be added once at that shared validation boundary rather than implemented independently in each load path.

This is save-history hardening, not a new persistence format and not a chronology repair mechanism.

## Preflight — mandatory before production edits

1. Refresh current `main`, this packet, `Docs/Project/coordination/active-tasks.md`, the task claim and the repeated-interventions spec.
2. Record actual agent/model/provider, branch, worktree and starting HEAD in the claim before any production edit.
3. Inspect the current save/load call path and confirm `CanLoad` and `LoadData` still share the same pre-mutation validation seam.
4. Inventory the repository's shipped/accepted save fixtures and supported save-version test data. Scan every Scenario One management-event list that exists in those fixtures for:
   - `eventId <= 0`;
   - duplicate positive event IDs within the same Scenario One history.
5. If any legitimate shipped/accepted fixture has ambiguous IDs, **STOP before changing compatibility behaviour** and return:

`S1-D T-06 SAVE HISTORY AMBIGUITY — MANAGER DECISION REQUIRED`

Do not renumber, repair, migrate or reinterpret such a fixture without a new Manager decision.

## Required implementation

Implement the smallest shared pre-mutation validation needed to satisfy FR-E3:

- For every non-null Scenario One management-event list presented to the normal parsed-save validation path:
  - a management event with `eventId <= 0` is invalid;
  - two management events with the same positive `eventId` are invalid.
- Return a clear, deterministic validation reason suitable for the existing player-facing `Save not loaded: …` wrapper.
- Keep the rule in the shared `ForestSaveValidation` path used by both `CanLoad` and `LoadData`. A narrow helper such as `ValidateManagementEventIds` is appropriate; exact factoring is the worker's choice.
- Rejection must happen before any world mutation.
- Missing/null management-event lists remain compatible wherever the current save-version rules already allow them.
- Do **not** repair, renumber, deduplicate or reorder records.

Expected production surface is narrowly `Assets/ForestPrototype/ForestSaveValidation.cs`. If another production file is genuinely required, stop and explain before expanding scope unless it is only the existing shared load-validation wiring and no semantics change is needed.

## Verification requirements

### A. Fixture-history compatibility scan

Produce an auditable list of shipped/accepted save fixtures and versions checked. For each relevant fixture record whether a management-event list exists and whether all IDs are positive/unique. This scan is evidence; do not rewrite fixtures merely to pass.

### B. Focused positive/negative validation

Add bounded verification proving at minimum:

1. a valid Scenario One history with distinct positive event IDs remains accepted;
2. `eventId == 0` is rejected;
3. a negative `eventId` is rejected;
4. duplicate positive event IDs are rejected;
5. a historically valid missing/null event list remains accepted where current version semantics permit it;
6. unrelated save validation remains unchanged.

### C. `CanLoad` / `LoadData` parity

Prove both public load routes reject equivalent invalid management-event identity through the same underlying validation rule:

- invalid disk save → `CanLoad` false and `Load`/read path refuses it;
- equivalent parsed data → `LoadData(..., false)` false;
- the reason class/message is consistent enough to demonstrate one shared authority, not two drifting implementations.

### D. No mutation on rejection

For invalid zero-ID and duplicate-ID cases, show rejection does not mutate the live world. At minimum preserve ecological year and tree count; where the existing save-hardening harness can cheaply assert more Scenario One state, do so. The disk save bytes must not be rewritten as part of rejection.

### E. Preservation

Preserve and run proportionate existing save-hardening checks, including the existing `SAVE_HARDENING_VERIFY_PASS` gate and the cell-index hardening suite where applicable. Do not weaken or rebaseline existing assertions.

If an Editor/Unity run is necessary for the existing save-hardening gate, static implementation may complete first, but Unity execution waits until the Manager releases the machine-wide lane after the current Rack priority (#49), unless #49 is already completed and live coordination says otherwise.

## Hard exclusions

This task does **not** authorise:

- save schema/version bump;
- new persisted fields;
- management-event chronology repair or migration;
- changing event creation semantics except if required solely to preserve the already-valid positive/unique contract and separately justified;
- completion-rule changes;
- S1-C UI/history changes;
- Rack V2, MFM, parity, ecology, economy or objective changes;
- changing Reference Future v1;
- Windows/Player build.

If implementation reveals a legitimate historical compatibility case that cannot satisfy positive/unique IDs without changing meaning, stop for Manager decision rather than weakening the rule.

## Handoff and review gate

Publish:

`Docs/Handoffs/S1-D-Implementation-06/HANDOFF.md`

The handoff must include:
- actual model/provider;
- branch/worktree;
- starting HEAD, implementation SHA and published tip;
- exact production and verification files changed;
- fixture-history inventory/results;
- focused positive/negative results;
- `CanLoad`/`LoadData` parity evidence;
- no-mutation evidence;
- save-hardening preservation results;
- whether Unity was run, and exact candidate if so;
- repo/worktree cleanliness and unrelated-work accounting;
- unresolved risks.

A successful implementation handoff returns:

`MANAGEMENT EVENT ID INTEGRITY HARDENED — READY FOR INDEPENDENT REVIEW`

If static implementation is complete but the applicable Unity save-hardening gate cannot run because the Rack task owns/has priority on the Unity lane, return instead:

`S1-D T-06 STATIC IMPLEMENTED / UNITY SAVE-HARDENING WAITING — RACK UNITY PRIORITY`

**Independent review is mandatory before Manager integration/acceptance.** After implementation handoff, the Manager will create a separate review claim for a different model/provider where practical. The implementation worker must not self-approve this shared load-path change.

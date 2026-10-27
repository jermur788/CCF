# S1-C IMPLEMENTATION 03 — INTERVENTION DRILL-DOWN AND MONEY HONESTY

**Task ID:** S1-C-IMPL-03 / spec T-03  
**Feature:** Scenario One Review / Management History V1  
**Spec:** `specs/scenario-one-review-history-v1/` — APPROVED  
**Overall Manager:** ChatGPT Game Dev  
**Task type:** PACKET ONLY — bounded implementation + Tier-2 rendered qualification

## Status

**READY FOR IMPLEMENTATION.**

T-01 and T-02 are canonically integrated on `main` via PR #18 / merge `169970f2d6b0268d53a4077e0e97934d290bf7eb`. The T-02 Manager review is `Docs/Handoffs/S1-C-Implementation-02/MANAGER-REVIEW.md`.

Start from current `main` descending from `169970f2`. Record the exact starting SHA in the task claim before edits.

## Goal

From a selected Annual Review year, let the player inspect each recorded intervention at the approved aggregate-first detail level without inventing per-order financial attribution.

This is T-03 only.

## Required product behaviour

Follow approved spec T-03 / R-3 / FR-9 exactly:

1. The selected year's **Work** presentation remains aggregate-first.
2. Recorded interventions/work groups for that selected year can be expanded/drilled down on demand.
3. Default rows show concise, useful aggregate facts rather than a per-tree analytics dump.
4. On expansion, show only recorded target detail that actually exists, including as applicable:
   - work type;
   - resolution year;
   - aggregate count/cells/species;
   - recorded target tree IDs;
   - recorded cell and/or position when persisted;
   - execution method;
   - status/outcome;
   - recorded failure reason;
   - recorded result fields supported by existing records.
5. Cancelled orders are presented from their surviving cancellation event. Do not imply the deleted order object still exists.
6. Failed orders are visibly distinct from successful work and use the recorded failure reason.
7. Grouped commissioned harvest money is presented as **one job settlement**, not fabricated per-order money.
8. `harvestJobId` is a **resolution-year label**, not a unique authoritative job identifier. Do not present it as a durable unique ID.
9. Annual-report totals remain the per-year money authority.
10. The selected-year boundary remains `ScenarioOneManager.HistoryYears` / `YearHistoryView`. Do not reintroduce raw-list joins in `AnnualReviewView` for selected-year facts.
11. Browsing an older year remains read-only and cannot acknowledge the latest review or alter simulation/save state.

## Preferred implementation surface

Primary:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs`

Allowed only if a concrete T-03 detail cannot be expressed through the existing query surface without duplication:

- `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`

Verification-only additions/changes under:

- `Tools/Verification/S1CManagementHistory/`

Do not modify other production files without returning to the Manager for an explicit scope decision.

## Required UI structure

The exact visual mechanism is implementation choice, but it must satisfy:

- aggregate summary is visible without expansion;
- recorded target details require deliberate expansion/drill-down;
- the expanded state remains readable at 1280×720 and 1920×1080;
- no row implies per-tree/per-order financial precision that the records do not contain;
- grouped harvest settlement is clearly associated with the year's grouped commissioned job;
- absent recorded target/location fields say `not recorded` or omit the field honestly rather than reconstructing an unsupported value.

Reuse existing UI Toolkit patterns where practical. Do not introduce a new history screen or navigation mode.

## Required fixtures / proof cases

At minimum, qualification must cover:

1. **Grouped felling year**
   - multiple harvest orders in one commissioned harvest group;
   - aggregate-first row;
   - expanded recorded targets;
   - exactly one grouped settlement presentation;
   - no fabricated per-order revenue/cost allocation.
2. **Planting year**
   - aggregate species/count summary;
   - expanded recorded target/location/order detail where actually persisted;
   - execution method shown honestly.
3. **Cancelled order**
   - survives through cancellation event only;
   - displayed as cancelled without inventing a live order record.
4. **Failed order**
   - failure visibly distinct;
   - recorded failure reason shown.
5. **No-work year**
   - retains coherent T-02 behaviour.
6. **Older-year browsing**
   - remains read-only;
   - latest-year acknowledgement semantics unchanged.

## Verification

### Offline / focused checks

Add or extend a deterministic harness proving:

- attribution rules above;
- grouping is deterministic;
- `YearHistoryView.HarvestSettlement` is consumed as grouped settlement metadata;
- no per-order grouped-harvest money is invented;
- cancellation/event-only handling;
- missing target/location fields do not become reconstructed facts;
- selected-year UI source audit does not add raw-list joins over `ManagementEvents`, `AnnualReports` or `EcologicalSnapshots` for the selected year.

Re-run the existing T-01 offline history check if `ScenarioManagementHistory.cs` or its shared parsing helpers change. If they do not change, retained T-01 evidence may be referenced with an explicit no-diff proof.

### Unity / rendered Tier 2

Use the machine-wide Unity lock normally. Do not bypass another owner.

Run a bounded Annual Review fixture including the four required intervention cases above. Verify:

- no compile/import errors;
- drill-down opens/closes correctly;
- selected-year switching keeps the correct intervention detail;
- latest acknowledgement gate is unchanged;
- older-year drill-down cannot mutate state;
- rendered captures at **1280×720** and **1920×1080** for representative collapsed and expanded states;
- grouped-harvest money wording is legible and unambiguous;
- cancelled/failed states are legible.

**Unity sequencing:** PARITY-03 Unity qualification claim #10 is the older queued Unity task after T-02. Static implementation may proceed while that task owns the lane, but T-03 must wait its turn for rendered qualification.

## Exclusions

Do not implement:

- T-04 provenance labelling pass;
- T-05 objective achievement persistence/save schema;
- T-06 prior-treatment guidance/waypoints;
- T-07 contract/stability work beyond tests needed for T-03;
- T-08 whole-feature qualification;
- new ledger-category persistence;
- per-order allocation of grouped harvest settlement;
- new ecology/economy mechanics;
- Rack V2, S1-D or MFM changes;
- scene/prefab/species asset edits;
- Windows build.

The inherited older-year Objectives hint may be noted but is not a T-03 requirement unless the new drill-down copy itself makes it materially misleading. Provenance wording beyond money honesty stays T-04.

## Stop conditions

STOP and return to the Manager if:

- authoritative per-order grouped-harvest money would be required;
- the current records cannot support a requested target/status/location detail honestly;
- implementation would require a new persisted field or save-schema change;
- another live writer owns `AnnualReviewView.cs` or the history-query files;
- a Unity owner is active when this task reaches rendered qualification;
- a protected T-02 acknowledgement/navigation behaviour would need to change.

## Required handoff

Publish:

`Docs/Handoffs/S1-C-Implementation-03/HANDOFF.md`

Include:

- exact starting SHA, implementation SHA and final tested SHA;
- actual model/provider, branch and worktree;
- exact production file diff;
- grouped-harvest attribution proof;
- cancellation/failed-order proof;
- selected-year source audit;
- offline command/results;
- Unity lock/HEAD evidence;
- rendered capture list and inspection result;
- T-02 regression result;
- caveats / unverified items;
- confirmation that T-04+ were not implemented.

## Exit token

On success:

`S1-C INTERVENTION DRILL-DOWN IMPLEMENTED / QUALIFIED — READY FOR MANAGER REVIEW`

If blocked, return a precise blocker token and publish the handoff with the evidence obtained.

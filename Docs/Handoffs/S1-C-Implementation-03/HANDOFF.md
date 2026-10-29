# S1-C Implementation 03 — intervention drill-down and money honesty: static handoff

**Exit token (blocked, not the success token):**

`S1-C T-03 STATIC IMPLEMENTED / OFFLINE QUALIFIED / RENDERED TIER-2 NOT RUN — UNITY OWNER ACTIVE, RETURN TO MANAGER`

The production change is implemented and passes every offline gate. The packet's rendered Tier-2 qualification has **not** been run, so the success token (`S1-C INTERVENTION DRILL-DOWN IMPLEMENTED / QUALIFIED — READY FOR MANAGER REVIEW`) is deliberately **not** claimed. This closes **T-03 static work only**. No integration, no Unity import/compile, no rendered review, no full S1-C or spec conformance and no acceptance is claimed.

## Identity

- Task: `S1-C-IMPL-03 / spec T-03`; claim issue **#19** (marked `[RUNNING]` before any production edit; left open as `[BLOCKED]`, not closed, because rendered qualification is outstanding).
- Worker: T3 Code / Claude Code, local session, Anthropic `claude-sonnet-5-5` (Sonnet 5.5). No subagents, no delegation.
- Branch / worktree: `feature/s1-c-history-drilldown-v1`, `/media/jer/ZX20/CCF-S1C-Impl03` (code/static; no Unity Library; created clean from `origin/main`).
- **Exact starting SHA:** `b56af1aa150a1e5b3a886f7c82e8dfbe0f428dd1` (origin/main; descends from `169970f2`; includes S1-D T-02 `9fcb5ee`). The first worktree was created at `7b78a52` and fast-forwarded to `b56af1a` (two docs-only research commits) before any edit.
- **Implementation SHA / final tested SHA:** `859f74e2c07c87d7f166f58982d914d72a29b34f`. The publication commit above it changes this file only.
- Live coordination: the `active-tasks.md` row for S1-C-IMPL-03 was set to RUNNING on `main` at `9d4aa00850fcf512be944a546061eddb747dd9c5` (one row, coordination-only) before production edits, and is updated again with this handoff.
- Context: packet `Docs/Handoffs/S1-C-Implementation-03/TASK-PACKET.md`; approved spec `specs/scenario-one-review-history-v1/` (T-03 / R-3 / FR-8 / FR-9 / AC-1 / AC-7).
- Durable evidence: `/media/jer/Files/CCF/Verification/S1C-T03-DrillDown-20261028/`.

## Exact production file diff (base `b56af1a` → `859f74e`)

| File | Change |
| --- | --- |
| `Assets/ForestPrototype/UI/AnnualReviewView.cs` | +196 / −7. `BuildWork` is now an instance method. Existing aggregate labels are byte-for-byte unchanged and each gets an optional "Show recorded details" row. New: one harvest-settlement block, a cancelled-orders aggregate line, drill-down item rendering, one muted note in the Money column. Open rows are tracked per year in a `HashSet`, cleared when the review is reopened or a new latest year appears. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs` | +199 / −0, additive only. New `WorkItemStatus`, `WorkItemDetail`, `WorkGroupDetail`, `YearWorkDetail` and `ScenarioManagementHistory.DescribeWork(YearHistoryView)` plus private helpers. No existing member, field or behaviour changed; `BuildYearViews`/`BuildYearView` are untouched. |

**Why a query-layer addition was needed (packet allows this only for a concrete reason):** the attribution rules (grouped-harvest items carry no money, cancelled items come from the cancellation event, unrecorded fields stay unrecorded, deterministic grouping) are exactly what the packet wants proven offline. Putting them in `AnnualReviewView.cs` would make them untestable outside Unity and would duplicate the pairing the query layer already does. `DescribeWork` reads only a `YearHistoryView`, so the selected-year boundary is preserved.

Not touched: `ScenarioOneManager.History.cs` (so the integrated `ScenarioOneManager.CycleState` accessor is byte-identical — empty diff), `ScenarioInterventionHistory.cs`, `ManagementHistoryText.cs`, `ScenarioOneUi.uss` (only existing classes `btn-ghost`, `chip-bad`, `chip-warn`, `faint`, `muted`, `body`, `row-wrap` are used), any save/schema, scene, prefab, species asset, `.meta`, ProjectSettings or Packages file.

Verification-only additions: `Tools/Verification/S1CManagementHistory/S1CDrillDownOfflineCheck.cs` and `run_t03_offline_check.py` (new; the existing T-01/T-02 tooling is unchanged).

## Behaviour implemented

- Aggregate-first: every existing Work line keeps its text. Under each, **Show recorded details · N tasks · M map squares** opens the recorded tasks; **Hide recorded details** closes them. Opening is designed as an in-place show/hide with no review rebuild (so no scroll reset is expected — **not yet observed in Unity**) and is presentation state only.
- Each recorded task shows: work label, target tree id (tree tasks only; "tree not recorded" if blank), order id, species, map square and/or position (or "location not recorded"), execution method ("done by you" / "done by contractor", or "execution method not recorded"), year, shelter (only while the order still exists), recorded result, recorded stem volume, and — for successful **non-harvest** work only — that task's own recorded contractor cost / materials / your time.
- **Grouped harvest:** one block `annual-harvest-settlement` per Work card: "Harvest job · year N: K harvest order(s) settled together as one job. Timber income … · harvesting cost …", with a faint note that the records hold one settlement, that no money is shown per order or tree, and that the year number labels the job and is not a unique ID. Grouped harvest items carry no money. The Money column adds one muted line saying its timber and harvesting lines belong to one grouped job. Annual-report totals remain the per-year authority.
- **Cancelled:** a new "N order(s) cancelled before they were carried out" line with a drill-down, built from `OrderCancelled` events only, `CANCELLED` chip, "execution method not recorded", and a footer saying the order no longer exists.
- **Failed:** `FAILED` chip (`chip-bad`) plus "Reason: …" from the recorded failure reason, or "Reason: not recorded". A failed harvest order is outside the grouped job and has no money or volume.
- `harvestJobId` is never read by the drill-down or the view; the settlement exposes only the resolution-year label.
- Older-year drill-down is the same read-only presentation. The acknowledgement footer predicate and the callback guard are untouched.

## Grouped-harvest attribution proof (offline)

Fixture year 1: three fellings + one windthrow salvage resolved as one commissioned job, the settlement (revenue 180000, cost 250000) on the last harvest event (event 4), earlier harvest events at zero money.

- `DescribeWork(year).HarvestSettlement` is the **same object** as `YearHistoryView.HarvestSettlement` (not recomputed); it carries event id, revenue, cost and the year label.
- All four harvest items are `IsGroupedHarvest` and `OwnMoneyRecorded == false` with zero cost fields. Writing `contractorCostCents = 999999` / `timberRevenueCents = 888888` onto an earlier harvest event leaves the whole detail serialisation unchanged.
- Rewriting every order's `harvestJobId` to unrelated values leaves the detail unchanged; the `DescribeWork` source text does not mention `harvestJobId`; no `JobId`-named member is exposed.
- Exactly one settlement presentation is built per Work card (source audit counts `AddHarvestSettlement(c, detail)` = 1 and the element name = 1).
- Only successful **non-harvest** work passes its own recorded cost fields through (planting stock 500 / 5 owner minutes, pruning contractor 1800).

## Cancellation / failed-order proof (offline)

- Year 3 fixture has two cancellations and **no order objects** for either; both surface from the events alone with target, species and cell as recorded, `ExecutionMethodRecorded == false`, no failure reason, treatment or money, and they are neither "done" nor "failed".
- Year 4: a failed harvest order (`GONE1`, reason carried verbatim) and a failed planting (reason verbatim) are in the failed group; they have `IsGroupedHarvest == false`, no volume or money, and are excluded from `HarvestSettlement.GroupedOrderIds`, which still holds only the two successful fellings.
- Year 6: a failure with no recorded reason has an empty reason (rendered as "Reason: not recorded"); a pruning whose event has no tree id, cell, position or species stays blank even though its still-existing order record holds all of them — values are never borrowed from current state.

## Selected-year source audit

`AnnualReviewView.cs` raw-list mentions are identical to the T-02 baseline: `.ManagementEvents` ×1 (change key), `.AnnualReports` ×3 (change key + all-year trend), `.EcologicalSnapshots` ×2 (all-year trend); every such line is a count or an all-year trend. No `harvestJobId`, no raw work-order access, no join/filter over a raw record list, no per-event money field in the Work card, and no state-changing call in the drill-down code. The Work column reads `ScenarioManagementHistory.DescribeWork(year)` on the selected `YearHistoryView`.

## Offline command / results (final tested SHA `859f74e`)

```bash
python3 -I -B Tools/Verification/S1CManagementHistory/run_t03_offline_check.py       # S1C_T03_DRILLDOWN_PASS checks=72
python3 -I -B Tools/Verification/S1CManagementHistory/run_offline_check.py           # S1C_HISTORY_QUERY_PASS checks=55   (T-01, re-run: ScenarioManagementHistory.cs changed)
python3 -I -B Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py  # CYCLE_STATE_OFFLINE_PASS checks=93  (S1-D T-02 regression; CycleState preserved)
```

- **Mutation controls, 6/6 caught:** grouped harvest keeping per-order money; a cancellation claiming an execution method; a failed harvest joining the job; a default position treated as recorded; the drill-down reading `harvestJobId`; unstable group order. Each made the T-03 check fail; the file was restored byte-identical afterwards.
- **Type-check (not a Unity compile):** the whole runtime `Assets/` tree (172 files, Editor and TutorialInfo excluded) was compiled with `dotnet` against Unity 6000.6.0f1's own managed assemblies plus a tiny `UnityEngine.InputSystem` stub. `AnnualReviewView.cs` and `ScenarioManagementHistory.cs` report **0 errors**; the 74 errors are all InputSystem stub gaps in other files. A deliberately injected error in `AnnualReviewView.cs` was reported (`CS0103`), proving the check reaches method bodies, then reverted. This is evidence, not a substitute for Unity import/compile.
- Evidence files: `t03-offline.txt`, `t01-offline.txt`, `s1d-t02-offline.txt`, `tested-head.txt`, `typecheck-note.txt` in the evidence directory above.

## Unity lock / HEAD evidence — rendered Tier-2 NOT run

- At 2026-10-28T22:12Z a Unity Editor process was running on `/media/jer/ZX20/CCF-U1` (RACK-V2-08-LAYOUT-REPAIR-01, claim #59 `[RUNNING]`, the active Rack V2 critical path). Claims #70 (PARITY-05 Unity qualification) and #74 are `[WAITING]` for Unity ahead of this task.
- Per the packet's stop condition ("a Unity owner is active when this task reaches rendered qualification") and the Manager's instruction not to bypass the Rack V2 path, **no Unity process was started, the machine-wide lock was not acquired or touched, and no lane was checked out.** No exact-HEAD Unity evidence exists.

## Rendered captures

None. **Not verified:** that the code compiles/imports in the Editor, that drill-down opens/closes and keeps the right detail when switching years, that the latest acknowledgement gate and older-year callback behave as in T-02 with the new UI present, and that the collapsed/expanded states, `FAILED`/`CANCELLED` chips and settlement wording are legible at 1280×720 and 1920×1080.

## T-02 regression result

Offline/static only: the footer predicate and the callback guard are present and unchanged (source audit), `ScenarioOneUiRoot` and manager acknowledgement logic untouched, and the existing aggregate Work labels are byte-identical. The Unity T-02 gate (`run_t02_gate.py`, `compare_reviews.py`) has **not** been re-run. Expected textual deltas against T-02 at the rendered turn: new toggle buttons in the Work column, the settlement block for harvest years, the cancelled line where cancellations exist, and one muted note in the Money column of harvest years — nothing else should differ.

## UNITY GATES STILL REQUIRED (for the rendered turn)

1. Take the machine-wide lock normally after #59 releases and #70/#74 have run (queue order is the Manager's), with the lane placed at the exact tested SHA.
2. Import/compile clean at `859f74e` (or the then-current published tip).
3. Author a disposable T-03 fixture alongside the T-02 harness (`AnnualReviewYearBrowsingVerification.cs` is the pattern: real new-game session, manager APIs, `Settle`, `ClickButton`, `Shot`, text dumps): a grouped felling year (+ salvage if available), a planting year (one by owner, one by contractor), a cancelled order, a failed order, and a no-work year. Failing/cancelling through the real APIs is preferable to constructing records.
4. Assert: toggles open/close and `work-detail-toggle-*`/`work-detail-*` names exist; open state survives switching to another year and back and is cleared on reopen; exactly one `annual-harvest-settlement` in a harvest year; no settlement or money in item rows; `FAILED`/`CANCELLED` chips and reasons present; "not recorded" wording appears only where fields are absent; world hash, event/report counts and `AnnualReviewSeen` unchanged by any drill-down on an older year; latest acknowledgement still gates exactly as before; T-02 gate and `compare_reviews.py` show only the expected deltas.
5. Captures at 1280×720 and 1920×1080 of representative collapsed and expanded states for the grouped-harvest, planting, cancelled and failed years, opened and inspected.

## Caveats and decisions for the Manager

- **Per-task cost on non-harvest work.** Planting/pruning/regeneration items show that task's own recorded contractor cost, materials and owner minutes (the resolution event holds them per task). Grouped harvest is the only place the spec forbids per-order money, and it carries none. If the Manager prefers no per-task money at all, it is a four-line removal in `BuildDetailItem` and the query layer's `OwnMoneyRecorded` fields.
- **Cancellation year.** `OrderCancelled` events are stamped with the year current when the player cancelled. A cancellation made while planning the next year therefore appears in the previously completed year's review. The text says "cancelled in year N" from the event; it does not claim more.
- **Position heuristic.** A world position of exactly (0, 0, 0) is the event's unwritten default and is treated as "not recorded"; a genuinely recorded origin position would read as not recorded (errs honest).
- **Execution method.** Resolution events carry it; cancellation events never do, so cancelled items say "execution method not recorded" rather than defaulting to Contractor.
- **Large felling years.** All recorded tasks are listed on expansion (one row each, built only when first opened). No cap or paging was added; the Tier-2 capture of a large year may show whether one is wanted.
- **Not changed (T-04 owns):** the inherited `BuildHistory` intervention list and `Read year N` buttons, the older-year Objectives hint, and provenance labelling beyond the money wording.
- The Money column note and the Work-column settlement block are the only grouped-harvest money presentations; the per-year totals above them remain the annual report's.

## Confirmation — T-04+ not implemented

No T-04 provenance pass, T-05 achievement persistence/save schema, T-06 guidance/waypoints, T-07 contract/stability work, T-08 qualification, ledger-category persistence, per-order allocation, ecology/economy change, Rack V2/S1-D/MFM change, scene/prefab/species edit, package/ProjectSettings change or Windows build. Save schema is unchanged (no new persisted field).

## Unrelated work preserved

`/media/jer/ZX20/CCF-Primary` (branch `ops/workflow-foundation-v1`) still carries its unrelated modified `AGENTS.md` / `active-tasks.md` and untracked `Docs/Operations/StorageArchitecture.md` / `Docs/ScenarioTwoPreProductionAudit.md`; none were touched. All work was done in the new worktree. The single coordination commit pushed to `main` (`9d4aa00`) edited only the S1-C-IMPL-03 row.

## Recommended next step

Manager dispositions the rendered turn: either queue T-03 for the U1 lock after #59/#70/#74, or authorise a different Unity-capable lane. Static review of the two production files can proceed now in parallel. On the rendered pass, if the T-02 comparison shows only the expected deltas and the captures are legible, T-03 is ready for Manager review; otherwise return the captures with the failure.

## 2026-10-29 static reconciliation to current main (#91 orchestrator)

- `origin/main` advanced to `72dbee8` while this branch's accepted static candidate was `859f74e` (published tip `cf92b9c`).
- A normal merge of `origin/main` into `feature/s1-c-history-drilldown-v1` produced merge commit `6aa00a5`. `git merge-tree` reported no conflict; no T-03 production byte was rewritten by hand.
- Diff against current `origin/main` remains exactly the accepted surface: `Assets/ForestPrototype/UI/AnnualReviewView.cs`, additive `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs`, and the T-03 offline verification (`S1CDrillDownOfflineCheck.cs`, `run_t03_offline_check.py`, this handoff). No save schema, ecology/economy, Rack V2, S1-D, MFM, scene/prefab/species, package or ProjectSettings change.
- Offline gate rerun on the merged tree: `S1C_T03_DRILLDOWN_PASS checks=72`, exit 0, output `/media/jer/Files/CCF/Verification/S1C-T03-Reconcile-20261029/`.
- Still **not qualified**: no Unity import/compile, no T-03 rendered fixtures, no T-02 rendered regression, no 1280x720/1920x1080 captures. Rendered qualification remains pending behind the serialized Rack/S1-D Unity queue and will run from `6aa00a5` or a later descendant.
- No integration to `main`, no Windows build, no lane/lock acquired. This reconciliation is static preparation only.

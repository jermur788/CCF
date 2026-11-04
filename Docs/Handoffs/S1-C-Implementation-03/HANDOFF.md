# S1-C T-03 — rendered Tier-2 qualification continuation, 2026-10-29

`S1-C INTERVENTION DRILL-DOWN IMPLEMENTED / QUALIFIED — READY FOR MANAGER REVIEW`

**Worker assessment:** bounded T-03 qualification PASS after one in-scope rendering repair. Ready for Manager review; this is not Manager acceptance, integration to main, full S1-C conformance or Tier-3 qualification. Claim #19 remains **open**, as the latest direct instruction requires.

## Identity and exact candidates

- Test qualifier: **OpenAI `gpt-6.1-sol`, high reasoning**, T3 Code / Codex harness. No model/provider transition and no delegated workers.
- Umbrella #91; claim #19; branch `qual/s1-c-t03-rendered-v1`; worktree `/media/jer/Videos/CCF-S1C03Qual`.
- Supplied/first tested candidate: **`df09329a47c721edb4cd9d275d8513359507655f`**.
- Pre-T-03 integration control: **`23fe4b88827665c9707bfcc36778b8cfe98d5ffe`**. The supplied candidate differs from it only in the accepted T-03 source, verification and handoff surfaces.
- **Final qualified candidate / repair-and-tooling commit: `e998d42eb7c480c17052e19c12bedc71f3762dc8`.** The publication containing this updated handoff is documentation-only; its final branch-tip SHA is recorded in claim #19 and the final report.
- Live `origin/main` read back as `72dbee8b8eab66f58046abfd49e77ead4c29d357`; candidate-contained context commit `63e95ba2175f7e239a278c616681b286719821f8`.
- Read the workflow/context, approved S1-C spec/T-03, continuation, Manager disposition and static reconciliation evidence. Live coordination still says WAITING; the user's later explicit U1/qualification assignment overrides that stale queue row. No canonical coordination file was edited by this qualifier.
- At actual preflight the qualification worktree was clean at `df09329`; U1 was clean/detached at `1d7445e302f31641183549a3e18a8c7282f3c36a`, and the lock was free. Prepared U1 at the supplied candidate under the direct assignment. ZX20 had 21 GiB free, above the 15 GiB floor. No other Editor was running.

## Actual repair and scope

The original 1280×720 **and** 1920×1080 collapsed-harvest captures showed the long “Show recorded details” text extending beyond its button/Work card content. This was a T-03 rendering defect even though the initial bounds-only harness checks passed.

The only production repair is three added lines in `Assets/ForestPrototype/UI/AnnualReviewView.cs`: one explanatory comment, `toggle.style.maxWidth = Length.Percent(100)`, and `toggle.style.whiteSpace = WhiteSpace.Normal`. The label now wraps inside the card. This makes a long toggle taller; it changes no record, grouping, acknowledgement or simulation rule. No pagination/capping was added.

Added T-03 verification only:

- `AnnualReviewDrillDownVerification.cs`: disposable rendered fixture and text-height/containment assertions.
- `run_t03_gate.py`: exact-HEAD/owner/lock launcher for T-03 or the unchanged protected T-02 fixture, plus known import-side-effect restoration.
- `compare_t03_reviews.py`: retains raw comparison and permits only explicitly recognised T-03 addition lines in a separate comparison copy, then invokes unchanged `compare_reviews.py`.
- `T03-RENDERED.md`: execution, comparison and cleanup instructions.

No query-layer repair was necessary. No save/schema, objectives, ecology/economy, Rack/S1-D/MFM, scenes/prefabs, species, package/ProjectSettings, Reference Future v1 or protected T-02 verification source was changed. `protected-source-preservation.json` proves all six audited protected files byte-identical to the pre-T-03 control, including S1-D cycle-state script/meta/accessor and the T-02 harness/launcher/comparer.

## Commands and results

Durable evidence root for every path below:

`/media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/`

From the qualification worktree, the final rendered and protected candidate commands were:

```bash
python3 -B Tools/Verification/S1CManagementHistory/run_t03_gate.py --project /media/jer/ZX20/CCF-U1 --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t03-repair --lock-id 0626a2522ac84f54976f4a7036309462 --expected-head e998d42eb7c480c17052e19c12bedc71f3762dc8
python3 -B Tools/Verification/S1CManagementHistory/run_t03_gate.py --gate t02 --project /media/jer/ZX20/CCF-U1 --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t02-repair --lock-id 0626a2522ac84f54976f4a7036309462 --expected-head e998d42eb7c480c17052e19c12bedc71f3762dc8
python3 -B Tools/Verification/S1CManagementHistory/run_t03_gate.py --gate t02 --project /media/jer/ZX20/CCF-U1 --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t02-control --lock-id 92b657f6302345ca85c45f7fc7335a68 --expected-head 23fe4b88827665c9707bfcc36778b8cfe98d5ffe
python3 -B Tools/Verification/S1CManagementHistory/compare_t03_reviews.py --baseline /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t02-control --candidate /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t02-repair --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t02-comparison --repo /media/jer/Videos/CCF-S1C03Qual --base-rev 23fe4b88827665c9707bfcc36778b8cfe98d5ffe --head-rev e998d42eb7c480c17052e19c12bedc71f3762dc8
```

Lock IDs above are historical, now released; acquire a new normal exact-HEAD lock for any reproduction.

| Gate | Actual result |
| --- | --- |
| Unity 6000.6.0f1 import/compile | PASS at original and repaired candidate; **0 C# compile errors**, no fixture-captured runtime errors |
| Deferred S1-D T-02 import | PASS: `DEFERRED_S1D_T02_CYCLE_STATE_RUNTIME_IMPORT` reads the real `manager.CycleState`; script/meta imported without compilation failure, original GUID `9963eda3e48649438d03b1b5eaa006fd` preserved |
| Final T-03 rendered fixture | **279 PASS / 0 failed / 16 captures**, Editor exit 0, launcher exit 0, 215.94 seconds |
| Protected T-02 at `df09329` | **246 PASS / 0 failed / 12 captures**, Editor/launcher exit 0 |
| Protected T-02 at repaired `e998d42` | **246 PASS / 0 failed / 12 captures**, Editor/launcher exit 0 |
| Protected T-02 at pre-T-03 `23fe4b8` | **246 PASS / 0 failed / 12 captures**, Editor/launcher exit 0; candidate mode, no checks skipped |
| T-03 offline | **72 PASS**, exit 0, both original and repaired candidate |
| S1-C history query | **55 PASS**, exit 0, both original and repaired candidate |
| S1-D T-02 cycle state | **93 PASS**, exit 0, both original and repaired candidate |
| Python syntax / diff hygiene | PASS (`ast.parse` for both new runners; `git diff --check`) |

Offline commands (each with `--out` under the durable evidence root; final folders `t03-offline-repair`, `history-offline-repair`, `cycle-offline-repair`):

```bash
python3 -I -B Tools/Verification/S1CManagementHistory/run_t03_offline_check.py --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/t03-offline-repair
python3 -I -B Tools/Verification/S1CManagementHistory/run_offline_check.py --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/history-offline-repair
python3 -I -B Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py --out /media/jer/Files/CCF/Verification/S1C-T03-Rendered-20261029/cycle-offline-repair
```

The initial original-candidate T-03 attempt reported 263 internal checks and 16 captures, but its launcher received **signal 15 during shutdown** (`unity_exit: null`, process receipt FAIL). Its cause was not established; it is retained as an incomplete process attempt, not counted as qualification. The repaired run above completed normally. Initial evidence also preserves the clipped label. Inherited obsolete-API and native shutdown allocation diagnostics remain in the logs; no candidate C# or fixture runtime error was observed.

## What the fixture actually exercised

Real new-game, manager work/approval/advance and cancellation APIs built five completed years in the production default stand (1,344 starting trees):

1. Twelve fellings, one real grouped settlement: timber €54.52 / harvest €2,500.00. Exactly one named `annual-harvest-settlement`; all twelve item rows have no money of their own.
2. Two planted oaks: one owner-executed with shelter (€5.00 materials, 3 minutes), one contractor-executed (€0.75). Their own recorded values and execution labels render; no harvest settlement.
3. Idle resolution, then cancellation in recorded year 3 using `RemovePendingOrder`; the deleted order is represented by its surviving event only.
4. Two real failure resolutions: after approval the fixture replaces their target IDs in memory before `AdvanceYear`; the manager records both failure reasons. The fixture then blanks only the second event's reason to model a genuinely absent historical field. Both FAILED chips render; the first reason is verbatim, the second says `Reason: not recorded`. This record injection is verification-only and explicitly labelled, not a new gameplay rule.
5. No-work year/latest review; older-year browsing and latest acknowledgement.

Checks cover `work-detail-toggle-*`/`work-detail-*` names, open/close, independent per-year open/closed restoration, reopen clearing every year's presentation, exact settlement values, failed/cancelled attribution and absent-field honesty. T-03 world hash `DAAE4E899C8C82C0`, serialized scenario save data, event/report counts, ecological year, cash and acknowledgement stay unchanged throughout browsing/drill-down. An older-year acknowledgement callback is inert. The real latest callback adds exactly one acknowledgement event and releases advancement.

## Protected T-02 comparison — no rebaseline

`23fe4b8` is the supplied integration **before** T-03, used as a matched control with the same unchanged protected harness; no historical evidence or expected hash was replaced. Every run uses all 246 checks.

- Both control and repaired candidate: world hash **`7073FCB8FEBBDC82`**.
- Raw protected comparison exits 1 as expected: **17 approved T-03 addition lines across seven states** (toggle rows, grouped settlement/attribution note, Money note). The cancellation UI is covered separately by the T-03 fixture; the T-02 fixture contains no cancellation.
- `t02-comparison/raw-comparison.txt` and `approved-additions.json` retain every difference.
- A separate comparison copy strips only recognised additions, then unchanged `compare_reviews.py` passes: **18/18 remaining review texts identical**, no missing/extra states, gate tables identical and acknowledgement-footer predicate identical. **No other movement.** See `protected-comparison.txt` / `result.json`.
- Original `df09329` vs repaired `e998d42` passes the unchanged raw comparer directly: **18/18 texts identical**, same world hash/gates/predicate, exit 0 (`t02-repair-vs-original.txt`).
- Measurement negative control: append one unexpected protected-content line to an external evidence copy. The new comparison rejects it with exit 1 and `text differs: year1`; original evidence/source remain untouched (`comparison-negative-control.txt`).
- Inspected representative paired T-02 control/candidate captures at 720p (harvest year) and 1080p (latest planting year): visible changes are the T-03 additions and their normal layout displacement only.

## Final rendered captures and inspection

All **16** final PNGs in `t03-repair/evidence/shots/` were opened and inspected at their actual 1280×720 / 1920×1080 sizes. Each basename below has both `-1280x720.png` and `-1920x1080.png`:

| Basename | Inspection |
| --- | --- |
| `harvest-collapsed` | Full wrapped toggle; one legible grouped settlement and attribution note |
| `harvest-expanded` | Recorded targets/execution/results/volume; no per-order money |
| `harvest-expanded-scroll` | Twelfth entry and final volume reachable/readable; navigation header remains available |
| `planting-expanded` | Both methods, own cost/material/time, shelter state wrap clearly |
| `planting-expanded-scroll` | Last planting entry reachable; no clipping |
| `cancelled-expanded` | CANCELLED chip, recorded year/location, honest missing execution method and deleted-order explanation |
| `failed-expanded` | Two FAILED chips; verbatim reason and genuinely missing reason legible |
| `failed-expanded-scroll` | Last failure/reason reachable and fully readable |

No observed horizontal clipping, header overlap or lost text after repair. Vertical cropping of offscreen entries is normal scroll viewport behaviour; last-entry captures prove reach. The 12-order year is longer but readable; no evidence justified pagination/capping. `capture-index.json` records dimensions, bytes and SHA-256 for each final capture; `render-review.txt` records inspection.

## Ownership, cleanup and handback

Normal owner/task throughout: `Sol-S1C-T03` / `S1-C-T03`, lane U1, detached exact HEAD. Normal acquire/release receipts are retained for:

- `1859264a3b154715a90917b5afc0ef9c`: original `df09329`;
- `0626a2522ac84f54976f4a7036309462`: repaired `e998d42`;
- `92b657f6302345ca85c45f7fc7335a68`: pre-T-03 `23fe4b8` control.

Each lock was released before a checkout change; no forced release or unrelated process termination. After the control, U1 was returned clean/detached to **`e998d42`**. `lock-final.json`: **free**, no Unity Editor PIDs. `u1-final.txt`: empty status. Qualification worktree was clean before the handoff-only publication; final clean/push receipts are retained with the publication record.

All staged harnesses and generated metas were removed. No verification auto-start script remains under `Assets/ForestPrototype/`. Copied licences were removed, no disk `forest-save.json*` was created, and isolated configuration kept player preferences separate. Unity-import rewrites to shared VS Code settings and three Forb materials were saved as evidence patches and restored to their original bytes. The first T-02 run used explicit restoration; later runs performed the same restoration automatically and report a clean lane. No scene/prefab/ProjectSettings edit remains. Unrelated work was never stashed, reset, cleaned or absorbed.

**Remaining scope limitations:** UI callbacks and renderer/ScrollTo were exercised; OS-native pointer/wheel targeting, disk save/load, full curriculum progression, later S1-C tasks, Tier-3 and Windows are not claimed. No Windows build was made. Manager G2/G3 review of the qualification evidence/tooling remains the next step; no product decision was needed.

**Operational outstanding item:** dashboard ingestion/publisher was not available in this task's tools/workflow. No generated dashboard state was manually edited and no dashboard-publication success is claimed. The Git handoff and claim comment are the published task record; Manager/umbrella #91 owns coordination/dashboard reconciliation.

---

## Historical static handoff (superseded status; retained as provenance)

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

# P3 — Work Plan "What you are leaving" + cash dead-end warning: implementation record

**Branch:** `task/work-plan-residual-stand` · **Worktree:** `/home/jer/CCF-work-plan-residual` · **Base:** `origin/main` @ `1fbefd8a40c69dd90abb83f2149e24b546a184cf` (Regeneration Model 2 integrated, save v18).
**Status:** implemented on the task branch; **Unity verification pending**. Sol held Unity for storms throughout, so no Editor was launched. Verification done: a type-check of the game assembly and the gate with Unity's bundled Roslyn, plus an offline logic check on the .NET runtime.

## Start record

| Check | Result |
|---|---|
| `origin/main` | `1fbefd8` (unchanged since P2 started) |
| Regeneration Model 2 / save v18 on main | Yes (`62593b2`; `ForestSaveData.CurrentVersion = 18`) |
| P2 (`task/crop-tree-competitor-reasoning`) | Pushed checkpoint `a219d7a`; local fixes to `33e8d55`; **not Unity-verified, not integrated**. P3 does not copy P2 code |
| Sol | Storms worktree (`task/storms-windthrow-v1`, uncommitted) edits `WorkPlanView.BuildHarvest` (salvage), `TreeInspectionView`, `ScenarioOneUiRoot`, `AnnualReviewView`, `StandMapView` and others. P3's one-line `WorkPlanView` change merges cleanly with Sol's working copy (`git merge-file`, 0 conflicts) |

## What the player sees (Work Plan, above the job cards)

1. **⚠ CASH AFTER THIS PLAN**: only when the plan would leave too little cash to commission another harvest (below).
2. **MONEY**:
   - expected timber sales and harvesting work, including the small-job minimum;
   - planting / pruning / clearance;
   - net; cash now; expected cash after approved work; "… if all pending work is approved";
   - the contractor-minimum explanation;
   - the remaining spending headroom.
3. **WHAT YOU ARE LEAVING** ("If all open work is carried out. An estimate before this year's growth; Annual Review shows what actually happened."):

| Row | Example (Year 0, T2 plan) | Class |
|---|---|---|
| Trees standing | 336 → 306 (−30) | DERIVED (live trees − open valid Fell orders) |
| Basal area | 41.1 → 36.2 m²/ha | DERIVED (same formula and tree set as the Annual Review snapshot) |
| Standing stem volume | 39.9 → 34.7 m³ | DERIVED (`BiologicalStemVolumeM3`) |
| Removal across the stand | 23 cells · largest opening 1 cell (25 m²) | DERIVED (5 m ecology cells; "opened" = ≥ 30 % of the cell's basal area removed) |
| Pattern sentence | shown only when robust (see below) | DERIVED description |
| Crop Trees kept | 16 of 16 | AUTHORITATIVE (marks; Crop and Fell are exclusive) |
| Trees producing seed | n → m (only when > 0) | AUTHORITATIVE (`GetSeedPotential > 0`, the value that drives seed rain) |
| Fallen deadwood | +x m³ left from felling · y m³ on site | FORECAST (harvest quote) / AUTHORITATIVE (records) |
| Clearance per cell | "removes 2 young-tree groups, 1 planted sapling" / "no young trees inside" | AUTHORITATIVE (`QueryClearance`, the same query the walking preview and execution use) |
| Model 2 clearance note | "Bramble 0.42 · bracken 0.42 cover here, removed this year and able to return. 1 of the young trees removed is already taller than 1.5 m…" | AUTHORITATIVE (`UnderstoreyCells` covers, `CompetitionCalibration.EscapeHeight`) |
| Clearance meaning | `LearningObjectivesView.ClearanceExplanation(model)` (existing, gate-verified, model-aware) | — |

**Not shown, by design (UNSUPPORTED or not owned):** regeneration damage from felling (no harvest-damage model); habitat or "resilience" claims; any score; wind stability (storm model not authoritative on main); the Crop Tree release estimate (P2 not integrated).

## Spatial pattern

The label appears only when **≥ 90 % of 81 nearby threshold choices** agree. The choices vary the opened share (0.25/0.30/0.40), the concentrated cell count (3/4/5), the concentrated share (0.5/0.6/0.7) and the spread share (0.3/0.4/0.5). Otherwise only the facts are shown. Year-0 results:

| Plan | Label | Agreement |
|---|---|---|
| T4 conservative | "Spread out: no part of the stand is heavily opened." | 81/81 |
| T3 heavy | "Openings in several places." (with largest opening 7 cells, 175 m², shown as a fact) | 81/81 |
| T5 concentrated, same volume as T2 | "One concentrated opening." | 81/81 |
| T2 crop-tree release | **no label** | 54/81 |
| T1 clean-up | **no label** | 72/81 |

## Cash dead-end

**Exact condition (from code):**
- Timber sales are the only cash inflow (`SettleHarvestJob`).
- Harvest is contractor-only.
- Every harvest costs ≥ the €2,500 minimum (`MinimumJobAdjustment = max(0, min − variable)`).
- Approval requires cash ≥ full cost; revenue arrives only at settlement.

Therefore a harvest can be commissioned only while cash ≥ `MinimumHarvestJobCents`. Once cash is below it, with no approved harvest still to settle, no harvest and no income can follow.

**Warning (`CashOutlook`):**
- *Expected cash after approved work* = cash − approved non-harvest cost − approved harvest cost + approved harvest revenue.
- *Expected cash if all pending work is approved* adds pending work in the same way, so the plan's own timber revenue counts (case I: no false warning).

| State | Shown |
|---|---|
| Pending work would take expected cash below the minimum | "If you approve the pending work, expected cash after this year is €X: less than the €2,500 minimum for a harvesting visit. Timber sales are your only income in this scenario, so you might not be able to commission another harvest. Review the plan before approving it." |
| Approved work alone does | Same consequence; "Approved work can still be cancelled in this plan." |
| Already below the minimum, no approved harvest | Explains that no harvest can be commissioned; walking, observing and planting owned stock with own time remain possible (shelters cost money) |
| Otherwise | No warning; MONEY shows "You could spend €H more before expected cash falls below one harvesting visit." |

Nothing is forbidden; approval and purchases behave exactly as before. Purchases spend immediately, so the headroom line is the early signal before buying.

## Contractor-minimum teaching

MONEY, when a harvest is planned: "Harvesting has a minimum charge per contractor visit (€2,500 in this scenario). All felling approved for the same year shares one visit, so one visit costs less per tree than several small visits in different years. Planting, pruning and clearance are charged by time, with no minimum." This matches the code: one grouped harvest job per resolution year; hourly pricing with no minimum for other work. It states a cost fact, not a reason to fell more.

## Extension points

`IWorkPlanResidualRows` with `WorkPlanOverview.ExtraRows`:
- **P2:** the Crop Tree release row registers here when integrated, e.g. "Crop Trees with reduced competition: 16 of 16 · competition 6.09 → 4.89 (−20 %)". It is computed by P2's helper, not copied.
- **Storms:** the stability row registers here only when the storm model is authoritative. The old universal "high" wind label is never shown in P3.
- **Post-storm port note:** Sol adds `SalvageDeadwood` harvest orders. When rebasing, `OutlookInput`'s "other cost" and MONEY should treat `ScenarioOneManager.IsHarvestOrder(o)` as harvest. Salvage targets are deadwood, so they correctly stay out of the standing-tree residual.

## Files

| File | Change |
|---|---|
| `UI/ResidualStand.cs` (new) | Pure before/after and spatial-pattern calculation |
| `UI/CashOutlook.cs` (new) | Pure expected-cash and dead-end state |
| `UI/WorkPlanOverview.cs` (new) | Builds warning, MONEY and WHAT YOU ARE LEAVING from manager state; extension interface |
| `UI/WorkPlanView.cs` | **One line**: `WorkPlanOverview.Build(scroll, ui);` before the job cards |
| `.meta` ×3 | New script GUIDs |

Not touched: `WalkingHudView`, `TreeInspectionView`, `MenuHelpView`, the stylesheet (inline styles instead), manager, ecology, economy, save, objectives, Annual Review, tutorial.

## Verification

| Check | Mode | Result |
|---|---|---|
| Game assembly + gate type-check (Unity 6000.6 references, Editor defines) | offline compiler | **0 errors** |
| `P3/OfflineLogic/run_offline_check.py` | offline .NET | **P3_OFFLINE_PASS**: A no work; B/C/D/T2/T1 reconcile with the Unity harness (trees, basal area ±0.02, volume ±0.01), basal-area accounting, pattern labels; no double counting; determinism; G adequate cash; H crosses; I revenue avoids a false warning; approved-crossing; below-minimum-now; approved revenue counted; no warning without cause |
| `WorkPlanResidualVerification` | **Unity batch ×2: NOT RUN** | Read-only Work Plan (world hash), live T2 residual and rendered text, live warning H appears and clears, clearance E/F against `QueryClearance` + Model 2 covers after 4 years, model-aware explanation, determinism hash |
| `WorkPlanResidualVerification` | **Unity interactive: NOT RUN** | MONEY and LEAVING cards fit and no text overflow at 1280 / 1600 / 1920; captures |
| Regression (completion, lifecycle, Reference, economy, Model 2, growth, MenuTutorial, Clearance) | **NOT RUN** | Presentation only; anchors must be unchanged |

### Commands (when Unity is free; one Editor at a time)

```bash
python3 Tools/Verification/ScenarioOneCompletion/P3/run_p3_gates.py
```

```bash
python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py
```

`run_p3_gates.py` refuses to start while any Unity Editor runs. It stages and removes the harness and runs batch ×2 (hashes compared) plus interactive ×1. Results: `Build/P3/results.json`.

### Manual review (human)

1. Mark a realistic thinning; open the Work Plan. Read what is removed, the money, and what remains.
2. Close; change marks; reopen; check the values update.
3. Buy stock until the warning appears; remove the thinning (or stock-dependent work) and see it disappear.
4. Plan a clearance in a cell with young trees (after a few years) and in one without. Read the consequences.
5. Repeat at 1280 × 720, 1600 × 900 and 1920 × 1080.

## Overlaps

- **P1 / P2:** `git merge-tree` reports no textual conflict with either branch. P3 avoids their files.
- **Sol:** clean three-way merge with Sol's uncommitted `WorkPlanView`. Semantic note on salvage above. Re-verify after storms integrate.

## Known limitations

- Unity gates and manual review are pending.
- The headroom line is an aggregate. The nursery's per-purchase buttons do not preview the warning before a purchase (they show it right after).
- Planting-spot clearance (1 m² per planting order) also removes young trees. It is not itemised in v1 (cohort fractions are relative abundance).
- Crop Tree release is deferred to P2's integration through the extension point.

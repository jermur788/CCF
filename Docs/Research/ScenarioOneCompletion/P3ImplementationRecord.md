# P3 — Work Plan "What you are leaving" + cash dead-end warning: implementation record

**Current branch:** `task/work-plan-residual-stand-poststorm`, freshly based on P2 main `869ee92a983a1af5fc470392eccc7557fbe45def` (save v19; RNG1 / Regeneration Model2 / Growth1 / Storm0). P2 was integrated from `task/crop-tree-competitor-reasoning-poststorm` at `869ee92a983a1af5fc470392eccc7557fbe45def` after the required P2 gates passed; remote main was verified at that exact SHA. The P3 source branch `634e4d8` was not merged or replayed; its reviewable files were ported to this clean post-P2 branch and adapted to the integrated APIs.

**Current status:** P3's offline and dedicated Unity checks pass. Its full responsive-render checks pass at 1280×720, 1600×900, and 1920×1080. The broad post-P3 regression set ran all 24 gates: 23 pass, with one unrelated Menu Tutorial/Help Escape failure. The P3 presentation does not change that input path; tutorial behavior is out of scope. This failure is reported separately for Manager review. The pre-integration implementation notes below are retained as historical context and are superseded where they mention P2 or Storm availability.

## Start record

| Check | Result |
|---|---|
| P2 main before integration | `341ccbf1b2877e8bf21c16761b883a0936a5f7b7` |
| Integrated P2 main | `869ee92a983a1af5fc470392eccc7557fbe45def` (remote `main` confirmed) |
| P2 required gates | Completion; Reference Future; Save Hardening; Storm Core (107 checks); Storm UI (1,167 checks): all PASS |
| Storm policy | StormModel0 remains the new-game default; no change in P3 |
| P3 source history | `634e4d806c7296935c23c085b9a3ff146b1565a3`; reviewed as a source only, not merged wholesale |
| P3 test evidence | Offline compile and logic checks PASS; live Unity batch ×2 deterministic; interactive render ×1 PASS at all three sizes. Full-suite result is recorded below. |

## Final verification and handoff

| Area | Result and evidence |
|---|---|
| P3 logic | `Tools/Verification/ScenarioOneCompletion/P3/OfflineLogic/run_offline_check.py`: assembly compile errors 0; A–D residual fixtures reconcile; G–I cash states pass; J confirms `IsHarvestOrder` classifies salvage with felling; deterministic result. |
| P3 live UI | `Build/P3/results.json`: batch ×2 and interactive all PASS; identical hash `F7C2FC966816BBAD`. Read-only empty plan, T2 residual, genuine cash warning and clearing, live Model 2 clearance cases E/F, and P2 competition case K pass. |
| Responsive layout | Interactive log has `P3_RENDERED_PASS` for 1280×720, 1600×900, 1920×1080. Captures are under `Build/P3/evidence/`; 1280 image visually inspected. |
| P2 competition | Final `Build/P2/results.json`: both batch runs PASS, deterministic hash `F58FB0B1A421D28B`. |
| Storm Core | Final `Build/WindthrowV1/StormCoreVerification/result.json`: PASS, 107 checks, production source unchanged. |
| Storm UI / live salvage | Final `Build/WindthrowV1/StormUiVerification/result.json`: PASS, 1,167 checks, production source unchanged. The gate presses X to add/cancel a real salvage order, tests Keep/Sell, approves and resolves a salvage-only job, and checks the annual review and minimum charge. Storm remains off in new games. |
| Broad regression | `Build/WindthrowV1/Regression/results.json`: 24 gates, 23 PASS, 1 FAIL; all 24 production-source snapshots report unchanged. The runner's process exit code is 0 despite the failed gate, so the per-gate JSON is authoritative. |
| Tutorial/Help failure | `Build/WindthrowV1/Regression/MenuTutorialVerification.log`: `MENU_TUTORIAL_FAIL ... Help Escape also closed inspection`, at the harness assertion for Help closing while the player remains inspecting. P3 changes only WorkPlan presentation and does not touch the Help/inspection Escape path. No tutorial behavior was changed. |
| Save and simulation | P3 does not modify save, RNG, regeneration, growth, mortality, storm, economy, or completion production code. P2 save hardening, Reference Future and completion gates passed; StormModel0 stays the new-game default. |

## Separate P1 finding: mixed-species SDI mortality

**Confirmed; not fixed in P3.** In `ForestEcologyController.AdvanceOneYear`, Growth Model1 calls `ApplyAdultDensityMortality()` at line 440 whenever the site-class growth model is active. That method builds its stand from every living tree (`1014–1020`), computes the stand's relative density using every stem (`StandRelativeDensity`, `1092–1102`), then applies Sitka's density-pressure and annual mortality equations to every living tree (`1021–1044`). The boundary self-thinning path likewise removes from this mixed survivor set (`1048–1056`).

`GrowAdults` applies the Sitka site-height equation only to Sitka (`984–985`); broadleaf trees still receive species-specific DBH/height growth but can die under the stand-level Sitka density line. Player-visible consequence: oak and beech may be automatically killed as “self-thinning” and added to deadwood when mixed-stand density crosses the Sitka threshold. Existing `SitkaGrowthModelVerification` tests a synthetic Sitka-only dense stand (`187–205`, mortality at `209+`); no mixed oak/beech adult-mortality assertion was found. Treat this as a separate P1 model-correctness review.

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
| Crop Tree competition | mean competition index now → planned after; number with ≥ 10% modelled reduction | AUTHORITATIVE P2 calculation (`CropTreeCompetition.Summarise`); same-year estimate only, not a growth promise |
| Trees producing seed | n → m (only when > 0) | AUTHORITATIVE (`GetSeedPotential > 0`, the value that drives seed rain) |
| Deadwood volume | x m³ now → about y m³ after selected salvage and planned felling | AUTHORITATIVE current records plus selected salvage; planned felling retention from the harvest quote |
| Clearance per cell | "affects 3 cohort portions and 1 planted sapling" / "no regeneration cohorts or planted saplings inside" | AUTHORITATIVE (`QueryClearance`, the same query the walking preview and execution use); cohort portions are groups, not individual trees |
| Clearance density | relative cohort density before → remaining after the patch | DERIVED from the `QueryClearance` cohort targets and remaining fractions |
| Model 2 clearance note | "Bramble 0.42 · bracken 0.42 cover here, removed this year and able to return. 1 of the young trees removed is already taller than 1.5 m…" | AUTHORITATIVE (`UnderstoreyCells` covers, `CompetitionCalibration.EscapeHeight`) |
| Clearance meaning | `LearningObjectivesView.ClearanceExplanation(model)` (existing, gate-verified, model-aware) | — |

**Not shown, by design (UNSUPPORTED or not owned):** regeneration damage from felling (no harvest-damage model); habitat or "resilience" claims; any score; a wind-stability label (not part of this port). The Crop Tree competition estimate is now shown through P2's authoritative helper; it is not represented as growth.

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

`IWorkPlanResidualRows` remains an extension point for later owned summaries. P3 directly uses P2's `CropTreeCompetition.Summarise` for planned felling. It uses `ScenarioOneManager.IsHarvestOrder` so salvage is included in harvest money and excluded from standing-tree residuals. No universal wind label is shown.

## Files

| File | Change |
|---|---|
| `UI/ResidualStand.cs` (new) | Pure before/after and spatial-pattern calculation |
| `UI/CashOutlook.cs` (new) | Pure expected-cash and dead-end state |
| `UI/WorkPlanOverview.cs` (new) | Builds warning, MONEY and WHAT YOU ARE LEAVING from manager state; extension interface |
| `UI/WorkPlanView.cs` | Adds `WorkPlanOverview.Build(scroll, ui);` before the job cards while preserving P2's integrated salvage flow and controls |
| `.meta` ×3 | New script GUIDs |

Not touched: `WalkingHudView`, `TreeInspectionView`, `MenuHelpView`, the stylesheet (inline styles instead), manager, ecology, economy, save, objectives, Annual Review, tutorial.

## Pre-integration verification plan (historical; superseded)

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

## Overlaps (historical notes)

- **P1 / P2:** `git merge-tree` reports no textual conflict with either branch. P3 avoids their files.
- **Sol:** clean three-way merge with Sol's uncommitted `WorkPlanView`. Semantic note on salvage above. Re-verify after storms integrate.

## Known limitations

- The 24-gate regression includes one Menu Tutorial/Help Escape failure; P3 does not alter that input path. Human keyboard/mouse acceptance of P2 remains pending.
- The headroom line is an aggregate. The nursery's per-purchase buttons do not preview the warning before a purchase (they show it right after).
- Planting-spot clearance (1 m² per planting order) also removes young trees. It is not itemised in v1 (cohort fractions are relative abundance).
- Crop Tree competition release is shown through P2's existing `CropTreeCompetition.Summarise` helper.

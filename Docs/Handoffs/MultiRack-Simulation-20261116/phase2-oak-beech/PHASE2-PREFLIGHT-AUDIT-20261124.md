# MULTI Phase 2 (Oak & Beech) — Pre-flight audit and Step A handoff

```text
RESEARCH / SPEC HANDOFF
Task:            CCF Phase 2 Sessile Oak & European Beech — Step A (pre-flight audit + DRAFT spec quartet)
Worker:          Claude Code (Opus 5.5), design sub-agent
Role:            Primary author, Step A only
Branch:          docs/multi-phase2-oak-beech-spec-20261124-v1
Base:            origin/feat/multi-stage5-sitka-regeneration-20261123-v1 @ 5b757d9b4a353d07557db95d3af9227c8d841874
Context commit:  5b757d9b (origin/main at drafting: 37ceb264b5431fe254d67e5702af39bf66cddc1d)
HEAD:            see the commit that adds this file
FILES CHANGED:   specs/multi-phase2-oak-beech-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}; this file
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none (docs only)
SIMULATION / SAVE IMPACT: none (docs only)
MUST-NOT-CHANGE HASHES: all (§8) — no code changed
VERIFICATION RUN: read-only source inspection with git/grep on the base tree; no build, no tests, no Unity
SPEC CONFORMANCE: not applicable (Step A)
Ready for review: Yes — DRAFT QUARTET PUBLISHED, AWAITING OWNER APPROVAL
RECOMMENDED NEXT STEP: owner answers spec §14 Q1–Q6 and marks the quartet APPROVED; only then Step B
```

Labels used below: **[V] verified** (read in the cited file at `5b757d9b`), **[A] assumption**, **[P] proposal**. Unity paths are under `Assets/ForestPrototype/` and are byte-identical at `37ceb264` (`git diff --stat 5b757d9b 37ceb264 -- <cited Unity files, Species/>` is empty) [V].

## 1. Stage 5 clearance on the record

- [V] Stage 5 `5b757d9b` is APPROVED on a delta-only basis (`e8f47e73`) — as supplied in the task packet.
- **Owner acceptance recorded (2026-11-24):** both outstanding conditions are accepted **as written**:
  - **R1** — `S5-07A` certifies the matched population in aggregate only; matched edge blocks are not edge-consistent under Option A.
  - **R3** — the `S5-02` margins stand.
- Step B's Stage 5 gate is therefore satisfied on the record.

## 2. Shelter implementation

- [V] `BrowsingConditions.cs:56` `effectiveYears = 8`; `:58` `failedYear = -1`; `:60` `MatchRadiusM = 0.25f`.
- [V] `BrowsingConditions.cs:62-64` `IsEffective(year) => year >= installedYear && year < installedYear + Mathf.Max(0, effectiveYears) && (failedYear < 0 || year < failedYear)`. Expiry is **strict `<`**: effective `installed … installed+7`; `installed+8` expired. **Not `<=`.**
- [V] `BrowsingConditions.cs:135-174` `ProtectionAt`: effective shelter within 0.25 m → `access = 0f`; else a point inside an intact fence → `0f`; otherwise `access = 1f`. Access is **binary**. Failed state: `failedYear >= 0 && year >= failedYear` → `FailedShelter` (`:148`), access 1.
- [V] `BrowsingConditions.cs:176-191` `CohortAccess`: fences only, 5×5 supersampling; "Shelters do not apply to cohorts".
- [V] `ScenarioOne/ScenarioOneManager.cs:1908-1914`: shelter created with `shelterId = "S-J" + juvenile.juvenileId`, `installedYear = report.year`, `effectiveYears = 8`, `failedYear = -1`.
- [V] No code sets `failedYear` other than creation: other references are save validation (`ForestSaveValidation.cs:307`), diagnosis (`RegenerationDiagnosis.cs:241-242`) and visuals (`ScenarioProtectionVisuals.cs:68,129`). No stochastic failure mechanism exists.
- **Owner decision:** port `FailedYear` faithfully, never set it; shelter failure out of Phase 2 scope; no invented probability.

## 3. Species constants (spot-verified)

- [V] `Species/SessileOak.asset`: `growingSpaceDemand: 2`, `crownRadiusIntercept: 0.1`, `crownRadiusPerCmDbh: 0.1045`, `usePowerLawCrown: 0`, maturity `40/60`, dispersal `4/80`, mast `0.15/0.35`, multipliers good/normal/poor `1/0.3/0.075`, `seedSaturationS50: 5`, `useDistinctJuvenileLightResponses: 1`, `promotionMinimumLight: 0.2`, `regenInitialHeightM: 0.18`, `regenDensityMax: 1.5`, `browsePalatability: 1`, browse heights `1.2/1.8`.
- [V] `Species/BeechSpecies.asset`: `growingSpaceDemand: 1`, `usePowerLawCrown: 1` (serialized linear fields `0.9415`/`0.07635` present but unused), maturity `40/60`, dispersal `6/50`, multipliers `1/0.4/0.1`, `S50: 5`, `regenInitialHeightM: 0.15`, `browsePalatability: 0.5`, browse heights `1.2/1.8`.
- [V] Mast multiplier order is good / normal / poor (`ForestEcologyController.cs:1285-1290`).
- [V] Seed potential `maturity · clamp01(CrownRadius/4) · seedPotentialPerMatureTree · mast` for all species (`ForestEcologyController.cs:1340-1354`).
- [A] Growth (`Pd`, `Ph`, `Dmax`, `Hmax`, `Ci50`, `k`) and light-table values are taken from the task packet and not re-read in this audit; Step B Stage 1 re-reads them against the assets before coding.

## 4. Annual step order

- [V] `ScenarioOneManager.cs:1346-1357`: approved work and settlement, then `ecology.AdvanceOneYear()`, then `AdvancePlantedJuveniles()`, `RefreshPlantingMarkers()`, `AdvanceUnderstorey()`, `AdvanceDeadwood()`.
- [V] `ForestEcologyController.cs:471-498`: storm → competition → canopy → adults → crown relaxation → density mortality (batched, growth model ≥1) → canopy → Model 2 `PrepareCompetitionForYear` → regeneration growth → mast → seed rain → establishment → promotion → suitability → exposure decay → log.
- [V] `ScenarioOneManager.cs:1989-2056` `AdvancePlantedJuveniles`: ordinal `juvenileId` order; `age += 1`; vegetation roll key `{id}/vegetation`; browse via `AssessIndividualBrowse` on pre-growth height and `RealiseBrowse` key `{id}/browse` (`JuvenileEcologyRules.cs:126-127`); growth and survival read **`cell.Light`** (the containing ecology cell), survival key `{id}`; promotion `CanPromote = height >= PromotionHeightM && light >= PromotionMinimumLight` (`JuvenileEcologyRules.cs:33-34`), id `"PL-" + juvenileId`, `DBH = clamp(1.5·H, 2, 20)` (`:37`), age `max(1, RoundToInt(ageYears))`, initial crown `species.PotentialCrownRadiusM(dbh)` (open-grown); no canopy recompute.

### Audit qualification A-1 (new finding)

- [V] Under `RegenerationModel.Competition`, bramble/bracken covers of cells **and** planting patches are stepped in `PrepareCompetitionForYear` (`ScenarioOne/ScenarioOneCompetition.cs:15-41`), called at ecology step 5b — **before** regeneration growth and before `AdvancePlantedJuveniles`.
- [V] `AdvanceUnderstorey` (`ScenarioOneManager.cs:2536-2542` → `ScenarioOneUnderstorey.Advance`) steps only habitat groups (mosses, ferns, grasses, forbs, shrubs, fungi), guarded by `lastUpdatedYear`.
- Consequence: the packet's "planted juveniles read the pre-recovery understorey" is accurate for the habitat groups, but **planted juveniles' vegetation-loss roll reads bramble/bracken already stepped for the current year**. [P] Phase 2 reproduces the Model 2 behaviour (spec §4, Q4).

## 5. Clearance geometry and nursery defaults

- [V] `ScenarioOne/VegetationClearance.cs:14` planting footprint radius `Mathf.Sqrt(1f / Mathf.PI)` = 0.5641896 m (area 1.0 m²). `PlantedJuvenile.cs:33-41` patch fields `center`, `radiusMeters 0.564`, `createdYear`, `brambleCover`, `brackenCover`, `competitionUpdatedYear`.
- [V] `ForestEcologyController.cs:31` `plantedJuvenileAgeYears = 3`; `:33` `plantedJuvenileHeightM = 0.6f`.
- [V] Unity clearance is **destructive** to cohorts: `ScenarioOneClearance.cs:8-40` `QueryClearance` removes cohort density in proportion to the cleared fraction of each cell.
- [V] Unity area-weights patches into cohort `CellPressure` (`UnderstoreyCompetition.cs`, `CompetitionExposureCache`). [P] Not ported (spec §7.4, Q2).
- [A] Patch `competitionUpdatedYear` at creation equals the execution year, so a new patch holds 0 until its first annual step; to be confirmed in Step B Stage 1.

## 6. Spacing rules — honest labels

- [V] **0.5 m is a TRANSFER**: `ScenarioOneManager.cs:2198-2203` refuses when another open `PlantJuvenile` order with a **lower `workOrderId`** is `< 0.5f` away ("Another marker occupies this planting position").
- [V] **0.7 m from a living adult stem does not exist anywhere in Unity.** It is a **new owner rule**, not a transfer.

## 7. Engine structure

- [V] `Tools/MultiRackSitkaP1Lab/src/Core/Stage4Models.cs:45-46` `MultiAreaModelSelection(Adult, Mortality, Crown, Regeneration = Off)`; `:77-89` `RequireSupported()` allow-lists exactly `Legacy`, `Stage3CrownC2`, `Stage4`, `Stage5`. A fifth defaulted field is structurally the same change as `Regeneration`; byte-identity at `Species = Off` is achievable.
- [V] `MultiAreaContinuation.cs:134-135` + `Stage5Clearing.cs:48`: the regeneration version is appended to `AreaModelVersion` only when `!= 0`.
- [V] RNG keys: `AnnualRules.cs:87` `"ADULT-MORT-" + TreeId`; `Stage5Mast.cs:11` `STAGE5-MAST`; `:15` `STAGE5-PROMO-{areaId}-{column}-{row}-{year}`.
- [V] `Stage4Records.cs:17-30` Sitka pruning 3 lifts / 5 years / 0.92; `:79-87` deadwood volume `DBH²·0.00007854·(H·0.5)`.
- [V] Unity broadleaf pruning `ForestTree.cs:193-217`: Oak/Beech 2 lifts, 3 years, target must be exactly 3.5 m then 6.0 m, 60 % guard (Teagasc-guided targets; interval and 8 % are `[D]` calibration).
- [V] Unity two-part mixed density exists and is the transfer source: `MixedDensityModel.cs` (weights 1.0/1.5/2.0, normaliser 55, victim `Ci50 = 5`, exponent 4), `MixedDensityModel.Composite.cs` (`G(O)` 0.5/1.60/2, BA budget 0.125, 1-stem fallback, quota remainder key `~generic-quota`, exponential-race victim draw keyed per tree), `SitkaSubsetDensitySignal.cs` (Sitka component on Sitka stems alone, per-tree `ADULT-MORT-` key).
- [V] `SpeciesProfile.cs:34` holds only `sitka-spruce`.

## 8. Protected anchors (must not change)

| Anchor | Hash |
|---|---|
| single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| multi `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| multi `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| P0 Core binary pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |

- [V] `c4b670ed…` is **printed, not pinned**: no occurrence under `Tools/MultiRackSitkaP1Lab/tests/`; recorded in `stage5-regeneration/MULTI-STAGE5-SITKA-REGENERATION-REPAIR-20261123.md:158,174` ("recorded, not pinned").
- [P] Step B promotes it to a pinned constant — a **declared test change** in the quartet scope (closes reviewer nit R8).

## 9. Scope honesty

- [V] Unity `ResolvePlanting` consumes stock, cash, owner-minutes and a `stockItemId` offer identity (`ScenarioOneManager.cs:1906-1907` shows `stockItemId = order.stockItemId`).
- Phase 2 `Plant`/`Shelter`/`SpotClear` carry **zero economy and zero stock**; they borrow geometry, nursery defaults and browse rules only. No parity is claimed.

## 10. Not verified

- No build, test or runtime was run (docs-only Step A).
- No Unity Editor, import, scene or rendered verification.
- Growth and light-table constants in spec §3 come from the packet and were not each re-read here (§3 [A]).

## 11. Decisions needed (owner)

Spec §14 Q1–Q6 (token, patch exposure for cohorts, order shapes, bramble/bracken timing, mixed fixture, pure-Sitka equivalence), each with a recommendation.

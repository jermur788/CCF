# Storms / windthrow / salvage v1 — implementation roadmap

**Status:** proposal. **Start only from current `origin/main` after Regeneration Model 2 integration** (Sol's `task/understorey-recruitment-causality`, expected save v18).

## 1. Sequence

| Step | Content | Owner type | Gate |
|---|---|---|---|
| 0 | Decisions #1, #4, #5, #6, #12, #17 (`DecisionMatrix.md`) | Manager/user | before W1 |
| W1 | **Storm core**: `StormModel` versioning, save field + `stormEvents[]`, Layer 1 + Layer 2 resolver in the annual step, batched mortality, windthrow `RecentOpening` increments, deadwood records via the existing handler, deterministic rolls | ecology owner (Sol) | stormModel 0 anchors identical; determinism two-process |
| W2 | **Calibration matrix** (`LongRunCalibrationPlan.md`), tune `S_class` / `siteWindHazard`; record new anchors | ecology owner | targets + sanity checks |
| W3 | **Visuals**: base-anchored directed logs, root plates, crown Option A with distance limits, soil pit asset, rebuild on load | presentation owner (with asset worker for the pit) | rendered review; performance fixtures |
| W4 | **Salvage**: work order type, world marking (X on windthrown stems), Work Plan section, quote via harvest job with `WindDamage` sections and `SiteCostBasisPoints`, events | management/economy owner | economy gates; anchors for salvage fixtures |
| W5 | **Player feedback**: stability bands replacing the wind label, marking-forecast wording, Annual Review storm section, optional map layer | UI owner | MenuTutorial (interactive) + copy gate |
| W6 | **Tutorial lessons** (`TutorialHandoff.md`) | pedagogy owner | copy gate |

W1 → W2 must be sequential. W3/W4/W5 can run in parallel after W2 **only if** file ownership is split (below). W6 comes after W5.

Recommended: run **W1 + W2 as the Sol implementation packet** (`SolImplementationPacket.md`); W3–W5 as follow-up packets.

## 2. File ownership (likely)

| File | W1 | W2 | W3 | W4 | W5 |
|---|---|---|---|---|---|
| `ForestEcologyController.cs` (annual step, resolver, vulnerability, `GetWindRisk` redirect) | ● | ● (constants) | ○ | ○ | ○ |
| new `StormModel.cs` / `StormVulnerability.cs` (pure functions, unit-testable) | ● | ● | ○ | ○ | ○ |
| `ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation.cs` | ● | | | ● (enum use) | |
| `ScenarioOne/ScenarioOneDefinition.cs` + `ScenarioOne.asset` (storm parameters; **serialized**) | ● | ● | | | |
| `ScenarioOne/ScenarioOneManager.cs` (death handler cause-awareness, storm events, salvage orders) | ● (events) | | ● (visual spawn) | ● | ○ |
| `ScenarioOne/ScenarioFallenLogVisual.cs` (+ new root-plate/crown visual) | | | ● | ● (removal) | |
| `ScenarioOne/ScenarioOneEconomyAdapter.cs`, `WorkEconomy/*` | | | | ● | |
| `UI/*` (`TreeInspectionView`, `WalkingHudView`, `AnnualReviewView`, `WorkPlanView`, `StandMapView`) | | | | ● (Work Plan salvage card) | ● |
| `ForestTreeMarkingManager.cs` (forecast wording; X on windthrown stems) | | | | ● | ● |
| `Assets/ForestPrototype/Art/…` (pit), prefabs, catalog | | | ● | | |
| `Tools/Verification/WindthrowV1/*` (harnesses) | ● | ● | ● | ● | ● |

● writes, ○ reads. **Hot spots:** `ScenarioOneManager.cs` (W1, W3, W4) and `UI/*` (W4, W5). Sequence those writers. Pedagogy P1 (`task/scenario-one-pedagogy-p1`) also edits `UI/*` and `ForestTreeMarkingManager.cs`: integrate it before W4/W5.

## 3. Integration points with new ecology (packet item 10) — do not hard-code until landed

Chain: **windthrow → canopy opening → light change → competitor vegetation response → regeneration response.**

| Point | Storm provides | Sol's ecology (expected) consumes | Rule |
|---|---|---|---|
| Canopy/light | victims removed → `RecomputeCanopy` (existing) | Light-driven bramble/bracken cover growth; juvenile growth | No storm-specific code: reuse cell light |
| Opening | `RecentOpening` increments | Establishment suitability (existing dip); possibly competitor colonisation (`ScenarioOneUnderstorey` already reads `RecentOpening`) | Keep `RecentOpening` semantics unchanged (still +1 per removed tree, cap, decay) |
| Deadwood | records (cause windthrow) | Habitat/deadwood indicators (existing) | none |
| Clearance after storms | — | Regeneration Model 2 clearance/recovery state | The player may clear storm gaps; no special case |
| Microsites (pits/mounds) | — | future | Not in v1. If Regeneration Model 2+ adds seedbed variables, a storm could set them: a **future decision** |
| Juvenile damage by falling stems | — | future | Not in v1 |
| Save | `stormModel`, `stormEvents[]` | v18 fields (competitor cover, model 2) | The storm takes the next version after v18. Both models must be independently versioned |
| Calibration | storm matrix | understorey response metrics | Re-run the storm calibration if Regeneration Model 2 calibration changes afterwards |

## 4. Risks

| Risk | Mitigation |
|---|---|
| Storms make Year-25 completion fail (`retained-canopy ≥ 60`) | Calibration target ≥ 90 % viability; grace years; objective review only if needed (decision #22) |
| Deadwood visual population (Growth Model 1 + storms) | Measure the Growth Model 1 baseline first; distance limits; instancing |
| H/D from Growth Model 1 is high (83–86 at age 30–40), so storms may bite hard mid-scenario | Intended. Calibrate S with that stand, not the old stouter one (Sol's own warning in `HeightCandidateComparison.md`) |
| Player perceives storms as punishment | Disturbance framing, salvage choice, regeneration follow-up, review wording |
| Overlap with Sol's deadwood asset packet 05 (silhouette/decay variation) | Coordinate: the long fallen stem brief and packet 05 should be one asset task |

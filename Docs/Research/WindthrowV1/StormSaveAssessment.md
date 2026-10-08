# Storm save assessment

**Status:** assessment only. **No schema change made.** Main is v17 at `a8596df`. Sol's understorey / Regeneration Model 2 work may land as **v18**, so storms would be **v19** or whatever follows. The storm packet must take the next free version at its start.

## 1. What existing saved data already provides [REPO]

| Need | Already saved | Where |
|---|---|---|
| Which trees were windthrown and when | **Yes**, given a cause string | `TreeSaveData.biologicallyDead`, `mortalityCause` ("windthrow"), `mortalityYear` (v14+) |
| Fallen stem record (volume, position, size, decay) | **Yes** | `ScenarioDeadwoodRecord` (management layer; created by the existing death handler under growth model ≥ 1) |
| Link from deadwood to cause | **Derivable** | `ScenarioDeadwoodRecord.treeId` → tree's `mortalityCause` |
| New exposure after storm | **Yes** | `ForestCellSaveData.recentOpening` |
| Light/canopy | Derived on load | `RecomputeCanopy` |
| Salvage orders and history | **Yes**, with enum additions | `ScenarioOneWorkOrder` (type, target, outcome, status, year); `ScenarioManagementEvent` |
| Salvaged state of a stem | **Derivable** | Record removed on salvage, plus the `WorkResolved` event |
| Annual damage totals | Derivable | trees with cause/year; records; snapshot deadwood fields |
| RNG / model state | Yes | `rngModelVersion`, `growthModel`, `regenerationModel`, `simulationSeed` |

## 2. Minimum additions

| Field | Why it cannot be derived | Proposal |
|---|---|---|
| **`stormModel` (int, root of `ForestSaveData`)** | Determinism and compatibility: an absent field must mean "no storms" so v ≤ 18 saves and Reference Future v1 replay unchanged | **Required.** Same pattern as `growthModel`: missing/0 = none; new games = 1 (product decision) |
| **`scenarioOne.stormEvents[]`**: `{ year, severityClass, directionDegrees, victims, volumeM3 }` | Severity and direction *could* be re-derived from the rolls, but (a) storms with zero victims leave no other trace, (b) history/UI must not depend on re-running the roll code, (c) fall directions of existing logs must stay stable if the roll code is later revised under a new model version | **Recommended** (small: about 5 numbers per storm, a handful per game) |
| Fall bearing per log | Derivable: storm event direction + hash(treeId) | **Not needed** |
| `windthrow` vs other death cause on the deadwood record | Derivable via `treeId` | **Not needed** (optionally cache it at runtime) |
| Salvage eligibility / value state | Derivable: decay class + years since the storm event | **Not needed** |
| `windsnap` break height | v1.1 only | Not in v1 |
| Per-tree exposure history | Derivable from events + `RecentOpening` | **Not needed** |

**Enum additions (int-serialized; old saves unaffected):**

- `ScenarioWorkType.SalvageDeadwood`
- `ScenarioEcologicalTreatment.WindthrowSalvaged` (and optionally `WindthrowRetained`)
- optionally `ScenarioManagementEventType.StormOccurred`. Either use this **or** `stormEvents[]`, not both. Recommendation: `stormEvents[]` in the scenario block, because the event list is management history and the storm is not a management act.

**Definition fields** (`ScenarioOneDefinition`: storm probability, severity, direction, site hazard, sides) are asset data, **not save data**. Changing them changes future storms but not saved history.

## 3. Validation (`ForestSaveValidation`)

- `stormModel` ∈ [0, latest].
- `stormEvents`: years ≤ current year, unique years, finite non-negative volumes, class enum defined, direction in [0, 360).
- Consistency (warning, not failure): every tree with cause "windthrow" has a storm event in its death year.

## 4. Load/restore rules

- `stormModel` restored before any annual step. An absent field means 0.
- Reference Future v1 archive and previews: model 0 always. The archive is never re-serialised (D-042).
- Visual rebuild on load: root plates and directed logs are rebuilt from deadwood records + tree cause + storm events (no visual state saved), like current log rebuilds.

## 5. Anchor consequences

- `stormModel = 0`: **byte-identical** behaviour. Every existing anchor must reproduce (growth model 1 lifecycle `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, completion `D7C4DDD36B53FCCE`, legacy and Reference anchors). The JSON world hash changes because a new field exists, even at 0. Follow the D-047/D-048 practice: a legacy-layout hash helper, or explicitly re-recorded layout anchors.
- `stormModel = 1`: new anchors (lifecycle, completion), recorded by the storm packet.

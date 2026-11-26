# CCF Multi 2D — Phase 2B-1 Requirements Checklist

| ID | Requirement | Verification |
|---|---|---|
| **P2B-01** | `EconomyModelVersion { Off = 0, Enabled = 1 }` defaults to `Off`; `MultiAreaRoot.Economy` is last and `null` when Off; all 9/9 frozen anchors and Save v2 golden are unchanged. | Test 1 (`economy: Off is inert`), Core tests, Save tests, G3/G4 |
| **P2B-02** | When Off, zero financial state mutates and history money fields read `NOT RECORDED` (never `€0`). | Test 1 (`economy: Off is inert`) |
| **P2B-03** | Quote is pure, read-only, deterministic, and bound to `PlanStateRevision`. | Test 2 (`economy: quote is pure and revision-bound`) |
| **P2B-04** | Approve freezes the quote immutably into `root.Economy`; later re-quotes cannot alter it. | Test 3 (`economy: approve freezes the quote`) |
| **P2B-05** | Execute settles atomically under the same `sync` lock and single `root` assignment; replaying the execution key returns the stored receipt with settlement and never re-settles. | Test 4 (`economy: execute settles atomically and replay never re-settles`) |
| **P2B-06** | Upfront cash covers external costs before approval/execution; expected timber revenue never pre-finances; balance never negative; no debt. | Test 5 (`economy: upfront cash and no debt`) |
| **P2B-07** | 40.0 h/year landowner allowance depletes on Execute and resets on `Advance` via `ReservedPhase2BYearEndSummaryStep`; `Fell` is contractor-only (`EconomyFellContractorOnly`). | Test 6 (`economy: 40h landowner allowance and Fell is contractor-only`) |
| **P2B-08** | Grouped contractor harvest job priced by engine-derived selective CCF thinning stage (`FirstThinning` €20/t, `SecondThinning` €23/t, `LaterThinning` €20/t; clearfelling not modelled; no player stage override) with €2,500 minimum floor per executed felling Work Plan; two plans incur two floors. | Test 7 (`economy: grouped harvest floor per felling plan`), Test 9 (`economy: CCF selective thinning only (no override, auto-derived stage)`) |
| **P2B-09** | Deterministic 0.64 ha (80×80 m, 5-lane, 1,344-stem) calibration stand with €12,000 opening cash; D-062 Oak/Beech/Sitka assortment ladders and 890 kg/m³ proxy conversion; Beech has no veneer/beam/fencing; pruned Oak gains veneer eligibility without unit price change; Sitka unchanged. | Test 8 (`economy: 0.64 ha calibration and D-062 assortment`) |
| **P2B-10** | Save v2 refuses with typed `SaveEconomyRequiresV3` when Economy is Enabled; zero `IFeatureBlockCodec` implementations and zero `src/Core/Phase1/**` edits. | Test 1 (`economy: Off is inert`), Save tests, G4 |

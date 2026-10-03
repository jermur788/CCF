# Scenario 1 Management / Economy Integration

Authorised primary: OpenAI/OpenCode. Shared base explicitly confirmed by user:
`9238e5673d9ee24f245df916982bd6228c225a5a`, branch
`task/scenario-one-economy-loop`, worktree `/home/jer/CCF-opencode-economy`.
Locked context read at `db2b4f1c3c9eb5740709bf7eae9316b7a54f6eb8`.
Remote main anchor `bb314a4ebe495fb0d186b185cb7d63d1e20629a8`.

## Foundation adoption

Clean cherry-picks, no conflicts:

| Original | Adopted |
|---|---|
| 66bdd4d WorkEconomy | 9fa8c7b |
| 5af31df TimberYield | 6380335 |
| 5eb508d pruning diagnostic | a7685b0 |

These foundations are reused, not rebuilt. Adoption verification and live
integration results are recorded below as work completes.

Adoption import/compile and both Unity foundation gates passed:
economy 66 fixtures/420 assertions, SHA
`05E5E664EE3D8F74D20D76B9E2629C48C6D30FDB8F955A5E9FA43228C8AB62E1`;
timber 120 fixtures/3,007 assertions, SHA
`88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.

## Phase 0 browsing branch review

Read-only review of `2ca8c32`, `e8e9486`, `d77f0c5`, `1be675c`, `4dc6713`,
including production diff, protection API, docs and verification. The shared
base code tree is byte-identical to 4dc6713; commits were cherry-picked onto main.
Claude's subsequent ecology-completion work remains separately owned.

### MUST-FIX

- No blocking defect found in the five-commit production browsing path.
- **Final integration harness coordination:** the browsing fixtures install
  shelters/fences before `manager.RestoreSaveData(state)` (e.g. matrix/setup and
  timing fixtures). Required v15 replacement restoration clears those external
  records. Claude must adapt its owned fixtures to put protection into saved
  state or install it after restore. Do not change production replacement
  semantics or biological assertions to preserve outdated fixture setup.

### SHOULD-FIX

- Save v15 validation must reject malformed/nonfinite protection geometry,
  duplicate IDs and impossible shelter years before live mutation. Public
  protection DTOs are intentionally simple and frozen; validation belongs here.
- The earlier browsing document's *proposed* v15 fields include pressure,
  browse/form history. This packet explicitly excludes them. Do not implement
  that obsolete proposal; the current bounded v15 field list wins.

### NOTE

- Zero browsed fraction delegates to the unchanged neutral rule; browse draws
  use `juvenileId + "/browse"`. Preserve juvenile identity and annual ordering.
- Scenario pressure is definition configuration 0.2, applied after ecology
  Awake by ScenarioOneManager.Start; it is not saved or reset by restoration.
- Shelter working interval is `[installedYear, installedYear + effectiveYears)`.
  Claude's timing fixture verifies installation at **planting resolution year**
  (`report.year = EcologicalYear + 1`), protecting that first annual juvenile step.
- Protection replacement must happen on every restore, including old schemas,
  scenario reset and reference-preview enter/exit. Pressure remains configuration.
- Shelters affect exact-position individuals within 0.25 m, not cohorts. Existing
  planting spacing is 0.5 m, so normal designated plants do not share one shelter.
- Normal 0.2 and neutral anchors are `3485B6630C9EA448` and `BFC55473C1506067`.
  Review is inspection; author-reported browsing runs are not claimed as new runs.

## Integration design / exact existing owners

| Work | Mapping | Existing authoritative effect |
|---|---|---|
| FellTree | One annual commissioned Harvest job, pre-fell snapshots → TimberYield → volume batches → WorkEconomy | Existing ResolveFelling calls ForestTree.Fell and existing deadwood/residue presentation once; one grouped finance settlement after effects |
| PlantJuvenile | ExistingStock nursery requirement; Contractor or LandownerSimulated; optional shelter task/material, same executor | Existing cohort/exact planting and clearance logic first; shelter created only after successful exact planting; settle once |
| PruneTree | Required scope keeps legacy pricing/resolution | Existing TryPrune and legacy charge, no second accounting path |
| RemoveRegeneration | Required scope keeps legacy pricing/resolution | Existing removal and legacy charge |

Quote lifecycle: group all open felling orders for Work Plan; reserve the approved
subset as one job; at annual resolution discard invalid targets and re-quote the
valid approved remainder. Empty remainder costs nothing. Upfront cash is reserved
once, not per tree; future receipts do not prefinance work. The next annual owner
budget is 2,400 min [C], with approved/pending reservations checked in stable order.

Execution selection is stored on orders. Planning category defaults/toggles are
transient UI state; approved changes require renewed approval. Harvest is
contractor-only. Shelter is coupled to planting and shares its executor.

Save v15 bounded fields: order executionMethod/installShelter/harvestJobId;
scenario shelters/protectedAreas/ownerMinutesUsedThisYear; report minimum
adjustment, owner minutes, retained volume and per-product sold volume/revenue;
event executionMethod/ownerMinutes. No pressure, browse history/form damage or
additional unapproved save fields. Derive harvest intervention count from existing
successful felling event years rather than adding a counter.

Annual review adds bounded economic lines and the literal
`// INTEGRATION: ScenarioEcologyReviewLines` hook. Existing objective thresholds,
starting cash, live nursery prices and definitionVersion remain unchanged.

## Calibration conventions

- Production contractor minimum stays €2,500 [C]. Sensitivities are diagnostic only.
- Biological stem volume and grading/pricing use over-bark game basis [S], not
  measured industrial truth. No broadleaf sale deck or fabricated pulp revenue.
- KeepForUse is contractor-forwarded roadside, no retained haulage, volume
  deposited once. Deadwood never enters retained roadside stock.
- Report supplies no actual Irish retail shelter price (the €2.56 figure is a
  scheme-derived allowance, not cost). Use an explicitly labelled new Scenario
  One material placeholder, never a grant-as-invoice; document the selected value.
- Purchased nursery stock is consumed as existing stock, never purchased twice.
- Owner labour cash wage and opportunity cost remain €0 [S]; time is a resource.

## Progress / blockers

Required implementation complete and verified on the task branch. No production
minimum, starting cash, nursery price, objective threshold, biology or reference
contract was retuned. No main merge/push. Final integration must coordinate the
Claude-owned fixture setup and ecology formatter described above.

## Live implementation conventions

- Added work-only `UnpricedHarvestGreenGrams`/`UnpricedForwardGreenGrams` to the
  pure ForestryTask input. Zero defaults preserve all original foundation
  behaviour. No market assortment, price or sale is invented for these quantities.
- Scenario work uses the whole pre-fell over-bark volume, converted by the
  configured fresh-volume work-equivalent proxy (890 kg/m³ at the adopted deck).
  For broadleaf/ungraded material this is explicitly **[S] work equivalent**, not
  a claimed measured broadleaf density or broadleaf timber price. The existing
  combined harvest/forward service rate prices the commissioned job; this does
  not assert deadwood is physically extracted. No separate empirical rate split
  is invented. All retained/deadwood material dispositions remain world-owned.
- KeepForUse/deadwood retain the existing whole-stem volume convention, quantized
  to cm³ for settlement. Sold Sitka only uses recovered graded log quantities.
  Source budget buckets (sold/kept/deadwood/residual) reconcile exactly; the
  underlying TimberYield result independently conserves its sections.
- Shelter material is **€5 per item [D]**, configurable new placeholder; it is
  not sourced as a retail Irish quotation and not the €2.56 grant allowance.
- Planting uses existing stock, foundation productivity and existing €45/h
  contractor rate. One tree rounds up to 1 min; one shelter to 2 min. Owner
  sheltered planting uses 3 min and €5 material cash, no wage. Contractor uses
  €0.75 planting work plus €1.50 shelter work plus €5 material. Live sapling
  purchase prices remain €4.50 Beech / €5.50 Oak, charged once at purchase.
- Owner use is recorded for the just-resolved report year. Next annual advance
  starts a fresh 2,400-min budget; the Work Plan separately displays next-year
  planned/reserved minutes. No extra persisted budget-year field is introduced.
- One harvest finance event is attached to the last successful felling event;
  other felling events contain world results with zero finance. Reports persist
  only the authorised bounded fields. Variable harvest/other work/material review
  lines are derived from existing event fields instead of extra schema fields.

First live integration run passed 94 assertions, including exact one-time
felling/settlement, all-residual/broadleaf charging, owner/shelter timing, v14
defaults, executor biology invariance and 10-year mid-plan save continuation:
`C7A99591E8F5D487` uninterrupted/restored. Gate restored the original in-memory
world and never wrote a save slot. The final expanded gate and viability results
are recorded below.

## Final verification / required freeze

Final new gate: **120 assertions**, `SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_PASS`,
world restored afterward; no save-slot writes. Additional cases cover repeated
same-plan restore (quote caches invalidated because orders are object-bound),
player shelters replaced/returned across frozen preview, failed approved
planting, below-breast-height all-residual felling, and chronological closing cash.
Final uninterrupted/restored ten-year mid-plan hash:
**`023F4B627B758284`**, both paths.

Grouped settlement happens immediately after the **last valid felling world
effect** in the existing ordered annual work loop, before later non-harvest work
events. This preserves world-effect order and makes event cash chronology
reconcile. The grouped ledger is posted exactly once; no old hourly felling or
single €/m³ settlement remains active.

| Required gate | Result |
|---|---|
| Import/compile | PASS, no C# compiler errors |
| WorkEconomy foundation | PASS, 66 fixtures/420 assertions; adopted original Unity SHA unchanged |
| TimberYield foundation | PASS, 120 fixtures/3,007 assertions; `88FAF806…C4A8` unchanged |
| ScenarioOneEconomyIntegrationVerification | PASS, 120 assertions, original world restored |
| SaveHardening | PASS, atomic swap/backup/invalid-save checks on isolated slot |
| Planting | PASS |
| Deadwood | PASS |
| Removal | PASS; legacy settlement untouched |
| Pruning | PASS; legacy settlement untouched |
| Progress | PASS; objective thresholds unchanged |
| Interaction | PASS; original physical controls/clearance/pruning/reference assertions retained |
| Canonical juvenile | PASS, neutral `BFC55473C1506067`, normal 0.2 `3485B6630C9EA448` |
| Economic viability diagnostic | PASS, three minimums, no production retune |

Logs: ignored `Build/ScenarioOneEconomyGates/`. Re-run one selected gate:

```bash
python3 Tools/Verification/run_scenario_one_economy_gate.py ScenarioOneEconomyIntegrationVerification
# --method BeginCanonical for JuvenileMortalityFoundationVerification
# --method BeginViability for ScenarioOneEconomyIntegrationVerification
```

The launcher copies one disposable gate, runs it and removes the copy/meta.
Legacy disk-writing gates use `XDG_CONFIG_HOME=/tmp/opencode/ccf-economy-isolated-config`:
their save path is under that sandbox, never the user's real slot. Locally cached
Unity licensing files are reused privately without printing contents. An empty
isolated config first failed licensing before tests; that run is not counted.
Final gates all use the licensed isolated sandbox. New integration/viability
gates are memory-only and restore the original world even on failure.

Editor QuickSearch produced the known startup index exception in one initial
run; it is not a C# compiler or integration-gate failure. No rendered or interactive
player acceptance is claimed; existing interaction and new live-manager flows
were exercised programmatically. Final integrator owns rendered/combined acceptance.
Full Reference Future, calibration adoption and full legacy sweep were not
repeated; final integrator runs those once, as the packet requires.

## Economic expectation changes (only)

No biological assertion or expected biology hash was edited. Two existing
economic fixture files changed:

| Fixture | Old expectation | New expectation / measured value | Reason |
|---|---|---|---|
| Planting gate felling history | Per-tree `estimatedCostCents`, old hourly fixture €10.50 | Grouped quote €2,500; product revenue €9.66 for its target | Once-per-job commission replaces hourly felling, event still checks same tree/cell/volume/treatment |
| Deadwood retention gate | Per-tree hourly €10.50 | Grouped quote €2,500, zero receipts | Deadwood work remains charged once, same world/deadwood assertions |
| Deadwood sale gate | Per-tree cost and whole volume × €72/m³; old revenue €17.40 | Grouped quote €2,500; recovered product revenue €12.73 | Multiple graded volume batches at adopted roadside prices; no double valuation |

Fixture logs include `ECONOMIC_EXPECTATION` derivations. Original nursery
purchase prices are unchanged; planting work is now derived from the adopted
profile (one tree → ceil(60/100) = 1 minute → €0.75 contractor work), versus
the old shop plantingMinutes=10 → €7.50. Its gate reads approved quote estimates
and therefore requires no hard-coded value change. No changes were needed to
removal/pruning/progress/interaction/save-hardening expected biology.

## Economic viability — diagnostic only

40×40 m Scenario One, same production quote logic, unchanged €12,000 opening cash.
Modest interventions at planning years 0/8/16/24, removing 36/24/24/24 smaller
original trees; one deadwood stem inside each commissioned job. Owner sheltered
Beech + Oak enrichment at year 0 and replacement at year 24. This replacement
is a management-plan response to lost earlier plantings, not a change to ecology.
Initial early-enrichment-only diagnostic stayed solvent but had lost both
species at year 30; it was not declared an economic impossibility.

| Minimum | First thinning net | Second intervention net | Cash after Y1 | Y9 | Y17 | Y25/Y30 | Outcome |
|---|---:|---:|---:|---:|---:|---:|---|
| €1,500 | −€1,458.96 | −€1,382.07 | €10,521.04 | €9,138.97 | €7,907.30 | €6,994.87 | Completed Y25 |
| €2,000 | −€1,958.96 | −€1,882.07 | €10,021.04 | €8,138.97 | €6,407.30 | €4,994.87 | Completed Y25 |
| **€2,500 production** | **−€2,458.96** | **−€2,382.07** | **€9,521.04** | **€7,138.97** | **€4,907.30** | **€2,994.87** | **Completed Y25** |

Between listed intervention years, cash stays unchanged in this plan. Y25 includes
the replacement nursery/shelter purchases. All advances succeed, no negative
cash, and all original objectives remain met at Y30. Minimum always binds in
these small jobs; aggregating/removing stronger stems changes product receipts
without an arbitrary CCF premium or minimum per tree.

Do-nothing control: €12,000 at Y30, **Active**, failing fallen deadwood,
commissioned opening and both introduced-species objectives. Thus it is dominant
on **cash alone**, not on scenario completion. On this genuinely small property
light commercial interventions are costly; this is reported rather than retuned.
No mandatory `PRODUCT DECISION REQUIRED — ECONOMIC CALIBRATION`: €2,500 does
not make the required scenario structurally unviable in the tested reasonable plan.
Whether a more profitable small-property loop is desired remains a product observation.

## Completion handoff / final integration needs

Required stream: **READY FOR FINAL INTEGRATION**, save v15 sole-owner work complete.
No implementation blocker or required price/objective decision. Keep this
branch's foundation adoption commits when integrating; do not independently
rebuild or add the old foundation chain a second time.

Final integrator/Claude must:
1. Adapt its owned protection fixture setup to the v15 clear/replace rule.
2. Wire `ScenarioEcologyReviewLines` at the literal hook after merging the ecology
   formatter; user-facing shelter visuals likewise arrive from Claude's stream.
3. Complete the shared completion harness TODO phases, then run the combined
   acceptance/reference/calibration sweep once.
4. Review the explicit [S] unmarketed work-equivalent and whole-stem kept/deadwood
   conventions; no broadleaf sale deck, fake pulp, grants or pruning premium.

Stretch pruning/fencing is not started. Required legacy pruning and regeneration
removal settlement remain untouched. Only economical quote/readability polish
inside required scope was applied; no additional persisted fields were added.

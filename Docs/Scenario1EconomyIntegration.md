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

Shared base received; adoption complete. Required implementation in progress.
No product calibration changes or main merge/push are authorised.

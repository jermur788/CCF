# Save v20 compatibility contract v1

Audit base `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a`; also inspected stop report `157ea7d12f361b824f56405284d46312e7c0b9fc`. Packet authorises a proposed append-only AnnualReviewAcknowledged contract with schema remaining **20**, but this audit does not implement it. No Assets edits or Unity verification.

## Verdict

**Backward compatibility is feasible without a schema bump, conditional on append-only event ordinals and terminal-outcome grandfathering. Forward compatibility is limited, not guaranteed behavioural equivalence.** The source validator does not reject unknown `ScenarioManagementEventType` values: an event with numeric 9 is not itself a refusal reason at b4894cc. Source CloneEvents copies it through JsonUtility, while history selects WorkResolved events and old completion ignores chronological review. This is a source-derived prediction, not a measured JsonUtility round trip. A future stricter older validator may reject it; name/ordinal changes in any persisted enum are unsafe. Pre-broadleaf code also rejects new assortment values and pre-model2 code rejects growthModel=2.

## Persisted enum numeric contract

These are all enum types reachable from ForestSaveData/ScenarioOneSaveData serialized records (including integer-backed stage, mark and cohort origin). Reference schedules, price-book configuration, ecology diagnostics, UI enums and density classes are not save fields. Storm event records store numbers, booleans and strings, not the candidate/transform enums used by dormant profile configuration.

| Enum | Numeric values | Source | Unknown-value behaviour / source limit |
|---|---|---|---|
| `ForestTreeStage` | Mature=0, Stump=1, Sapling=2, Young=3 | `Assets/ForestPrototype/ForestTree.cs` | Validator rejects outside 0–3. |
| `TreeMarkType` | None=0, Fell=1, CropTree=2 | `Assets/ForestPrototype/ForestTree.cs` | No global defined check at load; restored through tree mark handling. |
| `RegenerationOrigin` | Natural=0, Planted=1 | `Assets/ForestPrototype/ForestPlanting.cs` | Positive-density age-band records (v16+ model≥1) reject outside 0/1; legacy route casts. |
| `WorkExecutionMethod` | Contractor=0, LandownerSimulated=1 | `Assets/ForestPrototype/WorkEconomy/ForestryWorkDomain.cs` | Enum.IsDefined in v15 order/event validation rejects unknown. |
| `TimberAssortment` | Pulp=0, Stake=1, Pallet=2, Sawlog=3, VeneerPlanking=4, Beam=5, Fencing=6, SecondPlanking=7, Firewood=8 | `Assets/ForestPrototype/WorkEconomy/ForestryWorkDomain.cs` | Enum.IsDefined for v15 sale reports rejects unknown; older pre-broadleaf code rejects values 4–8. |
| `ScenarioManagementEventType` | StockPurchased=0, OrderCreated=1, OrderApproved=2, OrderCancelled=3, WorkResolved=4, YearAdvanced=5, ScenarioCompleted=6, ScenarioFailed=7, CenturyReviewed=8 | `Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs` | Not checked by save validator; CloneEvents preserves numeric value, known-type consumers ignore unknowns. |
| `ScenarioManagementOutcome` | None=0, Succeeded=1, Failed=2, Cancelled=3 | `Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs` | Not defined-checked; success filters recognise only Succeeded. |
| `ScenarioEcologicalTreatment` | None=0, TreeFelledAndExtracted=1, JuvenilePlanted=2, RegenerationRemoved=3, TreePruned=4, TreeRetainedAsDeadwood=5, WindthrowSalvaged=6 | `Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs` | Not defined-checked; known comparisons only. |
| `ScenarioWorkType` | FellTree=0, PlantJuvenile=1, RemoveRegeneration=2, PruneTree=3, SalvageDeadwood=4 | `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` | Not globally defined-checked; unknown has no recognised resolution case. |
| `ScenarioWorkStatus` | Pending=0, Approved=1, Completed=2, Failed=3 | `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` | Not globally defined-checked; IsOpen recognises Pending/Approved only. |
| `FellingMaterialOutcome` | SellAndExtract=0, RetainAsFallenDeadwood=1, KeepForUse=2 | `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` | No global defined check; downstream adapters switch known values. |
| `ScenarioOneOutcome` | Active=0, Completed=1, Failed=2 | `Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs` | Restored directly; unknown is neither Active nor Completed/Failed. |

Append-only safety: preserve every existing number; never insert/reorder/delete/reuse a value. Proposed `AnnualReviewAcknowledged=9` follows CenturyReviewed=8; giving the appended entry an explicit 9 protects reviewability. Default zero remains StockPurchased, not review acknowledgement. No absence/default conversion may fabricate chronology.

Integer model selectors are separate from enums: RNG 0 legacy/1 keyed; regeneration 0 legacy/1 age bands/2 competition; growth 0 legacy/1 SiteClassDensity/2 SpeciesAware; storm 0 off/1 dormant profile; geometry 0 Legacy40/1 Enlarged80. Growth/regeneration bounds are rejected by ForestSaveValidation; storm/geometry dedicated validation rejects unsupported values. Normalize helpers clamp model integers when called directly, but disk load validates first. Growth2 at b4894cc includes broadleaf market yet still identical shipped Oak/Beech growth constants; it is not a calibrated species-growth guarantee. No default-model change follows from load support.

## Capture/restore and field invariants

- ForestSaveController.CaptureData writes CurrentVersion=20, all model selectors and geometry; stage/mark numeric casts, biological death cause/year, species identity and sizes, cell bands origin/originYear. Seed rain remains derived. Origin never determines biological growth; natural/planted chronology survives capture and restoration.
- ScenarioOneManager.CaptureSaveData clones work orders, inventory, annual reports, events, snapshots, deadwood, shelters, patches, juveniles, storm records and outcome; saves counters and annualReviewSeen. Capture stores actual outcome/outcomeYear/reason, not a history-derived substitute. Restore clones these records and advances next event ID beyond max stored ID.
- Work-order createdYear is planning chronology; resolvedYear and successful WorkResolved events establish resolved work. An empty plan, approval, cancellation or failed-only plan cannot count as a successful cycle. Existing history groups successes by resolution year; annual totals alone do not establish plan boundaries or acknowledgement. Future implementation must define grouping from actual resolved orders/events while retaining different-year rule; it must not assume every report is a Work Plan.
- annualReviewSeen is one historical tutorial boolean. MarkAnnualReviewSeen sets it when a report exists; it stores no year or event. Automatic opening is not acknowledgement. The stop report's two histories with identical persisted records prove boolean-only inference impossible.
- Ecological snapshots have year/structure/light/habitat/species summaries. They do not store a review action. Annual reports have year, costs/revenue, task counts and sales. No review timestamp may be inferred from either.
- Intervention history and completion summary are reconstructed from persisted lists. Timber sale assortment, volumes in cm³ and revenue in cents are persisted in annual reports; no derived display label or current quote may overwrite historical accounting. Biological death is distinct from harvesting; history cannot invent mortality causes from aggregate snapshots.
- Restore preserves data.outcome; Active gets outcomeYear=-1; terminal values retain recorded year. EvaluateProgress evaluates objectives only while Active. Legacy v15 migration runs only for versions<15, resets introduced fields and contractor defaults; no v20 review migration exists.
- Validate before world mutation (shared CanLoad/LoadData problem checks). Disk path adds raw JSON geometry/storm/competition presence checks; test-object LoadData is not identical to raw-disk JSON validation. Enlarged80 indices must be checked against saved geometry; no grid reinterpretation.

## Compatibility matrix

“Current” below means inspected b4894cc; “extended” means proposed N3, still absent.

| Path | Required result | Current evidence | Risk / blocking gate |
|---|---|---|---|
| OLD v20 → current / extended | Existing enum numbers/accounting/models retained; terminal saves stable | Source capture/restore/validators support same fields | Real loader fixtures and integrated save/load NOT RUN |
| NEW v20 → older b4894cc | May load event9 but cannot enforce new chronology; must not promise reversible semantics | No eventType defined-check; old success/history filters ignore event9 | Actual JsonUtility preservation and old save overwrite test required |
| NEW v20 → pre-overnight older code | Not guaranteed: growth2 or assortments4–8 may be refused | Existing validators bound models and sales enum | Document consumer-specific minimum, never claim universal v20 compatibility |
| Legacy Active v20 | Remain Active until existing objectives plus new truthful chronology; boolean true never manufactures event | No chronological acknowledgement currently exists | N3 must permit new acknowledgement after a real successful cycle, before subsequent different-year work |
| Legacy Completed v20 | Remain Completed, preserve year/reason; no retroactive revocation | Restore copies outcome; active-only reevaluation | N3 grandfather + save/load runtime gate |
| Legacy Failed v20 | Remain Failed, preserve reason/year; no resurrection | Same terminal restore path | Terminal-outcome fixture/runtime gate |
| Growth Model1 save | Preserve growth1, existing Sitka biology and old eight objectives | Capture/Restore version-gated selector | Full exact Sitka anchors and continuation; no migration to2 |
| Growth Model2 save | Preserve growth2, history/timber, density lineage; review event interpreted only by extended code | Latest2 supported in source | Density v2 U3 and post-integration load; historical model2 means legacy implementation coefficients, not new calibration |

## Proposed event contract and required tests

Record appended event9 only for an actual acknowledgement of an existing Annual Review via the handler. Use current simulation year and monotonic eventId, with valid default executionMethod and non-negative ownerMinutes so existing validation remains satisfied. Event ordering establishes “after first, before second” even when first resolution and review share a year. Require two nonempty successfully resolved Work Plans in different simulation years and existing eight objectives; second work remains non-prescriptive. No new objective, schema number, fabricated legacy event, or altered annualReviewSeen meaning.

Test duplicate acknowledgement handling explicitly; multiple reviews must not create extra work cycles. Same-year two resolutions fail. Review-before-first, failed-only work, empty/approval-only plans fail. Capture after first, after review and immediately before second; restore and compare counter/event ordering/accounting. New complete path passes; old shallow path fails for Active saves; old Completed/Failed stay terminal. Unknown enum9 in older code and unknown 99 in extended code need explicit policy tests; no silent inference. Event ID uniqueness/monotonicity and malformed inputs are not comprehensively validated today and must not be presumed safe merely because deserialization succeeds.

## Offline test objects

`python3 Tools/Verification/SaveCompatibility/check_contract.py` creates ten partial scenario contract objects, preserves legacy/terminal/new/same-year/failed cases and runs stdlib JSON round trips. They are **not complete valid Unity saves** and do not execute production validation, JsonUtility, objectives or loader. Output: 10/10 PASS. `enum_snapshot.json` records inspected ordinals for future implementation comparison. These objects test the proposed predicate, not N3 production behaviour. No Unity save test has run in this queue.

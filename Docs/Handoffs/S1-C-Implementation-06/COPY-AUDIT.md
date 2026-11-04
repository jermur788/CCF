# T-06 prior-treatment guidance copy audit

Authority: approved S1-C FR-6/FR-7/AC-5, specification §10 management-guidance classification, #147 task packet, and Manager merge disposition on #147. Guidance is L4 management guidance; it is not measured evidence, a simulation result, a causal attribution or an instruction to commission work.

## Source path and selection

`AnnualReviewView.BuildPriorTreatmentGuidance(year)` calls the existing read-only `ScenarioManagementHistory.DescribeWork(YearHistoryView)`. It reads successful `work.Done` items only and selects the first recorded example per biological treatment, in existing history order. The short card states that selection limit. All targets remain available through the existing Work done drill-down. Unknown/None treatment, failed and cancelled work yield no treatment guidance.

`WorkItemDetail` copies the existing `WorkResolved` event's year, event ID, order ID, biological treatment and recorded cell. No new fact, ID, record or query store is introduced. `HistoryProvenance` and its six meanings are unchanged. Guidance is headed `L4 MANAGEMENT GUIDANCE` and every inspection prompt starts `L4 GUIDANCE`.

## Sentence/template mapping

| Case / displayed template | Exact source | Interpretation / boundary | Waypoint source |
|---|---|---|---|
| `Year {Year} · event {EventId} · order {WorkOrderId}: {TreatmentText}.` | Selected successful `ScenarioManagementEvent`: `year`, `eventId`, `workOrderId`, `ecologicalTreatment`, copied by DescribeWork | RECORDED. `TreatmentText` is the existing result wording. Does not infer a thinning purpose or present condition. | None in this line |
| `Map square {label}` | Selected event `cellIndex`, rendered through the loaded save's existing grid labels | RECORDED. The label formats the recorded cell; it is not a current tree lookup. | The same event cell, only if in the active grid's valid range |
| `Map square not recorded; no inspection waypoint offered.` | Event has `cellIndex < 0` | NOT RECORDED. Does not claim all other location fields are absent. Position-only details remain available in Work done. | None. No current tree/juvenile/position lookup or map-square reconstruction |
| `Look at canopy, young regeneration and the trees still standing before judging whether any further work is appropriate.` | Successful treatment `TreeFelledAndExtracted` | L4 inspection prompt. Does not assert that a canopy gap or regeneration resulted, or suggest an amount/interval of further work. | Recorded cell only |
| `Look at fallen deadwood and surrounding regeneration before considering any further work; this record does not establish their condition now.` | Successful treatment `TreeRetainedAsDeadwood` | L4; names recorded retention, makes no present-condition claim. | Recorded cell only |
| `Look for the planted tree and inspect its condition and surrounding vegetation. A planting record does not establish survival now.` | Successful treatment `JuvenilePlanted` | L4; explicitly separates planting from present survival. Does not recommend planting or replacing trees. | Recorded cell only |
| `Check the recorded shelter's condition too; its present effectiveness is a separate observation.` | Planting item has `ShelterRecorded && ShelterInstalled`; this comes from the retained resolved order's `installShelter` | L4; suffix absent when shelter provenance is absent or false. Does not claim the shelter survives or works now. | Same planting-event cell; no separate shelter position inferred |
| `Inspect the recorded tree's condition and crown before considering further work; a pruning record does not establish its condition now.` | Successful treatment `TreePruned` | L4; no second pruning recommendation, crown assessment or current target relocation. | Recorded cell only |
| `Inspect young regeneration and vegetation now before considering further work; the earlier clearing record does not describe today's cover.` | Successful treatment `RegenerationRemoved` | L4; no claim that vegetation remained clear or that clearing benefits the site. | Recorded cell only |
| `Inspect the recorded area for remaining deadwood and regeneration before considering further work; salvage does not describe everything present now.` | Successful treatment `WindthrowSalvaged` | L4; does not reconstruct storm loss, remaining material or salvage benefits. | Recorded cell only |
| `One recorded example per treatment in this year. These are inspection prompts, not a recommendation to carry out work. Observe the forest now separately from the earlier record.` | Presentation selection rule and L4 classification | Explanatory limit, not a new historical/current fact. | None |
| `Look at recorded map square` | `PriorTreatmentWaypointCell(item, CellCount)` returns the unchanged valid event cell | Reuses existing `ShowStormWaypoint` → `StandMapView.SetWaypointCell`. Marks the recorded **area**, not an exact historical stem. | Recorded event `cellIndex` only |

## Current observation and causal limits

No new L2 observation value is manufactured or displayed by this card. Inspection is something the player does in the current forest after reading the L1 record. Existing Annual Review CURRENT observations remain explicitly labelled; historical forest/storm sections retain T-04's suppression of latest-only live values under an older year. The guidance never turns a current observation into a historical fact. It makes no treatment→outcome causal claim, interval recommendation, universal prescription or choice of forestry decision.

## Read-only and persistence boundary

The new production methods only read the existing projection and build UI. They never acknowledge a review, advance a year, write/serialize a save, change a work order, alter a lesson/curriculum, or mutate history. Expand/collapse is transient UI state. The optional look action reuses the existing map mechanism, including its pre-existing local `map.waypoint` tutorial preference; it introduces no new persistence or curriculum ID. Map navigation does not acknowledge the annual review or rewrite scenario history.

The offline check covers all six known biological treatments, failed/cancelled/None/null cases, missing/out-of-range/zero cells, absence of a live grid and unknown shelter provenance. Runtime qualification covers real felling and planting histories and a partial record with no location while live planted objects still exist. Production changes relative to approved pushed base `3ea4af8b` are limited to `ManagementHistoryText.cs` and `AnnualReviewView.cs`; every other Assets/Packages/ProjectSettings Git object is unchanged (scope receipt in durable evidence).

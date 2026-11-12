# Scenario One — Repeated Representative Rack Management V1

**DRAFT — MANAGER REVIEW REQUESTED. NOT APPROVED FOR IMPLEMENTATION.** Revised 12 November 2026, [#413](https://github.com/jermur788/CCF/issues/413) B0–B2. [Plan](plan.md), [tasks](tasks.md), [requirements](checklists/requirements.md).

## Authority and baseline

D-076 accepts the product intent, not this extension's source/save policy. D-064/D-065 and D-071's five parallel headland-entry racks remain controlling. Locked context `63044dfbbeb869996ceecec8a47619dcba048b0b`; newer decisions separately read at [main 91511faa](https://github.com/jermur788/CCF/blob/91511faab737718f65fee896b32628a755cae90f/Docs/Project/decision-log.md). See [source audit](../../Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/FABLE-HP-D076-REPEATED-AREA-MFM-SOURCE-AUDIT.md).

Source **E** = `e759193354b4613f88e3370a62ccaccf02c556cf` (isolated cadence integration); helper **R** = `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4` (#406 offline source). #397 is CLOSED source handoff; #402/#408 feature-qualified isolated cadence later. #406 is CLOSED / offline-source approved and is not cadence implementation. Neither this draft nor that helper proves this repeated-source workflow exists or is Unity-qualified.

## Desired player flow

Preserve the original fixed third-rack lesson and earned credit. In a later cycle, inspect a supported current rack-served source area, demonstrate several real tree decisions and execute a sample treatment, then review which other areas are currently comparable. Individually inspect/confirm exterior Crop Trees, review proposals and fit, approve a prescription to create Pending ordinary work, and separately approve the Work Plan. Advance actual years, walk all actually treated places and review recorded work and stand observations before deciding again.

One representative source may suffice for a homogeneous episode. Different conditions can require a separately demonstrated source or manual remainder. Five racks do not mean five samples or mandatory treatment. Recommend one later current source plus manual remainder for the first bounded release, preserving multiple-source extension as an explicit Manager choice.

## Present source truth

| Inspected E source | Existing contract | Extension gap |
|---|---|---|
| ScenarioOneRackSampleGeometry.Derive | Exactly five confirmed racks; fixed third ordered rack; teaching ±12 m along, ±9 m across. Midpoint strips and explicit outer remainders. | No arbitrary later source selector. |
| ScenarioOneCoverage.Classify / Compare | Three anchor-aligned areas per rack; source Manual, explicit manual choice first, conflicts Excluded, too few/different NotCovered. | No episode-specific multiple-source ownership. Completed target work can keep an area excluded. |
| ScenarioOnePrescription.Derive / CheckFit | Executed original lesson, minimum demonstrated release and removed BA share; live rederivation; Sitka adults only. | Historical sample is not a fresh later demonstration. |
| ScenarioOnePrescriptionApproval.ValidatePrescription / Build | Rule/calibration/source identity and live links rechecked; current Accepted state only. | No new episode/source-area identity or generic age-expiry policy. |
| ScenarioOneManager.CreatePrescription / ApprovePrescription | Stores source rack/year/anchor/tree sets; all-or-nothing Pending order creation with rollback. Work Plan is separate. | Approved record cannot simply be approved again; selecting another source is not implemented. |
| ForestSaveData / ScenarioOneWorkOrder | Prescription ID, source rack/year, sample sets and explicit choices; order unit/crop/reason links. | No episode ID, independently selected source-area ID or saved historical per-rack ecology. |

Coverage compares a shared reconstructed pre-sample state: current living stems plus original sample removals put back using stump diameters, not an archived rack-year census. Net measured area excludes corridors/masks consistently. Current density/mean DBH/mean Hegyi tolerances are gameplay calibration (20%/15%/20%, inclusive; minimum 30 stems), not general forestry thresholds. The one-fixture 13/14 covered result and SecondSamplePolicy.NotRequired are not evidence that all heterogeneous stands need one sample.

## Proposed requirements

**FR-01 — First teaching and access unchanged.** Keep the fixed third-rack 24 m teaching footprint, three Crop Trees, 8 m teaching spacing, 30% release/40% BA safeguards, existing marks, five parallel access layout and historical credit. A later source is separate, never a migration of first-lesson identity. Preserve Legacy40/frozen Reference Future.

**FR-02 — New player-selected demonstrated source.** A later episode can select a supported bounded rack-served area from confirmed geometry and execute a fresh multi-tree demonstration. MD-02 must settle footprint, number/spread of decisions, source eligibility, source reuse and resolution proof with authentic fixtures. Do not copy the first lesson's constants as universal later criteria. A single arbitrary selected tree, old removal set or current rack selection alone is insufficient. Reusing an area requires explicit fresh demonstration and revalidation; a historical prescription is not automatically re-applied.

**FR-03 — Comparable current conditions only.** Evaluate source and target in an explicitly defined consistent state using real geometry, net areas, masks, species, density/diameter/competition and visible limitations. Manager must approve how later samples separate new removals from older stumps, growth and previous work. Existing “put original sample removals back” semantics cannot be assumed to represent a later episode. Test homogeneous, deliberately heterogeneous, tolerance-boundary and too-few-stem cases. Preserve Covered/NotCovered/Excluded/Manual; do not force coverage. Soil/slope/wind/moisture are not measured classifiers here. No imaginary hectares, universal stocking factors or Sitka automated rules for Oak/Beech. Broadleaf/heterogeneous/manual remainder remains legitimate.

**FR-04 — Stable episode/source identity.** Proposed identity includes episode, source rack/area and geometry revision, fresh sample resolution year and tree sets, rule/calibration signature and explicit target/manual choices. This is a design proposal, not existing saved state. Source identity must remain distinct from prescription, resolution year and coverage-unit labels. Existing sourceYear is prescription acceptance year, not proof of sample execution year. Preserve original IDs/history on reload and later cycles; no old-source reconstruction from unit-ID text or stand totals. MD-05 must approve versioning, lifecycle and hash/old-save policy before fields or persistence.

**FR-05 — Exclusive ownership and overlap.** Each target unit belongs to at most one source group within an episode. Overlap, duplicate tree IDs or ambiguous identity must fail closed to a visible Conflict/Manual state until explicit ownership choice and full revalidation. Never merge proposal sets or give hidden priority to rack number/proximity. Recheck target-tree uniqueness across the entire episode even where geometry is ambiguous. Across episodes a unit can be considered again only with fresh eligibility; already resolved work is history, not a reusable order. Multiple sources in one episode remain optional pending MD-03.

**FR-06 — Staleness and previous-work policy.** Current-state changes can invalidate source, coverage, crop trees, masks or target links. Mark/recompute before acceptance/preview/approval and revalidate immediately before writes; no stale approval. MD-04 must distinguish re-evaluation from expiry: a year tick calls for fresh validation but does not prove every old prescription invalid. Current code has no generic year-expiry check and excludes areas with completed work. Do not silently remove that repeat blocker. Define episode-scoped treatment of prior work, pending orders, failed/cancelled tasks and manual marks, retaining completed history and manual fallbacks.

**FR-07 — Staged all-or-nothing commitment.** Proposals remain data, not Fell marks. Outside-sample candidates need individual 3D confirmation or manual choice. Player marks/open orders prevail and conflicts remain visible. A single invalid generated link refuses the proposed approval as a whole; no partial prefix. Preserve current fresh validation/rollback and ordinary Pending orders, then separate Work Plan labour/cost approval and annual execution. No auto-work on skip, no duplicate felling, no rollback of earlier successful world work.

**FR-08 — Actual work and effects at honest precision.** Show each verified resolution year + prescription + stored unit with actual succeeded order/event IDs, not all accepted/proposed coverage as treated. Failed/cancelled/unresolved work stays explicit; no per-tree allocation of annual grouped harvest money. Source/episode association must come from approved durable evidence, not parsing labels. Current #406 helper cannot yet be relied on for real MFM grouping because production resolved events omit prescriptionId while ValidOrder requires equality (audit D076-08). Resolve under a separate reviewed scope before depending on it; this draft makes no event/hash change.

All actually executed work affects authoritative trees/ecology and should be visible in current 3D throughout treated areas. Existing annual snapshots support stand/species observations only; never label stand-total deltas as historical rack ecology or causal treatment response. Optional per-rack historical measurements need a separate approved measurement/save/geometry contract; recommend current 3D plus honest work history first.

**FR-09 — Independent learning and navigation.** D-074 substantive/spaced learning is a separate unapproved goal policy; choosing/reusing a source does not satisfy it. D-077 four-year suggestion/five-tick skip is calendar navigation through real annual steps, not a treatment schedule. D-073 owns honest read-only historical comparisons. No changes to completion, cadence, saves, current MFM or protected hash under this draft.

## Manager decisions before APPROVED

| ID | Choice | Conservative recommendation |
|---|---|---|
| MD-01 | First delivery: current 3D + executed-work history, or new per-rack historical ecology? | Existing 3D plus truthful work history first; separate approval if historic local metrics essential. |
| MD-02 | Later source eligibility, fresh demonstration, footprint/decision calibration and consistent comparison state? | Fresh multi-tree demonstration; do not use original stumps as a new episode. Trial authentic fixtures. |
| MD-03 | One source + manual remainder or simultaneous multiple sources? | One + manual first; multi-source requires explicit exclusivity/overlap proof. |
| MD-04 | Staleness, previous completed-work blocker and conflict policy? | Fresh live validation; preserve completed history, manual fallback and no duplicate orders. |
| MD-05 | Episode/source identity, persistence, old saves and hash policy? | Independent save/history review; unknown old identity stays unknown. |
| MD-06 | Exact resolved-work linkage and UI truth? | Independently qualify real manager-produced MFM events/orders before relying on helper groups. No silent event stamping. |

## Future acceptance matrix

All proposed runtime gates are **NOT RUN** here.

| ID | Expected observable result | Requirements |
|---|---|---|
| AC-01 | Original first lesson, five racks and earned credit unchanged; no mandatory five samples. | FR-01 |
| AC-02 | Original lesson followed by at least two later current-state decisions across annual change, with fresh multi-tree demonstration when treating; homogeneous one-source and heterogeneous/manual paths. | FR-02/03 |
| AC-03 | Inclusive tolerance edges, sparse source/target, masks, species and previous-stump comparison produce honest Covered/NotCovered/Excluded/Manual results. | FR-03 |
| AC-04 | Approved multi-source variant: overlapping units/trees stop without duplicate work, explicit owner selection revalidates. If deferred, UI permits only one source + manual remainder. | FR-04/05 |
| AC-05 | Year change/source drift/crop death/manual marks/open or completed work trigger approved revalidation/refusal; failed/cancelled work cannot pretend to be a fresh sample. | FR-06/07 |
| AC-06 | Candidate confirmation → prescription → Pending → Work Plan → actual annual resolution; any invalid generated link changes no approval state; trees treated only once. | FR-07 |
| AC-07 | Exact succeeded unit work and all current 3D effects visible; missing/mismatched/duplicate IDs remain unknown; no fabricated per-rack history, money or causality. | FR-08 |
| AC-08 | New/old/Legacy40/frozen saves, reload, stable identity/hashes, Sitka/Oak/Beech and annual/skip semantics preserved; 1280×720/1920×1080 explain source/current/history/unknowns. | FR-01/04/08/09 |

## Stop conditions

Unresolved MD-01…MD-06, unstable geometry/source identity, absent evidence, protected hash drift or competing MFM/UI ownership block production. Manager approval of all four artifacts, new bounded source claims, independent review and actual qualification are still required. No Unity, production implementation, integration or build is claimed.

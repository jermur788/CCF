# Whole-Rack Objective 2 V1 — Proposed Implementation Plan
**STATUS: DRAFT / NO PRODUCTION AUTHORITY** | Claim [#441](https://github.com/jermur788/CCF/issues/441). Parent [spec](spec.md), [requirements](checklists/requirements.md). This is sequencing/ownership guidance contingent on explicit #425 approval and actual user-Editor release.

## A. Entry/approval gates before any source edit

1. Independent browser review of [research](../../Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-FULL-RACK-RESEARCH-AND-EXTRAPOLATION-EVIDENCE.md), full four-file DRAFT, existing D-065 and D-076, and final decision table. Return APPROVED / NEEDS CHANGES / REJECTED **for the draft quality only**, not Unity implementation. Strategic Manager #425/user explicitly resolves OPEN-1..OPEN-5 and writes corresponding canonical Decision Log / corrected approved specs; Manager #424 accepts source routing and safely tracks ownership.
2. Before implementation, inspect live Docs/Project/coordination/active-tasks.md and all claims. Register a new **implementation** claim, specific worker/provider, branch, isolated local worktree and exact HEAD; verify no #440 writer overlap and confirm user has released the protected personal Unity session. Current #441 has **no local worktree and no Unity lease**.
3. Pick an exact current **accepted integration lineage**, NOT the static C-01 research-copy branch solely because its files were read here. Inspect current rack/learning/MFM/order/save code, actual schema version, existing parity and preservation anchors. This design does not declare a working tested candidate.
4. #418 authentic prescription-resolution provenance: obtain a real independent PASS/repair disposition before claiming complete provenance. Do not invent PASS from code field existence.
5. No new Windows build before user personally finishes test and explicitly authorises build. No default-main merge, Source branch, user PlayerPrefs/save/Unity asset changes during docs phase.

## B. Non-overlapping ownership matrix (critical)

| Layer / source areas (verify actual paths on implementation HEAD) | Primary implementation owner | Conflict boundary |
|---|---|---|
| Whole-rack pure geometry: Assets/ForestPrototype/ScenarioOne/ScenarioOneRackSampleGeometry.cs, ScenarioOneCoverage.cs, ScenarioOneCoverageWorld.cs and new area-identification helper if needed | **Future #441-successor management/MFM core implementer** | Own numeric coverage and area IDs only. Do not edit source/Unity until assigned. |
| Teaching analysis: ScenarioOneRackBay.cs, ScenarioOneRackBayReader/Review, ScenarioOnePrescription.cs, ScenarioOnePrescriptionCalibration.cs, ScenarioOneCropTreeCandidates.cs | **Future #441-successor lesson/MFM core implementer** (same writer until API stable) | Version old exactly-three gates and new full-rack gate separately; never change legacy lesson semantics in place without save migration and exact tests. |
| Work/persistence: ScenarioOneManager.PrescriptionApproval.cs, ScenarioOnePrescriptionApproval.cs, relevant ForestSaveData/Validation, ordinary order/history read models | **Future #441-successor management/MFM owner** with save-specific independent reviewer if changed | No second order authority; preserve old history and active work; publish API before UI implementation. |
| Generic Objective UX, typography, keys, backdrop, Stand Map HUD, first-rack wayfinding, shared scenario UI: LearningObjectivesView*, ScenarioOneRackBayText, ScenarioOneTeachingSequence, ScenarioOneUiRoot, StandMapView, StandMapAreaFootprints | **#440 has primary UI/source ownership** | **#441 only specifies API/data requirements**, must not become a competing writer. Later handoff: request #440 owner integrate new Objective 2 view via a documented data/viewmodel contract, or Manager formally transfer explicit paths before edits. |
| Rack V2 saved geometry, actual rack corridor and entrance physical marks, user D-073 test candidate; five parallel layout | Existing owner/user; **read-only for this task** | Do not alter. |
| QA offline verification files and targeted Unity gate (once released) | Dedicated QA owner per claimed bounded task; one exclusive Unity Editor lease machine-wide | No duplicate Tier-3; independent source review for persistence/history and actual Unity evidence. |
| Canonical decisions/current milestone, live coordination and dashboard | #425 strategic, #424 operational | #441 writes new docs only; no active-tasks.md edit or decision promotion. |

**API boundary proposal:** a pure read-only WholeRackObjective2ViewModel surfaced by the future management core (not a second ecology state): source rack ID and 5 finite footprint polygons, source inspection-progress/resolved sector records, eligibility/candidate counts, source fit, per-rack per-subarea coverage/reasons, selected/confirmed Crop Tree IDs, distinct preview/proposed/approved/executed status, aggregate BA/cost, next 3D inspection target, and provenance label. No UI layer writes Crop/Fell/tree orders directly through callbacks; commands are explicit user actions to existing authoritative manager and are revalidated there. #440 decides presentation; this feature decides semantics. Stable rack IDs, stable saved lesson step IDs; UI display ordinal separate.

## C. Design slices and dependencies

### Phase 0 — baseline and measured world evidence (offline/GitHub where possible)
- Compare current candidate with published C-01 source and accepted D-065; identify new save version, exact object state, source ID, and whether current ecological snapshot supports any spatial historical comparisons.
- Acquire actual 1,344-tree archived reference fixture **with coordinates**, five confirmed rack segments, current Rack V2 route/masks, eligible living species/marks and exact year. The report correctly says NOT MEASURED until this is available.
- Produce a deterministic JSON/CSV-style fixture witness (not a new authoritative game source): per-rack polygons, net m², accessible living Sitka adults, other species, actual Crop Trees, baseline BA, mean DBH, competition and treatment metrics; exact hash + five source possibilities. Check no double counting and edge residues.
- Run source-size/workload sensitivity: new finite reach vs old infinite full strip; old 24×18m rectangle vs whole strip; central vs outer source; calibration tolerances and along-rack variation; source seen before treatment vs after treatment. Return to Manager if source is unrepresentative or requires unsupported site classes.

### Phase 1 — pure whole-rack geometry and source identity
- Add **versioned whole-rack W_i geometry** from confirmed Rack V2 rather than changing original fixed teaching logic. Pre-feature calculation and old saves continue via old version. For five racks report explicit five working polygons plus true external remainders, source and four destination identities.
- Reuse authoritative masks and geometry policies, but **cap area by actual rack longitudinal reach**; model checks for nonparallel/overlapping/too-short/crossing finite segments and unresolved outer edge.
- Pure deterministic geometry tests compare actual tree-ID classification and GIS/independent polygon oracle on the fixture. A count/area discrepancy is a STOP, not a tolerance waiver. Source identification survives rack sorting/order changes.

### Phase 2 — teaching/curriculum source-completion contract
- New first Objective 2 guide presents whole source visit; inspect left/right and along full length. Guide player to real trees and let them manually choose/reject; record only necessary explicit source-sector completion evidence that cannot safely be inferred from marks. No fixed count and no machine health/quality score.
- Teach local competitor decisions (and justified non-removal), source result, Work Plan and review. Preserve D-065 old three-tree sample path for genuine prior saves where needed, separate old stable teaching step IDs from new versioned IDs.
- Decide OPEN-3 before implementing full-source “30% each Crop Tree” gating. Check minimum no-release/insufficient-source-evidence truth: survey may complete yet extrapolation may remain unavailable until a real treatment is demonstrated. Distinguish “survey done” from “valid transfer rule”.

### Phase 3 — whole-source rule, fit, destination comparability
- Preserve current authoritative CompetitionShare and original pure-Sitka biological simulation; version any changed sample baseline logic and source/coverage entity names. Actual same-year source baseline must survive executed removal, multi-year advance and save/reopen; if stump reconstruction is not sufficient, add minimum versioned persisted source evidence **after approval**.
- Derive closed-vocabulary release-envelope policy from *all real source Crop Tree/competitor decisions*, never a copied three-tree numeric pattern. Report “source evidence insufficient” when appropriate. Fit-check against source manually selected marks and executed removals with explainable disagreement; no hidden quality grading.
- Recompute each of four target rack areas using current masked net area, density, DBH, CompetitionShare, species, protection, marks and already worked areas. Within-rack Low/Centre/High (or revised v2 area segmentation) may be individually Covered / NotCovered / Excluded / Manual; an aggregate rack badge may summarize **partial**, never turn one Covered segment into whole-rack eligibility.
- Recalibrate existing 20/15/20%, min 30, 16m spacing proposal values using **measured** fixture and mismatched conditions; preserve original calibration signature for prior approvals. No claims these are empirical Irish constants.

### Phase 4 — candidate confirmation and proposal chain
- Default A: per-candidate 3D accept/reject, each confirmed Crop Tree remains genuine Blue state. Use existing candidate queue/current-tree inspection, no automatic health inference, no batch action. Link coverage proof and rule to proposed neighbouring Sitka removals. Reuse authoritative release calculation and cap including already marked/approved removals, one competitor counted once.
- Source execution/review (required for rule under current D-065) precedes actionable transfer. A preview earlier may be advisory only. Make clear how source and destination orders sit in first management loop without duplicate payment or year execution.
- Revalidate source/year/area/confirmations/rule and persistent player overrides at prescription approval; generate **only ordinary Pending work orders** with durable links. Reuse separate Work Plan approval and ordinary contractor/landowner simulation; price, regeneration, canopy and year effects come from existing authoritative systems.
- If #425 accepts alternate B automation after workload evidence, **new separate version and review gate**; do not implement it inside A as performance optimisation.

### Phase 5 — persistence/history and repeated cycles
- Record minimum new source/inspection-completion/mode/provenance facts through a separately reviewed versioned save upgrade. Old actual three-tree completion does **not** prove full-rack survey. Old 3-tree source and MFM orders remain authentic history; don't rewrite or re-execute. Current pre-feature Enlarged80 gets an explicit new task or user-authorised continuation, never silent credit.
- Existing stand/species annual ecological snapshot is not a per-rack time series; show current actual tree state and recorded work by area from genuine order identity. A per-rack ecology-history snapshot would be a *separate SPEC REQUIRED change*, not part of this slice.
- Integrate D-076: after one-year or skip, source evidence is no longer automatically current; allow another representative rack/year, re-compare today, block double prescription ownership for same target, preserve executed orders. Full manual mode remains available. Avoid arbitrary additional source requirement in a homogeneous stand.

### Phase 6 — focused qualification, presentation and integration decision
- Tier 1 pure offline matrix (fixtures, math, old signatures, negative approvals, all five rack shifts, exact source baseline and two cycles). No speculation about Unity evidence from a CLI test.
- Static independent management/save review for candidate before Unity. If source conflicts with #440 HUD, stop and agree API/file ownership; #440 integrates UI under a distinct claim or formal handover. No concurrent edits.
- Tier 2 **one** exclusive qualified current Unity candidate after user releases session: full-length source 3D left/right/far, rendered 1280×720 and 1920×1080, physical inspection/no hidden quality, map statuses, correct Work Plan, authentic year changes in all treated racks, save/reopen and true old-world load, no manual override loss. Additional fast repro only for actual defect.
- Qualification handoff links real logs/screens/witnesses with executed code/branch/HEAD; independent conformance reviewer checks D-065/D-076/D-071/canonical conditions, #418, frozen Reference Future and Legacy40. Integration verdict **APPROVED / NEEDS CHANGES / REJECTED** belongs to Manager; no implied ACCEPTED/user playtest. Tier 3 only at integrated milestone candidate.

## D. Save and algorithm migration ledger

| Old object/contract | Proposed migration | Risk / explicit test |
|---|---|---|
| Fixed 24×18m centre teaching footprint, three trees, three zones | Keep legacy version path; new whole-rack source geometry and completion semantics | Old saves cannot auto-gain complete survey; one-way v24 latch meaning preserved |
| C-01 15 coverage areas, one teaching-centre source | All three source rack segments source-owned; targets are four physical racks, possibly segmented | No source-centre leakage, overlap/duplicate orders, honest 4-rack status |
| MFM-03 versioned thresholds/rule, 13/14 old test fixture | New separately named calibration and rule signature after measured data | Old prescriptions replay on historical rules; version drift blocked |
| Source executed-removal stump reconstruction | Audit full-rack cumulative source; store source-year baseline only if reconstruction inadequate | Save/load and year-shift same/changed area cannot fabricate pre-state |
| Existing pending order/provenance links and #418 | Reuse actual authoritative order pipeline, unique source/rack/target links | No approved work without two gates; no double felling, honest settlement |
| Annual whole-stand/per-species snapshots | Read-only factual summaries only | No per-rack historical graphs absent stored evidence |
| Legacy40, Reference Future v1, protected hashes | No mutation except approved additive compatible save contract | Fixture hashes, snapshots and historic UI negatives remain |

## E. Stop/routing decisions and source verification

Stop if a new branch conflicts with #440, if the live user Editor remains active, if new persistence would corrupt historic first sample or protected identities, if geometry cannot safely derive finite W_i, if actual fixture fails conservation, if candidate quality automation is required but no user decision, if failed #418 would make approval provenance unauditable, or if aggregate BA/canopy/economy/completion destabilises the accepted scenario. Record blocker in claim, return evidence and a bounded proposal; never weaken a gate.

**Suggested future worker:** local Sol/Codex GPT-6.1-Sol High or equally capable Unity/C# coder, **one primary writer**, new branch/worktree/HEAD/claim only after Manager #424 issues it. Separate independent reviewer for save/history/rule derivation, #440 owner for shared UI. Current docs research task requires no user action; **implementation requires safe personal-playtest release and approved four-file spec first**.

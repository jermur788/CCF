# SCENARIO TWO PRE-PRODUCTION AUDIT

**Status: PLANNING — PROPOSAL ONLY. NOT AN ACCEPTED MECHANIC, SCOPE OR DECISION.**
Nothing in this document is accepted by appearing here. It does **not** edit the
Decision Log, does not create Unity files, and implements no Scenario Two code.
It should be treated as reviewable planning input for the user/Overall Manager.

- Task: CCF Scenario Two pre-production design audit (non-Unity planning/requirements).
- Role: non-Unity planning / requirements analyst. Unity required: No. Production code: No.
- Base/context inspected: `ops/workflow-foundation-v1` @ `d3770415514d8834ecaeca57df884e7c4c3792c4`.
- Sources read: `Docs/Project/game-brief.md`, `decision-log.md`, `current-milestone.md`,
  `unity-project-overview.md`, `AgentWorkflow.md`, `research-index.md`,
  `coordination/active-tasks.md`, plus `Docs/BrowsingProtectionV1.md`,
  `Docs/Scenario1EcologyCompletion.md`, `Docs/Stage1WorkEconomyFoundation.md`,
  `Docs/Scenario1EconomyIntegration.md`, `Docs/TimberAssortmentYieldV1.md`,
  `Docs/PruningTimberQualityInterfaceDiagnostic.md`, `Docs/BrowseHistoryFormDamageV16Proposal.md`,
  `Docs/Scenario1LearningObjectives.md`, `Docs/Scenario1UiRedesignAudit.md`.

**Statement labels used throughout** (per the task and project conventions):

- **ACCEPTED EXISTING DIRECTION** — confirmed in the Decision Log / Game Brief / Current Milestone.
- **CURRENT IMPLEMENTATION FACT** — inspected code/document behaviour in the Unity Project Overview.
- **RESEARCH EVIDENCE** — supported material in the existing research pack (`research-index.md`).
- **INFERENCE** — reasoned conclusion drawn from facts, not itself accepted.
- **PROPOSAL** — design suggestion from this audit.
- **OPEN DECISION** — a product/scope choice that requires the user/Manager.

---

## 1. Scenario Two purpose

**ACCEPTED EXISTING DIRECTION.** Scenario Two is a separate hole-play scenario, not a
modification of Scenario One (D-006, D-051, D-052). Forestry scenarios are independent
forests; the player carries knowledge/experience between them, not inventories or the
same forest. The custom sandbox comes *after* the scenario-driven simulation work, not
before Scenario Two (D-052). Scenario progression should teach mechanics rather than
expose everything immediately (Game Brief staged direction).

**ACCEPTED EXISTING DIRECTION.** Scenario One must be completed first; multi-scenario
infrastructure and Scenario Two are explicitly excluded from the current milestone
(Current Milestone "Explicit exclusions"). This audit therefore does not authorise
implementation — it prepares a bounded proposal for a later decision.

**ACCEPTED EXISTING DIRECTION.** The primary reference audience is smaller private
woodland/farm-forest owners and managers, not industrial estates (D-053). Scenario Two
should keep credible trade-offs for that audience.

**PROPOSAL.** Scenario Two's purpose is: *reuse the Scenario One decision→task→time→
observe→review loop at greater scale and structural complexity, and add the management
choices that Scenario One defers (production fencing/protection, access, pruning-for-
quality, retained deadwood/habitat, broader species establishment), so the player learns
that CCF decisions compound across a larger, messier, more constrained holding.*

**INFERENCE.** Because Scenario One defers deer fencing, pruning-quality premium,
broader understorey, adult suppression mortality, storm activation and the broadleaf
market (D-044, D-050), Scenario Two is the natural home for the *first* production
introduction of several of these — but each still needs its own accepted packet.

---

## 2. Scenario One system inventory (Task 1) with Scenario Two reuse classification

Classification key: **REUSE UNCHANGED**, **REUSE WITH NEW DATA/PARAMETERS**, **EXTEND**,
**NEW SYSTEM REQUIRED**, **UNKNOWN**. "Why" is grounded in the inspected overview and
system docs.

| System | Scenario One implementation today (CURRENT IMPLEMENTATION FACT unless noted) | Scenario Two class | Why |
|---|---|---|---|
| Stand generation / geometry | `ForestStartingStand.Generate()` / `GenerateEnlarged80()`; `StandGeometryModel` {Legacy40=0, Enlarged80=1}; `StandWorldBoundary` applies geometry at runtime; `ScenarioOne.asset` definition `scenario-one-v13` | **REUSE WITH NEW DATA/PARAMETERS** | The generator/boundary mechanism is geometry-parameterised but the model enum currently hardcodes two named geometries; a third size needs new data plus a geometry-aware calibration decision (D-057 pattern). |
| Species | `TreeSpeciesDefinition`; Sitka, Oak, Beech | **EXTEND** | D-059 species-parity P0 (adult parity, regeneration/juvenile/browse traits, canopy/light/site traits) is accepted but **not started**. Broader/native species establishment is a Scenario Two proposal needing new data. |
| Growth | `SitkaGrowthModel` Class III curve (growth model 1); adult height growth is species-gated; other species keep legacy height law | **REUSE WITH NEW DATA/PARAMETERS** / **EXTEND** | Site class is a parameter; but Oak/Beech height growth parity is pending (D-059). A different Irish site class or a British/European site needs a labelled evidence decision. |
| Density / mortality | `ApplyAdultDensityMortality` + `StandRelativeDensity` using British Sitka SDI (RD); **not species-gated** (known inspected limitation) | **EXTEND** | D-061 requires ordinal species-aware growing-space-demand classes; the D-061 work is accepted but not implemented. Scenario Two inherits this fix. |
| Regeneration | `RegenerationModel` 1/2; cell age bands, sub-threshold accumulator, promotion handoff; Model 2 bramble/bracken competition | **REUSE WITH NEW DATA/PARAMETERS** | Models are scenario-restricted to Scenario One by calibration (D-049); a new site needs reassessed drivers (site/soil/class), i.e. new data. |
| Planting | `ScenarioOneManager.ResolvePlanting`, exact planted juveniles with ground-selected positions, purchased nursery stock, `ForestPlanting` | **REUSE UNCHANGED** / **REUSE WITH NEW DATA/PARAMETERS** | Mechanism is generic; species/nursery-stock prices are scenario data. |
| Browsing / protection | `BrowsingConditions` (`BrowseProtectedArea`, `BrowseShelter`), `JuvenileEcologyRules` browse path, `backgroundBrowsePressure` 0.2 [C]; `BrowseProtectionGeometry` builds fence-area records only — **no fencing gameplay** | **EXTEND** | The ecology exists and persists (v15), but there is no production fence/work-cost/gameplay. Scenario Two is where fencing becomes a player action (proposal). |
| Pruning | `ForestTree.TryPrune` (max 3 lifts, ≥5 yr apart, current targets 2.5/5/6.5 m [D]); `BatchPruneCropTrees`; `ResolvePruning`; **no quality premium** | **EXTEND** | D-014 designation exists; quality/history recording is a documented diagnostic proposal only (`PruningTimberQualityInterfaceDiagnostic.md`). |
| Marking | `ForestTreeMarkingManager`; exclusive None/Fell/CropTree marks | **REUSE UNCHANGED** | Mechanism is species/geometry independent. |
| Harvesting | `CreateFellingOrder` / `ResolveFelling`; outcomes SellAndExtract / RetainAsFallenDeadwood / KeepForUse; pre-fell volume snapshot | **REUSE UNCHANGED** / **EXTEND** | Mechanism generic; larger/multi-coupe operations and machinery are Stage 2/speculative (see §7). |
| Timber / economy | `WorkEconomy` (`ForestryWorkCalculator`, `ForestryPriceBook`, `Stage1EconomyDefaults`), `TimberYield` assortment allocation, `ScenarioOneEconomyAdapter`; cash in cents; retained timber m³; contractor job minimum €2,500 [C]; start €12,000 | **REUSE WITH NEW DATA/PARAMETERS** | Price book explicitly supports a replaceable scenario deck; broadleaf deck is D-062 pre-demo scope; larger scale changes values, not architecture. |
| Contractors / labour | Contractor vs LandownerSimulated execution per order/profile; owner budget 2,400 min/yr [C]; harvest contractor-only | **REUSE WITH NEW DATA/PARAMETERS** / **EXTEND** | Profiles/eligibility are data; contractor availability windows/choice of multiple contractors is a new proposal. |
| Work plans | `ScenarioOneWorkOrder`, Work Plan UI (P3), `ApprovePendingWork`, quote lifecycle | **REUSE UNCHANGED** | Order/approval/resolution model is scale-agnostic. |
| Objectives | `ScenarioOneObjectives`, 8 parent objectives, geometry-aware effective targets (D-057) | **EXTEND** | Targets are scenario data, but the accessor rejects unknown geometries → needs a new calibration decision for a new property. |
| Completion | `ScenarioOneCompletionVerification`; Year 25, canopy 0.35, century review Year 100 | **REUSE WITH NEW DATA/PARAMETERS** | Completion criteria are Scenario One's; Scenario Two needs its own conceptual conditions. |
| Teaching / guidance | `Scenario1LearningObjectives` (42 sub-objectives), MenuTutorial, Teaching Copy, learning panel | **EXTEND** | Framework is scenario-agnostic (PlayerPrefs keys `CCF.Learning.v1.<step>`), but the *curriculum* is Scenario One-specific and must be authored for Scenario Two. |
| Save / load | `ForestSaveController` / `ForestSaveData` v20, `ScenarioOneSaveMigration`, atomic hardening, geometry validation | **EXTEND** | New persistent Scenario Two state (e.g. multiple stands, access features, browse-management targets) needs schema additions. |
| History / review | `ScenarioAnnualReport`, `ScenarioManagementEvent`, `EcologicalSnapshots`, Annual Review; **S1-C review/history and S1-D repeat-intervention support are not implemented** | **EXTEND** | S1-C/S1-D are pre-demo Scenario One work; Scenario Two depends on the same review/history foundations. |
| Paths / access | A presentation track (`Forest Path.mat`) and walkable ground; **no access-path construction system** | **NEW SYSTEM REQUIRED** | Access/road building is listed only as Stage 2 direction ("fencing, paths and infrastructure"). |
| Deadwood / biodiversity | `ScenarioDeadwoodRecord`, self-thinning and felling deadwood, `ScenarioHabitatInterpretation/Visuals`; ring-barked/defect assets unspawned (D-020) | **EXTEND** | Deadwood state is causal; retained-deadwood/ring-barking *interventions* and habitat targets are new gameplay. |
| Tools / equipment | `WorkTool` flags in WorkEconomy; `ForestPlayer` chop; tool running cost configurable; construction frames hidden (D-055) | **UNKNOWN / EXTEND** | There is no tool-inventory system; whether Scenario Two needs one is an open design question (avoid a crafting ladder, D-012). |
| Player movement / inspection | `ForestPlayer` (New Input System), inspection card, `RegenerationDiagnosis`, Stand Map (new UI direction) | **REUSE UNCHANGED** | Movement/inspection are scenario-agnostic. |

**INFERENCE.** The majority of Scenario One's *mechanics* are reusable with new data.
The genuinely new subsystems Scenario Two is likely to require are: production fencing/
protection gameplay, access-path construction, pruning-quality/history persistence,
broader species/natural-colonisation representation, and scenario-parameterisation of
geometry/objectives. Most other "differences" are configuration, not architecture.

---

## 3. Candidate Scenario Two concepts (Task 2)

Three concepts are offered. None is chosen. **RECOMMENDATION — NOT ACCEPTED DECISION**
is stated at the end of this section.

### Concept A — The neglected mixed farm woodland (under-managed, heavy browse, poor access)

- **Starting situation.** A larger (~1–2 ha) older holding with an irregular mixed
  broadleaf/conifer structure, patchy natural regeneration, long-standing high deer
  pressure, and no usable internal access track.
- **Why harder than Scenario One.** No clean even-aged plantation to thin; browse is
  already high rather than a low pre-protection 0.2; the player must *create* access
  before large work is economic; capital is tighter relative to the work needed.
- **New management choices.** Where to fence first; which regeneration to protect vs
  enrich; whether to prune a crop-tree cohort for future quality; how much deadwood to
  retain; where an access spur pays for itself.
- **What the player already knows.** Inspect → mark → plan → choose executor → advance
  → review → walk; browsing delays regeneration; shelters protect exact plants.
- **What it teaches.** Protection and access are prerequisites for scale; early form/
  quality decisions compound over decades; retention is a deliberate trade-off.
- **Completion conceptually.** The holding reaches continuous cover with viable
  regeneration *and* improved structural/quality/habitat outcomes under demonstrated
  protection and access.
- **Reuses.** Nearly all Scenario One systems with new data + fencing/access/pruning-
  quality extensions.

### Concept B — The larger multi-coupe estate (scale, logistics, cash-flow)

- **Starting situation.** A substantially larger property split into several coupes of
  differing species/age, with contractor availability windows and tight annual cash.
- **Why harder.** Sequencing work across coupes; spreading the €2,500 job minimum over
  larger volumes; managing cash-flow between harvests; choosing among contractors.
- **New choices.** Which coupe to intervene in this year; single large vs several small
  jobs; contractor selection/availability; enrichment timing across the estate.
- **What the player knows.** The single-coupe loop and the economics of a small job.
- **What it teaches.** Scale and logistics change strategy, not just numbers.
- **Completion conceptually.** All coupes are on a viable CCF trajectory and the estate
  stays solvent.
- **Reuses.** Work Plan, economy, marking, harvest — but at larger geometry scale.

### Concept C — Salvage after a storm (recovery and rebuild)

- **Starting situation.** A stand that has just suffered a significant windthrow event,
  leaving a mosaic of damage, salvage value and new openings.
- **Why harder.** Immediate flag fall + long-term rebuild; value recovery vs structural
  retention; risk of further damage.
- **New choices.** Salvage extent/disposition, sanitation vs retention, replant vs allow
  natural regeneration in openings, future risk reduction.
- **What the player knows.** The loop and deadwood/retention ideas.
- **What it teaches.** Disturbance as an opportunity and a risk; salvage vs habitat.
- **Completion conceptually.** The disturbed stand is stabilised and on a viable
  trajectory.
- **Reuses.** Dormant `StormModel 1` core exists (D-050) but requires activation
  profiling; deadwood/economy/work plan.

**Research/design caution.** **RESEARCH EVIDENCE (D-050):** storms are frozen [C]
gameplay calibration; default remains storm 0; activation requires standalone Player
profiling and crown/root acceptance. **INFERENCE:** Concept C therefore carries the
highest "new-system capture" risk of the three.

**RECOMMENDATION — NOT ACCEPTED DECISION.** Recommend **Concept A (neglected mixed farm
woodland)**. It best matches the primary audience (small private woodland/farm-forest,
D-053), reuses the most existing systems, introduces exactly the mechanics Scenario One
defers (fencing, access, pruning quality, deadwood), and does not depend on activating
dormant storm state. Concepts B and C remain possible later scenarios, not this one.

**OPEN DECISION.** The user must choose the concept; this audit does not.

---

## 4. Progression outline for the recommended concept (Task 3)

**PROPOSAL.** 6 stages, ~1 ecological year per action cycle. Unlocks are justified by
the *holding, finances, contractor access or management complexity expanding*, not by an
XP ladder (D-011). Each unlock is a *capability becoming sensible*, not a level.

| Stage | Starting condition | Management problem | Available tools/actions | Newly introduced mechanic | Player decision | Visible consequence | Success / progression condition |
|---|---|---|---|---|---|---|---|
| **1. Assess the mess** | Irregular mixed woodland, poor regeneration, no access, ~0.5 ha worked at a time | Read an irregular structure that has no clean thinning rows | Walk, inspect, Stand Map, Fell/Crop marks, Work Plan | Structure/quality readout for irregular mixed stands (incl. form damage if implemented) | Which stems are crop trees; where light actually reaches the floor | A legible picture of where regeneration is limited by light vs browse | Player has marked a credible crop-tree/removal set and identified a protection/access priority |
| **2. Open the way** | Knowledge from Stage 1; a protected regeneration patch is unreachable/useless | Access and browse block any meaningful intervention | + access-spur construction; + production fencing | Multi-feature layout (fence polygon, access spur) with cost/labour | Where one cheap spur unlocks the most manageable ground; fence the most valuable regeneration first | Spur appears; fenced regeneration escapes browse; driver's yard reachable | First enclosure and access spur complete and demonstrably effective |
| **3. Shape the future trees** | Regeneration now viable; cash tight | Early form/quality decisions compound; no premium yet | + pruning crop-tree cohort (contractor) | Pruning lift history/quality bands recorded (proposal) | Which cohort to prune, when, and how many lifts | Future clear-stem form; visible lift count; later assortment eligibility | A pruned crop-tree cohort with recorded lifts and defined future intent |
| **4. First real harvest at scale** | Access + regeneration in place; volume now justifies machinery | Small jobs lose money; the €2,500 minimum must be spread | + larger/multi-tree commissioned jobs; + contractor availability/choice | Building one job across a coupe with one minimum; choice among contractors | Single large job vs staged small jobs; contractor selection | Cash movement; one settlement; volume actually removed | A commissioned harvest that nets positive/acceptable and does not harm regeneration |
| **5. Diversify and retain** | Structure improving; species/quality mix is still thin | Broadleaf/native establishment and habitat are under-served | + enrichment planting; + natural-colonisation management; + retained deadwood / habitat intervention | Native colonisation from seed sources; deliberate retention targets | Enrich vs allow natural colonisation; how much deadwood to retain | New native regeneration; standing/fallen deadwood; habitat readout | Agreed species/structural/habitat targets met |
| **6. The long horizon** | Multi-decade history; choices have compounding consequences | Keep the holding viable across decades and a possible disturbance | All previous tools; + risk-aware planning | Long-horizon cash-flow/review across the expanded holding | Where to accept lower early return for later quality/resilience | A century review showing intentional management vs unintended consequences | Scenario Two completion: continuous cover + viable regeneration + improved quality/habitat/access under solvent management |

**INFERENCE.** Stages 2–5 each introduce exactly one mechanic, matching the project's
"teach mechanics rather than expose everything" direction and the "one small playable
improvement at a time" rule.

**OPEN DECISION.** Exact number/order of stages, and whether storm disturbance belongs
in Stage 6, are product choices.

---

## 5. Scale / resources / tools differences (Task 4)

Classified as: **existing CCF mechanic**, **existing research-supported extension**,
**new proposal**, **research gap**. (RESEARCH EVIDENCE references are to the existing
research pack named in `research-index.md`; no new web research was performed.)

| Dimension | Scenario One today (CURRENT IMPLEMENTATION FACT) | Scenario Two proposal | Class |
|---|---|---|---|
| Site size | Enlarged80: 80×80 m, 0.64 ha, 256 cells | Larger, irregular property (~1–2 ha), possibly non-square | **existing CCF mechanic** (generator) + **new proposal** (a third geometry + calibration) |
| Starting cash | €12,000 [C] | Tighter relative to work needed; requires new scenario value | **new proposal** |
| Timber inventory | 1,344 trees ≈2,100 stems/ha, age 20 | Older, irregular mixture with variable stocking and some merchantable volume already | **existing CCF mechanic** + **new proposal** (starting data) |
| Species composition | Sitka, Oak, Beech (parity P0 pending) | Broader mix incl. native species; **broader species establishment** | **research gap** (species evidence pack exists; quantitative replacement design separate) |
| Stand structure | Even-aged plantation → CCF | Already irregular/under-managed | **existing CCF mechanic** (structure representation) + **new proposal** (start state) |
| Regeneration state | Model 2 bramble/bracken competition; browse 0.2 | Patchy natural regeneration, heavy browse, colonisation from existing natives | **existing research-supported extension** (D-049 model, Browsing & Protection report) + **research gap** (colonisation rates) |
| Deer / browse pressure | 0.2 low [C]; no player fencing | Moderate/high; player-managed through fencing | **existing research-supported extension** ("Irish_CCF_Browsing_Protection_v1_Report.pdf") |
| Access | Presentation track only; no construction | Player-built access spurs/roads | **new proposal**; Stage 2 direction lists paths/infrastructure | 
| Labour | Contractor or landowner-simulated; 2,400 owner min/yr [C] | Larger labour requirement; contractor availability/choice | **existing CCF mechanic** + **new proposal** (availability/choice) |
| Contractor availability | Implicit; harvest contractor-only | Explicit availability windows, multiple contractors | **new proposal** |
| Machinery / tools | `WorkTool` flags; costs configurable; no inventory | Possibly tool/equipment capability choices (recognisable tools only, D-012) | **new proposal**; avoid crafting ladder |
| Fencing / protection | Ecology + shelters persist; **no fencing gameplay**; fence geometry helper only | Production fencing + shelter gameplay as a core action | **existing research-supported extension** (fencing is [G] in the browsing report) + **new proposal** (productionisation) |
| Pruning | Up to 3 lifts; no premium | Pruning-for-quality as a managed decision with recorded history | **existing research-supported extension** (Teagasc high-pruning guidance) + **research gap** (occlusion/defect-core, premium) |
| Timber-quality management | Sitka assortment; Oak/Beech deck D-062 pending | Higher-value quality/assortment management incl. broadleaf | **existing research-supported extension** (Teagasc/IFA specs) + **research gap** (Irish broadleaf prices) |
| Biodiversity / deadwood | Causal deadwood; retention outcomes; ring-bark assets unspawned | Deliberate retained deadwood / ring-barking / habitat targets | **existing research-supported extension** (ecology reports) + **research gap** (retention levels/species responses) |

---

## 6. Feature candidate matrix (Task 5)

Legend for **RECOMMENDATION**: CORE / OPTIONAL / LATER / RESEARCH FIRST. Statements are
labelled inline.

| # | Candidate mechanic | GAMEPLAY VALUE | ECOLOGICAL / FORESTRY BASIS (existing research) | IMPLEMENTATION DEPENDENCY | SCENARIO TWO FIT | RECOMMENDATION |
|---|---|---|---|---|---|---|
| 1 | Fencing / deer exclusion | High — converts browsing from a fixed pressure to a managed one; clear cost/benefit | **RESEARCH EVIDENCE:** `Irish_CCF_Browsing_Protection_v1_Report.pdf` (fencing is [G]; intact/breached; IS436 ~15 yr horizon noted) | Ecology + persistence exist (v15); needs production work-cost/material + maintenance/inspection | Strong — central Scenario Two choice | **CORE** |
| 2 | Using small timber for stakes / fencing | High — ties harvested small material to protection; closes a material loop | **RESEARCH EVIDENCE:** IFA/Teagasc stakewood specs exist in the timber-yield pack | Stake assortment already exists in `TimberYield`; needs a fence-material→retained-timber link | Strong | **CORE** (bounded) |
| 3 | Pruning for timber quality | Medium–High — long-delayed visible payoff; teaches patience | **RESEARCH EVIDENCE:** Teagasc high-pruning guidance (2-lift 3.5/6 m; may attract premium) | Diagnostic exists; needs lift-history persistence + a non-premium-first adapter | Strong | **CORE (history first), premium later** |
| 4 | Chipping pruning residues for paths | Low–Medium — reuse loop, but risks a crafting system | **RESEARCH EVIDENCE:** pruning residue report notes no authoritative branch volume exists | Needs branch/residue quantity (currently unavailable) + path materials | Weak without a path system | **RESEARCH FIRST / LATER** |
| 5 | Advanced access / path construction | High — prerequisites scale work and ties to access economics | **ACCEPTED DIRECTION:** Game Brief lists paths/access as future progression | New subsystem (terrain/road placement, cost, soil suitability) | Strong | **CORE but SIMPLEST FORM** |
| 6 | Contractor choice / availability | Medium — logistics depth without new architecture | **RESEARCH EVIDENCE:** economics/labour report (contractor operations) | Profiles exist; needs availability data + selection UI | Good | **OPTIONAL** |
| 7 | Larger harvesting operations | Medium — spreads the €2,500 minimum; scale realism | **RESEARCH EVIDENCE:** economics report (job-scale, combined harvest/forward) | Existing Work Plan/economy supports multi-tree jobs; no machinery physics needed | Good | **OPTIONAL** (data, not machinery) |
| 8 | More complicated cash-flow decisions | Medium — planning depth | **CURRENT FACT:** one feasibility witness only (D-057); no NPV model | Needs scenario values, maybe multi-year plan visibility | Good | **OPTIONAL** |
| 9 | Retained deadwood / ring-barking / habitat interventions | Medium–High — ecological story; visible structures | **RESEARCH EVIDENCE:** ecology/deadwood reports; D-020 requires matching causal state | Ring-bark needs a causal state + asset spawn (currently unspawned) | Good | **OPTIONAL** (retained deadwood CORE-ish via existing path; ring-barking LATER) |
| 10 | Broader species establishment | Medium — composition choice | **RESEARCH EVIDENCE:** species evidence pack; **GROWFOR** documents species limits | Needs species-parity P0 + new species data | Good | **OPTIONAL / RESEARCH FIRST** |
| 11 | Natural colonisation of native species | Medium–High — reduces reliance on planting; realistic | **RESEARCH EVIDENCE:** ecology reports discuss colonisation; requires seed sources | Needs seed-source/boundary representation + colonisation model | Good | **RESEARCH FIRST** |
| 12 | Higher-value timber quality management | Medium — visible value from earlier decisions | **RESEARCH EVIDENCE:** Teagasc/IFA specs; D-062 broadleaf assortment throughput | Needs quality model (pruning/form/defect) + broadleaf deck | Good | **OPTIONAL** (after #3 history) |

**INFERENCE.** The CORE set (1, 2, 3, 5) is coherent: each is a Scenario One deferral
with existing ecological/economic foundations and no new physics. Everything requiring
a new terrain/road engine, machinery physics, a crafting system or unvalidated
ecological causation should be deferred or researched first.

**NOT PROPOSED (avoid).** Do not invent ecological causality. E.g. do not claim
chipping residues improves paths, or that fencing guarantees regeneration, without
evidence. The browsing report's own rule ("protection improves outcomes but does not
guarantee success") is the model to preserve.

---

## 7. Research gap analysis (Task 6)

**RESEARCH EVIDENCE / GAP.** Statements below cite the existing pack only. No new web
research was performed. Where the pack supports a proposal it is noted; where it does
not, a precise question is returned.

### Already sufficiently supported (existing pack)

- **Fencing as an access barrier, and individual shelters** — supported by
  `Irish_CCF_Browsing_Protection_v1_Report.pdf` (intact/breached fence; shelters ~6–8 yr).
- **Browsing as leader-height delay with modest mortality** — supported; already
  implemented in `BrowsingProtectionV1`.
- **Multiple timber assortments and roadside/standing accounting** — supported by the
  economics report and Teagasc/IFA spec sheets (already in `TimberAssortmentYieldV1`).
- **Sitka growth/mortality and Class III site potential** — supported (D-048, Lekwadi
  et al.; Comeau et al.).
- **Pruning as a future-quality investment that may attract a premium** — supported as
  **guidance**, not a guarantee (Teagasc high-pruning).

### Additional evidence required (precise questions)

1. At what DBH/age ranges is small-diameter Sitka realistically used for fencing stakes
   in Irish forestry, and what is the typical stake length/diameter specification?
2. What are realistic Irish costs, labour rates and material specifications for deer
   fencing on small private woodlands, and what maintenance/repair interval does Irish
   guidance assume?
3. At what site/deer-pressure levels does fencing change regeneration *outcomes* (not
   just leader risk) over 10–30 years in Irish conditions?
4. For pruning, what defect-core/occlusion model (DBH at cut, largest branch diameter,
   years to occlusion) is defensible for Sitka, and what measured premium evidence
   exists for pruned Irish Sitka?
5. Are pruning residues (branchwood/chips) a realistic material for forestry access
   tracks in Ireland, and what compaction/engineering constraints apply?
6. How quickly do native broadleaves naturally colonise Irish conifer clearings/openings
   from existing seed sources, and what site/soil conditions govern it?
7. What retained-deadwood volumes and ring-barking practices are supported by Irish/
   Atlantic woodland evidence for biodiversity, and over what timescales?
8. What is a defensible broadleaf (Oak/Beech) assortment-and-price basis for Ireland
   where no established broadleaf price series was found (D-062)?
9. What are realistic Irish contractor availability/seasonal windows and mobilisation
   behaviour for small private woodland jobs?
10. What site/soil classes are suitable for low-cost forest access spurs in Ireland, and
    what are representative construction costs and drainage requirements?

**OPEN DECISION.** Whether/when to commission these as bounded research tasks.

---

## 8. Architecture risk (Task 7)

**PROJECT RULE (ACCEPTED).** Small playable improvements; no speculative
infrastructure; avoid unnecessary managers/singletons/frameworks; prefer built-in
Unity features.

| Temptation | Risk | Simplest compatible approach (RECOMMENDATION) |
|---|---|---|
| Generalised scenario framework / registry / abstract `Scenario : MonoBehaviour` base | High — speculative infrastructure explicitly excluded from current milestone | Keep Scenario Two as a separate scene/definition first; only extract shared parameters when a *second* scenario actually needs them (pressure-point list in §9). |
| Broad scenario-parameter system in one refactor | High — touches `ScenarioOneManager`, saves, objectives, geometry | Add parameters incrementally where the data already lives (`ScenarioOneDefinition`, `StandGeometryModel`, `ForestryPriceBook` id); do not pre-build. |
| New save schema for many features at once | High — schema is v20 with strict validation/migration conventions | Add the minimum fields per accepted feature; version bump per persisted addition (project convention). |
| Networking assumptions | High — D-022/D-034 defer multiplayer | Preserve method-separation boundaries only; build no networking. |
| New terrain system | High — current terrain is a runtime-scaled ground plane + boundary ridges | Reuse the runtime boundary/geometry approach; a larger/non-square property needs a geometry decision, not a new terrain engine. |
| Machinery physics | High effort, low teaching value vs cost | Keep machinery abstract (contractor capability/productivity data), as today. |
| Procedural construction framework | High — construction is hidden (D-055) and not in scope | Do not introduce construction/crafting; use direct material placement (fencing/access) as bounded features. |
| Broad inventory / crafting system | High — D-012 forbids a stone-age crafting ladder | Reuse recognisable tools/materials; no inventory subsystem unless a bounded feature proves it necessary. |
| Per-tree growth history (e.g. 6-year ΔDBH for UI) | Medium — new persisted state | Show last year only, or presentation-side reset-on-load, until a save task is authorised (already flagged in the UI audit). |

**INFERENCE.** Most Scenario Two depth can be achieved by (a) new scenario data, (b) two
or three bounded new feature systems (fencing/access/pruning-history), and (c) reusing
the existing economy/ecology. That keeps the architecture risk low.

---

## 9. Scenario data vs code (Task 8)

**PROPOSAL.** Identify future pressure points only. **Do not implement a generic
scenario framework.**

### Where Scenario One is already reusable enough

- **`ScenarioOneDefinition` asset** (`ScenarioOne.asset`, definition `scenario-one-v13`)
  already externalises browse pressure, prices, targets, owner minutes and material
  values — new *values* are data.
- **`ForestryPriceBook` / `Stage1EconomyDefaults`** explicitly support a replaceable
  scenario deck (new price-book ID).
- **`TreeSpeciesDefinition`** and species assets are data-driven.
- **`ForestryTask` / `Quote` / `Resolve`** separate execution method from task, so
  contractor vs landowner vs later manual is already a policy layer, not new code.
- **Learning progress** uses `CCF.Learning.v1.<step>` PlayerPrefs, independent of forest
  saves, so a new scenario can have its own curriculum without sharing state.

### Future pressure points (where Scenario Two may need parameterisation)

1. **`StandGeometryModel` is a two-valued enum** (Legacy40/Enlarged80) with hardcoded
   cell counts, `StandWorldBoundary` scaling, and geometry-aware effective targets that
   *reject unknown geometry*. A third geometry is a pressure point; adding one is a
   calibration decision (D-057 pattern), not merely data.
2. **`ScenarioOneManager` / `ScenarioOneObjectives` / `ScenarioOneDefinition` are named
   "ScenarioOne"** and couple scenario logic to one class/asset. A second scenario will
   expose whether a shared base or a sibling implementation is cheaper — decide *when*
   the second scenario is real, not now.
3. **Objectives/completion** are scenario-specific; effective-target accessors currently
   assume geometry variants of one scenario.
4. **Starting stand generation** (`ForestStartingStand`) has Scenario-One-specific
   generators; a differently structured start needs new generator data.
5. **Startup/title flow** (`StandaloneSessionMenu`, D-058) currently offers one scenario;
   scenario selection is a future UI/data pressure point.
6. **Save identity**: `scenario-one-v13` definition string and geometry-scoped
   validation; a second scenario needs an explicit scenario identity in the save.

**RECOMMENDATION.** Treat Scenario Two differences as **configuration first, code only
when a convincing shared abstraction exists**. Specifically: new scenario values/deck,
new starting-stand data, a new geometry entry (with its own calibration), and a new
objectives/completion definition. Do not refactor `ScenarioOneManager` into a framework
until the second scenario demonstrates the real shared surface.

---

## 10. Likely implementation sequence (Task 9.9)

**PROPOSAL.** Ordered, bounded, each with its own accepted packet and verification tier.
This is *not* authorised work.

1. **Complete Scenario One** (D-060): species-parity P0 (D-059/061/062) → S1-C → S1-D →
   completion → integrated qualification. Scenario Two does not start before this.
2. **Concept + scope decision** (user/Manager): choose a concept; write the Scenario Two
   scope/milestone entry and its own accepted decisions.
3. **Scenario data foundation**: new `ScenarioOne`-equivalent definition asset, price
   deck id, objective/completion definition, starting-stand data and a geometry decision
   (with calibration). Mostly data; no new framework.
4. **First playable slice (Stage 1–2)**: irregular-stand assessment + access spur +
   production fencing, reusing ecology/economy. Smallest end-to-end loop.
5. **Pruning-quality history** (bounded): record lift history without an automatic
   premium; adapt to `TimberYield` quality flags; prove baseline unchanged.
6. **Scale/economy slice (Stage 4)**: contractor choice/availability, larger commissioned
   jobs, cash-flow planning — data-driven where possible.
7. **Enrichment / colonisation / habitat slice (Stage 5)**: broader species + natural
   colonisation (research-first) and retained-deadwood/ring-barking interventions.
8. **Long-horizon / completion** (Stage 6): Scenario Two completion conditions, review/
   history, teaching curriculum, and integrated qualification.

**INFERENCE.** Steps 4–7 each map to the CORE/OPTIONAL features in §6 and should be
sequenced by teaching value and evidence readiness, not by novelty.

---

## 11. Decisions the user must make (Task 9.10)

- **OPEN DECISION:** Which Scenario Two concept (A/B/C or other)?
- **OPEN DECISION:** Scenario Two property size/shape, site class and structure.
- **OPEN DECISION:** Which deferrals (D-044/D-050) become Scenario Two scope: fencing,
  pruning premium, broader understorey, adult suppression mortality, storm activation,
  broadleaf market, browse history/form damage.
- **OPEN DECISION:** Whether storm disturbance is part of Scenario Two and, if so, whether
  `StormModel 1` activation profiling is required first.
- **OPEN DECISION:** Starting cash, price deck, owner-minutes and contractor-minimum
  values for Scenario Two (calibration).
- **OPEN DECISION:** Objectives/completion conditions and the teaching curriculum.
- **OPEN DECISION:** Whether to commission the §7 research questions before design.
- **OPEN DECISION:** When (if ever) to extract shared scenario abstractions vs keep a
  sibling implementation (architecture timing).

## 12. Items safe to defer (Task 9.11)

- Multi-scenario framework/registry, scenario selection UI, sandbox — explicitly later
  (D-052, Current Milestone exclusions).
- Networking / cooperative play (D-022/D-034).
- Machinery physics and any crafting/inventory system (D-012; avoid).
- Chipping residues for paths; full pruning-quality premium (research first).
- Ring-barking assets until a matching causal state exists (D-020).
- Broadleaf price series until D-062 evidence is settled.
- Brokerage-grade performance/optimisation work; nothing here implies a Player/performance
  claim.
- A third geometry calibration until a concrete property is chosen.

---

## Handoff

```text
IMPLEMENTATION HANDOFF
Task: CCF Scenario Two pre-production design audit (planning only)
Worker: DeepSeek 4.1 Flash (NonUnity planning/requirements analyst)
Role: Non-Unity planning / requirements analyst
Branch: ops/workflow-foundation-v1
Base: d3770415514d8834ecaeca57df884e7c4c3792c4
Context commit: d3770415514d8834ecaeca57df884e7c4c3792c4
HEAD: d3770415514d8834ecaeca57df884e7c4c3792c4 (no commits; documentation only)
WHAT CHANGED:
- Added Docs/ScenarioTwoPreProductionAudit.md (planning report + handoff).
FILES CHANGED:
- Docs/ScenarioTwoPreProductionAudit.md (new; not yet committed)
SERIALIZED UNITY FILES TOUCHED:
- None.
PLAYER-FACING RESULT:
- None (no production code, no Unity files).
SIMULATION / SAVE IMPACT:
- None. No schema, ecology, economy, geometry or reference change.
ASSETS ADDED OR MODIFIED:
- None.
MUST-NOT-CHANGE HASHES / ANCHORS:
- All Scenario One model/geometry/reference anchors unchanged (no code touched).
INTENTIONALLY CHANGED HASHES / ANCHORS:
- None.
VERIFICATION RUN:
- No Unity. Repository inspected read-only; git status clean pre-write.
VERIFICATION RESULTS:
- Not applicable (planning deliverable).
REMAINING VERIFICATION GATES:
- User/Manager review and acceptance of any proposed direction.
KNOWN LIMITATIONS:
- No new web/field research was conducted; research statements cite the existing pack only.
- Scenario/objective/price values are illustrative proposals, not calibrated.
- Concept/progression/feature recommendations are NOT ACCEPTED DECISIONS.
UNRELATED WORK PRESERVED:
- None touched; dirty/main free; other lanes and preservation holds untouched.
DECISIONS NEEDED:
- See section 11 (concept, size/site, deferral scope, storm inclusion, calibration,
  objectives, research commissioning, architecture timing).
WORKER ASSESSMENT:
- Ready for review: Yes
RECOMMENDED NEXT STEP:
- User/Manager reviews sections 2, 3 and 11; no Scenario Two work begins before
  Scenario One completion (D-060) and an explicit accepted concept decision.
```

**CLOUD/NON-UNITY HANDOFF — NOT UNITY-VERIFIED.** No Unity editor, Player or build was
run for this task. This is planning/requirements analysis against canonical documents
and the existing research pack only.

---

*End of Scenario Two pre-production audit. Nothing here is an accepted mechanic,
scope or decision. Do not edit the Decision Log from this document; open decisions are
listed in section 11.*

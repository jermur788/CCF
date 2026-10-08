# Decision matrix

**Status:** classification for Manager/user review. **Nothing in this folder becomes accepted by appearing here.** Recommendations stay recommendations until the user accepts them and the Decision Log records them.

Classes:

- **ALREADY ACCEPTED** — Decision Log entry.
- **CURRENT IMPLEMENTATION FACT** — verified in code at `a8596df`.
- **SUPPORTED DIRECTION** — consistent with accepted decisions and evidence; not decided in this form.
- **RECOMMENDED NEW DECISION** — this study recommends it; the Manager can accept it within existing authority (no product-level trade-off).
- **PRODUCT DECISION REQUIRED** — a genuine user choice with trade-offs.
- **DEFERRED** — consciously not now.

## 1. Already accepted (basis)

| Item | Decision |
|---|---|
| CCF central; decide → advance → walk → understand | D-003, D-004 |
| Work Plan reviews and approves; spatial decisions made walking | D-010 |
| Fell/Crop exclusive marks; Crop Tree pruning batches | D-013, D-014 |
| Presentation derives from authoritative state | D-020 |
| Choices inside interfaces, not shortcut accumulation | D-021 |
| Scenario One functionally complete; deferrals (fencing, storms, understorey, browse history, …) | D-043, D-044 |
| New-game model stack RNG 1 / regen 1 / growth 1; Reference Future v1 frozen | D-046, D-047, D-048, D-042 |
| No universal prescription (Game Brief) | Game Brief |

## 2. Current implementation facts (that decisions must respect)

| Fact | Where |
|---|---|
| 8 forest objectives; 5 met without management under growth 1 (deadwood via self-thinning) [INF from Sol's run] | `ObjectiveDependencyGraph.md` |
| Cash dead end before the first thinning; failure reported only at Year 100 | `PlayerStallPoints.md` S1 |
| Learning/help progress is per device | `MenuHelpView`, `LearningObjectivesView` |
| Clearance preview on every ground aim | `ForestPlayer.cs:438` |
| Wind label saturated; competition label nearly saturated | [HARNESS-Y0] |
| Hegyi competition (DBH/distance, 8 m), not crowns | `ForestEcologyController` |
| Every Year-0–25 thinning loses money on 0.16 ha | [HARNESS-Y0], Sol |
| Sapling price: definition €4.50/€5.50 vs price book €0.95/€1.00 | `ScenarioOneDefinition`, `Stage1EconomyDefaults` |
| Fixed new-game seed 20260914 | `ForestEcologyController` |
| P1 copy and Model 2 are candidates, not on main; Model 2 parameters are not approved | branches `1a36ea9`, `902903f` |

## 3. Matrix

| # | Item | Class | Recommendation | Ref |
|---|---|---|---|---|
| R1 | Integrate P1 copy first (re-gate at v17) | RECOMMENDED NEW DECISION | Yes | ImplementationRoadmap |
| R2 | P2 presentation **C** (ranked list + temporary numbered in-world tags, gated on a manual review) | RECOMMENDED NEW DECISION | Yes, list first | CropTreeCompetitorDesign |
| R3 | Release metric as CI change; reject "meaningful competitors removed" | RECOMMENDED NEW DECISION | Yes | CropTreeReleaseMetric |
| R4 | P3 "What you are leaving" block, AVAILABLE-NOW metrics only | SUPPORTED DIRECTION (pedagogy #15) | Yes | ResidualStandReview |
| R5 | Spatial descriptor: facts + a label only when robust ("One concentrated gap" / "Spread out" / "Mixed") | RECOMMENDED NEW DECISION | Yes | SpatialPatternFeedback |
| E1 | No single forestry score; six fixed dimensions; no aggregate | **PRODUCT DECISION REQUIRED** (how the game feels; pedagogy paper 5) | Accept | MultidimensionalFeedback |
| G1 | Annual Review v2 IA with PLACES TO INSPECT (no recommendations) | RECOMMENDED NEW DECISION | Yes | AnnualReviewV2 |
| H1 | Forest Diary v1: hybrid (whole forest + place history), no save change | SUPPORTED DIRECTION (pedagogy #18) | Yes | ForestDiaryV1 |
| H2 | Snapshot additions (standing volume, Crop Tree CI/DBH, cell regeneration bits) | **PRODUCT DECISION REQUIRED** (save) | Defer until playtest evidence | ForestDiaryV1 §4 |
| H3 | Sample plots | DEFERRED (pedagogy #19) | Phase 2 | ForestDiaryV1 |
| I1 | Map cell history (read-only, waypoint only) | RECOMMENDED NEW DECISION | Yes | MapHistoryDesign |
| J1 | Model 2 parameter/target rule acceptance | **PRODUCT DECISION REQUIRED** (Manager/ecology; Sol's handoff) | Not this study's call. Teaching assumes the candidate | RegenerationModel2Teaching |
| J2 | Model-dependent clearance copy (exact P1 replacement sentence) | SUPPORTED DIRECTION (Sol TutorialHandoff) | Yes, on M2 integration | RegenerationModel2Teaching |
| J3 | Clearance preview only after U | **PRODUCT DECISION REQUIRED** (pedagogy #9) | Yes | PlayerStallPoints S9 |
| K1 | Lift the D-044 storm deferral | **PRODUCT DECISION REQUIRED** (storm #1) | Storm study recommends yes | StormTeaching |
| K2 | Storm entry: organic + event-driven explanation (no scripted storm) | RECOMMENDED NEW DECISION | Yes | StormTeaching §3 |
| **PD-K1** | **Fixed seed → severe storm in Year 3 of every game under proposed defaults.** Choose: calibrate + grace 6 / per-game seed / different roll ids | **PRODUCT DECISION REQUIRED** | Grace 6 + calibrate (first storm Year 13) | StormTeaching §2 |
| L1 | Replace counters with stages S1–S12; Tier A (UI only) first | RECOMMENDED NEW DECISION (Tier A) | Yes | ObjectiveRedesign |
| L2 | Per-forest stage progress (save record) | **PRODUCT DECISION REQUIRED** (pedagogy #8; save queue) | Yes, after v19 | ObjectiveRedesign §4 |
| L3 | Broadleaf requirement: both species / **one species surviving 5 years** / none | **PRODUCT DECISION REQUIRED** (pedagogy #22) | One species, surviving 5 years | ObjectiveRedesign §5 |
| M1 | Second-look trigger: RD ≥ 0.6, light-limited regeneration, storm or planting loss; floor 5 yrs; fallback 12 yrs | RECOMMENDED NEW DECISION ([C] thresholds) | Yes, after calibration | SecondInterventionDesign |
| M2d | Explicit "No work needed this year" decision event | **PRODUCT DECISION REQUIRED** (save; pedagogy #24) | Later | SecondInterventionDesign §3 |
| N1 | Completion model **D** (two cycles + guardrails + renewal) | **PRODUCT DECISION REQUIRED** (anchor change) | D | ScenarioCompletionDesign |
| N2 | Remove the deadwood-volume objective from completion | **PRODUCT DECISION REQUIRED** | Yes (report it instead) | ObjectiveRedesign |
| O1 | No hard fail; Year-100 outcome reworded; dead-end detection + prevention warning | **PRODUCT DECISION REQUIRED** | Yes | FailureRecoveryDesign |
| O2 | Recovery route for the cash dead end: standing sale / net settlement / none | **PRODUCT DECISION REQUIRED** (economy owner) | Standing sale, if any | FailureRecoveryDesign §3 |
| P1e | State the €2,500 minimum as a scenario rule; explain scale | RECOMMENDED NEW DECISION | Yes | EconomyLearningSequence |
| P2e | Sapling price inconsistency (definition vs price book) | **PRODUCT DECISION REQUIRED** (economy owner) | Document or align | EconomyTeachingAudit E2 |
| Q1 | Marteloscope: B-lite plan comparison now; C training mode later; **drop** the practice copy of the real forest with time advance (oracle) | **PRODUCT DECISION REQUIRED** (challenges the Manager's direction C) | As stated | MarteloscopeFinalDesign |
| Q2 | Authored training storm in training mode only | **PRODUCT DECISION REQUIRED** | Yes, labelled | MarteloscopeFinalDesign |
| R6 | Stem-form/quality flag (poor-form competitor case) | **PRODUCT DECISION REQUIRED** (save) | Not for Scenario One | TrainingStandV2 |
| S1p | Run beginner playtest 1 after P3 | RECOMMENDED NEW DECISION | Yes | BeginnerPlaytestProtocol |
| T1 | Forester review (no economy balancing) | RECOMMENDED NEW DECISION | Yes | ForesterReviewProtocol |
| U1 | Canonical terms and glossary | RECOMMENDED NEW DECISION | Yes | TerminologyAudit |
| V1 | Readability blockers V1–V3 before playtest | RECOMMENDED NEW DECISION | Yes | ReadabilityAudit |
| W1 | Asset packets 01 and 07 before playtest 1; 03/04 with M2 | SUPPORTED DIRECTION (Sol's priorities) | Yes | PlayerFacingAssetGaps |
| X1 | Three-tier Definition of Done | RECOMMENDED NEW DECISION | Yes | ScenarioOneDefinitionOfDone |
| Y1 | Post-scenario order: training → Stage 2 slice → art → Scenario Two → survival → multiplayer | **PRODUCT DECISION REQUIRED** | As stated | PostScenarioRoadmap |
| D1 | Bio Tree, fencing gameplay, browse history, pruning premium, broadleaf market, sample plots, player-declared goals | DEFERRED | — | D-044 + this study |

## 4. Product decisions, in the order they block work

1. **E1** no single score (blocks P3/P4 copy tone; cheap to decide).
2. **J3** clearance preview behind U (P5 / M2 teaching).
3. **PD-K1** storm seed/grace (before the storm calibration finishes).
4. **K1** lift the storm deferral (Sol's packet precondition).
5. **L2, L3, N1, N2, O1** progression, completion and failure (block P6).
6. **O2, P2e** economy owner items.
7. **Q1, Q2** marteloscope direction.
8. **Y1** post-scenario order.

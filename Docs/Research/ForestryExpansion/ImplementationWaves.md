# Implementation waves, performance, and the forestry-mature gate (Parts 14, 15, 17)

**Status:** proposal. Generalisation steps G1–G10 refer to `SpeciesTraitArchitecture.md` §5. Each wave is a set of bounded packets with the usual AgentWorkflow gates (anchors, determinism, save/load, regression). Scenario One anchors must stay bit-identical under its existing model stack throughout.

## 1. Waves

### W0 — Forestry Foundation

| Field | Content |
|---|---|
| Player outcome | None visible in Scenario One (neutral). New-model test stands show species-specific shade and site effects |
| Systems | G1 species tags / literal-id removal; G2 shade casting; G3 additive relative density; G4 site map + tolerance envelopes; `forestModel` selector (one version bump); scenario-start authoring stub (only what tests need) |
| Species | Existing three (parameters given transmission/site/density classes) |
| Dependencies | Scenario One P2–P6 settled (UI owner); no other ecology writer |
| Research | R2, R6, R7 at ordinal level; R5 for the group defaults |
| Tests | Pure-Sitka bit-identity under the new model; beech-vs-birch shade fixture (once birch exists, a synthetic transmission fixture before that); site-neutral Scenario One identity; determinism; save/load v-next; Reference v1 untouched |
| Art | None |
| Performance risk | Low (per-tree transmission is a multiply; additive RD is O(N)) |
| Completion | All anchors preserved under old models; new-model fixtures pass; documented calibration table |

### W1 — Species Wave 1 (native strategies)

| Field | Content |
|---|---|
| Player outcome | In test stands and Scenario One's surroundings: birch flushes in large wet openings, rowan held back by deer, holly persisting under oak/beech. Oak and beech regenerate naturally from retained seed trees |
| Systems | G5 bird long-tail dispersal; G6 senescence; species tags in UI; P2/P3 species lines (competitor species, seed trees by species) |
| Species | **Downy birch, rowan, holly** + oak/beech as full participants |
| Dependencies | W0 |
| Research | R2, R7, R8, R9, R10 for the three species (ordinal + calibration) |
| Tests | Strategy contrast fixtures (MixedStandDesign §6, 2–4); browse bottleneck; birch short life; seed source dependence ("broadleaves do not appear simply because light rises" [R:SD]) |
| Art | 3 species × (seedling, sapling, pole, mature, stump) on the existing broadleaf pipeline; LOD budget per the asset audit |
| Performance risk | Low–medium: more bands per cell (≤ 5 per species × origin) |
| Completion | Fixtures pass; forester review of trajectories (direction only); readable in rendered review |

### W2 — Mixed-Stand Wave

| Field | Content |
|---|---|
| Player outcome | Mixtures matter: who regenerates under whom, which trees to favour by species, composition shifts visible in the Diary |
| Systems | Species composition and layer diagnostics (map layer, Annual Review, Diary); Crop Tree species line; storey-aware inspection; additive density tuned on mixed fixtures |
| Species | W1 set + Sitka/oak/beech |
| Dependencies | W0, W1; P4 (Diary) integrated |
| Research | R5 validation; R15 forester review |
| Tests | Mixed self-thinning; composition trajectories over 100 years in 3–5 fixtures; determinism |
| Art | Map/UI only |
| Performance risk | Low |
| Completion | Beginner playtest shows players can say *why* composition changed (protocol extension); forester review: no MISLEADING trajectory |

### W3 — Species Wave 2 (productive transformation conifers)

| Field | Content |
|---|---|
| Player outcome | Underplanting decisions with real consequences: tolerant conifers (hemlock, cedar) vs semi-tolerant Douglas fir vs light-demanding Scots pine; new timber value |
| Systems | G7 species height curves; G8 timber registry + generic adapter; T1/T3 markets |
| Species | **Douglas fir, western hemlock, western red cedar, Norway spruce, Scots pine** |
| Dependencies | W0–W2; **research R1, R3, R4** (BLOCKS) |
| Tests | Height curves reproduce source anchors; timber quotes per species; economy anchors (Sitka) unchanged |
| Art | 5 conifers × stages (conifer pipeline exists for Sitka; substantial) |
| Performance risk | Medium (art memory and draw calls; see §2) |
| Completion | Calibrated against R1 anchors; forester review |

### W4 — Quality / Timber Depth

| Field | Content |
|---|---|
| Player outcome | Crop Tree selection by form; pruning pays (where a market exists); browse forks visible and costly |
| Systems | Per-tree `QualityState` (version bump), browse-history persistence (D-044 item), T2 low-grade outlets, T4/T5 quality hardwood markets |
| Species | Oak, beech, + Wave C productive broadleaves (**sycamore, wild cherry**) |
| Dependencies | W3 (registry); research R4, R11 |
| Tests | Yield sections from quality; save/load; anchors under the old model |
| Art | Form variants (bent, forked) tied to state (D-020) |
| Performance risk | Low |
| Completion | Pruning and form decisions show in quotes, with no fabricated premium |

### W5 — Additional Forestry Scenarios

Scenario Two (mixed transformation), then Three (native restoration: adds **alder** with wet sites at full strength, **hazel** resprouting G9), then Four (irregular productive). Each scenario has a packet set, completion design and playtest (`ScenarioRoadmap.md`).

### W6 — Custom Forestry Sandbox

After Scenario Two's authoring format exists and W1–W3 species are calibrated. Mostly UI over the scenario format.

### W7 — Player / Forester Validation (continuous)

At the end of W1, W2, W3 and each scenario: beginner playtest (concept codes extended to species/site), forester review (plausibility of trajectories, terminology). **Gate items below.**

## 2. Performance and scale (Part 15)

Current evidence [repo handoffs]:
- annual simulation step (fresh loaded worlds, Model 2): 37 ms (336 adults), 273 ms (1,300), 766 ms (3,000), 1,515 ms (5,000);
- storm-on 5,000-tree step ≈ 1.33–1.39 s [WindthrowV1 Handoff].

Growth is super-linear: 15× trees costs ≈ 41× time, consistent with the O(n²) Hegyi pair loop and O(cells × trees) canopy.

| Area | Effect of more species | Structural limit? |
|---|---|---|
| Competition | **None** (species-blind pairs) | O(n²) in *tree count*: the limit for larger properties, not species |
| Canopy light | +1 multiply per tree | No |
| Seed rain | Each tree contributes only to its own species; a per-species filter pass O(S × N) | Mild; restructure to bucket trees by species once S > ~8 |
| Regeneration bands | ≤ 5 per species × origin × cell | Linear in S. At 15 species × 2 × 64 × 5 = 9,600 bands max in a 0.16 ha stand; scales with area |
| Save size | Bands and trees dominate; Model 2 save ≈ 300 KB for 64 cells | Linear in S and area; acceptable |
| UI | Species lists in map/review | Design for ≤ 15 entries; group by strategy |
| Visual catalog / memory | **Main risk**: each species × life stages × LODs × materials | Profile per wave; enforce the broadleaf LOD budget from the asset audit |
| Long-run testing | Matrix grows with species × scenarios | Fixture-based tests per strategy, not per species × everything |

**Do not optimise speculatively.** The real limit is tree count (area), which matters for Scenario Two's size. Measure there first. A spatial grid for Hegyi is the obvious lever, and is only justified by a measured need.

## 3. The forestry-mature gate (Part 17)

CCF's forestry side is **mature enough to begin Stage 2** when **all** hold:

| # | Criterion | Evidence |
|---|---|---|
| M1 | Strategies, not counts: at least one calibrated representative each of pioneer, shade-tolerant canopy, shade-tolerant understorey, long-lived slow canopy, **and ≥ 2 productive conifers besides Sitka** | W1 + W3 complete |
| M2 | Mixtures matter: shade casting, species-aware density, site matching, seed-source dependence all active and fixture-verified | W0–W2 tests |
| M3 | ≥ 2 scenarios beyond Scenario One's plantation case, with different starting structures, completed with completion designs and playtests | W5 (Two + one of Three/Four) |
| M4 | Timber value by species, with no fabricated prices, and a quality system that makes Crop Tree form and pruning matter | W3 + W4 |
| M5 | Repeated management is legible: history/diary, second interventions, completion based on demonstrated management | Scenario One P4–P6 pattern applied to every scenario |
| M6 | **Players understand it:** beginner playtests meet the concept criteria for species/site/mixture questions ("why did birch take this gap?", "why did you underplant hemlock here?") | W7 |
| M7 | **Foresters accept it:** forester review finds no MISLEADING trajectory or causal claim for the implemented species | W7 |
| M8 | Engineering health: all anchors and model versions documented, Reference v1 untouched, performance within budget on the largest scenario | Regression + profile |

Species count is a consequence, not the gate. M1 implies roughly 9–11 species in total.

**Then Stage 2 begins with FRUIT AND NUT TREES**, as the first Stage 2 slice. They reuse the species/site/planting/quality architecture: hazel and sweet chestnut bridge directly. Ponds, construction, wider landscaping and other land-management systems follow only after that first slice.

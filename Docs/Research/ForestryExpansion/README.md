# Forestry simulator expansion programme

**Branch:** `task/forestry-simulator-expansion` · **Base / context:** `341ccbf1b2877e8bf21c16761b883a0936a5f7b7` (StormModel1 dormant, save v19, RNG 1, regeneration 2, growth 1).

**Status:** research and architecture for Manager review. **No production file changed. Unity and Blender were not run.** Evidence: static code inspection at `341ccbf`, the project's research PDFs (text-extracted locally), and existing handoff evidence.

**Accepted user direction recorded here (not yet canonical):**
- CCF becomes a fully fledged CCF simulator before broad Stage 2;
- Stage 2 starts with fruit and nut trees.

## Read in this order

1. `CurrentSpeciesArchitectureAudit.md`: what is generic, data-driven, Sitka-specific or hard-coded.
2. `ForestrySimulatorDefinition.md`: what "fully fledged" means (essential / later / deferred), including the management tools (Part 10).
3. `SpeciesCandidateMatrix.md`: candidates and Waves A/B/C.
4. `SpeciesTraitArchitecture.md`: trait model, site model, generalisation order G1–G10, save impact.
5. `MixedStandDesign.md`: mechanics that make mixtures matter.
6. `TimberQualityEconomyRoadmap.md`: timber registry, markets (no fabricated prices), quality state.
7. `ScenarioRoadmap.md`: starting conditions, scenario progression, sandbox timing.
8. `ResearchBacklog.md`: ranked; what blocks Wave B.
9. `ImplementationWaves.md`: W0–W7, performance, the forestry-mature gate, Stage 2 first slice.
10. `ManagerHandoff.md`: proposed canonical text and decisions.

## Key findings

1. **No ecology rewrite needed.** Growth, competition, regeneration, seed, browse, promotion, storms and save are species-data driven.
2. **Light is species-blind**: the core mixed-stand gap.
3. **Growth Model 1's Sitka density law kills broadleaves too**; and Sitka and the other species follow different height laws.
4. **No site state exists** (`SiteProductivity` ≡ 1, unsaved).
5. **Timber and price structures are species-keyed**; only the Sitka content and one adapter are hard-coded.
6. **Research covers native broadleaves, not productive conifers.** Wave A (birch, rowan, holly) can start on existing evidence plus calibration. Wave B (Douglas fir, hemlock, cedar, Norway spruce, Scots pine) is research-blocked.
7. **Save:** new species need no schema change. One model-selector int for the Foundation; a version bump only for tree quality and standing deadwood.

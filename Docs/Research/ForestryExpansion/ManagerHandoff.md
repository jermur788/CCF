# Manager handoff — forestry simulator expansion

## 1. Summary

- The current architecture can grow into a multi-species CCF simulator **without replacing the ecology**. Growth, competition, regeneration, seed, mast, browse, promotion, storms and save are already species-data driven.
- Five bounded gaps block credible mixtures:
  - species-blind shade;
  - a Sitka density law applied to all species;
  - two height laws;
  - no site state;
  - Sitka-only timber content and about 30 literal species checks.
- None needs an unavoidable save migration: one model-selector int for the Foundation; a later version bump for tree quality.
- Wave A (birch, rowan, holly) is supported by existing project evidence.
- Wave B (productive conifers) is **blocked on research**: the project has no evidence on any conifer except Sitka.

**No stop condition is triggered.**

## 2. Proposed canonical changes (exact text; not applied)

### Decision Log — new entries

| ID | Date | Topic | Status | Decision | Implication |
|---|---|---|---|---|---|
| D-0xx | 2026-10-08 | Forestry before Stage 2 | **Confirmed (user direction)** | Before broad Stage 2 land management, CCF becomes a fully fledged continuous-cover forestry simulator. Scenario One is the first reference scenario, not the extent of forestry. The next major domain expansion deepens forestry with more species and mixed-stand management | Stage 2 starts only after the forestry-mature gate (`Docs/Research/ForestryExpansion/ImplementationWaves.md` §3) |
| D-0xx+1 | 2026-10-08 | Stage 2 first slice | **Confirmed (user direction)** | Within Stage 2, **fruit and nut trees come first**; ponds, construction, wider landscaping and other systems follow | Reuses species/site/planting/quality architecture |
| D-0xx+2 | — | Forestry expansion programme | **Proposed** | Adopt W0 Foundation → W1 native strategies (birch, rowan, holly) → W2 mixed-stand → W3 productive conifers (research-gated) → W4 quality/timber → W5 scenarios → W6 sandbox, with continuous player/forester validation | Each wave needs its own accepted packets |
| D-0xx+3 | — | Forestry-mature gate | **Proposed** | Gate M1–M8 (strategies, mixtures, ≥ 2 further scenarios, species timber without fabricated prices, legible repeated management, player and forester understanding, engineering health), not a species count | Defines when Stage 2 may start |

### Game Brief — edit to "Staged gameplay direction", after the Stage 1 paragraph

> Stage 1 grows from Scenario One into a fully fledged continuous-cover forestry simulator: multiple species with distinct strategies, mixed and irregular stands, species–site matching, natural regeneration and enrichment planting, repeated selective management, species-specific timber and quality, and several forestry scenarios. Broad Stage 2 land management begins only when this forestry depth is demonstrated and understood by players.
>
> Stage 2 begins with **fruit and nut trees**, before ponds, construction, wider landscaping and other land-management systems.

### Current Milestone

No change until the Manager opens the forestry programme as the next milestone (after Scenario One P-series acceptance).

## 3. Product decisions needed (not implied by the accepted direction)

1. **`forestModel` selector** vs extending `growthModel` semantics (save schema owner).
2. **Shade casting re-anchoring**: keep Sitka/oak/beech at today's implicit opacity (bit-identical) or re-calibrate under the new model.
3. **Site map for Scenario One**: neutral (recommended) or authored variation (changes Scenario One).
4. **Larch and ash**: exclude or allow as non-plantable present trees (plant health).
5. **Research commissioning** for R1, R3, R4, R5 (Wave B blockers).
6. **Fencing gameplay** (D-044) for Scenario Three.

## 4. First packets (if accepted)

1. **R2/R6/R7 ordinal research note** (no code): shade tolerance, shade casting and site classes for Sitka, oak, beech, birch, rowan and holly.
2. **W0-a G1 species tags / literal-id removal** (presentation; anchors unchanged).
3. **W0-b G2 + G3 + selector** (ecology owner; independent review; new-model anchors).
4. **W0-c G4 site map** (neutral Scenario One map; identity test).
5. **W1 species packets**: birch, then rowan, then holly.

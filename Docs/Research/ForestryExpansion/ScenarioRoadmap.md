# Silvicultural range and scenario roadmap (Parts 7 and 11)

**Status:** proposal. Principle: **scenarios reuse one core**. Each new scenario adds species, structure, constraints or problems, not rewritten systems. CCF is presented as a **process**, never one target diagram.

## 1. Starting conditions vs current support (Part 7)

| Starting condition | Supported now [REPO] | Gaps |
|---|---|---|
| Even-aged conifer plantation | **Yes** (Scenario One) | — |
| Mixed plantation (e.g. Sitka with Douglas fir/Norway spruce, or conifer–broadleaf rows) | Partly: the spawner can place any species | Shade casting, species density, site, markets for other conifers (Wave B) |
| Semi-natural woodland (oak–birch–holly–hazel) | Weak | Wave A species, site map, hazel resprouting (C), habitat retention |
| Young regeneration stand (respacing decision) | Partly (bands promote to trees) | Authored initial bands/juveniles; a respacing operation (new) |
| Irregular mature stand | Partly (any ages can be authored) | Authoring tool for size distributions; layer diagnostics |
| Neglected stand (dense, unthinned, unstable) | **Yes** in principle (old unthinned Sitka) | Storms on; senescence |
| Recently disturbed stand (windthrow) | Partly (storm state exists) | Authoring a disturbed start; salvage decision at Year 0 |

**Authoring need (cross-cutting):** a scenario-start description (stand table by species/size/position rules + site map + bands) instead of the hard-coded `ForestStartingStand`. This is the single enabling piece for every new scenario. **Generalise when Scenario Two needs it, not before.**

## 2. Scenario progression (Part 11)

| Scenario | Starting condition | New forestry depth it teaches | New systems it requires | Reuses |
|---|---|---|---|---|
| **One** (exists) | Even-aged Sitka, 0.16 ha, Class III | Positive selection, thinning, regeneration, planting, browse, vegetation, storms | — | Everything |
| **Two — "Mixed transformation"** | Older Sitka with a scattered Douglas fir/Norway spruce component and birch on wet patches; varied site map | Species–site matching; underplanting tolerant conifers vs relying on birch; mixtures | Foundation G1–G6, Wave A, at least 2 Wave B conifers, site map, scenario authoring | Scenario One UI, economy, history |
| **Three — "Native woodland restoration"** | Semi-natural oak–birch with holly/hazel, heavy deer pressure, low timber value | Browse as the bottleneck, holly/hazel understorey dynamics, deadwood/habitat retention, fencing (if accepted) | Hazel resprouting (C), fencing gameplay (D-044 decision), standing deadwood | Wave A species |
| **Four — "Irregular productive forest"** | Already irregular mixed conifer stand | Single-tree selection in an established CCF stand, quality-driven Crop Trees, markets | Quality wave, T3–T5 markets | All |
| **Five (optional) — "After the storm"** | Recently windthrown stand | Salvage vs retention, regeneration in gaps, rebuilding stability | Storms on; disturbed-start authoring | Storm Model 1 |

**Custom forestry sandbox** becomes worthwhile when (a) the scenario-start authoring format exists (Scenario Two), (b) at least Wave A + B species are calibrated, and (c) the site map exists. The sandbox is then mostly UI over the same format: choose site map, starting stand template, species pool, browse pressure, storms on/off. Before that, a sandbox would expose uncalibrated species and invite meaningless combinations.

The marteloscope/training mode (Scenario One readiness Q2) uses the same authoring format and can arrive alongside Scenario Two.

# What "a fully fledged CCF simulator" means for CCF (Parts 2 and 10)

**Status:** proposed definition for Manager/user review. It is **not** "many tree meshes", and **not** "every forestry process ever modelled". It is the minimum capability at which a player can practise continuous-cover forestry as foresters understand it: repeated, selective, reasoned interventions in mixed and irregular stands, with credible species differences and visible consequences. That follows the Game Brief: one excellent reference before broad generalisation; consequences visible in 3D; no single prescription.

Evidence labels: **[REPO]** inspected code at `341ccbf`; **[R]** project research (named); **[I]** general forestry knowledge, unverified for this project; **[C]** gameplay calibration.

## 1. Capability definition

### ESSENTIAL FOR A CCF SIMULATOR

| # | Capability | Meaning in this game | State at `341ccbf` [REPO] |
|---|---|---|---|
| E1 | **Multiple species with distinct strategies** | At least one representative of each strategy that changes CCF decisions: shade-tolerant canopy; light-demanding pioneer; long-lived slow canopy; shade-tolerant understorey; productive conifer(s) beyond Sitka | 3 species (Sitka, oak, beech); oak and beech enter mainly by planting |
| E2 | **Mixed-species stands where mixture matters** | Species-specific shade casting and tolerance, height stratification, cross-species competition, species-aware density | Competition cross-species (yes). Shade casting species-blind. Density = Sitka law for all |
| E3 | **Irregular structure** | Multiple cohorts and layers coexist and can be diagnosed (diameter/height distributions, layers) | Partly emergent; no layer diagnostics |
| E4 | **Natural regeneration driven by seed source × light × site × competition × browse** | Already the core model, plus site | Yes except site |
| E5 | **Enrichment / underplanting** | Targeted planting of species the stand lacks, with site and light consequences | Yes (exact planting), but no site |
| E6 | **Positive selection and Crop Tree management** | Choose trees to favour, see their competitors, release them repeatedly; per species | Yes (P2 verified, pending manual review) |
| E7 | **Species/site matching** | A small number of site factors that make a species thrive, struggle or fail | **Missing** |
| E8 | **Mortality and disturbance** | Density, senescence, storms, browsing, vegetation competition | Density (Sitka law), storms (dormant), browse, vegetation. No senescence |
| E9 | **Deadwood and habitat retention** | Fallen deadwood (exists); standing deadwood / retention trees | Fallen only |
| E10 | **Timber value by species and size** | Assortments and markets for each productive species; low-grade outlets | Sitka only |
| E11 | **Repeated interventions and management history** | Second and later cycles, history and diary | P3/P4/P6 designs; partly built |
| E12 | **Economic trade-offs** | Contractor minimum, small-job economics, standing value, species value | Sitka only |
| E13 | **Stand diagnostics** | Basal area, stems/ha, diameter distribution, species composition, layers, regeneration state, light | Partly (Work Plan residual P3, Annual Review) |
| E14 | **Scenario variation** | More than one starting condition reusing one core | One scenario |

### HIGH-VALUE LATER FORESTRY DEPTH

| Capability | Why later |
|---|---|
| Tree quality history (branchiness, form, pruning, damage → grade) | Needs a quality state and market evidence (`TimberQualityEconomyRoadmap.md`) |
| Resprouting / coppice (hazel, birch stools) | A new regeneration pathway |
| Group selection / gap planning tools beyond marking | Marking already allows gaps; dedicated tools are UI |
| Fencing / deer exclusion as gameplay | D-044 deferral; geometry exists |
| Access / extraction racks | Stage 2 infrastructure crossover |
| Ring-barking / standing deadwood creation | Needs a standing-dead state (D-020: the asset exists, the state does not) |
| Height-asymmetric competition (dominance) | Valuable refinement once shade casting exists |
| Species-specific storm rooting | Needs site wetness first |

### SPECIALIST / DEFERRED

Pests and diseases as dynamic systems (ash dieback, *Phytophthora ramorum* on larch: handled as **availability constraints**, not simulations) [I]; nutrient cycling; hydrology beyond a wetness class; genetics/provenance; climate change trajectories; carbon accounting; fine-scale soil; microclimate; mycorrhiza.

**Rule:** a concept becomes simulated only when it changes a decision the player can make, and only when evidence supports its direction.

## 2. Management tools (Part 10)

| Tool | Status | Classification | Note |
|---|---|---|---|
| Marking (Fell / Crop Tree) | Exists | **Already implemented** | Exclusive marks (D-013) |
| Crop Tree selection + competitor reasoning | P2 | **Implemented on branch** | Pending manual review |
| Thinning (selective) | Exists | Implemented | Contractor job, minimum charge |
| Group selection / gap creation | By marking clusters | **Needs expansion** (diagnostics only: opening size, P3 pattern) | No remote gap tool (D-010) |
| Planting / enrichment | Exists (beech, oak) | **Needs expansion** (more species, site feedback) | |
| Vegetation control | Exists (Model 2) | Implemented | |
| Protection (shelters) | Exists | Implemented | Needs species shelter sizes later |
| Pruning | Exists (Sitka-style lifts) | **Needs expansion** (species/form-aware; quality link) | |
| Deadwood retention (felled) | Exists | Implemented | |
| Standing deadwood / ring-barking | Asset only | **New mechanic** | Needs standing-dead state |
| Fencing / deer exclusion | Geometry only | **New mechanic** (D-044) | |
| Coppicing / respacing | None | **Later specialist** | |
| Access / extraction constraints | None | **Later specialist** (Stage 2 crossover) | |
| Salvage | Exists (storms) | Implemented, dormant | |
| Stand inventory / plots | None | **New mechanic** (Forest Diary Phase 2) | |

## 3. What "fully fledged" does *not* require

- A fixed species count (see the gate in `ImplementationWaves.md`).
- A universal biodiversity or sustainability score.
- A prescribed target structure (inverse-J, BDq).
- Real-time markets or national price simulation.
- Physiological growth models (light-use efficiency, water balance).

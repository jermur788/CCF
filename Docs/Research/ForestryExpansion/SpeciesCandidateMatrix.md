# Species candidate matrix (Part 3)

**Status:** proposal. **Inclusion is not an ecological endorsement for every site**: each species carries site tolerance (`SpeciesTraitArchitecture.md` §3), and unsuitable planting performs poorly.

Evidence labels:
- **[R:WE]** *Irish Atlantic Woodland Ecology for a CCF Game Simulation* (project PDF);
- **[R:SD]** *Irish Sitka CCF Deep Research Report*;
- **[R:PS]** *European Pro Silva CCF Practice Synthesis*;
- **[R:EC]** *Irish Forestry Economics, Labour and Contractor Operations* (nursery list);
- **[REPO]** current assets;
- **[I]** general forestry knowledge, **not yet verified for this project**. Every [I] trait is a research requirement before a parameter is set (`ResearchBacklog.md`).

**Key evidence fact:** the project research covers native broadleaves well ([R:WE] gives explicit simulation roles for oak, downy birch, rowan, hazel, holly and alder; [R:SD] gives the Irish Native Forest Framework soil templates). It contains **essentially nothing** on productive conifers other than Sitka: zero mentions of Douglas fir, Norway spruce, larch, western hemlock, western red cedar or silver fir across the ten research files (searched). That decides the wave order.

## 1. Matrix

Scores: H / M / L. "Distinct gameplay contribution" means a decision the player makes differently because this species exists.

| Species | Status (IE) | CCF use | Shade tolerance | Regeneration strategy | Browse sensitivity | Site notes | Timber role | Distinct gameplay contribution | Data available | Art burden | Impl. difficulty |
|---|---|---|---|---|---|---|---|---|---|---|---|
| **Sitka spruce** (exists) | Introduced, dominant plantation species | H | Intermediate (~20 % relative light threshold [R:SD]) | Wind seed, abundant | Low [REPO] | Wide; gley/peat tolerant [I] | Main softwood (exists) | Baseline | **H** [R:SD], Growth Model 1 | done | done |
| **Sessile oak** (exists) | Native | H | Light-demanding as a sapling; seedlings fail under canopy [R:WE] | Heavy seed, local dispersal (jays [I]) | H [R:WE] | Podzols, brown earths [R:SD] | Quality hardwood (no market yet) | Long-lived canopy; difficult regeneration | M [R:WE] | partial | Low (exists) |
| **Beech** (exists) | Introduced, naturalised [I] | M | Very tolerant [I] | Heavy seed, mast | M–H [REPO] | Free-draining [I] | Hardwood (no market) | Shade-casting late-successional; can suppress others | M (BeechSpeciesParameters doc) | partial | Low |
| **Downy birch** | Native | H (pioneer nurse) | Light-demanding pioneer [R:WE] | Wind, far, prolific [R:WE] | M [I] | Wet, acid, peaty soils [R:WE][R:SD] | Low-grade / firewood [I] | **Large openings follow a different trajectory**: a fast pioneer flush [R:WE] | **M–H** [R:WE][R:SD: High priority] | M (broadleaf; reuse oak/beech pipeline) | **Low** |
| **Rowan** | Native | M (minor mixture) | Comparatively shade-capable seedling [R:WE] | Bird-dispersed [R:WE] | **H**; deer can stop escape [R:WE] | Wide, acid tolerant [R:SD] | None (habitat) | **Advance regeneration held back by deer**: browse as a recruitment bottleneck [R:WE] | M [R:WE][R:SD: High] | L–M (small tree) | Low–M (bird dispersal) |
| **Holly** | Native | M (understorey) | **Shade-tolerant evergreen** [R:WE] | Bird-dispersed [R:WE] | M (deer exclusion → dense holly [R:WE]) | Podzols [R:SD] | None | **Persistent understorey that captures light beneath oak** (Killarney example [R:WE]) | M [R:WE] | M (evergreen shrub-tree) | M (needs height strata/shade casting) |
| **Hazel** | Native | M | Moderately tolerant shrub layer [R:WE] | Animal-dispersed nuts, **resprouting** [R:WE] | M [I] | Richer mineral soils [R:SD: Later] | Coppice products [I] | Clonal shrub layer, neither good nor bad [R:WE]; links to Stage 2 nut trees | M [R:WE] | M | **M–H** (resprouting = new pathway) |
| **Alder** | Native | M (wet sites) | Shade-intolerant pioneer [R:WE] | Wind/water, local [R:WE] | M [I] | **Wet-site specialist** [R:WE][R:SD] | Low–mid [I] | **Makes hydrology a first-class constraint** [R:WE] | M [R:WE] | M | **Needs site wetness** |
| **Willow (grey/eared)** | Native | L | Intolerant [I] | Wind, wet ground [R:SD] | H [I] | Wet openings [R:SD] | None | Overlaps alder/birch | L | M | Low (but redundant) |
| **Douglas fir** | Introduced | **H** in Irish/British CCF [I] | Intermediate–moderately tolerant when young [I] | Wind seed [I] | M–H (fraying) [I] | Free-draining, sheltered; not wet/exposed [I] | **High-value structural** [I] | A productive conifer that tolerates partial shade: **underplanting a Sitka stand** | **Not in project research** | M (conifer pipeline) | M |
| **Norway spruce** | Introduced | M [I] | Moderately tolerant [I] | Wind [I] | M [I] | Wetter, frost-tolerant; less exposure-tolerant than Sitka [I] | Softwood similar to Sitka [I] | Lower contrast with Sitka (but frost/wet sites, Christmas-tree thinnings [I]) | Not in research | M | Low–M |
| **Western hemlock** | Introduced | **H** for shade-tolerant underplanting [I] | **Very tolerant** [I] | Prolific light seed, regenerates under canopy [I] | M [I] | Acid, moist [I] | Mid-value softwood [I] | **Shade-tolerant conifer for transformation**; can dominate regeneration (a management problem) [I] | Not in research | M | M |
| **Western red cedar** | Introduced | M [I] | Very tolerant [I] | Seed + layering [I] | **H** (deer) [I] | Moist, fertile [I] | Durable cladding [I] | Shade-tolerant, browse-limited | Not in research | M | M |
| **Silver / grand fir** | Introduced | M [I] | Very tolerant (silver) [I] | Seed [I] | **H** [I] | Moist, sheltered [I] | Softwood [I] | Classic continental CCF species ([R:PS] continental context) | Not in research | M | M |
| **Scots pine** | Native status complex (reintroduced) [I] | M [I] | **Light-demanding** [I] | Wind [I] | M [I] | Dry, poor, free-draining [I] | Mid-value softwood [I] | A light-demanding conifer: needs large gaps; contrasts with hemlock | 1 mention only | M | Low–M |
| **Larch (Japanese/hybrid/European)** | Introduced | (was M) | Very light-demanding [I] | Wind [I] | M [I] | Wide [I] | Durable [I] | **Deferred**: *Phytophthora ramorum* restrictions in Ireland/Britain [I]. Treat as unavailable for planting until verified | Not in research | M | — |
| **Sycamore** | Introduced, naturalised; can be invasive [I] | M [I] | Moderately tolerant [I] | Wind (samaras), prolific [I] | M [I] | Wide [I] | **Quality hardwood** [I] | Productive broadleaf that regenerates freely: a management problem and an opportunity | Not in research | M | M (needs quality/market) |
| **Wild cherry** | Native [I] | L–M [I] | Light-demanding [I] | Bird, root suckers [I] | M [I] | Fertile [I] | High-value veneer if well formed [I] | Quality-tree focus (form, pruning) | Not in research | M | M |
| **Sweet chestnut** | Introduced [I] | L [I] | Moderate [I] | Heavy seed, coppice [I] | M [I] | Acid, free-draining [I] | Durable, coppice [I] | Coppice overlap with hazel; also a Stage 2 nut tree [I] | Not in research | M | M |
| **Ash** | Native | — | — | — | — | — | — | **Excluded**: ash dieback; planting not a credible option [I]. Possibly present as declining mature trees later (D-020 needs state) | — | — | — |
| **Yew** | Native | L | Very tolerant [I] | Bird | M | Limestone [I] | — | Niche; low value for transformation of Sitka | 3 mentions | M | — |

## 2. Recommended waves

### Wave A — "native strategies" (proves the architecture with evidence we have)

**Downy birch, rowan, holly**, and making **oak and beech full participants** (natural regeneration from planted/retained seed trees, site tolerance, shade casting).

- Each Wave A species contributes a *different strategy* that existing mechanisms can express once shade casting and site wetness exist:
  - birch is the pioneer (light × wet site × far wind seed);
  - rowan is browse-limited advance regeneration (bird dispersal, palatability);
  - holly is the shade-tolerant evergreen understorey (shade casting + tolerance; storey structure).
- The evidence is in hand: [R:WE] roles, [R:SD] priorities (birch and rowan High, holly Medium), [R:SD] soil templates.
- The art burden is moderate (broadleaf pipeline exists for oak and beech).
- Timber value is negligible, so Wave A tests ecology without needing new markets.

### Wave B — "productive transformation conifers" (BLOCKED on research)

**Douglas fir, western hemlock, western red cedar, Norway spruce, Scots pine.**

- These are what make a Sitka transformation a *forestry* decision: underplanting tolerant conifers vs light-demanders, new timber value, new browse risks [I].
- **Blocked:** none has project evidence. Each needs at least a growth/site curve, shade tolerance anchors, seed and dispersal behaviour, a density relationship (or a species-group rule), browse palatability, assortment specifications and a price basis (`ResearchBacklog.md` R1–R4).

### Wave C — "site range and productive broadleaves"

**Alder** (needs the wetness site factor at full strength), **hazel** (resprouting pathway; bridges to Stage 2 nut trees), **sycamore** and **wild cherry** (quality hardwood markets; need the quality system), **silver fir** (optional).

**Deferred or excluded:** larch (plant-health restrictions), ash (dieback), willow (redundant with birch/alder at this depth), sweet chestnut (revisit for Stage 2 nut trees), yew.

**Total additional:** 3 (A) + 5 (B) + 4–5 (C) = **12–13 species**, within the 8–15 target. **None is added for visual variety alone.**

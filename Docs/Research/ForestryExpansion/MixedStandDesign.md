# Mixed-stand CCF design (Part 6)

**Status:** proposal. The goal is that **mixtures change outcomes and decisions**, not that they look varied. Two rules hold throughout: **mixed is not automatically better**, and **there is no biodiversity score**.

## 1. Mechanics: needed vs already present

| Mechanic | Why it matters in mixtures | Present at `341ccbf` [REPO] | Needed | Wave |
|---|---|---|---|---|
| **Cross-species competition** | Neighbours of any species constrain a Crop Tree | Yes: Hegyi over all living trees, DBH-based | Nothing for v1. Later, optional height asymmetry (a taller neighbour counts more) | — / later |
| **Shade casting** | A dense-crowned beech or hemlock understorey suppresses regeneration; birch lets light through | **No**: species-blind | Per-species crown transmission in `RecomputeCanopy` (G2) | Foundation |
| **Shade tolerance** | Who can regenerate where | Yes: juvenile light anchors per species | Calibrate anchors per species (ordinal classes) | Wave A |
| **Height stratification** | Overstorey / midstorey / understorey; holly under oak | Emergent from heights; height laws inconsistent between Sitka and others | Species height curves (G7); a layer diagnostic (top / mid / lower by height relative to local top) | Foundation (diagnostic), Wave B (curves) |
| **Regeneration beneath mixtures** | Composition of the next generation depends on light under each canopy type and seed sources | Seed source × light × browse × vegetation: yes | Shade casting + site + dispersal mode | Foundation + A |
| **Seed-source distribution** | Retaining scattered seed trees of minority species matters | Yes (per-species kernels, maturity) | Bird long-tail kernel (G5); a "seed trees retained by species" line (P3 extension) | A |
| **Species replacement over time** | Tolerant species ingrowth under light-demanders (or not) | Emergent once shade and site exist | Senescence for short-lived pioneers (G6) | A |
| **Crop Tree selection by species** | Favouring a valuable or minority species is a real CCF decision | Crop Tree is species-agnostic; the P2 competitor list shows tree IDs, not species | P2 list shows species name per neighbour; a Crop Tree species line in the P3 summary | A (UI) |
| **Different target structures** | Group mixtures vs intimate mixtures | Any structure possible | Diagnostics only (no target) | — |
| **Species-specific timber outcomes** | Value differs by species, size and quality | Sitka only | Assortment registry per species (G8) | B |
| **Risk diversification** | Storm, browse, disease and market risk spread across species | Storm: species hooks = 1; browse per species | Storm rooting by wetness × species (later); species-specific market risk is **not** simulated in v1 | Later |
| **Storm response** | Stability differs (rooting, crown) | Generic vulnerability | Keep generic until evidence (`ResearchBacklog.md` R9) | Later |
| **Browsing effects** | Selective browsing shifts composition (rowan, oak, cedar lost; birch, Sitka less) | Per-species palatability | Calibrate palatability ordering for new species | A/B |
| **Species-aware density** | Mixed stands self-thin differently | Sitka law for all | Additive relative density across species (G3) | Foundation |

## 2. Additive relative density (G3) — the minimum honest mixed rule

`RD_stand = Σ_species N_i·(Dq_i / 25)^{b_i} / SDImax_i`.

- Species with no evidence use a **species-group default** (light-demanding conifer / tolerant conifer / broadleaf), labelled TR/CAL.
- Mortality vulnerability stays suppression-weighted, as now. A pure-Sitka stand reduces exactly to today's rule (bit-identical anchors).

This is a common additive density approach for mixtures [I; to be verified against the literature, `ResearchBacklog.md` R5]. Use it only as a self-thinning boundary, as Growth Model 1 does.

## 3. Shade casting (G2) — minimal form

The influence of a tree on a cell becomes `lateral × vertical × (1 − transmission_species)`, with a **phenology flag** for deciduous species (summer value only; no seasons in Stage 1).

- Transmission classes: dense (beech, hemlock, holly, cedar), medium (Sitka, Douglas fir, oak, Norway spruce), light (birch, larch, Scots pine, rowan) [I; ordering to verify, R6].
- Calibrate so that Sitka's current light is unchanged when its class equals today's implicit full opacity, or re-anchor under a new model version.

## 4. Diagnostics (no scores)

| Diagnostic | Where | Form |
|---|---|---|
| Species composition | Map layer "Species", Annual Review, Forest Diary | Basal area share by species, stems/ha by species; *facts* |
| Layers | Inspection ("in the lower layer under P0707"), map layer | Counts by layer |
| Regeneration by species and light | Ground report, map | Existing bands per species |
| Seed trees retained by species | Work Plan (P3 extension row) | Count before → after |
| Diameter distribution | Diary / stand panel | Histogram (no target overlay) |

## 5. Explicit non-goals

- No "mixture bonus" to growth or resilience. Any benefit must emerge from shade, density, site, browse and storm mechanics.
- No target mixture percentages in objectives (consistent with the Scenario One completion design).
- No biodiversity index.

## 6. Tests the mixed-stand wave must pass (offline / Unity, later)

1. A pure-Sitka world is bit-identical to growth model 1 under the new density and light code with Sitka parameters.
2. A beech understorey reduces regeneration light more than a birch understorey of the same basal area (shade casting).
3. Under a closed canopy, tolerant species (holly/hemlock) persist while birch fails; in a large gap, birch dominates early (strategy contrast).
4. Rowan is lost to browsing at pressure X but escapes with shelters or lower pressure (browse bottleneck).
5. A mixed stand self-thins at additive RD ≈ boundary; no species is killed by another species' density law alone.
6. Determinism and save/load across the new model version.

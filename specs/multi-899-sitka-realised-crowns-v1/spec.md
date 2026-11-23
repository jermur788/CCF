# MULTI #899 — Realised Sitka crown size, expansion and canopy cover

**Status: APPROVED — owner decision D-094 (2026-11-23).** SPEC REQUIRED: crown initialisation and annual response cross model, reproducibility and frozen-fixture boundaries. This document authorises implementation of `CrownModel 1` only; it does not authorise any Unity, save-schema, frozen-fixture or map-artwork change.

## Approved model — CrownModel 1 / area-scaled equilibrium (C2)

Version tag `crownModelVersion = 1`, rule id `area-scaled-equilibrium-c2-v1`.

```text
r_pot(DBH_cm) = max(0.1, 0.9415 + 0.07635 * DBH_cm)            metres  (open-grown potential)
r_target(CI)  = max(0.1, r_pot / sqrt(1 + CI / Ci50))          metres  (realised target)
r_0           = r_target(CI_0)                                          (year-0 equilibrium)
r_new         = max(0.1, r + (r_target(CI) - r) * k)                    (annual relaxation)

Sitka spruce: k = 0.15 per year, Ci50 = 5
```

- `CI` is the Hegyi index with an 8 m neighbour radius, 0.5 m distance floor, self-excluding, evaluated over **all living trees in the connected multi-rack woodland** (see boundary policy below) in deterministic `TreeId` order.
- `CI_0` is the same index evaluated once on the genesis stand. Year 0 therefore starts at the **realised equilibrium**, not at open-grown `r_pot`; this removes the multi-year artificial crown shrinkage the player currently sees.
- Annual DBH growth, height growth, mortality, work-plan semantics and all other rules are unchanged. Only crown initialisation and crown response change.
- The 15 % annual relaxation caps any single-year radius change at 15 % of the remaining gap, so no one Fell can produce a multi-metre jump.

### Model 0 is frozen

`crownModelVersion = 0` (`LegacyFrozen`) keeps today's exact path: year-0 radius = `r_pot(CI = 0)`, annual target = `r_pot / (1 + CI / Ci50)`, relaxation `k = 0.15`. Model 0 must reproduce the current outputs and hashes byte-identically, and stays available for baseline regression. Do not silently migrate existing fixtures, scenarios or archived runs.

### Connected multi-rack woodland — boundary policy

- The **outer perimeter of the combined woodland borders open grassland**. There are no synthetic "ghost" trees outside the site and no neighbours invented beyond the outer boundary.
- **Neighbouring racks interact ecologically as one forest**: Hegyi competition within 8 m, crown overlap and shade reach cross the shared rack border. The MA-1 areas A and B already share one world frame and a real boundary segment, so this is a direct nearest-neighbour calculation, not a synthetic bridge.
- **Work Plans, approvals and derived summaries remain scoped per rack/area.** The ecological neighbourhood is global to the woodland; the management unit is not.
- Area seeds and annual stepping remain per area; a shared neighbour calculation must not introduce order-dependent or cross-area mutation.

### Honesty label

`CrownModel 1` is a **Simulation Abstraction / Gameplay Calibration**, not a measured Irish growth model. The independent comparison of 2026-11-23 found it predicts Teagasc Frenchpark (Co. Roscommon) measured maximum crown radii within **+2–7 %**, probably overstates mean projected radius by about **5–15 %**, and reproduces the thinning response to about 2 percentage points. That evidence is one site, one age class and 12 trees per treatment, measuring maximum width. The synthetic MA-1 stands are an experimental demonstration, not an Irish empirical forecast.

## What was decided and why (history)

Three approaches were examined before selection:

- **A. Scalar realised-radius formula** using size and local competition. Selected as C2: it reuses species `Ci50` and `k`, adds no unfitted parameters, and matched the independent Frenchpark check above.
- **B. Eight-sector directional crown envelope.** Rejected for this pass: six parameters (`p`, `L`, `β`, `γ`, `vMax`, sector count) had to be fitted to the same single site, so it added complexity without independent support.
- **C. Separate adult initialisation from annual response.** Its legitimate concern — years of grown-in crown space being relaxed away — is addressed by the year-0 equilibrium `r_0 = r_target(CI_0)` plus the bounded annual relaxation, without a second hidden initialisation model.

The independent report also confirmed that a high **sum** of crown disk areas is not canopy cover. Cover must be the polygon-clipped **union** of crown projections, reported separately from competition and light (#900).

## Evidence hierarchy and validity

- **Direct British open-grown potential envelope:** P. Tabbush & E. White, *Canopy Closure in Sitka Spruce*, Forestry 61(1):23–27 (1988), https://doi.org/10.1093/forestry/61.1.23 — n=73 open-grown Sitka, crown width 188.30 + 15.27 × DBH (cm), hence radius `0.9415 + 0.07635 × DBH_cm` metres. This is **potential canopy guidance**, not an observed overstocked plantation crown width.
- **Direct relevant stem-mapped Sitka crown evidence:** O. Davies & A. Pommerening, *The contribution of structural indices to the modelling of Sitka spruce and birch crowns*, Forest Ecology and Management 256:68–77 (2008), https://doi.org/10.1016/j.foreco.2008.03.052 — 154 Sitka and 125 birch in six Welsh plots; local spatial indices improve realised-crown prediction. The pooled Hegyi coefficient is counterintuitive and must not be treated as a causal radial multiplier.
- **Direct Irish realisation check:** G. Fernandez, *The influence of thinning on tree stability in Sitka spruce*, UCD MSc (2017), Teagasc Frenchpark thinning trial — realised stand-grown maximum crown widths, unthinned vs thinned. One site, one age, 12 trees per treatment, mean-DBH trees only; maximum width is an upper bound on mean projected radius.
- **Direct UK thinning evidence:** *Respacing Sitka Spruce*, Forestry 61(1):1–22 (1988), https://doi.org/10.1093/forestry/61.1.1-a — respaced stands develop greater mean crown width and lower crown height; it does not define a one-year multiplier.
- Multiple crown disks may legitimately overlap. **Crown projection union, crown shape, light transmission and stem density are different metrics.** Do not equate 100 % crown union with 0 % transmitted light or zero regeneration.

## Requirements for implementation and qualification

- Preserve exact positions, IDs, species, live/stump states, Work Plan approval/execution, per-rack reporting, protected P0/Legacy40/save/reference baselines, deterministic replay and archived scenarios.
- **Version** the new crown semantics. Do not rewrite frozen `p1d-30m-sitka-adult-v1`, fudge hashes, change Unity or silently edit old scenarios. A new crown rule id / derived path is required; record whether old saves/scenarios remain readable and keep the existing default at model 0 until explicitly selected.
- Evaluate `CI` across the connected woodland; verify a stem within 8 m of the shared A/B border sees its cross-border neighbours and a stem beyond the outer perimeter sees none.
- Measure physical model output for both areas at years 0, 1, 3 (10 optional) with and without a real 10-tree Fell: per-tree radius distribution (not just mean), clipped crown-footprint **union** and uncovered area, connected-gap count and dimensions, crown expansion per year, basal area, density and exact tree IDs.
- Compare predicted radii and release to independent measurements. State error, domain and the known positive bias; do not claim field accuracy from a screenshot.
- Negative cases: radii finite and positive; within justified potential bounds; no multi-metre expansion in one year; no forced gap when none is physically warranted; provenance consistent; no synthetic neighbours outside the perimeter.
- Evidence-based qualification may be offline first, then one targeted real `/multi` run after approved implementation. No heavy Unity, Windows build, unrelated Q1, broad regression matrix or Unity source edit.

**Owner acceptance condition:** the model is demonstrably more credible against known crown evidence and transparently limited where data are absent — not merely a forest that looks less crowded.

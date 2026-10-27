# Sessile Oak v1 ecology

`sessile-oak` models *Quercus petraea* as a long-lived native canopy species whose
acorns can establish below canopy, but whose regeneration cannot recruit into the
individual-tree layer below 20% relative light.

The generic juvenile-light facility separates three responses for species that opt
in: establishment, annual density survival, and height growth. Sitka spruce and
European beech remain on the legacy combined response path.

Oak calibration (the juvenile, reproduction and crown values below are the initial
Oak v1 values and are still current; the adult growth values were superseded and
are given in *Adult growth (current)* below):

- natural seedling height: 0.18 m;
- favourable juvenile growth: 0.35 m/year;
- promotion: 3.5 m and at least 20% relative light;
- reproduction ramps from age 40 to full output at age 60;
- mast probabilities use 15% good and 35% poor years, with output multipliers
  1.0 / 0.3 / 0.075 for good / normal / poor;
- acorn dispersal is `exp(-distance / 4 m)`, cut off at 80 m;
- adult growth: see *Adult growth (current)* below (the initial 40 m height and
  0.4 cm/year DBH increment recorded here originally are superseded);
- crown radius uses the age-free fallback derived from the requested Oak crown
  relationship: `0.10 + 0.1045 * DBH_cm`, relaxed at 0.12/year.

## Adult growth (current)

The current accepted adult calibration (`SessileOak.asset`) is the Oak/Beech
ordinal production calibration v1:

| Parameter | Value |
|---|---:|
| potential DBH growth | 0.8 cm/year |
| potential height growth | 0.65 m/year |
| soft maximum DBH | 120 cm |
| maximum height | 42.5 m |
| `Ci50` (Hegyi index at which DBH growth is halved) | 5 |
| growing-space class | High (weight 2.0) |

- Evidence class: every value above is `GAMEPLAY CALIBRATION` (asset tag `[C]`), the
  search-grid centre of the pre-registered Oak/Beech calibration (candidate
  Oak-122). They are not fitted or measured Irish oak growth constants. The High
  class is a weak, provisional ordinal ranking (Oak above Beech) and is read only by
  growth model 2's density route.
- Adult growth uses the shared legacy asymptotic law (DBH: `Pd × site × (1 − DBH/Dmax)
  × 1/(1 + CI/Ci50)`; height: `Ph × site × (1 − H/Hmax)`, no competition term). It
  does not use Sitka's Class III age/site envelope.
- Scenario One's species×site response is neutral and dormant (exactly 1.0), so no
  drought or waterlogging effect is modelled for Oak. Canopy and light are shared
  with the other species; Oak differs only through its crown relationship and
  relaxation rate above and its `Ci50` in the crown target. Competition uses the
  shared 8 m Hegyi cutoff.
- From a promoted 3.5 m stem (age about 10) the law gives Oak about 21.45 m, 28.18 m
  and 32.76 m at ages 50, 75 and 100 (arithmetic derivation, not simulation
  output). The provisional calibration bands exist for ages 50 and 100 only; Oak sits
  inside both (20–25 m and 30–35 m), and the bands are gameplay-calibration targets,
  not measurements.

The supplied 2 m sapling, 8 m young-tree and 20 m mature Oak models are imported
as three-level Unity LOD prefabs. Regeneration uses the sapling visual, promoted
young trees use the young visual, and taller individuals transition to the mature
visual. The source models are normalized inside their prefab roots so the existing
authoritative ecological height continues to control displayed height.

The v1 dispersal model intentionally includes only the local exponential kernel.
Jay-mediated long-distance dispersal, rodent handling and acorn predation are
deferred; there is no hidden long-distance tail. Browsing, soil-moisture mapping,
genetics/provenance, hybridisation, disease, coppicing and deadwood are also out of
scope for this version.

The deterministic verification source is `Tools/Verification/CCFOakVerification.cs`.

# Sessile Oak v1 ecology

`sessile-oak` models *Quercus petraea* as a long-lived native canopy species whose
acorns can establish below canopy, but whose regeneration cannot recruit into the
individual-tree layer below 20% relative light.

The generic juvenile-light facility separates three responses for species that opt
in: establishment, annual density survival, and height growth. Sitka spruce and
European beech remain on the legacy combined response path.

Initial Oak calibration:

- natural seedling height: 0.18 m;
- favourable juvenile growth: 0.35 m/year;
- promotion: 3.5 m and at least 20% relative light;
- reproduction ramps from age 40 to full output at age 60;
- mast probabilities use 15% good and 35% poor years, with output multipliers
  1.0 / 0.3 / 0.075 for good / normal / poor;
- acorn dispersal is `exp(-distance / 4 m)`, cut off at 80 m;
- adult potential is 40 m height, 120 cm soft DBH scale, and 0.4 cm/year
  favourable DBH increment;
- crown radius uses the age-free fallback derived from the requested Oak crown
  relationship: `0.10 + 0.1045 * DBH_cm`, relaxed at 0.12/year.

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

# Section 5 — Unity integration of delivered art

Shared project: `/home/jer/CCF`. Source delivery:
`/media/jer/ZX20/Unity Assets/Forest_Floor_Detail`, revision
`2026-09-29_reviewed_v2`. All 40 manifest-listed payload hashes match; with
the checksum manifest itself this is the reported 41-file delivery.

## Integrated

- **Forest_Floor_Detail:** all five models, 15 FBXs. Oak, Beech and mixed
  litter are selected beneath observed living broadleaf crowns. The pure-Sitka
  Year-0 start has no broadleaf litter. Two small-wood layouts appear beside
  recorded decaying logs, rather than creating new deadwood records.
- **Woodland_Assets:** all seven models, 21 FBXs. Authored shade fern,
  bracken-type and bramble-type clumps replace their procedural silhouettes
  using the existing light/functional-cover mapping. Moss-on-wood patches
  accompany decaying logs. Fresh/decayed Sitka logs replace cylinder displays;
  recorded decay class selects the appearance and recorded volume controls
  shrinkage. Sitka stumps use the authored stump prefab.
- **Tree_Variation_Additions:** Beech_Sapling_01, three FBXs. It supplies the
  previously absent Beech regeneration visual and exact-position planted
  Beech juveniles. Planted Oak uses the existing Oak sapling asset. Individuals
  remain at their saved positions and cease juvenile display upon promotion.

39 FBXs produce **13 three-LOD prefabs** in
`Assets/ForestPrototype/Prefabs/Forestry/SectionFive/`. Explicit references live
in the Resources `SectionFiveVisualCatalog` asset. `ForestTest` and
`MixedSpeciesTest` are wired. `ForestSceneBuilder` preserves that wiring for
fresh scene creation when the catalog exists.

## Materials and boundaries

The litter material uses **Broadleaf_Litter_BaseColorAlpha_v2.png**, sRGB,
alpha clipping at 0.40, double-sided rendering and mip-coverage preservation.
The original unsanitised atlas is not imported. Twig/bark fragments are opaque
and double-sided. Materials are URP/Lit, zero-metallic and low-smoothness.
All new display prefabs have no colliders; hard LODs avoid requiring a dither
crossfade shader variant for the repaired atlas.

The remaining seven regular bases in Tree_Variation_Additions already have
their pruning-compatible equivalents integrated from Crop_Tree_Pruning.
The newly delivered legacy bent and Ancient/Forked packages are not swapped
into the crop-tree family: their original limitations and missing pruning
coverage remain explicit. No bent/cavity revisions or source modelling were
performed here. That next work belongs to the other asset-authoring agent.

Presentation writes no ecology, populations, inventory, objective targets or
save-schema fields. No audio was added. Spruce-specific brash/log models are
used only for recorded Sitka work; broadleaf logs retain their generic display.

## Checks

- `SectionFiveAssetSetup.BuildAndWire`: import-copy SHA checks, 13 prefabs,
  both scenes and scene validation pass.
- `SectionFiveAssetSetup.VerifyAssets`: repaired atlas, cutoff, alpha/mip
  settings, double-sided wood and collision-free LODs pass.
- `ScenarioHabitatPresentationVerification`: Years 0/20/50/100 pass; Year 100
  has 10 broadleaf litter patches and 70 small-wood details. Returning to Year 0
  clears future-only litter and preserves the player's full-world hash.
- `ScenarioOneInteractionVerification`: exact-position juvenile displays,
  red-mark auto-import, stump visibility and legacy reference replay pass.
- Scenario One deadwood persistence/migration and full Forestry integration
  pass. Canonical Sitka remains `7E39B70A14959FAD`; frozen reference Year-100
  archive remains `7AD177B3CC2F73C7` (v13 replay retains its documented P0601
  same-year pruning/felling exclusion).

The pending white-text patch had invalid GUIStyle disabled-state references
and a mismatched helper signature; those were corrected and Unity compilation
now passes. Text colour is reapplied as white in all eight actual GUIStyle
states. The material rebuild also clears the unusable RGB-only bark gloss map.

Still requiring player review: litter edges at distance, fern/bracken
silhouettes, ground fitting, log appearance, white HUD/Work Plan text and
frame time in a rendered Game view. Offline export and headless checks are not
a substitute for this review.

Reimport command: **Tools → Forest Prototype → Integrate Delivered Section 5
Assets** (`SectionFiveAssetSetup.BuildAndWire`). Existing differing imported
files cause a stop for inspection rather than being silently overwritten.

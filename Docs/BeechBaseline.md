# Stable baseline before Beech

The suppression-history slice is additive: age, physical dimensions and reproduction retain their existing behavior. Recorded history does not apply another growth penalty.

## Regression contract

- Unity 6000.6.0f1, Editor play-mode configuration. The Unity executable may
  report `buildType: Release`; that describes the Editor executable and does
  not make this a standalone Release-player baseline.
- Playable ForestTest: 336 P trees, 2,100 stems/ha, mean DBH 15.586 cm, basal area 41.099 m²/ha.
- Separate 68-tree lifecycle fixture: 80 annual steps, 30 reproductive recruits.
- Existing ecological-state hash: `7E39B70A14959FAD`, identical on two consecutive runs. This hash intentionally excludes the new diagnostic history so it detects changes to the established biological trajectory. History has separate assertions.
- Release, history persistence, no second growth penalty, harvesting, save/load and version-5 migration are covered by the regression harness.
- Compare hashes only within the same build configuration.

## Future mixed-species fixture

`Assets/Scenes/MixedSpeciesTest.unity` is a separate test scene with its own
asset GUID. It preserves the normal 336-tree Sitka starting generator and
adds four explicit Beech individuals at Play start. Beech reproduction is
disabled in this slice. The canonical `ForestTest` and mature lifecycle
fixture remain controls; MixedSpeciesTest is not added to the player build
list.

The verification harness has a `BeginMixedScaffold` entry point to run the same initial-stand and lifecycle checks starting in this scene. See SuppressionHistory.md for runner installation and cleanup.

# Stable baseline before Beech

The suppression-history slice is additive: age, physical dimensions and reproduction retain their existing behavior. Recorded history does not apply another growth penalty.

## Regression contract

- Unity 6000.6.0f1, Editor configuration.
- Playable ForestTest: 336 P trees, 2,100 stems/ha, mean DBH 15.586 cm, basal area 41.099 m²/ha.
- Separate 68-tree lifecycle fixture: 80 annual steps, 30 reproductive recruits.
- Existing ecological-state hash: `7E39B70A14959FAD`, identical on two consecutive runs. This hash intentionally excludes the new diagnostic history so it detects changes to the established biological trajectory. History has separate assertions.
- Release, history persistence, no second growth penalty, harvesting, save/load and version-5 migration are covered by the regression harness.
- Compare hashes only within the same build configuration.

## Future mixed-species fixture

`Assets/Scenes/MixedSpeciesTest.unity` is a separate copy of the unchanged playable scene, with its own asset GUID. At this checkpoint it is deliberately Sitka-only: Beech spawning and ecology belong to the next slice. Make later mixed-stand changes here, preserving ForestTest and the mature lifecycle fixture as controls. It is not added to the player build list.

The verification harness has a `BeginMixedScaffold` entry point to run the same initial-stand and lifecycle checks starting in this scene. See SuppressionHistory.md for runner installation and cleanup.

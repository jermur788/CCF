# core-stand-live-2m.csv provenance

This is the 336-tree Legacy40 core captured from live Unity at the scene's serialized 2.0 m planting spacing. It replaces the
retired 1.9 m generator replica (`core-stand-replica.csv`, now in git history only); that replica's figures (7.6 m half-width,
76-tree strip, 14 initial removals, 24.362% basal share, 30.485% minimum share) are not authoritative.

- Source task: `SCENARIO-ONE-APPROACH-ANCHOR-DIAGNOSIS-V1`, 2026-10-22.
- Source checkpoint: `1a40fde` (scene `Assets/Scenes/ForestTest.unity`: `latticeSpacingMeters: 2`, `latticePerAxis: 21`,
  `targetTreeCount: 336`).
- Source evidence: `/media/jer/Files/CCF/Verification/ScenarioOneApproachAnchorDiagnosis-20261022T211047Z/gates/determinism-session1/evidence/legacy40-regen.csv`
  (sha256 `ff1025ae0c3ed84ac0dc42c418128a836ad82381e8becb4a1adc429b9e284e31`).
- Format: `id,x,z,dbh`, 336 data rows. `P0009` is present and `P0600` is absent, because this is the pre-approach Legacy40 core.
- Live 2.0 m results: rack half-width 8.004 m, teaching footprint 69 trees / 1.29490 m2 initial BA, representative initial release
  13 removals / 24.738% basal share / 30.060% minimum share.

`RackBayOfflineCheck` also parses `ForestTest.unity` and compares the scene's serialized spacing with the captured layout gap, so
a future spacing change fails the check instead of silently reusing this fixture.

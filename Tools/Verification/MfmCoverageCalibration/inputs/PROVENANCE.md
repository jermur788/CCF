# MFM-03 frozen inputs — provenance

| File | Derived from | Source SHA-256 |
|---|---|---|
| `fixture-stand.csv` | `trees` of the `final-west-preserved` entry in `/media/jer/Files/CCF/Verification/RackV2-02A-Headland-20261026T230400Z/generated-stands.json` (accepted RACK-V2-02A fresh-V2, target-preserving headland generation; identical to `MFM-03-CoverageCandidateCalibration-20261027/inputs/stand.json`). Columns copied verbatim (`id, x, z, dbh, height`). | generated-stands.json `e6a74e5fd1fbebb95aa0181bcf74e37321a71eb3340916d8cc7a4a31b830170f`; stand.json `9802efcc797ac8e8990fccffc7b1ae13132deb930ade14668498754a2eed7edc` |
| `fixture-infrastructure.json` | `paths`, `clearing`, `player`, `buildables`, `config` of `RackV2-02A-Headland-20261026T230400Z/scene-input.json` (extracted from the authored scene) and the south approach-extension points `MFM-03-CoverageCandidateCalibration-20261027/inputs/approach-route.json`. | scene-input.json `6b80d2ce1a74f49c838a9be8a6ba3e1385b264dc8a2904c99b7748bc29f0b7a2`; approach-route.json `edd2020c571bf2eb6d94f7603f32e4d2be4312e29144a728d2d690865609b75e` |
| `historical-lesson5-regeneration.json` | Cells with natural cohorts plus scenario counts from the legitimate pre-Rack-V2 Enlarged80 Lesson-5 save (schema 20, ecological year 2) `MFM-03-CoverageCandidateCalibration-20261027/inputs/historical-stage5-save.json`, itself the `ScenarioOneUxShotsFixtureReconciliation-20261023T181828Z/fixtures/stage5-save.json` fixture. | `ec54f44cbf453f0aeb944f589067febacfa89699e7cbeb09c6f1191a70ab6a71` |

## Identity with the live game

The stand is an offline reconstruction, not a Unity dump. Its identity with the live MFM-02 Unity fixture (tested HEAD `8f9be4f`) is
checked on every run: with the five qualified racks the teaching footprint holds exactly the live **75 stems, 1.4417531 m²**, footprint
SHA **`c2f2fd5418794915`**, **10** corridor-excluded band stems, **11** stems between 8.00385 m and 9 m and **8** living stems in the
9–10 m band — all equal to `MFM02-RackSampleGeometry-20261027T202415Z/unity-v2/evidence/mfm02-v2-measurements.txt`. The whole-stand count
(1,344) matches the Enlarged80 overview. Positions/DBH outside the footprint are not individually verified against a live dump.

Production files that could change the stand or the measurement (`ForestStartingStand.cs`, `ScenarioOneHeadlandGeometry.cs`,
`ScenarioOneRackSampleGeometry.cs`, `ScenarioOneRackBay.cs`, `CompetitionShare.cs`, `RackGeometry.cs`, `ForestEcologyController.cs`)
are unchanged between `8f9be4f` and MFM-03's inspected HEAD `72dbee8`.

# Multi-Rack Sitka P1 Lab (F0 foundation)

Standalone sibling of the P0 lab (`Tools/MultiRackSitkaLab`, which this tool never writes). Specification:
`specs/multirack-sitka-p1-adult-rules-comparator-v1/`. Task: CCF #639.

> **SYNTHETIC CLOSED ADULT SITKA. NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY.**

## What exists in this stage

| Part | State |
|---|---|
| Three domain tags (`DerivedP1ThirtyMetreExperimentV1`, `DerivedUnitySquareParityV1`, `OriginalPolygonExploratoryV1`) with exact, case-sensitive parsing | Implemented |
| Deterministic 30 × 30 m starting-forest manifest `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` and its strict parser | Implemented. **Pending independent review and Manager acceptance** |
| Compiled original-P0 trust table (3 fixtures) and read-only, no-follow, descriptor-checked inspection | Implemented. **Records pending review**: normal inspection refuses them |
| Annual gate | Refuses every domain. **No annual engine exists in this stage** |
| Reuse of Scenario One `SimulationRandom.cs` and `SitkaGrowthModel.cs` | Linked unchanged (read-only), function-level probes only |
| Host / UI, saves, `.p1-data/` root, work plans, felling, replay | **Not implemented** (UNAVAILABLE) |

## Build and test (Linux x64, .NET SDK 8.0.133)

```bash
cd Tools/MultiRackSitkaP1Lab
build/build-p0-core-ref.sh              # P0 Core DLL from the pinned tree, built outside the repo -> .p0ref/ (ignored)
dotnet build MultiRackSitkaP1Lab.sln
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
tests/p0-isolation.sh <evidence-dir>    # cold rebuild + tests under strace; L0–L4 proof that P0 is untouched
```

Set `TMPDIR` to a directory **outside** the repository. The build script refuses to run if it is inside.

## CLI

```bash
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll verify-derived p1d-30m-sitka-adult-v1 fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll inspect-original ../MultiRackSitkaLab/fixtures/synthetic oracle-quarter-ha [--review-evidence]
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1   # always REFUSED in F0
```

`--review-evidence` lets a reviewer run the full check sequence on a pending record. Its output is labelled
`UNTRUSTED-PENDING-REVIEW` and never creates experiment state.

## Isolation rules

- P0 is a **binary** reference (never a `ProjectReference`). The project build fails if the DLL's SHA-256 differs from the
  expected value in `src/Core/SitkaP1.Core.csproj`.
- Only P0's public `LabFile.Parse`, `LabFile.Serialize` (tests only) and `CanonicalJson.SnapshotHash` are used.
- The trust table never reads P0's `MANIFEST.sha256`.
- P1 never binds port 5187 and never opens P0's `.lab-data/`.
- Changing a trust or catalog record to `Accepted` is a reviewed source change that cites the independent review and the
  Manager acceptance.

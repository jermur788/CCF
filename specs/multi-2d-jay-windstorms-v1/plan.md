# CCF Multi 2D — Jay Acorn Dispersal, Nearby Oak Woodland and Windstorms V1 — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-jay-windstorms-V1`
- **Status:** `APPROVED / IMPLEMENTING` — Part 1 ACTIVE; Part 2 QUEUED behind Playable Milestone 1
- **Branch:** `feat/multi-2d-jay-windstorms-engine-v1`
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`, verified after `git fetch origin` on 2026-11-27)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`git rev-parse origin/main`, verified 2026-11-27; main untouched)
- **Pinned .NET SDK:** `8.0.133` (`/usr/bin/dotnet`, resolved through `Tools/MultiRackSitkaP1Lab/global.json`; `dotnet --version` prints `8.0.133` from that folder)

---

## 1. File plan (inside the four allowed prefixes only)

### 1.1 Spec quartet — `specs/multi-2d-jay-windstorms-v1/`
`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`.

### 1.2 Engine — `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/` (one responsibility per file)
| File | Responsibility |
|---|---|
| `NaturalProcessContracts.cs` | Immutable request/result records, enums, refusal codes |
| `NaturalProcessRandom.cs` | Fixed stream IDs; keyed roll (model 1 port) with per-call draw counter and test draw log |
| `WoodlandFootprint.cs` | Woodland polygon: validity, shoelace area/hectares, extent, boundary-inclusive containment |
| `JayAcornTransport.cs` | Shared transport: `r = 5 + 195u²`, uniform bearing, export when outside |
| `JayAcornDispersal.cs` | OFF/mast gates, caps, on-map diversion, orchestration |
| `NearbyOakWoodland.cs` | Authored off-map source line on one outer edge |
| `StormVulnerability.cs` | Frozen D-050 calibration, index V, BoundedRational p, qualitative bands |
| `StormAdapter.cs` | StormModel 0/1, one woodland-wide event, two-pass victims, outcomes, glyph convention |
| `SalvageAccounting.cs` | Leave/Sell/Keep accounting, Work Plan gate, duplicate-deadwood guards |
| `NaturalProcessText.cs` | Player/log text constants and dated teaching note |

### 1.3 Tests — `Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/`
`SitkaP1.NaturalProcesses.Tests.csproj` (Exe, single Compile glob, zero references), `TestRunner.cs`, `Program.cs`, `Fixtures.cs`, `JayDispersalTests.cs`, `StormAdapterTests.cs`, `SalvageAccountingTests.cs`, `HygieneAndParityTests.cs`.

### 1.4 Handoff — `Docs/Handoffs/MultiRack-Simulation-20261116/jay-windstorms/IMPLEMENTATION-HANDOFF-PART1.md`

## 2. Frozen-parity method

The frozen Unity sources `Assets/ForestPrototype/StormModel.cs` and `SimulationRandom.cs` were compiled **read-only, outside the repository** (`/tmp`, SDK 8.0.133, a three-function `Mathf` shim) to print roll, index, failure-chance and event-year vectors. Those vectors are hard-coded in `HygieneAndParityTests` and `Fixtures.FrozenEvents12345`. No repository file outside the fence was modified.

## 3. Verification commands (from `Tools/MultiRackSitkaP1Lab`)

```
dotnet restore tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj
dotnet build   tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore
dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
```

Gate: 0 warnings, 0 errors, exit code 0. `dotnet test`, SDK 10, `build/2d-gate.sh` and browser suites are not used.

## 4. Fence proof

`git diff --name-only c82e4e75..HEAD` must list only the four allowed prefixes.

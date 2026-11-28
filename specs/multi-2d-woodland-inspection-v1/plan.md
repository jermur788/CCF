# CCF Multi 2D — Woodland Inspection V1 — Implementation Plan (Part 1)

- **Spec ID:** `SPEC-multi-2d-woodland-inspection-V1`
- **Feature ID:** `multi-2d-woodland-inspection-v1`
- **Status:** `APPROVED / Part 2 IMPLEMENTING` — this file is the Part 1 plan, kept as history. Part 2 (Playable Milestone 3 Part 2, `/multi` wiring) is planned and recorded in `spec.md` §8 and `tasks.md` (Part 2); its files are `src/Ui/MultiWoodlandInspection.cs`, guard/clear hooks in `PlayerApi.cs`, `MultiApi.Woodland.cs`, `MultiApi.Saves.cs`, `MultiRoutes.cs`, embedded fixtures in `SitkaP1.Ui.csproj`, `multi/dialogs/woodland-inspection.js` plus small page hooks, and `tests/woodland-inspection-part2-api.sh`.
- **Branch:** `feat/multi-2d-woodland-inspection-engine-v1`
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`, verified at task start 2026-11-27)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`, verified at task start 2026-11-27)
- **Pinned .NET SDK:** `8.0.133` at `/usr/lib/dotnet` (pinned by `Tools/MultiRackSitkaP1Lab/global.json`). `~/.dotnet-8.0.133` does **not** exist on this host; the Importer V1 plan's WSL path does not apply here.
- **Builder provenance:** Requested Gemini 4 Neon (high) — unavailable. Selected/executed Claude Opus 5.5 (high), owner-selected packet fallback, 2026-11-27.

---

## 1. File Plan (inside the four allowed prefixes only)

### 1.1 Spec quartet — `specs/multi-2d-woodland-inspection-v1/`
`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`.

### 1.2 Pure-BCL engine — `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/Inspection/`
1. `WoodlandInspectionContracts.cs` — immutable view records (`WoodlandInspectionView`, `InspectedField<T>`, `InspectedTree`, `InspectedStandIdentity`, `InspectedBoundary`, `ObservedYear0Section`, `SpeciesBreakdown`, `SpeciesCategoryCount`, `SpeciesRow`, `InspectedMicrohabitat`, `InspectedAttribution`, `ProvenanceBoundarySection`, `BlockerChecklistItem`, `InspectionNote`).
2. `BuiltInWoodlandCatalogue.cs` — `BuiltInWoodlandDefinition`, `BuiltInWoodlandEntry`, and the fixed two-entry catalogue with `Load(Func<string, byte[]>)`.
3. `WoodlandInspectionProjector.cs` — `Project(WoodlandImportResult)`.
4. `BlockerExplanations.cs` — plain-English title/explanation per known importer issue code with message fallback.
5. `WoodlandInspectionSerializer.cs` — canonical JSON + SHA-256.

### 1.3 Standalone test executable — `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandInspection/`
1. `SitkaP1.WoodlandInspection.Tests.csproj` — `Exe`; compiles only `../../../src/Core/WoodlandImport/**/*.cs` plus its own files; no `ProjectReference`, `PackageReference`, Unity or P0 reference.
2. `TestRunner.cs` — copy of the Importer V1 runner pattern in namespace `SitkaP1.Core.WoodlandImport.Inspection.Tests`; non-zero exit on failure.
3. `TestFixtures.cs` — locates `tests/Core/WoodlandImport/Fixtures/Qualified` (read-only) by walking up from `AppContext.BaseDirectory`; `LoadFromDirectory` convenience; synthetic stand builders run through the real validator.
4. `WoodlandInspectionTests.cs` — tests T1–T10 (+T2b, T3b, T10c, hygiene).
5. `Program.cs` — entry point.

### 1.4 Handoff — `Docs/Handoffs/MultiRack-Simulation-20261116/woodland-inspection/`
`IMPLEMENTATION-HANDOFF-PART1.md`.

## 2. Design Notes

- **Reuse, not re-implementation.** Import, hash verification, species mapping and readiness all come from closed Importer V1. The engine only loads fixtures through it and projects its result.
- **No file I/O in production.** The catalogue takes a `Func<string, byte[]>` so `src/Core/WoodlandImport/**` stays free of file-system APIs (P1 `#644 B-01`). The directory loader is test-side (spec §4.1 deviation).
- **Missing stays null.** `InspectedField<T>` is instantiated with nullable types (`double?`, `int?`, `string?`, `LocalPoint2D?`, `TreeVitalStatus?`) so a Missing value is a real `null`, never `0`.
- **Defensive `CanAdvanceTime`.** Besides requiring Candidate readiness, the projector counts `Simulated` fields itself, so a hand-assembled Candidate result containing a simulated value still cannot advance.
- **Positions as stored.** The view shows the stand's already-normalised local metres plus the origin and derivation rule; raw source text stays in each field's evidence.

## 3. Verification Commands

From `Tools/MultiRackSitkaP1Lab`:

```bash
dotnet --version   # must print 8.0.133
dotnet restore tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj
dotnet build   tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-restore
dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore
```

Run the last command twice and compare the printed `[determinism]` hashes. Regression check (read-only use of the closed importer suite):

```bash
dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj
dotnet build   tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
```

Not used: `dotnet test`, SDK 10, `build/2d-gate.sh`, browser suites, `src/Core/SitkaP1.Core.csproj` (needs the git-ignored `.p0ref/` binary).

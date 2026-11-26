# CCF Multi 2D — Real-World Woodland Importer V1 — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-woodland-importer-V1`
- **Feature ID:** `multi-2d-woodland-importer-v1`
- **Status:** `APPROVED / IMPLEMENTING (owner-approved 2026-11-26)`
- **Branch:** `feat/multi-2d-woodland-importer-v1`
- **Base Commit:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Pinned .NET SDK:** `8.0.133` (`/home/jer/.dotnet-8.0.133/dotnet` in WSL Ubuntu; `Tools/MultiRackSitkaP1Lab/global.json`)

---

## 1. File Plan (Strictly Inside the Four Allowed Prefixes)

### 1.1 Spec Quartet (`specs/multi-2d-woodland-importer-v1/`)
1. `spec.md` — Normative Woodland Importer V1 specification (Option A decisions, contracts, caps, XXE hardening, geometry, species mapping, readiness precedence, gates).
2. `plan.md` — This implementation plan stamped with the verified canonical context commit SHA.
3. `tasks.md` — Task breakdown `WI-T00` through `WI-T11` with per-task acceptance criteria.
4. `checklists/requirements.md` — Requirements traceability matrix (`WI-REQ-01` .. `WI-REQ-14`).

### 1.2 Pure-BCL Core Importer (`Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/`)
1. `WoodlandImportContracts.cs` — Fixed Layer 1 immutable contracts (`WoodlandImportStand`, `ImportedTreeRecord`, `ImportField<T>`, `FieldEvidence`, `FieldProvenance`, `ImportSourceAttribution`, `WoodlandImportResult`, `WoodlandReadiness`, `ImportValidationIssue`, `IWoodlandImportReader`, `WoodlandImportOptions`, `WoodlandImportRequest`, plus supporting value types).
2. `WoodlandSpeciesMapper.cs` — Deterministic exact-match species classifier (`ModelledAtBase`, `PendingModelSupport`, `UnmodelledSpecies`, `UnidentifiedSpecies`).
3. `WoodlandBoundary.cs` — Surveyed polygon validation (refuses `<3` vertices, non-finite, zero-length edges, self-intersection, zero area), exact shoelace area (`m²` and `ha`), local tangent-plane geographic projection, and boundary-inclusive ray-casting point-in-polygon.
4. `LocalCoordinateNormalizer.cs` — Translation-only coordinate normalization to deterministic boundary (or tree) origin preserving pairwise distances exactly.
5. `WoodlandImportValidator.cs` — Enforces refusal/InventoryOnly/Candidate precedence, Year-0 `Simulated` refusal, conservative living-stem counting, cap enforcement, and out-of-bounds tree refusal without clamping.
6. `DarwinCoreArchiveReader.cs` — Bounded in-memory `ZipArchive` reader with per-entry & cumulative uncompressed byte caps, XXE/DTD-prohibited `XmlReader` `meta.xml` parser, invariant-culture delimited reader, and `coreid` extension joiner.
7. `IntegrateMarteloscopeReader.cs` — `IWoodlandImportReader` implementation mapping Integrate+ TreMs DwC-A core + `MeasurementOrFact` extension rows into canonically sorted `WoodlandImportStand` records with honest provenance.
8. `WoodlandImportSerializer.cs` — Deterministic canonical UTF-8 `System.Text.Json` writer and SHA-256 digest helper.

### 1.3 Standalone Offline Test Executable (`Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/`)
1. `SitkaP1.WoodlandImport.Tests.csproj` — `OutputType Exe`, `AssemblyName`/`RootNamespace` `SitkaP1.Core.WoodlandImport.Tests`, single `<Compile Include="../../../src/Core/WoodlandImport/**/*.cs" />`, zero `ProjectReference`, zero `PackageReference`, zero P0 reference.
2. `TestRunner.cs` — Minimal deterministic console test harness with named tests, suite tags (`synthetic` vs `qualified`), per-test PASS/FAIL reporting, totals, and non-zero exit code on any failure.
3. `SyntheticArchiveBuilder.cs` — In-memory deterministic DwC-A ZIP builder for synthetic test fixtures.
4. `WoodlandImportCoreTests.cs` — Comprehensive synthetic coverage: positive import, malformed archive, refusal cases, InventoryOnly cases, Year-0 Simulated refusal, all 6 species + unmodelled taxon + PLANTAE/unidentified, non-rectangular shoelace area vs hand-computed value, geographic tangent-plane projection, out-of-bounds refusal without clamping, translation-only distance preservation, all four caps (living stems incl. unknown status, total records, measurement rows, uncompressed bytes), XXE/DOCTYPE refusal with no external fetch, and two-run determinism (byte-identical JSON + identical SHA-256).
5. `FixtureQualificationTests.cs` — Offline tests reading committed real-data fixtures under `Fixtures/Qualified/` (Irish plot + compatible mixed European plot), verifying attribution, species classification, readiness, and deterministic hashes.
6. `Program.cs` — Entry point running both synthetic and qualified test suites.
7. `Fixtures/Qualified/` — Committed subset fixtures and attribution metadata derived from INRAE Integrate+ TreMs (DOI `10.15468/ocof3v`, CC BY 4.0); never the full archive.

### 1.4 Handoff & Qualification Docs (`Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/`)
1. `SOURCE-DATA-QUALIFICATION.md` — Archive retrieval outcome, exact network error or SHA-256, CC BY 4.0 licence & citation, observed field/extension/coordinate/status encodings, Irish plot qualification, and compatible mixed European plot qualification.
2. `IMPLEMENTATION-HANDOFF.md` — Summary of built components, exact verification commands and captured output, pass/fail split (synthetic vs qualified), determinism SHA-256 evidence, fence proof, residual gaps, and D-030 state words.

---

## 2. Verification Strategy

Run from `Tools/MultiRackSitkaP1Lab` inside WSL Ubuntu with `/home/jer/.dotnet-8.0.133` first on `PATH`:

```bash
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet --version
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet build   tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
```

`src/Core/SitkaP1.Core.csproj` is deliberately NOT built because `.p0ref/SitkaLab.Core.dll` is git-ignored and absent in this worktree. Standalone compilation under the shared `Directory.Build.props` (`TreatWarningsAsErrors=true`, `Nullable=enable`, `LangVersion=12`, `Deterministic=true`, `InvariantGlobalization=true`) proves warning-free pure-BCL compatibility, alongside a dedicated static hygiene test asserting zero `#644 B-01` banned file API substrings in `src/Core/WoodlandImport/**`.

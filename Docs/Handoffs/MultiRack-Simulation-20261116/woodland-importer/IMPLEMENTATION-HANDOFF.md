# CCF Multi 2D — Real-World Woodland Importer V1 — Implementation Handoff

- **Date (UTC):** `2026-11-26`
- **Lane:** Coding Lane 4 — Real-World Woodland Importer V1
- **Branch:** `feat/multi-2d-woodland-importer-v1`
- **Base Commit:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Spec Commit:** `cb3152f20a81d3fcfdce44d30656b26380b4c72a`
- **Implementation Commit:** `5d49a7b9017b1f9bb4d8a1cbc776ad704a00805f` (followed by this handoff commit on the same feature branch)
- **Pinned Toolchain:** WSL Ubuntu, `/home/jer/.dotnet-8.0.133/dotnet` (`8.0.133`)
- **D-030 State:** **Isolated feature branch verified; NOT merged to `integration/2d-sim`; NOT wired into `MultiAreaContinuation`, `/multi`, Save v2/v3, UI, or Unity; NOT accepted or integrated.**

---

## 1. What Was Built (Strictly Inside the Four Allowed Prefixes)

### 1.1 Spec Quartet (`specs/multi-2d-woodland-importer-v1/`)
- `spec.md` — Normative specification (`APPROVED / IMPLEMENTING (owner-approved 2026-11-26)`) encoding all four Option A decisions, file fence, Layer 1 contracts, caps, XXE hardening, geometry/shoelace/tangent-plane rules, species mapping, validation precedence, and acceptance gates.
- `plan.md` — File plan and verification strategy stamped with verified canonical context SHA `bdeb6a324acaf4129c10fb0846faac8b64974360`.
- `tasks.md` — Per-task acceptance table for `WI-T00` through `WI-T11`.
- `checklists/requirements.md` — Requirements traceability matrix (`WI-REQ-01` .. `WI-REQ-14`).

### 1.2 Pure-BCL Core Importer (`Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/`)
- `WoodlandImportContracts.cs` — Immutable v1 Layer 1 contracts: `WoodlandImportStand`, `ImportedTreeRecord`, `ImportField<T>`, `FieldEvidence`, `FieldProvenance`, `ImportSourceAttribution`, `WoodlandImportResult`, `WoodlandReadiness`, `ImportValidationIssue`, `IWoodlandImportReader`, `WoodlandImportOptions`, `WoodlandImportRequest`. Zero wall-clock or filesystem API calls.
- `WoodlandSpeciesMapper.cs` — Case-insensitive exact-match classifier (`ModelledAtBase`: `sitka-spruce`, `sessile-oak`, `beech`; `PendingModelSupport`: `scots-pine`, `downy-birch`, `common-alder`; `UnmodelledSpecies`: other named taxa & hybrids; `UnidentifiedSpecies`: `PLANTAE`, blank, unknown, genus-only, unparsable). Zero fuzzy mapping.
- `WoodlandBoundary.cs` — Exact shoelace area (`m²` and `ha`, no bounding-box approximation); documented local tangent-plane geographic projection (`cos(centroid_lat)` with WGS-84 mean radius `6,371,008.8 m`); refuses `<3` vertices, non-finite vertices, zero-length edges, collinear zero-area rings, and self-intersecting polygons; boundary-inclusive ray-casting point-in-polygon.
- `LocalCoordinateNormalizer.cs` — Pure translation to deterministic boundary (or tree) origin preserving pairwise Euclidean distances identically (zero scale, rotation, snapping, or clamping).
- `WoodlandImportValidator.cs` — Enforces readiness precedence (`Refused` > `InventoryOnly` > `CandidateForSimulationIntegration`), unconditionally refuses Year-0 `FieldProvenance.Simulated`, counts unknown status conservatively as living, refuses out-of-bounds trees without clamping (`TreeOutOfBounds` per tree), and enforces all four hard caps (`MaxLivingStems = 3000`, `MaxTotalRecords = 20000`, `MaxMeasurementRows = 250000`, `MaxUncompressedBytes = 67108864`).
- `DarwinCoreArchiveReader.cs` — Bounded in-memory `ZipArchive` reader with per-entry and cumulative uncompressed byte accounting (both declared and streamed); `XmlReader` with `DtdProcessing.Prohibit` and `XmlResolver = null` (XXE/DOCTYPE refused with zero external fetch); `meta.xml` parsing, `CultureInfo.InvariantCulture` numbers, and `coreid` extension joining.
- `IntegrateMarteloscopeReader.cs` — `IWoodlandImportReader` converting TreMs DwC-A core + `MeasurementOrFact` rows into canonically ordinal-sorted `WoodlandImportStand` snapshots with stable source IDs, honest `FieldProvenance.Missing`, and zero allometric equations.
- `WoodlandImportSerializer.cs` — Deterministic canonical UTF-8 `System.Text.Json` writer and SHA-256 digest helper.

### 1.3 Standalone Offline Test Executable & Qualified Fixtures (`Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/`)
- `SitkaP1.WoodlandImport.Tests.csproj` — `OutputType Exe`, `AssemblyName`/`RootNamespace` `SitkaP1.Core.WoodlandImport.Tests`, single `<Compile Include="../../../src/Core/WoodlandImport/**/*.cs" />`, zero `ProjectReference`, zero `PackageReference`, zero P0 reference.
- `TestRunner.cs`, `SyntheticArchiveBuilder.cs`, `WoodlandImportCoreTests.cs`, `FixtureQualificationTests.cs`, `Program.cs`.
- `Fixtures/Qualified/ie-donadea-mhf-subset.dwca.zip` (`2,137` bytes, Irish plot `IPlus_Donadea_MHF_2`, 20 verbatim real trees spanning all 7 observed Irish species).
- `Fixtures/Qualified/lu-mersch-oak-beech-subset.dwca.zip` (`2,145` bytes, Luxembourg Oak-Beech plot `IPlus_Mersch`, 20 verbatim real trees, 100% `ModelledAtBase`).
- `Fixtures/Qualified/ATTRIBUTION.json` (DOI `10.15468/ocof3v`, CC BY 4.0 licence, full citation, GBIF occurrence keys, fixture SHA-256s, and publisher IPT fallback provenance).

### 1.4 Handoff & Source Qualification Docs (`Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/`)
- `SOURCE-DATA-QUALIFICATION.md`
- `IMPLEMENTATION-HANDOFF.md` (this file)

---

## 2. Exact Verification Commands & Verbatim Captured Output

Executed from `Tools/MultiRackSitkaP1Lab` inside WSL Ubuntu with pinned SDK `/home/jer/.dotnet-8.0.133` first on `PATH`:

```bash
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet --version
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet build   tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
PATH="$HOME/.dotnet-8.0.133:$PATH" dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
```

### Captured Output (Verbatim)
```text
=== DOTNET VERSION ===
8.0.133
=== RESTORE ===
  Determining projects to restore...
  All projects are up-to-date for restore.
=== BUILD ===
MSBuild version 17.8.60+293f66545 for .NET
  SitkaP1.WoodlandImport.Tests -> /mnt/c/Users/hp/.t3/worktrees/CCF-HP/feat-multi-2d-woodland--c498c41811949a0d/Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/bin/Debug/net8.0/SitkaP1.Core.WoodlandImport.Tests.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:04.58
=== RUN ===
PASS [synthetic] positive: supported mixed stand with surveyed boundary yields CandidateForSimulationIntegration
PASS [synthetic] malformed: empty, non-zip, missing meta.xml, broken XML, missing core/ext files refused
PASS [synthetic] refusal: duplicate ID, non-positive DBH, malformed coords, and Year-0 Simulated provenance refused
PASS [synthetic] inventory-only: unsupported living species retained without deletion, missing measurements never estimated, missing boundary never inferred
PASS [synthetic] species: all 6 target species, synonyms, authors, unmodelled taxa, PLANTAE/unidentified, and zero fuzzy matching
PASS [synthetic] boundary: non-rectangular shoelace area vs hand value, geographic tangent plane, invalid polygons refused, edge-inclusive ray casting
PASS [synthetic] bounds-and-coords: out-of-bounds trees refused without clamping; translation preserves distances exactly
PASS [synthetic] caps: MaxLivingStems (unknown counted as living), MaxTotalRecords, MaxMeasurementRows, MaxUncompressedBytes
PASS [synthetic] security: XXE / DOCTYPE declaration in meta.xml prohibited with zero external fetch
  [determinism] synthetic-canonical-sha256=dd0f6cb9331067a63fdae45cecba146e812db608121ec1b936ce8ef1e8b830db (5203 bytes)
PASS [synthetic] determinism: two imports produce byte-identical canonical JSON and identical SHA-256
PASS [synthetic] hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls
PASS [qualified] qualified-attribution: ATTRIBUTION.json and both CC BY 4.0 fixture ZIPs match pinned SHA-256 offline
PASS [qualified] qualified-ie-donadea: real Irish plot imports cleanly as InventoryOnly with all 20 verbatim trees retained
PASS [qualified] qualified-lu-mersch: compatible European Oak-Beech plot is InventoryOnly without boundary and CandidateForSimulationIntegration with boundary
  [determinism] ie-donadea-canonical-sha256=72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63 (31903 bytes)
  [determinism] lu-mersch-canonical-sha256=7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf (29876 bytes)
PASS [qualified] qualified-determinism: two independent offline imports of both CC BY 4.0 fixtures yield byte-identical JSON and SHA-256
---
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 11/11 passed, 0 failed
TOTAL: 15/15 passed, 0 failed
=== EXIT_CODE=0 ===
```

### Pass / Fail Split
- **Build:** `0 Warning(s)`, `0 Error(s)` (`TreatWarningsAsErrors=true`, `Nullable=enable`, `LangVersion=12`, `Deterministic=true`, `InvariantGlobalization=true`)
- **Synthetic suite:** `11/11` passed, `0` failed
- **Qualified real-data suite (offline):** `4/4` passed, `0` failed
- **Total:** `15/15` passed, `0` failed, exit code `0`

---

## 3. Determinism Evidence (Two-Run Byte Identity & SHA-256)

Every determinism test ran two independent imports and verified `SequenceEqual` on the canonical UTF-8 JSON bytes and equality of SHA-256 digests:

| Case | Canonical JSON Bytes | SHA-256 Digest |
|---|---|---|
| Synthetic mixed stand (CSV quoted) | `5,203` | `dd0f6cb9331067a63fdae45cecba146e812db608121ec1b936ce8ef1e8b830db` |
| Qualified Irish plot (`ie-donadea-mhf-subset.dwca.zip`, `50×100 m` boundary) | `31,903` | `72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63` |
| Qualified European plot (`lu-mersch-oak-beech-subset.dwca.zip`, `100×100 m` boundary) | `29,876` | `7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf` |

---

## 4. Real-Archive Status, Known Gaps, and Residual Risks

1. **Publisher IPT ZIP download (`PENDING REAL ARCHIVE DOWNLOAD`):** The publisher endpoint `http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate` timed out (`curl (28) Connection timed out after 60001 milliseconds`; GBIF's own weekly crawls on `2026-11-11`, `2026-11-18`, and `2026-11-25` also `ABORT`ed). Consequently, original publisher ZIP SHA-256 and byte-level inspection of the publisher's `meta.xml` remain `PENDING REAL ARCHIVE DOWNLOAD`. All real plot records and `MeasurementOrFact` rows in the two committed subset fixtures were retrieved verbatim from `api.gbif.org/v1/occurrence/{key}/verbatim` (GBIF crawlId 420 on `2026-10-30T23:26:12.556+00:00`, DOI `10.15468/ocof3v`, CC BY 4.0) and packaged into a standard tab-delimited UTF-8 DwC-A wrapper. See `SOURCE-DATA-QUALIFICATION.md`.
2. **`SitkaP1.Core.csproj` not built in this worktree:** `.p0ref/SitkaLab.Core.dll` is git-ignored and absent by design; warning-free pure-BCL compilation under the shared `Directory.Build.props` and zero `#644 B-01` banned file APIs were verified directly by the standalone test executable.
3. **Not integrated into simulation runtime:** Per D-030 and Lane 4 boundaries, this branch is **NOT** merged to `integration/2d-sim`, **NOT** wired into `MultiAreaContinuation`, `/multi`, Save v2/v3, UI, or Unity, and does **NOT** add `MultiRackSitkaP1Lab.sln` entries.
4. **Protected local listeners:** Verified down (`:33767`, `:43649`, `:46593` have zero listeners).

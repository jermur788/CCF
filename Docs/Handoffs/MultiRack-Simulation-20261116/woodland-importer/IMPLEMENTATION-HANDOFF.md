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

## 2. Exact Verification Commands & Verbatim Captured Output *(Initial Cycle 1 Baseline — Superseded by §5 Repair Cycle 1 and §6 Repair Cycle 2)*

> **Superseded historical record (NF-06):** The Cycle 1 output and test names below (15/15, `dd0f6cb9…`) are preserved as historical baseline evidence only. Authoritative current totals (`29/29`), updated test names, and current determinism hashes are in **§6 Repair Cycle 2** below.

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

## 3. Determinism Evidence *(Initial Cycle 1 Baseline — Synthetic Hash Superseded by §6; Qualified Pins Unchanged)*

Every determinism test ran two independent imports and verified `SequenceEqual` on the canonical UTF-8 JSON bytes and equality of SHA-256 digests:

| Case | Canonical JSON Bytes | SHA-256 Digest |
|---|---|---|
| Synthetic mixed stand (CSV quoted) | `5,203` | `dd0f6cb9331067a63fdae45cecba146e812db608121ec1b936ce8ef1e8b830db` |
| Qualified Irish plot (`ie-donadea-mhf-subset.dwca.zip`, `50×100 m` boundary) | `31,903` | `72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63` |
| Qualified European plot (`lu-mersch-oak-beech-subset.dwca.zip`, `100×100 m` boundary) | `29,876` | `7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf` |

---

## 4. Real-Archive Status, Known Gaps, and Residual Risks

1. **Publisher IPT ZIP & GBIF-mediated bulk DwC-A download (`PENDING REAL ARCHIVE DOWNLOAD`):**
   - Registered publisher endpoint `http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate` timed out (`curl (28) Connection timed out after 60001 milliseconds`; GBIF's own weekly crawls on `2026-11-11`, `2026-11-18`, and `2026-11-25` also `ABORT`ed).
   - GBIF-mediated DwC-A download API (`POST https://api.gbif.org/v1/occurrence/download/request`, unauthenticated) returned **`HTTP 403 Access is denied`** (credentials required); `GET https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a` timed out (`curl (28)` after 15 s).
   - Consequently, original publisher ZIP SHA-256 and byte-level inspection of the publisher's `meta.xml` remain `PENDING REAL ARCHIVE DOWNLOAD`. All real plot records and `MeasurementOrFact` rows in the two committed subset fixtures were retrieved verbatim via `HTTP 200` from `api.gbif.org/v1/occurrence/{key}/verbatim` (GBIF crawlId 420 on `2026-10-30T23:26:12.556+00:00`, DOI `10.15468/ocof3v`, CC BY 4.0) and packaged into a standard tab-delimited UTF-8 DwC-A wrapper. See `SOURCE-DATA-QUALIFICATION.md`.
2. **`SitkaP1.Core.csproj` not built in this worktree:** `.p0ref/SitkaLab.Core.dll` is git-ignored and absent by design; warning-free pure-BCL compilation under the shared `Directory.Build.props` and zero `#644 B-01` banned file APIs were verified directly by the standalone test executable.
3. **Not integrated into simulation runtime:** Per D-030 and Lane 4 boundaries, this branch is **NOT** merged to `integration/2d-sim`, **NOT** wired into `MultiAreaContinuation`, `/multi`, Save v2/v3, UI, or Unity, and does **NOT** add `MultiRackSitkaP1Lab.sln` entries.
4. **Protected local listeners:** Verified down (`:33767`, `:43649`, `:46593` have zero listeners).

---

## 5. Repair Cycle 1 (Response to Independent Review `origin/review/multi-2d-woodland-importer-v1`)

Addresses every finding (`F-01` through `F-16`) raised against candidate `e51b1e20625e51240a770211f42e1bec70c54c5c`:

| Finding | Severity | Status | Change Made |
|---|---|---|---|
| **F-01** | Blocking | **Fixed** | `DarwinCoreArchiveReader.Read` refuses blank extension `coreid`s (`ArchiveBlankExtensionCoreId`) and orphaned extension rows with no matching core record (`ArchiveOrphanExtensionRow`, sorted per missing id with row counts). Zero extension rows can be silently dropped. Covered by synthetic test `F-01`. |
| **F-02** | Blocking | **Fixed** | `IntegrateMarteloscopeReader.MapTreeRecord` tracks all single-valued measurement fields (`VitalStatus`, `DbhCm`, `HeightM`, `CrownRadiusM`, `AgeYears`) and `trem:{code}` microhabitat keys per tree and refuses `>1` row for the same field (`DuplicateMeasurementRow`) with canonically sorted values (row-order-independent SHA-256). Covered by synthetic test `F-02`. |
| **F-03** | Major | **Fixed** | `DarwinCoreArchiveReader.TryReadEntryBounded` indexes ZIP Central Directory/Local File Header raw payloads for Deflate (method 8) and Store (method 0) entries, streams decompression directly against `MaxUncompressedBytes` so lying headers cannot truncate decompression, refuses actual-vs-declared byte mismatches (`ArchiveEntrySizeMismatch`), and verifies IEEE CRC-32 (`ArchiveEntryCrcMismatch`). Covered by synthetic test `F-03` (both size-mismatch and streamed-bytes cap paths). |
| **F-04** | Major | **Fixed** | `IntegrateMarteloscopeReader.Read` tracks all distinct plot IDs when `TargetPlotId` is omitted and refuses multi-plot archives (`MultiplePlotsRequireTargetPlotId`) rather than merging separate local frames. Covered by synthetic test `F-04`. |
| **F-05** | Major | **Fixed** | `IntegrateMarteloscopeReader.Read` detects `BoundaryCoordinateSource.GeographicTangentPlane` combined with non-georeferenced plot-local tree coordinates, withholds the unregistered boundary (`Boundary = null`), emits `BoundaryTreeFrameMismatch`, and keeps the stand at `InventoryOnly`. Covered by synthetic test `F-05`. |
| **F-06** | Major | **Fixed** | (a) `ParseVitalStatus` preserves declared provenance (`Inferred`/`Simulated`/`Observed`) on unrecognised status codes instead of laundering to `Observed`; (b) core-level `provenance` is parsed first and applied to `RawSpeciesName` and `PositionLocalM` (so core `provenance=simulated` triggers `SimulatedFieldAtYearZero`). Covered by synthetic test `F-06`. |
| **F-07** | Major | **Fixed** | `ParseDoubleMeasurement` explicitly validates units (`cm`/`mm`/`m` only) and refuses unrecognised units (e.g., `inch`) with `UnsupportedMeasurementUnit` (`Refused`). Covered by synthetic test `F-07`. |
| **F-08** | Major | **Fixed** | `WoodlandSpeciesMapper.Map` classifies single-word unmapped names (bare genera `Quercus`, `Fagus`, `Betula`), genus-with-authority (`Quercus L.`, `Tilia L.`), and higher taxonomic ranks (`Fagaceae`, `Pinaceae`, `-ales`/`-opsida`/`-phyta`) as `UnidentifiedSpecies`. Covered by expanded `species` test. |
| **F-09** | Major | **Fixed** | Updated `SOURCE-DATA-QUALIFICATION.md`, `Fixtures/Qualified/ATTRIBUTION.json`, and `FixtureQualificationTests.cs` to: label local-metres `verbatimCoordinates` as an unverified inference contradicting `verbatimCoordinateSystem = decimal degrees` (axis order unverified); mark `countryCode` as GBIF-interpreted; list dropped verbatim columns (e.g. `measurementAccuracy`); add CC BY 4.0 §3(a)(1)(B) `changesMade` disclosure; and describe the 50×100 m / 100×100 m test polygons plainly as constructed test constants with no cited survey source (stating plainly that `IPlus_Mersch` reaches `CandidateForSimulationIntegration` only under that constructed test polygon). |
| **F-10** | Minor | **Fixed & Documented** | Switched `SyntheticArchiveBuilder` default compression to `CompressionLevel.NoCompression` so synthetic ZIP bytes do not vary across zlib builds, corrected the recorded synthetic hash below (`471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af`), and explicitly note that runtime-generated synthetic ZIP hashes are within-run checks only — the two committed qualified fixture hashes (`72cec3ba…` and `7fd8b963…`) are the portable cross-machine pins. |
| **F-11** | Minor | **Fixed** | `DarwinCoreArchiveReader` refuses negative `ignoreHeaderLines` (`ArchiveMalformedMetaXml`) and refuses blank lines inside the declared header region (`ArchiveMalformedHeader`) instead of shifting headers into data rows. Covered by synthetic test `F-11`. |
| **F-12** | Minor | **Fixed** | Replaced `MaxCharactersFromEntities = 0` with positive secondary bound `1024` and corrected `spec.md` §4 + code comments to state `DtdProcessing.Prohibit` + `XmlResolver = null` are the primary defence (noting `0` means unlimited in .NET). |
| **F-13** | Minor | **Fixed** | Aligned `spec.md` §5.1 wording with §5.3 and Option A #3: invalid polygons are rejected by `WoodlandBoundary.TryCreate*` (returning no boundary), leaving the stand at `InventoryOnly`. |
| **F-14** | Minor | **Documented** | Documented single-character `fieldsTerminatedBy`/`fieldsEnclosedBy` support and universal CR/LF/CRLF row-ending handling in `spec.md` §4 and `DarwinCoreArchiveReader` XML docs. |
| **F-15** | Minor | **Documented** | Documented bare `Beech` → *Fagus sylvatica* and `Alder` → *Alnus glutinosa* as explicit UK/Irish regional calibration assumptions (with *Alnus incana* substitution risk noted) in `spec.md` §6 and `WoodlandSpeciesMapper`. |
| **F-16** | Major | **Fixed** | Added 8 dedicated regression tests (`F-01`..`F-07`, `F-11`) plus expanded `species` (`F-08`) and `qualified-attribution` (`F-09`) checks; suite expanded from `15/15` to `23/23`. |

### Repair Cycle 1 Gate Verification (Pinned SDK `8.0.133`, WSL Ubuntu)

```text
=== DOTNET VERSION ===
8.0.133
=== RESTORE ===
  Determining projects to restore...
  All projects are up-to-date for restore.
=== BUILD ===
MSBuild version 17.8.60+293f66545 for .NET
  SitkaP1.WoodlandImport.Tests -> .../Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/bin/Debug/net8.0/SitkaP1.Core.WoodlandImport.Tests.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.01
=== RUN ===
PASS [synthetic] positive: supported mixed stand with surveyed boundary yields CandidateForSimulationIntegration
PASS [synthetic] malformed: empty, non-zip, missing meta.xml, broken XML, missing core/ext files refused
PASS [synthetic] refusal: duplicate ID, non-positive DBH, malformed coords, and Year-0 Simulated provenance refused
PASS [synthetic] inventory-only: unsupported living species retained without deletion, missing measurements never estimated, missing boundary never inferred
PASS [synthetic] species: all 6 target species, synonyms, authors, unmodelled taxa, genus/higher-rank/PLANTAE unidentified, zero fuzzy matching
PASS [synthetic] boundary: non-rectangular shoelace area vs hand value, geographic tangent plane, invalid polygons refused, edge-inclusive ray casting
PASS [synthetic] bounds-and-coords: out-of-bounds trees refused without clamping; translation preserves distances exactly
PASS [synthetic] caps: MaxLivingStems (unknown counted as living), MaxTotalRecords, MaxMeasurementRows, MaxUncompressedBytes
PASS [synthetic] security: XXE / DOCTYPE declaration in meta.xml prohibited with zero external fetch
  [determinism] synthetic-canonical-sha256=471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af (5203 bytes)
PASS [synthetic] determinism: two imports produce byte-identical canonical JSON and identical SHA-256
PASS [synthetic] hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls
PASS [synthetic] F-01: orphan extension rows and blank coreid extension rows are refused deterministically
PASS [synthetic] F-02: repeated/conflicting measurement rows for same tree refused identically regardless of row order
PASS [synthetic] F-03: lying ZIP uncompressed-size header refused via streamed cap or ArchiveEntrySizeMismatch without silent truncation
PASS [synthetic] F-04: multi-plot archive without TargetPlotId is refused; specifying TargetPlotId succeeds for that plot
PASS [synthetic] F-05: geographic boundary combined with non-georeferenced local trees yields InventoryOnly with BoundaryTreeFrameMismatch
PASS [synthetic] F-06: unknown status preserves Inferred provenance; core provenance=simulated applies to species and position and refuses
PASS [synthetic] F-07: unknown measurement unit (inch) refused with UnsupportedMeasurementUnit
PASS [synthetic] F-11: negative ignoreHeaderLines and leading blank line before header refused with format codes
PASS [qualified] qualified-attribution: ATTRIBUTION.json (incl. CC BY 4.0 changesMade) and both fixture ZIPs match pinned SHA-256 offline
PASS [qualified] qualified-ie-donadea: real Irish plot imports cleanly as InventoryOnly with all 20 verbatim trees retained
PASS [qualified] qualified-lu-mersch: European Oak-Beech subset is InventoryOnly without boundary and Candidate only under constructed 100x100 m test boundary
  [determinism] ie-donadea-canonical-sha256=72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63 (31903 bytes)
  [determinism] lu-mersch-canonical-sha256=7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf (29876 bytes)
PASS [qualified] qualified-determinism: two independent offline imports of both CC BY 4.0 fixtures yield byte-identical JSON and SHA-256
---
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 19/19 passed, 0 failed
TOTAL: 23/23 passed, 0 failed
=== EXIT_CODE=0 ===
```

- **Repair Cycle 1 Totals:** `0 Warning(s)`, `0 Error(s)`, exit code `0`; **Synthetic `19/19` passed, Qualified `4/4` passed, Total `23/23` passed (`0` failed)**.
- **Portable Cross-Machine Determinism Pins (committed fixtures):**
  - `ie-donadea-mhf-subset.dwca.zip` (under constructed `50×100 m` test boundary): `72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63` (`31,903` bytes — unchanged from initial submission)
  - `lu-mersch-oak-beech-subset.dwca.zip` (under constructed `100×100 m` test boundary): `7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf` (`29,876` bytes — unchanged from initial submission)
- **Within-run synthetic determinism check (non-portable runtime ZIP, F-10):** `471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af` (`5,203` bytes). *(Note from R2: the earlier `471f61e8…` vs `395962d1…` split was caused by CRLF vs LF line endings in C# verbatim string literals inside `BuildDefaultMetaXml`, not zlib; resolved in Repair Cycle 2 below.)*

---

## 6. Repair Cycle 2 (Final Cycle — Response to R2 Verification & Opus Cycle-1 Review)

Addresses every remaining blocking and minor finding from `REVIEW-R2-REPAIR-VERIFICATION.md` and `OPUS-INDEPENDENT-REVIEW-20261126.md` against candidate `add375a4135387daf16e9ff506066bae6bd28a62`:

| Finding | Severity | Status | Change Made |
|---|---|---|---|
| **R3.1** | Blocking | **Fixed** | Replaced loose substring matching in `IntegrateMarteloscopeReader` with exact documented `measurementType`/`measurementRemarks` classifiers. Checked crown measurements before stem diameter; explicitly excluded `"crown"` from DBH and height classifiers; mapped `Crown diameter` (`m`/`cm`/`mm`) to `CrownRadiusM` (`radius = diameter / 2`) while leaving `DbhCm` `Missing` when no stem DBH row is present. Covered by new test `R3.1` (both `m` and `cm` cases + combined DBH/crown control). |
| **R5** | Blocking | **Fixed** | Added optional additive `DeclaredArchiveSha256` on `ImportSourceAttribution` (`WoodlandImportContracts.cs` & `WoodlandImportSerializer.cs`). `IntegrateMarteloscopeReader` ALWAYS records the computed archive SHA-256 in `ArchiveSha256` (never overwritten by caller overrides), verifies any caller-declared hash (`DeclaredArchiveSha256` or legacy `AttributionOverride.ArchiveSha256`), and refuses mismatches with `ArchiveSha256Mismatch` (`Stand = null`). Covered by new test `R5`. |
| **NF-01 / N3** | Blocking / Minor | **Fixed & Documented** | Replaced `ExtractPlotId` with `TryExtractPlotSurveyId`: extracts a plot-survey key ONLY from explicit columns (`locationID`/`eventID`/`locality`) or the Integrate+ `{digits}_{plotName}[_{surveyIndex}]` pattern (all-digit prefix + letter-containing suffix), retaining any trailing `_{surveyIndex}` (N3) intentionally so distinct survey epochs are never merged into one Year-0 stand. Unpatterned single-plot archives (`t1`/`t2`, `tree_1`/`tree_2`) return `null` and import cleanly as one stand. Covered by new test `NF-01/N3` and documented in `spec.md` §5.3 & `SOURCE-DATA-QUALIFICATION.md`. |
| **NF-02** | Minor | **Fixed** | Changed `distinctPlots` and `TargetPlotId` matching to `StringComparer.Ordinal` so plot IDs differing only by case (`PlotA` vs `plota`) are refused as distinct plots (`MultiplePlotsRequireTargetPlotId`) and never silently merged. Covered by new test `NF-02`. |
| **NF-03** | Minor | **Fixed** | When `>1` row exists for any single-valued measurement field on a tree, `MapTreeRecord` resets that field deterministically to `Missing` regardless of row order, so duplicate rows where one row is malformed (`abc` + `30` vs `30` + `abc`) yield byte-identical refusal JSON and SHA-256. Covered by new test `NF-03`. |
| **NF-04** | Minor (test) | **Fixed** | Fixed `F-03` Case B to use a core-only deflated archive (`includeExtension: false`) whose total declared ZIP size (< 1 KB) passes the 4,096 B declared pre-check and genuinely triggers the streamed-bytes cap while decompressing `occurrence.txt` (asserting the exact streamed-cap message). |
| **NF-05** | Minor | **Fixed** | `ParseDoubleMeasurement` now refuses empty `measurementUnit` on dimensional `MeasurementOrFact` rows with `MissingMeasurementUnit` (`Refused`) instead of silently assuming `cm`/`m` (real TreMs fixtures carry explicit `cm`/`m` units and are unaffected). Covered by new test `NF-05`. |
| **NF-06** | Minor (docs) | **Fixed** | Marked Cycle 1 §2/§3 explicitly as superseded historical baseline records and corrected the F-10 root-cause explanation. |
| **F-10 residual** | Minor | **Fixed & Documented** | Normalized verbatim-string `\r\n` to `\n` inside `SyntheticArchiveBuilder.BuildDefaultMetaXml` so synthetic `meta.xml` bytes no longer vary between CRLF and LF checkouts. Explicitly note that the runtime-built synthetic hash is still a within-run check only; the two committed qualified fixture hashes (`72cec3ba…` and `7fd8b963…`) remain the authoritative portable cross-machine pins. |

### Repair Cycle 2 Gate Verification (Pinned SDK `8.0.133`, WSL Ubuntu)

```text
=== DOTNET VERSION ===
8.0.133
=== RESTORE ===
  Determining projects to restore...
  All projects are up-to-date for restore.
=== BUILD ===
MSBuild version 17.8.60+293f66545 for .NET
  SitkaP1.WoodlandImport.Tests -> .../Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/bin/Debug/net8.0/SitkaP1.Core.WoodlandImport.Tests.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.75
=== RUN ===
PASS [synthetic] positive: supported mixed stand with surveyed boundary yields CandidateForSimulationIntegration
PASS [synthetic] malformed: empty, non-zip, missing meta.xml, broken XML, missing core/ext files refused
PASS [synthetic] refusal: duplicate ID, non-positive DBH, malformed coords, and Year-0 Simulated provenance refused
PASS [synthetic] inventory-only: unsupported living species retained without deletion, missing measurements never estimated, missing boundary never inferred
PASS [synthetic] species: all 6 target species, synonyms, authors, unmodelled taxa, genus/higher-rank/PLANTAE unidentified, zero fuzzy matching
PASS [synthetic] boundary: non-rectangular shoelace area vs hand value, geographic tangent plane, invalid polygons refused, edge-inclusive ray casting
PASS [synthetic] bounds-and-coords: out-of-bounds trees refused without clamping; translation preserves distances exactly
PASS [synthetic] caps: MaxLivingStems (unknown counted as living), MaxTotalRecords, MaxMeasurementRows, MaxUncompressedBytes
PASS [synthetic] security: XXE / DOCTYPE declaration in meta.xml prohibited with zero external fetch
  [determinism] synthetic-canonical-sha256=471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af (5203 bytes)
PASS [synthetic] determinism: two imports produce byte-identical canonical JSON and identical SHA-256
PASS [synthetic] hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls
PASS [synthetic] F-01: orphan extension rows and blank coreid extension rows are refused deterministically
PASS [synthetic] F-02: repeated/conflicting measurement rows for same tree refused identically regardless of row order
PASS [synthetic] F-03: lying ZIP uncompressed-size header refused via streamed cap or ArchiveEntrySizeMismatch without silent truncation
PASS [synthetic] F-04: multi-plot archive without TargetPlotId is refused; specifying TargetPlotId succeeds for that plot
PASS [synthetic] F-05: geographic boundary combined with non-georeferenced local trees yields InventoryOnly with BoundaryTreeFrameMismatch
PASS [synthetic] F-06: unknown status preserves Inferred provenance; core provenance=simulated applies to species and position and refuses
PASS [synthetic] F-07: unknown measurement unit (inch) refused with UnsupportedMeasurementUnit
PASS [synthetic] F-11: negative ignoreHeaderLines and leading blank line before header refused with format codes
PASS [synthetic] R3.1: Crown diameter in m and cm maps to CrownRadiusM and never satisfies DBH
PASS [synthetic] R5: computed ArchiveSha256 is never replaced; wrong declared hash refused; matching declared hash recorded separately
PASS [synthetic] NF-01/N3: plain-ID single-plot archives import cleanly; multi-survey Integrate+ epochs without TargetPlotId are refused
PASS [synthetic] NF-02: plot IDs differing only by case (PlotA vs plota) are refused as distinct plots
PASS [synthetic] NF-03: duplicate measurement with one malformed row yields byte-identical refusal JSON in both row orders
PASS [synthetic] NF-05: empty measurementUnit on DBH or height row is refused with MissingMeasurementUnit
PASS [qualified] qualified-attribution: ATTRIBUTION.json (incl. CC BY 4.0 changesMade) and both fixture ZIPs match pinned SHA-256 offline
PASS [qualified] qualified-ie-donadea: real Irish plot imports cleanly as InventoryOnly with all 20 verbatim trees retained
PASS [qualified] qualified-lu-mersch: European Oak-Beech subset is InventoryOnly without boundary and Candidate only under constructed 100x100 m test boundary
  [determinism] ie-donadea-canonical-sha256=72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63 (31903 bytes)
  [determinism] lu-mersch-canonical-sha256=7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf (29876 bytes)
PASS [qualified] qualified-determinism: two independent offline imports of both CC BY 4.0 fixtures yield byte-identical JSON and SHA-256
---
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 25/25 passed, 0 failed
TOTAL: 29/29 passed, 0 failed
=== EXIT_CODE=0 ===
```

- **Repair Cycle 2 Totals:** `0 Warning(s)`, `0 Error(s)`, exit code `0`; **Synthetic `25/25` passed, Qualified `4/4` passed, Total `29/29` passed (`0` failed)**.
- **Authoritative Portable Determinism Pins (committed CC BY 4.0 fixtures — unchanged across all cycles):**
  - `ie-donadea-mhf-subset.dwca.zip`: `72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63` (`31,903` bytes)
  - `lu-mersch-oak-beech-subset.dwca.zip`: `7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf` (`29,876` bytes)
- **Within-run synthetic determinism hash (LF-normalized `meta.xml`, not a cross-machine pin):** `471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af` (`5,203` bytes).
- **Exact Residual Risk List (honest boundary):**
  1. Original INRAE publisher IPT ZIP & `meta.xml` remain `PENDING REAL ARCHIVE DOWNLOAD` (`dynids.toulouse.inra.fr:8180` timed out; unauthenticated GBIF bulk download returned `HTTP 403`).
  2. `verbatimCoordinates` local-metres reading (contradicting `verbatimCoordinateSystem = decimal degrees`) and X/Y axis orientation remain unverified assumptions pending publisher documentation.
  3. Constructed `50×100 m` / `100×100 m` test polygons in `FixtureQualificationTests.cs` have no cited survey source for Donadea or Mersch; without a real surveyed polygon both plots remain `InventoryOnly`.
  4. Full `MultiRackSitkaP1Lab.sln` / `SitkaP1.Core.csproj` / `Core.Tests` rebuild was not run in this worktree because `.p0ref/SitkaLab.Core.dll` is git-ignored and absent by design.
  5. Pure-BCL importer is compiled into `SitkaP1.Core.dll` by default globbing once merged, and remains **NOT** wired into `MultiAreaContinuation`, `/multi`, Save v2/v3, UI, or Unity (D-030).

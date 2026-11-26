# CCF Multi 2D — Real-World Woodland Importer V1 — Specification

- **Spec ID:** `SPEC-multi-2d-woodland-importer-V1`
- **Feature ID:** `multi-2d-woodland-importer-v1`
- **Status:** `APPROVED / IMPLEMENTING (owner-approved 2026-11-26)`
- **Base Commit:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Branch:** `feat/multi-2d-woodland-importer-v1`
- **Scope Boundary (D-030):** Isolated pure-BCL Layer 1 contracts, validation, species mapping, coordinate normalization, Darwin Core Archive (DwC-A) reader, Integrate+ marteloscope reader, standalone offline test executable, and real-archive qualification fixtures/documentation. **NOT** merged to `integration/2d-sim`; **NOT** wired into `MultiAreaContinuation`, `/multi`, Save v2/v3, UI, or Unity.

---

## 1. Locked Owner Product Decisions (Option A — Non-Negotiable)

1. **Unsupported species:** Retain **all** measured trees in the imported inventory for inspection; return `WoodlandReadiness.InventoryOnly`; block simulation admission while any unsupported *living* tree remains (unknown vital status is counted conservatively as living). Zero silent tree deletion and zero silent species substitution.
2. **Missing measurements:** Preserve missing measurements honestly with `FieldProvenance.Missing`. **No** allometric, height–diameter, crown, or growth estimation equations exist anywhere in this feature.
3. **Incomplete boundaries:** Return `WoodlandReadiness.InventoryOnly` until a valid surveyed boundary polygon is supplied. Never infer or fabricate a convex-hull or bounding-box boundary for simulation admission.
4. **First real-world demonstration:** Qualify an Irish plot first (real-data import and honest inspection, returning `InventoryOnly` where unsupported living species or missing surveyed boundary apply), and qualify at least one compatible mixed European plot alongside it.

---

## 2. Strict File Fence

Writable paths are strictly confined to four prefixes:

- `specs/multi-2d-woodland-importer-v1/**`
- `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/**`
- `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/**`
- `Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/**`

Production code under `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/**` MUST remain pure BCL (`net8.0`, nullable enabled, `TreatWarningsAsErrors=true`, deterministic, invariant globalization), with zero Unity dependencies, zero P0 binary dependencies, zero `MultiAreaContinuation` coupling, and zero filesystem-writing or path-based `System.IO.File`/`Directory` API calls (preserving the P1 `#644 B-01` production source constraint).

---

## 3. Layer 1 Contracts (`SitkaP1.Core.WoodlandImport`)

Fixed public contract types (immutable, deterministic, zero wall-clock or machine-dependent values in outputs):

- `WoodlandImportStand` (`SchemaVersion = 1`) — stand metadata, `ImportSourceAttribution`, optional `WoodlandBoundary`, optional `LocalCoordinateOrigin`, canonically ordered `Trees`, scan counters, and conservative living/dead/unknown stem totals.
- `ImportedTreeRecord` — `SourceTreeId`, `RawSpeciesName`, `SpeciesMapping`, `VitalStatus`, `PositionLocalM`, `DbhCm`, `HeightM`, `CrownRadiusM`, `AgeYears`, deterministic `ExtraAttributes`.
- `ImportField<T>` — `Provenance`, `HasValue`, `Value`, `Evidence`.
- `FieldEvidence` — `SourceColumn`, `RawValue`, optional `Note`.
- `FieldProvenance` — `Missing`, `Observed`, `Inferred`, `Simulated` (Year-0 `Simulated` is unconditionally refused by validation).
- `ImportSourceAttribution` — `DatasetTitle`, `Publisher`, `Licence`, `Citation`, `Doi`, `DatasetKey`, `ArchiveSha256` (always computed from the imported archive bytes; never overwritten by caller input), optional `DeclaredArchiveSha256` (verified against `ArchiveSha256`, refusing on mismatch with `ArchiveSha256Mismatch`), `RetrievalDateUtc` (explicit metadata input, never system clock), `SourcePlotId`.
- `WoodlandImportResult` — `Readiness`, `Stand` (`null` on `Refused`), canonically sorted `Issues`, `IsAdmissibleForSimulation`.
- `WoodlandReadiness` — `Refused`, `InventoryOnly`, `CandidateForSimulationIntegration`.
- `ImportValidationIssue` — `Code`, `Severity`, `Message`, optional `SourceTreeId`, optional `FieldName`.
- `IWoodlandImportReader` — `WoodlandImportResult Read(WoodlandImportRequest request)`.
- `WoodlandImportOptions` — deterministic caps and admission policy flags.
- `WoodlandImportRequest` — memory-backed `ArchiveBytes` (`ReadOnlyMemory<byte>`), optional `TargetPlotId`, optional surveyed boundary input, optional attribution override, and `Options`.

---

## 4. Hard Caps and Security Controls

All caps are enforced **before** unbounded memory allocation:

| Cap | Default Value | Enforcement Rule |
|---|---|---|
| `MaxLivingStems` | `3,000` | Unknown/missing vital status is counted conservatively as living. |
| `MaxTotalRecords` | `20,000` | Enforced while streaming core table rows. |
| `MaxMeasurementRows` | `250,000` | Enforced cumulatively across all extension tables. |
| `MaxUncompressedBytes` | `67,108,864` (64 MiB) | Checked on both declared ZIP entry sizes and actual streamed decompressed bytes (per-entry and cumulative total). |

### XML / XXE Hardening & Archive Integrity (`DarwinCoreArchiveReader`)
- `meta.xml` is parsed using `XmlReaderSettings` with **`DtdProcessing = DtdProcessing.Prohibit`** and **`XmlResolver = null`** as the primary XXE / DTD / entity-expansion defence, plus `MaxCharactersInDocument = options.MaxUncompressedBytes` and a small positive secondary `MaxCharactersFromEntities = 1024` bound (note: `0` means *unlimited* in .NET, so `DtdProcessing.Prohibit` is the actual entity defence).
- Any `<!DOCTYPE` or entity declaration is refused deterministically (`ArchiveXmlDtdProhibited`) with zero external network fetches.
- **ZIP integrity & streamed-cap enforcement:** every read entry verifies actual decompressed bytes against both `options.MaxUncompressedBytes` (streamed directly from the raw Deflate/Store payload so lying uncompressed-size headers cannot truncate decompression) and the declared `entry.Length` (`ArchiveEntrySizeMismatch`), and verifies IEEE CRC-32 (`ArchiveEntryCrcMismatch`).
- **Extension join & row accounting:** every extension row MUST match a non-blank core id present in the core table. Blank extension `coreid`s (`ArchiveBlankExtensionCoreId`) and orphaned extension rows referencing missing core ids (`ArchiveOrphanExtensionRow`, with count and missing id) are refused deterministically; zero rows are silently dropped.
- **Header & delimiter rules:** negative `ignoreHeaderLines` is refused (`ArchiveMalformedMetaXml`); blank lines inside the declared header region are refused (`ArchiveMalformedHeader`) rather than shifting the header into data rows. Only single-character `fieldsTerminatedBy` and `fieldsEnclosedBy` (first character after unescaping `\t`/`\r`/`\n`) are supported; CR, LF, and CRLF are always treated as row endings regardless of `linesTerminatedBy`.

---

## 5. Geometry, Coordinates, and Validation Precedence

### 5.1 Surveyed Boundary (`WoodlandBoundary`)
- Planar polygon area is calculated using the exact **shoelace formula** in `m²` and `ha` (`AreaM2 / 10_000.0`). Bounding-box approximations are prohibited.
- Geographic WGS-84 vertices are projected to local metres on a documented local tangent plane centred at the arithmetic centroid of the distinct polygon ring vertices:
  - `xM = (lon - lon0) * cos(lat0 * pi / 180) * (R * pi / 180)`
  - `yM = (lat - lat0) * (R * pi / 180)`
  - `R = 6,371,008.8 m` (WGS-84 mean spherical Earth radius).
  - Approximation boundary: valid for stand-scale woodland plots (tens to hundreds of metres); does not claim ellipsoidal UTM distortion correction.
- Polygons with `< 3` distinct vertices, any non-finite coordinate, zero-length/degenerate edges, self-intersections, or zero area (`<= 1e-9 m²`) are rejected by `WoodlandBoundary.TryCreateLocal` / `TryCreateFromGeographic` (returning no boundary object). Per Option A #3 and §5.3, when a stand lacks a valid boundary (whether omitted or rejected), `WoodlandImportValidator` keeps the stand at `WoodlandReadiness.InventoryOnly` with `Boundary = null` and the boundary issue recorded.
- **Frame registration guard:** a `GeographicTangentPlane` boundary (centred at the polygon's WGS-84 centroid) is NOT registered to plot-local tree coordinates lacking per-tree georeferencing. When combined with non-georeferenced plot-local trees, `IntegrateMarteloscopeReader` withholds the unregistered boundary (`Boundary = null`), records `BoundaryTreeFrameMismatch`, and keeps the stand at `InventoryOnly` rather than computing cross-frame containment.
- Point-in-polygon uses ray casting with one documented edge rule: **boundary-inclusive** (points within `1e-9 m` of any edge or vertex are inside; strictly interior points are inside; exterior or non-finite points are outside).
- Trees outside a valid surveyed boundary are **refused without clamping**, emitting one stable `TreeOutOfBounds` refusal issue per out-of-bounds tree.

### 5.2 Coordinate Normalization (`LocalCoordinateNormalizer`)
- Translates boundary vertices and observed tree coordinates by subtracting a deterministic origin:
  - Surveyed boundary present: `(min(v.XM), min(v.YM))` (`boundary-min-xy-translation-only-v1`).
  - No boundary present: `(min(t.XM), min(t.YM))` over observed tree coordinates (`tree-min-xy-translation-only-v1`).
- **Translation only:** pairwise Euclidean distances are preserved identically; zero scale, zero rotation, zero snapping, zero clamping.

### 5.3 Validation Readiness Precedence (`WoodlandImportValidator`)
1. **`Refused`** (highest precedence — structurally untrustworthy): malformed ZIP/XML/header, ZIP entry size or CRC-32 mismatch, declared vs computed `ArchiveSha256Mismatch`, DTD/XXE payload, cap exceeded, orphaned or blank-key extension rows, repeated/conflicting single-valued measurement or TreM rows for the same tree (`DuplicateMeasurementRow`, reported order-independently with duplicated fields reset to `Missing`), multi-plot or multi-survey archive without `TargetPlotId` (`MultiplePlotsRequireTargetPlotId`, ordinal case-sensitive distinctness; plot-survey IDs derived only from explicit `locationID`/`eventID`/`locality` or Integrate+ `{digits}_{plotName}[_{surveyIndex}]` IDs so plain unpatterned single-plot archives import cleanly and distinct survey epochs never merge), missing or unsupported dimensional measurement unit (`MissingMeasurementUnit`, `UnsupportedMeasurementUnit`), `FieldProvenance.Simulated` at Year 0 (including core-level `provenance=simulated` on species/position), inconsistent provenance/value flags, blank or duplicate `SourceTreeId`, non-finite coordinates, non-finite/non-positive observed DBH or height, negative crown radius or age, or any tree outside a valid surveyed boundary. `Stand` is returned as `null`.
   - **Exact measurement-type classification (R3.1):** `MeasurementOrFact` rows are classified by exact documented `measurementType`/`measurementRemarks` values (no loose substring matching). `Crown diameter` maps to `CrownRadiusM` (`radius = diameter / 2` in metres) and never satisfies stem `DbhCm`.
2. **`InventoryOnly`** (structurally sound inventory retained in full, blocked from simulation admission): missing, invalid, or frame-unregistered (`BoundaryTreeFrameMismatch`) boundary, zero living trees, any living (or unknown-status) tree whose species is not simulation-supported, or any living (or unknown-status) tree missing a simulation-required field (`VitalStatus`, `PositionLocalM`, `DbhCm`, or `HeightM` when required).
3. **`CandidateForSimulationIntegration`**: zero refusal issues and zero inventory-only blockers.

---

## 6. Species Mapping (`WoodlandSpeciesMapper`)

Case-insensitive exact matching on scientific names (including standard taxonomic authority suffixes, excluding hybrids `x`/`×`) and documented common synonyms. **No fuzzy or near-match substitution.**

| Category | Canonical IDs | Scientific Names & Synonyms |
|---|---|---|
| `ModelledAtBase` | `sitka-spruce`, `sessile-oak`, `beech` | `Picea sitchensis` (`Sitka spruce`), `Quercus petraea` (`Sessile oak`, `Durmast oak`, `Quercus sessiliflora`), `Fagus sylvatica` (`European beech`, `Common beech`, `Beech`*) |
| `PendingModelSupport` | `scots-pine`, `downy-birch`, `common-alder` | `Pinus sylvestris` (`Scots pine`, `Scotch pine`), `Betula pubescens` (`Downy birch`, `Moor birch`, `Hairy birch`), `Alnus glutinosa` (`Common alder`, `Black alder`, `European alder`, `Alder`*) |
| `UnmodelledSpecies` | `null` | Any other named species/binomial taxon or hybrid (e.g., `Picea abies`, `Fraxinus excelsior`, `Quercus robur`, `Ulmus glabra`, `Acer pseudoplatanus`, `Quercus petraea x robur`). |
| `UnidentifiedSpecies` | `null` | `PLANTAE`, blank/whitespace, `unknown`, `indet.`, `sp.`/`spp.`, bare genus (`Quercus`, `Fagus`, `Betula`), genus-with-authority (`Quercus L.`, `Tilia L.`), genus `sp.`, higher taxonomic ranks (`Fagaceae`, `Pinaceae`, `-ales`, `-opsida`, `-phyta`), or unparsable non-taxon tokens. |

*\*Deliberate regional calibration/assumption (F-15):* Mapping bare common names `Beech` → *Fagus sylvatica* and `Alder` → *Alnus glutinosa* is an explicit UK/Irish forestry calibration choice, not a universal global taxonomic equivalence; if an external dataset uses bare `"Alder"` for *Alnus incana* (grey alder), this creates a substitution risk and scientific binomials should be preferred whenever present.

---

## 7. Real-World Source Qualification (`trems_integrate`)

- Target dataset: **Integrate+ TreMs** (INRAE), DOI `10.15468/ocof3v`, GBIF dataset key `2e102194-f384-4712-89a4-5db7a3fc409a`, licence **CC BY 4.0**.
- Real-archive download of the publisher IPT ZIP (`http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate`) is attempted first. If unreachable, the pre-settled fallback applies: document the exact network error in `SOURCE-DATA-QUALIFICATION.md`, record publisher ZIP / verbatim `meta.xml` status transparently, and qualify real CC BY 4.0 verbatim records retrieved from GBIF where available without fabricating a publisher ZIP hash or claiming an unverified publisher `meta.xml`.

---

## 8. Acceptance Gates

1. `dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj` — 0 warnings, 0 errors (pinned SDK `8.0.133`).
2. `dotnet build tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore` — 0 warnings, 0 errors (`TreatWarningsAsErrors=true`).
3. `dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore` — exit code 0, all synthetic and qualified tests PASS.
4. Determinism: two independent imports of the same archive produce byte-identical canonical JSON and identical SHA-256 hashes.
5. Fence proof: `git diff --name-only 6d72c30538b1fbe30d3cbcf083b5b368733662b8...HEAD` contains exclusively files inside the four allowed prefixes.

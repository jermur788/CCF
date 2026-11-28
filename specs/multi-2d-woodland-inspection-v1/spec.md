# CCF Multi 2D — Woodland Inspection V1 — Specification

- **Spec ID:** `SPEC-multi-2d-woodland-inspection-V1`
- **Feature ID:** `multi-2d-woodland-inspection-v1`
- **Status:** `APPROVED / IMPLEMENTING — Part 1 (inspection engine) ACTIVE; Part 2 (on-screen picker/UI and any wiring) QUEUED behind Playable Milestone 1` (owner-approved 2026-11-27, Option A locked)
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`, closed Woodland Importer V1)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Part 1 Branch:** `feat/multi-2d-woodland-inspection-engine-v1` (review ref `review/multi-2d-woodland-inspection-engine-v1`)
- **Owner decision record:** `2D_Simulator_Strategic_Roadmap.md v1.17` — **external**; it is not present in the Git tree and is cited here as the owner's out-of-repo record.
- **Scope Boundary (D-030):** Part 1 is an isolated pure-BCL engine plus a standalone offline test executable. It is **NOT** merged to `integration/2d-sim` and **NOT** wired into `MultiAreaContinuation`, `AnnualPipeline`, saves, or `/multi`.

---

## 1. Purpose

Let a player look at a real, qualified woodland exactly as it was measured (Year 0), with every value's provenance and every reason it cannot yet be simulated, without the game inventing anything. Part 1 builds the data side only: a fixed catalogue of built-in plots, a projection from the importer's result into a read-only view, and canonical JSON for that view. Part 2 will put it on screen.

## 2. Locked Owner Decisions (Option A — non-negotiable)

1. **Built-in qualified catalogue only.** Part 1 offers only the woodlands already qualified by Woodland Importer V1, in a fixed order with **Donadea (Ireland) first**, then Mersch (Luxembourg). No local ZIP upload and no network access in Part 1.
2. **Read-only Year-0 inspection.** Observed data and any simulated future are strictly separated. The view never contains simulated future values. `CanAdvanceTime` is `true` only when the importer returned `CandidateForSimulationIntegration`, a stand is present, and no field carries `Simulated` provenance. It is `false` for `InventoryOnly` and `Refused`.
3. **Importer stays closed.** Parked importer items NF-07, NF-08 and NF-09 stay deferred. Closed Importer V1 (`c82e4e75`) is not reopened: no file under `src/Core/WoodlandImport/*.cs` or `tests/Core/WoodlandImport/**` is modified.

## 3. Strict File Fence (Part 1)

Only these prefixes may change relative to the base commit:

- `specs/multi-2d-woodland-inspection-v1/**`
- `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/Inspection/**`
- `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandInspection/**`
- `Docs/Handoffs/MultiRack-Simulation-20261116/woodland-inspection/**`

Production code under `src/Core/WoodlandImport/Inspection/**` is pure BCL (`net8.0`, nullable, warnings as errors, deterministic, invariant globalization) and uses no file-system, clock, random, network or Unity API. Because it sits inside `src/Core/WoodlandImport/**`, it is also scanned and compiled by the existing Importer V1 hygiene test (P1 `#644 B-01`), which must keep passing.

## 4. Components (namespace `SitkaP1.Core.WoodlandImport.Inspection`)

### 4.1 `BuiltInWoodlandCatalogue`

- `Definitions` — immutable (`ImmutableList`), fixed order:

| # | PlotId | Display name | Country | Fixture | Pinned SHA-256 | Subset / GBIF plot total |
|---|---|---|---|---|---|---|
| 0 | `IPlus_Donadea_MHF_2` | Donadea MHF 2 plot (Ireland) | IE | `ie-donadea-mhf-subset.dwca.zip` | `4ee2f4a125d1a06474d7086ccf5a6ca96735e16124ad3e4f9a11647ddcee853d` | 20 / 271 |
| 1 | `IPlus_Mersch` | Mersch oak-beech plot (Luxembourg) | LU | `lu-mersch-oak-beech-subset.dwca.zip` | `3c275b0115b6330ebabdad422ea9a25451489c93949dd5d80be1fb5e8b57b1ae` | 20 / 134 |

- `Load(Func<string, byte[]> readFixtureFile)` reads `ATTRIBUTION.json` (the single source of truth for citation, licence, DOI, dataset key, retrieval date, `changesMade` disclosure and caveats), checks that each fixture's ATTRIBUTION SHA-256 and record counts equal the pinned values (otherwise `InvalidDataException`), then imports each ZIP through the existing `IntegrateMarteloscopeReader` with `TargetPlotId = PlotId`, an `AttributionOverride` carrying the ATTRIBUTION values, and `DeclaredArchiveSha256 = pinned hash`. A tampered ZIP is therefore `Refused` by the existing reader. **No boundary is ever passed.**
- Each `BuiltInWoodlandEntry` carries: plot id, display name, country code, tree count, living stem count, "N of M GBIF records" text, readiness, licence summary `CC BY 4.0` and licence URL, dataset title, publisher, full citation, DOI, dataset key, retrieval date, pinned archive SHA-256, observation date(s), the verbatim modifications disclosure, caveats, `Disclosures`, the built-in `InspectionView` (projected with those disclosures), and the `WoodlandImportResult` as an immutable snapshot (every list and dictionary copied into an immutable collection, so no backing collection is reachable). *(Repair R1: WI-02, WI-03.)*
- `Load` throws `InvalidDataException` when `changesMade` has no `Note:` caveat about `verbatimCoordinates`, so a built-in stand is never shown without it. *(Repair R1: WI-02.)*
- **Deviation from the task packet:** the directory convenience loader (`LoadFromDirectory`) lives in the test project (`TestFixtures.LoadFromDirectory`), not in production code. Path-based file APIs in `src/Core/WoodlandImport/**` would break P1 `#644 B-01` and the existing Importer V1 hygiene test, which may not be edited. Part 2 supplies its own `readFixtureFile` function.

### 4.2 `WoodlandInspectionProjector.Project(WoodlandImportResult[, InspectionDisclosures?])` → `WoodlandInspectionView`

Immutable records; every collection is an `ImmutableList` (no mutable backing store reachable through `ICollection.SyncRoot`). *(Repair R1: WI-03.)*

- `Readiness`, `CanAdvanceTime` (rule in §2.2), `AdvanceTimeStatement` (plain English).
- `Stand` identity — `null` when the import was Refused.
- Projection-boundary rules that hold even for a hand-built result *(Repair R1: WI-01, WI-04)*: a `Refused` result projects no stand data (no identity, attribution, trees, origin, boundary, species, microhabitats or observation dates), whatever the result object carries; a stand holding any `Simulated` field is withheld the same way, reported as `Refused`, with a `SimulatedFieldAtYearZero` Refusal checklist item naming the affected trees but none of their values. `SimulatedValueCount` still reports how many such fields were found.
- `Disclosures` — `{ModificationsDisclosure, CoordinateInterpretationCaveat}` when supplied (built-in stands), else `null`. They are additional fields; no provenance label is changed. *(Repair R1: WI-02.)*
- `ObservedYear0`: trees with local-metre positions **exactly as stored on the stand** (no re-normalisation or rounding); the stand's `LocalCoordinateOrigin` (origin + derivation rule); boundary `{Observed, VerticesLocalM, AreaM2, AreaHa, CoordinateSource, ProjectionNote}` or `Observed = false` with no vertices and null area.
- Every measurement (species name, vital status, position, DBH, height, crown radius, age) as `InspectedField<T>` = `{Provenance, Value (null when Missing), SourceColumn, RawValue, Note}` copied verbatim. No value is ever estimated.
- `SpeciesBreakdown`: all four `SpeciesSupportCategory` values always listed (total + living), and one row per raw species name (raw name, canonical id, category, total, living) sorted by category then ordinal raw name. "Living" uses the importer's conservative rule (unknown status counts as living).
- `Microhabitats`: `{SourceTreeId, Code, Value}` from importer `trem:<code>` attributes, sorted ordinal.
- `Attribution`: licence, citation, DOI, dataset key, publisher, archive SHA-256, declared SHA-256, retrieval date, source plot id, observation date(s) from `eventDate`.
- `ProvenanceBoundary`: observed state year 0, observation date(s), simulated future starts at year 1, `SimulatedFutureAvailable = false`, `SimulatedValueCount`, and a statement that nothing past Year 0 is shown.
- `BlockerChecklist`: one item per distinct blocking code (severity `InventoryOnlyBlocker` or `Refusal`) with code, severity, short title, plain-English explanation (written for every blocker and refusal code the importer can emit — Repair R1: WI-05; unknown codes fall back to the importer's message and use the code as title), affected tree count, sorted tree ids and sorted raw species names. Ordered Refusal first, then ordinal code. `Info`/`Warning` issues go to `Notes` verbatim. Part 2 needs no logic to render it.

### 4.3 `WoodlandInspectionSerializer`

`SerializeUtf8`, `SerializeJson`, `ComputeSha256Hex`: canonical UTF-8 JSON via `Utf8JsonWriter`, fixed key order (including a top-level `disclosures` object or `null` after `attribution`), invariant numbers, `-0.0` written as `0`, non-finite doubles as invariant strings, no clock or machine values.

## 5. Facts vs Assumptions

| Statement | Status |
|---|---|
| Both fixtures are 20-record subsets of GBIF-verbatim TreMs records (CC BY 4.0, DOI 10.15468/ocof3v) with the pinned SHA-256 values above. | **Verified** (ATTRIBUTION.json + importer qualification tests + this lane's tests) |
| Without a boundary, Donadea imports as InventoryOnly with `BoundaryMissing` + `UnsupportedLivingSpecies`; Mersch with `BoundaryMissing` only. | **Verified** (tests T1, T4) |
| The 100×100 m polygon used in test T9 is a **constructed test constant with no survey source**. | **Fact about the test**; it says nothing about the real plot boundary. |
| Interpreting `verbatimCoordinates` as plot-local metres (X, Y) is an **unverified inference** that contradicts the verbatim `verbatimCoordinateSystem = 'decimal degrees'`. | **Assumption** (per ATTRIBUTION.json `changesMade`); surfaced as a catalogue caveat. |
| Original publisher IPT archive bytes / `meta.xml` have not been verified. | **Open** (ATTRIBUTION.json `publisherArchiveDownloadStatus`); surfaced as a caveat. |
| Species support categories reflect CCF's current simulator coverage, not ecological suitability. | **Simulation rule**, not an ecological claim. |

## 6. Part 2 (QUEUED behind Playable Milestone 1)

On-screen "Inspect Real Woodland" picker and inspection UI in the 2D simulator, and any wiring into `MultiAreaContinuation`, `AnnualPipeline`, saves or `/multi`. Not started; requires its own task packet.

## 7. Gates (Part 1)

- Pinned SDK `8.0.133` (`Tools/MultiRackSitkaP1Lab/global.json`); `dotnet restore`, `dotnet build --no-restore` (0 warnings, 0 errors), `dotnet run --no-build --no-restore` (all tests pass, exit 0), run twice with identical printed hashes.
- Golden SHA-256 of both built-in views pinned in the test executable.
- Existing Importer V1 test executable still passes unchanged (it compiles the new `Inspection/` files through its `src/Core/WoodlandImport/**` glob).
- `git diff --name-only c82e4e75..HEAD` confined to the four prefixes in §3.

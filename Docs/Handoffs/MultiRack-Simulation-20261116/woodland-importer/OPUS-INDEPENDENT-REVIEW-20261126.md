# Independent Review — Real-World Woodland Importer V1 (Coding Lane 4)

- **Review date (UTC):** 2026-11-26
- **Reviewer:** Claude Opus 5.5 (`claude-opus-5-5`). This is a separate, fresh, read-only thread that did not author any commit on `feat/multi-2d-woodland-importer-v1`, so this is not a self-review.
- **Repository:** `jermur788/CCF`
- **Base (`origin/integration/2d-sim`):** `6d72c30538b1fbe30d3cbcf083b5b368733662b8`
- **Reviewed feature HEAD:** `e51b1e20625e51240a770211f42e1bec70c54c5c` (`feat/multi-2d-woodland-importer-v1`; the live ref matched at review time)
- **Protected `main`:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (live ref matched; not touched)
- **Review branch:** `review/multi-2d-woodland-importer-v1`, branched from the reviewed HEAD. It contains only this file.
- **Scope:** read-only code inspection, an independent rebuild and test run, independent behavioural probes, and a check of the fixtures against live GBIF. No feature code, tests or specs were edited, and nothing was merged.

---

## INTEGRATION DECISION: **NEEDS CHANGES**

| Verdict | Result |
|---|---|
| **Gate G2 (Acceptance)** | **PASS.** Every gate in spec §8 reproduced independently: restore/build 0 warnings and 0 errors, 15/15 tests, exit 0, two-run determinism, and the file fence. |
| **Gate G3 (Measurement Integrity)** | **FAIL.** Independent probes give `CandidateForSimulationIntegration` for stands holding wrong or conflated measurements (R1–R4 below). |
| **Spec Conformance** | **PARTIAL.** The four Option A decisions, the caps, XXE hardening, the species table, provenance refusal and licence honesty all conform. However, §5 bounds checking is wrong for the geographic-boundary path that §5.1 itself specifies (R2), and the multi-plot and measurement-mapping paths break the spec's "no silent substitution / honest provenance" intent (R1, R3). |

The code is clean, isolated and honest where the author tested it. The defects are on untested input paths that the real publisher archive or a real caller will hit. Each required fix is small and local to `IntegrateMarteloscopeReader.cs` and `DarwinCoreArchiveReader.cs`, plus the tests for it.

---

## 1. File fence and isolation — PASS

`git diff --name-only 6d72c305..e51b1e20` lists 23 files, all added (`create mode 100644`, 4,670 insertions, no deletions, no mode changes). Every file sits inside the four allowed prefixes:

- `specs/multi-2d-woodland-importer-v1/**`: 4 files
- `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/**`: 8 files
- `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/**`: 9 files, including 2 fixture ZIPs and `ATTRIBUTION.json`
- `Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/**`: 2 files

There are no edits to `Assets/`, `Packages/`, `ProjectSettings/`, `MultiArea/`, `SpeciesWave2/`, `Phase1/`, `Ui/`, `Core.Tests/`, `build/`, existing `.csproj`/`.sln` files, `Docs/Project/**`, or `main`. The merge-base equals the base SHA. The branch has 4 commits (`cb3152f2`, `5d49a7b9`, `d3635e25`, `e51b1e20`).

**Note (non-blocking, N6):** `src/Core/SitkaP1.Core.csproj` uses default compile globbing, so the importer is compiled into the production `SitkaP1.Core.dll` (verified: `WoodlandImportValidator` is present in the built DLL). It is not referenced or wired anywhere. This is consistent with the fence, but the handoff's statement that the importer is "isolated" refers to wiring, not assembly membership.

## 2. Independent build and test — PASS

Environment: WSL Ubuntu, `/home/jer/.dotnet-8.0.133` first on `PATH`, `DOTNET_ROOT` pinned, `dotnet --version` = `8.0.133`. The review used a fresh `git clone --shared` sparse checkout, detached at `e51b1e20`, with `TMPDIR` outside the repo. Commands were run from `Tools/MultiRackSitkaP1Lab`:

```text
dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj   -> RESTORE_EXIT=0
dotnet build tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
                                                                               -> 0 Warning(s), 0 Error(s), BUILD_EXIT=0
dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 11/11 passed, 0 failed
TOTAL: 15/15 passed, 0 failed                                                  -> RUN_EXIT=0
```

A second run also exited 0 with 15/15. The determinism digests match the handoff exactly: synthetic `dd0f6cb9…b830db` (5,203 B), IE `72cec3ba…601a63` (31,903 B), LU `7fd8b963…2425bf` (29,876 B).

**Additional checks the author could not run** (the handoff §4 item 2 says `SitkaP1.Core.csproj` was not built):

- `dotnet build MultiRackSitkaP1Lab.sln --no-incremental` with the pinned `.p0ref` (sha256 `9a99f781…34b3`, matching the csproj pin): **0 warnings, 0 errors**.
- `dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj --no-build`: **291/291 passed, exit 0**. A first attempt failed 1 test only because my sparse checkout omitted `Assets/ForestPrototype/TreeSpeciesDefinition.cs`. That was environmental; it passed once the file was added.

`dotnet test` and SDK 10 were not used.

## 3. Locked Owner decisions (Option A)

| # | Decision | Result | Evidence |
|---|---|---|---|
| 1 | Unsupported species retained, `InventoryOnly`, no deletion or substitution | **PASS** | Validator lines 209–229 add an `InventoryOnlyBlocker` per unsupported living or unknown-status tree. IE fixture: all 20 trees retained and still `InventoryOnly` with a boundary. No code path removes trees or rewrites species. |
| 2 | Missing measurements marked `FieldProvenance.Missing`, no estimation | **PASS** | No allometric, height–diameter, crown or growth equations exist anywhere in `src/Core/WoodlandImport`. Missing DBH/height produce `MissingRequired*` blockers. (See R3: an unknown *unit* is silently accepted, which is a separate issue.) |
| 3 | Incomplete boundary gives `InventoryOnly`, nothing inferred | **PASS** | `BoundaryMissing` blocker. No hull or bounding-box code. Invalid polygons give `InventoryOnlyBlocker`. |
| 4 | Irish plot first, plus a European oak–beech plot | **PASS** | `IPlus_Donadea_MHF_2` (IE) and `IPlus_Mersch` (LU) are both qualified offline. **I checked all 40 fixture trees against live `api.gbif.org/v1/occurrence/{key}/verbatim`: 40/40 match on scientificName and verbatimCoordinates, and on every MeasurementOrFact (remarks, value) pair.** |

## 4. Technical and honesty audit

| Item | Result | Notes |
|---|---|---|
| Year-0 `Simulated` refused | **PASS** | Every field of every tree is checked (`SimulatedFieldAtYearZero`). Missing/HasValue mismatches are refused as `InconsistentFieldProvenance`. |
| Species mapping table | **PASS** | All 6 target species and their synonyms map as specified. Hybrids, `var.` and non-nominate `subsp.` go to `UnmodelledSpecies`. Author suffixes are matched. No fuzzy matching. Small deviations are listed as N1 and N2. |
| Geometry | **PASS (local) / FAIL (geographic)** | Shoelace area, self-intersection, collinear and degenerate checks and boundary-inclusive ray casting are all correct. The geographic path is broken (R2). |
| Coordinate normalisation | **PASS** | Translation only; distances are preserved. |
| Join / record-loss safety | **FAIL** | Orphan extension rows are dropped silently (R4). Multi-plot archives are conflated (R1). |
| XXE | **PASS** | `DtdProcessing.Prohibit`, `XmlResolver=null`, `MaxCharactersFromEntities=0`, document char cap. Test covered. |
| Caps | **PASS** | Declared and streamed byte caps (per entry and cumulative) are checked before buffering. Row, measurement and living-stem caps are enforced in both reader and validator. Unknown status counts as living. |
| Deterministic ordering | **PASS** | Trees are sorted ordinally by ID; issues are sorted by a total comparator; serializer keys are ordinal; byte identity was reproduced. |
| Licence and source honesty | **PASS with one defect** | CC BY 4.0, DOI, GBIF key and full citation are kept in `ATTRIBUTION.json` and the docs. Publisher ZIP SHA-256 and `meta.xml` are honestly marked `PENDING REAL ARCHIVE DOWNLOAD`, with exact network errors recorded; nothing fabricated was found. However, the API allows a caller to replace the computed archive hash (R5). |

---

## 5. Findings

Each required finding was confirmed with an independent probe: an out-of-repo console project that compiles the feature's `src/Core/WoodlandImport/**` and `SyntheticArchiveBuilder.cs` unchanged and feeds synthetic or real fixture archives to `IntegrateMarteloscopeReader`. The probe output is quoted.

### Required (blocking)

**R1 — A multi-plot archive read without `TargetPlotId` silently merges plots into one admissible stand.**
`IntegrateMarteloscopeReader.Read` lines 84–107 select *every* record when `TargetPlotId` is null. The stand then takes the first plot's name. The comment says "for single-plot archives", but nothing checks that only one plot is present. Each Integrate+ plot has its own local `X , Y` frame, so trees from different plots are overlaid. The real `trems_integrate` archive is multi-plot.
```text
P1 multi-plot (1_PlotA,2_PlotA,3_PlotB,4_PlotB), no TargetPlotId, 100x100 boundary:
   readiness=CandidateForSimulationIntegration trees=4 plot=PlotA issues=[]
   1_PlotA pos=(10,10) beech   3_PlotB pos=(10,10) sessile-oak   <- two trees at the same spot
```
*Fix:* when `TargetPlotId` is null and more than one distinct plot id is present, refuse with something like `ArchiveMultiplePlotsRequireTarget`. Add a test.

**R2 — A geographic boundary is checked against tree coordinates in a different frame.**
`TryCreateFromGeographic` projects vertices to a tangent plane centred on the *polygon centroid*, roughly −L/2..+L/2. Tree positions stay in the plot's own local marteloscope frame (0..L). `LocalCoordinateNormalizer` then shifts both by the boundary minimum, so every tree ends up offset by about L/2 against the boundary. Using the real LU fixture:
```text
geo 100 m square:  readiness=Refused TreeOutOfBounds=18     (same-size LOCAL square: Candidate, 0 OOB)
geo 300 m square:  readiness=CandidateForSimulationIntegration
   tree 33254_IPlus_Mersch: geo-run pos=(233.77,238.50)  local-run pos=(83.77,88.50)  <- displaced 150 m
```
The result is either spurious refusals or admission with every tree misregistered. There is no georeference that links tree-local coordinates to WGS-84; the plot `decimalLatitude/Longitude` has a 500 m uncertainty. The existing test only checks the projected area.
*Fix (smallest):* while trees carry only plot-local coordinates, a geographic boundary must not admit. Either have `RawGeoBoundaryVertices` give an `InventoryOnly` blocker (for example `BoundaryFrameNotRegistered`), or require an explicit, documented tree-frame anchor. Add a test.

**R3 — Measurement-type and unit mapping is not exact (G3).**
1. `IsDiameterRow` uses `measurementType.Contains("diameter")`, and it runs before the crown check. A `Crown diameter` row of 8 m becomes **DBH = 800 cm**, labelled `Observed`.
2. An unrecognised unit (for example `in`) is accepted unconverted as the target unit: **12 in is stored as DBH 12 cm**.
3. When a tree has several rows of the same kind, the last one wins silently: DBH 30 then 95 is stored as **95 cm** with no issue raised.
```text
P4/P5/P7: readiness=CandidateForSimulationIntegration issues=[]
   1_P dbhCm=800 ev={SourceColumn=Crown diameter, RawValue=8, unit=m}
   2_P dbhCm=12  ev={SourceColumn=diameter, RawValue=12, unit=in}
   3_P dbhCm=95  ev={SourceColumn=diameter, RawValue=95, unit=cm}
```
The real TreMs rows (`Tree diameter`/`diameter`/`cm`, `tree height`/`height`/`m`) map correctly. The defect is on any other input.
*Fix:* match exact, documented measurementType/remarks values. Treat an unknown or empty unit on a dimensional measurement as malformed (refuse, or mark the field `Missing` with a blocker); never pass it through. Refuse or block conflicting duplicate measurements per tree and field. Add tests for all three.

**R4 — Orphan extension rows are dropped silently.**
`DarwinCoreArchiveReader` keeps extension rows by `coreid` but only emits rows whose id has a core record. Rows whose `coreid` has no core record vanish without an issue, though they still count toward `TotalMeasurementRowsScanned`. Blank extension `coreid`s are not refused either. A broken join (for example a wrong `<coreid index>`) would therefore make every tree's measurements disappear, and the stand would quietly become `InventoryOnly` with `MissingRequired*` blockers.
```text
P2 orphan rows for 9_GHOST: readiness=CandidateForSimulationIntegration issues=[]  totalMeasurementRowsScanned=5
```
*Fix:* emit a deterministic issue (at least `Warning`; `Refusal` is preferable for record-loss safety) that counts orphan and blank-coreid extension rows. Add a test.

**R5 — `AttributionOverride.ArchiveSha256` replaces the computed archive hash.**
`ResolveAttribution` uses `override.ArchiveSha256 ?? archive.ArchiveSha256`, so a caller-supplied string replaces the SHA-256 of the bytes actually imported:
```text
P3: stand.Attribution.ArchiveSha256=0000-not-the-real-hash  actual=12014493…e33549
```
That allows exactly the fabricated archive claim the qualification rules forbid.
*Fix:* always record the computed hash. If a declared publisher hash is supplied, store it separately and refuse on mismatch.

### Non-blocking

- **N1 — Bare genus and family names map to `UnmodelledSpecies`.** `Quercus` and `Fagaceae` both do. Spec §6 lists higher taxonomic ranks under `UnidentifiedSpecies`. Both categories block admission, so this is a classification nit only.
- **N2 — Two generic common names map to a single species.** `alder` maps to *Alnus glutinosa* and `beech` to *Fagus sylvatica*. This matches the owner-approved spec table, but it is an ecological assumption: grey alder *Alnus incana* also occurs in European data. Consider restricting common-name matching to qualified names (for example "common alder") in a later revision.
- **N3 — Plot id derivation keeps the survey index.** `ExtractPlotId` takes the part of `occurrenceID` after the first `_`. Per SOURCE-DATA-QUALIFICATION §2 (`{id}_{plot}[_{surveyIndex}]`), that includes the survey index, so `IPlus_Donadea_MHF_2` is a plot plus survey, not a plot. This should be documented, or plot and survey parsed separately, when R1 is fixed.
- **N4 — Multi-character delimiters are truncated.** `fieldsTerminatedBy` is cut to its first character. The case-insensitive `FindEntry` fallback could also pick a different entry if two names differ only by case. Neither is reachable with the real archives inspected.
- **N5 — Default attribution uses the archive's dataset name verbatim.** Without an override, `DatasetTitle` comes from `datasetName`, which is `trems dataset` in the real data, and the default citation is abbreviated to "Kraus D, Schuck A, et al.". The full title and citation are correctly kept in `ATTRIBUTION.json` and the docs.
- **N6 — The importer is compiled into `SitkaP1.Core.dll`** by default globbing (see §1). It is acceptable, but integrators should know about it.

---

## 6. Re-review scope

A delta re-review should check R1–R5: each fix plus its new test, and each probe above re-run with the expected refusal or blocker. The re-review should also confirm that 15+N tests pass, the determinism digests either stay the same or have their change explained, the fence still holds, and the full `.sln` plus Core.Tests (291) still pass.

## 7. Model and effort provenance

| Field | Value |
|---|---|
| **Requested** | Claude Opus 5.5, medium effort (step up to high only if medium fails) |
| **Selected** | Claude Opus 5.5 (`claude-opus-5-5`) |
| **Resolved** | `claude-opus-5-5`, as reported by the host session |
| **Executed** | Claude Opus 5.5 (`claude-opus-5-5`) at the host session's configured effort. The reviewer cannot read the numeric effort setting independently. The review finished at the first effort level, so no step-up happened. |

*Independent review only: no feature code was edited and nothing was merged. Unity was not involved. This is not an integration or acceptance claim.*

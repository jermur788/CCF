# Woodland Inspection V1 — Part 1 Implementation Handoff

- **Lane:** Woodland Inspection Part 1 (owner-approved 2026-11-27, Option A locked)
- **Branch:** `feat/multi-2d-woodland-inspection-engine-v1` (review ref `review/multi-2d-woodland-inspection-engine-v1`, same HEAD)
- **Base:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`, confirmed unmoved at task start)
- **Context:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Implementation commit:** `5069913461f10f551d3fb7c566a7536088613825` (this handoff is the following commit)
- **D-030 state:** isolated branch verified; NOT merged to integration/2d-sim; NOT wired into MultiAreaContinuation, AnnualPipeline, saves, or /multi.
- **Model provenance:** Requested Gemini 4 Neon (high) — unavailable: the OpenCode Zen route failed "Reconnect OpenCode Console" twice and is now disabled by policy; the Antigravity route lists no models in T3. Selected/Executed: Claude Opus 5.5 (high), owner-selected packet fallback, 2026-11-27.
- **Review status:** not reviewed. The builder did not review its own work; independent review is to run on a separate fresh thread.
- **Unity:** not involved, not run (pure .NET 8 lane).

## 1. What was built

All under `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/Inspection/` (namespace `SitkaP1.Core.WoodlandImport.Inspection`):

| File | Role |
|---|---|
| `WoodlandInspectionContracts.cs` | Immutable view records; Missing values are real `null`. |
| `BuiltInWoodlandCatalogue.cs` | Fixed catalogue `[0] IPlus_Donadea_MHF_2 (IE)`, `[1] IPlus_Mersch (LU)`; `Load(Func<string, byte[]>)` reads ATTRIBUTION.json, cross-checks pinned SHA-256 and counts, imports each ZIP through the existing `IntegrateMarteloscopeReader` with `TargetPlotId`, `AttributionOverride` and `DeclaredArchiveSha256`; never passes a boundary. |
| `WoodlandInspectionProjector.cs` | `Project(WoodlandImportResult)` → `WoodlandInspectionView`. |
| `BlockerExplanations.cs` | Plain-English title/explanation per known importer code; message fallback. |
| `WoodlandInspectionSerializer.cs` | Canonical JSON (`SerializeUtf8`, `SerializeJson`, `ComputeSha256Hex`). |

Test executable `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj` (Exe; compiles only `../../../src/Core/WoodlandImport/**/*.cs` plus its own files; no Unity/P0/package reference). Spec quartet in `specs/multi-2d-woodland-inspection-v1/`.

## 2. Exact commands and output

SDK: `dotnet` resolves to `/usr/lib/dotnet/dotnet`, version 8.0.133, pinned by `Tools/MultiRackSitkaP1Lab/global.json`. `~/.dotnet-8.0.133` does **not** exist on this host. `dotnet test`, SDK 10, `build/2d-gate.sh` and browser suites were not used.

From `Tools/MultiRackSitkaP1Lab`, after deleting the test project's `bin/` and `obj/`:

```text
$ dotnet --version
8.0.133
$ dotnet restore tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj
rc=0
$ dotnet build tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)
rc=0
$ dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore   (run 1)
PASS [catalogue] T1 order and metadata: exactly Donadea then Mersch, offline, 20 trees, IE/LU, InventoryOnly, CC BY 4.0, pinned SHA, full citation
PASS [catalogue] T2 tampered archive bytes (one byte flipped in memory) are Refused and cannot advance time
PASS [catalogue] T2b ATTRIBUTION.json disagreeing with a pinned SHA-256 is rejected before import
  [checklist] ie-donadea codes=BoundaryMissing,UnsupportedLivingSpecies
  [checklist] ie-donadea unmodelled living species=Crataegus monogyna | Fraxinus excelsior | Picea abies | Quercus robur | Sorbus aucuparia | Ulmus glabra
PASS [projection] T4 blocker checklists: Donadea has BoundaryMissing + UnsupportedLivingSpecies naming unmodelled species; Mersch is exactly [BoundaryMissing]
PASS [projection] T7 provenance preservation: every projected field equals its source record; no fabricated height/age/crown; no Simulated provenance in JSON
PASS [projection] T8a Year-0 boundary on both fixtures: observed year 0, future starts at year 1, no simulated future, zero simulated values
PASS [projection] T9 constructed-test-boundary Mersch (100x100 m CONSTRUCTED TEST CONSTANT, no survey source) is Candidate and CanAdvanceTime
  [microhabitats] IPlus_Donadea_MHF_2 count=14
  [microhabitats] IPlus_Mersch count=26
PASS [projection] T10a real-fixture species breakdown and microhabitats match the source records
  [determinism] ie-donadea-inspection-sha256=2649a7128a8463b78aed89c7a9aa1f28486278523f073935614bcf481ce8661f (25619 bytes)
  [determinism] lu-mersch-inspection-sha256=00d9f7603118262ef4c03c9991a5cd81447275ecdfd41b8dd2f5204ff41ee589 (25162 bytes)
PASS [determinism] T3 each built-in view is byte-identical across repeated projection and independent catalogue loads, and matches golden SHA-256
PASS [determinism] T3b serializer writes -0.0 as 0 and invariant numbers
  [refused] codes=ArchiveMalformedZip
PASS [synthetic] T5 garbage archive through the real reader: Refused view has no trees, refusal blockers listed, cannot advance
PASS [synthetic] T6 missing height/age/crown and missing boundary stay null + Missing with a MissingRequiredHeight blocker
PASS [synthetic] T8b a Simulated field at Year 0 can never advance time (validator Refused, and defensive guard on a hand-built Candidate result)
PASS [synthetic] T10b species breakdown covers all four categories; synthetic trem: microhabitats projected; collections read-only
PASS [synthetic] T10c unknown codes fall back to the importer message; Refusal items sort before blockers
PASS [hygiene] inspection sources use no file, clock, network, Unity or simulation-wiring APIs
---
SUITE catalogue: 3/3 passed, 0 failed
SUITE determinism: 2/2 passed, 0 failed
SUITE hygiene: 1/1 passed, 0 failed
SUITE projection: 5/5 passed, 0 failed
SUITE synthetic: 5/5 passed, 0 failed
TOTAL: 16/16 passed, 0 failed
rc=0
$ dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore   (run 2)
  [determinism] ie-donadea-inspection-sha256=2649a7128a8463b78aed89c7a9aa1f28486278523f073935614bcf481ce8661f (25619 bytes)
  [determinism] lu-mersch-inspection-sha256=00d9f7603118262ef4c03c9991a5cd81447275ecdfd41b8dd2f5204ff41ee589 (25162 bytes)
TOTAL: 16/16 passed, 0 failed
rc=0
```

(Run 2 printed the same 16 PASS lines; it is shortened here to the hashes and total.) After commit `50699134` a third clean rebuild and run printed the same two hashes, `0 Warning(s) 0 Error(s)`, `TOTAL: 16/16 passed, 0 failed`.

**Pass/fail:** 16/16 passed, 0 failed (catalogue 3, projection 5, determinism 2, synthetic 5, hygiene 1).

**Regression, closed Importer V1 suite (run read-only).** It compiles the new `Inspection/` files through its `src/Core/WoodlandImport/**` glob, and its `#644 B-01` hygiene test scans them:

```text
$ dotnet restore/build/run tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj (--no-restore / --no-build --no-restore)
    0 Warning(s)
    0 Error(s)
PASS [synthetic] hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 25/25 passed, 0 failed
TOTAL: 29/29 passed, 0 failed
```

## 3. Golden inspection JSON hashes (pinned in `WoodlandInspectionTests.cs`)

| Stand (as shipped: no boundary) | Readiness | SHA-256 of canonical view JSON | Bytes |
|---|---|---|---|
| `IPlus_Donadea_MHF_2` | InventoryOnly | `2649a7128a8463b78aed89c7a9aa1f28486278523f073935614bcf481ce8661f` | 25619 |
| `IPlus_Mersch` | InventoryOnly | `00d9f7603118262ef4c03c9991a5cd81447275ecdfd41b8dd2f5204ff41ee589` | 25162 |

These are comparable only for the same view schema, fixture bytes and SDK 8.0.133 `Utf8JsonWriter` behaviour.

## 4. Diff vs base and fence

`git diff --stat c82e4e759c2ceef1c13ddb6a033e034097aef77e..5069913461f10f551d3fb7c566a7536088613825`:

```text
 .../Inspection/BlockerExplanations.cs              | 106 ++++
 .../Inspection/BuiltInWoodlandCatalogue.cs         | 250 +++++++++
 .../Inspection/WoodlandInspectionContracts.cs      | 166 ++++++
 .../Inspection/WoodlandInspectionProjector.cs      | 309 +++++++++++
 .../Inspection/WoodlandInspectionSerializer.cs     | 325 +++++++++++
 .../tests/Core/WoodlandInspection/Program.cs       |   4 +
 .../SitkaP1.WoodlandInspection.Tests.csproj        |  13 +
 .../tests/Core/WoodlandInspection/TestFixtures.cs  | 133 +++++
 .../tests/Core/WoodlandInspection/TestRunner.cs    |  73 +++
 .../WoodlandInspection/WoodlandInspectionTests.cs  | 593 +++++++++++++++++++++
 .../checklists/requirements.md                     |  19 +
 specs/multi-2d-woodland-inspection-v1/plan.md      |  63 +++
 specs/multi-2d-woodland-inspection-v1/spec.md      |  88 +++
 specs/multi-2d-woodland-inspection-v1/tasks.md     |  30 ++
 14 files changed, 2172 insertions(+)
```

The handoff commit adds this file only. **Fence:** every changed path falls under the four allowed prefixes; only new files were added. No existing file changed, including `src/Core/WoodlandImport/*.cs`, `tests/Core/WoodlandImport/**` (fixtures used read-only), csproj/sln/props/global.json, `.gitignore`, `Docs/Project/**`, Unity folders, `src/Ui/**`, `src/Core/MultiArea/**`, `tests/Core.Tests/**` and `build/**`. `bin/` and `obj/` are ignored by `Tools/MultiRackSitkaP1Lab/.gitignore` and were not committed.

## 5. Deviations from the packet

1. **`LoadFromDirectory` is test-side.** The packet asked for a production `BuiltInWoodlandCatalogue.LoadFromDirectory(string)`. Any path-based file read (`File.ReadAllBytes`, `File.Open*`, `FileStream`) inside `src/Core/WoodlandImport/**` breaks P1 `#644 B-01`. It would also fail the closed Importer V1 hygiene test, which scans that directory recursively and may not be edited. So production exposes only `Load(Func<string, byte[]>)`, and the directory wrapper is `TestFixtures.LoadFromDirectory` in the test project. If the owner wants a production directory loader, it needs a ruling on B-01 scope first.
2. **Tamper test uses two offsets.** Flipping byte 10 (local-header timestamp) leaves the ZIP readable, and the importer refuses it with `ArchiveSha256Mismatch`, which proves the pinned-hash path. Flipping byte 1000 corrupts the content, and it is refused by the archive reader before the hash check. Both give `Refused` and `CanAdvanceTime = false`.

## 6. Residual risks and open items

- **Coordinate frame unverified.** Treating `verbatimCoordinates` as plot-local metres is an unverified inference (ATTRIBUTION.json `changesMade`). The view shows the stand's translation-normalised positions plus origin and rule. The raw source text stays in each field's evidence. The catalogue surfaces this as a caveat.
- **Publisher archive unverified.** The original IPT archive and `meta.xml` were never downloaded (`publisherArchiveDownloadStatus`). This is surfaced as a caveat.
- **No surveyed boundary exists** for either plot, so both stay InventoryOnly and `CanAdvanceTime` is false. T9's 100×100 m polygon is a constructed test constant only.
- **Explanation wording** is builder-written plain English, not reviewed by the owner. Unknown future importer codes fall back to the raw importer message.
- **Golden hashes** change if the view schema, any blocker wording or the fixtures change. That is intended, but every wording edit needs a re-pin.
- **Licence URL** is carried as written in ATTRIBUTION.json (`http://creativecommons.org/licenses/by/4.0/legalcode`).
- **Parked importer items NF-07/NF-08/NF-09** stay deferred. Their definitions are not in this Git tree, and the owner decision record `2D_Simulator_Strategic_Roadmap.md v1.17` is external.
- **Part 2 is not started:** UI picker/inspection screen and all wiring are queued behind Playable Milestone 1.

## 7. How to reproduce

From `Tools/MultiRackSitkaP1Lab` on this host:

```bash
dotnet --version   # 8.0.133
dotnet restore tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj
dotnet build tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-restore
dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore
```

Expected: `0 Warning(s)`, `0 Error(s)`, `TOTAL: 16/16 passed, 0 failed`, exit code 0, and the two hashes in §3.

---

## Repair R1 (2026-11-27)

- **Trigger:** independent review by GPT-6.2 Sol, `INTEGRATION DECISION: NEEDS CHANGES` on `24250caf64fadb81bbe82e9ea684b760be3bff3f` (WI-01..WI-06).
- **Repair commit:** `1889741a9ccfb4118b17f410e21124af5b98a212` (this handoff update is the following commit). Base unchanged: `c82e4e759c2ceef1c13ddb6a033e034097aef77e`. There was no rebase or merge; `integration/2d-sim` (now `fa69d6d3`) was not merged in.
- **Writer:** Claude Opus 5.5 (the original builder, repairing its own candidate). There has been no self-review. A non-author delta re-review is required.
- **D-030 state:** isolated branch verified; NOT merged to integration/2d-sim; NOT wired into MultiAreaContinuation, AnnualPipeline, saves, or /multi.
- **Unity:** not involved, not run.

### R1.1 Finding → change → test

| Finding | Change (all under `src/Core/WoodlandImport/Inspection/`) | Test |
|---|---|---|
| **WI-01 Major**: a hand-built Candidate leaked a Simulated value into `ObservedYear0` | `WoodlandInspectionProjector.Project`: if any tree field is `Simulated`, the whole stand is withheld. There is no identity, attribution, trees, origin, boundary, species, microhabitats or observation dates. Readiness is reported as `Refused`, and the projector adds one `SimulatedFieldAtYearZero` Refusal issue per field the result does not already list. The checklist item names the tree ids but no values or species. `SimulatedValueCount` still reports the count found. | **T8b** (updated): height `123.45` with `Simulated` provenance gives `Refused`, 0 trees, one renderable Refusal item for `t1`. The value `123.45`, `"provenance":"Simulated"` and the evidence note are all absent from the JSON, and the value is absent from the view. A result that already lists the issue is not listed twice. |
| **WI-02 Major**: built-in JSON lost the CC BY 4.0 `changesMade` disclosure and the coordinate caveat | New `InspectionDisclosures {ModificationsDisclosure, CoordinateInterpretationCaveat}` record. New overload `Project(result, disclosures)`. `BuiltInWoodlandEntry.Disclosures` and `BuiltInWoodlandEntry.InspectionView` are projected with the disclosures. The serializer writes a top-level `disclosures` object (or `null`) after `attribution`. `Load` throws `InvalidDataException` when `changesMade` lacks the `Note:` caveat on `verbatimCoordinates`. No provenance enum is relabelled, and importer fields and raw evidence are unchanged. | **T1b** (new): view and JSON carry `changesMade` verbatim and the `Note:` sentence, which names `verbatimCoordinates` and "unverified inference". Position provenance and raw value equal the importer's. `Project(result, disclosures)` reproduces the view byte for byte. A plain `Project(result)` has `null` disclosures. ATTRIBUTION.json without the Note is rejected. |
| **WI-03 Major**: `ICollection.SyncRoot` exposed mutable backing lists | Every view collection is an `ImmutableList<T>` (in the BCL shared framework, so no package reference). `Definitions`, `Load`'s return, `Caveats` and `ObservationDates` are immutable too. The entry's `ImportResult` is an immutable snapshot taken at load (`WoodlandImportResultCopier`): `Issues`, `Stand.Trees` and `Boundary.VerticesLocalM` become `ImmutableList`, and each tree's `ExtraAttributes` becomes an ordinal `ImmutableSortedDictionary`, preserving the importer's ordinal order. The closed importer types are unchanged. | **T10e** (new): the probe first proves it detects `ReadOnlyCollection` (List and array) and `ReadOnlyDictionary` backing stores. It then attempts mutation through `SyncRoot` on the catalogue, entries, caveats, observation dates, every view collection (built-in and synthetic), and the nested issues, trees and attribute dictionaries. All attempts fail. Catalogue and entry order, view hashes and re-projected goldens are unchanged. Mutating a caller's own input list after projection does not alter the view. |
| **WI-04 Minor**: projector trusted `result.Stand` on Refused | `Refused ⇒ stand = null` at the public boundary, whatever the result carries. | **T8c** (new): a hand-built Refused result carrying the Donadea stand gives no identity, attribution, trees, origin, boundary, microhabitats, species or dates. The JSON contains no plot name, stand id, citation, species, tree id or tree field. |
| **WI-05 Minor**: explanations narrowed or overstated checks; `ArchiveCorruptEntry` had no text | `BlockerExplanations` now covers all 57 blocker and refusal codes in `src/Core/WoodlandImport/*.cs`. `BoundaryMissing` now says "No boundary supplied" / "No boundary polygon was supplied…". `BoundaryZeroArea` covers collinear, zero, degenerate and non-finite area. `MissingVitalStatus` covers missing or unrecognised codes. `BoundaryNonFiniteVertex` covers out-of-range WGS-84. `BoundarySelfIntersecting` covers edges that touch. `ArchiveSha256Mismatch` covers conflicting declared values. `UnidentifiedLivingSpecies` covers genus- or family-only names. | **T10d** (new): scans the closed importer sources for every `InventoryOnlyBlocker`/`Refusal` code (pinned count 57, which guards the scan) and asserts each has text and a non-code title. **T4** asserts the new BoundaryMissing title and opening. |
| **WI-06 Nit**: T2 offset 1000 asserted only a generic refusal | — | **T2** asserts `ArchiveCorruptEntry` at offset 1000 (printed `[tamper] offset 1000 codes=ArchiveCorruptEntry`), with a plain-English title and no raw decompression message. |

Accepted deviations kept: (a) `LoadFromDirectory` stays test-side; (b) the two-offset tamper test.

**Mutants (run in a `/tmp` copy, not committed):** each new guard was reverted in turn and the suite went red:

- WI-01 withholding removed: T8b fails.
- WI-04 Refused rule removed: T8c fails.
- WI-01 synthesized refusal removed: T8b fails.
- Disclosures dropped: T1b, T3 and T10e fail.
- Snapshot trees reverted to `ToList().AsReadOnly()`: T10e fails ("nested trees: no mutable backing collection").

**Docs:** `tasks.md` and `checklists/requirements.md` rows changed only where R1 changes them. `spec.md` §4.1–§4.3 got minimal R1-tagged edits because three statements there were otherwise false: "read-only wrapper" collections, the "full `WoodlandImportResult`", and the Refused/Simulated projection rule. `plan.md` is unchanged.

### R1.2 Golden hashes (re-pinned)

The goldens now hash each built-in `BuiltInWoodlandEntry.InspectionView`, the view a UI receives, with disclosures.

| Stand | Old (24250caf) | New (R1) | Bytes old → new |
|---|---|---|---|
| `IPlus_Donadea_MHF_2` | `2649a7128a8463b78aed89c7a9aa1f28486278523f073935614bcf481ce8661f` | `b53c6339242f6a0abc22a5292a042920fd0dee17d595f3a567782abd99babeb6` | 25619 → 27093 |
| `IPlus_Mersch` | `00d9f7603118262ef4c03c9991a5cd81447275ecdfd41b8dd2f5204ff41ee589` | `e01bbfada73ab2260300ff4f5e092b1d537e76ac96a72ecab3df1d9ef1d0b9a4` | 25162 → 26636 |

**Reason:** the JSON gained the `disclosures` object (WI-02), and the BoundaryMissing title and explanation were reworded (WI-05). Proof: a throwaway `/tmp` harness built the 24250caf sources and the R1 sources and dumped `Project(entry.ImportResult)` for both stands. The 24250caf build reproduced the old goldens exactly. A structural JSON diff of old against new plain projections showed only three differences per stand: `blockerChecklist[0].title`, `blockerChecklist[0].explanation`, and the added `disclosures: null`. The built-in view then fills `disclosures` with the two strings.

### R1.3 Exact commands and output (committed tree `1889741a`, after deleting both test projects' `bin/` and `obj/`)

```text
$ dotnet --version
8.0.133
$ dotnet restore tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj
rc=0
$ dotnet build tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)
rc=0
$ dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore   (run 1, tail)
  [determinism] ie-donadea-inspection-sha256=b53c6339242f6a0abc22a5292a042920fd0dee17d595f3a567782abd99babeb6 (27093 bytes)
  [determinism] lu-mersch-inspection-sha256=e01bbfada73ab2260300ff4f5e092b1d537e76ac96a72ecab3df1d9ef1d0b9a4 (26636 bytes)
PASS [determinism] T3 each built-in view is byte-identical across repeated projection and independent catalogue loads, and matches golden SHA-256
PASS [determinism] T3b serializer writes -0.0 as 0 and invariant numbers
  [refused] codes=ArchiveMalformedZip
PASS [synthetic] T5 garbage archive through the real reader: Refused view has no trees, refusal blockers listed, cannot advance
PASS [synthetic] T6 missing height/age/crown and missing boundary stay null + Missing with a MissingRequiredHeight blocker
PASS [synthetic] T8b a Simulated field at Year 0 never reaches the view (validator Refused; a hand-built Candidate is withheld with a renderable refusal)
PASS [synthetic] T8c a Refused result that still carries a stand projects no stand data
PASS [synthetic] T10b species breakdown covers all four categories; synthetic trem: microhabitats projected; collections read-only
PASS [synthetic] T10c unknown codes fall back to the importer message; Refusal items sort before blockers
  [codes] importer emits 57 distinct blocker/refusal codes
PASS [synthetic] T10d every blocker and refusal code the closed importer can emit has plain-English text
PASS [synthetic] T10e no backing collection reachable through ICollection.SyncRoot can reorder the catalogue or alter a view or its hash
PASS [hygiene] inspection sources use no file, clock, network, Unity or simulation-wiring APIs
---
SUITE catalogue: 4/4 passed, 0 failed
SUITE determinism: 2/2 passed, 0 failed
SUITE hygiene: 1/1 passed, 0 failed
SUITE projection: 5/5 passed, 0 failed
SUITE synthetic: 8/8 passed, 0 failed
TOTAL: 20/20 passed, 0 failed
rc=0
$ dotnet run ... (run 2)
rc=0   — output byte-identical to run 1 (cmp), same two hashes, TOTAL: 20/20 passed, 0 failed
```

Run 1 also printed `[tamper] offset 1000 codes=ArchiveCorruptEntry` and PASS for T1, T2, T2b, T1b, T4, T7, T8a, T9 and T10a.

Closed Importer V1 suite, run read-only. It compiles and hygiene-scans the R1 `Inspection/` files:

```text
$ dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj            rc=0
$ dotnet build tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)
$ dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
PASS [synthetic] hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 25/25 passed, 0 failed
TOTAL: 29/29 passed, 0 failed
rc=0
```

### R1.4 Diff and fence

`git diff --stat 24250caf..1889741a`:

```text
 .../Inspection/BlockerExplanations.cs              | 168 ++++++++--
 .../Inspection/BuiltInWoodlandCatalogue.cs         | 110 +++++--
 .../Inspection/WoodlandInspectionContracts.cs      |  57 +++-
 .../Inspection/WoodlandInspectionProjector.cs      | 161 ++++++----
 .../Inspection/WoodlandInspectionSerializer.cs     |  17 +
 .../WoodlandInspection/WoodlandInspectionTests.cs  | 341 ++++++++++++++++++++-
 .../checklists/requirements.md                     |  12 +-
 specs/multi-2d-woodland-inspection-v1/spec.md      |  15 +-
 specs/multi-2d-woodland-inspection-v1/tasks.md     |  14 +-
 9 files changed, 738 insertions(+), 157 deletions(-)
```

The handoff commit adds only this section. No file was added; the five `Inspection/` sources still number five, as the hygiene test pins. No closed importer file, csproj, sln, props, global.json or `Docs/Project/**` file changed.

### R1.5 Residual notes

- **Readiness downgrade.** When the projector withholds a contaminated stand, the view reports `Readiness = Refused` even though the hand-built result said Candidate. Without the downgrade, the view would pair a Refusal checklist item with a Candidate readiness. The importer's own result is untouched.
- **Importer static.** `ImportedTreeRecord.EmptyAttributes` in the closed importer is a shared `ReadOnlyDictionary` over a `Dictionary`, so its `SyncRoot` is mutable process-wide. It is no longer reachable through the inspection API, because snapshots replace it with fresh immutable dictionaries. The importer itself is closed and was not changed.
- **Reflection** can still mutate any .NET object; the guarantee covers the public API surface.
- **Wording** remains builder-written plain English and has not been reviewed by the owner.

## Repair R2 (2026-11-27)

- **Trigger:** independent delta review by GPT-6.2 Sol, `INTEGRATION DECISION: NEEDS CHANGES` on `00fb77bed6fd5a4ec2730d9b0535298145a38a76` (WI-07, WI-08, WI-09). WI-03..WI-06 were resolved and are not regressed. The reviewer accepted the view-level Refused downgrade, the R1 re-pins, the EmptyAttributes limitation and the 57-code scan in T10d.
- **Repair commit:** `c424302b2ace81b64a99808e2debae4714668c9b`. This handoff update is the following commit. The base is unchanged at `c82e4e759c2ceef1c13ddb6a033e034097aef77e`. There was no rebase or merge, and `integration/2d-sim` was not merged in.
- **Writer:** Claude Opus 5.5, the original builder, repairing its own candidate. There has been no self-review, so a non-author delta re-review is required.
- **D-030 state:** isolated branch verified; NOT merged to integration/2d-sim; NOT wired into MultiAreaContinuation, AnnualPipeline, saves, or /multi.
- **Unity:** not involved, not run.

### R2.1 Finding → change → test

| Finding | Change | Test |
|---|---|---|
| **WI-07 Major** (completes WI-01): a same-code `Warning` for the same tree and field suppressed the synthesized refusal. The result was Refused with 0 trees, 0 checklist items and 1 note. | `WoodlandInspectionProjector.AddSimulatedFieldRefusals`: an issue counts as "already listed" only when it has code `SimulatedFieldAtYearZero` **and** severity `Refusal`. A same-code Warning or Info stays a note. A same-code InventoryOnlyBlocker joins the code group, and the group severity rises to Refusal. | **T8b** (extended): the `Warning`, `Info` and `InventoryOnlyBlocker` variants with the same code, tree `t1` and field `HeightM` each give Refused, 0 trees, exactly one checklist item `SimulatedFieldAtYearZero` with severity Refusal naming `t1`, and 1/1/0 notes respectively. The existing case where a matching Refusal is reused, not duplicated, still passes. |
| **WI-08 Major** (completes WI-02): the loader accepted any `Note:` that merely mentioned `verbatimCoordinates`. | `BuiltInWoodlandCatalogue.VettedCoordinateCaveat` pins the vetted caveat text verbatim. `Load` throws `InvalidDataException` unless the `Note:` sentence of `changesMade` equals it exactly. A missing `Note:` is still rejected, with the original message. The caveat that reaches the view is the same string as before. | **T1b** (extended): asserts that the fixture note equals the pinned constant. Then, for each of five weakened notes, it rewrites `changesMade` through `JsonNode` and asserts that `Load` throws. The five notes are: `"Note: verbatimCoordinates were used."`; a note naming plot-local metres without the qualification; "an unverified inference" → "an inference"; "is an unverified inference" → "is unverified"; and the vetted note plus trailing text. |
| **WI-09 Minor**: the `ArchiveSha256Mismatch` explanation implied damage even when the cause was conflicting declared fingerprints (`IntegrateMarteloscopeReader.cs:300`). | `BlockerExplanations`: the text now states the two causes separately. The first is bytes that do not match the declared fingerprint, which may mean damage or alteration. The second is import metadata that declared two different fingerprints, leaving no single fingerprint to check against, with no damage implied. | **T10d** (extended): asserts that the explanation contains the byte-mismatch clause, the "declared two different fingerprints" clause and the "no single fingerprint to check the file against" clause. |

**Mutants** (scratch edits in the worktree, each reverted immediately and confirmed by `git diff`). With each new guard removed, the suite went red:

- WI-07: dedup on code only (the `&& i.Severity == IssueSeverity.Refusal` dropped) → `FAIL [synthetic] T8b … Assertion failed (Warning: one checklist item): expected '1', got '0'.`, `TOTAL: 19/20`.
- WI-08: the equality check replaced by the R1 `Contains("verbatimCoordinates")` → `FAIL [catalogue] T1b … InvalidDataException for weakened caveat 'Note: verbatimCoordinates were used.'`, `TOTAL: 19/20`.
- WI-09: the R1 `BlockerExplanations.cs` restored → `FAIL [synthetic] T10d … ArchiveSha256Mismatch explains conflicting declarations`, `TOTAL: 19/20`.

No production file was added. `Inspection/` still holds five sources, so the file count the hygiene test pins is unaffected.

### R2.2 Spec ratification

R1 spec.md §4.1–4.3 corrections ratified by owner/Manager 2026-11-27 after delta-review ruling (ii). The edits are kept unchanged. R2 makes no statement in `spec.md` untrue, so `spec.md` is not edited in R2. §4.1 still says `Load` throws when `changesMade` has no `Note:` caveat about `verbatimCoordinates`. That remains true; R2 only narrows which notes are accepted. `tasks.md` records the ratification and the R2 rows. `checklists/requirements.md` WN-REQ-02, -07 and -08 cite R2.

### R2.3 Golden hashes: unchanged

| Stand | R1 (`00fb77be`) | R2 |
|---|---|---|
| `IPlus_Donadea_MHF_2` | `b53c6339242f6a0abc22a5292a042920fd0dee17d595f3a567782abd99babeb6` (27093 bytes) | unchanged |
| `IPlus_Mersch` | `e01bbfada73ab2260300ff4f5e092b1d537e76ac96a72ecab3df1d9ef1d0b9a4` (26636 bytes) | unchanged |

WI-09 changes no serialized output, because `ArchiveSha256Mismatch` is not in either built-in view. WI-08 accepts the same caveat string as before. WI-07 affects only views that hold a Simulated field, which built-in stands do not.

### R2.4 Exact commands and output (committed tree `c424302b`, after deleting both test projects' `bin/` and `obj/`)

```text
$ dotnet --version
8.0.133
$ dotnet restore tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj
rc=0
$ dotnet build tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)
$ dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore   (run 1, excerpt + tail)
  [tamper] offset 1000 codes=ArchiveCorruptEntry
  [determinism] ie-donadea-inspection-sha256=b53c6339242f6a0abc22a5292a042920fd0dee17d595f3a567782abd99babeb6 (27093 bytes)
  [determinism] lu-mersch-inspection-sha256=e01bbfada73ab2260300ff4f5e092b1d537e76ac96a72ecab3df1d9ef1d0b9a4 (26636 bytes)
PASS [determinism] T3 each built-in view is byte-identical across repeated projection and independent catalogue loads, and matches golden SHA-256
...
PASS [synthetic] T8b a Simulated field at Year 0 never reaches the view (validator Refused; a hand-built Candidate is withheld with a renderable refusal)
PASS [synthetic] T8c a Refused result that still carries a stand projects no stand data
PASS [synthetic] T10b species breakdown covers all four categories; synthetic trem: microhabitats projected; collections read-only
PASS [synthetic] T10c unknown codes fall back to the importer message; Refusal items sort before blockers
  [codes] importer emits 57 distinct blocker/refusal codes
PASS [synthetic] T10d every blocker and refusal code the closed importer can emit has plain-English text
PASS [synthetic] T10e no backing collection reachable through ICollection.SyncRoot can reorder the catalogue or alter a view or its hash
PASS [hygiene] inspection sources use no file, clock, network, Unity or simulation-wiring APIs
---
SUITE catalogue: 4/4 passed, 0 failed
SUITE determinism: 2/2 passed, 0 failed
SUITE hygiene: 1/1 passed, 0 failed
SUITE projection: 5/5 passed, 0 failed
SUITE synthetic: 8/8 passed, 0 failed
TOTAL: 20/20 passed, 0 failed
rc=0
$ dotnet run ... (run 2)
rc=0   (output byte-identical to run 1 by cmp: same two hashes, TOTAL: 20/20 passed, 0 failed)
```

The closed Importer V1 suite was run read-only. It compiles and hygiene-scans the R2 `Inspection/` files:

```text
$ dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj            rc=0
$ dotnet build tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)
$ dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
PASS [synthetic] hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 25/25 passed, 0 failed
TOTAL: 29/29 passed, 0 failed
rc=0
```

### R2.5 Diff and fence

`git diff --stat 00fb77be..c424302b`:

```text
 .../Inspection/BlockerExplanations.cs              |  3 +-
 .../Inspection/BuiltInWoodlandCatalogue.cs         | 20 ++++++-
 .../Inspection/WoodlandInspectionProjector.cs      | 10 +++-
 .../WoodlandInspection/WoodlandInspectionTests.cs  | 65 ++++++++++++++++++++++
 .../checklists/requirements.md                     |  6 +-
 specs/multi-2d-woodland-inspection-v1/tasks.md     | 12 ++--
 6 files changed, 102 insertions(+), 14 deletions(-)
```

The handoff commit adds only this section to this file. No closed importer file, csproj, sln, props, global.json, `spec.md`, `plan.md` or `Docs/Project/**` file changed.

### R2.6 Residual notes

- **The vetted caveat is now exact-match.** Any future rewording of the ATTRIBUTION.json `Note:`, even an accurate one, needs a matching change to `VettedCoordinateCaveat`. This coupling is deliberate.
- The R1 residual notes (readiness downgrade, the importer's `EmptyAttributes` static, reflection, unreviewed wording) still apply.

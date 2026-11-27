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

# Woodland Importer V1 — Independent Review (Coding Lane 4)

- **Review date (UTC):** 2026-11-26
- **Reviewer:** Claude Opus 5.5 (Claude Code), independent provider/fresh context; did not author the candidate
- **Candidate reviewed:** `e51b1e20625e51240a770211f42e1bec70c54c5c` (`origin/feat/multi-2d-woodland-importer-v1`, confirmed via `git ls-remote`)
- **Base:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`, confirmed via `git ls-remote`)
- **Review branch:** `review/multi-2d-woodland-importer-v1` (cut from the candidate; this file is its only change)
- **Role limits honoured:** I made no edits to source, tests, spec or fixtures. All probes and mutations ran on throw-away copies under WSL `/tmp`. Protected listeners were not started.

---

## INTEGRATION DECISION

**INTEGRATION DECISION: NEEDS CHANGES**

There are two blocking record-loss defects (F-01, F-02). Together with the major findings below, they mean the candidate does not yet meet the brief's priority-1 rule: "account for every input row; silent loss is blocking". The gates are green, but the suite has no tests for the paths where these defects live (F-16).

---

## 1. Gates I ran myself (WSL Ubuntu, pinned SDK)

I used the approved commands without modification. I ran them twice: (A) in the review worktree, which is a Windows checkout with `core.autocrlf=true`, so files have CRLF line endings; and (B) on a clean `git archive e51b1e20` export with LF line endings under `/tmp/wr-lf`.

```bash
cd Tools/MultiRackSitkaP1Lab && export PATH="$HOME/.dotnet-8.0.133:$PATH"
dotnet --version
dotnet restore tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj
dotnet build tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-restore
dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore
```

Captured output from run A (run B was identical apart from paths):

```text
== dotnet --version
8.0.133
== restore
  Determining projects to restore...
  Restored .../Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj (in 225 ms).
RESTORE_EXIT=0
== build
MSBuild version 17.8.60+293f66545 for .NET
  SitkaP1.WoodlandImport.Tests -> .../tests/Core/WoodlandImport/bin/Debug/net8.0/SitkaP1.Core.WoodlandImport.Tests.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
BUILD_EXIT=0
== run
PASS [synthetic] positive: ... (x11 synthetic, all PASS)
  [determinism] synthetic-canonical-sha256=7be5a873781fe273ac3ea70f10249794c49039758a7d84ca1a91617a291f6c19 (5203 bytes)
PASS [qualified] ... (x4 qualified, all PASS)
  [determinism] ie-donadea-canonical-sha256=72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63 (31903 bytes)
  [determinism] lu-mersch-canonical-sha256=7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf (29876 bytes)
---
SUITE qualified: 4/4 passed, 0 failed
SUITE synthetic: 11/11 passed, 0 failed
TOTAL: 15/15 passed, 0 failed
RUN_EXIT=0
```

| Gate | Result |
|---|---|
| `dotnet --version` = `8.0.133` | PASS |
| restore: 0 warnings / 0 errors | PASS |
| build `--no-restore`: 0 warnings / 0 errors (`TreatWarningsAsErrors=true`) | PASS |
| run `--no-build --no-restore`: exit 0 | PASS: **synthetic 11/11, qualified 4/4, total 15/15** |
| Extra: rerun under `TZ=UTC`, `Europe/Dublin`, `America/New_York`, `Asia/Kolkata` | 15/15 each time, with the same three hashes |

The implementer's approved commands were the ones run, and nothing was substituted, so `unverified_completion` does **not** apply. However, the handoff's synthetic hash (`dd0f6cb9…`) does not reproduce (see F-10).

**Can the tests fail? (brief item 10)** Yes. I ran two mutations on the `/tmp` copy:
- Mutation A disabled the `Simulated` refusal. Result: `TOTAL 14/15`, exit 1.
- Mutation B silently dropped `UnmodelledSpecies` trees. Result: `TOTAL 12/15`, exit 1 (3 tests caught it, including the qualified Irish test).

After restoring the files, the suite was back to 15/15. The suite is real, but it has blind spots (F-16).

---

## 2. Findings

Severity scale: **blocking** means integration must not proceed; **major** means it must be fixed or explicitly waived before integration; **minor** means a recommended fix.

Probe IDs (`Pxx`) refer to my adversarial harness, described in §6. It lives outside the repo, compiles the candidate `src/Core/WoodlandImport/*.cs` unchanged, and builds in-memory archives.

### F-01 — BLOCKING — Extension rows with no core row, or a blank `coreid`, are silently dropped
- **Where:** `src/Core/WoodlandImport/DarwinCoreArchiveReader.cs:241-250` adds every extension row to `extRowsById` under its key, with no check that the key exists in `coreRowsById` and no check for a blank key. `:258-267` assembles joined records by iterating only the core ids, so any key that is not a core id is discarded.
- **Evidence:** P01 has 2 core trees plus 3 MoF rows for `GHOST`, which has no core row. The result is `CandidateForSimulationIntegration` with **0 issues**. DwC reader counters: `ExtensionRowsRead=9 joinedToCore=6 unaccounted=3 issues=0`. P02 has one MoF row with a blank `coreid` (DBH 77). Result: `CandidateForSimulationIntegration`, 0 issues.
- **Consequence:** Input measurement rows disappear without any report. `TotalMeasurementRowsScanned` (9) counts them, but nothing tells the consumer that 3 of them went nowhere. This is exactly the "extension row with no core row must not be silently dropped" case.
- **Required:** Emit a deterministic issue (a Refusal, or at minimum a reported issue with a count and the ids) for orphaned and blank-key extension rows, and add tests for both.

### F-02 — BLOCKING — Repeated or conflicting measurement rows for the same tree: the last row silently wins
- **Where:** `src/Core/WoodlandImport/IntegrateMarteloscopeReader.cs:238-280`. Each matching row reassigns `vitalField`, `dbhField`, `heightField`, `crownField` or `ageField`, so earlier rows are overwritten. (`:286` does the same for duplicate TreM codes.)
- **Evidence:** In P05, tree `T1` has DBH rows `30` and then `55`. Output: `dbh=55`, evidence `55`, readiness `CandidateForSimulationIntegration`, **0 issues**. The first observation is lost and no conflict is reported.
- **Consequence:** An observed measurement is silently discarded, and the result depends on file row order. For example, a re-measured DBH or contradictory status rows would collapse to whichever row came last.
- **Required:** Detect more than one row for the same single-valued field on the same tree. Either refuse it, or retain all values and report the conflict. Add a test.

### F-03 — MAJOR — A ZIP entry whose declared size understates its real size is silently truncated; CRC is not checked
- **Where:** `DarwinCoreArchiveReader.cs:113-135` checks only the *declared* sizes. `:326-356` streams the entry, but .NET 8 `ZipArchiveEntry.Open()` stops at the declared length, so the streamed-bytes cap check at `:345` can never trigger. There is also no `entryBytes == entry.Length` check and no CRC check.
- **Evidence:** In P12 the core file is really 680,018 B with 20,000 rows. I patched both of its size fields to 100 B and set the cap to 200,000. The importer read **3 rows** (the last one cut mid-row, giving species `'Fagus '`) and returned `InventoryOnly` with only ordinary blockers. No truncation or corruption issue was raised; 19,997 rows were lost silently.
- **Note:** Memory stays bounded, so the system is **not** vulnerable to a zip bomb. What fails is integrity: a corrupt or tampered archive is accepted as a smaller valid inventory.
- **Required:** Refuse when the bytes read differ from `entry.Length`, and/or verify CRC-32. Add a test with a lying header (the existing byte-cap test only exercises the declared-size path; see F-16).

### F-04 — MAJOR — A multi-plot archive read without `TargetPlotId` is merged into one stand
- **Where:** `IntegrateMarteloscopeReader.cs:79-107` (with no target set, every record is selected) and `:173-175` (the stand is named after the first plot id).
- **Evidence:** In P06, `PlotA/A1` and `PlotB/B1` both sit at local `(10,10)`. The result is a single stand named `PlotA` (`sourcePlotId=PlotA`), `CandidateForSimulationIntegration`, 0 issues.
- **Consequence:** The real publisher archive contains many plots. Reading it without a target would produce one stand that mixes trees from different plots in overlapping local frames, labelled as one plot, and it could be admitted. Coordinate fidelity and provenance are both lost.
- **Required:** When more than one distinct plot id is present and no `TargetPlotId` is given, refuse (or require a target).

### F-05 — MAJOR — A geographic boundary is not registered to the plot-local tree frame
- **Where:** `IntegrateMarteloscopeReader.cs:67-77` (the `RawGeoBoundaryVertices` path), `WoodlandBoundary.cs:169-249` (vertices are projected around the ring centroid, so they come out centred on about (0,0)), and `LocalCoordinateNormalizer.cs:44-60` (trees are translated by the boundary's min-xy).
- **Evidence:** In P15, a geographic rectangle of about 100 × 100 m is combined with plot-local trees at `(10,10)` and `(20,20)`. The result is `CandidateForSimulationIntegration`. Nothing relates the centroid-centred boundary frame to the plot-corner tree frame. Whether a tree counts as "inside", and its final position relative to the boundary, is an accident of the two origins.
- **Consequence:** A public request field can produce an admissible stand where tree positions relative to the boundary are meaningless. No test combines a geographic boundary with trees.
- **Required:** Either refuse (or mark InventoryOnly) when a geographic boundary is used with trees that have no georeference, or define and test an explicit registration rule.

### F-06 — MAJOR — Provenance laundering in two paths
- **(a) Inferred becomes Observed.** At `IntegrateMarteloscopeReader.cs:411-419`, an unrecognised status code gets `Provenance = Simulated ? Simulated : Observed`, so a declared `Inferred` row becomes `Observed`. P07: status `7` with `provenance=inferred` produced `prov=Observed`. The control case, recognised `1` with `inferred`, correctly stayed `Inferred`.
- **(b) The row-level "simulated" marker is not applied to species or position.** At `:213` and `:356-358`, species and position are always `Observed`, even though `:236` parses a core-level `provenance` term and applies it only to measurements. P09: a core-only row declaring `provenance=simulated` returned `InventoryOnly` with `posProv=Observed speciesProv=Observed` and no `SimulatedFieldAtYearZero` issue.
- **Consequence:** These are direct conflicts with the brief's priority 5 ("never silently accepted or converted"). Admission impact is limited today: (a) is blocked anyway because the status is Unknown, and the marker in (b) is a convention the implementer invented. Even so, the output provenance is wrong.
- **Field-level Simulated refusal is otherwise correct:** P08 (`measurementMethod=SIMULATED` on a height row) returned `Refused` with `SimulatedFieldAtYearZero` on `HeightM`, and `Stand=null`.

### F-07 — MAJOR — Unknown measurement units are silently treated as the target unit
- **Where:** `IntegrateMarteloscopeReader.cs:457-467`. Only `mm`, `cm` and `m` are converted; anything else passes the value through unchanged.
- **Evidence:** In P17, `12` with unit `inch` came out as `dbhCm=12` (it should be 30.48), with `Provenance=Observed`. The only trace is `Evidence.Note="unit=inch"`. The stand was still `CandidateForSimulationIntegration`.
- **Required:** Refuse unrecognised units, or mark the field Missing and report it.

### F-08 — MAJOR — A bare genus or a higher rank is classified `UnmodelledSpecies`, not `UnidentifiedSpecies`
- **Where:** `WoodlandSpeciesMapper.cs:167-185`. Any single plausible word falls through to `UnmodelledSpecies`.
- **Evidence:** P16: `Quercus`, `Fagus` and `Betula` (genus only) and `Fagaceae` and `Pinaceae` (family rank) all map to `UnmodelledSpecies`. Spec §6 lists "genus-only … higher taxonomic ranks" as `UnidentifiedSpecies`, and the handoff (§1.2) and `tasks.md` WI-T04 claim this is done. `Genus sp.`, `PLANTAE`, blank and numeric input are classified correctly.
- **Impact:** Admission and living counts are unaffected (a living tree is blocked in either category, and P18 shows dead trees are not counted as living), but the label is wrong and the handoff's claim is overstated.

### F-09 — MAJOR — The source qualification document overstates what was observed
I spot-checked three fixture records against `api.gbif.org/v1/occurrence/{key}/verbatim`: `3714470359`, `3714454821` and `3714457104`. Their occurrenceID, scientificName, verbatimCoordinates, eventDate and DBH/height/status values **match the fixtures exactly**, and GBIF reports the licence as `http://creativecommons.org/licenses/by/4.0/legalcode`. So the records are genuine. Several statements about them are not:
- `SOURCE-DATA-QUALIFICATION.md:43` presents the `verbatimCoordinates` "Local Cartesian metres `X , Y`" reading as observed. The same verbatim records declare `verbatimCoordinateSystem = decimal degrees`. Reading the values as plot-local metres is plausible (they range 0–100) but it is an **inference that contradicts the publisher's own field**, and the X/Y axis order is unverified. It should be labelled as an assumption.
- `:45` and the fixtures' `countryCode` column: the verbatim record has only `country` (`Ireland`). `countryCode=IE` is a GBIF-interpreted value, not verbatim. `measurementAccuracy` (`5%`, `20 cm`) was dropped from the fixtures. So `:32` ("Every field … observed directly from GBIF's stored verbatim publisher records") is overstated.
- `:75` and `:86` describe the 50 × 100 m and 100 × 100 m polygons as "surveyed" boundaries. They are hard-coded test constants (`FixtureQualificationTests.cs:13-27`) with no cited source for either plot's real dimensions or orientation. The headline claim that the "LU Mersch plot is `CandidateForSimulationIntegration`" therefore rests on an unsourced boundary.
- Honest parts: the IPT timeout, GBIF crawl ABORTs, the HTTP 403 on unauthenticated download, and `PENDING REAL ARCHIVE DOWNLOAD` for the publisher ZIP and `meta.xml` are all disclosed clearly. No publisher hash is fabricated.

### F-10 — MINOR — The handoff's synthetic determinism hash does not reproduce
`IMPLEMENTATION-HANDOFF.md:83,113` record `synthetic-canonical-sha256=dd0f6cb9…`. I got `7be5a873…` in 6 runs: on CRLF and LF checkouts, and under 4 timezones. Both **qualified** hashes match the handoff exactly. Within-run determinism holds every time. The synthetic hash depends on ZIP bytes compressed at runtime, so it is not a portable pin. Either the handoff output came from a slightly different tree, or the deflate output varies by environment. I could not tell which. Do not use the synthetic hash as a cross-machine reference.

### F-11 — MINOR — Header-line handling can parse the header as data
`DarwinCoreArchiveReader.cs:455-465` and `:568` silently clamp a negative `ignoreHeaderLines` to 0. `:623-632` counts a leading blank line toward `lineNumber`, so the real header is then read as a data row. P11.5 and P19 both produce a phantom tree `id`. They end up `Refused`, but with misleading codes (`MalformedTreeCoordinates`) rather than a meta/format refusal.

### F-12 — MINOR — The XML-hardening description is inaccurate
Spec §4 and `DarwinCoreArchiveReader.cs:384` set `MaxCharactersFromEntities = 0` as a hardening measure, but in .NET `0` means **no limit**. This is harmless, because `DtdProcessing.Prohibit` blocks every entity declaration (P11.3 and the suite's XXE test are refused, and the payload targets `127.0.0.1:1`, with no fetch observed). The documentation should be corrected. The DTD-versus-malformed classification at `:421-429` relies on substrings of the exception message; that works under invariant globalisation but is fragile.

### F-13 — MINOR — The spec contradicts itself on invalid boundaries
`spec.md:80` (§5.1) says invalid polygons "are refused". `spec.md:92` (§5.3) and Option A #3 say invalid boundaries give `InventoryOnly`. The code follows §5.3, which is consistent with Option A. Fix the §5.1 wording.

### F-14 — MINOR — Delimiter support is partial
`DarwinCoreArchiveReader.cs:454` and `:611` use only the first character of `fieldsTerminatedBy` and `fieldsEnclosedBy`, and `linesTerminatedBy` is ignored (CR, LF and CRLF are always treated as row ends). That is fine for the observed data, but it is undocumented.

### F-15 — MINOR (ecology assumption to flag) — Generic common names map to single species
`WoodlandSpeciesMapper.cs:73-74` and `:81-82` map the bare common names `Beech` to *Fagus sylvatica* and `Alder` to *Alnus glutinosa* (P16). The spec allows this, but it is a substitution risk for other alders such as *Alnus incana*. It should be recorded as a deliberate calibration and assumption.

### F-16 — MAJOR — Test coverage gaps hid F-01 to F-05
No test covers: orphan or blank-key extension rows; repeated measurement rows; a lying ZIP size header or the streamed-bytes cap path (`WoodlandImportCoreTests.cs:432-444` only trips the declared-size check); multi-plot archives without a target; or a geographic boundary combined with trees. All of these return green today.

---

## 3. Acceptance-criteria mapping

| Criterion | Result | Evidence |
|---|---|---|
| Spec §8.1 restore, 0 warnings / 0 errors | **PASS** | §1 |
| Spec §8.2 build, 0 warnings / 0 errors | **PASS** | §1 |
| Spec §8.3 run, exit 0, all PASS | **PASS** | §1 (15/15) |
| Spec §8.4 determinism (two-run byte identity + SHA-256) | **PASS** (within-run); cross-environment synthetic pin not reproduced | §1, P20, F-10 |
| Spec §8.5 fence | **PASS** | §5 |
| Brief 1: join / record-loss safety | **FAIL** | F-01, F-02, F-03. Duplicate core ids are refused correctly, including the whitespace variant `T1 ` (P04). A core row with no extension rows survives with `Missing` fields (P03). |
| WI-REQ-01 unsupported-species retention / InventoryOnly / dead not living | **PASS** | suite; P18a/b/c (dead PLANTAE gives living=1, dead=1, Candidate; living PLANTAE gives `UnidentifiedLivingSpecies`; no status counts as living) |
| WI-REQ-02 Missing preserved, no allometry | **PASS** | P03; a search for `Math.Pow/Exp/Log/Round`, "estimat", "impute" found no estimation arithmetic. The only arithmetic is linear unit conversion at `IntegrateMarteloscopeReader.cs:460-466` (see F-07). |
| WI-REQ-03 no boundary inference | **PASS** | suite 4c/4d; Boundary stays null |
| WI-REQ-04 Irish first + compatible European plot | **PASS** for import and inspection of genuine records; **NOT VERIFIED** for the "surveyed boundary" Candidate claim | F-09 |
| WI-REQ-05 Layer 1 contracts, pure BCL, no clock | **PASS** | source read; hygiene test |
| WI-REQ-06 Year-0 Simulated refused | **PASS** at field level (P08); **FAIL** for provenance conversion | F-06 |
| WI-REQ-07 species mapping | **FAIL** (genus-only and higher ranks) | F-08; target-species, author-suffix, hybrid and no-fuzzy behaviour PASS (P16, suite) |
| WI-REQ-08 shoelace + tangent plane | **PASS** | P13: non-axis-aligned pentagon `(0,0),(30,5),(45,25),(20,40),(-5,20)`. My hand calculation: Σ(xᵢyᵢ₊₁−xᵢ₊₁yᵢ) = 0+525+1300+600+0 = 2425, so the area is **1212.5 m²**. The code returns `1212.5` in both winding orders. P14: compared with spherical great-circle distance, the tangent-plane edge error is **1.2e-5 at ~111 m, 1.2e-4 at ~1.1 km, 5.9e-4 at ~5.5 km**. A spherical R = 6,371,008.8 m differs from the WGS-84 ellipsoid at 53°N by roughly 0.08% (N–S) and 0.3% (E–W) (my calculation, not run), which is consistent with the spec's "not UTM" disclaimer. |
| WI-REQ-09 point-in-polygon, no clamping | **PASS** | P10: edge `(100,40)` and vertex `(0,0)` are inside; `(100.0001,40)` is refused with one `TreeOutOfBounds`, even for a dead tree, and is not clamped |
| WI-REQ-10 translation-only | **PASS** in the local frame (no scale, rotation or snapping; distances preserved apart from IEEE subtraction rounding of about 1e-14 m) | suite; F-05 is a registration issue, not a scaling one |
| WI-REQ-11 caps before allocation | **PARTIAL / FAIL** | Living cap (unknown counted as living), records cap and measurement cap: PASS. Declared byte cap: PASS. The streamed-bytes clause does not work and size lies truncate silently (F-03). |
| WI-REQ-12 XXE / DOCTYPE | **PASS** | suite; P11.3 |
| WI-REQ-13 determinism | **PASS** (within run) | F-10 |
| WI-REQ-14 build / hygiene / fence | **PASS** | §1, §5 |
| Malformed `meta.xml` handling | **PASS** | P11.0–P11.8 (missing `<id>`/`<coreid>`, bad index, DOCTYPE, unknown encoding, key index out of range, multiple roots, two `<core>` elements) were all refused with no unhandled exception. P11.5 is refused for the wrong reason (F-11). |

---

## 4. Licence audit (CC BY 4.0)

- The licence is confirmed independently: GBIF's interpreted record `3714470359` has `license = http://creativecommons.org/licenses/by/4.0/legalcode`.
- `Fixtures/Qualified/ATTRIBUTION.json` records the title, publisher (INRAE), DOI `10.15468/ocof3v`, GBIF dataset key, licence and URL, the full citation with access date, the retrieval date, the crawl id and per-fixture GBIF occurrence keys. The fixtures are attributed.
- **No full archive was committed.** The fixtures are 2,137 B and 2,145 B, holding 20 of 271 (IE) and 20 of 134 (LU) records.
- **No licence laundering:** the licence is carried through unchanged into `ImportSourceAttribution` (`IntegrateMarteloscopeReader.cs:15`, `:612`).
- **Gaps (minor, part of F-09):** CC BY 4.0 §3(a)(1)(B) requires saying whether the material was modified. ATTRIBUTION.json says the rows were "packaged into a standard DwC-A envelope" and gives subset counts. It does not say that columns were dropped (for example `measurementAccuracy`) or that `countryCode` came from GBIF's interpreted record. The ZIPs carry no attribution or `eml.xml` of their own, so a copy of a ZIP on its own loses attribution. I recommend adding an explicit "changes made" statement and embedding the attribution in each ZIP.
- **Is the wrapper a valid DwC-A?** Yes. Its `meta.xml` follows the DwC text guide structure (`archive/core/files/location`, `id index=0`, field indices, an `extension` with `coreid index=0`, an empty `fieldsEnclosedBy`, `ignoreHeaderLines=1`), and it exercises the reader's real tab/header/join path. `eml.xml` is optional for the reader. The wrapper does **not** test the real IPT `meta.xml`, and the document correctly says this is `PENDING`.

## 5. File-fence audit

`git diff --name-only 6d72c305..e51b1e20` lists 23 files. Filtering out the four allowed prefixes leaves **0 files outside the fence**. **PASS.** No `.sln`, `Directory.Build.props`, P0 or other files were touched.

There is one integration-risk note that is not a fence violation: `src/Core/SitkaP1.Core.csproj` uses the default SDK glob, so after a merge `src/Core/WoodlandImport/*.cs` will compile into `SitkaP1.Core` as well. That build was **not verified**: the implementer did not run it, and neither did I, because `.p0ref/SitkaLab.Core.dll` is absent. I did confirm that the new files contain none of the substrings banned by the existing `Core.Tests` `644 B-01` scan.

## 6. Adversarial probes (outside the repo)

The harness consists of `Probe.csproj` and `Program.cs`. I kept them outside the repo, in the reviewer's temp folder, and built them in WSL `/tmp/wr-probe`. They compile the candidate's `src/Core/WoodlandImport/*.cs` unchanged. Summary of observed behaviour:

| Probe | Input | Observed | Verdict |
|---|---|---|---|
| P01 | 3 MoF rows for a missing core id | Candidate, 0 issues, 3 rows unaccounted | F-01 |
| P02 | MoF row with blank coreid | Candidate, 0 issues | F-01 |
| P03 | core row, no MoF | retained; DBH/height/status `Missing`; InventoryOnly | OK |
| P04 | core ids `T1`, `T1 ` | Refused `DuplicateSourceTreeId` | OK |
| P05 | DBH 30 then 55 | DBH 55, 0 issues | F-02 |
| P06 | two plots, no target | one stand "PlotA", Candidate | F-04 |
| P07 | unknown status, `provenance=inferred` | `Observed` | F-06a |
| P08 | height `SIMULATED` | Refused, `SimulatedFieldAtYearZero` | OK |
| P09 | core `provenance=simulated` | InventoryOnly, position/species `Observed` | F-06b |
| P10 | edge / vertex / 0.1 mm outside | edge and vertex inside; outside refused | OK |
| P11.x | 9 malformed `meta.xml` | all refused, no crash | OK (F-11) |
| P12 | lying ZIP size header | 3 of 20,000 rows, InventoryOnly | F-03 |
| P13 | pentagon | 1212.5 m² = hand calculation | OK |
| P14 | tangent plane vs great circle | 1e-5 … 6e-4 relative | OK (documented approximation) |
| P15 | geographic boundary + local trees | Candidate | F-05 |
| P16 | genus / family / var. / hybrid / case | see F-08 / F-15; var./f./subsp. non-nominate become Unmodelled (conservative) | F-08 |
| P17 | `mm` / `inch` units | mm converted correctly; inch passed through as cm | F-07 |
| P18 | PLANTAE dead / living / no status | dead is not counted as living; living or unknown blocks | OK |
| P19 | leading blank line | header parsed as data | F-11 |
| P20 | two-run hash | equal | OK |

## 7. Residual risks and what I could not verify

- **Real publisher archive:** neither the implementer nor I could test it. The IPT `meta.xml` delimiters, quoting, encoding and multi-plot layout are untested, and they interact with F-04 and F-14.
- **The `verbatimCoordinates` interpretation** (local metres, X then Y) is unverified against publisher documentation (F-09).
- **Plot boundary dimensions** for Donadea and Mersch are unverified (F-09).
- **The `SitkaP1.Core.csproj` build** with WoodlandImport included is unverified (§5).
- **Ellipsoidal error magnitude** is my own calculation, not a run.
- **No Unity, runtime, save/load or `/multi` integration exists or was tested;** this is correct per D-030.
- **I spot-checked 3 of the 40 fixture records against GBIF verbatim.** The other 37 were not individually checked.
- **Git environment:** WSL git worked. `git status` was clean (0 entries) at the start of the review, and no provisioning artefacts appeared. My gate run (A) used a CRLF checkout; run (B) used an LF `git archive`, and both gave identical results.

# Woodland Importer V1 — R2 Focused Re-Review of Repair Cycle 1

- **Review date (UTC):** 2026-11-26
- **Reviewer:** Claude Opus 5.5 (`claude-opus-5-5`, Claude Code). This was a fresh, independent context. I did not author the implementation, the repair, or either cycle-1 review.
- **Repaired candidate:** `add375a4135387daf16e9ff506066bae6bd28a62` (`origin/feat/multi-2d-woodland-importer-v1`, confirmed with `git ls-remote`)
- **Original candidate (cycle 1):** `e51b1e20625e51240a770211f42e1bec70c54c5c`. `add375a4` is a direct single-commit descendant of it (`fix(woodland-importer): repair cycle 1 addressing independent review F-01..F-16`).
- **Base:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`, confirmed with `git ls-remote`; it is also the merge-base)
- **Prior reviews read:** `INDEPENDENT-REVIEW.md` (F-01..F-16) and `OPUS-INDEPENDENT-REVIEW-20261126.md` (R1..R5, N1..N6)
- **Role limits honoured:** I edited no source, test, spec or fixture. All builds and probes ran on `git archive` exports and on an out-of-repo probe project under WSL `/tmp/r2`. This file is the only change on the review branch. The protected listeners `:33767`, `:43649` and `:46593` were down (checked with `ss -ltn`).
- **Git environment:** WSL git worked normally. `git status --short` in the review worktree was clean (0 entries) at the start.

---

## R2 INTEGRATION DECISION

**R2 INTEGRATION DECISION: NEEDS CHANGES**

The repair genuinely closes both cycle-1 blocking findings (F-01, F-02) and the major findings F-03 to F-08. I reproduced each fix with my own probes, and the qualified fixtures still give identical canonical hashes. Three defects stop approval:

1. **R3.1 is still open (blocking, carried from the Opus cycle-1 review).** A `measurementType = "Crown diameter"` row with value 8 m is still mapped to **DBH = 800 cm**, provenance `Observed`, readiness **`CandidateForSimulationIntegration`**, with 0 issues. That is a silently wrong measurement on an admissible stand.
2. **R5 is still open (blocking, carried from the Opus cycle-1 review).** `AttributionOverride.ArchiveSha256` still replaces the computed archive SHA-256 (`stand.Attribution.ArchiveSha256 = "0000-not-the-real-hash"`).
3. **New regression NF-01 (major).** The F-04 multi-plot guard relies on the `ExtractPlotId` heuristic. A legitimate single-plot DwC-A whose `occurrenceID`s do not follow the Integrate+ `{n}_{plot}` convention (for example `t1`, `t2` or `tree_1`, `tree_2`), and which has no `locationID`, `eventID` or `locality`, is now **Refused** with `MultiplePlotsRequireTargetPlotId`. No `TargetPlotId` can select the whole plot. The existing tests were renamed from `t1` / `dup_1` / `t_oob_1` to `1_PlotA` so that they avoid this behaviour.

Each fix is small and local to `IntegrateMarteloscopeReader.cs`, plus one test each.

---

## 1. Delta scope

`git diff --stat e51b1e20..add375a4` lists 10 files, +1008 / −200 lines. All of them sit inside the lane fence:

| File | Lines changed | Change |
|---|---|---|
| `src/Core/WoodlandImport/DarwinCoreArchiveReader.cs` | 259 | Orphan and blank-key refusal; raw ZIP locator, size check and CRC-32 check; header and `ignoreHeaderLines` checks; `MaxCharactersFromEntities = 1024` |
| `src/Core/WoodlandImport/IntegrateMarteloscopeReader.cs` | 174 | Multi-plot refusal, geo-frame guard, duplicate-row detection, provenance propagation, unit refusal |
| `src/Core/WoodlandImport/WoodlandSpeciesMapper.cs` | 117 | Genus, genus+authority and higher rank become `UnidentifiedSpecies` |
| `tests/Core/WoodlandImport/WoodlandImportCoreTests.cs` | 318 | 8 new tests, expanded species test, tree IDs renamed (see §5) |
| `tests/Core/WoodlandImport/SyntheticArchiveBuilder.cs` | 137 | `extraMofRows`, core provenance column, `corePrefix`, ZIP size patcher; **default compression is now `NoCompression`** |
| `tests/Core/WoodlandImport/FixtureQualificationTests.cs` | 35 | Constants renamed to `ConstructedTestBoundary*`; `changesMade` assertion |
| `tests/.../Fixtures/Qualified/ATTRIBUTION.json` | 3 | Adds `changesMade`; download status mentions HTTP 403 |
| `specs/multi-2d-woodland-importer-v1/spec.md` | 24 | §4, §5.1, §5.3 and §6 updated |
| `Docs/.../SOURCE-DATA-QUALIFICATION.md`, `IMPLEMENTATION-HANDOFF.md` | 59, 82 | F-09 honesty edits; repair §5 |

The fixture ZIPs are byte-unchanged (`git diff --stat` on `Fixtures/` touches only `ATTRIBUTION.json`).

---

## 2. Gates (run myself, WSL Ubuntu, pinned SDK)

I ran the three approved commands without changes on **two clean `git archive add375a4` exports**: (A) `core.autocrlf=true`, which gives CRLF and matches the Windows checkout, and (B) `core.autocrlf=false`, which gives LF. The review worktree itself is checked out on the review branch (`fec2ccef`, which carries cycle-1 code), so it is not the candidate. That is why the gates ran on exports.

| Gate | A (CRLF) | B (LF) |
|---|---|---|
| `dotnet --version` | `8.0.133` | `8.0.133` |
| restore | exit 0 | exit 0 |
| build `--no-restore` (`TreatWarningsAsErrors=true`) | **0 Warning(s), 0 Error(s)**, exit 0 | **0 Warning(s), 0 Error(s)**, exit 0 |
| run `--no-build --no-restore` | **synthetic 19/19, qualified 4/4, TOTAL 23/23, 0 failed, exit 0** | **synthetic 19/19, qualified 4/4, TOTAL 23/23, 0 failed, exit 0** |
| IE canonical SHA-256 | `72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63` (31,903 B) | same |
| LU canonical SHA-256 | `7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf` (29,876 B) | same |
| Synthetic canonical SHA-256 | `395962d1e484ac02e2184f34c28c968d89bc57c8a16e7ef8fd87ea46f25d8723` | `471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af` (matches handoff) |

I reran B twice more under `TZ=Asia/Kolkata`: 23/23 with the same three hashes each time.

**Not run:** the full `MultiRackSitkaP1Lab.sln` and `Core.Tests` (291). `.p0ref/SitkaLab.Core.dll` is absent from this worktree. The cycle-1 Opus review ran them against `e51b1e20`; they have not been re-verified for `add375a4`.

---

## 3. Per-finding verdicts

My probe harness (`/tmp/r2/probe`, outside the repo) compiles the candidate's `src/Core/WoodlandImport/**/*.cs` and `SyntheticArchiveBuilder.cs` unchanged. It is ephemeral and not committed.

| ID | Sev (cycle 1) | R2 verdict | My evidence |
|---|---|---|---|
| **F-01** orphan / blank-key ext rows | Blocking | **CLOSED** | P01: 3 rows for `9_GHOST` give `Refused`, `ArchiveOrphanExtensionRow@9_GHOST`, "3 extension row(s)". P01b: 1 + 2 orphans plus blank `""` and whitespace `"   "` keys give `ArchiveBlankExtensionCoreId` ("2 extension row(s)") plus one orphan issue per id with counts 2 and 1, so all 5 rows are accounted for. P01c: the same rows reordered give an identical result hash. P02: one blank key gives `Refused`. P02b: ext key `"1_P "` joins core `"1_P"` (trimmed; not a loss). |
| **F-02** repeated / conflicting rows | Blocking | **CLOSED** (minor residual NF-03) | P05a DBH 30→55 and P05b 55→30 both give `Refused` with `DuplicateMeasurementRow@1_P.DbhCm`, values `[30 cm, 55 cm]`. Result SHA-256 is equal and the JSON is byte-identical. Identical repeat 30/30, status 1/0, and 300 mm + 30 cm are all refused. |
| **F-03** lying ZIP size / CRC | Major | **CLOSED** (test-coverage nit NF-04) | 20,000-row deflate archive with `occurrence.txt` declared as 100 B. Cap 5 MB gives `ArchiveEntrySizeMismatch` ("decompressed to 1988984 bytes … declared 100"). The stored-method variant gives the same refusal. A CRC flipped on `occurrence.txt` (deflate), MoF (stored) or `meta.xml` gives `ArchiveEntryCrcMismatch`. One payload byte flipped with the CRC header untouched gives `ArchiveEntryCrcMismatch`. **Streamed cap:** a 64 MiB deflate core (65,834 B zip) declared as 100 B with cap 1 MB gives `CapUncompressedBytesExceeded` ("Decompressed byte count while reading 'occurrence.txt' exceeded cap"), with **2.09 MiB allocated** during `Read`. The honest-header control is refused by the declared-size cap with 0.07 MiB allocated. Memory stays bounded. The honest 20,000-row archive still gives Candidate. |
| **F-04** multi-plot merge | Major | **PARTIALLY CLOSED**: merge is refused for Integrate+ IDs, but the guard introduces NF-01 | P06: `1_PlotA` + `2_PlotB` with no target gives `Refused` `MultiplePlotsRequireTargetPlotId`; target `PlotB` gives Candidate with 1 tree. Edge: `PlotA` and `plota` are still merged, because the set uses `OrdinalIgnoreCase` (NF-02). |
| **F-05** geo boundary vs local trees | Major | **CLOSED** | P15: `RawGeoBoundaryVertices` with local trees gives `InventoryOnly`, `BoundaryTreeFrameMismatch[InventoryOnlyBlocker]`, `Stand.Boundary == null`, and both trees retained. P15b: a pre-built geographic `WoodlandBoundary` passed via `SurveyedBoundary` is caught the same way. |
| **F-06** provenance laundering | Major | **CLOSED** | P07: unknown status `7` with `provenance=inferred` gives `VitalStatus.Provenance = Inferred`. P09: core-only `provenance=simulated` gives `Refused` with `SimulatedFieldAtYearZero` on both `RawSpeciesName` and `PositionLocalM`. P09b: core `inferred` gives species, position and DBH all `Inferred` (readiness Candidate, because the validator allows Inferred; this is unchanged spec behaviour). |
| **F-07** unknown units | Major | **CLOSED for non-empty unknown units** (residual NF-05) | `inch` on DBH and `ft` on height give `Refused` `UnsupportedMeasurementUnit`. `mm` converts (300 mm → 30 cm). `CM` is accepted case-insensitively. |
| **F-08** genus / higher rank | Major | **CLOSED** | `Quercus`, `Fagus`, `Betula`, `Fagaceae`, `Pinaceae`, `Rosales`, `Pinopsida`, `Magnoliophyta`, `Quercus L.`, `Tilia L.`, `Quercus (L.)`, `Fagaceae Dumort.` and `Quercus sp./spp.` all give `UnidentifiedSpecies`. `Quercus robur`, `Abies alba Mill.`, `Picea abies (L.) H.Karst.`, the hybrid and `Norway spruce` give `UnmodelledSpecies`. `Fagus sylvatica L.` gives `beech`. Side effect: single-word common names such as `Ash` and `oak` now give `UnidentifiedSpecies` (previously Unmodelled). That is conservative and acceptable. |
| **F-09** qualification honesty | Major | **CLOSED** (doc nit NF-06) | `SOURCE-DATA-QUALIFICATION.md` §2 now labels `verbatimCoordinates` as **"INFERENCE / ASSUMPTION"**, explicitly contradicting `verbatimCoordinateSystem = "decimal degrees"`, and says the X/Y order and orientation are unverified. `countryCode` is marked **GBIF-interpreted (not verbatim)**. Dropped columns are listed, including `measurementAccuracy` and `verbatimCoordinateSystem`. There is a CC BY 4.0 §3(a)(1)(B) "Statement of Modifications" in the doc, and `ATTRIBUTION.json` has `changesMade`, which a test asserts. The §3 banner says both polygons are **"constructed test-harness constants with no cited survey source"**, and the LU Candidate claim is explicitly made conditional on that polygon. The "Every field … observed directly" overstatement is gone. I found no remaining overstatement in the qualification document. |
| **F-10** synthetic hash | Minor | **PARTIALLY CLOSED** | `471f61e8…` reproduces on an **LF** export only. On a CRLF (Windows) export the suite prints `395962d1…`. The handoff blames zlib variance, but the driver I observed is line endings: `BuildDefaultMetaXml` uses multi-line verbatim C# string literals, so the `meta.xml` bytes, and therefore the archive SHA embedded in the output, depend on the checkout's EOL. The handoff correctly says the hash is "within-run only", so this is a documentation error, not a correctness one. |
| **F-11** header handling | Minor | **CLOSED** | A leading blank line gives `ArchiveMalformedHeader`. A leading whitespace-only line gives `ArchiveMalformedHeader`. `ignoreHeaderLines=-1` gives `ArchiveMalformedMetaXml`. |
| **F-12** XML hardening text | Minor | **CLOSED** | `MaxCharactersFromEntities = 1024`. The spec §4 and code comment now say `0` means unlimited and that `DtdProcessing.Prohibit` + `XmlResolver = null` is the primary defence. The DTD classification now also checks for a `<!DOCTYPE` / `<!ENTITY` token, which is less fragile than matching message substrings. The suite's XXE test passes. |
| **F-13** spec self-contradiction | Minor | **CLOSED** | Spec §5.1 now says invalid polygons are "rejected by `TryCreate*`" and leave the stand `InventoryOnly`, which is consistent with §5.3. |
| **F-14** delimiter support | Minor | **CLOSED (documented)** | Spec §4 and the reader's XML doc say that only single-character field and quote delimiters are supported and that CR/LF/CRLF always end a row. |
| **F-15** generic common names | Minor | **CLOSED (documented)** | Spec §6 has a footnote and the mapper has comments marking `Beech` → *F. sylvatica* and `Alder` → *A. glutinosa* as a UK/Irish calibration assumption, with the *A. incana* risk noted. The mapping itself is unchanged, by design. |
| **F-16** test coverage | Major | **PARTIALLY CLOSED** | Eight new regression tests exist, and each corresponds to a probe that failed on `e51b1e20` and passes on `add375a4`. Gaps: (a) the suite's F-03 "Case B" does **not** exercise the streamed cap (NF-04); (b) there is no test for R3.1, R5, plain-ID single-plot archives (NF-01) or empty units. |

**Counts:** F-01..F-16 give **13 CLOSED, 3 PARTIALLY CLOSED (F-10, F-16, and F-04 because it closes but introduces NF-01), 0 STILL OPEN.** The two items from the Opus cycle-1 review, R3.1 and R5, are **STILL OPEN** (see below).

### Carried from the Opus cycle-1 review (not in the F-numbering, still applicable)

| ID | R2 verdict | Evidence |
|---|---|---|
| R1 multi-plot | CLOSED (= F-04) | above |
| R2 geo frame | CLOSED (= F-05) | above |
| R3.1 `Contains("diameter")` grabs crown diameter | **STILL OPEN — blocking** | `IsDiameterRow` (`IntegrateMarteloscopeReader.cs:654-658`) still uses `type.Contains("diameter")`. Probe: a single MoF row `Crown diameter / crown / 8 / m` gives **`CandidateForSimulationIntegration`, `dbhCm = 800`, `Observed`, 0 issues**. If a genuine DBH row is also present, F-02 now refuses with `DuplicateMeasurementRow`, but on its own the crown row is admitted as DBH. |
| R3.2 unknown / empty unit | PARTIAL (= F-07; NF-05) | above |
| R3.3 duplicates | CLOSED (= F-02) | above |
| R4 orphans | CLOSED (= F-01) | above |
| R5 override replaces computed hash | **STILL OPEN — blocking** | `IntegrateMarteloscopeReader.cs:717` `ArchiveSha256 = request.AttributionOverride.ArchiveSha256 ?? archive.ArchiveSha256`. Probe: the override `"0000-not-the-real-hash"` is emitted as `stand.Attribution.ArchiveSha256`. |
| N3 plot id includes survey index | Open, undocumented | This is now load-bearing for F-04: `X_Plot_2` and `X_Plot_3` count as different plots, so the archive is refused. That is fail-closed, which is acceptable, but it should be documented. |

---

## 4. New findings introduced by the repair

| ID | Severity | Finding |
|---|---|---|
| **NF-01** | **Major** | **Legitimate single-plot archives with non-Integrate+ IDs are now refused.** When there is no `locationID`, `eventID` or `locality`, `ExtractPlotId` (`IntegrateMarteloscopeReader.cs:674-693`) returns the whole id if it has no `_`, or the text after the first `_`. The new guard (`:114-129`) then sees one "plot" per tree. Probes: `t1`, `t2` give `Refused` "Archive contains 2 distinct plots (t1, t2)". `tree_1`, `tree_2` give `Refused` (plots `1` and `2`). A single tree `t1` gives Candidate. `TargetPlotId` cannot help, because it selects only one tree. The behaviour fails closed (no data loss), but it is collateral damage to imports the reader previously accepted, and the core-only fallbacks (`dbhCm`, `heightM`, `vitality`) show that generic archives are in scope. **The suite hid it:** every pre-existing synthetic test ID (`t1`, `dup_1`, `t_sim`, `t_in`, `t_oob_1/2`, `t2`…`t5`) was renamed to `N_PlotA`. *Suggested fix:* only treat the `{n}_{plot}` suffix as a plot id when the archive clearly follows the convention, and otherwise treat the archive as one plot. Or refuse with a specific code that tells the caller to supply `locationID`. Add a test that keeps a plain-ID archive. |
| **NF-02** | Minor | `distinctPlots` uses `StringComparer.OrdinalIgnoreCase`, so `1_PlotA` and `2_plota` are merged into one Candidate stand. If plot ids differing only by case are real, separate plots, frames are silently merged. Use `Ordinal` for the distinctness check, or refuse when the case-insensitive and ordinal counts differ. |
| **NF-03** | Minor | **The refusal report is order-dependent when one of the duplicate rows is malformed.** DBH `abc` + `30` in both orders: both are `Refused` with `DuplicateMeasurementRow` and `MalformedMeasurementValue`. But when the malformed row comes last, the last-wins `dbhField` becomes Missing and an extra `MissingRequiredDbh` blocker appears, so the result hashes differ. Readiness is still deterministic. Fix: when a duplicate is detected, set the field to Missing regardless of row order, or suppress per-row derived issues. |
| **NF-04** | Minor (test) | The F-03 test "Case B" is labelled as the streamed-cap path, but it triggers the *declared*-size cap on `measurementorfact.txt` (~22 KB declared > 4096 B cap) before streaming starts. The streamed path works (my 64 MiB bomb probe), but no committed test covers it. The handoff's claim "Covered by synthetic test F-03 (both size-mismatch and streamed-bytes cap paths)" is overstated. |
| **NF-05** | Minor | An empty `measurementUnit` on a DBH, height or crown row is still silently taken as the target unit (12 with unit `""` gives `dbhCm = 12`, `Observed`, Candidate, no note). The real TreMs rows carry units, so the fixtures are unaffected. Either document it as a deliberate assumption or block it. |
| **NF-06** | Minor (docs) | The handoff's original §1–§4 still show the pre-repair test names (for example "qualified-lu-mersch … CandidateForSimulationIntegration with boundary") and the stale `dd0f6cb9…` hash at lines 83 and 113, with no "superseded by §5" marker. The F-10 root cause is misattributed (zlib rather than line endings). |

ZIP-reader notes (not findings): `IndexRawZipEntries` falls back to `entry.Open()` for ZIP64, unknown compression methods or non-UTF-8 names. In that fallback the size and CRC checks still catch truncation, but the streamed cap is limited by the declared size, as before. Duplicate entry names map last-wins in the raw locator, while `FindEntry` returns the first, so any mismatch would be refused by the size or CRC check rather than accepted.

---

## 5. Test-weakening audit (diff, not totals)

- **No assertion was removed or relaxed.** All pre-existing `TestRunner.Eq` / `True` checks still assert the same readiness, codes, counts and hashes. Changes are limited to labels ("refused" → "rejected" for `TryCreate*`, "surveyed" → "constructed test").
- **However, tree IDs in every pre-existing synthetic test were renamed** to the `N_PlotA` convention. The assertions are equivalent, but this rename is exactly what hides NF-01. I treat it as a coverage regression rather than a weakened assertion.
- `SyntheticArchiveBuilder` now defaults to `CompressionLevel.NoCompression`, so most synthetic tests exercise the new **Store** raw path. The **Deflate** raw path is still covered by test 8d (explicit `Optimal`), F-03 (explicit `Optimal`) and both qualified fixtures (deflated, hashes unchanged).
- Totals went from 15 to 23 tests (synthetic 11 → 19, qualified 4 → 4).

## 6. Fence audit

- `git diff --name-only 6d72c305..add375a4`: **23 files, 0 outside** `specs/multi-2d-woodland-importer-v1/`, `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/`, `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/` and `Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/`.
- The repair delta `e51b1e20..add375a4` has 10 files, all inside the fence.
- `git diff --summary 6d72c305..add375a4` contains only `create mode 100644`, with no mode changes or deletions. No `.sln`, `.csproj`, `Directory.Build.props`, `Assets/`, `ProjectSettings/` or `Docs/Project/` changes.
- I searched `src/Core/WoodlandImport` for `File.`, `Directory.`, `DateTime.Now/UtcNow`, `Stopwatch` and `Environment.TickCount`: no hits. The suite's hygiene test also passes.
- **PASS.**

## 7. Residual risks and what I did not verify

- The real publisher IPT archive and its `meta.xml` are still untested (`PENDING REAL ARCHIVE DOWNLOAD`). The multi-plot guard (NF-01, N3) and delimiter handling will meet real data for the first time there.
- The full `.sln` and `Core.Tests` (291) were **not** rebuilt for `add375a4` (`.p0ref` is absent). Because of default globbing, the repaired importer compiles into `SitkaP1.Core.dll`.
- I did not re-check the fixture rows against live GBIF in this cycle. The fixture ZIP bytes are unchanged since cycle 1, where 40/40 matched.
- The local-metres reading of `verbatimCoordinates` and both plot boundaries are still unverified assumptions. They are now labelled honestly.
- My F-10 root-cause attribution (verbatim string literals with CRLF) is an inference from the observed CRLF/LF hash split. I did not bisect it.
- Ellipsoidal error and Unity, runtime or `/multi` integration are out of scope and were not tested.

## 8. Required to reach APPROVED

1. **R3.1:** match exact, documented `measurementType`/`measurementRemarks` values for DBH (or exclude crown and other diameters), and add a test that a `Crown diameter` row alone does not become DBH.
2. **R5:** always record the computed archive SHA-256; keep any declared hash in a separate field, refuse on mismatch, and add a test.
3. **NF-01:** stop refusing single-plot archives whose IDs do not follow the Integrate+ convention, and add a test using plain IDs (`t1`, `t2`).
4. Recommended in the same pass: NF-02 to NF-06.

*Independent review only. Nothing was merged, no feature-branch ref was touched, and Unity was not involved.*

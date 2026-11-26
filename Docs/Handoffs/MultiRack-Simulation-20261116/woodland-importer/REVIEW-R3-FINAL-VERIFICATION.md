# Woodland Importer V1 — R3 Final Verification of Repair Cycle 2

- **Review date (UTC):** 2026-11-26
- **Reviewer:** Claude Opus 5.5 (`claude-opus-5-5`, Claude Code). This was a fresh, independent context. I did not author the implementation, either repair, or any earlier review.
- **Candidate under review:** `674f3350c3554aee63cb5f3f411c9858985e538d` (`origin/feat/multi-2d-woodland-importer-v1`, confirmed with `git ls-remote`)
- **Revision chain:** base `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`, which is also the merge-base) → `e51b1e20` (cycle 1, NEEDS CHANGES) → `add375a4` (repair 1, R2 NEEDS CHANGES) → **`674f3350`** (repair 2, a single commit on top of `add375a4`)
- **Prior reviews read first:** `INDEPENDENT-REVIEW.md`, `OPUS-INDEPENDENT-REVIEW-20261126.md`, `REVIEW-R2-REPAIR-VERIFICATION.md`
- **Role limits honoured:** I edited no source, test, spec or fixture, and touched no feature-branch ref. All builds and probes ran on `git archive` exports and a throwaway `--shared` sparse clone under WSL `/tmp/r3`. This file is the only change on the review branch.
- **Environment:** WSL Ubuntu with the pinned SDK (`dotnet --version` printed `8.0.133`). The review worktree's `git status --short` was clean (0 entries), with no mass deletions. The protected listeners `:33767`, `:43649` and `:46593` were down, checked with `ss -ltn` at the start and the end.

---

R3 FINAL DECISION: APPROVED FOR INTEGRATION

Every blocking and minor item carried from R2 is closed, and I confirmed each one with my own probes. The test diff only adds tests or makes one stricter. Both qualified fixtures produce the same pinned canonical hashes. The fence is intact. The full solution and `Core.Tests` (291) also pass at the candidate, which R2 could not verify. Three new observations are minor and fail-closed. They are listed in §4 as non-blocking follow-ups, not as conditions.

---

## 1. Cycle-2 delta

`git diff --stat add375a4..674f3350` lists 8 files, +602 / −90 lines:

| File | Change |
|---|---|
| `src/Core/WoodlandImport/IntegrateMarteloscopeReader.cs` | Exact measurement-type classifiers, with crown checked before stem; crown diameter → crown radius; `VerifyDeclaredArchiveSha256`; `TryExtractPlotSurveyId` (nullable, Integrate+ pattern only); ordinal plot distinctness and `TargetPlotId` match; duplicated fields reset to `Missing`; empty dimensional unit refused |
| `src/Core/WoodlandImport/WoodlandImportContracts.cs` | Adds `ImportSourceAttribution.DeclaredArchiveSha256` (optional, additive) |
| `src/Core/WoodlandImport/WoodlandImportSerializer.cs` | Writes `declaredArchiveSha256` **only when it is non-null**, so existing canonical JSON is byte-unchanged |
| `tests/.../SyntheticArchiveBuilder.cs` | `BuildDefaultMetaXml` normalises `\r\n` → `\n` (F-10) |
| `tests/.../WoodlandImportCoreTests.cs` | F-03 Case B rebuilt (NF-04); 6 new tests (R3.1, R5, NF-01/N3, NF-02, NF-03, NF-05) |
| `specs/.../spec.md` | §3 attribution contract; §5.3 refusal list and exact-classification note |
| `Docs/.../IMPLEMENTATION-HANDOFF.md`, `SOURCE-DATA-QUALIFICATION.md` | §2/§3 marked superseded; new §6; N3 note |

No fixture files changed in this cycle (`git diff --name-only add375a4..674f3350 -- '*Fixtures*'` is empty).

## 2. Gates (run myself)

I ran the three approved commands unchanged on two clean `git archive 674f3350` exports: (A) `core.autocrlf=true`, which gives CRLF like the Windows checkout, and (B) `core.autocrlf=false`, which gives LF.

| Gate | A (CRLF) | B (LF) |
|---|---|---|
| `dotnet --version` | `8.0.133` | `8.0.133` |
| restore | exit 0 | exit 0 |
| build `--no-restore` (`TreatWarningsAsErrors=true`) | **0 Warning(s), 0 Error(s)**, exit 0 | **0 Warning(s), 0 Error(s)**, exit 0 |
| run `--no-build --no-restore` | **synthetic 25/25, qualified 4/4, TOTAL 29/29, 0 failed, exit 0** | **synthetic 25/25, qualified 4/4, TOTAL 29/29, 0 failed, exit 0** |
| IE canonical SHA-256 | `72cec3badfc4878e5e1166dec8e02d90ef5bf8bd257f3ff258a78ffe29601a63` (31,903 B) | same |
| LU canonical SHA-256 | `7fd8b96308c15b101a74ffd1ae019db3f24ea0ce2abece2302fad0061d2425bf` (29,876 B) | same |
| Synthetic canonical SHA-256 | `471f61e86bdb8700ec4d5695763dc2d4ce3827fa3b7ee56281e3936ab09b88af` | same, **so the CRLF/LF split is gone** |

**Additional gate R2 could not run (full solution).** I made a throwaway `git clone --shared` of the repository object store under `/tmp/r3/clone`, sparse-checked-out `/Tools/`, `/Docs/` and `/Assets/ForestPrototype/{*.cs,Species/}`, and detached at `674f3350`. I copied in the git-ignored `.p0ref` from a sibling worktree (`SitkaLab.Core.dll` sha256 `9a99f781…`, `provenance.txt` `p0QualifiedCommit=a7f41983…`). Results:
- `dotnet build MultiRackSitkaP1Lab.sln`: **0 Warning(s), 0 Error(s)**, exit 0. `SitkaP1.Core.dll` contains the repaired importer (`IntegrateMarteloscopeReader`, `TryExtractPlotSurveyId`, `DeclaredArchiveSha256` and `IsCrownDiameterRow` are all present as metadata strings).
- `Core.Tests`: **291/291 passed, 0 failed**, exit 0.
- My first attempts on a plain export, which had no `.git`, no sibling `Tools/MultiRackSitkaLab` and only partial `Assets/`, failed 7 and then 1 tests. All of those failures were missing-path or `git cat-file` errors caused by my export layout, and none involved woodland code. Each one disappeared once the needed files were present.

## 3. Per-item verdicts

My probe harness (`/tmp/r3/probe`, outside the repo, ephemeral) compiles the candidate's `src/Core/WoodlandImport/**/*.cs` and `SyntheticArchiveBuilder.cs` unchanged: **60 probes, 0 failed**.

| ID | R3 verdict | Evidence (my probes unless stated) |
|---|---|---|
| **R3.1** crown diameter captured as DBH | **CLOSED** | `Crown diameter / crown / 8 / m` alone gives `InventoryOnly` with `DbhCm` Missing, `CrownRadiusM = 4.0` (`Observed`), the note "crown radius = crown diameter / 2; unit=m", and only `MissingRequiredDbh`. The same with `800 cm` gives crown radius 4.0 and DBH Missing. Adversarial cases: type `Crown diameter` with remarks literally `diameter`, and type `Diameter` with remarks `crown diameter`, both give crown radius 4.0 and DBH Missing. `Crown diameter (m)`, `crown-diameter`, `Crown width` and `Crown base height` never populate DBH or height. A real DBH of 32 cm plus a 7.5 m crown diameter gives `Candidate`, DBH 32, crown radius 3.75. Stem rows still map: `DBH 300 mm` → 30, `Diameter at breast height 0.45 m` → 45, `Stem diameter 28 cm` → 28, `Tree diameter 28 CM` → 28, all `Candidate`. Crown radius and crown diameter on the same tree are refused with `DuplicateMeasurementRow@CrownRadiusM`, with an identical result hash in both row orders. A **`CandidateForSimulationIntegration` with a bogus DBH was never produced.** |
| **R5** override replaced computed hash | **CLOSED** | `ResolveAttribution` now always sets `ArchiveSha256 = archive.ArchiveSha256`. Legacy override `"0000-not-the-real-hash"` gives `Refused`, `Stand` null, `ArchiveSha256Mismatch`. A wrong `DeclaredArchiveSha256` gives `Refused`. A correct declared hash that is upper-case and whitespace-padded gives `Candidate`; `ArchiveSha256` equals the computed hash, `DeclaredArchiveSha256` is the normalised computed hash, and the JSON carries both. A legacy-correct hash is accepted. Legacy-correct with declared-wrong, and legacy-wrong with declared-correct, are both `Refused`. An override with no hash, or no override at all, records the computed hash and emits no `declaredArchiveSha256` key, which is why the pinned fixture hashes are unchanged. |
| **NF-01** plain-ID single-plot refused | **CLOSED** | `t1, t2` and `tree_1, tree_2` each give `Candidate` as one stand of 2 trees (`PlotName = "unnamed-plot"`). `1_PlotA, 2_PlotB` with no target gives `Refused` `MultiplePlotsRequireTargetPlotId`; target `PlotB` gives 1 tree (`2_PlotB`). Mixed patterned and plain IDs (`1_PlotA, t2`) give `Refused`, which is fail-closed. |
| **N3** survey index merging epochs | **CLOSED (documented)** | `1_PlotA_1, 2_PlotA_2` with no target gives `Refused`. Target `PlotA_2` gives 1 tree. Target `PlotA` gives `TargetPlotNotFound`, **so the epochs are never merged.** The same epoch (`1_PlotA_2, 2_PlotA_2`) gives one stand of 2 trees. This is documented in the spec §5.3, the reader XML doc and `SOURCE-DATA-QUALIFICATION.md`. Both real fixtures use one plot-survey key each (`IPlus_Donadea_MHF_2`, `IPlus_Mersch`). |
| **NF-02** case-only plot ids merged | **CLOSED** | `1_PlotA, 2_plota` gives `Refused` `MultiplePlotsRequireTargetPlotId`. Target `plota` keeps only `2_plota`. |
| **NF-03** order-dependent refusal report | **CLOSED** | DBH `abc` + `30` in either order gives byte-identical result SHA-256 (issues `DuplicateMeasurementRow`, `MalformedMeasurementValue`, `MissingRequiredDbh` in both). Conflicting DBH 30/55 and status 1/0 are both refused with order-independent hashes. |
| **NF-04** streamed-cap test hit the declared path | **CLOSED** | The new Case B uses a core-only deflated archive with `occurrence.txt` declared as 100 B and asserts the streamed-path message `"Decompressed byte count while reading 'occurrence.txt' exceeded cap"`, which only the streamed path emits. My probe confirms the two paths are distinct: with a lying header and an 8 KiB cap, I get the streamed message. The honest-header control gives `"declared uncompressed size (307983 bytes) exceeds cap"`. |
| **NF-05** empty unit silently assumed | **CLOSED** | An empty unit on DBH, height or crown diameter gives `Refused` `MissingMeasurementUnit` on the right field. `inch` and `ft` still give `UnsupportedMeasurementUnit`. Status and microhabitat rows, which have empty units in the real fixtures, are not affected because they do not go through `ParseDoubleMeasurement`. |
| **NF-06** stale handoff | **CLOSED** | Handoff §2 and §3 are headed "Initial Cycle 1 Baseline — Superseded", and there is an explicit note that `15/15` and `dd0f6cb9…` are historical. The new §6 records 29/29 and the current hashes, which **match my run exactly**. The F-10 root cause is corrected to CRLF/LF in verbatim literals. Residual nit: the §5 F-10 table row still says "zlib builds", but the line right below it and §6 correct this. |
| **F-10 residual** synthetic hash depends on EOL | **CLOSED** | `471f61e8…` on both the CRLF and LF exports (§2). |
| **R3.2 / R3.3 / R4 / F-01..F-09, F-11..F-16** (still-hold probes) | **STILL HOLD** | Orphan ext row gives `ArchiveOrphanExtensionRow@9_GHOST`. Blank key gives `ArchiveBlankExtensionCoreId`. Lying declared size gives `ArchiveEntrySizeMismatch`. A flipped payload byte in a stored entry gives `ArchiveEntryCrcMismatch`. Core `provenance=simulated` gives `SimulatedFieldAtYearZero` on species, position, DBH, height and status. A status row with method `inferred` keeps `Inferred`, so it is not laundered. A `simulated` DBH row is refused. `Quercus`, `Fagaceae`, `Quercus sp.` and `Quercus L.` give `UnidentifiedSpecies`. The suite's earlier F-01..F-11 and XXE tests still pass. |

### Regression checks

| Check | Result |
|---|---|
| Core row with no extension rows | **PASS.** `2_PlotA` with no MoF rows survives (2 trees, `InventoryOnly`) with DBH, height and status Missing and only `Missing*` blockers. |
| Qualified fixtures, same canonical hashes | **PASS.** `72cec3ba…` / `7fd8b963…` on both exports and in the full-clone run. The fixture MoF types (`Tree diameter/diameter/cm`, `tree height/height/m`, `Tree status (dead or living)/status/""`, `Tree related micro-habitat`) are all matched by the new exact classifiers. |
| Test-weakening audit | **PASS.** The only removed lines in the test diff are the old Case B archive and its looser `Any(CapUncompressedBytesExceeded)` check, which were replaced by a stricter streamed-path message assertion. No test was renamed, no tree IDs were changed this cycle, and no assertion was relaxed. |
| Fence | **PASS** (§5) |

## 4. New findings (all non-blocking, fail-closed)

| ID | Severity | Finding |
|---|---|---|
| **NF-07** | Minor | **Unrecognised `measurementType` values are discarded without an issue.** Exact matching closes R3.1 safely, but a stem row spelled differently, such as `Diameter at 1.3 m` 30 cm, is now dropped completely. The tree reports only `MissingRequiredDbh`, and the value does not appear in `ExtraAttributes`. This never produces a wrong admissible stand (it becomes `InventoryOnly`), and dropping unclassified rows also happened before cycle 2 (for example `Crown width`). It does weaken the "inventory retained in full" message. Suggested follow-up: emit an informational `UnrecognisedMeasurementType` issue, or keep the raw row in `ExtraAttributes`. |
| **NF-08** | Minor | **The plot-ID heuristic still false-refuses some generic IDs.** IDs with a numeric prefix and a letter suffix that are not Integrate+ (for example `001_oak`, `002_beech`) are read as two plots and give `Refused` `MultiplePlotsRequireTargetPlotId`, and no `TargetPlotId` can select the whole set. This fails closed and the rule is documented in spec §5.3. A generic archive can avoid it by supplying `locationID`. |
| **NF-09** | Info | `TargetPlotId` matching is now ordinal and case-sensitive (`plotb` does not match `PlotB`; you get `TargetPlotNotFound`). The handoff §6 NF-02 row documents this. Spec §5.3 mentions ordinal distinctness but not target matching explicitly. |

## 5. Fence audit

- `git diff --name-only 6d72c305..674f3350`: **23 files, 0 outside** `specs/multi-2d-woodland-importer-v1/`, `Tools/MultiRackSitkaP1Lab/src/Core/WoodlandImport/`, `Tools/MultiRackSitkaP1Lab/tests/Core/WoodlandImport/` and `Docs/Handoffs/MultiRack-Simulation-20261116/woodland-importer/`.
- `git diff --summary` shows only `create mode 100644`. There are no deletions or mode changes, and no `.sln`, `.csproj`, `Directory.Build.props`, `Assets/`, `ProjectSettings/` or `Docs/Project/` changes.
- I searched `src/Core/WoodlandImport` for `File.`, `Directory.`, `DateTime.Now/UtcNow`, `Stopwatch` and `Environment.TickCount`: **no hits**. The suite's hygiene test also passes.

## 6. Not verified / residual risk

- The real publisher IPT archive and its `meta.xml` are still `PENDING REAL ARCHIVE DOWNLOAD`. The exact classifiers (NF-07) and the plot heuristic (NF-08) will meet real data for the first time there.
- The local-metres reading of `verbatimCoordinates`, the X/Y orientation and both constructed test polygons are still documented assumptions.
- `.p0ref/SitkaLab.Core.dll` was borrowed from a sibling worktree, and I trusted its `provenance.txt` without rebuilding it from P0.
- I did not re-check the fixture rows against live GBIF; the fixture bytes are unchanged since cycle 1.
- Unity, runtime, `/multi`, Save v2/v3 and UI integration are out of scope, and the importer is not wired into any of them (D-030). None of these was tested.

*Independent review only. Nothing was merged, no feature-branch ref was touched, and Unity was not involved.*

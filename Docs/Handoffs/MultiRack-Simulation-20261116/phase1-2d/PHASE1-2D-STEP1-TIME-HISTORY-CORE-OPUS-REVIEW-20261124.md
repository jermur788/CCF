# CCF Multi 2D — Phase 1 Step 1 (Time Pacing, History Query & Woodland Structure Core): Independent Review

**Date:** 2026-11-25 (UTC). The lane name keeps the 20261124 task date.
**Verdict:** **NEEDS CHANGES.** There are 2 bounded repairs, both in test code only. The Core source behaves to spec, and no source change is required.

> **EXPLICIT STATE:** The feature is **NOT MERGED**, **NOT INTEGRATED** and **NOT OWNER-ACCEPTED**. This review ticks no row in `specs/multi-2d-phase1-time-history-core-v1/checklists/requirements.md`. It records evidence for the Overall Manager.

---

## 1. Reviewer identity and separation

| Item | Value |
|---|---|
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), running in Claude Code under T3 Code |
| Role | Independent, non-author reviewer. No implementation role. |
| Implementer | Neon (`antigravity_2` / Gemini 4 Neon), a different provider and a different thread |
| Separation | I did not write, edit or advise on any spec, source, test or handoff in this feature. I spawned no subagents. |
| Writes | Only this file, committed on the review branch. Builds wrote only to the review worktree's git-ignored `bin/`/`obj/`. Mutation and fragility probes ran in a disposable `git clone --shared` under `/home/jer/ccf-p1th-review-tmp/` (outside the repository). That clone was deleted at the end. |

## 2. Exact objects under review

| Item | Value |
|---|---|
| Feature branch | `feat/multi-2d-phase1-time-history-core-20261124-v1` |
| Feature HEAD (reviewed) | `54761edc2ca0f66698182a309eb497ad318f34ea` |
| Base | `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43` (accepted unmerged Save/Load baseline) |
| Review branch | `review/multi-2d-phase1-time-history-core-20261124-v1` at `54761edc`; this file is the only commit on top |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistory-Review-20261124` |
| Toolchain | `~/.dotnet/dotnet` SDK `8.0.134` (`global.json` pins `8.0.133`, `rollForward: latestPatch`). System dotnet 10.x not used. |
| `TMPDIR` | `/home/jer/ccf-p1th-review-tmp` (outside the repository) |

Commits `5438c94c..54761edc` (5): `12e9dcd5` spec quartet · `e3ee9ce0` Core modules · `318d38fe` tests · `a85b31cc` Save.Tests assertion fix · `54761edc` implementation handoff.

## 3. Start-of-review checks (run by me)

| # | Check | Result |
|---|---|---|
| 1 | `git rev-parse HEAD` | `54761edc2ca0f66698182a309eb497ad318f34ea` — **PASS** |
| 1 | `git status --short` | empty — **PASS** (still empty after every gate run, before this file was written) |
| 2 | `sha256sum Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` — **PASS** at start and end. The directory and files are read-only (`r--r--r--`). The DLL was never rebuilt. |
| 3 | `git diff --name-only 5438c94c..HEAD` | exactly the 12 claimed files (4 spec, 4 Core `MultiArea/`, 3 test, 1 handoff) — **PASS** |
| 3 | Paths under `src/Ui/`, `src/Core/Phase1/`, `Assets/`, `Packages/`, `ProjectSettings/` | **0** (grep exit 1) — **PASS**. `git diff --summary` shows 9 creates, all mode `100644`, and no mode changes. The only edit to an existing source file is `MultiAreaContinuation.cs`: `sealed class` → `sealed partial class` (1 line). |
| 4 | `git ls-remote --heads origin main` | `cc2757a730ad9451e3d6e3680dffd123171a5df5` — **PASS** (untouched; re-checked at end) |
| 5 | `git ls-remote --heads origin feat/…-v1` | `54761edc2ca0f66698182a309eb497ad318f34ea` — **PASS** (re-checked at end) |
| 6 | `ss -ltnp` for `:33767`, `:43649`, `:46593` | all `LISTEN` (dotnet PIDs `1750123`, `1806361`, `839989`) at start and at end (§12). I never stopped them. |

## 4. Gates rerun by me (exact commands and counts)

```bash
export PATH="$HOME/.dotnet:$PATH"
export TMPDIR=/home/jer/ccf-p1th-review-tmp
mkdir -p "$TMPDIR"
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistory-Review-20261124/Tools/MultiRackSitkaP1Lab
dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj
dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers
# extra, because `dotnet run` does not print build warnings:
dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --disable-build-servers --no-incremental
```

| Gate | Exit | Observed result |
|---|---|---|
| `.sln` build | 0 | **Build succeeded — 0 Warning(s), 0 Error(s)** (`TreatWarningsAsErrors=true` in `Directory.Build.props`) |
| Core suite | 0 | **277/277 passed, 0 failed** (267 baseline + 10 new `P1-TH-*`). No `FAIL` lines. |
| Save suite (`dotnet run`) | 0 | **50/50 passed, 0 failed** (48 baseline + 2 new). `scratch removed: True`. |
| UI build | 0 | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Save tests project build (extra) | 0 | **Build succeeded — 0 Warning(s), 0 Error(s)** |

Run window: 2026-11-25T00:46:43Z – 00:47:54Z (Save project rebuild later, same result). I confirm the author's counts: 277/277, 50/50, and 0 warnings / 0 errors on all three builds.

## 5. Nine frozen anchors (observed by me)

| # | Anchor | Observed in my Core run | How the suite asserts it | Result |
|---|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | pinned literal (`Model0FingerprintTests.cs`) | **EXACT** |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | pinned literal | **EXACT** |
| 3 | Multi-area `0/0/1/0` (C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | pinned literal (`Stage4Tests.cs`) | **EXACT** |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | pinned literal (`Stage5Tests.cs`) | **EXACT** |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | pinned literal | **EXACT** |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | pinned literal (`Stage5GridStudy.cs`) | **EXACT** |
| 7 | Phase 2 Q6 | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` (both `1/1/1/1/0` and `1/1/1/1/1`) | the suite asserts only h0 == h1; I compared the printed value with the spec table | **EXACT** |
| 8 | P0 Core binary pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | `sha256sum` (start and end) plus the P2-01 pin check | **EXACT** |
| 9 | MA-1-MIXED 30-year | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | the suite asserts only run1 == run2; I compared the printed value with the spec table | **EXACT** |

All nine anchors are unchanged. Anchors 7 and 9 are relational assertions that already existed before this feature, so the suite would not catch drift in them by itself. I checked the printed lines by hand.

## 6. Independent mutation probes (evidence of test strength)

**Method.** I made one disposable `git clone --shared`, sparse-checked-out at `54761edc`: the lab tree, the P0 tree, the 4 linked Unity files and `global.json`. I copied `.p0ref/` in read-only and re-checked its hash (`9a99f781…`). A Python driver then ran each mutation:

1. Apply exact single-occurrence string replacements (asserting each pattern occurs once).
2. Run a clean `--no-incremental` `.sln` build.
3. Run the full Core suite.
4. Restore the files with `git checkout` (restoration verified clean every time).

Every mutated build compiled with warnings-as-errors on. The control run came first.

| Key | Mutation (Core source) | Result | Caught by |
|---|---|---|---|
| M00 | none (control) | 277/277 | — |
| M01 | overdue fallback removed (post-Fell always `anchor + 4`) | **KILLED** 276/277 | P1-TH-01 "expected 9, got 5" |
| M02 | `AdvanceYears` up-front `HasOpenWork` guard disabled (`MultiAreaTimePacing.cs:257`) | **SURVIVED** 277/277 | — (see F-2) |
| M03 | M02 **and** each step passes `consentToCancelWork: true` | **KILLED** 276/277 | P1-TH-03a "expected WorkPlanOpen, got ReachedReviewYear" |
| M04 | `HasOpenWork` reduced to `root.HasUnexecutedWork` | SURVIVED (equivalent) | — (N-2) |
| M05 | distance tie-break reversed (`CompareOrdinal(b, a)`) | **KILLED** 276/277 | P1-TH-06b tie-break |
| M06 | distance tie-break by roster index instead of `TreeId` | **KILLED** 276/277 | P1-TH-06b tie-break |
| M07 | between-step cancellation check disabled | **KILLED** 276/277 | P1-TH-03b |
| M08 | structure population includes dead stems/stumps | **KILLED** 276/277 | P1-TH-06b exclusions "expected 2, got 4" |
| M09 | index `HasAnnualOutcome` keyed by `YearBefore` | **KILLED** 276/277 | P1-TH-05 |
| M10 | `ReadYear` shows Year-Y Fells under Year Y+1 | **KILLED** 276/277 | P1-TH-05 "expected 6, got 5" |
| M11 | anchor uses the earliest instead of the latest Fell | **KILLED** 276/277 | P1-TH-01 "expected 6, got 1" |
| M12 | `T_i` counts pairs where max DBH = 0 | **KILLED** 276/277 | P1-TH-06b "zero-DBH T is null" |
| M13 | post-Fell `+5` clamp removed | SURVIVED (equivalent) | — (N-1) |
| M14 | a 5-year request refused (`>=` instead of `>`) | **KILLED** 274/277 | P1-TH-02, P1-TH-03b, P1-TH-05 |
| M15 | `ContinuationNoLivingStems` mapped to `AnnualStepRefused` | **KILLED** 276/277 | P1-TH-03b |
| M16 | neighbour search limited to the focal tree's own rack | **KILLED** 275/277 | P1-TH-06a 4-tree, P1-TH-06b 2-tree |
| M17 | `Preview` no longer reports `WorkPlanOpen` | **KILLED** 276/277 | P1-TH-03a |
| M18 | pre-Fell observation target `+5` instead of `+4` | **KILLED** 275/277 | P1-TH-01, P1-TH-02 |

**Summary:** 15 of 17 real mutations were caught. M04 and M13 cannot change any reachable behaviour, so no test could catch them. Only **M02** is a real survivor (F-2).

**P1-TH-08 fragility probes** (same clone, clean rebuild, source unmutated):

| Probe | Change | Result |
|---|---|---|
| A | add an untracked scratch file `src/Ui/REVIEW-PROBE-UNTRACKED.txt`; no tracked change | **276/277**: `FAIL P1-TH-08 … forbidden src/Ui/ edit: …/REVIEW-PROBE-UNTRACKED.txt` |
| B | append one comment line to tracked `src/Ui/Program.cs` (a Step-2-style edit) | **276/277**: `FAIL P1-TH-08 … forbidden src/Ui/ edit: …/src/Ui/Program.cs` |

## 7. Per-item verdicts

| Item | Verdict | Basis (my own evidence unless stated) |
|---|---|---|
| **P1-TH-01** Preview cadence | **PASS** | Read `MultiAreaTimePacing.cs:138-213`. Pre-Fell: `currentYear + 4` (clamped `+5`), `IsObservationPeriod = true`, with the observation label ("not a completed silvicultural thinning cycle") and `CalibrationNote` ("GAMEPLAY CALIBRATION …"). Post-Fell: anchor = max `MultiAreaFellRecord.EcologicalYear` over `Events[].Areas[].Fells`; non-Fell work sets no anchor (a Plant-only event is tested). Overdue (`anchor + 4 <= currentYear`) → `currentYear + 4`. Tested at Y5 (equal case) and Y6 (strict case). `MaxTicksPerPress = 5`. Stale `PlanStateRevision`, source token and session id are refused through `RequireFence` on both APIs with byte-identical root. Mutations M01, M11, M18 caught. |
| **P1-TH-02** Parity oracle | **PASS** | `Canonical()` = `CaptureSave` → `Phase1SaveCodec.Encode`, with only session-derived PlanId/ExecutionKey prefixes and `SavedUtc` normalised. `Phase1RootMapping.Capture` (read) serialises the whole root: areas/trees, events, executed receipts, pending/approved orders, `AnnualHistory`, and the Stage 4/5/Phase 2 states with logs and accounts. This is a full-root comparison, not a shallow field. Both fixtures are covered: MA-1 (run under the Phase 2 `1/1/1/1/1` selection, all-Sitka) and MA-1-MIXED. Each is checked pre-Fell 0→4, post-Fell 4→8 and with a 5-tick press 8→13, each against N direct `Advance(…, false)` calls. M14 and M18 caught. Structurally, `AdvanceYears` loops the unchanged `Advance` under the same lock with a fresh per-step fence (`:313-317`), so it re-implements no biology. |
| **P1-TH-03** Refusals / cancellation | **PASS (behaviour); test gap F-2** | `WorkPlanOpen` refusal is checked first, with zero mutation, for Pending and Approved orders of all 6 families. `consentToCancelWork` is never `true` (`:317`). `InvalidTargetYear` holds for `<= current`, `> current + 5`, `int.MaxValue` and `int.MinValue`, using `long` delta arithmetic. Cancellation is checked only at the top of each loop iteration, before a step starts, so no partial year exists (each `Advance` commits atomically by one reference assignment, `MultiAreaContinuation.cs:1133`). A mid-run cancel after Year 2 equals a 2-year manual control byte for byte, with exactly 2 history records. The stop reasons are exactly the six. M03, M07, M15, M17 caught; M02 survives (F-2). |
| **P1-TH-04** Save compatibility | **PASS** | `src/Core/Phase1/` is untouched (my diff). `SchemaVersion == 2` is asserted in Core and Save tests. The Core test does real Capture → Encode → Decode → `Restore` into a fresh session, pre-Fell (Y2→Y6) and post-Fell (all 6 actions in Y0, Y2→Y4). Preview record equality, index rows, selected year-detail fields, structure bits and the post-skip canonical root all match the uninterrupted control. The anchor (Y0 → target Y4) recovers from saved events. The 2 Save.Tests cases are additive contract round-trips on the existing samples. |
| **P1-TH-05** History query | **PASS** | Recorded years = union of `AnnualHistory` before/after, event years, fell years, PruneLog, ClearLog and WorkLog, sorted newest-first. Keyset `< beforeYear` paging is tested cursor-stable across 2 appended years (17 rows, 4 pages). `WorkYear = Y` and `ObservedYear = Y + 1` are kept separate: Year-0 cuts stay in Year 0, Year 1 shows 0 work, and Year 6 has both its 5→6 and 6→7 records. All six provenance labels exist and are distinct, with D-073 tier strings; five of them are attached to real data (see N-3 for SESSION-ONLY). Canopy history, past tree-by-tree structure and per-area `Phase2PlantedAccount` splits are disclosed as `NOT RECORDED`, and planted accounts appear only with `Scope = "WholeWoodland"`. The non-causal wording is exact, with no weather wording. No economy/money/price/objective/euro fields exist: reflection check over 19 DTOs plus my grep of the three new files. M09 and M10 caught. |
| **P1-TH-06** Structure metrics | **PASS** | The `M_i`/`T_i` code matches spec §6.2: `k = min(3, N-1)`; squared Euclidean distance in `double`; ties broken by ordinal `TreeId`; living adults only (`IsLiving`), across all racks; species from `Phase2.TreeSpecies` (broadleaf only, else Sitka); `T_i` over valid pairs with `max > 0`, null when `m_i = 0`. **I re-derived every oracle by hand:** 4-tree (M 5/6, T 11/24 per area and woodland), 6-tree (A 4/9, 1/2; B 7/9, 19/36; woodland 11/18, 37/72; genuine nearest-3 selection across the x = 10 border), 2-tree (1, 0.5), 3-tree (2/3, 1/3), tie fixtures (four trees at exactly d² = 4: 1.0/0.5 and 2/3, 1/3). They are genuinely analytical. 0/1-tree stands → null with `EffectiveNeighbourCount 0`. Zero-DBH → `T` null with `M` still 1. Stumps, standing dead, deadwood and PJ (at 0.02–0.1 m) are excluded. `Compute` is pure: byte-identical canonical root before and after. M05, M06, M08, M12, M16 caught. |
| **P1-TH-07** Honesty / G3 labels | **PASS** | Exact `CalibrationNote`, observation label, non-causal statement and structure honesty label are asserted. The `TimePacingStopReason` enum has exactly 6 names; none of the 5 forbidden 3D values exists. My grep of the three new source files found no weather, storm, objective, teaching, unread-review, year-25/100, euro or "port" claims. The handoff does not claim a 3D code port. |
| **P1-TH-08** Scope | **Criterion PASS; test implementation NEEDS CHANGES (F-1)** | My `git diff --name-only 5438c94c..HEAD` shows zero forbidden paths. The *test* that encodes this criterion, however, fails on legitimate future changes and on checkout state (probes A/B). |
| **P1-TH-REG** | **PASS** | §4 counts, §5 anchors, listeners alive, `.p0ref` exact. |
| **Gate G2** (behaviour/acceptance safety: refusals, zero mutation, save continuity) | **PASS** | As P1-TH-01/03/04 above. |
| **Gate G3** (honesty and measurement integrity) | **NEEDS CHANGES** | The honesty labels pass. Measurement integrity fails on F-1: a permanent test whose result depends on git state and untracked files, not on the code under test. F-2 is a smaller test gap. |

Other checks requested in the brief:

- **Concurrency/locking — PASS by inspection.**
  - `PreviewNextReviewYear` and `AdvanceYears` take `lock (sync)` and call `RequireFence` inside it.
  - `AdvanceYears` keeps the lock across all ≤ 5 steps, and the nested `Advance` re-enters the same monitor.
  - `ReadHistoryIndex`, `ReadHistoryYear` and `ComputeStructure` read the `root` reference under the lock and compute over that snapshot. This is safe because every commit publishes a new root by reference assignment with fresh arrays (`MultiAreaContinuation.cs:1133-1152`). `Phase2Annual.Step` copies `TreeSpecies` into a new dictionary (`Phase2Annual.cs:65,214`) instead of mutating a published one, and `areas` is a readonly immutable list.
  - I did not run a multi-threaded stress test (see §11).
- **Mutation on refusal — PASS.** Every refusal path the tests exercise (fence ×3 per API, WorkPlanOpen ×12, InvalidTargetYear ×7, pre-cancel, empty stand) is byte-compared or history-count-checked. By inspection, refusals return before `Advance` is called or come from `Advance` before its commit.
- **Determinism — PASS.**
  - No RNG or wall clock in the new files. `FormattableString.Invariant` is used for detail text.
  - The `HashSet<int>` years are ordered with `OrderByDescending`, and the `Dictionary` instances are used only for lookups.
  - Floating-point sums run in fixed list order.
  - One exposed dictionary is a nit (N-4).
- **API surface honesty — PASS.** Grep shows the history and structure types are called only from their own thin wrappers. Nothing in `Advance`, `Execute`, `Approve`, `Restore` or `src/Ui/` uses them.
- **Hidden UI/HTTP dependency — none.** The three new files have no `using` directives, no `System.IO`/`File`/`Directory`, and no ASP.NET types.

## 8. Findings

| ID | Severity | File:line | Observed behaviour | Evidence |
|---|---|---|---|---|
| **F-1** | **major** | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs:762-785` (plus `:17` `BaseCommitSha` and the `:1075-1098` `FindRepoRoot`/`RunGit` helpers) | P1-TH-08 is a **permanent Core regression test**. It runs `git diff --name-only 5438c94c` (the *working tree*, including uncommitted edits, against a fixed historical commit) plus `git ls-files --others --exclude-standard`. It fails on any path under `src/Ui/`, `src/Core/Phase1/`, `Assets/`, `Packages/`, `ProjectSettings/` or `Docs/Project/coordination/`. So the suite's result depends on checkout state and on the history of other lines, not on the code under test. | Probe A (one untracked scratch file in `src/Ui/`) → 276/277. Probe B (one comment line in `src/Ui/Program.cs`, exactly what the planned Step 2 UI wiring will do) → 276/277. By code reading: the test also throws, and so fails, in any clone without git or without the `5438c94c` object (shallow/CI), and on any later tree whose protected paths differ from `5438c94c`. No baseline test diffs against a fixed branch SHA (grep: this is the only `diff --name-only`). The author disclosed the Step 2 hazard (handoff §6, risk 3) but shipped the test anyway. Scope compliance itself is satisfied (§3); the defect is that a one-time review gate was encoded as an environment-dependent permanent test, so the next legitimate step cannot report a clean suite without editing it. |
| **F-2** | minor | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs:889-911` (guard under test: `src/Core/MultiArea/MultiAreaTimePacing.cs:257-271`) | The tests do not pin `AdvanceYears`' own `WorkPlanOpen` refusal. With the up-front guard disabled, the engine's `WorkPendingConsentRequired` refusal is mapped to `WorkPlanOpen` (`:324`), so every assertion still passes. Two things are unasserted: spec §4.5 ordering (open work is checked *before* target bounds, so open work plus an invalid target must report `WorkPlanOpen`), and the `StopDetail` label. Runtime behaviour is correct today; this is a test gap, not a source defect. | M02 survived 277/277. M03 (guard disabled **and** consent `true`) is caught. |
| N-1 | nit | `MultiAreaTimePacing.cs:153` | The post-Fell `Math.Min(candidate, currentYear + 5)` clamp can never bind: a recorded Fell year is ≤ the current year, so `anchor + 4 ≤ currentYear + 4`. The spec mandates the clamp; it is harmless. No change. | M13 equivalent |
| N-2 | nit | `MultiAreaTimePacing.cs:122-125` | The `ApprovedPrunes`/`ApprovedClears`/`ApprovedOrders` clauses in `HasOpenWork` cannot be reached through public commands, because `Approve` always sets `root.Approved` (`MultiAreaContinuation.cs:756-776`) and `HasUnexecutedWork` already covers that. Defensive only; no change. | M04 equivalent |
| N-3 | nit | `MultiAreaHistoryQuery.cs:21,72-74` | `SESSION-ONLY` is vocabulary only in Step 1: it appears in the glossary and the canopy `NOT RECORDED` text, but no output value carries it. This is honest (nothing is fabricated). If Step 2 surfaces the session canopy/light read-out next to history, it should tag it `SessionOnly`. | code reading |
| N-4 | nit | `MultiAreaHistoryQuery.cs:291,851-884` | `NaturalCohortAbundanceBySpecies` is a `Dictionary`. Its enumeration order is insertion order in practice but is not guaranteed. Step 2 serialisation should emit keys in ordinal order. | code reading |
| N-5 | nit | `MultiAreaHistoryQuery.cs:962-981`, `WoodlandStructureMetrics.cs:245-250` | The read APIs take no fence and return no source token, session id or `PlanStateRevision`. They are safe (an immutable snapshot taken under the lock), but a Step 2 HTTP surface should attach the token so a page can prove which root a history page or structure summary came from. | code reading |
| N-6 | nit | `MultiAreaTimePacing.cs:250-358` | `AdvanceYears` holds `sync` across up to 5 annual steps, so all reads wait meanwhile. The spec mandates this and the author disclosed it. | code reading |
| N-7 | nit | `Phase1TimeHistoryTests.cs:938-955` | P1-TH-04 compares `ReadHistoryYear` only on selected fields. The risk is low: `ReadYear` is a pure function of the root, and the full canonical root is byte-compared. | code reading |

**Count by severity:** blocker 0 · major 1 · minor 1 · nit 7.

## 9. Repair list (one implementer pass, test code only)

1. **(F-1)** Remove the git-dependent working-tree/untracked scan from P1-TH-08 (`Phase1TimeHistoryTests.cs:762-785`) and the now-unused `BaseCommitSha`, `FindRepoRoot` and `RunGit`. Replace it with a hermetic assertion that depends only on the built code. For example: `Phase1SaveFormat.SchemaVersion == 2`, and the `SitkaP1.Core` assembly references no UI/ASP.NET Core assembly. Keep path-scope compliance where it already lives: the reviewer/integration gate `git diff --name-only 5438c94c..HEAD` recorded in the handoff. Report the true resulting Core count.
2. **(F-2)** In `VerifyWorkPlanOpenRefused` (`Phase1TimeHistoryTests.cs:889-911`), add assertions for both the Pending and Approved cases:
   - (a) `StopDetail == MultiAreaTimePacingConstants.WorkPlanOpenLabel`;
   - (b) `AdvanceYears(fence, currentYear + 6)` and `AdvanceYears(fence, currentYear)` with open work return `WorkPlanOpen`, not `InvalidTargetYear`, with a byte-identical root.

   Confirm that disabling the guard at `MultiAreaTimePacing.cs:257` (M02) then fails.

No Core source, spec, Save.Tests or handoff change is required. Re-review scope: the `Phase1TimeHistoryTests.cs` diff plus a rerun of the four gates and the anchor check.

## 10. What I verified myself vs what I accepted from the author

**Verified myself:**

- Everything in §3–§7: start checks, all four gates plus the Save project build, all nine anchors, 18 mutations plus 2 fragility probes, hand re-derivation of every structure oracle, and line-level reading of all three new Core files, the test file and both test-program diffs.
- `MultiAreaContinuation.Advance`/`Approve`/`CaptureSave`/`Restore`, `Phase1RootMapping.Capture`, `PlantedJuvenile.IsActive`, and the `Phase2Annual` species-map copy.
- Scope, mode and remote SHAs.

**Accepted from the author without independent proof:**

- The commit narrative and the reason for `a85b31cc`. The diff matches the description, but I did not re-run `318d38fe`.
- The single-writer claim (the feature worktree was not inspected).
- The author's own run timings and `TMPDIR`.

## 11. NOT TESTED

- **Multi-threaded concurrency:** no stress test with concurrent readers and writers. Lock correctness is by inspection only.
- **UI/HTTP/browser:** none exists in Step 1 (out of scope). No live server was started; protected listeners were only observed.
- **Unity Editor/runtime:** not applicable and not run.
- **`AnnualStepRefused` and in-loop `WorkPendingConsentRequired` paths:** unreachable while the lock is held. The mapping was checked by reading only.
- **Integration onto `main`:** not simulated. In this local object store, `cc2757a7` (67 commits, root `159f0ff8`) and `5438c94c` (root `845accd1`) share no merge base (possibly a shallow or grafted local history; not investigated). The integration route belongs to the Manager.

## 12. End-of-review state (2026-11-25T01:13:47Z)

- Review worktree HEAD `54761edc2ca0f66698182a309eb497ad318f34ea`. `git status --short` shows only this new file before the commit.
- `.p0ref/SitkaLab.Core.dll` = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.
- Listeners `127.0.0.1:33767` (pid 1750123), `:43649` (pid 1806361) and `:46593` (pid 839989) all `LISTEN`.
- `origin/main` = `cc2757a730ad9451e3d6e3680dffd123171a5df5`; `origin/feat/…-v1` = `54761edc2ca0f66698182a309eb497ad318f34ea`. Neither was touched.
- Disposable mutation clone deleted. Only logs remain in `/home/jer/ccf-p1th-review-tmp/` (3.3 MB, outside the repository).

## 13. INTEGRATION DECISION

```text
INTEGRATION DECISION
Verdict:          NEEDS CHANGES
Feature:          MULTI-2D-PHASE1-TIME-HISTORY-CORE-V1 (SPEC-multi-2d-phase1-time-history-core-V1)
Reviewed HEAD:    54761edc2ca0f66698182a309eb497ad318f34ea (feat/multi-2d-phase1-time-history-core-20261124-v1)
Base:             5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43
Scope:            Core-only Step 1 — MultiAreaTimePacing.cs, MultiAreaHistoryQuery.cs, WoodlandStructureMetrics.cs,
                  1-line partial edit to MultiAreaContinuation.cs, Phase1TimeHistoryTests.cs (+1 registration line),
                  2 additive Phase1.Save.Tests cases, spec quartet, implementation handoff. Zero src/Ui/, src/Core/Phase1/,
                  Assets/, Packages/, ProjectSettings/ edits.
Gates (reviewer): .sln 0W/0E; Core 277/277; Save 50/50; Ui 0W/0E; Save project 0W/0E; 9/9 anchors exact;
                  .p0ref 9a99f781…34b3; listeners 33767/43649/46593 alive.
Source behaviour: Conforms to spec §4–§6 (P1-TH-01..07 PASS; G2 PASS).
Conditions:       Repairs 1–2 (§9) in tests/Core.Tests/Phase1TimeHistoryTests.cs only; then bounded re-review of that diff
                  plus gate rerun. No Core source, schema, UI or anchor change permitted or needed.
Repair count:     2 (1 major F-1, 1 minor F-2)
Merge/PR/tag:     NONE performed. main untouched.
State:            NOT MERGED · NOT INTEGRATED · NOT OWNER-ACCEPTED
```

The feature is **NOT MERGED**, **NOT INTEGRATED** and **NOT OWNER-ACCEPTED**.

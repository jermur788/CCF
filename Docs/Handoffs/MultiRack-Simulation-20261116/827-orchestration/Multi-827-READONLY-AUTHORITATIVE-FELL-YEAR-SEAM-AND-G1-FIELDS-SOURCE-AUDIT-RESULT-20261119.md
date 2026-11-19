# Multi #827 — Read-only authoritative Fell, following-year seam and G1 field source audit (result)

**Date:** 19 November 2026
**Issue:** [#827](https://github.com/jermur788/CCF/issues/827) · orchestration parent #834 · Manager #730 (not notified or edited by this audit)
**Result type:** SOURCE AUDIT ONLY. No implementation approval. No build, test, .NET, Python harness, Unity, browser or game run. No Unity/native verification. No V-01/V-02 waiver. No L4 FAIL38 waiver. No Stage 5 checklist box is checked.

## Headline verdict

**G2 BLOCKED.** The inspected #789 Year2–10 candidate engine has **no player Fell, no Work Plan, no work-event record and no execution identity**. It is a natural-ecology-only, in-memory engine. Natural mortality is clearly identifiable. The only Fell-shaped code found anywhere is the **P0 lab `TestWorkExecuted` event** (public commit `a7f41983`, not on main), which belongs to a different domain, has a calendar-only year step with no ecology, and is refused by the P1 import path. The year-step transaction pattern in `AnnualContinuation` (fenced, compute-then-publish under one lock, fault seam proven by recorded tests) is a sound base for a future Fell seam, but that seam does not exist yet.

## Actor and method

| Item | Value |
|---|---|
| Actor | Claude Opus 5.5 (high), via the T3 claudeAgent pool; independent NONAUTHOR inspector; no subagents |
| Orchestration | parent #834; RUNNING registered at https://github.com/jermur788/CCF/issues/827#issuecomment-7020107710 before any result write |
| Host | owner Windows 11 + WSL2 (Ubuntu); `gh` authenticated as `jermur788` |
| Method | Reading files (`cat -n`, `sed`, `grep`), read-only Git (`rev-parse`, `log`, `status`, `ls-files -s`, `cat-file -t`, `ls-remote` on the local bundle), GitHub REST reads, `sha256sum` of fixture and archive files. No code executed. |
| Writes | This one Markdown on branch `review/multi-827-legal-fell-year-and-g1-field-audit-20261119-v1` (created from live main `852304d8a668d6ee2b04547aaec751be6b462c53`) via the GitHub API, plus #827 comments. No local checkout written. Scratch copies of GitHub-fetched documents were read in WSL `/tmp` only. |
| Live coordination check | Live main `Docs/Project/coordination/active-tasks.md` has no #827 lane entry, so no conflicting lock was found. Not modified. |

## Provenance table

| Role | Worktree path | Branch | HEAD | Dirty | On GitHub? |
|---|---|---|---|---|---|
| #789 Year2–10 engine (primary) | `/home/jer/CCF-Dev/Multi-20261119/y2engine` | `feature/multi-789-year2-10-engine-20261119-v1` | `6f9d2e990201bc47e361c3562be9390ed221baf6` | clean | **No** (branch 404, commit 422). The origin is the local bundle `/mnt/c/CCF-Archives/Multi-20261119/engine/ccf-y2-10-engine-20261119.gitbundle`. 14 commits; the root commit is `4b81e71` "narrow Year1 export from #725 df467dcc". Tracked tree is `Tools/MultiRackSitkaP1Lab/**` only (32 files, **no `Assets/`**). |
| H0A Year0 core | `/home/jer/CCF-Dev/Multi-20261119/core` | `feature/multi-h0a-core-year0-admission-20261118-v1` | `c4124b63252fb563dc7d84c9fc7fc4e3e0d8d2d1` | untracked `Tools/MultiRackSitkaYear0Viewer/fixtures/` (not touched) | No (404) |
| H0A Year0 viewer | `/home/jer/CCF-Dev/Multi-20261119/viewer` | `feature/multi-h0a-a-year0-viewer-20261118-v1` | `f3537fe37305199a55a06882e32a81487c53e540` | clean | No (404) |
| #811 history viewer | `/home/jer/CCF-Dev/Multi-20261119/_811/history` | `feature/multi-811-offline-history-viewer-20261119-v1` | `896132373542023ccbd0e4ed9d96826202ac4e81` | clean | No (404) |
| #814 candidate marking | `/home/jer/CCF-Dev/Multi-20261119/_814/marking` | `feature/multi-814-local-candidate-marking-20261119-v1` | `e8e1d37682c1fc8ace197c189dce9bd2a55a2b0b` | clean | No (404) |
| P0 lab (Fell reference) | GitHub only | commit `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` ("#567 post-save HTTP response repair handoff") | subtree `Tools/MultiRackSitkaLab` = `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (matches `.p0ref/provenance.txt` `p0QualifiedTree`) | n/a | Yes, as a commit. `Tools/MultiRackSitkaLab` is **not on main**. |
| CCF pinned source | GitHub | `90583811c4fb88508a4bf05c14365732e24a21ac` (`CcfSourcePins.InspectedCommit`) | — | n/a | Yes |
| Live main | GitHub | `main` | `852304d8a668d6ee2b04547aaec751be6b462c53` | n/a | Yes. `Tools/MultiRack*` is absent from main. |
| Archive | `/mnt/c/CCF-Archives/Multi-20261119` | — | — | — | Local. `sha256sum -c SHA256SUMS`: **9/9 OK** (engine bundle `4733de42…`, viewer bundle `9e6e2904…`, tars, MANIFEST, RESTORE, HANDOFF). |
| Stage 5 design | GitHub `docs/multi-730-stage5-approved-design-gates-20261119-v1` | `specs/multirack-sitka-2d-intervention-workflow-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}` | — | — | Read: spec.md and requirements.md in full; plan/tasks fetched, not line-cited |
| #827 dispatch | GitHub `docs/multi-827-g2-and-g1-source-readonly-audit-packet-20261119-v1` | `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-827-MANAGER-G2-AUTHORITATIVE-FELL-AND-G1-FIELD-READONLY-SOURCE-AUDIT-DISPATCH-20261119.md` | — | — | Read in full |

Unless stated otherwise, paths below are relative to `y2engine/Tools/MultiRackSitkaP1Lab/` at `6f9d2e99`. `P0:` means `Tools/MultiRackSitkaLab/src/Core/` at GitHub commit `a7f41983`. `main:` means `Assets/ForestPrototype/` at `852304d8`.

**Mirror identity:** every `src/Core/*.cs` file and the fixture in y2engine has the **same Git blob ID** as the copies under `_811/history/Tools/MultiRackSitkaHistoryViewer/build-inputs/` (e.g. `AnnualRules.cs` `dc299b32…`, `AnnualContinuation.cs` `a452fb3c…`, fixture `8bf81a43…`). The mirrored `ccf/SitkaGrowthModel.cs` (`e4c73402…`) and `ccf/SimulationRandom.cs` (`bb83d7f8…`) have the same blob IDs as main and as the `CcfSourcePins` pins. The findings therefore apply equally to the engine copies inside #811/#814.

## Evidence table

| # | Question / seam | Decisive evidence (file:line, read) | Status |
|---|---|---|---|
| 1a | **Genesis roster: exactly 100 IDs** | Fixture `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`: `treeCount:100`, 100 `treeId` keys, `P30-001`…`P30-100`; file sha256 `bcaaa5c3…` and 32567 bytes = catalog `TrustTables.cs:99-102`; contentHash `b7dee8a3…` = `TrustTables.cs:102`. Parse rules: `DerivedManifest.cs:291-294` (count equals array length, 1..10000), `:332-333` (IDs unique **and** strictly ordinal-ascending, case-sensitive), `:335` (v1 rows must be `live`), `:318` (no duplicate XY), `:324-325` (content hash plus byte-exact canonical form). The exactly-100 / `P30-NNN` rule is **not** in the engine; it lives in the viewers: `core/Tools/MultiRackSitkaYear0Viewer/core/CONTRACT.md:61-62` (`RosterCountMismatch`, `RosterIdInvalid`). | **PROVED (local candidate)** for Genesis identity. The engine itself would accept any count from 1 to 10000. |
| 1b | **Admission path** | `AnnualEngine.cs:45-49` `Open` → `FixtureAccess.ReadDerived(…, TrustUse.RequireAccepted)`; `FixtureAccess.cs:104-123` (catalog record chosen by ID; domain; Accepted status checked before the file is opened); `:129-139` (size, sha256, strict parse, embedded ID, content hash); `TrustTables.cs:104` `ReviewStatus.Accepted`; `AnnualEngine.cs:51-57` (keeps the domain/catalog refusals and swallows only the `AnnualEngineNotImplemented` sentinel from `FixtureAccess.cs:158`). | **PROVED (local)** |
| 1c | **float32 vs float64 representation** | Authoritative per-tree values are f32 bit words: `F32.cs:6-7,18-27` (8 lowercase hex digits; NaN/Inf refused); `DerivedManifest.cs:77-85` (conversions), `:339-342` (derived bits re-checked). Engine state uses `float`: `AnnualEngine.cs:9-10`. Stand aggregates are `double`: `AnnualEngine.cs:17-18`, `AnnualRules.cs:102-118` (BA = Σdbh²·π/40000/areaHa in double). The viewer contract reserves f64 for aggregates only (`CONTRACT.md:80`). #811 admission reads stand metrics as f64 bit words (`_811 …/web/history-admission.js:184-187`). | **PROVED (local)** |
| 1d | **AfterMortality / current source-year commit** | The engine has **no `sourceKind`, `Genesis` or `AfterMortality` type**. The committed per-year state is `ContinuationSnapshot(AnnualState, Revision)` (`AnnualContinuation.cs:3`), which is **`internal`** (`:6`) and reached only through `AnnualEngine.ContinueInMemory()`, itself `internal` and commented "Private experiment seam: no CLI route or public continuation contract" (`AnnualEngine.cs:66-70`). The public `AnnualEngine.Advance()` runs **one year only** and refuses once the year is not 0 (`AnnualEngine.cs:74-80`). The Genesis/Before/PostGrowth/AfterMortality labels are projections made by the #811 exporter (`_811 …/src/Exporter/Program.cs:165-186,260-263`) and admission (`history-admission.js:55,361-383`). The exporter reaches the internal seam by compiling the engine sources into its own assembly (`Program.cs:11`). | **GAP.** There is no public, committed current-source frame API. "Committed" here means in memory only (`AnnualContinuation.cs:44`). |
| 1e | **Stale / uncommitted refusal** | `AnnualContinuation.cs:31-34` (`ContinuationYearMismatch`, `ContinuationRevisionMismatch`). Fault seam before publish: `:43-44`. Recorded tests: `tests/Core.Tests/ContinuationTests.cs:76-97` (fences, one concurrent winner), `:99-115` (injected fault publishes nothing). Not executed by this audit. | **PROVED (source + recorded tests), annual step only** |
| 2a | **Player Fell: any authoritative path?** | **None in the #789 engine.** Program `src/Cli/Program.cs:14-85` has the commands `generate-manifest`, `verify-derived`, `inspect-original` and `annual` only. The only write paths are `AnnualEngine.cs:84` and `AnnualContinuation.cs:44`, and both publish ecology results only. A grep of src/tests for fell/harvest/work plan/intervention finds only the import refusal `FixtureAccess.cs:67` ("felled trees are unsupported in an original import") and the disclosure `Domain.cs:50` ("ANNUAL ADVANCE AND WORK REFUSED"). `AnnualContinuation.cs:5` says it is a "no-intervention experiment". The viewers state the same: `_811 …/README.md:51`; `_814 …/web/candidate-marks.js:1-12` ("never fells a stem … or grants work/plan/save authority"); `_814 …/README.md:64`. | **GAP — absent. G2 BLOCKED.** |
| 2b | **Only Fell-shaped code (P0, other domain)** | `P0:Models.cs:42` `TreeStatus { Live, Dead, Felled }`; `:48` `PlanStage {Draft, Proposed, Approved, PendingWork, Executed, Rejected, Cancelled, Stale}`; `:50-64` `EventType` (incl. `TestWorkExecuted`, `PlanExecutionRejected`). `P0:Engine.cs:239-254` sets `Status = Felled` for every plan ID and bumps `ForestRevision`; `P0:LabSession.cs:228-245` `ExecutePlan`; `:247-253` the step is "calendar only: no age, DBH, height, mortality or recruitment change". P1 refuses felled P0 trees on import (`FixtureAccess.cs:67`) and refuses annual work for the original domain (`FixtureAccess.cs:150-151`). | **NOT USABLE AS-IS.** Reference pattern only (ADAPT); see appendix. |
| 2c | **Natural mortality separate and identifiable** | `AnnualEngine.cs:7` `AnnualDeath(TreeId, EcologicalYear, Cause, Mechanism)`; `AnnualRules.cs:49` `("self-thinning","hazard")`; `:138` `("self-thinning","boundary")`; dead rows are carried frozen at `AnnualRules.cs:62-63`. `Cause` is a free `string` with no enum and no Fell value. | **PROVED** for natural mortality. A Fell cause **does not exist**. |
| 3 | **Work Plan atomicity** (whole-batch validation, all-or-nothing, rollback, refusal of duplicate/missing/dead/stale IDs, write ordering) | No Work Plan, batch, order or event list exists in the #789 engine. The closest analogues are: (i) the annual step, which computes the whole next state and publishes it with one reference assignment under a lock (`AnnualContinuation.cs:28-46`), so a fault before publish leaves zero mutation (`ContinuationTests.cs:99-115`); (ii) `P0:LabSession.cs:108-142`, where events apply to a cloned snapshot and `Commit()` swaps state, appends events and updates the hash together, plus `P0:Engine.cs:57-96` whole-plan prechecks (duplicate, missing, not-live, below-threshold, unreserved, revision, epoch); (iii) the #814 JS layer, which does whole-batch validation of local annotations before mutation (`candidate-marks.js:210-234`). That layer is non-authoritative. | **GAP.** Pattern exists; Fell transaction absent. |
| 4a | **Once-only / execution identity** | No execution key, batch revision or result map exists. The annual step has a `(expectedYear, expectedRevision)` fence (`AnnualContinuation.cs:31-34`). A repeated identical call is **refused** (`ContinuationYearMismatch`), not returned as a no-op (`ContinuationTests.cs:114`). P0 refuses re-execution with `PlanAlreadyExecuted` (`P0:Engine.cs:62`), also not a no-op. | **GAP.** The spec's identical-key no-op replay (R-15/X-14) is not provided anywhere. |
| 4b | **Zero-live terminal edge** | `AnnualContinuation.cs:35-36` refuses `ContinuationNoLivingStems`; `AnnualRules.cs:111` returns zero stand metrics without dividing by zero; `:124` boundary loop stops at one survivor. Recorded tests: `ContinuationTests.cs:55-74` (empty and archived-only refuse; a genuine natural transition to zero is retained). | **PROVED for annual advance. GAP for Fell** (no policy on felling the last live stem). |
| 5a | **Year y intervention → distinct Year y+1 ecology on survivors** | Year advance takes only living stems in ordinal order (`AnnualRules.cs:16`); CI skips dead stems (`:73`); growth `:18,84-100`; mortality hazard plus boundary `:27-59,120-142`; dead rows frozen `:62-63`. **No input carries a Fell or other intervention into this step.** The only way to remove a stem is natural death inside `Calculate`. | **GAP.** The ecology half is ready to consume a reduced survivor roster. The intervention half is missing. |
| 5b | **RNG determinism** | `AnnualRules.cs:43-44` `SimulationRandom.Roll(RngModelVersion, "ADULT-MORT-"+TreeId, year, simulationSeed)`, keyed by tree ID, year and seed rather than draw order. `main:SimulationRandom.cs:41` `Roll(int model, string id, int year, int simulationSeed)`. Removing a stem therefore does not change survivors' rolls, though it does change their CI, density pressure and boundary order. | **PROVED (source)** |
| 5c | **Protected Year0–10 no-work oracle, separable** | `ContinuationTests.cs:11` records 10 per-year digests (Year 1 `df2d44fd…` … Year 10 `10d31913…`); `:18-42` checks sequential repeat-identical bytes. The #811 exporter enforces `revision == year` (`Program.cs:171-186`) and the test asserts it (`ContinuationTests.cs:32`). This audit did **not** execute these tests. | **PROVED as recorded source oracle. Not re-executed.** Note: any future Fell that bumps `Revision` without changing the year would break the `revision == year` invariant used by the exporter and tests. That is a design choice the Manager must settle. |
| 6a | **V-01 compile-linked public inputs** | `src/Core/SitkaP1.Core.csproj:26-29` compiles `$(CcfRepoRoot)Assets/ForestPrototype/SitkaGrowthModel.cs` and `SimulationRandom.cs`. `Directory.Build.props:18` sets `CcfRepoRoot = ../../` (the worktree root). The y2engine worktree has **no `Assets/` directory** (`git ls-files` = 32 Tools files), so this candidate **cannot build on its own**: it needs an external overlay or full-repo mirror. Public blob IDs match the pins and main exactly (`CcfSourcePins.cs:16-19`; main tree `SitkaGrowthModel.cs e4c73402…`, `SimulationRandom.cs bb83d7f8…`). The P0 binary `.p0ref/SitkaLab.Core.dll` (sha256 `9a99f781…`, `csproj:7,15-20,31-37`) is **git-ignored** (`.gitignore:7-8`), untracked and local. | **GAP (unresolved, unwaived).** Agrees with #804. |
| 6b | **V-02 scratch / qualification provenance** | #811 `build-inputs/MIRROR-MANIFEST.txt:1-17` lists every input as `sealed-baseline|…` (a scratch mirror source, not the candidate branch). The blobs match y2engine, but no native reconstruction from the narrow lane was inspected. The archive checksums verify (9/9 OK); the bundle content was not unpacked. | **GAP (unresolved, unwaived).** |
| 6c | **#804 / L4** | #804 closed. Manager #730 verdict (https://github.com/jermur788/CCF/issues/804#issuecomment-7005938037): **PARK FOR DESKTOP / MAIN BLOCKED**, V-01 and V-02 unresolved, **L4 FAIL38 UNWAIVED**. No candidate branch exists on GitHub (all five are 404). | **Status unchanged. Not waived or downgraded.** L4 not evaluated here. |
| 7 | **G1 field inventory** | See G1 section. | **GAP for multi-area** (no area identity) |
| 8 | **Reuse appendix** | See appendix. | Source-similarity judgement only |

## Smallest remediation shape for Manager decision (design choice, NOT approval)

These are the precise open design choices that a G2 contract revision would need to settle before any code. They are offered as options, not decided:

1. **Writer.** Add one Fell transaction beside `AnnualContinuation.Advance`, under the same `sync` lock and the same compute-then-single-publish pattern (`AnnualContinuation.cs:28-46`). It would validate the whole batch against the current snapshot: exact case-sensitive ID present, `IsLiving`, ordinal-unique, expected year and revision. It would build the full next snapshot plus exactly one immutable event, then publish both in one assignment, with a fault seam before publish. P0's clone/commit `Tx` (`P0:LabSession.cs:108-142`) and `CheckExecute` (`P0:Engine.cs:57-96`) are the closest existing precedents.
2. **Cause.** Add a distinct Fell cause instead of reusing the free string `"self-thinning"` (`AnnualEngine.cs:7`, `AnnualRules.cs:49,138`). Natural death and player Fell must be distinguishable by type, not by convention.
3. **Revision/year invariant.** Choose either (a) Fell increments `Revision` within the same `EcologicalYear`, which means amending the `revision == year` assumption in tests and exporter (`ContinuationTests.cs:32`; `_811 Program.cs:178,185`), or (b) a separate work-revision counter that leaves the protected no-work digests untouched.
4. **Execution identity.** Keep a session-local map from execution key to payload digest and result: an identical key returns the stored result, a changed payload refuses. Nothing like this exists today.
5. **Eligibility threshold.** The fixture carries `actionEligibleDbhThresholdMm = 170` (`DerivedManifest.cs:150,267-268`; fixture value 170), and P0 refuses below-threshold Fell (`P0:Engine.cs:88,93`). The Stage 5 spec does not say whether 170 mm binds Stage 5 Fell. This is an **open product question**; no threshold is invented here.
6. **Public surface.** The continuation is `internal` (`AnnualEngine.cs:66-70`, `AnnualContinuation.cs:6`). Any Stage 5 consumer needs an approved public contract rather than more `InternalsVisibleTo` or source-compiling exporters.
7. **Management scope ID.** Add a V1 singleton scope value to the work and event records (spec §5). No such field exists today.

## G1 field / scope inventory (inventory only — no predicate, threshold or fixture invented)

| Field | Present? | Source, units, precision | Missingness / note |
|---|---|---|---|
| Tree ID | Yes | `DerivedManifest.cs:9`, safe ID ≤64 (`:65,331`); fixture `P30-001…P30-100` | Unique, ordinal, case-sensitive |
| XY position | Yes | Integer mm `xMm,yMm` in [0,30000) (`DerivedManifest.cs:10-11,334`), plus f32 centred `unityX/unityZ` (`:20-21,79,341-342`); engine `XM,ZM` float (`AnnualEngine.cs:9`) | Duplicate XY refused (`:318`) |
| DBH | Yes | int mm (`:14`) and f32 cm (`:17,77`); engine `DbhCm` float | Domain [1,200] cm (`:61,337`) |
| Height | Yes | int mm and f32 m (`:15,18,78`) | [0.1,60] m |
| Crown radius | Yes, synthetic | f32 m from a linear rule (`:55,80-85`); grows per year (`AnnualRules.cs:94-98`) | Stated to be not measured (`DerivedManifest.cs:55`) |
| Age | Yes | int years (`:13`); fixture uniform age 30 (`ManifestGenerator.cs:30`) | All trees are one cohort, so age cannot discriminate between areas |
| Status / death | Yes | `status` must be `live` at genesis (`:335`); `AnnualDeath` year/cause/mechanism (`AnnualEngine.cs:7`) | No Fell cause |
| Competition | Computed, not stored | Hegyi CI, 8 m cutoff, 0.5 m distance floor (`AnnualRules.cs:6-7,67-82`); per-tree `Neighbours`, `PreGrowthCi`, `Suppression` in the audit row (`AnnualEngine.cs:20-21`) | Report-only diagnostics, not part of the source state |
| Density / BA | Computed | `AnnualStand` count, stems/ha, BA m²/ha, Dq, RD (double) (`AnnualEngine.cs:17-18`, `AnnualRules.cs:102-118`) | Whole-stand only; there is no per-area or local BA |
| Species | Constant | `"Sitka"` (`DerivedManifest.cs:52,264`), stage `MatureEquivalent`, class `Reference` | Single species, so it cannot discriminate |
| Site / terrain / wind | **Not present** | `siteProductivity` fixed at 1.0f (`:50,262`); elevation 0 (`:229`); storm excluded (`:58-59`) | **UNKNOWN / absent.** Must not be fabricated. |
| Area / management-area identity | **Absent** | One fixture = one 30 m square polygon (`:117-128,232-240`), `areaHa` f32 `3db851ec` (`:46`); `DerivedFixtureId` is a fixture identity, not a management-area identity | **No stable distinct management-area ID exists.** Only one derived fixture is catalogued (`TrustTables.cs:97-107`). `P1Domain` (`Domain.cs:7-15`) is a source-claim domain, not an area. |

**Multi-area comparability:** the inspected source **cannot** support one sample plus at least two comparable target areas plus a mismatched/unknown control. That needs a new, independently validated larger fixture with explicit area IDs and some real between-area discriminating attributes. Today site, terrain and wind are absent, species and age are constant, and competition and BA are derived. **#828 remains DRAFT.** This is an inventory, not a G1 comparator assessment.

## Appendix — REUSE / ADAPT / EXCLUDE against actual main types

These are **source-similarity judgements from grep-level inspection** of headers, usings, type declarations and key lines at main `852304d8`. They are **not an ABI or compile proof**. Blob IDs on main are unchanged from pinned commit `90583811` for all nine files.

| main path (blob) | Coupling evidence | Verdict | Reason |
|---|---|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` (`f1a34b44`) | `using UnityEngine` (`:2`), `[Serializable]` (`:28`), `public Vector3 worldPosition` (`:38`), `ScenarioWorkType.FellTree` (`:6`), `FellingMaterialOutcome` (`:21,40`) | **ADAPT (concepts only)** | The order/status vocabulary (FellTree, open/valid felling `:75`) matches Stage 5 PendingWork. But Unity `Vector3` and 3D world position, plus felling-material/economy fields, conflict with 2D-only scope and G4. Re-express as a plain record keyed by exact tree ID. |
| `…/ScenarioOne/ScenarioManagementEvent.cs` (`ac6c4579`) | `using UnityEngine` (`:2`), `[Serializable]` (`:39`), `Vector3 worldPosition` (`:52`); `ScenarioEcologicalTreatment.TreeFelledAndExtracted` (`:31`) | **ADAPT (concepts only)** | Event type/outcome split and a distinct "felled" treatment are the right ideas for ExecutedWorkEvent and Fell cause. Unity coupling and 3D extraction semantics must not transfer. |
| `…/ScenarioOne/ScenarioManagementHistory.cs` (`a110a3ff`, 569 lines) | `System`, `System.Linq`, `CCF.Forestry.WorkEconomy` (`:1-4`); money and settlement types (`:83-123`); depends on work-order/event types (`:370,499`) | **EXCLUDE (for Stage 5)** | Bound to economy/settlement (G4 deferred) and to Unity-coupled order/event types. The read-only year history view is conceptually relevant to FR-12, but this class brings too much Scenario One scope. |
| `…/ScenarioOne/ScenarioOnePrescriptionApproval.cs` (`c19c953d`, 396 lines) | Header says "Plain data, no UnityEngine" (`:6`); depends on `ScenarioOneCoverage.Stem` (`:4`); manual Fell mark / open order wins over proposals (`:19,191-194,228`) | **ADAPT** | Closest match to Stage 5 proposal → prescription approval → PendingWork with player precedence (D-065). Its coverage model is Scenario One rack/unit-specific (`ConflictKind.UnitNoLongerCovered` `:103`), which G0 forbids. Reuse the precedence and refusal rules, not the coverage dependency. Only relevant once G1 is enabled. |
| `…/ScenarioOne/ScenarioCycleState.cs` (`74047062`, 196 lines) | `System` only (`:1-3`); comment says nothing in the game reads it to decide Work Plan or Advance (`:14`) | **EXCLUDE** | A descriptive review-state projection, not an authority. Stage 5 needs an authoritative transaction. |
| `…/WorkEconomy/ForestryWorkDomain.cs` (`aaafcac2`) | `System` only (`:1`); `[Serializable]` is System's; enums for task type, timber, ledger and cash (`:5-21`) | **EXCLUDE (for Stage 5)** | Pure, but it is the economy domain. G4 forbids new Stage 5 economics, so display UNAVAILABLE instead. |
| `…/WorkEconomy/ForestryWorkCalculator.cs` (`9de22358`) | `System` only (`:1-2`); `static class` (`:6`) | **EXCLUDE (for Stage 5)** | Same reason (G4). It could be reused later if economics is separately approved. |
| `Assets/ForestPrototype/SitkaGrowthModel.cs` (`e4c73402`) — **not** under `WorkEconomy/` | `using System` (`:1`); `static class GrowthModel` (`:14`) and `SitkaGrowthModel` (`:28`) | **REUSE (already linked)** | Pure maths; already compile-linked unchanged by the engine (`csproj:27`). Any reuse must resolve V-01 (an approved explicit build-input scope). |
| `Assets/ForestPrototype/SimulationRandom.cs` (`bb83d7f8`) — **not** under `WorkEconomy/` | `static class` (`:13`); `Roll(model,id,year,seed)` (`:41`) | **REUSE (already linked)** | Pure ID-keyed RNG; supports survivor-independent rolls after Fell. Same V-01 caveat. |
| P0 `Tools/MultiRackSitkaLab/src/Core/{Engine,LabSession,Models}.cs` at `a7f41983` (not main) | No Unity; plan state machine, clone/commit Tx, `TestWorkExecuted` | **ADAPT (pattern reference only)** | The best existing precedent for an atomic whole-plan Fell with event append. But it is a different domain, its year step is calendar-only, and it has a session-epoch model. P1 refuses its felled state. Do not link it into Stage 5. |

## What is NOT proved (strict)

**Overall:** none of the 26 R or 24 X items is evidenced as satisfied. All 50 remain **unchecked / NOT RUN**. The table records whether any source support exists.

| Item | Source support found | Why not proved |
|---|---|---|
| R-01 G0 one area | Partial: one undivided square (`DerivedManifest.cs:117-128`); no rack code found | No UI/workflow exists to witness |
| R-02 separate worlds | Partial: domain plus disclosure (`Domain.cs:7-15,47-50`) | Not qualified |
| R-03 closed input | Partial: Genesis identity proved locally (1a–1c) | No binding to decisions/orders (none exist); no public committed frame (1d) |
| R-04 diagnostics not authority | Partial: viewer admission only | No engine-side current-frame authority |
| R-05 manual per-ID selection | None (only #814 local annotations) | No PendingWork path |
| R-06 sample evidence | None | — |
| R-07 G1 comparator | None | G1 absent; N/A disposition not decided |
| R-08 proposal traceability | None | G1 absent |
| R-09 player judgement | None | — |
| R-10 candidate not Fell | Partial: #814 never fells (`candidate-marks.js:10-11`) | Vacuously true because no Fell exists |
| R-11 ordinary pending orders | None | — |
| R-12 separate commit/execute | None | — |
| R-13 whole-batch revalidation | Pattern only (P0 `CheckExecute`; #814 JS) | Not in the engine |
| R-14 authoritative Fell | **None** | **G2 BLOCKED** |
| R-15 same-process exactly once | None | Fence refuses; no identical-key no-op |
| R-16 transaction atomicity | Annual step only | No Fell transaction |
| R-17 edits/year-advance consent | None | — |
| R-18 no-work and ecology | Partial: no-work ecology proved (5a–5c) | No post-Fell path |
| R-19 immutable session history | None (report per call only) | — |
| R-20 biological integrity | Partial: natural death typed; zero-live refusal | No Fell cause |
| R-21 G4 economics honesty | None (no economics in engine) | — |
| R-22 accessibility | Not inspected | Out of source scope |
| R-23 G3 persistence | Partial: in-memory only (`AnnualContinuation.cs:5`) | Not qualified |
| R-24 protected scope | Partial: hashes pinned (`TrustTables.cs`, `CcfSourcePins.cs`, test digests) | Not re-executed |
| R-25 implementation gates | — | G2 blocked; V-01/V-02 open |
| R-26 explicit refusals | Partial: stable `P1Refusal` codes (`Domain.cs:38-42`) | No work refusals |
| X-01 original source | Partial (1a) | Not run |
| X-02 committed current | Partial (1d gap) | Not run |
| X-03 one-area UI | None | — |
| X-04 per-ID judgement | None | — |
| X-05 sample/G1 fallback | None | — |
| X-06 #805 boundary | Partial (#814) | Not run by audit |
| X-07 G1 proposals | None | N/A not decided |
| X-08 manual pending work | None | — |
| X-09 prescription pending | None | N/A not decided |
| X-10 consolidated Work Plan | None | — |
| X-11 distinct approval | None | — |
| X-12 authoritative Execute | **None** | G2 BLOCKED |
| X-13 mid-batch fault | Annual-step analogue only | — |
| X-14 idempotent same process | None | — |
| X-15 edit/consent | None | — |
| X-16 genuine following year | Ecology half only | No Fell input |
| X-17 no-work oracle | Recorded digests (`ContinuationTests.cs:11`) | Not re-executed; pending-work branch absent |
| X-18 repeated cycle/history | None | — |
| X-19 mortality/zero-live | Natural half only | No Fell cause/policy |
| X-20 economics honesty | None | — |
| X-21 accessibility | Not inspected | — |
| X-22 protected source | Hash pins present | Not re-executed |
| X-23 scope/persistence | Partial (in-memory) | Not qualified |
| X-24 gates/provenance | — | G2 blocked; candidates not on GitHub |

### Contradictions and inconsistencies found

1. **Stale comments inside the source:** `TrustTables.cs:95-96` says "No ecological year runs: AnnualGate still refuses with AnnualEngineNotImplemented". But `AnnualEngine.cs:51-57` swallows exactly that sentinel and `Advance` runs a year (`:74-87`). `FixtureAccess.cs:142-145,158` has the same stale wording. **CONTRADICTED (comment vs code), no behavioural risk identified.**
2. **Spec premise vs engine surface:** the spec (§2, FR-01) assumes an admitted committed "Genesis or AfterMortality end-year" under the #805 `sourceKind`. The engine exposes no such type; the multi-year state is `internal` and explicitly "no public continuation contract" (`AnnualEngine.cs:66`). **GAP**, not a contradiction of the design itself.
3. **Revision semantics:** the spec wants an execution identity bound to "source-year + batch-revision". Today `Revision` equals the year count (`ContinuationTests.cs:32`; exporter `Program.cs:185`). A same-year Fell revision would conflict with that invariant. **Design choice open** (remediation item 3).
4. **Action threshold:** the fixture's `actionEligibleDbhThresholdMm = 170` is not addressed by the Stage 5 spec. **Open product question** (remediation item 5).
5. **Public availability:** the dispatch asks for "accepted source code". No inspected candidate (the #789 engine, H0A core/viewer, #811 or #814) is on GitHub. For a GitHub-only reviewer, all of them are **NOT INSPECTABLE**. This audit used the owner's local read-only worktrees, which are not public or accepted source.

### Not inspected, and why

- **Running anything** (tests, builds, browsers, the exporter): forbidden by this read-only scope. All "recorded test" evidence is source text only.
- **Contents of the archive bundle and tars:** checksums verified, not unpacked (unpacking would be a write).
- **`plan.md` / `tasks.md`:** fetched, not line-cited. The verdicts rely on spec.md, requirements.md and the dispatch.
- **Full bodies of the main Scenario One / WorkEconomy files:** grep-level only.
- **Desktop / ZX20 / `/media/jer`:** not mounted here and out of scope.
- **L4 FAIL38 details:** status taken from the #804 Manager verdict; not re-evaluated.

## Statement of limits

This is an **audit only**. It grants **no implementation approval**, performs **no Unity or native verification**, and **does not waive or downgrade V-01, V-02 or L4 FAIL38**. It does not change #730, #834, specs, coordination, tests, fixtures, main or any physical Ops/board/lock state. Manager #730 decides whether to commission a G2 source-contract revision based on the design choices above.

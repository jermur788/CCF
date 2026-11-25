# CCF Multi 2D — Pre-2A (S0) Seam Extraction and 2D Integration Trunk — Specification

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1`
- **Feature ID:** `multi-2d-s0-seam-extraction-v1`
- **Title:** Pre-2A (S0) Seam Extraction and 2D Integration Trunk
- **Status:** `DRAFT` — authorises **zero** implementation
- **Classification:** `SPEC REQUIRED` (cross-system, multi-step, multi-agent; branch/integration policy; determinism and save-contract seams)
- **Owner:** Project owner (approval, product decisions, Open Questions); Overall Manager (classification, coordination, lane admission); appointed 2D integrator (trunk custody once approved)
- **Spec author:** Claude Opus 5.5 spec-author lane (identity record in the handoff provenance table)
- **Created:** 2026-11-25
- **Last updated:** 2026-11-25
- **Authoring base (context) commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (tip of `origin/feat/multi-2d-phase1-time-history-core-20261124-v1`)
- **Read-only inspection references:** Phase 1 Step 2 UI `adea7af220259a1808346cd5381babed3e9052fa` (`feat/multi-2d-phase1-time-history-ui-20261125-v1`, inspected only with `git show` / `git ls-tree` / `git grep`, never checked out); `origin/main` `cc2757a730ad9451e3d6e3680dffd123171a5df5` (3D-only; never a merge target for this work)
- **Decision log references:** D-020 (presentation authority), D-062 (Oak/Beech timber/economy — Phase 2B), D-063 (selective SDD), D-073–D-077 (history, pacing, calibration framing), D-093 (Multi 2D Stage 5 policy). **No decision-log entry yet records the 2D trunk / S0 roadmap.** Locked decisions A–D below come from the owner direction relayed in the dispatch brief; recording them in the decision log on approval is listed under §14 and in the handoff's DECISIONS NEEDED.
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`). This sits outside the Compact30 Scenario One 3D milestone; S0 uses no Unity lane.
- **Supersedes / superseded by:** none

---

## 0. Authority and status

This specification becomes task-level implementation authority for the S0 pass **only once its status is `APPROVED`**. While it is `DRAFT` it authorises nothing: no branch creation, no code, no test, no script, no asset and no coordination change. Per `specs/README.md`, only `APPROVED`, `IMPLEMENTING`, `VERIFYING` or `VERIFIED` specs authorise implementation.

It remains subordinate to, in order:

1. the latest direct user / product-owner instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`.

Implementation truth is the inspected repository at the commits named above. A specification states intended behaviour and acceptance criteria; it never proves that any behaviour exists. Every number in §3 was measured by the spec author from Git blobs; none of them is runtime, build or test evidence. If implementation reality contradicts this spec, the worker stops and returns to the Manager/spec instead of reinterpreting it.

---

## 1. Problem statement

The 2D two-area simulator has grown its whole `/multi` presentation layer inside one C# file. `src/Ui/PlayerMultiApi.cs` is 7,525 lines and 438,983 bytes. It holds the route table, every handler, the payload-bounding helpers, and three raw-string constants that carry the whole page (876 lines of HTML, 684 lines of CSS, 4,609 lines of JavaScript). The engine's annual step is one 163-line `Advance` body. Its RNG key prefixes are string literals spread over four Core files. Save format v2 has no extension mechanism.

Phase 2A (Forest Maker, Save v3), Phase 2B (Economy), Phase 3 (Jay dispersal, storms, Species Wave 2) and Phase 4 (MapLibre, imports) all need to work in parallel. With the current shape, every one of those lanes would edit the same two files (`PlayerMultiApi.cs`, `MultiAreaContinuation.cs`) and the same JavaScript closure. That means constant merge conflicts and a high risk of silently changing determinism.

S0 is a **zero-behaviour-change** restructuring pass. It creates:

1. a dedicated 2D integration trunk with a fail-closed gate;
2. mechanical file splits that give each future lane its own files;
3. fixed compile-time engine seams (no plugin framework) plus frozen cross-lane contracts;
4. a zero-build native ES-module split of the `/multi` JavaScript behind a map-renderer contract.

## 2. Position in the roadmap

```text
Phase 1 (Save/Load, Step 1 Core, Step 2 UI) ── owner-accepted consolidated baseline (NOT YET DELIVERED: Step 2 in R2/R3)
   │
   ▼  integration/2d-sim cut from that exact accepted commit
S0a  mechanical moves + trunk + 2d-gate.sh
   ▼
S0b  fixed engine seams + frozen contracts ──► unlocks Phase 2B Economy ENGINE coding (parallel lane)
   ▼
S0c  native ES modules + map renderer plug ──► unlocks Phase 2A and Phase 2B UI coding (parallel lanes)
   ▼
Phase 2A / 2B / 3 / 4 (each under its own approved spec)
```

S0 comes before 2A, 2B, 3 and 4. It cannot start until the consolidated Phase 1 baseline is verified, R3-approved and owner-accepted (§8, entry condition E-0). Unlock statements say only *when* a parallel lane may begin coding. Each lane still needs its own approved spec and its own lane admission.

---

## 3. Measured current state (implementation truth)

All facts are at Step 2 `adea7af2…` unless stated otherwise. `src/Core/**`, `Program.cs`, `PlayerApi.cs` and `SitkaP1.Ui.csproj` are byte-identical at the base `5fe2fabe…`: Step 2 changed only `PlayerMultiApi.cs` under `src/`. Paths are relative to `Tools/MultiRackSitkaP1Lab/` unless they start with `Tools/`.

### 3.1 The `/multi` page and route surface

| Fact | Measured value |
|---|---|
| `src/Ui/PlayerMultiApi.cs` | 7,525 lines; 438,983 bytes; UTF-8; no BOM; LF only (0 CR) |
| Route/handler class | `internal sealed partial class PlayerApi`, lines 19–1339 (namespace `SitkaP1.Ui`, line 10). The other partial part is `src/Ui/PlayerApi.cs` (449 lines). |
| Page-constant class | `internal static class MultiAreaPage`, lines 1346–7525 (same file, separate class; **not** part of `PlayerApi`) |
| `internal const string Html = """` | declaration line 1348; content lines 1349–2224 (876 lines, 57,612 bytes); closing `""";` line 2225 at column 0 |
| `internal const string Css = """` | declaration line 2227; content lines 2228–2911 (684 lines, 57,702 bytes); closing `""";` line 2912 at column 0 |
| `internal const string Js = """` | declaration line 2914; content lines 2915–7523 (4,609 lines, 246,563 bytes); closing `""";` line 7524 at column 0 |
| Raw-literal shape | Not interpolated. Closing delimiters at column 0, so the dedent is zero. No tab characters. Non-ASCII UTF-8 present on 51 (HTML), 2 (CSS) and 244 (JS) content lines. |
| Spec-author content SHA-256 (informational only, adea7af blob, raw-literal rule of §5.3) | HTML `f9d72dad1d2271e6d6f26c132da93b0257e43cb11a85521c748d98d7d3821868`; CSS `f3b2b68a7547acc78444f6389db0f9f39080589950a54417506c4aaf036f00a5`; JS `62c29114850506f50a6f02529f5343f98950ae67229144ffde595aa114f9985e` |
| Serving | Lines 53–55: `static readonly byte[] MultiPage{Html,Css,Js} = Encoding.UTF8.GetBytes(MultiAreaPage.{Html,Css,Js})`. Lines 73–75 in `AddMultiAreaRoutes()` (line 71): routes `multi`, `multi.css`, `multi.js` → `WriteAssetAsync` with `text/html; charset=utf-8`, `text/css; charset=utf-8`, `text/javascript; charset=utf-8`. |
| Route count in `AddMultiAreaRoutes` | 33 (3 asset routes + 30 `api/multi/*` routes) |
| HTML asset references | `<link rel="stylesheet" href="multi.css">` and `<script src="multi.js" defer></script>` (HTML content lines 9–10) |
| JS structure | Header comment (2915–2917), `'use strict';` (2918), IIFE `(function () {` (2920) … `})();` (7523). Prelude 2915–3017, then 17 `// ---- … ----` banners (below). `function wire()` 7342–7517; the only four top-level statements are `initFloorDefs(); renderLegend(); wire(); reread(false);` (7519–7522). No top-level `let`/`var`; 32 top-level `const` declarations; shared mutable state lives in `const state` and `const byId`; no multi-line template literals. |
| JS cross-section calls | The "talking to the local session process" section calls `renderAll()` (rendering), `withdrawConfirmation()` (Execute confirmation) and `lockCommands()` (alerts). `renderAll()` is called from 8 sections, and `run`/`call`/`jsonCall` from 7. **A naive one-section-per-module split is therefore cyclic** (§7.2 P4). |

JS section banners, in order (start line in `PlayerMultiApi.cs` and line count):

| # | Start | Lines | Banner |
|---|---|---|---|
| 0 | 2915 | 103 | prelude: constants, `state`, `byId` (no banner) |
| 1 | 3018 | 75 | small DOM helpers |
| 2 | 3093 | 179 | lookups in the latest atomic composite payload |
| 3 | 3272 | 107 | talking to the local session process |
| 4 | 3379 | 278 | Phase 1 Save/Load (server-managed named slots; no path, no upload, no download) |
| 5 | 3657 | 166 | alerts, announcements and the re-read lock |
| 6 | 3823 | 79 | Execute confirmation |
| 7 | 3902 | 179 | rendering |
| 8 | 4081 | 557 | the map: 1 SVG user unit = 1 metre; U east, V south |
| 9 | 4638 | 155 | status marks |
| 10 | 4793 | 348 | Interactive modes: Inspect Cell, Plant, Spot Clear, and PJ Sapling inspection |
| 11 | 5141 | 89 | grid and legend |
| 12 | 5230 | 153 | Tree inspector, Prune box (species-aware) and Competition |
| 13 | 5383 | 340 | Competition and Fell draft |
| 14 | 5723 | 402 | Work Plan, Receipt, Results, Trees, Details |
| 15 | 6125 | 251 | selection & Escape precedence |
| 16 | 6376 | 413 | Work Plan commands (Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear, Crop, Approve, Cancel, Execute, Year +1) |
| 17 | 6789 | 735 | Phase 1 Step 2: Skip to Review, On-Demand History Drawer, and On-Demand Structure Panel (contains `wire()` 7342–7517 and the bootstrap) |

### 3.2 Protected single-area asset trees (must never be modified)

- **P1 single-area page:** `src/Ui/wwwroot/index.html` (274 lines, 14,205 B), `app.js` (1,674 lines, 86,935 B), `app.css` (435 lines, 24,878 B). They are compiled in as `EmbeddedResource` with `LogicalName` `SitkaP1.Ui.wwwroot.{index.html,app.css,app.js}` (`src/Ui/SitkaP1.Ui.csproj` lines 25–30) and read by `EmbeddedAsset()` (`PlayerApi.cs:407`). `tests/ui-smoke.sh` check `G11b` (line 183) byte-compares the served bodies against these committed files.
- **Legacy P0 lab page:** `Tools/MultiRackSitkaLab/src/Ui/wwwroot/{index.html,app.js,styles.css}`. The whole P0 subtree is pinned to tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` by `build/build-p0-core-ref.sh`, which exits 2 on any mismatch.
- The new `/multi` asset tree **must be a physically distinct directory that is neither of these and is not inside either of them** (§5.1).

### 3.3 HTTP security surface (preserve byte-for-byte)

| Element | Location | Behaviour |
|---|---|---|
| Single listener | `src/Ui/Program.cs:58–64` | `WebApplication.CreateEmptyBuilder`; `UseKestrelCore`; `AddServerHeader = false`; `MaxRequestBodySize = PlayerApi.MaxBodyBytes` (16 KiB, `PlayerApi.cs:27`); `kestrel.Listen(IPAddress.Loopback, 0)` (127.0.0.1, OS-assigned ephemeral port) |
| Bind check | `Program.cs:72–78` | exactly one bound URL, host `127.0.0.1`, port ≠ 5187 (P0 lab), port ≥ 1024, otherwise stop with exit 3 |
| Capability URL | `Program.cs:53`; `PlayerApi.cs:137–142`, `395–405` | 32 bytes from `RandomNumberGenerator`, lowercase hex; path `/s/<capability>/<route>`; `CryptographicOperations.FixedTimeEquals`; 404 with no body on mismatch; 308 to add the trailing slash |
| Admission | `Program.cs:81`; `PlayerApi.cs:112–113`, `122–127` | 503 until `AdmitRequestsFor(port)` after the bind check |
| Host guard | `PlayerApi.cs:129–134` | `Host` must equal `127.0.0.1:<port>` exactly (DNS-rebinding guard) → 403, no body |
| Route/method allow-list | `PlayerApi.cs:82–108`, `151–162` | `Dictionary<string, Route>` with `StringComparer.Ordinal` exact keys → 404 JSON; method mismatch → 405 with `Allow` |
| Origin guard | `PlayerApi.cs:164–172` | every POST, and any request carrying `Origin`, must name `http://127.0.0.1:<port>` exactly → 403 `OriginRefused`; no CORS header is ever sent |
| Security headers incl. CSP | `PlayerApi.cs:33–34`, `383–391` | `Cache-Control: no-store`; `X-Content-Type-Options: nosniff`; `X-Frame-Options: DENY`; `Content-Security-Policy: default-src 'none'; script-src 'self'; style-src 'self'; img-src data:; connect-src 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'`; `Referrer-Policy: no-referrer`; `Cross-Origin-Resource-Policy: same-origin` |
| Asset writer | `PlayerApi.cs:375–381` | fixed status 200, fixed `Content-Type`, exact `Content-Length`; no MIME guessing, no filesystem read |
| No page file reads | `SitkaP1.Ui.csproj` comment lines 21–24; `tests/Core.Tests/Program.cs:428–445` (`644 B-01`) | Pages are served from memory. The `644 B-01` source guard bans `File.ReadAll*`, `FileStream`, `File.Open`, etc. in every `src/**/*.cs` except `src/Core/Phase1/Phase1SaveSlots.cs`. |

### 3.4 Annual advance (`src/Core/MultiArea/MultiAreaContinuation.cs`)

- The file is 1,572 lines, already `internal sealed partial class MultiAreaContinuation` (line 74). Its other parts are `MultiAreaTimePacing.cs`, `MultiAreaHistoryQuery.cs` and `WoodlandStructureMetrics.cs`. The single lock is `private readonly object sync = new();` (line 79). Every read and command takes `lock (sync)` exactly once.
- `MultiAreaRoot.HasUnexecutedWork` (lines 34–35): `Pending.Count > 0 || Approved is not null || Stage4.PendingPrunes > 0 || Stage5.PendingClears > 0 || Phase2.PendingOrders > 0`.
- Work Plan command methods: lines 459–983 (`MarkCrop` … `Execute`). `Execute` (808–983) is all-or-nothing and already idempotent. Replay is answered from `root.Executed` keyed by the engine-minted `planId + ":execute"` (`MintedExecutionKey`, line 1527), and a key conflict is refused.
- `Advance(fence, consentToCancelWork, afterArea?, beforeCommit?)`: lines 993–1155, `lock (sync)` at 996. **Measured statement order:**
  1. `RequireFence` (998); `hasWork = root.HasUnexecutedWork` (999); consent refusal `WorkPendingConsentRequired` (1000–1002); empty-stand refusal `ContinuationNoLivingStems` (1004–1006).
  2. Locals (1008–1024): `year`, `revision = checked(AnnualRevision + 1)`, `nextAreas`/`records`/`reports`, `lostCrop`, start-of-year `woodland` (crown model 1 only), `stage4`, `deadwood` list, `nextDeadwood`, `phase2Start`, `density`.
  3. **Per-area loop in `root.Areas` index order (1025–1080):** adult step (`Phase2MixedDensity.StepRack` | `MultiAreaAnnual.StepStage4` | legacy `Step` | crown-C2 `Step`). **Stage 4 fallen-deadwood records are appended inside the same iteration**, numbered in area order and then death order (1049–1061). Then per-area year row and report (1062–1073), Crop-mark pruning (1075–1078), and the test-only `afterArea` seam (1079).
  4. **Year record constructed** from the adult-step rows (1081–1082).
  5. Regeneration (1087–1128): `Phase2Annual.Step` (Stage 5 + Phase 2) or `Stage5Annual.Step` (Stage 5 only). Promotions are merged into rosters in ordinal `TreeId` order. Consented cancellation clears pending/approved clears and plantings.
  6. Test-only `beforeCommit` seam (1131).
  7. **One atomic root assignment** (1133–1152): year, revision, areas, `AnnualHistory` append of the year record, plan cancellation, `PlanStateRevision`, Stage 4 deadwood concatenation and `NextDeadwoodNumber`, Stage 5, Phase 2.
- **Consequence:** the locked slot names *AdultGrowth → Deadwood → Regeneration → YearRecord* match the **ecological / data-dependency order**: deaths become deadwood after their own adult step, regeneration sees the post-adult woodland, and the year record is published last. They do **not** match the **statement order**. Deadwood capture is interleaved per area, and the year record is *constructed* before regeneration. §6.2–§6.3 turn this into the explicit S0b safety-valve decision point.

### 3.5 RNG key prefixes (simulation randomness is stateless and keyed)

The randomness is `SimulationRandom.Roll(RngModelVersion, key, year, seed)` and `Stage5Random.Stream(key, seed, year) = SimulationRandom.Create(model, seed, year, FNV-1a(key))`. A draw on one key can never move another key's value. `SimulationRandom` is the linked Unity source `Assets/ForestPrototype/SimulationRandom.cs` (`src/Core/SitkaP1.Core.csproj:28`; `new System.Random(Seed(…))` at its line 37), which is outside `src/Core` and outside the trunk write surface. `DerivedManifest.RngModelVersion = 1` (`DerivedManifest.cs:49`). The world generators (`MultiAreaGenerator`, `ManifestGenerator`) use their own SplitMix64 with integer seeds and no string keys.

| Key family (exact literal) | Mechanism | Site(s) in `src/Core` |
|---|---|---|
| `"ADULT-MORT-"` + TreeId | Roll | `AnnualRules.cs:87`; `MultiArea/Phase2MixedDensity.cs:108` |
| `"ADULT-MORT-GENERIC-"` + TreeId | Roll | `MultiArea/Phase2Rules.cs:159` |
| `"ADULT-MORT-GENERIC-QUOTA-"` + AreaId | Roll | `MultiArea/Phase2Rules.cs:160` |
| `"PHASE2-MAST-"` + SpeciesId | Stream | `MultiArea/Phase2Rules.cs:158` |
| `"PHASE2-PJ-"` + JuvenileId, with sub-keys `+ "/vegetation"`, `+ "/browse"` | Roll | `MultiArea/Phase2Rules.cs:161–163` |
| `"STAGE5-MAST"` (complete key) | Stream | `MultiArea/Stage5Mast.cs:11` |
| `$"STAGE5-PROMO-{areaId}-{column}-{row}-{year}"` | Stream | `MultiArea/Stage5Mast.cs:14–15` |

The tree-ID prefixes `"P30-"` (`ManifestGenerator.cs:55`, `MultiArea/MultiAreaExperiment.cs:107`) and `"PL-"` (`MultiArea/Phase2Annual.cs:545`) are **not** RNG salts, although tree IDs become part of `ADULT-MORT-*` keys. **Measured: the frozen families are not prefix-free.** `"ADULT-MORT-"` is a prefix of `"ADULT-MORT-GENERIC-"`, which is a prefix of `"ADULT-MORT-GENERIC-QUOTA-"`. Changing any of these literals would change RNG streams and frozen anchors (§6.4).

### 3.6 Entropy and clock sites in `src/Core`

- The brief's literal forbidden set (`new Random`, `DateTime.Now`, `Guid.NewGuid`, `Parallel.*`/`AsParallel`) has **zero** hits in `src/Core`.
- Four entropy/clock sites exist: (1) `MultiAreaContinuation.NewSessionId()` (`RandomNumberGenerator`, lines 1188–1189); (2) the `AnnualContinuation.sessionId` field initialiser (`RandomNumberGenerator`, `AnnualContinuation.cs:337–338`); (3) `MultiAreaContinuation.CaptureSave` header time (`DateTime.UtcNow`, line 1171); (4) `Phase1SaveSlots.NewTempPath` temp-name nonce (`RandomNumberGenerator`, `Phase1/Phase1SaveSlots.cs:412–413`). None of them feeds simulation state, tokens or anchors.

### 3.7 Save format v2 and the v2 golden

- `src/Core/Phase1/Phase1SaveFormat.cs`: `Format = "ccf-2d-multiarea-save"` (line 12), `SchemaVersion = 2` (line 20), `MaxSaveBytes = 64 MiB` (line 26), `ChecksumAlgorithm = "sha256(header||payload)"` (line 38).
- Envelope (`Phase1SaveCodec.cs:61–64`, `145–147`): exact top-level key set `format`, `schemaVersion`, `header`, `payload`, `checksum{alg,value}`. Strict UTF-8, no BOM, duplicate keys refused, depth ≤ 16, canonical re-encode byte equality, checksum verified. Any other schema version → `SaveSchemaUnsupported` (line 138). There is no migration.
- `Phase1SaveEnvelope.cs` (30 lines): `Phase1SaveHeader(Label, SavedUtc, SaveGeneration, EcologicalYear, Models, Genesis, Lineage)`, `Phase1SaveEnvelope(Header, State)`, `Phase1DecodedSave`.
- **No committed golden save file exists in the repository.** The v2 golden is an **in-code pin** in `tests/Phase1.Save.Tests/Program.cs:727–738`. The encoding of `Samples.Phase2Approved()` is 24,703 bytes, file SHA-256 `c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7`, embedded checksum `9629ac1648fabd85702fc4105075cebe9f69e884a816edcce4c441a3866c9086`.

### 3.8 The 9/9 frozen anchors — where they actually live

The canonical list of expected values is the table in `specs/multi-2d-phase1-time-history-core-v1/spec.md` §7, repeated in prior handoffs. **It is not in `build/build-p0-core-ref.sh`**, which pins only the P0 tree `f212c8f6…` and commit `a7f41983…`. Enforcement is distributed:

| # | Anchor | Expected SHA-256 | Enforced by |
|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `tests/Core.Tests/Model0FingerprintTests.cs:16` const `PinnedSingleAreaHash` |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `Model0FingerprintTests.cs:17` `PinnedMultiAreaHash` |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `Stage4Tests.cs:12` `PinnedStage3CrownModel1Hash` |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `Stage5Tests.cs:13` `PinnedStage4FullRunHash` |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `Stage5Tests.cs:19` `PinnedStage5FullRunHash` |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `Stage5GridStudy.cs:49` `PinnedFiveMetrePurityHash` |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | **not pinned in code.** `Phase2Tests.cs:448` prints `P2-03 M-08 Q6 hash 1/1/1/1/0 <h0> \| 1/1/1/1/1 <h1>` and asserts only `h0 == h1` |
| 8 | P0 Core binary `.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | `src/Core/SitkaP1.Core.csproj:7` `P0CoreExpectedSha256` (build error); `Phase2Tests.cs:16`; `tests/build-output-safety.sh:14` |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | **not pinned in code.** `Phase2Tests.cs:1323` prints `P2-10 MA-1-MIXED-v1 30-year fingerprint <h>` and asserts only run-to-run equality |

### 3.9 Build, test and gate inventory

| Item | Real invocation (from `Tools/MultiRackSitkaP1Lab`) | Mode / prerequisites |
|---|---|---|
| P0 reference | `build/build-p0-core-ref.sh` | bash + git archive + dotnet; offline; required before any P1 build on a clean checkout (`.p0ref/` is git-ignored) |
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | Core + Cli + Core.Tests; `TreatWarningsAsErrors=true` (`Directory.Build.props`); SDK `8.0.133`, `rollForward: latestPatch` (`global.json`); no `PackageReference` anywhere |
| UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | not in the `.sln`; framework reference only |
| Save tests build/run | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | not in the `.sln` |
| Core tests | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | headless; prints the anchor lines for #7 and #9 |
| `tests/ui-smoke.sh [<evidence-dir>]` | built UI required | **headless** (bash + curl + python3 + node DOM stand-in) |
| `tests/ui-smoke-multiarea.sh [<evidence-dir>]` | built UI required | **headless**. Asserts the `<script src="multi.js" defer>` tag (line 128) and `node --check multi.js` (M05), runs `multi.js` via `vm.runInContext` (lines 1231–1241), and holds the payload-budget checks view ≤ 524,288 B / ecology ≤ 153,600 B (lines 174, 520, 551, 831, 880) |
| `tests/phase1-save-ui/run-edge-e2e.sh [<evidence-dir>]` | built UI; Windows `msedge.exe` + Windows `node.exe` through WSL2 interop | **browser**: Edge `--headless=new`, 1280×720 and 1920×1080 (`drive.mjs`) |
| `tests/phase1-time-history-ui/run-step2-edge.sh [<evidence-dir>]` | as above (Step 2 only, `drive-step2.mjs`) | **browser**: headless Edge, 1280×720 and 1920×1080 |
| `tests/p0-isolation.sh <evidence-dir>` | dotnet, strace | headless |
| `tests/build-output-safety.sh <evidence-dir>` | runs the P0 builder against hostile states | headless |
| `tests/cli-safety.sh <evidence-dir>` | built CLI | headless |
| `evidence/` | git-ignored local scratch (`.gitignore`; `evidence/README.md`) | durable evidence is off-repo (`/media/jer/Files/CCF/Verification/`) |

### 3.10 Corrections to the dispatch brief (measured values win)

1. `PlayerMultiApi.cs` is **7,525** lines (brief: "~7,210"). The embedded JS is **4,609** lines / 246,563 bytes (brief: "~4,590 lines").
2. The three constants belong to **`internal static class MultiAreaPage`**, not to `PlayerApi`. The `PlayerApi` partial in the same file holds the routes and handlers.
3. `Advance` statement order differs from the slot list (§3.4).
4. The 9/9 anchor list is **not** in `build-p0-core-ref.sh`. Anchors #7 and #9 have **no code-level pin** (§3.8).
5. There are **no committed v2 golden save files**. The golden is an in-code pin (§3.7).
6. There is **not one** allow-listed nondeterminism site but four entropy/clock sites, and the brief's literal tokens have zero hits (§3.6).
7. The frozen RNG prefixes are **not prefix-free** (§3.5).
8. A naive section-wise ES-module split is **cyclic** (§3.1).
9. `ui-smoke-multiarea.sh` hard-codes the classic `multi.js` script tag and loader, so S0c must own an update to that harness.
10. `evidence/` is git-ignored, so committed proof files need another home (OQ-4).

---

## 4. Locked decision A — Dedicated 2D integration trunk and `2d-gate.sh`

### 4.1 Trunk

- **A-1.** `integration/2d-sim` is cut **strictly** from the verified, R3-approved, owner-accepted consolidated Phase 1 baseline commit (`T0`). That commit does not exist yet: Phase 1 Step 2 is still in R2/R3. Creating the trunk from any other commit, from `main`, or from an unaccepted Step 2 tip is forbidden.
- **A-2.** `main` stays strictly 3D-only. Nothing is ever merged from `integration/2d-sim` (or any 2D feature branch) into `main` automatically. Any future 2D→`main` movement needs a separate owner decision outside this spec.
- **A-3.** Feature branches fork from the current `integration/2d-sim` tip. Each has a strict single-writer file-ownership list (§13).
- **A-4.** **One integrator** rebases the feature branch onto the trunk tip, runs `2d-gate.sh`, confirms an **independent non-author review** of the exact gated HEAD, and merges with `git merge --no-ff`, **one branch at a time**.
- **A-5.** **Freeze-on-red:** if the gate fails on the trunk tip, the trunk is FROZEN. No further merge is allowed except a repair or a `git revert -m 1` of the offending merge (never a force-push or history rewrite), until the gate is green again.

### 4.2 `2d-gate.sh` — stages in locked order

`2d-gate.sh` runs a preflight and then the seven locked stages **in this order**. It stops at the first failing stage; later stages are reported `NOT RUN`.

| Stage | Name | Checks |
|---|---|---|
| G0 | Preflight (environment only) | Run from inside a Git checkout whose `Tools/MultiRackSitkaP1Lab` is the real directory; `git status --porcelain` empty; `TMPDIR` and the evidence directory outside the repository; required tools present (git, bash, dotnet SDK per `global.json`, python3, node, strace); base ref resolved (default `git merge-base HEAD origin/integration/2d-sim`). G0 performs no check that belongs to G1–G7. |
| G1 | Clean build, 0 warnings / 0 errors | Remove only P1-owned `bin/`/`obj/` under `Tools/MultiRackSitkaP1Lab` (never under P0). Run `build/build-p0-core-ref.sh`. Build `MultiRackSitkaP1Lab.sln`, `src/Ui/SitkaP1.Ui.csproj` and `tests/Phase1.Save.Tests/Phase1Save.Tests.csproj`, each with `--disable-build-servers`. Each MSBuild summary must show `0 Warning(s)` and `0 Error(s)`. |
| G2 | All Core / Save / UI test suites | Core tests (stdout captured for G3); Save tests; headless UI suites `tests/ui-smoke.sh` and `tests/ui-smoke-multiarea.sh`; safety suites `tests/cli-safety.sh`, `tests/build-output-safety.sh`, `tests/p0-isolation.sh`. Zero failures. Per-suite pass counts ≥ the counts recorded at baseline (`build/2d-gate/baseline-counts.json`); a lower count (a removed test) fails. Packet sub-checks run here: **S0a** `/multi` asset identity (§5.3), from S0a onward; **S0b** RngSalts test inside Core tests; **S0c** module graph and JS reconstruction (§7.5), from S0c onward. |
| G3 | 9/9 frozen SHA-256 anchors | Against `build/2d-gate/frozen-anchors.json` (OQ-3). #1–#6 and #8: the manifest value must equal the literal in the enforcing source file (grep cross-check, so manifest and tests cannot drift) **and** the G2 Core run must have passed. #7 and #9: the G2 Core stdout line must exist exactly once and its hash(es) must equal the manifest value (for #7, both `h0` and `h1`). #8 also: `sha256sum .p0ref/SitkaLab.Core.dll`. |
| G4 | Committed v2 golden saves | The Save-suite case `golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding` passed in G2. Its three literals (24,703 B, `c0ea8a46…`, `9629ac16…`) equal the manifest. `Phase1SaveFormat.SchemaVersion = 2` literal present. `git diff --name-only <base>..HEAD -- src/Core/Phase1/` is empty (v2 frozen read-only). |
| G5 | Payload budget checks | The G2 `ui-smoke-multiarea.sh` evidence log contains every baseline-recorded budget check ID marked PASS (view ≤ 524,288 B, ecology ≤ 153,600 B), with no budget check missing. |
| G6 | Core determinism grep gate | Scope `src/Core/**/*.cs` (excluding `bin/`, `obj/`). **F1 (forbidden, zero hits allowed):** `new Random(`, `new System.Random(`, `DateTime.Now`, `DateTimeOffset.Now`, `Guid.NewGuid`, `Parallel.`, `.AsParallel(`. **F2 (entropy/clock, allowed only at listed sites):** `RandomNumberGenerator`, `DateTime.UtcNow`, `DateTimeOffset.UtcNow`, `Environment.TickCount`, `Stopwatch`. F2 sites are allowed only in `build/2d-gate/determinism-allowlist.txt`, keyed by file + enclosing member (not line number), initially exactly the four sites of §3.6 with `NewSessionId` first. From S0b onward: no RNG key-prefix string literal outside `src/Core/RngSalts.cs` (§6.4). |
| G7 | Protected-path guard | `git diff --name-only <base>...HEAD` (and, on the trunk, the cumulative diff from `T0`). **Every** path must start with `Tools/MultiRackSitkaP1Lab/` or `Docs/`. Any path under `Assets/`, `Packages/`, `ProjectSettings/` → explicit violation. Deny-list inside the allowed roots: `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/**`, `Tools/MultiRackSitkaP1Lab/fixtures/**`, `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**`, `Docs/Project/**` (including `Docs/Project/coordination/active-tasks.md`). The `src/Ui/wwwroot` tree id must equal the `T0` tree id. The deny-list lives in `build/2d-gate/protected-paths.txt`; changing it needs a reviewed decision. |
| B (optional, `--browser`) | Browser tier | `tests/phase1-save-ui/run-edge-e2e.sh`, `tests/phase1-time-history-ui/run-step2-edge.sh` and, from S0c, the S0c parity driver (§7.5). Needs Windows Edge + Windows node through WSL2 interop. **Required** for S0c acceptance. |

### 4.3 Fail-closed semantics and exit codes

- `set -euo pipefail`. An `ERR`/`EXIT` trap turns any unexpected termination into a FAIL. Each stage must print an explicit `GATE <stage> PASS` marker, and the final verdict is computed from the markers. A missing marker is a FAIL, never a pass.
- The gate **never** modifies tracked files, never deletes outside its own temp directories and P1-owned `bin/`/`obj/`, never binds or kills the protected local listeners `:33767`, `:43649`, `:46593` (it records them before and after and fails if one it saw listening disappeared), and never contacts the network.
- **Exit codes:**

| Code | Meaning | Mergeable? |
|---|---|---|
| 0 | PASS: every stage G1–G7 passed (and B, when `--browser` was requested) | yes, with independent review |
| 1 | FAIL: a check ran and failed (includes unexpected errors) | no; triggers freeze-on-red on the trunk |
| 2 | REFUSED: preflight precondition (dirty tree, TMPDIR/evidence inside repo, wrong root, unresolvable base) | no |
| 3 | PREREQUISITE MISSING: a required tool or host capability is absent (including Edge for `--browser`) | no |
| 4 | PROTECTED-PATH VIOLATION (G7) | no |
| 5 | PARTIAL: only with an explicit `--headless-only` and no `--browser`, when every headless stage passed but a packet requires the browser tier | no for packets requiring tier B (S0c); see packet rules |
| 64 | Usage error | no |

### 4.4 Clean checkout, no network

The gate must succeed from a fresh `git clone` of the branch with **no network**. This is possible because the projects have no `PackageReference` (restore resolves only SDK packs), `.p0ref` is rebuilt offline by `build/build-p0-core-ref.sh` from the in-repo P0 tree, and all UI tests bind only `127.0.0.1` ephemeral ports. Where `unshare -rn` (Linux network namespace) is available, the gate runs G1–G2 inside it and records `NETWORK ISOLATION: PROVEN`; otherwise it records `NETWORK ISOLATION: NOT PROVEN` (not a failure, but it must be reported).

### 4.5 Gate self-test (proves fail-closed)

`build/2d-gate/selftest.sh` clones the repository locally (`git clone --local --no-hardlinks`) into `TMPDIR`. **It never touches the real working tree.** It plants one violation per stage in that clone (e.g. an F1 token in a Core file, a file under `Assets/`, a tampered anchor manifest value, a deleted Core test, a removed `GATE` marker, a missing tool on `PATH`) and asserts the specified non-zero exit code for each.

---

## 5. Locked decision B — S0a: mechanical moves and the 2D trunk

### 5.1 Dedicated `/multi` asset tree

- **Default path (owner-approvable, OQ-1):** `Tools/MultiRackSitkaP1Lab/src/Ui/multi/`, with S0a files `multi.html`, `multi.css`, `multi.js`.
- **Why this path:** it is a sibling of `src/Ui/wwwroot/`, not inside it, so no glob, copy or embed rule aimed at either tree can touch the other. It is named after its route. It lives in the UI project directory, so `EmbeddedResource` can include it without `..` paths. It is outside the P0 subtree. `wwwroot` has no special meaning in this `Microsoft.NET.Sdk` project, so a second `wwwroot-*` name would only add confusion.
- **Embedding, not disk reads:** each file is an explicit `EmbeddedResource` with an explicit `LogicalName` (`SitkaP1.Ui.multi.multi.html`, `SitkaP1.Ui.multi.multi.css`, `SitkaP1.Ui.multi.multi.js`), plus `<None Remove="multi/**" />`, mirroring the existing `wwwroot` entries. The running process still reads **no page file from disk**: this is required by the `644 B-01` source guard and by §3.3.
- **Byte pinning:** `src/Ui/multi/.gitattributes` with `* -text`, so Git never converts line endings in this tree. This follows the existing `Tools/MultiRackSitkaLab/.gitattributes` precedent. The protected trees are not touched.

### 5.2 Serving rules (unchanged surface)

- The routes `multi`, `multi.css`, `multi.js`, their methods, `Content-Type` strings, status codes, `Content-Length` behaviour, guards, headers and CSP stay **exactly** as in §3.3. There is no new listener, no static-file middleware, no directory listing, no path parameter, no filesystem read and no MIME inference.
- **The only permitted non-move code edits in S0a are:**
  - (i) the three `MultiPage{Html,Css,Js}` static initialisers change from `Encoding.UTF8.GetBytes(MultiAreaPage.X)` to an embedded-resource read through one small private helper (`MultiEmbeddedAsset(string logicalName)`, same pattern as `EmbeddedAsset`, throwing `InvalidOperationException` when the resource is missing). The single-area `EmbeddedAsset` in `PlayerApi.cs` stays untouched.
  - (ii) the now-empty `MultiAreaPage` class is removed;
  - (iii) the `SitkaP1.Ui.csproj` `EmbeddedResource` / `None Remove` entries are added;
  - (iv) `using` directives and `partial` class headers are added to the new partial files.

  A missing resource fails at type initialisation, so the process cannot start. That is fail-closed and matches the single-area behaviour.

### 5.3 Verbatim extraction and byte-identity proof

"Verbatim" means: **the bytes of each served response after S0a equal the bytes served before S0a, which equal `UTF-8(value of the C# constant at T0)`, which equal the bytes of the committed file.** The procedure is scripted (`build/2d-gate/raw-literal.py`, python3 standard library only). Nobody copies anything by hand.

1. **Source:** `git show <T0>:Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (Git blob bytes, independent of the checkout's `core.autocrlf`). Refuse if it is not valid UTF-8, starts with a BOM (`EF BB BF`), or contains any CR byte.
2. **Locate:** exactly one line equal to `    internal const string <Name> = """` for each of `Html`, `Css`, `Js`, in that order. The terminator is the first following line that equals `""";` exactly (column 0). Refuse if any count ≠ 1, the order differs, a `$` interpolation prefix is present, or the closing delimiter has leading whitespace (a non-zero dedent → stop and ask).
3. **Value (C# raw-string-literal rule):** the lines strictly between the opening and closing lines, joined with LF (`0x0A`), **with no trailing LF**. The newline after `"""` and the newline before the closing `"""` are not part of the value. With the closing delimiter at column 0, no indentation is removed (dedent = 0). Encoding: UTF-8 without BOM, exactly what `Encoding.UTF8.GetBytes(string)` produces.
4. **Reference hash** `R_x = SHA-256(value)` for x ∈ {html, css, js}.
5. **Pre-extraction oracle:** build the `T0` UI, start it on loopback, GET the three routes with valid Host and capability, and record body SHA-256 `P_x` and all response headers except `Date`. Require `P_x == R_x`. This proves the script's model of the C# compiler (dedent, line endings, encoding) is right.
6. **Write files** `src/Ui/multi/multi.{html,css,js}` with exactly the value bytes: LF only, **no final newline**, no BOM. Commit.
7. **Post-extraction proof:** build the S0a tip, start it, GET the three routes, record `S_x` and headers. Require `S_x == R_x`, `S_x == SHA-256(git show HEAD:<file>)`, and headers equal to step 5 except `Date`.
8. **Evidence file (gate input, committed):** `build/2d-gate/s0a-multi-asset-identity.json` (OQ-4), with `T0` SHA, the blob id of `PlayerMultiApi.cs` at `T0`, line spans, byte counts, `R_x`, `P_x`, `S_x`, committed-file hashes, the header set, tool versions and the UTC timestamp. From S0a onward the gate recomputes `R_x` from `T0` and re-serves the current build for **every merge** until S0c legitimately changes the HTML script tag and retires `multi.js` (§7.4). After S0c the identity set is `multi.css` (byte-identical to `T0`) plus the single permitted HTML line delta.

### 5.4 C# route-handler split (mechanical)

- `PlayerMultiApi.cs` is replaced (via `git mv` to `MultiRoutes.cs`, then member moves) by five partial files of the **existing** `internal sealed partial class PlayerApi` in namespace `SitkaP1.Ui`: `src/Ui/MultiRoutes.cs`, `src/Ui/MultiApi.View.cs`, `src/Ui/MultiApi.Commands.cs`, `src/Ui/MultiApi.Saves.cs`, `src/Ui/MultiApi.History.cs`. The member→file table is in `plan.md` §2.1 and is keyed by member name. Line numbers there are informational at `adea7af`.
- **Hard rule (static initialisation order):** C# runs static field initialisers in textual order within one part, but the order **across** partial parts is unspecified. `MultiTreeMembers` reads `MultiFenceMembers`, and `MultiAddMembers` reads `MultiTreeMembers`. Therefore **every static field and constant of the `/multi` partial stays in `MultiRoutes.cs` in its original relative order.** Splitting them across files is forbidden.
- Every moved member keeps its exact text, including XML doc comments, attributes and indentation. Members keep their original relative order inside each target file. There are no renames, signature changes, statement reorders, or whitespace changes beyond adding the per-file header (`using` lines, namespace line, class declaration line and braces). `PlayerApi.cs` stays untouched.

### 5.5 Continuation command split (mechanical)

- The Work Plan command methods move from `MultiAreaContinuation.cs` into `src/Core/MultiArea/MultiAreaContinuation.Commands.<Subsystem>.cs` files: `Crop` (`MarkCrop`, `UnmarkCrop`), `Fell` (`AddPending`, `RemovePending`), `Prune` (`AddPrune`, `AddScheduledPrune`, `RemovePrune`), `Clear` (`AddClearUndergrowth`, `RemoveClearUndergrowth`), `Planting` (`AddPlant`, `AddShelter`, `AddSpotClear`, `AddPlanting`, `RemovePlanting`), `Plan` (`Approve`, `CancelApproved`, `Execute`).
- **All fields, constructors, `Advance`, Save/Load (`CaptureSave` … `DraftOrderCount`), reads and private helpers stay in `MultiAreaContinuation.cs`.** That file remains the single home of `sync` and of the root commit. Instance field initialisers must not be split across parts, for the same ordering reason as §5.4.

### 5.6 Move proof (zero logic delta)

`build/2d-gate/move-proof.py` (python3 standard library) checks two things for both splits:

- (a) **line-multiset equality:** the multiset of non-header lines in the new files equals the multiset of lines of the original file(s) at `T0`, minus the removed lines and plus the added lines listed in §5.2 (i)–(iv), and nothing else;
- (b) **block contiguity and order:** each member moves as one contiguous block, and blocks keep their original relative order within each target file.

The output report is committed evidence (§8, packet S0a).

---

## 6. Locked decision C — S0b: fixed compile-time engine seams (no plugin framework)

### 6.1 Single lock and commit authority (invariant)

`MultiAreaContinuation` stays the **only** holder of `sync` and the **only** code that assigns `root`. Feature modules (annual steps, codecs, future 2A/2B/3 code) receive read-only inputs and scratch accumulators. They never assign live state, take the lock, or publish anything. S0b adds no reflection, no dynamic loading, no DI container, no service locator and no runtime registration. Every seam is a compile-time list.

### 6.2 `AnnualPipeline` (illustrative signatures only)

```csharp
// src/Core/MultiArea/AnnualPipeline.cs  (integrator-owned)
internal interface IAnnualStep
{
    string Name { get; }
    void Run(YearInProgress year); // reads year.Start (immutable root); writes only year's scratch fields
}

internal sealed class YearInProgress
{
    // Start-of-year root (read-only), hasWork, year, revision, nextAreas[], records[], reports[], lostCrop, woodland,
    // stage4, deadwood list, nextDeadwood, phase2Start, density list, yearRecord, nextStage5, nextPhase2,
    // and the test-only afterArea seam delegate. Exactly the locals of today's Advance body, nothing new.
}

internal static class AnnualPipeline
{
    // Fixed, compile-time, ordered. No registration API.
    internal static readonly IAnnualStep[] Steps =
    {
        AdultGrowthStep.Instance,        // per-area loop: adult step, Crop-mark pruning, afterArea seam
        ReservedPhase3DisturbanceStep.Instance, // RESERVED, EMPTY in S0b; provisional position, owned by the Phase 3 spec
        DeadwoodStep.Instance,           // Stage 4 fallen-deadwood records, area order then death order
        RegenerationStep.Instance,       // Phase2Annual.Step | Stage5Annual.Step, promotions, consented-cancel clears
        YearRecordStep.Instance,         // MultiAreaYearRecord from adult-step rows
        ReservedPhase2BYearEndSummaryStep.Instance, // RESERVED, EMPTY in S0b; read-only summary slot for Phase 2B
    };
}
```

`Advance` keeps the guards (fence, consent, empty-stand), builds the `YearInProgress` from the current root, runs `AnnualPipeline.Steps` in order, invokes `beforeCommit`, and performs the **one** root assignment exactly as today. Reserved steps are true no-ops: they compute nothing and touch no field. **The reserved positions are provisional.** Phase 3 and Phase 2B decide them in their own specs. The Phase 3 slot is placed before Deadwood only so that future disturbance deaths could become deadwood; that is a modelling assumption, not an established fact, and the Phase 3 spec must confirm it.

**Permitted statement relocations (the only ones):** realising the locked slot order requires exactly two relocations of pure computations, relative to today's statement order (§3.4):

- **M1 — Deadwood hoist.** The Stage 4 deadwood capture (today inside each area's loop iteration, 1049–1061) runs in `DeadwoodStep` after all adult steps. It iterates areas in the same index order and deaths in the same report order, using each area's stepped roster and the same `nextDeadwood` counter. The record numbering is therefore identical.
- **M2 — Year-record placement.** `new MultiAreaYearRecord(year, year + 1, revision, root.WorkRevision, records, hasWork)` (today 1081–1082, before regeneration) runs in `YearRecordStep` after regeneration. Its inputs (`records`, `hasWork`, `revision`, `root.WorkRevision`) are not modified by regeneration.

Known, reviewed side effects of M1 and M2: the only observable difference is the **test-only** `afterArea` invocation count when an internal invariant fails (`byId[death.TreeId]` missing), which cannot happen by construction, and the order of an unreachable `checked(year + 1)` overflow. In both cases nothing is committed, before or after the change. Any other relocation is forbidden.

### 6.3 Mandatory safety valve (exact decision point)

- **Decision point DP-S0b**, recorded **before** the S0b worker commits any pipeline routing. The worker implements the pipeline types and the extraction with M1 and M2 only, on the branch. The extraction is then **retained (abandoned)** if **any** trigger fires:
  - **T1:** any of the 9 anchors, the v2 golden, or the Step 1 skip-vs-manual parity case `P1-TH-02` changes or fails;
  - **T2:** any relocation beyond M1 and M2 is needed;
  - **T3:** any edit is needed in a biology or rules file (`AnnualRules.cs`, `CrownModel*`, `Stage4*`, `Stage5*`, `Phase2*`, `MultiAreaGenerator.cs`, `ManifestGenerator.cs`, `DerivedManifest.cs`) beyond the RngSalts literal substitution of §6.4;
  - **T4:** any existing test's assertion, expected refusal code or expected exception type would have to change;
  - **T5:** the independent reviewer rejects the M1/M2 equivalence argument;
  - **T6:** the worker cannot show that the reserved steps are no-ops.
- **RETAIN shape:** the `Advance` body stays byte-identical to `T0` (verified by `move-proof.py` member equality). `AnnualPipeline.cs`, `IAnnualStep`, `YearInProgress` and the reserved slot types may still ship as unused, documented types if the reviewer agrees. The rest of S0b (RngSalts, `IFeatureBlockCodec`, checklist, frozen contracts) ships normally.
- **Recording:** the outcome, `EXTRACTED` or `RETAINED (trigger Tn)`, is written to `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-SAFETY-VALVE-DECISION-<YYYYMMDD>.md`. It names the trigger and evidence, and it is confirmed by the independent reviewer **before** the merge. A silent partial (some steps routed, others inline, with no record) is a hard FAIL of S0-06. The default first attempt is set by OQ-5.

### 6.4 `RngSalts.cs` — centralised RNG key prefixes

- New `src/Core/RngSalts.cs` (integrator-owned): `internal static class RngSalts` with one `internal const string` per family in §3.5, holding **byte-identical** values (`"ADULT-MORT-"`, `"ADULT-MORT-GENERIC-"`, `"ADULT-MORT-GENERIC-QUOTA-"`, `"PHASE2-MAST-"`, `"PHASE2-PJ-"`, sub-key suffixes `"/vegetation"` and `"/browse"`, `"STAGE5-MAST"`, `"STAGE5-PROMO-"`), plus a `Families` registry array used by the test.
- Every call site in §3.5 replaces the literal with the constant. The resulting string value must be identical. For the interpolated promotion key, `$"{RngSalts.Stage5Promotion}{areaId}-{column}-{row}-{year}"` with the same `InvariantCulture`. Nothing else changes in those lines (subject to OQ-6).
- **New automated test** `tests/Core.Tests/RngSaltsTests.cs` (one registration line in `tests/Core.Tests/Program.cs`). It asserts:
  - (a) every constant equals its frozen literal (literals duplicated in the test on purpose);
  - (b) all registered family values are pairwise distinct;
  - (c) **no registered value is a prefix of another**, except the pairs listed in an explicit `FrozenNestedPrefixes` allow-list, which must contain exactly `ADULT-MORT-` ⊂ `ADULT-MORT-GENERIC-`, `ADULT-MORT-` ⊂ `ADULT-MORT-GENERIC-QUOTA-` and `ADULT-MORT-GENERIC-` ⊂ `ADULT-MORT-GENERIC-QUOTA-`;
  - (d) for each allow-listed pair, **collision freedom of generated keys**: across MA-1 and MA-1-MIXED genesis plus a 30-year run (including `PL-` promotions), no tree ID starts with `GENERIC-` or `QUOTA-` and no area ID starts with `QUOTA-`;
  - (e) any family registered after S0 must satisfy (c) with **no** allow-list entry.
- The G6 gate extension fails on any quoted RNG key-prefix literal in `src/Core` outside `RngSalts.cs`. The pattern list comes from the registry.

### 6.5 `IFeatureBlockCodec` (interface defined, v3 **not** implemented)

```csharp
// src/Core/Phase1/… is frozen; this lives at src/Core/Saves/IFeatureBlockCodec.cs (integrator-owned). Illustrative.
internal interface IFeatureBlockCodec
{
    string BlockKey { get; }              // ordinal-unique key inside the v3 "blocks" object, e.g. "genesisRecipe", "economy"
    int CurrentBlockVersion { get; }      // version this build writes
    bool CanRead(int blockVersion);       // false ⇒ load refused (fail-closed); never guessed or upgraded silently
    string Encode(MultiAreaRoot root);    // canonical compact JSON object text (UTF-8, fixed key order, no whitespace)
    MultiAreaRoot Decode(string canonicalBlockJson, int blockVersion, MultiAreaRoot restoredSoFar); // throws P1Refusal
}
```

S0b ships the interface with **zero implementations and zero references** from v2 code. The gate greps that no type implements it. The exact signature is finalised in S0b review and must compile against existing types only.

### 6.6 Frozen future cross-lane contracts (documented here, implemented later)

**C-V3 — Save v3 feature-block envelope (implemented by Phase 2A):**

- `schemaVersion` = `3` (integer). A strict canonical envelope with the same rules as v2: strict UTF-8, no BOM, duplicate keys refused, exact key sets, canonical re-encode byte equality, size cap, checksum over all canonical bytes except the checksum object.
- A top-level `blocks` object whose keys are **ordinal-sorted** and unique. Each value has exactly `blockVersion` (integer ≥ 1) and `data`.
- **Fail-closed:** an unknown block key → refused; an unknown `blockVersion` → refused; a missing block required by the session's model selection → refused; any extra or missing envelope key → refused. Nothing is ever ignored, defaulted or repaired.
- Ownership: block `genesisRecipe` is owned by **Phase 2A**; block `economy` is **reserved for Phase 2B** (absent until 2B ships).
- `Phase1SaveCodec` v2 stays **frozen read-only**: no v3 field leaks into v2 and `src/Core/Phase1/**` is not edited for v3.
- **No silent v2→v3 migration.** A v2 file is either refused or converted only by an explicit, user-visible action that writes a **new** v3 slot or file. A v2 file is **never rewritten in place**.
- Exact refusal codes, the format marker and block payload schemas are Phase 2A decisions. The rules above are frozen now.

**C-ECON — Economy Work-Plan seam (implemented by Phase 2B):**

- Economy attaches **primarily to atomic Work Plan execution**: `Pending → Quote → Approve → Execute → Settle`.
- Quote is a pure, read-only computation over the pending orders and the current committed root, bound to `PlanStateRevision`. Approve freezes the quote into the approved plan.
- Settle is computed under the same `sync` acquisition as `Execute` and published in the **same single root assignment**: work and money commit together or not at all.
- Settle is keyed by the existing idempotent execution key (`planId + ":execute"`). A replay returns the stored receipt **including** its settlement and never re-settles. A key conflict refuses, as today. Retries can therefore never double-charge or double-pay.
- Economy is **not** placed arbitrarily inside annual biology. Its only annual attachment is the reserved read-only `ReservedPhase2BYearEndSummaryStep` (§6.2). D-062 timber detail belongs to the Phase 2B spec.

**C-FENCE — Revision-fenced split API (implemented by 2A/2B/4 UI lanes):**

- Any future composed read (e.g. `/stand` plus `?lanes=`) and every per-lane or per-feature readout carries the same `fence` object as today's `/multi` responses: `sessionId`, full `source` token (domain, experimentId, manifestContentHash, areaIds, ecologicalYear, annualRevision, workRevision, rngModelVersion, growthModelVersion, p1AlgorithmVersion, areaModelVersion, actionEligibleDbhThresholdMm) and `planStateRevision` (see `MultiFenceMembers`, `PlayerMultiApi.cs:30–37`).
- Each response is produced under **one** lock acquisition. The browser composes a frame **only** from responses whose fences are byte-identical. On any mismatch it discards the whole frame and re-reads; it never mixes years or revisions across responses.
- Server-side, per-lane reads accept a fence and refuse stale ones with the existing codes (pattern: `ReadCompositeUiFor(source, planStateRevision)`).

### 6.7 Unlock

S0b merged to `integration/2d-sim` with a green gate **unlocks Phase 2B Economy engine coding in a parallel lane** under the Phase 2B spec. Phase 2B persistence still depends on C-V3 being implemented by Phase 2A (see checklist §12, item 4).

---

## 7. Locked decision D — S0c: zero-build native ES modules and the map-renderer plug

### 7.1 Module tree and mapping principle

The extracted `src/Ui/multi/multi.js` is split into native `<script type="module">` files under `src/Ui/multi/`: `main.js`, `api.js`, `store.js`, `dom.js`, `map/contract.js`, `map/renderer-svg.js`, plus `modes/`, `panels/` and `dialogs/`. The default section→module table is in `plan.md` §2.3. **Each banner section moves whole into exactly one module.** Sections keep their original relative order inside a module. Original indentation is preserved byte-for-byte, with no re-indentation and no reformatting. There is zero npm, zero bundler or transpiler, zero React/Vue/Blazor and zero forestry math in JS (D-020). CSS stays one file.

### 7.2 Permitted transformations (exhaustive)

- **P1 — Section moves.** Whole sections move to their mapped module. The single named exception: `wire()` (7342–7517) and the four bootstrap statements (7519–7522) move to `main.js`.
- **P2 — Wrapper removal.** Remove `(function () {`, `})();` and `'use strict';` (modules are always strict). The three header comment lines move to `main.js`.
- **P3 — Module syntax.** Add `export` before top-level declarations used by other modules. Add single-line `import { … } from './….js';` declarations at the top of each module. Add a one-line provenance comment per module.
- **P4 — Cycle breaking.** Replace an upward or cyclic direct call `f(…)` with `hooks.f(…)`. `store.js` exports `export const hooks = Object.seal({ … })`, with one entry per inverted function. `main.js` assigns every hook before the bootstrap statements. The full inventory (function, original call sites, layer) is committed evidence. Calling an unassigned hook throws, which is fail-closed. This cannot happen at module evaluation, because the only top-level statements are the bootstrap in `main.js`.
- **P5 — Renderer plug.** Calls from outside `map/` into map drawing functions go through the `svgRenderer` contract object, whose methods delegate 1:1 with identical arguments and call order. The call-site map is committed evidence.
- **P6 — Serving.** The single permitted HTML byte change is replacing `<script src="multi.js" defer></script>` with `<script type="module" src="multi/main.js"></script>`. In the C# route table, the `multi.js` route is removed and one explicit route `multi/<relative-path>` is added per module, each serving its embedded resource with `text/javascript; charset=utf-8`. `SitkaP1.Ui.csproj` lists each module as `EmbeddedResource` with an explicit `LogicalName`. `multi.js` is deleted only after its content is fully redistributed; this deletion is authorised only once this spec is approved.

Nothing else is allowed: no renames, no logic, behaviour or DOM change, and no CSS change. Module layer order (imports point only downward): `store.js` → `dom.js` → `api.js` → `map/*` → `dialogs/*`, `modes/*`, `panels/*`, `commands.js` → `main.js`.

### 7.3 Renderer contract `map/contract.js`

```js
/**
 * Map renderer contract (S0c). Renderers draw exactly the frame they are given; they compute no forestry (D-020).
 * @typedef {object} MapFrame        The latest atomic composite payload slice already held in store.state (read-only).
 * @typedef {{kind: 'tree'|'cell'|'juvenile'|null, areaId: string|null, id: string|null}} MapSelection
 * @typedef {object} MapRendererDeps  Callbacks owned by main.js (e.g. onMapClick) and the fixed label lookup.
 * @typedef {object} MapRenderer
 * @property {(container: Element, deps: MapRendererDeps) => void} mount   Attach map DOM inside container; called once.
 * @property {(frame: MapFrame) => void} render                             Draw exactly this frame.
 * @property {(selection: MapSelection) => void} setSelection               Reflect a selection the page already decided.
 * @property {(areaId: string) => void} focusArea                           Frame the named area; changes no data.
 * @property {() => void} destroy                                           Detach listeners and DOM; idempotent.
 */
export const MAP_RENDERER_METHODS = Object.freeze(['mount', 'render', 'setSelection', 'focusArea', 'destroy']);
/** @param {unknown} candidate @returns {MapRenderer} @throws {TypeError} when a method is missing. */
export function assertMapRenderer(candidate) { /* checks typeof candidate[m] === 'function' for each method */ }
```

`map/renderer-svg.js` exports `svgRenderer`, which satisfies the contract now. Every method maps 1:1 onto existing functions (e.g. `mount` → `initFloorDefs`, `render` → `renderMap`). A method with no existing 1:1 behaviour is a documented thin delegate, or a no-op that S0c never calls; it adds no new rendering behaviour. Phase 4's `map/renderer-maplibre.js` can replace `svgRenderer` behind the same contract without touching callers. Contract conformance (`assertMapRenderer(svgRenderer)`) is checked by the test harness, not by the page, so the page gains no new runtime statement.

### 7.4 Security surface after S0c

CSP, headers, guards and the listener are unchanged. Same-origin module scripts are allowed by the existing `script-src 'self'`. Module requests are GETs. If a browser attaches an `Origin` header, it equals the admitted origin and passes the existing guard. Every module route is an exact dictionary key, so there is no traversal and no filesystem access.

### 7.5 Verification

- **Reconstruction proof (gate G2, static):** `build/2d-gate/js-reconstruct.mjs` (Node built-ins only) deterministically reverses P2–P5 using the committed hook and call-site maps. It concatenates modules in the mapped section order and must reproduce the **S0b-tip `multi.js` byte-for-byte**.
- **Module graph (gate G2, no bundler):** `build/2d-gate/module-graph.mjs` (Node built-ins only) checks that:
  - every import is a static, single-line, top-of-file declaration;
  - there is no dynamic `import(` and no re-export;
  - every specifier starts with `./` or `../`, ends in `.js`, resolves inside `src/Ui/multi/`, exists, and has an allow-listed route;
  - every module is reachable from `main.js`;
  - a three-colour DFS finds **no cycle**. The topological order is written to evidence;
  - the served HTML has exactly one `<script>` element (the module entry) and no inline script.
- **Parse check:** each served module, copied to a scratch `.mjs`, passes `node --check`.
- **Headless harness:** `tests/ui-smoke-multiarea.sh` is updated (S0c-owned). The M-series asset checks cover every module route. The J-check DOM stand-in loads the **served** modules through Node's `vm.SourceTextModule` (`--experimental-vm-modules`), with a linker that fetches only same-origin module routes. `assertMapRenderer(svgRenderer)` runs there.
- **No remote origin:** the static specifier rule, plus CSP, plus the browser driver's network log, which must show only `127.0.0.1:<port>` requests.
- **Edge parity (browser tier B, required):** a new driver `tests/s0c-parity/run-parity-edge.sh` + `drive-parity.mjs` (same Windows-Edge/WSL2 pattern as the existing drivers, `--headless=new`, DSF 1) runs scripted states at **1280×720 and 1920×1080** for MA-1 and MA-1-MIXED. States: initial load; tree selected with competition; Fell draft pending; approved plan; after Execute; after Year +1; Saved games dialog; history drawer; structure panel. They run once against the S0b trunk tip (reference) and once against the S0c tip. **Primary oracle:** the serialised `document.body.outerHTML` at each state is byte-identical. **Secondary:** screenshots are pixel-identical. Any differing pixel needs a diff image and reviewer acceptance as a non-semantic browser artefact (e.g. caret), recorded in evidence. The existing Edge suites must also pass unchanged.

### 7.6 Unlock

S0c merged with a green gate (including tier B) **unlocks parallel Phase 2A and Phase 2B UI coding**, under their own specs and the ownership map (§13).

---

## 8. Packets

Three packets, strictly in order **S0a → S0b → S0c**. Each is independently reviewed by a non-author route (AgentWorkflow: GPT-6.1 Sol preferred; a suitably independent Opus 5.5 route otherwise; never the author's thread, never a coordination-only route). Each is merged alone with `--no-ff`. Detailed tasks are in `tasks.md`.

**Global entry condition E-0** (all packets): this spec is `APPROVED`; OQ answers are recorded; the consolidated Phase 1 baseline `T0` is verified, R3-approved and owner-accepted; `integration/2d-sim` is created at `T0` (task T-00); the Manager has registered the packet's single writer in `Docs/Project/coordination/active-tasks.md` (this spec does not edit that file).

### S0a — Mechanical moves + 2D trunk gate

- **Goal:** create `2d-gate.sh` and its data; extract `/multi` HTML/CSS/JS verbatim into `src/Ui/multi/`; split the `/multi` route partial and the continuation commands mechanically; zero behaviour change.
- **Owned files (exact):**
  - `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh`
  - `Tools/MultiRackSitkaP1Lab/build/2d-gate/{frozen-anchors.json, protected-paths.txt, determinism-allowlist.txt, baseline-counts.json, s0a-multi-asset-identity.json, s0a-move-table.json, raw-literal.py, move-proof.py, selftest.sh}`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/multi/{multi.html, multi.css, multi.js, .gitattributes}`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/{MultiRoutes.cs, MultiApi.View.cs, MultiApi.Commands.cs, MultiApi.Saves.cs, MultiApi.History.cs}`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (removed by `git mv` into `MultiRoutes.cs` plus moves)
  - `Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj` (EmbeddedResource / None Remove lines only)
  - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` (members removed by move only)
  - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.Commands.{Crop,Fell,Prune,Clear,Planting,Plan}.cs`
  - `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-*.md`
- **Forbidden:** everything else, in particular `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Ui/wwwroot/**`, `src/Core/Phase1/**`, all biology/rules files, `fixtures/**`, every test source, `Tools/MultiRackSitkaLab/**`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/**`.
- **Entry:** E-0; the T-00b baseline record exists.
- **Exit:** S0-01, S0-02, S0-03, S0-04, S0-05 and S0-REG pass on the rebased HEAD; independent review APPROVED on that exact HEAD; merged `--no-ff`; the gate is green on the merge commit.
- **Stop if:** the closing raw-literal delimiter is not at column 0; `P_x ≠ R_x`; any protected file would need an edit; a static field would have to leave `MultiRoutes.cs`; the gate cannot run offline on a clean checkout; any anchor, golden or test count changes.
- **Review gate RG-S0a:** an independent reviewer checks the identity JSON, the move-proof report, the gate log, the self-test log and the diff scope.

### S0b — Fixed engine seams + frozen contracts

- **Goal:** `AnnualPipeline` (or the recorded RETAIN), `RngSalts` plus its test, `IFeatureBlockCodec` (interface only), the subsystem checklist, and the frozen contracts C-V3, C-ECON and C-FENCE.
- **Owned files (exact):**
  - `src/Core/MultiArea/AnnualPipeline.cs` (contains `IAnnualStep`, `YearInProgress`, the steps and the reserved steps)
  - `src/Core/MultiArea/MultiAreaContinuation.cs` (`Advance` routing only, or untouched under RETAIN)
  - `src/Core/RngSalts.cs`
  - the literal-substitution lines in `src/Core/AnnualRules.cs`, `src/Core/MultiArea/Phase2MixedDensity.cs`, `src/Core/MultiArea/Phase2Rules.cs` and `src/Core/MultiArea/Stage5Mast.cs` (subject to OQ-6)
  - `src/Core/Saves/IFeatureBlockCodec.cs`
  - `tests/Core.Tests/RngSaltsTests.cs`
  - one registration line in `tests/Core.Tests/Program.cs`
  - the G6 registry-pattern update in `build/2d-gate/determinism-allowlist.txt` / `2d-gate.sh`
  - `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/{S0B-*.md, S0B-SAFETY-VALVE-DECISION-<date>.md}`

  All paths are under `Tools/MultiRackSitkaP1Lab/` except the `Docs/` ones.
- **Forbidden:** any other Core or biology line; `src/Core/Phase1/**`; any v3 encoder or decoder; any UI file; every other test assertion.
- **Entry:** S0a merged and the trunk green; OQ-5 and OQ-6 answered.
- **Exit:** S0-06, S0-07, S0-08 and S0-REG pass; the safety-valve decision record exists and is reviewer-confirmed; review APPROVED; merged `--no-ff`; the gate is green.
- **Stop if:** any trigger T1–T6 fires (→ RETAIN and record; never push on); a key literal would change; `IFeatureBlockCodec` cannot compile against existing types; anything needs `src/Core/Phase1/**`.
- **Review gate RG-S0b:** an independent reviewer confirms the M1/M2 equivalence (or the RETAIN), the RngSalts byte-identity and test, the zero `IFeatureBlockCodec` implementations, and that the contracts in §6.6 are present unchanged.

### S0c — Native ES modules + map renderer plug

- **Goal:** split `multi.js` into native modules per §7 with zero behaviour, DOM or design change.
- **Owned files (exact):**
  - `src/Ui/multi/{main.js, api.js, store.js, dom.js, commands.js, map/contract.js, map/renderer-svg.js, modes/*.js, panels/*.js, dialogs/*.js}`
  - `src/Ui/multi/multi.html` (script-tag line only)
  - `src/Ui/multi/multi.js` (removed)
  - `src/Ui/MultiRoutes.cs` (asset route lines only)
  - `src/Ui/SitkaP1.Ui.csproj` (EmbeddedResource lines only)
  - `build/2d-gate/{module-graph.mjs, js-reconstruct.mjs, s0c-hook-inventory.json, s0c-renderer-callsite-map.json}` and the G2 wiring in `2d-gate.sh`
  - `tests/ui-smoke-multiarea.sh` (M-series asset checks and J-loader only)
  - `tests/s0c-parity/{run-parity-edge.sh, drive-parity.mjs}`
  - `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0C-*.md`
- **Forbidden:** `multi.css`; any CSS, DOM-producing logic or label change; any `api/multi/*` handler; Core; `wwwroot/**`; every other test.
- **Entry:** S0b merged and the trunk green; the reference parity capture taken at the S0b trunk tip.
- **Exit:** S0-09 and S0-REG pass including tier B; review APPROVED; merged `--no-ff`; the gate (with `--browser`) is green.
- **Stop if:**
  - a section would need splitting, beyond the `wire()`/bootstrap exception;
  - any top-level statement other than the bootstrap would be needed;
  - the hook inventory exceeds 20 functions, or the layering would invert wholesale;
  - `vm.SourceTextModule` is unavailable on the recorded Node version;
  - the body-HTML parity differs at any state;
  - the CSP would need any change.
- **Review gate RG-S0c:** an independent reviewer checks the reconstruction proof, the module graph, the hook and call-site inventories, the parity evidence (both viewports, both experiments) and the network logs.

---

## 9. Acceptance criteria

Each criterion is one objectively checkable statement. Evidence paths assume the OQ defaults.

| ID | Criterion | Verification method | Evidence artifact |
|---|---|---|---|
| **S0-01** | `origin/integration/2d-sim` was created pointing exactly at the owner-accepted consolidated Phase 1 baseline `T0` named in the owner acceptance record. Every first-parent commit after `T0` on the trunk is a `--no-ff` merge or a recorded revert/repair. No commit reachable from `origin/main` has a trunk or 2D-feature-branch commit as a merge parent. | At T-00, immediately after the push: `git ls-remote origin refs/heads/integration/2d-sim` == `T0` (recorded). Later: `git merge-base --is-ancestor T0 origin/integration/2d-sim`; `git log --first-parent --format='%H %P' T0..origin/integration/2d-sim` shows only two-parent merges or recorded reverts; `git log origin/main --merges --format=%P` contains no trunk commit | `S0A-TRUNK-CREATION-RECORD-<date>.md` in the handoff directory |
| **S0-02** | `2d-gate.sh` runs G0 then G1–G7 in the locked order, emits a `GATE <stage> PASS` marker per stage, returns 0 only when every stage passes, and returns the specified non-zero code (1/2/3/4/5/64) for each planted violation in `selftest.sh`, from a clean clone with no network access required. | `build/2d-gate.sh <ev>` on the trunk tip; `build/2d-gate/selftest.sh <ev>` | gate log and self-test log summarised in the S0a handoff; raw logs external |
| **S0-03** | For each of `multi`, `multi.css` and `multi.js`, the SHA-256 of the S0a-tip served body equals `R_x` (from the `T0` blob by the §5.3 rule), equals `P_x` (the `T0` served body), and equals the committed file's SHA-256. Response headers are identical except `Date`. | gate G2 S0a sub-check (`raw-literal.py` + live GETs) | `build/2d-gate/s0a-multi-asset-identity.json` |
| **S0-04** | Gate G7 returns 4 for any changed path outside `Tools/MultiRackSitkaP1Lab/` and `Docs/`, or on the deny-list (including `Assets/`, `Packages/`, `ProjectSettings/`, `src/Ui/wwwroot/**`). The cumulative S0 diff from `T0` contains no such path. The `src/Ui/wwwroot` tree id equals `T0`'s. | gate G7 on each packet and on the cumulative trunk diff; self-test planted paths | gate logs; self-test log |
| **S0-05** | The route partial split (`MultiRoutes.cs`, `MultiApi.{View,Commands,Saves,History}.cs`) and the continuation command split (`MultiAreaContinuation.Commands.*.cs`) pass the line-multiset and block-order move proof with only the §5.2 (i)–(iv) exceptions. All static fields of the `/multi` partial are in `MultiRoutes.cs` in original order. No member is renamed. | `build/2d-gate/move-proof.py` on `T0` vs the S0a tip; G1/G2 green | move-proof report attached to the S0a handoff |
| **S0-06** | Either (EXTRACTED) `Advance` runs `AnnualPipeline.Steps` in the order AdultGrowth, ReservedPhase3Disturbance, Deadwood, Regeneration, YearRecord, ReservedPhase2BYearEndSummary, with only relocations M1 and M2, and anchors, golden and `P1-TH-02` unchanged; or (RETAINED) the `Advance` member text is byte-identical to `T0`. In both cases a reviewer-confirmed safety-valve decision record names the outcome. | `move-proof.py` (member equality or the M1/M2-only diff); gate G2–G4; record inspection | `S0B-SAFETY-VALVE-DECISION-<date>.md` |
| **S0-07** | `src/Core/RngSalts.cs` holds every simulation RNG key prefix with byte-identical values. No RNG key-prefix literal remains elsewhere in `src/Core`. `RngSaltsTests` passes: frozen values, pairwise distinct, prefix-free except exactly the three allow-listed nested `ADULT-MORT-*` pairs, collision freedom of generated keys, and new families strictly prefix-free. | Core test run (G2); gate G6 registry grep | Core test output (summarised in the S0b handoff) |
| **S0-08** | The Save v3 envelope contract C-V3 is present in this approved spec, unchanged. `IFeatureBlockCodec` exists with zero implementing types. `Phase1SaveFormat.SchemaVersion == 2`. `src/Core/Phase1/**` is unchanged from `T0`. No v3 encoder or decoder exists in `src/`. | gate G4; `git grep` for `: IFeatureBlockCodec` (no hits) and `SchemaVersion = 3` (no hits); spec diff | gate log; S0b handoff |
| **S0-09** | The `/multi` JavaScript runs as native ES modules per §7. The reconstruction proof reproduces the S0b-tip `multi.js` byte-for-byte. The module graph is acyclic with only local relative specifiers. `svgRenderer` passes `assertMapRenderer`. At 1280×720 and 1920×1080, for MA-1 and MA-1-MIXED, every scripted state has byte-identical `document.body.outerHTML` against the S0b reference, with screenshots identical or reviewer-accepted. The network log shows only loopback requests. | gate G2 (`js-reconstruct.mjs`, `module-graph.mjs`, updated `ui-smoke-multiarea.sh`); gate tier B (`tests/s0c-parity/run-parity-edge.sh`) | `s0c-hook-inventory.json`, `s0c-renderer-callsite-map.json`, the parity summary in the S0c handoff; screenshots external |
| **S0-REG** | Across the whole S0 pass there is zero behaviour change: no gameplay, biology, constant, RNG-stream, UI-design or save-format change. All 9/9 frozen anchors equal §3.8. The v2 golden equals §3.7. All suites pass with counts ≥ baseline and no test removed or weakened. Payload budgets hold. The existing Edge suites pass. The protected listeners are unaffected. | gate G1–G7 (plus B at S0c) on every packet and on the final trunk tip; test-source diff limited to the listed additions | final S0 closure handoff, `S0-CLOSURE-<date>.md` |

---

## 10. Out of scope — hard exclusions

- **Zero behaviour changes of any kind:** no gameplay features, no biology, constant or RNG changes, no UI redesign, no new labels, no layout or CSS change, no payload shape change.
- **No Phase 2A work:** no Forest Maker, no 0.64 ha presets, no Save v3 codec. **No Phase 2B work:** no Economy, no D-062 timber. **No Phase 3 work:** no Jay dispersal, storms or Species Wave 2. **No Phase 4 work:** no MapLibre, no imports.
- No dynamic plugin loaders, reflection-based registration, DI frameworks or service locators. No JS build pipeline, npm, bundler or transpiler. No new NuGet or npm packages.
- **Zero edits** to Unity `Assets/` (including the linked `Assets/ForestPrototype/SimulationRandom.cs` and `SitkaGrowthModel.cs`), Compact30, Legacy40, Enlarged80, Scenario One Reference Future v1, Unity `ForestSaveData`, the protected single-area `src/Ui/wwwroot/**`, the legacy `Tools/MultiRackSitkaLab/**`, `Packages/`, `ProjectSettings/`, `main`, and the protected local listeners `:33767`, `:43649`, `:46593`.
- No edit to `Docs/Project/**` (including `coordination/active-tasks.md`) from any S0 branch. Coordination and canonical docs follow the Manager route.

## 11. Compatibility contracts

| Contract | Rule | Proved by |
|---|---|---|
| Frozen anchors | All 9 values in §3.8 unchanged at every merge | G3 |
| Save v2 | `SchemaVersion = 2`; codec and validator unchanged; golden pin unchanged; `src/Core/Phase1/**` read-only | G4 |
| Determinism | No F1 token; F2 only at allow-listed sites; RNG keys byte-identical and centralised; the pipeline adds no state, ordering or draw | G6, G3, S0-07 |
| Protected paths | Only `Tools/MultiRackSitkaP1Lab/` and `Docs/` change; deny-list respected; `wwwroot` and the P0 tree pinned | G7; `build-p0-core-ref.sh` |
| Listeners | One loopback listener on an ephemeral port; never `:5187`; protected local listeners never bound, killed or stopped | `ui-smoke*.sh`; gate before/after record |
| HTTP surface | Guards, CSP, headers, MIME and the exact route allow-list unchanged (S0c changes only the asset routes, per P6) | S0-03, S0-09, `ui-smoke-multiarea.sh` |
| Payload budgets | view ≤ 524,288 B; ecology ≤ 153,600 B | G5 |

## 12. "Adding a subsystem" integrator checklist (for post-S0 lanes)

When a future lane adds an engine subsystem `<Feature>`, the **integrator** verifies each item. The lane writes only its own files and the integrator applies the shared-file lines:

1. **Root property:** a nullable `<Feature>State? <Feature> = null` appended **last** to the positional `MultiAreaRoot` record (after `Phase2`). It is `null` on every root whose model selection lacks the feature, so existing roots and anchors are byte-identical with the feature off.
2. **`HasUnexecutedWork`:** if the feature has orders, add `|| (<Feature>?.Pending….Count ?? 0) > 0` (`MultiAreaContinuation.cs:34–35`). `Advance`'s consented cancellation must clear the feature's pending and approved orders, and `Execute` must revalidate them all-or-nothing.
3. **Options equality:** extend `RequireSameOptions` (`MultiAreaContinuation.cs:1225`) with `Same<Feature>`, so a save is never continued under different feature options.
4. **Save codec registration:** v2 is frozen and never gains the feature. Register an `IFeatureBlockCodec` (block key + version) in the v3 registry once Phase 2A implements C-V3. Until then a feature with persistent state must make Save **refuse** with a typed code. It must never silently drop state.
5. **Pipeline / Work-Plan slot:** annual effects go into a named `IAnnualStep` slot (a reserved slot or a new slot by reviewed spec amendment). Player actions become Work Plan order types flowing `Pending → Approve → Execute` (and C-ECON's Quote/Settle where economic). Never a direct mutation.
6. **Also:** register any new random key family in `RngSalts` (strictly prefix-free); keep all 9 anchors exact with the feature off; add view/readout DTOs inside the payload budgets; give UI readouts the C-FENCE stamps; and add tests in the lane's own test file, with the registration line applied by the integrator.

## 13. Ownership map (post-S0)

| Owner | Files |
|---|---|
| **Integrator-owned (locked by decision D)** | `src/Ui/multi/main.js`, `src/Ui/multi/api.js`, `src/Ui/multi/store.js`, `src/Ui/MultiRoutes.cs`, `src/Core/MultiArea/AnnualPipeline.cs`, `src/Core/RngSalts.cs` |
| Integrator-owned (proposed by this spec as shared infrastructure) | `src/Ui/multi/{dom.js, commands.js, map/contract.js, multi.html, multi.css}`; `src/Ui/SitkaP1.Ui.csproj`; `src/Core/MultiArea/MultiAreaContinuation.cs` (root file: lock, fields, `Advance`, Save/Load); `src/Core/Saves/IFeatureBlockCodec.cs`; `build/2d-gate.sh` and `build/2d-gate/**`; registration lines in `tests/Core.Tests/Program.cs` |
| **Parallel-lane-owned** | `src/Core/<Feature>/**`; `src/Ui/multi/panels/<feature>.js`; `src/Ui/multi/dialogs/<feature>.js`; proposed: `src/Ui/multi/modes/<feature>.js`, `src/Ui/MultiApi.<Feature>.cs`, `src/Core/MultiArea/MultiAreaContinuation.Commands.<Feature>.cs`, `tests/Core.Tests/<Feature>Tests.cs` |
| Phase 4 lane (later) | `src/Ui/multi/map/renderer-maplibre.js` (behind `map/contract.js`) |
| Nobody on the trunk | `src/Ui/wwwroot/**`, `src/Core/Phase1/**`, `fixtures/**`, `Tools/MultiRackSitkaLab/**`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/**` |

## 14. Open Questions for Owner Approval

Workers must not decide any of these. Each has a recommended default that applies only if the owner approves the spec without overriding it.

| # | Question | Recommended default |
|---|---|---|
| **OQ-1** | Exact `/multi` asset directory path. | `Tools/MultiRackSitkaP1Lab/src/Ui/multi/`: a sibling of the protected `wwwroot/`, embedded resources only, `-text` attributes (§5.1). |
| **OQ-2** | Location of the gate script and its data. | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh` plus `build/2d-gate/`, beside the existing `build/build-p0-core-ref.sh`. |
| **OQ-3** | Where the 9/9 anchor list (and v2 golden pins) live for the gate, given that only 7/9 are code-pinned and #7/#9 exist only as printed lines. | A committed JSON manifest `build/2d-gate/frozen-anchors.json` holding all 9 anchors and the three v2 golden values. The gate cross-checks it against the test literals (#1–6, #8) and parses Core stdout for #7 and #9. No test edits in S0. Adding code-level pins for #7/#9 and committing byte-exact golden save files are deferred to a later reviewed packet (recommended alongside Phase 2A's v3 goldens). |
| **OQ-4** | Where the extraction byte-identity evidence is committed (`evidence/` is git-ignored). | The gate-input proof JSON in `build/2d-gate/s0a-multi-asset-identity.json`; human-readable packet evidence in `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/`; large raw logs and screenshots off-repo under `/media/jer/Files/CCF/Verification/` per `evidence/README.md`. |
| **OQ-5** | S0b safety-valve default on the first attempt: extract or retain. | **Extract, single attempt**, with relocations M1 and M2 only. Any trigger T1–T6 → automatic **RETAIN** with a reviewer-confirmed decision record. A second extraction attempt only by a new owner decision. |
| **OQ-6** | RngSalts scope, given the measured nested legacy prefixes: (a) accept the exact three-pair `ADULT-MORT-*` allow-list with collision-freedom tests instead of a literal "no prefix of another" over the frozen keys; (b) permit literal→constant substitution in the frozen-biology files `AnnualRules.cs`, `Phase2MixedDensity.cs`, `Phase2Rules.cs`, `Stage5Mast.cs`. | (a) **Yes**. Renaming any key would change RNG streams and anchors, which S0-REG forbids. New families are strictly prefix-free. (b) **Yes**, value-identical substitutions only, proven by all anchors. If the owner says no to (b), the constants exist and the gate allow-lists those exact literals. |

Additional owner items (not product decisions in this spec): record locked decisions A–D in `Docs/Project/decision-log.md` on approval; and confirm whether the browser tier B should become mandatory for every UI-touching trunk merge after S0. Recommended: yes.

## 15. Global stop conditions

Stop and report with evidence, never work around, if:

1. `T0` is not owner-accepted, or the trunk would be cut from anything else.
2. Any change would touch `Assets/`, `Packages/`, `ProjectSettings/`, `src/Ui/wwwroot/**`, `Tools/MultiRackSitkaLab/**`, `src/Core/Phase1/**`, `Docs/Project/**` or `main`.
3. Any anchor, golden, payload budget, test count or Edge suite changes, or any behaviour difference appears.
4. A packet would need a transformation not listed in §5.2, §6.2 (M1/M2), §6.4 or §7.2.
5. The gate cannot be made fail-closed, offline and clean-checkout runnable.
6. An Open Question is unanswered at packet entry.
7. A required independent review route is unavailable. Do not substitute self-review.

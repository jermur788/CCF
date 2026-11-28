# CCF Multi 2D — Woodland Inspection V1 — Part 2 Implementation Handoff

**Playable Milestone 3 — Part 2: Real-World Woodland Inspection & Import Workflow on `/multi`**

**D-030 state: implemented and repaired (Repair R1) on the isolated branch `feat/multi-2d-woodland-inspection-part2-v1`. NOT merged to `integration/2d-sim` or `main`. No PR opened. Independent review returned NEEDS CHANGES on `eaae40a4`; Repair R1 is in §8; delta re-review pending.**

> **Correction (Repair R1).** The original §6 claim that a mutation "that passed the guard a moment before a concurrent load commits is ordered before that load" was **false**: the admission guard ran before the request body was read, so a paused Advance could commit after a survey opened (reproduced by the reviewer). The fix and its proof are in §8; §2.3 and §6 below are corrected.

- **Builder:** Claude Opus 5.5 — approved fallback after the Gemini worker hit an individual quota error before executing.
- **Spec:** `specs/multi-2d-woodland-inspection-v1/` — status `APPROVED / Part 2 IMPLEMENTING` (spec §8, tasks Part 2, checklist WN-P2-REQ-01..09). Part 1 history kept.

## 1. Refs

| Ref | SHA |
|---|---|
| Base `origin/integration/2d-sim` (verified after `git fetch` at task start) | `b2006767cbe70fa2c822497470908251a0fd6ffd` |
| Protected `origin/main` (untouched) | `bdeb6a324acaf4129c10fb0846faac8b64974360` |
| Source + spec commit (all gates below ran on this exact tree) | `076c902484227097fd776f2c599b70fbb670f431` |
| Feature `feat/multi-2d-woodland-inspection-part2-v1` = review `review/multi-2d-woodland-inspection-part2-v1` | the commit that adds this file (docs only, child of `076c9024`); its SHA is in the push/read-back report, because a commit cannot record its own SHA |

Compare URLs:

- https://github.com/jermur788/CCF/compare/b2006767cbe70fa2c822497470908251a0fd6ffd...feat/multi-2d-woodland-inspection-part2-v1
- https://github.com/jermur788/CCF/compare/integration/2d-sim...review/multi-2d-woodland-inspection-part2-v1

**Branch note.** In the shared local clone, the local name `feat/multi-2d-woodland-inspection-part2-v1` was already checked out (clean, at base) in the previous Gemini worker's worktree. To avoid disturbing that worktree, the work was committed on a local branch `p2/woodland-inspection-part2-v1` and pushed with explicit refspecs to the exact remote names `refs/heads/feat/multi-2d-woodland-inspection-part2-v1` and `refs/heads/review/multi-2d-woodland-inspection-part2-v1` (same SHA).

## 2. What changed

### 2.1 Server (pure .NET; no forestry or import maths in the browser)

- **`src/Ui/MultiWoodlandInspection.cs`** (new `PlayerApi` partial):
  - Built-in catalogue is loaded lazily, once, through `BuiltInWoodlandCatalogue.Load(readFixtureFile)`. `readFixtureFile` reads embedded resources with `Assembly.GetManifestResourceStream`. Fully offline: no upload, path or network input exists. A load failure becomes a typed refusal (`WoodlandInspectionUnavailable`), never a 500 loop.
  - Fixed plain-English display labels for every enum value used in the view (readiness, species categories with notes, provenance, vital status, severity, field names, units, "Not recorded", banner title/lead).
- **`src/Ui/SitkaP1.Ui.csproj`**: embeds `ATTRIBUTION.json`, `ie-donadea-mhf-subset.dwca.zip` and `lu-mersch-oak-beech-subset.dwca.zip` directly from `../../tests/Core/WoodlandImport/Fixtures/Qualified/` (not copied, not edited; the importer re-checks each pinned SHA-256), and the new JS module.
- **`src/Ui/PlayerApi.cs`**: one central guard call in `HandleAsync`, after the Host, capability, route/method and Origin guards and before any handler runs.
- **`src/Ui/MultiRoutes.cs`**: registers the inspection routes and the new module route.
- **`src/Ui/MultiApi.Woodland.cs`**: a committed New woodland clears the inspection inside the `AtomicReplace` lock, after the candidate is built and immediately before the swap.
- **`src/Ui/MultiApi.Saves.cs`**: a committed Load clears it in the same lock block. A refused New woodland or Load leaves it unchanged.
- **Part 1 engine (`src/Core/WoodlandImport/Inspection/**`) is unchanged**, as are the closed importer files and `tests/Core/WoodlandImport/**`. `git diff --name-only b2006767..076c9024` over every forbidden path is empty.

### 2.2 API contract

| Route | Body | 200 response | Refusals |
|---|---|---|---|
| `GET api/multi/woodland/inspection/catalogue` | — | `{entries[], inspection, refusal:null}`. Entries are in catalogue order (Donadea, then Mersch). Each entry has `plotId, displayName, countryCode, treeCount, livingStemCount, subsetOfGbifRecords, readiness, readinessLabel, canAdvanceTime, advanceTimeStatement, licenceSummary, licenceUrl, datasetTitle, publisher, citation, doi, doiUrl, datasetKey, retrievalDateUtc, archiveSha256, observationDates, modificationsDisclosure, caveats` | 400 for a query string; 409 `WoodlandInspectionUnavailable` |
| `POST api/multi/woodland/inspection/load` | `{"plotId": "..."}` (strict; unknown members are refused) | `{current:{entry, view, viewSha256, summary{treeCount, plottedTreeCount, unplottedTreeCount}, labels}, inspection, refusal:null}`. `view` is the Part 1 canonical JSON verbatim, and `viewSha256` equals the Part 1 goldens | 400 malformed; 409 `WoodlandInspectionPlotUnknown`; 403 foreign Origin |
| `POST api/multi/woodland/inspection/exit` | `{}` | `{wasActive, inspection}` (idempotent) | 400 malformed |
| `GET api/multi/woodland/inspection/current` | — | `{inspection, current or null}` | 400 for a query string |

`inspection` is `{active, plotId, displayName, canAdvanceTime}`.

### 2.3 Fail-closed guard coverage

While the inspected plot has `CanAdvanceTime == false` (both built-ins), the guard checks every POST before its handler runs. A POST outside the allow-list below gets **HTTP 409**: `{"error":{code,message},"refusal":{code,message},"inspection":{...}}` with code **`WoodlandInspectionReadOnly`**. The message is plain English, for example: "Advance one year is not available in survey inspection mode. Donadea MHF 2 plot (Ireland) is a Year-0 survey that cannot be simulated, so nothing changed. Choose "Return to normal woodland" to keep playing."

- **Allowed POSTs:**
  - inspection load/exit;
  - `api/multi/woodland/new` and `api/multi/load` (both clear the inspection on commit);
  - read-only POSTs: woodland/preview, hit, cell, scots-pine/tree, competition, year/preview-review, history/year, economy/quote, economy/history/year, stand, single-area hit/competition.
- **Refused (every other POST, including any route added later):**
  - Advance (with and without consent) and Skip;
  - Work Plan approve, execute and cancel;
  - Crop, pending, prune, clear, plant, shelter, spot-clear and planting edits;
  - Save and save recovery;
  - Money switch, economy executor and economy disposition;
  - the single-area page's mutations.
- **GETs are never blocked.** No silent advance happens, and no height, age, crown, position, species or boundary is fabricated.
- **Two checkpoints (Repair R1).** The check in `HandleAsync` is an *admission* fast path (`"checkpoint":"admission"`), made before the body is read. The authoritative check is made at *commit*: every mutating handler calls `InspectionBlockerLocked()` inside the `multiHost.SyncLock` block that guards its commit, after its body is read and immediately before the engine call or slot write (`"checkpoint":"commit"`). Inspection load/exit, New woodland and Load save change the inspection under that same lock, so a command and a survey opening are always wholly ordered.

### 2.4 `/multi` page (presentation only; D-020)

- **New module:** `multi/dialogs/woodland-inspection.js` (L4; added to the module-graph `LAYER` table).
- **Small hooks in existing modules:**
  - `store.js`: `state.inspection`, `INSPECTION_LOCKED_IDS`, and `resetSessionUiState` clears it.
  - `api.js`: `call()` sends no simulation POST while inspecting, and `renderCommandGate()` disables the locked controls last and restores their original titles afterwards.
  - `commands.js` / `time-history.js`: Advance and Skip handlers return early while inspecting.
  - `saves.js`: Save and Recover are disabled while inspecting.
  - `main.js`: import, render, wire, a boot restore after the unchanged boot reads, and the stand-map hotkeys off while inspecting.
- **Entry point and dialog:** an "Inspect real woodland" button sits immediately after New woodland. The catalogue dialog lists Donadea first and Mersch second.
- **Banner:** a persistent "Survey inspection mode — Year 0" banner shows:
  - the plot and its readiness;
  - the blocker checklist (titles and explanations in plain English, affected species);
  - the attribution line (dataset, publisher, CC BY 4.0 link, DOI link, "Modified by CCF: 20 of N GBIF records");
  - visibly disabled Advance / Skip / Approve / Execute controls, plus the disabled header Advance/Skip;
  - "Return to normal woodland".
- **Survey screen:** it replaces the stand map workspace and has exactly two columns:
  - **Map:** a flat local-metre SVG (X east, Y north as stored), a 10 m reference grid and axis captions. No boundary is drawn, because none was recorded. Markers show position only, not crown size. The note states how many trees are plotted and how many are unplotted.
  - **Tree inspector:** for each of the 7 fields, the value or "Not recorded", plus provenance, source column, raw value and the importer note.
  - **All surveyed trees:** a list of every surveyed tree.
  - **Species mapping:** all four categories, plus per-raw-name counts.
  - **TreMs records:** codes are shown as the dataset's own source codes, with no interpretation.
  - **Source, licence and changes:** citation, licence, DOI, GBIF dataset key, plot ID, observation date, retrieval date, subset, archive SHA-256, the CC BY 4.0 modifications disclosure, caveats and the Year-0-only statement.
- **Return to normal woodland:** posts `exit`, re-reads, and restores the unchanged playable woodland with Advance enabled. A page reload restores an open inspection from `current`.

### 2.5 Tests

- **New:** `tests/woodland-inspection-part2-api.sh` (mode 100755). It boots one fresh process on an ephemeral loopback port with a scratch fixture and scratch XDG data, and checks:
  - **C:** catalogue;
  - **L:** load, run for both plots;
  - **R:** refusals;
  - **V:** validation;
  - **X:** exit, New, Load and stale-fence behaviour;
  - **Z:** clean SIGINT, empty stderr, unchanged lab tree and git status.
- **Amended:** `tests/ui-smoke-multiarea.sh` J01. It still requires the page to boot with exactly `GET api/multi/stand` then `GET api/multi/view`. It now also accepts a single trailing read-only `GET api/multi/woodland/inspection/current`, which the page issues after those boot reads. No other smoke check was changed.

## 3. Verification (all on tree `076c9024`; .NET SDK 8.0.134 at `/home/jer/.dotnet/dotnet`, global.json 8.0.133 latestPatch; Linux node v26.13.1)

Run from `Tools/MultiRackSitkaP1Lab` in WSL Ubuntu.

| # | Command | Result |
|---|---|---|
| 1 | `dotnet restore` + `dotnet build --no-restore` of `MultiRackSitkaP1Lab.sln`, `tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj`, `tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj`, `src/Ui/SitkaP1.Ui.csproj` | all four **0 Warning(s), 0 Error(s)** |
| 2 | `dotnet run --project tests/Core/WoodlandInspection/SitkaP1.WoodlandInspection.Tests.csproj --no-build --no-restore` | **TOTAL: 20/20 passed, 0 failed** (goldens `b53c6339…beb6` / `e01bbfad…b9a4` unchanged) |
| 3 | `dotnet run --project tests/Core/WoodlandImport/SitkaP1.WoodlandImport.Tests.csproj --no-build --no-restore` | **TOTAL: 29/29 passed, 0 failed** |
| 4 | `dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj --no-build --no-restore` | **326/326 passed, 0 failed**; 9/9 frozen anchors exact (§4) |
| 5 | `DOTNET=~/.dotnet/dotnet bash tests/woodland-inspection-part2-api.sh <evidence>` | **89 passed, 0 failed** |
| 6 | `node build/2d-gate/module-graph.mjs` | `MODULE-GRAPH PASS: 17 modules, acyclic, all reachable from main.js, 1 module script tag` |
| 7 | `DOTNET=~/.dotnet/dotnet NODE=<linux node> bash tests/ui-smoke-multiarea.sh <evidence>` (UI built first) | **OVERALL: PASS (363 passed, 0 failed)** |
| 8 | Real Edge (headless, CDP) at 1920×1080 and 1280×720, fresh server per viewport | **1920×1080: 69 passed, 0 failed. 1280×720: 69 passed, 0 failed. OVERALL: PASS (139 passed, 0 failed)** |

**Gate 8 coverage, per viewport.**

- 66 driver checks:
  - Advance is enabled in normal play.
  - For each of Donadea and Mersch:
    - the dialog order is Donadea, Mersch;
    - catalogue rows show the tree count and CC BY 4.0;
    - focus lands on the banner;
    - the banner title is exactly "Survey inspection mode — Year 0", with the plot and readiness;
    - the blocker checklist is shown;
    - CC BY 4.0, DOI and the modification notice are shown;
    - the workspace is replaced by exactly 2 columns;
    - **20 Year-0 markers** are drawn, with "All 20 … plotted" (0 unplotted);
    - the map is flat X east / Y north with no boundary;
    - the inspector shows 7 fields, and crown radius and age read "Not recorded" with provenance;
    - observed height shows its value and source column;
    - position shows X east / Y north and its source;
    - 4 species categories plus per-name rows;
    - TreMs records are listed;
    - the full source section is shown (citation, licence, DOI, date, changes);
    - all 20 trees are listed;
    - Advance, Skip, Approve, Execute and the lock buttons are visibly disabled;
    - Return is visible;
    - no internal codes appear on screen;
    - there is no horizontal overflow;
    - the button sits beside New woodland inside the viewport;
    - a click on the disabled Advance changes nothing;
    - **the server refuses Advance, Skip and Work Plan execute with 409 `WoodlandInspectionReadOnly`**;
    - a map click selects a tree.
  - A reload restores the open survey with Advance still disabled.
  - Return to normal woodland: the server inspection is cleared, the previous view is restored (same year), and Advance is enabled with its original title.
  - **Advance works in the UI** (year 0 → 1).
  - **Zero console errors or page exceptions, zero 5xx responses and zero 4xx responses** seen by the browser.
- 3 runner checks: the server stopped cleanly on SIGINT (exit 0), empty stderr, no server left running.
- Overall: git status of the lab is unchanged (Z04).

The browser driver and runner were disposable and lived outside the repo (`%TEMP%\ccf-p2-edge\drive-inspection.mjs`, `/tmp/p2/edge.sh`). Screenshots (`insp-{0-normal,1-donadea,2-mersch,3-restored}-{1920x1080,1280x720}.png`) and logs are kept outside the repo under WSL `/tmp/p2/final/`. They were not committed (no binary footprint was added).

**Skipped as instructed:** `build/2d-gate/selftest.sh`, Phase1Save.Tests, `run-step2-edge.sh`, full `build/2d-gate.sh --browser`, Unity.

## 4. Frozen anchors (verbatim Core.Tests lines; 0 drift)

Each value was compared with `build/2d-gate/frozen-anchors.json` and with its source pin.

| # | Anchor | Pinned SHA-256 | Core.Tests line | Result |
|---|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `model0 single-area fingerprint 7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | EXACT |
| 2 | Multi-area 0/0/0/0 | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `model0 multi-area fingerprint a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | EXACT |
| 3 | Multi-area 0/0/1/0 (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `model1 multi-area fingerprint 814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | EXACT |
| 4 | Stage 4 1/1/1/0 | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `S4 stage4 run fingerprint 8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | EXACT |
| 5 | Stage 5 1/1/1/1 | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `S5 1/1/1/1 full-run fingerprint c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | EXACT |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `S5-07A 5.0 m purity fingerprint c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8 (pinned, measured at cb739cbd: c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8)` | EXACT |
| 7 | Phase 2 Q6 (1/1/1/1/1 vs 1/1/1/1/0) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | `P2-03 M-08 Q6 hash 1/1/1/1/0 776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d \| 1/1/1/1/1 776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | EXACT |
| 8 | P0 Core binary `.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | `P2-01 P0 Core binary 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (also `sha256sum .p0ref/SitkaLab.Core.dll`) | EXACT |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | `P2-10 MA-1-MIXED-v1 30-year fingerprint 93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | EXACT |

**`ANCHORS EXACT 9/9`.** No engine or simulation file changed, so no drift was expected and none was observed.

**P0 pin.** This worktree had no `.p0ref/`. Running `build/build-p0-core-ref.sh` with SDK 8.0.134 produced a *different* DLL (`bcbdbf05…97b0`), because the pin was built with SDK 8.0.133. The verified pinned binary (`9a99f781…34b3`, provenance `sdk=8.0.133`) was therefore copied byte-for-byte from an existing qualified worktree (`CCF-HP-Worktrees/CCF-Multi-2D-Year30Compaction-20261128`, read only), and the Core project's hash gate accepted it. The pin was not changed. Ports 33767, 43649 and 46593 were not touched: all servers used OS-assigned ephemeral ports and were stopped.

## 5. `git diff --stat b2006767..076c9024`

```
 .../build/2d-gate/module-graph.mjs                 |   1 +
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs |   3 +
 .../src/Ui/MultiApi.Woodland.cs                    |   6 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiRoutes.cs    |   3 +
 .../src/Ui/MultiWoodlandInspection.cs              | 382 ++++++++++++++++++
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs      |   3 +
 Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj |  12 +
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/api.js      |  28 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/commands.js |   2 +-
 .../src/Ui/multi/dialogs/saves.js                  |   7 +-
 .../src/Ui/multi/dialogs/woodland-inspection.js    | 447 +++++++++++++++++++++
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/main.js     |   9 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css   |  85 ++++
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.html  |  79 ++++
 .../src/Ui/multi/panels/time-history.js            |   2 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/multi/store.js    |   8 +
 .../tests/ui-smoke-multiarea.sh                    |   9 +-
 .../tests/woodland-inspection-part2-api.sh         | 274 +++++++++++++
 .../checklists/requirements.md                     |  14 +
 specs/multi-2d-woodland-inspection-v1/plan.md      |   2 +-
 specs/multi-2d-woodland-inspection-v1/spec.md      |  31 +-
 specs/multi-2d-woodland-inspection-v1/tasks.md     |  16 +-
 22 files changed, 1404 insertions(+), 19 deletions(-)
```

The final branch adds this handoff file on top. `git status --short` after both commits: empty.

## 6. Honest limitations and unverified items

- **Not merged or reviewed.** No independent review has run yet. Nothing is on `integration/2d-sim` or `main`.
- **Overlay, not integration.** The inspection does not enter `MultiAreaContinuation`, `AnnualPipeline` or saves. A survey whose `CanAdvanceTime` were true would still only be inspected: the guard would not block, and the hidden playable woodland would remain the one that advances. Both built-ins are InventoryOnly, so this path cannot occur today; it needs its own packet.
- **Guard ordering (corrected in Repair R1).** The original claim here was false; see §8. Ordering now rests on the commit-time recheck inside `multiHost.SyncLock`. Remaining boundary: a command that takes the lock *before* a survey opens commits normally and is ordered before the survey (K11 control); that is correct, not a leak.
- **Single-area page.** The guard also refuses the single-area page's mutating POSTs while a `/multi` inspection is open (fail-closed by design). That page shows no banner for it.
- **Map framing.** The frame fits the recorded tree positions only, which is not a boundary. Positions follow the Part 1 caveat that reading `verbatimCoordinates` as plot-local metres is an unverified inference; this is shown verbatim in the Source section.
- **TreMs codes.** They are shown as recorded (for example `CV32`). CCF does not translate them, so no ecological meaning is claimed.
- **Smoke amendment.** J01 in `tests/ui-smoke-multiarea.sh` was deliberately amended (§2.5). A reviewer should confirm the amendment keeps the check's intent.
- **Browser driver host.** The Edge driver ran on Windows `node.exe`, as the existing `drive-*.mjs` suites do. WSL uses NAT networking, so Linux node cannot reach Edge's loopback CDP port. Every build, test, smoke, API and module-graph gate used Linux node and .NET 8 only. The driver is disposable and not committed; its checks are listed in §3.
- **Not tested:** screen readers, keyboard-only end-to-end use beyond Enter/Space on map markers, and any viewport other than the two required.
- **Not run (skipped as instructed):** Unity, Phase1Save.Tests, the 2d-gate selftest and the full browser gate.

## 8. Repair cycle 1 (independent review follow-up)

**D-030 state: repaired on the isolated branch `feat/multi-2d-woodland-inspection-part2-v1`. NOT merged to `integration/2d-sim` or `main`. An independent delta review is pending.**

Source: GPT-6.2 Sol independent review of `eaae40a4df1027f3f0c2fbdaf53b16c8716d7fe1` (source/spec `076c9024`) — **INTEGRATION DECISION: NEEDS CHANGES**. Repair implementer: Claude Opus 5.5.

**Repair commit: `8e986e63cadb8496f82d1cf98423d3dc90ccd816`** (source, tests, spec), parent `eaae40a4`. This section is in a separate docs commit on top of it; the final feature/review SHA is in the push/read-back report.

### 8.1 Findings and fixes

| Finding | Fix |
|---|---|
| **BLOCKER** — a command admitted before inspection could mutate after inspection started (admission check before the body read; no recheck at commit) | `InspectionBlockerLocked()` is called inside the existing `multiHost.SyncLock` commit block, after the body is read, immediately before the engine call or file write, in: `MultiCommandAsync` (crop, pending, prune, clear, plant, shelter, spot-clear, planting remove, economy executor and disposition, plan approve/execute/cancel, year advance), `SkipMultiYearsAsync`, `MultiSaveAsync` (capture **and slot write** now inside the lock, so a save cannot land after a survey opens), `MultiRecoverAsync` (now under the lock), `SwitchMultiEconomySessionAsync`, and the single-area `CommandAsync` (now an instance method, committing under the same lock). No lock is held across a body read. New woodland and Load save stay allowed and still clear the inspection under the same lock. A blocked commit returns the same 409 `WoodlandInspectionReadOnly` body with `"checkpoint":"commit"` and changes nothing. |
| **MAJOR** — overlapping loads returned contradictory `current` vs `inspection` | Load builds both halves from the entry it committed; current, catalogue and exit build status from one read (exit reports the inactive status of its own commit). |
| Nit — harness checked only the declared `viewSha256` | The server now writes the Part 1 canonical bytes verbatim (`CanonicalJson` raw converter). The harness extracts the exact `view` bytes from each reply and hashes them against the goldens (L-*-view BYTES, O03). |
| Nit — checklist "12 routes ×2" | Corrected: 13 refused request cases over 10 distinct mutating routes, 2 checks each (+R01, R04), plus K (commit-time) cases. |

### 8.2 New concurrency tests (`tests/woodland-inspection-part2-api.sh`)

- **K (paused request bodies).** A raw socket sends only the headers of a valid command while no survey is open (it passes admission), the harness opens a survey, then releases the body. `checkpoint == "commit"` proves the request had passed admission and was stopped by the in-lock recheck. That is the reviewer's race, not a sequential refusal.
  - K01–K03 multi **Advance**: **409 `WoodlandInspectionReadOnly` (checkpoint commit); playable year unchanged (1 → 1, same session and revisions); Donadea stays active.**
  - K04–K05 **Skip**: 409 commit, nothing changed. K06–K07 **Save**: 409 commit, no slot written. K08 **Recover**: 409 commit before any slot file is touched. K09–K10 **single-area Advance**: 409 commit, single-area session unchanged.
  - K11 control: the same paused Advance with no survey opened commits (200, year +1), proving the technique does not refuse on its own.
- **O (overlapping loads).** 50 pairs of Donadea/Mersch loads released together by a barrier: **100 replies, all 200, 0 contradictory** (current plot = status plot = requested plot), **0 hash mismatches** (declared and actual view-byte SHA-256 equal that plot's golden); GET current afterwards self-consistent; exit reports an inactive status from its own commit.
- **Negative proof (disposable, not committed).** With the `MultiCommandAsync` recheck removed and the old status re-read restored, the same harness failed **K01, K02** (the paused Advance committed and the year moved) and **O02** (contradictory replies), plus K05/K11 as knock-ons of the moved year: 103 passed, 5 failed. Both files were restored byte-for-byte (sha256 verified) and rebuilt before the final run.

### 8.3 Verification on repair tree `8e986e63` (.NET 8.0.134, Linux node v26.13.1)

| Gate | Result |
|---|---|
| Builds: sln, WoodlandInspection.Tests, WoodlandImport.Tests, Ui | all **0 Warning(s), 0 Error(s)** |
| WoodlandInspection.Tests | **20/20** |
| WoodlandImport.Tests | **29/29** |
| Core.Tests | **326/326**; **ANCHORS EXACT 9/9** (same values as §4, 0 drift) |
| `tests/woodland-inspection-part2-api.sh` | **108 passed, 0 failed** (89 original + K01–K11, O01–O05, R04, L-view-BYTES ×2) |
| `node build/2d-gate/module-graph.mjs` | **PASS: 17 modules, acyclic, all reachable, 1 module script tag** |
| `tests/ui-smoke-multiarea.sh` | **363 passed, 0 failed** (J01 amendment unchanged; boot order unchanged) |
| Edge CDP | **1920×1080: 69 passed, 0 failed; 1280×720: 69 passed, 0 failed; OVERALL PASS (139/0)**, 0 console errors, 0 4xx/5xx in the browser, servers stopped cleanly |

Forbidden-path diff against base (including `src/Core/WoodlandImport/**`, `tests/Core/WoodlandImport/**`, `src/Ui/wwwroot`, `src/Core/Phase1`, `Docs/Project`, Unity paths): empty. Part 1 engine unchanged.

`git diff --stat eaae40a4..8e986e63`:

```
 .../src/Ui/MultiApi.Commands.cs                    |  55 +++++---
 .../MultiRackSitkaP1Lab/src/Ui/MultiApi.History.cs |  23 +++-
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs |  64 ++++++---
 .../src/Ui/MultiWoodlandInspection.cs              | 104 +++++++++-----
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs      |  24 +++-
 .../tests/woodland-inspection-part2-api.sh         | 149 ++++++++++++++++++++-
 .../checklists/requirements.md                     |   6 +-
 specs/multi-2d-woodland-inspection-v1/spec.md      |   5 +-
 specs/multi-2d-woodland-inspection-v1/tasks.md     |   3 +-
 9 files changed, 348 insertions(+), 85 deletions(-)
```

### 8.4 Remaining limitations after R1

- Save now holds `multiHost.SyncLock` during its slot write (a small, bounded file write), so reads wait for that write; this was chosen over letting a save land after a survey opened.
- **Superseded by R2 (§8.5):** R1's Save took the slot lock and then `multiHost.SyncLock` — the reverse of Recover — and could deadlock.
- The paused-body tests rely on an 0.8 s pause after the headers. If the server were too slow to reach the body read, the request would be refused at admission, and the `checkpoint == "commit"` assertion would then **fail**. The pause cannot produce a false pass.
- Everything in §6 still applies, except the corrected guard-ordering item.

### 8.5 Repair R2 — one lock order for Save and Recover

Coordinator follow-up on the R1 diff: R1's `MultiSaveAsync` held `saveV3Slots.LockSlot(slot)` and then took `multiHost.SyncLock`, while `MultiRecoverAsync` holds `multiHost.SyncLock` and then `RecoverFromBackup` takes the same slot lock — a lock inversion that could deadlock a Save and a Recover of one slot.

**R2 commit: `f8dbac6c75c088265fb1b5895b59913220147f75`** (parent `89612ab1`); this subsection is in the docs commit on top of it.

- **Fix.** `MultiSaveAsync` now takes `multiHost.SyncLock` first, runs the inspection commit check, and only then takes the slot lock for `NextGeneration`, the `SlotExists` check, capture and the write. A blocked Save is therefore refused (`WoodlandInspectionReadOnly`, checkpoint `commit`) before it reads any slot metadata or can answer `SlotExists`.
- **The one lock order:** `multiHost.SyncLock` → save-slot lock. Documented on `MultiSessionHost.SyncLock`, Save and Recover. Audit of every `LockSlot`/slot-lock site: Save (host → slot), Recover (host → slot); `NextGeneration`/`Save`/`RecoverFromBackup` take the slot lock only inside those; `ReadMultiSavesAsync` and `MultiLoadAsync` read slot files only after releasing the host lock and take no slot lock (`SaveV3Slots.Load`/`Inspect` do not lock). No path takes a slot lock and then the host lock.
- **New checks (API harness S01–S05):** 30 simultaneous Save + Recover pairs on one slot, each request bounded by a 20 s timeout: **0 timeouts, 30/30 Saves 200, 30/30 Recovers 200 or typed 409**; reads still answer and the playable woodland is unchanged; a blocked Save to an **existing** slot is refused for the survey at commit, before any `SlotExists` check.
- **Negative proof (disposable, not committed):** with R1's `MultiApi.Saves.cs` restored temporarily, the same harness **deadlocked** — S01–S04 failed (request timeout, server stopped answering reads). The R2 file was restored byte-for-byte (`cmp`) and rebuilt; no server was left running.

**R2 verification (.NET 8.0.134, Linux node v26.13.1):**

| Gate | Result |
|---|---|
| Build `src/Ui/SitkaP1.Ui.csproj` and `MultiRackSitkaP1Lab.sln` | **0 Warning(s), 0 Error(s)** |
| `tests/woodland-inspection-part2-api.sh` | **113 passed, 0 failed** (108 from R1 + S01–S05), including K01–K11 paused-body and O01–O05 overlapping-load checks |
| `node build/2d-gate/module-graph.mjs` | **PASS: 17 modules, acyclic, all reachable, 1 module script tag** (unchanged) |
| `tests/ui-smoke-multiarea.sh` | **363 passed, 0 failed** |
| Engine suites / anchors | **NOT RERUN in R2** — only `src/Ui` (not in any engine test) and the API harness changed; R1 results (20/20, 29/29, 326/326, 9/9 anchors exact) stand for the unchanged engine. |
| Edge CDP | **NOT RERUN in R2** — no response shape, page asset or JS changed (Save's 200/409 bodies are byte-for-byte the same shapes); R1 Edge results (69/0 at each viewport) stand. The final re-review will rerun key gates. |

`git diff --stat 89612ab1..f8dbac6c`:

```
 Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs | 61 ++++++++++------------
 .../MultiRackSitkaP1Lab/src/Ui/MultiSessionHost.cs |  4 ++
 .../tests/woodland-inspection-part2-api.sh         | 45 +++++++++++++++-
 3 files changed, 76 insertions(+), 34 deletions(-)
```

**D-030: NOT merged to `integration/2d-sim` or `main`.**

## 7. Push / read-back

Pushed with `git push origin HEAD:refs/heads/feat/multi-2d-woodland-inspection-part2-v1 HEAD:refs/heads/review/multi-2d-woodland-inspection-part2-v1`. Verified with `git ls-remote origin` that both refs equal the final local HEAD; the 40-character SHA is in the implementation report returned to the coordinator. `origin/main` stays at `bdeb6a324acaf4129c10fb0846faac8b64974360`.

**D-030: NOT merged to `integration/2d-sim` or `main`.**

# Multi-Area Map Integration — Pass 1 Implementation Handoff

**State (D-030):** `IMPLEMENTED ON TASK BRANCH — READY FOR INDEPENDENT REVIEW — NOT INTEGRATED, NOT ACCEPTED`

---

## 1. Identity, Session Metadata, and Git Provenance

| Field | Value |
|---|---|
| Implementer | Gemini 4 Neon (Antigravity ACP, full-access, effort `high` / `1.00`) |
| Configured Git author note | Worktree Git config is `DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>` (inherited from repo config); actual implementation and verification were executed by Gemini 4 Neon. |
| Delegating T3 thread | `c00d4799-5f15-4a7d-9231-0f133246a64c` |
| Start UTC | `2026-11-24T11:57:27Z` |
| End UTC | `2026-11-24T12:48:00Z` |
| Worktree (only writable checkout) | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass1-20261124` |
| Root checkout | `/home/jer/Development/CCF-HP` (read-only; verified clean and untouched) |
| Task branch | `feat/multi-map-integration-pass1-20261124-v1` |
| Base SHA | `cf6fe7a52aa096e824827c568f43fe05724dcce9` |
| Implementation commit SHA (Commit 4) | `73f9a3893b6d6c32acf46ed8bd35e70b1a6c3b25` |
| Final HEAD SHA | Commit 5 (this handoff commit; exact SHA reported in the Final Report after commit) |
| External evidence archive (outside Git, mode `700`) | `/home/jer/Development/CCF-Multi-MapIntegration-Pass1-Evidence-20261124` |
| Tools used | `read_file`, `write_file`, `edit_file`, `run_command` (PowerShell → `wsl -d Ubuntu`), `/home/jer/.dotnet/dotnet` SDK `8.0.134` (`TMPDIR=/home/jer/tmp`), `python3`, `curl`, Windows `node.exe` `v26.13.1`, real headless Microsoft Edge (`C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`) over CDP |

### Commit sequence on `feat/multi-map-integration-pass1-20261124-v1`

1. `c00e047abea7bfd24ab5992f562b806bc06b0ddd` — `docs(multi-map-pass1): add Pass 1 Map Integration spec quartet and owner visual reference assets`
2. `5f74f8a7c7fb11fba9385085fe0529dfad35befe` — `feat(multi-map-pass1): reconcile Stage 4 Owner UX (0e777455) onto Phase 2 base (cf6fe7a5)`
3. `85ff855773499dfb0ccd35dd4d228fe39f6fdbe2` — `feat(multi-map-pass1): add Pass 1 Core composite snapshot, MA-1-MIXED demo factory, ecology DTO and cell detail`
4. `73f9a3893b6d6c32acf46ed8bd35e70b1a6c3b25` — `Multi Pass 1 UI: pure inline SVG system, overlays, read-only cell endpoint and mixed CLI`
5. Commit 5 (this file) — durable implementation handoff

### `git diff --stat cf6fe7a52aa096e824827c568f43fe05724dcce9..73f9a3893b6d6c32acf46ed8bd35e70b1a6c3b25`

```text
 .../src/Core/MultiArea/MultiAreaContinuation.cs    |  145 +-
 .../src/Core/MultiArea/MultiAreaContracts.cs       |    8 +
 .../src/Core/MultiArea/MultiAreaEcology.cs         |  546 ++++
 .../src/Core/MultiArea/MultiAreaExperiment.cs      |    5 +-
 .../src/Core/MultiArea/MultiAreaUiReadout.cs       |   35 +-
 .../src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs |   58 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | 2668 +++++++++++++-------
 Tools/MultiRackSitkaP1Lab/src/Ui/Program.cs        |   57 +-
 .../tests/Core.Tests/MapIntegrationPass1Tests.cs   |  310 +++
 .../tests/Core.Tests/MultiAreaTests.cs             |    7 +-
 .../tests/Core.Tests/Program.cs                    |    1 +
 .../tests/Core.Tests/Stage4UiCatchupTests.cs       |   19 +-
 .../tests/ui-smoke-multiarea.sh                    |  440 +++-
 .../assets/Beech_Crown_Assets_TopDown_v2.png       |  Bin 0 -> 2787607 bytes
 .../assets/Oak_Crown_Assets_TopDown_v2.png         |  Bin 0 -> 2768004 bytes
 .../assets/Spruce_Crown_Assets_TopDown_v2.png      |  Bin 0 -> 2813069 bytes
 .../assets/Standard_SVG_Asset_Sheet_v2.png         |  Bin 0 -> 2285386 bytes
 .../checklists/requirements.md                     |   76 +
 specs/multi-map-integration-pass1-v1/plan.md       |   74 +
 specs/multi-map-integration-pass1-v1/spec.md       |  171 ++
 specs/multi-map-integration-pass1-v1/tasks.md      |   31 +
 21 files changed, 3676 insertions(+), 975 deletions(-)
```

---

## 2. Owner Visual Reference Assets Committed

All four owner-supplied 1254×1254 PNG reference boards were copied verbatim without re-encoding into `specs/multi-map-integration-pass1-v1/assets/`. The earlier "owner visual asset pending" limitation is **resolved and removed**.

| File | Size (bytes) | Observed SHA-256 (verified match) |
|---|---|---|
| `specs/multi-map-integration-pass1-v1/assets/Standard_SVG_Asset_Sheet_v2.png` | `2,285,386` | `35a44df40c88abeac58fe7d40469dbad2ddbcfbc017e404cd0872d4c34c0227f` |
| `specs/multi-map-integration-pass1-v1/assets/Spruce_Crown_Assets_TopDown_v2.png` | `2,813,069` | `1ce74faaaba24f176df59b41f3d0252e9cb095bce9bcc0577ea436e086dcfb26` |
| `specs/multi-map-integration-pass1-v1/assets/Oak_Crown_Assets_TopDown_v2.png` | `2,768,004` | `edc4237f1748c2a981b3772d8fbf42e3d4a67d8382acc162d56508138a39e787` |
| `specs/multi-map-integration-pass1-v1/assets/Beech_Crown_Assets_TopDown_v2.png` | `2,787,607` | `bd1cfcd6d2e9f167951e78cf33aafebf292ef18e3e277e9658c1f24c322937e2` |

**Conflict rule enforced:** Where `Standard_SVG_Asset_Sheet_v2.png` depicts *Expired Shelter* among Work Plan action icons, the binding brief (§7.6 rule 4) takes precedence: `Expired Shelter` is rendered strictly as a **map and legend state only**, and never appears as a Work Plan action button.

---

## 3. File-by-File Summary of Changes

### Spec quartet and assets (`specs/multi-map-integration-pass1-v1/`)
- `spec.md`: Self-contained Pass 1 Map Integration specification covering `/multi` default `1/1/1/1/1`, `MA-1-MIXED` demo factory, atomic composite snapshot, additive `ecology` DTO and hard byte budgets, read-only `POST api/multi/cell`, pure inline SVG system referencing `Standard_SVG_Asset_Sheet_v2.png` + all 3 species boards by exact path and SHA-256, and Pass 1 mode/overlay/inspector rules.
- `plan.md`, `tasks.md`: Milestone plan, reconciliation strategy (`0e777455` onto `cf6fe7a5`), and task breakdown.
- `checklists/requirements.md`: Requirement verification matrix (`MI-P1-01`..`MI-P1-06`, `MI-REG`, `MI-P1-REVIEW`). Every checkbox remains unticked `[ ]` for independent review.
- `assets/*.png`: Four committed binary visual reference PNGs listed in §2.

### Core additions (`Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/`)
- `MultiAreaEcology.cs` *(new)*:
  - `MultiAreaDemoExperiments`: deterministic demo-only factory `CreateMixedDemo()` for `ma-1-mixed` (Area A 100% Sitka; Area B adult `TreeId`s sorted with `StringComparer.Ordinal`, assigned round-robin `0 -> sitka-spruce`, `1 -> sessile-oak` re-initialised at age 60, `2 -> beech` re-initialised at age 60 via `Phase2Options.GenesisSpecies`), plus canonical truthful badges (`Synthetic MA-1 (all Sitka)` and `Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)`). Never references test assemblies; leaves `MA-1` and `Phase2Support.MixedFixture()` untouched.
  - `MultiAreaSpeciesPruneHelper`: engine-authoritative species-aware pruning readout (`sessile-oak` / `beech`: max 2 lifts at `3.5 m` and `6.0 m`, 3-year wait; `sitka-spruce`: max 3 lifts at `2.5 m` / `5.0 m` / `6.5 m`, 5-year wait).
  - `MultiAreaEcologyDto` + `MultiAreaEcologyBuilder`: compact camelCase additive DTO (`null` when `SpeciesModelVersion == 0`), sparse `speciesByTreeId`, engine-authoritative `pruneStatusByTreeId`, per-rack `areas[]` aggregates, compact numeric `cellsCompact` rows with `cellSchema: ["areaIndex","col","row","light","bramble","bracken","speciesMask","bandCount"]` and non-zero floor preservation (`> 0` never rounded to `0`), `planted[]`, `shelters[]` (engine-derived `expiryYear` and `isEffective`), `patches[]` (`radiusM ≈ 0.564 m`), and read-only `pendingClear[]`, `approvedClear[]`, `pendingPlanting[]`, `approvedPlanting[]`.
  - `MultiAreaCellDetail`, `MultiAreaCellCohortDetail`, `MultiAreaCellDetailResult`, `MultiAreaCompositeUiSnapshot`, `MultiAreaCompositeUiReadout`.
- `MultiAreaExperiment.cs`: Added optional `DisclosureOverride` (`null` on canonical `MA-1`, preserving byte-identical canonical manifest & SHA-256 `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95`).
- `MultiAreaContracts.cs`: Added Pass 1 presentation labels (`MultiNotModelledPhase2`, `MultiLightNotPar`, `MultiDeadwoodSchematic`, `MultiNaturalSeedlingsNote`, `MultiClearUndergrowthRule`, `MultiExpiredShelterRule`).
- `MultiAreaUiReadout.cs`: Accepts optional `MultiAreaPhase2State?` so `stage4` and `ecology` prune readouts are computed from one shared species-aware engine rule.
- `MultiAreaContinuation.cs`: Added single-lock `ReadCompositeUiSnapshot()`, `ReadCompositeUiFor()`, and strictly read-only `ReadCellDetail()` (refusing stale fences and unknown/out-of-bounds cells with a fresh composite snapshot and zero state/revision change).
- `MultiAreaWorkPlanAdapter.cs`: Updated `OpenSynthetic` default model selection to `MultiAreaModelSelection.Phase2` (`1/1/1/1/1`, while Core `MultiAreaContinuation` default stays `0/0/0/0/0`), added `OpenExperiment(string? experimentKey, ...)`, `ExperimentKey`, `ExperimentBadge`, `ReadCompositeUiSnapshot()`, `CompositeFor()`, and `ReadCellDetail()`.

### UI and HTTP server (`Tools/MultiRackSitkaP1Lab/src/Ui/`)
- `Program.cs`: Added strict CLI parsing for `--multi-experiment=ma-1` (default) and `--multi-experiment=ma-1-mixed`. Rejects unknown flags, invalid experiment values, duplicate flags, or unexpected extra positional arguments with exit code `64` and a clear usage message on stderr. No extra listeners and no arbitrary filesystem paths.
- `PlayerMultiApi.cs`:
  - Reconciled Stage 4 Owner UX (`0e777455`, including R1/R2/R3 fold geometry, F-2 unpaired readout guard, F-05 dialog-only Escape) onto `cf6fe7a5`.
  - Replaced multi-lock readout pairing with single-lock `ReadCompositeUiSnapshot()` / `CompositeFor()`, adding optional `ecology` to `GET api/multi/view` and command/hit/competition responses without breaking any existing member.
  - Added read-only `POST api/multi/cell` under the unchanged `PlayerApi` guards (`404`/`403`/`405`/`415`/`413`/`400` + `409` stale/OOB/unknown area, `200` full cell detail + fresh composite snapshot, zero revision change).
  - Appended pure-ASCII `experimentKey`, `experimentBadge`, and `models` after `"contentHash"` on `GET api/multi/manifest`, keeping M07 canonical body-hash verification unbroken for `ma-1`.
  - Removed 100% of `data:image/webp` base64 sprites and replaced them with 21 pure inline SVG `<symbol>`/`<pattern>` definitions in `<svg id="map"><defs>` (`#ccf-sitka`, `#ccf-oak`, `#ccf-beech`, `#ccf-standing-dead`, `#ccf-log`, `#ccf-crop-ring`, `#ccf-sapling-sitka`, `#ccf-sapling-oak`, `#ccf-sapling-beech`, `#ccf-shelter-active`, `#ccf-shelter-expired`, `#ccf-spot-clear`, `#ccf-natural-seedlings`, `#ccf-bramble-hatch`, `#ccf-bracken-hatch`, `#ccf-action-fell`, `#ccf-action-prune`, `#ccf-action-clear`, `#ccf-action-plant`, `#ccf-action-shelter`, `#ccf-action-spot`).
  - Removed Stage 4 elliptical crown stretching (`scale(r*stretch, r/stretch)`); every crown renders strictly circular at `x = u - r`, `y = v - r`, `width = height = 2 * r` with deterministic seeded rotation and preserved transparent true-radius hit disc.
  - Implemented explicit `Select Tree` (default) vs `Inspect Cell` mode toggle (swapping `#cell-panel` in and hiding `#tree-panel`, never stacking; `Escape` exits cell mode preserving draft Fell ticks and active overlay), single ground overlay selector (`Off | Light Guide | Undergrowth | Natural Seedlings`), visible `Modelled light · not calibrated PAR` and `direction schematic, not simulated` labels, species-aware Prune box, non-colour Work Plan state pills (`Pending` dashed gold + clock, `Approved` solid pale-blue + check, `Completed · History Log` dark-slate), and zero Pass 2 write controls.

### Tests (`Tools/MultiRackSitkaP1Lab/tests/`)
- `Core.Tests/MapIntegrationPass1Tests.cs` *(new)*: `MI-P1 C-01..C-06` testing deterministic `MA-1-MIXED` demo factory, single-lock atomic composite snapshot, `ecology` DTO parity/bitmasks/non-zero floors/shelters/patches, species-aware prune readout, strictly read-only `ReadCellDetail`, and raw UTF-8 payload byte budgets at Year 0 and Year +5 on both experiments.
- `Core.Tests/Program.cs`: Registered `MapIntegrationPass1Tests.Run`.
- `Core.Tests/MultiAreaTests.cs`: Updated `MA-W12` adapter public-member allow-list for the 4 additive Pass 1 methods (`OpenExperiment`, `ReadCompositeUiSnapshot`, `CompositeFor`, `ReadCellDetail`).
- `Core.Tests/Stage4UiCatchupTests.cs`: Updated `UI U-02` to verify default `1/1/1/1/1` (`Phase2`) on `/multi` alongside explicit `Stage4` (`1/1/1/0`) and `Legacy` (`0/0/0/0`) openings.
- `ui-smoke-multiarea.sh`: Reconciled Stage 4 Owner UX `F2`/`R2`/`R3`, restored mode `0755`, updated `R2` and `J` DOM selectors for Pass 1 `Inspect Cell` swap / overlay selector / Stage 4 controls, and added Pass 1 checks `M08..M13` (zero WebP, all 21 SVG IDs, manifest badge/models, payload budgets, `--multi-experiment=ma-1-mixed`, invalid CLI exit 64) plus `POST api/multi/cell` HTTP 400/409/200 read-only checks.

---

## 4. Verification Commands and Real Observed Results

All commands were executed inside `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass1-20261124` on Commit 4 (`73f9a3893b6d6c32acf46ed8bd35e70b1a6c3b25`) with `TMPDIR=/home/jer/tmp` and `/home/jer/.dotnet/dotnet` (`8.0.134`).

### 4.1 Clean builds (`--no-incremental --disable-build-servers`)

```bash
export TMPDIR=/home/jer/tmp
/home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln --no-incremental --disable-build-servers --nologo
/home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj --no-incremental --disable-build-servers --nologo
```

- `MultiRackSitkaP1Lab.sln`: **Build succeeded. `0 Warning(s)`, `0 Error(s)`** (`8.73 s`)
- `SitkaP1.Ui.csproj`: **Build succeeded. `0 Warning(s)`, `0 Error(s)`** (`6.38 s`)

### 4.2 Full Core test suite (2 consecutive runs)

```bash
/home/jer/.dotnet/dotnet run --no-build --project Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj
```

- **Run 1:** `251/251 passed, 0 failed` (245 baseline + 6 new `MI-P1 C-01..C-06`)
- **Run 2:** `251/251 passed, 0 failed`
- **`diff -u` of Run 1 vs Run 2 `^(PASS|FAIL)` sets:** empty (byte-identical PASS set across both runs). Logs saved in external evidence archive (`core-run1.txt`, `core-run2.txt`).

### 4.3 All 9 frozen anchors (Expected vs Observed)

| # | Frozen Anchor | Expected SHA-256 | Observed SHA-256 | Status |
|---|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | MATCH |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | MATCH |
| 3 | Multi-area `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | MATCH |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | MATCH |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | MATCH |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | MATCH |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` *(runtime equality in `P2-03 M-08`)* | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` (both `1/1/1/1/0` and `1/1/1/1/1`) | MATCH |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` *(verified before & after)* | MATCH |
| 9 | MA-1-MIXED 30-y fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | MATCH |

### 4.4 `ui-smoke-multiarea.sh` end-to-end witness

```bash
NODE="$wrap" Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh /home/jer/Development/CCF-Multi-MapIntegration-Pass1-Evidence-20261124
```

- Result: **`OVERALL: PASS (170 passed, 0 failed)`**, exit `0`.
- Covers: `S01..S02`, `M01..M13` (including strict CSP headers, `node --check multi.js`, M07 canonical hash, zero WebP, all 21 SVG IDs, truthful badges, payload budgets, `--multi-experiment=ma-1-mixed`, exit 64 invalid CLI), `W001..W094` + `W999` (including `POST api/multi/cell` 400/409/200 read-only checks), `F2a..F2h`, `R2a..R2i`, `R3a..R3j`, `J01..J22` + `J99`, `Z01..Z04` (clean SIGINT shutdown, empty stderr, zero repo file mutations).

### 4.5 Raw UTF-8 Payload Byte Budgets & Browser Parse/Paint Timings

Budgets: `GET api/multi/view` raw UTF-8 JSON **≤ 524,288 B (512 KiB)**; incremental `ecology` object **≤ 153,600 B (150 KiB)**.

#### Core unit test (`MI-P1 C-06`, untouched 5-year run without Year 0 Work Plan execution)

| Experiment | Year | Total raw UTF-8 bytes | Total KiB | `ecology` raw UTF-8 bytes | `ecology` KiB | Budget |
|---|---:|---:|---:|---:|---:|---|
| `ma-1` | 0 | `387,472` | `378.39` | `43,438` | `42.42` | PASS |
| `ma-1` | 5 | `410,538` | `400.92` | `45,385` | `44.32` | PASS |
| `ma-1-mixed` | 0 | `388,736` | `379.63` | `44,393` | `43.35` | PASS |
| `ma-1-mixed` | 5 | `414,571` | `404.85` | `46,961` | `45.86` | PASS |

#### Live loopback HTTP + Real Headless Microsoft Edge (includes executed Year 0 Work Plan of 4 Fells + 1 Prune prior to Year 5)

| Matrix Cell | Year | Total raw HTTP bytes | `ecology` bytes | `fetch` ms | `JSON.parse` ms | `rAF` paint ms | Budget |
|---|---:|---:|---:|---:|---:|---:|---|
| `ma-1 @ 1920x1080` | 0 | `387,472` | `43,428` | `18.3` | `2.0` | `49.7` | PASS |
| `ma-1 @ 1920x1080` | 5 | `411,355` | `45,375` | `28.2` | `2.7` | `18.6` | PASS |
| `ma-1 @ 1280x720` | 0 | `387,472` | `43,428` | `19.1` | `1.3` | `45.1` | PASS |
| `ma-1 @ 1280x720` | 5 | `411,355` | `45,375` | `7.7` | `1.4` | `11.7` | PASS |
| `ma-1-mixed @ 1920x1080` | 0 | `388,736` | `44,371` | `17.9` | `1.3` | `45.2` | PASS |
| `ma-1-mixed @ 1920x1080` | 5 | `415,398` | `46,940` | `31.2` | `1.5` | `16.7` | PASS |
| `ma-1-mixed @ 1280x720` | 0 | `388,736` | `44,371` | `16.9` | `1.4` | `67.6` | PASS |
| `ma-1-mixed @ 1280x720` | 5 | `415,398` | `46,940` | `9.8` | `1.8` | `3.4` | PASS |

### 4.6 Real Loopback + Real Headless Microsoft Edge CDP Matrix

Driver: external dependency-free Node `v26.13.1` CDP script (`pass1-verify.mjs` + `cdp-ux.mjs` in `/home/jer/Development/CCF-Multi-MapIntegration-Pass1-Evidence-20261124/browser/`, capability-redacted, 36 screenshots + 4 JSON reports captured and key shots visually inspected).

| Cell | Result | Zero horiz. scroll | R3 loaded blocked fold | Inspect Cell swap & `POST api/multi/cell` (`200`/`409`/`400`) | `Escape` keeps draft ticks & overlay | Circular crowns (`w=h=2r`, 0 `scale()`, 0 WebP) | Visible PAR + schematic caveats & 0 Pass 2 writes | Console / JS / CSP / 5xx errors |
|---|---|---|---|---|---|---|---|---|
| `ma-1 @ 1920x1080` | `31/31 PASS` | PASS (`1920 <= 1920`) | PASS (3 & 4 blocked) | PASS (0 rev change) | PASS | PASS (171 A + 73 B Sitka) | PASS | `0 / 0 / 0 / 0` |
| `ma-1 @ 1280x720` | `31/31 PASS` | PASS (`1280 <= 1280`) | PASS (`590 <= 594`, `650 <= 720`) | PASS (0 rev change) | PASS | PASS (171 A + 73 B Sitka) | PASS | `0 / 0 / 0 / 0` |
| `ma-1-mixed @ 1920x1080` | `33/33 PASS` | PASS (`1920 <= 1920`) | PASS (3 & 4 blocked) | PASS (0 rev change) | PASS | PASS (171 A Sitka; B: 25 Sitka, 24 Oak, 24 Beech) | PASS (plus Oak/Beech `/ 2 lifts (3-yr wait)`) | `0 / 0 / 0 / 0` |
| `ma-1-mixed @ 1280x720` | `33/33 PASS` | PASS (`1280 <= 1280`) | PASS (`590 <= 594`, `650 <= 720`) | PASS (0 rev change) | PASS | PASS (171 A Sitka; B: 25 Sitka, 24 Oak, 24 Beech) | PASS (plus Oak/Beech `/ 2 lifts (3-yr wait)`) | `0 / 0 / 0 / 0` |

### 4.7 Host Hygiene and Protected Listeners

- Every ephemeral `sitkap1-ui.dll` process launched during verification was stopped cleanly via `SIGTERM`/`SIGINT` with zero stderr.
- Protected Windows listeners verified still listening on PID `5564` via `netstat -ano`:
  - `127.0.0.1:33767` (`LISTENING`, PID `5564`)
  - `127.0.0.1:43649` (`LISTENING`, PID `5564`)
  - `127.0.0.1:46593` (`LISTENING`, PID `5564`)
- Root checkout `/home/jer/Development/CCF-HP`: `git status --short` empty (untouched).

---

## 5. Explicit Scope Confirmations

- **Zero Unity / `Assets/**` / `Packages/**` / `ProjectSettings/**` edits:** Confirmed via `git diff --name-only cf6fe7a5..HEAD`.
- **Zero save-system or economy/cash/price edits:** Confirmed.
- **Zero Pass 2 write controls or write endpoints:** Confirmed both in `PlayerMultiApi.cs` routing/DOM and by real Edge DOM audit across all 4 matrix cells.
- **Zero `data:image/webp` remaining:** Confirmed by `grep -rn "data:image/webp" Tools/MultiRackSitkaP1Lab/src/`, `ui-smoke-multiarea.sh` `M08`, and live Edge HTML/CSS/JS/DOM audit.
- **Zero ticked checklist rows:** Confirmed that every row in `specs/multi-map-integration-pass1-v1/checklists/requirements.md` remains `[ ]`.
- **No capability URLs committed:** Confirmed zero `/s/<64hex>/` tokens in any tracked file.

---

## 6. Explicitly NOT Verified / NOT Done

- **Independent review (`MI-P1-REVIEW`):** Not performed; checklist boxes in `specs/multi-map-integration-pass1-v1/checklists/requirements.md` are intentionally left unticked for an independent reviewer and Overall Manager.
- **Integration / PR / merge to `main`:** Not performed per binding brief (task branch pushed only; no PR, no merge, no force push, no tag).
- **Unity Editor / Play Mode / Unity C# builds:** Out of scope for `/multi` standalone lab Pass 1 and not run.
- **Interactive Windows desktop browser / screen-reader (NVDA/JAWS) / non-Edge browsers (Firefox/Safari/Chrome):** Verified in real headless Microsoft Edge (CDP) on Windows + WSL2 loopback; interactive human visual review by the owner has not yet occurred.
- **Every-tree exhaustive 244-tree loaded fold sweep (`fold-sweep-owner-ux.mjs`):** Not run across all 244 trees; R3 loaded fold geometry (clean, 3 blocked + post-Add card, 4 blocked + Add form) was verified on representative multi-competitor reference trees across both experiments and both viewports (`1920x1080` and `1280x720`).
- **Live planted saplings (`PJ`), active/expired shelters, `SpotClear` patches, and `ClearUndergrowth` orders rendered via UI write actions:** Pass 1 is read-only for Phase 2 actions (no Pass 2 write endpoints exist yet); their DTO serialization (`MI-P1 C-03`), SVG `<defs>` symbols, legend entries, and read-only rendering paths are implemented and unit-tested in Core, while full end-to-end UI creation/execution of planting/shelters/spot-clear/undergrowth-clear is deferred to Pass 2.

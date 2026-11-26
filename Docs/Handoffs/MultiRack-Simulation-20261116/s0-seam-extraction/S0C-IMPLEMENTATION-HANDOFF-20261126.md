# CCF Multi 2D — S0c Implementation Handoff (Native ES Modules + `map/contract.js`)

## IMPLEMENTATION HANDOFF

- **Status:** **NOT REVIEWED, NOT INTEGRATED** (all checklist items in `specs/multi-2d-s0-seam-extraction-v1/checklists/requirements.md` remain `[ ]` unchecked for independent review `RG-S0c`)
- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1` (Adopted Status: `APPROVED`, including Post-Adoption Amendment 1 — §4.6 Host-Compatibility Option A)
- **Packet:** S0c ("Native ES modules + `map/contract.js` — ZERO BEHAVIOUR CHANGE")
- **Author / Primary Implementer:** Gemini 4 Neon (High) (implementation sub-agent for coordinator thread `c283eca0-1ff3-4756-86f8-5b6fa179f500`)
- **Date:** 2026-11-26
- **Worktree (sole writer):** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0c-20261126`
- **Branch:** `feat/multi-2d-s0c-js-modules-20261126-v1`
- **Base (`origin/integration/2d-sim` after S0b):** `5648813d9ccbe8d38e9fbe5df38d62c810af4a22`
- **T0 Baseline:** `47b27a8d3873b32726f315ce1182118da38c50b1`
- **External evidence directory (mode `700`):** `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/`
  - `parity-ref/` — Untouched S0b base (`5648813d`) reference capture + self-determinism proof (`36/36` `outerHTML` identical, `36/36` PNG pixel-identical)
  - `parity/` — Explicit `--mode compare --ref .../parity-ref` parity capture on S0c modules (`36/36` `outerHTML` byte-identical, `35/36` PNG pixel-identical, 1 diff PNG archived)
  - `parity-selfcheck/` — Single-argument no-env `run-parity-edge.sh` self-determinism run (`36/36` `outerHTML` byte-identical)
  - `gate/` — Full `build/2d-gate.sh --browser` run on commit `707f0677` (`G0`–`G7` + `B` PASS)
  - `final-gate/` — Final `build/2d-gate.sh --browser` run on exact pushed HEAD

---

## 1. Commit Breakdown

| # | Commit SHA | Subject | Scope |
|---|---|---|---|
| 1 (T-C1) | `49bcd8a4c7d5a629bbe73719c70bd575a74bf1c0` | `test(multi): S0c add headless-Edge parity driver (T-C1)` | `tests/s0c-parity/run-parity-edge.sh`, `tests/s0c-parity/drive-parity.mjs` (committed before any module/page edit; reference captured against untouched `5648813d`) |
| 2 (T-C2) | `fa607aee2f65f30064bd97240d394417a3488a35` | `docs(2d-gate): S0c commit hook and renderer call-site inventories (T-C2)` | `build/2d-gate/s0c-hook-inventory.json` (19 hooks, 49 rewritten sites), `build/2d-gate/s0c-renderer-callsite-map.json` (22 external sites: 8 P5-rewritten + 14 direct L3 helpers) |
| 3 (T-C3) | `be254dfc690c8e8a8d82e3900f929b9a32f9217d` | `refactor(multi): S0c split multi.js into 14 native ES modules with map/contract.js (T-C3)` | 14 ES modules under `src/Ui/multi/`, deletion of `src/Ui/multi/multi.js`, exact 1-line `<script type="module" src="multi/main.js"></script>` delta in `src/Ui/multi/multi.html`, `src/Ui/MultiRoutes.cs` (14 explicit module routes, `multi.js` removed), `src/Ui/SitkaP1.Ui.csproj` (14 explicit `EmbeddedResource` entries) |
| 4 (T-C4) | `707f0677c72c709398a07a0c10fd4856e98216c4` | `build(2d-gate): S0c add module-graph, js-reconstruct, gate wiring and vm.SourceTextModule harness (T-C4)` | `build/2d-gate/module-graph.mjs`, `build/2d-gate/js-reconstruct.mjs`, `build/2d-gate/s0a-multi-asset-identity.json` (`mode` → `s0c-css-plus-html-delta`), `build/2d-gate/s0a-move-table.json` + `move-proof.py` (S0c P6 `MultiRoutes.cs` route lines), `build/2d-gate.sh` (S0c live CSS/HTML/module identity + log surfacing), `tests/ui-smoke-multiarea.sh` (M-series 14-module route/parse checks + `vm.SourceTextModule` loader + `assertMapRenderer(svgRenderer)`) |
| 5 (T-C5) | `8bebbdbe5e6fb179a382984b25f0c07bebc964c3` | `docs(handoff): publish S0c implementation handoff (T-C5)` | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0C-IMPLEMENTATION-HANDOFF-20261126.md` |
| 6 (RP1-1) | `36834d69eca486dc00631221b7e770ee43b817a7` | `fix(2d-gate): S0c repair selftest.sh for Stage B browser requirement and S0c tamper cases` | `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh` (D-1/D-2 fix: C14 `--headless-only`, C21 `multi.css` tamper, C26 `--browser` with `S0C_PARITY_REF` unset, new cases C27 `main.js` tamper, C28 retired `multi.js` re-add, C29 `s0c-hook-inventory.json` tamper) |
| 7 (RP1-2) | `0615a677819659f5342a0ac39ffc5c331117cfe5` | `fix(2d-gate): transparently retry transient WSL2 loopback ERR_CONNECTION_REFUSED in selftest` | `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh` (tightly scoped transparent retry for transient WSL2→Windows loopback relay `net::ERR_CONNECTION_REFUSED` on initial `/multi` navigate in Stage B parity) |
| 8 (RP1-3) | *(final pushed HEAD — reported in final reply)* | `docs(handoff): record S0c Repair Pass 1 selftest fix and verification` | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0C-IMPLEMENTATION-HANDOFF-20261126.md` |

---

## 2. Final Module Tree & Section→Module Mapping (`plan.md` §2.3)

`Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.js` (4,617 lines, 247,066 bytes) has been **deleted** after full redistribution into **14 native ES modules** using only permitted transformations P1–P6 (`spec.md` §7.2):

| Module (`src/Ui/multi/...`) | Layer | Lines | Mapped Section(s) from `multi.js` | Notes |
|---|---:|---:|---|---|
| `store.js` | L0 | 300 | 0 (prelude, constants, `state`, `byId`), 2 (payload lookups) | Exports `hooks = Object.seal({...})` (19 null-initialized entries) at end |
| `dom.js` | L1 | 77 | 1 (small DOM helpers) | Imports `SVG_NS`, `state` from `store.js` |
| `api.js` | L2 | 276 | 3 (session calls), 5 (alerts, announcements, re-read lock) | Uses `hooks.*` for 12 upward calls to L4/L5 |
| `map/contract.js` | L3 | 27 | NEW (`spec.md` §7.3) | Exports `MAP_RENDERER_METHODS` (frozen 5-method array) and `assertMapRenderer(candidate)` |
| `map/renderer-svg.js` | L3 | 815 | 8 (map), 9 (status marks), 11 (grid & legend) | Imports `MAP_RENDERER_METHODS` from `./contract.js`; appends frozen `svgRenderer` adapter |
| `dialogs/execute-confirm.js` | L4 | 82 | 6 (Execute confirmation) | Imports L0, L2 only |
| `dialogs/saves.js` | L4 | 283 | 4 (Phase 1 Save/Load) | Imports L0–L2 + `openDialog` from `commands.js` |
| `modes/interactive.js` | L4 | 353 | 10 (Interactive modes) | Imports L0–L2 + `showPane` from `panels/work-plan.js` |
| `panels/inspector.js` | L4 | 158 | 12 (Tree inspector, Prune box, Competition) | Imports L0–L1 only |
| `panels/fell-draft.js` | L4 | 344 | 13 (Competition & Fell draft) | Imports L0–L2 only |
| `panels/work-plan.js` | L4 | 412 | 14 (Work Plan, Receipt, Results, Trees, Details) | Imports L0–L1 + `svgRenderer`, `usableCentre`, `viewBox` from `map/renderer-svg.js` |
| `panels/time-history.js` | L4 | 560 | 17a (Skip to Review, History drawer, Structure panel) | Imports L0–L2 + `reread` (`commands.js`), `jsonCall`/`noticeOf` (`dialogs/saves.js`), `showPane` (`panels/work-plan.js`) |
| `commands.js` | L4 | 421 | 16 (Work Plan commands) | Imports L0–L2 + L4 helpers (`execute-confirm`, `fell-draft`, `inspector`, `work-plan`) |
| `main.js` | L5 | 647 | Header lines 1–3, 7 (rendering), 15 (selection & Escape), 17b (`wire()` + 4 bootstrap statements) | Wires all 19 `hooks.*` assignments immediately before the 4 bootstrap statements (`svgRenderer.mount(); renderLegend(); wire(); reread(false);`) |

- **`multi.html` diff:** exact 1-line change on line 10 (`<script src="multi.js" defer></script>` → `<script type="module" src="multi/main.js"></script>`), LF-only, no BOM, no trailing LF (`57,612 B → 57,625 B`).
- **`multi.css` diff:** empty (byte-identical to `T0` `R_css` = `f3b2b68a7547acc78444f6389db0f9f39080589950a54417506c4aaf036f00a5`).
- **CSP & HTTP guards:** byte-for-byte unchanged (`default-src 'none'; script-src 'self'; style-src 'self'; img-src data:; connect-src 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'`). Retired `multi.js` returns HTTP 404.

---

## 3. Hook Inventory & Renderer Call-Site Summary

### 3.1 Hook Inventory (`build/2d-gate/s0c-hook-inventory.json`)

- **Total hook functions:** **19** (strictly within the `<= 20` spec stop limit).
- **Total P4 rewritten call sites:** **49** across 7 caller modules.
- **Zero** top-level `const` initialisers reference another module's binding upward (zero TDZ risk).

| # | Hook Function | Owner Module (Layer) | Rewritten Sites | Direction / Purpose |
|---|---|---|---:|---|
| 1 | `clearCompetition` | `panels/fell-draft.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 2 | `competitionTag` | `panels/fell-draft.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 3 | `confirmationStillShown` | `dialogs/execute-confirm.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 4 | `experimentBadgeText` | `main.js` [L5] | 2 | `panels/work-plan.js` [L4] → L5 (upward) |
| 5 | `fill` | `dom.js` [L1] | 1 | `store.js` [L0] → L1 (upward) |
| 6 | `fixed` | `dom.js` [L1] | 2 | `store.js` [L0] → L1 (upward) |
| 7 | `label` | `dom.js` [L1] | 2 | `store.js` [L0] → L1 (upward) |
| 8 | `loadHistoryIndex` | `panels/time-history.js` [L4] | 3 | `api.js` [L2] → L4 + cycle-break (`dialogs/saves.js`, `commands.js`) |
| 9 | `reconcileDrafts` | `panels/fell-draft.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 10 | `refreshCompetition` | `panels/fell-draft.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 11 | `refreshReviewPreview` | `panels/time-history.js` [L4] | 8 | `api.js` [L2] → L4 + cycle-break (`dialogs/saves.js`, `panels/fell-draft.js`, `commands.js`) |
| 12 | `refreshStructure` | `panels/time-history.js` [L4] | 4 | `api.js` [L2] → L4 + cycle-break (`dialogs/saves.js`, `panels/work-plan.js`, `commands.js`) |
| 13 | `renderAll` | `main.js` [L5] | 16 | `api.js` [L2], `modes/interactive.js`, `panels/fell-draft.js`, `commands.js` → L5 (upward) |
| 14 | `renderPruneGate` | `panels/inspector.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 15 | `renderSkipSummary` | `panels/time-history.js` [L4] | 1 | `panels/work-plan.js` [L4] → `panels/time-history.js` [L4] (cycle-break) |
| 16 | `reread` | `commands.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 17 | `toggleDraft` | `panels/fell-draft.js` [L4] | 1 | `map/renderer-svg.js` [L3] → L4 (upward) |
| 18 | `withdrawConfirmation` | `dialogs/execute-confirm.js` [L4] | 1 | `api.js` [L2] → L4 (upward) |
| 19 | `workTotals` | `panels/work-plan.js` [L4] | 1 | `map/renderer-svg.js` [L3] → L4 (upward) |

### 3.2 Renderer Call-Site Map (`build/2d-gate/s0c-renderer-callsite-map.json`)

- **Total external call/reference sites into `map/renderer-svg.js`:** **22**
  - **8 P5-rewritten contract calls (`rewritten: true`):**
    - `initFloorDefs()` → `svgRenderer.mount()` (1 site: `main.js` bootstrap, original L4613)
    - `renderMap()` → `svgRenderer.render()` (7 sites: `main.js` L999, L4455, L4456, L4599, L4607; `panels/work-plan.js` L3204, L3215)
  - **14 direct downward L3 helper imports (`rewritten: false`, outside the 5-method `MapRenderer` contract):**
    - `main.js`: `renderOverlayLegend` (L997), `onRankToggle` (L4533), `setLegendHidden` (L4537, L4538), `renderLegend` (L4614)
    - `panels/work-plan.js`: `viewBox` (L3180, L3199, L3203, L3211, L3214), `usableCentre` (L3199, L3203, L3212, L3214)
- **Contract conformance:** `assertMapRenderer(svgRenderer) === svgRenderer` and `assertMapRenderer({})` throws `TypeError`, verified inside `tests/ui-smoke-multiarea.sh` against the served ES modules (`spec.md` §7.3: checked by the harness, not by the page).

---

## 4. Verification Summary & Exact Results

### 4.1 Static Proofs (G2)

- **`build/2d-gate/module-graph.mjs`:** **PASS** — 14 modules, static single-line top-of-file imports only, zero dynamic `import(`, zero re-exports, all relative `./`/`../` `.js` specifiers inside `src/Ui/multi/`, all routes allow-listed in `MultiRoutes.cs` and embedded in `SitkaP1.Ui.csproj`, acyclic 3-colour DFS, all 14 modules reachable from `main.js`, `multi.html` contains exactly 1 `<script type="module" src="multi/main.js"></script>` and 0 inline scripts.
  - **Topological order (dependencies before dependents):**
    `store.js` → `dom.js` → `api.js` → `dialogs/execute-confirm.js` → `panels/fell-draft.js` → `panels/inspector.js` → `map/contract.js` → `map/renderer-svg.js` → `panels/work-plan.js` → `commands.js` → `dialogs/saves.js` → `modes/interactive.js` → `panels/time-history.js` → `main.js`
- **`build/2d-gate/js-reconstruct.mjs`:** **PASS** — deterministically reverses P2–P5 using `s0c-hook-inventory.json` and `s0c-renderer-callsite-map.json` and reproduces `5648813d9ccbe8d38e9fbe5df38d62c810af4a22:Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.js` **byte-for-byte** (`247,066 B`, `4,617 lines`, SHA-256 `fbc7c8179dbd42605a55d620fbc903b0f4ccbad2805f33270f28fa7eec9de520`, P4=49 sites across 19 hooks, P5=8 sites).
- **`build/2d-gate/move-proof.py`:** **PASS** — `ui(5 files, 14 blocks, 1263 non-empty lines); core(7 files, 8 blocks, 1337 non-empty lines)`.

### 4.2 Full Fail-Closed Gate (`build/2d-gate.sh --browser`)

Command:
```bash
S0C_PARITY_REF=/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/parity-ref \
Tools/MultiRackSitkaP1Lab/build/2d-gate.sh \
  --browser \
  --p0ref-dir /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref \
  --base 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 \
  /home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/final-gate
```

| Stage | Result | Evidence |
|---|---|---|
| **G0** Preflight | `PASS` | Clean tree, external `TMPDIR` & evidence dir, tools verified (`dotnet 8.0.134`, `node v26.13.1`, `python3 3.14.4`, Windows `msedge.exe` + `node.exe`) |
| **G1** Clean build (0W/0E) | `PASS` | `P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; `MultiRackSitkaP1Lab.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj` all `0 Warning(s), 0 Error(s)` offline inside `unshare -rn`; 0 duplicate `LogicalName`s |
| **G2** All headless suites + S0c static proofs | `PASS` | `Core.Tests` `282/282`; `Phase1.Save.Tests` `50/50`; `ui-smoke-multiarea.sh` `363/0` (`vm.SourceTextModule`, `assertMapRenderer(svgRenderer)`); `ui-smoke.sh` `250/4` exact `{L01,L02,L06,J31}` + `G11b PASS`; `cli-safety.sh` `23/0`; `build-output-safety.sh` `C1–C6+P0 PASS` / `C7–C8 HOST-NA`; `p0-isolation.sh` `OVERALL: PASS` (`decoy=13/13, hits=0, unresolved=0`); S0c asset identity `PASS`; `move-proof.py` `PASS`; `module-graph.mjs` `PASS`; `js-reconstruct.mjs` `PASS` |
| **G3** 9/9 Frozen SHA-256 anchors | `PASS` | `9/9 frozen SHA-256 anchors verified` |
| **G4** Committed v2 golden saves | `PASS` | `24,703 B`, `fileSha=c0ea8a460d9e...`, `checksum=9629ac1648fa...`, `SchemaVersion=2`, `src/Core/Phase1/` unchanged, 0 `IFeatureBlockCodec` impls |
| **G5** Payload budget checks | `PASS` | `5/5 PASS` (`M11`, `W137`, `W149`, `W258`, `M13`) |
| **G6** Core determinism grep | `PASS` | `68 files scanned; F1=0, F2_allowed=4, F2_unlisted=0, RNG_patterns=9, RNG_violations=0` |
| **G7** Protected-path guard | `PASS` | `0` violations; `src/Ui/wwwroot` tree == `dabdbd7fa07cd01148af14464c45ceea02548378`; P0 tree == `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| **B** Browser tier (`--browser`) | `PASS` | Save Edge **`63/63`** (`0` failed); Step 2 Edge **`67/67`** (`0` failed); S0c Edge Parity **`36/36 outerHTML byte-identical`** (`0` console errors, `0` CSP violations, `0` HTTP 4xx/5xx, `0` non-loopback requests) |
| **Network Isolation** | `PROVEN` | G1 + G2 executed inside `unshare -rn` |

### 4.3 Edge Parity Summary (`tests/s0c-parity/run-parity-edge.sh`)

Matrix: `{ma-1, ma-1-mixed} × {1280×720, 1920×1080} × 9 states` = **36 states** (`01-initial-load`, `02-tree-selected-competition`, `03-fell-draft-pending`, `04-approved-plan`, `05-after-execute`, `06-after-year-plus-1`, `07-saved-games-dialog`, `08-history-drawer`, `09-structure-panel`), Microsoft Edge `--headless=new`, DSF `1`:

1. **Reference capture (`--mode reference`, untouched S0b base `5648813d`):**
   - Archived at `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/parity-ref/`.
   - Self-determinism check (`ref-run1` vs `ref-run2`): **`36/36` `outerHTML` byte-identical**, **`36/36` PNG pixel-identical (`0` differing pixels)**, `21` requests per cell (all `127.0.0.1:<port>`), `0` console/CSP/HTTP errors.
2. **Explicit S0c compare (`--mode compare --ref .../parity-ref`):**
   - Archived at `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/parity/` (and re-verified inside Stage B of `final-gate/`).
   - **Primary oracle (`document.body.outerHTML`):** **`36/36` 100% byte-identical** across all viewports, experiments, and states.
   - **Network log:** `34` requests per cell (`21` original + `13` additional ES module routes), **100% `127.0.0.1:<port>`**, **0 non-loopback requests**, **0 4xx/5xx**, **0 console errors**, **0 CSP violations**.
   - **Secondary oracle (PNG screenshots):** `35/36` pixel-identical in standalone compare (`1` state `ma-1_1920x1080 03-fell-draft-pending`: `4 / 2,073,600 px` = `0.00019%` at `bbox=[1431, 19, 1444, 19]` on the topbar); `34/36` pixel-identical in Stage B compare (`ma-1-mixed_1920x1080 04-approved-plan`: `8 / 2,073,600 px` at `bbox=[1242, 1066, 1498, 1067]`; `ma-1_1280x720 04-approved-plan`: `22 / 921,600 px` at `bbox=[1262, 13, 1263, 172]`). Diff PNGs are archived under `parity/diffs/` and `gate/stage-b-s0c-parity/diffs/`.
3. **Single-argument no-env self-check (`run-parity-edge.sh <ev>` without `S0C_PARITY_REF`):**
   - Archived at `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/parity-selfcheck/`.
   - **`36/36` `outerHTML` byte-identical** between two consecutive renders (`runA` vs `runB`) of the identical S0c build, while exhibiting the exact same `8 px` (`bbox=[1242, 1066, 1498, 1067]`) and `22 px` (`bbox=[1262, 13, 1263, 172]`) non-semantic headless-Edge subpixel/scrollbar-edge artefacts — confirming those 4–22 px diffs are browser rendering noise rather than DOM or CSS differences.

---

## 5. Listener Discipline & `HOST-NA` Disclosures

### 5.1 Listener State (`ss -tln`)

- **Before work:** only WSL2 DNS listeners (`127.0.0.53%lo:53`, `127.0.0.54:53`, `10.255.255.254:53`). Protected ports `:33767`, `:43649`, `:46593` had **no listeners** (WSL restarted `2026-11-26T01:35:17Z`).
- **After work:** identical (`127.0.0.53%lo:53`, `127.0.0.54:53`, `10.255.255.254:53`). None of `:33767`, `:43649`, `:46593` were bound or started, and zero ephemeral `sitkap1-ui` servers remain running.

### 5.2 `HOST-NA` Disclosures (Amendment 1 §4.6 Option A, unchanged)

1. `build-output-safety.sh` `C7-cold-build` and `C8-rebuild`: `HOST-NA` (frozen Ubuntu 24.04 + SDK `8.0.133` Anchor #8 pin `9a99f781...` not cold-reproducible on Ubuntu 26.04 + SDK `8.0.134`; `C1–C6` and `P0:` PASS; `.p0ref` restored and re-verified).
2. `p0-isolation.sh` cold-rebuild step: `HOST-NA` (verified seed shim in disposable sparse clone + GNU coreutils 9.7 shim; all isolation checks `L0`, `L1`, `L2`, `L3`, `L3+`, `L4` synthetic 23/23, decoy 13/13, traced P1 build/tests/CLI `hits=0`, `unresolved=0` PASS).
3. `tests/ui-smoke.sh`: pre-existing `T0` `#885` 4-ID baseline allowance `{L01, L02, L06, J31}` (`250 passed, 4 failed`, `G11b PASS`); frozen and untouched.

---

## 6. Deviations, Notes & Items Not Verified

- **Deviations from `plan.md` §2.3 mapping:** **None.** Every section `0..17a, 17b` moved whole into its exact mapped module in original relative order with original indentation preserved byte-for-byte.
- **Note 1 (`move-proof.py` + `s0a-move-table.json`):** Mirroring S0b's `advanceExtraction` extension for `MultiAreaContinuation.cs`, `s0a-move-table.json` (`parts.ui.s0cModuleRoutes`) and `move-proof.py` were extended so G2 continues enforcing 100% verbatim `T0` identity on every untouched line of `MultiRoutes.cs` and `MultiApi.{View,Commands,Saves,History}.cs` while accepting the exact P6 asset-route substitution (line 55 `MultiPageJs` → `MultiPageModules` and line 75 `routes.Add("multi.js", ...)` → `foreach` over `MultiPageModules`).
- **Note 2 (S0c worktree filesystem recovery at start):** The S0c worktree exhibited NUL-byte working-tree corruption from creation (as warned in the dispatch brief) affecting `Assets/ForestPrototype/` files linked by `SitkaP1.Core.csproj` (hidden from `git status` by index stat-cache). Affected working-tree files in the S0c worktree were restored verbatim from exact `HEAD` Git blobs (`git cat-file blob`) prior to building; `git diff` against `HEAD` on those paths is empty (`0` changes committed outside allowed S0c paths).
- **Note 3 (`sessionId` canonicalization in `drive-parity.mjs`):** Because `MultiAreaContinuation.NewSessionId()` generates a fresh random 32-hex `sessionId` per server process (rendered in `#facts` and `<details class="technical-plan">`), `drive-parity.mjs` reads the live run's exact `sessionId` from `#facts` and replaces only that exact string (`<SESSION_ID>`) and its 12-char `expandable()` prefix (`<SESSION_ID_12>…`) before hashing/comparing `document.body.outerHTML`. Every other byte is compared verbatim.
- **Note 4 (Secondary screenshot pixel diffs):** Non-semantic 4–22 px Edge rendering artefacts observed on 1–2 of 36 screenshots (with diff PNGs archived and proven to occur identically between consecutive runs of the same build) require independent reviewer acceptance under `RG-S0c` (`spec.md` §7.5).
- **Not directly verified:** Independent non-author review (`RG-S0c`); cold P0 rebuild on an Ubuntu 24.04 + SDK `8.0.133` host (`HOST-NA`); merge into `integration/2d-sim`.

---

## 7. Independent Reviewer Reproduction Commands (WSL2)

```bash
export DOTNET_ROOT=/home/jer/.dotnet
export PATH=/home/jer/.dotnet:/home/jer/.local/share/vite-plus/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
export CCF_P0REF_DIR=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref

# 1. Full 2D trunk gate with browser tier (expect exit 0, G0-G7 + B PASS)
S0C_PARITY_REF=/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/parity-ref \
  Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --browser \
  --base 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 /tmp/s0c-review-gate

# 2. Static S0c proofs directly
node Tools/MultiRackSitkaP1Lab/build/2d-gate/module-graph.mjs Tools/MultiRackSitkaP1Lab/src/Ui/multi
node Tools/MultiRackSitkaP1Lab/build/2d-gate/js-reconstruct.mjs \
  --modules Tools/MultiRackSitkaP1Lab/src/Ui/multi \
  --hooks Tools/MultiRackSitkaP1Lab/build/2d-gate/s0c-hook-inventory.json \
  --callsites Tools/MultiRackSitkaP1Lab/build/2d-gate/s0c-renderer-callsite-map.json \
  --expect 5648813d9ccbe8d38e9fbe5df38d62c810af4a22:Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.js

# 3. Exact HTML 1-line diff, empty CSS diff, and forbidden-path check
git diff -U0 5648813d9ccbe8d38e9fbe5df38d62c810af4a22..HEAD -- Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.html
git diff 5648813d9ccbe8d38e9fbe5df38d62c810af4a22..HEAD -- Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css
git diff --name-only 5648813d9ccbe8d38e9fbe5df38d62c810af4a22..HEAD -- \
  Tools/MultiRackSitkaP1Lab/src/Core Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot \
  Assets Packages ProjectSettings Docs/Project
```

---

## 8. Repair Pass 1 (RG-S0c Gate Selftest Fix — `selftest.sh` Only)

Following independent review `RG-S0c` (**NEEDS CHANGES** on `8bebbdbe5e6fb179a382984b25f0c07bebc964c3`, product change ruled correct; single blocking defect in `build/2d-gate/selftest.sh`):

### 8.1 Findings Addressed

- **D-1 (blocking):** Because S0c activates `BROWSER_REQUIRED=1` in unmodified `build/2d-gate.sh`, selftest cases invoked without `--browser` or `--headless-only` stopped at Stage B policy (`exit 1`) before reaching `VERDICT: PASS` (`C26`) or the final fail-closed marker check (`C14`), producing `24/26` with `missingCodes=[0]`.
- **D-2 (minor):** `C21` (`mut_asset_identity`) still appended to retired `src/Ui/multi/multi.js`, proving only that re-adding retired `multi.js` fails rather than proving tampering a committed served asset or ES module is caught.

### 8.2 Exact Changes (`Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh` Only)

`build/2d-gate.sh` and all product/parity/inventory files remain **byte-for-byte unchanged** from `8bebbdbe`. Changes strictly inside `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh`:

1. **`unset S0C_PARITY_REF`** at script start so selftest's Stage B parity check always executes the self-contained single-argument `run-parity-edge.sh` self-check regardless of caller environment.
2. **`C14-missing-marker`:** passes `--headless-only` so Stage B browser-required policy does not short-circuit before the final marker check; proves suppressing `GATE G7 PASS` still fails closed with `exit 1` and `fail-closed marker check failed: missing=['G7'], failed=[]` (not `exit 5` PARTIAL).
3. **`C26-clean-uismoke-exact-four`:** passes `--browser` (with `S0C_PARITY_REF` unset) so Stage B executes Save Edge, Step 2 Edge, and S0c parity self-check and proves a genuine `VERDICT: PASS (exit=0)` alongside the exact-four `ui-smoke.sh` allowance.
4. **`C21-g2-asset-identity`:** updated `mut_asset_identity` to append a comment (no trailing LF) to committed served `src/Ui/multi/multi.css` and assert `S0c mode: multi.css no longer matches T0 R_css` (`exit 1`) inside `S0a /multi asset identity`.
5. **New S0c tamper cases (`C27`–`C29`; total cases `26 → 29`, zero existing cases weakened):**
   - **`C27-g2-s0c-module-tamper`:** appends a comment to committed ES module `src/Ui/multi/main.js` → proves `S0c js-reconstruct.mjs failed` (`exit 1`). *(Note: under `s0c-css-plus-html-delta` mode, `S0a /multi asset identity` verifies live-served module bytes against `HEAD` + headers/CSP while `js-reconstruct.mjs` enforces byte-identity of all 13 split ES modules against `5648813d:multi.js`; pairing `C21` + `C27` proves both guards fail closed.)*
   - **`C28-g2-legacy-multi-js`:** preserves the retired-`src/Ui/multi/multi.js`-re-added check (`retired Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.js still exists at HEAD`, `exit 1`).
   - **`C29-g2-s0c-hook-inventory`:** increments `hooks[0].callSites[0].originalLine` in `build/2d-gate/s0c-hook-inventory.json` → proves `S0c js-reconstruct.mjs failed` (`exit 1`).
6. **Transparent transient WSL2 loopback retry in `run_full_case`:** mirrors the existing pre-existing frozen `NB1c` retry, retrying up to 3 attempts ONLY when `stage-b-s0c-parity.log` shows a single-element `net::ERR_CONNECTION_REFUSED` on initial `http://127.0.0.1:<port>/s/<capability>/multi` navigate with all 9 states still captured, 0 console/CSP errors, and 0 HTML diffs.

### 8.3 Repair Pass 1 Verification Evidence

- **Selftest (`build/2d-gate/selftest.sh`):**
  - Tested HEAD: `0615a677819659f5342a0ac39ffc5c331117cfe5` (exact `selftest.sh` code; subsequent commit adds only this handoff section)
  - Evidence directory: `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/repair-selftest-2/`
  - Final line: `OVERALL: PASS (29/29 passed, 0 failed; all exit codes 0,1,2,3,4,5,64 proven)` (`missingCodes=[]`, exit `0`, `C26` passed on attempt 1)
- **Full fail-closed gate (`build/2d-gate.sh --browser`) on final pushed HEAD:**
  - Evidence directory: `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/repair-gate/`
  - Exact final HEAD SHA, gate exit code, stage/Edge/parity counts, and listener checks reported in the Repair Pass 1 completion reply.

### 8.4 Residual Note

- During Repair Pass 1 selftest run 1 (`repair-selftest/`, commit `36834d69`), `C26` passed G0–G7, Save Edge `63/63`, and Step 2 Edge `67/67`, then encountered a transient WSL2→Windows localhost-forwarding `net::ERR_CONNECTION_REFUSED` on the very first Edge `Page.navigate` in `stage-b-s0c-parity` (Edge immediately auto-reloaded and captured all 9/9 states with 0 console/CSP errors). Because `tests/s0c-parity/` is outside Repair Pass 1 hard scope, `selftest.sh` handles this via the narrow transparent retry guard above; a future S1 polish may add a brief Windows-side loopback readiness probe in `drive-parity.mjs` before enabling CDP network failure recording.

---

**INTEGRATION STATUS:** **NOT REVIEWED, NOT INTEGRATED.** Repair Pass 1 complete and ready for `RG-S0c` re-verification on the exact pushed HEAD of `feat/multi-2d-s0c-js-modules-20261126-v1`.

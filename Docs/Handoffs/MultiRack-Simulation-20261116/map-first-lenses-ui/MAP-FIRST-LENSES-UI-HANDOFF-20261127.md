# CCF Multi 2D — Map-First Menu & Lenses Upgrade — Implementation Handoff

- **Date:** 2026-11-27
- **Branch:** `feat/multi-2d-map-first-lenses-ui-20261127-v1` (NOT merged to `integration/2d-sim`; protected `origin/main` untouched)
- **Base (`origin/integration/2d-sim`):** `674c7c9e470d0aff0a028bd284fd7510f973e2d5`
- **Commit 1 (Binding 4-File Spec Quartet):** `e1556e3bac969c38f789e3a4688b4ad6669da6e2`
- **Commit 2 (Implementation + Targeted Edge Suite):** `e17ca43a937feaa1c0c006405259f30ef855d299`
- **Role:** Sole Implementer (single writer; zero subagents spawned)
- **Status:** Ready for independent read-only review

---

## 1. Summary of Changes & Locked Owner Decisions Implemented

Items 1–8 of the locked owner decisions (Option B on items 1–4, Safe Hybrid on item 5, plus structural guardrails and Part 2 readiness; see Section 6 for Item 9 deferral) are implemented strictly within the existing 16 plain ES modules under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/` (0 new JS modules, zero Core/Assets/`csproj`/`MultiRoutes.cs`/roadmap edits):

1. **Binding 4-File Spec Quartet (Commit 1):**
   - `specs/multi-2d-map-first-lenses-ui-v1/spec.md`
   - `specs/multi-2d-map-first-lenses-ui-v1/plan.md`
   - `specs/multi-2d-map-first-lenses-ui-v1/tasks.md`
   - `specs/multi-2d-map-first-lenses-ui-v1/checklists/requirements.md`
2. **Five Task-Focused Lenses Strictly Separate from Active Tools (Option B — `MLUI-01`):**
   - Lenses: `Woodland` (default overview), `Trees`, `Regeneration`, `Harvesting`, `Habitat` (`store.js` `LENSES`, `main.js` `setLens()`).
   - Switching lenses changes presentation/overlay defaults and available tool buttons only. It **never** calls a mutating API, **never** approves/executes work, **always** resets the active tool to safe `Select Tree` (switching to `Regeneration` never auto-activates `Plant`), and **preserves** tree/cell/juvenile selection, pending Fell drafts, crop marks, active lane, and SVG pan/zoom `viewBox`.
   - Data-driven Part 2 extension slots (`data-extension-slot`) are rendered cleanly inside `Trees` (3 slots), `Regeneration` (3 slots), `Harvesting` (2 slots), `Habitat` (3 slots), Woodland Navigator (2 slots), and Alerts (`Significant Event` / `Information` tiers) for `Scots Pine & Soil Moisture`, `Real-World Woodland Inspection`, `Jay Acorn Dispersal & 2D Windstorms`, and `Downy Birch & Common Alder` — with zero premature Part 2 simulation wiring.
3. **Default Adjacent Lanes + 1-Lane Precision Toggle + Mini-Map Range Sync (Option B — `MLUI-03`):**
   - Extends the existing multi-lane view and compact mini-map (`store.js` `visibleLaneIds()`, `api.js` `refreshMultiStandFrame()`, `map/minimap.js` `renderMinimap()`).
   - Default adjacent lanes: `2–3` at `1920×1080` (`MA-B,MA-C,MA-D` at interior Lane C) and `2` at `1280×720` (`MA-C,MA-D` at interior Lane C).
   - `1 lane` precision toggle button (`#lane-mode-toggle`) and `M` hotkey isolate the focused lane (`MA-C`) and toggle back cleanly; the mini-map draws a synchronized `.minimap-range-frame` highlighting the active visible lane range.
4. **Compact Contextual Legend + Floating Expand Toggle (Option B — `MLUI-02`):**
   - Replaces the permanent 3rd legend column with a strictly 2-column desktop workspace (`minmax(0, 1fr) var(--side-w)` — never 3 or 4 columns), a compact contextual legend strip (`#legend-bar`) inside `.map-panel`, and a floating full-legend popover (`#legend-panel`, 16 symbols) toggled without shrinking map width.
   - Reclaimed map width measured directly in Edge:
     - **`1920×1080`:** `1226px → 1486px` (**`+21.2%`**)
     - **`1280×720`:** `738px → 918px` (**`+24.4%`**)
5. **Priority-Ranked Grouped Non-Blocking Alerts (Option B — `MLUI-05`):**
   - Implemented in `store.js` (`ALERT_TIERS`), `api.js` (`renderAlertStrip()`), `multi.html`, and `multi.css` inside `#alert`.
   - Strict priority ranking: `Blocking` → `Attention Required` → `Significant Event` → `Information` → `Historical` (direct link to existing single `#history-drawer`), with grouped collapse/expand, redundant icon + badge + colour encoding, direct `Open Work Plan` navigation for unfinished work, and zero modal alert chains.
6. **Single-Level Accessible Pinned Explanations (Safe Hybrid — `MLUI-06`):**
   - One shared accessible popover component (`#explain-popover`) in `dom.js` (`initExplainPopover`, `openExplain`, `closeExplain`) backed by `EXPLANATIONS` in `store.js`.
   - Hover and keyboard focus reveal, click/Enter/Space pin, WCAG 2.2 SC 1.4.13 compliant (dismissible via `Esc`/close button, hoverable onto the popover surface without dismissal, persistent while hovered/focused/pinned), zero 2-level nested links, and essential restrictions/refusals remain visible inline in panels/Work Plan.
7. **Collapsible Woodland Navigator + Safe Bounded Hotkeys + Scroll Isolation (`MLUI-04`, `MLUI-07`, `MLUI-08`, `MLUI-09`):**
   - Collapsible `Navigator` tab inside `#side` (`panels/work-plan.js` `renderNavigator()`) listing Lanes (whole woodland), Marked Crop Trees (visible lanes with explicit scope pill), Unfinished Work (whole woodland), plus Part 2 slots for Storm-felled stems and Inspection blockers, with `Prev`/`Next` cycling buttons.
   - Safe bounded hotkeys (`1–5`, `[` / `]`, `H`, `W`, `M`, `?`, `Esc`) strictly guarded in text-entry/input/select/textarea/contenteditable contexts, read-only `#shortcuts-dialog`, and zero unguarded shortcuts for Approve, Execute, Advance, New Woodland, Load Save, or Money Reset.
   - Scroll-ownership containment guard (`overscroll-behavior: contain` + wheel `stopPropagation` in `main.js`): prevents wheel events over `#side`, `#legend-panel`, or `#history-drawer` from chaining or propagating to `#map` (note: `/multi` currently has no map wheel-zoom/pan handler; this is a proactive containment guard, not a fix for a pre-existing map wheel bug).
   - Zero page-wide horizontal scroll at both viewports; reference tree + competitor ranks `1–5` + `Add` remain strictly above the fold (`addBottom = 933px <= 1080px` at `1920×1080`; `addBottom = 624px <= 720px` at `1280×720`).

---

## 2. Exact 16-Module Architecture (0 New JS Modules)

`module-graph.mjs` verifies all 16 existing plain ES modules remain acyclic, layer-ordered (`L0→L5`), LF-only, UTF-8 without BOM, and loaded via a single `<script type="module" src="multi/main.js"></script>`:

| Module | Layer | Role in This Change |
|---|---|---|
| `store.js` | L0 | `LENSES`, `ALERT_TIERS`, `EXPLANATIONS`, `SHORTCUTS`, `PART2_SLOTS`, responsive `visibleLaneIds()`, alert state, lens/navigator state |
| `dom.js` | L1 | Shared WCAG 2.2 SC 1.4.13 `#explain-popover` controller (hover/focus/pin/Esc), `explainTrigger()` helper |
| `api.js` | L2 | Priority-ranked grouped `#alert` strip renderer, multi-lane range-aware `refreshMultiStandFrame()` |
| `dialogs/execute-confirm.js` | L3 | Unchanged (Playable Milestone 1 confirmation gate preserved) |
| `panels/fell-draft.js` | L3 | Unchanged (draft queue preserved across lens switches) |
| `panels/inspector.js` | L3 | Lens-aware contextual hints & Part 2 slot cards, inline explanation triggers, preserved inline refusal text |
| `map/contract.js` | L3 | **Untouched** (byte-identical to base) |
| `map/renderer-svg.js` | L3 | Compact contextual legend bar + floating 16-symbol `#legend-panel`, lens presentation attributes, preserved pan/zoom `viewBox` |
| `panels/work-plan.js` | L4 | Collapsible Woodland Navigator rendering, explicit visible-vs-whole-woodland count scopes, Prev/Next cycling, Part 2 slots |
| `commands.js` | L4 | Unchanged (commitment gate & mutation routes preserved) |
| `dialogs/saves.js` | L4 | Preserves Save v2/v3 flow; clears transient manual alerts on session load |
| `dialogs/woodland.js` | L4 | Unchanged |
| `map/minimap.js` | L4 | Synchronized `.minimap-range-frame` highlighting active visible lanes + mode badge (`3 of 5 shown` / `2 of 5 shown` / `1 lane precision`) |
| `modes/interactive.js` | L4 | Tool visibility per active lens; always falls back safely to `Select Tree` on lens switch |
| `panels/time-history.js` | L4 | **Untouched** (byte-identical to base; single History drawer preserved) |
| `main.js` | L5 | `setLens()`, lane mode toggle, safe bounded hotkeys + input guards, strict `Esc` stack, `#shortcuts-dialog`, wheel isolation |

---

## 3. Verification Results

All checks below were executed on the final implementation commit (`e17ca43a937feaa1c0c006405259f30ef855d299`):

### A. Module Graph Gate
- **Command:** `/home/jer/.local/share/vite-plus/bin/node Tools/MultiRackSitkaP1Lab/build/2d-gate/module-graph.mjs`
- **Result:** **PASS** — `16 modules, acyclic, all reachable from main.js, 1 module script tag`.

### B. `.NET 8` UI Project Build
- **Command:** `/home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror -p:P0CorePath=/home/jer/Development/CCF-Multi-2D-Phase2A2-Evidence-20261127/p0ref-seed/SitkaLab.Core.dll`
- **Note:** Read-only verified P0 Core seed passed via `-p:P0CorePath=...` so no `.p0ref` file was created inside the worktree.
- **Result:** **Build succeeded — `0 Warning(s), 0 Error(s)`**.

### C. Targeted Headless-Edge CDP Qualification Suite
- **Command:** `bash Tools/MultiRackSitkaP1Lab/tests/map-first-lenses-ui/run-map-first-lenses-edge.sh`
- **External Evidence Dir (outside repo):** `/mnt/c/Users/hp/AppData/Local/Temp/ccf-map-first-lenses-edge-20261127T211116Z`
- **Overall Result:** **OVERALL: PASS (`56 passed, 0 failed`)**
  - **`1920×1080`:** `24 passed, 0 failed` (`vp1920/main`: `22/22`, `vp1920/load`: `2/2`; map width `1226px → 1486px` `+21.2%`; `addBottom=933px <= 1080px`; adjacent lanes `MA-B,MA-C,MA-D`; 3-way felling `retained=0.126 m³`, `DW0001`, `cash=€9,505.23` intact after cross-process Save v3 reload; `console=0, csp=0, net=0`).
  - **`1280×720`:** `24 passed, 0 failed` (`vp1280/main`: `22/22`, `vp1280/load`: `2/2`; map width `738px → 918px` `+24.4%`; `addBottom=624px <= 720px`; adjacent lanes `MA-C,MA-D`; 3-way felling `retained=0.126 m³`, `DW0001`, `cash=€9,505.23` intact after cross-process Save v3 reload; `console=0, csp=0, net=0`).
  - **Server Lifecycle Checks:** `8 passed, 0 failed` (`Z01`/`Z02` clean SIGINT exit and empty stderr across all 4 ephemeral server runs).

### D. Explicitly Skipped (Per Task Scope)
- `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh` — **SKIPPED** (explicitly excluded by task scope).
- Full `2d-gate.sh --browser` — **SKIPPED** (explicitly excluded by task scope).
- `Core.Tests` and `Phase1.Save.Tests` — **SKIPPED** (`src/Core/**` untouched; excluded by task scope).

---

## 4. Non-Blocking Trade-Offs & Unverified Items

- **Marked Crop Trees vs Unfinished Work count scope in Woodland Navigator (N-2 verified):** Verified directly in `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Stand.cs:281-346, 444, 512` that lane-filtered reads (`GET api/multi/stand?lanes=...`) filter only `Areas`, `Planted`, `Shelters`, and `Patches`, while keeping `view.Pending`, `view.Approved`, `PendingPrunes`/`ApprovedPrunes`, `PendingClear`/`ApprovedClear`, and `PendingPlanting`/`ApprovedPlanting` **whole-woodland** across all lanes. Consequently, `Lanes` and `Unfinished Work` truthfully display `Whole woodland` (`data-scope-kind="woodland"`), while `Marked Crop Trees` (whose per-stem `cropTreeIds` arrive per loaded area) truthfully displays `Visible lanes (N of M)` (`data-scope-kind="visible"`).
- **Wheel containment guard scope (N-1):** `/multi` has no SVG map wheel-zoom/pan handler at base or HEAD; the `overscroll-behavior: contain` CSS and panel `wheel` `stopPropagation` act as a proactive containment guard rather than a fix for a pre-existing map wheel bug.
- **Affected existing suites (m-2):** Both mandatory gate **G2** (`tests/ui-smoke-multiarea.sh`) and **Stage B** (`tests/phase1-time-history-ui/run-step2-edge.sh`) exercise `/multi` Plant/Spot Clear/alert flows and were updated and executed in Repair Round 1 (see Section 7). Other Edge suites (`phase1-save-ui`, `s0c-parity`, `phase2b2b-ui`) were not run because full `2d-gate.sh --browser` is explicitly excluded by task scope.
- **Screen-reader / live assistive-technology check:** ARIA roles (`role="alert"` on blocking alert rows, `role="region"` on `#alert` and `#explain-popover`), `#announce` live announcements on blocking refusals, `aria-pressed`, `aria-expanded`, keyboard focus retention, and `Esc` layering are verified in headless Edge DOM/CDP, but manual live testing with NVDA/JAWS/VoiceOver was not performed in this environment.
- **Unity / 3D project:** Untouched and not run (2D web UI lane only).

---

## 5. File-Fence & Branch Safety Confirmation

- All modified/added files are strictly within:
  - `specs/multi-2d-map-first-lenses-ui-v1/**`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/multi/**` (0 new JS modules; `map/contract.js` and `panels/time-history.js` untouched)
  - `Tools/MultiRackSitkaP1Lab/tests/map-first-lenses-ui/**`
  - `Docs/Handoffs/MultiRack-Simulation-20261116/map-first-lenses-ui/**`
- Zero edits to `Assets/**`, `Packages/**`, `ProjectSettings/**`, `src/Core/**`, `src/Ui/*.cs`, `SitkaP1.Ui.csproj`, `src/Ui/wwwroot/**`, `build/2d-gate/**`, `Docs/Project/**`, or `Docs/Handoffs/MultiRack-Simulation-20261116/2D_Simulator_Strategic_Roadmap.md`.
- Branch `feat/multi-2d-map-first-lenses-ui-20261127-v1` is **NOT merged** to `integration/2d-sim`; `origin/main` is untouched.

---

## 6. Deferred / not done

- Item 9 (Strategic Roadmap v1.21 single-writer sync) = NOT DONE / DEFERRED by owner decision 2026-11-27, because the verbatim payload supplied to the lane was truncated mid-document; the file is unchanged at v1.18 on this branch.

---

## 7. Repair Round 1 (Independent Review Response)

Addressed every finding from the independent read-only review (`origin/review/multi-2d-map-first-lenses-ui-20261127-v1` @ `a99b6b4b`, `MAP-FIRST-LENSES-UI-REVIEW-20261127.md`) as new commits on top of `78dd6432` (published history not amended):

### Findings Resolved

| Finding | Root Cause & Fix |
|---|---|
| **B-1 (G2 crash `dom.js:203`)** | `initExplainPopover()` unconditionally called `document.addEventListener`, crashing under `ui-smoke-multiarea.sh`'s `FakeDocument`. Fixed in `dom.js` to attach delegated popover listeners to `typeof document.addEventListener === 'function' ? document : document.documentElement` (guarded if unavailable), plus safe `pop.contains` checks. |
| **B-1 (G2 `F2e`, `R3i`, `J22`, `P2b–P2g` & Stage B)** | (1) `F2e`: `api.js` now ranks direct user-action blocking alerts (`state.manualAlert`) first and deduplicates `'readout-unavailable'` (`commands.js` passes `{ tier: 'blocking', id: 'readout-unavailable' }`), so `"Execute needs a re-read"` is primary rather than hidden under `Blocking (2)`. (2) `P2b–P2g` & Stage B: updated `tests/ui-smoke-multiarea.sh` and `tests/phase1-time-history-ui/drive-step2.mjs` with explicit `[data-lens-btn="regeneration"]` lens switches where Plant/Spot Clear moved by locked Decision #2 (zero assertions deleted or weakened; Stage B NB-2 keeps `#alert` open across both map clicks so `#map` `getScreenCTM()` stays bit-identical for exact-float `Phase2DuplicateOrder`). (3) `R3i` & `J22`: updated `ui-smoke-multiarea.sh` to assert zero `blocking` alert (and no `'Outcome unconfirmed'`) while allowing the locked Decision #5 `Attention Required` reminder for queued pending work. |
| **M-1 / M-3 (Alert cloning, dead listeners, focus loss, dead `focusTarget`)** | `renderAlertRow()` in `api.js` now inserts real caller-supplied DOM nodes directly (never `cloneNode`), preserving direct `addEventListener` handlers (such as the untouched `panels/time-history.js` Skip refusal `Open Work Plan` button) and making `showAlert(parts, focusTarget)` focus the live node. `rereadButton()` removed redundant `data-alert-action` to prevent double-firing and gained `data-focus-key="alert-reread"`. All built-in alert controls (`#alerts-expand`, `◷ History`, action buttons, Dismiss) now carry stable `data-focus-key` attributes; `renderAlertStrip()` skips no-op DOM rebuilds via an alert state signature and restores focus cleanly across re-renders, Enter expand/collapse, and Dismiss (never dropping focus to `<body>`). |
| **M-2 (Refusals silent to assistive tech)** | Blocking `.alert-item` rows now carry `role="alert"`, and `showAlert()` announces blocking plain-text summaries via `announce()` (which cancels any pending 50 ms timer via `clearTimeout` so explicit caller announcements cleanly supersede). |
| **m-1 (Alert tiering & dedup)** | Benign search miss (`main.js` `"No exact match"`) now passes `{ tier: 'info', id: 'search-no-match' }`. `showLockAlert()` passes `{ tier: 'blocking', id: 'lock', dismissible: false }` and `buildAlertItems()` deduplicates `'lock'` and `'readout-unavailable'` against `state.manualAlert` and ignores stale lock alerts once `state.lock === null`. |
| **m-2 / N-1 / N-2 (Handoff honesty & scope proof)** | Updated Section 4 above and this section to report G2 and Stage B explicitly, describe wheel isolation accurately as a containment guard, and cite `MultiApi.Stand.cs:281-346, 444, 512` confirming whole-woodland Work Plan order lists on 3+ lane stands. |

### Post-Repair Verification Results

| Gate | Command | Result |
|---|---|---|
| **Build** | `/home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror -p:P0CorePath=/home/jer/Development/CCF-Multi-2D-Phase2A2-Evidence-20261127/p0ref-seed/SitkaLab.Core.dll` | **PASS** — `0 Warning(s), 0 Error(s)` |
| **Module Graph** | `/home/jer/.local/share/vite-plus/bin/node Tools/MultiRackSitkaP1Lab/build/2d-gate/module-graph.mjs` | **PASS** — `16 modules, acyclic, all reachable from main.js, 1 module script tag` |
| **G2 (`ui-smoke-multiarea.sh`)** | `NODE=/home/jer/.local/share/vite-plus/bin/node bash Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-mfl-r1-g2-smoke` | **PASS — `363 passed, 0 failed`** (exact parity with base `674c7c9e`; all `F2a..`, `R2a..`, `R3a..R3j`, `J01..J23`, `P2a..P2n`, `S2a..S2l`, `J99`, `Z01..Z04` green) |
| **Targeted Edge (`map-first-lenses-ui`)** | `bash Tools/MultiRackSitkaP1Lab/tests/map-first-lenses-ui/run-map-first-lenses-edge.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-mfl-r1-edge-3` | **PASS — `56 passed, 0 failed`** (`1920×1080`: `24/24`, `1280×720`: `24/24`, server SIGINT/stderr: `8/8`; now also asserts M-1 live Skip-refusal `Open Work Plan` button, M-2 `role="alert"` + `#announce`, M-3 Enter focus retention on `#alerts-expand` + post-Dismiss focus, and m-1 `info`-tier search miss) |
| **Stage B (`run-step2-edge.sh`)** | `bash Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/run-step2-edge.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-mfl-r1-step2-3` | **EXACT MATCH TO BASE — `55 passed, 10 failed`** (all 55 UI, layout, single-row topbar, 1280×720 R3 fold, NB-2 order 3 & order 2 at 1280 & 1920, Skip, History drawer, Structure, and Save/Restart/Load checks **PASS**; all 10 failures are the pre-existing carried server Year-30 `503 PAYLOAD BUDGET EXCEEDED` defect documented in `PHASE2B2B-MILESTONE1-CLOSEOUT-20261127.md` §4 & §6 on base `674c7c9e`, with byte-identical sweep sizes `ma-1-mixed` Y00–Y06 = `393501/400141/404285/406670/408943/412640/415260`, `bytes=524551`/`527778` vs `524288`) |
| **Skipped per brief** | `build/2d-gate/selftest.sh`, full `2d-gate.sh --browser`, `Core.Tests`, `Phase1.Save.Tests` | **SKIPPED** (excluded by task brief) |

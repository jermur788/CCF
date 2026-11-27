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

All nine locked owner decisions (Option B on items 1–4, Safe Hybrid on item 5, plus structural guardrails and Part 2 readiness) are implemented strictly within the existing 16 plain ES modules under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/` (0 new JS modules, zero Core/Assets/`csproj`/`MultiRoutes.cs`/roadmap edits):

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
   - Scroll-ownership isolation (`overscroll-behavior: contain` + wheel target scoping in `main.js`): wheel scrolling over `#side`, `#legend-panel`, or `#history-drawer` never zooms or pans the SVG map.
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

- **Marked Crop Trees count scope in Woodland Navigator:** The server overview payload (`GET api/multi/stand`) returns summary metrics per lane while individual stem `isCropTree` flags arrive in the lane-filtered payload (`GET api/multi/stand?lanes=...`). Rather than inventing JS estimates or changing backend routes (`src/Ui/*.cs` is out of scope), the Navigator honestly labels the Crop Trees section with a `Visible lanes (N of M)` scope badge (`data-scope-kind="visible"`), while `Lanes` and `Unfinished Work` carry `Whole woodland` badges (`data-scope-kind="woodland"`).
- **Screen-reader / live assistive-technology check:** ARIA roles, `aria-pressed`, `aria-expanded`, `aria-describedby`, `role="status"` / `role="alert"`, keyboard focus reveal, and `Esc` layering are verified in headless Edge DOM/CDP, but manual live testing with NVDA/JAWS/VoiceOver was not performed in this environment.
- **Older Edge suites (`phase2a-ui`, `phase2b2b-ui`):** Not re-run here because the task scope specifies running the targeted Map-First Lenses Edge suite rather than the full `2d-gate.sh --browser` sweep; Playable Milestone 1 core flows (3-way felling, Approve/Execute/Advance, cross-process Save v3) are directly verified in `drive-map-first-lenses.mjs`.
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

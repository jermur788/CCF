# CCF Multi 2D — Map-First Menu & Lenses Upgrade — Specification

- **Spec ID:** `SPEC-multi-2d-map-first-lenses-ui-V1`
- **Feature ID:** `multi-2d-map-first-lenses-ui-v1`
- **Status:** `APPROVED` (Owner UI decisions locked: Option B on items 1–4, Safe Hybrid on item 5; authorised for implementation on this lane)
- **Base Commit:** `674c7c9e470d0aff0a028bd284fd7510f973e2d5` (`origin/integration/2d-sim`, Playable Milestone 1 green)
- **Branch:** `feat/multi-2d-map-first-lenses-ui-20261127-v1`
- **Authority:** Task-level implementation authority for this UI architecture upgrade only; subordinate to latest user instruction, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md`, and `Docs/Project/game-brief.md`. Implementation truth remains the inspected repository state.
- **Scope:** Frontend `/multi` presentation assets only (`Tools/MultiRackSitkaP1Lab/src/Ui/multi/**`, preserving the exact 16 existing plain ES modules — **0 new JS modules**), targeted headless-Edge verification suite (`Tools/MultiRackSitkaP1Lab/tests/map-first-lenses-ui/**`), and lane handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/map-first-lenses-ui/**`). Zero edits to `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Tools/MultiRackSitkaP1Lab/src/Core/**`, `Tools/MultiRackSitkaP1Lab/src/Ui/*.cs`, `SitkaP1.Ui.csproj`, `src/Ui/wwwroot/**`, `build/2d-gate/**`, `Docs/Project/**`, or `origin/main`.

---

## 1. Locked Owner UI Decisions

| # | Decision | Binding Rule |
|---|---|---|
| **1** | **Five Task-Focused Lenses Strictly Separate from Active Tools (Option B)** | Provide five presentation lenses — **Woodland** (default), **Trees**, **Regeneration**, **Harvesting**, and **Habitat** — strictly separate from active map interaction tools (`Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear`). Switching lenses changes presentation emphasis, default overlay, contextual legend items, and available tools **only**. Switching lenses **never** mutates simulation state, **never** submits/approves/executes work, **always** resets the active tool to safe inspection (`Select Tree`), and **preserves** tree selection, pending Fell drafts, marks, Work Plan orders, and map pan/zoom/lane position. |
| **2** | **Default Adjacent Lanes + 1-Lane Precision Toggle (Option B)** | Extend (do not rebuild) the existing multi-lane view (`>= 3` lanes) and compact mini-map. Default to adjacent lanes: **2–3 visible lanes at 1920×1080** (2 at outer edges, 3 at interior lanes) and **2 visible lanes at 1280×720** (active lane + immediate neighbour). Retain and clarify the **1-lane precision toggle** (`#lane-mode-toggle` / hotkey `M`), and highlight the **active visible lane range** on the compact mini-map. Two-area stands (`MA-1`, `MA-1-MIXED`) keep their existing two-area switcher behaviour. |
| **3** | **Compact Contextual Legend + Expand Toggle (Option B)** | Replace the permanent dedicated legend column (`--legend-w`) with a **compact contextual legend bar** integrated into `.map-panel` showing only the key symbols relevant to the active lens/overlay, paired with an **Expand/Full legend toggle** that opens the complete 16-symbol legend as a non-blocking floating popover inside `.map-pane`. Desktop `.workspace` uses strictly **2 columns** (`map | side`), reclaiming roughly **21–26% map width** at both 1920×1080 (~21%) and 1280×720 (~26%). Never introduce a fourth permanent column. |
| **4** | **Priority-Ranked Grouped Non-Blocking Alerts (Option B)** | Replace ad-hoc single alert messages with a priority-ranked, grouped, non-blocking alert strip in the header/top band (zero modal pop-up chains): **Blocking** (persistent explanation + corrective action, e.g. stale session lock, unavailable readout, payload overflow, refusal, or future inspection-only plot); **Attention Required** (warning + direct Work Plan link for unfinished pending/approved work or insufficient cash/hours); **Significant Event** (grouped event notice with direct map/results navigation — ready for future storms, never one popup per stem); **Information** (quiet dismissible notices — ready for future mast years/observations); and **Historical** (direct link to the existing single Decision → Outcome History drawer). |
| **5** | **Single-Level Accessible Pinned Explanations (Safe Hybrid)** | Provide **one shared accessible explanation popover component** (`#explain-popover`) for key forestry/UI terms. Reveals on pointer hover or keyboard focus; click or Enter/Space pins/unpins; `Esc` dismisses topmost layer; satisfies **WCAG 2.2 SC 1.4.13** (dismissible, hoverable, persistent). Explicitly **defer 2-level nested links** inside explanations. Never hide essential restrictions, disabled reasons, or server refusal codes solely inside tooltips or explanations. |

---

## 2. Five Lenses, Tool Separation & Part 2 Extension Slots

Every lens is defined in a single data-driven `LENSES` registry in `store.js`. Opening any lens always defaults `mapMode` to `'tree'` (`Select Tree`, safe read-only inspection).

| Hotkey | Lens ID | Label | Default Overlay | Available Tools | Map / Inspector Focus | Data-Driven Part 2 Extension Slots |
|---|---|---|---|---|---|---|
| `1` | `woodland` | **Woodland** *(default)* | `off` | `Select Tree`, `Inspect Cell` | Whole-stand overview, canopy & living stems, lane orientation. | Aggregates active Part 2 layers without adding a 4th column. |
| `2` | `trees` | **Trees** | `off` | `Select Tree` | Dimensions, Hegyi competition, crop-tree marking (`C`), pruning (`✂`), timber quality. | `scots-pine-profile` (Scots Pine & Soil Moisture), `birch-alder-profiles` (Downy Birch & Common Alder), `real-woodland-measurements` (Year-0 measured trees). |
| `3` | `regeneration` | **Regeneration** | `light` | `Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear` | Modelled light, undergrowth, natural seedlings, planting points, 8-yr shelters, spot clearing. **Opening Regeneration never auto-activates `Plant` or `Spot Clear`.** | `soil-moisture-overlay` (5-level moisture suitability), `jay-acorn-dispersal` (mast-year acorn landings & nearby oak edge), `birch-alder-planting` (nursery species). |
| `4` | `harvesting` | **Harvesting** | `off` | `Select Tree` | Proposed Fell orders (`◷P` / `✓A`), executed stumps (`X`), retained Crop protection (`C`), timber grades, 3-way destinations, contractor quotes. | `scots-pine-timber-grader` (Sawlog/SRW/Pulp grades), `windstorm-salvage` (fallen storm-stem salvage choice & dated windblow note). |
| `5` | `habitat` | **Habitat** | `undergrowth` | `Select Tree`, `Inspect Cell` | Fallen deadwood logs, standing dead (`D`), species diversity, undergrowth cover, stand structure (`M_i`, `T_i`). | `real-woodland-microhabitats` (TreMs microhabitats & survey provenance), `windstorm-deadwood-resilience` (wind-aligned fallen deadwood icons), `sensitive-wet-ground` (wet/bog moisture caution). |

### Lens Switch Invariants
1. Zero mutating HTTP calls on lens switch.
2. Zero simulation, Work Plan, or save-state changes on lens switch.
3. Active tool always resets to `'tree'` (`Select Tree`); any unsent `plantDraft` or `spotDraft` coordinate preview is cleared safely without touching adult Fell drafts (`state.drafts`), selected tree (`state.selected`), crop marks, Work Plan orders, active area (`state.areaId`), `singleLaneMode`, `zoom`, or `centre`.
4. User may still manually change the overlay dropdown (`#overlay-select`) within any lens after switching.
5. Extension slots are pure declarative metadata descriptors rendered honestly as read-only "Ready in Part 2" / data-bound slots; Part 2 features (`Scots Pine`, `Real-World Woodland Inspection`, `Jay/Windstorms`, `Birch/Alder`) are **not wired** in this lane.

---

## 3. Collapsible Woodland Navigator & Safe Bounded Hotkeys

### Woodland Navigator Tab (`#pane-navigator`)
- Lives inside the existing right-hand contextual panel (`#side`) as the `Navigator` tab (`data-pane-button="navigator"`), never a 4th column.
- Contains collapsible `<details>` sections with **Previous / Next** cycling controls and explicit **Count Scope** badges (`Whole woodland` vs `Visible lanes`):
  1. **Lanes / Areas** (`Whole woodland` scope): lists all lanes/areas from `allAreas()`, indicates active/visible/off-screen state and living count, provides click-to-focus plus `Prev` (`[`) / `Next` (`]`) cycling.
  2. **Marked Crop Trees** (`Visible lanes` on `>= 3`-lane stands with clear disclosure when off-screen lanes exist; `Whole woodland` on 2-area stands where all areas are loaded): lists marked Crop Trees with click-to-select plus `Prev` / `Next` cycling.
  3. **Unfinished Work** (`Whole woodland` scope): lists all Pending and Approved orders across all 6 verbs with non-colour state pills (`◷ Pending` / `✓ Approved`), click-to-navigate, plus `Prev` / `Next` cycling.
  4. **Storm-felled Stems** *(Part 2 extension slot)*: clean collapsible slot ready for 2D Windstorms (`0` / inactive until wired).
  5. **Inspection Blockers** *(Part 2 extension slot)*: clean collapsible slot ready for Real-World Woodland Inspection (`0` / inactive until wired).

### Safe Bounded Hotkeys
- **Strict text-entry guard:** All shortcuts (except `Escape` per precedence rules) are ignored whenever focus is inside `input`, `textarea`, `select`, `option`, any `contenteditable` element, or when `Ctrl` / `Meta` / `Alt` modifiers are pressed, or while a modal `<dialog open>` is active.
- **Allowed shortcuts only:**
  - `1`–`5`: Switch to Lenses 1–5 (`Woodland`, `Trees`, `Regeneration`, `Harvesting`, `Habitat`).
  - `[` / `]`: Previous / Next lane or area.
  - `H`: Toggle the existing single Woodland History drawer.
  - `W`: Open the Work Plan tab inside `#side`.
  - `M`: Toggle 1-lane precision vs adjacent-lanes view (on multi-lane stands).
  - `?`: Open/close the read-only Keyboard Shortcuts help dialog (`#shortcuts-dialog`).
  - `Esc`: Dismiss topmost layer in strict order:
    1. Open native `<dialog open>` (including `#shortcuts-dialog`)
    2. Open `#explain-popover` (pinned or revealed explanation)
    3. Expanded `#legend-panel` full-legend popove
    4. Open `#history-drawer` (preserving selection, drafts, lens, mode, overlay)
    5. Non-empty `<input type="search">` (native clear)
    6. Active non-inspection tool (`plant` / `spot` / `cell`) → `Select Tree` (preserving Fell drafts & overlay)
    7. Active planted-sapling (`PJ`) inspection → adult tree inspecto
    8. Selected adult tree inspection
- **Forbidden hotkeys (hard safety rule):** Zero unguarded hotkeys for `Approve`, `Cancel Approved`, `Execute`, `Advance Year`, `Skip Year`, `New Woodland`, `Load Save`, or `Money Reset`.

---

## 4. Structural Guardrails & Anti-Pattern Guards

1. **Dark Woodland Palette Preserved:** `--page: #0b1523`, `--panel: #152438`, `--panel-3: #20354f`, `--text: #e2ecf7`.
2. **Zero New JS Modules:** Implement strictly within the existing 16 plain ES modules under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/`. Preserve `map/contract.js`, `SitkaP1.Ui.csproj`, `MultiRoutes.cs`, and `build/2d-gate/module-graph.mjs` (acyclic, layer-ordered, LF, no BOM, single `<script type="module" src="multi/main.js"></script>`).
3. **Zero-Build & Strict CSP:** Zero npm/bundler, zero inline JS, zero HTML inline `style=` attributes, zero forestry or financial arithmetic in browser JS (D-020).
4. **Redundant Non-Colour State Encoding ("Encode Twice"):** Every lens, alert severity, legend entry, Work Plan pill, and navigator status pairs a distinct icon/shape/border pattern + text label + colour.
5. **Unchanged Commitment Gate & Save Workflows:** Preserve `Mark → Quote → Approve → Execute → Advance Time`, 3-way felling dispositions (`Sell & Extract`, `Keep for Own Use`, `Leave as Deadwood`), and Save v2 / Save v3 round-trips without altering any server route or Core logic.
6. **Explicit Anti-Pattern Guards:**
   - **Scroll-ownership isolation:** Scrolling inside `#side`, `#legend-panel`, `#history-drawer`, or `#explain-popover` **never** zooms or pans `#map` (`overscroll-behavior: contain` + explicit wheel guard).
   - **No modal pop-up chains:** Alerts are inline, grouped, and non-blocking.
   - **Selection & context preservation:** Opening/closing History drawer, Details tab, Navigator tab, or switching lenses never drops tree selection, Fell draft ticks, crop marks, or map pan/zoom/lane state.
   - **Zero horizontal scroll:** `document.documentElement.scrollWidth <= window.innerWidth + 1` at both `1920×1080` and `1280×720`.
   - **Above-the-fold marking gate at 1280×720:** With a living reference tree selected in `Select Tree` mode, `#inspector-heading`, competitor ranks `1–5`, and `#add-button` remain completely visible above `720px` without scrolling `#pane-tree`.

---

## 5. Stop Conditions

Stop and report immediately if:
- Any C# / Core / `SitkaP1.Ui.csproj` / `MultiRoutes.cs` edit becomes necessary;
- Any existing Playable Milestone 1 Work Plan (`Mark → Quote → Approve → Execute → Advance`), 3-way felling disposition, or Save v2/v3 workflow cannot be preserved;
- Any forbidden path (`Assets/**`, `src/Core/**`, `src/Ui/wwwroot/**`, `build/2d-gate/**`, `Docs/Project/**`, `origin/main`) would need modification.

---

## 6. Acceptance Criteria (`MLUI-01..12` + `MLUI-REG`)

| ID | Criterion |
|---|---|
| **MLUI-01** | All 5 lenses (`Woodland`, `Trees`, `Regeneration`, `Harvesting`, `Habitat`) switch cleanly, apply presentation/overlay defaults and lens-scoped tools only, never mutate simulation or Work Plan, always reset active tool to safe `Select Tree` (opening Regeneration never auto-activates Plant), and preserve selection, Fell drafts, marks, and map pan/zoom/lane position. |
| **MLUI-02** | Compact contextual legend bar + expand toggle replaces the permanent legend column, leaving strictly 2 desktop workspace columns (`map | side`), zero 4th column, and reclaiming ~21–26% map width at both `1920×1080` and `1280×720`. |
| **MLUI-03** | Default multi-lane view shows adjacent lanes (`2–3` at `1920×1080`, `2` at `1280×720`), 1-lane precision toggle switches to `1` lane and back, and the compact mini-map highlights the active visible range in sync. |
| **MLUI-04** | Collapsible Woodland Navigator tab inside `#side` lists Lanes, Marked Crop Trees, Unfinished Work, and Part 2 extension slots (Storm-felled stems, Inspection blockers), supports Prev/Next cycling, and displays explicit visible-vs-whole-woodland count scope. |
| **MLUI-05** | Priority-ranked grouped non-blocking alerts render in the top/header strip across tiers (`Blocking`, `Attention Required`, `Significant Event`, `Information`, plus `Historical` link to existing History drawer) with redundant non-colour encoding and zero modal chains. |
| **MLUI-06** | Single-level accessible pinned explanation component reveals on hover and keyboard focus, pins on click/Enter/Space, remains hoverable/persistent (WCAG 2.2 SC 1.4.13), dismisses on `Esc` as topmost non-modal layer, contains zero 2-level nested links, and never hides essential restrictions/refusals solely inside tooltips. |
| **MLUI-07** | Safe bounded hotkeys (`1–5`, `[` / `]`, `H`, `W`, `M`, `?`, `Esc`) work outside text fields, are strictly ignored inside `input`/`textarea`/`select`/`contenteditable`, and assign zero unguarded shortcuts to Approve, Execute, or destructive session replacement. |
| **MLUI-08** | Scroll-ownership isolation: wheel scrolling inside `#side` (and `#legend-panel`/`#history-drawer`) scrolls only the panel and never zooms or pans the underlying map. |
| **MLUI-09** | Layout & fold integrity at both `1920×1080` and `1280×720`: zero page-wide horizontal scroll, selection preserved across History/Details/Navigator/Lenses, and critical crop-tree/competitor marking controls (reference tree + ranks `1–5` + `Add`) remain strictly above the fold at `1280×720`. |
| **MLUI-10** | Data-driven Part 2 extension slots (`Scots Pine & Soil Moisture`, `Real-World Woodland Inspection`, `Jay Acorn Dispersal & 2D Windstorms`, `Downy Birch & Common Alder`) exist cleanly in the lens/navigator/alert registries without wiring unimplemented simulation logic. |
| **MLUI-11** | Playable Milestone 1 flows remain intact end-to-end: `Mark → Quote → Approve → Execute → Advance`, 3-way felling dispositions (`Sell & Extract`, `Keep for Own Use`, `Leave as Deadwood`), and Save v3 cross-process round-trip. |
| **MLUI-12** | Zero console errors, zero uncaught exceptions, zero CSP violations, zero 404/5xx responses across both viewports; clean `.NET 8` build of `SitkaP1.Ui.csproj` (0 warnings, 0 errors); `module-graph.mjs` passes with exact 16 modules. |
| **MLUI-REG** | Strict file-fence compliance (`git diff --name-only` 100% inside allowed paths, 0 Core/Assets/csproj/routes edits, `2D_Simulator_Strategic_Roadmap.md` byte-identical to base, LF/UTF-8 no BOM, clean `git status --short`). |

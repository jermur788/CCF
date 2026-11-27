# CCF Multi 2D — Map-First Menu & Lenses Upgrade — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-map-first-lenses-ui-V1`
- **Branch:** `feat/multi-2d-map-first-lenses-ui-20261127-v1`
- **Base:** `674c7c9e470d0aff0a028bd284fd7510f973e2d5`

---

## 1. Pre-Settled 16-Module Architecture Mapping (Zero New JS Modules)

All implementation lives inside the existing 16 plain ES modules under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/`, preserving the S0c layer order (`L0 → L5`), `map/contract.js`, `MultiRoutes.cs`, and `SitkaP1.Ui.csproj` byte-for-byte.

| Layer | Existing File | Planned Responsibility in This Upgrade |
|---|---|---|
| Markup | `multi.html` | Replace permanent `<aside id="legend-panel">` third column with compact contextual `#legend-bar` + floating expandable `#legend-panel` inside `.map-panel`; add `#lens-bar` (5 lenses strictly separated from `.mode-switch` tools) inside `.map-panel`; place `.view-tabs` (including `Navigator`) at the top of `#side`; add `#pane-navigator`; upgrade `.messages` to priority-ranked `#alert-strip` (preserving `#alert`); add shared `#explain-popover` and read-only `#shortcuts-dialog`. Zero inline `<script>`, zero inline `style=`. |
| Styles | `multi.css` | Switch desktop `.workspace` to strictly 2 columns (`minmax(0, 1fr) var(--side-w)`) reclaiming ~21% width at 1920×1080 and ~26% at 1280×720; preserve dark woodland palette (`#0b1523`, `#152438`, `#20354f`, `#e2ecf7`); style 5-lens bar, per-lens map presentation accents (`[data-lens="..."]`), compact contextual legend strip & floating full-legend popover, priority-ranked non-blocking alert tiers, accessible `#explain-popover`, `#pane-navigator` collapsibles, mini-map visible-range frame, and scroll-isolation (`overscroll-behavior: contain`). Keep 1280×720 above-the-fold margin for reference tree + ranks 1–5 + Add. |
| L0 | `store.js` | Add `LENSES` registry (with `defaultOverlay`, `tools`, `legendKeys`, `extensionSlots` for Part 2), `ALERT_TIERS`, `EXPLANATIONS` (single-level help copy), `SHORTCUTS` registry, `PANES` including `'navigator'`, responsive `visibleLaneIds(areas, activeAreaId, singleLaneMode, viewportWidth)` returning 2–3 lanes at `>= 1440px` and 2 lanes at `< 1440px` (or 1 when `singleLaneMode`), and UI-only state fields (`lens`, `legendExpanded`, `alertsExpanded`, `dismissedAlerts`, `manualAlert`, `extensionAlerts`, `navigatorIndex`, `explainState`). Reset UI-only lens/alert transients cleanly in `resetSessionUiState()`. |
| L1 | `dom.js` | Add shared single-level accessible explanation helpers (`explainTrigger`, `initExplainPopover`, `openExplain`, `closeExplain`, `isExplainOpen`) implementing WCAG 2.2 SC 1.4.13 (hover, keyboard focus, click/Enter/Space pin, viewport clamp, Esc dismiss) with zero nested triggers and zero inline `style=` attributes in HTML. |
| L2 | `api.js` | Upgrade alert rendering (`renderAlertStrip`, `showAlert`, `clearAlert`, `dismissAlert`) to compose priority-ranked grouped non-blocking alerts (`blocking` → `attention` → `event` → `info`, plus `historical` link) from live server state (`state.lock`, readout unavailability, economy upfront-cash block, unfinished Work Plan orders, natural deaths / executed work, `manualAlert`, and Part 2 `extensionAlerts`) without modal chains or JS forestry math. |
| L3 | `map/contract.js` | Preserved unchanged (`MAP_RENDERER_METHODS`, `assertMapRenderer`). |
| L3 | `map/minimap.js` | Extend compact mini-map to draw an explicit **active visible range highlight frame** spanning all currently visible lanes, synchronized with responsive adjacent-lane count (2–3 at 1920×1080, 2 at 1280×720) and 1-lane precision mode, with clear scope label (`Showing X of N lanes` / `1 lane precision`). |
| L3 | `map/renderer-svg.js` | Pass viewport width into `visibleLaneIds(...)`; apply `data-lens` visual presentation emphasis on `#map-pane`; replace side-column `setLegendHidden` with `renderCompactLegend()` (contextual symbols for active lens + overlay) and `setLegendExpanded(expanded, moveFocus)` toggling the floating full-legend popover inside `.map-pane` without altering `.workspace` column count. |
| L4 | `modes/interactive.js` | Add `setLens(lensId, moveFocus)` strictly separating lenses from active tools: applies lens presentation & default overlay, always resets `mapMode` to `'tree'` (safe inspection), clears provisional `plantDraft`/`spotDraft` only, preserves `selected`, `drafts`, `overlap`, `competition`, `zoom`, `centre`, `areaId`, and `singleLaneMode`, and renders zero mutating HTTP calls. Guard `setMapMode` to allow tools permitted by the active lens (or switch safely). |
| L4 | `panels/inspector.js` | Render lens-aware contextual hint & single-level explanation triggers (e.g., Hegyi CI, Crop Tree, Pruning) without disturbing 1280×720 above-the-fold fold geometry; surface clean read-only Part 2 extension slot metadata when applicable. |
| L4 | `panels/fell-draft.js` | Preserve exact competitor 1–5 & Add flow; ensure Fell draft ticks remain untouched across lens switches, Navigator/History/Details navigation, and Esc dismissal of upper layers. |
| L4 | `panels/work-plan.js` | Add `renderNavigator()` and Navigator Prev/Next cycling helpers (`cycleNavigator(category, delta)`) with explicit `Whole woodland` vs `Visible lanes` count scope badges and Part 2 extension slots (Storm-felled stems, Inspection blockers); attach single-level explanation triggers to Work Plan commitment gate & €2,500 harvest floor without hiding refusal/blocking text; preserve `showPane` including `'navigator'`. |
| L4 | `panels/time-history.js` | Keep single History drawer unchanged for Decision → Outcome & Financial Ledger; preserve selection/drafts/lens when opening/closing History. |
| L4 | `dialogs/{woodland,saves,execute-confirm}.js` | Preserve Save v2/v3, Forest Maker, and Execute confirmation flows unchanged; route loaded-save notices cleanly into the non-blocking `info` alert tier. |
| L4 | `commands.js` | Preserve all fenced Work Plan commands and Economy flows unchanged. |
| L5 | `main.js` | Wire 5-lens bar, compact legend expand/collapse toggle, alert strip actions (Work Plan link, Map focus, History link, Dismiss, Expand), Navigator click & Prev/Next cycling, shared `#explain-popover` lifecycle, `#shortcuts-dialog`, unified safe bounded keyboard handler (`1–5`, `[` / `]`, `H`, `W`, `M`, `?`, `Esc` with strict text-entry guard), responsive viewport lane-window sync on resize, and explicit wheel scroll-ownership isolation on side/popover/drawer panels. |

---

## 2. Targeted Verification Strategy (Minimum Necessary Testing Diet)

1. **Static & Build Gates:**
   - `$HOME/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj` — 0 warnings, 0 errors.
   - `node Tools/MultiRackSitkaP1Lab/build/2d-gate/module-graph.mjs` — PASS, exact 16 modules, acyclic, 1 module script tag, LF-only, no BOM.
   - File-boundary check: `git diff --name-only 674c7c9e470d0aff0a028bd284fd7510f973e2d5..HEAD` 100% inside writable fence.
2. **Targeted Headless-Edge Qualification (`Tools/MultiRackSitkaP1Lab/tests/map-first-lenses-ui/`):**
   - `run-map-first-lenses-edge.sh` + `drive-map-first-lenses.mjs` (following `phase2b2b-ui` Edge/CDP pattern, evidence written outside repo under `/mnt/c/Users/hp/AppData/Local/Temp/...`, fresh ephemeral server ports leaving `:33767`, `:43649`, `:46593` untouched).
   - Runs full acceptance matrix at **both `1920×1080` and `1280×720`** (including cross-process server restart for Save v3):
     - All 5 lenses switching cleanly, zero mutation, safe `Select Tree` default (Regeneration never auto-activates Plant), selection/drafts/marks/pan/zoom/lane preserved;
     - Compact contextual legend + expand toggle, ~21–26% map width reclaimed, 2 desktop columns (never 4), zero horizontal scroll, reference tree + ranks `1–5` + `Add` above the fold at `1280×720`;
     - Adjacent-lanes default (`2–3` at 1920×1080, `2` at 1280×720) + 1-lane precision toggle + mini-map visible-range highlight sync;
     - Collapsible Woodland Navigator tab + Prev/Next cycling + visible-vs-whole-woodland count scope + Part 2 extension slots;
     - Priority-ranked grouped non-blocking alerts (Blocking, Attention Required, Significant Event, Information, Historical link; zero modal chains);
     - Single-level accessible pinned explanations (hover + keyboard focus + pin + Esc dismissal, WCAG 2.2 SC 1.4.13, zero nested triggers, restrictions remain inline);
     - Safe keyboard shortcuts (`1–5`, `[` / `]`, `H`, `W`, `M`, `?`, `Esc`) guarded in text inputs, zero unguarded Approve/Execute/Reset shortcuts;
     - Scroll-ownership isolation (mouseWheel over `#side` never zooms `#map`);
     - Unchanged Playable Milestone 1 Work Plan (`Mark → Quote → Approve → Execute → Advance`), 3-way felling choice, and Save v3 cross-process round-trip;
     - Zero console errors, zero CSP violations, zero 404/5xx.
3. **Explicitly Skipped (per packet):** `build/2d-gate/selftest.sh`, full `2d-gate.sh --browser` sweep, `Core.Tests`, and `Phase1.Save.Tests` (Core and Save are untouched and verified at base).

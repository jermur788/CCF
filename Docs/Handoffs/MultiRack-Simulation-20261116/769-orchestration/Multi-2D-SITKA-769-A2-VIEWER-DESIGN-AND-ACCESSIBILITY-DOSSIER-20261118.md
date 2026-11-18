# Multi #769 — A2 Viewer design and accessibility/browser acceptance dossier (docs-only)

**18 November 2026 · [Multi #769](https://github.com/jermur788/CCF/issues/769) · Manager [#730](https://github.com/jermur788/CCF/issues/730)**

**Status: COMPLETE DRAFT, docs-only. NOT source, NOT implementation, NOT qualification.** No code was executed, built, hashed or browsed; every acceptance row is **NOT RUN**.

| Field | Recorded |
| --- | --- |
| Actual author | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), provider `claudeAgent`, Claude Code/T3 native child thread |
| Child thread | `thread:delegated-task:command%3Amcp%3A168e4aa7-2aa0-41fd-94c6-fabccfa02532%3Adelegate-task%3A3ac71cc2-2ac2-4391-b1e0-e7326a088448` |
| Delegating parent | DeepSeek `opencode-go/deepseek-v4.1-flash` (high), T3 thread `3325d55e-b948-4569-b8db-4ea574220d57`, native TG Fork `delegate_task` |
| Docs branch | `docs/multi-769-a2-viewer-design-dossier-20261118-v1`, base `f6e52be18399637ec93a20a53b937b7bd097b39e` |
| Sole path | `Docs/Handoffs/MultiRack-Simulation-20261116/769-orchestration/Multi-2D-SITKA-769-A2-VIEWER-DESIGN-AND-ACCESSIBILITY-DOSSIER-20261118.md` |
| Authority | Design only. G2 source HELD. No writer, branch, file or run is authorised by this document. |
| Forbidden models | GPT-6.2 Sol/Astra were never selected, escalated or used as fallback. |

---

# Viewer (A): Coding-Task Design and Accessibility/Browser Acceptance Dossier
**CCF Multi #769 · 2D Sitka H0-A Year0 static viewer · contract `ccf.h0a.core-read` v1.0.0 (consumed read-only)**

## 1. Scope, authority and non-goals

| Item | Position |
|---|---|
| Nature | Docs-only design. Nothing here was executed, built, hashed or browsed, so **no PASS is claimed**. Every acceptance row below is a *proposed* check, status NOT RUN. |
| Authority | The adopted contract is fixed and not redesigned here. Field names (`xMm`, `yMm`, `crownRadiusM.value`, `dbhMm`, `belowActionNote`, `actionNote.comparison`, `centredX/Z`) are cited from the task packet. The writer must confirm them against the frozen contract before coding. |
| Gate | G2 is held. This dossier authorises no writer, branch, path or run. |
| Non-goals | Core, fixture or assembler edits. Marking, Year 1, finance, save and Host. Any decode, parse or hash of fixture or P0 bytes. Any BA, Dq, RD or N/ha computation. Any DBH comparison or "eligible/ineligible" wording. Health colours. Servers or network. |

## 2. File/responsibility mapping (frozen allowlist under `Tools/MultiRackSitkaYear0Viewer/`)

| File | Owns | Must NOT contain |
|---|---|---|
| `viewer/shell.template.html` | CSP `<meta>` as the **first** child of `<head>`, with hash placeholders filled by the assembler. Static landmarks: `header` (disclosure banner), `main`, polite `role="status"` live region, `role="alert"` refusal slot. `<template>` elements for map, list, inspector and chooser, so refused states have **no** such DOM. Script/style slots. | External `src`/`href`, inline event handlers, `<link>`, `<img>`, `<form>`, `<base>`, modules. The disclosure wording is copied verbatim from the adopted contract, never re-authored. |
| `viewer/viewer.css` | Layout, focus rings, text/non-text contrast tokens, `@media (prefers-reduced-motion)` and `forced-colors` rules. | `url()`, `@import`, `@font-face`, animations or transitions that carry meaning. |
| `viewer/viewer-boot.js` | The first action: `CcfYear0Core.contract==="ccf.h0a.core-read" && contractVersion==="1.0.0"`. Then `openGenesis()`, refusal vs admit branching, the held admitted observation, and the `SelectionNotificationV1` factory (`deepFrozen`) and dispatcher. The only module that touches the Core global. | Drawing, hit-testing, key handling, fixture data, any Core test hook. |
| `viewer/viewer-map.js` | Pure view-plane functions (projection, fit, clamp, zoom, scale-bar choice, hit-test candidates) and canvas drawing. | DOM outside the canvas. Core calls. Any data derived from display values handed back to Core. |
| `viewer/viewer-ui.js` | Complete-ID listbox, inspector (exact key display), chooser, live-region text, button controls and key handling. Calls `isCurrentSelection` on every notification. | Geometry maths beyond display formatting. Any DBH comparison. |
| `tests/viewer/**` | Immutable public-schema doubles, DOM/geometry tests, mutant checks. | Core test hooks, fixture bytes or pins, copies of protected data. |

**Namespace decision (D-V1, proposed).** The three modules are IIFEs. They share one A-owned namespace, `CcfYear0ViewerV1`. It is created by the first-loaded module, deep-frozen at the end of boot, and is never named like a Core global. Load order is map, ui, boot. V-07 permits exactly two new window properties: Core's global and this one.

## 3. Data flow

1. **Gate.** Check `CcfYear0Core.contract` and `contractVersion` strictly. On a missing global or a mismatch, render **only** the incompatibility notice (a bounded fixed string plus the two observed values, `textContent`, truncated to 80 characters each) and the disclosure banner. `openGenesis` is not called.
2. **Open.** Call `openGenesis()`. Hold the result as `observation` (Core-frozen). A passes no arguments or bytes.
3. **Refusal.** Show a bounded status: a fixed heading, the refusal code and reason via `textContent` (≤200 characters), and the disclosure banner. Instantiate no map, list or inspector template. Hiding is not sufficient.
4. **Admit.** Verify the 100 trees are exactly P30-001..P30-100, one each. If not, treat it as a Viewer-side refusal-style notice, not a partial render. Instantiate the templates. Draw the map. Build the list from Core order, with an ordinal `treeId` sort only for tie-breaks.
5. **F1.** Render only what Core reports. Unknown or unavailable shows the words "Not available" with a reason from Core. It never shows `0`, `—0`, or a blank that reads as zero.
6. **Annotation.** If `belowActionNote` is present, show `actionNote.comparison` verbatim (`textContent`). A never reads `dbhMm` to decide this.
7. **Selection.** Input (pointer, keyboard or list) → `SelectionNotificationV1 = deepFrozen({type:"ccf.h0a.selection", treeId, observationKey, via})`. Every consumer (map highlight, inspector, live region) calls `isCurrentSelection(observation, ev)` first. `false` means ignore silently, with no DOM change.
8. **Test doubles.** Immutable objects matching the public schema only. Required variants: good admit, refusal, wrong `contract`, wrong `contractVersion`, F1-unavailable, a below-note tree, a coincident-centre pair, and a second foreign observation. Doubles carry a spy counter on `openGenesis`, `keysEqual` and `isCurrentSelection`. They use no Core internals and no real fixture content.

## 4. Canvas rendering and interaction

**View plane.** Metres come from integer `xMm/1000` and `yMm/1000` only. `centredX/Z` appear in the inspector as labelled display text and are never used to draw.

**Projection.** `cssX = ox + (xM − camX)·s` and `cssY = oy − (yM − camY)·s`, with `ox = w/2` and `oy = h/2` in CSS px. `s` is one finite uniform scale in CSS px/m. Y is flipped so north-up is true.

**Backing store.** Set `canvas.width = round(cssW·dpr)` and `canvas.height = round(cssH·dpr)`, then `ctx.setTransform(dpr,0,0,dpr,0,0)` at the start of each frame. All coordinates in drawing and hit-testing are CSS px. Re-run on `resize` and on dpr change (`matchMedia` resolution query). Skip drawing if `cssW` or `cssH` is 0.

**Fit and limits.** `s_fit = min(cssW, cssH)/(extentM + 2·margin)`, where the extent comes from min/max of admitted `xM,yM` plus the maximum crown radius, to cover the 30 m plot. Require `Number.isFinite(s) && s>0`, otherwise reset to `s_fit`. Clamp `s ∈ [0.5·s_fit, 16·s_fit]`. Clamp the camera to the padded extents, so the map cannot be lost.

**Zoom.** Wheel/pinch about the pointer: `wx = camX + (px−ox)/s`, `wy = camY + (oy−py)/s`. After `s'`, `camX' = wx − (px−ox)/s'` and `camY' = wy − (oy−py)/s'`. The Zoom in/out buttons use ×1.25 about the canvas centre. Pan uses pointer drag, or Pan buttons that move 10% of the view.

**Draw order.** Crowns, then stems, then selection glyph. Crown radius in px is `crownRadiusM.value·s` (**radius**, not diameter). Crowns are a low-alpha fill plus a 1.5 px outline so overlaps stay legible. Stems are a fixed marker (4 CSS px). The selection glyph is a thick double ring (white/black) with four tick marks, so it reads by shape and not by colour.

**Scale bar.** Choose the largest 1/2/5×10ᵏ m bar of 60–160 CSS px at the current `s`. Label it "N m" in text, with the number of metres exact.

**Hit-testing.**
- CSS px only.
- Candidates are stems whose squared CSS distance to the pointer is ≤ pick radius² (proposed 12 CSS px, 22 for touch).
- Winner = smallest squared distance. Ties are broken by ordinal `treeId` string comparison (code-unit `<`).
- **Chooser:** if more than one candidate's centre lies within 6 CSS px of the winner, or two centres coincide, open the chooser. It is an inline listbox of those complete IDs, focus moves into it, Esc closes it and returns focus to the canvas, and Enter selects with `via:"pointer"` or `"keyboard"` as appropriate.

**Keyboard inspector (list).**
- A listbox of all 100 complete IDs.
- Roving `tabindex` is one tab stop. Up/Down, Home/End and PageUp/PageDown (±10) move focus only.
- Enter/Space selects (`via:"keyboard"`; list click is `via:"list"`).
- Selection never changes on bare focus, so there is no announcement flood.
- The inspector shows labelled rows via `textContent` in `<code>`: `treeId`, the **exact observation key** (every member untruncated, unreformatted, with units), mm and m coordinates, `centredX/Z` (labelled "display only"), crown radius m, DBH mm, F1 state, and the annotation if any.

**Stale/foreign rejection.** A late or foreign notification (different observation, or a `treeId` not in the observation) returns `false` from `isCurrentSelection`. The consumer returns with no DOM, canvas or live-region change.

## 5. Accessibility matrix

| Area | Design |
|---|---|
| Keyboard-only path | Banner → view controls (Zoom in, Zoom out, Reset, 4 Pan buttons) → ID list (single tab stop) → chooser when open → inspector (focusable heading, `tabindex=-1`). All 100 IDs reachable and selectable without a pointer. |
| Focus order | Matches DOM and visual order. There are no positive `tabindex` values. Selection never steals focus. The chooser returns focus on close. |
| ARIA | Canvas is `role="img"` with an `aria-label` summary ("Plan view, 100 trees, 30 m plot; use the tree list for full access"). List is `role="listbox"` with `aria-label`; items are `role="option"` with `aria-selected`, `aria-posinset` and `aria-setsize`. Inspector is `role="region"` with a label. Live region is `role="status"` (`aria-atomic`). Refusal is `role="alert"`. |
| Colour-independent cues | The selection glyph is shape-based. The F1 and annotation states are text labels, and nothing is encoded by hue alone. There is no health colour. |
| Live announcements | Polite only, on: selection ("Selected P30-017, 17 of 100"), chooser open/close, view reset. Pan/zoom are not announced. |
| Contrast | Text ≥4.5:1. UI components, focus ring, selection glyph, scale bar and crown outline ≥3:1 against adjacent backgrounds. Focus ring ≥2 px. Check by sampled measurement, not by eye. |
| Reduced motion | No animation exists. Pan/zoom are instant. The `prefers-reduced-motion` rule is retained defensively. |
| Forced colours | Rely on system colours for text and focus. The glyph keeps its shape contrast. |
| SR script outline | NVDA+Firefox or Orca+Firefox, if available. (1) Load; the banner is read first. (2) Tab to the list; hear "P30-001, 1 of 100". (3) Arrow through; hear no selection announcements. (4) Press Enter; hear one "Selected…". (5) Check the inspector region reads the exact key. (6) Trigger a refusal double; the alert is read. Record the SR and version actually used, or "NOT RUN". |

## 6. Browser/offline acceptance (all rows NOT RUN until a future run)

| ID | Check | Exact expected observable |
|---|---|---|
| V-01 | Open the package from `file://` in Linux **Firefox 159** and **Chromium 156** (actual versions recorded) | Page renders map, list and inspector. Console has 0 errors and 0 CSP violations. |
| V-02 | CSP enforcement: a scratch **mutant** copy with one byte altered in one inline script/style, plus a copy with an injected inline `onclick` | The mutated block does not execute. A CSP violation is logged naming the blocked hash/directive. The Viewer fails closed (no partial UI), not partially functional. |
| V-03 | DevTools Network, cache disabled, reload | Exactly **one** request (the document). Any other entry, including a favicon attempt, is recorded verbatim and is a STOP-for-decision, not waved through. |
| V-04 | All 100 IDs by keyboard only | Tab/arrows/Enter select each of P30-001..P30-100. The inspector `treeId` matches each. The count is 100. No pointer events were used. |
| V-05 | Pointer parity and chooser | Click on a lone stem selects the same ID as the list. A coincident-centre pair opens the chooser listing both. Tie order is ordinal by `treeId`. |
| V-06 | Foreign/stale key rejection (double with two observations) | `isCurrentSelection` returns `false`. A `MutationObserver` records 0 DOM mutations and the live-region text is unchanged. |
| V-07 | No global leaks | `Object.getOwnPropertyNames(window)` diff before/after = only Core's global and `CcfYear0ViewerV1`. No second Core-like global. |
| V-08 | Deterministic package | Two independent assemblies from the same inputs are byte-identical, with SHA-256 compared by external tooling (not in-page). No timestamps or random values are embedded. |
| V-09 | Version mismatch (`"1.0.1"`, `"2.0.0"`, wrong `contract`, missing global) | Only the notice and banner render. The `openGenesis` spy count is 0. There is no canvas, list or inspector node. |
| V-10 | Refusal from `openGenesis` | Bounded status plus banner. No map, list or inspector. Text ≤ the limits above. |
| V-11 | F1 unavailable | "Not available" with Core's reason. No standalone `0` for F1 in the DOM text. |
| V-12 | Annotation | Trees with `belowActionNote` show `actionNote.comparison` verbatim. `textContent` of the whole page contains neither "eligible" nor "ineligible". A's source contains no `dbhMm` comparison (static search). |
| V-13 | Geometry and dpr (dpr 1, 1.5, 2 via browser zoom or emulation) | Backing store = `round(css·dpr)`. A sampled crown edge sits at `crownRadiusM.value·s` CSS px from its stem. A 10 m scale bar spans `10·s` CSS px. |
| V-14 | Frozen Core objects | A strict-mode mutation attempt on any Core object throws. A's code never assigns into Core objects. |

## 7. Security/trust-negative checks visible to A

| Check | Expected |
|---|---|
| External resources | A search of the package finds no `http(s)://`, `//`-relative URL, `src=`, `href=` to a non-fragment target, `url(`, `@import`, `<link>`, `<img>`, `<iframe>` or `<form>`. |
| Network APIs | No `fetch`, `XMLHttpRequest`, `WebSocket`, `EventSource`, `sendBeacon`, `Worker`, `import()` or `<script type=module>`. |
| Storage | No `localStorage`, `sessionStorage`, `indexedDB`, `document.cookie` or Cache API use. |
| Dynamic code | No `eval`, `new Function`, string-argument `setTimeout/Interval`, `innerHTML` or `document.write` (all text via `textContent`). |
| Crypto | No `crypto.subtle` or hashing in A. |
| Core boundary | One Core global, a version check first, and only three facade calls. No alternate or shimmed Core global. A passes no bytes. |
| Test hygiene | Doubles contain no fixture bytes and no pins. No Core test hook is referenced. |

## 8. STOP conditions and residual risks

**STOP and escalate (never weaken the CSP or the contract):**
- Hash-based CSP cannot run from `file://` in either browser, or a browser needs a server. Do not loosen directives.
- Core freeze **Cf** is absent, or the contract's field names or types differ from the packet's.
- A needs anything outside the allowlist (Core, fixture, assembler), a fourth JS file, bytes, or a DBH comparison.
- V-03 shows any extra request.
- The disclosure wording is not available verbatim.
- Contract or version mismatch handling cannot be made fail-closed.

**Residual risks (unverified):**
- Firefox 159 and Chromium 156 availability and exact behaviour on this Linux host are unverified.
- Meta-CSP hash handling on `file://` is untested. Favicon requests are untested.
- The pick-radius, zoom-limit and tie/chooser thresholds are proposals that need owner confirmation.
- The `CcfYear0ViewerV1` namespace is a proposal (D-V1).
- The screen-reader environment is unconfirmed.
- No source write rights exist; G2 is held.
- Dark mode is deferred.

## 9. Ready-to-execute Viewer task brief (a template, not a grant)

| Field | Content |
|---|---|
| Task | Implement the A Viewer per sections 2–7. |
| Branch / base | `<branch named by custodian>` created from **Cf** after the custody fast-forward. Both the fast-forward and the freeze must be confirmed in the task packet. |
| Allowlist | `Tools/MultiRackSitkaYear0Viewer/`: `viewer/shell.template.html`, `viewer/viewer.css`, at most 3 `viewer/viewer-*.js` (planned: boot, map, ui), `tests/viewer/**`. Nothing else. |
| Preconditions | G2 released in writing. The writer is named, with the lane and lock recorded. The frozen contract v1.0.0 doc and Core facade are readable. The disclosure text is supplied. Reviewer is distinct from the writer. |
| Forbidden | Git actions beyond the packet's, Core/fixture/assembler edits, protected-byte access, network, storage, new packages, weakened CSP. |
| Output | Source diff, doubles-based test results with exact commands, V-01..V-14 evidence with browser versions (or NOT RUN with reasons), and a list of deviations. No claim of integration. |

Author: Claude (claudeAgent), docs-only design; no code executed.

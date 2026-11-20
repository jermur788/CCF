# CCF Multi #870 — Blind B result: "Canopy Ledger" 2D forestry UI prototype

**Date:** 2026-11-20 · **Task:** [#870](https://github.com/jermur788/CCF/issues/870) · **Manager:** [#863](https://github.com/jermur788/CCF/issues/863) · **Status:** HANDED OFF — prototype ready for the owner's blind comparison. **Unmerged. Not independently reviewed.**

> **PROTOTYPE — NOT THE REAL FORESTRY ENGINE.** All data and outcomes are mock. This is not real-playable forestry and it is not connected to the qualified #849 engine (`4c42e201…` on `feat/multi-standmap-playable2d-20261120-v1`).

## Identity and provenance

| Item | Value |
|---|---|
| Author | Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), Claude Code on a T3 Code host, on the owner's Windows 11 + WSL Ubuntu machine |
| GitHub session | local `gh`/git authenticated as `jermur788`. Commit author name set per-commit to `Claude Opus 5.5 (claude-opus-5-5)`. The repo's configured `user.name` (`DeepSeek V4.1 Flash 1017`) was **not** used and was **not** changed. |
| Registration | [#870 comment 7040649935](https://github.com/jermur788/CCF/issues/870#issuecomment-7040649935), RUNNING, posted before any tracked write |
| Packet | `docs/multi-870-blind-claude-ui-prototype-packet-20261120-v1` @ `0b84fdb2` |
| Branch | `experiment/multi-870-blind-claude-ui-v1` (new) |
| Start (observed `main`) | `2e9fd8cb63a69adfbd76ced054c46e1a84b9412e` |
| Prototype commit | `5363ce383eb731e76f68bbf6170cd16213433eff` |
| Workspace | separate sparse git worktree `~/Development/CCF-870-BlindB`. The owner's dirty `CCF-HP` main checkout was not touched. |
| `main` drift | `origin/main` moved to `f1f2dd8d…` during the task, from other work. The branch was not rebased; it's based on the observed start. |

**Blind rule:** I didn't open the previous Unity/2D UI or its screenshots, the StandMapView code, the #849 browser `index.html`/`app.css`/`app.js`, the #868 design packet, or the other prototype. The only inputs were the #870 packet text and the #870 issue text. `active-tasks.md` was searched for #870/BlindB/UiExperiments only (no matching row).

## Diff against the start (GitHub compare, read back)

`2e9fd8cb…...experiment/multi-870-blind-claude-ui-v1`: ahead 1, behind 0, after the prototype commit. That commit only **adds** files:

| File | Lines | Bytes | SHA-256 (GitHub raw readback = local) |
|---|---|---|---|
| `Tools/UiExperiments/BlindB/index.html` | +208 | 11348 | `0755d29ad60772cb0afeab0d9d1ac38a2f2bd0d527c6e22d8135335077a72c12` |
| `Tools/UiExperiments/BlindB/style.css` | +316 | 16882 | `e03b5d870b6853a27b67d31b48a83bf0b3fb2d2ddb23d55d0df916c0c73c52d8` |
| `Tools/UiExperiments/BlindB/app.js` | +724 | 30286 | `95d34d001f45d6de8094d9ceef35f140560deb7e9dc53b927b57b27b7c324e48` |
| `Tools/UiExperiments/BlindB/README.md` | +70 | 6515 | `4886726aa04774ea9058e28b49085fb088acc8916d7da92adfbcf30a1c275f97` |

This result Markdown is added in a second commit. Nothing else changed: no Assets, Unity, specs, `Tools/MultiRackSitkaP1Lab/`, P0, saves, coordination or authority docs. There are no new dependencies, network calls or credentials. The prototype is not merged.

## How to open it

1. `git fetch origin experiment/multi-870-blind-claude-ui-v1`, then check it out (or download `Tools/UiExperiments/BlindB/`).
2. Open `Tools/UiExperiments/BlindB/index.html` in a current Chrome, Edge or Firefox. It's plain static HTML/CSS/JS with no modules, `fetch`, CDN or fonts, so `file://` **should** work, but I didn't test it that way. **Tested route:** run `python3 -m http.server 8870 --bind 127.0.0.1` in that folder and open `http://127.0.0.1:8870/`.
3. Reload to reset. There's no persistence.

`README.md` beside the files has the full controls and a guided workflow.

## Design (original)

The concept is a **surveyor's ledger**: warm paper panels around a dark top-down "night survey" map.

- **Map:** north-up SVG of the 30 × 30 m stand with a 5 m labelled grid. Crowns are sized by mock DBH and numbered. State is shown by shape and glyph as well as colour: dashed ring ⚑ = pending, solid blue ring ✓ = approved, ringed stump ✕ = felled. An amber halo marks the selection.
- **Pan/zoom:** a compact 44 px **compass cross** (N/E/S/W with Fit in the hub) plus a +/− pair, floating in the map's empty margin. **Fit** shifts the stand so the pad and legend never cover trees. Dragging pans, the wheel zooms at the cursor, and there are keyboard shortcuts. This replaces seven full-width text buttons.
- **Dossier panel:** exact-ID search, then the selected-tree card (ID, status, DBH, height, position) with the reason form, then tabs **Work Plan / History / All trees**. Only the inside of a tab scrolls, so there's no 100-row page.
- **Workflow:** a Draft → Approved → Executed tracker. **Approve** only locks the plan and logs "Trees felled by approval: 0". **Execute…** is a separate red button that opens a modal; a confirmation checkbox must be ticked before "Fell trees" is enabled.
- **Blocked commands:** buttons use `aria-disabled` so they stay focusable and explain why when pressed. Guards are also inside the functions (for example, calling `executePlan()` a second time does nothing).

Trade-offs are listed in the README table. The main ones: labels are small when the whole stand is fitted on small screens, and the deliberate execute flow costs extra clicks.

**Felling reasons:** the packet doesn't give the owner's chosen wording, so the dropdown has **placeholder** entries (Thinning, Poor form, Damage/disease, Wind risk, Access route, Mature/final harvest, Other). The owner needs to supply the real list. Other requires a custom text of at least 3 non-space characters.

## Actual vs mocked

| Real in the prototype | Mocked |
|---|---|
| UI behaviour, state transitions, validation, keyboard/pointer handling, layout | All 100 trees (seeded generator, seed 870), DBH, height, position, "growth" on Year +1, basal area, plan IDs, approval/execution, history |

There's no API, server or engine logic, and no ecological claim. The mock allometry (height ≈ 4 + 0.62 × DBH) and growth (0.25–0.8 cm/yr) are arbitrary interface placeholders, **not research-based**.

## Test evidence (what was actually run)

**Environment:** the T3 Code embedded Chromium preview, page served by `python3 -m http.server` on 127.0.0.1:8870 in WSL. The server was stopped afterwards. `node --check app.js` passed.

**Limit to note:** the preview tool's real pointer/keyboard automation **did not work** in this session (every `click`/`press` timed out and the tab reported `visible:false`). From mid-session, screenshots also failed ("browser is not producing screenshot frames"). So:
- **Interaction tests used synthetic DOM events** (`PointerEvent` on crown elements, `KeyboardEvent` on the map and tabs, `.click()` on buttons) dispatched with `preview_evaluate` in the real page. They exercise the app's own handlers, but **trusted human input was NOT verified.**
- **Layout was verified by measuring element boxes** (`getBoundingClientRect`, scroll sizes) at each viewport. **No final-state screenshot exists.** One early screenshot (before the crown, legend and narrow-layout fixes) isn't included as evidence of the final build.

**Functional results** (final build, fresh load, all PASS, no JS errors):

| Check | Observed |
|---|---|
| Map click selects a tree; card shows ID/status/DBH/height | T042 → "Tree T042 · Standing · DBH 36.1 cm · 25.3 m" |
| Add with no reason | blocked: "Choose a felling reason.", `aria-invalid`, 0 orders |
| Other with blank text | blocked: "…A blank Other reason is not allowed.", 0 orders |
| Other with text / preset reason | order recorded, e.g. `T042: Other: Shading planted oak`; tree shows ⚑ pending |
| Duplicate order for the same tree | blocked "T042 is already in the plan" |
| Exact ID `t57` / `57` / `100`; invalid `T999` | selected T057/T100; "No tree “T999”. Valid IDs are T001–T100." |
| Execute before approval | blocked; Standing 100 |
| Approve | stage Approved, **Standing 100, Felled 0**; log "Trees felled by approval: 0" |
| Order while approved | form hidden: "P-001 is approved and locked…" |
| Execute → dialog: Go while unticked; Cancel; Esc | no change; "Execution cancelled. Nothing felled."; focus returns to Execute |
| Execute confirmed (2 trees) | **Standing 98, Felled 2**; crowns hidden, stumps shown with ✕; plan Executed |
| Execute again (UI and direct `executePlan()`) | refused "Duplicate execution refused; nothing felled"; counts unchanged |
| Stump selected | "Felled in year 1 under P-001. A stump cannot be ordered again." |
| Year +1 | Year 2; standing trees grew (T001 DBH 38.16 → 38.74), stumps unchanged |
| Approve an empty or executed plan | blocked with messages |
| Next order after execution | auto-starts plan P-002 |
| Map keys: Enter / arrows / Shift+arrows / + − / F / Esc | select nearest to centre / move to the neighbouring tree / pan / zoom / fit / clear selection; Enter jumps to the reason select |
| Tabs: ArrowRight / End | moves selection and focus correctly |
| Pan N, zoom clamp | viewBox shifts; zoom clamped to 0.8×–6.6× |
| History | ordered log of order, approve, execute, blocked, year and plan events |
| All-trees filter "stump" | 2 rows (T042, T057); empty filter shows 100 rows |

**Layout results** (measured):

| Viewport | Page scroll (W×H) | Result |
|---|---|---|
| 1280×720 | 1280×720 — **none** | map 880×620, dossier 400 px; stand fully visible; **0 crowns** under the pad or legend; plan actions at y 449–493 |
| 1920×1080 | 1920×1080 — **none** | stand 891 px; 0 crowns covered |
| 640×360 (= 1280×720 at 200% zoom) | 625×1222 — vertical only | stacked: map, then controls and legend strip, then dossier; no overlap; 0 crowns covered |
| 360×740 (narrow phone) | 345×1297 — vertical only | same stacked layout; no horizontal scroll; 0 crowns covered |

At every size, every visible button, input, select and tab outside the dialog and the All-trees table measured **≥ 44×44 px**. Table ID buttons are 36 px tall; exact-ID search is the full-size alternative. Browser-level 200% zoom was **simulated by CSS viewport size**, not by the zoom control itself.

**Contrast** (computed, WCAG formula): body text 12.9:1; secondary text ≥ 6.66:1; primary and danger buttons 7.6 and 7.5:1; mock tag 7.5:1; banner 12.6:1; crown labels 5.46:1 (raised from 4.08 by darkening the crown); state rings and selection halo on the ground 7.4–8.3:1.

**Not verified:** real mouse, touch or keyboard input; Firefox and Safari; `file://` launch; screen readers; final rendered visual appearance (no screenshot); performance on low-end hardware.

## Accessibility summary

Landmarks, skip link, labelled form controls, a single-tab-stop map using `role="application"` with roving arrow-key selection, ARIA tabs, a native modal `<dialog>`, live regions for selection, errors and toasts, statuses that never rely on colour alone, visible focus rings, and `prefers-reduced-motion` support. Limitations are listed above and in the README.

## Bugs found and fixed during the task

1. The dialog's `close` event never fired in the test browser, so confirmed Execute did nothing. Fixed: the buttons now act directly, and Esc is handled via `cancel`.
2. The floating pad and legend covered south-east and bottom-row trees at 1280×720. Fixed: Fit offsets the stand and the legend moved into the gutter.
3. On narrow layouts the dossier was drawn over the map, and overlays covered trees. Fixed: a stacked flow layout below 900 px.
4. Crown label contrast was 4.08:1. Fixed: 5.46:1.

## Handback

- **To #870:** prototype B is complete on `experiment/multi-870-blind-claude-ui-v1`. Owner: open it as described above, and compare only once both blind prototypes are finished.
- **To #863 Manager:** additive-only diff verified by GitHub readback. Runtime testing used synthetic events and box measurement in embedded Chromium; trusted input and screenshots were not available. **This is author self-testing, not independent review or qualification.** Nothing is merged. Connecting any chosen design to the real engine still needs the owner's choice and the approved four-file production UI spec (#868).
- **Open owner inputs:** the real felling-reason wording, and whether a "withdraw approval" step is wanted.
- **Writer status:** after this result is published, Claude B releases the branch and paths; no further writes are planned. The local worktree `~/Development/CCF-870-BlindB` remains for owner inspection and can be removed with `git worktree remove`.

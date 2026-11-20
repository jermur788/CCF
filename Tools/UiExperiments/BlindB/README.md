# Canopy Ledger — Blind B 2D UI prototype (CCF #870)

> **PROTOTYPE — NOT THE REAL FORESTRY ENGINE.** Every tree, DBH, height, growth figure and basal-area value here is deterministic **mock demo data** generated in `app.js`. Nothing connects to the qualified #849 C# engine, its API or any save. This build exists only so the owner can compare interface ideas.

## Open it

No install, server, Unity or internet needed. It's three static files (`index.html`, `style.css`, `app.js`), with no external fonts, libraries or network requests.

1. Check out branch `experiment/multi-870-blind-claude-ui-v1` (or download this folder).
2. Double-click `Tools/UiExperiments/BlindB/index.html` to open it in a current Chrome, Edge or Firefox. It uses no modules or `fetch`, so `file://` should work. The author tested it only over loopback HTTP, as below.
   - Optional: `python3 -m http.server 8870 --bind 127.0.0.1` in this folder, then open `http://127.0.0.1:8870/`.
3. Reload the page to reset everything. There is deliberately no saving.

## Design idea

A **field surveyor's ledger**: warm paper-coloured panels around a dark, top-down "night survey" map. The dark ground makes crowns, state rings and the amber selection halo easy to see. The paper side reads like a notebook.

| Area | Decision | Trade-off |
|---|---|---|
| Map | North-up SVG plan of the 30 × 30 m stand, with a 5 m grid labelled in metres. Each crown is sized by mock DBH and labelled with its number. | All 100 labels are always visible. At the smallest sizes they get small, so zoom helps. |
| Pan / zoom | A **compass cross** (N/E/S/W around a central Fit button) plus a vertical +/− pair, all 44 px. It floats in the map's spare margin. Fit shifts the stand left so the pad never covers trees. You can also drag to pan and scroll to zoom. | It's an overlay on wide screens. Below 900 CSS px it moves under the map. |
| Tree states | Every state has its own shape and glyph as well as its colour: **dashed white ring + ⚑** = pending order; **solid blue ring + ✓** = approved; **stump with growth rings + ✕** = felled. The legend uses the same symbols. | The rings add some clutter when many trees are ordered. |
| Right "dossier" | Exact-ID search at the top, a selected-tree card with the order form, then tabs: **Work Plan / History / All trees**. Only the inside of a tab scrolls, so the 100-row list never makes the page long. | The full list is one tab away rather than always on screen. |
| Workflow | The plan shows a 3-step tracker: **Draft → Approved → Executed**. **Approve** only locks the list (the log records "Trees felled by approval: 0"). **Execute…** is a separate red button that opens a confirmation dialog; you must tick a box before "Fell trees" is enabled. | Two deliberate clicks plus a checkbox, which is intentionally slow. |
| Blocked actions | Approve and Execute use `aria-disabled` instead of `disabled`. They stay focusable, and clicking them while blocked shows the reason (for example "approve first" or "already executed"). | They look dimmed but can still be pressed, which is intentional. |

## Controls

- **Mouse:** click a crown to select it. Drag to pan; the mouse wheel zooms at the cursor.
- **Map keyboard** (Tab to the map first): arrow keys move to the nearest tree in that direction. **Shift+arrows** pan, **+ / −** zoom, **F** or **0** fits the stand, **Enter** selects the tree nearest the centre (or jumps to the reason field once a tree is selected), and **Esc** clears the selection.
- **Exact ID:** type `T042`, `t42` or `42` and press **Go** or Enter. Anything outside T001–T100 is rejected with a message.
- **Tabs:** Left/Right/Home/End keys. **Dialog:** Esc cancels, and nothing is felled.

## Workflow to try

1. Select a tree, choose a reason, then **Add to Work Plan**. The tree gets a ⚑ ring and Standing stays 100.
2. Try adding with no reason, or with **Other** and a blank description. Both are blocked with a message next to the field.
3. Click **Approve plan**. The stage becomes *Approved · locked*. Standing is unchanged, the log says 0 felled, and new orders are refused until the plan is executed.
4. Click **Execute…**, tick the box, then **Fell trees**. Crowns become ✕ stumps, Standing/Felled change, and the plan closes.
5. Click **Execute…** again. It's refused ("Duplicate execution refused; nothing felled"). Calling `executePlan()` directly is a no-op too.
6. **Year +1** applies fake growth to standing trees and logs it. The next order starts plan P-002 automatically.

## Felling reasons — placeholder wording

The packet says the owner chose a predefined reason dropdown plus Other/custom, but it doesn't give the list's wording. These labels are placeholders for the owner to replace: Thinning — release a crop tree; Poor form; Damage or disease; Wind instability risk; Access / extraction route; Mature — final harvest; Other (describe). An Other reason needs at least 3 non-space characters.

## Mock vs real

| Mock in this prototype | Real engine (#849, not connected) |
|---|---|
| 100 trees from a seeded jittered grid (seed 870); DBH 14–40 cm; height = 4 + 0.62 × DBH ± noise | Qualified synthetic stand and its own data |
| "Year +1" adds random 0.25–0.8 cm DBH. It is not a growth model. | Engine simulation |
| Basal area (m²/ha) is computed from the mock DBH values | Engine calculations |
| Plan IDs, approval and execution exist only in browser memory | Server-authoritative API with fences, PlanId and execution keys |

Labels marked **MOCK**, plus the yellow banner, show which numbers are fake.

## Accessibility notes

- Semantic landmarks, a skip link, labelled controls, `role="tablist"`, a modal `<dialog>`, and `aria-live` for selection, field errors and toasts.
- The map is a single focusable `role="application"` element with roving keyboard selection, rather than 100 separate Tab stops.
- States are never shown by colour alone. Measured contrast includes: body text 12.9:1, secondary text ≥ 6.7:1, buttons ≥ 7.5:1 and crown labels 5.5:1.
- Every interactive control outside the tree table is at least 44 × 44 px. Table ID links are 36 px tall; each row's ID is also reachable through exact-ID search.
- Respects `prefers-reduced-motion`.

## Known limitations

- It doesn't re-fit automatically after a window resize; press Fit.
- There's no undo after execution, no "withdraw approval", and no save/load, all by design for this mock.
- It hasn't been tested with a real screen reader.

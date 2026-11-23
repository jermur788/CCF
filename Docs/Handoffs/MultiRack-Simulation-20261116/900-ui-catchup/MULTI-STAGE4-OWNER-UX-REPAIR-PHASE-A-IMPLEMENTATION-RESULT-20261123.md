# MULTI Stage 4 owner UX repair, Phase A: implementation result

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Spec | `specs/multi-stage4-owner-ux-20261123-v1/` at `b95a56bf` (APPROVED, Phase A only) |
| Role | Sole implementer (Claude Opus 5.5). **No self-review**: the independent reviewer is a different agent and thread |
| Branch | `feat/multi-stage4-owner-ux-20261123-v1` |
| Base | `e7c1a57da519ed1ee03ccfbad240196a66df0905` (reviewed Stage 4 UI catch-up candidate; not moved) |
| Source/test HEAD | `9fed4eb91ed51522e3be2188c9f1539d06106d4d` |
| Final HEAD | the commit that adds this file, directly on top of `9fed4eb9` (evidence + driver + this handoff; no source change) |
| Worktree | `C:\Users\hp\.t3\worktrees\CCF-HP\t3-8fc40b07-b22b8bb5869955c0` (Windows checkout of a worktree whose git dir is `/home/jer/Development/CCF-HP/.git/worktrees/t3-8fc40b07-b22b8bb5869955c0`; WSL path `/mnt/c/Users/hp/.t3/worktrees/CCF-HP/t3-8fc40b07-b22b8bb5869955c0`) |
| Phase B (UX-10) | **Not started.** It was not needed to satisfy Phase A |
| Integration | None. No merge, no rebase, no force push, `main` untouched |

Unity, Windows builds, save/load and `main` integration were **not** run and are **not** claimed. Everything here is about the standalone browser `/multi` page in `Tools/MultiRackSitkaP1Lab`.

## 1. Commits

| Commit | What |
|---|---|
| `267be9ff` | `feat(multi-owner-ux)`: UX-01..UX-09 and F-2 in `MultiAreaPage` (`PlayerMultiApi.cs`) |
| `0e17f641` | `test(ui-smoke-multiarea)`: F-2 unpaired read-out checks F2a..F2h in the existing page witness |
| `9fed4eb9` | `fix(multi-owner-ux)`: keep "Add selected N to Fell plan" on one line (found by the browser fold re-check) |
| *(this commit)* | Browser driver for this task, results, screenshots, this handoff |

## 2. Changed files and why

| File | Purpose |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` | The whole change, inside the `MultiAreaPage` raw strings (HTML, CSS, JS). No route, payload, fence or C# logic changed (the C# above `MultiAreaPage` is byte-identical) |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` | Adds an `unpaired` fault mode (the server's real answer with `stage4` set to null) and eight F-2 checks. Existing checks untouched |
| `Docs/Handoffs/.../900-ui-catchup/browser-driver-owner-ux/cdp-ux.mjs` | New. The committed `../browser-driver/cdp.mjs` pattern plus real key events, clipped screenshots and a tiny PNG decoder for pixel checks. The old driver is not modified |
| `Docs/Handoffs/.../900-ui-catchup/browser-driver-owner-ux/scenario-owner-ux.mjs` | New. 63 real-browser checks per viewport |
| `Docs/Handoffs/.../900-ui-catchup/browser-driver-owner-ux/browser-results-{1280x720,1920x1080}.json`, `browser-run-*.txt` | Raw results of the final runs (capability redacted, checked: zero 64-hex tokens) |
| `Docs/Handoffs/.../900-ui-catchup/evidence-owner-ux/*.png` | 11 screenshots (§7) |
| this file | Handoff |

Not touched (verified with `git diff e7c1a57d HEAD`, empty): `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Core/**`, `fixtures/**`, the `.sln`, `.p0ref`, Unity `Assets/**`, packages, saves, the approved quartet, every historical handoff and the previous `browser-driver/`.

## 3. Requirement mapping (claimed by the implementer; observed results in §5)

| ID | Status | What was done |
|---|---|---|
| **UX-01** | DONE | Inspector reordered: compact reference block (eyebrow + ID + status chip on one line, beside **× Close inspection**; one line of DBH / Height / Crown r), then **Competitors for Fell** ranks 1–5, then the sticky Add bar. Fell reason, Crop, "Fell this tree instead", Pruning, remove/approved notes, competition details and the tree record now sit **below** the primary flow (`.tree-more`). Age moved to the Tree record (it was already listed there). Fewer than five: heading shows "N of 5", no padding rows. At 1280×720 the five rows end at y=651 and the Add button at y=706 (viewport 720), pane scrollTop 0, both with nothing chosen and with all five chosen |
| **UX-02** | DONE | The numbered rank tiles are real `<button>`s on a new HTML layer over the map (`#rank-layer`), each with `data-rank-toggle="<engine tree ID>"`, `aria-pressed`, ARIA label "Competitor k, tree ID, Fell draft", 28×28 px, 3 px focus outline. A click goes through the same `toggleDraft(treeId)` as the inspector checkbox, so both directions stay in sync by ID. Positions come from the display-only spread layout and are **never** used to find a tree. The layer is a sibling of the SVG, so a rank click never reaches the map handler: no hit-test, no reference change, no request. Blocked competitors stay focusable with `aria-disabled` and announce why. Chosen state = filled tile + tick corner + `aria-pressed` (not colour alone); the count is announced through the existing polite live region. No new route |
| **UX-03** | DONE | **× Close inspection** beside the ID; Escape runs the same action. Precedence: (1) an open `<dialog>` closes itself first and the handler does nothing in that key press; (2) a search box with text clears itself first; (3) **Inspect ground** stops; (4) the inspection closes. Closing clears that area's selection, its reference's unsent draft and batch result, and the enlarged focus layout; zoom, pan, area, Light Guide, Crop designations, queued/approved orders and year are kept; nothing is sent. With the Light Guide on, Escape never turns it off |
| **UX-04** | DONE | One quiet line "Synthetic Sitka experiment · 2 areas · Site unknown" + native `<details>` "Details". Inside, unchanged and verbatim: the full Stage 4 disclosure (`MultiDisclosureStage4`: synthetic not surveyed, not comparable, site unknown, connected woodland for growth/crowns, per-area density mortality / Work Plans / summaries, no transfer, no Unity parity), the chips with their full-statement tooltips, "Session only" and a link to the Details pane. The map-foot line no longer repeats "site UNKNOWN · growth and crowns use both areas…" (AC07) |
| **UX-05** | DONE | Dock 158 px wide (was 212). Compass 3×3 (N top-centre, W/E middle, S bottom-centre, empty centre), then one `− · Fit · +` row, then `Area A · Area B` on one row. 44 px targets kept; north-up, pan/zoom limits, Fit and area-switch handlers unchanged |
| **UX-06** | DONE | Light cells moved **under the crowns** (`#light-cells` now precedes `#crowns`, clipped to the area, no pointer events, no text). Per cell: `t = L^0.65`, opacity `= 0.9·t` (floor `0.0001` for any `L > 0`, written with 4 decimals, so it can never print as 0), colour linear through `#7F3B4A → #DA796D → #FFD9BD` by `t`. `L = 0`: no element. Old numbered band markers, `lc-1..5` fills and their halos (F-3) are gone. OFF by default |
| **UX-07** | DONE | Ground panel: level word (Very shaded / Low / Moderate / Bright; `L = 0` adds "no modelled light"), "Light index 0.037 on a 0–1 scale (modelled; 1 = unshaded)", "Canopy cover 96% of this cell under living crowns" (the engine's separate `geometricCoveredFraction`), "Shading trees 8". Closed disclosures: **Contributing trees (n)** with the IDs; **Technical details** with the index (4 dp), the cover fraction, column/row, grid size and clipped m², centre X/Z, cross-border trees, light model label + version, "Calibrated PAR no", the guide label and the not-calibrated caveat |
| **UX-08** | DONE, *with an interpretation to review* (§6 D-1) | With the Light Guide on and Inspect ground off, a crown click selects the tree exactly as before (server hit-test). **Inspect ground** is an explicit `aria-pressed` toggle shown only with the Light Guide on; while on, a map click finds the engine cell containing the point and shows it (crosshair cursor, status line, outline on the chosen cell). Rank buttons and dialogs still take precedence. A point with no cell says "No modelled ground sample… No value is shown or estimated". Leaving (button, **× Stop**, Escape) restores tree selection and leaves the Light Guide as it was |
| **UX-09** | DONE | The map legend strip says "Light Guide · Shaded [ramp] Bright · Inspect ground reads the value at a spot." The ramp is an SVG gradient sampled from the same mapping. No band labels, no "modelled light – not measured sunlight", no calibration text on the map. Source/version/uncertainty remain in the ground panel's Technical details and the Details pane (unchanged). The level is always given as text + number in the inspector |
| **F-2** | DONE | `readoutState()` = `stage4` / `legacy` / `unavailable` (`stage4: null`). Unavailable shows neither the model 0 statements (edge exclusion, "Light … NOT MODELLED") nor the Stage 4 ones; notice, Details pane, history line and rack cards say the read-out is unavailable. **Approve and Execute are disabled** (Execute label "… · re-read needed"), the Work Plan pane shows a notice with a Re-read button, `openExecuteDialog` refuses with a Re-read alert, and an Execute confirmation open when an unpaired view arrives is withdrawn unsent (`samePrunes` now returns false without a read-out). Server-side stale-fence 409 untouched and re-verified |

## 4. Verification commands (run by me in this lane)

```bash
# WSL, pinned toolchain; TMPDIR outside the repo
export TMPDIR=/tmp/opus-ownerux/tmp
export GIT_DIR=/home/jer/Development/CCF-HP/.git/worktrees/t3-8fc40b07-b22b8bb5869955c0 GIT_WORK_TREE=/mnt/c/Users/hp/.t3/worktrees/CCF-HP/t3-8fc40b07-b22b8bb5869955c0
export GIT_CONFIG_COUNT=1 GIT_CONFIG_KEY_0=core.filemode GIT_CONFIG_VALUE_0=false
cd /mnt/c/Users/hp/.t3/worktrees/CCF-HP/t3-8fc40b07-b22b8bb5869955c0/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers
DOTNET=/home/jer/.dotnet/dotnet bash tests/ui-smoke-multiarea.sh
DOTNET=/home/jer/.dotnet/dotnet bash tests/ui-smoke.sh
# playtest / browser server: fresh OS-assigned loopback port per run
/home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
```

```powershell
# Windows node v26 + Microsoft Edge (headless, CDP); one fresh server per viewport
node Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/browser-driver-owner-ux/scenario-owner-ux.mjs <outDir> //wsl.localhost/Ubuntu/<file holding the base URL> 1280x720
node .../scenario-owner-ux.mjs <outDir> //wsl.localhost/Ubuntu/<file holding the base URL> 1920x1080
```

`build-p0-core-ref.sh` was **not** run. `.p0ref/` was copied from the reviewed candidate lane.

## 5. Results (observed by me, unless marked otherwise)

| Check | Base `e7c1a57d` (re-observed by me) | Final `9fed4eb9` |
|---|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln` | 0 warnings, 0 errors | **0 warnings, 0 errors** |
| Core suite | 184/184 | **184/184**, 0 failed |
| `dotnet build src/Ui/SitkaP1.Ui.csproj` | 0 / 0 | **0 / 0** |
| Anchor F-01 (Model 0 single area) | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | identical |
| Anchor F-02 (Model 0 MA-1) | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | identical |
| Anchor S4 V-02 (0/0/1 MA-1) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | identical |
| Anchor S4 R-02 (Stage 4 ten-year) | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | identical |
| P0 pin `.p0ref/SitkaLab.Core.dll` (before and after) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | identical |
| `ui-smoke-multiarea.sh` | 110 pass / 6 fail: J04, J05, J09, J10, J11, J99 | **118 pass / 6 fail**: the same J04, J05, J09, J10, J11, J99, plus **8 new F2a–F2h, all PASS** |
| `ui-smoke.sh` (single area) | 250 pass / 4 fail: L01, L02, L06, J31 | **250 pass / 4 fail**: the same L01, L02, L06, J31 |
| Real Edge, 1280×720 | n/a | **63/63 PASS** |
| Real Edge, 1920×1080 | n/a | **63/63 PASS** |
| Horizontal overflow (extra probe) | n/a | none at 1280×720, 1920×1080, 1024×768, 390×844 |
| CSP violations / console errors / exceptions / HTTP 5xx | n/a | 0 / 0 / 0 / 0 (both viewports) |
| Protected listeners 127.0.0.1:33767, :43649, :46593 | alive | alive, never bound |

**Smoke failure classification: all pre-existing, none caused by this change.** Multi-area: the page witness still reads #885-era elements that the page no longer has (`[data-competitor]`, `.competitor-rank`, `#reason-choice`); J04/J05/J09/J10/J11 fail on those selectors and J99 records the same crash, `byId('reason-choice').value = 'other'` (base: witness line 357; final: line 416, shifted only by the inserted F-2 block). Single area: files read by `ui-smoke.sh` are unchanged by this branch. The pass count rose 110 → 118 only because of the eight added F-2 checks; I numbered them F2a..F2h so the J numbering (and so the baseline failure IDs) did not move.

**Real-browser checks (each viewport, fresh session).** Light Guide OFF by default; one-line notice ≤ 40 px and Details with every caveat verbatim; compass 3×3, `− Fit +` row, `Area A | B` row, dock ≤ 170 px with 44 px targets; zoom/pan N/W/E/S/Fit/area switch; UX-01 fold with 0 and with 5 chosen; five rank buttons carrying the engine's top-five IDs in order; ranks 1→5 each toggling exactly its ID and the matching checkbox, re-click clears, inspector→map sync, Tab focus with a visible 3 px outline, Space toggles with focus kept, announced count, **server view unchanged by toggles** (only `hit`/`competition` reads sent); Add queued exactly the chosen IDs; Escape closes the Year+1 dialog only, the next Escape closes inspection and restores normal rendering with zoom/pan/area/Crop/orders/year kept; × Close does the same and the cleared draft does not return; light cells = exactly the cells with `L > 0`, all with opacity > 0, under the crowns, no text, monotonic over real cells; ramp values at L = 0 / 0.01 / 0.05 / 0.5 / 1 = opacity 0 / 0.0451 / 0.1284 / 0.5736 / 0.9 with `#ffd9bd` at 1; **pixels** over the floor colour: summed RGB change 0 / 8 / 26 / 227 / 496; crown click selects with the Light Guide on; rank toggles with it on; Inspect ground On state; a ground click on a crown reads that cell and does **not** select the tree; summary values equal the engine cell (index 3 dp, cover %, count); IDs and provenance behind closed disclosures; rank click in ground mode toggles and does not read ground; outside point says "no sample"; Escape stops ground mode first with the Light Guide still on; tree selection returns; no server side effect from Light/ground/inspection; Approve fells nothing; F-2 unavailable state (no legacy claims, Approve/Execute withdrawn), paired re-read restores Execute, an open confirmation is withdrawn unsent; real Execute fells exactly the confirmed trees; Year+1 advances. Full lines in `browser-driver-owner-ux/browser-run-*.txt`.

How the F-2 state was produced: the real server only sends `stage4: null` when another request moves the session between a command and its read-out (a race). Both tests therefore inject it: the smoke witness nulls `stage4` in the real server answer; the browser scenario wraps `window.fetch` to do the same for one `api/multi/view` answer. The 409 check uses a real stale fence against the real server.

**Reused evidence:** none. The base numbers in the table were re-observed by me on `e7c1a57d` in this worktree before any edit; they match the prior reported 184/184, 110/6 and 250/4.

## 6. Deviations, interpretations and limitations a reviewer should attack

- **D-1, UX-08 click precedence (interpretation).** The spec lists "modal > rank toggle > tree crown selection > ground inspection (only if ground mode is active)" and also "returning from ground inspection restores normal tree interaction". Area A is ~96% crown cover, so if crowns always won, ground mode would almost never reach the ground. I implemented: modal > rank button > (Inspect ground **on**: ground) / (Inspect ground **off**, Light Guide on or off: crown selection; ground is never inspected). Crown selection is therefore paused only while the user has explicitly switched Inspect ground on, and this is stated on screen. If the owner wants crowns to win even inside ground mode, it is a one-line change in `onMapClick`.
- **D-2, F-2 also withdraws Approve.** The spec names Execute. Without a read-out the pending Prune orders are unknown, so approving would approve orders the user cannot see; I disabled Approve too. An Execute confirmation open when an unpaired view arrives triggers the existing withdraw-and-lock path, so the whole page waits for a re-read.
- **D-3, inspector wording/layout.** Heading "Choose competitors for Fell" became "Competitors for Fell · N within 8 m"; the strength word moved under the Hegyi CI value (column header "Hegyi CI"); "Clear selection" became "Clear" with `aria-label` "Clear the Fell draft selection"; on short desktops (≤ 820 px high) the visible "n selected · draft only" line and the area tag next to "Reference tree" are hidden (the map subtitle and the pressed Area button name the area; the count is in the Add button and announced); a long status chip shortens with an ellipsis (full text in its tooltip and the Tree record); Age is in the Tree record only. All other information is still present below the fold.
- **D-4, map foot.** The foot no longer repeats "site UNKNOWN · growth and crowns use both areas; Work Plans and summaries per area" or the model 0 edge sentence; the full statements stay in the notice Details and the Details pane (AC07). The rack cards' "Model 1/1/1 · Synthetic stand · Light: not PAR" badges are **unchanged** (not in the UX list); the owner may consider them repeated warnings, which would be a follow-up.
- **D-5, Escape and search boxes.** A search box with text keeps the browser's own Escape (clear the field) and the inspection stays open in that key press. Not stated in the spec; chosen to avoid destroying typed text.
- **D-6, light mapping constants** (`LIGHT_EASE 0.65`, `LIGHT_ALPHA_MAX 0.9`, floor `0.0001`, level-word thresholds 0.1 / 0.35 / 0.65) are visual parameters, not ecology. The suggested `min(0.65, 0.65·L^1.2)` would give opacity ≈ 0.0026 at L = 0.01, which rounds to **no pixel change**; the ease 0.65 gives an 8-level summed change at 0.01 (measured). Below about L = 0.0007 the change rounds to zero pixels on an 8-bit screen although the written opacity stays positive ("potentially imperceptibly subtle", per spec §1). Cells are crisp 2.5 m squares (`shape-rendering: crispEdges`, no blur), so the layer looks blocky up close; blurring would spill light into L = 0 cells, which the spec forbids.
- **D-7, ground lookup.** The page matches a clicked point to the read-out's own clipped cell polygon (point-in-polygon), the same kind of screen→map conversion the page already does for hit-testing. It never interpolates or estimates a value.
- **D-8, rank buttons are positioned with CSSOM** (`element.style.left/top`), which the page CSP (`style-src 'self'`) allows; zero CSP reports in both runs. Positions are recomputed on every map render (zoom, pan, resize).
- **L-1, AC-9 screenshots.** All nine 1280×720 states are committed plus two 1920×1080 states (Light Guide Area B, ground inspection): 11 PNGs, 4,517,240 B, largest 762,902 B, within ≤ 0.9 MB/image and ≤ 5 MB/set. The other seven 1920×1080 PNGs (one is 1,184,051 B, over the per-image cap) are kept outside the repo with the raw logs at `/home/jer/.local/share/ccf-evidence/multi-stage4-owner-ux-20261123/` (`SHA256SUMS` there).
- **L-2, data reality.** At year 0 no cell has L exactly 0 (Area A: 144 cells, L 0.00067–0.22, median 0.0053; Area B: 239 cells, 0.0101–0.819, median 0.109). The L = 0 rule is verified through the ramp/pixel test and the "only L > 0 cells are drawn" check, not on a real zero cell.
- **L-3, the old witness drift is not fixed** (out of scope; pre-existing #885 selectors).
- **L-4, environment.** This worktree is a Windows checkout (system Git has `core.autocrlf=true`, no `.gitattributes`). Before any edit I re-materialised the tracked files with `core.autocrlf=false` (LF, byte-identical to the commit; otherwise the Core source-identity test failed on CRLF and the `.sh` smokes could not run), and used `core.filemode=false` per command. `.p0ref` was copied read-only, then made user-writable on `/mnt/c` because the read-only bit blocked MSBuild's copy; content and hash unchanged. Builds ran from WSL on `/mnt/c`.
- **L-5, coordination board.** At task start `origin/main:Docs/Project/coordination/active-tasks.md` (`55d9f67b`) still said "Phase A awaiting owner approval / no lane admitted"; the owner approval is recorded at `b95a56bf` and the writer grant came in the task packet. I did not edit the board.
- **Not done / not claimed:** Phase B (UX-10); F-7 stale documents; owner playtest; independent review; Unity, Windows, save/load; `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`; any `main` integration.

## 7. Screenshots (`evidence-owner-ux/`, real Edge, fresh session per viewport)

| File | State | Bytes | sha256 |
|---|---|---|---|
| `01-initial-map-1280x720.png` | Initial map, one-line notice, narrow dock, Light Guide off | 342,424 | `3265c715…` |
| `02-selected-five-ranks-1280x720.png` | Living tree selected: ranks 1–5 and Add above the fold | 342,735 | `5641e161…` |
| `03-ranks-chosen-synced-1280x720.png` | Map ranks 1, 2, 4, 5 chosen, matching inspector ticks | 345,991 | `47ba7f1c…` |
| `04-inspection-closed-1280x720.png` | After Escape: normal map, zoom and orders kept | 436,934 | `f8565494…` |
| `05-light-guide-area-a-1280x720.png` | Light Guide on, Area A (almost all very low light) | 328,254 | `8bc04947…` |
| `06-light-guide-area-b-1280x720.png` | Light Guide on, Area B (brighter real openings) | 313,703 | `87ac7abf…` |
| `06-light-guide-area-b-1920x1080.png` | Same at 1920×1080 | 762,902 | `75086cd3…` |
| `07-ground-inspection-1280x720.png` | Inspect ground on: concise summary, contributing trees expanded | 305,578 | `d48071be…` |
| `07-ground-inspection-1920x1080.png` | Same at 1920×1080 | 748,388 | `c2213e6b…` |
| `08-notice-details-1280x720.png` | Notice Details open with every caveat | 219,667 | `2696243c…` |
| `09-f2-readout-unavailable-1280x720.png` | F-2: unavailable read-out, Approve/Execute withdrawn | 370,664 | `cde1086f…` |

No capability URL is in any committed file. A fresh capability URL for the owner playtest must be generated at playtest time and given privately.

## 8. Owner playtest steps (expected behaviour)

1. Start: `/home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived` in `Tools/MultiRackSitkaP1Lab`, open the printed `/multi` URL at 1280×720.
2. Click any tree crown: ranks 1–5 and **Add selected to Fell plan** are visible in the right panel without scrolling.
3. Click map numbers 1, 3, 5: each turns orange with a tick, the matching rows tick, the button says "Add selected 3 to Fell plan". Click 3 again: it clears. Untick row 1: map 1 clears.
4. Press Escape (or **× Close inspection**): the map returns to normal; zoom and area stay.
5. **Light Guide: On**: soft rose-to-peach light appears on the floor under the crowns (Area A is almost all dark; Area B's open edges are pale peach). Clicking a crown still selects it.
6. **Inspect ground: On**, click a spot: a short summary (level, light index, canopy cover, shading trees). Escape stops ground mode; the Light Guide stays on.
7. Queue → Approve → Execute work as before.

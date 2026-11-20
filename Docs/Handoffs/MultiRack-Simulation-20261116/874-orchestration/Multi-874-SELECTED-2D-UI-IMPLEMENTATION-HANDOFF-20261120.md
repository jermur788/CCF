# Multi #874: selected 2D Stand Map UI, implementation handoff

**Date:** 2026-11-20
**Status:** `IMPLEMENTATION HANDOFF`. This is not independently reviewed, not integrated, not owner-accepted and not G2. Reference-image fidelity is `NOT VERIFIED`.
**Task:** [#874](https://github.com/jermur788/CCF/issues/874) · **Manager:** [#863](https://github.com/jermur788/CCF/issues/863) · **Spec:** [#868 approved four-file spec](https://github.com/jermur788/CCF/blob/docs/multi-868-selected-ui-fourfile-draft-spec-20261120-v1/specs/multi-standmap-2d-selected-ui-v1/spec.md)
**Dispatch packet:** [Multi-863-874 handoff](https://github.com/jermur788/CCF/blob/docs/multi-868-selected-ui-fourfile-draft-spec-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/874-orchestration/Multi-863-874-APPROVED-868-SELECTED-2D-UI-SINGLE-AGENT-IMPLEMENTATION-HANDOFF-20261120.md)

## 1. Candidate identity

| Item | Value |
|---|---|
| Branch | `feat/multi-868-selected-standmap-ui-v1` |
| Base (exact #849 source) | `4c42e2012a4c1bee3a422e41a58535343d63ac9f` |
| **Source candidate commit** | **`ccdb0b8af3ded00f2910a58296c5353884e75faa`** (one commit on the base) |
| Source diff against the base | Exactly 3 files: `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/index.html`, `app.css`, `app.js` (+645 / −226). No mode changes. |
| This handoff | One new Markdown file in a later commit on the same branch. It is not part of the source candidate. |

Nothing changed outside the allowlist: no Core, `PlayerApi.cs`, `Program.cs`, csproj, test, fixture, P0, README, spec, Unity or `Docs/Project` file.

## 2. Writer, lane and preflight

- **Writer:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`) in Claude Code, running in the T3 Code harness. Codex/Sol was not used. One writer, with no parallel source agents or helpers.
- **Tools actually used:** WSL2 bash, git, `gh` (actor `jermur788`), .NET SDK 8.0.134 (`~/.dotnet`), node v26 (WSL and Windows), and headless **Microsoft Edge 157.0.4341.60** driven over the DevTools protocol. No Unity, no Windows build and no main merge.
- **Host:** `DESKTOP-DL1BQI7`, confirmed by `hostname`, `/etc/hostname` and Windows `COMPUTERNAME`. WSL distro `Ubuntu`. Not jer-laptop.
- **Lane:** new worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-868-SelectedUI`, created with `git worktree add -b` at exactly `4c42e201`. The checkout was clean, and the branch existed neither locally nor on origin. No existing worktree was reused.
- **Coordination:** the live `origin/main` `active-tasks.md` had no #874 claimant and #874 had no comments. There is no `/home/jer/CCF-Ops` registry, so no physical lease or lock existed to take or respect. Registered RUNNING at [#874 comment](https://github.com/jermur788/CCF/issues/874#issuecomment-7042548045).
- **Overlap:** the only related process was the #867 launch-only owner-playtest server (PID 390578, its own detached worktree, `127.0.0.1`). It was never touched and was still running at handoff. This lane used its own build output and ephemeral ports.
- **P0, read-only:** the three `.p0ref` files (git-ignored) were copied read-only from `CCF-Multi-StandMap2D-Source/.p0ref`, as #867 did. The 4-way pin is equal at start and at the final candidate: DLL SHA-256, `.sha256` file, `provenance.txt dllSha256=` and `P0CoreExpectedSha256` all equal `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. P0 was **not** rebuilt; `build-p0-core-ref.sh` was not run.
- **Fixture:** `p1d-30m-sitka-adult-v1.sitkap1fx.json` SHA-256 `bcaaa5c3…f757c4`, unchanged. Every live session ran on a scratch copy under `/tmp`.
- `TMPDIR` and all evidence stayed outside the repository. `git status` was clean after every test run.

## 3. What changed (frontend only, real engine)

Every value still comes from the #849 server view and scene. The page sends the same API requests with the same fences. No new endpoint, contract, enum, save, money or species was added.

| Spec item | Implementation |
|---|---|
| **R-01 desktop composition** | **≥ 1024 CSS px:** the page fits one screen. A compact status rail (ecological year, living trees, basal area, stems/ha, Work Plan state) sits beside **Advance one ecological year** and **Re-read**. Below it the disclosure line and the four flags stay always visible. The map takes the remaining width. The right-hand side panel has four view buttons: **Plan** (selected tree + reason + Add, Work Plan with Approve/Cancel/Execute, latest receipt), **Trees** (full 100-row exact-ID table), **History**, and **Details** (full technical provenance: fixture SHA, content hash, model versions, revisions, session). Panes scroll inside themselves. **< 1024 CSS px** (360 and 640 px, and 200 % zoom of any window up to 2,047 px wide, e.g. 960 CSS px for 1920 px): one stacked column with vertical page scroll and every pane shown, as before #868. |
| **R-02 map** | The same north-up 1 m = 1 SVG unit scene, paint order, grid, north arrow and scale bar. The added decoration is a forest-floor texture, crown shadows, a radial crown shade, eight "needle spokes" and a leader dot per living crown, and growth rings on stumps. All of it is built from each item's own centre, radius and status, and it carries `pointer-events: none`. The **only** pointer target is still the server's true disc, which remains the tree group's first child, so the browser hit and the server hit-test agree (S-5 ambiguity check unchanged). Felled trees are still drawn at the engine's stump radius, so the real canopy gap opens. |
| **R-03 controls** | Seven text buttons replaced by a 3×3 D-pad (▲ ◀ ▶ ▼ around a decorative N) plus **+ / − / Fit**, each 46×46 px, with `aria-label` and `title`. They sit in a column **beside** the map, never over it, so at Fit they cannot cover a crown, a selection or the legend. Zoom, pan and Fit semantics are unchanged (the same functions and the same 64 px/m target). Desktop opens on Fit; the stacked layout keeps the earlier default (64 px/m below 900 px or at DPR ≥ 2). No drag or wheel was added. |
| **R-04 reason** | A required `<select>` with the placeholder "Choose a reason…" and the **five Manager-approved labels exactly**. A predefined choice is sent as its **exact label text** through the existing `api/pending/add` `reason` field. **Other — specify reason** reveals a required text box, and the trimmed text is sent. Placeholder, empty Other and whitespace-only Other are refused before sending, with an inline message, `aria-invalid` and focus on the field; the engine still enforces `WorkReasonRequired`. A "Will be recorded as: “…”" preview shows the exact text. After each successful Add the list returns to the placeholder, so there is no hidden default. The reason is shown in pending, approved, the Execute confirmation, the receipt, the inspector ("Recorded reason") and the History table. |
| **R-05 Work Plan / ecology** | Unchanged state machine and handlers: Add/Remove fell nothing, Approve fells nothing, Execute only from the frozen-copy confirmation (PlanId, key and fence), the re-read lock after unconfirmed outcomes, withdrawal of a stale confirmation, and Year+1 with a consent dialog when work is outstanding. Advance moved to the status rail and keeps its `#advance` id and behaviour. |
| **R-06 legend** | Two independent sections. **Tree species:** Sitka spruce (count of scene trees) with the note "the only species in this synthetic fixture". **Management / felling state:** Living, Pending Fell (dashed yellow ring + **P** badge), Approved Fell (double orange ring + **A** badge), Felled (stump + cross; gap open), and Natural Dead (hatched grey) **only when present**. Each row has glyph + text + colour and "(n on map)", a tally of the server's scene statuses. "Fell is the only management operation in this model"; no other operation type is shown. **Map marks:** selected-tree ring and the boundary. |
| **R-07 access / provenance** | Exact-ID quick select in the Plan view (Enter, case-sensitive). The table and its search are kept. Live regions, focus return and the skip link (which now opens the Trees view) are kept. Session-only / no save / money and comparison unavailable remain always visible. |

## 4. Tests on the exact candidate `ccdb0b8a`

### 4.1 Tier 1 (WSL, .NET 8.0.134)

| Gate | Result |
|---|---|
| `node --check app.js` | OK |
| `dotnet build src/Ui/SitkaP1.Ui.csproj` | 0 warnings, 0 errors |
| Core tests (`SitkaP1.Core.Tests.dll`, as the README documents) | **92/92 passed** |
| `tests/ui-smoke.sh` at the **base** `4c42e201` (same machine, before any edit) | 137/137 PASS |
| `tests/ui-smoke.sh` at **`ccdb0b8a`** | **136/137: one FAIL, L03** (§5). Every G guard (capability, Host, Origin, 405/413/415/400, CSP/no-store headers, served files byte-identical), S/engine cycle, hit-test oracle, L01/L02/L04/L05, **all J01–J31 page-script checks** (D1 confirmation withdrawal, D2 lost-answer lock, no request while locked, exact Execute payload), N01 loopback-only, Z SIGINT, and W01–W05 (nothing written; P0 digest, git status and fixture unchanged) pass. |

### 4.2 Real-browser witness (headless Edge 157, trusted input): **59/59 PASS**

The witness ran headless Microsoft Edge over the DevTools protocol against a fresh live session of the built candidate. Clicks and keys were sent with `Input.dispatchMouseEvent`, `dispatchKeyEvent` and `insertText`. The page recorded **73 of 73 click/key/change/submit events as `isTrusted: true`** (B40). This is **automation, not a human**: it proves the page responds to real browser input events, not that a person played it. The engine state after every step was checked through the server's own `api/view`. The capability URL was never printed or published.

Selected results (the full log is reproduced in §8):

- **1280×720, DPR 1:** no page scroll at all (scroll 1280×720). The map pane is 654×531 beside a 384 px side panel; the header is 107 px. Every shown control is inside the viewport and at least 44×44. With a tree selected and a reason chosen, the reason list and Add sit at 520–661 px with the Plan pane unscrolled (B01–B05b).
- **Mouse click on a drawn crown** selected exactly that tree through the server hit-test (P30-063, B04).
- **Reason:** placeholder refused; keyboard ArrowDown chose "Poor form or stem quality" and the engine recorded exactly that; the list then reset; Other with empty or whitespace text refused; a custom reason was recorded trimmed (B06–B17).
- **Keyboard exact-ID** select of P30-100 works; wrong-case `p30-001` is refused and the selection is unchanged (B11–B12).
- **Approve:** the engine minted a PlanId, both trees are still living, there is no work event (**0 Fell**), and the map shows A badges (B19–B20).
- **Execute confirmation** names the PlanId, the exact IDs and their reasons, and starts focus on Back. Execute: the engine felled **exactly** P30-063 and P30-100 in one event; living **100 → 98**; BA **55.87 → 55.05 m²/ha**. The map draws two stumps; Execute is not offered again (B21–B25).
- **Negative paths:** a stale fence gives 409 and a foreign Origin gives 403 (B26–B27).
- **Year+1:** the engine year went 0 → 1 with no outstanding work. With a pending order the consent dialog opens; **Keep** changes nothing and focus returns to Advance; **Cancel and advance** goes to year 2 with the order cancelled at zero Fell and the tree still living (B28–B32).
- **Views:** Trees, History and Details replace the Plan pane with `aria-pressed`. Table Select brings the Plan view back with focus on the selected tree (B33–B34).
- **Map controls by keyboard:** Tab reaches Zoom in after 7 presses; Enter zooms to 32 px/m and then 64 px/m (the smallest item is 44.8 px); Pan E/S scroll the pane, not the page; Fit returns to the whole area (B35–B39).
- **1920×1080:** no page scroll; the map pane grows to 1148×891 (B41).
- **Stacked layouts:** at 960×540 DPR 2, 640×360 DPR 2, 640 px and 360 px there is no horizontal page scroll, every shown control is inside the width and at least 44×44, the layout is one column, the view buttons are hidden and the side panel sits below the map (B42–B44).
- **640×360 DPR 2:** the Execute dialog fits the viewport; Escape closes it unsent, focus returns to Execute and the engine keeps the plan unexecuted (B45–B46).
- **No console error or uncaught exception** (B47). The strict CSP raised no violation.

**Screenshots** (22 PNG) are at `C:\Users\hp\AppData\Local\Temp\ccf874-cdp\final\` on `DESKTOP-DL1BQI7`. They were not committed, because the file allowlist permits only the three UI files plus this Markdown. A Manager can grant a reference-asset path if they should be archived.

## 5. The one failing check: L03, a requested test change

`L03 no @media rule touches the page layout (main, .layout, .side, panels), so no width or zoom level gives two columns` fails. The served `app.css` has one `@media (min-width: 1024px)` block that sets `.layout` to two columns and gives `.side`, `.map-panel` and `.pane .panel` their desktop sizing.

**Why this is not a regression I could avoid:** approved spec R-01 requires a dominant map with a right-hand Inspector and Work Plan at 1280×720. That is a two-column page at some width, which L03 forbids by construction. I did **not** rename classes to slip past the check, and I did **not** edit the test.

**Equivalent-or-better reachability evidence** for the protected intent ("200 % zoom must not produce a clipped multi-column page"):

- L01/L02 (one column at 360 and 960 CSS px) and L04/L05 (no core action needs sideways scroll at 360/960) still **pass**.
- The two-column rule applies only at a CSS viewport of 1024 px or more. Browser zoom at 200 % reaches that only on a window at least 2,048 px wide, where the viewport is genuinely desktop-sized.
- The real-browser checks B42–B46 measure no horizontal page scroll, every control inside the viewport width and at least 44×44, a stacked column, and the dialog fitting, at 960×540 DPR 2, 640×360 DPR 2, 640 and 360 px. B01–B05b measure no clipping at 1280×720.

**Request to Manager #863:** a narrow fourth-file grant for `Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh`, limited to:

1. Replace L03 with: "the only `@media` rule that touches the page layout is `(min-width: 1024px)`, and it applies at neither 360 nor 960 CSS px".
2. Add an L-check at 640 CSS px with the same one-column and reach conditions as L01/L04.
3. Keep L01, L02, L04 and L05 unchanged.

No J, G, S, N, Z or W check needs any change. Until that grant exists, the candidate's smoke status is honestly **136/137**.

## 6. Not verified and open gates

- **Reference-image fidelity: NOT VERIFIED.** Image #1 and image #3 were AI-generated concepts that are not frozen in GitHub. This work follows the approved **text** direction (dominant forest map, composed game-style hierarchy, richer two-part legend). No pixel or composition comparison was made or is claimed.
- **Literal browser zoom:** "200 %" was emulated as the equivalent CSS viewport and device pixel ratio (960×540 at DPR 2, 640×360 at DPR 2) through DevTools device metrics. The browser's own Ctrl + zoom control was not driven, and a real window was not resized.
- **Human input:** no person used this build. All input was trusted automation. The owner G2 playtest is still required.
- **Screen readers:** no screen-reader (NVDA/Narrator) run. Semantics (labels, live regions, `aria-pressed`, `aria-invalid`, focus return) were inspected in code and partly asserted in the witness.
- **Natural Dead:** no natural death occurred within the two years advanced in the final run, so its legend row and hatching were not exercised on the final candidate. An earlier run of the same page (before the final layout tweaks) did produce a natural death (P30-044), and the witness recorded the Natural Dead legend row as shown.
- **Contrast:** colours were chosen for AA body text on the dark panels. No automated contrast audit was run.
- **The T3 preview browser was not usable** for trusted input in this session: coordinate clicks dispatched nothing and selector clicks later timed out. That is why headless Edge was used. Nothing from the preview attempt counts as evidence.
- Not run, because no protected code changed and they are out of proportion for a UI-only change: `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`. P0 is covered by the 4-way pin and smoke W03.
- **Independent review (S7): not done.** The implementing thread cannot self-certify. Not integrated, no G4/main merge, no Windows/Unity build.

## 7. Reproduce

```bash
cd Tools/MultiRackSitkaP1Lab          # on feat/multi-868-selected-standmap-ui-v1 @ ccdb0b8a, pinned .p0ref present
export TMPDIR=/tmp/<outside-repo>
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
~/.dotnet/dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj && ~/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
NODE=$(command -v node) tests/ui-smoke.sh /tmp/<evidence>      # expect 136/137, L03 only
~/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll <scratch copy of fixtures/derived>   # open the printed 127.0.0.1 URL privately
```

The browser witness script (`drive.mjs`) is local scratch at `C:\Users\hp\AppData\Local\Temp\ccf874-cdp\` and is not committed. Run it as `node drive.mjs <capability URL> <out dir>` with Windows Node 22+ and Edge.

**Manual owner check at 1280×720:**

1. Click a crown and see it ringed in white.
2. Choose a reason and confirm "Will be recorded as" shows it.
3. Add the tree and see a P badge; the living count is unchanged.
4. Approve and see an A badge; still 0 felled.
5. Execute, check that the confirmation lists the ID and reason, then confirm. A stump appears and the living count and BA drop.
6. Press **Advance one ecological year** and the year goes up by one.
7. With an order pending, Advance asks for consent.

## 8. Final witness log (verbatim, capability redacted at source)

```
browser: Edg/157.0.4341.60 (headless), protocol 1.3
witness start 2026-11-20T15:16:35.679Z
server view: year 0, 100 trees, living 100, BA 55.87290918844456
PASS B01 1280x720: no page scroll at all (scroll 1280x720)
PASS B02 1280x720: every shown control inside the viewport and at least 44x44 px
PASS B03 1280x720: map pane 25,164,654,531 beside side panel 884,119,384,589
PASS B04 a mouse click on the drawn crown of P30-063 selected exactly that tree (server hit-test)
PASS B05 1280x720: the reason list (top 520) and Add button (bottom 638) are on screen without scrolling
PASS B06 Add with the placeholder reason is not sent; the alert asks for a reason and the engine has no pending order
PASS B07 keyboard ArrowDown on the reason list chose "Poor form or stem quality"
PASS B08 the page shows the exact text that will be recorded
PASS B05b 1280x720: with a reason chosen and its preview shown, Add (bottom 661) is still on screen with the Plan pane unscrolled
PASS B09 engine: P30-063 pending with reason "Poor form or stem quality"; living still 100 (Add fells nothing)
PASS B10 after a successful Add the reason list is back on the placeholder (no hidden default for the next tree)
PASS B11 typing the exact ID P30-100 and Enter selected it (keyboard, no pointer on the map)
PASS B12 a wrong-case ID is refused and the selection is unchanged
PASS B13 choosing "Other — specify reason" reveals the required text box
PASS B14 Other with an empty box is not sent
PASS B15 Other with only spaces is not sent
PASS B16 engine records the trimmed custom reason for P30-100
PASS B17 each pending order shows its exact ID and recorded reason
PASS B18 the map marks both pending trees with a dashed ring and a P badge
PASS B19 Approve: engine minted PlanId 19700ed7925164e7aa08a6298e099945:plan:3, both trees still living (100), no work event (0 Fell)
PASS B20 approved trees show the A badge and the approved list keeps each reason
PASS B21 the confirmation names the PlanId, the exact IDs with their reasons, and starts on Back
PASS B22 Execute: engine felled exactly P30-063,P30-100 in one event; living 100 → 98; BA 55.87 → 55.05
PASS B23 the map now draws two stumps with crosses at the felled positions
PASS B24 the status rail shows the engine living count
PASS B25 no approved plan remains, so Execute is not offered again
PASS B26 a command with the stale fence from the start of the session is refused (HTTP 409)
PASS B27 a POST naming a foreign Origin is refused by the page server (HTTP 403)
PASS B28 Advance: engine year 0 → 1; natural deaths this year: none
PASS B29 Advance with a pending order opens the consent dialog naming P30-099
PASS B30 "Keep orders" changes nothing: same year, order still pending
PASS B31 closing the consent dialog returns focus to Advance
PASS B32 consented Advance: year 2, the pending order was cancelled at zero Fell, P30-099 still living
note: natural deaths in the engine after two years: none (map Natural Dead legend row shown only if present: false)
PASS B33 the trees view button shows that pane in place of the Plan pane (aria-pressed true)
PASS B33 the history view button shows that pane in place of the Plan pane (aria-pressed true)
PASS B33 the details view button shows that pane in place of the Plan pane (aria-pressed true)
PASS B34 Select in the tree table brings back the Plan pane with focus on the selected tree
PASS B35 Zoom in is reachable with the Tab key (7 presses from the top)
PASS B36 Enter on Zoom in zooms the map (Scale 32 px per metre: the sma…)
PASS B37 a second zoom reaches 64 px/m, where the smallest drawn item is 44.8 px
PASS B38 Pan east and Pan south scroll the map pane (1454,246 → 1780,492), not the page
PASS B39 Fit returns to the whole area
PASS B40 every recorded click, key, change and submit event was isTrusted (73 events)
PASS B41 1920x1080: no page scroll; the map grows (25,164,654,531 → 25,164,1148,891)
PASS B42 200pct-of-1920x1080: no horizontal page scroll (scrollWidth 960 ≤ 960)
PASS B43 200pct-of-1920x1080: every shown control inside the viewport width and at least 44x44 px
PASS B44 200pct-of-1920x1080: one stacked column (grid 928px); view buttons hidden; side panel below the map; vertical page scroll 8911px
PASS B42 200pct-of-1280x720: no horizontal page scroll (scrollWidth 640 ≤ 640)
PASS B43 200pct-of-1280x720: every shown control inside the viewport width and at least 44x44 px
PASS B44 200pct-of-1280x720: one stacked column (grid 608px); view buttons hidden; side panel below the map; vertical page scroll 9582px
PASS B42 640px: no horizontal page scroll (scrollWidth 640 ≤ 640)
PASS B43 640px: every shown control inside the viewport width and at least 44x44 px
PASS B44 640px: one stacked column (grid 608px); view buttons hidden; side panel below the map; vertical page scroll 9846px
PASS B42 360px: no horizontal page scroll (scrollWidth 360 ≤ 360)
PASS B43 360px: every shown control inside the viewport width and at least 44x44 px
PASS B44 360px: one stacked column (grid 328px); view buttons hidden; side panel below the map; vertical page scroll 10939px
PASS B45 200% of 1280x720: the Execute dialog fits in the viewport (it scrolls inside itself: false)
PASS B46 Escape closes the dialog unsent, focus returns to Execute, and the engine still has the plan unexecuted
PASS B47 no console error or uncaught exception in the page
OVERALL: PASS (59 passed, 0 failed)
```

The B06 label is out of date. The missing-reason message is now inline under the field (`#reason-error`, with focus on the field) rather than in the page alert, and the check asserts the inline message and the focus.

## 9. Hand-back

- **Source:** implemented on `feat/multi-868-selected-standmap-ui-v1` @ `ccdb0b8a`, with this handoff committed on top.
- **Next gates, all open:**
  - Manager decision on the L03 test grant (§5).
  - Freezing the reference image and comparing against it.
  - Independent non-author review (S7).
  - Owner G2 playtest.
  - Any G4 / main decision.

# Multi #874: L03 narrow test repair, handoff

**Date:** 2026-11-20
**Status:** `TEST REPAIR HANDOFF`. This is not independently reviewed, not integrated, not owner-accepted and not G2. Visual-reference fidelity is `NOT VERIFIED`.
**Task:** [#874](https://github.com/jermur788/CCF/issues/874) · **Manager:** [#863](https://github.com/jermur788/CCF/issues/863)
**Authorisation:** [Manager #863 L03 narrow test-repair grant](https://github.com/jermur788/CCF/blob/docs/multi-863-874-l03-conditional-test-repair-authorisation-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/874-orchestration/Multi-863-874-L03-NARROW-TEST-REPAIR-AUTHORISATION-AND-REVIEW-GATES-20261120.md)
**Previous author handoff:** [Multi-874 implementation handoff](https://github.com/jermur788/CCF/blob/feat/multi-868-selected-standmap-ui-v1/Docs/Handoffs/MultiRack-Simulation-20261116/874-orchestration/Multi-874-SELECTED-2D-UI-IMPLEMENTATION-HANDOFF-20261120.md)

## 1. Identity

| Item | Value |
|---|---|
| Branch | `feat/multi-868-selected-standmap-ui-v1` |
| Base (#849) | `4c42e2012a4c1bee3a422e41a58535343d63ac9f` |
| UI source candidate (unchanged) | `ccdb0b8af3ded00f2910a58296c5353884e75faa` |
| Earlier handoff commit | `da133fe6c6b43aa063f607eb5c7b4ace843aecdd` |
| **Test-repair commit (new candidate)** | **`03547c6d56b6ba05742e71714d0cc2a4431ecf78`**. Changes only `Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh` (+68 / −9). The file mode stays `100755`. |
| This handoff | One new Markdown file in a later commit on the same branch. |

**Comparison against the base `4c42e201`:** exactly the three UI files (`index.html`, `app.css`, `app.js`, unchanged since `ccdb0b8a`), one test file (`tests/ui-smoke.sh`), and two handoff Markdowns. That is within the Manager's limit of 3 UI, 1 test and 2 Markdown files. No change to Core, API, fixtures, P0, Unity, specs or main.

## 2. Writer, lane and preflight

- **Writer:** Claude Opus 5.5 (`claude-opus-5-5`) in Claude Code (T3 harness). Same author as the #874 implementation; sole writer, no helpers.
- **Host:** `DESKTOP-DL1BQI7`, WSL Ubuntu. Same worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-868-SelectedUI`.
- **Before editing:**
  - The live `origin/main` board showed no other #874 claimant.
  - There is no `/home/jer/CCF-Ops` registry, so no physical lease or lock exists.
  - HEAD `da133fe6` equalled origin and the worktree was clean.
  - The only related process was the #867 owner-playtest server (PID 390578). It was not touched and was still running afterwards.
  - Registered at [#874 TEST REPAIR RUNNING](https://github.com/jermur788/CCF/issues/874#issuecomment-7043804451).
- **P0 4-way pin**, after the repair and all test runs: the DLL SHA-256, `.sha256` file, `provenance.txt dllSha256=` and `P0CoreExpectedSha256` all equal `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. P0 was not rebuilt.
- **Fixture** SHA-256 `bcaaa5c3…f757c4`, unchanged. Smoke W03 reports the P0 tracked-file digest unchanged.

## 3. What changed in `tests/ui-smoke.sh`

**Removed (exactly):**

- The header bullet that said "no media query that touches the page layout".
- The python `independent` mode.
- The old `L03` check line.

**Replaced `L03`** with the python `breakpoints` mode. The check is: *"the only @media query touching the page layout (main, .layout, .side, panels, or any element containing them) is exactly (min-width: 1024px), and it applies at none of 360, 640 or 960 CSS px"*.

- **"Touches the page layout"** keeps the #849 rule: the selector names `main`, `.layout`, `.side`, `.panel`, `.map-panel`, `.trees-panel` or `.history-panel`.
- It **adds** an element test: does the selector's last compound (pseudo-classes and attribute tests stripped) match any static element of the served page that *is or contains* `main` or a page part (`layout`, `side`, `panel`, `*-panel`, `pane`)? This covers `html`, `body`, `*`, `div`, an `#id`, and any renamed wrapper. A second breakpoint therefore cannot hide behind a new class name.
- This test is property-agnostic, like the old L03: any declaration on such a selector inside a media query counts.
- Media rules on other selectors stay legitimate. One example is the existing `.filters` rule at 600 px.
- The rule **fails** if any page-layout media query is not exactly `(min-width: 1024px)`, if there are no page-layout media rules at all, or if `(min-width: 1024px)` would apply at 360, 640 or 960.
- The shared parser is **unchanged**. It still rejects any non-`@media` at-rule and any media query other than min-/max-width. L01, L02 and L06 still reject any grid placement, float or multi-column property applied at their width.

**Added:**

- **L06:** 640 CSS px, the page grid is exactly one column. This reuses the unchanged `columns` mode.
- **L07:** 640 CSS px, no core action needs horizontal scrolling. This reuses the unchanged `reach` mode.
- **L08:** a 1280 CSS px structural check (new `desktop` mode). The last applied `.layout` grid has two tracks and comes from `(min-width: 1024px)`; `display` is `grid`; the `.view-tabs` display is `flex`; and `main`'s static children are the map panel followed by `#side`. This reads the stylesheet and page statically. It does **not** prove real browser rendering or zoom.

**Unchanged:** L01, L02, L04 and L05, and every G, V, S, R, A, H, P, E, Y, C, D, J, N, Z and W check. Comparing the results before and after shows the same 137 labels plus L06, L07 and L08. Every non-L label's text is identical, with volatile IDs and ports masked.

## 4. Negative evidence: scratch CSS mutations

The checker was extracted **verbatim** from the committed `ui-smoke.sh` (the heredoc between `<< 'PY'` and `PY`). It ran against scratch copies of the served `app.css` and `index.html` under `/tmp/ccf874/mutation/`. No tracked file was modified: `git status` showed only the intended test edit before the commit, and the tree was clean after it.

The committed CSS passes all eight modes: `columns` 360/640/960, `breakpoints`, `reach` 360/640/960 and `desktop` 1280. The table below was generated by running the checker at the time of writing.

| Scratch mutation (appended or edited) | New L03 | Other L checks |
|---|---|---|
| `@media (min-width: 900px) { .layout { grid-template-columns: minmax(0, 1fr) 20rem; } }` | **FAIL** | L02 (960): **FAIL** |
| `@media (min-width: 700px) { body { display: grid; grid-template-columns: 1fr 1fr; } }` (wrapper with no page-part name) | **FAIL** | L02 (960): pass |
| `@media (min-width: 700px) { * { float: left; } }` | **FAIL** | L02 (960): **FAIL** |
| `@media (max-width: 1023px) { .pane { display: none; } }` | **FAIL** | L02 (960): pass |
| `@media (min-width: 800px) { #side { display: flex; } }` (id, not class) | **FAIL** | L02 (960): pass |
| `@media (min-width: 700px) { .filters { gap: 2rem; } }` (benign, inside a panel) | pass | L02 (960): pass |
| M7: the approved block moved to `(min-width: 900px)` | **FAIL** | L02: **FAIL**; L08: **FAIL** |
| M8: the desktop rules made unconditional (no media query) | **FAIL** | L01 (360): **FAIL**; L06 (640): **FAIL** |

What the mutations show:

- **M2, M4 and M5 are caught only by the new L03.** The width-specific column checks do not see them. These are the cases where a page-layout breakpoint hides behind a wrapper, a `max-width` query or an id.
- **M6 is correctly not flagged.** The repaired check is not a blanket ban on media queries.
- For each failing case, the checker named the offending query: `(min-width: 900px)`, `(min-width: 700px)`, `(max-width: 1023px)`, `(min-width: 800px)`, and for M7 `(min-width: 900px)`.
- **M8** fails L03 because no page-layout media rule remains, and it fails L01 and L06 because two columns now apply at 360 and 640.

## 5. Results on commit `03547c6d`

| Gate | Result |
|---|---|
| `bash -n tests/ui-smoke.sh` | OK |
| `dotnet build src/Ui/SitkaP1.Ui.csproj` / Core tests build | 0 errors |
| Core tests (`SitkaP1.Core.Tests.dll`) | **92/92 passed** |
| **`tests/ui-smoke.sh`** (fresh scratch live server, pinned P0, scratch fixture copy) | **OVERALL: PASS, 140 passed, 0 failed** (was 136/137 with the obsolete L03) |
| P0 4-way pin, fixture SHA, `git status` after the runs | unchanged / unchanged / clean |

One mistake happened along the way. The first smoke attempt exited 126 because editing the file over the `\\wsl.localhost` share dropped its exec bit. The mode was restored to 755 before the commit, and the committed mode is `100755`, the same as the base.

Complete result list (capability redacted at source; no capability string appears in the log):

```
PASS S00 scratch fixture copy sha256 bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4 equals the committed fixture
PASS S01 printed URL is http://127.0.0.1:<port>/s/<64 hex>/ with port 41659 (not 5187, not privileged)
PASS S02 the only LISTEN socket of pid 810399 is [/proc/net/tcp 0100007F:A2BB] (got [/proc/net/tcp 0100007F:A2BB])
PASS G01 GET / without capability: 404, empty body: HTTP 404
PASS G02 wrong capability: 404, empty body: HTTP 404
PASS G03 63-character capability prefix: 404, empty body: HTTP 404
PASS G04 Host localhost:41659: 403, empty body: HTTP 403
PASS G05 DNS-rebinding Host: 403, empty body: HTTP 403
PASS G06 page with capability and exact Host: HTTP 200
PASS G07 page is text/html with the external script and style only
PASS G08 page answer: no CORS header, Cache-Control no-store, strict CSP, nosniff, no-referrer, DENY framing, no Server
PASS G09 /s/<capability> without slash: 308 to /s/<capability>/ (got 308)
PASS G10 app.css: 200 text/css
PASS G11 app.js: 200 text/javascript
PASS G11b the page, app.css and app.js answers are byte-identical to the committed src/Ui/wwwroot files
PASS L01 at 360 CSS px the page grid is exactly one column (one track, display grid, no column placement or multi-column property)
PASS L02 at 960 CSS px (200 % zoom of a 1920 px window) the page grid is exactly one column
PASS L03 the only @media query touching the page layout (main, .layout, .side, panels, or any element containing them) is exactly (min-width: 1024px), and it applies at none o
PASS L04 at 360 CSS px no core action needs horizontal scrolling: none is inside a sideways scroller, Select is the table's first column, action groups wrap, no fixed width ov
PASS L05 at 960 CSS px no core action needs horizontal scrolling (same conditions as L04)
PASS L06 at 640 CSS px (200 % zoom of a 1280 px window) the page grid is exactly one column
PASS L07 at 640 CSS px no core action needs horizontal scrolling (same conditions as L04)
PASS L08 at 1280 CSS px (min-width: 1024px) gives the approved desktop page: two grid tracks, the map panel then the side panel, view buttons shown
PASS G12 GET naming a foreign Origin: 403: HTTP 403
PASS G13 GET on a POST endpoint: 405, Allow: POST (got 405)
PASS G14 unknown endpoint under the capability: 404: HTTP 404
PASS V00 GET api/view: HTTP 200
PASS V01 view: 100 exact fixture IDs, 100 scene items, year 0, annual 0, work 0, plan-state 0, nothing pending or approved
PASS V02 labels come from Core: G1 disabled, money unavailable (not 0), session only
PASS V03 every frame tree and scene item carries the JSON names the page reads (xm, zm, crownRadiusM, u, v, radiusM, paintOrder)
PASS V04 the scene has exactly the frame's exact IDs, in paint order 0..99
PASS G15 POST without Origin: 403: HTTP 403
PASS G16 POST with Origin http://localhost:41659: 403: HTTP 403
PASS G17 POST text/plain: 415: HTTP 415
PASS G18 POST form-urlencoded: 415: HTTP 415
PASS G19 POST body over 16 KiB: 413: HTTP 413
PASS G20 malformed JSON: 400: HTTP 400
PASS G21 a caller-built plan object (unknown member): 400: HTTP 400
PASS G22 JSON null body: 400: HTTP 400
PASS G23 after every guard refusal the view is unchanged (plan-state 0, nothing pending, work 0)
PASS R01 Add with a wrong-case ID: HTTP 409 UnknownTreeId, fresh view attached
PASS R02 Add with a blank reason: HTTP 409 WorkReasonRequired, fresh view attached
PASS R03 Add with another session's fence: HTTP 409 PlayerSessionMismatch, fresh view attached
PASS R04 Approve without a fence: HTTP 409 PlayerSessionMismatch, fresh view attached
PASS R05 Approve with nothing pending: HTTP 409 WorkEmptyPlan, fresh view attached
PASS R06 Execute without an approved plan: HTTP 409 WorkPlanNotApproved, fresh view attached
PASS R07 after the refusals: plan-state 0, nothing pending, 100 living
PASS A01 Add P30-001 with a reason: HTTP 200
PASS A02 Add: pending [P30-001], plan-state 1, work 0, 100 living, year 0, no event (no tree felled)
PASS A03 the scene marks P30-001 PendingFell
PASS A04 Add with the pre-Add fence: HTTP 409 PlayerPlanStateStale, fresh view attached
PASS A05 the stale Add changed nothing (pending [P30-001], plan-state 1)
PASS H01 hit-test at the centre of P30-001 (1.069000244140625, 28.092000007629395): HTTP 200
PASS H02 candidates include P30-001, topmost first, and every candidate disc contains the point
PASS H03 hit-test on bare ground outside every crown: 200 and no candidate (got 200)
PASS H04 hit-test with a stale fence: HTTP 409 PlayerPlanStateStale, fresh view attached
PASS H05 the stale hit-test returned no candidate
PASS H06 hit-test without u and v: 400 (a missing point is never read as 0): HTTP 400
PASS H07 hit-test at the most-overlapped grid point (2.75, 19.25): 200, candidates exactly ["P30-042", "P30-022", "P30-041", "P30-032", "P30-031"], topmost first (got 200)
PASS H08 that point is a real overlap: 5 crowns (at least 2)
PASS H09 hit-test on bare ground inside the square (29.75, 30.0; no crown covers it): 200 and no candidate, so the nearest tree is not chosen (got 200)
PASS P01 Approve the whole Work Plan: HTTP 200
PASS P02 engine-minted PlanId 871a93c231d7456e5d929928a51fda56:plan:2 = <session>:plan:2 and key = <PlanId>:execute
PASS P03 Approve fells nothing: 100 living, work 0, no event, pending moved into the plan, plan-state 2
PASS E00 Execute the approved plan under an unminted key (the key plan 12 will be minted with): HTTP 409 WorkExecutionKeyConflict, fresh view attached
PASS E00b the refused Execute changed nothing (plan 871a93c231d7456e5d929928a51fda56:plan:2 still approved, plan-state 2, work 0, 100 living)
PASS E01 Execute the approved plan: HTTP 200
PASS E02 one typed Fell of exactly [P30-001] (fell / manual-work-plan), not a replay
PASS E03 same year 0 and annual revision 0; work revision 0 -> 1; 99 living; plan-state 3; approval cleared
PASS E04 the work event: living 100 -> 99 and basal area falls; the frame stand equals the event's after-stand
PASS E05 P30-001 is drawn as a Felled stump (radius 0.35 m) with death year 0, cause fell
PASS E06 the same Execute again with the original fence (a retry): HTTP 200
PASS E07 replay: replayed=true and nothing changed (work 1, 99 living, one event, plan-state 3)
PASS E08 the same key with another PlanId: HTTP 409 WorkExecutionKeyConflict, fresh view attached
PASS E09 Add of the felled tree P30-001: HTTP 409 UnknownTreeId, fresh view attached
PASS Y01 Advance with the pre-Execute fence: HTTP 409 PlayerSourceStale, fresh view attached
PASS Y02 the refused Advance changed nothing (year 0, annual revision 0)
PASS Y03 re-read for a fresh fence: HTTP 200
PASS Y04 Advance with the fresh fence: HTTP 200
PASS Y05 one genuine year 0 -> 1 (annual revision 1); P30-001 stays dead with its Fell cause; plan-state unchanged (3)
PASS Y06 history: one AnnualYearRecord 0 -> 1 without cancelled work; the year started from 99 living
PASS C01 Add P30-002 in Year 1: HTTP 200
PASS C02 Advance with unexecuted work and no consent: HTTP 409 WorkPendingConsentRequired, fresh view attached
PASS C03 the refused Advance changed nothing (year 1, pending [P30-002])
PASS C04 Advance with consent to cancel the unexecuted work: HTTP 200
PASS C05 consent: Year 2, nothing pending, still one work event (zero Fell), P30-002 not felled, plan-state 5, record marks the cancellation
PASS D01 Year 2: Add P30-004: HTTP 200
PASS D01b Year 2: Add P30-005: HTTP 200
PASS D01c Year 2: Remove P30-005: HTTP 200
PASS D01d Remove: pending back to [P30-004], plan-state 8 (5, then add, add, remove), work 1, still one event (no Fell)
PASS D01e Remove of an ID that is no longer pending: HTTP 409 UnknownWorkOrder, fresh view attached
PASS D02a Year 2: Approve: HTTP 200
PASS D02b Year 2: Cancel the approved plan: HTTP 200
PASS D02c Cancel: no approved plan, its orders not returned to pending, plan-state 10, work 1, still one event (no Fell)
PASS D02d Execute of the cancelled plan: HTTP 409 WorkPlanNotApproved, fresh view attached
PASS D02e Year 2: re-add P30-004: HTTP 200
PASS D02 Year 2: Approve: HTTP 200
PASS D02f this plan is minted with the key that E00 tried to take: 871a93c231d7456e5d929928a51fda56:plan:12:execute
PASS D03 Year 2: Execute under its own minted key (not blocked by E00): HTTP 200
PASS D04 Year 2 Fell of [P30-004] in year 2; two work events; work revision 2
PASS J01 the served page starts with one GET api/view and lists 100 trees (year 2); trees used: P30-098, P30-099, P30-100
PASS J02 one-column default: the map is drawn at 64 px per metre and states that the smallest drawn item (a stump) is 44.8 px across
PASS J03 with a re-read still pending, the Execute confirmation shows plan A only (P30-098) and names its PlanId
PASS J04 a one-tree confirmation is worded in the singular (D5)
PASS J05 a second client of the same session cancelled plan A and approved plan B (P30-099) while the dialog was open
PASS J06 Confirm while the re-read is still pending sends nothing and leaves the dialog as it is
PASS J07 when the view with plan B arrives, the plan-A confirmation is withdrawn and the alert names plan A
PASS J08 no Execute request was sent for either plan
PASS J09 every engine command is disabled until a re-read; Re-read stays available
PASS J10 clicking Execute or the withdrawn dialog's Execute button sends nothing and opens nothing
PASS J11 on the server, A and B are both still living, no work was executed and plan B is still approved
PASS J12 a successful re-read lifts the lock and clears the alert
PASS J13 the fresh confirmation shows plan B only (P30-099)
PASS J14 the Execute request carries exactly the confirmed plan B: its PlanId, minted key and fence
PASS J15 the server felled exactly P30-099 (plan B) in one event; P30-098 is still living
PASS J16 the server did commit the Add (P30-100 pending, plan-state 20)
PASS J17 the page drew nothing it was not told: P30-100 not shown as pending, plan-state still shown as 19
PASS J18 every engine command control is disabled (Add, Remove, Approve, Cancel, Execute, its confirmation, Advance, consent)
PASS J19 Re-read is the available action (status bar and alert) and focus is in the alert
PASS J20 while the outcome is unconfirmed, activating every engine command handler (Add, Approve, Cancel, Execute, its confirmation, Advance, consent, Remove) and a map hit-te
PASS J21 no dialog opened, and the alert says what was not sent
PASS J22 a failed re-read says so and leaves the lock in place: Advance is still not sent, nothing but reads can be
PASS J23 a successful re-read shows the committed Add, lifts the lock and clears the alert
PASS J24 after the re-read a command is sent again (Approve)
PASS J25 the server did commit the Execute (P30-100 felled, approval cleared)
PASS J26 the page still shows the plan as it last knew it, claims no Fell, and every command is disabled
PASS J27 neither a retry nor any other command can be sent
PASS J28 the re-read shows the receipt and no approved plan, so nothing is left to execute twice
PASS J29 the server recorded exactly two work events from the page (plans B and C)
PASS J30 no script error in the page
PASS J31 the page witness ran to the end (exit 0) with nothing on stderr
PASS N01 every TCP socket of the process is on 127.0.0.1 (socket count at this moment: 1)
PASS Z01 SIGINT ends the process with exit 0 and 'Session ended; nothing was saved.' (exit 0)
PASS Z02 the server wrote nothing to stderr (no unconfirmed outcome, no fault)
PASS Z03 the port no longer answers after exit (curl status 000)
PASS W01 no file created, changed or removed anywhere in the lab tree (170 entries, bin/obj/.p0ref included)
PASS W02 ~/.aspnet unchanged (absent before and after)
PASS W03 P0 tracked-file digest unchanged (9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8)
PASS W04 git status of both labs unchanged
PASS W05 committed fixture sha256 unchanged
OVERALL: PASS (140 passed, 0 failed)
```

## 6. Security and accessibility assertions: intact

- **Guards:** G01–G23 (capability, Host, DNS rebinding, Origin, methods, media types, size, malformed and unknown JSON, CSP/no-store/nosniff/no-referrer/DENY headers, served files byte-identical to source).
- **Engine and API:** V, R, A, H, P, E, Y, C and D, including the hit-test oracle, stale fences, approve at zero Fell, Execute once with replay and key conflict, and consent.
- **Page script:** J01–J31 (the confirmation withdrawal and the re-read lock after an unconfirmed outcome).
- **Loopback and shutdown:** N01 and Z01–Z03.
- **Nothing written:** W01–W05.
- All of these are unchanged and pass.
- **Accessibility layout checks:** L01, L02, L04 and L05 are unchanged and pass. L06 and L07 extend the same one-column and reach conditions to 640 CSS px.

## 7. Limitations and verification still required

- L03, L06, L07 and L08 are a **static reading** of the stylesheet and page, not a layout engine. They do not prove that real browser zoom works. Real-browser evidence remains the author's earlier headless-Edge witness (59/59, trusted CDP input, emulated 200 % zoom). That witness was **not re-run** here, because no UI file changed.
- The element-containment test sees only **static** elements in `index.html`. Elements that `app.js` creates at runtime are not considered; none of them is a page-level container today. Selectors are judged by their last compound only, so the test is conservative and can over-match.
- **Still open:**
  - Independent non-author review of the UI source plus this test change. This thread cannot provide it.
  - Freezing the reference image and checking visual fidelity: **NOT VERIFIED**. The local screenshots in `C:\Users\hp\AppData\Local\Temp\ccf874-cdp\final\` are not archived in GitHub.
  - Literal Ctrl+zoom, a screen-reader run and the owner G2 playtest.
  - Any G4 or main decision. No integration is claimed.

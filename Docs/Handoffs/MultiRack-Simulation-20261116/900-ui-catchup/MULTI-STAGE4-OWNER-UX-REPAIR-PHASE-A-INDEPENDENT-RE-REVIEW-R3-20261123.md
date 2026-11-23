# MULTI Stage 4 owner UX repair, Phase A: independent re-review of R3 (candidate 0e777455)

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Reviewer | Claude Sonnet 5.5, independent reviewer; the same reviewer who issued NEEDS CHANGES on `5f3b5bf3`. Not the author of the code |
| Repository / branch | `jermur788/CCF`, `feat/multi-stage4-owner-ux-20261123-v1` |
| **Reviewed HEAD** | **`0e7774557dbb2947b29993cf032c084aa3b7227f`**. Equal to `origin/feat/multi-stage4-owner-ux-20261123-v1` at the start, in the middle and at the end of this review; the branch did not move |
| PREV | `5f3b5bf3c9f711125fb61ddee55de1ff9690c19b`, the commit I reviewed before. Confirmed an ancestor of HEAD |
| BASE | `e7c1a57da519ed1ee03ccfbad240196a66df0905`, confirmed an ancestor of HEAD |
| Commits reviewed (R3) | `9d40b166` page, `9abf5c3e` smoke R3a–R3j, `4e3ba42b` driver and loaded sweep, `0e777455` evidence and handoff. Twelve commits BASE..HEAD, linear, no merges |
| Authority | `specs/multi-stage4-owner-ux-20261123-v1/spec.md` at `7b06da6f4e860fe71cd126c617ad1329e18b86b6`, section 0.2 (the owner clarifications on the minimum width and on F-03). That commit is not in the candidate tree; I read it from its own commit. See §8 for a later spec commit that I did **not** use |
| Handoff treated as a claim | `MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-IMPLEMENTATION-RESULT-20261123.md` at HEAD |
| This document | New file on my own docs branch `docs/multi-stage4-owner-ux-independent-re-review-r3-20261123-v1`, cut from the reviewed SHA. **Not** on the candidate branch |
| **VERDICT** | **APPROVED** (review verdict only) |

This is a review verdict. It is **not** owner acceptance and **not** integration. Nothing was merged. The candidate branch was not checked out, modified, rebased, amended or pushed to.

---

## 1. Verdict in one page

**APPROVED.** The one MAJOR finding from my previous review is resolved, I found no new MAJOR or BLOCKING issue, and what I attacked in the R3 diff held.

**Is AC01 satisfied at the documented minimum width in a loaded session? Yes, with one precise qualification.**

* **Yes**, at a **1280×720 CSS-pixel viewport** (the page area; this is how the owner clarification, the spec, the writer's driver and both of my probes measure it). I measured every living tree of both areas, selected by exact ID with a real Enter key, in **five session states** (clean; 4 queued Fell orders; approved plan plus Crop designations; after a real Execute; after Year+1). **0 failures in 1,209 tree measurements per viewport**, every tree genuinely measured, 22 to 61 trees per state with blocked competitor rows, and one tree with all five rows blocked. Row height is **38 px for every row, blocked or free**. At 1920×1080 the same: 0 failures, 50.2 px rows.
* **The qualification:** at a 1280-wide viewport the fold holds only down to about **697 px of height** (§4.6). A literal 1280×720 browser **window** has a viewport roughly 100 px shorter once tabs and the address bar are counted, and there the fold fails (at a 1280×610 viewport 53 of 87 sampled rows are reachable). The owner says no fold requirement applies below 1280×720, so this is not an AC01 failure. But the in-page note says "browser **windows** of at least 1280 × 720", which promises more than the page delivers (N-01).

What I verified with my own runs, not the writer's numbers:

* **F-01 is genuinely fixed, and my sweep can tell.** The same sweep against the unfixed `5f3b5bf3` page fails **all 22 of the 22** trees that have a blocked row (rows 64.8 px against 38 px); on HEAD it passes.
* **The reason is reachable without sight**, not merely hidden: full text in the row tooltip, in screen-reader text inside the label, and in the checkbox's accessible name as computed by Edge; the visible short form is `aria-hidden`.
* **The post-Add card no longer covers the rows** at 1280×720 (one line, 36.4 px, all five rows above it), including after a **real partial Add, a guard refusal and an unconfirmed outcome**, which the writer did not provoke in a browser.
* **The stale-answer matrix still holds** after the `onEscape` reorder: 16 of 16 cases per viewport dropped whole, the non-stale controls still behave, and the new dialog-only-Escape cases behave as the writer intended.
* **F-07 is fixed on both paths** (summary and announcement).
* **Builds 0/0, Core 184/184, all four anchors and the P0 pin unchanged.** The 6 + 4 smoke failures are byte-identical to BASE.

What the owner should know (none blocks):

* **Fold headroom at 1280×720 is about 23 px of viewport height** (§4.6): adequate for the stated size, thin as a design margin.
* **At 1280×720 the one-line card title is cut** (`Stopped after 1 …`). The count and the refused tree are still on screen in the map header's status line, and in the tooltip and under Details (N-02).
* **Both behaviour changes you asked me to judge are acceptable, MINOR, and not a trap** (§3).
* I found and fixed **several defects in my own probe** during this review (§6.2). None changed a conclusion, but two of them first looked like page failures, so I list them all.

---

## 2. Findings

### 2.1 Previous findings

| ID | Was | Status now | Evidence in one line |
|---|---|---|---|
| F-01 | MAJOR | **RESOLVED** | 0 failures in 1,209 measurements per viewport over 5 states; 38 px parity; the unfixed page fails 22/22; reason reachable without sight (§4.1) |
| F-02 | MINOR | **RESOLVED** (see N-02, N-03) | The card replaces the form; at 1280×720 one line, 36.4 px, five rows above; holds for complete, partial, guard and unconfirmed Adds (§4.2) |
| F-03 | MINOR | **WON'T FIX** (owner decision, spec 0.2) | Left unchanged as directed; re-measured, same behaviour (§4.8) |
| F-04 | MINOR | **RESOLVED** (see N-01, N-07) | 1280×720 documented in the page's HTML comment, its CSS comment and a visible Details-pane note; the fold holds there (§4.3) |
| F-05 | OBSERVATION | **RESOLVED** | A dialog-only Escape no longer cancels a pending click; every other Escape still does (§4.4) |
| F-06 | OBSERVATION | **STILL OPEN** (observation, no action needed) | Unchanged. Fail-closed and self-heal re-verified (§4.4) |
| F-07 | OBSERVATION | **RESOLVED** (see N-06) | Every positive index below 0.0005 reads "< 0.001" in the summary and the announcement; zero still reads "0.000" with the no-light wording (§4.5) |
| F-08 | OBSERVATION | **STILL OPEN** (disclosed limit, D-6) | Not touched by R3; the ramp numbers are identical to R2 (§5) |
| F-09 | OBSERVATION | **RESOLVED** (improved; adequate but thin) | Free room below the dock 8.4 → 18.0 px; the number that governs is that plus the dock margin, about 13 → 23 px (§4.6) |
| F-10 | OBSERVATION | **STILL OPEN** (pre-existing, not from this lane) | Same 6 + 4 witness failures; the J99 crash still stops later witness checks. R3a–R3j sit above it and run (§5) |
| F-11 | OBSERVATION | **RESOLVED** | The header row "Authorship (all commits)" covers every commit; I checked all twelve (§4.7) |

### 2.2 New observations from this review

| ID | Severity | Observation |
|---|---|---|
| N-01 | MINOR | The in-page note and the page comments say "browser **windows** of at least 1280 × 720". The fold is measured on the **viewport**. A 1280×720 window leaves roughly 610 px of page height, where the fold fails. The guarantee holds only down to a viewport height of about 697 px (§4.6). Suggest "page area (viewport)" instead of "browser windows". The owner's number is unchanged |
| N-02 | MINOR | At 1280×720 the one-line card title is cut by an ellipsis for failure titles and for the success title: `Stopped after 1 …` hides "of 3 · refused MA-A-005"; `Added 2 trees to …`. Mitigation, which I measured: the status line in the map header is visible and carries it (`Stopped after 1 of 3: refused MA-A-016. Added: MA-A-015. Not attempted: MA-A-002.`) until the next announcement, and the full title is in the tooltip and under Details. The two new controls are 63×28 px (Dismiss) and 46×28 px (Details): above the 24 px WCAG 2.2 AA minimum, below the page's own 44 px convention |
| N-03 | OBSERVATION | With Details **open** at 1280×720 the card is 83.7 px and the sticky dock covers the lower 11 px of row 5, which cuts its second text line; the checkbox stays reachable. On request only, and Hide details restores the fold |
| N-04 | OBSERVATION | At 1280×720 the DBH / CI-share tail of a blocked row is cut by the ellipsis whenever the reason is "Approved plan" or "Retained Crop": 81 of 81 such rows in the approved-plus-Crop state, for example `29.6 cm · 1…`, `24.8 cm · 8.…`, `20.9 cm · 7.2…`. "Pending Fell" rows are not cut (0 of 34). The DBH number was always visible (31 of 31 rows sampled); the full text is in the tooltip and the accessible name. At 1920×1080 nothing is cut. The writer disclosed that a tail "can" be cut; the extent is every Crop and approved-plan row |
| N-05 | OBSERVATION (pre-existing) | `#add-caption { display: none; }` in the short-desktop block, present since BASE (`e7c1a57d`, line 1022). At 1280×720 a disabled Add button therefore shows no reason ("The plan is approved, so editing is locked…", "Tick one or more competitors to add."). While the post-Add card is up the caption is also hidden at any height. Not introduced by R3 |
| N-06 | OBSERVATION | Technical details keep four decimals, so a positive index below 0.00005 still reads `0.0000` **there** (the concise summary is fixed). Not reachable with real data: the smallest real L over years 0–3 is 0.00066555 |
| N-07 | OBSERVATION | The screen-size note sits at 97% of the Details pane (3,365 of 3,479 px down at 1280×720). Documented, but you have to scroll to the very end to find it |

No BLOCKING or MAJOR finding. To be plain about the one that touches the owner's rule: **the owner's rule is "1280x720", and the page meets it as a viewport.** N-01 is about how the rule is worded to a player, not about whether the rule is met.

---

## 3. The two behaviour changes, judged

Both are disclosed by the writer, both follow from not letting the sticky dock grow, and I tested both with a real mouse and keyboard, including real failures.

### 3.1 "Any draft change now ends the Add result card, at every size": **MINOR, acceptable, not a trap**

* **Verified:** ticking a map rank after an Add ends the card and returns the form with the new count (`Add selected 1 to Fell plan`); ticking an inspector checkbox does the same; both at 1280×720 and 1920×1080.
* **Why acceptable:** after a **complete** Add nothing is lost; the new orders are in the Work Plan and the card was only a confirmation. It is also what makes change 2 harmless, because any tick or untick is a way back to the form.
* **The residual cost, and why it stays MINOR:** after a **partial or failed** Add the card is the persistent place that shows the refusal code and the recovery text. A plain refusal raises **no alert** (asserted at both sizes: alert region empty), so the card and the status line carry it. Any tick or untick erases the card, and the status line is replaced by "Ticked …". What the player keeps is the important part: the unsent choices stay ticked and nothing is rolled back. So a player loses the **explanation**, not the **state**, and a repeat refusal would show it again.
* **Change from before at tall sizes:** at 1920×1080 the card used to persist while the player kept choosing; it now disappears on the first tick.
* **If the owner wants it softer (not required):** end the card on a new Add, a new reference or Dismiss, but not on a draft change, and let the tall layout keep it. At the short height there is no room, which is the reason for the change.

### 3.2 "After a partial Add the Add control stays hidden until Dismiss or a draft change": **MINOR, acceptable, not a trap**

* **What I provoked for real** (the writer did not): a partial Add of three trees where the server fence moved so call 2 was refused; a guard refusal at the first order; a transport failure at the second order (unconfirmed, commands locked until a re-read).
* **Every exit works.** Partial: the card read `Stopped after 1 of 3 · refused MA-A-005`, the two unsent choices stayed ticked, **Enter** on Dismiss returned the form as `Add selected 2 to Fell plan` with focus on the Add button, and the retry sent only those two: all three chosen trees ended **pending exactly once** (`[1,1,1]`), so no duplicate orders. Guard: Dismiss returned the form with both choices intact and the retry succeeded. Unconfirmed: after the re-read and Dismiss the form came back. A tick or untick is the second way out. Keyboard order after the Add is card → Details → Dismiss.
* **Residual hazard (the reason it is not NONE): discoverability at the minimum size.** The card does not say "Dismiss to retry", the title is cut (N-02), and the caption that would explain an unavailable Add is hidden at that height (N-05). A player who does not read the card sees the Add button vanish. A player who then selects another tree **loses the unsent draft** (the page announces "Cleared N unsent draft choices …"), which is two clicks to redo and nothing worse. The ways out are one click or one tick away and visible, so I do not call it a trap.

---

## 4. Evidence, item by item

All figures are from my own runs on the final probe unless stated (provenance in §6).

### 4.1 F-01: the loaded-state fold sweep

Every living tree, selected by exact ID, Enter pressed for real, no scrolling. A tree **passes** only if the reference ID and `× Close inspection` are reachable, all of its rows are inside the pane and above the dock and reachable at their centre, the rendered Add button is reachable and inside the pane, and the inspector `scrollTop` is 0.

| State | Trees | Trees with blocked rows (rows) | Blocked rows per tree (n: trees) | Reasons seen | 1280×720 failing | 1920×1080 failing |
|---|---|---|---|---|---|---|
| clean | 244 | 0 | none | none | **0** | **0** |
| 4 queued Fell orders (real mouse on map ranks, then the page's Add) | 244 | 22 (34 rows) | 1: 12, 2: 9, 4: 1 | Pending Fell | **0** | **0** |
| plan approved + 5 Crop designations (the page's own Crop and Approve buttons) | 244 | 61 (81 rows) | 1: 44, 2: 16, **5: 1** | Retained Crop 47, Approved plan 34 | **0** | **0** |
| after a real Execute (4 trees felled) | 240 | 43 (47 rows) | 1: 39, 2: 4 | Retained Crop | **0** | **0** |
| after Year+1 (3 more trees gone) | 237 | 43 (47 rows) | 1: 39, 2: 4 | Retained Crop | **0** | **0** |

* **Every tree was measured.** 0 unmeasured in every state (§6.2 explains the probe defect that made this guard necessary). Seven trees genuinely have fewer than five competitors (for example MA-B-073 has 2) and are not padded.
* **Row heights:** 38 px (1280×720) and 50.2 px (1920×1080) for **every** row of every tree, blocked or free. Parity failures: 0.
* **A tree with all five rows blocked passes** (MA-A-042 in the approved-plus-Crop state: 38 px rows, fold intact; 50.2 px at 1920×1080).
* **The Add button, ticked.** On the six tightest trees of each state, with every free rank ticked by the real mouse ("Add selected 5 to Fell plan" is the widest label), rows and Add stay above the fold at both sizes. On the unfixed page the same measurement gives −98.6 px of free room below the dock for the tightest tree (the dock overruns the pane).
* **Negative control (my own probe against the unfixed `5f3b5bf3` page).** Clean: 244/244 pass. Four queued orders: **all 22 trees with blocked rows fail** (1 blocked: 12/12 fail, 2: 9/9, 4: 1/1), row heights 38 and **64.8 px**, worst rows-to-dock gap −94.1 px. The sweep therefore detects F-01 and does not report it on HEAD. (My previous review found 11 of 244 failing; this run queues the competitors that appear most often in other trees' lists, so it finds 22 trees with blocked rows. It is a different set-up, not a discrepancy.)
* **Is the reason reachable without sight?** Yes. Checked row by row (matched by position) on the trees with the most blocked rows, in both the queued and the approved-plus-Crop states:

| Channel | Result |
|---|---|
| Row tooltip (`title` on the label) | `Competitor 2 · MA-A-057 · Already pending Fell · 30.2 cm · 7.2%`, on every blocked row |
| Screen-reader text inside the label | `Cannot be chosen: Already pending Fell.` (the full reason), on every blocked row |
| Visible short text | `aria-hidden="true"`, so it is not read twice |
| Checkbox accessible name **as computed by Edge** | `2MA-A-057 2.50 m 0.4749 Moderate Cannot be chosen: Already pending Fell. 30.2 cm · 7.2%`: the full reason is present |
| Focusable map rank button of that competitor | `Competitor 2, tree MA-A-057, Fell draft (Already pending Fell)` |

  The accessible name lists distance and CI before the reason, so it reads a little out of order but loses nothing. The blocked checkbox is disabled and not tabbable, so a keyboard user reaches the reason through the map rank button, and a screen reader through browse mode. **Screen-reader output itself was not heard (NOT TESTED).**
* **Five different reasons on one tree (injected).** The engine never ranks a Felled or Dead tree, so I injected Felled, Dead, Approved plan, Retained Crop and Prune plan into one tree's five competitors. Heights `[38,38,38,38,38]` (and `[50.2 × 5]` at 1920×1080), no reason cut, the fold holds, each row has its tooltip and screen-reader text. The short forms "Reference" and "Other area" cannot occur in a ranked list and were not exercised.

### 4.2 F-02: the post-Add card

| | 1280×720 | 1920×1080 |
|---|---|---|
| Complete Add: card height | **36.4 px** (one line: title, Details, Dismiss) | 94.9 px (lines shown, no Details control) |
| Rows after the Add | all 5 above the card (last row bottom 633, dock top 638), 5/5 reachable | all 5 above (832 / 843), 5/5 reachable |
| Add form while the card is up | not rendered | not rendered |
| Partial / guard / unconfirmed card | 36.4 px, rows above, draft intact | 176 px, rows above, draft intact |
| Details open (on request) | 83.7 px; lower 11 px of row 5 under the dock (N-03); Hide details restores | not applicable |
| Dismiss | form back, 0 requests sent, live region says "Dismissed the Add result…" | same |
| Focus after the Add | the card (`tabindex=-1`); after Dismiss on the Add button, or on the heading if nothing is left to add | same |

I looked at the writer's `12-after-add-card-1280x720.png` (rows 1–5 above a one-line card "Added 4 trees to … · Details · × Dismiss") and at my own captures of the partial and the Details-open states. They show what the handoff says, and they show the cut title and the covered row 5 behind N-02 and N-03.

### 4.3 F-04: the documented minimum

* **Page comment:** the served page's top HTML comment says "The documented minimum supported desktop size is 1280 x 720 CSS px (owner decision, spec 0.2)". **CSS comment:** "1280 x 720 is the documented minimum supported desktop size (owner, spec 0.2); below it no fold is promised." I read both from the served resources, not from the source.
* **Visible note**, Details pane: "Screen size: this page is laid out for desktop browser windows of at least 1280 × 720. There, a selected tree's reference, its ranks 1–5 and Add fit without scrolling. Smaller windows still work but may need scrolling." Readable once scrolled to (N-07). The wording issue is N-01.
* **The fold holds there:** §4.1, including blocked rows, which the owner required.

### 4.4 F-05 and the stale-answer matrix

Each answer kind was held in flight (the real server answer held in the page's `fetch`; or the request held; or a forced guard or transport failure) against each interference. All 16 cases passed at both viewports: after release **nothing reselected, no draft cleared by the stale answer, alert empty, announcement unchanged and never naming the late tree, no write request, server view unchanged.**

| Answer \ interference | Escape | Close inspection | Area switch | Newer selection |
|---|---|---|---|---|
| ok | dropped whole | dropped whole | dropped whole | dropped whole (one read-only competition re-read for the newer tree) |
| refused (real 409, server fence moved) | dropped whole | dropped whole | dropped whole | dropped whole (same re-read) |
| guard (injected 403 with `error`) | dropped whole | dropped whole | dropped whole | dropped whole (same re-read) |
| transport (injected failure) | dropped whole | dropped whole | dropped whole | dropped whole (same re-read) |

* **Non-stale controls** (same answers, nothing interfering): `ok` selects the tree; `refused`, `guard` and `transport` each raise a visible alert and select nothing. So the silence above is caused by the stale check, not by a broken injection.
* **Dialog-only Escape (the F-05 change), three cases, both viewports:**
  * **D1 ok answer:** the dialog was opened while the hit-test was held; Escape closed **only** the dialog (the inspection stayed on the earlier reference); on release the late answer was applied as a **normal selection** of the tree the server answered (for example "Selected tree MA-A-106. 2 crowns overlap here."), no alert, server unchanged. This is the writer's intended new behaviour and what I reported as F-05.
  * **D2 refused answer:** same set-up with a real 409: the refusal is **shown** (`Refused: Map selection PlayerPlanStateStale …`), nothing selected, earlier reference kept. A dialog-only Escape does not make a current refusal look stale.
  * **D3 dialog Escape, then a normal Escape, still held:** the inspection closes and the late answer is dropped whole (no alert, announcement unchanged).
  * **D4 (extra):** Escape in the exact-ID box with text clears the box and the late answer is still dropped. This is the same class as F-05 and the writer left it as designed ("every other Escape still bumps"); the window is one loopback round trip, so it is practically unobservable. Reported for completeness, not as a finding.
* **Stale refused, nothing selected afterwards (fail closed):** no alert, no selection, no re-read, no write; the page stays on its older view. Its next fenced command, **Approve**, is refused by the server (`PlayerPlanStateStale`, 409), `approved` stays null, the revision does not move, the refusal is shown.
* **F-06 re-verified:** if instead the player **selects** a tree, the page self-heals: the selection's competition read carries the older fence, is refused, and the refused branch applies the newer view; the next Crop is then accepted (`rev` +1) on a fence that is now current. Never a write on a stale fence.

### 4.5 F-07: the light index text

Injected into one inspected open-ground cell **through the click's own hit answer** (a re-read would be overwritten, see §6.2). Identical at both viewports.

| L | Concise summary | Announcement | Technical details |
|---|---|---|---|
| 0 | `0.000` + "no modelled light" | `0.000` | `0.0000` |
| 1e-9, 0.00004, 0.0004, 0.00049999 | **`< 0.001`** | **`< 0.001`** | `0.0000`, `0.0000`, `0.0004`, `0.0005` |
| 0.0005, 0.00051, 0.0014 | `0.001` | `0.001` | 4 decimals |
| 0.0015 | `0.002` | `0.002` | `0.0015` |
| 0.302 (the real cell) | `0.302` | `0.302` | `0.3020` |

No positive index prints as `0.000`. After a normal re-read the real value returns. Residual: N-06.

### 4.6 F-09: headroom, and which number governs

| | PREV `5f3b5bf3` | HEAD `0e777455` |
|---|---|---|
| Free room below the Add bar, 1280×720, worst tree (dock measured static through CSSOM) | 8.4 px | **18.0 px** |
| Rows-to-dock gap | 4.8 px | 4.8 px (**unchanged**: it is the dock's own top margin) |
| Clean session: the tightest trees still fit down to a viewport height of | 708 px (fail at 704) | **697 px** (fail at 696) |
| Real headroom (viewport height that can be removed) | about 13 px | **about 23 px** |

**Which number governs.** Neither of the writer's two alone. The rows only meet the pinned dock when the content is taller than the pane, so what the page can absorb is **the free room below the dock plus the dock's own 4.8 px margin**: 18.0 + 4.8 ≈ 22.8 px. I confirmed it by shrinking the viewport: the tightest trees fit at 1280×697 and fail at 1280×696. The writer's "4 to 5 px" gap did not change (my earlier "4" was 4.8 rounded); the improvement is the free room, 8.4 → 18.0, which almost doubles the real headroom. The handoff is right that the gap is not what improved, but wrong that it does not count.

**Adequate or thin?** **Adequate for the stated minimum, and thin as design headroom.** Layout is deterministic here (rows are a fixed height, so the margin is the same in every state), 23 px is enough for the owner's rule, and it is nearly twice what the unfixed page had. It is not much against real variance: five rows of text at +5% font size would use it, and a different default font or a larger OS text setting could. Only the page area matters, not the pixel ratio: at 1280×720 CSS px with a device pixel ratio of 1.25, 1.5 and 2 the result is identical (18/18 sampled trees fit). **Browser zoom and text-size changes were not tested.**

Informational sampling, clean session, 18 trees spread over both areas (below 1280×720 the owner states no requirement applies):

| Viewport | Note | Trees fitting | Worst row overlap | Rows reachable | Add reachable |
|---|---|---|---|---|---|
| 1280×720 | documented minimum | 18/18 | none | 87/87 | 18/18 |
| 1280×697 | edge | 18/18 | 1 px | 87/87 | 18/18 |
| 1280×696 | | 1/18 | 2 px | 87/87 | 18/18 |
| 1280×650 | | 1/18 | 48 px | 70/87 | 18/18 |
| 1280×610 | a 1280×720 browser window after toolbars | 1/18 | 88 px | 53/87 | 18/18 |
| 1366×650 | a 1366×768 laptop window | 1/18 | 48 px | 70/87 | 18/18 |
| 1536×730 | a 1920×1080 screen at 125% scale | 18/18 | none | 87/87 | 18/18 |
| 1600×800 | | 18/18 | none | 87/87 | 18/18 |
| 1024×768 | below the minimum width | 1/18 | 29 px | 70/87 | 18/18 |

The **Add button stays reachable at every size**, so the harm below the minimum is hidden rows, not a lost action. (At 1280×697 the row overlap reads +1 px while all 18 trees still count as fitting, because the check allows 1 px of tolerance; the exact edge is 697.)

### 4.7 F-11: authorship

All **twelve** commits BASE..HEAD are authored and committed as `DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>` with a `Co-Authored-By: Claude Opus 5.5` trailer. The handoff's header row "Authorship (all commits)" now says so in one sentence covering every commit from `267be9ff` to the final HEAD, states that the Git author is not the actual author, and notes that the trailer is self-asserted. Complete and prominent.

### 4.8 F-03: unchanged, as the owner directed

The R3 diff does not touch `focusGroundPanel`, `scrollIntoView` or `inspectPoint`, apart from the F-07 text. Re-measured with my own R2 test (a tree inspected, then an open-ground read): the pane scrolls so the reading is visible, the reference ID and `× Close inspection` are not reachable, Add stays reachable. 1920×1080: `scrollTop` 244 and 5/5 rows reachable, **identical to R2**. 1280×720: `scrollTop` 360 (R2: 369; the 9 px follows from the spacing trims) and 0/5 rows reachable, as in R2. The next tree selection restores the primary block with no manual scrolling, at both sizes. The reading, its heading and the Add bar are visible.

---

## 5. Regression spot-checks (what R3 touched, or could have touched)

| Area | Result |
|---|---|
| Scope | `git diff BASE HEAD` touches only `PlayerMultiApi.cs`, `tests/ui-smoke-multiarea.sh`, and handoff, driver and evidence files. Forbidden paths (Core, `PlayerApi.cs`, `Program.cs`, Assets, fixtures, `.sln`, `.csproj`, `specs/`): none. PREV..HEAD non-doc files: exactly those two |
| C# outside the raw strings | Lines 1–227, the class header, the C# before `Css` and before `Js`, and the tail after the last `"""` are byte-identical to BASE. The PREV..HEAD page hunks start at lines 245 and 3937, inside the raw strings (class spans 228–4012) |
| Test script | `ui-smoke-multiarea.sh`: **0 lines removed** BASE..HEAD and PREV..HEAD; 131 added since PREV |
| Builds | `dotnet build MultiRackSitkaP1Lab.sln` and `src/Ui/SitkaP1.Ui.csproj` with the pinned `/home/jer/.dotnet/dotnet` **8.0.134**: **0 warnings, 0 errors** each |
| Core suite | **184/184**, exit 0. Anchors present and passing: `7e3a2a86…1ece`, `a692a6d5…233d`, `814e8165…6287`, `8a926776…87fb` |
| P0 pin | `.p0ref/SitkaLab.Core.dll` `9a99f781…34b3`: read-only copy, never rebuilt; before = after = expected; `sha256sum -c` OK |
| `ui-smoke-multiarea.sh` at HEAD | **138 pass / 6 fail**: J04, J05, J09, J10, J11, J99. The FAIL lines hash identically (`2dda0ed30311`) to my BASE run and to my R2 HEAD run. New check IDs against BASE: exactly F2a–F2h, R2a–R2j, R3a–R3j. **R3a–R3j: all 10 PASS** |
| `ui-smoke.sh` at HEAD | **250 pass / 4 fail**: L01, L02, L06, J31, identical to BASE |
| Writer's negative control, reproduced | HEAD's witness against the unfixed `5f3b5bf3` page: **130 pass / 17 fail** (the 6 pre-existing plus 11 R3 lines; the unfixed page also makes some checks time out, which adds lines and shifts the letters). R3b, the Add itself, still passes. Matches the handoff's 130/17 |
| Light ramp (P4, P4b) | Unchanged. Written opacity and fill for L = 0 / 1e-9 / 0.0004 / 0.01 / 0.05 / 0.2 / 0.5 / 1: not drawn / `0.00000127` / `0.00557` / `0.0451` / `0.1284` / `0.3162` / `0.5736` / `0.9000`; fills `#7f3b4a … #ffd9bd`; pixel change over the floor 0 / 0 / 1 / 8 / 27 / 89 / 231 / 497. L = 1e-200 is `9.000e-131` and Edge computes `0` for it (F-08, unchanged) |
| Ground precedence, ground-ON layout (P5, P6) | Crown wins; a rank toggles only its ID; a modal swallows the click with 0 requests; six crown selections with Inspect ground ON each leave rows and Add above the fold with `scrollTop` 0 and the ground panel below the dock (1280×720: rows end 633, dock 638–694, panel from 702; 1920×1080: 832 / 843–966 / 974) |
| Escape order (P3) | Dialog first, then Inspect ground (the Light Guide stays on), then the inspection; Close inspection preserves zoom, pan, area, Light Guide, Crop, queued orders and year; the draft is not restored |
| F-2 and Work Plan (P9) | `stage4:null` → read-out unavailable, Approve and Execute withdrawn, no model-0 statement; a paired re-read restores; a real Execute from a stale view is refused with nothing felled; a real Execute fells exactly the approved trees (A −11, B −1), one receipt; Year+1 reaches year 3. Smallest real L per area by year 0–3: A 0.00066978 / 0.00066555 / 0.00103927 / 0.00101745, B 0.01011 / 0.01004 / 0.00990 / 0.00972; none zero |
| Global | 0 CSP violations, 0 exceptions, 0 console errors other than the browser's own lines for the 409s I provoked, 0 HTTP 5xx, no horizontal overflow (1280 = 1280, 1920 = 1920) |

---

## 6. What I ran

Environment: pinned `/home/jer/.dotnet/dotnet` 8.0.134 only (the system dotnet was never used); `TMPDIR=/home/jer/.cache/review-r3-tmp` (outside the repo); Windows node v26.13.1 for the CDP driver and headless Microsoft Edge with a throwaway profile; every UI server on an **OS-assigned** 127.0.0.1 port, started and stopped by my scripts (verified by command line); listeners 33767, 43649 and 46593 alive before, during and after and never bound; `.p0ref` copied read-only from `CCF-Multi-Stage4UiCatchup-20261123`, never rebuilt. Scripts and sources are under `review-probe-r3-20261123/`.

### 6.1 Commands

| # | Check | Result |
|---|---|---|
| 1 | `git fetch origin`; worktree at the exact SHA; `git rev-parse HEAD` | equals `0e777455…`; PREV and BASE are ancestors; the candidate on origin equals it at start and end |
| 2 | P0 pin before / after | `9a99f781…34b3` both times; `sha256sum -c` OK |
| 3 | `dotnet build` solution and Ui | 0 / 0 each |
| 4 | Core suite | 184/184, four anchors present |
| 5 | Scope, C#-outside-raw-strings, test-script removals, authorship, PNG sizes and hashes, capability scan (`scripts/integrity.sh`) | §5, §4.7, §8 |
| 6 | Browser probe, final `review-probe-r3c.mjs` (plus `r3d` for the alert check) | §6.3 |
| 7 | `tests/ui-smoke-multiarea.sh` and `tests/ui-smoke.sh` at HEAD | 138/6 and 250/4; R3a–R3j PASS |
| 8 | Negative controls | HEAD's witness against the unfixed page: 130/17; my sweep against the unfixed page: 22 of 22 trees with blocked rows fail |

### 6.2 Defects in my own probe, found and fixed during this review

I list them because two of them first looked like failures of the page.

1. **A `.comp-empty` paragraph counted as a loaded competition.** My `selectById` (inherited from my R2 probe) accepted any `.comp-empty`, but the page shows "Reading competition…" in one while the read is in flight. Under load some trees were recorded with 0 rows, which my fold check passes vacuously: 44 trees in the after-execute state at 1280×720 and 45 in the Year+1 state at 1920×1080 in my first pass. Fixed; the sweep now waits, counts anything not loaded as **unmeasured**, and fails if there is any. The clean, queued and approved states of that first pass were unaffected (no zero-row trees) and the two final sweeps agree tree for tree.
2. **The accessible-name check matched rows by reason text**, so rows 2–4 with the same reason were compared with the first row's tooltip. It reported FAIL while every real sub-check was true. Fixed (match by position); the re-run passes.
3. **My fold probe injected a `<style>` element** to measure the dock's natural position. The page's CSP (`style-src 'self'`) blocks it, which showed up as 13 "CSP violations", the exact number of my probe calls. Not a page defect. Replaced by a CSSOM property; the final runs show 0 violations.
4. **The F-07 announcement check re-read through a path that replaced my injected value** with the real one (the hit-test answer re-applies the server view); it printed 0.302 for every row. Fixed by injecting into the hit answer, as the writer had also found.
5. **A D1 expectation predicted the topmost crown at the tree centre**, but Chromium rounds the click pixel and crowns overlap there. The page behaved correctly (it selected the tree the server answered). Fixed by recording what the server actually answered to that click.
6. **My P7 follow-up check assumed the page was still stale when Crop was clicked.** With fix 1 the selection read finishes first and the page self-heals, so Crop is legitimately accepted. Rewritten to accept only "refused with nothing written" or "self-healed, then accepted"; fail-closed is proved by the P7b Approve check.
7. **My key helper sent Enter without its character payload**, so Chromium never activated a focused button. Fixed; Enter and Space on Dismiss then both work.
8. **I placed two plan-approving sections ahead of other sections in one session** (P6, P7b and P9 then aborted for want of an unlocked plan), and I overwrote a launcher script while two runs were using it (bash read garbage after node returned; the two servers were then stopped by hand from their pid files, and the probe logs were unaffected).

### 6.3 Which run each result comes from

| Run (both viewports unless noted) | Sections | Result | Status |
|---|---|---|---|
| `fin-sw` | RS0–RS4 (five-state sweep, a11y, headroom) | 30/30 and 30/30 | **final** |
| `fin2-ft` | RDOC, RADD, RINJ, P0, P0b, P3, P4, P4b, P5, RESC, RF07, P6, P7, P7b, P9, PZ | **105/105** at 1280×720, **102/102** at 1920×1080 | **final** |
| `fin2-vp-1280` | RVP, RCARD2 | 2/2 (plus the viewport table) | **final** |
| `fin2-card-1920`, `fin2-meta-1280`, `fin2-meta-1920` | RCARD2, RMETA | 2/2 each | **final** |
| `fin2-alert-1280`, `fin2-alert-1920` | RALERT (probe `r3d`) | 3/3 each | **final** |
| `fin2-prev-1280` | RS0, RS1 against the unfixed `5f3b5bf3` page | 10/14: the 4 FAILs are the point of the control | **final** (negative control) |
| `sw2` | RS0–RS4, earlier probe | 30/30 each; per-tree records identical to `fin-sw` | supporting |
| `a11y1280` | RA11Y standalone | 6/6 | supporting |
| `sweep`, `ft`, `fin-ft`, `prev1280`, `trial*` | earlier passes | 23/25, 100/105 and 98/102, 93/99 and 92/96, … | **superseded** by §6.2 items 1–8; committed under `results/superseded/` |

---

## 7. NOT TESTED

* **Owner acceptance, integration, any `main` merge.**
* **Screen-reader output.** Accessible names, tooltips and the live region were read from the DOM and from Edge's accessibility tree, not heard.
* **Browser zoom, text-size or font changes, high-contrast mode, reduced motion, touch, other browsers, a real 1280×720 browser window on a real display.** Pixel ratios 1.25, 1.5 and 2 were emulated at a fixed CSS size.
* **Any viewport but 1280×720 and 1920×1080 for acceptance.** The other sizes in §4.6 are informational.
* **The writer's own driver and sweep** (`scenario-owner-ux.mjs`, `fold-sweep-owner-ux.mjs`) were not re-run; I wrote and ran my own. I checked the committed PNGs' sizes and hashes only (set 4,916,524 B, largest 762,902 B, under the 5 MB and 0.9 MB caps; the SHA-256 prefixes of the seven refreshed or new files match the handoff) and looked at two of them.
* **Felled, Dead and Prune-plan reasons on real data** (injected only); the short forms "Reference" and "Other area" (unreachable in a ranked list).
* **L < 0.0005 on a real cell** (injection only; none exists in years 0–3).
* **Lock collision / the D-091 grant** (AC17, AC18): I did not see the actual grant or edit the coordination board. I judged scope against the spec's proposed allowlist.
* **`p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`** (the two smokes' own safety checks ran).
* **Unity, Windows builds, save/load.**
* **A stale LOCKED hit answer** (static argument only, as before).
* **Whether a keyboard-only or screen-reader player finds Dismiss after a partial Add** (judged in §3.2 from the tab order and focus, not from a user).

---

## 8. Evidence integrity

* **What is mine.** Every PASS, FAIL and number in this document comes from commands I ran in my own worktrees with my own builds, fresh servers and my own probe. I treated the writer's handoff as a claim. The claims I tested and reproduced: blocked rows exactly as tall as free ones (38 px / 50 px); free room below the dock 8 → 18 px; the 130/17 negative control; the six pre-existing failures; the commit identity; the PNG sizes. The writer's own browser counts (96/96, 95/95, 14/14, 13/13) are **not** relied on.
* **Authority.** I read the owner clarifications at the commit you cited, `7b06da6f`. The Spec worktree has since moved to `901c7246` (another session's change, not mine), which adds one row to section 0.2 recording an AC22 substitution. The two rows I rely on are byte-identical at both commits. **I did not use the later row as authority**; whether the owner's AC22 substitution extends to this candidate is the owner's call, and I make no AC22 claim here.
* **Committed:** probe sources (`review-probe-r3c.mjs`, `r3d`, the helper, the patch, the parts, the as-run earlier file), scripts, redacted result JSON for every pass including superseded ones, the 14 per-tree sweep records, build, Core and smoke logs, and `results/OFFREPO-SHA256SUMS.txt`. **Off-repo** at `/home/jer/.local/share/ccf-evidence/review-r3-owner-ux-20261123/`: 24 raw run logs (capability redacted) and the PNG screenshots, with their checksums committed. I scanned every committed file: no `/s/<64 hex>/` path, and no one of the 24 real capability tokens my servers printed appears anywhere under the 900-ui-catchup folder (checked by searching for each token, text and binary). The other 64-hex strings are the SHA-256 checksums in the off-repo list, the four anchors, the P0 hash, and the fixture and experiment hashes that the Core and smoke logs print (for example the pinned `c66d566c…`). The smoke logs print the literal placeholder `/s/<capability>/`.
* **Screenshot caption warning.** In `RINJ-five-reasons-*.png` the Felled, Dead, Approved plan, Retained Crop and Prune plan reasons are **injected into the page's view**, not engine data. In the F-07 captures the cell's index is injected.
* **What I did not modify.** The candidate branch (still `0e777455` on origin at the end), `main`, the main checkout (clean), and the sibling worktrees `…OwnerUX-R2`, `…R3`, `…Independent`, `…ReviewBase`, `…Spec` and `…UiCatchup` (verified clean; HEADs checked by `scripts/wrapup.sh`). My review worktree stayed clean at the reviewed SHA. Worktrees I created and left in place: `CCF-Multi-Stage4OwnerUX-R3ReReview-20261123` (review, clean), `…R3ReReview-PREV-20261123` (detached at `5f3b5bf3`, **one tracked file deliberately modified**: HEAD's `ui-smoke-multiarea.sh` copied in for the negative control), and `…R3ReReview-Docs-20261123` (this deliverable). I reused my earlier `…R2Review-20261123` worktree read-only as the unfixed page for the sweep control.
* **Limits.** Windows node and Edge against servers in WSL; results are for that stack only.

---

## 9. Suggested follow-ups (none blocks this verdict)

1. **N-01 / N-07:** say "page area (viewport)" instead of "browser windows" in the note and the comments, and consider placing the note nearer the top of the Details pane.
2. **N-02:** at the short height, shorten the card title or drop the word "Details" so "of 3 · refused MA-A-005" survives; consider 44 px targets for the two new controls.
3. **N-04:** if the DBH and share tail matters on blocked rows, shorten "Approved plan" and "Retained Crop" by a character or two, or let the meta line use a slightly smaller font at 1280×720.
4. **N-05 (pre-existing):** give the reason Add is unavailable somewhere at the short height, for example in the Add button's `title`.
5. Keep the loaded-state sweep, and add the unmeasured-tree guard to the writer's driver.

---

## 10. Reproduction

```bash
# WSL, from the repository root, pinned dotnet 8.0.134, TMPDIR outside the repo; .p0ref copied read-only, never rebuilt
bash scripts/build-and-core.sh                                  # 0/0, 0/0, 184/184
# PROBE=review-probe-r3c.mjs (r3d adds RALERT); each state-mutating section needs its own fresh server
bash scripts/run-probe.sh <tag> 1280x720  RS0,RS1,RS2,RS3,RS4   # fresh server on an OS-assigned port
bash scripts/run-probe.sh <tag> 1920x1080 RS0,RS1,RS2,RS3,RS4
bash scripts/run-probe.sh <tag> 1280x720  RDOC,RADD,RINJ,P0,P0b,P3,P4,P4b,P5,RESC,RF07,P6,P7,P7b,P9,PZ
bash scripts/run-probe.sh <tag> 1280x720  RVP,RCARD2
bash scripts/run-probe.sh <tag> 1280x720  RMETA
bash scripts/smokes.sh && bash scripts/negctl.sh
```

Sections: RS0–RS4 five-state fold sweep with a11y and headroom · RDOC documented minimum · RADD post-Add card, real partial, guard and unconfirmed · RINJ five reasons injected · RESC dialog-only Escape · RF07 ground read and light-index text · RVP viewport and pixel-ratio sampling · RCARD2 card after approve · RMETA blocked-row text · RA11Y accessible names · RALERT plain refusals raise no alert · P0–P9 and PZ are the legacy sections of my R2 probe, unchanged except one wait condition (§6.2 item 1) and the P7 follow-up wording (item 6).

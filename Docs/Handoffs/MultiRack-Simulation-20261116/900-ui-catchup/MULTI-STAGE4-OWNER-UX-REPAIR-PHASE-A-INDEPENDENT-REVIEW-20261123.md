# MULTI Stage 4 owner UX repair, Phase A: independent review (R2 candidate)

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Reviewer | Claude Sonnet 5.5, independent reviewer. Owner-designated substitute for GPT-6.1 Sol (see §2). Not the author of the code and not the implementer's helper |
| Repository / branch | `jermur788/CCF`, `feat/multi-stage4-owner-ux-20261123-v1` |
| **Reviewed HEAD** | **`5f3b5bf3c9f711125fb61ddee55de1ff9690c19b`** (confirmed equal to `origin/feat/multi-stage4-owner-ux-20261123-v1` at start and again at the end; the branch did not move) |
| BASE | `e7c1a57da519ed1ee03ccfbad240196a66df0905` (confirmed an ancestor of HEAD) |
| Commits reviewed | `267be9ff`, `0e17f641`, `9fed4eb9`, `9dd6f601` (Phase A) · `1a22d01c`, `f0db8323` (R1) · `550e10b4`, `5f3b5bf3` (R2). Linear, nothing rewritten |
| Authority | `specs/multi-stage4-owner-ux-20261123-v1/` at `b95a56bf` (APPROVED, Phase A only). That commit is **not** an ancestor of the candidate, so the spec is not in the candidate tree; I read it from its own commit |
| Handoff treated as a claim | `MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-IMPLEMENTATION-RESULT-20261123.md` at HEAD |
| This document | New file on my own docs branch `docs/multi-stage4-owner-ux-independent-review-r2-20261123-v1`, cut from the reviewed SHA. **Not** on the candidate branch |
| **VERDICT** | **NEEDS CHANGES** |

This is a review verdict. It is **not** owner acceptance, **not** AC22 as written, and **not** integration. Nothing was merged. The candidate branch was not modified, rebased, amended or pushed to.

---

## 1. Verdict in one page

**NEEDS CHANGES**, because one acceptance criterion fails at the mandatory viewport in a realistic session:

* **F-01 (MAJOR): AC01 / UX-01 does not hold at 1280×720 once any of the five competitors is already blocked** (queued, approved, Crop, felled). The five rows plus Add are above the fold for every one of the 244 living trees in a *clean* session, but a single blocked row wraps to three lines (≈65 px against the 38 px compact rule), and rows 4 and 5 slide under the sticky Add dock. With 4 queued Fell orders, 11 of 244 trees fail; every tree with exactly one blocked row fails. This is the owner's original complaint, and it returns in the normal working loop (queue a tree, then inspect its neighbours). At 1920×1080 it does not occur (0 of 244).

What is good, and verified by my own runs rather than the handoff's numbers:

* **Both R2 repairs are genuinely fixed.** Layout with Inspect ground ON passes for every crown selection tested (§5.1). Stale hit-test answers of all four kinds (ok, refused, guard, transport), each held in flight against four different interferences (Escape, Close inspection, area switch, newer selection), are dropped whole, with non-stale controls proving the silence is caused by the stale check (§5.2). A stale REFUSED answer leaves the page on an older view, and the next fenced command (Approve, or a real Execute) is refused by the server with nothing written, so it fails closed.
* **The light ramp is continuous at zero** and never writes a positive opacity as "0" (§5.3).
* **Builds, Core suite, four frozen anchors and the P0 pin are unchanged** from BASE to HEAD (§6).
* **All product changes sit inside the three `MultiAreaPage` raw strings.** The C# above, between and after them is byte-identical to BASE.
* **Scope is respected**: no Core, `PlayerApi.cs`, `Program.cs`, Assets, fixtures, `.sln` or `specs/` touched.

The repair is small and stays inside the allowed file (see §10). Everything else in the owner's list works as specified.

### Findings at a glance

| ID | Severity | One line |
|---|---|---|
| F-01 | **MAJOR** | AC01 fails at 1280×720 in a loaded session: any blocked competitor row breaks the fold (11/244 trees with 4 queued orders; 11–24/244 with an approved plan and a Crop) |
| F-02 | MINOR | The post-Add confirmation card grows the sticky dock to ≈173 px at 1280×720 and covers competitor rows 2–5 until the next selection or Clear |
| F-03 | MINOR | An open-ground read with a tree inspected scrolls the reference ID, `× Close inspection` and (at 1280×720) all five rows out of view. Judged below: does not contradict AC01, AC13 or UX-07 |
| F-04 | MINOR | No documented minimum supported desktop width (UX-01 asks for one); at 1024×768 the fold fails for 5 of 6 sampled clean-session trees |
| F-05 | OBSERVATION | An Escape that only dismisses a dialog also cancels an in-flight map click |
| F-06 | OBSERVATION | A dropped stale REFUSED answer discards the newer view it carried; the page self-heals on the next selection and fails closed on the next command (verified) |
| F-07 | OBSERVATION | The concise ground summary prints 3 decimals, so a positive L below 0.0005 would read "0.000". Not reachable with real data (smallest real L over years 0 to 3 is 0.00067) |
| F-08 | OBSERVATION | Light ramp precision limits: opacity below roughly 1e-38 computes to 0 in Edge; positive L below about 7e-4 changes no 8-bit pixel (both disclosed or far outside real data) |
| F-09 | OBSERVATION | Clean-session fold margin is only 4 px at 1280×720 (rows to dock) |
| F-10 | OBSERVATION | The 6 + 4 smoke failures are pre-existing (reproduced at BASE), but the J99 witness crash means checks after it never run, at BASE or HEAD |
| F-11 | OBSERVATION | Commit authorship disclosure: present but narrow and buried (§9) |

No BLOCKING findings. Owner decisions (UX-08 crown-wins, F-2 Approve lock) were judged for faithful implementation and honest communication only (§9).

---

## 2. Reviewer-route context

* The intended reviewer was **GPT-6.1 Sol**. Its run against `f0db8323` was **terminated by a provider usage limit before it published anything**, so before this review there was **no review and no verdict** on the work. The handoff itself says the two R2 defects and the ramp note were "unverified observations" from that run.
* The owner designated **me (Claude Sonnet 5.5)** to produce the first verdict. I have **not seen any output of the Sol run**, only the handoff's description of it. I did not rely on the writer's evidence for any PASS below.
* **AC22 is written as "Independent GPT-6.1 Sol review".** I cannot satisfy that wording. This document is a substitute review; whether it satisfies AC22 is for the owner to decide. I mark AC22 NOT TESTED / not met as written.
* I am a different model family and thread from the implementer (Claude Opus 5.5) and from the coordinator. I wrote my own browser probe and did not import the writer's scenario.

---

## 3. Findings in detail

All line numbers are in `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` at HEAD unless stated.

### F-01 MAJOR: AC01 fails at 1280×720 in a loaded session

**Mechanism.** A competitor row that is blocked renders a status sub-line (`comp-why`, e.g. "Already pending Fell", "In the approved plan", "Retained Crop") as a third line in its left cell (`:2872`). The compact 1280×720 rule fixes only a minimum height (`.comp-row label { min-height: 38px; … }` at `:1120`, inside the `(max-height: 820px)` block opened at `:1100`), so a blocked row grows to about 65 px. The Add bar is `position: sticky; bottom: 0` (`.add-dock`, `:923`), so rows 4 and 5 end up behind it. The Add button itself stays reachable in every case I measured.

**Measured (my probe, real mouse and keyboard in Edge, fresh server per run):**

| Session state | 1280×720 | 1920×1080 |
|---|---|---|
| Clean (no orders), all 244 living trees by exact ID | **0 failures**; minimum gap rows→dock **4 px**, Add→viewport bottom 22 px | 0 failures; gaps 11 px / 151 px |
| Clean, 28 real crown clicks across both areas | 0 failures (9 clicks selected an overlapping neighbour; the hit-test picks the topmost) | 0 failures |
| **4 Fell orders queued in Area A** | **11 of 244 fail.** 233/233 trees with 0 blocked rows pass; 4/4 with 1 blocked row **fail**; 5/5 with 2 fail; 1/1 with 3 fail; 1/1 with 4 fail | 0 of 244 |
| Plan **approved** + Crop designation(s) (the passes differed in how many orders and Crops had been made) | **24 of 244 fail** (final pass: 5 approved) and **11 of 244 fail** (pass 3: 4 approved) | 0 of 244 (both runs) |

Sample failure: `MA-A-001` with 4 blocked rows: last row bottom 750 against a 720 viewport, only 3 of 5 rows reachable. `MA-A-002` with 2 blocked rows: last row bottom 696 against dock top 656, 4 of 5 rows reachable.

**Why the writer's evidence missed it.** The browser driver's `selectReference` only accepts a tree "with five ranked competitors none of which is blocked" (`browser-driver-owner-ux/scenario-owner-ux.mjs:74-86`), and the smoke witness uses clean sessions. Every fold check therefore ran on the favourable case.

**Why it matters.** The spec says "after selecting any living tree" and AC01 says "any suitable living tree". Queuing several trees and then inspecting neighbours is the core workflow, and the fraction of affected trees grows as the plan grows.

**Suggested fix (not applied; I did not edit the candidate).** Keep the blocked-reason on one line (single-line ellipsis with the full text in `title` and `aria-label`, or fold it into the meta line), or lower the row height for blocked rows at `max-height: 820px`; then add a loaded-state check (select references whose rows include blocked ones) to the browser driver and the smoke witness.

### F-02 MINOR: post-Add confirmation card hides rows 2–5 at 1280×720

After **Add**, the confirmation card (`.batch-result`, up to 11 rem, `:918`) sits inside the sticky dock (`:923`). At 1280×720 the dock grows to about 173 px (dock top 539, bottom 712), 2 of 5 rows stay reachable; at 1920×1080 4 of 5. It clears on the next selection or Clear. This is **not** an AC01 violation (AC01 is about selecting a tree) but it is the same "rows behind the dock" family as F-01. Screenshot index entry: `pass3-…/P2-after-add-1280x720.png`.

### F-03 MINOR: an open-ground read scrolls the primary block out of view (judgement requested in the task)

`inspectPoint` calls `focusGroundPanel()` (`:2547`), which calls `scrollIntoView({ block: 'nearest' })` on the panel (`:2500-2503`). With a tree inspected the panel sits below the Add bar, so the read scrolls the inspector.

| | 1280×720 | 1920×1080 |
|---|---|---|
| scrollTop after the read | 369 | 244 |
| Reference ID reachable | no | no |
| `× Close inspection` reachable | no | no |
| Competitor rows reachable | **0 of 5** | 5 of 5 |
| Reading (heading + summary) reachable | yes | yes |
| Add bar | at the top edge of the pane, top 285 against pane top 292 (≈7 px clipped) | reachable |

**Judgement.**
* **AC01: no contradiction.** AC01 is about what a *tree selection* shows. I verified separately that every selection path resets the pane to the top (`:3451`): after a scrolled inspector (scrollTop 708 at 1280×720, 584 at 1920×1080) a different tree selected **by map** and **by exact ID** shows the full primary block again with no manual scrolling; and after a scrolled ground read the next crown selection does the same.
* **AC13 / UX-07: no contradiction.** The reading (level word, engine index, separate canopy cover, shading-tree count) is visible; IDs and provenance stay behind closed disclosures.
* **Minor cost.** For a moment the reference ID and `× Close inspection` are off-screen (Escape and `× Stop` still work). A draft in progress remains visible through the map's rank markers and the sticky Add bar. I rate it MINOR as a design trade-off, not a spec violation; the owner may prefer the reading to appear without scrolling the primary block.

### F-04 MINOR: no documented minimum supported desktop width

UX-01: "If narrow viewport makes this impossible, apply a responsive stack and verify a documented minimum supported desktop width; 1280×720 remains mandatory." The handoff documents no minimum width (it says only that below 1024 px "no fold requirement applies", and lists a horizontal-overflow probe at 1024×768). My informational sample: at **1024×768**, 5 of the 6 trees sampled in Area B (all five that have five rows) have rows partly under the dock (`lastBottom` 742 against `addBottom` 754); at **800×600** the layout stacks (page scrolls vertically, no horizontal overflow: `scrollWidth` 785 within 800). Not an acceptance viewport, but the documentation the spec asked for is missing.

### F-05 OBSERVATION: dialog-only Escape cancels an in-flight map click

`onEscape` increments `state.mapPick` **before** it checks for an open dialog (`:3543-3545`), so an Escape that only dismisses the Year+1 dialog also cancels a map click whose hit-test is still pending. Reproduced (E1): the dialog opened while the hit-test was held, Escape closed it, the late answer was discarded and the earlier reference stayed. The window is a loopback round trip, so this is practically unobservable. Not an acceptance issue.

### F-06 OBSERVATION: the dropped stale REFUSED answer's newer view is not applied

The stale check (`:3477-3482`) runs before the outcome branches, so a stale refused answer's newer `view` is not applied. Verified behaviour: the page stays on its older view; the next **selection** self-heals it (its competition read is refused and the refused branch applies the newer view, `:2820`); the next **fenced command** is refused by the server (`PlayerPlanStateStale`, 409), nothing is written, and the refusal is shown (§5.2). Fail-closed holds. A stale LOCKED answer cannot exist: `call()` returns `locked` synchronously, before any await (`:1440`), only if `state.lock` was already set, and `onMapClick` checks the lock immediately before calling (`:3468`). That is a static argument; I did not execute it.

### F-07 OBSERVATION: 3-decimal ground summary

The concise summary prints `fixed(cell.lightIndex, 3)` (`:2456`; `fixed` is `toFixed` at `:1284`). Injecting L = 0.0004 and L = 0.00004 into one inspected cell gives "Very shaded · Light index 0.000" (L = 0 gives the same number plus "· no modelled light"). Technical details show 4 decimals. **Not reachable with real data I produced**: smallest real L per area by year (all cells, both areas, none zero):

| Year | Area A min | Area B min |
|---|---|---|
| 0 | 0.00066978 | 0.01011 |
| 1 | 0.00066555 | 0.01004 |
| 2 | 0.00103855 | 0.00990 |
| 3 | 0.00101614 | 0.00972 |

Suggest 4 decimals or "< 0.001" for robustness if the stand ever gets darker.

### F-08 OBSERVATION: ramp precision limits

L = 1e-200 is written as `9.000e-131` and Edge computes opacity `0` (float underflow below about 1e-38); far below any real cell (smallest 6.7e-4). Positive L below about 7e-4 changes no 8-bit pixel (measured: L = 0.0004 changes 1 summed RGB level per pixel; L = 1e-9 changes 0). The handoff discloses the second point (D-6).

### F-09 OBSERVATION: thin fold margin

In a clean session at 1280×720 the five rows end only 4 px above the dock in the worst tree. Any extra line in the header, a longer chip, browser zoom or a different device pixel ratio would break it. I did not test zoom or DPR (NOT TESTED).

### F-10 OBSERVATION: smoke witness health

All six multi-area failures (J04, J05, J09, J10, J11, J99) and all four single-area failures (L01, L02, L06, J31) **reproduce at BASE** in my own BASE worktree, so none was introduced by this change. J99 is the witness crashing on `byId('reason-choice')`; checks after the crash point never run at BASE or HEAD. The R2 tests sit above it. J09 is a map-click check, so the old witness has not been exercising map-click selection since #885.

### F-11 OBSERVATION: commit identity disclosure

See §9.

---

## 4. AC01–AC22

Status uses only PASS / FAIL / NOT TESTED. Each PASS rests on evidence **I executed**; the run that produced it is named in §6.

| AC | Status | Evidence (mine) |
|---|---|---|
| AC01 | **FAIL** | PASS in a clean session at both viewports (244/244 trees, exact-ID; 28 real crown clicks; ground ON 6/6 crown selections). **FAIL at 1280×720 in a loaded session** (F-01). Pruning is below the primary block ✓ |
| AC02 | PASS | Five `<button>` rank markers carry engine IDs in engine rank order; map clicks 1..5 toggle exactly that ID, multi-select, re-click clears, reference unchanged, **zero requests (CDP and page level)**; keyboard Tab + Space with 3 px focus ring; at both viewports |
| AC03 | PASS | Inspector→map and map→inspector both directions; draft intact and in sync after Area B and back; new reference clears both |
| AC04 | PASS | Add queued exactly the chosen four IDs; Pending, Approve, Execute separate (Approve fells nothing; Execute fells exactly the four) |
| AC05 | PASS | `× Close inspection` and Escape restore the normal map (no rank layer, no dimming, no leaders), clear the draft (not restored on reselect), and preserve zoom, pan (viewBox identical), area, Light Guide, Crop, queued orders and year; server view identical. (First-pass FAIL was my own confound; repaired run PASS at both viewports) |
| AC06 | PASS | Dialog open: Escape closes only the dialog, inspection stays |
| AC07 | PASS | One 34 px line "Synthetic Sitka experiment · 2 areas · Site unknown"; Details holds every caveat; map pane text contains none; Details body uses the same `MultiDisclosureStage4` label as BASE's banner |
| AC08 | PASS | Compass 3×3 (N top-centre, W/E middle, S bottom), one `− \| Fit \| +` row, one `Area A \| Area B` row, all targets ≥ 44 px; N/W/E/S pan the right way, + narrows, − steps back, Fit restores the exact view, pans disabled at Fit; both viewports (also 1024×768, 800×600) |
| AC09 | PASS | Default OFF; ON adds **only** `polygon.light-cell` nodes (per-tag DOM delta = polygons only); no disc, text or band marker; Light-on vs off pixel diff **outside the drawn cells is 0** |
| AC10 | PASS | Opacity and colour from the engine L, continuous at 0 (no floor), strictly increasing; L=0 draws nothing and changes 0 pixels; L=0.01 faint but non-zero; L=1 `#ffd9bd` at 0.9; real-layer pixel change 0 / 0 / 1 / 8 / 27 / 89 / 230 / 497 for L = 0 / 1e-9 / 0.0004 / 0.01 / 0.05 / 0.2 / 0.5 / 1 (§5.3) |
| AC11 | PASS | Hues 347° / 7° / 25° (rose, coral, peach): not green, not amber. Composite L=1 over the floor 9.97:1, over the sampled canopy 8.7–9.0:1; L=0.5 3.09:1; low L is faint by design. Visual review of screenshots: crowns stay readable. (Judgement partly visual) |
| AC12 | PASS | Light ON: crown click selects; rank toggles work; Inspect ground ON: crown wins, rank beats crown beats ground, modal swallows clicks with **zero requests** |
| AC13 | PASS | Level word, engine light index (3 dp), separate engine canopy cover, shading-tree count, no IDs on top; IDs, cell, 2.5 m grid, `k10a10` provenance, "Calibrated PAR no" and the caveat behind closed disclosures. Numbers equal the engine cell (spot L 0.3023, cover 5 %, 6 trees). See F-07 for the 3 dp note |
| AC14 | PASS | No "not measured sunlight"/PAR/calibration text on the map, Light ON or OFF; limits remain in Technical details and Details |
| AC15 | PASS | `stage4:null` → "read-out unavailable" banner, Approve and Execute disabled ("… · re-read needed"), no model-0 or edge-exclusion statements, paired re-read restores; server still returns 409 `PlayerPlanStateStale` for a stale fence; a real Execute from a stale view is refused, nothing felled |
| AC16 | PASS | 1280×720 and 1920×1080: no horizontal overflow at start and end; stand never under the dock at Fit (A and B); legend Hide/Show keeps the fold and causes no overflow; shown area exposed as the pressed Area button; focus and keyboard verified; announcements read from the DOM. Screen-reader output NOT TESTED. Below 1024 px: informational only (F-04) |
| AC17 | PASS | Diff touches only `PlayerMultiApi.cs`, `tests/ui-smoke-multiarea.sh` and handoff/evidence files. No Core, `PlayerApi.cs`, `Program.cs`, Assets, fixtures, `.sln`, `specs/`. **Caveat:** judged against the spec's *proposed* allowlist; the actual D-091 grant was not available to me |
| AC18 | PASS | Candidate derives from `e7c1a57d` (ancestor), eight linear commits, branch unmoved during my review, candidate not contained in `origin/main`. **Lock collision: NOT TESTED** |
| AC19 | PASS | Builds 0/0, Core 184/184, four anchors and P0 pin unchanged BASE→HEAD; the 6 + 4 baseline smoke failures reproduce at BASE and are reported honestly by the writer |
| AC20 | PASS | Real headless Edge at both viewports; no CSP violation, no 5xx, no console error other than the browser's own "409" lines for refusals I provoked; no capability URL in any committed file |
| AC21 | PASS | Light Guide, Inspect ground, Close inspection: server view byte-identical; real Execute fells exactly the four approved trees with one receipt and year unchanged; Year+1 advances one year at a time (to year 3) |
| AC22 | **NOT TESTED** | Written as "Independent GPT-6.1 Sol review"; Sol produced nothing (§2). This document is a substitute; the owner decides whether it counts |

Phase B (AC23–AC29) is out of scope and unapproved; the diff contains no Phase B work.

---

## 5. The three repaired items, attacked

### 5.1 Layout (R2 defect 1): genuinely fixed for selection; one disclosed design choice

* **Ground ON + crown selection at 1280×720:** six distinct crown selections: reference, rows 434–643, Add 654–698, `scrollTop` 0; ground panel in `#ground-slot` starting at 712, below the dock bottom 704 (so it is below the fold, as designed). With all five ticked: "Add selected 5 to Fell plan", still above the fold. Same at 1920×1080 (rows to 832, Add to 929).
* **Scrolled inspector then another tree:** `scrollTop` 708 → 0 by map, 708 → 0 by exact ID (1920×1080: 584 → 0, 602 → 0). Primary block visible with no manual scrolling.
* **Writer's smoke negative control, re-run by me:** HEAD's `ui-smoke-multiarea.sh` against the pre-R2 page `f0db8323` in a throwaway worktree: **8 of the 10 R2 checks fail** (R2a–R2e, R2h–R2j); R2f and R2g are non-regression guards and pass on both. The new tests are real.
* **Residual:** F-01 (blocked rows), F-02 (post-Add card), F-03 (ground read scroll), F-04 (narrow width).

### 5.2 Stale hit-test answers (R2 defect 2): genuinely fixed

I produced **four answer kinds**: `ok` (server answer held after processing), `refused` (the request held; the server fence advanced by a side-channel Crop; the late request then gets a real 409 with a newer view), `guard` (injected 403 with `error`), `transport` (injected network failure). Each was held in flight while one of four things happened: **Escape**, **Close inspection**, **area switch**, **newer selection by exact ID**. 16 cases per viewport.

* In all 16 cases at both viewports, after release: nothing reselected, no draft cleared *by the stale answer* (a "Cleared…" announcement can only come from the newer selection itself, and it was already in place before release), alert empty and not shown, announcement unchanged and never naming the late tree, **no write request** (only a read-only competition re-read for the newer selection), server view hash unchanged.
* **Controls (not stale):** `ok` selects the tree; `refused`, `guard`, `transport` each raise a visible alert and select nothing. So the silence above is caused by the stale check, not by a broken injection.
* **Stale REFUSED, nothing selected afterwards (P7b):** no alert, no selection, **no re-read**, no write; the page keeps its older view. Its next fenced command, **Approve**, is refused by the server (`PlayerPlanStateStale`, HTTP 409), `approved` stays null, server revision unchanged, refusal shown. A **real Execute from a stale view** is refused the same way: 0 events, living counts unchanged. The writer said a stale REFUSED/LOCKED case was not covered by a test; REFUSED is now covered by mine. LOCKED: static argument only (F-06).
* **Exploratory (no pass/fail):** E1 = F-05. E2 (clean state): a rank toggled while a map click is in flight is honoured in order; the late answer then selects the newly clicked tree and clears that draft with the announcement "Cleared 1 unsent draft choice for the previous reference MA-B-031." Consistent and announced.

### 5.3 Light ramp: continuous at zero, never "0"

* Opacity is `0.9 · L^0.65` (`:1359-1366`), text from `opacityText` (`:1369-1372`). Injected values on the **real layer**, Area B, composite measured on clean floor pixels (5×5 and ≥3 px from any overlay):

| L | written opacity | fill | mean summed-RGB change over floor |
|---|---|---|---|
| 0 | not drawn | n/a | 0 |
| 1e-9 | `0.00000127` | `#7f3b4a` | 0 |
| 0.0004 | `0.00557` | `#803c4a` | 1 |
| 0.01 | `0.0451` | `#88414e` | 8 |
| 0.05 | `0.1284` | `#994d54` | 26.6 |
| 0.2 | `0.3162` | `#bf6763` | 89 |
| 0.5 | `0.5736` | `#e49383` | 231 |
| 1 | `0.9000` | `#ffd9bd` | 497 |

  Strictly increasing opacity and monotone pixel change; L=0 changes exactly 0 pixels; every positive L has an opacity text that is positive and not "0", and Edge parses it (computed value is not the default 1). L = 1e-200 is `9.000e-131` as text (F-08).
* **Composite accuracy (my analysis, with a stated limit).** For clean-floor pixels the page renders `under·(1−a)+colour·a` for each cell's own opacity and colour. Binned by distance to the nearest non-floor overlay pixel:

| Viewport | ≥5 px from any overlay | 3–4 px from an overlay |
|---|---|---|
| 1280×720 | 42 samples, max error 0.6/255 | 516 samples, 1 pixel at 4.1 |
| 1920×1080 | 2,113 samples, max error 1.7/255 | 3,786 samples, 380 pixels deviate, max 29.2 |

  All deviation is in the 3–4 px band beside crowns. That is consistent with the translucent edges of the tree sprites (`<image>` sprites, `:2123`) being drawn *above* the light layer, which is by design. I did **not** isolate that element; the evidence is the distance pattern. My probe's pass flag for this check is vacuously true at the far bins, so I rely on the table, not the flag.
* **No halo or glow anywhere else:** pixels outside the drawn cells are identical with the guide on and off (0 differences at both viewports).
* **Area A (real values):** 144 cells, all L > 0 (0.00067 to 0.22 before any Fell), all 144 drawn; no light is invented. After the 4-tree Execute the engine's own maximum rises to 0.56, as expected.

---

## 6. Every check I ran (command and result)

Environment: pinned `/home/jer/.dotnet/dotnet` **8.0.134** (system dotnet 10 never invoked); `TMPDIR=/home/jer/.cache/review-r2-tmp` (outside the repo); Windows node v26.13.1 for the CDP driver, WSL login-shell node v26.13.1 for the smokes; headless Microsoft Edge launched with `--remote-debugging-port=0` and a throwaway profile; every UI server on an **OS-assigned** 127.0.0.1 port, started and stopped (SIGTERM, only by verified command line) by my scripts. Scripts are in `review-probe-r2-20261123/scripts/`.

| # | Check | Exact command | Result |
|---|---|---|---|
| 1 | Review worktree at the exact SHA | `git fetch origin; git worktree add -B review/multi-stage4-owner-ux-r2-20261123 <wt> 5f3b5bf3c9f711125fb61ddee55de1ff9690c19b; git rev-parse HEAD` | HEAD equals `5f3b5bf3…`; remote ref equal; BASE is an ancestor |
| 2 | P0 pin before | `sha256sum .p0ref/SitkaLab.Core.dll` after a read-only copy from the UiCatchup worktree; `build-p0-core-ref.sh` never run | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` = expected; `sha256sum -c` OK |
| 3 | Solution build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | 0 warnings, 0 errors |
| 4 | Ui build | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | 0 warnings, 0 errors |
| 5 | Core suite | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **184/184**, exit 0. Anchors present and PASS: `7e3a2a86…ece`, `a692a6d5…233d`, `814e8165…6287`, `8a926776…87fb`; P0 trust-record checks PASS |
| 6 | Same at BASE | same two commands in my own BASE worktree `e7c1a57d` | 0/0; 184/184; **anchor fingerprint lines identical to HEAD**; P0 hash identical |
| 7 | P0 after | `sha256sum` again at the end | identical to before and expected |
| 8 | Multi-area smoke at HEAD | `DOTNET=… NODE=… bash tests/ui-smoke-multiarea.sh <evidence-dir>` | **128 pass / 6 fail**: J04, J05, J09, J10, J11, J99. `git status` of the lab: 0 entries before and after |
| 9 | Single-area smoke at HEAD | `bash tests/ui-smoke.sh <evidence-dir>` | **250 pass / 4 fail**: L01, L02, L06, J31 |
| 10 | Same two at BASE | own script in the BASE worktree | **110 pass / 6 fail** (same six); **250 / 4** (same four): the failures are pre-existing |
| 11 | Negative control | HEAD's `ui-smoke-multiarea.sh` copied into my throwaway `f0db8323` worktree | 120 pass / 14 fail: the same six plus R2a, R2b, R2c, R2d, R2e, R2h, R2i, R2j. R2f, R2g pass |
| 12 | Scope | `git diff --name-only BASE HEAD` grouped; grep of forbidden paths | 21 files: 1 handoff, 6 driver, 12 PNG, `PlayerMultiApi.cs`, `ui-smoke-multiarea.sh`. Forbidden paths: none |
| 13 | C# outside the raw strings | line-range `diff`, `md5sum` of tails | Lines 1–227 identical; class header identical; C# before `Css` and before `Js` identical; tail after the last `"""` identical (md5 equal). First hunk at line 280, last at 3876, class spans 228–3920 |
| 14 | Test script diff | `git diff BASE HEAD -- tests/ui-smoke-multiarea.sh` | 182 lines added, **0 removed** |
| 15 | Numeric logic leaked into the page | grep of added lines for `Math.*`, `*100`, constants | Only the presentation mapping (`LIGHT_EASE`, `LIGHT_ALPHA_MAX`, easing, colour interpolation, opacity text). No ecological computation |
| 16 | Event handling diff | grep of added/removed lines for propagation, focus, scroll, listeners | The old `notice-details` click handler was removed and replaced by `notice-provenance`; the rank layer is a *sibling* of the SVG so a rank click never reaches the map handler; `stopPropagation` is belt and braces |
| 17 | Evidence integrity | sizes, `sha256sum`, `git grep` | See §8 |
| 18 | Browser probe | `node review-probe.mjs <outDir> <urlFile> <W>x<H> [sections]` (sources committed) | See table below |

### Browser probe: which run each result comes from

I ran the probe in several passes because my own harness had defects the first time. Every result I rely on comes from a pass in which the harness was sound; superseded results are listed so nothing is hidden.

| Pass (both viewports unless noted) | Sections | Result | Notes |
|---|---|---|---|
| **run1-full** | P0 P1 P2 P1b P3 P4 P5 P6 P7 P9 PZ | 1280×720 59/71, 1920×1080 61/71 | **Valid source for:** P1 (clean sweep), P1b (4 queued), P5, P6, P7 (16-case matrix and controls). **Superseded:** P3 (my confound: I turned the Light Guide on between baseline and close, and its legend changes the map aspect), P4 (map height differed by 25 px between frames because the legend took height), P7 follow-up (selecting a tree first re-synced the page through its competition read, so nothing was refused), P9 (aborted: the quick-ID box lives in the Tree pane and the Plan pane was active), PZ (13 console lines were the browser reporting my provoked 409s) |
| **final** | P2 P3 P4 P4b P7b P9 PZ | 1280×720 40/44, 1920×1080 41/44 | **Valid source for:** P3 (AC05/06), P4b (L=1e-200), P7b (stale REFUSED fail-closed), approved-plan sweep. 1 P4 composite FAIL at the strict ±3 tolerance (led to the distance analysis); 1 P9 abort that became the deliberate stale-Execute check |
| **pass3** | P0 P0b P2 P4 P9 PZ | 1280×720 49/51, 1920×1080 50/51 | **Valid source for:** P0, P0b (every nav button, dock vs stand, legend, pressed state), P4 (distance-binned composite), P9 (F-2, stale Execute, real Execute, years 0–3), PZ. Remaining FAILs: F-02 (post-Add card, both viewports) and F-01 (approved-plan sweep, 1280×720) |
| **narrow** | P0 P0b PN PZ | 1024×768 16/17, 800×600 16/17 | Informational (F-04). The one FAIL each is the fold check |

**Observed in the valid passes, both viewports unless noted:** AC02/AC03 transfers PASS; `AC01 clean` PASS; `AC01 loaded` FAIL at 1280×720 only; ground-ON layout PASS; stale-answer matrix 16/16 PASS and controls PASS; P7b PASS; F-2 PASS; stale-Execute PASS; real Execute PASS; no CSP violation; no 5xx; no horizontal overflow.

**Two things I must be explicit about:** (a) of the 12 FAIL lines in the first 1280×720 run, **10 were my own harness defects** (P3 ×1, P4 ×6, the P7 follow-up, the P9 abort, the PZ console count) and **2 were real findings** (F-01 in P1b, F-02 in P2); of the 10 FAIL lines in the first 1920×1080 run, **9 were harness defects** and **1 was real** (F-02; F-01 does not occur at that size). (b) The strict-tolerance P4 composite check shows 1 pixel at 4.08 (1280×720) and 380 pixels with errors up to 29 (1920×1080), all adjacent to overlays, analysed in §5.3 rather than hidden.

---

## 7. NOT TESTED

* **AC22 as written** (independent GPT-6.1 Sol review); owner playtest; owner acceptance; integration.
* **Lock collision / the actual D-091 grant** (AC17, AC18): only the spec's proposed allowlist.
* **Dynamic test of a stale LOCKED answer:** static argument only (F-06).
* **Any viewport other than 1280×720 and 1920×1080** for acceptance; below 1024 px layout only sampled (informational).
* **Browser zoom or device pixel ratio other than 1**, other browsers, touch, high-contrast mode, reduced motion, and key-repeat behaviour on the rank buttons.
* **Screen-reader output** (announcements were read from the DOM, not heard).
* **Unity, Windows builds, save/load, `main` integration.**
* `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh` (not run; the `ui-smoke*.sh` harnesses' own safety checks were).
* **Whether the committed `scenario-owner-ux.mjs` matches the copy that produced the writer's recorded runs:** I verified only that its header (lines 1–4) is comments; I cannot check the run copy.
* **L = 0 on a real cell:** no real cell is zero in years 0–3; the L = 0 path was exercised only by injection.
* **The writer's own screenshots at pixel level:** I checked size, SHA-256 prefixes and the capability scan, and compared one (the initial map, 342,424 bytes) to my own render of the same state (identical size), but did not diff PNGs.
* **`ui-smoke*` checks after the J99 crash point:** they do not run at BASE or HEAD.

---

## 8. Evidence-integrity statement

* **What is mine.** Every PASS/FAIL/NOT TESTED in §4 and §6 rests on commands I ran in my own worktrees, with my own builds, fresh servers and my own browser probe. I did not reuse the writer's logs, JSON or screenshots as evidence. I read the writer's driver and handoff as claims to test.
* **What I confirmed about the writer's committed evidence** (read-only checks): 12 PNGs, **4,838,103 B**, largest **762,902 B**, against AC-9 caps of ≤ 0.9 MB per image and ≤ 5 MB per set (compliant under both decimal and binary readings; the set is 161,897 B under the decimal cap). SHA-256 prefixes of `07-ground-inspection-1280x720.png` (`58f31fb2`), `07-ground-inspection-1920x1080.png` (`8b9eb359`), `10-ground-mode-crown-selects-1280x720.png` (`9c7ae4a9`) and `01-initial-map-1280x720.png` (`3265c715`) match the handoff. **Zero** `/s/<64 hex>/` capability paths in any committed non-PNG file at HEAD; the only 64-hex strings are the four anchors and the P0 hash. The committed results JSON files each record 87/87 PASS with no abort (that is the writer's claim; I did not rely on it).
* **My own evidence storage.** Raw run logs, 44 PNG screenshots and a `SHA256SUMS` (55 lines) are **off-repo** at `/home/jer/.local/share/ccf-evidence/review-r2-owner-ux-20261123/`; the checksum list is also committed as `results/OFFREPO-SHA256SUMS.txt`. The probe sources, helper, scripts, redacted result JSON for every pass and the smoke, build and Core logs are committed under `review-probe-r2-20261123/`. Caption warning for my screenshots: in `P4-light-area-b-injected-*.png` the single bright peach block is **my injected L = 1 test cell**, not engine data. Before staging I scanned every committed file: no `/s/<64hex>/` capability path (the smoke logs print the literal placeholder `/s/<capability>/`).
* **Capability handling.** Each server's capability URL was written only to a scratch file outside the repo and never printed; the probe redacts it from every output line.
* **Protected resources.** Listeners `127.0.0.1:33767`, `:43649`, `:46593` were alive before and after, and never bound by me (my servers took OS-assigned ports and were stopped by verified command line). I did not touch the other sessions' many `dotnet` listeners.
* **What I did not modify.** The candidate branch (still `5f3b5bf3` on origin at the end), `main`, the sibling worktrees `CCF-Multi-Stage4OwnerUX-R2-20261123`, `…-Independent-20261123` and `…-ReviewBase-20261123` (HEADs and clean status verified unchanged), and any tracked file of the candidate. My review worktree stayed clean (0 status entries) at the reviewed SHA. Worktrees I created and left in place: `CCF-Multi-Stage4OwnerUX-R2Review-20261123` (review), `…-R2Review-BASE-20261123` (BASE), `…-R2Review-F0DB-20261123` (pre-R2 page with HEAD's test script copied in for the negative control; modified locally on purpose), `…-R2Review-Docs-20261123` (this deliverable).
* **Limits.** The WSL ↔ Windows setup means node and Edge ran on Windows against servers in WSL; results are for that stack only.

---

## 9. Disclosed items, judged (not re-litigated)

* **UX-08 crown-wins (owner decision).** Implemented faithfully. Crown selection beats ground inspection, a rank button beats a crown, a modal beats everything (zero requests), and only a hit-test that finds *no crown* reads the ground, without changing selection or draft. Communication is honest: the status line says "crowns still select trees", the panel hint says clicking a crown still selects it, the cursor is a crosshair only over open ground. I independently measured the consequence: **Area A 2 of 144 cell centres (1.4 %), Area B 22 of 239 (9.2 %)** have no crown, exactly the writer's figures. One small point: the always-visible legend note says only "Inspect ground reads the value at a spot", which is less precise than the status line when the mode is on.
* **F-2 Approve lock (owner decision).** Implemented faithfully and communicated honestly: with `stage4: null` the banner says "Prune orders cannot be shown and Approve and Execute are withdrawn until a re-read", Execute reads "… · re-read needed", Approve is disabled, no model-0 statements appear, a paired re-read restores both, and the server's stale-fence refusal still fails closed.
* **F-2 tests inject `stage4:null`.** I did the same (a one-shot view mutation), because the natural race is hard to trigger. My P9 therefore has the same limit; the server's real refusal paths were exercised for real (stale fence, stale Execute).
* **No real cell has L = 0 at year 0.** Confirmed for years 0–3 in both areas. The L = 0 behaviour was verified by injection only.
* **Below 1024 px.** Sampled informationally (F-04).
* **Browser driver header comment.** Lines 1–4 of the committed `scenario-owner-ux.mjs` are comments only, so they cannot change behaviour. I cannot verify the run copy (NOT TESTED).
* **PNG sizes.** Compliant (§8).
* **Authorship.** All **eight** commits are authored *and* committed as "DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>" with a `Co-Authored-By: Claude Opus 5.5` trailer, so the Git author is not the actual author. The handoff discloses this only in a bullet at R2.7 and only for "the R2 commits … the same as on `f0db8323`"; it does not say that the Phase A and R1 commits carry it too, and the header table says only "Sole implementer (Claude Opus 5.5)". The trailer is self-asserted and not verifiable from Git. **Adequate but narrow and buried (F-11):** one sentence in the header table covering all eight commits would be enough. For this document I used an explicit reviewer identity on the commit and left the repository's configured identity untouched.

---

## 10. Suggested repair scope (not applied; for the owner and implementer)

All inside `PlayerMultiApi.cs`'s `MultiAreaPage` strings and the browser/smoke tests, so no scope change:

1. **F-01:** keep the blocked-reason on a single line (ellipsis, full text in `title` and `aria-label`) or lower blocked-row height at `max-height: 820px`; add loaded-state fold checks (references whose rows include blocked ones) to the browser driver and the smoke witness. Re-test at 1280×720; 1920×1080 already passes.
2. **F-02:** cap or collapse `.batch-result` at short heights so rows stay reachable after Add.
3. **F-03 (optional):** consider showing the ground reading without scrolling the primary block.
4. **F-04:** state the minimum supported desktop width and make the fold hold there, or document that 1280×720 is the minimum.
5. **F-07 (optional):** print 4 decimals or "< 0.001" in the concise summary.
6. **F-11:** one header sentence on authorship for all eight commits.

A narrow re-review would suffice: the loaded-state sweep and the fold at the documented minimum width, plus the existing stale-answer matrix if the Escape/`mapPick` code changes. The stale-answer, ramp and ground-precedence results above need no repeat unless those areas change.

---

## 11. Reproduction

```bash
# WSL: fresh in-memory session on an OS-assigned port; capability URL goes to a scratch file only
bash scripts/server-start.sh <tag>
# Windows node v26 + Edge: sections P0 P0b P1 P1b P2 P3 P4 P4b P5 P6 P7 P7b P9 PN PZ
node review-probe.mjs <outDir> <fileHoldingBaseUrl> 1280x720 P0,P1,P2,P1b,P3,P4,P5,P6,P7,P9,PZ
bash scripts/server-stop.sh <tag>
```

Sections: P0 defaults/notice/nav geometry · P0b every nav button, dock vs stand, legend · P1 clean fold sweep · P1b loaded fold sweep · P2 ranks, strict request counting, Add · P3 Escape order, Close inspection preservation · P4 real-layer pixel ramp · P4b extreme L · P5 precedence and ground inspector · P6 ground-mode layout · P7 stale-answer matrix · P7b stale REFUSED fail-closed · P9 F-2, stale Execute, real Execute, years · PN narrow widths · PZ global.

# MULTI Stage 4 owner UX repair, Phase A: implementation result

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Spec | `specs/multi-stage4-owner-ux-20261123-v1/` at `b95a56bf` (APPROVED, Phase A only) |
| Role | Sole implementer (Claude Opus 5.5). **No self-review**: the independent reviewer is a different agent and thread |
| **Authorship (all commits)** | **Every commit of this lane from `267be9ff` (the first after `e7c1a57d`) to the final HEAD carries the repository's configured Git identity, `DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>`, as author and committer. That Git author is not the actual author: the implementer of all of them was Claude Opus 5.5, as each commit's `Co-Authored-By: Claude Opus 5.5` trailer says** (the trailer is self-asserted; Git cannot verify it) |
| Branch | `feat/multi-stage4-owner-ux-20261123-v1` |
| Base | `e7c1a57da519ed1ee03ccfbad240196a66df0905` (reviewed Stage 4 UI catch-up candidate; not moved) |
| Source/test HEAD (Phase A) | `9fed4eb91ed51522e3be2188c9f1539d06106d4d` |
| Phase A handoff HEAD | `9dd6f601100d55bfca12c1584afb6fc081ba5b7e` (kept in history; not amended) |
| Repair R1 source HEAD | `1a22d01c` (`fix(multi-owner-ux): UX-08 crown selection outranks ground inspection`), on top of `9dd6f601`; R1 docs HEAD `f0db8323` |
| Repair R2 source HEAD | `550e10b49fef5d2c3c71cbb8b299f86b9ecb96b8`, on top of `f0db8323`; R2 docs HEAD `5f3b5bf3` (reviewed; verdict NEEDS CHANGES, see R3) |
| **Repair R3 commits** | **`9d40b166`** page fix, **`9abf5c3e`** smoke witness R3a–R3j, **`4e3ba42b`** browser driver and loaded fold sweep, all on top of `5f3b5bf3` |
| **Final HEAD** | the commit that adds this R3 update (results, refreshed screenshots, handoff), directly on top of `4e3ba42b` |
| Worktree (R3) | native WSL worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R3-20261123`, local branch `feat/multi-stage4-owner-ux-20261123-v1-r3-local` created at `5f3b5bf3`, pushed to the lane branch as a fast-forward |
| Worktree (Phase A) | `C:\Users\hp\.t3\worktrees\CCF-HP\t3-8fc40b07-b22b8bb5869955c0` (Windows checkout of a worktree whose git dir is `/home/jer/Development/CCF-HP/.git/worktrees/t3-8fc40b07-b22b8bb5869955c0`; WSL path `/mnt/c/Users/hp/.t3/worktrees/CCF-HP/t3-8fc40b07-b22b8bb5869955c0`) |
| Phase B (UX-10) | **Not started.** It was not needed to satisfy Phase A |
| Integration | None. No merge, no rebase, no force push, `main` untouched |

Unity, Windows builds, save/load and `main` integration were **not** run and are **not** claimed. Everything here is about the standalone browser `/multi` page in `Tools/MultiRackSitkaP1Lab`.

## R3 — independent review findings (NEEDS CHANGES) and repairs (2026-11-23)

### R3.1 Where the findings came from

The first published independent review of this work is by **Claude Sonnet 5.5** (owner-designated substitute for GPT-6.1 Sol, whose run ended at a provider usage limit without publishing anything). It reviewed exactly **`5f3b5bf3`** and is published at review commit **`53796c58`** (`Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-INDEPENDENT-REVIEW-20261123.md` on `docs/multi-stage4-owner-ux-independent-review-r2-20261123-v1`). **Verdict: NEEDS CHANGES**, with one **MAJOR** finding (F-01: the fold breaks at 1280×720 once competitor rows are blocked), MINOR findings F-02, F-03 and F-04, and observations F-05 to F-11. Its PASS results on the R2 repairs (ground-mode layout, stale hit-test answers, light ramp) were not redone, except where R3 touched the code they cover (see R3.5). This R3 is the writer's repair. It is **not** a re-review, owner acceptance or integration.

### R3.2 Owner clarifications (canonical: spec commit `7b06da6f`, `specs/multi-stage4-owner-ux-20261123-v1/spec.md` section 0.2)

1. **1280×720 is the documented minimum supported desktop width.** The five competitor rows and Add must hold above the fold there, including rows whose competitor is blocked (queued, approved, Crop or felled). Below 1280×720 no fold requirement applies.
2. **F-03, an open-ground read scrolling the primary block out of view: keep the current behaviour.** Not changed in R3.

### R3.3 What changed, per finding (commit `9d40b166`, all inside the `MultiAreaPage` HTML, CSS and JS strings)

| Finding | Outcome | What changed |
|---|---|---|
| **F-01 MAJOR** | **Fixed** | A blocked row no longer gets a third line. Its **short reason opens the existing second (meta) line**: "Pending Fell · 19.1 cm · 15.5%". The short forms are Pending Fell, Approved plan, Retained Crop, Prune plan, Reference, Other area, Felled, Dead. That line now spans the ID and distance columns, so the reason is never cut. At 1280×720 a long meta tail can end in an ellipsis. The **full reason** ("Already pending Fell") is in the row's **tooltip** (`title` on the label: "Competitor 4 · MA-A-030 · Already pending Fell · 24.7 cm · 9.8%") and in **screen-reader text** inside the label ("Cannot be chosen: Already pending Fell."). The visible short form is `aria-hidden`, so a screen reader reads the full text once. Keyboard users also still reach the reason through the focusable map rank button, whose label and announcement carry the full reason (unchanged). Result: **blocked rows are exactly the height of free rows, 38 px at 1280×720 and 50 px at 1920×1080**, with no row shrunk, no row removed and the Add button unchanged (44 px / 48 px). Visible side effect for every row: the distance now sits on line 1 beside the ID instead of being centred across both lines |
| **F-02** | **Fixed** | On desktop (≥ 1024 px wide) the post-Add card now **takes the Add form's place** in the sticky dock instead of stacking on it. On short desktops (≤ 820 px high) it is **one line**: the title, **Details** (`aria-expanded`, opens the full lines on request) and **× Dismiss**. The title ends in an ellipsis when long ("Added 4 trees to …"); the full text is in its tooltip, under Details and in the live-region announcement. The card ends on **Dismiss**, on **any draft change** (new at every size: ticking or unticking a rank or checkbox clears the previous Add result) or on another selection, as before. The Add form then returns. Consequence a reviewer should know: after a *partial* Add, the remaining draft is visible in the rows, but the Add button is hidden until the player dismisses the card or changes the draft. Below 1024 px wide (stacked layout, static dock) the card and form still stack, as before |
| F-03 | **Unchanged** (owner decision R3.2) | — |
| **F-04** | **Fixed** (documented and held) | 1280×720 is stated as the minimum supported desktop size in the page's top HTML comment, the short-desktop CSS comment, and a new visible note in the **Details** pane: "Screen size: this page is laid out for desktop browser windows of at least 1280 × 720. There, a selected tree's reference, its ranks 1–5 and Add fit without scrolling. Smaller windows still work but may need scrolling." Below 1280×720 nothing is claimed or tested |
| **F-05** | **Changed** | `onEscape` now returns on an open dialog **before** it bumps `state.mapPick`. An Escape that only closes a dialog therefore no longer cancels a pending map click. Every other Escape still bumps first. The stale-answer guarantee is unchanged: such an Escape closes nothing else, so a late answer cannot reopen anything it closed. Re-proved below (R3.5) |
| F-06 | Unchanged | Observation only; fail-closed behaviour unchanged (no code touched there) |
| **F-07** | **Fixed** | The concise ground summary and its announcement read **"< 0.001"** for 0 < L < 0.0005 instead of "0.000". Technical details keep 4 decimals. Not reachable with real data today (smallest real L 0.00067) |
| F-08 | Unchanged | Disclosed precision limits (D-6) |
| F-09 | **Improved** | Spacing-only trims at short heights: competition top padding, heading margin, column-head padding, row gap .30 → .25 rem, reference-card bottom padding. **Rows are not shrunk.** See the margin numbers in R3.5 |
| F-10 | Unchanged | Pre-existing witness failures; the new R3 witness checks sit **above** the J99 crash point and run |
| **F-11** | **Fixed** | One sentence in the header table now covers authorship for every commit from `267be9ff` to the final HEAD |

### R3.4 The test-bias correction (the root cause of F-01 surviving two rounds)

**Plainly:** the old browser driver's `selectReference` only accepted a reference whose five competitors were **all unblocked**, and the smoke witness used clean sessions. Every fold check I ran in Phase A, R1 and R2 was therefore the favourable case. The blocked row's extra line was never measured, and I reported a fold PASS that did not hold in the normal working loop. The independent reviewer caught it.

What the tests do now (`4e3ba42b`, `9abf5c3e`):

- **`fold-sweep-owner-ux.mjs` (new)**: one fresh server, three session states. Each state sweeps **every living tree of both areas** (244), selected by exact ID with the real Enter key:
  - **clean**;
  - **queued**: 4 Fell orders added through the page's own Add, with real mouse clicks on map ranks 1–4 of MA-A-001, so trees with 1, 2, 3 and 4 blocked rows exist;
  - **approved + Crop**: plus Crop on MA-A-001's fifth competitor and on two competitors of MA-B-001, then Approve. This gives "Approved plan" and "Retained Crop" rows, and MA-A-001 then has **all five rows blocked**.
  
  In the clean and queued states each tree is also measured with **every free row ticked**. A tree passes only if, with no scrolling, the reference card, ranks 1–k and the rendered Add button are inside the pane and viewport, the last row ends above the dock, and every row is ≥ 36 px and the same height. The run **reports how many trees were swept and how many had blocked rows**, so coverage is visible. It also checks the post-Add card.
- **Main scenario**: the old helper is kept for steps that need five tickable ranks, with a comment saying it is the favourable case. A new `selectByIdWithBlocked` picks references with exactly k blocked rows. A new **R3 section** runs right after the first real Add: the post-Add card, Details, Dismiss, then references with **1, 2 and 4 blocked rows with every free rank ticked by the real mouse**. Also new: race (d) for F-05 and the F-07 check. 87 → **96** checks at 1280×720 and **95** at 1920×1080 (Details is short-height only).
- **Smoke witness R3a–R3j**: a loaded session made through the page's Add. It checks the two-line row structure for blocked and free rows (the stand-in has no layout, so the fold is structural there), the reason text, tooltip and screen-reader text, the card's Details and Dismiss, that a draft change ends the card, and F-05. The orders are removed again afterwards.
- **Negative controls, run by me against the unfixed `5f3b5bf3` page in a throwaway worktree:**
  - **Sweep, 1280×720**: clean 244/244 pass. **Queued: all 10 trees with blocked rows fail** (20 of 488 measurements; rows 65 px; worst rows-to-dock gap −211 px). **Approved + Crop: all 22 trees with blocked rows fail**, including MA-A-001 with 5 blocked (rows up to 79 px).
  - **Witness**: 130 pass / 17 fail. Every R3 check fails except R3b (the Add itself) and R3j (clean-up).
  - Committed as `fold-sweep-negative-control-5f3b5bf3-*`.

### R3.5 Re-proof (all re-run by me on the final files; nothing reused)

Environment unchanged: `/home/jer/.dotnet/dotnet` 8.0.134 only; `TMPDIR=/tmp/opus-ownerux-r3/tmp`; `.p0ref` copied read-only from `CCF-Multi-Stage4UiCatchup-20261123` and never rebuilt; Windows node v26.13.1 and headless Edge over CDP; every server on an OS-assigned port, one fresh server per browser run.

| Gate | Result |
|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | **0 warnings, 0 errors** |
| `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | **0 warnings, 0 errors** |
| Core suite | **184/184**, exit 0; anchors `7e3a2a86…1ece`, `a692a6d5…233d`, `814e8165…6287`, `8a926776…87fb` present and PASS |
| P0 pin `.p0ref/SitkaLab.Core.dll` | `9a99f781…34b3` before and after |
| `ui-smoke-multiarea.sh` (final witness) | **138 pass / 6 fail**: J04, J05, J09, J10, J11, J99, **all pre-existing**. The FAIL and TypeError lines are byte-identical to the reviewer's committed runs at both HEAD and BASE. +10 = R3a–R3j, all PASS. **No new failure** |
| `ui-smoke.sh` | **250 pass / 4 fail**: L01, L02, L06, J31, **all pre-existing** (FAIL lines byte-identical to the reviewer's HEAD and BASE runs). **No new failure** |
| Real Edge scenario 1280×720 | **96/96 PASS** |
| Real Edge scenario 1920×1080 | **95/95 PASS** |
| Real Edge fold sweep 1280×720 | **14/14 PASS** |
| Real Edge fold sweep 1920×1080 | **13/13 PASS** |
| CSP violations / exceptions / console errors / HTTP 5xx | 0 / 0 / 0 / 0 in all four browser runs |
| No sideways scroll | 1280 = 1280 and 1920 = 1920, start and end |
| Protected listeners 127.0.0.1:33767, :43649, :46593 | alive before and after; never bound |

**Loaded fold sweep** (244 living trees in every state; trees counted by how many of their five rows are blocked):

| State | Trees with blocked rows (by count) | 1280×720 | 1920×1080 | Unfixed `5f3b5bf3`, 1280×720 |
|---|---|---|---|---|
| clean | 0 | **244/244** (488 measurements incl. 244 ticked) | **244/244** | 244/244 |
| 4 queued | **10** (1: 5, 2: 3, 3: 1, 4: 1) | **244/244** (488 measurements) | **244/244** | **all 10 fail** |
| approved + Crop | **22** (1: 13, 2: 5, 3: 2, 4: 1, 5: 1) | **244/244** (244 measurements; editing locked) | **244/244** | **all 22 fail** |

Row heights in every state: **38 px** at 1280×720 and **50 px** at 1920×1080, blocked or not.

**Fold margin**, worst tree, same in every state:

| | 1280×720 | 1920×1080 | Unfixed page, 1280×720 clean |
|---|---|---|---|
| Rows → dock gap | **5 px** | **11 px** | 4 px |
| Free pane room below the dock | **18 px** | **83 px** | 8 px |

How to read these. The rows-to-dock gap (the reviewer's "4 px") is the dock's own top margin. It stays the same whenever the dock sits in normal flow, which is why it reads 5 px here. The number that decides when rows get covered is the free room below the dock: how much a row or the dock can grow before the sticky dock starts overlapping rows. At 1280×720 that rose from **8 px to 18 px**.

**F-02 post-Add card** (scenario and sweep, MA-A-001 right after adding ranks 1–4):

| | 1280×720 | 1920×1080 |
|---|---|---|
| Last row bottom | 633 | 832 |
| Card | top 644, height **36** (one line) | top 852, height 95 |
| Dock | 638–680 | 843–947 |
| Add form | not rendered | not rendered |
| Rows above the card | **all five** | **all five** |

At 1280×720, Details opens the card to 104 px and closes it back to 36 px. × Dismiss: card gone, Add back above the fold, focus on the heading, nothing sent. A draft change ends the card ("Add selected 1 to Fell plan").

**Worst cases with the real mouse** (scenario, every free rank ticked):

| Reference | Blocked rows | Add label | Fold at 1280×720 |
|---|---|---|---|
| MA-A-003 | 1 | "Add selected 4" | rows end 633, dock top 638, Add bottom 688 |
| MA-A-002 | 2 | "Add selected 3" | same |
| MA-A-001 | 4 | "Add selected 1" | same |

All three PASS at both viewports. The sweep's MA-A-001 with **all five blocked** (4 approved + 1 Crop) also PASSES at both.

**R2 gains re-confirmed (same scenario runs, both viewports):**
- **Ground-ON layout**: crown click; all five ticked; scrolled inspector → reset; open-ground read then crown; ground OFF scroll reset.
- **Stale-answer races**: (a) Escape, (b) newer selection and (c) area switch all still PASS. The matrix was re-run because R3 changed `onEscape`. **(d) new, F-05**: Escape that only closes the Year+1 dialog while the hit-test answer is held. Afterwards the late answer selects MA-B-044 as a normal click, and the server is unchanged by all four races.
- **Light ramp**: values unchanged (pixels 0 / 0 / 8 / 26 / 227 / 496; L = 1e-200 → `9.000e-131`; continuous at 0).
- **Ground precedence**: crown wins; rank toggles only its ID; a modal swallows clicks with no request; Escape order unchanged.
- **F-07**: an injected L = 0.0004 reads "Light index < 0.001" (Technical details `0.0004`); the next real read shows 0.302 again.

**Run history, for honesty (none of these runs is committed evidence):**
1. My first sweep reported every *ticked* measurement as failing. That was a harness bug: it clicked several checkboxes from one list, and each click re-renders the rows. The layout values in those records passed. Fixed by re-querying after each tick.
2. My first F-07 browser check injected into a view that the click's own hit-test answer then replaced. The injection was moved to that answer.
3. Two background runs (an early negative control and an early 1920 sweep) died because I stopped their servers while they were still running.
4. The first witness runs read the live region before `announce()` had set it, and read rows during the post-Add re-read. Waits added.

Every committed result comes from the final files listed above.

### R3.6 Screenshots

Refreshed because their visible behaviour changed (the new row layout; 07-1920 shows the rows), plus two new clipped Tree-pane shots:

| File | State | Bytes | sha256 |
|---|---|---|---|
| `02-selected-five-ranks-1280x720.png` | new row layout (distance on line 1, meta line spanning) | 341,669 | `bb85d4bc…` |
| `03-ranks-chosen-synced-1280x720.png` | same | 345,070 | `38e9c7c7…` |
| `07-ground-inspection-1280x720.png` | same state as R2; retaken with the R3 page | 303,947 | `526f7a52…` |
| `07-ground-inspection-1920x1080.png` | rows visible in the new layout | 752,308 | `d0d7529a…` |
| `10-ground-mode-crown-selects-1280x720.png` | new row layout | 318,049 | `ee415d95…` |
| `11-blocked-rows-1280x720.png` (**new**, Tree pane only) | MA-A-001 with four "Pending Fell" rows at full height, the free rank 3 ticked, "Add selected 1 to Fell plan" above the fold | 40,005 | `e3b73ee8…` |
| `12-after-add-card-1280x720.png` (**new**, Tree pane only) | right after Add: five rows above the one-line card "Added 4 trees to … · Details · × Dismiss" | 40,928 | `5d4a329d…` |

Set: **14 PNGs, 4,916,524 B, largest 762,902 B** (AC-9: ≤ 0.9 MB per image, ≤ 5 MB per set). The 1920×1080 versions of 11 and 12 (50,894 B and 52,009 B) are **not** committed because they would take the set over 5 MB. They, every other R3 PNG, all raw run logs, the full per-tree sweep records, the build, Core and smoke logs, and the negative-control logs are off-repo at `/home/jer/.local/share/ccf-evidence/multi-stage4-owner-ux-repair3-20261123/` (`SHA256SUMS`, 47 files). Committed results: `browser-results-*.json`, `browser-run-*.txt`, `fold-sweep-*.json`, `fold-sweep-run-*.txt` and the negative-control pair in `browser-driver-owner-ux/`. **Zero** `/s/<64 hex>/` capability paths and zero 64-hex tokens in that folder (checked).

### R3.7 Proposed follow-up — NOT done

**Map-letter badge beside the competitor ID.** A small P / A / C / X / D badge, the letters the map legend already uses, beside each blocked competitor's ID, next to the short reason text. It would tie the list to the map and make blocked rows easier to spot at a glance. **The owner chose not to include it in this repair.** It is not required by any acceptance criterion, it adds a new visual element to a repair that is about height, and every extra element is another thing the reviewer must re-check. Words remain the carrier in any case: seven reasons cannot each get a legible icon, and the page's own rule is that a state is never carried by colour or symbol alone. Proposed only; not implemented.

### R3.8 Still not verified

- An independent re-review of R3; owner playtest and acceptance; AC22 as written; integration.
- Unity, Windows builds, save/load.
- `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`.
- **Screen-reader output**: the screen-reader text and announcements were read from the DOM, not heard.
- **Viewports**: browser zoom, device pixel ratio other than 1, other browsers, touch, high contrast; any viewport other than 1280×720 and 1920×1080. Below 1280×720 nothing is claimed.
- **Data coverage**: a *real* partial or failed Add at short heights (the card's partial path was not provoked in a browser); a Felled or Dead competitor row in the sweep (only Pending, Approved and Crop reasons occurred); the L < 0.0005 path on real data (injection only).
- **Locks**: lock collision / the D-091 grant (not available to me). I did not edit the coordination board.
- A throwaway worktree for the `5f3b5bf3` negative controls was created at `/tmp/opus-ownerux-r3/negctl-5f3b` (its smoke script replaced locally) and removed afterwards.

## R2 — independent-review findings and repairs (2026-11-23)

**Where the findings came from, plainly.** The two defects and the ramp note below came from an independent adversarial review run by **GPT-6.1 Sol** against `f0db8323`. That run was **terminated by a provider usage limit** and **never produced a published review or a verdict**. They are therefore **unverified observations** that the task packet asked me to fix and re-prove. They are **not an approval** of anything, and nothing in this section should be read as a completed independent review. An independent review of R2 is still outstanding.

Same writer (Claude Opus 5.5), same branch, new commits only on top of `f0db8323` (no amend, rebase or force push; `e7c1a57d` and `f0db8323` stay in history). Worktree for R2: a native WSL worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R2-20261123`, created detached at `f0db8323` (the lane branch was still checked out in the two earlier T3 worktrees, which I did not touch) and pushed to the lane branch as a fast-forward. Being native WSL, it needed none of R1's CRLF or `GIT_DIR` workarounds. TMPDIR `/tmp/opus-ownerux-r2/tmp`. A setup slip, recorded for completeness: my first `git worktree add` lost its path variable in the `wsl.exe` layer and created an empty, clean, detached worktree named `f0db8323…` inside the main checkout at `main`'s HEAD. I removed it at once with `git worktree remove` (0 changes in it; nothing else touched).

Write scope respected: `src/Ui/PlayerMultiApi.cs` (inside the `MultiAreaPage` HTML, CSS and JS strings only), `tests/ui-smoke-multiarea.sh`, and this folder. `src/Core`, `PlayerApi.cs`, `Program.cs`, `Assets`, fixtures, `.sln`, `.p0ref`, saves and `main` are untouched. `.p0ref` was copied read-only from `CCF-Multi-Stage4UiCatchup-20261123` and never rebuilt (`9a99f781…`, `sha256sum -c` OK before and after).

### R2.1 Defect 1 (MAJOR): the ground panel pushed ranks 1–5 below the fold

What was reported: at 1280×720, with Inspect ground on, the ground panel sat **above** the inspector, so a crown selection pushed the five competitor rows below the fold. Separately, after the inspector had been scrolled, selecting another tree left its primary controls off-screen. Both break AC01/UX-01.

What changed (commit `550e10b4`):

- **Placement.** A new empty slot `#ground-slot` sits in `#inspector-body` directly **after** the Add bar (`.add-dock`). `placeGroundPanel()`, called from `renderGround()` on every render, moves the existing `#ground-panel` there while a tree is inspected in the shown area, and back to the top of `#pane-tree` when nothing is inspected. Keyboard focus inside the panel is preserved across the move. In the slot the panel gets a slightly inset card style. The order is therefore reference → ranks 1–5 → Add → ground panel → everything else. The ground panel can no longer push the primary block down.
- **Scroll reset.** `chooseTree()` (every selection path: crown click, Trees table, competitor link, exact ID) sets `#pane-tree.scrollTop = 0` after rendering.
- **Ground mode stays obvious and keyboard reachable (UX-08 unchanged).** `aria-pressed` and "Inspect ground: On/Off", the status line, the crosshair, the × Stop button and Escape all work as before. A crown click in ground mode still clears only the ground read (`state.ground = null`); the mode stays on and the Light Guide is never switched off.
- **Focus rules.** Turning Inspect ground on **with a tree inspected** keeps focus on the pressed toggle and does not scroll the inspector. The panel is below the primary block, and the button state, the status line and the announcement say what a click now does. With nothing inspected, the heading is focused as before. An **open-ground read** focuses the ground heading (`preventScroll`) and scrolls the panel into view with `scrollIntoView({ block: 'nearest' })`, because the reading is what the user just asked for. The next tree selection resets the inspector to the top.
- Rows and the Add button were **not** shrunk or hidden. The measured sizes are unchanged from R1: rows 38 px high, 13.44 px text, Add 44 px at 1280×720; rows 50 px, 14.4 px, Add 48 px at 1920×1080.

Design choice for a reviewer to attack: after an open-ground read **while a tree is inspected**, the reading is scrolled into view, so the reference summary and the upper rank rows scroll out of view until the next selection. At 1280×720: scrollTop 369, and the sticky Add bar stays visible just above the reading (screenshot 07). I treated this as a ground interaction, not a tree selection. The AC01 guarantee (every tree selection shows the primary block without scrolling) holds.

### R2.2 Defect 2 (MAJOR): a late hit-test answer reopened a closed inspection

What was reported: Escape closed the inspection while `api/multi/hit` was pending; the late answer then re-applied its payload and reselected the tree.

Mechanism (commit `550e10b4`): a **map-selection generation counter** `state.mapPick`.

- `onMapClick` takes `const pick = ++state.mapPick` just before sending the hit-test. After `await call(...)`, it releases `state.busy` exactly as before and then checks `if (pick !== state.mapPick)`. If the generation moved, the answer is **dropped whole**, whatever its kind (ok, refused, guard, locked, transport). No `applyPayload`, no `chooseTree`, no draft clear, no ground read, no alert and no announcement.
- These advance the counter and so invalidate an in-flight attempt:
  - **every Escape key press**: at the top of `onEscape`, before the dialog and search-box early returns, so Escape stays immediately responsive and is never blocked;
  - **Close inspection** (`closeInspection`, also used by Escape);
  - **an area switch** (`showArea`, after its existing guards);
  - **any new selection** (`chooseTree`, which every selection path uses, including the map click's own success path after the check).
- Knock-on handled: while a hit-test is pending, `state.busy` is true. `refreshCompetition` already discards an answer that arrives while busy, so a newer selection made during the pending hit could be left without competitors. When a stale answer is dropped and the current reference has no competitor report, the page now re-reads it (read-only, the existing `refreshCompetition`).
- Unchanged and intact: the `state.batch` / `refusedByLock` / `state.busy` entry guards; the refused / guard / locked / failed branches for a **current** answer (same messages); a refused or locked hit-test still never falls through to a ground read. Busy semantics are unchanged: a second map click while a hit-test is pending is still ignored. A dropped stale `refused` answer means its newer view is not applied; the page keeps its current view, and the next command is fenced by the server as always.

### R2.3 Ramp decision (MINOR): the floor is removed, not kept

`LIGHT_ALPHA_FLOOR` is gone. Opacity is `LIGHT_ALPHA_MAX · L^LIGHT_EASE` (0.9 · L^0.65): exactly 0 at L = 0 (no element drawn), and continuous, strictly positive and strictly increasing for L > 0. The "never prints as 0" guarantee moved into formatting. `opacityText()` writes 4 decimals as before down to opacity 0.01, so strings for L ≥ ~0.0013 are byte-identical to R1, e.g. `0.0451`. Below that it writes enough decimals for three significant figures, e.g. `0.00226`, `0.00000127`. Only below 1e-100, which `toFixed` cannot express, it writes exponent form, e.g. `9.000e-131`. Edge parses that form: its computed value is not the default 1, and it serialises as `0`.

Observed with the page's own `lightPaint` (injected L values on one re-read of Area A, then a normal re-read restored all 144 real cells). Same at both viewports:

| L | Opacity written | Colour | Summed RGB change over the floor colour (pixels) |
|---|---|---|---|
| 0 | not drawn | — | **0** |
| 1e-200 | `9.000e-131` | `#7f3b4a` | (not drawn as a swatch) |
| 1e-9 | `0.00000127` | `#7f3b4a` | (not drawn as a swatch) |
| 0.0001 | `0.00226` | `#7f3b4a` | **0** (below one 8-bit step; R1 wrote the same value, the floor only mattered below L ≈ 7.6e-7) |
| 0.01 | `0.0451` | `#88414e` | **8** (unchanged from R1) |
| 0.05 | `0.1284` | `#994d54` | **26** |
| 0.5 | `0.5736` | `#e49383` | **227** |
| 1 | `0.9000` | `#ffd9bd` | **496** |

Invariants checked: L = 0 draws nothing (143 of 144 cells drawn after one cell was set to 0); every L > 0 has a positive opacity text that never reads "0"; the values are strictly increasing over the eight chosen L; at L = 1e-200 the value equals 0.9·L^0.65 to three significant figures, so there is no floor. As in R1 (D-6), positive L below about 7e-4 changes no 8-bit pixel even though the written opacity is positive.

### R2.4 Tests added

- `tests/ui-smoke-multiarea.sh`, checks **R2a–R2j** in the existing page witness (numbered R2.. so the J IDs and the baseline failure IDs do not move). They sit after the F-2 block and before the pre-existing `reason-choice` crash point, so they run. The DOM stand-in gained the standard `prepend`, `firstElementChild` and a no-op `scrollIntoView`; without them the page's first render threw in the stand-in.
  - R2a/R2b: scroll reset after a table selection and after a crown click.
  - R2c/R2d/R2e/R2f: panel placement and focus rules.
  - R2g/R2h/R2i: Escape during a **held** hit-test (the witness's existing `arm(…, 'hold')` fault). Expected: no reselection, no announcement, no competitor read, no alert.
  - R2j: a newer selection during a held hit-test keeps its selection and its draft.
  - **Negative control:** against the `f0db8323` page (same witness), **8 of 10 fail** (R2a–R2e, R2h, R2i, R2j). For example, the late answer reselected `MA-A-086` and announced "Selected tree MA-A-086 in Area A.", and scrollTop stayed 420. R2f and R2g are non-regression guards and pass on both.
- Real-browser driver `scenario-owner-ux.mjs`: one new check in section G (where an open-ground read appears) and a new **R2 section** (17 checks) between G and H. The R1 screenshot 10 moved from section G into R2, where it shows the fold. Section G's checks and every other check are unchanged. 70 → **87** checks per viewport.

### R2.5 Re-proof (all re-run by me in the R2 worktree; nothing reused from earlier runs)

| Gate | Result |
|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln` (8.0.134, `/home/jer/.dotnet/dotnet`) | **0 warnings, 0 errors** |
| `dotnet build src/Ui/SitkaP1.Ui.csproj` | **0 warnings, 0 errors** |
| Core suite | **184/184**, 0 failed; anchors F-01 `7e3a2a86…`, F-02 `a692a6d5…`, S4 V-02 `814e8165…`, S4 R-02 `8a926776…` present and PASS; P0 trust checks PASS |
| `ui-smoke-multiarea.sh` | **128 pass / 6 fail**: J04, J05, J09, J10, J11, J99, **all pre-existing** (identical IDs to the 118/6 baseline; J99 is the same `byId('reason-choice').value = 'other'` TypeError, now at witness line 539 because the R2 block was inserted above it). +10 = the new R2a–R2j, all PASS. **No new failure** |
| `ui-smoke.sh` | **250 pass / 4 fail**: L01, L02, L06, J31, **all pre-existing** (identical to baseline; J31 is the same `getBoundingClientRect` witness crash). **No new failure** |
| Real headless Edge 1280×720, fresh server | **87/87 PASS** (+3 INFO lines) |
| Real headless Edge 1920×1080, fresh server | **87/87 PASS** (+3 INFO lines) |
| CSP violations / exceptions / console errors or warnings / HTTP 5xx | 0 / 0 / 0 / 0 at both viewports |
| No sideways scroll | 1280 = 1280 and 1920 = 1920, start and end |
| Protected listeners 127.0.0.1:33767, :43649, :46593 | alive before, during and after; never bound. Lane servers took OS-assigned ports and were stopped afterwards |

New regression checks, observed numbers (pane = `#pane-tree`; at 1280×720 the pane spans y 292–712 and the viewport is 720; at 1920×1080 it spans y 326–1068 and the viewport is 1080):

| Check | 1280×720 | 1920×1080 |
|---|---|---|
| Ground ON, nothing inspected | panel first child of the Tree pane, heading focused | same |
| **Ground ON + crown click** (MA-B-003) | scrollTop 0; reference top 293; rows 434–643; Add 654–698; ground panel starts at 712, below the Add bar (bottom 704), in `#ground-slot`; mode On, Light Guide On | scrollTop 0; rows 555–832; Add 881–929; panel below the Add bar |
| Same, **all five ticked** | "Add selected 5 to Fell plan", rows 434–643, Add bottom 698, scrollTop 0 | same layout, Add bottom 929 |
| **Scrolled inspector** (scrollTop 708) + another crown, ground ON | scrollTop 0, rows and Add as above | scrolled 584 → 0 |
| Open-ground read scrolled the pane (scrollTop 420) + crown click | scrollTop 0, primary block back | 324 → 0 |
| Scrolled (526) + another crown, **ground OFF** | scrollTop 0, panel hidden, Light Guide still On | 416 → 0 |
| Open-ground read with a tree inspected | panel in slot, top 343 ≥ Add bar bottom 335, inside the pane, heading focused, scrollTop 369 | top 730 ≥ 722, scrollTop 244 |
| **Escape while the hit-test answer is held** | inspection closed at once; after release: still closed, no rank layer, announcement unchanged ("Closed the inspection of MA-B-005…"), 0 competitor reads, no alert; the late topmost MA-B-044 ignored | same |
| **Newer selection** (exact ID MA-B-003 + Enter) while held | MA-B-003 kept; late answer (MA-B-044) ignored; its five competitors loaded after the drop | same |
| **Area switch** while held, with a 1-tick draft in B | page stays on Area A; back in B: MA-B-003 and "Add selected 1 to Fell plan" intact | same |
| Server view across the three races | unchanged | unchanged |

**How the race was made deterministic:** the driver wraps `window.fetch` in the page. The real `api/multi/hit` request is sent and answered by the real server (it is read-only), and the wrapper then holds that answer inside the page (`await` on a promise) until the driver calls `__releaseHit()`. The driver waits for `__hitHeld === n`, performs the interfering action (real Escape key, real Enter in the exact-ID box, real click on the Area A tab), checks the immediate state, releases, waits 700 ms plus idle, and checks again. The smoke witness does the same with its existing `hold` fault, which holds the request before it is sent.

Earlier ground-mode checks, re-confirmed at both viewports (same numbers as R1):

- A crown click with Inspect ground on selects MA-B-044.
- An open-ground click reads "ground at X 38.8, Z 8.8" and leaves the selection, draft and server unchanged.
- A rank click changes only `MA-B-010`.
- With a modal open, a map click is swallowed (`posts: []`).
- Escape order holds: dialog, then ground mode (Light Guide stays on, 239 cells, inspection open), then inspection.

### R2.6 Screenshots

Refreshed because their visible behaviour changed:

| File | Change | Bytes | sha256 |
|---|---|---|---|
| `07-ground-inspection-1280x720.png` | the open-ground reading now appears **below** the Add bar, scrolled into view | 303,919 | `58f31fb2…` |
| `07-ground-inspection-1920x1080.png` | same at 1920×1080 | 752,456 | `8b9eb359…` |
| `10-ground-mode-crown-selects-1280x720.png` | ground ON + crown click: reference, ranks 1–5 and Add fully visible; the ground panel is no longer above them | 318,454 | `9c7ae4a9…` |

The other nine committed PNGs are unchanged; their states differ only by bytes or by scenario order, not by behaviour. Set: 12 PNGs, **4,838,103 B**, largest 762,902 B (AC-9: ≤ 0.9 MB per image, ≤ 5 MB per set). `10-ground-mode-crown-selects-1920x1080.png` (763,953 B) is **not** committed because it would take the set over 5 MB. It and every other R2 PNG, both raw run logs, the Core log, the final smoke logs and the negative-control smoke log are off-repo at `/home/jer/.local/share/ccf-evidence/multi-stage4-owner-ux-repair2-20261123/` (`SHA256SUMS`, 28 files, capability redacted). Committed results and logs contain **zero** `/s/<64 hex>/` capability paths and zero 64-hex tokens (checked).

### R2.7 Not verified / notes

- **Not verified:** owner playtest; any independent review of R2 (the GPT-6.1 Sol run above did not complete); Unity, Windows builds, save/load; `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`; screen-reader output (announcements were read from the DOM, not heard); narrow layouts below 1024 px for the new placement (the side panel is not a scroll container there; no fold requirement applies). A stale **refused** or **locked** hit-test answer is dropped by the same single check before the outcome branches. No test injects a refused or locked hit answer during a race; the current-answer branches are byte-identical to R1.
- The committed `scenario-owner-ux.mjs` differs from the copy that ran only by the 3-line header comment added afterwards (checked with `diff`).
- The R2 commits carry the author identity configured in this repository's Git config (`DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>`, the same as on `f0db8323`), with a `Co-Authored-By: Claude Opus 5.5` trailer. The writer of R2 is Claude Opus 5.5.

## R1. Bounded repair after the owner's decisions (2026-11-23)

Same writer (Claude Opus 5.5), same branch, new commits only on top of `9dd6f601`; no amend, rebase or force push; the reviewed base `e7c1a57d` did not move. Worktree for R1: `C:\Users\hp\.t3\worktrees\CCF-HP\t3-51cf0f49-fc6c0cea68fda749` (git dir `/home/jer/Development/CCF-HP/.git/worktrees/t3-51cf0f49-fc6c0cea68fda749`).

### R1.1 Owner decision on UX-08 (precedence): the literal spec order, code changed

> modal dialog **>** rank toggle button **>** tree crown selection **>** ground inspection (only when ground mode is active)

The Phase A interpretation D-1 (ground first while Inspect ground is on) was **rejected by the owner** and is removed. What `onMapClick` in `src/Ui/PlayerMultiApi.cs` now does (commit `1a22d01c`):

```js
async function onMapClick(event) {
  if (state.batch) { waitForBatch(); return; }
  if (!state.view || refusedByLock('Map selection') || state.busy) return;
  ...
  const outcome = await call('api/multi/hit', { fence: currentFence(), areaId: area.areaId, u: point.x, v: point.y });
  ...
  if (candidates.length === 0) {
    state.overlap = null;
    applyPayload(outcome.payload, false);
    // Re-checked after the await: Escape or Stop may have ended Inspect ground while the hit-test was in flight.
    if (state.lightOn && state.groundMode) { inspectPoint(area.areaId, point.x, point.y); return; }
    announce('No crown at this point — the selection is unchanged.');
    return;
  }
  if (state.lightOn && state.groundMode) state.ground = null; // a crown hit is a selection, not a ground read
  ... chooseTree(area.areaId, candidates[0].treeId, false);   // unchanged normal selection
```

- An open `<dialog>` is modal, so it takes every click; rank buttons are on a sibling HTML layer and never reach the map handler (unchanged).
- Every other map click, with Inspect ground on or off, goes to the **existing** read-only `api/multi/hit` path behind the existing `state.batch` / `refusedByLock` / `state.busy` guards (no new endpoint, no new hit-test, no double send). A crown there is selected exactly as a normal selection.
- Only when Inspect ground is on **and** the hit-test reports no crown is the local ground cell read: no further request, no selection change, no draft change (Phase A's ground-read semantics are unchanged).
- A refused, guarded, locked or failed hit-test shows its usual message and **never** falls through to a ground read.
- Truthful affordance: the status line reads "Inspect ground is on: click open ground to read its light · crowns still select trees · Esc stops"; the panel hint reads "Click open ground (a spot with no crown over it) inside the area to read its modelled ground light. Clicking a crown still selects that tree."; the announcements say the same; the crosshair cursor is shown only over open ground and crowns keep the pointer. The button keeps its `aria-pressed` and "Inspect ground: On/Off" text and stays keyboard reachable.
- Not changed: crown geometry, cell values, ranking, the Light Guide mapping, the hit-test tolerance, routes, payloads, any C#.

### R1.2 Owner decision on F-2: Approve lock accepted, no code change

The owner **accepted** that Approve, Execute and the Execute confirmation are all disabled/withdrawn while the read-out is unavailable (Phase A D-2). Behaviour is unchanged and re-verified (smoke F2a–F2h and the browser F-2 checks below).

### R1.3 UX RISK (accepted by the owner)

**Crown-wins precedence leaves ground inspection limited to open ground.** Area A is ~96% canopy cover (the area card: 96%, open 33 m² of 900 m², ≈ 3.7% open area; the owner's figure ~4%). Measured in this run with the server's own hit-test at every light-cell centre (read-only): **Area A 2 of 144 cell centres (1.4%)** have no crown; Area B 22 of 239 (9.2%). In practice, in Area A almost every ground click selects a tree instead, and a ground value under a crown cannot be read by clicking. Two further consequences a reviewer should know: (a) a click a pixel inside a crown edge selects that tree (observed: a point 0.02 m outside MA-B-006's crown radius rounded onto it and selected MA-B-006); (b) because a crown click in ground mode is a normal selection, choosing a different tree clears the previous reference's unsent draft, as it always does outside ground mode (observed and announced: "Cleared 2 unsent draft choices…"). The rule was implemented as directed; I did not substitute another rule. If the owner later wants ground values under crowns, that needs a separate decision (not proposed here).

### R1.4 Re-run evidence (observed by me in the R1 worktree; nothing reused from the Phase A browser run)

| Check | Result |
|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln` (8.0.134, `/home/jer/.dotnet/dotnet`) | **0 warnings, 0 errors** |
| `dotnet build src/Ui/SitkaP1.Ui.csproj` | **0 warnings, 0 errors** |
| Core suite | **184/184**, 0 failed (anchor / P0 trust / source-identity checks all PASS) |
| `.p0ref/SitkaLab.Core.dll` (copied read-only from the reviewed candidate lane, never rebuilt) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, `sha256sum -c` OK |
| `ui-smoke-multiarea.sh` | **118 pass / 6 fail**: J04, J05, J09, J10, J11, J99, identical to the baseline; F2a–F2h all PASS. **All 6 pre-existing**: J99 is the same witness crash `byId('reason-choice').value = 'other'` (TypeError on null), the others the same #885-era selectors |
| `ui-smoke.sh` | **250 pass / 4 fail**: L01, L02, L06, J31, identical to the baseline. **All 4 pre-existing** (J31 = the same `getBoundingClientRect is not a function` witness crash) |
| Real headless Edge, 1280×720, fresh server | **70/70 PASS** (+ 2 INFO lines) |
| Real headless Edge, 1920×1080, fresh server | **70/70 PASS** (+ 2 INFO lines) |
| CSP violations / exceptions / console errors / HTTP 5xx | 0 / 0 / 0 / 0 at both viewports |
| No sideways scroll | 1280 = 1280, 1920 = 1920 (start and end) |
| Protected listeners 127.0.0.1:33767, :43649, :46593 | alive before, during and after; never bound (each lane server took an OS-assigned port) |

Ground-mode precedence checks, both viewports (same numbers at both):

| Check | Observed |
|---|---|
| Inspect ground ON, click a crown | MA-B-044 (the hit-test's topmost there) **selected**; ground panel "Area B · choose a spot", no ground mark, Technical details hidden, mode still On; server view unchanged |
| Inspect ground ON, click open ground (hit-test: no crown; u 3.6, v 46.4, 1.84 m from the nearest crown edge) | ground inspector "Area B · ground at X 38.8, Z 8.8", mark drawn; reference stays MA-B-001, Add label stays "Add selected 1 to Fell plan", rows and rank states identical, server view identical |
| Inspect ground ON, rank button | only MA-B-010 changed (`changed: ["MA-B-010"]`); reference and ground reading unchanged |
| Inspect ground ON, modal open (Year+1 consent), click the map | dialog stays open, no request sent (`posts: []`), ground reading and selection unchanged; Escape then closes only the dialog and Inspect ground stays On |
| Escape order | first Escape stops Inspect ground (Light Guide stays On, 239 cells, inspection of MA-B-044 stays open); after leaving, a crown click selects MA-B-002; the next Escape closes the inspection |
| Hint / cursor | status and panel text as in R1.1; `#map` cursor `crosshair`, crown cursor `pointer`; no "paused" wording |
| Outside the area | "no sample", nothing estimated (unchanged) |

UX-06 light ramp spot checks, both viewports, unchanged from Phase A: L = 0 / 0.01 / 0.05 / 0.5 / 1 → opacity **0 / 0.0451 / 0.1284 / 0.5736 / 0.9**, colours `#7f3b4a` / `#88414e` / `#994d54` / `#e49383` / `#ffd9bd`; pixels over the floor colour: summed RGB change **0 / 8 / 26 / 227 / 496**. Every cell with L > 0 is drawn with opacity > 0; monotonic over the real cells.

**Driver changes** (`browser-driver-owner-ux/scenario-owner-ux.mjs`, section G rewritten for crown-wins; 70 checks per viewport, was 63; INFO lines are not counted). Honest run history: the first 1280×720 attempt had 4 FAIL + abort because the driver picked an "open" cell centre 0.02 m outside a crown edge and the integer-pixel click landed on the crown (the page correctly selected it); the driver now picks the hit-test-empty cell centre with the most clearance. A 1920×1080 attempt then had 1 FAIL because the "click a crown after leaving ground mode" step clicked a crown centre covered by something other than that crown; the step now picks a crown point that is on `#crowns`, not under a rank button, with a known hit-test topmost. Both viewports were then re-run on fresh servers: 70/70 each. Those are the committed results.

**Screenshots changed by R1:** `07-ground-inspection-1280x720.png` and `07-ground-inspection-1920x1080.png` (new hint text; the read is now of open ground) replaced; `10-ground-mode-crown-selects-1280x720.png` added. Set: 12 PNGs, **4,827,430 B**, largest 762,902 B (AC-9: ≤ 0.9 MB each, ≤ 5 MB set). The other 1280×720 shots were retaken and are unchanged in visible behaviour, so the Phase A files stay (02 differs by 90 bytes, same state). All 20 R1 PNGs, both raw run logs, the Core and smoke logs are off-repo at `/home/jer/.local/share/ccf-evidence/multi-stage4-owner-ux-repair-20261123/` (`SHA256SUMS`, capability-redacted). No capability URL in any committed file (checked: zero `/s/<64 hex>/` matches).

**R1 environment notes.** Fresh T3 worktree; `git checkout --ignore-other-worktrees -B feat/multi-stage4-owner-ux-20261123-v1 9dd6f601` because the branch was still checked out (clean, idle) in the Phase A worktree `t3-8fc40b07`, which I did not touch; that worktree's checkout is now behind its branch ref and will show R1 as reverse changes if used. As in Phase A (L-4), Windows Git has `core.autocrlf=true`; I re-materialised `Tools/MultiRackSitkaP1Lab`, `900-ui-catchup` and the 3,350 CRLF text files under `Assets/ForestPrototype` with `core.autocrlf=false` (byte-identical to the commit; `git status` clean before editing). The Core tests' own `git` calls need `GIT_DIR`/`GIT_WORK_TREE` set in WSL. `TMPDIR` was `/tmp/opus-ownerux-repair/tmp`.

**Not verified in R1:** owner playtest; independent review; Unity, Windows builds, save/load; `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`; screen-reader output (announcement text was read from the DOM, not heard); any `main` integration.

## 1. Commits

| Commit | What |
|---|---|
| `267be9ff` | `feat(multi-owner-ux)`: UX-01..UX-09 and F-2 in `MultiAreaPage` (`PlayerMultiApi.cs`) |
| `0e17f641` | `test(ui-smoke-multiarea)`: F-2 unpaired read-out checks F2a..F2h in the existing page witness |
| `9fed4eb9` | `fix(multi-owner-ux)`: keep "Add selected N to Fell plan" on one line (found by the browser fold re-check) |
| `9dd6f601` | Browser driver for this task, results, screenshots, this handoff |
| `1a22d01c` | R1 `fix(multi-owner-ux)`: UX-08 crown selection outranks ground inspection (owner decision) |
| `f0db8323` | R1 driver section G, re-run results, changed screenshots, R1 update of this file |
| `550e10b4` | R2 `fix(multi-owner-ux)`: ground panel below the primary block, scroll reset on selection, stale hit-test answers dropped (`mapPick`), continuous ramp; witness R2a–R2j |
| `5f3b5bf3` | R2 driver section, re-run results, three refreshed screenshots, R2 update of this file (reviewed: NEEDS CHANGES) |
| `9d40b166` | R3 `fix(multi-owner-ux)`: blocked rows stay two lines (F-01), post-Add card replaces the Add form (F-02), 1280×720 minimum documented (F-04), dialog-only Escape keeps a pending map click (F-05), "< 0.001" (F-07), spacing (F-09) |
| `9abf5c3e` | R3 `test(ui-smoke-multiarea)`: loaded-session witness checks R3a–R3j |
| `4e3ba42b` | R3 `test(owner-ux driver)`: `fold-sweep-owner-ux.mjs`, scenario R3 section, race (d), F-07 check, `shotClip` |
| *(R3 docs commit)* | R3 results, sweep and negative-control results, refreshed and new screenshots, this R3 update |

## 2. Changed files and why

| File | Purpose |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` | The whole change, inside the `MultiAreaPage` raw strings (HTML, CSS, JS). No route, payload, fence or C# logic changed (the C# above `MultiAreaPage` is byte-identical) |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` | Adds an `unpaired` fault mode (the server's real answer with `stage4` set to null) and eight F-2 checks. Existing checks untouched |
| `Docs/Handoffs/.../900-ui-catchup/browser-driver-owner-ux/cdp-ux.mjs` | New. The committed `../browser-driver/cdp.mjs` pattern plus real key events, clipped screenshots and a tiny PNG decoder for pixel checks. The old driver is not modified |
| `Docs/Handoffs/.../900-ui-catchup/browser-driver-owner-ux/scenario-owner-ux.mjs` | New. 63 real-browser checks per viewport at Phase A; 70 after the R1 section G rewrite |
| `Docs/Handoffs/.../900-ui-catchup/browser-driver-owner-ux/browser-results-{1280x720,1920x1080}.json`, `browser-run-*.txt` | Raw results of the final runs (capability redacted, checked: zero 64-hex tokens) |
| `Docs/Handoffs/.../900-ui-catchup/evidence-owner-ux/*.png` | 12 screenshots after R1 (§7; 11 at Phase A) |
| this file | Handoff |

Not touched (verified with `git diff e7c1a57d HEAD`, empty): `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Core/**`, `fixtures/**`, the `.sln`, `.p0ref`, Unity `Assets/**`, packages, saves, the approved quartet, every historical handoff and the previous `browser-driver/`.

## 3. Requirement mapping (claimed by the implementer; observed results in §5)

| ID | Status | What was done |
|---|---|---|
| **UX-01** | DONE | Inspector reordered: compact reference block (eyebrow + ID + status chip on one line, beside **× Close inspection**; one line of DBH / Height / Crown r), then **Competitors for Fell** ranks 1–5, then the sticky Add bar. Fell reason, Crop, "Fell this tree instead", Pruning, remove/approved notes, competition details and the tree record now sit **below** the primary flow (`.tree-more`). Age moved to the Tree record (it was already listed there). Fewer than five: heading shows "N of 5", no padding rows. At 1280×720 the five rows end at y=651 and the Add button at y=706 (viewport 720), pane scrollTop 0, both with nothing chosen and with all five chosen. **R2:** every selection resets the Tree pane to the top, and with Inspect ground on the ground panel sits **below** the Add bar while a tree is inspected (R2.1). Measured with ground on: rows 434–643, Add 654–698, scrollTop 0, also with all five ticked and after a scrolled inspector |
| **UX-02** | DONE | The numbered rank tiles are real `<button>`s on a new HTML layer over the map (`#rank-layer`), each with `data-rank-toggle="<engine tree ID>"`, `aria-pressed`, ARIA label "Competitor k, tree ID, Fell draft", 28×28 px, 3 px focus outline. A click goes through the same `toggleDraft(treeId)` as the inspector checkbox, so both directions stay in sync by ID. Positions come from the display-only spread layout and are **never** used to find a tree. The layer is a sibling of the SVG, so a rank click never reaches the map handler: no hit-test, no reference change, no request. Blocked competitors stay focusable with `aria-disabled` and announce why. Chosen state = filled tile + tick corner + `aria-pressed` (not colour alone); the count is announced through the existing polite live region. No new route |
| **UX-03** | DONE | **× Close inspection** beside the ID; Escape runs the same action. Precedence: (1) an open `<dialog>` closes itself first and the handler does nothing in that key press; (2) a search box with text clears itself first; (3) **Inspect ground** stops; (4) the inspection closes. Closing clears that area's selection, its reference's unsent draft and batch result, and the enlarged focus layout; zoom, pan, area, Light Guide, Crop designations, queued/approved orders and year are kept; nothing is sent. With the Light Guide on, Escape never turns it off. **R2:** any Escape, Close inspection, area switch or new selection invalidates a pending map hit-test (`state.mapPick`), so a late answer can no longer reopen a closed inspection (R2.2) |
| **UX-04** | DONE | One quiet line "Synthetic Sitka experiment · 2 areas · Site unknown" + native `<details>` "Details". Inside, unchanged and verbatim: the full Stage 4 disclosure (`MultiDisclosureStage4`: synthetic not surveyed, not comparable, site unknown, connected woodland for growth/crowns, per-area density mortality / Work Plans / summaries, no transfer, no Unity parity), the chips with their full-statement tooltips, "Session only" and a link to the Details pane. The map-foot line no longer repeats "site UNKNOWN · growth and crowns use both areas…" (AC07) |
| **UX-05** | DONE | Dock 158 px wide (was 212). Compass 3×3 (N top-centre, W/E middle, S bottom-centre, empty centre), then one `− · Fit · +` row, then `Area A · Area B` on one row. 44 px targets kept; north-up, pan/zoom limits, Fit and area-switch handlers unchanged |
| **UX-06** | DONE | Light cells moved **under the crowns** (`#light-cells` now precedes `#crowns`, clipped to the area, no pointer events, no text). Per cell: `t = L^0.65`, opacity `= 0.9·t` (**R2:** no floor, continuous at 0. It is written with 4 decimals down to 0.01, then with three significant figures, so it never prints as 0; see R2.3. Phase A/R1 had a `0.0001` floor), colour linear through `#7F3B4A → #DA796D → #FFD9BD` by `t`. `L = 0`: no element. Old numbered band markers, `lc-1..5` fills and their halos (F-3) are gone. OFF by default |
| **UX-07** | DONE | Ground panel: level word (Very shaded / Low / Moderate / Bright; `L = 0` adds "no modelled light"), "Light index 0.037 on a 0–1 scale (modelled; 1 = unshaded)", "Canopy cover 96% of this cell under living crowns" (the engine's separate `geometricCoveredFraction`), "Shading trees 8". Closed disclosures: **Contributing trees (n)** with the IDs; **Technical details** with the index (4 dp), the cover fraction, column/row, grid size and clipped m², centre X/Z, cross-border trees, light model label + version, "Calibrated PAR no", the guide label and the not-calibrated caveat |
| **UX-08** | DONE, **owner-decided precedence (R1, `1a22d01c`)** | Precedence is the literal spec order: modal dialog > rank toggle button > tree crown selection > ground inspection (only while Inspect ground is on). With the Light Guide on or off and Inspect ground on or off, a map click goes to the server hit-test and a crown there is selected exactly as before. **Inspect ground** is an explicit `aria-pressed` toggle shown only with the Light Guide on; while on, a click where the hit-test reports **no crown** finds the engine cell containing the point and shows it (status line, outline on the chosen cell; crosshair over open ground, pointer over crowns). A refused/locked/failed hit-test never falls through to a ground read. A point with no cell says "No modelled ground sample… No value is shown or estimated". Leaving (button, **× Stop**, Escape) stops ground reads and leaves the Light Guide as it was. UX risk accepted by the owner: ground reads are limited to open ground (R1.3). **R2:** while a tree is inspected the ground panel sits below the primary block. Turning the mode on then keeps focus on the pressed toggle, and an open-ground read scrolls its panel into view (R2.1) |
| **UX-09** | DONE | The map legend strip says "Light Guide · Shaded [ramp] Bright · Inspect ground reads the value at a spot." The ramp is an SVG gradient sampled from the same mapping. No band labels, no "modelled light – not measured sunlight", no calibration text on the map. Source/version/uncertainty remain in the ground panel's Technical details and the Details pane (unchanged). The level is always given as text + number in the inspector |
| **F-2** | DONE; **Approve lock accepted by the owner (R1.2, no code change)** | `readoutState()` = `stage4` / `legacy` / `unavailable` (`stage4: null`). Unavailable shows neither the model 0 statements (edge exclusion, "Light … NOT MODELLED") nor the Stage 4 ones; notice, Details pane, history line and rack cards say the read-out is unavailable. **Approve and Execute are disabled** (Execute label "… · re-read needed"), the Work Plan pane shows a notice with a Re-read button, `openExecuteDialog` refuses with a Re-read alert, and an Execute confirmation open when an unpaired view arrives is withdrawn unsent (`samePrunes` now returns false without a read-out). Server-side stale-fence 409 untouched and re-verified |

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
| Real Edge, 1280×720 | n/a | 63/63 PASS at `9fed4eb9` (superseded: **70/70** at R1 `1a22d01c`, R1.4) |
| Real Edge, 1920×1080 | n/a | 63/63 PASS at `9fed4eb9` (superseded: **70/70** at R1 `1a22d01c`, R1.4) |
| Horizontal overflow (extra probe) | n/a | none at 1280×720, 1920×1080, 1024×768, 390×844 |
| CSP violations / console errors / exceptions / HTTP 5xx | n/a | 0 / 0 / 0 / 0 (both viewports) |
| Protected listeners 127.0.0.1:33767, :43649, :46593 | alive | alive, never bound |

**Smoke failure classification: all pre-existing, none caused by this change.** Multi-area: the page witness still reads #885-era elements that the page no longer has (`[data-competitor]`, `.competitor-rank`, `#reason-choice`); J04/J05/J09/J10/J11 fail on those selectors and J99 records the same crash, `byId('reason-choice').value = 'other'` (base: witness line 357; final: line 416, shifted only by the inserted F-2 block). Single area: files read by `ui-smoke.sh` are unchanged by this branch. The pass count rose 110 → 118 only because of the eight added F-2 checks; I numbered them F2a..F2h so the J numbering (and so the baseline failure IDs) did not move.

**Real-browser checks (each viewport, fresh session; Phase A run at `9fed4eb9`; the ground-mode part is superseded by R1.4).** Light Guide OFF by default; one-line notice ≤ 40 px and Details with every caveat verbatim; compass 3×3, `− Fit +` row, `Area A | B` row, dock ≤ 170 px with 44 px targets; zoom/pan N/W/E/S/Fit/area switch; UX-01 fold with 0 and with 5 chosen; five rank buttons carrying the engine's top-five IDs in order; ranks 1→5 each toggling exactly its ID and the matching checkbox, re-click clears, inspector→map sync, Tab focus with a visible 3 px outline, Space toggles with focus kept, announced count, **server view unchanged by toggles** (only `hit`/`competition` reads sent); Add queued exactly the chosen IDs; Escape closes the Year+1 dialog only, the next Escape closes inspection and restores normal rendering with zoom/pan/area/Crop/orders/year kept; × Close does the same and the cleared draft does not return; light cells = exactly the cells with `L > 0`, all with opacity > 0, under the crowns, no text, monotonic over real cells; ramp values at L = 0 / 0.01 / 0.05 / 0.5 / 1 = opacity 0 / 0.0451 / 0.1284 / 0.5736 / 0.9 with `#ffd9bd` at 1; **pixels** over the floor colour: summed RGB change 0 / 8 / 26 / 227 / 496; crown click selects with the Light Guide on; rank toggles with it on; Inspect ground On state; (R1: a click on a crown in ground mode now **selects** the tree and a click on open ground reads the cell, see R1.4); summary values equal the engine cell (index 3 dp, cover %, count); IDs and provenance behind closed disclosures; rank click in ground mode toggles and does not read ground; outside point says "no sample"; Escape stops ground mode first with the Light Guide still on; tree selection returns; no server side effect from Light/ground/inspection; Approve fells nothing; F-2 unavailable state (no legacy claims, Approve/Execute withdrawn), paired re-read restores Execute, an open confirmation is withdrawn unsent; real Execute fells exactly the confirmed trees; Year+1 advances. Full lines in `browser-driver-owner-ux/browser-run-*.txt`.

How the F-2 state was produced: the real server only sends `stage4: null` when another request moves the session between a command and its read-out (a race). Both tests therefore inject it: the smoke witness nulls `stage4` in the real server answer; the browser scenario wraps `window.fetch` to do the same for one `api/multi/view` answer. The 409 check uses a real stale fence against the real server.

**Reused evidence:** none. The base numbers in the table were re-observed by me on `e7c1a57d` in this worktree before any edit; they match the prior reported 184/184, 110/6 and 250/4.

## 6. Deviations, interpretations and limitations a reviewer should attack

- **D-1, UX-08 click precedence. SUPERSEDED by the owner's decision (R1.1).** Phase A read the ground first while Inspect ground was on; the owner rejected that and confirmed the literal spec order (crowns win; ground only where the hit-test finds no crown). Implemented in `1a22d01c`; the accepted UX risk is R1.3.
- **D-2, F-2 also withdraws Approve. ACCEPTED by the owner (R1.2).** The spec names Execute. Without a read-out the pending Prune orders are unknown, so approving would approve orders the user cannot see; I disabled Approve too. An Execute confirmation open when an unpaired view arrives triggers the existing withdraw-and-lock path, so the whole page waits for a re-read.
- **D-3, inspector wording/layout.** Heading "Choose competitors for Fell" became "Competitors for Fell · N within 8 m"; the strength word moved under the Hegyi CI value (column header "Hegyi CI"); "Clear selection" became "Clear" with `aria-label` "Clear the Fell draft selection"; on short desktops (≤ 820 px high) the visible "n selected · draft only" line and the area tag next to "Reference tree" are hidden (the map subtitle and the pressed Area button name the area; the count is in the Add button and announced); a long status chip shortens with an ellipsis (full text in its tooltip and the Tree record); Age is in the Tree record only. All other information is still present below the fold.
- **D-4, map foot.** The foot no longer repeats "site UNKNOWN · growth and crowns use both areas; Work Plans and summaries per area" or the model 0 edge sentence; the full statements stay in the notice Details and the Details pane (AC07). The rack cards' "Model 1/1/1 · Synthetic stand · Light: not PAR" badges are **unchanged** (not in the UX list); the owner may consider them repeated warnings, which would be a follow-up.
- **D-5, Escape and search boxes.** A search box with text keeps the browser's own Escape (clear the field) and the inspection stays open in that key press. Not stated in the spec; chosen to avoid destroying typed text.
- **D-6, light mapping constants** (`LIGHT_EASE 0.65`, `LIGHT_ALPHA_MAX 0.9`, level-word thresholds 0.1 / 0.35 / 0.65; the Phase A/R1 floor `0.0001` was **removed in R2**, see R2.3) are visual parameters, not ecology. The suggested `min(0.65, 0.65·L^1.2)` would give opacity ≈ 0.0026 at L = 0.01, which rounds to **no pixel change**; the ease 0.65 gives an 8-level summed change at 0.01 (measured). Below about L = 0.0007 the change rounds to zero pixels on an 8-bit screen although the written opacity stays positive ("potentially imperceptibly subtle", per spec §1). Cells are crisp 2.5 m squares (`shape-rendering: crispEdges`, no blur), so the layer looks blocky up close; blurring would spill light into L = 0 cells, which the spec forbids.
- **D-7, ground lookup.** The page matches a clicked point to the read-out's own clipped cell polygon (point-in-polygon), the same kind of screen→map conversion the page already does for hit-testing. It never interpolates or estimates a value.
- **D-8, rank buttons are positioned with CSSOM** (`element.style.left/top`), which the page CSP (`style-src 'self'`) allows; zero CSP reports in both runs. Positions are recomputed on every map render (zoom, pan, resize).
- **L-1, AC-9 screenshots (Phase A; R1 set: 12 PNGs, 4,827,430 B, see R1.4).** All nine 1280×720 states are committed plus two 1920×1080 states (Light Guide Area B, ground inspection): 11 PNGs, 4,517,240 B, largest 762,902 B, within ≤ 0.9 MB/image and ≤ 5 MB/set. The other seven 1920×1080 PNGs (one is 1,184,051 B, over the per-image cap) are kept outside the repo with the raw logs at `/home/jer/.local/share/ccf-evidence/multi-stage4-owner-ux-20261123/` (`SHA256SUMS` there).
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
| `07-ground-inspection-1280x720.png` | **R2:** Inspect ground on, open-ground read with a tree inspected: the reading sits below the Add bar, scrolled into view; concise summary, contributing trees expanded | 303,919 | `58f31fb2…` |
| `07-ground-inspection-1920x1080.png` | **R2:** same at 1920×1080 | 752,456 | `8b9eb359…` |
| `10-ground-mode-crown-selects-1280x720.png` | **R2:** Inspect ground on, a crown click selected MA-B-003: reference, ranks 1–5 and Add fully visible, ground panel below them | 318,454 | `9c7ae4a9…` |
| `08-notice-details-1280x720.png` | Notice Details open with every caveat | 219,667 | `2696243c…` |
| `09-f2-readout-unavailable-1280x720.png` | F-2: unavailable read-out, Approve/Execute withdrawn | 370,664 | `cde1086f…` |

No capability URL is in any committed file. A fresh capability URL for the owner playtest must be generated at playtest time and given privately.

## 8. Owner playtest steps (expected behaviour)

1. Start: `/home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived` in `Tools/MultiRackSitkaP1Lab`, open the printed `/multi` URL at 1280×720.
2. Click any tree crown: ranks 1–5 and **Add selected to Fell plan** are visible in the right panel without scrolling.
3. Click map numbers 1, 3, 5: each turns orange with a tick, the matching rows tick, the button says "Add selected 3 to Fell plan". Click 3 again: it clears. Untick row 1: map 1 clears.
4. Press Escape (or **× Close inspection**): the map returns to normal; zoom and area stay.
5. **Light Guide: On**: soft rose-to-peach light appears on the floor under the crowns (Area A is almost all dark; Area B's open edges are pale peach). Clicking a crown still selects it.
6. **Inspect ground: On**. Click a crown: it is selected as usual (no ground read), and ranks 1–5 and Add stay visible; the ground panel waits below them. Click open ground with no crown over it (easiest in Area B; Area A is almost all crown): a short summary (level, light index, canopy cover, shading trees); the selection and draft are unchanged. Escape stops ground mode; the Light Guide stays on.
7. Queue → Approve → Execute work as before.

# SCENARIO ONE UX UNITY QUALIFICATION HANDOFF

Role: UX Qualification Engineer. **Not merged. No ecology, density, growth, timber, save or objective code touched. No production C# changed.**
Everything below was run in real Unity 6000.6.0f1. Limits (native Windows, physical mouse, Player build) are stated at the end.

## IDENTITY

| | |
|---|---|
| Source branch / head | `claude/dazzling-pascal-s0jzlz` @ `84e881aa6d0e96c5752f09492d5db37d2cc8925e` (equal to `origin/claude/dazzling-pascal-s0jzlz` after `git fetch`; `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a` is its ancestor) |
| Qualification branch | `verify/scenario-one-ux-84e881a` (local only, **not pushed, not merged**); head = the commit that adds this file (tools and evidence only) |
| Unity | 6000.6.0f1 |
| Worktree | `/media/jer/Files/CCF-verify-scenario-one-ux-84e881a` (own `Library`; nothing developed on the source branch) |
| Interactive display | **a private nested X display (Xephyr `:77`)**, see "Method note" |
| Real saves | SHA-256 of `forest-save.json`, `.bak`, `prefs` checked after everything: **identical** (every Editor used an isolated `XDG_CONFIG_HOME`) |

**Method note, important for anyone re-running these gates.** The first interactive runs were on the live desktop and two failed: `MenuTutorialVerification` ("F1 does not reopen HUD help") and the rendered startup gate ("no key opened a forest panel…"). The active window at that moment was a web browser, and the log shows X focus contention. In the Editor, keyboard input only reaches the Game View while it has focus, so virtual key presses were silently dropped. Re-run on a private nested display (where focus cannot be taken), `MenuTutorialVerification` passes its whole playthrough. **Interactive gates on a shared desktop are not reliable evidence.** The desktop-run results are kept in `Build/UxQualification/desktop-display-run1/`. Software rendering on that display also makes frame times meaningless (about 2 s per frame); performance below is CPU-only for that reason.

## COMPILE

PASS. Fresh `Library`, 595 s, exit 0, **0 `error CS`**, no missing scripts, no importer failures. The only incidental changes are the Editor's own re-serialisation (`.vscode/settings.json`, 11 `Art/SectionFive/**/*.mat` files with `_Color` float noise), identical to what this Unity install writes on every branch; recorded and reverted. No scene, prefab, package, `.meta` or ProjectSettings change; nothing left staged in `Assets/`.

## EDITOR GATES

| Gate | Result |
|---|---|
| `MenuTutorialVerification` (edited by the patch, never run before) | **PASS ×2** (+1 smoke) on the private display. Its new Annual Review assertions hold: no help modal over the first results, in-screen reading guidance, F1 reopens the explanation. (FAIL on the live desktop, cause: focus, above.) |
| `ScenarioOneTeachingCopyVerification` (edited, never run) | **PASS ×3** (20, 20 and 57 checks; read-only world hash `EBE7A228F630AE41` in all three; batch hashes equal) |
| `StartupScreenVerification` | batch **PASS**; rendered **FAIL**, **pre-existing and not caused by the patch** (below) |
| `S1B/SessionMenuVerification` | **PASS** |
| `WorkPlanResidualVerification` (P3) | assertions **PASS ×3** (batch, batch, interactive); **determinism hash is `AF7405918195F0DE`, not the recorded anchor `F7C2FC966816BBAD`** (below) |
| `CropTreeCompetitorVerification` (P2) | **PASS ×3**, anchor `F58FB0B1A421D28B` unchanged |
| also run: `ClearanceVerification`, `ScenarioOneRemovalVerification` | PASS, PASS |

**Startup rendered failure.** It fails with the same message and same state on the pre-patch UI code (`a395e3f`, same private display), so it is not a regression. A diagnostic copy (message only) shows `year=0 workPlanOpen=False screen=None helpOpen=True helpMenu=WalkingHud`, and a screenshot plus log taken *before any key* shows the panel already open on a device that has never dismissed it (`introducedWalkingHud=False`, `uiEnabled=False`). The gate asserts "no help open", which only holds on a device that has already seen the panel. The screenshot shows the plain title with nothing visible, so a first-time player sees nothing wrong. The gate's assertion is too strict for a fresh device. Evidence: `evidence/startup-gate-diagnosis.md`.

**P3 anchor.** `P3_DETERMINISM_HASH` is a hash of the *rendered* "WHAT YOU ARE LEAVING" and "MONEY" text. The patch deliberately rewrote that card's subtitle to define the residual stand and basal area (packet item 8), so the hash moves from `F7C2…` to `AF74…`, deterministically across runs, with every P3 assertion green. It is an **intentionally changed anchor that the patch handoff did not declare** ("None unless explicitly authorised"). I did not edit the runner's expected value; `run_s1a_gates.py --phase focused` therefore reports a failure until the Manager authorises the new value.

## RESET FLOW

PASS (all 13 related checks, run on a played session with 4 help screens seen, 9 lesson steps done, the first review acknowledged and a real save file).
F10 opens the menu. "Reset tutorial/help progress…" asks first and says the forest and saved game are not changed. **Cancel** changes nothing (help 4→4, lessons 9→9, world hash unchanged, menu still open). **Confirm**: help 4→0, lessons 9→0, the cue returns to "Inspect a living tree", while the **forest hash, the save file's SHA-256, the objectives and the review acknowledgement are unchanged**, the year is unchanged (5→5, not a new game), and the menu shows "Tutorial and help progress reset on this device. Your forest and your saved game were not changed." Closing the menu opens "Your role · Scenario One" **once**; "More detail" opens the full help; Continue does not reopen it over 60 frames; F1 reopens it later.

## TEACHING ORDER

PASS. From a fresh player the persistent cue went: Inspect a living tree → Designate a Crop Tree → (Crop Tree) → Import the Fell mark into the Work Plan → Approve a felling job → Review a successful felling result → … → **Open the map last**. Monotonic in the packet's order, never map-first. The **42 stored step IDs are the identical set before and after the patch** (same sorted-ID hash; only the order changed). No year gate and no state machine was observed: the year advanced with lessons unfinished.

## PENDING-WORK GUARD

PASS, all paths, through the real buttons: pending only → the year does **not** advance and the inline card appears ("You have planned work that has not been approved.", Back to the plan / Approve pending work / Advance anyway); **Back** hides it and keeps the order and the year; **Approve pending work** needs the player's own click and then approves (year still unchanged until the next Advance); **Advance anyway** passes the year with "0 task(s)" and the order stays pending; reopening the Work Plan after the card was shown drops the old question; some approved + some pending → no card, the year advances; nothing pending → no card, immediate advance. *Observation, by the patch's own design:* in the mixed case the leftover pending order then waits silently.

## ANNUAL REVIEW

PASS. The first review opens with **no Help modal over it**; the reading guidance sits inline beside "I've read the annual results" (wraps to two lines at 1280×720); the acknowledgement is the one hard gate and clears it; F1 opens the review's help; **Esc closes only Help** and the review stays open; layout usable at all three sizes.

## CONTROLS / TERMINOLOGY

PASS. Full help contains the shared keys line and mentions left-click, X, C, 1–5, planting, clearance, Work Plan and map. The inspection card says "competition index" (0 standalone "CI"). The Work Plan card defines basal area and the residual stand. The HUD reads "Deer browsing: …". The startup Controls card lists all 16 rows and **offers only Back**.
*Layout observation:* the Controls card shows about 7½ of its 16 rows **at every window size** (the scroll box scales with the card), cut mid-row at "G — Planting mode… (buy"; the new G and U rows and M/Tab/O/F1/Esc/F5-F9/F10 are all below the fold on first sight. Usable (scrollbar, Back always visible) but the new controls are not seen without scrolling.

## MODEL-AWARE COPY

PASS. Growth model 1: the lesson says "only Sitka spruce earns timber income" and not the broadleaf-prices sentence. Growth model 2: "oak and beech stems have timber prices" and not the Sitka-only sentence; the Work Plan carries no stale "no broadleaf market" text. (Biological rationale for planting Oak/Beech not assessed, as instructed.)

## 1280×720

All 25 states rendered; no text outside the viewport, no clipping or overlap in the screens the patch changed: the role panel and its "More detail" button, first-use panels (Tree Inspection, Work Plan), the pending-work warning card (inline, amber edge, three buttons; it shrinks the scroll area above it), the Annual Review footer, the startup Controls list, the lesson cue (wraps to two lines), history and completion UI. Smallest text 10.4 px physical, median 12.0 px; 20 of 25 states have some text under 12 px.
*Defect (inherited from the lineage that added the count, not from this patch):* the HUD title row **"Scenario One · Year 7 · planning · 2 interventions" is truncated at the right screen edge** ("… 2 interve"); measured elsewhere as needing 381 against 294 panel units.

## 1920×1080

Same states, same layout, nothing clipped; smallest text 15.6 px, median 18.0 px. The Controls card is the same size on a larger screen (see above) and the HUD title truncation is the same.

## 1024×768

Rendered and examined. **Nothing clipped, nothing off-screen, all functions reachable** (the Annual Review, Work Plan with the warning card, first-use panels, reset dialog and Controls card all lay out). Legibility is the issue: UI scale 0.747, **median text 11.2 px, smallest 9.6 px; 11 of 25 states contain text under 10 px and 21 of 25 under 12 px** (chips and the startup/first-use panels are smallest). Body text reads, chips are small, buttons are fine, the Controls card shows ~7 rows of 16, the Annual Review and Work Plan remain usable with scrolling.
**Recommendation: NEEDS LOW-RISK SCALE FIX.** Not "supported as-is" (about a third of states have sub-10 px text), not "do not support" (layout and function hold). The patch's arithmetic (11.1 px body, 9.6 px chips) is confirmed by measurement. Not implemented, as instructed.

## INPUT

PASS on every check, on the Editor's virtual input: E inspects and returns; X and C mark and unmark (reversible); 1–5 on a Crop Tree's card (5 competitors listed, #2 selected); G refuses without stock ("purchase stock first", per the game) and enters planting mode with stock, Esc leaves it; U measured (no preview at that spot); Work Plan, Map, Objectives, Help, Session menu and Work-Plan-by-Esc each open and close. **Mouse capture:** after every modal closure the cursor is `Locked`, hidden, the player enabled and not moved. Esc frees the mouse; the first left click then retakes it **without** inspecting (documented), the second inspects.
*Not exercised:* a physical mouse. Editor-synthetic mouse events do not reach runtime UI panels (the shipped gates say the same), so every button was pressed through its registered callback.

## PRESERVED FEATURES

All present and working: Crop Tree competitor panel (+ P2 gate) and release forecast, reversible marks, pending → approved → resolved orders, MONEY and WHAT YOU ARE LEAVING cards, Stand Map open/return, management history with Earlier/Later year stepping, the first-intervention prompt, the stale-order card ("Orders needing attention"), completion summary at the completion year, marked "not a score" (outcome forced for the screenshot), HUD intervention count (**present but truncated**, above).

## PERFORMANCE SANITY

Record only. **Frame timing is invalid here** (software rendering, ~2.2 s/frame). CPU-only measurements: the lesson cue getter, called 200 000 times, takes **0.8 ms in total (4 ns each), allocates 0 bytes and triggers 0 collections**, so it does not allocate or spike per frame. Work Plan open: mean **33.6 ms** (max 53). Guard card: 40 show/hide clicks average **25.7 ms** (max 66.7), **the year never moved**, Mono heap growth ~70 KB per click (the Work Plan refresh), no refresh loop. Help + reset: **0.7 ms** per cycle. No Profiler capture.

## NATIVE WINDOWS

**Not available in this worker (Linux).** Not attempted; no Wine or other substitute is offered as acceptance. Font rendering, real mouse input, the F10 reset flow, the Annual Review and the warning card on native Windows at 1024×768 and 1280×720 remain **unverified**.

## FIX COMMITS

None to the source code or to any shipped gate. The qualification branch adds only `Tools/Verification/ScenarioOneUx/Qualification/` (harness, runner, diagnostic generator) and this evidence.

## OPEN UX DECISIONS

1. **Authorise the new P3 text anchor** `AF7405918195F0DE` (or revert the subtitle wording); the patch's handoff declared none.
2. **1024×768:** is it a supported demo window? If yes, a low-risk scale fix (smaller reference resolution or match-height for narrow aspect) is needed first.
3. **HUD title row** truncates the intervention count at every size (cut the label, widen the panel, or wrap).
4. **Startup Controls card height:** raise it or reorder rows so the new G/U controls are seen without scrolling.
5. **Partial approval:** leftover pending orders wait silently when some work is approved (a lighter, non-blocking note was left out by design).
6. **`StartupScreenVerification` rendered** needs a fresh-device-safe assertion (close or pre-set the first-use panel); the shared-desktop focus problem needs a documented private-display procedure.
7. H-5 (why the objectives ask for Oak/Beech) remains unwritten, as the patch said.

## RESULT

**B — QUALIFIED WITH SMALL FIXES.** Every behaviour the patch added passes in real Unity on a fresh-player storyline (reset flow, cue order, guard, Annual Review, controls and terminology, model-aware copy, input, preservation), the two never-run gates pass, and nothing is clipped at 1280×720 or 1920×1080. What remains is bounded: one declared-anchor decision (P3), one gate-assertion fix (startup rendered), three small layout items (HUD title, Controls card height, 1024×768 scale), and native Windows.

## NEXT STEP

Manager: authorise or revert the P3 text anchor; decide the 1024×768 support question; then a small UX layout packet (HUD title, Controls card, scale). Separately, give the interactive gates a fresh-device-safe startup assertion and a documented private-display runner so their results stop depending on what the person at the desk is doing. A native Windows smoke remains the open gate.

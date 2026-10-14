# IMPLEMENTATION HANDOFF — Scenario One UX small fixes and final qualification (packet U1)

Worker: Claude Sonnet 5.5, local Linux, real Unity 6000.6.0f1. Role: Primary for the three layout fixes and the verification-gate corrections; qualification engineer for the result.
**Not pushed. Not merged.** No ecology, density, growth, timber, save, objective or simulation code was touched.

## IDENTITY

| | |
|---|---|
| Source branch / head | `claude/dazzling-pascal-s0jzlz` @ `84e881aa6d0e96c5752f09492d5db37d2cc8925e` |
| Evidence branch it builds on | `verify/scenario-one-ux-84e881a` @ `cc54d6a` (left untouched) |
| This branch | `ux/scenario-one-small-fixes-84e881a`, local only, a descendant of both of the above |
| Worktree | `/media/jer/Files/CCF-verify-scenario-one-ux-84e881a` (its own Library) |
| Packet context commit | `321791b8203bc709c57615fd4d6a19614d78db24` |
| Display | private nested X display (Xephyr `:77` + openbox). Software GL, so frame times there mean nothing; this phase judges layout, input and logic only |

## COMMITS (production and test changes are separate, so the Manager can take either)

| Commit | What | Touches |
|---|---|---|
| `d6842c1` | new hard fit checks in the UX harness (added first, so they could be run against the unfixed code) | `Tools/Verification/ScenarioOneUx/Qualification/` |
| `5e4d66e` | **the three layout fixes** | `Assets/ForestPrototype/UI/` only (`ScenarioOneUi.uss`, `ScenarioOneUiRoot.cs`, `TreeInspectionView.cs`, `UiKit.cs`, `WorkPlanOverview.cs`) |
| `9b3a1f1` | P3 expected text anchor `F7C2FC966816BBAD` → `AF7405918195F0DE` | `Tools/Verification/run_s1a_gates.py`, `S1A_README.md` |
| `daf743f` | startup gate asserts what a player can see; harness hardening | `Tools/Verification/Startup/StartupScreenVerification.cs`, the UX harness |
| `38913c7`, `5df19a0` | startup gate layout check made scroll-aware | `Tools/Verification/Startup/StartupScreenVerification.cs` |

No scene, prefab, ProjectSettings, package or `.meta` file changed. The Editor re-serialises `.vscode/settings.json` and 5–11 `Art/SectionFive/**/*.mat` files (`_Color` float noise) on every run; they were reverted each time and their diffs are in `Build/U1Gates/editor-normalisation-*.diff` (ignored).

## U1.1 — THE P3 ANCHOR

* Full exact observed hash: **`AF7405918195F0DE`** (16 digits), reproduced in this worktree in 2 batch runs and 1 interactive run (all three identical). Old anchor: `F7C2FC966816BBAD`.
* **What the hash covers:** `FNV-1a(leaving + "\n" + money)`, where each part is the `Label` texts of the rendered cards "WHAT YOU ARE LEAVING" and "MONEY" joined by newlines (`WorkPlanResidualVerification.cs:161`). It is a hash of *presentation text*, not of the simulation.
* **Proof that only the intentional text changed** (`evidence/p3-anchor-proof.txt`, produced by a disposable copy of the gate that also logs the hashed text): a Python port of the gate's `StableHash` reproduces `AF7405918195F0DE` from the logged text; replacing **only** the new "WHAT YOU ARE LEAVING" subtitle with its old wording gives **exactly `F7C2FC966816BBAD`**. Source diff `b4894cc..84e881a` in `WorkPlanOverview.cs` is that one string.
* **World/simulation state did not change:** the same run logs `P3_READONLY_PASS empty plan; world hash unchanged EBE7A228F630AE41`, identical to the hash recorded for the old lineage (`Docs/Verification/S1B/Records/p3-results.json`), and `P3_RESIDUAL_PASS T2 336->306 BA 41.10->36.23 vol 39.87->34.68` is identical too.
* The expected value was updated in the two places that hold it (`run_s1a_gates.py`, `S1A_README.md`), both annotated: **"intentional UX verification-anchor update; not simulation rebaseline."** No ecological, reference or other anchor was touched. The layout fixes below are text-neutral (same labels, same order), which is why the hash did not move a second time.

## U1.2 — THE THREE RENDERED DEFECTS

Checked first against the unfixed code (the new checks fail there: `evidence/before-fix-failures.txt`), then against the fixed code.

1. **HUD title truncated** ("… · 2 interve"). The title wrapped nowhere: it needs 381 panel units and had 290. `.hud-status .heading { white-space: normal; }`. It now reads "Scenario One · Year 5 · planning · 2" / "interventions", panel height grows by one line, and the right-hand panels (placed from the status panel's bottom edge) follow.
2. **Tree Inspection card overflowed with the broadleaf product list** (text painted past the card, over the HUD forecast and marking panels, off the bottom of the screen for a pruned Oak). Fix: the "If felled now — estimate" section moves into **its own panel** (`inspect-estimate`, 380 wide) in the right-hand slot under the status panel that the Crop Tree competitor panel already uses (they are mutually exclusive: a Crop Tree being assessed shows competitors, any other living tree shows the estimate). The card's height cap rises from 72% to 80% so it can hold the remaining content and still clear the forecast panel. The cursor is locked while inspecting, so the card cannot scroll and has to fit. **This is a visible layout change for the Manager to be aware of.** Text is unchanged.
3. **"Deadwood volume" wrapped a syllable per line.** The long value never shrank, so it took the row and squeezed the name. `UiKit.Line(..., valueBelow: true)` puts a sentence value under its name for that one row (`.table-row-stacked`). Same two labels, same order, so the P3 hash is unaffected.

Acceptance, **1280×720 and 1920×1080 (hard checks, every inspection case)**: card, estimate panel and competitor panel are on screen; no text outside its own panel; no overlap with the HUD status, forecast, ground or marking panels; HUD title not truncated; Deadwood name on one line and value not clipped. Cases: Sitka with marks (the tallest HUD bottom-left stack: 4 Fell + 2 Crop), pruned Oak (3 lifts, longest product text), unpruned Oak, Beech, Oak Crop Tree (competitor panel), Oak under growth model 1 (no timber market).
Measured worst case (1280×720 reference units): pruned-Oak card **720 of 900 (exactly the 80% cap)**, bottom at 734; the HUD forecast panel starts at about 741–744. **The clearance in that deliberately worst-case fixture is only about 8–10 units.** It passes, but one more wrapped line in the card, or a taller forecast, would touch. See "Known limitations".

## U1.3 — GATES (each result is a file in `Build/U1Gates/`, ignored)

Production code (`5e4d66e`) was identical for every run below. Runs after it differ only in test files: P2, P3, SessionMenu and the S1-A targeted gates ran on `daf743f`; the final UX harness and the first two startup re-runs on `38913c7`; the passing startup re-run on `5df19a0`.

| Gate | Result |
|---|---|
| `MenuTutorialVerification` (interactive) | **PASS ×2** (`MENU_TUTORIAL_PLAYTHROUGH_PASS`) |
| `ScenarioOneTeachingCopyVerification` | **PASS ×3** (batch, batch, rendered), read-only world hash `EBE7A228F630AE41` in all three |
| `StartupScreenVerification` batch | **PASS** (`STARTUP_SCREEN_VERIFY_PASS`) |
| `StartupScreenVerification` rendered | **PASS** after the gate corrections below (was FAIL on the pre-UX lineage as well) |
| `S1B/SessionMenuVerification` | **PASS** |
| `WorkPlanResidualVerification` (P3) | **PASS ×3**, deterministic, hash `AF7405918195F0DE`, rendered 1280×720, 1600×900, 1920×1080 |
| `CropTreeCompetitorVerification` (P2) | **PASS ×3**, anchor `F58FB0B1A421D28B` **unchanged**, rendered at the same three sizes (Crop Tree card 641–648 units tall, within the new cap) |
| also run | `ClearanceVerification` PASS, `ScenarioOneRemovalVerification` PASS, offline UX logic check 176/176 PASS |
| Fresh-player UX harness (all phases, three sizes) | **191 hard checks, 0 failures**, 99 captures |

**`StartupScreenVerification` rendered, the inherited failure.** Cause (unchanged from the qualification): on a device that has never dismissed the first-use panel, the forest UI "opens" its role panel internally before the title appears, but the whole forest UI is hidden behind the title (`display: none`), so a player sees nothing. The gate asserted `!ui.Help.IsOpen`. It now asserts what a player can see and what the keys did: the forest UI root is not displayed behind the title, and no key changed the help state, the year, the Work Plan or the screen. No assertion was removed. Fixing that exposed a second, previously masked assertion: `CheckLayout` required every Controls row fully on screen at once, which contradicts the accepted design (the 16-row card may scroll if Back stays visible). The layout check is now scroll-aware (rows of a scrolling list must be reachable: the list is fully on screen, each row fits its viewport and lies inside the scrollable content) and a new step scrolls the list to its last row and requires that row and Back to be visible (`STARTUP_CONTROLS_SCROLL … content=820 viewport=440 scrolls=True`). Two small defects in my own first version of that check (a zero-area clipped rectangle counted as overlapping) were found by the gate and fixed before the final run.

## U1.4 — RENDER AND INPUT (UX harness, `evidence/final-hard-checks.txt`)

* **1280×720 and 1920×1080 (required):** all states rendered (role panel, More detail, Work Plan with the guard card, Annual Review, Tree Inspection, broadleaf product listing, Controls card, completion and history UI, session menu, reset flow, stand map, stale-order card). Text outside the viewport: 0 in all 33 states at both sizes (now a hard check). Smallest text 10.4 px at 1280×720 and 15.6 px at 1920×1080; none under 10 px.
* **Controls scrolling:** the 16-row card scrolls to F10 with Back on screen (hard check).
* **Input (Editor virtual input, private display):** F1, F10, Esc, E and left-click inspection, X and C marking (reversible), 1–5 competitor selection, G (refuses without stock, enters planting with stock, Esc leaves), U, and the Work Plan interactions all pass. Mouse capture after every modal closure is correct.
* **1024×768, functional smoke only (compatibility, not presentation):** the same harness ran at 1024×768 and every hard check passed, including all inspection fit checks (cards have clear margins there: reference height 1029). Legibility is unchanged and accepted as out of scope: UI scale 0.747, smallest text 9.6 px, median 11.2 px, 18 of 33 states have some text under 10 px. No UI was redesigned for it.
* **Isolation:** every interactive run was on the private display, never the live desktop (the earlier false failures came from another application holding focus).

## RESULT

**A — UX QUALIFIED**, on Linux in the Editor, for the packet's scope: the three defects are fixed and verified at both required sizes, every required gate passes (the inherited startup failure is corrected without weakening it), the P3 anchor change is explained and proven to be text-only, and nothing outside the UI layer changed.
This is **not** a native Windows result and **not** a Player result.

## KNOWN LIMITATIONS AND NOT VERIFIED

* **Native Windows, a physical mouse and a Player build were not exercised** (Linux worker). Mouse input in the Editor is simulated; buttons are pressed through their registered callbacks.
* **Thin margin in the worst-case inspection fixture** (about 8–10 reference units between the pruned-Oak card and the HUD forecast panel). If the Manager wants more room: shorten or move the storm-exposure paragraph (it appears even with storms off), or cap the forecast width. Neither is a layout-only change, so I did not do it.
* The "before" run was made with an earlier version of the harness; two of its 23 recorded failures (`INSPECTION_FIXTURE_LEFT_NO_MARKS`, and the Crop Tree number tags counted as "outside the viewport") were harness defects fixed before the final run, not product defects.
* 1024×768 legibility (above) and the long-standing items from the earlier qualification that this packet did not ask to change: leftover pending orders waiting silently after a partial approval, H-5 (why the objectives ask for Oak/Beech) still unwritten.
* No Profiler capture; frame timing on the private display is software-rendered and not meaningful (U2 measures performance on the real GPU).

## DECISIONS FOR THE MANAGER

1. Take this branch (or cherry-pick `5e4d66e` and `9b3a1f1`, plus the Startup gate changes) onto `claude/dazzling-pascal-s0jzlz`. The branch is a descendant of `84e881a`; its only non-production additions are test tooling and this evidence.
2. Accept the player-visible change: the felling estimate now appears in a separate panel on the right.
3. Whether the thin forecast-panel margin needs a content-level follow-up.

## FILES

`evidence/p3-anchor-proof.txt`, `evidence/before-fix-failures.txt`, `evidence/final-hard-checks.txt` (191 checks), `evidence/text-sizes.csv`. Screenshots (100 PNGs) stay in the ignored `Build/UxQualification/u1-final/evidence/ui/`, with the before-fix set in `u1-before/` (no binary footprint decision has been made for them).

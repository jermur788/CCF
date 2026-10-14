# U4 — Scenario One reviewed two-cycle completion: Unity qualification

Role: Unity Integration & Qualification Engineer. **Not merged. No production C# changed. No anchor rebaselined. Nothing pushed to `main`.**
Everything below was run in real Unity 6000.6.0f1 on the worktree named here, unless it says otherwise.

## RESULT: B — QUALIFIED WITH A BOUNDED PRESENTATION ISSUE

Every behavioural property in the packet holds. One thing the packet asked me to verify does not: **the game never tells the player that another reviewed management cycle is required.** After one intervention the player sees `Forest objectives 8 of 8`, eight green ticks, and one generic line: "N years since intervention 1. Inspect the stand, read the history in the Annual Review, and plan intervention 2 if the forest needs it." Nothing says that the scenario is not complete yet, or why. That is a text/guidance gap, not a logic defect, so I rate it B, not C. Details and the exact strings are under 1A and 1E.

Rule I applied (so it can be overruled): *A* = all hard properties pass and the guidance is explicit; *B* = all hard properties pass and a presentation item is open; *C* = a hard property fails; *D* = the design is unsafe.

## IDENTITY

| | |
|---|---|
| Source branch / head | `task/scenario-one-two-cycle-completion-implementation-v1` @ `eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac` (equal to `origin/` at `git fetch` on 2026-10-14) |
| Qualification branch | `verify/scenario-one-two-cycle-eb7fd3d` (new, from `eb7fd3d`) |
| Harness head | `7eb525c` (test-only: `Tools/Verification/U4/`, `Tools/Verification/Qualification/`). Evidence and this file are added by the commit on top. |
| Production diff vs source head | **none**: `git diff eb7fd3d HEAD -- Assets Packages ProjectSettings` is empty |
| Worktree | `/media/jer/Files/CCF-verify-two-cycle-eb7fd3d` (own `Library`, imported from scratch; `/home/jer/CCF-main` untouched) |
| Unity | 6000.6.0f1, one Editor at a time on this project, `systemd-run` scope capped at 8 GB, isolated `XDG_CONFIG_HOME` per run (the real save slot was never touched) |
| Geometry / models | New-game default: Enlarged80, growth model 1, RNG 1, regeneration 2. The historical gates ran in `CCF_STAND_GEOMETRY=0` (Legacy40). |
| Context commit | The packet gave none. I locked to the exact source head above and to `Docs/Project` as it stands on `main` (`691dd18`). Flagging this as a packet gap. |

## COMPILE

PASS. Fresh `Library` import plus compile in batch mode, exit 0, **0 `error CS`**, 490 `warning CS` (the pre-existing obsolete-API class; 481 on the density-v2 worktree). `Tundra build success` in 87.9 s of the 16.5 min import. Evidence: `evidence/compile-summary.txt`.

Asserted at runtime (`core` phase): `ForestSaveData.CurrentVersion == 20`; `ScenarioManagementEventType` values `StockPurchased..CenturyReviewed == 0..8` unchanged and `AnnualReviewAcknowledged == 9` is the only addition (10 values in total).

Static: `git diff 157ea7d HEAD` is empty for `ScenarioOneObjectives.cs`, `ScenarioOneDefinition.cs`, `ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation.cs`, `ForestEcologyController.cs` and `SitkaGrowthModel.cs`. Production files that differ from the pre-U4 head: `ScenarioManagementEvent.cs` (enum), `ScenarioOneCompletionRules.cs` (+ `.meta`, new), `ScenarioOneManager.cs`, `AnnualReviewView.cs`. The eight objective ids and values are identical before and after (`1A_objective_lines`).

## HOW THE GATE WORKS (and what is and is not synthetic)

`Tools/Verification/U4/U4Qualification.cs`, staged into `Assets/ForestPrototype/` by `Tools/Verification/Qualification/run_qualification.py`, removed with its `.meta` after every run (the runner fails the run on any other new working-tree change). It drives **public handlers only**: `marking.Mark` → `AddMarkedTreesToWorkPlan` → `TryPurchaseStock` → `TryDesignateExactPlanting` → `ApprovePendingWork` → `AdvanceYear` → `MarkAnnualReviewSeen`, and the real `ForestSaveController.Save()/Load()` file path, with a fresh Editor process between save and load.

- **Plan A** = one Work Plan resolved in year 1: a 27 % basal-area thinning (275 trees: 4 left as fallen deadwood, 3 kept for use, the rest sold), 8 Oak and 8 Beech saplings (4 pairs each, half owner-planted with shelters). One planning session, one approval, one `AdvanceYear`.
- **Synthetic** (labelled `SYNTHETIC` in the evidence): the four ordering fixtures in 1C (same-year plans with a review between them, failed-only second plan, duplicate event IDs, and the valid control). They edit a **real** captured save. Everything else is gameplay.
- **Legacy (pre-U4) saves** are made by running the same play with the **old acknowledgement handler** (it only sets `annualReviewSeen`), so event IDs stay continuous. They were *not* produced by the old binary. The A/B in 1F does run the old binary, for the historical completion gate.
- Single seed per fixture. Time: the 112-check `core` phase (about 230 simulated years across all forks on Enlarged80) took 594 s.

## 1A — OLD SHALLOW PATH → NOT COMPLETE (PASS) · guidance (OPEN)

All eight original objectives satisfied, one resolved Work Plan, one felling, one Oak planting, one Beech planting, one acknowledgement (year 1, event id 877). At year 25: `Outcome = Active`; still `Active` at year 30; objectives `8 of 8`, `Interventions = 1`:

`✓ minimum-year 25/25 · retained-canopy 921/240 · continuous-canopy 0.91/0.35 · regeneration 218/12 · fallen-deadwood 64.62/0.08 · managed-opening 1/1 · introduced-beech 1/1 · introduced-sessile-oak 1/1`.

No objective definition changed (static diff above; ids, targets and values identical to the pre-U4 run of the same play).

**Guidance — OPEN.** The player-facing strings in exactly this state (all captured, see `evidence/evidence-core.txt` and `evidence/evidence-ui.txt`):
- `NextStepHint`: "24 years since intervention 1. Inspect the stand, read the history in the Annual Review, and plan intervention 2 if the forest needs it."
- HUD: "Forest objectives 8 of 8", no completion text.
- Annual Review (help closed): "OBJECTIVES 8 OF 8", eight "✓ done" lines, then the same hint.
A search for "second intervention / another cycle / required / to complete / completion / acknowledge" over every visible string finds **nothing**. `plan intervention 2` is an implicit nudge ("if the forest needs it"), not an explanation. Screenshot: `evidence/ui-shallow25-review.jpg` (Annual Review) and `ui-shallow25-hud.jpg`.

## 1B — VALID TWO-CYCLE PATHS → COMPLETE (PASS)

| path | first intervention (Work Plan A) | acknowledgement | second intervention (Work Plan B) | completion |
|---|---|---|---|---|
| **V1, shortest** | year 1: `FellTree ×275`, `PlantJuvenile(sessile-oak) ×8`, `PlantJuvenile(beech) ×8` | year 1, event id 877 | year 2: `PruneTree ×1` | **year 25** (the earliest the minimum-year objective allows) |
| V2 | same | same | designated year 16, resolved year 17: `FellTree ×187` (20 % thinning) | year 25 |
| V3 | same | same | designated year 24, resolved **year 25**: `PlantJuvenile(sessile-oak) ×1` | year 25, the same step |
| V4 | same | same | approved at year 25 (still `Active` after an explicit `EvaluateProgress`), resolved year 26: `PruneTree ×1` | **year 26** |

The second action is **non-prescriptive**: pruning, thinning and planting each complete. With plan B only approved, never resolved (V3, V4), the scenario stays `Active`. Interventions = 2 in each; the completion summary lists both. Final world hash of V1 at year 25: `572025058B227E92` (it reappears after both relaunch sequences, below).

## 1C — NEGATIVE ORDERING CASES (all PASS)

| case | how produced | result |
|---|---|---|
| two resolved plans, same year | gameplay: Plan A's felling and plantings are many orders in one year (this is 1A). SYNTHETIC: a second plan resolved in year 1 with a review between the two | `Active`, one intervention |
| two years, no review between | N1: A (Y1), B (Y2), no acknowledgement at all | `Active` at 25, 8/8 objectives; footer acknowledgement offered |
| review only before the first plan | N2: idle first year, acknowledge, A (Y2), B (Y3), no second review | `Active` at 25, 8/8; footer offered. **This is the stuck-looking case** (see 1E) |
| `annualReviewSeen` alone | N3: flag true, no event 9, two plans | `Active`; fresh acknowledgement reachable |
| pending-only second plan | N4: order designated, never approved | `Active`, still one intervention, the order stays open |
| approved but unresolved second plan | V3/V4 | `Active` until the step that resolves it |
| empty / invalid plan | N5: approving an empty Work Plan is refused; SYNTHETIC: a second plan whose only resolution is a failure | `Active` |
| failed scenario | spend every euro on nursery stock (3 Oak + 2663 Beech, cash exactly 0), advance | `Failed`, "No cash remains…"; advancing, acknowledging, re-evaluating and designating new work do not revive it; `Failed` after reload |
| duplicate event IDs | SYNTHETIC | cannot prove chronology → `Active` |
| control | SYNTHETIC valid ack-then-later-plan | `Completed` (so the negatives are not vacuous) |

Recovery is not a dead end: in N1 a late acknowledgement at year 25 does not complete the old plans (PASS), but a **third** plan afterwards completes (year 26).

Informational (N6, not asserted): prune + fell on the same tree in one plan resolves as `FellTree ×1`, and that plan counts as plan B.

## 1D — SAVE / LOAD (all PASS, real file save, fresh Editor process between steps)

| | steps | result |
|---|---|---|
| **A** | process 1: Plan A → year 1 → save. Process 2: load → acknowledge → save. Process 3: load → second intervention → run to 25 | `Completed` at 25; final world hash `572025058B227E92` = the uninterrupted V1 run |
| **B** | process 1: Plan A → acknowledge → save. Process 2: load → second intervention → run to 25 | `Completed` at 25; hash `572025058B227E92` |
| **C** legacy ACTIVE v20 (flag true, no event 9) | load from disk | loads `Active`; **no event 9 fabricated**; interventions intact; footer acknowledgement reachable; re-evaluation does not complete; a fresh acknowledgement is recorded at year 1; a later plan B completes at year 25 |
| **D** legacy COMPLETED v20 | load from disk | stays `Completed` after re-evaluation and after another year; the completion summary is still produced (it lists **1** intervention, as the old save recorded) |
| **E** legacy FAILED v20 | load from disk | stays `Failed`; further years are refused |

Saved `version` is 20 in all four fixtures and after every save; no event 9 is present in the legacy fixtures (`fixtures-manifest.txt` has their SHA-256). **No save-version bump.**

## 1E — HISTORY / PRESENTATION

PASS: the acknowledgement event is not an intervention (interventions stay 1 after the acknowledgement and 2 after plan B; completion lists 2 with ordinals 1, 2 in year order); event IDs strictly increase; the acknowledgement lies between the two resolved plans in recorded order; the history fingerprint, the event order and the single acknowledgement are identical after reload; the completion wording is produced ("You carried out 2 interventions: … Intervention 2 · year 2: 1 pruned").

PASS (graphical Editor, Xephyr, `Clickable.Invoke`): for a legacy ACTIVE save the Annual Review shows the acknowledgement footer and button `annual-review-acknowledge`; the first-review gate itself is not active, so the footer is the only route; clicking records the event at the current year; the control disappears afterwards.

OPEN presentation points (all text, none logic):
1. 1A: no explicit "another reviewed cycle is required" anywhere.
2. The footer wording is the generic first-review prompt ("Read WORK DONE, MONEY and FOREST before continuing." / "I've read the annual results"). It does not say that this acknowledgement is what a later plan needs.
3. **The one path that looks stuck:** the UI forces the first acknowledgement before the next advance, so the normal path (Plan A resolves in year 1, acknowledge, any later plan) completes by itself. A player who advances an **idle first year**, acknowledges, then does two plans without re-opening the review (N2) reaches eight ticks at year 25 and is `Active` with no explanation; `NeedsCompletionReviewAcknowledgement` is true and the footer is reachable, but nothing points to it.

## 1F — HISTORICAL GATES AND THE INTENTIONAL CONTRACT DELTA

| gate | head | result |
|---|---|---|
| shipped `N3CompletionVerification` (never run before) | U4 | PASS (`N3_U4_VERIFY_PASS`) |
| `ScenarioHistoryVerification` | U4 | PASS (39 assertions) |
| N3 offline rules (67 checks), density 35 / timber 71 / history 70 | U4 | PASS (plain .NET; not Unity) |
| `ScenarioOneCompletionVerification` | **pre-U4 production `157ea7d`** | **PASS**: `outcome=Completed` at year 25, v18 compat hash `702766DECE591E21` |
| `ScenarioOneCompletionVerification` | **U4** | **FAIL, as the author predicted**: the same play ends `Active` at year 25 (it never clicks the review), and the v18 layout hash is `F5320B2FA2333B52` instead of `702766DECE591E21`. The eight objective lines and the cash figures (`minimum=613986`) are identical in both runs, so the ecology is unchanged; only the completion chronology differs |

I ran the A/B in this worktree on a temporary branch at `157ea7d` (production only differs in the four files above), then restored the verify branch and deleted the temporary one.

Not run on the U4 head: the 24-gate regression and the protected Phase-0 anchors. Other gates that read `Outcome == Completed`, `annualReviewSeen` or the v18 hash (`ScenarioOneProgressVerification`, `MenuTutorialVerification`, `ScenarioReferenceVerification`, `Model2Verification`, `Enlarged80ViabilityVerification`, the storm long-run harnesses) are the likely ones to need a recorded expectation change. I did not rebaseline any of them.

## LIMITS

- One seed, one stand per fixture; Enlarged80 (the new-game default) for U4's own gate, Legacy40 only for the historical gates.
- Legacy fixtures are old-handler emulation on real play, not saves written by the old binary.
- Four ordering fixtures are synthetic (labelled).
- "Unknown enum" handling by an older reader is not exercised (no older reader exists here); the offline suite covers the typed/raw validators.
- UI checks used `Clickable.Invoke`, not synthetic mouse events.
- The first UI run's guidance search matched the word "further" in the first-use help overlay. I fixed the search (help closed, specific phrases) and re-ran; the first run's evidence is kept in `evidence/superseded-first-ui-run/`.
- The `core`, relaunch, legacy and shipped-gate steps ran on gate commit `3ca0b11`; only the UI phase differs in `7eb525c` (and the UI step ran on that file: staged SHA `fa2fa6b4…`).

## DECISIONS NEEDED (Manager)

1. Add explicit guidance (wording and where it appears is a product call). Smallest change: when all eight objectives are met and `Outcome == Active`, say so in `NextStepHint`, and when `NeedsCompletionReviewAcknowledgement` is true say what the acknowledgement is for. I did not write it (no production edits in this packet).
2. Whether the idle-first-year route (N2) should instead record the acknowledgement automatically when the Annual Review is opened after a resolved plan.
3. Which historical gates get a recorded expectation change for the intentional delta.

## EVIDENCE

`evidence/` — `evidence-core.txt` (112 hard checks), `evidence-saveA1..A3`, `saveB1..B2`, `legacyMake`, `legacyC..E`, `evidence-ui.txt`; `results-*.json` per run (exit code, run head, staged-gate SHA, working-tree changes); `*.markers.txt` (log lines that matter); `compile-summary.txt`; `fixtures-manifest.txt`; two screenshots (JPEG, half-size). Totals: **170 hard checks passed, 0 failed**, plus the shipped N3 gate and 39 history assertions; 1 open presentation finding.

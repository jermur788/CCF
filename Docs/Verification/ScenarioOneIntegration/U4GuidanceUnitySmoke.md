# U4 completion guidance A1: Unity smoke (overnight queue job B2, Tier 1)

**RESULT: PASS** (107 checks, 0 failed) on the A1 guidance head, with presentation observations below. Real Unity 6000.6.0f1, graphical Editor (nested X display, 1280x800 window), Linux. Not a Player, not native Windows.

**Question.** Does the new completion guidance actually appear in the three intended states: (1) all objectives met plus one plan, (2) first plan plus missing qualifying review, (3) review complete and waiting for a second plan? Is the text visible, is the Annual Review direction correct, does the second intervention stay non-prescriptive, and did any completion logic change?

## Identity

| | |
|---|---|
| Source (A1, the head the integrator packet names) | `task/scenario-one-two-cycle-guidance-v1` @ `a93a7220fd8b4d8246b3613d913360dad06988e8` (read again from origin before the run: unchanged) |
| Verification branch | `verify/u4-guidance-a1-smoke-a93a722`, from `a93a722`. Production diff against `a93a722` is **empty** (`git diff a93a722 HEAD -- Assets Packages ProjectSettings`). Added: the U4 gate and generic runner (unchanged copies from `verify/scenario-one-two-cycle-eb7fd3d`) and `Tools/Verification/U4Guidance/U4GuidanceSmoke.cs` |
| Run head | `48fd2da564577b34508765cd818eeb43f1e4faf2` (gate commit on top of the source) |
| Worktree | `/media/jer/Files/CCF-verify-two-cycle-eb7fd3d` (own Library; demo-HDR quarantine wrapper) |
| Geometry / models | Enlarged80, growth model 1, RNG 1, regeneration 2 (new-game defaults) |
| Not run | the 170-check U4 qualification and the 24 gates (as instructed) |

**A note on the first attempt.** I first ran this job on `ba48591` (the earlier guidance implementation) because I had not noticed that A1 had been published at 00:14. That record is on `verify/u4-guidance-smoke-ba48591` and is marked superseded. Its result was also PASS with the same structure; A1 differs in wording (see the table) and in being a separate class (`ScenarioOneCompletionGuidance`) instead of members of `ScenarioOneCompletionRules`.

## What the gate does

`U4GuidanceSmoke.cs` reuses the U4 gate's helper blocks verbatim (same Work Plan A: a 27 % thinning plus four Oak and four Beech planting pairs; same pruning plan B). It drives real handlers: mark, Work Plan, purchase, exact planting, approve, `AdvanceYear`, the acknowledgement, and for the second path the real UI button click. It reads the guidance from the production class and every visible text of the real UI Toolkit panel, and checks that each text is **fully inside its scroll viewport and the panel** (not merely displayed).

The three texts as A1 defines them:

| State | Text |
|---|---|
| 2 AcknowledgeReview | Open the Annual Review and choose "I've read the annual results". Scenario One completes only when an intervention you have reviewed there is followed by another intervention in a different simulation year. |
| 1 ObjectivesMetOneCycle | All eight objectives are met. Another management cycle is required in a different simulation year. Return to the forest and carry out another intervention when you judge the forest needs it. |
| 3 AnotherIntervention | You have reviewed your intervention. Return to the forest and carry out another intervention when you judge the forest needs it; Scenario One completes once the eight objectives are met and that second intervention is resolved in a different simulation year. |

| State | How it is reached | Observed |
|---|---|---|
| none (control) | fresh Year 0 | no guidance; the old hint unchanged |
| first review pending | Plan A, Year 1 | first-review wording kept; **none** of the three texts shows (as designed) |
| **3** | Plan A, Year 1, acknowledged | state 3; `NextStepHint` is exactly the text; in `TutorialHint`; on screen without scrolling in the Work Plan status line; at the end of the Annual Review |
| **1** | the same play idle to Year 25: eight of eight objectives, Active, one intervention | state 1; `NextStepHint` is exactly the text; at the end of the Annual Review; **not** on the HUD |
| **2** | idle first year, acknowledged by the real UI click, then Plan A resolves in Year 2 | state 2; the exact text is the Annual Review footer prompt directly above the control, on screen without scrolling; it quotes the button label, which equals the real button text `I've read the annual results` |
| precedence | the same play idle to Year 25, review still missing | still state 2; after the click state 1 |
| after the click in state 2 | click at Year 2 | state 3, footer and control disappear |

Non-prescriptive: none of the three texts matches felling, thinning, pruning, planting, harvest, cutting, Oak, Beech, Sitka, species, sell or deadwood (13 whole-word patterns per text).

**No completion-logic change.** `git diff eb7fd3d a93a722 -- Assets` touches only `ScenarioInterventionHistory.cs` (one optional parameter), the new `ScenarioOneCompletionGuidance.cs`, `ScenarioOneManager.History.cs` and `AnnualReviewView.cs`; `ScenarioOneCompletionRules.cs` is untouched. At runtime the U4 "V1" path (Plan A, acknowledge, pruning plan in Year 2, idle to Year 25) completes at **Year 25 with world hash `572025058B227E92`, equal to the value the U4 qualification recorded on `eb7fd3d`**; after the second plan resolves the guidance is gone and the completion hint is back. The second path (first plan resolved in Year 2, acknowledged only in Year 25 through the footer, second plan in Year 26) completes in Year 26. Save version is still 20.

## Observations for the Manager (none fails the question asked)

1. **In states 1 and 3 the Annual Review shows the text at the very end of the scrolling page**, below Management History and Trends, as the last line of the objectives card (not on screen without scrolling; fully on screen after scrolling to the end). State 3 is also on screen without scrolling in the Work Plan status line, and state 2 is in the always-visible footer. **State 1, the moment a player sees "Forest objectives 8 of 8", is not on the HUD and not on the first screen of the Annual Review** (`STATE1_hud_shows_guidance False`; the guidance change leaves the HUD alone). A player finds it by opening the Work Plan or scrolling the review.
2. **A1's state-1 text does not point at the Annual Review** (the `ba48591` wording did). That is consistent with its job (go back and intervene); the direction to the Annual Review is carried by state 2, which does name it and the button.
3. **The state-2 text begins "Open the Annual Review and choose ..."** and is also shown in the review's own footer, where the review is already open: right for the hint line, redundant inside the review.
4. All three texts say the second intervention must be in "a different simulation year". The runtime behaviour matches in the paths run here (second plans in later years complete). The same-year negative case is part of the U4 matrix on `eb7fd3d` (not rerun), and `ScenarioOneCompletionRules.cs` is untouched by A1.

## Method notes

- Two Unity runs of this gate shape have been made: on `ba48591` (97 checks, then 107 after I replaced "displayed" by a real viewport check once the screenshots showed states 1 and 3 sit below the fold) and this one on A1 (107 checks). Only the A1 run answers B2.
- The gate's A1 adaptation is two lines: the production class name, and the state-1 assertion (A1's text states the eight objectives and "another", instead of pointing at the Annual Review; whether it mentions the Annual Review is recorded as a note).
- Editor normalisation (`.vscode/settings.json`, several `Art/SectionFive` materials) was reverted; no unexpected tree change. Run time 239 s.

## Not covered

The HUD beyond the texts above, 1920x1080 and 1024x768 layouts (B5/B6), saves from earlier versions, and the full U4 matrix.

## Evidence

`U4GuidanceUnitySmokeEvidence/`: `evidence-guidance-smoke.txt` (every check and note), `results-guidance-smoke.json` and `guidance-smoke.markers.txt` (receipt: PASS, exit 0, run head, interactive true, no compile errors, no unexpected tree change), six captures downscaled from 3840x2160 to 1600x900 JPEG (about 1.4 MB; the full PNGs, about 48 MB, stay in the worktree's untracked `Build/B2/data/`).
Reproduce: inside `Tools/Build/S1B/with_demo_hdr_quarantined.py`, quiet machine, nested X display: `python3 Tools/Verification/Qualification/run_qualification.py --label guidance-smoke --out Build/B2/run --method U4GuidanceSmoke.Begin --file Tools/Verification/U4Guidance/U4GuidanceSmoke.cs --marker U4_GUIDANCE_PASS --env U4_OUT=Build/B2/data --interactive --window 1280x800`.

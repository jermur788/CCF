# U4 completion guidance: Unity smoke (overnight queue job B2, Tier 1)

> **CORRECTION, SUPERSEDED AS THE B2 ANSWER.** This record tests `ba48591` (`task/scenario-one-completion-guidance-v1`), the *earlier* guidance implementation. The integrator packet (`docs/scenario-one-integrator-packet-v1` @ `60394d7`) names the guidance head for the queue as **A1 = `a93a7220fd8b4d8246b3613d913360dad06988e8`** (`task/scenario-one-two-cycle-guidance-v1`, published 2026-10-15 00:14) and says `ba48591` is replaced by it. I ran this job without noticing that branch and wrote below that no other head existed; **that sentence was wrong.** The B2 answer for A1 is in the follow-up record on branch `verify/u4-guidance-a1-smoke-a93a722` (same gate, adapted to A1's class name and wording). What follows is still a true account of `ba48591` and is kept as evidence of the gate's method.

**RESULT: PASS** (107 checks, 0 failed), with presentation observations below. Real Unity 6000.6.0f1, graphical Editor (nested X display, 1280x800 window), Linux. Not a Player, not native Windows.

**Question.** Does the new completion guidance appear in the three intended states: (1) all objectives met plus one plan, (2) first plan plus missing qualifying review, (3) review complete and waiting for a second plan? Is the text visible, is the Annual Review direction correct, does the second intervention stay non-prescriptive, and did any completion logic change?

## Identity

| | |
|---|---|
| Guidance head under test | `task/scenario-one-completion-guidance-v1` @ `ba48591d874d666e72d421d1d0e5ff4dcfe3cd71` (origin read again before the final run: unchanged). **This is NOT the A1 head the queue meant** (see the correction above): it is the earlier implementation, the only guidance head I had found when I ran. |
| Verification branch | `verify/u4-guidance-smoke-ba48591`, from `ba48591`; production diff against `ba48591` is **empty** (`git diff ba48591 HEAD -- Assets Packages ProjectSettings`). Added: the U4 gate and generic runner copied unchanged from `verify/scenario-one-two-cycle-eb7fd3d` (1d8cca2), and `Tools/Verification/U4Guidance/U4GuidanceSmoke.cs` |
| Worktree | `/media/jer/Files/CCF-verify-two-cycle-eb7fd3d` (own Library; the demo-HDR quarantine wrapper was used) |
| Geometry / models | Enlarged80, growth model 1, RNG 1, regeneration 2 (new-game defaults) |
| Scope | Not the 170-check U4 qualification, not the 24 gates. No save/legacy/negative matrices. |

## What the gate does

`U4GuidanceSmoke.cs` reuses the U4 gate's helper blocks verbatim (same Work Plan A: a 27 % thinning plus four Oak and four Beech planting pairs; same pruning plan B). It drives real handlers: mark, Work Plan, purchase, exact planting, approve, `AdvanceYear`, the acknowledgement, and for the second path the real UI button click. It reads the guidance from the production class, then reads every visible text of the real UI Toolkit panel.

| State | How it is reached | Observed |
|---|---|---|
| none (control) | fresh Year 0 | no guidance; the old hint "No intervention yet..." unchanged |
| first review pending | Plan A, Year 1, first-review gate active | the review and the tutorial hint keep their first-review wording; **none** of the three texts shows (as designed) |
| **3** AnotherIntervention | Plan A, Year 1, acknowledged | `CompletionGuidance` = state 3; `NextStepHint` is exactly the state-3 text; it is in `TutorialHint`, in the Work Plan status line (on screen without scrolling), and at the end of the Annual Review |
| **1** ObjectivesMetOneCycle | the same play idle to Year 25: eight of eight objectives, Active, one intervention | state 1; `NextStepHint` is exactly the state-1 text; in the Annual Review (end of the objectives card); **not** on the HUD |
| **2** AcknowledgeReview | idle first year, acknowledged by the real UI click, then Plan A resolves in Year 2 | state 2; `NeedsCompletionReviewAcknowledgement` true, first-review gate not active; the exact text is the Annual Review footer prompt directly above the control, on screen without scrolling; the text quotes the button label, which equals the real button text `I've read the annual results` |
| precedence | the same play idle to Year 25, review still missing | still state 2 (acknowledge comes before "objectives met"); after the click state 1 |
| after the click in state 2 | click at Year 2 | state 3, the footer and control disappear, the review shows the state-3 text |

Non-prescriptive: none of the three texts matches felling, thinning, pruning, planting, harvest, cutting, Oak, Beech, Sitka, species, sell or deadwood (whole-word patterns, 13 checks per text).

**No completion-logic change.** The U4 "V1" path (Plan A, acknowledge, pruning plan in Year 2, idle to Year 25) completes at **Year 25 with world hash `572025058B227E92`, equal to the value the U4 qualification recorded on `eb7fd3d`**. After the second plan is resolved the guidance is gone (state none) and the completion hint ("Objectives met in year 25 ...") is back. The second path (first plan resolved in Year 2, acknowledged only in Year 25 through the footer, second plan resolved in Year 26) completes in Year 26. Save version is still 20.

## Observations for the Manager (none fails the question asked)

1. **In states 1 and 3 the Annual Review shows the text at the very end of the scrolling page**, below Management History and Trends, as the last line of the objectives card (viewport check: not on screen without scrolling, fully on screen after scrolling to the end). State 3 is also on screen without scrolling in the Work Plan status line, and state 2 is in the always-visible footer. **State 1, the moment a player sees "Forest objectives 8 of 8", is not on the HUD** (the HUD was deliberately left alone by the guidance commit; `STATE1_hud_shows_guidance False`) and is not on the first screen of the Annual Review. A player finds it by opening the Work Plan or scrolling the review.
2. **The state-2 text begins "Open the Annual Review and choose ..."** and is also shown in the Annual Review's own footer, where the review is already open. The direction is right for the hint line and the Work Plan; inside the review it is redundant.
3. The first review keeps its own wording and shows none of the guidance (verified, intended).

## Method notes and what was corrected

- **Two runs.** Run 1 (gate head `5f57d10`, 97 checks) used "displayed in the layout tree" as visibility. The screenshots then showed that state 1 and state 3 are below the fold, so run 1's "visible" was weaker than the question. I added a real viewport check (`OnScreenCheck`: fully inside the scroll viewport and the panel, before scrolling where the design says it must be, after scrolling otherwise) and scrolled captures, and ran again (gate head `ffbfd3a`, **107 checks, the result above**). Run 1's evidence is kept in `UnitySmokeEvidence/superseded-run1/`.
- Run 1 had a probe line (`Y1_firstReview_footerOrPromptText`) that captured help text, not the footer; replaced in run 2 by `Y1_firstReview_footerTexts`.
- Editor normalisation (`.vscode/settings.json` and eight to ten `Art/SectionFive` materials) was reverted after each run. No unexpected tree change in either receipt.
- Run time 245 s and 241 s.

## Not covered

HUD behaviour beyond the texts above, 1920x1080 and 1024x768 layouts (job B5/B6), saves made by earlier versions, the 170-check U4 matrix (it stands from `verify/scenario-one-two-cycle-eb7fd3d`), and anything on a Player build.

## Evidence

`UnitySmokeEvidence/`: `evidence-guidance-smoke.txt` (every check and note, final run), `results-guidance-smoke.json` and `guidance-smoke.markers.txt` (receipt: PASS, exit 0, run head, interactive true, no compile errors, no unexpected tree change), six captures downscaled from 3840x2160 to 1600x900 JPEG (about 1.5 MB in all; the full-resolution PNGs, about 48 MB, stay in the worktree's untracked `Build/B2/data/`), `superseded-run1/`.
Reproduce: inside `Tools/Build/S1B/with_demo_hdr_quarantined.py`, on a quiet machine with a nested X display: `python3 Tools/Verification/Qualification/run_qualification.py --label guidance-smoke --out Build/B2/run --method U4GuidanceSmoke.Begin --file Tools/Verification/U4Guidance/U4GuidanceSmoke.cs --marker U4_GUIDANCE_PASS --env U4_OUT=Build/B2/data --interactive --window 1280x800`.

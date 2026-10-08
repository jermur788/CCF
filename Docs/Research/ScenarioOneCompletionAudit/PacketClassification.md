# Stale packet classification

Classes: **PORT AS WRITTEN** · **PORT WITH CHANGES** · **PARTLY SUPERSEDED** · **FULLY SUPERSEDED** · **DEFER**.

A packet is not a reason to implement. Each row says where any surviving scope goes (S1-A … S1-D, `ImplementationSequence.md`).

## A. Pedagogy research packets (`task/scenario-one-pedagogy-overnight` @ `60674f1`)

| Packet | Class | Reason | Surviving scope goes to |
|---|---|---|---|
| **P0** Gate run-mode manifest | **PARTLY SUPERSEDED** | Implemented on the P1 branch (`ff6b887`, `db21642`). Main's tracked harnesses are still the stale versions; the regression runner overlays them from `1a36ea9` at run time. `ScenarioOneGateModes.md` is not on main | S1-A: land the corrected harnesses and the gate-mode file on main; drop the runner overlay |
| **P1** Teaching copy and framing | **PORT WITH CHANGES** | Still valuable (CCF definition, positive selection, prompt order, map/Work Plan/review purpose, pruning purpose). But: written for save v16; overlaps P2 (`MenuHelpView` conflict, duplicate Crop Tree line, footer); **its clearance sentence "clearing does not change how well seedlings survive or grow" is false under Model 2** and must be replaced by `LearningObjectivesView.ClearanceExplanation(model)` everywhere it appears (`ClearancePreview`, `VegetationClearance`, `WorkPlanView`, `AnnualReviewView`) | S1-A |
| **P2** Positive-selection feedback | **FULLY SUPERSEDED** | Integrated P2 (`43be659` … `e8c7a06`) does this with a better metric and layout | — |
| **P3** Residual-stand Work Plan block | **FULLY SUPERSEDED** | P3 candidate `0ab7399` | — |
| **P5** Annual Review history phase 1 | **PARTLY SUPERSEDED** | Superseded in design by readiness P4 / `AnnualReviewV2.md` / `ForestDiaryV1.md`; its "what changed since your last intervention" idea survives | S1-C |
| **P6** (Later) Tutorial arc + per-forest stage progress + "no thinning" event | **DEFER** | Needs a save change and decisions #8/#24; should be shaped by beginner-test evidence. Per-forest progress becomes necessary only when a second scenario exists | After beginner test; revisit with Scenario Two |
| **P4** (Later) Practice mode on a Year-0 / current copy | **PARTLY SUPERSEDED → DEFER** | The readiness study showed a time-advanced copy of the real forest is an oracle under the fixed seed; replaced by plan comparison (B-lite) plus an authored training stand later | Training stand: Scenario Two era |
| **P7** (Later) Objective redesign | **PARTLY SUPERSEDED** | Superseded by readiness P6 / model D; serialized-asset risk noted there still applies | S1-D |
| **P8** (Later) Permanent sample plots | **DEFER** | Save change; value unproven before testing | After forester review |
| **P9** (Later) Bio Tree | **DEFER** | Needs a habitat-tree state | Later |
| Wind/competition label calibration | **PARTLY SUPERSEDED → DEFER** | Storm bands integrated with D-050 (wind label replaced). Competition label saturation is now mitigated by the P2 list | After beginner test, only if testers misread it |

## B. Readiness packets (`task/scenario-one-completion-readiness` @ `6a0e01f`)

| Packet | Class | Reason | Surviving scope goes to |
|---|---|---|---|
| **P1-INT** Integrate pedagogy copy | **PORT WITH CHANGES** | Base is now save v19, not v17; Model 2 is integrated so the model-aware sentence is mandatory, not conditional; P2 merge notes (`P2ImplementationRecord.md` "Overlaps") must be applied; MenuTutorial failure must be classified first | S1-A |
| **P2** Crop Tree competitors | **FULLY SUPERSEDED** | Integrated on main | Human smoke only |
| **P3** "What you are leaving" + cash warning + terminology | **PARTLY SUPERSEDED** | Residual stand, money and dead-end warning done by P3 candidate. **Not done:** species display names in objectives/review lines/century review, "cohort"/"juvenile" wording, "Approve all pending jobs (n)", readability V2 (no information text below 13 px) | S1-A |
| **P4** Annual Review v2 + Forest Diary + map history | **PORT WITH CHANGES** | Still the right scope. Changes: the storm card already exists on main and slots into DISTURBANCE; no W5 dependency; self-thinning deaths now available; base v19 | S1-C |
| **P5** Progression Tier A + Model 2 copy | **PARTLY SUPERSEDED** | Model 2 copy shipped with D-049. The S1–S12 stage engine should wait for beginner-test evidence. Objective visibility (HUD list, `[O]` showing forest objectives, Year-25 horizon) is small and needed before testing. J3 (preview after U) is a decision that can ride with S1-A if accepted | Objective visibility → S1-A; stage engine → DEFER |
| **P6** Second intervention + completion + failure/recovery | **PORT WITH CHANGES** | Still required. Changes: storms are no longer a precondition (they stay off in Scenario One); drop the per-forest progress save record (derive from events); drop D7 "reviews read" (only the first acknowledgement is saved); second cycle must accept any work type so the cash dead end cannot block completion after a first thinning; standing sale stays out | S1-D |
| **P7** Plan comparison (B-lite) | **DEFER** | Useful but not needed to close the loop; the progression study wants it in every scenario, so build it once with Scenario Two-era machinery | After beginner test / Scenario Two |
| **P8** Playtest fixes (template) | **PORT AS WRITTEN** (as a template) | Per-round copy/UI fixes from real findings | After each test round (not counted in the four packets) |
| **W5** Storm UI and teaching | **FULLY SUPERSEDED** for Scenario One | D-050 integration already provides bands, forecast wording, review card, salvage UI and waypoint. Remaining storm teaching belongs to the storm branch scenario, not Scenario One | DEFER to Scenario Two era (branch B1) |
| H1 / H2 human gates | **PORT WITH CHANGES** | Still required; H1 now needs a standalone build (S1-B) | `ReleaseGates.md` |

## C. Other proposals

| Item | Class | Note |
|---|---|---|
| `PostScenarioRoadmap.md` (Stage 2 before Scenario Two) | **FULLY SUPERSEDED** if the user confirms forestry-first | Record in the Decision Log |
| ScenarioProgression **SCN-0** Scenario package v0 | **DEFER** | Not Scenario One work. Start only after S1-C and S1-D are integrated |
| Forestry expansion waves W0–W4 | **DEFER** | Multi-species foundation; Scenario Three |
| Sapling price recommendation (P3 audit) | **PORT WITH CHANGES** | Documentation now; the adapter override is a one-line economy change owned by the economy owner, best re-gated with the S1-D completion/economy run |

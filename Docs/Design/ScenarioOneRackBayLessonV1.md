# Scenario One rack-area lesson V1 (Objectives 2-5)

Task: SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1. Base: `6bdcdd5c6ec2f868275ae4942ac1caea4b723192` (the Scenario One playtest candidate). Status: **implemented and verified on plain .NET only; not yet compiled or run in Unity** (see "What is and is not verified").

This note records the four non-UI player-test items: the forest track's approach, the exact meaning of the competitor table's `Share` (for the UI task), how Objectives 2-5 worked before (Stage A), the replacement that manages one rack area (Stage B), and a design-only follow-on for repeating the work on other racks (Stage C). Competitor keyboard targeting is described in the code comments of `CompetitorKeyboardTarget.cs`, `CompetitorAssessment.cs` and `ForestTreeMarkingManager.KeyboardTarget`.

## The forest track's approach

**Finding.** The scene is authored as the 40 m stand and stays that way on disk. The track is 52 planks, "Dirt Path 00".."51" (2.6 m wide, no colliders), running from (-0.93, -14.75) in the south to the work clearing in the north (z = 10.75). In the 80 m property (south edge z = -40) the south end therefore began about 25 m inside the plantation, which is what the player saw. Nothing is modelled outside the property: the boundary is four 5 m ridge boxes and the ground ends at the edge.

**What was done (Enlarged80 only; the 40 m world and the Reference Future are untouched).** `StandApproachPath` adds, at run time and only in Play Mode, under one object "Stand Approach":

- more "Dirt Path X000.." planks from the authored south end to the south edge, along `StandApproachRoute`: the track eases from the direction the authored planks leave in round to due south over 7 m (bend never tighter than about 10 m radius, like the authored curves), then runs straight. They share the "Dirt Path" prefix on purpose, so the starting stand keeps its trees clear of them (1.5 m corridor, as for the authored track) and the Stand Map draws them (it reads planks by that prefix and starts a second line where the planks jump): the map needed no design change, only a re-read of the planks when the stand geometry changes (`StandMapSource`).
- an opening 6 m wide in the south ridge where the track meets it (`StandWorldBoundary` calls `StandApproachPath.ShapeSouthRidge`), and beyond it a 10 m long cutting: ground at property level, two side walls and an end wall in the ridge's material, and the rest of the track ("Approach Track NN", ignored by the map and the stand because it is outside the property).
- an invisible blocker across the opening. The cutting is scenery: the player stays inside the ecology bounds, as before. Walking into the opening is stopped at the property line.

The planks are made inside `ForestEcologyController.ApplyStandGeometry`, before the event and before anything plants a stand, so a new game, a load and the Reference preview all see the right world whichever object wakes first. Legacy40 removes them again.

**Cost, stated plainly.** Because the starting stand clears trees round every "Dirt Path" plank, a new Enlarged80 game differs from before: the outer plantation loses the trees on the new corridor (the generator's mortality budget keeps the count at 1,344), and in the core, legacy tree **P0009** is cleared and the budget restores **P1200** (the core still holds 336; 335 keep their exact ID and position; this was predicted by an offline replica of the generator and pinned in `Enlarged80Verification`, to be confirmed by the live run). So the Enlarged80 start-world hash changes: record it as an intentional anchor delta; Legacy40 and the Reference Future v1 anchors must not. A save made before this change loads with trees standing on the new track (they are in the save); this is cosmetic and affects no rule.

**Chosen by the user (2026-10-22):** extend to the edge and add the gateway. The cutting's size and look are my minimum reading of "visibly enters from outside"; they were not specified and need a rendered review.

## What `Share` means (for the UI task)

`Share` is read from `CropTreeCompetition.Analyse` and is exact, not a new interpretation:

- Each living neighbour within **8 m** (`ForestEcologyController.HegyiCutoffMeters`) of the inspected Crop Tree contributes one Hegyi term: `(neighbour DBH / Crop Tree DBH) / distance in m` (distance floored at 0.5 m, Crop Tree DBH floored at 1 cm).
- The Crop Tree's competition index is the sum of those terms over all such neighbours, Crop Trees included, listed or not.
- `Share` = that neighbour's term ÷ the sum. It is a fraction of the *whole* competition the Crop Tree feels, so the shares of all neighbours add up to 100 %. The table lists only the strongest five, so the visible rows add up to less.
- `Strong` is a share of 10 % or more, `Moderate` 5 % or more, `Low` below that (`CropTreeCompetition.StrongShare` / `ModerateShare`; presentation labels, not ecology).

Plain-language wording for the UI task: **"Share is how much of this tree's total competition comes from that one neighbour: bigger and closer neighbours take a bigger share. Felling a neighbour with a 20 % share removes about a fifth of the competition."**

The second sentence is exact for the competition index the game already uses (the index is a plain sum, so removing a term removes its share). It says nothing about how much faster the Crop Tree will then grow; that depends on the growth model and is not stated by `Share`.

## Stage A - how Objectives 2-5 worked before

Source: `LearningObjectivesView.Sequence.cs` (`TeachingFacts`) and `ScenarioOneTeachingSequence.cs` (`Current`) at base `6bdcdd5`. The teaching sequence is derived from the world every time (trees, marks, Work Plan orders, management events); it stores nothing of its own.

| Objective | Moves on when | What counted |
|---|---|---|
| 2 Choose future trees | `CropTrees >= 1` (or a release was already carried out) after the access racks have been reviewed | **Any living Crop Tree anywhere in the stand.** One mark was enough. |
| 3 Release a future tree | `ReleasePlanned` or `ReleaseExecuted` | **Any one** open, valid felling order created after the access work whose tree stands within 8 m of **any one** Crop Tree. One removal, near a single Crop Tree, anywhere. |
| 4 Keep some fallen wood | `ReleaseExecuted` **and** `DeadwoodExecuted` | A completed felling near a Crop Tree, plus **one** successful retained-as-deadwood felling **anywhere** (or the global `fallen-deadwood` goal already achieved). The deadwood tree need not be a release tree. |
| 5 Review and manage again | `ReleaseReviewed` **and** `ReviewedTwoCycles` | An Annual Review acknowledged after the release year, and the protected two-cycle review rule (`ScenarioOneCompletionRules.HasReviewedTwoCycleSequence`). Nothing asked the player to look at the trees again or release further. |

Crop Trees were counted as `trees.Where(IsLiving && IsCropTree)` over the whole scene; competitors were not counted at all (only "is a felling within 8 m of a Crop Tree"); deadwood was one global event.

Why this allowed a trivial completion: the five conditions were all existence tests on different, unrelated trees. One Crop Tree, one or two removals beside it and one deadwood felling on any other tree satisfied Objectives 2-4; Objective 5 then only needed a review. Nothing tied the steps to the same place, to competition actually being relieved, or to a decision about timber versus deadwood being made alongside the others.

## Stage B - the replacement: one rack area, managed over four objectives

The objective ids, titles' numbering and order are unchanged (`future-trees`, `release`, `habitat`, `response`); only their conditions and wording change, and only in a stand with a rack layout (the 80 m stand). A 40 m Legacy stand keeps the Stage A rules exactly.

**The teaching area.** The standing trees beside the **middle rack** (rack 3 of 5): 12 m either side of the stand centre along the rack, and half the distance to the neighbouring racks (7.6 m) across it, excluding every rack corridor. The corridors themselves are the access work from Objective 1 and never count as release. The area is not chosen by the player: one fixed, named place keeps the lesson checkable and the player's attention in one spot. (Open question for the Manager: whether the player should choose the rack. See "Product decisions".)

**Objective 2 - choose future trees.** The area needs **three future trees**. A Crop Tree counts as a future tree when it stands in the area, is **not overtopped** (at least half the neighbours within 8 m are no bigger than it) and is **at least 8 m** from another future tree (the competition reach, so each has growing space of its own). The wording tells the player when a marked Crop Tree does not count and why (too close to another, overtopped, or outside the area). Marking Crop Trees elsewhere does not progress the lesson.

**Objective 3 - release them.** For each future tree, the game measures the share of that tree's competition (the same Hegyi terms and shares as the inspection card) that is in the Work Plan or already removed. The lesson moves on when **every** future tree has at least **30 %** of its competition planned for removal and the area stays a forest (removed basal area at most 40 % of the area's). One removal that serves two future trees counts for both. Marks that are not yet in the Work Plan are shown as progress but do not count.

**Objective 4 - what happens to the felled trees.** The removals made for the future trees must include **at least one sold and at least one left as deadwood**, carried out (approved and a year advanced). The two fates are compared tree by tree in the Work Plan, so a decision about timber versus habitat is made inside the same job.

**Objective 5 - review and release further.** After the Annual Review is acknowledged (the release year is the year the work was carried out), the player re-inspects the future trees and removes more until every future tree has at least **40 %** of its competition gone, then reviews again. The protected two-cycle review rule still applies. A player who reached 40 % in the first round has nothing left to repeat and moves straight on.

**What stays derived, what is stored.** No new state, event or save field. The area, the future trees and every share are recomputed from living trees, stumps, Crop marks, Work Plan orders and management events. A save/load therefore always lands on the same lesson, and the lesson cannot be advanced by a stale value. A finished release is judged with 10 % give (`ExecutedTolerance`) because survivors keep growing while a stump keeps the diameter it was felled at, so the removed share slowly thins; growth can never un-finish it.

### What is established and what is teaching calibration

- Used unchanged from the game: the Hegyi competition term, its 8 m reach, the competitor shares, the Crop Tree mark, the Work Plan, selling versus leaving a felled tree as fallen deadwood, the Annual Review.
- Established forestry practice (qualitatively): positive selection of well-formed, dominant trees to favour, and releasing them by removing their competitors.
- **Simplified teaching calibration [D]**, flagged as assumptions, not silvicultural prescriptions: three future trees; 8 m spacing; "not overtopped" at 50 %; 30 % then 40 % competition release; the 40 % cap on removed basal area; the 10 % give. They only decide when the lesson advances. They do not gate the scenario, change any rule, or alter any model coefficient. A silviculturist should review them before they are quoted as guidance.

### How the old trivial route is blocked

The offline check replays the old route and shows: one Crop Tree no longer completes Objective 2; two removals do not complete Objective 3 or the release; a deadwood felling elsewhere does not complete Objective 4; Crop Trees or removals outside the area, or within 8 m of another future tree, change nothing. Real management takes about 14 removals in the checked stand (the area keeps most of its canopy: removed basal area about 22 %).

## Stage C - automation follow-on (design only, not implemented)

**Decision: not implemented.** The similarity thresholds below are new calibration that has to be agreed, the preview needs UI that is owned by the UI task, and creating Work Plan orders in bulk touches the economy and Work Plan pipeline. None of that is "completely unambiguous and safe" yet. The design is written so that it can be added later without changing the analysis above.

**Where it appears.** Offered only after Objective 5 is complete on the teaching area, and then as an optional action on any other rack's area (not part of the objectives).

**What counts as a similar rack area** (all must hold; thresholds are [D] proposals):

- Same geometry rules (strip beside one rack, corridors excluded) and the same species and age class.
- Comparable structure to the teaching area *before* its treatment: living tree count within 20 %, mean DBH within 15 %, mean Hegyi competition index within 20 %.
- No planted juveniles, browse shelters, retained deadwood records or storm damage in the area that the teaching area did not have, and no clearing, path or built structure inside it.
- Not already managed by the player in a way that conflicts (existing Crop Trees or open orders must be resolved first).

**What can be copied (the rule, not the trees).** The release target (share of each future tree's competition to remove, 30 % then 40 %); the order of removal (strongest competitor first, a competitor serving two future trees preferred); the canopy cap (removed basal area at most 40 %); and the fate policy for removals (for example stems too small to sell go to deadwood), as a *suggestion*.

**What must still be inspected manually.**

- **Which trees are future trees.** Choosing trees worth keeping needs the player to look at each one. The tool may highlight candidates by the same rules as Objective 2, but the player must mark each Crop Tree (`C`) themselves; automation starts only from the player's Crop Trees.
- Any removal that would open more than the cap, remove a Crop Tree, or touch an unusual tree (very large, with regeneration beneath it, recorded deadwood).
- The sold-versus-deadwood decision for any stem large enough to have timber value (an economic and habitat judgement).
- Timing: whether to act this year.

**Preview.** A *proposal* only: the removals are drawn on the Stand Map and in the world, with a summary per future tree (competition before and after, using `Share`), removed basal area against the cap, number of trees, sale value against value forgone as deadwood, and the cost. A "needs your inspection" list names anything above. Nothing is written to the Work Plan yet.

**Approval.** Two steps. The player reviews the preview and chooses to add it; the tool then creates ordinary *pending* felling orders in the Work Plan. They go through the normal Work Plan approval, execution and Annual Review, so the safeguards and the economy are unchanged and every order can be removed one by one. The tool never approves or executes anything itself.

**When another rack is materially different** (outside any threshold, or with an extra feature above): the tool is not offered for that rack and says which test failed (for example "much denser than the area you managed"). The player manages it by hand, as in the lesson. No partial or "closest match" automation, so it cannot reproduce a selection that suited a different stand.

## What is and is not verified

Verified (no Unity needed):

- Plain .NET, `Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py`: the competitor keyboard rules (36 checks), the rack-area analysis, sequence order, wording and the old-route checks (128), and the track route (22: it continues the authored direction with no kink, runs south at even spacing to the edge, joins the map as one line, sorts after the authored plank names, and clears exactly core tree P0009). These prove decision rules and arithmetic, not Unity behaviour.
- Syntax/type compile of all runtime C# (156 files; 160 with the four Unity gates) against the Unity runtime assemblies of a built Player: 0 errors; the base commit also compiles with 0 errors, and a deliberate error is caught. This is not Unity's own compile.

**Not verified (needs a Unity run under the lock; U1 is reserved for the UI task, U3 is not a registered lane):** compilation by Unity itself, real key input, the live reader and Learning panel, the track and the opening as built, the Stand Map's drawing of them, save/load in the running game, the Work Plan and Annual Review, a rendered review of the cutting, and the protected scenario gates. Three disposable gates are written for this and have not been run: `CompetitorKeyboardVerification`, `RackLessonVerification`, `StandApproachVerification` (`Tools/Verification/ScenarioOneNonUi/UnityGates`, README beside them). The pinned P0009 / P1200 swap comes from a float64 replica of the generator until the live run confirms it.

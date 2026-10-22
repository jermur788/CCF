# Scenario One rack-area lesson V1 (Objectives 2-5)

Task: SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1. Base: `6bdcdd5c6ec2f868275ae4942ac1caea4b723192` (the Scenario One playtest candidate). Status: **V2 Manager amendment implemented; non-Unity qualification only, live Unity gates outstanding.** Independent SOL review of checkpoint 5c6033f was NEEDS CHANGES. The accepted amendment replaces the earlier lesson below; it does not grant integration approval.

This note records the four non-UI player-test items: the forest track's approach, the exact meaning of the competitor table's `Share` (for the UI task), how Objectives 2-5 worked before (Stage A), the replacement that manages one rack area (Stage B), and a design-only follow-on for repeating the work on other racks (Stage C). Competitor keyboard targeting is described in the code comments of `CompetitorKeyboardTarget.cs`, `CompetitorAssessment.cs` and `ForestTreeMarkingManager.KeyboardTarget`.

## The forest track's approach

**Finding.** The scene is authored as the 40 m stand and stays that way on disk. The track is 52 planks, "Dirt Path 00".."51" (2.6 m wide, no colliders), running from (-0.93, -14.75) in the south to the work clearing in the north (z = 10.75). In the 80 m property (south edge z = -40) the south end therefore began about 25 m inside the plantation, which is what the player saw. Nothing is modelled outside the property: the boundary is four 5 m ridge boxes and the ground ends at the edge.

**What was done (Enlarged80 only; the 40 m world and the Reference Future are untouched).** `StandApproachPath` adds, at run time and only in Play Mode, under one object "Stand Approach":

- more "Dirt Path X000.." planks from the authored south end to the south edge, along `StandApproachRoute`: the track eases from the direction the authored planks leave in round to due south over 7 m (bend never tighter than about 10 m radius, like the authored curves), then runs straight. They share the "Dirt Path" prefix on purpose, so the starting stand keeps its trees clear of them (1.5 m corridor, as for the authored track) and the Stand Map draws them (it reads planks by that prefix and starts a second line where the planks jump): the map needed no design change, only a re-read of the planks when the stand geometry changes (`StandMapSource`).
- an opening 6 m wide in the south ridge where the track meets it (`StandWorldBoundary` calls `StandApproachPath.ShapeSouthRidge`), and beyond it a 10 m long cutting: ground at property level, two side walls and an end wall in the ridge's material, and the rest of the track ("Approach Track NN", ignored by the map and the stand because it is outside the property).
- an invisible blocker across the opening. The cutting is scenery: the player stays inside the ecology bounds, as before. Walking into the opening is stopped at the property line.

The planks are made inside `ForestEcologyController.ApplyStandGeometry`, before the event and before anything plants a stand, so a new game, a load and the Reference preview all see the right world whichever object wakes first. Legacy40 removes them again.

**Cost, stated plainly.** Because the starting stand clears trees round every "Dirt Path" plank, a new Enlarged80 game differs from before: the outer plantation loses the trees on the new corridor (the generator's mortality budget keeps the count at 1,344), and in the core, legacy tree **P0009** is cleared and the budget restores **P0600** (the core still holds 336; 335 keep their exact ID and position). P0600 is the highest-hash mortality omission of the Legacy40 core, so it is the slot the budget gives back first; it stands at (-19.40, -8.15) on the core's west margin, 19.5 m from the track. This is the live result, repeated across two Editor sessions and pinned in `Enlarged80Verification`. An earlier offline replica predicted P1200 because it used the code's default 1.9 m lattice spacing, not the scene's 2.0 m. So the Enlarged80 start-world hash changes: record it as an intentional anchor delta; Legacy40 and the Reference Future v1 anchors must not. **A save made before this change keeps the world it was made in.** `ForestSaveController.LoadData` asks `StandApproachPath.SaveHasApproach`, which reads the saved tree record (no new save field): an original plantation tree - living, dead or a stump - inside the 1.5 m corridor of the extension marks an old world, so the pieces are removed and the south ridge is closed. A world made with the approach has no original tree in that corridor, so the pieces are built and the ridge opened. The test is only about the extension south of the authored track's end and only inside the property; recruits, plantings and fixtures do not count. A new game always wants the approach, even after an old save was loaded in the same session.

**Can the route avoid the core tree?** Not without a visibly poorer track. The authored south end at (-0.93, -14.75) runs away from the clearing at about -153 deg; P0009 stands 4.1 m south and only about 0.7 m west of the natural continuation. The existing smooth 7 m bend keeps a minimum radius of about 9.9 m and passes within 0.73 m of P0009. Shortening the bend to avoid it tightens the radius below the authored track's own curves (about 6 m at a 5 m bend) and still does not clear the 1.5 m corridor. Moving P0009 is excluded because the Legacy40 core positions are preserved. The one-tree cost is therefore the minimum price of a natural approach; it is pinned in `Enlarged80Verification` and awaits live proof.

**Chosen by the user (2026-10-22):** extend to the edge and add the gateway. The cutting's size and look are my minimum reading of "visibly enters from outside"; they were not specified and need a rendered review.

## What `Share` means (for the UI task)

`Share` is read from `CropTreeCompetition.Analyse` and is exact, not a new interpretation:

- Each living neighbour within **8 m** (`ForestEcologyController.HegyiCutoffMeters`) of the inspected Crop Tree contributes one Hegyi term: `(neighbour DBH / Crop Tree DBH) / distance in m` (distance floored at 0.5 m, Crop Tree DBH floored at 1 cm).
- The Crop Tree's competition index is the sum of those terms over all such neighbours, Crop Trees included, listed or not.
- `Share` = that neighbour's term ÷ the sum. It is a fraction of the *whole* competition the Crop Tree feels, so the shares of all neighbours add up to 100 %. The table lists only the strongest five, so the visible rows add up to less.
- `Strong` is a share of 10 % or more, `Moderate` 5 % or more, `Low` below that (`CropTreeCompetition.StrongShare` / `ModerateShare`; presentation labels, not ecology).

Accepted plain-language wording for the SOL UI task: **"Share shows how much of this Crop Tree's current competition comes from that neighbour. Larger and closer neighbours usually contribute more. A neighbour with a 20% share currently accounts for about one fifth of the measured competition around the Crop Tree."**

The statement is exact for the competition index the game already uses (the index is a plain sum, so removing a term removes its share). It deliberately does not imply that removing 20 % of the competition produces exactly a 20 % future growth response; growth depends on the model and is not stated by `Share`.

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

## Stage B — accepted Manager amendment V2 (22 October 2026)

This section supersedes the rejected checkpoint's selection/release requirements. Accepted outcome: by the end of Objectives 2–5 the player has meaningfully managed the whole teaching area associated with one rack and learned how retained trees, competitors, removals and deadwood interact spatially.

### Footprint and selections

The fixed middle layout rack supplies actual Start/End endpoints to `ScenarioOneRackBay.Frame`. Its midpoint and unit direction define along/across projections. The teaching woodland extends 12 m each way from that midpoint and halfway toward the neighbouring racks (7.6 m here), excluding the 2.4 m access corridor. The core rules accept rotated/translated rack frames; the live authored layout is still unchanged. Near/middle/far are deterministic along-rack thirds, with internal boundaries at −4 m and +4 m; near starts at the rack's Start end. Boundary points at −4 m enter middle and +4 m enter far. No player-relative/world-axis zone rule is used. Inspection labels identify the tree's zone and side.

Keep **exactly three Crop Tree selections inside this footprint**, one per zone, at least 8 m apart, collectively on both sides. Additional Crop Trees elsewhere remain valid gameplay. The unsupported DBH-rank/overtopping eligibility rule is removed; promising individuals are the player's choice. There is no new dominance/crown model. Eight-metre neighbourhoods can overlap below 16 m crop separation; at exactly 8 m the existing Hegyi model still includes the other tree. Spacing is not a guarantee of crown position or longitudinal spread.

### Objectives 2–5

2. **Choose promising trees to retain along the rack.** Inspect, compare, retain and distribute attention across near/middle/far and both sides.
3. **Identify the competitors that matter.** Compare Share, size/distance and spatial relationships. Initial plan credit must supply approximately **30%** of measured competition around every selection. Marks show a preview; valid Work Plan orders establish planned progress. All living neighbours within the unchanged Hegyi radius remain in the denominator, along with recorded non-access removals when reconstructing executed shares. Outside stumps contribute to that denominator only, so outside felling cannot inflate inside-work credit merely by disappearing. Only removals **inside the footprint** contribute to the credited numerator and outcomes. Outside work cannot satisfy the lesson.
4. **Plan and carry out the intervention.** Both sold timber and fallen deadwood must occur among the relevant in-footprint removals. Retain structure, approve and resolve through the existing Work Plan. The footprint-wide safeguard includes all in-footprint felling, including removals not close enough to a selected tree to supply release/fate credit.
5. **Return and review.** After executed initial release and acknowledgement of its results, genuinely reopen inspection on all three retained trees. In the forest inspection panel, **[J] Retain here** records no additional removal needed; **[K] Release further** records a player decision backed by relevant in-footprint Fell marks/orders. There is **no 40% second-release target and no compulsory additional cut**. Inspect and confirm retained woodland in each third on both sides (six portions); any retained representative can supply that portion's review, including a selected Crop Tree. No one-per-side-per-zone crop selection rule or cut-per-portion rule is added. An entire portion can be retained without work. A tree marked/planned to fell cannot stand in for retained woodland in that review.

All three tree decisions, six surrounding-stand decisions and current safeguard must hold to complete. A retain decision must agree with no outstanding credited removal around that tree; a further-release decision must still have a relevant mark/order. Optional further work can remain planned at this boundary; execution is not compulsory to finish the lesson. If more in-footprint felling is executed, earlier review evidence is invalidated and the latest results must be acknowledged/revisited. The existing global two-cycle **scenario success** rule is unchanged; it is no longer an extra-cut requirement inside this rack teaching boundary. Objective 1 and Objectives 6+ retain their existing rules and text.

### Real inspection evidence without a save-schema change

`ForestPlayer.InspectionSerial` increments only when its existing real InspectTree interaction opens an inspection. `ScenarioOneUiRoot` feeds that serial and the actually inspected tree to `ScenarioOneRackReview` only while walking/inspecting, outside help/reference/menus. Keeping a panel open across the Annual Review cannot count as a revisit. Annual acknowledgement alone records no tree visit or decision. J/K are contextual inspection controls, not global commands.

The checklist is session-only teaching state, owned by `LearningObjectivesView`. It is not PlayerPrefs completion, a management event, a world simulation rule or a saved field. `ScenarioOneManager.TeachingWorldVersion` is a non-serialized lifecycle counter incremented on new scenario/load; it invalidates the cached checklist even on a same-year/same-save reload. Load/reference return therefore requires fresh post-release inspection and decisions. Changing the selected group or executing more in-footprint work also invalidates earlier review. Inspection evidence is intentionally conservative; it does not persist between sessions. No save format/version or reference archive is changed.

### Provisional safeguard and calibration classification

**40% cumulative basal-area safeguard remains GAMEPLAY CALIBRATION.** Numerator: recorded felled stump BA plus valid open felling orders and loose Fell marks inside the entire footprint. Denominator: current living BA plus recorded felled-stump BA there, excluding access. This is explicitly a current reconstruction, not an immutable historical baseline or canopy-cover calculation; survivor growth/natural losses can change the ratio. No claim that it proves real-world stability/cover or reconstructs historical breaches is made.

The safeguard gates initial planning, executed-release facts and Objective 5 completion. Interactive Work Plan approval and year advance also refuse an over-cap footprint, including a later draft. Already executed over-cap stands cannot pass the normal lesson predicates; loading does not manufacture review evidence. Simulation/reference verification APIs and global success/economy rules are unchanged. There is no normal-success bypass for executed over-felling.

- **Forestry principle:** positive selection; meaningful competitor relationships; retained structure/deadwood; inspect response and adapt, including deciding not to cut. Supported qualitatively by project book/photos/research; see the independent review for inspected pages and provenance limits.
- **Model-derived:** existing Hegyi size/distance relationship and 8 m radius, used unchanged. Its shares describe this simulation, not field-measured crown responses.
- **Teaching/gameplay calibration:** three trees, 8 m crop spacing, thirds/both-side sampling, 30% first release, 40% trunk-area safeguard, six-portion stand review and existing 0.9 executed tolerance. Executed first-release threshold remains 27% on the reconstructed denominator to allow modest growth; no new retuning was made.
- **Removed claims:** DBH rank proves overtopping; 30/40 competition percentages come from the book; two roughly 20% cuts establish a 40% cumulative allowance; all valid triples/any removal plan are safe; completion of a larger numerical quota proves the whole area was inspected.

The corrected fixture still contains 76 living trees and 1.51987 m² initial woodland BA. The representative three trees P0714/P1109/P0706 need **14 inside-footprint removals**, about **24.362%** of strip BA, with minimum initial share **30.485%** (no growth). This is source-rule arithmetic on the replica, not live Unity or ecological validation. It replaces the old 14-total/13-inside example and its 22.2% figure. No forced second-release calibration remains. Full traversal and perceived tedium/visibility must be tested in Unity before tuning the 30% value again.

### Verification and ownership

U3 owns lesson logic and inspection/progression hooks by explicit user assignment; U1 presentation stays separate for later composition. Updated offline checks cover the clustered counterexample, zones/both sides/spacing, rotated frames, footprint credit, initial release, all-footprint safeguard, over-cap execution, inspection chronology, retained-stand decisions, load invalidation and optional additional work. The live `RackLessonVerification` now exercises actual Work Plan/Annual Review, synthetic keyboard inspection/decision hooks, zero-additional-felling completion and in-memory reload. It remains a disposable harness and has **not been run in Unity**. Original approach/keyboard gates and protected gates remain required on the same checkpoint in a registered Unity lane. No automation is implemented.

## Stage C - automation follow-on (design only, not implemented)

**Decision: not implemented.** The similarity thresholds below are new calibration that has to be agreed, the preview needs UI that is owned by the UI task, and creating Work Plan orders in bulk touches the economy and Work Plan pipeline. None of that is "completely unambiguous and safe" yet. The design is written so that it can be added later without changing the analysis above.

**Where it appears.** Offered only after Objective 5 is complete on the teaching area, and then as an optional action on any other rack's area (not part of the objectives).

**What counts as a similar rack area** (all must hold; thresholds are [D] proposals):

- Same geometry rules (strip beside one rack, corridors excluded) and the same species and age class.
- Comparable structure to the teaching area *before* its treatment: living tree count within 20 %, mean DBH within 15 %, mean Hegyi competition index within 20 %.
- No planted juveniles, browse shelters, retained deadwood records or storm damage in the area that the teaching area did not have, and no clearing, path or built structure inside it.
- Not already managed by the player in a way that conflicts (existing Crop Trees or open orders must be resolved first).

**What might later be copied (rules, not tree IDs).** A preview of the initial calibrated release, footprint safeguard and player-selected fate policy; never a forced later-release quota. Which trees to retain and whether further removal is justified remain player decisions. Exact future automation rules are deferred and are not implemented by V2.

**What must still be inspected manually.**

- **Which trees are future trees.** Choosing trees worth keeping needs the player to look at each one. The tool may highlight candidates by the same rules as Objective 2, but the player must mark each Crop Tree (`C`) themselves; automation starts only from the player's Crop Trees.
- Any removal that would open more than the cap, remove a Crop Tree, or touch an unusual tree (very large, with regeneration beneath it, recorded deadwood).
- The sold-versus-deadwood decision for any stem large enough to have timber value (an economic and habitat judgement).
- Timing: whether to act this year.

**Preview.** A *proposal* only: the removals are drawn on the Stand Map and in the world, with a summary per future tree (competition before and after, using `Share`), removed basal area against the cap, number of trees, sale value against value forgone as deadwood, and the cost. A "needs your inspection" list names anything above. Nothing is written to the Work Plan yet.

**Approval.** Two steps. The player reviews the preview and chooses to add it; the tool then creates ordinary *pending* felling orders in the Work Plan. They go through the normal Work Plan approval, execution and Annual Review, so the safeguards and the economy are unchanged and every order can be removed one by one. The tool never approves or executes anything itself.

**When another rack is materially different** (outside any threshold, or with an extra feature above): the tool is not offered for that rack and says which test failed (for example "much denser than the area you managed"). The player manages it by hand, as in the lesson. No partial or "closest match" automation, so it cannot reproduce a selection that suited a different stand.

## Earlier V1 verification record (historical; V2 qualification is separate)

Verified (no Unity needed):

- Plain .NET, `Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py`: the competitor keyboard rules (36 checks), the rack-area analysis, sequence order, wording, old-route and value-bound checks (128), and the track route and old/new save classification (49: it continues the authored direction with no kink, runs south at even spacing to the edge, joins the map as one line, sorts after the authored plank names, clears exactly core tree P0009, and tells an old world from a new one). These prove decision rules and arithmetic, not Unity behaviour.
- Syntax/type compile of all runtime C# (156 files; 160 with the four Unity gates) against the Unity runtime assemblies of a built Player: 0 errors; the base commit also compiles with 0 errors, and a deliberate error is caught. This is not Unity's own compile.

**Not verified (needs a Unity run under the lock; U1 is reserved for the UI task, U3 is not a registered lane):** compilation by Unity itself, real key input, the live reader and Learning panel, the track and the opening as built, the Stand Map's drawing of them, save/load in the running game, the Work Plan and Annual Review, a rendered review of the cutting, and the protected scenario gates. Three disposable gates are written for this and have not been run: `CompetitorKeyboardVerification`, `RackLessonVerification`, `StandApproachVerification` (`Tools/Verification/ScenarioOneNonUi/UnityGates`, README beside them). The pinned P0009 / P1200 swap comes from a float64 replica of the generator until the live run confirms it. **The old-save compatibility path is also not live-proven:** its logic is checked offline, but no Unity run has yet loaded an old Enlarged80 save over a new world and back.

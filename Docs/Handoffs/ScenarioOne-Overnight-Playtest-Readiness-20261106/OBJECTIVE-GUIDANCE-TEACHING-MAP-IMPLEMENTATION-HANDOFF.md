# Stage B / T3 (#296): Objective 1 illustrative five-rack figure, SOURCE IMPLEMENTATION HANDOFF

**Date:** 2026-11-06. **Issue:** [#296](https://github.com/jermur788/CCF/issues/296) under D-072 ([packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-T3-FIVE-RACK-TEACHING-DIAGRAM-IMPLEMENTATION-PACKET.md), [#297 Manager approval and diagram release](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-MAP-SOURCE-APPROVAL-AND-DIAGRAM-RELEASE-20261106.md)).
**Verdict:** **SOURCE IMPLEMENTED + OFFLINE-CHECKED ONLY. NOT Unity-compiled, rendered, input-tested, integrated, independently reviewed or user-accepted.** This is not a self-approval.

The older research-only report `OBJECTIVE-GUIDANCE-TEACHING-MAP-HANDOFF.md`, which explains why source work stopped earlier, is preserved unchanged. The runner checks that.

## Worker, lane and HEADs

- **Worker:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness in T3 Code, dispatched by the DeepSeek orchestrator as the sole implementation sub-agent. The reasoning-effort setting this session exposed was **15 (low band)**. I was not told a requested effort, and I do not claim a higher one.
- **Branch:** `feature/scenario-one-next-action-guidance-v1`. **Worktree:** `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106`.
- **Start HEAD:** `1b68ddcd583ab2bddd43b5ce18365171c0bab3b4`. After fetch it equalled origin and matched the reviewed #297 target. Tracked and untracked porcelain were empty. ZX20 was exFAT rw with 31 G free. No other worktree was on the branch. Live main `active-tasks.md` showed #296 READY with this lane conditionally allocated.
- **Source commit:** `579ffaac27ecbaceca6132cb2c64ecafaf118e75` (pushed). **End HEAD:** the documentation commit that adds this file. A file cannot contain its own hash; the [HANDED-OFF] comment on #296 records it.
- **Not touched:** `/media/jer/Files/CCF-Primary`, `active-tasks.md`, saves, Unity, Editor, Player and builds.

## What changed (exact files)

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/TeachingDiagramPlan.cs` | Added the `ObjectiveOneFigure(Site)` builder and its recorded fixture with provenance. Appended new `Role`/`Kind` values (existing values keep their order) plus `CorridorState`. Added optional `Item.Points/State/SourceId`, `Text.Backed` (new overload; the old constructor is unchanged) and `Stage.Title/Notes/MinTextSize`. The existing `For(lesson)` stages are unchanged and dump byte-identically to base. |
| `Assets/ForestPrototype/UI/TeachingDiagram.cs` | Lesson 0 now draws `Plan.ObjectiveOneFigure(SiteFromAuthority())`; every other lesson still draws `Plan.For(lesson)`. `SiteFromAuthority()` reads `ScenarioOneHeadlandGeometry`, `StandGeometryModel.StandSizeMeters(Enlarged80)` and `RackGeometry.DefaultWidthM`. New painter cases are corridor (planned/draft/harvested), track, pad, clearing, entrance, field mark, stump, north arrow and dashed boundary. The figure's title sits above the frame and its notes below it, using the existing `diagram-label`/`diagram-caption` classes at 20 UI units. A per-stage text floor applies (20 for the figure). Existing painter methods are unchanged (the runner compares them). The element name `rack-lesson-diagram` is kept. |
| `Tools/Verification/ScenarioOneFiveRackDiagram/FiveRackDiagramCheck.cs` | New plain-.NET structural checker (100 checks). |
| `Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py` | New runner: parses authorities, scene and archive; runs source and scope guards, the dotnet build/run, the unchanged-lesson dump comparison and 7 falsifying mutations. |
| This handoff | New. |

`ScenarioOneUi.uss` is **not** changed. I first added two diagram-only rules, then reverted them, because the #297 suite guards the whole stylesheet as protected T2 bytes. The title and notes now reuse existing classes. Nothing else changed: no `.meta`, scene, prefab, save, rack, map, T2, MFM or Work Plan file, and the four #297 map sources are byte-identical (the runner checks all of these).

## What the figure shows

It occupies the same 1000 × 520 design canvas as the lessons, so the Unity proportion audit is unchanged. The map is on the left (5.2 design units per metre, uniform, x east, **north up**, no rotation) and the state key is on the right.

- **Five parallel planned rack corridors** (solid outline, translucent fill). Each runs from the headland entrance (HeadlandX, Z) to the east edge (40, Z) at width 2.4 m. Numbers 1–5 sit at their east ends.
- **Five entrance squares** on the headland, one per rack.
- **Existing western headland** (A), the **single southern feeder** (B) with its two authored junction pads, and the **existing approach track** (C) to the clearing. All casings are drawn before all fills.
- **Standing trees** in all six retained bands (4 interior, 2 outer) and a row of standing trees **inside every planned corridor**. Planning does not clear anything.
- **State key**, shape plus words, colour-independent: existing (opaque band), draft (dashed edges), planned (solid edges, trees stand), entrance (square), field-marked (two vertical stripes, "not felled"), harvested (opaque pale strip with stumps, key only), standing tree (disc). The map itself shows no harvested, draft, field-marked, stump, Fell-colour or slash symbol.
- **Title**, exactly: "Illustrative five-rack plan — not your saved Stand Map. Planning or confirming does not clear the routes."
- **Notes** below the picture:
  - What A, B and C are, and that the feeder joins between racks 1 and 2.
  - Confirming creates pending work only; a field-marked entrance tree still stands and is not a Fell mark; only Work Plan approval commits the work.
  - "About 16–20 m between racks is guidance for this plantation, not a hard rule; the spacings shown are uneven (15.6–19.4 m)." The numbers are computed from the fixture.
  - Approximate-scale / north-up caveat, and that the tree discs are not a tree count.

## Authentic fixture provenance: how each number was derived

| Value | Authority / evidence | How it reached the figure | Re-derived by |
|---|---|---|---|
| Headland (−35.4, −37.7)→(−35.4, 37.7); feeder (−35.4, −27.4)→(−2.436166, −27.4); width 2.6; pad radius 2.2 | `ScenarioOneHeadlandGeometry.cs` constants | Passed in at runtime by `TeachingDiagram.SiteFromAuthority()`. There is no second copy in the plan (guarded by regex). | Runner parses the constants; the checker rebuilds the figure from them and inverts the transform. |
| Stand 80 m, rack width 2.4 m | `StandGeometryModel.StandSizeMeters(Enlarged80)`, `RackGeometry.DefaultWidthM` | Same as above | Parsed and checked |
| Entrance identities PO0203, PO1204, PO2003, PO2803, PO3803 | `ScenarioOneSuggestedRackLayout.EntranceTreeIds` | Recorded as `FixtureEntranceIds` | Checked equal to the parsed source array, in order |
| Entrance Z −35.271175384521484, −16.421764373779297, 0.23823529481887817, 15.793529510498047, 35.23235321044922 | Archived Enlarged80 Rack V2 snapshot `/media/jer/Files/CCF/Verification/RackV2-Combined-20261027T1334Z/unity-combined-write/config/unity3d/DefaultCompany/CCF/forest-save.json`, SHA-256 `e8bb96f483e598e7449f370cf56f928503dc79ec7337307758eff23104b10ac5`, model 1/1, seed 20260914, 1,344 trees. The layout's algebra uses each entrance tree's own Z. | Recorded as `FixtureEntranceZ` with the path and hash as provenance constants. The figure **never opens the file**. | Runner re-reads the archive read-only, checks the hash and model, finds each ID exactly once, and the checker compares as float32. **All five equal.** |
| Gaps 18.849, 16.660, 15.555, 19.439 m (nonuniform) | Consequence of the above | Kept as recorded; not regularised | Checked against the inverse transform; spread > 3 m |
| Approach track (sampled) | `ForestTest.unity` root planks "Dirt Path 51".."00" (every other plank) plus `StandApproachRoute.Build` from "Dirt Path 00" (yaw 27.169°), cut off at Z −43.5 | Recorded as `FixtureTrackXZ` | Checker rebuilds the dense polyline from the scene and the real `StandApproachRoute.cs`. Worst deviation 0 m. The feeder end (PathX, FeederZ) lies on that track (0 m), and the gate X equals PathX. |
| Clearing (−2.771994, 11), radius 4.5 | `ForestTest.unity` "Forest Clearing" position, scale 9 / 2 (the same rule as `StandMapSource`) | `FixtureClearing*` | Equal to the scene values |
| Retained bands | Bands are derived from corridor edges and the property edge: [−40, −36.47], [−34.07, −17.62], [−15.22, −0.96], [1.44, 14.59], [16.99, 34.03], [36.43, 40] | Schematic tree rows: 1.5 m discs, rows ≥ 3.2 m apart, columns 4.2 m apart, clear of existing surfaces | Tree counts wholly inside each band: 16 / 65 / 61 / 60 / 85 / 17. Along the corridors: 16 / 15 / 16 / 16 / 17. |

The fixture and the source still agree with the prior blocked handoff at `7f5278bf`. I did not invent or regularise any value. The tree discs are a schematic, **not a census or real crown positions**.

## Tests run (actual output)

All runs used `python3 -I -B` and .NET 10, with generated output outside the repo.

| # | Command | Result |
|---|---|---|
| 1 | `python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py` (at `579ffaac`; log `/media/jer/Files/CCF/Verification/D072-T3-FiveRackDiagram-20261106/suite-at-579ffaac.log`) | **PASS**: `FIVE_RACK_PASS checks=100`, `FIVE_RACK_SUITE_PASS source_checks=75 failed=0`, 0 build warnings. |
| 1a | ↳ snapshot re-read (hash, model, 5 IDs, float Z) | **PASS** |
| 1b | ↳ exactly 5 corridors / 5 entrances, identities, endpoints, ≤1° direction, width, nonuniform gaps | **PASS** |
| 1c | ↳ headland and feeder from the authority, 2 pads, feeder between entrances 1 and 2 and not on the south edge, casings before fills, no northern spine or diagonal | **PASS** |
| 1d | ↳ track = scene + `StandApproachRoute` (0 m), feeder end on the track, gate = PathX, clearing = scene | **PASS** |
| 1e | ↳ 6 retained bands populated; standing trees inside each corridor; no tree on existing surfaces | **PASS** |
| 1f | ↳ key signatures pairwise distinct with colour ignored; no harvested/stump/draft/field-mark/Fell/slash on the map; wording; planned ≠ Fell; Work Plan commitment | **PASS** |
| 1g | ↳ exact title, 16–20 m guidance wording, computed spread, scale/north/no-census, word boxes in-bounds and non-overlapping (design units), font 22 with a 20 floor | **PASS** (design-unit only) |
| 1h | ↳ deterministic dump across 2 processes, sha256 `af5c982a3f32e61762e370c6bc10dcc0d622772e67be20874788eb0154fbeba8`; 422 unique item IDs | **PASS** |
| 1i | ↳ source guards: Site fields mapped to authority constants; no save/scene/live Stand Map/rack plan/Work Plan access in either file; plan has no `using Unity`; only allowed files changed; 15 protected paths unchanged; USS unchanged; 8 existing painter methods unchanged | **PASS** |
| 1j | ↳ introduction + lessons 2–8: existing check's dump at base vs current, 862 lines | **PASS** (byte-identical) |
| 1k | ↳ 7 mutations: regularised spacing, sixth corridor, cleared corridors, feeder on south edge, draft distinguished by colour only, outer band missing, harvested rack on map | **PASS** (all 7 caught; e.g. regularised spacing fails 7 checks) |
| 2 | `python3 -I -B Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/run_diagram_check.py` | **PASS** `DIAGRAM_PLAN_PASS checks=1330`. Base gives 1310; the +20 is the existing no-white check covering the 5 new roles × 4 alphas. |
| 3 | Same with `--design /media/jer/Files/CCF/Verification/ScenarioOneUxPedagogy-20261021T074859Z/astra/stand.json` | **FAIL, pre-existing**: `DIAGRAM_DESIGN_MATCH_FAIL lines=762`. It fails identically with the base plan (archived design data predates the current intro panels). Not caused by this change. |
| 4 | `python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod` | **PASS** `TYPECHECK_CLEAN` (6 distinct errors = baseline 6). This approximates Unity 2021.3 references plus stubs, **not** Unity 6000.6. A deliberate typo in `TeachingDiagram.cs` via `--replace` was caught (`TYPECHECK_NEW_ERRORS`), so the file is not blind. |
| 5 | `python3 -I -B Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py` | **PASS** `176 passed, 0 failed` |
| 6 | `python3 -I -B Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py` (#297 suite, unmodified) | **FAIL by design** at its first scope guard: "only four authorised display files changed". It pins the production delta vs `7f5278bf` to #297's four map files, so any authorised later Assets change trips it. The later assertions do not run in that invocation. |
| 6a | The same suite from an out-of-repo copy with only that one guard downgraded to a print (diagnostic, clearly not the real gate) | **PASS** `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`, `HEADLAND_SOURCE_AND_OFFLINE_PASS`, including "original verification assertions and Stage T2 UI bytes preserved". |
| 7 | `python3 -I -B Tools/Verification/NextActionGuidance/check_sources.py` (T2) | **38 PASS / 1 FAIL, pre-existing**: "production diff restricted to five owned source/style files" vs `e1be15c1` already fails at the starting HEAD because of the #297 map files. This change adds the two diagram files to that same inventory. |
| 8 | `git diff --check` | **PASS** |
| 9 | Unity import/compile, Editor, `UxPedagogyShots`, `GuidedOnboardingVerification`, `StandApproachVerification`, `StandMapParity`, render at 1280×720 / 1920×1080, grayscale, text measurement, recognition, native input, save/load, Legacy40, Reference Future v1 | **NOT RUN** (forbidden in this task) |

**Author self-review aid, not evidence:** `/media/jer/Files/CCF/Verification/D072-T3-FiveRackDiagram-20261106/preview/five-rack-preview-PIL-NOT-UNITY{,-gray}.png`, made by `render_preview.py` from the plan dump with PIL. It is **not a Unity/UI Toolkit render or screenshot**: fonts, wrapping, stroke floors and panel scale differ. It must not be used for any 720p/1080p, grayscale, legibility or recognition gate.

## Known caveats and open questions for the reviewer / Manager

1. **Legacy40 and other worlds see the same figure.** `TeachingDiagram` is told only the lesson index; `LearningObjectivesView` is read-only here. Objective 1 therefore shows this Enlarged80 Rack V2 illustration in every world model. The title says it is illustrative and not the saved map, but a Legacy40 learner sees an 80 m five-rack picture. This needs a Manager decision on whether that is acceptable or needs a separately owned gate.
2. **Lessons 2–8 keep the conceptual two-rack stand** (in scope: "later objectives unchanged"). A learner moves from five real racks to the two-strip concept picture. This needs user-clarity review.
3. **The meaning of an existing check shifted (not weakened).** `TeachingDiagramPlanCheck.FirstLessonIsRacksOnly` still checks `For(0)`, the conceptual stage that lessons 2–8 build on. That stage is no longer what Objective 1 displays. The new checker covers the displayed figure. The existing check file was not edited.
4. **One world's seeded positions.** Entrance Z comes from tree positions, which are seeded per world. A player's real layout can differ slightly; the title disclaims this.
5. **The approach is a stub** at Z −43.5. The real route continues to −48.5 through the gate cutting, and the ridge and cutting are not drawn. The track samples every other authored plank (0 m deviation from the polyline at the sampled points).
6. **Text fit is unmeasured.** Design size 22 with a 20 UI-unit floor: at frame widths below about 900 UI units the floor makes words relatively larger than their scaled boxes, so key rows (≤ 36 characters) may wrap to two lines. The 70-unit row pitch leaves little slack at about 780 width. The title plus three notes add roughly 5–8 lines at 20 px to the Objective 1 card. The stylesheet comment says the longest objective should fit without scrolling, so `UxPedagogyShots` and the 720p/1080p audit must check clipping and scroll. The 20 px size is an inline constant equal to `--fs-reading`, chosen to avoid touching the T2-protected stylesheet.
7. **Grayscale field-mark contrast.** In the offline preview, the orange stripes on a green disc are only moderately distinct in grayscale. The stripe shape and the words carry the meaning, but the stripe contrast must be checked in Unity.
8. **Inherited #297 notes m-1..m-5** remain carried to the Unity gate unchanged; #296 did not touch map code.
9. **Evidence labels for 16–20 m** (D-064): CCF book p. 40 conditional early-transformation guidance, used as the Scenario One recommended band, never a universal rule. Five racks is Scenario One calibration. The figure says "guidance for this plantation, not a hard rule".

## Future Unity gate (NOT RUN): exact steps and expected observations

On an authorised exact candidate with an exclusive lease (not this lane):

1. Compile in Unity 6000.6.0f1. Expect no errors in `TeachingDiagram.cs`/`TeachingDiagramPlan.cs`.
2. Run the existing `UxPedagogyShots` and `GuidedOnboardingVerification`. Expect `rack-lesson-diagram` visible, proportions kept, no word outside the frame, no overlaps, and the objectives card still reachable.
3. Open Objectives at Objective 1 in a fresh Enlarged80 Rack V2 world at 1280×720 and 1920×1080, in colour and grayscale. Expect:
   - the title above the picture and the three notes below;
   - five parallel corridors with standing trees inside them and five entrance squares on the west headland;
   - the feeder between racks 1 and 2, joined to the approach;
   - trees in all six bands;
   - key states told apart in grayscale;
   - text ≥ 16 physical px, with no clipping or reflow loss.
4. Compare against the Full Stand Map in the same world: the headland, feeder and approach agree, the five suggested racks agree within FR-5 tolerance after the documented uniform transform, and no extra road appears.
5. Run the two-reviewer 4/4 recognition test and the user clarity review.
6. Check Legacy40, Reference Future v1 and old saves, and decide caveat 1.

## Independent review request

Requesting a **separate independent source/geometry/diagram reviewer**, not this author and preferably from a different provider, in a **new review claim and docs-only lane**, as the packet requires. Please:

- inspect `579ffaac` source and the checker's quality;
- re-run `run_five_rack_check.py` and tests 2–8;
- try further falsifying mutations;
- verify fixture provenance against the archive and scene;
- judge caveats 1–3 and 6;
- return a severity-coded **source-only** verdict, with the actual effort logged, before the Manager #200 decision.

No Unity, Windows or Player work is implied.

# D-072 / #297 — full Stand Map existing-headland source handoff

**Date:** 2026-11-06. **State: IMPLEMENTED ON TASK BRANCH; SOURCE/OFFLINE TESTED ONLY.** Not independently reviewed, Manager-approved, integrated, Unity-verified, or accepted. This worker does not self-approve and has not started #296.

## Actual worker, authority and lane

- Actual runtime: OpenAI / Codex harness in T3 Code, `gpt-6.1-sol`, **HIGH reasoning effort**, implementing as the sole sub-agent dispatched by the DeepSeek orchestrator. This records the actual session, not historical Medium preference text; no subscription/quota availability is inferred.
- Branch: `feature/scenario-one-next-action-guidance-v1`.
- Worktree: `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106`.
- Starting HEAD: `7f5278bfd42997da055da63423b6b4c30c7bb8f3`, equal to upstream and live origin at preflight.
- Implementation/tested end HEAD: **`3bc357dcda5203da8cf1b8c28479025d9c096612`**, eight source/test files, normal push succeeded (`7f5278bf..3bc357dc`). The final documentation publication HEAD is the commit containing this handoff, resolvable with `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-STAND-MAP-HEADLAND-REPAIR-HANDOFF.md`; its exact SHA is also posted in the final #297 `[HANDED-OFF]` comment. A document cannot contain its own Git commit hash.
- Authority read: D072-STAND-MAP-HEADLAND-REPAIR-PACKET.md, Manager S0 approval/map release, and all four canonical spec files from `origin/main:specs/scenario-one-stand-map-headland-teaching-alignment-v1/`. Refreshed `origin/main` from `994908791bdea1a3664218c45652110798d37665` to `a3f025b211d019b8c8b5e1de56f6997847878515`; the four spec files had no intervening diff. No merge, rebase or cherry-pick.
- Read shared project workflow/overview/context and live Manager reservation. Actual issue had no competing claim. Posted `[RUNNING]` before the first edit: https://github.com/jermur788/CCF/issues/297#issuecomment-6684071415 .
- Preflight: correct `pwd`/branch/HEAD; tracked and untracked status empty; `/media/jer/ZX20` exFAT **rw**, 477G total / 447G used / 31G available. No Unity/cache allocation was needed.
- The coordination anchor `/media/jer/Files/CCF-Primary` and `Docs/Project/coordination/active-tasks.md` were not edited. No unrelated user work was present in this lane or changed. Source drift audit found only prior #295 changes in `StandMapView.cs` versus main within the inspected map/physical-source set; those feature-branch bytes were preserved.

## Exact changed files

1. `Assets/ForestPrototype/UI/Map/StandMapSource.cs` — attached-component, fail-closed active-surface proof, bounded retries, cadence and constants-to-model projection/read counter.
2. `Assets/ForestPrototype/UI/Map/StandMapModel.cs` — immutable two-segment/two-pad composite, separate `ExistingHeadlandSurfaces`, pure predicate and static hashing.
3. `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs` — eight Full-only static items, in the required union/order beneath existing Paths.
4. `Assets/ForestPrototype/UI/Map/StandMapLegend.cs` — one presence-gated row, `Existing V2 headland / feeder — not a planned rack`.
5. `Tools/Verification/StandMap/HeadlandOffline/HeadlandFixture.cs` — controlled existing path/five-rack fixture and baseline fingerprint executable.
6. `Tools/Verification/StandMap/HeadlandOffline/HeadlandSourceStubs.cs` — explicit Unity stand-ins for extracted source-method checks, not runtime evidence.
7. `Tools/Verification/StandMap/HeadlandOffline/HeadlandOfflineCheck.cs` — pure-map and extracted-source checks.
8. `Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py` — .NET 10 external-project runner, source contracts, source-constant parser and baseline comparison.
9. This handoff document.

No `StandMapSymbols.cs` change: the existing Path/PathCasing palette and Line legend symbol adequately communicate existing infrastructure; existing symbol numbers, styles, Mini flags and contracts remain unchanged. No new dependencies, packages, scenes, prefabs, serialized settings, `.meta`, save fields, routes, world objects, mesh, physics, rack IDs or validation changes.

## Source-to-3D mapping (not a new path design)

The Unity-aware adapter reads the seven **actual `ScenarioOneHeadlandGeometry` constants**, not duplicated numeric geometry. The offline runner parses those named `const float` fields by regex and tests the actual extracted projection against them; it does **not** compile the Unity-dependent geometry class. It also checks the presentation's real rectangle/pad arguments and active/template conditions.

| Map element | Constant mapping | Current source-derived world metres | Existing physical source |
|---|---|---|---|
| Western segment | `(HeadlandX, SouthZ)` → `(HeadlandX, NorthZ)`; `Width` | `(-35.4, -37.7)` → `(-35.4, 37.7)`; 2.6 m | First `AddRectangle`; its two endpoint discs supply the existing round caps |
| Southern feeder | `(HeadlandX, FeederZ)` → `(PathX, FeederZ)`; `Width` | `(-35.4, -27.4)` → `(-2.436166, -27.4)`; 2.6 m | Second `AddRectangle` |
| Western feeder pad | `(HeadlandX, FeederZ)`; `PadRadius` | `(-35.4, -27.4)`; **2.2 m radius**, 4.4 m diameter | First `AddDisc` with `PadRadius` |
| Approach feeder pad | `(PathX, FeederZ)`; `PadRadius` | `(-2.436166, -27.4)`; **2.2 m radius**, 4.4 m diameter | Second `AddDisc` with `PadRadius` |

Exactly **two new segments and two pads**, represented by one composite. The pre-existing approach remains exclusively in unchanged `model.Paths`: it is not a third new segment or a duplicate stub. No northern spine, road extension, new path/gameplay authority, terrain or stem-clearance change. Floats in the captured JSON show ordinary C# single-precision rounding, not different endpoints.

## Gate, cadence and cache evidence

- Pure predicate: only `V2 && Enlarged80 && surfaceActive` shows anything; all eight boolean combinations executed.
- Runtime adapter reads `ui.Manager.RackPlanningV2Active`, the ecology geometry model, and additionally suppresses Reference preview. Requires manager/ecology attachment agreement.
- The current `ForestTest` scene places `ForestStartingStand` and the ecology component on the same GameObject, verified by their actual script GUIDs and serialized GameObject IDs. `ForestStartingStand.Awake` attaches `ScenarioOneHeadlandPresentation` there. No scene-wide fallback is necessary or added: a noncanonical/unresolved attachment fails closed.
- Acquisition accepts exactly one attached presentation and an attached starting stand. A disabled presentation, missing/ambiguous named child, inactive self/hierarchy, absent MeshRenderer or disabled renderer hides the new collection. The named child/renderer is the approved source-backed proof that the existing template-dependent `CreateSurface` succeeded; this adapter never creates a surface and never reads mesh vertices.
- The helper is called from `Refresh` but owns a separate **0.25 s deadline**, so dirty racks, pointer moves and forced view rebuilds cannot accelerate proof acquisition. No-property clears immediately; source/scene/destroyed-component identity changes invalidate immediately. Surface/V2 state changes are observed by the next cadence even if the old scenery key is unchanged.
- Unresolved **local** component acquisition is limited to six attempts with backoff at offsets **0, 0.5, 1.5, 3.5, 7.5, 15.5 s** in the simulated cadence. No global `FindObjects`, `FindFirstObject`, `GameObject.Find`, new `ReadScenery`, tree scan or mesh read exists in the new methods.
- Deliberate bounded fail-closed consequence: if a presentation first appears after that budget is exhausted, the overlay remains hidden until property/source/scene/eligibility identity is reset. Normal attached startup and late child creation after a resolved presentation are covered. This is not an unbounded rediscovery loop.
- `HeadlandProjectionReads` increments only after actual geometry projection. **600 simulated walking/pointer/view refreshes = one projection and one attached lookup**. Hiding does not increment; reappearance/replacement increments once.
- Nonempty overlay data contributes exact float hashes for both segment endpoints/widths and both pad centres/radii plus collection count. Empty collections preserve the old complete model hash; populated collections alter only StaticVersion, not CanopyVersion or DynamicVersion. Sub-centimetre changes in all ten independent geometric inputs invalidate the hash.

## Layer/drawing evidence and preserved consumers

`StandMapDrawPlan.BuildStatic` inserts the explicitly `full`-guarded collection immediately after corridor bands and immediately before original Paths:

1. Both segment casing strokes.
2. Both **outline-only** pad rings.
3. Both tan segment strokes.
4. Both **fill-only**, zero-outline pad discs.
5. All original Path casing/fill items, unchanged.
6. Original rack centre lines and subsequent existing symbols/dynamic entrance/handle items.

The new layer is **eight static items**, with `SourceIndex = -1`, and uses the existing round-cap stroke painter without changing it. No single outlined+filled pad disc is appended after tan strokes. Fixture full static count is **29 → 37**; removing those eight items yields the original Full items byte-for-byte in their original order. This is draw-plan proof, not a claim of observed junction pixels.

- **Mini:** collection insertion/removal leaves complete Mini draw lists byte-identical at sides 128, 196, 520 and 880 across all five overlays. Both actual consumers still request `MapDetail.Mini`: HUD `HudMinimapView` and the embedded `StandMapView` overview. HUD teaching-note/legend content and consumer sources are untouched. StaticVersion can legitimately cause a Mini repaint, but shapes do not change.
- **Old Dirt Path:** `ReadScenery` is byte-identical to starting HEAD; no headland method references `model.Paths`. Counts, indices, widths and points survive projection and source transitions byte-for-byte.
- **Five racks:** the fixture uses the five source-derived Z positions from the preserved earlier teaching-map handoff (PO0203/PO1204/PO2003/PO2803/PO3803). Fixture plan states/IDs are controlled synthetic inputs, not an invocation of the saved-world planner. Five-rack data, bands, centre lines and selected-rack dynamic entrance/handle items remain byte-identical. All actual rack layout/planner/validation/save sources are unchanged.
- **Legacy40/non-V2/reference:** negative source-seam cases produce no new collection, and empty-overlay Full/Mini items, legend and original model hash are byte-identical to starting HEAD. No real Legacy/reference archive or save/load session was run.
- Baseline fingerprint compiled **actual starting-HEAD** pure-map files, not a rewritten expected renderer; compared 40 Full/Mini plans, original legend and model hash with current empty-overlay output. SHA-256: `1e1bd6388f287525d17793c553a6bc2adc88f9e279b50d67c98ba5cb6fdaa6d1`.
- Original `ReadTrees`, `ReadRacks`, `ReadSmallLayers`, `ReadScenery` and `Rebuild` methods are exact baseline matches. Prior #295 UI, shared symbols, old verification assertions, world/save/Legacy/reference code and assets are untouched. Existing tree/scenery scanning behaviour is not falsely described as eliminated; this change adds no scene scanning.

## Commands actually executed and captured output

All commands ran with working directory `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106`. SDK inventory: `8.0.133` and `10.0.114`. New projects target `net10.0`, language version 9; original harness retains its original `net8.0` target. Generated projects, copies and build products are outside the repository under `/media/jer/Files/CCF/Verification/D072-StandMapHeadland-20261106/`.

Final executed test commands (both observed shell exit **0**; repeated during implementation, with no failed run):

```bash
python3 Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py --out /media/jer/Files/CCF/Verification/D072-StandMapHeadland-20261106/shared-map-dotnet
python3 Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py
```

The new runner's default output is `/media/jer/Files/CCF/Verification/D072-StandMapHeadland-20261106/headland-dotnet`. It builds baseline/current fingerprint and extracted-source check executables. The final extracted-source .NET build log reports:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Whitespace commands actually executed: `git diff --check`, `git diff --cached --check` before implementation commit, and `git diff 7f5278bfd42997da055da63423b6b4c30c7bb8f3 HEAD --check` after it. Captured output: **empty stdout/stderr, exit 0** for each.

### Original shared-map suite — unabridged captured stdout

Durable transcript: `/media/jer/Files/CCF/Verification/D072-StandMapHeadland-20261106/shared-map.log`.

```text
PASS V2 suggested is dashed and player rack solid
PASS legacy minimap-only rack is hidden from full map
PASS RACK-V2-07: the minimap draws V2 suggestions dashed and the V2 plan solid, with the Stand Map's own symbols
PASS RACK-V2-07: the minimap and the Stand Map colour each V2 rack symbol identically
PASS V2 legend names both line meanings
PASS V2 full-map legend excludes minimap-only legacy rack
PASS player rack has a legend outside planning mode
PASS no selection or ghost: the dynamic layer holds nothing for racks
PASS selecting a rack leaves the static version alone
PASS selecting a rack moves the dynamic version
PASS a selected rack gets one outline, one entrance square and one handle
PASS every editing item is in the dynamic layer
PASS the handle is centred on the rack's far end
PASS the entrance square is centred on the rack's entrance
PASS selection and handles are not drawn on the minimap
PASS the handle is large enough to grab and small enough to read
PASS a locked rack shows no rotate handle
PASS a ghost proposal does not touch the static version
PASS a valid ghost is a dashed rack with a band
PASS a ghost is dashed, not solid
PASS the ghost is absent from the static plan
PASS a refused ghost is dashed and red (colour is never the only cue)
PASS the ghost and selection add no legend row
PASS ghosts are not drawn on the minimap
PASS a harvested rack is drawn on the full map
PASS RACK-V2-07: a harvested V2 rack is drawn on the minimap as well
PASS a harvested rack is named in the legend
PASS no harvested rack, no harvested legend row
PASS only proposals are dynamic racks
PASS north-west corner of the property is the top-left of the map
PASS south-east corner is the bottom-right
PASS the property centre is the map centre
PASS north is up the map
PASS east is to the right
PASS pixels per metre 7.6
PASS 2000 random world points: identical fractional position at 608 px and at 200 px 1.2e-7
PASS pixel -> world round trip 1.1e-5
PASS 8x8 cells tile the map exactly (row 0 at the bottom, north at the top)
PASS 8x8: each cell's centre is where the transform puts the cell's world centre
PASS 16x16 cells tile the map exactly (row 0 at the bottom, north at the top)
PASS 16x16: each cell's centre is where the transform puts the cell's world centre
PASS world direction -> compass heading (N 0, E 90, S 180, W -90)
PASS heading -> map direction (north is up the screen)
PASS symbol has one style and legend words: Property
PASS symbol has one style and legend words: Path
PASS symbol has one style and legend words: Landmark
PASS symbol has one style and legend words: RackToDo
PASS symbol has one style and legend words: RackPlanned
PASS symbol has one style and legend words: RackOpen
PASS symbol has one style and legend words: RackOther
PASS symbol has one style and legend words: RackPreview
PASS symbol has one style and legend words: Waypoint
PASS symbol has one style and legend words: Player
PASS symbol has one style and legend words: PathCasing
PASS symbol has one style and legend words: RackBand
PASS symbol has one style and legend words: GridLine
PASS symbol has one style and legend words: PlayerDot
PASS symbol has one style and legend words: ScaleBar
PASS symbol has one style and legend words: NorthArrow
PASS symbol has one style and legend words: TreeMarkFell
PASS symbol has one style and legend words: TreeMarkCrop
PASS symbol has one style and legend words: Juvenile
PASS symbol has one style and legend words: Shelter
PASS symbol has one style and legend words: ShelterLapsed
PASS symbol has one style and legend words: RackSuggested
PASS symbol has one style and legend words: RackPlayer
PASS symbol has one style and legend words: RackDraft
PASS symbol has one style and legend words: RackRefused
PASS symbol has one style and legend words: RackSelection
PASS symbol has one style and legend words: RackHandle
PASS symbol has one style and legend words: RackEntrance
PASS symbol has one style and legend words: RackHarvested
PASS each rack state has its own colour
PASS each rack state also differs by line pattern or width, so colour never carries the meaning alone
PASS rack to plan is dashed, rack in the Work Plan is solid, off-layout rack is dotted
PASS rack-planning preview is full-map only
PASS everything on the minimap is also on the full map
PASS the minimap hides the grid, the scale, marks, juveniles, shelters and the rack being planned
PASS legend follows the same filter
PASS RACK-V2-07: a draft (full-map-only) rack is not drawn on the minimap
PASS RACK-V2-07: the confirmed rack and the harvested rack are drawn on the minimap
PASS RACK-V2-07: the full map still draws the draft rack and draws no minimap guidance outside planning
PASS RACK-V2-07: guidance outside planning is drawn dashed on the minimap while Racks holds no suggestion
PASS RACK-V2-07: confirming (FullOnly cleared) changes the static map version so the minimap repaints
PASS both presentations draw something full 37, mini 20
PASS the minimap draws no rack-planning preview
PASS the full map draws the rack-planning preview
PASS every minimap item is a symbol the shared table marks for the minimap
PASS every minimap item has a counterpart on the full map
PASS every item's anchor is at the same fractional position in both 6.0e-8
PASS every path, rack, rack corridor and boundary point is at the same fractional position in both 6.0e-8
PASS the minimap player dot maps back to the player's world position
PASS the facing wedge's apex is exactly on the player dot
PASS the wedge points along the heading, read back from the drawn shape mini 37 full 37
PASS wedge heading round trip every 15 degrees
PASS you are a small dot (about 5 px on the full map), not an oversized marker
PASS rack states are all carried to the minimap (3 to do, 1 planned, 1 open, 1 other)
PASS a draw item has no text field, so a presentation cannot print values from it
PASS items are tagged with their layer
PASS the dynamic layer holds only you, the destination and the rack being planned
PASS the static layer holds none of them
PASS static plus dynamic is the whole plan 32+5 vs 37
PASS standing still (movement under the rounding step) moves no version
PASS walking moves the dynamic version and not the static one
PASS confirming a rack moves the static version and not the dynamic one
PASS dropping the rack being planned moves only the dynamic version
PASS a new tree moves the canopy version
PASS marking a tree moves the mark version and does NOT redraw the canopy picture
PASS a tree growing moves the canopy version
PASS confirming a rack changes the model's content hash
PASS after the change both presentations show it (2 to do, 2 planned)
PASS the plan is deterministic
PASS grid step: 10 m on the 80 m property, 5 m on the 40 m property
PASS the 80 m property has seven interior grid lines each way
PASS grid labels are local metres from the south-west corner: 0, 20, 40, 60, 80
PASS local easting and northing are measured from the south-west corner
PASS scale bar: 20 m on 80 m, 10 m on 40 m
PASS the scale bar is exactly 20 m long at the map's own scale
PASS 14 grid lines on the full map; the minimap has no grid and no scale bar
PASS north is marked on both
PASS a 100 px line with 10 px dashes and 5 px gaps has 7 dashes totalling 70 px 7
PASS the dash pattern carries around a corner (same 70 px of dash in 100 px of line) 70
PASS no pattern returns the whole line
PASS a rack corridor is drawn at the real rack width 2.4 m vs 2.4
PASS one corridor per rack, the one being planned in the dynamic layer
PASS the path has a wider dark edge under it
PASS the path is drawn at its real width
PASS the dash pattern travels with the rack item, so one rack stays one item
PASS the property boundary is a closed dashed line
PASS the ordinary plan carries no mark or shelter rings
PASS the Marks layer rings each marked LIVING tree (a marked stump gets none)
PASS the Browsing layer rings each shelter, effective and lapsed apart
PASS a layer adds to the same base plan; it never replaces it
PASS the minimap draws no marks, juveniles or shelters even when asked for a layer
PASS a mark ring is centred on its tree's world position
PASS Light: shade washes blue, open ground warm, the middle is nearly clear
PASS Regeneration: none is clear and the wash strengthens with the tallest cohort
PASS a planted juvenile alone lifts a square
PASS forest picture 768px: two crowns 43 m east and 26 m south of each other are that far apart on the picture got (412.0,-250.0) expected (412.8,-249.6); 1511/1511 px
PASS forest picture 768px: the crown is drawn at the tree's own radius drawn radius 2.28 m vs 3.00 m
PASS forest picture 768px: open ground is the ground colour, fully opaque
PASS forest picture 256px: two crowns 43 m east and 26 m south of each other are that far apart on the picture got (138.0,-84.0) expected (137.6,-83.2); 195/195 px
PASS forest picture 256px: the crown is drawn at the tree's own radius drawn radius 2.46 m vs 3.00 m
PASS forest picture 256px: open ground is the ground colour, fully opaque
PASS forest picture 768px: a stump is exactly where the transform says its world position is got (265.79,592.50) expected (265.92,592.32)
PASS a stump is drawn pale on the full map and left off the minimap
PASS the forest picture is deterministic: the same trees give the same bytes
PASS drawing a few trees at a time gives exactly the same picture as drawing at once 7 steps
PASS a redraw works from its own copy of the trees, so a change to the live list cannot tear it
PASS no property: just the ground colour, no trees and no failure
PASS 1,400 trees draw on the minimap picture quickly (diagnostic, plain .NET) 23 ms
PASS a legend crown swatch is a crown on a transparent square
PASS a living conifer's outline is pointed: sharper turns than Beech or Oak sharpness sitka 0.053 oak 0.033 beech 0.018
PASS Beech stays a smoother rounded crown than Oak (no conifer-like spikes) beech 0.018 oak 0.033
PASS the conifer's tips reach the tree's own crown radius (the star does not change the radius) 27.0 px vs 28.2 px
PASS the species silhouette is deterministic
PASS a standing dead conifer still draws
PASS stand figures count living, dead and stumps separately 64 20 100
PASS basal area is the cross-section of the living stems per hectare
PASS no trees, stumps or juveniles on the map: no rows for them
PASS you, destination, boundary and grid are listed
PASS the legend lists nothing the scenario does not have (no contours, water, rock, tracks or buildings)
PASS trees are named by their species, and only the kinds that stand are listed
PASS a layer adds its own legend rows and the base legend has none of them
PASS nothing marked and no shelters: the Marks and Browsing legends have no ring rows
PASS the Marks legend lists fell, crop or both, exactly what is marked
PASS a mark on a tree that is no longer living is neither drawn nor listed
PASS when the last fell mark goes, its legend row goes
PASS when the last mark goes, the Marks legend has no ring rows
PASS the Browsing legend lists effective, lapsed or both, exactly what is ringed
PASS when the last effective shelter goes, its legend row goes
PASS a ring row appears only on its own layer
PASS no property: nothing drawn
PASS zero size: nothing drawn
PASS no player and no waypoint: neither is drawn
PASS no property: an empty legend
176 passed, 0 failed
STANDMAP_OFFLINE_PASS
```

### New D-072 suite — unabridged captured stdout

Durable transcript: `/media/jer/Files/CCF/Verification/D072-StandMapHeadland-20261106/headland.log`. **30 source guards + 92 C# checks passed**, including the baseline/current comparison; zero failures.

```text
PASS SOURCE all generated projects/output outside repository
PASS SOURCE unchanged private void ReadScenery()
PASS SOURCE unchanged private void ReadRacks()
PASS SOURCE unchanged private void ReadTrees()
PASS SOURCE unchanged private void ReadSmallLayers(ForestEcologyController eco)
PASS SOURCE unchanged public StandMapModel Rebuild()
PASS SOURCE cache/projection/invalidation adds no scene discovery or mesh reads
PASS SOURCE projection cannot mutate Paths, save, world or geometry
PASS SOURCE attached lookup budget and exponential backoff remain in actual source
PASS SOURCE actual Refresh calls separately throttled source seam once
PASS SOURCE actual source cadence is 0.25 seconds
PASS SOURCE only four authorised display files changed; world/save/racks/Reference/Mini consumers untouched
PASS SOURCE original verification assertions and Stage T2 UI bytes preserved
PASS SOURCE no disposable Assets script or generated metadata
PASS SOURCE production map types and predicate stay Unity-free
PASS SOURCE gate parity with actual manager and successful physical template/surface conditions
PASS SOURCE physical Reconcile active condition still exactly manager V2
PASS SOURCE source-to-3D mapping parity: rectangles, width alias, and exactly authored feeder pads
PASS SOURCE single scene source for ForestStartingStand
PASS SOURCE single scene source for ForestEcologyController
PASS SOURCE attached presentation lookup matches actual starting stand/ecology scene object
PASS SOURCE new collection explicitly Full-only
PASS SOURCE unique authority constant HeadlandX
PASS SOURCE unique authority constant SouthZ
PASS SOURCE unique authority constant NorthZ
PASS SOURCE unique authority constant FeederZ
PASS SOURCE unique authority constant PathX
PASS SOURCE unique authority constant Width
PASS SOURCE unique authority constant PadRadius
AUTHORITY_CONSTANTS {'HeadlandX': '-35.4', 'SouthZ': '-37.7', 'NorthZ': '37.7', 'FeederZ': '-27.4', 'PathX': '-2.436166', 'Width': '2.6', 'PadRadius': '2.2'}
PASS SOURCE baseline/current Full+Mini draw lists, legend and model hash byte-identical without surface (40 plans)
BASELINE_CURRENT_FINGERPRINT_SHA256 1e1bd6388f287525d17793c553a6bc2adc88f9e279b50d67c98ba5cb6fdaa6d1
PASS pure truth table 0
PASS pure truth table 1
PASS pure truth table 2
PASS pure truth table 3
PASS pure truth table 4
PASS pure truth table 5
PASS pure truth table 6
PASS pure truth table 7
PASS source constants parity: western endpoints
PASS source constants parity: feeder endpoints, no approach stub
PASS source constants parity: both widths and two pads
GEOMETRY {"Headland":{"Start":{"X":-35.4,"Z":-37.7},"End":{"X":-35.4,"Z":37.7},"WidthM":2.6},"Feeder":{"Start":{"X":-35.4,"Z":-27.4},"End":{"X":-2.436166,"Z":-27.4},"WidthM":2.6},"HeadlandJunction":{"Centre":{"X":-35.4,"Z":-27.4},"RadiusM":2.2},"PathJunction":{"Centre":{"X":-2.436166,"Z":-27.4},"RadiusM":2.2}}
PASS visibility bumps only static version
PASS deterministic static hash and unchanged restamp
FULL_STATIC_COUNT BEFORE=29 AFTER=37 HEADLAND=8; two casing strokes, two pad outlines, two tan strokes, two pad fills
PASS exactly eight new full static items; all old items/order identical
PASS layer immediately after rack bands and before original Paths
PASS both casings and both pad outlines before all tan fills
PASS two segments and two separate outline/fill pads, no late dark disc outline
PASS exact projected endpoints/width/casing 0
PASS exact projected endpoints/width/casing 1
PASS pad centre/radius 0
PASS pad centre/radius 1
PASS selected rack entrance/handles/dynamic plan unchanged
PASS Dirt Path and five-rack positions/plan byte-identical
PASS presence-gated legend reuses Path, distinguishes rack plans
PASS Mini byte-identical, side 128 overlay None
PASS Mini byte-identical, side 128 overlay Light
PASS Mini byte-identical, side 128 overlay Regeneration
PASS Mini byte-identical, side 128 overlay Browsing
PASS Mini byte-identical, side 128 overlay Marks
PASS Mini byte-identical, side 196 overlay None
PASS Mini byte-identical, side 196 overlay Light
PASS Mini byte-identical, side 196 overlay Regeneration
PASS Mini byte-identical, side 196 overlay Browsing
PASS Mini byte-identical, side 196 overlay Marks
PASS Mini byte-identical, side 520 overlay None
PASS Mini byte-identical, side 520 overlay Light
PASS Mini byte-identical, side 520 overlay Regeneration
PASS Mini byte-identical, side 520 overlay Browsing
PASS Mini byte-identical, side 520 overlay Marks
PASS Mini byte-identical, side 880 overlay None
PASS Mini byte-identical, side 880 overlay Light
PASS Mini byte-identical, side 880 overlay Regeneration
PASS Mini byte-identical, side 880 overlay Browsing
PASS Mini byte-identical, side 880 overlay Marks
PASS repeat full projection byte-identical
PASS independently projected equivalent model hash identical
PASS sub-centimetre geometry invalidates static hash field 0
PASS sub-centimetre geometry invalidates static hash field 1
PASS sub-centimetre geometry invalidates static hash field 2
PASS sub-centimetre geometry invalidates static hash field 3
PASS sub-centimetre geometry invalidates static hash field 4
PASS sub-centimetre geometry invalidates static hash field 5
PASS sub-centimetre geometry invalidates static hash field 6
PASS sub-centimetre geometry invalidates static hash field 7
PASS sub-centimetre geometry invalidates static hash field 8
PASS sub-centimetre geometry invalidates static hash field 9
PASS collection count participates in static hash
PASS hide invalidates static layer and removes only new legend
PASS fail closed: Legacy40
PASS fail closed: non-V2 Enlarged80
PASS fail closed: Reference preview
PASS fail closed: no manager
PASS fail closed: disabled presentation
PASS fail closed: inactive surface
PASS fail closed: inactive parent hierarchy
PASS fail closed: disabled renderer
PASS fail closed: missing renderer
PASS fail closed: duplicate named surface
PASS fail closed: ambiguous attached presentations
PASS fail closed: missing attached starting stand
PASS fail closed: wrong manager/ecology attachment
PASS missing template/surface fails closed
PASS late physical surface appears at next cadence
STEADY_REFRESH FRAMES=600 PROJECTIONS=1 ATTACHED_LOOKUPS=1
PASS 600 walking/pointer/view refreshes: one projection, one attached lookup
PASS surface loss hides without projection
PASS before 0.25 second deadline no visibility read
PASS 0.25 second deadline reacquires surface once
PASS V2 to non-V2 on unchanged scenery key hides
PASS non-V2 to V2 on unchanged scenery key projects once
PASS no property clears immediately, independent of cadence
PASS property return reacquires attached presentation
PASS destroyed child hides
PASS replacement child reprojects once
PASS Unity-style destroyed presentation invalidates cached reference
PASS destroyed presentation replacement acquired after backoff
PASS scene identity invalidates cache immediately
PASS source transitions preserve existing Paths and five racks byte-for-byte
PASS unresolved component: six local retries total across 60 seconds
PASS retry backoff deadlines, never 4 Hz discovery
PASS exhausted proof budget remains fail-closed
PASS eligibility transition resets bounded acquisition budget
PASS source/ecology identity reacquires without stale surface
HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0; Unity/runtime/render NOT RUN
HEADLAND_SOURCE_AND_OFFLINE_PASS; Unity compile/live linkage/render/input/build NOT RUN
```

## Known unknowns / explicit NOT RUN

- **Unity import/compile and Editor execution: NOT RUN**, source-only release prohibits launching Unity. Pure .NET success is not a Unity compiler result. Only real Unity-free production map code and extracted new adapter methods compiled; no claim that the complete Unity-dependent source compiled in Unity.
- **Live active-surface linkage/lifecycle and cadence performance: NOT RUN in Unity.** Tests use clearly labelled component/time/destroyed-object stand-ins. The wrapper drives exact extracted production methods/no-property statements and statically checks their real Refresh invocation; it is not the actual full Refresh loop, scene or renderer.
- **Native movement/input/map drag/Confirm/keyboard/prompt/Help interaction: NOT RUN.** Existing #295 UI bytes were preserved, not runtime-qualified here.
- **1280×720 / 1920×1080 colour/grayscale rendered Full map/legend, pad union, approach junction and Mini screenshots: NOT RUN.** No generated diagram, screenshot or source-derived image is presented as Unity evidence.
- **StandApproachVerification and StandMapParity Unity end-to-end gates: NOT RUN.** Their source assertions were not changed, weakened, or rebaselined. No claim of save/load, rack-plan, field-marking, Work Plan/MFM or real Legacy40/frozen Reference Future v1 runtime equivalence.
- **Historical Objective CTA feedback checker: NOT RUN / not repaired**; separate #299 ownership/repair remains outside this lane. Its prior obsolete-anchor exit 2 is not called PASS.
- **Full #295 `Tools/Verification/NextActionGuidance/check_sources.py`: NOT RUN for this expanded diff.** It was inspected and remains byte-identical; its historical five-production-file scope inventory deliberately does not include this authorised four-file #297 map change. No check was removed or relaxed to manufacture a combined pass. The new suite separately checks the original #295 UI bytes are untouched.
- **Player/Windows build and user playtest: NOT RUN / NOT AUTHORISED**. No Editor, Player, build, personal save, geometry, scene, frozen reference, package or project-setting write occurred.
- **Independent source review / Manager verdict / integration: pending.** No self-approval, main merge, #296 work, new successor claim or coordination-file edit.

## Later authorised Unity qualification (instructions only, not execution)

After independent source review, Manager approval, accepted subsequent Stage B work and a new legitimate exclusive Unity lease, use an isolated candidate on **Unity 6000.6.0f1**:

1. Import/compile the exact integrated head; require zero Unity C# errors. Inspect the existing verification harnesses and save backup/restore behaviour before any run, then follow their documented copy/run/remove workflow, removing disposable scripts and generated `.meta` afterwards. Existing map gate entry is `-executeMethod StandMapParity.Begin`; keep original assertions.
2. In `ForestTest`, start the authorised Enlarged80 Rack V2 case. Open Stand Map with **M**. Compare visible physical west headland/feeder against the Full map: exactly two strips/two pads, feeder stops on the old approach, five authentic racks and entrance symbols remain visible, legend distinguishes existing surface from planned racks. Capture at 1280×720 and 1920×1080, colour and grayscale; inspect for dark pad-ring/approach cut-through or apparent extra road.
3. Capture HUD Mini and embedded Mini before/after compatible state switches, checking no added headland paths or note/legend change. Check Legacy40, old non-V2 Enlarged80, frozen Reference preview, no property/manager, missing/disabled physical surface and V2→non-V2→V2 loads without modifying personal saves. No phantom overlay is acceptable.
4. Check steady walking/forced map rebuilds leave `HeadlandProjectionReads` unchanged after first successful projection; visibility/replacement changes update within cadence and reproject only when necessary. Verify live component destruction/scene replacement and bounded retry behaviour.
5. Run the required movement/interaction, rack IDs/positions/validation, old Dirt Path count/index, construction/resource/harvest-once/regeneration and persistent save/load gates selected by the authorised integrator; do not infer these from the read-only source diff. No Windows/Player build without separate explicit authorisation.

**Return to orchestrator/Manager:** separately assign independent #297 source review of implementation commit `3bc357dcda5203da8cf1b8c28479025d9c096612` plus this durable evidence. The Manager alone records APPROVED / NEEDS CHANGES / REJECTED and releases any next writer. This handoff is not that verdict.

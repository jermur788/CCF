# D-072 / #297 — Independent source review: full Stand Map existing-headland repair

**This is a review opinion. It is NOT Manager approval.** Only the Manager records APPROVED / NEEDS CHANGES / REJECTED and releases the next writer (#296). Nothing here was run in Unity.

## Verdict: PASS WITH NON-BLOCKING NOTES

Blocking findings: **0**. Major findings: **0**. Minor / informational: **5** (below). The offline suites re-ran green on my own machine and I could not make the implementation fail any audit point by reading code. The remaining risk is runtime/render behaviour that offline tests cannot show (see "Could NOT verify").

## Reviewer identity and lane (observed)

| Item | Value |
|---|---|
| Reviewer | Claude Code (Claude Agent SDK harness inside T3 Code), model **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Anthropic |
| Effort | The session carried a `reasoning_effort` setting of **10 (low)**. The packet asked for Medium. My `[RUNNING]` comment said "effort not exposed"; that was imprecise, this line is the accurate record. |
| Independence | Different provider and model from the author (OpenAI `gpt-6.1-sol`, high). I did not write or edit any of the reviewed code. |
| Date | 2026-11-06 |
| Lane | `/media/jer/Files/CCF/Operations/CCF-D072-MapSourceReview-20261106` (ext4), branch `docs/d072-stand-map-source-review-20261106-v1`, HEAD `1b68ddcd583ab2bddd43b5ce18365171c0bab3b4`, clean at start and at end (`git status --short` empty) |
| Session start dir | `/media/jer/Files/CCF-Primary`; nothing was written there. `tools.opencode.session_move` was not available, so I used absolute lane paths. |
| Reviewed SHAs | base `7f5278bfd42997da055da63423b6b4c30c7bb8f3` → implementation `3bc357dcda5203da8cf1b8c28479025d9c096612` → docs `1b68ddcd583ab2bddd43b5ce18365171c0bab3b4`; remote `feature/scenario-one-next-action-guidance-v1` is still at `1b68ddcd` (`git ls-remote`) |
| Authority read | Packet and Manager release (from `origin/main`), four-file spec (`origin/main:specs/scenario-one-stand-map-headland-teaching-alignment-v1/`), the handoff, the full `git diff 7f5278bf..1b68ddcd`, and the real source files named in the task |

## Findings

Severity: **B** blocking, **M** major, **m** minor. No B or M findings.

**m-1 — Per-cadence child enumeration allocates a string per child.**
`Assets/ForestPrototype/UI/Map/StandMapSource.cs:259-260` runs `foreach (Transform child in headlandPresentation.transform)` with `child.name == "Scenario One V2 Headland"` every 0.25 s while V2 is active. That transform is the "Game State" object, which also parents one seedling visual per ecology cell (`ForestEcologyController.cs:317`, `Instantiate(..., transform)`, up to 256 on the 16×16 grid). In Unity `Object.name` allocates a managed string per call, so this is roughly 1,000 small allocations per second in a regenerating stand. It is not a scene-wide discovery and does not break the packet's rule, and I could not measure it. Suggested fix, for the Manager to weigh: cache the resolved child `Transform` and re-scan names only when the cache is null or destroyed. Not blocking.

**m-2 — Residual dark Path edge across the approach pad (render risk, by design).**
The Manager-mandated order puts the new layer before the existing Paths block (`StandMapDrawPlan.cs:115-129` then `:131-146`), so each old Path's dark casing (alpha 185) is painted after, and over, the tan pad. The pad is 4.4 m across and the path 2.6 m, so two thin dark path-edge lines will cross the tan pad bulge at the PathX junction. This is the "existing Paths overpaint junction" behaviour the Manager accepted, not a defect against the spec. Tan fills are also alpha 240 (`StandMapSymbols.cs:77`), so overlaps (strip over pad over path) deepen slightly and the pad ring under the strip may show a faint seam. The draw plan proves order, not pixels. Check at 1280×720 and 1920×1080, colour and grayscale, in the Unity phase.

**m-3 — `Scene.handle` is a new API surface in this repo.**
`StandMapSource.cs:219` uses `eco.gameObject.scene.handle`; nothing else in `Assets` uses it. It exists in current Unity and I see no reason it fails, but only a Unity compile can say so for 6000.6.0f1. Stand-ins in the offline suite cannot detect an obsolete-API warning or removal.

**m-4 — Runner proves attachment for two of the three components, in one scene.**
`run_headland_check.py` checks that `ForestStartingStand` and `ForestEcologyController` share one GameObject in `ForestTest.unity`, but not `ScenarioOneManager`. I checked this myself: all three sit on GameObject `780226341`, and `ForestSceneBuilder.cs:144-146` adds them to one "Game State" object. The gate can therefore be satisfied in the main scene. The other scenes (`MixedSpeciesTest`, `ForestryAssetReview`, `SampleScene`) were not checked; the gate fails closed there, which is the safe direction.

**m-5 — Legend swatch is the same as the "Forest path" row.**
`StandMapLegend.cs:73-74` reuses `MapSymbol.Path` for the new row, so only the words distinguish the headland from the forest path. That matches the map (same tan) and the handoff documents the no-new-symbol choice (NB-3). The legend is only consumed by the full map (`StandMapView.cs:249`; the HUD minimap has no legend), and its refresh key includes `model.StaticVersion` (`StandMapView.cs:234`), so the row appears and disappears correctly. Informational.

Informational, no action: `ShouldShowExistingHeadland`'s `enlarged80` is redundant because `RackPlanningV2Active` already requires Enlarged80 (`ScenarioOneManager.RackPlan.cs:37-40`, NB-5); keeping it matches spec FR-M1. The exhausted-budget fail-closed consequence (six lookups, then hidden until an identity or eligibility reset) is disclosed in the handoff and is low risk because `ForestStartingStand.cs:54-55` attaches the presentation in `Awake`.

## Disposition of audit points 1–9

| # | Point | Disposition | Why |
|---|---|---|---|
| 1 | Scope | **PASS** | `git diff --name-status 7f5278bf..1b68ddcd` shows exactly: `StandMapDrawPlan.cs`, `StandMapLegend.cs`, `StandMapModel.cs`, `StandMapSource.cs` (M); the handoff and four files under `Tools/Verification/StandMap/HeadlandOffline/` (A). `StandMapView.cs`, `HudMinimapView.cs`, symbols, geometry, presentation, scene, prefab, rack, MFM and save files are untouched. No `.meta` in the diff (0), no untracked files. New tool files are outside `Assets/`. |
| 2 | Separate collection | **PASS** | `StandMapModel.cs:168` adds `ExistingHeadlandSurfaces`, a separate list. Neither `ClearExistingHeadland` nor `ReadExistingHeadland` references `model.Paths` (runner regex, and I read the code). `ReadScenery()` is byte-identical to base (runner compares method text against `git show`). Hash: `StandMapModel.cs:254-262` contributes only when the list is non-empty, so empty-state hash is unchanged (baseline/current fingerprint `1e1bd638…` identical on my run). Paths and five-rack JSON are identical before/after in the test. |
| 3 | Geometry | **PASS** | The source reads `ScenarioOneHeadlandGeometry` constants (HeadlandX −35.4, SouthZ −37.7, NorthZ 37.7, FeederZ −27.4, PathX −2.436166, Width 2.6, PadRadius 2.2). Output: headland (−35.4,−37.7)→(−35.4,37.7); feeder (−35.4,−27.4)→(−2.436166,−27.4); two 2.6 m widths; two 2.2 m pads centred on the feeder ends. This matches the 3D builder (`ScenarioOneHeadlandPresentation.cs:41-46`, rectangles plus `PadRadius` discs). The collection holds exactly 2 segments and 2 pads. There is no third segment, no northern spine, and no stub (the approach stays in `Paths`). |
| 4 | Draw order | **PASS** (order proven; pixels not) | `StandMapDrawPlan.cs:115-129` sits after the corridor-band loops and before the Paths block. Eight items: 2 segment casings, 2 pad rings (`Ring`, non-filled), 2 tan strokes, 2 pad fills (`Disc` with zero stroke; `StandMapCanvas.cs:57-60` skips a zero-width stroke). No outlined+filled disc follows a fill. Rack centre lines (`:148-153`, comment at `:148`) and the dynamic-layer entrance and handle items still follow. The old items are byte-identical after the layer is removed (`FULL_STATIC_COUNT BEFORE=29 AFTER=37`). See m-2 for render risk. |
| 5 | Full-only / Mini / legend | **PASS** | Layer guarded by `if (full)` (`StandMapDrawPlan.cs:115`). Mini output is byte-identical at sides 128/196/520/880 × 5 overlays (20 checks). Both Mini consumers use `MapDetail.Mini` (`HudMinimapView.cs:51`, `StandMapView.cs:179`). Legend row is gated on `ExistingHeadlandSurfaces.Count > 0` (`StandMapLegend.cs:73`), and the only legend consumer is the full map. No divergence found; the embedded Mini overview sits beside the full-map legend, which names a feature that overview does not draw. That is the specified design (full-map legend). |
| 6 | Active-surface gate | **PASS** (source and stand-ins; live linkage not run) | `:235-237` requires `RackPlanningV2Active && !ReferencePreviewActive` and `StandGeometryModelVersion == Enlarged80`. `:248-255` takes the attached presentation only if exactly one exists, the starting stand is attached, and the manager's `GetComponent<ForestEcologyController>() == eco` (`:246`). `:256-268` needs exactly one named child that is self- and hierarchy-active with an enabled `MeshRenderer`. Lookup is six tries with backoff 0/0.5/1.5/3.5/7.5/15.5 s (`:61`, `:248-251`). Cadence is `RackRefreshSeconds = 0.25` (`:41`, `:232-233`). No `Find*` or scene scan; the only repeated cost is m-1. Fails closed for Legacy40, non-V2, Reference, no manager, wrong manager/ecology, ambiguous or missing presentation, missing or duplicate surface, disabled renderer. The named-child proof is the spec's chosen mechanism (FR-M1). |
| 7 | Test quality | **PASS WITH NOTES** | See below. |
| 8 | Honesty | **PASS** | I found no material overstatement. Reproduced: 30 source guards + 92 C# checks (122 PASS lines, 0 FAIL), 176/0 shared suite, fingerprint hash, 29→37 count, `STEADY_REFRESH FRAMES=600 PROJECTIONS=1 ATTACHED_LOOKUPS=1`. The handoff states plainly that the wrapper drives extracted methods and not the real `Refresh`, that stand-ins replace Unity, and lists NOT RUN items. It does not discuss m-2 (path-edge lines over the pad) but does not claim pixel results either. |
| 9 | Protected regressions | **PASS** at source level | `ReadScenery`, `ReadRacks`, `ReadTrees`, `ReadSmallLayers`, `Rebuild` text-identical to base; no Dirt Path, Legacy40, Reference, save-schema, scene or 3D-world file changed; no existing verification file modified or weakened (`git diff` shows only added test files). Runtime behaviour of each is **not verified** (needs Unity). |

### Point 7 detail — are the tests meaningful?

What is real:
- The pure map files (`StandMapModel`, `StandMapDrawPlan`, `StandMapLegend`, symbols, transform, canopy raster) are compiled **unmodified** from the working tree, and the baseline fingerprint compiles the same files **from `git show 7f5278bf`**. Empty-state output must match byte for byte across 40 plans, the legend and the hash. That is a strong regression guard.
- Constants are **parsed from `ScenarioOneHeadlandGeometry.cs`** by regex (seven unique matches) and compiled in; the test does not hardcode a second set of literals, as FR-M4 required. Expected values are composed in the test from those constants, so a swapped X/Z or wrong pairing in the projection would fail.
- The gate and cadence tests run the **extracted production method text** (`ClearExistingHeadland`, `ReadExistingHeadland`, the field block, the no-property statements), not a rewrite.

My own mutation check (copies in my scratchpad; nothing in the repo; I recompiled the check project against a mutated `StandMapDrawPlan.cs`):
- M1: change `if (full)` to `if (true)` → **20 FAIL** (all Mini byte-identical cases), tally `PASS=72 FAIL=20`.
- M2: give the pad fill a dark outline (one dark ring after the tan) → **1 FAIL** ("no late dark disc outline"), tally `PASS=91 FAIL=1`.
So the Mini-leak and draw-order checks are not tautological.

Limits (none blocking):
- `HeadlandSourceStubs.cs` is the author's own model of Unity (`==` null semantics, `activeInHierarchy`, `GetComponents`). It matches my understanding of Unity, but it can't detect a real divergence. I confirmed the real names used by the production code exist: `ReferencePreviewActive`, `RackPlanningV2Active`, `StandGeometryModelVersion`, `StandGeometryModel.Enlarged80`, `ScenarioOneUiRoot.Manager`, `ForestStartingStand`, `ScenarioOneHeadlandPresentation`.
- The truth-table test of the three-bool predicate is near-trivial (a pure `&&`); the meaningful gate coverage is the 13 `Negative` cases plus the transition and cadence cases that go through the extracted method.
- The "Paths and five racks unchanged" test checks a model into which the test itself places synthetic Paths. Its real protection is the regex that bars `model.Paths` in the new methods, plus the byte-identical `ReadScenery`.
- Some source guards are string matches (for example the presentation contract is a whitespace-stripped substring check), so they are sensitive to reformatting and give false failures rather than false passes.
- The wrapper `Read()` replaces the real `Refresh`; the call site is only asserted by a count of `ReadExistingHeadland(eco);` in the real `Refresh` text.

## Independent re-run evidence

Commands run from the lane with `python3 -I -B` (the `-B` avoids `__pycache__` dirtying the worktree), outputs redirected outside the repo to `/media/jer/Files/CCF/Verification/D072-StandMapHeadland-IndependentReview-20261106/` so the author's evidence directory was not overwritten. .NET SDK `10.0.114`.

```text
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py --out <review-dir>/headland-dotnet
  ... 30 x "PASS SOURCE ..." ...
  AUTHORITY_CONSTANTS {'HeadlandX': '-35.4', 'SouthZ': '-37.7', 'NorthZ': '37.7', 'FeederZ': '-27.4', 'PathX': '-2.436166', 'Width': '2.6', 'PadRadius': '2.2'}
  BASELINE_CURRENT_FINGERPRINT_SHA256 1e1bd6388f287525d17793c553a6bc2adc88f9e279b50d67c98ba5cb6fdaa6d1
  FULL_STATIC_COUNT BEFORE=29 AFTER=37 HEADLAND=8; two casing strokes, two pad outlines, two tan strokes, two pad fills
  STEADY_REFRESH FRAMES=600 PROJECTIONS=1 ATTACHED_LOOKUPS=1
  HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0; Unity/runtime/render NOT RUN
  HEADLAND_SOURCE_AND_OFFLINE_PASS; Unity compile/live linkage/render/input/build NOT RUN
  exit=0
```
Counted from the log: 30 `PASS SOURCE` lines, 122 `PASS` lines in total, 0 `FAIL`. **Real tally: 30 source guards + 92 C# checks = 122 PASS, 0 FAIL**, matching the handoff.

```text
python3 -I -B Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py --out <review-dir>/shared-map-dotnet
  176 passed, 0 failed
  STANDMAP_OFFLINE_PASS
  exit=0
```

Mutation runs: M1 `PASS=72 FAIL=20`, M2 `PASS=91 FAIL=1` (see above). After all runs, `git status --short` in the lane was empty. Logs: `rerun.log`, `shared-rerun.log` in the review evidence directory.

## What I could NOT verify

- **Unity compile and linkage**: `StandMapSource.cs` was never compiled with Unity (it is not Unity-free, so only extracted methods were compiled in .NET against stand-ins). That includes the `Scene.handle` use (m-3).
- **Live active-surface behaviour**: the real `ScenarioOneHeadlandPresentation` lifecycle, `Reconcile`/`LateUpdate` timing against the 0.25 s read, scene reload, destroy ordering, and the real `Refresh` loop.
- **Performance**: m-1's allocation cost and any frame-time effect, in the Editor or a Player.
- **Native input**: map drag, Confirm, keyboard, pointer and prompts.
- **Rendering at 1280×720 and 1920×1080**, colour and grayscale: pad union, round caps, dark path-edge over the pad (m-2), alpha overlap seams, legend row appearance, and the HUD and embedded Mini on screen.
- **Save/load** and persistent-world behaviour, including V2→non-V2→V2 reloads.
- **Legacy40 and frozen Reference Future v1 gameplay**: only source-seam negative cases were seen, not a real archive or save.
- **`StandApproachVerification` and `StandMapParity` Unity gates**: I read that nothing in them was edited, and I found no hard-coded count in `StandMapParity.cs` that adding eight full-only items would break, but I did not run them.
- **Historic feedback checker and `check_sources.py` for the expanded diff**: not run (the handoff discloses this).
- **Windows/Player build**: not applicable and not authorised.

## Recommendation to the Manager

Accept for source purposes with the five notes carried forward: m-1 to decide whether to cache the child transform now or accept 4 Hz enumeration; m-2 and m-3 to be checked in the first authorised Unity pass; m-4 and m-5 for information. The Unity-phase checklist in the handoff (lines 430-439) covers the render and lifecycle checks above; I would add "confirm dark Path edge across the approach pad is acceptable" and "profile GC from `ReadExistingHeadland`". No block on #296 sequencing arises from this review, but that gate belongs to the Manager.

**Not Manager approval.** This review does not record APPROVED / NEEDS CHANGES / REJECTED, does not integrate anything, and makes no Unity, save, Legacy40, Reference or Player claim.

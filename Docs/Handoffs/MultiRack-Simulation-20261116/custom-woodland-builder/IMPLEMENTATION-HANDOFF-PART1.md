# Full Custom Woodland Builder V1 — Part 1 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: MULTI-2D-CUSTOM-WOODLAND-BUILDER-P1-20261128
Worker: OpenCode / DeepSeek V4.1 Flash (opencode-go), single lane writer
Role: Sole implementation writer (Option A, owner-approved 2026-11-28)
Branch: feat/multi-2d-custom-woodland-builder-engine-v1
Base: f4dee8023d3492a25e61451c7e2a49515e507ed3 (origin/integration/2d-sim, live tip at cut time)
Context commit: bdeb6a324acaf4129c10fb0846faac8b64974360 (origin/main)
HEAD: the latest commit touching this file. The engine and tests under test are commits 2 and 3 below it.
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/custom-woodland-builder/IMPLEMENTATION-HANDOFF-PART1.md
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/Multi2DCustomWoodlandBuilder-20261128/
  (version.txt, restore.txt, build.txt, run1.txt, run2.txt, negative-build.txt, negative-run.txt, fence.txt)
```

**State:** isolated branch implemented and green; **NOT reviewed**; **NOT merged**; **NOT wired**. Parked for independent review, then for the single integrator to combine one branch at a time. Part 2 is queued behind the Milestones 2–5 Part 2 wiring wave.

## What changed

- A four-file spec quartet, `SPEC-multi-2d-custom-woodland-builder-v1`, status APPROVED / IMPLEMENTING (Part 1). It covers Part 1 (implemented here) and Part 2, which is **QUEUED** behind the Milestones 2–5 Part 2 wiring wave.
- A pure .NET 8 module in namespace `SitkaP1.Core.CustomWoodlandBuilder`. BCL only: no I/O, no clock, no `System.Random`, no RNG-salt file edit.
  - **`PlanPoint` / `WoodlandPolygon`** — flat-local-metre geometry with fail-closed validation: 3–16 vertices, all finite, no repeated closing vertex, no zero-length edge, no spike, no touching or crossing between non-adjacent edges, a 30–120 m bounding footprint on both axes, at least 450 m² of area, counter-clockwise normalisation and a canonical start vertex (smallest X, then smallest Z). Containment is boundary-inclusive; edge distance and the area centroid are provided for the grid and the generator.
  - **`WoodlandShapeSpec` / `WoodlandBoundaryTemplates`** — the four approved Irish field-shape templates (`Rectangle`, `LShape`, `TaperedWedge`, `AngledStreamEdge`) built as base rings, then a chamfer pass applies the four `CornerAdjustments`. A chamfer must be finite, non-negative and strictly shorter than half of the shorter adjacent edge, so adjacent chamfers can never meet. The L-shape's four adjustments belong to its two southern corners and its two northern column ends; its two notch corners are not adjustable.
  - **`LaneClipper`** — 2–8 equal horizontal bands intersected with the polygon using Sutherland–Hodgman half-plane clipping, so every lane vertex lies inside or on the boundary by construction. Index 0 is the south. A lane needs at least three distinct vertices, a mean width of at least **8.0 m** and an area of at least **60 m²**; otherwise the whole plan is refused with the failing lane and a named reason. Lanes are never dropped, merged or widened.
  - **`MoistureGrid` / `MoisturePatternSpec`** — a 2.5 m grid aligned to the global lattice (`origin = floor(min / 2.5) * 2.5`), active only where a cell centre is inside the polygon. Four deterministic patterns: `Uniform`, `RidgeAndSlope` (25 % seasonally wet toe / 50 % fresh slope / 25 % dry ridge), `StreamCorridor` (8 m default corridor, 6 m banks) and `WetHollowBogPocket` (10 m default hollow, 5 m ring). Per-cell `MoisturePaintOverride` entries apply in declaration order after the base pattern; a later entry wins. An inactive cell, an out-of-range index or a `NeutralLegacy` paint is refused. The habitat caution is recomputed after painting.
  - **`DeerBrowsePresets`** — exactly four presets: `None = 0.0`, `Low = 0.1`, `Standard = 0.2` (the existing baseline), `Heavy = 0.4`, each with `[E]` `Calibration` provenance and an explicit note that these are not Irish deer/km² thresholds and no deer are simulated.
  - **`CustomWoodlandStandGenerator`** — six-species mix with exact largest-remainder allocation, a jittered lattice restricted to active cells, lane assignment per tree, the resolved moisture class and caution per tree, optional mature edge seed trees on a 4 m inward ring with 0.5 m minimum clearance, and the fail-closed **3,000** living-tree cap with a **2,500** near-cap warning. A SplitMix64 stream seeded only by the recipe seed drives jitter and the species shuffle.
  - **`CustomWoodlandBuilder`** — one `Build(recipe)` entry point that validates every constraint and returns an immutable `CustomWoodlandBuild` with the boundary, lanes, grid, deer setting, stand and a canonical text form.
- Test project `tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj`, an Exe. It compiles only `../../../src/Core/SpeciesWave2/**/*.cs` and `../../../src/Core/CustomWoodlandBuilder/**/*.cs` plus its own test files. It runs **42 checks**, prints per-suite and total counts and the canonical SHA-256, and exits non-zero on any failure.

## Files changed (vs base f4dee802)

- `specs/multi-2d-custom-woodland-builder-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
- `Tools/MultiRackSitkaP1Lab/src/Core/CustomWoodlandBuilder/{PlanGeometry,WoodlandBoundaryTemplates,LaneClipper,MoisturePatternGrid,DeerBrowsePresets,CustomWoodlandStandGenerator,CustomWoodlandRecipe}.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core/CustomWoodlandBuilder/{SitkaP1.CustomWoodlandBuilder.Tests.csproj,Program,TestRunner,Fixtures,BoundaryTests,LaneTests,MoistureTests,DeerTests,MixAndStandTests,DeterminismTests}.cs`
- `Docs/Handoffs/MultiRack-Simulation-20261116/custom-woodland-builder/IMPLEMENTATION-HANDOFF-PART1.md`

Everything is inside the four authorised prefixes. **No existing file was modified.** Commits are fence-separated: spec quartet, engine module, tests, handoff.

- **Serialized Unity files touched:** none. **Assets added or modified:** none. **Package/ProjectSettings changes:** none. **New packages:** none.

## Player-facing result

None yet. Part 1 is a callable library the game does not call. Part 2 will make it visible: draw the field shape, adjust its corners, paint the wet and dry cells, choose a plain-English deer setting, set the mix and stocking, place edge seed trees, read the habitat caution and the living-tree count against the cap, and create the woodland.

## Simulation / save impact

None. No change to `AnnualPipeline`, `MultiAreaContinuation`, `ForestMaker`, `RngSalts`, `src/Ui`, `src/Core/Saves`, Version 2 or Version 3 saves, or the existing Wave 2 files. No save fields and no `genesisRecipe` change. `NeutralLegacy` stays bit-exact (1.0, 1.0) for all six species and is never generated or painted by this module.

## Anchors

- **Must-not-change:** Scenario One, `Legacy40`, `Enlarged80`, Reference Future v1, existing P1 save and determinism anchors, `src/Core/SpeciesWave2/*.cs`, `RngSalts.cs`, the parked Scots Pine / Woodland Inspection / Jay-Windstorm / Birch-Alder lanes. All untouched because no existing source changed.
- **Intentionally changed:** none.
- **New pin:** the Part 1 canonical digest `86db0555aaf41405995b4213ecb3cbcadd9c0bab8fa36ac58bc37412dce8cbf5`, under SDK 8.0.133.

## Verification run (from `Tools/MultiRackSitkaP1Lab`, `dotnet --version` = 8.0.133)

```text
$ dotnet restore tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj
  Restored .../SitkaP1.CustomWoodlandBuilder.Tests.csproj (in 1.34 sec).                    exit 0
$ dotnet build tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)                                                                               exit 0
$ dotnet run --project tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj --no-build --no-restore
SUITE Boundary: 10/10 passed, 0 failed
SUITE Deer: 3/3 passed, 0 failed
SUITE Determinism: 2/2 passed, 0 failed
SUITE Lanes: 6/6 passed, 0 failed
SUITE Mix: 4/4 passed, 0 failed
SUITE Moisture: 9/9 passed, 0 failed
SUITE Stand: 8/8 passed, 0 failed
TOTAL: 42/42 passed, 0 failed
DIGEST canonical-sha256 86db0555aaf41405995b4213ecb3cbcadd9c0bab8fa36ac58bc37412dce8cbf5   exit 0
```

`TreatWarningsAsErrors` is on through `Tools/MultiRackSitkaP1Lab/Directory.Build.props`; the pinned `global.json` selects SDK 8.0.133 even though the machine default is newer.

**Determinism.** A second separate process run gave byte-identical full output (`cmp run1.txt run2.txt`, identical SHA-256 of both bodies) and the same digest. DET-01 pins the digest and DET-02 proves the canonical text is stable inside one process, has one line per tree, records the pattern, the preset and the caution flag, contains no `NaN`, and changes when a paint override is removed. Doubles and floats enter the canonical text as raw IEEE-754 bit patterns.

**Negative control.** Changing the pinned digest's last hexadecimal digit produced `TOTAL: 41/42 passed, 1 failed` and exit 1, which confirms the pin is live rather than decorative. It was restored immediately; the green run above is the restored state.

**Not run, by brief:** Unity, browser suites, `dotnet test` and `2d-gate.sh`. Part 1 is not Unity-relevant, so no `CLOUD HANDOFF — NOT UNITY-VERIFIED` item is claimed beyond that statement.

## SPEC-DRIVEN

- **Spec:** `SPEC-multi-2d-custom-woodland-builder-v1`, status APPROVED / IMPLEMENTING (Part 1).
- **Tasks completed:** T-01 to T-08. T-09 (Part 2 wiring) is QUEUED behind the Milestones 2–5 Part 2 wiring wave.
- **Deviations:** none from the locked Option A decisions. One implementation clarification is recorded in the spec: trees are placed only in active 2.5 m cells, so every tree has a defined moisture class instead of an invented `NeutralLegacy` value. The engine also declares that it creates nothing outside the boundary; the surrounding grassland is a Part 2 terrain/render concern.
- **Unresolved requirements:** AC-9 (Part 2) only.

Acceptance criteria tested:

| AC | Cases | Result |
| --- | --- | --- |
| AC-1 four templates, areas, CCW normalisation, canonical start, chamfer bounds, footprint/area/vertex refusals, self-intersection | BOUND-01–10 | PASS |
| AC-2 2–8 lanes, south-first indices, exact clipping, area tiling, inside-boundary vertices, narrow and small-area refusal with a named lane | LANE-01–06 | PASS |
| AC-3 2.5 m lattice grid, active-cell rule, four deterministic patterns, no active legacy cell | MOIST-01–07, MOIST-09 | PASS |
| AC-4 overrides after the base, later wins, invalid targets refused, caution recomputed, table identity and caution unchanged | MOIST-07/08 | PASS |
| AC-5 four presets at 0.0/0.1/0.2/0.4 with `[E]` calibration provenance and the not-a-density note | DEER-01–03 | PASS |
| AC-6 six-species mix, exact allocation, sum/duplicate/unknown/negative refusals, age and stocking ends | MIX-01–04 | PASS |
| AC-7 exact planted count, trees inside boundary and lane, edge seed trees, cap refusal, near-cap warning, seeded determinism, bog flags | GEN-01–06, CAP-01/02, DET-02 | PASS |
| AC-8 pinned restore/build/run, 0 warnings, 0 errors, cross-process digest, file fence | DET-01 + receipts above | PASS |
| AC-9 integrated Part 2 | — | NOT TESTED (queued) |

**Spec conformance (Part 1):** PASS by worker self-check only. No independent review has been performed.

## Remaining verification gates

- A fresh-thread independent review of the final feature commit: Part 1 conformance, calibration wording, determinism, and the four-prefix file fence.
- `review/multi-2d-custom-woodland-builder-engine-v1` is created by the coordinator after review; this worker does not create or merge it.
- All Part 2 gates: New Woodland picker, `ForestMaker` generation, `AnnualPipeline` moisture consumption, Version 3 `genesisRecipe` round-trip, Version 2 and older save behaviour with no fabricated recipe, `/multi` caution and cap display.

## Known limitations and residual risks

- **Shapes are stylised, not surveyed.** The 30–120 m footprint and 450 m² minimum will refuse genuinely small fields. Part 2 must word the picker as "close enough to your field", not "your field".
- **Mean lane width is an area average.** A dog-legged lane can pass the 8.0 m mean while pinching locally. The clipper refuses narrow-average and degenerate lanes, not every local pinch.
- **Patterns are teaching shapes, not hydrology.** The catena proportions, bank margin and hollow ring are fixed calibration values; a real wet patch is painted, not modelled.
- **Deer presets are one aggregate number.** `[E]` calibration on `backgroundBrowsePressure`; no damage model, no home ranges, no fencing.
- **The cap is a simulation guardrail.** A legal 120 × 120 m field at 3,000 stems/ha asks for 4,320 trees and is refused. That refusal is deliberate and must stay visible in the picker rather than being silently clamped.
- **Lattice placement is not a planting prescription.** Jittered rows imply no machine reach, drainage or design intent.
- **Edge seed trees depend on shape.** A very concave polygon has a short usable inset ring and the generator refuses rather than crowding. Part 2 should surface the capacity plainly.
- **Active-cell restriction.** Because trees must sit in active cells, the outermost few metres of a field can stay empty. That is a deliberate consequence of keeping every tree's moisture class honest.
- **`WoodlandShapeSpec` uses one record with per-template fields.** Unused fields stay zero and are never read; the alternatives were four nominal types for one small validated input.

## Unrelated work preserved

The worktree was cut clean from `f4dee802` and contains no other change. No other worktree, branch, review branch, lane, Unity library, save or merge was touched. `/media/jer/Files/CCF-Primary` retains its pre-existing modified and untracked operations files, unabsorbed.

## Decisions needed

None for Part 1. For Part 2 the owner or Manager should confirm (a) whether the picker blocks or only cautions a `< 2,500`-to-`3,000` stand, (b) whether painted cells are persisted as absolute cell indices or re-derived from the lattice origin plus an override list, and (c) whether edge seed trees count toward the displayed stocking figure (Part 1 excludes them from stems/ha and includes them in the living-tree cap).

## Roadmap synchronization — not performed in this lane

The 2D Delivery Manager's packet asks for the complete `2D_Simulator_Strategic_Roadmap.md` **v1.23** to be bundled into an upcoming single-integrator combine onto `origin/integration/2d-sim`. This lane did **not** do that, for two verifiable reasons:

1. The target file is outside this lane's four authorised prefixes, and Guardrail 11 explicitly forbids assigning the roadmap to more than one parallel branch at once.
2. No v1.23 text exists on this machine or on the base branch. `origin/integration/2d-sim` at `f4dee802` contains **v1.22** (Change Log row 2026-11-28, v1.22) and the file ends cleanly at that row. Fabricating a v1.23 Change Log row would be an invented strategy record, so nothing was written.

This branch therefore contains zero roadmap edits. The v1.23 sync still needs its own single-writer combine with the notebook text supplied.

## Worker assessment

Ready for review: Yes. Scope is Part 1, offline .NET only; nothing is merged or wired.

## Recommended next step

Dispatch a fresh-thread independent review of the final feature commit, publish its verdict on `review/multi-2d-custom-woodland-builder-engine-v1`, and park the branch until the single integrator combines it one at a time after the Milestones 2–5 Part 2 wiring wave.

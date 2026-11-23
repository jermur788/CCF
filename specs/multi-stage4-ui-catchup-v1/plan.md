# Multi Stage 1–4 `/multi` UI Catch-up Implementation Plan

- **Spec ID:** SPEC-multi-stage4-ui-catchup-V1
- **Spec status:** APPROVED
- **Plan owner:** Implementation sub-agent (sole writer, lane `CCF-Multi-Stage4UiCatchup-20261123`)
- **Created:** 2026-11-23
- **Last updated:** 2026-11-23

## Architecture / current-state findings (verified at `d3a9f20f`)

- `/multi` is served from the `MultiAreaPage` string constants in `src/Ui/PlayerMultiApi.cs` (HTML, CSS, JS; crown sprites inline WebP). Routes are registered in `AddMultiAreaRoutes()` behind the unchanged `PlayerApi` guards.
- `src/Ui/Program.cs` (read-only) calls `MultiAreaWorkPlanAdapter.OpenSynthetic()`, which currently constructs the model 0 continuation.
- `MultiAreaContinuation` already supports `MultiAreaModelSelection.Stage4`, `AddPrune`/`RemovePrune`, Prune in Approve/Execute/Advance, `ReadStage4View()` and `ReadCanopyLight()` (cached per AnnualRevision/WorkRevision).
- `CanopyLightCell` has centre, clipped area, light index and cross-border count, but no clipped polygon, per-cell cover fraction or contributing tree IDs.
- `PruningRules.RefusalCode` needs a target height; the engine has no lift schedule. Unity's verified Sitka schedule is 2.5 / 5 / 6.5 m.
- Core test `MA-W12` pins the exact adapter member names; it will be extended (not weakened) with the new members.
- Execute-time Prune revalidation cannot be reached through public commands: any tree, pruning or year change bumps the source token, so `WorkPlanStaleSource` fires first. The test must inject guard drift directly into the private root by reflection (the suite already reads `root` by reflection in `MultiAreaTests.Root`).
- `tests/ui-smoke-multiarea.sh` J01/J99 fail on the base: the DOM stand-in lacks `cloneNode`, used by the #885 `drawGrid`, so the first render throws before the table is filled.

## Proposed technical approach

1. **Core read-out** (`src/Core/MultiArea/MultiAreaUiReadout.cs`, new): internal records for models, per-tree Stage 4 + prune eligibility, per-rack canopy summary + deadwood, per-rack ground cells; builder fed by `ReadStage4View` data, the cached `CanopyLightReport` and new internal per-cell helpers. Cached per committed tree state with the canopy cache.
2. **Evaluator helpers** (`CanopyLightEvaluator`, internal, behaviour-neutral): clipped cell polygon (same Sutherland–Hodgman clip), per-cell covered area on the same 0.25 m micro-grid rule as Layer B, contributing trees (influence > 0, same float kernel). `Evaluate` output unchanged.
3. **Pruning schedule** (`PruningRules`): `SitkaLiftTargetHeightsM`, `NextLiftTargetM(lifts)`, scheduled refusal code. Continuation `AddScheduledPrune` passes the engine target into the existing `AddPrune`.
4. **Continuation**: `ReadUiSnapshot()` (view + read-out under one lock) and `ReadUiReadout(source, planStateRevision)` (null when the root moved).
5. **Adapter**: `OpenSynthetic(MultiAreaModelSelection? models = null)` defaulting to Stage 4; `ReadSnapshot`, `ReadoutFor`, `AddPrune`, `RemovePrune`.
6. **Ui API**: additive `stage4` member on view/command/competition/hit payloads; prune add/remove routes using `MultiTreeMembers`.
7. **Page**: rack readouts, model badge, prune inspector and plan lists, execute confirmation including prunes, Light Guide toggle + ramp legend + ground inspector, fallen-log markers + legend.
8. **Tests**: atomic-refusal test, snapshot/readout tests (contributors reproduce the light index bit for bit, cover fraction vs an independent sampler, 1/1/1 default and 0/0/0 path), adapter prune route tests; smoke harness: add `cloneNode` to the stand-in only.

## Integration boundaries

- May touch: `src/Core/**`, `src/Ui/PlayerMultiApi.cs`, `tests/**`, `specs/multi-stage4-ui-catchup-v1/**`, `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/**`.
- Must not touch: `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Ui/wwwroot/**`, `Assets/**`, fixtures, `.sln`, `global.json`, `Directory.Build.props`, build scripts, `.p0ref`, `Tools/MultiRackSitkaLab/`, `active-tasks.md`.
- No serialized Unity file, save format or package change.

## Affected systems / files

- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaUiReadout.cs` (new)
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs`, `MultiAreaWorkPlanAdapter.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/CanopyLightEvaluator.cs`, `Stage4Records.cs`
- `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/*` (new test file + registration + MA-W12 member list)
- `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` (stand-in `cloneNode` only)

## Ownership / concurrency considerations

One writer (D-091). No serialized or single-writer Unity files. Protected live listeners `:33767`, `:43649`, `:46593` are never touched; the verification server binds an OS-assigned loopback port and is stopped by its own PID.

## Migration / save implications

None (session-only).

## Verification strategy

- **Implementation correctness:** solution and Ui builds 0/0; Core suite ≥ 181/0 with the five anchors and the P0 sha256 before/after; UI smoke; new Core tests with independent oracles.
- **Spec conformance:** requirements checklist; real Edge over CDP at exact viewports with console/CSP/network capture and DOM measurements; eight screenshots.
- **Verification tier:** Tier 1 while building, Tier 2 at handoff (AgentWorkflow).

## Risks

- Payload size grows (ground cells per rack in every response); mitigated by caching and acceptable on loopback; measured and reported.
- Layout pressure at 1280×720 from extra card lines; verified by measurement.
- The DOM stand-in (J section) may still disagree with later #885 controls after the `cloneNode` fix; reported honestly, not hidden.

## Implementation sequence

1. Spec quartet commit (T-01).
2. Core pruning schedule, evaluator helpers, read-out, continuation/adapter, atomic-refusal and read-out tests (T-02, T-03).
3. Ui API payload + prune routes (T-04).
4. Page (T-05).
5. Harness alignment, Tier 2 gates, real-browser verification, screenshots, handoff (T-06, T-07).

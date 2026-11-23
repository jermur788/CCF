# Multi Stage 1–4 `/multi` UI Catch-up Specification

- **Spec ID:** SPEC-multi-stage4-ui-catchup-V1
- **Status:** APPROVED
- **Owner:** Project owner (product decisions); orchestrator custody under D-091
- **Created:** 2026-11-23
- **Last updated:** 2026-11-23
- **Decision log references:** D-095 (Stage 4 adult parity, natural mortality, deadwood, fenced Prune), D-091 (lane custody / one writer), D-020 (presentation authority)
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`)
- **Related evidence:** `Docs/Handoffs/MultiRack-Simulation-20261116/899-900/Multi-Stage4-AdultParity-IMPLEMENTATION-RESULT-20261123.md`; `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-D091-LANE-ADMISSION-20261123.md`
- **Supersedes / superseded by:** none

## Authority statement

This spec is authored from the owner packet **"CCF 2D / MULTI SIMULATOR — STAGES 1–4 UI CATCH-UP HANDOFF", 2026-11-23**, whose four product decisions are locked (restated verbatim in substance below), together with D-095 and D-091. It adds **no product scope** beyond those decisions. Any deviation from them, or any material ambiguity they do not settle, **returns to the owner** instead of being invented by the implementer.

## Product intent

The Stage 1–4 engine (connected-woodland crown response, canopy union and decoupled 2.5 m light, adult parity, per-rack density mortality with fallen deadwood, fenced Prune) already exists in Core but the standalone 2D `/multi` page still runs the frozen model 0 session and shows none of it. The player should see, on the same #885 page, what the engine now says about canopy, open ground, density, natural deaths and light, and be able to plan Prune work through the same commitment gate as Fell.

## Problem

- `/multi` sessions start on Model 0/0/0, so the page shows legacy crown radii and no Stage 4 behaviour.
- The Stage 4 read-out (`ReadStage4View`) and the canopy/light read-out (`ReadCanopyLight`) are not exposed to the page.
- There is no Prune route, so the fenced Prune work plan is reachable only from tests.
- Execute-time Prune guard revalidation with whole-plan atomic refusal is implemented but UNTESTED (Stage 4 handoff §9 item 12).

## Locked owner decisions (2026-11-23)

1. `/multi` sessions default to CrownModelVersion 1, AdultModelVersion 1, MortalityModelVersion 1, canopy union and 2.5 m decoupled light (b = 2.5 m). Model 0/0/0 stays frozen and unchanged in Core for every existing fingerprint and regression test.
2. Natural crowding deaths appear as fallen-log markers on the map (distinct non-colour-only shape plus legend entry) and are counted (this year / cumulative) in the rack summary. Standing biologically dead trees stay visually distinct from felled stumps.
3. No split-screen untreated twin view. Clean 1280×720 and 1920×1080 layouts with zero sideways scrolling.
4. D-020 presentation authority: the UI does zero forestry, canopy, light, RD or pruning-eligibility arithmetic. Every radius, cover %, light index, RD, deadwood record and refusal reason comes from an engine snapshot/API response. JS may only format/round for display. The 2× displaced competitor overlay stays purely visual: it must not alter true coordinates, hit-testing, canopy cover or light values.

## Goals

- `/multi` runs 1/1/1 by default; 0/0/0 remains reachable from Core and tested.
- Additive, engine-authoritative Stage 4, canopy and light read-outs in every `/multi` view and command payload.
- Fenced Prune work-plan routes that follow the Fell pattern exactly.
- #885 page extended with rack readouts, a prune inspector, an optional Light Guide with ground inspector and fallen-deadwood markers.
- A Core test proving execute-time Prune revalidation with whole-plan atomic refusal.

## Non-goals

- No change to any frozen fingerprint, the P0 pin, the single-area page, `PlayerApi.cs` guards or page policy.
- No untreated twin / split view; no Stage 5 content (seedlings, brambles, bracken, browsing), Oak/Beech, economy, storms, decay, habitat score or timber value.
- No save-format change, no persistent 2D save, no Unity or `Assets/**` change, no new package or npm dependency.
- No new images: the #885 owner-supplied crown sprites and their provenance stay unchanged.

## User / player workflows

1. **Load** `/multi`: Area A shown at Fit, C2 crown radii, rack summary cards with Canopy Cover % + Open Ground m², Stand Density (RD), Natural Deaths / Fallen Deadwood (this year / total logs) and a compact model badge (Model 1/1/1, synthetic stand, light not calibrated PAR).
2. **Fell** (unchanged #885 flow): tick competitors, add, Approve (zero Fell), Execute (confirmation), receipt. Cover drops, open ground rises, light lifts — all as engine values.
3. **Prune**: select a living tree; inspector shows Pruning Lifts (X / 3) and engine eligibility or refusal reason; "Add to Prune plan" (disabled with the engine reason when ineligible) creates a Pending Prune (no crown or light change); Approve (no ecology change); Execute (confirmation; one lift r = max(0.1, r × 0.92) immediately; canopy/light refresh; year unchanged); the 5-year guard then reports ineligible.
4. **Light Guide**: Off by default. On draws the polygon-clipped 2.5 m cells with a colour ramp plus a band number per cell and a numeric band legend, under the exact label "Modelled Light Guide (Unity k10a10 proxy — not calibrated PAR)". A click on a cell inspects Modelled Light Index (0–1), overhead cover fraction and contributing tree IDs without clearing unsent ticked Fell choices.
5. **Year+1**: gradual gap re-closure from engine crown growth; natural deaths add fallen-log markers and increment the rack counters.

## Functional requirements

- **FR-1** `MultiAreaWorkPlanAdapter.OpenSynthetic()` (the only constructor the read-only `Program.cs` calls) MUST start a 1/1/1 Stage 4 session. An explicit selection (including `MultiAreaModelSelection.Legacy`) MUST remain available from Core. `new MultiAreaContinuation(experiment)` MUST stay model 0.
- **FR-2** Every `/multi` view and command payload MUST carry an additive `stage4` member (existing members unchanged) built by Core from the same committed root as the payload's view (or `null` with an explicit note when a concurrent change made the roots differ), containing:
  - model metadata: `crownModelVersion`, `adultModelVersion`, `mortalityModelVersion` (integers), `lightModelVersion = "unity-k10a10-decoupled-v1"`, the light label, `calibratedPar = false`, the not-calibrated-PAR caveat, the synthetic experimental-stand disclaimer, Stage 4 evidence labels, the 2.5 m cell size;
  - per tree: `lifeState` (Living | FelledStump | BiologicallyDead), `deathCause`, `deathYear`, `pruningLifts`, `maxPruningLifts`, `pruneEligible`, a stable refusal code and human-readable reason (non-living, max 3 lifts, 5-year interval, target below 60 % of height), the engine's next lift target and next eligible year, and Pending/Approved Prune plan state;
  - per rack: `geometricCoverFraction`, `geometricUncoveredM2`, largest open-gap summary (opening count, largest opening area and width), relative density RD, annual and cumulative natural deaths, `FallenDeadwoodRecord[]` with engine and map coordinates;
  - per rack: the 2.5 m `GroundCell[]` (polygon-clipped coordinates in engine and map metres, `lightIndex` 0..1, `geometricCoveredFraction`, `contributingTreeIds`).
- **FR-3** Prune target heights MUST come from the engine. The engine uses the verified Unity Sitka lift schedule (`ScenarioOne.asset` `pruningLiftTargetHeightsM` 2.5 / 5 / 6.5 m, `ScenarioOneDefinition.NextPruningTargetHeightM`); the client never sends or computes a target.
- **FR-4** New routes `api/multi/prune/add` and `api/multi/prune/remove` (POST) MUST follow the Fell pattern: the existing `PlayerApi` guards, required-member parsing (full fence + `areaId` + `treeId`), duplicate/unknown/null member refusal, length checks, session + full token + `planStateRevision` fence, 409 with a fresh view on refusal. Queue = Pending only; Approve = zero ecology change; Execute = one lift per approved Prune, immediate, canopy/light refreshed, calendar year unchanged. Existing routes, status codes and guards stay intact.
- **FR-5** The page MUST NOT compute forestry, canopy, light, RD or pruning eligibility; it formats engine values only. Banding a light index into five display bands is display formatting of an engine value.
- **FR-6** Fallen deadwood records MUST be drawn as fallen-log symbols at their engine coordinates (fixed screen size, no fall direction implied, not to scale), with a legend entry beside Naturally dead (D) and Felled stump (X).
- **FR-7** Every new state MUST carry a non-colour cue (letter, glyph, band number, text).
- **FR-8** At 1920×1080 and 1280×720, legend shown and hidden, Light Guide on and off: no horizontal page scroll; the stand stays clear of the navigation dock at Fit.
- **FR-9** A Core test MUST prove execute-time Prune revalidation with whole-plan atomic refusal (zero partial mutation, plan still identifiable, stable refusal code).

## Authority / source of truth

- **Accepted product intent:** owner packet 2026-11-23 (locked decisions above); D-095; D-091; D-020.
- **Implementation facts:** inspected `Tools/MultiRackSitkaP1Lab` at `d3a9f20f` (code base `500a50ae`); `src/Ui/PlayerMultiApi.cs` (`MultiAreaPage` constants); `src/Core/MultiArea/*`; `src/Core/CanopyLightEvaluator.cs`; Unity `Assets/ForestPrototype/ForestTree.cs` (`CanPrune`/`TryPrune`), `ScenarioOneDefinition.cs`, `ScenarioOne.asset` (read-only).
- **Spec authority:** task-level implementation authority for this scope, subordinate to the project authority hierarchy.

## Evidence classification

- Pruning guards, the 8 % cut and the 2.5 / 5 / 6.5 m lift schedule: **Gameplay calibration** ported verbatim from Unity (not timber-quality or physiological evidence).
- Light index: **Simulation abstraction / gameplay calibration** (Unity k10a10 kernel), not calibrated PAR.
- Size-density line and mortality hazard: as labelled by `Stage4Evidence` (UK transfer; gameplay calibration).
- MA-1 stands: **synthetic experimental demonstration**, not an Irish field forecast.

## Save / backward compatibility

No save impact — `/multi` is an in-memory session; nothing is saved. Model 0/0/0 remains the Core default ctor; all frozen fingerprints stay pinned.

## Security / write-back

The new routes sit behind the unchanged `PlayerApi.HandleAsync` guards (capability prefix, route+method, Host, Origin, content type, body size) and use the same strict multi-area body parser. Every write is engine-fenced; a refusal changes nothing and returns 409 with a fresh view; any other failure is 500 outcome unconfirmed (fail closed). No capability URL is ever committed.

## Acceptance criteria

- **AC-1** Build `MultiRackSitkaP1Lab.sln` and `src/Ui/SitkaP1.Ui.csproj`: 0 warnings, 0 errors.
- **AC-2** Core tests: ≥ 181 passing, 0 failing, including the new atomic-refusal test.
- **AC-3** The five anchors re-observed unchanged in the same run: Model 0 single-area `7e3a2a86…1ece`, Model 0 MA-1 `a692a6d5…233d`, Stage 1–3 0/0/1 MA-1 `814e8165…f46287`, Stage 4 ten-year script `8a926776…2b87fb`, P0 pin `9a99f781…34b3` (before and after).
- **AC-4** `tests/ui-smoke-multiarea.sh`: no regression against the base (107 pass; J01/J99 triaged with an exact root cause).
- **AC-5** Real server + real browser (headless Microsoft Edge over CDP): `/multi` loads on 1/1/1; 0 console errors, 0 CSP violations, 0 server 5xx; at 1920×1080 and 1280×720 with legend shown/hidden and Light Guide on/off, `document.documentElement.scrollWidth ≤ innerWidth`, stand clear of the dock at Fit, non-colour cues present.
- **AC-6** Prune gate end to end in the browser: Pending (no crown/light change), Approve (no ecology change), Execute (confirmation, radius × 0.92 immediately, canopy/light readouts update, year unchanged, then ineligible by the 5-year guard), ineligible tree refused with the engine reason.
- **AC-7** Fell + light + years in the browser: Light Guide on, cell inspected, multi-tree Fell in Area A, cover drops, uncovered m² rises, local light lifts, caveat visible; Year+1 several times shows gradual re-closure and natural deaths producing fallen-log markers and counter increments.
- **AC-8** Cross-rack: a tree or cell near the A/B border shows connected ecological response while Work Plans and rack summaries stay per area.
- **AC-9** Eight committed screenshots (1920×1080 and 1280×720 × baseline, Light Guide + ground inspector, Prune Pending/Approved, post-thinning gaps + fallen deadwood), PNG, each ≲ 400 KB, set ≲ 3 MB, no capability token visible.

## Stop conditions

- A change needed outside the grant (`src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `Assets/**`, `.sln`, `global.json`, `Directory.Build.props`, fixtures, build scripts, `.p0ref`, other worktrees).
- Any frozen hash or P0 pin drift; a material public-contract break.
- A product ambiguity the locked decisions and this spec do not settle.
- The real browser cannot be driven.
- A second writer appears on the branch.

## Open product decisions

None blocking. Presentation choices made inside the locked decisions (recorded for owner review in the handoff, not new scope):

- A Stage 4 natural death keeps its standing-dead crown drawing and D badge (engine LifeState BiologicallyDead) and additionally gets the fallen-log symbol for its deadwood record at the same coordinates. Unity hides the tree and shows only a log; the owner may prefer that.
- With the Light Guide on, map clicks inspect ground cells; tree selection stays available from the Trees table and exact-ID box (turning the guide off restores crown clicks).

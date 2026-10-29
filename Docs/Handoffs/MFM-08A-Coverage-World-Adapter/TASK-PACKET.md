# MFM-08A — authoritative coverage-world adapter

**Claim:** #129  
**Status:** **HANDED-OFF — MANAGER REVIEW / INTEGRATION PENDING**  
**Parent:** #91 / #106  
**Exact base:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5` (post-MFM-07 integrated/qualified umbrella; save schema v23)  
**Difficulty:** **4/5** — bounded adapter work, but incorrect spatial mapping would silently change coverage/exclusion and prescription eligibility.  
**Best-suited executor:** OpenAI Sol/Codex high-reasoning T3 Code lane or Claude Code Ultra; one primary writer.  
**Suggested branch/worktree:** `feature/mfm-v1-coverage-world-adapter-v1` / `/media/jer/Videos/CCF-MFM-08A`.

## Purpose

Close the accepted MFM-07 residual before MFM-08 review UX: production `TryReadPrescriptionWorld` currently feeds empty `Masks` and empty `Features` into the already accepted pure `ScenarioOneCoverage.Input` contract. Wire only authoritative existing world/management state into that contract. Do not invent geometry or management facts.

## Existing accepted contract

`ScenarioOneCoverage` already defines:
- masks: `Headland`, `Track`, `Clearing`, `Site`;
- management features: `PlantedJuvenile`, `Shelter`, `ClearancePatch`, `ProtectedArea`, `DeadwoodRecord`, `ActiveManagementWork`, `NaturalRegeneration`;
- excluded/manual/covered semantics and evidence IDs;
- player marks/open/completed work already participate through tree/order inputs.

At the exact base, `ScenarioOneManager.PrescriptionApproval.cs::TryReadPrescriptionWorld` correctly builds confirmed rack geometry, stems and felling-order state but deliberately supplies `new ScenarioOneCoverage.Mask[0]` and `new ScenarioOneCoverage.Feature[0]`. That is the only accepted gap this task closes.

## Binding implementation rule

Map a mask or feature **only** when its position/shape and identity come from an authoritative existing source/API on the exact base.

Known authoritative manager/world state to inspect and map where present includes planted juveniles, shelters/protection, clearance patches, deadwood records and ordinary active management work. Natural regeneration may be supplied only from the authoritative ecology/regeneration state already used by Scenario One.

### Manager source-authority amendment — 2026-10-29

The original STOP rule below was triggered correctly. The Overall Manager then issued the binding disposition directly on #129 (comments `6509104711` and `6509324739`). This amendment records those decisions without creating new product/spec authority:

- `Headland`: use the existing public `ScenarioOneHeadlandGeometry` authority; do not invent alternate geometry/calibration.
- `Track`: use authoritative Dirt Path geometry plus the existing `ForestStartingStand.pathCorridorRadiusMeters`. Minimal read-only accessor exposure is permitted; no reflection or copied literals.
- `Clearing`: use the authoritative `Forest Clearing` position plus the existing `ForestStartingStand.clearingRadiusMeters`. Visual scale is not management authority.
- `Site`: use durable buildable/structure positions plus the same existing `clearingRadiusMeters` authority.
- Player start / live player position is **excluded** from runtime MFM `Site` coverage. Do not add a spawn/save field for this task.
- `ProtectedArea`: do not introduce polygon sampling. Current Scenario One emits zero such features; if an authoritative stored polygon appears, fail closed / report an invalid coverage-world problem until separately authorised mapping exists.
- Shelter/protection features represent authoritative physical objects that still exist in the current world; ecological-effect expiry alone does not remove them from coverage evidence.
- `ScenarioOneManager.ReadCoverage` is permitted only as a thin, side-effect-free read-only façade over the same authoritative world-read + `ScenarioOneCoverage.Classify` path used by production approval. No cache, persistence, fallback or alternate semantics.
- No coverage calibration/tolerance, save/schema, Work Plan commitment, Reference Future, UI or teaching change is authorised by this amendment.

The durable browser closure is `Docs/Handoffs/MFM-08A-Source-Authority-Review-20261029/BROWSER-RESULT.md`.

For infrastructure masks (`Headland`, `Track`, `Clearing`, `Site`), inspect the actual Rack V2 / stand / scene-authoritative geometry APIs before writing anything. **If any accepted mask kind still has no authoritative source/API that can be read without inventing coordinates, widths/radii, or site semantics, STOP and return the exact missing source/API.** Do not guess from visuals, constants, screenshots or calibration values.

## Owned surface

Primary expected production ownership:
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs`
- optionally one new narrowly scoped, read-only adapter/helper beside the MFM coverage code if that keeps source mapping testable.
- minimal read-only `ForestStartingStand` accessor exposure for the two already-existing authoritative radius settings, as allowed by the Manager amendment above.

Focused verification under `Tools/Verification/Mfm*` may be added/updated.

Treat `ScenarioOneCoverage.cs` and calibration/tolerance rules as **read-only accepted contract**. If the existing contract itself prevents faithful mapping, STOP and report a contract defect rather than changing it inside #129.

## Explicit exclusions

No MFM-08 UI/map/3D review UX. No MFM-09 teaching. No Work Plan presentation change. No S1-C/S1-D history or curriculum files. No save/schema/validator/hash/reference change. No calibration/tolerance change. No proposal→Fell-mark behaviour. No broadleaf quantitative extrapolation. No new infrastructure geometry. No Windows build.

## Required verification

Offline/static first:
- focused fixtures for each mapped feature/mask kind;
- evidence IDs are deterministic and sourced from authoritative IDs where available;
- a mapped exclusion changes coverage state/reason exactly through the existing `ScenarioOneCoverage` rules;
- removing/moving the authoritative source changes the adapter result, proving production is not still passing empty arrays;
- no mutation from reading coverage;
- no changes to pure coverage tolerances/calibration;
- authoritative radii are reached through the permitted read-only accessor path;
- moving the player does not change runtime MFM `Site` coverage.

Unity / integration witness under the normal machine-wide lock:
- import/compile on exact candidate;
- runtime world with at least one authoritative management feature demonstrably affecting the production coverage result;
- infrastructure mask witness for every faithfully mappable accepted mask kind;
- if a mask kind cannot be faithfully mapped, STOP instead of qualifying partial silent coverage;
- MFM-04/05/06/07 focused regressions remain green, including proposal read-only and Work Plan commitment separation;
- protected hash/reference comparison unchanged, no rebaseline.

## Stop conditions

Stop and return to Manager if:
- another live writer owns the intended adapter file;
- authoritative geometry/state for an accepted required mask/feature does not exist or cannot be accessed read-only under the Manager amendment;
- mapping would require a new persisted field/schema or a calibration/product decision;
- `ScenarioOneCoverage` contract must change;
- proposed implementation would mutate marks/orders/world while merely previewing/classifying coverage.

## Handoff

Worker handoff is published on `feature/mfm-v1-coverage-world-adapter-v1` at `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/HANDOFF.md`.

Reported gated production candidate: `716486a21588e76a482d3eaea4fed12069f75ffd`; docs-only handoff successor: `0b33c3288ebc78ab7361d0cdb3fcee537324dadb`.

The worker reports the required offline and Unity gates and returned:

`MFM COVERAGE-WORLD ADAPTER COMPLETE — MFM-08 UX MAY RELY ON PRODUCTION COVERAGE`

This is a worker handoff only. Manager implementation review, approval, integration and #156 repin remain separate gates.

## Copy/paste dispatch line

Historical execution dispatch is complete. Do not rerun production implementation from this packet unless the Manager explicitly reopens #129. The next step is Manager review of the exact pushed handoff/candidate, followed by integration/repin if approved.

# Species Wave 2 Moisture Table and Native Profile Cards Implementation Plan

- **Spec ID:** SPEC-multi-2d-species-wave2-profiles-V1
- **Spec status:** IMPLEMENTING
- **Plan owner:** Overall Manager
- **Created:** 2026-11-26
- **Last updated:** 2026-11-26

## Architecture / current-state findings

- Verified: `Tools/MultiRackSitkaP1Lab/src/Core/SitkaP1.Core.csproj` uses SDK default globbing, so new `.cs` files under
  `src/Core/SpeciesWave2/` compile without csproj edits.
- Verified: the Core test project is an Exe whose `Program.cs` calls each test class's `Register(...)` explicitly, then
  `T.RunAll()`. `Program.cs` is outside the file fence.
- Verified: no existing type in `src/` or `tests/` uses the new names (grep at base 6d72c305).
- Verified: gate G6 scans `src/Core/**/*.cs` for clock/entropy tokens and RNG salt literals; the new code uses none.

## Proposed technical approach

- Namespace `SitkaP1.Core.SpeciesWave2`, three files: shared types, moisture table, profile catalog.
- Records and readonly record structs for immutability; static readonly instances; no I/O.
- Crown profile is an abstract record with a shared competition response and two concrete potential-radius forms
  (pine quadratic with clamp; power law for birch and alder).
- Tests self-register through a `[ModuleInitializer]` that calls `T.Add(...)`, avoiding any `Program.cs` edit. Module
  initializers run before top-level statements, so the cases are in the list before `T.RunAll()`.

## Integration boundaries

Touches only the four allowed new-path roots. Nothing references the new namespace from existing code.

## Affected systems / files

- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/{SpeciesWave2Types,SpeciesMoistureTable,FutureSpeciesCatalog}.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SpeciesWave2/SpeciesWave2Tests.cs`
- this spec folder; the handoff folder.

## Ownership / concurrency considerations

Single writer on a dedicated branch/worktree; no shared single-writer files touched.

## Migration / save implications

None.

## Verification strategy

- **Implementation correctness:** 8 new Core tests (W2-01..W2-08) against an independently restated expected table.
- **Spec conformance:** checklist in `checklists/requirements.md`.
- **Verification tier:** headless — build, Core tests, 2D gate `--headless-only` (G0–G7 + frozen anchors). Browser tier
  not tested (no Edge in WSL).

## Risks

- Module-initializer registration is a new pattern in this test project; a reviewer may prefer explicit registration in
  `Program.cs` in a later, authorised change.
- Authored tiers are Calibration; values may change after research review.

## Implementation sequence

1. T-01 types and moisture table.
2. T-02 profile cards.
3. T-03 tests.
4. T-04 build, tests, commit, gate, handoff.

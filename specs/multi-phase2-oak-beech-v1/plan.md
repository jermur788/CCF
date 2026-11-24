# Plan — MULTI Phase 2 Sessile Oak and European Beech (engine only)

**Status: DRAFT — awaiting owner approval (2026-11-24).** `spec.md` is `DRAFT`. The six owner decisions are locked (6/6 Option A). Production edits require owner approval of this quartet, answers to spec §14 Q1–Q6, and a D-091 lane receipt (Gate 0).

- **Spec ID:** SPEC-multi-phase2-oak-beech-V1
- **Plan owner:** Overall Manager
- **Created / last updated:** 2026-11-24

## Architecture / current-state findings (verified read-only; see the pre-flight audit)

- `MultiAreaModelSelection` is a 4-field record (`src/Core/Stage4Models.cs:45-46`); `RequireSupported()` (`:77-89`) allow-lists exactly `Legacy`, `Stage3CrownC2`, `Stage4`, `Stage5` by record equality. `Regeneration` was added as a defaulted fourth parameter — the same pattern takes `Species`.
- `AreaModelVersion` is composed in `MultiAreaContinuation.cs:134-135` via `Stage5Identity.AreaModelVersion(…, models.Regeneration)` (`Stage5Clearing.cs:48`), which appends only when `!= 0`.
- RNG: `ADULT-MORT-` + `TreeId` (`AnnualRules.cs:87`), `STAGE5-MAST` and `STAGE5-PROMO-…` (`Stage5Mast.cs:11-15`).
- Pruning: Sitka-only `PruningRules` (`Stage4Records.cs:17-30`): 3 lifts, 5 years, 0.92. Deadwood volume `Stage4Records.cs:79-87`.
- Stage 5 files: `src/Core/MultiArea/Stage5*.cs` (Annual, Bands, Clearing, Juvenile, Mast, Models, Promotion, Records, SeedRain, Undergrowth); order template `MultiAreaClearUndergrowthOrder` (`Stage5Records.cs:38`), `MultiAreaPruneOrder` (`MultiAreaStage4.cs:8`).
- `SpeciesProfile.cs` holds a single `sitka-spruce` profile (`:34`).
- The Stage 5 `1/1/1/1` fingerprint `c4b670ed…` is **printed, not pinned** (no occurrence under `tests/`; reviewer nit R8).
- Unity already contains reference implementations Phase 2 transfers from: `MixedDensityModel.cs`, `MixedDensityModel.Composite.cs`, `SitkaSubsetDensitySignal.cs`, `BrowsingConditions.cs`, `JuvenileEcologyRules.cs`, `ForestTree.CanPrune`, `ScenarioOneManager.AdvancePlantedJuveniles`, `ScenarioOneCompetition.cs`, `VegetationClearance.cs`.

## Gate 0 — before any production edit (all must be true)

1. Quartet marked `APPROVED`; Q1–Q6 answered and recorded verbatim in `spec.md`.
2. Fresh `Docs/Project/coordination/active-tasks.md` and `origin/main` read; no overlapping writer on `Tools/MultiRackSitkaP1Lab`.
3. D-091 worktree/path admission recorded; implementation branch created from `5b757d9b` (or from this docs branch's approved HEAD, which is docs-only on top of it).
4. Baseline recorded **before** edits: all protected anchors (spec §0 / audit §8), the `1/1/1/1` full-run hash, test count, `dotnet build` warnings/errors.

## Proposed technical approach — stages (each ends green with a recorded hash)

**Stage 1 — read-only confirmations (no edits).** Comeau callable on a Sitka subset without pure-rack change; Unity band/accumulator limit per cell vs per species-in-cell; Unity patch `competitionUpdatedYear` at creation; Unity `report.year` vs ecological year for `InstalledYear`; engine `SimulationRandom` call shape for the new keys.

**Stage 2 — contracts, no behaviour.** `SpeciesModelVersion`, fifth selection field, `Phase2` static, allow-list and refusals, null `MultiAreaPhase2State`, token rule (Q1). **Promote `c4b670ed…` to a pinned constant** in the frozen-path test (declared test change, closes nit R8). Prove `P2-01`.

**Stage 3 — pure rules, unit-first.** New small files under `src/Core/MultiArea/`, each with tests before wiring: `Phase2Species` (constants, crown `r_pot`, light tables), `Phase2AdultGrowth`, `Phase2MixedDensity` (generic component + routing), `Phase2Pruning`, `Phase2Mast`, `Phase2SeedRain`, `Phase2Establishment` (shared capacity), `Phase2Promotion` (shared block cap, tie-break), `Phase2Browse` (shelter, access, expiry), `Phase2SpotPatches`, `Phase2PlantedJuveniles`.

**Stage 4 — annual wiring.** Insert per spec §4; start-of-year reads; one root commit; adult-empty racks continue.

**Stage 5 — commands.** `Plant`, `Shelter`, `SpotClear` through the existing fence; validation table spec §7.2; atomic refusals.

**Stage 6 — qualification `P2-01`…`P2-10`** in `Core.Tests` (Tier 2 bounded pure-.NET). Status words kept separate: `IMPLEMENTED` ≠ `NUMERICALLY QUALIFIED` ≠ `REVIEWED`.

**Stage 7 — handoff** on GitHub, then independent review, then owner acceptance.

## Integration boundaries

- **May touch:** `Tools/MultiRackSitkaP1Lab/src/Core/**`, `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/**`, `specs/multi-phase2-oak-beech-v1/**`, `Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/**`.
- **Must not touch:** `Assets/`, `.meta`, `ProjectSettings`, `/multi` wwwroot (HTML/JS/CSS), save schemas, generator defaults, MA-1 fixture data, Stage 1–5 rule code paths reachable at `Species = Off`.
- **Declared test change:** the `1/1/1/1` fingerprint becomes pinned (R8); no other existing test is edited.

## Ownership / concurrency

Single writer on `Tools/MultiRackSitkaP1Lab` under D-091. `/multi` UI lane must not run concurrently on the same files.

## Migration / save implications

None — session-only state; no schema change.

## Verification strategy

- **Build:** `dotnet build --disable-build-servers` with `TMPDIR` outside the repo; **0 warnings / 0 errors**.
- **Implementation correctness:** unit tests per rule file; all existing Stage 1–5 Core tests pass unchanged.
- **Spec conformance:** `checklists/requirements.md` `P2-01`…`P2-10` with evidence.
- **Verification tier:** Tier 1 during build; Tier 2 bounded pure-.NET numerical qualification. No Unity, no UI, no browser verification is claimed.

## Risks

- Calling Comeau on a subset may expose hidden whole-rack assumptions (Stage 1 check; stop condition).
- Adding `SpeciesId` to bands could reorder Stage 5 serialisation and move `c4b670ed…` (mitigated: new shape only in Phase 2 paths).
- Oak `PromotionMinimumLight = 0.2` may make Oak promotion rare under C2 canopy; that is a calibration consequence, reported, not retuned.
- Generic victim choice is species-neutral; Beech/Oak survival differences therefore come from growth only (by design, D-061).

## Stop conditions

As spec §Stop conditions: any frozen hash moves at `Species = Off`; need to touch excluded files; Comeau subset impossible without pure-rack change; band-limit port moves `1/1/1/1/0`; D-091 not granted. Report the fact; do not retune, widen or invent.

# Scots Pine and Site Moisture V1 — Part 1 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: MULTI-2D-SCOTS-PINE-MOISTURE-P1-20261127
Worker: Codex GPT-6.2 Sol (high) authored the spec quartet and module (5b453e30, 08b26003);
        the run then failed on provider 429 rate limits. Claude Opus 5.5 (claude-opus-5-5) resumed in the
        same worktree and authored the test project and this handoff. Neither author has reviewed the other's work.
Role: Sole implementation writer (Option A, owner-approved 2026-11-27)
Branch: feat/multi-2d-scots-pine-engine-v1
Base: c82e4e759c2ceef1c13ddb6a033e034097aef77e (origin/integration/2d-sim)
Context commit: bdeb6a324acaf4129c10fb0846faac8b64974360 (origin/main)
HEAD: the latest commit touching this file. Code under test is 576cafde (tests), on top of 08b26003 (module) and 5b453e30 (spec).
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/IMPLEMENTATION-HANDOFF-PART1.md
GITHUB HANDOFF COMMIT: the latest commit touching this file on feat/multi-2d-scots-pine-engine-v1
EXTERNAL EVIDENCE ARCHIVE: /tmp/scotspine-gate/{version,restore,build,run1,run2}.txt (local, volatile; summarised below)
```

**State (D-030):** isolated branch implemented and verified; NOT merged; NOT wired.

## What changed

- A four-file spec quartet, `SPEC-multi-2d-scots-pine-moisture-V1`, status APPROVED / IMPLEMENTING. It covers Part 1 (implemented) and Part 2, which is QUEUED behind Playable Milestone 1. The plan stamps context `bdeb6a32…4974360` and base `c82e4e7`.
- A pure .NET 8 module in namespace `SitkaP1.Core.SpeciesWave2.ScotsPine`. It uses only the BCL, with no I/O, no clock and no `System.Random`:
  - `SiteMoistureEngine`:
    - per-species modifiers read straight from the existing `SpeciesMoistureTable`;
    - area-weighted dominant-class selection, with ties resolved by declaration order and empty input giving `NeutralLegacy`;
    - the `DryRidgeAndMoistSlopeMap`, split on the x-axis midpoint: x below the midpoint is `DryFreeDraining`, and the midpoint itself plus everything above is `FreshMoistMineral`;
    - `MoistureSelection.HabitatProtectionCaution`.
  - **Leader note (wording correction):** Scots pine is the *sole* leader on `DryFreeDraining`. On `FreshMoistMineral`, Sitka spruce is only a *joint* leader. All six species are G/G there in the existing `SpeciesMoistureTable`, so no species leads on its own. Saying "Sitka leading" on fresh moist mineral soil overstates the table. MOISTURE-02b asserts the six-way tie exactly.
  - `ScotsPineEngine.EvaluateGrowth`: a provisional [Calibration] DBH increment inside the GROWFOR domain from the card. It returns null outside that domain.
  - `ScotsPineEngine.EvaluateRegeneration` / `IsGoodSeedYear`:
    - local seed only, from a seed tree at least 18 years old;
    - one good seed year per 4-year block, which gives gaps of 3–5 years. The salt `0x1A96C03752AE8D4B` is declared inside the module; `RngSalts.cs` is untouched;
    - canopy openness at least 0.60 and undergrowth at most 0.30.
  - `ScotsPineEngine.GradeTimber`:
    - prices: Sawlog 68, Small Roundwood 45, Pulp/Chip 38 EUR/t;
    - green density 1.02 t/m³, read from the pine card;
    - sapling material cost EUR 0.45 per tree;
    - Sawlog only for mature stems that are pruned to at least 5.0 m or grew under tight early stocking.
- Test project `tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj`, an Exe. It compiles only `../../../src/Core/SpeciesWave2/**/*.cs` plus its own test files. It runs 26 checks, prints pass/fail counts per suite and in total, prints the canonical SHA-256, and exits non-zero on any failure.

## Files changed (vs base c82e4e7)

- `specs/multi-2d-scots-pine-moisture-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/ScotsPine/{SiteMoistureEngine,ScotsPineGrowth,ScotsPineRegeneration,ScotsPineTimber}.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core/ScotsPine/{SitkaP1.ScotsPine.Tests.csproj,Program,TestRunner,SiteMoistureTests,ScotsPineBiologyTests,ScotsPineTimberTests,DeterminismTests}.cs`
- `Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/IMPLEMENTATION-HANDOFF-PART1.md`

Everything is inside the four authorised prefixes. No existing file was modified.

- **Serialized Unity files touched:** none. **Assets added or modified:** none.

## Player-facing result

None yet. Part 1 is a callable library that the game does not call. Part 2 will make it visible: dry and moist zones, Scots pine leading on the dry ridge, local-seed gap regeneration, pruning value and a bog caution in `/multi`.

## Simulation / save impact

None. No change to AnnualPipeline, MultiAreaContinuation, ForestMaker, economy, saves, UI, RNG salts or the existing Wave 2 files. No save fields. `NeutralLegacy` stays bit-exact (1.0, 1.0) for all six species.

## Anchors

- **Must-not-change:** Scenario One / Reference Future v1 and existing P1 save and determinism anchors. They are untouched because no existing source changed.
- **Intentionally changed:** none.
- **New pin:** the Part 1 canonical digest `96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748`, under SDK 8.0.133.

## Verification run (from `Tools/MultiRackSitkaP1Lab`, `dotnet --version` = 8.0.133, clean `bin/obj`)

```text
$ dotnet restore tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj
  Determining projects to restore...
  Restored .../tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj (in 294 ms).      exit 0
$ dotnet build tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)                                                                        exit 0
$ dotnet run --project tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj --no-build --no-restore
SUITE Determinism: 1/1 passed, 0 failed
SUITE ScotsPineBiology: 9/9 passed, 0 failed
SUITE ScotsPineTimber: 5/5 passed, 0 failed
SUITE SiteMoisture: 11/11 passed, 0 failed
TOTAL: 26/26 passed, 0 failed
DIGEST canonical-sha256 96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748   exit 0
```

`TreatWarningsAsErrors` is on through `Directory.Build.props`.

**Determinism.** A second separate process run gave byte-identical full output (`cmp run1 run2`) with the same digest. DET-01 also checks that the canonical text is identical within one process and that it matches the pinned digest. Doubles enter the canonical text as raw IEEE-754 bits.

**Negative control.** With a one-character change to the pinned digest, the run printed `TOTAL: 25/26 passed, 1 failed` and exited 1. The pin was then restored.

**Not run, by brief:** `dotnet test`, Unity, browser suites and `2d-gate.sh`.

## SPEC-DRIVEN

- **Spec:** `SPEC-multi-2d-scots-pine-moisture-V1`. Status APPROVED / IMPLEMENTING.
- **Tasks completed:** T-01 to T-06. T-07 (Part 2 wiring) is QUEUED behind Playable Milestone 1.
- **Deviations:** none from the spec. The four file-fence commits are spec (5b453e30), module (08b26003), tests (576cafde) and this handoff.
- **Unresolved requirements:** AC-7 (Part 2) only.

Acceptance criteria tested:

| AC | Cases | Result |
| --- | --- | --- |
| AC-1 neutral identity, table agreement, separate modifiers, dry lead / fresh tie | MOISTURE-01a–d, 02a–b | PASS |
| AC-2 weighted selection, full two-zone coverage with midpoint rule, bog-only caution | MOISTURE-03a–d, 04 | PASS |
| AC-3 regeneration gates (seed tree, seed year, light, undergrowth), 3–5 yr spacing, determinism, site multiplier | REGEN-01a–c, 02a–c | PASS |
| AC-4 both Sawlog paths, both downgrades, 68/45/38 EUR/t, 1.02 t/m³, EUR 0.45/sapling, no Sitka reuse | TIMBER-01a–d, 02 | PASS |
| AC-5 card reuse, domain-bounded growth, potential vs adjusted, Calibration tag | PINE-01, GROWTH-01/02 | PASS |
| AC-6 pinned gate, 0 warnings/errors, cross-run digest, file fence | DET-01 + receipts above | PASS |
| AC-7 integrated Part 2 | — | NOT TESTED (queued) |

**Spec conformance (Part 1):** PASS, worker-verified only. It has not been independently reviewed.

## Remaining verification gates

- An independent review of the spec, module and tests at this HEAD. The module was authored by Codex and the tests by Claude, so the reviewer should be a third session that is neither author.
- The coordinator creates `review/multi-2d-scots-pine-engine-v1` after that review. This worker did not create it.
- All Part 2 gates: integrated annual loop, generation, Version 3 save/load compatibility including old saves defaulting to `NeutralLegacy`, and `/multi` UI.

## Known limitations and residual risks

- **Price coincidences.** The locked pine prices are declared separately from Sitka, but two of them equal Sitka figures. Pine Pulp/Chip EUR 38/t equals the Sitka `sitka-pulp` selected price (3800 c/t), and pine Sawlog EUR 68/t equals the Sitka `sitka-pallet` selected price (6800 c/t). TIMBER-02 checks that pine Sawlog is not the Sitka sawlog price (EUR 95/t) and that 1.02 t/m³ is not the D-062 890 kg/m³ fresh-volume proxy. Equal numbers cannot be told apart from reuse by value alone; these are separately declared constants.
- **Sapling cost scale.** EUR 0.45 per Scots pine sapling is ten times lower than the economy's Beech (EUR 4.50) and Oak (EUR 5.50) on-demand sapling materials. It is a brief-locked [Calibration] value, but the gap should be confirmed before Part 2 puts it into Work Plans.
- **Tie at exactly equal areas.** The dominant-class tie rule (declaration order) makes a perfectly 50/50 Dry Ridge & Moist Slope woodland select `DryFreeDraining` as its whole-woodland class. This is documented and tested, and Part 2 should decide whether that is the intended label.
- **Calibration, not prediction.** The growth shape (0.60 → 0.20 cm/yr, up to 50 % crowding reduction), the 18-year seed-tree age, the 0.60 openness and 0.30 undergrowth thresholds, and the 4-year seed-year block are all provisional [Calibration] teaching values, not empirical predictions (D-059). The block design can never produce gaps of 2 years or 6 or more.
- **Caller-supplied inputs.** Regeneration takes the seed-tree age, openness and undergrowth from the caller. There is no dispersal-distance model and no recruit creation. Part 2 must define how these come from map state.
- **Copied Sitka literals.** The test project cannot see the Sitka economy sources, so the Sitka comparison values in TIMBER-02 are copied literals. A future change to the Sitka prices would not update them.

## Unrelated work preserved

The tree was clean at the base. No other worktree, branch, review branch or merge was touched.

## Decisions needed

None for Part 1. For Part 2, the owner or Manager should confirm the sapling-cost scale and the label for an exact 50/50 tie.

## Worker assessment

Ready for review: Yes. The scope is Part 1, offline .NET only.

## Recommended next step

Run a non-author independent review of `feat/multi-2d-scots-pine-engine-v1` at this HEAD against the spec quartet, then let the coordinator create the review branch. Part 2 stays queued behind Playable Milestone 1.

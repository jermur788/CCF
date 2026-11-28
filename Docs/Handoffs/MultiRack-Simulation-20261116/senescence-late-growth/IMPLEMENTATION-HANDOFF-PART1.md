# Natural Old-Age Tree Death and Smooth Late-Life Growth V1 — Part 1 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: MULTI-2D-SENESCENCE-P1-20261128
Worker: Claude Opus 5.5 (claudeAgent), single lane writer, continuing the aborted Gemini 4 Neon draft after provider quota exhaustion
Role: Sole implementation writer (Playable Milestone 8 Part 1 packet, 2026-11-28)
Branch: feat/multi-2d-senescence-engine-v1
Base: b2006767cbe70fa2c822497470908251a0fd6ffd (origin/integration/2d-sim)
Context commit: bdeb6a324acaf4129c10fb0846faac8b64974360 (origin/main)
HEAD: the latest commit touching this file. Under test: spec d4208f27, engine c852cc91, tests 16addae6.
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/senescence-late-growth/IMPLEMENTATION-HANDOFF-PART1.md
EVIDENCE (local scratch, not archived): /tmp/ccf-senescence-p1-evidence/
  (version.txt, restore.txt, build.txt, run1.txt, run2.txt, export-build.txt, run3-export.txt)
```

**State:** implemented on an isolated branch. **NOT merged** to `integration/2d-sim` or `main`. **NOT wired** into anything. Not pushed. Awaiting independent review.

## Continuation of the aborted draft

- **Inherited, contrary to the brief:** the previous writer had already committed the spec quartet as `54ac2547`, with the exact commit-1 subject. Only the four engine files were untracked. I amended that local, unpushed commit with the spec corrections below, giving `d4208f27`. The base is untouched, and `54ac2547` survives only in the reflog.
- **Kept:** all six binding rulings were already implemented correctly in the drafts: hazard parameters and smoothstep, version gate, key layout, Model 1 hash, death semantics, and the late-life anchor and k.
- **Repaired:**
  - `SenescenceDeathRecord` took `Cause` and `PlayerCauseText` as constructor arguments. They are now fixed by the type; the only public constructor is `(TreeId, SpeciesId, Year)`.
  - `SenescenceStreams.KeyFor` accepted any species string. It now accepts only the six supported IDs, which makes key injectivity hold by construction.
  - `RollModel1` changed from private to internal, so the test can prove Model 1 parity.
  - The survival figures mixed interval conventions. Alder 70→150 is ≈ 0.140, not ~0.144, under the single inclusive convention now stated in the spec and code.
  - Spec additions: the no-cliff statement, the integration prerequisites, and the GROW-13 / HYG-01 case mapping. `LateLifeGrowth.cs` is unchanged.

## What changed

- **Spec quartet** `SPEC-multi-2d-senescence-late-growth-v1`: Part 1 is implemented; Part 2 is deferred.
- **Module** `SitkaP1.Core.NaturalProcesses.Senescence`. It is pure BCL plus read-only calls into Species Wave 2: no I/O, no clock, no `System.Random`, and no edit to any existing file.
  - `SenescenceHazard`: six profiles keyed by the `SpeciesMoistureTable` IDs, the smoothstep hazard `h(age)`, and `SurvivalProbability` (inclusive at both ends).
  - `SenescenceEngine`: the version gate.
    - `Disabled` (the default) returns zero deaths, zero draws and zero evaluations.
    - `OldAgeV1` sorts trees by `TreeId` and makes one stateless roll per tree. The roll uses key `SENESCENCE-OLD-AGE-v1-<species>/<treeId>`, FNV-1a and SplitMix64, and returns a 53-bit double.
  - `SenescenceDeathRecord`: cause `old-age`, wording "Died of old age", biological death, EUR 0 timber revenue, clears crop-tree marks, never counts as a fell.
  - `LateLifeGrowth`: for Scots pine (cutoff 60), Common alder (60) and Downy birch (70).
    - At or below the cutoff it delegates bit-exactly to the existing evaluators.
    - Beyond the cutoff the increment is `A·exp(−k(age−cutoff))`, where `A` is the delegated anchor and `k = −windowSlope/A`. It keeps the same crowding rule, domain guards and single moisture multiplier.
    - Sitka spruce, Sessile oak and European beech are refused.
- **Test executable** `tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj`: 42 checks.

## Files changed (vs base b2006767)

- `specs/multi-2d-senescence-late-growth-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
- `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/Senescence/{SenescenceContracts,SenescenceHazard,SenescenceEngine,LateLifeGrowth}.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core/Senescence/{SitkaP1.Senescence.Tests.csproj,Program,TestRunner,Fixtures,VersionGateTests,HazardTests,RollsAndSemanticsTests,LateLifeGrowthTests,DeterminismTests,HygieneTests}.cs`
- `Docs/Handoffs/MultiRack-Simulation-20261116/senescence-late-growth/IMPLEMENTATION-HANDOFF-PART1.md`

All 19 files are new and inside the four authorised prefixes. **No existing file was modified.** No serialized Unity file, asset, package or ProjectSettings change. `2D_Simulator_Strategic_Roadmap.md` and `Docs/Project/**` were not edited.

## Player-facing result

None yet. Part 1 is a library that nothing calls. In Part 2, a woodland with `OldAgeV1` will occasionally lose old trees as natural deaths that read "Died of old age". Pioneers will thin out first, and old pine, birch and alder stems will keep growing slowly instead of stopping at age 60 or 70.

## Simulation / save impact

None. `AnnualPipeline`, `RngSalts`, `MultiAreaContinuation`, saves (Version 2 and Version 3), `src/Ui` and every existing Wave 2 and NaturalProcesses file are untouched. `Disabled` is the default and draws nothing.

## Anchors

- **Must-not-change:** all frozen baselines, Scenario One, save anchors, and the existing Wave 2, Birch/Alder and Scots pine digests. All are untouched because no existing source changed.
- **New pin:** `46da98026d8e591702ba09cf272bf3b0a76cd4f969ea5c0e1a4825197b52878c` (SDK 8.0.133, linux-x64).

## Verification run (from `Tools/MultiRackSitkaP1Lab`, `dotnet --version` = 8.0.133)

```text
$ dotnet restore tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj
  Restored .../tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj (in 2.23 sec).     exit 0
$ dotnet build tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj -c Release --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)                                                                          exit 0
$ dotnet run --project tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj -c Release --no-build --no-restore > run1.txt   exit 0
$ dotnet run --project tests/Core/Senescence/SitkaP1.Senescence.Tests.csproj -c Release --no-build --no-restore > run2.txt   exit 0
SUITE Determinism: 2/2 passed, 0 failed
SUITE Hazard: 10/10 passed, 0 failed
SUITE Hygiene: 1/1 passed, 0 failed
SUITE LateLifeGrowth: 13/13 passed, 0 failed
SUITE RollsAndSemantics: 10/10 passed, 0 failed
SUITE VersionGate: 6/6 passed, 0 failed
TOTAL: 42/42 passed, 0 failed
DIGEST canonical-sha256 46da98026d8e591702ba09cf272bf3b0a76cd4f969ea5c0e1a4825197b52878c   (run1)
DIGEST canonical-sha256 46da98026d8e591702ba09cf272bf3b0a76cd4f969ea5c0e1a4825197b52878c   (run2)
$ cmp run1.txt run2.txt                                                                  byte-identical
```

- **Committed tree:** a `git archive` export of `16addae6`, cold-built in `/tmp`, gave 0 warnings and 0 errors, 42/42, and output byte-identical to `run1.txt`.
- **Negative control:** the first run with a placeholder pin printed `TOTAL: 41/42 passed, 1 failed` and exited 1. The pin is live.
- `TreatWarningsAsErrors` comes from the lab `Directory.Build.props`. Doubles enter the canonical text as raw IEEE-754 bits.
- **Not run, by brief:** `dotnet test`, Unity, browser suites, `Core.Tests`, `Phase1.Save.Tests`, `build/2d-gate.sh`, the NaturalProcesses test project, and the Core project build (which needs `.p0ref`). Part 1 is not Unity-relevant, so no Unity verification is claimed.

## Integration prerequisites

- **NaturalProcesses test project (blocking at combination).** By inspection, `tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj` compiles `src/Core/NaturalProcesses/**/*.cs` recursively and no Species Wave 2 sources. It therefore picks up this module and fails to compile. This lane does not edit it. The integrator applies one of:
  - (a) `<Compile Include="../../../src/Core/SpeciesWave2/**/*.cs" />`
  - (b) `<Compile Remove="../../../src/Core/NaturalProcesses/Senescence/**/*.cs" />`

  Either option also needs a matching update to that project's JW-T64, which asserts exactly one `<Compile ` item and the exact glob line. Its JW-T63 text scan reads this folder too; HYG-01 shows these sources pass JW-T63's banned list.
- **Main Core project.** It compiles these files via its `src/Core/**` glob, but nothing references them. By inspection it is unaffected: no type-name clash, and no G6, B-01 or RNG-literal hit. It was not built in this lane.

## SPEC-DRIVEN

- **Spec:** `SPEC-multi-2d-senescence-late-growth-v1`, APPROVED / IMPLEMENTING (Part 1). T-01 to T-06 are done; T-07 is deferred.

| AC | Cases | Result |
| --- | --- | --- |
| AC-1 version gate and baseline preservation | GATE-01..06 | PASS |
| AC-2 six-species hazard curves, ordering, pioneer persistence, heart-rot separation | HAZ-01..10 | PASS |
| AC-3 keyed rolls, guards, death semantics, wording, residual cohort | ROLL-01..10, DET-02 | PASS |
| AC-4 Wave 2 late-life growth continuation | GROW-01..13 | PASS |
| AC-5 pinned offline gate, 0/0, cross-process digest | DET-01 + receipts above | PASS |
| AC-6 hard write fence | `git diff --name-only b2006767..HEAD`: 19 new files, four prefixes | PASS |
| AC-7 Part 2 integration | — | DEFERRED, not tested |

**Spec conformance (Part 1):** PASS, worker-verified only.

## Deviations from the brief

- **"Drop cutoff→cutoff+1 < 3 % of anchor" is unattainable under binding ruling 6.** With `k = −windowSlope/A`, the first-year drops are Scots pine 3.92 %, Common alder 6.59 % and Downy birch 3.63 % of `A`. Each is strictly below the existing window's own final-year decline (4.00 / 6.82 / 3.69 %). GROW-07 asserts that principled no-cliff form instead, and GROW-06 asserts C1 continuity. The ruling was kept; the threshold was not.
- **Added cases and files:** HYG-01 with `HygieneTests.cs`, and the shared `Fixtures.cs`. The case total is 42.
- **Commit 1 was amended rather than created**; see the continuation section above.

## Remaining verification gates

- Independent review of `d4208f27..16addae6` by a fresh, non-implementing thread.
- At combination: the NaturalProcesses prerequisite above, then the integrator's full gate.
- All Part 2 gates (AC-7): pipeline wiring, the `RngSalts` registration with a Core test pinning equality, live growth wiring, Version 3 six-species deadwood saves, and History/Habitat UI.

## Known limitations

- **Calibration, not prediction.** Every onset, ramp, ceiling and decay rate is a `[C]` gameplay value (D-059). Only the longevity ordering is `[I]`/`[T]`.
- **Libm dependence.** The continuation uses `Math.Exp`, so the digest is tied to the verified runtime and platform. Part 2 must decide whether these values may reach saves or cross-platform comparisons.
- **Disabled still validates its envelope.** It refuses `Year < 1` and undefined model values, unlike StormModel 0. Part 2 must call it with `Year >= 1`.
- **Extreme ages.** The continuation underflows to `0.0` only at absurd ages, around 10,000+ years. It is never negative.
- **Copied literals.** GATE-06 (RNG families) and DET-02 (Model 1 vectors) copy values from files this standalone project cannot compile.

## Unrelated work preserved

The worktree started with only this lane's drafts. No other worktree, branch, lane, save, Unity library or `/media/jer/Files/CCF-Primary` content was touched. Nothing was pushed or merged.

## Worker assessment

Ready for review: Yes. Scope is Part 1 only, an offline .NET library; it is not merged and not wired.

# Natural Old-Age Tree Death and Smooth Late-Life Growth V1 — Part 1 Independent Review

```text
INDEPENDENT REVIEW RECORD
Task: MULTI-2D-SENESCENCE-P1-20261128
Reviewed branch: review/multi-2d-senescence-engine-v1 (implementation branch feat/multi-2d-senescence-engine-v1)
Reviewed code SHA: 693826a11e911312d96df62d83ec26c4dbd9bb67
Base: b2006767cbe70fa2c822497470908251a0fd6ffd (origin/integration/2d-sim)
Protected origin/main: bdeb6a324acaf4129c10fb0846faac8b64974360 (unchanged)
First review: c1201e8f82acfb4195a43265e6b8bca98d133f4a -> NEEDS CHANGES
Delta review: 693826a11e911312d96df62d83ec26c4dbd9bb67 -> APPROVE WITH NITS
Reviewer model: OpenAI Codex GPT-6.2 Sol (reasoning effort high), two separate fresh top-level threads
Implementation: Gemini 4 Neon (draft, quota-exhausted) continued by Claude Opus 5.5 (effort max)
INTEGRATION DECISION: APPROVE WITH NITS
```

## Scope

Read-only independent review of Playable Milestone 8 Part 1 (isolated engine), covering the four-file
spec quartet, the six-species senescence engine, the Wave 2 late-life growth continuation, the standalone
offline console gate and the implementation handoff. The reviewer did not author or modify the branch.

## First review — c1201e8f (NEEDS CHANGES)

The first independent review reproduced the gate (**42/42, 0 warnings, 0 errors**, digest
`46da9802...7b52878c`, two byte-identical processes) and the 19-file fence, and returned **NEEDS CHANGES**:

| Severity | Finding |
| --- | --- |
| MAJOR | The 32-bit FNV-1a key hash let distinct tree IDs collide before year/seed mixing. Proven pair: `common-alder/MA-A-5B7Z` and `common-alder/MA-A-Q3QA` (both `0xF88F6C5C`), so equally aged trees with those IDs shared every annual roll. |
| MINOR | `SurvivalProbability(int.MaxValue, int.MaxValue)` wrapped its `int` loop counter and threw. |
| MINOR | Track A longevity-ordering provenance was not traceable enough: no per-species evidence table, no explicit `[E]` disposition and limit. |

## Repair cycle 1 — 693826a1

One bounded repair commit (`fix(senescence): repair cycle 1 — 64-bit keyed roll, survival overflow guard, Track A provenance`):

- The key string is hashed with **64-bit** FNV-1a and mixed through SplitMix64 without any 32-bit truncation
  before the final 53-bit uniform conversion. `OldAgeV1` and the `SENESCENCE-OLD-AGE-v1-` prefix are unchanged
  because v1 was never accepted, registered or persisted; the collision-hardened roll is part of its
  pre-registration definition, and any post-integration change requires `OldAgeV2`.
- DET-02 replaced 32-bit Model 1 parity with a collision-hardened roll case, including the reported pair, a
  named regression, a 20,000-ID distinct-roll check and a mortality-band check.
- `SurvivalProbability` uses a 64-bit loop counter; HAZ-08 asserts the single-year max-int call equals
  `1 - h(int.MaxValue)` without throwing.
- Track A gained a per-species approximate-longevity table (`[I]`/`[T]`/`[C]`), an explicit no-`[E]` statement,
  an explicit "no primary source supplied, none cited" limit, and a documented Sitka mid-longevity `[C]`
  simplification for Irish plantation conditions.
- New pinned digest: `313e0f5a795bd7774752affb673927cc875bc6d89b9b988ddaadf3efa4e4bbb6`.

## Delta review — 693826a1

Independent re-run from the review worktree with SDK 8.0.133: restore/build **0 warnings, 0 errors**;
two separate processes **42/42**, byte-identical, both printing
`313e0f5a795bd7774752affb673927cc875bc6d89b9b988ddaadf3efa4e4bbb6`. Fence: exactly **19 additions**
under the four authorised prefixes, `git status` clean, no existing file modified. Live `origin/main` and
`origin/integration/2d-sim` matched their protected SHAs.

| Prior finding | Closure |
| --- | --- |
| MAJOR key collision | **CLOSED.** Full key -> 64-bit FNV-1a -> SplitMix64; the reported pair and a fresh 32-bit collision pair diverged in 30/30 year/seed cells; 20,000 IDs produced 20,000 distinct rolls; death fixture 638/20,000 (3.19%) against a 3% hazard; spec/tasks/handoff no longer claim 32-bit Model 1 parity. |
| MINOR survival overflow | **CLOSED.** 64-bit loop; max-int single-year returns 0.97 without throwing. |
| MINOR provenance | **CLOSED.** Per-species approximate longevity table, explicit labels and limits, no fabricated citation, no `[E]` overclaim. |

No Part 1 behavioural regression was found. The reviewer independently confirmed exact in-window delegation,
cutoff anchors `0.20` / `0.19999999999999996` / `0.25`, C1/cliff behaviour, single crowding and moisture
application, refusals, order independence, one death per tree, Disabled zero-draw behaviour and the exact
player text "Died of old age".

## Verbatim decision

```text
INTEGRATION DECISION: APPROVE WITH NITS
```

## Nits and prerequisites

- **NIT (publication status):** the implementation handoff line "not pushed" was written before publication.
  The live `origin/feat/multi-2d-senescence-engine-v1` now points at `693826a1`; the reviewed code SHA is
  unchanged and this record resolves the staleness. No code change was made for this nit.
- **Part 2 prerequisite (RNG registry):** registering the senescence stream in `RngSalts.cs` must keep a single
  production literal source; a duplicated literal would trip the G6 duplicate-literal scan.
- **Combination prerequisite (confirmed by `dotnet msbuild -getItem:Compile`):** the existing standalone
  `tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj` recursively globs
  `src/Core/NaturalProcesses/**` and includes no Species Wave 2 sources, so it will pick up this module and fail
  to compile. The single integrator must add either
  `<Compile Include="../../../src/Core/SpeciesWave2/**/*.cs" />` or
  `<Compile Remove="../../../src/Core/NaturalProcesses/Senescence/**/*.cs" />`, plus the matching JW-T64 update.
  Both sit outside this lane's write fence and were deliberately not made here.

## State

- Reviewed as an isolated Part 1 engine branch.
- **NOT merged** to `integration/2d-sim` or `main`.
- **NOT wired** into `AnnualPipeline`, `RngSalts`, saves or `/multi`.
- Unity, live save/UI behaviour and the integrated gate remain unverified and out of scope for Part 1.

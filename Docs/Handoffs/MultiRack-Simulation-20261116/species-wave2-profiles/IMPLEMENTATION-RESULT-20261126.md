# Species Wave 2 Profiles — Implementation Result (BLOCKED at gate G2)

## Identity

- Provider/model: Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code; effort medium.
- Role: sole implementation sub-agent, owner-authorised bounded 2D simulation lane.
- Branch: `feat/multi-2d-species-wave2-profiles-20261126-v1`
- Base: `6d72c30538b1fbe30d3cbcf083b5b368733662b8`
- Implementation commit: `8490c218155db139367a46a04a3da94215a9d685` (this handoff is committed on top of it; see `git log`).
- Worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-SpeciesWave2-20261126`
- Push status: **NOT PUSHED.** The packet's stop rule applies because gate G2 failed (see below).

## Verdict

**BLOCKED — gate G2 FAIL. The only failing case is the frozen Save test NB1c, which is a known race and is not part of
this change.** The implementation itself builds, and Core tests pass 299/299 both standalone and inside the gate.
Not integrated, not wired, not independently reviewed.

## Changed files (all brand-new, all inside the fence)

- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/SpeciesWave2Types.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/SpeciesMoistureTable.cs`
- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/FutureSpeciesCatalog.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SpeciesWave2/SpeciesWave2Tests.cs`
- `specs/multi-2d-species-wave2-profiles-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
- `Docs/Handoffs/MultiRack-Simulation-20261116/species-wave2-profiles/**` (this file + `evidence/*.txt|json`)

No existing file was edited. `.p0ref/` is gitignored (the seed copy is untracked).

## Commands and key results

| Step | Command (inside worktree, via `wsl -e bash -lc`) | Result |
|---|---|---|
| Base check | `git status --porcelain; git rev-parse HEAD; git branch --show-current` | clean; 6d72c305…; correct branch |
| Seed | copy from Integration `.p0ref`; sha256sum / `.sha256` / `provenance.txt` | all four = `9a99f781…4b3` |
| Build | `/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln -c Debug --disable-build-servers` | Build succeeded, 0 Warning(s), 0 Error(s) |
| Core tests | `/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **299/299 passed, 0 failed** (8 new W2 cases) |
| Gate run 1 | `2d-gate.sh --base 6d72c305 --p0ref-dir <Integration .p0ref> --headless-only <ext>/gate` | exit 3, G0 FAIL: PATH `dotnet` is SDK 10.0.115 (environment, not code) |
| Gate run 2 | same, with `PATH=/home/jer/.dotnet:$PATH DOTNET_ROOT=/home/jer/.dotnet` | exit 1, G0 PASS, G1 PASS, **G2 FAIL**: Core 299/299 PASS; Save 49/50, NB1c |
| Gate run 3 (one retry) | same as run 2, launched with INT/QUIT/HUP/PIPE/XFSZ reset to default | exit 1, identical: G2 FAIL on NB1c only |
| NB1c rate | 12 direct runs of `Phase1Save.Tests.dll` at HEAD | NB1c failed 4/12; no other case failed |

NB1c message both times: `the second save carries the next generation after the first: expected 1, got 3`. This matches
the documented lock-order race over `LockSlot` (about 7.5% at T0 and HEAD alike on a quiet machine). During this session
the host load average was about 6 on 6 cores because other lanes were running, and the measured rate was about 33%.
`Phase1.Save.Tests` does not reference `SitkaP1.Core.SpeciesWave2`. The base rate was **not** re-measured here.

## Gate stage table (run 3, the final run)

| Stage | Result |
|---|---|
| G0 prerequisites | PASS |
| G1 build + P0 seed | PASS (PRE-SEEDED VERIFIED 9a99f781…) |
| G2 suites | **FAIL** (Core 299/299 PASS; Save 49/50, NB1c) |
| G3–G7 | NOT RUN (gate stops at first failure) |
| Frozen anchors 9/9 | **NOT RUN** (G7 not reached) |
| Tier B browser | NOT RUN / NOT TESTED (`--headless-only`; no Edge in WSL) |
| Network isolation | PROVEN |
| Exit code | 1 (FAIL) |

Raw gate output: external `/home/jer/Development/CCF-Multi-2D-SpeciesWave2-Evidence-20261126/{gate,gate-run2-nb1c-flake}/`;
text copies in `evidence/`.

## Evidence-class table (summary; full table in spec.md)

| Constant | Class |
|---|---|
| Tier values G/M/L/X and all 30 authored cells | Calibration |
| NeutralLegacy (1.0, 1.0) | Calibration (design invariant) |
| Pine crown quadratic | Transfer |
| Birch crown power law | Inference + Calibration |
| Alder crown power law | Empirical + Inference |
| All Ci50 defaults/bands; 0.10 m radius floor | Calibration |
| Pine/birch green density | Empirical + Inference |
| Alder green density | Transfer |
| All sortment price bands | Calibration (pine "provisional Irish" bands had no class in the packet) |

## Deviations

1. Test path: `tests/Core.Tests/SpeciesWave2/` (manager-confirmed correction of the literal `tests/Core/**`).
2. Test registration: `Program.cs` is outside the fence, so the tests self-register with `T.Add` from a
   `[ModuleInitializer]`. That runs before the top-level statements, so the cases are added before `T.RunAll()`. The
   8 cases appear first in the output.
3. Gate SDK: the gate resolves `dotnet` from PATH. `/usr/bin/dotnet` is SDK 10, so the gate needed
   `PATH=/home/jer/.dotnet:$PATH`. Neither the gate nor the code was changed.
4. Commit author: committed with `-c user.name="Claude Opus 5.5 (species-wave2 implementation sub-agent)"` (no config
   change), because the worktree's configured name is a different model.
5. Pine browse recovery is left null because the packet doesn't specify it. Alder pruning relevance is `Low`, read from
   "self-pruning" (Inference).

## Unknowns

- NB1c base rate under the same load (not measured).
- Whether G3–G7 and the 9 frozen anchors pass. They have not run on this branch. The new code adds no clock, entropy
  or RNG-salt tokens (relevant to G6) and touches no protected path (relevant to G7), but neither stage was executed.
- Per-cell moisture tiers and sortment bands still need a research review.

## Next step

Manager decision: either re-run the headless gate on a quiet host (or accept NB1c by its documented signature), then
push, or authorise a push with the gate recorded as blocked. After that, an independent review. Nothing was merged or
pushed, and nothing was wired into the annual loop, economy, saves or UI.

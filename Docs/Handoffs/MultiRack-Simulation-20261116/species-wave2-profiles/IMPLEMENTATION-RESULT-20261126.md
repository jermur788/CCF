# Species Wave 2 Profiles — Implementation Result (F1 repaired; headless gate G0–G7 PASS)

## Identity

- Provider/model: Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code.
- Role: sole implementation sub-agent, owner-authorised bounded 2D simulation lane.
- Branch: `feat/multi-2d-species-wave2-profiles-20261126-v1`
- Base: `6d72c30538b1fbe30d3cbcf083b5b368733662b8`
- Commits: `8490c218` (implementation) → `01459ff9` (blocked handoff) → `f2aceeda4e8845f65b674b2be222c65de11e3e93`
  (F1 repair, **gated head**). This handoff and its evidence are committed on top of `f2aceeda`; that commit changes
  only `Docs/Handoffs/.../species-wave2-profiles/**`. History was not amended.
- Worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-SpeciesWave2-20261126`
- Push status: pushed to `origin/feat/multi-2d-species-wave2-profiles-20261126-v1`. No PR was opened and nothing was merged.

## Verdict

**HEADLESS GATE PARTIAL (exit 5): G0–G7 PASS, 9/9 frozen anchors PASS, browser tier B skipped (`--headless-only`).**
That is the success condition for this lane. The work is not integrated, not wired and not independently reviewed.

## F1 repair (Major): structured readiness metadata

`SpeciesProfileCard`'s `ReadinessStatus ReadinessStatus` and `string ReadinessNotes` are replaced by one
`ReadinessProfile Readiness`:

```csharp
public sealed record ReadinessProfile(ReadinessStatus Status, ValueBand? AgeRangeYears, ValueBand? StemsPerHectare,
    ValueBand? BasalAreaM2PerHa, double? MinimumTopHeightM, int? IrishYieldClassPotential, string Trajectory, string Notes);
```

| Field | Scots pine | Downy birch | Common alder |
|---|---|---|---|
| Status | ReadyForInitialDesign | PartialWithAbstractions | PartialWithAbstractions |
| Age / stems / BA / min top height | 10–60 y / 100–5,000 /ha / 10–100 m²/ha / 5.0 m (Irish GROWFOR) | null | null |
| Irish YC potential | null | 8 | null |
| Trajectory | open-grown fit applied to an initial-design stand inside the GROWFOR domain | Fast pioneer flush -> moderated competition-sensitive adult | Very fast early pioneer -> sharp moderation; short-lived with late heart-rot risk |
| Notes | cite GROWFOR | crown fit is a proxy | heart rot is tied to the timber heart-rot inspection flag (50–60 y), never mortality |

A null domain means "no defensible source domain", not "unbounded". The records are metadata only and nothing reads them.

Files changed in the repair (all inside the fence):
- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/FutureSpeciesCatalog.cs`: record, card field, and the three populations.
- `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SpeciesWave2/SpeciesWave2Tests.cs`: adds W2-09. W2-01..W2-08 are unchanged; only the header comment changed (W2-01..W2-09).
- `specs/multi-2d-species-wave2-profiles-v1/spec.md`: Readiness table and AC-10.
- `specs/multi-2d-species-wave2-profiles-v1/{plan.md,tasks.md,checklists/requirements.md}`: test-range references updated to W2-09 / AC-10.

**W2-09** asserts:
- pine: `ReadyForInitialDesign`; age (10,60), stems (100,5000), BA (10,100) and min top height 5.0 as exact values; YC null; the note cites GROWFOR.
- birch: `PartialWithAbstractions`; YC 8; the trajectory contains "pioneer" and "competition"; numeric domains null.
- alder: `PartialWithAbstractions`; the trajectory contains "pioneer" and "short-lived"; numeric domains and YC null.
- All three cards are still `AuthoredOnly`.

Result: **PASS**.

## Commands and key results (F1 repair)

| Step | Command (inside worktree, via `wsl.exe bash` script with `PATH=/home/jer/.dotnet:$PATH`) | Result |
|---|---|---|
| SDK | `dotnet --version` | 8.0.134 |
| Build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | 0 Warning(s), 0 Error(s) |
| Core build | `dotnet build tests/Core.Tests --disable-build-servers` | 0 Warning(s), 0 Error(s) |
| Core tests | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **300/300 passed, 0 failed** (9 W2 cases) |

## Gate attempts on `f2aceeda`

Command: `PATH=/home/jer/.dotnet:$PATH DOTNET_ROOT=/home/jer/.dotnet Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base
6d72c30538b1fbe30d3cbcf083b5b368733662b8 --p0ref-dir <Integration>/Tools/MultiRackSitkaP1Lab/.p0ref --headless-only
/home/jer/Development/CCF-Multi-2D-SpeciesWave2-Evidence-20261126/gate-attempt-N`

| Attempt | loadavg (1 min) | Exit | Stages | Note |
|---|---|---|---|---|
| 1 | 2.76 | 2 REFUSED | G0 FAIL; G1–G7 NOT RUN | Operator error: I had overwritten the tracked `evidence/core-tests.txt` with fresh output before the gate. The file was restored from git (its content was byte-identical to `/tmp` output), and the tree was confirmed clean. No code involved. |
| 2 | 2.75 | **5 PARTIAL** | **G0–G7 PASS**; B NOT RUN | Core 300/300; Save 50/50 (NB1c PASS); ui-smoke-multiarea 363/0; ui-smoke 250 + 4 known T0 #885 (PASS-BASELINE-DRIFT); cli-safety 23/0; build-output-safety PASS (C7/C8 HOST-NA); p0-isolation PASS; S0a/S0c PASS; network isolation PROVEN |

No further attempts were needed.

- **G3: 9/9 frozen SHA-256 anchors verified.**
- G4: v2 golden verified.
- G5: 5/5 payload budgets PASS.
- G6: determinism PASS (77 files; RNG violations 0; F2 unlisted 0).
- G7: protected-path guard PASS (99 paths).

**NB1c:** it passed in attempt 2 (`PASS NB1c: a generation read and written inside the slot scope is unique per save`).
The earlier pre-repair NB1c failures (`expected 1, got 3`, 4/12 under load ≈ 6) remain recorded in
`evidence/gate-run2-*`, `evidence/gate-run3-*` and `evidence/nb1c-rate-under-load.txt`. `Phase1.Save.Tests` and the
gate were not edited.

Raw gate output is external, under `/home/jer/Development/CCF-Multi-2D-SpeciesWave2-Evidence-20261126/gate-attempt-{1,2}/`.
Text copies are in `evidence/gate-attempt2-stdout.txt`, `evidence/gate-attempt2-summary.json` and
`evidence/gate-attempt1-summary.json`. `evidence/core-tests.txt` is refreshed to the 300/300 run.

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
| Readiness domains / YC / trajectories | As supplied by packet B.1 (GROWFOR for pine); not re-researched here |

## Deviations

1. Test path: `tests/Core.Tests/SpeciesWave2/` (manager-confirmed correction of the literal `tests/Core/**`).
2. Test registration: `Program.cs` is outside the fence, so the tests self-register through a `[ModuleInitializer]`.
3. Gate SDK: the gate needed `PATH=/home/jer/.dotnet:$PATH` because `/usr/bin/dotnet` is SDK 10. The gate was not changed.
4. Commit author: commits were made with `-c user.name="Claude Opus 5.5 (species-wave2 implementation sub-agent)"`; no config was changed.
5. Pine browse recovery is null because the packet doesn't specify it. Alder pruning relevance `Low` is an Inference.
6. The pine readiness record leaves YC null, as instructed. The packet gives no pine YC.

## Unknowns

- Tier B (browser) was not run: `--headless-only`, and there is no Edge in WSL.
- The NB1c base rate under the same load was not re-measured. It passed once here, which says nothing about its rate.
- Per-cell moisture tiers, sortment bands and readiness values still need a research review. No independent review has been done.

## Next step

Independent review of `f2aceeda` (with this handoff commit on top). Nothing was merged, no PR was opened, and nothing
was wired into the annual loop, economy, saves or UI.

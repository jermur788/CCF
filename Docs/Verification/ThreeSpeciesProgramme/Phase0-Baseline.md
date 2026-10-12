# Phase 0 — baseline qualification

Programme: Scenario One full-demo overnight programme (three-species parity, then the full management loop).
Locked base and context commit: `c1564b857972ee18917e8a461c457533a54cc3f1` (verified equal to `origin/main` after `git fetch`, and to the starting HEAD of this branch).
Worker branch: `claude/stoic-mccarthy-gtzaub` (the session-designated branch; the packet names `feature/scenario1-full-demo-programme` — see "Identity discrepancy").

## What this session could and could not do

| Capability | State |
|---|---|
| Unity 6000.6.0f1 Editor / batch gates | **Not available.** No Unity install exists in this cloud container. No gate in `Tools/Verification/` was run, and no Unity hash was re-measured. |
| .NET SDK | Not preinstalled. `dotnet-sdk-8.0` (8.0.131) was installed from the Ubuntu archive at session start. |
| NuGet | Reachable. Used for the type-check harness below. |
| Windows / Player | Not applicable. |

Everything in this programme is therefore **code-only**. Every claim below is labelled with how it was established:

- **offline** — compiled and executed on the plain .NET SDK against the unchanged production source files that do not need UnityEngine;
- **type-checked** — compiled (never run) against UnityEngine 2021.3 reference assemblies plus stubs, with the constant API-drift errors removed by a committed baseline;
- **read** — established by reading source;
- **not run** — requires Unity.

## Preflight

- `git fetch origin` completed; `origin/main` = `c1564b857972ee18917e8a461c457533a54cc3f1` (matches the locked base).
- Working tree clean at start; no other worktree.
- Required documents read at this commit: `AGENTS.md`, `AgentWorkflow.md`, `decision-log.md`, `current-milestone.md`, `unity-project-overview.md`, `coordination/active-tasks.md`. `game-brief.md` and `research-index.md` were consulted for scope only.
- `Docs/Project/coordination/active-tasks.md` lists no open lock on the simulation, density, species, Scenario One UI or save subsystems; every listed task is completed and cleared. No ownership conflict.
- The canonical files MiMo is syncing (`decision-log.md`, `current-milestone.md`, `game-brief.md`, `unity-project-overview.md`, context manifests) are **not edited** by this programme. Required later canonical edits are collected in the final handoff.
- Unity version per the Overview and `ProjectSettings`: 6000.6.0f1 (not exercised here).

### Identity discrepancy (recorded, not resolved)

The packet asks for branch `feature/scenario1-full-demo-programme` in a new worktree. This session was started on `claude/stoic-mccarthy-gtzaub`, a clean checkout at exactly the locked base, and the session's instructions designate that branch for all commits. Work was done there. The two names point at the same base; renaming is a one-line `git branch -m` for whoever integrates.

## Audit findings A–F, verified against source before any edit

| # | Packet finding | Result | Evidence (at `c1564b8`) |
|---|---|---|---|
| A | Adult Sitka height is species-gated | **Confirmed** | `ForestEcologyController.GrowAdults`: `if (UsesSiteClassGrowth && SameSpeciesId(treeSpecies, SitkaGrowthModel.SpeciesId)) heightGrowth = SitkaGrowthModel.NextHeight(...)` |
| B | Adult density / self-thinning is not species-gated | **Confirmed** | `ApplyAdultDensityMortality` collects every living tree; `StandRelativeDensity` pools stems/ha and Dq over all species and calls `SitkaGrowthModel.RelativeDensity`; the same pressure, hazard and maximum-line boundary then apply to every species |
| C | Oak/Beech already differ in several respects | **Confirmed** | species assets: DBH growth 0.4 cm/yr (Sitka 1.2), Ci50 3 (Sitka 5), maturity 40/60 (Sitka 20/30), seed dispersal scale 4–, power-law Beech crown, distinct juvenile light/establishment responses, browse palatability, promotion light thresholds |
| D | Oak/Beech height under growth model 1 is the legacy law and shallower than Sitka | **Confirmed** (see Phase 2/3 for whether "shallower" is wrong) | both assets: `potentialHeightGrowthMPerYear: 0.4`, `maxHeightM: 40`; law `dH = 0.4 × site × (1 − H/40)` |
| E | Site productivity is not species-sensitive | **Confirmed** | `GetSiteProductivity(Vector3)` returns one per-cell scalar (`cells[i].SiteProductivity`); no species argument; `GrowAdults` multiplies every species' DBH and height potential by it |
| F | Oak/Beech verification does not qualify growth model 1 | **Confirmed** | `CCFOakVerification`, `CCFBeechVerification` and `CCFOakPlayerPlantingVerification` pin `ecology.GrowthModelVersion = GrowthModel.Legacy` (Beech: "legacy contract") |

No finding was contradicted. No stop condition from §26 applies at this point.

## Verification entry points found

- Regression runner: `Tools/Verification/UnderstoreyRecruitment/run_regression.py` lists the 24 gates (`SitkaGrowthModelVerification` … `Model2PedagogyVerification`). Each is an `-executeMethod X.Begin` Unity batch/interactive process of 1–3 minutes. The latest recorded run is `Docs/Verification/Enlarged80Calibration/regression-results.json` (24/24 PASS at `fa9f66d`, explicit Editor-only Legacy40 geometry via `CCF_STAND_GEOMETRY=0`).
- Completion gate honours `CCF_RNG_MODEL`, `CCF_REGEN_MODEL`, `CCF_GROWTH_MODEL`: `CCF_GROWTH_MODEL=2` will run it under the new model.
- Offline-logic precedent: `Tools/Verification/ScenarioOneCompletion/P2/OfflineLogic/` (needs a Unity-bundled .NET). The new offline check in this programme needs only the plain SDK because it covers files that do not use UnityEngine.

## Baseline anchors

**Not re-measured in this session** (no Unity). Taken from committed evidence and from the hash constants asserted in the harness sources.

Pure-Sitka fixtures (the lifecycle fixture is 68 Sitka trees; no Oak/Beech seed source exists in it, so no non-Sitka adult can appear):

| Anchor | Value | Source |
|---|---|---|
| Neutral lifecycle, legacy (RNG0/regen0/growth0) | `BFC55473C1506067` | `ScenarioOneInteractionVerification`, harnesses |
| Scenario One lifecycle (browse 0.2), legacy | `3485B6630C9EA448` | same |
| Neutral / normal, RNG1 regen1 growth0 | `962846D2F517B293` / `FBB8F470D85FF815` | `SitkaGrowthModelVerification` |
| Neutral / normal, growth 1 (pure-Sitka Growth Model 1) | `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` | regression-results.json |
| Reference Future v1 Y100 / continuation | `7AD177B3CC2F73C7` / `9CDF21A541C5968D` | `ScenarioReferenceVerification` |
| Reference schedule / start / Y20 / Y50 | `56C8B99FA1E8DDD1` / `A564039D9B7CE31D` / `F7DF7DAB53B6FD32` / `D5E75D6D21D631AC` | same |

Fixtures that can contain non-Sitka adults (Oak/Beech are planted by the completion path and recruit naturally), and are therefore the ones a mixed-density change could move:

| Anchor | Value | Note |
|---|---|---|
| Model2 start / year 1 / completion | `FA855239CDDA32D8` / `02334804F65C0234` / `702766DECE591E21` | Legacy40; completion plants Oak and Beech |
| Enlarged80 start / year 3 | `04A78188A6F8E6F9` / `081DB58CE3034648` | Enlarged80 |
| Legacy40 start world, P2, P3 | `EBE7A228F630AE41`, `F58FB0B1A421D28B`, `F7C2FC966816BBAD` | |
| Storm anchors | `8333BAA4126E8A09`, `84796FAEF211823C`, `52037B943B62DD39` | |

All of the packet's listed hashes appear in the repository's own records, including `04A78188A6F8E6F9`, `081DB58CE3034648` and `702766DECE591E21`; none is unaccounted for. The authoritative list is the one the harness sources assert, tabulated above.

## Compile baseline (type-check harness)

`Tools/Verification/CloudTypeCheck/run_typecheck.py` compiles the runtime sources (and, in `tools` mode, the `Tools/Verification` harnesses with `UNITY_EDITOR` defined) against NuGet `UnityEngine.Modules` 2021.3.33, with minimal stubs for the InputSystem and UnityEditor surfaces in use.

- `prod` baseline at `c1564b8`: **7** distinct errors. All are Unity 2021.3 vs 6000.x API drift in files untouched by this programme (`FindObjectsByType()` overload, `Painter2D`, `Object.GetEntityId`, `Mouse.WarpCursorPosition`).
- `tools` baseline: **43** distinct errors. All are missing UnityEditor stub members in unrelated visual-review harnesses, plus the same 7.
- Both baselines are committed (`baseline-prod.txt`, `baseline-tools.txt`). The meaningful signal is a **diff against them**: any new error is a real regression in the edited code against this approximation.

This is a code-only safety net. It is not Unity's compiler and does not replace the Unity compile gate.

## Phase 0 result

Environment qualified for **code-only** work; baseline recorded from committed evidence; no baseline hash could be re-measured. All Unity gates remain required for every behavioural claim in the later phases.

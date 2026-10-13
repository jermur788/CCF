# Phase 10 — complete-scenario qualification: what is established, and the Unity runbook that closes the rest

**Status: CLOUD / NOT UNITY-VERIFIED.** No Unity install existed in this session, so no gate in `Tools/Verification/` was executed, no scene was opened, nothing was rendered, and no frame time was measured. This phase therefore does **not** qualify the complete scenario. It records exactly what is and is not established, and gives the ordered runbook, and a runner, for the session that can run Unity.

## 1. Evidence tiers (read this before any table)

| Tier | Meaning | Where it applies here |
|---|---|---|
| **Executed offline** | Production C# files that do not need `UnityEngine` were compiled with the plain .NET SDK and run | Mixed-density model (35 checks), timber allocator + economy + price book (71), history derivation + wording against the real persisted record classes through a tiny `UnityEngine` shim (70) |
| **Type-checked** | Compiled against community UnityEngine 2021.3 reference assemblies plus two stubs; new errors diffed against committed baselines | All production code and all `Tools/Verification` gates (`TYPECHECK_CLEAN`: 7 and 43 constant API-drift errors, none new). Not Unity's compiler; not the 6000.6 API |
| **Unity-required, not run** | Needs the Editor: scene, controller, UI Toolkit, serialisation, timing, rendering | Everything in section 3 |

Offline execution proves the logic of the pure layers on the inputs the checks chose. It does not prove the Unity wiring, the persisted-save round trip of a real session, the UI layout or performance.

## 2. Technical completeness matrix

| Phase | Deliverable | Code | Offline-executed | Type-checked | Unity gate written | Unity gate run |
|---|---|---|---|---|---|---|
| 0 | Baseline, type-check harness | yes | harness itself runs | n/a | n/a | no |
| 1 | Species-aware mixed density, growth model 2 (reworked, see `Phase1-DensityRework.md`) | yes | 91 checks + 2 isolated compiles | yes | `MixedDensityVerification` | **no** |
| 2 | Oak adult parity | **stopped (evidence)** | — | — | `BroadleafAdultGrowthVerification` (pins the *current* contract) | **no** |
| 3 | Beech adult parity | **stopped (evidence)** | — | — | same | **no** |
| 4 | Species × site, canopy/light | **stopped (design decision)** | — | — | none | — |
| 5 | Three-species qualification | runbook only | — | — | combination of the above | **no** |
| 6 | Oak/Beech timber and economy (model-2 gated) | yes, minus form grade / formative shaping | 71 checks, Sitka identical to the unmodified base by golden fingerprint | yes | `BroadleafTimberEconomyVerification` | **no** |
| 7–9 | Derived management history, drill-down, first-intervention prompt, stale-order surfacing, completion presentation | yes | 70 checks | yes | `ScenarioHistoryVerification` | **no** |
| 10 | This record, runner | `run_unity_gates.py` | runner mechanics exercised against a fake editor (section 5) | n/a | n/a | no |

The three stopped lines are deliberate: they were stopped by the packet's own stop conditions (no evidence to replace the Oak/Beech height and DBH laws; no site identity in data or documents; no persisted state from which a stem-form grade could be derived), not by a failure. Details are in `Phase2to5-BroadleafParity.md` and `Phase6-BroadleafTimber.md`.

## 3. What only Unity can establish (the open gates)

| # | Claim not yet established | How it closes |
|---|---|---|
| U1 | Everything compiles and imports on Unity 6000.6.0f1 | Step 1 |
| U2 | **No existing anchor moved** (growth models 0/1, all pure-Sitka fixtures, Reference Future v1) | Step 2: the existing 24-gate regression, unchanged defaults |
| U3 | Mixed density behaves as specified in a running scene; pure Sitka under model 2 equals model 1 on tree state; model 3 and v16 saves handled | `MixedDensityVerification` |
| U4 | Oak/Beech adult growth under models 0, 1, 2 matches the documented current contract and is deterministic across save/load | `BroadleafAdultGrowthVerification` |
| U5 | Oak/Beech quotes, sales and Work Plan/inspection text match the ladders, prices and quality rules; Sitka quotes unchanged | `BroadleafTimberEconomyVerification` |
| U6 | Three real cycles with idle years produce the expected history, stale order kept and revalidated, no recurring gate, history identical after save/load, completion summary derivable | `ScenarioHistoryVerification` |
| U7 | The Scenario One completion under model 2 (outcome year, minimum cash) | Step 4 (a measurement, see below) |
| U8 | New cards and text fit and read well at 1280×720 | Step 5 (by eye) |
| U9 | Frame time / hitch with the new reads (history rebuilt on refresh; HUD count only when the event list changes) | Step 6 |

## 4. Ordered Unity runbook

Work in a fresh checkout of this branch with Unity closed. Each worktree has its own `Library`; do not share one. Record every result under `Build/` (git-ignored) and copy only reviewed summaries into `Docs/Verification/`.

### Step 1 — compile and import

```text
<UNITY> -batchmode -nographics -quit -projectPath <checkout> -logFile <log>
```

Expect zero `error CS`, no new importer exceptions, and **no new or changed `.meta` files** in `git status` (the two modified `.asset` files and every new `.cs` already carry a `.meta`).

### Step 2 — existing regression, unchanged defaults (the anchor gate)

```text
python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py
```

That runner hard-codes a Unity path and a `systemd-run` scope for the author's workstation; edit the two lines for your machine and do not commit the edit. Expect all 24 gates `PASS` with the anchors listed in `Phase0-Baseline.md` (for example Reference Future v1 `7AD177B3CC2F73C7` / `9CDF21A541C5968D`, the accepted regeneration-model-2 / growth-model-1 completion in its v18 layout, `702766DECE591E21`).

**Stop here if any anchor differs.** New games still start on growth model 1 (`ScenarioOneManager.NewGameGrowthModel`), the Sitka route is untouched, and history/UI changes read state only, so a moved anchor means a defect in this branch, not a rebaseline.

### Step 3 — the four new gates

```text
python3 Tools/Verification/ThreeSpecies/run_unity_gates.py --unity <UNITY>
```

Runs, in order, `MixedDensityVerification`, `BroadleafAdultGrowthVerification`, `BroadleafTimberEconomyVerification`, `ScenarioHistoryVerification` (the last with a graphical Editor). Expected terminal markers: `MIXED_DENSITY_VERIFY_PASS`, `BROADLEAF_GROWTH_VERIFY_PASS`, `BROADLEAF_TIMBER_VERIFY_PASS`, `SCENARIO_HISTORY_VERIFY_PASS`. The runner stages each copy under `Assets/ForestPrototype/`, always removes the copy and its `.meta`, and fails a gate if anything else appears in `git status`. Read it before first use (section 5). `--dry-run` prints the commands without staging or running anything.

A gate **failure is information, not an instruction to loosen the gate.** The gates assert the contracts the packet and supplements state; if one fails, decide first whether the code or the assertion is wrong, and record which.

### Step 4 — model-2 completion measurement (not a pass/fail anchor)

```text
CCF_GROWTH_MODEL=2 CCF_STAND_GEOMETRY=0 python3 Tools/Verification/ThreeSpecies/run_unity_gates.py --unity <UNITY> --gate ScenarioOneCompletionVerification
CCF_GROWTH_MODEL=2 CCF_STAND_GEOMETRY=1 python3 Tools/Verification/ThreeSpecies/run_unity_gates.py --unity <UNITY> --gate ScenarioOneCompletionVerification --out Build/ThreeSpecies/enlarged80
```

Record: does the gate still pass (outcome `Completed` at the minimum year, cash never negative), the minimum cash, the year-30 snapshot line, and `SCENARIO_ONE_COMPLETION_HASH`.

Reading the hash correctly: the saved JSON contains `"growthModel":N`, so a model-2 `WorldHash` differs from model 1's **by that field alone** even if every tree is identical. A different hash is therefore not evidence of divergence. The gate's own exact-layout assertion (`702766DECE591E21`) applies only when `GrowthModelVersion == 1` and is skipped under model 2. The like-for-like comparison is the tree-state hash inside `MixedDensityVerification` (`PURE_SITKA_MODEL2_EQUALS_MODEL1`), which ignores the model number. To localise a first divergence in a mixed stand, diff the per-year lines of the model-1 and model-2 logs; no tool for that was written.

Predicted (a reading of the code, not a measurement): the Scenario One completion changes only if a planted or recruited Oak/Beech tree is promoted to an adult and the weighted density crosses a threshold the pooled density did not.

### Step 5 — by-eye review (needed because nothing has been seen)

Use the interactive Editor on `Assets/Scenes/ForestTest.unity`. To see the broadleaf market in a new game, temporarily set `ScenarioOneManager.NewGameGrowthModel = GrowthModel.SpeciesAware` (line ~472) **locally and do not commit it**; the model-2 adoption is a Manager decision.

1. Start a new game, plan and approve thinning of at least three Sitka, buy and plant one sheltered Oak, advance one year.
   *Expect:* the Annual Review opens with a "Your first intervention" card and a Management history block (Intervention 1 · year 1: felled/planted counts, cost, income, net); HUD shows "· 1 intervention" (and "· 2 interventions" after the next one).
2. Acknowledge once, advance two more years without work.
   *Expect:* no further acknowledgement is demanded; the history card remains; the Earlier year / Later year buttons step through years and show only snapshot lines for past years.
3. Open the Work Plan after leaving an order pending for two or more years.
   *Expect:* an "Orders needing attention" card names the order, the reason and the age; nothing is removed or approved.
4. Inspect a mature Oak and a mature Beech in the Work Plan and tree inspection.
   *Expect:* readable product names (not enum identifiers), Oak shows veneer only if the stem was pruned and defect-free, Beech shows second planking/firewood only, and the unmarketed text no longer says "broadleaf".
5. Cause or force completion (the gate does this by reflection; by hand you must meet the objectives).
   *Expect:* a completion summary at the actual completion year, labelled as not a score, with the Century Review separate.

Record layout problems, truncated text, scroll issues and anything unclear. None of this has been looked at.

### Step 6 — performance (not measured)

Compare against the Phase 0 values for the same scene: frame time while walking, and the time to open the Annual Review and Work Plan at year 1 and after several interventions. The only new per-refresh cost is `ScenarioInterventionHistory.Build` over a few thousand records, rebuilt on each read by design (a cache could go stale across loads). No cut to stocking or content is justified by anything measured here.

## 5. The runner, and how much of it was tested

`Tools/Verification/ThreeSpecies/run_unity_gates.py` was exercised in this session against a **fake editor script** that only mimics the command-line contract (`-logFile`, `-executeMethod`, exit code, a generated `.meta`). That checked: the dry run; all four gates passing in order with correct modes and isolated config; failure on a `_FAIL` marker, on a missing terminal marker, on `error CS` in the log, and on a stray file appearing in the working tree; refusal to start over an existing staged copy (without deleting it); cleanup of the staged copy and its `.meta` after a failure and after a missing executable; refusal to run with no Unity path; and `CCF_GROWTH_MODEL`/`CCF_STAND_GEOMETRY` passthrough. It has never driven a real Unity. Assume it can fail in ways the fake could not, in particular: batch-mode exit codes, whether `Application.Quit` is reached, an Editor that stays open after the gate, and licence prompts. It records the head, branch, source hash, geometry, growth model and any unexpected tree change in each result JSON.

## 6. Known gaps that remain whatever Unity says

- Oak and Beech adult height/DBH laws are the previously shipped ones (zero-competition DBH ≤37.8 cm at age 112; identical parameters for both species). Replacing them needs evidence that could not be retrieved here.
- Premium Oak classes are close to unreachable under that DBH calibration; this is a finding for the Manager, not something to tune around.
- Species × site and canopy/light response is not implemented (Phase 4 stop); site productivity remains one uniform, unsaved scalar.
- Stem-form grade (1–4) and formative-shaping are not implemented; Oak quality comes only from pruned sections and recorded defects.
- The broadleaf price basis (roadside versus stumpage) is not stated in the supplement; the prices are [C] calibration inside the evidence ranges.
- Mortality cause is not recoverable from persisted records, so the history says "not felled", not "self-thinning".
- The `fell.cost` teaching copy still says no broadleaf market is configured; untrue under model 2, to be updated when model 2 is adopted.
- No performance or render measurement of any kind.

## 7. Canonical updates required (not made)

- `current-milestone.md`: the scope status of Scenario One full demo; the open Unity gates U1–U9.
- `unity-project-overview.md`: growth model 2 (`GrowthModel.SpeciesAware`, mixed density via ordinal growing-space-demand classes, broadleaf market gated on model ≥ 2), `MixedDensityModel`, the Oak/Beech assortment ladders and price book, `ScenarioInterventionHistory`, the new Verification folders and `run_unity_gates.py`.
- `decision-log.md`: acceptance of D-059…D-062 as implemented (with the stops above), and a decision on adopting growth model 2 as the new-game default after Steps 1–4 pass.

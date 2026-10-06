# Residual-stand evaluation prototype (Workstreams D5–D8)

**Status:** disposable, read-only research harness, plus results. **Not production code.** Lives under `Tools/Verification/ScenarioOnePedagogy/`.

## 1. Harness

`Tools/Verification/ScenarioOnePedagogy/ResidualStandEvaluation.cs`

What it does (D5):

1. Opens `ForestTest`, enters play mode, and requires a fresh Scenario One Year 0 (isolated `XDG_CONFIG_HOME`, so no save is auto-loaded).
2. Captures the world once (`ForestSaveController.CaptureData`) and keeps it as JSON.
3. Chooses 16 Crop Trees by the same proxy as `ScenarioOneCompletionVerification` (largest DBH per 10 m block). This is a *vigour proxy*: the game has no stem-form state.
4. Builds six marking plans (T0–T5) from authoritative state only.
5. For each plan, in the restored in-memory world:
   - marks the trees and obtains the **production Work Plan harvest quote**;
   - restores, then quotes the retained stand (notional value);
   - restores, then computes release, spatial, light, stability and seed metrics with **production formulas** (`HegyiTerm`, canopy gap product, growth response, wind formula).
6. Applies each plan through the **real work cycle** (mark → Work Plan → approve → `AdvanceYear`) and records Years 1, 5, 10 and 20.
7. Repeats T2 to Year 20 twice, in-process, and compares world hashes.
8. Builds a Year-10 untreated context and evaluates T0/T2/T5 there (seed-bearing stand).
9. Writes JSON/CSV, logs a SHA-256 of all outputs, restores the original world and verifies the restore by hash.

**It never calls `Save()`.** Every restore uses a fresh `JsonUtility.FromJson` copy followed by two frames (see §4).

### Treatments (D6)

| Id | Plan | Removed |
|---|---|---|
| T0 | no treatment | 0 |
| T1 | novice clean-up: every "crowded" non-crop stem in the smallest DBH quartile | 84 |
| T2 | Crop-Tree release: top 2 competitors (largest Hegyi term) per Crop Tree | 30 |
| T3 | heavy opening: top 6 per Crop Tree | 87 |
| T4 | conservative: top 1 per Crop Tree | 15 |
| T5 | concentrated gap: non-crop stems nearest one interior point until removed volume ≥ T2's | 48 |

### How to run

```bash
cp Tools/Verification/ScenarioOnePedagogy/ResidualStandEvaluation.cs Assets/ForestPrototype/
XDG_CONFIG_HOME=<isolated dir with unity3d/Unity licence symlink> CCF_ACCEPTANCE_OUTPUT=<out dir> \
  <Unity 6000.6.0f1> -batchmode -nographics -projectPath <worktree> -executeMethod ResidualStandEvaluation.Begin -logFile <log>
rm Assets/ForestPrototype/ResidualStandEvaluation.cs Assets/ForestPrototype/ResidualStandEvaluation.cs.meta
```

Markers: `RESIDUAL_STAND_IMMEDIATE`, `RESIDUAL_STAND_LONG`, `RESIDUAL_STAND_DETERMINISM`, `RESIDUAL_STAND_OUTPUT_SHA256`, `RESIDUAL_STAND_WORLD_RESTORED`, `RESIDUAL_STAND_EVALUATION_PASS`.

Outputs (copied to `Evidence/`, checksums in `Evidence/SHA256SUMS.txt`):

| File | Content |
|---|---|
| `residual-stand.json` | everything, structured |
| `residual-stand-immediate.csv` | one row per plan and context (Y0, Y10) |
| `residual-stand-longterm.csv` | plan × Year 1/5/10/20 |
| `residual-stand-croptrees.csv` | per Crop Tree, per plan: competition and growth before/after |
| `residual-stand-neighbours.csv` | every neighbour within 8 m of every Crop Tree: Hegyi term, share, rank, which plans remove it |
| `residual-stand-trees.csv` | full tree list (marteloscope-style) at Y0 and Y10 |
| `residual-stand-cells.csv` | per-cell light, canopy, stems, basal area and regeneration at Y0 and Y10 |

## 2. Runs and determinism

All runs: Unity 6000.6.0f1, `-batchmode -nographics`, worktree `/home/jer/CCF-pedagogy` at `3e4ee40` with the harness staged, isolated config, `nice` + 7 GiB memory cap. RNG model 1, regeneration model 1, browse 0.20.

| Run | Harness version | Result | Output SHA-256 | T2 Year-20 world hash (×2 in-process) |
|---|---|---|---|---|
| rs-a | first draft (in-frame restore) | FAIL in determinism block (see §4) | — | `B23BEEC2C4E2A0C1` ≠ `3B594671A24266A6` |
| rs-b | restore fix | PASS | `923E0011…A458258` | `D8802246D311A2F8` = `D8802246D311A2F8` |
| rs-c | same as rs-b, separate process | PASS | `923E0011…A458258` (identical) | identical |
| rs-d | final (adds tree and cell tables) | PASS | `30BB936F…5C50F36E` | `D8802246D311A2F8` ×2 |
| rs-e | same as rs-d, separate process | PASS | `30BB936F…5C50F36E` — **all 7 files byte-identical** | identical |

The long-term rows of rs-a and rs-b are identical. Only rs-a's in-frame determinism check was affected.

## 3. Validation (D7)

| Claim to demonstrate | Result | Evidence |
|---|---|---|
| Equal harvested volume can leave different forests | **Demonstrated** | T2 vs T5: 5.19 vs 5.22 m³ removed; 23 vs 11 cells touched; largest opening 3 vs 9 cells; 16 vs 7 Crop Trees released ≥10 %; mean light after 0.060 vs 0.130 |
| Removing many small trees can release Crop Trees less than removing a few real competitors | **Demonstrated** | T1 (84 small stems): Crop-Tree competition −16.6 %. T2 (30 competitors): −19.7 %. T4 (15 competitors): release per m² removed 4.25 vs T1's 2.57 |
| Retained capital and immediate income trade off | **Demonstrated, with an economic caveat** | Higher income goes with less capital left (T3: €629 gross, 28.0 m²/ha left; T4: €134, 38.6 m²/ha). But at Year 0 every plan has a negative net (−€1,871 to −€2,366), because the €2,500 contractor minimum exceeds the whole stand's notional value (€1,702) |
| Spatial pattern matters | **Demonstrated** | as row 1; plus Year-1 regenerating cells 23 (T5) vs 17 (T2), converging to 40 by Year 20 |
| Deterministic | **Demonstrated** | identical outputs across two processes; identical in-process repeat |
| Read-only | **Demonstrated** | world hash restored (`RESIDUAL_STAND_WORLD_RESTORED`); no Save call; isolated profile; staged script and `.meta` removed after each run |

## 4. Technical finding: restore timing

The first harness version restored the world with `LoadData` and ran 20 years **in the same frame**. The two repeats gave different world hashes. Cause, from inspection:

- `ForestSaveController.LoadData` deactivates trees that are not in the save and calls `Destroy`, which completes at frame end.
- `ForestSaveController.CaptureData` enumerates `ForestTree` with `FindObjectsInactive.Include`, so it captured the previous run's inactive recruits.

With a fresh JSON copy and two frames after each restore — the same pattern as `ScenarioOneCompletionVerification` — the repeats are identical. **This is not a production defect** (normal play always crosses frames), but it is a **hard requirement for any production practice/marteloscope reset** (`LongTermTrainingFlow.md` §4).

## 5. Findings that affect teaching (summary)

1. The HUD's stand-wide "growth +x %" forecast can rank a clean-up thinning above a Crop-Tree release (T1 +12.2 % vs T2 +6.7 %), the reverse of the Crop-Tree result. **Packet 2.**
2. The mean Crop-Tree effect hides concentration (T5). **Report the released count.**
3. The wind label is "high" for all trees; the competition label is "crowded" for 328 of 336. **Labels need recalibration** before they can teach discrimination (presentation; after Sol's height work).
4. Every early thinning loses money under the current minimum on a 0.16 ha property. **Economy decision**, not a pedagogy fix.
5. In the untreated stand, regeneration starts in Year 1 (14 cells) and reaches 37 by Year 20, so the "regeneration ≥3 cells" objective is met by inaction.
6. Without adult mortality, basal area rises to about 93 m²/ha by Year 20 untreated. Re-run this harness after Sol's integration; the comparative conclusions should be re-checked, not assumed.

## 6. Re-baseline instruction

After Sol's growth/mortality integration (or any ecology change), run the harness twice in separate processes and compare `RESIDUAL_STAND_OUTPUT_SHA256`. Expect *different* numbers from this record. The **qualitative validations** in §3 are the regression criteria, not the exact values.

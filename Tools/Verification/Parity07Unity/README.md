# PARITY-07 rendered qualification harness (disposable)

**Status: authored and statically inspected only. NOT UNITY-VERIFIED. No rendered pass is claimed.** The evidence comes from
the later run on U1 under the machine-wide Unity lock (parent #91 / T3 #106). Base for the copy under test: `478bd84`.

Files here (nothing is staged or built unless the runner is run):

| File | Purpose |
|---|---|
| `Parity07UnityRender.cs` | Editor-only, disposable harness. Copied to `Assets/ForestPrototype/` for the run only. |
| `run_gate.py` | Runner. Preflight, private display, isolated config, Unity launch, cleanup, side-effect restore. |
| `HANDOFF-STUB.md` | Template for the result handoff. Fill in after the run. |

## Run command (caller already holds the lock for the exact clean U1 lane)

```bash
cd /media/jer/ZX20/CCF-U1
python3 -I -B /media/jer/Videos/CCF-Parity07Unity/Tools/Verification/Parity07Unity/run_gate.py \
  --project /media/jer/ZX20/CCF-U1 --expected-head <full-sha-of-U1-HEAD> --lock-id <held-lock-id> \
  --out /media/jer/Files/CCF/Verification/Parity07Unity-<UTC-stamp>
```

Add `--preflight-only` first to validate ownership with no staging or launch. Optional: `--task-id <id>` (also requires that task id
in the lock metadata), `--sizes`, `--display`, `--screen`, `--timeout`, `--grace`. The runner does **not** take or release the
lock. The harness is copied from next to the script, so U1 only needs to be at the commit under test; the harness does not have to
be committed in U1.

The runner refuses (exit 2, nothing written): wrong or dirty lane HEAD, a lock that is not held with this `--lock-id`
(or names another lane/head/task), any running Unity Editor, an existing staged harness/`.meta`, an occupied display, a
missing licence, and an evidence folder that is not a fresh folder under `/media/jer/Files/CCF/Verification/`.

## What it does

1. Stages `Parity07UnityRender.cs`; starts private Xephyr and openbox; copies the existing licence into the isolated
   `XDG_CONFIG_HOME` (`<out>/config`), so the player's `forest-save.json` is never read or written.
2. Runs Unity 6000.6.0f1 `-executeMethod Parity07Unity.Begin`: opens `Assets/Scenes/ForestTest.unity`, enters Play mode, uses the
   real startup menu **Start New Scenario**, waits for the live `ScenarioOneUiRoot`, closes first-use help.
3. Drives the real UI at **1280x720 and 1920x1080** (`CCF_UI_SIZES`), reads text back from the rendered UI Toolkit labels, writes a
   PNG and a text dump per state, and audits each panel (see below). Evidence goes to `<out>/evidence/`.
4. Always removes the harness and `.meta` and the copied licence, restores the known Editor rewrites (`.vscode/settings.json`,
   Section Five materials) by exact path (anything else is reported, never discarded), and checks the lane is clean.
5. If the Editor hangs after writing its verdict and `Cleanup mono`, reaps only its own process group after `--grace` seconds
   (`reaper.txt`).

Tokens: `PARITY07_CHECK_PASS|FAIL`, `PARITY07_UNITY_HARNESS_PASS|FAIL`. Result: `process-result.json`,
`evidence/parity07-result.json`, `summary-lines.txt`.

## States captured and asserted (each at both sizes)

Fixture trees are spawned in memory (`ForestTreeSpawner`) and pruned with the production `ForestTree.TryPrune`; the world is restored
from an in-memory snapshot and the world hash is compared at the end (`WORLD_RESTORED_AFTER_FIXTURES`). Expected copy is
written as literals in the harness, not read from `ScenarioOneUiFacts`.

| # | Capture name | Asserts |
|---|---|---|
| 1 | `oak-unpruned`, `oak-unpruned-large` | Felled-estimate panel: quality-limit copy (size and pruning history; fork/defect not modelled), "Not pruned: ... unavailable", size thresholds (45 cm / 30 cm), premium-Oak note present only when no veneer/beam product |
| 2 | `oak-pruned-below-thresholds`, `oak-pruned-above-thresholds` | "Pruned to 3.5 m: ... can become eligible ... once they are large enough"; premium-Oak note and threshold line **absent** when a veneer/beam product is in the estimate, present when not. The large fixture is searched (DBH 50-90) until it reaches veneer/beam; if none does, `FIXTURE_LARGE_PRUNED_OAK_REACHES_VENEER_OR_BEAM` fails |
| 3 | `beech`, `sitka` | Beech: quality limit + "no veneer, beam or fencing class", no Oak notes. Sitka: no broadleaf quality/premium copy |
| 4 | `detail-oak-storms-off/on`, `detail-beech-storms-off/on`, `detail-sitka-storms-off/on`, `detail-oak-old-growth-model` | **[V] More detail**: `SharedSpeciesRulesNote` (site/wind not calibrated per species; canopy shading still reflects species-specific crown size and crown adjustment) followed by exactly the matching storm tail ("Storm events are off in this forest." or the simplified shared model wording). Sitka: shared broadleaf note absent (production only shows it for non-Sitka). Old growth model (no broadleaf market): "No timber market is configured for this species" and no premium note |
| 5 | `workplan-harvest-broadleaf-market`, `workplan-pruning-broadleaf-market`, plus `-old-growth-model` | Harvest card: "If this visit includes Oak or Beech: quality limit + premium note" with the broadleaf market, absent without it. Pruning card: "Where Oak timber is marketed, the recorded pruned section ...; Beech has no such class", no unqualified "For Oak ..." claim. The crop Oak pruning order is created by the production `BatchPruneCropTrees` |
| 6 | `learning-pruning-prune-read` | Objectives, "All learning topics", **6. Pruning** expanded: "where Oak timber is marketed ..." topic text; `prune.read` with the market-qualified two-lift wording and the unchanged Sitka three-lift distinction |

All text from every state is also checked against the prohibited-claim patterns (straightness/form claims, guaranteed premium,
the stale "no timber-price premium" wording). Every panel is checked for: visible; horizontal containment in the root; vertical
containment for fixed panels (the inspection cards cannot scroll because the cursor is locked); no label outside its panel; no
wrapped label whose measured height exceeds its box; no cut one-line label; PNG size equals the target and is not blank; no console
errors during the run.

## Known limits (to state in the result handoff)

- Callback/API driven through the real UI state and renderer. No OS-native key/mouse input, so `[V]` is exercised through
  `TreeInspectionView.ToggleDetail()` (the method the key calls), not a key press.
- "Where Oak timber is marketed" wording exists in the Work Plan Pruning card and the Learning topic, not in the inspection card.
  Inspection pruning copy is asserted separately (pruned/not pruned lines and the premium note).
- The market and storm models are set directly on the in-memory ecology controller. "Old save" is emulated by
  `GrowthModel.SiteClassDensity`; no old save file is loaded.
- Pruned state uses only the first 3.5 m lift. The second lift (6.0 m) copy is read in the Learning text, not driven.
- Not covered: a Windows build, the player's real save, and any OS-level pointer behaviour.

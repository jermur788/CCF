# Scenario One 80 m integration — post-integration verification

Integrator task, 2026-10-10. Approved source `task/scenario-one-80m-stand` at `a309f99e0f2c6a770044604bbf88ee2e2471a5b5`, integrated as a pure
fast-forward of `origin/main` `11c3596072a0422d5426291e370129d209c56342`:

`11c3596 → 9f49e2e (Phase-1 audit) → b024633 (80A geometry/save foundation) → a309f99 (80B Enlarged80)`

Clean integration worktree `/home/jer/CCF-integration-80m`, branch `integration/scenario-one-80m`, Unity 6000.6.0f1, a fresh Library (imported
from scratch). `/home/jer/CCF-main` was not touched. Implementation was pushed to `origin/main` only after every gate below was green or had a
pre-accepted disposition. Raw logs stay in ignored `Build/`; the curated results and a SHA-256 index are in `Evidence/`.

## Pre-integration

Live `origin/main` and the source head were exactly as expected; first-parent chain exactly the three commits above; source 3 ahead / 0 behind;
merge base `11c3596`; pure fast-forward. The serialized-asset audit of `main..a309f99` found **no** scene, prefab, material, `.asset`, `Packages` or
`ProjectSettings` change; the only `.meta` additions are the three new C# scripts (`StandGeometryModel`, `ForestSaveValidation.Geometry`,
`StandWorldBoundary`). After the local fast-forward the integration tree was identical to the source tree (`9e0ef41bed98…`).

## Results

| Step | Result |
|---|---|
| A. Compile / import | 0 C# errors, 0 compiler-failure lines, 0 import errors, clean exit, 570 s (fresh Library) |
| B. `StandGeometryVerification` | PASS, 10 check groups; v20, Legacy40 = 0, Enlarged80 = 1, ≤ v19 → Legacy40, explicit v20 field, unknown/forged/missing/non-integer geometry rejected before world mutation, cell checks against the save's own geometry, geometry applied before restore, geometry-aware current hash, Legacy40-only compatibility hash, Enlarged80 cannot take a Legacy40 hash; Legacy40 start world `EBE7A228F630AE41` |
| C. `Enlarged80Verification` | PASS, 12 check groups; 80 × 80 m, 0.64 ha, 16 × 16 = 256 cells, 1,344 trees, 2,100 stems/ha, 336/336 core IDs and exact positions, deterministic unique outer IDs, no construction frames or solid site colliders. **START `04A78188A6F8E6F9`, YEAR 3 `081DB58CE3034648` exact** |
| D. Save / load / Reference | v19 → Legacy40, v19 re-save → v20 + geometry 0, v20 Legacy40 round trip, v20 Enlarged80 restore and deterministic continuation, Reference preview from Enlarged80 (all within B/C). Additional integrator check `IntegrationPreviewCheck` (PASS on its second run): Enlarged80 → Legacy40 → Enlarged80 restores ground, ridges, player position, body rotation, look pitch, camera, waypoint cell/target/marker, and the 256-cell map |
| E. Existing Scenario One gates | SessionMenu PASS in Enlarged80 and under `CCF_STAND_GEOMETRY=0`; targeted phase 7/7 PASS (Teaching Copy ×3, MenuTutorial ×2, Clearance, Removal), MenuTutorial passing normally both times so the transient rule was not invoked; ScenarioOneInteraction PASS inside the 24-gate regression |
| F. P2 | batch ×2 `F58FB0B1A421D28B`; **interactive PASS** (all three layouts) with the same hash |
| G. P3 | batch ×2 and interactive PASS, `F7C2FC966816BBAD` |
| H. Full Legacy regression | **24/24 PASS** under the explicit Editor-only Legacy40 path, `production_source_unchanged` true for every gate, run head `a309f99`; Model2 `702766DECE591E21`, Reference Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D` present |
| I. Rendered 80 m review | `Enlarged80Review` PASS (556 s): 256 cells inside the screen, grid and side panel not overlapping, no label overflow at 1280 × 720 and 1920 × 1080; the review report equals the committed evidence apart from timing; the map grid, side panel and Work Plan regions are pixel-identical to the committed captures (0 differing pixels), only the animated forest behind the UI differs. Ground at 80 m, boundary ridges at ±40 m, no old ±20 m collider, no construction frames or prompts, ground texture not stretched (captures viewed) |
| J. Diff / cleanliness | no scene/prefab/material/`.asset`/`Packages`/`ProjectSettings` drift; no disposable harness or `.meta` left in `Assets`; the Editor's normalisation of 11 `.mat` files and `.vscode/settings.json` (listed in `Evidence/driver/post-status.txt`) was reverted and never committed; committed evidence indexes verified (Evidence80: 96 files, 0 mismatches; Phase-1 index: 14 committed files, 0 mismatches, the other 39 entries are ignored `Build/` files named in its note) |

## Disclosures

* **My own check failed once, falsely.** `IntegrationPreviewCheck` run 1 failed because I captured the player's "before" pose one frame after setting
  it, before the character controller had settled onto the ground (Y 0.10 vs 0.08; X and Z were exact). I fixed the check to settle both sides and compare
  with a 1 mm tolerance; run 2 passed. Both results are in `Evidence/D-preview-check/`. No product code was involved.
* **P2 interactive passed here.** It is bimodal: it failed with the known signature at the beginning and end of the 80 m task worktree and passed on its first
  run in this fresh worktree. The gate remains open and unexplained; one pass does not close it.
* **Disk.** The usual build disk had 6.2 GB free, less than a Library needs, so this worktree's derived `Library` and `Build` live in
  `/media/jer/Files/CCF-integration-80m-local-output/` (symlinked). Nothing else on that disk was touched.
* **Cosmetic.** The committed 80A gate still prints "new games Legacy40" in one pass message although it now asserts the production policy is
  Enlarged80 under the Legacy40 override. The assertion is correct; only the label is stale. Not changed (no commits on the implementation).
* Unity logged harmless import notes: it recreated an empty `Packages/com.kitwright.unity.mcp/UserSettings` folder (git cannot track empty
  directories) and could not pre-warm the compiler server or refresh a licensing token.

## Known follow-ups (recorded, not fixed)

1. Enlarged80 area-sensitive calibration (next required task; `Docs/Verification/StandExpansion/Enlarged80Calibration.md`).
2. Century Review copy: in Enlarged80 the review falls back to the aspirational design targets but the copy still says it is compared with the
   frozen Reference Future.
3. Malformed-save hardening: not every Scenario One record that carries a cell index (for example some work orders, planted juveniles, deadwood
   or history records) has a full geometry-range check before mutation. Normal generated v19/v20 saves are covered and verified.
4. Minimal startup/title screen, standalone Player performance profiling, native Windows technical smoke.

Stage A remains **not ready**.

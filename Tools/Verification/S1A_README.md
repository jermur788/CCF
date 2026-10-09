# S1-A Teaching Readiness verification

Run from the dedicated task worktree with Unity 6000.6.0f1. Never run against a dirty shared checkout. One Editor runs at a time. The runner refuses an existing staged harness and removes its own Assets copy and .meta in a finally block.

```sh
python3 Tools/Verification/run_s1a_gates.py --phase targeted
python3 Tools/Verification/run_s1a_gates.py --phase focused
python3 Tools/Verification/run_s1a_gates.py --phase regression
```

The default `--phase all` runs those three phases sequentially. Logs, results and full-resolution PNGs are retained in ignored `Build/S1A`; the focused gates retain their existing `Build/P2` and `Build/P3` locations. The regression retains `Build/WindthrowV1/Regression`. Configuration/preferences/saves are isolated in `Build/UnderstoreyRecruitment/config` (licence copied from the user's installed Unity licence). These launchers use the installed Editor at `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` and a systemd user scope with an 8 GB memory limit.

## Targeted contract

- Teaching-copy batch twice: accepted model stack, positive selection, repeated CCF management, contingent regeneration, Model2/legacy clearance meaning, pruning without price premium, all eight unchanged objective IDs, readable species labels, actual seed-present/seed-absent diagnosis and read-only forest hash.
- Nursery fixture: existing unit price and quantity determine exact total; current quotes/minimum determine the warning. An affordable purchase that leaves cash below the harvest minimum is still accepted, adds inventory and creates no planted juvenile.
- Interactive teaching gate: Help, Learning, current objectives, Crop Tree Inspection/P2 competitors, Work Plan, low-cash pre-purchase Nursery and first Annual Review at 1280×720 and 1920×1080. Inspect the PNGs as well as the automated bounds/control checks. Captures must dismiss first-use Help before inspecting job cards.
- MenuTutorialVerification: two independent interactive repetitions. The fixture waits for real aim/arrival state; if vegetation or a stem obstructs its ground ray, it exercises the accepted E-inspection alternative in the destination cell. No fabricated learning-credit call is used.
- ClearanceVerification: interactive, dismisses first-use Help and restores preferences; authoritative Model2 clearance target/execution contract preserved.
- ScenarioOneRemovalVerification: batch; U plans speciesless area clearance from the current authoritative preview, without immediately clearing or charging cash. Legacy selective-removal API tests remain.

## Compatibility gates

P2 and P3 each run batch twice plus interactive once through their existing launchers. Required hashes: `F58FB0B1A421D28B` and `F7C2FC966816BBAD`. Full current regression requires all 24 gates and explicitly checks Model2 completion `702766DECE591E21`, Reference v1 year 100 `7AD177B3CC2F73C7` and continuation `9CDF21A541C5968D`. Every exercised 16-digit hash is also compared with the committed modern regression record in `Docs/Verification/StormsWindthrowIntegration/regression-results.json`, including the save-19 completion and continuation hashes.

The regression now stages the reconciled current Clearance/Menu/Removal files directly. Its existing historical integration fixture and version-5 adjustment remain unchanged. Do not replace the modern suite with the historical pedagogy suite.

After all Editors exit, audit Git status and restore only task-generated editor/material normalisation to the clean task baseline; retain a record of the exact diffs. Never restore another checkout's pre-existing changes. No temporary verification source or generated .meta may remain under Assets. Do not include scenes, materials, prefabs, settings, packages or save data in the S1-A change.

These are Editor verification gates and controlled rendered fixtures. They do not certify standalone Player readiness or a human beginner pilot.

# PARITY-07 rendered qualification — result handoff

**Status: PASSED in Unity (rendered, Xephyr private display, callback/API driven — no OS-native input). UI presentation fix included.**

- Parent #91 / T3 #106. Candidate (U1 HEAD under test): `5cbde15d2fd1e72dcb87ccfd25101d4e29a7bf9d` (branch `qual/parity-07-unity-v1`; this handoff is the next commit on top, docs only).
- Lock id: `e7d884c3fd894b438ea211c3f753ff75` (owner T3-Parity07, task PARITY-07, lane /media/jer/ZX20/CCF-U1), released normally. Earlier tryout lock f03d887f36fc4a44bbf71c3d97665d28 (head c158c95, found the prompt overlap) also released.
- Run: 2026-10-29 (UTC), Unity 6000.6.0f1, executor Claude B (Claude Sonnet 5.5 Medium).
- Command: `python3 -I -B Tools/Verification/Parity07Unity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 5cbde15d2fd1e72dcb87ccfd25101d4e29a7bf9d --lock-id e7d884c3fd894b438ea211c3f753ff75 --task-id PARITY-07 --out /media/jer/Files/CCF/Verification/Parity07Unity-Repair2-20261029T1352`
- Evidence: `/media/jer/Files/CCF/Verification/Parity07Unity-Repair2-20261029T1352/` (36 PNGs + text dumps in `evidence/`).

## Result

- `process-result.json`: pass=true, unity_exit=0, reaped_after_verdict=false, screenshots=36, harness/licence copy removed, lane clean after.
- `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0`. World hash restored (A0E0B2A51CC9274A).
- Sizes: 1280x720 and 1920x1080 (UI reference scale 1600x900).

## Diagnosis of the earlier failures (fixable presentation issue, no wording decision needed)

- The felling-estimate card (`inspect-estimate`) is a non-scrolling panel (cursor is locked; no ScrollView) whose max height is the space under the status column and above the minimap: ~245 ref units. Its PARITY-07 copy needs ~445-490 at 600 wide (~640 at 400 wide), so text spilled below the card and over the minimap.
- The expanded detail column (600 wide) stacked a 2-per-row stat card (364) and detail (~555-598): bottom 919-976 > 900.

## UI change (presentation only; no wording/ecology/economy/save/Rack/MFM/S1 change)

1. `TreeInspectionView.EstimateWidth = 600` (was column width 400): estimate panel is wider, so the same copy wraps to fewer lines.
2. `ScenarioOneUiRoot`: minimap steps aside while the felling estimate is shown (same precedent as the competitor list); prompt max width now clears the wider of the two side columns symmetrically (the prompt is centred).
3. `ScenarioOneUi.uss`: when the detail column is expanded, 4 stats per row in the card and 3 per row in the detail (was 2); stat vertical padding 0 (from the earlier partial repair). Type sizes unchanged.
4. Harness: `INSPECTION_CLEARS_CENTRAL_PROMPT` and new `ESTIMATE_CLEARS_CENTRAL_PROMPT` checks (the prompt overlap was visible in a PNG, not caught by the earlier checks).

Measured after: estimate height 445-491 (y 373-865 < 900); expanded left column bottom 778-799 (< 900).

## Rendered verdict (every PNG opened or numerically checked; none transparent/blank)

All 36 captures are opaque, varied renders. Reviewed by eye: oak-unpruned-large 1280, oak-pruned-above 1920, detail-oak-storms-on 1280/1920, detail-beech-storms-off 1280, detail-sitka-storms-on 1920, workplan-pruning 1280, learning-pruning 1920; the rest passed the same numeric containment/blank checks. No clipped or overlapping text in the inspection cards; prompt clear of both columns; all PARITY-07 copy visible. Work Plan / Objectives modals scroll inside their own ScrollViews (reachable); the harvest card capture is scrolled to the end by the harness, so its first lines are above the viewport.

## Residual risk

- Minimap is hidden while a non-crop tree is inspected (new, deliberate, presentation-only); reviewer may prefer a different trade-off.
- Not tested: other aspect ratios than 16:9, Windows build, native-input interaction. Prompt wraps to 2 lines at 1920 while a tree is inspected.

# Observed intentional anchor deltas (evidence only)

**Policy (Manager, 2026-10-14): no broad historical expected hash is updated now. Final expected values are established on the integrated candidate, after all accepted feature branches are combined. Frozen archive anchors are not rebaselined.** This file records what was *observed* on isolated branches so the later rebaseline is a decision against evidence, not a surprise. It changes no assertion, runner or baseline.

## Observed deltas

| branch (isolated) | anchor | before | after | cause, with the proof |
|---|---|---|---|---|
| growth `796da99` (Oak/Beech ordinal [C] rows) | Reference continuation, `legacyV15LayoutHash` printed by `ScenarioReferenceVerification` | `9CDF21A541C5968D` | `BFFF66015A6619DA` | The frozen archive contains Oak/Beech trees that grow under live rules. Same gate, same head, pre-calibration rows applied in memory: `9CDF21A541C5968D`. |
| growth `796da99` | Model2 completion, `SCENARIO_ONE_COMPLETION_V18_COMPAT_HASH` (`ScenarioOneCompletionVerification`, Legacy40) | `702766DECE591E21` | `D99EC9FC2C635064` | A Scenario One play plants Oak/Beech. Same gate with old rows in memory: PASS with `702766DECE591E21` and world hash `84CD51EB6A951D9E`. |
| U4 `eb7fd3d` (reviewed two-cycle completion, event 9) | same gate and anchor | `702766DECE591E21` | `F5320B2FA2333B52` | The gate's play never clicks the Annual Review, so the scenario stays Active at year 25 and records no `ScenarioCompleted`. Same gate on pre-U4 production `157ea7d`: PASS with `702766DECE591E21`. The eight objective lines and the cash figures are identical in both runs. |

U4 chronology (event 9, the later completion year and the absent or present `ScenarioCompleted`) intentionally changes **every completion-state hash**, not only the one above. Any exact world, save or completion hash taken after a Scenario One play that acknowledges a review, completes, or fails under U4 differs from its pre-U4 value.

The deltas do not add up: growth and U4 each change the completion-play hash for a different reason, so the integrated candidate will produce a third value. Do not combine the two "after" values.

## Not rebaselined, observed unchanged on the growth head

Frozen Reference Future v1 archive and preview: start `A564039D9B7CE31D`, year 20 `F7DF7DAB53B6FD32`, year 50 `D5E75D6D21D631AC`, year 100 `7AD177B3CC2F73C7`, schedule `56C8B99FA1E8DDD1`. Pure-Sitka lifecycle and growth anchors `BFC55473C1506067`, `3485B6630C9EA448`, `962846D2F517B293`, `FBB8F470D85FF815`, `7E57B9DAEF5BF4D0`, `35E2BF1C2F55C2DE`; Legacy40 start world `EBE7A228F630AE41`; Enlarged80 start and year 3 `04A78188A6F8E6F9`, `081DB58CE3034648`; storm anchor 1 `8333BAA4126E8A09`. Not observed in that packet: P2/P3 (`F58FB0B1A421D28B`, `F7C2FC966816BBAD`), Model2 start/year 1, storm anchors 2 and 3.

## Gates whose assertions a later rebaseline will have to address (not touched here)

`ScenarioOneCompletionVerification` (exact v18 layout hash and completion at year 25), `ScenarioReferenceVerification` (continuation hash), and, for U4, any gate that reads `Outcome == Completed` or `annualReviewSeen` (`ScenarioOneProgressVerification`, `MenuTutorialVerification`, `Model2Verification`, `Enlarged80ViabilityVerification`, the storm long-run harnesses; not run on the U4 head).

## Evidence

On the verify branches `verify/oak-beech-production-calibration-796da99` (`Docs/Verification/OakBeechProductionCalibrationQualification/`, `evidence/ab-old-rows/`, `evidence/regression/`) and `verify/scenario-one-two-cycle-eb7fd3d` (`Docs/Verification/TwoCycleCompletionQualification/`, `evidence/baseline-historical-completion.markers.txt`, `evidence/historical-completion.markers.txt`).

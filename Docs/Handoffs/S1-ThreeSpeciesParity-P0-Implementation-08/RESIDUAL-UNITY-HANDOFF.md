# PARITY-08 residual Unity evidence (#166) — HANDOFF

**RESULT: PARTIAL — STOPPED. Sitka timber anchor PASS; growth-model-0 replay FAILED. Success token NOT issued.**

Parent #91, feature #126, review #128. Verification-only; no production, save, fixture, anchor or assertion change. No Windows build.

- Candidate under test (exact, U1 detached, clean): `3eb9c954c363659a1b012e06ce58a35ab87f4303`
- Verification branch `verify/parity-08-residual-unity-v1`, worktree `/media/jer/Videos/CCF-Parity08-Residual`, base `b6b96cb3` (contains approved #165). Plumbing commit `a3cb615f`; this handoff follows on the same branch.
- Plumbing (only file touched: `Tools/Verification/MfmApprovalBoundary/run_unity_gate.py`): new gates `completion-model0` (existing completion harness, `CCF_GROWTH_MODEL=0`, `CCF_REGEN_MODEL=1`) and `sitka-timber` (existing `TimberAssortmentYieldVerification`, with digest comparison to the PARITY-06 recorded anchor).
- Lock `e758e73a628149a2bb61a4042a0a003b` (owner Claude-PARITY-08-RESIDUAL, task PARITY-08-RESIDUAL). First release attempt was rejected for missing owner/task args; the second released normally (not forced). Lock free, no Editor, U1 restored clean at `b9bf11aa`. ZX20 free 20 GiB before the run.
- Evidence: `/media/jer/Files/CCF/Verification/PARITY-08/residual-unity-20261030/` (`chain.log`, `out/<gate>/`, lock JSONs, `driver/chain.sh`).

## Receipts
| Gate | Result | Exit | Sec |
|---|---|---|---|
| compile | pass | 0 | 61 |
| sitka-timber | `TIMBER_YIELD_VERIFY_PASS`; anchor_match true | 0 | 61 |
| completion-model0 | **FAIL**, no PASS marker | 1 | 88 |

Host saves unchanged, staged harness/metas and licence copy removed, lane clean in all three.

## Sitka timber anchor — PASS
Expected (PARITY-06 `sitka-timber`): fixtures=120 assertions=3007 evidenceSha256=`88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.
Observed: identical (all three values). Clean exit 0.

## Growth-model-0 replay — FAIL (not waived, not rerun)
Failure: `SCENARIO_ONE_COMPLETION_VERIFY_FAIL ... reasonable CCF plan did not complete at Year 25` although every objective line was `ok` (minimum-year 25/25, canopy 227/60, regeneration 51/3, deadwood 0.336/0.02, openings, Beech, Oak).
Observed markers: growth model 0 forced, regen 1 forced, RNG 1; `SCENARIO_ONE_COMPLETION_HASH 046551EB29730CE1`; v16LayoutHash `20B85B0152EA747A` vs documented historical anchor `6F84AF319D301F87` (Overview D-048 table, RNG1/regen1/growth0). V17 compat `F4B1D020BC8731D9`, V18 compat `0C91B203BA2FBB6A`.

Assessment (hypothesis, not verified): the model-1 path installs `HistoricalV18Model1Reconstruction.InstallPreCalibrationOakBeechRows()` to reproduce its historical anchor; the harness has no equivalent for model 0, so the model-0 replay runs on post-calibration Oak/Beech rows and drifts from the 2026-10 anchor. This may be a stale harness configuration rather than a PARITY-08 production defect, but it cannot be told apart from a candidate regression without a baseline run, and the same replay was never recorded on 185200f5. Making the replay pass would mean changing harness semantics (reconstruction scope or expected values), which #166 forbids. STOPPED per claim; no production or anchor repair attempted.

## Needed from Manager
Decide: (a) authorise a bounded harness change to apply the pre-calibration reconstruction for model 0, or define the intended model-0 positive replay (e.g. save-load-only fixture) and its expected values; and/or (b) authorise a baseline run of the identical model-0 gate on `185200f5` to separate candidate regression from stale anchor. #128 final verdict should still wait.

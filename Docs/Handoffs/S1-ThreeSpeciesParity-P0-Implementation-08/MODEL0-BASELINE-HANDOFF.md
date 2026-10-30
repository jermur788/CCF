# PARITY-08 model-0 baseline discrimination (#167) — HANDOFF

**PARITY-08 MODEL-0 FAILURE REPRODUCED ON BASE — NOT CANDIDATE REGRESSION**

Parent #91, feature #126, follow-up to #166. Verification-only; no production, save, fixture, anchor, harness or reconstruction change; no Windows build.

## Setup
- Baseline production under test: `185200f57140c65ed4d540ff0c4d173451170d14` (U1 detached, clean). Candidate: `3eb9c954c363659a1b012e06ce58a35ab87f4303`.
- Verifier and runner: identical to #166 (plumbing `a3cb615f`; branch `verify/parity-08-model0-baseline-v1` rooted at `0deab5ef`, worktree `/media/jer/Videos/CCF-Parity08-Model0Baseline`, `Tools/` identical to `a3cb615f`). `harness_sha256` and `runner_sha256` in both `command.json` files are equal; the environment differs only in output paths (`XDG_CONFIG_HOME`, `CCF_DIAG_DIR`, `CCF_ACCEPTANCE_OUTPUT`). Env: `CCF_GROWTH_MODEL=0`, `CCF_REGEN_MODEL=1`, Legacy40.
- Preflight: ZX20 20 GiB free, lock free, no Editor. Lock `085f2d7f13144a8f94a59b46ec357028` (owner Claude-PARITY-08-BASE), released normally (not forced); U1 restored clean at `b9bf11aa`, no Editor.
- Commands: `driver/chain.sh compile:compile completion-model0:completion-model0`. Evidence: `/media/jer/Files/CCF/Verification/PARITY-08/model0-baseline-20261030/` (candidate: `.../residual-unity-20261030/out/completion-model0`).

## Comparison
| Item | Candidate `3eb9c954` | Baseline `185200f5` |
|---|---|---|
| compile | pass, exit 0 | pass, exit 0 (61 s) |
| completion-model0 | FAIL, exit 1, no PASS marker | FAIL, exit 1, no PASS marker (84 s) |
| Failure | did not complete at Year 25; all 8 objectives `ok` (canopy 227/60, regen 51/3, deadwood 0.336/0.02, …) | identical text and values |
| Completion hash | `046551EB29730CE1` | `046551EB29730CE1` |
| v16 layout hash | `20B85B0152EA747A` | `20B85B0152EA747A` |
| v17 compat hash | `F4B1D020BC8731D9` | `F4B1D020BC8731D9` |
| v18 compat hash | `0C91B203BA2FBB6A` | `0C91B203BA2FBB6A` |
| Historical anchor `6F84AF319D301F87` | not reproduced | not reproduced |
| Host saves / harness / lane | unchanged / removed / clean | unchanged / removed / clean |

The failure and every hash are byte-identical on the accepted pre-PARITY-08 base, so PARITY-08 did not introduce the completion-replay drift. It is pre-existing D-047/D-048 verification debt (hypothesis, unverified: the model-0 replay lacks the pre-calibration Oak/Beech reconstruction that model 1 installs). `6F84AF319D301F87` was not rebaselined and the harness was not rewritten. Positive old-model-0 save compatibility still rests on the already-green Reference Future v1 (save v12) load/continuation, per the #167 packet.

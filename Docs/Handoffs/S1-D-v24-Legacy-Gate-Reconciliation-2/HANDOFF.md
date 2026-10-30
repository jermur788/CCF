# S1-D v24 Legacy Gate Reconciliation 2 — HANDOFF

**S1-D v24 LEGACY GATE RECONCILIATION 2 PASS** (all four gates have clean exit-0 receipts; see caveats)

Claim #165, parent #91. Related #126 / #143 / #164. Verification-only; no production, save/schema/validator/hash/reference, scene/prefab/package/ProjectSettings edits. No merge/integration, no Windows build.

## SHAs
- Branch `verify/s1d-v24-legacy-gate-reconciliation-2`, worktree `/media/jer/Videos/CCF-S1D-GateRecon2`, base `185200f5`.
- Commit 1 `798a88d3` (initial pins + progress allowance); commit 2 `358c5ee4` (completion line-443 pin + density-save allowance). Handoff commit follows on the same branch.
- Unity candidate (exact, NOT this branch HEAD): `3eb9c954c363659a1b012e06ce58a35ab87f4303` in U1 `/media/jer/ZX20/CCF-U1` (detached, clean). Runner and repaired harness sources were taken from this branch's checkout (`--project` U1); the runner stages them into U1 and removes them. Harness-file differences between 185200f5 and the candidate are limited to `SaveHardeningVerification.cs`, which is not one of these gates.

## Changes and authority (all in `Tools/Verification/`)
Authority for every version change: `ForestSaveData.CurrentVersion = 24` (D-066 teaching latch; `ForestSaveData.cs:7`), PARITY-08 RESULT (`/media/jer/Files/CCF/Verification/PARITY-08/unity-20261030/RESULT.md`), issue #165.
1. `ScenarioOneCompletionVerification.cs:153` — `CurrentVersion == 23` -> `== 24`. Other predicates unchanged (`MinimumHarvestJobCents == 250000`, `OwnerMinutesPerYear == 2400`, message text intact).
2. `ScenarioOneCompletionVerification.cs:443` — **not listed in #165**. Once the first pin passed, the gate failed at a second stale pin of the same class: `completed.version == 23` on the captured save (`growth-model-2 completion has the expected v21/...` check). Changed to `== 24`; growthModel/Legacy40/year 30/Completed/outcomeYear predicates unchanged. Only reached on the model-2 path (`completion`; `completion-model1` skips it).
3. `ThreeSpecies/Qualification/DensityQualification.cs:952` — `CurrentVersion == 23` and `"version":23` JSON check -> 24. Key-set equality (`keys[1].SetEquals(keys[2])`) and `"growthModel":2` retained. Marker name `SAVE_V23_...` unchanged. Legacy40 anchors untouched.
4. `MfmApprovalBoundary/run_unity_gate.py` — isolated-save allowlist now includes `progress`. Inspection: `ScenarioOneProgressVerification.cs` calls `saves.Save()` (lines ~157, 218, 236) and backs up/restores its own save path, so the isolated `forest-save.json.bak` is legitimate. Host-save, lane-clean, harness-removal and cleanup assertions are unchanged.
5. `run_unity_gate.py` — **extension beyond the packet**: `density-save` also added to the allowlist. Its `SAVE_DISK_ROUND_TRIP_MIXED_CONTINUES_IDENTICALLY` assertion calls `saves.Save()` (DensityQualification.cs:979), so the isolated `forest-save.json` is expected; first launch had PASS marker + exit 0 but runner `pass=false` for this reason alone. Manager may reject this item; if so density-save has no runner-clean receipt.

## Gate results (lock `3e26cffb11d04f9694ef3331ab4105d0`, owner Claude-S1D-GateRecon2, task S1D-GATERECON2)
Evidence: `/media/jer/Files/CCF/Verification/S1D-v24-GateReconciliation2-20261030/` (`out/<gate>/`, `chain.log`, `driver/chain.sh`, lock JSONs).

| Gate | Receipt dir | Result |
|---|---|---|
| compile | `01-compile` | pass, exit 0, 56 s |
| progress | `progress` | `SCENARIO_ONE_PROGRESS_VERIFY_PASS`, pass, exit 0, 72 s (no hang this launch) |
| completion | `completion-rerun` | `SCENARIO_ONE_COMPLETION_VERIFY_PASS`, pass, exit 0, 81 s; P0 `save=v24 minimum=250000 ownerMin=2400` |
| completion-model1 | `completion-model1-rerun` | `SCENARIO_ONE_COMPLETION_VERIFY_PASS`, pass, exit 0, 71 s |
| density-save | `density-save-rerun` | `DENSITY_QUAL_PASS`, pass, exit 0, 85 s; `SAVE_V23_MODEL2_MIXED_ADDS_NO_PERSISTED_FIELD version=24 keys 104/104 onlyIn=[]`; Legacy40 anchors `7E57B9DAEF5BF4D0/35E2BF1C2F55C2DE` PASS |

Host saves unchanged and harness/meta removed in every receipt.

## Preserved non-clean runs (not waived)
- `out/completion` — FAIL exit 1: second stale pin (item 2), found after item 1 let it past P0.
- `out/completion-model1` — **Editor exit hang**: `SCENARIO_ONE_COMPLETION_VERIFY_PASS` printed, then the Editor did not exit; runner timeout 1200 s (1233 s), `unity_exit` null. Not a pass. Clean rerun above. (Hang rate this session: 1 of 7 launches; PARITY-08 saw it on `progress`.)
- `out/density-save` — PASS marker + exit 0 but runner `pass=false` (isolated `forest-save.json`, item 5).

## Compatibility with integrated `185200f5`
This branch is based on 185200f5 and changes only verification harness/runner files. Schema v24 is identical in 185200f5 and the candidate; the candidate adds only unknown-species save validation (ForestSaveController/ForestSaveValidation) plus PARITY-08 harness files. The edits are version-expectation and allowlist changes only, so they should be compatible with 185200f5 production. **Not run in Unity on 185200f5 itself** — Unity ran on 3eb9c954 as instructed.

## Residuals
- Other stale `CurrentVersion == 23` pins remain elsewhere and were out of scope: `ScenarioOneEconomyIntegrationVerification.cs:104`, `ScenarioOneRackUx/RackUxRack.cs:507`, `ModelDefault/ModelDefaultVerification.cs:253` (named `..._STILL_23`), `MfmCoverageWorld/MfmCoverageWorldUnityGate.cs:166`, `RackV2FieldMarking/RackV2FieldMarkingVerification.cs:162`. Not run here; they will fail the same way if run on v24.
- Model-0 completion replay and Sitka timber anchors still not run in Unity (from PARITY-08).
- Free space on ZX20 was 19.5 GiB (>=15 mandatory, just under 20 preferred).
- Independent review not performed. Lock released normally; U1 restored clean at `b9bf11aa`, no Editor.

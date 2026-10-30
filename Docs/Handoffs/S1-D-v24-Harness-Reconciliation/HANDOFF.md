# S1-D v24 verification-harness reconciliation

Claim #164 (parent #91; related #124, #143). Worker: Claude Sonnet 5.5 (test sub-agent). Verification-only; no production, save/schema/validator/hash/reference, scene/prefab/package/ProjectSettings edit. No merge/integration, no Windows build.

**Result: `S1-D v24 HARNESS RECONCILIATION PASS`** (every required gate has a clean exit-0 receipt on the exact candidate; the intermittent teaching-latch Editor-exit hang is a residual, below). Independent review not performed.

## SHAs

- Unity candidate (U1, detached, production == integrated `bccd2b90`): `c838aca0b405490c5792c4150868b319c02529c8`.
- Harness branch `verify/s1d-v24-harness-reconciliation-v1`, worktree `/media/jer/Videos/CCF-S1D-HarnessRecon`: harness commits `43cfebf6` + `1d561f10`, cleanup `9022d621` (removes an empty stray file made by a malformed local sed; harness bytes unchanged), plus this handoff commit. Harness sources are copied from this worktree into the exact-candidate U1 lane by driver copies of the runners (only change: ROOT/`__file__` and the hard-coded lock task/owner -> `S1-D-164`/`Claude-S1D-164`); drivers are in the evidence folder.
- Lock `65be655211754244b3340caeb12296c6` (Claude-S1D-164 / S1-D-164), acquired after fresh preflight (lock free, no Editor, 20 GiB free on ZX20, exact HEAD), released normally; U1 restored clean detached at `b9bf11aa`, no Editor, no lane residue.
- Evidence: `/media/jer/Files/CCF/Verification/S1D-v24-HarnessReconciliation-20261030/` (`out/<gate>/` receipts+logs, `hang-diag/`, `driver/`, `chain*.log`). All failed/hung logs retained.

## Changed expectations and authority

| File | Was | Now | Authority |
|---|---|---|---|
| `Tools/Verification/ScenarioOneTeachingCopyVerification.cs` "accepted model stack" | `ForestSaveData.CurrentVersion == 23` | `== 24` (explicit equality, rest of the stack unchanged) | v24 schema: `ForestSaveData.CurrentVersion = 24` (`Assets/ForestPrototype/ForestSaveData.cs:7`), #143 handoff `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md` |
| same, "pruning without price promise" | `"timber quality"` and `"no timber-price premium"` | `"timber quality"` and `"Pruning sets no price by itself"` and `"Sitka and Beech have no such premium"` and `"fork or defect shape is not modelled"` | PARITY-07 `1e7b9413` (`LearningObjectivesView.Curriculum.cs`, `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-07/HANDOFF.md`: removed "applies no timber-price premium for pruning", untrue for Oak). The check still asserts no price promise, and now more strongly |
| `Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs` reload | one reload expecting index 4 | **unlatched** reload (latch cleared in a JSON copy of the completed save) keeps the original assertions: index 4, no session review facts, fresh J decisions restore completion with no cutting; then asserts that review re-earns the latch and index 5. **Latched** reload (completed save): index 5, session inspection facts not restored, subsequent J decisions inert (no cutting, no rewind, stays 5). Also asserts the completed capture carries the latch | D-066 / #143: `ScenarioOneSaveData.firstGuidedRackReviewComplete`; `DecideRackInspection` only acts when `Current(f)==4` (`LearningObjectivesView.Sequence.cs:122`); `Current` = `RecruitmentAndLater` when latched (`ScenarioOneTeachingSequence.cs:71`) |

No assertion was deleted or loosened: the pre-latch behaviour is still asserted, the post-latch behaviour is asserted explicitly. My first RackLesson edit (reload only) failed the next assertion, which also encoded index 4; that failure (`out/lesson`) is retained and was the reason for the unlatched/latched split.

## Gate results (exact candidate, one Editor at a time, isolated config/private Xephyr)

| Gate | Marker | Exit | Receipt |
|---|---|---|---|
| compile | `MFM06_COMPILE_PASS` | 0 | `out/01-compile` (and repeat `out/exp-compile-then-latch/01-compile`) |
| TeachingCopy | `S1A_TEACHING_VERIFY_PASS checks=61` | 0 | `out/teaching` |
| RackLesson | `LESSON_PASS checks=36` | 0 | `out/lesson-r2` (first try FAIL, retained: `out/lesson`) |
| #143 teaching-latch | `TEACHING_LATCH_UNITY_PASS checks=60` | **0** x3 | `out/teaching-latch-r2`, `-r3`, `exp-compile-then-latch/teaching-latch` (first try hung: `out/teaching-latch`) |
| S1-C history (T-03) | `S1C_T03_HARNESS_PASS checks=279 shots=16` | 0, 0 CHECK_FAIL | `out/history` |
| S1-C T-04 provenance rendered | `S1C_T04_HARNESS_PASS checks=529 shots=30` | 0 | `out/s1c-t04` |
| S1-C T-06 guidance rendered | `S1C_T06_HARNESS_PASS checks=170 shots=8` | 0 | `out/s1c-t06` (inspected the 1280x720 waypoint HUD capture: not blank, correct) |

Every passing receipt: pass=true, unity_exit 0, harness+meta+licence copy removed, host saves unchanged, no remaining lane changes.

## Diagnoses

**#143 teaching-latch hang: real, intermittent Unity Editor exit hang; not a runner/timeout/cleanup bug.** The first run here reproduced it: verdict `PASS checks=60`, Unity's own shutdown log ran to the final `MemoryLeaks` line (byte-for-byte the same tail as clean-exiting gates), then the Editor *process* stayed alive with the main thread at 99.9% CPU (state R, 8 other threads in futex/poll waits, `[dotnet]`/`Unity.Licensing` children defunct) until the runner's own `--timeout 900` fired and recorded `unity_exit: null`, `driver_issue: timed out`, pass=false (correct). gdb could not attach (`ptrace_scope`), so no native stack. #124's `exit -9` was the previous worker's external reaper, not the runner. Three further runs, including a compile-then-latch run to test a "first gate after compile" pattern, exited 0 in 180-189 s, so that pattern is not confirmed. The harness and production are unchanged between hung and clean runs. Treat as environmental/intermittent (Unity 6000.6.0f1, Linux/NVIDIA Vulkan/Xephyr). I did **not** accept the hung run's marker; acceptance rests on the clean-exit runs. Observed hang rate on this gate across #124 and #164: 2 of 5 launches. Evidence `hang-diag/`.

**S1-C history blank-screenshot failure (#124 first run): environmental/flaky, not reproducible.** That run reported 16 `SHOT_NOT_BLANK` failures (blank captures) and, separately, hit the same post-shutdown hang (reaped). Three later runs of the same gate (#124 rerun, `out/history` here) pass with all 16 shot checks, 0 failures, exit 0. No harness or production change was made for it.

## Residuals

- The Editor exit hang is unresolved upstream of this task. A gate whose Editor hangs after the verdict still fails its receipt (correctly). A future runner improvement could record an explicit "verdict printed, Editor hung at exit" state for triage, but must still not count it as pass; not done here (out of scope).
- Queued input remains synthetic (no native-OS input); the T-06 and T-04 rendered gates verify actual UI callbacks/rendering but not OS-native walking.
- RackLesson/TeachingCopy remain disposable harnesses; `ScenarioOneTeachingCopyVerification` copy pins are substring checks and will need updating again if PARITY-07 wording changes.
- Independent review and Manager acceptance not performed.

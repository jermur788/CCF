# Candidate-v2 tooling corrections, Phase 2

Starting HEAD: `2ba7408de312cd777249c5fc0d5e654e1573007f`, retained as an ancestor. Branch: `fix/candidate-v2-review-tooling`. Context: `cddad55213d3491b9f320a7a9a7a1bcd913b6ec8`. Production base: `c20c6dc242e4463b3de72e6c2d06b4ea9fa1fbc2`. Final HEAD is in the delivery handoff and branch readback. **Unity NOT RUN. READY FOR FOCUSED UNITY RE-RUN.**

**Final status addendum (2026-10-16): focused Unity rerun COMPLETED — PASS.** ScenarioHistoryVerification PASS (`SCENARIO_HISTORY_VERIFY_PASS assertions=43`, exit 0) and U4GuidanceSmoke PASS at a true 1280×720 Game view (`U4_GUIDANCE_PASS checks=113 failed=0 screen=1280x720`, exit 0) on tooling HEAD `ec9068371eae998774ca93dc75c8ff5b74d5a743`. Evidence: `/media/jer/ZX20/CCF-Verification/ScenarioOneCandidateV2-Phase2FocusedRerun-20261016T070954Z/` ([Phase2FocusedRerunManifest.json](Phase2FocusedRerunManifest.json)). The line above and the "Manager's focused Unity rerun" section below are retained as the state/commands prepared at handoff time.

## What changed and what remains to verify

- **History:** Y3 now requires `AnotherIntervention` and exact `GuidanceText(AnotherIntervention)`. Acknowledgement-year handling is inclusive (`>= first.Year`). The older idle-years wording is checked independently through `ScenarioInterventionHistory.NextStepHint(..., null)`. The original cycles 2–3, stale orders, chronology, save/reload and completion checks remain. Added explicit reviewed-two-cycle/None guidance and six marked-target completed-order checks after reload. The corrected Unity gate has not yet executed those later checks.
- **U4 hash:** removed the obsolete expected mixed-world hash `572025058B227E92`. `V1_world_hash_Y25_OBSERVED` records the current value only. The previously observed `009588E5ACCB4F8F` is **not** an expectation. Functional guidance/completion assertions remain. Final current-model hash adjudication is Tier-3; frozen/reference anchors are unchanged.
- **U4 size:** copied the proven `UxQualification.SetGameSize` approach into this disposable harness. It sets a fixed 1280×720 Game view before Play, rejects batch mode, waits boundedly for actual `Screen.width/height`, and aborts layout checks on a mismatch. Every capture records/asserts texture dimensions; terminal evidence reports `screen=...`, and receipts retain actual Game-view dimensions. `--window` alone was insufficient. Actual sizing/rendering remains for the graphical Unity rerun.
- **Selectors and phases:** density validates every phase and short scenario token, including mixed valid/invalid requests. `B,F`, `C,G` and `C1,G1` remain distinct valid selectors. Unknown/empty tokens fail explicitly before Unity launch in the runners and in the C# harness. An **explicitly empty** value — `CCF_QUAL_PHASES=` or `CCF_UXQ_PHASES=` — is **invalid and fails closed** (the empty string parses as an unknown token); omitting the variable altogether keeps the default phases. UX had the same silent-unknown-phase defect; `CCF_UXQ_PHASES` now rejects every unknown token too. Valid UX subsets and default phases remain available.
- **Measurement integrity:** check-bearing density phases cannot succeed with zero hard checks. Measurement-only LONGRUN/replay=0 instead requires complete positive scenario/year rows; the Python receipt reader checks the real CSV for exact expected keys, row count and geometry, rejecting header-only, partial, duplicate or unrelated data. Collection success has `measurement_verdict=PENDING_DOWNSTREAM_ANALYSIS`; run `max_ba_loss.py` or the appropriate downstream analyser for the measurement verdict. Existing PERF-only measurement runs similarly require all six expected records, without inventing performance thresholds. Explicit `--min-hard-checks` remains available when a caller requires assertions. No production density behavior or calibration changed.
- **UX licence source:** optional `--license-dir` copies the chosen source into the isolated config using the same pattern as the density runner; existing isolated licence folders are retained. Tests exercise argument/default handling, copying and command/environment construction with a fake Editor process, never a real licence or Unity launch.

## Shutdown event: retained as non-clean

Original evidence root: `/media/jer/ZX20/CCF-Verification/ScenarioOneCandidateV2-DeltaSmoke-20261015T235102Z/`. `E-L40-mixed-gate` emitted `MIXED_DENSITY_VERIFY_PASS checks=34`, then SIGSEGV'd on `ShutdownControllerHIDAPI` / `Application::CoreShutdown`; raw exit **255** and `mono_crash.mem.897289.1.blob` remain preserved. The identical rerun exited **0**. This is a single non-reproducible event, not a production fix or a waived crash.

Receipts now distinguish `PASS_CLEAN_SHUTDOWN` from `VERDICT_PASS_POST_SHUTDOWN_CRASH`. The latter is possible only after a valid verdict, with shutdown-path evidence, raw Linux exit 255/139/−11, and only root-level untracked `mono_crash.*` artifacts (or no unexpected artifacts). Overall status remains **FAIL**, raw non-zero exit and artifacts are retained, and the runner exits non-zero. Crash before verdict, an unrelated tree change, an ordinary non-zero exit or a missing verdict remains a failure without the special annotation. **A reproducible shutdown crash requires investigation.**

The original evidence directory was not modified. A byte-identical crash copy is under `/media/jer/ZX20/CCF-Verification/CandidateV2ToolingPhase2-20261016/preserved-shutdown-crash/`. The new receipt audit is explicitly a reinterpretation, not a rerun or a replacement for any original receipt. It retains the old vacuous receipts' historical PASS while demonstrating that the corrected evaluator rejects them; corrected measurement-only runs with complete rows remain accepted for downstream analysis.

## Non-Unity verification

Fresh receipts are in `/media/jer/ZX20/CCF-Verification/CandidateV2ToolingPhase2-20261016/`. SHA-256 identities and exact commands: [Phase2EvidenceManifest.json](Phase2EvidenceManifest.json).

- Phase-1 F10: **13/13 PASS**, both flow wiring checks PASS.
- Qualification/runner tests: **22/22 PASS**, including **28 actual C# density/UX policy cases**, invalid CLI rejection before licence/Unity setup, actual CSV coverage, shutdown limits and licence/command construction.
- Existing History offline: **74/74 PASS**; N3 offline: **88/88 PASS**; UX offline: **176/176 PASS**.
- Approximate tools type-check: **CLEAN**, zero new errors (13 existing errors against baseline 43); this is not a Unity compile.
- Original receipt audit: PASS; 276 original evidence files hashed, valid zero-check measurement collections retained, invalid filters rejected, crash exit 255 retained as FAIL.
- Phase-1 manifest/originals, frozen/production source invariants and `git diff --check`: PASS. Final committed `Assets`, `Packages`, `ProjectSettings` diff from original candidate must be empty.

## Manager's focused Unity rerun

Use a clean designated Unity lane at the exact final correction HEAD, under the existing Unity ownership lock. These commands are prepared for that future run; **they were not executed in this task**. Use fresh output folders. The disposable runners stage/remove harnesses and `.meta`; History captures/restores in memory, while other qualification tools can use isolated disk saves. Preserve normal runner exit status and inspect evidence. No Tier-3 or Claude review is requested here.

History, Legacy40:

```sh
python3 Tools/Verification/ThreeSpecies/Qualification/run_qualification.py \
  --label history-phase2 --out /media/jer/ZX20/CCF-Verification/Phase2UnityRerun/history \
  --method ScenarioHistoryVerification.Begin \
  --file Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs \
  --marker SCENARIO_HISTORY_VERIFY_PASS --min-hard-checks 1 \
  --license-dir /home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses \
  --env CCF_STAND_GEOMETRY=0
```

Expect the gate to reach cycles 2–3, stale-order checks, the reviewed-two-cycle chronology assertion, history identity after reload, all six marked targets' completed orders, and completion-summary checks, ending `SCENARIO_HISTORY_VERIFY_PASS assertions=...`, exit 0.

U4, Enlarged80 as in the original smoke, graphical 1280×720:

```sh
python3 Tools/Verification/ScenarioOneUx/Qualification/run_qualification.py \
  --label u4-phase2 --out /media/jer/ZX20/CCF-Verification/Phase2UnityRerun/u4 \
  --method U4GuidanceSmoke.Begin \
  --file Tools/Verification/U4Guidance/U4GuidanceSmoke.cs \
  --marker U4_GUIDANCE_PASS --interactive \
  --license-dir /home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses \
  --env CCF_STAND_GEOMETRY=1 \
  --env U4_OUT=/media/jer/ZX20/CCF-Verification/Phase2UnityRerun/u4/evidence
```

Expect `U4_NOTE GAME_VIEW actual=1280x720`, each screenshot at 1280×720, all functional guidance/completion assertions passing, an observation-only Y25 hash, and `U4_GUIDANCE_PASS ... screen=1280x720`, exit 0. Any other actual size must fail. Final current-model hash adjudication remains Tier-3.

For later measurement commands use `--env CCF_QUAL_PHASES=LONGRUN --env CCF_QUAL_SCENARIOS=B,F --env CCF_QUAL_YEARS=300 --env CCF_QUAL_REPLAY=0`, followed by `max_ba_loss.py <out>/evidence/longrun-E80.csv`. A completed data collection alone is not a passing BA-loss analysis.

Actual execution route: T3 Code / Codex / GPT-6.1-Sol / this session's allowance; requested DeepSeek route unavailable and disclosed. No main integration, acceptance decision, production change or new simulation anchor was made.

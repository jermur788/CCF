# S1-D T-06 — Unity qualification after test-only fixture reconciliation

**Task:** S1-D T-06 management-event ID save hardening, claim #88; bounded test-only repair claim #95.
**Exact qualified candidate:** `51e685ef7400a7171d34ba063ece93b1a15d1820`
**Base reviewed code candidate:** `29bb6cbd6bf5dd81499a1e45bb68666f34969309`
**Branch:** `fix/s1-d-t06-fixture-legacy40-v1`
**Worktree:** `/media/jer/Videos/CCF-S1D-T06Fix` (source); Unity lane `/media/jer/ZX20/CCF-U1` detached at the same SHA.
**Executor:** DeepSeek orchestration/integration operator, after the Claude B qualifying thread was rate-limited. Tasks Q1/Q2 were originally run by Claude B / Claude Opus 5.5 at `1a44526`; Q1-Q5 were rerun by the operator on the final exact test-fix SHA `51e685e`.
**Exit:** `S1-D T-06 UNITY QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

## Production bytes

Production is byte-identical to the reviewed candidate `29bb6cb`:
- `Assets/ForestPrototype/ForestSaveValidation.cs` = `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb`
- no production, save-schema, validation, migration or verification-assertion change in `29bb6cb..51e685e`.

The only changes are test fixtures in:
- `Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/SaveEventIdIntegrityVerification.cs`
- `Tools/Verification/SaveCellIndexHardening/SaveCellIndexHardeningVerification.cs`

## Why the test-only repair was required

1. **Legacy40 copies retained Rack V2.** Since save v21 (D-064) the live Enlarged80 world carries `rackPlanningModel = RackPlanningFieldMarkingV2`. The Q1/Q3 fixtures cloned that world and shrank it to Legacy40 without clearing V2, so `ForestSaveValidation.RackV2` correctly refused the fixture before any event-ID/cell-index case. Repair: `MakeRackModelZero` sets model 0, no racks, plan unconfirmed, `nextRackId = 1`, removes rack-owned orders; explicit preconditions assert model-0/Legacy40 and call `ValidateRackV2` before the cases.
2. **Q3 startup Continue used stale menu state.** Normal Editor Play already had the startup screen open, so `ShowStartupScreen()` returned early and `ContinueAvailable` retained a pre-fixture value. Repair: the test closes an open startup menu before reopening it. A pristine `enlargedBaseline` clone is also used for the final valid-save case so a fixture-mutated working capture cannot leak into it.
3. **No assertion was weakened or rebaselined.** The event-ID cases, rejection reasons, no-mutation checks, cell-index cases, startup Continue checks and malformed-save checks are unchanged. The new preconditions add evidence, not exclusions.

## Gate results on exact `51e685e`

| Gate | Result | Evidence |
|---|---|---|
| Q1 event-ID runtime parity/no mutation | **PASS** `SAVE_EVENT_ID_VERIFY_PASS cases=19 rejected=9 accepted=10 canLoad=LoadData=Load sharedReason=exact diskBytes=unchanged noMutation=exact` | `fix-51e/q1/` |
| Q2 save hardening preservation | **PASS** `SAVE_HARDENING_VERIFY_PASS` (73.4 s) | `fix-51e/q2/` |
| Q3 cell-index hardening | **PASS x2** `SAVE_CELL_INDEX_HARDENING_VERIFY_PASS cases=131 rejected=78 accepted=53 diskParity=exact noMutation=exact` + `SAVE_CELL_STARTUP_PASS` | `fix-51e/q3/` |
| Q4 U4 duplicate-ID runtime | **PASS** `U4_VERIFY_PASS phase=core checks=112 failed=0 openFindings=0`; includes `SYN duplicate event IDs` refused, year preserved, Scenario One/world snapshot preserved | `fix-51e/q4q5/` |
| Q5 Unity import/compile | **PASS** exit 0, 0 `error CS`, includes deferred T-02 import/compile | `fix-51e/q4q5/compile.log` |

Q1/Q2 were also run by Claude B at `1a44526` with the same PASS results; the final `51e685e` reruns are the authoritative evidence because the Q3-only fixture changes came later.

## Licence / environment note

The checked-in host licence copy is expired (`com.unity.editor.headless` not found, exit 198). All runs used the current Unity Hub licence in an isolated `XDG_CONFIG_HOME`/throwaway config; no licence file or global config was edited. This is an operational debt for Ops to refresh the host copy.

## Lane, lock and side effects

- U1 was clean and detached at `51e685e`; lock owner `DeepSeek-S1D-T06`, lane U1, exact HEAD.
- Lock released normally; status free, no Unity Editor running.
- Editor side effects (`.vscode/settings.json`, Section Five materials) were captured and restored; U1 `git status` is empty.
- No staged harness script or `.meta` remains under `Assets/ForestPrototype/`.
- No Windows or standalone Player build.

## Residual

- Independent review of this test-only reconciliation is still required before integration.
- Production is unchanged from the already independently reviewed `29bb6cb`; the requested production blob is exact.
- Q3's repaired startup-Continue case now passes at both runs; its earlier failure was fixture state, not production behavior.

# Scenario One Final Tier-3 — Complete-Loop Qualification — BLOCKED RETURN

**Claim:** #231  
**Parent:** #91 / Browser Overall #200 / local orchestration #203  
**Amendment consumed:** `TEST-REDUCTION-AMENDMENT-20261103.md` (#238)  
**Date:** 2026-11-03  
**Status:** **STOPPED — BLOCKED — RETURNED TO BROWSER OVERALL**  
**Success token:** **NOT ISSUED**

No production repair was made under #231. The qualification stopped on a reproducible current-candidate reference-preview defect and returned to Browser Overall for a separate bounded repair claim.

## 1. Exact identity and routing

| Field | Value |
|---|---|
| Exact starting candidate | `6849caa0ef05baf1a52b95465ec6bba4ff94202b` (`integration/scenario-one-player-test-ready-v1`) |
| Qualification branch | `qual/scenario-one-final-tier3-v1` |
| Dedicated Unity lane | `/media/jer/ZX20/CCF-U1` (registered, warm, clean; no other writer) |
| Verification-only evidence head | `a95c4c4e35decb7a518d543488678473430f69b6` |
| Production diff candidate → evidence head | **empty** (`git diff 6849caa0..a95c4c4e -- Assets Packages ProjectSettings`) |
| Primary worker | `opencode-go/deepseek-v4.1-flash` (T3 Code local orchestrator; release/repin fallback lane, same lane as the accepted MFM-12 return) |
| Unity | 6000.6.0f1, Linux graphical Editor, private Xephyr displays |
| Evidence root | `/media/jer/Files/CCF/Verification/ScenarioOneFinalTier3-20261103/` |
| Windows build | none; #194 NO ACTION |

Fresh preflight before the run: lock FREE/metadata None/no Editor PIDs; no Unity/Xephyr/openbox/native-input driver; ZX20 22 GiB free (15 GiB floor); licence present; host saves hashed. Lock `dcddd6fe…` and four re-acquisitions after verification-only head advances were all released normally; final state FREE.

## 2. Result summary

The reduced plan completed the highest-value current-candidate gates and found **one genuine retained-gate blocker**:

1. **BLOCKER — reference-preview exit does not restore the player's Enlarged80 world.**  
   `ScenarioOneEconomyIntegrationVerification` fails at *“preview exit replaces exact saved protection/player state”*. The product’s own pre-preview return snapshot is correct (`trees=1344 version=24 geometry=1`), but after `TryBeginReferencePreview(20)` / `EndReferencePreview()` the live world contains **335** living/tree records instead of **1344**. A manual `LoadData` of the same snapshot restores 1344, so the data is recoverable but the product’s preview-exit path leaves the wrong world. This is a player-facing state/protection defect in the frozen Reference Future preview contract on the default Enlarged80 geometry.  
   Clean receipt: `unity/49i-economy/` (`SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_FAIL … preview exit replaces exact saved protection/player state`; `ECONOMY_PREVIEW_CAPTURE trees=1344 version=24 geometry=1`; `ECONOMY_PREVIEW_TREES beforeCount=1344 afterCount=335`). Earlier clean failure receipts `49b`–`49f` are retained.

2. **Unresolved Rack gate findings (secondary, need Browser Overall triage):**
   - `RACK_V2_MAP_PLANNING_UNITY_FAIL checks=17 shots=0 exception=System.InvalidOperationException: five navigation tabs remain navigation` (`unity/38-rack-map/`). The same run also hit an X `BadWindow` error, so an environment/driver contribution cannot be excluded; the assertion is a historical UI-structure assumption.
   - `RACK_V2_EDITING_UNITY_FAIL checks=270 shots=12 exception=System.InvalidOperationException: 1920x1080: rotating well off level is free (0 deg), and the entrance did not move` (`unity/39-rack-editing/`). Real-pointer rotation at 1920×1080 did not move the entrance; 1280×720 passed. This may be a current rendered/interaction defect and is reported, not waived.

3. All other retained current-candidate gates below passed. Two transient blank-capture failures (`13-combined-v2` first attempt, `23-s1c-t06` first attempt) were re-run on fresh displays and passed cleanly; they are recorded as superseded diagnostics, not product findings.

## 3. Retained current-candidate evidence (all on one lineage)

### Compile / import
- `01-compile` PASS at `d9caa769`/`150c6d34`/`ee4b46b7` (and again at the final relabel head); `S1C_T05_COMPILE_PASS`, Unity 6000.6.0f1.

### Mandatory S1-C T-05 final-candidate rerun
- `S1C_T05_UNITY_PASS checks=544 cases=34 shots=12` (`unity/02-objective/`).
- `A0_CURRENT_VERSION_24 version=24`; current v24 save/load, genuine v20/v21 old saves (`v20-y5`, `v21-y1`, `v21-y0-rack`), derived/not-recorded history states, refusal-before-mutation for malformed/history data.
- `T05_RENDER_PASS captures=6` (`unity/03-render/`): recorded / derived / not-recorded Annual Review lines fully visible and unclipped at 1280×720 and 1920×1080.
- Verification-only relabel: the accepted T-05 harness's current-schema pins were moved from the obsolete v22 to the candidate's v24, with the fixture renamed `recorded-v24-year3-save.json`; every assertion and the v22 feature-boundary refusal text were preserved. Diff is visible in the branch history.

### Save / corruption / compatibility / reference
- `MFM06_UNITY_PASS checks=159` (`06-mfm-save`): v24 round-trip, malformed/ambiguous rejection before mutation, provenance identity.
- `SAVE_HARDENING_VERIFY_PASS` (`08-mfm-hardening`): malformed saves rejected before mutation.
- `REFERENCE_FUTURE_V1_PASS`; frozen archive/preview/continuation contract (`07-mfm-reference`).
- `RACKV2SAVE_VERIFY_PASS checks=35` (`42c-rack-save`); `RACKV2PERSIST_VERIFY_PASS` v2/coldstart/legacy40 (`43d`/`44d`/`45d`); `Reference Future v1 archive verified and Year-100 hash 7AD177B3CC2F73C7 untouched` in every persistence run.
- `PARITY08` offline PASS; `13-save-mutation-controls` 25/25 mutants killed; `12-save-provenance` PASS.

### MFM player path / commitment boundary
- `MFM07_UNITY_PASS checks=106` (`04-mfm-approval`).
- `MFM08A_UNITY_PASS checks=56` (`05-mfm-coverage-world`).
- `MFM09_UNITY_PASS checks=261 shots=16` (`09-mfm-extension`): sample → fit/coverage → candidate → 3D confirm/reject → proposal → prescription approval → ordinary Work Plan → execution.
- `MFM08_UNITY_PASS checks=412 shots=29` (`10-mfm-treatment-review`): player judgement, proposal ≠ Fell mark, candidate judgement, 3D spot-check, approval ≠ commitment, Work Plan commitment, provenance, invalid/unavailable-fit refusal.
- `MFM11_UNITY_PASS checks=47 shots=2` (`11-mfm-calibration`): complete-loop witness, year-25 completion, 8/8 objectives, later intervention, solvency, accepted MFM-11 calibration boundary.
- `SCENARIO_ONE_COMPLETION_VERIFY_PASS` (`33-completion`, `34-completion-model1`); `N3_U4_VERIFY_PASS` (`35-n3`); `SCENARIO_ONE_PROGRESS_VERIFY_PASS` (`37-progress`); `DENSITY_QUAL_PASS` save (`36-density-save`).

### Rack V2 / Stand Map / physical entrance marking
- `RACK_V2_CONFIRM_MINIMAP_UNITY_PASS` (`40-rack-confirm`).
- `RACK_V2_FIELD_MARKING_PASS checks=99 shots=10` (`41c-rack-field`): physical entrance marking with real OS input.
- `RACKV2_O1E2E_PASS checks=173 shots=24` (`46-rack-o1-e2e`): real startup → map planning/edit/confirm → field marking → approval → execution → Objective 2 → save/load.
- Rack edit/spacing/suggested-layout/StandMap offline checks PASS.
- Map-planning and editing Unity gates remain the two unresolved findings in §2.

### S1-C / S1-D history and repeated interventions
- `S1C_T02`, `S1C_T03`, `S1C_T04`, `S1C_T06` Unity harnesses PASS (`20`–`23b`); T-06 clean rerun `checks=162` with no failures.
- `S1D_T05_UNITY_PASS` previous-vs-proposed Work Plan presentation (`32-s1d-previous-proposed`).
- `S1D_T04_GUIDE_UNITY_PASS` guide monotonicity (`31-s1d-guide`).
- `TEACHING_CAP_UNITY_PASS`, `LESSON_PASS`, `MENU_TUTORIAL_PLAYTHROUGH_PASS`, `S1A_TEACHING_VERIFY_PASS`, `RACKV2RUNTIME_VERIFY_PASS`, `SAVE_EVENT_ID_VERIFY_PASS`, `TEACHING_LATCH_UNITY_PASS` (`24`–`30`).
- Three-cycle repeated-intervention/history witness: `SCENARIO_HISTORY_VERIFY_PASS assertions=43` (`19b-history-l40`), including idle/stale carried work, prior-vs-proposed, completion and continued management.

### Species / economy / geometry
- `MIXED_DENSITY_VERIFY_PASS` Legacy40 (`16b`); `BROADLEAF_TIMBER_VERIFY_PASS` Legacy40 and Enlarged80 (`17b`/`18b`).
- `STAND_GEOMETRY_VERIFY_PASS` (`47b`); `ENLARGED80_VERIFY_PASS` (`48b`).
- Offline: timber, history, pruning, prescription core, candidates, coverage world, approval boundary, treatment review, site, growth, density (three accepted pure-Beech R2 misses against unchanged 0.40).

### Rendered / interaction
- `GUIDED_ONBOARDING_PASS` 535/0 (`12-guided-v2`), `CHC_DONE pass=465 fail=0 shots=24` (`13-combined-v2`), `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0` (`14-parity07`), `SCENARIO_ONE_INTERACTION_VERIFY_PASS` (`15-interaction`).
- Representative 1280×720 / 1920×1080 captures exist for Rack map/field, MFM fit/coverage/candidate/proposal/approval, Annual Review/history, Work Plan previous/pending, completion, HUD. The mandatory T-05 rendered set was inspected; the economy blocker prevented a clean final rendered pass of the full loop.

## 4. Reduced test plan (amendment #238)

Already-completed evidence was preserved; no successful test was restarted.

| Category | Disposition |
|---|---|
| Exact-candidate compile/import | Retained (final relabel head) |
| Mandatory S1-C T-05 v24 save/history + rendered states | Retained, fresh current-candidate rerun |
| Current v24 / malformed / old-save / provenance | Retained via MFM-06 save, hardening, parity08, mutation controls |
| Legacy40 / frozen Reference Future | Retained via MFM reference, Rack persistence legacy40, StandGeometry/Enlarged80 |
| MFM judgement/proposal/Work Plan boundary | Retained via integrated treatment-review + approval + extension + complete-loop witness |
| Completion / solvency / objectives | Retained via MFM-11 witness, Completion model-2/model-1, N3, Progress |
| Rack integrated path | Retained via ConfirmMiniMap + field marking + Objective-1 E2E + persistence; component map/editing gates attempted and failed |
| S1-C/S1-D history/repeated cycles | Retained via T-02/T-03/T-04/T-06, guide, previous-vs-proposed, three-cycle history |
| Representative both-resolution rendered evidence | Retained; duplicate near-identical captures not additionally generated |
| Omitted / inherited | Repeated species component reruns with unchanged production bytes; duplicate guided/combined/parity rendered reruns after one clean integrated suite; repeated save rejection reruns already covered by MFM-06 + hardening + parity08; duplicate rack runtime/persistence variants subsumed by field+save+persistence+E2E; repeated history cycles beyond the mandatory T-05 rerun plus the three-cycle witness |
| Approx. remaining executions avoided | ~10–15 planned Unity gate executions (about half of the remaining plan) plus duplicate screenshot runs, before the blocker made further qualification moot |
| Residual confidence limitation | The two Rack map/editing failures are unresolved and the reference-preview exit defect blocks the player-test candidate; no full-loop green claim is made |

The reduction did **not** alter assertions, thresholds, anchors, save schema or protected references.

## 5. Accepted limitations retained (must not be promoted)

- Three pure-Beech reduced-simulator R2 misses remain accepted against the unchanged 0.40 criterion.
- MFM-11 extension-state calibration remains GAMEPLAY CALIBRATION.
- Post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.
- F11 candidate workload remains a human/user-test obligation.
- Fit results remain recomputable rather than separately persisted; rule-only/invalid-fit rendered coverage remains as previously disclosed.

## 6. Cleanup and final state

- Final lane `/media/jer/ZX20/CCF-U1`: **clean including untracked**, HEAD `a95c4c4e35decb7a518d543488678473430f69b6`, production diff to `6849caa0` empty.
- No Unity Editor, Xephyr, openbox or native-input driver running.
- Machine-wide Unity lock **FREE** (`final-lock-status.json`); final release recorded.
- Host saves unchanged (`7effe9b7…` / `c0169790…`).
- No staged harness or `.meta` remains under `Assets/ForestPrototype/`.
- No Windows build; #194 NO ACTION.

## 7. Return to Browser Overall

This is **not** a Tier-3 pass. Recommended next action:

1. Open a bounded production repair claim for the reference-preview exit on Enlarged80 → Legacy40 archive → Enlarged80 (restore must leave the exact 1344-tree player world).
2. Triage the Rack map-planning tab-count assertion and the 1920×1080 rack-rotation interaction failure; distinguish stale historical assertions from current product defects.
3. After repair, rerun the affected gates and the mandatory S1-C T-05 chain on the new exact candidate; the rest of the retained reduced plan above can be reused only where its production surface is byte-identical.

Local success token **not issued**:

`SCENARIO ONE TIER-3 COMPLETE-LOOP QUALIFIED — READY FOR INDEPENDENT FINAL REVIEW`

Browser Overall token **not issued**:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

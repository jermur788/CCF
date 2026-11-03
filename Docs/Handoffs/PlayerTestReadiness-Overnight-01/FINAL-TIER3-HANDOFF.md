# Scenario One Final Tier-3 — Complete-Loop Qualification — BLOCKED RETURN

**Claim:** #231  
**Parent:** #91 / Browser Overall #200 / local orchestration #203  
**Amendment consumed:** `TEST-REDUCTION-AMENDMENT-20261103.md` (#238)  
**Date:** 2026-11-03  
**Status:** **STOPPED AT TASK 1 CHECKPOINT — BLOCKED / NEEDS MORE EVIDENCE — RETURNED TO BROWSER OVERALL**  
**Independent review verdict:** `NEEDS MORE EVIDENCE`  
**Success token:** **NOT ISSUED**

No production repair was made under #231. The qualification stopped at a Task 1 checkpoint on an unresolved current-candidate reference-preview lifecycle/timing finding and a rendered-completion evidence gap. The independent review did **not** confirm an ordinary player-facing preview defect and found that the completion-rendering claim is not substantiated. This is a blocked/needs-evidence return, not a confirmed production defect and not a Tier-3 pass.

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

The reduced plan completed the highest-value current-candidate gates and produced **one unresolved lifecycle/timing finding, one rendered-evidence gap, and two Rack classifications**:

1. **CONFIRMED SAME-FRAME LIFECYCLE FAILURE — ordinary player-facing preview defect NOT established.**  
   `ScenarioOneEconomyIntegrationVerification` fails at *“preview exit replaces exact saved protection/player state”*. The product’s private pre-preview return snapshot is correct (`trees=1344 version=24 geometry=1`), but the harness calls `TryBeginReferencePreview(20)` and `EndReferencePreview()` **without yielding a frame between them**. The live world then has 335 tree records instead of 1344. Production `LoadData` deactivates and schedules unwanted tree objects for destruction at frame end and reuses matching objects on later loads, so an immediate same-frame exit races that deferred destruction.  
   Retained frame-separated tests already report exact Enlarged80 restoration (`47b-standgeometry`, `48b-enlarged80`). A **frame-separated Year-20 round trip** with the protected-player fixture is required to decide whether a normal player visit loses trees.  
   Clean receipt: `unity/49i-economy/` (`SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_FAIL … preview exit replaces exact saved protection/player state`; `ECONOMY_PREVIEW_CAPTURE trees=1344 version=24 geometry=1`; `ECONOMY_PREVIEW_TREES beforeCount=1344 afterCount=335`). The earlier 49g/49h diagnostics manually restored the world before the assertion and are **not valid gate receipts**; commit `a95c4c4e` removed that mutation, so 49i is the clean receipt. Do **not** prescribe production repair from these receipts alone.

2. **Rack findings — classifications corrected by independent review:**
   - **Map planning: stale assertion, not a product failure.** The harness requires exactly five navigation tabs (`RackV2MapPlanningVerification.cs:318`), while current `StandMapView.cs:255` intentionally adds a sixth, “Treatment review”, tab in V2. The planning control remains separate from navigation. The same run’s X `BadWindow` is real but not needed to explain the failure (`unity/38-rack-map/`).
   - **1920×1080 rotation: input-contaminated receipt; product defect unconfirmed.** `39-rack-editing` records 0°, while 720p passed. The pointer trace shows the pointer leaving the intended destination and releasing around world `(-49.88, -30.74)`, while the driver requested endpoint `(884, 729)`; this is not a clean intended rotation/release. The failure wording concerns the angle; the entrance is supposed to remain fixed. Re-run cleanly before classifying as a product defect.

3. **Rendered-completion evidence gap.** Both `11-mfm-calibration/evidence/mfm11-final-*.png` files are uniformly black. The harness restores the baseline before taking them (`Mfm11CalibrationImpactGate.cs:587`, screenshots at `:655–662`), so even usable images there would not show Year-25 completion. The complete-loop data is present (Completed year 25, cash 348152 cents, 8/8 objectives), but **no usable rendered Completed state** substantiates the handoff’s completion-rendering claim. This is an evidence gap, not a product failure.

4. All other retained current-candidate gates below passed. Two transient blank-capture failures (`13-combined-v2` first attempt, `23-s1c-t06` first attempt) were re-run on fresh displays and passed cleanly; they are recorded as superseded diagnostics, not product findings.

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
- Representative 1280×720 / 1920×1080 captures exist for Rack map/field, MFM fit/coverage/candidate/proposal/approval, Annual Review/history, Work Plan previous/pending and HUD. The mandatory T-05 rendered set was inspected. **No usable rendered Completed state was produced:** the two MFM-11 final captures are uniformly black, and the harness restores baseline before capturing them. Understandable completion is therefore **not visually verified**, and no clean full-loop rendered pass is claimed.

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
| Rack integrated path | Retained via ConfirmMiniMap + field marking + Objective-1 E2E + persistence (all PASS). The component map gate is a stale six-tab assertion; the 1920×1080 editing receipt is input-contaminated. |
| S1-C/S1-D history/repeated cycles | Retained via T-02/T-03/T-04/T-06, guide, previous-vs-proposed, three-cycle history |
| Representative both-resolution rendered evidence | Retained; duplicate near-identical captures not additionally generated |
| Omitted / inherited | Repeated species component reruns with unchanged production bytes; duplicate guided/combined/parity rendered reruns after one clean integrated suite; repeated save rejection reruns already covered by MFM-06 + hardening + parity08; duplicate rack runtime/persistence variants subsumed by field+save+persistence+E2E; repeated history cycles beyond the mandatory T-05 rerun plus the three-cycle witness |
| Approx. remaining executions avoided | **~10–15 is an estimate, not a gate-by-gate reconciliation.** The reduction kept the MUST KEEP gates and removed duplicate/overlapping reruns; after the Task 1 stop, no further remaining gates were executed. |
| Residual confidence limitation | Normal-player reference-preview restoration is not proven either way (same-frame harness race); the Rack map failure is a stale six-tab assertion; the Rack 1920×1080 rotation receipt is input-contaminated; and rendered completion is unverified (black MFM-11 captures). No full-loop green claim is made. |

The reduction did **not** alter assertions, thresholds, anchors, save schema or protected references.

## 5. Independent review

Published separately as `FINAL-INDEPENDENT-REVIEW.md` in this folder. Reviewer: OpenAI GPT-6.1-Sol (high reasoning), read-only, cross-provider, independent of the qualification worker.

**Verdict: `NEEDS MORE EVIDENCE`.** The stop/return is justified because qualification remains unresolved. The review corrected three handoff overstatements:

- The reference-preview failure is a confirmed **same-frame lifecycle** failure, but the receipts do **not** establish that an ordinary player preview visit loses trees. A frame-separated Year-20 round trip is the discriminating test.
- The Rack map-planning “five tabs” failure is a **stale assertion** (the current UI intentionally has a sixth “Treatment review” tab). The Rack 1920×1080 rotation receipt is **input-contaminated**; product defect unconfirmed.
- The MFM-11 “final” screenshots are black, and the harness restores the baseline before taking them; **rendered completion is not demonstrated**. The complete-loop data itself is present.

The review found no false green in the retained compile, v24 history, save-safety, MFM boundary or complete-loop data, and no production defect confirmed.

## 6. Accepted limitations retained (must not be promoted)

- Three pure-Beech reduced-simulator R2 misses remain accepted against the unchanged 0.40 criterion.
- MFM-11 extension-state calibration remains GAMEPLAY CALIBRATION.
- Post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.
- F11 candidate workload remains a human/user-test obligation.
- Fit results remain recomputable rather than separately persisted; rule-only/invalid-fit rendered coverage remains as previously disclosed.

## 7. Cleanup and final state

- Final lane `/media/jer/ZX20/CCF-U1`: **clean including untracked**, HEAD `a95c4c4e35decb7a518d543488678473430f69b6`, production diff to `6849caa0` empty.
- No Unity Editor, Xephyr, openbox or native-input driver running.
- Machine-wide Unity lock **FREE** (`final-lock-status.json`); final release recorded.
- Host saves unchanged (`7effe9b7…` / `c0169790…`).
- No staged harness or `.meta` remains under `Assets/ForestPrototype/`.
- No Windows build; #194 NO ACTION.

## 8. Task 1 checkpoint / continuation

This is the Task 1 checkpoint. Automatic continuation is **STOPPED**. Task 2 of 12 is **NOT STARTED** and not authorized in this run. The durable continuation handoff is:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/TASK-1-CONTINUATION-HANDOFF-20261103.md`

## 9. Return to Browser Overall

This is **not** a Tier-3 pass. Recommended next action:

1. **Do not open a production repair yet.** Run the frame-separated Year-20 reference-preview round trip (entry, at least one frame, exit) with the protected-player fixture. Only that result distinguishes a normal-player restoration defect from the confirmed same-frame harness race.
2. Treat the Rack map-planning “five tabs” assertion as stale against the current six-tab UI; do not change production for it. Re-run the Rack 1920×1080 rotation with an uncontaminated pointer trace before classifying it.
3. Produce a usable rendered Completed-state checkpoint at both resolutions before any player-test readiness claim.
4. After those evidence items, rerun only the affected gates and the mandatory S1-C T-05 chain on the new exact candidate; the rest of the retained reduced plan above can be reused only where its production surface is byte-identical.

Local success token **not issued**:

`SCENARIO ONE TIER-3 COMPLETE-LOOP QUALIFIED — READY FOR INDEPENDENT FINAL REVIEW`

Browser Overall token **not issued**:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

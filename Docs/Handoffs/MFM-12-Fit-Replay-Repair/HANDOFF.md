# MFM-12 Fit-Replay Review Integration — HANDOFF

**Claim:** #230  
**Parent:** #225 MFM-12 / Browser Overall #200 / local orchestration #203  
**Date:** 2026-11-02  
**Verdict:** `MFM FIT REPLAY REPAIR COMPLETE — READY FOR MANAGER REVIEW`  
**MFM-12 product token:** **NOT ISSUED.** The full MFM-12 qualification is not restarted or claimed by this repair.

## 1. Identity

| Field | Value |
|---|---|
| Exact starting integration HEAD | `b4f79c6f4cf8e30a5fc74dd4ce7f8f5f1aa7f789` |
| Branch | `fix/mfm-fit-replay-review-v1` |
| Worktree / Unity lane | `/media/jer/ZX20/CCF-U1` |
| Tested production HEAD | `a5a1483f8a8a9f84630169dd4de151cf2a670611` |
| Orchestration | `opencode-go/deepseek-v4.1-flash` |
| Unity execution | `codex / gpt-6.1-sol` |
| Independent reviewer | Claude Sonnet 5.5 (cross-provider; see §9) |
| Evidence root | `/media/jer/Files/CCF/Verification/MFM12-FitReplayRepair-20261102/` |
| Unity lock | FREE at handoff; last lock `0fff64dce10c41e49316bc70c4ea3a04`, released normally |

The starting head is the integration head after the MFM-12 STOP Manager review. The production bytes under it are the stopped MFM-12 candidate `508b3e81`; the newer commits are management/docs only.

This HANDOFF is added by a docs/tooling commit above the tested production HEAD. `git diff a5a1483f..HEAD -- Assets Packages ProjectSettings` is empty.

## 2. What changed

### 2.1 Production (four files, exact diff `b4f79c6f..a5a1483f`)

| File | Change |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` | New side-effect-free `ReadPrescriptionFit(int, out string)`; the manager obtains the current replay from the existing `CheckFit` core using the stored sample ids/removals and the same live geometry/stems/masks as approval. `ApprovePrescription` now also refuses with `FitUnavailable` when a valid current replay is unavailable, after the existing prescription/proposal validation and before any write. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescriptionApproval.cs` | Adds the `FitUnavailable` refusal code and carries the live `Geometry` / `Masks` in the existing approval context. No rule, threshold or proposal semantics changed. |
| `Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs` | Adds plain `FitView` / `FitRow` / `FitCropRow` mapping from the authoritative `FitResult`: agreement count, player-only and rule-only rows with tree/Share/basal-area context, per-Crop-Tree player-vs-rule release, overall basal-area shares, ceiling note, honest invalid state; the approval gate now requires a valid fit view. No fit algorithm is reimplemented. |
| `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs` | New Treatment Review fit section (heading, comparison framing, summary, consequences, player-only/rule-only/agreement cards, Crop Tree rows, invalid state); the approval button and handler re-evaluate the gate with a fresh fit read. |

### 2.2 Verification tooling / tests (separate from production)

| File | Change |
|---|---|
| `Tools/Verification/MfmTreatmentReview/OfflineLogic/TreatmentReviewOfflineCheck.cs` | New fit mapping/gate assertions (all-agree, player-only, rule-only, Shares/removal consequence, unavailable/invalid fit, approval blocked, deterministic repeat, no second fit algorithm; gate tests now supply a fit view). |
| `Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py` | Boundary pin advanced to the exact repair base and the repair files added to the allowed surface; checks are not weakened. |
| `Tools/Verification/MfmTreatmentReviewUnity/MfmTreatmentReviewUnityGate.cs` (new) | Live fit gate: derives the replay from the confirmed sample, checks individual disagreements, consequences, invalid fit, approval refusal with no mutation, restoration, and captures fit shots at both sizes. Reused from the accepted MFM-08 post-integration gate; only fit assertions/shots added. |
| `Tools/Verification/MfmTreatmentReviewUnity/run_gate.py` (new) | Runner for the above; stages the gate/native input, isolated config/licence, cleans up. |
| `Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs` | Stale save-version assertion v23 → accepted v24. |
| `Tools/Verification/MfmSaveProvenance/run_mutation_controls.py` | Stale M14 v23 target → v24 so M14–M25 execute. Two further stale targets surfaced while doing so (M18 and M24, both broken by the same accepted hash-layout change) and were mechanically repointed without weakening intent; all 25 mutants now run and are killed. |
| `Docs/Project/coordination/active-tasks.md` | Refreshed from live coordination and the #230 row marked RUNNING. |

No save-schema, calibration, Rack V2, species scope, ecology, economy, candidate/coverage, Work Plan or protected Reference/Legacy40 file changed.

## 3. Before / after

**Before:** The player could prepare and approve a wider prescription without ever seeing how the derived rule reproduced their own sample treatment. The tested `CheckFit` core was offline-only.

**After:** Treatment Review shows, before approval:
- the framing “This compares the rule derived from your sample treatment with the decisions you actually carried out. It is a comparison, not a score of forestry correctness.”;
- an agreement/disagreement summary;
- each player-only and rule-only sample removal as its own row, with the affected tree, its Crop Tree/Share context and basal area;
- per-Crop-Tree player-vs-rule release and overall player-vs-rule basal-area shares, plus the ceiling note;
- an explicit invalid/unavailable state when the current replay cannot be read.

The approval gate refuses when the current fit replay is invalid or unavailable. Prescription approval still creates ordinary Pending work only; the Work Plan remains the commitment gate; proposals remain distinct from Fell marks; player judgement and candidate/coverage semantics are unchanged.

## 4. Verification evidence

### 4.1 Offline / static

| Gate | Result |
|---|---|
| MFM treatment-review offline (final) | `MFM08_TREATMENT_REVIEW_PASS checks=312 failed=0`; byte-identical repeat; `MFM08_BOUNDARY_PASS changed=11 outside-owned-surface=[]` |
| MFM-04 prescription core | `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`; byte-identical repeat |
| MFM-07 approval boundary | `MFM07_APPROVAL_BOUNDARY_PASS checks=125 failed=0`; byte-identical repeat |
| MFM-08A coverage world | `MFM08A_COVERAGE_WORLD_PASS checks=102 failed=0`; byte-identical repeat |
| MFM-06 save mutation controls | `mutants=25 killed=25 survived=0`; `MFM06_MUTATION_CONTROLS_PASS` |
| Production typecheck | No new error in the changed files; only the known baseline approximation drift (`Painter2D`/legacy API) in unrelated files. Unity compile below is authoritative. |

### 4.2 Unity / runtime (exact HEAD `a5a1483f`)

| Gate | Result |
|---|---|
| Compile | PASS — `MfmCompileGate`, Unity exit 0, `pass: true`, lane clean, host saves unchanged, harness/licence removed |
| MFM Treatment Review + fit | **`MFM08_UNITY_PASS checks=412 shots=29`** |
| MFM-07 approval boundary | `MFM07_UNITY_PASS checks=106` |
| MFM-08A coverage world | `MFM08A_UNITY_PASS checks=56` (v24 assertion) |
| guided-v2 | **535 PASS / 0 FAIL** (`GUIDED_ONBOARDING_PASS`) |
| Combined HUD v2 | **`CHC_DONE pass=465 fail=0 shots=24`** |
| PARITY-07 rendered | **`PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0`** |

The fit gate live results (from `02f-fit-review/unity.log`):
- `current source-sample fit replay is available`;
- `reading the fit replay changes no mark, order, event, id, cash or hash`;
- `the fit replay contains sample removal decisions [agree=18 playerOnly=1 ruleOnly=0]`;
- `the sample fixture produces a player-only disagreement to show [1]`;
- `player-only row is individually inspectable [P0716]`;
- `per-Crop-Tree player-vs-rule Share is shown`;
- `a changed sample makes the current fit replay invalid` and `wider approval is refused ... with no mark/order/event/id/cash change` (the live stale-sample case reached the existing sample-validation refusal; the new manager `FitUnavailable` branch is code/UI covered but not separately live-exercised — see N1 in §10);
- `restoring the sample restores a valid current fit replay`.

The fit fixture deliberately includes one player removal outside every Crop Tree’s 8 m reach so the rendered comparison has a genuine player-only disagreement. The replay itself is the unchanged core.

### 4.3 Rendered evidence

`unity/02f-fit-review/evidence/` contains 29 captures; 14 at 1280×720 and 15 at 1920×1080 (the Work Plan pending capture is intentionally one size). The fit-specific captures are:

| Capture | Sizes | Shows |
|---|---|---|
| `07a-fit-comparison-1280x720.png`, `07a-fit-comparison-1920x1080.png` | both | fit heading, comparison framing, agreement/disagreement summary, player-vs-rule basal-area consequences, player-only row and agreed rows; no critical clipping/overlap |
| `07b-fit-disagreement-1280x720.png`, `07b-fit-disagreement-1920x1080.png` | both | the individual player-only removal row (tree, direction, basal area) fully readable |
| `10-approve-ready-*.png` | both | approval state with the valid fit in place |
| `11-approved-pending-*.png` | both | post-approval Pending tasks, unchanged Work Plan boundary |

The full gate also captures the pre-existing review states (sample/coverage/candidate/proposal/conflict/3D/Work Plan) at both sizes, preserving the accepted distinctions.

## 5. Verification-tool debt repaired

- **MFM Coverage World** save-version assertion now asserts the accepted v24 schema (was stale v23).
- **M14** mutation now mutates the accepted v24 header to an invalid 25, so M14–M25 execute. While running the expanded suite, **M18** and **M24** were also found to have stale text from the same accepted hash-layout change; their targets were mechanically repointed to the current equivalent lines, preserving intent. Result: 25/25 mutants killed, no survivors.
- **MFM-08 Treatment Review offline boundary** now pins the exact repair base and allows only the repair’s production/tooling/docs surface; the behavioural 312 checks still pass and the boundary reports no outside-owned-surface changes.

These are verification-tool changes only. No assertion was weakened, no expected product behaviour was changed and no safety/compatibility threshold moved.

## 6. Boundaries / not changed

- No save schema, migration, old-save or protected Reference Future/Legacy40 change.
- No calibration, TargetShare, 30% first-release, 40% cumulative safeguard, Hegyi radius, candidate spacing, coverage tolerance or species scope change.
- No Rack V2 geometry/world identity change.
- No candidate confirmation, auto-confirm, bulk automation, proposal-as-Fell or Work Plan commitment change.
- No ecology, economy, timber, deadwood or regeneration change.
- No Rack/minimap source or style change; the Combined HUD regression covers its rendered contracts and no separate Rack/minimap gate was required.
- No Windows build. #194 NO ACTION. #225 was not restarted.

## 7. Unity lock lifecycle / cleanup

Lock acquisitions were normal, never forced:
- `382623e23c2e42ca8b36902d5347aef7` (first compile, released after the tooling compile fix);
- `61bdb03ef36b4b8b91a54ee72f318911` (compile/fit iteration, released);
- `57143a78ee3d4a96b8f9fb725f19d803` (viewport iteration, released);
- `b48740c15ad14c44b301a9e004752d0d` (heading iteration, released);
- `305a22bc1acf4bd6be4914abf74c4680` (approval refusal ordering, released);
- `0fff64dce10c41e49316bc70c4ea3a04` (final compile/fit/approval/coverage/guided/combined/parity chain), released normally.

Final state: lock **free**, metadata None, no Unity Editor PID. Lane clean at the tested production HEAD; known Unity import side effects (`.vscode/settings.json`, five SectionFive materials) were restored byte-exactly after every run; no staged harness or `.meta` remains. The failed harness attempts and compile-error runs are preserved under the evidence root and are not counted as passes.

## 8. What was not verified

- Full MFM-12 qualification, save/load/old-save/malformed-save, complete Scenario One loop, two-cycle provenance/history, Legacy40/Reference Future live preservation and full rendered 720p/1080p matrix are **not** part of this repair and were not run. They remain for the fresh #225 repin after this repair is accepted/integrated.
- The MFM-09 extension gate was not rerun separately: the repair does not touch the teaching/extension production surface, and guided-v2 (535/0) plus the MFM-07/08 approval/treatment gates cover the changed review/approval surface.
- Human workload (F11) and the user’s personal playtest remain external gates.
- The typecheck approximation is not a Unity compile; the Unity compile pass above is the compile evidence.

## 9. Independent review

**Reviewer:** Claude Sonnet 5.5 (Anthropic), cross-provider to the `gpt-6.1-sol` implementation. Read-only; no Unity, no lock, no repository edit.

**Verdict:** `APPROVED`.

The reviewer independently checked the exact four-file production diff, confirmed the unchanged `CheckFit` core is reused and that the UI only maps its result, confirmed the manager/UI approval boundary and that no other caller bypasses it, opened the 07a/07b PNGs at both resolutions, and matched every regression marker. It found no blocking issue.

Non-blocking review findings and their disposition are recorded in §10. Full review: `analysis/INDEPENDENT-REVIEW.md` under the evidence root.

## 10. Independent-review findings and dispositions

The reviewer's verdict is `APPROVED` with no blocking findings. Its non-blocking findings are retained here:

- **N1 — manager `FitUnavailable` not directly live-exercised.** The live stale-sample case is refused by the existing sample/approval validation (`SampleNotDerived`) before the new fit branch; the UI gate blocks with the fit reason, and the manager branch is code/UI-covered but not claimed as live-proven. This handoff does not claim otherwise.
- **N2 — early `NotAccepted` guard.** `ApprovePrescription` returns `NotAccepted` before building the approval context for an already-approved record. The refusal and result are the same as the existing path, but it runs earlier. This is disclosed; no valid/accepted path is affected.
- **N3 — unused allow-list entries.** The two unused production entries were removed from the treatment-review offline allow-list; the final boundary is `changed=10 outside-owned-surface=[]`.
- **N4 — coordination file.** The live `Docs/Project/coordination/active-tasks.md` is **not** part of the repair diff. The #230 RUNNING marker is on the claim; the live file remains #200-owned and should not be integrated from this branch.
- **N5 — rendered coverage.** The rendered evidence covers a valid fit with a player-only disagreement, the summary/consequences and the approval-ready state. Rule-only, the invalid-fit card and the disabled Approve button are exercised by the pure/live assertions but not separately screenshotted; no additional rendered claim is made.
- **N6 — run binding.** Runs 05 (guided-v2) and 06 (Combined HUD) have no head file of their own. `RUN-BINDING.md` in the evidence root records the single lock, exact HEAD, sequential session and timestamps; the reviewer accepted this as binding.
- **N7 — handoff publication.** This file is the required durable handoff for the repair claim.

## 11. Next action

Browser Overall reviews this repair and its independent review. If accepted, integrate #230 onto `integration/scenario-one-player-test-ready-v1` (production and verification commits; not the coordination file), freshly repin #225, and run the **full** MFM-12 matrix from the repaired candidate — not from Unity step 02 of the stopped run. Do not issue the MFM-12 product token from this repair handoff.

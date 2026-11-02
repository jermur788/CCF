# MFM-12 — Integrated MFM Qualification — RETURN

**Claim:** #225  
**Parent:** #91 / #200 / #221; local orchestration #203  
**Date:** 2026-11-02  
**Status:** **STOPPED — PRODUCTION/SPEC DEFECT RETURNED TO BROWSER OVERALL**  
**Verdict:** `MFM-12 QUALIFICATION STOP — PRODUCTION/SPEC DEFECT`  
**Success token:** **NOT ISSUED.** `MFM-12 QUALIFICATION PASS / INDEPENDENT REVIEW APPROVED / READY FOR MANAGER ACCEPTANCE` is **not** claimed.

MFM-12 is qualification-only. This return reports a blocking production/spec conformance finding and the partial evidence collected before the stop. No production, save-schema, calibration, protected-anchor, UI or test change was made.

## 1. Exact identity

| Field | Value |
|---|---|
| Exact production candidate | `508b3e8124b5d9c477a4d2471cd56ebf6608f197` (`integration/scenario-one-player-test-ready-v1`) |
| Qualification branch | `qual/mfm-12-integrated-v1` |
| Qualification HEAD | `95c12be59f7db0253a524cbaf923c1cc03671911` |
| Worktree / Unity lane | `/media/jer/ZX20/CCF-U1` |
| Production diff candidate → qualification HEAD | **empty** (`git diff 508b3e81..95c12be5 -- Assets Packages ProjectSettings`) |
| Orchestration | `opencode-go/deepseek-v4.1-flash` |
| Unity execution | `codex / gpt-6.1-sol` (xhigh, priority) |
| Static conformance analysis | `codex / gpt-6.1-sol` (high) |
| Independent reviewer | Claude Sonnet 5.5 (see `INDEPENDENT-REVIEW.md`) |
| Evidence root | `/media/jer/Files/CCF/Verification/MFM12-IntegratedQualification-20261102/` |
| Unity lock | FREE at handoff; last lock `c2caa8620ee6436aaac675f597bd2574`, released normally after the compile stop (the independent reviewer inspected the earlier held snapshot) |

The qualification HEAD adds verification tooling, binding docs and one bounded coordination-row refresh only. It does not change production.

This HANDOFF and `INDEPENDENT-REVIEW.md` are a docs-only commit above the tested qualification HEAD; they change no production file. The tested HEAD remains `95c12be59f7db0253a524cbaf923c1cc03671911`.

## 2. Blocking finding S1 — sample fit replay is disconnected from the player flow

**Requirement.** Spec §7.3 requires that, before wider approval, the derived rule is applied back to the original sample and the player can see how closely it reproduces the player's own treatment: an agreement/disagreement count or equivalent clear summary, **which sample decisions the rule would have made differently**, and the relevant Share/removal consequences. Checklist **E4** (fit check replays the rule against the source sample), **E5** (fit disagreements are inspectable, not hidden behind one score) and **J7** (player can inspect disagreements and uncovered areas) carry this obligation.

**Production fact.** `ScenarioOnePrescription.CheckFit` implements the required replay and the `FitResult` lists (`Agreements`, `PlayerOnly`, `RuleOnly`, `CropTrees`) at `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescription.cs:143-273`. A repository-wide search of production `Assets` finds **only the definitions and the internal overload forwarding call at line 179**; there is **no caller in the manager, treatment-review UI, approval path or any other production file**. The live review UI builds Sample / Legend / Areas / Candidates / Prescription (`Assets/ForestPrototype/UI/StandMapTreatmentReview.cs:199-203`); `BuildSample` (207-214) shows the sample title/metrics/source note, and `BuildPrescription` (335-363) shows the spot-check/proposals/approval — not a rule-replay fit report. `ScenarioOneManager.PrescriptionApproval.cs:216-270` reads coverage and derives the prescription without invoking `CheckFit`. The area rows show unit-comparison deltas (“density/DBH/competition against the sample”), which are unit similarity, not rule-vs-player-removal agreement.

**Why this is a production/spec defect, not merely missing evidence.** The required player-visible fit report does not exist on this candidate. The teaching copy in `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs` says the game “derives a prescription from the decisions you actually carried out, checks it against your own sample, and shows which rack-served areas are covered…”. The player can reach prescription approval without ever seeing the required rule-replay agreement/disagreement or Share/removal consequences. A Unity run cannot make an absent production surface appear; re-running the offline `CheckFit` core only proves the core itself, which was already known to pass.

**Stop disposition.** Per the MFM-12 stop conditions, the qualification stops here and returns the defect to Browser Overall. If any production repair is authorised, it must be a separate bounded packet; it must not be made under #225. No production repair was attempted.

**Localisation.** `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescription.cs:143-273` (core exists), `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs:216-270` (live read/derive, no fit call), `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs:199-363` (review UI, no fit report), `Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs` (fit data types not consumed by the UI).

## 3. What was executed before the stop

### 3.1 Offline / static matrix (U1, exact HEAD)

| Step | Result | Marker / key evidence |
|---|---|---|
| 01-site | PASS | `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true` |
| 02-growth-integrated | PASS | `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` |
| 03-timber | PASS | `THREE_SPECIES_TIMBER_PASS checks=73` |
| 04-density | **ACCEPTED LIMITATION** | `THREE_SPECIES_OFFLINE_PASS checks=91`; only `COMPOSITE_DENSITY_FAIL checks=107 failures=3` = the accepted pure-Beech R2 misses `0.423 / 0.407 / 0.417` against the unchanged `0.40` |
| 05-history | PASS | `THREE_SPECIES_HISTORY_PASS checks=74` |
| 06-pruning | PASS | `PARITY06_PRUNING_PASS checks=3946 failures=0 NOT_UNITY_VERIFIED`; preservation 17 files; 1296 legacy-differential cases |
| 07-prescription-core | PASS | `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`; byte-identical repeat |
| 08-candidates | PASS | `MFM05_CROP_TREE_CANDIDATES_PASS checks=95 failed=0`; byte-identical repeat |
| 09-coverage-world | PASS | `MFM08A_COVERAGE_WORLD_PASS checks=102 failed=0`; byte-identical repeat |
| 10-approval-boundary | PASS | `MFM07_APPROVAL_BOUNDARY_PASS checks=125 failed=0`; byte-identical repeat |
| 11-treatment-review | **ACCEPTED LIMITATION** | Behavioural half `MFM08_TREATMENT_REVIEW_PASS checks=297 failed=0` + byte-identical repeat; `MFM08_BOUNDARY_FAIL` is the known stale `cfdc93bf` base pin that lists later integrated production as outside MFM-08's old file set. No product claim is made for the boundary half. |
| 12-save-provenance | PASS | `MFM06_SAVE_OFFLINE_PASS checks=172` |
| 13-save-mutation-controls | **ACCEPTED LIMITATION (partial execution)** | M01–M13 KILLED; the run stops on `MUTANT TEXT NOT UNIQUE/PRESENT: M14 wrong schema number (0)` because the mutation still targets `CurrentVersion = 23` after the accepted v24/D-066 schema. **M14–M25 did not execute**, so this is not a full mutation pass. No pass is claimed; malformed/old-save rejection was **not** covered because the Unity save gate did not run. |
| 14-parity08-save | PASS | `PARITY08_SPECIES_OFFLINE_PASS checks=23` |

Exact logs and summary: evidence root `summary.json`, `offline-summary.json`, `logs/offline/`, `offline/`.

### 3.2 Unity evidence actually collected

| Step | Result |
|---|---|
| `unity/01-compile` | **PASS** — `MfmCompileGate`, Unity exit 0, `pass_marker: true`, no failures, harness/`.meta` removed, licence copy removed, host saves unchanged, remaining lane changes empty, `pass: true` (87.15 s) |
| `unity/02-approval` … `17-history-l40` | **NOT RUN** — stopped after the S1 finding; no further Editor was launched |

The stopped coordinator and clean state are recorded in `matrix-stop-receipt.json`, `after-state.json`, `lock-after.json` and `failure-*.json`. No staged harness, `.meta`, Xephyr, openbox or Unity Editor remains. The lane is clean at `95c12be5`; the lock is free.

## 4. What is therefore not verified

- live Rack-derived sample/coverage geometry and coverage reasons on the exact candidate;
- prescription determinism and live fit replay;
- candidate confirm/reject/manual alternative and the 3D player-judgement boundary;
- proposal vs Fell state and the Work Plan commitment boundary;
- ordinary contractor/economy execution and durable prescription provenance;
- save/load, pre-feature/old saves and malformed-save rejection-before-mutation on the exact candidate (the offline suites cover parts, but the Unity save gate did not run);
- rendered 1280×720 and 1920×1080 UX;
- BroadleafTimber Legacy40/Enlarged80, MixedDensity, pruning Unity, ThreeSpecies history, guided-v2, Combined HUD, PARITY-07 and Rack rendered regressions on the exact candidate;
- exact-candidate integration regression for safeguard/economy/ecology/timber/deadwood/regeneration/light;
- provenance/history across at least two management cycles in one world;
- one explicit complete Scenario One loop through later intervention and completion;
- the independent review verdict on the full package (only the STOP finding is reviewed here).

None of these may be described as PASS on the strength of inherited evidence alone.

## 5. Inheritance and unchanged-surface basis (what may still be cited)

`3acdf9cb635cdca79606335801e97f9d7dcce8b0` (accepted MFM-11 production) is an ancestor of `508b3e81`. The only production path changed between them is `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` (the accepted #224 layout repair, +13/−10). `ad9a8f76`/`7e7ba702` are verification/docs; `95db20ae`/`9a4f8014`/`508b3e81` are the #224 USS repair and docs.

This supports careful reuse of unchanged-surface evidence, not a blanket “previously passed”:
- MFM-11 accepted calibration and its explicit limitation: extension-state CI delta `4.169829%` within the accepted 20% **GAMEPLAY CALIBRATION** tolerance; **post-treatment source-vs-current-Covered CI parity remains NOT DEMONSTRATED** (treated units become `PlayerCompletedWork` exclusions; later sample qualification can refuse). No robustness claim is upgraded.
- MFM-10 accepted `SPECIES GUARD QUALIFIED` on `6784ab17` for the pure species-guard surfaces; the later #199/#224 UI changes do not touch the species rule.
- MFM-09 accepted extension/Work Plan evidence on `6784ab17` (`MFM09_UNITY_PASS checks=261 shots=16`; `MFM08_UNITY_PASS checks=373 shots=25`; `MFM07_UNITY_PASS checks=106`) remains relevant to the unchanged extension/Work Plan UI/manager surfaces, but it is not fresh exact-candidate evidence for MFM-12.
- Rack V2/PARITY-09 protected compatibility may be cited by exact ancestry where the relevant save/reference surfaces are unchanged, but the exact-candidate save/reference gates did not run.
- All rendered UI evidence is fresh-required because `ScenarioOneUi.uss` changed at #224.

The full per-item classification and unchanged-surface checks are in `analysis/STATIC-CONFORMANCE.md`, `analysis/checklist-matrix.json` and `analysis/GAPS.md`.

## 6. Checklist disposition (A–P)

| Section | Disposition at this stop |
|---|---|
| A Architecture | Static/architecture evidence only; no runtime correctness inferred. |
| B Scenario One prerequisites | B1/B2 inherited by ancestry; B3/B4 fresh-required and **not run**. |
| C Existing teaching | Constants/authority inherited; fresh guided/teaching regression **not run**. |
| D Sample and coverage | Fresh-required and **not run**. |
| E Prescription derivation | E1/E2/E3/E6/E7/E8 static/inherited support; **E4/E5 blocked by S1**. |
| F Crop Tree candidates | Fresh-required and **not run**. |
| G Species honesty | MFM-10 inherited; fresh guard regression **not run**. |
| H Proposal state | Fresh-required and **not run**. |
| I Approval/work execution | Fresh-required and **not run**. |
| J Explainability | J1–J6 fresh-required and **not run**; **J7 blocked by S1**. |
| K Persistence/compatibility | Fresh-required and **not run**; protected compatibility only cited by ancestry. |
| L UX | Fresh-required and **not run**. |
| M Calibration/safety | MFM-11 accepted result and its limitation carried; exact-candidate integration regression **not run**. |
| N Teaching/scope | Static/process evidence only; fresh guided evidence **not run**. |
| O Evidence/wording | Static inspection only; rendered copy spot review **not run**. |
| P Final gate | P1–P5 **not met**; P6 is a later user-playtest gate and is not self-issued. |

## 7. Other observations (non-blocking at this stop)

- `MfmCoverageWorldUnityGate.cs:166` still asserts save version 23 while the candidate is version 24. This is stale verification wiring, not a product defect; it was scheduled but the Unity run did not reach it.
- `Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py` pins the old MFM-08 boundary base and therefore reports later integrated production as outside its file set; its behavioural half passed 297/0.
- `Tools/Verification/MfmSaveProvenance/run_mutation_controls.py` has a stale v23-specific M14 mutation pattern. M01–M13 were killed; M14–M25 were never executed because the runner stops at M14. No full mutation pass is claimed.
- The MFM-11 calibration witness logs completion/cash/planting diagnostics without failing the gate on an absent completion; any future use of that witness must read the raw outcomes, not only the green marker.
- F11 candidate workload is a human/user-test obligation and was not manufactured here.
- The compile gate's log/patch records the known tracked `.vscode/settings.json` Editor rewrite; the gate restored it and reported `remaining_lane_changes: ""`. No product finding is inferred.

## 8. Clean return state

- Lane `/media/jer/ZX20/CCF-U1` clean at `95c12be5` including untracked files.
- No staged harness or `.meta` under `Assets/ForestPrototype/`; the compile gate removed its harness and licence copy.
- No Unity Editor, Xephyr or openbox process remains.
- Unity lock released normally after the STOP (the independent reviewer saw the pre-release held snapshot); final state `free`, metadata None, no Editor PIDs.
- Rendered screenshots: **0 PNGs**; no rendered gate ran.
- No production, package, ProjectSettings, save-schema, calibration, protected-anchor or test change.
- No Windows build. #194 remains NO ACTION.
- Coordination row refreshed with the local RUNNING identity; the qualification status is now returned as STOP.

## 9. Recommended next action

Browser Overall should:

1. accept this STOP as a returned production/spec defect, not a qualification pass;
2. decide whether to authorise a separate bounded repair packet for connecting the existing `CheckFit` replay to the player-visible review flow (E4/E5/J7) without changing the accepted rule, coverage, calibration, save schema or protected anchors;
3. after any accepted repair and a fresh repin, re-run the full MFM-12 matrix, including all Unity steps that did not run here;
4. keep the MFM-11 boundary intact: extension-state calibration within its gameplay-calibration tolerance; post-treatment source-vs-current-Covered CI parity **NOT DEMONSTRATED**;
5. not issue the product token `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` from this return.

## 10. Exact commands (for reproduction)

Offline matrix (from U1; no lock needed):

```bash
python3 -B Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head 95c12be59f7db0253a524cbaf923c1cc03671911 \
  --lock-id c2caa8620ee6436aaac675f597bd2574 \
  --owner codex-gpt-6.1-sol-MFM-12 --phase offline
```

Unity phase (was stopped after compile):

```bash
python3 -B Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head 95c12be59f7db0253a524cbaf923c1cc03671911 \
  --lock-id <held-MFM-12-lock> \
  --owner codex-gpt-6.1-sol-MFM-12 --phase unity
```

The Unity phase was not completed; the table in §3.2 is the actual executed set.

## 11. Independent review

**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic) — cross-provider to the gpt-6.1-sol qualification and static-analysis roles. Read-only; no Unity, no lock, no repository edit.

**Verdict:** `NEEDS CHANGES`.

The reviewer independently reproduced S1 from source and confirmed the STOP was correct:
- `CheckFit`/`FitResult` exist at `ScenarioOnePrescription.cs:143-273` but have no production caller outside the internal overload;
- `CreatePrescription`/`ApprovePrescription` never call it;
- the treatment-review UI (`StandMapTreatmentReview.cs:207-430`) has no agreement count, player-only/rule-only list, or Share/removal comparison;
- the approval gate (`ScenarioOneTreatmentReview.cs:433-453`) does not require fit;
- `ScenarioOneTeachingSequence.cs:79` claims the check is shown when it is not;
- the omission is a task-decomposition gap: `tasks.md` assigns fit only to MFM-04, and no later task owned its presentation.

The reviewer also recorded these non-blocking findings, all retained here:
1. **Step 13 wording.** M01–M13 ran and were killed; M14–M25 were not executed because the runner stops at the stale M14. "13/13 applicable" understated the 12 unrun mutants. The Unity save gate that was said to cover them did not run.
2. **Step 11** is correctly a non-pass: a stale `cfdc93bf` boundary pin; the behavioural half passed 297/0 and cannot evidence fit.
3. **Step 04** is the accepted three pure-Beech R2 limitation, unchanged `0.40`.
4. **Fit is not persisted.** `PrescriptionSaveData` stores no fit result. Spec §15 says history must eventually answer "what fit was reported"; the saved sample/removals and deterministic `CheckFit` make it recomputable, but any S1 repair should decide this deliberately.
5. **Lock timing.** The reviewer saw the lock still held; it was released normally by the orchestrator immediately after the review snapshot, and final status is `free`.
6. **Compile side effect.** The known tracked `.vscode/settings.json` rewrite was disclosed, reverted and recorded; the lane returned clean.

The reviewer's full findings and its "what I did not verify" section are in `INDEPENDENT-REVIEW.md` in this folder.

No PASS token is issued. The product token `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` is not claimed and remains Browser Overall's decision after a repair and a fresh qualification.

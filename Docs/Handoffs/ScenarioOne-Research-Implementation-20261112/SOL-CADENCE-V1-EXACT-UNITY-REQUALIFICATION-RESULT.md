# Claim #404 — exact-candidate Unity requalification: FAIL

GPT-6.1 Sol / OpenAI / medium, Codex harness in T3 Code. Sole worker; no subagents, production edits, merge or Player build. Execution 2026-11-12 09:34–09:42 UTC.

Candidate production HEAD `9ce4ab6bddd5279d8c032641fbc6fe83548c597e`, tree `a95c5beee5233130a62ede3476470f3673f011d8`; origin confirmed via GitHub git-ref REST API. Lane ResearchCopyQual391 `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`, branch `qualify/scenario-one-cadence-v1-after-harness-repair-20261112`. Started at617a954978e84e5f35b404bab6f9a427a56640f5; lease/run HEAD2e80c98fd21b6c2b23cec4ddae823da0a741ec81 (runner-only commit). Production Assets diff empty at acquisition and end. Final report/runner commit is the tip of this branch; publication receipt records its full SHA externally.

## Reproducible candidate failure / stop

F-8 reproduced TWICE, from independent reloads of the authentic fixture in the same Editor run. Real skip completed Year4→6, then `ForestSaveController.LoadData(original,false)` loaded the different original Year0 world. `ui.ShowReview()` displayed the previous world's session summary. Screenshots show “Current Year 0” alongside “advanced Years 5–6”. No summary field was injected. This is the task's reproducible candidate-failure stop condition. No production fix permitted in this claim. NOT QUALIFIED; no merge.

Exact output, both attempts:

    CADENCE404_F8 attempt=1 sourceYear=6 loadedYear=0 summary=Session only: advanced Years 5–6. Suggested reassessment year reached. Browse earlier years below; only the latest review can be acknowledged.
    CADENCE404_F8 attempt=2 sourceYear=6 loadedYear=0 summary=Session only: advanced Years 5–6. Suggested reassessment year reached. Browse earlier years below; only the latest review can be acknowledged.
    CADENCE403_FAIL System.InvalidOperationException: CANDIDATE DEFECT F-8 reproduced twice: previous-world skip summary survives different-world load

Reproduction: finish genuine lesson7; acknowledge current review; open Work Plan and complete a skip; load a different saved world through LoadData; open Annual Review. Expected: old session-only range cleared. Actual: old range remains. Source: `AnnualReviewView.SetSkipSummary` retains label text; `ScenarioOneUiRoot.FinishSkip` sets it; one-year advancement clears it, world loading does not. Test invokes genuine game action handlers directly, not native keyboard/mouse input. Final runner defaults to parity; `CCF_CADENCE_F8=1` selects reproduction. The small final dispatcher wrapper was not rerun; staged-source hashes preserve the exact executed bytes.

## Fixture repair

Runner now calls the player's actual `InspectTree` handler (reflection only to invoke the private handler), followed by real Learning `ObserveRackInspection` and `DecideRackInspection(false)` handlers for P0706/P0714/P1109 and six retained stand portions. These actions earn `firstGuidedRackReviewComplete`; no fields, events, acknowledgments or save credits fabricated. Then genuine stock purchase/designation, planting execution and establishment-year review produce stage7. Both runs logged stage7 and empty SkipProblem. All CadenceParity assertions unchanged. Initial runner54 checks PASS.

## Eight-row matrix

| Row | Verdict | Subcases and evidence |
|---|---|---|
| 1 Compile/import | PASS scoped runtime import | Both Editor runs compile/import exact production tree with no error CS. Pre-existing Search exception classified below. No separate fresh batch import this claim. |
| 2 Parity A strict | NOT RUN in full; quiet 5/5 PASS | Quiet N1,2,3,4,5 each passed exact pre-open JSON/hash and every per-tick canonical JSON/hash assertion. Post-felling 0/5 NOT RUN. Per-year hash values were compared but not printed by unchanged CadenceParity: no numeric hash evidence claimed. |
| 3 Parity B semantic | NOT RUN in full; quiet 5/5 PASS | Genuine one-year UI advance/ack route, only acknowledgment/event-ID normalization, quiet N1..5 PASS. Post-felling 0/5 NOT RUN. |
| 4 Stops/negatives | NOT RUN in full | Quiet target stop/exact range/restoration, double-click and destination genuine acknowledgment passed in five parity runs. Other nine-type pre-disabled/exit fixtures, stages0..6, Pending/Approved/MFM/invalid, unread review, preview/authoring, modal/session/help/warning/rack introduction/consent/reset/reopen/drag/ghost, unavailable ecology/controller, rack problems, Failed/100/closed plan/paused all NOT RUN. Objective, Completed, Failed, storm events/victims,25/100, lesson, controller loss, exception, failed tick k, OnDisable, cancel0/k, RejectWrongStopReason NOT RUN. No invented PASS. |
| 5 MFM/rack safety | NOT RUN | Historical accepted prescription replay, pending/approved creation, automatic Fell mark/rack selection/3D confirmation and F9 actual Crop Tree mortality comparison NOT RUN. |
| 6 Compatibility | NOT RUN | Genuine Legacy40/F10, frozen anchors0/20/50/100, archive identity, destination save/reload, old/no-history saves and schema/HashLayout runtime checks NOT RUN. No protected archive/save modified. F8 different-world in-memory load was run, not a disk-save compatibility suite. |
| 7 UI/performance | FAIL F8 twice | Two reviewed rendered captures of contradictory range/year. F7, required720p/1080p native-input matrix, confirmation/progress/cancel/year browser and4-year/5-tick separate latency measurements NOT RUN. Total runtime1=264s, runtime2=112s are whole Editor runs, not skip timings. |
| 8 Safety | PASS |37 host hashes equal; one normal lease released after Editor exit; own drift preserved/restored; no disposable Assets/metas remain; Assets identical; disk above floor. Final clean-state receipt follows publication. |

All NOT RUN cases have no execution output: execution stopped on reproduced F8. There is no full-matrix PASS and no numeric per-year hash list. The five quiet runs are useful partial evidence, not qualification completion.

## Logs, screenshots, commands

Evidence root: /media/jer/Files/CCF/Verification/CadenceV1-UnityRequal-404-20261112T093358Z

- `runtime1/cadence.log`, `result.json`, `summary-lines.txt`, staged-source hashes: parity run exit0, no reaper,54 fixture checks; CADENCE PARITY A+B PASS requested=N actual=N for N1..5.
- `runtime2/cadence.log`, `result.json`, staged-source hashes: F8 twice, Editor exit1, no reaper. Expected intentional gate failure, not unexplained crash.
- `runtime2/evidence/F8-attempt1.png`, `F8-attempt2.png`: both opened and visually inspected; nonblank, forest and Annual Review rendered; stale5–6 range against Year0. Actual texture dimensions recorded in screenshot receipt. These are not claimed as required native-input size-matrix captures.
- `runtime1-drift.patch` and `editor-drift.patch`: own Editor settings and three Forb materials restored from candidate, never committed.
- `custody-before.json`, `custody-after.json`: exact37 paths/hashes; equality asserted by finalize driver.
- `acquire.json`, `acquire-command.json`, `release.json`, `no-editor-before-release.json`, `final-lock.json`, `final-commands.jsonl`, `disk-before.json`, `final-state.json`: custody receipts and cleanup command exit codes.

Commands (all evidence paths relative to root above):

    source /media/jer/ZX20/CCF-Ops/environment.sh #0
    scripts/lane-status --storage-only #0 PASS
    scripts/unity-lock status --json #0 free, no Editor
    gh api repos/jermur788/CCF/git/ref/heads/feature/scenario-one-cadence-dual-advance-v1-20261112 --jq .object.sha #0 exact SHA
    gh api contents/...active-tasks.md?ref=main -H Accept:application/vnd.github.raw #0; assignment404 confirmed
    git diff 9ce4ab6b -- Assets #0 empty
    git commit runner #0 2e80c98f
    python3 -B preflight.py #0 acquired actual HEAD;37 hashes
    CCF_GATE_LANE=<lane> python3 -B run403.py --name cadence --method Cadence403Verification.Begin --files CadenceDualAdvance/CadenceParity.cs CadenceDualAdvance/Cadence403Verification.cs --out <root>/runtime1 --pass-token CADENCE403_PASS --fail-token CADENCE403_FAIL --timeout 900 #0 Editor0
    # Save own drift patch and restore four files;0. Modify only own runner for F8.
    # First runtime2 driver attempt1: DIRTY LANE: refuse staging (exit1, before Editor).
    # Correct allowance to exact own runner; repeat same command with --out <root>/runtime2 #1 Editor1; F8 twice
    python3 -B finalize403.py #0; pgrep1(empty), Assets diff0(empty), hashes match, release0

The driver/task id and short TMPDIR were adapted from403; evidence hashes identify executed sources. Held ONE lease across both Editors; no commit while held. Runtime2 tested an uncommitted own runner revision, explicitly allowed only that path by driver; actual HEAD remained truthful. Final runner/report committed only after release. No production edits or changed assertions.

Search classification: runtime1 line1617 and runtime2 line1574 have UnityEditor.Search.SearchDatabase ArgumentOutOfRangeException. Same older stack at `/media/jer/Files/CCF/Verification/RackV2-09-20261029/field-04/field.log:1600–1611`. Not attributed to cadence. Licensing “Access token unavailable” was followed by successful entitlement resolution and successful launches. No human terms/licensing action required. UI renderer stack output in runtime2 also retained; no additional Exception header beyond classified Search and intentional test failure was identified. No blanket clean-console claim beyond this classification.

## Safety receipts

Lease `c40cf58540294b90a6cf7f463aa71b2e`, acquired09:34:10.689937Z against2e80c98f, owner GPT-6.1 Sol Medium, task CCF-404-CADENCE-V1-UNITY-REQUAL. No-Editor anchored pgrep exit1 at09:41:43.151279Z; normal release immediately afterwards; final lock free/no PIDs. Release time bounded by that proof and final-state09:41:43.736621Z (release tool has no timestamp field). Lock history supplies precise release timestamp if required.

ZX20 before20,446,183,424 bytes; post-first-import sample20,449,460,224; end20,446,969,856; floor16,106,127,360. No continuous disk-monitoring claim. Host37 hashes unchanged. Original in-memory world restored in finally in both runs. Primary's unrelated AGENTS.md/StorageArchitecture.md preserved. Protected lanes untouched. Final qualifier lane clean after report/runner commit, captured externally.

Immediate next step: coordinator dispositions F8 and assigns any production correction separately, then requalifies the exact changed candidate and completes outstanding matrix. Dashboard/Drive downstream synchronization NOT VERIFIED; coordinator owns closeout. No integration.

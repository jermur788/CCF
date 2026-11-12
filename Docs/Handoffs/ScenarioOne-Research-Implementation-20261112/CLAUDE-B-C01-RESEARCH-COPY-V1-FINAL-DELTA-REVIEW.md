# Claude B — C01 research-copy v1 — FINAL DELTA REVIEW (issue #392)

Reviewer: Claude B, **Sonnet 5.5 (claude-sonnet-5-5), low reasoning effort**, no subagents, no Unity, no builds, no source edits.
Date: 2026-11-12. Candidate: **a314d308315b5894263d30461a1e8b52ca0f3605** (feature/scenario-one-research-copy-v1-20261112). Previous approved source: 868643a1. Baseline: b62446ca. Author report: f4bd3a02 (report branch).

## Verdict: APPROVED for candidate a314d308 (source and the F391-01 rendered fix)

Approval is scoped. The Work Plan clearance row now renders correctly (my own reading of the author's captures). Several acceptance checks stay NOT RUN or UNDETERMINED, as the author says; they are listed in section 5 and should not be called closed.

## 1. Delta 868643a1..a314d308

`git diff --stat`: two files. `WorkPlanOverview.cs` has 1 line changed. The other file is the author implementation report (+15 lines).

- The only code change is `, valueBelow: true` added to the `UiKit.Line` call for "Clearance, square {cell}" at `WorkPlanOverview.cs:192-193`. The wording is byte-identical to 868643a1.
- `UiKit.Line` (`UiKit.cs:82-89`) adds class `table-row-stacked` when `valueBelow` is true. USS (`ScenarioOneUi.uss:146-148`) sets `flex-direction: column`, resets name grow/basis, and left-aligns the value. A second rule at `uss:636` is scoped to `.annual-headlines`. Only rows built with `valueBelow` are affected.
- Existing callers: `WorkPlanOverview.cs:170` (Deadwood volume, in the same card) and the Annual Review headlines. The pattern is established, not new.
- Dependencies on the old side-by-side structure: grep over `Assets` and `Tools` finds only `WorkPlanResidualVerification.cs:210`. It builds the label string and asserts `leaving.Contains(label)` against flattened text (`AllText`), so it does not depend on row layout. No verifier or UI code references the row's class structure.
- Is a layout argument acceptable under "copy/presentation only"? **My judgement: yes, narrowly. It does go beyond string edits.** The spec non-goals and plan section 4 exclude behaviour, objectives, saves and ecology. They name panel clipping as a stop condition, and that is the defect that occurred. The change touches no model, save, hash or ScenarioOne code, and it reuses an existing helper argument. Unity captures showed the original defect was real, so I rate the fix proportionate. The coordinator should record it as an explicit authorised exception, which the author already did.

## 2. Whole diff b62446ca..a314d308

Nine files: six under `Assets/ForestPrototype/UI` (AnnualReviewView, LearningObjectivesView.Curriculum, MenuHelpView.Text, ScenarioOneTeachingSequence, ScenarioOneUiFacts, WorkPlanOverview), two verifier pins (`P3/WorkPlanResidualVerification.cs`, `ScenarioOneTeachingCopyVerification.cs`), and the author report. Nothing under `Assets/ForestPrototype/ScenarioOne`. No save, objective-definition or hash code. No scene, prefab, meta or USS change. Protected identity holds at the source level. The only non-string production change in the whole diff is the one argument above.

## 3. Evidence audit (author-run captures; I did not produce them)

I extracted the PNGs from f4bd3a02 and looked at them.

| Image | My reading |
|---|---|
| before-fix-clearance-young 1280x720 and 1920x1080 | Defect confirmed. "Clearance, square A1" is one character per line (about 20 lines), with the value squeezed to the right. This is clearly unacceptable. |
| attempt4-clearance-young 1280 and 1920 | Fixed. "Clearance, square A1" is on one line. The value "affects 3 natural young-tree group portions and 0 planted saplings" sits below it on two lines. The A2 label and value are fully visible. No overlap or cut-off. |
| attempt4-clearance-empty 1280 and 1920 | A2 label and "no natural young-tree groups or planted saplings inside" are fully visible. At 1280 the density paragraph is clipped at the scroll edge, which the author states. |
| attempt4-clearance-density 1280 and 1920 | The density paragraph is fully visible ("Relative young-tree density… a portion can remain after clearing."). Note: the density frames are visually the same scroll position as the young frames (apparent duplicates). This is harmless, because the content is correct, but they are not independent views. |
| attempt4-clearance-top 1280 and 1920 | Work Plan top, Money and "What you are leaving" rows are aligned and intact. No spill from the changed row. |
| help-share-passage 1280, help-walking-passages 1920 | The changed Share and mixed-species paragraphs are fully visible. At the Walking HUD bottom the "Keys:" line is cut at the scroll edge (ordinary scrolling, not changed text). |
| objectives-goals 1280 | "Plant, with species present — beech/Sessile oak" rows and "Young trees present in 0 areas" are fully visible. |
| learning-topic2-full 1280 and lesson-release-why 1280 | The Topic 2 sentence and the "Why it matters" passage are fully visible. |
| annual-young-objective 1920 | "Objective first reached: Areas with young trees" is fully visible. Top-edge scroll clipping affects only the preceding line. The left half of the pane is blank. It looks the same as in the Work Plan frames and is not from this patch. I flag it only as an unexamined layout oddity. |

The author's table reading (labels about 5 px wide before, about 517 px after; A1 on two lines; A2 on one) matches what I saw.

Internal consistency of the report:
- Help, objectives, curriculum, lesson, Annual Review, Reference and anchors are credited to **868643a1** (attempt 3). Clearance, residual P3 and offline checks are credited to **a314d308** (attempt 4). The report states this split explicitly in its verdict table. It does not claim Help and related checks on the new head.
- Lease IDs are listed for all attempt-3 and attempt-4 sessions, each released without force with final FREE. The nine host save and Prefs hashes are reported equal before and after.
- The Reference anchors table in the report shows eight lines, and the report says nine matched. I did not re-verify the ninth line. The eight shown equal the quoted #381 values.
- Overclaim check: none found. The author labels the clearance fixture as legacy test state, keeps baseline-save, Oak/Beech and nursery as NOT RUN / UNDETERMINED, and says Help etc. were not rerun on the new head.

## 4. Commands I ran

- `git fetch origin`; `git checkout --detach a314d308`; diffs as above.
- `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out <scratch>`: **PASS 157**; all five mutants KILLED; "Composition/source contracts PASS (not render proof)". Matches the author's number. This is a source-slice check, not a rendering test.
- Greps in section 1.

## 5. Acceptance checks and evidence class

I did not map AC-1..AC-8 to the spec text one by one. The classes below use the check areas as the author and spec describe them.

| Check | Class |
|---|---|
| Source scope, copy-only diff, protected paths untouched (diff scope) | PASS by my own verification |
| Offline presentation/composition checks (157, mutants) | PASS by my own verification (re-run) |
| Real-Editor compile, P3 WorkPlanResidual gate on a314d308 | PASS on author Unity evidence (marker lines audited; I did not re-run Unity) |
| Clearance rendering at 1280/1920 on a314d308 | PASS on author evidence I audited visually |
| Help, goals, curriculum, lesson, Annual Review text visibility | PASS on author evidence, **on 868643a1 only**. Carry-over to a314d308 is a source-scope inference (see below) |
| Reference Future anchors and #381 comparison | PASS on author evidence on 868643a1; reasonable carry-over (no model code changed) |
| Baseline-save CurrentWorldHash comparison | NOT VERIFIED (NOT RUN) |
| Oak/Beech "first reached" variants | NOT VERIFIED (NOT RUN) |
| Old S1A `nursery warning visible` assertion | UNDETERMINED / NOT VERIFIED |
| Windows/Player build, dashboard ingestion | NOT VERIFIED (out of packet) |

Residual-risk judgement:
- **Carry-over of Help and objectives to a314d308.** The delta touches one argument in `BuildClearance` in `WorkPlanOverview` only. Help, objectives, curriculum and Annual Review use different files. I judge the carry-over reasonable but unproven, and the author does not claim otherwise.
- **Nursery assertion.** The assertion is at `ScenarioOneTeachingCopyVerification.cs:198`. The warning element is built in `WorkPlanView.cs:585` (`nursery-warning-<itemId>`), not in `WorkPlanOverview`. The test scrolls to that element and checks bounds overlap. The stacked-row change alters the height of the "What you are leaving" card above it by roughly one row, which can shift scroll offsets but not the element. Making `ScrollTo` fail would need the warning to be absent or hidden, and nothing in this diff does that. So I judge the patch an implausible cause. I could not prove it, because it needs a baseline rerun. The earlier failure also followed five black captures, which confounds it. The author's UNDETERMINED label is right. I would not call the nursery assertion a blocker for this copy patch, but it should not be reported as passing.
- **Baseline-save hash.** No model, save, serialization or hash code changed, so the risk is low. It is still unverified. Run it before any integration claim that depends on saved-world identity.
- **Oak/Beech variants.** The first-reached label goes through `ScenarioOneUiFacts.ObjectiveName` (a string mapping). Low risk, unrun.

## Findings

| ID | Severity | Location | Finding | Smallest fix |
|---|---|---|---|---|
| F392-01 | Info | `WorkPlanOverview.cs:192-193` | One-argument layout change goes beyond pure copy edits; justified by a real rendered defect and an existing helper | Record it as an explicit exception in the integration note |
| F392-02 | Low | author report | `clearance-density` frames duplicate the `clearance-young` scroll position at both sizes | None needed; content is verified in them |
| F392-03 | Low | author report | Reference anchors table shows eight lines vs "nine" matched in prose | Note only |
| F392-04 | Low (process) | residual gaps | Baseline-save hash, Oak/Beech, nursery assertion remain unrun or undetermined | Run before an integration claim; do not describe them as passed |

No blocking findings.

## Not verified by me
I ran no Unity, no rendering and no verifier other than `run_checks.py`. Everything about runtime behaviour rests on the author's logs and screenshots, which I inspected but did not produce.

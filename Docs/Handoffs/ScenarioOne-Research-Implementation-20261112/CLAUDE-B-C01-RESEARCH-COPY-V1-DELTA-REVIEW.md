# Claude B delta re-review — #392 Research Copy V1 (rework of #391 candidate)

**Reviewer:** Claude B, model Sonnet 5.5 (`claude-sonnet-5-5`), standard effort (not raised). Did not author the patch; no subagents.
**Unity: NOT RUN.** No Editor, Player, build, screenshot, save, PlayerPrefs, `/media/jer/ZX20/*` or the Sol operations folder was touched. Offline .NET tools only; outputs went to a private scratch directory.

**Reviewed:** `origin/feature/scenario-one-research-copy-v1-20261112` at report head `868643a1311ad79a759dbe18d4353f1b928ce2a7` (rework source `99af87ee`); baseline `b62446ca`; earlier review of `f32be4c5`.

## Verdict: APPROVED (source candidate)

All earlier findings are closed; no new defect found. **Not verified by me and still a separate gate:** rendered 1280×720 / 1920×1080 layout and runtime world-hash checks, plus running the two Unity verifiers (`WorkPlanResidualVerification`, `ScenarioOneTeachingCopyVerification`) in an Editor.

## Closure of earlier findings
| # | Earlier finding | Status | Evidence |
|---|---|---|---|
| 1 | MAJOR stale P3 pin | **Fixed** | `WorkPlanResidualVerification.cs:212` now `Contains("natural young-tree group portion")` / `Contains("no natural young-tree groups or planted saplings inside")`; both are exact substrings of `WorkPlanOverview.cs:192-193` (young case `…natural young-tree group portion{s}`, empty case `no natural young-tree groups or planted saplings inside`). Same strength, not weakened. Compiled in the offline route (below). |
| 2 | MINOR Oak/Beech label | **Fixed** | `ScenarioOneUiFacts.cs:92`; see hand-render. |
| 3 | MINOR inconsistent history label | **Fixed** | `AnnualReviewView.cs:714` uses `ScenarioOneUiFacts.ObjectiveName(match)`. |
| 4 | MINOR dropped "inspect neighbours" teaching | **Fixed** | `MenuHelpView.Text.cs:109` ends the Share passage with "Inspect which neighbours contribute most to the Share before deciding what to remove." |
| 5 | NOTE job vs group count | **Fixed** | `AnnualReviewView.cs:378` "N young-tree removal job(s) completed". |
| 6–8 | Notes (lesson framing, optional FR-C1 sentence, pre-existing offline failures) | Unchanged / acceptable | Not defects; RackV2HelpTeaching baseline failures not re-run (unchanged in nature; no Rack text changed in the delta). |

## Targeted risks
**(a) AnnualReviewView:714.** `m.Objectives` elements are `ScenarioObjectiveResult`; `ScenarioOneUiFacts.ObjectiveName(ScenarioObjectiveResult)` is `public static`, same assembly, qualified call so it does not bind to the private two-argument `ObjectiveName(ScenarioOneManager,string)` overload. Null-safe (`match != null ?` retained; fallback is the raw id). Only consumer is `:653` `AddTagged(... "Objective first reached: …")`, a display string; nothing stores, compares or parses it. Offline compile passes (191 sources, 0 errors). Pass.

**(b) Hand-rendered objective lines** (`displayName` = `"Establish planted " + speciesId`, `ScenarioOneObjectives.cs:93`; then Replace chain; `SpeciesName` fallback "sessile oak", beech stays "beech" — pre-existing):
- Open: `○ open · Plant, with species present — sessile oak: 0 / 1`; `○ open · Plant, with species present — beech: 0 / 1`
- Achieved: `✓ done · Plant, with species present — sessile oak: 1 / 1`; beech likewise.
- Same renderer in objective list (`LearningObjectivesView.cs:208`), Annual Review (`:725`, `:820`); Century row (`:831`) `Plant, with species present — sessile oak: yours 1 · target 1`; first-reached `Objective first reached: Plant, with species present — sessile oak`.
Truthful: predicate is succeeded PlantJuvenile work/completed order AND `livingTrees+regenerationCells+plantedJuveniles>0` (`:86-94`); wording says planting plus presence, promises no survival/establishment. Grammar is slightly telegraphic (imperative fragment, em dash, then colon) but unambiguous and acceptable. Nit only.

**(c) "removal job(s) completed".** `regenerationRemovalTasks` is incremented once per completed order at `ScenarioOneManager.cs:2087` (clearance) and `:2116` (RemoveRegeneration), i.e. per job. Wording is exact.

**(d) Topic 2.** "When inspecting competition Share, smaller neighbours usually contribute less to the measured Share." Accurate for the Hegyi term (neighbour DBH / target DBH / distance); natural enough (slight repetition of "Share", nit). The old guard "being small is not by itself a reason to remove" is carried by Help ("not … a verdict that one particular neighbour must go") and Lesson 3 / HUD cycle; no meaning lost that the spec required.

**(e) Verifier pins.** `ScenarioOneTeachingCopyVerification.cs:92` pins `"smaller neighbours usually contribute less to the measured Share"`, which is an exact substring of Help `:109`, with the other two original pins retained. P3 pins match shipped strings (see #1). Neither is weakened.

## Stale-wording grep (Assets + Tools, all file types)
Searched every replaced wording in the full diff (cohort(s) removed, regeneration cohort(s)/cohorts, natural seedling cohort, Planting completed, Being small / suppressed … by itself, "Cohorts are", "Relative regeneration density"). Remaining hits are unrelated C# comments/tooltips/validation messages about the data model (`ForestEcologyController`, `ForestSaveValidation`, `ClearanceVerification`, etc.), not player copy or matchers. No stale pin remains.

## Scope and protected identity
`git diff --name-only b62446ca 868643a1`: six UI files (`AnnualReviewView`, `LearningObjectivesView.Curriculum`, `MenuHelpView.Text`, `ScenarioOneTeachingSequence`, `ScenarioOneUiFacts`, `WorkPlanOverview`), two verifier files, the author report. `git diff b62446ca 868643a1 -- Assets/ForestPrototype/ScenarioOne` is empty (0 bytes). Word-diff is string literals plus the display-only `ObjectiveName` call; no save, serializer, objective logic, hash input or layout change.

## New findings
None blocking. Nits (no action required): Century/objective label telegraphic grammar; "Share … Share" repetition in Topic 2.

## Acceptance criteria
| AC | Result | Basis |
|---|---|---|
| AC-1 Share explanation | PASS | Help `:109`, Topic 2, Lesson text; Hegyi accurate |
| AC-2 Residual regeneration vocabulary | PASS | WorkPlan, Annual Review, job count wording truthful |
| AC-3 Oak/Beech label non-promising | PASS | hand-render above |
| AC-4 General-only environments | PASS | unchanged from first review |
| AC-5 Wind limitation | PASS | unchanged |
| AC-6 No protected-identity change | PASS (source) | ScenarioOne diff empty; display-only |
| AC-7 Layout / world-hash / screenshots | NOT VERIFIED | Unity not run; separate gate |
| AC-8 Verifiers consistent | PASS offline; Unity run NOT RUN | pins match shipped strings; both files compile |

## Commands and real results
- `git fetch origin`; `git checkout --detach 868643a1…` OK.
- `git diff --word-diff f32be4c5 868643a1 -- Assets Tools` and `git diff --word-diff b62446ca 868643a1 -- Assets Tools`: read in full (string-only + one method call).
- `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src $PWD --out <scratch>`: **PASS 157**, five mutants KILLED, composition contracts PASS (source slices; not render proof).
- `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src $PWD --out <scratch> --extra Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs Tools/Verification/ScenarioOneTeachingCopyVerification.cs` (tool read first; reads Unity DLLs from `/home/jer/ccf-verify-player/.../Managed`, writes only to `--out`): **`sources=191 refs=116 exit=0 errors=0`**. Offline syntax/type gate, not Unity compile; the Editor-guarded body of the teaching-copy verifier is excluded by this route, so its pins were verified by reading, not compiling.
- Greps for old/new wording, `ObjectiveName(`, `ObjectiveLine(`, `regenerationRemovalTasks`: as above.
- **NOT RUN:** Unity, Player build, either Unity verifier at runtime, runtime world-hash, 1280×720/1920×1080 renders, RackV2HelpTeaching offline suite.

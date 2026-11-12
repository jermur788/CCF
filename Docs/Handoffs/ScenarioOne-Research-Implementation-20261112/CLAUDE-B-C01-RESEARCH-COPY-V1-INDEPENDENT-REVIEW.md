# Claude B independent review — #392 Research Copy V1 (candidate for #391)

**Reviewer:** Claude B, model Sonnet 5.5 (`claude-sonnet-5-5`), standard effort (not raised). Did not author the patch; no subagents.
**Unity: NOT RUN.** No Editor, Player, build, screenshot, save, PlayerPrefs or `/media/jer/ZX20/*` access. All checks are source reading plus offline .NET tools. Rendered/layout/runtime-hash criteria are NOT VERIFIED.

**Reviewed:** branch `origin/feature/scenario-one-research-copy-v1-20261112`; source commit `02cd3ec9084a0327dddf61075fa5bb49f0907568`; report head `f32be4c5bd7711ab93d92f249f4f02b4f21a3bb1`; baseline `b62446caf40fc2ac939d55a6bea6e201f41de700`. Spec: the four files on `origin/docs/claude-a-fable-orchestration-research-copy-20261112-v1`.

## Verdict: NEEDS CHANGES

The patch is correctly scoped, display-only, truthful on Share/wind/regeneration, and protected identity is untouched. One **major** defect (a Unity verifier pin made stale by the patch and not disclosed) plus small copy fixes. All fixes are string-only.

## Findings

1. **MAJOR — stale verifier pin, Unity check would FAIL.** `Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs:212` requires `leaving.Contains("cohort portion")` (young case) and `leaving.Contains("no regeneration cohorts or planted saplings inside")` (empty case). The patch changed both strings in `WorkPlanOverview.cs:192-193` to "natural young-tree group portion(s)" / "no natural young-tree groups or planted saplings inside". The author changed only one verifier pin (`ScenarioOneTeachingCopyVerification.cs:92`) and the report claims the verifier grep was clean. Nothing offline exercises this file, so it is invisible until a Unity run.
   *Smallest fix* (same file, line 212): `young ? leaving.Contains("young-tree group portion") : leaving.Contains("no natural young-tree groups or planted saplings inside")`. Equivalent strength, no weakening.
2. **MINOR — new Oak/Beech label reads badly and contradicts its own state.** `ScenarioOneUiFacts.cs:92` produces lines such as `○ open · Planting completed; species present: sessile oak: 0 / 1` (double colon; "Planting completed" shown on an *open* line, where the predicate may be unmet on either half). Same label appears in the Century comparison row (`AnnualReviewView.cs:831`: "…: yours 1 · target 1"). Beech stays lower-case (`SpeciesName` fallback) — pre-existing. Truthfulness is fine: it does not promise survival; the predicate (`ScenarioOneObjectives.cs:~90-95`: succeeded PlantJuvenile work/completed order AND `livingTrees+regenerationCells+plantedJuveniles>0`) matches "planting done + species present".
   *Smallest fix:* `.Replace("Establish planted ", "Plant and keep present: ")`? — no, "keep" implies survival. Use `.Replace("Establish planted ", "Planting done and species present in the forest — ")` or, simpler, `"Plant, with species present: "` → `○ open · Plant, with species present: sessile oak: 0 / 1`. Any wording that is a verb phrase (not "completed") and avoids the double colon is acceptable; avoid "established/survive".
3. **MINOR — inconsistent label surface.** `AnnualReviewView.cs:711-715` (`ObjectiveName(m,id)` used at `:653` "Objective first reached: …") returns raw `displayName`, so history can still show "Establish planted sessile-oak" while objective lists show the new text. Pre-existing raw species id, but the patch now makes the two surfaces disagree.
   *Smallest fix:* `return match != null ? ScenarioOneUiFacts.ObjectiveName(match) : objectiveId;` (display-only; AnnualReviewView is already in the changed set).
4. **MINOR — teaching point dropped that the spec did not ask to remove.** `MenuHelpView.Text.cs:109` no longer contains "Inspect which neighbours contribute most to competition before deciding what to remove" nor "Nearby trees can reduce their growing space". FR-C1 asked to rewrite only the "being small" line.
   *Smallest fix:* append after the "…must go." sentence: ` Inspect which neighbours contribute most to the Share before deciding what to remove.`
5. **NOTE — "natural young-tree group(s) removed" counts jobs, not groups.** `AnnualReviewView.cs:378` prints `report.regenerationRemovalTasks` (incremented per completed task, `ScenarioOneManager.cs:2087,2116`); one task can cover several cohort portions. Previously "cohort(s)" had the same imprecision. Optional: "N young-tree removal job(s) completed". "Natural" is truthful: RemoveRegeneration targets natural cohorts; planted saplings are counted separately (see the Work Plan wording).
6. **NOTE — lesson framing precedes the exercise.** `LearningObjectivesView.Curriculum.cs` Topic 2 intro says "In this crown-release exercise, smaller neighbours usually contribute less to the measured Share" before Lesson 3 introduces the exercise. Acceptable; "usually" is correct for the Hegyi term (neighbour DBH / target DBH / distance). Dropping the explicit "not a reason to remove" guard is mitigated by the retained HUD cycle step 3 ("not simply the smallest trees").
7. **NOTE — optional FR-C1 sentence not added** ("This lesson teaches a game-calibrated…"). Lesson 3 already carries "gameplay calibration" (`ScenarioOneTeachingSequence.cs:~66`) and Help now says "30% target is this game's calibrated crown-release exercise". Spec marks the sentence a candidate; no defect.
8. **NOTE — pre-existing offline failures confirmed** (see commands): RackV2HelpTeaching fails S6 and ten step-6 checks identically on baseline and candidate; the candidate adds S10 only because that tool's whitelist predates #391 (3 vs 2 aggregate failures). Not caused by or masked by the patch, but the author's pin change is not covered by it.

## Checks performed (no issue found)
- **Scope (check 1):** `git diff --stat` = six UI `.cs` files (11+/11−), one verifier, one report. Word-diff shows string literals only (plus the single `.Replace`). No logic, layout, save, objective, Share, storm, MFM or history change. No `.meta`, scene, prefab or non-UI Assets change (`git diff --name-only -- Assets` outside `UI/` is empty).
- **Protected identity (check 2):** `ScenarioOneObjectives.cs`, `ScenarioReferenceArchive`, serializer/save untouched (zero diff). `ObjectiveName` callers: `ObjectiveLine` (display), `AnnualReviewView:831` (display); ID-keyed consumers (`LearningObjectivesView.Sequence.cs:102`, `ForestSaveValidation.cs:392` prefix `"introduced-"`) use IDs, not labels. Verifiers compare `ObjectiveLine` to itself (consistent). Nothing persists, hashes or parses the label.
- **Prefix/substring matchers (check 3):** `ManagementHistoryText.cs:71` and S1C provenance checks match "Areas with young regeneration", which is unchanged (`RegenerationAreasName` untouched). Repo-wide grep (Assets, Tools; cs/py/json/md) for old wording: only finding 1. `tagged_text.py` etc. do not pin changed strings.
- **Share truthfulness (check 4):** `ForestEcologyController.HegyiTerm` = (neighbour DBH / target DBH) / distance, cutoff `HegyiCutoffMeters = 8f`; `CompetitionShare.Measure` uses only DBH and distance. The new sentence (8 m, size-and-distance, does not assess species/crown/height/shade/light direction/timber/habitat/wind) is accurate; 30% framed as calibrated exercise; no "must remove" claim.
- **FR-C3:** one general sentence at `MenuHelpView.Text.cs:134` (Walking HUD help), after the ground-conditions sentence, no mapped-location class; the light "open/1 (open)" text is in a different (Annual/forest-light) paragraph at line 124, not adjacent. Acceptable; omits "check over time" tail, which was optional.
- **FR-C4:** exact sentence once (`:132`, grep finds one occurrence in Assets/Tools). New-game default: `ScenarioOneManager.cs:475` sets `StormModel.None`; `StormModel.None=0` = no windthrow. "Default" is accurate (a save or Editor override can enable `WindthrowV1`).

## Readability (check 5) — NOT VERIFIED by render
- Help: `MenuHelpView.cs:44` uses a vertical `ScrollView` inside `.menu-help-panel` (`max-height:92%`, `ScenarioOneUi.uss:328-337`), so the grown competition and HUD paragraphs scroll rather than clip (source reasoning only).
- Lesson 3 "Why it matters" is inside a collapsed `Foldout` "More detail" (`LearningObjectivesView.cs:165-168`); Curriculum topics are in a `ScrollView` (`:99`). Work Plan clearance rows grew by ~10 chars; Annual Review row is shorter/similar. I did not confirm Work Plan card or inspection/competitor labels at 1280×720 — requires Unity.

## Acceptance criteria
| AC | Result | Basis |
|---|---|---|
| AC-1 Share explanation (FR-C1) | PASS (finding 4 minor) | source + Hegyi/CompetitionShare read |
| AC-2 Residual regeneration vocabulary (FR-C2) | PASS with minor | WorkPlan/AnnualReview/map text truthful; findings 2, 3, 5 |
| AC-3 Oak/Beech label non-promising | PASS with minor | no survival claim; wording clumsy (2) |
| AC-4 General-only environments (FR-C3) | PASS | one sentence, no local class |
| AC-5 Wind limitation (FR-C4) | PASS | exact, once, default None |
| AC-6 No protected-identity change | PASS (source) | zero diff on objectives/save/archive; display-only mapping |
| AC-7 Layout/world-hash/screenshots | NOT VERIFIED | Unity not run; runtime hash not executed |
| AC-8 Verifiers/offline tests consistent | FAIL | finding 1 stale Unity pin; offline suites otherwise as baseline |

## Commands run and real results
- `git fetch origin`; `git checkout --detach f32be4c5…` — OK. `git diff --stat b62446ca f32be4c5` — 8 files, 132+/12−. `git diff --word-diff` over Assets/Tools — string-only.
- `git show origin/docs/claude-a-fable-…:specs/…/{spec,plan,tasks,checklists/requirements}.md` — read.
- Greps (`rg`-equivalent `grep -rn`) for old/new wording, `Establish planted`, `ObjectiveName(`, `introduced-`, `regenerationRemovalTasks`, `StormModel`, `HegyiTerm`, scroll/USS — results as above.
- `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src $PWD --out <scratch>` (candidate): **PASS 157**, five mutants KILLED, composition contracts PASS (source slices only; not Unity/render).
- `python3 -I -B Tools/Verification/RackV2HelpTeaching/run_offline.py --out <scratch> --base b62446ca` (candidate): FAIL — S6, S10, ten step-6 offline checks; `failed=3 static_and_offline=15`, offline `failures=10 checks=97`.
- Same tool on a detached baseline worktree `b62446ca` (scratch, removed after): FAIL — S6 + the same ten step-6 checks; `failed=2`, offline `failures=10 checks=97`. So all but S10 are pre-existing; S10 is the tool's file whitelist.
- Did not re-run the author's `compile_runtime_sources.py` route (not required for string-only literals; author reported PASS 189 sources, 0 errors — taken on trust, not independently re-verified).
- **Not run:** Unity, any Player build, `ScenarioOneTeachingCopyVerification` runtime, `WorkPlanResidualVerification` runtime, world-hash comparison, screenshots.

## Required before APPROVED
Fix finding 1 (required) and 2–4 (strongly recommended, string-only; changes 2–3 touch `ScenarioOneUiFacts.cs`/`AnnualReviewView.cs` only), then rerun presentation checks and the Unity copy/P3 verifiers when a Unity lane exists.

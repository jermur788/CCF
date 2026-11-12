# C-01 / #391 implementation result

Actual runtime: GPT-6.1 Sol, medium reasoning effort, Codex harness through T3 Code. Single implementer; no delegates.

Baseline: `b62446caf40fc2ac939d55a6bea6e201f41de700`. Final source commit: `02cd3ec9084a0327dddf61075fa5bb49f0907568`. The following report-only commit is identified by `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-IMPLEMENTATION-RESULT.md` (a report cannot contain its own commit hash). Branch: `feature/scenario-one-research-copy-v1-20261112`. Worktree: `/media/jer/Files/CCF/Operations/CCF-S1-ResearchCopy-V1-Sol`.

Implementation ready for independent review; not integrated or rendered-qualified. Only the six allowed UI files and one legitimate verifier string pin changed. No architecture, gameplay, layout, thresholds, ecology or persistence changes. The additional Replace call is presentation mapping in the existing ObjectiveName chain; it changes no predicate or stored label.

## Per-requirement exact changes

The tables quote exact changed C# source lines, including string contents and literal escape sequences. Unchanged interpolation expressions remain visible for review.

### FR-C2: `Assets/ForestPrototype/UI/AnnualReviewView.cs`

| Exact before | Exact after |
|---|---|
| AddTagged(c, HistoryProvenance.Recorded, $"{regenerationRemovals} regeneration cohort(s) removed"); | AddTagged(c, HistoryProvenance.Recorded, $"{regenerationRemovals} natural young-tree group(s) removed"); |

### FR-C1 / FR-C2: `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs`

| Exact before | Exact after |
|---|---|
| new Topic("2. Tree inspection and Crop Trees", "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.", | new Topic("2. Tree inspection and Crop Trees", "Begin with trees worth retaining and favouring, then inspect their competitors. In this crown-release exercise, smaller neighbours usually contribute less to the measured Share. There is no single correct tree picked by this lesson.", |
| new Step("map.regeneration", "Explore Regeneration", "Choose Regeneration. P counts living planted juveniles; the height is the tallest natural seedling cohort. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty square is information, not an instruction to plant."), | new Step("map.regeneration", "Explore Regeneration", "Choose Regeneration. P counts living planted juveniles; the height shown is for the tallest group of naturally seeded young trees. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty square is information, not an instruction to plant."), |

### FR-C1 / FR-C3 / FR-C4: `Assets/ForestPrototype/UI/MenuHelpView.Text.cs`

| Exact before | Exact after |
|---|---|
| + "Crop Trees are trees you choose to favour for the future. Nearby trees can reduce their growing space, but being small or suppressed does not by itself make a tree a problem. Inspect which neighbours contribute most to competition before deciding what to remove. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\n" | + "Crop Trees are trees you choose to favour for the future. In this crown-release exercise, smaller neighbours usually contribute less to the measured Share. Share is a local (8 m) size-and-distance indicator (DBH/distance, Hegyi). It does not itself assess species, crown shape, height, shade tolerance, light direction, timber quality, habitat or wind stability. The 30% target is this game's calibrated crown-release exercise, not a universal rule or a verdict that one particular neighbour must go. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\n" |
| + "Real transformations often take decades; yours will change one year at a time. In this game, mixing species does not by itself make the forest more productive; what it adds is variety, habitat and more than one kind of timber to sell. There is no single correct prescription, and the amounts, costs and targets in this scenario are gameplay calibration, not forecasts.\n\n" | + "Real transformations often take decades; yours will change one year at a time. In this game, mixing species does not by itself make the forest more productive; what it adds is variety, habitat and more than one kind of timber to sell. There is no single correct prescription, and the amounts, costs and targets in this scenario are gameplay calibration, not forecasts. The default Scenario One does not simulate wind damage.\n\n" |
| + "The forest is where you make management decisions. Look at ground to check light, regeneration and browsing conditions (browsing is deer eating the shoots of young trees), or press E (or left-click) while looking at a tree to inspect it. Use these observations before marking work.\n\n" | + "The forest is where you make management decisions. Look at ground to check light, regeneration and browsing conditions (browsing is deer eating the shoots of young trees), or press E (or left-click) while looking at a tree to inspect it. Use these observations before marking work. Beneath canopy, shade can limit growth; at gap edges, light and shelter vary; open spaces may get more light but also more competing plants.\n\n" |

### FR-C1: `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`

| Exact before | Exact after |
|---|---|
| "Compare neighbour relationships and competition Share around each retained tree. Selective removal can help more than one tree, but being small is not a reason to fell. Retain useful structure and exercise restraint.", | "Compare neighbour relationships and competition Share around each retained tree. Selective removal can help more than one tree. In this crown-release exercise, smaller neighbours usually contribute less to the measured Share. Retain useful structure and exercise restraint.", |

### FR-C2: `Assets/ForestPrototype/UI/ScenarioOneUiFacts.cs`

| Exact before | Exact after |
|---|---|
| return objective.displayName.Replace("sitka-spruce", SpeciesName("sitka-spruce")) | return objective.displayName.Replace("Establish planted ", "Planting completed; species present: ").Replace("sitka-spruce", SpeciesName("sitka-spruce")) |

### FR-C2: `Assets/ForestPrototype/UI/WorkPlanOverview.cs`

| Exact before | Exact after |
|---|---|
| UiKit.Line(card, $"Clearance, square {cell}", c.CohortGroups + c.PlantedSaplings == 0 ? "no regeneration cohorts or planted saplings inside"<br>: $"affects {c.CohortGroups} cohort portion{(c.CohortGroups == 1 ? "" : "s")} and {c.PlantedSaplings} planted sapling{(c.PlantedSaplings == 1 ? "" : "s")}"); | UiKit.Line(card, $"Clearance, square {cell}", c.CohortGroups + c.PlantedSaplings == 0 ? "no natural young-tree groups or planted saplings inside"<br>: $"affects {c.CohortGroups} natural young-tree group portion{(c.CohortGroups == 1 ? "" : "s")} and {c.PlantedSaplings} planted sapling{(c.PlantedSaplings == 1 ? "" : "s")}"); |
| UiKit.Add(card, $"Relative regeneration density in those cohort portions: {UiKit.F(c.CohortDensityBefore, "0.0")} → {UiKit.F(c.CohortDensityAfter, "0.0")} seedlings/ha equivalent. Cohorts are modelled groups, not individual seedling counts; a portion can remain after clearing.", "muted"); | UiKit.Add(card, $"Relative young-tree density in those group portions: {UiKit.F(c.CohortDensityBefore, "0.0")} → {UiKit.F(c.CohortDensityAfter, "0.0")} seedlings/ha equivalent. These are modelled groups, not individual seedling counts; a portion can remain after clearing.", "muted"); |

### FR-C1: `Tools/Verification/ScenarioOneTeachingCopyVerification.cs`

| Exact before | Exact after |
|---|---|
| Check(Has(tree, "Start with a tree worth keeping") &amp;&amp; Has(tree, "competitors") &amp;&amp; Has(tree, "suppressed does not by itself"), "positive selection without suppression prescription"); | Check(Has(tree, "Start with a tree worth keeping") &amp;&amp; Has(tree, "competitors") &amp;&amp; Has(tree, "smaller neighbours usually contribute less to the measured Share"), "positive selection without suppression prescription"); |

## Deliberate no-ops and protected strings

- FR-C2: `Areas with young trees` already ships in ScenarioOneUiFacts.RegenerationObjectiveName; retained byte-identically. No optional extra Help explanation was needed.
- `Areas with young regeneration` / `areas with young regeneration` in ScenarioEcologyReviewLines.cs, ManagementHistoryText.cs and RegenerationAreasName deliberately retained: history prefix parsing and protected history wording.
- ScenarioOneObjectives.cs and its `Establish planted ...`, `Maintain regenerating cells`, `Regenerating cells` remain byte-identical. The new display prefix `Planting completed; species present: ` describes the existing successful-planting plus species-presence predicate, without promising survival of originally planted trees.
- FR-C3 added one general sentence to Walking HUD Help, away from the Annual Review light-band `open` explanation; no mapped location classified.
- FR-C4 exact limitation sentence occurs once in Help. No competitor-panel changes.
- Existing lesson calibration statement (30% and 40%) retained; Help now connects the 30% exercise explicitly to its purpose. All IDs, calculations, recorded values and source hashes untouched.

## Commands and actual results

All shell commands ran in the designated worktree. Every Python invocation used `python3 -I -B`. Durable offline output: `/media/jer/Files/CCF/Verification/ResearchCopy-391/` (baseline and final subdirectories). Inspection commands below were read-only; some combined output was truncated by the tool, so relevant source/checker ranges were read again.

| Command / command group | Actual result |
|---|---|
| `pwd`; `git status --short`; `git branch --show-current`; `git rev-parse HEAD` | PASS: exact lane/branch, clean requested baseline |
| `cat AGENTS.md`; four `git show origin/docs/claude-a-fable-orchestration-research-copy-20261112-v1:specs/scenario-one-research-informed-management-learning-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` | PASS: binding scope inspected |
| `cat Docs/Project/{AgentWorkflow.md,decision-log.md,current-milestone.md,game-brief.md,unity-project-overview.md,research-index.md,coordination/active-tasks.md}` | PASS inspection; combined output truncated |
| `git remote show origin -n` | PASS read-only remote inventory (larger than needed) |
| `gh api repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md --jq .content` piped to `base64 -d`, also filtered by `rg` and `head -55` | PASS live read; historical top entry predates explicit dispatch, no competing writer found; dispatch grants this exact lane |
| `rg -n 'Share|smaller|cohort|Establish planted|Limit|canopy|regenerat'` on six allowed UI files; `rg --files Tools/Verification` filtered for relevant checkers/compile/offline | PASS inventory |
| `cat` MenuHelpView.Text.cs, ScenarioOneTeachingCopyVerification.cs, compile_runtime_sources.py, presentation README/run_checks.py/PresentationChecks.cs, Rack README/run_offline.py/RackV2HelpTeachingOfflineCheck.cs; targeted `sed -n` reads of Help, UiFacts and ScenarioOneObjectives | PASS inspection; no Unity checker executed |
| `rg -n 'Establish planted'` on ScenarioOneObjectives.cs; `rg -n 'suppressed does not|being small|cohort|Establish planted'` on relevant verifiers | PASS inspected predicate and exact verifier pin |
| `mkdir -p /media/jer/Files/CCF/Verification/ResearchCopy-391/{baseline,final}` | PASS durable output created |
| `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out /media/jer/Files/CCF/Verification/ResearchCopy-391/baseline/presentation` | PASS 157 checks, five mutants killed, composition contracts PASS |
| `python3 -I -B Tools/Verification/RackV2HelpTeaching/run_offline.py --out /media/jer/Files/CCF/Verification/ResearchCopy-391/baseline/rack --base b62446ca` | FAIL pre-existing: S6 wiring pin and ten step-6 wording checks; 2 aggregate failures / 15, offline 10 failures / 97 |
| Python literal-replacement edit script; second Python edit shortening the duplicated height wording | PASS; only intended presentation and one verifier pin |
| `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out /media/jer/Files/CCF/Verification/ResearchCopy-391/final/presentation` | PASS 157, five mutants killed, composition contracts PASS |
| `python3 -I -B Tools/Verification/RackV2HelpTeaching/run_offline.py --out /media/jer/Files/CCF/Verification/ResearchCopy-391/final/rack --base b62446ca` (twice, second after final curriculum edit) | FAIL same ten offline checks and S6; additional S10 scope whitelist excludes legitimate #391 files: 3 aggregate failures / 15. No rebaseline or weakened check |
| `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out /media/jer/Files/CCF/Verification/ResearchCopy-391/final/compile` | PASS sources=189 refs=116 exit=0 errors=0. Offline syntax/type gate, not a Unity or Player build |
| `command -v dotnet`; `ls /home/jer/ccf-verify-player/CCFVerify_Data/Managed/UnityEngine.CoreModule.dll` | PASS existing compiler/runtime references available |
| `cat global.json` | FAIL absent; not required by checker; actual compiler process used SDK 10.0.114 |
| `ps -eo pid,etime,args` filtered for dotnet/checker processes; `cat`, `tail`, `rg` on baseline/final logs | PASS progress/result inspection. Early reads of not-yet-created final logs returned absent; later reads succeeded |
| `git diff --check` (repeated); `git diff --numstat`; `git diff -- Assets/ForestPrototype/UI`; `git status --short` | PASS review; seven intended modified files before commit |
| `git diff b62446ca -- Assets/ForestPrototype/ScenarioOne 'Assets/ForestPrototype/*.cs'` | Broad quoted Git pathspec also included UI recursively; superseded by exact requested shell-expanded command below |
| `git diff b62446ca -- Assets/ForestPrototype/ScenarioOne Assets/ForestPrototype/*.cs` redirected to protected.diff; `wc -c` | PASS exactly 0 bytes |
| `git diff --stat b62446caf40fc2ac939d55a6bea6e201f41de700` | PASS seven allowed source/verifier files; reproduced below |
| `git add` exact seven files; `git commit -m "copy(#391): clarify Share and young-tree learning text"`; `git rev-parse HEAD` | PASS source commit recorded above |
| Python report-generation script (`git show`, `git diff --name-only`, `git rev-parse` reads) | PASS generated this exact-change report |
| Report commit; `git push -u origin feature/scenario-one-research-copy-v1-20261112`; `git ls-remote origin refs/heads/feature/scenario-one-research-copy-v1-20261112`; final `git status --short` | Completion receipts returned with handoff; report commit necessarily identifies itself via Git history |
| Unity Editor, ScenarioOneTeachingCopyVerification runtime, game/Player builds, runtime world-hash comparison, saves/PlayerPrefs, screenshots | NOT RUN as instructed; no save or other worktree accessed |

The runtime world-hash criterion remains NOT RUN: unchanged protected sources are source evidence only, not proof from a saved-world execution. No fixture/save was opened.

## Diff footprint at source commit

```
 Assets/ForestPrototype/UI/AnnualReviewView.cs                  | 2 +-
 Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs | 4 ++--
 Assets/ForestPrototype/UI/MenuHelpView.Text.cs                 | 6 +++---
 Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs       | 2 +-
 Assets/ForestPrototype/UI/ScenarioOneUiFacts.cs                | 2 +-
 Assets/ForestPrototype/UI/WorkPlanOverview.cs                  | 6 +++---
 Tools/Verification/ScenarioOneTeachingCopyVerification.cs      | 2 +-
 7 files changed, 12 insertions(+), 12 deletions(-)
```

## Render follow-up and open questions

At both 1280×720 and 1920×1080, capture Tree Inspection full Help (Share paragraph), Walking HUD full Help (limitations and general environment sentence), Objectives topic 2, map regeneration lesson, Lesson 3 concept, current Oak/Beech success objectives, Work Plan clearance consequences (empty and populated), and Annual Review work-done young-tree removal row. Open Help with F1 in the relevant context; scroll through the complete text. Expect all copy readable with no clipping/overlap and unchanged controls.

Help's Tree Inspection paragraph has the largest addition; Walking HUD Help adds 40 words; curriculum/lesson and objective label wrapping also need direct review. Existing Help is scrollable; no layout changes were made. Render fit at 720p/1080p is NOT VERIFIED. Independent reviewer/integrator must resolve visual acceptance and the existing stale Rack verifier gates. No source implementation blocker remains; release acceptance is pending review, rendered checks and any required integration qualification. No issues or active-task records edited; dashboard synchronization is left to the coordinator and is NOT VERIFIED.


## Rework 1 — 2026-11-12

Source commit: `99af87eeabcdb40b9b36269197198742c944ffb0`. Starting head: `f32be4c5bd7711ab93d92f249f4f02b4f21a3bb1`. Same authorised branch/lane, clean before editing; no unrelated changes. This section supersedes the earlier statement that no source blocker remained and corrects the earlier incomplete verifier search. Review read from `origin/review/scenario-one-research-copy-v1-claude-b-20261112`; approved spec and requirements read from the specified docs branch. No subagents.

### Findings and exact replacements

| Finding | Before | After / outcome |
|---|---|---|
| 1 Major, stale P3 pins | `leaving.Contains("cohort portion")`; `leaving.Contains("no regeneration cohorts or planted saplings inside")` | `leaving.Contains("natural young-tree group portion")`; `leaving.Contains("no natural young-tree groups or planted saplings inside")`. Same conditional assertions, exact new wording, stronger full young-group phrase. No weakened check. |
| 2 Objective label | `Planting completed; species present: ` | `Plant, with species present — `. Deliberate punctuation deviation from reviewer’s simpler colon suggestion: that suggestion still produces two colons in ObjectiveLine. Verb phrase fits open/done states and promises no survival. |
| 3 First reached display | `return match != null ? match.displayName : objectiveId;` | `return match != null ? ScenarioOneUiFacts.ObjectiveName(match) : objectiveId;`. One-line display-only mapping. Assets/Tools search for `Objective first reached` and `ObjectiveName(m,` found only the display emission at AnnualReviewView:653, no parser. Stored/comparison/hash/lookup code unchanged. |
| 4 Help restoration | `must go. On a Crop Tree` | `must go. Inspect which neighbours contribute most to the Share before deciding what to remove. On a Crop Tree`. Reviewer’s exact sentence and placement; “Share” follows the reviewer’s narrower wording rather than the original “competition”. |
| 5a Job count | `{regenerationRemovals} natural young-tree group(s) removed` | `{regenerationRemovals} young-tree removal job(s) completed`. Read AnnualReviewView:347 and ScenarioOneManager.ResolveRegenerationRemoval: both completion paths increment regenerationRemovalTasks once per completed job (cell clearance or species removal), not once per group. |
| 5b Topic 2 | `In this crown-release exercise, smaller neighbours usually contribute less to the measured Share.` | `When inspecting competition Share, smaller neighbours usually contribute less to the measured Share.` Stands alone before Lesson 3; narrow measured-Share statement retained. Existing teaching verifier pins only the unchanged `smaller neighbours usually contribute less to the measured Share` substring, so no additional pin edit required. |
| 5c Optional fuller FR-C1 Help | omitted | Remains omitted as instructed. Existing calibration explanation retained. |

### Source-derived display examples (not Unity-rendered evidence)

Both species, all affected places below. Open/done examples use current values 0/1 respectively; the current objective predicate remains successful planting plus species presence somewhere.

**sessile oak**

- Objectives list, open: `○ open · Plant, with species present — sessile oak: 0 / 1`
- Objectives list, achieved: `✓ done · Plant, with species present — sessile oak: 1 / 1`
- Annual Review objective list, open: `○ open · Plant, with species present — sessile oak: 0 / 1`
- Annual Review objective list, achieved: `✓ done · Plant, with species present — sessile oak: 1 / 1`
- Century / Year-100 current objective list, open: `○ open · Plant, with species present — sessile oak: 0 / 1`
- Century / Year-100 current objective list, achieved: `✓ done · Plant, with species present — sessile oak: 1 / 1`
- Annual Review first reached: `Objective first reached: Plant, with species present — sessile oak`
- Century / Year-100 comparison: `Plant, with species present — sessile oak: yours 1 · target 1` (or `reference 1` for frozen-reference comparison). Comparison rows have no open/done prefix; yours can also be 0.

**beech**

- Objectives list, open: `○ open · Plant, with species present — beech: 0 / 1`
- Objectives list, achieved: `✓ done · Plant, with species present — beech: 1 / 1`
- Annual Review objective list, open: `○ open · Plant, with species present — beech: 0 / 1`
- Annual Review objective list, achieved: `✓ done · Plant, with species present — beech: 1 / 1`
- Century / Year-100 current objective list, open: `○ open · Plant, with species present — beech: 0 / 1`
- Century / Year-100 current objective list, achieved: `✓ done · Plant, with species present — beech: 1 / 1`
- Annual Review first reached: `Objective first reached: Plant, with species present — beech`
- Century / Year-100 comparison: `Plant, with species present — beech: yours 1 · target 1` (or `reference 1` for frozen-reference comparison). Comparison rows have no open/done prefix; yours can also be 0.

### Commands and actual results

- `git fetch origin`, source/review/spec reads, `git status --short`, branch/head inspection: PASS, requested lane/head and no unrelated changes.
- Whole-repository tracked-text search excluding Docs (including Assets and Tools), using `python3 -I -B`: searched 24 removed literal/substrings from baseline diff plus old interior phrases. Evidence: `/media/jer/Files/CCF/Verification/ResearchCopy-391/rework1/old-wording-audit.txt`. Nine remaining matches are unchanged titles (`2. Tree inspection and Crop Trees`, `Explore Regeneration`, `Start with a tree worth keeping`), unchanged verifier description (`positive selection without suppression prescription`), row label (`Clearance, square {cell}`), and interpolation fragments. No remaining stale pin/matcher for replaced copy. Protected internal `Establish planted ` remains intentionally as the presentation mapping input and objective source; protected history vocabulary remains unchanged.
- `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out /media/jer/Files/CCF/Verification/ResearchCopy-391/rework1/presentation`: PASS 157 checks, all five mutants KILLED, composition/source contracts PASS.
- `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out /media/jer/Files/CCF/Verification/ResearchCopy-391/rework1/compile --extra Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs Tools/Verification/ScenarioOneTeachingCopyVerification.cs`: PASS `sources=191 refs=116 exit=0 errors=0`. Offline syntax/type compilation, not Unity or a Player build. P3 runtime body included; its Editor-only sections and the entirely UNITY_EDITOR-guarded teaching-copy verifier body are excluded by this non-Editor route. No claim of Editor compilation or runtime execution.
- `git diff --check`: PASS. `git diff b62446ca -- Assets/ForestPrototype/ScenarioOne`: empty. No objective/persistence/ecology/Share source changed.
- RackV2HelpTeaching: NOT rerun or repaired; earlier documented pre-existing failures remain outside scope.
- Unity Editor, builds, save/PlayerPrefs access, rendered screenshots, runtime verifiers and world hashes: NOT RUN. No ZX20 or other worktree access. Visual acceptance and runtime integration remain pending with the coordinator; no GitHub issue or active-tasks edit.

### Cumulative diff from baseline (including this report)

```text
 Assets/ForestPrototype/UI/AnnualReviewView.cs      |   4 +-
 .../UI/LearningObjectivesView.Curriculum.cs        |   4 +-
 Assets/ForestPrototype/UI/MenuHelpView.Text.cs     |   6 +-
 .../UI/ScenarioOneTeachingSequence.cs              |   2 +-
 Assets/ForestPrototype/UI/ScenarioOneUiFacts.cs    |   2 +-
 Assets/ForestPrototype/UI/WorkPlanOverview.cs      |   6 +-
 ...L-C01-RESEARCH-COPY-V1-IMPLEMENTATION-RESULT.md | 176 +++++++++++++++++++++
 .../P3/WorkPlanResidualVerification.cs             |   2 +-
 .../ScenarioOneTeachingCopyVerification.cs         |   2 +-
 9 files changed, 190 insertions(+), 14 deletions(-)
```

The stat above was captured immediately before appending this stat block; final report line totals increase accordingly. Report commit/pushed head is identified by Git history and the final handoff; push confirmation uses `git ls-remote origin refs/heads/feature/scenario-one-research-copy-v1-20261112`.


## Rework 2 — 2026-11-12

Finding **F391-01**: the longer approved clearance values squeeze “Clearance, square A1/A2” into vertical character stacks at 1280×720 and 1920×1080. Source fix `a9eef4da85e3b7ea4bd13172ff0b5daf9f4371d6` adds only `valueBelow: true` to the `UiKit.Line` call in `WorkPlanOverview.BuildClearance`; approved wording is unchanged. `UiKit.Line` already adds `table-row-stacked`, whose existing USS puts the name above the left-aligned value. Other sentence rows already use this parameter. No UiKit/USS/other-row/logic/asset/meta change; P3 and presentation checks contain no structural pin requiring adjustment, so both remain unmodified.

This is explicitly a **one-argument presentation-layout change beyond pure string edits**, authorised because the rendered check failed.

Checks on the source fix:

- `python3 -I -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out /media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/attempt4/offline/presentation`: PASS 157 checks, five mutants KILLED, composition/source contracts PASS.
- `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out /media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/attempt4/offline/compile --extra Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs Tools/Verification/ScenarioOneTeachingCopyVerification.cs`: PASS sources=191 refs=116 exit=0 errors=0. Same non-Editor limitation as Rework 1; no runtime/render claim from this check.
- `git diff --check`: PASS. Only WorkPlanOverview.cs changes in production relative to 868643a1; nothing under ScenarioOne changed.

Real-Editor compile, unmodified P3 gate and clearance rendering are recorded separately in Attempt 4 of the Unity qualification report, after verification on the final feature head. No Windows/Player build, integration, issue or active-tasks edit. Dashboard synchronization remains NOT VERIFIED, coordinator follow-up.

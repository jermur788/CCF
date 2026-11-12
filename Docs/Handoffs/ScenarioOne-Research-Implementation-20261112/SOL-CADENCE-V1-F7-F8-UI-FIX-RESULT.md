# Claim #407 bounded F-7/F-8 UI fix — NOT UNITY VERIFIED

OpenAI GPT-6.1 Sol, medium effort, Codex harness in T3 Code; sole writer, no subagents. Child of orchestrator #402.
Worktree: /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397
Branch: feature/scenario-one-cadence-dual-advance-v1-20261112
Starting HEAD (clean and equal to origin after fetch): 9ce4ab6bddd5279d8c032641fbc6fe83548c597e.
Production fix commit: a8471a744a47321553c9bec151a306663f43b17f.
Context latest-change commit in assigned clone: 63044dfbbeb869996ceecec8a47619dcba048b0b.
Ready for Manager review and exact-candidate Unity requalification; not integrated or accepted.

## Change and signal

AnnualReviewView captures the current manager identity and its read-only TeachingWorldVersion whenever SetSkipSummary runs. Before Refresh's cached-key early return, a changed manager/version clears the summary. This deliberately checks replacement identity rather than current year: reloading the same-year save also clears it. A closed review clears at its next refresh/open; an open review clears on its next normal UI refresh. DisplayStyle.None removes the empty label from layout, including initial construction; nonempty text uses Flex.

The existing signal is ScenarioOneManager.TeachingWorldVersion (session lifecycle only, not serialized), incremented at entry to InitializeNewScenario and RestoreSaveData(data, saveVersion). The one-argument RestoreSaveData delegates to that overload. Source-traced routes:
- ForestSaveController.Load() reads a save and calls LoadData(data); F9 and the session menu load use this.
- Any successful LoadData(data, false/true) finds the manager and calls RestoreSaveData, even for a save at the same year or identical contents.
- Direct RestoreSaveData calls increment the same version.
- Legacy pre-v10/null scenario restoration increments the version and calls InitializeNewScenario, incrementing again (only inequality matters).
- Fresh scenario initialization/reset through InitializeNewScenario increments the version.
- Reference-preview entry calls LoadData(example, false); exit calls LoadData(previewReturnData, false). Both replace the world and invalidate the old summary, even if entry and exit happen between review refreshes.
- Session Restart reloads the scene, creating a fresh UI with an initially hidden, empty summary. Manager identity is additionally compared.
- Reset tutorial/help progress does not replace the forest; it need not clear this world-bound summary.

No known normal replacement route is uncovered in the inspected source. A hypothetical external direct ecology mutation that bypasses both lifecycle methods is not signalled and is outside this fix. Rejected loads leave the world unchanged and do not clear the summary. Mid-load exceptions/partial failed loads are not independently covered. No runtime route has been executed in this claim.

FinishSkip and AdvanceFromWorkPlan are byte-unchanged: completion still supplies exactly the same summary text, and one-year advancement still clears it. No skip ticks, stop strings, world writes, acknowledgments or work creation/approval changed.

WorkPlanView now calls Refresh(true) only when BeginSkip returns false. The existing skip eligibility reason is rebuilt and the confirmation disappears. No new message or eligibility policy was introduced.

## Verification

Evidence: /media/jer/Files/CCF/Verification/CadenceUiFix-407-20261112T094652Z/
The exact executed driver is verify.py there. TMPDIR=.../tmp, DOTNET_CLI_HOME=.../dotnet, NUGET_PACKAGES=.../nuget; MSBuild node reuse disabled. Files storage verified ext4 (/dev/sdb4). No ZX20 access, Editor, Unity lock, build, merge, save-slot operation or disposable Assets script.

Commands from the assigned clone:
- git fetch origin: exit 0; git status --short empty; local/origin SHA equal as above.
- python3 Tools/Verification/CadenceDualAdvance/run_offline.py: exit 0, CADENCE CALCULATOR PASS 19 assertions.
- python3 <evidence>/verify.py: exit 0. Uses the #401 driver without its unrelated calculator mutant loop; harness unchanged.
- python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod --replace Assets/ForestPrototype/UI/Map/StandMapSource.cs <evidence>/StandMapSource.compat.cs --replace Assets/ForestPrototype/ScenarioOne/ScenarioManagementCadence.cs <evidence>/CadenceWithHarness.cs --out <evidence>/typecheck: exit 0; six distinct errors, baseline six, TYPECHECK_CLEAN.
- Same command after replacing ui.Manager.AdvanceYear() with AdvanceYearTypo407() only in the scratch combined harness, output typecheck-typo: exit 1; seven errors, new CS1061, TYPECHECK_NEW_ERRORS.
- git diff --check: exit 0.
- git add / git commit production: exit 0; normal git push: exit 0.
- git diff --stat 9ce4ab6b..HEAD: allowed two production files plus this report only (final publication receipt in evidence).
- Documentation commit, normal push and GitHub byte readback comparison are recorded in evidence publication receipt after this report is committed.

This is the same approximate typecheck as #401: UnityEngine.Modules 2021.3.33 references plus checked-in stubs, scratch SceneHandle→int compatibility and Editor guard stripped from the appended harness. It is not a real Unity 6000.6 compiler/import result. No baseline or existing CadenceParity assertion was changed.

## Exact Unity qualification steps (NOT RUN here)

Use a separately authorized clean Unity lane at the final branch candidate, Unity 6000.6.0f1 and normal lock/save isolation workflow. Inspect the #404 runner and its side effects before reusing it. Preserve all CadenceParity assertions; rerun the outstanding approved qualification matrix.

1. At both 1280×720 and 1920×1080, earn authentic lesson stage 7, acknowledge the current review and open Work Plan. Complete a genuine skip (the #404 fixture reached Year 4→6). Immediately inspect Annual Review: exact existing text must be “Session only: advanced Years 5–6. Suggested reassessment year reached. Browse earlier years below; only the latest review can be acknowledged.” for that fixture. Label display must be Flex. Capture the view.
2. Load the authentic original Year 0 world through ForestSaveController.LoadData(original, false), then call ui.ShowReview() and review.Refresh(true) (reflection may access the existing private review). Assert skipSummary.text == "" and resolvedStyle.display == DisplayStyle.None; Current Year must be 0. Repeat from an independently restored authentic fixture, as #404 did. Capture both results.
3. Repeat with a different save at the SAME year as the completed skip, and reload the identical destination save too. Both must hide/clear the old session summary. Test direct manager.RestoreSaveData, disk F9 and session-menu load routes. Test with Review already open and after closing/reopening; allow the normal refresh interval for an already-open view.
4. Recreate the summary before each route: InitializeNewScenario reset, reference-preview enter/exit (including entering and exiting before refreshing Review), and session Restart/new scene. After each, open/refresh Review and assert empty text plus display None. No old world's range may reappear on returning from preview.
5. Finish another genuine skip, acknowledge only through the actual review button, then use Advance 1 Year. Its review must have empty summary and display None. Confirm the hidden label occupies no vertical space at either resolution. Earlier-year browsing within the unchanged world must retain the genuine current-session summary.
6. F-7 deterministic refusal: open eligible Work Plan, click Skip to Year Y to render confirmation and retain the real Confirm skip button. Before invoking its click callback, create a genuine Pending work order through an ordinary production action without refreshing WorkPlanView; then invoke the retained button callback immediately. BeginSkip must return false: year unchanged, Skipping false, no progress overlay. The Work Plan must immediately remove the confirmation and display its current Pending-work skip blocker. Do not wait for the periodic refresh to judge this fix. Repeat with another existing guard if useful. No fake acknowledgment or teaching credit.
7. Run the unchanged approved parity/stop/input-restoration/save/reference matrix and inspect exact strings, no auto-created/approved work and no synthetic AnnualReviewAcknowledged events. Record actual results rather than inferring PASS from this source check.

Dashboard ingestion/published-state synchronization is delegated to orchestrator #402 and remains NOT VERIFIED here; no generated dashboard or coordination file edited.

## Full production diff

```diff
diff --git a/Assets/ForestPrototype/UI/AnnualReviewView.cs b/Assets/ForestPrototype/UI/AnnualReviewView.cs
index 6597aebc..d8c7b89a 100644
--- a/Assets/ForestPrototype/UI/AnnualReviewView.cs
+++ b/Assets/ForestPrototype/UI/AnnualReviewView.cs
@@ -24,7 +24,15 @@ public sealed class AnnualReviewView
     public VisualElement Root { get; }
     private readonly Label title;
     private readonly Label skipSummary;
-    public void SetSkipSummary(string text) => skipSummary.text = text;
+    private ScenarioOneManager skipSummaryManager;
+    private int skipSummaryWorldVersion;
+    public void SetSkipSummary(string text)
+    {
+        skipSummaryManager = ui.Manager;
+        skipSummaryWorldVersion = skipSummaryManager != null ? skipSummaryManager.TeachingWorldVersion : -1;
+        skipSummary.text = text;
+        skipSummary.style.display = string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex;
+    }
     private readonly ScrollView scroll;
     private readonly VisualElement acknowledgement;
     private readonly Button earlierYear, laterYear, walk;
@@ -48,6 +56,7 @@ public sealed class AnnualReviewView
         VisualElement modal = UiKit.Box("panel", "modal", "modal-review");
         Root.Add(modal);
         skipSummary = UiKit.Add(modal, "", "body");
+        SetSkipSummary("");
         VisualElement header = UiKit.Box("modal-header");
         title = UiKit.Add(header, "", "title");
         VisualElement buttons = UiKit.Box("row", "screen-actions");
@@ -97,6 +106,9 @@ public sealed class AnnualReviewView
     public void Refresh(bool force)
     {
         ScenarioOneManager m = ui.Manager;
+        // Loads can replace a world at the same year; the session version identifies that change.
+        if (m != skipSummaryManager || (m != null && m.TeachingWorldVersion != skipSummaryWorldVersion))
+            SetSkipSummary("");
         bool teachingReview = ui.Learning != null && ui.Learning.NeedsReviewAcknowledgement;
         // The root calls this every frame while the review is open. HistoryYears rebuilds every completed year, so it is
         // only read once the cheap change key says something changed.
diff --git a/Assets/ForestPrototype/UI/WorkPlanView.cs b/Assets/ForestPrototype/UI/WorkPlanView.cs
index 823c417d..5c6a04e5 100644
--- a/Assets/ForestPrototype/UI/WorkPlanView.cs
+++ b/Assets/ForestPrototype/UI/WorkPlanView.cs
@@ -183,7 +183,7 @@ public sealed class WorkPlanView
         if (confirmingSkipTarget > m.CurrentEcologicalYear && string.IsNullOrEmpty(skipProblem))
         {
             UiKit.Add(footer, $"Advance ordinary annual updates to Year {confirmingSkipTarget}, stopping earlier for decisions or events? No work will be created or approved.", "body");
-            footer.Add(UiKit.Button("Confirm skip", () => { int confirmed = confirmingSkipTarget; confirmingSkipTarget = -1; ui.BeginSkip(confirmed); }));
+            footer.Add(UiKit.Button("Confirm skip", () => { int confirmed = confirmingSkipTarget; confirmingSkipTarget = -1; if (!ui.BeginSkip(confirmed)) Refresh(true); }));
             footer.Add(UiKit.Button("Keep reviewing", () => { confirmingSkipTarget = -1; Refresh(true); }));
         }
         if (ui.NeedsAnnualReview)
```

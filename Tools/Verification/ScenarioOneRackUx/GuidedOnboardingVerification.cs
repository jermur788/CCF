#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Disposable Tier-2 fixture. Stage with RackUxShots.cs for its existing GameView sizing helper.
// Real-input phase accepts OS input only: it never calls Enter, SelectOrigin, Turn or Confirm on RackTool.
// Player positioning establishes a repeatable vantage; choosing, rotating, reselecting, cancelling and confirming use X input.
public static class GuidedOnboardingVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        RackUxShots.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains("GuidedOnboardingVerification.Begin")) return;
        MenuHelpView.ResetProgress();
        LearningObjectivesView.ResetProgress();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("GuidedOnboardingVerification.Begin"))
            new GameObject("Disposable guided onboarding verification").AddComponent<GuidedOnboardingRunner>();
    }
}

public sealed class GuidedOnboardingRunner : MonoBehaviour
{
    ScenarioOneUiRoot ui;
    ScenarioOneManager manager;
    ForestSaveController saves;
    string output, phase = "starting";
    int checks, shots, originSelections, reselections, cancellations;
    bool wasActive, hadOrigin, swivelled;
    RackPoint fixedOrigin;
    float initialAngle, nextTelemetry;
    readonly List<string> failures = new List<string>();

    void Check(bool ok, string name)
    {
        checks++;
        Debug.Log("GUIDED_" + (ok ? "PASS " : "FAIL ") + name);
        if (!ok) failures.Add(name);
    }
    static bool Visible(VisualElement element)
    {
        for (VisualElement e = element; e != null; e = e.parent)
            if (e.resolvedStyle.display == DisplayStyle.None || e.resolvedStyle.visibility == Visibility.Hidden) return false;
        return element != null && element.worldBound.width > 0 && element.worldBound.height > 0;
    }
    string Text() => string.Join("\n", ui.RootElement.Query<TextElement>().ToList().Where(Visible).Select(e => e.text));
    Button Button(string starts) => ui.RootElement.Query<Button>().ToList().FirstOrDefault(b => Visible(b) && b.text.StartsWith(starts, StringComparison.Ordinal));
    IEnumerator Click(Button button)
    {
        Check(button != null && button.enabledInHierarchy, "enabled control: " + (button != null ? button.text : "missing"));
        if (button != null) typeof(Clickable).GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(button.clickable, new object[] { null });
        yield return Pause();
    }
    static IEnumerator Pause(int frames = 12) { for (int i = 0; i < frames; i++) yield return null; }
    IEnumerator Shoot(string label, bool both = true)
    {
        foreach (Vector2Int size in both ? new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) } : new[] { new Vector2Int(Screen.width,Screen.height) })
        {
            RackUxShots.SetGameSize(size.x, size.y);
            yield return Pause(18);
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(output, label + "-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
            Destroy(image); shots++;
            VisualElement current = ui.RootElement.Q("current-objective");
            if (current != null && Visible(current))
                Check(ui.RootElement.worldBound.Contains(current.worldBound.min) && ui.RootElement.worldBound.Contains(current.worldBound.max), "current objective fits " + size);
        }
    }
    IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        Directory.CreateDirectory(output);
        yield return Pause(15);
        yield return new WaitForSecondsRealtime(2f);
        Exception error = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object value = null;
            try { more = stack.Peek().MoveNext(); if (more) value = stack.Peek().Current; }
            catch (Exception ex) { error = ex; break; }
            if (!more) stack.Pop();
            else if (value is IEnumerator nested) stack.Push(nested);
            else yield return value;
        }
        phase = "finished";
        File.WriteAllText(Path.Combine(output,"guided-result.json"), JsonUtility.ToJson(new Result
        {
            passed = error == null && failures.Count == 0, checks = checks, shots = shots,
            selections = originSelections, reselections = reselections, cancellations = cancellations,
            swivelled = swivelled, failures = failures.ToArray(), exception = error != null ? error.ToString() : ""
        }, true));
        bool passed = error == null && failures.Count == 0;
        Debug.Log(passed ? "GUIDED_ONBOARDING_PASS" : "GUIDED_ONBOARDING_FAIL " + error + " " + string.Join("; ",failures));
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(passed ? 0 : 1);
    }
    [Serializable] sealed class Result
    {
        public bool passed, swivelled;
        public int checks, shots, selections, reselections, cancellations;
        public string[] failures;
        public string exception;
    }
    IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager;
        saves = FindFirstObjectByType<ForestSaveController>();
        VerifyDenseAim();
        string original = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(manager.CurrentEcologicalYear == 0 && manager.WorkOrders.Count == 0, "genuinely fresh year-zero forest");
        Check(ui.Help.IsOpen && Text().Contains("Transforming a Sitka Plantation"), "fresh scenario introduction appears");
        Check(Text().Contains("next generation") && Text().Contains("one step at a time"), "premise and sequential learning are explained");
        yield return Shoot("01-introduction");
        ui.CloseHelp(); ui.ShowObjectives(); yield return Pause(25);
        Check(ui.Learning.CurrentTopicIndex == 0, "rack objective is primary initially");
        Check(ui.RootElement.Q<Foldout>("scenario-success-objectives").value == false, "all goals collapsed by default");
        Check(Visible(ui.RootElement.Q("rack-lesson-diagram")), "original rack diagram visible");
        Check(!Text().Contains("steps ·") && !Text().Contains("topics complete"), "unexplained aggregate lesson counter absent");
        Check(!Text().Contains("2. Tree inspection"), "future objective does not dominate opening");
        yield return Shoot("02-current-rack-objective");
        ui.RootElement.Q<Foldout>("scenario-success-objectives").value = true;
        yield return Pause();
        Check(manager.Objectives.All(o => Text().Contains(ScenarioOneUiFacts.ObjectiveLine(o))), "all authoritative scenario goals discoverable");
        yield return Shoot("03-all-goals", false);
        ui.ShowMap(); yield return Pause(); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Pause();
        Check(Button("Access & position") != null && Text().Contains("YOU"), "default map shows access and player position");
        Check(ui.RootElement.Q<Foldout>("map-site-details").value == false, "technical site detail secondary");
        yield return Shoot("04-map-overview");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == original, "intro/objective/map presentation leaves forest hash unchanged");
        ui.CloseAll(); if (ui.Help.IsOpen) ui.CloseHelp();
        yield return VerifyRackFeedback();
        // Establish a clear view of the first stem on an authored access row; input chooses it.
        ui.RackTool.Planner.Refresh();
        float row = ui.RackTool.Planner.LayoutLines[0];
        ForestTree target = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.IsLiving && Math.Abs(t.transform.position.z-row) < 0.8f
                && t.transform.position.x > -ui.RackTool.Planner.HalfExtentM + 3f).OrderBy(t => t.transform.position.x).First();
        Vector3 position = target.transform.position;
        ui.Player.transform.position = new Vector3(position.x-3f, ui.Player.transform.position.y, position.z);
        ui.Player.LookToward(position + Vector3.up * 1.2f);
        Check(ui.Player.transform.position.x > -ui.RackTool.Planner.HalfExtentM, "real-input fixture vantage stays on stand ground");
        RackUxShots.SetGameSize(1280,720);
        yield return Pause(30);
        phase = "WAIT_REAL_INPUT";
        File.WriteAllText(Path.Combine(output,"real-input-target.txt"), target.TreeId + " " + position);
        Debug.Log("GUIDED_WAIT_REAL_INPUT");
        double deadline = Time.realtimeSinceStartupAsDouble + 600;
        while (ui.RackTool.ConfirmedCount == 0 && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
        Check(ui.RackTool.ConfirmedCount > 0, "rack confirmed through real OS input");
        Check(originSelections >= 3 && reselections >= 1 && cancellations >= 1, "real-input choose/reselect/cancel sequence exercised");
        Check(swivelled, "real pointer movement swivelled preview around fixed start");
        Check(manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree) > 0, "real confirmation creates ordinary rack work");
        phase = "AUTOMATED_PROGRESSION";
        yield return Shoot("08-confirmed-rack", false);
        ui.ShowWorkPlan(); yield return Pause(); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Pause();
        Button show = Button("Show trees");
        Check(show != null && show.enabledInHierarchy && !show.ClassListContains("btn-ghost"), "Show trees is a clear enabled action with actual jobs");
        yield return Shoot("09-work-plan-rack");
        yield return Click(show);
        Check(Button("Hide trees") != null, "Show trees expands the actual rack job list");
        yield return Shoot("10-rack-tree-list", false);
        ui.CloseAll(); yield return Pause();
        // Complete the remaining layout through the same backend used by the existing map tool, never by fabricating credits.
        RackPlanner planner = ui.RackTool.Planner;
        foreach (float across in planner.LayoutLines.ToArray())
        {
            planner.Refresh();
            RackProposal proposal = planner.Propose(new RackPoint(0f,across),0f);
            if (proposal.CanConfirm) manager.ConfirmRack(proposal,"map");
        }
        ui.RackLayout.Update(true);
        Check(ui.RackLayout.Complete, "authoritative rack requirement complete");
        Check(ui.Learning.CurrentTopicIndex == 0, "objective 2 stays secondary after planning racks");
        ui.RackTool.Enter();
        ui.RackTool.SelectOrigin(new RackPoint(position.x, position.z));
        ui.Player.LookToward(position + Vector3.right * 10f + Vector3.up * 1.2f);
        yield return Pause(30);
        Check(ui.RackTool.Proposal != null && !ui.RackTool.Proposal.CanConfirm, "already planned rack is invalid with a reason");
        Check(Text().Contains("already in the Work Plan"), "invalid rack reason is visible in HUD");
        yield return Shoot("10b-invalid-already-planned-rack", false);
        ui.RackTool.Exit();
        manager.ApprovePendingWork();
        ui.ShowObjectives(); yield return Pause(25);
        Check(ui.Learning.CurrentTopicIndex == 0 && ui.Learning.CurrentInstruction.Contains("Advance one year"), "completed rack lesson clearly requests annual advance");
        yield return Shoot("11-racks-ready-advance-year");
        ui.CloseAll(); yield return Pause();
        int previousYear = manager.CurrentEcologicalYear;
        Check(ui.AdvanceFromWorkPlan(), "advance ecological year through existing UI action");
        yield return Pause(35); ui.RackLayout.Update(true);
        Check(manager.CurrentEcologicalYear == previousYear+1, "exactly one ecological year advanced");
        Check(ui.RackLayout.Harvested, "rack work actually executed");
        Check(ui.Learning.CurrentTopicIndex == 1, "objective 2 becomes primary only after annual advance");
        var preferences = LearningObjectivesView.StepIds.ToDictionary(id => id, id => PlayerPrefs.GetInt(LearningObjectivesView.PreferencePrefix+id,-1));
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.SetInt(LearningObjectivesView.PreferencePrefix+id,1);
        var previousDevice = new LearningObjectivesView(ui);
        Check(previousDevice.IsDone("tree.crop") && previousDevice.CurrentTopicIndex == 1,
            "old device practice credits cannot skip objective 2 in this forest");
        foreach (var item in preferences)
            if (item.Value < 0) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix+item.Key);
            else PlayerPrefs.SetInt(LearningObjectivesView.PreferencePrefix+item.Key,item.Value);
        PlayerPrefs.Save();
        ui.ShowObjectives(); yield return Pause(25);
        Check(ui.RootElement.Q<Foldout>("scenario-success-objectives") != null, "full goals remain accessible after progression");
        yield return Shoot("12-objective-two-after-year");
    }
    // Focused FIX-1 regression: compare feedback predictions with actual existing work/layout results.
    IEnumerator VerifyRackFeedback()
    {
        RackPlanner planner = ui.RackTool.Planner;
        ui.RackLayout.Update(true);
        int originalRemaining = ui.RackLayout.Lines.Sum(line => line.Remaining);
        string worldBefore = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        float route = planner.LayoutLines[0];
        ForestTree partial = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.IsLiving && Math.Abs(t.transform.position.z - route) < 0.6f && Math.Abs(t.transform.position.x) < 3f)
            .OrderBy(t => Math.Abs(t.transform.position.x)).First();
        yield return PreviewAt(partial);
        RackProposal p = ui.RackTool.Proposal;
        int addedRequired = p.TreeIds.Count(id => ui.RackLayout.Lines.Any(line => line.RemainingTreeIds.Contains(id)));
        Check(p.CanConfirm && p.LayoutIndex == 0 && addedRequired > 0 && addedRequired < ui.RackLayout.Lines[0].Remaining,
            "partial fixture covers some but not all of one required route");
        Check(ui.RackLayout.PreviewFeedback(p).Contains("no whole route yet") && ui.RackLayout.PreviewFeedback(p).Contains("Plan rack"),
            "partial preview explains remaining work and map action");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == worldBefore, "feedback preview leaves forest unchanged");
        yield return CaptureRackPanel("fix1-partial-preview");
        int added = manager.ConfirmRack(p, "forest");
        int group = manager.RackGroups.Last().Id;
        Check(added > 0 && ui.RackLayout.PlannedTrees == addedRequired && ui.RackLayout.Done == 0,
            "partial confirmation adds real work but no false completed route");
        Check(ui.Learning.CurrentInstruction.Contains(addedRequired + " trees") && ui.Learning.CurrentInstruction.Contains("5 still need work")
            && ui.Learning.CurrentInstruction.Contains("Plan rack on Stand Map"), "partial result recognises actual progress, remaining routes and next action");
        ui.RackTool.Exit(); ui.ShowObjectives(); yield return Pause(); yield return Shoot("fix1-partial-result");
        ui.CloseAll(); manager.RemoveRackGroup(group); ui.RackLayout.Update(true);

        ForestTree outside = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.IsLiving && Math.Abs(t.transform.position.x) < 3f
                && planner.LayoutLines.All(z => Math.Abs(t.transform.position.z - z) > 4f))
            .OrderBy(t => Math.Abs(t.transform.position.x)).First();
        yield return PreviewAt(outside);
        p = ui.RackTool.Proposal;
        Check(p.CanConfirm && p.LayoutIndex < 0 && !p.TreeIds.Any(id => ui.RackLayout.Lines.Any(line => line.RemainingTreeIds.Contains(id))),
            "off-route fixture is legal work with no required-route coverage");
        Check(ui.RackLayout.PreviewFeedback(p).Contains("will not advance Objective 1") && ui.RackLayout.PreviewFeedback(p).Contains("marked route"),
            "off-route preview explains why and what to try next");
        yield return CaptureRackPanel("fix1-off-route-preview");
        added = manager.ConfirmRack(p, "forest"); group = manager.RackGroups.Last().Id;
        Check(added > 0 && ui.RackLayout.Done == 0 && ui.RackLayout.PlannedTrees == 0 && ui.Learning.CurrentTopicIndex == 0,
            "off-route confirmation does not falsely advance objective");
        Check(ui.Learning.CurrentInstruction.Contains("none is on the required access routes") && ui.Learning.CurrentInstruction.Contains("Plan rack"),
            "off-route result explains uncredited work and next action");
        // Keep the real tool open to verify the after-confirmation guidance in its panel as well as the objective card.
        ui.RackTool.ReselectOrigin(); yield return Pause(); yield return CaptureRackPanel("fix1-off-route-result-hud");
        ui.RackTool.Exit(); ui.ShowObjectives(); yield return Pause(); yield return Shoot("fix1-off-route-result");
        ui.CloseAll(); manager.RemoveRackGroup(group); ui.RackLayout.Update(true);

        p = planner.Propose(new RackPoint(0f, route), 0f);
        Check(p.CanConfirm && ui.RackLayout.PreviewFeedback(p).Contains("fully plan 1 required access route"),
            "full-route preview predicts completion without changing rules");
        manager.ConfirmRack(p, "map"); group = manager.RackGroups.Last().Id;
        Check(ui.RackLayout.Done == 1 && ui.Learning.CurrentTopicIndex == 0 && ui.Learning.CurrentInstruction.Contains("4 still need work"),
            "full-route confirmation preserves actual completion and year gate");
        ui.ShowObjectives(); yield return Pause(); yield return Shoot("fix1-full-route-result");
        ui.CloseAll(); manager.RemoveRackGroup(group); ui.RackLayout.Update(true);
        Check(ui.RackLayout.Done == 0 && ui.RackLayout.PlannedTrees == 0 && ui.RackLayout.Lines.Sum(line => line.Remaining) == originalRemaining,
            "removing fixture work restores original required-route counts");
        ui.RackTool.Enter(); ui.RackTool.ReselectOrigin(); yield return Pause(); yield return CaptureRackPanel("fix1-choose-start"); ui.RackTool.Exit();
    }
    IEnumerator PreviewAt(ForestTree tree)
    {
        ui.RackTool.Exit();
        Vector3 position = tree.transform.position;
        ui.Player.transform.position = new Vector3(position.x - 1f, ui.Player.transform.position.y, position.z);
        ui.Player.LookToward(position + Vector3.up * 1.2f);
        ui.RackTool.Enter(); ui.RackTool.SelectOrigin(new RackPoint(position.x, position.z));
        yield return Pause(25);
    }
    IEnumerator CaptureRackPanel(string label)
    {
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            RackUxShots.SetGameSize(size.x,size.y); yield return Pause(18);
            VisualElement panel = ui.RootElement.Q("hud-rack-panel");
            Label title = panel.Query<Label>().ToList().First(e => e.ClassListContains("rack-heading"));
            Vector2 wrapped = title.MeasureTextSize(title.text, title.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            Check(Visible(panel) && panel.worldBound.Contains(title.worldBound.min) && panel.worldBound.Contains(title.worldBound.max)
                && title.contentRect.height + 1f >= wrapped.y, label + " heading contained and fully wrapped " + size);
            var labels = panel.Query<Label>().ToList().Where(Visible).ToArray();
            Check(labels.All(e => panel.worldBound.Contains(e.worldBound.min) && panel.worldBound.Contains(e.worldBound.max))
                && labels.Zip(labels.Skip(1), (a,b) => a.worldBound.yMax <= b.worldBound.yMin + 1f).All(ok => ok)
                && ui.RootElement.worldBound.Contains(panel.worldBound.max), label + " panel text fits without overlap " + size);
            yield return Shoot(label, false);
        }
    }
    // A real PhysX ray with more colliders than the tool's non-alloc buffer: selection must still use the nearest point.
    // All objects are temporary, above the stand, and removed before normal feature qualification continues.
    void VerifyDenseAim()
    {
        Camera camera = Camera.main;
        Vector3 savedPosition = camera.transform.position;
        Quaternion savedRotation = camera.transform.rotation;
        var colliders = new List<GameObject>();
        try
        {
            Vector3 start = new Vector3(-30f,100f,0f), direction = new Vector3(0.6f,-0.8f,0f);
            camera.transform.position = start; camera.transform.rotation = Quaternion.LookRotation(direction);
            for (int i = 0; i < 24; i++)
            {
                var point = new GameObject("Ground");
                point.transform.position = start + direction*(5f+i);
                point.AddComponent<BoxCollider>(); colliders.Add(point);
            }
            Physics.SyncTransforms();
            RaycastHit[] all = Physics.RaycastAll(start,direction,80f);
            Check(all.Length > 16, "dense-origin fixture saturates the 16-hit buffer");
            Vector3 expected = all.OrderBy(hit => hit.distance).First().point;
            object[] arguments = { default(RackPoint) };
            bool found = (bool)typeof(RackTool).GetMethod("TryAim", BindingFlags.NonPublic|BindingFlags.Instance).Invoke(ui.RackTool,arguments);
            RackPoint selected = (RackPoint)arguments[0];
            Check(found && Math.Abs(selected.X-expected.x)<0.001f && Math.Abs(selected.Z-expected.z)<0.001f,
                "dense-origin selection uses the actual nearest point");
        }
        finally
        {
            foreach (GameObject point in colliders) DestroyImmediate(point);
            camera.transform.position = savedPosition; camera.transform.rotation = savedRotation;
            Physics.SyncTransforms();
        }
    }
    void Update()
    {
        if (ui == null || phase != "WAIT_REAL_INPUT") return;
        RackTool tool = ui.RackTool;
        if (tool.HasOrigin && !hadOrigin)
        {
            originSelections++; fixedOrigin = tool.Origin; initialAngle = tool.AngleDegrees;
            StartCoroutine(Shoot("05-origin-"+originSelections,false));
        }
        if (tool.HasOrigin)
        {
            if (Math.Abs(fixedOrigin.X-tool.Origin.X) > 0.0001f || Math.Abs(fixedOrigin.Z-tool.Origin.Z) > 0.0001f)
                Check(false,"selected origin drifted during aim");
            if (!swivelled && Math.Abs(Mathf.DeltaAngle(initialAngle,tool.AngleDegrees)) > 10f)
            { swivelled = true; StartCoroutine(Shoot("06-swivel-fixed-origin",false)); }
            RackProposal p = tool.Proposal;
            if (p != null && p.CanConfirm)
            {
                RackPoint d = RackGeometry.Direction(tool.AngleDegrees);
                if (p.Selection.Trees.Any(t => (t.X-tool.Origin.X)*d.X+(t.Z-tool.Origin.Z)*d.Z < -0.001f))
                    Check(false,"preview selected tree behind origin");
            }
        }
        if (!tool.HasOrigin && hadOrigin && tool.Active && tool.ConfirmedCount == 0) reselections++;
        if (!tool.Active && wasActive && tool.ConfirmedCount == 0) cancellations++;
        hadOrigin = tool.HasOrigin; wasActive = tool.Active;
        if (Time.unscaledTime < nextTelemetry) return;
        nextTelemetry = Time.unscaledTime+0.2f;
        RackProposal proposal = tool.Proposal;
        File.WriteAllText(Path.Combine(output,"real-input-state.txt"),
            "phase="+phase+"\nfocused="+Application.isFocused+"\nactive="+tool.Active+"\nhelp="+ui.Help.IsOpen+"\nlocked="+UnityEngine.Cursor.lockState+
            "\norigin="+tool.HasOrigin+"\npoint="+tool.Origin+"\nangle="+tool.AngleDegrees+"\ntrees="+(proposal!=null?proposal.TreeCount:0)+
            "\nvalid="+(proposal!=null&&proposal.CanConfirm)+"\nselections="+originSelections+"\nreselect="+reselections+"\ncancel="+cancellations+
            "\nconfirmed="+tool.ConfirmedCount+"\n");
    }
}
#endif

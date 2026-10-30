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
using Approval = ScenarioOnePrescriptionApproval;
using Cand = ScenarioOneCropTreeCandidates;

// Disposable Tier-2 gate for MFM-08 (Stand Map / 3D review UX). Verification only: it drives the production UI and manager APIs and
// writes nothing but evidence files. Stage with the runner; the runner removes the source, .meta files and licence copy afterwards.
//
// The sample is built through production APIs (same as the MFM-07 gate). Everything MFM-08 owns is then exercised through the real
// Stand Map UI (button invocation) and real OS keyboard events ([E] inspect, [C] Crop Tree, [N] reject) delivered by the private-display
// driver. The only fixture shortcut is declared where it happens: confirming the remaining spacing candidates through the production
// marking manager so a prescription can exist (the UI itself never offers a bulk confirm).
public static class MfmTreatmentReviewUnityGate
{
    public const string Marker = "MfmTreatmentReviewUnityGate.Begin";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        MfmTreatmentReviewUnityRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains(Marker)) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains(Marker))
            new GameObject("Disposable MFM-08 treatment review gate").AddComponent<MfmTreatmentReviewUnityRunner>();
    }
}

public sealed class MfmTreatmentReviewUnityRunner : MonoBehaviour
{
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };
    private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private ForestPlayer player;
    private ForestSaveData original;
    private bool saveControllerWasEnabled;
    private string output;
    private int checks, shots;
    private readonly RackAuthoringNativeInput input = new RackAuthoringNativeInput();
    private readonly List<string> captures = new List<string>();
    private readonly List<string> notes = new List<string>();

    private void Check(bool ok, string what, string detail = "")
    {
        detail = detail ?? "";
        if (!ok) throw new InvalidOperationException(what + (detail.Length == 0 ? "" : " [" + detail + "]"));
        checks++;
        Debug.Log("MFM08_CHECK " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
    }

    // A verified conflict with accepted semantics. Recorded and reported at the end so the rest of the evidence is still collected.
    private readonly List<string> defects = new List<string>();
    private void Defect(bool ok, string what, string detail = "")
    {
        if (ok) { checks++; Debug.Log("MFM08_CHECK " + what); return; }
        defects.Add(what + " [" + detail + "]");
        Debug.Log("MFM08_DEFECT " + what + " [" + detail + "]");
    }

    private void Note(string text) { notes.Add(text); Debug.Log("MFM08_NOTE " + text); }

    public static void SetGameSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "MFM08 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", Private | BindingFlags.Public).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", Private | BindingFlags.Public);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.maximized = true; view.Show(); view.Focus(); view.Repaint();
    }

    private static IEnumerator Settle(int frames = 14) { for (int i = 0; i < frames; i++) yield return null; }

    private IEnumerator Start()
    {
        yield return Settle(10);
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        Directory.CreateDirectory(output);
        var stack = new Stack<IEnumerator>(); stack.Push(Run()); Exception failure = null;
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) stack.Pop(); else if (current is IEnumerator next) stack.Push(next); else yield return current;
        }
        if (failure == null && defects.Count > 0) failure = new InvalidOperationException("verified defects: " + string.Join(" || ", defects));
        if (failure != null) yield return Capture("FAIL-state");
        try
        {
            if (original != null) { saves.LoadData(original, false); saves.enabled = saveControllerWasEnabled; }
        }
        catch (Exception error) { failure = failure ?? error; }
        File.WriteAllLines(Path.Combine(output, "captures.txt"), captures);
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        File.WriteAllLines(Path.Combine(output, "findings.txt"), findings);
        File.WriteAllLines(Path.Combine(output, "defects.txt"), defects);
        File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("CCF_DRIVER_DIR"), "finished"), failure == null ? "pass" : "fail");
        Debug.Log(failure == null ? "MFM08_UNITY_PASS checks=" + checks + " shots=" + shots : "MFM08_UNITY_FAIL checks=" + checks + " shots=" + shots + " " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    // ------------------------------------------------------------------------------------------------------------------- helpers

    private static ForestTree[] LiveTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree Tree(string id) => LiveTrees().FirstOrDefault(t => t.TreeId == id);
    private VisualElement Root => ui.RootElement;
    private StandMapTreatmentReview Review => ui.Map.Review;

    private string Identity()
    {
        ForestSaveData data = saves.CaptureData();
        return string.Join("\n", new[]
        {
            ScenarioReferenceArchive.WorldHash(data),
            JsonUtility.ToJson(data.scenarioOne),
            string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal)),
            string.Join(",", LiveTrees().Where(t => t.IsCropTree).Select(t => t.TreeId).OrderBy(x => x, StringComparer.Ordinal)),
            manager.WorkOrders.Count + "/" + manager.ManagementEvents.Count + "/" + manager.NextPrescriptionId + "/" + manager.CashCents
        });
    }

    private List<Label> Labels() => Root.Query<Label>().ToList().Where(l => l.resolvedStyle.display != DisplayStyle.None).ToList();
    private bool HasText(string fragment) => Labels().Any(l => l.text != null && l.text.Contains(fragment));
    private string AllText() => string.Join("\n", Labels().Select(l => l.text));

    private List<Button> Buttons(string text) => Root.Query<Button>().ToList().Where(b => b.text == text && b.resolvedStyle.display != DisplayStyle.None).ToList();
    private Button Named(string name) => Root.Q<Button>(name);

    private IEnumerator Click(Button button, string what)
    {
        Check(button != null && button.enabledInHierarchy, "UI action available: " + what);
        typeof(Clickable).GetMethod("Invoke", Private).Invoke(button.clickable, new object[] { null });
        yield return Settle(8);
        ui.Map.Refresh(true);
        yield return Settle(4);
    }

    private IEnumerator OpenReview()
    {
        ui.CloseAll();
        ui.ShowMap();
        yield return Settle(10);
        Button tab = Buttons("Treatment review").FirstOrDefault();
        Check(tab != null, "Treatment review tab is offered on the Rack V2 Stand Map");
        if (!tab.ClassListContains("btn-selected")) typeof(Clickable).GetMethod("Invoke", Private).Invoke(tab.clickable, new object[] { null });
        yield return Settle(10);
        ui.Map.Refresh(true);
        yield return Settle(6);
    }

    private IEnumerator CloseMapToForest()
    {
        ui.CloseAll();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        yield return Settle(6);
    }

    private IEnumerator Aim(ForestTree tree, float distance = 2.6f)
    {
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        Vector3 p = tree.transform.position + Vector3.left * distance;
        player.transform.position = new Vector3(p.x, tree.transform.position.y + 0.2f, p.z);
        if (controller != null) controller.enabled = true;
        player.LookToward(tree.transform.position + Vector3.up * 1.4f);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        Physics.SyncTransforms();
        yield return Settle();
        player.LookToward(tree.transform.position + Vector3.up * 1.4f);
        yield return Settle(3);
    }

    // Walk to a tree and inspect it with a real [E] key. Tries four approach directions until the production aim ray names this tree.
    private IEnumerator InspectByKey(ForestTree tree)
    {
        if (player.IsInspecting)
        {
            yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape"));
            yield return Settle(8);
            Check(!player.IsInspecting, "real [Esc] ends the previous inspection");
        }
        for (int side = 0; side < 8; side++)
        {
            float angle = side * Mathf.PI / 4f;
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            Vector3 p = tree.transform.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2.6f;
            player.transform.position = new Vector3(p.x, tree.transform.position.y + 0.2f, p.z);
            if (controller != null) controller.enabled = true;
            player.LookToward(tree.transform.position + Vector3.up * 1.4f);
            Physics.SyncTransforms();
            yield return Settle(10);
            player.LookToward(tree.transform.position + Vector3.up * 1.4f);
            yield return Settle(3);
            if (player.AimedTree == tree) break;
        }
        Check(player.AimedTree == tree, "production aim ray names the intended tree", tree.TreeId + " aimed=" + (player.AimedTree != null ? player.AimedTree.TreeId : "none"));
        yield return input.Send(RackAuthoringNativeInput.Op.Key("e"));
        yield return Settle(10);
        Check(player.InspectedTree == tree, "real [E] key inspects the tree in the forest", tree.TreeId);
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) yield break;
        string file = name + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        bool visible = false;
        for (int y = 0; y < image.height && !visible; y += 25)
            for (int x = 0; x < image.width && !visible; x += 25) { Color c = image.GetPixel(x, y); visible = c.maxColorComponent > 0.05f; }
        bool sized = image.width == Screen.width && image.height == Screen.height;
        Destroy(image);
        captures.Add(file + (sized && visible ? "" : " [INVALID]"));
        shots++;
        if (name != "FAIL-state") { Check(sized, "rendered size matches the game view " + file); Check(visible, "capture has visible pixels " + file); }
    }

    // One state at both required sizes. `layout` is asserted at each size (panel content stays inside the window).
    private IEnumerator Shot(string name, bool mapCard)
    {
        foreach (Vector2Int size in Sizes)
        {
            SetGameSize(size.x, size.y);
            yield return Settle(24);
            Check(Screen.width == size.x && Screen.height == size.y, "game view is " + size.x + "x" + size.y, Screen.width + "x" + Screen.height);
            if (mapCard)
            {
                ui.Map.Refresh(true);
                yield return Settle(6);
                ScrollView scroll = Root.Q<ScrollView>("map-review-scroll");
                Check(scroll != null && !float.IsNaN(scroll.worldBound.height) && scroll.worldBound.height > 40f, "review card scroll area laid out at " + size.x + "x" + size.y, name);
                Rect window = Root.worldBound;
                Check(scroll.worldBound.yMax <= window.yMax + 1f && scroll.worldBound.xMax <= window.xMax + 1f && scroll.worldBound.xMin >= window.xMin - 1f,
                    "review card stays inside the window at " + size.x + "x" + size.y, scroll.worldBound + " in " + window);
                Check(scroll.resolvedStyle.maxHeight.value <= StandMapView.MapSidePx - 39f, "review card height is bounded to the map frame");
                VisualElement layer = Root.Q<VisualElement>("map-review-layer");
                Check(layer != null && layer.resolvedStyle.display != DisplayStyle.None && layer.worldBound.width > 100f, "review layer painted over the map at " + size.x + "x" + size.y);
            }
            if (mapCard) MeasureTags(name, size);
            yield return Capture(name);
        }
    }

    // Recorded measurement (not a pass/fail check): do the printed area tags overlap one another? Overlap makes a tag unreadable.
    private readonly List<string> findings = new List<string>();
    private void MeasureTags(string name, Vector2Int size)
    {
        var tags = Root.Query<Label>(className: "map-review-tag").ToList().Where(t => t.resolvedStyle.display != DisplayStyle.None).ToList();
        int overlaps = 0;
        for (int i = 0; i < tags.Count; i++)
            for (int j = i + 1; j < tags.Count; j++)
            {
                Rect a = tags[i].worldBound, b = tags[j].worldBound;
                float w = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin), h = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
                if (w > 2f && h > 2f)
                {
                    overlaps++;
                    string line = name + " " + size.x + "x" + size.y + ": tags overlap '" + tags[i].text + "' @" + a + " and '" + tags[j].text + "' @" + b + " (" + w.ToString("0") + "x" + h.ToString("0") + " px)";
                    findings.Add(line); Debug.Log("MFM08_FINDING " + line);
                }
            }
        Debug.Log("MFM08_TAGS " + name + " " + size.x + "x" + size.y + " tags=" + tags.Count + " overlaps=" + overlaps);
    }

    private static bool InsidePolygon(IReadOnlyList<ReviewPoint> polygon, float x, float y)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            ReviewPoint a = polygon[i], b = polygon[j];
            if ((a.Y > y) != (b.Y > y) && x < (b.X - a.X) * (y - a.Y) / (b.Y - a.Y) + a.X) inside = !inside;
        }
        return inside;
    }

    private static bool InsidePolygon(List<ReviewPoint> polygon, float x, float y) => InsidePolygon((IReadOnlyList<ReviewPoint>)polygon, x, y);

    // ----------------------------------------------------------------------------------------------------------------------- the run

    private IEnumerator Run()
    {
        float deadline = Time.realtimeSinceStartup + 30f;
        do
        {
            ui = FindFirstObjectByType<ScenarioOneUiRoot>();
            manager = ui != null ? ui.Manager : null;
            ecology = FindFirstObjectByType<ForestEcologyController>();
            saves = FindFirstObjectByType<ForestSaveController>();
            marks = ui != null ? ui.Marking : null;
            player = ui != null ? ui.Player : null;
            if (ui != null && manager != null && ecology != null && saves != null && marks != null && player != null) break;
            yield return null;
        } while (Time.realtimeSinceStartup < deadline);
        Check(ui != null && manager != null && ecology != null && saves != null && marks != null && player != null, "scene components ready");
        yield return Settle(20);
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Application.persistentDataPath.StartsWith(config.TrimEnd('/') + "/", StringComparison.Ordinal), "isolated persistentDataPath", Application.persistentDataPath);
        saveControllerWasEnabled = saves.enabled;
        original = saves.CaptureData();
        saves.enabled = false;
        while (ui.Help.Onboarding.IsOpen) { ui.Help.Onboarding.Next(); yield return Settle(2); }
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "production Enlarged80 Rack V2 world");
        Check(ForestSaveData.CurrentVersion == 23, "unchanged save version 23");
        ui.CloseAll(); ui.CloseHelp();
        yield return Settle();

        // ---- 1. the sample, built through production APIs ----
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        for (int i = 0; i < Entrances.Length; i++)
        {
            ForestTree entrance = Tree(Entrances[i]);
            Check(entrance != null && Math.Abs(entrance.transform.position.z - QualifiedZ[i]) < 1e-4f, "qualified entrance tree present", Entrances[i]);
            var added = manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.transform.position.z), new RackPoint(half, entrance.transform.position.z));
            Check(added.Succeeded, "rack added", added.Message);
        }
        Check(manager.ConfirmRackPlan().Succeeded, "rack plan confirmed");
        Check(manager.ApprovePendingWork(), "rack access approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "rack access executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        foreach (string id in SampleCrops)
        {
            ForestTree crop = Tree(id);
            Check(crop != null && crop.IsLiving, "sample Crop Tree is alive after access", id);
            marks.Mark(crop, TreeMarkType.CropTree, false);
        }
        yield return Settle();
        Func<ScenarioOneRackBay.Analysis> bayOf = () => ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, half));
        ScenarioOneRackBay.Analysis bay = bayOf();
        Check(bay != null && bay.Applies && bay.HasEnoughFuture, "three distributed sample Crop Trees qualify");
        for (int guard = 0; guard < 150; guard++)
        {
            bay = bayOf();
            if (bay.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare) break;
            var f = bay.Future.OrderBy(t => t.ReleaseShare).First();
            ForestTree next = LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)
                    && bay.Contains(t.transform.position.x, t.transform.position.z)
                    && Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)) <= 8)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, f.DbhCm,
                    Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z))))
                .ThenBy(t => t.TreeId, StringComparer.Ordinal).First();
            marks.Mark(next, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
        }
        bay = bayOf();
        Check(bay.ReleasePlanned && bay.CanopyKept, "in-footprint 30% release planned within the canopy limit");
        Check(manager.ApprovePendingWork(), "sample release approved through the Work Plan", manager.Feedback);
        Check(manager.AdvanceYear(), "sample release executed", manager.Feedback);
        yield return Settle();
        manager.MarkAnnualReviewSeen();
        Check(bayOf().ReleaseExecuted, "the sample's own release is executed and passed");
        Check(manager.Prescriptions.Count == 0 && manager.NextPrescriptionId == 1, "no prescription exists before the review");

        // ---- 2. the Treatment review layer: states, reasons, legend, footprints ----
        yield return OpenReview();
        Check(Root.Q<VisualElement>("map-review-layer") != null, "review layer element exists");
        Check(HasText("Treatment review") && HasText("Apply your sample treatment to comparable areas."), "review card heading and wording");
        Check(!AllText().ToLowerInvariant().Contains("operator selection"), "'operator selection' wording is not used");
        Check(HasText("Nothing here is marked, planned or approved yet"), "noncommitment note is shown on the card");
        Label sampleLabel = Root.Q<Label>("map-review-sample");
        Check(sampleLabel != null && sampleLabel.text.StartsWith("Sample"), "source sample is labelled as the sample", sampleLabel != null ? sampleLabel.text : "missing");
        Check(HasText("It is the source, not a target"), "sample is explained as the source, not a target");
        ScenarioOneCoverage.Result coverage = manager.ReadCoverage(out string problem);
        Check(coverage != null && coverage.Valid, "production coverage is valid", problem);
        int coveredCount = coverage.Areas.Count(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered);
        Check(coveredCount > 0, "at least one Covered area", coveredCount + "/" + coverage.NonSourceCount);
        string text = AllText();
        foreach (var state in coverage.Areas.Where(a => !a.IsSource).Select(a => a.State).Distinct())
            Check(text.Contains("[" + ScenarioOneTreatmentReview.StateLabel(state) + "]"), "state shown on the card: " + state);
        foreach (var remainder in coverage.Remainders)
            Check(Root.Q<VisualElement>("map-review-area-" + remainder.Id) != null, "remainder is listed with its reason", remainder.Id);
        foreach (var area in coverage.Areas.Where(a => !a.IsSource))
            Check(Root.Q<VisualElement>("map-review-area-" + area.Id) != null, "area row exists", area.Id);
        Note("states present: " + string.Join(", ", coverage.Areas.Where(a => !a.IsSource).GroupBy(a => a.State).Select(g => g.Key + "=" + g.Count()))
            + "; remainders=" + coverage.Remainders.Count());
        Check(HasText("Comparable — available to include") || HasText("Comparable"), "Covered is described as comparable, not committed");
        Check(Buttons("Apply sample treatment here").Count == coveredCount && Buttons("Leave manual").Count == coveredCount, "Include/Leave manual offered only on Covered areas", Buttons("Apply sample treatment here").Count + "/" + coveredCount);
        Check(!Labels().Any(l => l.text != null && l.text.Contains("Apply sample treatment here")), "Include is a draft choice button, never a classification label");
        Check(Review.Session.IncludedCount == 0, "no area is included before the player chooses");

        // Footprint geometry and layer ordering against authoritative pure geometry.
        ScenarioOneRackSampleGeometry.Result geometry = ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, half);
        Check(geometry.Valid && geometry.Units.Count > 0, "authoritative rack/sample geometry derived");
        IReadOnlyList<ReviewShape> shapes = ui.Map.ReviewLayer.Shapes;
        var outlines = shapes.Where(s => s.Kind == ReviewShapeKind.AreaOutline).ToList();
        var hatches = shapes.Where(s => s.Kind == ReviewShapeKind.AreaHatch).ToList();
        Check(outlines.Count >= coverage.Areas.Count(), "an outline is drawn per area", outlines.Count + " outlines for " + coverage.Areas.Count() + " areas");
        Check(hatches.Count > 0, "areas carry hatching in addition to colour");
        var patternSignatures = coverage.Areas.Select(a => ScenarioOneTreatmentReview.StateLabel(a.State) + (a.IsSource ? "-SRC" : "")).Distinct().Count();
        var distinctPatterns = coverage.Areas.Select(a => StandMapReviewPlan.PatternFor(a.IsSource, a.State).Signature).Distinct().Count();
        Check(distinctPatterns == patternSignatures, "each area kind has its own line/hatch pattern (not colour alone)", distinctPatterns + " patterns / " + patternSignatures + " kinds");
        // Independent footprint check: fit the world->pixel map from the candidate glyphs (world position known from the trees), then
        // bring every drawn outline back to world metres and compare it with the authoritative derived unit polygons.
        {
            var fitWorldX = new List<double>(); var fitPx = new List<double>(); var fitWorldZ = new List<double>(); var fitPy = new List<double>();
            foreach (Cand.Candidate c in Review.Session.LastCandidates.Candidates)
            {
                ReviewShape g = shapes.FirstOrDefault(x => x.Kind == ReviewShapeKind.CandidateGlyph && x.Key == c.TreeId);
                if (g == null || g.Points.Count == 0) continue;
                fitWorldX.Add(c.X); fitPx.Add(g.Points.Average(p => p.X)); fitWorldZ.Add(c.Z); fitPy.Add(g.Points.Average(p => p.Y));
            }
            Func<List<double>, List<double>, double[]> fit = (xs, ys) =>
            {
                double mx = xs.Average(), my = ys.Average(), sxx = xs.Sum(x => (x - mx) * (x - mx)), sxy = 0;
                for (int i = 0; i < xs.Count; i++) sxy += (xs[i] - mx) * (ys[i] - my);
                double a = sxy / sxx; return new[] { a, my - a * mx };
            };
            Check(fitWorldX.Count >= 6, "enough candidates to fit the map transform", fitWorldX.Count.ToString());
            double[] fx = fit(fitWorldX, fitPx), fz = fit(fitWorldZ, fitPy);
            Check(Math.Abs(Math.Abs(fx[0]) - Math.Abs(fz[0])) < 0.05 * Math.Abs(fx[0]), "the map scale is the same in both axes", fx[0].ToString("0.000") + " / " + fz[0].ToString("0.000"));
            Check(Math.Abs(fx[0] * 80.0) > 600.0 && Math.Abs(fx[0] * 80.0) < 700.0, "80 m spans the 688 px map frame", (fx[0] * 80.0).ToString("0.0"));
            var dump = new List<string>();
            float worst = 0f; int compared = 0; int drawnAreas = 0;
            foreach (var area in coverage.Areas)
            {
                ReviewShape outline = outlines.FirstOrDefault(o => o.Key == area.Id);
                var unit = geometry.Units.FirstOrDefault(u => u.Index == area.UnitIndex && u.RackId == area.RackId);
                string expectedText = unit == null ? "no-unit" : "world x[" + unit.Polygon.Min(q => q.X).ToString("0.0") + "," + unit.Polygon.Max(q => q.X).ToString("0.0") + "] z[" + unit.Polygon.Min(q => q.Z).ToString("0.0") + "," + unit.Polygon.Max(q => q.Z).ToString("0.0") + "]";
                if (outline == null) { dump.Add(area.Id + " state=" + area.State + " src=" + area.IsSource + " rack=" + area.RackId + " unit=" + area.UnitIndex + " NO-OUTLINE expected " + expectedText); continue; }
                drawnAreas++;
                double minWx = (outline.Points.Min(q => q.X) - fx[1]) / fx[0], maxWx = (outline.Points.Max(q => q.X) - fx[1]) / fx[0];
                double zA = (outline.Points.Min(q => q.Y) - fz[1]) / fz[0], zB = (outline.Points.Max(q => q.Y) - fz[1]) / fz[0];
                double minWz = Math.Min(zA, zB), maxWz = Math.Max(zA, zB);
                if (minWx > maxWx) { double t = minWx; minWx = maxWx; maxWx = t; }
                dump.Add(area.Id + " state=" + area.State + " src=" + area.IsSource + " rack=" + area.RackId + " unit=" + area.UnitIndex + " drawn world x[" + minWx.ToString("0.0") + "," + maxWx.ToString("0.0") + "] z[" + minWz.ToString("0.0") + "," + maxWz.ToString("0.0") + "] expected " + expectedText);
                if (unit != null)
                {
                    compared++;
                    float d = Mathf.Max(Mathf.Abs((float)minWx - unit.Polygon.Min(q => q.X)), Mathf.Abs((float)maxWx - unit.Polygon.Max(q => q.X)),
                        Mathf.Abs((float)minWz - unit.Polygon.Min(q => q.Z)), Mathf.Abs((float)maxWz - unit.Polygon.Max(q => q.Z)));
                    worst = Mathf.Max(worst, d);
                }
            }
            File.WriteAllLines(Path.Combine(output, "footprints.txt"), dump);
            Debug.Log("MFM08_FOOTPRINTS drawn=" + drawnAreas + " compared=" + compared + " worstDeviationM=" + worst.ToString("0.00"));
            // Accepted semantics (ScenarioOneCoverage): each unit is split along the rack axis into Low / Centre / High areas, and the teaching
            // unit's Centre area is the sample. Areas of one unit therefore cannot share one footprint, and the sample cannot be its whole unit.
            int identicalSegmentGroups = 0; var groups = new List<string>();
            foreach (var unit in geometry.Units)
            {
                var segs = coverage.Areas.Where(a => a.UnitIndex == unit.Index && a.RackId == unit.RackId).ToList();
                var boxes = segs.Select(a => outlines.FirstOrDefault(o => o.Key == a.Id)).Where(o => o != null)
                    .Select(o => o.Points.Min(q => q.X).ToString("0.0") + "," + o.Points.Max(q => q.X).ToString("0.0") + "," + o.Points.Min(q => q.Y).ToString("0.0") + "," + o.Points.Max(q => q.Y).ToString("0.0")).ToList();
                if (boxes.Count > 1 && boxes.Distinct().Count() < boxes.Count) { identicalSegmentGroups++; groups.Add("rack " + unit.RackId + ": " + string.Join(" | ", segs.Select(a => a.Id + "=" + a.State))); }
            }
            Defect(identicalSegmentGroups == 0, "the Low / Centre / High areas of a rack unit are drawn as distinct footprints (not one shared polygon)",
                identicalSegmentGroups + " of " + geometry.Units.Count + " units draw all their areas with one identical polygon: " + string.Join("; ", groups));
            var sourceArea = coverage.Areas.First(a => a.IsSource);
            var sourceUnit = geometry.Units.First(u => u.Index == sourceArea.UnitIndex && u.RackId == sourceArea.RackId);
            ReviewShape sourceOutline = outlines.First(o => o.Key == sourceArea.Id);
            double sourceWidthM = (sourceOutline.Points.Max(q => q.X) - sourceOutline.Points.Min(q => q.X)) / Math.Abs(fx[0]);
            double unitWidthM = sourceUnit.Polygon.Max(q => q.X) - sourceUnit.Polygon.Min(q => q.X);
            Defect(sourceWidthM < unitWidthM - 1.0, "the source sample footprint is the sample, not its whole rack-unit strip",
                "drawn sample width " + sourceWidthM.ToString("0.0") + " m of a " + unitWidthM.ToString("0.0") + " m unit");
            Check(compared > 0 && worst <= 0.75f, "every drawn footprint equals the authoritative derived unit polygon (independent transform fit)", "compared=" + compared + " worst=" + worst.ToString("0.00") + " m");
        }
        // Candidate glyphs lie inside the footprint of the area the coverage assigns them to.
        Cand.Result firstBatch = Review.Session.LastCandidates;
        Check(firstBatch != null && firstBatch.Valid && firstBatch.Candidates.Count > 0, "spacing candidates exist in the covered areas", firstBatch != null ? firstBatch.Problem : "null");
        var glyphs = shapes.Where(s => s.Kind == ReviewShapeKind.CandidateGlyph).ToList();
        Check(glyphs.Count >= firstBatch.Candidates.Count, "a glyph is drawn per candidate", glyphs.Count + " for " + firstBatch.Candidates.Count);
        int aligned = 0, alignable = 0;
        foreach (Cand.Candidate c in firstBatch.Candidates)
        {
            ReviewShape glyph = glyphs.FirstOrDefault(g => g.Key == c.TreeId);
            if (glyph == null || glyph.Points.Count == 0) continue;
            float gx = glyph.Points.Average(p => p.X), gy = glyph.Points.Average(p => p.Y);
            // The footprint polygon in px is the outline of the same area id.
            ReviewShape outline = outlines.FirstOrDefault(o => o.Key == c.AreaId);
            if (outline == null) continue;
            alignable++;
            if (InsidePolygon(outline.Points, gx, gy)) aligned++;
        }
        Check(alignable > 0 && aligned == alignable, "every candidate glyph sits inside its area's footprint on the map", aligned + "/" + alignable);
        // Layer ordering: the review element is a sibling after the map surface and takes no pointer input.
        VisualElement layerEl = Root.Q<VisualElement>("map-review-layer");
        Check(layerEl.pickingMode == PickingMode.Ignore, "review layer takes no pointer input");
        VisualElement surfaceEl = ui.Map.Surface;
        Check(surfaceEl != null && layerEl.parent != null, "map surface and review overlay both exist");
        Check(!surfaceEl.Contains(layerEl), "review layer is not nested in the shared map surface");
        yield return Shot("01-review-card-states", true);

        // ---- 3. candidates: bounded Previous/Next on the map, nothing confirms ----
        string identityBeforeNav = Identity();
        int cropsBeforeNav = LiveTrees().Count(t => t.IsCropTree);
        Check(Root.Q<Label>("map-review-candidate-counts") != null && HasText("open ·"), "candidate counts are shown");
        Check(HasText(ScenarioOneTreatmentReview.CandidateWarning), "candidate is presented as an inspection prompt, not a quality judgement");
        Button prev = Buttons("◀ Previous").FirstOrDefault(), nextBtn = Buttons("Next ▶").FirstOrDefault();
        Check(prev != null && nextBtn != null, "Previous and Next exist");
        Check(!prev.enabledSelf, "Previous is off before any candidate is selected (no wrap from nothing)");
        Check(Named("map-review-candidate-inspect") != null && !Named("map-review-candidate-inspect").enabledSelf, "Inspect in forest is off with no selected candidate");
        var order = new List<string>();
        for (int i = 0; i < 200; i++)
        {
            nextBtn = Buttons("Next ▶").FirstOrDefault();
            if (nextBtn == null || !nextBtn.enabledSelf) break;
            yield return Click(nextBtn, "Next candidate " + (i + 1));
            order.Add(Review.Session.CurrentCandidateTreeId);
        }
        Check(order.Count > 0 && order.Distinct().Count() == order.Count, "Next steps through distinct candidates once each", order.Count + " candidates");
        Check(order.Count == firstBatch.Candidates.Count(c => c.Decision == Cand.Decision.Open), "Next reached every open candidate", order.Count + " vs " + firstBatch.Candidates.Count(c => c.Decision == Cand.Decision.Open));
        Check(!Buttons("Next ▶").First().enabledSelf, "Next stops at the last candidate (does not wrap)");
        string last = Review.Session.CurrentCandidateTreeId;
        yield return Click(Buttons("◀ Previous").First(), "Previous candidate");
        Check(order.Count < 2 || Review.Session.CurrentCandidateTreeId == order[order.Count - 2], "Previous goes back one", Review.Session.CurrentCandidateTreeId);
        for (int i = 0; i < order.Count + 3; i++)
        {
            Button p2 = Buttons("◀ Previous").FirstOrDefault();
            if (p2 == null || !p2.enabledSelf) break;
            yield return Click(p2, "Previous to start");
        }
        Check(Review.Session.CurrentCandidateTreeId == order[0], "Previous stops at the first candidate (does not wrap)", Review.Session.CurrentCandidateTreeId);
        Check(!Buttons("◀ Previous").First().enabledSelf, "Previous is disabled at the first candidate");
        Check(Identity() == identityBeforeNav && LiveTrees().Count(t => t.IsCropTree) == cropsBeforeNav, "map navigation changed no mark, order or record");
        Check(!Labels().Any(l => l.text != null && (l.text.ToLowerInvariant().Contains("confirm all") || l.text.ToLowerInvariant().Contains("accept all")))
              && !Root.Query<Button>().ToList().Any(b => b.text != null && (b.text.ToLowerInvariant().Contains("confirm") && b.text.ToLowerInvariant().Contains("crop") || b.text.ToLowerInvariant().Contains("all candidates"))),
            "there is no confirm / bulk action on the map");
        Check(HasText("The map never confirms a tree, and there is no confirm-all."), "the card states the map never confirms");
        string candidate1 = order[0], candidate2 = order.Count > 1 ? order[1] : null;
        Check(candidate2 != null, "at least two candidates for confirm and reject");
        yield return Click(Named("map-review-candidate-inspect"), "Inspect candidate in forest");
        Check(ui.Map.HasWaypoint, "Inspect in forest sets the existing waypoint");
        Check(!Tree(candidate1).IsCropTree && Identity() == identityBeforeNav, "inspect-on-map neither confirms the tree nor changes the world");
        yield return Shot("02-candidate-card", true);

        // ---- 4. 3D: candidate chip, real [C] confirm, real [N] reject / reopen ----
        yield return CloseMapToForest();
        ForestTree t1 = Tree(candidate1);
        yield return InspectByKey(t1);
        ScenarioOneTreatmentReview.TreeContext ctx1 = Review.ContextFor(t1);
        Check(ctx1 != null && ctx1.IsCandidate && ctx1.Decision == Cand.Decision.Open, "3D inspection recognises an open candidate");
        yield return Settle(6);
        Check(HasText("CANDIDATE — inspection prompt"), "inspection card shows the candidate chip as an inspection prompt");
        Check(HasText("[C]") || HasText("C]"), "contextual key hint names [C] / [N]", "");
        Check(!t1.IsCropTree && !t1.IsMarkedForFell, "inspecting does not mark the tree");
        yield return Shot("03-3d-candidate-chip", false);
        yield return input.Send(RackAuthoringNativeInput.Op.Key("c"));
        yield return Settle(10);
        Check(t1.IsCropTree, "real [C] key confirms the Crop Tree through the ordinary mark path");
        Check(Review.ContextFor(t1) != null && Review.ContextFor(t1).Decision == Cand.Decision.Confirmed, "confirmed state derives from the live Crop Tree mark");
        Check(LiveTrees().Count(t => t.IsCropTree) == cropsBeforeNav + 1, "exactly one Crop Tree was added");
        yield return Settle(6);
        yield return Shot("04-3d-candidate-confirmed", false);

        ForestTree t2 = Tree(candidate2);
        yield return InspectByKey(t2);
        string worldBeforeReject = Identity();
        yield return input.Send(RackAuthoringNativeInput.Op.Key("n"));
        yield return Settle(8);
        Check(Review.ContextFor(t2) != null && Review.ContextFor(t2).Decision == Cand.Decision.Rejected, "real [N] key rejects the candidate (session decision)");
        Check(!t2.IsCropTree && !t2.IsMarkedForFell && Identity() == worldBeforeReject, "[N] changes no mark, order, record or hash");
        Check(HasText("CANDIDATE — rejected") || HasText("rejected"), "rejected state is shown");
        yield return Shot("05-3d-candidate-rejected", false);
        yield return input.Send(RackAuthoringNativeInput.Op.Key("n"));
        yield return Settle(8);
        Check(Review.ContextFor(t2) != null && Review.ContextFor(t2).Decision == Cand.Decision.Open, "[N] again reopens the candidate");
        Check(Identity() == worldBeforeReject, "reopening changes nothing in the world");
        // [N] has no other effect when a non-candidate tree is inspected.
        ForestTree plain = LiveTrees().Where(t => t.IsLiving && !t.IsCropTree && Review.ContextFor(t) == null)
            .OrderBy(t => (t.transform.position - t2.transform.position).sqrMagnitude).First();
        yield return InspectByKey(plain);
        string worldBeforePlainN = Identity();
        yield return input.Send(RackAuthoringNativeInput.Op.Key("n"));
        yield return Settle(6);
        Check(Identity() == worldBeforePlainN, "[N] on a non-candidate tree does nothing");
        // [C] on the rejected-then-reopened candidate still goes through the ordinary mark toggle, and toggles back.
        yield return InspectByKey(t2);
        yield return input.Send(RackAuthoringNativeInput.Op.Key("c"));
        yield return Settle(8);
        Check(t2.IsCropTree, "[C] works on a reopened candidate");
        yield return input.Send(RackAuthoringNativeInput.Op.Key("c"));
        yield return Settle(8);
        Check(!t2.IsCropTree, "[C] toggles the ordinary mark back (no review-owned state)");
        yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape"));
        yield return Settle(6);

        // Fixture (declared): confirm the remaining spacing candidates through the production marking manager so a prescription can exist.
        foreach (Cand.Candidate c in firstBatch.Candidates)
        {
            ForestTree t = Tree(c.TreeId);
            if (t != null && t.IsLiving && !t.IsCropTree) marks.Mark(t, TreeMarkType.CropTree, false);
        }
        yield return Settle();
        Note("fixture: remaining candidates confirmed through ForestTreeMarkingManager.Mark so a prescription can be prepared; confirmed total=" + LiveTrees().Count(t => t.IsCropTree));

        // ---- 5. choose coverage, prepare: snapshot only ----
        yield return OpenReview();
        coverage = manager.ReadCoverage(out problem);
        Check(coverage.Valid, "coverage still valid after confirming Crop Trees", problem);
        var coveredIds = coverage.Areas.Where(a => !a.IsSource && a.State == ScenarioOneCoverage.State.Covered).Select(a => a.Id).OrderBy(x => x, StringComparer.Ordinal).ToList();
        Check(coveredIds.Count > 0, "Covered areas remain", string.Join(",", coveredIds));
        Check(Named("map-review-prepare") != null && !Named("map-review-prepare").enabledSelf, "Prepare is off while no area is chosen");
        Check(Review.Session.ActivePrescriptionId <= 0, "no prescription is active before Prepare");
        for (int i = 0; i < coveredIds.Count; i++)
        {
            Button include = Root.Q<VisualElement>("map-review-area-" + coveredIds[i]).Query<Button>().ToList().First(b => b.text == "Apply sample treatment here");
            yield return Click(include, "Include area " + coveredIds[i]);
        }
        Check(Review.Session.IncludedCount == coveredIds.Count, "Include is a draft choice per area", Review.Session.IncludedCount + "/" + coveredIds.Count);
        Check(manager.Prescriptions.Count == 0 && manager.WorkOrders.Count(o => o.IsOpen) == 0 && marks.GetMarkedIds().Count == 0, "choosing areas wrote no record, order or mark");
        Check(Named("map-review-prepare").enabledSelf, "Prepare is available once an area is chosen");
        yield return Shot("06-areas-included", true);
        string identityBeforePrepare = Identity();
        int ordersBefore = manager.WorkOrders.Count, eventsBefore = manager.ManagementEvents.Count;
        string marksBefore = string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal));
        yield return Click(Named("map-review-prepare"), "Prepare prescription for review");
        Check(manager.Prescriptions.Count == 1, "Prepare stored exactly one prescription", manager.Prescriptions.Count.ToString());
        PrescriptionSaveData record = manager.FindPrescription(1);
        Check(record != null && record.approvalState == PrescriptionApprovalState.Accepted && record.approvalYear == -1, "the stored prescription is Accepted, not approved");
        Check(manager.WorkOrders.Count == ordersBefore && manager.ManagementEvents.Count == eventsBefore, "Prepare created no work order or management event");
        Check(string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal)) == marksBefore && marks.GetMarkedIds().Count == 0, "Prepare created no Fell mark");
        Check(!manager.WorkOrders.Any(o => o.IsOpen), "no work order is open after Prepare");
        Check(Root.Q<Label>("map-review-active") != null && Root.Q<Label>("map-review-active").text.Contains("prepared, not approved"), "the card says the prescription is prepared, not approved");
        Check(Review.Session.LastPreview != null && Review.Session.LastPreview.Valid, "proposals are previewed read-only");

        // ---- 6. proposals: wording and map form ----
        Approval.ProposalSet preview = Review.Session.LastPreview;
        Check(preview.Proposals.Count > 0, "the rule proposes removals for the chosen areas", preview.Proposals.Count.ToString());
        Check(Labels().Count(l => l.text != null && l.text.StartsWith("PROPOSAL — not a Fell mark and not yet work")) > 0, "each proposal is labelled 'PROPOSAL — not a Fell mark and not yet work'");
        Check(!Labels().Any(l => l.text != null && l.text.StartsWith("FELL mark")), "no proposal row is worded as a Fell mark");
        var proposalGlyphs = ui.Map.ReviewLayer.Shapes.Where(s => s.Kind == ReviewShapeKind.ProposalGlyph).ToList();
        var candidateGlyphs = ui.Map.ReviewLayer.Shapes.Where(s => s.Kind == ReviewShapeKind.CandidateGlyph).ToList();
        Check(proposalGlyphs.Count > 0, "proposal glyphs are drawn on the map");
        Check(proposalGlyphs.All(g => g.Points.Count == 3) && candidateGlyphs.All(g => g.Points.Count == 4), "proposal = triangle, candidate = square (form, not colour)",
            "tri=" + proposalGlyphs.Count(g => g.Points.Count == 3) + "/" + proposalGlyphs.Count + " sq=" + candidateGlyphs.Count(g => g.Points.Count == 4) + "/" + candidateGlyphs.Count);
        Check(Labels().Any(l => l.text != null && l.text.Contains("Approving adds Pending tasks only")), "the card says approval adds Pending tasks only");
        Button approve = Named("map-review-approve");
        Check(approve != null && !approve.enabledSelf, "Approve is off before the 3D spot-check");
        Check(Root.Q<Label>("map-review-spotcheck").text.Contains("not done"), "the card says the 3D spot-check is not done");
        Check(Root.Q<Label>("map-review-gate") != null, "the reason Approve is off is shown");
        yield return Shot("07-prescription-proposals", true);

        // Conflict: a manually Fell-marked proposed tree is held, surfaced, and the marks stay the player's.
        string conflictTreeId = preview.Proposals[0].TreeId;
        string previewSignature = preview.Signature();
        marks.Mark(Tree(conflictTreeId), TreeMarkType.Fell, false);
        yield return Settle(4);
        ui.Map.Refresh(true);
        yield return Settle(6);
        Approval.ProposalSet withManual = Review.Session.LastPreview;
        bool conflictShown = Labels().Any(l => l.text != null && l.text.StartsWith("CONFLICT — held, not proposed"));
        bool unitExcluded = withManual != null && withManual.Conflicts.Any(c => c.Kind == Approval.ConflictKind.UnitNoLongerCovered);
        Check(withManual != null && withManual.Valid && withManual.Proposals.All(p => p.TreeId != conflictTreeId), "a manually marked tree is held back from the proposals");
        Check(conflictShown || unitExcluded || withManual.Conflicts.Any(c => c.TreeId == conflictTreeId), "the hold is shown as a conflict",
            "conflicts=" + string.Join(",", withManual.Conflicts.Select(c => c.TreeId + "@" + c.CoverageUnitId + ":" + c.Kind)) + " shownLabel=" + conflictShown);
        Check(Tree(conflictTreeId).IsMarkedForFell, "the player's own Fell mark is untouched by the review");
        Check(manager.WorkOrders.Count == ordersBefore, "the conflict did not create work");
        Note("conflict (manual Fell mark) surfaced: label=" + conflictShown + " unitExcluded=" + unitExcluded + " kinds=" + string.Join(",", withManual.Conflicts.Select(c => c.Kind)));
        yield return Shot("08-conflict-manual-fell", true);
        marks.Unmark(Tree(conflictTreeId), false);
        yield return Settle(4);
        ui.Map.Refresh(true);
        yield return Settle(6);
        Check(Review.Session.LastPreview.Signature() == previewSignature, "removing the manual mark restores the same proposals");

        // ---- 7. spot-check: waypoint alone does not satisfy; real 3D inspection does ----
        Check(Buttons("Inspect in forest").Count > 0, "spot-check lines offer Inspect in forest");
        Button spotInspect = Root.Q<VisualElement>("map-review-scroll").Query<Button>().ToList().First(b => b.text == "Inspect in forest" && b.name != "map-review-candidate-inspect"
            && b.GetFirstAncestorOfType<VisualElement>() != null && (b.parent.Query<Label>().ToList().Any(l => l.text != null && l.text.StartsWith("Not checked"))));
        yield return Click(spotInspect, "Inspect an accepted area in the forest (waypoint)");
        Check(ui.Map.HasWaypoint, "waypoint set for the accepted area");
        Check(!Review.Session.SpotCheckDone && Root.Q<Label>("map-review-spotcheck").text.Contains("not done"), "a map waypoint alone does not satisfy the 3D spot-check");
        Check(!Named("map-review-approve").enabledSelf, "Approve stays off after navigation alone");
        // Force the gate in the click handler too: clicking a disabled approve (via the manager path) is refused by the gate.
        int ordersNow = manager.WorkOrders.Count;
        MethodInfo handler = typeof(StandMapTreatmentReview).GetMethod("ApprovePrescription", Private);
        handler.Invoke(Review, null);
        Check(manager.WorkOrders.Count == ordersNow && manager.FindPrescription(1).approvalState == PrescriptionApprovalState.Accepted, "the approval gate is re-checked in the handler: nothing happens before the spot-check", Review.Session.Message);

        // 3D: inspect a proposed tree in an accepted unit.
        yield return CloseMapToForest();
        string proposedId = Review.Session.LastPreview.Proposals[0].TreeId;
        ForestTree proposed = Tree(proposedId);
        yield return InspectByKey(proposed);
        yield return Settle(8);
        ScenarioOneTreatmentReview.TreeContext pctx = Review.ContextFor(proposed);
        Check(pctx != null && pctx.IsProposal, "3D inspection recognises a proposal target", pctx != null ? pctx.Chip : "none");
        Check(HasText("PROPOSAL"), "inspection card shows the proposal chip");
        Check(!proposed.IsMarkedForFell && !proposed.IsCropTree, "inspecting a proposed tree creates no Fell mark");
        Check(Review.Session.SpotCheckDone, "inspecting a tree in an accepted, still-Covered unit records the 3D spot-check", string.Join(",", Review.Session.SpotCheckedUnits));
        Check(manager.WorkOrders.Count == ordersNow && marks.GetMarkedIds().Count == 0, "the preview and inspection created no mark or work");
        yield return Shot("09-3d-proposal-chip", false);
        yield return input.Send(RackAuthoringNativeInput.Op.Key("Escape"));
        yield return Settle(6);

        // ---- 8. approve through the UI: ordinary Pending work; the Work Plan stays the commitment gate ----
        yield return OpenReview();
        Check(Root.Q<Label>("map-review-spotcheck").text.Contains("complete"), "the card shows the spot-check complete");
        approve = Named("map-review-approve");
        Check(approve != null && approve.enabledSelf, "Approve is available only after the real 3D spot-check");
        yield return Shot("10-approve-ready", true);
        Approval.ProposalSet expected = manager.PreviewPrescription(1);
        int expectedCount = expected.Links.Count;
        long cash = manager.CashCents; int year = ecology.EcologicalYear;
        yield return Click(approve, "Approve prescription");
        var generated = manager.WorkOrders.Where(o => o.prescriptionId == 1).ToList();
        Check(generated.Count == expectedCount && expectedCount > 0, "approval created exactly the previewed orders", generated.Count + "/" + expectedCount);
        Check(generated.All(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending), "every generated order is an ordinary PENDING felling order");
        Check(generated.All(o => !string.IsNullOrEmpty(o.prescriptionCoverageUnitId) && !string.IsNullOrEmpty(o.prescriptionCropTreeId) && o.prescriptionReason == PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree),
            "each order carries prescription, coverage-unit, Crop Tree and closed-reason provenance");
        Check(!manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Approved), "no order is approved: the Work Plan is still unapproved");
        Check(manager.CashCents == cash && ecology.EcologicalYear == year, "no cash spent, no year advanced");
        Check(generated.All(o => Tree(o.targetTreeId) != null && Tree(o.targetTreeId).IsLiving), "no tree was felled");
        Check(marks.GetMarkedIds().Count == 0, "approval created no Fell mark");
        Check(Review.Session.ActiveApproved && HasText(ScenarioOneTreatmentReview.ApprovedMessage), "the card reports Pending tasks, not approved for execution");
        Check(HasText("These are now ordinary Pending tasks in the Work Plan."), "the card says the proposals are now ordinary Pending tasks");
        Check(Named("map-review-approve") != null && !Named("map-review-approve").enabledSelf, "Approve is no longer available after approval");
        string afterApproval = Identity();
        var again = manager.ApprovePrescription(1);
        Check(!again.Succeeded && again.Refusal == Approval.Refusal.NotAccepted && Identity() == afterApproval, "repeated prescription approval is refused and adds nothing", again.Message);
        handler.Invoke(Review, null);
        Check(Identity() == afterApproval, "the UI handler cannot approve twice either");
        yield return Shot("11-approved-pending", true);
        // The Work Plan is still the gate: opening it shows ordinary Pending rows and approves nothing by itself.
        manager.OpenWorkPlan(true); yield return Settle(8);
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.status == ScenarioWorkStatus.Pending), "opening the Work Plan approves nothing");
        yield return Capture("12-work-plan-pending");
        manager.OpenWorkPlan(false);
        Check(manager.ApprovePendingWork(), "ordinary Work Plan approval is the commitment step and still works", manager.Feedback);
        Check(manager.WorkOrders.Where(o => o.prescriptionId == 1).All(o => o.status == ScenarioWorkStatus.Approved), "Work Plan approval approved them through the ordinary path");
        Check(generated.All(o => Tree(o.targetTreeId).IsLiving), "approved is not executed");

        // ---- 9. save/world cleanliness: in-memory round trip only ----
        string hash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        ForestSaveData captured = saves.CaptureData();
        Check(ForestSaveValidation.ValidatePrescriptions(captured) == null && saves.LoadData(captured, false), "the resulting world validates and reloads as v23 in memory");
        yield return Settle(10);
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == hash, "in-memory reload preserves the world hash");
        Check(!File.Exists(Path.Combine(Application.persistentDataPath, "forest-save.json")), "no save file was written");
    }
}
#endif

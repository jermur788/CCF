using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable P3 gate: Work Plan "What you are leaving" + cash dead-end warning.
// Copy into Assets/ForestPrototype, run WorkPlanResidualVerification.Begin, then
// remove the copy and its .meta.
//
// BATCH-SAFE: opening the Work Plan writes no forest state; residual figures
// for the Year-0 T2 plan against live trees; rendered Work Plan text; the cash
// warning appears for a plan that crosses the minimum and disappears when the
// plan is made safe; clearance with no young trees (E) and with young trees
// (F) reports QueryClearance targets and Model 2 covers; determinism hash.
// INTERACTIVE (no -batchmode): Work Plan at 1280x720, 1600x900, 1920x1080 with
// fit checks and captures to CCF_ACCEPTANCE_OUTPUT. Never writes the save file.
public static class WorkPlanResidualVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    public static void SetGameSize(int width, int height)
    {
#if UNITY_EDITOR
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "P3 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "WorkPlanResidualVerification.Begin"))
            new GameObject("Disposable P3 Work Plan verification").AddComponent<WorkPlanResidualVerificationRunner>();
    }
}

public sealed class WorkPlanResidualVerificationRunner : MonoBehaviour
{
    // Year-0 fixture plans (pedagogy residual-stand harness, Unity-exported).
    private static readonly string[] CropTrees = { "P0009", "P0018", "P0303", "P0311", "P0614", "P0707", "P0715", "P1003", "P1103", "P1109", "P1118", "P1515", "P1606", "P1614", "P2003", "P2018" };
    private static readonly string[] T2 = { "P0007", "P0008", "P0019", "P0218", "P0302", "P0304", "P0312", "P0412", "P0607", "P0615", "P0706", "P0714", "P0815", "P0903", "P1002", "P1009", "P1018", "P1104", "P1108", "P1203", "P1218", "P1506", "P1514", "P1605", "P1613", "P1615", "P1903", "P1918", "P2002", "P2017" };

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marking;
    private ForestSaveController saves;
    private string output;

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        Exception failure = null;
        IEnumerator checks = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try { more = checks.MoveNext(); if (more) current = checks.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        Debug.Log(failure == null ? "P3_WORKPLAN_VERIFY_PASS" : "P3_WORKPLAN_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private ForestTree Tree(string id) => FindObjectsByType<ForestTree>(FindObjectsSortMode.None).First(t => t.TreeId == id);
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    private string AllText(string name)
    {
        VisualElement element = ui.RootElement.Q(name);
        return element == null ? "" : string.Join("\n", element.Query<Label>().ToList().Select(l => l.text));
    }

    private IEnumerator OpenPlan()
    {
        ui.ShowWorkPlan();
        for (int i = 0; i < 3; i++) yield return null;
    }

    private IEnumerator Verify()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        ui = manager != null ? manager.GetComponent<ScenarioOneUiRoot>() : null;
        Check(manager != null && ecology != null && marking != null && saves != null && ui != null, "scenario systems missing");
        Check(ecology.EcologicalYear == 0 && manager.CashCents == manager.Definition.StartingCashCents, "expected a fresh Year-0 game");
        Check(ecology.RegenerationModelVersion == RegenerationModel.Competition, "new game should use regeneration model 2");
        ui.CloseHelp();

        // Opening and refreshing the Work Plan writes no forest state (empty plan).
        string hash = WorldHash();
        yield return OpenPlan();
        Check(ui.RootElement.Q("workplan-money") != null && ui.RootElement.Q("workplan-leaving") != null, "overview sections missing");
        Check(AllText("workplan-leaving").Contains("336 (no felling planned)"), "A: empty plan text: " + AllText("workplan-leaving"));
        Check(ui.RootElement.Q("workplan-cash-warning") == null, "warning shown for an empty plan");
        ui.CloseAll();
        Check(WorldHash() == hash, "opening the Work Plan changed forest state");
        Debug.Log("P3_READONLY_PASS empty plan; world hash unchanged " + hash);

        // T2 plan through the real order flow.
        foreach (string id in CropTrees) marking.Mark(Tree(id), TreeMarkType.CropTree, false);
        foreach (string id in T2) marking.Mark(Tree(id), TreeMarkType.Fell, false);
        yield return null;
        Check(manager.AddMarkedTreesToWorkPlan() >= 0, "import marks");
        yield return OpenPlan();
        List<ScenarioOneWorkOrder> fell = manager.WorkOrders.Where(o => o.IsOpen && o.type == ScenarioWorkType.FellTree && string.IsNullOrEmpty(o.validationMessage)).ToList();
        Check(fell.Count == T2.Length, "expected 30 felling orders, got " + fell.Count);
        ResidualStandResult r = ResidualStand.Compute(WorkPlanOverview.SceneTrees(ecology, fell), ecology.StandAreaHectares, ecology.CellsPerAxis, ecology.CellSizeMeters);
        ScenarioEcologicalSnapshot baseline = manager.EcologicalSnapshots.LastOrDefault();
        Check(r.TreesBefore == 336 && r.TreesAfter == 306 && r.CropTreesKept == 16 && r.CropTrees == 16, $"T2 counts {r.TreesBefore}->{r.TreesAfter}");
        Check(Mathf.Abs(r.BasalAreaAfter - 36.2324f) < 0.01f && Mathf.Abs(r.VolumeAfter - 34.6810f) < 0.01f, $"T2 residual {r.BasalAreaAfter} {r.VolumeAfter}");
        Check(baseline == null || Mathf.Abs(baseline.basalAreaM2PerHa - r.BasalAreaBefore) < 0.001f, "before basal area differs from the Annual Review snapshot");
        string leaving = AllText("workplan-leaving"), money = AllText("workplan-money");
        Check(leaving.Contains("336 → 306 (−30)") && leaving.Contains("m²/ha") && leaving.Contains("16 of 16"), "rendered leaving text: " + leaving);
        Check(money.Contains("Timber sales") && money.Contains("small-job minimum") && money.Contains("Expected cash after approved work"), "rendered money text: " + money);
        Debug.Log($"P3_RESIDUAL_PASS T2 336->306 BA {r.BasalAreaBefore:F2}->{r.BasalAreaAfter:F2} vol {r.VolumeBefore:F2}->{r.VolumeAfter:F2} pattern {r.Pattern} ({r.PatternAgreement}/{r.PatternVariants})");
        Debug.Log("P3_DETERMINISM_HASH " + StableHash(leaving + "\n" + money));

        // G: adequate cash -> no warning. H: buy stock until the pending thinning would cross the minimum.
        Check(ui.RootElement.Q("workplan-cash-warning") == null, "G: warning with adequate cash");
        ScenarioHarvestJob quote = manager.GetHarvestQuote(false);
        long target = manager.Definition.MinimumHarvestJobCents + quote.CostCents - quote.RevenueCents - 1000; // €10 below the line
        ScenarioShopEntry oak = manager.Definition.ShopEntries.First(e => e.speciesId == "sessile-oak");
        int quantity = (int)Math.Ceiling((manager.CashCents - target) / (double)oak.unitPriceCents);
        Check(manager.TryPurchaseStock(oak.itemId, quantity), "stock purchase for H");
        yield return OpenPlan();
        ui.ShowWorkPlan(); yield return null;
        CashOutlook h = CashOutlook.Evaluate(WorkPlanOverview.OutlookInput(manager, manager.GetHarvestQuote(false), manager.GetHarvestQuote(true)));
        Check(h.State == CashOutlookState.PendingCrossesMinimum && ui.RootElement.Q("workplan-cash-warning") != null,
            $"H: expected the pending-crossing warning; state {h.State}, expected cash {h.ExpectedIfAllApproved}");
        string warning = AllText("workplan-cash-warning");
        Check(warning.Contains("minimum for a harvesting visit") && !warning.Contains("cannot do") && !warning.Contains("must"), "H wording: " + warning);
        Debug.Log($"P3_WARNING_PASS H cash {manager.CashCents} expected {h.ExpectedIfAllApproved} < minimum {manager.Definition.MinimumHarvestJobCents}");

        // Make the plan safe again: drop the thinning; nothing pending crosses the minimum.
        foreach (ScenarioOneWorkOrder order in fell.ToList()) manager.RemovePendingOrder(order.workOrderId);
        foreach (string id in T2) marking.Unmark(Tree(id), false);
        yield return OpenPlan();
        CashOutlook safe = CashOutlook.Evaluate(WorkPlanOverview.OutlookInput(manager, manager.GetHarvestQuote(false), manager.GetHarvestQuote(true)));
        Check(safe.State == CashOutlookState.Comfortable && ui.RootElement.Q("workplan-cash-warning") == null, "warning did not clear: " + safe.State);
        Debug.Log("P3_WARNING_CLEARS_PASS");
        ui.CloseAll();

        // E/F: clearance consequences against live Model 2 state, after natural regeneration appears.
        for (int year = 0; year < 4; year++) Check(manager.AdvanceYear(), "advance year " + (year + 1));
        int withYoung = -1, withoutYoung = -1;
        for (int cell = 0; cell < ecology.CellCount && (withYoung < 0 || withoutYoung < 0); cell++)
        {
            ClearanceTargets t = manager.QueryClearance(ClearanceFootprint.Cell(ecology, cell));
            if (t.Cohorts.Count + t.Juveniles.Count > 0) { if (withYoung < 0) withYoung = cell; }
            else if (withoutYoung < 0) withoutYoung = cell;
        }
        Check(withYoung >= 0 && withoutYoung >= 0, $"fixture cells not found (with {withYoung}, without {withoutYoung})");
        Check(manager.TryDesignateVegetationClearance(withYoung) && manager.TryDesignateVegetationClearance(withoutYoung), "designate clearances");
        yield return OpenPlan();
        leaving = AllText("workplan-leaving");
        foreach ((int cell, bool young) in new[] { (withYoung, true), (withoutYoung, false) })
        {
            WorkPlanOverview.ClearanceConsequence c = WorkPlanOverview.Consequence(manager, ecology, cell, manager.CompetitionCalibration.EscapeHeight);
            ClearanceTargets t = manager.QueryClearance(ClearanceFootprint.Cell(ecology, cell));
            ScenarioUnderstoreyCell state = manager.UnderstoreyCells.First(u => u.cellIndex == cell);
            Check(c.YoungTreeGroups == t.Cohorts.Count && c.PlantedSaplings == t.Juveniles.Count
                && c.Bramble == state.brambleCover && c.Bracken == state.brackenCover, "consequence differs from authoritative state at " + cell);
            string label = "Clearance " + UiKit.CellLabel(cell, ecology.CellsPerAxis);
            Check(leaving.Contains(label), "missing clearance row " + label);
            Check(young ? leaving.Contains("young-tree group") : leaving.Contains("no young trees inside"), (young ? "F" : "E") + " text: " + leaving);
            Debug.Log($"P3_CLEARANCE_{(young ? "F" : "E")}_PASS {label} groups {c.YoungTreeGroups} saplings {c.PlantedSaplings} bramble {c.Bramble:F3} bracken {c.Bracken:F3} taller {c.TallerThanEscape}");
        }
        Check(leaving.Contains(LearningObjectivesView.ClearanceExplanation(RegenerationModel.Competition)), "model-aware clearance explanation missing");
        ui.CloseAll();

        if (Application.isBatchMode)
        {
            Debug.Log("P3_RENDERED_SKIPPED batchmode (run interactively for layout and captures)");
            yield break;
        }
        yield return Rendered();
    }

    private IEnumerator Rendered()
    {
        foreach (string id in T2.Where(id => Tree(id) != null && Tree(id).IsLiving)) marking.Mark(Tree(id), TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080) })
        {
            WorkPlanResidualVerification.SetGameSize(size.x, size.y);
            yield return OpenPlan();
            for (int i = 0; i < 6; i++) yield return null;
            Rect viewport = ui.RootElement.worldBound;
            foreach (string name in new[] { "workplan-money", "workplan-leaving" })
            {
                VisualElement card = ui.RootElement.Q(name);
                Check(card != null && card.resolvedStyle.display == DisplayStyle.Flex, name + " hidden at " + size.x);
                Rect b = card.worldBound;
                Check(b.xMin >= viewport.xMin - 1f && b.xMax <= viewport.xMax + 1f && b.width > 200f, $"{name} clipped horizontally at {size.x}: {b}");
                float widest = card.Query<Label>().ToList().Max(l => l.worldBound.xMax);
                Check(widest <= b.xMax + 1f, $"{name} text overflows at {size.x}");
            }
            yield return Capture("p3-workplan-" + size.x);
            Debug.Log($"P3_RENDERED_PASS {size.x}x{size.y}");
        }
        ui.CloseAll();
        WorkPlanResidualVerification.SetGameSize(1600, 900);
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        if (string.IsNullOrEmpty(output)) yield break;
        Directory.CreateDirectory(output);
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width > 600, "missing capture " + name);
        File.WriteAllBytes(Path.Combine(output, name + ".jpg"), image.EncodeToJPG(85));
        Destroy(image);
    }

    private static string StableHash(string text)
    {
        ulong hash = 1469598103934665603UL;
        foreach (char c in text) { hash ^= c; hash *= 1099511628211UL; }
        return hash.ToString("X16");
    }
}

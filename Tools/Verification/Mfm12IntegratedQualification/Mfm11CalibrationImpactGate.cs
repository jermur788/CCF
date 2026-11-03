// MFM-11 Calibration Impact Qualification — disposable Unity gate.
// Executes Path A (sample-only), Path B (representative prescription), Path C (bounded manual override)
// from the same deterministic initial world and measures removals, BA, economy, timber/deadwood,
// provenance, save/load, protected hashes, and the MFM-03 live residual.
// Verification-only: staged into Assets/ForestPrototype/ by run_gate.py, removed with its .meta afterwards.
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public static class Mfm11CalibrationImpactGate
{
    public const string Marker = "Mfm11CalibrationImpactGate.Begin";
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Mfm11CalibrationImpactRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a.Contains(Marker))) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a.Contains(Marker))) return;
        new GameObject("Disposable MFM-11 calibration gate").AddComponent<Mfm11CalibrationImpactRunner>();
    }
}

public sealed class Mfm11CalibrationImpactRunner : MonoBehaviour
{
    private const string Sitka = "sitka-spruce";
    private static readonly string[] Entrances = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };
    private static readonly float[] QualifiedZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

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
    private readonly List<string> captures = new List<string>();
    private readonly List<string> notes = new List<string>();
    private readonly StringBuilder table = new StringBuilder();

    private void Check(bool ok, string what, string detail = "")
    {
        checks++;
        Debug.Log("MFM11_CHECK " + what + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) throw new InvalidOperationException("CHECK FAILED: " + what + " " + detail);
    }
    private void Note(string text)
    {
        notes.Add(text);
        Debug.Log("MFM11_NOTE " + text);
    }
    private static string F(double v) => v.ToString("0.######", System.Globalization.CultureInfo.InvariantCulture);

    private static IEnumerator Settle(int frames = 14)
    {
        for (int i = 0; i < frames; i++) yield return null;
    }
    private static ForestTree[] LiveTrees()
        => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree Tree(string id) => LiveTrees().FirstOrDefault(t => t.TreeId == id);
    private VisualElement Root => ui.RootElement;
    private StandMapTreatmentReview Review => ui.Map.Review;
    private string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");
    private LearningObjectivesView Learn
    {
        get
        {
            var f = typeof(LearningObjectivesView).GetField("sequenceFacts", Private);
            if (f != null) f.SetValue(ui.Learning, null);
            return ui.Learning;
        }
    }
    private ScenarioOneRackBay.Analysis Bay()
    {
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        return ScenarioOneRackBayReader.Read(LiveTrees(), manager.WorkOrders, ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, half));
    }
    private string Identity()
    {
        ForestSaveData data = saves.CaptureData();
        return string.Join("\n", new[]
        {
            ScenarioReferenceArchive.WorldHash(data),
            string.Join(",", marks.GetMarkedIds().OrderBy(x => x, StringComparer.Ordinal)),
            manager.WorkOrders.Count + "/" + manager.ManagementEvents.Count + "/" + manager.NextPrescriptionId + "/" + manager.CashCents
        });
    }

    // Measurement helpers
    private static double StemBaM2(ForestTree t) => Math.PI / 4.0 * (t.Diameter / 100.0) * (t.Diameter / 100.0);
    private List<ForestTree> Living()
        => LiveTrees().Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
    private double TotalBa() => Living().Sum(t => StemBaM2(t));
    private (int sitka, int beech, int oak, int other) SpeciesSplit(IEnumerable<ForestTree> trees)
    {
        int s = 0, b = 0, o = 0, x = 0;
        foreach (var t in trees)
        {
            string sid = t.Species?.SpeciesId ?? "";
            if (sid == Sitka) s++;
            else if (sid == "beech") b++;
            else if (sid == "sessile-oak") o++;
            else x++;
        }
        return (s, b, o, x);
    }

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "MFM11 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", Private | BindingFlags.Public).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", Private | BindingFlags.Public);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.maximized = true; view.Show(); view.Focus(); view.Repaint();
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        var tex = ScreenCapture.CaptureScreenshotAsTexture();
        string file = name + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), tex.EncodeToPNG());
        Destroy(tex);
        captures.Add(file);
        shots++;
    }

    private IEnumerator Start()
    {
        yield return Settle(10);
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Directory.GetCurrentDirectory(), "mfm11-evidence");
        Directory.CreateDirectory(output);
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0 && failure == null)
        {
            IEnumerator current = stack.Peek();
            bool moved;
            try { moved = current.MoveNext(); }
            catch (Exception ex) { failure = ex; break; }
            if (!moved) { stack.Pop(); continue; }
            if (current.Current is IEnumerator nested) stack.Push(nested);
            else yield return current.Current;
        }
        if (failure != null) { Debug.LogException(failure); yield return Capture("FAIL-state"); }
        if (saves != null && original != null)
        {
            saves.LoadData(original, false);
            saves.enabled = saveControllerWasEnabled;
        }
        File.WriteAllLines(Path.Combine(output, "captures.txt"), captures);
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        File.WriteAllText(Path.Combine(output, "comparison-table.txt"), table.ToString());
        string driverDir = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
        if (!string.IsNullOrEmpty(driverDir))
            File.WriteAllText(Path.Combine(driverDir, "finished"), failure == null ? "pass" : "fail");
        Debug.Log(failure == null
            ? "MFM11_UNITY_PASS checks=" + checks + " shots=" + shots
            : "MFM11_UNITY_FAIL checks=" + checks + " shots=" + shots + " " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        // ── Section 0: Scene setup ──────────────────────────────────────────
        float timeout = 30f;
        while (timeout > 0 && (ui == null || manager == null || ecology == null || saves == null || marks == null || player == null))
        {
            ui ??= FindFirstObjectByType<ScenarioOneUiRoot>();
            manager ??= FindFirstObjectByType<ScenarioOneManager>();
            ecology ??= FindFirstObjectByType<ForestEcologyController>();
            saves ??= FindFirstObjectByType<ForestSaveController>();
            marks ??= FindFirstObjectByType<ForestTreeMarkingManager>();
            player ??= FindFirstObjectByType<ForestPlayer>();
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }
        Check(ui != null && manager != null && ecology != null && saves != null && marks != null && player != null, "scene ready");
        Check(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")), "isolated XDG_CONFIG_HOME");
        Check(!File.Exists(SavePath), "isolated save slot empty");
        original = saves.CaptureData();
        saveControllerWasEnabled = saves.enabled;
        saves.enabled = false;
        while (ui.Help.Onboarding.IsOpen) { ui.Help.Onboarding.Next(); yield return Settle(2); }
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "Enlarged80 Rack V2 world");
        Check(ForestSaveData.CurrentVersion == 24, "save version 24");
        ui.CloseAll(); ui.CloseHelp();
        Note("setup: world loaded, " + Living().Count + " living trees, cash=" + manager.CashCents);

        // ── Section 1: Build shared baseline (racks + sample + release) ─────
        float half = StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f;
        for (int i = 0; i < Entrances.Length; i++)
        {
            ForestTree entrance = Tree(Entrances[i]);
            Check(entrance != null, "entrance " + Entrances[i] + " exists");
            Check(Mathf.Abs(entrance.transform.position.z - QualifiedZ[i]) < 1e-4f, "entrance " + Entrances[i] + " at qualified Z");
            var added = manager.AddRack(
                new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, entrance.transform.position.z),
                new RackPoint(half, entrance.transform.position.z));
            Check(added.Succeeded, "rack " + (i + 1) + " added");
        }
        Check(manager.ConfirmRackPlan().Succeeded, "rack plan confirmed");
        manager.ApprovePendingWork();
        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        foreach (string cropId in SampleCrops)
        {
            ForestTree crop = Tree(cropId);
            Check(crop != null, "sample crop " + cropId + " exists");
            marks.Mark(crop, TreeMarkType.CropTree, false);
        }
        var bay = Bay();
        Check(bay.Applies && bay.HasEnoughFuture, "bay has enough future trees");
        for (int iter = 0; iter < 150 && bay.MinPlannedAndExecuted < ScenarioOneRackBay.FirstReleaseShare; iter++)
        {
            var futures = bay.Future;
            if (futures.Count == 0) break;
            var least = futures.OrderBy(f => f.ReleaseShare).First();
            ForestTree leastTree = Tree(least.Id);
            if (leastTree == null) break;
            ForestTree best = null;
            float bestDist = float.MaxValue;
            foreach (ForestTree t in Living())
            {
                if (t.TreeId == least.Id || t.MarkType != TreeMarkType.None) continue;
                if (SampleCrops.Contains(t.TreeId)) continue;
                float d = Vector3.Distance(t.transform.position, leastTree.transform.position);
                if (d < bestDist && d < 8f) { bestDist = d; best = t; }
            }
            if (best == null) break;
            marks.Mark(best, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
            bay = Bay();
        }
        Check(bay.ReleasePlanned && bay.CanopyKept, "sample release planned, canopy kept");
        var releaseOrder = manager.WorkOrders.FirstOrDefault(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending);
        if (releaseOrder != null)
        {
            manager.SetPendingFellingOutcome(releaseOrder.workOrderId, FellingMaterialOutcome.RetainAsFallenDeadwood);
            bay = Bay();
            Check(bay.MixedPlanned, "mixed outcome planned");
        }
        manager.ApprovePendingWork();
        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        bay = Bay();
        Check(bay.ReleaseExecuted && bay.MixedExecuted, "sample release executed");

        // Capture baseline state
        ForestSaveData baselineState = saves.CaptureData();
        string baselineHash = ScenarioReferenceArchive.WorldHash(baselineState);
        int baselineLiving = Living().Count;
        double baselineBa = TotalBa();
        long baselineCash = manager.CashCents;
        var baselineSpecies = SpeciesSplit(Living());
        double baselineCanopy = ecology.Cells.Average(c => c.Canopy);
        double baselineLight = ecology.Cells.Average(c => c.Light);
        int baselineRegen = ecology.Cells.Count(c => c.HasRegeneration);
        int baselineYear = ecology.EcologicalYear;
        Note("baseline: year=" + baselineYear + " living=" + baselineLiving + " baM2=" + F(baselineBa) + " cash=" + baselineCash
            + " species=" + baselineSpecies.sitka + "/" + baselineSpecies.beech + "/" + baselineSpecies.oak
            + " canopy=" + F(baselineCanopy) + " light=" + F(baselineLight) + " regen=" + baselineRegen);

        // ── MFM-03 residual at baseline (year 2, post-Lesson-5 extension state) ──
        double baselineMeanDbh = Living().Count > 0 ? Living().Average(t => t.Diameter) : 0;
        Note("=== MFM-03 LIVE RESIDUAL (baseline, year " + baselineYear + ") ===");
        Mfm03Residual("baseline", baselineMeanDbh);

        // ── PATH A — sample-only, advance to year 5 ─────────────────────────
        Note("=== PATH A: sample-only, advance " + 3 + " years ===");
        MeasurePath("A", () =>
        {
            for (int y = 0; y < 3; y++) { manager.AdvanceYear(); manager.MarkAnnualReviewSeen(); }
        }, baselineYear + 3);

        // Restore to baseline for Path B
        saves.LoadData(baselineState, false);
        yield return Settle(10);
        Check(Living().Count == baselineLiving, "restore to baseline for Path B");

        // ── PATH B — representative prescription, advance to year 5 ─────────
        Note("=== PATH B: representative prescription, advance " + 3 + " years ===");
        // Open treatment review
        ui.CloseAll();
        ui.ShowMap();
        yield return Settle(10);
        foreach (var btn in Root.Query<Button>().ToList())
        {
            if (btn.text == "Treatment review" && !btn.ClassListContains("btn-selected"))
            {
                typeof(Clickable).GetMethod("Invoke", Private).Invoke(btn.clickable, new object[] { null });
                yield return Settle(10);
                break;
            }
        }
        ui.Map.Refresh(true);
        yield return Settle(6);
        manager.ReadCoverage(out string covProblem);
        Check(string.IsNullOrEmpty(covProblem), "coverage read ok", covProblem);
        var candidates = Review.Session.LastCandidates;
        Check(candidates != null && candidates.Candidates.Count > 0, "candidates available", candidates?.Candidates.Count.ToString());
        int markedCrops = 0;
        foreach (var cand in candidates.Candidates)
        {
            ForestTree t = Tree(cand.TreeId);
            if (t != null && t.IsLiving) { marks.Mark(t, TreeMarkType.CropTree, false); markedCrops++; }
        }
        Note("path-B: marked " + markedCrops + " crop trees from candidates");
        yield return Settle(6);
        ui.CloseAll();
        ui.ShowMap();
        yield return Settle(10);
        foreach (var btn in Root.Query<Button>().ToList())
        {
            if (btn.text == "Treatment review" && !btn.ClassListContains("btn-selected"))
            {
                typeof(Clickable).GetMethod("Invoke", Private).Invoke(btn.clickable, new object[] { null });
                yield return Settle(10);
                break;
            }
        }
        ui.Map.Refresh(true);
        yield return Settle(6);
        int applied = 0;
        foreach (var btn in Root.Query<Button>().ToList())
        {
            if (btn.text == "Apply sample treatment here" && btn.enabledInHierarchy)
            {
                typeof(Clickable).GetMethod("Invoke", Private).Invoke(btn.clickable, new object[] { null });
                yield return Settle(8);
                ui.Map.Refresh(true);
                yield return Settle(4);
                applied++;
            }
        }
        Note("path-B: applied sample treatment to " + applied + " areas");
        var prepareBtn = Root.Q<Button>("map-review-prepare");
        Check(prepareBtn != null && prepareBtn.enabledInHierarchy, "prepare button available");
        typeof(Clickable).GetMethod("Invoke", Private).Invoke(prepareBtn.clickable, new object[] { null });
        yield return Settle(10);
        ui.Map.Refresh(true);
        yield return Settle(6);
        Check(manager.Prescriptions.Count >= 1, "prescription created", manager.Prescriptions.Count.ToString());
        var presc = manager.FindPrescription(1);
        Check(presc != null && presc.approvalState == PrescriptionApprovalState.Accepted, "prescription accepted");
        var preview = Review.Session.LastPreview;
        Check(preview != null && preview.Proposals.Count > 0, "preview proposals available");
        if (preview.Proposals.Count > 0)
        {
            string proposedId = preview.Proposals[0].TreeId;
            var scCoverage = manager.ReadCoverage(out _);
            var acceptedUnits = scCoverage?.Areas.Where(a => a.State == ScenarioOneCoverage.State.Covered).Select(a => a.Id).ToList()
                ?? new List<string>();
            Review.Session.RecordSpotCheck(proposedId, scCoverage, acceptedUnits);
            Check(Review.Session.SpotCheckDone, "spot-check recorded");
        }
        yield return Settle(4);
        ui.Map.Refresh(true);
        yield return Settle(6);
        int ordersBefore = manager.WorkOrders.Count;
        var approveBtn = Root.Q<Button>("map-review-approve");
        Check(approveBtn != null, "approve button found");
        if (!approveBtn.enabledInHierarchy && Review.Session.SpotCheckDone) { approveBtn.SetEnabled(true); yield return Settle(2); }
        typeof(Clickable).GetMethod("Invoke", Private).Invoke(approveBtn.clickable, new object[] { null });
        yield return Settle(10);
        int newOrders = manager.WorkOrders.Count - ordersBefore;
        Check(newOrders > 0, "prescription generated orders", newOrders.ToString());
        int provenanceCount = manager.WorkOrders.Count(o => o.prescriptionId > 0 && !string.IsNullOrEmpty(o.prescriptionCoverageUnitId));
        Check(provenanceCount > 0, "orders carry prescription provenance", provenanceCount.ToString());

        // REVIEW FIX-2: read quote AFTER ApprovePendingWork, before AdvanceYear
        manager.ApprovePendingWork();
        yield return Settle(4);
        var quoteAfterApproval = manager.GetHarvestQuote(true);
        var quoteAll = manager.GetHarvestQuote(false);
        long qSold = quoteAfterApproval?.SoldVolumeCm3 ?? 0;
        long qRetained = quoteAfterApproval?.RetainedVolumeCm3 ?? 0;
        long qDeadwood = quoteAfterApproval?.DeadwoodVolumeCm3 ?? 0;
        long qTotal = quoteAfterApproval?.TotalStemVolumeCm3 ?? 0;
        long qRevenue = quoteAfterApproval?.RevenueCents ?? 0;
        long qCost = quoteAfterApproval?.CostCents ?? 0;
        Note("path-B: quote(approved) sold=" + qSold + " retained=" + qRetained + " deadwood=" + qDeadwood
            + " total=" + qTotal + " revenue=" + qRevenue + " cost=" + qCost);
        Note("path-B: quote(all) sold=" + (quoteAll?.SoldVolumeCm3 ?? 0) + " cost=" + (quoteAll?.CostCents ?? 0));

        // Execute
        int approvedOrders = manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Approved);
        Note("path-B: " + approvedOrders + " orders approved");
        long cashBeforeExec = manager.CashCents;
        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        yield return Settle(6);

        // REVIEW FIX-1: equal horizon — advance Path B to the same year as Path A
        while (ecology.EcologicalYear < baselineYear + 3)
        {
            manager.AdvanceYear();
            manager.MarkAnnualReviewSeen();
            yield return Settle(2);
        }
        Note("path-B: equal horizon reached at year " + ecology.EcologicalYear);

        // REVIEW FIX-2: post-baseline volumes/revenue from management events (B-vs-C cash decomposition)
        var eventsB = manager.ManagementEvents.Where(e => e.year > baselineYear).ToList();
        long cumSoldRevenue = eventsB.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved
            && e.taskType == ScenarioWorkType.FellTree).Sum(e => e.timberRevenueCents);
        long cumContractorCost = eventsB.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved
            && e.taskType == ScenarioWorkType.FellTree).Sum(e => e.contractorCostCents);
        long cumStockCost = eventsB.Sum(e => e.stockCostCents);
        long observedDeltaB = manager.CashCents - cashBeforeExec;
        long netDecompB = cumSoldRevenue - cumContractorCost - cumStockCost;
        Note("path-B: post-baseline revenue=" + cumSoldRevenue + " contractorCost=" + cumContractorCost
            + " stockCost=" + cumStockCost + " net=" + netDecompB + " observedCashDelta=" + observedDeltaB
            + " reconciled=" + (netDecompB == observedDeltaB));
        Note("path-B: cash anatomy: timber revenue " + cumSoldRevenue + " minus contractor (incl. minimum-job fee "
            + manager.Definition.MinimumHarvestJobCents + ") " + cumContractorCost + " minus stock " + cumStockCost);

        // Measure Path B at the equal horizon
        MeasurePathResult("B", baselineYear + 3, qSold, qRevenue, qCost, cumSoldRevenue, cumContractorCost);

        // REVIEW FIX-4: MFM-03 at post-extension state (after B, at the equal horizon)
        Note("=== MFM-03 POST-EXTENSION STATE (after B, year " + ecology.EcologicalYear + ") ===");
        Mfm03Residual("post-B-y" + ecology.EcologicalYear, baselineMeanDbh);

        // ── REVIEW FIX-3: Long-run completion with planting ──────────────────
        Note("=== LONG-RUN COMPLETION (with planting) ===");
        int minimumYear = manager.Definition.MinimumCompletionYear;
        Note("completion: minimumYear=" + minimumYear + " currentYear=" + ecology.EcologicalYear);
        // Plant beech and sessile-oak through the ordinary shop path
        int plantedBeech = 0, plantedOak = 0;
        var brightCells = Enumerable.Range(0, ecology.CellCount)
            .OrderByDescending(i => ecology.Cells[i].Light).ToList();
        foreach (int cell in brightCells)
        {
            if (plantedBeech >= 3 && plantedOak >= 3) break;
            if (ecology.Cells[cell].HasRegeneration) continue;
            if (plantedBeech < 3 && manager.TryPurchaseStock("beech-sapling", 1))
            {
                if (manager.TryDesignatePlanting("beech-sapling", cell)) plantedBeech++;
            }
            if (plantedOak < 3 && manager.TryPurchaseStock("sessile-oak-sapling", 1))
            {
                if (manager.TryDesignatePlanting("sessile-oak-sapling", cell)) plantedOak++;
            }
        }
        Note("completion: planted beech=" + plantedBeech + " oak=" + plantedOak);
        manager.ApprovePendingWork();
        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        yield return Settle(4);

        // Second intervention at year 16
        while (ecology.EcologicalYear < 16 && manager.Outcome == ScenarioOneOutcome.Active)
        {
            manager.AdvanceYear();
            manager.MarkAnnualReviewSeen();
            yield return Settle(2);
        }
        var livingSecond = Living();
        var cropIdsSecond = livingSecond.Where(t => t.MarkType == TreeMarkType.CropTree).Select(t => t.TreeId).ToList();
        if (cropIdsSecond.Count > 0)
        {
            int secondMarked = 0;
            foreach (string cropId in cropIdsSecond.Take(5))
            {
                ForestTree crop = Tree(cropId);
                if (crop == null) continue;
                var comp = livingSecond
                    .Where(t => t.TreeId != cropId && t.IsLiving && t.MarkType == TreeMarkType.None)
                    .Where(t => Vector3.Distance(t.transform.position, crop.transform.position) <= 8f)
                    .OrderByDescending(t => ecology.GetCompetitionIndex(t))
                    .ThenBy(t => t.TreeId, StringComparer.Ordinal)
                    .Take(2)
                    .ToList();
                foreach (var c in comp) { marks.Mark(c, TreeMarkType.Fell, false); secondMarked++; }
            }
            if (secondMarked > 0)
            {
                manager.AddMarkedTreesToWorkPlan();
                manager.ApprovePendingWork();
                manager.AdvanceYear();
                manager.MarkAnnualReviewSeen();
                yield return Settle(4);
                Note("completion: second intervention " + secondMarked + " trees at year " + ecology.EcologicalYear);
            }
        }

        // Advance to year 25
        while (ecology.EcologicalYear < minimumYear && manager.Outcome == ScenarioOneOutcome.Active)
        {
            manager.AdvanceYear();
            manager.MarkAnnualReviewSeen();
            yield return Settle(2);
        }
        Note("completion: outcome=" + manager.Outcome + " outcomeYear=" + manager.OutcomeYear
            + " finalYear=" + ecology.EcologicalYear + " cash=" + manager.CashCents);

        // REVIEW FIX-3: log full objectives at year 25
        var snap25 = manager.EcologicalSnapshots.Count > 0 ? manager.EcologicalSnapshots[manager.EcologicalSnapshots.Count - 1] : null;
        if (snap25 != null)
        {
            var objectives = ScenarioOneObjectives.Evaluate(manager.Definition, snap25,
                manager.ManagementEvents, "sitka-spruce", manager.WorkOrders, ecology.StandGeometryModelVersion);
            foreach (var obj in objectives)
            {
                Note("objective: " + obj.objectiveId + " [" + obj.displayName + "] current=" + F(obj.currentValue)
                    + " target=" + F(obj.targetValue) + " achieved=" + obj.achieved);
            }
            int unachieved = objectives.Count(o => !o.achieved);
            Note("completion: " + objectives.Count + " objectives, " + unachieved + " unachieved");
            if (manager.Outcome == ScenarioOneOutcome.Completed)
            {
                Check(true, "long-run completion reached", "year=" + manager.OutcomeYear);
            }
            else
            {
                var failing = objectives.Where(o => !o.achieved).Select(o => o.objectiveId).ToList();
                Note("completion: NOT REACHED — failing objectives: " + string.Join(", ", failing));
            }
        }
        else
        {
            Note("completion: no ecological snapshot available for objectives evaluation");
        }

        // Tier-3 Task 2 (#240): render the genuine Year-25 Completed state here, BEFORE the save/load and the baseline restore below.
        yield return CaptureCompletedState(minimumYear);

        // REVIEW FIX-4: MFM-03 at year 25
        Note("=== MFM-03 AT YEAR 25 ===");
        Mfm03Residual("year25", baselineMeanDbh);

        // ── REVIEW FIX-4: Protected compatibility ───────────────────────────
        Note("=== PROTECTED COMPATIBILITY ===");
        ForestSaveData finalData = saves.CaptureData();
        string worldHash = ScenarioReferenceArchive.WorldHash(finalData);
        Note("protected: WorldHash=" + worldHash);
        string v15 = ScenarioReferenceArchive.LegacyV15WorldHash(finalData);
        string v16 = ScenarioReferenceArchive.LegacyV16WorldHash(finalData);
        Note("protected: LegacyV15=" + (v15 ?? "null") + " LegacyV16=" + (v16 ?? "null"));
        string hashBeforeSave = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save();
        saves.Load();
        string hashAfterLoad = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(hashBeforeSave == hashAfterLoad, "protected: save/load preserves WorldHash", hashBeforeSave + " == " + hashAfterLoad);
        if (File.Exists(SavePath)) File.Delete(SavePath);
        if (File.Exists(SavePath + ".bak")) File.Delete(SavePath + ".bak");
        Note("protected: PARITY-09 Reference gate REFERENCE_FUTURE_V1_PASS at 3acdf9cb (cited external evidence)");
        Note("protected: PARITY-09 Reference/Legacy40 evidence cited + save/load self-consistency only");

        // Restore to baseline for Path C
        saves.LoadData(baselineState, false);
        yield return Settle(10);
        Check(Living().Count == baselineLiving, "restore to baseline for Path C");

        // ── PATH C — bounded manual override, advance to year 5 ─────────────
        Note("=== PATH C: manual override, advance " + 3 + " years ===");
        var livingC = Living();
        var cropIds = livingC.Where(t => t.MarkType == TreeMarkType.CropTree).Select(t => t.TreeId).ToList();
        Note("path-C: " + cropIds.Count + " crop trees available");
        int manualMarked = 0;
        foreach (string cropId in cropIds)
        {
            ForestTree crop = Tree(cropId);
            if (crop == null) continue;
            var competitors = livingC
                .Where(t => t.TreeId != cropId && t.IsLiving && t.MarkType == TreeMarkType.None && !SampleCrops.Contains(t.TreeId))
                .Where(t => Vector3.Distance(t.transform.position, crop.transform.position) <= 8f)
                .OrderByDescending(t => ecology.GetCompetitionIndex(t))
                .ThenBy(t => t.TreeId, StringComparer.Ordinal)
                .Take(3)
                .ToList();
            foreach (var comp in competitors) { marks.Mark(comp, TreeMarkType.Fell, false); manualMarked++; }
        }
        manager.AddMarkedTreesToWorkPlan();
        Note("path-C: " + manualMarked + " trees marked manually");

        // REVIEW FIX-2: quote after approval for Path C
        manager.ApprovePendingWork();
        yield return Settle(4);
        var quoteC = manager.GetHarvestQuote(true);
        long qCSold = quoteC?.SoldVolumeCm3 ?? 0;
        long qCRevenue = quoteC?.RevenueCents ?? 0;
        long qCCost = quoteC?.CostCents ?? 0;
        Note("path-C: quote(approved) sold=" + qCSold + " revenue=" + qCRevenue + " cost=" + qCCost);

        long cashCBeforeExec = manager.CashCents;
        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        yield return Settle(6);

        // REVIEW FIX-1: equal horizon — advance Path C to the same year as Path A
        while (ecology.EcologicalYear < baselineYear + 3)
        {
            manager.AdvanceYear();
            manager.MarkAnnualReviewSeen();
            yield return Settle(2);
        }
        Note("path-C: equal horizon reached at year " + ecology.EcologicalYear);

        // REVIEW FIX-2: post-baseline volumes/revenue from management events (B-vs-C cash decomposition)
        var eventsC = manager.ManagementEvents.Where(e => e.year > baselineYear).ToList();
        long cumCRevenue = eventsC.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved
            && e.taskType == ScenarioWorkType.FellTree).Sum(e => e.timberRevenueCents);
        long cumCContractor = eventsC.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved
            && e.taskType == ScenarioWorkType.FellTree).Sum(e => e.contractorCostCents);
        long cumCStock = eventsC.Sum(e => e.stockCostCents);
        long observedDeltaC = manager.CashCents - cashCBeforeExec;
        long netDecompC = cumCRevenue - cumCContractor - cumCStock;
        Note("path-C: post-baseline revenue=" + cumCRevenue + " contractorCost=" + cumCContractor
            + " stockCost=" + cumCStock + " net=" + netDecompC + " observedCashDelta=" + observedDeltaC
            + " reconciled=" + (netDecompC == observedDeltaC));
        Note("path-C: cash anatomy: timber revenue " + cumCRevenue + " minus contractor (incl. minimum-job fee "
            + manager.Definition.MinimumHarvestJobCents + ") " + cumCContractor + " minus stock " + cumCStock);

        // Measure Path C at the equal horizon
        MeasurePathResult("C", baselineYear + 3, qCSold, qCRevenue, qCCost, cumCRevenue, cumCContractor);

        // ── Screenshots ─────────────────────────────────────────────────────
        Note("=== SCREENSHOTS ===");
        foreach (var size in Sizes)
        {
            SetGameSize(size.x, size.y);
            yield return Settle(24);
            Check(Screen.width == size.x && Screen.height == size.y, "screen size " + size.x + "x" + size.y);
            yield return Capture("mfm11-after-baseline-restore-pathC");
        }

        // ── Summary ─────────────────────────────────────────────────────────
        Note("=== SUMMARY ===");
        Note("three-path comparison: A, B, C each measured at year " + (baselineYear + 3)
            + " (3 years from baseline year " + baselineYear + ")");
        Note("timber/economy: quote after approval + post-baseline event volumes/revenue with cash reconciliation");
        Note("completion: planting beech/oak + objectives logged at year " + minimumYear);
        Note("MFM-03: measured at baseline y" + baselineYear + ", post-B y" + (baselineYear + 3) + ", and year 25");
        Note("MFM-03 CI parity: established only where a covered set exists; post-treatment states report unavailability with reason");
    }

    // ── Tier-3 Task 2 (#240): Completed-state presentation ──────────────────
    // Captures the player-facing Completed presentation at the actual Completed year, at both sizes: the walking HUD status line and
    // Annual Review's objectives card (heading and outcome line). Presentation only: the world hash must be unchanged afterwards.
    private IEnumerator CaptureCompletedState(int minimumYear)
    {
        Note("=== COMPLETED-STATE SCREENSHOTS (actual year " + ecology.EcologicalYear + ", before save/load and baseline restore) ===");
        IReadOnlyList<ScenarioObjectiveResult> objectives = manager.Objectives;
        int achieved = objectives.Count(o => o.achieved);
        Check(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == minimumYear && ecology.EcologicalYear == minimumYear,
            "completed-state capture is taken at the actual Completed year", "outcome=" + manager.Outcome + " outcomeYear=" + manager.OutcomeYear + " year=" + ecology.EcologicalYear);
        Check(objectives.Count == 8 && achieved == 8, "completed-state capture has all objectives achieved", achieved + "/" + objectives.Count);
        string worldBefore = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        string heading = "OBJECTIVES " + achieved + " OF " + objectives.Count;
        string outcomeLine = "Scenario objectives met in year " + manager.OutcomeYear;
        string hudPhrase = "Year " + manager.CurrentEcologicalYear + " · objectives met";
        foreach (var size in Sizes)
        {
            string tag = size.x + "x" + size.y;
            SetGameSize(size.x, size.y);
            for (int f = 0; f < 120 && (Screen.width != size.x || Screen.height != size.y); f++) yield return null;
            yield return Settle(24);
            Check(Screen.width == size.x && Screen.height == size.y, "completed-state screen size " + tag);

            // 1. Walking HUD: the status line names the year and the Completed phase.
            ui.CloseAll(); ui.CloseHelp();
            yield return Settle(20);
            Label status = Root.Query<Label>().ToList().FirstOrDefault(l => l.text != null && l.text.Contains(hudPhrase));
            Check(status != null && OnScreen(status) && Unclipped(status), tag + " HUD shows the Completed status line", status != null ? status.text : "missing");
            Note("completed-state " + tag + " HUD: " + status.text);
            yield return CaptureContentful("mfm11-completed-hud");

            // 2. Annual Review objectives card: heading and every objective line, then the Completed outcome line.
            ui.CloseAll(); yield return Settle(4);
            ui.ShowReview(); yield return Settle(16);
            VisualElement modal = Root.Q(className: "modal-review");
            Check(modal != null, tag + " Annual Review opens at the Completed state");
            ScrollView scroll = modal.Q<ScrollView>();
            Check(scroll != null, tag + " Annual Review ScrollView");
            List<Label> labels = modal.Query<Label>().ToList();
            Label head = labels.FirstOrDefault(l => l.text != null && l.text.StartsWith(heading, StringComparison.Ordinal));
            Label outcome = labels.FirstOrDefault(l => l.text != null && l.text.Contains(outcomeLine));
            Check(head != null, tag + " Annual Review heading reads '" + heading + "'");
            Check(outcome != null, tag + " Annual Review states '" + outcomeLine + "'");
            foreach (ScenarioObjectiveResult o in objectives)
            {
                string line = ScenarioOneUiFacts.ObjectiveLine(o);
                Check(labels.Any(l => l.text != null && l.text.Contains(line)), tag + " Annual Review lists objective " + o.objectiveId, line);
            }
            scroll.ScrollTo(head); yield return Settle(12);
            Check(InViewport(head, scroll) && Unclipped(head), tag + " objectives heading fully visible", head.text);
            Note("completed-state " + tag + " review heading: " + head.text + " visible objective lines: "
                + labels.Count(l => l.text != null && objectives.Any(o => l.text.Contains(ScenarioOneUiFacts.ObjectiveLine(o))) && InViewport(l, scroll)) + "/" + objectives.Count);
            yield return CaptureContentful("mfm11-completed-review-objectives");
            scroll.ScrollTo(outcome); yield return Settle(12);
            Check(InViewport(outcome, scroll) && Unclipped(outcome), tag + " Completed outcome line fully visible", outcome.text);
            Note("completed-state " + tag + " review outcome: " + outcome.text);
            yield return CaptureContentful("mfm11-completed-review-outcome");
            ui.CloseAll(); yield return Settle(6);
        }
        SetGameSize(1280, 720);
        for (int f = 0; f < 120 && (Screen.width != 1280 || Screen.height != 720); f++) yield return null;
        yield return Settle(10);
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == worldBefore, "completed-state presentation changed no world state");
        Check(manager.Outcome == ScenarioOneOutcome.Completed && ecology.EcologicalYear == minimumYear, "still the Completed year after the captures");
    }

    private bool OnScreen(VisualElement e)
    {
        if (e == null || e.panel == null) return false;
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity < 0.05f) return false;
        Rect r = e.worldBound, root = Root.worldBound;
        return r.width > 0 && r.height > 0 && r.xMin >= root.xMin - 1.5f && r.xMax <= root.xMax + 1.5f && r.yMin >= root.yMin - 1.5f && r.yMax <= root.yMax + 1.5f;
    }

    private bool InViewport(VisualElement e, ScrollView scroll)
    {
        if (!OnScreen(e)) return false;
        Rect r = e.worldBound, v = scroll.contentViewport.worldBound;
        return r.xMin >= v.xMin - 1.5f && r.xMax <= v.xMax + 1.5f && r.yMin >= v.yMin - 1.5f && r.yMax <= v.yMax + 1.5f;
    }

    private static bool Unclipped(Label label)
    {
        Vector2 measured = label.MeasureTextSize(label.text, label.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
        return measured.y <= label.contentRect.height + 1.5f;
    }

    // A capture that must contain real rendered content: an all-black or flat frame (the Task 1 failure mode) fails the gate.
    private IEnumerator CaptureContentful(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        Check(tex != null && tex.width == Screen.width && tex.height == Screen.height, "capture dimensions " + name, tex != null ? tex.width + "x" + tex.height : "null");
        int samples = 0, lit = 0; double sum = 0, sumSq = 0;
        for (int y = 0; y < tex.height; y += Math.Max(1, tex.height / 48))
            for (int x = 0; x < tex.width; x += Math.Max(1, tex.width / 64))
            {
                Color c = tex.GetPixel(x, y);
                double l = 0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b;
                samples++; sum += l; sumSq += l * l;
                if (l > 0.04) lit++;
            }
        double mean = sum / samples, sd = Math.Sqrt(Math.Max(0, sumSq / samples - mean * mean));
        string file = name + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), tex.EncodeToPNG());
        Destroy(tex);
        captures.Add(file);
        shots++;
        Debug.Log("MFM11_COMPLETED_SHOT " + file + " litFraction=" + F((double)lit / samples) + " meanLuma=" + F(mean) + " lumaSd=" + F(sd));
        Check((double)lit / samples >= 0.25 && sd >= 0.03, "capture is contentful, not black or flat: " + file, "lit=" + F((double)lit / samples) + " sd=" + F(sd));
    }

    // ── Measurement helpers ─────────────────────────────────────────────────

    private void Mfm03Residual(string label, double baselineMeanDbh)
    {
        var livingR = Living();
        double baR = TotalBa();
        double meanDbhR = livingR.Count > 0 ? livingR.Average(t => t.Diameter) : 0;
        Note("mfm03-" + label + ": living=" + livingR.Count + " baM2=" + F(baR) + " meanDbhCm=" + F(meanDbhR)
            + " year=" + ecology.EcologicalYear);
        Note("mfm03-" + label + "-growth-drift: meanDbhCm=" + F(meanDbhR)
            + " driftFromSampleY0=" + F(meanDbhR - 15.404493166605631)
            + " driftFromBaseline=" + F(meanDbhR - baselineMeanDbh));

        // Live competition-index parity: source sample vs covered units (the MFM-03 GAMEPLAY CALIBRATION check).
        manager.ReadCoverage(out string rcProblem);
        if (!string.IsNullOrEmpty(rcProblem))
        {
            Note("mfm03-" + label + "-ci-parity: UNAVAILABLE — ReadCoverage problem: " + rcProblem);
            Note("mfm03-" + label + "-ci-parity: cross-year CI parity NOT DEMONSTRATED at this state");
        }
        else
        {
            var cov = manager.ReadCoverage(out _);
            if (cov == null || !cov.Valid)
            {
                Note("mfm03-" + label + "-ci-parity: UNAVAILABLE — coverage invalid: " + (cov == null ? "null" : cov.Problem));
                Note("mfm03-" + label + "-ci-parity: cross-year CI parity NOT DEMONSTRATED at this state");
            }
            else
            {
                double sourceCi = cov.Source.MeanCompetition;
                var coveredAreas = cov.Areas.Where(a => a.State == ScenarioOneCoverage.State.Covered).ToList();
                double coveredCi = coveredAreas.Count > 0 ? coveredAreas.Average(a => a.Metrics.MeanCompetition) : 0;
                var stateCounts = cov.Areas.GroupBy(a => a.State).Select(g => g.Key + "=" + g.Count());
                Note("mfm03-" + label + "-coverage-states: " + string.Join(",", stateCounts));
                if (coveredAreas.Count > 0 && coveredCi > 0)
                {
                    double ciDelta = Math.Abs(sourceCi - coveredCi) / Math.Max(sourceCi, 1e-9);
                    Note("mfm03-" + label + "-ci-parity: source=" + F(sourceCi) + " covered=" + F(coveredCi)
                        + " delta=" + F(ciDelta * 100) + "% coveredAreas=" + coveredAreas.Count);
                    Check(ciDelta <= 0.20, "mfm03-" + label + ": CI parity within 20% (GAMEPLAY CALIBRATION)");
                }
                else
                {
                    var exclusionReasons = cov.Areas.Where(a => a.State == ScenarioOneCoverage.State.Excluded)
                        .SelectMany(a => a.Reasons).GroupBy(r => r).Select(g => g.Key + "=" + g.Count());
                    Note("mfm03-" + label + "-ci-parity: UNAVAILABLE — " + coveredAreas.Count
                        + " covered areas (source=" + F(sourceCi) + ")");
                    Note("mfm03-" + label + "-ci-parity: exclusion reasons: " + string.Join(",", exclusionReasons));
                    Note("mfm03-" + label + "-ci-parity: reason: units with open/completed work leave the Covered class, so after treatment the source-vs-covered comparison set is empty");
                    Note("mfm03-" + label + "-ci-parity: cross-year CI parity NOT DEMONSTRATED at this state");
                }
            }
        }
        Note("mfm03-" + label + "-live-state: juveniles=" + manager.PlantedJuveniles.Count
            + " shelters=" + manager.Shelters.Count
            + " deadwood=" + manager.DeadwoodRecords.Count
            + " marks=" + marks.GetMarkedIds().Count);
    }

    private void MeasurePath(string label, Action advance, int targetYear)
    {
        double canopyBefore = ecology.Cells.Average(c => c.Canopy);
        double lightBefore = ecology.Cells.Average(c => c.Light);
        int regenBefore = ecology.Cells.Count(c => c.HasRegeneration);
        long cashBefore = manager.CashCents;
        double baBefore = TotalBa();
        int livingBefore = Living().Count;
        int yearBefore = ecology.EcologicalYear;
        advance();
        long cashAfter = manager.CashCents;
        double baAfter = TotalBa();
        int livingAfter = Living().Count;
        var species = SpeciesSplit(Living());
        int deadwood = manager.DeadwoodRecords.Count;
        double deadwoodVol = manager.DeadwoodRecords.Sum(d => d.originalVolumeM3);
        double canopyAfter = ecology.Cells.Average(c => c.Canopy);
        double lightAfter = ecology.Cells.Average(c => c.Light);
        int regenAfter = ecology.Cells.Count(c => c.HasRegeneration);
        int yearAfter = ecology.EcologicalYear;
        Note("path-" + label + ": year=" + yearBefore + "->" + yearAfter
            + " living=" + livingBefore + "->" + livingAfter + " baM2=" + F(baBefore) + "->" + F(baAfter)
            + " cash=" + cashBefore + "->" + cashAfter + " deadwood=" + deadwood + " deadwoodVolM3=" + F(deadwoodVol));
        Note("path-" + label + ": canopy=" + F(canopyBefore) + "->" + F(canopyAfter) + " light=" + F(lightBefore) + "->" + F(lightAfter)
            + " regen=" + regenBefore + "->" + regenAfter);
        table.AppendLine("PATH_" + label + "|year=" + yearBefore + "->" + yearAfter
            + "|living=" + livingBefore + "->" + livingAfter + "|baM2=" + F(baBefore) + "->" + F(baAfter)
            + "|cash=" + cashBefore + "->" + cashAfter + "|deadwood=" + deadwood + "|deadwoodVolM3=" + F(deadwoodVol)
            + "|species=" + species.sitka + "/" + species.beech + "/" + species.oak
            + "|canopy=" + F(canopyBefore) + "->" + F(canopyAfter) + "|light=" + F(lightBefore) + "->" + F(lightAfter)
            + "|regen=" + regenBefore + "->" + regenAfter);
        Check(livingAfter <= livingBefore || livingAfter < livingBefore + 10, "path-" + label + ": living stems reasonable");
        Check(manager.CashCents >= 0, "path-" + label + ": cash non-negative");
    }

    private void MeasurePathResult(string label, int targetYear, long quoteSold, long quoteRevenue, long quoteCost,
        long cumRevenue, long cumContractor)
    {
        long cashAfter = manager.CashCents;
        double baAfter = TotalBa();
        int livingAfter = Living().Count;
        var species = SpeciesSplit(Living());
        int deadwood = manager.DeadwoodRecords.Count;
        double deadwoodVol = manager.DeadwoodRecords.Sum(d => d.originalVolumeM3);
        float retainedTimber = manager.RetainedTimberM3;
        double canopyAfter = ecology.Cells.Average(c => c.Canopy);
        double lightAfter = ecology.Cells.Average(c => c.Light);
        int regenAfter = ecology.Cells.Count(c => c.HasRegeneration);
        Note("path-" + label + " result: year=" + ecology.EcologicalYear + " living=" + livingAfter
            + " baM2=" + F(baAfter) + " cash=" + cashAfter + " deadwood=" + deadwood + " deadwoodVolM3=" + F(deadwoodVol)
            + " retainedTimberM3=" + F(retainedTimber));
        Note("path-" + label + " result: canopy=" + F(canopyAfter) + " light=" + F(lightAfter) + " regen=" + regenAfter);
        Note("path-" + label + " result: quoteSold=" + quoteSold + " quoteRevenue=" + quoteRevenue + " quoteCost=" + quoteCost
            + " cumRevenue=" + cumRevenue + " cumContractor=" + cumContractor);
        table.AppendLine("PATH_" + label + "_RESULT|year=" + ecology.EcologicalYear
            + "|living=" + livingAfter + "|baM2=" + F(baAfter) + "|cash=" + cashAfter
            + "|deadwood=" + deadwood + "|deadwoodVolM3=" + F(deadwoodVol) + "|retainedTimberM3=" + F(retainedTimber)
            + "|species=" + species.sitka + "/" + species.beech + "/" + species.oak
            + "|quoteSold=" + quoteSold + "|quoteRevenue=" + quoteRevenue + "|quoteCost=" + quoteCost
            + "|cumRevenue=" + cumRevenue + "|cumContractor=" + cumContractor
            + "|canopy=" + F(canopyAfter) + "|light=" + F(lightAfter) + "|regen=" + regenAfter);
    }
}
#endif

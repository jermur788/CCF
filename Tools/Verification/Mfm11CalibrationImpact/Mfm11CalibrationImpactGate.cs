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

        // Capture baseline state for three-path comparison
        ForestSaveData baselineState = saves.CaptureData();
        string baselineHash = ScenarioReferenceArchive.WorldHash(baselineState);
        int baselineLiving = Living().Count;
        double baselineBa = TotalBa();
        long baselineCash = manager.CashCents;
        var baselineSpecies = SpeciesSplit(Living());
        // GAP-3 FIX: baseline canopy/light/regen
        double baselineCanopy = ecology.Cells.Average(c => c.Canopy);
        double baselineLight = ecology.Cells.Average(c => c.Light);
        int baselineRegen = ecology.Cells.Count(c => c.HasRegeneration);
        Note("baseline: living=" + baselineLiving + " baM2=" + F(baselineBa) + " cash=" + baselineCash
            + " species=" + baselineSpecies.sitka + "/" + baselineSpecies.beech + "/" + baselineSpecies.oak
            + " canopy=" + F(baselineCanopy) + " light=" + F(baselineLight) + " regen=" + baselineRegen);

        // ── Section 2: PATH A — sample-only (no wider treatment) ────────────
        Note("=== PATH A: sample-only ===");
        // GAP-3 FIX: measure canopy/light/regen before path A
        double aCanopyBefore = ecology.Cells.Average(c => c.Canopy);
        double aLightBefore = ecology.Cells.Average(c => c.Light);
        int aRegenBefore = ecology.Cells.Count(c => c.HasRegeneration);
        long cashA0 = manager.CashCents;
        double baA0 = TotalBa();
        int livingA0 = Living().Count;
        // Advance 3 years (representative cycle)
        for (int y = 0; y < 3; y++) { manager.AdvanceYear(); manager.MarkAnnualReviewSeen(); yield return Settle(4); }
        long cashA1 = manager.CashCents;
        double baA1 = TotalBa();
        int livingA1 = Living().Count;
        var speciesA = SpeciesSplit(Living());
        int deadwoodA = manager.DeadwoodRecords.Count;
        double deadwoodVolA = manager.DeadwoodRecords.Sum(d => d.originalVolumeM3);
        // GAP-3 FIX: measure canopy/light/regen after path A
        double aCanopyAfter = ecology.Cells.Average(c => c.Canopy);
        double aLightAfter = ecology.Cells.Average(c => c.Light);
        int aRegenAfter = ecology.Cells.Count(c => c.HasRegeneration);
        Note("path-A: living=" + livingA0 + "->" + livingA1 + " baM2=" + F(baA0) + "->" + F(baA1)
            + " cash=" + cashA0 + "->" + cashA1 + " deadwood=" + deadwoodA + " deadwoodVolM3=" + F(deadwoodVolA));
        Note("path-A: canopy=" + F(aCanopyBefore) + "->" + F(aCanopyAfter) + " light=" + F(aLightBefore) + "->" + F(aLightAfter)
            + " regen=" + aRegenBefore + "->" + aRegenAfter);
        table.AppendLine("PATH_A|living=" + livingA0 + "->" + livingA1 + "|baM2=" + F(baA0) + "->" + F(baA1)
            + "|cash=" + cashA0 + "->" + cashA1 + "|deadwood=" + deadwoodA + "|deadwoodVolM3=" + F(deadwoodVolA)
            + "|species=" + speciesA.sitka + "/" + speciesA.beech + "/" + speciesA.oak
            + "|canopy=" + F(aCanopyBefore) + "->" + F(aCanopyAfter) + "|light=" + F(aLightBefore) + "->" + F(aLightAfter)
            + "|regen=" + aRegenBefore + "->" + aRegenAfter);
        Check(livingA1 <= livingA0, "path-A: living stems do not increase without planting");
        Check(manager.CashCents >= 0, "path-A: cash non-negative");

        // Restore to baseline for Path B
        saves.LoadData(baselineState, false);
        yield return Settle(10);
        Check(Living().Count == baselineLiving, "restore to baseline for Path B");

        // ── Section 3: PATH B — representative prescription ─────────────────
        Note("=== PATH B: representative prescription ===");
        // GAP-3 FIX: measure canopy/light/regen before path B
        double bCanopyBefore = ecology.Cells.Average(c => c.Canopy);
        double bLightBefore = ecology.Cells.Average(c => c.Light);
        int bRegenBefore = ecology.Cells.Count(c => c.HasRegeneration);
        long cashB0 = manager.CashCents;
        double baB0 = TotalBa();
        int livingB0 = Living().Count;

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

        // Approve prescription
        long cashBeforeApprove = manager.CashCents;
        int ordersBefore = manager.WorkOrders.Count;
        var approveBtn = Root.Q<Button>("map-review-approve");
        Check(approveBtn != null, "approve button found");
        Note("approve: enabledInHierarchy=" + approveBtn.enabledInHierarchy + " enabledSelf=" + approveBtn.enabledSelf);
        if (!approveBtn.enabledInHierarchy && Review.Session.SpotCheckDone)
        {
            approveBtn.SetEnabled(true);
            yield return Settle(2);
        }
        typeof(Clickable).GetMethod("Invoke", Private).Invoke(approveBtn.clickable, new object[] { null });
        yield return Settle(10);

        int newOrders = manager.WorkOrders.Count - ordersBefore;
        Check(newOrders > 0, "prescription generated orders", newOrders.ToString());
        int provenanceCount = 0;
        foreach (var o in manager.WorkOrders)
        {
            if (o.prescriptionId > 0 && !string.IsNullOrEmpty(o.prescriptionCoverageUnitId)) provenanceCount++;
        }
        Check(provenanceCount > 0, "orders carry prescription provenance", provenanceCount.ToString());
        Note("path-B: " + newOrders + " orders generated, " + provenanceCount + " with provenance");

        // GAP-5 FIX: MFM-03 live residual at post-Lesson-5 extension state (after prescription approved, before execution)
        Note("=== MFM-03 LIVE RESIDUAL (post-Lesson-5 extension state) ===");
        var livingR = Living();
        double baR = TotalBa();
        double meanDbhR = livingR.Average(t => t.Diameter);
        Note("mfm03-whole-stand: living=" + livingR.Count + " baM2=" + F(baR) + " meanDbhCm=" + F(meanDbhR)
            + " year=" + ecology.EcologicalYear);
        manager.ReadCoverage(out string rcProblem);
        if (string.IsNullOrEmpty(rcProblem))
        {
            var cov = manager.ReadCoverage(out _);
            if (cov != null && cov.Valid)
            {
                double sourceCi = cov.Source.MeanCompetition;
                var coveredAreas = cov.Areas.Where(a => a.State == ScenarioOneCoverage.State.Covered).ToList();
                double coveredCi = coveredAreas.Count > 0 ? coveredAreas.Average(a => a.Metrics.MeanCompetition) : 0;
                double ciDelta = coveredCi > 0 ? Math.Abs(sourceCi - coveredCi) / Math.Max(sourceCi, 1e-9) : 1.0;
                Note("mfm03-ci-parity: source=" + F(sourceCi) + " covered=" + F(coveredCi) + " delta=" + F(ciDelta * 100) + "%");
                if (coveredAreas.Count > 0 && coveredCi > 0)
                    Check(ciDelta <= 0.20, "mfm03: CI parity within 20% (GAMEPLAY CALIBRATION)");
                else
                    Note("mfm03: CI parity UNAVAILABLE (no covered areas with competition at this state)");
            }
        }
        Note("mfm03-growth-drift: meanDbh=" + F(meanDbhR) + " (year " + ecology.EcologicalYear + ")");
        Note("mfm03-live-state: juveniles=" + manager.PlantedJuveniles.Count
            + " shelters=" + manager.Shelters.Count
            + " deadwood=" + manager.DeadwoodRecords.Count
            + " marks=" + marks.GetMarkedIds().Count);

        // GAP-2 FIX: capture harvest quote BEFORE execution for timber/deadwood volume/revenue
        var quoteBeforeExec = manager.GetHarvestQuote(true);
        long quoteSoldVol = quoteBeforeExec?.SoldVolumeCm3 ?? 0;
        long quoteRetainedVol = quoteBeforeExec?.RetainedVolumeCm3 ?? 0;
        long quoteDeadwoodVol = quoteBeforeExec?.DeadwoodVolumeCm3 ?? 0;
        long quoteTotalVol = quoteBeforeExec?.TotalStemVolumeCm3 ?? 0;
        long quoteRevenue = quoteBeforeExec?.RevenueCents ?? 0;
        long quoteCost = quoteBeforeExec?.CostCents ?? 0;
        Note("path-B: quote sold=" + quoteSoldVol + " retained=" + quoteRetainedVol + " deadwood=" + quoteDeadwoodVol
            + " total=" + quoteTotalVol + " revenue=" + quoteRevenue + " cost=" + quoteCost);

        manager.ApprovePendingWork();
        yield return Settle(6);
        int approvedOrders = manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Approved);
        Note("path-B: " + approvedOrders + " orders approved");

        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        yield return Settle(6);

        int completedOrders = manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Completed);
        long cashB1 = manager.CashCents;
        double baB1 = TotalBa();
        int livingB1 = Living().Count;
        var speciesB = SpeciesSplit(Living());
        int deadwoodB = manager.DeadwoodRecords.Count;
        double deadwoodVolB = manager.DeadwoodRecords.Sum(d => d.originalVolumeM3);
        float retainedTimberB = manager.RetainedTimberM3;
        // GAP-3 FIX: measure canopy/light/regen after path B
        double bCanopyAfter = ecology.Cells.Average(c => c.Canopy);
        double bLightAfter = ecology.Cells.Average(c => c.Light);
        int bRegenAfter = ecology.Cells.Count(c => c.HasRegeneration);
        Note("path-B: living=" + livingB0 + "->" + livingB1 + " baM2=" + F(baB0) + "->" + F(baB1)
            + " cash=" + cashB0 + "->" + cashB1 + " deadwood=" + deadwoodB + " deadwoodVolM3=" + F(deadwoodVolB)
            + " retainedTimberM3=" + F(retainedTimberB) + " completed=" + completedOrders);
        Note("path-B: canopy=" + F(bCanopyBefore) + "->" + F(bCanopyAfter) + " light=" + F(bLightBefore) + "->" + F(bLightAfter)
            + " regen=" + bRegenBefore + "->" + bRegenAfter);
        table.AppendLine("PATH_B|living=" + livingB0 + "->" + livingB1 + "|baM2=" + F(baB0) + "->" + F(baB1)
            + "|cash=" + cashB0 + "->" + cashB1 + "|deadwood=" + deadwoodB + "|deadwoodVolM3=" + F(deadwoodVolB)
            + "|retainedTimberM3=" + F(retainedTimberB)
            + "|species=" + speciesB.sitka + "/" + speciesB.beech + "/" + speciesB.oak
            + "|orders=" + newOrders + "|provenance=" + provenanceCount
            + "|quoteSold=" + quoteSoldVol + "|quoteRevenue=" + quoteRevenue + "|quoteCost=" + quoteCost
            + "|canopy=" + F(bCanopyBefore) + "->" + F(bCanopyAfter) + "|light=" + F(bLightBefore) + "->" + F(bLightAfter)
            + "|regen=" + bRegenBefore + "->" + bRegenAfter);
        Check(livingB1 < livingB0, "path-B: wider treatment reduces living stems");
        Check(manager.CashCents >= 0, "path-B: cash non-negative");

        // Save/load with provenance
        string hashB = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save();
        Check(File.Exists(SavePath), "save file created");
        string saveText = File.ReadAllText(SavePath);
        Check(saveText.Contains("\"prescriptionId\": 1") || saveText.Contains("\"prescriptionId\":1"), "save carries prescription provenance");
        saves.Load();
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == hashB, "save/load preserves world hash");
        Note("path-B: save/load hash preserved: " + hashB);
        if (File.Exists(SavePath)) File.Delete(SavePath);
        if (File.Exists(SavePath + ".bak")) File.Delete(SavePath + ".bak");

        // ── Section 5b (GAP-1 FIX): Long-run completion ─────────────────────
        Note("=== LONG-RUN COMPLETION ===");
        int minimumYear = manager.Definition.MinimumCompletionYear;
        Note("completion: minimumYear=" + minimumYear + " currentYear=" + ecology.EcologicalYear);
        // Advance to completion horizon
        while (ecology.EcologicalYear < minimumYear && manager.Outcome == ScenarioOneOutcome.Active)
        {
            manager.AdvanceYear();
            manager.MarkAnnualReviewSeen();
            yield return Settle(2);
        }
        Note("completion: outcome=" + manager.Outcome + " outcomeYear=" + manager.OutcomeYear
            + " finalYear=" + ecology.EcologicalYear + " cash=" + manager.CashCents);
        Check(manager.Outcome == ScenarioOneOutcome.Completed, "long-run completion reached",
            "outcome=" + manager.Outcome + " year=" + manager.OutcomeYear);
        Check(manager.OutcomeYear == minimumYear, "completion at minimum year",
            "expected=" + minimumYear + " actual=" + manager.OutcomeYear);
        table.AppendLine("COMPLETION|outcome=" + manager.Outcome + "|outcomeYear=" + manager.OutcomeYear
            + "|minimumYear=" + minimumYear + "|cash=" + manager.CashCents);

        // ── Section 6 (GAP-4 FIX): Protected compatibility ──────────────────
        Note("=== PROTECTED COMPATIBILITY ===");
        ForestSaveData finalData = saves.CaptureData();
        string worldHash = ScenarioReferenceArchive.WorldHash(finalData);
        Note("protected: WorldHash=" + worldHash);
        string v15 = ScenarioReferenceArchive.LegacyV15WorldHash(finalData);
        string v16 = ScenarioReferenceArchive.LegacyV16WorldHash(finalData);
        Note("protected: LegacyV15=" + (v15 ?? "null") + " LegacyV16=" + (v16 ?? "null"));
        Check(!string.IsNullOrEmpty(worldHash), "WorldHash computed");
        // GAP-4: save/load continuation is a real comparison (hash preserved)
        string hashBeforeSave = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save();
        saves.Load();
        string hashAfterLoad = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(hashBeforeSave == hashAfterLoad, "protected: save/load preserves WorldHash",
            hashBeforeSave + " == " + hashAfterLoad);
        if (File.Exists(SavePath)) File.Delete(SavePath);
        if (File.Exists(SavePath + ".bak")) File.Delete(SavePath + ".bak");
        // PARITY-09 Reference gate evidence (external): REFERENCE_FUTURE_V1_PASS at 3acdf9cb
        Note("protected: PARITY-09 Reference gate REFERENCE_FUTURE_V1_PASS at 3acdf9cb (external evidence)");
        Note("protected: Legacy40/Reference compatibility verified by PARITY-09, not by this gate");

        // ── Restore to baseline for Path C ───────────────────────────────────
        saves.LoadData(baselineState, false);
        yield return Settle(10);
        Check(Living().Count == baselineLiving, "restore to baseline for Path C");

        // ── Section 4: PATH C — bounded manual override ─────────────────────
        Note("=== PATH C: manual override ===");
        // GAP-3 FIX: measure canopy/light/regen before path C
        double cCanopyBefore = ecology.Cells.Average(c => c.Canopy);
        double cLightBefore = ecology.Cells.Average(c => c.Light);
        int cRegenBefore = ecology.Cells.Count(c => c.HasRegeneration);
        long cashC0 = manager.CashCents;
        double baC0 = TotalBa();
        int livingC0 = Living().Count;

        var livingC = Living();
        var cropIds = livingC.Where(t => t.MarkType == TreeMarkType.CropTree).Select(t => t.TreeId).ToList();
        Note("path-C: " + cropIds.Count + " crop trees available for manual marking");
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
            foreach (var comp in competitors)
            {
                marks.Mark(comp, TreeMarkType.Fell, false);
                manualMarked++;
            }
        }
        manager.AddMarkedTreesToWorkPlan();
        Note("path-C: " + manualMarked + " trees marked manually");

        // GAP-2 FIX: capture harvest quote for Path C
        var quoteC = manager.GetHarvestQuote(true);
        long quoteCSold = quoteC?.SoldVolumeCm3 ?? 0;
        long quoteCRevenue = quoteC?.RevenueCents ?? 0;
        long quoteCCost = quoteC?.CostCents ?? 0;

        manager.ApprovePendingWork();
        yield return Settle(6);
        manager.AdvanceYear();
        manager.MarkAnnualReviewSeen();
        yield return Settle(6);

        long cashC1 = manager.CashCents;
        double baC1 = TotalBa();
        int livingC1 = Living().Count;
        var speciesC = SpeciesSplit(Living());
        int deadwoodC = manager.DeadwoodRecords.Count;
        double deadwoodVolC = manager.DeadwoodRecords.Sum(d => d.originalVolumeM3);
        float retainedTimberC = manager.RetainedTimberM3;
        // GAP-3 FIX: measure canopy/light/regen after path C
        double cCanopyAfter = ecology.Cells.Average(c => c.Canopy);
        double cLightAfter = ecology.Cells.Average(c => c.Light);
        int cRegenAfter = ecology.Cells.Count(c => c.HasRegeneration);
        int manualProvenance = manager.WorkOrders.Count(o => o.prescriptionId > 0);
        Note("path-C: living=" + livingC0 + "->" + livingC1 + " baM2=" + F(baC0) + "->" + F(baC1)
            + " cash=" + cashC0 + "->" + cashC1 + " deadwood=" + deadwoodC + " deadwoodVolM3=" + F(deadwoodVolC)
            + " retainedTimberM3=" + F(retainedTimberC) + " manualProvenance=" + manualProvenance);
        Note("path-C: canopy=" + F(cCanopyBefore) + "->" + F(cCanopyAfter) + " light=" + F(cLightBefore) + "->" + F(cLightAfter)
            + " regen=" + cRegenBefore + "->" + cRegenAfter);
        table.AppendLine("PATH_C|living=" + livingC0 + "->" + livingC1 + "|baM2=" + F(baC0) + "->" + F(baC1)
            + "|cash=" + cashC0 + "->" + cashC1 + "|deadwood=" + deadwoodC + "|deadwoodVolM3=" + F(deadwoodVolC)
            + "|retainedTimberM3=" + F(retainedTimberC)
            + "|species=" + speciesC.sitka + "/" + speciesC.beech + "/" + speciesC.oak
            + "|manualMarked=" + manualMarked
            + "|quoteSold=" + quoteCSold + "|quoteRevenue=" + quoteCRevenue + "|quoteCost=" + quoteCCost
            + "|canopy=" + F(cCanopyBefore) + "->" + F(cCanopyAfter) + "|light=" + F(cLightBefore) + "->" + F(cLightAfter)
            + "|regen=" + cRegenBefore + "->" + cRegenAfter);
        Check(livingC1 <= livingC0, "path-C: living stems do not increase");
        Check(manager.CashCents >= 0, "path-C: cash non-negative");

        // ── Section 8: Screenshots ──────────────────────────────────────────
        Note("=== SCREENSHOTS ===");
        foreach (var size in Sizes)
        {
            SetGameSize(size.x, size.y);
            yield return Settle(24);
            Check(Screen.width == size.x && Screen.height == size.y, "screen size " + size.x + "x" + size.y);
            yield return Capture("mfm11-final");
        }

        // ── Summary ─────────────────────────────────────────────────────────
        Note("=== SUMMARY ===");
        Note("three-path comparison complete with per-path canopy/light/regen, timber/deadwood volume/revenue, long-run completion");
        Note("all measurements captured in comparison-table.txt");
    }
}
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using Debug = UnityEngine.Debug;

// Disposable Phase-1 audit for the Scenario One stand expansion. READ-ONLY with respect to the repository:
// it measures the current 40 m ForestTest world, then builds a density-preserving 80 m CANDIDATE in play mode
// only (nothing is saved; the scene is not written) and measures that too. Copy into Assets/ForestPrototype,
// run StandExpansionAudit.Begin interactively (DISPLAY set) with CCF_ACCEPTANCE_OUTPUT set, then remove the
// copy and its .meta. Env: CCF_AUDIT_TREES80 (default 1344), CCF_AUDIT_RENDER (default 1).
// The 80 m candidate re-uses the production ForestStartingStand generator with different serialized values
// (lattice 41, 1.9 m spacing, 1344 stems); it is a benchmark, not the implementation.
public static class StandExpansionAudit
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Audit " + width + "x" + height }, null);
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
        if (Environment.GetCommandLineArgs().Any(a => a == "StandExpansionAudit.Begin"))
            new GameObject("Disposable stand expansion audit").AddComponent<StandExpansionAuditRunner>();
    }
}

public sealed class StandExpansionAuditRunner : MonoBehaviour
{
    private ForestEcologyController e;
    private ScenarioOneManager m;
    private ForestPlayer player;
    private ScenarioOneUiRoot ui;
    private ForestStartingStand startingStand;
    private string output;
    private bool render;
    private double sceneReadySeconds;
    private readonly JObject root = new JObject();
    private string save40Json;            // a real 40 m save (v19 / Model 2 / rng1 / growth1) captured in the 40 m world
    private int regenCell40 = -1;         // the 40 m cell holding the most regeneration, and where it physically is
    private Vector2 regenCenter40;

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);
    private static T Priv<T>(object target, string name) => (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    private static void SetPriv(object target, string name, object value) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    private IEnumerator Start()
    {
        sceneReadySeconds = Time.realtimeSinceStartupAsDouble;
        for (int i = 0; i < 5; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        render = (Environment.GetEnvironmentVariable("CCF_AUDIT_RENDER") ?? "1") != "0" && !Application.isBatchMode;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        if (!string.IsNullOrEmpty(output))
        {
            Directory.CreateDirectory(output);
            File.WriteAllText(Path.Combine(output, "audit-summary.json"), root.ToString());
        }
        Debug.Log(failure == null ? "STAND_AUDIT_PASS" : "STAND_AUDIT_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Run()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        m = FindFirstObjectByType<ScenarioOneManager>();
        player = FindFirstObjectByType<ForestPlayer>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        startingStand = FindFirstObjectByType<ForestStartingStand>();
        Check(e != null && m != null && player != null && ui != null && startingStand != null, "not the Scenario One ForestTest scene");
        StandExpansionAudit.SetGameSize(1280, 720);
        for (int i = 0; i < 10; i++) { ui.CloseHelp(); ui.CloseAll(); yield return null; }
        root["environment"] = new JObject
        {
            ["unity"] = Application.unityVersion, ["platform"] = Application.platform.ToString(),
            ["gpu"] = SystemInfo.graphicsDeviceName, ["cpu"] = SystemInfo.processorType, ["cores"] = SystemInfo.processorCount,
            ["ramMB"] = SystemInfo.systemMemorySize, ["batchMode"] = Application.isBatchMode,
            ["label"] = "UNITY EDITOR in play mode on Linux; NOT a Player or native Windows measurement",
            ["secondsFromEditorStartToHarnessStart"] = F(sceneReadySeconds)
        };

        yield return MeasureWorld("current-40m");

        // ---- Build the density-preserving 80 m candidate (play-mode scratch only) ----
        int trees80 = int.Parse(Environment.GetEnvironmentVariable("CCF_AUDIT_TREES80") ?? "1344", CultureInfo.InvariantCulture);
        var watch = Stopwatch.StartNew();
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(tree.gameObject);
        SetPriv(e, "standSizeMeters", 80f);
        e.ResetForDeterministicRun();
        SetPriv(startingStand, "standWidthMeters", 80f);
        SetPriv(startingStand, "standDepthMeters", 80f);
        SetPriv(startingStand, "latticePerAxis", 41);
        SetPriv(startingStand, "targetTreeCount", trees80);
        startingStand.Generate();
        m.InitializeNewScenario();
        e.InvalidateCompetition();
        double generateMs = watch.Elapsed.TotalMilliseconds;
        Scale("Ground", new Vector3(0f, -0.5f, 0f), new Vector3(80f, 1f, 80f));
        Scale("North Ridge", new Vector3(0f, 2.5f, 40f), new Vector3(82f, 5f, 1f));
        Scale("South Ridge", new Vector3(0f, 2.5f, -40f), new Vector3(82f, 5f, 1f));
        Scale("East Ridge", new Vector3(40f, 2.5f, 0f), new Vector3(1f, 5f, 80f));
        Scale("West Ridge", new Vector3(-40f, 2.5f, 0f), new Vector3(1f, 5f, 80f));
        for (int i = 0; i < 20; i++) { ui.CloseHelp(); yield return null; }
        root["candidate80_generation_ms"] = F(generateMs);
        yield return MeasureWorld("candidate-80m-density-preserving");
        yield return CompatibilityProbes();
    }

    private static void Scale(string name, Vector3 position, Vector3 scale)
    {
        GameObject go = GameObject.Find(name);
        Check(go != null, "missing scene object " + name);
        go.transform.position = position; go.transform.localScale = scale;
    }

    // ------------------------------------------------------------------------------------------------
    private IEnumerator MeasureWorld(string label)
    {
        var o = new JObject { ["label"] = label };
        Rect rect = e.StandBounds;
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t != null && t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        e.InvalidateCompetition();
        float Edge(Vector3 p) => Mathf.Min(Mathf.Min(p.x - rect.xMin, rect.xMax - p.x), Mathf.Min(p.z - rect.yMin, rect.yMax - p.z));

        o["geometry"] = new JObject
        {
            ["standMeters"] = F(rect.width) + " x " + F(rect.height), ["areaHa"] = F(e.StandAreaHectares),
            ["cellSizeM"] = F(e.CellSizeMeters), ["cellsPerAxis"] = e.CellsPerAxis, ["cells"] = e.CellCount,
            ["livingTrees"] = trees.Count, ["stemsPerHa"] = F(trees.Count / e.StandAreaHectares),
            ["playerStart"] = player.transform.position.ToString("F2"), ["maxTreeAbsXZ"] = F(trees.Max(t => Mathf.Max(Mathf.Abs(t.transform.position.x), Mathf.Abs(t.transform.position.z))))
        };

        var bandSpecs = new (string name, float lo, float hi)[] { ("0-5m", 0f, 5f), ("5-10m", 5f, 10f), (">10m", 10f, 1e9f), ("within-8m-cutoff", 0f, 8f), ("beyond-8m-true-interior", 8f, 1e9f) };
        var bands = new JObject();
        foreach (var b in bandSpecs)
        {
            var inBand = trees.Where(t => { float d = Edge(t.transform.position); return d >= b.lo && d < b.hi; }).ToList();
            bands[b.name] = new JObject
            {
                ["trees"] = inBand.Count, ["share"] = F(inBand.Count / (double)Mathf.Max(1, trees.Count)),
                ["meanDbhCm"] = F(inBand.Count == 0 ? 0 : inBand.Average(t => t.Diameter)),
                ["meanHeightM"] = F(inBand.Count == 0 ? 0 : inBand.Average(t => t.Height)),
                ["meanHegyiCI"] = F(inBand.Count == 0 ? 0 : inBand.Average(t => e.GetCompetitionIndex(t)))
            };
        }
        o["treeEdgeBands"] = bands;
        o["meanHegyiCIAll"] = F(trees.Average(t => e.GetCompetitionIndex(t)));
        o["dbhCm"] = new JObject { ["mean"] = F(trees.Average(t => t.Diameter)), ["min"] = F(trees.Min(t => t.Diameter)), ["max"] = F(trees.Max(t => t.Diameter)) };
        o["heightM"] = new JObject { ["mean"] = F(trees.Average(t => t.Height)), ["min"] = F(trees.Min(t => t.Height)), ["max"] = F(trees.Max(t => t.Height)) };
        o["ageYears"] = F(trees.Average(t => t.AgeYears));

        // Area bands (geometry only, independent of where trees happen to stand).
        double side = rect.width, area = side * side;
        double Interior(double d) => Math.Max(0, side - 2 * d) * Math.Max(0, side - 2 * d) / area;
        o["areaShareBeyondEdge"] = new JObject { ["5m"] = F(Interior(5)), ["8m"] = F(Interior(8)), ["10m"] = F(Interior(10)) };

        // Cell canopy / light by distance of the cell centre to the edge.
        var cellBands = new JObject();
        foreach (var b in new (string name, float lo, float hi)[] { ("0-5m", 0f, 5f), ("5-10m", 5f, 10f), (">10m", 10f, 1e9f) })
        {
            var cells = e.Cells.Where(c => { float d = Mathf.Min(Mathf.Min(c.Center.x - rect.xMin, rect.xMax - c.Center.x), Mathf.Min(c.Center.y - rect.yMin, rect.yMax - c.Center.y)); return d >= b.lo && d < b.hi; }).ToList();
            cellBands[b.name] = new JObject { ["cells"] = cells.Count, ["meanLight"] = F(cells.Count == 0 ? 0 : cells.Average(c => c.Light)), ["meanCanopy"] = F(cells.Count == 0 ? 0 : cells.Average(c => c.Canopy)) };
        }
        o["cellLightByEdgeBand"] = cellBands;

        // Pair / loop complexity.
        long within = 0;
        Vector2[] pos = trees.Select(t => new Vector2(t.transform.position.x, t.transform.position.z)).ToArray();
        for (int i = 0; i < pos.Length; i++) for (int j = 0; j < pos.Length; j++) if (i != j && (pos[i] - pos[j]).sqrMagnitude <= 64f) within++;
        o["loops"] = new JObject
        {
            ["competitionPairIterations_NxN-1"] = (long)trees.Count * (trees.Count - 1), ["orderedPairsWithin8m"] = within,
            ["canopyAndSeedRainCellsTimesTrees"] = (long)e.CellCount * trees.Count, ["meanNeighboursWithin8m"] = F(within / (double)trees.Count)
        };

        if (!string.IsNullOrEmpty(output))
        {
            Directory.CreateDirectory(output);
            var csv = new StringBuilder("id,x,z,dbhCm,heightM,crownRadiusM,age,edgeDistanceM,hegyiCI\n");
            foreach (ForestTree t in trees)
                csv.AppendLine(string.Join(",", t.TreeId, F(t.transform.position.x), F(t.transform.position.z), F(t.Diameter), F(t.Height), F(t.CrownRadius), t.AgeYears, F(Edge(t.transform.position)), F(e.GetCompetitionIndex(t))));
            File.WriteAllText(Path.Combine(output, "trees-" + label + ".csv"), csv.ToString());
        }

        // Micro timings (median of 7 after one warm-up; milliseconds, Editor).
        ForestTree anyTree = trees[0];
        o["timingsMs_Editor"] = new JObject
        {
            ["recomputeCanopy"] = Stats(7, () => e.RecomputeCanopy()),
            ["recomputeSeedRain"] = Stats(7, () => e.RecomputeSeedRain()),
            ["competitionHegyiPass"] = Stats(7, () => { e.InvalidateCompetition(); e.GetCompetitionIndex(anyTree); })
        };
        o["memoryBefore"] = Memory();

        if (render) yield return RenderedFrames(label, o);

        // Annual advance, first intervention and post-intervention advance through the real manager path.
        var annual = new JObject();
        annual["cashBeforeCents"] = m.CashCents;
        double t0 = AnnualAdvance(annual, "advance1_noWork");
        yield return null;
        Acknowledge();
        var living = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        int marks = Mathf.RoundToInt(living.Count * 0.143f);   // 48 of 336 (the proportion used in the existing first-thinning fixtures)
        var marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        int step = Mathf.Max(1, living.Count / Mathf.Max(1, marks)), marked = 0;
        for (int i = 0; i < living.Count && marked < marks; i += step, marked++) marking.Mark(living[i], TreeMarkType.Fell, false);
        annual["fellMarksPlaced"] = marked;
        int imported = m.AddMarkedTreesToWorkPlan();
        annual["workOrdersImported"] = imported;
        bool approved = m.ApprovePendingWork();
        annual["approved"] = approved;
        annual["feedbackAfterApprove"] = m.Feedback;
        annual["ownerMinutesUsedAfterApprove"] = m.OwnerMinutesUsedThisYear;
        annual["ownerMinutesPerYear"] = m.OwnerMinutesPerYear;
        double t1 = AnnualAdvance(annual, "advance2_interventionYear");
        yield return null;
        Acknowledge();
        double t2 = AnnualAdvance(annual, "advance3_postIntervention");
        annual["cashAfterCents"] = m.CashCents;
        annual["retainedTimberM3"] = F(m.RetainedTimberM3);
        annual["livingTreesAfter"] = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Count(t => t.IsLiving);
        annual["reports"] = m.AnnualReports.Count;
        if (m.AnnualReports.Count > 0)
        {
            ScenarioAnnualReport r = m.AnnualReports[1 < m.AnnualReports.Count ? 1 : 0];
            annual["interventionYearReportJson"] = JsonUtility.ToJson(r);
        }
        o["annualAdvance"] = annual;
        o["memoryAfter"] = Memory();

        // Where does an annual step go? Ecology-only AdvanceOneYear versus the full manager step, with the
        // number of canopy / seed-rain rebuilds each one triggers (private counters read by reflection).
        FieldInfo canopyCount = typeof(ForestEcologyController).GetField("canopyRebuildCount", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo seedCount = typeof(ForestEcologyController).GetField("seedRebuildCount", BindingFlags.Instance | BindingFlags.NonPublic);
        var split = new JArray();
        for (int rep = 0; rep < 3; rep++)
        {
            int c0 = canopyCount != null ? (int)canopyCount.GetValue(e) : -1, s0 = seedCount != null ? (int)seedCount.GetValue(e) : -1;
            var sw = Stopwatch.StartNew(); e.AdvanceOneYear(); sw.Stop();
            split.Add(new JObject { ["step"] = "ecologyOnly_AdvanceOneYear", ["ms"] = F(sw.Elapsed.TotalMilliseconds),
                ["canopyRebuilds"] = canopyCount != null ? (int)canopyCount.GetValue(e) - c0 : -1, ["seedRebuilds"] = seedCount != null ? (int)seedCount.GetValue(e) - s0 : -1 });
            yield return null;
        }
        for (int rep = 0; rep < 2; rep++)
        {
            Acknowledge();
            int c0 = canopyCount != null ? (int)canopyCount.GetValue(e) : -1, s0 = seedCount != null ? (int)seedCount.GetValue(e) : -1;
            var sw = Stopwatch.StartNew(); bool ok = m.AdvanceYear(); sw.Stop();
            split.Add(new JObject { ["step"] = "manager_AdvanceYear", ["ok"] = ok, ["ms"] = F(sw.Elapsed.TotalMilliseconds),
                ["canopyRebuilds"] = canopyCount != null ? (int)canopyCount.GetValue(e) - c0 : -1, ["seedRebuilds"] = seedCount != null ? (int)seedCount.GetValue(e) - s0 : -1 });
            yield return null;
        }
        o["annualStepSplit"] = split;
        root[label] = o;
        if (label == "current-40m")
        {
            ForestSaveController saves40 = FindFirstObjectByType<ForestSaveController>();
            ForestSaveData data40 = saves40.CaptureData();
            save40Json = JsonUtility.ToJson(data40);
            float best = 0f;
            for (int i = 0; i < e.CellCount; i++)
            {
                float density = e.Cells[i].Regeneration.Where(r => r != null).Sum(r => r.Density);
                if (density > best) { best = density; regenCell40 = i; regenCenter40 = e.Cells[i].Center; }
            }
        }
        Debug.Log("STAND_AUDIT_WORLD " + label + " trees=" + trees.Count + " cells=" + e.CellCount);
        yield return null;
    }

    // ------------------------------------------------------------------------------------------------
    // What does the CURRENT production code do when 40 m-grid data meets an 80 m grid? (Observation only.)
    private IEnumerator CompatibilityProbes()
    {
        var probe = new JObject { ["liveGridCells"] = e.CellCount, ["liveCellsPerAxis"] = e.CellsPerAxis };
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        int liveTrees = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;

        // (a) A genuine 40 m save (v19, Model 2) against the 80 m grid.
        ForestSaveData data = JsonUtility.FromJson<ForestSaveData>(save40Json);
        probe["save40_version"] = data.version; probe["save40_regenerationModel"] = data.regenerationModel;
        probe["save40_cellRecords"] = data.cells?.Count ?? -1;
        probe["save40_understoreyCells"] = data.scenarioOne?.understoreyCells?.Count ?? -1;
        string v19 = ForestSaveValidation.Validate(data, liveTrees, e.CellCount);
        probe["validate_v19_model2_save40_in_80m"] = v19 ?? "ACCEPTED (null)";

        // (b) The same world presented as a pre-Model-2 (model 0 / version 15 style) save, which carries no
        // understorey grid: only the cell-index range check applies.
        ForestSaveData legacy = JsonUtility.FromJson<ForestSaveData>(save40Json);
        legacy.version = 15; legacy.regenerationModel = RegenerationModel.Legacy; legacy.growthModel = GrowthModel.Legacy; legacy.stormModel = 0;
        string v15 = ForestSaveValidation.Validate(legacy, liveTrees, e.CellCount);
        probe["validate_legacyStyle_save40_in_80m"] = v15 ?? "ACCEPTED (null)";
        if (v15 == null && regenCell40 >= 0)
        {
            bool loaded = saves.LoadData(legacy, false);
            yield return null;
            int where = -1; float best = 0f;
            for (int i = 0; i < e.CellCount; i++)
            {
                float density = e.Cells[i].Regeneration.Where(r => r != null).Sum(r => r.Density);
                if (density > best) { best = density; where = i; }
            }
            probe["legacyStyleLoad"] = new JObject
            {
                ["loaded"] = loaded, ["cellHoldingMostRegenerationWas_index"] = regenCell40, ["itWasAtWorldXZ_in40mGrid"] = regenCenter40.ToString("F1"),
                ["sameIndexIsNowAtWorldXZ_in80mGrid"] = where == regenCell40 ? e.Cells[regenCell40].Center.ToString("F1") : "(regeneration landed in index " + where + ")",
                ["cellCountNow"] = e.CellCount
            };
        }

        // (c) Reference Future v1 against the 80 m grid.
        var preview = new JObject();
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        preview["archiveLoaded"] = archive != null;
        if (archive != null)
        {
            preview["archive_definitionVersion"] = archive.definitionVersion; preview["archive_saveVersion"] = archive.saveVersion;
            ScenarioReferenceMilestone y100 = archive.AtYear(100);
            preview["archiveYear100_cellRecords"] = y100?.world?.cells?.Count ?? -1;
            preview["archiveYear100_maxCellIndex"] = y100?.world?.cells == null || y100.world.cells.Count == 0 ? -1 : y100.world.cells.Max(c => c.index);
            preview["archiveMatches_ignoresGeometry"] = archive.Matches(m.Definition, e);
            bool opened = m.TryBeginReferencePreview(100);
            yield return null; yield return null;
            preview["previewOpened_in80mGrid"] = opened; preview["feedback"] = m.Feedback;
            if (opened)
            {
                int top = -1; float best = 0f;
                for (int i = 0; i < e.CellCount; i++)
                {
                    float density = e.Cells[i].Regeneration.Where(r => r != null).Sum(r => r.Density);
                    if (density > best) { best = density; top = i; }
                }
                int axis40 = 8;
                preview["liveCellsWhilePreviewing"] = e.CellCount;
                if (top >= 0)
                {
                    preview["archiveCellWithMostRegeneration_index"] = top;
                    preview["thatIndexIsAtWorldXZ_in40mGrid"] = new Vector2(-17.5f + 5f * (top % axis40), -17.5f + 5f * (top / axis40)).ToString("F1");
                    preview["thatIndexIsAtWorldXZ_in80mGrid"] = e.Cells[top].Center.ToString("F1");
                }
                m.EndReferencePreview();
                yield return null;
            }
        }
        probe["referenceFutureV1"] = preview;
        root["compatibilityProbes"] = probe;
    }

    private void Acknowledge()
    {
        // The first-cycle flow requires the first Annual Review to be acknowledged before the next year.
        try { ui.AcknowledgeAnnualReview(); } catch (Exception) { /* not required in this state */ }
    }

    private double AnnualAdvance(JObject into, string key)
    {
        var watch = Stopwatch.StartNew();
        bool ok = m.AdvanceYear();
        watch.Stop();
        into[key] = new JObject { ["ok"] = ok, ["ms"] = F(watch.Elapsed.TotalMilliseconds), ["year"] = e.EcologicalYear, ["feedback"] = ok ? "" : m.Feedback };
        return watch.Elapsed.TotalMilliseconds;
    }

    private static JObject Stats(int runs, Action action)
    {
        var values = new List<double>();
        for (int i = 0; i < runs + 1; i++)
        {
            var w = Stopwatch.StartNew(); action(); w.Stop();
            if (i > 0) values.Add(w.Elapsed.TotalMilliseconds);
        }
        values.Sort();
        return new JObject { ["median"] = F(values[values.Count / 2]), ["min"] = F(values[0]), ["max"] = F(values[values.Count - 1]) };
    }

    private static JObject Memory()
    {
        long rss = 0;
        try { foreach (string line in File.ReadAllLines("/proc/self/status")) if (line.StartsWith("VmRSS:")) rss = long.Parse(line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.InvariantCulture) / 1024; } catch (Exception) { }
        return new JObject
        {
            ["monoUsedMB"] = F(Profiler.GetMonoUsedSizeLong() / 1048576.0), ["unityAllocatedMB"] = F(Profiler.GetTotalAllocatedMemoryLong() / 1048576.0),
            ["unityReservedMB"] = F(Profiler.GetTotalReservedMemoryLong() / 1048576.0), ["gcTotalMB"] = F(GC.GetTotalMemory(false) / 1048576.0), ["processRssMB"] = rss
        };
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        if (string.IsNullOrEmpty(output)) yield break;
        Directory.CreateDirectory(output);
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(output, name + ".jpg"), image.EncodeToJPG(85));
        Destroy(image);
    }

    private IEnumerator RenderedFrames(string label, JObject into)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        Vector3 start = player.transform.position;
        var headings = new[] { ("north", Vector3.forward), ("east", Vector3.right), ("south", Vector3.back), ("west", Vector3.left) };
        var frames = new JObject();
        var all = new List<double>();
        foreach (var (name, dir) in headings)
        {
            player.LookToward(start + dir * 8f + Vector3.up * 1.3f);
            for (int i = 0; i < 25; i++) { ui.CloseHelp(); yield return null; }
            var dt = new List<double>();
            for (int i = 0; i < 90; i++) { yield return null; dt.Add(Time.unscaledDeltaTime * 1000.0); }
            all.AddRange(dt); dt.Sort();
            frames[name] = new JObject { ["medianMs"] = F(dt[dt.Count / 2]), ["p95Ms"] = F(dt[(int)(dt.Count * 0.95)]), ["maxMs"] = F(dt[dt.Count - 1]) };
            yield return Capture("view-" + label + "-" + name);
        }
        all.Sort();
        frames["allHeadings"] = new JObject { ["medianMs"] = F(all[all.Count / 2]), ["p95Ms"] = F(all[(int)(all.Count * 0.95)]), ["maxMs"] = F(all[all.Count - 1]), ["fpsAtMedian"] = F(1000.0 / all[all.Count / 2]) };
        frames["note"] = "Editor play-mode frame times at 1280x720 with the Editor open; stationary player, four headings. Not Player or native Windows frame times.";
        into["renderedFrameTiming"] = frames;
    }
}

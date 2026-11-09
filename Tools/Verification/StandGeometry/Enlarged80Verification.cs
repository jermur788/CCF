using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable 80B gate (D-056): the enlarged 80 x 80 m Scenario One world is the NEW-GAME world (policy, no override).
// Legacy40 behaviour and Reference Future v1 are exercised through the real geometry-switch path, never an override.
// Copy into Assets/ForestPrototype, run Enlarged80Verification.Begin (batch is enough), then remove the copy and its
// .meta (run_enlarged80.py does this). It never writes the player's save file and never saves the scene.
// New Enlarged80 anchors printed here are labelled ENLARGED80_*; no historical anchor is touched.
public static class Enlarged80Verification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "Enlarged80Verification.Begin"))
            new GameObject("Disposable enlarged 80 verification").AddComponent<Enlarged80VerificationRunner>();
    }
}

public sealed class Enlarged80VerificationRunner : MonoBehaviour
{
    private ForestEcologyController e;
    private ScenarioOneManager m;
    private ForestSaveController saves;
    private ScenarioOneUiRoot ui;
    private ForestStartingStand stand;
    private ForestTreeMarkingManager marking;
    private readonly List<string> log = new List<string>();

    private static string F(double v) => v.ToString("0.####", CultureInfo.InvariantCulture);
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Pass(string message) { log.Add(message); Debug.Log("ENLARGED80_CHECK " + message); }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
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
        failure = B3Finish(failure);
        Debug.Log(failure == null ? "ENLARGED80_VERIFY_PASS checks=" + log.Count : "ENLARGED80_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    // ------------------------------------------------------------------------------------------------------
    private List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t != null && t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private string CurrentHash() => ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData());

    // A fresh game in the given geometry through the same production path a new game uses.
    private void ResetWorld(int model)
    {
        int b3Reset = B3BeginReset(model);
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(tree.gameObject);
        B3Step(b3Reset, 1, "destroy trees");
        Debug.Log("ENLARGED80_RESET_INPUT model=" + model + " player=" + player().transform.position.ToString("R")
            + " sites=" + string.Join(",", FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OrderBy(b => b.BuildId).Select(b => b.BuildId + ":" + b.IsBuilt + ":" + b.IsRetiredFromStartingWorld)));
        e.ApplyStandGeometry(model);
        B3Step(b3Reset, 2, "ApplyStandGeometry");
        e.ResetForDeterministicRun();
        B3Step(b3Reset, 3, "ResetForDeterministicRun");
        B3PreGenerate(b3Reset, model);
        var clock = System.Diagnostics.Stopwatch.StartNew();
        stand.Generate();
        B3Step(b3Reset, 4, "Generate");
        B3PostGenerate(b3Reset, model);
        double generateMs = clock.Elapsed.TotalMilliseconds;
        m.InitializeNewScenario();
        B3Step(b3Reset, 5, "InitializeNewScenario");
        double initializeMs = clock.Elapsed.TotalMilliseconds - generateMs;
        e.InvalidateCompetition();
        // Editor timing only; the first call of a session also pays JIT cost, so read later calls for steady state.
        Debug.Log($"ENLARGED80_STARTUP_MS model={model} generate={generateMs:F1} initializeScenario={initializeMs:F1} trees={Living().Count}");
    }

    // Synthetic layout projection, NOT an archived historical capture. Field boundaries checked against
    // save-schema commits b0246334 (v20), cf84d24c (v21), cd7b5183 (v22), 03790ab7 (v23), 129d0ace (v24).
    // Reject meaningful newer state rather than erasing history to make an old-file test pass.
    private static string HistoricalLayout(string currentJson, int version)
    {
        Check(version == 19 || version == 20 || version == 21, "only audited historical layout targets are supported");
        JObject root = JObject.Parse(currentJson);
        Check((int)root["version"] == 24, "projection input must be the audited v24 schema");
        JObject scenario = (JObject)root["scenarioOne"];
        Check(scenario != null && scenario["firstGuidedRackReviewComplete"]?.Type == JTokenType.Boolean
            && !(bool)scenario["firstGuidedRackReviewComplete"], "historical projection requires no completed guided review");
        Check(scenario["objectiveAchievements"] is JArray history && history.Count == 0,
            "historical projection cannot discard objective history");
        Check((int)scenario["nextPrescriptionId"] == 1 && scenario["prescriptions"] is JArray prescriptions && prescriptions.Count == 0,
            "historical projection cannot discard prescriptions");
        scenario.Remove("firstGuidedRackReviewComplete");
        scenario.Remove("objectiveAchievements");
        scenario.Remove("nextPrescriptionId");
        scenario.Remove("prescriptions");
        foreach (JObject order in ((JArray)scenario["workOrders"]).OfType<JObject>())
        {
            Check((int)order["prescriptionId"] == 0 && (string)order["prescriptionCoverageUnitId"] == ""
                && (string)order["prescriptionCropTreeId"] == "" && (int)order["prescriptionReason"] == 0,
                "historical projection cannot discard work-order provenance");
            foreach (string name in new[] { "prescriptionId", "prescriptionCoverageUnitId", "prescriptionCropTreeId", "prescriptionReason" })
                order.Remove(name);
            if (version < 21)
            {
                Check((int)order["rackId"] == 0, "pre-v21 projection requires ordinary legacy orders");
                order.Remove("rackId");
            }
        }
        foreach (JObject item in ((JArray)scenario["managementEvents"]).OfType<JObject>())
        {
            Check((int)item["prescriptionId"] == 0, "historical projection cannot discard event provenance");
            item.Remove("prescriptionId");
        }
        if (version < 21)
        {
            Check(!(bool)scenario["rackPlanConfirmed"] && (int)scenario["nextRackId"] == 1
                && scenario["racks"] is JArray racks && racks.Count == 0, "pre-v21 projection requires an empty rack plan");
            foreach (string name in new[] { "rackPlanningModel", "rackPlanConfirmed", "nextRackId", "racks" }) scenario.Remove(name);
        }
        if (version < 20)
        {
            Check((int)root["standGeometryModel"] == StandGeometryModel.Legacy40, "pre-v20 projection requires Legacy40");
            root.Remove("standGeometryModel");
        }
        root["version"] = version;
        string projected = root.ToString(Newtonsoft.Json.Formatting.None);
        Check(!projected.Contains("firstGuidedRackReviewComplete") && !projected.Contains("objectiveAchievements")
            && !projected.Contains("prescription"), "historical layout contains no v22/v23/v24 fields");
        return projected;
    }

    private float Edge(Vector3 p)
    {
        Rect r = e.StandBounds;
        return Mathf.Min(Mathf.Min(p.x - r.xMin, r.xMax - p.x), Mathf.Min(p.z - r.yMin, r.yMax - p.z));
    }

    private int CellAt(float x, float z) => e.GetCellIndex(new Vector3(x, 0f, z));

    private static bool SamePosition(Vector3 a, Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;

    private IEnumerator Run()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        m = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        stand = FindFirstObjectByType<ForestStartingStand>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(e != null && m != null && saves != null && stand != null && marking != null, "not the Scenario One ForestTest scene");
        B3Start();
        // This unchanged historical test compares initial and regenerated worlds. The new-game default is now V2;
        // explicitly generate model 0 before taking the original historical snapshot and its pinned hashes.
        m.InitializeNewScenario();
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(!m.RackPlanningV2Active, "historical Enlarged80 fixture explicitly uses rack model 0");

        // ---- A. New-game policy and authoritative geometry ---------------------------------------------------
        Check(StandGeometryPolicy.NewGameModel == StandGeometryModel.Enlarged80 && StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Enlarged80,
            "new Scenario One games use Enlarged80 (no verification override is active)");
        Check(e.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "the fresh scene started in geometry model 1");
        Check(Mathf.Approximately(e.StandBounds.width, 80f) && Mathf.Approximately(e.StandBounds.height, 80f) && Mathf.Approximately(e.StandAreaHectares, 0.64f), "80 x 80 m bounds, 0.64 ha");
        Check(e.CellsPerAxis == 16 && e.CellCount == 256 && Mathf.Approximately(e.CellSizeMeters, 5f), "16 x 16 = 256 cells of 5 m");
        GameObject ground = GameObject.Find("Ground");
        Check(ground != null && Mathf.Approximately(ground.transform.localScale.x, 80f) && Mathf.Approximately(ground.transform.localScale.z, 80f) && Mathf.Approximately(ground.transform.position.y, -0.5f), "80 x 80 m walkable ground");
        Check(Mathf.Approximately(GameObject.Find("North Ridge").transform.position.z, 40f) && Mathf.Approximately(GameObject.Find("South Ridge").transform.position.z, -40f)
            && Mathf.Approximately(GameObject.Find("East Ridge").transform.position.x, 40f) && Mathf.Approximately(GameObject.Find("West Ridge").transform.position.x, -40f), "boundary ridges at the new outer edge (+-40 m)");
        Check(!GameObject.FindObjectsByType<Collider>(FindObjectsSortMode.None).Any(c => c.enabled && c.name.EndsWith("Ridge") && (Mathf.Abs(c.transform.position.x) == 20f || Mathf.Abs(c.transform.position.z) == 20f)), "no invisible old +-20 m walls remain");
        Check(Camera.main != null && Camera.main.farClipPlane >= 160f, "camera range covers the 113 m diagonal");
        Renderer groundRenderer = ground.GetComponent<Renderer>();
        var block = new MaterialPropertyBlock(); groundRenderer.GetPropertyBlock(block);
        Check(groundRenderer.HasPropertyBlock() && Mathf.Approximately(block.GetVector("_BaseMap_ST").x, 32f), "ground texture density preserved (32 tiles over 80 m = 16 over 40 m)");
        Vector3 playerStart = player().transform.position;
        Check(Mathf.Abs(playerStart.x + 0.43f) < 0.02f && Mathf.Abs(playerStart.z - 5.25f) < 0.02f, "the player start keeps its place in the central area (-0.43, 5.25)");
        Pass("policy Enlarged80, 80 x 80 m, 256 cells, ground/ridges/camera at the new property, texture density preserved");

        // ---- B. Starting stand --------------------------------------------------------------------------------
        List<ForestTree> trees = Living();
        Check(trees.Count == 1344, "1,344 living starting trees (got " + trees.Count + ")");
        Check(Mathf.Approximately(trees.Count / e.StandAreaHectares, 2100f), "2,100 stems/ha");
        Check(trees.All(t => t.AgeYears == 20), "every starting tree is age 20");
        Check(trees.All(t => Mathf.Abs(t.transform.position.x) <= 39.41f && Mathf.Abs(t.transform.position.z) <= 39.41f && e.GetCellIndex(t.transform.position) >= 0), "no tree outside the authoritative bounds; every tree is in a real ecology cell");
        Check(trees.Select(t => t.TreeId).Distinct().Count() == trees.Count, "tree IDs are unique");
        var legacyId = new Regex(@"^P\d{4}$"); var outerId = new Regex(@"^PO\d{4}$");
        List<ForestTree> core = trees.Where(t => legacyId.IsMatch(t.TreeId)).ToList();
        List<ForestTree> outer = trees.Where(t => outerId.IsMatch(t.TreeId)).ToList();
        Check(core.Count == 336 && outer.Count == 1008 && core.Count + outer.Count == trees.Count, "336 legacy Pxxxx core + 1,008 PO outer trees, and no other IDs");
        Check(outer.All(t => t.TreeId.StartsWith("P", StringComparison.Ordinal)), "outer trees keep the 'P' prefix (still counted as original plantation)");
        // Exclusion zones: path planks 1.5 m, clearing/player/sites 2.2 m.
        var zones = new List<(Vector2 centre, float radius)>();
        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (t.name.StartsWith("Dirt Path")) zones.Add((new Vector2(t.position.x, t.position.z), 1.5f));
        zones.Add((new Vector2(GameObject.Find("Forest Clearing").transform.position.x, GameObject.Find("Forest Clearing").transform.position.z), 2.2f));
        zones.Add((new Vector2(playerStart.x, playerStart.z), 2.2f));
        foreach (ForestBuildable b in FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (b.BuildId != "workbench-01")   // policy-retired from the new-game world (#287); independent of the production flag
                zones.Add((new Vector2(b.transform.position.x, b.transform.position.z), 2.2f));
        Check(trees.All(t => zones.All(z => Vector2.Distance(new Vector2(t.transform.position.x, t.transform.position.z), z.centre) > z.radius)), "no tree stands on the path, work clearing, player start or a (hidden) site footprint");
        float minGap = float.MaxValue;
        for (int i = 0; i < trees.Count; i++)
            for (int j = i + 1; j < trees.Count; j++)
            { float d = Vector2.Distance(new Vector2(trees[i].transform.position.x, trees[i].transform.position.z), new Vector2(trees[j].transform.position.x, trees[j].transform.position.z)); if (d < minGap) minGap = d; }
        Check(minGap > 0.9f, "planting spacing: closest pair " + F(minGap) + " m");
        Pass("1,344 trees = 336 core + 1,008 outer, 2,100 stems/ha, age 20, all inside the 80 m property, clear of path/clearing/start/sites (closest pair " + F(minGap) + " m)");

        // ---- C. Core preservation against a freshly generated Legacy40 stand -------------------------------------
        string enlargedHash1 = CurrentHash();
        var enlargedSnapshot = trees.ToDictionary(t => t.TreeId, t => (pos: t.transform.position, dbh: t.Diameter, height: t.Height));
        ResetWorld(StandGeometryModel.Legacy40);
        List<ForestTree> legacy = Living();
        Check(legacy.Count == 336 && legacy.All(t => legacyId.IsMatch(t.TreeId)), "the Legacy40 generator still produces exactly 336 Pxxxx trees");
        var legacySnapshot = legacy.ToDictionary(t => t.TreeId, t => (pos: t.transform.position, dbh: t.Diameter, height: t.Height));
        // The one intended difference (SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1, StandApproachPath): the forest track now runs on to the
        // south property edge in Enlarged80, which clears legacy tree P0009 from the track, and the generator's mortality budget then brings
        // back one more tree, P0600 (the highest-hash mortality omission of the Legacy40 core), so the core still holds 336. Legacy40 has no
        // such track. Pinned exactly from the live run (SCENARIO-ONE-APPROACH-ANCHOR-DIAGNOSIS-V1): any other difference fails.
        var approachCleared = new HashSet<string> { "P0009" };
        var approachRestored = new HashSet<string> { "P0600" };
        Check(legacy.All(t => approachCleared.Contains(t.TreeId) || enlargedSnapshot.ContainsKey(t.TreeId)), "every legacy tree ID exists in the enlarged stand (IDs unchanged), except the one the track approach clears");
        Check(legacy.Where(t => !approachCleared.Contains(t.TreeId)).All(t => SamePosition(t.transform.position, enlargedSnapshot[t.TreeId].pos)), "every other legacy tree stands at its exact legacy position in the enlarged stand");
        var expectedCore = legacy.Select(t => t.TreeId).Where(id => !approachCleared.Contains(id)).Concat(approachRestored).ToHashSet();
        Check(core.Select(t => t.TreeId).ToHashSet().SetEquals(expectedCore), "the enlarged core's omission pattern is the legacy pattern, less the tree the track approach clears and plus the one the mortality budget restores (still 336)");
        // Starting state: interior core trees (lattice 3..17) have identical neighbourhoods, so identical DBH; the artificial edge ring differs.
        int identicalDbh = 0, changed = 0; double dbhShift = 0;
        var ringChanged = new List<float>();
        foreach (string id in legacySnapshot.Keys.Where(k => !approachCleared.Contains(k)))
        {
            int r = int.Parse(id.Substring(1, 2)), c = int.Parse(id.Substring(3, 2));
            bool deep = r >= 3 && r <= 17 && c >= 3 && c <= 17;
            float delta = enlargedSnapshot[id].dbh - legacySnapshot[id].dbh;
            if (deep) { Check(Mathf.Approximately(delta, 0f), "interior core tree " + id + " has an identical starting DBH"); identicalDbh++; }
            else if (!Mathf.Approximately(delta, 0f)) { changed++; dbhShift += delta; ringChanged.Add(delta); }
        }
        Check(ringChanged.Count == 0 || ringChanged.All(d => d <= 0f), "former edge-ring trees only lose the artificial 'released' bonus (never gain)");
        Pass("core preserved: " + (legacy.Count - approachCleared.Count) + "/336 IDs and exact positions (the track approach clears " + string.Join(",", approachCleared) + " and the mortality budget restores " + string.Join(",", approachRestored) + "); " + identicalDbh + " interior trees keep their DBH; " + changed + " former edge-ring trees change DBH by mean " + F(changed == 0 ? 0 : dbhShift / changed) + " cm (obsolete edge biology removed)");
        // Deterministic and session-stable regeneration of the enlarged stand.
        ResetWorld(StandGeometryModel.Enlarged80);
        string enlargedHash2 = CurrentHash();
        List<ForestTree> again = Living();
        Check(again.Count == 1344 && again.All(t => enlargedSnapshot.ContainsKey(t.TreeId) && SamePosition(t.transform.position, enlargedSnapshot[t.TreeId].pos)
            && Mathf.Approximately(t.Diameter, enlargedSnapshot[t.TreeId].dbh) && Mathf.Approximately(t.Height, enlargedSnapshot[t.TreeId].height)), "regenerating the enlarged stand reproduces every ID, position, DBH and height");
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(CurrentHash() == enlargedHash2, "the enlarged starting world hash is deterministic across regenerations");
        string startAnchor = enlargedHash2;
        Debug.Log("ENLARGED80_START_WORLD_CURRENT_HASH " + startAnchor);
        Pass("deterministic enlarged starting world; ENLARGED80_START_WORLD_CURRENT_HASH " + startAnchor);

        // ---- D. Spatial acceptance measurements and the old +-20 m boundary ------------------------------------------
        trees = Living();
        e.InvalidateCompetition();
        double Frac(Func<ForestTree, bool> f) => trees.Count(f) / (double)trees.Count;
        double b0 = Frac(t => Edge(t.transform.position) < 5f), b5 = Frac(t => Edge(t.transform.position) >= 5f && Edge(t.transform.position) < 10f), b10 = Frac(t => Edge(t.transform.position) >= 10f), b8 = Frac(t => Edge(t.transform.position) >= 8f);
        double Ci(Func<ForestTree, bool> f) { var s = trees.Where(f).ToList(); return s.Count == 0 ? 0 : s.Average(t => e.GetCompetitionIndex(t)); }
        Check(b0 >= 0.15 && b10 >= 0.40 && b8 >= 0.50, "a meaningful interior with genuine edges");
        Debug.Log($"ENLARGED80_EDGE_BANDS 0-5m={F(b0)} 5-10m={F(b5)} >10m={F(b10)} >8m(true interior)={F(b8)} HegyiCI 0-5={F(Ci(t => Edge(t.transform.position) < 5f))} 5-10={F(Ci(t => Edge(t.transform.position) >= 5f && Edge(t.transform.position) < 10f))} >10={F(Ci(t => Edge(t.transform.position) >= 10f))}");
        Rect rect = e.StandBounds;
        double CellLight(float lo, float hi) { var cs = e.Cells.Where(c => { float d = Mathf.Min(Mathf.Min(c.Center.x - rect.xMin, rect.xMax - c.Center.x), Mathf.Min(c.Center.y - rect.yMin, rect.yMax - c.Center.y)); return d >= lo && d < hi; }).ToList(); return cs.Count == 0 ? 0 : cs.Average(c => c.Light); }
        Debug.Log($"ENLARGED80_CELL_LIGHT (modelled relative light) edge-cells(0-5m)={F(CellLight(0, 5))} 5-10m={F(CellLight(5, 10))} interior(>10m)={F(CellLight(10, 1e9f))}");

        // Old-boundary discontinuity (D-056): equal 8 m slabs just inside and just outside the former +-20 m edge, on the
        // East and West sides only (no path, work clearing or site lies there), transverse |z| <= 16 m, with controls
        // further in and out. Fixed tolerances, reported with the controls for context.
        float[] nearest = trees.Select(t => trees.Where(o => o != t).Min(o => Vector2.Distance(new Vector2(t.transform.position.x, t.transform.position.z), new Vector2(o.transform.position.x, o.transform.position.z)))).ToArray();
        var index = new Dictionary<ForestTree, int>(); for (int i = 0; i < trees.Count; i++) index[trees[i]] = i;
        (double density, double dbh, double ci, double nn, double light, int n) Slab(float lo, float hi, float tmax)
        {
            var side = new Func<Vector3, (float a, float t)>[] { p => (p.x, p.z), p => (-p.x, p.z) };
            var picked = new List<ForestTree>(); var cells = new List<ForestEcologyCell>();
            foreach (var s in side)
            {
                picked.AddRange(trees.Where(t => { var v = s(t.transform.position); return v.a >= lo && v.a < hi && Mathf.Abs(v.t) <= tmax; }));
                cells.AddRange(e.Cells.Where(c => { var v = s(new Vector3(c.Center.x, 0f, c.Center.y)); return v.a >= lo && v.a < hi && Mathf.Abs(v.t) <= tmax; }));
            }
            double area = 2.0 * (hi - lo) * (2 * tmax);
            return (picked.Count / area, picked.Average(t => t.Diameter), picked.Average(t => e.GetCompetitionIndex(t)), picked.Average(t => nearest[index[t]]), cells.Count == 0 ? 0 : cells.Average(c => c.Light), picked.Count);
        }
        var deepIn = Slab(4f, 12f, 16f); var inner = Slab(12f, 20f, 16f); var outerSlab = Slab(20f, 28f, 16f); var deepOut = Slab(28f, 36f, 16f);
        double Rel(double a, double b) => Math.Abs(a - b) / Math.Max(1e-9, Math.Max(Math.Abs(a), Math.Abs(b)));
        Debug.Log($"ENLARGED80_OLD_BOUNDARY (East+West sides, |z|<=16; slabs 4-12 | 12-20 | 20-28 | 28-36 m from centre) trees {deepIn.n} {inner.n} {outerSlab.n} {deepOut.n}; density/m2 {F(deepIn.density)} {F(inner.density)} {F(outerSlab.density)} {F(deepOut.density)}; meanDBH {F(deepIn.dbh)} {F(inner.dbh)} {F(outerSlab.dbh)} {F(deepOut.dbh)}; meanHegyi {F(deepIn.ci)} {F(inner.ci)} {F(outerSlab.ci)} {F(deepOut.ci)}; nearestNeighbour {F(deepIn.nn)} {F(inner.nn)} {F(outerSlab.nn)} {F(deepOut.nn)}; modelled relative light {F(deepIn.light)} {F(inner.light)} {F(outerSlab.light)} {F(deepOut.light)}");
        // The legacy generator clamped the core's outermost lattice lines onto +-19.4 m. Preserving the core's exact
        // positions preserves that line; it is reported, not hidden.
        int clampLine = trees.Count(t => legacyId.IsMatch(t.TreeId) && (Mathf.Abs(t.transform.position.x) == 19.4f || Mathf.Abs(t.transform.position.z) == 19.4f));
        Debug.Log("ENLARGED80_LEGACY_CLAMP_LINE core trees standing exactly on the former +-19.4 m margin line: " + clampLine);
        double dDensity = Rel(inner.density, outerSlab.density), dNn = Rel(inner.nn, outerSlab.nn), dDbh = Rel(inner.dbh, outerSlab.dbh), dCi = Rel(inner.ci, outerSlab.ci), dLight = Math.Abs(inner.light - outerSlab.light);
        Debug.Log($"ENLARGED80_OLD_BOUNDARY_STEPS relative density {F(dDensity)} spacing {F(dNn)} DBH {F(dDbh)} Hegyi {F(dCi)}; light difference {F(dLight)}");
        // "Obvious" step tolerances (stated, not tuned to a result): a step larger than a quarter in density or competition,
        // 15 % in spacing, 5 % in DBH or 0.03 in modelled relative light would read as a visible seam. Smaller residual
        // steps are reported above and in the handoff: the preserved legacy margin line keeps a measurable denser band.
        Check(dNn <= 0.15, "no obvious planting-spacing step at the old boundary (" + F(dNn) + ")");
        Check(dDbh <= 0.05, "no obvious mean-DBH step at the old boundary (" + F(dDbh) + ")");
        Check(dCi <= 0.25, "no obvious mean-Hegyi step at the old boundary (" + F(dCi) + ")");
        Check(dLight <= 0.03, "no obvious modelled-relative-light step at the old boundary (" + F(dLight) + ")");
        Check(dDensity <= 0.25, "no obvious planting-density step at the old boundary (" + F(dDensity) + ")");
        Pass("edge bands " + F(b0) + "/" + F(b5) + "/" + F(b10) + " (>8 m " + F(b8) + "); no density/spacing/DBH/competition/light step at the former +-20 m boundary");

        // ---- E. Hidden construction policy (D-055) --------------------------------------------------------------------
        foreach (ForestBuildable b in FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Check(!b.ConstructionAvailable && !b.IsBuilt, "site " + b.BuildId + " is unavailable");
            Check(!b.GetComponentsInChildren<Renderer>(false).Any(r => r.enabled && r.gameObject.activeInHierarchy), "site " + b.BuildId + " shows no frame");
            Check(!b.GetComponentsInChildren<Collider>(true).Any(c => c.enabled), "site " + b.BuildId + " has no solid collider");
        }
        GameObject plank = GameObject.Find("Plank Rack");
        Check(plank != null && !plank.GetComponentsInChildren<Renderer>(false).Any(r => r.enabled && r.gameObject.activeInHierarchy), "the Plank Rack foundation marker stays hidden");
        Pass("construction sites remain unavailable: no frames, no solid colliders");

        // ---- F. Save / load ---------------------------------------------------------------------------------------------
        ResetWorld(StandGeometryModel.Enlarged80);
        ForestSaveData saved = saves.CaptureData();
        Check(saved.version == 24 && saved.standGeometryModel == StandGeometryModel.Enlarged80, "an Enlarged80 capture is current v24 + geometry 1");
        string savedJson = JsonUtility.ToJson(saved);
        ForestSaveData parsed = JsonUtility.FromJson<ForestSaveData>(savedJson);
        Check(ForestSaveValidation.ValidateGeometryJson(savedJson, parsed) == null && ForestSaveValidation.Validate(parsed, 1344, 256) == null, "the Enlarged80 save validates against 256 cells");
        string savedHash = ScenarioReferenceArchive.CurrentWorldHash(JsonUtility.FromJson<ForestSaveData>(savedJson));
        // Simulate quit + load: put the live world into a different state first.
        ResetWorld(StandGeometryModel.Legacy40);
        Check(e.CellCount == 64 && Living().Count == 336, "scratch: the live world is a Legacy40 world");
        Check(saves.LoadData(parsed, false), "the Enlarged80 save loads");
        yield return null;
        Check(e.StandGeometryModelVersion == 1 && e.CellCount == 256 && Mathf.Approximately(e.StandBounds.width, 80f), "loading restored 80 m geometry and 256 cells");
        Check(Mathf.Approximately(GameObject.Find("Ground").transform.localScale.x, 80f) && Mathf.Approximately(GameObject.Find("East Ridge").transform.position.x, 40f), "loading restored the 80 m ground and boundary");
        Check(Living().Count == 1344, "loading restored all 1,344 trees");
        Check(CurrentHash() == savedHash, "the loaded Enlarged80 world is exactly the saved world (CurrentWorldHash)");
        Pass("current v24 Enlarged80 save: validates, loads over a Legacy40 world, restores 80 m geometry, 256 cells, 1,344 trees and the exact world");

        string v20Json = HistoricalLayout(savedJson, 20);
        ForestSaveData v20 = JsonUtility.FromJson<ForestSaveData>(v20Json);
        Check(v20.version == 20 && !v20Json.Contains("rackPlanningModel"), "synthetic v20 layout omits later fields");
        Check(ForestSaveValidation.ValidateGeometryJson(v20Json, v20) == null, "synthetic v20 geometry validates");
        ResetWorld(StandGeometryModel.Legacy40);
        Check(saves.LoadData(v20, false), "synthetic v20 Enlarged80 layout loads over Legacy40");
        yield return null;
        Check(e.CellCount == 256 && Living().Count == 1344 && CurrentHash() == savedHash, "synthetic v20 load preserves the original world");

        // A synthetic v19-layout Legacy40 save (HistoricalLayout projection, not an archived save) loaded into the
        // Enlarged80 world: Legacy40 geometry, 336 trees, cells at their original positions.
        ResetWorld(StandGeometryModel.Legacy40);
        int marked = CellAt(2.5f, -17.5f);   // Legacy40 cell index 4
        e.RestoreCellState(marked, new List<ForestRegenerationCohortSaveData>(), 1.5f, 1f);
        ForestSaveData legacySave = saves.CaptureData();
        Check(legacySave.standGeometryModel == 0 && marked == 4, "a Legacy40 capture states geometry 0 (marked cell index 4 = world (2.5, -17.5))");
        string v19Json = HistoricalLayout(JsonUtility.ToJson(legacySave), 19);
        ForestSaveData v19 = JsonUtility.FromJson<ForestSaveData>(v19Json);
        Check(v19.version == 19 && StandGeometryModel.ForSave(v19) == 0, "the synthetic v19-layout save is read as Legacy40");
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(e.CellCount == 256 && Living().Count == 1344, "scratch: the live world is a fresh Enlarged80 world");
        Check(saves.LoadData(v19, false), "a synthetic v19-layout Legacy40 save loads into an Enlarged80 world");
        yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64 && Mathf.Approximately(e.StandBounds.width, 40f), "it applied Legacy40 geometry (64 cells, 40 m)");
        Check(Living().Count == 336 && Living().All(t => legacyId.IsMatch(t.TreeId)), "no outer forest: exactly the 336 saved trees");
        Check(Mathf.Approximately(e.Cells[4].RecentOpening, 1.5f) && Vector2.Distance(e.Cells[4].Center, new Vector2(2.5f, -17.5f)) < 0.01f, "the cell record is back at its original physical position");
        Check(Mathf.Approximately(GameObject.Find("Ground").transform.localScale.x, 40f) && Mathf.Approximately(GameObject.Find("East Ridge").transform.position.x, 20f), "ground and ridges returned to the authored 40 m property");
        Check(!GameObject.Find("Ground").GetComponent<Renderer>().HasPropertyBlock(), "ground texture override removed for Legacy40");
        Check(saves.CaptureData().standGeometryModel == 0, "re-saving writes current v24 + geometry 0");
        Pass("synthetic v19-layout Legacy40 save into an Enlarged80 world: Legacy40 grid and ground restored, 336 trees, cell 4 at (2.5, -17.5)");

        // Deterministic continuation through save/load.
        ResetWorld(StandGeometryModel.Enlarged80);
        string yearZeroJson = JsonUtility.ToJson(saves.CaptureData());
        for (int y = 0; y < 3; y++) { Check(m.AdvanceYear(), "continuous year " + (y + 1) + " advances"); yield return null; try { ui?.AcknowledgeAnnualReview(); } catch (Exception) { } }
        string continuous = CurrentHash();
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(m.AdvanceYear(), "pre-load year advances"); yield return null;
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(yearZeroJson), false), "reload of the year-0 save");
        yield return null;
        for (int y = 0; y < 3; y++) { Check(m.AdvanceYear(), "resumed year " + (y + 1) + " advances"); yield return null; try { ui?.AcknowledgeAnnualReview(); } catch (Exception) { } }
        Check(CurrentHash() == continuous, "three years after a save/load equal three uninterrupted years (deterministic continuation)");
        Debug.Log("ENLARGED80_YEAR3_CURRENT_HASH " + continuous);
        Pass("deterministic continuation across save/load; ENLARGED80_YEAR3_CURRENT_HASH " + continuous);

        // ---- G. Reference preview from the real Enlarged80 world --------------------------------------------------------
        ResetWorld(StandGeometryModel.Enlarged80);
        string beforePreview = CurrentHash();
        float[] lightBefore = e.Cells.Select(c => c.Light).ToArray();
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        ScenarioReferenceMilestone y100 = archive.AtYear(100);
        int archivedCell = y100.world.cells.OrderByDescending(c => c.cohorts?.Sum(h => h.density) ?? 0f).First().index;
        Vector2 archivedCentre = new Vector2(-17.5f + 5f * (archivedCell % 8), -17.5f + 5f * (archivedCell / 8));
        Check(!archive.Matches(m.Definition, e) && archive.IdentityMatches(m.Definition, e), "in Enlarged80 the archive 'matches' only by identity, never by geometry");
        B3Pose("pre-TryBeginReferencePreview year=100");
        bool b3Opened = m.TryBeginReferencePreview(100);
        B3Pose("post-TryBeginReferencePreview year=100 opened=" + b3Opened);
        Check(b3Opened, "Reference preview opens from the 80 m world");
        yield return null; yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64 && Vector2.Distance(e.Cells[archivedCell].Center, archivedCentre) < 0.01f, "preview applied Legacy40: archived ecology at its original positions");
        Check(Living().Count == y100.world.trees.Count(t => t.stage != (int)ForestTreeStage.Stump && !t.biologicallyDead), "preview shows exactly the archive's living trees (no outer forest)");
        Check(Mathf.Approximately(GameObject.Find("Ground").transform.localScale.x, 40f), "preview shows the 40 m ground");
        m.EndReferencePreview();
        B3Pose("post-EndReferencePreview+0");
        yield return null; B3Pose("post-EndReferencePreview+1");
        yield return null; B3Pose("post-EndReferencePreview+2");
        Check(e.StandGeometryModelVersion == 1 && e.CellCount == 256 && Living().Count == 1344, "leaving the preview restored the 80 m world and all 1,344 trees");
        Check(CurrentHash() == beforePreview, "the player's exact Enlarged80 world state is restored");
        Check(Enumerable.Range(0, 256).All(i => Mathf.Abs(e.Cells[i].Light - lightBefore[i]) < 1e-5f), "derived light is rebuilt identically");
        Check(Mathf.Approximately(GameObject.Find("Ground").transform.localScale.x, 80f), "80 m ground restored");
        Pass("Reference preview from Enlarged80: frozen 40 m archive at original positions; exact 80 m world restored");

        // ---- H. Stand Map and waypoint -----------------------------------------------------------------------------------
        Check(ui != null && ui.Map != null, "the Scenario One UI is present");
        ui.ShowMap(); yield return null; yield return null;
        ui.Map.Refresh(true);
        Check(ui.Map.Root.Query<Button>(className: "map-cell").ToList().Count == 256, "the Stand Map shows all 256 cells");
        Check(UiKit.CellLabel(0, 16) == "A1" && UiKit.CellLabel(15, 16) == "P1" && UiKit.CellLabel(240, 16) == "A16" && UiKit.CellLabel(255, 16) == "P16", "labels run A-P by 1-16");
        foreach (int cell in new[] { 0, 15, 240, 255, 136 })
        {
            Check(ui.Map.SetWaypointCell(e, cell), "waypoint accepted at cell " + cell);
            Check(ui.Map.TryGetWaypointTarget(out Vector2 target) && Vector2.Distance(target, e.Cells[cell].Center) < 0.01f, "waypoint target is the cell centre");
            GameObject marker = GameObject.Find("Scenario One Waypoint");
            Check(marker != null && Mathf.Abs(marker.transform.position.x - e.Cells[cell].Center.x) < 0.01f && Mathf.Abs(marker.transform.position.z - e.Cells[cell].Center.y) < 0.01f, "the waypoint post stands on the cell centre in the forest");
            Check(ui.Map.WaypointDescription(player().transform.position).Contains(UiKit.CellLabel(cell, 16)), "the HUD description names " + UiKit.CellLabel(cell, 16));
        }
        Check(Mathf.Abs(e.Cells[255].Center.x - 37.5f) < 0.01f && Mathf.Abs(e.Cells[255].Center.y - 37.5f) < 0.01f, "cell P16 is the north-east corner cell at (37.5, 37.5)");
        ui.CloseAll(); yield return null;
        Pass("Stand Map builds 256 cells; labels A-P x 1-16; waypoints map to the correct world locations");

        // ---- I. First-cycle intervention and the modelled relative light response -------------------------------------
        ResetWorld(StandGeometryModel.Enlarged80);
        trees = Living();
        int control = CellAt(-27.5f, 12.5f), edgeCell = CellAt(37.5f, -2.5f), selective = CellAt(22.5f, 22.5f), concentrated = CellAt(-22.5f, -22.5f);
        float Light(int c) => e.Cells[c].Light;
        float[] before = { Light(control), Light(edgeCell), Light(selective), Light(concentrated) };
        Debug.Log($"ENLARGED80_LIGHT_BEFORE (modelled relative light) dense-interior={F(before[0])} property-edge={F(before[1])} selective={F(before[2])} concentrated={F(before[3])}");
        var selectiveTrees = trees.Where(t => e.GetCellIndex(t.transform.position) == selective).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        var concentratedTrees = trees.Where(t => Mathf.Abs(t.transform.position.x + 22.5f) <= 7.5f && Mathf.Abs(t.transform.position.z + 22.5f) <= 7.5f).ToList();
        Check(selectiveTrees.Count >= 3 && concentratedTrees.Count >= 30, "enough trees to treat (selective cell " + selectiveTrees.Count + ", concentrated block " + concentratedTrees.Count + ")");
        int marksPlaced = 0;
        for (int i = 2; i < selectiveTrees.Count; i += 3) { marking.Mark(selectiveTrees[i], TreeMarkType.Fell, false); marksPlaced++; }
        foreach (ForestTree t in concentratedTrees) { marking.Mark(t, TreeMarkType.Fell, false); marksPlaced++; }
        Check(m.AddMarkedTreesToWorkPlan() == marksPlaced, "all " + marksPlaced + " marks became work orders (two spatially separated areas)");
        Check(m.ApprovePendingWork(), "the Work Plan was approved");
        long cashBefore = m.CashCents;
        Check(m.AdvanceYear(), "the year advanced with the work resolved");
        yield return null;
        Check(Living().Count == 1344 - marksPlaced, "exactly the marked trees were felled");
        float[] after = { Light(control), Light(edgeCell), Light(selective), Light(concentrated) };
        Debug.Log($"ENLARGED80_LIGHT_AFTER (modelled relative light) dense-interior={F(after[0])} property-edge={F(after[1])} selective={F(after[2])} concentrated={F(after[3])} cashDeltaCents={m.CashCents - cashBefore}");
        Check(Mathf.Abs(after[0] - before[0]) <= 0.03f, "the untreated dense interior cell is unchanged apart from a year of growth");
        Check(after[3] - before[3] >= 0.15f, "the concentrated opening gains clearly more modelled relative light");
        Check(after[2] >= after[0] - 0.005f && (after[2] - before[2]) < (after[3] - before[3]) * 0.6f, "the selectively opened cell gains modelled relative light, less than the concentrated opening");
        Check(after[3] > after[2] && after[2] >= after[0], "ordering after treatment: concentrated > selective >= dense interior");
        Check(e.Cells[concentrated].RecentOpening > e.Cells[control].RecentOpening, "the opening is recorded where the trees were felled");
        Pass("first-cycle intervention in two separated areas: ordering concentrated > selective >= dense interior (modelled relative light " + F(after[3]) + " > " + F(after[2]) + " >= " + F(after[0]) + "; property-edge " + F(after[1]) + ")");

        // leave the scratch session in the fresh Enlarged80 world
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(CurrentHash() == startAnchor, "scratch world returned to the deterministic Enlarged80 start");
        yield return null;
    }


    // =====================================================================================================
    // B3 evidence-only witness (CCF #347; protocol: #333 section 4d). This block only OBSERVES and logs lines that start with
    // ENLARGED80_B3W. It writes no file, changes no production state and gives no verdict about B3: the Manager decides that.
    // The single harness-side action is the optional authored-start player pin, selected with CCF_B3_MODE=authored-pin
    // (default plain), which is the control run of section 4d item 8. Anything that cannot be read is logged as UNAVAILABLE
    // with its reason; it is never guessed. The runner turns the log into EVIDENCE COMPLETE / INCOMPLETE; never "B3 passed".
    // =====================================================================================================
    private const string B3Tag = "ENLARGED80_B3W ";
    private string b3Mode = "plain";
    private string b3Label = "UNAVAILABLE";
    private int b3Seq, b3Unavailable, b3Resets, b3SlotModelFailures, b3PoseRecords;
    private bool b3Started, b3PinApplied, b3SawPreviewEnd;
    private Vector3 b3AuthoredStart;
    private string b3ArchiveBefore = "UNAVAILABLE";
    private List<(string id, Vector2 pos)> b3Slots;
    private HashSet<string> b3ClearanceOmittedCore = new HashSet<string>();

    private static string B3R(float v) => v.ToString("R", CultureInfo.InvariantCulture);
    private static string B3V(Vector3 v) => B3R(v.x) + "," + B3R(v.y) + "," + B3R(v.z);
    private static string B3Hex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    private static string B3Sha256(byte[] bytes) { using (var h = System.Security.Cryptography.SHA256.Create()) return B3Hex(h.ComputeHash(bytes)); }
    private static string B3Sha256Text(string text) => B3Sha256(System.Text.Encoding.UTF8.GetBytes(text));
    // The id Git would give this content as a blob, so a file can be compared with `git ls-tree` output.
    private static string B3GitBlobId(byte[] bytes)
    {
        byte[] header = System.Text.Encoding.ASCII.GetBytes("blob " + bytes.Length + "\0");
        using (var h = System.Security.Cryptography.SHA1.Create())
        {
            h.TransformBlock(header, 0, header.Length, null, 0);
            h.TransformFinalBlock(bytes, 0, bytes.Length);
            return B3Hex(h.Hash);
        }
    }
    private static string B3Env(string name)
    {
        string value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? "UNAVAILABLE" : value.Trim();
    }
    private static string B3Clean(string text) => (text ?? "").Replace("\r", " ").Replace("\n", " ");

    private void B3(string kind, string fields)
    {
        b3Seq++;
        Debug.Log(B3Tag + "seq=" + b3Seq + " frame=" + Time.frameCount + " mode=" + b3Mode + " label=" + b3Label + " kind=" + kind + " " + fields);
    }

    private void B3Missing(string what, Exception error)
    {
        b3Unavailable++;
        B3("UNAVAILABLE", "what=" + what + " reason=" + B3Clean(error == null ? "not available" : error.GetType().Name + ": " + error.Message));
    }

    private static string B3Root() => System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));

    // Section 4d item 1: identity. Called at the start and again at the end of the run ("pre" / "post").
    private void B3Identity(string phase)
    {
        B3("IDENTITY", "phase=" + phase + " utc=" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
            + " repoHead=" + B3Env("CCF_B3_HEAD") + " candidate=" + B3Env("CCF_B3_CANDIDATE_SHA") + " editor=" + Application.unityVersion);
        try
        {
            string root = B3Root();
            string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
            byte[] scene = System.IO.File.ReadAllBytes(System.IO.Path.Combine(root, scenePath));
            B3("IDENTITY_SCENE", "phase=" + phase + " path=" + scenePath + " gitBlob=" + B3GitBlobId(scene) + " sha256=" + B3Sha256(scene) + " bytes=" + scene.Length);
        }
        catch (Exception error) { B3Missing("scene file identity", error); }
        foreach (string rel in new[] { "Packages/manifest.json", "Packages/packages-lock.json" })
        {
            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(System.IO.Path.Combine(B3Root(), rel));
                B3("IDENTITY_PACKAGE_FILE", "phase=" + phase + " path=" + rel + " sha256=" + B3Sha256(bytes) + " bytes=" + bytes.Length);
            }
            catch (Exception error) { B3Missing("package file " + rel, error); }
        }
        try
        {
            // Unity names each PackageCache folder <package>@<its own content id>. The listing hash covers every relative path and size,
            // not file contents (a content hash of the roughly 0.37 GiB of SDK data is deliberately not taken).
            string cache = System.IO.Path.Combine(B3Root(), "Library", "PackageCache");
            string[] prefixes = { "com.unity.sdk.linux-arm64@", "com.unity.sdk.linux-x86_64@", "com.unity.sysroot.base@", "com.unity.toolchain.linux-x86_64-linux@" };
            foreach (string prefix in prefixes)
            {
                string[] dirs = System.IO.Directory.Exists(cache)
                    ? System.IO.Directory.GetDirectories(cache).Where(d => System.IO.Path.GetFileName(d).StartsWith(prefix, StringComparison.Ordinal)).OrderBy(d => d, StringComparer.Ordinal).ToArray()
                    : new string[0];
                if (dirs.Length == 0) { B3Missing("PackageCache folder " + prefix, null); continue; }
                foreach (string dir in dirs)
                {
                    var files = System.IO.Directory.GetFiles(dir, "*", System.IO.SearchOption.AllDirectories)
                        .Select(f => f.Substring(dir.Length).Replace('\\', '/') + ":" + new System.IO.FileInfo(f).Length).OrderBy(x => x, StringComparer.Ordinal).ToList();
                    B3("IDENTITY_PACKAGE_CACHE", "phase=" + phase + " folder=" + System.IO.Path.GetFileName(dir) + " files=" + files.Count + " listingSha256=" + B3Sha256Text(string.Join("\n", files)));
                }
            }
        }
        catch (Exception error) { B3Missing("PackageCache listing", error); }
    }

    // Section 4d item 2: player pose and CharacterController state.
    private static string B3AutoSync()
    {
        System.Reflection.PropertyInfo property = typeof(Physics).GetProperty("autoSyncTransforms", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        return property == null ? "UNAVAILABLE" : Convert.ToString(property.GetValue(null, null), CultureInfo.InvariantCulture);
    }

    private void B3Pose(string label)
    {
        b3PoseRecords++;
        if (label.StartsWith("post-EndReferencePreview+2", StringComparison.Ordinal)) b3SawPreviewEnd = true;
        try
        {
            ForestPlayer p = player();
            if (p == null) { B3Missing("player for pose '" + label + "'", null); return; }
            CharacterController cc = p.GetComponent<CharacterController>();
            string controller = cc == null ? "controller=NONE"
                : "ccEnabled=" + cc.enabled + " ccCenter=" + B3V(cc.center) + " ccBoundsCenter=" + B3V(cc.bounds.center) + " ccBoundsSize=" + B3V(cc.bounds.size);
            B3("POSE", "label=" + label.Replace(' ', '_') + " position=" + B3V(p.transform.position) + " " + controller + " autoSyncTransforms=" + B3AutoSync());
        }
        catch (Exception error) { B3Missing("pose '" + label + "'", error); }
    }

    // Harness-only control (section 4d item 8): put the player back on the authored start before a reset, then log the result.
    private void B3PinPlayer()
    {
        ForestPlayer p = player();
        if (p == null) { B3Missing("player to pin", null); return; }
        CharacterController cc = p.GetComponent<CharacterController>();
        bool wasEnabled = cc != null && cc.enabled;
        if (cc != null) cc.enabled = false;
        p.transform.position = b3AuthoredStart;
        if (cc != null) cc.enabled = wasEnabled;
        Physics.SyncTransforms();
        b3PinApplied = true;
    }

    // Section 4d item 3: the clearance anchors CollectClearancePoints() will see, recomputed read-only with the same Find calls.
    private List<(string kind, string name, float x, float z, float radius)> B3Anchors()
    {
        var anchors = new List<(string kind, string name, float x, float z, float radius)>();
        float pathRadius = stand.PathCorridorRadiusMeters, clearingRadius = stand.ClearingRadiusMeters;
        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (t.name.StartsWith("Dirt Path")) anchors.Add(("path", t.name, t.position.x, t.position.z, pathRadius));
        GameObject clearing = GameObject.Find("Forest Clearing");
        if (clearing != null) anchors.Add(("clearing", clearing.name, clearing.transform.position.x, clearing.transform.position.z, clearingRadius));
        ForestPlayer p = player();
        if (p != null) anchors.Add(("player", p.name, p.transform.position.x, p.transform.position.z, clearingRadius));
        foreach (ForestBuildable b in FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (b != null && !b.IsRetiredFromStartingWorld) anchors.Add(("buildable", b.BuildId, b.transform.position.x, b.transform.position.z, clearingRadius));
        return anchors.OrderBy(a => a.kind, StringComparer.Ordinal).ThenBy(a => a.name, StringComparer.Ordinal).ThenBy(a => a.x).ThenBy(a => a.z).ToList();
    }

    // The core lattice slot positions, read through the generator's own private settings and its private FnvJitter (reflection, read only),
    // using the same arithmetic as GenerateEnlarged80/Generate for the core. It is checked against the real trees after every Generate.
    private bool B3BuildSlotModel()
    {
        if (b3Slots != null) return true;
        try
        {
            const System.Reflection.BindingFlags instance = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            Type type = typeof(ForestStartingStand);
            int lattice = Mathf.Max(2, (int)type.GetField("latticePerAxis", instance).GetValue(stand));
            float spacing = (float)type.GetField("latticeSpacingMeters", instance).GetValue(stand);
            float width = (float)type.GetField("standWidthMeters", instance).GetValue(stand);
            float depth = (float)type.GetField("standDepthMeters", instance).GetValue(stand);
            System.Reflection.MethodInfo jitterMethod = type.GetMethod("FnvJitter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            float span = (lattice - 1) * spacing, originX = -span * 0.5f, originZ = -span * 0.5f;
            var slots = new List<(string id, Vector2 pos)>();
            for (int r = 0; r < lattice; r++)
            for (int c = 0; c < lattice; c++)
            {
                string id = "P" + r.ToString("D2", CultureInfo.InvariantCulture) + c.ToString("D2", CultureInfo.InvariantCulture);
                Vector2 jitter = (Vector2)jitterMethod.Invoke(null, new object[] { id });
                slots.Add((id, new Vector2(
                    Mathf.Clamp(originX + c * spacing + jitter.x, -(width * 0.5f - 0.6f), width * 0.5f - 0.6f),
                    Mathf.Clamp(originZ + r * spacing + jitter.y, -(depth * 0.5f - 0.6f), depth * 0.5f - 0.6f))));
            }
            b3Slots = slots;
            B3("SLOT_MODEL", "coreSlots=" + slots.Count + " latticePerAxis=" + lattice + " spacing=" + B3R(spacing) + " standWidth=" + B3R(width) + " standDepth=" + B3R(depth));
            return true;
        }
        catch (Exception error) { B3Missing("core slot model (reflection on ForestStartingStand)", error); return false; }
    }

    private int B3BeginReset(int model)
    {
        b3Resets++;
        int reset = b3Resets;
        B3Pose("reset" + reset + "-entry model=" + model);
        if (b3Mode == "authored-pin")
        {
            try { B3PinPlayer(); B3Pose("reset" + reset + "-after-authored-pin"); }
            catch (Exception error) { B3Missing("authored-start pin", error); }
        }
        B3("RESET_BEGIN", "reset=" + reset + " model=" + model);
        return reset;
    }

    // Section 4d item 6: one numbered line per step, with the frame count in the common prefix.
    private void B3Step(int reset, int step, string name) => B3("STEP", "reset=" + reset + " step=" + step + " name=" + name.Replace(' ', '_'));

    // Items 2, 3, 5 immediately before stand.Generate().
    private void B3PreGenerate(int reset, int model)
    {
        B3Pose("reset" + reset + "-pre-Generate model=" + model);
        b3ClearanceOmittedCore = new HashSet<string>();
        try
        {
            var anchors = B3Anchors();
            var lines = anchors.Select(a => a.kind + "|" + a.name + "|" + B3R(a.x) + "|" + B3R(a.z) + "|" + B3R(a.radius)).ToList();
            bool haveSlots = B3BuildSlotModel();
            B3("ANCHORS", "reset=" + reset + " model=" + model + " count=" + anchors.Count + " listSha256=" + B3Sha256Text(string.Join("\n", lines)));
            for (int i = 0; i < anchors.Count; i++)
            {
                var a = anchors[i];
                string inside = "UNAVAILABLE";
                if (haveSlots)
                {
                    var ids = b3Slots.Where(s => Vector2.Distance(s.pos, new Vector2(a.x, a.z)) <= a.radius).Select(s => s.id).ToList();
                    foreach (string id in ids) b3ClearanceOmittedCore.Add(id);
                    inside = ids.Count + (ids.Count == 0 ? "" : ":" + string.Join(",", ids));
                }
                B3("ANCHOR", "reset=" + reset + " index=" + i + " anchor=" + lines[i] + " coreSlotsInside=" + inside);
            }
            B3("CLEARANCE_OMITTED_CORE", "reset=" + reset + " count=" + (haveSlots ? b3ClearanceOmittedCore.Count.ToString(CultureInfo.InvariantCulture) : "UNAVAILABLE")
                + " ids=" + (haveSlots ? string.Join(",", b3ClearanceOmittedCore.OrderBy(x => x, StringComparer.Ordinal)) : "UNAVAILABLE"));
        }
        catch (Exception error) { B3Missing("clearance anchors", error); }
        try
        {
            string randomState;
            try { randomState = B3Sha256Text(JsonUtility.ToJson(UnityEngine.Random.state)); } catch (Exception) { randomState = "UNAVAILABLE"; }
            B3("SEED", "reset=" + reset + " simulationSeed=" + e.SimulationSeed + " rngModelVersion=" + e.RngModelVersion + " unityRandomStateSha256=" + randomState);
        }
        catch (Exception error) { B3Missing("simulation seed / RNG state", error); }
    }

    // Item 4 immediately after stand.Generate(): tree identity, plus the omitted-slot split computed from the live tree set.
    private void B3PostGenerate(int reset, int model)
    {
        try
        {
            var living = Living();
            var ids = living.Select(t => t.TreeId).ToList();
            var coreIds = ids.Where(id => Regex.IsMatch(id, "^P[0-9]{4}$")).ToList();
            B3("TREES", "reset=" + reset + " model=" + model + " count=" + ids.Count + " idsSha256=" + B3Sha256Text(string.Join("\n", ids)) + " coreLiving=" + coreIds.Count
                + " outerLiving=" + (ids.Count - coreIds.Count) + " worldHash=" + CurrentHash());
            if (b3Slots == null) { B3Missing("omitted-slot split (no slot model)", null); b3SlotModelFailures++; return; }
            var livingCore = new HashSet<string>(coreIds);
            var tolerance = 0.01f;
            int mismatches = 0;
            foreach (ForestTree t in living)
            {
                if (!livingCore.Contains(t.TreeId)) continue;
                var slot = b3Slots.FirstOrDefault(s => s.id == t.TreeId);
                if (slot.id == null || Vector2.Distance(slot.pos, new Vector2(t.transform.position.x, t.transform.position.z)) > tolerance) mismatches++;
            }
            if (mismatches > 0) b3SlotModelFailures++;
            var absent = b3Slots.Select(s => s.id).Where(id => !livingCore.Contains(id)).OrderBy(x => x, StringComparer.Ordinal).ToList();
            var byClearance = absent.Where(id => b3ClearanceOmittedCore.Contains(id)).ToList();
            var byHashBudget = absent.Where(id => !b3ClearanceOmittedCore.Contains(id)).ToList();
            var clearanceButLiving = b3ClearanceOmittedCore.Where(id => livingCore.Contains(id)).OrderBy(x => x, StringComparer.Ordinal).ToList();
            B3("SLOT_MODEL_CHECK", "reset=" + reset + " model=" + model + " positionMismatches=" + mismatches + " slotModelValid=" + (mismatches == 0));
            B3("OMITTED_CORE", "reset=" + reset + " model=" + model + " absentCoreSlots=" + absent.Count + " absentSha256=" + B3Sha256Text(string.Join(",", absent))
                + " clearanceOmitted=" + byClearance.Count + ":" + string.Join(",", byClearance)
                + " hashBudgetOmitted=" + byHashBudget.Count + ":" + string.Join(",", byHashBudget)
                + " clearanceZoneButStillLiving=" + clearanceButLiving.Count + ":" + string.Join(",", clearanceButLiving));
        }
        catch (Exception error) { B3Missing("tree identity / omitted core slots", error); }
    }

    // Item 7: the frozen Reference Future v1 (permanently Legacy40) and its Legacy40 anchors. The tracked tree holds no separate
    // Legacy40 fixture files; the Legacy40 anchors are the archive's own startingStandHash and per-milestone worldHash strings.
    private string B3ArchiveState(string phase)
    {
        try
        {
            byte[] disk = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Application.dataPath, "ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes"));
            TextAsset asset = Resources.Load<TextAsset>(ScenarioReferenceArchive.ResourceName);
            ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
            string resourceSha = asset == null ? "UNAVAILABLE" : B3Sha256(asset.bytes);
            string anchors = archive == null ? "UNAVAILABLE"
                : "startingStand=" + archive.startingStandHash + ";" + string.Join(";", archive.milestones.OrderBy(x => x.year).Select(x => x.year + "=" + x.worldHash));
            string state = "diskSha256=" + B3Sha256(disk) + " diskGitBlob=" + B3GitBlobId(disk) + " resourceBytesSha256=" + resourceSha
                + " legacy40AnchorsSha256=" + (archive == null ? "UNAVAILABLE" : B3Sha256Text(anchors)) + " milestones=" + (archive == null ? -1 : archive.milestones.Count);
            B3("ARCHIVE", "phase=" + phase + " " + state);
            if (archive != null) B3("LEGACY40_ANCHORS", "phase=" + phase + " " + anchors);
            return state;
        }
        catch (Exception error) { B3Missing("archive state (" + phase + ")", error); return "UNAVAILABLE"; }
    }

    private void B3Start()
    {
        string requested = B3Env("CCF_B3_MODE");
        if (requested != "UNAVAILABLE" && requested != "plain" && requested != "authored-pin")
            throw new InvalidOperationException("CCF_B3_MODE must be plain or authored-pin, not '" + requested + "'");
        b3Mode = requested == "authored-pin" ? "authored-pin" : "plain";
        b3Label = B3Env("CCF_B3_LABEL");
        b3Started = true;
        ForestPlayer p = player();
        if (p != null) b3AuthoredStart = p.transform.position;
        B3("START", "authoredStart=" + (p == null ? "UNAVAILABLE" : B3V(b3AuthoredStart)));
        B3Pose("harness-start");
        B3Identity("pre");
        b3ArchiveBefore = B3ArchiveState("before");
    }

    // Called once at the end, whether or not the harness failed. Returns the failure to report: an archive change is a failure.
    private Exception B3Finish(Exception failure)
    {
        if (!b3Started) return failure;
        try
        {
            B3Pose("harness-end");
            B3Identity("post");
            string after = B3ArchiveState("after");
            bool archiveSame = b3ArchiveBefore != "UNAVAILABLE" && after != "UNAVAILABLE" && b3ArchiveBefore == after;
            // Complete means every observation was captured through the end of the first Reference preview, whatever the harness's own checks did.
            bool complete = b3Resets > 0 && b3SawPreviewEnd && b3Unavailable == 0 && b3SlotModelFailures == 0 && archiveSame;
            B3("SUMMARY", "resets=" + b3Resets + " poseRecords=" + b3PoseRecords + " unavailable=" + b3Unavailable + " slotModelFailures=" + b3SlotModelFailures
                + " authoredPinApplied=" + b3PinApplied + " archiveUnchanged=" + archiveSame + " harnessFailed=" + (failure != null)
                + " witness=" + (complete ? "EVIDENCE_COMPLETE" : "EVIDENCE_INCOMPLETE") + " b3Verdict=NOT_DECIDED_BY_HARNESS");
            if (b3ArchiveBefore != "UNAVAILABLE" && after != "UNAVAILABLE" && b3ArchiveBefore != after && failure == null)
                return new InvalidOperationException("Reference Future v1 / Legacy40 anchors changed during the run");
        }
        catch (Exception error) { Debug.Log(B3Tag + "kind=FINISH_ERROR reason=" + B3Clean(error.Message)); }
        return failure;
    }

    private ForestPlayer player() => FindFirstObjectByType<ForestPlayer>();
}

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
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(tree.gameObject);
        e.ApplyStandGeometry(model);
        e.ResetForDeterministicRun();
        var clock = System.Diagnostics.Stopwatch.StartNew();
        stand.Generate();
        double generateMs = clock.Elapsed.TotalMilliseconds;
        m.InitializeNewScenario();
        double initializeMs = clock.Elapsed.TotalMilliseconds - generateMs;
        e.InvalidateCompetition();
        // Editor timing only; the first call of a session also pays JIT cost, so read later calls for steady state.
        Debug.Log($"ENLARGED80_STARTUP_MS model={model} generate={generateMs:F1} initializeScenario={initializeMs:F1} trees={Living().Count}");
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
        Check(legacy.All(t => enlargedSnapshot.ContainsKey(t.TreeId)), "every legacy tree ID exists in the enlarged stand (IDs unchanged)");
        Check(legacy.All(t => SamePosition(t.transform.position, enlargedSnapshot[t.TreeId].pos)), "every legacy tree stands at its exact legacy position in the enlarged stand");
        Check(core.Select(t => t.TreeId).ToHashSet().SetEquals(legacy.Select(t => t.TreeId)), "the enlarged core's omission pattern is exactly the legacy pattern (same 336 IDs)");
        // Starting state: interior core trees (lattice 3..17) have identical neighbourhoods, so identical DBH; the artificial edge ring differs.
        int identicalDbh = 0, changed = 0; double dbhShift = 0;
        var ringChanged = new List<float>();
        foreach (string id in legacySnapshot.Keys)
        {
            int r = int.Parse(id.Substring(1, 2)), c = int.Parse(id.Substring(3, 2));
            bool deep = r >= 3 && r <= 17 && c >= 3 && c <= 17;
            float delta = enlargedSnapshot[id].dbh - legacySnapshot[id].dbh;
            if (deep) { Check(Mathf.Approximately(delta, 0f), "interior core tree " + id + " has an identical starting DBH"); identicalDbh++; }
            else if (!Mathf.Approximately(delta, 0f)) { changed++; dbhShift += delta; ringChanged.Add(delta); }
        }
        Check(ringChanged.Count == 0 || ringChanged.All(d => d <= 0f), "former edge-ring trees only lose the artificial 'released' bonus (never gain)");
        Pass("core preserved: 336/336 IDs and exact positions; " + identicalDbh + " interior trees keep their DBH; " + changed + " former edge-ring trees change DBH by mean " + F(changed == 0 ? 0 : dbhShift / changed) + " cm (obsolete edge biology removed)");
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
        Check(saved.version == 20 && saved.standGeometryModel == StandGeometryModel.Enlarged80, "an Enlarged80 capture is v20 + geometry 1");
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
        Pass("v20 Enlarged80 save: validates, loads over a Legacy40 world, restores 80 m geometry, 256 cells, 1,344 trees and the exact world");

        // A v19 Legacy40 save loaded into the Enlarged80 world: Legacy40 geometry, 336 trees, cells at their original positions.
        ResetWorld(StandGeometryModel.Legacy40);
        int marked = CellAt(2.5f, -17.5f);   // Legacy40 cell index 4
        e.RestoreCellState(marked, new List<ForestRegenerationCohortSaveData>(), 1.5f, 1f);
        ForestSaveData legacySave = saves.CaptureData();
        Check(legacySave.standGeometryModel == 0 && marked == 4, "a Legacy40 capture states geometry 0 (marked cell index 4 = world (2.5, -17.5))");
        string withoutField = JsonUtility.ToJson(legacySave).Replace("\"standGeometryModel\":0,", "");
        string v19Json = "{\"version\":19" + withoutField.Substring(withoutField.IndexOf(','));
        ForestSaveData v19 = JsonUtility.FromJson<ForestSaveData>(v19Json);
        Check(v19.version == 19 && StandGeometryModel.ForSave(v19) == 0, "the v19-layout save is read as Legacy40");
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(e.CellCount == 256 && Living().Count == 1344, "scratch: the live world is a fresh Enlarged80 world");
        Check(saves.LoadData(v19, false), "a v19 Legacy40 save loads into an Enlarged80 world");
        yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64 && Mathf.Approximately(e.StandBounds.width, 40f), "it applied Legacy40 geometry (64 cells, 40 m)");
        Check(Living().Count == 336 && Living().All(t => legacyId.IsMatch(t.TreeId)), "no outer forest: exactly the 336 saved trees");
        Check(Mathf.Approximately(e.Cells[4].RecentOpening, 1.5f) && Vector2.Distance(e.Cells[4].Center, new Vector2(2.5f, -17.5f)) < 0.01f, "the cell record is back at its original physical position");
        Check(Mathf.Approximately(GameObject.Find("Ground").transform.localScale.x, 40f) && Mathf.Approximately(GameObject.Find("East Ridge").transform.position.x, 20f), "ground and ridges returned to the authored 40 m property");
        Check(!GameObject.Find("Ground").GetComponent<Renderer>().HasPropertyBlock(), "ground texture override removed for Legacy40");
        Check(saves.CaptureData().standGeometryModel == 0, "re-saving writes v20 + geometry 0");
        Pass("v19 Legacy40 save into an Enlarged80 world: Legacy40 grid and ground restored, 336 trees, cell 4 at (2.5, -17.5)");

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
        Check(m.TryBeginReferencePreview(100), "Reference preview opens from the 80 m world");
        yield return null; yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64 && Vector2.Distance(e.Cells[archivedCell].Center, archivedCentre) < 0.01f, "preview applied Legacy40: archived ecology at its original positions");
        Check(Living().Count == y100.world.trees.Count(t => t.stage != (int)ForestTreeStage.Stump && !t.biologicallyDead), "preview shows exactly the archive's living trees (no outer forest)");
        Check(Mathf.Approximately(GameObject.Find("Ground").transform.localScale.x, 40f), "preview shows the 40 m ground");
        m.EndReferencePreview();
        yield return null; yield return null;
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

    private ForestPlayer player() => FindFirstObjectByType<ForestPlayer>();
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable 80A gate (D-056): stand-geometry versioning with ZERO change to Legacy40 behaviour.
// Copy into Assets/ForestPrototype, run StandGeometryVerification.Begin (batch or interactive), then remove the
// copy and its .meta (run_stand_geometry.py does this). It never writes the player's save file, never saves the
// scene, and only switches the ecology GRID to Enlarged80 (no 80 m world content exists in 80A) to exercise the
// geometry-switch boundary that Reference preview relies on.
public static class StandGeometryVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "StandGeometryVerification.Begin"))
            new GameObject("Disposable stand geometry verification").AddComponent<StandGeometryVerificationRunner>();
    }
}

public sealed class StandGeometryVerificationRunner : MonoBehaviour
{
    private ForestEcologyController e;
    private ScenarioOneManager m;
    private ForestSaveController saves;
    private ScenarioOneUiRoot ui;
    private int geometryEvents;
    private readonly List<string> log = new List<string>();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private void Pass(string message) { log.Add(message); Debug.Log("STAND_GEOMETRY_CHECK " + message); }

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
        Debug.Log(failure == null ? "STAND_GEOMETRY_VERIFY_PASS checks=" + log.Count : "STAND_GEOMETRY_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    private int LivingTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Count(t => t != null && t.IsLiving);

    // The pre-v20 byte layout of a Legacy40 world: no standGeometryModel field and a v19 header.
    private static string V19Layout(ForestSaveData data)
    {
        string json = JsonUtility.ToJson(data).Replace("\"standGeometryModel\":0,", "");
        return "{\"version\":19" + json.Substring(json.IndexOf(','));
    }

    private (string hash, int cells, int trees, int model) Snapshot() => (WorldHash(), e.CellCount, LivingTrees(), e.StandGeometryModelVersion);

    private IEnumerator Run()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        m = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(e != null && m != null && saves != null, "not the Scenario One ForestTest scene");
        e.StandGeometryApplied += _ => geometryEvents++;

        // ---- 1. Model identities, policy and the explicit Legacy40 default -------------------------------
        Check(StandGeometryModel.Legacy40 == 0 && StandGeometryModel.Enlarged80 == 1, "model identities");
        Check(StandGeometryModel.CellCount(StandGeometryModel.Legacy40) == 64 && StandGeometryModel.CellCount(StandGeometryModel.Enlarged80) == 256, "model cell counts 64/256");
        Check(StandGeometryModel.StandSizeMeters(0) == 40f && StandGeometryModel.StandSizeMeters(1) == 80f && StandGeometryModel.CellSizeMeters == 5f, "model sizes");
        Check(!StandGeometryModel.IsKnown(-1) && !StandGeometryModel.IsKnown(2) && !StandGeometryModel.IsKnown(7), "unknown models are not known");
        // 80B: the production policy is Enlarged80 (Enlarged80Verification asserts that without any override). This
        // foundation gate checks Legacy40 behaviour, so its runner sets the Editor-only Legacy40 override.
        Check(StandGeometryPolicy.NewGameModel == StandGeometryModel.Enlarged80, "80B: the production new-game policy is Enlarged80");
        Check(StandGeometryPolicy.NewGameModelForSession == StandGeometryModel.Legacy40, "this gate runs under the Editor-only Legacy40 verification override (CCF_STAND_GEOMETRY=0)");
        Check(e.StandGeometryModelVersion == StandGeometryModel.Legacy40 && e.CellCount == 64 && e.CellsPerAxis == 8, "fresh world is explicitly Legacy40 on an 8 x 8 grid");
        Check(Mathf.Approximately(e.StandBounds.width, 40f) && Mathf.Approximately(e.StandAreaHectares, 0.16f), "Legacy40 bounds 40 x 40 m, 0.16 ha");
        Check(LivingTrees() == 336, "the 336-tree Legacy40 starting world is unchanged");
        Pass("model identities, 64/256 cells, new games Legacy40, 336-tree start unchanged");

        // ---- 2. Capture records the active geometry; v20 round trip ---------------------------------------
        ForestSaveData start = saves.CaptureData();
        Check(start.version == 20 && ForestSaveData.CurrentVersion == 20 && start.standGeometryModel == 0, "capture writes v20 + geometry 0");
        string startHash = ScenarioReferenceArchive.WorldHash(start);
        Check(startHash == ScenarioReferenceArchive.Hash(V19Layout(start)), "a Legacy40 world hashes exactly as the pre-v20 layout (historical anchors stay valid)");
        string v20Json = JsonUtility.ToJson(start);
        Check(v20Json.Contains("\"standGeometryModel\":0"), "the v20 JSON states its geometry explicitly");

        // Two explicit identities (D-056): the CURRENT v20 hash is geometry-aware and never strips the field; the
        // LEGACY-COMPATIBLE hash is the historical layout for Legacy40 only. An Enlarged80 world must never hash
        // like an otherwise equivalent Legacy40 world just because compatibility hashing drops the new field.
        string currentHash = ScenarioReferenceArchive.CurrentWorldHash(start);
        Check(v20Json.Contains("\"version\":20") && currentHash == ScenarioReferenceArchive.Hash(v20Json) && currentHash != startHash,
            "CurrentWorldHash hashes the full v20 JSON, geometry field included (it is not the legacy identity)");
        Check(ScenarioReferenceArchive.LegacyCompatibleWorldHash(start) == startHash, "LegacyCompatibleWorldHash is the historical layout for a Legacy40 world");
        ForestSaveData asEnlarged = JsonUtility.FromJson<ForestSaveData>(v20Json);
        asEnlarged.standGeometryModel = StandGeometryModel.Enlarged80;
        string enlargedCurrent = ScenarioReferenceArchive.CurrentWorldHash(asEnlarged);
        Check(enlargedCurrent != currentHash && ScenarioReferenceArchive.WorldHash(asEnlarged) == enlargedCurrent && enlargedCurrent != startHash,
            "an Enlarged80 world never hashes like the otherwise identical Legacy40 world (the geometry field is not stripped)");
        Check(ScenarioReferenceArchive.LegacyCompatibleWorldHash(asEnlarged) == null && ScenarioReferenceArchive.LegacyV18WorldHash(asEnlarged) == null
            && ScenarioReferenceArchive.LegacyV15WorldHash(asEnlarged) == null, "no historical layout exists for a non-Legacy40 world");
        Pass("hash identities explicit: current v20 hash keeps the geometry field; Legacy40 compat stays historical; Enlarged80 can never collide with it");
        ForestSaveData parsed = JsonUtility.FromJson<ForestSaveData>(v20Json);
        Check(ForestSaveValidation.ValidateGeometryJson(v20Json, parsed) == null && ForestSaveValidation.Validate(parsed, 336, 64) == null, "a v20 Legacy40 save validates");
        Check(saves.LoadData(parsed, false), "v20 geometry-0 save loads");
        yield return null;
        Check(WorldHash() == startHash && e.CellCount == 64 && e.StandGeometryModelVersion == 0, "v20 geometry-0 round trip is exact");
        Pass("v20 round trip exact; Legacy40 world hash == pre-v20 layout hash " + startHash);

        // ---- 3. A v19 save loads as Legacy40 and is written back as v20 + geometry 0 -----------------------
        string v19Json = V19Layout(start);
        Check(!v19Json.Contains("standGeometryModel"), "the v19 layout has no geometry field");
        ForestSaveData v19 = JsonUtility.FromJson<ForestSaveData>(v19Json);
        Check(v19.version == 19 && v19.standGeometryModel == 0, "v19 parses with the default geometry 0");
        Check(StandGeometryModel.ForSave(v19) == StandGeometryModel.Legacy40, "a save before v20 is read as Legacy40");
        Check(ForestSaveValidation.ValidateGeometryJson(v19Json, v19) == null && ForestSaveValidation.Validate(v19, 336, 64) == null, "v19 validates");
        Check(saves.LoadData(v19, false), "v19 save loads");
        yield return null;
        ForestSaveData rewritten = saves.CaptureData();
        Check(rewritten.version == 20 && rewritten.standGeometryModel == 0, "v19 -> saved again writes v20 + geometry 0");
        Check(ScenarioReferenceArchive.WorldHash(rewritten) == startHash, "v19 loaded then re-saved is the same world (hash unchanged)");
        Pass("v19 loads as Legacy40; re-save writes v20 + geometry 0 with an identical world hash");

        // ---- 4. Malformed / unknown geometry is rejected BEFORE any world mutation --------------------------
        var before = Snapshot();
        foreach (int bad in new[] { 2, 7, -1, 1000 })
        {
            ForestSaveData forged = JsonUtility.FromJson<ForestSaveData>(v20Json);
            forged.standGeometryModel = bad;
            Check(ForestSaveValidation.Validate(forged, 336, 64) != null, "unknown geometry " + bad + " fails validation");
            Check(!saves.LoadData(forged, false), "unknown geometry " + bad + " is refused by LoadData");
            Check(Snapshot().Equals(before), "refused unknown geometry " + bad + " left the world untouched");
        }
        ForestSaveData olderWithModel = JsonUtility.FromJson<ForestSaveData>(v19Json);
        olderWithModel.standGeometryModel = 1;
        Check(ForestSaveValidation.Validate(olderWithModel, 336, 64) != null && !saves.LoadData(olderWithModel, false) && Snapshot().Equals(before),
            "a save older than v20 cannot claim a geometry model");
        JObject missing = JObject.Parse(v20Json); missing.Remove("standGeometryModel");
        Check(ForestSaveValidation.ValidateGeometryJson(missing.ToString(), parsed) != null, "a v20 save with the field missing is rejected on the disk path");
        JObject asText = JObject.Parse(v20Json); asText["standGeometryModel"] = "1";
        Check(ForestSaveValidation.ValidateGeometryJson(asText.ToString(), parsed) != null, "a non-integer geometry is rejected on the disk path");
        JObject mismatched = JObject.Parse(v20Json); mismatched["standGeometryModel"] = 1;
        Check(ForestSaveValidation.ValidateGeometryJson(mismatched.ToString(), parsed) != null, "raw JSON disagreeing with the parsed value is rejected");
        Pass("unknown/forged/missing/non-integer geometry rejected before world mutation (world unchanged)");

        // ---- 5. Cell-indexed data is validated against the SAVE's geometry, not the live grid ----------------
        ForestSaveData wideCells = JsonUtility.FromJson<ForestSaveData>(v20Json);
        wideCells.cells.Add(new ForestCellSaveData { index = 200, recentOpening = 1f, establishmentSuitability = 1f });
        Check(ForestSaveValidation.Validate(wideCells, 336, StandGeometryModel.CellCount(0)) != null, "cell 200 is outside Legacy40's 64 cells");
        wideCells.standGeometryModel = 1;
        Check(ForestSaveValidation.Validate(wideCells, 336, StandGeometryModel.CellCount(1)) != null, "an Enlarged80 Model-2 save needs 256 understorey cells");
        Pass("cell index and Model-2 completeness are checked against the geometry's own cell count (64 vs 256)");

        // ---- 6. Load order: geometry is applied before cell-indexed state is restored ------------------------
        ForestSaveData enlarged = JsonUtility.FromJson<ForestSaveData>(v20Json);
        enlarged.standGeometryModel = 1;
        ScenarioUnderstoreyCell seed = enlarged.scenarioOne.understoreyCells[0];
        for (int i = 64; i < 256; i++)
            enlarged.scenarioOne.understoreyCells.Add(new ScenarioUnderstoreyCell { cellIndex = i, lastUpdatedYear = seed.lastUpdatedYear });
        enlarged.cells.Add(new ForestCellSaveData { index = 200, recentOpening = 1.5f, establishmentSuitability = 1f });
        Check(ForestSaveValidation.Validate(enlarged, 336, StandGeometryModel.CellCount(1)) == null, "an Enlarged80 save validates against 256 cells");
        int eventsBefore = geometryEvents;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64, "the live world is still Legacy40 (64 cells) when the Enlarged80 save arrives");
        Check(saves.LoadData(enlarged, false), "an Enlarged80 save loads into a Legacy40 live world (validated against ITS grid)");
        yield return null;
        Check(e.StandGeometryModelVersion == 1 && e.CellCount == 256 && e.CellsPerAxis == 16 && Mathf.Approximately(e.StandBounds.width, 80f), "geometry was applied from the save (256 cells, 80 m bounds)");
        Check(Mathf.Approximately(e.Cells[200].RecentOpening, 1.5f), "the cell-200 record was restored onto the 256-cell grid");
        Check(geometryEvents == eventsBefore + 1, "exactly one geometry event was raised");
        Check(saves.CaptureData().standGeometryModel == 1, "a capture now records geometry 1");
        Check(m.UnderstoreyCells.Count == 256, "understorey state follows the 256-cell grid");
        // and back: a Legacy40 save puts the 64-cell grid back (never validated against the 256-cell live grid)
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(v20Json), false), "a Legacy40 save loads into an Enlarged80 live world");
        yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64 && WorldHash() == startHash, "Legacy40 restored exactly (64 cells, identical world hash)");
        Pass("load order verified: model decided and validated first, grid applied before restore, both directions");

        // ---- 7. Reference archive requires Legacy40 -------------------------------------------------------------
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(archive != null && archive.AtYear(100)?.verifiedFrozenWorld == true, "Reference Future v1 archive loads and verifies");
        Check(ScenarioReferenceArchive.RequiredStandGeometryModel == StandGeometryModel.Legacy40, "Reference Future v1 is Legacy40");
        Check(archive.Matches(m.Definition, e) && archive.IdentityMatches(m.Definition, e), "archive matches in a Legacy40 world");
        e.ApplyStandGeometry(1);
        Check(!archive.Matches(m.Definition, e) && archive.IdentityMatches(m.Definition, e), "archive does NOT match a wrong (Enlarged80) live geometry");
        e.ApplyStandGeometry(0);
        yield return null;
        Check(WorldHash() == startHash, "grid-only geometry round trip leaves the world unchanged");
        Pass("Reference archive: Matches requires Legacy40; identity check is separate");

        // ---- 8. Reference preview boundary, from Legacy40 and from Enlarged80 -------------------------------
        ScenarioReferenceMilestone y100 = archive.AtYear(100);
        int archivedCell = y100.world.cells.OrderByDescending(c => c.cohorts?.Sum(h => h.density) ?? 0f).First().index;
        Vector2 archivedCentre = new Vector2(-17.5f + 5f * (archivedCell % 8), -17.5f + 5f * (archivedCell / 8));

        float[] lightBefore = e.Cells.Select(c => c.Light).ToArray();
        Check(m.TryBeginReferencePreview(100), "preview opens from Legacy40");
        yield return null; yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64 && Vector2.Distance(e.Cells[archivedCell].Center, archivedCentre) < 0.01f, "preview shows the archive on the Legacy40 grid at its original positions");
        m.EndReferencePreview();
        yield return null; yield return null;
        Check(WorldHash() == startHash && e.CellCount == 64, "leaving the preview restores the exact Legacy40 world");
        Check(Enumerable.Range(0, 64).All(i => Mathf.Abs(e.Cells[i].Light - lightBefore[i]) < 1e-5f), "derived canopy/light state is rebuilt identically");
        Pass("preview from Legacy40: archive on its own grid; exact world restored");

        // From an Enlarged80 grid: the player's world is an Enlarged80 capture; preview must switch to Legacy40 and back.
        Check(saves.LoadData(enlarged, false), "Enlarged80 world loaded for the preview test");
        yield return null;
        string enlargedHash = WorldHash();
        Check(e.StandGeometryModelVersion == 1 && e.CellCount == 256, "player world is Enlarged80 (256 cells)");
        if (ui != null && ui.Map != null)
        {
            Check(ui.Map.SetWaypointCell(e, 200), "a waypoint is set in the Enlarged80 grid");
        }
        float[] enlargedLight = e.Cells.Select(c => c.Light).ToArray();
        int eventsBeforePreview = geometryEvents;
        Check(m.TryBeginReferencePreview(100), "preview opens from Enlarged80");
        yield return null; yield return null;
        Check(e.StandGeometryModelVersion == 0 && e.CellCount == 64, "entering the preview applied Legacy40 (64 cells)");
        Check(Vector2.Distance(e.Cells[archivedCell].Center, archivedCentre) < 0.01f, "archived ecology sits at its original Legacy40 position (not displaced onto the 256-cell grid)");
        if (ui != null && ui.Map != null)
            Check(!ui.Map.HasWaypoint, "the Enlarged80 waypoint is not reinterpreted on the 64-cell grid");
        m.EndReferencePreview();
        yield return null; yield return null;
        Check(e.StandGeometryModelVersion == 1 && e.CellCount == 256 && e.CellsPerAxis == 16, "leaving the preview restored Enlarged80 geometry");
        Check(WorldHash() == enlargedHash, "leaving the preview restored the player's exact Enlarged80 world state");
        Check(Enumerable.Range(0, 256).All(i => Mathf.Abs(e.Cells[i].Light - enlargedLight[i]) < 1e-5f), "derived canopy/light state is rebuilt identically on the 256-cell grid");
        if (ui != null && ui.Map != null)
            Check(ui.Map.HasWaypoint, "the waypoint set in Enlarged80 is restored when Enlarged80 returns");
        Check(geometryEvents == eventsBeforePreview + 2, "two geometry events: into and out of the preview");
        Pass("preview from Enlarged80: switched to Legacy40 and back; exact world, light and waypoint restored");

        // leave the scratch session in the Legacy40 start world
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(v20Json), false), "scratch world returned to Legacy40");
        yield return null;
        Check(WorldHash() == startHash, "final world equals the starting world");
        Debug.Log("STAND_GEOMETRY_START_WORLD_HASH " + startHash);
        yield return null;
    }
}

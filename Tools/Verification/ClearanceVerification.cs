using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable acceptance fixture. Never saves a scene; restores isolated save.
public static class ClearanceVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(1600, 900); EditorApplication.isPlaying = true;
    }
    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Clearance review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "ClearanceVerification.Begin"))
            new GameObject("Disposable clearance verification").AddComponent<ClearanceVerificationRunner>();
    }
}
public sealed class ClearanceVerificationRunner : MonoBehaviour
{
    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestPlayer player;
    private string output, savePath;
    private readonly System.Collections.Generic.Dictionary<string, int?> helpPreferences = new System.Collections.Generic.Dictionary<string, int?>();
    private byte[] priorSave;
    private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private static object Call(object o, string name, params object[] args) => o.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(o, args);
    private static void Field(object o, string name, object value) => o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(o, value);
    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null; var steps = new System.Collections.Generic.Stack<IEnumerator>(); steps.Push(Verify());
        while (steps.Count > 0)
        {
            bool more; object current = null;
            try { more = steps.Peek().MoveNext(); if (more) current = steps.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { steps.Pop(); continue; }
            if (current is IEnumerator child) { steps.Push(child); continue; }
            yield return current;
        }
        if (savePath != null) { if (priorSave != null) File.WriteAllBytes(savePath, priorSave); else File.Delete(savePath); }
        foreach (var entry in helpPreferences)
            if (entry.Value.HasValue) PlayerPrefs.SetInt(entry.Key, entry.Value.Value); else PlayerPrefs.DeleteKey(entry.Key);
        if (helpPreferences.Count > 0) PlayerPrefs.Save();
        if (failure == null) Debug.Log("CLEARANCE_ACCEPTANCE_PASS"); else Debug.LogError("CLEARANCE_VERIFY_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }
    private IEnumerator Verify()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>(); manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>(); player = FindFirstObjectByType<ForestPlayer>();
        // Run with the rendered launcher (run_clearance_gate.py --interactive).
        Check(!Application.isBatchMode, "ClearanceVerification requires an interactive (non -batchmode) Editor: batch mode cannot lock the cursor, so walking aim, tree inspection and keyboard input to play mode are unavailable");
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT"); Directory.CreateDirectory(output);
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json"); if (File.Exists(savePath)) priorSave = File.ReadAllBytes(savePath);
        ForestSaveData original = saves.CaptureData();
        // Since the menu-teaching follow-up (466456f) a fresh profile opens the
        // first-use HUD introduction, which pauses forest input (AnyPanelOpen)
        // and therefore, by design, suppresses the walking clearance preview.
        // Dismiss it as a player would; restore device preferences on exit.
        ScenarioOneUiRoot teaching = manager.GetComponent<ScenarioOneUiRoot>();
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
        {
            string key = MenuHelpView.PreferencePrefix + menu;
            helpPreferences[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
        }
        if (teaching != null && teaching.Help != null && teaching.Help.IsOpen) teaching.CloseHelp();
        yield return null; yield return null;
        Check(!manager.AnyPanelOpen, "a UI panel is still open before the walking-ray fixture");
        ForestTree retained = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.IsLiving && Mathf.Abs(ecology.Cells[ecology.GetCellIndex(t.transform.position)].Center.x) < 15f && Mathf.Abs(ecology.Cells[ecology.GetCellIndex(t.transform.position)].Center.y) < 15f)
            .OrderBy(t => t.transform.position.sqrMagnitude).ThenBy(t => t.TreeId, StringComparer.Ordinal).First();
        retained.SetMark(TreeMarkType.CropTree); string retainedId = retained.TreeId;
        int index = ecology.GetCellIndex(retained.transform.position), outside = index % ecology.CellsPerAxis < ecology.CellsPerAxis - 1 ? index + 1 : index - 1;
        ClearanceFootprint footprint = ClearanceFootprint.Cell(ecology, index);
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        foreach (string id in new[] { "sitka-spruce", "sessile-oak", "beech" })
        {
            ecology.Cells[index].GetOrCreateCohort(spawner.ResolveSpecies(id)).Restore(.4f, .3f, 0);
            ecology.Cells[outside].GetOrCreateCohort(spawner.ResolveSpecies(id)).Restore(.4f, .3f, 0);
        }
        ecology.Cells[index].Light = .9f;
        ScenarioUnderstoreyCell understorey = manager.UnderstoreyCells[index];
        understorey.ferns = understorey.grasses = understorey.forbs = understorey.shrubs = .8f;
        ScenarioOneSaveData state = manager.CaptureSaveData();
        Vector3 justOutside = footprint.Center + new Vector3(footprint.HalfCell + .01f, 0f, 0f);
        state.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = "PJ-FIXTURE-IN", speciesId = "beech", position = footprint.Center,
            cellIndex = index, ageYears = 3f, heightMeters = .6f, alive = true });
        state.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = "PJ-FIXTURE-OUT", speciesId = "sessile-oak", position = justOutside,
            cellIndex = ecology.GetCellIndex(justOutside), ageYears = 3f, heightMeters = .6f, alive = true });
        manager.RestoreSaveData(state, ForestSaveData.CurrentVersion);
        ecology.RefreshRegenerationDisplays(); yield return null;
        ClearanceTargets targets = manager.QueryClearance(footprint);
        Check(targets.Cohorts.Count == 3 && targets.Juveniles.Count == 1 && targets.GroundPlants.Count > 0, "A/B/C/G target query");
        Check(!footprint.Contains(justOutside) && targets.Cohorts.All(t => t.CellIndex == index), "D footprint exclusion");
        Check(retained.IsLiving && retained.IsCropTree, "E retained crop tree");
        // Controlled eye pose for reproducible evidence; input is disabled only in this fixture.
        player.enabled = false; player.Clearance.enabled = false;
        Camera camera = Camera.main;
        camera.transform.position = footprint.Center + new Vector3(0f, 1.65f, -3.5f);
        camera.transform.LookAt(footprint.Center);
        Field(player, "isAimingGround", true); Field(player, "aimedSurfacePoint", footprint.Center);
        Cursor.lockState = CursorLockMode.Locked;
        bool aimed = false;
        for (int direction = 0; direction < 8; direction++)
        {
            float angle = direction * Mathf.PI / 4f;
            camera.transform.position = footprint.Center + new Vector3(Mathf.Cos(angle) * 1.6f, 1.65f, Mathf.Sin(angle) * 1.6f);
            camera.transform.LookAt(footprint.Center);
            Call(player, "UpdateTreeInspection", false, false, false, false, false, false, false);
            if (player.IsAimingGround && player.Clearance.Targets != null && player.Clearance.Targets.Footprint.CellIndex == index)
            { aimed = true; break; }
        }
        Check(aimed && player.Clearance.Targets.Cohorts.Count == targets.Cohorts.Count
            && player.Clearance.Targets.Juveniles.Count == targets.Juveniles.Count, "walking ray preview did not use authoritative cell query");
        Debug.Log("CLEARANCE_WALKING_PREVIEW_PASS groundRay=true sharedQuery=true");
        player.Clearance.Clear();
        camera.transform.position = footprint.Center + new Vector3(0f, 1.65f, -3.5f); camera.transform.LookAt(footprint.Center);
        Field(player, "isAimingGround", true); Field(player, "aimedSurfacePoint", footprint.Center); Field(player, "isLookingAtTree", false);
        ForestTreeMarkingManager marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        marking.enabled = false; Field(marking, "aimedTree", null); Field(marking, "aimingAtGround", true); Field(marking, "aimedGroundPoint", footprint.Center);
        yield return Capture("before");
        string before = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        player.Clearance.Show(targets);
        Check(player.Clearance.Visible && player.Clearance.Targets.Footprint.Center == footprint.Center
            && player.Clearance.Targets.Footprint.HalfCell == footprint.HalfCell, "F preview geometry");
        yield return Capture("preview");
#if UNITY_EDITOR
        if (Environment.GetEnvironmentVariable("CCF_CLEARANCE_CAPTURE") == "1")
        {
            foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
            { ClearanceVerification.SetGameSize(size.x, size.y); yield return Capture("preview-" + size.x); }
            ClearanceVerification.SetGameSize(1600, 900);
        }
#endif
        player.Clearance.Clear();
        Check(!player.Clearance.Visible && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == before, "H cancel mutated state");
        Check(manager.TryDesignateVegetationClearance(index), "designation failed: " + manager.Feedback);
        ScenarioOneUiRoot ui = manager.GetComponent<ScenarioOneUiRoot>();
        player.enabled = true;
        player.Clearance.Show(targets); ui.ShowWorkPlan(); Call(player.Clearance, "LateUpdate");
        Check(manager.AnyPanelOpen && !player.Clearance.Visible, "Work Plan left preview visible");
        yield return Capture("work-plan");
        ui.ShowMap(); player.Clearance.Show(targets); Call(player.Clearance, "LateUpdate");
        Check(manager.AnyPanelOpen && !player.Clearance.Visible, "Map left preview visible");
        yield return Capture("map");
        Vector3 playerPosition = player.transform.position;
        Call(ui.Map, "SetWaypoint", ecology, outside);
        Check(player.transform.position == playerPosition && !string.IsNullOrEmpty(ui.Map.WaypointDescription(playerPosition)), "waypoint moved player / missing label");
        ui.CloseAll(); Cursor.lockState = CursorLockMode.Locked;
        Call(player, "InspectTree", retained); player.Clearance.Show(targets); Call(player.Clearance, "LateUpdate");
        Check(player.IsInspecting && !player.Clearance.Visible, "inspection left preview visible");
        player.enabled = false;
        yield return Capture("inspection");
        Field(player, "isInspecting", false); Field(player, "inspectedTree", null);
        player.enabled = true;
        player.Clearance.Show(targets); ui.ShowReview(); Call(player.Clearance, "LateUpdate");
        Check(!player.Clearance.Visible, "review left preview visible");
        ui.Map.DestroyWaypoint();
        ui.CloseAll(); player.enabled = false;
        camera.transform.position = footprint.Center + new Vector3(0f, 1.65f, -3.5f); camera.transform.LookAt(footprint.Center);
        Check(!manager.TryDesignateVegetationClearance(index), "J duplicate pending job");
        ScenarioOneWorkOrder order = manager.WorkOrders.Last();
        long cancelCash = manager.CashCents;
        Check(manager.RemovePendingOrder(order.workOrderId) && manager.CashCents == cancelCash
            && targets.Cohorts.All(c => Mathf.Approximately(c.Cohort.Density, .4f)), "cancelled work changed cash/vegetation");
        Check(manager.TryDesignateVegetationClearance(index), "replan after cancel failed");
        order = manager.WorkOrders.Last();
        Check(string.IsNullOrEmpty(order.speciesId), "species defines clearance scope");
        Check(manager.ApprovePendingWork(), "approve clearance: " + manager.Feedback);
        long cash = manager.CashCents;
        var report = new ScenarioAnnualReport { year = ecology.EcologicalYear };
        // Resolve the same production work effect without annual recolonisation,
        // so the exact target list can be checked independently of new seed rain.
        Call(manager, "ResolveOrder", order, report);
        Check(order.status == ScenarioWorkStatus.Completed && cash - manager.CashCents == order.estimatedCostCents, "J settlement");
        Check(targets.Cohorts.All(t => t.Cohort.Density == 0f), "A/B cohort clearance");
        Check(!manager.PlantedJuveniles.Single(j => j.juvenileId == "PJ-FIXTURE-IN").alive, "exact juvenile missed");
        Check(manager.PlantedJuveniles.Single(j => j.juvenileId == "PJ-FIXTURE-OUT").alive
            && ecology.Cells[outside].Regeneration.All(c => Mathf.Approximately(c.Density, .4f)), "D outside vegetation changed");
        Check(retained.IsLiving && retained.IsCropTree, "E crop tree removed");
        Check(manager.UnderstoreyCells[index].shrubs == 0f && manager.UnderstoreyCells[index].grasses == 0f
            && manager.IsVegetationDisplayCleared(footprint.Center), "C understorey not cut back");
        Check(!manager.TryDesignateVegetationClearance(index), "J repeated clearance accepted");
        Check(manager.QueryClearance(footprint).Cohorts.Count == 0 && manager.QueryClearance(footprint).Juveniles.Count == 0
            && manager.QueryClearance(footprint).GroundPlants.Count == 0, "G unresolved visible targets");
        // This isolated effect deliberately has no annual snapshot. Invoke the
        // same production renderer used by annual snapshots and save restore.
        FindFirstObjectByType<ScenarioHabitatVisuals>().Rebuild(ecology, manager.UnderstoreyCells,
            manager.DeadwoodRecords, manager.PlantedJuveniles);
        Cursor.lockState = CursorLockMode.Locked; yield return null;
        yield return Capture("after");
        string clearedHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save(); saves.Load(); yield return null; yield return null;
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == clearedHash, "I save/load state mismatch");
        Check(manager.IsVegetationDisplayCleared(footprint.Center) && manager.QueryClearance(footprint).GroundPlants.Count == 0, "I cleared display returned");
        Check(FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Single(t => t.TreeId == retainedId).IsCropTree, "I retained crop lost");
        camera.transform.position = footprint.Center + new Vector3(0f, 1.65f, -3.5f); camera.transform.LookAt(footprint.Center);
        Cursor.lockState = CursorLockMode.Locked;
        yield return Capture("loaded");
        Debug.Log("CLEARANCE_FIXTURES_PASS A B C D E F G H I J cohorts=3 exactJuveniles=1 groundPatches=" + targets.GroundPlants.Count);
        Check(saves.LoadData(original, false), "fixture original restore");
        yield return null; yield return null;
        // The existing 1 m² planting circle affects every cohort proportionally,
        // while exact-position saplings and ground displays use their real bases.
        Vector3 spot = footprint.Center;
        foreach (string id in new[] { "sitka-spruce", "sessile-oak", "beech" })
            ecology.Cells[index].GetOrCreateCohort(spawner.ResolveSpecies(id)).Restore(1f, .3f, 0);
        ScenarioOneSaveData spotState = manager.CaptureSaveData();
        float radius = ClearanceFootprint.Planting(spot).Radius;
        spotState.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = "SPOT-IN", speciesId = "beech",
            position = spot + Vector3.right * (radius - .01f), cellIndex = index, ageYears = 3f, heightMeters = .6f, alive = true });
        spotState.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = "SPOT-OUT", speciesId = "beech",
            position = spot + Vector3.right * (radius + .01f), cellIndex = index, ageYears = 3f, heightMeters = .6f, alive = true });
        manager.RestoreSaveData(spotState, ForestSaveData.CurrentVersion);
        ClearanceTargets circle = manager.QueryClearance(ClearanceFootprint.Planting(spot), ecology.EcologicalYear);
        Check(circle.Cohorts.Count == 3 && circle.Juveniles.Count == 1, "circle mixed target query");
        Check(manager.TryPurchaseStock("beech-sapling", 1), "circle UI stock"); Call(player, "RefreshPlantingInventory", manager);
        Field(player, "isAimingGround", true); Field(player, "aimedSurfacePoint", spot); Field(player, "isLookingAtTree", false);
        Cursor.lockState = CursorLockMode.Locked;
        Field(player, "isPlantingMode", true); player.Clearance.Show(circle);
        camera.transform.position = spot + new Vector3(0f, 2.2f, -2.5f); camera.transform.LookAt(spot);
        yield return Capture("planting-circle");
#if UNITY_EDITOR
        if (Environment.GetEnvironmentVariable("CCF_CLEARANCE_CAPTURE") == "1")
        {
            foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
            { ClearanceVerification.SetGameSize(size.x, size.y); yield return Capture("planting-circle-" + size.x); }
            ClearanceVerification.SetGameSize(1600, 900);
        }
#endif
        player.Clearance.Clear(); Field(player, "isPlantingMode", false);
        Call(manager, "ApplyPlantingClearance", spot, ecology.EcologicalYear);
        Check(circle.Cohorts.All(c => Mathf.Abs(c.Cohort.Density - .96f) < .004f), "circle did not clear all cohort portions");
        Check(!manager.PlantedJuveniles.Single(j => j.juvenileId == "SPOT-IN").alive
            && manager.PlantedJuveniles.Single(j => j.juvenileId == "SPOT-OUT").alive, "circle exact boundary mismatch");
        float[] densities = circle.Cohorts.Select(c => c.Cohort.Density).ToArray();
        int patchCount = manager.ClearancePatches.Count;
        Debug.Log("CLEARANCE_REPEAT_DIAGNOSTIC before=" + string.Join(",", densities.Select(v => v.ToString("R")))
            + " year=" + ecology.EcologicalYear + " patches=" + manager.ClearancePatches.Count
            + " priorArea=" + ScenarioOneManager.ClearanceUnionArea(ecology.Cells[index].Center, footprint.HalfCell, manager.ClearancePatches)
            + " nextTargets=" + manager.QueryClearance(ClearanceFootprint.Planting(spot), ecology.EcologicalYear).Cohorts.Count);
        Call(manager, "ApplyPlantingClearance", spot, ecology.EcologicalYear);
        Check(circle.Cohorts.Select(c => c.Cohort.Density).SequenceEqual(densities), "repeated circle changed density twice: "
            + string.Join(",", circle.Cohorts.Select(c => c.Cohort.Density.ToString("R"))));
        Check(manager.ClearancePatches.Count == patchCount, "identical spot duplicated saved history");
        Debug.Log("CLEARANCE_CIRCLE_PASS area=1m2 species=3 inside=cleared outside=retained repeat=identical");
        Check(manager.TryDesignateVegetationClearance(index), "small circle blocked later whole-cell clearance");
        for (int cellIndex = 0; cellIndex < ecology.CellCount; cellIndex++)
        {
            manager.RestoreSaveData(original.scenarioOne, ForestSaveData.CurrentVersion);
            ForestRegenerationCohort cohort = ecology.Cells[cellIndex].GetOrCreateCohort(spawner.ResolveSpecies("beech"));
            cohort.Restore(1f, .3f, 0);
            Vector2 centre = ecology.Cells[cellIndex].Center; Vector3 repeated = new Vector3(centre.x, 0f, centre.y);
            Call(manager, "ApplyPlantingClearance", repeated, ecology.EcologicalYear); float density = cohort.Density;
            int records = manager.ClearancePatches.Count;
            Check(!manager.QueryClearance(ClearanceFootprint.Planting(repeated), ecology.EcologicalYear).HasTargets,
                "identical spot returned targets in cell " + cellIndex);
            Call(manager, "ApplyPlantingClearance", repeated, ecology.EcologicalYear);
            Check(cohort.Density == density && manager.ClearancePatches.Count == records, "identical spot changed density/history in cell " + cellIndex);
            yield return null;
        }
        Debug.Log("CLEARANCE_REPEAT_MATRIX_PASS cells=" + ecology.CellCount + " identicalSpot=noTargets density=unchanged");
        Check(saves.LoadData(original, false), "spot fixture restore");
        yield return null; yield return null;
        ecology.Cells[index].GetOrCreateCohort(spawner.ResolveSpecies("beech")).Restore(.4f, .3f, 0);
        manager.UnderstoreyCells[index].shrubs = .8f;
        Check(manager.TryDesignateVegetationClearance(index) && manager.ApprovePendingWork(), "annual clearance planning");
        order = manager.WorkOrders.Last(); cash = manager.CashCents;
        Check(manager.AdvanceYear(), "annual clearance resolution"); yield return null;
        Check(order.status == ScenarioWorkStatus.Completed && order.resolvedYear == ecology.EcologicalYear
            && cash - manager.CashCents == order.estimatedCostCents && manager.IsVegetationDisplayCleared(footprint.Center)
            && manager.QueryClearance(footprint).GroundPlants.Count == 0, "annual clearance state/display/charge");
        Check(!manager.TryDesignateVegetationClearance(index), "annual repeated clearance accepted");
        Debug.Log("CLEARANCE_ANNUAL_PASS completed=true debit=once treatmentYearDisplay=cleared");
        Check(saves.LoadData(original, false), "annual fixture restore");
    }
    private IEnumerator Capture(string name)
    {
        if (Environment.GetEnvironmentVariable("CCF_CLEARANCE_CAPTURE") != "1") yield break;
        for (int i = 0; i < 5; i++)
        {
            // Resizing the Editor Game view releases capture. Reacquire it as
            // a walking player would before reviewing the contextual prompt.
            if (!manager.AnyPanelOpen && !player.IsInspecting) Cursor.lockState = CursorLockMode.Locked;
            yield return null;
        }
        if (!Application.isBatchMode && player.Clearance.Visible && !manager.AnyPanelOpen && !player.IsInspecting)
        {
            ScenarioOneUiRoot ui = manager.GetComponent<ScenarioOneUiRoot>();
            object hud = ui.GetType().GetField("hud", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ui);
            var prompt = (UnityEngine.UIElements.Label)hud.GetType().GetField("prompt", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(hud);
            Check(prompt.resolvedStyle.display == UnityEngine.UIElements.DisplayStyle.Flex
                && prompt.text.Contains(player.Clearance.Targets.Footprint.SizeLabel), "rendered footprint label hidden/missing at " + name);
            if (player.IsPlantingMode)
            {
                var hotbar = (UnityEngine.UIElements.VisualElement)hud.GetType().GetField("hotbarBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(hud);
                var ground = (UnityEngine.UIElements.VisualElement)hud.GetType().GetField("groundPanel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(hud);
                Debug.Log("CLEARANCE_LAYOUT " + name + " hotbar=" + hotbar.worldBound + " ground=" + ground.worldBound);
                Check(!hotbar.worldBound.Overlaps(ground.worldBound), "planting controls overlap ground report at " + name);
            }
            Debug.Log("CLEARANCE_RENDERED_UI_PASS " + name + " " + Screen.width + "x" + Screen.height);
        }
        Camera camera = Camera.main; RenderTexture rt = RenderTexture.GetTemporary(1600, 900, 24);
        RenderTexture old = camera.targetTexture, active = RenderTexture.active;
        camera.targetTexture = rt; camera.Render(); RenderTexture.active = rt;
        var image = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); image.Apply();
        File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
        Destroy(image); camera.targetTexture = old; RenderTexture.active = active; RenderTexture.ReleaseTemporary(rt);
        if (!Application.isBatchMode)
        {
            yield return new WaitForEndOfFrame(); Texture2D screen = ScreenCapture.CaptureScreenshotAsTexture();
            Check(screen != null && screen.width >= 640, "UI rendered capture missing");
            File.WriteAllBytes(Path.Combine(output, name + "-ui.png"), screen.EncodeToPNG()); Destroy(screen);
        }
    }
}

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;
using Debug = UnityEngine.Debug;

// Disposable rendered review, never an automatic production component.
// Uses live planning/resolution APIs; save snapshots are external evidence only.
// Capture PASS means evidence was captured, NOT that visual acceptance passed.
public static class ScenarioOnePresentationReview
{
    private const string Requested = "CCF.FinalPresentation.Requested";
    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(1600, 900);
        EditorApplication.isPlaying = true;
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
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize");
        Type kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "CCF acceptance " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("Disposable final presentation review").AddComponent<ScenarioOnePresentationCapture>();
    }
}

public sealed class ScenarioOnePresentationCapture : MonoBehaviour
{
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestPlayer player;
    private Camera cameraView;
    private ForestSaveData original;
    private string output;
    private readonly List<string> observations = new List<string>();
    private readonly List<string> failures = new List<string>();
    private static readonly BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;

    private static void Check(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }
    private static void Field(object owner, string name, object value)
    { owner.GetType().GetField(name, Private).SetValue(owner, value); }
    private static void Call(object owner, string name, params object[] args)
    { owner.GetType().GetMethod(name, Private).Invoke(owner, args); }
    private List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT") ?? "/tmp/opencode/ccf-final-acceptance/evidence";
        Directory.CreateDirectory(output);
        yield return null;
        var steps = new Stack<IEnumerator>();
        steps.Push(Review());
        while (steps.Count > 0)
        {
            bool more; object current = null;
            try { more = steps.Peek().MoveNext(); if (more) current = steps.Peek().Current; }
            catch (Exception error) { failures.Add(error.ToString()); break; }
            if (!more) { steps.Pop(); continue; }
            if (current is IEnumerator child) { steps.Push(child); continue; }
            yield return current;
        }
        if (original != null && saves != null)
        {
            manager.EndReferencePreview();
            Call(manager, "SetWorkPlanOpen", false);
            Check(saves.LoadData(original, false), "original world restore");
            yield return null; yield return null;
        }
        File.WriteAllLines(Path.Combine(output, "observations.txt"), observations);
        if (failures.Count > 0) Debug.LogError("SCENARIO_ONE_PRESENTATION_CAPTURE_FAIL " + string.Join("\n", failures));
        else Debug.Log("SCENARIO_ONE_PRESENTATION_CAPTURE_PASS evidence=" + output + " visualAcceptance=REQUIRES_IMAGE_REVIEW");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failures.Count == 0 ? 0 : 1);
    }

    private void Note(string line) { observations.Add(line); Debug.Log("ASSET_REVIEW_" + line); }
    private void Pose(Vector3 eye, Vector3 target)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.position = new Vector3(eye.x, eye.y - 1.65f, eye.z);
        player.LookToward(target);
        Field(player, "verticalSpeed", 0f);
        controller.enabled = true;
        Field(player, "isInspecting", false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator Capture(string name, bool ui = false)
    {
        for (int i = 0; i < 8; i++) yield return null;
        // Explicit main-camera URP render: world evidence at a fixed resolution.
        RenderTexture rt = RenderTexture.GetTemporary(1600, 900, 24, RenderTextureFormat.ARGB32);
        RenderTexture old = cameraView.targetTexture;
        cameraView.targetTexture = rt;
        cameraView.Render();
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        var image = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); image.Apply();
        File.WriteAllBytes(Path.Combine(output, name + "-world.png"), image.EncodeToPNG());
        Destroy(image); RenderTexture.active = active; cameraView.targetTexture = old;
        RenderTexture.ReleaseTemporary(rt);
        if (ui && !Application.isBatchMode)
        {
            yield return new WaitForEndOfFrame();
            Texture2D screen = ScreenCapture.CaptureScreenshotAsTexture();
            Check(screen != null && screen.width >= 640, "Game view screenshot missing");
            File.WriteAllBytes(Path.Combine(output, name + "-ui.png"), screen.EncodeToPNG());
            Note("SCREEN " + name + " " + screen.width + "x" + screen.height);
            Destroy(screen);
        }
        Note("STATE " + name + " year=" + ecology.EcologicalYear + " living=" + Living().Count
            + " juveniles=" + manager.PlantedJuveniles.Count + " shelters=" + manager.Shelters.Count
            + " retained=" + manager.RetainedTimberM3.ToString("0.000") + " deadwood=" + manager.DeadwoodRecords.Count);
        ValidateVisuals(name);
    }

    private void ValidateVisuals(string label)
    {
        ScenarioHabitatPresentationVerification.VerifyActiveSitkaVisuals();
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        int invalid = renderers.Where(r => r.enabled && r.gameObject.activeInHierarchy)
            .SelectMany(r => r.sharedMaterials).Count(m => m == null || m.shader == null || m.shader.name.Contains("InternalError"));
        Check(invalid == 0, "missing/error materials at " + label + ": " + invalid);
        Transform[] children = manager.transform.Cast<Transform>().Where(t => t.gameObject.activeInHierarchy).ToArray();
        int residue = children.Count(t => t.name.StartsWith("Felling Residue "));
        int logs = children.Count(t => t.name.StartsWith("Fallen Log "));
        Check(logs == manager.DeadwoodRecords.Count, "log instance/record count at " + label);
        Check(residue == manager.WorkOrders.Count(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed && o.speciesId == "sitka-spruce"), "residue instance/order count at " + label);
        foreach (Transform t in children.Where(t => t.name.StartsWith("Felling Residue ")))
        {
            Bounds b = RenderBounds(t);
            Check(b.size.x < 3.1f && b.size.z < 3.1f && b.size.y < .5f && Mathf.Abs(b.min.y) < .025f, "residue bounds/ground " + t.name + " " + b);
        }
        Note("VISUALS " + label + " renderers=" + renderers.Length + " materialsMissing=0 residue=" + residue + " logs=" + logs);
        foreach (Transform t in ecology.transform.Cast<Transform>().Where(t => t.name.Contains(" seedling cell ")))
        {
            int cell = int.Parse(t.name.Substring(t.name.LastIndexOf(' ') + 1));
            ForestRegenerationCohort cohort = ecology.Cells[cell].Regeneration.FirstOrDefault(c =>
                c.Species != null && t.name.StartsWith(c.Species.DisplayName + " seedling cell "));
            if (cohort != null)
                Note("REGEN_SCALE " + label + " " + t.name + " stateHeight=" + cohort.Height.ToString("0.000")
                    + " renderHeight=" + RenderBounds(t).size.y.ToString("0.000"));
        }
    }

    private static Bounds RenderBounds(Transform root)
    {
        Renderer[] rs = root.GetComponentsInChildren<Renderer>(true);
        Bounds b = rs[0].bounds;
        foreach (Renderer r in rs.Skip(1)) b.Encapsulate(r.bounds);
        return b;
    }

    private IEnumerator Performance(string label)
    {
        int vsync = QualitySettings.vSyncCount; int limit = Application.targetFrameRate;
        QualitySettings.vSyncCount = 0; Application.targetFrameRate = -1;
        for (int i = 0; i < 30; i++) yield return null;
        var times = new List<double>();
        double previous = Time.realtimeSinceStartupAsDouble;
        for (int i = 0; i < 120; i++)
        {
            yield return null;
            double now = Time.realtimeSinceStartupAsDouble;
            times.Add((now - previous) * 1000); previous = now;
        }
        times.Sort();
        Note("PERFORMANCE " + label + " meanMs=" + times.Average().ToString("0.00") + " medianMs=" + times[60].ToString("0.00")
            + " p95Ms=" + times[114].ToString("0.00") + " editorBatches=" + Stat("batchesCount") + " editorTriangles=" + UnityStats.triangles
            + " screen=" + Screen.width + "x" + Screen.height + " device=" + SystemInfo.graphicsDeviceName + " api=" + SystemInfo.graphicsDeviceType
            + " quality=" + QualitySettings.names[QualitySettings.GetQualityLevel()] + " samples=120 vSync=0 mode=EditorGameplay");
        QualitySettings.vSyncCount = vsync; Application.targetFrameRate = limit;
    }

    private static string Stat(string name)
    {
        PropertyInfo property = typeof(UnityStats).GetProperty(name, BindingFlags.Public | BindingFlags.Static);
        return property != null ? Convert.ToString(property.GetValue(null)) : "unavailable";
    }

    private IEnumerator PlanUI(string name, bool review = false, float scrollY = 0)
    {
        // UI Toolkit screens: the Work Plan or the Annual Review, scrolled for evidence.
        ScenarioOneUiRoot uiRoot = manager.GetComponent<ScenarioOneUiRoot>();
        if (review) uiRoot.ShowReview(); else uiRoot.ShowWorkPlan();
        for (int i = 0; i < 3; i++) yield return null;
        foreach (var view in UnityEngine.UIElements.UQueryExtensions.Query<UnityEngine.UIElements.ScrollView>(uiRoot.RootElement).ToList())
            if (view.resolvedStyle.display == UnityEngine.UIElements.DisplayStyle.Flex) view.scrollOffset = new Vector2(0, scrollY);
        foreach (var size in new[] { new Vector2Int(1600, 900), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
        {
            ScenarioOnePresentationReview.SetGameSize(size.x, size.y);
            yield return Capture(name + "-" + size.x, true);
        }
        ScenarioOnePresentationReview.SetGameSize(1600, 900);
        uiRoot.CloseAll();
    }

    private IEnumerator MapUI(string name)
    {
        ScenarioOneUiRoot uiRoot = manager.GetComponent<ScenarioOneUiRoot>();
        uiRoot.ShowMap();
        foreach (var size in new[] { new Vector2Int(1600, 900), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
        {
            ScenarioOnePresentationReview.SetGameSize(size.x, size.y);
            yield return Capture(name + "-" + size.x, true);
        }
        ScenarioOnePresentationReview.SetGameSize(1600, 900);
        uiRoot.CloseAll();
    }

    private IEnumerator Review()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>(); ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>(); marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        player = FindFirstObjectByType<ForestPlayer>(); cameraView = Camera.main;
        Check(manager != null && ecology != null && saves != null && player != null && cameraView != null, "scene systems");
        original = saves.CaptureData();
        Check(ecology.EcologicalYear == 0 && Living().Count == 336, "not fresh authoritative Year 0");
        Check(ForestSaveData.CurrentVersion == 16 && manager.Definition.BackgroundBrowsePressure == .2f, "configuration");
        Note("ENV engine=" + Application.unityVersion + " device=" + SystemInfo.graphicsDeviceName + " cpu=" + SystemInfo.processorType);
        Pose(new Vector3(-.43f, 1.75f, 5.25f), new Vector3(3, 1.5f, 1));
        yield return Capture("01-start", true); yield return Performance("Year0");
        Pose(new Vector3(0, 1.65f, 13), new Vector3(0, .05f, 3));
        yield return Capture("01-track-clearing", true);
        Pose(new Vector3(-8, 1.65f, -6), new Vector3(-7, .1f, -9));
        yield return Capture("01-floor-moss", true);
        List<ForestTree> living = Living();
        List<string> crops = living.GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10), Mathf.FloorToInt(t.transform.position.z / 10)))
            .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId).OrderBy(id => id).ToList();
        ForestTree crop = living.First(t => t.TreeId == crops.OrderBy(id => Vector3.Distance(living.First(t=>t.TreeId==id).transform.position,new Vector3(0,0,5))).First());
        Vector3 cropEye = crop.transform.position + new Vector3(0,1.65f,3);
        Pose(cropEye, crop.transform.position + Vector3.up*1.6f);
        yield return Capture("02-unmarked-tree", true);
        string sourceBefore = crop.ActiveVisualSource;
        marking.Mark(crop, TreeMarkType.CropTree, false);
        yield return Capture("02-crop-mark", true);
        Check(crop.ActiveVisualSource == sourceBefore, "Crop designation changed active tree family");
        Call(player, "InspectTree", crop);
        yield return Capture("02-inspection", true);
        foreach (var size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            ScenarioOnePresentationReview.SetGameSize(size.x,size.y);
            yield return Capture("02-inspection-" + size.x,true);
        }
        ScenarioOnePresentationReview.SetGameSize(1600,900);
        Field(player, "isInspecting", false);
        Pose(cropEye, crop.transform.position + new Vector3(.7f,0,.7f));
        yield return Capture("02-ground-diagnosis", true);
        // Pruning fixture uses the real order/resolution, then returns to the start.
        Check(manager.TryDesignatePruning(crop.TreeId) && manager.ApprovePendingWork() && manager.AdvanceYear(), "live pruning");
        Pose(cropEye, crop.transform.position + Vector3.up*2.0f);
        yield return Capture("03-pruned", true);
        Note("PRUNING tree="+crop.TreeId+" lifts="+crop.PruningLifts+" source="+crop.ActiveVisualSource+" crownBase="+crop.CrownBaseHeightM);
        Check(saves.LoadData(original,false),"reset after pruning fixture"); yield return null; yield return null;
        living=Living(); foreach(string id in crops) marking.Mark(living.First(t=>t.TreeId==id),TreeMarkType.CropTree,false);
        List<ForestTree> fell=Thinning(living,crops,.27f);
        for(int i=0;i<fell.Count;i++)
        {
            manager.PlanningFellingOutcome=i<4?FellingMaterialOutcome.RetainAsFallenDeadwood:i<7?FellingMaterialOutcome.KeepForUse:FellingMaterialOutcome.SellAndExtract;
            marking.Mark(fell[i],TreeMarkType.Fell,false); manager.AddMarkedTreesToWorkPlan();
        }
        Pose(cropEye, fell.OrderBy(t=>Vector3.Distance(t.transform.position,cropEye)).First().transform.position+Vector3.up);
        yield return Capture("02-fell-marks", true); yield return PlanUI("05-pending-work");
        Check(manager.ApprovePendingWork(),"approve harvest"); yield return PlanUI("05-approved-work");
        Check(manager.AdvanceYear(),"harvest resolve");
        Pose(new Vector3(0,1.65f,5),new Vector3(0,1.4f,-8));
        yield return Capture("04-thinned",true); yield return Performance("PostThinning");
        // Same residue patch, fresh now and dry later, from inside the stand at walking distance.
        Transform brash=manager.transform.Cast<Transform>().Where(t=>t.gameObject.activeInHierarchy&&t.name.StartsWith("Felling Residue "))
            .OrderBy(t=>t.position.sqrMagnitude).ThenBy(t=>t.name,StringComparer.Ordinal).First();
        string brashName=brash.name; Vector3 brashPos=RenderBounds(brash).center;
        Vector3 inward=new Vector3(-brashPos.x,0,-brashPos.z).normalized;
        Pose(brashPos+inward*3f+Vector3.up*1.65f,brashPos);
        yield return Capture("04-fresh-brash",true);
        ScenarioDeadwoodRecord log=manager.DeadwoodRecords.OrderBy(d => d.worldPosition.sqrMagnitude).First();
        Vector3 logInward=new Vector3(-log.worldPosition.x,0,-log.worldPosition.z).normalized;
        Pose(log.worldPosition+logInward*3f+Vector3.up*1.65f,log.worldPosition+Vector3.up*.2f);
        yield return Capture("05-deadwood",true); yield return PlanUI("05-annual-review",true);
        yield return PlanUI("05-annual-outcomes",true,450);
        yield return PlanUI("05-stock-summary",false,1100);
        yield return PlanUI("05-objectives",false,100000);
        Check(manager.RetainedTimberM3>0&&manager.DeadwoodRecords.Count==4,"three dispositions");
        Note("DISPOSITIONS " + JsonUtility.ToJson(manager.AnnualReports.Last()));
        Check(manager.TryPurchaseStock("sessile-oak-sapling",8)&&manager.TryPurchaseStock("beech-sapling",8),"stock purchase");
        int pair=0;
        foreach(int cell in Enumerable.Range(0,ecology.CellCount).OrderByDescending(i=>ecology.Cells[i].Light).ThenBy(i=>i))
        {
            if(pair==8)break;
            if(PlantPair(cell,pair<4?"sessile-oak-sapling":"beech-sapling"))pair++;
        }
        Check(pair==8,"plant pairs");
        ScenarioOneWorkOrder firstPlant=manager.WorkOrders.First(o=>o.type==ScenarioWorkType.PlantJuvenile&&o.IsOpen);
        Vector3 plantingEye=firstPlant.worldPosition+new Vector3(1.5f,1.65f,2);
        Pose(plantingEye,firstPlant.worldPosition+Vector3.up*.5f);
        yield return Capture("06-planting-markers",true); yield return MapUI("06-stand-map"); yield return PlanUI("06-planting-execution",false,650);
        Check(manager.ApprovePendingWork()&&manager.AdvanceYear(),"plant resolve");
        Pose(plantingEye,firstPlant.worldPosition+Vector3.up*.55f);
        yield return Capture("07-oak-shelters",true); yield return Performance("PlantingShelters");
        PlantedJuvenile beech=manager.PlantedJuveniles.First(j=>j.speciesId=="beech");
        Pose(beech.position+new Vector3(1.6f,1.65f,2.4f),beech.position+Vector3.up*.55f);
        yield return Capture("06-beech",true);
        ForestSaveData planted=saves.CaptureData();
        ForestSaveData statuses=JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(planted));
        statuses.scenarioOne.shelters[0].installedYear=0;
        statuses.scenarioOne.shelters[0].effectiveYears=1;
        statuses.scenarioOne.shelters[1].failedYear=ecology.EcologicalYear;
        Check(saves.LoadData(statuses,false),"status fixture restore");yield return null;yield return null;
        Pose(plantingEye,firstPlant.worldPosition+Vector3.up*.5f);yield return Capture("07-expired-failed-fixture",true);
        Check(saves.LoadData(planted,false),"return from status fixture");yield return null;yield return null;
        yield return Advance(6);
        Pose(plantingEye,firstPlant.worldPosition+Vector3.up);
        yield return Capture("08-year6",true);
        ForestSaveData mid=saves.CaptureData();
        string signature=VisualSignature(); string hash=ScenarioReferenceArchive.WorldHash(mid);
        File.WriteAllText(Path.Combine(output,"player-year6.json"),JsonUtility.ToJson(mid,true));
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(mid)),false),"first restore");yield return null;yield return null;
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(mid)),false),"repeat restore");yield return null;yield return null;
        Check(signature==VisualSignature(),"visual signature after repeated restore");
        yield return Capture("10-loaded-year6",true);
        foreach(int year in new[]{20,50,100})
        {
            Check(manager.TryBeginReferencePreview(year),"reference "+year);yield return null;yield return null;
            Check(FindFirstObjectByType<ScenarioProtectionVisuals>().VisualCount==0,"preview shelter leak");
            Pose(new Vector3(0,1.65f,5),new Vector3(0,2,-7));yield return Capture("10-reference"+year,true);
            manager.EndReferencePreview();yield return null;yield return null;
            Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData())==hash&&VisualSignature()==signature,"preview return state/signature");
        }
        Pose(plantingEye,firstPlant.worldPosition+Vector3.up);yield return Capture("10-returned-year6",true);
        yield return Advance(16);
        Pose(plantingEye,firstPlant.worldPosition+Vector3.up*2);yield return Capture("08-promoted-year16",true);
        Pose(new Vector3(0,1.65f,5),new Vector3(0,2,-8));yield return Performance("LaterStand");
        Transform dry=manager.transform.Cast<Transform>().First(t=>t.gameObject.activeInHierarchy&&t.name==brashName);
        Pose(brashPos+inward*3f+Vector3.up*1.65f,brashPos);yield return Capture("09-dry-brash",true);
        Note("DRY_BRASH "+brashName+" source="+dry.GetChild(0).name+" bounds="+RenderBounds(dry).size.ToString("F2"));
        Vector3 logIn=new Vector3(-log.worldPosition.x,0,-log.worldPosition.z).normalized;
        Pose(log.worldPosition+logIn*3f+Vector3.up*1.65f,log.worldPosition+Vector3.up*.2f);yield return Capture("09-dry-log",true);
        fell=Thinning(Living().Where(t=>t.Species.SpeciesId=="sitka-spruce").ToList(),crops,.20f);
        manager.PlanningFellingOutcome=FellingMaterialOutcome.SellAndExtract;
        foreach(ForestTree t in fell)marking.Mark(t,TreeMarkType.Fell,false);
        manager.AddMarkedTreesToWorkPlan();Check(manager.ApprovePendingWork()&&manager.AdvanceYear(),"second intervention");
        Pose(new Vector3(0,1.65f,5),new Vector3(0,1,-8));yield return Capture("09-second-intervention",true);
        yield return Advance(30);yield return Capture("09-year30",true);yield return PlanUI("09-completion-review",true);
        File.WriteAllText(Path.Combine(output,"player-year30.json"),JsonUtility.ToJson(saves.CaptureData(),true));
        Note("RECONSTRUCTION repeatedLoadAndPreview=PASS playerSheltersRestored=true");
    }

    private IEnumerator Advance(int year)
    {
        while(ecology.EcologicalYear<year){Check(manager.AdvanceYear(),"advance "+year+" "+manager.Feedback);yield return null;}
        yield return null;
    }
    private static float BA(ForestTree t)=>Mathf.PI*Mathf.Pow(t.Diameter/200f,2);
    private static List<ForestTree> Thinning(List<ForestTree> trees,List<string> crops,float fraction)
    {
        var result=new List<ForestTree>();float taken=0,target=trees.Sum(BA)*fraction;
        List<ForestTree> cropTrees=trees.Where(t=>crops.Contains(t.TreeId)).ToList();
        foreach(ForestTree t in trees.Where(t=>!crops.Contains(t.TreeId)&&t.CanChop)
            .OrderBy(t=>cropTrees.Count==0?0:cropTrees.Min(c=>Vector3.Distance(c.transform.position,t.transform.position))).ThenBy(t=>t.TreeId,StringComparer.Ordinal))
        {if(taken>=target)break;result.Add(t);taken+=BA(t);}return result;
    }
    private bool PlantPair(int cell,string item)
    {
        Vector2 c=ecology.Cells[cell].Center;Vector3? first=null;
        for(int ix=-2;ix<=2;ix++)for(int iz=-2;iz<=2;iz++)
        {
            Vector3 p=new Vector3(c.x+ix*.9f,0,c.y+iz*.9f);
            if(ecology.GetCellIndex(p)!=cell||(first.HasValue&&Vector3.Distance(first.Value,p)<.8f))continue;
            if(!manager.TryDesignateExactPlanting(item,p,first.HasValue?WorkExecutionMethod.Contractor:WorkExecutionMethod.LandownerSimulated,!first.HasValue))continue;
            if(first.HasValue)return true;first=p;
        }return false;
    }
    private string VisualSignature()=>string.Join(";",manager.transform.Cast<Transform>().Where(t=>t.gameObject.activeInHierarchy
        &&(t.name.StartsWith("Felling Residue ")||t.name.StartsWith("Fallen Log "))).OrderBy(t=>t.name)
        .Select(t=>t.name+"/"+t.position.ToString("F4")+"/"+RenderBounds(t).size.ToString("F4")))
        +"|shelters="+FindFirstObjectByType<ScenarioProtectionVisuals>().VisualCount;
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable: stage under Assets/ForestPrototype only for this gate, then remove source/meta.
public static class SaveCellIndexHardeningVerification
{
    private const string ExitKey = "CCF.SaveCellIndexHardening.Exit";
    [InitializeOnLoadMethod] private static void ExitHook()
    {
        EditorApplication.update += () =>
        {
            int code = SessionState.GetInt(ExitKey, -1);
            if (code < 0 || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            SessionState.EraseInt(ExitKey); EditorApplication.Exit(code);
        };
    }
    public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("SaveCellIndexHardeningVerification.Begin"))
            new GameObject("Disposable save cell-index verification").AddComponent<SaveCellIndexHardeningRunner>();
    }
    public static void Finish(int code) { SessionState.SetInt(ExitKey, code); EditorApplication.delayCall += EditorApplication.ExitPlaymode; }
}
public sealed class SaveCellIndexHardeningRunner : MonoBehaviour
{
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private string path;
    private readonly Dictionary<string, byte[]> originals = new Dictionary<string, byte[]>();
    private int cases, rejected, accepted;
    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        var routines = new Stack<IEnumerator>(); routines.Push(Verify());
        while (routines.Count > 0)
        {
            bool more = false; object current = null;
            try { more = routines.Peek().MoveNext(); if (more) current = routines.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { routines.Pop(); continue; }
            if (current is IEnumerator nested) routines.Push(nested);
            else yield return current;
        }
        foreach (var entry in originals)
            if (entry.Value == null) { if (File.Exists(entry.Key)) File.Delete(entry.Key); }
            else File.WriteAllBytes(entry.Key, entry.Value);
        if (failure == null) Debug.Log($"SAVE_CELL_INDEX_HARDENING_VERIFY_PASS cases={cases} rejected={rejected} accepted={accepted} diskParity=exact noMutation=exact");
        else Debug.LogError("SAVE_CELL_INDEX_HARDENING_VERIFY_FAIL " + failure);
        SaveCellIndexHardeningVerification.Finish(failure == null ? 0 : 1);
    }
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
    // Since Rack Planning V2 (save v21, D-064) the live Enlarged80 world uses rack planning model 1, which the save validator
    // correctly refuses on any other stand. A Legacy40 copy must therefore also drop to model 0 with no V2 plan.
    // Returns how many rack-owned work orders were removed (a fresh world is expected to have none).
    private static int MakeRackModelZero(ForestSaveData data)
    {
        ScenarioOneSaveData scenario = data.scenarioOne;
        scenario.rackPlanningModel = RackPlanningModel.Legacy;
        scenario.racks = new List<RackSaveData>();
        scenario.rackPlanConfirmed = false;
        scenario.nextRackId = 1;
        if (scenario.workOrders == null) scenario.workOrders = new List<ScenarioOneWorkOrder>();
        return scenario.workOrders.RemoveAll(o => o != null && o.rackId > 0);
    }
    private string World()
    {
        ForestSaveData data = saves.CaptureData();
        return ecology.StandGeometryModelVersion + "|" + ecology.EcologicalYear + "|"
            + JsonUtility.ToJson(player.transform.position) + "|" + JsonUtility.ToJson(player.transform.rotation) + "|"
            + ScenarioReferenceArchive.CurrentWorldHash(data) + "|" + JsonUtility.ToJson(data.scenarioOne);
    }
    private string Disk() => string.Join("|", new[] { path, path + ".bak", path + ".tmp" }
        .Select(p => File.Exists(p) ? Convert.ToBase64String(File.ReadAllBytes(p)) : "missing"));
    private IEnumerator Fixture(string label, ForestSaveData data, bool valid, bool? centralValid = null)
    {
        cases++;
        File.WriteAllText(path, JsonUtility.ToJson(data));
        File.WriteAllText(path + ".bak", "backup fixture — must remain unchanged");
        File.WriteAllText(path + ".tmp", "temporary fixture — must remain unchanged");
        string world = World(), disk = Disk();
        string beforeWorldHash = ScenarioReferenceArchive.Hash(world), beforeDiskHash = ScenarioReferenceArchive.Hash(disk);
        // Deliberately pass the wrong LIVE grid size to prove geometry comes from the save.
        int wrongCount = data.standGeometryModel == 0 ? 256 : 64;
        string validation = ForestSaveValidation.Validate(data, data.trees.Count, wrongCount);
        Check((validation == null) == (centralValid ?? valid), label + " central validation mismatch: " + validation);
        bool can = saves.CanLoad(out string refusal);
        Check(can == valid, label + " CanLoad mismatch: " + refusal);
        Check(World() == world && Disk() == disk, label + " CanLoad mutated world/disk");
        bool loaded = saves.LoadData(Clone(data), false);
        Check(loaded == valid, label + " LoadData acceptance differs from CanLoad");
        if (!valid)
        {
            rejected++;
            Check(World() == world && Disk() == disk, label + " LoadData mutated before refusal");
            saves.Load();
            Check(World() == world && Disk() == disk, label + " disk Load mutated before refusal");
            yield return null;
            Check(World() == world && Disk() == disk, label + " delayed mutation after refusal");
        }
        else
        {
            accepted++;
            Check(Disk() == disk, label + " valid LoadData wrote disk");
            yield return null;
            saves.Load(); yield return null;
            Check(ecology.StandGeometryModelVersion == StandGeometryModel.ForSave(data), label + " disk Load geometry mismatch");
            Check(ecology.EcologicalYear == data.ecologicalYear && Disk() == disk, label + " valid disk Load mismatch");
        }
        Debug.Log("SAVE_CELL_CASE " + label + " " + (valid ? "ACCEPT" : "REJECT") + " CanLoad=LoadData=Load disk=unchanged" + (valid ? "" : " world=unchanged") + " beforeWorld=" + beforeWorldHash + " afterWorld=" + ScenarioReferenceArchive.Hash(World())
            + " beforeDisk=" + beforeDiskHash + " afterDisk=" + ScenarioReferenceArchive.Hash(Disk()));
    }
    private static void Assign(ForestSaveData data, string field, int index)
    {
        var scenario = data.scenarioOne;
        switch (field)
        {
            case "ecology": data.cells[data.cells.Count - 1].index = index; break;
            case "understorey": scenario.understoreyCells[scenario.understoreyCells.Count - 1].cellIndex = index; break;
            case "juvenile": scenario.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId="PJ0001",speciesId="beech",position=Vector3.zero,cellIndex=index,legacyCohortManaged=true }); break;
            case "deadwood": scenario.deadwoodRecords.Add(new ScenarioDeadwoodRecord { deadwoodId="DW0001",treeId=data.trees[0].treeId,speciesId=data.trees[0].speciesId,cellIndex=index,worldPosition=Vector3.zero,originalVolumeM3=0.1f,remainingVolumeM3=0.1f,originalHeightMeters=2,originalDiameterCm=10 }); break;
            case "event": scenario.managementEvents.Add(new ScenarioManagementEvent { eventId=1,cellIndex=index,eventType=ScenarioManagementEventType.YearAdvanced }); break;
            default:
                var type=(ScenarioWorkType)Enum.Parse(typeof(ScenarioWorkType),field);
                scenario.workOrders.Add(new ScenarioOneWorkOrder { workOrderId=1,type=type,status=ScenarioWorkStatus.Completed,targetTreeId=data.trees[0].treeId,cellIndex=index,speciesId="beech",worldPosition=Vector3.zero }); break;
        }
    }
    private IEnumerator Verify()
    {
        saves=FindFirstObjectByType<ForestSaveController>(); ecology=FindFirstObjectByType<ForestEcologyController>(); player=FindFirstObjectByType<ForestPlayer>();
        Check(saves != null && ecology != null && player != null,"scene services missing");
        player.enabled=false; // Freeze movement during the no-mutation snapshots; no simulation changes.
        path=Path.Combine(Application.persistentDataPath,"forest-save.json"); Directory.CreateDirectory(Path.GetDirectoryName(path));
        foreach (string p in new[] { path,path+".bak",path+".tmp" }) originals[p]=File.Exists(p)?File.ReadAllBytes(p):null;
        ForestSaveData enlarged=saves.CaptureData(); Check(enlarged.standGeometryModel==1 && ecology.CellCount==256,"gate must start on actual Enlarged80 policy");
        // Ecology saves intentionally omit pristine cells. Materialize a valid complete
        // fixture so every boundary case can target a known record without relying on capture order.
        var recorded = enlarged.cells.ToDictionary(c => c.index);
        enlarged.cells = Enumerable.Range(0,256).Select(i => recorded.TryGetValue(i,out var cell)
            ? cell : new ForestCellSaveData { index=i,establishmentSuitability=1f }).ToList();
        // Keep a pristine valid Enlarged80 baseline. The later fixture loop loads other saves into the live
        // world; reusing the working capture for the startup-Continue case can carry a fixture-mutated grid.
        ForestSaveData enlargedBaseline = Clone(enlarged);
        ForestSaveData legacy=Clone(enlarged); legacy.standGeometryModel=0;legacy.cells=legacy.cells.Take(64).ToList();legacy.scenarioOne.understoreyCells=legacy.scenarioOne.understoreyCells.Take(64).ToList();
        int rackOrdersRemoved=MakeRackModelZero(legacy);
        // Precondition: the Legacy40 basis is a valid model-0 world, so every geometry=0 verdict below is about cell indices.
        Check(legacy.standGeometryModel==0 && legacy.cells.Count==64 && legacy.scenarioOne.understoreyCells.Count==64,"Legacy40 basis must be the 64-cell model-0 stand");
        Check(legacy.scenarioOne.rackPlanningModel==RackPlanningModel.Legacy && legacy.scenarioOne.racks.Count==0 && !legacy.scenarioOne.rackPlanConfirmed && legacy.scenarioOne.nextRackId==1
            && !legacy.scenarioOne.workOrders.Any(o => o!=null && o.rackId>0),"Legacy40 basis must carry no Rack V2 state");
        Check(ForestSaveValidation.ValidateRackV2(legacy)==null,"Legacy40 basis refused by the Rack V2 validator: "+ForestSaveValidation.ValidateRackV2(legacy));
        Debug.Log("SAVE_CELL_LEGACY40_BASIS geometry=0 cells=64 rackPlanningModel=0 racks=0 confirmed=false nextRackId=1 rackOwnedOrdersRemoved="+rackOrdersRemoved
            +" enlargedRackPlanningModel="+enlarged.scenarioOne.rackPlanningModel+" enlargedRacks="+enlarged.scenarioOne.racks.Count);
        string[] fields={"ecology","understorey","juvenile","deadwood","event","FellTree","PlantJuvenile","RemoveRegeneration","PruneTree","SalvageDeadwood"};
        foreach (ForestSaveData basis in new[] { legacy,enlarged })
        {
            int count=StandGeometryModel.CellCount(StandGeometryModel.ForSave(basis));
            foreach (string field in fields)
            {
                bool required=field=="ecology" || field=="understorey" || field=="PlantJuvenile" || field=="RemoveRegeneration";
                Debug.Log("SAVE_CELL_AUDIT "+field+" geometry="+basis.standGeometryModel+" sentinel="+(required?"none":"-1")+" expectedCells="+count);
                foreach (int index in new[] {-1,-2,count,int.MaxValue,count-1})
                {
                    ForestSaveData fixture=Clone(basis); Assign(fixture,field,index);
                    yield return Fixture(field+" geometry="+basis.standGeometryModel+" index="+index,fixture,index==count-1 || (!required && index==-1));
                }
            }
            ForestSaveData duplicate=Clone(basis);duplicate.cells[1].index=duplicate.cells[0].index;
            yield return Fixture("duplicate ecology geometry="+basis.standGeometryModel,duplicate,false);
            duplicate=Clone(basis);duplicate.scenarioOne.understoreyCells[1].cellIndex=0;
            yield return Fixture("duplicate understorey geometry="+basis.standGeometryModel,duplicate,false);
            ForestSaveData sparse=Clone(basis);sparse.cells.RemoveAt(count-1);
            yield return Fixture("sparse ecology grid geometry="+basis.standGeometryModel,sparse,true);
            ForestSaveData incomplete=Clone(basis);incomplete.scenarioOne.understoreyCells.RemoveAt(count-1);
            yield return Fixture("incomplete model2 grid geometry="+basis.standGeometryModel,incomplete,false);
            ForestSaveData order=Clone(basis);order.scenarioOne.understoreyCells.Reverse();
            yield return Fixture("unordered model2 grid geometry="+basis.standGeometryModel,order,false);
            ForestSaveData patch=Clone(basis);patch.scenarioOne.clearancePatches.Add(new PlantingClearancePatch { center=new Vector3(1000,0,1000),radiusMeters=1,createdYear=patch.ecologicalYear,competitionUpdatedYear=patch.ecologicalYear });
            yield return Fixture("outside model2 patch geometry="+basis.standGeometryModel,patch,false,true);
            // Duplicated cell identities are valid for individual/history/work records.
            foreach(string field in new[]{"juvenile","deadwood","event","FellTree","PlantJuvenile","RemoveRegeneration","PruneTree","SalvageDeadwood"})
            {
                ForestSaveData repeat=Clone(basis);Assign(repeat,field,count-1);Assign(repeat,field,count-1);
                // Record IDs must remain distinct, irrespective of shared cell identity.
                if(field=="juvenile")repeat.scenarioOne.plantedJuveniles[1].juvenileId="PJ0002";
                if(field=="deadwood")repeat.scenarioOne.deadwoodRecords[1].deadwoodId="DW0002";
                if(field=="event")repeat.scenarioOne.managementEvents[1].eventId=2;
                if(repeat.scenarioOne.workOrders.Count==2)repeat.scenarioOne.workOrders[1].workOrderId=2;
                yield return Fixture("shared cell "+field+" geometry="+basis.standGeometryModel,repeat,true);
            }
        }
        // Legitimate historical absent grids and sentinel values stay accepted.
        ForestSaveData historical=Clone(legacy);historical.version=12;historical.regenerationModel=0;historical.growthModel=0;historical.stormModel=0;
        historical.scenarioOne.understoreyCells.Clear();historical.cells=historical.cells.Take(2).ToList();
        Assign(historical,"juvenile",-1);Assign(historical,"deadwood",-1);Assign(historical,"event",-1);Assign(historical,"FellTree",-1);
        yield return Fixture("historical v12 partial grids and sentinels",historical,true);
        ForestSaveData previous=Clone(legacy);previous.version=19;yield return Fixture("historical v19",previous,true);
        yield return Fixture("restore enlarged baseline",Clone(enlargedBaseline),true);
        // Real title integration: valid enables Continue; malformed nested index disables it.
        File.WriteAllText(path,JsonUtility.ToJson(enlargedBaseline));
        var menu=FindFirstObjectByType<StandaloneSessionMenu>();if(menu==null)menu=new GameObject("Disposable startup menu").AddComponent<StandaloneSessionMenu>();
        menu.ShowStartupScreen();for(int i=0;i<120&&!StandaloneSessionMenu.IsOpen;i++)yield return null;
        Check(StandaloneSessionMenu.IsOpen && menu.ContinueAvailable,"valid save must enable Continue");
        ForestSaveData bad=Clone(legacy);Assign(bad,"juvenile",64);File.WriteAllText(path,JsonUtility.ToJson(bad));
        string before=World(),bytes=Disk();menu.ContinueSavedForest();yield return null;
        Check(StandaloneSessionMenu.IsOpen && !menu.ContinueAvailable && menu.ContinueNote.Length<120,"malformed nested save must disable Continue with concise refusal");
        Check(World()==before && Disk()==bytes,"rejected Continue mutated live world or disk");
        menu.StartNewScenario();yield return null;
        Check(!StandaloneSessionMenu.IsOpen && World()==before && Disk()==bytes,"Start New changed generated world or removed save");
        Debug.Log("SAVE_CELL_STARTUP_PASS validContinue=enabled malformedContinue=disabled refusalLength="+menu.ContinueNote.Length+" startNew=unchanged");
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Disposable U4 decision/persistence fixtures. These synthetic reports/snapshots isolate completion policy;
// they are not a physical forest playthrough. No disk save writes. Stage only for this explicit gate.
public static class N3CompletionVerification
{
    private const string Requested = "N3CompletionVerification.Requested";
    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("N3 completion qualification").AddComponent<N3CompletionGate>();
    }
}
public sealed class N3CompletionGate : MonoBehaviour
{
    private ForestSaveController saves;
    private ForestSaveData original;
    private ScenarioOneManager manager;
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message);Debug.Log("N3_U4_CHECK_PASS "+message); }
    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        try { Verify(); } catch (Exception e) { failure = e; }
        try { if (original != null) Check(saves.LoadData(original, false), "original world restored"); }
        catch (Exception e) { failure = failure ?? e; }
        if (failure == null) Debug.Log("N3_U4_VERIFY_PASS"); else Debug.LogError("N3_U4_VERIFY_FAIL "+failure);
        EditorApplication.ExitPlaymode();EditorApplication.Exit(failure == null ? 0 : 1);
    }
    private void Evaluate() => typeof(ScenarioOneManager).GetMethod("EvaluateProgress", BindingFlags.Instance|BindingFlags.NonPublic).Invoke(manager,null);
    private static ScenarioOneWorkOrder Order(int id,int year,ScenarioWorkType type,string species="")
        => new ScenarioOneWorkOrder {workOrderId=id,resolvedYear=year,status=ScenarioWorkStatus.Completed,type=type,speciesId=species,cellIndex=0};
    private static ScenarioManagementEvent Work(int id,ScenarioOneWorkOrder order)
        => new ScenarioManagementEvent {eventId=id,year=order.resolvedYear,workOrderId=order.workOrderId,
            eventType=ScenarioManagementEventType.WorkResolved,outcome=ScenarioManagementOutcome.Succeeded,taskType=order.type,speciesId=order.speciesId,cellIndex=0};
    private ScenarioOneSaveData Fixture(bool second,bool review)
    {
        ScenarioOneSaveData data = manager.CaptureSaveData();
        data.outcome=ScenarioOneOutcome.Active;data.outcomeYear=-1;data.outcomeReason="";data.annualReviewSeen=true;
        data.workOrders=new List<ScenarioOneWorkOrder>{Order(1,10,ScenarioWorkType.FellTree),Order(2,10,ScenarioWorkType.PlantJuvenile,"sessile-oak"),Order(3,10,ScenarioWorkType.PlantJuvenile,"beech")};
        data.managementEvents=data.workOrders.Select((o,i)=>Work(i+1,o)).ToList();
        if (review) data.managementEvents.Add(new ScenarioManagementEvent {eventId=4,year=10,eventType=ScenarioManagementEventType.AnnualReviewAcknowledged});
        if (second) {var order=Order(4,25,ScenarioWorkType.PruneTree);data.workOrders.Add(order);data.managementEvents.Add(Work(5,order));}
        data.nextManagementEventId=6;
        data.annualReports=new List<ScenarioAnnualReport>{new ScenarioAnnualReport{year=10},new ScenarioAnnualReport{year=25}};
        ScenarioOneDefinition d=manager.Definition;int geometry=saves.CaptureData().standGeometryModel;
        var snapshot=new ScenarioEcologicalSnapshot {year=d.MinimumCompletionYear,meanCanopy=1f,
            occupiedRegenerationCells=d.EffectiveMinimumRegenerationCells(geometry),deadwoodVolumeM3=d.EffectiveMinimumDeadwoodVolumeM3(geometry),
            species=new List<ScenarioSpeciesOutcome>{new ScenarioSpeciesOutcome{speciesId="sitka-spruce",livingTrees=d.EffectiveMinimumRetainedOriginalTrees(geometry)},
                new ScenarioSpeciesOutcome{speciesId="sessile-oak",plantedJuveniles=1},new ScenarioSpeciesOutcome{speciesId="beech",plantedJuveniles=1}}};
        data.ecologicalSnapshots=new List<ScenarioEcologicalSnapshot>{snapshot};return data;
    }
    private void RestoreAndRoundTrip(ScenarioOneSaveData data,string label)
    {
        manager.RestoreSaveData(data,20);
        ForestSaveData full=saves.CaptureData();
        // Current-year handler/load checks need a consistent world clock; fixtures do not advance ecology.
        full.ecologicalYear=25;
        ForestSaveData roundTrip=JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(full));
        Check(saves.LoadData(roundTrip,false),label+" real JsonUtility/CaptureData/LoadData");
        Check(manager.ManagementEvents.Count==data.managementEvents.Count,label+" event count preserved");
    }
    private void Verify()
    {
        manager=FindFirstObjectByType<ScenarioOneManager>();saves=FindFirstObjectByType<ForestSaveController>();
        Check(manager!=null&&saves!=null,"scene systems available");original=saves.CaptureData();
        var shallow=Fixture(false,false);RestoreAndRoundTrip(shallow,"after first");
        Check(manager.Objectives.Count==8&&manager.Objectives.All(o=>o.achieved),"unchanged eight objectives satisfied by isolated decision fixture");
        Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Active,"old shallow path fails completion");
        Check(manager.NeedsCompletionReviewAcknowledgement,"legacy true boolean exposes fresh acknowledgement");
        manager.MarkAnnualReviewSeen();var ack=manager.ManagementEvents.Last();
        Check(ack.eventType==ScenarioManagementEventType.AnnualReviewAcknowledged&&ack.year==25&&ack.eventId==6,"real handler current year plus monotonic ID");
        int count=manager.ManagementEvents.Count;manager.MarkAnnualReviewSeen();Check(manager.ManagementEvents.Count==count,"duplicate click idempotent");
        var afterReview=Fixture(false,true);RestoreAndRoundTrip(afterReview,"after review");Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Active,"review without second incomplete");
        var pending=Fixture(true,true);pending.workOrders.Last().status=ScenarioWorkStatus.Approved;pending.managementEvents.RemoveAt(pending.managementEvents.Count-1);
        RestoreAndRoundTrip(pending,"before second");Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Active,"approval-only boundary incomplete");
        var noReview=Fixture(true,false);RestoreAndRoundTrip(noReview,"no review");Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Active,"legacy boolean alone cannot qualify");
        var sameYear=Fixture(true,true);sameYear.workOrders.Last().resolvedYear=10;sameYear.managementEvents.Last().year=10;
        RestoreAndRoundTrip(sameYear,"same year");Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Active,"same-year plans fail");
        var failed=Fixture(true,true);failed.workOrders.Last().status=ScenarioWorkStatus.Failed;failed.managementEvents.Last().outcome=ScenarioManagementOutcome.Failed;
        RestoreAndRoundTrip(failed,"failed work");Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Active,"failed-only second fails");
        var valid=Fixture(true,true);RestoreAndRoundTrip(valid,"second complete");Evaluate();Check(manager.Outcome==ScenarioOneOutcome.Completed,"valid non-prescriptive second pruning completes");
        foreach (ScenarioOneOutcome terminal in new[]{ScenarioOneOutcome.Completed,ScenarioOneOutcome.Failed})
        {
            var legacy=Fixture(false,false);legacy.outcome=terminal;legacy.outcomeYear=25;legacy.outcomeReason="legacy terminal";
            RestoreAndRoundTrip(legacy,"old "+terminal);Evaluate();Check(manager.Outcome==terminal&&manager.CaptureSaveData().outcomeReason=="legacy terminal","legacy terminal preserved "+terminal);
        }
        Check(ForestSaveData.CurrentVersion==20&&(int)ScenarioManagementEventType.AnnualReviewAcknowledged==9,"v20 enum extension");
    }
}
#endif

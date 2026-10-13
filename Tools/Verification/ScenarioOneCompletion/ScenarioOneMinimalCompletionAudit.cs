#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Disposable audit: normal public gameplay and UI actions only. No forced outcome,
// ecology mutation, private API, model override, or fabricated save data.
public static class ScenarioOneMinimalCompletionAudit
{
    public static void Begin() { EditorPrefs.SetBool("MinimalCompletion.Run", true); EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install() { if (!EditorPrefs.GetBool("MinimalCompletion.Run", false)) return; EditorPrefs.SetBool("MinimalCompletion.Run", false); new GameObject("Minimal completion audit").AddComponent<MinimalCompletionGate>(); }
}
public sealed class MinimalCompletionGate : MonoBehaviour
{
    ScenarioOneManager m; ScenarioOneUiRoot ui; ForestSaveController saves; ForestEcologyController ecology; ForestTreeMarkingManager marks;
    int acknowledgements;
    void Check(bool ok, string message) { Debug.Log("MINIMAL_CHECK " + ok + " " + message); if (!ok) throw new InvalidOperationException(message + " feedback=" + m?.Feedback); }
    ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving && t.TreeId.StartsWith("P", StringComparison.Ordinal)).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
    void Record(string phase) { Debug.Log("MINIMAL_STATE " + phase + " year=" + m.CurrentEcologicalYear + " outcome=" + m.Outcome + " cash=" + m.CashCents + " interventions=" + m.Interventions.Count + " acknowledgements=" + acknowledgements); foreach(var o in m.Objectives) Debug.Log("MINIMAL_OBJECTIVE " + phase + " " + o.objectiveId + " " + o.currentValue.ToString("R",System.Globalization.CultureInfo.InvariantCulture) + "/" + o.targetValue.ToString("R",System.Globalization.CultureInfo.InvariantCulture) + " " + o.achieved); Debug.Log("MINIMAL_SNAPSHOT " + phase + " " + JsonUtility.ToJson(m.EcologicalSnapshots.Last())); }
    void Advance() { if (ui.NeedsAnnualReview) { Check(!ui.AdvanceFromWorkPlan(), "normal UI blocks advance before review"); ui.AcknowledgeAnnualReview(); acknowledgements++; } ui.ShowWorkPlan(); Check(ui.AdvanceFromWorkPlan(), "normal UI annual advance"); Record("annual"); }
    ScenarioOneWorkOrder Fell() { var t=Trees().First(); m.PlanningFellingOutcome=FellingMaterialOutcome.RetainAsFallenDeadwood; marks.Mark(t,TreeMarkType.Fell,false); m.AddMarkedTreesToWorkPlan(); return m.WorkOrders.Single(o=>o.IsOpen && o.targetTreeId==t.TreeId && o.type==ScenarioWorkType.FellTree); }
    void Plant(string item) { Check(m.TryPurchaseStock(item,1),"buy exactly one " + item); foreach(var c in ecology.Cells.OrderByDescending(c=>c.Light).ThenBy(c=>c.Center.x).ThenBy(c=>c.Center.y)) foreach(float x in new[]{-1.5f,0f,1.5f}) foreach(float z in new[]{-1.5f,0f,1.5f}) { var p=new Vector3(c.Center.x+x,0,c.Center.y+z); if(m.TryDesignateExactPlanting(item,p,WorkExecutionMethod.LandownerSimulated,true)) { Debug.Log("MINIMAL_PLANT " + item + " " + p); return; } } throw new Exception("No legal planting point"); }
    string History() => string.Join("|",m.Interventions.Select(i=>$"{i.Ordinal}:{i.Year}:{i.Felled}:{i.Planted}:{i.TasksFailed}:{i.CostCents}:{i.RevenueCents}"));
    IEnumerator RoundTrip(string phase) { string h=History(); int events=m.ManagementEvents.Count, year=m.CurrentEcologicalYear, outcomeYear=m.OutcomeYear; var outcome=m.Outcome; string hash=ScenarioReferenceArchive.WorldHash(saves.CaptureData()); saves.Save(); Check(saves.CanLoad(out string problem),"normal disk save validates " + problem); saves.Load(); yield return null; yield return null; Check(History()==h && m.ManagementEvents.Count==events && m.CurrentEcologicalYear==year && m.Outcome==outcome && m.OutcomeYear==outcomeYear,"normal disk load preserves history and outcome " + phase); Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData())==hash,"normal disk load preserves world " + phase); }
    IEnumerator Start() { yield return null; yield return null; yield return null; m=FindFirstObjectByType<ScenarioOneManager>(); ui=FindFirstObjectByType<ScenarioOneUiRoot>(); saves=FindFirstObjectByType<ForestSaveController>(); ecology=FindFirstObjectByType<ForestEcologyController>(); marks=FindFirstObjectByType<ForestTreeMarkingManager>(); var stack=new Stack<IEnumerator>(); stack.Push(Run()); Exception failure=null; while(stack.Count>0) { bool more; object current=null; try { more=stack.Peek().MoveNext(); if(more)current=stack.Peek().Current; } catch(Exception ex){failure=ex;break;} if(!more){stack.Pop();continue;} if(current is IEnumerator nested)stack.Push(nested);else yield return current; } if(failure==null)Debug.Log("MINIMAL_AUDIT_PASS"); else Debug.LogError("MINIMAL_AUDIT_FAIL " + failure); EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure==null?0:1); }
    IEnumerator Run() {
        Check(m.CurrentEcologicalYear==0 && m.Interventions.Count==0,"untouched normal new game"); Record("baseline");
        bool early=Environment.GetEnvironmentVariable("CCF_MINIMAL_ROUTE")=="early";
        if(!early) { while(m.CurrentEcologicalYear<24){Advance();yield return null;if(m.CurrentEcologicalYear==10)yield return RoundTrip("year10");} }
        Fell(); Record("marked-pending"); Plant("sessile-oak-sapling"); Plant("beech-sapling"); Record("planted-pending"); Check(m.ApprovePendingWork(),"approve one combined plan"); Record("approved-unresolved"); Advance(); yield return null;
        while(m.CurrentEcologicalYear<25){Advance();yield return null;if(m.CurrentEcologicalYear==10)yield return RoundTrip("year10");}
        Record("candidate-completion");
        if(m.CompletionReview!=null) { var c=m.CompletionReview; Debug.Log($"MINIMAL_COMPLETION year={c.Year} plans={c.Interventions.Count} felled={c.TotalFelled} planted={c.TotalPlanted} pruned={c.TotalPruned} cost={c.TotalCostCents} revenue={c.TotalRevenueCents} startingCash={c.StartingCashCents} closingCash={c.ClosingCashCents} otherDeaths={c.TotalOtherDeaths}"); foreach(var line in ManagementHistoryText.Completion(c,new HistoryTextContext()))Debug.Log("MINIMAL_SUMMARY " + line); }
        yield return RoundTrip("year25");
        // Capability probes happen after the completion candidate and cannot contribute to it.
        Fell(); Check(m.ApprovePendingWork(),"second intervention approval"); Advance();yield return null;Check(m.Interventions.Count==2,"second intervention resolves");
        var stale=Fell(); int id=stale.workOrderId; Advance();yield return null;Advance();yield return null;ui.ShowWorkPlan();Check(m.WorkOrders.Any(o=>o.workOrderId==id && o.IsOpen) && m.StaleOrders.Any(n=>n.Order.workOrderId==id && n.AgeYears>=2),"pending order survives two years and is surfaced stale");
        Check(m.ApprovePendingWork(),"third intervention approval of stale order");Advance();yield return null;Check(m.Interventions.Count==3,"third intervention resolves");Record("repeated-capability");yield return RoundTrip("after-third");
    }
}
#endif

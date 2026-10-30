using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class N3OfflineCheck
{
    private static int checks;
    private static void Check(bool condition,string label) { if (!condition) throw new Exception("FAIL "+label);checks++;Console.WriteLine("PASS "+label); }
    private static ScenarioManagementEvent Work(int id,int year,int orderId,ScenarioManagementOutcome result=ScenarioManagementOutcome.Succeeded)
        => new ScenarioManagementEvent {eventId=id,year=year,workOrderId=orderId,eventType=ScenarioManagementEventType.WorkResolved,
            outcome=result,taskType=ScenarioWorkType.FellTree};
    private static ScenarioOneWorkOrder Order(int id,int year,ScenarioWorkStatus status=ScenarioWorkStatus.Completed)
        => new ScenarioOneWorkOrder {workOrderId=id,resolvedYear=year,type=ScenarioWorkType.FellTree,status=status};
    private static ScenarioManagementEvent Review(int id,int year) => new ScenarioManagementEvent {eventId=id,year=year,eventType=ScenarioManagementEventType.AnnualReviewAcknowledged};
    private static bool Chronology(List<ScenarioManagementEvent> e,List<ScenarioOneWorkOrder> o) => ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(e,o);
    private static ForestSaveData Save(List<ScenarioManagementEvent> events,List<ScenarioOneWorkOrder> orders,bool seen=true)
        => new ForestSaveData { ecologicalYear=25,growthModel=1,regenerationModel=1,stormModel=0,standGeometryModel=0,
            trees=new List<TreeSaveData>{new TreeSaveData {treeId="P1",stage=0}},
            scenarioOne=new ScenarioOneSaveData {initialized=true,annualReviewSeen=seen,managementEvents=events,workOrders=orders,
                annualReports=new List<ScenarioAnnualReport>{new ScenarioAnnualReport {year=10},new ScenarioAnnualReport {year=25}},
                nextManagementEventId=events.Select(e=>e.eventId).DefaultIfEmpty(0).Max()+1} };
    private static ForestSaveData RoundTrip(ForestSaveData save,string label)
    {
        string json=JsonUtility.ToJson(save);ForestSaveData restored=JsonUtility.FromJson<ForestSaveData>(json);
        Check(ForestSaveValidation.Validate(restored,1,64)==null,label+" typed production validator");
        Check(ForestSaveValidation.ValidateGeometryJson(json,restored)==null,label+" raw geometry");
        Check(ForestSaveValidation.ValidateStormJson(json,restored)==null,label+" raw storm");
        Check(ForestSaveValidation.ValidateCompetitionJson(json,restored)==null,label+" raw competitor");
        Check(json==JsonUtility.ToJson(restored),label+" deterministic field roundtrip");return restored;
    }
    public static int Main()
    {
        var orders=new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25)};
        var one=new List<ScenarioManagementEvent>{Work(1,10,1)};
        Check(!Chronology(one,orders),"old shallow path no second plan");
        var valid=new List<ScenarioManagementEvent>{Work(1,10,1),Review(2,10),Work(3,25,2)};
        Check(Chronology(valid,orders),"valid two cycle same-year acknowledgement after first");
        Check(!Chronology(valid.Select(e=>new ScenarioManagementEvent {eventId=e.eventId,year=10,workOrderId=e.workOrderId,eventType=e.eventType,outcome=e.outcome,taskType=e.taskType}).ToList(),new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,10)}),"same-year plans fail");
        Check(!Chronology(new List<ScenarioManagementEvent>{Work(1,10,1),Work(2,25,2)},orders),"no intervening review");
        Check(!Chronology(new List<ScenarioManagementEvent>{Review(1,10),Work(2,10,1),Work(3,25,2)},orders),"review before first same year");
        Check(!Chronology(new List<ScenarioManagementEvent>{Work(1,10,1),Work(2,25,2),Review(3,25)},orders),"review after second does not retroactively count");
        Check(!Chronology(new List<ScenarioManagementEvent>{Work(1,10,1),Review(2,10),Work(3,25,2,ScenarioManagementOutcome.Failed)},new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25,ScenarioWorkStatus.Failed)}),"failed-only second plan");
        Check(!Chronology(valid,new List<ScenarioOneWorkOrder>{Order(1,10)}),"missing completed order cannot fabricate second plan");
        Check(!Chronology(valid,new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25,ScenarioWorkStatus.Approved)}),"approval alone fails");
        Check(!Chronology(new List<ScenarioManagementEvent>(),orders),"empty work fails");
        Check(!Chronology(new List<ScenarioManagementEvent>{Work(1,10,1),Review(1,10),Work(3,25,2)},orders),"duplicate IDs ambiguous");
        var mixed=new List<ScenarioManagementEvent>{Work(1,10,1),Review(2,10),Work(3,10,3,ScenarioManagementOutcome.Failed),Work(4,25,2)};
        Check(!Chronology(mixed,orders),"review inside first plan before its final failed resolution cannot count");
        mixed[1]=Work(2,10,3,ScenarioManagementOutcome.Failed);mixed[2]=Review(3,10);
        Check(Chronology(mixed,orders),"mixed-success plan counts after entire plan resolved");
        var nonPrescriptive=valid.Select(e=>JsonUtility.FromJson<ScenarioManagementEvent>(JsonUtility.ToJson(e))).ToList();
        var alternateOrders=new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25)};
        alternateOrders[1].type=ScenarioWorkType.PruneTree;nonPrescriptive[2].taskType=ScenarioWorkType.PruneTree;
        Check(Chronology(nonPrescriptive,alternateOrders),"second task non-prescriptive pruning");
        var afterFirst=RoundTrip(Save(one,new List<ScenarioOneWorkOrder>{Order(1,10)},false),"after first");
        Check(!Chronology(afterFirst.scenarioOne.managementEvents,afterFirst.scenarioOne.workOrders),"restored first incomplete");
        var afterReview=RoundTrip(Save(valid.Take(2).ToList(),new List<ScenarioOneWorkOrder>{Order(1,10)}),"after review");
        Check(!Chronology(afterReview.scenarioOne.managementEvents,afterReview.scenarioOne.workOrders),"restored review still requires second");
        var beforeSecond=Save(valid.Take(2).ToList(),new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25,ScenarioWorkStatus.Approved)});
        Check(!Chronology(RoundTrip(beforeSecond,"before second").scenarioOne.managementEvents,beforeSecond.scenarioOne.workOrders),"pending boundary incomplete");
        var completed=RoundTrip(Save(valid,orders),"after second");Check(Chronology(completed.scenarioOne.managementEvents,completed.scenarioOne.workOrders),"restored second qualifies");
        var legacy=RoundTrip(Save(new List<ScenarioManagementEvent>{Work(1,10,1),Work(2,25,2)},orders),"legacy true boolean");
        Check(legacy.scenarioOne.annualReviewSeen&&!Chronology(legacy.scenarioOne.managementEvents,legacy.scenarioOne.workOrders),"legacy boolean alone fails");
        Check(ScenarioOneCompletionRules.NeedsReviewAcknowledgement(legacy.scenarioOne.managementEvents,legacy.scenarioOne.workOrders),"legacy fresh acknowledgement available");
        var bridge=new ManagerBoundaryBridge {CurrentYear=25,annualReports=legacy.scenarioOne.annualReports,managementEvents=legacy.scenarioOne.managementEvents,nextManagementEventId=3,annualReviewSeen=true};
        bridge.MarkAnnualReviewSeen();Check(bridge.managementEvents.Last().eventType==ScenarioManagementEventType.AnnualReviewAcknowledged&&bridge.managementEvents.Last().eventId==3&&bridge.managementEvents.Last().year==25,"actual manager handler current year monotonic ID");
        bridge.MarkAnnualReviewSeen();Check(bridge.managementEvents.Count==3,"actual handler duplicate click idempotent");
        Check(!Chronology(bridge.managementEvents,orders),"fresh late review cannot retroactively qualify");
        bridge.managementEvents.Add(Work(4,26,3));orders.Add(Order(3,26));Check(Chronology(bridge.managementEvents,orders),"legacy fresh review then future plan qualifies");
        var noReport=new ManagerBoundaryBridge {CurrentYear=0};noReport.MarkAnnualReviewSeen();Check(noReport.managementEvents.Count==0&&!noReport.annualReviewSeen,"no report cannot acknowledge");
        foreach (var terminal in new[]{ScenarioOneOutcome.Completed,ScenarioOneOutcome.Failed})
        {
            var old=Save(new List<ScenarioManagementEvent>(),new List<ScenarioOneWorkOrder>());old.scenarioOne.outcome=terminal;old.scenarioOne.outcomeYear=25;old.scenarioOne.outcomeReason="legacy terminal";
            old=RoundTrip(old,"legacy "+terminal);var restored=new ManagerBoundaryBridge();restored.RestoreTerminalAndCounter(old.scenarioOne);
            Check(restored.outcome==terminal&&restored.outcomeYear==25&&restored.outcomeReason=="legacy terminal","actual restore assignments preserve "+terminal);
            Check(restored.CanComplete(true)==(terminal==ScenarioOneOutcome.Completed),"terminal stable "+terminal);
        }
        var active=new ManagerBoundaryBridge {outcome=ScenarioOneOutcome.Active,managementEvents=valid,workOrders=new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25)}};
        Check(active.CanComplete(true)&&!active.CanComplete(false),"actual completion predicate still requires all eight objectives");
        var invalid=Save(valid,orders);invalid.growthModel=99;Check(ForestSaveValidation.Validate(invalid,1,64)!=null,"growth-version refusal retained");
        Check((int)ScenarioManagementEventType.AnnualReviewAcknowledged==9&&ForestSaveData.CurrentVersion==24,"enum9 schema24");
        // ---- completion guidance: presentation only. It reads the same chronology and decides, records and changes nothing. ----
        var two=new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25)};
        CompletionGuidanceState G(List<ScenarioManagementEvent> e,List<ScenarioOneWorkOrder> o,bool met)=>ScenarioOneCompletionGuidance.GuidanceState(e,o,met);
        var planOnly=new List<ScenarioManagementEvent>{Work(1,10,1)};
        var planThenReview=new List<ScenarioManagementEvent>{Work(1,10,1),Review(2,10)};
        var firstPlan=new List<ScenarioOneWorkOrder>{Order(1,10)};
        Check(G(new List<ScenarioManagementEvent>(),new List<ScenarioOneWorkOrder>(),true)==CompletionGuidanceState.None,"guidance none before any qualifying plan");
        Check(G(planOnly,firstPlan,false)==CompletionGuidanceState.AcknowledgeReview&&G(planOnly,firstPlan,true)==CompletionGuidanceState.AcknowledgeReview,"guidance state 2 acknowledge: first plan, no acknowledgement, objectives met or not");
        Check(G(new List<ScenarioManagementEvent>{Work(1,10,1),Work(2,25,2)},two,true)==CompletionGuidanceState.AcknowledgeReview,"guidance state 2 for two plans with no review at all (legacy boolean is not read)");
        Check(G(new List<ScenarioManagementEvent>{Review(1,10),Work(2,10,1),Work(3,25,2)},two,true)==CompletionGuidanceState.AcknowledgeReview,"guidance state 2 when the only review came before the first plan");
        Check(G(planThenReview,firstPlan,true)==CompletionGuidanceState.ObjectivesMetOneCycle,"guidance state 1: objectives met, one reviewed plan");
        Check(G(planThenReview,firstPlan,false)==CompletionGuidanceState.AnotherIntervention,"guidance state 3: reviewed first plan, objectives not yet met");
        Check(G(new List<ScenarioManagementEvent>{Work(1,10,1),Work(2,25,2),Review(3,25)},two,true)==CompletionGuidanceState.AnotherIntervention,"guidance state 3 (not state 1) after a late review of two plans");
        Check(G(new List<ScenarioManagementEvent>{Work(1,10,1),Review(2,10),Work(3,25,2,ScenarioManagementOutcome.Failed)},new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25,ScenarioWorkStatus.Failed)},true)==CompletionGuidanceState.ObjectivesMetOneCycle,"guidance: a failed-only second plan is still one qualifying plan");
        Check(G(planThenReview,new List<ScenarioOneWorkOrder>{Order(1,10),Order(2,25,ScenarioWorkStatus.Approved)},true)==CompletionGuidanceState.ObjectivesMetOneCycle,"guidance: an approved but unresolved second plan does not count");
        Check(G(new List<ScenarioManagementEvent>{Work(1,10,1),Review(2,10),Work(3,10,3,ScenarioManagementOutcome.Failed),Work(4,25,2)},two,true)==CompletionGuidanceState.AcknowledgeReview,"guidance: a review inside the first plan does not count");
        Check(G(valid,two,true)==CompletionGuidanceState.None&&G(valid,two,false)==CompletionGuidanceState.None,"guidance none once the reviewed two-cycle sequence exists");
        Check(G(valid,two,true)==CompletionGuidanceState.None&&Chronology(valid,two),"guidance is silent exactly where the completion rule is satisfied");
        Check(!Chronology(planOnly,firstPlan)&&G(planOnly,firstPlan,true)!=CompletionGuidanceState.None&&!Chronology(planThenReview,firstPlan)&&G(planThenReview,firstPlan,true)!=CompletionGuidanceState.None,"guidance speaks exactly where the completion rule is not satisfied");
        var legacyBridge=new ManagerBoundaryBridge {CurrentYear=25,annualReports=legacy.scenarioOne.annualReports,managementEvents=new List<ScenarioManagementEvent>{Work(1,10,1),Work(2,25,2)},nextManagementEventId=3,annualReviewSeen=true};
        Check(G(legacyBridge.managementEvents,two,true)==CompletionGuidanceState.AcknowledgeReview,"guidance: legacy Active save asks for the acknowledgement");
        legacyBridge.MarkAnnualReviewSeen();
        Check(G(legacyBridge.managementEvents,two,true)==CompletionGuidanceState.AnotherIntervention&&!Chronology(legacyBridge.managementEvents,two),"guidance moves on after the real handler records the acknowledgement; completion is unchanged");
        string[] texts={ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.AcknowledgeReview),ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.ObjectivesMetOneCycle),ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.AnotherIntervention)};
        Check(texts.All(t=>t.Length>0)&&texts.Distinct().Count()==3&&ScenarioOneCompletionGuidance.GuidanceText(CompletionGuidanceState.None)=="","guidance text exists for exactly the three states and they differ");
        Check(texts[0].Contains("Annual Review")&&texts[0].Contains("I've read the annual results"),"state 2 text names the Annual Review and its button");
        Check(texts[1].Contains("All eight objectives")&&texts[1].Contains("Another management cycle")&&texts[1].Contains("different simulation year")&&texts[1].Contains("another intervention"),"state 1 text: objectives met, review, another decision");
        Check(texts[2].Contains("Return to the forest")&&texts[2].Contains("another intervention"),"state 3 text: return to the forest, another intervention");
        Check(texts.All(t=>new[]{"fell","prun","plant","thin","oak","beech","sitka","deadwood"}.All(w=>t.IndexOf(w,StringComparison.OrdinalIgnoreCase)<0)),"guidance never prescribes the second intervention");
        Check(texts.All(t=>t.IndexOf("auto",StringComparison.OrdinalIgnoreCase)<0),"guidance does not promise automatic acknowledgement");
        Console.WriteLine("N3_OFFLINE_PASS checks="+checks+" NOT_UNITY_VERIFIED");return 0;
    }
}

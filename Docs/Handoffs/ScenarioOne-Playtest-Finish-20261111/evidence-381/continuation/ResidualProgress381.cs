#if UNITY_EDITOR
using System; using System.Linq; using System.Collections; using System.IO; using UnityEngine;
public static class ResidualProgress381 {
static void Need(bool b,string s){if(!b)throw new Exception(s);Debug.Log("RESIDUAL_PROGRESS_PASS "+s);}
public static IEnumerator Run(ScenarioOneUiRoot ui){
var manager=ui.Manager;var marks=ui.Marking;var ecology=UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
Func<ForestTree[]> trees=()=>UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
Func<string,ForestTree> tree=id=>trees().First(t=>t.TreeId==id);
float half=StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion)*.5f;
foreach(var id in new[]{"PO0203","PO1204","PO2003","PO2803","PO3803"}){var t=tree(id);Need(manager.AddRack(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX,t.transform.position.z),new RackPoint(half,t.transform.position.z)).Succeeded,"rack added");}
Need(manager.ConfirmRackPlan().Succeeded,"racks confirmed");foreach(var rack in manager.RackPlan.ToList())Need(manager.MarkRackEntrance(rack.Id).Succeeded,"entrance marked");
Need(ui.ApproveWorkPlan(),"rack Work Plan approved");Need(ui.AdvanceFromWorkPlan(),"rack year advanced");manager.MarkAnnualReviewSeen();
string[] crops={"P0810","P1206","P1214"};foreach(var id in crops)marks.Mark(tree(id),TreeMarkType.CropTree,false);
Func<ScenarioOneRackBay.Analysis> bay=()=>ScenarioOneRackBayReader.Read(trees(),manager.WorkOrders,ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks,half));
for(int i=0;i<150&&bay().MinPlannedAndExecuted<ScenarioOneRackBay.FirstReleaseShare;i++){
var least=bay().Future.OrderBy(f=>f.ReleaseShare).First();var ct=tree(least.Id);
var next=trees().Where(t=>t.IsLiving&&!t.IsStump&&t.MarkType==TreeMarkType.None&&!crops.Contains(t.TreeId)&&Vector3.Distance(t.transform.position,ct.transform.position)<8).OrderBy(t=>Vector3.Distance(t.transform.position,ct.transform.position)).First();
marks.Mark(next,TreeMarkType.Fell,false);manager.AddMarkedTreesToWorkPlan();}
Need(bay().ReleasePlanned&&bay().CanopyKept,"release planned canopy kept");var order=manager.WorkOrders.First(o=>o.type==ScenarioWorkType.FellTree&&o.status==ScenarioWorkStatus.Pending);manager.SetPendingFellingOutcome(order.workOrderId,FellingMaterialOutcome.RetainAsFallenDeadwood);
Need(ui.ApproveWorkPlan(),"sample Work Plan approved");Need(ui.AdvanceFromWorkPlan(),"sample year advanced");manager.MarkAnnualReviewSeen();Need(bay().ReleaseExecuted,"sample release executed");
ui.CloseAll();ui.ShowMap();for(int i=0;i<20;i++)yield return null;
}
}
#endif

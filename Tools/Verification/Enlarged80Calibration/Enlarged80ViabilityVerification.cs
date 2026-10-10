#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CCF.Forestry.WorkEconomy;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class Enlarged80ViabilityVerification
{
    public static void Begin(){EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");EditorApplication.isPlaying=true;}
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install(){if(Environment.GetCommandLineArgs().Contains("Enlarged80ViabilityVerification.Begin"))new GameObject("E80 viability").AddComponent<Enlarged80ViabilityRunner>();}
}
public sealed class Enlarged80ViabilityRunner : MonoBehaviour
{
    ScenarioOneManager manager; ForestEcologyController ecology; ForestTreeMarkingManager marking;
    long minimumCash;
    static void Check(bool ok,string why){if(!ok)throw new InvalidOperationException(why);}
    IEnumerator Start()
    {
        for(int i=0;i<5;i++)yield return null;
        var stack=new Stack<IEnumerator>();stack.Push(Run());Exception failure=null;
        while(stack.Count>0){bool more;object current=null;try{more=stack.Peek().MoveNext();if(more)current=stack.Peek().Current;}catch(Exception e){failure=e;break;}if(!more){stack.Pop();continue;}if(current is IEnumerator nested){stack.Push(nested);continue;}yield return current;}
        Debug.Log(failure==null?"E80_VIABILITY_VERIFY_PASS":"E80_VIABILITY_VERIFY_FAIL "+failure);EditorApplication.Exit(failure==null?0:1);
    }
    List<ForestTree> Living()=>FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving&&t.Species.SpeciesId=="sitka-spruce").OrderBy(t=>t.TreeId,StringComparer.Ordinal).ToList();
    IEnumerator Run()
    {
        manager=FindFirstObjectByType<ScenarioOneManager>();ecology=FindFirstObjectByType<ForestEcologyController>();marking=FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(ecology.StandGeometryModelVersion==StandGeometryModel.Enlarged80 && Living().Count==1344,"fresh Enlarged80");minimumCash=manager.CashCents;
        // Bounded economy probes: quotes only, no simulation or price edits.
        var probe=Living().First();marking.Mark(probe,TreeMarkType.Fell,false);manager.AddMarkedTreesToWorkPlan();var small=manager.GetHarvestQuote();
        Check(small.CostCents>=250000 && small.RevenueCents<small.CostCents,"small job minimum/loss");
        Debug.Log("E80_ECONOMY_SMALL cost="+small.CostCents+" revenue="+small.RevenueCents);
        // The first intervention includes this tree; pending orders are deduplicated normally.
        Thin(0.27f,true);
        var large=manager.GetHarvestQuote(true);Check(large.CostCents>=250000 && large.Resolution.Ledger.Count(x=>x.Category==LedgerCategory.MinimumJobAdjustment)<=1,"once per job");
        Check(large.CostCents/(double)large.Orders.Count<small.CostCents,"fixed minimum spread over trees");
        Debug.Log("E80_ECONOMY_LARGE trees="+large.Orders.Count+" cost="+large.CostCents+" revenue="+large.RevenueCents+" cash="+manager.CashCents);
        Advance(); yield return null;
        Check(manager.TryPurchaseStock("sessile-oak-sapling",8)&&manager.TryPurchaseStock("beech-sapling",8),"stock purchase"); minimumCash=Math.Min(minimumCash,manager.CashCents);
        foreach(string item in new[]{"sessile-oak-sapling","beech-sapling"})for(int n=0;n<8;n++)Check(Plant(item,n%2==0),"planting location "+item);
        Check(manager.ApprovePendingWork(),"plant approval "+manager.Feedback);Advance();yield return null;
        while(ecology.EcologicalYear<16){Advance();yield return null;}
        Thin(0.2f,false);
        while(ecology.EcologicalYear<30){Advance();yield return null;}
        var snapshot=manager.EcologicalSnapshots.Last();var events=manager.ManagementEvents.Where(x=>x.eventType==ScenarioManagementEventType.WorkResolved&&x.outcome==ScenarioManagementOutcome.Succeeded).ToList();
        Debug.Log("E80_VIABILITY_RESULT completion="+manager.Outcome+" completedYear="+manager.OutcomeYear+" minimumCash="+minimumCash+" interventionYears="+manager.AnnualReports.Count(x=>x.completedTasks>0)+" treesFelled="+events.Count(x=>x.taskType==ScenarioWorkType.FellTree)+" ownerMinutes="+manager.AnnualReports.Sum(x=>x.ownerMinutes)+" oak="+events.Count(x=>x.taskType==ScenarioWorkType.PlantJuvenile&&x.speciesId=="sessile-oak")+" beech="+events.Count(x=>x.taskType==ScenarioWorkType.PlantJuvenile&&x.speciesId=="beech")+" retained="+snapshot.species.First(x=>x.speciesId=="sitka-spruce").livingTrees+" regenCells="+snapshot.occupiedRegenerationCells+" deadwood="+snapshot.deadwoodVolumeM3+" canopy="+snapshot.meanCanopy);
        Check(manager.Outcome==ScenarioOneOutcome.Completed && manager.OutcomeYear<=manager.Definition.MinimumCompletionYear,"not viable by established Year-25 completion horizon: "+string.Join(";",manager.Objectives.Select(ScenarioOneUiFacts.ObjectiveLine)));
        Check(minimumCash>=0 && manager.AnnualReports.All(x=>x.ownerMinutes<=2400),"cash/owner constraint");
    }
    void Thin(float share,bool retain)
    {
        var trees=Living();var crop=SelectCropTrees(trees);foreach(string id in crop)marking.Mark(trees.First(x=>x.TreeId==id),TreeMarkType.CropTree,false);
        // Same proportional thinning strategy as the established Legacy40 gate.
        // Retain 16 stems instead of its four: the same deadwood allocation per property area.
        var fell=SelectThinning(trees,crop,share,out float removed);for(int i=0;i<fell.Count;i++){manager.PlanningFellingOutcome=retain&&i<16?FellingMaterialOutcome.RetainAsFallenDeadwood:FellingMaterialOutcome.SellAndExtract;marking.Mark(fell[i],TreeMarkType.Fell,false);manager.AddMarkedTreesToWorkPlan();}
        Check(manager.ApprovePendingWork(),"harvest approval "+manager.Feedback);
        Debug.Log("E80_VIABILITY_INTERVENTION year="+ecology.EcologicalYear+" trees="+fell.Count+" basalAreaShare="+removed);
    }
    bool Plant(string item,bool shelter)
    {
        foreach(int cell in Enumerable.Range(0,ecology.CellCount).OrderByDescending(i=>ecology.Cells[i].Light).ThenBy(i=>i)){Vector2 c=ecology.Cells[cell].Center;for(int x=-2;x<=2;x++)for(int z=-2;z<=2;z++)if(manager.TryDesignateExactPlanting(item,new Vector3(c.x+x*0.9f,0,c.y+z*0.9f),shelter?WorkExecutionMethod.LandownerSimulated:WorkExecutionMethod.Contractor,shelter))return true;}return false;
    }
    void Advance(){Check(manager.AdvanceYear(),"advance "+ecology.EcologicalYear+" "+manager.Feedback);minimumCash=Math.Min(minimumCash,manager.CashCents);}
    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);

    // Largest stem per 10 m block: the retained crop-tree framework.
    private static List<string> SelectCropTrees(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId)
        .OrderBy(id => id, StringComparer.Ordinal).ToList();

    // Competitor release: nearest non-crop neighbours of crop trees first, until
    // the basal-area fraction is reached.
    private static List<ForestTree> SelectThinning(List<ForestTree> trees, List<string> cropIds, float fraction, out float removed)
    {
        var crop = new HashSet<string>(cropIds);
        List<ForestTree> crops = trees.Where(t => crop.Contains(t.TreeId)).ToList();
        float total = trees.Sum(BasalArea), target = total * fraction, taken = 0f;
        var result = new List<ForestTree>();
        foreach (ForestTree t in trees.Where(t => !crop.Contains(t.TreeId) && t.CanChop)
                     .OrderBy(t => crops.Count == 0 ? 0f : crops.Min(c => Vector3.Distance(c.transform.position, t.transform.position)))
                     .ThenBy(t => t.TreeId, StringComparer.Ordinal))
        {
            if (taken >= target) break;
            result.Add(t);
            taken += BasalArea(t);
        }
        removed = total > 0f ? taken / total : 0f;
        return result;
    }

 }
#endif

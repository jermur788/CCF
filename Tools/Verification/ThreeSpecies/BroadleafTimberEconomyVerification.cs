#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Disposable in-Editor gate: the Oak/Beech timber market inside Scenario One (D-062 and its supplement).
//
// STATUS: PARITY-06 strengthened gate; NOT YET RUN IN UNITY on the PARITY-06 candidate.
// The pure grading, price and valuation rules are verified offline (OfflineLogic/BroadleafTimberOfflineCheck); this gate
// covers what only Unity can: that the Scenario One adapter and manager use them, that a game on growth model 0/1 keeps
// its original economy, that Sitka quotes are identical under both, and that sales of the new products save and load.
//
// Copy into Assets/ForestPrototype, run with
//   CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -executeMethod BroadleafTimberEconomyVerification.Begin
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
public static class BroadleafTimberEconomyVerification
{
    public static void Begin()
    {
        EditorPrefs.SetBool("BroadleafTimberEconomyVerification.Run", true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool("BroadleafTimberEconomyVerification.Run", false)) return;
        EditorPrefs.SetBool("BroadleafTimberEconomyVerification.Run", false);
        new GameObject("Broadleaf timber economy gate").AddComponent<BroadleafTimberEconomyGate>();
    }
}

public sealed class BroadleafTimberEconomyGate : MonoBehaviour
{
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marks;
    private ForestTreeSpawner spawner;
    private ForestSaveData original;
    private string originalHash;
    private int assertions;

    private void Check(bool ok, string what) { assertions++; Debug.Log($"BROADLEAF_TIMBER_{(ok ? "PASS" : "FAIL")} {what}"); if (!ok) throw new InvalidOperationException(what); }
    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));

    private IEnumerator Start()
    {
        yield return null;
        manager = FindFirstObjectByType<ScenarioOneManager>(); saves = FindFirstObjectByType<ForestSaveController>();
        ecology = FindFirstObjectByType<ForestEcologyController>(); marks = FindFirstObjectByType<ForestTreeMarkingManager>(); spawner = FindFirstObjectByType<ForestTreeSpawner>();
        original = saves.CaptureData(); originalHash = ScenarioReferenceArchive.WorldHash(original);
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        bool restored = saves.LoadData(original, false);
        yield return null; yield return null;
        if (!restored || ScenarioReferenceArchive.WorldHash(saves.CaptureData()) != originalHash)
            failure = failure ?? new InvalidOperationException("Gate failed to restore the original world.");
        if (failure == null) Debug.Log("BROADLEAF_TIMBER_VERIFY_PASS assertions=" + assertions);
        else Debug.LogError("BROADLEAF_TIMBER_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Reset(int growthModel)
    {
        Check(saves.LoadData(Clone(original), false), "restore baseline");
        yield return null; yield return null;
        manager.InitializeNewScenario();
        ecology.GrowthModelVersion = growthModel;
    }

    private ScenarioOneWorkOrder Mark(ForestTree tree)
    {
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract; marks.Mark(tree, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        return manager.WorkOrders.Single(order => order.type == ScenarioWorkType.FellTree && order.targetTreeId == tree.TreeId && order.IsOpen);
    }

    // Synthetic large stems so the upper classes are testable; the simulation reaches them rarely (see Phase 6 record).
    private (ForestTree oak, ForestTree beech, ForestTree sitka) Spawn()
    {
        var oak = spawner.Spawn("ECO-OAK", spawner.ResolveSpecies("sessile-oak"), new Vector3(1f, 0f, 1f), 110, 60f, 24f, 5f);
        var beech = spawner.Spawn("ECO-BEECH", spawner.ResolveSpecies("beech"), new Vector3(-1f, 0f, 1f), 110, 60f, 24f, 5f);
        var sitka = spawner.Spawn("ECO-SITKA", spawner.DefaultSpecies, new Vector3(1f, 0f, -1f), 60, 50f, 28f, 4f);
        return (oak, beech, sitka);
    }

    private static long Revenue(ScenarioHarvestJob job, string treeId) => job.Resolution.Quote.Timber.Where(v => v.Batch.SourceTreeId == treeId).Sum(v => v.SaleRevenueCents);
    private static IEnumerable<TimberAssortment> Products(ScenarioHarvestJob job, string treeId) => job.Resolution.Quote.Timber.Where(v => v.Batch.SourceTreeId == treeId).Select(v => v.Batch.Assortment).Distinct();

    private IEnumerator Verify()
    {
        Check(ForestSaveData.CurrentVersion == 23, "save version unchanged");
        Check(ScenarioOneManager.NewGameGrowthModel == GrowthModel.SpeciesAware, "broadleaf market is on for new games: growth model 2 is the new-game default");
        yield return VerifyPruning();

        // ---- growth model 1: original economy ----
        yield return Reset(GrowthModel.SiteClassDensity);
        var t = Spawn(); Mark(t.oak); Mark(t.beech); Mark(t.sitka);
        Check(!manager.BroadleafMarketActive, "model 1: market off");
        ScenarioHarvestJob old = manager.GetHarvestQuote();
        Check(old.HasUnmarketedSpecies && Revenue(old, t.oak.TreeId) == 0 && Revenue(old, t.beech.TreeId) == 0 && Revenue(old, t.sitka.TreeId) > 0, "model 1: Oak and Beech unmarketed, Sitka sells");
        long sitkaRevenueModel1 = Revenue(old, t.sitka.TreeId), costModel1 = old.CostCents;

        // ---- growth model 2: Oak/Beech market ----
        yield return Reset(GrowthModel.SpeciesAware);
        t = Spawn(); Mark(t.oak); Mark(t.beech); Mark(t.sitka);
        Check(manager.BroadleafMarketActive, "model 2: market on");
        ScenarioHarvestJob mkt = manager.GetHarvestQuote();
        Check(!mkt.HasUnmarketedSpecies && mkt.BroadleafMarket && mkt.Eligible, "model 2: nothing unmarketed, quote eligible");
        Check(Revenue(mkt, t.oak.TreeId) > 0 && Revenue(mkt, t.beech.TreeId) > 0, "model 2: Oak and Beech earn revenue");
        var oakProducts = Products(mkt, t.oak.TreeId).ToList(); var beechProducts = Products(mkt, t.beech.TreeId).ToList();
        Check(oakProducts.Contains(TimberAssortment.Beam) && !oakProducts.Contains(TimberAssortment.VeneerPlanking), "unpruned large Oak: beam, no veneer/planking");
        Check(beechProducts.All(p => p == TimberAssortment.SecondPlanking || p == TimberAssortment.Firewood) && beechProducts.Contains(TimberAssortment.SecondPlanking), "Beech: second-quality planking and firewood only");
        Check(Revenue(mkt, t.sitka.TreeId) == sitkaRevenueModel1, "Sitka revenue identical under model 1 and model 2");
        Check(mkt.CostCents == costModel1, "harvest cost identical under model 1 and model 2 for the same orders (generic, no species multiplier)");
        Check(Math.Abs(mkt.TotalStemVolumeCm3 - old.TotalStemVolumeCm3) == 0, "same whole-stem work volume");
        var priceBefore = mkt.Resolution.Quote.Timber.GroupBy(v => v.Batch.SpeciesId + "/" + v.Batch.Assortment).ToDictionary(g => g.Key, g => g.First().SelectedRoadsideCentsPerTonne);

        // ---- pruning changes eligibility, not price ----
        Check(t.oak.TryPrune(3.5f, 0, true) == null && t.oak.TryPrune(6f, 3, true) == null, "two real broadleaf high-pruning lifts succeed");
        ScenarioHarvestJob pruned = manager.GetHarvestQuote();
        Check(!ReferenceEquals(pruned, mkt), "a pruning lift invalidates the cached quote");
        Check(Products(pruned, t.oak.TreeId).Contains(TimberAssortment.VeneerPlanking), "pruned large Oak: veneer/planking becomes eligible");
        var priceAfter = pruned.Resolution.Quote.Timber.GroupBy(v => v.Batch.SpeciesId + "/" + v.Batch.Assortment).ToDictionary(g => g.Key, g => g.First().SelectedRoadsideCentsPerTonne);
        Check(priceBefore.Where(p => priceAfter.ContainsKey(p.Key)).All(p => priceAfter[p.Key] == p.Value), "no product changes price because of pruning");
        Check(Revenue(pruned, t.oak.TreeId) > Revenue(mkt, t.oak.TreeId), "pruned Oak is worth more only through the products it can now reach");
        Check(Revenue(pruned, t.beech.TreeId) == Revenue(mkt, t.beech.TreeId) && Revenue(pruned, t.sitka.TreeId) == sitkaRevenueModel1, "pruning Oak does not move Beech or Sitka");

        // ---- settlement uses the same market ----
        ScenarioHarvestJob again = manager.GetHarvestQuote();
        long settled = ScenarioOneEconomyAdapter.ReResolveHarvest(again, manager.Definition, manager.CashCents).Quote.TimberSaleRevenueCents;
        Check(settled == again.RevenueCents, "re-resolution at settlement reproduces the quote");
        Check(manager.ApprovePendingWork(), "approve");
        long cash = manager.CashCents;
        Check(manager.AdvanceYear(), "annual resolution");
        Check(manager.CashCents - cash == ForestryWorkCalculator.SumLedger(again.Resolution.Ledger), "cash equals the job ledger");
        var sales = manager.AnnualReports.Last().timberSales;
        Check(sales.Any(s => s.assortment == TimberAssortment.VeneerPlanking) && sales.Any(s => s.assortment == TimberAssortment.SecondPlanking), "annual report records the new products");

        // ---- save / load of the new product records ----
        ForestSaveData data = saves.CaptureData();
        Check(ForestSaveValidation.Validate(data, 0, ecology.CellCount) == null, "save with Oak/Beech product sales validates");
        ForestSaveData roundTrip = Clone(data);
        Check(saves.LoadData(roundTrip, false), "save with Oak/Beech product sales loads");
        yield return null; yield return null;
        Check(manager.AnnualReports.Last().timberSales.Select(s => (int)s.assortment).SequenceEqual(sales.Select(s => (int)s.assortment)) && ecology.GrowthModelVersion == GrowthModel.SpeciesAware, "product sales and model survive a load");
        ForestSaveData bad = Clone(data); bad.scenarioOne.annualReports.Last().timberSales[0].assortment = (TimberAssortment)99;
        Check(ForestSaveValidation.Validate(bad, 0, ecology.CellCount) != null, "an undefined product is still rejected");
        Check(ForestSaveData.CurrentVersion == 23, "save version still 23");
    }

    private IEnumerator VerifyPruning()
    {
        // Both old growth models retain their configured three-lift schedule and five-year recovery.
        foreach (int model in new[] { GrowthModel.Legacy, GrowthModel.SiteClassDensity })
        {
            yield return Reset(model);
            var old = Spawn();
            foreach (ForestTree tree in new[] { old.oak, old.beech, old.sitka })
            {
                Check(manager.TryDesignatePruning(tree.TreeId), "legacy model pruning designation");
                Check(manager.WorkOrders.Single(o => o.targetTreeId == tree.TreeId && o.IsOpen).targetCrownBaseHeightM == 2.5f,
                    "legacy model first target remains 2.5 m");
                Check(tree.TryPrune(2.5f, 0) == null && tree.CanPrune(5f, 3) != null && tree.CanPrune(5f, 5) == null,
                    "legacy direct API retains five-year interval");
            }
        }

        yield return Reset(GrowthModel.SpeciesAware);
        var t = Spawn();
        t.oak.SetMark(TreeMarkType.CropTree); t.beech.SetMark(TreeMarkType.CropTree);
        Check(manager.EligibleCropTreePruningCount == 2, "model2 Oak/Beech eligible Crop Trees");
        Check(manager.BatchPruneCropTrees() == 2 && manager.WorkOrders.Where(o => o.IsOpen).All(o => o.targetCrownBaseHeightM == 3.5f),
            "batch chooses two 3.5 m first lifts");
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "first batch resolves through ordinary work");
        Check(t.oak.PruningLifts == 1 && t.beech.PruningLifts == 1 && t.oak.CrownBaseHeightM == 3.5f && t.beech.CrownBaseHeightM == 3.5f,
            "first batch changes authoritative pruning history");
        Check(manager.EligibleCropTreePruningCount == 0 && !manager.TryDesignatePruning(t.oak.TreeId), "no immediate repeat lift");
        Check(manager.AdvanceYear(), "wait first year");
        Check(!manager.TryDesignatePruning(t.oak.TreeId), "second lift cannot resolve only two years after first");
        Check(manager.AdvanceYear(), "wait second year");
        Check(manager.EligibleCropTreePruningCount == 2 && manager.BatchPruneCropTrees() == 2,
            "second lifts may resolve exactly three years after first");
        Check(manager.WorkOrders.Where(o => o.IsOpen).All(o => o.targetCrownBaseHeightM == 6f), "second target exactly 6 m");
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "second batch resolves");
        Check(t.oak.PruningLifts == 2 && t.beech.PruningLifts == 2 && t.oak.CrownBaseHeightM == 6f && t.beech.CrownBaseHeightM == 6f,
            "second batch retains authoritative high-pruning history");
        Check(manager.EligibleCropTreePruningCount == 0 && !manager.TryDesignatePruning(t.oak.TreeId) && !manager.TryDesignatePruning(t.beech.TreeId),
            "no third Oak/Beech lift through manager");
        ForestSaveData pruned = saves.CaptureData();
        Check(saves.LoadData(Clone(pruned), false), "pruned broadleaf save loads without schema change");
        yield return null; yield return null;
        var loaded = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(tree => tree.TreeId == "ECO-OAK" || tree.TreeId == "ECO-BEECH").ToArray();
        Check(loaded.Length == 2 && loaded.All(tree => tree.PruningLifts == 2 && tree.CrownBaseHeightM == 6f && tree.LastPruningYear == 4),
            "two-lift state and recovery year survive save/load");
    }
}
#endif

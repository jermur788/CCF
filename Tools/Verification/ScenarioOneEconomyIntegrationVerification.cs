#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Explicit disposable gate; memory captures only, never Save(), Load() or file IO.
public static class ScenarioOneEconomyIntegrationVerification
{
    public static void Begin()
    {
        EditorPrefs.SetBool("ScenarioOneEconomyIntegrationVerification.Run", true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
    public static void BeginViability()
    {
        EditorPrefs.SetBool("ScenarioOneEconomyIntegrationVerification.Viability", true); Begin();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool("ScenarioOneEconomyIntegrationVerification.Run", false)) return;
        EditorPrefs.SetBool("ScenarioOneEconomyIntegrationVerification.Run", false);
        new GameObject("Scenario One economy integration gate").AddComponent<ScenarioOneEconomyIntegrationGate>();
    }
}

public sealed class ScenarioOneEconomyIntegrationGate : MonoBehaviour
{
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marks;
    private ForestTreeSpawner spawner;
    private ForestSaveData original;
    private ScenarioOneDefinition originalDefinition, temporaryDefinition;
    private string originalHash;
    private int assertions;

    private IEnumerator Start()
    {
        yield return null;
        manager = FindFirstObjectByType<ScenarioOneManager>(); saves = FindFirstObjectByType<ForestSaveController>();
        ecology = FindFirstObjectByType<ForestEcologyController>(); marks = FindFirstObjectByType<ForestTreeMarkingManager>(); spawner = FindFirstObjectByType<ForestTreeSpawner>();
        original = saves.CaptureData(); originalHash = ScenarioReferenceArchive.WorldHash(original); originalDefinition = manager.Definition;
        bool viability = EditorPrefs.GetBool("ScenarioOneEconomyIntegrationVerification.Viability", false);
        EditorPrefs.SetBool("ScenarioOneEconomyIntegrationVerification.Viability", false);
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(viability ? Viability() : Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        manager.ConfigureDefinition(originalDefinition);
        if (temporaryDefinition != null) DestroyImmediate(temporaryDefinition);
        bool restored = saves.LoadData(original, false);
        yield return null; yield return null;
        if (!restored || ScenarioReferenceArchive.WorldHash(saves.CaptureData()) != originalHash)
            failure = failure ?? new InvalidOperationException("Gate failed to restore original world.");
        if (failure == null) Debug.Log((viability ? "SCENARIO_ONE_ECONOMY_VIABILITY_PASS" : "SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_PASS") + " assertions=" + assertions + " worldRestored=true noSaveSlotWrites=true");
        else Debug.LogError("SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Reset()
    {
        manager.ConfigureDefinition(originalDefinition);
        if (temporaryDefinition != null) { DestroyImmediate(temporaryDefinition); temporaryDefinition = null; }
        Check(saves.LoadData(Clone(original), false), "restore baseline");
        yield return null; yield return null;
        manager.InitializeNewScenario();
    }
    private ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(tree => tree.IsLiving).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
    private ScenarioOneWorkOrder Mark(ForestTree tree, FellingMaterialOutcome outcome)
    {
        manager.PlanningFellingOutcome = outcome; marks.Mark(tree, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan(); return manager.WorkOrders.Single(order => order.type == ScenarioWorkType.FellTree && order.targetTreeId == tree.TreeId && order.IsOpen);
    }
    private Vector3 PlantPosition(int index = 0)
    {
        var candidates = new List<Vector3>(); var trees = Trees();
        foreach (var cell in ecology.Cells.OrderByDescending(cell => cell.Light).ThenBy(cell => cell.Center.x).ThenBy(cell => cell.Center.y))
        foreach (float x in new[] { -1.5f, 0f, 1.5f }) foreach (float z in new[] { -1.5f, 0f, 1.5f })
        {
            var point = new Vector3(cell.Center.x + x, 0f, cell.Center.y + z);
            if (trees.All(tree => Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z), new Vector2(point.x, point.z)) >= 0.8f)) candidates.Add(point);
        }
        return candidates[index];
    }

    private IEnumerator Verify()
    {
        Check(ForestSaveData.CurrentVersion == 19 && originalDefinition.MinimumHarvestJobCents == 250000 && originalDefinition.OwnerMinutesPerYear == 2400, "production conventions");
        yield return Reset();
        var trees = Trees();
        Mark(trees[0], FellingMaterialOutcome.SellAndExtract); Mark(trees[1], FellingMaterialOutcome.KeepForUse); Mark(trees[2], FellingMaterialOutcome.RetainAsFallenDeadwood);
        var quote = manager.GetHarvestQuote();
        Check(quote.Orders.Count == 3 && quote.CostCents == 250000, "one small-job minimum for three trees");
        Equal(quote.TotalStemVolumeCm3, quote.SoldVolumeCm3 + quote.RetainedVolumeCm3 + quote.DeadwoodVolumeCm3 + quote.ResidualVolumeCm3, "whole modeled material conservation");
        foreach (var stem in quote.Yield.Stems)
            Equal(stem.StemVolumeCm3, stem.SaleAssortmentVolumeCm3 + stem.RetainedForUseVolumeCm3 + stem.DeadwoodAssortmentVolumeCm3 + stem.ResidualVolumeCm3, "foundation conservation");
        Check(manager.ApprovePendingWork() && manager.ReservedContractorCashCents == 250000, "reserve minimum once");
        long cash = manager.CashCents; float kept = manager.RetainedTimberM3;
        var felled = new Dictionary<string, int>(); Action<ForestTree> count = tree => { if (quote.Orders.Any(order => order.targetTreeId == tree.TreeId)) felled[tree.TreeId] = felled.TryGetValue(tree.TreeId, out int n) ? n + 1 : 1; };
        ForestTree.Felled += count;
        try { Check(manager.AdvanceYear(), "grouped annual work"); } finally { ForestTree.Felled -= count; }
        Check(felled.Count == 3 && felled.Values.All(n => n == 1), "every target felled exactly once");
        Equal(ForestryWorkCalculator.SumLedger(quote.Resolution.Ledger), manager.CashCents - cash, "cash equals signed job ledger");
        Near(kept + quote.RetainedVolumeCm3 / 1000000f, manager.RetainedTimberM3, "retained deposited once");
        Check(manager.DeadwoodRecords.Count == 1 && manager.AnnualReports.Last().harvestMinimumAdjustmentCents == quote.Resolution.Quote.Costs.MinimumJobAdjustmentCents, "deadwood path and minimum report");
        Check(manager.ManagementEvents.Where(e => e.taskType == ScenarioWorkType.FellTree && e.eventType == ScenarioManagementEventType.WorkResolved).Sum(e => e.cashDeltaCents) == manager.CashCents - cash, "one financial settlement in history");
        float afterKept = manager.RetainedTimberM3; Check(manager.AdvanceYear(), "following year"); Near(afterKept, manager.RetainedTimberM3, "no repeat deposit");

        yield return Reset();
        var small = spawner.Spawn("ECO-TINY", spawner.DefaultSpecies, new Vector3(0f, 0f, 0f), 5, 4f, 3.5f, 0.4f);
        var shortStem = spawner.Spawn("ECO-SHORT", spawner.DefaultSpecies, new Vector3(-2f, 0f, 0f), 3, 4f, 1.1f, 0.3f);
        var broadleaf = spawner.Spawn("ECO-BEECH", spawner.ResolveSpecies("beech"), new Vector3(2f, 0f, 0f), 20, 20f, 12f, 2f);
        Mark(small, FellingMaterialOutcome.SellAndExtract); Mark(broadleaf, FellingMaterialOutcome.SellAndExtract);
        Mark(shortStem, FellingMaterialOutcome.SellAndExtract);
        var noMarket = manager.GetHarvestQuote(); Check(noMarket.HasUnmarketedSpecies && noMarket.RevenueCents == 0 && noMarket.CostCents > 0 && noMarket.Task.Timber.Length == 0, "unmarketed/all-residual work without fake sale/pulp");
        Check(manager.ApprovePendingWork(), "approve unpriced work"); cash = manager.CashCents;
        Check(manager.AdvanceYear() && small.IsStump && broadleaf.IsStump && shortStem.IsStump, "unpriced world effects including below-breast-height stem"); Equal(-noMarket.CostCents, manager.CashCents - cash, "unpriced charge");

        yield return Reset(); trees = Trees();
        var dropped = Mark(trees[0], FellingMaterialOutcome.SellAndExtract); Mark(trees[1], FellingMaterialOutcome.SellAndExtract);
        Check(manager.ApprovePendingWork(), "approve before target loss"); trees[0].ApplyMortality("fixture", 0);
        var requoted = manager.GetHarvestQuote(true); Check(requoted.Orders.Count == 1, "dead target dropped before execution");
        cash = manager.CashCents; Check(manager.AdvanceYear(), "valid remainder executes"); Equal(requoted.RevenueCents - requoted.CostCents, manager.CashCents - cash, "remainder re-quote settlement");
        Check(dropped.status != ScenarioWorkStatus.Completed, "dead target not harvested");
        yield return Reset(); trees = Trees(); Mark(trees[0], FellingMaterialOutcome.SellAndExtract); Check(manager.ApprovePendingWork(), "approve empty-remainder setup");
        trees[0].Fell(); cash = manager.CashCents; Check(manager.AdvanceYear(), "empty remainder year advances"); Equal(cash, manager.CashCents, "empty remainder no charge");

        yield return Reset();
        var forbidden = Mark(Trees()[0], FellingMaterialOutcome.SellAndExtract); forbidden.executionMethod = WorkExecutionMethod.LandownerSimulated;
        Check(!manager.ApprovePendingWork() && forbidden.validationMessage.Contains("contractor-only"), "owner harvest readable ineligibility");

        yield return Reset();
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1), "buy owner planting stock"); cash = manager.CashCents;
        Check(manager.TryDesignateExactPlanting("sessile-oak-sapling", PlantPosition(), WorkExecutionMethod.LandownerSimulated, true), "owner sheltered designation");
        var order = manager.WorkOrders.Single(); var plantQuote = manager.GetPlantingQuote(order);
        Check(plantQuote.OwnerMinutes == 3 && plantQuote.WorkCents == 0 && plantQuote.MaterialCents == 500, "owner time/material quote");
        Check(manager.ApprovePendingWork() && manager.AdvanceYear(), "owner sheltered planting resolution");
        Equal(cash - 500, manager.CashCents, "owner pays material, not wage or stock twice"); Equal(3, manager.OwnerMinutesUsedThisYear, "owner minutes consumed");
        Check(manager.GetStockQuantity("sessile-oak-sapling") == 0 && manager.Shelters.Count == 1 && ecology.Browsing.Shelters.Count == 1, "stock consumed and shelter installed once");
        var juvenile = manager.PlantedJuveniles.Single(); var shelter = manager.Shelters.Single();
        Check(shelter.shelterId == "S-J" + juvenile.juvenileId && shelter.installedYear == juvenile.plantingYear && shelter.installedYear == 1 && shelter.effectiveYears == 8, "shelter timing/ID contract");
        Check(juvenile.lastBrowseAssessment.Protection == BrowseProtectionState.EffectiveShelter && !juvenile.lastYearBrowsed && shelter.IsEffective(8) && !shelter.IsEffective(9), "sheltered first step and eight-step lifetime");
        var shelteredSave = saves.CaptureData(); Check(saves.LoadData(Clone(shelteredSave), false) && saves.LoadData(Clone(shelteredSave), false), "repeat shelter restore");
        yield return null; yield return null; Check(ecology.Browsing.Shelters.Count == 1, "restore does not duplicate protection");
        string beforeProtectedPreview = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(manager.TryBeginReferencePreview(20), "protected player can preview frozen reference");
        Check(ecology.Browsing.Shelters.Count == 0, "historical preview replaces player protection with none");
        manager.EndReferencePreview(); yield return null; yield return null;
        Check(ecology.Browsing.Shelters.Count == 1 && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == beforeProtectedPreview, "preview exit replaces exact saved protection/player state");
        var legacy = Clone(shelteredSave); legacy.version = 14; legacy.regenerationModel = RegenerationModel.Legacy; legacy.growthModel = GrowthModel.Legacy; Check(saves.LoadData(legacy, false), "v14 safe migration");
        yield return null; yield return null;
        Check(ecology.Browsing.Shelters.Count == 0 && ecology.Browsing.ProtectedAreas.Count == 0 && manager.OwnerMinutesUsedThisYear == 0
            && manager.WorkOrders.All(o => o.executionMethod == WorkExecutionMethod.Contractor && !o.installShelter && o.harvestJobId == -1), "v14 defaults replace protection");

        yield return Reset();
        Check(manager.TryPurchaseStock("beech-sapling", 1), "budget stock");
        temporaryDefinition = Instantiate(originalDefinition); Field(temporaryDefinition, "ownerMinutesPerYear", 2); manager.ConfigureDefinition(temporaryDefinition);
        Check(manager.TryDesignateExactPlanting("beech-sapling", PlantPosition(), WorkExecutionMethod.LandownerSimulated, true), "budget designation");
        Check(!manager.ApprovePendingWork() && manager.WorkOrders.Single().validationMessage.Contains("owner time"), "compound owner budget enforced");
        Equal(0, manager.Shelters.Count, "ineligible work no shelter");
        Check(!manager.TryDesignateExactPlanting("beech-sapling", new Vector3(1000, 0, 1000), WorkExecutionMethod.Contractor, true), "failed planting outside stand");
        Equal(0, ecology.Browsing.Shelters.Count, "failed designation no protection");

        yield return Reset();
        Check(manager.TryPurchaseStock("beech-sapling", 1) && manager.TryDesignateExactPlanting("beech-sapling", PlantPosition(), WorkExecutionMethod.LandownerSimulated, true)
            && manager.ApprovePendingWork(), "approved planting failure setup");
        manager.WorkOrders.Single().cellIndex = -1; cash = manager.CashCents;
        Check(manager.AdvanceYear(), "invalid planting does not block year");
        Check(manager.Shelters.Count == 0 && ecology.Browsing.Shelters.Count == 0 && manager.GetStockQuantity("beech-sapling") == 1, "failed work installs no shelter/consumes no stock");
        Equal(cash, manager.CashCents, "failed planting no material/wage settlement");

        string contractorBio = null, ownerBio = null;
        foreach (var method in new[] { WorkExecutionMethod.Contractor, WorkExecutionMethod.LandownerSimulated })
        {
            yield return Reset(); Check(manager.TryPurchaseStock("beech-sapling", 1), "invariance stock");
            Check(manager.TryDesignateExactPlanting("beech-sapling", PlantPosition(), method, true) && manager.ApprovePendingWork(), "invariance plan");
            cash = manager.CashCents; var quotePlant = manager.GetPlantingQuote(manager.WorkOrders.Single()); Check(manager.AdvanceYear(), "invariance execute");
            Equal(cash + quotePlant.LedgerCashDelta, manager.CashCents, "plant cash equals ledger");
            if (method == WorkExecutionMethod.Contractor) { Check(quotePlant.WorkCents > 0, "contractor charged"); contractorBio = Biology(saves.CaptureData()); }
            else ownerBio = Biology(saves.CaptureData());
        }
        Check(contractorBio == ownerBio, "executor changes finance/time, not biology");

        yield return Reset();
        Mark(Trees()[0], FellingMaterialOutcome.KeepForUse); Check(manager.ApprovePendingWork(), "mid-plan approved harvest");
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 1) && manager.TryDesignateExactPlanting("sessile-oak-sapling", PlantPosition(), WorkExecutionMethod.LandownerSimulated, true), "mid-plan pending owner/shelter");
        var mid = saves.CaptureData(); Check(mid.scenarioOne.workOrders.Any(o => o.status == ScenarioWorkStatus.Pending && o.installShelter && o.executionMethod == WorkExecutionMethod.LandownerSimulated)
            && mid.scenarioOne.workOrders.Any(o => o.status == ScenarioWorkStatus.Approved && o.harvestJobId == 1), "mid-plan fields captured");
        manager.GetHarvestQuote(true);
        Check(saves.LoadData(Clone(mid), false) && saves.LoadData(Clone(mid), false), "same-plan repeated restore invalidates object-bound quotes");
        yield return null; yield return null;
        Check(manager.ApprovePendingWork(), "uninterrupted approve remainder"); for (int year = 0; year < 10; year++) Check(manager.AdvanceYear(), "uninterrupted continuation");
        string uninterrupted = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(saves.LoadData(Clone(mid), false), "mid-plan restore"); yield return null; yield return null;
        Check(manager.ApprovePendingWork(), "restored approve remainder"); for (int year = 0; year < 10; year++) Check(manager.AdvanceYear(), "restored continuation");
        string resumed = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Debug.Log("ECONOMY_MIDPLAN_CONTINUITY uninterrupted=" + uninterrupted + " restored=" + resumed); Check(uninterrupted == resumed, "ten-year mid-plan continuation");
        long chronologicalCash = manager.Definition.StartingCashCents;
        foreach (var entry in manager.ManagementEvents)
        {
            chronologicalCash += entry.cashDeltaCents;
            Equal(chronologicalCash, entry.closingCashCents, "history closing cash follows settlement chronology");
        }
    }

    private IEnumerator Viability()
    {
        foreach (int minimum in new[] { 150000, 200000, 250000 })
        {
            yield return Reset(); temporaryDefinition = Instantiate(originalDefinition); Field(temporaryDefinition, "minimumHarvestJobCents", (long)minimum); manager.ConfigureDefinition(temporaryDefinition);
            long first = 0, second = 0; var trajectory = new List<string>(); bool allAdvanced = true;
            // Four modest spaced interventions, retaining an intact canopy; one deadwood stem in first job.
            for (int year = 0; year < 30; year++)
            {
                if (year == 0 || year == 8 || year == 16 || year == 24)
                {
                    var targets = Trees().Where(tree => tree.TreeId.StartsWith("P", StringComparison.Ordinal)).OrderBy(tree => tree.Diameter).ThenBy(tree => tree.TreeId).Take(year == 0 ? 36 : 24).ToArray();
                    for (int i = 0; i < targets.Length; i++) Mark(targets[i], i == 0 ? FellingMaterialOutcome.RetainAsFallenDeadwood : FellingMaterialOutcome.SellAndExtract);
                    var job = manager.GetHarvestQuote(); if (year == 0) first = job.RevenueCents - job.CostCents; if (year == 8) second = job.RevenueCents - job.CostCents;
                    Debug.Log($"ECONOMY_VIABILITY_JOB minimum={minimum} year={year} trees={job.Orders.Count} revenue={job.RevenueCents} cost={job.CostCents} net={job.RevenueCents - job.CostCents}");
                    Check(manager.ApprovePendingWork(), "viability thinning affordable");
                }
                if (year == 0 || year == 24)
                {
                    for (int p = 0; p < 2; p++)
                    {
                        string item = p == 0 ? "beech-sapling" : "sessile-oak-sapling";
                        Check(manager.TryPurchaseStock(item, 1) && manager.TryDesignateExactPlanting(item, PlantPosition(p), WorkExecutionMethod.LandownerSimulated, true), "viability enrichment");
                    }
                    Check(manager.ApprovePendingWork(), "viability enrichment approval");
                }
                bool advanced = manager.AdvanceYear(); allAdvanced &= advanced;
                trajectory.Add((year + 1) + ":" + manager.CashCents);
                if (!advanced) break;
                if (year % 5 == 4) yield return null;
            }
            Debug.Log($"ECONOMY_VIABILITY minimum={minimum} firstNet={first} secondNet={second} finalCash={manager.CashCents} outcome={manager.Outcome} completedYear={manager.OutcomeYear} allAdvanced={allAdvanced} objectives={string.Join(",", manager.Objectives.Select(o => o.objectiveId + "=" + o.achieved))} trajectory={string.Join(";", trajectory)}");
        }
        yield return Reset(); for (int year = 0; year < 30; year++) { Check(manager.AdvanceYear(), "do-nothing control"); if (year % 5 == 4) yield return null; }
        Debug.Log($"ECONOMY_VIABILITY_DO_NOTHING cash={manager.CashCents} outcome={manager.Outcome} objectives={string.Join(",", manager.Objectives.Select(o => o.objectiveId + "=" + o.achieved))}");
    }

    [Serializable] private sealed class BiologyView { public int year; public List<TreeSaveData> trees; public List<ForestCellSaveData> cells; public List<PlantedJuvenileSaveData> planted; }
    private string Biology(ForestSaveData data)
    {
        ScenarioReferenceArchive.Canonicalize(data); return JsonUtility.ToJson(new BiologyView { year = data.ecologicalYear, trees = data.trees, cells = data.cells, planted = data.scenarioOne.plantedJuveniles });
    }
    private static T Clone<T>(T data) => JsonUtility.FromJson<T>(JsonUtility.ToJson(data));
    private static void Field(object target, string name, object value) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    private void Check(bool ok, string message) { assertions++; if (!ok) throw new InvalidOperationException(message); }
    private void Equal(long expected, long actual, string message) => Check(expected == actual, message + " expected=" + expected + " actual=" + actual);
    private void Near(float expected, float actual, string message) => Check(Mathf.Abs(expected - actual) < 0.00001f, message + " expected=" + expected + " actual=" + actual);
}
#endif

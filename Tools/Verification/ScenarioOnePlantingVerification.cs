using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Play Mode integration runner. Copy into Assets/ForestPrototype,
// run ScenarioOnePlantingVerification.Begin in Editor batchmode, then remove
// the Assets copy and generated .meta. Restores the user's existing save.
public static class ScenarioOnePlantingVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MixedSpeciesTest.unity");
        ScenarioOneManager mixed = UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>();
        if (mixed == null || mixed.Definition == null || mixed.Definition.ShopEntries.Count != 2)
            throw new InvalidOperationException("MixedSpeciesTest has no configured Scenario One nursery.");
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("Scenario One Planting Verification").AddComponent<ScenarioOnePlantingVerificationRunner>();
    }
}

public sealed class ScenarioOnePlantingVerificationRunner : MonoBehaviour
{
    private string savePath;
    private byte[] previousSave;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator test = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try
            {
                more = test.MoveNext();
                if (more) current = test.Current;
            }
            catch (Exception ex) { failure = ex; break; }
            if (!more) break;
            yield return current;
        }

        if (failure == null) Debug.Log("SCENARIO_ONE_PLANTING_VERIFY_PASS");
        else Debug.LogError("SCENARIO_ONE_PLANTING_VERIFY_FAIL: " + failure);
        if (savePath != null)
        {
            if (previousSave != null) File.WriteAllBytes(savePath, previousSave);
            else if (File.Exists(savePath)) File.Delete(savePath);
        }
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Verify()
    {
        ScenarioOneManager manager = FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        Require(manager != null && ecology != null && spawner != null && saves != null, "scenario systems missing");
        Require(ecology.LivingTreeCount == 336, "fresh stand changed");
        Require(manager.Definition.ShopEntries.Count == 2, "shop offers missing");
        Require(spawner.ResolveSpecies("beech") != null && spawner.ResolveSpecies("sessile-oak") != null,
            "shop species missing from scene");

        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) previousSave = File.ReadAllBytes(savePath);
        long initialCash = manager.CashCents;
        Require(!manager.TryPurchaseStock("beech-sapling", 0), "zero-quantity purchase accepted");
        Require(!manager.TryPurchaseStock("beech-sapling", -1), "negative-quantity purchase accepted");
        Require(!manager.TryPurchaseStock("unknown", 1), "unknown stock accepted");
        Require(!manager.TryPurchaseStock("beech-sapling", int.MaxValue), "unaffordable bulk purchase accepted");
        Require(manager.CashCents == initialCash && manager.Inventory.Count == 0, "rejected purchases mutated state");

        ScenarioShopEntry beechOffer = manager.Definition.FindShopEntry("beech-sapling");
        ScenarioShopEntry oakOffer = manager.Definition.FindShopEntry("sessile-oak-sapling");
        Require(manager.TryPurchaseStock(beechOffer.itemId, 1) && manager.TryPurchaseStock(oakOffer.itemId, 1),
            "stock purchase failed");
        long afterPurchases = initialCash - beechOffer.unitPriceCents - oakOffer.unitPriceCents;
        Require(manager.CashCents == afterPurchases, "purchase settlement in cents differs");
        Require(manager.GetStockQuantity(beechOffer.itemId) == 1 && manager.GetStockQuantity(oakOffer.itemId) == 1,
            "stock quantities wrong");

        const int beechCell = 20, oakCell = 21, pendingCell = 22;
        Require(!manager.TryDesignatePlanting(beechOffer.itemId, -1), "outside cell designation accepted");
        Require(manager.TryDesignatePlanting(beechOffer.itemId, beechCell), "Beech designation failed");
        Require(!manager.TryDesignatePlanting(beechOffer.itemId, beechCell), "duplicate cell/species accepted");
        Require(manager.TryDesignatePlanting(oakOffer.itemId, oakCell), "Oak designation failed");
        Require(manager.TryDesignatePlanting(beechOffer.itemId, pendingCell), "second Beech designation failed");
        Require(!manager.TryDesignatePlanting("unknown", 19), "unknown designation accepted");
        Require(manager.WorkOrders.Count == 3 && manager.WorkOrders[0].workOrderId == 1
            && manager.WorkOrders[2].workOrderId == 3, "order identity is not persistent/monotonic");
        Require(manager.ApprovePendingWork(), "affordable stocked work was not approved");
        Require(manager.WorkOrders.Count(order => order.status == ScenarioWorkStatus.Approved) == 2,
            "unstocked third task was approved");
        Require(manager.WorkOrders[2].status == ScenarioWorkStatus.Pending && !string.IsNullOrEmpty(manager.WorkOrders[2].validationMessage),
            "unstocked task did not remain pending with feedback");
        Require(manager.GetStockQuantity(beechOffer.itemId) == 1, "approval consumed stock early");

        long contractorCost = manager.WorkOrders[0].estimatedCostCents + manager.WorkOrders[1].estimatedCostCents;
        int overspend = (int)((manager.CashCents - contractorCost) / beechOffer.unitPriceCents) + 1;
        Require(!manager.TryPurchaseStock(beechOffer.itemId, overspend), "approved contractor cash was spent in the shop");
        Require(manager.CashCents == afterPurchases, "rejected post-approval purchase changed cash");
        Require(manager.AdvanceYear(), "annual resolution failed");
        Require(ecology.EcologicalYear == 1 && manager.AnnualReports.Count == 1, "ecology did not advance exactly once");
        Require(manager.AnnualReports[0].completedTasks == 2 && manager.AnnualReports[0].contractorCostCents == contractorCost,
            "annual contractor reporting wrong");
        Require(manager.CashCents == afterPurchases - contractorCost, "contractor settlement wrong");
        Require(manager.GetStockQuantity(beechOffer.itemId) == 0 && manager.GetStockQuantity(oakOffer.itemId) == 0,
            "stock was not consumed once per successful planting");
        Require(manager.WorkOrders[0].status == ScenarioWorkStatus.Completed
            && manager.WorkOrders[1].status == ScenarioWorkStatus.Completed
            && manager.WorkOrders[2].status == ScenarioWorkStatus.Pending, "order status incorrect after resolution");
        VerifyPlanted(ecology, beechCell, "beech");
        VerifyPlanted(ecology, oakCell, "sessile-oak");
        Require(!manager.TryDesignatePlanting(beechOffer.itemId, beechCell), "live cohort was accepted as new planting");

        ScenarioOneSaveData plantedState = manager.CaptureSaveData();
        saves.Save();
        ecology.RestoreEcologyState(0, ecology.SimulationSeed);
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Require(ecology.EcologicalYear == 1 && manager.CashCents == plantedState.cashCents
            && manager.WorkOrders.Count == 3 && manager.AnnualReports.Count == 1
            && manager.WorkOrders[0].status == ScenarioWorkStatus.Completed
            && manager.WorkOrders[1].status == ScenarioWorkStatus.Completed
            && manager.WorkOrders[2].status == ScenarioWorkStatus.Pending,
            "completed planting orders or reports did not survive save/load");
        VerifyPlanted(ecology, beechCell, "beech");
        VerifyPlanted(ecology, oakCell, "sessile-oak");

        // Full-cell biological failure: approved work pays nothing and keeps the sapling.
        manager.InitializeNewScenario();
        ecology.RestoreEcologyState(0, ecology.SimulationSeed);
        const int fullCell = 24;
        ForestRegenerationCohort sitka = ecology.Cells[fullCell].GetOrCreateCohort(spawner.DefaultSpecies);
        sitka.Restore(spawner.DefaultSpecies.RegenDensityMax, 1f, 0);
        Require(manager.TryPurchaseStock(beechOffer.itemId, 1), "failure-test purchase failed");
        Require(manager.TryDesignatePlanting(beechOffer.itemId, fullCell), "full-cell order rejected before execution");
        Require(manager.ApprovePendingWork(), "full-cell order approval failed");
        long cashBeforeFailure = manager.CashCents;
        Require(manager.AdvanceYear(), "failed work blocked annual advance");
        Require(manager.WorkOrders[0].status == ScenarioWorkStatus.Failed && manager.AnnualReports[0].failedTasks == 1,
            "full-cell biology did not fail task");
        Require(manager.CashCents == cashBeforeFailure && manager.GetStockQuantity(beechOffer.itemId) == 1,
            "failed planting consumed cash or stock");

        ScenarioOneSaveData failedState = manager.CaptureSaveData();
        long failedCost = manager.WorkOrders[0].estimatedCostCents;
        int spendAll = (int)(cashBeforeFailure / oakOffer.unitPriceCents);
        Require(cashBeforeFailure - (long)spendAll * oakOffer.unitPriceCents < failedCost,
            "release probe would not distinguish a stranded cash reservation");
        Require(manager.TryPurchaseStock(oakOffer.itemId, spendAll),
            "failed work stranded its contractor cash reservation");
        manager.RestoreSaveData(failedState);
        Require(manager.CashCents == cashBeforeFailure && manager.GetStockQuantity(beechOffer.itemId) == 1,
            "restoring the failure probe changed retained cash or stock");

        // Save v10 round-trip with pending work and exact inventory; load also
        // restores the cells before validating the order against them.
        const int saveCell = 30;
        Require(manager.TryDesignatePlanting(beechOffer.itemId, saveCell), "save-test designation failed");
        Require(manager.ApprovePendingWork(), "failed work stranded its sapling reservation");
        Require(manager.CashCents == cashBeforeFailure && manager.GetStockQuantity(beechOffer.itemId) == 1,
            "re-approval charged or consumed stock prematurely");
        Require(manager.ReservedContractorCashCents == manager.WorkOrders[1].estimatedCostCents
            && manager.GetReservedStockQuantity(beechOffer.itemId) == 1,
            "approved cash or stock is not visibly reserved");
        Require(manager.CancelApprovedOrder(2), "approved planting could not be cancelled");
        Require(manager.ReservedContractorCashCents == 0 && manager.GetReservedStockQuantity(beechOffer.itemId) == 0
            && manager.CashCents == cashBeforeFailure && manager.GetStockQuantity(beechOffer.itemId) == 1,
            "cancelling approved work left a cash or stock reservation stranded");
        manager.RestoreSaveData(failedState);
        Require(manager.TryDesignatePlanting(beechOffer.itemId, saveCell), "save-test re-designation failed");
        ScenarioOneSaveData before = manager.CaptureSaveData();
        ScenarioOneSaveData json = JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(before));
        Require(JsonUtility.ToJson(json) == JsonUtility.ToJson(before), "scenario JSON round-trip changed state");
        saves.Save();
        Require(ecology.TryPlantJuvenile(spawner.ResolveSpecies("beech"), Center(ecology, saveCell)).Success,
            "mutation before load failed");
        manager.InitializeNewScenario();
        saves.Load();
        yield return null;
        Require(ecology.EcologicalYear == 1 && manager.CashCents == before.cashCents
            && manager.GetStockQuantity(beechOffer.itemId) == before.inventory[0].quantity,
            "save-load economy or ecology changed");
        Require(manager.WorkOrders.Count == 2 && manager.WorkOrders[0].status == ScenarioWorkStatus.Failed
            && manager.WorkOrders[1].status == ScenarioWorkStatus.Pending && manager.WorkOrders[1].workOrderId == 2,
            "save-load order state changed");
        Require(string.IsNullOrEmpty(manager.WorkOrders[1].validationMessage),
            "restored order was validated against stale ecology");
        Require(manager.ApprovePendingWork(), "restored pending order could not be approved");
        Require(manager.AdvanceYear() && ecology.EcologicalYear == 2, "restored order did not continue annual cycle");
        Require(manager.GetStockQuantity(beechOffer.itemId) == 0
            && manager.WorkOrders[1].status == ScenarioWorkStatus.Completed
            && manager.AnnualReports.Count == 2, "restored stock/order/report did not continue");
        ForestRegenerationCohort continued = ecology.Cells[saveCell].FindCohort("beech");
        Require(continued != null && continued.Origin == RegenerationOrigin.Planted && continued.OriginYear == 1,
            "restored planting did not continue with the right provenance and year");

        saves.Save();
        ForestSaveData legacy = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(savePath));
        legacy.version = 9;
        File.WriteAllText(savePath, JsonUtility.ToJson(legacy));
        saves.Load();
        yield return null;
        Require(ecology.EcologicalYear == 2 && manager.CashCents == initialCash
            && manager.Inventory.Count == 0 && manager.WorkOrders.Count == 0
            && ecology.Cells[saveCell].FindCohort("beech") != null,
            "version-9 migration lost ecology or failed to start the configured economy");
        Debug.Log("SCENARIO_ONE_PLANTING_DETAIL beech=" + beechCell + " oak=" + oakCell
            + " failureStockRetained=True saveVersion=" + ForestSaveData.CurrentVersion);
    }

    private static Vector3 Center(ForestEcologyController ecology, int index)
    {
        Vector2 center = ecology.Cells[index].Center;
        return new Vector3(center.x, 0f, center.y);
    }

    private static void VerifyPlanted(ForestEcologyController ecology, int index, string speciesId)
    {
        ForestRegenerationCohort cohort = ecology.Cells[index].FindCohort(speciesId);
        Require(cohort != null && cohort.Density > 0f, speciesId + " biological cohort missing");
        Require(cohort.Origin == RegenerationOrigin.Planted && cohort.OriginYear == 0
            && cohort.EstablishYear == -ecology.PlantedJuvenileAgeYears,
            speciesId + " planted origin/year wrong");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

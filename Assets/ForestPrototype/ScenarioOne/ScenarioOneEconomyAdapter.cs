using System;
using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using CCF.Forestry.TimberYield;
using UnityEngine;

// Scenario conventions only. Existing managers remain world/approval/wallet owners.
public sealed class ScenarioHarvestJob
{
    public int JobId;
    public List<ScenarioOneWorkOrder> Orders = new List<ScenarioOneWorkOrder>();
    public StandOperationYield Yield;
    public ForestryTask Task;
    public WorkResolution Resolution;
    public long TotalStemVolumeCm3, SoldVolumeCm3, RetainedVolumeCm3, DeadwoodVolumeCm3, ResidualVolumeCm3;
    public bool HasUnmarketedSpecies;
    public string Problem = "";
    public long CostCents => Resolution != null ? Resolution.Quote.Costs.TotalExternalCostCents : 0;
    public long RevenueCents => Resolution != null ? Resolution.Quote.TimberSaleRevenueCents : 0;
    public bool Eligible => Orders.Count == 0 || (Resolution != null && Resolution.Resolved && string.IsNullOrEmpty(Problem));
}

public sealed class ScenarioPlantingQuote
{
    public WorkResolution Planting, Shelter;
    public long CostCents, WorkCents, MaterialCents;
    public int OwnerMinutes, PersonMinutes;
    public string Problem = "";
    public bool Eligible => string.IsNullOrEmpty(Problem);
    public long LedgerCashDelta => (Planting != null ? ForestryWorkCalculator.SumLedger(Planting.Ledger) : 0)
        + (Shelter != null ? ForestryWorkCalculator.SumLedger(Shelter.Ledger) : 0);
}

public static class ScenarioOneEconomyAdapter
{
    public const string ShelterMaterialId = "scenario-tree-shelter";

    public static ForestryPriceBook PriceBook(ScenarioOneDefinition definition)
    {
        var book = Stage1EconomyDefaults.Create();
        book.MinimumHarvestJobCents.Selected = definition.MinimumHarvestJobCents;
        book.ContractorLabourCentsPerHour.Selected = definition.ContractorHourlyRateCents;
        book.OwnerTimeValueCentsPerHour.Selected = 0; book.OwnerEquipmentCentsPerHour.Selected = 0;
        var materials = new List<MaterialPrice>(book.Materials);
        foreach (var offer in definition.ShopEntries)
            if (offer != null && book.FindMaterial(offer.itemId) == null)
                materials.Add(Material(offer.itemId, MaterialCategory.NurseryStock, offer.unitPriceCents, "Existing live nursery price; consumed stock is not repurchased"));
        materials.Add(Material(ShelterMaterialId, MaterialCategory.Shelter, definition.TreeShelterMaterialCents, "[D] €5 Scenario One material placeholder; no quoted Irish retail price in report; NOT €2.56 grant allowance"));
        book.Materials = materials.ToArray(); return book;
    }

    private static MaterialPrice Material(string id, MaterialCategory category, long cents, string source) => new MaterialPrice
    {
        Id = id, Category = category, QuantityUnit = WorkQuantityUnit.Items,
        CentsPerReferenceQuantity = new EconomicParameter { Low = cents, Reference = cents, High = cents, Selected = cents,
            Unit = ParameterUnit.CentsPerReferenceQuantity, RangeEvidence = EvidenceLabel.GameplayCalibration, SelectedEvidence = EvidenceLabel.GameplayCalibration, Source = source }
    };

    public static ScenarioHarvestJob QuoteHarvest(IEnumerable<ScenarioOneWorkOrder> orders, Dictionary<string, ForestTree> trees,
        ScenarioOneDefinition definition, int resolutionYear, int previousInterventions, long availableCash)
    {
        var job = new ScenarioHarvestJob { JobId = resolutionYear };
        var requests = new List<StemYieldRequest>(); var ids = new HashSet<string>(StringComparer.Ordinal);
        var shortResidualStems = new List<StemYieldResult>();
        var book = PriceBook(definition); long fullWorkGrams = 0;
        foreach (var order in orders.OrderBy(x => x.workOrderId))
        {
            if (order == null || !trees.TryGetValue(order.targetTreeId ?? "", out var tree) || tree == null || !tree.IsLiving || !tree.CanChop || !ids.Add(tree.TreeId)) continue;
            if (order.executionMethod != WorkExecutionMethod.Contractor) job.Problem = "Scenario One harvest is contractor-only; owner production felling is ineligible.";
            if (!Enum.IsDefined(typeof(FellingMaterialOutcome), order.fellingOutcome)) { job.Problem = "Unknown felling material outcome."; continue; }
            long volume = checked((long)decimal.Floor((decimal)tree.BiologicalStemVolumeM3 * 1000000));
            if (volume <= 0) { job.Problem = "Target has no representable modeled stem material."; continue; }
            var stem = MerchantableStemModel.FromMetres(tree.TreeId, tree.Species.SpeciesId, tree.Diameter, tree.Height,
                tree.BiologicalStemVolumeM3, "[S] Scenario One over-bark biological stem volume/diameter basis");
            stem.StemVolumeCm3 = volume;
            var disposition = order.fellingOutcome == FellingMaterialOutcome.SellAndExtract ? TimberDisposition.SellAndExtract
                : order.fellingOutcome == FellingMaterialOutcome.KeepForUse ? TimberDisposition.KeepForUse : TimberDisposition.RetainAsFallenDeadwood;
            // The current whole-stem retained/deadwood world effect is preserved. Yield remains a grading diagnostic.
            if (stem.TotalHeightMm > 1300)
                requests.Add(new StemYieldRequest { Stem = stem, DefaultDisposition = disposition, ExtractRetainedToRoadside = true });
            else
                shortResidualStems.Add(new StemYieldResult { TreeId = stem.TreeId, SpeciesId = stem.SpeciesId,
                    ConfigurationId = "scenario-short-stem-budget-only", MeasurementBasis = stem.MeasurementBasis, DbhMm = stem.DbhMm,
                    TotalHeightMm = stem.TotalHeightMm, StemVolumeCm3 = volume, ResidualVolumeCm3 = volume,
                    ResidualSections = new[] { new StemResidualSection { Kind = StemResidualKind.BelowMerchantableDiameter,
                        StartHeightMm = 0, EndHeightMm = stem.TotalHeightMm, VolumeCm3 = volume } } });
            job.Orders.Add(order); job.TotalStemVolumeCm3 = checked(job.TotalStemVolumeCm3 + volume);
            if (disposition == TimberDisposition.KeepForUse) job.RetainedVolumeCm3 = checked(job.RetainedVolumeCm3 + volume);
            else if (disposition == TimberDisposition.RetainAsFallenDeadwood) job.DeadwoodVolumeCm3 = checked(job.DeadwoodVolumeCm3 + volume);
            if (tree.Species.SpeciesId != "sitka-spruce") job.HasUnmarketedSpecies = true;
            // [S] Whole modeled volume is the work basis; unmarketed species use the configured fresh-volume
            // work-equivalent proxy, not a claimed broadleaf green density or a broadleaf sale deck.
            fullWorkGrams = checked(fullWorkGrams + Grams(volume, book.FindDensity("sitka-fresh-roadside").KilogramsPerCubicMetre.Selected));
        }
        job.Yield = TimberYieldCalculator.ResolveStandOperation("harvest-" + resolutionYear, requests.ToArray(), TimberYieldDefaults.CreateSitka());
        if (shortResidualStems.Count > 0)
        {
            job.Yield.Stems = job.Yield.Stems.Concat(shortResidualStems).OrderBy(stem => stem.TreeId, StringComparer.Ordinal).ToArray();
            job.Yield.StemVolumeCm3 += shortResidualStems.Sum(stem => stem.StemVolumeCm3);
            job.Yield.ResidualVolumeCm3 += shortResidualStems.Sum(stem => stem.ResidualVolumeCm3);
        }
        if (job.Orders.Count == 0) return job;
        var saleYield = new StandOperationYield { OperationId = job.Yield.OperationId, ConfigurationId = job.Yield.ConfigurationId };
        var soldStems = new List<StemYieldResult>();
        foreach (var stem in job.Yield.Stems)
        {
            // Only marketable sold Sitka logs become priced batches. Kept/deadwood/ungraded material
            // is accounted as work quantity without fake product revenue or fake pulp.
            var sold = JsonUtility.FromJson<StemYieldResult>(JsonUtility.ToJson(stem));
            sold.Logs = sold.Logs.Where(x => x.Disposition == TimberDisposition.SellAndExtract && x.SpeciesId == "sitka-spruce").ToArray();
            sold.SaleAssortmentVolumeCm3 = sold.Logs.Sum(x => x.VolumeCm3); sold.RetainedForUseVolumeCm3 = 0; sold.DeadwoodAssortmentVolumeCm3 = 0;
            sold.ResidualVolumeCm3 = sold.StemVolumeCm3 - sold.SaleAssortmentVolumeCm3;
            soldStems.Add(sold); saleYield.StemVolumeCm3 += sold.StemVolumeCm3;
            saleYield.SaleAssortmentVolumeCm3 += sold.SaleAssortmentVolumeCm3; saleYield.ResidualVolumeCm3 += sold.ResidualVolumeCm3;
        }
        saleYield.Stems = soldStems.ToArray();
        var bindings = Enum.GetValues(typeof(TimberAssortment)).Cast<TimberAssortment>().Select(product => new TimberMarketBinding
            { SpeciesId = "sitka-spruce", Assortment = product, DensityId = "sitka-fresh-roadside", RoadsidePriceId = "sitka-" + product.ToString().ToLowerInvariant() }).ToArray();
        var batches = TimberYieldEconomyAdapter.ToTimberBatches(saleYield, bindings);
        long pricedWork = batches.Sum(batch => Grams(batch.Quantity, book.FindDensity(batch.DensityId).KilogramsPerCubicMetre.Selected));
        job.SoldVolumeCm3 = batches.Sum(x => x.Quantity);
        job.ResidualVolumeCm3 = job.TotalStemVolumeCm3 - job.SoldVolumeCm3 - job.RetainedVolumeCm3 - job.DeadwoodVolumeCm3;
        job.Task = new ForestryTask { TaskId = job.Yield.OperationId, WorldOperationId = job.Yield.OperationId,
            Type = ForestryTaskType.Harvest, Quantity = new WorkQuantity { Amount = job.Orders.Count },
            TargetIds = job.Orders.Select(x => x.targetTreeId).ToArray(), Timber = batches,
            UnpricedHarvestGreenGrams = Math.Max(0, fullWorkGrams - pricedWork), UnpricedForwardGreenGrams = Math.Max(0, fullWorkGrams - pricedWork),
            HarvestContext = previousInterventions == 0 ? HarvestOperationContext.FirstThinning : previousInterventions == 1 ? HarvestOperationContext.SecondThinning : HarvestOperationContext.LaterThinning };
        job.Resolution = ForestryWorkCalculator.Resolve(job.Task, WorkExecutionMethod.Contractor, Contractor(availableCash), book);
        if (!job.Resolution.Resolved && string.IsNullOrEmpty(job.Problem)) job.Problem = string.Join(" ", job.Resolution.Quote.UnmetRequirements.Select(x => x.Detail));
        return job;
    }

    public static WorkResolution ReResolveHarvest(ScenarioHarvestJob job, ScenarioOneDefinition definition, long availableCash)
        => ForestryWorkCalculator.Resolve(job.Task, WorkExecutionMethod.Contractor, Contractor(availableCash), PriceBook(definition));

    public static ScenarioPlantingQuote QuotePlanting(ScenarioOneWorkOrder order, ScenarioOneDefinition definition, int stockQuantity, long cash, int ownerMinutes)
    {
        var result = new ScenarioPlantingQuote();
        if (order.installShelter && !order.exactPosition) { result.Problem = "Tree shelters require a newly designated exact-position planting."; return result; }
        var actor = new ExecutionResources { AvailableCashCents = cash, AvailableOwnerMinutes = Math.Max(0, ownerMinutes), ContractorAvailable = true,
            Capabilities = WorkCapability.Planting | WorkCapability.Protection, Tools = WorkTool.PlantingTools | WorkTool.ProtectionTools,
            Stock = new[] { new AvailableMaterial { MaterialId = order.stockItemId, Quantity = new WorkQuantity { Amount = Math.Max(0, stockQuantity) } } } };
        var book = PriceBook(definition);
        result.Planting = ForestryWorkCalculator.Resolve(new ForestryTask { TaskId = "plant-" + order.workOrderId, WorldOperationId = "order-" + order.workOrderId,
            Type = ForestryTaskType.Planting, Quantity = new WorkQuantity { Amount = 1 }, Materials = new[] { new MaterialRequirement
                { MaterialId = order.stockItemId, Quantity = new WorkQuantity { Amount = 1 }, Supply = MaterialSupply.ExistingStock } } }, order.executionMethod, actor, book);
        if (order.installShelter)
            result.Shelter = ForestryWorkCalculator.Resolve(new ForestryTask { TaskId = "shelter-" + order.workOrderId, WorldOperationId = "order-" + order.workOrderId,
                Type = ForestryTaskType.ShelterInstallation, Quantity = new WorkQuantity { Amount = 1 }, Materials = new[] { new MaterialRequirement
                    { MaterialId = ShelterMaterialId, Quantity = new WorkQuantity { Amount = 1 }, Supply = MaterialSupply.PurchaseForJob } } }, order.executionMethod, actor, book);
        var parts = new[] { result.Planting, result.Shelter }.Where(x => x != null).ToArray();
        result.CostCents = parts.Sum(x => x.Quote.Costs.TotalExternalCostCents); result.WorkCents = parts.Sum(x => x.Quote.Costs.ContractorWorkCents);
        result.MaterialCents = parts.Sum(x => x.Quote.Costs.MaterialPurchasesCents);
        result.OwnerMinutes = checked((int)parts.Sum(x => x.Quote.Requirements.OwnerMinutes)); result.PersonMinutes = checked((int)parts.Sum(x => x.Quote.Requirements.PersonMinutes));
        result.Problem = string.Join(" ", parts.SelectMany(x => x.Quote.UnmetRequirements).Select(x => x.Detail));
        if (result.CostCents > cash) result.Problem = "Insufficient cash for planting and shelter materials together.";
        if (result.OwnerMinutes > ownerMinutes) result.Problem = "Planting and shelter work exceed the remaining annual owner time.";
        return result;
    }

    public static long Grams(long volumeCm3, long kilogramsPerM3) => checked((long)decimal.Round((decimal)volumeCm3 * kilogramsPerM3 / 1000, 0, MidpointRounding.AwayFromZero));
    private static ExecutionResources Contractor(long cash) => new ExecutionResources { AvailableCashCents = cash, ContractorAvailable = true,
        Capabilities = WorkCapability.ProductionHarvesting, Tools = WorkTool.HarvestingSystem };
}

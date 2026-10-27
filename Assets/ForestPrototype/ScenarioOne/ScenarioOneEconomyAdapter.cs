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
    // True when quoted under the Oak/Beech market (growth model 2, D-062); settlement must use the same price book.
    public bool BroadleafMarket;
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

    // broadleafMarket adds the Oak and Beech product prices and work-proxy densities (D-062). Off, the book is exactly
    // the Sitka-only book, so games that predate the market are priced as before.
    public static ForestryPriceBook PriceBook(ScenarioOneDefinition definition, bool broadleafMarket = false)
    {
        var book = Stage1EconomyDefaults.Create();
        if (broadleafMarket)
        {
            book.TimberPrices = book.TimberPrices.Concat(Stage1EconomyDefaults.BroadleafTimberPrices()).ToArray();
            book.Densities = book.Densities.Concat(Stage1EconomyDefaults.BroadleafDensities()).ToArray();
        }
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
        ScenarioOneDefinition definition, int resolutionYear, int previousInterventions, long availableCash,
        IReadOnlyDictionary<string, ScenarioDeadwoodRecord> fallenStems = null, int salvageCostBasisPoints = 10000, bool broadleafMarket = false)
    {
        var job = new ScenarioHarvestJob { JobId = resolutionYear, BroadleafMarket = broadleafMarket };
        var requests = new List<StemYieldRequest>(); var ids = new HashSet<string>(StringComparer.Ordinal);
        var shortResidualStems = new List<StemYieldResult>();
        var book = PriceBook(definition, broadleafMarket); long fullWorkGrams = 0, salvageWorkGrams = 0;
        var config = broadleafMarket ? TimberYieldDefaults.CreateScenarioOne() : TimberYieldDefaults.CreateSitka();
        var marketed = new HashSet<string>(book.TimberPrices.Select(price => price.SpeciesId), StringComparer.Ordinal);
        if (salvageCostBasisPoints < 10000 || salvageCostBasisPoints > 15000) throw new ArgumentOutOfRangeException(nameof(salvageCostBasisPoints));
        foreach (var order in orders.OrderBy(x => x.workOrderId))
        {
            if (order == null || !trees.TryGetValue(order.targetTreeId ?? "", out var tree) || tree == null || !ids.Add(tree.TreeId)) continue;
            bool salvage = order.type == ScenarioWorkType.SalvageDeadwood;
            ScenarioDeadwoodRecord fallen = null;
            if (salvage)
            {
                if (!tree.IsBiologicallyDead || tree.MortalityCause != "windthrow" || fallenStems == null
                    || !fallenStems.TryGetValue(tree.TreeId, out fallen) || fallen == null || fallen.remainingVolumeM3 <= 0 || fallen.DecayClass > 1) continue;
                if (order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood) { job.Problem = "Leaving deadwood requires no salvage order or charge."; continue; }
            }
            else if (!tree.IsLiving || !tree.CanChop || order.type != ScenarioWorkType.FellTree) continue;
            float stemVolume = salvage ? fallen.remainingVolumeM3 : tree.BiologicalStemVolumeM3;
            if (order.executionMethod != WorkExecutionMethod.Contractor) job.Problem = "Scenario One harvest is contractor-only; owner production felling is ineligible.";
            if (!Enum.IsDefined(typeof(FellingMaterialOutcome), order.fellingOutcome)) { job.Problem = "Unknown felling material outcome."; continue; }
            long volume = checked((long)decimal.Floor((decimal)stemVolume * 1000000));
            if (volume <= 0) { job.Problem = "Target has no representable modeled stem material."; continue; }
            var stem = salvage ? WindthrowSalvage.Measurements(fallen, resolutionYear)
                : MerchantableStemModel.FromMetres(tree.TreeId, tree.Species.SpeciesId, tree.Diameter, tree.Height,
                stemVolume, "[S] Scenario One over-bark biological stem volume/diameter basis");
            stem.StemVolumeCm3 = volume;
            // Species whose ladder has a quality-dependent rung (Oak) get the stem's pruned section from its recorded pruning
            // history. Sitka has none, so Sitka stems and their grading are unchanged. Salvage keeps its own damage sections.
            if (!salvage && config.Specifications.Any(spec => spec.SpeciesId == stem.SpeciesId && spec.RequiredQuality != StemQualityFlags.None))
                stem.QualitySections = PrunedSections(tree, stem.TotalHeightMm);
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
            if (!marketed.Contains(tree.Species.SpeciesId)) job.HasUnmarketedSpecies = true;
            // [S] Whole modeled volume is the work basis; unmarketed species use the configured fresh-volume
            // work-equivalent proxy, not a claimed broadleaf green density or a broadleaf sale deck.
            long stemGrams = Grams(volume, book.FindDensity("sitka-fresh-roadside").KilogramsPerCubicMetre.Selected);
            fullWorkGrams = checked(fullWorkGrams + stemGrams);
            if (salvage) salvageWorkGrams = checked(salvageWorkGrams + stemGrams);
        }
        job.Yield = TimberYieldCalculator.ResolveStandOperation("harvest-" + resolutionYear, requests.ToArray(), config);
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
            // Only sold logs of a species with a market become priced batches (Sitka; Oak and Beech when the broadleaf
            // market is on). Kept/deadwood/ungraded material is accounted as work quantity without fake product revenue or fake pulp.
            var sold = JsonUtility.FromJson<StemYieldResult>(JsonUtility.ToJson(stem));
            sold.Logs = sold.Logs.Where(x => x.Disposition == TimberDisposition.SellAndExtract && marketed.Contains(x.SpeciesId)).ToArray();
            sold.SaleAssortmentVolumeCm3 = sold.Logs.Sum(x => x.VolumeCm3); sold.RetainedForUseVolumeCm3 = 0; sold.DeadwoodAssortmentVolumeCm3 = 0;
            sold.ResidualVolumeCm3 = sold.StemVolumeCm3 - sold.SaleAssortmentVolumeCm3;
            soldStems.Add(sold); saleYield.StemVolumeCm3 += sold.StemVolumeCm3;
            saleYield.SaleAssortmentVolumeCm3 += sold.SaleAssortmentVolumeCm3; saleYield.ResidualVolumeCm3 += sold.ResidualVolumeCm3;
        }
        saleYield.Stems = soldStems.ToArray();
        // One binding per priced species/product, taken from the price book. For Sitka these are exactly the four bindings
        // built before (price ids sitka-pulp .. sitka-sawlog, density sitka-fresh-roadside).
        var bindings = book.TimberPrices.Select(price => new TimberMarketBinding
            { SpeciesId = price.SpeciesId, Assortment = price.Assortment, RoadsidePriceId = price.Id,
              DensityId = book.Densities.First(density => density.SpeciesId == price.SpeciesId).Id }).ToArray();
        var batches = TimberYieldEconomyAdapter.ToTimberBatches(saleYield, bindings);
        long pricedWork = batches.Sum(batch => Grams(batch.Quantity, book.FindDensity(batch.DensityId).KilogramsPerCubicMetre.Selected));
        job.SoldVolumeCm3 = batches.Sum(x => x.Quantity);
        job.ResidualVolumeCm3 = job.TotalStemVolumeCm3 - job.SoldVolumeCm3 - job.RetainedVolumeCm3 - job.DeadwoodVolumeCm3;
        job.Task = new ForestryTask { TaskId = job.Yield.OperationId, WorldOperationId = job.Yield.OperationId,
            Type = ForestryTaskType.Harvest, Quantity = new WorkQuantity { Amount = job.Orders.Count },
            TargetIds = job.Orders.Select(x => x.targetTreeId).ToArray(), Timber = batches,
            // A candidate salvage multiplier applies only to its share of work;
            // the existing grouped minimum remains one charge.
            SiteCostBasisPoints = 10000 + (fullWorkGrams > 0 ? (int)decimal.Round((decimal)salvageWorkGrams * (salvageCostBasisPoints - 10000) / fullWorkGrams, 0, MidpointRounding.AwayFromZero) : 0),
            UnpricedHarvestGreenGrams = Math.Max(0, fullWorkGrams - pricedWork), UnpricedForwardGreenGrams = Math.Max(0, fullWorkGrams - pricedWork),
            HarvestContext = previousInterventions == 0 ? HarvestOperationContext.FirstThinning : previousInterventions == 1 ? HarvestOperationContext.SecondThinning : HarvestOperationContext.LaterThinning };
        job.Resolution = ForestryWorkCalculator.Resolve(job.Task, WorkExecutionMethod.Contractor, Contractor(availableCash), book);
        if (!job.Resolution.Resolved && string.IsNullOrEmpty(job.Problem)) job.Problem = string.Join(" ", job.Resolution.Quote.UnmetRequirements.Select(x => x.Detail));
        return job;
    }

    public static WorkResolution ReResolveHarvest(ScenarioHarvestJob job, ScenarioOneDefinition definition, long availableCash)
        => ForestryWorkCalculator.Resolve(job.Task, WorkExecutionMethod.Contractor, Contractor(availableCash), PriceBook(definition, job.BroadleafMarket));

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

    // The recorded pruning history as a quality section: the stem from the ground to the crown base left by the last lift.
    // Pruning is the only persisted per-tree form state, so it is the only form signal available (a persistent fork or
    // shaping state does not exist). It marks a section eligible for clear grades; it sets no price.
    public static StemQualitySection[] PrunedSections(ForestTree tree, int totalHeightMm)
    {
        if (tree == null || tree.PruningLifts <= 0 || tree.CrownBaseHeightM <= 0f) return Array.Empty<StemQualitySection>();
        int end = Math.Min(totalHeightMm - 1, (int)Math.Round(tree.CrownBaseHeightM * 1000f, MidpointRounding.AwayFromZero));
        return end > 0 ? new[] { new StemQualitySection { StartHeightMm = 0, EndHeightMm = end, Flags = StemQualityFlags.Pruned } } : Array.Empty<StemQualitySection>();
    }

    public static long Grams(long volumeCm3, long kilogramsPerM3) => checked((long)decimal.Round((decimal)volumeCm3 * kilogramsPerM3 / 1000, 0, MidpointRounding.AwayFromZero));
    private static ExecutionResources Contractor(long cash) => new ExecutionResources { AvailableCashCents = cash, ContractorAvailable = true,
        Capabilities = WorkCapability.ProductionHarvesting, Tools = WorkTool.HarvestingSystem };
}

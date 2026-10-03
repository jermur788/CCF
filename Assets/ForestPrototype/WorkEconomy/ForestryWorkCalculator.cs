using System;
using System.Collections.Generic;

namespace CCF.Forestry.WorkEconomy
{
    public static class ForestryWorkCalculator
    {
        private const long GramsPerTonne = 1000000;

        public static WorkQuote Quote(ForestryTask task, WorkExecutionMethod method, ExecutionResources actor, ForestryPriceBook book)
        {
            ForestryEconomyValidation.Validate(task, method, actor, book);
            // Every arithmetic boundary is checked. Overflow fails rather than returning a corrupted quote.
            checked
            {
                var profile = book.FindProfile(task.Type);
                var quote = new WorkQuote { PriceBookId = book.Id, Method = method, Operation = Describe(task), PricingComplete = true,
                    HarvestContext = task.HarvestContext, SiteCostBasisPoints = task.SiteCostBasisPoints };
                var issues = new List<UnmetRequirement>();
                var ledger = new List<EconomicLedgerEntry>();
                bool hasWork = task.Quantity.Amount > 0;
                bool owner = method == WorkExecutionMethod.LandownerSimulated;
                var required = quote.Requirements;
                required.Materials = CopyMaterials(task.Materials);
                required.PersonMinutes = CeilRatio(task.Quantity.Amount, profile.MinutesPerReferenceQuantity.Selected, profile.ReferenceQuantity);
                required.OwnerMinutes = owner ? required.PersonMinutes : 0;
                required.PersonMinutesKnown = task.Type != ForestryTaskType.Harvest || profile.MinutesPerReferenceQuantity.Selected > 0;
                required.Capabilities = hasWork ? profile.RequiredCapabilities : WorkCapability.None;
                required.Tools = hasWork ? profile.RequiredTools : WorkTool.None;
                required.LeadDays = hasWork ? (owner ? profile.LandownerLeadDays : profile.ContractorLeadDays) : 0;
                if (hasWork && !(owner ? profile.LandownerAllowed : profile.ContractorAllowed))
                    Issue(issues, RequirementIssue.MethodProhibited, owner ? profile.OwnerRestriction : "Contractor execution is disabled for this work.");
                if (hasWork && !owner && !actor.ContractorAvailable) Issue(issues, RequirementIssue.ContractorUnavailable, "Contractor is unavailable.");
                if ((actor.Capabilities & required.Capabilities) != required.Capabilities)
                    Issue(issues, RequirementIssue.MissingCapability, "Required capability: " + required.Capabilities);
                if ((actor.Tools & required.Tools) != required.Tools)
                    Issue(issues, RequirementIssue.MissingTool, "Required tools: " + required.Tools);
                if (owner && actor.AvailableOwnerMinutes < required.OwnerMinutes)
                    Issue(issues, RequirementIssue.InsufficientLabour, "Owner time budget is insufficient.", required.OwnerMinutes, actor.AvailableOwnerMinutes);
                if (hasWork && profile.RequiresMaterialSpecification && !Array.Exists(task.Materials,
                    x => x.Quantity.Amount > 0 && book.FindMaterial(x.MaterialId).Category == profile.RequiredMaterialCategory))
                    Issue(issues, RequirementIssue.MissingMaterialSpecification, "Specify required " + profile.RequiredMaterialCategory + " materials, owned or purchased; no installed grant proxy is inferred.");

                foreach (var material in required.Materials)
                {
                    if (material.Supply == MaterialSupply.ExistingStock)
                    {
                        var stock = Array.Find(actor.Stock, x => x.MaterialId == material.MaterialId);
                        long available = stock != null && stock.Quantity.Unit == material.Quantity.Unit ? stock.Quantity.Amount : 0;
                        if (available < material.Quantity.Amount)
                            Issue(issues, RequirementIssue.InsufficientStock, material.MaterialId, material.Quantity.Amount, available);
                    }
                    else
                    {
                        var price = book.FindMaterial(material.MaterialId);
                        long cost = RoundRatio(material.Quantity.Amount, price.CentsPerReferenceQuantity.Selected, price.ReferenceQuantity);
                        quote.Costs.MaterialPurchasesCents += cost;
                        Add(ledger, LedgerCategory.Materials, material.MaterialId, -cost);
                    }
                }

                var timber = (TimberBatch[])task.Timber.Clone();
                Array.Sort(timber, (a, b) => StringComparer.Ordinal.Compare(a.BatchId, b.BatchId));
                var valued = new List<TimberValuation>();
                long harvestedGrams = task.UnpricedHarvestGreenGrams, forwardedGrams = task.UnpricedForwardGreenGrams, hauledGrams = 0;
                foreach (var batch in timber)
                {
                    if (batch.Quantity == 0) continue;
                    long grams = batch.Unit == TimberQuantityUnit.GreenGrams ? batch.Quantity :
                        RoundRatio(batch.Quantity, book.FindDensity(batch.DensityId).KilogramsPerCubicMetre.Selected, 1000);
                    if (batch.Quantity > 0 && grams == 0) throw new ArgumentException("Positive timber rounded below one gram; provide a representable quantity.");
                    long price = book.FindTimberPrice(batch.RoadsidePriceId).RoadsideCentsPerTonne.Selected;
                    long reference = RoundRatio(grams, price, GramsPerTonne);
                    var value = new TimberValuation { Batch = CopyBatch(batch), GreenGrams = grams,
                        SelectedRoadsideCentsPerTonne = price, RoadsideReferenceValueCents = reference,
                        SaleRevenueCents = batch.Disposition == TimberDisposition.SellAndExtract ? reference : 0 };
                    valued.Add(value);
                    harvestedGrams += grams;
                    if (batch.ExtractToRoadside) forwardedGrams += grams;
                    if (batch.OwnerPaysHaulage) hauledGrams += grams;
                    quote.GrossRoadsideReferenceValueCents += reference;
                    quote.TimberSaleRevenueCents += value.SaleRevenueCents;
                    if (batch.Disposition == TimberDisposition.KeepForUse) quote.RetainedForUseReferenceValueCents += reference;
                    if (batch.Disposition == TimberDisposition.RetainAsFallenDeadwood) quote.DeadwoodReferenceValueCents += reference;
                    Add(ledger, LedgerCategory.TimberSale, batch.BatchId, value.SaleRevenueCents);
                }
                quote.Timber = valued.ToArray();

                if (hasWork && task.Type == ForestryTaskType.Harvest && !owner)
                    CostHarvest(task, book, harvestedGrams, forwardedGrams, quote, issues, ledger);
                else if (!owner)
                {
                    quote.Costs.ContractorLabourCents = RoundRatio(required.PersonMinutes, book.ContractorLabourCentsPerHour.Selected, 60);
                    Add(ledger, LedgerCategory.ContractorLabour, task.TaskId, -quote.Costs.ContractorLabourCents);
                }
                if (owner)
                {
                    quote.Costs.OwnerEquipmentCents = RoundRatio(required.OwnerMinutes, book.OwnerEquipmentCentsPerHour.Selected, 60);
                    quote.OwnerTimeOpportunityCostCents = RoundRatio(required.OwnerMinutes, book.OwnerTimeValueCentsPerHour.Selected, 60);
                    Add(ledger, LedgerCategory.OwnerEquipment, task.TaskId, -quote.Costs.OwnerEquipmentCents);
                }
                quote.Costs.HaulageCents = RoundRatio(hauledGrams, book.HaulageCentsPerTonne.Selected, GramsPerTonne);
                Add(ledger, LedgerCategory.Haulage, task.TaskId, -quote.Costs.HaulageCents);
                var costs = quote.Costs;
                costs.ContractorWorkCents = costs.CombinedHarvestingForwardingCents + costs.HarvestingCents + costs.ForwardingCents + costs.MinimumJobAdjustmentCents + costs.ContractorLabourCents;
                costs.TotalExternalCostCents = costs.ContractorWorkCents + costs.MaterialPurchasesCents + costs.OwnerEquipmentCents + costs.HaulageCents;
                required.UpfrontCashCents = costs.TotalExternalCostCents;
                if (actor.AvailableCashCents < required.UpfrontCashCents)
                    Issue(issues, RequirementIssue.InsufficientCash, "Upfront cash is required; future timber receipts do not prefinance the job.", required.UpfrontCashCents, actor.AvailableCashCents);
                // An unpriced fell-only job must not advertise a positive net result from receipts alone.
                if (quote.PricingComplete)
                {
                    quote.NetCashCents = quote.TimberSaleRevenueCents - costs.TotalExternalCostCents;
                    quote.NetAfterOwnerTimeCents = quote.NetCashCents - quote.OwnerTimeOpportunityCostCents;
                    quote.EstimatedLedger = ledger.ToArray();
                }
                quote.UnmetRequirements = issues.ToArray();
                quote.Eligible = issues.Count == 0;
                if (SumLedger(quote.EstimatedLedger) != quote.NetCashCents) throw new InvalidOperationException("Ledger does not reconcile.");
                return quote;
            }
        }

        // Pure result calculation. The integrator applies this result only once, after the existing
        // world operation succeeds. Calling twice does not spend cash, consume stock or fell a tree.
        public static WorkResolution Resolve(ForestryTask task, WorkExecutionMethod method, ExecutionResources actor, ForestryPriceBook book)
        {
            var quote = Quote(task, method, actor, book);
            var result = new WorkResolution { Quote = quote, Resolved = quote.Eligible };
            if (!result.Resolved) return result;
            result.ExternalCashFlowCents = quote.NetCashCents;
            result.OwnerMinutesConsumed = quote.Requirements.OwnerMinutes;
            result.Ledger = CopyLedger(quote.EstimatedLedger);
            result.MaterialOutputs = CopyValuations(quote.Timber);
            result.MaterialsConsumed = CopyMaterials(quote.Requirements.Materials);
            return result;
        }

        public static long SumLedger(EconomicLedgerEntry[] ledger)
        {
            long total = 0;
            foreach (var entry in ledger) total = checked(total + entry.CashDeltaCents);
            return total;
        }

        private static void CostHarvest(ForestryTask task, ForestryPriceBook book, long harvested, long forwarded,
            WorkQuote quote, List<UnmetRequirement> issues, List<EconomicLedgerEntry> ledger)
        {
            checked
            {
                var costs = quote.Costs;
                var schedule = book.FindHarvestSchedule(task.HarvestContext);
                if (task.HasHarvestQuote)
                {
                    quote.HarvestPricingSource = task.HarvestQuote.Source;
                    if (forwarded == 0 && task.HarvestQuote.ForwardingCents > 0)
                        throw new ArgumentException("A fell-only quote cannot contain forwarding cost.");
                    costs.HarvestingCents = task.HarvestQuote.HarvestingCents;
                    costs.ForwardingCents = task.HarvestQuote.ForwardingCents;
                }
                else if (schedule.Basis == HarvestCostBasis.CombinedHarvestingAndForwarding)
                {
                    quote.HarvestPricingSource = schedule.CombinedCentsPerTonne.Source;
                    if (harvested != forwarded)
                    {
                        quote.PricingComplete = false;
                        Issue(issues, RequirementIssue.IncompleteHarvestPricing,
                            "Combined harvest/forward evidence cannot price fell-only material. Supply a split schedule or explicit job quote.");
                        return;
                    }
                    costs.CombinedHarvestingForwardingCents = MassCost(harvested, schedule.CombinedCentsPerTonne.Selected, task.SiteCostBasisPoints);
                }
                else
                {
                    quote.HarvestPricingSource = schedule.HarvestingCentsPerTonne.Source + "; " + schedule.ForwardingCentsPerTonne.Source;
                    costs.HarvestingCents = MassCost(harvested, schedule.HarvestingCentsPerTonne.Selected, task.SiteCostBasisPoints);
                    costs.ForwardingCents = MassCost(forwarded, schedule.ForwardingCentsPerTonne.Selected, task.SiteCostBasisPoints);
                }
                long variable = costs.CombinedHarvestingForwardingCents + costs.HarvestingCents + costs.ForwardingCents;
                // A floor, not a mobilisation charge added on top. Once per economic task/job.
                costs.MinimumJobAdjustmentCents = task.HasHarvestQuote ? 0 : Math.Max(0, book.MinimumHarvestJobCents.Selected - variable);
                Add(ledger, LedgerCategory.HarvestingAndForwarding, task.TaskId, -costs.CombinedHarvestingForwardingCents);
                Add(ledger, LedgerCategory.Harvesting, task.TaskId, -costs.HarvestingCents);
                Add(ledger, LedgerCategory.Forwarding, task.TaskId, -costs.ForwardingCents);
                Add(ledger, LedgerCategory.MinimumJobAdjustment, task.TaskId, -costs.MinimumJobAdjustmentCents);
            }
        }

        private static long MassCost(long grams, long centsPerTonne, int siteBasisPoints)
        {
            // Decimal intermediates give exact fixed-unit multiplication without binary floats.
            return checked((long)decimal.Round((decimal)grams * centsPerTonne * siteBasisPoints / (GramsPerTonne * 10000m), 0, MidpointRounding.AwayFromZero));
        }

        private static long RoundRatio(long quantity, long rate, long denominator) =>
            checked((long)decimal.Round((decimal)quantity * rate / denominator, 0, MidpointRounding.AwayFromZero));
        private static long CeilRatio(long quantity, long rate, long denominator) =>
            checked((long)decimal.Ceiling((decimal)quantity * rate / denominator));

        private static void Add(List<EconomicLedgerEntry> ledger, LedgerCategory category, string id, long delta)
        {
            if (delta != 0) ledger.Add(new EconomicLedgerEntry { Category = category, ReferenceId = id, CashDeltaCents = delta });
        }

        private static void Issue(List<UnmetRequirement> issues, RequirementIssue issue, string detail, long required = 0, long available = 0) =>
            issues.Add(new UnmetRequirement { Issue = issue, Detail = detail, Required = required, Available = available });

        private static WorkQuantity CopyQuantity(WorkQuantity value) => new WorkQuantity { Unit = value.Unit, Amount = value.Amount };
        private static WorldOperationDescription Describe(ForestryTask task)
        {
            var targets = (string[])task.TargetIds.Clone(); Array.Sort(targets, StringComparer.Ordinal);
            return new WorldOperationDescription { TaskId = task.TaskId, WorldOperationId = task.WorldOperationId,
                Type = task.Type, Quantity = CopyQuantity(task.Quantity), TargetIds = targets };
        }
        private static TimberBatch CopyBatch(TimberBatch value) => new TimberBatch { BatchId = value.BatchId, SourceTreeId = value.SourceTreeId,
            SpeciesId = value.SpeciesId, Assortment = value.Assortment, Unit = value.Unit, Quantity = value.Quantity, DensityId = value.DensityId,
            RoadsidePriceId = value.RoadsidePriceId, Disposition = value.Disposition, ExtractToRoadside = value.ExtractToRoadside, OwnerPaysHaulage = value.OwnerPaysHaulage };
        private static MaterialRequirement[] CopyMaterials(MaterialRequirement[] values)
        {
            var result = Array.ConvertAll(values, x => new MaterialRequirement { MaterialId = x.MaterialId, Quantity = CopyQuantity(x.Quantity), Supply = x.Supply });
            Array.Sort(result, (a, b) => StringComparer.Ordinal.Compare(a.MaterialId, b.MaterialId)); return result;
        }
        private static EconomicLedgerEntry[] CopyLedger(EconomicLedgerEntry[] values) => Array.ConvertAll(values,
            x => new EconomicLedgerEntry { Category = x.Category, ReferenceId = x.ReferenceId, CashDeltaCents = x.CashDeltaCents });
        private static TimberValuation[] CopyValuations(TimberValuation[] values) => Array.ConvertAll(values,
            x => new TimberValuation { Batch = CopyBatch(x.Batch), GreenGrams = x.GreenGrams, SelectedRoadsideCentsPerTonne = x.SelectedRoadsideCentsPerTonne,
                RoadsideReferenceValueCents = x.RoadsideReferenceValueCents, SaleRevenueCents = x.SaleRevenueCents });
    }
}

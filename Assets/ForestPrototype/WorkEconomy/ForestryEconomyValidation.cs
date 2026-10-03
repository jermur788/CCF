using System;
using System.Collections.Generic;

namespace CCF.Forestry.WorkEconomy
{
    // Invalid data is a programming/configuration error (ArgumentException).
    // Valid but unavailable work is reported through UnmetRequirement instead.
    public static class ForestryEconomyValidation
    {
        public static void Validate(ForestryTask task, WorkExecutionMethod method, ExecutionResources resources, ForestryPriceBook book)
        {
            Require(task != null && resources != null && book != null, "Task, actor resources and price book are required.");
            Defined(method); Defined(task.Type); Defined(task.HarvestContext);
            Id(task.TaskId); Id(task.WorldOperationId); Id(book.Id);
            Require(task.SiteCostBasisPoints > 0, "Site cost multiplier must be positive.");
            Nonnegative(resources.AvailableCashCents); Nonnegative(resources.AvailableOwnerMinutes);
            Capabilities(resources.Capabilities); Tools(resources.Tools);
            Quantity(task.Quantity);
            Nonnegative(task.UnpricedHarvestGreenGrams); Nonnegative(task.UnpricedForwardGreenGrams);
            Require(task.UnpricedForwardGreenGrams <= task.UnpricedHarvestGreenGrams, "Unpriced forwarding exceeds harvested work quantity.");
            Require(task.Type == ForestryTaskType.Harvest || task.UnpricedHarvestGreenGrams == 0, "Unpriced harvest quantity belongs to harvest work only.");
            Require(task.Quantity.Amount > 0 || task.UnpricedHarvestGreenGrams == 0, "Zero work cannot carry unpriced harvested quantity.");
            Unique(task.TargetIds, x => x);
            Unique(book.TimberPrices, x => x.Id);
            foreach (var price in book.TimberPrices)
            {
                Id(price.SpeciesId); Defined(price.Assortment);
                Parameter(price.RoadsideCentsPerTonne, ParameterUnit.CentsPerTonne);
            }
            Unique(book.Densities, x => x.Id);
            foreach (var density in book.Densities)
            {
                Id(density.SpeciesId); Id(density.MoistureCondition);
                Parameter(density.KilogramsPerCubicMetre, ParameterUnit.KilogramsPerCubicMetre);
                Require(density.KilogramsPerCubicMetre.Selected > 0, "Selected density must be positive.");
            }
            Unique(book.Materials, x => x.Id);
            foreach (var material in book.Materials)
            {
                Defined(material.Category); Defined(material.QuantityUnit);
                Require(material.ReferenceQuantity > 0, "Material reference quantity must be positive.");
                Parameter(material.CentsPerReferenceQuantity, ParameterUnit.CentsPerReferenceQuantity);
            }
            Unique(book.WorkProfiles, x => x.Type.ToString());
            foreach (var profile in book.WorkProfiles)
            {
                Defined(profile.Type); Defined(profile.QuantityUnit); Defined(profile.RequiredMaterialCategory); Capabilities(profile.RequiredCapabilities); Tools(profile.RequiredTools);
                Require(profile.ReferenceQuantity > 0 && profile.ContractorLeadDays >= 0 && profile.LandownerLeadDays >= 0, "Invalid work reference quantity/lead days.");
                Parameter(profile.MinutesPerReferenceQuantity, ParameterUnit.MinutesPerReferenceQuantity);
                Require(profile.MinutesPerReferenceQuantity.Selected > 0 || (profile.Type == ForestryTaskType.Harvest && !profile.LandownerAllowed), "Eligible owner/non-harvest work needs a positive time estimate.");
            }
            Unique(book.HarvestSchedules, x => x.Context.ToString());
            foreach (var schedule in book.HarvestSchedules)
            {
                Defined(schedule.Context); Defined(schedule.Basis);
                Parameter(schedule.CombinedCentsPerTonne, ParameterUnit.CentsPerTonne);
                Parameter(schedule.HarvestingCentsPerTonne, ParameterUnit.CentsPerTonne);
                Parameter(schedule.ForwardingCentsPerTonne, ParameterUnit.CentsPerTonne);
            }
            Parameter(book.MinimumHarvestJobCents, ParameterUnit.CentsPerJob);
            Parameter(book.HaulageCentsPerTonne, ParameterUnit.CentsPerTonne);
            Parameter(book.ContractorLabourCentsPerHour, ParameterUnit.CentsPerHour);
            Parameter(book.OwnerEquipmentCentsPerHour, ParameterUnit.CentsPerHour);
            Parameter(book.OwnerTimeValueCentsPerHour, ParameterUnit.CentsPerHour);
            var work = book.FindProfile(task.Type);
            Require(work != null && work.QuantityUnit == task.Quantity.Unit, "Task needs a matching work profile and quantity unit.");

            Unique(resources.Stock, x => x.MaterialId);
            foreach (var stock in resources.Stock) Quantity(stock.Quantity);
            Unique(task.Materials, x => x.MaterialId);
            long nurseryQuantity = 0;
            long shelterQuantity = 0;
            foreach (var material in task.Materials)
            {
                Quantity(material.Quantity); Defined(material.Supply);
                var price = book.FindMaterial(material.MaterialId);
                Require(price != null && price.QuantityUnit == material.Quantity.Unit, "Material needs a matching configured unit/price.");
                Require(task.Quantity.Amount > 0 || material.Quantity.Amount == 0, "Zero work cannot consume materials.");
                if (price.Category == MaterialCategory.NurseryStock) nurseryQuantity = checked(nurseryQuantity + material.Quantity.Amount);
                if (price.Category == MaterialCategory.Shelter) shelterQuantity = checked(shelterQuantity + material.Quantity.Amount);
                if (price.Category == MaterialCategory.NurseryStock || price.Category == MaterialCategory.Shelter)
                    Require(price.QuantityUnit == WorkQuantityUnit.Items, "Plants and shelters use whole items.");
            }
            if (task.Type == ForestryTaskType.Planting) Require(nurseryQuantity == task.Quantity.Amount, "Planting needs one nursery-stock item per planted individual.");
            if (task.Type == ForestryTaskType.ShelterInstallation) Require(shelterQuantity == task.Quantity.Amount, "Shelter work needs one shelter item per individual.");

            Unique(task.Timber, x => x.BatchId);
            bool hasTimberQuantity = false;
            foreach (var batch in task.Timber)
            {
                Id(batch.SourceTreeId); Id(batch.SpeciesId); Id(batch.RoadsidePriceId);
                Defined(batch.Assortment); Defined(batch.Unit); Defined(batch.Disposition); Nonnegative(batch.Quantity);
                Require(task.Type == ForestryTaskType.Harvest, "Only harvest work can produce timber.");
                Require(task.Quantity.Amount > 0 || batch.Quantity == 0, "Zero work cannot produce timber.");
                Require(task.TargetIds.Length == 0 || Array.IndexOf(task.TargetIds, batch.SourceTreeId) >= 0, "Timber source is outside the prescribed targets.");
                var price = book.FindTimberPrice(batch.RoadsidePriceId);
                Require(price != null && price.Assortment == batch.Assortment && price.SpeciesId == batch.SpeciesId, "Timber price must match assortment and species.");
                if (batch.Unit == TimberQuantityUnit.CubicCentimetres)
                {
                    var density = book.FindDensity(batch.DensityId);
                    Require(density != null && density.SpeciesId == batch.SpeciesId, "Volume conversion needs an explicit matching species/moisture density.");
                }
                Require(batch.Disposition != TimberDisposition.SellAndExtract || batch.ExtractToRoadside, "Roadside sale requires extraction.");
                Require(batch.Disposition != TimberDisposition.RetainAsFallenDeadwood || !batch.ExtractToRoadside, "Fallen deadwood is not forwarded to roadside.");
                Require(!batch.OwnerPaysHaulage || batch.ExtractToRoadside, "Owner-paid haulage requires extracted timber.");
                // Inputs may mix mass and volume; do not sum incompatible raw units here.
                hasTimberQuantity |= batch.Quantity > 0;
            }
            if (task.Type == ForestryTaskType.Harvest)
            {
                Require(book.FindHarvestSchedule(task.HarvestContext) != null, "Harvest context has no cost schedule.");
                Require(task.Quantity.Amount == 0 || hasTimberQuantity || task.UnpricedHarvestGreenGrams > 0, "Nonzero harvest needs quantified produced timber or unpriced work quantity.");
            }
            else Require(!task.HasHarvestQuote, "Harvest quotes apply only to harvest work.");
            if (task.HasHarvestQuote)
            {
                Require(task.HarvestQuote != null, "Enabled harvest quote is missing.");
                Id(task.HarvestQuote.Source); Nonnegative(task.HarvestQuote.HarvestingCents); Nonnegative(task.HarvestQuote.ForwardingCents);
                Require(task.Quantity.Amount > 0 || (task.HarvestQuote.HarvestingCents == 0 && task.HarvestQuote.ForwardingCents == 0), "Zero work cannot carry a harvest charge.");
            }
        }

        private static void Parameter(EconomicParameter parameter, ParameterUnit unit)
        {
            Require(parameter != null, "Economic parameter missing.");
            Defined(parameter.Unit); Defined(parameter.RangeEvidence); Defined(parameter.SelectedEvidence);
            Require(parameter.Unit == unit, "Economic parameter has the wrong unit.");
            Require(parameter.Low >= 0 && parameter.Low <= parameter.Reference && parameter.Reference <= parameter.High && parameter.Selected >= 0 && parameter.SourceYear >= 0, "Invalid economic range/value/year.");
            Id(parameter.Source);
        }

        private static void Quantity(WorkQuantity quantity)
        {
            Require(quantity != null, "Quantity missing."); Defined(quantity.Unit); Nonnegative(quantity.Amount);
        }

        private static void Unique<T>(T[] values, Func<T, string> key)
        {
            Require(values != null, "Arrays must be present (empty is allowed).");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in values)
            {
                Require(value != null, "Null array entry.");
                string id = key(value); Id(id);
                Require(ids.Add(id), "Duplicate identifier: " + id);
            }
        }

        private static void Capabilities(WorkCapability value) => Require(((int)value & ~63) == 0, "Unknown capability flag.");
        private static void Tools(WorkTool value) => Require(((int)value & ~511) == 0, "Unknown tool flag.");
        private static void Defined<T>(T value) => Require(Enum.IsDefined(typeof(T), value), "Unknown enum value: " + value);
        private static void Nonnegative(long value) => Require(value >= 0, "Negative input quantity/cost/resource.");
        private static void Id(string value) => Require(!string.IsNullOrWhiteSpace(value), "Identifier/provenance cannot be blank.");
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new ArgumentException(message);
        }
    }
}

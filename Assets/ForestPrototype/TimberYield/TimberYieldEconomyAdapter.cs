using System;
using System.Collections.Generic;
using CCF.Forestry.WorkEconomy;

namespace CCF.Forestry.TimberYield
{
    [Serializable]
    public sealed class TimberMarketBinding
    {
        public string SpeciesId = "";
        public TimberAssortment Assortment;
        public string RoadsidePriceId = "";
        public string DensityId = ""; // Explicit caller-selected species/product/moisture conversion.
    }

    // IDs are supplied by the integration layer. This adapter contains no prices, density
    // values, operating costs, labour rules or wallet mutation. Residuals are never sold.
    public static class TimberYieldEconomyAdapter
    {
        public static TimberBatch[] ToTimberBatches(StandOperationYield operation, TimberMarketBinding[] bindings)
        {
            TimberYieldValidation.Require(operation != null && operation.Stems != null && bindings != null, "Adapter inputs missing.");
            TimberYieldValidation.Id(operation.OperationId);
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var binding in bindings)
            {
                TimberYieldValidation.Require(binding != null, "Null market binding."); TimberYieldValidation.Id(binding.SpeciesId);
                TimberYieldValidation.Defined(binding.Assortment); TimberYieldValidation.Id(binding.RoadsidePriceId); TimberYieldValidation.Id(binding.DensityId);
                TimberYieldValidation.Require(keys.Add(binding.SpeciesId + "/" + (int)binding.Assortment), "Duplicate market binding.");
            }
            var batches = new List<TimberBatch>(); var logIds = new HashSet<string>(StringComparer.Ordinal);
            long operationVolume = 0, saleVolume = 0, retainedVolume = 0, deadwoodVolume = 0, residualVolume = 0;
            var treeIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var stem in operation.Stems)
            {
                TimberYieldValidation.Require(stem != null && stem.Logs != null, "Null stem/log list.");
                TimberYieldValidation.Id(stem.TreeId); TimberYieldValidation.Id(stem.SpeciesId);
                TimberYieldValidation.Require(treeIds.Add(stem.TreeId) && stem.StemVolumeCm3 >= 0 && stem.ResidualVolumeCm3 >= 0, "Invalid/duplicate stem result.");
                long stemSale = 0, stemRetained = 0, stemDeadwood = 0;
                foreach (var log in stem.Logs)
                {
                    TimberYieldValidation.Require(log != null && log.VolumeCm3 > 0, "Invalid allocated log."); TimberYieldValidation.Id(log.LogId);
                    TimberYieldValidation.Defined(log.Assortment); TimberYieldValidation.Defined(log.Disposition);
                    TimberYieldValidation.Require(log.TreeId == stem.TreeId && log.SpeciesId == stem.SpeciesId && log.StartHeightMm >= 0 && log.EndHeightMm > log.StartHeightMm && log.EndHeightMm <= stem.TotalHeightMm, "Log provenance/section mismatch.");
                    if (log.Disposition == TimberDisposition.SellAndExtract) stemSale = checked(stemSale + log.VolumeCm3);
                    else if (log.Disposition == TimberDisposition.KeepForUse) stemRetained = checked(stemRetained + log.VolumeCm3);
                    else stemDeadwood = checked(stemDeadwood + log.VolumeCm3);
                    TimberYieldValidation.Require(logIds.Add(log.LogId), "Duplicate log ID.");
                    var binding = Array.Find(bindings, x => x.SpeciesId == log.SpeciesId && x.Assortment == log.Assortment);
                    TimberYieldValidation.Require(binding != null, "No explicit species/assortment market binding.");
                    batches.Add(new TimberBatch { BatchId = operation.OperationId + "/" + log.LogId, SourceTreeId = log.TreeId, SpeciesId = log.SpeciesId,
                        Assortment = log.Assortment, Unit = TimberQuantityUnit.CubicCentimetres, Quantity = log.VolumeCm3,
                        RoadsidePriceId = binding.RoadsidePriceId, DensityId = binding.DensityId, Disposition = log.Disposition,
                        ExtractToRoadside = log.ExtractToRoadside, OwnerPaysHaulage = log.OwnerPaysHaulage });
                }
                TimberYieldValidation.Require(stemSale == stem.SaleAssortmentVolumeCm3 && stemRetained == stem.RetainedForUseVolumeCm3 && stemDeadwood == stem.DeadwoodAssortmentVolumeCm3 &&
                    checked(stemSale + stemRetained + stemDeadwood + stem.ResidualVolumeCm3) == stem.StemVolumeCm3, "Stem result does not conserve material.");
                operationVolume = checked(operationVolume + stem.StemVolumeCm3); saleVolume = checked(saleVolume + stemSale);
                retainedVolume = checked(retainedVolume + stemRetained); deadwoodVolume = checked(deadwoodVolume + stemDeadwood); residualVolume = checked(residualVolume + stem.ResidualVolumeCm3);
            }
            TimberYieldValidation.Require(operationVolume == operation.StemVolumeCm3 && saleVolume == operation.SaleAssortmentVolumeCm3 && retainedVolume == operation.RetainedForUseVolumeCm3 &&
                deadwoodVolume == operation.DeadwoodAssortmentVolumeCm3 && residualVolume == operation.ResidualVolumeCm3, "Operation result totals disagree with stems.");
            batches.Sort((a, b) => StringComparer.Ordinal.Compare(a.BatchId, b.BatchId)); return batches.ToArray();
        }
    }
}

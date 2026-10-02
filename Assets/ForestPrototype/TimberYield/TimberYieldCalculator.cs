using System;
using System.Collections.Generic;
using System.Globalization;
using CCF.Forestry.WorkEconomy;

namespace CCF.Forestry.TimberYield
{
    public static class TimberYieldCalculator
    {
        public static StemYieldResult ResolveSingleStem(StemYieldRequest request, TimberYieldConfiguration config)
        {
            TimberYieldValidation.Validate(request, config);
            var stem = request.Stem; var model = new MerchantableStemModel(stem, config.BreastHeightMm);
            var specs = new List<AssortmentSpecification>();
            foreach (var spec in config.Specifications) if (spec.SpeciesId == stem.SpeciesId) specs.Add(spec);
            specs.Sort((a, b) => { int order = a.Priority.CompareTo(b.Priority); return order != 0 ? order : StringComparer.Ordinal.Compare(a.Id, b.Id); });
            var lengths = new Dictionary<string, int[]>(StringComparer.Ordinal);
            int minimumDiameter = int.MaxValue;
            foreach (var spec in specs)
            {
                minimumDiameter = Math.Min(minimumDiameter, spec.MinimumSmallEndDiameterMm);
                var choices = (int[])spec.NominalLengthsMm.Clone(); Array.Sort(choices);
                if (config.LengthPreference == LogLengthPreference.LongestFirst) Array.Reverse(choices);
                lengths.Add(spec.Id, choices);
            }
            var result = new StemYieldResult { TreeId = stem.TreeId, SpeciesId = stem.SpeciesId, ConfigurationId = config.Id,
                MeasurementBasis = stem.MeasurementBasis, ProfileKind = stem.ProfileKind, StemVolumeCm3 = stem.StemVolumeCm3, DbhMm = stem.DbhMm, TotalHeightMm = stem.TotalHeightMm,
                GeometricProfileVolumeCm3 = model.GeometricVolumeCm3,
                MerchantableTopHeightMm = specs.Count == 0 ? 0 : model.HeightToDiameter(minimumDiameter) };
            var logs = new List<AllocatedTimberLog>(); var residual = new List<StemResidualSection>();
            int position = Math.Min(config.StumpHeightMm, stem.TotalHeightMm);
            AddResidual(residual, model, StemResidualKind.Stump, 0, position);
            while (position < stem.TotalHeightMm)
            {
                if (stem.StemVolumeCm3 == 0 || specs.Count == 0 || position >= result.MerchantableTopHeightMm)
                {
                    AddResidual(residual, model, specs.Count == 0 ? StemResidualKind.Unallocated : StemResidualKind.BelowMerchantableDiameter, position, stem.TotalHeightMm); break;
                }
                AssortmentSpecification selected = null; int end = position; StemQualityFlags quality = stem.Quality;
                foreach (var spec in specs)
                {
                    if (position < spec.MinimumStartHeightMm) continue;
                    foreach (int length in lengths[spec.Id])
                    {
                        result.CandidateChecks = checked(result.CandidateChecks + 1);
                        if (length > stem.TotalHeightMm - position) continue;
                        int candidateEnd = position + length;
                        if (spec.MaximumEndHeightMm > 0 && candidateEnd > spec.MaximumEndHeightMm) continue;
                        decimal smallSquared = model.DiameterSquaredAt(candidateEnd), largeSquared = model.DiameterSquaredAt(position);
                        if (smallSquared < (decimal)spec.MinimumSmallEndDiameterMm * spec.MinimumSmallEndDiameterMm ||
                            (spec.MaximumSmallEndDiameterMm > 0 && smallSquared > (decimal)spec.MaximumSmallEndDiameterMm * spec.MaximumSmallEndDiameterMm) ||
                            (spec.MaximumLargeEndDiameterMm > 0 && largeSquared > (decimal)spec.MaximumLargeEndDiameterMm * spec.MaximumLargeEndDiameterMm)) continue;
                        var anyQuality = QualityUnion(stem, position, candidateEnd);
                        if ((anyQuality & spec.ExcludedQuality) != 0) continue;
                        if (spec.RequiredQuality != StemQualityFlags.None && (QualityThroughout(stem, position, candidateEnd) & spec.RequiredQuality) != spec.RequiredQuality) continue;
                        selected = spec; end = candidateEnd; quality = anyQuality; break;
                    }
                    if (selected != null) break;
                }
                if (selected == null)
                {
                    int next = position + Math.Min(config.UnallocatedScanStepMm, stem.TotalHeightMm - position);
                    AddResidual(residual, model, StemResidualKind.Unallocated, position, next); position = next; continue;
                }
                long volume = model.VolumeBetween(position, end);
                if (volume == 0) AddResidual(residual, model, StemResidualKind.SubUnitAllocation, position, end);
                else
                {
                    var disposition = request.DefaultDisposition;
                    foreach (var rule in request.DispositionOverrides) if (rule.Assortment == selected.Assortment) disposition = rule.Disposition;
                    bool extracted = disposition == TimberDisposition.SellAndExtract || (disposition == TimberDisposition.KeepForUse && request.ExtractRetainedToRoadside);
                    var log = new AllocatedTimberLog { LogId = stem.TreeId + "/log/" + position.ToString(CultureInfo.InvariantCulture), TreeId = stem.TreeId, SpeciesId = stem.SpeciesId,
                        SpecificationId = selected.Id, Assortment = selected.Assortment, PotentialUse = selected.PotentialUse,
                        StartHeightMm = position, EndHeightMm = end, SmallEndDiameterMm = model.DiameterFloorAt(end), LargeEndDiameterMm = model.DiameterFloorAt(position),
                        VolumeCm3 = volume, Quality = quality, Disposition = disposition, ExtractToRoadside = extracted, OwnerPaysHaulage = extracted && request.OwnerPaysHaulage };
                    logs.Add(log);
                    if (disposition == TimberDisposition.SellAndExtract) result.SaleAssortmentVolumeCm3 = checked(result.SaleAssortmentVolumeCm3 + volume);
                    else if (disposition == TimberDisposition.KeepForUse) result.RetainedForUseVolumeCm3 = checked(result.RetainedForUseVolumeCm3 + volume);
                    else result.DeadwoodAssortmentVolumeCm3 = checked(result.DeadwoodAssortmentVolumeCm3 + volume);
                }
                int afterCut = end + Math.Min(config.CuttingLossMm, stem.TotalHeightMm - end);
                AddResidual(residual, model, StemResidualKind.CuttingLoss, end, afterCut); position = afterCut;
            }
            foreach (var part in residual) result.ResidualVolumeCm3 = checked(result.ResidualVolumeCm3 + part.VolumeCm3);
            result.Logs = logs.ToArray(); result.ResidualSections = residual.ToArray();
            if (checked(result.SaleAssortmentVolumeCm3 + result.RetainedForUseVolumeCm3 + result.DeadwoodAssortmentVolumeCm3 + result.ResidualVolumeCm3) != result.StemVolumeCm3)
                throw new InvalidOperationException("Stem material does not conserve.");
            return result;
        }

        public static StandOperationYield ResolveStandOperation(string operationId, StemYieldRequest[] requests, TimberYieldConfiguration config)
        {
            TimberYieldValidation.Id(operationId); TimberYieldValidation.Require(requests != null && config != null, "Operation inputs missing.");
            TimberYieldValidation.ValidateConfiguration(config);
            var ordered = (StemYieldRequest[])requests.Clone();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var request in ordered)
            {
                TimberYieldValidation.Validate(request, config);
                TimberYieldValidation.Require(ids.Add(request.Stem.TreeId), "Duplicate tree in operation.");
            }
            Array.Sort(ordered, (a, b) => StringComparer.Ordinal.Compare(a.Stem.TreeId, b.Stem.TreeId));
            var result = new StandOperationYield { OperationId = operationId, ConfigurationId = config.Id };
            var stems = new List<StemYieldResult>(); var summaries = new List<AssortmentYieldSummary>();
            foreach (var request in ordered)
            {
                var stem = ResolveSingleStem(request, config); stems.Add(stem);
                result.StemVolumeCm3 = checked(result.StemVolumeCm3 + stem.StemVolumeCm3);
                result.SaleAssortmentVolumeCm3 = checked(result.SaleAssortmentVolumeCm3 + stem.SaleAssortmentVolumeCm3);
                result.RetainedForUseVolumeCm3 = checked(result.RetainedForUseVolumeCm3 + stem.RetainedForUseVolumeCm3);
                result.DeadwoodAssortmentVolumeCm3 = checked(result.DeadwoodAssortmentVolumeCm3 + stem.DeadwoodAssortmentVolumeCm3);
                result.ResidualVolumeCm3 = checked(result.ResidualVolumeCm3 + stem.ResidualVolumeCm3);
                foreach (var log in stem.Logs)
                {
                    var summary = summaries.Find(x => x.SpeciesId == log.SpeciesId && x.Assortment == log.Assortment && x.Disposition == log.Disposition);
                    if (summary == null) { summary = new AssortmentYieldSummary { SpeciesId = log.SpeciesId, Assortment = log.Assortment, Disposition = log.Disposition }; summaries.Add(summary); }
                    summary.VolumeCm3 = checked(summary.VolumeCm3 + log.VolumeCm3); summary.Pieces = checked(summary.Pieces + 1);
                }
            }
            summaries.Sort((a, b) => { int order = StringComparer.Ordinal.Compare(a.SpeciesId, b.SpeciesId); if (order != 0) return order;
                order = a.Assortment.CompareTo(b.Assortment); return order != 0 ? order : a.Disposition.CompareTo(b.Disposition); });
            result.Stems = stems.ToArray(); result.Assortments = summaries.ToArray(); return result;
        }

        private static StemQualityFlags QualityUnion(StemMeasurements stem, int start, int end)
        {
            var flags = stem.Quality;
            foreach (var section in stem.QualitySections) if (section.StartHeightMm < end && section.EndHeightMm > start) flags |= section.Flags;
            return flags;
        }
        private static StemQualityFlags QualityThroughout(StemMeasurements stem, int start, int end)
        {
            var boundaries = new List<int> { start };
            foreach (var section in stem.QualitySections)
            {
                if (section.StartHeightMm > start && section.StartHeightMm < end) boundaries.Add(section.StartHeightMm);
                if (section.EndHeightMm > start && section.EndHeightMm < end) boundaries.Add(section.EndHeightMm);
            }
            var common = (StemQualityFlags)63;
            foreach (int point in boundaries)
            {
                var flags = stem.Quality;
                foreach (var section in stem.QualitySections) if (section.StartHeightMm <= point && section.EndHeightMm > point) flags |= section.Flags;
                common &= flags;
            }
            return common;
        }
        private static void AddResidual(List<StemResidualSection> values, MerchantableStemModel model, StemResidualKind kind, int start, int end)
        {
            if (end <= start) return;
            long volume = model.VolumeBetween(start, end);
            if (values.Count > 0 && values[values.Count - 1].Kind == kind && values[values.Count - 1].EndHeightMm == start)
            {
                var previous = values[values.Count - 1]; previous.EndHeightMm = end; previous.VolumeCm3 = checked(previous.VolumeCm3 + volume);
            }
            else values.Add(new StemResidualSection { Kind = kind, StartHeightMm = start, EndHeightMm = end, VolumeCm3 = volume });
        }
    }
}

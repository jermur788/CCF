using System;
using System.Collections.Generic;
using CCF.Forestry.WorkEconomy;

namespace CCF.Forestry.TimberYield
{
    public static class TimberYieldValidation
    {
        public static void Validate(StemYieldRequest request, TimberYieldConfiguration config)
        {
            Require(request != null, "Yield request missing."); ValidateConfiguration(config);
            ValidateStem(request.Stem, config.BreastHeightMm);
            Require(request.Stem.TotalHeightMm <= config.MaximumStemHeightMm && request.Stem.DbhMm <= config.MaximumDbhMm, "Stem exceeds configured engineering limits.");
            Defined(request.DefaultDisposition);
            Require(request.DispositionOverrides != null, "Disposition overrides missing.");
            var overrides = new HashSet<TimberAssortment>();
            foreach (var rule in request.DispositionOverrides)
            {
                Require(rule != null, "Null disposition rule."); Defined(rule.Assortment); Defined(rule.Disposition);
                Require(overrides.Add(rule.Assortment), "Duplicate disposition override.");
            }
        }

        public static void ValidateConfiguration(TimberYieldConfiguration config)
        {
            Require(config != null, "Yield configuration missing."); Id(config.Id); Id(config.ModelSource);
            Defined(config.Strategy); Defined(config.LengthPreference);
            Require(config.BreastHeightMm > 0 && config.StumpHeightMm >= 0 && config.CuttingLossMm >= 0 && config.UnallocatedScanStepMm > 0, "Invalid stem processing settings.");
            Require(config.MaximumStemHeightMm > config.BreastHeightMm && config.MaximumDbhMm > 0, "Invalid engineering limits.");
            Require(config.Specifications != null, "Specifications missing.");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var spec in config.Specifications)
            {
                Require(spec != null, "Null specification."); Id(spec.Id); Id(spec.SpeciesId); Id(spec.Source);
                Require(ids.Add(spec.Id), "Duplicate specification ID."); Defined(spec.Assortment); Defined(spec.PotentialUse);
                Defined(spec.DimensionEvidence); Defined(spec.SelectionEvidence); Flags(spec.RequiredQuality); Flags(spec.ExcludedQuality);
                Require((spec.RequiredQuality & spec.ExcludedQuality) == 0, "Required quality is excluded.");
                Require(spec.Priority >= 0 && spec.MinimumSmallEndDiameterMm > 0 && spec.MaximumSmallEndDiameterMm >= 0 && spec.MaximumLargeEndDiameterMm >= 0, "Invalid priority/diameter specification.");
                Require(spec.MaximumSmallEndDiameterMm == 0 || spec.MaximumSmallEndDiameterMm >= spec.MinimumSmallEndDiameterMm, "Reversed diameter bounds.");
                Require(spec.MaximumLargeEndDiameterMm == 0 || spec.MaximumLargeEndDiameterMm >= spec.MinimumSmallEndDiameterMm, "Large-end maximum below small-end minimum.");
                Require(spec.MinimumLengthMm > 0 && spec.MaximumLengthMm >= spec.MinimumLengthMm, "Invalid log length range.");
                Require(spec.MinimumStartHeightMm >= 0 && (spec.MaximumEndHeightMm == 0 || spec.MaximumEndHeightMm > spec.MinimumStartHeightMm), "Invalid section bounds.");
                Require(spec.NominalLengthsMm != null && spec.NominalLengthsMm.Length > 0, "Published/configured nominal lengths required.");
                var lengths = new HashSet<int>();
                foreach (int length in spec.NominalLengthsMm)
                    Require(length >= spec.MinimumLengthMm && length <= spec.MaximumLengthMm && lengths.Add(length), "Invalid or duplicate nominal length.");
            }
        }

        public static void ValidateStem(StemMeasurements stem, int breastHeightMm)
        {
            Require(stem != null, "Stem missing."); Id(stem.TreeId); Id(stem.SpeciesId); Id(stem.MeasurementBasis); Defined(stem.ProfileKind); Flags(stem.Quality);
            Require(breastHeightMm > 0 && stem.TotalHeightMm > breastHeightMm && stem.DbhMm > 0 && stem.StemVolumeCm3 >= 0, "Invalid DBH/height/volume; DBH requires a stem taller than breast height.");
            Require(stem.QualitySections != null && stem.ProfilePoints != null, "Stem arrays missing.");
            foreach (var section in stem.QualitySections)
            {
                Require(section != null, "Null quality section."); Flags(section.Flags);
                Require(section.StartHeightMm >= 0 && section.EndHeightMm <= stem.TotalHeightMm && section.EndHeightMm > section.StartHeightMm, "Quality section outside stem.");
            }
            if (stem.ProfileKind == StemProfileKind.VolumeBudgetedParaboloid)
            {
                Require(stem.ProfilePoints.Length == 0, "Measured points require explicitly selected measured-profile mode."); return;
            }
            Require(stem.ProfilePoints.Length >= 2, "Measured profile needs base and tip.");
            for (int i = 0; i < stem.ProfilePoints.Length; i++)
            {
                var point = stem.ProfilePoints[i]; Require(point != null && point.DiameterMm >= 0, "Invalid measured diameter.");
                Require(point.HeightMm >= 0 && point.HeightMm <= stem.TotalHeightMm, "Measured height outside stem.");
                if (i > 0) Require(point.HeightMm > stem.ProfilePoints[i - 1].HeightMm && point.DiameterMm <= stem.ProfilePoints[i - 1].DiameterMm, "V1 measured profile must be strictly height-ordered and non-increasing in diameter.");
            }
            Require(stem.ProfilePoints[0].HeightMm == 0 && stem.ProfilePoints[stem.ProfilePoints.Length - 1].HeightMm == stem.TotalHeightMm &&
                stem.ProfilePoints[stem.ProfilePoints.Length - 1].DiameterMm == 0, "Measured profile must cover base to zero-diameter tip.");
            // Reject a profile on a different bark/measurement basis rather than rescaling DBH silently.
            for (int i = 1; i < stem.ProfilePoints.Length; i++)
            {
                var a = stem.ProfilePoints[i - 1]; var b = stem.ProfilePoints[i]; if (breastHeightMm > b.HeightMm) continue;
                decimal squared = (decimal)a.DiameterMm * a.DiameterMm + ((decimal)b.DiameterMm * b.DiameterMm - (decimal)a.DiameterMm * a.DiameterMm) * (breastHeightMm - a.HeightMm) / (b.HeightMm - a.HeightMm);
                decimal low = Math.Max(0, stem.DbhMm - 1), high = (decimal)stem.DbhMm + 1;
                Require(squared >= low * low && squared <= high * high, "Measured profile differs from supplied DBH by more than the 1 mm quantization allowance."); break;
            }
        }

        internal static void Require(bool condition, string message) { if (!condition) throw new ArgumentException(message); }
        internal static void Id(string value) => Require(!string.IsNullOrWhiteSpace(value), "Identifier/provenance cannot be blank.");
        internal static void Defined<T>(T value) => Require(Enum.IsDefined(typeof(T), value), "Unknown enum: " + value);
        private static void Flags(StemQualityFlags value) => Require(((int)value & ~63) == 0, "Unknown quality flag.");
    }
}

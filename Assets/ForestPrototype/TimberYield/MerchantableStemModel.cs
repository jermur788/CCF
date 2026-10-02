using System;

namespace CCF.Forestry.TimberYield
{
    // No growth simulation: immutable local shape/volume snapshot used only while resolving yield.
    public sealed class MerchantableStemModel
    {
        private readonly int dbh, height, breastHeight;
        private readonly long volume;
        private readonly StemProfilePoint[] points;
        private readonly decimal totalAreaIntegral;
        public StemProfileKind Kind { get; }

        public MerchantableStemModel(StemMeasurements stem, int breastHeightMm)
        {
            TimberYieldValidation.ValidateStem(stem, breastHeightMm);
            dbh = stem.DbhMm; height = stem.TotalHeightMm; breastHeight = breastHeightMm; volume = stem.StemVolumeCm3; Kind = stem.ProfileKind;
            points = Array.ConvertAll(stem.ProfilePoints, x => new StemProfilePoint { HeightMm = x.HeightMm, DiameterMm = x.DiameterMm });
            totalAreaIntegral = Kind == StemProfileKind.VolumeBudgetedParaboloid ? (decimal)dbh * dbh * height * height / (2m * (height - breastHeight)) : IntegrateMeasured(height);
        }

        public decimal DiameterSquaredAt(int heightMm)
        {
            CheckHeight(heightMm);
            if (Kind == StemProfileKind.VolumeBudgetedParaboloid) return (decimal)dbh * dbh * (height - heightMm) / (height - breastHeight);
            for (int i = 1; i < points.Length; i++)
            {
                var lower = points[i - 1]; var upper = points[i];
                if (heightMm > upper.HeightMm) continue;
                decimal a = (decimal)lower.DiameterMm * lower.DiameterMm, b = (decimal)upper.DiameterMm * upper.DiameterMm;
                return a + (b - a) * (heightMm - lower.HeightMm) / (upper.HeightMm - lower.HeightMm);
            }
            return 0;
        }

        public int DiameterFloorAt(int heightMm)
        {
            decimal squared = DiameterSquaredAt(heightMm);
            int result = checked((int)Math.Sqrt((double)squared));
            // Correct sqrt rounding at the integer boundary; classification itself never uses sqrt.
            while ((decimal)result * result > squared) result--;
            while ((decimal)(result + 1) * (result + 1) <= squared) result++;
            return result;
        }

        public long CumulativeVolumeAt(int heightMm)
        {
            CheckHeight(heightMm);
            if (heightMm == height) return volume;
            decimal fraction = Kind == StemProfileKind.VolumeBudgetedParaboloid ?
                ((decimal)2 * height * heightMm - (decimal)heightMm * heightMm) / ((decimal)height * height) : IntegrateMeasured(heightMm) / totalAreaIntegral;
            return checked((long)decimal.Floor(volume * fraction));
        }

        public long VolumeBetween(int startMm, int endMm)
        {
            if (endMm < startMm) throw new ArgumentException("Reversed section.");
            return checked(CumulativeVolumeAt(endMm) - CumulativeVolumeAt(startMm));
        }

        public long GeometricVolumeCm3 => checked((long)decimal.Floor(totalAreaIntegral * 0.7853981633974483096156608458m / 1000));

        public int HeightToDiameter(int diameterMm)
        {
            if (diameterMm <= 0) throw new ArgumentException("Diameter limit must be positive.");
            decimal squared = (decimal)diameterMm * diameterMm;
            if (DiameterSquaredAt(0) < squared) return 0;
            int low = 0, high = height;
            while (high - low > 1)
            {
                int mid = low + (high - low) / 2;
                if (DiameterSquaredAt(mid) >= squared) low = mid; else high = mid;
            }
            return low;
        }

        private decimal IntegrateMeasured(int toMm)
        {
            decimal total = 0;
            for (int i = 1; i < points.Length; i++)
            {
                var lower = points[i - 1]; var upper = points[i];
                if (toMm <= lower.HeightMm) break;
                int end = Math.Min(toMm, upper.HeightMm);
                decimal a = (decimal)lower.DiameterMm * lower.DiameterMm, b = (decimal)upper.DiameterMm * upper.DiameterMm;
                decimal endArea = a + (b - a) * (end - lower.HeightMm) / (upper.HeightMm - lower.HeightMm);
                total += (a + endArea) * (end - lower.HeightMm) / 2;
            }
            return total;
        }

        private void CheckHeight(int value)
        {
            if (value < 0 || value > height) throw new ArgumentException("Height outside the stem.");
        }

        public static StemMeasurements FromMetres(string treeId, string speciesId, double dbhCm, double heightM, double volumeM3, string measurementBasis)
        {
            if (double.IsNaN(dbhCm) || double.IsInfinity(dbhCm) || dbhCm <= 0 || double.IsNaN(heightM) || double.IsInfinity(heightM) || heightM <= 0 ||
                double.IsNaN(volumeM3) || double.IsInfinity(volumeM3) || volumeM3 < 0) throw new ArgumentException("Invalid measurements.");
            // Floor the source volume budget; do not create material while quantizing m³.
            return new StemMeasurements { TreeId = treeId, SpeciesId = speciesId, DbhMm = checked((int)Math.Round(dbhCm * 10, MidpointRounding.AwayFromZero)),
                TotalHeightMm = checked((int)Math.Round(heightM * 1000, MidpointRounding.AwayFromZero)),
                StemVolumeCm3 = checked((long)decimal.Floor((decimal)volumeM3 * 1000000)), MeasurementBasis = measurementBasis };
        }
    }
}

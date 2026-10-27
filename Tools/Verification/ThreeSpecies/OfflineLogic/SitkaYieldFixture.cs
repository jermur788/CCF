// Seeded Sitka stems and a result fingerprint, shared by the timber offline check and by the one-off program
// that captured the golden value from the unmodified base commit (c1564b8). It only uses members that exist in
// both the base sources and the current ones, so the same file compiles against either.
using System;
using System.Collections.Generic;
using System.Text;
using CCF.Forestry.TimberYield;

public static class SitkaYieldFixture
{
    public const int StemCount = 3000;

    public static List<StemYieldRequest> Requests()
    {
        var random = new Random(20261012);
        var requests = new List<StemYieldRequest>();
        for (int i = 0; i < StemCount; i++)
        {
            double dbh = 6 + random.NextDouble() * 70;          // 6-76 cm
            double height = 5 + random.NextDouble() * 33;       // 5-38 m
            if (height <= 1.35) height = 1.4;
            double volume = dbh * dbh * 0.00007854 * height * 0.5;
            StemMeasurements stem = MerchantableStemModel.FromMetres("S" + i.ToString("D5"), "sitka-spruce", dbh, height, volume, "[S] seeded fixture");
            stem.StemVolumeCm3 = (long)Math.Floor(volume * 1000000d);
            int kind = random.Next(6);
            if (kind == 1) stem.Quality = StemQualityFlags.PoorForm;
            else if (kind == 2) stem.Quality = StemQualityFlags.Pruned;
            else if (kind == 3)
            {
                int start = random.Next(0, Math.Max(1, stem.TotalHeightMm / 2));
                int end = Math.Min(stem.TotalHeightMm, start + 500 + random.Next(4000));
                stem.QualitySections = new[] { new StemQualitySection { StartHeightMm = start, EndHeightMm = end,
                    Flags = random.Next(2) == 0 ? StemQualityFlags.StemDefect : StemQualityFlags.WindDamage | StemQualityFlags.Pruned } };
            }
            else if (kind == 4) stem.Quality = StemQualityFlags.Unusable;
            var disposition = (CCF.Forestry.WorkEconomy.TimberDisposition)random.Next(3);
            requests.Add(new StemYieldRequest { Stem = stem, DefaultDisposition = disposition, ExtractRetainedToRoadside = random.Next(2) == 0 });
        }
        return requests;
    }

    // Everything the allocator decides, except the configuration id (which differs by design for the scenario configuration).
    public static string Fingerprint(IEnumerable<StemYieldResult> results)
    {
        ulong hash = 14695981039346656037UL;
        void Text(string value)
        {
            foreach (byte b in Encoding.UTF8.GetBytes(value ?? "")) { hash ^= b; hash *= 1099511628211UL; }
            hash ^= 0xFF; hash *= 1099511628211UL;
        }
        void Number(long value) => Text(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        foreach (StemYieldResult r in results)
        {
            Text(r.TreeId); Number(r.StemVolumeCm3); Number(r.DbhMm); Number(r.TotalHeightMm); Number(r.GeometricProfileVolumeCm3);
            Number(r.MerchantableTopHeightMm); Number(r.SaleAssortmentVolumeCm3); Number(r.RetainedForUseVolumeCm3);
            Number(r.DeadwoodAssortmentVolumeCm3); Number(r.ResidualVolumeCm3); Number(r.CandidateChecks);
            foreach (AllocatedTimberLog log in r.Logs)
            {
                Text(log.LogId); Text(log.SpecificationId); Number((int)log.Assortment); Number((int)log.PotentialUse);
                Number(log.StartHeightMm); Number(log.EndHeightMm); Number(log.SmallEndDiameterMm); Number(log.LargeEndDiameterMm);
                Number(log.VolumeCm3); Number((int)log.Quality); Number((int)log.Disposition); Number(log.ExtractToRoadside ? 1 : 0);
                Number(log.OwnerPaysHaulage ? 1 : 0);
            }
            foreach (StemResidualSection s in r.ResidualSections) { Number((int)s.Kind); Number(s.StartHeightMm); Number(s.EndHeightMm); Number(s.VolumeCm3); }
        }
        return hash.ToString("X16");
    }
}

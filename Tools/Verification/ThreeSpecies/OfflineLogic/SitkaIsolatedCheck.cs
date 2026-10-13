// Compile-time proof that pure Sitka cannot depend on the species-aware occupancy model.
//
// This is built with SitkaGrowthModel.cs ALONE: MixedDensityModel.cs is deliberately not in the build. If the Sitka
// growth model referenced the occupancy model (OccupancyNormalization, MaximumAnnualLoss, OccupancyCalibration, ...)
// this project would fail to compile. Success therefore means that no number in the occupancy model, including the
// normalization, can change the pure-Sitka density arithmetic.
using System;

public static class SitkaIsolatedCheck
{
    public static int Main(string[] args)
    {
        double rd = SitkaGrowthModel.RelativeDensity(1200.0, 28.0);
        double pressure = SitkaGrowthModel.DensityPressure(rd);
        bool ok = rd > 0.0 && pressure > 0.0 && GrowthModel.Normalize(2) == GrowthModel.SpeciesAware;
        Console.WriteLine(ok
            ? $"SITKA_MODEL_ISOLATED_PASS compiledWithoutMixedDensityModel=true relativeDensity={rd:0.000000} pressure={pressure:0.000000}"
            : "SITKA_MODEL_ISOLATED_FAIL");
        return ok ? 0 : 1;
    }
}

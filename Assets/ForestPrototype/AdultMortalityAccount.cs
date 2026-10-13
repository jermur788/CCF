// Growth model 1/2 adult density mortality for one annual step. Diagnostic
// only: never saved, never read by the simulation.
public sealed class AdultMortalityAccount
{
    public int Year;
    public int LivingBefore;
    public double StemsPerHectare;
    public double QuadraticMeanDbhCm;
    // Which implementation handled the step (None when the stand was empty). Under growth model 2 a stand of only
    // Reference (Sitka) trees reports LegacySitkaExact; a stand with any Oak or Beech reports SpeciesAwareOccupancy.
    public DensityRoute Route;
    // LegacySitkaExact only: British Sitka relative density. Never computed for a species-aware step.
    public double RelativeDensity;
    // SpeciesAwareOccupancy only: demand-weighted occupancy, 1.0 = capacity. Not a Sitka relative density.
    public double Occupancy;
    // The pressure that drove mortality on either route (each route defines its own onset).
    public double DensityPressure;
    public int HazardDeaths;
    public int BoundaryDeaths;
    public int Deaths;
    public double DeadStemVolumeM3;
}

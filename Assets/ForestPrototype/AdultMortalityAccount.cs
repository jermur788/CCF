// Growth model 1/2 adult density mortality for one annual step. Diagnostic
// only: never saved, never read by the simulation.
public sealed class AdultMortalityAccount
{
    public int Year;
    public int LivingBefore;
    public double StemsPerHectare;
    public double QuadraticMeanDbhCm;
    // The density that drives mortality. Under the species-aware route (growth model 2,
    // stand with a non-Reference species) this is the demand-weighted density.
    public double RelativeDensity;
    public double DensityPressure;
    public int HazardDeaths;
    public int BoundaryDeaths;
    public int Deaths;
    public double DeadStemVolumeM3;
    // Growth model 2 only: true when the species-aware route handled the step, and the
    // pooled Sitka-relationship density the Sitka-only route would have seen.
    public bool SpeciesAwareRoute;
    public double UnweightedRelativeDensity;
}

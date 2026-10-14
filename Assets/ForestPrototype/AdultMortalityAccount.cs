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
    // SpeciesAwareOccupancy only: dimensionless demand-weighted occupancy; 1.0 is where overcrowding pressure starts.
    // A simulation quantity, not a Sitka relative density and not a density limit for any species.
    public double Occupancy;
    // The pressure that drove mortality on either route (each route defines its own onset and scale).
    public double DensityPressure;
    // Hazard deaths. On the species-aware route: the Sitka component's hazard victims plus every generic victim.
    public int HazardDeaths;
    // The Sitka boundary stage. LegacySitkaExact: the whole stand. SpeciesAwareOccupancy: the Sitka component only;
    // pure Oak or Beech stands never have one.
    public int BoundaryDeaths;
    // SpeciesAwareOccupancy only, diagnostics (never saved). The Sitka component is the original Sitka density process
    // run on the Sitka trees alone; the generic component is the occupancy response over the trees it left standing.
    public double SitkaSubsetRelativeDensity;
    public int SitkaComponentDeaths;
    public int GenericDeaths;
    // Generic response times the non-Reference share of the occupied space: the expected fraction of the remaining trees.
    public double GenericLossFraction;
    public double NonReferenceShare;
    public int Deaths;
    public double DeadStemVolumeM3;
}

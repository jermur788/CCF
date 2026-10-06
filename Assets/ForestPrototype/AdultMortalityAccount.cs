// Growth model 1 adult density mortality for one annual step. Diagnostic
// only: never saved, never read by the simulation.
public sealed class AdultMortalityAccount
{
    public int Year;
    public int LivingBefore;
    public double StemsPerHectare;
    public double QuadraticMeanDbhCm;
    public double RelativeDensity;
    public double DensityPressure;
    public int HazardDeaths;
    public int BoundaryDeaths;
    public int Deaths;
    public double DeadStemVolumeM3;
}

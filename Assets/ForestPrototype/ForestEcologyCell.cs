using UnityEngine;

public sealed class ForestEcologyCell
{
    public Vector2 Center;
    public float Canopy;
    public float Light;

    // Site state ([C] simple defaults; no detailed soil chemistry in this milestone).
    public float SiteProductivity = 1f;
    public float SoilStability = 1f;
    public float EstablishmentSuitability = 1f;

    // Seed and regeneration state for this cell.
    public float SitkaSeedRain;
    public float RegenDensity;
    public float RegenHeight;
    public int RegenEstablishYear = -1;

    // Wind exposure from recent local removals (diagnostic; decays annually).
    public float RecentOpening;
}

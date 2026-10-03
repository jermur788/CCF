using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Scenario 1 regeneration "why" diagnosis (Docs/Scenario1EcologyCompletion.md).
//
// Read-only: answers "what is limiting the juvenile here?" from current
// authoritative ecology using the existing shared rules only. It never changes
// Unity or ecology state and adds no biological rule. Browse exposure describes
// the UPCOMING annual step (EcologicalYear + 1), which is when protection and
// pressure next act.

public enum RegenerationLimit
{
    NoneEstablished,
    LightLimited,
    BrowsedExposed,
    Protected,
    AboveBrowseReach,
    Promoting,
    // Neither light nor browsing currently limits (e.g. pressure 0, unpalatable species).
    Growing
}

public enum JuvenileStage
{
    None,
    Seedling,       // below 0.5 m
    Juvenile,       // within full browse vulnerability
    NearEscape,     // in the browse-vulnerability taper
    Sapling,        // above browse reach, below promotion height
    ReadyToPromote
}

public struct RegenerationDiagnosis
{
    public bool HasJuvenile;
    public bool IsPlantedIndividual;
    public string JuvenileId;          // planted individuals only
    public string SpeciesId;
    public string SpeciesName;
    public int CellIndex;
    public float Height;
    public float Density;              // cohorts only
    public float Light;
    public string LightBand;
    public JuvenileStage Stage;
    public float LightGrowthResponse;
    public double LightSurvival;
    public BrowseAssessment Browse;    // for the upcoming annual step
    public float BrowseEscapeHeightM;
    public int ShelterStepsRemaining;  // upcoming annual steps still protected; -1 = no effective shelter
    public bool CanPromoteNow;
    public float PromotionHeightM;
    public float PromotionMinimumLight;
    public float PoorLightThreshold;
    public RegenerationLimit Limit;
    // Last annual step's browsing: "yes"/"no" (individual) or a fraction (cohort)
    // when recorded by that step in this session; "n/a" after load (not saved).
    public string LastYearBrowsed;

    public string Summary()
    {
        if (!HasJuvenile)
            return "Regeneration: none established here" + (Light >= 0f ? $" (light {F2(Light)}, {LightBand})" : "");
        string who = IsPlantedIndividual ? $"{SpeciesName} planted {JuvenileId}" : $"{SpeciesName} regeneration ({F2(Density)}/m²)";
        string reason;
        switch (Limit)
        {
            case RegenerationLimit.Promoting:
                reason = "ready to join the canopy layer"; break;
            case RegenerationLimit.LightLimited:
                reason = Light < PromotionMinimumLight && LightGrowthResponse >= PoorLightThreshold && LightSurvival >= 1d - 1e-6
                    ? $"light-limited: below {F2(PromotionMinimumLight)} light needed to recruit"
                    : $"light-limited: {LightBand} (growth response {F2(LightGrowthResponse)})"; break;
            case RegenerationLimit.AboveBrowseReach:
                reason = $"above browse reach (>{BrowseEscapeHeightM.ToString("0.0", CultureInfo.InvariantCulture)} m)"; break;
            case RegenerationLimit.Protected:
                reason = Browse.Protection == BrowseProtectionState.EffectiveShelter
                    ? $"protected by shelter ({ShelterStepsRemaining} yr left)"
                    : "protected inside intact fence"; break;
            case RegenerationLimit.BrowsedExposed:
                reason = $"exposed to browsing: {Mathf.RoundToInt(Browse.Probability * 100f)}%/yr leader risk ({Browse.PressureBand})"
                    + (Browse.Protection == BrowseProtectionState.ExpiredShelter ? ", shelter expired"
                        : Browse.Protection == BrowseProtectionState.FailedShelter ? ", shelter failed"
                        : Browse.Protection == BrowseProtectionState.BreachedFence ? ", fence breached" : ""); break;
            default:
                reason = "growing; not limited by light or browsing"; break;
        }
        return $"{who} {Height.ToString("0.00", CultureInfo.InvariantCulture)} m · {reason} · last year browsed: {LastYearBrowsed}";
    }

    private static string F2(float value) => value.ToString("0.00", CultureInfo.InvariantCulture);
}

public static class RegenerationDiagnostics
{
    // Exact planted juveniles closer than this to the aimed point are diagnosed individually.
    public const float PlantedMatchRadiusM = 0.75f;

    // [C] Readout bands only; ecology always uses the scalar light value.
    public static string LightBand(float light)
        => light < 0.10f ? "deep shade" : light < 0.20f ? "shade" : light < 0.35f ? "partial shade"
            : light < 0.60f ? "open" : "bright";

    // Diagnose the juvenile at a ground position: the nearest living, unpromoted
    // exact planted juvenile within PlantedMatchRadiusM, otherwise the tallest
    // cohort in the cell.
    public static RegenerationDiagnosis Diagnose(ForestEcologyController ecology, Vector3 worldPosition,
        IReadOnlyList<PlantedJuvenile> planted = null, ForestTreeSpawner spawner = null)
    {
        var none = new RegenerationDiagnosis { CellIndex = -1, Light = -1f, LightBand = "", LastYearBrowsed = "n/a",
            ShelterStepsRemaining = -1, SpeciesId = "", SpeciesName = "", JuvenileId = "" };
        if (ecology == null || ecology.Cells == null)
            return none;
        int index = ecology.GetCellIndex(worldPosition);
        if (index < 0)
            return none;
        ForestEcologyCell cell = ecology.Cells[index];
        none.CellIndex = index;
        none.Light = cell.Light;
        none.LightBand = LightBand(cell.Light);

        PlantedJuvenile nearest = NearestPlanted(planted, worldPosition);
        if (nearest != null)
        {
            if (spawner == null)
                spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
            TreeSpeciesDefinition species = spawner != null ? spawner.ResolveSpecies(nearest.speciesId) : null;
            int plantedCell = ecology.GetCellIndex(nearest.position);
            if (species != null && plantedCell >= 0)
                return DiagnosePlanted(ecology, nearest, species, plantedCell);
        }

        ForestRegenerationCohort lead = null;
        foreach (ForestRegenerationCohort cohort in cell.Regeneration)
        {
            if (cohort == null || cohort.Species == null || cohort.Density <= 0f)
                continue;
            if (lead == null || cohort.Height > lead.Height
                || (cohort.Height == lead.Height && string.CompareOrdinal(cohort.SpeciesId, lead.SpeciesId) < 0))
                lead = cohort;
        }
        return lead != null ? DiagnoseCohort(ecology, index, lead) : none;
    }

    // Every established cohort in a cell (ordinal species order).
    public static List<RegenerationDiagnosis> DiagnoseCell(ForestEcologyController ecology, int cellIndex)
    {
        var result = new List<RegenerationDiagnosis>();
        if (ecology?.Cells == null || cellIndex < 0 || cellIndex >= ecology.Cells.Length)
            return result;
        foreach (ForestRegenerationCohort cohort in ecology.Cells[cellIndex].Regeneration)
            if (cohort != null && cohort.Species != null && cohort.Density > 0f)
                result.Add(DiagnoseCohort(ecology, cellIndex, cohort));
        return result;
    }

    public static RegenerationDiagnosis DiagnoseCohort(ForestEcologyController ecology, int cellIndex, ForestRegenerationCohort cohort)
    {
        ForestEcologyCell cell = ecology.Cells[cellIndex];
        int upcoming = ecology.EcologicalYear + 1;
        RegenerationDiagnosis d = Common(cohort.Species, cohort.Height, cell.Light, cellIndex);
        d.Density = cohort.Density;
        d.Browse = ecology.AssessCohortBrowse(cellIndex, cohort.Species, cohort.Height, upcoming);
        d.ShelterStepsRemaining = -1;
        d.LastYearBrowsed = cohort.LastBrowseAssessmentYear == ecology.EcologicalYear && ecology.EcologicalYear > 0
            ? cohort.LastBrowsedFraction.ToString("P0", CultureInfo.InvariantCulture) + " of leaders" : "n/a";
        d.Limit = Classify(d);
        return d;
    }

    public static RegenerationDiagnosis DiagnosePlanted(ForestEcologyController ecology, PlantedJuvenile juvenile,
        TreeSpeciesDefinition species, int cellIndex)
    {
        ForestEcologyCell cell = ecology.Cells[cellIndex];
        int upcoming = ecology.EcologicalYear + 1;
        RegenerationDiagnosis d = Common(species, juvenile.heightMeters, cell.Light, cellIndex);
        d.IsPlantedIndividual = true;
        d.JuvenileId = juvenile.juvenileId ?? "";
        d.Density = 0f;
        d.Browse = ecology.AssessIndividualBrowse(juvenile.position, species, juvenile.heightMeters, upcoming);
        d.ShelterStepsRemaining = ShelterStepsRemaining(ecology.Browsing, juvenile.position, upcoming);
        d.LastYearBrowsed = juvenile.lastBrowseAssessmentYear == ecology.EcologicalYear && ecology.EcologicalYear > 0
            ? (juvenile.lastYearBrowsed ? "yes" : "no") : "n/a";
        d.Limit = Classify(d);
        return d;
    }

    // Upcoming annual steps (starting at `upcomingYear`) during which an
    // effective shelter at this position still protects; -1 if none does.
    public static int ShelterStepsRemaining(BrowsingConditions browsing, Vector3 worldPosition, int upcomingYear)
    {
        if (browsing == null)
            return -1;
        var position = new Vector2(worldPosition.x, worldPosition.z);
        int best = -1;
        foreach (BrowseShelter shelter in browsing.Shelters)
        {
            if (shelter == null || !shelter.IsEffective(upcomingYear)
                || (shelter.position - position).sqrMagnitude > BrowseShelter.MatchRadiusM * BrowseShelter.MatchRadiusM)
                continue;
            int end = shelter.installedYear + Mathf.Max(0, shelter.effectiveYears);
            if (shelter.failedYear >= 0)
                end = Mathf.Min(end, shelter.failedYear);
            best = Mathf.Max(best, end - upcomingYear);
        }
        return best;
    }

    private static RegenerationDiagnosis Common(TreeSpeciesDefinition species, float height, float light, int cellIndex)
    {
        var d = new RegenerationDiagnosis
        {
            HasJuvenile = true,
            SpeciesId = species.SpeciesId,
            SpeciesName = species.DisplayName,
            JuvenileId = "",
            CellIndex = cellIndex,
            Height = height,
            Light = light,
            LightBand = LightBand(light),
            LightGrowthResponse = JuvenileEcologyRules.LightResponse(species, light),
            LightSurvival = JuvenileEcologyRules.SurvivalResponse(species, light),
            BrowseEscapeHeightM = species.BrowseEscapeHeightM,
            CanPromoteNow = JuvenileEcologyRules.CanPromote(species, height, light),
            PromotionHeightM = species.PromotionHeightM,
            PromotionMinimumLight = species.PromotionMinimumLight,
            PoorLightThreshold = species.RegenPoorLightThreshold
        };
        d.Stage = d.CanPromoteNow ? JuvenileStage.ReadyToPromote
            : height >= species.BrowseEscapeHeightM ? JuvenileStage.Sapling
            : height > species.BrowseFullVulnerabilityHeightM ? JuvenileStage.NearEscape
            : height < 0.5f ? JuvenileStage.Seedling
            : JuvenileStage.Juvenile;
        return d;
    }

    private static RegenerationLimit Classify(RegenerationDiagnosis d)
    {
        if (!d.HasJuvenile)
            return RegenerationLimit.NoneEstablished;
        if (d.CanPromoteNow)
            return RegenerationLimit.Promoting;
        // Light limits growth/survival (existing responses) or recruitment light.
        bool lightLimited = d.LightSurvival < 1d - 1e-6
            || d.LightGrowthResponse < d.PoorLightThreshold
            || d.Light < d.PromotionMinimumLight;
        if (lightLimited)
            return RegenerationLimit.LightLimited;
        if (d.Browse.Reason == BrowseExposureReason.AboveBrowseReach)
            return RegenerationLimit.AboveBrowseReach;
        if (d.Browse.Reason == BrowseExposureReason.Protected)
            return RegenerationLimit.Protected;
        if (d.Browse.Reason == BrowseExposureReason.Exposed && d.Browse.Probability > 0f)
            return RegenerationLimit.BrowsedExposed;
        return RegenerationLimit.Growing;
    }

    private static PlantedJuvenile NearestPlanted(IReadOnlyList<PlantedJuvenile> planted, Vector3 worldPosition)
    {
        if (planted == null)
            return null;
        PlantedJuvenile best = null;
        float bestDistance = PlantedMatchRadiusM * PlantedMatchRadiusM;
        foreach (PlantedJuvenile juvenile in planted)
        {
            if (juvenile == null || !juvenile.alive || juvenile.legacyCohortManaged || !string.IsNullOrEmpty(juvenile.promotedTreeId))
                continue;
            float dx = juvenile.position.x - worldPosition.x, dz = juvenile.position.z - worldPosition.z;
            float distance = dx * dx + dz * dz;
            if (distance < bestDistance || (distance == bestDistance && best != null
                && string.CompareOrdinal(juvenile.juvenileId, best.juvenileId) < 0))
            {
                best = juvenile;
                bestDistance = distance;
            }
        }
        return best;
    }
}

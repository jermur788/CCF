using System;

// Static site configuration per scenario (Oak/Beech site infrastructure v1).
//
// DORMANT and NOT PERSISTED: the site is derived from the scenario id, so nothing is added to the save (version stays
// 20) and no saved profile can drift from the scenario. Treat each entry as immutable: a changed site is a NEW version,
// never an in-place edit that would silently reinterpret old saves.
public static class ScenarioSiteProfiles
{
    // Same string as ScenarioOneDefinition's and ForestSaveData's default scenarioId (checked by the offline verification).
    public const string ScenarioOneId = "scenario-one";

    // Bump only by adding a new profile, not by editing this one.
    public const int ScenarioOneReferenceVersion = 1;

    // Scenario One's canonical site: a versioned COMPATIBILITY reference (research v2 section 10). It is not a soil
    // survey, not "optimal", and not a typical Irish Sitka gley. Moisture, drainage/aeration, fertility and
    // exposure/climate are each explicitly Reference. Rooting/stability is Unknown because v2 leaves it
    // "unspecified/reserved, not an invented measured depth"; Reference would claim a configured condition that v2 does not define.
    // Every numerical response at this site is exactly 1.0 (see SiteGrowthResponse).
    public static readonly SiteConditionProfile ScenarioOneReference = new SiteConditionProfile(
        MoistureClass.Reference,
        DrainageAerationClass.Reference,
        FertilityClass.Reference,
        ExposureClimateClass.Reference,
        RootingContextClass.Unknown);

    // False for a scenario id with no configured site; the site is then Unspecified (all Unknown), never Reference.
    public static bool TryGet(string scenarioId, out SiteConditionProfile site)
    {
        if (string.Equals(scenarioId, ScenarioOneId, StringComparison.Ordinal))
        {
            site = ScenarioOneReference;
            return true;
        }
        site = SiteConditionProfile.Unspecified;
        return false;
    }
}

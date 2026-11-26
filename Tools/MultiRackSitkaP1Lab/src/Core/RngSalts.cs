namespace SitkaP1.Core;

/// <summary>
/// Centralised registry of all simulation RNG key prefixes, suffixes and fixed stream keys
/// (SPEC-multi-2d-s0-seam-extraction-V1 §6.4). Every value is frozen byte-for-byte against T0 so
/// no deterministic RNG stream or frozen anchor can drift. Any new family added after S0 must be
/// strictly prefix-free against every existing entry.
/// </summary>
internal static class RngSalts
{
    // Adult density mortality rolls (frozen legacy nested family; see RngSaltsTests.FrozenNestedPrefixes).
    internal const string AdultMortality = "ADULT-MORT-";
    internal const string AdultMortalityGeneric = "ADULT-MORT-GENERIC-";
    internal const string AdultMortalityGenericQuota = "ADULT-MORT-GENERIC-QUOTA-";

    // Phase 2 broadleaf mast and planted juvenile roll keys.
    internal const string Phase2Mast = "PHASE2-MAST-";
    internal const string Phase2Juvenile = "PHASE2-PJ-";
    internal const string Phase2JuvenileVegetationSuffix = "/vegetation";
    internal const string Phase2JuvenileBrowseSuffix = "/browse";

    // Stage 5 Sitka mast (complete key, not a prefix) and cell promotion stream prefix.
    internal const string Stage5Mast = "STAGE5-MAST";
    internal const string Stage5Promotion = "STAGE5-PROMO-";

    // Phase 2A Forest Maker deterministic generation key prefix (independent of SimulationRandom mortality keys;
    // registered here for centralised prefix-freedom and G6 enforcement).
    internal const string ForestMaker = "FOREST-MAKER-";

    /// <summary>Ordered registry of every RNG salt constant for automated uniqueness/prefix checks and G6.</summary>
    internal static readonly IReadOnlyList<string> Families = Array.AsReadOnly(new[]
    {
        AdultMortality,
        AdultMortalityGeneric,
        AdultMortalityGenericQuota,
        Phase2Mast,
        Phase2Juvenile,
        Phase2JuvenileVegetationSuffix,
        Phase2JuvenileBrowseSuffix,
        Stage5Mast,
        Stage5Promotion,
        ForestMaker,
    });
}

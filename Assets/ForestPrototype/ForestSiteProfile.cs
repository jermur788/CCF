using System;

// Site description for future species x site modelling (Oak/Beech site infrastructure v1.1).
//
// DORMANT: nothing in the simulation reads these types. They describe a site; they do not change growth,
// competition, density, saves or Scenario One outcomes. The species side and the always-1.0 numerical seam are in
// SpeciesSiteResponse.cs; the canonical Scenario One site is in ScenarioSiteProfiles.cs.
//
// The descriptors are kept SEPARATE on purpose (research v2 sections 8 and 14). A wet site is not necessarily a poor
// one, a fertile site is not necessarily sheltered, and above all how much water a site holds is not the same thing as
// how well it drains and aerates: "moist and well drained" and "moist and waterlogged" are different sites, so water
// availability (MoistureClass) and drainage/aeration (DrainageAerationClass) are two descriptors rather than one
// composite scale with a category for every pairing. Do not collapse any of them into one "site quality" number or a
// weighted sum. These are also not the existing cell.SiteProductivity scalar or the soil-stability value, which are unchanged.
//
// Every descriptor has the same two special values, always first in its enum:
//   Unknown   (0) = no value was supplied: "we do not know". A default-constructed profile is all Unknown.
//   Reference (1) = an explicitly configured canonical compatibility condition. It is NOT "optimal" and NOT a survey.
// They are different states even though both currently give a numerical response of exactly 1.0.
// The remaining classes are coarse ordinal labels (a rank, not a measurement); no numeric effect is attached to any of them.

// Water availability only: how much water the site holds for the trees. Says nothing about drainage or aeration.
public enum MoistureClass
{
    Unknown = 0,
    Reference = 1,
    Drier = 2,
    Moist = 3,
    Wetter = 4,
}

// Drainage and soil aeration only, ordered by increasing constraint. Says nothing about how much water is available.
// Wetter is not WaterloggedRisk: a moist or wet soil can still drain and aerate well.
public enum DrainageAerationClass
{
    Unknown = 0,
    Reference = 1,
    WellAerated = 2,
    Impeded = 3,
    WaterloggedRisk = 4,
}

public enum FertilityClass
{
    Unknown = 0,
    Reference = 1,
    Low = 2,
    Moderate = 3,
    High = 4,
}

// Exposure and climate context. Recording exposure here does not activate storms or windthrow.
public enum ExposureClimateClass
{
    Unknown = 0,
    Reference = 1,
    Sheltered = 2,
    Moderate = 3,
    Exposed = 4,
}

// Reserved extension point for rooting/stability context (depth, compaction, texture, rockiness). Deliberately has no
// descriptive classes yet: inventing them now would invent ecology. Add them here when evidence supports specific ones.
public enum RootingContextClass
{
    Unknown = 0,
    Reference = 1,
}

// An immutable description of one site: five independent descriptors. default(SiteConditionProfile) and
// SiteConditionProfile.Unspecified are the same value: every descriptor Unknown.
public readonly struct SiteConditionProfile : IEquatable<SiteConditionProfile>
{
    public readonly MoistureClass Moisture;
    public readonly DrainageAerationClass DrainageAeration;
    public readonly FertilityClass Fertility;
    public readonly ExposureClimateClass ExposureClimate;
    public readonly RootingContextClass RootingContext;

    public SiteConditionProfile(MoistureClass moisture, DrainageAerationClass drainageAeration, FertilityClass fertility,
        ExposureClimateClass exposureClimate, RootingContextClass rootingContext = RootingContextClass.Unknown)
    {
        if (!Enum.IsDefined(typeof(MoistureClass), moisture))
            throw new ArgumentOutOfRangeException(nameof(moisture));
        if (!Enum.IsDefined(typeof(DrainageAerationClass), drainageAeration))
            throw new ArgumentOutOfRangeException(nameof(drainageAeration));
        if (!Enum.IsDefined(typeof(FertilityClass), fertility))
            throw new ArgumentOutOfRangeException(nameof(fertility));
        if (!Enum.IsDefined(typeof(ExposureClimateClass), exposureClimate))
            throw new ArgumentOutOfRangeException(nameof(exposureClimate));
        if (!Enum.IsDefined(typeof(RootingContextClass), rootingContext))
            throw new ArgumentOutOfRangeException(nameof(rootingContext));
        Moisture = moisture;
        DrainageAeration = drainageAeration;
        Fertility = fertility;
        ExposureClimate = exposureClimate;
        RootingContext = rootingContext;
    }

    // "No site information supplied". Never the same thing as a Reference site.
    public static SiteConditionProfile Unspecified => default;

    public bool Equals(SiteConditionProfile other) =>
        Moisture == other.Moisture && DrainageAeration == other.DrainageAeration && Fertility == other.Fertility
        && ExposureClimate == other.ExposureClimate && RootingContext == other.RootingContext;

    public override bool Equals(object obj) => obj is SiteConditionProfile other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)Moisture;
            hash = hash * 31 + (int)DrainageAeration;
            hash = hash * 31 + (int)Fertility;
            hash = hash * 31 + (int)ExposureClimate;
            return hash * 31 + (int)RootingContext;
        }
    }

    public static bool operator ==(SiteConditionProfile left, SiteConditionProfile right) => left.Equals(right);
    public static bool operator !=(SiteConditionProfile left, SiteConditionProfile right) => !left.Equals(right);

    public override string ToString() =>
        $"moisture={Moisture}, drainage/aeration={DrainageAeration}, fertility={Fertility}, exposure/climate={ExposureClimate}, rooting={RootingContext}";
}

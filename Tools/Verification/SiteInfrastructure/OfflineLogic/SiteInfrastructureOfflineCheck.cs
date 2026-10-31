// Offline verification for the dormant Oak/Beech site infrastructure v1.1:
//   Assets/ForestPrototype/ForestSiteProfile.cs, SpeciesSiteResponse.cs, ScenarioSiteProfiles.cs
//
// run_site_check.py builds this with ONLY those three production files, so a successful build is itself the proof that
// they depend on nothing else: no UnityEngine, SitkaGrowthModel, TreeSpeciesDefinition, ForestEcologyController or save class.
//
// PROVES: the five site descriptors (moisture, drainage/aeration, fertility, exposure/climate, rooting) are independent and
// no composite moisture-and-drainage category exists; Unknown and Reference stay distinct in every descriptor; every numerical
// response is exactly 1.0; Oak/Beech/Sitka carry different qualitative descriptors without conflating dry with free-draining
// or wet with waterlogged; the Scenario One site is explicit; lookups are deterministic; and, by reading source text, that no
// production code and no unlisted verification tool calls the new types and the save version is unchanged.
// DOES NOT PROVE: that any descriptor is ecologically right, that Unity imports or compiles the files, or any anchor hash.
//
// Guard H is about THIS packet: production must not reference the seam at all, and verification tools may reference it
// only from exact, reviewable paths in VerificationAllowList. There is no directory-wide or pattern-wide escape; the
// controls in CheckH prove both the positive (allow-listed) and negative (unlisted) cases.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public static class SiteInfrastructureOfflineCheck
{
    private static int checks;
    private static int failures;
    private static string root;

    private static void Check(string name, bool ok, string detail = "")
    {
        checks++;
        if (!ok) failures++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + " " + detail);
    }

    private static bool Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return true; }
        return false;
    }

    private static T[] Values<T>() where T : struct, Enum => (T[])Enum.GetValues(typeof(T));
    private static string[] Names<T>() where T : struct, Enum => Enum.GetNames(typeof(T));
    private static int Bits(float value) => BitConverter.SingleToInt32Bits(value);

    private static readonly string[] RegisteredSpecies =
        { SpeciesSiteResponses.SitkaSpruceId, SpeciesSiteResponses.SessileOakId, SpeciesSiteResponses.BeechId };
    // Not in the catalog. Matching is ordinal, so "Beech" is not "beech".
    private static readonly string[] UnregisteredSubjects = { "douglas-fir", "", "Beech", null };

    private static readonly string[] NewTypeNames =
    {
        "SiteConditionProfile", "MoistureClass", "DrainageAerationClass", "FertilityClass", "ExposureClimateClass", "RootingContextClass",
        "SiteSuitability", "SiteResponseBasis", "SiteAxisResponse", "SpeciesSiteResponse", "SpeciesSiteResponses",
        "SiteGrowthResponse", "ScenarioSiteProfiles",
    };

    // The five descriptor fields of SiteConditionProfile, in declaration order.
    private static readonly string[] DescriptorFieldNames = { "Moisture", "DrainageAeration", "Fertility", "ExposureClimate", "RootingContext" };

    // Exact repository-relative verification paths that may reference the dormant seam. No directories, globs or
    // prefixes: IsVerificationAllowed uses ordinal exact matching, and the controls below prove it. The adult gate is
    // the reviewed consumer named by Verification Harness Hygiene 01; it does not reference the seam today, so this is
    // the pre-authorised path for that gate rather than an observed reference.
    private static readonly string[] VerificationAllowList =
    {
        "Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs",
    };

    private static List<SiteConditionProfile> AllProfiles()
    {
        var list = new List<SiteConditionProfile>();
        foreach (MoistureClass moisture in Values<MoistureClass>())
            foreach (DrainageAerationClass drainage in Values<DrainageAerationClass>())
                foreach (FertilityClass fertility in Values<FertilityClass>())
                    foreach (ExposureClimateClass exposure in Values<ExposureClimateClass>())
                        foreach (RootingContextClass rooting in Values<RootingContextClass>())
                            list.Add(new SiteConditionProfile(moisture, drainage, fertility, exposure, rooting));
        return list;
    }

    // A profile as five integers, and back. Lets the independence check treat all five descriptors the same way.
    private static int[] Fields(SiteConditionProfile p) =>
        new[] { (int)p.Moisture, (int)p.DrainageAeration, (int)p.Fertility, (int)p.ExposureClimate, (int)p.RootingContext };

    private static SiteConditionProfile Make(int[] f) =>
        new SiteConditionProfile((MoistureClass)f[0], (DrainageAerationClass)f[1], (FertilityClass)f[2], (ExposureClimateClass)f[3], (RootingContextClass)f[4]);

    private static int[][] ValuesPerDescriptor() => new[]
    {
        Values<MoistureClass>().Select(v => (int)v).ToArray(), Values<DrainageAerationClass>().Select(v => (int)v).ToArray(),
        Values<FertilityClass>().Select(v => (int)v).ToArray(), Values<ExposureClimateClass>().Select(v => (int)v).ToArray(),
        Values<RootingContextClass>().Select(v => (int)v).ToArray(),
    };

    private static bool SameSet(IEnumerable<string> actual, params string[] expected) =>
        new HashSet<string>(actual).SetEquals(expected);

    // What a species says about each of the four assessable descriptors of a site, as one comparable string.
    private static string Verdicts(SpeciesSiteResponse species, SiteConditionProfile site) =>
        species.Moisture.SuitabilityOf(site.Moisture) + "|" + species.DrainageAeration.SuitabilityOf(site.DrainageAeration) + "|"
        + species.Fertility.SuitabilityOf(site.Fertility) + "|" + species.Exposure.SuitabilityOf(site.ExposureClimate);

    public static int Main(string[] args)
    {
        root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        List<SiteConditionProfile> profiles = AllProfiles();

        CheckA_FiveIndependentDescriptors(profiles);
        CheckB_UnknownIsNotReference();
        CheckC_NeutralNumericalResponse(profiles);
        CheckD_SpeciesIdentity();
        CheckE_ScenarioOneReference();
        CheckF_Determinism(profiles);
        CheckG_NoSaveImpact();
        CheckH_NoCurrentGrowthEffect();
        CheckI_InputValidation();

        Console.WriteLine(failures == 0
            ? $"SITE_INFRASTRUCTURE_PASS checks={checks} siteProfiles={profiles.Count} registeredSpecies={RegisteredSpecies.Length} isolatedCompile=true"
            : $"SITE_INFRASTRUCTURE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    // ---- A. Five independent descriptors, no composite moisture-and-drainage category -----------------------------

    private static void CheckA_FiveIndependentDescriptors(List<SiteConditionProfile> profiles)
    {
        Check("A_DOMAIN_CATEGORIES_AS_SPECIFIED",
            SameSet(Names<MoistureClass>(), "Unknown", "Reference", "Drier", "Moist", "Wetter")
            && SameSet(Names<DrainageAerationClass>(), "Unknown", "Reference", "WellAerated", "Impeded", "WaterloggedRisk")
            && SameSet(Names<FertilityClass>(), "Unknown", "Reference", "Low", "Moderate", "High")
            && SameSet(Names<ExposureClimateClass>(), "Unknown", "Reference", "Sheltered", "Moderate", "Exposed")
            && SameSet(Names<RootingContextClass>(), "Unknown", "Reference"),
            "coarse ordinal classes only; no category was added");

        // Structural proof of "five independent descriptors": exactly five public instance fields, each of its own type.
        FieldInfo[] fields = typeof(SiteConditionProfile).GetFields(BindingFlags.Public | BindingFlags.Instance);
        Check("A_PROFILE_HAS_EXACTLY_FIVE_DESCRIPTOR_FIELDS_OF_FIVE_DISTINCT_TYPES",
            fields.Length == 5 && SameSet(fields.Select(f => f.Name), DescriptorFieldNames) && fields.Select(f => f.FieldType).Distinct().Count() == 5,
            string.Join(", ", fields.Select(f => f.Name + ":" + f.FieldType.Name)));

        // The old composite scale must not survive under any name, in the build or in the source.
        bool compositeTypeGone = typeof(SiteConditionProfile).Assembly.GetTypes().All(t => !t.Name.StartsWith("MoistureDrainage", StringComparison.Ordinal));
        bool compositeSourceGone = NewProductionFiles().All(f => !Regex.IsMatch(File.ReadAllText(f), "MoistureDrainage|DrierFreeDraining|MoistWellAerated|PoorlyDrained"));
        Check("A_NO_COMPOSITE_MOISTURE_DRAINAGE_CATEGORY_EXISTS", compositeTypeGone && compositeSourceGone,
            "water availability and drainage/aeration are separate enums, not one scale with a value per pairing");

        // Equality is exactly "all five fields equal", over every pair of the full product.
        bool equalityMatchesFields = true;
        for (int i = 0; i < profiles.Count && equalityMatchesFields; i++)
        {
            int[] a = Fields(profiles[i]);
            for (int j = 0; j < profiles.Count; j++)
            {
                bool fieldsEqual = a.SequenceEqual(Fields(profiles[j]));
                if (profiles[i].Equals(profiles[j]) != fieldsEqual || (profiles[i] == profiles[j]) != fieldsEqual
                    || (profiles[i] != profiles[j]) == fieldsEqual || (fieldsEqual && profiles[i].GetHashCode() != profiles[j].GetHashCode()))
                {
                    equalityMatchesFields = false;
                    break;
                }
            }
        }
        Check("A_EQUALITY_IS_EXACTLY_FIELDWISE", equalityMatchesFields, $"{profiles.Count}x{profiles.Count} pairs");
        Check("A_EVERY_COMBINATION_IS_DISTINCT", new HashSet<SiteConditionProfile>(profiles).Count == profiles.Count && profiles.Count == 5 * 5 * 5 * 5 * 2,
            $"combinations={profiles.Count}");

        // Each of the five descriptors can take every value while the other four stay exactly as they were.
        var baseline = new SiteConditionProfile(MoistureClass.Moist, DrainageAerationClass.Impeded, FertilityClass.High,
            ExposureClimateClass.Sheltered, RootingContextClass.Reference);
        int[] baselineFields = Fields(baseline);
        int[][] domain = ValuesPerDescriptor();
        var independent = new bool[5];
        for (int d = 0; d < 5; d++)
        {
            independent[d] = true;
            foreach (int value in domain[d].Where(v => v != baselineFields[d]))
            {
                int[] changed = (int[])baselineFields.Clone();
                changed[d] = value;
                SiteConditionProfile p = Make(changed);
                int[] after = Fields(p);
                for (int other = 0; other < 5; other++)
                    if (other != d && after[other] != baselineFields[other]) independent[d] = false;
                if (after[d] != value || p == baseline) independent[d] = false;
            }
        }
        Check("A_EACH_DESCRIPTOR_VARIES_WITHOUT_CHANGING_THE_OTHER_FOUR", independent.All(x => x),
            "moisture=" + independent[0] + " drainage/aeration=" + independent[1] + " fertility=" + independent[2]
            + " exposure/climate=" + independent[3] + " rooting=" + independent[4]);

        // The reason for the split: every moisture class can be paired with every drainage/aeration class.
        var pairings = new HashSet<(MoistureClass, DrainageAerationClass)>();
        foreach (MoistureClass m in Values<MoistureClass>())
            foreach (DrainageAerationClass a in Values<DrainageAerationClass>())
            {
                var p = new SiteConditionProfile(m, a, FertilityClass.Reference, ExposureClimateClass.Reference);
                if (p.Moisture == m && p.DrainageAeration == a) pairings.Add((m, a));
            }
        var moistWellAerated = new SiteConditionProfile(MoistureClass.Moist, DrainageAerationClass.WellAerated, FertilityClass.Reference, ExposureClimateClass.Reference);
        var moistWaterlogged = new SiteConditionProfile(MoistureClass.Moist, DrainageAerationClass.WaterloggedRisk, FertilityClass.Reference, ExposureClimateClass.Reference);
        var wetWellAerated = new SiteConditionProfile(MoistureClass.Wetter, DrainageAerationClass.WellAerated, FertilityClass.Reference, ExposureClimateClass.Reference);
        var wetWaterlogged = new SiteConditionProfile(MoistureClass.Wetter, DrainageAerationClass.WaterloggedRisk, FertilityClass.Reference, ExposureClimateClass.Reference);
        var drierWaterlogged = new SiteConditionProfile(MoistureClass.Drier, DrainageAerationClass.WaterloggedRisk, FertilityClass.Reference, ExposureClimateClass.Reference);
        Check("A_EVERY_MOISTURE_AND_DRAINAGE_PAIRING_IS_EXPRESSIBLE", pairings.Count == 25
            && moistWellAerated != moistWaterlogged && wetWellAerated != wetWaterlogged && wetWellAerated != moistWellAerated && drierWaterlogged != moistWaterlogged,
            "pairings=25: moist+well-aerated, moist+waterlogged, wetter+well-aerated and drier+waterlogged are four different sites");
    }

    // ---- B. Unknown is not Reference, in all five descriptors ----------------------------------------------------

    private static void CheckB_UnknownIsNotReference()
    {
        Check("B_UNKNOWN_IS_ZERO_AND_REFERENCE_IS_ONE_IN_ALL_FIVE_DESCRIPTORS",
            (int)MoistureClass.Unknown == 0 && (int)MoistureClass.Reference == 1
            && (int)DrainageAerationClass.Unknown == 0 && (int)DrainageAerationClass.Reference == 1
            && (int)FertilityClass.Unknown == 0 && (int)FertilityClass.Reference == 1
            && (int)ExposureClimateClass.Unknown == 0 && (int)ExposureClimateClass.Reference == 1
            && (int)RootingContextClass.Unknown == 0 && (int)RootingContextClass.Reference == 1
            && default(MoistureClass) == MoistureClass.Unknown && default(DrainageAerationClass) == DrainageAerationClass.Unknown
            && default(FertilityClass) == FertilityClass.Unknown && default(ExposureClimateClass) == ExposureClimateClass.Unknown
            && default(RootingContextClass) == RootingContextClass.Unknown,
            "default(enum) is therefore Unknown, never Reference");

        var unknown = default(SiteConditionProfile);
        var reference = new SiteConditionProfile(MoistureClass.Reference, DrainageAerationClass.Reference, FertilityClass.Reference,
            ExposureClimateClass.Reference, RootingContextClass.Reference);
        Check("B_DEFAULT_PROFILE_IS_ALL_UNKNOWN",
            Fields(unknown).All(v => v == 0) && unknown == SiteConditionProfile.Unspecified);
        Check("B_ALL_UNKNOWN_PROFILE_DIFFERS_FROM_ALL_REFERENCE_PROFILE", unknown != reference && !unknown.Equals(reference));

        // One descriptor at a time: Unknown and Reference are different states in each of the five.
        int[] allReference = Fields(reference);
        var alone = new bool[5];
        for (int d = 0; d < 5; d++)
        {
            int[] oneUnknown = (int[])allReference.Clone();
            oneUnknown[d] = 0;
            alone[d] = Make(oneUnknown) != reference && Fields(Make(oneUnknown))[d] == 0;
        }
        Check("B_UNKNOWN_DIFFERS_FROM_REFERENCE_IN_EACH_OF_THE_FIVE_DESCRIPTORS_ALONE", alone.All(x => x),
            "moisture=" + alone[0] + " drainage/aeration=" + alone[1] + " fertility=" + alone[2] + " exposure/climate=" + alone[3] + " rooting=" + alone[4]);

        bool bothNeutral = RegisteredSpecies.All(id => Bits(SiteGrowthResponse.Resolve(id, unknown)) == Bits(1f)
            && Bits(SiteGrowthResponse.Resolve(id, reference)) == Bits(1f));
        Check("B_BOTH_STATES_ARE_NUMERICALLY_NEUTRAL_YET_DISTINCT", bothNeutral && unknown != reference,
            "the same 1.0 does not make them the same state");

        // A scenario with no configured site is Unknown, never silently Reference.
        bool unrecognised = !ScenarioSiteProfiles.TryGet("scenario-two", out SiteConditionProfile other) && other == SiteConditionProfile.Unspecified
            && other != ScenarioSiteProfiles.ScenarioOneReference;
        Check("B_UNCONFIGURED_SCENARIO_IS_UNKNOWN_NOT_REFERENCE", unrecognised);

        // The compatibility reference is never recorded as suitable or unsuitable for a species.
        bool neverAssessed = SpeciesSiteResponses.All.All(r =>
            r.Moisture.SuitabilityOf(MoistureClass.Unknown) == SiteSuitability.NotAssessed
            && r.Moisture.SuitabilityOf(MoistureClass.Reference) == SiteSuitability.NotAssessed
            && r.DrainageAeration.SuitabilityOf(DrainageAerationClass.Unknown) == SiteSuitability.NotAssessed
            && r.DrainageAeration.SuitabilityOf(DrainageAerationClass.Reference) == SiteSuitability.NotAssessed
            && r.Fertility.SuitabilityOf(FertilityClass.Unknown) == SiteSuitability.NotAssessed
            && r.Fertility.SuitabilityOf(FertilityClass.Reference) == SiteSuitability.NotAssessed
            && r.Exposure.SuitabilityOf(ExposureClimateClass.Unknown) == SiteSuitability.NotAssessed
            && r.Exposure.SuitabilityOf(ExposureClimateClass.Reference) == SiteSuitability.NotAssessed);
        Check("B_NO_SPECIES_IS_CLAIMED_SUITED_TO_UNKNOWN_OR_REFERENCE", neverAssessed);
    }

    // ---- C. Every numerical response is exactly 1.0 -------------------------------------------------------------

    private static void CheckC_NeutralNumericalResponse(List<SiteConditionProfile> profiles)
    {
        int combinations = 0;
        bool allExactlyOne = true;
        foreach (string speciesId in RegisteredSpecies.Concat(UnregisteredSubjects))
        {
            foreach (SiteConditionProfile profile in profiles)
            {
                combinations++;
                if (Bits(SiteGrowthResponse.Resolve(speciesId, profile)) != Bits(1f)) allExactlyOne = false;
            }
        }
        Check("C_EVERY_SPECIES_SITE_COMBINATION_RETURNS_EXACTLY_1", allExactlyOne,
            $"combinations={combinations} (Sitka, Oak, Beech and species with no data, x all {profiles.Count} site profiles)");

        Check("C_NEUTRAL_CONSTANT_IS_EXACTLY_1", Bits(SpeciesSiteResponse.NeutralGrowthMultiplier) == Bits(1f));
        Check("C_NO_SPECIES_RESPONSE_IS_NUMERICALLY_ACTIVE", SpeciesSiteResponses.All.All(r => !r.NumericallyActive));
        Check("C_PER_SPECIES_MULTIPLIER_IS_1_AT_EVERY_SITE",
            SpeciesSiteResponses.All.All(r => profiles.All(p => Bits(r.GrowthMultiplier(p)) == Bits(1f))));
        Check("C_SCENARIO_ONE_SITE_IS_NEUTRAL_FOR_SITKA_OAK_BEECH",
            RegisteredSpecies.All(id => Bits(SiteGrowthResponse.Resolve(id, ScenarioSiteProfiles.ScenarioOneReference)) == Bits(1f)));
        Check("C_UNREGISTERED_SPECIES_HAVE_NO_RESPONSE_DATA",
            UnregisteredSubjects.All(id => !SpeciesSiteResponses.TryGet(id, out SpeciesSiteResponse none) && none == null));
    }

    // ---- D. Species keep different qualitative descriptors, each on its own axis ---------------------------------

    private static void CheckD_SpeciesIdentity()
    {
        SpeciesSiteResponse sitka = SpeciesSiteResponses.SitkaSpruce, oak = SpeciesSiteResponses.SessileOak, beech = SpeciesSiteResponses.Beech;

        Check("D_CATALOG_HOLDS_SITKA_OAK_BEECH_IN_FIXED_ORDER",
            SpeciesSiteResponses.All.Count == 3 && SpeciesSiteResponses.All[0] == sitka && SpeciesSiteResponses.All[1] == oak
            && SpeciesSiteResponses.All[2] == beech);
        Check("D_LOOKUP_RETURNS_THE_CATALOGUED_INSTANCE",
            SpeciesSiteResponses.TryGet("sitka-spruce", out SpeciesSiteResponse s) && s == sitka
            && SpeciesSiteResponses.TryGet("sessile-oak", out SpeciesSiteResponse o) && o == oak
            && SpeciesSiteResponses.TryGet("beech", out SpeciesSiteResponse b) && b == beech);

        // Sessile oak (research v2 sections 5 and 8): a freer-draining tendency, which is a DRAINAGE statement.
        Check("D_OAK_DRAINAGE_WELL_AERATED_PREFERRED_WATERLOGGING_LIMITING_IMPEDED_NOT_ASSESSED",
            oak.DrainageAeration.SuitabilityOf(DrainageAerationClass.WellAerated) == SiteSuitability.Preferred
            && oak.DrainageAeration.SuitabilityOf(DrainageAerationClass.WaterloggedRisk) == SiteSuitability.Limiting
            && oak.DrainageAeration.SuitabilityOf(DrainageAerationClass.Impeded) == SiteSuitability.NotAssessed
            && oak.DrainageAeration.AssessedCount == 2 && oak.DrainageAeration.Basis == SiteResponseBasis.DirectOrTransferEvidence);
        Check("D_OAK_MOISTURE_ENCODES_NO_DIRECTION_SO_DRY_IS_NEVER_GOOD",
            oak.Moisture.AssessedCount == 0 && oak.Moisture.Basis == SiteResponseBasis.Deferred
            && Values<MoistureClass>().All(c => oak.Moisture.SuitabilityOf(c) == SiteSuitability.NotAssessed),
            "free drainage being favoured does not make Drier favourable, and v2 denies a universal dry-site advantage");

        // Beech (research v2 sections 6 and 8): moist and drought are water availability; aeration and waterlogging are drainage.
        Check("D_BEECH_MOISTURE_MOIST_PREFERRED_DROUGHT_LIMITING_WETTER_NOT_ASSESSED",
            beech.Moisture.SuitabilityOf(MoistureClass.Moist) == SiteSuitability.Preferred
            && beech.Moisture.SuitabilityOf(MoistureClass.Drier) == SiteSuitability.Limiting
            && beech.Moisture.SuitabilityOf(MoistureClass.Wetter) == SiteSuitability.NotAssessed
            && beech.Moisture.AssessedCount == 2 && beech.Moisture.Basis == SiteResponseBasis.DirectOrTransferEvidence);
        Check("D_BEECH_DRAINAGE_WELL_AERATED_PREFERRED_WATERLOGGING_LIMITING_IMPEDED_NOT_ASSESSED",
            beech.DrainageAeration.SuitabilityOf(DrainageAerationClass.WellAerated) == SiteSuitability.Preferred
            && beech.DrainageAeration.SuitabilityOf(DrainageAerationClass.WaterloggedRisk) == SiteSuitability.Limiting
            && beech.DrainageAeration.SuitabilityOf(DrainageAerationClass.Impeded) == SiteSuitability.NotAssessed
            && beech.DrainageAeration.AssessedCount == 2 && beech.DrainageAeration.Basis == SiteResponseBasis.DirectOrTransferEvidence);
        Check("D_WETTER_IS_NOT_WATERLOGGING_FOR_ANY_SPECIES",
            SpeciesSiteResponses.All.All(r => r.Moisture.SuitabilityOf(MoistureClass.Wetter) == SiteSuitability.NotAssessed),
            "a waterlogging limitation lives on the drainage/aeration axis only (research v2 section 8)");

        // The split in action: beech gives the SAME moisture verdict and a DIFFERENT drainage verdict for two moist sites.
        var moistWellAerated = new SiteConditionProfile(MoistureClass.Moist, DrainageAerationClass.WellAerated, FertilityClass.Reference, ExposureClimateClass.Reference);
        var moistWaterlogged = new SiteConditionProfile(MoistureClass.Moist, DrainageAerationClass.WaterloggedRisk, FertilityClass.Reference, ExposureClimateClass.Reference);
        Check("D_BEECH_TELLS_MOIST_WELL_AERATED_FROM_MOIST_WATERLOGGED",
            Verdicts(beech, moistWellAerated) == "Preferred|Preferred|NotAssessed|NotAssessed"
            && Verdicts(beech, moistWaterlogged) == "Preferred|Limiting|NotAssessed|NotAssessed",
            "no combined category needed");

        // Fertility and exposure: only what v2 states.
        Check("D_FERTILITY_ONLY_OAK_LOW_TOLERATED_IS_STATED",
            oak.Fertility.SuitabilityOf(FertilityClass.Low) == SiteSuitability.Tolerated && oak.Fertility.AssessedCount == 1
            && oak.Fertility.Basis == SiteResponseBasis.DirectOrTransferEvidence
            && beech.Fertility.AssessedCount == 0 && beech.Fertility.Basis == SiteResponseBasis.Deferred);
        Check("D_EXPOSURE_DIRECTION_IS_DEFERRED_FOR_ALL_SPECIES",
            SpeciesSiteResponses.All.All(r => r.Exposure.AssessedCount == 0 && r.Exposure.Basis == SiteResponseBasis.Deferred));

        // Same neutral number, different qualitative meaning.
        var drierLowFertility = new SiteConditionProfile(MoistureClass.Drier, DrainageAerationClass.WellAerated, FertilityClass.Low, ExposureClimateClass.Moderate);
        bool disagreeYetNeutral =
            oak.Moisture.SuitabilityOf(drierLowFertility.Moisture) != beech.Moisture.SuitabilityOf(drierLowFertility.Moisture)
            && oak.Fertility.SuitabilityOf(drierLowFertility.Fertility) != beech.Fertility.SuitabilityOf(drierLowFertility.Fertility)
            && Verdicts(oak, drierLowFertility) != Verdicts(beech, drierLowFertility)
            && Bits(SiteGrowthResponse.Resolve("sessile-oak", drierLowFertility)) == Bits(1f)
            && Bits(SiteGrowthResponse.Resolve("beech", drierLowFertility)) == Bits(1f);
        Check("D_OAK_AND_BEECH_DISAGREE_QUALITATIVELY_YET_BOTH_RETURN_1", disagreeYetNeutral,
            "on a drier, well-aerated, low-fertility site: oak NotAssessed/Preferred/Tolerated, beech Limiting/Preferred/NotAssessed");
        var waterlogged = new SiteConditionProfile(MoistureClass.Reference, DrainageAerationClass.WaterloggedRisk, FertilityClass.High, ExposureClimateClass.Exposed);
        Check("D_WATERLOGGED_SITE_IS_LIMITING_FOR_BOTH_YET_NEUTRAL",
            oak.DrainageAeration.SuitabilityOf(waterlogged.DrainageAeration) == SiteSuitability.Limiting
            && beech.DrainageAeration.SuitabilityOf(waterlogged.DrainageAeration) == SiteSuitability.Limiting
            && Bits(SiteGrowthResponse.Resolve("beech", waterlogged)) == Bits(1f),
            "neutral 1.0 does not mean favourable");

        int moistureDiffers = Values<MoistureClass>().Count(c => oak.Moisture.SuitabilityOf(c) != beech.Moisture.SuitabilityOf(c));
        int drainageDiffers = Values<DrainageAerationClass>().Count(c => oak.DrainageAeration.SuitabilityOf(c) != beech.DrainageAeration.SuitabilityOf(c));
        int fertilityDiffers = Values<FertilityClass>().Count(c => oak.Fertility.SuitabilityOf(c) != beech.Fertility.SuitabilityOf(c));
        int exposureDiffers = Values<ExposureClimateClass>().Count(c => oak.Exposure.SuitabilityOf(c) != beech.Exposure.SuitabilityOf(c));
        Check("D_OAK_AND_BEECH_DESCRIPTORS_DIFFER_IN_MOISTURE_AND_FERTILITY_ONLY",
            moistureDiffers == 2 && drainageDiffers == 0 && fertilityDiffers == 1 && exposureDiffers == 0,
            $"differing classes: moisture={moistureDiffers} drainage/aeration={drainageDiffers} fertility={fertilityDiffers} exposure/climate={exposureDiffers}");

        Check("D_SITKA_HAS_NO_ASSESSMENTS_AND_KEEPS_LEGACY_ROUTE",
            sitka.UsesLegacySiteRoute && !oak.UsesLegacySiteRoute && !beech.UsesLegacySiteRoute
            && sitka.Moisture.AssessedCount == 0 && sitka.DrainageAeration.AssessedCount == 0 && sitka.Fertility.AssessedCount == 0
            && sitka.Exposure.AssessedCount == 0
            && sitka.Moisture.Basis == SiteResponseBasis.Deferred && sitka.DrainageAeration.Basis == SiteResponseBasis.Deferred
            && sitka.Fertility.Basis == SiteResponseBasis.Deferred && sitka.Exposure.Basis == SiteResponseBasis.Deferred);

        bool provenanceConsistent = SpeciesSiteResponses.All.All(r =>
            !string.IsNullOrWhiteSpace(r.Moisture.Note) && !string.IsNullOrWhiteSpace(r.DrainageAeration.Note)
            && !string.IsNullOrWhiteSpace(r.Fertility.Note) && !string.IsNullOrWhiteSpace(r.Exposure.Note)
            && (r.Moisture.Basis == SiteResponseBasis.Deferred) == (r.Moisture.AssessedCount == 0)
            && (r.DrainageAeration.Basis == SiteResponseBasis.Deferred) == (r.DrainageAeration.AssessedCount == 0)
            && (r.Fertility.Basis == SiteResponseBasis.Deferred) == (r.Fertility.AssessedCount == 0)
            && (r.Exposure.Basis == SiteResponseBasis.Deferred) == (r.Exposure.AssessedCount == 0));
        Check("D_EVIDENCE_BASIS_NEVER_CLAIMS_A_DIRECTION_THAT_IS_NOT_ENCODED", provenanceConsistent);
        Check("D_NO_DATA_USES_THE_CALIBRATION_BASIS",
            SpeciesSiteResponses.All.All(r => r.Moisture.Basis != SiteResponseBasis.Calibration && r.DrainageAeration.Basis != SiteResponseBasis.Calibration
                && r.Fertility.Basis != SiteResponseBasis.Calibration && r.Exposure.Basis != SiteResponseBasis.Calibration));
    }

    // ---- E. Scenario One's canonical site -----------------------------------------------------------------------

    private static void CheckE_ScenarioOneReference()
    {
        bool found = ScenarioSiteProfiles.TryGet("scenario-one", out SiteConditionProfile site);
        Check("E_SCENARIO_ONE_SITE_IS_CONFIGURED", found && site == ScenarioSiteProfiles.ScenarioOneReference);
        Check("E_SCENARIO_ONE_IS_REFERENCE_REFERENCE_REFERENCE_REFERENCE_UNKNOWN",
            site.Moisture == MoistureClass.Reference && site.DrainageAeration == DrainageAerationClass.Reference
            && site.Fertility == FertilityClass.Reference && site.ExposureClimate == ExposureClimateClass.Reference
            && site.RootingContext == RootingContextClass.Unknown,
            "moisture / drainage-aeration / fertility / exposure-climate / rooting");
        Check("E_SCENARIO_ONE_ROOTING_IS_UNKNOWN_BECAUSE_V2_LEAVES_IT_UNSPECIFIED",
            site.RootingContext == RootingContextClass.Unknown && site != SiteConditionProfile.Unspecified && Fields(site).Count(v => v == 1) == 4,
            "so the canonical site itself shows Unknown and Reference as different states");
        Check("E_SCENARIO_ONE_SITE_IS_VERSIONED", ScenarioSiteProfiles.ScenarioOneReferenceVersion == 1);
        Check("E_SCENARIO_ONE_SITE_RESOLVES_TO_1_FOR_EVERY_SPECIES",
            RegisteredSpecies.Concat(UnregisteredSubjects).All(id => Bits(SiteGrowthResponse.Resolve(id, site)) == Bits(1f)));
        Check("E_SCENARIO_ID_MATCHING_IS_ORDINAL",
            new[] { "Scenario-One", " scenario-one", "scenario-one ", "", null }.All(id => !ScenarioSiteProfiles.TryGet(id, out SiteConditionProfile x) && x == SiteConditionProfile.Unspecified));

        // The id used as the key must be the one the project really uses (read from source, no coupling).
        string definition = ReadSource("Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs");
        string saveData = ReadSource("Assets/ForestPrototype/ForestSaveData.cs");
        Check("E_SCENARIO_ID_MATCHES_DEFINITION_AND_SAVE_DEFAULT",
            ScenarioSiteProfiles.ScenarioOneId == "scenario-one"
            && Regex.IsMatch(definition, "scenarioId\\s*=\\s*\"scenario-one\"") && Regex.IsMatch(saveData, "scenarioId\\s*=\\s*\"scenario-one\""));

        // Species ids must be the ids on the species assets.
        string speciesFolder = Path.Combine(root, "Assets", "ForestPrototype", "Species");
        var assetIds = new List<string>();
        foreach (string file in Directory.EnumerateFiles(speciesFolder, "*.asset"))
        {
            Match match = Regex.Match(File.ReadAllText(file), "^\\s*speciesId:\\s*(\\S+)\\s*$", RegexOptions.Multiline);
            if (match.Success) assetIds.Add(match.Groups[1].Value);
        }
        Check("E_SPECIES_IDS_MATCH_THE_SPECIES_ASSETS",
            SameSet(assetIds, RegisteredSpecies) && assetIds.Count == RegisteredSpecies.Length,
            "assets: " + string.Join(", ", assetIds.OrderBy(x => x, StringComparer.Ordinal)));
        Check("E_SITKA_ID_MATCHES_SITKA_GROWTH_MODEL_SOURCE",
            Regex.IsMatch(ReadSource("Assets/ForestPrototype/SitkaGrowthModel.cs"), "SpeciesId\\s*=\\s*\"" + Regex.Escape(SpeciesSiteResponses.SitkaSpruceId) + "\""));
    }

    // ---- F. Determinism -----------------------------------------------------------------------------------------

    private static void CheckF_Determinism(List<SiteConditionProfile> profiles)
    {
        List<int> first = Snapshot(profiles), second = Snapshot(profiles);
        Check("F_FULL_LOOKUP_TABLE_IS_IDENTICAL_ACROSS_PASSES", first.SequenceEqual(second), $"values={first.Count}");

        var oakDry = new SiteConditionProfile(MoistureClass.Drier, DrainageAerationClass.WellAerated, FertilityClass.Low, ExposureClimateClass.Moderate);
        int reference = Bits(SiteGrowthResponse.Resolve("sessile-oak", oakDry));
        bool repeated = true;
        for (int i = 0; i < 1000; i++)
            if (Bits(SiteGrowthResponse.Resolve("sessile-oak", oakDry)) != reference) repeated = false;
        Check("F_1000_REPEATED_LOOKUPS_ARE_BIT_IDENTICAL", repeated);

        bool descriptors = true;
        for (int i = 0; i < 200; i++)
            if (SpeciesSiteResponses.Beech.Moisture.SuitabilityOf(MoistureClass.Moist) != SiteSuitability.Preferred
                || SpeciesSiteResponses.Beech.DrainageAeration.SuitabilityOf(DrainageAerationClass.WaterloggedRisk) != SiteSuitability.Limiting
                || !SpeciesSiteResponses.TryGet("beech", out SpeciesSiteResponse again) || again != SpeciesSiteResponses.Beech)
                descriptors = false;
        Check("F_DESCRIPTOR_LOOKUPS_ARE_STABLE", descriptors);
        Check("F_LOOKUP_DOES_NOT_MUTATE_THE_PROFILE",
            oakDry == new SiteConditionProfile(MoistureClass.Drier, DrainageAerationClass.WellAerated, FertilityClass.Low, ExposureClimateClass.Moderate));
    }

    private static List<int> Snapshot(List<SiteConditionProfile> profiles)
    {
        var values = new List<int>();
        foreach (string id in RegisteredSpecies.Concat(UnregisteredSubjects))
            foreach (SiteConditionProfile profile in profiles)
                values.Add(Bits(SiteGrowthResponse.Resolve(id, profile)));
        foreach (SpeciesSiteResponse response in SpeciesSiteResponses.All)
        {
            foreach (MoistureClass c in Values<MoistureClass>()) values.Add((int)response.Moisture.SuitabilityOf(c));
            foreach (DrainageAerationClass c in Values<DrainageAerationClass>()) values.Add((int)response.DrainageAeration.SuitabilityOf(c));
            foreach (FertilityClass c in Values<FertilityClass>()) values.Add((int)response.Fertility.SuitabilityOf(c));
            foreach (ExposureClimateClass c in Values<ExposureClimateClass>()) values.Add((int)response.Exposure.SuitabilityOf(c));
        }
        return values;
    }

    // ---- G. No save impact --------------------------------------------------------------------------------------

    private static void CheckG_NoSaveImpact()
    {
        string saveData = ReadSource("Assets/ForestPrototype/ForestSaveData.cs");
        Check("G_SAVE_VERSION_IS_STILL_21", Regex.IsMatch(saveData, "public\\s+const\\s+int\\s+CurrentVersion\\s*=\\s*21\\s*;"));

        string[] saveSources = Directory.EnumerateFiles(Path.Combine(root, "Assets"), "ForestSave*.cs", SearchOption.AllDirectories).ToArray();
        var typeRegex = TypeNameRegex();
        Check("G_NO_SAVE_FILE_MENTIONS_ANY_NEW_TYPE", saveSources.Length >= 4 && saveSources.All(f => !typeRegex.IsMatch(File.ReadAllText(f))),
            $"save sources scanned={saveSources.Length}");

        // Nothing in the new files can be written to a save or an asset.
        string[] unitySerialisation = { "UnityEngine", "[Serializable]", "SerializeField", "JsonUtility", "PlayerPrefs", "ScriptableObject", "MonoBehaviour", "Newtonsoft" };
        Check("G_NEW_TYPES_ARE_NOT_SERIALISABLE_AND_USE_NO_UNITY_API",
            NewProductionFiles().All(f => unitySerialisation.All(token => !File.ReadAllText(f).Contains(token))));
    }

    // ---- H. No current growth effect ----------------------------------------------------------------------------

    private static void CheckH_NoCurrentGrowthEffect()
    {
        var typeRegex = TypeNameRegex();
        string[] ownFiles = NewProductionFiles().ToArray();

        // The scanner must be able to see every new name in the new files, or the "none found" results below mean nothing.
        var found = new HashSet<string>();
        foreach (string file in ownFiles)
            foreach (Match match in typeRegex.Matches(File.ReadAllText(file))) found.Add(match.Value);
        Check("H_SCANNER_SEES_EVERY_NEW_TYPE_NAME_IN_THE_NEW_FILES", found.SetEquals(NewTypeNames), $"found={found.Count}/{NewTypeNames.Length}");

        string[] production = Directory.EnumerateFiles(Path.Combine(root, "Assets"), "*.cs", SearchOption.AllDirectories)
            .Where(f => !ownFiles.Contains(f)).ToArray();
        string[] mustBeScanned =
        {
            "ForestEcologyController.cs", "SitkaGrowthModel.cs", "MixedDensityModel.cs", "TreeSpeciesDefinition.cs", "AdultMortalityAccount.cs",
            "JuvenileEcologyRules.cs", "StormModel.cs", "RegenerationModel.cs", "ForestSaveData.cs", "ForestSaveController.cs",
        };
        Check("H_SCAN_COVERS_THE_GROWTH_DENSITY_AND_SAVE_SOURCES",
            production.Length > 100 && mustBeScanned.All(name => production.Any(f => Path.GetFileName(f) == name)),
            $"production files scanned={production.Length}");
        string[] referencing = production.Where(f => typeRegex.IsMatch(File.ReadAllText(f))).Select(f => Path.GetRelativePath(root, f)).ToArray();
        Check("H_NO_PRODUCTION_FILE_REFERENCES_THE_NEW_TYPES_OR_RESOLVER", referencing.Length == 0,
            referencing.Length == 0 ? "GrowAdults, Sitka growth, competition, promotion, density, mortality and UI do not call the seam" : string.Join(", ", referencing));

        string toolsRoot = Path.Combine(root, "Tools", "Verification");
        string ownTools = Path.Combine(toolsRoot, "SiteInfrastructure") + Path.DirectorySeparatorChar;
        string[] tools = Directory.EnumerateFiles(toolsRoot, "*.cs", SearchOption.AllDirectories).Where(f => !f.StartsWith(ownTools, StringComparison.Ordinal)).ToArray();
        string[] toolReferences = tools.Where(f => typeRegex.IsMatch(File.ReadAllText(f))).Select(RelativePath).ToArray();
        string[] unlisted = toolReferences.Where(p => !IsVerificationAllowed(p)).ToArray();
        Check("H_NO_UNLISTED_VERIFICATION_TOOL_REFERENCES_THE_NEW_TYPES",
            tools.Length > 20 && unlisted.Length == 0,
            $"tool files scanned={tools.Length} allowed={toolReferences.Length - unlisted.Length}"
            + (unlisted.Length == 0 ? "" : " unlisted: " + string.Join(", ", unlisted)));
        CheckH_VerificationAllowListControls(typeRegex);

        // The data files hold no numeric curve: the only numeric literal in code is the neutral 1f.
        var literals = new SortedSet<string>(StringComparer.Ordinal);
        foreach (string file in ownFiles)
            foreach (Match match in Regex.Matches(CodeOnly(File.ReadAllText(file)), "(?<![\\w.])\\d+\\.\\d+[fFdDmM]?(?![\\w.])|(?<![\\w.])\\d+[fFdDmM](?![\\w.])"))
                literals.Add(match.Value);
        Check("H_ONLY_NUMERIC_LITERAL_IN_THE_DATA_FILES_IS_THE_NEUTRAL_1F", literals.SetEquals(new[] { "1f" }), "literals: " + string.Join(", ", literals));

        string[] forbidden = { "SiteIndex", "SiteQuality", "SiteScore", "Weighted", "Ci50", "Productivity", "Mathf", "Random", "DateTime", "Stopwatch" };
        string[] offending = ownFiles.SelectMany(f => forbidden.Where(token => CodeOnly(File.ReadAllText(f)).Contains(token)).Select(token => Path.GetFileName(f) + ":" + token)).ToArray();
        Check("H_NO_UNIVERSAL_SITE_SCALAR_WEIGHTING_COMPETITION_OR_NONDETERMINISM_IN_THE_DATA_FILES", offending.Length == 0, string.Join(", ", offending));
    }

    // ---- H controls: the allow-list is narrow, and production is not affected by it ------------------------------

    private static void CheckH_VerificationAllowListControls(Regex typeRegex)
    {
        const string withReference = "SpeciesSiteResponses.Beech";
        const string withoutReference = "int value = 1;";
        string allowed = VerificationAllowList[0];
        string unlisted = "Tools/Verification/Synthetic/UnlistedConsumer.cs";

        Check("H_ALLOW_LIST_ACCEPTS_AN_ALLOW_LISTED_REFERENCE",
            UnlistedToolReferences(new[] { (allowed, withReference) }, VerificationAllowList, typeRegex).Length == 0,
            "path=" + allowed);
        Check("H_ALLOW_LIST_REJECTS_AN_UNLISTED_SYNTHETIC_REFERENCE",
            UnlistedToolReferences(new[] { (unlisted, withReference) }, VerificationAllowList, typeRegex).Length == 1,
            "path=" + unlisted);
        Check("H_ALLOW_LIST_ACCEPTS_A_CLEAN_FILE_WITHOUT_A_REFERENCE",
            UnlistedToolReferences(new[] { (unlisted, withoutReference) }, VerificationAllowList, typeRegex).Length == 0);
        Check("H_ALLOW_LIST_REJECTS_DIRECTORY_AND_PATTERN_WIDE_CONSUMERS",
            UnlistedToolReferences(new[] { (allowed, withReference) }, new[] { "Tools/Verification/ThreeSpecies" }, typeRegex).Length == 1
            && UnlistedToolReferences(new[] { (allowed, withReference) }, new[] { "Tools/Verification/ThreeSpecies/*.cs" }, typeRegex).Length == 1);
        Check("H_ALLOW_LIST_ENTRIES_ARE_EXPLICIT_EXISTING_CS_FILES",
            VerificationAllowList.Length > 0 && VerificationAllowList.Length <= 5
            && VerificationAllowList.Distinct(StringComparer.Ordinal).Count() == VerificationAllowList.Length
            && VerificationAllowList.All(p => p.StartsWith("Tools/Verification/", StringComparison.Ordinal)
                && p.EndsWith(".cs", StringComparison.Ordinal) && !p.Contains("*") && !p.Contains("?") && !p.EndsWith("/", StringComparison.Ordinal)
                && File.Exists(Path.Combine(root, p.Replace('/', Path.DirectorySeparatorChar)))));
        Check("H_PRODUCTION_REFERENCES_ARE_REJECTED_EVEN_IF_A_PATH_LOOKS_ALLOW_LISTED",
            UnlistedToolReferences(new[] { ("Assets/ForestPrototype/SyntheticProductionConsumer.cs", withReference) }, VerificationAllowList, typeRegex).Length == 1);
    }

    // ---- I. Invalid input is rejected, not coerced --------------------------------------------------------------

    private static void CheckI_InputValidation()
    {
        const MoistureClass m = MoistureClass.Reference;
        const DrainageAerationClass d = DrainageAerationClass.Reference;
        const FertilityClass f = FertilityClass.Low;
        const ExposureClimateClass e = ExposureClimateClass.Moderate;
        Check("I_UNDEFINED_ENUM_VALUES_ARE_REJECTED_IN_ALL_FIVE_DESCRIPTORS",
            Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile((MoistureClass)99, d, f, e))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(m, (DrainageAerationClass)99, f, e))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(m, d, (FertilityClass)99, e))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(m, d, f, (ExposureClimateClass)99))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(m, d, f, e, (RootingContextClass)99)));
        Check("I_AXIS_RESPONSE_REJECTS_UNKNOWN_AND_REFERENCE_ENTRIES",
            Throws<ArgumentException>(() => new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.Ordinal, "n", (DrainageAerationClass.Unknown, SiteSuitability.Preferred)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<MoistureClass>(SiteResponseBasis.Ordinal, "n", (MoistureClass.Reference, SiteSuitability.Preferred))));
        Check("I_AXIS_RESPONSE_REJECTS_INCONSISTENT_BASIS",
            Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Deferred, "n", (FertilityClass.Low, SiteSuitability.Tolerated)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n")));
        Check("I_AXIS_RESPONSE_REJECTS_DUPLICATES_NOTASSESSED_ENTRIES_AND_EMPTY_NOTES",
            Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n",
                (FertilityClass.Low, SiteSuitability.Tolerated), (FertilityClass.Low, SiteSuitability.Limiting)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n", (FertilityClass.Low, SiteSuitability.NotAssessed)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Deferred, "")));
        SpeciesSiteResponse beech = SpeciesSiteResponses.Beech;
        Check("I_SPECIES_RESPONSE_REJECTS_MISSING_ID_OR_AXES",
            Throws<ArgumentException>(() => new SpeciesSiteResponse("", beech.Moisture, beech.DrainageAeration, beech.Fertility, beech.Exposure, false))
            && Throws<ArgumentNullException>(() => new SpeciesSiteResponse("x", null, beech.DrainageAeration, beech.Fertility, beech.Exposure, false))
            && Throws<ArgumentNullException>(() => new SpeciesSiteResponse("x", beech.Moisture, null, beech.Fertility, beech.Exposure, false)));
    }

    // ---- helpers ------------------------------------------------------------------------------------------------

    private static string ReadSource(string relativePath) => File.ReadAllText(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static IEnumerable<string> NewProductionFiles()
    {
        string folder = Path.Combine(root, "Assets", "ForestPrototype");
        return new[] { "ForestSiteProfile.cs", "SpeciesSiteResponse.cs", "ScenarioSiteProfiles.cs" }.Select(name => Path.Combine(folder, name));
    }

    private static Regex TypeNameRegex() => new Regex("\\b(" + string.Join("|", NewTypeNames) + ")\\b");

    private static string RelativePath(string absolutePath) =>
        Path.GetRelativePath(root, absolutePath).Replace('\\', '/');

    private static bool IsVerificationAllowed(string relativePath) =>
        VerificationAllowList.Any(p => string.Equals(p, relativePath.Replace('\\', '/'), StringComparison.Ordinal));

    private static string[] UnlistedToolReferences((string Path, string Text)[] files, string[] allowList, Regex typeRegex) =>
        files.Where(f => !allowList.Any(p => string.Equals(p, f.Path.Replace('\\', '/'), StringComparison.Ordinal)) && typeRegex.IsMatch(f.Text))
            .Select(f => f.Path).ToArray();

    // Source with line comments and string literals removed, so prose in comments and notes is not mistaken for code.
    private static string CodeOnly(string source)
    {
        string withoutComments = Regex.Replace(source, "//[^\\r\\n]*", "");
        return Regex.Replace(withoutComments, "\"(\\\\.|[^\"\\\\\\r\\n])*\"", "\"\"");
    }
}

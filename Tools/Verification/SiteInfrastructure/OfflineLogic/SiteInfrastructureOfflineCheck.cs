// Offline verification for the dormant Oak/Beech site infrastructure v1:
//   Assets/ForestPrototype/ForestSiteProfile.cs, SpeciesSiteResponse.cs, ScenarioSiteProfiles.cs
//
// run_site_check.py builds this with ONLY those three production files, so a successful build is itself the proof that
// they depend on nothing else: no UnityEngine, SitkaGrowthModel, TreeSpeciesDefinition, ForestEcologyController or save class.
//
// PROVES: the site axes are independent; Unknown and Reference stay distinct; every numerical response is exactly 1.0;
// Oak/Beech/Sitka carry different qualitative descriptors; the Scenario One site is explicit; lookups are deterministic;
// and, by reading source text, that no production or tool code calls the new types and the save version is unchanged.
// DOES NOT PROVE: that any descriptor is ecologically right, that Unity imports or compiles the files, or any anchor hash.
//
// Guard H is about THIS packet. The future packet that deliberately wires the seam into growth must change guard H on
// purpose, in review, rather than have it pass by accident.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        "SiteConditionProfile", "MoistureDrainageClass", "FertilityClass", "ExposureClimateClass", "RootingContextClass",
        "SiteSuitability", "SiteResponseBasis", "SiteAxisResponse", "SpeciesSiteResponse", "SpeciesSiteResponses",
        "SiteGrowthResponse", "ScenarioSiteProfiles",
    };

    private static List<SiteConditionProfile> AllProfiles()
    {
        var list = new List<SiteConditionProfile>();
        foreach (MoistureDrainageClass moisture in Values<MoistureDrainageClass>())
            foreach (FertilityClass fertility in Values<FertilityClass>())
                foreach (ExposureClimateClass exposure in Values<ExposureClimateClass>())
                    foreach (RootingContextClass rooting in Values<RootingContextClass>())
                        list.Add(new SiteConditionProfile(moisture, fertility, exposure, rooting));
        return list;
    }

    private static bool SameSet(IEnumerable<string> actual, params string[] expected) =>
        new HashSet<string>(actual).SetEquals(expected);

    public static int Main(string[] args)
    {
        root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        List<SiteConditionProfile> profiles = AllProfiles();

        CheckA_SiteAxesAreDistinct(profiles);
        CheckB_UnknownIsNotReference(profiles);
        CheckC_NeutralNumericalResponse(profiles);
        CheckD_SpeciesIdentity();
        CheckE_ScenarioOneReference(profiles);
        CheckF_Determinism(profiles);
        CheckG_NoSaveImpact();
        CheckH_NoCurrentGrowthEffect();
        CheckI_InputValidation();

        Console.WriteLine(failures == 0
            ? $"SITE_INFRASTRUCTURE_PASS checks={checks} siteProfiles={profiles.Count} registeredSpecies={RegisteredSpecies.Length} isolatedCompile=true"
            : $"SITE_INFRASTRUCTURE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    // ---- A. The site axes are separate fields, not one scalar ---------------------------------------------------

    private static void CheckA_SiteAxesAreDistinct(List<SiteConditionProfile> profiles)
    {
        Check("A_DOMAIN_CATEGORIES_AS_SPECIFIED",
            SameSet(Names<MoistureDrainageClass>(), "Unknown", "Reference", "DrierFreeDraining", "MoistWellAerated", "PoorlyDrainedWaterloggingRisk")
            && SameSet(Names<FertilityClass>(), "Unknown", "Reference", "Low", "Moderate", "High")
            && SameSet(Names<ExposureClimateClass>(), "Unknown", "Reference", "Sheltered", "Moderate", "Exposed")
            && SameSet(Names<RootingContextClass>(), "Unknown", "Reference"),
            "no category was added beyond the packet's");

        var domainTypes = new[] { typeof(MoistureDrainageClass), typeof(FertilityClass), typeof(ExposureClimateClass), typeof(RootingContextClass) };
        Check("A_FOUR_DISTINCT_DOMAIN_TYPES", domainTypes.Distinct().Count() == 4);

        // Every combination is a distinct value, and equality is exactly "all four fields equal".
        bool equalityMatchesFields = true;
        for (int i = 0; i < profiles.Count && equalityMatchesFields; i++)
        {
            for (int j = 0; j < profiles.Count; j++)
            {
                SiteConditionProfile a = profiles[i], b = profiles[j];
                bool fieldsEqual = a.MoistureDrainage == b.MoistureDrainage && a.Fertility == b.Fertility
                    && a.ExposureClimate == b.ExposureClimate && a.RootingContext == b.RootingContext;
                if (a.Equals(b) != fieldsEqual || (a == b) != fieldsEqual || (a != b) == fieldsEqual
                    || (fieldsEqual && a.GetHashCode() != b.GetHashCode()))
                {
                    equalityMatchesFields = false;
                    break;
                }
            }
        }
        Check("A_EQUALITY_IS_EXACTLY_FIELDWISE", equalityMatchesFields, $"{profiles.Count}x{profiles.Count} pairs");
        Check("A_EVERY_COMBINATION_IS_DISTINCT", new HashSet<SiteConditionProfile>(profiles).Count == profiles.Count && profiles.Count == 5 * 5 * 5 * 2,
            $"combinations={profiles.Count}");

        // Changing one domain must change nothing else.
        var baseline = new SiteConditionProfile(MoistureDrainageClass.DrierFreeDraining, FertilityClass.High,
            ExposureClimateClass.Sheltered, RootingContextClass.Reference);
        bool moistureOnly = Values<MoistureDrainageClass>().Where(v => v != baseline.MoistureDrainage).All(v =>
        {
            var p = new SiteConditionProfile(v, baseline.Fertility, baseline.ExposureClimate, baseline.RootingContext);
            return p != baseline && p.MoistureDrainage == v && p.Fertility == baseline.Fertility
                && p.ExposureClimate == baseline.ExposureClimate && p.RootingContext == baseline.RootingContext;
        });
        bool fertilityOnly = Values<FertilityClass>().Where(v => v != baseline.Fertility).All(v =>
        {
            var p = new SiteConditionProfile(baseline.MoistureDrainage, v, baseline.ExposureClimate, baseline.RootingContext);
            return p != baseline && p.Fertility == v && p.MoistureDrainage == baseline.MoistureDrainage
                && p.ExposureClimate == baseline.ExposureClimate && p.RootingContext == baseline.RootingContext;
        });
        bool exposureOnly = Values<ExposureClimateClass>().Where(v => v != baseline.ExposureClimate).All(v =>
        {
            var p = new SiteConditionProfile(baseline.MoistureDrainage, baseline.Fertility, v, baseline.RootingContext);
            return p != baseline && p.ExposureClimate == v && p.MoistureDrainage == baseline.MoistureDrainage
                && p.Fertility == baseline.Fertility && p.RootingContext == baseline.RootingContext;
        });
        bool rootingOnly = Values<RootingContextClass>().Where(v => v != baseline.RootingContext).All(v =>
        {
            var p = new SiteConditionProfile(baseline.MoistureDrainage, baseline.Fertility, baseline.ExposureClimate, v);
            return p != baseline && p.RootingContext == v && p.MoistureDrainage == baseline.MoistureDrainage
                && p.Fertility == baseline.Fertility && p.ExposureClimate == baseline.ExposureClimate;
        });
        Check("A_MOISTURE_FERTILITY_EXPOSURE_ROOTING_VARY_INDEPENDENTLY", moistureOnly && fertilityOnly && exposureOnly && rootingOnly);
    }

    // ---- B. Unknown is not Reference ----------------------------------------------------------------------------

    private static void CheckB_UnknownIsNotReference(List<SiteConditionProfile> profiles)
    {
        Check("B_UNKNOWN_IS_ZERO_AND_REFERENCE_IS_ONE_IN_EVERY_DOMAIN",
            (int)MoistureDrainageClass.Unknown == 0 && (int)MoistureDrainageClass.Reference == 1
            && (int)FertilityClass.Unknown == 0 && (int)FertilityClass.Reference == 1
            && (int)ExposureClimateClass.Unknown == 0 && (int)ExposureClimateClass.Reference == 1
            && (int)RootingContextClass.Unknown == 0 && (int)RootingContextClass.Reference == 1,
            "default(enum) is therefore Unknown, never Reference");

        var unknown = default(SiteConditionProfile);
        var reference = new SiteConditionProfile(MoistureDrainageClass.Reference, FertilityClass.Reference,
            ExposureClimateClass.Reference, RootingContextClass.Reference);
        Check("B_DEFAULT_PROFILE_IS_ALL_UNKNOWN",
            unknown.MoistureDrainage == MoistureDrainageClass.Unknown && unknown.Fertility == FertilityClass.Unknown
            && unknown.ExposureClimate == ExposureClimateClass.Unknown && unknown.RootingContext == RootingContextClass.Unknown
            && unknown == SiteConditionProfile.Unspecified);
        Check("B_ALL_UNKNOWN_PROFILE_DIFFERS_FROM_ALL_REFERENCE_PROFILE", unknown != reference && !unknown.Equals(reference));

        // One domain at a time: Unknown and Reference are different states in each of the four domains.
        bool eachDomain =
            new SiteConditionProfile(MoistureDrainageClass.Unknown, FertilityClass.Reference, ExposureClimateClass.Reference, RootingContextClass.Reference)
                != reference
            && new SiteConditionProfile(MoistureDrainageClass.Reference, FertilityClass.Unknown, ExposureClimateClass.Reference, RootingContextClass.Reference)
                != reference
            && new SiteConditionProfile(MoistureDrainageClass.Reference, FertilityClass.Reference, ExposureClimateClass.Unknown, RootingContextClass.Reference)
                != reference
            && new SiteConditionProfile(MoistureDrainageClass.Reference, FertilityClass.Reference, ExposureClimateClass.Reference, RootingContextClass.Unknown)
                != reference;
        Check("B_UNKNOWN_DIFFERS_FROM_REFERENCE_IN_EACH_DOMAIN_ALONE", eachDomain);

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
            r.Moisture.SuitabilityOf(MoistureDrainageClass.Unknown) == SiteSuitability.NotAssessed
            && r.Moisture.SuitabilityOf(MoistureDrainageClass.Reference) == SiteSuitability.NotAssessed
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

    // ---- D. Species keep different qualitative descriptors -----------------------------------------------------

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

        // Moisture/drainage directions, exactly as research v2 sections 5, 6 and 8 state them.
        Check("D_OAK_MOISTURE_FREER_DRAINING_PREFERRED_WET_LIMITING_MOIST_NOT_ASSESSED",
            oak.Moisture.SuitabilityOf(MoistureDrainageClass.DrierFreeDraining) == SiteSuitability.Preferred
            && oak.Moisture.SuitabilityOf(MoistureDrainageClass.PoorlyDrainedWaterloggingRisk) == SiteSuitability.Limiting
            && oak.Moisture.SuitabilityOf(MoistureDrainageClass.MoistWellAerated) == SiteSuitability.NotAssessed
            && oak.Moisture.Basis == SiteResponseBasis.DirectOrTransferEvidence);
        Check("D_BEECH_MOISTURE_MOIST_AERATED_PREFERRED_DROUGHT_AND_WATERLOGGING_LIMITING",
            beech.Moisture.SuitabilityOf(MoistureDrainageClass.MoistWellAerated) == SiteSuitability.Preferred
            && beech.Moisture.SuitabilityOf(MoistureDrainageClass.DrierFreeDraining) == SiteSuitability.Limiting
            && beech.Moisture.SuitabilityOf(MoistureDrainageClass.PoorlyDrainedWaterloggingRisk) == SiteSuitability.Limiting
            && beech.Moisture.Basis == SiteResponseBasis.DirectOrTransferEvidence);
        // Only what v2 states is encoded; the rest is deliberately left unassessed rather than invented.
        Check("D_FERTILITY_ONLY_OAK_LOW_TOLERATED_IS_STATED",
            oak.Fertility.SuitabilityOf(FertilityClass.Low) == SiteSuitability.Tolerated && oak.Fertility.AssessedCount == 1
            && oak.Fertility.Basis == SiteResponseBasis.DirectOrTransferEvidence
            && beech.Fertility.AssessedCount == 0 && beech.Fertility.Basis == SiteResponseBasis.Deferred);
        Check("D_EXPOSURE_DIRECTION_IS_DEFERRED_FOR_ALL_SPECIES",
            SpeciesSiteResponses.All.All(r => r.Exposure.AssessedCount == 0 && r.Exposure.Basis == SiteResponseBasis.Deferred));

        // Same neutral number, different qualitative meaning.
        var dry = new SiteConditionProfile(MoistureDrainageClass.DrierFreeDraining, FertilityClass.Low, ExposureClimateClass.Moderate);
        var wet = new SiteConditionProfile(MoistureDrainageClass.PoorlyDrainedWaterloggingRisk, FertilityClass.High, ExposureClimateClass.Exposed);
        bool disagreeYetNeutral =
            oak.Moisture.SuitabilityOf(dry.MoistureDrainage) != beech.Moisture.SuitabilityOf(dry.MoistureDrainage)
            && oak.Fertility.SuitabilityOf(dry.Fertility) != beech.Fertility.SuitabilityOf(dry.Fertility)
            && Bits(SiteGrowthResponse.Resolve("sessile-oak", dry)) == Bits(1f)
            && Bits(SiteGrowthResponse.Resolve("beech", dry)) == Bits(1f);
        Check("D_OAK_AND_BEECH_DISAGREE_QUALITATIVELY_YET_BOTH_RETURN_1", disagreeYetNeutral,
            "on a drier free-draining low-fertility site: oak Preferred/Tolerated vs beech Limiting/NotAssessed");
        Check("D_WATERLOGGED_SITE_IS_LIMITING_FOR_BOTH_YET_NEUTRAL",
            oak.Moisture.SuitabilityOf(wet.MoistureDrainage) == SiteSuitability.Limiting
            && beech.Moisture.SuitabilityOf(wet.MoistureDrainage) == SiteSuitability.Limiting
            && Bits(SiteGrowthResponse.Resolve("beech", wet)) == Bits(1f),
            "neutral 1.0 does not mean favourable");

        int differingCells = 0;
        foreach (MoistureDrainageClass c in Values<MoistureDrainageClass>())
            if (oak.Moisture.SuitabilityOf(c) != beech.Moisture.SuitabilityOf(c)) differingCells++;
        foreach (FertilityClass c in Values<FertilityClass>())
            if (oak.Fertility.SuitabilityOf(c) != beech.Fertility.SuitabilityOf(c)) differingCells++;
        foreach (ExposureClimateClass c in Values<ExposureClimateClass>())
            if (oak.Exposure.SuitabilityOf(c) != beech.Exposure.SuitabilityOf(c)) differingCells++;
        Check("D_OAK_AND_BEECH_DESCRIPTORS_DIFFER", differingCells == 3, $"differing site classes={differingCells}");

        Check("D_SITKA_HAS_NO_ASSESSMENTS_AND_KEEPS_LEGACY_ROUTE",
            sitka.UsesLegacySiteRoute && !oak.UsesLegacySiteRoute && !beech.UsesLegacySiteRoute
            && sitka.Moisture.AssessedCount == 0 && sitka.Fertility.AssessedCount == 0 && sitka.Exposure.AssessedCount == 0
            && sitka.Moisture.Basis == SiteResponseBasis.Deferred && sitka.Fertility.Basis == SiteResponseBasis.Deferred
            && sitka.Exposure.Basis == SiteResponseBasis.Deferred);

        bool provenanceConsistent = SpeciesSiteResponses.All.All(r =>
            !string.IsNullOrWhiteSpace(r.Moisture.Note) && !string.IsNullOrWhiteSpace(r.Fertility.Note) && !string.IsNullOrWhiteSpace(r.Exposure.Note)
            && (r.Moisture.Basis == SiteResponseBasis.Deferred) == (r.Moisture.AssessedCount == 0)
            && (r.Fertility.Basis == SiteResponseBasis.Deferred) == (r.Fertility.AssessedCount == 0)
            && (r.Exposure.Basis == SiteResponseBasis.Deferred) == (r.Exposure.AssessedCount == 0));
        Check("D_EVIDENCE_BASIS_NEVER_CLAIMS_A_DIRECTION_THAT_IS_NOT_ENCODED", provenanceConsistent);
        Check("D_NO_DATA_USES_THE_CALIBRATION_BASIS",
            SpeciesSiteResponses.All.All(r => r.Moisture.Basis != SiteResponseBasis.Calibration && r.Fertility.Basis != SiteResponseBasis.Calibration
                && r.Exposure.Basis != SiteResponseBasis.Calibration));
    }

    // ---- E. Scenario One's canonical site -----------------------------------------------------------------------

    private static void CheckE_ScenarioOneReference(List<SiteConditionProfile> profiles)
    {
        bool found = ScenarioSiteProfiles.TryGet("scenario-one", out SiteConditionProfile site);
        Check("E_SCENARIO_ONE_SITE_IS_CONFIGURED", found && site == ScenarioSiteProfiles.ScenarioOneReference);
        Check("E_SCENARIO_ONE_DECLARES_REFERENCE_MOISTURE_FERTILITY_EXPOSURE",
            site.MoistureDrainage == MoistureDrainageClass.Reference && site.Fertility == FertilityClass.Reference
            && site.ExposureClimate == ExposureClimateClass.Reference);
        Check("E_SCENARIO_ONE_ROOTING_IS_UNKNOWN_BECAUSE_V2_LEAVES_IT_UNSPECIFIED",
            site.RootingContext == RootingContextClass.Unknown && site != SiteConditionProfile.Unspecified,
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

        var oakDry = new SiteConditionProfile(MoistureDrainageClass.DrierFreeDraining, FertilityClass.Low, ExposureClimateClass.Moderate);
        int reference = Bits(SiteGrowthResponse.Resolve("sessile-oak", oakDry));
        bool repeated = true;
        for (int i = 0; i < 1000; i++)
            if (Bits(SiteGrowthResponse.Resolve("sessile-oak", oakDry)) != reference) repeated = false;
        Check("F_1000_REPEATED_LOOKUPS_ARE_BIT_IDENTICAL", repeated);

        bool descriptors = true;
        for (int i = 0; i < 200; i++)
            if (SpeciesSiteResponses.Beech.Moisture.SuitabilityOf(MoistureDrainageClass.MoistWellAerated) != SiteSuitability.Preferred
                || !SpeciesSiteResponses.TryGet("beech", out SpeciesSiteResponse again) || again != SpeciesSiteResponses.Beech)
                descriptors = false;
        Check("F_DESCRIPTOR_LOOKUPS_ARE_STABLE", descriptors);
        Check("F_LOOKUP_DOES_NOT_MUTATE_THE_PROFILE",
            oakDry == new SiteConditionProfile(MoistureDrainageClass.DrierFreeDraining, FertilityClass.Low, ExposureClimateClass.Moderate));
    }

    private static List<int> Snapshot(List<SiteConditionProfile> profiles)
    {
        var values = new List<int>();
        foreach (string id in RegisteredSpecies.Concat(UnregisteredSubjects))
            foreach (SiteConditionProfile profile in profiles)
                values.Add(Bits(SiteGrowthResponse.Resolve(id, profile)));
        foreach (SpeciesSiteResponse response in SpeciesSiteResponses.All)
        {
            foreach (MoistureDrainageClass c in Values<MoistureDrainageClass>()) values.Add((int)response.Moisture.SuitabilityOf(c));
            foreach (FertilityClass c in Values<FertilityClass>()) values.Add((int)response.Fertility.SuitabilityOf(c));
            foreach (ExposureClimateClass c in Values<ExposureClimateClass>()) values.Add((int)response.Exposure.SuitabilityOf(c));
        }
        return values;
    }

    // ---- G. No save impact --------------------------------------------------------------------------------------

    private static void CheckG_NoSaveImpact()
    {
        string saveData = ReadSource("Assets/ForestPrototype/ForestSaveData.cs");
        Check("G_SAVE_VERSION_IS_STILL_20", Regex.IsMatch(saveData, "public\\s+const\\s+int\\s+CurrentVersion\\s*=\\s*20\\s*;"));

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

        // The scanner must be able to see all twelve names in the new files, or the "none found" results below mean nothing.
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
        Check("H_NO_OTHER_VERIFICATION_TOOL_REFERENCES_THE_NEW_TYPES", tools.Length > 20 && tools.All(f => !typeRegex.IsMatch(File.ReadAllText(f))),
            $"tool files scanned={tools.Length}");

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

    // ---- I. Invalid input is rejected, not coerced --------------------------------------------------------------

    private static void CheckI_InputValidation()
    {
        Check("I_UNDEFINED_ENUM_VALUES_ARE_REJECTED",
            Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile((MoistureDrainageClass)99, FertilityClass.Low, ExposureClimateClass.Moderate))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(MoistureDrainageClass.Reference, (FertilityClass)99, ExposureClimateClass.Moderate))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(MoistureDrainageClass.Reference, FertilityClass.Low, (ExposureClimateClass)99))
            && Throws<ArgumentOutOfRangeException>(() => new SiteConditionProfile(MoistureDrainageClass.Reference, FertilityClass.Low, ExposureClimateClass.Moderate, (RootingContextClass)99)));
        Check("I_AXIS_RESPONSE_REJECTS_UNKNOWN_AND_REFERENCE_ENTRIES",
            Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n", (FertilityClass.Unknown, SiteSuitability.Preferred)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n", (FertilityClass.Reference, SiteSuitability.Preferred))));
        Check("I_AXIS_RESPONSE_REJECTS_INCONSISTENT_BASIS",
            Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Deferred, "n", (FertilityClass.Low, SiteSuitability.Tolerated)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n")));
        Check("I_AXIS_RESPONSE_REJECTS_DUPLICATES_NOTASSESSED_ENTRIES_AND_EMPTY_NOTES",
            Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n",
                (FertilityClass.Low, SiteSuitability.Tolerated), (FertilityClass.Low, SiteSuitability.Limiting)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Ordinal, "n", (FertilityClass.Low, SiteSuitability.NotAssessed)))
            && Throws<ArgumentException>(() => new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Deferred, "")));
        Check("I_SPECIES_RESPONSE_REJECTS_MISSING_ID_OR_AXES",
            Throws<ArgumentException>(() => new SpeciesSiteResponse("", SpeciesSiteResponses.Beech.Moisture, SpeciesSiteResponses.Beech.Fertility, SpeciesSiteResponses.Beech.Exposure, false))
            && Throws<ArgumentNullException>(() => new SpeciesSiteResponse("x", null, SpeciesSiteResponses.Beech.Fertility, SpeciesSiteResponses.Beech.Exposure, false)));
    }

    // ---- helpers ------------------------------------------------------------------------------------------------

    private static string ReadSource(string relativePath) => File.ReadAllText(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static IEnumerable<string> NewProductionFiles()
    {
        string folder = Path.Combine(root, "Assets", "ForestPrototype");
        return new[] { "ForestSiteProfile.cs", "SpeciesSiteResponse.cs", "ScenarioSiteProfiles.cs" }.Select(name => Path.Combine(folder, name));
    }

    private static Regex TypeNameRegex() => new Regex("\\b(" + string.Join("|", NewTypeNames) + ")\\b");

    // Source with line comments and string literals removed, so prose in comments and notes is not mistaken for code.
    private static string CodeOnly(string source)
    {
        string withoutComments = Regex.Replace(source, "//[^\\r\\n]*", "");
        return Regex.Replace(withoutComments, "\"(\\\\.|[^\"\\\\\\r\\n])*\"", "\"\"");
    }
}

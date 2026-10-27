// Offline verification of the Scenario Two species site rules (SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-V1).
//
// Lives in this folder because the site-infrastructure dormancy checks require that no other verification tool names the site
// types. Compiled on the plain .NET SDK with ONLY ForestSiteProfile.cs and SpeciesSiteResponse.cs (no UnityEngine).
//
// Encodes nothing new: it checks that only uncontested qualitative directions exist for Scots pine, downy birch and common alder,
// that the alder waterlogging contradiction (C-13) is left on HOLD with no direction on that axis, that every numerical effect is
// still exactly 1.0, that the Scenario One catalog is still exactly Sitka / Oak / Beech, and that nothing in the runtime reads the
// Scenario Two rows.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class ScenarioTwoSiteRulesOfflineCheck
{
    private static int checks;
    private static int failures;
    private static string root;
    private static string fp;

    private static void Check(string name, bool ok, string detail = "")
    {
        checks++;
        if (!ok) failures++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? "  [" + detail + "]" : ""));
    }

    private static string AssetSpeciesId(string name)
    {
        string path = Path.Combine(fp, "Species", "ScenarioTwo", name + ".asset");
        Match m = Regex.Match(File.ReadAllText(path), @"^  speciesId: (\S+)\s*$", RegexOptions.Multiline);
        return m.Success ? m.Groups[1].Value : "";
    }

    public static int Main(string[] args)
    {
        if (args.Length < 1 || !Directory.Exists(Path.Combine(args[0], "Assets", "ForestPrototype")))
        {
            Console.WriteLine("usage: check <repository root>");
            return 2;
        }
        root = args[0];
        fp = Path.Combine(root, "Assets", "ForestPrototype");
        CheckSiteRules();
        Console.WriteLine(failures == 0
            ? "SCENARIO_TWO_SITE_RULES_OFFLINE_PASS checks=" + checks
            : "SCENARIO_TWO_SITE_RULES_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    private static void CheckSiteRules()
    {
        IReadOnlyList<SpeciesSiteResponse> all = ScenarioTwoSpeciesSiteResponses.All;
        Check("E_CATALOG_HOLDS_PINE_BIRCH_ALDER_IN_FIXED_ORDER",
            all.Count == 3 && all[0] == ScenarioTwoSpeciesSiteResponses.ScotsPine && all[1] == ScenarioTwoSpeciesSiteResponses.DownyBirch
            && all[2] == ScenarioTwoSpeciesSiteResponses.CommonAlder
            && all[0].SpeciesId == "scots-pine" && all[1].SpeciesId == "downy-birch" && all[2].SpeciesId == "common-alder");
        Check("E_SCENARIO_ONE_CATALOG_IS_STILL_EXACTLY_SITKA_OAK_BEECH",
            SpeciesSiteResponses.All.Count == 3 && !SpeciesSiteResponses.TryGet("scots-pine", out _) && !SpeciesSiteResponses.TryGet("common-alder", out _)
            && SiteGrowthResponse.Resolve("scots-pine", default) == 1f);
        Check("E_IDS_MATCH_THE_NEW_ASSETS",
            all[0].SpeciesId == AssetSpeciesId("ScotsPine") && all[1].SpeciesId == AssetSpeciesId("DownyBirch") && all[2].SpeciesId == AssetSpeciesId("CommonAlder"));
        Check("E_EVERY_RESPONSE_IS_NUMERICALLY_INACTIVE_AND_NEUTRAL",
            all.All(r => !r.NumericallyActive && r.GrowthMultiplier(default) == 1f));

        SpeciesSiteResponse alder = ScenarioTwoSpeciesSiteResponses.CommonAlder;
        Check("E_ALDER_WATERLOGGING_IS_HOLD_NO_DRAINAGE_CLASS_IS_ASSESSED",
            alder.DrainageAeration.AssessedCount == 0 && alder.DrainageAeration.Basis == SiteResponseBasis.Deferred
            && new[] { DrainageAerationClass.WellAerated, DrainageAerationClass.Impeded, DrainageAerationClass.WaterloggedRisk }
                .All(c => alder.DrainageAeration.SuitabilityOf(c) == SiteSuitability.NotAssessed)
            && alder.DrainageAeration.Note.Contains("HOLD") && alder.DrainageAeration.Note.Contains("C-13"));
        Check("E_ALDER_MOISTURE_IS_THE_UNCONTESTED_WET_PREFERRED_DRY_LIMITING",
            alder.Moisture.SuitabilityOf(MoistureClass.Wetter) == SiteSuitability.Preferred
            && alder.Moisture.SuitabilityOf(MoistureClass.Moist) == SiteSuitability.Tolerated
            && alder.Moisture.SuitabilityOf(MoistureClass.Drier) == SiteSuitability.Limiting);
        Check("E_ALDER_EXPOSURE_AND_FERTILITY_LOW_ARE_NOT_ASSERTED",
            alder.Exposure.AssessedCount == 0 && alder.Fertility.SuitabilityOf(FertilityClass.Low) == SiteSuitability.NotAssessed
            && alder.Fertility.SuitabilityOf(FertilityClass.Moderate) == SiteSuitability.Preferred);
        SpeciesSiteResponse birch = ScenarioTwoSpeciesSiteResponses.DownyBirch;
        Check("E_BIRCH_WATERLOGGING_IS_TOLERATED_NOT_PREFERRED_AND_WETTER_IS_NOT_A_BONUS",
            birch.DrainageAeration.SuitabilityOf(DrainageAerationClass.WaterloggedRisk) == SiteSuitability.Tolerated
            && birch.Moisture.SuitabilityOf(MoistureClass.Wetter) == SiteSuitability.Tolerated
            && birch.Moisture.SuitabilityOf(MoistureClass.Moist) == SiteSuitability.Preferred
            && birch.Moisture.SuitabilityOf(MoistureClass.Drier) == SiteSuitability.NotAssessed);
        SpeciesSiteResponse pine = ScenarioTwoSpeciesSiteResponses.ScotsPine;
        Check("E_PINE_WATERLOGGING_LIMITING_FERTILITY_LOW_TOLERATED_MOISTURE_NOT_ASSERTED",
            pine.DrainageAeration.SuitabilityOf(DrainageAerationClass.WaterloggedRisk) == SiteSuitability.Limiting
            && pine.Fertility.SuitabilityOf(FertilityClass.Low) == SiteSuitability.Tolerated
            && pine.Moisture.AssessedCount == 0 && pine.Exposure.AssessedCount == 0
            && pine.DrainageAeration.SuitabilityOf(DrainageAerationClass.Impeded) == SiteSuitability.NotAssessed);
        Check("E_WETTER_IS_NEVER_LIMITING_BECAUSE_WATERLOGGING_IS",
            all.All(r => r.Moisture.SuitabilityOf(MoistureClass.Wetter) != SiteSuitability.Limiting));
        Check("E_NO_RESPONSE_ASSESSES_UNKNOWN_OR_REFERENCE_AND_BASIS_MATCHES_DIRECTIONS",
            all.All(r => (r.Moisture.Basis == SiteResponseBasis.Deferred) == (r.Moisture.AssessedCount == 0)
                && (r.DrainageAeration.Basis == SiteResponseBasis.Deferred) == (r.DrainageAeration.AssessedCount == 0)
                && (r.Fertility.Basis == SiteResponseBasis.Deferred) == (r.Fertility.AssessedCount == 0)
                && (r.Exposure.Basis == SiteResponseBasis.Deferred) == (r.Exposure.AssessedCount == 0)
                && r.Moisture.SuitabilityOf(MoistureClass.Unknown) == SiteSuitability.NotAssessed && r.Moisture.SuitabilityOf(MoistureClass.Reference) == SiteSuitability.NotAssessed));
        Check("E_NOTHING_IN_THE_RUNTIME_READS_THE_NEW_CATALOG",
            Directory.GetFiles(fp, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.EndsWith("SpeciesSiteResponse.cs") && !p.Contains(Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar))
                .All(p => !File.ReadAllText(p).Contains("ScenarioTwoSpeciesSiteResponses")));
    }
}

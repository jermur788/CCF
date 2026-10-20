// Offline verification for the Scenario Two species-model foundation (SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-V1).
//
// Compiled on the plain .NET SDK with ONLY these production files (none uses UnityEngine, Sitka growth or save code):
//   SpeciesConfigurationPolicy.cs, MixedDensityModel.cs, MixedDensityModel.Composite.cs
// plus this file. The site-rule checks are in Tools/Verification/SiteInfrastructure/ (where the dormancy checks expect any
// code that touches the site types). It reads repository text (the species assets and a few C# sources) to check what Unity would load,
// but it NEVER proves Unity import/compile, a ScriptableObject load, or any runtime behaviour: see
// Tools/Verification/ScenarioTwoSpeciesFoundation/UnityLogic/ for the checks that need the Editor (not run yet).
//
// Test fixtures below use made-up numbers purely to exercise the rules. They are NOT species values.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class ScenarioTwoSpeciesFoundationOfflineCheck
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

    public static int Main(string[] args)
    {
        if (args.Length < 1 || !Directory.Exists(Path.Combine(args[0], "Assets", "ForestPrototype")))
        {
            Console.WriteLine("usage: check <repository root>");
            return 2;
        }
        root = args[0];
        fp = Path.Combine(root, "Assets", "ForestPrototype");

        CheckA_SafeDefaults();
        CheckB_DensityClass();
        CheckC_AgePhases();
        CheckD_Dispersal();
        CheckF_SpeciesAssets();
        CheckG_SourceWiring();

        Console.WriteLine(failures == 0
            ? "SCENARIO_TWO_SPECIES_FOUNDATION_OFFLINE_PASS checks=" + checks
            : "SCENARIO_TWO_SPECIES_FOUNDATION_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    // ---------------------------------------------------------------- helpers

    private static bool Has(SpeciesConfigurationReport report, string fragment) =>
        report.Errors.Any(e => e.IndexOf(fragment, StringComparison.Ordinal) >= 0);

    // A fully declared fixture species with made-up (non-species) numbers. Evaluate must call it ready.
    private static SpeciesConfigurationFacts ReadyFacts(string id) => new SpeciesConfigurationFacts
    {
        SpeciesId = id,
        GrowingSpaceDemand = GrowingSpaceDemandClass.Intermediate,
        GrowthState = SpeciesParameterState.Placeholder,
        PotentialDbhGrowthCmPerYear = 0.5f, PotentialHeightGrowthMPerYear = 0.5f, MaxDbhCm = 60f, MaxHeightM = 25f,
        CrownRadiusIntercept = 0.5f, CrownRadiusPerCmDbh = 0.1f,
        BrowseState = SpeciesParameterState.Set, BrowsePalatability = 0.5f,
        JuvenileLightState = SpeciesParameterState.Placeholder, UsesDistinctJuvenileLightResponses = true,
        SeedDispersal = NaturalSeedDispersal.Wind, SeedKernelState = SpeciesParameterState.Placeholder,
        SeedDispersalScaleM = 19f, SeedDispersalCutoffM = 150f,
        MaturityOnsetYears = 15f, MaturityFullYears = 15f,
        MastGoodProbability = 0f, MastPoorProbability = 0f, MastGoodMultiplier = 1f, MastNormalMultiplier = 1f, MastPoorMultiplier = 1f,
        SupportsRegeneration = true,
    };

    private static Dictionary<string, string> ReadAsset(string name)
    {
        var values = new Dictionary<string, string>();
        foreach (string raw in File.ReadAllLines(AssetPath(name)))
        {
            Match m = Regex.Match(raw, @"^  ([A-Za-z0-9_]+): (.+)$");
            if (m.Success) values[m.Groups[1].Value] = m.Groups[2].Value.Trim();
        }
        return values;
    }

    // The three Scenario Two assets live in Species/ScenarioTwo/ (kept out of the folder the Scenario One site check scans).
    private static string AssetPath(string name)
    {
        string two = Path.Combine(fp, "Species", "ScenarioTwo", name + ".asset");
        return File.Exists(two) ? two : Path.Combine(fp, "Species", name + ".asset");
    }

    private static float F(Dictionary<string, string> a, string key) => float.Parse(a[key], CultureInfo.InvariantCulture);
    private static int I(Dictionary<string, string> a, string key) => int.Parse(a[key], CultureInfo.InvariantCulture);

    private static SpeciesConfigurationFacts FactsFromAsset(Dictionary<string, string> a)
    {
        return new SpeciesConfigurationFacts
        {
            SpeciesId = a["speciesId"],
            GrowingSpaceDemand = (GrowingSpaceDemandClass)I(a, "growingSpaceDemand"),
            GrowthState = (SpeciesParameterState)I(a, "growthState"),
            BrowseState = (SpeciesParameterState)I(a, "browseState"),
            JuvenileLightState = (SpeciesParameterState)I(a, "juvenileLightState"),
            SeedKernelState = (SpeciesParameterState)I(a, "seedKernelState"),
            SeedDispersal = (NaturalSeedDispersal)I(a, "naturalSeedDispersal"),
            SupportsRegeneration = I(a, "supportsRegeneration") == 1,
            UsesDistinctJuvenileLightResponses = I(a, "useDistinctJuvenileLightResponses") == 1,
            BrowsePalatability = F(a, "browsePalatability"),
            PotentialDbhGrowthCmPerYear = F(a, "potentialDbhGrowthCmPerYear"),
            PotentialHeightGrowthMPerYear = F(a, "potentialHeightGrowthMPerYear"),
            MaxDbhCm = F(a, "maxDbhCm"), MaxHeightM = F(a, "maxHeightM"),
            CrownRadiusIntercept = F(a, "crownRadiusIntercept"), CrownRadiusPerCmDbh = F(a, "crownRadiusPerCmDbh"),
            UsesPowerLawCrown = I(a, "usePowerLawCrown") == 1,
            MaturityOnsetYears = F(a, "maturityOnsetYears"), MaturityFullYears = F(a, "maturityFullYears"),
            MastGoodProbability = F(a, "mastGoodProbability"), MastPoorProbability = F(a, "mastPoorProbability"),
            MastGoodMultiplier = F(a, "mastGoodMultiplier"), MastNormalMultiplier = F(a, "mastNormalMultiplier"),
            MastPoorMultiplier = F(a, "mastPoorMultiplier"),
            SeedDispersalScaleM = F(a, "seedDispersalScaleM"), SeedDispersalCutoffM = F(a, "seedDispersalCutoffM"),
            AgePhases = new GrowthAgePhase[0],
        };
    }

    // ---------------------------------------------------------------- A: safe defaults

    private static void CheckA_SafeDefaults()
    {
        var minimal = new SpeciesConfigurationFacts { SpeciesId = "test-new-species" };
        SpeciesConfigurationReport r = SpeciesConfigurationPolicy.Evaluate(minimal);
        Check("A_MINIMAL_NEW_SPECIES_IS_NOT_READY", !r.IsReady && r.HasErrors, r.Errors.Count + " errors");
        Check("A_MINIMAL_NEW_SPECIES_FLAGS_REFERENCE_GROWTH_BROWSE_LIGHT_AND_DISPERSAL",
            Has(r, "Reference") && Has(r, "growthState") && Has(r, "browseState") && Has(r, "juvenileLightState") && Has(r, "naturalSeedDispersal"));
        Check("A_MINIMAL_NEW_SPECIES_DEFAULT_DEMAND_IS_REFERENCE_AND_IS_REJECTED",
            minimal.GrowingSpaceDemand == GrowingSpaceDemandClass.Reference && Has(r, "reserved for sitka-spruce"));
        Check("A_MINIMAL_NEW_SPECIES_IS_NOT_BROWSE_PROOF_BY_DEFAULT",
            minimal.BrowsePalatability == 0f && minimal.BrowseState == SpeciesParameterState.Unspecified && Has(r, "browse-proof"));
        Check("A_MINIMAL_NEW_SPECIES_DOES_NOT_DISPERSE_BY_DEFAULT",
            !SpeciesConfigurationPolicy.DispersesSeedNaturally(minimal.SpeciesId, minimal.SeedDispersal, minimal.SupportsRegeneration, minimal.SeedKernelState)
            && minimal.SeedDispersal == NaturalSeedDispersal.Unspecified && minimal.SupportsRegeneration);
        Check("A_NO_DEFAULT_GROWTH_IS_ACCEPTED_FOR_A_NEW_SPECIES", minimal.GrowthState == SpeciesParameterState.Unspecified && Has(r, "growthState"));

        // The declared-defaults mirror: the facts defaults and the policy constants equal the TreeSpeciesDefinition initializers.
        string source = File.ReadAllText(Path.Combine(fp, "TreeSpeciesDefinition.cs"));
        Func<string, string> init = name =>
        {
            Match m = Regex.Match(source, @"private\s+[A-Za-z\[\]]+\s+" + name + @"\s*=\s*([^;]+);");
            return m.Success ? m.Groups[1].Value.Trim() : null;
        };
        Func<string, float> num = name => float.Parse(init(name).TrimEnd('f'), CultureInfo.InvariantCulture);
        Check("A_POLICY_CONSTANTS_MIRROR_THE_SOURCE_DEFAULTS",
            num("crownRadiusIntercept") == SpeciesConfigurationPolicy.DefaultCrownRadiusIntercept
            && num("crownRadiusPerCmDbh") == SpeciesConfigurationPolicy.DefaultCrownRadiusPerCmDbh
            && num("maturityOnsetYears") == SpeciesConfigurationPolicy.DefaultMaturityOnsetYears
            && num("maturityFullYears") == SpeciesConfigurationPolicy.DefaultMaturityFullYears
            && num("mastGoodProbability") == SpeciesConfigurationPolicy.DefaultMastGoodProbability
            && num("mastPoorProbability") == SpeciesConfigurationPolicy.DefaultMastPoorProbability
            && num("mastGoodMultiplier") == SpeciesConfigurationPolicy.DefaultMastGoodMultiplier
            && num("mastNormalMultiplier") == SpeciesConfigurationPolicy.DefaultMastNormalMultiplier
            && num("mastPoorMultiplier") == SpeciesConfigurationPolicy.DefaultMastPoorMultiplier);
        var defaults = new SpeciesConfigurationFacts();
        Check("A_FACTS_DEFAULTS_MIRROR_THE_SOURCE_DEFAULTS",
            num("potentialDbhGrowthCmPerYear") == defaults.PotentialDbhGrowthCmPerYear
            && num("potentialHeightGrowthMPerYear") == defaults.PotentialHeightGrowthMPerYear
            && num("maxDbhCm") == defaults.MaxDbhCm && num("maxHeightM") == defaults.MaxHeightM
            && num("seedDispersalScaleM") == defaults.SeedDispersalScaleM && num("seedDispersalCutoffM") == defaults.SeedDispersalCutoffM
            && init("supportsRegeneration") == "true" && defaults.SupportsRegeneration
            && init("growingSpaceDemand") == "GrowingSpaceDemandClass.Reference" && defaults.GrowingSpaceDemand == GrowingSpaceDemandClass.Reference);
        Check("A_THE_SOURCE_BROWSE_DEFAULT_IS_STILL_ZERO_SO_THE_GUARD_IS_NEEDED", init("browsePalatability") == null || init("browsePalatability") == "0f");

        // Why the Reference default must never be changed: the Sitka asset does not serialize growingSpaceDemand at all.
        Dictionary<string, string> sitka = ReadAsset("SitkaSpruce");
        Check("A_SITKA_ASSET_RELIES_ON_THE_REFERENCE_DEFAULT_SO_THE_DEFAULT_IS_UNCHANGED",
            !sitka.ContainsKey("growingSpaceDemand") && init("growingSpaceDemand") == "GrowingSpaceDemandClass.Reference");

        // Fully declared fixture is ready; each guard fails it.
        SpeciesConfigurationFacts ok = ReadyFacts("fixture");
        Check("A_FULLY_DECLARED_FIXTURE_IS_READY", SpeciesConfigurationPolicy.Evaluate(ok).IsReady);
        Action<string, Action<SpeciesConfigurationFacts>, string> broken = (name, mutate, fragment) =>
        {
            SpeciesConfigurationFacts f = ReadyFacts("fixture");
            mutate(f);
            SpeciesConfigurationReport rep = SpeciesConfigurationPolicy.Evaluate(f);
            Check(name, !rep.IsReady && Has(rep, fragment), string.Join(" | ", rep.Errors));
        };
        broken("A_GUARD_GROWTH_SET_WITH_SENTINEL_NUMBERS", f => { f.PotentialDbhGrowthCmPerYear = 0f; }, "inert HOLD values");
        broken("A_GUARD_GROWTH_SET_WITH_THE_SITKA_CROWN_DEFAULT", f => { f.CrownRadiusIntercept = SpeciesConfigurationPolicy.DefaultCrownRadiusIntercept; f.CrownRadiusPerCmDbh = SpeciesConfigurationPolicy.DefaultCrownRadiusPerCmDbh; }, "Sitka default");
        broken("A_GUARD_BROWSE_PLACEHOLDER_WITH_ZERO_PALATABILITY", f => { f.BrowseState = SpeciesParameterState.Placeholder; f.BrowsePalatability = 0f; }, "browse-proof");
        broken("A_GUARD_JUVENILE_LIGHT_SET_WITHOUT_DISTINCT_RESPONSES", f => { f.UsesDistinctJuvenileLightResponses = false; }, "legacy Sitka-calibrated");
        broken("A_GUARD_KERNEL_SET_WITH_SENTINEL_NUMBERS", f => { f.SeedDispersalScaleM = 1f; f.SeedDispersalCutoffM = 1f; }, "inert HOLD values");
        broken("A_GUARD_KERNEL_SET_WITH_THE_SITKA_MATURITY_RAMP", f => { f.MaturityOnsetYears = 20f; f.MaturityFullYears = 30f; }, "maturity onset");
        broken("A_GUARD_KERNEL_SET_WITH_THE_SITKA_MAST_REGIME", f => { f.MastGoodProbability = 0.15f; f.MastPoorProbability = 0.35f; f.MastGoodMultiplier = 1.5f; f.MastNormalMultiplier = 1f; f.MastPoorMultiplier = 0.4f; }, "mast regime");
        broken("A_GUARD_DISPERSAL_WITHOUT_THE_JUVENILE_LAYER", f => { f.SupportsRegeneration = false; }, "supportsRegeneration");

        // Browse-proof can be declared, deliberately.
        SpeciesConfigurationFacts proof = ReadyFacts("fixture");
        proof.BrowseState = SpeciesParameterState.Set; proof.BrowsePalatability = 0f;
        Check("A_BROWSE_PROOF_IS_ALLOWED_ONLY_AS_AN_EXPLICIT_SET", SpeciesConfigurationPolicy.Evaluate(proof).IsReady);
        // Hold is explicit but not ready.
        SpeciesConfigurationFacts held = ReadyFacts("fixture");
        held.GrowthState = SpeciesParameterState.Hold; held.BrowseState = SpeciesParameterState.Hold;
        held.JuvenileLightState = SpeciesParameterState.Hold; held.SeedKernelState = SpeciesParameterState.Hold;
        SpeciesConfigurationReport heldReport = SpeciesConfigurationPolicy.Evaluate(held);
        Check("A_HOLD_IS_VALID_BUT_NOT_READY", !heldReport.HasErrors && !heldReport.IsReady && heldReport.HoldAreas.Count == 4,
            string.Join(", ", heldReport.HoldAreas));

        // The established three are untouched: no declaration is asked of them and their behaviour is unchanged.
        var sitkaFacts = new SpeciesConfigurationFacts { SpeciesId = "sitka-spruce" };
        var oakFacts = new SpeciesConfigurationFacts { SpeciesId = "sessile-oak", GrowingSpaceDemand = GrowingSpaceDemandClass.High };
        var beechFacts = new SpeciesConfigurationFacts { SpeciesId = "beech", GrowingSpaceDemand = GrowingSpaceDemandClass.Intermediate };
        Check("A_ESTABLISHED_SITKA_OAK_BEECH_NEED_NO_DECLARATION",
            SpeciesConfigurationPolicy.Evaluate(sitkaFacts).IsReady && SpeciesConfigurationPolicy.Evaluate(oakFacts).IsReady
            && SpeciesConfigurationPolicy.Evaluate(beechFacts).IsReady);
        Check("A_ESTABLISHED_QUICK_PATH_AGREES_WITH_THE_FULL_EVALUATION",
            SpeciesConfigurationPolicy.IsEstablishedAndValid("sitka-spruce", GrowingSpaceDemandClass.Reference)
            && SpeciesConfigurationPolicy.IsEstablishedAndValid("sessile-oak", GrowingSpaceDemandClass.High)
            && SpeciesConfigurationPolicy.IsEstablishedAndValid("beech", GrowingSpaceDemandClass.Intermediate)
            && !SpeciesConfigurationPolicy.IsEstablishedAndValid("beech", GrowingSpaceDemandClass.Reference)
            && !SpeciesConfigurationPolicy.IsEstablishedAndValid("scots-pine", GrowingSpaceDemandClass.Intermediate));
        Check("A_REFERENCE_IS_REJECTED_EVEN_FOR_AN_ESTABLISHED_NON_SITKA_SPECIES",
            Has(SpeciesConfigurationPolicy.Evaluate(new SpeciesConfigurationFacts { SpeciesId = "beech" }), "reserved for sitka-spruce")
            && Has(SpeciesConfigurationPolicy.Evaluate(new SpeciesConfigurationFacts { SpeciesId = "sessile-oak" }), "reserved for sitka-spruce"));
        Check("A_ONLY_SITKA_MAY_HOLD_REFERENCE",
            SpeciesConfigurationPolicy.IsReferenceDemandAllowed("sitka-spruce") && !SpeciesConfigurationPolicy.IsReferenceDemandAllowed("scots-pine")
            && !SpeciesConfigurationPolicy.IsReferenceDemandAllowed("downy-birch") && !SpeciesConfigurationPolicy.IsReferenceDemandAllowed("common-alder")
            && !SpeciesConfigurationPolicy.IsReferenceDemandAllowed("Sitka-Spruce") && !SpeciesConfigurationPolicy.IsReferenceDemandAllowed(""));
        Check("A_ESTABLISHED_IDS_MATCH_THE_SCENARIO_ONE_ASSETS",
            ReadAsset("SitkaSpruce")["speciesId"] == SpeciesConfigurationPolicy.SitkaSpruceId
            && ReadAsset("SessileOak")["speciesId"] == SpeciesConfigurationPolicy.SessileOakId
            && ReadAsset("BeechSpecies")["speciesId"] == SpeciesConfigurationPolicy.BeechId);
    }

    // ---------------------------------------------------------------- B: density class

    private static void CheckB_DensityClass()
    {
        string[] names = { "ScotsPine", "DownyBirch", "CommonAlder" };
        bool allIntermediate = names.All(n => ReadAsset(n).TryGetValue("growingSpaceDemand", out string v) && v == "1");
        Check("B_PINE_BIRCH_ALDER_ASSETS_SERIALIZE_INTERMEDIATE_EXPLICITLY", allIntermediate);
        Check("B_INTERMEDIATE_IS_THE_ORDINAL_VALUE_1_AND_REFERENCE_IS_0",
            (int)GrowingSpaceDemandClass.Intermediate == 1 && (int)GrowingSpaceDemandClass.Reference == 0);
        Check("B_NO_NEW_ASSET_IS_REFERENCE_AND_ALL_PASS_THE_REFERENCE_RULE",
            names.All(n => !SpeciesConfigurationPolicy.Evaluate(FactsFromAsset(ReadAsset(n))).Errors.Any(e => e.Contains("reserved for"))));
        double reference = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", GrowingSpaceDemandClass.Reference, 30f, 0.0));
        double intermediate = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", GrowingSpaceDemandClass.Intermediate, 30f, 0.0));
        Check("B_INTERMEDIATE_TAKES_MORE_SPACE_THAN_REFERENCE_VIA_THE_EXISTING_ORDINAL_WEIGHT", intermediate > reference && Math.Abs(intermediate / reference - 1.5) < 1e-9,
            "ratio " + (intermediate / reference).ToString("0.###", CultureInfo.InvariantCulture) + " ([ORD/C] weight, not evidence)");
        string tooltip = File.ReadAllText(Path.Combine(fp, "TreeSpeciesDefinition.cs"));
        Check("B_THE_FIELD_DOCUMENTATION_SAYS_ORDINAL_PLACEHOLDER_AND_REFERENCE_IS_SITKA_ONLY",
            tooltip.Contains("ORDINAL PLACEHOLDER") && tooltip.Contains("not a shared measured maximum-density coefficient") && tooltip.Contains("Reference is reserved for sitka-spruce"));
        Check("B_DENSITY_MODEL_ITSELF_IS_NOT_EDITED",
            !File.ReadAllText(Path.Combine(fp, "MixedDensityModel.cs")).Contains("scots-pine"));
    }

    // ---------------------------------------------------------------- C: age-dependent growth

    private static void CheckC_AgePhases()
    {
        GrowthAgePhase[] none = new GrowthAgePhase[0];
        bool noneNeutral = true;
        foreach (int age in new[] { 0, 1, 10, 60, 300 })
        {
            GrowthAgePhases.FactorsAt(none, age, out float d, out float h);
            noneNeutral &= BitConverter.SingleToInt32Bits(d) == BitConverter.SingleToInt32Bits(1f) && BitConverter.SingleToInt32Bits(h) == BitConverter.SingleToInt32Bits(1f);
        }
        GrowthAgePhases.FactorsAt(null, 5, out float nd, out float nh);
        Check("C_NO_PHASES_GIVES_EXACTLY_1_AT_EVERY_AGE_AND_NULL_IS_THE_SAME", noneNeutral && nd == 1f && nh == 1f && !GrowthAgePhases.HasPhases(none) && !GrowthAgePhases.HasPhases(null));

        // Fixture phases (made-up numbers): juvenile faster, then moderated.
        var two = new[] { new GrowthAgePhase(0f, 1.0f, 1.0f), new GrowthAgePhase(20f, 0.5f, 0.25f) };
        GrowthAgePhases.FactorsAt(two, 0, out float a0, out float h0);
        GrowthAgePhases.FactorsAt(two, 19, out float a19, out float h19);
        GrowthAgePhases.FactorsAt(two, 20, out float a20, out float h20);
        GrowthAgePhases.FactorsAt(two, 90, out float a90, out float h90);
        Check("C_TWO_PHASES_GIVE_DIFFERENT_FACTORS_BEFORE_AND_AFTER_THE_BOUNDARY",
            a0 == 1f && h0 == 1f && a19 == 1f && h19 == 1f && a20 == 0.5f && h20 == 0.25f && a90 == 0.5f && h90 == 0.25f);
        var three = new[] { new GrowthAgePhase(0f, 1f, 1f), new GrowthAgePhase(10f, 0.8f, 0.9f), new GrowthAgePhase(40f, 0.2f, 0.1f) };
        GrowthAgePhases.FactorsAt(three, 9, out float t9, out _);
        GrowthAgePhases.FactorsAt(three, 10, out float t10, out _);
        GrowthAgePhases.FactorsAt(three, 39, out float t39, out _);
        GrowthAgePhases.FactorsAt(three, 40, out float t40, out _);
        Check("C_THREE_PHASES_SELECT_THE_LAST_PHASE_STARTED", t9 == 1f && t10 == 0.8f && t39 == 0.8f && t40 == 0.2f);
        // A phased increment differs from the unphased one only when phases exist: the same potential x factor.
        float potential = 0.7f * 1.1f * 0.9f;
        float unphased = potential;
        GrowthAgePhases.FactorsAt(two, 30, out float f30, out _);
        Check("C_PHASED_INCREMENT_DIFFERS_FROM_UNPHASED_ONLY_WITH_PHASES", potential * f30 != unphased && potential * 1f == unphased);

        string problem;
        Check("C_VALID_PHASES_PASS", GrowthAgePhases.TryValidate(two, out problem) && GrowthAgePhases.TryValidate(none, out problem) && GrowthAgePhases.TryValidate(null, out problem));
        Check("C_FIRST_PHASE_MUST_START_AT_ZERO", !GrowthAgePhases.TryValidate(new[] { new GrowthAgePhase(5f, 1f, 1f) }, out problem));
        Check("C_START_AGES_MUST_STRICTLY_INCREASE",
            !GrowthAgePhases.TryValidate(new[] { new GrowthAgePhase(0f, 1f, 1f), new GrowthAgePhase(0f, 1f, 1f) }, out problem)
            && !GrowthAgePhases.TryValidate(new[] { new GrowthAgePhase(0f, 1f, 1f), new GrowthAgePhase(20f, 1f, 1f), new GrowthAgePhase(10f, 1f, 1f) }, out problem));
        Check("C_FACTORS_MUST_BE_FINITE_AND_NOT_NEGATIVE",
            !GrowthAgePhases.TryValidate(new[] { new GrowthAgePhase(0f, -0.1f, 1f) }, out problem)
            && !GrowthAgePhases.TryValidate(new[] { new GrowthAgePhase(0f, 1f, float.NaN) }, out problem)
            && !GrowthAgePhases.TryValidate(new[] { new GrowthAgePhase(0f, float.PositiveInfinity, 1f) }, out problem));
        SpeciesConfigurationFacts bad = ReadyFacts("fixture");
        bad.AgePhases = new[] { new GrowthAgePhase(3f, 1f, 1f) };
        Check("C_INVALID_PHASES_MAKE_A_SPECIES_INVALID_INCLUDING_AN_ESTABLISHED_ONE",
            Has(SpeciesConfigurationPolicy.Evaluate(bad), "growthAgePhases")
            && Has(SpeciesConfigurationPolicy.Evaluate(new SpeciesConfigurationFacts { SpeciesId = "sitka-spruce", AgePhases = bad.AgePhases }), "growthAgePhases"));
        Check("C_NO_PRODUCTION_ASSET_SUPPLIES_A_PHASE_TABLE_SO_NO_COEFFICIENT_IS_INVENTED",
            new[] { "ScotsPine", "DownyBirch", "CommonAlder", "SitkaSpruce", "SessileOak", "BeechSpecies" }
                .All(n => !File.ReadAllText(AssetPath(n)).Contains("startAgeYears")));
        Check("C_THE_THREE_ESTABLISHED_ASSETS_DO_NOT_MENTION_PHASES",
            new[] { "SitkaSpruce", "SessileOak", "BeechSpecies" }.All(n => !File.ReadAllText(AssetPath(n)).Contains("growthAgePhases")));
    }

    // ---------------------------------------------------------------- D: dispersal

    private static void CheckD_Dispersal()
    {
        Func<string, NaturalSeedDispersal, bool, SpeciesParameterState, bool> disperses = SpeciesConfigurationPolicy.DispersesSeedNaturally;
        Check("D_PLANTING_ONLY_NEVER_DISPERSES_EVEN_WITH_A_KERNEL_AND_THE_JUVENILE_LAYER",
            !disperses("scots-pine", NaturalSeedDispersal.None, true, SpeciesParameterState.Set)
            && !disperses("scots-pine", NaturalSeedDispersal.None, true, SpeciesParameterState.Hold));
        Check("D_WIND_NEEDS_A_SET_OR_PLACEHOLDER_KERNEL",
            disperses("downy-birch", NaturalSeedDispersal.Wind, true, SpeciesParameterState.Placeholder)
            && disperses("downy-birch", NaturalSeedDispersal.Wind, true, SpeciesParameterState.Set)
            && !disperses("downy-birch", NaturalSeedDispersal.Wind, true, SpeciesParameterState.Hold)
            && !disperses("downy-birch", NaturalSeedDispersal.Wind, true, SpeciesParameterState.Unspecified)
            && !disperses("downy-birch", NaturalSeedDispersal.Wind, false, SpeciesParameterState.Placeholder));
        Check("D_WIND_AND_WATER_USES_THE_WIND_KERNEL_ONLY_AND_NEEDS_IT",
            disperses("common-alder", NaturalSeedDispersal.WindAndWater, true, SpeciesParameterState.Placeholder)
            && !disperses("common-alder", NaturalSeedDispersal.WindAndWater, true, SpeciesParameterState.Hold));
        Check("D_UNDECLARED_NEW_SPECIES_NEVER_DISPERSE", !disperses("anything", NaturalSeedDispersal.Unspecified, true, SpeciesParameterState.Placeholder));
        Check("D_ESTABLISHED_SPECIES_KEEP_THEIR_EXISTING_WIND_BEHAVIOUR_UNCHANGED",
            disperses("sitka-spruce", NaturalSeedDispersal.Unspecified, true, SpeciesParameterState.Unspecified)
            && disperses("sessile-oak", NaturalSeedDispersal.Unspecified, true, SpeciesParameterState.Unspecified)
            && disperses("beech", NaturalSeedDispersal.Unspecified, true, SpeciesParameterState.Unspecified)
            && !disperses("beech", NaturalSeedDispersal.Unspecified, false, SpeciesParameterState.Unspecified));
        Check("D_WATER_PROPAGATION_IS_NOT_IMPLEMENTED_AND_SAYS_SO", !SpeciesConfigurationPolicy.WaterDispersalPropagationImplemented);

        Dictionary<string, string> pine = ReadAsset("ScotsPine"), birch = ReadAsset("DownyBirch"), alder = ReadAsset("CommonAlder");
        Check("D_PINE_ASSET_IS_PLANTING_ONLY_WITH_THE_JUVENILE_LAYER_ON",
            I(pine, "naturalSeedDispersal") == (int)NaturalSeedDispersal.None && I(pine, "supportsRegeneration") == 1
            && !disperses("scots-pine", (NaturalSeedDispersal)I(pine, "naturalSeedDispersal"), true, (SpeciesParameterState)I(pine, "seedKernelState")));
        Check("D_PINE_CARRIES_NO_INFREQUENT_CONE_CROP_RULE",
            !File.ReadAllText(AssetPath("ScotsPine")).Contains("infrequent") && !File.ReadAllText(Path.Combine(fp, "SpeciesConfigurationPolicy.cs")).Contains("infrequent"));

        float scale = F(birch, "seedDispersalScaleM"), cutoff = F(birch, "seedDispersalCutoffM");
        Check("D_BIRCH_USES_ONE_CONSERVATIVE_SCALE_FROM_THE_CLOSED_STAND_ANCHOR",
            I(birch, "naturalSeedDispersal") == (int)NaturalSeedDispersal.Wind && I(birch, "seedKernelState") == (int)SpeciesParameterState.Placeholder
            && scale == 38f / 2f, "scale " + scale.ToString(CultureInfo.InvariantCulture) + " = 38 m closed-stand mean / 2");
        Check("D_BIRCH_SCALE_IS_BELOW_THE_OPEN_ANCHOR_CONVERSION", scale < 65f / 2f);
        // 2-D exponential: share of the unnormalised per-area rain inside R is 1 - (1 + R/s) exp(-R/s); mean distance is 2s.
        Func<double, double> within = r => 1.0 - (1.0 + r / scale) * Math.Exp(-r / scale);
        double beyond = 1.0 - within(cutoff);
        Check("D_BIRCH_CUTOFF_TRUNCATES_UNDER_HALF_A_PERCENT_OF_THE_KERNEL", beyond < 0.005, "beyond cutoff " + beyond.ToString("0.0000", CultureInfo.InvariantCulture));
        double mean = 0, norm = 0;
        for (double r = 0.05; r < 600; r += 0.1) { double w = Math.Exp(-r / scale) * r; mean += r * w; norm += w; }
        Check("D_BIRCH_KERNEL_MEAN_DISTANCE_IS_THE_38_M_ANCHOR", Math.Abs(mean / norm - 38.0) < 0.5, (mean / norm).ToString("0.00", CultureInfo.InvariantCulture) + " m");
        Check("D_BIRCH_ONSET_IS_THE_FOREST_RESEARCH_15_YEARS_WITH_NO_INVENTED_RAMP",
            F(birch, "maturityOnsetYears") == 15f && F(birch, "maturityFullYears") == 15f);
        Check("D_BIRCH_MAST_IS_NEUTRAL_NOT_SITKA",
            F(birch, "mastGoodProbability") == 0f && F(birch, "mastPoorProbability") == 0f && F(birch, "mastNormalMultiplier") == 1f);
        Check("D_BIRCH_BROWSE_IS_EXPLICITLY_NON_ZERO", I(birch, "browseState") == (int)SpeciesParameterState.Set && F(birch, "browsePalatability") == 0.5f);

        Check("D_ALDER_DECLARES_WIND_AND_WATER_CAPABILITY",
            I(alder, "naturalSeedDispersal") == (int)NaturalSeedDispersal.WindAndWater);
        Check("D_ALDER_WIND_KERNEL_IS_ON_HOLD_SO_NO_30_M_CUTOFF_EXISTS",
            I(alder, "seedKernelState") == (int)SpeciesParameterState.Hold
            && new[] { "seedDispersalScaleM", "seedDispersalCutoffM" }.All(k => F(alder, k) != 30f)
            && !disperses("common-alder", (NaturalSeedDispersal)I(alder, "naturalSeedDispersal"), true, (SpeciesParameterState)I(alder, "seedKernelState")));
        Check("D_ALDER_BROWSE_IS_EXPLICITLY_NON_ZERO", I(alder, "browseState") == (int)SpeciesParameterState.Set && F(alder, "browsePalatability") == 0.2f);
        string policy = File.ReadAllText(Path.Combine(fp, "SpeciesConfigurationPolicy.cs"));
        Check("D_NO_HARD_30_M_ALDER_CUTOFF_ANYWHERE_IN_THE_FOUNDATION_SOURCE",
            !Regex.IsMatch(policy + File.ReadAllText(Path.Combine(fp, "TreeSpeciesDefinition.cs")), @"(?i)alder[^;\n]{0,80}\b30\s*f?\b[^;\n]{0,20}cutoff"));
    }

    // ---------------------------------------------------------------- F: the new species assets

    private static void CheckF_SpeciesAssets()
    {
        Dictionary<string, string> pine = ReadAsset("ScotsPine"), birch = ReadAsset("DownyBirch"), alder = ReadAsset("CommonAlder");
        SpeciesConfigurationReport rp = SpeciesConfigurationPolicy.Evaluate(FactsFromAsset(pine));
        SpeciesConfigurationReport rb = SpeciesConfigurationPolicy.Evaluate(FactsFromAsset(birch));
        SpeciesConfigurationReport ra = SpeciesConfigurationPolicy.Evaluate(FactsFromAsset(alder));
        Check("F_NO_NEW_ASSET_HAS_A_CONFIGURATION_ERROR", !rp.HasErrors && !rb.HasErrors && !ra.HasErrors,
            string.Join(" | ", rp.Errors.Concat(rb.Errors).Concat(ra.Errors)));
        Check("F_NO_NEW_ASSET_IS_SIMULATION_READY_YET_BECAUSE_UNSUPPORTED_AREAS_ARE_ON_HOLD", !rp.IsReady && !rb.IsReady && !ra.IsReady);
        Check("F_PINE_HOLDS_GROWTH_BROWSE_AND_JUVENILE_LIGHT",
            rp.HoldAreas.SequenceEqual(new[] { "growth", "browse susceptibility", "juvenile light response" }), string.Join(", ", rp.HoldAreas));
        Check("F_BIRCH_HOLDS_GROWTH_AND_JUVENILE_LIGHT_ONLY", rb.HoldAreas.SequenceEqual(new[] { "growth", "juvenile light response" }), string.Join(", ", rb.HoldAreas));
        Check("F_ALDER_HOLDS_GROWTH_JUVENILE_LIGHT_AND_THE_WIND_KERNEL",
            ra.HoldAreas.SequenceEqual(new[] { "growth", "juvenile light response", "seed dispersal kernel" }), string.Join(", ", ra.HoldAreas));
        Check("F_GROWTH_NUMBERS_ARE_INERT_SENTINELS_NOT_VALUES",
            new[] { pine, birch, alder }.All(a => F(a, "potentialDbhGrowthCmPerYear") == 0f && F(a, "potentialHeightGrowthMPerYear") == 0f
                && F(a, "maxDbhCm") == 1f && F(a, "maxHeightM") == 1f && I(a, "growthState") == (int)SpeciesParameterState.Hold));
        Check("F_NO_NEW_ASSET_IS_BROWSE_PROOF_BY_DEFAULT",
            I(pine, "browseState") == (int)SpeciesParameterState.Hold
            && I(birch, "browseState") == (int)SpeciesParameterState.Set && F(birch, "browsePalatability") > 0f
            && I(alder, "browseState") == (int)SpeciesParameterState.Set && F(alder, "browsePalatability") > 0f);
        Check("F_BIRCH_KERNEL_IS_EXPLICITLY_DECLARED_AND_NOT_THE_SITKA_SCALE",
            I(birch, "seedKernelState") != (int)SpeciesParameterState.Unspecified && F(birch, "seedDispersalScaleM") != 20f);
        // Calibration path: what a later packet must do. Flipping Hold -> Placeholder without real values is refused.
        SpeciesConfigurationFacts flip = FactsFromAsset(birch);
        flip.GrowthState = SpeciesParameterState.Placeholder;
        SpeciesConfigurationReport flipped = SpeciesConfigurationPolicy.Evaluate(flip);
        Check("F_FLIPPING_GROWTH_TO_PLACEHOLDER_WITHOUT_NUMBERS_IS_AN_ERROR_NOT_A_SILENT_SITKA_GROWTH", flipped.HasErrors && Has(flipped, "inert HOLD values"));
        SpeciesConfigurationFacts flipLight = FactsFromAsset(birch);
        flipLight.JuvenileLightState = SpeciesParameterState.Placeholder;
        Check("F_FLIPPING_JUVENILE_LIGHT_WITHOUT_DISTINCT_RESPONSES_IS_AN_ERROR", Has(SpeciesConfigurationPolicy.Evaluate(flipLight), "legacy Sitka-calibrated"));

        // None of the three is registered anywhere that Scenario One loads.
        string[] guids = new[] { "ScotsPine", "DownyBirch", "CommonAlder" }.Select(n =>
            Regex.Match(File.ReadAllText(AssetPath(n) + ".meta"), @"guid: ([0-9a-f]{32})").Groups[1].Value).ToArray();
        Check("F_NEW_ASSET_GUIDS_ARE_UNIQUE_AND_WELL_FORMED", guids.All(g => g.Length == 32) && guids.Distinct().Count() == 3);
        var scanned = new List<string>();
        foreach (string ext in new[] { "*.unity", "*.prefab", "*.asset" })
            scanned.AddRange(Directory.GetFiles(Path.Combine(root, "Assets"), ext, SearchOption.AllDirectories));
        bool referenced = scanned.Where(p => !Path.GetFileName(p).Equals("ScotsPine.asset") && !Path.GetFileName(p).Equals("DownyBirch.asset") && !Path.GetFileName(p).Equals("CommonAlder.asset"))
            .Any(p => { string t = File.ReadAllText(p); return guids.Any(g => t.Contains(g)); });
        Check("F_NO_SCENE_PREFAB_OR_ASSET_REFERENCES_THE_NEW_SPECIES", !referenced, scanned.Count + " files scanned");
        string legacyText = string.Concat(new[] { "SitkaSpruce", "SessileOak", "BeechSpecies" }.Select(n => File.ReadAllText(AssetPath(n))));
        Check("F_THE_ESTABLISHED_ASSETS_NAME_NONE_OF_THE_NEW_FIELDS",
            !new[] { "growthState", "browseState", "juvenileLightState", "naturalSeedDispersal", "seedKernelState" }.Any(k => legacyText.Contains(k)));
    }

    // ---------------------------------------------------------------- G: source wiring and scope

    private static void CheckG_SourceWiring()
    {
        string eco = File.ReadAllText(Path.Combine(fp, "ForestEcologyController.cs"));
        int register = eco.IndexOf("regenerationSpeciesById[targetSpecies.SpeciesId] = targetSpecies;", StringComparison.Ordinal);
        int gate = eco.IndexOf("if (!targetSpecies.DispersesSeedNaturally)", StringComparison.Ordinal);
        int mast = eco.IndexOf("float mastMultiplier = GetMastMultiplier(targetSpecies);", StringComparison.Ordinal);
        Check("G_SEED_RAIN_GATE_SITS_AFTER_REGISTRATION_AND_BEFORE_ANY_SEED_ACCUMULATION", register > 0 && gate > register && mast > gate);
        Check("G_ADULT_GROWTH_USES_PHASES_ONLY_WHEN_THEY_EXIST",
            Regex.IsMatch(eco, @"if \(treeSpecies\.HasGrowthAgePhases\)\s*ApplyGrowthAgeFactors\(treeSpecies, tree\.AgeYears"));
        string flat = Regex.Replace(eco, @"\s+", " ");
        Check("G_THE_LEGACY_GROWTH_EXPRESSIONS_ARE_PRESERVED_VERBATIM",
            flat.Contains("float dbhPotential = treeSpecies.PotentialDbhGrowthCmPerYear * site * Mathf.Clamp01(1f - tree.Diameter / treeSpecies.MaxDbhCm);")
            && flat.Contains("float heightGrowth = treeSpecies.PotentialHeightGrowthMPerYear * site * Mathf.Clamp01(1f - tree.Height / treeSpecies.MaxHeightM);")
            && flat.Contains("float dbhGrowth = dbhPotential * (1f / (1f + ci / treeSpecies.Ci50));"));
        string marking = File.ReadAllText(Path.Combine(fp, "ForestTreeMarkingManager.cs"));
        Check("G_THE_THINNING_FORECAST_USES_THE_SAME_PHASES", marking.Contains("treeSpecies.HasGrowthAgePhases") && marking.Contains("GrowthAgeFactors(tree.AgeYears"));
        string spawner = File.ReadAllText(Path.Combine(fp, "ForestTreeSpawner.cs"));
        Check("G_ONLY_SIMULATION_READY_SPECIES_ARE_REGISTERED_AND_RESOLVED",
            Regex.Matches(spawner, "IsSimulationReady").Count >= 5 && spawner.Contains("is not simulation-ready and is not registered"));
        // External colonisation stays authored-only: no spontaneous map-edge / off-map seed rule exists anywhere in the runtime.
        var runtime = Directory.GetFiles(fp, "*.cs", SearchOption.AllDirectories).Where(p => !p.Contains(Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar)).ToList();
        Check("G_NO_SPONTANEOUS_EXTERNAL_COLONISATION_RULE_EXISTS",
            runtime.All(p => !Regex.IsMatch(File.ReadAllText(p), @"(?i)ExternalSeed|Immigrat|SpontaneousColon|MapEdgeSeed|OffMapSeed|BoundarySeedInflow|EdgeColoni")));
        string seedRain = eco.Substring(eco.IndexOf("private void ComputeSpeciesSeedRain", StringComparison.Ordinal), 2400);
        Check("G_SEED_RAIN_COMES_ONLY_FROM_TREES_IN_THE_STAND", seedRain.Contains("SameSpecies(tree.Species, targetSpecies)") && seedRain.Contains("foreach (ForestTree tree in trees)"));
        Check("G_THE_ESTABLISHED_SPECIES_LIST_IS_EXACTLY_THREE",
            File.ReadAllText(Path.Combine(fp, "SpeciesConfigurationPolicy.cs")).Contains("{ SitkaSpruceId, SessileOakId, BeechId }"));
    }
}

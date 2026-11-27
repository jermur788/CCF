using System.Globalization;

namespace SitkaP1.Core;

/// <summary>Species composition mode for a deterministic forest-maker stand (SPEC-multi-2d-phase2a-forest-maker-V1 §5.1).</summary>
internal enum ForestMakerSpeciesMix
{
    PureSitka = 0,
    SitkaPlusEdgeBroadleaves = 1,
    CustomSplit = 2,
}

/// <summary>Core-enforced bounds and constants for the CCF 2D rectangular forest maker v1.</summary>
internal static class ForestMakerBounds
{
    internal const string GeneratorId = "ccf-2d-forest-maker";
    internal const int GeneratorVersion = 1;

    internal const int MinSideM = 30;
    internal const int MaxSideM = 120;
    internal const int MinLanes = 2;
    internal const int MaxLanes = 8;
    internal const int MinAgeYears = 10;
    internal const int MaxAgeYears = 60;
    internal const int MinStemsPerHa = 500;
    internal const int MaxStemsPerHa = 3_000;
    internal const int MaxLivingStems = 3_000;
    internal const int MinSeed = 1;
    internal const int MaxSeed = 100_000_000;

    internal const long EdgeInsetMm = 1_250;
    internal const long ExclusionRadiusMm = 2_000;
    internal const long EdgeMarginMm = 500;
    internal const int EdgeBroadleafAgeYears = 60;
    internal const int EdgeBroadleavesPerOuterEdge = 4;

    internal const long LightCellSizeMm = 2_500;
    internal const long LightCellHalfMm = 1_250;
    internal const long MicroCellSizeMm = 250;
    internal const long MicroCellHalfMm = 125;

    internal const string Disclosure =
        "SYNTHETIC FOREST-MAKER STAND. NOT A SURVEYED IRISH FOREST. SITE UNKNOWN. STARTING SIZES ARE TRANSFERRED/CALIBRATED " +
        "GAMEPLAY VALUES, NOT A YIELD TABLE. CONNECTED WOODLAND: GROWTH, CROWNS AND SEED RAIN USE ALL LANES; DENSITY MORTALITY, " +
        "WORK PLANS AND SUMMARIES STAY PER LANE. NOT UNITY NUMERICAL OR GEOMETRY PARITY.";

    internal const string GeneratorNote =
        "Designed in code by the CCF 2D forest maker v1 from a validated recipe. " +
        "Not derived from the P1 30 m fixture, any P0 fixture or the Unity Enlarged80 lattice.";
}

/// <summary>
/// Immutable, validated input recipe for <see cref="ForestMakerGenerator"/> (SPEC-multi-2d-phase2a-forest-maker-V1 §5.1).
/// Canonical bytes are produced with <see cref="CanonicalWriter"/> in exact table order; any non-canonical encoding or
/// out-of-range value is refused with its own typed <see cref="P1Refusal"/>.
/// </summary>
internal sealed record ForestMakerRecipe(
    string GeneratorId,
    int GeneratorVersion,
    string? PresetId,
    int WidthM,
    int LengthM,
    int LaneCount,
    int StartAgeYears,
    int StemsPerHa,
    int Seed,
    ForestMakerSpeciesMix SpeciesMix,
    int OakPercent,
    int BeechPercent)
{
    private static readonly string[] CanonicalKeys =
    {
        "generatorId",
        "generatorVersion",
        "presetId",
        "widthM",
        "lengthM",
        "laneCount",
        "startAgeYears",
        "stemsPerHa",
        "seed",
        "speciesMix",
        "oakPercent",
        "beechPercent",
    };

    internal long WidthMm => WidthM * 1_000L;
    internal long LengthMm => LengthM * 1_000L;

    /// <summary>Integer round-half-up total stem count: (StemsPerHa * W_mm * L_mm + 5e9) / 1e10.</summary>
    internal int TotalStems()
    {
        Int128 numerator = (Int128)StemsPerHa * WidthMm * LengthMm + 5_000_000_000;
        return checked((int)(numerator / 10_000_000_000));
    }

    /// <summary>
    /// Validates every field and invariant in the exact refusal order of spec §5.1.
    /// Nothing is coerced; every defect throws its own typed <see cref="P1Refusal"/>.
    /// </summary>
    internal ForestMakerRecipe RequireValid()
    {
        if (!string.Equals(GeneratorId, ForestMakerBounds.GeneratorId, StringComparison.Ordinal) ||
            GeneratorVersion != ForestMakerBounds.GeneratorVersion)
        {
            throw new P1Refusal("ForestMakerGeneratorUnsupported",
                $"unsupported forest-maker generator '{GeneratorId}' v{GeneratorVersion}; only {ForestMakerBounds.GeneratorId} v{ForestMakerBounds.GeneratorVersion} is supported");
        }

        if (PresetId is not null)
        {
            var canonicalPreset = ForestMakerPresets.FindByPresetId(PresetId)
                ?? throw new P1Refusal("ForestMakerPresetUnknown", "unknown forest-maker presetId: " + PresetId);
            if (this != canonicalPreset)
                throw new P1Refusal("ForestMakerPresetMismatch", "recipe fields do not match canonical preset " + PresetId);
        }

        if (WidthM is < ForestMakerBounds.MinSideM or > ForestMakerBounds.MaxSideM)
            throw new P1Refusal("ForestMakerWidthOutOfRange",
                $"widthM {WidthM} is outside [{ForestMakerBounds.MinSideM}, {ForestMakerBounds.MaxSideM}] m");

        if (LengthM is < ForestMakerBounds.MinSideM or > ForestMakerBounds.MaxSideM)
            throw new P1Refusal("ForestMakerLengthOutOfRange",
                $"lengthM {LengthM} is outside [{ForestMakerBounds.MinSideM}, {ForestMakerBounds.MaxSideM}] m");

        if (LaneCount is < ForestMakerBounds.MinLanes or > ForestMakerBounds.MaxLanes)
            throw new P1Refusal("ForestMakerLaneCountOutOfRange",
                $"laneCount {LaneCount} is outside [{ForestMakerBounds.MinLanes}, {ForestMakerBounds.MaxLanes}]");

        if (StartAgeYears is < ForestMakerBounds.MinAgeYears or > ForestMakerBounds.MaxAgeYears)
            throw new P1Refusal("ForestMakerAgeOutOfRange",
                $"startAgeYears {StartAgeYears} is outside [{ForestMakerBounds.MinAgeYears}, {ForestMakerBounds.MaxAgeYears}]");

        if (StemsPerHa is < ForestMakerBounds.MinStemsPerHa or > ForestMakerBounds.MaxStemsPerHa)
            throw new P1Refusal("ForestMakerDensityOutOfRange",
                $"stemsPerHa {StemsPerHa} is outside [{ForestMakerBounds.MinStemsPerHa}, {ForestMakerBounds.MaxStemsPerHa}]");

        if (Seed is < ForestMakerBounds.MinSeed or > ForestMakerBounds.MaxSeed)
            throw new P1Refusal("ForestMakerSeedOutOfRange",
                $"seed {Seed} is outside [{ForestMakerBounds.MinSeed}, {ForestMakerBounds.MaxSeed}]");

        if (!Enum.IsDefined(SpeciesMix))
            throw new P1Refusal("ForestMakerSpeciesMixInvalid", "unknown speciesMix value: " + SpeciesMix);

        if (SpeciesMix == ForestMakerSpeciesMix.CustomSplit)
        {
            if (OakPercent is < 0 or > 100 || BeechPercent is < 0 or > 100 || OakPercent + BeechPercent > 100)
                throw new P1Refusal("ForestMakerSpeciesMixInvalid",
                    $"CustomSplit requires oakPercent ({OakPercent}) and beechPercent ({BeechPercent}) in [0, 100] with sum <= 100");
        }
        else if (OakPercent != 0 || BeechPercent != 0)
        {
            throw new P1Refusal("ForestMakerSpeciesMixInvalid",
                $"oakPercent and beechPercent must be 0 unless speciesMix is CustomSplit (got oak={OakPercent}, beech={BeechPercent})");
        }

        int totalStems = TotalStems();
        if (totalStems > ForestMakerBounds.MaxLivingStems)
            throw new P1Refusal("ForestMakerStemCapExceeded",
                string.Create(CultureInfo.InvariantCulture,
                    $"derived total stems {totalStems} exceeds the hard cap of {ForestMakerBounds.MaxLivingStems} living stems"));

        ForestMakerGenerator.RequireGridAlignment(WidthMm, LaneCount);
        int[] quotas = ForestMakerGenerator.ComputeLaneQuotas(totalStems, WidthMm, LaneCount);
        ForestMakerGenerator.RequireNonEmptyQuotas(quotas);

        return this;
    }

    internal static string SpeciesMixWire(ForestMakerSpeciesMix mix) => mix switch
    {
        ForestMakerSpeciesMix.PureSitka => "PureSitka",
        ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves => "SitkaPlusEdgeBroadleaves",
        ForestMakerSpeciesMix.CustomSplit => "CustomSplit",
        _ => throw new P1Refusal("ForestMakerSpeciesMixInvalid", "unknown speciesMix value: " + mix),
    };

    internal static ForestMakerSpeciesMix ParseSpeciesMix(string value) => value switch
    {
        "PureSitka" => ForestMakerSpeciesMix.PureSitka,
        "SitkaPlusEdgeBroadleaves" => ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves,
        "CustomSplit" => ForestMakerSpeciesMix.CustomSplit,
        _ => throw new P1Refusal("ForestMakerSpeciesMixInvalid", "unknown speciesMix: " + value),
    };

    /// <summary>Writes the canonical JSON object representation into <paramref name="writer"/>.</summary>
    internal void WriteTo(CanonicalWriter writer) => writer
        .BeginObject()
        .Str("generatorId", GeneratorId)
        .Int("generatorVersion", GeneratorVersion)
        .StrOrNull("presetId", PresetId)
        .Int("widthM", WidthM)
        .Int("lengthM", LengthM)
        .Int("laneCount", LaneCount)
        .Int("startAgeYears", StartAgeYears)
        .Int("stemsPerHa", StemsPerHa)
        .Int("seed", Seed)
        .Str("speciesMix", SpeciesMixWire(SpeciesMix))
        .Int("oakPercent", OakPercent)
        .Int("beechPercent", BeechPercent)
        .EndObject();

    /// <summary>Canonical UTF-8 bytes in spec §5.1 field order (no whitespace, no trailing newline).</summary>
    internal byte[] CanonicalBytes()
    {
        var writer = new CanonicalWriter();
        WriteTo(writer);
        return writer.ToBytes();
    }

    /// <summary>Lowercase hex SHA-256 of <see cref="CanonicalBytes"/>.</summary>
    internal string RecipeHash() => DerivedManifest.Sha256Hex(CanonicalBytes());

    /// <summary>
    /// Strict canonical decode from UTF-8 bytes. Refuses non-canonical encodings (extra whitespace, reordered keys,
    /// trailing newline) with <c>ForestMakerRecipeNotCanonical</c>, and field defects with their own typed refusal.
    /// </summary>
    internal static ForestMakerRecipe Decode(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        var node = StrictJson.Parse(bytes);
        var recipe = Decode(node);
        if (!recipe.CanonicalBytes().AsSpan().SequenceEqual(bytes))
            throw new P1Refusal("ForestMakerRecipeNotCanonical", "forest-maker recipe bytes are not in canonical form");
        return recipe;
    }

    /// <summary>Strict decode from a parsed <see cref="JNode"/>, enforcing key set, key order, and full validation.</summary>
    internal static ForestMakerRecipe Decode(JNode node)
    {
        var map = J.Keys(node, "recipe", CanonicalKeys);
        var obj = (JObj)node;

        string generatorId = J.Str(map["generatorId"], "recipe.generatorId");
        long generatorVersionRaw = J.Int(map["generatorVersion"], "recipe.generatorVersion");
        if (!string.Equals(generatorId, ForestMakerBounds.GeneratorId, StringComparison.Ordinal) ||
            generatorVersionRaw != ForestMakerBounds.GeneratorVersion)
        {
            throw new P1Refusal("ForestMakerGeneratorUnsupported",
                $"unsupported forest-maker generator '{generatorId}' v{generatorVersionRaw}");
        }

        string? presetId = J.StrOrNull(map["presetId"], "recipe.presetId");
        if (presetId is not null && ForestMakerPresets.FindByPresetId(presetId) is null)
            throw new P1Refusal("ForestMakerPresetUnknown", "unknown forest-maker presetId: " + presetId);

        int widthM = ReadBoundedInt(map["widthM"], "recipe.widthM", ForestMakerBounds.MinSideM, ForestMakerBounds.MaxSideM,
            "ForestMakerWidthOutOfRange", presetId is not null);
        int lengthM = ReadBoundedInt(map["lengthM"], "recipe.lengthM", ForestMakerBounds.MinSideM, ForestMakerBounds.MaxSideM,
            "ForestMakerLengthOutOfRange", presetId is not null);
        int laneCount = ReadBoundedInt(map["laneCount"], "recipe.laneCount", ForestMakerBounds.MinLanes, ForestMakerBounds.MaxLanes,
            "ForestMakerLaneCountOutOfRange", presetId is not null);
        int startAgeYears = ReadBoundedInt(map["startAgeYears"], "recipe.startAgeYears", ForestMakerBounds.MinAgeYears, ForestMakerBounds.MaxAgeYears,
            "ForestMakerAgeOutOfRange", presetId is not null);
        int stemsPerHa = ReadBoundedInt(map["stemsPerHa"], "recipe.stemsPerHa", ForestMakerBounds.MinStemsPerHa, ForestMakerBounds.MaxStemsPerHa,
            "ForestMakerDensityOutOfRange", presetId is not null);
        int seed = ReadBoundedInt(map["seed"], "recipe.seed", ForestMakerBounds.MinSeed, ForestMakerBounds.MaxSeed,
            "ForestMakerSeedOutOfRange", presetId is not null);
        var speciesMix = ParseSpeciesMix(J.Str(map["speciesMix"], "recipe.speciesMix"));
        int oakPercent = ReadInt32(map["oakPercent"], "recipe.oakPercent", presetId is not null ? "ForestMakerPresetMismatch" : "ForestMakerSpeciesMixInvalid");
        int beechPercent = ReadInt32(map["beechPercent"], "recipe.beechPercent", presetId is not null ? "ForestMakerPresetMismatch" : "ForestMakerSpeciesMixInvalid");

        var recipe = new ForestMakerRecipe(
            generatorId,
            (int)generatorVersionRaw,
            presetId,
            widthM,
            lengthM,
            laneCount,
            startAgeYears,
            stemsPerHa,
            seed,
            speciesMix,
            oakPercent,
            beechPercent);

        recipe.RequireValid();

        for (int i = 0; i < CanonicalKeys.Length; i++)
        {
            if (!string.Equals(obj.Props[i].Key, CanonicalKeys[i], StringComparison.Ordinal))
                throw new P1Refusal("ForestMakerRecipeNotCanonical", "forest-maker recipe keys are not in canonical order");
        }

        return recipe;
    }

    private static int ReadBoundedInt(JNode node, string what, int min, int max, string outOfRangeCode, bool deferRangeForPreset)
    {
        long value = J.Int(node, what);
        if (value is < int.MinValue or > int.MaxValue)
            throw new P1Refusal(deferRangeForPreset ? "ForestMakerPresetMismatch" : outOfRangeCode, $"{what} ({value}) is outside [{min}, {max}]");
        if (!deferRangeForPreset && (value < min || value > max))
            throw new P1Refusal(outOfRangeCode, $"{what} ({value}) is outside [{min}, {max}]");
        return (int)value;
    }

    private static int ReadInt32(JNode node, string what, string refusalCode)
    {
        long value = J.Int(node, what);
        if (value is < int.MinValue or > int.MaxValue)
            throw new P1Refusal(refusalCode, $"{what} ({value}) is out of range");
        return (int)value;
    }
}

/// <summary>Canonical 0.64 ha five-lane benchmark presets (SPEC-multi-2d-phase2a-forest-maker-V1 §5.4).</summary>
internal static class ForestMakerPresets
{
    internal const string PrimaryPresetId = "fm-064-sitka-edge-broadleaves-v1";
    internal const string PrimaryExperimentId = "mad-fm-064-sitka-edge-broadleaves-v1";
    internal const int PrimarySeed = 6_400_101;
    internal const string PrimaryBadge =
        "Forest maker \u2014 0.64 ha, 5 lanes, Sitka + edge broadleaves (synthetic, not a surveyed Irish forecast)";

    internal const string PureSitkaPresetId = "fm-064-pure-sitka-v1";
    internal const string PureSitkaExperimentId = "mad-fm-064-pure-sitka-v1";
    internal const int PureSitkaSeed = 6_400_202;
    internal const string PureSitkaBadge =
        "Forest maker \u2014 0.64 ha, 5 lanes, pure Sitka (synthetic, not a surveyed Irish forecast)";

    /// <summary>Primary 0.64 ha preset: 80 x 80 m, 5 lanes, age 20 Sitka + 8 age-60 edge broadleaves (4 Oak + 4 Beech).</summary>
    internal static readonly ForestMakerRecipe SitkaEdgeBroadleaves064 = new(
        GeneratorId: ForestMakerBounds.GeneratorId,
        GeneratorVersion: ForestMakerBounds.GeneratorVersion,
        PresetId: PrimaryPresetId,
        WidthM: 80,
        LengthM: 80,
        LaneCount: 5,
        StartAgeYears: 20,
        StemsPerHa: 2_100,
        Seed: PrimarySeed,
        SpeciesMix: ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves,
        OakPercent: 0,
        BeechPercent: 0);

    /// <summary>Secondary 0.64 ha control preset: 80 x 80 m, 5 lanes, pure age 20 Sitka (1,344 stems).</summary>
    internal static readonly ForestMakerRecipe PureSitka064 = new(
        GeneratorId: ForestMakerBounds.GeneratorId,
        GeneratorVersion: ForestMakerBounds.GeneratorVersion,
        PresetId: PureSitkaPresetId,
        WidthM: 80,
        LengthM: 80,
        LaneCount: 5,
        StartAgeYears: 20,
        StemsPerHa: 2_100,
        Seed: PureSitkaSeed,
        SpeciesMix: ForestMakerSpeciesMix.PureSitka,
        OakPercent: 0,
        BeechPercent: 0);

    internal static readonly IReadOnlyList<ForestMakerRecipe> All = Array.AsReadOnly(new[]
    {
        SitkaEdgeBroadleaves064,
        PureSitka064,
    });

    internal static ForestMakerRecipe? FindByPresetId(string? presetId) => presetId switch
    {
        PrimaryPresetId => SitkaEdgeBroadleaves064,
        PureSitkaPresetId => PureSitka064,
        _ => null,
    };

    internal static ForestMakerRecipe ByPresetId(string presetId) =>
        FindByPresetId(presetId) ?? throw new P1Refusal("ForestMakerPresetUnknown", "unknown forest-maker presetId: " + presetId);

    internal static string ExperimentIdFor(ForestMakerRecipe recipe, string recipeHash) => recipe.PresetId switch
    {
        PrimaryPresetId => PrimaryExperimentId,
        PureSitkaPresetId => PureSitkaExperimentId,
        null => "mad-fm-custom-" + recipeHash[..16],
        _ => throw new P1Refusal("ForestMakerPresetUnknown", "unknown forest-maker presetId: " + recipe.PresetId),
    };

    internal static string BadgeFor(ForestMakerRecipe recipe, double totalAreaHa) => recipe.PresetId switch
    {
        PrimaryPresetId => PrimaryBadge,
        PureSitkaPresetId => PureSitkaBadge,
        null => string.Create(CultureInfo.InvariantCulture,
            $"Forest maker \u2014 {totalAreaHa:0.00} ha, {recipe.LaneCount} lanes, {MixBadgeLabel(recipe)} (synthetic, not a surveyed Irish forecast)"),
        _ => throw new P1Refusal("ForestMakerPresetUnknown", "unknown forest-maker presetId: " + recipe.PresetId),
    };

    private static string MixBadgeLabel(ForestMakerRecipe recipe) => recipe.SpeciesMix switch
    {
        ForestMakerSpeciesMix.PureSitka => "pure Sitka",
        ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves => "Sitka + edge broadleaves",
        ForestMakerSpeciesMix.CustomSplit => string.Create(CultureInfo.InvariantCulture,
            $"custom {100 - recipe.OakPercent - recipe.BeechPercent}% Sitka / {recipe.OakPercent}% Oak / {recipe.BeechPercent}% Beech"),
        _ => recipe.SpeciesMix.ToString(),
    };
}

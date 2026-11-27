namespace SitkaP1.Core.Phase1;

// 2D Phase 1 (spec multi-2d-phase1-save-time-history-v1 §3, APPROVED 2026-11-24): the fixed identity, limits and
// encodings of one independent 2D save file. This is a serialization contract only. It holds no ecological formula, no
// work-approval logic, no economics and no random-seed route; the engine stays the only authority (A1 wires capture and
// restore under the engine's single root lock).

/// <summary>Fixed format marker, schema version and limits of the 2D save file (version 1).</summary>
public static class Phase1SaveFormat
{
    /// <summary>Format marker written first in every file. Anything else is refused, never guessed.</summary>
    public const string Format = "ccf-2d-multiarea-save";

    /// <summary>
    /// The only schema version this code reads or writes. No migration exists (owner choice A). Version 2 (A0 repair R1)
    /// added the engine lineage to the header and removed planId/executionKey/recordedSource from the saved approved plan.
    /// No version 1 file was ever produced outside A0 test scratch, but any stray A0-candidate file now gets a clear
    /// <c>SaveSchemaUnsupported</c> instead of a confusing shape error.
    /// </summary>
    public const int SchemaVersion = 2;

    /// <summary>
    /// Largest file accepted, in bytes (64 MiB). Checked before parsing. A 100-year Phase 2 save with per-cell Stage 5
    /// establishment diagnostics is estimated well below this; the cap exists to refuse hostile or runaway input.
    /// </summary>
    public const int MaxSaveBytes = 64 * 1024 * 1024;

    /// <summary>Longest save label, in characters. Labels are printable ASCII without quote or backslash.</summary>
    public const int MaxLabelLength = 64;

    /// <summary>
    /// Longest free-text reason accepted, in UTF-16 characters. Matches the 2D API's existing limit (PlayerMultiApi
    /// MaxReasonLength = 2000), so every reason the running game can hold can be saved.
    /// </summary>
    public const int MaxReasonLength = 2000;

    /// <summary>Checksum algorithm tag. The checksum covers the exact canonical header bytes followed by the payload bytes.</summary>
    public const string ChecksumAlgorithm = "sha256(header||payload)";

    /// <summary>
    /// The only Stage 4 site-productivity field that can be saved: the neutral uniform 1.0 multiplier
    /// (Stage4Options.Uniform). Any other field is a code delegate with no data form and is refused at capture.
    /// </summary>
    public const string SiteProductivityUniform = "uniform-1.0";

    /// <summary>The value tags used for exact non-integer and non-ASCII values.</summary>
    public static class Tags
    {
        /// <summary>IEEE-754 float32 bit pattern, 8 lowercase hex digits (existing F32 convention).</summary>
        public const string F32 = "f32";
        /// <summary>IEEE-754 float64 bit pattern, 16 lowercase hex digits (same rule, for authoritative doubles).</summary>
        public const string F64 = "f64";
        /// <summary>Two's-complement int64 bit pattern, 16 lowercase hex digits (for longs, never a double).</summary>
        public const string I64 = "i64";
        /// <summary>Free text as lowercase hex of its strict UTF-8 bytes (player reasons may hold any character).</summary>
        public const string Utf8 = "utf8";
    }

    /// <summary>Type tags of the polymorphic Phase 2 planting orders.</summary>
    public static class PlantingTags
    {
        public const string Plant = "plant";
        public const string Shelter = "shelter";
        public const string SpotClear = "spotClear";
    }
}

/// <summary>
/// The model-selection vector (selection order Adult / Mortality / Crown / Regeneration / Species), as plain integers so the
/// public contract does not expose engine-internal enums. Only the five combinations the engine accepts are valid.
/// </summary>
public sealed record Phase1ModelVector(int Adult, int Mortality, int Crown, int Regeneration, int Species)
{
    public static readonly Phase1ModelVector Model0 = new(0, 0, 0, 0, 0);
    public static readonly Phase1ModelVector Stage3CrownC2 = new(0, 0, 1, 0, 0);
    public static readonly Phase1ModelVector Stage4 = new(1, 1, 1, 0, 0);
    public static readonly Phase1ModelVector Stage5 = new(1, 1, 1, 1, 0);
    public static readonly Phase1ModelVector Phase2 = new(1, 1, 1, 1, 1);

    /// <summary>The engine's accepted selections (MultiAreaModelSelection.RequireSupported), nothing more.</summary>
    public static readonly IReadOnlyList<Phase1ModelVector> Supported =
        Array.AsReadOnly(new[] { Model0, Stage3CrownC2, Stage4, Stage5, Phase2 });

    public bool IsSupported => Supported.Contains(this);
    public bool IsStage4 => Adult == 1 && Mortality == 1 && Crown == 1;
    public bool IsStage5 => IsStage4 && Regeneration == 1;
    public bool IsPhase2 => IsStage5 && Species == 1;

    public override string ToString() => FormattableString.Invariant($"{Adult}/{Mortality}/{Crown}/{Regeneration}/{Species}");
}

/// <summary>One area's immutable genesis inputs that affect continuation: its ID, simulation seed and exact area.</summary>
public sealed record Phase1GenesisArea(string AreaId, int SimulationSeed, float AreaHa);

/// <summary>
/// The immutable experiment/genesis identity a save belongs to: the experiment manifest's content hash and the
/// deterministic generator inputs. A save is loadable only into a session whose identity matches every field exactly.
/// </summary>
public sealed record Phase1GenesisIdentity(string ExperimentId, string ManifestContentHash, string GeneratorId,
    int GeneratorVersion, long GeneratorSeed, IReadOnlyList<Phase1GenesisArea> Areas);

/// <summary>
/// The engine/algorithm lineage a save was made under: the same five values the engine stamps into every source token
/// (MultiAreaContinuation.Token). Two engines with the same model vector but a different RNG, growth, P1 algorithm,
/// area-model or action-threshold lineage would continue the forest differently, so a load requires every value to match.
/// </summary>
public sealed record Phase1EngineLineage(int RngModelVersion, int GrowthModelVersion, string P1AlgorithmVersion,
    string AreaModelVersion, long ActionEligibleDbhThresholdMm)
{
    /// <summary>
    /// The lineage THIS engine build stamps for a session with <paramref name="models"/>, built exactly as the
    /// MultiAreaContinuation constructor builds its area-model version and as Token() fills the other four values.
    /// </summary>
    public static Phase1EngineLineage Current(Phase1ModelVector models)
    {
        ArgumentNullException.ThrowIfNull(models);
        if (!models.IsSupported) throw new P1Refusal("SaveModelUnsupported", "model vector " + models + " is not one the engine accepts");
        string baseVersion = models.IsStage4 ? MultiAreaIdentity.AreaModelVersionStage4
            : models.Crown != 0 ? MultiAreaIdentity.AreaModelVersionConnectedCrownC2 : MultiAreaIdentity.AreaModelVersion;
        string areaModel = Phase2Identity.AreaModelVersion(
            Stage5Identity.AreaModelVersion(baseVersion, (MultiAreaRegenerationModelVersion)models.Regeneration),
            (SpeciesModelVersion)models.Species);
        return new Phase1EngineLineage(DerivedManifest.RngModelVersion, DerivedManifest.GrowthModelVersion,
            DerivedManifest.P1AlgorithmVersion, areaModel, MultiAreaIdentity.ActionEligibleDbhThresholdMm);
    }

    internal static Phase1EngineLineage Current(Phase1ModelVector models, EconomyModelVersion economy)
    {
        var baseLineage = Current(models);
        return economy == EconomyModelVersion.Off
            ? baseLineage
            : baseLineage with { AreaModelVersion = EconomyIdentity.AreaModelVersion(baseLineage.AreaModelVersion, economy) };
    }

    public override string ToString() => FormattableString.Invariant(
        $"rng {RngModelVersion}, growth {GrowthModelVersion}, p1 {P1AlgorithmVersion}, area {AreaModelVersion}, threshold {ActionEligibleDbhThresholdMm} mm");
}

/// <summary>
/// What a load requires: the exact model vector, genesis identity and engine lineage of the session the save would
/// replace. Any difference is a hard, explanatory refusal; nothing is upgraded, migrated or substituted (owner choice A).
/// </summary>
public sealed record Phase1SaveExpectation(Phase1ModelVector Models, Phase1GenesisIdentity Genesis, Phase1EngineLineage Lineage);

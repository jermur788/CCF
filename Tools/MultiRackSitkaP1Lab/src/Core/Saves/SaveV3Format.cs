namespace SitkaP1.Core.Saves;

using SitkaP1.Core;
using SitkaP1.Core.Phase1;

/// <summary>
/// Fixed format markers and block keys for Save Schema v3 under frozen S0b contract C-V3
/// (<c>specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md</c> §1).
/// Frozen v2 (<c>src/Core/Phase1/**</c>) stays untouched at <c>SchemaVersion = 2</c>.
/// </summary>
internal static class SaveV3Format
{
    /// <summary>Shared format marker matching v2.</summary>
    internal const string Format = Phase1SaveFormat.Format;

    /// <summary>Save v3 schema version integer (C-V3).</summary>
    internal const int SchemaVersionV3 = 3;

    /// <summary>Outer v3 checksum covers every canonical UTF-8 byte of the envelope before the checksum object.</summary>
    internal const string ChecksumAlgorithm = "sha256(canonical-without-checksum)";

    /// <summary>Fixed generator note carried in the Phase 2A <c>genesisRecipe</c> feature block.</summary>
    internal const string RecipeGeneratorNote = "ccf-2d-forest-maker v1";

    /// <summary>Phase 2A feature block key inside top-level ordinal-sorted <c>blocks</c>.</summary>
    internal const string GenesisRecipeBlockKey = "genesisRecipe";

    /// <summary>Reserved Phase 2B feature block key inside <c>blocks</c> (registered slot, absent until Phase 2B-2b).</summary>
    internal const string EconomyBlockKey = "economy";

    /// <summary>
    /// Scots pine and site-moisture feature block (SPEC-multi-2d-scots-pine-moisture-V1 Part 2): the site layout, Scots pine
    /// identity, juveniles, recruits, deadwood species and pine planting work that the frozen v2 payload cannot name.
    /// </summary>
    internal const string ScotsPineBlockKey = "scotsPine";

    /// <summary>Maximum v3 save file size (same 64 MiB cap as v2).</summary>
    internal const int MaxSaveBytes = Phase1SaveFormat.MaxSaveBytes;
}

/// <summary>
/// Immutable Forest Maker genesis metadata attached to <see cref="MultiAreaRoot.GenesisRecipe"/> for generated stands.
/// Null on every non-Forest-Maker stand (<c>MA-1</c>, <c>MA-1-MIXED</c>) so all 9/9 frozen anchors remain byte-identical.
/// </summary>
internal sealed record ForestMakerGenesisState(
    ForestMakerRecipe Recipe,
    string RecipeHash,
    string GeneratorNote,
    bool EconomyEnabled);

/// <summary>One decoded feature block entry inside a Save v3 <c>blocks</c> object.</summary>
internal sealed record SaveV3Block(int BlockVersion, string CanonicalDataJson);

/// <summary>Complete Save v3 envelope (v2 header + v2 payload + ordinal-sorted feature blocks).</summary>
internal sealed record SaveV3Envelope(
    Phase1SaveHeader Header,
    Phase1SaveState State,
    IReadOnlyDictionary<string, SaveV3Block> Blocks);

/// <summary>Verified Save v3 decode outcome (all structural, canonical, checksum, validator and block gates passed).</summary>
internal sealed record SaveV3Decoded(
    SaveV3Envelope Envelope,
    ForestMakerGenesisState? GenesisRecipe,
    string Checksum,
    int ByteLength);

/// <summary>Unified v2 or v3 decoded save result.</summary>
internal sealed record MultiDecodedSave(
    int SchemaVersion,
    Phase1DecodedSave? V2,
    SaveV3Decoded? V3)
{
    internal Phase1SaveHeader Header => SchemaVersion == SaveV3Format.SchemaVersionV3
        ? V3!.Envelope.Header
        : V2!.Envelope.Header;

    internal string Checksum => SchemaVersion == SaveV3Format.SchemaVersionV3
        ? V3!.Checksum
        : V2!.Checksum;

    internal int ByteLength => SchemaVersion == SaveV3Format.SchemaVersionV3
        ? V3!.ByteLength
        : V2!.ByteLength;

    internal ForestMakerGenesisState? GenesisRecipe => V3?.GenesisRecipe;
}

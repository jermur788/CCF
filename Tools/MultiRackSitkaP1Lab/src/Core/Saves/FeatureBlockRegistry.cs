using SitkaP1.Core;

namespace SitkaP1.Core.Saves;

/// <summary>
/// A reserved Save v3 feature-block slot registered ahead of its owning feature's persistence addendum
/// (for example <c>"economy"</c> reserved for Phase 2B under C-V3).
/// </summary>
internal sealed record ReservedFeatureBlockSlot(string BlockKey, string RefusalCode, string RefusalMessage);

/// <summary>
/// Fail-closed Save v3 feature-block codec registry (C-V3).
/// Maps ordinal-unique block keys to <see cref="IFeatureBlockCodec"/> implementations or explicitly reserved slots.
/// Unknown keys, unsupported block versions, and unimplemented reserved blocks are always refused.
/// </summary>
internal sealed class FeatureBlockRegistry
{
    private readonly Dictionary<string, IFeatureBlockCodec> codecs = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ReservedFeatureBlockSlot> reserved = new(StringComparer.Ordinal);

    /// <summary>Default registry: <c>genesisRecipe</c>, <c>economy</c> and (Scots pine Part 2) <c>scotsPine</c> codecs implemented.</summary>
    internal static FeatureBlockRegistry Default { get; } = CreateDefault();

    internal static FeatureBlockRegistry CreateDefault()
    {
        var registry = new FeatureBlockRegistry();
        registry.Register(new GenesisRecipeBlockCodec());
        registry.Register(new EconomyFeatureBlockCodec());
        registry.Register(new ScotsPineFeatureBlockCodec());
        return registry;
    }

    internal void Register(IFeatureBlockCodec codec)
    {
        ArgumentNullException.ThrowIfNull(codec);
        if (string.IsNullOrWhiteSpace(codec.BlockKey))
            throw new P1Refusal("SaveBlockRegistryInvalid", "feature block key must be non-empty");
        if (codec.CurrentBlockVersion < 1)
            throw new P1Refusal("SaveBlockRegistryInvalid", "feature block CurrentBlockVersion must be >= 1");
        if (codecs.ContainsKey(codec.BlockKey))
            throw new P1Refusal("SaveBlockRegistryDuplicate", "duplicate feature block codec registration: " + codec.BlockKey);
        reserved.Remove(codec.BlockKey);
        codecs.Add(codec.BlockKey, codec);
    }

    internal void Reserve(ReservedFeatureBlockSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);
        if (string.IsNullOrWhiteSpace(slot.BlockKey))
            throw new P1Refusal("SaveBlockRegistryInvalid", "reserved feature block key must be non-empty");
        if (codecs.ContainsKey(slot.BlockKey) || reserved.ContainsKey(slot.BlockKey))
            throw new P1Refusal("SaveBlockRegistryDuplicate", "duplicate feature block slot reservation: " + slot.BlockKey);
        reserved.Add(slot.BlockKey, slot);
    }

    internal bool TryGetCodec(string blockKey, out IFeatureBlockCodec? codec) =>
        codecs.TryGetValue(blockKey, out codec);

    internal bool TryGetReserved(string blockKey, out ReservedFeatureBlockSlot? slot) =>
        reserved.TryGetValue(blockKey, out slot);

    internal bool IsKnownKey(string blockKey) =>
        codecs.ContainsKey(blockKey) || reserved.ContainsKey(blockKey);

    internal IReadOnlyList<string> RegisteredKeys =>
        Array.AsReadOnly(codecs.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray());

    internal IReadOnlyList<string> ReservedKeys =>
        Array.AsReadOnly(reserved.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray());

    internal IFeatureBlockCodec RequireCodecForDecode(string blockKey, int blockVersion)
    {
        if (blockVersion < 1)
            throw new P1Refusal("SaveBlockVersionUnsupported",
                $"feature block '{blockKey}' has invalid blockVersion {blockVersion}; must be >= 1");
        if (reserved.TryGetValue(blockKey, out var slot))
            throw new P1Refusal(slot.RefusalCode, slot.RefusalMessage);
        if (!codecs.TryGetValue(blockKey, out var codec))
            throw new P1Refusal("SaveBlockUnknown",
                $"unknown Save v3 feature block key '{blockKey}'; fail-closed refusal (nothing is ignored)");
        if (!codec.CanRead(blockVersion))
            throw new P1Refusal("SaveBlockVersionUnsupported",
                $"feature block '{blockKey}' version {blockVersion} is not supported by this build");
        return codec;
    }
}

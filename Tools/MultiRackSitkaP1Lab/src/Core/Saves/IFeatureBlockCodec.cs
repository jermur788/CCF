namespace SitkaP1.Core.Saves;

using SitkaP1.Core;

/// <summary>
/// Integrator-owned Save v3 feature-block codec seam (SPEC-multi-2d-s0-seam-extraction-V1 §6.5, §6.6 C-V3).
/// Pure .NET with zero UI/HTTP dependencies, aligned with <see cref="System.Text.Json"/> canonical UTF-8 JSON.
/// S0b ships this interface with zero implementations and zero references from frozen Save v2 code
/// (<c>src/Core/Phase1/**</c> remains read-only with <c>SchemaVersion = 2</c>).
/// </summary>
internal interface IFeatureBlockCodec
{
    /// <summary>
    /// Ordinal-unique key inside the Save v3 <c>blocks</c> object (for example <c>"genesisRecipe"</c> for Phase 2A
    /// or <c>"economy"</c> reserved for Phase 2B).
    /// </summary>
    string BlockKey { get; }

    /// <summary>Positive integer block schema version (&gt;= 1) that this build writes.</summary>
    int CurrentBlockVersion { get; }

    /// <summary>
    /// Fail-closed version gate: returns <c>true</c> only for block versions this codec explicitly understands.
    /// Returning <c>false</c> must cause load refusal; versions are never guessed, coerced, or silently upgraded.
    /// </summary>
    bool CanRead(int blockVersion);

    /// <summary>
    /// Serialises this feature's slice of <paramref name="root"/> as canonical compact UTF-8 JSON object text
    /// (fixed key order, no extra whitespace, aligned with <see cref="System.Text.Json.Utf8JsonWriter"/>).
    /// Must never mutate <paramref name="root"/> or acquire the continuation lock.
    /// </summary>
    string Encode(MultiAreaRoot root);

    /// <summary>
    /// Decodes <paramref name="canonicalBlockJson"/> at <paramref name="blockVersion"/> (aligned with
    /// <see cref="System.Text.Json.JsonDocument"/>) and returns a new immutable <see cref="MultiAreaRoot"/> with
    /// this feature's state attached to <paramref name="restoredSoFar"/>.
    /// Fail-closed: throws <see cref="P1Refusal"/> on any unsupported version, unknown/missing key, or invalid value;
    /// nothing is silently ignored, defaulted, or repaired.
    /// </summary>
    MultiAreaRoot Decode(string canonicalBlockJson, int blockVersion, MultiAreaRoot restoredSoFar);
}

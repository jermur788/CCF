using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Core.Saves;

/// <summary>
/// Strict canonical Save Schema v3 encoder/decoder under frozen S0b contract C-V3.
/// Delegates header/payload canonicalization and validation to frozen <see cref="Phase1SaveCodec"/> without modifying
/// any file under <c>src/Core/Phase1/**</c>. Top-level <c>blocks</c> keys are ordinal-sorted, each block has exact keys
/// <c>{blockVersion, data}</c>, and decoding is strictly fail-closed on unknown keys, unsupported versions, missing
/// required blocks, non-canonical bytes, or checksum mismatch.
/// </summary>
internal static class SaveV3Codec
{
    /// <summary>
    /// Header lineage of a Save v3 session. Economy On appends its area-model version exactly as the live session does
    /// (<see cref="MultiAreaContinuation"/>) without touching the frozen v2 path in <c>src/Core/Phase1/**</c>, which the
    /// 2D gate keeps byte-identical to the base commit.
    /// </summary>
    internal static Phase1EngineLineage LineageFor(Phase1ModelVector models, EconomyModelVersion economy)
    {
        var baseLineage = Phase1EngineLineage.Current(models);
        return economy == EconomyModelVersion.Off
            ? baseLineage
            : baseLineage with { AreaModelVersion = EconomyIdentity.AreaModelVersion(baseLineage.AreaModelVersion, economy) };
    }

    private static readonly string[] TopLevelKeys =
    {
        "format",
        "schemaVersion",
        "header",
        "payload",
        "blocks",
        "checksum",
    };

    private static readonly string[] BlockWrapperKeys =
    {
        "blockVersion",
        "data",
    };

    private static readonly string[] ChecksumKeys =
    {
        "alg",
        "value",
    };

    /// <summary>Encodes a Forest Maker (or feature-blocked) session into a canonical Save v3 UTF-8 byte array.</summary>
    internal static byte[] Encode(Phase1SaveEnvelope innerV2, MultiAreaRoot root, FeatureBlockRegistry? registry = null)
    {
        ArgumentNullException.ThrowIfNull(innerV2);
        ArgumentNullException.ThrowIfNull(root);
        var reg = registry ?? FeatureBlockRegistry.Default;

        if (root.Economy is not null || (root.GenesisRecipe?.EconomyEnabled ?? false))
        {
            if (root.GenesisRecipe is { EconomyEnabled: true } && root.Economy is null)
                throw new P1Refusal("SaveInconsistent", "genesisRecipe selects Economy enabled but root has no Economy state");
            if (root.GenesisRecipe is { EconomyEnabled: false } && root.Economy is not null)
                throw new P1Refusal("SaveInconsistent", "genesisRecipe selects Economy disabled but root has Economy state");
            if (reg.TryGetReserved(SaveV3Format.EconomyBlockKey, out var slot))
                throw new P1Refusal(slot!.RefusalCode, slot.RefusalMessage);
            if (!reg.TryGetCodec(SaveV3Format.EconomyBlockKey, out _))
                throw new P1Refusal("SaveEconomyRequiresV3",
                    "sessions with Economy enabled cannot be saved without a registered economy block codec");
        }

        // Reuse frozen Phase1SaveCodec.Encode so header + payload satisfy every v2 validation and canonical rule.
        byte[] v2Bytes = Phase1SaveCodec.Encode(innerV2);
        var (headerJson, payloadJson) = ExtractCanonicalHeaderAndPayload(v2Bytes);

        // Collect all applicable feature blocks in strict ordinal key order.
        var encodedBlocks = new SortedDictionary<string, SaveV3Block>(StringComparer.Ordinal);
        if (root.GenesisRecipe is not null)
        {
            if (!reg.TryGetCodec(SaveV3Format.GenesisRecipeBlockKey, out var codec) || codec is null)
                throw new P1Refusal("SaveRequiredBlockMissing", "genesisRecipe codec is not registered");
            string dataJson = codec.Encode(root);
            encodedBlocks.Add(codec.BlockKey, new SaveV3Block(codec.CurrentBlockVersion, dataJson));
        }

        if (root.Economy is not null && reg.TryGetCodec(SaveV3Format.EconomyBlockKey, out var econCodec) && econCodec is not null)
        {
            string dataJson = econCodec.Encode(root);
            encodedBlocks.Add(econCodec.BlockKey, new SaveV3Block(econCodec.CurrentBlockVersion, dataJson));
        }

        string blocksJson = EncodeBlocksJson(encodedBlocks);
        string prefixWithoutChecksum = BuildPrefixWithoutChecksum(headerJson, payloadJson, blocksJson);
        string checksum = Sha256Hex(prefixWithoutChecksum);
        byte[] bytes = Encoding.UTF8.GetBytes(BuildFullEnvelope(prefixWithoutChecksum, checksum));
        if (bytes.Length > SaveV3Format.MaxSaveBytes)
            throw new P1Refusal("SaveTooLarge", FormattableString.Invariant(
                $"the encoded v3 save is {bytes.Length} bytes; the limit is {SaveV3Format.MaxSaveBytes}"));
        return bytes;
    }

    /// <summary>
    /// Decodes and fully verifies a Save v3 byte payload (C-V3 fail-closed gates).
    /// Checks size, UTF-8, strict JSON, format/schema, exact keys &amp; order, v2 header/payload gates,
    /// ordinal-sorted blocks, codec version/data decoding, canonical byte equality, checksum, and required blocks.
    /// </summary>
    internal static SaveV3Decoded Decode(
        ReadOnlySpan<byte> bytes,
        FeatureBlockRegistry? registry = null,
        IReadOnlySet<string>? requiredBlockKeys = null,
        int maxBytes = SaveV3Format.MaxSaveBytes)
    {
        if (maxBytes <= 0 || maxBytes > SaveV3Format.MaxSaveBytes)
            throw new ArgumentOutOfRangeException(nameof(maxBytes));
        if (bytes.Length > maxBytes)
            throw new P1Refusal("SaveTooLarge", FormattableString.Invariant(
                $"the save is {bytes.Length} bytes; the limit is {maxBytes}"));
        if (!System.Text.Unicode.Utf8.IsValid(bytes))
            throw new P1Refusal("SaveUtf8Invalid", "the save is not valid UTF-8; it is corrupt");

        JNode rootNode;
        try { rootNode = StrictJson.Parse(bytes); }
        catch (InvalidOperationException)
        {
            throw new P1Refusal("SaveUtf8Invalid", "the save could not be read as UTF-8 JSON text; it is corrupt");
        }

        RequireFormatAndSchemaV3(rootNode);
        var top = J.Keys(rootNode, "save", TopLevelKeys);
        var topObj = (JObj)rootNode;
        for (int i = 0; i < TopLevelKeys.Length; i++)
        {
            if (!string.Equals(topObj.Props[i].Key, TopLevelKeys[i], StringComparison.Ordinal))
                throw new P1Refusal("SaveNotCanonical", "top-level Save v3 envelope keys are not in canonical order");
        }

        var check = J.Keys(top["checksum"], "checksum", ChecksumKeys);
        var checkObj = (JObj)top["checksum"];
        if (!string.Equals(checkObj.Props[0].Key, "alg", StringComparison.Ordinal) ||
            !string.Equals(checkObj.Props[1].Key, "value", StringComparison.Ordinal))
            throw new P1Refusal("SaveNotCanonical", "checksum keys are not in canonical order");
        if (!string.Equals(J.Str(check["alg"], "checksum.alg"), SaveV3Format.ChecksumAlgorithm, StringComparison.Ordinal))
            throw new P1Refusal("SaveChecksumInvalid", "unknown v3 checksum algorithm");
        string storedChecksum = J.Str(check["value"], "checksum.value");
        if (!IsLowerHex(storedChecksum, 64))
            throw new P1Refusal("SaveChecksumInvalid", "the checksum must be 64 lowercase hex digits");

        // Extract raw UTF-8 slices for header, payload, and each block's data directly from bytes.
        var (rawHeaderJson, rawPayloadJson, rawBlockDataByKey) = ExtractRawV3Slices(bytes);

        // Verify header + payload with EVERY frozen v2 gate (UTF-8, strict JSON, canonical bytes, checksum, validator).
        string innerChecksum = Sha256Hex(rawHeaderJson + rawPayloadJson);
        string innerV2Json = "{\"format\":\"" + Phase1SaveFormat.Format + "\",\"schemaVersion\":" +
            Phase1SaveFormat.SchemaVersion.ToString(CultureInfo.InvariantCulture) +
            ",\"header\":" + rawHeaderJson + ",\"payload\":" + rawPayloadJson +
            ",\"checksum\":{\"alg\":\"" + Phase1SaveFormat.ChecksumAlgorithm + "\",\"value\":\"" + innerChecksum + "\"}}";
        var innerDecoded = Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(innerV2Json), expectation: null, maxBytes: SaveV3Format.MaxSaveBytes);

        // Verify blocks object: strictly ordinal-sorted keys FIRST, then exact {blockVersion, data} shape and codec gates.
        if (top["blocks"] is not JObj blocksObj)
            throw new P1Refusal("SchemaInvalid", "save.blocks must be a JSON object");

        string? prevKey = null;
        foreach (var kv in blocksObj.Props)
        {
            if (prevKey is not null && string.CompareOrdinal(prevKey, kv.Key) >= 0)
                throw new P1Refusal("SaveNotCanonical",
                    $"blocks keys must be strictly ordinal-sorted ('{kv.Key}' cannot follow '{prevKey}')");
            prevKey = kv.Key;
        }

        var reg = registry ?? FeatureBlockRegistry.Default;
        var decodedBlocks = new SortedDictionary<string, SaveV3Block>(StringComparer.Ordinal);
        ForestMakerGenesisState? genesisRecipe = null;

        foreach (var kv in blocksObj.Props)
        {
            string key = kv.Key;

            var bMap = J.Keys(kv.Value, "blocks." + key, BlockWrapperKeys);
            var bObj = (JObj)kv.Value;
            if (!string.Equals(bObj.Props[0].Key, "blockVersion", StringComparison.Ordinal) ||
                !string.Equals(bObj.Props[1].Key, "data", StringComparison.Ordinal))
                throw new P1Refusal("SaveNotCanonical", $"blocks.{key} keys must be [blockVersion, data] in canonical order");

            long rawVer = J.Int(bMap["blockVersion"], $"blocks.{key}.blockVersion");
            if (rawVer < 1 || rawVer > int.MaxValue)
                throw new P1Refusal("SaveBlockVersionUnsupported", $"blocks.{key}.blockVersion ({rawVer}) must be a positive 32-bit integer");
            int blockVersion = (int)rawVer;

            var codec = reg.RequireCodecForDecode(key, blockVersion);
            string rawDataJson = rawBlockDataByKey[key];

            if (codec is GenesisRecipeBlockCodec genesisCodec)
            {
                genesisRecipe = genesisCodec.DecodeState(rawDataJson, blockVersion);
            }
            else
            {
                // For any other registered IFeatureBlockCodec, validate decoding against a placeholder root.
                var restoredBase = Phase1RootMapping.Restore(innerDecoded.Envelope.State, innerDecoded.Envelope.Header.Models).Root;
                _ = codec.Decode(rawDataJson, blockVersion, restoredBase);
            }

            decodedBlocks.Add(key, new SaveV3Block(blockVersion, rawDataJson));
        }

        // Canonical byte equality over the entire v3 file BEFORE accepting checksum.
        string canonicalBlocks = EncodeBlocksJson(decodedBlocks);
        string prefixWithoutChecksum = BuildPrefixWithoutChecksum(rawHeaderJson, rawPayloadJson, canonicalBlocks);
        byte[] canonicalBytes = Encoding.UTF8.GetBytes(BuildFullEnvelope(prefixWithoutChecksum, storedChecksum));
        if (!bytes.SequenceEqual(canonicalBytes))
            throw new P1Refusal("SaveNotCanonical",
                "the v3 save is not in the one canonical encoding (order, spacing or spelling differs)");

        string computedChecksum = Sha256Hex(prefixWithoutChecksum);
        if (!string.Equals(computedChecksum, storedChecksum, StringComparison.Ordinal))
            throw new P1Refusal("SaveChecksumMismatch",
                "the v3 save content does not match its checksum; it is corrupt or was edited");

        // Fail-closed required-block checks (C-V3): never silently default missing required blocks.
        if (string.Equals(innerDecoded.Envelope.Header.Genesis.GeneratorId, ForestMakerBounds.GeneratorId, StringComparison.Ordinal) &&
            !decodedBlocks.ContainsKey(SaveV3Format.GenesisRecipeBlockKey))
        {
            throw new P1Refusal("SaveRequiredBlockMissing",
                "this forest-maker v3 save is missing the required 'genesisRecipe' block");
        }

        if (genesisRecipe is { EconomyEnabled: true } && !decodedBlocks.ContainsKey(SaveV3Format.EconomyBlockKey))
        {
            throw new P1Refusal("SaveRequiredBlockMissing",
                "this v3 save selects Economy enabled but is missing the required 'economy' block; economy state is never defaulted");
        }

        if (genesisRecipe is { EconomyEnabled: false } && decodedBlocks.ContainsKey(SaveV3Format.EconomyBlockKey))
        {
            throw new P1Refusal("SaveInconsistent",
                "this v3 save selects Economy disabled but contains an 'economy' block");
        }

        if (requiredBlockKeys is not null)
        {
            foreach (string req in requiredBlockKeys)
            {
                if (!decodedBlocks.ContainsKey(req))
                    throw new P1Refusal("SaveRequiredBlockMissing",
                        $"the v3 save is missing required feature block '{req}' for the session's model selection");
            }
        }

        var envelope = new SaveV3Envelope(innerDecoded.Envelope.Header, innerDecoded.Envelope.State, decodedBlocks);
        return new SaveV3Decoded(envelope, genesisRecipe, storedChecksum, bytes.Length);
    }

    /// <summary>
    /// Spec §6.3 steps 2–4: verifies the decoded v3 genesis recipe, regenerates the experiment from the recipe,
    /// requires exact manifest content hash / area / seed / hectare match, and requires model vector 1/1/1/1/1 + current lineage.
    /// </summary>
    internal static ForestMakerResult VerifyRecipeGenesis(SaveV3Decoded decoded)
    {
        ArgumentNullException.ThrowIfNull(decoded);
        if (decoded.GenesisRecipe is not { } genesis)
            throw new P1Refusal("SaveRequiredBlockMissing", "a forest-maker v3 load requires the 'genesisRecipe' block");

        if (genesis.EconomyEnabled && !decoded.Envelope.Blocks.ContainsKey(SaveV3Format.EconomyBlockKey))
            throw new P1Refusal("SaveRequiredBlockMissing",
                "this forest-maker v3 save selects Economy enabled but is missing the required 'economy' block");

        var header = decoded.Envelope.Header;
        if (header.Models != Phase1ModelVector.Phase2)
            throw new P1Refusal("SaveModelMismatch",
                "this save was made with model vector " + header.Models +
                " and cannot be loaded into a " + Phase1ModelVector.Phase2 +
                " session; saves are never upgraded or migrated between models");

        var expectedLineage = LineageFor(
            Phase1ModelVector.Phase2,
            genesis.EconomyEnabled ? EconomyModelVersion.Enabled : EconomyModelVersion.Off);
        if (header.Lineage != expectedLineage)
            throw new P1Refusal("SaveLineageMismatch",
                "this save was made by an engine with lineage [" + header.Lineage +
                "], but the running engine has [" + expectedLineage +
                "]; a save is never continued by a different random, growth, algorithm, area-model or threshold version");

        ForestMakerRecipe recipe = genesis.Recipe;
        try { recipe.RequireValid(); }
        catch (P1Refusal r) when (string.Equals(r.Code, "ForestMakerGeneratorUnsupported", StringComparison.Ordinal))
        {
            throw new P1Refusal("SaveRecipeUnsupported", r.Message);
        }

        string computedRecipeHash = recipe.RecipeHash();
        if (!string.Equals(genesis.RecipeHash, computedRecipeHash, StringComparison.Ordinal))
            throw new P1Refusal("SaveRecipeHashMismatch", "stored recipeHash does not match canonical recipe SHA-256");

        var generated = ForestMakerGenerator.Generate(recipe);
        var expectedGenesis = Phase1RootMapping.GenesisOf(generated.Experiment);
        var savedGenesis = header.Genesis;

        bool matches =
            string.Equals(savedGenesis.ExperimentId, expectedGenesis.ExperimentId, StringComparison.Ordinal) &&
            string.Equals(savedGenesis.ManifestContentHash, expectedGenesis.ManifestContentHash, StringComparison.Ordinal) &&
            string.Equals(savedGenesis.GeneratorId, expectedGenesis.GeneratorId, StringComparison.Ordinal) &&
            savedGenesis.GeneratorVersion == expectedGenesis.GeneratorVersion &&
            savedGenesis.GeneratorSeed == expectedGenesis.GeneratorSeed &&
            savedGenesis.Areas.Count == expectedGenesis.Areas.Count &&
            savedGenesis.Areas.Zip(expectedGenesis.Areas).All(pair =>
                string.Equals(pair.First.AreaId, pair.Second.AreaId, StringComparison.Ordinal) &&
                pair.First.SimulationSeed == pair.Second.SimulationSeed &&
                F32.Bits(pair.First.AreaHa) == F32.Bits(pair.Second.AreaHa));

        if (!matches)
            throw new P1Refusal("SaveRecipeGenesisMismatch",
                $"the regenerated forest-maker stand '{expectedGenesis.ExperimentId}' (content hash {expectedGenesis.ManifestContentHash}) " +
                $"does not match the save header genesis '{savedGenesis.ExperimentId}' (content hash {savedGenesis.ManifestContentHash})");

        return generated;
    }

    /// <summary>Peeks the schema version of a raw save file (2 or 3) and dispatches to the frozen v2 or strict v3 decoder.</summary>
    internal static MultiDecodedSave DecodeAny(
        ReadOnlySpan<byte> bytes,
        Phase1SaveExpectation? v2Expectation = null,
        FeatureBlockRegistry? registry = null,
        IReadOnlySet<string>? requiredV3Blocks = null,
        int maxBytes = SaveV3Format.MaxSaveBytes)
    {
        if (bytes.Length > maxBytes)
            throw new P1Refusal("SaveTooLarge", FormattableString.Invariant(
                $"the save is {bytes.Length} bytes; the limit is {maxBytes}"));
        if (!System.Text.Unicode.Utf8.IsValid(bytes))
            throw new P1Refusal("SaveUtf8Invalid", "the save is not valid UTF-8; it is corrupt");

        int? schemaVersion = PeekSchemaVersion(bytes);
        if (schemaVersion == Phase1SaveFormat.SchemaVersion)
        {
            var v2 = Phase1SaveCodec.Decode(bytes, v2Expectation, maxBytes);
            return new MultiDecodedSave(Phase1SaveFormat.SchemaVersion, v2, null);
        }
        if (schemaVersion == SaveV3Format.SchemaVersionV3)
        {
            var v3 = Decode(bytes, registry, requiredV3Blocks, maxBytes);
            return new MultiDecodedSave(SaveV3Format.SchemaVersionV3, null, v3);
        }

        // Delegate unknown/malformed format or unsupported schema versions so error codes match v2 exactly.
        _ = Phase1SaveCodec.Decode(bytes, v2Expectation, maxBytes);
        throw new P1Refusal("SaveSchemaUnsupported", "unsupported save schema version");
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static int? PeekSchemaVersion(ReadOnlySpan<byte> bytes)
    {
        try
        {
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow });
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject) return null;
            string? format = null;
            int? version = null;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) return null;
                string prop = reader.GetString()!;
                if (!reader.Read()) return null;
                if (prop == "format" && reader.TokenType == JsonTokenType.String)
                    format = reader.GetString();
                else if (prop == "schemaVersion" && reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int v))
                    version = v;
                else if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                    reader.Skip();
            }
            return string.Equals(format, SaveV3Format.Format, StringComparison.Ordinal) ? version : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static void RequireFormatAndSchemaV3(JNode root)
    {
        if (root is not JObj obj)
            throw new P1Refusal("SaveFormatUnknown", "a 2D save must be a JSON object");
        JNode? format = null, schema = null;
        foreach (var kv in obj.Props)
        {
            if (kv.Key == "format") format = kv.Value;
            else if (kv.Key == "schemaVersion") schema = kv.Value;
        }
        if (format is not JStr f || f.Value != SaveV3Format.Format)
            throw new P1Refusal("SaveFormatUnknown", "this is not a " + SaveV3Format.Format + " file");
        if (schema is not JInt s || s.Value != SaveV3Format.SchemaVersionV3)
            throw new P1Refusal("SaveSchemaUnsupported",
                "only schema versions 2 and 3 can be read; no migration exists");
    }

    private static (string HeaderJson, string PayloadJson) ExtractCanonicalHeaderAndPayload(ReadOnlySpan<byte> v2Bytes)
    {
        var reader = new Utf8JsonReader(v2Bytes);
        reader.Read(); // StartObject
        string? header = null, payload = null;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            string name = reader.GetString()!;
            reader.Read();
            if (name is "header" or "payload")
            {
                int start = checked((int)reader.TokenStartIndex);
                reader.Skip();
                int end = checked((int)reader.BytesConsumed);
                string slice = Encoding.UTF8.GetString(v2Bytes[start..end]);
                if (name == "header") header = slice;
                else payload = slice;
            }
            else if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
            {
                reader.Skip();
            }
        }
        if (header is null || payload is null)
            throw new P1Refusal("SaveNotCanonical", "failed to extract canonical header and payload from inner v2 save");
        return (header, payload);
    }

    private static (string HeaderJson, string PayloadJson, Dictionary<string, string> BlockDataByKey) ExtractRawV3Slices(ReadOnlySpan<byte> bytes)
    {
        var reader = new Utf8JsonReader(bytes);
        reader.Read(); // StartObject
        string? header = null, payload = null;
        var blockData = new Dictionary<string, string>(StringComparer.Ordinal);
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            string name = reader.GetString()!;
            reader.Read();
            if (name is "header" or "payload")
            {
                int start = checked((int)reader.TokenStartIndex);
                reader.Skip();
                int end = checked((int)reader.BytesConsumed);
                string slice = Encoding.UTF8.GetString(bytes[start..end]);
                if (name == "header") header = slice;
                else payload = slice;
            }
            else if (name == "blocks" && reader.TokenType == JsonTokenType.StartObject)
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    string blockKey = reader.GetString()!;
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                        {
                            string field = reader.GetString()!;
                            reader.Read();
                            if (field == "data")
                            {
                                int start = checked((int)reader.TokenStartIndex);
                                if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                                    reader.Skip();
                                int end = checked((int)reader.BytesConsumed);
                                blockData[blockKey] = Encoding.UTF8.GetString(bytes[start..end]);
                            }
                            else if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                            {
                                reader.Skip();
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        reader.Skip();
                    }
                }
            }
            else if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
            {
                reader.Skip();
            }
        }
        if (header is null || payload is null)
            throw new P1Refusal("SaveNotCanonical", "Save v3 envelope missing header or payload object");
        return (header, payload, blockData);
    }

    private static string EncodeBlocksJson(SortedDictionary<string, SaveV3Block> blocks)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        bool first = true;
        foreach (var (key, block) in blocks)
        {
            if (!first) sb.Append(',');
            first = false;
            sb.Append('"').Append(JsonEncodedText.Encode(key)).Append("\":{\"blockVersion\":")
                .Append(block.BlockVersion.ToString(CultureInfo.InvariantCulture))
                .Append(",\"data\":").Append(block.CanonicalDataJson).Append('}');
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static string BuildPrefixWithoutChecksum(string headerJson, string payloadJson, string blocksJson) =>
        "{\"format\":\"" + SaveV3Format.Format + "\",\"schemaVersion\":" +
        SaveV3Format.SchemaVersionV3.ToString(CultureInfo.InvariantCulture) +
        ",\"header\":" + headerJson + ",\"payload\":" + payloadJson + ",\"blocks\":" + blocksJson;

    private static string BuildFullEnvelope(string prefixWithoutChecksum, string checksum) =>
        prefixWithoutChecksum + ",\"checksum\":{\"alg\":\"" + SaveV3Format.ChecksumAlgorithm +
        "\",\"value\":\"" + checksum + "\"}}";

    private static string Sha256Hex(string utf8Text) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(utf8Text))).ToLowerInvariant();

    private static bool IsLowerHex(string s, int length)
    {
        if (s.Length != length) return false;
        foreach (char c in s)
            if (c is not ((>= '0' and <= '9') or (>= 'a' and <= 'f'))) return false;
        return true;
    }
}

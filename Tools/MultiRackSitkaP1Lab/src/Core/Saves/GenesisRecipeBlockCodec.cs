using System.Text;
using SitkaP1.Core;

namespace SitkaP1.Core.Saves;

/// <summary>
/// Phase 2A Save v3 feature-block codec for <c>blocks.genesisRecipe</c> (C-V3 contract, spec §6.2–§6.3).
/// Pure .NET, zero UI/HTTP dependencies, fail-closed canonical JSON.
/// </summary>
internal sealed class GenesisRecipeBlockCodec : IFeatureBlockCodec
{
    private static readonly string[] CanonicalKeys =
    {
        "recipe",
        "recipeHash",
        "generatorNote",
        "economyEnabled",
    };

    public string BlockKey => SaveV3Format.GenesisRecipeBlockKey;

    public int CurrentBlockVersion => 1;

    public bool CanRead(int blockVersion) => blockVersion == 1;

    public string Encode(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (root.GenesisRecipe is not { } genesis)
            throw new P1Refusal("SaveRequiredBlockMissing", "cannot encode genesisRecipe block: root has no forest-maker recipe");
        if (root.Economy is not null || genesis.EconomyEnabled)
            throw new P1Refusal("SaveEconomyRequiresV3",
                "sessions with Economy enabled cannot be saved until the Phase 2B economy feature block is implemented; economy state is never silently dropped");

        genesis.Recipe.RequireValid();
        string expectedHash = genesis.Recipe.RecipeHash();
        if (!string.Equals(genesis.RecipeHash, expectedHash, StringComparison.Ordinal))
            throw new P1Refusal("SaveRecipeHashMismatch", "genesis recipe hash does not match canonical recipe bytes");
        if (!string.Equals(genesis.GeneratorNote, SaveV3Format.RecipeGeneratorNote, StringComparison.Ordinal))
            throw new P1Refusal("SaveRecipeUnsupported", "unsupported genesisRecipe generatorNote: " + genesis.GeneratorNote);

        return EncodeCanonical(genesis);
    }

    internal static string EncodeCanonical(ForestMakerGenesisState genesis)
    {
        StrictJson.CheckString(genesis.RecipeHash);
        StrictJson.CheckString(genesis.GeneratorNote);
        var sb = new StringBuilder();
        sb.Append("{\"recipe\":").Append(Encoding.UTF8.GetString(genesis.Recipe.CanonicalBytes()))
            .Append(",\"recipeHash\":\"").Append(genesis.RecipeHash)
            .Append("\",\"generatorNote\":\"").Append(genesis.GeneratorNote)
            .Append("\",\"economyEnabled\":").Append(genesis.EconomyEnabled ? "true" : "false")
            .Append('}');
        return sb.ToString();
    }

    public MultiAreaRoot Decode(string canonicalBlockJson, int blockVersion, MultiAreaRoot restoredSoFar)
    {
        ArgumentNullException.ThrowIfNull(canonicalBlockJson);
        ArgumentNullException.ThrowIfNull(restoredSoFar);
        var state = DecodeState(canonicalBlockJson, blockVersion);
        return restoredSoFar with { GenesisRecipe = state };
    }

    internal ForestMakerGenesisState DecodeState(string canonicalBlockJson, int blockVersion)
    {
        if (!CanRead(blockVersion))
            throw new P1Refusal("SaveBlockVersionUnsupported",
                $"genesisRecipe block version {blockVersion} is not supported by this build");

        byte[] raw = Encoding.UTF8.GetBytes(canonicalBlockJson);
        var node = StrictJson.Parse(raw);
        var map = J.Keys(node, "blocks.genesisRecipe.data", CanonicalKeys);
        var obj = (JObj)node;
        for (int i = 0; i < CanonicalKeys.Length; i++)
        {
            if (!string.Equals(obj.Props[i].Key, CanonicalKeys[i], StringComparison.Ordinal))
                throw new P1Refusal("SaveNotCanonical", "blocks.genesisRecipe.data keys are not in canonical order");
        }

        ForestMakerRecipe recipe;
        try
        {
            recipe = ForestMakerRecipe.Decode(map["recipe"]);
        }
        catch (P1Refusal r) when (string.Equals(r.Code, "ForestMakerGeneratorUnsupported", StringComparison.Ordinal))
        {
            throw new P1Refusal("SaveRecipeUnsupported", r.Message);
        }

        string storedHash = J.Str(map["recipeHash"], "blocks.genesisRecipe.data.recipeHash");
        string computedHash = recipe.RecipeHash();
        if (!string.Equals(storedHash, computedHash, StringComparison.Ordinal))
            throw new P1Refusal("SaveRecipeHashMismatch",
                $"stored recipeHash '{storedHash}' does not match canonical recipe SHA-256 '{computedHash}'");

        string generatorNote = J.Str(map["generatorNote"], "blocks.genesisRecipe.data.generatorNote");
        if (!string.Equals(generatorNote, SaveV3Format.RecipeGeneratorNote, StringComparison.Ordinal))
            throw new P1Refusal("SaveRecipeUnsupported",
                $"unsupported genesisRecipe generatorNote '{generatorNote}'; expected '{SaveV3Format.RecipeGeneratorNote}'");

        bool economyEnabled = map["economyEnabled"] is JBool eb
            ? eb.Value
            : throw new P1Refusal("SchemaInvalid", "blocks.genesisRecipe.data.economyEnabled must be a boolean");

        var decoded = new ForestMakerGenesisState(recipe, storedHash, generatorNote, economyEnabled);
        string canonical = EncodeCanonical(decoded);
        if (!string.Equals(canonical, canonicalBlockJson, StringComparison.Ordinal))
            throw new P1Refusal("SaveNotCanonical", "blocks.genesisRecipe.data is not in canonical form");

        return decoded;
    }
}

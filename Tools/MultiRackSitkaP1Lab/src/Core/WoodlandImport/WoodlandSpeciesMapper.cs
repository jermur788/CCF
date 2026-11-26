namespace SitkaP1.Core.WoodlandImport;

/// <summary>
/// Deterministic, exact-match species mapper for Woodland Importer V1.
/// Performs case-insensitive matching on scientific names and documented common synonyms.
/// Never performs fuzzy or near-match substitution.
/// Bare genus names, genus-with-author forms (e.g. "Quercus L.", "Tilia L."), and higher taxonomic ranks
/// (e.g. "Fagaceae", "Pinaceae", "PLANTAE") are classified as <see cref="SpeciesSupportCategory.UnidentifiedSpecies"/>.
/// Regional calibration note (F-15): bare common names "Beech" and "Alder" map to Fagus sylvatica and
/// Alnus glutinosa as an explicit UK/Irish forestry calibration assumption (not a global taxonomic equivalence;
/// carries substitution risk if an external dataset uses bare "Alder" for Alnus incana).
/// </summary>
public static class WoodlandSpeciesMapper
{
    private sealed record KnownSpeciesEntry(
        string CanonicalId,
        SpeciesSupportCategory Category,
        string ScientificName);

    private static readonly Dictionary<string, KnownSpeciesEntry> ExactLookup = BuildLookup();

    private static readonly HashSet<string> UnidentifiedTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "plantae",
        "unknown",
        "unidentified",
        "indet",
        "indet.",
        "indeterminate",
        "n/a",
        "na",
        "none",
        "null",
        "sp",
        "sp.",
        "spp",
        "spp.",
        "tree",
        "broadleaf",
        "conifer",
        "tracheophyta",
        "magnoliopsida",
        "pinopsida",
        "angiospermae",
        "gymnospermae",
        "incertae sedis",
    };

    private static readonly string[] HigherRankSuffixes =
    [
        "aceae",
        "ales",
        "opsida",
        "phyta",
        "oideae",
        "ineae",
    ];

    private static readonly HashSet<string> KnownBinomialsWithAuthors = new(StringComparer.OrdinalIgnoreCase)
    {
        "picea sitchensis",
        "quercus petraea",
        "fagus sylvatica",
        "pinus sylvestris",
        "betula pubescens",
        "alnus glutinosa",
    };

    private static Dictionary<string, KnownSpeciesEntry> BuildLookup()
    {
        var map = new Dictionary<string, KnownSpeciesEntry>(StringComparer.OrdinalIgnoreCase);

        void Add(string canonicalId, SpeciesSupportCategory category, string scientificName, params string[] synonyms)
        {
            var entry = new KnownSpeciesEntry(canonicalId, category, scientificName);
            map[scientificName] = entry;
            foreach (string syn in synonyms)
            {
                map[syn] = entry;
            }
        }

        // ModelledAtBase (Scenario One / Phase 2 three-species set)
        Add("sitka-spruce", SpeciesSupportCategory.ModelledAtBase, "Picea sitchensis",
            "sitka spruce", "sitka-spruce");
        Add("sessile-oak", SpeciesSupportCategory.ModelledAtBase, "Quercus petraea",
            "sessile oak", "sessile-oak", "durmast oak", "quercus sessiliflora");
        // Regional UK/Irish calibration: bare "Beech" -> Fagus sylvatica (see F-15).
        Add("beech", SpeciesSupportCategory.ModelledAtBase, "Fagus sylvatica",
            "european beech", "common beech", "beech");

        // PendingModelSupport (Species Wave 2 target set)
        Add("scots-pine", SpeciesSupportCategory.PendingModelSupport, "Pinus sylvestris",
            "scots pine", "scots-pine", "scotch pine");
        Add("downy-birch", SpeciesSupportCategory.PendingModelSupport, "Betula pubescens",
            "downy birch", "downy-birch", "moor birch", "hairy birch");
        // Regional UK/Irish calibration: bare "Alder" -> Alnus glutinosa (see F-15).
        Add("common-alder", SpeciesSupportCategory.PendingModelSupport, "Alnus glutinosa",
            "common alder", "common-alder", "black alder", "european alder", "alder");

        return map;
    }

    public static SpeciesMappingResult Map(string? rawSpeciesName)
    {
        string raw = rawSpeciesName ?? string.Empty;
        string normalized = NormalizeWhitespace(raw);

        if (string.IsNullOrEmpty(normalized))
        {
            return Unidentified(raw, string.Empty);
        }

        // Direct exact match on full normalized string (scientific binomial or documented common synonym).
        if (ExactLookup.TryGetValue(normalized, out var direct))
        {
            return new SpeciesMappingResult(
                CanonicalSpeciesId: direct.CanonicalId,
                Category: direct.Category,
                RawName: raw,
                NormalizedInput: normalized,
                MatchedScientificName: direct.ScientificName);
        }

        // Reject known unidentified/placeholder/higher-rank tokens.
        if (UnidentifiedTokens.Contains(normalized))
        {
            return Unidentified(raw, normalized);
        }

        // Check for unparsable strings (no alphabetic letters at all, e.g., numeric codes or punctuation).
        if (!ContainsLetter(normalized) || !HasPlausibleTaxonCharacters(normalized))
        {
            return Unidentified(raw, normalized);
        }

        string[] tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Single-word input not in ExactLookup is a bare genus (e.g. "Quercus", "Fagus", "Betula"),
        // higher taxonomic rank (e.g. "Fagaceae", "Pinaceae"), or unrecognised single token -> UnidentifiedSpecies.
        if (tokens.Length == 1)
        {
            return Unidentified(raw, normalized);
        }

        // Higher taxonomic rank with optional authority (e.g. "Fagaceae Dumort.", "Pinaceae Spreng.").
        if (HasHigherRankSuffix(tokens[0]))
        {
            return Unidentified(raw, normalized);
        }

        // Genus-only unidentified forms like "Quercus sp.", "Betula spp.", "Pinus indet."
        if (IsUnidentifiedEpithet(tokens[1]))
        {
            return Unidentified(raw, normalized);
        }

        // Genus with taxonomic authority only (e.g. "Quercus L.", "Tilia L.", "Betula (L.)") -> UnidentifiedSpecies.
        if (LooksLikeGenusWithAuthorityOnly(tokens))
        {
            return Unidentified(raw, normalized);
        }

        // Scientific name with taxonomic authority suffix (e.g., "Fagus sylvatica L.",
        // "Quercus petraea (Matt.) Liebl.", "Picea sitchensis (Bong.) Carrière").
        // Hybrids ("x" / "×") are explicitly NOT matched to a parent species.
        if (tokens.Length >= 3 && !ContainsHybridMarker(tokens))
        {
            string binomial = tokens[0] + " " + tokens[1];
            if (KnownBinomialsWithAuthors.Contains(binomial) &&
                LooksLikeAuthorOrNominateInfraspecific(tokens, binomial) &&
                ExactLookup.TryGetValue(binomial, out var fromAuthor))
            {
                return new SpeciesMappingResult(
                    CanonicalSpeciesId: fromAuthor.CanonicalId,
                    Category: fromAuthor.Category,
                    RawName: raw,
                    NormalizedInput: normalized,
                    MatchedScientificName: fromAuthor.ScientificName);
            }
        }

        return new SpeciesMappingResult(
            CanonicalSpeciesId: null,
            Category: SpeciesSupportCategory.UnmodelledSpecies,
            RawName: raw,
            NormalizedInput: normalized,
            MatchedScientificName: null);
    }

    private static SpeciesMappingResult Unidentified(string raw, string normalized) =>
        new(
            CanonicalSpeciesId: null,
            Category: SpeciesSupportCategory.UnidentifiedSpecies,
            RawName: raw,
            NormalizedInput: normalized,
            MatchedScientificName: null);

    private static string NormalizeWhitespace(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var span = input.AsSpan().Trim();
        var chars = new char[span.Length];
        int length = 0;
        bool prevSpace = false;
        foreach (char c in span)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!prevSpace)
                {
                    chars[length++] = ' ';
                    prevSpace = true;
                }
            }
            else
            {
                chars[length++] = c;
                prevSpace = false;
            }
        }
        return new string(chars, 0, length);
    }

    private static bool ContainsLetter(string s)
    {
        foreach (char c in s)
        {
            if (char.IsLetter(c)) return true;
        }
        return false;
    }

    private static bool HasHigherRankSuffix(string token)
    {
        for (int i = 0; i < HigherRankSuffixes.Length; i++)
        {
            string suffix = HigherRankSuffixes[i];
            if (token.Length > suffix.Length + 1 &&
                token.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private static bool LooksLikeGenusWithAuthorityOnly(string[] tokens)
    {
        // E.g. "Quercus L.", "Tilia L.", "Fagus (L.)" where second token is an abbreviated or parenthesized authority,
        // never a lowercase specific epithet.
        if (tokens.Length < 2) return false;
        string second = tokens[1];
        if (second.Length == 0) return false;
        if (second[0] == '(') return true;
        if (second.EndsWith('.') && char.IsUpper(second[0])) return true;
        return false;
    }

    private static bool IsUnidentifiedEpithet(string token)
    {
        return token.Equals("sp", StringComparison.OrdinalIgnoreCase)
            || token.Equals("sp.", StringComparison.OrdinalIgnoreCase)
            || token.Equals("spp", StringComparison.OrdinalIgnoreCase)
            || token.Equals("spp.", StringComparison.OrdinalIgnoreCase)
            || token.Equals("indet", StringComparison.OrdinalIgnoreCase)
            || token.Equals("indet.", StringComparison.OrdinalIgnoreCase)
            || token.Equals("cf.", StringComparison.OrdinalIgnoreCase)
            || token.Equals("aff.", StringComparison.OrdinalIgnoreCase)
            || token.Equals("unknown", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsHybridMarker(string[] tokens)
    {
        foreach (string t in tokens)
        {
            if (t.Equals("x", StringComparison.OrdinalIgnoreCase) || t == "×")
            {
                return true;
            }
        }
        return false;
    }

    private static bool LooksLikeAuthorOrNominateInfraspecific(string[] tokens, string binomial)
    {
        // Standard botanical authority begins with an uppercase letter or opening parenthesis,
        // e.g. "L.", "(Matt.) Liebl.", "Ehrh.", "(Bong.) Carrière".
        string third = tokens[2];
        if (third.Length > 0 && (char.IsUpper(third[0]) || third[0] == '('))
        {
            return true;
        }

        // Allow nominate subspecies only (e.g., "Quercus petraea subsp. petraea").
        if (tokens.Length >= 4 &&
            (third.Equals("subsp.", StringComparison.OrdinalIgnoreCase) || third.Equals("ssp.", StringComparison.OrdinalIgnoreCase)))
        {
            string specificEpithet = binomial.Split(' ')[1];
            return tokens[3].Equals(specificEpithet, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static bool HasPlausibleTaxonCharacters(string s)
    {
        int letters = 0;
        foreach (char c in s)
        {
            if (char.IsLetter(c))
            {
                letters++;
                continue;
            }
            if (c is ' ' or '-' or '.' or '(' or ')' or '\'' or '&' or ',' or '×')
            {
                continue;
            }
            return false;
        }
        return letters >= 3;
    }
}

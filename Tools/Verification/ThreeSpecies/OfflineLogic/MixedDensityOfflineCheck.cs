// Offline logic check for the growth model 2 density routes (D-059, D-061).
// Compiles the PRODUCTION SitkaGrowthModel.cs and MixedDensityModel.cs unchanged
// (neither uses UnityEngine) together with this file, on the plain .NET SDK. No
// Unity Editor, no Library, nothing written inside the repository.
//
// What it proves:
//   - ROUTING: pure Sitka (and every stand under growth models 0 and 1) takes the original Sitka route;
//     any Oak/Beech under model 2 takes the occupancy route. Pure functions, exhaustive and random.
//   - LEGACY ROUTE UNCHANGED: SHA-256 fingerprints of the original Sitka code, taken from the accepted
//     base commit c1564b8, match the working tree: the offline stand-in for "bit-identical".
//   - NO SITKA ARITHMETIC IN THE OCCUPANCY ROUTE, AND NO OCCUPANCY IN SITKA: source guards here, and two
//     isolated compiles (the density-isolated suites): MixedDensityModel.cs without SitkaGrowthModel.cs and
//     SitkaGrowthModel.cs without MixedDensityModel.cs.
//   - NO BIOLOGICAL DENSITY CLAIM: production identifiers and strings never express a species maximum
//     density, maximum basal area or SDImax.
//   - BEHAVIOUR of the dimensionless occupancy model: demand order, additivity, a generic bounded mortality
//     response with a zero region, species-neutral victim choice, pure Oak, pure Beech and mixed fixtures.
//   - CALIBRATION: the occupancy normalization and loss scale are measured against recorded Scenario One
//     Sitka structures and recorded legacy mortality (game calibration, not ecological evidence).
//
// What it cannot prove: controller wiring at runtime, scene state or any Unity anchor hash. The Editor
// gate MixedDensityVerification.cs does that and has NOT been run in this environment.
//
// Usage: run_offline_check.py [--suite density]   (passes the repository root as the first argument)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class MixedDensityOfflineCheck
{
    private static int failures;
    private static int checks;

    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string F(double v, string f = "0.0000") => v.ToString(f, Inv);

    private const GrowingSpaceDemandClass Ref = GrowingSpaceDemandClass.Reference;
    private const GrowingSpaceDemandClass Mid = GrowingSpaceDemandClass.Intermediate;
    private const GrowingSpaceDemandClass Hi = GrowingSpaceDemandClass.High;
    private const float Area = 0.64f;   // Enlarged80 property, ha

    // Fingerprints of the ORIGINAL Sitka code at the accepted base commit c1564b8 (growth model 1).
    private const string SitkaModelClassSha256 = "1a81640dcd11cac892a51bde6e28115b84465e08fd20c5d4b31859337964d032";
    private const string LegacyDecisionBlockSha256 = "ee7dc266441de752d3c9e322faee493b2484d3e7d0b23206c167b238dae45fa8";
    private const string StandRelativeDensitySha256 = "80ef453c02a4298dbde5d41a7508016e0008bd0a55d07855c20b5276122e6448";

    public static int Main(string[] args)
    {
        string repo = args.Length > 0 ? args[0] : ".";
        Weights();
        GrowthModelVersions();
        AssetClasses(repo);
        Routing();
        LegacyRouteIsUnchanged(repo);
        NoBiologicalDensityClaim(repo);
        OccupancyAbstraction();
        DemandOrder();
        GenericMortality();
        NormalizationOnlyMovesTheMixedRoute();
        NoSpeciesInTheMortalityFormula(repo);
        PureBroadleafDense();
        MixedStands();
        Thresholds();
        VictimSelection();
        Calibration(repo);
        Determinism();
        Degenerate();
        Console.WriteLine(failures == 0 ? $"THREE_SPECIES_OFFLINE_PASS checks={checks}" : $"THREE_SPECIES_OFFLINE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // ---------- fixtures ----------

    private static DensityStem Stem(string id, GrowingSpaceDemandClass demand, double dbh, double ci)
        => new DensityStem(id, demand, (float)dbh, ci);

    // Equal-sized, equally competed stems of one class.
    private static List<DensityStem> Uniform(GrowingSpaceDemandClass demand, int count, float dbh, double ci, string prefix)
    {
        var stems = new List<DensityStem>(count);
        for (int i = 0; i < count; i++)
            stems.Add(Stem(prefix + i.ToString("D5", Inv), demand, dbh, ci));
        return stems;
    }

    // Random sizes and competition, sorted by tree id as FindTrees() presents them to the controller.
    private static List<DensityStem> RandomStand(Random random, int count, double dbhMin, double dbhMax, Func<int, GrowingSpaceDemandClass> classOf, string prefix = "P")
    {
        var stems = new List<DensityStem>(count);
        for (int i = 0; i < count; i++)
            stems.Add(Stem(prefix + i.ToString("D5", Inv), classOf(i), dbhMin + random.NextDouble() * (dbhMax - dbhMin), random.NextDouble() * 8.0));
        return stems.OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
    }

    private static GrowingSpaceDemandClass ThreeWay(int i) => i % 3 == 0 ? Ref : i % 3 == 1 ? Mid : Hi;

    // A per-tree uniform draw that is a pure function of (id, seed): no stream, no call order.
    private static Func<string, double> KeyedRoll(int seed)
    {
        return id =>
        {
            unchecked
            {
                ulong h = 14695981039346656037UL;
                foreach (char c in id) { h ^= c; h *= 1099511628211UL; }
                h ^= (ulong)(uint)seed * 0x9E3779B97F4A7C15UL;
                h ^= h >> 30; h *= 0xBF58476D1CE4E5B9UL; h ^= h >> 27; h *= 0x94D049BB133111EBUL; h ^= h >> 31;
                return (h >> 11) / 9007199254740992.0;
            }
        };
    }

    private static double Occ(IReadOnlyList<DensityStem> stems) => MixedDensityModel.Occupancy(stems, Area);

    private static string Fingerprint(DensityMortalityPlan p)
        => $"{BitConverter.DoubleToInt64Bits(p.Occupancy):X16}|{p.HazardDeaths}|{string.Join(",", p.VictimIndices)}";

    private static OccupancyCalibration Default => OccupancyCalibration.Default;

    // ---------- contracts ----------

    private static void Weights()
    {
        double r = MixedDensityModel.Weight(Ref), i = MixedDensityModel.Weight(Mid), h = MixedDensityModel.Weight(Hi);
        Check(r == 1.0 && i == 1.5 && h == 2.0, "WEIGHTS_ARE_THE_PROVISIONAL_CLASSES", $"{F(r)} {F(i)} {F(h)}");
        Check(r < i && i < h, "WEIGHT_ORDINAL_ORDER");
        bool threw = false;
        try { MixedDensityModel.Weight((GrowingSpaceDemandClass)99); } catch (ArgumentOutOfRangeException) { threw = true; }
        Check(threw, "WEIGHT_UNKNOWN_CLASS_FAILS_LOUD");
        Check(Enum.GetValues(typeof(GrowingSpaceDemandClass)).Length == 3, "WEIGHT_THREE_CLASSES_ONLY");
        Check((int)Ref == 0 && (int)Mid == 1 && (int)Hi == 2, "CLASS_VALUES_MATCH_SERIALIZED_ASSETS");
    }

    private static void GrowthModelVersions()
    {
        Check(GrowthModel.Legacy == 0 && GrowthModel.SiteClassDensity == 1 && GrowthModel.SpeciesAware == 2 && GrowthModel.Latest == 2,
            "GROWTH_MODEL_VERSION_VALUES", $"legacy={GrowthModel.Legacy} siteClass={GrowthModel.SiteClassDensity} speciesAware={GrowthModel.SpeciesAware} latest={GrowthModel.Latest}");
        Check(GrowthModel.Normalize(-3) == 0 && GrowthModel.Normalize(0) == 0 && GrowthModel.Normalize(1) == 1
            && GrowthModel.Normalize(2) == 2 && GrowthModel.Normalize(7) == 2, "GROWTH_MODEL_NORMALIZE");
    }

    // The serialized species assets: Sitka has no field (default = Reference), Oak and Beech are explicitly
    // classified, and no species other than Sitka is Reference (a Reference species would get the Sitka route).
    private static void AssetClasses(string repo)
    {
        string dir = Path.Combine(repo, "Assets", "ForestPrototype", "Species");
        if (!Directory.Exists(dir))
        {
            Check(false, "ASSET_DIRECTORY_FOUND", dir);
            return;
        }
        int? Read(string file)
        {
            foreach (string line in File.ReadAllLines(Path.Combine(dir, file)))
                if (line.StartsWith("  growingSpaceDemand:", StringComparison.Ordinal))
                    return int.Parse(line.Substring("  growingSpaceDemand:".Length).Trim(), Inv);
            return null;
        }
        int? sitka = Read("SitkaSpruce.asset"), oak = Read("SessileOak.asset"), beech = Read("BeechSpecies.asset");
        Check(sitka == null, "ASSET_SITKA_UNCHANGED_DEFAULT_REFERENCE", "field=" + (sitka?.ToString() ?? "absent"));
        Check(oak == (int)Hi, "ASSET_OAK_HIGH", "field=" + (oak?.ToString() ?? "absent"));
        Check(beech == (int)Mid, "ASSET_BEECH_INTERMEDIATE", "field=" + (beech?.ToString() ?? "absent"));
        string[] referenceSpecies = Directory.GetFiles(dir, "*.asset")
            .Where(f => (Read(Path.GetFileName(f)) ?? 0) == (int)Ref).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Check(referenceSpecies.Length == 1 && referenceSpecies[0] == "SitkaSpruce.asset", "ONLY_SITKA_IS_REFERENCE_CLASS", string.Join(",", referenceSpecies));
    }

    // ---------- routing ----------

    private static void Routing()
    {
        // Every combination of 1-6 Reference-only stems under model 2 takes the legacy route.
        var all = new List<DensityStem>();
        bool pureSitkaLegacy = true;
        for (int n = 1; n <= 6; n++)
        {
            all.Add(Stem("S" + n, Ref, 20 + n, 1.0));
            pureSitkaLegacy &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(all)) == DensityRoute.LegacySitkaExact;
        }
        var thousand = Uniform(Ref, 1000, 30f, 2.0, "S");
        pureSitkaLegacy &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(thousand)) == DensityRoute.LegacySitkaExact;
        Check(pureSitkaLegacy, "ROUTE_PURE_SITKA_IS_LEGACY_SITKA_EXACT");

        // Any stand containing a non-Reference stem under model 2 takes the occupancy route, wherever the stem sits.
        var random = new Random(61);
        int trials = 0, aware = 0;
        for (int t = 0; t < 400; t++)
        {
            int n = 1 + random.Next(60);
            var stems = Uniform(Ref, n, 25f, 1.0, "S").ToList();
            int at = random.Next(n);
            stems[at] = Stem(stems[at].TreeId, random.Next(2) == 0 ? Mid : Hi, 25, 1.0);
            trials++;
            if (MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(stems)) == DensityRoute.SpeciesAwareOccupancy) aware++;
        }
        Check(aware == trials, "ROUTE_ANY_OAK_OR_BEECH_IS_SPECIES_AWARE_OCCUPANCY", $"trials={trials} speciesAware={aware}");

        // Models 0 and 1 never leave the Sitka route, whatever the stand holds.
        Check(MixedDensityModel.SelectRoute(false, true) == DensityRoute.LegacySitkaExact
            && MixedDensityModel.SelectRoute(false, false) == DensityRoute.LegacySitkaExact, "ROUTE_MODELS_BELOW_2_ALWAYS_LEGACY");

        // One Oak flips a thousand-Sitka stand, and removing it flips it back.
        var mixed = Uniform(Ref, 1000, 30f, 2.0, "S").Concat(new[] { Stem("T", Hi, 30, 2.0) }).ToList();
        bool flips = MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(mixed)) == DensityRoute.SpeciesAwareOccupancy
            && MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(mixed.Take(1000).ToList())) == DensityRoute.LegacySitkaExact;
        Check(flips, "ROUTE_ONE_OAK_FLIPS_AND_REMOVAL_FLIPS_BACK");
    }

    // ---------- source helpers ----------

    private static string ReadSource(string repo, params string[] parts)
        => File.ReadAllText(Path.Combine(new[] { repo }.Concat(parts).ToArray())).Replace("\r\n", "\n");

    private static string Sha256(string text)
    {
        using (SHA256 sha = SHA256.Create())
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2", Inv)));
    }

    private static string Between(string text, string start, string end)
    {
        int i = text.IndexOf(start, StringComparison.Ordinal);
        if (i < 0) return null;
        int j = text.IndexOf(end, i, StringComparison.Ordinal);
        return j < 0 ? null : text.Substring(i, j - i);
    }

    // Identifiers and string-literal contents of C# source with comments removed. A small lexer, enough for this code base:
    // line and block comments, ordinary, verbatim and interpolated strings, and char literals.
    private static void Lex(string text, List<string> identifiers, List<string> strings, StringBuilder code)
    {
        int i = 0, n = text.Length;
        while (i < n)
        {
            char c = text[i];
            if (c == '/' && i + 1 < n && text[i + 1] == '/') { while (i < n && text[i] != '\n') i++; continue; }
            if (c == '/' && i + 1 < n && text[i + 1] == '*') { i += 2; while (i + 1 < n && !(text[i] == '*' && text[i + 1] == '/')) i++; i += 2; continue; }
            if (c == '@' && i + 1 < n && text[i + 1] == '"' || c == '$' && i + 2 < n && text[i + 1] == '@' && text[i + 2] == '"' || c == '@' && i + 2 < n && text[i + 1] == '$' && text[i + 2] == '"')
            {
                i = text.IndexOf('"', i) + 1;
                var sb = new StringBuilder();
                while (i < n) { if (text[i] == '"') { if (i + 1 < n && text[i + 1] == '"') { sb.Append('"'); i += 2; continue; } break; } sb.Append(text[i++]); }
                i++; strings.Add(sb.ToString()); code.Append("\"\""); continue;
            }
            if (c == '"' || c == '$' && i + 1 < n && text[i + 1] == '"')
            {
                if (c == '$') i++;
                i++;
                var sb = new StringBuilder();
                while (i < n && text[i] != '"') { if (text[i] == '\\' && i + 1 < n) { sb.Append(text[i + 1]); i += 2; continue; } sb.Append(text[i++]); }
                i++; strings.Add(sb.ToString()); code.Append("\"\""); continue;
            }
            if (c == '\'') { i++; while (i < n && text[i] != '\'') { if (text[i] == '\\') i++; i++; } i++; code.Append("''"); continue; }
            if (char.IsLetter(c) || c == '_')
            {
                int s = i;
                while (i < n && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
                string id = text.Substring(s, i - s);
                identifiers.Add(id); code.Append(id); continue;
            }
            code.Append(c); i++;
        }
    }

    private static string StripComments(string text)
    {
        var code = new StringBuilder();
        Lex(text, new List<string>(), new List<string>(), code);
        return code.ToString();
    }

    // Source text of a method from its signature to the matching closing brace.
    private static string MethodSource(string text, string signature)
    {
        int start = text.IndexOf(signature, StringComparison.Ordinal);
        if (start < 0) return null;
        int open = text.IndexOf('{', start);
        int arrow = text.IndexOf("=>", start, StringComparison.Ordinal);
        if (arrow >= 0 && (open < 0 || arrow < open)) return text.Substring(start, text.IndexOf(';', arrow) - start);
        int depth = 0, i = open;
        for (; i < text.Length; i++) { if (text[i] == '{') depth++; else if (text[i] == '}' && --depth == 0) break; }
        return text.Substring(start, i - start + 1);
    }

    // ---------- the legacy route is the same bytes as the accepted base ----------

    private static void LegacyRouteIsUnchanged(string repo)
    {
        string sitka, controller, model;
        try
        {
            sitka = ReadSource(repo, "Assets", "ForestPrototype", "SitkaGrowthModel.cs");
            controller = ReadSource(repo, "Assets", "ForestPrototype", "ForestEcologyController.cs");
            model = ReadSource(repo, "Assets", "ForestPrototype", "MixedDensityModel.cs");
        }
        catch (IOException ex)
        {
            Check(false, "SOURCES_FOUND", ex.Message);
            return;
        }

        int classAt = sitka.IndexOf("public static class SitkaGrowthModel", StringComparison.Ordinal);
        string sitkaClass = classAt >= 0 ? sitka.Substring(classAt) : "";
        Check(Sha256(sitkaClass) == SitkaModelClassSha256, "LEGACY_SITKA_ARITHMETIC_UNCHANGED_FROM_BASE_C1564B8", Sha256(sitkaClass).Substring(0, 16));

        string block = Between(controller, "account.RelativeDensity = StandRelativeDensity(living", "        KillDensityVictims(victims, account);");
        Check(block != null && Sha256(block) == LegacyDecisionBlockSha256, "LEGACY_CONTROLLER_DECISION_BLOCK_UNCHANGED_FROM_BASE_C1564B8",
            block == null ? "block not found" : Sha256(block).Substring(0, 16));
        string helper = Between(controller, "private double StandRelativeDensity(", "    // Dynamic crown radius");
        Check(helper != null && Sha256(helper) == StandRelativeDensitySha256, "LEGACY_STAND_RELATIVE_DENSITY_UNCHANGED_FROM_BASE_C1564B8",
            helper == null ? "helper not found" : Sha256(helper).Substring(0, 16));

        // Wiring, in source order: route chosen by the pure function, occupancy route returns before the legacy
        // statements, legacy route labelled, then the unchanged legacy block.
        int select = controller.IndexOf("MixedDensityModel.SelectRoute(UsesSpeciesAwareGrowth, anyNonReference)", StringComparison.Ordinal);
        int aware = controller.IndexOf("ApplySpeciesAwareDensityMortality(living, account);", StringComparison.Ordinal);
        int label = controller.IndexOf("account.Route = DensityRoute.LegacySitkaExact;", StringComparison.Ordinal);
        int legacy = controller.IndexOf("account.RelativeDensity = StandRelativeDensity(living", StringComparison.Ordinal);
        Check(select > 0 && select < aware && aware < label && label < legacy, "LEGACY_ROUTE_WIRING_ORDER", $"select={select} aware={aware} label={label} legacy={legacy}");
        Check(controller.Contains("private bool UsesSpeciesAwareGrowth => growthModelVersion >= GrowthModel.SpeciesAware;")
            && controller.Contains("bool anyNonReference = UsesSpeciesAwareGrowth && HasNonReferenceDemand(living);"),
            "SPECIES_AWARE_ROUTE_IS_GATED_ON_GROWTH_MODEL_2");

        // The occupancy route's controller method and model must not touch any Sitka density arithmetic, and the
        // Sitka route must not touch the occupancy model: changing one cannot change the other.
        string method = Between(controller, "private void ApplySpeciesAwareDensityMortality(", "    private double StandRelativeDensity(");
        string methodCode = method == null ? null : StripComments(method);
        Check(methodCode != null && !methodCode.Contains("SitkaGrowthModel") && !methodCode.Contains("StandRelativeDensity") && !methodCode.Contains("RelativeDensity")
            && methodCode.Contains("MixedDensityModel.PlanComposite(")
            && methodCode.Contains("SitkaSubsetDensitySignal.RequiredDeaths("), "OCCUPANCY_ROUTE_CONTROLLER_METHOD_HAS_NO_SITKA_ARITHMETIC");
        string modelCode = StripComments(model);
        Check(!modelCode.Contains("SitkaGrowthModel") && !modelCode.Contains("RelativeDensity") && !modelCode.Contains("MaximumSdi") && !modelCode.Contains("2.063"),
            "MIXED_DENSITY_MODEL_SOURCE_HAS_NO_SITKA_REFERENCE");
        string sitkaCode = StripComments(sitkaClass);
        string blockCode = block == null ? "" : StripComments(block);
        Check(!sitkaCode.Contains("MixedDensityModel") && !sitkaCode.Contains("Occupancy") && !blockCode.Contains("MixedDensityModel") && !blockCode.Contains("Occupancy")
            && !blockCode.Contains("OccupancyCalibration"), "SITKA_ROUTE_AND_SITKA_MODEL_HAVE_NO_REFERENCE_TO_OCCUPANCY_MODEL");
    }

    // ---------- TEST A: no biological maximum-density claim ----------

    // Identifiers and string literals in PRODUCTION code must never express a species maximum density, maximum basal
    // area or SDImax. SitkaGrowthModel.cs is exempt: it holds the Sitka-only British line and says so.
    private static void NoBiologicalDensityClaim(string repo)
    {
        string root = Path.Combine(repo, "Assets", "ForestPrototype");
        string[] speciesWords = { "oak", "beech", "broadleaf", "sessile", "species" };
        string[] limitWords = { "max", "maximum", "capacity", "ceiling", "carrying" };
        string[] densityWords = { "density", "basal", "ba", "sdi", "stocking", "stems" };
        string[] densityFiles = { "MixedDensityModel.cs", "AdultMortalityAccount.cs" };
        // Identifiers are judged by their camel-case WORDS, so "PlantingSpeciesDisplayName" does not contain "sdi".
        List<string> Words(string id) => Regex.Matches(id, "[A-Z]+(?![a-z])|[A-Z]?[a-z]+|[0-9]+").Cast<Match>().Select(m => m.Value.ToLowerInvariant()).ToList();
        // One pre-existing tooltip (present at base c1564b8) about pulling up a REGENERATION cohort at "its species maximum
        // density" (regenDensityMax). It is juvenile cohort density, not adult stand density, and unrelated to this work.
        const string allowedRegenerationTooltip = "[D] Seconds required to pull up a cohort at its species maximum density.";

        var violations = new List<string>();
        int files = 0, identifiersSeen = 0, stringsSeen = 0;
        foreach (string file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            string name = Path.GetFileName(file);
            if (name == "SitkaGrowthModel.cs") continue;
            files++;
            var identifiers = new List<string>(); var strings = new List<string>();
            Lex(File.ReadAllText(file).Replace("\r\n", "\n"), identifiers, strings, new StringBuilder());
            identifiersSeen += identifiers.Count; stringsSeen += strings.Count;
            foreach (string id in identifiers.Distinct())
            {
                List<string> words = Words(id);
                bool species = words.Any(speciesWords.Contains), limit = words.Any(limitWords.Contains), density = words.Any(densityWords.Contains);
                // A species-qualified maximum of density, basal area or SDI; or any maximum basal area / SDI outside the Sitka-only model.
                if (species && limit && density) violations.Add($"{name}: identifier {id}");
                if (words.Any(w => w == "max" || w == "maximum") && words.Any(w => w == "basal" || w == "sdi")) violations.Add($"{name}: identifier {id}");
                if (densityFiles.Contains(name) && (words.Contains("capacity") || words.Contains("sdi") || words.Contains("basal") || Regex.IsMatch(id.ToLowerInvariant(), "maxdensity|maximumdensity|m2ha|m2perha")))
                    violations.Add($"{name}: identifier {id}");
            }
            foreach (string text in strings)
            {
                if (text == allowedRegenerationTooltip) continue;
                string lower = text.ToLowerInvariant();
                bool species = speciesWords.Any(lower.Contains);
                bool limit = Regex.IsMatch(lower, @"maximum density|max density|max\.? basal|maximum basal|basal area (max|ceiling|limit)|sdi ?max|sdimax|density (limit|ceiling|capacity)|carrying capacity|m²/ha|m2/ha");
                if (species && limit) violations.Add($"{name}: string \"{(text.Length > 60 ? text.Substring(0, 60) : text)}\"");
            }
        }
        Check(violations.Count == 0, "NO_PRODUCTION_IDENTIFIER_OR_STRING_EXPRESSES_A_SPECIES_MAXIMUM_DENSITY_OR_MAX_BASAL_AREA",
            violations.Count == 0 ? $"files={files} identifiers={identifiersSeen} strings={stringsSeen} (one pre-existing regeneration tooltip allowlisted)" : string.Join("; ", violations.Take(8)));

        // The scanner really sees code: it finds known identifiers and a known string literal in the density model.
        var probeIdentifiers = new List<string>(); var probeStrings = new List<string>();
        Lex(File.ReadAllText(Path.Combine(root, "MixedDensityModel.cs")).Replace("\r\n", "\n"), probeIdentifiers, probeStrings, new StringBuilder());
        Check(probeIdentifiers.Contains("OccupancyNormalization") && probeIdentifiers.Contains("MaximumAnnualLoss") && !probeIdentifiers.Contains("SpeciesAwareOccupancyXYZ")
            && probeStrings.Contains("unknown growing-space demand class") && !probeStrings.Any(t => t.Contains("WHAT THIS IS")), "SOURCE_SCANNER_SEES_CODE_AND_SKIPS_COMMENTS",
            $"identifiers={probeIdentifiers.Count} strings={probeStrings.Count}");

        // The constants that remain are named for what they are.
        string[] names = typeof(MixedDensityModel).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).Select(f => f.Name).ToArray();
        Check(names.Contains("OccupancyNormalization") && names.Contains("MaximumAnnualLoss") && names.Contains("OnsetOccupancy")
            && !names.Any(nm => Regex.IsMatch(nm.ToLowerInvariant(), "capacity|basal|sdi|m2")), "CALIBRATION_CONSTANTS_ARE_NAMED_AS_NORMALIZATION_AND_LOSS_NOT_DENSITY_LIMITS", string.Join(",", names));

        // Documentation must not state species capacities either.
        string doc = ReadSource(repo, "Docs", "Verification", "ThreeSpeciesProgramme", "Phase1-DensityRework.md");
        string[] banned = { "45 m²", "60 m²", "45 m2", "60 m2", "90 m²", "90 m2", "Oak's capacity", "Beech's capacity", "capacity of 90" };
        Check(banned.All(b => !doc.Contains(b)), "DOCUMENTATION_STATES_NO_SPECIES_CAPACITY", string.Join(",", banned.Where(doc.Contains)));
    }

    // ---------- the occupancy abstraction ----------

    private static void OccupancyAbstraction()
    {
        // Plain geometry: cross-section, exponent exactly 2 (not a fitted 2.063).
        bool geometry = true;
        foreach (double dbh in new[] { 4.0, 10.0, 25.0, 47.5, 90.0 })
        {
            double expected = Math.PI / 4.0 * (dbh / 100.0) * (dbh / 100.0);
            double got = MixedDensityModel.StemOccupiedSpace(Stem("a", Ref, dbh, 0));
            double doubled = MixedDensityModel.StemOccupiedSpace(Stem("a", Ref, dbh * 2.0, 0));
            geometry &= Math.Abs(got - expected) < 1e-15 && Math.Abs(doubled / got - 4.0) < 1e-6;
        }
        Check(geometry, "OCCUPANCY_IS_STEM_CROSS_SECTION_GEOMETRY_EXPONENT_TWO");

        // No mixture bonus: occupancy is a plain sum over stems, so any split of a stand adds back up.
        var random = new Random(2026);
        double worst = 0.0;
        for (int t = 0; t < 200; t++)
        {
            List<DensityStem> stems = RandomStand(random, 100 + random.Next(500), 5.0, 70.0, ThreeWay);
            double whole = Occ(stems);
            double parts = Occ(stems.Where(s => s.Demand == Ref).ToList()) + Occ(stems.Where(s => s.Demand == Mid).ToList()) + Occ(stems.Where(s => s.Demand == Hi).ToList());
            worst = Math.Max(worst, Math.Abs(whole - parts) / Math.Max(whole, 1e-12));
        }
        Check(worst < 1e-12, "OCCUPANCY_IS_ADDITIVE_NO_MIXTURE_BONUS_NO_DEC_NO_DMC", $"worstRelativeDifference={worst:E2}");

        var one = Uniform(Hi, 300, 31f, 1.0, "O");
        double a = MixedDensityModel.Occupancy(one, 0.64f), b = MixedDensityModel.Occupancy(one, 1.28f);
        Check(Math.Abs(a / b - 2.0) < 1e-12, "OCCUPANCY_SCALES_INVERSELY_WITH_AREA", $"{F(a)} vs {F(b)}");
        var shuffled = one.OrderByDescending(s => s.TreeId, StringComparer.Ordinal).ToList();
        Check(BitConverter.DoubleToInt64Bits(MixedDensityModel.Occupancy(one, Area)) == BitConverter.DoubleToInt64Bits(MixedDensityModel.Occupancy(shuffled, Area)),
            "OCCUPANCY_INDEPENDENT_OF_LISTING_ORDER_BIT_FOR_BIT");

        // The abstraction is not the old identity "weighted density = weight x Sitka relative density".
        bool notSitkaMultiple = true;
        foreach (float dbh in new[] { 28f, 60f })
        {
            double sitkaRd = SitkaGrowthModel.RelativeDensity(400 / Area, dbh);
            double oakOccupancy = Occ(Uniform(Hi, 400, dbh, 1.0, "O"));
            notSitkaMultiple &= Math.Abs(oakOccupancy / sitkaRd - 2.0) > 1e-3;
        }
        Check(notSitkaMultiple, "OAK_OCCUPANCY_IS_NOT_WEIGHT_TIMES_SITKA_RELATIVE_DENSITY");
    }

    // ---------- TEST B: demand order, dimensionless occupancy ----------

    private static void DemandOrder()
    {
        bool ordered = true, ratios = true;
        foreach (double dbh in new[] { 3.0, 8.0, 15.0, 28.0, 45.0, 70.0, 110.0 })
        {
            double r = MixedDensityModel.StemOccupiedSpace(Stem("a", Ref, dbh, 1.0));
            double m = MixedDensityModel.StemOccupiedSpace(Stem("a", Mid, dbh, 1.0));
            double h = MixedDensityModel.StemOccupiedSpace(Stem("a", Hi, dbh, 1.0));
            ordered &= r < m && m < h;
            ratios &= Math.Abs(m / r - 1.5) < 1e-12 && Math.Abs(h / r - 2.0) < 1e-12;
        }
        Check(ordered, "DEMAND_ORDER_HIGHER_CLASS_GREATER_CONTRIBUTION_AT_EQUAL_SIZE");
        Check(ratios, "DEMAND_ORDER_CONTRIBUTIONS_FOLLOW_THE_PROVISIONAL_WEIGHTS");

        // Same stems, same size and competition, three classes: dimensionless occupancy and pressure follow Reference < Beech < Oak.
        double oS = Occ(Uniform(Ref, 400, 28f, 1.0, "S")), oB = Occ(Uniform(Mid, 400, 28f, 1.0, "B")), oO = Occ(Uniform(Hi, 400, 28f, 1.0, "O"));
        double pS = MixedDensityModel.OvercrowdingPressure(oS), pB = MixedDensityModel.OvercrowdingPressure(oB), pO = MixedDensityModel.OvercrowdingPressure(oO);
        Check(oS < oB && oB < oO && pS == 0.0 && pB > 0.0 && pO > pB, "DEMAND_ORDER_RAISES_OCCUPANCY_AND_PRESSURE_FOR_SAME_STEMS",
            $"occupancy reference={F(oS)} beech={F(oB)} oak={F(oO)} pressure reference={F(pS)} beech={F(pB)} oak={F(pO)}");

        // Monotone over a size sweep as well: at every size the order holds, and occupancy is strictly increasing with size.
        bool everywhere = true, growing = true; double previous = 0.0;
        foreach (float dbh in new[] { 10f, 14f, 18f, 24f, 30f, 38f, 48f, 60f })
        {
            double r = Occ(Uniform(Ref, 300, dbh, 1.0, "S")), m = Occ(Uniform(Mid, 300, dbh, 1.0, "B")), h = Occ(Uniform(Hi, 300, dbh, 1.0, "O"));
            everywhere &= r < m && m < h;
            growing &= r > previous;
            previous = r;
        }
        Check(everywhere && growing, "DEMAND_ORDER_HOLDS_AT_EVERY_SIZE_AND_OCCUPANCY_RISES_WITH_SIZE");
    }

    // ---------- TEST D: the generic mortality response ----------

    private static void GenericMortality()
    {
        // Zero region: nothing at or below the onset.
        bool zero = true;
        foreach (double o in new[] { 0.0, 0.01, 0.3, 0.7, 0.999999, MixedDensityModel.OnsetOccupancy })
            zero &= MixedDensityModel.OvercrowdingPressure(o) == 0.0 && MixedDensityModel.ExpectedAnnualLoss(MixedDensityModel.OvercrowdingPressure(o), Default) == 0.0;
        Check(zero, "MORTALITY_ZERO_REGION_AT_AND_BELOW_ONSET");

        // Pressure is the share of the occupancy beyond the onset; loss is the loss scale times that share.
        bool definition = true;
        foreach (double o in new[] { 1.0001, 1.1, 1.5, 2.0, 3.0, 10.0 })
        {
            double p = MixedDensityModel.OvercrowdingPressure(o);
            definition &= Math.Abs(p - (o - 1.0) / o) < 1e-15 && Math.Abs(MixedDensityModel.ExpectedAnnualLoss(p, Default) - MixedDensityModel.MaximumAnnualLoss * p) < 1e-15;
        }
        Check(definition, "PRESSURE_IS_THE_SHARE_OF_OCCUPANCY_BEYOND_THE_ONSET_AND_LOSS_IS_SCALE_TIMES_PRESSURE");
        Check(Math.Abs(MixedDensityModel.ExpectedAnnualLoss(MixedDensityModel.OvercrowdingPressure(2.0), Default) - MixedDensityModel.MaximumAnnualLoss / 2.0) < 1e-15,
            "LOSS_AT_TWICE_THE_ONSET_IS_HALF_THE_MAXIMUM");

        // Monotone region: strictly increasing in occupancy all the way up.
        double previousPressure = 0.0, previousLoss = 0.0; bool monotone = true;
        for (double o = 1.0005; o < 1000.0; o *= 1.07)
        {
            double p = MixedDensityModel.OvercrowdingPressure(o), l = MixedDensityModel.ExpectedAnnualLoss(p, Default);
            monotone &= p > previousPressure && l > previousLoss;
            previousPressure = p; previousLoss = l;
        }
        Check(monotone, "MORTALITY_MONOTONE_ABOVE_ONSET");

        // Bounded: pressure below 1, loss below the maximum however crowded; a stand never expects to lose more than the maximum.
        bool bounded = true;
        foreach (double o in new[] { 1.5, 5.0, 100.0, 1e6, 1e12 })
        {
            double p = MixedDensityModel.OvercrowdingPressure(o);
            bounded &= p < 1.0 && MixedDensityModel.ExpectedAnnualLoss(p, Default) <= MixedDensityModel.MaximumAnnualLoss && MixedDensityModel.ExpectedAnnualLoss(1e9, Default) <= MixedDensityModel.MaximumAnnualLoss;
        }
        var random = new Random(99);
        double worstShare = 0.0; bool probabilitiesInRange = true;
        for (int t = 0; t < 60; t++)
        {
            List<DensityStem> stems = RandomStand(random, 200 + random.Next(900), 8.0, 70.0, ThreeWay);
            double o = Occ(stems), loss = MixedDensityModel.ExpectedAnnualLoss(MixedDensityModel.OvercrowdingPressure(o), Default);
            double[] suppression = stems.Select(s => MixedDensityModel.Suppression(s.CompetitionIndex)).ToArray();
            double mean = suppression.Average(MixedDensityModel.Vulnerability);
            double[] probability = suppression.Select(s => MixedDensityModel.AnnualLossProbability(loss, s, mean)).ToArray();
            probabilitiesInRange &= probability.All(p => p >= 0.0 && p <= 1.0);
            worstShare = Math.Max(worstShare, probability.Sum() / stems.Count);
        }
        Check(bounded && probabilitiesInRange && worstShare <= MixedDensityModel.MaximumAnnualLoss + 1e-12, "MORTALITY_BOUNDED_PER_STEM_AND_PER_STAND",
            $"largestExpectedShareOfStemsLost={F(worstShare, "0.00000")} maximum={F(MixedDensityModel.MaximumAnnualLoss, "0.00")}");

        // Allocation does not change HOW MANY die: with no stem capped, expected deaths equal the stand's expected loss x stems.
        List<DensityStem> crowded = RandomStand(new Random(43), 900, 20.0, 40.0, ThreeWay);
        DensityMortalityPlan reference = MixedDensityModel.Plan(crowded, Area, KeyedRoll(0));
        double[] sup = crowded.Select(s => MixedDensityModel.Suppression(s.CompetitionIndex)).ToArray();
        double meanV = sup.Average(MixedDensityModel.Vulnerability);
        double maxProbability = sup.Max(s => MixedDensityModel.AnnualLossProbability(reference.ExpectedAnnualLoss, s, meanV));
        long deaths = 0; const int years = 400;
        for (int seed = 1; seed <= years; seed++) deaths += MixedDensityModel.Plan(crowded, Area, KeyedRoll(seed)).VictimIndices.Count;
        double expected = reference.ExpectedAnnualLoss * crowded.Count, observed = deaths / (double)years;
        double sigma = Math.Sqrt(expected / years);
        Check(maxProbability < 1.0 && Math.Abs(observed - expected) < 5.0 * sigma, "EXPECTED_DEATHS_EQUAL_STAND_EXPECTED_LOSS_TIMES_STEMS",
            $"occupancy={F(reference.Occupancy)} pressure={F(reference.OvercrowdingPressure)} expectedLoss={F(reference.ExpectedAnnualLoss, "0.0000")} expectedDeaths={F(expected, "0.0")} observedMean={F(observed, "0.0")} ({F((observed - expected) / sigma, "0.0")} sigma)");
    }

    // ---------- TEST C: changing the normalization moves only the mixed route ----------

    private static void NoSpeciesInTheMortalityFormula(string repo)
    {
        // TEST E. Reflection: no pressure, loss, suppression or probability function takes a class, a stem or a string.
        string[] formulaMethods = { "OvercrowdingPressure", "ExpectedAnnualLoss", "Suppression", "Vulnerability", "AnnualLossProbability" };
        bool onlyNumbers = true; var seen = new List<string>();
        foreach (string name in formulaMethods)
            foreach (MethodInfo method in typeof(MixedDensityModel).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == name))
            {
                seen.Add(name);
                onlyNumbers &= method.GetParameters().All(p => p.ParameterType == typeof(double) || p.ParameterType == typeof(OccupancyCalibration)) && method.ReturnType == typeof(double);
            }
        Check(onlyNumbers && seen.Distinct().Count() == formulaMethods.Length, "NO_SPECIES_OR_CLASS_ARGUMENT_IN_ANY_MORTALITY_FORMULA", $"methods={seen.Distinct().Count()}");

        // Source: the bodies of those methods never mention a class, weight, species, stem or id.
        string model = StripComments(ReadSource(repo, "Assets", "ForestPrototype", "MixedDensityModel.cs"));
        bool clean = true; var dirty = new List<string>();
        foreach (string signature in new[] { "public static double OvercrowdingPressure(", "public static double ExpectedAnnualLoss(", "public static double Suppression(", "public static double Vulnerability(", "public static double AnnualLossProbability(" })
        {
            string body = MethodSource(model, signature);
            bool bad = body == null || Regex.IsMatch(body, "Demand|Weight|Species|TreeId|DensityStem|Oak|Beech|Sitka|Reference");
            if (bad) { clean = false; dirty.Add(signature); }
        }
        Check(clean, "NO_SPECIES_CLASS_OR_WEIGHT_IN_ANY_MORTALITY_FORMULA_SOURCE", string.Join(",", dirty));

        // The only place the class is used is the occupancy sum.
        var usesOfWeight = Regex.Matches(model, @"\bWeight\(").Count;
        string stemOccupied = MethodSource(model, "public static double StemOccupiedSpace(");
        Check(stemOccupied != null && stemOccupied.Contains("Weight(stem.Demand)") && usesOfWeight == 2 /* definition + the one use */, "THE_CLASS_WEIGHT_IS_USED_ONLY_IN_THE_OCCUPANCY_SUM", $"occurrences={usesOfWeight}");
    }

    private static void NormalizationOnlyMovesTheMixedRoute()
    {
        List<DensityStem> mixed = RandomStand(new Random(5), 520, 20.0, 34.0, ThreeWay);
        var strong = new OccupancyCalibration(MixedDensityModel.OccupancyNormalization * 0.5, MixedDensityModel.MaximumAnnualLoss);
        var weak = new OccupancyCalibration(MixedDensityModel.OccupancyNormalization * 2.0, MixedDensityModel.MaximumAnnualLoss);
        DensityMortalityPlan baseline = MixedDensityModel.Plan(mixed, Area, KeyedRoll(3));
        DensityMortalityPlan plain = MixedDensityModel.Plan(mixed, Area, KeyedRoll(3), Default);
        DensityMortalityPlan more = MixedDensityModel.Plan(mixed, Area, KeyedRoll(3), strong);
        DensityMortalityPlan less = MixedDensityModel.Plan(mixed, Area, KeyedRoll(3), weak);
        Check(Fingerprint(baseline) == Fingerprint(plain), "DEFAULT_CALIBRATION_IS_THE_PRODUCTION_CONSTANTS");
        Check(Math.Abs(more.Occupancy / baseline.Occupancy - 2.0) < 1e-12 && Math.Abs(less.Occupancy / baseline.Occupancy - 0.5) < 1e-12
            && less.OvercrowdingPressure < baseline.OvercrowdingPressure && baseline.OvercrowdingPressure < more.OvercrowdingPressure
            && less.ExpectedAnnualLoss < baseline.ExpectedAnnualLoss && baseline.ExpectedAnnualLoss < more.ExpectedAnnualLoss,
            "NORMALIZATION_CHANGE_MOVES_MIXED_ROUTE_OCCUPANCY_PRESSURE_AND_LOSS", $"occupancy {F(less.Occupancy)} < {F(baseline.Occupancy)} < {F(more.Occupancy)}; loss {F(less.ExpectedAnnualLoss, "0.0000")} < {F(baseline.ExpectedAnnualLoss, "0.0000")} < {F(more.ExpectedAnnualLoss, "0.0000")}");
        // The same keyed draws: stronger pressure can only add victims to the weaker pressure's victims.
        Check(less.VictimIndices.All(more.VictimIndices.Contains) && baseline.VictimIndices.All(more.VictimIndices.Contains) && less.VictimIndices.All(baseline.VictimIndices.Contains)
            && less.VictimIndices.Count < more.VictimIndices.Count, "NORMALIZATION_CHANGE_ONLY_ADDS_OR_REMOVES_VICTIMS_NESTED", $"victims {less.VictimIndices.Count} <= {baseline.VictimIndices.Count} <= {more.VictimIndices.Count}");

        // Pure Sitka: the legacy numbers are a pure function of the stand and cannot see the calibration.
        bool sitkaStable = true;
        foreach ((int n, float dq) in new[] { (900, 28f), (400, 50f), (1500, 18f) })
        {
            double before = SitkaGrowthModel.RelativeDensity(n / Area, dq);
            MixedDensityModel.Plan(Uniform(Hi, 300, 30f, 1.0, "O"), Area, KeyedRoll(1), strong);
            MixedDensityModel.Plan(Uniform(Hi, 300, 30f, 1.0, "O"), Area, KeyedRoll(1), weak);
            double after = SitkaGrowthModel.RelativeDensity(n / Area, dq);
            sitkaStable &= BitConverter.DoubleToInt64Bits(before) == BitConverter.DoubleToInt64Bits(after)
                && BitConverter.DoubleToInt64Bits(SitkaGrowthModel.DensityPressure(before)) == BitConverter.DoubleToInt64Bits(SitkaGrowthModel.DensityPressure(after));
        }
        // No shared mutable state could carry a calibration into the Sitka route.
        bool noState = typeof(MixedDensityModel).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).All(f => f.IsLiteral || f.IsInitOnly)
            && typeof(OccupancyCalibration).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).All(f => f.IsInitOnly);
        bool invalidRejected = false;
        try { new OccupancyCalibration(0.0, 0.1); } catch (ArgumentOutOfRangeException) { invalidRejected = true; }
        try { new OccupancyCalibration(55.0, 1.0); invalidRejected = false; } catch (ArgumentOutOfRangeException) { }
        Check(sitkaStable && noState && invalidRejected, "PURE_SITKA_NUMBERS_UNCHANGED_BY_ANY_NORMALIZATION_CHANGE_AND_NO_SHARED_STATE", $"sitkaBitIdentical={sitkaStable} noMutableState={noState} invalidCalibrationRejected={invalidRejected}");
    }

    // ---------- pure Oak and pure Beech ----------

    private static void PureBroadleafDense()
    {
        foreach ((GrowingSpaceDemandClass cls, string label, int stems, int zoneStems) in new[] { (Hi, "OAK", 500, 340), (Mid, "BEECH", 600, 450) })
        {
            // Intentionally overcrowded: occupancy well above the onset.
            List<DensityStem> dense = RandomStand(new Random(label.Length * 17), stems, 24.0, 36.0, _ => cls);
            DensityMortalityPlan plan = MixedDensityModel.Plan(dense, Area, KeyedRoll(5));
            Check(plan.Occupancy > 1.5 && plan.OvercrowdingPressure > 0.3, $"PURE_{label}_DENSE_OCCUPANCY_EXCEEDS_THE_ONSET",
                $"occupancy={F(plan.Occupancy)} pressure={F(plan.OvercrowdingPressure)} expectedLoss={F(plan.ExpectedAnnualLoss, "0.0000")} stems={stems}");
            Check(plan.VictimIndices.Count > 0 && plan.HazardDeaths == plan.VictimIndices.Count && plan.VictimIndices.All(i => dense[i].Demand == cls), $"PURE_{label}_CAN_DIE_FROM_DENSITY_PRESSURE",
                $"victims={plan.VictimIndices.Count}/{stems}");
            var survivors = dense.Where((s, i) => !plan.VictimIndices.Contains(i)).ToList();
            Check(Occ(survivors) < plan.Occupancy, $"PURE_{label}_OCCUPANCY_FALLS_AFTER_THINNING", $"{F(plan.Occupancy)} -> {F(Occ(survivors))}");

            // Over the years the stand keeps losing stems while it stays above the onset, and loses far fewer once under it.
            int lost = 0, years = 12;
            var living = new List<DensityStem>(dense);
            for (int year = 1; year <= years; year++)
            {
                DensityMortalityPlan step = MixedDensityModel.Plan(living, Area, KeyedRoll(100 + year));
                lost += step.VictimIndices.Count;
                var dead = new HashSet<int>(step.VictimIndices);
                living = living.Where((s, i) => !dead.Contains(i)).ToList();
            }
            double after = Occ(living);
            Check(lost > stems / 4 && after < plan.Occupancy && living.Count > 1, $"PURE_{label}_SELF_THINS_TOWARDS_THE_ONSET_OVER_{years}_YEARS_WITHOUT_BEING_REMOVED",
                $"lost={lost}/{stems} occupancy {F(plan.Occupancy)} -> {F(after)} (no growth in this fixture)");

            // Modestly above the onset the loss is small but not zero.
            List<DensityStem> zone = Uniform(cls, zoneStems, 28f, 3.0, "Z").Select((s, i) => Stem(s.TreeId, cls, 24.0 + (i % 9), 1.0 + (i % 7))).ToList();
            double zoneOccupancy = Occ(zone);
            int zoneDeaths = 0;
            for (int seed = 1; seed <= 40; seed++) zoneDeaths += MixedDensityModel.Plan(zone, Area, KeyedRoll(seed)).VictimIndices.Count;
            Check(zoneOccupancy > 1.0 && zoneOccupancy < 1.5 && zoneDeaths > 0, $"PURE_{label}_JUST_ABOVE_THE_ONSET_LOSES_FEW_STEMS",
                $"occupancy={F(zoneOccupancy)} deathsOver40Years={zoneDeaths}");
        }

        // The same dense Oak stems counted as Reference class are exactly half as crowded and far less pressed.
        List<DensityStem> oak = RandomStand(new Random(3 * 17), 500, 24.0, 36.0, _ => Hi);
        List<DensityStem> asReference = oak.Select(s => Stem(s.TreeId, Ref, s.DbhCm, s.CompetitionIndex)).ToList();
        double oakOccupancy = Occ(oak), referenceOccupancy = Occ(asReference);
        Check(Math.Abs(oakOccupancy / referenceOccupancy - 2.0) < 1e-12 && MixedDensityModel.OvercrowdingPressure(referenceOccupancy) < MixedDensityModel.OvercrowdingPressure(oakOccupancy) / 10.0,
            "SAME_STEMS_COUNTED_AS_REFERENCE_ARE_HALF_AS_CROWDED_AS_OAK", $"oak={F(oakOccupancy)} reference={F(referenceOccupancy)}");
    }

    // ---------- mixed Sitka / Oak / Beech ----------

    private static void MixedStands()
    {
        // Replace stems by higher-demand classes at the same size: occupancy rises step by step.
        const int n = 300;
        double previous = -1.0; bool rising = true;
        foreach (int oaks in new[] { 0, 30, 60, 120, 200, 300 })
        {
            var stems = Enumerable.Range(0, n).Select(i => Stem("M" + i.ToString("D4", Inv), i < oaks ? Hi : Ref, 26, 1.0)).OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
            double o = Occ(stems);
            if (o <= previous) rising = false;
            previous = o;
        }
        Check(rising, "MIXED_REPLACING_SITKA_BY_OAK_RAISES_OCCUPANCY_STEP_BY_STEP");

        var baseStand = RandomStand(new Random(8), 450, 12.0, 40.0, ThreeWay);
        int oakIndex = baseStand.FindIndex(s => s.Demand == Hi);
        DensityStem oak = baseStand[oakIndex];
        double with = Occ(baseStand);
        double without = Occ(baseStand.Where((s, i) => i != oakIndex).ToList());
        double asBeech = Occ(baseStand.Select((s, i) => i == oakIndex ? Stem(s.TreeId, Mid, s.DbhCm, s.CompetitionIndex) : s).ToList());
        double asSitka = Occ(baseStand.Select((s, i) => i == oakIndex ? Stem(s.TreeId, Ref, s.DbhCm, s.CompetitionIndex) : s).ToList());
        double expectedDrop = MixedDensityModel.StemOccupiedSpace(oak) / Area / MixedDensityModel.OccupancyNormalization;
        Check(without < asSitka && asSitka < asBeech && asBeech < with, "MIXED_REMOVING_OR_DOWNGRADING_A_HIGH_DEMAND_STEM_LOWERS_OCCUPANCY_IN_ORDER",
            $"removed={F(without, "0.00000")} asSitka={F(asSitka, "0.00000")} asBeech={F(asBeech, "0.00000")} oak={F(with, "0.00000")}");
        Check(Math.Abs((with - without) - expectedDrop) < 1e-12, "MIXED_REMOVING_A_STEM_LOWERS_OCCUPANCY_BY_EXACTLY_ITS_CONTRIBUTION", $"drop={expectedDrop:E3}");

        // Deterministic replay of a dying three-species stand, including its victim list.
        List<DensityStem> dying = RandomStand(new Random(1234), 800, 18.0, 40.0, ThreeWay);
        string a = Fingerprint(MixedDensityModel.Plan(dying, Area, KeyedRoll(9)));
        string b = Fingerprint(MixedDensityModel.Plan(dying, Area, KeyedRoll(9)));
        DensityMortalityPlan plan = MixedDensityModel.Plan(dying, Area, KeyedRoll(9));
        var perClass = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        foreach (int i in plan.VictimIndices) perClass[dying[i].Demand]++;
        Check(a == b && plan.VictimIndices.Count > 0, "MIXED_THREE_SPECIES_REPLAY_IDENTICAL", a.Substring(0, 24));
        Check(perClass.Values.All(v => v > 0), "MIXED_STAND_LOSES_STEMS_OF_ALL_THREE_CLASSES (no class is immune)",
            $"reference={perClass[Ref]} intermediate={perClass[Mid]} high={perClass[Hi]} occupancy={F(plan.Occupancy)}");
    }

    // ---------- TESTS F and G: below and above the onset ----------

    private static void Thresholds()
    {
        // Below the onset: a roll that would kill any stem with a non-zero probability kills none, and none is consumed.
        int calls = 0;
        Func<string, double> alwaysKills = id => { calls++; return 0.0; };
        List<DensityStem> below = RandomStand(new Random(41), 350, 18.0, 30.0, ThreeWay);
        DensityMortalityPlan low = MixedDensityModel.Plan(below, Area, alwaysKills);
        Check(low.Occupancy > 0.3 && low.Occupancy <= MixedDensityModel.OnsetOccupancy && low.OvercrowdingPressure == 0.0 && low.VictimIndices.Count == 0 && calls == 0,
            "BELOW_THRESHOLD_MIXED_STAND_NO_DENSITY_MORTALITY", $"occupancy={F(low.Occupancy)} onset={F(MixedDensityModel.OnsetOccupancy, "0.00")} victims={low.VictimIndices.Count} rolls={calls}");

        // Exact crossing: one stem fewer is at or under the onset (no pressure, no roll), one more is over it.
        int under = 0, over = 0;
        for (int stems = 20; stems < 4000; stems++)
        {
            double o = Occ(Uniform(Hi, stems, 25f, 1.0, "O"));
            if (o <= MixedDensityModel.OnsetOccupancy) under = stems; else { over = stems; break; }
        }
        calls = 0;
        DensityMortalityPlan atUnder = MixedDensityModel.Plan(Uniform(Hi, under, 25f, 1.0, "O"), Area, alwaysKills);
        int callsUnder = calls;
        DensityMortalityPlan atOver = MixedDensityModel.Plan(Uniform(Hi, over, 25f, 1.0, "O"), Area, alwaysKills);
        Check(over == under + 1 && atUnder.OvercrowdingPressure == 0.0 && callsUnder == 0 && atOver.OvercrowdingPressure > 0.0 && atOver.VictimIndices.Count > 0,
            "THRESHOLD_CROSSING_IS_EXACT", $"stemsUnder={under} stemsOver={over} pressureOver={F(atOver.OvercrowdingPressure, "0.000000")}");
        Check(calls - callsUnder <= over, "AT_MOST_ONE_ROLL_PER_STEM", $"rolls={calls - callsUnder} stems={over}");

        // Above the onset the hazard decides: with a draw that never kills there are no deaths, with one that always
        // kills every stem with a non-zero probability dies.
        List<DensityStem> mid = RandomStand(new Random(42), 520, 20.0, 34.0, ThreeWay);
        DensityMortalityPlan zone = MixedDensityModel.Plan(mid, Area, KeyedRoll(2));
        DensityMortalityPlan zoneNever = MixedDensityModel.Plan(mid, Area, id => 0.9999999);
        DensityMortalityPlan zoneAlways = MixedDensityModel.Plan(mid, Area, id => 0.0);
        Check(zone.Occupancy > MixedDensityModel.OnsetOccupancy && zone.OvercrowdingPressure > 0.0 && zone.VictimIndices.Count > 0
            && zoneNever.VictimIndices.Count == 0 && zoneAlways.VictimIndices.Count > 0,
            "ABOVE_THRESHOLD_MIXED_STAND_DENSITY_MORTALITY_OCCURS", $"occupancy={F(zone.Occupancy)} pressure={F(zone.OvercrowdingPressure)} deaths={zone.VictimIndices.Count} neverKillsDraw={zoneNever.VictimIndices.Count} alwaysKillsDraw={zoneAlways.VictimIndices.Count}");

        // Heavily overcrowded: more pressure, more loss, still bounded by the maximum.
        List<DensityStem> crowded = RandomStand(new Random(43), 900, 20.0, 40.0, ThreeWay);
        DensityMortalityPlan heavy = MixedDensityModel.Plan(crowded, Area, KeyedRoll(2));
        Check(heavy.OvercrowdingPressure > zone.OvercrowdingPressure && heavy.ExpectedAnnualLoss > zone.ExpectedAnnualLoss && heavy.ExpectedAnnualLoss < MixedDensityModel.MaximumAnnualLoss
            && heavy.VictimIndices.Count < crowded.Count / 5, "HEAVY_OVERCROWDING_THINS_MORE_BUT_REMAINS_BOUNDED",
            $"occupancy={F(heavy.Occupancy)} expectedLoss={F(heavy.ExpectedAnnualLoss, "0.0000")} deaths={heavy.VictimIndices.Count}/{crowded.Count}");
    }

    // ---------- TEST F: victim selection is competitive position only ----------

    private static void VictimSelection()
    {
        var random = new Random(424242);
        int trials = 0, noDuplicates = 0, ordered = 0, selectivity = 0, selectivityTrials = 0;
        var perClassDeaths = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        for (int rep = 0; rep < 40; rep++)
        {
            List<DensityStem> stems = RandomStand(random, 600 + rep * 20, 16.0, 38.0, ThreeWay);
            DensityMortalityPlan plan = MixedDensityModel.Plan(stems, Area, KeyedRoll(rep));
            trials++;
            if (plan.VictimIndices.Distinct().Count() == plan.VictimIndices.Count) noDuplicates++;
            if (plan.VictimIndices.Zip(plan.VictimIndices.Skip(1), (x, y) => string.CompareOrdinal(stems[x].TreeId, stems[y].TreeId) < 0).All(b => b)) ordered++;
            var dead = new HashSet<int>(plan.VictimIndices);
            if (plan.VictimIndices.Count >= 15)
            {
                selectivityTrials++;
                if (plan.VictimIndices.Average(i => stems[i].CompetitionIndex) > stems.Where((s, i) => !dead.Contains(i)).Average(s => s.CompetitionIndex)) selectivity++;
            }
            foreach (int i in plan.VictimIndices) perClassDeaths[stems[i].Demand]++;
        }
        Check(noDuplicates == trials, "PLAN_NO_DUPLICATE_VICTIMS", $"trials={trials}");
        Check(ordered == trials, "VICTIMS_ARE_LISTED_IN_ORDINAL_TREE_ID_ORDER", $"trials={trials}");
        Check(selectivityTrials > 10 && selectivity == selectivityTrials, "VICTIMS_MORE_SUPPRESSED_THAN_SURVIVORS", $"trials={selectivityTrials} ok={selectivity}");
        Check(perClassDeaths.Values.All(v => v > 0), "ALL_THREE_CLASSES_LOSE_STEMS_IN_AGGREGATE", $"reference={perClassDeaths[Ref]} intermediate={perClassDeaths[Mid]} high={perClassDeaths[Hi]}");

        // The suppression scale is one scale for every class, and risk rises with competition.
        Check(Math.Abs(MixedDensityModel.Suppression(MixedDensityModel.NeutralHalfSaturationCi) - 0.5) < 1e-12 && MixedDensityModel.Suppression(0) == 0.0
            && MixedDensityModel.Suppression(1e9) <= 1.0 && MixedDensityModel.Suppression(2.0) < MixedDensityModel.Suppression(6.0), "SUPPRESSION_ONE_COMMON_SCALE");
        Check(MixedDensityModel.AnnualLossProbability(0.03, MixedDensityModel.Suppression(2.0), 0.05) < MixedDensityModel.AnnualLossProbability(0.03, MixedDensityModel.Suppression(6.0), 0.05),
            "LOSS_PROBABILITY_RISES_WITH_COMPETITION");

        // No systematic species preference: 150 slots of three stems (one per class) with identical size and
        // competition and rotating ids, tuned to a moderate occupancy.
        double scale = 1.0;
        List<DensityStem> Triples(double s, double ciFactorForNonReference)
        {
            var stems = new List<DensityStem>();
            for (int slot = 0; slot < 150; slot++)
            {
                double dbh = (20.0 + slot % 25) * s, ci = 1.0 + slot % 6;
                for (int j = 0; j < 3; j++)
                {
                    GrowingSpaceDemandClass cls = ThreeWay(slot + j);
                    stems.Add(Stem("N" + (slot * 3 + j).ToString("D5", Inv), cls, dbh, cls == Ref ? ci : ci * ciFactorForNonReference));
                }
            }
            return stems.OrderBy(x => x.TreeId, StringComparer.Ordinal).ToList();
        }
        double lo = 0.2, hi = 3.0;
        for (int i = 0; i < 40; i++) { scale = (lo + hi) / 2.0; if (Occ(Triples(scale, 1.0)) > 1.6) hi = scale; else lo = scale; }
        List<DensityStem> neutral = Triples(scale, 1.0);
        var counts = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        for (int seed = 1; seed <= 400; seed++)
            foreach (int i in MixedDensityModel.Plan(neutral, Area, KeyedRoll(seed)).VictimIndices) counts[neutral[i].Demand]++;
        int total = counts.Values.Sum();
        double sigma = Math.Sqrt(total * (1.0 / 3.0) * (2.0 / 3.0));
        double spread = counts.Values.Max(v => Math.Abs(v - total / 3.0)) / sigma;
        Check(total > 3000 && spread < 5.0, "HAZARD_HAS_NO_SPECIES_PREFERENCE_AT_EQUAL_COMPETITION_AND_SIZE",
            $"occupancy={F(Occ(neutral))} deaths={total} reference={counts[Ref]} intermediate={counts[Mid]} high={counts[Hi]} maxDeviation={F(spread, "0.00")}sigma");

        // Negative control: the same fixture with the OLD per-species suppression (a broadleaf stem of Ci50 3 reads
        // as Ci 5/3 higher on the Sitka Ci50 5 scale) is clearly biased against Oak and Beech, so the test above
        // has the power to detect a species preference. This is what was removed.
        List<DensityStem> biased = Triples(scale, 5.0 / 3.0);
        var biasedCounts = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        for (int seed = 1; seed <= 400; seed++)
            foreach (int i in MixedDensityModel.Plan(biased, Area, KeyedRoll(seed)).VictimIndices) biasedCounts[biased[i].Demand]++;
        int biasedTotal = biasedCounts.Values.Sum();
        double biasedSpread = biasedCounts.Values.Max(v => Math.Abs(v - biasedTotal / 3.0)) / Math.Sqrt(biasedTotal * (1.0 / 3.0) * (2.0 / 3.0));
        Check(biasedSpread > 5.0 && biasedCounts[Ref] < biasedCounts[Mid] && biasedCounts[Ref] < biasedCounts[Hi], "NEGATIVE_CONTROL_PER_SPECIES_CI50_WOULD_BE_DETECTED",
            $"reference={biasedCounts[Ref]} intermediate={biasedCounts[Mid]} high={biasedCounts[Hi]} maxDeviation={F(biasedSpread, "0.0")}sigma");
    }

    // ---------- calibration against recorded Scenario One Sitka structures (game calibration, not evidence) ----------

    private static void Calibration(string repo)
    {
        string evidence = Path.Combine(repo, "Docs", "Research", "SitkaGrowthMortality", "Evidence");
        string structuresFile = Path.Combine(evidence, "growth_model1_stand_development.csv");
        string mortalityFile = Path.Combine(evidence, "calibration_selected_mortality_by_year.csv");
        if (!File.Exists(structuresFile) || !File.Exists(mortalityFile))
        {
            Check(false, "CALIBRATION_EVIDENCE_FILES_FOUND", evidence);
            return;
        }

        // Recorded growth-model-1 Sitka stands: four management regimes from age 20 to 120.
        string[] lines = File.ReadAllLines(structuresFile);
        string[] header = lines[0].Split(',');
        int run = Array.IndexOf(header, "run"), age = Array.IndexOf(header, "age"), nha = Array.IndexOf(header, "stems_ha"), dq = Array.IndexOf(header, "dq_cm"), rdColumn = Array.IndexOf(header, "relative_density_comeau");
        var structures = lines.Skip(1).Select(l => l.Split(',')).Where(c => c[run].StartsWith("growthModel1/", StringComparison.Ordinal))
            .Select(c => (run: c[run].Substring("growthModel1/".Length), age: int.Parse(c[age], Inv), n: double.Parse(c[nha], Inv), dq: double.Parse(c[dq], Inv), rd: double.Parse(c[rdColumn], Inv))).ToList();
        double onsetRd = SitkaGrowthModel.MortalityOnsetRelativeDensity;

        double worstDeviation = 0.0, sumDeviation = 0.0, worstRecordedVsCode = 0.0;
        Console.WriteLine("INFO calibration structures (regime age stems/ha Dq legacyRD occupancy onsetAlignedRD difference legacyHazardLoss genericLoss)");
        foreach (var s in structures)
        {
            double legacy = SitkaGrowthModel.RelativeDensity(s.n, s.dq);
            worstRecordedVsCode = Math.Max(worstRecordedVsCode, Math.Abs(legacy - s.rd));
            double occupancy = s.n * Math.PI / 4.0 * (s.dq / 100.0) * (s.dq / 100.0) / MixedDensityModel.OccupancyNormalization;
            double aligned = occupancy * onsetRd, deviation = aligned / legacy - 1.0;
            worstDeviation = Math.Max(worstDeviation, Math.Abs(deviation)); sumDeviation += Math.Abs(deviation);
            double legacyLoss = SitkaGrowthModel.MortalityStrength * Math.Pow(SitkaGrowthModel.DensityPressure(legacy), 2.0);
            double genericLoss = MixedDensityModel.ExpectedAnnualLoss(MixedDensityModel.OvercrowdingPressure(occupancy), Default);
            if (s.run == "unthinned")
                Console.WriteLine($"INFO   {s.run,-10} {s.age,3} {s.n,5:0} {s.dq,6:0.0} {F(legacy, "0.000")} {F(occupancy, "0.000")} {F(aligned, "0.000")} {F(deviation * 100, "+0.0;-0.0")}% {F(legacyLoss * 100, "0.0")}% {F(genericLoss * 100, "0.0")}%");
        }
        double mean = sumDeviation / structures.Count;
        Check(structures.Count == 32 && worstRecordedVsCode < 0.003, "CALIBRATION_STRUCTURES_MATCH_THE_LEGACY_RELATIVE_DENSITY_CODE", $"structures={structures.Count} largestDifferenceToRecordedRd={worstRecordedVsCode:0.0000} (recorded inputs are rounded; guard 0.003)");
        Check(worstDeviation < 0.07 && mean < 0.035, "CONTINUITY_ONSET_ALIGNED_OCCUPANCY_NEAR_LEGACY_RELATIVE_DENSITY_ON_RECORDED_STRUCTURES",
            $"structures={structures.Count} worst={F(worstDeviation * 100, "0.0")}% mean={F(mean * 100, "0.00")}% (guard 7% and 3.5%)");

        // The pressure onset lands where the legacy onset does for a Reference stand at representative sizes.
        bool onsetNear = true;
        foreach (double diameter in new[] { 16.0, 25.0, 40.0, 64.0 })
        {
            double stemsAtLegacyOnset = onsetRd * 1868.0 / Math.Pow(diameter / 25.0, 2.063);
            double occupancy = stemsAtLegacyOnset * Math.PI / 4.0 * (diameter / 100.0) * (diameter / 100.0) / MixedDensityModel.OccupancyNormalization;
            onsetNear &= Math.Abs(occupancy - 1.0) < 0.06;
        }
        Check(onsetNear, "OCCUPANCY_ONSET_FALLS_WITHIN_6_PERCENT_OF_THE_LEGACY_ONSET_AT_DQ_16_TO_64_CM");

        // Realised legacy annual loss by relative-density bin (the recorded selected rule, hazard AND ceiling) against the generic loss.
        string[] mLines = File.ReadAllLines(mortalityFile);
        string[] mh = mLines[0].Split(',');
        int mRun = Array.IndexOf(mh, "run"), mRd = Array.IndexOf(mh, "rd_before"), mLiving = Array.IndexOf(mh, "living"), mDeaths = Array.IndexOf(mh, "deaths");
        var records = mLines.Skip(1).Select(l => l.Split(',')).Where(c => c[mRun].Contains("onset0.6_m0.08"))
            .Select(c => (rd: double.Parse(c[mRd], Inv), living: int.Parse(c[mLiving], Inv), deaths: int.Parse(c[mDeaths], Inv))).ToList();
        double sumDifference = 0.0; int bins = 0;
        Console.WriteLine("INFO calibration loss by relative density (bin realisedLegacy% genericAtBinMid%)");
        foreach (double mid in new[] { 0.7, 0.8, 0.9 })
        {
            var inBin = records.Where(r => r.rd >= mid - 0.05 && r.rd < mid + 0.05).ToList();
            double realised = inBin.Sum(r => r.deaths) / (double)inBin.Sum(r => r.living);
            double occupancy = mid / onsetRd;
            double generic = MixedDensityModel.ExpectedAnnualLoss(MixedDensityModel.OvercrowdingPressure(occupancy), Default);
            sumDifference += Math.Abs(generic - realised); bins++;
            Console.WriteLine($"INFO   {F(mid, "0.00")} n={inBin.Count} {F(realised * 100, "0.00")}% {F(generic * 100, "0.00")}%");
        }
        double meanDifference = sumDifference / bins;
        Check(records.Count > 300 && meanDifference < 0.01, "GENERIC_LOSS_TRACKS_REALISED_LEGACY_LOSS_IN_THE_HAZARD_ZONE_WITHIN_ONE_POINT",
            $"records={records.Count} meanAbsoluteDifference={F(meanDifference * 100, "0.00")} percentage points over relative density 0.65-0.95 (guard 1.0)");
    }

    // ---------- determinism ----------

    private static void Determinism()
    {
        var random = new Random(1234);
        List<DensityStem> stems = RandomStand(random, 800, 15.0, 40.0, ThreeWay);
        string a = Fingerprint(MixedDensityModel.Plan(stems, Area, KeyedRoll(9)));
        string b = Fingerprint(MixedDensityModel.Plan(stems, Area, KeyedRoll(9)));
        Check(a == b, "REPLAY_IDENTICAL", a.Substring(0, 24));

        // The model puts stems in ordinal id order itself, so listing order cannot change who dies or in what order.
        var rng = new Random(7);
        List<DensityStem> shuffled = stems.OrderBy(_ => rng.Next()).ToList();
        DensityMortalityPlan p1 = MixedDensityModel.Plan(stems, Area, KeyedRoll(9));
        DensityMortalityPlan p2 = MixedDensityModel.Plan(shuffled, Area, KeyedRoll(9));
        bool sameVictims = p1.VictimIndices.Select(i => stems[i].TreeId).SequenceEqual(p2.VictimIndices.Select(i => shuffled[i].TreeId));
        Check(sameVictims && p1.VictimIndices.Count > 0 && BitConverter.DoubleToInt64Bits(p1.Occupancy) == BitConverter.DoubleToInt64Bits(p2.Occupancy)
            && p1.HazardDeaths == p2.HazardDeaths, "LISTING_ORDER_DOES_NOT_CHANGE_VICTIMS_OR_OCCUPANCY", $"victims={p1.VictimIndices.Count}");

        var distinct = new HashSet<string>();
        for (int year = 1; year <= 12; year++)
            distinct.Add(Fingerprint(MixedDensityModel.Plan(stems, Area, KeyedRoll(year))));
        Check(distinct.Count > 6, "DIFFERENT_YEARS_GIVE_DIFFERENT_DRAWS", $"distinctOutcomes={distinct.Count}/12");
    }

    // ---------- degenerate inputs ----------

    private static void Degenerate()
    {
        Check(MixedDensityModel.Plan(new List<DensityStem>(), Area, KeyedRoll(0)).VictimIndices.Count == 0, "EMPTY_STAND_NO_VICTIMS");
        Check(MixedDensityModel.Plan(Uniform(Hi, 5, 25f, 1.0, "O"), 0f, KeyedRoll(0)).VictimIndices.Count == 0, "ZERO_AREA_NO_VICTIMS");
        DensityMortalityPlan single = MixedDensityModel.Plan(Uniform(Hi, 1, 90f, 1.0, "O"), 0.01f, id => 0.0);
        Check(single.Occupancy > 1.0 && single.VictimIndices.Count <= 1, "SINGLE_STEM_STAND_IS_HANDLED", $"occupancy={F(single.Occupancy)} victims={single.VictimIndices.Count}");
        // No competition at all: nobody is vulnerable, so the hazard picks nobody however crowded the stand is.
        DensityMortalityPlan free = MixedDensityModel.Plan(Uniform(Hi, 800, 30f, 0.0, "F"), Area, id => 0.0);
        Check(free.OvercrowdingPressure > 0.0 && free.ExpectedAnnualLoss > 0.0 && free.HazardDeaths == 0, "NO_COMPETITION_NO_VULNERABLE_STEM_NO_DEATH",
            $"occupancy={F(free.Occupancy)} expectedLoss={F(free.ExpectedAnnualLoss, "0.0000")}");
        Check(MixedDensityModel.AnyNonReference(Uniform(Hi, 1, 20f, 1.0, "O")) && !MixedDensityModel.AnyNonReference(Uniform(Ref, 3, 20f, 1.0, "S")), "ANY_NON_REFERENCE");
        Check(MixedDensityModel.StemOccupiedSpace(Stem("z", Hi, 0, 1.0)) == 0.0 && MixedDensityModel.ExpectedAnnualLoss(0.0, Default) == 0.0 && MixedDensityModel.OvercrowdingPressure(0.0) == 0.0,
            "ZERO_SIZE_AND_ZERO_PRESSURE_ARE_INERT");
    }
}

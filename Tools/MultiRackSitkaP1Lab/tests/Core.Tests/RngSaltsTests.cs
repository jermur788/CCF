using System.Reflection;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Phase2Support;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// S0b verification suite for <see cref="RngSalts"/> (SPEC-multi-2d-s0-seam-extraction-V1 §6.4, S0-07):
/// frozen byte-identical literals, pairwise distinctness (case-sensitive and case-insensitive),
/// strict prefix freedom except the exact three frozen nested ADULT-MORT-* pairs,
/// collision freedom across real MA-1 and MA-1-MIXED genesis + 30-year runs (including PL- promotions),
/// and rejection of empty or untrimmed entries.
/// </summary>
internal static class RngSaltsTests
{
    /// <summary>
    /// Exact, explicit allow-list of the three frozen nested legacy prefix pairs (spec §6.4 / OQ-6a).
    /// Any post-S0 family must satisfy prefix freedom with NO allow-list entry.
    /// </summary>
    private static readonly (string Prefix, string Longer)[] FrozenNestedPrefixes =
    {
        ("ADULT-MORT-", "ADULT-MORT-GENERIC-"),
        ("ADULT-MORT-", "ADULT-MORT-GENERIC-QUOTA-"),
        ("ADULT-MORT-GENERIC-", "ADULT-MORT-GENERIC-QUOTA-"),
    };

    internal static void Register()
    {
        T.Add("S0b RngSalts: every constant equals its frozen literal, call-site helpers match, and Families covers every const", () =>
        {
            // Literals intentionally duplicated here to freeze every key family byte-for-byte against T0.
            T.Eq("ADULT-MORT-", RngSalts.AdultMortality, "AdultMortality");
            T.Eq("ADULT-MORT-GENERIC-", RngSalts.AdultMortalityGeneric, "AdultMortalityGeneric");
            T.Eq("ADULT-MORT-GENERIC-QUOTA-", RngSalts.AdultMortalityGenericQuota, "AdultMortalityGenericQuota");
            T.Eq("PHASE2-MAST-", RngSalts.Phase2Mast, "Phase2Mast");
            T.Eq("PHASE2-PJ-", RngSalts.Phase2Juvenile, "Phase2Juvenile");
            T.Eq("/vegetation", RngSalts.Phase2JuvenileVegetationSuffix, "Phase2JuvenileVegetationSuffix");
            T.Eq("/browse", RngSalts.Phase2JuvenileBrowseSuffix, "Phase2JuvenileBrowseSuffix");
            T.Eq("STAGE5-MAST", RngSalts.Stage5Mast, "Stage5Mast");
            T.Eq("STAGE5-PROMO-", RngSalts.Stage5Promotion, "Stage5Promotion");

            // Verify substituted helper sites produce byte-identical keys.
            T.Eq("STAGE5-MAST", Stage5Random.MastKey, "Stage5Random.MastKey");
            T.Eq("STAGE5-PROMO-MA-A-3-7-12", Stage5Random.PromotionKey("MA-A", 3, 7, 12), "Stage5Random.PromotionKey");
            T.Eq("PHASE2-MAST-sessile-oak", Phase2Random.MastKey("sessile-oak"), "Phase2Random.MastKey");
            T.Eq("ADULT-MORT-GENERIC-MA-A-001", Phase2Random.GenericTreeKey("MA-A-001"), "Phase2Random.GenericTreeKey");
            T.Eq("ADULT-MORT-GENERIC-QUOTA-MA-B", Phase2Random.GenericQuotaKey("MA-B"), "Phase2Random.GenericQuotaKey");
            T.Eq("PHASE2-PJ-PJ-001", Phase2Random.JuvenileKey("PJ-001"), "Phase2Random.JuvenileKey");
            T.Eq("PHASE2-PJ-PJ-001/vegetation", Phase2Random.JuvenileVegetationKey("PJ-001"), "Phase2Random.JuvenileVegetationKey");
            T.Eq("PHASE2-PJ-PJ-001/browse", Phase2Random.JuvenileBrowseKey("PJ-001"), "Phase2Random.JuvenileBrowseKey");

            // Reflection check: every const string declared on RngSalts must be registered in RngSalts.Families.
            var constValues = typeof(RngSalts)
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => (string)f.GetRawConstantValue()!)
                .ToArray();
            T.Eq(constValues.Length, RngSalts.Families.Count, "Families count equals const string field count");
            foreach (var cv in constValues)
                T.True(RngSalts.Families.Contains(cv, StringComparer.Ordinal), "Families contains const " + cv);
        });

        T.Add("S0b RngSalts: every registered value is non-empty, trimmed, and pairwise distinct (case-sensitive and case-insensitive)", () =>
        {
            ValidateNonEmptyAndDistinct(RngSalts.Families);
        });

        T.Add("S0b RngSalts: no registered value is a prefix of another except the exact three allow-listed ADULT-MORT-* pairs", () =>
        {
            T.Eq(3, FrozenNestedPrefixes.Length, "allow-list must contain exactly three pairs");
            ValidatePrefixFreedom(RngSalts.Families);
        });

        T.Add("S0b RngSalts: collision freedom of generated keys across MA-1 and MA-1-MIXED genesis plus 30-year runs (including PL- promotions)", () =>
        {
            var observedAreaIds = new HashSet<string>(StringComparer.Ordinal);
            var observedTreeIds = new HashSet<string>(StringComparer.Ordinal);
            var observedPlPromotions = new HashSet<string>(StringComparer.Ordinal);

            void RecordSessionState(MultiAreaContinuation session)
            {
                var view = session.ReadView();
                foreach (var area in view.Areas)
                {
                    observedAreaIds.Add(area.AreaId);
                    foreach (var tree in area.Trees)
                    {
                        observedTreeIds.Add(tree.TreeId);
                        if (tree.TreeId.StartsWith("PL-", StringComparison.Ordinal))
                            observedPlPromotions.Add(tree.TreeId);
                    }
                }
            }

            void PlantBrightAreaB(MultiAreaContinuation session)
            {
                var s5 = Stage5Of(session);
                var locate = Phase2Annual.Locator(s5);
                float brightest = s5.Areas.Single(a => a.AreaId == "MA-B").Cells.Max(cell => cell.Light);
                var p = FindPoint(session, "MA-B", 1.2f, (x, z) =>
                    locate(x, z) is var (a, ci) && s5.Areas[a].AreaId == "MA-B" &&
                    s5.Areas[a].Cells[ci].Light >= 0.9f * brightest);
                session.AddSpotClear(Fence(session.ReadView()), "MA-B", p.X, p.Z);
                session.AddPlant(Fence(session.ReadView()), "MA-B", Oak, p.X, p.Z);
                session.AddShelter(Fence(session.ReadView()), "MA-B", p.X, p.Z);
                ApproveAndExecute(session);
            }

            // 1. MA-1 (Phase 2 model selection, bright Area B planting so PL- promotions occur + 30 years).
            var ma1 = Phase2Session(MultiAreaGenerator.Generate());
            RecordSessionState(ma1);
            PlantBrightAreaB(ma1);
            for (int y = 0; y < 30; y++)
            {
                Advance(ma1);
                RecordSessionState(ma1);
            }

            // 2. MA-1-MIXED (genesis + bright Area B planting + 30 years).
            var mixed = MixedSession();
            RecordSessionState(mixed);
            PlantBrightAreaB(mixed);
            for (int y = 0; y < 30; y++)
            {
                Advance(mixed);
                RecordSessionState(mixed);
            }

            T.True(observedAreaIds.Count >= 2, "at least two real areas observed");
            T.True(observedTreeIds.Count > 200, "real genesis and promoted trees observed");
            T.True(observedPlPromotions.Count > 0, "at least one PL- planted promotion occurred across 30-year MA-1 / MA-1-MIXED runs");

            foreach (var treeId in observedTreeIds)
            {
                T.True(!treeId.StartsWith("GENERIC-", StringComparison.OrdinalIgnoreCase),
                    "treeId must never start with GENERIC-: " + treeId);
                T.True(!treeId.StartsWith("QUOTA-", StringComparison.OrdinalIgnoreCase),
                    "treeId must never start with QUOTA-: " + treeId);
            }
            foreach (var areaId in observedAreaIds)
            {
                T.True(!areaId.StartsWith("QUOTA-", StringComparison.OrdinalIgnoreCase),
                    "areaId must never start with QUOTA-: " + areaId);
            }

            // Direct key-set disjointness check across all generated ADULT-MORT* families.
            var adultKeys = observedTreeIds.Select(id => RngSalts.AdultMortality + id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var genericKeys = observedTreeIds.Select(id => RngSalts.AdultMortalityGeneric + id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var quotaKeys = observedAreaIds.Select(id => RngSalts.AdultMortalityGenericQuota + id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            T.True(!adultKeys.Overlaps(genericKeys), "ADULT-MORT-* and ADULT-MORT-GENERIC-* keys are disjoint");
            T.True(!adultKeys.Overlaps(quotaKeys), "ADULT-MORT-* and ADULT-MORT-GENERIC-QUOTA-* keys are disjoint");
            T.True(!genericKeys.Overlaps(quotaKeys), "ADULT-MORT-GENERIC-* and ADULT-MORT-GENERIC-QUOTA-* keys are disjoint");
        });

        T.Add("S0b RngSalts: validation rules accept a clean post-S0 family and reject empty, whitespace, duplicate and new prefix entries", () =>
        {
            var withCleanNew = RngSalts.Families.Concat(new[] { "PHASE3-STORM-" }).ToArray();
            ValidateNonEmptyAndDistinct(withCleanNew);
            ValidatePrefixFreedom(withCleanNew);

            ExpectThrows(() => ValidateNonEmptyAndDistinct(RngSalts.Families.Concat(new[] { "" }).ToArray()), "empty rejected");
            ExpectThrows(() => ValidateNonEmptyAndDistinct(RngSalts.Families.Concat(new[] { "   " }).ToArray()), "whitespace rejected");
            ExpectThrows(() => ValidateNonEmptyAndDistinct(RngSalts.Families.Concat(new[] { " PHASE3-STORM-" }).ToArray()), "untrimmed rejected");
            ExpectThrows(() => ValidateNonEmptyAndDistinct(RngSalts.Families.Concat(new[] { "STAGE5-MAST" }).ToArray()), "exact duplicate rejected");
            ExpectThrows(() => ValidateNonEmptyAndDistinct(RngSalts.Families.Concat(new[] { "stage5-mast" }).ToArray()), "case-insensitive duplicate rejected");
            ExpectThrows(() => ValidatePrefixFreedom(RngSalts.Families.Concat(new[] { "PHASE2-MAST-EXTRA-" }).ToArray()), "new prefix pair rejected");
        });
    }

    private static void ValidateNonEmptyAndDistinct(IReadOnlyList<string> families)
    {
        var seenOrdinal = new HashSet<string>(StringComparer.Ordinal);
        var seenIgnoreCase = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var value in families)
        {
            T.True(!string.IsNullOrEmpty(value), "RNG salt must not be empty");
            T.True(!string.IsNullOrWhiteSpace(value), "RNG salt must not be whitespace");
            T.Eq(value.Trim(), value, "RNG salt must have no leading/trailing whitespace: " + value);
            T.True(seenOrdinal.Add(value), "duplicate RNG salt (ordinal): " + value);
            T.True(seenIgnoreCase.Add(value), "duplicate RNG salt (case-insensitive): " + value);
        }
    }

    private static void ValidatePrefixFreedom(IReadOnlyList<string> families)
    {
        var allowSet = new HashSet<(string Prefix, string Longer)>(FrozenNestedPrefixes);
        var observed = new HashSet<(string Prefix, string Longer)>();
        for (int i = 0; i < families.Count; i++)
        {
            for (int j = 0; j < families.Count; j++)
            {
                if (i == j) continue;
                string a = families[i];
                string b = families[j];
                if (b.StartsWith(a, StringComparison.OrdinalIgnoreCase))
                {
                    var pair = (a, b);
                    T.True(allowSet.Contains(pair), $"forbidden prefix pair in RngSalts.Families: \"{a}\" is a prefix of \"{b}\"");
                    observed.Add(pair);
                }
            }
        }
        T.Eq(allowSet.Count, observed.Count, "observed nested prefix pairs must match the exact three allow-listed pairs");
    }

    private static void ExpectThrows(Action action, string label)
    {
        try
        {
            action();
        }
        catch (Exception)
        {
            return;
        }
        throw new Exception("expected validation failure: " + label);
    }
}

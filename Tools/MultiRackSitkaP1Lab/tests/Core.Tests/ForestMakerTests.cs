using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Phase2Support;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// CCF Multi 2D — Phase 2A-1 Forest Maker verification suite (SPEC-multi-2d-phase2a-forest-maker-V1 §5, §9 P2A-01..P2A-06
/// and Economy interop 2A1-06).
/// </summary>
internal static class ForestMakerTests
{
    // Anchor 10: fm-064-sitka-edge-broadleaves-v1 / mad-fm-064-sitka-edge-broadleaves-v1 canonical manifest body SHA-256.
    // Captured once in Phase 2A-1 and pinned here (not added to build/2d-gate/frozen-anchors.json, which locks 1..9).
    internal const string PinnedPrimaryPresetContentHash = "d79fabccf3a9447666c436a08c04895c58934fde19c5b5aaf072103612ae4fde";

    // Anchor 11: fm-064-pure-sitka-v1 / mad-fm-064-pure-sitka-v1 canonical manifest body SHA-256.
    // Captured once in Phase 2A-1 and pinned here (not added to build/2d-gate/frozen-anchors.json, which locks 1..9).
    internal const string PinnedPureSitkaPresetContentHash = "de0eb3f63d13423336348d3cd4f398b9c4f6ce23d77578e902c25960265a5945";

    private static readonly Regex FourDigitTreeId = new("^MA-[A-H]-[0-9]{4}$", RegexOptions.CultureInvariant);

    internal static void Register()
    {
        // 1. P2A-01: bounds table, exact cap 3,000 vs 3,001, all typed refusals, canonical encode/decode
        T.Add("forestmaker P2A-01a: recipe bounds table (min/max/min-1/max+1), exact 3000 cap, typed refusals and canonical decode", () =>
        {
            var customBase = CustomRecipe(widthM: 80, lengthM: 80, laneCount: 5, startAgeYears: 20, stemsPerHa: 2_100, seed: 42_001);
            customBase.RequireValid();

            // Generator ID & version
            T.Refuses("ForestMakerGeneratorUnsupported", () => (customBase with { GeneratorId = "other-gen" }).RequireValid());
            T.Refuses("ForestMakerGeneratorUnsupported", () => (customBase with { GeneratorVersion = 2 }).RequireValid());
            T.Refuses("ForestMakerGeneratorUnsupported", () => (customBase with { GeneratorVersion = 0 }).RequireValid());

            // Preset unknown & preset mismatch
            T.Refuses("ForestMakerPresetUnknown", () => (customBase with { PresetId = "fm-does-not-exist-v1" }).RequireValid());
            T.Refuses("ForestMakerPresetUnknown", () => ForestMakerPresets.ByPresetId("fm-does-not-exist-v1"));
            T.Refuses("ForestMakerPresetMismatch", () => (ForestMakerPresets.SitkaEdgeBroadleaves064 with { Seed = 123 }).RequireValid());
            T.Refuses("ForestMakerPresetMismatch", () => (ForestMakerPresets.PureSitka064 with { WidthM = 90 }).RequireValid());
            T.Eq(ForestMakerPresets.SitkaEdgeBroadleaves064, ForestMakerPresets.ByPresetId(ForestMakerPresets.PrimaryPresetId), "primary preset lookup");
            T.Eq(ForestMakerPresets.PureSitka064, ForestMakerPresets.ByPresetId(ForestMakerPresets.PureSitkaPresetId), "pure sitka preset lookup");

            // WidthM: min=30, max=120, min-1=29, max+1=121 (use LaneCount=2 & StemsPerHa=500 so cap & alignment stay clear)
            var dimBase = CustomRecipe(widthM: 60, lengthM: 60, laneCount: 2, startAgeYears: 20, stemsPerHa: 500, seed: 100);
            (dimBase with { WidthM = ForestMakerBounds.MinSideM }).RequireValid();
            (dimBase with { WidthM = ForestMakerBounds.MaxSideM }).RequireValid();
            T.Refuses("ForestMakerWidthOutOfRange", () => (dimBase with { WidthM = ForestMakerBounds.MinSideM - 1 }).RequireValid());
            T.Refuses("ForestMakerWidthOutOfRange", () => (dimBase with { WidthM = ForestMakerBounds.MaxSideM + 1 }).RequireValid());

            // LengthM: min=30, max=120, min-1=29, max+1=121
            (dimBase with { LengthM = ForestMakerBounds.MinSideM }).RequireValid();
            (dimBase with { LengthM = ForestMakerBounds.MaxSideM }).RequireValid();
            T.Refuses("ForestMakerLengthOutOfRange", () => (dimBase with { LengthM = ForestMakerBounds.MinSideM - 1 }).RequireValid());
            T.Refuses("ForestMakerLengthOutOfRange", () => (dimBase with { LengthM = ForestMakerBounds.MaxSideM + 1 }).RequireValid());

            // LaneCount: min=2, max=8, min-1=1, max+1=9 (use WidthM=120 so 8 lanes = 15 m, grid-aligned)
            var laneBase = CustomRecipe(widthM: 120, lengthM: 60, laneCount: 4, startAgeYears: 20, stemsPerHa: 500, seed: 101);
            (laneBase with { LaneCount = ForestMakerBounds.MinLanes }).RequireValid();
            (laneBase with { LaneCount = ForestMakerBounds.MaxLanes }).RequireValid();
            T.Refuses("ForestMakerLaneCountOutOfRange", () => (laneBase with { LaneCount = ForestMakerBounds.MinLanes - 1 }).RequireValid());
            T.Refuses("ForestMakerLaneCountOutOfRange", () => (laneBase with { LaneCount = ForestMakerBounds.MaxLanes + 1 }).RequireValid());

            // StartAgeYears: min=10, max=60, min-1=9, max+1=61
            (dimBase with { StartAgeYears = ForestMakerBounds.MinAgeYears }).RequireValid();
            (dimBase with { StartAgeYears = ForestMakerBounds.MaxAgeYears }).RequireValid();
            T.Refuses("ForestMakerAgeOutOfRange", () => (dimBase with { StartAgeYears = ForestMakerBounds.MinAgeYears - 1 }).RequireValid());
            T.Refuses("ForestMakerAgeOutOfRange", () => (dimBase with { StartAgeYears = ForestMakerBounds.MaxAgeYears + 1 }).RequireValid());

            // StemsPerHa: min=500, max=3000, min-1=499, max+1=3001
            (dimBase with { StemsPerHa = ForestMakerBounds.MinStemsPerHa }).RequireValid();
            (dimBase with { StemsPerHa = ForestMakerBounds.MaxStemsPerHa }).RequireValid();
            T.Refuses("ForestMakerDensityOutOfRange", () => (dimBase with { StemsPerHa = ForestMakerBounds.MinStemsPerHa - 1 }).RequireValid());
            T.Refuses("ForestMakerDensityOutOfRange", () => (dimBase with { StemsPerHa = ForestMakerBounds.MaxStemsPerHa + 1 }).RequireValid());

            // Seed: min=1, max=100_000_000, min-1=0, max+1=100_000_001
            (dimBase with { Seed = ForestMakerBounds.MinSeed }).RequireValid();
            (dimBase with { Seed = ForestMakerBounds.MaxSeed }).RequireValid();
            T.Refuses("ForestMakerSeedOutOfRange", () => (dimBase with { Seed = ForestMakerBounds.MinSeed - 1 }).RequireValid());
            T.Refuses("ForestMakerSeedOutOfRange", () => (dimBase with { Seed = ForestMakerBounds.MaxSeed + 1 }).RequireValid());

            // SpeciesMix & Oak/Beech percentages
            T.Refuses("ForestMakerSpeciesMixInvalid", () => (dimBase with { SpeciesMix = (ForestMakerSpeciesMix)99 }).RequireValid());
            T.Refuses("ForestMakerSpeciesMixInvalid", () => (dimBase with { SpeciesMix = ForestMakerSpeciesMix.PureSitka, OakPercent = 10 }).RequireValid());
            T.Refuses("ForestMakerSpeciesMixInvalid", () => (dimBase with { SpeciesMix = ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves, BeechPercent = 5 }).RequireValid());
            T.Refuses("ForestMakerSpeciesMixInvalid", () => (dimBase with { SpeciesMix = ForestMakerSpeciesMix.CustomSplit, OakPercent = -1, BeechPercent = 0 }).RequireValid());
            T.Refuses("ForestMakerSpeciesMixInvalid", () => (dimBase with { SpeciesMix = ForestMakerSpeciesMix.CustomSplit, OakPercent = 0, BeechPercent = 101 }).RequireValid());
            T.Refuses("ForestMakerSpeciesMixInvalid", () => (dimBase with { SpeciesMix = ForestMakerSpeciesMix.CustomSplit, OakPercent = 60, BeechPercent = 41 }).RequireValid());
            (dimBase with { SpeciesMix = ForestMakerSpeciesMix.CustomSplit, OakPercent = 60, BeechPercent = 40 }).RequireValid();
            (dimBase with { SpeciesMix = ForestMakerSpeciesMix.CustomSplit, OakPercent = 0, BeechPercent = 0 }).RequireValid();

            // Hard cap: exact 3,000 accepted, exact 3,001 refused, and 120x120 @ 3,000/ha = 4,320 refused
            var exact3000 = CustomRecipe(widthM: 100, lengthM: 100, laneCount: 5, startAgeYears: 20, stemsPerHa: 3_000, seed: 3_000);
            T.Eq(3_000, exact3000.TotalStems(), "100x100 @ 3000/ha is exact 3,000");
            var res3000 = ForestMakerGenerator.Generate(exact3000);
            T.Eq(3_000, res3000.TotalStems, "3,000-stem stand generates");
            T.Eq(3_000, res3000.Experiment.Areas.Sum(a => a.Trees.Count), "3,000 living stems generated");

            var rounded3000 = CustomRecipe(widthM: 101, lengthM: 100, laneCount: 2, startAgeYears: 20, stemsPerHa: 2_970, seed: 3_001);
            T.Eq(3_000, rounded3000.TotalStems(), "101x100 @ 2970/ha rounds half-up to 3,000");
            rounded3000.RequireValid();

            var exact3001 = CustomRecipe(widthM: 101, lengthM: 100, laneCount: 2, startAgeYears: 20, stemsPerHa: 2_971, seed: 3_002);
            T.Eq(3_001, exact3001.TotalStems(), "101x100 @ 2971/ha rounds half-up to 3,001");
            try
            {
                ForestMakerGenerator.Generate(exact3001);
                throw new Exception("expected ForestMakerStemCapExceeded for 3,001 stems");
            }
            catch (P1Refusal e)
            {
                T.Eq("ForestMakerStemCapExceeded", e.Code, "3,001 cap refusal code");
                T.True(e.Message.Contains("3001", StringComparison.Ordinal), "3,001 cap refusal message reports computed count");
            }

            var cap4320 = CustomRecipe(widthM: 120, lengthM: 120, laneCount: 4, startAgeYears: 20, stemsPerHa: 3_000, seed: 4_320);
            T.Eq(4_320, cap4320.TotalStems(), "120x120 @ 3000/ha = 4,320");
            try
            {
                ForestMakerGenerator.Generate(cap4320);
                throw new Exception("expected ForestMakerStemCapExceeded for 4,320 stems");
            }
            catch (P1Refusal e)
            {
                T.Eq("ForestMakerStemCapExceeded", e.Code, "4,320 cap refusal code");
                T.True(e.Message.Contains("4320", StringComparison.Ordinal), "4,320 cap refusal message reports computed count");
            }

            // Grid alignment guard: W=30, N=8 hits 2.5 m centre at x=3,750 mm; W=31, N=8 hits 0.25 m centre at x=3,875 mm
            T.Refuses("ForestMakerLaneGridAlignment", () => CustomRecipe(widthM: 30, lengthM: 30, laneCount: 8, startAgeYears: 20, stemsPerHa: 1_000, seed: 7).RequireValid());
            T.Refuses("ForestMakerLaneGridAlignment", () => CustomRecipe(widthM: 31, lengthM: 30, laneCount: 8, startAgeYears: 20, stemsPerHa: 1_000, seed: 7).RequireValid());

            // LaneEmpty & InfeasibleStocking refusals (direct invariant checks on real code paths)
            T.Refuses("ForestMakerLaneEmpty", () => ForestMakerGenerator.RequireNonEmptyQuotas(new[] { 10, 0, 10 }));
            var tinyPolygon = Array.AsReadOnly(new[]
            {
                new MillimetrePoint(0, 0), new MillimetrePoint(1_000, 0),
                new MillimetrePoint(1_000, 1_000), new MillimetrePoint(0, 1_000),
            });
            var tinyLane = new ForestMakerGenerator.LaneBounds(0, "MA-A", "Lane A", 0, 1_000, 1_000, tinyPolygon);
            T.Refuses("ForestMakerInfeasibleStocking", () => ForestMakerGenerator.SelectExact(1, tinyLane, 10, 10, Array.Empty<ForestMakerGenerator.PlacedStem>()));

            // Canonical bytes, hash stability, round-trip and non-canonical refusals
            byte[] canonA = ForestMakerPresets.SitkaEdgeBroadleaves064.CanonicalBytes();
            byte[] canonB = ForestMakerPresets.SitkaEdgeBroadleaves064.CanonicalBytes();
            T.True(canonA.AsSpan().SequenceEqual(canonB), "canonical recipe bytes repeat identically");
            T.Eq(ForestMakerPresets.SitkaEdgeBroadleaves064.RecipeHash(), DerivedManifest.Sha256Hex(canonA), "RecipeHash is SHA-256 of canonical bytes");
            T.Eq(ForestMakerPresets.SitkaEdgeBroadleaves064, ForestMakerRecipe.Decode(canonA), "canonical preset round-trips");

            byte[] customBytes = customBase.CanonicalBytes();
            T.True(Encoding.UTF8.GetString(customBytes).Contains("\"presetId\":null,", StringComparison.Ordinal), "custom recipe encodes presetId as JSON null");
            T.Eq(customBase, ForestMakerRecipe.Decode(customBytes), "canonical custom recipe round-trips");

            // Whitespace, trailing newline, reordered keys -> ForestMakerRecipeNotCanonical
            string customJson = Encoding.UTF8.GetString(customBytes);
            T.Refuses("ForestMakerRecipeNotCanonical", () => ForestMakerRecipe.Decode(Encoding.UTF8.GetBytes(customJson.Replace(":", ": ", StringComparison.Ordinal))));
            T.Refuses("ForestMakerRecipeNotCanonical", () => ForestMakerRecipe.Decode(customBytes.Concat(new byte[] { (byte)'\n' }).ToArray()));
            string reordered = customJson.Replace("\"widthM\":80,\"lengthM\":80", "\"lengthM\":80,\"widthM\":80", StringComparison.Ordinal);
            T.Refuses("ForestMakerRecipeNotCanonical", () => ForestMakerRecipe.Decode(Encoding.UTF8.GetBytes(reordered)));
            T.Refuses("ForestMakerRecipeNotCanonical", () => ForestMakerRecipe.Decode(StrictJson.Parse(Encoding.UTF8.GetBytes(reordered))));
        });

        // 2. P2A-01 corner matrix + CustomSplit + custom ages
        T.Add("forestmaker P2A-01b: corner matrix (64 cases), CustomSplit and custom cohort ages", () =>
        {
            int[] sides = { ForestMakerBounds.MinSideM, ForestMakerBounds.MaxSideM };
            int[] lanes = { ForestMakerBounds.MinLanes, ForestMakerBounds.MaxLanes };
            int[] ages = { ForestMakerBounds.MinAgeYears, ForestMakerBounds.MaxAgeYears };
            int[] densities = { ForestMakerBounds.MinStemsPerHa, ForestMakerBounds.MaxStemsPerHa };
            ForestMakerSpeciesMix[] mixes = { ForestMakerSpeciesMix.PureSitka, ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves };

            int generated = 0, refused = 0;
            foreach (int w in sides)
            foreach (int l in sides)
            foreach (int n in lanes)
            foreach (int age in ages)
            foreach (int dens in densities)
            foreach (var mix in mixes)
            {
                var r = CustomRecipe(w, l, n, age, dens, 77_001, mix);
                try
                {
                    var res = ForestMakerGenerator.Generate(r);
                    res.Experiment.Validate();
                    T.Eq(r.TotalStems(), res.TotalStems, "corner total stems matches formula");
                    generated++;
                }
                catch (P1Refusal)
                {
                    refused++;
                }
            }
            T.Eq(64, generated + refused, "all 64 corner cases handled with zero unexpected exceptions");
            T.True(generated > 0 && refused > 0, "corner matrix covers both valid generation and typed refusals");

            // CustomSplit: 25% Oak, 25% Beech, 50% Sitka at custom age 35
            var split = CustomRecipe(60, 60, 3, 35, 1_500, 55_555, ForestMakerSpeciesMix.CustomSplit, oakPercent: 25, beechPercent: 25);
            var splitRes = ForestMakerGenerator.Generate(split);
            splitRes.Experiment.Validate();
            T.True(splitRes.Experiment.ExperimentId.StartsWith("mad-fm-custom-", StringComparison.Ordinal), "custom experiment ID prefix");
            T.Eq(14 + 16, splitRes.Experiment.ExperimentId.Length, "custom experiment ID length");
            T.True(splitRes.Lanes.Sum(x => x.OakCount) > 0, "CustomSplit creates Oak stems");
            T.True(splitRes.Lanes.Sum(x => x.BeechCount) > 0, "CustomSplit creates Beech stems");
            T.True(splitRes.Lanes.Sum(x => x.SitkaCount) > 0, "CustomSplit creates Sitka stems");
            T.True(splitRes.Experiment.Areas.SelectMany(a => a.Trees).All(t => t.AgeYears == 35), "CustomSplit broadleaves share cohort age");

            // 100% Oak CustomSplit
            var allOak = CustomRecipe(40, 40, 2, 47, 1_000, 12_345, ForestMakerSpeciesMix.CustomSplit, oakPercent: 100, beechPercent: 0);
            var allOakRes = ForestMakerGenerator.Generate(allOak);
            T.Eq(allOakRes.TotalStems, allOakRes.Lanes.Sum(x => x.OakCount), "100% Oak split assigns every stem to Oak");
            T.Eq(0, allOakRes.Lanes.Sum(x => x.SitkaCount), "100% Oak split has 0 Sitka");
            T.Eq(allOakRes.TotalStems, allOakRes.Options.GenesisSpecies.Count, "every stem in GenesisSpecies");

            // Q1 piecewise-linear age anchors exact
            T.Eq(70L, ForestMakerGenerator.MeanDbhForAgeMm(10), "age 10 anchor");
            T.Eq(160L, ForestMakerGenerator.MeanDbhForAgeMm(20), "age 20 anchor");
            T.Eq(240L, ForestMakerGenerator.MeanDbhForAgeMm(30), "age 30 anchor");
            T.Eq(300L, ForestMakerGenerator.MeanDbhForAgeMm(40), "age 40 anchor");
            T.Eq(345L, ForestMakerGenerator.MeanDbhForAgeMm(47), "age 47 anchor");
            T.Eq(400L, ForestMakerGenerator.MeanDbhForAgeMm(60), "age 60 anchor");
        });

        // 3. P2A-02: Determinism, seed sensitivity, validity, margins, 4-digit IDs, lane seeds, 5-year repeat hash, RngSalts & MA-1 invariance
        T.Add("forestmaker P2A-02: determinism, seed sensitivity, geometry margins, 4-digit IDs, 5-year repeat and MA-1 invariance", () =>
        {
            var r1 = ForestMakerGenerator.Generate(ForestMakerPresets.SitkaEdgeBroadleaves064);
            var r2 = ForestMakerGenerator.Generate(ForestMakerPresets.SitkaEdgeBroadleaves064);
            T.True(r1.Experiment.ManifestFileBytes().AsSpan().SequenceEqual(r2.Experiment.ManifestFileBytes()), "primary preset manifest bytes identical");
            T.Eq(r1.ContentHash, r2.ContentHash, "primary preset content hash identical");
            T.Eq(r1.RecipeHash, r2.RecipeHash, "primary preset recipe hash identical");

            // Different seed -> different positions, same stem counts
            var customSeedA = CustomRecipe(80, 80, 5, 20, 2_100, 6_400_101, ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves);
            var customSeedB = customSeedA with { Seed = 6_400_999 };
            var resA = ForestMakerGenerator.Generate(customSeedA);
            var resB = ForestMakerGenerator.Generate(customSeedB);
            T.Eq(resA.TotalStems, resB.TotalStems, "different seed preserves total stem count");
            T.True(resA.Lanes.Select(l => l.TotalStems).SequenceEqual(resB.Lanes.Select(l => l.TotalStems)), "different seed preserves per-lane counts");
            var posA = resA.Experiment.Areas[1].Trees.Select(t => (t.XMm, t.YMm)).ToArray();
            var posB = resB.Experiment.Areas[1].Trees.Select(t => (t.XMm, t.YMm)).ToArray();
            T.True(!posA.SequenceEqual(posB), "different seed produces different stem positions");

            // Validate geometry, >= 500 mm edge margin, unique positions, 4-digit ordinal IDs, unique lane seeds
            foreach (var preset in ForestMakerPresets.All)
            {
                var gen = ForestMakerGenerator.Generate(preset);
                gen.Experiment.Validate();

                var allPositions = new HashSet<(long, long)>();
                var laneSeeds = new HashSet<int>();
                foreach (var area in gen.Experiment.Areas)
                {
                    T.True(area.SimulationSeed != preset.Seed, area.AreaId + " simulation seed differs from generator seed");
                    T.True(laneSeeds.Add(area.SimulationSeed), area.AreaId + " simulation seed unique");

                    string? prevId = null;
                    for (int i = 0; i < area.Trees.Count; i++)
                    {
                        var tree = area.Trees[i];
                        T.True(FourDigitTreeId.IsMatch(tree.TreeId), "4-digit ID format: " + tree.TreeId);
                        string expectedId = area.AreaId + "-" + (i + 1).ToString("D4", CultureInfo.InvariantCulture);
                        T.Eq(expectedId, tree.TreeId, "ordinal 4-digit ID sequence");
                        T.True(prevId is null || string.CompareOrdinal(prevId, tree.TreeId) < 0, "ordinal ID order");
                        prevId = tree.TreeId;

                        var pt = new MillimetrePoint(tree.XMm, tree.YMm);
                        T.True(MultiAreaGeometry.StrictlyInside(area.PolygonMm, pt), "strictly inside lane: " + tree.TreeId);
                        T.True(MultiAreaGeometry.InsideWithMargin(area.PolygonMm, pt, ForestMakerBounds.EdgeMarginMm), ">= 500 mm from lane edge: " + tree.TreeId);
                        T.True(allPositions.Add((tree.XMm, tree.YMm)), "globally unique position: " + tree.TreeId);
                    }
                }

                // 5-year simulation fingerprint identical on repeat
                string simHash1 = FiveYearFingerprint(preset);
                string simHash2 = FiveYearFingerprint(preset);
                T.Eq(simHash1, simHash2, preset.PresetId + " 5-year simulation hash repeats identically");
                Console.WriteLine($"  P2A-02 {preset.PresetId} 5-year hash {simHash1}");
            }

            // RngSalts registration check
            T.True(RngSalts.Families.Contains(RngSalts.ForestMaker, StringComparer.Ordinal), "RngSalts.ForestMaker registered in Families");
            T.True(ForestMakerGenerator.StreamNamespaceKey("test").StartsWith(RngSalts.ForestMaker, StringComparison.Ordinal), "StreamNamespaceKey uses RngSalts.ForestMaker");

            // MA-1 and MA-1-MIXED manifest bytes remain unchanged (GeneratorNoteOverride null-preserving)
            var ma1 = MultiAreaGenerator.Generate();
            T.Eq("c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95", ma1.ContentHash(), "MA-1 content hash unchanged");
            T.True(ma1.GeneratorNoteOverride is null, "MA-1 GeneratorNoteOverride is null");
            var (mixed, _, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            T.True(mixed.GeneratorNoteOverride is null, "MA-1-MIXED GeneratorNoteOverride is null");
        });

        // 4. P2A-03 & P2A-04: Both canonical 0.64 ha presets exact + anchors 10 & 11
        T.Add("forestmaker P2A-03 & P2A-04: primary and pure-Sitka 0.64 ha presets exact and anchors 10-11 pinned", () =>
        {
            long expectedTwiceLaneMm2 = 2L * 16_000L * 80_000L;
            float expectedLaneHa = MultiAreaGeometry.Hectares(expectedTwiceLaneMm2);
            int[] expectedCellCounts = { 192, 224, 192, 224, 192 };

            // --- Primary preset: fm-064-sitka-edge-broadleaves-v1 ---
            var primary = ForestMakerGenerator.Generate(ForestMakerPresets.SitkaEdgeBroadleaves064);
            T.Eq(ForestMakerPresets.PrimaryExperimentId, primary.Experiment.ExperimentId, "primary experiment ID");
            T.Eq(ForestMakerBounds.Disclosure, primary.Experiment.DisclosureOverride, "primary disclosure override");
            T.Eq(ForestMakerBounds.GeneratorNote, primary.Experiment.GeneratorNoteOverride, "primary generator note override");
            T.Eq(5, primary.Experiment.Areas.Count, "primary 5 lanes");
            T.Eq(1_344, primary.TotalStems, "primary 1,344 total stems");
            T.Eq(1_336, primary.Lanes.Sum(l => l.SitkaCount), "primary 1,336 Sitka");
            T.Eq(4, primary.Lanes.Sum(l => l.OakCount), "primary 4 Oak");
            T.Eq(4, primary.Lanes.Sum(l => l.BeechCount), "primary 4 Beech");
            T.Eq(8, primary.Options.GenesisSpecies.Count, "primary 8 broadleaves in GenesisSpecies");

            // Check exact lane geometry & per-lane species breakdown
            (string AreaId, long X0, long X1, int Sitka, int Oak, int Beech)[] expectedPrimaryLanes =
            {
                ("MA-A", 0, 16_000, 265, 2, 2),
                ("MA-B", 16_000, 32_000, 269, 0, 0),
                ("MA-C", 32_000, 48_000, 269, 0, 0),
                ("MA-D", 48_000, 64_000, 269, 0, 0),
                ("MA-E", 64_000, 80_000, 264, 2, 2),
            };
            for (int i = 0; i < 5; i++)
            {
                var expLane = expectedPrimaryLanes[i];
                var area = primary.Experiment.Areas[i];
                var sum = primary.Lanes[i];
                T.Eq(expLane.AreaId, area.AreaId, "lane ID");
                T.Eq(expectedLaneHa, area.AreaHa, area.AreaId + " AreaHa == 0.128");
                T.Eq("1280", area.AreaM2Exact, area.AreaId + " exact 1280 m2");
                T.Eq(expLane.X0, area.PolygonMm[0].XMm, area.AreaId + " x0");
                T.Eq(expLane.X1, area.PolygonMm[1].XMm, area.AreaId + " x1");
                T.Eq(0L, area.PolygonMm[0].YMm, area.AreaId + " y0");
                T.Eq(80_000L, area.PolygonMm[2].YMm, area.AreaId + " y1");
                T.Eq(expLane.Sitka, sum.SitkaCount, area.AreaId + " Sitka count");
                T.Eq(expLane.Oak, sum.OakCount, area.AreaId + " Oak count");
                T.Eq(expLane.Beech, sum.BeechCount, area.AreaId + " Beech count");
                T.Eq(expLane.Sitka + expLane.Oak + expLane.Beech, area.Trees.Count, area.AreaId + " total count");
            }

            // Check exact broadleaf positions, alternating species, ages, and >= 2,000 mm Sitka exclusion
            var allPrimaryTrees = primary.Experiment.Areas.SelectMany(a => a.Trees).ToArray();
            var broadleaves = allPrimaryTrees.Where(t => primary.Options.GenesisSpecies.ContainsKey(t.TreeId)).ToArray();
            var sitkas = allPrimaryTrees.Where(t => !primary.Options.GenesisSpecies.ContainsKey(t.TreeId)).ToArray();
            T.Eq(8, broadleaves.Length, "8 broadleaf stems");
            T.Eq(1_336, sitkas.Length, "1,336 Sitka stems");
            T.True(broadleaves.All(t => t.AgeYears == 60), "all 8 edge broadleaves are age 60");
            T.True(sitkas.All(t => t.AgeYears == 20), "all 1,336 Sitka are age 20");

            var westBroadleaves = primary.Experiment.Areas[0].Trees
                .Where(t => primary.Options.GenesisSpecies.ContainsKey(t.TreeId))
                .OrderBy(t => t.YMm).ToArray();
            T.Eq(4, westBroadleaves.Length, "4 west edge broadleaves in MA-A");
            (long X, long Y, string Sp)[] expectedWest =
            {
                (1_250, 10_000, Phase2Species.OakId),
                (1_250, 30_000, Phase2Species.BeechId),
                (1_250, 50_000, Phase2Species.OakId),
                (1_250, 70_000, Phase2Species.BeechId),
            };
            for (int k = 0; k < 4; k++)
            {
                T.Eq(expectedWest[k].X, westBroadleaves[k].XMm, $"west broadleaf {k} x");
                T.Eq(expectedWest[k].Y, westBroadleaves[k].YMm, $"west broadleaf {k} y");
                T.Eq(expectedWest[k].Sp, primary.Options.GenesisSpecies[westBroadleaves[k].TreeId], $"west broadleaf {k} species");
            }

            var eastBroadleaves = primary.Experiment.Areas[4].Trees
                .Where(t => primary.Options.GenesisSpecies.ContainsKey(t.TreeId))
                .OrderBy(t => t.YMm).ToArray();
            T.Eq(4, eastBroadleaves.Length, "4 east edge broadleaves in MA-E");
            (long X, long Y, string Sp)[] expectedEast =
            {
                (78_750, 10_000, Phase2Species.BeechId),
                (78_750, 30_000, Phase2Species.OakId),
                (78_750, 50_000, Phase2Species.BeechId),
                (78_750, 70_000, Phase2Species.OakId),
            };
            for (int k = 0; k < 4; k++)
            {
                T.Eq(expectedEast[k].X, eastBroadleaves[k].XMm, $"east broadleaf {k} x");
                T.Eq(expectedEast[k].Y, eastBroadleaves[k].YMm, $"east broadleaf {k} y");
                T.Eq(expectedEast[k].Sp, primary.Options.GenesisSpecies[eastBroadleaves[k].TreeId], $"east broadleaf {k} species");
            }

            long minExclusionSq = ForestMakerBounds.ExclusionRadiusMm * ForestMakerBounds.ExclusionRadiusMm;
            foreach (var s in sitkas)
                foreach (var b in broadleaves)
                {
                    long dx = s.XMm - b.XMm;
                    long dy = s.YMm - b.YMm;
                    T.True(dx * dx + dy * dy >= minExclusionSq, $"Sitka {s.TreeId} >= 2000 mm from broadleaf {b.TreeId}");
                }

            // Verify 1,024 cells of 2.5 m with 192/224/192/224/192 per lane on primary
            var primarySession = new MultiAreaContinuation(primary.Experiment, MultiAreaModelSelection.Phase2, null, null, primary.Options);
            var primaryS5 = primarySession.ReadStage5View();
            T.Eq(1_024, primaryS5.Areas.Sum(a => a.Cells.Count), "primary 1,024 cells total");
            for (int i = 0; i < 5; i++)
                T.Eq(expectedCellCounts[i], primaryS5.Areas[i].Cells.Count, $"primary {primaryS5.Areas[i].AreaId} cell count");

            Console.WriteLine($"  P2A-03 Anchor 10 ({ForestMakerPresets.PrimaryPresetId}) recipeHash {primary.RecipeHash} contentHash {primary.ContentHash}");
            T.Eq(PinnedPrimaryPresetContentHash, primary.ContentHash, "Anchor 10 primary preset content hash");

            // --- Pure-Sitka preset: fm-064-pure-sitka-v1 ---
            var pure = ForestMakerGenerator.Generate(ForestMakerPresets.PureSitka064);
            T.Eq(ForestMakerPresets.PureSitkaExperimentId, pure.Experiment.ExperimentId, "pure-Sitka experiment ID");
            T.Eq(5, pure.Experiment.Areas.Count, "pure-Sitka 5 lanes");
            T.Eq(1_344, pure.TotalStems, "pure-Sitka 1,344 stems");
            T.Eq(1_344, pure.Lanes.Sum(l => l.SitkaCount), "pure-Sitka 1,344 Sitka");
            T.Eq(0, pure.Options.GenesisSpecies.Count, "pure-Sitka empty GenesisSpecies");
            int[] expectedPureQuotas = { 269, 269, 269, 269, 268 };
            for (int i = 0; i < 5; i++)
            {
                T.Eq(expectedPureQuotas[i], pure.Experiment.Areas[i].Trees.Count, $"pure-Sitka lane {i} count");
                T.True(pure.Experiment.Areas[i].Trees.All(t => t.AgeYears == 20), $"pure-Sitka lane {i} all age 20");
            }
            var pureSession = new MultiAreaContinuation(pure.Experiment, MultiAreaModelSelection.Phase2, null, null, pure.Options);
            var pureS5 = pureSession.ReadStage5View();
            T.Eq(1_024, pureS5.Areas.Sum(a => a.Cells.Count), "pure-Sitka 1,024 cells total");
            for (int i = 0; i < 5; i++)
                T.Eq(expectedCellCounts[i], pureS5.Areas[i].Cells.Count, $"pure-Sitka {pureS5.Areas[i].AreaId} cell count");

            Console.WriteLine($"  P2A-04 Anchor 11 ({ForestMakerPresets.PureSitkaPresetId}) recipeHash {pure.RecipeHash} contentHash {pure.ContentHash}");
            T.Eq(PinnedPureSitkaPresetContentHash, pure.ContentHash, "Anchor 11 pure-Sitka preset content hash");
        });

        // 5. P2A-05: Existing species constants respected (maturity, C2 broadleaf crowns, §5.3 DBH/height)
        T.Add("forestmaker P2A-05: species constants respected (maturity, C2 broadleaf genesis crowns, starting sizes)", () =>
        {
            // Maturity asserted from constants, not magic literals
            T.Eq(1f, Phase2SeedRain.Maturity(Phase2Species.Oak, (int)Phase2Species.Oak.MaturityFullYears), "Oak mature at MaturityFullYears");
            T.Eq(1f, Phase2SeedRain.Maturity(Phase2Species.Beech, (int)Phase2Species.Beech.MaturityFullYears), "Beech mature at MaturityFullYears");
            T.Eq((int)Phase2Species.Oak.MaturityFullYears, ForestMakerBounds.EdgeBroadleafAgeYears, "edge broadleaf age matches Oak MaturityFullYears");
            T.Eq((int)Phase2Species.Beech.MaturityFullYears, ForestMakerBounds.EdgeBroadleafAgeYears, "edge broadleaf age matches Beech MaturityFullYears");
            T.Eq(0f, Stage5SeedRain.Maturity((int)Stage5Constants.MaturityOnsetYears), "Sitka maturity 0 at MaturityOnsetYears");
            T.Eq((int)Stage5Constants.MaturityOnsetYears, ForestMakerPresets.SitkaEdgeBroadleaves064.StartAgeYears, "preset Sitka start age matches MaturityOnsetYears");

            // Broadleaf genesis crowns equal Phase2Crown.TargetRadiusM over connected woodland
            var (gen, session) = ForestMakerGenerator.CreateSession(ForestMakerPresets.SitkaEdgeBroadleaves064);
            var view = session.ReadView();
            var woodland = CrownModel.Woodland(gen.Experiment.Areas.SelectMany(MultiAreaExperiment.GenesisTrees));

            int verifiedBroadleaves = 0;
            foreach (var area in view.Areas)
            {
                foreach (var tree in area.Trees)
                {
                    if (!gen.Options.GenesisSpecies.TryGetValue(tree.TreeId, out var speciesId)) continue;
                    var species = Phase2Species.BroadleafOrNull(speciesId)!;
                    float ci0 = CrownModel.WoodlandCompetition(tree, woodland).Ci;
                    float expectedCrown = Phase2Crown.TargetRadiusM(species, tree.DbhCm, ci0);
                    T.Eq(F32.Bits(expectedCrown), F32.Bits(tree.CrownRadiusM), $"broadleaf {tree.TreeId} C2 genesis crown");
                    verifiedBroadleaves++;
                }
            }
            T.Eq(8, verifiedBroadleaves, "verified all 8 broadleaf genesis crowns");

            // Starting DBH and height formulas match spec §5.3
            foreach (var area in gen.Experiment.Areas)
            {
                foreach (var t in area.Trees)
                {
                    if (gen.Options.GenesisSpecies.ContainsKey(t.TreeId))
                    {
                        T.True(t.DbhMm is >= 220 and <= 480, "edge broadleaf DBH in [220, 480] mm");
                        T.Eq(27_200L + 20L * (t.DbhMm - 345L), t.HeightMm, "edge broadleaf height formula");
                    }
                    else
                    {
                        T.True(t.DbhMm is >= 80 and <= 240, "age-20 Sitka DBH in [80, 240] mm");
                        T.Eq(Math.Max(3_500L, 2_000L + 62L * t.DbhMm), t.HeightMm, "age-20 Sitka height formula");
                    }
                }
            }
        });

        // 6. P2A-06: Cross-lane ecology on 0.64 ha (Hegyi CI, per-lane RD, cross-lane broadleaf seed vs pure Sitka, no outside leakage, 15-year advance)
        T.Add("forestmaker P2A-06: connected 5-lane ecology (Hegyi CI, per-lane RD, cross-lane seed, no outside leakage, 15y advance)", () =>
        {
            var (primaryGen, primary) = ForestMakerGenerator.CreateSession(ForestMakerPresets.SitkaEdgeBroadleaves064);
            var (_, pure) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064);

            // (a) Sitka in MA-B within 8 m of MA-A boundary (x in (16, 24)) has woodland Hegyi CI != lane-B-only CI
            var view0 = primary.ReadView();
            var woodland = CrownModel.Woodland(view0.Areas.SelectMany(a => a.Trees));
            var laneBOnly = CrownModel.Woodland(view0.Areas[1].Trees);
            var borderSitka = view0.Areas[1].Trees
                .Where(t => t.IsLiving && t.XM > 16f && t.XM < 24f && !primaryGen.Options.GenesisSpecies.ContainsKey(t.TreeId))
                .OrderBy(t => t.XM)
                .First();
            var (woodlandCi, woodlandN) = CrownModel.WoodlandCompetition(borderSitka, woodland);
            var (laneCi, laneN) = CrownModel.WoodlandCompetition(borderSitka, laneBOnly);
            T.True(woodlandCi > laneCi && F32.Bits(woodlandCi) != F32.Bits(laneCi), "MA-B border Sitka woodland CI > lane-B-only CI");
            T.True(woodlandN > laneN, "MA-B border Sitka sees cross-border MA-A neighbours");

            // (b) Density mortality / RD stays per lane at 0.128 ha (never pooled at 0.64 ha)
            var s4View0 = primary.ReadStage4View();
            float pooledHa = view0.Areas.Sum(a => a.AreaHa);
            for (int i = 0; i < 5; i++)
            {
                var area = view0.Areas[i];
                double expectedLaneRd = WorkRules.Summarize(area.Trees, area.AreaHa).RelativeDensity;
                double pooledRd = WorkRules.Summarize(area.Trees, pooledHa).RelativeDensity;
                T.True(Math.Abs(s4View0.Areas[i].RelativeDensity - expectedLaneRd) < 1e-12, $"{area.AreaId} RD uses own 0.128 ha");
                T.True(Math.Abs(s4View0.Areas[i].RelativeDensity - pooledRd) > 0.1, $"{area.AreaId} RD is not pooled over 0.64 ha");
            }

            // Zero outside-grassland leakage: all trees and all cell centres lie strictly inside [0, 80] x [0, 80],
            // and light far beyond the outer perimeter (open grassland, no synthetic neighbours) is 1.0f.
            var s5View0 = primary.ReadStage5View();
            T.True(view0.Areas.SelectMany(a => a.Trees).All(t => t.XM > 0f && t.XM < 80f && t.ZM > 0f && t.ZM < 80f),
                "all stems lie strictly inside 80x80 m");
            T.True(s5View0.Areas.SelectMany(a => a.Cells).All(c => c.CentreXM > 0f && c.CentreXM < 80f && c.CentreZM > 0f && c.CentreZM < 80f),
                "all 1,024 ground cells lie strictly inside 80x80 m");
            T.Eq(1f, CanopyLightEvaluator.LightIndexAt(-20f, 40f, woodland), "open grassland outside west perimeter has light 1.0");
            T.Eq(1f, CanopyLightEvaluator.LightIndexAt(100f, 40f, woodland), "open grassland outside east perimeter has light 1.0");

            // (c) Advance both presets 5 years first to check cross-lane broadleaf seed input vs pure-Sitka control
            for (int y = 0; y < 5; y++)
            {
                Advance(primary);
                Advance(pure);
            }

            var p2Primary5 = primary.ReadPhase2View();
            T.Eq(5, p2Primary5.Accounts.Count, "primary recorded 5 Phase 2 accounts");
            double laneBOakSeed = p2Primary5.Accounts
                .SelectMany(acc => acc.Natural)
                .Where(n => n.AreaId == "MA-B" && n.SpeciesId == Phase2Species.OakId)
                .Sum(n => n.SeedArrival);
            double laneBBeechSeed = p2Primary5.Accounts
                .SelectMany(acc => acc.Natural)
                .Where(n => n.AreaId == "MA-B" && n.SpeciesId == Phase2Species.BeechId)
                .Sum(n => n.SeedArrival);
            T.True(laneBOakSeed > 0.0 || laneBBeechSeed > 0.0, "primary MA-B receives non-zero Oak/Beech seed arrival within 5 years");
            T.True(laneBOakSeed > 0.0 && laneBBeechSeed > 0.0, "primary MA-B receives both Oak and Beech cross-lane seed arrival");

            // Also verify directly at an individual MA-B cell centre near MA-A
            var primaryView5 = primary.ReadView();
            var woodland5 = CrownModel.Woodland(primaryView5.Areas.SelectMany(a => a.Trees));
            var oakSources = Phase2SeedRain.Sources(
                woodland5.Where(t => p2Primary5.TreeSpecies.TryGetValue(t.TreeId, out var sp) && sp == Phase2Species.OakId),
                Phase2Species.Oak, 1f);
            var beechSources = Phase2SeedRain.Sources(
                woodland5.Where(t => p2Primary5.TreeSpecies.TryGetValue(t.TreeId, out var sp) && sp == Phase2Species.BeechId),
                Phase2Species.Beech, 1f);
            var laneBCells = primary.ReadStage5View().Areas[1].Cells;
            T.True(laneBCells.Any(c => Phase2SeedRain.SeedRainAt(Phase2Species.Oak, c.CentreXM, c.CentreZM, oakSources) > 0f),
                "at least one MA-B cell receives direct Oak seed rain from MA-A");
            T.True(laneBCells.Any(c => Phase2SeedRain.SeedRainAt(Phase2Species.Beech, c.CentreXM, c.CentreZM, beechSources) > 0f),
                "at least one MA-B cell receives direct Beech seed rain from MA-A");

            // Per-lane density routing: MA-A and MA-E mixed, MA-B/C/D pure Sitka
            var year1Density = p2Primary5.Accounts[0].Density.ToDictionary(d => d.AreaId, StringComparer.Ordinal);
            T.True(year1Density["MA-A"].Mixed && year1Density["MA-E"].Mixed, "outer lanes MA-A and MA-E take mixed density route");
            T.True(!year1Density["MA-B"].Mixed && !year1Density["MA-C"].Mixed && !year1Density["MA-D"].Mixed,
                "middle lanes MA-B/C/D take pure-Sitka per-lane density route");

            // Control: pure-Sitka preset has zero Oak/Beech seed input everywhere across all 5 years
            var p2Pure5 = pure.ReadPhase2View();
            double pureBroadleafSeed = p2Pure5.Accounts
                .SelectMany(acc => acc.Natural)
                .Where(n => n.SpeciesId is Phase2Species.OakId or Phase2Species.BeechId)
                .Sum(n => n.SeedArrival);
            T.Eq(0.0, pureBroadleafSeed, "pure-Sitka control has zero Oak/Beech seed input everywhere");
            T.True(p2Pure5.Areas.SelectMany(a => a.Cells).All(c => c.Bands.Count == 0), "pure-Sitka control has zero broadleaf bands");

            // (d) Continue both presets to Year 15 under 1/1/1/1/1 without refusal
            for (int y = 5; y < 15; y++)
            {
                Advance(primary);
                Advance(pure);
            }
            T.Eq(15, primary.ReadView().Source.EcologicalYear, "primary preset advances 15 years cleanly");
            T.Eq(15, pure.ReadView().Source.EcologicalYear, "pure-Sitka preset advances 15 years cleanly");
        });

        // 7. 2A1-06: Economy interop on the real generated 0.64 ha primary preset
        T.Add("forestmaker 2A1-06: Economy interop on real generated 0.64 ha primary preset", () =>
        {
            var (gen, session) = ForestMakerGenerator.CreateSession(
                ForestMakerPresets.SitkaEdgeBroadleaves064,
                MultiAreaModelSelection.Phase2WithEconomy);

            // Opening cash == EUR 12,000
            var econ0 = session.ReadEconomyView();
            T.Eq(EconomyDefaults.DefaultOpeningCashCents, econ0.CashBalanceCents, "opening cash == EUR 12,000");
            T.Eq(1_200_000L, econ0.CashBalanceCents, "opening cash == 1,200,000 cents");

            // Queue a small Fell in MA-B; verify contractor-only Fell refusal, pure quote, EUR 2,500 harvest floor
            var v = session.ReadView();
            string treeId = v.Areas[1].Trees.First(t => t.IsLiving).TreeId;
            v = session.AddPending(Fence(v), "MA-B", treeId, "2A1-06 economy interop fell").View;

            T.Refuses("EconomyFellContractorOnly", () => session.SetFellExecutor(Fence(v), "MA-B", treeId, WorkExecutor.Landowner));
            T.Refuses("EconomyFellContractorOnly", () => session.SetOrderExecutor(Fence(v), EconomyOrderKey.ForFell("MA-B", treeId), WorkExecutor.Landowner));

            var quote = session.QuoteWorkPlan(Fence(v));
            T.True(quote.IsEligible, "small felling quote on EUR 12,000 balance is eligible");
            T.Eq(HarvestThinningStage.FirstThinning, quote.HarvestJob!.Stage, "first felling plan is FirstThinning");
            T.Eq(EconomyDefaults.MinimumHarvestJobCents, quote.HarvestJob.TotalHarvestContractorCostCents, "EUR 2,500 minimum harvest fee enforced");
            T.Eq(250_000L, quote.TotalExternalCostCents, "total external cost equals EUR 2,500 floor");

            // Approve, Execute, settle, then advance 1 year cleanly
            v = session.Approve(Fence(v)).View;
            var exec = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(exec.RefusalCode is null && exec.Execution?.Settlement is not null, "execute settles cleanly");
            T.Eq(250_000L, exec.Execution!.Settlement!.TotalExternalCostCents, "settled EUR 2,500 minimum harvest fee");

            var afterAdv = session.Advance(Fence(exec.View), consentToCancelWork: false);
            T.True(afterAdv.RefusalCode is null, "1-year advance after execution succeeds");
            T.Eq(1, afterAdv.View.Source.EcologicalYear, "advanced cleanly to Year 1");
            T.Eq(1, session.ReadEconomyView().YearEndSummaries.Count, "economy year-end summary recorded");

            // Upfront cash check on the real generated primary preset with EUR 2,000 (< EUR 2,500 floor)
            var lowCashSession = new MultiAreaContinuation(
                gen.Experiment,
                MultiAreaModelSelection.Phase2WithEconomy,
                null,
                null,
                gen.Options,
                EconomyOptions.Default with { OpeningCashCents = 200_000L });
            var lv = lowCashSession.ReadView();
            string lowTree = lv.Areas[1].Trees.First(t => t.IsLiving).TreeId;
            lv = lowCashSession.AddPending(Fence(lv), "MA-B", lowTree, "upfront cash check").View;
            var lowQuote = lowCashSession.QuoteWorkPlan(Fence(lv));
            T.True(!lowQuote.IsEligible, "quote ineligible with EUR 2,000 cash < EUR 2,500 harvest floor");
            T.True(lowQuote.Issues.Any(i => i.Code == "EconomyInsufficientCash"), "quote reports EconomyInsufficientCash");
            T.Refuses("EconomyInsufficientCash", () => lowCashSession.Approve(Fence(lv)));
            T.Eq(200_000L, lowCashSession.ReadEconomyView().CashBalanceCents, "refused approve leaves cash unchanged");
        });
    }

    private static ForestMakerRecipe CustomRecipe(
        int widthM,
        int lengthM,
        int laneCount,
        int startAgeYears,
        int stemsPerHa,
        int seed,
        ForestMakerSpeciesMix mix = ForestMakerSpeciesMix.PureSitka,
        int oakPercent = 0,
        int beechPercent = 0) => new(
        GeneratorId: ForestMakerBounds.GeneratorId,
        GeneratorVersion: ForestMakerBounds.GeneratorVersion,
        PresetId: null,
        WidthM: widthM,
        LengthM: lengthM,
        LaneCount: laneCount,
        StartAgeYears: startAgeYears,
        StemsPerHa: stemsPerHa,
        Seed: seed,
        SpeciesMix: mix,
        OakPercent: oakPercent,
        BeechPercent: beechPercent);

    private static string FiveYearFingerprint(ForestMakerRecipe recipe)
    {
        var (_, session) = ForestMakerGenerator.CreateSession(recipe);
        for (int y = 0; y < 5; y++) Advance(session);

        var w = new Model0FingerprintTests.Fingerprint();
        var view = session.ReadView();
        foreach (var area in view.Areas)
        {
            w.Text(area.AreaId + "|" + area.Trees.Count);
            w.Trees(area.Trees);
        }
        WriteStage4(w, session.ReadStage4View());
        Write(w, session.ReadStage5View());
        WritePhase2(w, Phase2Of(session));
        return w.Hash();
    }
}

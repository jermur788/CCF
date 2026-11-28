using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;
using static SitkaP1.Core.Tests.Phase2Support;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Scots pine and site moisture, Part 2 live wiring (SPEC-multi-2d-scots-pine-moisture-V1, T-07): NeutralLegacy identity on
/// existing woodlands, moisture modifiers in the annual loop, Scots pine growth / regeneration / pruning / grading through the
/// Part 1 engine, the two-zone preset and label, and the RNG salt registration.
/// </summary>
internal static class ScotsPinePart2Tests
{
    private const string Pine = ScotsPineLive.SpeciesId;

    internal static void Register()
    {
        T.Add("SP2-01 salts: the Part 1 good-seed-year salt is registered unchanged and the two live families are registered", () =>
        {
            T.Eq(0x1A96C03752AE8D4BUL, RngSalts.ScotsPineGoodSeedYear, "registered Part 1 salt value");
            T.True(RngSalts.Families.Contains(RngSalts.ScotsPineRecruitment, StringComparer.Ordinal), "recruitment family registered");
            T.True(RngSalts.Families.Contains(RngSalts.ScotsPineEstablishment, StringComparer.Ordinal), "establishment family registered");
            // The registered value drives the same schedule as the Part 1 private copy: one good year per 4-year block.
            for (int block = 0; block < 25; block++)
                T.Eq(1, Enumerable.Range(block * 4, 4).Count(y => ScotsPineEngine.IsGoodSeedYear(6_400_303UL, y)), "one good year per block");
        });

        T.Add("SP2-02 neutral: an existing preset woodland run with a NeutralLegacy Scots pine state is bit-identical to the unchanged path", () =>
        {
            var recipe = ForestMakerPresets.SitkaEdgeBroadleaves064;
            var generated = ForestMakerGenerator.Generate(recipe);
            T.True(generated.ScotsPineGenesisFor(null) is null && generated.ScotsPineGenesisFor(SiteMoistureSetting.Neutral) is null,
                "an existing preset with NeutralLegacy opens no Scots pine state");
            var plain = new MultiAreaContinuation(generated.Experiment, MultiAreaModelSelection.Phase2, null, null, generated.Options,
                genesisRecipe: recipe);
            var neutral = new MultiAreaContinuation(generated.Experiment, MultiAreaModelSelection.Phase2, null, null, generated.Options,
                genesisRecipe: recipe, scotsPine: new ScotsPineGenesis(SiteMoistureSetting.Neutral, Array.Empty<string>()));
            T.True(Root(plain).ScotsPine is null && Root(neutral).ScotsPine is not null, "only the second session runs the new code path");
            foreach (var c in new[] { plain, neutral })
            {
                // The same managed year in both: plant one Oak, fell three Sitka, then advance six years.
                var (x, z) = FindPoint(c, "MA-B", 1.0f);
                var v = c.AddPlant(Fence(c.ReadView()), "MA-B", Oak, x, z).View;
                foreach (var tree in v.Areas[1].Trees.Where(t => t.IsLiving).Take(3))
                    v = c.AddPending(Fence(v), "MA-B", tree.TreeId, "SP2-02 gap").View;
                ApproveAndExecute(c);
                for (int y = 0; y < 6; y++) Advance(c);
            }
            T.Eq(Digest(plain), Digest(neutral), "trees, Stage 5 cells and Phase 2 bands/juveniles identical after six years");
            T.True(Root(neutral).ScotsPine!.Accounts.Count == 6 && Root(neutral).ScotsPine!.Recruits.Count == 0, "no recruits without Scots pine");
        });

        T.Add("SP2-03 moisture: a dry whole-woodland site scales Sitka growth by its 0.35 growth modifier; NeutralLegacy is exactly 1", () =>
        {
            T.Eq(new SiteResponse(1.0, 1.0), SpeciesMoistureTable.Response(SpeciesMoistureTable.SitkaSpruce, SiteMoistureClass.NeutralLegacy), "neutral identity");
            var recipe = ForestMakerPresets.PureSitka064;
            var (_, neutral) = ForestMakerGenerator.CreateSession(recipe);
            var (_, dry) = ForestMakerGenerator.CreateSession(recipe, siteMoisture: SiteMoistureSetting.Whole(SiteMoistureClass.DryFreeDraining));
            T.True(Root(neutral).ScotsPine is null && Root(dry).ScotsPine!.Site.WholeClass == SiteMoistureClass.DryFreeDraining, "dry state opened");
            var before = Root(dry).Areas.SelectMany(a => a.State.Trees).ToDictionary(t => t.TreeId);
            Advance(neutral);
            Advance(dry);
            var n = Root(neutral).Areas.SelectMany(a => a.State.Trees).Where(t => t.IsLiving).ToDictionary(t => t.TreeId);
            int compared = 0;
            foreach (var t in Root(dry).Areas.SelectMany(a => a.State.Trees).Where(t => t.IsLiving))
            {
                if (!n.TryGetValue(t.TreeId, out var nt)) continue;
                double dryGain = t.DbhCm - before[t.TreeId].DbhCm, neutralGain = nt.DbhCm - before[t.TreeId].DbhCm;
                if (neutralGain <= 1e-4) continue;
                T.True(Math.Abs(dryGain / neutralGain - 0.35) < 2e-3, t.TreeId + " dry DBH gain is 0.35 of the neutral gain");
                compared++;
            }
            T.True(compared > 1000, "most stems compared (" + compared + ")");
        });

        T.Add("SP2-04 two-zone preset: 0.64 ha, 5 lanes, 1,344 stems, mature Scots pine and Sitka at Year 0, more pine on the dry ridge", () =>
        {
            var recipe = ForestMakerPresets.ByPresetId("fm-064-scots-pine-sitka-two-zone-v1");
            T.Eq(ForestMakerPresets.ScotsPineTwoZone064, recipe, "stable preset ID");
            var a = ForestMakerGenerator.Generate(recipe);
            var b = ForestMakerGenerator.Generate(recipe);
            T.Eq(a.ContentHash, b.ContentHash, "deterministic generation");
            T.Eq(1_344, a.TotalStems, "stems consistent with the other 0.64 ha presets");
            T.Eq(5, a.Experiment.Areas.Count, "five lanes");
            T.True(a.Experiment.Areas.All(area => area.PolygonMm[1].XMm - area.PolygonMm[0].XMm == 16_000 &&
                area.PolygonMm[2].YMm - area.PolygonMm[1].YMm == 80_000), "equal 16 x 80 m lanes");
            var pine = a.ScotsPineTreeIds!.ToHashSet(StringComparer.Ordinal);
            var trees = a.Experiment.Areas.SelectMany(area => area.Trees).ToArray();
            int dryPine = trees.Count(t => pine.Contains(t.TreeId) && t.XMm < 40_000), dry = trees.Count(t => t.XMm < 40_000);
            int moistPine = trees.Count(t => pine.Contains(t.TreeId) && t.XMm >= 40_000), moist = trees.Count(t => t.XMm >= 40_000);
            T.True(dryPine > 0 && moistPine > 0 && dry - dryPine > 0 && moist - moistPine > 0, "pine and Sitka on both halves");
            T.True((double)dryPine / dry > 0.6 && (double)moistPine / moist < 0.4, "pine leads on the dry ridge");
            T.True(trees.Where(t => pine.Contains(t.TreeId)).All(t => t.AgeYears >= ScotsPineEngine.MinimumSeedTreeAgeYears),
                "every genesis pine is old enough to be a local seed tree");
            T.Eq(a.Lanes.Sum(l => l.ScotsPineCount), pine.Count, "lane summaries count the pine");
            T.True(a.Options.GenesisSpecies.Count == 0, "pine never enters the frozen Oak/Beech genesis species map");
            var genesis = a.ScotsPineGenesisFor(null)!;
            T.Eq(SiteMoistureSetting.DryRidgeMoistSlope, genesis.Site, "two-zone site");
            T.Refuses("SiteMoistureFixedByPreset", () => a.ScotsPineGenesisFor(SiteMoistureSetting.Whole(SiteMoistureClass.StagnantPoorBog)));
            var (_, session) = ForestMakerGenerator.CreateSession(recipe);
            var root = Root(session);
            T.Eq(pine.Count, root.Phase2!.TreeSpecies.Count(kv => kv.Value == Pine), "genesis pine named in the live species map");
            T.Eq(SiteMoistureLabels.TwoZoneLabel, session.ReadScotsPineView().Site.WholeStandLabel, "two-zone whole-stand label");
        });

        T.Add("SP2-05 label: a 50/50 two-zone woodland reads Two-Zone, never the DryFreeDraining tie winner; bog alone carries the caution", () =>
        {
            var field = new SiteMoistureField(SiteMoistureSetting.DryRidgeMoistSlope, new StandBounds(0, 0, 80, 80));
            var zones = field.ZoneAreas();
            T.Eq(zones[0].AreaM2, zones[1].AreaM2, "exact 50/50 split");
            T.Eq(SiteMoistureClass.DryFreeDraining, SiteMoistureEngine.SelectDominant(zones).Moisture, "Part 1 tie rule picks dry");
            T.Eq("Two-Zone (Dry Ridge & Moist Slope)", SiteMoistureLabels.WholeStandLabel(field.Setting), "locked label");
            T.True(SiteMoistureLabels.WholeStandLabel(field.Setting) != SiteMoistureLabels.ClassLabel(SiteMoistureClass.DryFreeDraining),
                "the label never collapses to DryFreeDraining");
            T.Eq(SiteMoistureClass.DryFreeDraining, field.At(39.9, 10), "x below the midpoint is dry");
            T.Eq(SiteMoistureClass.FreshMoistMineral, field.At(40, 10), "the midpoint is moist");
            foreach (var moisture in SpeciesMoistureTable.MoistureClasses)
                T.Eq(moisture == SiteMoistureClass.StagnantPoorBog, SiteMoistureLabels.HabitatCaution(SiteMoistureSetting.Whole(moisture)),
                    "caution only on bog: " + moisture);
            var view = ScotsPineReadouts.Site(new SiteMoistureField(SiteMoistureSetting.Whole(SiteMoistureClass.StagnantPoorBog), new StandBounds(0, 0, 80, 80)));
            T.True(view.HabitatCaution && view.HabitatCautionText is not null, "bog view carries the caution text from Core");
        });

        T.Add("SP2-06 growth: Scots pine DBH grows by the Part 1 site-adjusted increment; Sitka on the dry ridge by its 0.35 modifier", () =>
        {
            var recipe = ForestMakerPresets.ScotsPineTwoZone064;
            var generated = ForestMakerGenerator.Generate(recipe);
            var pineIds = generated.ScotsPineTreeIds!;
            MultiAreaContinuation Make(SiteMoistureSetting site) => new(generated.Experiment, MultiAreaModelSelection.Phase2, null, null,
                generated.Options, genesisRecipe: recipe, scotsPine: new ScotsPineGenesis(site, pineIds));
            var twoZone = Make(SiteMoistureSetting.DryRidgeMoistSlope);
            var neutral = Make(SiteMoistureSetting.Neutral);
            var start = Root(twoZone);
            var field = twoZone.ReadSiteMoisture();
            Advance(twoZone);
            Advance(neutral);
            var after = Root(twoZone).Areas.SelectMany(a => a.State.Trees).ToDictionary(t => t.TreeId);
            var afterNeutral = Root(neutral).Areas.SelectMany(a => a.State.Trees).ToDictionary(t => t.TreeId);
            int pineChecked = 0, dryChecked = 0, moistChecked = 0;
            foreach (var area in start.Areas)
            {
                var living = area.State.Trees.Where(t => t.IsLiving).ToArray();
                double areaHa = (double)generated.Experiment.Areas.Single(a => a.AreaId == area.AreaId).AreaHa;
                double sq = living.Sum(t => (double)t.DbhCm * t.DbhCm);
                var stand = new AnnualStand(living.Length, living.Length / (float)areaHa, sq * (Math.PI / 40000.0) / (float)areaHa, 0, 0);
                double top = living.Max(t => (double)t.HeightM);
                foreach (var tree in living)
                {
                    if (!after.TryGetValue(tree.TreeId, out var next) || !next.IsLiving) continue;
                    if (pineIds.Contains(tree.TreeId))
                    {
                        var expected = ScotsPineEngine.EvaluateGrowth(new ScotsPineGrowthRequest(tree.AgeYears, stand.StemsPerHa,
                            stand.BasalAreaM2PerHa, top, field.At(tree.XM, tree.ZM)));
                        T.True(expected.IsWithinCalibrationDomain, "genesis stand is inside the GROWFOR domain");
                        T.Eq(F32.Bits(Math.Clamp(tree.DbhCm + (float)expected.SiteAdjustedDbhIncrementCm!.Value, 1f, 200f)), F32.Bits(next.DbhCm),
                            tree.TreeId + " DBH = before + Part 1 site-adjusted increment");
                        T.Eq(F32.Bits(next.DbhCm), F32.Bits(afterNeutral[tree.TreeId].DbhCm), "pine growth modifier is 1.0 on dry and fresh moist");
                        pineChecked++;
                    }
                    else
                    {
                        double gain = next.DbhCm - tree.DbhCm, neutralGain = afterNeutral[tree.TreeId].DbhCm - tree.DbhCm;
                        if (neutralGain <= 1e-4) continue;
                        if (tree.XM < 40.0) { T.True(Math.Abs(gain / neutralGain - 0.35) < 2e-3, "dry-ridge Sitka at 0.35"); dryChecked++; }
                        else { T.Eq(F32.Bits(next.DbhCm), F32.Bits(afterNeutral[tree.TreeId].DbhCm), "moist-slope Sitka at 1.0"); moistChecked++; }
                    }
                }
            }
            T.True(pineChecked > 300 && dryChecked > 50 && moistChecked > 300, $"checked pine {pineChecked}, dry Sitka {dryChecked}, moist Sitka {moistChecked}");
            var account = Root(twoZone).ScotsPine!.Accounts.Single();
            T.Eq(pineChecked, account.AdultsGrowing, "account counts every growing pine");
        });

        T.Add("SP2-07 planting: Scots pine plants with a shelter and a 1 m2 spot clear on a Forest Maker woodland only; EUR 0.45 sapling", () =>
        {
            T.Eq(45L, EconomyDefaults.SaplingMaterialFor(Pine).UnitCents, "Part 1 sapling material cost");
            var mixed = MixedSession();
            var (mx, mz) = FindPoint(mixed, "MA-B", 1.0f);
            T.Refuses("Phase2SpeciesUnknown", () => mixed.AddPlant(Fence(mixed.ReadView()), "MA-B", Pine, mx, mz));

            var (_, c) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064);
            T.True(Root(c).ScotsPine is null, "no Scots pine state before planting");
            var (x, z) = FindPoint(c, "MA-C", 1.0f);
            var v = c.AddPlant(Fence(c.ReadView()), "MA-C", Pine, x, z).View;
            T.True(Root(c).ScotsPine is { Site.IsNeutral: true }, "the first pine order opens a neutral Scots pine state");
            v = c.AddShelter(Fence(v), "MA-C", x, z).View;
            v = c.AddSpotClear(Fence(v), "MA-C", x, z).View;
            ApproveAndExecute(c);
            var juvenile = Phase2Of(c).Juveniles.Single();
            T.True(juvenile.SpeciesId == Pine && juvenile.IsActive, "planted pine juvenile");
            T.True(Phase2Of(c).Shelters.Single().JuvenileId == juvenile.JuvenileId, "8-year shelter on the pine");
            T.Eq(8, Phase2Of(c).Shelters.Single().EffectiveYears, "shelter life");
            T.True(Phase2Of(c).Patches.Count == 1, "1 m2 spot clear");
            Advance(c);
            var grown = Phase2Of(c).Juveniles.Single();
            T.True(grown.LastAssessmentYear == 1 && !grown.LastYearBrowsed, "assessed and sheltered from browsing");
            T.True(!grown.IsActive || grown.HeightM >= juvenile.HeightM, "a living pine juvenile never shrinks");
        });

        T.Add("SP2-08 pruning: Scots pine lifts are 2.5 m then 5.5 m (the 5-6 m target), five years apart, below 60 % of height", () =>
        {
            var tree = new AnnualTree("P", 1, 1, 25, 20f, 12f, 2f);
            T.Eq(2.5f, ScotsPinePruning.NextLiftTargetM(0), "first lift");
            T.Eq(5.5f, ScotsPinePruning.NextLiftTargetM(1), "second lift inside 5-6 m");
            T.True(float.IsNaN(ScotsPinePruning.NextLiftTargetM(2)), "no third lift");
            T.Eq<string?>(null, ScotsPinePruning.RefusalCode(tree, TreePruningState.Unpruned, 2.5f, 5), "first lift allowed");
            T.Eq<string?>("PrunePineTargetInvalid", ScotsPinePruning.RefusalCode(tree, TreePruningState.Unpruned, 2.0f, 5), "off-schedule target");
            var once = new TreePruningState(1, 2.5f, 5);
            T.Eq<string?>("PruneIntervalTooShort", ScotsPinePruning.RefusalCode(tree, once, 5.5f, 8), "five-year interval");
            T.Eq<string?>(null, ScotsPinePruning.RefusalCode(tree, once, 5.5f, 10), "second lift after five years");
            T.Eq<string?>("PruneTargetOutOfRange", ScotsPinePruning.RefusalCode(tree with { HeightM = 9f }, once, 5.5f, 10), "below 60 % of height");
            T.Eq<string?>("PruneMaxLiftsReached", ScotsPinePruning.RefusalCode(tree, new TreePruningState(2, 5.5f, 10), 6f, 20), "two lifts only");

            var (_, c) = ForestMakerGenerator.CreateSession(ForestMakerPresets.ScotsPineTwoZone064);
            var root = Root(c);
            var pineTree = root.Areas[0].State.Trees.First(t => t.IsLiving && root.Phase2!.SpeciesIdOf(t.TreeId) == Pine && t.HeightM > 6f);
            var v = c.AddScheduledPrune(Fence(c.ReadView()), "MA-A", pineTree.TreeId).View;
            T.Eq(2.5f, Root(c).Stage4!.PendingPrunes.Single().TargetCrownBaseHeightM, "the scheduled pine lift is 2.5 m");
            T.Refuses("PrunePineTargetInvalid", () => c.AddPrune(Fence(v), "MA-A",
                root.Areas[0].State.Trees.First(t => t.IsLiving && t.TreeId != pineTree.TreeId && root.Phase2!.SpeciesIdOf(t.TreeId) == Pine && t.HeightM > 6f).TreeId, 5f));
        });

        T.Add("SP2-09 grading: Scots pine Sawlog EUR 68/t (pruned >= 5 m or tight early stocking), Small Roundwood 45, Pulp/Chip 38, 1.02 t/m3", () =>
        {
            var mature = new AnnualTree("SP-1", 1, 1, 45, 30f, 20f, 3f);
            var pruned = new TreePruningState(2, 5.5f, 30);
            var saw = D062TimberAssortmentEngine.GradeStem(mature, Pine, pruned);
            long vol = (long)decimal.Floor((decimal)FallenDeadwoodRecord.StemVolumeM3(30f, 20f) * 1_000_000m);
            T.Eq(vol, saw.StemVolumeCm3, "stem volume");
            T.Eq(TimberAssortmentKind.Sawlog, saw.Logs.Single().Assortment, "pruned mature stem is Sawlog");
            T.Eq(6_800L, saw.Logs.Single().UnitPriceCentsPerTonne, "EUR 68/t");
            T.Eq((long)decimal.Round(vol / 1_000_000m * 1.02m * 1_000_000m, 0, MidpointRounding.AwayFromZero), saw.HarvestedGreenGrams, "1.02 t/m3");
            T.Eq((long)decimal.Round(vol / 1_000_000m * 1.02m * 68m * 100m, 0, MidpointRounding.AwayFromZero), saw.TimberRevenueCents, "gross value");
            T.Eq(saw.StemVolumeCm3, saw.SaleVolumeCm3 + saw.ResidualVolumeCm3, "volume conservation");
            var tight = D062TimberAssortmentEngine.GradeStem(mature, Pine, TreePruningState.Unpruned, tightEarlyStocking: true);
            T.Eq(TimberAssortmentKind.Sawlog, tight.Logs.Single().Assortment, "tight early stocking route");
            var young = D062TimberAssortmentEngine.GradeStem(mature with { AgeYears = 30 }, Pine, pruned);
            T.Eq(4_500L, young.Logs.Single().UnitPriceCentsPerTonne, "immature stem falls back to Small Roundwood EUR 45/t");
            var thin = D062TimberAssortmentEngine.GradeStem(mature with { AgeYears = 30, DbhCm = 10f, HeightM = 10f }, Pine, pruned);
            T.Eq(TimberAssortmentKind.PulpChip, thin.Logs.Single().Assortment, "thin stem is Pulp/Chip");
            T.Eq(3_800L, thin.Logs.Single().UnitPriceCentsPerTonne, "EUR 38/t");
            var sitka = D062TimberAssortmentEngine.GradeStem(mature, Phase2Species.SitkaId, pruned);
            T.True(sitka.Logs.All(l => l.SpeciesId == Phase2Species.SitkaId) && saw.HarvestedGreenGrams != sitka.HarvestedGreenGrams,
                "Sitka keeps its own ladder and 890 kg/m3 proxy");
        });

        T.Add("SP2-10 regeneration: local seed fills bright, clean gaps only in good seed years; zero draws without seed trees or in poor years", () =>
        {
            var (s5, s2, inputs, field) = OpenGap(withPineAge: 30);
            ulong seed = 6_400_303UL;
            var drawn = new List<string>();
            Phase2Random.DrawLog = drawn;
            try
            {
                var state = MultiAreaScotsPineState.Create(SiteMoistureSetting.Neutral);
                int recruitedInGood = 0;
                for (int year = 0; year < 12; year++)
                {
                    drawn.Clear();
                    var result = ScotsPineAnnual.Step(state, s2, s5, inputs, field, seed, year + 1, new ScotsPineAdultTally());
                    bool good = ScotsPineEngine.IsGoodSeedYear(seed, year);
                    T.Eq(good, result.Account.GoodSeedYear, "account seed year");
                    if (!good) T.True(result.Account.Recruited == 0 && !drawn.Any(k => k.StartsWith(RngSalts.ScotsPineRecruitment, StringComparison.Ordinal)),
                        "no recruitment draw in a poor seed year");
                    else { T.True(result.Account.EligibleCells > 0, "eligible gap cells in a good year"); recruitedInGood += result.Account.Recruited; }
                    state = result.NextState;
                    s2 = result.NextPhase2;
                }
                T.True(recruitedInGood > 0, "recruits appear in good seed years");
                T.True(state.Recruits.All(r => r.Alive && r.HeightM >= ScotsPineLive.RecruitInitialHeightM), "recruits survive full light and grow");
                T.True(state.Recruits.Any(r => r.PromotedTreeId == "PN-" + r.RecruitId && r.HeightM >= ScotsPineLive.PromotionHeightM),
                    "early recruits reach 3.0 m and promote to Scots pine adults");

                // Same inputs twice: identical recruits (deterministic, keyed by woodland seed + registered salt).
                var again = ScotsPineAnnual.Step(MultiAreaScotsPineState.Create(SiteMoistureSetting.Neutral), OpenGap(30).S2, OpenGap(30).S5,
                    inputs, field, seed, FirstGoodYear(seed) + 1, new ScotsPineAdultTally());
                var twice = ScotsPineAnnual.Step(MultiAreaScotsPineState.Create(SiteMoistureSetting.Neutral), OpenGap(30).S2, OpenGap(30).S5,
                    inputs, field, seed, FirstGoodYear(seed) + 1, new ScotsPineAdultTally());
                T.True(again.NextState.Recruits.SequenceEqual(twice.NextState.Recruits), "deterministic recruits");

                // No mature local seed tree: zero draws even in a good year.
                var (n5, n2, nInputs, nField) = OpenGap(withPineAge: 12);
                drawn.Clear();
                var none = ScotsPineAnnual.Step(MultiAreaScotsPineState.Create(SiteMoistureSetting.Neutral), n2, n5, nInputs, nField, seed,
                    FirstGoodYear(seed) + 1, new ScotsPineAdultTally());
                T.True(none.Account.Recruited == 0 && none.Account.MatureSeedTrees == 0 && drawn.Count == 0, "zero draws without a mature seed tree");
            }
            finally { Phase2Random.DrawLog = null; }
        });

        T.Add("SP2-11 juveniles: the deterministic juvenile year dies in deep shade or when smothered, and grows by 0.40 m x light x moisture", () =>
        {
            T.Eq("light", ScotsPineAnnual.Juvenile(1f, 0.1f, 0f, false, 0f, 1f).DeathCause, "deep shade kills a light-demanding pine");
            T.Eq("vegetation", ScotsPineAnnual.Juvenile(0.5f, 0.8f, 0.8f, false, 0f, 1f).DeathCause, "smothered below 1 m");
            var open = ScotsPineAnnual.Juvenile(0.5f, 1f, 0f, false, 0f, 1f);
            T.Eq(F32.Bits(0.9f), F32.Bits(open.Height), "full light, no browse: +0.40 m");
            var wet = ScotsPineAnnual.Juvenile(0.5f, 1f, 0f, false, 0f, 0.35f);
            T.Eq(F32.Bits(0.5f + 0.40f * 0.35f), F32.Bits(wet.Height), "wet moving alluvial growth modifier 0.35");
            var browsed = ScotsPineAnnual.Juvenile(0.5f, 1f, 0f, false, 0.2f, 1f);
            var sheltered = ScotsPineAnnual.Juvenile(0.5f, 1f, 0f, true, 0.2f, 1f);
            T.True(browsed.Browsed && !sheltered.Browsed && browsed.Height < sheltered.Height, "a shelter prevents browse loss");
        });

        T.Add("SP2-12 two-zone runs: ten years twice give byte-identical state; pine deadwood is recorded as Scots pine", () =>
        {
            string Run()
            {
                var (_, c) = ForestMakerGenerator.CreateSession(ForestMakerPresets.ScotsPineTwoZone064);
                var v = c.ReadView();
                // Open gaps in lane A so light reaches the ground near the dry-ridge seed trees.
                foreach (var tree in v.Areas[0].Trees.Where(t => t.IsLiving).Where((t, i) => i % 3 != 0).ToArray())
                    v = c.AddPending(Fence(v), "MA-A", tree.TreeId, "SP2-12 regeneration gap").View;
                ApproveAndExecute(c);
                for (int y = 0; y < 10; y++) Advance(c);
                var r = Root(c);
                T.True(r.Stage4!.Deadwood.Where(d => r.Phase2!.SpeciesIdOf(d.TreeId) == Pine).All(d => d.SpeciesId == Pine),
                    "every dead pine's deadwood record names Scots pine");
                return Digest(c) + "|" + string.Join(",", r.ScotsPine!.Recruits.Select(x => x.RecruitId + "@" + F32.Bits(x.HeightM).ToString("x8")));
            }
            T.Eq(Run(), Run(), "deterministic ten-year two-zone run");
        });
    }

    // ---------------------------------------------------------------- helpers

    private static void Advance(MultiAreaContinuation c) => c.Advance(Fence(c.ReadView()), consentToCancelWork: true);

    private static int FirstGoodYear(ulong seed) => Enumerable.Range(0, 8).First(y => ScotsPineEngine.IsGoodSeedYear(seed, y));

    /// <summary>Two empty 10 x 10 m racks at full light and zero cover, with one Scots pine of the given age in rack A.</summary>
    private static (MultiAreaStage5State S5, MultiAreaPhase2State S2, Stage5AreaInput[] Inputs, SiteMoistureField Field) OpenGap(int withPineAge)
    {
        static MultiAreaVertex V(double x, double z) => new(x, z);
        var pine = new AnnualTree("MA-A-P01", 0.6f, 0.6f, withPineAge, 25f, 15f, 2f);
        var inputs = new[]
        {
            new Stage5AreaInput("MA-A", 901, Array.AsReadOnly(new[] { V(0, 0), V(10, 0), V(10, 10), V(0, 10) }), 0.01f,
                Array.AsReadOnly(new[] { pine })),
            new Stage5AreaInput("MA-B", 902, Array.AsReadOnly(new[] { V(10, 0), V(20, 0), V(20, 10), V(10, 10) }), 0.01f,
                Array.Empty<AnnualTree>()),
        };
        var species = new Dictionary<string, string>(StringComparer.Ordinal) { ["MA-A-P01"] = Pine };
        var (s5, s2) = EmptyStates(inputs, species: species);
        s5 = s5 with
        {
            Areas = Array.AsReadOnly(s5.Areas.Select(a => a with
            {
                Cells = Array.AsReadOnly(a.Cells.Select(cell => cell with { Light = 1f, Bramble = 0f, Bracken = 0f }).ToArray()),
            }).ToArray()),
        };
        var field = new SiteMoistureField(SiteMoistureSetting.Neutral, new StandBounds(0, 0, 20, 10));
        return (s5, s2, inputs, field);
    }

    /// <summary>A SHA-256 over every tree, Stage 5 cell and Phase 2 band and juvenile of a session, in exact float bits.</summary>
    private static string Digest(MultiAreaContinuation c)
    {
        var r = Root(c);
        var sb = new StringBuilder();
        static string B(float f) => F32.Bits(f).ToString("x8", CultureInfo.InvariantCulture);
        foreach (var area in r.Areas)
            foreach (var t in area.State.Trees)
                sb.Append(t.TreeId).Append(':').Append(t.AgeYears).Append(B(t.XM)).Append(B(t.ZM)).Append(B(t.DbhCm)).Append(B(t.HeightM))
                    .Append(B(t.CrownRadiusM)).Append(t.Death?.Cause ?? "-").Append(';');
        foreach (var area in r.Stage5!.Areas)
            foreach (var cell in area.Cells)
            {
                sb.Append(cell.Column).Append(',').Append(cell.Row).Append(B(cell.Light)).Append(B(cell.Bramble)).Append(B(cell.Bracken));
                foreach (var band in cell.Bands) sb.Append(B(band.Density)).Append(B(band.Height)).Append(band.EstablishYear);
            }
        foreach (var area in r.Phase2!.Areas)
            foreach (var cell in area.Cells)
                foreach (var band in cell.Bands) sb.Append(band.SpeciesId).Append(B(band.Density)).Append(B(band.Height));
        foreach (var j in r.Phase2.Juveniles)
            sb.Append(j.JuvenileId).Append(j.SpeciesId).Append(B(j.HeightM)).Append(j.Alive).Append(j.PromotedTreeId ?? "-");
        foreach (var (id, sp) in r.Phase2.TreeSpecies.OrderBy(kv => kv.Key, StringComparer.Ordinal)) sb.Append(id).Append('=').Append(sp);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()))).ToLowerInvariant();
    }
}

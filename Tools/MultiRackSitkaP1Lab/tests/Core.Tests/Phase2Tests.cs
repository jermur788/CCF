using System.Security.Cryptography;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Stage5Support;
using static SitkaP1.Core.Tests.Phase2Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// MULTI Phase 2 qualification (spec multi-phase2-oak-beech-v1, checklists/requirements.md P2-01 … P2-10). Each test names
/// the criterion it observes. Light is the modelled index, not calibrated PAR; every number is the consequence of the
/// stated gameplay rules, not an ecological forecast.
/// </summary>
internal static class Phase2Tests
{
    /// <summary>The frozen P0 Core binary pin (also enforced by the build, SitkaP1.Core.csproj P0CoreExpectedSha256).</summary>
    internal const string PinnedP0CoreSha256 = "9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3";

    private const AdultModelVersion A0 = AdultModelVersion.Legacy, A1 = AdultModelVersion.ConnectedWoodland;
    private const MortalityModelVersion M0 = MortalityModelVersion.Legacy, M1 = MortalityModelVersion.RecordedDeadwood;
    private const CrownModelVersion C0 = CrownModelVersion.LegacyFrozen, C1 = CrownModelVersion.AreaScaledEquilibriumC2;
    private const MultiAreaRegenerationModelVersion R0 = MultiAreaRegenerationModelVersion.Off,
        R1 = MultiAreaRegenerationModelVersion.SitkaCohortCompetitionBrowse;
    private const SpeciesModelVersion S0 = SpeciesModelVersion.Off, S1 = SpeciesModelVersion.ThreeSpeciesMixed;

    internal static void Register(string manifestPath)
    {
        FrozenPaths(manifestPath);
        BroadleafAdults();
        MixedDensity();
        Commands();
        BrowsingAndShelters();
        SpotClearance();
        PlantedPromotion();
        SeedRainAndMast();
        CapacityAndBlockCap();
        Determinism();
    }

    private static void Close(double expected, double actual, string what, double relative = 1e-6)
    {
        if (!(Math.Abs(expected - actual) <= relative * Math.Max(1.0, Math.Abs(expected))))
            throw new Exception($"{what}: expected {expected:R}, got {actual:R}");
    }

    // ============================================================================================ P2-01 frozen paths
    private static void FrozenPaths(string manifestPath)
    {
        T.Add("P2-01 F-01: Species 0 is byte-identical: single-area Model 0, 0/0/0/0/0, 0/0/1/0/0, 1/1/1/0/0 and 1/1/1/1/0 reproduce their pinned fingerprints (1/1/1/1 now pinned, R8); the P0 Core binary matches its pin", () =>
        {
            T.Eq(Model0FingerprintTests.PinnedSingleAreaHash, Model0FingerprintTests.SingleAreaFingerprint(Path.GetDirectoryName(manifestPath)!),
                "single-area Model 0");
            T.Eq(Model0FingerprintTests.PinnedMultiAreaHash, Stage4Tests.Fingerprint(new MultiAreaModelSelection(A0, M0, C0, R0, S0)), "0/0/0/0/0");
            T.Eq(Stage4Tests.PinnedStage3CrownModel1Hash, Stage4Tests.Fingerprint(new MultiAreaModelSelection(A0, M0, C1, R0, S0)), "0/0/1/0/0");
            T.Eq(Stage5Tests.PinnedStage4FullRunHash,
                Stage4BehaviourTests.RunFingerprint(Stage4Options.Default, new MultiAreaModelSelection(A1, M1, C1, R0, S0)), "1/1/1/0/0");
            string stage5 = Stage5Tests.FullRun(new MultiAreaContinuation(MultiAreaGenerator.Generate(), new MultiAreaModelSelection(A1, M1, C1, R1, S0)));
            Console.WriteLine("  P2-01 1/1/1/1/0 full-run fingerprint " + stage5);
            T.Eq(Stage5Tests.PinnedStage5FullRunHash, stage5, "1/1/1/1/0");
            var p0 = typeof(MultiAreaContinuation).Assembly.GetReferencedAssemblies().Single(a => a.Name == "SitkaLab.Core");
            string p0Path = System.Reflection.Assembly.Load(p0).Location;
            string p0Hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p0Path))).ToLowerInvariant();
            Console.WriteLine("  P2-01 P0 Core binary " + p0Hash);
            T.Eq(PinnedP0CoreSha256, p0Hash, "P0 Core pin");
        });

        T.Add("P2-01 F-02: tokens keep every earlier lineage and append the species version only when it is not 0 (Q1); Phase 2 state is null on every Species 0 root", () =>
        {
            var e = MultiAreaGenerator.Generate();
            string stage5Token = MultiAreaIdentity.AreaModelVersionStage4 + "+regeneration-v1";
            T.Eq(MultiAreaIdentity.AreaModelVersion, new MultiAreaContinuation(e, new MultiAreaModelSelection(A0, M0, C0, R0, S0)).ReadView().Source.AreaModelVersion, "0/0/0/0/0");
            T.Eq(MultiAreaIdentity.AreaModelVersionConnectedCrownC2,
                new MultiAreaContinuation(e, new MultiAreaModelSelection(A0, M0, C1, R0, S0)).ReadView().Source.AreaModelVersion, "0/0/1/0/0");
            T.Eq(MultiAreaIdentity.AreaModelVersionStage4,
                new MultiAreaContinuation(e, new MultiAreaModelSelection(A1, M1, C1, R0, S0)).ReadView().Source.AreaModelVersion, "1/1/1/0/0");
            T.Eq(stage5Token, new MultiAreaContinuation(e, new MultiAreaModelSelection(A1, M1, C1, R1, S0)).ReadView().Source.AreaModelVersion, "1/1/1/1/0");
            var phase2 = Phase2Session(e);
            T.Eq(stage5Token + "+species-v1", phase2.ReadView().Source.AreaModelVersion, "1/1/1/1/1");
            T.Eq(new MultiAreaContinuation(e).ReadView().Experiment.ManifestContentHash, phase2.ReadView().Experiment.ManifestContentHash,
                "manifest hash unchanged");
            foreach (var selection in new[] { MultiAreaModelSelection.Legacy, MultiAreaModelSelection.Stage3CrownC2, MultiAreaModelSelection.Stage4,
                MultiAreaModelSelection.Stage5 })
            {
                T.True(Root(new MultiAreaContinuation(e, selection)).Phase2 is null, "no Phase 2 state on " + selection);
                T.Eq(S0, selection.Species, "the four earlier static selections have Species 0");
            }
            T.True(Root(phase2).Phase2 is not null && Root(phase2).Stage5 is not null && Root(phase2).Stage4 is not null,
                "1/1/1/1/1 carries Stage 4, Stage 5 and Phase 2 state");
        });

        T.Add("P2-01 F-03: RequireSupported accepts exactly the five spec §2 tuples; every other Species 1 combination is SpeciesModelIncomplete, earlier refusals are unchanged, unknown values are refused; Phase 2 options, reads and commands need 1/1/1/1/1", () =>
        {
            var e = MultiAreaGenerator.Generate();
            int accepted = 0;
            foreach (var a in new[] { A0, A1 })
            foreach (var m in new[] { M0, M1 })
            foreach (var c in new[] { C0, C1 })
            foreach (var r in new[] { R0, R1 })
            foreach (var s in new[] { S0, S1 })
            {
                var selection = new MultiAreaModelSelection(a, m, c, r, s);
                string tuple = $"{(int)a}/{(int)m}/{(int)c}/{(int)r}/{(int)s}";
                bool supported = tuple is "0/0/0/0/0" or "0/0/1/0/0" or "1/1/1/0/0" or "1/1/1/1/0" or "1/1/1/1/1";
                if (supported)
                {
                    T.Eq(selection, selection.RequireSupported(), "accepted " + tuple);
                    accepted++;
                }
                else if (s == S1) T.Refuses("SpeciesModelIncomplete", () => new MultiAreaContinuation(e, selection));
                else if (r == R1) T.Refuses("Stage5ModelIncomplete", () => new MultiAreaContinuation(e, selection));
                else T.Refuses("Stage4ModelIncomplete", () => new MultiAreaContinuation(e, selection));
            }
            T.Eq(5, accepted, "exactly five supported tuples");
            T.Refuses("SpeciesModelUnknown", () => new MultiAreaContinuation(e, new MultiAreaModelSelection(A1, M1, C1, R1, (SpeciesModelVersion)7)));
            T.Refuses("Phase2ModelRequired", () => new MultiAreaContinuation(e, MultiAreaModelSelection.Stage5, null, null, Phase2Options.Default));
            var stage5 = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage5);
            T.Refuses("Phase2ModelRequired", () => stage5.ReadPhase2View());
            T.Refuses("Phase2ModelRequired", () => stage5.AddPlant(Fence(stage5.ReadView()), "MA-A", Oak, 10f, 10f));
            T.Refuses("Phase2ModelRequired", () => stage5.AddSpotClear(Fence(stage5.ReadView()), "MA-A", 10f, 10f));
            T.Refuses("Phase2ModelRequired", () => stage5.AddShelter(Fence(stage5.ReadView()), "MA-A", 10f, 10f));
            T.Refuses("Phase2GenesisSpeciesInvalid", () => Phase2Session(e, null, Phase2Options.Default with
            {
                GenesisSpecies = new Dictionary<string, string>(StringComparer.Ordinal) { ["MA-A-001"] = Sitka },
            }));
            T.Refuses("Phase2GenesisSpeciesInvalid", () => Phase2Session(e, null, Phase2Options.Default with
            {
                GenesisSpecies = new Dictionary<string, string>(StringComparer.Ordinal) { ["MA-Z-999"] = Oak },
            }));
            T.True(MultiAreaModelSelection.Phase2.IsPhase2 && MultiAreaModelSelection.Phase2.IsStage5 && !MultiAreaModelSelection.Stage5.IsPhase2,
                "1/1/1/1/1 is Stage 5 plus Phase 2");
        });
    }

    // ============================================================================================ P2-02 broadleaf adults
    private static void BroadleafAdults()
    {
        T.Add("P2-02 A-01: Oak and Beech DBH and height increments equal spec §3.1 to float precision on hand-computed cases (open, competed, near Dmax and Hmax)", () =>
        {
            var cases = new (float Dbh, float H, float Ci)[] { (30f, 20f, 0f), (30f, 20f, 8f), (119.5f, 20f, 3f), (30f, 42.4f, 2f), (121f, 43f, 0f) };
            foreach (var species in Phase2Species.Broadleaves)
                foreach (var (dbh, h, ci) in cases)
                {
                    var grown = Phase2Adult.Grow(new AnnualTree("X", 0f, 0f, 50, dbh, h, 3f), species, ci, 1f, 0);
                    double dDbh = species.PotentialDbhGrowthCm * Math.Clamp(1.0 - dbh / (double)species.MaxDbhCm, 0, 1) / (1.0 + ci / (double)species.Ci50);
                    double dH = species.PotentialHeightGrowthM * Math.Clamp(1.0 - h / (double)species.MaxHeightM, 0, 1);
                    string what = $"{species.SpeciesId} DBH {dbh} H {h} CI {ci}";
                    Close(Math.Clamp(dbh + dDbh, 1, 200), grown.DbhCm, what + " DBH");
                    Close(Math.Clamp(h + dH, 0.1, 60), grown.HeightM, what + " height");
                    T.Eq(51, grown.AgeYears, what + " age");
                }
            // Beech's own growth Ci50 is 6.5: the competed increment is not the Ci50 = 5 one.
            var beech = Phase2Adult.Grow(new AnnualTree("B", 0f, 0f, 50, 30f, 20f, 3f), Phase2Species.Beech, 8f, 1f, 0);
            Close(30 + 0.9 * 0.75 / (1 + 8 / 6.5), beech.DbhCm, "Beech Ci50 6.5");
            T.True(Math.Abs(beech.DbhCm - (30 + 0.9 * 0.75 / (1 + 8 / 5.0))) > 1e-3, "Beech growth does not use Ci50 5");
        });

        T.Add("P2-02 A-02: Oak r_pot = 0.1 + 0.1045·DBH; Beech r_pot = sqrt(exp(0.05 + 1.01·ln(max(0.1, DBH)))/π) and Beech's linear asset fields are never read; C2 target and relaxation use the species Ci50 and k", () =>
        {
            foreach (float dbh in new[] { 0.05f, 1f, 10f, 35f, 80f, 150f })
            {
                Close(Math.Max(0.1, 0.1 + 0.1045 * dbh), Phase2Species.Oak.PotentialCrownRadiusM(dbh), "Oak r_pot " + dbh);
                Close(Math.Max(0.1, Math.Sqrt(Math.Exp(0.05 + 1.01 * Math.Log(Math.Max(0.1, dbh))) / Math.PI)), Phase2Species.Beech.PotentialCrownRadiusM(dbh),
                    "Beech r_pot " + dbh);
                var poisoned = Phase2Species.Beech with { CrownRadiusIntercept = 99f, CrownRadiusPerCmDbh = 99f };
                T.Eq(F32.Bits(Phase2Species.Beech.PotentialCrownRadiusM(dbh)), F32.Bits(poisoned.PotentialCrownRadiusM(dbh)),
                    "Beech never reads its linear fields at " + dbh);
            }
            T.Eq(0f, Phase2Species.Beech.CrownRadiusIntercept, "Beech's serialised linear intercept 0.9415 is not stored");
            T.Eq(0f, Phase2Species.Beech.CrownRadiusPerCmDbh, "Beech's serialised linear slope 0.07635 is not stored");
            foreach (var species in Phase2Species.Broadleaves)
                foreach (var (dbh, ci, r) in new[] { (30f, 0f, 2f), (30f, 6f, 5f), (60f, 12f, 3f) })
                {
                    double target = Math.Max(0.1, species.PotentialCrownRadiusM(dbh) / Math.Sqrt(1 + ci / (double)species.Ci50));
                    Close(target, Phase2Crown.TargetRadiusM(species, dbh, ci), species.SpeciesId + " C2 target");
                    Close(Math.Max(0.1, r + (target - r) * species.CrownRelaxation), Phase2Crown.NextRadiusM(species, r, dbh, ci, 0),
                        species.SpeciesId + " C2 relaxation k");
                }
            Close(0.12, Phase2Species.Oak.CrownRelaxation, "Oak k");
            Close(0.15, Phase2Species.Beech.CrownRelaxation, "Beech k");
        });

        T.Add("P2-02 A-03: in a 1/1/1/1/1 session Oak and Beech adults grow by §3.1 with the connected-woodland CI and C2 crowns; the light field has no species term", () =>
        {
            var c = MixedSession();
            var start = Root(c);
            var woodland = CrownModel.Woodland(start.Areas.SelectMany(a => a.State.Trees));
            var expected = start.Areas.SelectMany(a => a.State.Trees).Where(t => start.Phase2!.BroadleafOf(t.TreeId) is not null)
                .ToDictionary(t => t.TreeId, t => Phase2Adult.Grow(t, start.Phase2!.BroadleafOf(t.TreeId)!, AnnualRules.Competition(t, woodland).Ci, 1f, 0),
                    StringComparer.Ordinal);
            // Year 0 light equals the evaluator's light of the rosters, which takes no species input at all.
            var fields = CanopyLightEvaluator.LightFields(Inputs(c.ReadView()).Select(a => new CanopyArea(a.AreaId,
                Array.AsReadOnly(a.Polygon.Select(p => new CanopyVertex(p.XM, p.ZM)).ToArray()), a.AreaHa, a.Trees)).ToArray(), 2.5);
            for (int i = 0; i < fields.Count; i++)
                T.True(fields[i].Cells.Select(cell => cell.LightIndex).SequenceEqual(start.Stage5!.Areas[i].Cells.Select(cell => cell.Light)),
                    "genesis light is the species-free evaluator light");
            Advance(c);
            int oak = 0, beech = 0;
            foreach (var tree in c.ReadView().Areas.SelectMany(a => a.Trees).Where(t => expected.ContainsKey(t.TreeId) && t.IsLiving))
            {
                var e = expected[tree.TreeId];
                T.True(F32.Bits(e.DbhCm) == F32.Bits(tree.DbhCm) && F32.Bits(e.HeightM) == F32.Bits(tree.HeightM) &&
                    F32.Bits(e.CrownRadiusM) == F32.Bits(tree.CrownRadiusM) && e.AgeYears == tree.AgeYears, "broadleaf adult step " + tree.TreeId);
                if (Phase2Of(c).TreeSpecies[tree.TreeId] == Oak) oak++; else beech++;
            }
            T.True(oak > 0 && beech > 0, $"living Oak ({oak}) and Beech ({beech}) adults compared");
        });
    }

    // ============================================================================================ P2-03 mixed density
    private static void MixedDensity()
    {
        T.Add("P2-03 M-01: a 100 % Sitka rack inside a mixed woodland gives victims and ADULT-MORT draws identical to the Stage 4 path and draws no ADULT-MORT-GENERIC key", () =>
        {
            var c = MixedSession();
            int deaths = 0, rolls = 0;
            for (int year = 0; year < 6; year++)
            {
                var before = Root(c);
                var v = c.ReadView();
                var woodland = CrownModel.Woodland(before.Areas.SelectMany(a => a.State.Trees));
                var (expected, _) = MultiAreaAnnual.StepStage4(before.Areas[0].State, v.Areas[0].SimulationSeed, v.Areas[0].AreaHa, woodland,
                    id => before.Stage4!.PruningOf(id).Lifts, Stage4Options.Default);
                List<string> log;
                Phase2Random.DrawLog = new List<string>();
                MultiAreaAnnualResult result;
                try { result = Advance(c); }
                finally { log = Phase2Random.DrawLog!; Phase2Random.DrawLog = null; }
                var w1 = new Model0FingerprintTests.Fingerprint();
                var w2 = new Model0FingerprintTests.Fingerprint();
                w1.Report(expected);
                w2.Report(result.Areas.Single(a => a.AreaId == "MA-A").Report!);
                T.Eq(w1.Hash(), w2.Hash(), "Area A report (victims, rolls) equals the Stage 4 step, year " + (year + 1));
                T.True(!log.Any(key => key.StartsWith("ADULT-MORT-GENERIC-MA-A", StringComparison.Ordinal) || key == "ADULT-MORT-GENERIC-QUOTA-MA-A"),
                    "no generic key of Area A or its stems is drawn");
                var density = Phase2Of(c).Accounts[^1].Density;
                T.True(!density.Single(d => d.AreaId == "MA-A").Mixed && density.Single(d => d.AreaId == "MA-B").Mixed, "routes: A pure, B mixed");
                deaths += expected.Deaths.Count;
                rolls += expected.Trees.Count(row => row.MortalityRoll is not null);
            }
            T.True(deaths > 0 && rolls > 0, $"Area A draws ADULT-MORT rolls ({rolls}) and loses stems ({deaths})");
        });

        T.Add("P2-03 M-02: in a mixed rack the Sitka component's victims are only Sitka stems and equal the Stage 4 Comeau result on the Sitka subset", () =>
        {
            // Rack B: a dense Sitka block (x 41–55 m) and Oak more than 8 m from every Sitka stem, so the Sitka subset's Hegyi
            // CI and growth are exactly those of a Sitka-only rack of the same area.
            var b = new List<(int, int, int, int, int, string)>();
            for (int x = 41_000; x <= 55_000; x += 1_200)
                for (int z = 1_000; z <= 19_000; z += 1_200)
                    b.Add((x, z, 30, 250, 18_000, Sitka));
            b.Add((70_000, 5_000, 60, 400, 25_000, Oak));
            b.Add((70_000, 15_000, 60, 400, 25_000, Oak));
            b.Add((76_000, 10_000, 60, 380, 24_000, Beech));
            var (experiment, options) = TwoRacks("mad-p2-comeau", 40, 20, new[] { (10_000, 10_000, 30, 250, 18_000, Sitka) }, b.ToArray());
            var c = Phase2Session(experiment, null, options);
            var start = Root(c);
            var v = c.ReadView();
            var woodland = CrownModel.Woodland(start.Areas.SelectMany(a => a.State.Trees));
            var sitkaOnly = start.Areas[1].State with
            {
                Trees = Array.AsReadOnly(start.Areas[1].State.Trees.Where(t => start.Phase2!.BroadleafOf(t.TreeId) is null).ToArray()),
            };
            var comeau = AnnualRules.CalculateStage4(sitkaOnly, v.Areas[1].SimulationSeed, v.Areas[1].AreaHa, woodland, _ => 0, Stage4Options.Default);
            var mixed = Advance(c).Areas[1].Report!;
            var sitkaDeaths = mixed.Deaths.Where(d => d.Mechanism != Phase2MixedDensity.GenericMechanism).ToArray();
            T.True(comeau.Deaths.Count > 0, "the Sitka subset is over the density line (" + comeau.Deaths.Count + " deaths)");
            T.True(sitkaDeaths.All(d => start.Phase2!.BroadleafOf(d.TreeId) is null), "Sitka-component victims are Sitka stems");
            T.Eq(string.Join(",", comeau.Deaths.Select(d => d.TreeId + "/" + d.Mechanism)), string.Join(",", sitkaDeaths.Select(d => d.TreeId + "/" + d.Mechanism)),
                "Sitka-component victims equal the Comeau result on the Sitka subset");
            foreach (var row in comeau.Trees)
            {
                var other = mixed.Trees.Single(r => r.Before.TreeId == row.Before.TreeId);
                T.Eq(row.MortalityRoll, other.MortalityRoll, "same ADULT-MORT roll " + row.Before.TreeId);
                T.Eq(row.DeathProbability, other.DeathProbability, "same hazard " + row.Before.TreeId);
            }
            var generic = mixed.Deaths.Where(d => d.Mechanism == Phase2MixedDensity.GenericMechanism).Select(d => d.TreeId).ToHashSet();
            T.True(!generic.Overlaps(sitkaDeaths.Select(d => d.TreeId)), "generic and Sitka victim sets are disjoint");
            Console.WriteLine($"  P2-03 M-02 Sitka-subset deaths {sitkaDeaths.Length}, generic deaths {generic.Count}");
        });

        T.Add("P2-03 M-03: generic occupancy, G(O), nonReferenceShare, quota and remainder roll match spec §5.1 on hand-computed cases", () =>
        {
            Phase2DensityStem S(string id, double w, bool sitka, float dbh, double ci) => new(id, w, sitka, dbh, ci);
            var stems = new[] { S("a", 1.0, true, 30f, 2), S("b", 2.0, false, 40f, 3), S("c", 1.5, false, 35f, 1), S("d", 1.0, true, 20f, 6) };
            const float area = 0.005f;
            double Space(Phase2DensityStem s) => s.Weight * Math.PI / 4 * (s.DbhCm / 100.0) * (s.DbhCm / 100.0);
            double total = stems.Sum(Space), broad = stems.Where(s => !s.IsSitka).Sum(Space);
            double o = total / area / 55;
            double g = 0.5 * (1 - Math.Exp(-1.6 * Math.Pow(Math.Max(0, o - 1), 2)));
            foreach (var sitkaVictims in new[] { new HashSet<int>(), new HashSet<int> { 0 } })
            {
                double expected = g * (broad / total) * (stems.Length - sitkaVictims.Count);
                int floor = (int)Math.Floor(expected + 1e-10);
                double remainder = expected - floor;
                T.True(remainder > 1e-6 && o > 1, $"case has O {o:F3} > 1 and a fractional remainder {remainder:F4}");
                foreach (var (quotaRoll, bump) in new[] { (0.0, 1), (0.999, 0) })
                {
                    var drawn = new List<string>();
                    var plan = Phase2MixedDensity.PlanGeneric(stems, area, sitkaVictims, key => { drawn.Add(key); return key == Phase2MixedDensity.QuotaKeyToken ? quotaRoll : 0.5; });
                    Close(o, plan.Occupancy, "occupancy");
                    Close(g, plan.GenericResponse, "G(O)");
                    Close(broad / total, plan.NonReferenceShare, "nonReferenceShare");
                    T.Eq(stems.Length - sitkaVictims.Count, plan.EligibleForGeneric, "eligible = stems − Sitka victims");
                    Close(expected, plan.ExpectedCount, "expected");
                    Close(remainder, plan.Remainder, "remainder");
                    T.Eq(floor + bump, plan.Quota, "quota = floor + [roll < remainder] with roll " + quotaRoll);
                    T.Eq(Phase2MixedDensity.QuotaKeyToken, drawn[0], "the quota remainder is drawn first, from its own key");
                    T.True(plan.VictimIndices.All(i => !sitkaVictims.Contains(i)), "Sitka victims are not eligible");
                }
            }
            // At or below the onset nothing is drawn.
            var sparse = Phase2MixedDensity.PlanGeneric(stems, 1f, new HashSet<int>(), _ => throw new Exception("no draw below the onset"));
            T.True(sparse.Occupancy <= 1 && sparse.GenericResponse == 0 && sparse.Quota == 0, "G = 0 at O ≤ 1");
            Close(0.5 * (1 - Math.Exp(-1.6)), Phase2MixedDensity.GenericLoss(2.0), "G(2)");
        });

        T.Add("P2-03 M-04: neutrality: a Beech and a Sitka stem with equal CI and DBH carry equal victim weight (Ci50 = 5 for every species), while Beech growth uses Ci50 = 6.5", () =>
        {
            Close(Math.Pow(4.0 / 9.0, 4), Phase2MixedDensity.Vulnerability(Phase2MixedDensity.Suppression(4.0)), "weight S⁴ with S = CI/(CI + 5)");
            foreach (var (beechId, sitkaId) in new[] { ("A", "B"), ("B", "A") })
            {
                var stems = new[] { new Phase2DensityStem(beechId, 1.5, false, 50f, 4.0), new Phase2DensityStem(sitkaId, 1.0, true, 50f, 4.0) }
                    .OrderBy(s => s.TreeId, StringComparer.Ordinal).ToArray();
                // Expected generic count 0.6 (G ≈ 0.5, share 0.6, two eligible): a quota draw of 0 rounds it up to one stem.
                var plan = Phase2MixedDensity.PlanGeneric(stems, 0.0005f, new HashSet<int>(), key => key == Phase2MixedDensity.QuotaKeyToken ? 0.0 : 0.3);
                T.True(plan.Quota >= 1, "the generic component removes a stem");
                T.Eq("A", stems[plan.VictimIndices[0]].TreeId, "equal weight and equal draw: the ordinal TreeId decides, not the species");
            }
            var beech = Phase2Adult.Grow(new AnnualTree("B", 0f, 0f, 50, 30f, 20f, 3f), Phase2Species.Beech, 4f, 1f, 0);
            Close(30 + 0.9 * 0.75 / (1 + 4 / 6.5), beech.DbhCm, "Beech growth response uses Ci50 6.5");
        });

        T.Add("P2-03 M-05: generic removal never exceeds 0.125 of stand basal area in a year except the documented one-stem fallback (hand cases and the 30-year MA-1-MIXED run)", () =>
        {
            // Few large stems: each alone is above the budget, so exactly one stem is taken (fallback).
            var large = Enumerable.Range(0, 4).Select(i => new Phase2DensityStem("L" + i, i % 2 == 0 ? 2.0 : 1.0, i % 2 == 1, 90f, 3.0)).ToArray();
            var fallback = Phase2MixedDensity.PlanGeneric(large, 0.002f, new HashSet<int>(), key => key == Phase2MixedDensity.QuotaKeyToken ? 0.9 : 0.5);
            T.True(fallback.Quota >= 1 && fallback.SingleStemFallback && fallback.VictimIndices.Count == 1, "fallback takes exactly one stem");
            // Ten equal stems (one Sitka, nine Oak) in a tiny rack: the quota is several stems, but 0.125 of the stand's basal
            // area holds only one of them (two would be 0.2), so the budget binds and exactly one is taken without fallback.
            var many = Enumerable.Range(0, 10).Select(i => new Phase2DensityStem("M" + i.ToString("00"), i == 0 ? 1.0 : 2.0, i == 0, 30f, i)).ToArray();
            var bound = Phase2MixedDensity.PlanGeneric(many, 0.001f, new HashSet<int>(), key => key == Phase2MixedDensity.QuotaKeyToken ? 0.9 : 0.4);
            T.True(bound.Quota >= 2, "the quota asks for several stems (" + bound.Quota + ")");
            T.True(!bound.SingleStemFallback && bound.VictimIndices.Count == 1 && bound.BudgetPassedOver > 0, "the budget binds: one stem fits");
            T.True(bound.GenericBasalAreaM2 <= 0.125 * bound.StandBasalAreaM2 + 1e-12, "budget respected");
            int mixedYears = 0, genericDeaths = 0;
            foreach (var record in Cached.Value.Phase2.Accounts.SelectMany(y => y.Density).Where(d => d.Mixed))
            {
                mixedYears++;
                genericDeaths += record.GenericDeaths;
                if (record.SingleStemFallback) T.Eq(1, record.GenericDeaths, "fallback removes one stem");
                else T.True(record.GenericBasalAreaM2 <= 0.125 * record.StandBasalAreaM2 + 1e-12, "BA budget " + record.AreaId);
            }
            T.True(mixedYears > 0 && genericDeaths > 0, $"MA-1-MIXED has mixed rack-years ({mixedYears}) and generic deaths ({genericDeaths})");
            Console.WriteLine($"  P2-03 M-05 MA-1-MIXED 30 y: mixed rack-years {mixedYears}, generic deaths {genericDeaths}");
        });

        T.Add("P2-03 M-06: broadleaf pruning: at most two lifts, three years apart, at exactly 3.5 m then 6.0 m (Unity CanPrune guard order); Sitka pruning unchanged", () =>
        {
            var tall = new AnnualTree("O", 0f, 0f, 60, 40f, 20f, 4f);
            var oak = Phase2Species.Oak;
            T.Eq("PruneBroadleafTargetInvalid", Phase2Pruning.RefusalCode(tall, TreePruningState.Unpruned, 3f, 10, oak), "lift 1 must be 3.5 m");
            T.Eq(null, Phase2Pruning.RefusalCode(tall, TreePruningState.Unpruned, 3.5f, 10, oak), "lift 1 at 3.5 m");
            var one = new TreePruningState(1, 3.5f, 10);
            T.Eq("PruneIntervalTooShort", Phase2Pruning.RefusalCode(tall, one, 6f, 12, oak), "three-year interval");
            T.Eq(null, Phase2Pruning.RefusalCode(tall, one, 6f, 13, oak), "lift 2 at 6.0 m after three years");
            T.Eq("PruneTargetNotAboveCrownBase", Phase2Pruning.RefusalCode(tall, one, 3.5f, 13, oak), "target above the current base");
            T.Eq("PruneBroadleafTargetInvalid", Phase2Pruning.RefusalCode(tall, one, 5f, 13, oak), "lift 2 must be 6.0 m");
            T.Eq("PruneMaxLiftsReached", Phase2Pruning.RefusalCode(tall, new TreePruningState(2, 6f, 13), 7f, 30, oak), "two lifts at most");
            T.Eq("PruneTargetOutOfRange", Phase2Pruning.RefusalCode(tall with { HeightM = 9f }, one, 6f, 13, Phase2Species.Beech), "below 60 % of height");
            T.Eq(3.5f, Phase2Pruning.NextLiftTargetM(0, oak), "schedule 1");
            T.Eq(6f, Phase2Pruning.NextLiftTargetM(1, oak), "schedule 2");
            T.True(float.IsNaN(Phase2Pruning.NextLiftTargetM(2, oak)), "no third lift");
            foreach (var state in new[] { TreePruningState.Unpruned, new TreePruningState(1, 2.5f, 3), new TreePruningState(3, 6.5f, 1) })
                foreach (float target in new[] { 2.5f, 3f, 5f, 6.5f, 13f })
                    foreach (int year in new[] { 4, 8, 20 })
                        T.Eq(PruningRules.RefusalCode(tall, state, target, year), Phase2Pruning.RefusalCode(tall, state, target, year, null),
                            "Sitka rules unchanged");
            // In a session: the scheduled broadleaf lift is 3.5 m, the cut is 8 %, the C2 target carries 0.92^lifts.
            var c = MixedSession();
            var p2 = Phase2Of(c);
            var prunee = c.ReadView().Areas[1].Trees.Where(t => t.IsLiving && p2.BroadleafOf(t.TreeId) == oak).OrderByDescending(t => t.DbhCm).First();
            var v = c.AddScheduledPrune(Fence(c.ReadView()), "MA-B", prunee.TreeId).View;
            T.Eq(3.5f, c.ReadStage4View().PendingPrunes.Single().TargetCrownBaseHeightM, "scheduled broadleaf lift");
            T.Refuses("PruneBroadleafTargetInvalid", () => c.AddPrune(Fence(c.ReadView()), "MA-B",
                c.ReadView().Areas[1].Trees.First(t => t.IsLiving && p2.BroadleafOf(t.TreeId) is not null && t.TreeId != prunee.TreeId).TreeId, 2.5f));
            ApproveAndExecute(c);
            var cut = c.ReadView().Areas[1].Trees.Single(t => t.TreeId == prunee.TreeId);
            T.Eq(F32.Bits(MathF.Max(0.1f, prunee.CrownRadiusM * 0.92f)), F32.Bits(cut.CrownRadiusM), "one lift cuts 8 % of the radius");
            var start = Root(c);
            var woodland = CrownModel.Woodland(start.Areas.SelectMany(a => a.State.Trees));
            var expected = Phase2Adult.Grow(cut, oak, AnnualRules.Competition(cut, woodland).Ci, 1f, 1);
            Advance(c);
            var after = c.ReadView().Areas[1].Trees.Single(t => t.TreeId == prunee.TreeId);
            if (after.IsLiving) T.Eq(F32.Bits(expected.CrownRadiusM), F32.Bits(after.CrownRadiusM), "C2 prunee × 0.92^lifts for a broadleaf");
            T.Refuses("PruneIntervalTooShort", () => c.AddPrune(Fence(c.ReadView()), "MA-B", prunee.TreeId, 6f));
        });

        T.Add("P2-03 M-07: every density death writes one fallen-deadwood record carrying its SpeciesId and volume DBH²·0.00007854·H·0.5", () =>
        {
            var run = Cached.Value;
            var bySpecies = new Dictionary<string, int>(StringComparer.Ordinal);
            var trees = run.Session.ReadView().Areas.SelectMany(a => a.Trees).ToDictionary(t => t.TreeId, StringComparer.Ordinal);
            foreach (var d in run.Session.ReadStage4View().Areas.SelectMany(a => a.Deadwood))
            {
                T.Eq(run.Phase2.SpeciesIdOf(d.TreeId), d.SpeciesId, "deadwood species " + d.TreeId);
                var tree = trees[d.TreeId];
                T.True(!tree.IsLiving && tree.Death!.Cause == LifeStates.DensitySuppression, "a density death");
                T.Eq(F32.Bits(tree.DbhCm), F32.Bits(d.DbhCm), "DBH at death");
                T.Eq(F32.Bits(d.DbhCm * d.DbhCm * 0.00007854f * (d.HeightM * 0.5f)), F32.Bits(d.EstimatedVolumeM3), "volume " + d.DeadwoodId);
                bySpecies[d.SpeciesId] = bySpecies.GetValueOrDefault(d.SpeciesId) + 1;
            }
            int densityDeaths = trees.Values.Count(t => t.Death?.Cause == LifeStates.DensitySuppression);
            T.Eq(densityDeaths, bySpecies.Values.Sum(), "exactly one record per density death");
            T.True(bySpecies.GetValueOrDefault(Oak) > 0 && bySpecies.GetValueOrDefault(Beech) > 0 && bySpecies.GetValueOrDefault(Sitka) > 0,
                "records for all three species: " + string.Join(", ", bySpecies.Select(p => p.Key + " " + p.Value)));
            Console.WriteLine("  P2-03 M-07 deadwood by species: " + string.Join(", ", bySpecies.OrderBy(p => p.Key).Select(p => p.Key + " " + p.Value)));
        });

        T.Add("P2-03 M-08 (Q6, mandatory): 1/1/1/1/1 on unmodified all-Sitka MA-1 equals 1/1/1/1/0 identically in trees, bands and accounts; only the version token differs", () =>
        {
            string Run(MultiAreaContinuation c)
            {
                var w = new Model0FingerprintTests.Fingerprint();
                ThinEverySecond(c);
                for (int year = 0; year < 30; year++)
                {
                    if (year == 5)
                    {
                        var cell = c.ReadStage5View().Areas[1].Cells[30];
                        var v = c.AddClearUndergrowth(Fence(c.ReadView()), "MA-B", cell.Column, cell.Row).View;
                        v = c.Approve(Fence(v)).View;
                        c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
                    }
                    foreach (var area in Advance(c).Areas)
                    {
                        w.Text(area.AreaId);
                        if (area.Report is { } report) w.Report(report); else w.Text("carried");
                    }
                }
                foreach (var area in c.ReadView().Areas) w.Trees(area.Trees);
                WriteState(w, Root(c).Stage5!);
                WriteStage4(w, c.ReadStage4View());
                return w.Hash();
            }
            var stage5 = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage5);
            var phase2 = Phase2Session();
            string h0 = Run(stage5), h1 = Run(phase2);
            Console.WriteLine("  P2-03 M-08 Q6 hash 1/1/1/1/0 " + h0 + " | 1/1/1/1/1 " + h1);
            T.Eq(h0, h1, "trees, Stage 5 bands, accounts, blocks and deadwood identical");
            T.Eq(stage5.ReadView().Source.AreaModelVersion + "+species-v1", phase2.ReadView().Source.AreaModelVersion, "only the token differs");
            T.True(Phase2Of(phase2).TreeSpecies.Count == 0 && Phase2Of(phase2).Areas.All(a => a.Cells.All(cell => cell.Bands.Count == 0)) &&
                Phase2Of(phase2).Accounts.SelectMany(y => y.Density).All(d => !d.Mixed), "no broadleaf adult, band or mixed route arose");
        });
    }

    // ============================================================================================ P2-04 commands
    private static void Commands()
    {
        T.Add("P2-04 C-01: Plant/Shelter/SpotClear: Pending and Approve mutate nothing; Execute never advances the year; engine-minted key, idempotent replay, fence and revision enforced; no economy or stock field exists", () =>
        {
            var c = MixedSession();
            var p = FindPoint(c, "MA-A", 1.0f);
            var before = Root(c);
            var v = c.AddSpotClear(Fence(c.ReadView()), "MA-A", p.X, p.Z).View;
            v = c.AddPlant(Fence(v), "MA-A", Oak, p.X, p.Z).View;
            T.Refuses("PlayerPlanStateStale", () => c.AddShelter(Fence(c.ReadView()) with { PlanStateRevision = 0 }, "MA-A", p.X, p.Z));
            v = c.AddShelter(Fence(v), "MA-A", p.X, p.Z).View;
            void Unchanged(string stage)
            {
                var now = Root(c);
                T.True(ReferenceEquals(before.Areas, now.Areas) && ReferenceEquals(before.Stage5!.Areas, now.Stage5!.Areas) &&
                    now.EcologicalYear == 0 && now.WorkRevision == 0, stage + ": no tree, ground cell, year or work revision changed");
                T.True(now.Phase2!.Juveniles.Count == 0 && now.Phase2.Shelters.Count == 0 && now.Phase2.Patches.Count == 0, stage + ": nothing planted");
            }
            Unchanged("Pending");
            T.Eq("P2W0001,P2W0002,P2W0003", string.Join(",", Phase2Of(c).PendingOrders.Select(o => o.OrderId)), "engine-minted order IDs");
            v = c.Approve(Fence(v)).View;
            Unchanged("Approve");
            T.Eq(3, Phase2Of(c).ApprovedOrders.Count, "approved batch");
            T.Refuses("WorkExecutionKeyConflict", () => c.Execute(Fence(v), v.Approved!.PlanId, "my-own-key"));
            var executed = c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            var p2 = Phase2Of(c);
            T.Eq(0, executed.View.Source.EcologicalYear, "Execute never advances the year");
            T.Eq(1, executed.View.Source.WorkRevision, "one work revision");
            T.Eq("SpotClear,Plant,Shelter", string.Join(",", p2.WorkLog.Select(r => r.Kind)), "resolve order SpotClear → Plant → Shelter");
            var j = p2.Juveniles.Single();
            T.True(j.JuvenileId == "PJ0001" && j.AgeYears == 3f && j.HeightM == 0.6f && j.PlantingYear == 1 && j.XM == p.X && j.ZM == p.Z && j.IsActive,
                "nursery juvenile PJ0001: age 3, 0.6 m, exact point");
            var s = p2.Shelters.Single();
            T.True(s.ShelterId == "S-JPJ0001" && s.InstalledYear == 1 && s.EffectiveYears == 8 && s.FailedYear == -1, "shelter S-JPJ0001");
            var patch = p2.Patches.Single();
            T.True(patch.CreatedYear == 1 && patch.UpdatedYear == 1 && patch.Bramble == 0f && patch.Bracken == 0f, "patch at 0, guard year 1");
            T.True(p2.PendingOrders.Count == 0 && p2.ApprovedOrders.Count == 0, "the batch is consumed");
            var committed = Root(c);
            var replay = c.Execute(Fence(v), v.Approved.PlanId, v.Approved.ExecutionKey);
            T.True(replay.Execution!.Replayed && ReferenceEquals(committed, Root(c)), "a replay with the same key is answered and changes nothing");
            T.Refuses("PlayerSourceStale", () => c.AddPlant(Fence(v), "MA-A", Beech, p.X + 3f, p.Z));
            var banned = new[] { "cash", "cost", "price", "stock", "minute", "cent", "money", "fee", "offer", "inventory", "ledger" };
            foreach (var type in new[] { typeof(MultiAreaPhase2State), typeof(MultiAreaPhase2View), typeof(MultiAreaPlantOrder), typeof(MultiAreaShelterOrder),
                typeof(MultiAreaSpotClearOrder), typeof(Phase2WorkRecord), typeof(PlantedJuvenile), typeof(BrowseShelter), typeof(SpotPatch),
                typeof(Phase2YearAccount), typeof(Phase2PlantedAccount), typeof(Phase2Options) })
                foreach (var property in type.GetProperties())
                    T.True(!banned.Any(word => property.Name.Contains(word, StringComparison.OrdinalIgnoreCase)), "no economy/stock field " + type.Name + "." + property.Name);
        });

        T.Add("P2-04 C-02: whole-batch refusals: out-of-polygon, < 0.5 m from another planting (lower work-order id wins), < 0.7 m from a living adult stem (new owner rule), duplicate order, shelter with no target or a second effective shelter, Species 0; a refused command or batch changes nothing", () =>
        {
            var c = MixedSession();
            var p = FindPoint(c, "MA-A", 1.2f);
            void Refused(string code, Func<MultiAreaFence, MultiAreaCommandResult> command)
            {
                var before = Root(c);
                T.Refuses(code, () => command(Fence(c.ReadView())));
                T.True(ReferenceEquals(before, Root(c)), code + " changed nothing");
            }
            Refused("Phase2PointOutsideArea", f => c.AddPlant(f, "MA-A", Oak, -5f, -5f));
            var inB = FindPoint(c, "MA-B", 1.0f);
            Refused("Phase2PointOutsideArea", f => c.AddSpotClear(f, "MA-A", inB.X, inB.Z));
            Refused("Phase2CoordinatesInvalid", f => c.AddPlant(f, "MA-A", Oak, float.NaN, 1f));
            Refused("MultiAreaUnknownArea", f => c.AddPlant(f, "MA-Q", Oak, p.X, p.Z));
            Refused("Phase2SpeciesUnknown", f => c.AddPlant(f, "MA-A", Sitka, p.X, p.Z));
            c.AddPlant(Fence(c.ReadView()), "MA-A", Oak, p.X, p.Z);
            try { c.AddPlant(Fence(c.ReadView()), "MA-A", Beech, p.X + 0.4f, p.Z); throw new Exception("expected a spacing refusal"); }
            catch (P1Refusal e)
            {
                T.Eq("PlantSpacingTooClose", e.Code, "0.4 m from another planting");
                T.True(e.Message.Contains("P2W0002", StringComparison.Ordinal) && e.Message.Contains("P2W0001", StringComparison.Ordinal),
                    "the later order is refused; the lower work-order id wins");
            }
            var adult = c.ReadView().Areas[0].Trees.Where(t => t.IsLiving).First(t =>
                Stage5Promotion.StrictlyInside(c.ReadView().Areas[0].Polygon, t.XM + 0.5f, t.ZM) &&
                (t.XM + 0.5f - p.X) * (t.XM + 0.5f - p.X) + (t.ZM - p.Z) * (t.ZM - p.Z) > 1f &&
                Phase2Annual.Locator(Stage5Of(c))(t.XM + 0.5f, t.ZM) is not null);
            Refused("PlantTooCloseToAdult", f => c.AddPlant(f, "MA-A", Oak, adult.XM + 0.5f, adult.ZM));
            var q = FindPoint(c, "MA-A", 1.0f, (x, z) => (x - p.X) * (x - p.X) + (z - p.Z) * (z - p.Z) > 9f);
            c.AddSpotClear(Fence(c.ReadView()), "MA-A", q.X, q.Z);
            Refused("Phase2DuplicateOrder", f => c.AddSpotClear(f, "MA-A", q.X, q.Z));
            Refused("ShelterNoTarget", f => c.AddShelter(f, "MA-A", q.X, q.Z));
            c.AddShelter(Fence(c.ReadView()), "MA-A", p.X, p.Z);
            Refused("ShelterAlreadyPresent", f => c.AddShelter(f, "MA-A", p.X + 0.1f, p.Z));
            ApproveAndExecute(c);
            Refused("ShelterAlreadyPresent", f => c.AddShelter(f, "MA-A", p.X, p.Z));
            Refused("PlantSpacingTooClose", f => c.AddPlant(f, "MA-A", Beech, p.X, p.Z + 0.3f));
            // The whole batch at Execute: a valid SpotClear and Plant plus a later Plant 0.3 m away injected after approval.
            var r = FindPoint(c, "MA-A", 1.2f, (x, z) => (x - p.X) * (x - p.X) + (z - p.Z) * (z - p.Z) > 16f && (x - q.X) * (x - q.X) + (z - q.Z) * (z - q.Z) > 16f);
            c.AddSpotClear(Fence(c.ReadView()), "MA-A", r.X, r.Z);
            c.AddPlant(Fence(c.ReadView()), "MA-A", Oak, r.X, r.Z);
            var v = c.Approve(Fence(c.ReadView())).View;
            var root = Root(c);
            RootField.SetValue(c, root with
            {
                Phase2 = root.Phase2! with
                {
                    ApprovedOrders = Array.AsReadOnly(root.Phase2.ApprovedOrders.Append(new MultiAreaPlantOrder("P2W9999", "MA-A", Beech, r.X + 0.3f, r.Z)).ToArray()),
                },
            });
            var injected = Root(c);
            T.Refuses("PlantSpacingTooClose", () => c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey));
            T.True(ReferenceEquals(injected, Root(c)) && Phase2Of(c).Juveniles.Count == 1 && Phase2Of(c).Patches.Count == 1,
                "the whole batch is refused: no patch, juvenile or shelter of it exists");
            var stage5 = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage5);
            T.Refuses("Phase2ModelRequired", () => stage5.AddPlant(Fence(stage5.ReadView()), "MA-A", Oak, p.X, p.Z));
        });
    }

    // ============================================================================================ P2-05 browsing and shelters
    private static void BrowsingAndShelters()
    {
        T.Add("P2-05 B-01: at pressure 0.2 an unprotected planted Oak loses height and survival per spec §6.2 (p, 0.90 increment loss, 0.96 survival) and a sheltered one at the same point does not (access 0)", () =>
        {
            var inputs = EmptySquares();
            var light = FixedLight(inputs, 2.5, (_, _) => 0.3f);
            var points = new List<(string Area, float X, float Z)>();
            foreach (var (area, x0) in new[] { ("MA-A", 0f), ("MA-B", 10f) })
                for (int k = 0; k < 8; k++) points.Add((area, x0 + 1.1f + (k % 4) * 2.4f, 2.1f + (k / 4) * 5f));
            var juveniles = points.Select((pt, i) => Juvenile("PJ" + (i + 1).ToString("0000"), pt.Area, Oak, pt.X, pt.Z, 3f, 0.6f)).ToArray();
            var (s5, s2) = EmptyStates(inputs);
            var open = (S5: s5, S2: s2 with { Juveniles = juveniles });
            var sheltered = (S5: s5, S2: s2 with
            {
                Juveniles = juveniles,
                Shelters = juveniles.Select(j => new BrowseShelter("S-J" + j.JuvenileId, j.JuvenileId, j.XM, j.ZM, 1, 8, -1)).ToArray(),
            });
            var seeds = inputs.ToDictionary(a => a.AreaId, a => a.SimulationSeed);
            int browsedEvents = 0, checkedYears = 0;
            for (int year = 1; year <= 6; year++)
            {
                var o = Phase2Annual.Step(open.S5, open.S2, inputs, year, Array.Empty<Phase2DensityRecord>(), light);
                var sh = Phase2Annual.Step(sheltered.S5, sheltered.S2, inputs, year, Array.Empty<Phase2DensityRecord>(), light);
                for (int n = 0; n < juveniles.Length; n++)
                {
                    var (prev, now, prevS, nowS) = (open.S2.Juveniles[n], o.NextPhase2.Juveniles[n], sheltered.S2.Juveniles[n], sh.NextPhase2.Juveniles[n]);
                    int seed = seeds[prev.AreaId];
                    if (prev.IsActive && now.DeathCause != "vegetation")
                    {
                        checkedYears++;
                        // Unity evaluates the taper width as escape − full in float (1.8f − 1.2f), not the literal 0.6f.
                        float v = prev.HeightM <= 1.2f ? 1f : prev.HeightM >= 1.8f ? 0f : 1f - (prev.HeightM - 1.2f) / (1.8f - 1.2f);
                        float p = MathF.Min(0.95f, 0.2f * 1f * v * 1f * 1f);
                        T.Eq(F32.Bits(p), F32.Bits(now.LastBrowseProbability), "unprotected p = min(0.95, 0.2 · 1.0 · V(h))");
                        bool browsed = p > 0f && SimulationRandom.Roll(DerivedManifest.RngModelVersion, "PHASE2-PJ-" + prev.JuvenileId + "/browse", year, seed) < p;
                        T.Eq(browsed, now.LastYearBrowsed, "deterministic browse event");
                        if (browsed) browsedEvents++;
                        float lr = Phase2Juvenile.LightResponse(Phase2Species.Oak, 0.3f);
                        double survival = Phase2Juvenile.LightSurvival(Phase2Species.Oak, 0.3f) * (browsed ? 1 - 0.04 : 1);
                        float roll = SimulationRandom.Roll(DerivedManifest.RngModelVersion, "PHASE2-PJ-" + prev.JuvenileId, year, seed);
                        T.Eq(roll < (float)survival, now.Alive, "survival · (browsed ? 0.96 : 1)");
                        if (now.Alive)
                            Close(prev.HeightM + 0.35 * lr * (browsed ? 0.1 : 1.0), now.HeightM, "height increment · (1 − 0.90 · browsed)");
                    }
                    if (prevS.IsActive && nowS.DeathCause != "vegetation")
                    {
                        T.Eq(0f, nowS.LastBrowseProbability, "sheltered: access 0 so p = 0");
                        T.True(!nowS.LastYearBrowsed, "sheltered: never browsed");
                        if (nowS.Alive)
                            T.Eq(F32.Bits(Phase2Juvenile.GrowHeight(Phase2Species.Oak, prevS.HeightM, 0.3f, 0f)), F32.Bits(nowS.HeightM),
                                "sheltered: the light-only height");
                    }
                }
                open = (o.NextStage5, o.NextPhase2);
                sheltered = (sh.NextStage5, sh.NextPhase2);
            }
            T.True(browsedEvents > 0 && checkedYears > 20, $"browse events happened ({browsedEvents} in {checkedYears} juvenile-years)");
            Console.WriteLine($"  P2-05 B-01 unprotected browse events {browsedEvents} in {checkedYears} juvenile-years; sheltered 0");
        });

        T.Add("P2-05 B-02: a shelter is effective in years installed … installed+7 and not in installed+8 (strict <); FailedYear stays -1; access is binary", () =>
        {
            var shelter = new BrowseShelter("S-JPJ0001", "PJ0001", 3f, 3f, 5, 8, -1);
            T.True(!shelter.IsEffective(4), "not before installation");
            for (int y = 5; y <= 12; y++) T.True(shelter.IsEffective(y), "effective in year " + y);
            T.True(!shelter.IsEffective(13), "NOT effective in installed + 8 (strict <)");
            var list = new[] { shelter };
            T.Eq(0f, Phase2Annual.AccessAt(list, 3f, 3f, 12), "access 0 while effective");
            T.Eq(1f, Phase2Annual.AccessAt(list, 3f, 3f, 13), "access 1 after expiry");
            T.Eq(0f, Phase2Annual.AccessAt(list, 3.25f, 3f, 8), "within 0.25 m matches");
            T.Eq(1f, Phase2Annual.AccessAt(list, 3.3f, 3f, 8), "beyond 0.25 m does not");
            var c = MixedSession();
            var p = FindPoint(c, "MA-A", 1.0f);
            c.AddPlant(Fence(c.ReadView()), "MA-A", Oak, p.X, p.Z);
            c.AddShelter(Fence(c.ReadView()), "MA-A", p.X, p.Z);
            ApproveAndExecute(c);
            var accessSeen = new HashSet<float>();
            for (int y = 1; y <= 10; y++)
            {
                Advance(c);
                var s = Phase2Of(c).Shelters.Single();
                T.True(s.FailedYear == -1 && s.InstalledYear == 1 && s.EffectiveYears == 8, "FailedYear is never set (no failure mechanism)");
                accessSeen.Add(Phase2Annual.AccessAt(Phase2Of(c).Shelters, p.X, p.Z, y));
            }
            T.True(accessSeen.SetEquals(new[] { 0f, 1f }), "access takes only the values 0 and 1 (effective to year 8, not year 9)");
        });

        T.Add("P2-05 B-03: Oak palatability 1.0 vs Beech 0.5 is visible in p; V(h) is full at ≤ 1.2 m and zero at ≥ 1.8 m; cohorts ignore shelters; pressure 0 is bit-identical to the light-only path", () =>
        {
            T.Eq(F32.Bits(0.2f), F32.Bits(Phase2Juvenile.BrowseProbability(Phase2Species.Oak, 0.2f, 0.6f, 1f)), "Oak p = 0.2 × 1.0");
            T.Eq(F32.Bits(0.2f * 0.5f), F32.Bits(Phase2Juvenile.BrowseProbability(Phase2Species.Beech, 0.2f, 0.6f, 1f)), "Beech p = 0.2 × 0.5");
            foreach (var species in Phase2Species.Broadleaves)
            {
                T.Eq(1f, Phase2Juvenile.BrowseHeightVulnerability(species, 1.2f), "V(1.2) = 1");
                T.Eq(1f, Phase2Juvenile.BrowseHeightVulnerability(species, 0.3f), "V(0.3) = 1");
                Close(0.5, Phase2Juvenile.BrowseHeightVulnerability(species, 1.5f), "V(1.5) = 0.5", 1e-5);
                T.Eq(0f, Phase2Juvenile.BrowseHeightVulnerability(species, 1.8f), "V(1.8) = 0");
                T.Eq(0f, Phase2Juvenile.BrowseHeightVulnerability(species, 2.5f), "V(2.5) = 0");
            }
            var inputs = EmptySquares();
            var light = FixedLight(inputs, 2.5, (_, _) => 0.3f);
            var (s5, s2) = EmptyStates(inputs);
            int cell = CellIndex(s5, "MA-A", 1, 1);
            var withBands = WithBroadleafBands(s2, "MA-A", cell, BroadleafBand(Beech, 0.1f, 0.5f, 0), BroadleafBand(Oak, 0.1f, 0.5f, 0));
            var centre = s5.Areas[0].Cells[cell];
            var shelterOverCell = withBands with { Shelters = new[] { new BrowseShelter("S-JPJ0099", "PJ0099", centre.CentreXM, centre.CentreZM, 0, 8, -1) } };
            var a = Phase2Annual.Step(s5, withBands, inputs, 1, Array.Empty<Phase2DensityRecord>(), light);
            var b = Phase2Annual.Step(s5, shelterOverCell, inputs, 1, Array.Empty<Phase2DensityRecord>(), light);
            var bandsA = a.NextPhase2.Areas[0].Cells[cell].Bands;
            T.True(bandsA.SequenceEqual(b.NextPhase2.Areas[0].Cells[cell].Bands), "a shelter over the cell changes no cohort");
            T.Eq(F32.Bits(0.2f), F32.Bits(bandsA.Single(x => x.SpeciesId == Oak).LastBrowsedFraction), "cohort Oak browse at access 1");
            T.Eq(F32.Bits(0.1f), F32.Bits(bandsA.Single(x => x.SpeciesId == Beech).LastBrowsedFraction), "cohort Beech browse at access 1");

            // Pressure 0: every band and planted juvenile takes the exact light-only expressions.
            var zero = new Stage5Options { BackgroundBrowsePressure = 0f };
            var (z5, z2) = EmptyStates(inputs, zero);
            z5 = WithCell(z5, "MA-A", cell, c => c with { Bands = new[] { Band(0.1f, 0.5f, 0) } });
            z2 = WithBroadleafBands(z2, "MA-A", cell, BroadleafBand(Beech, 0.1f, 0.5f, 0), BroadleafBand(Oak, 0.1f, 0.5f, 0)) with
            {
                Juveniles = new[] { Juvenile("PJ0001", "MA-B", Oak, 13.1f, 2.1f, 3f, 0.6f), Juvenile("PJ0002", "MA-B", Beech, 16.1f, 7.1f, 3f, 0.6f) },
            };
            var step = Phase2Annual.Step(z5, z2, inputs, 1, Array.Empty<Phase2DensityRecord>(), light);
            var coverA = step.NextStage5.Areas[0].Cells[cell];
            float pveg = Stage5Undergrowth.VegetationLossProbability(coverA.Bramble, coverA.Bracken, 0.5f);
            foreach (var band in step.NextPhase2.Areas[0].Cells[cell].Bands)
            {
                var species = Phase2Species.BroadleafOrNull(band.SpeciesId)!;
                T.Eq(0f, band.LastBrowsedFraction, "pressure 0: no browse");
                T.Eq(F32.Bits(0.5f + 0.35f * Phase2Juvenile.LightResponse(species, 0.3f) * 1f), F32.Bits(band.Height), "light-only height " + band.SpeciesId);
                T.Eq(F32.Bits((float)(0.1f * (Phase2Juvenile.LightSurvival(species, 0.3f) * (1d - pveg)))), F32.Bits(band.Density),
                    "light-only survival " + band.SpeciesId);
            }
            var sitkaBand = step.NextStage5.Areas[0].Cells[cell].Bands.Single();
            T.Eq(F32.Bits(0.5f + 0.35f * Stage5Juvenile.LightResponse(0.3f) * 1f), F32.Bits(sitkaBand.Height), "Sitka light-only height");
            foreach (var j in step.NextPhase2.Juveniles.Where(j => j.Alive))
            {
                T.True(j.LastBrowseProbability == 0f && !j.LastYearBrowsed, "pressure 0: planted juvenile never browsed");
                T.Eq(F32.Bits(0.6f + 0.35f * Phase2Juvenile.LightResponse(Phase2Species.BroadleafOrNull(j.SpeciesId)!, 0.3f) * 1f), F32.Bits(j.HeightM),
                    "planted light-only height " + j.SpeciesId);
            }
        });
    }

    // ============================================================================================ P2-06 spot clearance
    private static void SpotClearance()
    {
        T.Add("P2-06 S-01: 1 m² circular spot clearance vs unweeded vs whole-cell ClearUndergrowth (all selective; INTENTIONAL DIVERGENCE from Unity's destructive QueryClearance and planting auto-clearance): the circle zeroes cover only inside it, recovers at 0.30/0.45 toward its cell's target, changes the planted stem's p_veg, and leaves every natural cohort unchanged", () =>
        {
            var runs = new[] { Phase2Session(), Phase2Session(), Phase2Session() }; // spot, unweeded, whole-cell
            foreach (var r in runs) for (int y = 0; y < 3; y++) Advance(r);
            var s5 = Stage5Of(runs[0]);
            var locate = Phase2Annual.Locator(s5);
            var p = FindPoint(runs[0], "MA-B", 1.0f, (x, z) => locate(x, z) is var (a, c) && s5.Areas[a].Cells[c].Bands.Count > 0 &&
                s5.Areas[a].AreaId == "MA-B");
            var (pa, pc) = locate(p.X, p.Z)!.Value;
            var pCell = s5.Areas[pa].Cells[pc];
            string Bands(MultiAreaContinuation c)
            {
                var w = new Model0FingerprintTests.Fingerprint();
                foreach (var area in Stage5Of(c).Areas) foreach (var cell in area.Cells) foreach (var b in cell.Bands)
                    w.Text(string.Join("|", area.AreaId, cell.Column, cell.Row, F32.Bits(b.Density), F32.Bits(b.Height), b.EstablishYear));
                foreach (var area in Phase2Of(c).Areas) foreach (var cell in area.Cells) foreach (var b in cell.Bands)
                    w.Text(string.Join("|", area.AreaId, cell.Column, cell.Row, b.SpeciesId, F32.Bits(b.Density), F32.Bits(b.Height)));
                return w.Hash();
            }
            string bandsBefore = Bands(runs[0]);
            T.True(runs.All(r => Bands(r) == bandsBefore), "three identical sessions before work");
            runs[0].AddPlant(Fence(runs[0].ReadView()), "MA-B", Oak, p.X, p.Z);
            runs[0].AddSpotClear(Fence(runs[0].ReadView()), "MA-B", p.X, p.Z);
            runs[1].AddPlant(Fence(runs[1].ReadView()), "MA-B", Oak, p.X, p.Z);
            runs[2].AddPlant(Fence(runs[2].ReadView()), "MA-B", Oak, p.X, p.Z);
            runs[2].AddClearUndergrowth(Fence(runs[2].ReadView()), "MA-B", pCell.Column, pCell.Row);
            foreach (var r in runs) ApproveAndExecute(r);
            T.True(runs.All(r => Bands(r) == bandsBefore), "Execute changes no natural cohort in any of the three runs (selective)");
            var patch = Phase2Of(runs[0]).Patches.Single();
            T.Eq(F32.Bits((float)Math.Sqrt(1f / MathF.PI)), F32.Bits(patch.RadiusM), "radius Mathf.Sqrt(1f / Mathf.PI) as Unity");
            Close(0.5641896, patch.RadiusM, "radius 0.5641896 m", 1e-6);
            Close(1.0, Math.PI * patch.RadiusM * patch.RadiusM, "area 1.0 m²", 1e-6);
            var cellNow = Stage5Of(runs[0]).Areas[pa].Cells[pc];
            T.True(cellNow.Bramble > 0f && cellNow.Bracken > 0f, "the cell's own cover is untouched by the spot patch");
            T.Eq(0f, Phase2Annual.ExposureAt(Phase2Of(runs[0]).Patches, p.X, p.Z, (cellNow.Bramble, cellNow.Bracken)), "zero cover inside the circle");
            float edge = patch.RadiusM + 0.02f;
            T.Eq(MathF.Max(cellNow.Bramble, cellNow.Bracken), Phase2Annual.ExposureAt(Phase2Of(runs[0]).Patches, p.X + edge * 0.6f, p.Z + edge * 0.8f,
                (cellNow.Bramble, cellNow.Bracken)), "cell cover just outside the circle");
            T.Eq(0f, Stage5Of(runs[2]).Areas[pa].Cells[pc].Bramble, "whole-cell ClearUndergrowth zeroes the cell");
            var previousPatch = patch;
            var previous = runs.Select(r => Phase2Of(r).Juveniles.Single()).ToArray();
            for (int k = 1; k <= 6; k++)
            {
                foreach (var r in runs) Advance(r);
                T.Eq(WriteHash(runs[0]), WriteHash(runs[1]), "spot clearance never changes any natural cohort, cover or light (year " + k + ")");
                var cell = Stage5Of(runs[0]).Areas[pa].Cells[pc];
                var nowPatch = Phase2Of(runs[0]).Patches.Single();
                if (k == 1)
                    T.True(nowPatch.Bramble == 0f && nowPatch.Bracken == 0f && nowPatch.UpdatedYear == 4, "the first annual step leaves the new patch at 0 (Unity guard)");
                else
                {
                    double t = 0.8 * (0.1 + 0.9 * Math.Clamp(cell.Light, 0f, 1f));
                    double Rec(double cover) => Math.Clamp(cover + (t - cover) * (t > cover ? 0.30 : 0.45), 0, 1);
                    Close(Rec(previousPatch.Bramble), nowPatch.Bramble, "patch bramble recovers toward its cell's T", 1e-5);
                    Close(Rec(previousPatch.Bracken), nowPatch.Bracken, "patch bracken recovers toward its cell's T", 1e-5);
                }
                var now = runs.Select(r => Phase2Of(r).Juveniles.Single()).ToArray();
                double Pveg(float exposure, float h) => 0.35 * Math.Clamp(exposure, 0, 1) * Math.Clamp((1.5 - h) / 1.2, 0, 1);
                if (previous[0].IsActive)
                    Close(Pveg(MathF.Max(nowPatch.Bramble, nowPatch.Bracken), previous[0].HeightM), now[0].LastVegetationLossProbability, "spot p_veg", 1e-5);
                if (previous[1].IsActive)
                    Close(Pveg(MathF.Max(cell.Bramble, cell.Bracken), previous[1].HeightM), now[1].LastVegetationLossProbability, "unweeded p_veg", 1e-5);
                if (k == 1 && previous[0].IsActive && previous[1].IsActive)
                    T.True(now[0].LastVegetationLossProbability == 0f && now[1].LastVegetationLossProbability > 0f, "year 1: the patch removes the p_veg the cell imposes");
                previousPatch = nowPatch;
                previous = now;
            }
            Console.WriteLine($"  P2-06 S-01 patch after 6 y: bramble {previousPatch.Bramble:F3}; planted Oak alive: spot {previous[0].Alive}, unweeded {previous[1].Alive}, whole-cell {previous[2].Alive}");

            static string WriteHash(MultiAreaContinuation c)
            {
                var w = new Model0FingerprintTests.Fingerprint();
                WriteState(w, Stage5Of(c));
                foreach (var area in Phase2Of(c).Areas) foreach (var cell in area.Cells) foreach (var b in cell.Bands)
                    w.Text(string.Join("|", cell.Column, cell.Row, b.SpeciesId, F32.Bits(b.Density), F32.Bits(b.Height)));
                return w.Hash();
            }
        });
    }

    // ============================================================================================ P2-07 planted promotion
    private static void PlantedPromotion()
    {
        T.Add("P2-07 P-01: a planted Oak reaching 3.5 m promotes to PL-{juvenileId} at its exact X/Z in the same block and year as a natural promotion (block cap bypassed, natural block credit unchanged); initial radius is the C2 target; Age = max(1, round(age)); DBH = clamp(1.5·H, 2, 20)", () =>
        {
            var baseInputs = EmptySquares();
            var neighbour = new AnnualTree("MA-A-001", 1.0f, 8.5f, 30, 30f, 20f, 3f);
            var inputs = baseInputs.Select(a => a.AreaId == "MA-A" ? a with { Trees = new[] { neighbour } } : a).ToArray();
            var light = FixedLight(inputs, 2.5, (_, _) => 1f);
            var (s5, s2) = EmptyStates(inputs);
            int natural = CellIndex(s5, "MA-A", 0, 0);
            s5 = WithCell(s5, "MA-A", natural, c => c with { Bands = new[] { Band(0.3f, 3.6f, 0) } });
            var withPj = s2 with { Juveniles = new[] { Juvenile("PJ0001", "MA-A", Oak, 3.75f, 3.75f, 5f, 3.4f) } };
            var step = Phase2Annual.Step(s5, withPj, inputs, 1, Array.Empty<Phase2DensityRecord>(), light);
            var twin = Phase2Annual.Step(s5, s2, inputs, 1, Array.Empty<Phase2DensityRecord>(), light);
            var promoted = step.PromotedTrees["MA-A"];
            var naturalTree = promoted.Single(t => t.TreeId == "R1-MA-A-0-0");
            var planted = promoted.Single(t => t.TreeId == "PL-PJ0001");
            T.Eq(Stage5Promotion.BlockOf(0, 0, 2.5), Stage5Promotion.BlockOf((int)Math.Floor(planted.XM / 2.5), (int)Math.Floor(planted.ZM / 2.5), 2.5),
                "the planted recruit is in the natural promotion's block");
            T.True(planted.XM == 3.75f && planted.ZM == 3.75f, "PL-* at the exact X/Z");
            T.True(step.NextStage5.Blocks.SequenceEqual(twin.NextStage5.Blocks), "natural block credit unchanged by the planted promotion");
            T.Eq(1, step.Stage5Account.Areas[0].Promotions.Count, "one natural promotion in the block");
            float height = 3.4f + 0.35f * 1f * 1f;
            T.Eq(F32.Bits(height), F32.Bits(planted.HeightM), "height after this year's growth");
            T.Eq(Math.Max(1, (int)Math.Round(6f, MidpointRounding.ToEven)), planted.AgeYears, "Age = max(1, round(age))");
            T.Eq(F32.Bits(Math.Clamp(1.5f * height, 2f, 20f)), F32.Bits(planted.DbhCm), "DBH = clamp(1.5·H, 2, 20)");
            float ci = CrownModel.WoodlandCompetition(planted, CrownModel.Woodland(inputs.SelectMany(a => a.Trees))).Ci;
            T.True(ci > 0f, "a neighbour makes the CI positive");
            T.Eq(F32.Bits(Phase2Crown.TargetRadiusM(Phase2Species.Oak, planted.DbhCm, ci)), F32.Bits(planted.CrownRadiusM), "C2 initial radius");
            T.True(planted.CrownRadiusM < Phase2Species.Oak.PotentialCrownRadiusM(planted.DbhCm), "not the open-grown r_pot");
            T.Eq(Oak, step.NextPhase2.TreeSpecies["PL-PJ0001"], "the recruit is recorded as Oak");
            T.Eq("PL-PJ0001", step.NextPhase2.Juveniles.Single().PromotedTreeId, "the juvenile is marked promoted");
            T.Eq(1, step.Phase2Account.Planted.Single(a => a.SpeciesId == Oak).Promoted, "planted account");
            T.True(naturalTree.XM >= 0 && naturalTree.XM < 2.5 && naturalTree.ZM < 2.5, "the natural recruit stays in its own cell");
        });

        T.Add("P2-07 P-02: a planted Oak at H ≥ 3.5 m under cell light < 0.2 does not promote and at ≥ 0.2 does; Beech has no light gate; promotion does not recompute the same year's canopy", () =>
        {
            var inputs = EmptySquares();
            var (s5, s2) = EmptyStates(inputs);
            PlantedJuvenile RunOne(string species, float cellLight)
            {
                var state = s2 with { Juveniles = new[] { Juvenile("PJ0007", "MA-A", species, 6.25f, 6.25f, 9f, 3.6f) } };
                var step = Phase2Annual.Step(s5, state, inputs, 1, Array.Empty<Phase2DensityRecord>(), FixedLight(inputs, 2.5, (_, _) => cellLight));
                return step.NextPhase2.Juveniles.Single();
            }
            var dim = RunOne(Oak, 0.19f);
            T.True(dim.Alive && dim.PromotedTreeId is null && dim.HeightM >= 3.5f, "Oak at light 0.19: alive, tall enough, NOT promoted (gate 0.2)");
            T.Eq("PL-PJ0007", RunOne(Oak, 0.2f).PromotedTreeId, "Oak at light 0.2 promotes");
            T.Eq("PL-PJ0007", RunOne(Beech, 0.05f).PromotedTreeId, "Beech has no light gate");
            // Same-year canopy: the cell light recorded for the promotion year is the light without the new adult.
            var state2 = s2 with { Juveniles = new[] { Juvenile("PJ0007", "MA-A", Beech, 6.25f, 6.25f, 9f, 3.6f) } };
            var modelStep = Phase2Annual.Step(s5, state2, inputs, 1, Array.Empty<Phase2DensityRecord>());
            T.Eq("PL-PJ0007", modelStep.NextPhase2.Juveniles.Single().PromotedTreeId, "promoted under model light");
            T.True(modelStep.NextStage5.Areas.SelectMany(a => a.Cells).All(cell => cell.Light == 1f), "this year's light is the canopy without the new adult");
        });
    }

    // ============================================================================================ P2-08 seed rain and mast
    private static void SeedRainAndMast()
    {
        T.Add("P2-08 R-01: mature Oak in rack A seeds rack B and mature Beech in rack B seeds rack A within λ/cutoff (Oak 4/80 m, Beech 6/50 m); outer grassland adds nothing; reversed rack order gives identical results", () =>
        {
            var (experiment, options) = TwoRacks("mad-p2-seed", 60, 20,
                new[] { (58_000, 10_000, 60, 400, 25_000, Oak) }, new[] { (62_000, 10_000, 60, 400, 25_000, Beech) });
            var c = Phase2Session(experiment, null, options);
            var start = Root(c);
            var view0 = c.ReadView();
            Advance(c);
            var p2 = Phase2Of(c);
            var trees = c.ReadView().Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
            var mast = p2.Accounts[0].Mast.ToDictionary(m => m.SpeciesId, m => (double)m.Multiplier);
            int beyondCutoff = 0;
            foreach (var species in Phase2Species.Broadleaves)
                foreach (var area in Stage5Of(c).Areas)
                {
                    double oracle = 0;
                    foreach (var cell in area.Cells)
                    {
                        double s = 0;
                        foreach (var t in trees.Where(t => p2.SpeciesIdOf(t.TreeId) == species.SpeciesId))
                        {
                            double d = Math.Sqrt((cell.CentreXM - t.XM) * (double)(cell.CentreXM - t.XM) + (cell.CentreZM - t.ZM) * (double)(cell.CentreZM - t.ZM));
                            if (d > species.SeedDispersalCutoffM) { beyondCutoff++; continue; }
                            double m = t.AgeYears >= 60 ? 1 : t.AgeYears <= 40 ? 0 : (t.AgeYears - 40) / 20.0;
                            s += m * Math.Clamp(t.CrownRadiusM / 4.0, 0, 1) * mast[species.SpeciesId] * Math.Exp(-d / species.SeedDispersalScaleM);
                        }
                        if (s > 0) oracle += s;
                    }
                    double actual = p2.Accounts[0].Natural.Single(a => a.AreaId == area.AreaId && a.SpeciesId == species.SpeciesId).SeedArrival;
                    Close(oracle, actual, species.SpeciesId + " seed arrival in " + area.AreaId, 1e-5);
                    if ((species == Phase2Species.Oak) == (area.AreaId == "MA-B"))
                        T.True(actual > 0, species.SpeciesId + " seeds across the rack boundary into " + area.AreaId);
                }
            T.True(beyondCutoff > 0, "some cells lie beyond the 50 m Beech cutoff and receive none of it");
            T.Eq(0.0, Stage5Of(c).Accounts[0].Areas.Sum(a => a.SeedArrival), "no Sitka source, so no Sitka seed: the grassland adds nothing");
            var inputs = Inputs(view0);
            var forward = Phase2Annual.Step(start.Stage5!, start.Phase2!, inputs, 1, Array.Empty<Phase2DensityRecord>());
            var reversed = Phase2Annual.Step(start.Stage5!, start.Phase2!, inputs.Reverse().ToArray(), 1, Array.Empty<Phase2DensityRecord>());
            T.Eq(StepHash(forward), StepHash(reversed), "reversed rack order gives an identical year");
        });

        T.Add("P2-08 R-02: Oak, Beech and Sitka mast sequences are independent: changing one species' stream seed changes no other species' multipliers and no ADULT-MORT roll", () =>
        {
            (List<Phase2MastRecord[]> Broadleaf, List<string> Sitka, string Adults) Run(Phase2Options options)
            {
                var c = MixedSession(null, options);
                var w = new Model0FingerprintTests.Fingerprint();
                for (int y = 0; y < 9; y++) foreach (var area in Advance(c).Areas) if (area.Report is { } r) w.Report(r);
                return (Phase2Of(c).Accounts.Select(a => a.Mast.ToArray()).ToList(),
                    Stage5Of(c).Accounts.Select(a => a.MastRoll.ToString("R") + a.MastLabel + a.MastMultiplier.ToString("R")).ToList(), w.Hash());
            }
            var baseRun = Run(Phase2Options.Default with { OakMastSeed = 11, BeechMastSeed = 21 });
            var oakChanged = Run(Phase2Options.Default with { OakMastSeed = 12, BeechMastSeed = 21 });
            var beechChanged = Run(Phase2Options.Default with { OakMastSeed = 11, BeechMastSeed = 22 });
            Phase2MastRecord Of(Phase2MastRecord[] year, string id) => year.Single(m => m.SpeciesId == id);
            T.True(baseRun.Broadleaf.Zip(oakChanged.Broadleaf).All(p => Of(p.First, Beech) == Of(p.Second, Beech)), "Oak seed changes no Beech mast");
            T.True(baseRun.Broadleaf.Zip(oakChanged.Broadleaf).Any(p => Of(p.First, Oak).Roll != Of(p.Second, Oak).Roll), "Oak seed changes Oak mast");
            T.True(baseRun.Broadleaf.Zip(beechChanged.Broadleaf).All(p => Of(p.First, Oak) == Of(p.Second, Oak)), "Beech seed changes no Oak mast");
            T.True(baseRun.Sitka.SequenceEqual(oakChanged.Sitka) && baseRun.Sitka.SequenceEqual(beechChanged.Sitka), "STAGE5-MAST unchanged");
            T.Eq(baseRun.Adults, oakChanged.Adults, "every adult report, ADULT-MORT roll and death unchanged (Oak seed)");
            T.Eq(baseRun.Adults, beechChanged.Adults, "every adult report, ADULT-MORT roll and death unchanged (Beech seed)");
            T.True(baseRun.Broadleaf.SelectMany(y => y).Select(m => m.Label).Distinct().Count() >= 2, "the mast states vary across years");
        });

        T.Add("P2-08 R-03: a rack with 0 living adults advances, receives seed, updates undergrowth and grows and promotes cohorts", () =>
        {
            var (experiment, options) = TwoRacks("mad-p2-empty", 20, 20,
                new[] { (18_000, 10_000, 60, 400, 25_000, Oak), (10_000, 4_000, 40, 300, 20_000, Sitka), (3_000, 15_000, 60, 380, 24_000, Beech) },
                new[] { (30_000, 10_000, 5, 40, 2_000, Sitka) });
            var c = Phase2Session(experiment, new Stage5Options { MastSeed = 3 }, options);
            var root = Root(c);
            RootField.SetValue(c, root with
            {
                Areas = Array.AsReadOnly(root.Areas.Select(a => a.AreaId != "MA-B" ? a : a with
                {
                    State = a.State with
                    {
                        Trees = Array.AsReadOnly(a.State.Trees.Select(t => t with { Death = new AnnualDeath(t.TreeId, 0, "test-injected", "test") }).ToArray()),
                    },
                }).ToArray()),
            });
            var genesisCovers = Stage5Of(c).Areas[1].Cells.Select(cell => cell.Bramble).ToArray();
            int promotedYear = -1;
            for (int y = 1; y <= 25 && promotedYear < 0; y++)
            {
                var result = Advance(c);
                T.Eq(y, c.ReadView().Source.EcologicalYear, "the shared clock advances");
                T.True(result.Areas.Single(a => a.AreaId == "MA-B").Report is null, "rack B runs no adult step while it has no living adult");
                var natural = Phase2Of(c).Accounts[^1].Natural.Where(a => a.AreaId == "MA-B").ToArray();
                T.True(natural.Single(a => a.SpeciesId == Oak).SeedArrival > 0, "rack B receives Oak seed from rack A");
                if (y == 1)
                    T.True(!Stage5Of(c).Areas[1].Cells.Select(cell => cell.Bramble).SequenceEqual(genesisCovers), "rack B's undergrowth is stepped");
                if (y >= 3) T.True(natural.Sum(a => a.StartingAbundance) > 0, "rack B's broadleaf bands grow and survive");
                if (natural.Any(a => a.Promotions.Count > 0) || Stage5Of(c).Accounts[^1].Areas.Single(a => a.AreaId == "MA-B").Promotions.Count > 0)
                    promotedYear = y;
            }
            T.True(promotedYear > 0, "a cohort promotes in the adult-empty rack");
            Console.WriteLine("  P2-08 R-03 first promotion in the adult-empty rack in year " + promotedYear);
        });
    }

    private static string StepHash(Phase2AnnualResult r)
    {
        var w = new Model0FingerprintTests.Fingerprint();
        WriteState(w, r.NextStage5);
        WritePhase2(w, r.NextPhase2);
        foreach (var (area, trees) in r.PromotedTrees.OrderBy(p => p.Key, StringComparer.Ordinal)) { w.Text(area); w.Trees(trees); }
        return w.Hash();
    }

    // ============================================================================================ P2-09 shared capacity and block cap
    private static void CapacityAndBlockCap()
    {
        T.Add("P2-09 K-01: a cell seeded by all three species never exceeds combined relative occupancy 1 (capacity 1.5·a); the shared cap binds", () =>
        {
            // Four mature seed trees of each species inside the two racks; light 0.35 everywhere (survival 1 for all three).
            // One cell starts with 2 m bands of all three species at combined occupancy 0.96 (above browse and bramble reach),
            // so this year's seed rain must be capped by the shared capacity.
            var baseInputs = EmptySquares();
            var species = new Dictionary<string, string>(StringComparer.Ordinal);
            var adults = new List<AnnualTree>();
            foreach (var (id, x, z) in new[] { (Sitka, 1.2f, 1.2f), (Oak, 3.8f, 8.8f), (Beech, 6.2f, 1.2f), (Sitka, 8.8f, 8.8f), (Oak, 11.2f, 1.2f),
                (Beech, 13.8f, 8.8f), (Sitka, 16.2f, 1.2f), (Oak, 18.8f, 8.8f), (Beech, 1.2f, 8.8f), (Sitka, 13.8f, 1.2f), (Oak, 8.8f, 1.2f), (Beech, 18.8f, 1.2f) })
            {
                string treeId = (x < 10f ? "MA-A-" : "MA-B-") + (adults.Count + 1).ToString("000");
                adults.Add(new AnnualTree(treeId, x, z, id == Sitka ? 40 : 60, 40f, 25f, 4f));
                if (id != Sitka) species[treeId] = id;
            }
            var inputs = baseInputs.Select(a => a with { Trees = adults.Where(t => t.TreeId.StartsWith(a.AreaId, StringComparison.Ordinal)).ToArray() }).ToArray();
            var light = FixedLight(inputs, 2.5, (_, _) => 0.35f);
            var (s5, s2) = EmptyStates(inputs, null, species);
            int full = CellIndex(s5, "MA-A", 1, 1);
            s5 = WithCell(s5, "MA-A", full, cell => cell with { Bands = new[] { Band(0.12f, 2f, 0) } });
            s2 = WithBroadleafBands(s2, "MA-A", full, BroadleafBand(Beech, 0.12f, 2f, 0), BroadleafBand(Oak, 0.12f, 2f, 0));
            double capacity = 1.5 * 0.25, maxOccupancy = 0, rejected = 0;
            bool allThree = false;
            var promotions = new List<(int Year, int X, int Z, string Species)>();
            for (int year = 1; year <= 14; year++)
            {
                var step = Phase2Annual.Step(s5, s2, inputs, year, Array.Empty<Phase2DensityRecord>(), light);
                (s5, s2) = (step.NextStage5, step.NextPhase2);
                for (int i = 0; i < s5.Areas.Count; i++)
                    for (int c = 0; c < s5.Areas[i].Cells.Count; c++)
                    {
                        double occupancy = CellOccupancy(s5.Areas[i].Cells[c], s2.Areas[i].Cells[c], capacity);
                        T.True(occupancy <= 1 + 1e-5, $"cell occupancy {occupancy:R} ≤ 1");
                        maxOccupancy = Math.Max(maxOccupancy, occupancy);
                        allThree |= s5.Areas[i].Cells[c].Bands.Count > 0 && s2.Areas[i].Cells[c].Bands.Select(b => b.SpeciesId).Distinct().Count() == 2;
                    }
                rejected += step.Stage5Account.Areas.Sum(a => a.CapacityRejected) + step.Phase2Account.Natural.Sum(a => a.CapacityRejected);
                promotions.AddRange(step.Stage5Account.Areas.SelectMany(a => a.Promotions).Select(p => (year, p.BlockX, p.BlockZ, Sitka)));
                promotions.AddRange(step.Phase2Account.Natural.SelectMany(a => a.Promotions.Select(p => (year, p.BlockX, p.BlockZ, a.SpeciesId))));
            }
            T.True(allThree, "some cell holds Sitka, Oak and Beech together");
            T.True(maxOccupancy > 0.99 && rejected > 0, $"the shared capacity binds (max occupancy {maxOccupancy:F4}, rejected {rejected:F3})");
            T.True(promotions.GroupBy(p => (p.Year, p.X, p.Z)).All(g => g.Count() <= 1), "at most one natural promotion per block-year across species");
            T.True(promotions.Select(p => p.Species).Distinct().Count() >= 2, "promotions of more than one species compete for the blocks");
            Console.WriteLine($"  P2-09 K-01 max occupancy {maxOccupancy:F4}, rejected {rejected:F3}, promotions " +
                string.Join(", ", promotions.GroupBy(p => p.Species).Select(g => g.Key + " " + g.Count())));
        });

        T.Add("P2-09 K-02: on the 30-year MA-1-MIXED run at most one natural promotion per aligned 5 m block per year across all species, and each block within its Stage 5 partial edge credit", () =>
        {
            var run = Cached.Value;
            var promotions = Stage5Of(run.Session).Accounts.SelectMany(y => y.Areas.SelectMany(a => a.Promotions.Select(p => (y.YearAfter, p.BlockX, p.BlockZ))))
                .Concat(run.Phase2.Accounts.SelectMany(y => y.Natural.SelectMany(a => a.Promotions.Select(p => (y.YearAfter, p.BlockX, p.BlockZ))))).ToArray();
            T.True(promotions.GroupBy(p => p).All(g => g.Count() <= 1), "≤ 1 per block-year");
            int years = Stage5Of(run.Session).Accounts.Count;
            var blocks = Stage5Of(run.Session).Blocks;
            T.True(blocks.Any(b => b.InPolygonCells < 4), "the fixture has partial edge blocks");
            foreach (var b in blocks)
                T.True(promotions.Count(p => p.BlockX == b.BlockX && p.BlockZ == b.BlockZ) <= b.InPolygonCells * years / 4,
                    $"block ({b.BlockX},{b.BlockZ}) with {b.InPolygonCells} cells stays within its credit");
            Console.WriteLine($"  P2-09 K-02 MA-1-MIXED natural promotions {promotions.Length} over {years} years");
        });

        T.Add("P2-09 K-03: constructed ties resolve oldest EstablishYear → tallest → (Column, Row) → ordinal SpeciesId", () =>
        {
            Phase2Annual.Candidate C(int bx, int ey, float h, int col, int row, string species) => new(0, 0, 0, species, col, row, bx, 0, ey, h);
            var winners = Phase2Annual.Winners(new[]
            {
                C(0, 3, 5f, 0, 0, Oak), C(0, 2, 4f, 1, 1, Beech),                       // oldest wins over taller
                C(1, 2, 4f, 2, 0, Oak), C(1, 2, 4.5f, 3, 1, Sitka),                     // then tallest
                C(2, 2, 4f, 5, 0, Sitka), C(2, 2, 4f, 4, 1, Oak), C(2, 2, 4f, 4, 0, Beech), // then Column, then Row
                C(3, 2, 4f, 6, 0, Sitka), C(3, 2, 4f, 6, 0, Oak), C(3, 2, 4f, 6, 0, Beech), // then ordinal SpeciesId
            });
            T.Eq("2/4/1/1/beech", Key(winners[0]), "oldest EstablishYear");
            T.Eq("2/4.5/3/1/sitka-spruce", Key(winners[1]), "tallest");
            T.Eq("2/4/4/0/beech", Key(winners[2]), "lowest Column, then Row");
            T.Eq("2/4/6/0/beech", Key(winners[3]), "ordinal SpeciesId");
            static string Key(Phase2Annual.Candidate c) => string.Join("/", c.EstablishYear, c.Height.ToString(System.Globalization.CultureInfo.InvariantCulture), c.Column, c.Row, c.SpeciesId);

            // The same tie in one real cell: Beech and Oak bands of equal year and height; Beech promotes, Oak waits.
            var inputs = EmptySquares();
            var (s5, s2) = EmptyStates(inputs);
            int cell = CellIndex(s5, "MA-A", 0, 0);
            s2 = WithBroadleafBands(s2, "MA-A", cell, BroadleafBand(Beech, 0.1f, 3.6f, 0), BroadleafBand(Oak, 0.1f, 3.6f, 0));
            // Oak grows faster than Beech at light 0.4 (LR 0.8 vs 0.70), so equalise the growth by light 1 (both LR 1).
            var step = Phase2Annual.Step(s5, s2, inputs, 1, Array.Empty<Phase2DensityRecord>(), FixedLight(inputs, 2.5, (_, _) => 1f));
            T.Eq(Beech, step.NextPhase2.TreeSpecies["R1-MA-A-0-0"], "the ordinal SpeciesId breaks a full tie");
            T.True(step.NextPhase2.Areas[0].Cells[cell].Bands.Single().SpeciesId == Oak, "the Oak band waits");
        });

        T.Add("P2-09 K-04: cohort export removes only same-species, same-year sibling bands", () =>
        {
            var inputs = EmptySquares();
            var (s5, s2) = EmptyStates(inputs);
            int winner = CellIndex(s5, "MA-A", 0, 0), sibling = CellIndex(s5, "MA-A", 1, 0), sitkaCell = CellIndex(s5, "MA-A", 0, 1);
            s2 = WithBroadleafBands(s2, "MA-A", winner, BroadleafBand(Oak, 0.2f, 3.6f, 0));
            s2 = WithBroadleafBands(s2, "MA-A", sibling, BroadleafBand(Beech, 0.1f, 2f, 0), BroadleafBand(Oak, 0.1f, 2f, 0), BroadleafBand(Oak, 0.1f, 1f, 1));
            s5 = WithCell(s5, "MA-A", sitkaCell, c => c with { Bands = new[] { Band(0.1f, 2f, 0) } });
            var step = Phase2Annual.Step(s5, s2, inputs, 2, Array.Empty<Phase2DensityRecord>(), FixedLight(inputs, 2.5, (_, _) => 1f));
            T.Eq(Oak, step.NextPhase2.TreeSpecies["R2-MA-A-0-0"], "the Oak cohort promotes");
            var left = step.NextPhase2.Areas[0].Cells[sibling].Bands;
            T.True(!left.Any(b => b.SpeciesId == Oak && b.EstablishYear == 0), "the same-species, same-year sibling band left with it");
            T.True(left.Any(b => b.SpeciesId == Beech && b.EstablishYear == 0), "a same-year band of another species stays");
            T.True(left.Any(b => b.SpeciesId == Oak && b.EstablishYear == 1), "a same-species band of another year stays");
            T.Eq(1, step.NextStage5.Areas[0].Cells[sitkaCell].Bands.Count, "the Sitka same-year band stays");
            T.Eq(1, step.Phase2Account.Natural.Single(a => a.AreaId == "MA-A" && a.SpeciesId == Oak).CohortBandsExported, "one sibling exported");
        });
    }

    // ============================================================================================ P2-10 determinism
    private sealed record MixedRun(MultiAreaContinuation Session, MultiAreaPhase2State Phase2, string Hash);

    private static readonly Lazy<MixedRun> Cached = new(() => RunMixed());

    /// <summary>
    /// The MA-1-MIXED-v1 recipe: in year 0 three Oak and one Beech planted in Area A (two sheltered, one with a spot clear) and
    /// four Area A Sitka felled; in year 3 a scheduled lift on Area B's largest living Oak; 30 years. Every adult report, the
    /// final trees, the Stage 4 read-out and the whole Stage 5 and Phase 2 states are fingerprinted.
    /// </summary>
    private static MixedRun RunMixed(Action<MultiAreaContinuation, int>? beforeYear = null, Action<MultiAreaContinuation, int>? afterYear = null)
    {
        var c = MixedSession();
        var w = new Model0FingerprintTests.Fingerprint();
        var chosen = new List<(float X, float Z)>();
        bool Apart(float x, float z) => chosen.All(p => (p.X - x) * (p.X - x) + (p.Z - z) * (p.Z - z) > 9f);
        foreach (var species in new[] { Oak, Oak, Beech, Oak })
        {
            var p = FindPoint(c, "MA-A", 1.0f, Apart);
            chosen.Add(p);
            c.AddPlant(Fence(c.ReadView()), "MA-A", species, p.X, p.Z);
        }
        c.AddShelter(Fence(c.ReadView()), "MA-A", chosen[0].X, chosen[0].Z);
        c.AddShelter(Fence(c.ReadView()), "MA-A", chosen[2].X, chosen[2].Z);
        c.AddSpotClear(Fence(c.ReadView()), "MA-A", chosen[1].X, chosen[1].Z);
        var v = c.ReadView();
        foreach (var tree in v.Areas[0].Trees.Where(t => t.IsLiving && chosen.All(p => (p.X - t.XM) * (p.X - t.XM) + (p.Z - t.ZM) * (p.Z - t.ZM) > 4f)).Take(4))
            v = c.AddPending(Fence(v), "MA-A", tree.TreeId, "P2 recipe thinning").View;
        ApproveAndExecute(c);
        for (int year = 0; year < 30; year++)
        {
            if (year == 3)
            {
                var p2 = Phase2Of(c);
                var oak = c.ReadView().Areas[1].Trees.Where(t => t.IsLiving && p2.BroadleafOf(t.TreeId) == Phase2Species.Oak).OrderByDescending(t => t.DbhCm).First();
                c.AddScheduledPrune(Fence(c.ReadView()), "MA-B", oak.TreeId);
                ApproveAndExecute(c);
            }
            beforeYear?.Invoke(c, year);
            foreach (var area in Advance(c).Areas)
            {
                w.Text(area.AreaId);
                if (area.Report is { } report) w.Report(report); else w.Text("carried");
            }
            afterYear?.Invoke(c, year);
        }
        foreach (var area in c.ReadView().Areas) w.Trees(area.Trees);
        WriteStage4(w, c.ReadStage4View());
        WriteState(w, Stage5Of(c));
        WritePhase2(w, Phase2Of(c));
        return new MixedRun(c, Phase2Of(c), w.Hash());
    }

    private static void Determinism()
    {
        T.Add("P2-10 D-01: on MA-1-MIXED-v1 two 30-year two-rack runs give identical hashes", () =>
        {
            string first = Cached.Value.Hash;
            string second = RunMixed().Hash;
            Console.WriteLine("  P2-10 MA-1-MIXED-v1 30-year fingerprint " + first);
            T.Eq(first, second, "two runs identical");
            var p2 = Cached.Value.Phase2;
            Console.WriteLine($"  P2-10 MA-1-MIXED-v1 after 30 y: planted {string.Join(", ", p2.Juveniles.Select(j => j.JuvenileId + " " + j.SpeciesId + " " + (j.PromotedTreeId ?? (j.Alive ? "alive " + j.HeightM.ToString("F2") : "dead/" + j.DeathCause))))}; " +
                $"broadleaf adults living {Cached.Value.Session.ReadView().Areas.SelectMany(a => a.Trees).Count(t => t.IsLiving && p2.BroadleafOf(t.TreeId) is not null)}");
        });

        T.Add("P2-10 D-02: reversed rack processing order gives identical results every year (each rack's adult step and the regeneration step recomputed in reverse area order)", () =>
        {
            int years = 0;
            string? expectedTrees = null, expectedStates = null;
            RunMixed(beforeYear: (c, year) =>
            {
                var root = Root(c);
                var v = c.ReadView();
                var woodland = CrownModel.Woodland(root.Areas.SelectMany(a => a.State.Trees));
                var next = new Dictionary<string, AnnualState>(StringComparer.Ordinal);
                var density = new List<Phase2DensityRecord>();
                for (int i = root.Areas.Count - 1; i >= 0; i--)
                {
                    var area = root.Areas[i];
                    if (!area.State.Trees.Any(t => t.IsLiving)) { next[area.AreaId] = new AnnualState(year + 1, area.State.Trees); continue; }
                    var (_, stepped, record) = Phase2MixedDensity.StepRack(area.AreaId, area.State, v.Areas[i].SimulationSeed, v.Areas[i].AreaHa, woodland,
                        id => root.Stage4!.PruningOf(id).Lifts, Stage4Options.Default, root.Phase2!.BroadleafOf);
                    next[area.AreaId] = stepped;
                    density.Add(record);
                }
                var inputs = v.Areas.Reverse().Select(a => new Stage5AreaInput(a.AreaId, a.SimulationSeed, a.Polygon, a.AreaHa, next[a.AreaId].Trees)).ToArray();
                var regen = Phase2Annual.Step(root.Stage5!, root.Phase2!, inputs, year + 1, density);
                var w = new Model0FingerprintTests.Fingerprint();
                foreach (var a in v.Areas)
                    w.Trees(next[a.AreaId].Trees.Concat(regen.PromotedTrees[a.AreaId]).OrderBy(t => t.TreeId, StringComparer.Ordinal));
                expectedTrees = w.Hash();
                var s = new Model0FingerprintTests.Fingerprint();
                WriteState(s, regen.NextStage5);
                WritePhase2(s, regen.NextPhase2);
                expectedStates = s.Hash();
            }, afterYear: (c, year) =>
            {
                var w = new Model0FingerprintTests.Fingerprint();
                foreach (var a in c.ReadView().Areas) w.Trees(a.Trees);
                T.Eq(expectedTrees, w.Hash(), "reversed-order rosters, year " + (year + 1));
                var s = new Model0FingerprintTests.Fingerprint();
                WriteState(s, Stage5Of(c));
                WritePhase2(s, Phase2Of(c));
                T.Eq(expectedStates, s.Hash(), "reversed-order Stage 5 and Phase 2 states, year " + (year + 1));
                years++;
            });
            T.Eq(30, years, "every year compared");
        });

        T.Add("P2-10 D-03: replaying any Execute with the same key is a no-op; a forced pre-commit failure (Execute or Year+1) leaves the previous root intact and the retried year equals an uninterrupted one", () =>
        {
            var c = MixedSession();
            var twin = MixedSession();
            foreach (var s in new[] { c, twin })
            {
                var p = FindPoint(s, "MA-A", 1.0f);
                s.AddPlant(Fence(s.ReadView()), "MA-A", Beech, p.X, p.Z);
                s.AddShelter(Fence(s.ReadView()), "MA-A", p.X, p.Z);
            }
            var v = c.Approve(Fence(c.ReadView())).View;
            var before = Root(c);
            Throws<InvalidOperationException>(() => c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey,
                beforePublish: () => throw new InvalidOperationException("forced")));
            T.True(ReferenceEquals(before, Root(c)), "Execute: nothing published");
            c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            ApproveAndExecute(twin);
            var committed = Root(c);
            T.True(c.Execute(Fence(v), v.Approved.PlanId, v.Approved.ExecutionKey).Execution!.Replayed && ReferenceEquals(committed, Root(c)),
                "replay with the same key changes nothing");
            for (int y = 0; y < 4; y++) { Advance(c); Advance(twin); }
            var atYear = Root(c);
            Throws<InvalidOperationException>(() => c.Advance(Fence(c.ReadView()), false, beforeCommit: () => throw new InvalidOperationException("forced")));
            T.True(ReferenceEquals(atYear, Root(c)), "Year+1: nothing published");
            T.True(c.Execute(Fence(v), v.Approved.PlanId, v.Approved.ExecutionKey).Execution!.Replayed && ReferenceEquals(atYear, Root(c)),
                "a replay four years later is still a no-op");
            Advance(c);
            Advance(twin);
            string Hash(MultiAreaContinuation s)
            {
                var w = new Model0FingerprintTests.Fingerprint();
                foreach (var a in s.ReadView().Areas) w.Trees(a.Trees);
                WriteState(w, Stage5Of(s));
                WritePhase2(w, Phase2Of(s) with { WorkLog = Array.Empty<Phase2WorkRecord>() });
                return w.Hash();
            }
            T.Eq(Hash(twin), Hash(c), "the retried year equals the uninterrupted twin");
        });

        T.Add("P2-10 D-04: Phase 2 draws never change any ADULT-MORT-{treeId} roll (stateless keyed rolls; every recorded roll of the 30-year MA-1-MIXED run recomputes exactly)", () =>
        {
            var keys = Enumerable.Range(1, 40).Select(i => ("ADULT-MORT-MA-B-" + i.ToString("000"), i % 30 + 1)).ToArray();
            var first = keys.Select(k => SimulationRandom.Roll(DerivedManifest.RngModelVersion, k.Item1, k.Item2, 885_202)).ToArray();
            for (int i = 0; i < 5_000; i++)
            {
                Phase2Random.Roll(Phase2Random.GenericTreeKey("MA-B-" + i), i % 30, 885_202);
                Phase2Random.Roll(Phase2Random.JuvenileBrowseKey("PJ" + i), i % 30, 885_101);
                Phase2Mast.ForYear(Phase2Species.Oak, i, i % 30);
            }
            var again = keys.Select(k => SimulationRandom.Roll(DerivedManifest.RngModelVersion, k.Item1, k.Item2, 885_202)).ToArray();
            T.True(first.SequenceEqual(again), "ADULT-MORT rolls are unchanged by any number of Phase 2 draws");
            // Every ADULT-MORT roll recorded over 30 mixed years (with Phase 2 generic, mast and juvenile draws interleaved)
            // recomputes exactly from its key alone.
            int checkedRolls = 0, genericYears = 0;
            var verify = MixedSession();
            for (int year = 0; year < 30; year++)
                foreach (var area in Advance(verify).Areas.Where(a => a.Report is not null))
                {
                    if (area.Report!.Deaths.Any(d => d.Mechanism == Phase2MixedDensity.GenericMechanism)) genericYears++;
                    foreach (var row in area.Report.Trees.Where(r => r.MortalityRoll is not null))
                    {
                        float expected = SimulationRandom.Roll(DerivedManifest.RngModelVersion, "ADULT-MORT-" + row.Before.TreeId, year + 1,
                            verify.ReadView().Areas.Single(a => a.AreaId == area.AreaId).SimulationSeed);
                        T.Eq(F32.Bits(expected), F32.Bits(row.MortalityRoll!.Value), "recorded roll " + row.Before.TreeId);
                        checkedRolls++;
                    }
                }
            T.True(checkedRolls > 0 && genericYears > 0, $"rolls recomputed: {checkedRolls}; rack-years with generic draws: {genericYears}");
        });
    }
}

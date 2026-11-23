using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// MA-1 two-area manual forestry simulator (owner-approved MA-1 spec; #852 design adapted to the real code): experiment
/// identity and geometry (MA-F), per-area annual ecology (MA-E), the one cross-area Work Plan and its refusals (MA-W),
/// and the read-only competition inspection (MA-C). These are the author's feature tests, not independent qualification.
/// </summary>
internal static class MultiAreaTests
{
    private const string A = "MA-A";
    private const string B = "MA-B";
    // SHA-256 of the canonical manifest body of mad-sitka-ma1-two-area-v1 (generator v1). Any generator change moves it.
    private const string PinnedContentHash = "c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95";

    internal static void Register(string manifestPath)
    {
        string fixtureDirectory = Path.GetDirectoryName(manifestPath)!;

        // ---------------------------------------------------------------------------------------------- identity (MA-F)
        T.Add("multi MA-F01: the generator is deterministic and publishes canonical strict JSON with its pinned content hash", () =>
        {
            var first = MultiAreaGenerator.Generate();
            var bytes = first.ManifestFileBytes();
            T.True(bytes.AsSpan().SequenceEqual(MultiAreaGenerator.Generate().ManifestFileBytes()), "two generations are byte-identical");
            T.Eq(PinnedContentHash, first.ContentHash(), "pinned content hash");
            T.Eq(DerivedManifest.Sha256Hex(first.ManifestBodyBytes()), first.ContentHash(), "the hash is SHA-256 of the canonical body");
            var root = J.Keys(StrictJson.Parse(bytes), "manifest", "format", "schemaVersion", "experimentId", "domain", "sourceClass",
                "reviewStatus", "disclosure", "generator", "models", "localFrame", "initialEcologicalYear",
                "actionEligibleDbhThresholdMm", "areaHaExpression", "areas", "contentHash");
            string text = Encoding.UTF8.GetString(bytes);
            int cut = text.LastIndexOf(",\"contentHash\":\"", StringComparison.Ordinal);
            T.Eq(first.ContentHash(), DerivedManifest.Sha256Hex(Encoding.UTF8.GetBytes(text[..cut] + "}")),
                "a reviewer recomputes the hash from the published file alone");
            T.Eq(first.ContentHash(), J.Str(root["contentHash"], "contentHash"), "embedded hash");
            T.Eq("mad-sitka-ma1-two-area-v1", J.Str(root["experimentId"], "experimentId"), "versioned experiment identity");
            T.Eq(MultiAreaIdentity.Domain, J.Str(root["domain"], "domain"), "own domain string");
            T.Eq("SyntheticAuthored", J.Str(root["sourceClass"], "sourceClass"), "synthetic authorship");
            T.Eq("PendingIndependentReview", J.Str(root["reviewStatus"], "reviewStatus"), "the author never self-accepts");
            string disclosure = J.Str(root["disclosure"], "disclosure");
            foreach (var phrase in new[] { "NOT A SURVEYED IRISH FOREST", "AREAS NOT ASSESSED COMPARABLE", "SITE UNKNOWN",
                         "EDGE NEIGHBOURS EXCLUDED", "NO TREATMENT TRANSFER" })
                T.True(disclosure.Contains(phrase, StringComparison.Ordinal), "disclosure states " + phrase);
            var areas = J.Arr(root["areas"], "areas").Items;
            T.Eq(2, areas.Count, "two areas");
            foreach (var node in areas)
                T.Eq("UNKNOWN", J.Str(J.Keys(node, "area", "areaId", "label", "stocking", "site", "polygonMm", "twiceShoelaceMm2",
                    "areaM2Exact", "areaHa", "simulationSeed", "treeCount", "trees")["site"], "site"), "site is UNKNOWN in every area");
            Console.WriteLine($"MA-F01 manifest {bytes.Length} bytes, content hash {first.ContentHash()}");
        });

        T.Add("multi MA-F02: areas A and B differ in shape, size, stocking, age/DBH structure, competition and seed", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var (a, b) = (e.Areas[0], e.Areas[1]);
            T.True(a.AreaId == A && b.AreaId == B, "neutral ordinal area IDs");
            T.True(a.PolygonMm.Count == 5 && b.PolygonMm.Count == 4, "a pentagon and a quadrilateral");
            T.Eq("915", a.AreaM2Exact, "A exact area from the shoelace sum");
            T.Eq("1500", b.AreaM2Exact, "B exact area from the shoelace sum");
            T.Eq(F32.Bits((float)(1_830_000_000.0 / 2e10)), F32.Bits(a.AreaHa), "A hectares derived from geometry");
            T.Eq(F32.Bits((float)(3_000_000_000.0 / 2e10)), F32.Bits(b.AreaHa), "B hectares derived from geometry");
            T.True(a.Trees.Count == 171 && b.Trees.Count == 73, $"tree counts {a.Trees.Count} and {b.Trees.Count}");
            T.True(a.SimulationSeed != b.SimulationSeed && a.SimulationSeed != e.GeneratorSeed && b.SimulationSeed != e.GeneratorSeed,
                "distinct area seeds, both different from the generator seed");
            T.True(a.Trees.All(t => t.AgeYears == 24), "A is one 24-year cohort");
            T.Eq("29,47", string.Join(",", b.Trees.Select(t => t.AgeYears).Distinct().Order()), "B has two cohorts");
            var sa = WorkRules.Summarize(MultiAreaExperiment.GenesisTrees(a), a.AreaHa);
            var sb = WorkRules.Summarize(MultiAreaExperiment.GenesisTrees(b), b.AreaHa);
            T.True(sa.StemsPerHa > 3 * sb.StemsPerHa, "A has more than three times B's stocking");
            T.True(sa.QuadraticMeanDbhCm < sb.QuadraticMeanDbhCm, "A has smaller stems");
            T.True(sa.RelativeDensity > SitkaGrowthModel.MortalityOnsetRelativeDensity &&
                sb.RelativeDensity < SitkaGrowthModel.MortalityOnsetRelativeDensity, "A starts above the density-mortality onset, B below");
            var signatures = a.Trees.Select(t => (t.XMm, t.YMm, t.DbhMm, t.HeightMm)).ToHashSet();
            T.True(!b.Trees.Any(t => signatures.Contains((t.XMm, t.YMm, t.DbhMm, t.HeightMm))), "B is not a copy of A");
            Console.WriteLine($"MA-F02 A: {a.Trees.Count} trees, {sa.StemsPerHa:F1}/ha, BA {sa.BasalAreaM2PerHa:F2} m2/ha, Dq {sa.QuadraticMeanDbhCm:F2} cm, RD {sa.RelativeDensity:F3}; " +
                $"B: {b.Trees.Count} trees, {sb.StemsPerHa:F1}/ha, BA {sb.BasalAreaM2PerHa:F2} m2/ha, Dq {sb.QuadraticMeanDbhCm:F2} cm, RD {sb.RelativeDensity:F3}");
        });

        T.Add("multi MA-F03: tree IDs are globally unique and area-prefixed; no P30 ID or P1 tree is reused; P1 loaders refuse the family", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var ids = e.Areas.SelectMany(area => area.Trees.Select(t => t.TreeId)).ToArray();
            T.Eq(ids.Length, ids.Distinct(StringComparer.Ordinal).Count(), "unique by ordinal comparison");
            T.Eq(ids.Length, ids.Distinct(StringComparer.OrdinalIgnoreCase).Count(), "unique ignoring case");
            foreach (var area in e.Areas)
                T.True(area.Trees.All(t => t.TreeId.StartsWith(area.AreaId + "-", StringComparison.Ordinal)), area.AreaId + " prefix");
            T.True(ids.All(id => !id.StartsWith("P30-", StringComparison.OrdinalIgnoreCase)), "no P30- ID");
            var p1 = DerivedManifest.Parse(File.ReadAllBytes(manifestPath));
            var p1Ids = p1.Trees.Select(t => t.TreeId).ToHashSet(StringComparer.OrdinalIgnoreCase);
            T.True(!ids.Any(p1Ids.Contains), "no P1 ID");
            T.True(e.Areas.SelectMany(area => area.Trees).All(t => t.AgeYears != 30), "no tree of the all-30-year P1 roster");
            T.Refuses("UnknownDomain", () => P1DomainWire.Parse(MultiAreaIdentity.Domain));
            T.True(DerivedCatalog.Find(e.ExperimentId) is null, "the experiment is not a P1 catalog record");
            T.Refuses("NotDerivedFixtureId", () => FixtureAccess.ReadDerived(fixtureDirectory, e.ExperimentId, TrustUse.ReviewEvidenceOnly));
        });

        T.Add("multi MA-F04: every experiment identity, geometry and range defect is refused for its own reason", () =>
        {
            var e = MultiAreaGenerator.Generate();
            void Refused(string code, MultiAreaExperiment broken) => T.Refuses(code, () => new MultiAreaContinuation(broken));
            var shiftedB = WithArea(e, 1, area => area with
            {
                PolygonMm = Array.AsReadOnly(area.PolygonMm.Select(p => new MillimetrePoint(p.XMm - 3_000, p.YMm)).ToArray()),
                Trees = Array.AsReadOnly(area.Trees.Select(t => t with { XMm = t.XMm - 3_000 }).ToArray()),
            });
            Refused("ExperimentIdInvalid", e with { ExperimentId = "p1d-30m-sitka-adult-v1" });
            Refused("AreaCountInvalid", e with { Areas = Array.AsReadOnly(new[] { e.Areas[0] }) });
            Refused("AreaIdInvalid", WithArea(e, 0, area => area with { AreaId = "Area-A" }));
            Refused("AreaIdOrder", e with { Areas = Array.AsReadOnly(new[] { e.Areas[1], e.Areas[0] }) });
            Refused("PolygonInvalid", WithArea(e, 0, area => area with { PolygonMm = Array.AsReadOnly(area.PolygonMm.Reverse().ToArray()) }));
            Refused("PolygonInvalid", WithArea(e, 0, area => area with { PolygonMm = Array.AsReadOnly(area.PolygonMm.Append(area.PolygonMm[0]).ToArray()) }));
            Refused("PolygonInvalid", WithArea(e, 1, area => area with
            {
                PolygonMm = Array.AsReadOnly(new[] { area.PolygonMm[0], area.PolygonMm[2], area.PolygonMm[1], area.PolygonMm[3] }),
            }));
            Refused("PolygonInvalid", WithArea(e, 1, area => area with
            {
                PolygonMm = Array.AsReadOnly(new[] { area.PolygonMm[0], new MillimetrePoint(63_000, 1_000), area.PolygonMm[1], area.PolygonMm[2], area.PolygonMm[3] }),
            }));
            Refused("PolygonInvalid", WithArea(e, 0, area => area with { PolygonMm = Array.AsReadOnly(area.PolygonMm.Take(2).ToArray()) }));
            Refused("SeedNotUnique", WithArea(e, 1, area => area with { SimulationSeed = e.Areas[0].SimulationSeed }));
            Refused("AreaEmpty", WithArea(e, 1, area => area with { Trees = Array.AsReadOnly(Array.Empty<MultiAreaTreeDefinition>()) }));
            Refused("TreeIdInvalid", WithArea(e, 0, area => WithTree(area, 0, t => t with { TreeId = "MA-A 001" })));
            Refused("TreeIdReservedFamily", WithArea(e, 0, area => WithTree(area, 0, t => t with { TreeId = "P30-001" })));
            Refused("TreeAreaMismatch", WithArea(e, 0, area => WithTree(area, 0, t => t with { TreeId = "MA-B-000" })));
            Refused("TreeIdOrder", WithArea(e, 0, area => WithTree(area, 1, t => t with { TreeId = area.Trees[0].TreeId })));
            var caseTwins = WithArea(e, 0, area => WithTree(WithTree(area, 169, t => t with { TreeId = "MA-A-X01" }), 170, t => t with { TreeId = "MA-A-x01" }));
            Refused("TreeIdCaseCollision", caseTwins);
            Refused("TreeOutsideArea", WithArea(e, 0, area => WithTree(area, 0, t => t with { XMm = 1_000, YMm = 1_000 })));
            // The midpoint of the boundary segment MA-A and MA-B share: on an edge is never inside.
            Refused("TreeOutsideArea", WithArea(e, 1, area => WithTree(area, 0, t => t with { XMm = 38_000, YMm = 10_000 })));
            Refused("DuplicatePosition", WithArea(e, 0, area => WithTree(area, 1, t => t with { XMm = area.Trees[0].XMm, YMm = area.Trees[0].YMm })));
            Refused("AgeInvalid", WithArea(e, 0, area => WithTree(area, 0, t => t with { AgeYears = 0 })));
            Refused("DbhOutOfDomain", WithArea(e, 0, area => WithTree(area, 0, t => t with { DbhMm = 2_001 })));
            Refused("HeightOutOfDomain", WithArea(e, 0, area => WithTree(area, 0, t => t with { HeightMm = 60_001 })));
            Refused("AreasOverlap", shiftedB);
            new MultiAreaContinuation(e); // control: the unmodified experiment is accepted
        });

        T.Add("multi MA-F05: exact geometry: shared edges and corners touch without overlap; areas and decimals are exact", () =>
        {
            var e = MultiAreaGenerator.Generate();
            T.True(!MultiAreaGeometry.Overlap(e.Areas[0].PolygonMm, e.Areas[1].PolygonMm), "A and B share an edge without overlapping");
            MillimetrePoint[] Square(long x, long y, long side) => new MillimetrePoint[] { new(x, y), new(x + side, y), new(x + side, y + side), new(x, y + side) };
            T.True(!MultiAreaGeometry.Overlap(Square(0, 0, 10), Square(10, 10, 10)), "corner touch is not overlap");
            T.True(!MultiAreaGeometry.Overlap(Square(0, 0, 10), Square(10, 5, 10)), "partial shared edge is not overlap");
            T.True(MultiAreaGeometry.Overlap(Square(0, 0, 10), Square(9, 9, 10)), "a small overlap is overlap");
            T.True(MultiAreaGeometry.Overlap(Square(0, 0, 10), Square(0, 0, 10)), "identical squares overlap");
            T.True(MultiAreaGeometry.Overlap(Square(0, 0, 10), Square(2, 2, 3)), "containment is overlap");
            T.True(!MultiAreaGeometry.StrictlyInside(Square(0, 0, 10), new MillimetrePoint(10, 5)), "a point on an edge is outside");
            T.True(MultiAreaGeometry.StrictlyInside(Square(0, 0, 10), new MillimetrePoint(5, 5)), "the centre is inside");
            T.Eq("0.0000005", MultiAreaGeometry.ExactSquareMetres(1), "smallest unit");
            T.Eq("1.5000005", MultiAreaGeometry.ExactSquareMetres(3_000_001), "exact fraction");
            T.Eq("1", MultiAreaGeometry.ExactSquareMetres(2_000_000), "whole square metre");
            var big = Square(0, 0, MultiAreaGeometry.MaxCoordinateMm);
            T.Eq(2 * MultiAreaGeometry.MaxCoordinateMm * MultiAreaGeometry.MaxCoordinateMm, MultiAreaGeometry.TwiceSignedAreaMm2(big),
                "the largest allowed square has no overflow");
            MultiAreaGeometry.RequireStrictlyConvexCounterClockwise(big, "largest square");
        });

        // ---------------------------------------------------------------------------------------------- ecology (MA-E)
        T.Add("multi MA-E01: the per-area annual step reproduces the ten frozen P1 no-work digests (one-area equivalence)", () =>
        {
            var state = AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1, ManifestGenerator.FixtureId, fixtureDirectory).State;
            var digests = (string[])typeof(ContinuationTests).GetField("Digests", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
            T.Eq(10, digests.Length, "the ten frozen digests");
            for (int year = 1; year <= digests.Length; year++)
            {
                var (report, next) = MultiAreaAnnual.Step(state, (int)ManifestGenerator.SimulationSeed, F32.FromBits(DerivedManifest.AreaHaBits));
                var bytes = JsonSerializer.SerializeToUtf8Bytes(new { Snapshot = new ContinuationSnapshot(next, year), Report = report });
                T.Eq(digests[year - 1], DerivedManifest.Sha256Hex(bytes), "frozen digest of year " + year);
                state = next;
            }
        });

        T.Add("multi MA-E02: Year+1 runs the unchanged annual rules once per area with that area's own trees, hectares and seed", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var result = c.Advance(Fence(c.ReadView()), consentToCancelWork: false);
            var view = Ok(result, "first year");
            T.True(view.Source.EcologicalYear == 1 && view.Source.AnnualRevision == 1 && view.Areas.All(area => Area(view, area.AreaId).Trees.Count > 0),
                "one shared year and one annual revision");
            for (int i = 0; i < e.Areas.Count; i++)
            {
                var area = e.Areas[i];
                var genesis = new AnnualState(0, MultiAreaExperiment.GenesisTrees(area));
                var expected = AnnualRules.Calculate(genesis, area.SimulationSeed, area.AreaHa);
                var actual = result.Annual!.Areas[i];
                T.Eq(area.AreaId, actual.AreaId, "area order");
                T.True(Bytes(expected).SequenceEqual(Bytes(actual.Report!)), area.AreaId + " report equals a standalone Calculate of that area");
                T.True(Bytes(expected.Trees.Select(row => row.After).ToArray()).SequenceEqual(Bytes(Area(view, area.AreaId).Trees.ToArray())),
                    area.AreaId + " committed roster equals the report's after-trees");
                var record = view.AnnualHistory.Single().Areas[i];
                T.True(record.Before == expected.Before && record.After == expected.After && !record.NoLivingStems &&
                    record.NaturalDeaths.SequenceEqual(expected.Deaths), area.AreaId + " history equals the report");
            }
            var reportA = result.Annual!.Areas[0].Report!;
            T.True(reportA.Deaths.Count > 0 && result.Annual.Areas[1].Report!.Deaths.Count == 0,
                $"A thins naturally ({reportA.Deaths.Count} deaths) while B does not, in the same year");
            var otherSeed = AnnualRules.Calculate(new AnnualState(0, MultiAreaExperiment.GenesisTrees(e.Areas[0])), e.Areas[1].SimulationSeed, e.Areas[0].AreaHa);
            T.True(!Bytes(otherSeed).SequenceEqual(Bytes(reportA)), "A's own seed is used: B's seed would give A a different year");
            // Evaluation order cannot matter: each area depends only on its own state, seed and hectares.
            var reversed = e.Areas.Reverse().Select(area => AnnualRules.Calculate(new AnnualState(0, MultiAreaExperiment.GenesisTrees(area)),
                area.SimulationSeed, area.AreaHa)).Reverse().ToArray();
            for (int i = 0; i < reversed.Length; i++)
                T.True(Bytes(reversed[i]).SequenceEqual(Bytes(result.Annual.Areas[i].Report!)), "reverse evaluation order, area " + i);
        });

        T.Add("multi MA-E03: ten no-work years are deterministic across sessions and keep one shared clock", () =>
        {
            var first = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var second = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            for (int year = 1; year <= 10; year++)
            {
                var x = first.Advance(Fence(first.ReadView()), false);
                var y = second.Advance(Fence(second.ReadView()), false);
                T.True(x.RefusalCode is null && y.RefusalCode is null, "year " + year + " committed");
                T.True(Bytes(x.Annual!).SequenceEqual(Bytes(y.Annual!)), "identical annual results in year " + year);
                T.True(Bytes(x.View.Areas).SequenceEqual(Bytes(y.View.Areas)), "identical rosters in year " + year);
                T.True(x.View.Areas.All(area => area.Trees.All(t => t.AgeYears > 0)) && x.View.Source.EcologicalYear == year &&
                    x.View.Source.AnnualRevision == year && x.View.AnnualHistory.Count == year, "shared year and revision " + year);
                Console.WriteLine($"MA-E03 year {year}: A living {Area(x.View, A).Stand.LivingCount}, B living {Area(x.View, B).Stand.LivingCount}, " +
                    $"A deaths {x.Annual!.Areas[0].Report!.Deaths.Count}, B deaths {x.Annual.Areas[1].Report!.Deaths.Count}");
            }
        });

        T.Add("multi MA-E04: neighbours across the shared boundary are excluded from competition (isolation proof on real trees)", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var view = c.ReadView();
            var (a, b) = (Area(view, A), Area(view, B));
            var pairs = (from ta in a.Trees from tb in b.Trees where Distance(ta, tb) <= 8f select (ta, tb)).ToArray();
            T.True(pairs.Length > 0, "real A-B tree pairs lie within the 8 m cutoff");
            var aloneA = AnnualRules.Calculate(new AnnualState(0, a.Trees), a.SimulationSeed, a.AreaHa);
            var merged = AnnualRules.Calculate(new AnnualState(0, a.Trees.Concat(b.Trees).ToArray()), a.SimulationSeed, a.AreaHa);
            int checkedTargets = 0;
            foreach (var target in pairs.Select(p => p.ta).DistinctBy(t => t.TreeId))
            {
                var report = c.Competition(Fence(view), A, target.TreeId);
                var bIds = b.Trees.Select(t => t.TreeId).ToHashSet(StringComparer.Ordinal);
                T.True(report.Competition!.Neighbours.All(n => !bIds.Contains(n.TreeId)), target.TreeId + " lists no MA-B neighbour");
                var alone = aloneA.Trees.Single(row => row.Before.TreeId == target.TreeId);
                var withB = merged.Trees.Single(row => row.Before.TreeId == target.TreeId);
                T.Eq(F32.Bits(alone.PreGrowthCi), F32.Bits(report.Competition.CurrentTotal), target.TreeId + " CI equals the A-only annual CI");
                T.True(withB.PreGrowthCi > alone.PreGrowthCi, target.TreeId + " would have a larger CI if MA-B trees were counted");
                checkedTargets++;
            }
            Console.WriteLine($"MA-E04 {pairs.Length} cross-boundary pairs within 8 m; {checkedTargets} MA-A targets checked");
        });

        T.Add("multi MA-E05: a naturally emptied area stays empty while another advances; Year+1 refuses only when every area is empty", () =>
        {
            var (experiment, doomedSeed) = DoomedExperiment(lastAreaAlsoDoomed: false);
            var c = new MultiAreaContinuation(experiment);
            var player = new MultiAreaWorkPlanAdapter(c, experiment);
            var v = Ok(player.MarkCrop(Fence(player.Read()), "MA-Z", "MA-Z-001"), "Crop on a tree that will die");
            v = Ok(player.Advance(Fence(v), false), "year 1");
            T.Eq(0, Area(v, "MA-Z").Stand.LivingCount, $"MA-Z died out naturally under seed {doomedSeed}");
            T.Eq(4, Area(v, "MA-L").Stand.LivingCount, "MA-L is alive");
            T.True(Area(v, "MA-Z").CropTreeIds.Count == 0, "the dead tree's Crop mark was cleared in the same publish");
            T.True(Area(v, "MA-Z").Trees.All(t => t.Death?.Cause == "self-thinning"), "natural causes, not Fell");
            var emptyRoster = Bytes(Area(v, "MA-Z").Trees);
            var second = player.Advance(Fence(v), false);
            v = Ok(second, "year 2 with one empty area");
            var zYear = v.AnnualHistory[^1].Areas.Single(area => area.AreaId == "MA-Z");
            T.True(zYear.NoLivingStems && zYear.NaturalDeaths.Count == 0 && second.Annual!.Areas.Single(r => r.AreaId == "MA-Z").Report is null,
                "the empty area is carried forward, not simulated");
            T.True(emptyRoster.SequenceEqual(Bytes(Area(v, "MA-Z").Trees)), "the empty roster is unchanged");
            T.True(second.Annual!.Areas.Single(r => r.AreaId == "MA-L").Report is not null && v.Source.EcologicalYear == 2,
                "the living area advanced into year 2");
            RefusedWithoutMutation(c, "WorkTreeNotLive", () => player.AddPending(Fence(v), "MA-Z", "MA-Z-002", "dead tree"), "Fell on a naturally dead tree");

            var (allDoomed, _) = DoomedExperiment(lastAreaAlsoDoomed: true);
            var c2 = new MultiAreaContinuation(allDoomed);
            var p2 = new MultiAreaWorkPlanAdapter(c2, allDoomed);
            var v2 = Ok(p2.Advance(Fence(p2.Read()), false), "the year in which both areas die");
            T.True(v2.Areas.All(area => area.Stand.LivingCount == 0), "every area is empty");
            RefusedWithoutMutation(c2, "ContinuationNoLivingStems", () => p2.Advance(Fence(v2), false), "Year+1 with every area empty");
        });

        T.Add("multi MA-E06: a fault after one area or before the publish leaves the year unpublished; the retry commits once", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var control = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            FaultWithoutMutation(c, () => c.Advance(Fence(c.ReadView()), false, afterArea: id => { if (id == A) Inject(); }), "after MA-A, before MA-B");
            FaultWithoutMutation(c, () => c.Advance(Fence(c.ReadView()), false, beforeCommit: Inject), "before the commit");
            var retry = c.Advance(Fence(c.ReadView()), false);
            var expected = control.Advance(Fence(control.ReadView()), false);
            T.True(Bytes(retry.Annual!).SequenceEqual(Bytes(expected.Annual!)) && retry.View.Source.AnnualRevision == 1, "the retry commits exactly one year");
        });

        // ---------------------------------------------------------------------------------------------- competition (MA-C)
        T.Add("multi MA-C01: any living tree in either area shows its ranked Hegyi competitors before any Crop designation, read-only", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var view = c.ReadView();
            var root = Root(c);
            var before = Bytes(view);
            int inspected = 0;
            foreach (var area in view.Areas)
            {
                var annual = AnnualRules.Calculate(new AnnualState(0, area.Trees), area.SimulationSeed, area.AreaHa);
                foreach (var row in annual.Trees.Where(row => row.Before.IsLiving))
                {
                    var result = c.Competition(Fence(view), area.AreaId, row.Before.TreeId);
                    var report = result.Competition!;
                    T.Eq(F32.Bits(row.PreGrowthCi), F32.Bits(report.CurrentTotal), row.Before.TreeId + " CI bits equal the annual CI");
                    T.Eq(row.Neighbours, report.Neighbours.Count, row.Before.TreeId + " neighbour count");
                    T.True(report.TopFive.Count == Math.Min(5, report.Neighbours.Count) &&
                        report.TopFive.SequenceEqual(report.Neighbours.OrderByDescending(n => n.Contribution).ThenBy(n => n.TreeId, StringComparer.Ordinal).Take(5)),
                        "top five strongest, ordinal ties");
                    T.True(report.TopFive.All(n => n.Strength is "Low" or "Moderate" or "Strong" && double.IsFinite(n.SharePercent)),
                        "share and strength bands");
                    inspected++;
                }
            }
            T.True(ReferenceEquals(root, Root(c)), "inspection published nothing");
            T.True(before.SequenceEqual(Bytes(c.ReadView())), "the view is unchanged");
            var after = c.ReadView();
            T.True(after.Areas.All(area => area.CropTreeIds.Count == 0) && after.Pending.Count == 0 && after.Approved is null,
                "no tree was auto-Cropped or auto-proposed for Fell, however strong its competition");
            T.Eq(244, inspected, "every living tree of both areas was inspected");
        });

        T.Add("multi MA-C02: competition refuses unknown, dead, wrong-area and stale requests without any change", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var v = player.Read();
            string aTree = Area(v, A).Trees[10].TreeId;
            CompetitionRefused(c, player, "CompetitionTreeUnknown", Fence(v), A, "MA-A-999");
            CompetitionRefused(c, player, "MultiAreaWrongArea", Fence(v), B, aTree);
            CompetitionRefused(c, player, "MultiAreaUnknownArea", Fence(v), "MA-C", aTree);
            CompetitionRefused(c, player, "PlayerPlanStateStale", Fence(v) with { PlanStateRevision = 1 }, A, aTree);
            CompetitionRefused(c, player, "PlayerSourceStale", Fence(v) with { Source = v.Source with { WorkRevision = 1 } }, A, aTree);
            CompetitionRefused(c, player, "PlayerSessionMismatch", Fence(v) with { SessionId = "other" }, A, aTree);
            var felled = ExecuteOne(player, A, aTree, "competition dead-target probe");
            CompetitionRefused(c, player, "CompetitionTargetDead", Fence(felled), A, aTree);
        });

        // ---------------------------------------------------------------------------------------------- Work Plan (MA-W)
        T.Add("multi MA-W01: Crop and Fell in both areas, one combined plan; Approve fells zero and mints the PlanId and key", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var v0 = player.Read();
            string crop = Area(v0, A).Trees[60].TreeId;
            var report = player.Competition(Fence(v0), A, crop).Competition!;
            string competitor = report.TopFive[0].TreeId;
            string bTree = Area(v0, B).Trees[20].TreeId;
            var v = Ok(player.MarkCrop(Fence(v0), A, crop), "Crop in A");
            T.True(Area(v, A).CropTreeIds.SequenceEqual(new[] { crop }) && v.PlanStateRevision == 1, "Crop is session state, plan-state +1");
            v = Ok(player.AddPending(Fence(v), A, competitor, "Competition release for retained Crop Tree " + crop + " (operator intent)"), "Fell in A");
            v = Ok(player.AddPending(Fence(v), B, bTree, "ui test: manual selection in area B"), "Fell in B");
            T.True(v.Pending.Select(o => (o.AreaId, o.TreeId)).SequenceEqual(new[] { (A, competitor), (B, bTree) }), "one pending list with both areas");
            var approved = Ok(player.Approve(Fence(v)), "approve");
            var plan = approved.Approved!;
            T.Eq($"{approved.SessionId}:ma-plan:{approved.PlanStateRevision}", plan.PlanId, "engine-minted PlanId");
            T.Eq(plan.PlanId + ":execute", plan.ExecutionKey, "engine-minted execution key");
            T.True(plan.Orders.Count == 2 && plan.Source == approved.Source && approved.Pending.Count == 0, "the whole plan is bound to the current source");
            T.True(approved.Areas.All(area => area.Stand.LivingCount == Area(v0, area.AreaId).Stand.LivingCount) &&
                approved.Events.Count == 0 && approved.Source.WorkRevision == 0, "Approve fells ZERO trees in both areas");
            var scene = MultiAreaProjection.Scene(approved, Area(approved, A));
            T.Eq(PlayerMapStatus.ApprovedFell, scene.Single(item => item.TreeId == competitor).Status, "the map shows Approved, still standing");
            T.True(scene.Single(item => item.TreeId == crop).IsCrop, "the map shows the Crop mark");
        });

        T.Add("multi MA-W02: Execute fells exactly the approved trees in both areas in one event with per-area before/after", () =>
        {
            var (c, player, approved, plan) = ApprovedTwoAreaPlan();
            var rootBefore = Root(c);
            var result = player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey);
            var v = Ok(result, "execute");
            var ev = result.Execution!.Event;
            T.True(!result.Execution.Replayed && ev.PlanId == plan.PlanId && ev.ExecutionKey == plan.ExecutionKey, "a first execution");
            T.True(v.Events.Count == 1 && v.Source.WorkRevision == 1 && v.Source.EcologicalYear == 0 && v.Source.AnnualRevision == 0,
                "one event; work revision 0 -> 1; same year and annual revision");
            T.True(v.Approved is null && v.PlanStateRevision == approved.PlanStateRevision + 1, "the approval is cleared");
            var felled = v.Areas.SelectMany(area => area.Trees.Where(t => t.Death?.Cause == FellCause.Harvest).Select(t => (area.AreaId, t.TreeId)))
                .OrderBy(x => x.TreeId, StringComparer.Ordinal).ToArray();
            var ordered = plan.Orders.Select(o => (o.AreaId, o.TreeId)).OrderBy(x => x.TreeId, StringComparer.Ordinal).ToArray();
            T.True(felled.SequenceEqual(ordered), "exactly the approved IDs are felled, in their own areas");
            foreach (var change in ev.Areas)
            {
                var areaBefore = rootBefore.Areas.Single(area => area.AreaId == change.AreaId);
                var areaView = Area(v, change.AreaId);
                T.Eq(WorkRules.Summarize(areaBefore.State.Trees, areaView.AreaHa), change.Before, change.AreaId + " before");
                T.Eq(areaView.Stand, change.After, change.AreaId + " after equals the committed stand");
                T.Eq(change.Before.LivingCount - change.Fells.Count, change.After.LivingCount, change.AreaId + " living count falls by its Fells");
                T.True(change.After.BasalAreaM2PerHa < change.Before.BasalAreaM2PerHa, change.AreaId + " basal area falls");
                T.True(change.Fells.All(f => f.Cause == "fell" && f.Mechanism == "manual-work-plan" && f.AreaId == change.AreaId &&
                    f.EcologicalYear == 0 && f.WorkRevision == 1), change.AreaId + " typed Fell records");
            }
            T.True(ev.Areas.Select(change => change.AreaId).SequenceEqual(new[] { A, B }) && ev.Areas.All(change => change.Fells.Count == 1),
                "one receipt names both areas, one Fell each");
        });

        T.Add("multi MA-W03: after a cross-area Fell, Year+1 runs the annual rules on each area's real survivors", () =>
        {
            var (c, player, approved, plan) = ApprovedTwoAreaPlan();
            var v = Ok(player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey), "execute");
            var postFell = v.Areas.ToDictionary(area => area.AreaId, area => area.Trees);
            var year = player.Advance(Fence(v), false);
            var next = Ok(year, "year after Fell");
            foreach (var area in next.Areas)
            {
                var expected = AnnualRules.Calculate(new AnnualState(0, postFell[area.AreaId]), area.SimulationSeed, area.AreaHa);
                T.True(Bytes(expected).SequenceEqual(Bytes(year.Annual!.Areas.Single(r => r.AreaId == area.AreaId).Report!)),
                    area.AreaId + " year equals Calculate on its post-Fell roster");
            }
            foreach (var order in plan.Orders)
            {
                var tree = Area(next, order.AreaId).Trees.Single(t => t.TreeId == order.TreeId);
                T.True(tree.Death is { Cause: "fell", EcologicalYear: 0 }, order.TreeId + " stays felled with its cause and year");
            }
            T.True(next.Events.Count == 1 && next.AnnualHistory.Single().WorkRevision == 1 && !next.AnnualHistory.Single().CancelledUnexecutedWork,
                "the year record carries the work revision");
        });

        T.Add("multi MA-W04: identical replay returns the receipt once; altered or unminted keys refuse; nothing is felled twice", () =>
        {
            var (c, player, approved, plan) = ApprovedTwoAreaPlan();
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict", () => player.Execute(Fence(approved), plan.PlanId, plan.PlanId + ":other"), "unminted key");
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict", () => player.Execute(Fence(approved), plan.PlanId, plan.PlanId), "the PlanId as key");
            var first = player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey);
            Ok(first, "first execution");
            var root = Root(c);
            var replay = player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey);
            T.True(replay.RefusalCode is null && replay.Execution!.Replayed, "identical retry is a replay");
            T.Eq(first.Execution!.Event, replay.Execution!.Event, "the same recorded receipt");
            T.True(ReferenceEquals(root, Root(c)), "a replay publishes nothing");
            T.True(replay.View.Events.Count == 1 && replay.View.Source.WorkRevision == 1, "no second Fell");
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict", () => player.Execute(Fence(approved), plan.PlanId + "x", plan.ExecutionKey), "same key, another PlanId");
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(approved) with { Source = approved.Source with { WorkRevision = 1 } }, plan.PlanId, plan.ExecutionKey), "same key, another source");
            RefusedWithoutMutation(c, "PlayerSessionMismatch", () => player.Execute(Fence(approved) with { SessionId = "other" }, plan.PlanId, plan.ExecutionKey), "another session");
        });

        T.Add("multi MA-W05: stale year, revisions, plan state, session or any token field refuse every command with zero mutation", () =>
        {
            var (c, player, approved, plan) = ApprovedTwoAreaPlan();
            string livingA = Area(approved, A).Trees.First(t => t.IsLiving && !plan.Orders.Any(o => o.TreeId == t.TreeId)).TreeId;
            var good = Fence(approved);
            var cases = OneFieldChanges(good.Source).Select(change => (change.Field, Fence: good with { Source = change.Token }, Code: "PlayerSourceStale"))
                .Append(("PlanStateRevision", good with { PlanStateRevision = good.PlanStateRevision + 1 }, "PlayerPlanStateStale"))
                .Append(("SessionId", good with { SessionId = good.SessionId + "x" }, "PlayerSessionMismatch")).ToArray();
            T.Eq(14, cases.Length, "twelve token fields, the plan state and the session");
            foreach (var (field, fence, code) in cases)
            {
                foreach (var (what, call) in AllCommands(player, fence, livingA, plan))
                    RefusedWithoutMutation(c, code, call, what + " with a stale " + field);
                var hit = player.HitTest(fence, A, 5, 5);
                T.True(hit.RefusalCode == code && hit.Candidates.Count == 0, "hit-test with a stale " + field);
                CompetitionRefused(c, player, code, fence, A, livingA);
            }
            var executed = player.Execute(good, plan.PlanId, plan.ExecutionKey);
            T.True(executed.RefusalCode is null, "the original fence still executes");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.Advance(good, false), "a pre-Execute fence for Year+1");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.AddPending(good, A, livingA, "pre-Execute fence"), "a pre-Execute fence for Add");
        });

        T.Add("multi MA-W06: wrong-area, unknown, dead, duplicate and Crop-protected selections refuse with zero mutation", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var v = player.Read();
            string aTree = Area(v, A).Trees[5].TreeId, bTree = Area(v, B).Trees[5].TreeId, crop = Area(v, A).Trees[6].TreeId;
            RefusedWithoutMutation(c, "MultiAreaWrongArea", () => player.AddPending(Fence(v), B, aTree, "wrong area"), "Fell an A tree under B");
            RefusedWithoutMutation(c, "MultiAreaWrongArea", () => player.MarkCrop(Fence(v), A, bTree), "Crop a B tree under A");
            RefusedWithoutMutation(c, "MultiAreaUnknownArea", () => player.AddPending(Fence(v), "MA-C", aTree, "no such area"), "unknown area");
            RefusedWithoutMutation(c, "UnknownTreeId", () => player.AddPending(Fence(v), A, "MA-A-999", "no such tree"), "unknown tree");
            RefusedWithoutMutation(c, "UnknownTreeId", () => player.AddPending(Fence(v), A, aTree.ToLowerInvariant(), "wrong case"), "case-shifted ID");
            RefusedWithoutMutation(c, "WorkReasonRequired", () => player.AddPending(Fence(v), A, aTree, "   "), "blank reason");
            RefusedWithoutMutation(c, "WorkEmptyPlan", () => player.Approve(Fence(v)), "approve nothing");
            RefusedWithoutMutation(c, "WorkPlanNotApproved", () => player.CancelApproved(Fence(v)), "cancel nothing");
            RefusedWithoutMutation(c, "WorkPlanNotApproved", () => player.Execute(Fence(v), "invented", "invented:execute"), "execute nothing");
            RefusedWithoutMutation(c, "UnknownWorkOrder", () => player.RemovePending(Fence(v), A, aTree), "remove a non-pending ID");
            v = Ok(player.MarkCrop(Fence(v), A, crop), "Crop");
            RefusedWithoutMutation(c, "CropTreeRetained", () => player.AddPending(Fence(v), A, crop, "crop"), "Fell a retained Crop");
            RefusedWithoutMutation(c, "CropAlreadyMarked", () => player.MarkCrop(Fence(v), A, crop), "Crop twice");
            v = Ok(player.AddPending(Fence(v), A, aTree, "first"), "add");
            RefusedWithoutMutation(c, "WorkDuplicateTreeId", () => player.AddPending(Fence(v), A, aTree, "again"), "duplicate ID");
            RefusedWithoutMutation(c, "CropAlreadyPlannedForFell", () => player.MarkCrop(Fence(v), A, aTree), "Crop a pending Fell");
            RefusedWithoutMutation(c, "MultiAreaWrongArea", () => player.RemovePending(Fence(v), B, aTree), "remove under the wrong area");
            v = Ok(player.AddPending(Fence(v), B, bTree, "b"), "add B");
            v = Ok(player.Approve(Fence(v)), "approve");
            RefusedWithoutMutation(c, "WorkPlanAlreadyApproved", () => player.AddPending(Fence(v), A, Area(v, A).Trees[7].TreeId, "late"), "add after approval");
            RefusedWithoutMutation(c, "WorkPlanAlreadyApproved", () => player.RemovePending(Fence(v), A, aTree), "remove after approval");
            RefusedWithoutMutation(c, "WorkPlanAlreadyApproved", () => player.Approve(Fence(v)), "approve twice");
            RefusedWithoutMutation(c, "CropNotMarked", () => player.UnmarkCrop(Fence(v), A, aTree), "unmark a non-Crop");
            v = Ok(player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey), "execute");
            RefusedWithoutMutation(c, "WorkTreeNotLive", () => player.AddPending(Fence(v), A, aTree, "already felled"), "Fell a felled tree");
            RefusedWithoutMutation(c, "UnknownTreeId", () => player.MarkCrop(Fence(v), A, aTree), "Crop a felled tree");
            v = Ok(player.UnmarkCrop(Fence(v), A, crop), "unmark");
            v = Ok(player.AddPending(Fence(v), A, crop, "unmarked, so now eligible"), "a former Crop can be proposed after an explicit unmark");
        });

        T.Add("multi MA-W07: a batch that would leave any populated area with no living tree is refused whole; the approval stays", () =>
        {
            var (experiment, _) = DoomedExperiment(lastAreaAlsoDoomed: false);
            var c = new MultiAreaContinuation(experiment);
            var player = new MultiAreaWorkPlanAdapter(c, experiment);
            var v = player.Read();
            foreach (var tree in Area(v, "MA-Z").Trees) v = Ok(player.AddPending(Fence(v), "MA-Z", tree.TreeId, "empty this area"), "add " + tree.TreeId);
            v = Ok(player.AddPending(Fence(v), "MA-L", "MA-L-001", "other area survives"), "add in the living area");
            v = Ok(player.Approve(Fence(v)), "approve a plan that would empty MA-Z");
            RefusedWithoutMutation(c, "WorkWouldLeaveNoLivingStems", () => player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey),
                "Fell every living tree of MA-Z while MA-L keeps three");
            T.True(player.Read().Approved is not null && Area(player.Read(), "MA-L").Stand.LivingCount == 4, "nothing felled anywhere; the plan is still approved");
            v = Ok(player.CancelApproved(Fence(player.Read())), "cancel");
            v = Ok(player.AddPending(Fence(v), "MA-Z", "MA-Z-001", "leave one alive"), "re-add one");
            v = Ok(player.Approve(Fence(v)), "approve a survivable plan");
            v = Ok(player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey), "a plan leaving a survivor executes");
            T.Eq(1, Area(v, "MA-Z").Stand.LivingCount, "one tree remains in MA-Z");
        });

        T.Add("multi MA-W08: a fault after area A is built, or before the publish, leaves BOTH areas, events and revisions unchanged", () =>
        {
            var (c, player, approved, plan) = ApprovedTwoAreaPlan();
            FaultWithoutMutation(c, () => c.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey, afterAreaBuilt: id => { if (id == A) Inject(); }),
                "after MA-A's roster, before MA-B's");
            FaultWithoutMutation(c, () => c.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey, beforePublish: Inject), "before the publish");
            T.True(Root(c).Executed.Count == 0, "no execution key was recorded");
            var retry = player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey);
            T.True(retry.RefusalCode is null && !retry.Execution!.Replayed && retry.View.Events.Count == 1, "the retry commits once");
        });

        T.Add("multi MA-W09: unexecuted work blocks Year+1 without consent; consent cancels ALL of it at zero Fell, then a real year", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var v = player.Read();
            string aTree = Area(v, A).Trees[3].TreeId, bTree = Area(v, B).Trees[3].TreeId;
            v = Ok(player.AddPending(Fence(v), A, aTree, "pending in A"), "pending A");
            v = Ok(player.AddPending(Fence(v), B, bTree, "pending in B"), "pending B");
            RefusedWithoutMutation(c, "WorkPendingConsentRequired", () => player.Advance(Fence(v), false), "pending work, no consent");
            var cancelled = Ok(player.Advance(Fence(v), true), "consent");
            T.True(cancelled.Pending.Count == 0 && cancelled.Events.Count == 0 && cancelled.Source.EcologicalYear == 1 &&
                cancelled.AnnualHistory.Single().CancelledUnexecutedWork && cancelled.PlanStateRevision == v.PlanStateRevision + 1,
                "both areas' orders cancelled at zero Fell; one real year");
            T.True(Area(cancelled, B).Trees.Single(t => t.TreeId == bTree).IsLiving, "the B order was not executed");
            var approved = Ok(player.AddPending(Fence(cancelled), B, bTree, "approved in B"), "re-add");
            approved = Ok(player.Approve(Fence(approved)), "approve");
            RefusedWithoutMutation(c, "WorkPendingConsentRequired", () => player.Advance(Fence(approved), false), "approved work, no consent");
            var year2 = Ok(player.Advance(Fence(approved), true), "consent cancels the approved plan");
            T.True(year2.Approved is null && year2.Events.Count == 0 && year2.Source.EcologicalYear == 2, "approved plan cancelled at zero Fell");
            var plain = Ok(player.Advance(Fence(year2), false), "a no-work year needs no consent");
            T.True(plain.Events.Count == 0 && !plain.AnnualHistory[^1].CancelledUnexecutedWork && plain.PlanStateRevision == year2.PlanStateRevision,
                "no-work years create no work and keep the plan state");
        });

        T.Add("multi MA-W10: concurrent readers never observe a partial cross-area commit; racing Execute and Year+1 commit one", () =>
        {
            const int Rounds = 10;
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var problems = new ConcurrentQueue<string>();
            long reads = 0;
            using var stop = new CancellationTokenSource();
            var readers = Enumerable.Range(0, 3).Select(_ => Task.Run(() =>
            {
                while (!stop.IsCancellationRequested)
                {
                    foreach (var problem in TornViewProblems(c.ReadView())) problems.Enqueue(problem);
                    Interlocked.Increment(ref reads);
                }
            })).ToArray();
            int executeWins = 0;
            try
            {
                for (int round = 0; round < Rounds; round++)
                {
                    var v = player.Read();
                    v = Ok(player.AddPending(Fence(v), A, FirstLiving(v, A), "race A " + round), "add A");
                    v = Ok(player.AddPending(Fence(v), B, FirstLiving(v, B), "race B " + round), "add B");
                    v = Ok(player.Approve(Fence(v)), "approve");
                    var fence = Fence(v);
                    var plan = v.Approved!;
                    using var gate = new Barrier(2);
                    var execute = Task.Run(() => { gate.SignalAndWait(); return player.Execute(fence, plan.PlanId, plan.ExecutionKey); });
                    var advance = Task.Run(() => { gate.SignalAndWait(); return player.Advance(fence, true); });
                    Task.WaitAll(execute, advance);
                    bool executed = execute.Result.RefusalCode is null, advanced = advance.Result.RefusalCode is null;
                    T.True(executed != advanced, $"round {round}: exactly one of Execute and Year+1 commits");
                    T.Eq("PlayerSourceStale", executed ? advance.Result.RefusalCode : execute.Result.RefusalCode, $"round {round}: the loser is stale");
                    if (executed) { executeWins++; Ok(player.Advance(Fence(player.Read()), false), "year after the Fell"); }
                }
            }
            finally
            {
                stop.Cancel();
                Task.WaitAll(readers);
            }
            T.True(problems.IsEmpty, "torn view: " + string.Join("; ", problems.Take(3)));
            var final = player.Read();
            T.Eq(Rounds, final.Source.AnnualRevision, "one year per round");
            T.Eq(executeWins, final.Events.Count, "one event per committed Execute");
            T.True(final.Events.All(ev => ev.Areas.Sum(change => change.Fells.Count) == 2), "every event felled one tree in each area");
            Console.WriteLine($"MA-W10 rounds {Rounds}, Execute won {executeWins}, Year+1 won {Rounds - executeWins}, atomic reads {Interlocked.Read(ref reads)}");
        });

        T.Add("multi MA-W11: views and receipts are owned read-only copies that later commands never change", () =>
        {
            var (c, player, approved, plan) = ApprovedTwoAreaPlan();
            var snapshot = Bytes(approved);
            var executed = player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey);
            var year = Ok(player.Advance(Fence(executed.View), false), "year");
            T.True(snapshot.SequenceEqual(Bytes(approved)), "an earlier view never changes");
            foreach (var (list, what) in new (System.Collections.IList, string)[]
                     {
                         ((System.Collections.IList)year.Areas, "Areas"), ((System.Collections.IList)year.Areas[0].Trees, "Trees"),
                         ((System.Collections.IList)year.Areas[0].Polygon, "Polygon"), ((System.Collections.IList)year.Areas[0].CropTreeIds, "CropTreeIds"),
                         ((System.Collections.IList)year.Pending, "Pending"), ((System.Collections.IList)year.Events, "Events"),
                         ((System.Collections.IList)year.Events[0].Areas, "Event areas"), ((System.Collections.IList)year.Events[0].Areas[0].Fells, "Fells"),
                         ((System.Collections.IList)year.AnnualHistory, "AnnualHistory"), ((System.Collections.IList)year.AnnualHistory[0].Areas, "Year areas"),
                         ((System.Collections.IList)approved.Approved!.Orders, "Approved orders"),
                     })
            {
                T.True(list.IsReadOnly && list is not Array, what + " is a read-only, non-array list");
            }
            T.True(!ReferenceEquals(Root(c).Areas[0].State.Trees, player.Read().Areas[0].Trees), "the roster is copied, not shared");
        });

        T.Add("multi MA-W12: tokens round-trip through strict camelCase JSON and compare by value; the facade has no seam", () =>
        {
            var view = new MultiAreaContinuation(MultiAreaGenerator.Generate()).ReadView();
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
            var fence = Fence(view);
            var back = JsonSerializer.Deserialize<MultiAreaFence>(JsonSerializer.Serialize(fence, options), options)!;
            T.True(back == fence && back.Source == view.Source, "a JSON round trip of the fence equals the original by value");
            T.Eq("MA-A,MA-B", view.Source.AreaIds, "the area set in the token");
            T.Eq(PinnedContentHash, view.Source.ManifestContentHash, "the token names the manifest content hash");
            var adapter = typeof(MultiAreaWorkPlanAdapter);
            var members = adapter.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsPrivate && !m.IsSpecialName).ToArray();
            // #900 UI catch-up adds the fenced Prune commands and the one-root UI read-outs (spec multi-stage4-ui-catchup-v1).
            T.True(members.Select(m => m.Name).Order(StringComparer.Ordinal).SequenceEqual(new[] { "AddPending", "AddPrune", "Advance", "Approve",
                "CancelApproved", "Competition", "Execute", "HitTest", "ManifestBytes", "MarkCrop", "OpenSynthetic", "Read", "ReadSnapshot",
                "ReadoutFor", "RemovePending", "RemovePrune", "UnmarkCrop" }),
                "exact adapter members: " + string.Join(",", members.Select(m => m.Name).Order()));
            foreach (var parameter in members.SelectMany(m => m.GetParameters()))
                T.True(!typeof(Delegate).IsAssignableFrom(parameter.ParameterType) && parameter.ParameterType != typeof(MultiAreaApprovedPlan),
                    "no delegate seam or caller-built plan: " + parameter.Name);
            T.True(!adapter.Assembly.GetExportedTypes().Any(t => t.Name.StartsWith("MultiArea", StringComparison.Ordinal)),
                "the public #849 surface is unchanged: every multi-area type is internal");
        });

        T.Add("multi MA-W13: every living tree's own centre hit-tests to it in its area's map; stale fences select nothing", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var view = player.Read();
            foreach (var area in view.Areas)
            {
                var scene = MultiAreaProjection.Scene(view, area);
                T.Eq(area.Trees.Count, scene.Count, area.AreaId + " one map item per tree");
                foreach (var item in scene.Where(item => item.Status == PlayerMapStatus.Living))
                {
                    var hit = player.HitTest(Fence(view), area.AreaId, item.U, item.V);
                    T.True(hit.RefusalCode is null && hit.Candidates.Any(candidate => candidate.TreeId == item.TreeId), item.TreeId + " hit at its centre");
                }
                var polygonU = area.Polygon.Select(p => p.XM - area.Extent.MinXM).ToArray();
                T.True(polygonU.All(u => u > 0 && u < area.Extent.SideM), area.AreaId + " polygon lies inside its map square");
            }
            var refused = player.HitTest(Fence(view) with { PlanStateRevision = 9 }, A, 10, 10);
            T.True(refused.RefusalCode == "PlayerPlanStateStale" && refused.Candidates.Count == 0, "stale fence: no candidates");
        });

        T.Add("multi MA-W14: a plan may touch one area only, and a no-action session can advance with zero work", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var v = Ok(player.AddPending(Fence(player.Read()), B, FirstLiving(player.Read(), B), "B only"), "add B");
            v = Ok(player.Approve(Fence(v)), "approve");
            var executed = player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            v = Ok(executed, "execute a one-area plan");
            var change = executed.Execution!.Event.Areas;
            T.True(change.Single(x => x.AreaId == A).Fells.Count == 0 && change.Single(x => x.AreaId == A).Before == change.Single(x => x.AreaId == A).After &&
                change.Single(x => x.AreaId == B).Fells.Count == 1, "the receipt still reports A, unchanged, and B with its Fell");
            var idle = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var w = Ok(idle.Advance(Fence(idle.ReadView()), false), "a no-action year");
            T.True(w.Events.Count == 0 && w.Source.WorkRevision == 0, "no work was created");
        });
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static MultiAreaFence Fence(MultiAreaSessionView view) => new(view.SessionId, view.Source, view.PlanStateRevision);

    private static byte[] Bytes(object value) => JsonSerializer.SerializeToUtf8Bytes(value);

    private static MultiAreaAreaView Area(MultiAreaSessionView view, string areaId) => view.Areas.Single(area => area.AreaId == areaId);

    private static string FirstLiving(MultiAreaSessionView view, string areaId) => Area(view, areaId).Trees.First(t => t.IsLiving).TreeId;

    private static float Distance(AnnualTree a, AnnualTree b)
    {
        float dx = a.XM - b.XM, dz = a.ZM - b.ZM;
        return (float)Math.Sqrt(dx * dx + dz * dz);
    }

    private static MultiAreaSessionView Ok(MultiAreaCommandResult result, string what)
    {
        T.True(result.RefusalCode is null, $"{what}: unexpected refusal {result.RefusalCode}: {result.RefusalMessage}");
        return result.View;
    }

    // Test-only inspection of the committed root, by reflection as in the single-area tests; no production seam.
    private static MultiAreaRoot Root(MultiAreaContinuation continuation) => (MultiAreaRoot)typeof(MultiAreaContinuation)
        .GetField("root", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(continuation)!;

    private static void RefusedWithoutMutation(MultiAreaContinuation continuation, string code, Func<MultiAreaCommandResult> command, string what)
    {
        var root = Root(continuation);
        var before = Bytes(continuation.ReadView());
        var result = command();
        T.Eq(code, result.RefusalCode, what + ": refusal code (" + result.RefusalMessage + ")");
        T.True(!string.IsNullOrWhiteSpace(result.RefusalMessage), what + ": refusal message");
        T.True(result.Execution is null && result.Annual is null, what + ": no execution or annual result");
        T.True(ReferenceEquals(root, Root(continuation)), what + ": nothing published");
        T.True(before.SequenceEqual(Bytes(result.View)), what + ": the returned view equals the pre-call view");
        T.True(before.SequenceEqual(Bytes(continuation.ReadView())), what + ": the current view is unchanged");
    }

    private static void CompetitionRefused(MultiAreaContinuation continuation, MultiAreaWorkPlanAdapter player, string code,
        MultiAreaFence fence, string areaId, string treeId)
    {
        var root = Root(continuation);
        var before = Bytes(continuation.ReadView());
        var result = player.Competition(fence, areaId, treeId);
        T.Eq(code, result.RefusalCode, "competition refusal for " + treeId + " in " + areaId);
        T.True(result.Competition is null && ReferenceEquals(root, Root(continuation)) && before.SequenceEqual(Bytes(result.View)),
            "a refused inspection returns the unchanged view and no report");
    }

    private static void FaultWithoutMutation(MultiAreaContinuation continuation, Action command, string what)
    {
        var root = Root(continuation);
        var before = Bytes(continuation.ReadView());
        bool injected = false;
        try { command(); }
        catch (InvalidOperationException e) when (e.Message == "injected") { injected = true; }
        T.True(injected, what + ": the fault seam was reached");
        T.True(ReferenceEquals(root, Root(continuation)), what + ": nothing published");
        T.True(before.SequenceEqual(Bytes(continuation.ReadView())), what + ": every area, event, revision and plan unchanged");
    }

    private static void Inject() => throw new InvalidOperationException("injected");

    private static MultiAreaExperiment WithArea(MultiAreaExperiment experiment, int index, Func<MultiAreaDefinition, MultiAreaDefinition> change)
    {
        var areas = experiment.Areas.ToArray();
        areas[index] = change(areas[index]);
        return experiment with { Areas = Array.AsReadOnly(areas) };
    }

    private static MultiAreaDefinition WithTree(MultiAreaDefinition area, int index, Func<MultiAreaTreeDefinition, MultiAreaTreeDefinition> change)
    {
        var trees = area.Trees.ToArray();
        trees[index] = change(trees[index]);
        return area with { Trees = Array.AsReadOnly(trees) };
    }

    /// <summary>A real session with one Crop in A and one approved Fell in each of A and B.</summary>
    private static (MultiAreaContinuation, MultiAreaWorkPlanAdapter, MultiAreaSessionView, MultiAreaApprovedPlan) ApprovedTwoAreaPlan()
    {
        var e = MultiAreaGenerator.Generate();
        var c = new MultiAreaContinuation(e);
        var player = new MultiAreaWorkPlanAdapter(c, e);
        var v = player.Read();
        string crop = Area(v, A).Trees[60].TreeId;
        string competitor = player.Competition(Fence(v), A, crop).Competition!.TopFive[0].TreeId;
        v = Ok(player.MarkCrop(Fence(v), A, crop), "crop");
        v = Ok(player.AddPending(Fence(v), A, competitor, "Competition release for retained Crop Tree " + crop + " (operator intent)"), "add A");
        v = Ok(player.AddPending(Fence(v), B, Area(v, B).Trees[20].TreeId, "manual selection in B"), "add B");
        v = Ok(player.Approve(Fence(v)), "approve");
        return (c, player, v, v.Approved!);
    }

    private static MultiAreaSessionView ExecuteOne(MultiAreaWorkPlanAdapter player, string areaId, string treeId, string reason)
    {
        var v = Ok(player.AddPending(Fence(player.Read()), areaId, treeId, reason), "add");
        v = Ok(player.Approve(Fence(v)), "approve");
        return Ok(player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey), "execute");
    }

    /// <summary>
    /// A test-only two-area experiment: MA-L keeps four widely spaced trees (never thinned), and MA-Z holds two very large
    /// trees 0.5 m apart on 9 m², which the unchanged density rules kill outright for some seeds. The first seed from 1
    /// upward under which the real AnnualRules.Calculate kills both MA-Z trees in year 1 is chosen, so the test is
    /// deterministic and uses no seam. With <paramref name="lastAreaAlsoDoomed"/>, MA-L is built the same way.
    /// </summary>
    private static (MultiAreaExperiment Experiment, int DoomedSeed) DoomedExperiment(bool lastAreaAlsoDoomed)
    {
        MultiAreaDefinition Doomed(string id, long x0, int seed) => new(id, id, "test-only doomed pair",
            Array.AsReadOnly(new MillimetrePoint[] { new(x0, 0), new(x0 + 3_000, 0), new(x0 + 3_000, 3_000), new(x0, 3_000) }), seed,
            Array.AsReadOnly(new[] { new MultiAreaTreeDefinition(id + "-001", x0 + 1_000, 1_500, 50, 2_000, 30_000),
                new MultiAreaTreeDefinition(id + "-002", x0 + 1_500, 1_500, 50, 2_000, 30_000) }));
        int FindSeed(string id, long x0, int avoid)
        {
            for (int seed = 1; ; seed++)
            {
                if (seed == avoid) continue;
                var area = Doomed(id, x0, seed);
                if (AnnualRules.Calculate(new AnnualState(0, MultiAreaExperiment.GenesisTrees(area)), seed, area.AreaHa).After.LivingCount == 0)
                    return seed;
            }
        }
        int zSeed = FindSeed("MA-Z", 30_000, avoid: 0);
        MultiAreaDefinition living;
        if (lastAreaAlsoDoomed)
            living = Doomed("MA-L", 0, FindSeed("MA-L", 0, avoid: zSeed));
        else
            living = new MultiAreaDefinition("MA-L", "MA-L", "test-only open stand",
                Array.AsReadOnly(new MillimetrePoint[] { new(0, 0), new(20_000, 0), new(20_000, 20_000), new(0, 20_000) }), zSeed + 1,
                Array.AsReadOnly(new[] { new MultiAreaTreeDefinition("MA-L-001", 2_000, 2_000, 40, 300, 22_000),
                    new MultiAreaTreeDefinition("MA-L-002", 18_000, 2_000, 40, 300, 22_000),
                    new MultiAreaTreeDefinition("MA-L-003", 2_000, 18_000, 40, 300, 22_000),
                    new MultiAreaTreeDefinition("MA-L-004", 18_000, 18_000, 40, 300, 22_000) }));
        var experiment = new MultiAreaExperiment("mad-test-doomed-v1", "test-only", 1, 1,
            Array.AsReadOnly(new[] { living, Doomed("MA-Z", 30_000, zSeed) }));
        return (experiment, zSeed);
    }

    private static IEnumerable<(string Field, MultiAreaSourceToken Token)> OneFieldChanges(MultiAreaSourceToken s)
    {
        yield return (nameof(s.Domain), s with { Domain = s.Domain + "x" });
        yield return (nameof(s.ExperimentId), s with { ExperimentId = s.ExperimentId + "x" });
        yield return (nameof(s.ManifestContentHash), s with { ManifestContentHash = new string('0', 64) });
        yield return (nameof(s.AreaIds), s with { AreaIds = "MA-A" });
        yield return (nameof(s.EcologicalYear), s with { EcologicalYear = s.EcologicalYear + 1 });
        yield return (nameof(s.AnnualRevision), s with { AnnualRevision = s.AnnualRevision + 1 });
        yield return (nameof(s.WorkRevision), s with { WorkRevision = s.WorkRevision + 1 });
        yield return (nameof(s.RngModelVersion), s with { RngModelVersion = s.RngModelVersion + 1 });
        yield return (nameof(s.GrowthModelVersion), s with { GrowthModelVersion = s.GrowthModelVersion + 1 });
        yield return (nameof(s.P1AlgorithmVersion), s with { P1AlgorithmVersion = s.P1AlgorithmVersion + "x" });
        yield return (nameof(s.AreaModelVersion), s with { AreaModelVersion = s.AreaModelVersion + "x" });
        yield return (nameof(s.ActionEligibleDbhThresholdMm), s with { ActionEligibleDbhThresholdMm = s.ActionEligibleDbhThresholdMm + 1 });
    }

    private static IEnumerable<(string What, Func<MultiAreaCommandResult> Call)> AllCommands(MultiAreaWorkPlanAdapter player,
        MultiAreaFence fence, string treeId, MultiAreaApprovedPlan plan)
    {
        yield return ("MarkCrop", () => player.MarkCrop(fence, A, treeId));
        yield return ("UnmarkCrop", () => player.UnmarkCrop(fence, A, treeId));
        yield return ("AddPending", () => player.AddPending(fence, A, treeId, "fenced probe"));
        yield return ("RemovePending", () => player.RemovePending(fence, A, treeId));
        yield return ("Approve", () => player.Approve(fence));
        yield return ("CancelApproved", () => player.CancelApproved(fence));
        yield return ("Execute", () => player.Execute(fence, plan.PlanId, plan.ExecutionKey));
        yield return ("Advance", () => player.Advance(fence, true));
    }

    // Every invariant one atomic read must satisfy. Any failure means parts of the view came from different commits.
    private static IEnumerable<string> TornViewProblems(MultiAreaSessionView view)
    {
        var source = view.Source;
        if (view.Events.Count != source.WorkRevision) yield return "event count differs from the work revision";
        if (view.AnnualHistory.Count != source.AnnualRevision) yield return "history length differs from the annual revision";
        if (view.Approved is not null && (view.Pending.Count != 0 || view.Approved.Source != source))
            yield return "the approved plan is not bound to this view's root";
        foreach (var area in view.Areas)
        {
            var felled = area.Trees.Where(t => t.Death?.Cause == FellCause.Harvest).Select(t => t.TreeId).Order(StringComparer.Ordinal);
            var recorded = view.Events.SelectMany(ev => ev.Areas.Where(change => change.AreaId == area.AreaId).SelectMany(change => change.Fells))
                .Select(f => f.TreeId).Order(StringComparer.Ordinal);
            if (!felled.SequenceEqual(recorded)) yield return area.AreaId + ": felled roster and event ledger differ";
            if (area.Stand != WorkRules.Summarize(area.Trees, area.AreaHa)) yield return area.AreaId + ": stand differs from its roster";
            if (area.Trees.Any(t => t.Death is { } death && death.EcologicalYear > source.EcologicalYear))
                yield return area.AreaId + ": a death is later than the view's year";
        }
        if (view.Events.Count > 0 && view.Events[^1].Areas.Select(change => change.AreaId).Count() != view.Areas.Count)
            yield return "an event does not report every area";
    }
}

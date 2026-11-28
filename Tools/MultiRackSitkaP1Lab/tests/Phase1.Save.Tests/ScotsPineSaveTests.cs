using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;
using SitkaP1.Core.Saves;
using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Phase1Save.Tests;

/// <summary>
/// Scots pine and site moisture, Part 2: Version 3 save round trip (pine trees, planted and natural juveniles, shelters, pine
/// deadwood, pending pine work and the moisture configuration), old saves defaulting to NeutralLegacy, and fail-closed
/// refusals for a tampered <c>scotsPine</c> block. The inner v2 payload stays the frozen Phase 1 format (SchemaVersion 2).
/// </summary>
internal static class ScotsPineSaveTests
{
    private const string Pine = ScotsPineLive.SpeciesId;
    private static readonly FieldInfo RootField =
        typeof(MultiAreaContinuation).GetField("root", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static MultiAreaRoot Root(MultiAreaContinuation c) => (MultiAreaRoot)RootField.GetValue(c)!;
    private static MultiAreaFence FenceOf(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);
    private static void Advance(MultiAreaContinuation c) => c.Advance(FenceOf(c.ReadView()), consentToCancelWork: true);

    internal static void Register(Func<string> newSlotDir)
    {
        T.Add("SP2-S1 v3 Scots pine: Save -> new process slots -> RestoreFromV3 keeps pine trees, juveniles, shelters, deadwood, orders and the two-zone site", () =>
        {
            var (session, felledPine) = BuildPineWoodland();
            var before = Root(session);
            T.True(before.Stage4!.Deadwood.Any(d => d.TreeId == felledPine && d.SpeciesId == Pine), "felled pine left as Scots pine deadwood");
            T.True(before.Phase2!.Juveniles.Any(j => j.SpeciesId == Pine), "planted pine juveniles exist");
            T.True(before.Phase2.PendingOrders.Any(o => o is MultiAreaPlantOrder { SpeciesId: Pine }), "a pending pine order exists");

            string slotDir = newSlotDir();
            var view = session.ReadView();
            var (schema, bytes) = session.CaptureSaveBytes(FenceOf(view), "Scots pine v3", 1);
            T.Eq(3, schema, "saved as schema v3");
            var commit = new SaveV3Slots(new Phase1SaveSlots(slotDir)).Save("scots-pine-v3", bytes, replaceExisting: false);
            T.Eq(Phase1SlotCommitStatus.DurablyCommitted, commit.Status, "slot committed");

            // The inner payload is the frozen v2 format and names no Scots pine anywhere.
            var decoded = SaveV3Codec.Decode(bytes);
            T.True(decoded.Envelope.Blocks.ContainsKey(SaveV3Format.ScotsPineBlockKey), "scotsPine block written");
            T.True(decoded.Envelope.State.Phase2!.TreeSpecies.All(s => s.SpeciesId != Pine) &&
                   decoded.Envelope.State.Phase2.Juveniles.All(j => j.SpeciesId != Pine) &&
                   decoded.Envelope.State.Stage4!.Deadwood.All(d => d.SpeciesId != Pine), "v2 payload is the pine-free projection");

            var loaded = new SaveV3Slots(new Phase1SaveSlots(slotDir)).Load("scots-pine-v3", v2Expectation: null);
            var (restored, _, result) = MultiAreaContinuation.RestoreFromV3(loaded.V3!);
            T.True(result.RefusalCode is null, "RestoreFromV3 succeeded");
            var after = Root(restored);
            T.True(after.Phase2!.TreeSpecies.OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .SequenceEqual(before.Phase2.TreeSpecies.OrderBy(kv => kv.Key, StringComparer.Ordinal)), "species map identical (pine included)");
            T.True(after.Phase2.Juveniles.SequenceEqual(before.Phase2.Juveniles), "every juvenile identical and in order");
            T.True(after.Phase2.Shelters.SequenceEqual(before.Phase2.Shelters), "shelters identical");
            T.True(after.Phase2.WorkLog.SequenceEqual(before.Phase2.WorkLog), "planting work log identical");
            T.True(after.Phase2.PendingOrders.ToHashSet().SetEquals(before.Phase2.PendingOrders), "pending orders (pine included) restored");
            T.True(after.Stage4!.Deadwood.SequenceEqual(before.Stage4.Deadwood), "deadwood identical (Scots pine species restored)");
            T.Eq(before.ScotsPine!.Site, after.ScotsPine!.Site, "two-zone site restored");
            T.True(after.ScotsPine.Recruits.SequenceEqual(before.ScotsPine.Recruits), "natural recruits identical");
            T.True(after.ScotsPine.Accounts.SequenceEqual(before.ScotsPine.Accounts), "annual accounts identical");
            T.Eq(SiteMoistureLabels.TwoZoneLabel, restored.ReadScotsPineView().Site.WholeStandLabel, "label survives");

            // Re-saving the restored woodland gives the same canonical block, and the next year is identical in both.
            var (_, resaved) = restored.CaptureSaveBytes(FenceOf(result.View), "Scots pine v3", 2);
            T.Eq(decoded.Envelope.Blocks[SaveV3Format.ScotsPineBlockKey].CanonicalDataJson,
                SaveV3Codec.Decode(resaved).Envelope.Blocks[SaveV3Format.ScotsPineBlockKey].CanonicalDataJson, "canonical scotsPine block");
            Advance(session);
            Advance(restored);
            T.True(Root(session).Areas.SelectMany(a => a.State.Trees).SequenceEqual(Root(restored).Areas.SelectMany(a => a.State.Trees)),
                "the year after load is identical to the year without a load");
            T.True(Root(session).Phase2!.Juveniles.SequenceEqual(Root(restored).Phase2!.Juveniles), "juvenile year identical");
        });

        T.Add("SP2-S2 v3 site moisture: a whole-woodland class round-trips; a woodland without one writes no block and restores NeutralLegacy", () =>
        {
            var (_, dry) = ForestMakerGenerator.CreateSession(ForestMakerPresets.PureSitka064,
                siteMoisture: SiteMoistureSetting.Whole(SiteMoistureClass.StagnantPoorBog));
            Advance(dry);
            var dv = dry.ReadView();
            var (_, dryBytes) = dry.CaptureSaveBytes(FenceOf(dv), "bog", 1);
            var (dryRestored, _, _) = MultiAreaContinuation.RestoreFromV3(SaveV3Codec.Decode(dryBytes));
            var site = dryRestored.ReadScotsPineView().Site;
            T.True(site.WholeClass == "StagnantPoorBog" && site.HabitatCaution, "bog class and caution restored");

            var (_, plain) = ForestMakerGenerator.CreateSession(ForestMakerPresets.SitkaEdgeBroadleaves064);
            Advance(plain);
            var pv = plain.ReadView();
            var (_, plainBytes) = plain.CaptureSaveBytes(FenceOf(pv), "plain", 1);
            var plainDecoded = SaveV3Codec.Decode(plainBytes);
            T.True(!plainDecoded.Envelope.Blocks.ContainsKey(SaveV3Format.ScotsPineBlockKey), "no scotsPine block on an existing woodland");
            var (plainRestored, _, _) = MultiAreaContinuation.RestoreFromV3(plainDecoded);
            T.True(Root(plainRestored).ScotsPine is null && plainRestored.ReadScotsPineView().Site.IsNeutral, "absent block = NeutralLegacy");

            // Absent block on the two-zone preset (stripped and resealed): never a refusal; the recipe's own genesis site and pine.
            var (_, twoZone) = ForestMakerGenerator.CreateSession(ForestMakerPresets.ScotsPineTwoZone064);
            var tv = twoZone.ReadView();
            var (_, tzBytes) = twoZone.CaptureSaveBytes(FenceOf(tv), "tz", 1);
            var stripped = ResealV3(RemoveBlock(Encoding.UTF8.GetString(tzBytes), SaveV3Format.ScotsPineBlockKey));
            var (tzRestored, _, tzResult) = MultiAreaContinuation.RestoreFromV3(SaveV3Codec.Decode(stripped));
            T.True(tzResult.RefusalCode is null && tzRestored.ReadScotsPineView().Site.Mode == "DryRidgeMoistSlope" &&
                   tzRestored.ReadScotsPineView().PineTreeIds.SequenceEqual(twoZone.ReadScotsPineView().PineTreeIds),
                "absent block on the two-zone preset defaults to its genesis site and pine");
        });

        T.Add("SP2-S3 v2 compatibility: old Version 2 saves keep loading and carry no Scots pine or moisture state (NeutralLegacy)", () =>
        {
            var (mixedExp, mixedP2, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(mixedExp, MultiAreaModelSelection.Phase2, null, null, mixedP2);
            Advance(session);
            var v = session.ReadView();
            var (schema, bytes) = session.CaptureSaveBytes(FenceOf(v), "v2 mixed", 1);
            T.Eq(2, schema, "built-in stand saves as Version 2");
            var decoded = Phase1SaveCodec.Decode(bytes, session.SaveExpectation());
            var fresh = new MultiAreaContinuation(mixedExp, MultiAreaModelSelection.Phase2, null, null, mixedP2);
            var load = fresh.Restore(FenceOf(fresh.ReadView()), decoded);
            T.True(load.RefusalCode is null && Root(fresh).ScotsPine is null, "v2 load: no Scots pine state");
            T.True(fresh.ReadScotsPineView().Site.IsNeutral && fresh.ReadScotsPineView().Site.WholeStandLabel.StartsWith("Neutral", StringComparison.Ordinal),
                "v2 load defaults to NeutralLegacy");
        });

        T.Add("SP2-S4 refuse: tampered scotsPine block (bad version, unknown/missing member, bad hash length, hash mismatch, bad class, bad index)", () =>
        {
            var (session, _) = BuildPineWoodland();
            var v = session.ReadView();
            var (_, bytes) = session.CaptureSaveBytes(FenceOf(v), "tamper", 1);
            string json = Encoding.UTF8.GetString(bytes);
            string block = SaveV3Codec.Decode(bytes).Envelope.Blocks[SaveV3Format.ScotsPineBlockKey].CanonicalDataJson;

            T.Refuses("SaveBlockVersionUnsupported", () => SaveV3Codec.Decode(ResealV3(json.Replace(
                "\"scotsPine\":{\"blockVersion\":1", "\"scotsPine\":{\"blockVersion\":2", StringComparison.Ordinal))));
            T.Refuses("UnknownKey", () => SaveV3Codec.Decode(ResealV3(json.Replace(block, block.Replace("{\"site\":", "{\"extra\":1,\"site\":",
                StringComparison.Ordinal), StringComparison.Ordinal))));
            T.Refuses("MissingKey", () => SaveV3Codec.Decode(ResealV3(json.Replace(block, block.Replace(",\"nextRecruitNumber\":",
                ",\"nextRecruitNumberX\":", StringComparison.Ordinal), StringComparison.Ordinal))));
            string hash = block[^66..^2];
            T.Refuses("SaveScotsPineHashInvalid", () => SaveV3Codec.Decode(ResealV3(json.Replace(block,
                block.Replace(hash, hash[..63], StringComparison.Ordinal), StringComparison.Ordinal))));
            T.Refuses("SaveScotsPineHashMismatch", () => SaveV3Codec.Decode(ResealV3(json.Replace(block,
                block.Replace("\"wholeClass\":\"NeutralLegacy\"", "\"wholeClass\":\"DryFreeDraining\"", StringComparison.Ordinal)
                    .Replace("\"mode\":\"DryRidgeMoistSlope\"", "\"mode\":\"Whole\"", StringComparison.Ordinal), StringComparison.Ordinal))));
            T.Refuses("SiteMoistureInvalid", () => SaveV3Codec.Decode(ResealV3(json.Replace(block,
                block.Replace("\"wholeClass\":\"NeutralLegacy\"", "\"wholeClass\":\"Swamp\"", StringComparison.Ordinal), StringComparison.Ordinal))));
            // A resealed edit with a recomputed inner hash still fails the consistency rules (a pine on an unknown tree).
            string forged = Rehash(block.Replace("\"pineTreeIds\":[\"", "\"pineTreeIds\":[\"ZZ-NOT-A-TREE\",\"", StringComparison.Ordinal));
            T.Refuses("SaveNotCanonical", () => SaveV3Codec.Decode(ResealV3(json.Replace(block, forged, StringComparison.Ordinal))));
            string unknownTree = Rehash(block.Replace("\"pineTreeIds\":[\"", "\"pineTreeIds\":[\"MA-A-0000\",\"", StringComparison.Ordinal));
            T.Refuses("SaveInconsistent", () => SaveV3Codec.Decode(ResealV3(json.Replace(block, unknownTree, StringComparison.Ordinal))));
            // Unchanged bytes but an edited checksum: the outer envelope still refuses.
            T.Refuses("SaveChecksumMismatch", () => SaveV3Codec.Decode(Encoding.UTF8.GetBytes(json.Replace("\"tamper\"", "\"tampeX\"",
                StringComparison.Ordinal))));
        });

        // Independent review follow-up (repair cycle 1): the only approved order is a Scots pine Plant, which the v2 projection
        // strips, so the frozen v2 payload must not carry an empty approval.
        T.Add("SP2-S5 v3 approved pine-only plan: Approve -> Save before Execute -> Load gives one Pending draft that needs a fresh approval", () =>
        {
            var (_, c) = ForestMakerGenerator.CreateSession(ForestMakerPresets.ScotsPineTwoZone064);
            var (x, z) = PlantingPoint(c, "MA-E", 0);
            var v = c.AddPlant(FenceOf(c.ReadView()), "MA-E", Pine, x, z).View;
            var order = (MultiAreaPlantOrder)Root(c).Phase2!.PendingOrders.Single();
            v = c.Approve(FenceOf(v)).View;
            var oldPlan = v.Approved!;
            T.True(Root(c).Phase2!.ApprovedOrders.SequenceEqual(new MultiAreaPlantingOrder[] { order }), "the pine Plant is the whole approved plan");

            var (schema, bytes) = c.CaptureSaveBytes(FenceOf(v), "approved pine", 1);
            T.Eq(3, schema, "saved as schema v3");
            var decoded = SaveV3Codec.Decode(bytes);
            T.True(decoded.Envelope.State.Approved is null && decoded.Envelope.State.Phase2!.ApprovedOrders.Count == 0 &&
                   decoded.Envelope.State.Phase2.PendingOrders.Count == 0, "inner v2 payload has no approval marker and no orders");

            var (restored, _, result) = MultiAreaContinuation.RestoreFromV3(decoded);
            T.True(result.RefusalCode is null, "RestoreFromV3 succeeded");
            var after = Root(restored);
            T.True(after.Approved is null && result.View.Approved is null, "the approval is not restored (load policy A)");
            T.True(after.Phase2!.ApprovedOrders.Count == 0 &&
                   after.Phase2.PendingOrders.SequenceEqual(new MultiAreaPlantingOrder[] { order }),
                "the pine Plant is back as a Pending draft (same order ID, area, species and point)");
            T.Eq(1, result.DraftOrderCount, "one formerly approved order restored as a draft");

            // The pre-save plan cannot execute; a fresh approval under the new session can.
            T.Refuses("WorkPlanNotApproved", () => restored.Execute(FenceOf(result.View), oldPlan.PlanId, oldPlan.ExecutionKey));
            var reapproved = restored.Approve(FenceOf(restored.ReadView())).View.Approved!;
            T.True(reapproved.PlanId != oldPlan.PlanId, "a fresh approval mints a new plan");

            // Re-saving the restored (unapproved) plan works, and its scotsPine block is canonical across another round trip.
            var (again, _, _) = MultiAreaContinuation.RestoreFromV3(decoded);
            var (resaveSchema, resaved) = again.CaptureSaveBytes(FenceOf(again.ReadView()), "approved pine", 2);
            T.Eq(3, resaveSchema, "re-saved as schema v3");
            var resavedDecoded = SaveV3Codec.Decode(resaved);
            var (twice, _, twiceResult) = MultiAreaContinuation.RestoreFromV3(resavedDecoded);
            T.True(twiceResult.RefusalCode is null && Root(twice).Phase2!.PendingOrders.SequenceEqual(new MultiAreaPlantingOrder[] { order }),
                "the re-saved plan restores the same Pending draft");
            T.Eq(0, twiceResult.DraftOrderCount, "a pending (unapproved) plan restores no formerly approved drafts");
            var (_, resavedTwice) = twice.CaptureSaveBytes(FenceOf(twice.ReadView()), "approved pine", 2);
            T.Eq(resavedDecoded.Envelope.Blocks[SaveV3Format.ScotsPineBlockKey].CanonicalDataJson,
                SaveV3Codec.Decode(resavedTwice).Envelope.Blocks[SaveV3Format.ScotsPineBlockKey].CanonicalDataJson, "canonical scotsPine block");
        });

        T.Add("SP2-S6 v3 approved pine Plant + Shelter: Save before Execute -> Load gives both orders as Pending drafts and counts 2", () =>
        {
            var (_, c) = ForestMakerGenerator.CreateSession(ForestMakerPresets.ScotsPineTwoZone064);
            var (x, z) = PlantingPoint(c, "MA-E", 0);
            var v = c.AddPlant(FenceOf(c.ReadView()), "MA-E", Pine, x, z).View;
            v = c.AddShelter(FenceOf(v), "MA-E", x, z).View;
            var orders = Root(c).Phase2!.PendingOrders.ToArray();
            v = c.Approve(FenceOf(v)).View;

            var (schema, bytes) = c.CaptureSaveBytes(FenceOf(v), "approved pine + shelter", 1);
            T.Eq(3, schema, "saved as schema v3");
            var decoded = SaveV3Codec.Decode(bytes);
            T.True(decoded.Envelope.State.Approved is not null && decoded.Envelope.State.Phase2!.ApprovedOrders.Count == 1,
                "the Shelter keeps the inner v2 approval non-empty");

            var (restored, _, result) = MultiAreaContinuation.RestoreFromV3(decoded);
            T.True(result.RefusalCode is null, "RestoreFromV3 succeeded");
            var after = Root(restored);
            T.True(after.Approved is null && after.Phase2!.ApprovedOrders.Count == 0, "the approval is not restored (load policy A)");
            T.True(after.Phase2!.PendingOrders.Count == 2 && after.Phase2.PendingOrders.ToHashSet().SetEquals(orders),
                "both orders are back as Pending drafts");
            T.True(after.Phase2.PendingOrders.Any(o => o is MultiAreaPlantOrder { SpeciesId: Pine }) &&
                   after.Phase2.PendingOrders.Any(o => o is MultiAreaShelterOrder), "a pine Plant and a Shelter");
            T.Eq(2, result.DraftOrderCount, "two formerly approved orders restored as drafts");
        });
    }

    /// <summary>
    /// The two-zone preset with Economy: plant two Scots pine (one sheltered with a spot clear), fell a pine as deadwood,
    /// advance three years, then leave a pending pine order. Returns the session and the felled pine's ID.
    /// </summary>
    private static (MultiAreaContinuation Session, string FelledPine) BuildPineWoodland()
    {
        var (_, c) = ForestMakerGenerator.CreateSession(ForestMakerPresets.ScotsPineTwoZone064, MultiAreaModelSelection.Phase2WithEconomy);
        var root = Root(c);
        var felled = root.Areas[0].State.Trees.First(t => t.IsLiving && root.Phase2!.SpeciesIdOf(t.TreeId) == Pine);
        var (x1, z1) = PlantingPoint(c, "MA-E", 0);
        var (x2, z2) = PlantingPoint(c, "MA-E", 1);
        var v = c.AddPlant(FenceOf(c.ReadView()), "MA-E", Pine, x1, z1).View;
        v = c.AddShelter(FenceOf(v), "MA-E", x1, z1).View;
        v = c.AddSpotClear(FenceOf(v), "MA-E", x1, z1).View;
        v = c.AddPlant(FenceOf(v), "MA-E", Pine, x2, z2).View;
        v = c.AddPending(FenceOf(v), "MA-A", felled.TreeId, "pine deadwood").View;
        v = c.SetFellingDisposition(FenceOf(v), "MA-A", felled.TreeId, FellingDisposition.LeaveAsDeadwood).View;
        v = c.Approve(FenceOf(v)).View;
        var exec = c.Execute(FenceOf(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
        T.True(exec.RefusalCode is null, "pine plan executed: " + exec.RefusalCode);
        for (int y = 0; y < 3; y++) Advance(c);
        var (x3, z3) = PlantingPoint(c, "MA-D", 0);
        c.AddPlant(FenceOf(c.ReadView()), "MA-D", Pine, x3, z3);
        return (c, felled.TreeId);
    }

    // The n-th open point (0.25 m scan) at least 1.2 m from every living adult and every other planting.
    private static (float X, float Z) PlantingPoint(MultiAreaContinuation c, string areaId, int nth)
    {
        var v = c.ReadView();
        var area = v.Areas.Single(a => a.AreaId == areaId);
        var adults = v.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
        var locate = Phase2Annual.Locator(Root(c).Stage5!);
        double minX = area.Polygon.Min(p => p.XM), maxX = area.Polygon.Max(p => p.XM);
        double minZ = area.Polygon.Min(p => p.ZM), maxZ = area.Polygon.Max(p => p.ZM);
        int found = 0;
        var taken = new List<(float, float)>();
        for (double z = Math.Ceiling(minZ) + 0.125; z < maxZ; z += 0.25)
            for (double x = Math.Ceiling(minX) + 0.125; x < maxX; x += 0.25)
            {
                float fx = (float)x, fz = (float)z;
                if (!Stage5Promotion.StrictlyInside(area.Polygon, fx, fz) || locate(fx, fz) is null) continue;
                if (adults.Any(t => (t.XM - fx) * (t.XM - fx) + (t.ZM - fz) * (t.ZM - fz) < 1.44f)) continue;
                if (taken.Any(p => (p.Item1 - fx) * (p.Item1 - fx) + (p.Item2 - fz) * (p.Item2 - fz) < 4f)) continue;
                if (found++ == nth) return (fx, fz);
                taken.Add((fx, fz));
            }
        throw new Exception("no planting point in " + areaId);
    }

    private static string RemoveBlock(string json, string key)
    {
        int start = json.IndexOf(",\"" + key + "\":{", StringComparison.Ordinal);
        if (start < 0) throw new Exception("block not found: " + key);
        int depth = 0, i = json.IndexOf('{', start);
        for (; i < json.Length; i++)
        {
            if (json[i] == '{') depth++;
            else if (json[i] == '}' && --depth == 0) break;
        }
        return json[..start] + json[(i + 1)..];
    }

    // Re-signs an edited v3 envelope: sha256 over every byte before the checksum object.
    private static byte[] ResealV3(string json)
    {
        const string c = ",\"checksum\":{";
        int ci = json.LastIndexOf(c, StringComparison.Ordinal);
        string sum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json[..ci]))).ToLowerInvariant();
        const string v = "\"value\":\"";
        int vi = json.IndexOf(v, ci, StringComparison.Ordinal) + v.Length;
        return Encoding.UTF8.GetBytes(json[..vi] + sum + json[(vi + 64)..]);
    }

    // Recomputes the inner contentHash of an edited block body.
    private static string Rehash(string block)
    {
        int hi = block.LastIndexOf(",\"contentHash\":\"", StringComparison.Ordinal);
        string body = block[..hi] + "}";
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(body))).ToLowerInvariant();
        return block[..hi] + ",\"contentHash\":\"" + hash + "\"}";
    }
}

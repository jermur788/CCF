using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core;

namespace SitkaP1.Core.Saves;

/// <summary>
/// Everything about Scots pine and site moisture that the frozen Save v2 payload cannot name (its validator only knows Sitka,
/// Sessile Oak and Beech). Each list item keeps its exact position in the live list so a load rebuilds the same order.
/// </summary>
internal sealed record ScotsPineSaveParts(
    SiteMoistureSetting Site,
    IReadOnlyList<string> PineTreeIds,
    IReadOnlyList<string> TightEarlyStockingTreeIds,
    IReadOnlyList<string> PineDeadwoodIds,
    IReadOnlyList<(int Index, PlantedJuvenile Item)> Juveniles,
    IReadOnlyList<(int Index, BrowseShelter Item)> Shelters,
    IReadOnlyList<(int Index, Phase2WorkRecord Item)> WorkLog,
    IReadOnlyList<(int Index, MultiAreaPlantOrder Item)> PendingOrders,
    IReadOnlyList<(int Index, MultiAreaPlantOrder Item)> ApprovedOrders,
    IReadOnlyList<ScotsPineRecruit> Recruits,
    int NextRecruitNumber,
    IReadOnlyList<ScotsPineYearAccount> Accounts);

/// <summary>
/// The v2 projection of a Scots pine root and its inverse. The projection removes every Scots pine item from the Phase 2 lists
/// and the species map, and names pine deadwood as Sitka, so the inner v2 header + payload pass the frozen v2 validator
/// unchanged (src/Core/Phase1/** stays byte-identical). <see cref="Extract"/> returns exactly what was removed.
/// </summary>
internal static class ScotsPineSaveProjection
{
    internal static bool IsPineShelter(BrowseShelter shelter, ISet<string> pineJuveniles) => pineJuveniles.Contains(shelter.JuvenileId);

    internal static bool IsPineOrder(MultiAreaPlantingOrder order) =>
        order is MultiAreaPlantOrder plant && ScotsPineLive.IsPine(plant.SpeciesId);

    /// <summary>The root as the frozen v2 payload sees it (identity when the root has no Scots pine state).</summary>
    internal static MultiAreaRoot ForV2(MultiAreaRoot root)
    {
        if (root.ScotsPine is null) return root;
        var phase2 = root.Phase2;
        var stage4 = root.Stage4;
        if (phase2 is not null)
        {
            var pineJuveniles = phase2.Juveniles.Where(j => ScotsPineLive.IsPine(j.SpeciesId)).Select(j => j.JuvenileId)
                .ToHashSet(StringComparer.Ordinal);
            phase2 = phase2 with
            {
                TreeSpecies = phase2.TreeSpecies.Where(kv => !ScotsPineLive.IsPine(kv.Value))
                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal),
                Juveniles = Array.AsReadOnly(phase2.Juveniles.Where(j => !ScotsPineLive.IsPine(j.SpeciesId)).ToArray()),
                Shelters = Array.AsReadOnly(phase2.Shelters.Where(s => !IsPineShelter(s, pineJuveniles)).ToArray()),
                WorkLog = Array.AsReadOnly(phase2.WorkLog.Where(r => !ScotsPineLive.IsPine(r.SpeciesId)).ToArray()),
                PendingOrders = Array.AsReadOnly(phase2.PendingOrders.Where(o => !IsPineOrder(o)).ToArray()),
                ApprovedOrders = Array.AsReadOnly(phase2.ApprovedOrders.Where(o => !IsPineOrder(o)).ToArray()),
            };
        }
        if (stage4 is not null)
            stage4 = stage4 with
            {
                Deadwood = Array.AsReadOnly(stage4.Deadwood.Select(d => ScotsPineLive.IsPine(d.SpeciesId)
                    ? d with { SpeciesId = Phase2Species.SitkaId } : d).ToArray()),
            };
        return root with { Phase2 = phase2, Stage4 = stage4, ScotsPine = null };
    }

    /// <summary>The Scots pine parts the projection removed, with their live positions.</summary>
    internal static ScotsPineSaveParts Extract(MultiAreaRoot root)
    {
        var state = root.ScotsPine ?? throw new P1Refusal("SaveRequiredBlockMissing", "cannot encode scotsPine block: root has no Scots pine state");
        var phase2 = root.Phase2 ?? throw new P1Refusal("SaveInconsistent", "a Scots pine woodland needs Phase 2 state");
        var pineJuveniles = phase2.Juveniles.Where(j => ScotsPineLive.IsPine(j.SpeciesId)).Select(j => j.JuvenileId)
            .ToHashSet(StringComparer.Ordinal);
        static IReadOnlyList<(int, T)> Indexed<T>(IReadOnlyList<T> list, Func<T, bool> keep) =>
            Array.AsReadOnly(list.Select((item, i) => (i, item)).Where(pair => keep(pair.item)).ToArray());
        return new ScotsPineSaveParts(
            state.Site,
            Array.AsReadOnly(phase2.TreeSpecies.Where(kv => ScotsPineLive.IsPine(kv.Value)).Select(kv => kv.Key)
                .Order(StringComparer.Ordinal).ToArray()),
            state.TightEarlyStockingTreeIds,
            Array.AsReadOnly((root.Stage4?.Deadwood ?? Array.Empty<FallenDeadwoodRecord>()).Where(d => ScotsPineLive.IsPine(d.SpeciesId))
                .Select(d => d.DeadwoodId).ToArray()),
            Indexed(phase2.Juveniles, j => ScotsPineLive.IsPine(j.SpeciesId)),
            Indexed(phase2.Shelters, s => IsPineShelter(s, pineJuveniles)),
            Indexed(phase2.WorkLog, r => ScotsPineLive.IsPine(r.SpeciesId)),
            Array.AsReadOnly(phase2.PendingOrders.Select((o, i) => (i, o)).Where(p => IsPineOrder(p.o))
                .Select(p => (p.i, (MultiAreaPlantOrder)p.o)).ToArray()),
            Array.AsReadOnly(phase2.ApprovedOrders.Select((o, i) => (i, o)).Where(p => IsPineOrder(p.o))
                .Select(p => (p.i, (MultiAreaPlantOrder)p.o)).ToArray()),
            state.Recruits,
            state.NextRecruitNumber,
            state.Accounts);
    }

    /// <summary>
    /// Rebuilds the live root from a restored v2 root and the decoded parts, refusing anything inconsistent. Saved approved pine
    /// orders come back as Pending draft orders (owner load policy A: an approval is never restored).
    /// </summary>
    internal static MultiAreaRoot Apply(MultiAreaRoot restored, ScotsPineSaveParts parts)
    {
        var phase2 = restored.Phase2 ?? throw new P1Refusal("SaveInconsistent", "the scotsPine block needs a Phase 2 save");
        if (restored.ScotsPine is not null) throw new P1Refusal("SaveInconsistent", "the scotsPine block was applied twice");
        var treeArea = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var area in restored.Areas)
            foreach (var tree in area.State.Trees)
                treeArea[tree.TreeId] = area.AreaId;
        var areaIds = restored.Areas.Select(a => a.AreaId).ToHashSet(StringComparer.Ordinal);

        // Species map: every pine names a known tree that the v2 payload left as Sitka.
        var species = new Dictionary<string, string>(phase2.TreeSpecies, StringComparer.Ordinal);
        RequireSortedUnique(parts.PineTreeIds, "pineTreeIds");
        foreach (var id in parts.PineTreeIds)
        {
            Rule(treeArea.ContainsKey(id) && !species.ContainsKey(id), "a Scots pine names a known tree with no other species");
            species[id] = ScotsPineLive.SpeciesId;
        }
        RequireSortedUnique(parts.TightEarlyStockingTreeIds, "tightEarlyStockingTreeIds");
        Rule(parts.TightEarlyStockingTreeIds.All(id => species.TryGetValue(id, out var s) && ScotsPineLive.IsPine(s)),
            "tight early stocking names Scots pine only");

        // Deadwood: each pine record exists in the payload as Sitka and belongs to a Scots pine tree.
        var stage4 = restored.Stage4;
        if (parts.PineDeadwoodIds.Count > 0)
        {
            Rule(stage4 is not null, "Scots pine deadwood needs Stage 4 state");
            var pineDeadwood = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in parts.PineDeadwoodIds) Rule(pineDeadwood.Add(id), "Scots pine deadwood IDs are unique");
            var records = stage4!.Deadwood.ToArray();
            int found = 0;
            for (int i = 0; i < records.Length; i++)
            {
                if (!pineDeadwood.Contains(records[i].DeadwoodId)) continue;
                Rule(string.Equals(records[i].SpeciesId, Phase2Species.SitkaId, StringComparison.Ordinal) &&
                     species.TryGetValue(records[i].TreeId, out var sp) && ScotsPineLive.IsPine(sp),
                    "Scots pine deadwood names a Scots pine tree");
                records[i] = records[i] with { SpeciesId = ScotsPineLive.SpeciesId };
                found++;
            }
            Rule(found == pineDeadwood.Count, "every Scots pine deadwood ID names a deadwood record");
            stage4 = stage4 with { Deadwood = Array.AsReadOnly(records) };
        }

        // Planted pine juveniles, their shelters and work records, back in their live positions.
        var juveniles = MergeAt(phase2.Juveniles, parts.Juveniles, "juveniles");
        var juvenileIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var j in juveniles) Rule(juvenileIds.Add(j.JuvenileId), "planted juvenile IDs are unique");
        foreach (var (_, j) in parts.Juveniles)
        {
            Rule(ScotsPineLive.IsPine(j.SpeciesId) && areaIds.Contains(j.AreaId), "a Scots pine juvenile belongs to a known area");
            Rule(SequenceNumber(j.JuvenileId, "PJ") is int n && n < phase2.NextJuvenileNumber, "juvenile IDs are PJnnnn below the next juvenile number");
            Rule(j.PromotedTreeId is null || (species.TryGetValue(j.PromotedTreeId, out var sp) && ScotsPineLive.IsPine(sp)),
                "a promoted Scots pine juvenile names its Scots pine tree");
            Finite(j.XM, j.ZM, j.AgeYears, j.HeightM, j.LastBrowseProbability, j.LastVegetationLossProbability);
        }
        var pineJuvenileIds = parts.Juveniles.Select(p => p.Item.JuvenileId).ToHashSet(StringComparer.Ordinal);
        var shelters = MergeAt(phase2.Shelters, parts.Shelters, "shelters");
        var shelterIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var s in shelters) Rule(shelterIds.Add(s.ShelterId), "shelter IDs are unique");
        foreach (var (_, s) in parts.Shelters)
        {
            Rule(pineJuvenileIds.Contains(s.JuvenileId), "a Scots pine shelter names a Scots pine juvenile");
            Finite(s.XM, s.ZM);
        }
        var workLog = MergeAt(phase2.WorkLog, parts.WorkLog, "workLog");
        foreach (var (_, r) in parts.WorkLog)
        {
            Rule(r.Kind is "Plant" or "Shelter" && ScotsPineLive.IsPine(r.SpeciesId), "a Scots pine work record is a pine Plant or Shelter");
            Rule(areaIds.Contains(r.AreaId) && restored.Executed.ContainsKey(r.ExecutionKey),
                "a Scots pine work record names a known area and an executed key");
            Rule(SequenceNumber(r.OrderId, "P2W") is int n && n < phase2.NextOrderNumber, "a Scots pine work record is numbered");
            Finite(r.XM, r.ZM);
        }
        var pending = MergeAt(phase2.PendingOrders, parts.PendingOrders.Select(p => (p.Index, (MultiAreaPlantingOrder)p.Item)).ToArray(),
            "pendingOrders").ToList();
        // A saved approval never comes back approved: approved pine orders re-enter as Pending drafts after the pending ones.
        pending.AddRange(parts.ApprovedOrders.OrderBy(p => p.Index).Select(p => (MultiAreaPlantingOrder)p.Item));
        var orderIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var o in pending.Concat(phase2.ApprovedOrders)) Rule(orderIds.Add(o.OrderId), "planting order IDs are unique");
        foreach (var (_, o) in parts.PendingOrders.Concat(parts.ApprovedOrders))
        {
            Rule(ScotsPineLive.IsPine(o.SpeciesId) && areaIds.Contains(o.AreaId), "a Scots pine Plant order names a known area");
            Rule(SequenceNumber(o.OrderId, "P2W") is int n && n < phase2.NextOrderNumber, "Scots pine order IDs are numbered");
            Finite(o.XM, o.ZM);
        }

        // Natural recruits and the annual accounts.
        var recruitIds = new HashSet<string>(StringComparer.Ordinal);
        Rule(parts.NextRecruitNumber >= 1, "the next recruit number starts at 1");
        foreach (var r in parts.Recruits)
        {
            Rule(recruitIds.Add(r.RecruitId) && SequenceNumber(r.RecruitId, "SPR") is int n && n < parts.NextRecruitNumber,
                "recruit IDs are unique SPRnnnn below the next recruit number");
            Rule(areaIds.Contains(r.AreaId), "a recruit belongs to a known area");
            Rule(r.PromotedTreeId is null || (species.TryGetValue(r.PromotedTreeId, out var sp) && ScotsPineLive.IsPine(sp)),
                "a promoted recruit names its Scots pine tree");
            Finite(r.XM, r.ZM, r.AgeYears, r.HeightM);
        }
        for (int i = 0; i < parts.Accounts.Count; i++)
        {
            var a = parts.Accounts[i];
            Rule(a.YearAfter == a.YearBefore + 1 && (i == 0 || a.YearBefore == parts.Accounts[i - 1].YearAfter) &&
                 a.YearAfter <= restored.EcologicalYear && a.YearBefore >= 0, "Scots pine accounts are consecutive committed years");
            Rule(a.MatureSeedTrees >= 0 && a.EligibleCells >= 0 && a.Recruited >= 0 && a.EstablishmentDeaths >= 0 && a.JuvenileDeaths >= 0 &&
                 a.Promoted >= 0 && a.AdultsGrowing >= 0 && a.AdultsOutsideDomain >= 0, "Scots pine account counts are non-negative");
        }
        Rule(parts.Accounts.Count == 0 || parts.Accounts[^1].YearAfter == restored.EcologicalYear,
            "the last Scots pine account ends at the saved year");

        var state = new MultiAreaScotsPineState(parts.Site.RequireValid(), parts.TightEarlyStockingTreeIds, parts.Recruits,
            parts.NextRecruitNumber, parts.Accounts);
        return restored with
        {
            Stage4 = stage4,
            Phase2 = phase2 with
            {
                TreeSpecies = species,
                Juveniles = juveniles,
                Shelters = shelters,
                WorkLog = workLog,
                PendingOrders = pending.AsReadOnly(),
            },
            ScotsPine = state,
        };
    }

    /// <summary>
    /// Absent-block rule: a woodland whose regenerated recipe carries Scots pine genesis (the two-zone preset) takes that
    /// genesis site and pine identity; every other woodland stays neutral (no state at all). Never a refusal.
    /// </summary>
    internal static MultiAreaRoot ApplyGenesisDefaults(MultiAreaRoot restored, ScotsPineGenesis? genesis)
    {
        if (genesis is null || restored.Phase2 is null || restored.ScotsPine is not null) return restored;
        var known = restored.Areas.SelectMany(a => a.State.Trees).Select(t => t.TreeId).ToHashSet(StringComparer.Ordinal);
        var species = new Dictionary<string, string>(restored.Phase2.TreeSpecies, StringComparer.Ordinal);
        foreach (var id in genesis.PineTreeIds)
            if (known.Contains(id) && !species.ContainsKey(id)) species[id] = ScotsPineLive.SpeciesId;
        var stage4 = restored.Stage4 is null ? null : restored.Stage4 with
        {
            Deadwood = Array.AsReadOnly(restored.Stage4.Deadwood.Select(d =>
                species.TryGetValue(d.TreeId, out var sp) && ScotsPineLive.IsPine(sp) ? d with { SpeciesId = ScotsPineLive.SpeciesId } : d).ToArray()),
        };
        return restored with
        {
            Phase2 = restored.Phase2 with { TreeSpecies = species },
            Stage4 = stage4,
            ScotsPine = MultiAreaScotsPineState.Create(genesis.Site),
        };
    }

    private static IReadOnlyList<T> MergeAt<T>(IReadOnlyList<T> others, IReadOnlyList<(int Index, T Item)> placed, string what)
    {
        int total = others.Count + placed.Count;
        var result = new T[total];
        var filled = new bool[total];
        int previous = -1;
        foreach (var (index, item) in placed)
        {
            Rule(index > previous && index < total, "scotsPine." + what + " indices are strictly increasing and inside the list");
            previous = index;
            result[index] = item;
            filled[index] = true;
        }
        int next = 0;
        for (int i = 0; i < total; i++)
        {
            if (filled[i]) continue;
            result[i] = others[next++];
        }
        return Array.AsReadOnly(result);
    }

    private static void RequireSortedUnique(IReadOnlyList<string> ids, string what)
    {
        for (int i = 1; i < ids.Count; i++)
            if (string.CompareOrdinal(ids[i - 1], ids[i]) >= 0)
                throw new P1Refusal("SaveNotCanonical", "scotsPine." + what + " must be unique and ordinal-sorted");
    }

    internal static int? SequenceNumber(string id, string prefix)
    {
        if (!id.StartsWith(prefix, StringComparison.Ordinal) || id.Length < prefix.Length + 4) return null;
        var digits = id.AsSpan(prefix.Length);
        foreach (char c in digits) if (c is < '0' or > '9') return null;
        return int.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out int n) && n >= 1 ? n : null;
    }

    private static void Finite(params float[] values)
    {
        foreach (float v in values) Rule(float.IsFinite(v), "Scots pine save values are finite");
    }

    private static void Rule(bool ok, string what)
    {
        if (!ok) throw new P1Refusal("SaveInconsistent", "scotsPine block: " + what);
    }
}

/// <summary>
/// Save v3 feature-block codec for <c>blocks.scotsPine</c> (C-V3 envelope; SPEC-multi-2d-scots-pine-moisture-V1 Part 2).
/// Fail-closed canonical JSON with exact keys, a version gate and an inner <c>contentHash</c> (SHA-256 of the block body
/// without that field, 64 lowercase hex). An absent block means the woodland has no Scots pine / site-moisture state, which
/// is NeutralLegacy (old Version 2 and Version 3 saves load unchanged).
/// </summary>
internal sealed class ScotsPineFeatureBlockCodec : IFeatureBlockCodec
{
    private static readonly string[] TopKeys =
    {
        "site", "pineTreeIds", "tightEarlyStockingTreeIds", "pineDeadwoodIds", "juveniles", "shelters", "workLog",
        "pendingOrders", "approvedOrders", "recruits", "nextRecruitNumber", "accounts", "contentHash",
    };

    private static readonly string[] SiteKeys = { "mode", "wholeClass" };
    private static readonly string[] JuvenileKeys =
    {
        "index", "juvenileId", "areaId", "xM", "zM", "plantingYear", "ageYears", "heightM", "alive", "promotedTreeId", "deathYear",
        "deathCause", "lastYearBrowsed", "lastBrowseProbability", "lastVegetationLossProbability", "lastAssessmentYear",
    };
    private static readonly string[] ShelterKeys = { "index", "shelterId", "juvenileId", "xM", "zM", "installedYear", "effectiveYears", "failedYear" };
    private static readonly string[] WorkKeys =
    {
        "index", "orderId", "kind", "areaId", "xM", "zM", "speciesId", "juvenileId", "shelterId", "patchId", "ecologicalYear",
        "workRevision", "executionKey", "planId",
    };
    private static readonly string[] OrderKeys = { "index", "orderId", "areaId", "speciesId", "xM", "zM" };
    private static readonly string[] RecruitKeys =
    {
        "recruitId", "areaId", "xM", "zM", "establishYear", "ageYears", "heightM", "alive", "promotedTreeId", "deathYear", "deathCause",
        "lastAssessmentYear",
    };
    private static readonly string[] AccountKeys =
    {
        "yearBefore", "yearAfter", "goodSeedYear", "matureSeedTrees", "eligibleCells", "recruited", "establishmentDeaths",
        "juvenileDeaths", "promoted", "adultsGrowing", "adultsOutsideDomain",
    };

    public string BlockKey => SaveV3Format.ScotsPineBlockKey;

    public int CurrentBlockVersion => 1;

    public bool CanRead(int blockVersion) => blockVersion == 1;

    public string Encode(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        return EncodeParts(ScotsPineSaveProjection.Extract(root));
    }

    public MultiAreaRoot Decode(string canonicalBlockJson, int blockVersion, MultiAreaRoot restoredSoFar)
    {
        ArgumentNullException.ThrowIfNull(canonicalBlockJson);
        ArgumentNullException.ThrowIfNull(restoredSoFar);
        return ScotsPineSaveProjection.Apply(restoredSoFar, DecodeParts(canonicalBlockJson, blockVersion));
    }

    internal ScotsPineSaveParts DecodeParts(string canonicalBlockJson, int blockVersion)
    {
        if (!CanRead(blockVersion))
            throw new P1Refusal("SaveBlockVersionUnsupported", $"scotsPine block version {blockVersion} is not supported by this build");
        var node = StrictJson.Parse(Encoding.UTF8.GetBytes(canonicalBlockJson));
        var map = J.Keys(node, "blocks.scotsPine.data", TopKeys);

        string storedHash = J.Str(map["contentHash"], "blocks.scotsPine.data.contentHash");
        if (storedHash.Length != 64 || storedHash.Any(c => c is not ((>= '0' and <= '9') or (>= 'a' and <= 'f'))))
            throw new P1Refusal("SaveScotsPineHashInvalid", "blocks.scotsPine.data.contentHash must be 64 lowercase hex digits");

        var site = J.Keys(map["site"], "scotsPine.site", SiteKeys);
        var setting = new SiteMoistureSetting(SiteMoistureSetting.ParseMode(J.Str(site["mode"], "scotsPine.site.mode")),
            SiteMoistureSetting.ParseClass(J.Str(site["wholeClass"], "scotsPine.site.wholeClass"))).RequireValid();

        var parts = new ScotsPineSaveParts(
            setting,
            Strings(map["pineTreeIds"], "pineTreeIds"),
            Strings(map["tightEarlyStockingTreeIds"], "tightEarlyStockingTreeIds"),
            Strings(map["pineDeadwoodIds"], "pineDeadwoodIds"),
            List(map["juveniles"], "juveniles", JuvenileKeys, m => (I32(m["index"], "index"), new PlantedJuvenile(
                Str(m["juvenileId"]), Str(m["areaId"]), ScotsPineLive.SpeciesId, F(m["xM"], "xM"), F(m["zM"], "zM"),
                I32(m["plantingYear"], "plantingYear"), F(m["ageYears"], "ageYears"), F(m["heightM"], "heightM"),
                Bool(m["alive"], "alive"), StrOrNull(m["promotedTreeId"]), I32(m["deathYear"], "deathYear"), StrOrNull(m["deathCause"]),
                Bool(m["lastYearBrowsed"], "lastYearBrowsed"), F(m["lastBrowseProbability"], "lastBrowseProbability"),
                F(m["lastVegetationLossProbability"], "lastVegetationLossProbability"), I32(m["lastAssessmentYear"], "lastAssessmentYear")))),
            List(map["shelters"], "shelters", ShelterKeys, m => (I32(m["index"], "index"), new BrowseShelter(
                Str(m["shelterId"]), Str(m["juvenileId"]), F(m["xM"], "xM"), F(m["zM"], "zM"), I32(m["installedYear"], "installedYear"),
                I32(m["effectiveYears"], "effectiveYears"), I32(m["failedYear"], "failedYear")))),
            List(map["workLog"], "workLog", WorkKeys, m => (I32(m["index"], "index"), new Phase2WorkRecord(
                Str(m["orderId"]), Str(m["kind"]), Str(m["areaId"]), F(m["xM"], "xM"), F(m["zM"], "zM"), StrOrNull(m["speciesId"]),
                StrOrNull(m["juvenileId"]), StrOrNull(m["shelterId"]), StrOrNull(m["patchId"]), I32(m["ecologicalYear"], "ecologicalYear"),
                I32(m["workRevision"], "workRevision"), Str(m["executionKey"]), Str(m["planId"])))),
            List(map["pendingOrders"], "pendingOrders", OrderKeys, Order),
            List(map["approvedOrders"], "approvedOrders", OrderKeys, Order),
            List(map["recruits"], "recruits", RecruitKeys, m => new ScotsPineRecruit(
                Str(m["recruitId"]), Str(m["areaId"]), F(m["xM"], "xM"), F(m["zM"], "zM"), I32(m["establishYear"], "establishYear"),
                F(m["ageYears"], "ageYears"), F(m["heightM"], "heightM"), Bool(m["alive"], "alive"), StrOrNull(m["promotedTreeId"]),
                I32(m["deathYear"], "deathYear"), StrOrNull(m["deathCause"]), I32(m["lastAssessmentYear"], "lastAssessmentYear"))),
            I32(map["nextRecruitNumber"], "nextRecruitNumber"),
            List(map["accounts"], "accounts", AccountKeys, m => new ScotsPineYearAccount(
                I32(m["yearBefore"], "yearBefore"), I32(m["yearAfter"], "yearAfter"), Bool(m["goodSeedYear"], "goodSeedYear"),
                I32(m["matureSeedTrees"], "matureSeedTrees"), I32(m["eligibleCells"], "eligibleCells"), I32(m["recruited"], "recruited"),
                I32(m["establishmentDeaths"], "establishmentDeaths"), I32(m["juvenileDeaths"], "juvenileDeaths"), I32(m["promoted"], "promoted"),
                I32(m["adultsGrowing"], "adultsGrowing"), I32(m["adultsOutsideDomain"], "adultsOutsideDomain"))));

        string body = EncodeBody(parts);
        if (!string.Equals(Sha256Hex(body), storedHash, StringComparison.Ordinal))
            throw new P1Refusal("SaveScotsPineHashMismatch", "blocks.scotsPine.data.contentHash does not match the block content");
        if (!string.Equals(WithHash(body), canonicalBlockJson, StringComparison.Ordinal))
            throw new P1Refusal("SaveNotCanonical", "blocks.scotsPine.data is not in canonical form");
        return parts;
    }

    internal static string EncodeParts(ScotsPineSaveParts parts) => WithHash(EncodeBody(parts));

    private static string WithHash(string body) => body[..^1] + ",\"contentHash\":\"" + Sha256Hex(body) + "\"}";

    private static string EncodeBody(ScotsPineSaveParts p)
    {
        var w = new CanonicalWriter().BeginObject();
        w.BeginObject("site").Str("mode", SiteMoistureSetting.ModeWire(p.Site.Mode))
            .Str("wholeClass", SiteMoistureSetting.ClassWire(p.Site.WholeClass)).EndObject();
        w.StrArray("pineTreeIds", p.PineTreeIds).StrArray("tightEarlyStockingTreeIds", p.TightEarlyStockingTreeIds)
            .StrArray("pineDeadwoodIds", p.PineDeadwoodIds);
        w.BeginArray("juveniles");
        foreach (var (i, j) in p.Juveniles)
            w.BeginObject().Int("index", i).Str("juvenileId", j.JuvenileId).Str("areaId", j.AreaId).F32("xM", F32.Bits(j.XM))
                .F32("zM", F32.Bits(j.ZM)).Int("plantingYear", j.PlantingYear).F32("ageYears", F32.Bits(j.AgeYears))
                .F32("heightM", F32.Bits(j.HeightM)).Bool("alive", j.Alive).StrOrNull("promotedTreeId", j.PromotedTreeId)
                .Int("deathYear", j.DeathYear).StrOrNull("deathCause", j.DeathCause).Bool("lastYearBrowsed", j.LastYearBrowsed)
                .F32("lastBrowseProbability", F32.Bits(j.LastBrowseProbability))
                .F32("lastVegetationLossProbability", F32.Bits(j.LastVegetationLossProbability))
                .Int("lastAssessmentYear", j.LastAssessmentYear).EndObject();
        w.EndArray().BeginArray("shelters");
        foreach (var (i, s) in p.Shelters)
            w.BeginObject().Int("index", i).Str("shelterId", s.ShelterId).Str("juvenileId", s.JuvenileId).F32("xM", F32.Bits(s.XM))
                .F32("zM", F32.Bits(s.ZM)).Int("installedYear", s.InstalledYear).Int("effectiveYears", s.EffectiveYears)
                .Int("failedYear", s.FailedYear).EndObject();
        w.EndArray().BeginArray("workLog");
        foreach (var (i, r) in p.WorkLog)
            w.BeginObject().Int("index", i).Str("orderId", r.OrderId).Str("kind", r.Kind).Str("areaId", r.AreaId).F32("xM", F32.Bits(r.XM))
                .F32("zM", F32.Bits(r.ZM)).StrOrNull("speciesId", r.SpeciesId).StrOrNull("juvenileId", r.JuvenileId)
                .StrOrNull("shelterId", r.ShelterId).StrOrNull("patchId", r.PatchId).Int("ecologicalYear", r.EcologicalYear)
                .Int("workRevision", r.WorkRevision).Str("executionKey", r.ExecutionKey).Str("planId", r.PlanId).EndObject();
        w.EndArray();
        WriteOrders(w, "pendingOrders", p.PendingOrders);
        WriteOrders(w, "approvedOrders", p.ApprovedOrders);
        w.BeginArray("recruits");
        foreach (var r in p.Recruits)
            w.BeginObject().Str("recruitId", r.RecruitId).Str("areaId", r.AreaId).F32("xM", F32.Bits(r.XM)).F32("zM", F32.Bits(r.ZM))
                .Int("establishYear", r.EstablishYear).F32("ageYears", F32.Bits(r.AgeYears)).F32("heightM", F32.Bits(r.HeightM))
                .Bool("alive", r.Alive).StrOrNull("promotedTreeId", r.PromotedTreeId).Int("deathYear", r.DeathYear)
                .StrOrNull("deathCause", r.DeathCause).Int("lastAssessmentYear", r.LastAssessmentYear).EndObject();
        w.EndArray().Int("nextRecruitNumber", p.NextRecruitNumber).BeginArray("accounts");
        foreach (var a in p.Accounts)
            w.BeginObject().Int("yearBefore", a.YearBefore).Int("yearAfter", a.YearAfter).Bool("goodSeedYear", a.GoodSeedYear)
                .Int("matureSeedTrees", a.MatureSeedTrees).Int("eligibleCells", a.EligibleCells).Int("recruited", a.Recruited)
                .Int("establishmentDeaths", a.EstablishmentDeaths).Int("juvenileDeaths", a.JuvenileDeaths).Int("promoted", a.Promoted)
                .Int("adultsGrowing", a.AdultsGrowing).Int("adultsOutsideDomain", a.AdultsOutsideDomain).EndObject();
        w.EndArray().EndObject();
        return w.ToString();
    }

    private static void WriteOrders(CanonicalWriter w, string key, IReadOnlyList<(int Index, MultiAreaPlantOrder Item)> orders)
    {
        w.BeginArray(key);
        foreach (var (i, o) in orders)
            w.BeginObject().Int("index", i).Str("orderId", o.OrderId).Str("areaId", o.AreaId).Str("speciesId", o.SpeciesId)
                .F32("xM", F32.Bits(o.XM)).F32("zM", F32.Bits(o.ZM)).EndObject();
        w.EndArray();
    }

    private static (int, MultiAreaPlantOrder) Order(Dictionary<string, JNode> m)
    {
        string species = Str(m["speciesId"]);
        if (!ScotsPineLive.IsPine(species))
            throw new P1Refusal("SaveInconsistent", "scotsPine block: a stored order must plant scots-pine");
        return (I32(m["index"], "index"), new MultiAreaPlantOrder(Str(m["orderId"]), Str(m["areaId"]), species, F(m["xM"], "xM"), F(m["zM"], "zM")));
    }

    private static IReadOnlyList<T> List<T>(JNode node, string what, string[] keys, Func<Dictionary<string, JNode>, T> read) =>
        Array.AsReadOnly(J.Arr(node, "scotsPine." + what).Items.Select(item => read(J.Keys(item, "scotsPine." + what + "[]", keys))).ToArray());

    private static IReadOnlyList<string> Strings(JNode node, string what) =>
        Array.AsReadOnly(J.Arr(node, "scotsPine." + what).Items.Select(item => J.Str(item, "scotsPine." + what + "[]")).ToArray());

    private static string Str(JNode n) => J.Str(n, "scotsPine string");

    private static string? StrOrNull(JNode n) => J.StrOrNull(n, "scotsPine string");

    private static float F(JNode n, string what) => F32.FromBits(J.F32Bits(n, "scotsPine." + what));

    private static int I32(JNode n, string what)
    {
        long v = J.Int(n, "scotsPine." + what);
        if (v is < int.MinValue or > int.MaxValue) throw new P1Refusal("NumberOutOfRange", "scotsPine." + what + " is outside the 32-bit range");
        return (int)v;
    }

    private static bool Bool(JNode n, string what) =>
        n is JBool b ? b.Value : throw new P1Refusal("SchemaInvalid", "scotsPine." + what + " must be true or false");

    private static string Sha256Hex(string text) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
}

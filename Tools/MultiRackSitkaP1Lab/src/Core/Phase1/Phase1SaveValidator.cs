using System.Globalization;

namespace SitkaP1.Core.Phase1;

/// <summary>
/// In-memory consistency checks of a whole save, run before encoding and after decoding, before anything could be
/// activated. Each rule is a structural invariant read from the engine source at cf6fe7a5 (cited inline); none is an
/// ecological formula, and none repairs anything. A failure refuses the whole save with code
/// <c>SaveInconsistent</c> (or <c>SaveModelUnsupported</c>) and a message naming the rule.
/// <para>
/// This is NOT a substitute for the engine's own revalidation on restore (A1): pending/approved orders must still be
/// revalidated against the restored root by the engine's own rules, and the approved plan converted to an unapproved
/// draft (owner load policy A).
/// </para>
/// </summary>
public static class Phase1SaveValidator
{
    private const string SitkaId = "sitka-spruce";
    private const string OakId = "sessile-oak";
    private const string BeechId = "beech";

    public static void Validate(Phase1SaveEnvelope save)
    {
        ArgumentNullException.ThrowIfNull(save);
        ArgumentNullException.ThrowIfNull(save.Header);
        ArgumentNullException.ThrowIfNull(save.State);
        ValidateHeader(save.Header);
        ValidateState(save.State, save.Header);
    }

    // ------------------------------------------------------------------------------------------------ header

    private static void ValidateHeader(Phase1SaveHeader h)
    {
        if (string.IsNullOrEmpty(h.Label) || h.Label.Length > Phase1SaveFormat.MaxLabelLength || h.Label.Trim() != h.Label)
            throw new P1Refusal("SaveLabelInvalid", FormattableString.Invariant(
                $"the save label must be 1..{Phase1SaveFormat.MaxLabelLength} printable ASCII characters without leading/trailing spaces"));
        Ascii(h.Label, "SaveLabelInvalid");
        if (!DateTime.TryParseExact(h.SavedUtc, Phase1SaveHeader.SavedUtcFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var parsed) ||
            Phase1SaveHeader.FormatUtc(DateTime.SpecifyKind(parsed, DateTimeKind.Utc)) != h.SavedUtc)
            throw new P1Refusal("SaveTimeInvalid", "the saved time must be exactly yyyy-MM-ddTHH:mm:ssZ (UTC)");
        Rule(h.SaveGeneration >= 1, "the save generation counter starts at 1");
        if (!h.Models.IsSupported)
            throw new P1Refusal("SaveModelUnsupported", "model vector " + h.Models +
                " is not one the engine accepts (0/0/0/0/0, 0/0/1/0/0, 1/1/1/0/0, 1/1/1/1/0, 1/1/1/1/1)");
        var g = h.Genesis;
        Rule(g is not null, "the genesis identity is required");
        Rule(Id(g!.ExperimentId) && Id(g.GeneratorId), "the experiment and generator IDs are required");
        Rule(g.ManifestContentHash.Length == 64 && g.ManifestContentHash.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'),
            "the manifest content hash is 64 lowercase hex digits (DerivedManifest.Sha256Hex)");
        Rule(g.Areas is { Count: > 0 }, "the genesis names at least one area");
        // The engine keeps areas ordinal by AreaId (MultiAreaContinuation.areas comment) and root.Areas in the same order.
        RequireOrdinalUnique(g.Areas.Select(a => a.AreaId).ToArray(), "genesis area IDs");
        foreach (var a in g.Areas)
            Rule(float.IsFinite(a.AreaHa) && a.AreaHa > 0f, "every genesis area has a finite positive area");
    }

    // ------------------------------------------------------------------------------------------------ state

    private static void ValidateState(Phase1SaveState s, Phase1SaveHeader h)
    {
        var m = h.Models;
        Rule(s.EcologicalYear == h.EcologicalYear, "the header year equals the state year");
        Rule(s.EcologicalYear >= 0 && s.PlanStateRevision >= 0, "years and revisions are non-negative");
        // Advance is the only writer of EcologicalYear, AnnualRevision and AnnualHistory, each by exactly one per year.
        Rule(s.AnnualRevision == s.EcologicalYear && s.AnnualHistory.Count == s.EcologicalYear,
            "the year, the annual revision and the annual history length agree (one record per committed year)");
        // Execute is the only writer of WorkRevision, Events and Executed, each by exactly one per execution.
        Rule(s.WorkRevision == s.Events.Count && s.Executed.Count == s.Events.Count,
            "the work revision, the event ledger and the executed-key receipts agree (one each per execution)");

        // Optional parts exist exactly when their model is selected (MultiAreaContinuation constructor).
        Rule((s.Options.Stage4 is not null) == m.IsStage4 && (s.Stage4 is not null) == m.IsStage4,
            "Stage 4 options and state are present exactly for the Stage 4 selections");
        Rule((s.Options.Stage5 is not null) == m.IsStage5 && (s.Stage5 is not null) == m.IsStage5,
            "Stage 5 options and state are present exactly for the 1/1/1/1 selections");
        Rule((s.Options.Phase2 is not null) == m.IsPhase2 && (s.Phase2 is not null) == m.IsPhase2,
            "Phase 2 options and state are present exactly for 1/1/1/1/1");

        string[] areaIds = h.Genesis.Areas.Select(a => a.AreaId).ToArray();
        Rule(s.Areas.Select(a => a.AreaId).SequenceEqual(areaIds, StringComparer.Ordinal),
            "the saved areas are exactly the genesis areas, in the same order");

        // Rosters. Trees are never removed from a roster (dead stems stay), so every recorded ID must resolve to one.
        var treeArea = new Dictionary<string, string>(StringComparer.Ordinal);
        var living = new HashSet<string>(StringComparer.Ordinal);
        foreach (var area in s.Areas)
        {
            Rule(area.EcologicalYear == s.EcologicalYear, "every area roster carries the shared ecological year");
            foreach (var t in area.Trees)
            {
                Rule(Id(t.TreeId), "every tree has an ID");
                Rule(treeArea.TryAdd(t.TreeId, area.AreaId), "tree ID " + t.TreeId + " is unique across all areas");
                Finite(t.XM, t.ZM, t.DbhCm, t.HeightM, t.CrownRadiusM);
                if (t.Death is null) living.Add(t.TreeId);
                else
                {
                    Rule(t.Death.TreeId == t.TreeId, "a death record names its own tree (" + t.TreeId + ")");
                    Rule(t.Death.EcologicalYear >= 0 && t.Death.EcologicalYear <= s.EcologicalYear, "a death is not in the future");
                    Rule(Id(t.Death.Cause) && Id(t.Death.Mechanism), "a death has a cause and a mechanism");
                }
            }
            // Crop marks are sorted ordinal (MarkCrop) and cleared on death (Advance), so each is a living tree of the area.
            RequireOrdinalUnique(area.CropTreeIds, "crop marks of " + area.AreaId);
            foreach (var crop in area.CropTreeIds)
                Rule(living.Contains(crop) && treeArea[crop] == area.AreaId, "crop mark " + crop + " is a living tree of " + area.AreaId);
        }

        // Event ledger and executed-key receipts.
        var eventKeys = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < s.Events.Count; i++)
        {
            var e = s.Events[i];
            Rule(Id(e.ExecutionKey) && Id(e.PlanId), "every event has an execution key and a plan ID");
            Rule(eventKeys.Add(e.ExecutionKey), "event execution keys are unique");
            Rule(e.WorkRevision == i + 1, "events are in work-revision order 1, 2, 3, ...");
            Rule(e.EcologicalYear >= 0 && e.EcologicalYear <= s.EcologicalYear, "an event is not in the future");
            Rule(e.Areas.Select(a => a.AreaId).SequenceEqual(areaIds, StringComparer.Ordinal),
                "every event records every area in area order (Execute builds one change per area)");
            foreach (var change in e.Areas)
            {
                RequireOrdinalUnique(change.Fells.Select(f => f.TreeId).ToArray(), "fells of one area change");
                foreach (var f in change.Fells)
                {
                    Rule(f.AreaId == change.AreaId && f.ExecutionKey == e.ExecutionKey && f.PlanId == e.PlanId &&
                        f.WorkRevision == e.WorkRevision && f.EcologicalYear == e.EcologicalYear,
                        "a fell record carries its event's area, key, plan, revision and year");
                    Rule(treeArea.TryGetValue(f.TreeId, out var at) && at == f.AreaId && !living.Contains(f.TreeId),
                        "a felled tree " + f.TreeId + " is a dead stem of its area");
                    Reason(f.Reason);
                }
            }
        }
        RequireOrdinalUnique(s.Executed.Select(x => x.ExecutionKey).ToArray(), "executed keys");
        foreach (var x in s.Executed)
        {
            Rule(eventKeys.Contains(x.ExecutionKey), "every executed key has its event in the ledger");
            Token(x.RecordedSource);
            Orders(x.Orders, treeArea, requireLiving: null);
        }

        // Work plan. Approve moves every pending list into the approval; nothing is added while a plan is approved.
        Orders(s.Pending, treeArea, living);
        RequireUnique(s.Pending.Select(o => o.TreeId), "pending Fell tree IDs");
        bool hasApproved = s.Approved is not null;
        if (s.Approved is { } plan)
        {
            Rule(Id(plan.PlanId) && Id(plan.ExecutionKey), "an approved plan has a plan ID and an execution key");
            Rule(!eventKeys.Contains(plan.ExecutionKey), "an approved plan's key has not been executed");
            Token(plan.RecordedSource);
            Orders(plan.Orders, treeArea, living);
            RequireUnique(plan.Orders.Select(o => o.TreeId), "approved Fell tree IDs");
            Rule(s.Pending.Count == 0 && (s.Stage4?.PendingPrunes.Count ?? 0) == 0 && (s.Stage5?.PendingClears.Count ?? 0) == 0 &&
                (s.Phase2?.PendingOrders.Count ?? 0) == 0, "no pending order coexists with an approved plan");
        }

        // Annual history.
        for (int i = 0; i < s.AnnualHistory.Count; i++)
        {
            var y = s.AnnualHistory[i];
            Rule(y.YearBefore == i && y.YearAfter == i + 1 && y.AnnualRevisionAfter == i + 1,
                "annual records are consecutive years 0→1, 1→2, ... with matching revisions");
            Rule(y.WorkRevision >= 0 && y.WorkRevision <= s.WorkRevision && (i == 0 || y.WorkRevision >= s.AnnualHistory[i - 1].WorkRevision),
                "annual records carry a non-decreasing work revision no later than the current one");
            Rule(y.Areas.Select(a => a.AreaId).SequenceEqual(areaIds, StringComparer.Ordinal), "every annual record covers every area in order");
            foreach (var a in y.Areas)
            {
                Rule(!a.NoLivingStems || a.NaturalDeaths.Count == 0, "a carried-forward empty area has no natural deaths");
                foreach (var d in a.NaturalDeaths)
                    Rule(Id(d.TreeId) && treeArea.TryGetValue(d.TreeId, out var at) && at == a.AreaId && !living.Contains(d.TreeId) &&
                        d.EcologicalYear >= 0 && d.EcologicalYear <= s.EcologicalYear,
                        "a natural death names a dead tree of its area and is not in the future");
            }
        }

        if (s.Options.Stage4 is { } o4)
        {
            Rule(o4.SiteProductivity == Phase1SaveFormat.SiteProductivityUniform,
                "only the uniform site-productivity field can be saved (any other is code, not data)");
            if (o4.GrowthAgePhases is not null)
                foreach (var p in o4.GrowthAgePhases) Finite(p.StartAgeYears, p.DbhIncrementFactor, p.HeightIncrementFactor);
        }
        if (s.Options.Stage5 is { } o5)
            // Stage5Options: grid 2.5 m (default) or 5.0 m, browse pressure in [0, 1].
            Rule((o5.GridCellM == 2.5 || o5.GridCellM == 5.0) && o5.BackgroundBrowsePressure >= 0f && o5.BackgroundBrowsePressure <= 1f,
                "Stage 5 options: grid 2.5 or 5.0 m and browse pressure in [0, 1]");
        if (s.Options.Phase2 is { } o2)
        {
            RequireOrdinalUnique(o2.GenesisSpecies.Select(e => e.TreeId).ToArray(), "genesis species tree IDs");
            foreach (var e in o2.GenesisSpecies)
                Rule(treeArea.ContainsKey(e.TreeId) && Broadleaf(e.SpeciesId), "genesis species name a known tree and sessile-oak or beech");
        }

        if (s.Stage4 is { } s4) ValidateStage4(s4, treeArea, living, eventKeys, hasApproved);
        if (s.Stage5 is { } s5) ValidateStage5(s5, areaIds, eventKeys, hasApproved);
        if (s.Phase2 is { } p2) ValidatePhase2(p2, areaIds, treeArea, eventKeys, hasApproved);
    }

    private static void ValidateStage4(Phase1Stage4State s, Dictionary<string, string> treeArea, HashSet<string> living,
        HashSet<string> eventKeys, bool hasApproved)
    {
        RequireOrdinalUnique(s.Pruning.Select(p => p.TreeId).ToArray(), "pruning history tree IDs");
        foreach (var p in s.Pruning)
        {
            Rule(treeArea.ContainsKey(p.TreeId) && p.Lifts >= 0, "pruning history names a known tree with non-negative lifts");
            Finite(p.CrownBaseHeightM);
        }
        foreach (var o in s.PendingPrunes.Concat(s.ApprovedPrunes))
        {
            Rule(treeArea.TryGetValue(o.TreeId, out var at) && at == o.AreaId && living.Contains(o.TreeId),
                "a Prune order names a living tree of its area");
            Finite(o.TargetCrownBaseHeightM);
        }
        Rule(hasApproved || s.ApprovedPrunes.Count == 0, "approved Prune orders exist only with an approved plan");
        foreach (var r in s.PruneLog)
        {
            Rule(treeArea.TryGetValue(r.TreeId, out var at) && at == r.AreaId && eventKeys.Contains(r.ExecutionKey),
                "a prune record names a tree of its area and an executed key");
            Finite(r.TargetCrownBaseHeightM, r.CrownRadiusBeforeM, r.CrownRadiusAfterM);
        }
        // Deadwood is numbered DW0001, DW0002, ... from NextDeadwoodNumber = 1 and only appended (Advance).
        Rule(s.NextDeadwoodNumber == s.Deadwood.Count + 1, "the next deadwood number follows the deadwood records");
        for (int i = 0; i < s.Deadwood.Count; i++)
        {
            var d = s.Deadwood[i];
            Rule(d.DeadwoodId == "DW" + (i + 1).ToString("0000", CultureInfo.InvariantCulture), "deadwood IDs run DW0001, DW0002, ...");
            Rule(treeArea.TryGetValue(d.TreeId, out var at) && at == d.AreaId && !living.Contains(d.TreeId),
                "a deadwood record names a dead tree of its area");
            Rule(d.SpeciesId is SitkaId or OakId or BeechId, "a deadwood species is sitka-spruce, sessile-oak or beech");
            Finite(d.XM, d.ZM, d.DbhCm, d.HeightM, d.EstimatedVolumeM3);
        }
    }

    private static void ValidateStage5(Phase1Stage5State s, string[] areaIds, HashSet<string> eventKeys, bool hasApproved)
    {
        Rule(s.Areas.Select(a => a.AreaId).SequenceEqual(areaIds, StringComparer.Ordinal), "Stage 5 cells cover every area in order");
        var cells = new HashSet<(string, int, int)>();
        foreach (var area in s.Areas)
            foreach (var c in area.Cells)
            {
                Rule(cells.Add((area.AreaId, c.Column, c.Row)), "Stage 5 cells are unique per area");
                Finite(c.CentreXM, c.CentreZM, c.Light, c.Bramble, c.Bracken);
                foreach (var b in c.Bands) Finite(b.Density, b.Height, b.LastBrowsedFraction);
            }
        foreach (var o in s.PendingClears.Concat(s.ApprovedClears))
            Rule(cells.Contains((o.AreaId, o.Column, o.Row)), "a ClearUndergrowth order names an existing cell of its area");
        Rule(hasApproved || s.ApprovedClears.Count == 0, "approved ClearUndergrowth orders exist only with an approved plan");
        foreach (var r in s.ClearLog)
            Rule(eventKeys.Contains(r.ExecutionKey) && areaIds.Contains(r.AreaId, StringComparer.Ordinal),
                "a clear record names a known area and an executed key");
        foreach (var y in s.Accounts)
            foreach (var a in y.Areas)
                Rule(areaIds.Contains(a.AreaId, StringComparer.Ordinal), "a Stage 5 account names a known area");
    }

    private static void ValidatePhase2(Phase1Phase2State s, string[] areaIds, Dictionary<string, string> treeArea,
        HashSet<string> eventKeys, bool hasApproved)
    {
        RequireOrdinalUnique(s.TreeSpecies.Select(e => e.TreeId).ToArray(), "adult species tree IDs");
        foreach (var e in s.TreeSpecies)
            Rule(treeArea.ContainsKey(e.TreeId) && Broadleaf(e.SpeciesId), "adult species identity names a known tree and a broadleaf");
        Rule(s.Areas.Select(a => a.AreaId).SequenceEqual(areaIds, StringComparer.Ordinal), "Phase 2 bands cover every area in order");
        foreach (var area in s.Areas)
            foreach (var c in area.Cells)
                foreach (var b in c.Bands)
                {
                    // Phase 2 has natural bands only (Phase2Band.Natural); planted juveniles are exact individuals.
                    Rule(Broadleaf(b.SpeciesId) && b.Origin == "Natural", "a broadleaf band is a natural Oak or Beech band");
                    Finite(b.Density, b.Height, b.LastBrowsedFraction);
                }

        var juveniles = new HashSet<string>(StringComparer.Ordinal);
        foreach (var j in s.Juveniles)
        {
            Rule(juveniles.Add(j.JuvenileId), "planted juvenile IDs are unique");
            Rule(SequenceNumber(j.JuvenileId, "PJ") is int n && n < s.NextJuvenileNumber, "juvenile IDs are PJnnnn below the next juvenile number");
            Rule(areaIds.Contains(j.AreaId, StringComparer.Ordinal) && Broadleaf(j.SpeciesId), "a juvenile is an Oak or Beech of a known area");
            Rule(j.PromotedTreeId is null || treeArea.ContainsKey(j.PromotedTreeId), "a promoted juvenile names its adult tree");
            Finite(j.XM, j.ZM, j.AgeYears, j.HeightM, j.LastBrowseProbability, j.LastVegetationLossProbability);
        }
        var shelters = new HashSet<string>(StringComparer.Ordinal);
        foreach (var h in s.Shelters)
        {
            Rule(shelters.Add(h.ShelterId) && juveniles.Contains(h.JuvenileId), "shelter IDs are unique and name a planted juvenile");
            Finite(h.XM, h.ZM);
        }
        var patches = new HashSet<string>(StringComparer.Ordinal);
        foreach (var p in s.Patches)
        {
            Rule(patches.Add(p.PatchId), "spot patch IDs are unique");
            Rule(SequenceNumber(p.PatchId, "SP") is int n && n < s.NextPatchNumber, "patch IDs are SPnnnn below the next patch number");
            Rule(areaIds.Contains(p.AreaId, StringComparer.Ordinal), "a spot patch belongs to a known area");
            Finite(p.XM, p.ZM, p.RadiusM, p.Bramble, p.Bracken);
        }
        var orderIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var o in s.PendingOrders.Concat(s.ApprovedOrders))
        {
            Rule(orderIds.Add(o.OrderId), "planting order IDs are unique");
            Rule(SequenceNumber(o.OrderId, "P2W") is int n && n < s.NextOrderNumber, "planting order IDs are P2Wnnnn below the next order number");
            Rule(areaIds.Contains(o.AreaId, StringComparer.Ordinal), "a planting order names a known area");
            Rule(o is not Phase1PlantOrder plant || Broadleaf(plant.SpeciesId), "a Plant order plants sessile-oak or beech");
            Finite(o.XM, o.ZM);
        }
        Rule(hasApproved || s.ApprovedOrders.Count == 0, "approved planting orders exist only with an approved plan");
        foreach (var r in s.WorkLog)
        {
            Rule(r.Kind is "Plant" or "Shelter" or "SpotClear", "a planting work record is Plant, Shelter or SpotClear");
            Rule(SequenceNumber(r.OrderId, "P2W") is int n && n < s.NextOrderNumber && !orderIds.Contains(r.OrderId),
                "an executed planting order is numbered and no longer pending");
            Rule(eventKeys.Contains(r.ExecutionKey) && areaIds.Contains(r.AreaId, StringComparer.Ordinal),
                "a planting work record names a known area and an executed key");
            Finite(r.XM, r.ZM);
        }
        Rule(s.NextOrderNumber >= 1 && s.NextJuvenileNumber >= 1 && s.NextPatchNumber >= 1, "Phase 2 sequence numbers start at 1");
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static void Orders(IReadOnlyList<Phase1WorkOrder> orders, Dictionary<string, string> treeArea, HashSet<string>? requireLiving)
    {
        foreach (var o in orders)
        {
            Rule(Enum.IsDefined(o.Action), "a work order has a known action");
            Rule(treeArea.TryGetValue(o.TreeId, out var at) && at == o.AreaId, "a work order names a tree of its area");
            Rule(requireLiving is null || requireLiving.Contains(o.TreeId), "an unexecuted Fell order names a living tree");
            Reason(o.Reason);
        }
    }

    private static void Token(Phase1SourceToken t) =>
        Rule(Id(t.Domain) && Id(t.ExperimentId) && Id(t.AreaIds) && t.EcologicalYear >= 0 && t.AnnualRevision >= 0 && t.WorkRevision >= 0,
            "a recorded source token is complete");

    private static void Reason(string reason)
    {
        // The engine refuses a blank reason (AddPending, Execute); the API caps its length.
        Rule(!string.IsNullOrWhiteSpace(reason) && reason.Length <= Phase1SaveFormat.MaxReasonLength,
            "every Fell reason is non-blank and at most the API's reason length");
    }

    private static bool Broadleaf(string? id) => id is OakId or BeechId;

    private static int? SequenceNumber(string id, string prefix)
    {
        if (!id.StartsWith(prefix, StringComparison.Ordinal) || id.Length < prefix.Length + 4) return null;
        string digits = id[prefix.Length..];
        if (!digits.All(char.IsAsciiDigit)) return null;
        return int.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out int n) && n >= 1 ? n : null;
    }

    private static bool Id(string? s) => !string.IsNullOrWhiteSpace(s);

    private static void Ascii(string s, string code)
    {
        foreach (char c in s)
            if (c < 0x20 || c > 0x7E || c == '"' || c == '\\')
                throw new P1Refusal(code, "only printable ASCII without quote or backslash is allowed");
    }

    private static void Finite(params float[] values) =>
        Rule(values.All(float.IsFinite), "every saved float is finite");

    private static void RequireOrdinalUnique(IReadOnlyList<string> ids, string what)
    {
        for (int i = 1; i < ids.Count; i++)
            Rule(string.CompareOrdinal(ids[i - 1], ids[i]) < 0, what + " are unique and in ordinal order");
    }

    private static void RequireUnique(IEnumerable<string> ids, string what)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var id in ids) Rule(seen.Add(id), what + " are unique");
    }

    private static void Rule(bool ok, string rule)
    {
        if (!ok) throw new P1Refusal("SaveInconsistent", "inconsistent save: " + rule);
    }
}

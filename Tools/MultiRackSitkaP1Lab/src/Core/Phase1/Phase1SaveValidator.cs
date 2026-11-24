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
        if (h.Models is null || !h.Models.IsSupported)
            throw new P1Refusal("SaveModelUnsupported", "model vector " + (h.Models?.ToString() ?? "(missing)") +
                " is not one the engine accepts (0/0/0/0/0, 0/0/1/0/0, 1/1/1/0/0, 1/1/1/1/0, 1/1/1/1/1)");
        var l = h.Lineage;
        Rule(l is not null && Id(l.P1AlgorithmVersion) && Id(l.AreaModelVersion), "the engine lineage is required and complete");
        var g = h.Genesis;
        Rule(g is not null, "the genesis identity is required");
        Rule(Id(g!.ExperimentId) && Id(g.GeneratorId), "the experiment and generator IDs are required");
        Rule(g.ManifestContentHash is { Length: 64 } && g.ManifestContentHash.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'),
            "the manifest content hash is 64 lowercase hex digits (DerivedManifest.Sha256Hex)");
        Rule(g.Areas is { Count: > 0 } && g.Areas.All(a => a is not null && Id(a.AreaId)), "the genesis names at least one area, each with an ID");
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
        var harvestDeaths = new Dictionary<string, Phase1Death>(StringComparer.Ordinal);
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
                    // Execute is the only writer of a FellCause.Harvest death (the annual rules never use it).
                    if (t.Death.Cause == FellCause.Harvest) harvestDeaths.Add(t.TreeId, t.Death);
                }
            }
            // Crop marks are sorted ordinal (MarkCrop) and cleared on death (Advance), so each is a living tree of the area.
            RequireOrdinalUnique(area.CropTreeIds, "crop marks of " + area.AreaId);
            foreach (var crop in area.CropTreeIds)
                Rule(living.Contains(crop) && treeArea[crop] == area.AreaId, "crop mark " + crop + " is a living tree of " + area.AreaId);
        }

        // Event ledger and executed-key receipts.
        var eventKeys = new HashSet<string>(StringComparer.Ordinal);
        var eventsByKey = new Dictionary<string, Phase1WorkEvent>(StringComparer.Ordinal);
        var felled = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < s.Events.Count; i++)
        {
            var e = s.Events[i];
            Rule(Id(e.ExecutionKey) && Id(e.PlanId), "every event has an execution key and a plan ID");
            Rule(eventKeys.Add(e.ExecutionKey), "event execution keys are unique");
            eventsByKey.Add(e.ExecutionKey, e);
            Rule(e.WorkRevision == i + 1, "events are in work-revision order 1, 2, 3, ...");
            Rule(e.EcologicalYear >= 0 && e.EcologicalYear <= s.EcologicalYear &&
                (i == 0 || e.EcologicalYear >= s.Events[i - 1].EcologicalYear), "events are not in the future and never go back in time");
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
                    Rule(f.Cause == FellCause.Harvest && f.Mechanism == FellCause.Mechanism,
                        "a fell record has the engine's Fell cause and mechanism");
                    Rule(treeArea.TryGetValue(f.TreeId, out var at) && at == f.AreaId && !living.Contains(f.TreeId),
                        "a felled tree " + f.TreeId + " is a dead stem of its area");
                    // Execute writes the tree's death and the fell record together, in the event's year (F4e).
                    Rule(felled.Add(f.TreeId), "a tree is felled at most once");
                    Rule(harvestDeaths.TryGetValue(f.TreeId, out var death) && death.EcologicalYear == e.EcologicalYear &&
                        death.Mechanism == FellCause.Mechanism,
                        "the felled tree " + f.TreeId + " died by Fell in the event's year");
                    Reason(f.Reason);
                }
            }
        }
        foreach (var treeId in harvestDeaths.Keys)
            Rule(felled.Contains(treeId), "a Fell death of " + treeId + " has its fell record in the ledger");

        RequireOrdinalUnique(s.Executed.Select(x => x.ExecutionKey).ToArray(), "executed keys");
        foreach (var x in s.Executed)
        {
            Rule(eventsByKey.TryGetValue(x.ExecutionKey, out var e), "every executed key has its event in the ledger");
            Orders(x.Orders, treeArea, requireLiving: null);
            // The receipt's orders are the approved plan's Fell orders; Execute turned each into exactly one fell record.
            var fells = e!.Areas.SelectMany(c => c.Fells).Select(f => (f.AreaId, f.TreeId, f.Reason)).OrderBy(f => f.TreeId, StringComparer.Ordinal);
            var orders = x.Orders.Select(o => (o.AreaId, o.TreeId, o.Reason)).OrderBy(o => o.TreeId, StringComparer.Ordinal);
            Rule(orders.SequenceEqual(fells), "an executed receipt's orders are exactly its event's fell records");
            ReceiptToken(x.RecordedSource, e, h, areaIds);
        }

        // Work plan. Approve moves every pending list into the approval; nothing is added while a plan is approved.
        Orders(s.Pending, treeArea, living);
        RequireUnique(s.Pending.Select(o => o.TreeId), "pending Fell tree IDs");
        bool hasApproved = s.Approved is not null;
        if (s.Approved is { } plan)
        {
            Orders(plan.Orders, treeArea, living);
            RequireUnique(plan.Orders.Select(o => o.TreeId), "approved Fell tree IDs");
            Rule(s.Pending.Count == 0 && (s.Stage4?.PendingPrunes.Count ?? 0) == 0 && (s.Stage5?.PendingClears.Count ?? 0) == 0 &&
                (s.Phase2?.PendingOrders.Count ?? 0) == 0, "no pending order coexists with an approved plan");
            // Approve refuses an empty plan (WorkEmptyPlan), so an approved plan holds at least one order of some kind.
            Rule(plan.Orders.Count + (s.Stage4?.ApprovedPrunes.Count ?? 0) + (s.Stage5?.ApprovedClears.Count ?? 0) +
                (s.Phase2?.ApprovedOrders.Count ?? 0) > 0, "an approved plan holds at least one order");
        }

        // NB2 (A0 defect): the restored order set must be one the engine itself could have produced, or the WHOLE load is
        // refused unchanged (never repaired and never partly discarded). The engine keeps at most one unexecuted order per
        // tree: AddPending refuses a tree already pending as Fell or Prune, AddPrune refuses either, and Approve only moves
        // the pending lists into the approval. It also refuses to propose a retained Crop Tree for Fell (CropTreeRetained)
        // and refuses a Crop mark on a planned tree (CropAlreadyPlannedForFell), so no Fell order can name a Crop Tree.
        var cropsByArea = s.Areas.ToDictionary(a => a.AreaId, a => new HashSet<string>(a.CropTreeIds, StringComparer.Ordinal),
            StringComparer.Ordinal);
        IReadOnlyList<Phase1WorkOrder> allFell = s.Pending
            .Concat(s.Approved?.Orders ?? (IReadOnlyList<Phase1WorkOrder>)Array.Empty<Phase1WorkOrder>()).ToArray();
        var orderedTrees = new HashSet<string>(StringComparer.Ordinal);
        foreach (var o in allFell)
        {
            Rule(orderedTrees.Add(o.TreeId), "a tree carries at most one unexecuted Fell order across pending and approved work");
            Rule(!cropsByArea[o.AreaId].Contains(o.TreeId), "no Fell order names a retained Crop Tree (" + o.TreeId + ")");
        }
        IReadOnlyList<Phase1PruneOrder> allPrunes = (s.Stage4?.PendingPrunes ?? (IReadOnlyList<Phase1PruneOrder>)Array.Empty<Phase1PruneOrder>())
            .Concat(s.Stage4?.ApprovedPrunes ?? Array.Empty<Phase1PruneOrder>()).ToArray();
        foreach (var o in allPrunes)
            Rule(orderedTrees.Add(o.TreeId), "a tree carries at most one unexecuted order (Fell or Prune) across pending and approved work");
        // AddClearUndergrowth refuses a cell already in the pending plan, and Approve only moves the list.
        var unexecutedClearCells = new HashSet<(string, int, int)>();
        foreach (var o in (s.Stage5?.PendingClears ?? (IReadOnlyList<Phase1ClearOrder>)Array.Empty<Phase1ClearOrder>())
                     .Concat(s.Stage5?.ApprovedClears ?? Array.Empty<Phase1ClearOrder>()))
            Rule(unexecutedClearCells.Add((o.AreaId, o.Column, o.Row)), "an unexecuted plan clears one ground cell at most once");
        // AddPlanting refuses a second order of the same kind at the same exact point (Phase2DuplicateOrder); the geometric
        // batch rules that need the live polygon lattice are revalidated by the engine at restore, not here.
        var unexecutedPlantingPoints = new HashSet<(string, string, float, float)>();
        foreach (var o in (s.Phase2?.PendingOrders ?? (IReadOnlyList<Phase1PlantingOrder>)Array.Empty<Phase1PlantingOrder>())
                     .Concat(s.Phase2?.ApprovedOrders ?? Array.Empty<Phase1PlantingOrder>()))
            Rule(unexecutedPlantingPoints.Add((o.GetType().Name, o.AreaId, o.XM, o.ZM)),
                "an unexecuted planting plan names each kind and exact point at most once");

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
                        d.EcologicalYear >= 0 && d.EcologicalYear <= s.EcologicalYear && d.Cause != FellCause.Harvest,
                        "a natural death names a dead tree of its area, is not in the future and is not a Fell");
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
        if (s.Stage5 is { } s5)
        {
            ValidateStage5(s5, areaIds, eventKeys, hasApproved, s.EcologicalYear);
            // Constructor: Stage5Annual.Genesis(options, options.MastSeed ?? unchecked((int)GeneratorSeed), ...); never rewritten.
            Rule(s5.MastSeed == (s.Options.Stage5!.MastSeed ?? unchecked((int)h.Genesis.GeneratorSeed)),
                "the Stage 5 mast seed is the options' seed, or the generator seed when the options name none");
        }
        if (s.Phase2 is { } p2)
        {
            ValidatePhase2(p2, areaIds, treeArea, eventKeys, hasApproved, s.EcologicalYear);
            var stage5 = s.Stage5!; // Phase 2 implies Stage 5 (checked above)
            // Phase2Annual.Genesis: Oak/Beech seeds are the options' seeds, or the Stage 5 mast seed; never rewritten.
            Rule(p2.OakMastSeed == (s.Options.Phase2!.OakMastSeed ?? stage5.MastSeed) &&
                p2.BeechMastSeed == (s.Options.Phase2.BeechMastSeed ?? stage5.MastSeed),
                "the Phase 2 Oak and Beech mast seeds are the options' seeds, or the Stage 5 mast seed");
            // Phase2Annual indexes the broadleaf band lattice by POSITION against the Stage 5 cells (RequireSameAreas, and
            // next2[c] is built from cells[c].Column/Row), so the two lattices must match cell for cell.
            for (int i = 0; i < p2.Areas.Count; i++)
            {
                var cells5 = stage5.Areas[i].Cells;
                var cells2 = p2.Areas[i].Cells;
                Rule(cells2.Count == cells5.Count && cells2.Select(c => (c.Column, c.Row)).SequenceEqual(cells5.Select(c => (c.Column, c.Row))),
                    "the Phase 2 band lattice of " + p2.Areas[i].AreaId + " matches its Stage 5 cells position by position");
            }
        }
        RequirePlanStateRevision(s);
    }

    // PlanStateRevision (F4d). The engine only ever INCREMENTS it, by exactly 1, in: AddPending/AddPrune/AddClearUndergrowth/
    // AddPlant/AddShelter/AddSpotClear, RemovePending/RemovePrune/RemoveClearUndergrowth/RemovePlanting, MarkCrop/UnmarkCrop,
    // Approve, CancelApproved, Execute, and Advance when it cancels unexecuted work or clears a lost Crop mark. Removes and
    // unmarks make any upper bound impossible, so the derivable invariant is a LOWER bound counting the increments the saved
    // state proves happened (each counted once; every order has exactly one lifecycle: add → (remove | approve → (execute |
    // cancel | advance-cancel) | advance-cancel | still current)):
    //   executions:      2 per event (Approve + Execute)
    //   cancelled years: 1 per annual record with CancelledUnexecutedWork (that Advance's own increment)
    //   current plan:    1 if a plan is approved (its Approve)
    //   crop marks:      1 per current mark (its MarkCrop)
    //   adds:            at least max(A, B), where
    //       A = one add per execution, per cancelled year and per current approval (each needed a non-empty plan),
    //           plus every current pending order of every kind;
    //       B = one add per executed Fell order, prune record and clear record, per currently approved Fell/Prune/Clear
    //           order, per current pending Fell/Prune/Clear order, plus every planting order ever numbered
    //           (Phase 2 NextOrderNumber − 1; numbers are minted only by AddPlanting).
    private static void RequirePlanStateRevision(Phase1SaveState s)
    {
        long cancelledYears = s.AnnualHistory.Count(y => y.CancelledUnexecutedWork);
        long approved = s.Approved is null ? 0 : 1;
        long crops = s.Areas.Sum(a => (long)a.CropTreeIds.Count);
        long pendingNonPlanting = s.Pending.Count + (s.Stage4?.PendingPrunes.Count ?? 0) + (s.Stage5?.PendingClears.Count ?? 0);
        long pendingAll = pendingNonPlanting + (s.Phase2?.PendingOrders.Count ?? 0);
        long addsA = s.WorkRevision + cancelledYears + approved + pendingAll;
        long addsB = s.Executed.Sum(x => (long)x.Orders.Count) + (s.Stage4?.PruneLog.Count ?? 0) + (s.Stage5?.ClearLog.Count ?? 0) +
            (s.Approved?.Orders.Count ?? 0) + (s.Stage4?.ApprovedPrunes.Count ?? 0) + (s.Stage5?.ApprovedClears.Count ?? 0) +
            pendingNonPlanting + Math.Max(0, (s.Phase2?.NextOrderNumber ?? 1) - 1);
        long lowerBound = 2L * s.WorkRevision + cancelledYears + approved + crops + Math.Max(addsA, addsB);
        Rule(s.PlanStateRevision >= lowerBound, FormattableString.Invariant(
            $"the plan-state revision {s.PlanStateRevision} is at least {lowerBound}, the plan commands this state proves happened"));
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

    // Stage5Annual.Step and Phase2Annual.Step append exactly one account per Advance, (yearAfter − 1 → yearAfter), from year 0.
    private static void RequireYearAligned(IReadOnlyList<(int YearBefore, int YearAfter)> accounts, int ecologicalYear, string what)
    {
        Rule(accounts.Count == ecologicalYear, what + " has exactly one annual account per committed year");
        for (int i = 0; i < accounts.Count; i++)
            Rule(accounts[i].YearBefore == i && accounts[i].YearAfter == i + 1, what + " accounts are consecutive years 0→1, 1→2, ...");
    }

    private static void ValidateStage5(Phase1Stage5State s, string[] areaIds, HashSet<string> eventKeys, bool hasApproved, int year)
    {
        RequireYearAligned(s.Accounts.Select(a => (a.YearBefore, a.YearAfter)).ToArray(), year, "Stage 5");
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
        HashSet<string> eventKeys, bool hasApproved, int year)
    {
        RequireYearAligned(s.Accounts.Select(a => (a.YearBefore, a.YearAfter)).ToArray(), year, "Phase 2");
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

    // An executed receipt keeps the approved plan's source token (F4f). Execute refuses unless plan.Source == Token(root), so
    // the token is the root's token at the moment of execution: this experiment's domain, ID, content hash and area list,
    // the event's year (AnnualRevision == EcologicalYear always), the work revision just before the event, and the session's
    // lineage — which is the header lineage, because a session's lineage never changes.
    private static void ReceiptToken(Phase1SourceToken t, Phase1WorkEvent e, Phase1SaveHeader h, string[] areaIds)
    {
        Rule(t is not null, "an executed receipt has its recorded source");
        Rule(t!.Domain == MultiAreaIdentity.Domain && t.ExperimentId == h.Genesis.ExperimentId &&
            t.ManifestContentHash == h.Genesis.ManifestContentHash && t.AreaIds == string.Join(",", areaIds),
            "a receipt's recorded source names this experiment, its content hash and its areas");
        Rule(t.EcologicalYear == e.EcologicalYear && t.AnnualRevision == e.EcologicalYear && t.WorkRevision == e.WorkRevision - 1,
            "a receipt's recorded source is the forest exactly as it was when its event executed");
        var l = h.Lineage;
        Rule(t.RngModelVersion == l.RngModelVersion && t.GrowthModelVersion == l.GrowthModelVersion &&
            t.P1AlgorithmVersion == l.P1AlgorithmVersion && t.AreaModelVersion == l.AreaModelVersion &&
            t.ActionEligibleDbhThresholdMm == l.ActionEligibleDbhThresholdMm, "a receipt's recorded lineage is the save's engine lineage");
    }

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

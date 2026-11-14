namespace SitkaLab.Core;

/// <summary>Structural validation of a whole snapshot. Fails closed with a LabException; never clamps or repairs input.</summary>
public static class SnapshotValidator
{
    public static void ValidateSnapshot(ForestSnapshot s)
    {
        if (!LabConstants.IsSafeId(s.ForestId)) throw new LabException("InvalidId", "forestId must be 1-64 characters of [A-Za-z0-9_-]");
        if (s.InventoryYear < 1 || s.InventoryYear > 9999) throw new LabException("InvalidValue", "inventoryYear must be 1..9999");
        if (s.ExperimentStep < 0 || s.ExperimentStep > LabConstants.MaxYears) throw new LabException("InvalidValue", "experimentStep out of range");
        if (s.ForestRevision < 0 || s.ForestRevision > LabConstants.MaxYears) throw new LabException("InvalidValue", "forestRevision out of range");
        if (s.AdultThresholdMm < 1 || s.AdultThresholdMm > LabConstants.MaxMm) throw new LabException("InvalidValue", "adultThresholdMm must be positive integer millimetres");
        if (s.SlopeDeg != 0) throw new LabException("InvalidValue", "P0 sites are flat: slopeDeg must be 0");

        var polyError = Geometry.ValidatePolygon(s.Polygon);
        if (polyError is not null) throw new LabException("InvalidPolygon", polyError);

        string? prev = null;
        foreach (var t in s.Trees)
        {
            if (!LabConstants.IsSafeId(t.TreeId)) throw new LabException("InvalidId", $"treeId '{Trunc(t.TreeId)}' must be 1-64 characters of [A-Za-z0-9_-]");
            if (prev is not null)
            {
                int c = string.CompareOrdinal(prev, t.TreeId);
                if (c == 0) throw new LabException("DuplicateId", $"duplicate treeId '{t.TreeId}'");
                if (c > 0) throw new LabException("NotSorted", "trees must be sorted by case-sensitive ordinal treeId");
            }
            prev = t.TreeId;
            if (t.Species != LabConstants.Species) throw new LabException("InvalidValue", $"tree {t.TreeId}: only species 'Sitka' is supported");
            if (t.ZMm != 0) throw new LabException("InvalidValue", $"tree {t.TreeId}: zMm must be 0 on a flat site");
            if (Math.Abs(t.XMm) > LabConstants.MaxMm || Math.Abs(t.YMm) > LabConstants.MaxMm) throw new LabException("InvalidValue", $"tree {t.TreeId}: coordinate out of range");
            if (t.DbhMm < 1 || t.DbhMm > LabConstants.MaxMm) throw new LabException("InvalidValue", $"tree {t.TreeId}: dbhMm must be a positive integer (<= {LabConstants.MaxMm})");
            if (t.HeightMm < 1 || t.HeightMm > LabConstants.MaxMm) throw new LabException("InvalidValue", $"tree {t.TreeId}: heightMm must be a positive integer (<= {LabConstants.MaxMm})");
            if (t.AgeAtInventoryYears < 0 || t.AgeAtInventoryYears > LabConstants.MaxYears) throw new LabException("InvalidValue", $"tree {t.TreeId}: ageAtInventoryYears out of range");
            if (!Geometry.ContainsInclusive(s.Polygon, t.XMm, t.YMm)) throw new LabException("OutsidePolygon", $"tree {t.TreeId} lies outside the stand polygon");
            bool adult = s.IsAdult(t);
            if (t.Mark == MarkState.PlayerMarked && !adult)
                throw new LabException("InvalidMark", $"tree {t.TreeId}: only living adults can be PLAYER_MARKED (dead, felled and below-threshold trees must be UNMARKED)");
        }

        var reserved = new Dictionary<string, string>(StringComparer.Ordinal);
        string? prevPlan = null;
        foreach (var p in s.Plans)
        {
            if (!LabConstants.IsSafeId(p.PlanId)) throw new LabException("InvalidId", $"planId '{Trunc(p.PlanId)}' must be 1-64 characters of [A-Za-z0-9_-]");
            if (prevPlan is not null)
            {
                int c = string.CompareOrdinal(prevPlan, p.PlanId);
                if (c == 0) throw new LabException("DuplicateId", $"duplicate planId '{p.PlanId}'");
                if (c > 0) throw new LabException("NotSorted", "plans must be sorted by ordinal planId");
            }
            prevPlan = p.PlanId;
            if (p.TreeIds.Count == 0) throw new LabException("InvalidPlan", $"plan {p.PlanId} has no trees");
            string? prevId = null;
            foreach (var id in p.TreeIds)
            {
                if (!LabConstants.IsSafeId(id)) throw new LabException("InvalidId", $"plan {p.PlanId}: bad treeId");
                if (prevId is not null)
                {
                    int c = string.CompareOrdinal(prevId, id);
                    if (c == 0) throw new LabException("DuplicateId", $"plan {p.PlanId} lists tree {id} more than once");
                    if (c > 0) throw new LabException("NotSorted", $"plan {p.PlanId}: treeIds must be ordinal sorted");
                }
                prevId = id;
                var t = s.FindTree(id) ?? throw new LabException("InvalidPlan", $"plan {p.PlanId} references unknown tree {id}");
                if (p.IsOpen)
                {
                    if (!s.IsAdult(t)) throw new LabException("InvalidPlan", $"open plan {p.PlanId} contains tree {id} that is not a living adult");
                    if (t.Mark != MarkState.PlayerMarked) throw new LabException("InvalidPlan", $"open plan {p.PlanId}: tree {id} is not PLAYER_MARKED");
                    if (reserved.TryGetValue(id, out var other)) throw new LabException("InvalidPlan", $"tree {id} belongs to two open plans ({other}, {p.PlanId})");
                    reserved[id] = p.PlanId;
                }
                else if (p.Stage == PlanStage.Executed && t.Status != TreeStatus.Felled)
                    throw new LabException("InvalidPlan", $"executed plan {p.PlanId}: tree {id} is not felled");
            }

            bool hasTokens = p.BaseForestRevision is not null && p.ApprovalSessionEpoch is not null;
            bool noTokens = p.BaseForestRevision is null && p.ApprovalSessionEpoch is null;
            if (!hasTokens && !noTokens) throw new LabException("InvalidPlan", $"plan {p.PlanId}: baseForestRevision and approvalSessionEpoch must be both set or both null");
            switch (p.Stage)
            {
                case PlanStage.Draft:
                case PlanStage.Proposed:
                case PlanStage.Rejected:
                    if (!noTokens) throw new LabException("InvalidPlan", $"plan {p.PlanId}: approval tokens must be null before approval");
                    break;
                case PlanStage.Approved:
                case PlanStage.PendingWork:
                case PlanStage.Executed:
                case PlanStage.Stale:
                    if (!hasTokens) throw new LabException("InvalidPlan", $"plan {p.PlanId}: approval tokens are required in state {Wire.Name(p.Stage)}");
                    break;
            }
            if (p.BaseForestRevision is long b && (b < 0 || b > s.ForestRevision))
                throw new LabException("InvalidPlan", $"plan {p.PlanId}: baseForestRevision is outside 0..forestRevision");
            if (p.ApprovalSessionEpoch is string ep && !LabConstants.IsEpoch(ep))
                throw new LabException("InvalidPlan", $"plan {p.PlanId}: approvalSessionEpoch is not a hex token");
        }
        // Every PLAYER_MARKED tree may be free (marked, no plan yet) — nothing further to check.
    }

    private static string Trunc(string s) => s.Length > 24 ? s[..24] + "…" : s;
}

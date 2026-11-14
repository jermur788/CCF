namespace SitkaLab.Core;

/// <summary>
/// The single state-transition function. Live operations and replay both go through <see cref="Apply"/>, so the
/// guards that protect live play are the same guards that verify a recorded history.
/// </summary>
internal static class Engine
{
    internal sealed record ExecCheck(string Reason, string Message, List<string> OffendingIds);

    private static bool SameIds(IReadOnlyList<string> a, IReadOnlyList<string> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
            if (!string.Equals(a[i], b[i], StringComparison.Ordinal)) return false;
        return true;
    }

    private static string? CheckIdList(IReadOnlyList<string> ids, bool requireNonEmpty)
    {
        if (requireNonEmpty && ids.Count == 0) return "tree ID list is empty";
        for (int i = 0; i < ids.Count; i++)
        {
            if (!LabConstants.IsSafeId(ids[i])) return $"invalid tree ID '{Short(ids[i])}'";
            if (i > 0)
            {
                int c = string.CompareOrdinal(ids[i - 1], ids[i]);
                if (c == 0) return $"duplicate tree ID '{ids[i]}'";
                if (c > 0) return "tree IDs must be unique and in ordinal order";
            }
        }
        return null;
    }

    private static string Short(string s) => s.Length > 24 ? s[..24] + "…" : s;

    private static HashSet<string> OpenReserved(ForestSnapshot s, string? exceptPlanId = null)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var p in s.Plans)
            if (p.IsOpen && p.PlanId != exceptPlanId)
                foreach (var id in p.TreeIds) set.Add(id);
        return set;
    }

    private static void Release(ForestSnapshot s, PlanRecord plan)
    {
        var otherOpen = OpenReserved(s, plan.PlanId);
        foreach (var id in plan.TreeIds)
        {
            var t = s.FindTree(id);
            if (t is not null && !otherOpen.Contains(id)) t.Mark = MarkState.Unmarked;
        }
    }

    /// <summary>Pre-mutation validation of "Execute Test Work". Returns null when the whole plan may be executed.</summary>
    internal static ExecCheck? CheckExecute(ForestSnapshot s, PlanRecord plan, string? guardEpoch)
    {
        switch (plan.Stage)
        {
            case PlanStage.PendingWork: break;
            case PlanStage.Executed: return new("PlanAlreadyExecuted", $"plan {plan.PlanId} has already been executed", new());
            case PlanStage.Stale: return new("PlanStale", $"plan {plan.PlanId} is STALE and can no longer be executed", new());
            case PlanStage.Cancelled:
            case PlanStage.Rejected: return new("PlanCancelledOrRejected", $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}", new());
            default: return new("PlanNotPending", $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; it must be queued (PENDING_WORK) first", new());
        }
        if (plan.BaseForestRevision != s.ForestRevision)
            return new("RevisionMismatch", $"plan {plan.PlanId} was approved at forest revision {plan.BaseForestRevision}, current is {s.ForestRevision}", new());
        if (!string.Equals(plan.ApprovalSessionEpoch, guardEpoch, StringComparison.Ordinal))
            return new("EpochMismatch", $"plan {plan.PlanId} was approved in a different session epoch", new());

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var dups = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var id in plan.TreeIds) if (!seen.Add(id)) dups.Add(id);
        if (dups.Count > 0) return new("TreeDuplicate", "plan lists a tree more than once", dups.ToList());

        var others = OpenReserved(s, plan.PlanId);
        var missing = new SortedSet<string>(StringComparer.Ordinal);
        var notLive = new SortedSet<string>(StringComparer.Ordinal);
        var below = new SortedSet<string>(StringComparer.Ordinal);
        var unreserved = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var id in seen)
        {
            var t = s.FindTree(id);
            if (t is null) { missing.Add(id); continue; }
            if (t.Status != TreeStatus.Live) { notLive.Add(id); continue; }
            if (t.DbhMm < s.AdultThresholdMm) { below.Add(id); continue; }
            if (t.Mark != MarkState.PlayerMarked || others.Contains(id)) unreserved.Add(id);
        }
        if (missing.Count > 0) return new("TreeMissing", "plan references a tree that does not exist", missing.ToList());
        if (notLive.Count > 0) return new("TreeNotLive", "plan includes a tree that is dead or already felled", notLive.ToList());
        if (below.Count > 0) return new("TreeBelowThreshold", "plan includes a tree below the adult DBH threshold", below.ToList());
        if (unreserved.Count > 0) return new("TreeNotReserved", "plan includes a tree that is not reserved to this plan", unreserved.ToList());
        return null;
    }

    private static string? RequireFreeAdult(ForestSnapshot s, string id, MarkState expectedMark)
    {
        var t = s.FindTree(id);
        if (t is null) return $"tree {id} does not exist";
        if (t.Status != TreeStatus.Live) return $"tree {id} is {Wire.Name(t.Status)} and cannot be marked or worked";
        if (t.DbhMm < s.AdultThresholdMm) return $"tree {id} is below the adult DBH threshold and is not actionable";
        if (t.Mark != expectedMark) return expectedMark == MarkState.Unmarked ? $"tree {id} is already marked" : $"tree {id} is not marked";
        return null;
    }

    /// <summary>Applies one event to <paramref name="s"/>. Returns an error message (state may be partly changed — callers work on a clone) or null.</summary>
    internal static string? Apply(ForestSnapshot s, LabEvent e, string? guardEpoch, bool replay)
    {
        var pl = e.Payload;
        switch (e.Type)
        {
            case EventType.TreeMarked:
            case EventType.TreeUnmarked:
            {
                var err = CheckIdList(e.TreeIds, true);
                if (err is not null) return err;
                bool marking = e.Type == EventType.TreeMarked;
                var reserved = marking ? null : OpenReserved(s);
                foreach (var id in e.TreeIds)
                {
                    var bad = RequireFreeAdult(s, id, marking ? MarkState.Unmarked : MarkState.PlayerMarked);
                    if (bad is not null) return bad;
                    if (!marking && reserved!.Contains(id)) return $"tree {id} belongs to an open plan; cancel the plan first";
                }
                foreach (var id in e.TreeIds) s.FindTree(id)!.Mark = marking ? MarkState.PlayerMarked : MarkState.Unmarked;
                return null;
            }
            case EventType.PlanCreated:
            {
                if (!LabConstants.IsSafeId(pl.PlanId)) return "invalid planId";
                if (s.FindPlan(pl.PlanId!) is not null) return $"plan {pl.PlanId} already exists";
                if (s.Plans.Count >= LabConstants.MaxPlans) return "plan limit reached";
                var err = CheckIdList(e.TreeIds, true);
                if (err is not null) return err;
                var reserved = OpenReserved(s);
                foreach (var id in e.TreeIds)
                {
                    var bad = RequireFreeAdult(s, id, MarkState.PlayerMarked);
                    if (bad is not null) return bad;
                    if (reserved.Contains(id)) return $"tree {id} already belongs to another open plan";
                }
                s.Plans.Add(new PlanRecord { PlanId = pl.PlanId!, Stage = PlanStage.Draft, TreeIds = new List<string>(e.TreeIds) });
                s.Plans.Sort((a, b) => string.CompareOrdinal(a.PlanId, b.PlanId));
                return null;
            }
            case EventType.PlanProposed:
            case EventType.PlanApproved:
            case EventType.PlanQueued:
            case EventType.PlanRejected:
            case EventType.PlanCancelled:
            case EventType.PlanStale:
            case EventType.TestWorkExecuted:
            {
                var plan = pl.PlanId is null ? null : s.FindPlan(pl.PlanId);
                if (plan is null) return $"plan {pl.PlanId} does not exist";
                if (!SameIds(e.TreeIds, plan.TreeIds)) return $"event tree IDs do not match plan {plan.PlanId}";
                return ApplyPlanEvent(s, e, plan, guardEpoch, replay);
            }
            case EventType.PlanExecutionRejected:
            {
                var plan = pl.PlanId is null ? null : s.FindPlan(pl.PlanId);
                if (plan is null) return $"plan {pl.PlanId} does not exist";
                if (Array.IndexOf(Wire.RejectionReasons, pl.Reason) < 0) return "unknown rejection reason";
                return CheckIdList(e.TreeIds, false); // audit-only: no state change
            }
            case EventType.ExperimentStepAdvanced:
            {
                if (e.TreeIds.Count != 0) return "step event must not list trees";
                if (s.ExperimentStep >= LabConstants.MaxYears || s.ForestRevision >= LabConstants.MaxYears) return "step/revision limit reached";
                if (pl.ExperimentStepAfter != s.ExperimentStep + 1 || pl.ForestRevisionAfter != s.ForestRevision + 1) return "step payload does not match state";
                s.ExperimentStep++; s.ForestRevision++; // calendar only: age, DBH, height untouched
                return null;
            }
            default:
                return "unknown event type";
        }
    }

    private static string? ApplyPlanEvent(ForestSnapshot s, LabEvent e, PlanRecord plan, string? guardEpoch, bool replay)
    {
        var pl = e.Payload;
        switch (e.Type)
        {
            case EventType.PlanProposed:
            {
                if (plan.Stage != PlanStage.Draft) return $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; only a DRAFT can be proposed";
                var bad = RevalidateMembers(s, plan);
                if (bad is not null) return bad;
                plan.Stage = PlanStage.Proposed;
                return null;
            }
            case EventType.PlanApproved:
            {
                if (plan.Stage != PlanStage.Proposed) return $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; only a PROPOSED plan can be approved";
                if (!LabConstants.IsEpoch(pl.ApprovalSessionEpoch)) return "approvalSessionEpoch is not a hex token";
                if (!replay && !string.Equals(pl.ApprovalSessionEpoch, guardEpoch, StringComparison.Ordinal)) return "approval epoch does not match the live session";
                if (pl.BaseForestRevision != s.ForestRevision) return "approval base revision does not match the current forest revision";
                var bad = RevalidateMembers(s, plan);
                if (bad is not null) return bad;
                plan.Stage = PlanStage.Approved;
                plan.BaseForestRevision = pl.BaseForestRevision;
                plan.ApprovalSessionEpoch = pl.ApprovalSessionEpoch;
                return null;
            }
            case EventType.PlanQueued:
            {
                if (plan.Stage != PlanStage.Approved) return $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; only an APPROVED plan can be queued";
                if (plan.BaseForestRevision != s.ForestRevision) return "plan is stale: forest revision changed since approval";
                if (!string.Equals(plan.ApprovalSessionEpoch, guardEpoch, StringComparison.Ordinal)) return "plan is stale: session epoch changed since approval";
                var bad = RevalidateMembers(s, plan);
                if (bad is not null) return bad;
                plan.Stage = PlanStage.PendingWork;
                return null;
            }
            case EventType.PlanRejected:
            {
                if (plan.Stage != PlanStage.Proposed) return $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; only a PROPOSED plan can be rejected";
                plan.Stage = PlanStage.Rejected;
                Release(s, plan);
                return null;
            }
            case EventType.PlanCancelled:
            {
                if (!plan.IsOpen) return $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; only an open plan can be cancelled";
                plan.Stage = PlanStage.Cancelled;
                Release(s, plan);
                return null;
            }
            case EventType.PlanStale:
            {
                if (plan.Stage is not (PlanStage.Approved or PlanStage.PendingWork)) return $"plan {plan.PlanId} is {Wire.Name(plan.Stage)}; only APPROVED or PENDING_WORK plans can become STALE";
                if (Array.IndexOf(Wire.StaleReasons, pl.Reason) < 0) return "unknown stale reason";
                plan.Stage = PlanStage.Stale;
                Release(s, plan);
                return null;
            }
            case EventType.TestWorkExecuted:
            {
                var bad = CheckExecute(s, plan, guardEpoch);
                if (bad is not null) return bad.Message;
                if (s.ForestRevision >= LabConstants.MaxYears) return "forest revision limit reached";
                if (pl.ForestRevisionAfter != s.ForestRevision + 1) return "execution payload does not match state";
                foreach (var id in plan.TreeIds)
                {
                    var t = s.FindTree(id)!;
                    t.Status = TreeStatus.Felled;
                    t.Mark = MarkState.Unmarked;
                }
                plan.Stage = PlanStage.Executed;
                s.ForestRevision++;
                return null;
            }
            default:
                return "unexpected event";
        }
    }

    private static string? RevalidateMembers(ForestSnapshot s, PlanRecord plan)
    {
        var others = OpenReserved(s, plan.PlanId);
        foreach (var id in plan.TreeIds)
        {
            var bad = RequireFreeAdult(s, id, MarkState.PlayerMarked);
            if (bad is not null) return bad;
            if (others.Contains(id)) return $"tree {id} also belongs to another open plan";
        }
        return null;
    }
}

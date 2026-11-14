using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SitkaLab.Core;

/// <summary>Wire names for the enums (fixed by the v1 schema).</summary>
public static class Wire
{
    public static string Name(TreeStatus s) => s switch { TreeStatus.Live => "live", TreeStatus.Dead => "dead", _ => "felled" };
    public static string Name(MarkState m) => m == MarkState.Unmarked ? "UNMARKED" : "PLAYER_MARKED";
    public static string Name(PlanStage p) => p switch
    {
        PlanStage.Draft => "DRAFT",
        PlanStage.Proposed => "PROPOSED",
        PlanStage.Approved => "APPROVED",
        PlanStage.PendingWork => "PENDING_WORK",
        PlanStage.Executed => "EXECUTED",
        PlanStage.Rejected => "REJECTED",
        PlanStage.Cancelled => "CANCELLED",
        _ => "STALE",
    };

    public static bool TryTreeStatus(string s, out TreeStatus v)
    {
        switch (s) { case "live": v = TreeStatus.Live; return true; case "dead": v = TreeStatus.Dead; return true; case "felled": v = TreeStatus.Felled; return true; }
        v = default; return false;
    }

    public static bool TryMark(string s, out MarkState v)
    {
        switch (s) { case "UNMARKED": v = MarkState.Unmarked; return true; case "PLAYER_MARKED": v = MarkState.PlayerMarked; return true; }
        v = default; return false;
    }

    public static bool TryPlanStage(string s, out PlanStage v)
    {
        foreach (var p in Enum.GetValues<PlanStage>())
            if (Name(p) == s) { v = p; return true; }
        v = default; return false;
    }

    public static bool TryEventType(string s, out EventType v) => Enum.TryParse(s, false, out v) && Enum.IsDefined(v) && v.ToString() == s;

    /// <summary>Stale reasons and execution-rejection reasons are closed enumerations (safe ASCII tokens).</summary>
    public static readonly string[] StaleReasons = { "ForestRevisionChanged", "ExperimentStepAdvanced", "SessionLoaded" };
    public static readonly string[] RejectionReasons =
    {
        "PlanNotPending", "PlanAlreadyExecuted", "PlanStale", "PlanCancelledOrRejected",
        "RevisionMismatch", "EpochMismatch", "TreeDuplicate", "TreeMissing", "TreeNotLive", "TreeBelowThreshold", "TreeNotReserved",
    };
}

/// <summary>
/// Canonical v1 writer: UTF-8 without BOM, compact (no insignificant whitespace, no newline), fixed field order,
/// integers only. Strings are restricted to a safe ASCII set so no JSON escaping is ever required.
/// </summary>
public static class CanonicalJson
{
    private static readonly UTF8Encoding Utf8 = new(false);

    private static void Str(StringBuilder sb, string v)
    {
        foreach (char c in v)
        {
            bool ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_' || c == '-' || c == '.';
            if (!ok) throw new LabException("UnsafeString", $"String contains a character outside the canonical safe set: U+{(int)c:X4}");
        }
        sb.Append('"').Append(v).Append('"');
    }

    private static void Int(StringBuilder sb, long v) => sb.Append(v.ToString(CultureInfo.InvariantCulture));

    private static void Key(StringBuilder sb, string name, bool first = false)
    {
        if (!first) sb.Append(',');
        sb.Append('"').Append(name).Append("\":");
    }

    private static void IdArray(StringBuilder sb, IReadOnlyList<string> ids)
    {
        sb.Append('[');
        for (int i = 0; i < ids.Count; i++)
        {
            if (i > 0) sb.Append(',');
            Str(sb, ids[i]);
        }
        sb.Append(']');
    }

    public static string Snapshot(ForestSnapshot s)
    {
        var sb = new StringBuilder(256 + s.Trees.Count * 150);
        Key(sb, "forestId", true); Str(sb, s.ForestId);
        Key(sb, "inventoryYear"); Int(sb, s.InventoryYear);
        Key(sb, "experimentStep"); Int(sb, s.ExperimentStep);
        Key(sb, "forestRevision"); Int(sb, s.ForestRevision);
        Key(sb, "adultThresholdMm"); Int(sb, s.AdultThresholdMm);
        Key(sb, "standAreaPolygon"); sb.Append('[');
        for (int i = 0; i < s.Polygon.Count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append("{\"xMm\":"); Int(sb, s.Polygon[i].XMm);
            sb.Append(",\"yMm\":"); Int(sb, s.Polygon[i].YMm); sb.Append('}');
        }
        sb.Append(']');
        Key(sb, "siteProfile"); sb.Append("{\"slopeDeg\":"); Int(sb, s.SlopeDeg); sb.Append('}');
        Key(sb, "trees"); sb.Append('[');
        for (int i = 0; i < s.Trees.Count; i++)
        {
            var t = s.Trees[i];
            if (i > 0) sb.Append(',');
            sb.Append('{');
            Key(sb, "treeId", true); Str(sb, t.TreeId);
            Key(sb, "species"); Str(sb, t.Species);
            Key(sb, "xMm"); Int(sb, t.XMm);
            Key(sb, "yMm"); Int(sb, t.YMm);
            Key(sb, "zMm"); Int(sb, t.ZMm);
            Key(sb, "dbhMm"); Int(sb, t.DbhMm);
            Key(sb, "heightMm"); Int(sb, t.HeightMm);
            Key(sb, "ageAtInventoryYears"); Int(sb, t.AgeAtInventoryYears);
            Key(sb, "status"); Str(sb, Wire.Name(t.Status));
            Key(sb, "markState"); Str(sb, Wire.Name(t.Mark));
            sb.Append('}');
        }
        sb.Append(']');
        Key(sb, "plans"); sb.Append('[');
        for (int i = 0; i < s.Plans.Count; i++)
        {
            var p = s.Plans[i];
            if (i > 0) sb.Append(',');
            sb.Append('{');
            Key(sb, "planId", true); Str(sb, p.PlanId);
            Key(sb, "planState"); Str(sb, Wire.Name(p.Stage));
            Key(sb, "treeIds"); IdArray(sb, p.TreeIds);
            Key(sb, "baseForestRevision"); if (p.BaseForestRevision is long b) Int(sb, b); else sb.Append("null");
            Key(sb, "approvalSessionEpoch"); if (p.ApprovalSessionEpoch is string e) Str(sb, e); else sb.Append("null");
            sb.Append('}');
        }
        sb.Append(']');
        return "{" + sb + "}";
    }

    public static byte[] SnapshotBytes(ForestSnapshot s) => Utf8.GetBytes(Snapshot(s));

    public static string Sha256Hex(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    public static string SnapshotHash(ForestSnapshot s) => Sha256Hex(SnapshotBytes(s));

    /// <summary>Fixed payload field order per event type (documented in schemas/lab-file-v1.md).</summary>
    public static string Payload(EventType type, EventPayload p)
    {
        var sb = new StringBuilder();
        void Need(object? v, string what)
        {
            if (v is null) throw new LabException("PayloadIncomplete", $"{type} payload is missing {what}");
        }
        switch (type)
        {
            case EventType.TreeMarked:
            case EventType.TreeUnmarked:
                return "{}";
            case EventType.PlanCreated:
            case EventType.PlanProposed:
            case EventType.PlanQueued:
            case EventType.PlanRejected:
            case EventType.PlanCancelled:
                Need(p.PlanId, "planId");
                Key(sb, "planId", true); Str(sb, p.PlanId!);
                break;
            case EventType.PlanApproved:
                Need(p.PlanId, "planId"); Need(p.BaseForestRevision, "baseForestRevision"); Need(p.ApprovalSessionEpoch, "approvalSessionEpoch");
                Key(sb, "planId", true); Str(sb, p.PlanId!);
                Key(sb, "baseForestRevision"); Int(sb, p.BaseForestRevision!.Value);
                Key(sb, "approvalSessionEpoch"); Str(sb, p.ApprovalSessionEpoch!);
                break;
            case EventType.PlanStale:
            case EventType.PlanExecutionRejected:
                Need(p.PlanId, "planId"); Need(p.Reason, "reason");
                Key(sb, "planId", true); Str(sb, p.PlanId!);
                Key(sb, "reason"); Str(sb, p.Reason!);
                break;
            case EventType.TestWorkExecuted:
                Need(p.PlanId, "planId"); Need(p.ForestRevisionAfter, "forestRevisionAfter");
                Key(sb, "planId", true); Str(sb, p.PlanId!);
                Key(sb, "forestRevisionAfter"); Int(sb, p.ForestRevisionAfter!.Value);
                break;
            case EventType.ExperimentStepAdvanced:
                Need(p.ExperimentStepAfter, "experimentStepAfter"); Need(p.ForestRevisionAfter, "forestRevisionAfter");
                Key(sb, "experimentStepAfter", true); Int(sb, p.ExperimentStepAfter!.Value);
                Key(sb, "forestRevisionAfter"); Int(sb, p.ForestRevisionAfter!.Value);
                break;
            default:
                throw new LabException("UnknownEvent", "Unknown event type");
        }
        return "{" + sb + "}";
    }

    public static string Event(LabEvent e)
    {
        var sb = new StringBuilder();
        Key(sb, "eventSeq", true); Int(sb, e.EventSeq);
        Key(sb, "eventType"); Str(sb, e.Type.ToString());
        Key(sb, "treeIds"); IdArray(sb, e.TreeIds);
        Key(sb, "payload"); sb.Append(Payload(e.Type, e.Payload));
        Key(sb, "prevStateHash"); Str(sb, e.PrevStateHash);
        Key(sb, "postStateHash"); Str(sb, e.PostStateHash);
        return "{" + sb + "}";
    }

    /// <summary>Whole lab file. Root order: format,labSchemaVersion,sourceClass,fixtureId,snapshot,eventLog,snapshotHash.</summary>
    public static byte[] File(LabFileContent f)
    {
        var sb = new StringBuilder();
        Key(sb, "format", true); Str(sb, LabConstants.Format);
        Key(sb, "labSchemaVersion"); Int(sb, LabConstants.SchemaVersion);
        Key(sb, "sourceClass"); Str(sb, LabConstants.SourceClass);
        Key(sb, "fixtureId"); Str(sb, f.FixtureId);
        Key(sb, "snapshot"); sb.Append(Snapshot(f.Snapshot));
        Key(sb, "eventLog"); sb.Append('[');
        for (int i = 0; i < f.Events.Count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(Event(f.Events[i]));
        }
        sb.Append(']');
        Key(sb, "snapshotHash"); Str(sb, f.SnapshotHash);
        return Utf8.GetBytes("{" + sb + "}");
    }
}

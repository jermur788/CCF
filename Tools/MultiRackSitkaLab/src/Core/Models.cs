using System.Text.RegularExpressions;

namespace SitkaLab.Core;

/// <summary>Fixed constants of the P0 lab. Numeric limits are overflow-safety bounds, not biological claims.</summary>
public static class LabConstants
{
    public const string Format = "ccf-sitka-lab";
    public const int SchemaVersion = 1;
    public const string SourceClass = "Synthetic";
    public const string Species = "Sitka";
    public const string FileSuffix = ".sitkalab.json";

    public const string Title = "Synthetic Sitka test stand (flat)";
    public const string SourceBanner = "SYNTHETIC TEST FOREST";
    public const string GrowthBanner = "NO BIOLOGICAL GROWTH";
    public const string ModelName = "P0 deterministic forest-operations engine (no growth model)";
    public const string ModelVersion = "p0-1";
    public const string AdvanceStepLabel = "Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)";

    /// <summary>Largest absolute coordinate / measurement in millimetres (1,000 km); keeps all arithmetic far from overflow.</summary>
    public const long MaxMm = 1_000_000_000;
    public const long MaxYears = 1_000_000;
    public const int MaxTrees = 20_000;
    public const int MaxPolygonVertices = 256;
    public const int MaxPlans = 5_000;
    public const int MaxEvents = 200_000;
    public const long MaxFileBytes = 64L * 1024 * 1024;

    private static readonly Regex SafeId = new("^[A-Za-z0-9_-]{1,64}$", RegexOptions.CultureInvariant);
    private static readonly Regex Hex64 = new("^[0-9a-f]{64}$", RegexOptions.CultureInvariant);
    private static readonly Regex Epoch = new("^[0-9a-f]{8,64}$", RegexOptions.CultureInvariant);

    public static bool IsSafeId(string? value) => value is not null && SafeId.IsMatch(value);
    public static bool IsSha256Hex(string? value) => value is not null && Hex64.IsMatch(value);
    public static bool IsEpoch(string? value) => value is not null && Epoch.IsMatch(value);
}

public readonly record struct Vertex(long XMm, long YMm);

/// <summary>Biological tree status (separate from plan state and mark state).</summary>
public enum TreeStatus { Live, Dead, Felled }

/// <summary>Per-tree mark state (separate from biological status and plan state).</summary>
public enum MarkState { Unmarked, PlayerMarked }

/// <summary>Plan state machine stages.</summary>
public enum PlanStage { Draft, Proposed, Approved, PendingWork, Executed, Rejected, Cancelled, Stale }

public enum EventType
{
    TreeMarked,
    TreeUnmarked,
    PlanCreated,
    PlanProposed,
    PlanApproved,
    PlanQueued,
    PlanRejected,
    PlanCancelled,
    PlanStale,
    TestWorkExecuted,
    PlanExecutionRejected,
    ExperimentStepAdvanced,
}

public sealed class TreeState
{
    public string TreeId = "";
    public string Species = LabConstants.Species;
    public long XMm;
    public long YMm;
    public long ZMm;
    public long DbhMm;
    public long HeightMm;
    public long AgeAtInventoryYears;
    public TreeStatus Status = TreeStatus.Live;
    public MarkState Mark = MarkState.Unmarked;

    public TreeState Clone() => (TreeState)MemberwiseClone();
}

public sealed class PlanRecord
{
    public string PlanId = "";
    public PlanStage Stage;
    /// <summary>Exact unique IDs in ordinal order (never list indices).</summary>
    public List<string> TreeIds = new();
    public long? BaseForestRevision;
    public string? ApprovalSessionEpoch;

    public bool IsOpen => Stage is PlanStage.Draft or PlanStage.Proposed or PlanStage.Approved or PlanStage.PendingWork;

    public PlanRecord Clone() => new()
    {
        PlanId = PlanId,
        Stage = Stage,
        TreeIds = new List<string>(TreeIds),
        BaseForestRevision = BaseForestRevision,
        ApprovalSessionEpoch = ApprovalSessionEpoch,
    };
}

/// <summary>The one canonical forest authority (snapshot part). Trees ordinal-sorted by ID, plans ordinal-sorted by ID.</summary>
public sealed class ForestSnapshot
{
    public string ForestId = "";
    public long InventoryYear;
    public long ExperimentStep;
    public long ForestRevision;
    public long AdultThresholdMm;
    public List<Vertex> Polygon = new();
    public long SlopeDeg;
    public List<TreeState> Trees = new();
    public List<PlanRecord> Plans = new();

    public ForestSnapshot Clone() => new()
    {
        ForestId = ForestId,
        InventoryYear = InventoryYear,
        ExperimentStep = ExperimentStep,
        ForestRevision = ForestRevision,
        AdultThresholdMm = AdultThresholdMm,
        Polygon = new List<Vertex>(Polygon),
        SlopeDeg = SlopeDeg,
        Trees = Trees.Select(t => t.Clone()).ToList(),
        Plans = Plans.Select(p => p.Clone()).ToList(),
    };

    public TreeState? FindTree(string id)
    {
        int lo = 0, hi = Trees.Count - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) >>> 1;
            int c = string.CompareOrdinal(Trees[mid].TreeId, id);
            if (c == 0) return Trees[mid];
            if (c < 0) lo = mid + 1; else hi = mid - 1;
        }
        return null;
    }

    public PlanRecord? FindPlan(string id) => Plans.FirstOrDefault(p => p.PlanId == id);

    public bool IsAdult(TreeState t) => t.Status == TreeStatus.Live && t.DbhMm >= AdultThresholdMm;
}

public sealed class EventPayload
{
    public string? PlanId;
    public long? BaseForestRevision;
    public string? ApprovalSessionEpoch;
    public string? Reason;
    public long? ForestRevisionAfter;
    public long? ExperimentStepAfter;
}

public sealed class LabEvent
{
    public long EventSeq;
    public EventType Type;
    public List<string> TreeIds = new();
    public EventPayload Payload = new();
    public string PrevStateHash = "";
    public string PostStateHash = "";
}

/// <summary>Everything stored in a lab file (checked-in fixture or lab save).</summary>
public sealed class LabFileContent
{
    public string FixtureId = "";
    public ForestSnapshot Snapshot = new();
    public List<LabEvent> Events = new();
    public string SnapshotHash = "";
}

/// <summary>A rejected input / state; Code is a stable machine-readable token.</summary>
public sealed class LabException : Exception
{
    public string Code { get; }
    public LabException(string code, string message) : base(message) { Code = code; }
}

public sealed record OpResult(bool Ok, string Code, string Message, string? PlanId = null, IReadOnlyList<string>? TreeIds = null)
{
    public static OpResult Success(string? planId = null, string message = "ok") => new(true, "ok", message, planId, null);
    public static OpResult Fail(string code, string message, IReadOnlyList<string>? ids = null, string? planId = null) =>
        new(false, code, message, planId, ids);
}

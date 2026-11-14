using System.Text;
using System.Text.Json;

namespace SitkaLab.Core;

// Display DTOs. Every number the browser shows comes from here; the browser only formats and draws.

/// <summary>Position in metres (flat site, local plot coordinates).</summary>
public sealed record PointM(double XMetres, double YMetres);

public sealed record MetricsView(
    double AreaM2, double AreaHa, int EligibleLiveN, double NPerHa, double BaM2, double BaM2PerHa, double? DqCm,
    int LiveTotal, int LiveBelowThreshold, int Dead, int Felled);

public sealed record PreviewView(
    bool Valid, IReadOnlyList<string> InvalidIds, string? InvalidReason, int RemovedN, double RemovedBaM2, double? RemovedBaFractionOfLiveStand,
    MetricsView Before, MetricsView After);

public sealed record TreeView(
    string TreeId, string Species, double XMetres, double YMetres, double DbhCm, double HeightM, long AgeAtInventoryYears,
    string Status, string MarkState, bool BelowThreshold, bool Eligible, string? PlanId, string? PlanState);

public sealed record PlanActions(bool CanPropose, bool CanApprove, bool CanQueue, bool CanExecute, bool CanReject, bool CanCancel);

public sealed record PlanView(
    string PlanId, string PlanState, IReadOnlyList<string> TreeIds, long? BaseForestRevision, string? ApprovalSessionEpoch,
    PreviewView? Preview, PlanActions Actions);

public sealed record EventView(long EventSeq, string EventType, IReadOnlyList<string> TreeIds, string? PlanId, string? Reason, string PrevStateHash, string PostStateHash);

public sealed record SessionView(
    string Title, string SourceBanner, string GrowthBanner, string ModelName, string ModelVersion, string AdvanceStepLabel,
    string ForestId, string FixtureId, long InventoryYear, long ExperimentStep, long ForestRevision, long AdultThresholdMm, long SlopeDeg,
    string SessionEpoch, string StateHash, int EventCount,
    IReadOnlyList<PointM> PolygonMetres, double MinXMetres, double MinYMetres, double MaxXMetres, double MaxYMetres,
    MetricsView Metrics, IReadOnlyList<TreeView> Trees, IReadOnlyList<PlanView> Plans, IReadOnlyList<EventView> RecentEvents);

public static class Views
{
    public static MetricsView ToView(StandMetrics m) => new(
        (double)m.AreaM2, (double)m.AreaHa, m.EligibleLiveN, m.NPerHa, m.BaM2, m.BaM2PerHa, m.DqCm,
        m.LiveTotal, m.LiveBelowThreshold, m.Dead, m.Felled);

    public static PreviewView ToView(RemovalPreview p) => new(
        p.Valid, p.InvalidIds, p.InvalidReason, p.RemovedN, p.RemovedBaM2, p.RemovedBaFractionOfLiveStand, ToView(p.Before), ToView(p.After));

    public static EventView ToView(LabEvent e) =>
        new(e.EventSeq, e.Type.ToString(), e.TreeIds, e.Payload.PlanId, e.Payload.Reason, e.PrevStateHash, e.PostStateHash);

    public static SessionView Build(LabSession session, int recentEventCount = 12)
    {
        var s = session.State;
        var planOf = new Dictionary<string, PlanRecord>(StringComparer.Ordinal);
        foreach (var p in s.Plans)
            if (p.IsOpen) foreach (var id in p.TreeIds) planOf[id] = p;

        var trees = new List<TreeView>(s.Trees.Count);
        foreach (var t in s.Trees)
        {
            planOf.TryGetValue(t.TreeId, out var plan);
            trees.Add(new TreeView(
                t.TreeId, t.Species, t.XMm / 1000.0, t.YMm / 1000.0, t.DbhMm / 10.0, t.HeightMm / 1000.0, t.AgeAtInventoryYears,
                Wire.Name(t.Status), Wire.Name(t.Mark),
                t.Status == TreeStatus.Live && t.DbhMm < s.AdultThresholdMm, s.IsAdult(t),
                plan?.PlanId, plan is null ? null : Wire.Name(plan.Stage)));
        }

        var plans = new List<PlanView>(s.Plans.Count);
        foreach (var p in s.Plans)
        {
            PreviewView? preview = p.IsOpen ? ToView(Metrics.PreviewRemoval(s, p.TreeIds)) : null;
            bool exec = p.Stage == PlanStage.PendingWork && Engine.CheckExecute(s, p, session.SessionEpoch) is null;
            plans.Add(new PlanView(p.PlanId, Wire.Name(p.Stage), p.TreeIds, p.BaseForestRevision, p.ApprovalSessionEpoch, preview,
                new PlanActions(p.Stage == PlanStage.Draft, p.Stage == PlanStage.Proposed, p.Stage == PlanStage.Approved, exec,
                    p.Stage == PlanStage.Proposed, p.IsOpen)));
        }

        var poly = s.Polygon.Select(v => new PointM(v.XMm / 1000.0, v.YMm / 1000.0)).ToList();
        var ev = session.Events;
        var recent = ev.Skip(Math.Max(0, ev.Count - recentEventCount)).Select(ToView).ToList();
        return new SessionView(
            LabConstants.Title, LabConstants.SourceBanner, LabConstants.GrowthBanner, LabConstants.ModelName, LabConstants.ModelVersion, LabConstants.AdvanceStepLabel,
            s.ForestId, session.FixtureId, s.InventoryYear, s.ExperimentStep, s.ForestRevision, s.AdultThresholdMm, s.SlopeDeg,
            session.SessionEpoch, session.StateHash, ev.Count,
            poly, poly.Min(p => p.XMetres), poly.Min(p => p.YMetres), poly.Max(p => p.XMetres), poly.Max(p => p.YMetres),
            ToView(Metrics.Compute(s)), trees, plans, recent);
    }

    /// <summary>Provenance manifest + full event log + stand summary, as a self-describing JSON document.</summary>
    public static string ExportJson(LabSession session)
    {
        var s = session.State;
        var m = Metrics.Compute(s);
        using var ms = new MemoryStream();
        using (var w = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
        {
            w.WriteStartObject();
            w.WriteString("exportKind", "ccf-sitka-lab-provenance-export");
            w.WriteString("title", LabConstants.Title);
            w.WriteString("banner", LabConstants.SourceBanner + " — " + LabConstants.GrowthBanner);
            w.WriteStartObject("manifest");
            w.WriteString("format", LabConstants.Format);
            w.WriteNumber("labSchemaVersion", LabConstants.SchemaVersion);
            w.WriteString("sourceClass", LabConstants.SourceClass);
            w.WriteString("fixtureId", session.FixtureId);
            w.WriteString("forestId", s.ForestId);
            w.WriteString("snapshotHash", session.StateHash);
            w.WriteString("model", LabConstants.ModelName + " " + LabConstants.ModelVersion);
            w.WriteString("evidenceClass", "ENGINEERING TEST FIXTURE — not a measured Irish stand; no biological growth");
            w.WriteNumber("experimentStep", s.ExperimentStep);
            w.WriteNumber("forestRevision", s.ForestRevision);
            w.WriteNumber("eventCount", session.Events.Count);
            w.WriteEndObject();
            w.WriteStartObject("standSummary");
            w.WriteNumber("areaM2", (double)m.AreaM2);
            w.WriteNumber("areaHa", (double)m.AreaHa);
            w.WriteNumber("eligibleLiveAdultN", m.EligibleLiveN);
            w.WriteNumber("nPerHa", m.NPerHa);
            w.WriteNumber("baM2", m.BaM2);
            w.WriteNumber("baM2PerHa", m.BaM2PerHa);
            if (m.DqCm is double dq) w.WriteNumber("dqCm", dq); else w.WriteNull("dqCm");
            w.WriteNumber("liveBelowThreshold", m.LiveBelowThreshold);
            w.WriteNumber("dead", m.Dead);
            w.WriteNumber("felled", m.Felled);
            w.WriteEndObject();
            w.WriteStartArray("eventLog");
            foreach (var e in session.Events)
            {
                using var doc = JsonDocument.Parse(Encoding.UTF8.GetBytes(CanonicalJson.Event(e)));
                doc.RootElement.WriteTo(w);
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }
}

using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

/// <summary>Shared fixtures and builders for the headless tests.</summary>
internal static class Fx
{
    public static readonly FixtureCatalog Catalog = FixtureCatalog.LoadFrom(ToolPaths.FixtureDir);

    public static LabSession Session(string fixtureId) => LabSession.FromFixture(Catalog.TryGet(fixtureId) ?? throw new InvalidOperationException(fixtureId));
    public static LabSession Oracle() => Session("oracle-quarter-ha");
    public static LabSession Demo() => Session("sitka-flat-demo-v1");

    public static TreeState Tree(string id, long x, long y, long dbhMm, TreeStatus st = TreeStatus.Live, long heightMm = 10_000, long age = 28) =>
        new() { TreeId = id, XMm = x, YMm = y, ZMm = 0, DbhMm = dbhMm, HeightMm = heightMm, AgeAtInventoryYears = age, Status = st };

    public static ForestSnapshot Square(long sideMm, params TreeState[] trees) => new()
    {
        ForestId = "unit-square", InventoryYear = 2026, AdultThresholdMm = 100,
        Polygon = { new(0, 0), new(sideMm, 0), new(sideMm, sideMm), new(0, sideMm) },
        Trees = trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList(),
    };

    public static ForestSnapshot WithPolygon(params (long x, long y)[] pts) => new()
    {
        ForestId = "poly-test", InventoryYear = 2026, AdultThresholdMm = 100,
        Polygon = pts.Select(p => new Vertex(p.x, p.y)).ToList(),
    };

    /// <summary>First <paramref name="n"/> eligible live adult IDs in ordinal order, skipping any in <paramref name="skip"/>.</summary>
    public static List<string> Adults(LabSession s, int n, int skip = 0) =>
        s.State.Trees.Where(t => s.State.IsAdult(t) && t.Mark == MarkState.Unmarked).Skip(skip).Take(n).Select(t => t.TreeId).ToList();

    public static string Plan(LabSession s, IReadOnlyList<string> ids, PlanStage upTo)
    {
        Ok(s.MarkTrees(ids));
        var created = s.CreatePlan(ids); Ok(created);
        string id = created.PlanId!;
        if (upTo >= PlanStage.Proposed) Ok(s.ProposePlan(id));
        if (upTo >= PlanStage.Approved) Ok(s.ApprovePlan(id));
        if (upTo >= PlanStage.PendingWork) Ok(s.QueuePlan(id));
        return id;
    }

    public static void Ok(OpResult r) => T.True(r.Ok, $"operation failed: {r.Code}: {r.Message}");
    public static void Fails(OpResult r, string code, string msg) { T.False(r.Ok, msg + " should have failed"); T.Eq(code, r.Code, msg + " code"); }

    public static PlanRecord PlanOf(LabSession s, string id) => s.State.FindPlan(id) ?? throw new InvalidOperationException("no plan " + id);

    public static string Snapshot(LabSession s) => CanonicalJson.Snapshot(s.State);
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A proposed extraction rack: where its corridor lies, which trees it would take, and what taking them would be worth.
// Transient by design. A proposal is only a preview; nothing changes until ScenarioOneManager.ConfirmRack turns it into
// ordinary felling orders in the Work Plan, so cancelling costs nothing and there is no rack record to save.
public sealed class RackProposal
{
    public RackSelection Selection;
    public float AngleDegrees;
    public float WidthM => Selection.WidthM;
    public int TreeCount => Selection.Trees.Count;
    // Biological stem volume of the selected trees, and the part of it that a harvest sells as product.
    public float StemVolumeM3, SoldVolumeM3;
    // The estimate is the harvest quote for this rack on its own (one contractor visit); the real quote is for everything planned
    // for the year, so a small rack carries the whole visit minimum here and shares it once other work is planned.
    public long RevenueCents, CostCents;
    public long NetCents => RevenueCents - CostCents;
    public string Problem = "";
    // 0-based index of the planned layout rack this one lies on, or -1; LayoutCount is the number of racks in that layout.
    public int LayoutIndex = -1, LayoutCount;
    public IEnumerable<string> TreeIds => Selection.Trees.Select(t => t.Id);
    public bool CanConfirm => TreeCount > 0 && string.IsNullOrEmpty(Problem);
}

// Builds rack proposals from the standing forest. The in-world rack tool and the Stand Map both call this, so a rack means
// the same thing wherever it was planned. It reads trees and the Work Plan and writes nothing.
public sealed class RackPlanner
{
    // A pointed position is moved onto the nearest planted row only when it is this close to one (rows are 1.9 m apart in Scenario
    // One), so a rack aimed at open ground between rows is not dragged to a distant row.
    private const float SnapReachM = 1.7f;

    private readonly ScenarioOneManager manager;
    private readonly ForestEcologyController ecology;
    private readonly List<RackTree> trees = new List<RackTree>();
    private readonly Dictionary<string, float> volumeById = new Dictionary<string, float>();
    private readonly List<RackPoint> plantation = new List<RackPoint>();
    private readonly Dictionary<int, List<RackRow>> rowCache = new Dictionary<int, List<RackRow>>();
    private List<float> layoutLines = new List<float>();
    private string quoteKey = "";
    private RackProposal quoted;
    private float angleOfRows;
    // The planted rows do not change while the stand does not (stumps keep their place), so the row angle and the layout are worked
    // out again only when the number of planted trees or the stand geometry changes.
    private int plantationStamp = -1;

    public RackPlanner(ScenarioOneManager manager, ForestEcologyController ecology)
    {
        this.manager = manager;
        this.ecology = ecology;
    }

    // Half the side of the square stand: the rack ends where its line leaves the stand.
    public float HalfExtentM => ecology != null ? StandGeometryModel.StandSizeMeters(ecology.StandGeometryModelVersion) * 0.5f : 40f;

    // One of the two angles (90 degrees apart) along which the planted rows run.
    public float RowAngle => angleOfRows;

    // Centre lines (across coordinate at angle 0) of the planned Scenario One rack layout; empty when the stand has none.
    public IReadOnlyList<float> LayoutLines => layoutLines;

    // Reads the standing trees and the planned felling orders again. Cheap enough for a few times a second.
    public void Refresh()
    {
        trees.Clear();
        volumeById.Clear();
        plantation.Clear();
        rowCache.Clear();
        quoteKey = "";
        quoted = null;
        var planned = new HashSet<string>(System.StringComparer.Ordinal);
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
            if (order.IsOpen && order.type == ScenarioWorkType.FellTree && !string.IsNullOrEmpty(order.targetTreeId))
                planned.Add(order.targetTreeId);
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId))
                continue;
            Vector3 p = tree.transform.position;
            // Stumps keep their place in the planting rows, so the rows (and the planned layout on them) stay put after a harvest.
            if (tree.TreeId.StartsWith("P", System.StringComparison.Ordinal))
                plantation.Add(new RackPoint(p.x, p.z));
            if (!tree.IsLiving)
                continue;
            trees.Add(new RackTree(tree.TreeId, p.x, p.z, tree.CanChop, tree.IsCropTree, planned.Contains(tree.TreeId)));
            volumeById[tree.TreeId] = tree.BiologicalStemVolumeM3;
        }
        int stamp = plantation.Count * 7 + (ecology != null ? ecology.StandGeometryModelVersion : 0);
        if (stamp != plantationStamp)
        {
            plantationStamp = stamp;
            angleOfRows = plantation.Count > 0 ? RackRows.DominantAngle(plantation) : 0f;
            layoutLines = ecology != null && ScenarioOneRackLayout.AppliesTo(ecology.StandGeometryModelVersion)
                ? ScenarioOneRackLayout.Centres(plantation) : new List<float>();
        }
    }

    // The planted rows read across an angle (centre line coordinates, in order), for stepping a rack from one row to the next.
    public IReadOnlyList<RackRow> Rows(float angleDegrees) => RowsAt(angleDegrees);

    private List<RackRow> RowsAt(float angle)
    {
        int key = Mathf.RoundToInt(RackGeometry.NormaliseAngle(angle) * 10f);
        if (!rowCache.TryGetValue(key, out List<RackRow> rows))
        {
            rows = RackRows.Find(plantation.Count > 0 ? plantation : trees.Select(t => new RackPoint(t.X, t.Z)).ToList(), angle);
            rowCache[key] = rows;
        }
        return rows;
    }

    // The direction a new rack starts in: along the planned layout while one is still to do, otherwise the planted row direction
    // that is closest to where the player is facing (a rack runs away from you, not across your view).
    public float DefaultAngle(float facingDegrees, bool followLayout)
    {
        if (followLayout && layoutLines.Count > 0)
            return 0f;
        float a = RackGeometry.NormaliseAngle(angleOfRows), b = RackGeometry.NormaliseAngle(angleOfRows + 90f);
        return AngularDistance(a, facingDegrees) <= AngularDistance(b, facingDegrees) ? a : b;
    }

    private static float AngularDistance(float lineAngle, float facing)
    {
        float d = Mathf.Abs(RackGeometry.NormaliseAngle(lineAngle) - RackGeometry.NormaliseAngle(facing));
        return Mathf.Min(d, 180f - d);
    }

    // The row line the proposal snaps to when it is placed at this point, as the across coordinate at the given angle.
    public bool SnapAcross(RackPoint point, float angle, out float acrossSnapped)
    {
        float across = RackGeometry.Across(point, angle);
        List<RackRow> rows = RowsAt(angle);
        int i = RackRows.Nearest(rows, across);
        if (i >= 0 && Mathf.Abs(rows[i].Centre - across) <= SnapReachM)
        {
            acrossSnapped = rows[i].Centre;
            return true;
        }
        acrossSnapped = across;
        return false;
    }

    // A rack through (or nearest to) a pointed position at an angle, boundary to boundary of the stand.
    public RackProposal Propose(RackPoint point, float angleDegrees, float widthM = RackGeometry.DefaultWidthM, bool snapToRow = true)
    {
        float angle = RackGeometry.NormaliseAngle(angleDegrees);
        float across = RackGeometry.Across(point, angle);
        if (snapToRow) SnapAcross(point, angle, out across);
        RackPoint d = RackGeometry.Direction(angle);
        float along = RackGeometry.Along(point, angle);
        var through = new RackPoint(d.X * along - d.Z * across, d.Z * along + d.X * across);
        var proposal = new RackProposal { AngleDegrees = angle, LayoutCount = layoutLines.Count };
        if (!RackGeometry.ClipLineToSquare(through, angle, HalfExtentM, out RackPoint start, out RackPoint end))
        {
            proposal.Selection = new RackSelection { Start = through, End = through, WidthM = widthM };
            proposal.Problem = "That line does not cross the stand.";
            return proposal;
        }
        return ProposeBetween(start, end, widthM, angle, across, proposal);
    }

    // Bounded geometry: a rack between two explicit points. Scenario One uses boundary-to-boundary racks (Propose); a later
    // scenario can call this directly for a rack that covers part of a stand.
    public RackProposal ProposeBetween(RackPoint start, RackPoint end, float widthM = RackGeometry.DefaultWidthM)
    {
        float dx = end.X - start.X, dz = end.Z - start.Z;
        float angle = RackGeometry.NormaliseAngle(Mathf.Atan2(dz, dx) * Mathf.Rad2Deg);
        float across = RackGeometry.Across(start, angle);
        return ProposeBetween(start, end, widthM, angle, across, new RackProposal { AngleDegrees = angle, LayoutCount = layoutLines.Count });
    }

    // Forest placement uses the already supported bounded segment, with the chosen origin unchanged (no row snapping).
    // The map's whole-row proposals, width, authored layout, quoting and felling eligibility remain the same.
    public RackProposal ProposeFromOrigin(RackPoint origin, float directionDegrees)
    {
        if (RackGeometry.ClipForwardToSquare(origin, directionDegrees, HalfExtentM, out RackPoint end))
            return ProposeBetween(origin, end);
        return new RackProposal
        {
            Selection = new RackSelection { Start = origin, End = origin, WidthM = RackGeometry.DefaultWidthM },
            Problem = "Choose a starting point inside the woodland and aim into the stand."
        };
    }

    private RackProposal ProposeBetween(RackPoint start, RackPoint end, float widthM, float angle, float across, RackProposal proposal)
    {
        proposal.Selection = RackGeometry.Select(trees, start, end, widthM);
        if (proposal.Selection.Trees.Count == 0)
        {
            proposal.Problem = proposal.Selection.AlreadyPlanned > 0 ? "This rack is already in the Work Plan."
                : "No tree on this line can be felled.";
            return proposal;
        }
        proposal.LayoutIndex = LayoutIndexOf(angle, across);
        foreach (RackTree t in proposal.Selection.Trees)
            proposal.StemVolumeM3 += volumeById.TryGetValue(t.Id, out float v) ? v : 0f;
        // The quote only depends on which trees are selected, so a preview that is aimed steadily is quoted once.
        string key = string.Join(",", proposal.Selection.Trees.Select(t => t.Id));
        if (key != quoteKey || quoted == null)
        {
            quoted = new RackProposal();
            ScenarioHarvestJob job = manager.QuoteTrees(proposal.Selection.Trees.Select(t => t.Id));
            quoted.SoldVolumeM3 = job.SoldVolumeCm3 / 1000000f;
            quoted.RevenueCents = job.RevenueCents;
            quoted.CostCents = job.CostCents;
            quoted.Problem = job.Orders.Count == 0 ? "No tree on this line can be quoted." : job.Eligible ? "" : job.Problem;
            quoteKey = key;
        }
        proposal.SoldVolumeM3 = quoted.SoldVolumeM3;
        proposal.RevenueCents = quoted.RevenueCents;
        proposal.CostCents = quoted.CostCents;
        proposal.Problem = quoted.Problem;
        return proposal;
    }

    // Which planned layout rack this angle and line lie on (the layout runs along the rows at angle 0), or -1.
    public int LayoutIndexOf(float angle, float across)
    {
        float a = RackGeometry.NormaliseAngle(angle);
        if (layoutLines.Count == 0 || (a > 0.5f && a < 179.5f))
            return -1;
        for (int i = 0; i < layoutLines.Count; i++)
            if (Mathf.Abs(layoutLines[i] - across) <= 0.95f)
                return i;
        return -1;
    }

    // How much of each planned layout rack is still to do: living, fellable, non-Crop trees in its corridor that are not yet in
    // the Work Plan. A rack with none left is done: planned, or already harvested (its stumps are not trees to fell).
    public List<RackLayoutLine> LayoutStatus()
    {
        var lines = new List<RackLayoutLine>();
        for (int i = 0; i < layoutLines.Count; i++)
        {
            var through = new RackPoint(0f, layoutLines[i]);
            RackGeometry.ClipLineToSquare(through, 0f, HalfExtentM, out RackPoint start, out RackPoint end);
            RackSelection selection = RackGeometry.Select(trees, start, end, RackGeometry.DefaultWidthM);
            int inCorridor = trees.Count(t => Mathf.Abs(t.Z - layoutLines[i]) <= RackGeometry.DefaultWidthM * 0.5f && t.CanBeFelled && !t.IsCropTree);
            lines.Add(new RackLayoutLine
            {
                Index = i,
                AcrossM = layoutLines[i],
                Start = start,
                End = end,
                Remaining = selection.Trees.Count,
                Planned = selection.AlreadyPlanned,
                Done = selection.Trees.Count == 0,
                TreesLeft = inCorridor,
                RemainingTreeIds = selection.Trees.Select(tree => tree.Id).ToList()
            });
        }
        return lines;
    }
}

// One planned layout rack and how far it has got.
public sealed class RackLayoutLine
{
    // Read-only presentation evidence from the same selection that supplies Remaining; never saved or used to qualify work.
    public List<string> RemainingTreeIds = new List<string>();
    public int Index;
    public float AcrossM;
    public RackPoint Start, End;
    // Trees on the line that are not planned yet, trees on it that are planned, and whether none are left to plan.
    public int Remaining, Planned;
    public bool Done;
    // Living fellable trees still standing in the corridor (planned or not): zero once the rack has been harvested.
    public int TreesLeft;
}

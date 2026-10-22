using System;
using System.Collections.Generic;

// Scenario One teaching Objectives 2-5 are one practical lesson on ONE rack and the standing trees beside it: the "rack area". This is
// the rules for it, on plain data (no UnityEngine), so they can be checked on plain .NET (Tools/Verification/ScenarioOneNonUi) and give
// the same answer everywhere. ScenarioOneRackBayReader feeds it the live trees and Work Plan.
//
// Nothing here is saved and nothing here changes the forest. The lesson state is DERIVED from the trees, their marks and the work
// orders every time, exactly as the earlier teaching sequence was, so a save and load always lands on the same lesson.
//
// What is a forestry fact and what is not (see Docs/Design/ScenarioOneRackBayLessonV1.md):
//   * the Hegyi competition term, its 8 m reach and the competitor shares are the game's existing simulation signals, used unchanged;
//   * a "future tree" is the player's own Crop Tree mark; positive selection of vigorous trees with room to grow is established
//     practice, but the numbers below (how many, how far apart, how much release) are simplified TEACHING CALIBRATION [D], not
//     silvicultural prescriptions. They only decide when the lesson moves on; they never gate the scenario or change any rule.
public static class ScenarioOneRackBay
{
    // ---- Teaching calibration [D] ----------------------------------------------------------------------------------------------
    // Future trees the area needs: three show spacing and shared competitors while still fitting one area.
    public const int FutureTreesWanted = 3;
    // Future trees further apart than the competition reach do not compete with each other, so each has growing space of its own.
    public const float FutureTreeSpacingM = ForestEcologyController.HegyiCutoffMeters;
    // A future tree is "worth retaining" when at least this share of its competitors is no bigger than itself (not overtopped).
    public const float DominantShare = 0.5f;
    // The first release takes at least this share of each future tree's competition; the further release at least this share in all.
    public const float FirstReleaseShare = 0.30f;
    public const float FurtherReleaseShare = 0.40f;
    // Removed trees may take at most this share of the area's basal area: the area stays a forest (retained canopy), not a clearing.
    public const float MaxRemovedBasalShare = 0.40f;
    // Once trees have actually been felled, the surviving neighbours keep growing while a stump keeps the diameter it was felled at, so
    // the removed share slowly thins out after the work. A finished release is judged with this much give so growth never un-finishes it.
    public const float ExecutedTolerance = 0.9f;
    // The area runs this far either side of the stand centre along the rack, and half the distance to the neighbouring racks across it.
    public const float HalfLengthM = 12f;
    public const float DefaultHalfWidthM = 7.6f;

    public enum Disposition { Sold, Deadwood, Other }

    // One tree as the area sees it. A stump (Living false) still has the diameter it was felled at.
    public readonly struct Tree
    {
        public readonly string Id;
        public readonly float X, Z, DbhCm;
        public readonly bool Living, Crop;
        // A Fell mark that is not yet an order in the Work Plan: shown to the player as progress, never counted as planned work.
        public readonly bool Marked;
        public Tree(string id, float x, float z, float dbhCm, bool living, bool crop, bool marked = false)
        { Id = id ?? ""; X = x; Z = z; DbhCm = dbhCm; Living = living; Crop = crop; Marked = marked; }
    }

    // One felling order. Open = waiting in the Work Plan (pending or approved, nothing blocking it); otherwise completed.
    public readonly struct Order
    {
        public readonly string TargetId;
        public readonly bool Open, Completed;
        public readonly Disposition Disposition;
        public readonly int ResolvedYear;
        public Order(string targetId, bool open, bool completed, Disposition disposition, int resolvedYear)
        { TargetId = targetId ?? ""; Open = open; Completed = completed; Disposition = disposition; ResolvedYear = resolvedYear; }
    }

    public readonly struct FutureTree
    {
        public readonly string Id;
        public readonly float X, Z, DbhCm;
        // The share of this tree's competition (summed Hegyi terms of its neighbours within 8 m, including trees already removed
        // for it) that is in the Work Plan, already removed, and only marked so far.
        public readonly float PlannedShare, ExecutedShare, MarkedShare;
        public float ReleaseShare => PlannedShare + ExecutedShare;
        public float ReleaseWithMarksShare => PlannedShare + ExecutedShare + MarkedShare;
        public FutureTree(string id, float x, float z, float dbhCm, float planned, float executed, float marked)
        { Id = id; X = x; Z = z; DbhCm = dbhCm; PlannedShare = planned; ExecutedShare = executed; MarkedShare = marked; }
    }

    public sealed class Analysis
    {
        // False when the stand has no rack layout (a 40 m legacy stand): the earlier simple rules apply there.
        public bool Applies;
        public int RackIndex, RackCount;
        public float CentreZ, HalfWidthM;
        public float MinX => -HalfLengthM;
        public float MaxX => HalfLengthM;

        // Crop Trees in the area, sorted into what counts and why the others do not.
        public readonly List<FutureTree> Future = new List<FutureTree>();
        public readonly List<string> TooClose = new List<string>();
        public readonly List<string> Overtopped = new List<string>();
        public int CropElsewhere;

        public int FutureCount => Future.Count;
        public bool HasEnoughFuture => Future.Count >= FutureTreesWanted;

        public float RemovedBasalShare;
        public bool CanopyKept => RemovedBasalShare <= MaxRemovedBasalShare + 1e-6f;

        // Removals serving the future trees (felled for them) by what the player chose for the wood.
        public int SoldExecuted, DeadwoodExecuted, SoldPlanned, DeadwoodPlanned;
        public int ExecutedRemovals, PlannedRemovals, MarkedRemovals;
        // The year the latest such removal was carried out, or -1.
        public int ReleaseYear = -1;

        // The least released future tree, by what is in the Work Plan or done, only done, and with loose marks added.
        public float MinPlannedAndExecuted, MinExecuted, MinWithMarks;

        public bool ReleasePlanned, ReleaseExecuted, FurtherPlanned, FurtherExecuted, MixedPlanned, MixedExecuted;
    }

    // The area for a stand with these rack centre lines (the z coordinate, rows run east-west): the middle rack, and the strip beside it.
    public static Analysis Analyse(IReadOnlyList<float> rackLines, IReadOnlyList<Tree> trees, IReadOnlyList<Order> orders)
    {
        var result = new Analysis();
        if (rackLines == null || rackLines.Count == 0 || trees == null)
            return result;
        var lines = new List<float>(rackLines);
        lines.Sort();
        result.Applies = true;
        result.RackCount = lines.Count;
        result.RackIndex = lines.Count / 2;
        result.CentreZ = lines[result.RackIndex];
        result.HalfWidthM = HalfWidth(lines, result.RackIndex);
        float corridor = RackGeometry.DefaultWidthM * 0.5f;

        bool InCorridor(float z) { foreach (float line in lines) if (Math.Abs(z - line) <= corridor) return true; return false; }
        bool InArea(float x, float z) => Math.Abs(x) <= HalfLengthM && Math.Abs(z - result.CentreZ) <= result.HalfWidthM && !InCorridor(z);

        // Orders by target: what became of each tree.
        var completed = new Dictionary<string, Order>(StringComparer.Ordinal);
        var open = new Dictionary<string, Order>(StringComparer.Ordinal);
        if (orders != null)
            foreach (Order order in orders)
            {
                if (order.Completed) completed[order.TargetId] = order;
                else if (order.Open) open[order.TargetId] = order;
            }

        // Living trees; felled trees that count as removals (not the rack corridors: that was the access, not the release).
        var living = new List<Tree>();
        var removed = new List<Tree>();
        var plannedIds = new HashSet<string>(StringComparer.Ordinal);
        var markedIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (Tree tree in trees)
        {
            if (tree.Living)
            {
                living.Add(tree);
                if (open.ContainsKey(tree.Id)) plannedIds.Add(tree.Id);
                else if (tree.Marked) markedIds.Add(tree.Id);
            }
            else if (completed.ContainsKey(tree.Id) && !InCorridor(tree.Z))
                removed.Add(tree);
        }

        // Which Crop Trees count as future trees: in the area, not overtopped, and clear of each other. Biggest first, then by id, so
        // the answer never depends on the order the trees were listed in.
        var crops = new List<Tree>();
        foreach (Tree tree in living)
            if (tree.Crop)
            {
                if (InArea(tree.X, tree.Z)) crops.Add(tree);
                else result.CropElsewhere++;
            }
        crops.Sort((a, b) => { int c = b.DbhCm.CompareTo(a.DbhCm); return c != 0 ? c : string.CompareOrdinal(a.Id, b.Id); });
        var accepted = new List<Tree>();
        foreach (Tree crop in crops)
        {
            if (!NotOvertopped(crop, living)) { result.Overtopped.Add(crop.Id); continue; }
            bool clear = true;
            foreach (Tree other in accepted)
                if (Distance(crop, other) < FutureTreeSpacingM) { clear = false; break; }
            if (clear) accepted.Add(crop);
            else result.TooClose.Add(crop.Id);
        }

        // Each future tree's competition and how much of it is gone or going.
        result.MinPlannedAndExecuted = result.MinExecuted = result.MinWithMarks = accepted.Count == 0 ? 0f : float.MaxValue;
        foreach (Tree future in accepted)
        {
            double total = 0, planned = 0, executed = 0, marked = 0;
            foreach (Tree other in living)
            {
                if (other.Id == future.Id) continue;
                float d = Distance(future, other);
                if (d > ForestEcologyController.HegyiCutoffMeters) continue;
                double term = ForestEcologyController.HegyiTerm(other.DbhCm, future.DbhCm, d);
                total += term;
                if (plannedIds.Contains(other.Id)) planned += term;
                else if (markedIds.Contains(other.Id)) marked += term;
            }
            foreach (Tree gone in removed)
            {
                float d = Distance(future, gone);
                if (d > ForestEcologyController.HegyiCutoffMeters) continue;
                double term = ForestEcologyController.HegyiTerm(gone.DbhCm, future.DbhCm, d);
                total += term;
                executed += term;
            }
            float p = total > 0 ? (float)(planned / total) : 0f, e = total > 0 ? (float)(executed / total) : 0f, m = total > 0 ? (float)(marked / total) : 0f;
            result.Future.Add(new FutureTree(future.Id, future.X, future.Z, future.DbhCm, p, e, m));
            result.MinPlannedAndExecuted = Math.Min(result.MinPlannedAndExecuted, p + e);
            result.MinExecuted = Math.Min(result.MinExecuted, e);
            result.MinWithMarks = Math.Min(result.MinWithMarks, p + e + m);
        }

        // Removals made for the future trees: what they were, what the player chose for the wood, and when they were carried out.
        foreach (Tree gone in removed)
        {
            if (!ServesAFutureTree(gone, accepted)) continue;
            Order order = completed[gone.Id];
            result.ExecutedRemovals++;
            if (order.Disposition == Disposition.Sold) result.SoldExecuted++;
            else if (order.Disposition == Disposition.Deadwood) result.DeadwoodExecuted++;
            if (order.ResolvedYear > result.ReleaseYear) result.ReleaseYear = order.ResolvedYear;
        }
        foreach (Tree tree in living)
        {
            if (!ServesAFutureTree(tree, accepted)) continue;
            if (markedIds.Contains(tree.Id)) { result.MarkedRemovals++; continue; }
            if (!plannedIds.Contains(tree.Id)) continue;
            Order order = open[tree.Id];
            result.PlannedRemovals++;
            if (order.Disposition == Disposition.Sold) result.SoldPlanned++;
            else if (order.Disposition == Disposition.Deadwood) result.DeadwoodPlanned++;
        }

        // The area must stay a forest: what is removed (marked, planned or done) against all it held before.
        double areaBasal = 0, removedBasal = 0;
        foreach (Tree tree in living)
            if (InArea(tree.X, tree.Z))
            {
                double ba = BasalArea(tree.DbhCm);
                areaBasal += ba;
                if (plannedIds.Contains(tree.Id) || markedIds.Contains(tree.Id)) removedBasal += ba;
            }
        foreach (Tree gone in removed)
            if (InArea(gone.X, gone.Z))
            {
                double ba = BasalArea(gone.DbhCm);
                areaBasal += ba;
                removedBasal += ba;
            }
        result.RemovedBasalShare = areaBasal > 0 ? (float)(removedBasal / areaBasal) : 0f;

        bool any = accepted.Count > 0;
        result.ReleaseExecuted = any && result.MinExecuted >= FirstReleaseShare * ExecutedTolerance;
        result.FurtherExecuted = any && result.MinExecuted >= FurtherReleaseShare * ExecutedTolerance;
        result.ReleasePlanned = any && result.MinPlannedAndExecuted >= FirstReleaseShare && result.CanopyKept;
        result.FurtherPlanned = any && result.MinPlannedAndExecuted >= FurtherReleaseShare && result.CanopyKept;
        result.MixedExecuted = result.SoldExecuted >= 1 && result.DeadwoodExecuted >= 1;
        result.MixedPlanned = result.SoldExecuted + result.SoldPlanned >= 1 && result.DeadwoodExecuted + result.DeadwoodPlanned >= 1;
        return result;
    }

    // Half the distance to the nearer neighbouring rack: the strip each rack serves, so neighbouring areas meet without overlap.
    public static float HalfWidth(IReadOnlyList<float> sortedLines, int index)
    {
        float gap = float.MaxValue;
        if (index > 0) gap = Math.Min(gap, sortedLines[index] - sortedLines[index - 1]);
        if (index + 1 < sortedLines.Count) gap = Math.Min(gap, sortedLines[index + 1] - sortedLines[index]);
        return gap == float.MaxValue ? DefaultHalfWidthM : gap * 0.5f;
    }

    // A tree is not overtopped when at least DominantShare of its competitors within reach are no bigger than it is.
    private static bool NotOvertopped(Tree tree, List<Tree> living)
    {
        int neighbours = 0, notBigger = 0;
        foreach (Tree other in living)
        {
            if (other.Id == tree.Id || Distance(tree, other) > ForestEcologyController.HegyiCutoffMeters) continue;
            neighbours++;
            if (other.DbhCm <= tree.DbhCm) notBigger++;
        }
        return neighbours == 0 || notBigger >= DominantShare * neighbours;
    }

    // A removal serves the lesson when it lies inside some future tree's competition reach.
    private static bool ServesAFutureTree(Tree tree, List<Tree> future)
    {
        foreach (Tree f in future)
            if (Distance(tree, f) <= ForestEcologyController.HegyiCutoffMeters) return true;
        return false;
    }

    private static float Distance(Tree a, Tree b)
    {
        float dx = a.X - b.X, dz = a.Z - b.Z;
        return (float)Math.Sqrt(dx * dx + dz * dz);
    }

    private static double BasalArea(float dbhCm) => Math.PI * Math.Pow(dbhCm / 200.0, 2.0);
}

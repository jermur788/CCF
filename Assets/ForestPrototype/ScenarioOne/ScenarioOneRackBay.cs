using System;
using System.Collections.Generic;

// Scenario One teaching Objectives 2-5 are one practical lesson on ONE rack and the standing trees beside it: the "rack area". This is
// the rules for it, on plain data (no UnityEngine), so they can be checked on plain .NET (Tools/Verification/ScenarioOneNonUi) and give
// the same answer everywhere. ScenarioOneRackBayReader feeds it the live trees and Work Plan.
//
// This analysis is derived from current trees/orders. Post-intervention inspection decisions are separately held in a session-only
// ScenarioOneRackReview; loading a forest requires fresh inspection, without adding save fields.
//
// What is a forestry fact and what is not (see Docs/Design/ScenarioOneRackBayLessonV1.md):
//   * the Hegyi competition term, its 8 m reach and the competitor shares are the game's existing simulation signals, used unchanged;
//   * a "future tree" is the player's own Crop Tree mark; positive selection of vigorous trees with room to grow is established
//     practice, but the numbers below (how many, how far apart, how much release) are simplified TEACHING CALIBRATION [D], not
//     silvicultural prescriptions. They govern this lesson and its interactive planning safeguard, not global scenario success or ecology.
public static class ScenarioOneRackBay
{
    // ---- Teaching calibration [D] ----------------------------------------------------------------------------------------------
    // Future trees the area needs: three show spacing and shared competitors while still fitting one area.
    public const int FutureTreesWanted = 3;
    // Model-linked tutorial spacing; 8 m neighbourhoods can still overlap. This is not a crown-space guarantee.
    public const float FutureTreeSpacingM = ForestEcologyController.HegyiCutoffMeters;
    // Initial planned release only: GAMEPLAY CALIBRATION, not a book prescription.
    public const float FirstReleaseShare = 0.30f;
    // Provisional cumulative trunk-area safeguard, not a canopy measure or forestry prescription.
    public const float MaxRemovedBasalShare = 0.40f;
    // Once trees have actually been felled, the surviving neighbours keep growing while a stump keeps the diameter it was felled at, so
    // the removed share slowly thins out after the work. This bounded tolerance allows modest growth, not an unlimited completion guarantee.
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
        // PlannedShare and MarkedShare use the panel's current living-neighbour denominator. ExecutedShare uses the historical
        // denominator (current neighbours plus recorded non-access removals), so release is measured against the competition the
        // tree actually had before the work. Only in-footprint contributions enter any of the three numerators.
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
        public int AreaCropCount;
        public bool NearSelected, MiddleSelected, FarSelected, BothSides;
        public Frame Geometry;
        public int CropElsewhere;

        public int FutureCount => Future.Count;
        public bool HasEnoughFuture => AreaCropCount == FutureTreesWanted && Future.Count == FutureTreesWanted
            && NearSelected && MiddleSelected && FarSelected && BothSides;

        public float RemovedBasalShare;
        public bool CanopyKept => RemovedBasalShare <= MaxRemovedBasalShare + 1e-6f;

        // Removals serving the future trees (felled for them) by what the player chose for the wood.
        public int SoldExecuted, DeadwoodExecuted, SoldPlanned, DeadwoodPlanned;
        public int ExecutedRemovals, PlannedRemovals, MarkedRemovals;
        // The year the latest such removal was carried out, or -1.
        public int ReleaseYear = -1;

        // The least released future tree, by what is in the Work Plan or done, only done, and with loose marks added.
        public float MinPlannedAndExecuted, MinExecuted, MinWithMarks;

        public bool ReleasePlanned, ReleaseExecuted, MixedPlanned, MixedExecuted;
        public string ReleaseKey = "";
        public bool Contains(float x, float z) => Geometry.Contains(x, z);
        public int Portion(float x, float z) => Geometry.Portion(x, z);
    }

    // The frame comes from the actual layout rack endpoints. Along/across projections also work for rotated/translated racks.
    public readonly struct Frame
    {
        public readonly float X, Z, AlongX, AlongZ, HalfWidth;
        public Frame(RackPoint start, RackPoint end, float halfWidth)
        {
            X = (start.X + end.X) * .5f; Z = (start.Z + end.Z) * .5f;
            float dx = end.X - start.X, dz = end.Z - start.Z;
            float length = (float)Math.Sqrt(dx * dx + dz * dz);
            AlongX = length > 0 ? dx / length : 1f; AlongZ = length > 0 ? dz / length : 0f;
            HalfWidth = halfWidth;
        }
        public float Along(float x, float z) => (x - X) * AlongX + (z - Z) * AlongZ;
        public float Across(float x, float z) => -(x - X) * AlongZ + (z - Z) * AlongX;
        public bool Contains(float x, float z) => Math.Abs(Along(x, z)) <= HalfLengthM
            && Math.Abs(Across(x, z)) <= HalfWidth && Math.Abs(Across(x, z)) > RackGeometry.DefaultWidthM * .5f;
        public int Zone(float x, float z) => Along(x, z) < -HalfLengthM / 3f ? 0 : Along(x, z) < HalfLengthM / 3f ? 1 : 2;
        public int Portion(float x, float z) => Contains(x, z) ? Zone(x, z) * 2 + (Across(x, z) > 0 ? 1 : 0) : -1;
    }

    // Compatibility entry for the authored axis-aligned layout and offline fixtures. Live callers supply the actual rack frame.
    public static Analysis Analyse(IReadOnlyList<float> rackLines, IReadOnlyList<Tree> trees, IReadOnlyList<Order> orders)
        => Analyse(rackLines, trees, orders, null);

    public static Analysis Analyse(IReadOnlyList<float> rackLines, IReadOnlyList<Tree> trees, IReadOnlyList<Order> orders, Frame? frame)
    {
        var result = new Analysis();
        if (rackLines == null || rackLines.Count == 0 || trees == null) return result;
        var lines = new List<float>(rackLines); lines.Sort();
        result.Applies = true;
        result.RackCount = lines.Count; result.RackIndex = lines.Count / 2;
        result.CentreZ = lines[result.RackIndex]; result.HalfWidthM = HalfWidth(lines, result.RackIndex);
        result.Geometry = frame ?? new Frame(new RackPoint(-HalfLengthM, result.CentreZ),
            new RackPoint(HalfLengthM, result.CentreZ), result.HalfWidthM);
        bool InArea(float x, float z) => result.Contains(x, z);
        bool InAccess(float x, float z)
        {
            float across = result.Geometry.Across(x, z) + result.CentreZ;
            foreach (float line in lines) if (Math.Abs(across - line) <= RackGeometry.DefaultWidthM * .5f) return true;
            return false;
        }

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
            else if (completed.ContainsKey(tree.Id) && !InAccess(tree.X, tree.Z))
                removed.Add(tree);
        }

        // Promising trees are the player's choice: no DBH-ranking or unsupported dominance test. Stable ID order makes conflicts reproducible.
        var crops = new List<Tree>();
        foreach (Tree tree in living)
            if (tree.Crop)
            {
                if (InArea(tree.X, tree.Z)) crops.Add(tree);
                else result.CropElsewhere++;
            }
        result.AreaCropCount = crops.Count;
        crops.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));
        var accepted = new List<Tree>();
        foreach (Tree crop in crops)
        {
            bool clear = true;
            foreach (Tree other in accepted)
                if (Distance(crop, other) < FutureTreeSpacingM) { clear = false; break; }
            if (clear) accepted.Add(crop);
            else result.TooClose.Add(crop.Id);
        }

        bool left = false, right = false;
        foreach (Tree crop in accepted)
        {
            int zone = result.Geometry.Zone(crop.X, crop.Z);
            if (zone == 0) result.NearSelected = true;
            if (zone == 1) result.MiddleSelected = true;
            if (zone == 2) result.FarSelected = true;
            if (result.Geometry.Across(crop.X, crop.Z) < 0) left = true; else right = true;
        }
        result.BothSides = left && right;

        // Current Share uses the same living-neighbour denominator as the inspection panel. Only INSIDE-footprint work earns lesson
        // credit. The historical denominator (living plus recorded non-access removals) is used only for the executed-release account,
        // so outside felling cannot inflate inside credit merely by disappearing from current competition.
        result.MinPlannedAndExecuted = result.MinExecuted = result.MinWithMarks = accepted.Count == 0 ? 0f : float.MaxValue;
        var samples = new List<CompetitionSample>(living.Count + removed.Count);
        foreach (Tree future in accepted)
        {
            samples.Clear();
            foreach (Tree other in living)
            {
                if (other.Id == future.Id) continue;
                bool inside = InArea(other.X, other.Z);
                samples.Add(CompetitionShare.SampleFrom(other.Id, other.X, other.Z, future.X, future.Z, other.DbhCm,
                    true, inside && plannedIds.Contains(other.Id), inside));
            }
            foreach (Tree gone in removed)
            {
                bool inside = InArea(gone.X, gone.Z);
                samples.Add(CompetitionShare.SampleFrom(gone.Id, gone.X, gone.Z, future.X, future.Z, gone.DbhCm,
                    false, false, inside));
            }
            CompetitionShareSet competition = CompetitionShare.Measure(future.DbhCm, samples);
            double planned = 0, executed = 0, marked = 0;
            foreach (CompetitionContribution contribution in competition.Neighbours)
            {
                if (contribution.Living)
                {
                    if (!contribution.InFootprint) continue;
                    if (contribution.PlannedFell) planned += contribution.Contribution;
                    else if (markedIds.Contains(contribution.Id)) marked += contribution.Contribution;
                }
                else if (contribution.InFootprint)
                {
                    executed += contribution.Contribution;
                }
            }
            float p = competition.CurrentTotal > 0 ? (float)(planned / competition.CurrentTotal) : 0f;
            float e = competition.HistoricalTotal > 0 ? (float)(executed / competition.HistoricalTotal) : 0f;
            float m = competition.CurrentTotal > 0 ? (float)(marked / competition.CurrentTotal) : 0f;
            result.Future.Add(new FutureTree(future.Id, future.X, future.Z, future.DbhCm, p, e, m));
            result.MinPlannedAndExecuted = Math.Min(result.MinPlannedAndExecuted, p + e);
            result.MinExecuted = Math.Min(result.MinExecuted, e);
            result.MinWithMarks = Math.Min(result.MinWithMarks, p + e + m);
        }

        // Removals made for the future trees: what they were, what the player chose for the wood, and when they were carried out.
        foreach (Tree gone in removed)
        {
            if (!InArea(gone.X, gone.Z) || !ServesAFutureTree(gone, accepted)) continue;
            Order order = completed[gone.Id];
            result.ExecutedRemovals++;
            if (order.Disposition == Disposition.Sold) result.SoldExecuted++;
            else if (order.Disposition == Disposition.Deadwood) result.DeadwoodExecuted++;
            if (order.ResolvedYear > result.ReleaseYear) result.ReleaseYear = order.ResolvedYear;
        }
        foreach (Tree tree in living)
        {
            if (!InArea(tree.X, tree.Z) || !ServesAFutureTree(tree, accepted)) continue;
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

        result.ReleaseExecuted = result.HasEnoughFuture && result.CanopyKept && result.MinExecuted >= FirstReleaseShare * ExecutedTolerance;
        result.ReleasePlanned = result.HasEnoughFuture && result.CanopyKept && result.MinPlannedAndExecuted >= FirstReleaseShare;
        // Any newly executed in-footprint felling invalidates the session's earlier inspection decisions, even away from a Crop Tree.
        var releaseIds = new List<string>();
        foreach (Tree gone in removed) if (InArea(gone.X, gone.Z)) releaseIds.Add(gone.Id + ":" + completed[gone.Id].ResolvedYear);
        releaseIds.Sort(StringComparer.Ordinal);
        result.ReleaseKey = string.Join("|", releaseIds);
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

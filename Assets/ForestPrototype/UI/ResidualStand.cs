using System.Collections.Generic;
using UnityEngine;

// One living tree as seen by the Work Plan's "What you are leaving" summary.
// Plain data, so the summary can be tested without a scene.
public readonly struct ResidualTree
{
    public readonly string Id;
    public readonly int Cell;
    public readonly float DbhCm;
    public readonly float VolumeM3;
    public readonly bool PlannedFell;
    public readonly bool CropTree;
    public readonly bool SeedProducing;

    public ResidualTree(string id, int cell, float dbhCm, float volumeM3, bool plannedFell, bool cropTree, bool seedProducing)
    {
        Id = id ?? "";
        Cell = cell;
        DbhCm = dbhCm;
        VolumeM3 = volumeM3;
        PlannedFell = plannedFell;
        CropTree = cropTree;
        SeedProducing = seedProducing;
    }
}

// How the planned removal is arranged across the 5 m cells. The label is only
// a description, and only given when it holds across a range of thresholds;
// otherwise the facts are shown without one. It is not a judgement.
public enum OpeningPattern { NoRemoval, NoLabel, SpreadOut, SeveralOpenings, OneConcentratedOpening }

public sealed class ResidualStandResult
{
    public int TreesBefore, TreesAfter;
    public float BasalAreaBefore, BasalAreaAfter;   // m²/ha, as the Annual Review computes it
    public float VolumeBefore, VolumeAfter;         // modelled stem volume, m³ on the property
    public int CropTrees, CropTreesKept;
    public int SeedTreesBefore, SeedTreesAfter;
    public int PlannedFells => TreesBefore - TreesAfter;

    // Spatial facts (cells are the ecology grid; "opened" = the plan removes at
    // least OpenedShare of that cell's basal area).
    public int CellsTouched;
    public int OpenedCells;
    public int LargestOpeningCells;
    public float LargestOpeningM2;
    public float ShareOfRemovalInLargestOpening;
    public OpeningPattern Pattern;
    public int PatternAgreement, PatternVariants;
}

// Before → after facts for the trees that stay if every planned felling is
// carried out. Uses only current tree state and the planned Fell orders; it is
// an estimate before any growth, mortality or storm.
public static class ResidualStand
{
    public const float OpenedShare = 0.30f;
    public const int ConcentratedCells = 4;
    public const float ConcentratedShare = 0.60f;
    public const float SpreadShare = 0.40f;
    // A label is shown only when this fraction of nearby threshold choices agree.
    public const float LabelAgreement = 0.90f;

    public static float BasalAreaM2(float dbhCm) => Mathf.PI * Mathf.Pow(dbhCm / 200f, 2f);

    public static ResidualStandResult Compute(IReadOnlyList<ResidualTree> trees, float areaHa, int cellsPerAxis, float cellSizeM)
    {
        var result = new ResidualStandResult();
        if (trees == null) return result;
        areaHa = Mathf.Max(0.0001f, areaHa);
        int cellCount = Mathf.Max(0, cellsPerAxis * cellsPerAxis);
        var cellBasal = new float[cellCount];
        var removedBasal = new float[cellCount];

        foreach (ResidualTree tree in trees)
        {
            float basal = BasalAreaM2(tree.DbhCm);
            result.TreesBefore++;
            result.BasalAreaBefore += basal / areaHa;
            result.VolumeBefore += tree.VolumeM3;
            if (tree.SeedProducing) result.SeedTreesBefore++;
            if (tree.CropTree) result.CropTrees++;
            bool inGrid = tree.Cell >= 0 && tree.Cell < cellCount;
            if (inGrid) cellBasal[tree.Cell] += basal;
            if (tree.PlannedFell)
            {
                if (inGrid) removedBasal[tree.Cell] += basal;
                continue;
            }
            result.TreesAfter++;
            result.BasalAreaAfter += basal / areaHa;
            result.VolumeAfter += tree.VolumeM3;
            if (tree.SeedProducing) result.SeedTreesAfter++;
            if (tree.CropTree) result.CropTreesKept++;
        }

        DescribePattern(result, cellBasal, removedBasal, cellsPerAxis, cellSizeM);
        return result;
    }

    private static void DescribePattern(ResidualStandResult result, float[] cellBasal, float[] removedBasal, int perAxis, float cellSizeM)
    {
        float removedTotal = 0f;
        for (int i = 0; i < removedBasal.Length; i++)
        {
            removedTotal += removedBasal[i];
            if (removedBasal[i] > 0f) result.CellsTouched++;
        }
        if (removedTotal <= 0f)
        {
            result.Pattern = OpeningPattern.NoRemoval;
            return;
        }

        Openings central = Measure(cellBasal, removedBasal, removedTotal, perAxis, OpenedShare);
        result.OpenedCells = central.OpenedCells;
        result.LargestOpeningCells = central.LargestCells;
        result.LargestOpeningM2 = central.LargestCells * cellSizeM * cellSizeM;
        result.ShareOfRemovalInLargestOpening = central.LargestShare;

        // Robustness: the label must hold for nearby thresholds, not only the central ones.
        OpeningPattern centralLabel = Label(central, ConcentratedCells, ConcentratedShare, SpreadShare);
        int agree = 0, variants = 0;
        foreach (float opened in new[] { 0.25f, 0.30f, 0.40f })
        {
            Openings measured = Measure(cellBasal, removedBasal, removedTotal, perAxis, opened);
            foreach (int cells in new[] { 3, 4, 5 })
                foreach (float concentrated in new[] { 0.5f, 0.6f, 0.7f })
                    foreach (float spread in new[] { 0.3f, 0.4f, 0.5f })
                    {
                        variants++;
                        if (Label(measured, cells, concentrated, spread) == centralLabel) agree++;
                    }
        }
        result.PatternAgreement = agree;
        result.PatternVariants = variants;
        result.Pattern = agree >= Mathf.CeilToInt(LabelAgreement * variants) ? centralLabel : OpeningPattern.NoLabel;
    }

    private struct Openings
    {
        public int OpenedCells, LargestCells;
        public float LargestShare, OpenedShareOfRemoval;
    }

    private static OpeningPattern Label(Openings o, int concentratedCells, float concentratedShare, float spreadShare)
    {
        if (o.LargestCells >= concentratedCells && o.LargestShare >= concentratedShare) return OpeningPattern.OneConcentratedOpening;
        if (o.OpenedShareOfRemoval < spreadShare) return OpeningPattern.SpreadOut;
        return OpeningPattern.SeveralOpenings;
    }

    // Opened cells and their largest 4-connected group (by removed basal area).
    private static Openings Measure(float[] cellBasal, float[] removedBasal, float removedTotal, int perAxis, float share)
    {
        var o = new Openings();
        int count = removedBasal.Length;
        var opened = new bool[count];
        float openedRemoved = 0f;
        for (int i = 0; i < count; i++)
        {
            opened[i] = cellBasal[i] > 0f && removedBasal[i] / cellBasal[i] >= share;
            if (!opened[i]) continue;
            o.OpenedCells++;
            openedRemoved += removedBasal[i];
        }
        o.OpenedShareOfRemoval = openedRemoved / removedTotal;

        var seen = new bool[count];
        var stack = new Stack<int>();
        float bestRemoved = -1f;
        for (int start = 0; start < count; start++)
        {
            if (!opened[start] || seen[start]) continue;
            int cells = 0;
            float groupRemoved = 0f;
            seen[start] = true;
            stack.Push(start);
            while (stack.Count > 0)
            {
                int c = stack.Pop();
                cells++;
                groupRemoved += removedBasal[c];
                int x = c % perAxis, z = c / perAxis;
                Visit(x + 1, z); Visit(x - 1, z); Visit(x, z + 1); Visit(x, z - 1);
            }
            if (groupRemoved > bestRemoved)
            {
                bestRemoved = groupRemoved;
                o.LargestCells = cells;
                o.LargestShare = groupRemoved / removedTotal;
            }
        }
        return o;

        void Visit(int x, int z)
        {
            if (x < 0 || z < 0 || x >= perAxis || z >= perAxis) return;
            int n = z * perAxis + x;
            if (!opened[n] || seen[n]) return;
            seen[n] = true;
            stack.Push(n);
        }
    }

    public static string PatternSentence(ResidualStandResult r)
    {
        switch (r.Pattern)
        {
            case OpeningPattern.SpreadOut: return "Spread out: no part of the stand is heavily opened.";
            case OpeningPattern.SeveralOpenings: return "Openings in several places.";
            case OpeningPattern.OneConcentratedOpening: return "One concentrated opening.";
            default: return "";
        }
    }
}

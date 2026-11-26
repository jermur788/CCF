namespace SitkaP1.Core.WoodlandImport;

/// <summary>
/// Translates boundary and tree coordinates to local metres relative to a deterministic origin.
/// Pure translation only: pairwise distances are preserved exactly, with zero scaling,
/// zero rotation, zero snapping, and zero clamping.
/// </summary>
public static class LocalCoordinateNormalizer
{
    public const string BoundaryOriginRule = "boundary-min-xy-translation-only-v1";
    public const string TreeOriginRule = "tree-min-xy-translation-only-v1";
    public const string NoCoordinatesOriginRule = "zero-origin-no-coordinates-v1";

    public static (WoodlandBoundary? Boundary, IReadOnlyList<ImportedTreeRecord> Trees, LocalCoordinateOrigin Origin) Normalize(
        WoodlandBoundary? boundary,
        IReadOnlyList<ImportedTreeRecord> trees)
    {
        LocalCoordinateOrigin origin = ComputeOrigin(boundary, trees);

        WoodlandBoundary? translatedBoundary = boundary?.Translate(-origin.OriginXM, -origin.OriginYM);

        var translatedTrees = new ImportedTreeRecord[trees.Count];
        for (int i = 0; i < trees.Count; i++)
        {
            ImportedTreeRecord tree = trees[i];
            if (!tree.PositionLocalM.IsMissing && tree.PositionLocalM.Value.IsFinite)
            {
                LocalPoint2D p = tree.PositionLocalM.Value;
                var moved = new LocalPoint2D(p.XM - origin.OriginXM, p.YM - origin.OriginYM);
                translatedTrees[i] = tree with
                {
                    PositionLocalM = tree.PositionLocalM with { Value = moved },
                };
            }
            else
            {
                translatedTrees[i] = tree;
            }
        }

        return (translatedBoundary, Array.AsReadOnly(translatedTrees), origin);
    }

    public static LocalCoordinateOrigin ComputeOrigin(
        WoodlandBoundary? boundary,
        IReadOnlyList<ImportedTreeRecord> trees)
    {
        if (boundary is not null && boundary.VerticesLocalM.Count > 0)
        {
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            for (int i = 0; i < boundary.VerticesLocalM.Count; i++)
            {
                LocalPoint2D v = boundary.VerticesLocalM[i];
                if (v.XM < minX) minX = v.XM;
                if (v.YM < minY) minY = v.YM;
            }

            return new LocalCoordinateOrigin(minX, minY, BoundaryOriginRule);
        }

        double treeMinX = double.PositiveInfinity;
        double treeMinY = double.PositiveInfinity;
        bool anyTree = false;
        for (int i = 0; i < trees.Count; i++)
        {
            ImportField<LocalPoint2D> pos = trees[i].PositionLocalM;
            if (!pos.IsMissing && pos.Value.IsFinite)
            {
                if (pos.Value.XM < treeMinX) treeMinX = pos.Value.XM;
                if (pos.Value.YM < treeMinY) treeMinY = pos.Value.YM;
                anyTree = true;
            }
        }

        if (anyTree)
        {
            return new LocalCoordinateOrigin(treeMinX, treeMinY, TreeOriginRule);
        }

        return new LocalCoordinateOrigin(0.0, 0.0, NoCoordinatesOriginRule);
    }
}

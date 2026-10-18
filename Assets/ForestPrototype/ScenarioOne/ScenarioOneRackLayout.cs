using System.Collections.Generic;

// The authored extraction-rack layout of Scenario One.
//
// Scenario One opens racks across the whole planted block. The layout is evenly spaced over the planting rows: every eighth
// row, so seven rows are left standing between neighbouring racks, centred in the block. The spacing is an authored Scenario One
// choice: it is not a forestry rule and carries no research claim. The rack tools themselves (RackPlanner, RackGeometry) take any
// angle, width and start/end.
//
// The rows are found from the planted trees' positions (living trees and stumps), so the layout follows the stand as it was
// generated and keeps its place after a harvest. It is not stored: it is derived each time, so there is no save field.
public static class ScenarioOneRackLayout
{
    // Rows left standing between two neighbouring racks in the authored layout.
    public const int RowsBetween = 7;

    // Only the enlarged stand has the layout; a 40 m legacy stand and the frozen Reference Future are unchanged.
    public static bool AppliesTo(int geometryModel) => geometryModel == StandGeometryModel.Enlarged80;

    // Across coordinates (the z coordinate, rows run east-west) of the layout's rack centre lines, west to east order.
    public static List<float> Centres(IReadOnlyList<RackPoint> plantationPoints)
    {
        var centres = new List<float>();
        if (plantationPoints == null || plantationPoints.Count == 0)
            return centres;
        List<RackRow> rows = RackRows.Find(plantationPoints, 0f);
        foreach (int index in RackLayout.RowIndices(rows.Count, RowsBetween))
            centres.Add(rows[index].Centre);
        return centres;
    }
}

using System;
using System.Collections.Generic;

// Display guidance, never a player rack or a work order. Value fields cannot retain a mutable world record.
public readonly struct SuggestedRackSegment
{
    public readonly RackPoint Entrance, Far;
    public readonly float WidthM;

    internal SuggestedRackSegment(RackPoint entrance, RackPoint far, float widthM)
    {
        Entrance = entrance;
        Far = far;
        WidthM = widthM;
    }
}

// The five headland routes qualified by RACK-V2-02B-R and the combined gate. This V2 foundation owns
// their entrance identifiers; the historical every-eighth-row ScenarioOneRackLayout is unchanged.
public static class ScenarioOneSuggestedRackLayout
{
    private static readonly string[] EntranceTreeIds = { "PO0203", "PO1204", "PO2003", "PO2803", "PO3803" };

    // Supply the world's explicit saved identities and a snapshot of plantation trees, including
    // stumps. Positions, not current felling/marking eligibility, define this guidance. Results are
    // ordered south to north, detached and read-only. An incomplete/ambiguous snapshot returns no
    // layout rather than inventing replacement routes. No player rack IDs or state are involved.
    public static IReadOnlyList<SuggestedRackSegment> Get(int rackPlanningModel, int standGeometryModel,
        IReadOnlyList<RackTree> plantationTrees)
    {
        if (rackPlanningModel != RackPlanningModel.RackPlanningFieldMarkingV2
            || standGeometryModel != StandGeometryModel.Enlarged80 || plantationTrees == null)
            return Array.Empty<SuggestedRackSegment>();

        float farX = StandGeometryModel.StandSizeMeters(standGeometryModel) * 0.5f;
        var segments = new SuggestedRackSegment[EntranceTreeIds.Length];
        for (int route = 0; route < EntranceTreeIds.Length; route++)
        {
            int matches = 0;
            float z = 0f;
            for (int tree = 0; tree < plantationTrees.Count; tree++)
            {
                RackTree candidate = plantationTrees[tree];
                if (!string.Equals(candidate.Id, EntranceTreeIds[route], StringComparison.Ordinal))
                    continue;
                matches++;
                z = candidate.Z;
            }
            if (matches != 1 || float.IsNaN(z) || float.IsInfinity(z)
                || z < ScenarioOneHeadlandGeometry.SouthZ || z > ScenarioOneHeadlandGeometry.NorthZ)
                return Array.Empty<SuggestedRackSegment>();

            segments[route] = new SuggestedRackSegment(new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, z),
                new RackPoint(farX, z), RackGeometry.DefaultWidthM);
        }
        return Array.AsReadOnly(segments);
    }
}

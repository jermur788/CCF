using System.Globalization;

// S1-D T-03 (FR-A3, AC-2): the 40% cumulative teaching-footprint rule is a hard Work Plan safeguard only while the teaching sample
// activity is still open. Once the sample is historically complete it is an advisory note and never disables Approve or Advance.
// Plain data in, plain data out (no UnityEngine), so it is checked on the plain .NET SDK. It changes no analysis or threshold:
// ScenarioOneRackBay still owns MaxRemovedBasalShare, CanopyKept and the shares; this only decides what to do with CanopyKept.
public static class ScenarioOneTeachingCap
{
    public enum Phase
    {
        // No rack layout, no teaching footprint, or the racks are not open yet: the rule does not apply.
        NotApplicable,
        WithinCap,
        // The sample activity is still open and the footprint is over the cap: Approve and Advance are disabled.
        Hard,
        // The sample is historically complete and the footprint is over the cap: a note only.
        Advisory
    }

    // One definition of "this teaching sample is done", read from facts that already persist: the executed in-footprint release served
    // the three retained trees with both sold and deadwood outcomes, and the Annual Review after that work has been acknowledged.
    // The cap itself is deliberately not an input, so exceeding it cannot reopen the activity. The session-only [J]/[K] decisions are
    // not persisted and are not used here. T-04's guide latch can replace this one function.
    public static bool SampleHistoricallyComplete(ScenarioOneRackBay.Analysis bay, bool releaseReviewed)
        => bay != null && bay.Applies && releaseReviewed && bay.ReleaseYear >= 0 && bay.MixedExecuted
           && bay.MinExecuted >= ScenarioOneRackBay.FirstReleaseShare * ScenarioOneRackBay.ExecutedTolerance;

    public static Phase Classify(ScenarioOneRackBay.Analysis bay, bool accessOpen, bool releaseReviewed)
    {
        if (bay == null || !bay.Applies || !accessOpen) return Phase.NotApplicable;
        if (bay.CanopyKept) return Phase.WithinCap;
        return SampleHistoricallyComplete(bay, releaseReviewed) ? Phase.Advisory : Phase.Hard;
    }

    private static string Percent(float share)
        => ((int)System.Math.Round(share * 100f)).ToString(CultureInfo.InvariantCulture) + "%";

    // Names the rule, the footprint, the quantity and the remedy.
    public static string HardMessage(ScenarioOneRackBay.Analysis bay)
        => "Teaching rack safeguard exceeded: the teaching footprint beside " + ScenarioOneRackBayText.AreaName(bay) + " has "
           + Percent(bay.RemovedBasalShare) + " of its trunk area removed or planned for removal (limit "
           + Percent(ScenarioOneRackBay.MaxRemovedBasalShare) + " cumulative). Cancel/unmark in-footprint removals before approval or advance. "
           + "Already executed excess cannot complete this lesson.";

    public static string AdvisoryMessage(ScenarioOneRackBay.Analysis bay)
        => "Note (advisory): " + Percent(bay.RemovedBasalShare) + " of the trunk area in the teaching footprint beside "
           + ScenarioOneRackBayText.AreaName(bay) + " has been removed or planned, above the " + Percent(ScenarioOneRackBay.MaxRemovedBasalShare)
           + " teaching guide from the first lesson. It does not block Approve Work Plan or Advance; consider the remaining canopy and habitat here.";
}

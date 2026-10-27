using System;
using System.Globalization;
using System.Linq;

// Objectives 2-5 instructions derive from the footprint rules and the real session inspection checklist.
public static class ScenarioOneRackBayText
{
    private static string Pct(float share) => ((int)Math.Round(share * 100f)).ToString(CultureInfo.InvariantCulture) + "%";
    private static ScenarioOneRackBay.Analysis Of(ScenarioOneTeachingSequence.Facts f) => f != null && f.Bay != null && f.Bay.Applies ? f.Bay : null;
    public static string AreaName(ScenarioOneRackBay.Analysis a)
        => a.RackCount == 5 && a.RackIndex == 2 ? "the middle rack" : "rack " + (a.RackIndex + 1) + " of " + a.RackCount;
    private static string Named(ScenarioOneRackBay.Analysis a)
        => string.Join(", ", a.Future.Select(t => t.Id + " " + Pct(t.ReleaseWithMarksShare)));
    public static string Instruction(int lesson, ScenarioOneTeachingSequence.Facts f)
    {
        var a = Of(f); if (a == null) return null;
        if (!a.CanopyKept && lesson >= 2 && lesson <= 4)
            return "The teaching footprint exceeds the provisional 40% cumulative trunk-area safeguard (" + Pct(a.RemovedBasalShare)
                + "). Cancel/unmark excess planned removals. Already executed excess cannot pass this lesson. This is gameplay calibration, not a forestry prescription.";
        switch (lesson)
        {
            case 1:
                if (!f.AccessReviewed) return null;
                return ScenarioOneTeachingSequence.Lessons[1].Action + " Selected: " + a.AreaCropCount + " of 3. Zones: "
                    + (a.NearSelected ? "near ✓" : "near needed") + ", " + (a.MiddleSelected ? "middle ✓" : "middle needed") + ", "
                    + (a.FarSelected ? "far ✓" : "far needed") + (a.BothSides ? "; both sides represented." : "; use both rack sides.")
                    + (a.TooClose.Count > 0 ? " Some marked trees are less than 8 m apart; unmark one of each pair." : "")
                    + (a.AreaCropCount > 3 ? " Keep exactly three teaching selections in this footprint." : "");
            case 2:
                return ScenarioOneTeachingSequence.Lessons[2].Action + " Credited share (marks, orders and executed work): " + Named(a)
                    + ". About 30% per tree is the initial gameplay target. Only inside-footprint removals count.";
            case 3:
                if (!a.MixedPlanned)
                    return "Compare both outcomes in the teaching footprint: assign some release trees to sold timber and some to fallen deadwood. "
                        + "To sell " + (a.SoldExecuted + a.SoldPlanned) + "; deadwood " + (a.DeadwoodExecuted + a.DeadwoodPlanned) + ". Keep the cumulative safeguard.";
                return "Review the retained stand and the work across the rack, approve the Work Plan [Tab], then advance one year.";
            case 4:
                if (!f.ReleaseReviewed) return "Read and acknowledge the Annual Review, then return to inspect all three retained trees and the surrounding stand.";
                return "Did your intervention create enough growing space? Reopen each retained tree's inspection [E]. [J] Retain here; [K] Release further after marking justified competitors. "
                    + "Inspect and confirm retained stand in the near, middle and far zones on both sides. Good CCF management includes deciding not to cut. No second release percentage is required.";
            default: return null;
        }
    }
    public static string Hud(int lesson, ScenarioOneTeachingSequence.Facts f)
    {
        var a = Of(f); if (a == null) return null;
        if (!a.CanopyKept && lesson >= 2 && lesson <= 4) return "Teaching footprint over 40% trunk-area safeguard. Cancel excess planned removals; executed excess cannot pass.";
        switch (lesson)
        {
            case 1: return f.AccessReviewed ? "Choose one Crop Tree in each zone, 8 m apart; include both rack sides." : null;
            case 2: return "Compare Share and position; mark meaningful in-footprint competitors, then open Work Plan [Tab].";
            case 3: return "Use sold and deadwood outcomes within the safeguard; approve and advance a year.";
            case 4: return !f.ReleaseReviewed ? "Read the Annual Review, then return to the retained trees."
                : "Revisit trees and surrounding stand [E]. [J] Retain here; [K] justified further release. No extra cut required.";
            default: return null;
        }
    }
    public static string Progress(int lesson, ScenarioOneTeachingSequence.Facts f)
    {
        var a = Of(f); if (a == null) return null;
        switch (lesson)
        {
            case 1: return "Selections " + a.AreaCropCount + "/3 · zones " + ((a.NearSelected ? 1 : 0) + (a.MiddleSelected ? 1 : 0) + (a.FarSelected ? 1 : 0)) + "/3 · " + (a.BothSides ? "both sides" : "both sides needed");
            case 2: return "Initial release: " + Pct(a.MinWithMarks) + " minimum / about 30% each · trunk area " + Pct(a.RemovedBasalShare) + "/40%";
            case 3: return "To sell " + (a.SoldExecuted + a.SoldPlanned) + " · deadwood " + (a.DeadwoodExecuted + a.DeadwoodPlanned) + (f.ReleaseExecuted ? " · work done" : " · work not done");
            case 4: return "Revisited " + f.RevisitedTrees + "/3 · decisions " + f.ReviewDecisions + "/3 · stand portions " + f.StandPortionsReviewed + "/6";
            default: return null;
        }
    }
}

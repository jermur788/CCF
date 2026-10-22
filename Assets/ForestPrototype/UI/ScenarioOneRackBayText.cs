using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// The words of the rack-area lesson (Objectives 2-5): what to do next, the short line for the HUD, and the progress line, all from the
// numbers the analysis found (ScenarioOneRackBay). Pure text, no UnityEngine, so it is checked on plain .NET.
//
// Each function returns null when the facts carry no analysis (a stage injected by a harness, or a stand without a rack layout); the
// earlier wording then applies unchanged. The wording describes the player's own forest and what the lesson asks; it never names a tree
// to fell.
public static class ScenarioOneRackBayText
{
    private static string Pct(float share) => ((int)Math.Round(share * 100f)).ToString(CultureInfo.InvariantCulture) + "%";
    private static string Count(int n, string one, string many) => n.ToString(CultureInfo.InvariantCulture) + " " + (n == 1 ? one : many);
    private static ScenarioOneRackBay.Analysis Of(ScenarioOneTeachingSequence.Facts f) => f != null && f.Bay != null && f.Bay.Applies ? f.Bay : null;

    // "the middle rack" for the authored five-rack layout, otherwise its number.
    public static string AreaName(ScenarioOneRackBay.Analysis a)
        => a.RackCount == 5 && a.RackIndex == 2 ? "the middle rack" : "rack " + (a.RackIndex + 1) + " of " + a.RackCount;

    // How far each future tree is released, in the order the trees are listed, with the target beside it.
    private static string Released(ScenarioOneRackBay.Analysis a, float target, bool withMarks)
    {
        string list = string.Join(" · ", a.Future.Select(t => Pct(withMarks ? t.ReleaseWithMarksShare : t.ReleaseShare)));
        return list + " (about " + Pct(target) + " each)";
    }

    private static string Named(ScenarioOneRackBay.Analysis a, bool withMarks)
        => string.Join(", ", a.Future.Select(t => t.Id + " " + Pct(withMarks ? t.ReleaseWithMarksShare : t.ReleaseShare)));

    public static string Instruction(int lesson, ScenarioOneTeachingSequence.Facts f)
    {
        ScenarioOneRackBay.Analysis a = Of(f);
        if (a == null) return null;
        string action = ScenarioOneTeachingSequence.Lessons[lesson].Action;
        switch (lesson)
        {
            case 1:
            {
                if (!f.AccessReviewed) return null;
                var text = new List<string> { action, "Future trees in this area: " + a.FutureCount + " of " + ScenarioOneRackBay.FutureTreesWanted + "." };
                if (a.TooClose.Count > 0)
                    text.Add(Count(a.TooClose.Count, "marked tree stands", "marked trees stand") + " within " + ScenarioOneRackBay.FutureTreeSpacingM.ToString("0", CultureInfo.InvariantCulture)
                        + " m of another future tree and " + (a.TooClose.Count == 1 ? "does" : "do") + " not count: unmark one of each pair.");
                if (a.Overtopped.Count > 0)
                    text.Add(Count(a.Overtopped.Count, "marked tree is", "marked trees are") + " overtopped by bigger neighbours and " + (a.Overtopped.Count == 1 ? "does" : "do")
                        + " not count: choose a tree that is as big as or bigger than most of the trees around it.");
                if (a.CropElsewhere > 0)
                    text.Add(Count(a.CropElsewhere, "future tree", "future trees") + " outside this area " + (a.CropElsewhere == 1 ? "does" : "do") + " not count for this lesson.");
                return string.Join(" ", text);
            }
            case 2:
            {
                var text = new List<string> { action, "Released so far (marked, planned or done): " + Named(a, true) + "; about " + Pct(ScenarioOneRackBay.FirstReleaseShare) + " each is enough." };
                if (!a.CanopyKept)
                    text.Add("Your marks and plan would take " + Pct(a.RemovedBasalShare) + " of the area's trunk area (the limit is " + Pct(ScenarioOneRackBay.MaxRemovedBasalShare)
                        + "): unmark some. Removing more is not better; the area must stay a forest.");
                return string.Join(" ", text);
            }
            case 3:
            {
                int sold = a.SoldExecuted + a.SoldPlanned, left = a.DeadwoodExecuted + a.DeadwoodPlanned;
                string tally = "So far " + sold + " to sell and " + left + " to leave as deadwood.";
                if (f.ReleaseExecuted && !f.DeadwoodExecuted)
                {
                    string all = a.DeadwoodExecuted == 0 ? "sold" : "left as deadwood";
                    return "The release is done, but all of its wood was " + all + ". Mark one more neighbour of a future tree, give it the other fate in the Work Plan [Tab], approve and advance a year.";
                }
                if (!a.MixedPlanned)
                    return "Give the release trees different fates: leave some as deadwood and sell others (change a job's fate in the Work Plan [Tab]). " + tally;
                return "Approve the work in the Work Plan [Tab], then advance one year. " + tally;
            }
            case 4:
            {
                if (!f.ReleaseReviewed)
                    return "Read and acknowledge the Annual Review, then walk back and inspect each future tree. Has its growing space improved?";
                if (!f.AreaReleased)
                    return "Look at each future tree again. Released so far: " + Named(a, true) + ". Release each to about " + Pct(ScenarioOneRackBay.FurtherReleaseShare)
                        + ": mark further competitors, add them in the Work Plan [Tab], approve, advance a year and review the result.";
                return null;
            }
        }
        return null;
    }

    public static string Hud(int lesson, ScenarioOneTeachingSequence.Facts f)
    {
        ScenarioOneRackBay.Analysis a = Of(f);
        if (a == null) return null;
        switch (lesson)
        {
            case 1:
                return f.AccessReviewed ? "Beside " + AreaName(a) + ", mark future trees [C] 8 m apart: " + a.FutureCount + " of " + ScenarioOneRackBay.FutureTreesWanted + "." : null;
            case 2:
                return "Select a competitor [1–5] and mark it [X] to release each future tree, then open the Work Plan [Tab].";
            case 3:
                if (f.ReleaseExecuted && !f.DeadwoodExecuted) return "Give one more removal the other fate (sold or deadwood), approve and advance a year.";
                return a.MixedPlanned ? "Approve the work in the Work Plan [Tab] and advance a year." : "Leave some trees as deadwood and sell others, in the Work Plan [Tab].";
            case 4:
                if (!f.ReleaseReviewed) return "Read the review, then walk back and inspect each future tree.";
                return !f.AreaReleased ? "Release the future trees further, up to about " + Pct(ScenarioOneRackBay.FurtherReleaseShare) + " each." : null;
        }
        return null;
    }

    public static string Progress(int lesson, ScenarioOneTeachingSequence.Facts f)
    {
        ScenarioOneRackBay.Analysis a = Of(f);
        if (a == null) return null;
        switch (lesson)
        {
            case 1:
            {
                if (!f.AccessReviewed) return null;
                string line = "Area: " + AreaName(a) + " · future trees " + a.FutureCount + " of " + ScenarioOneRackBay.FutureTreesWanted;
                if (a.TooClose.Count > 0) line += " · " + a.TooClose.Count + " too close";
                if (a.Overtopped.Count > 0) line += " · " + a.Overtopped.Count + " overtopped";
                return line;
            }
            case 2:
                return "Released: " + Released(a, ScenarioOneRackBay.FirstReleaseShare, true) + (a.CanopyKept ? "" : " · too much removed");
            case 3:
            {
                int sold = a.SoldExecuted + a.SoldPlanned, left = a.DeadwoodExecuted + a.DeadwoodPlanned;
                return "To sell " + sold + " · deadwood " + left + (f.ReleaseExecuted ? " · work done" : " · work not yet done");
            }
            case 4:
                return "Released: " + Released(a, ScenarioOneRackBay.FurtherReleaseShare, true);
        }
        return null;
    }
}

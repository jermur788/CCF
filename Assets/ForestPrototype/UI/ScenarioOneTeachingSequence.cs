// Scenario One teaching is a read-only projection of the saved forest and management history.
// These lesson IDs are presentation identifiers, not the eight scenario success goals.
public static class ScenarioOneTeachingSequence
{
    public sealed class Facts
    {
        public bool AccessOpen, AccessReviewed, ReleasePlanned, ReleaseExecuted, DeadwoodExecuted;
        public bool ReleaseReviewed, ReviewedTwoCycles, BeechEstablished, OakEstablished, EstablishmentReviewed;
        // Objective 5: the future trees have been released as far as the further-release level and the result looked at again. True by
        // default, so a stage injected by a harness, or a stand with no rack area, keeps its earlier meaning.
        public bool AreaReleased = true;
        public int Year, CropTrees, PlantingYear = -1;
        // What the rack-area analysis found (lessons 2-5); null when the facts were injected or the stand has no rack layout. Read only
        // for wording: the lesson order below uses the plain facts above.
        public ScenarioOneRackBay.Analysis Bay;
    }
    public sealed class Lesson
    {
        // Concept = why it matters, Action = what to do, Practice = what good practice looks like (only where the lesson teaches it),
        // Caption = what the lesson's picture shows, HudAction = the one-line instruction on the HUD.
        public readonly string Id, Title, Concept, Action, Caption, HudAction, Practice;
        public Lesson(string id, string title, string concept, string action, string caption, string hudAction = "", string practice = "")
        { Id = id; Title = title; Concept = concept; Action = action; Caption = caption; HudAction = hudAction; Practice = practice; }
        // The title without its leading "1. ": the HUD widget shows the number separately.
        public string Heading { get { int dot = Title.IndexOf(". ", System.StringComparison.Ordinal); return dot >= 0 && dot <= 2 ? Title.Substring(dot + 2) : Title; } }
    }
    public static readonly Lesson[] Lessons =
    {
        new Lesson("access", "1. Create access racks",
            "A rack is a permanent access corridor. The harvesting machine drives along it, so the rows between racks can be thinned later without driving through standing trees. Planning access first is what makes careful, selective work possible: every later decision depends on it.",
            "Press B, click the tree or ground point where a rack should start, aim along the rows and click again to add it to the Work Plan. Finish the marked routes on the Stand Map [M], then approve the work in the Work Plan [Tab] and advance one year.",
            "Racks provide access while the rows between them remain standing.", "Press B, pick where a rack starts, aim forward and click to add it.",
            "Racks run straight and stay open for good, so plan the whole layout deliberately before you approve it. Take only the trees on each rack's line and leave the rows between racks standing: that standing forest is what you are keeping. Check the cost in the Work Plan before approving."),
        new Lesson("future-trees", "2. Choose future trees in one rack area",
            "Look at what the racks opened, then work ONE rack area: the standing trees beside the middle rack. A future tree is a positive choice to keep a tree for its later development. Choose three, far enough apart that each has growing space of its own; their neighbours will turn out to be shared.",
            "Walk to the middle rack, where the track crosses it. Inspect trees beside it [E], compare each with its neighbours, and mark three future trees [C] at least 8 m apart.",
            "Choose trees worth keeping, spaced so each has room.", "Beside the middle rack, inspect trees [E] and mark three future trees [C], 8 m apart.",
            "A future tree is bigger than most of the trees around it, so its crown is not squeezed by larger neighbours. Two future trees side by side would compete with each other: spread your choices so each keeps its own growing space."),
        new Lesson("release", "3. Release the future trees",
            "A neighbour takes growing space in proportion to its size and how close it stands. Taking out a future tree's strongest competitors releases it, but one removal can free two future trees, and removing more is not automatically better: the area must stay a forest.",
            "Inspect each future tree [E]. Select a listed competitor with [1–5] and mark it [X] (or aim at a neighbour and press [X]). Mark until your planned removals supply about a third of each future tree's competition, then add them to the Work Plan [Tab].",
            "Remove each future tree's strongest competitors; keep the canopy.", "Select competitors [1–5], mark them [X] to release each future tree, then open the Work Plan [Tab].",
            "Start with the closest, largest neighbours: they take the most space. Look for a competitor that serves two future trees. Keep removals a minority of the trees in the area, so it is still woodland afterwards."),
        new Lesson("habitat", "4. Decide what happens to the felled trees",
            "Every felled tree can be sold or left as fallen deadwood. Selling brings income; leaving a tree forgoes its sale but adds habitat. Decide tree by tree, then carry the work out.",
            "In Work Plan [Tab], leave some of the release trees as deadwood and sell others. Compare the money, approve the work, then advance one year.",
            "Some felled wood stays as habitat. Keeping it forgoes its sale.", "In the Work Plan [Tab], sell some release trees and leave others as deadwood, approve, advance a year.",
            "Small stems earn little or nothing as timber here, so they are the natural choice for deadwood. Sell the trees whose timber is worth taking."),
        new Lesson("response", "5. Review the response and release further",
            "CCF is repeated management. After a year, read what changed, then look at each future tree again: has its growing space improved enough? Release it further where it has not. An opening is an opportunity, not a promise of regeneration.",
            "Read the Annual Review and acknowledge it. Inspect each future tree again and mark further competitors until your removals together supply about two fifths of its competition. Approve, advance a year and review the result.",
            "Compare the response with your intention; release further where needed.", "Read the review, re-inspect the future trees and release further where they are still crowded."),
        new Lesson("recruitment", "6. Secure the next generation",
            "Young trees need suitable light and protection from browsing and vegetation. Inspect existing regeneration first. This scenario also asks you to introduce beech and sessile oak; planting gives a player-controlled route, but survival is not guaranteed.",
            "Use map layers to choose sites, then inspect them on foot. Buy nursery stock and plan beech and sessile oak. Review browsing and the optional shelter choice, approve planting, then advance one year.",
            "Inspect existing regeneration first. Consider suitable light, browsing and vegetation before planting; an optional shelter does not guarantee survival.", "Check light and young trees, then plan beech and sessile oak planting in the Work Plan [Tab]."),
        new Lesson("establishment", "7. Return to the young trees",
            "Establishment is not the same as successful recruitment. Let another year pass, then compare survival, browsing and vegetation with the planting-year results. Failure is useful information too.",
            "Read the planting results first. Advance one further year, read and acknowledge that Annual Review, then revisit the planted sites. Adapt any later work to what you find.",
            "The same planted sites are revisited after another year. Compare survival, browsing and vegetation with the planting-year results; this example includes failure despite a shelter.", "Advance a year, read the review, then revisit the planted trees."),
        new Lesson("transformation", "8. Continue the transformation",
            "The woodland is not finished after a few interventions. Continue observing and adapting while keeping tree cover. The eight scenario goals remain separate from these lessons.",
            "Review All scenario goals below and the management history. Revisit the forest and plan later work only where justified; advance time and reassess until the scenario goals are met.",
            "Retained forest, young trees and observed failure remain together. Reassess before any further work; the woodland is never finished.", "Keep observing and adapting; all scenario goals are listed under [O].")
    };

    public static int Current(Facts f)
    {
        if (!f.AccessOpen) return 0;
        if (!f.AccessReviewed || (f.CropTrees == 0 && !f.ReleaseExecuted)) return 1;
        if (!f.ReleasePlanned && !f.ReleaseExecuted) return 2;
        if (!f.ReleaseExecuted || !f.DeadwoodExecuted) return 3;
        if (!f.ReleaseReviewed || !f.ReviewedTwoCycles || !f.AreaReleased) return 4;
        if (!f.BeechEstablished || !f.OakEstablished) return 5;
        if (f.PlantingYear < 0 || f.Year <= f.PlantingYear || !f.EstablishmentReviewed) return 6;
        return 7;
    }
}

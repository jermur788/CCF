// Scenario One teaching is a read-only projection of the saved forest and management history.
// These lesson IDs are presentation identifiers, not the eight scenario success goals.
public static class ScenarioOneTeachingSequence
{
    public sealed class Facts
    {
        public bool AccessOpen, AccessReviewed, ReleasePlanned, ReleaseExecuted, DeadwoodExecuted;
        public bool ReleaseReviewed, ReviewedTwoCycles, BeechEstablished, OakEstablished, EstablishmentReviewed;
        public int Year, CropTrees, PlantingYear = -1;
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
        new Lesson("future-trees", "2. Choose future trees",
            "Look at what the rack work changed before choosing trees to favour. A Crop Tree is a positive choice to retain a tree for its future development.",
            "Walk back to the opened stand. Inspect a living tree [E], compare its neighbours, then mark a tree worth keeping [C].",
            "Inspect the opened stand and positively choose future trees to retain.", "Inspect trees [E], then mark one worth keeping as a Crop Tree [C]."),
        new Lesson("release", "3. Release a future tree",
            "A selected competitor can restrict a future tree's growing space. Selective removal changes competition while leaving a forest; removing more is not automatically better.",
            "Inspect a Crop Tree and its listed neighbours [1–5]. Mark a relevant competitor [X], then open Work Plan [Tab] to review the release and residual stand.",
            "Remove only selected competitors; more removal is not automatically better.", "Mark one competitor of a Crop Tree to remove [X], then open the Work Plan [Tab]."),
        new Lesson("habitat", "4. Keep some fallen wood",
            "A felled tree can remain as habitat rather than be extracted. Decide the fate of the timber as part of this intervention; retaining wood also forgoes its sale.",
            "In Work Plan [Tab], choose Leave as deadwood for a felling job if needed. Review costs and remaining canopy, approve the work, then advance one year.",
            "Some felled wood stays as habitat. Keeping this wood forgoes its sale.", "In the Work Plan [Tab], choose what happens to the felled wood, approve, advance a year."),
        new Lesson("response", "5. Review and manage again",
            "CCF is repeated management. Compare the work and forest response with your intention, then reassess before the next intervention. An opening is an opportunity, not a promise of regeneration.",
            "Read the Annual Review and acknowledge the results. Compare the intervention history, then walk back to inspect the opening, retained trees and fallen wood.",
            "Time has passed after the previous work. Compare the response with your intention; the opening is still unfilled.", "Read the Annual Review, then walk back and reassess before acting again."),
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
        if (!f.ReleaseReviewed || !f.ReviewedTwoCycles) return 4;
        if (!f.BeechEstablished || !f.OakEstablished) return 5;
        if (f.PlantingYear < 0 || f.Year <= f.PlantingYear || !f.EstablishmentReviewed) return 6;
        return 7;
    }
}

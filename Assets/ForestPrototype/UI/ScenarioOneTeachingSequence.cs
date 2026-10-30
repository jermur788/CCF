// Scenario One teaching is a read-only projection of the saved forest and management history.
// These lesson IDs are presentation identifiers, not the eight scenario success goals.
public static class ScenarioOneTeachingSequence
{
    public sealed class Facts
    {
        public bool AccessOpen, AccessReviewed, ReleasePlanned, ReleaseExecuted, DeadwoodExecuted;
        public bool ReleaseReviewed, ReviewedTwoCycles, BeechEstablished, OakEstablished, EstablishmentReviewed;
        // Rack-layout worlds need real post-release review. Null-Bay legacy/injected facts retain their earlier progression.
        public bool RackReviewComplete;
        public bool FirstGuidedRackReviewComplete;
        public int RevisitedTrees, ReviewDecisions, StandPortionsReviewed;
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
        new Lesson("future-trees", "2. Choose promising trees to retain along the rack",
            "Inspect and compare promising trees to retain and grow on. Work along the whole teaching area beside the middle rack, rather than choosing a cluster in one place. Your choices are the retained trees whose relationships you will manage.",
            "Inspect trees [E] and mark three Crop Trees [C]: one in the near, middle and far teaching zones. Keep them at least 8 m apart, with both rack sides represented between your three choices.",
            "Distribute attention along the rack and compare individuals.", "Choose one Crop Tree in each zone, 8 m apart, using both rack sides.",
            "Three trees and 8 m spacing are teaching choices tied to this game's competition neighbourhood, not a forestry prescription. Spacing alone does not distribute your attention or prove crown position."),
        new Lesson("release", "3. Identify the competitors that matter",
            "Compare neighbour relationships and competition Share around each retained tree. Selective removal can help more than one tree, but being small is not a reason to fell. Retain useful structure and exercise restraint.",
            "Inspect each Crop Tree [E]. Compare Share and position; select a listed competitor [1–5] and mark it [X]. Plan in-footprint removals supplying about 30% of each tree's measured competition, then review the Work Plan [Tab].",
            "Consider meaningful competitors and shared relationships; keep retained woodland.", "Compare Share, mark relevant competitors [X], then review the Work Plan [Tab].",
            "The initial 30% target and 40% cumulative trunk-area safeguard are gameplay calibration. Neighbours outside the teaching footprint still compete, but their removal earns no lesson credit."),
        new Lesson("habitat", "4. Plan and carry out the intervention",
            "Work across the teaching area while retaining forest structure. Selling timber brings income; leaving some felled wood forgoes that sale and adds deadwood habitat. Compare these outcomes in their actual positions.",
            "In Work Plan [Tab], assign both sold timber and fallen deadwood to the selected release trees. Check the retained stand and cumulative safeguard, approve the work, then advance one year.",
            "Some felled wood is sold and some stays beside the retained forest.", "Compare sold and deadwood outcomes, approve the Work Plan [Tab], advance a year.",
            "Consider habitat and forgone sale together. No single stem size is automatically the right deadwood choice. Unrelated felling elsewhere cannot satisfy this lesson."),
        new Lesson("response", "5. Return and review",
            "Did your intervention create enough growing space? Revisit all three retained trees and review the surrounding stand along both sides of the rack. Good CCF management includes deciding not to cut.",
            "Read and acknowledge the Annual Review, then close and reopen each Crop Tree's inspection [E]. Inspect the surrounding retained stand in each zone on both sides. While inspecting, choose [J] Retain here, or mark justified additional competitors and choose [K] Release further. No additional felling is required.",
            "Compare the response with your intention; retaining trees without another cut is success.", "Revisit all three trees and retained stand: [J] retain here, [K] justified further release.",
            "Retain here is a successful decision. Additional work must stay within the teaching footprint and safeguard to count. Revisit after further executed work. Individual inspection decisions are not stored. Once this first review is complete, the lesson remembers completion after loading."),
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
        => f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f) : CurrentUnlatched(f);

    // The pre-amendment sequence, also used to prove the write boundary without consulting its own latch.
    public static int CurrentUnlatched(Facts f)
    {
        if (!f.AccessOpen) return 0;
        if (!f.AccessReviewed || (f.CropTrees == 0 && !f.ReleaseExecuted) || (f.Bay != null && !f.Bay.HasEnoughFuture)) return 1;
        if (f.Bay != null && !f.Bay.CanopyKept) return 2;
        if (!f.ReleasePlanned && !f.ReleaseExecuted) return 2;
        if (!f.ReleaseExecuted || !f.DeadwoodExecuted) return 3;
        if (!f.ReleaseReviewed || (f.Bay != null ? !f.RackReviewComplete : !f.ReviewedTwoCycles || !f.AreaReleased)) return 4;
        return RecruitmentAndLater(f);
    }

    public static bool CrossesFirstRackReview(Facts before, Facts after)
        => before != null && after != null && before.Bay != null && after.Bay != null
            && !before.RackReviewComplete && after.ReleaseReviewed && after.RackReviewComplete
            && CurrentUnlatched(before) == 4 && CurrentUnlatched(after) == 5;

    private static int RecruitmentAndLater(Facts f)
    {
        if (!f.BeechEstablished || !f.OakEstablished) return 5;
        if (f.PlantingYear < 0 || f.Year <= f.PlantingYear || !f.EstablishmentReviewed) return 6;
        return 7;
    }
}

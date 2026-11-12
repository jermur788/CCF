// Scenario One teaching is a read-only projection of the saved forest and management history.
// These lesson IDs are presentation identifiers, not the eight scenario success goals.
public static class ScenarioOneTeachingSequence
{
    // Explicit navigation allowlist: commitments, destructive controls and modal choices are never animated.
    public static bool SafePulseTarget(string name) => name == "objective-primary-cta" || name == "objective-work-plan"
        || name == "objective-back-to-forest" || name == "map-work-plan" || name == "map-back-to-forest"
        || name == "map-teaching-waypoint" || name == "map-treatment-review" || name == "map-plan-rack"
        || name == "workplan-read-results" || name == "annual-review-acknowledge";

    public static string SourceQualifier(HistoryProvenance kind)
    {
        switch(kind) {
            case HistoryProvenance.Derived: return "calculated from records";
            case HistoryProvenance.Reconstructed: return "estimated / reconstructed from records";
            case HistoryProvenance.SessionOnly: return "this session only";
            case HistoryProvenance.NotRecorded: return "not recorded";
            case HistoryProvenance.Current: return "now";
            default: return "";
        }
    }

    public sealed class Facts
    {
        public bool AccessOpen, AccessReviewed, ReleasePlanned, ReleaseExecuted, DeadwoodExecuted;
        public bool ReleaseReviewed, ReviewedTwoCycles, BeechEstablished, OakEstablished, EstablishmentReviewed;
        // Rack-layout worlds need real post-release review. Null-Bay legacy/injected facts retain their earlier progression.
        public bool RackReviewComplete;
        public bool FirstGuidedRackReviewComplete;
        // MFM-09 "Extend your treatment". Both are derived from existing records on every read and never saved: the extension is
        // offered only in a Rack Planning V2 world with a confirmed plan (where the Treatment review exists), and it is done once
        // a stored prescription has been approved. Injected/legacy facts leave both false, so their sequence is the old one.
        public bool ExtensionApplies, ExtensionApproved;
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
            "Compare neighbour relationships and competition Share around each retained tree. Selective removal can help more than one tree. In this crown-release exercise, smaller neighbours usually contribute less to the measured Share. Retain useful structure and exercise restraint.",
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

    // MFM-09: the one lesson added after the eight above. It is deliberately NOT in Lessons: Lessons and Current/CurrentUnlatched keep
    // the accepted stage numbers (the first-guided-review latch and every earlier predicate read them). The extension is placed
    // between "response" and "recruitment" only in what the player sees (PresentedLessons / Presented).
    public const string ExtensionId = "extend-treatment";
    public static readonly Lesson Extension = new Lesson(ExtensionId, "6. Extend your treatment",
        "Your sample showed one careful treatment. Comparable areas of the stand may call for the same kind of judgement, but the sample is teaching calibration, not a stocking prescription for the whole stand. The game derives a prescription from the decisions you actually carried out, checks it against your own sample, and shows which rack-served areas are covered, not covered, excluded or left manual.",
        "Open the Stand Map [M] and choose Treatment review. Read your sample and the comparable areas, then choose the covered areas to apply it to or leave manual. Inspect each spacing-only Crop Tree candidate in the forest and confirm or reject it. Review the proposed release, spot-check at least one extrapolated tree in 3D, then revise, exclude or keep work manual where it does not look right. Approve the prescription. It adds ordinary Pending tasks with their origin recorded; open the Work Plan [Tab] and approve them there to commit the work.",
        "A sample treatment is extended to comparable areas only after you have inspected and confirmed it.", "Stand Map [M] > Treatment review: check, confirm, spot-check, approve; then approve the Pending work in the Work Plan [Tab].",
        "Spacing only suggests where to inspect; you judge stem form, branching and vigour in 3D. A proposal is not a Fell mark and not work. Approving the prescription is not approving the Work Plan: nothing is committed, funded or carried out until you approve it there. Manual work stays available.");

    // The sequence as the player sees it, in order: the eight accepted lessons with the extension between response and recruitment.
    public static readonly Lesson[] PresentedLessons =
    {
        Lessons[0], Lessons[1], Lessons[2], Lessons[3], Lessons[4], Extension, Lessons[5], Lessons[6], Lessons[7]
    };

    // True only while the player stands at the accepted recruitment stage (which Current reaches only after the genuine response review)
    // and has not yet approved a prescription. It never changes Current, so it cannot reopen or move the first guided review.
    public static bool ExtensionOffered(Facts f) => f.ExtensionApplies && !f.ExtensionApproved && Current(f) == 5;

    // Index into PresentedLessons for what the player is shown now. Always a PresentedLessons index, so a world without the extension
    // (Legacy40, injected facts) still reads the right lesson; it simply never shows the extension and Number/Total skip it.
    public static int Presented(Facts f)
    {
        int stage = Current(f);
        if (ExtensionOffered(f)) return 5;
        return stage >= 5 ? stage + 1 : stage;
    }

    // Where a lesson sits in the sequence this player sees: 1-based, with the extension counted only in worlds that have it.
    public static int Number(Lesson lesson, Facts f)
        => System.Array.IndexOf(f.ExtensionApplies ? PresentedLessons : Lessons, lesson) + 1;
    public static bool InSequence(Lesson lesson, Facts f) => f.ExtensionApplies || lesson != Extension;
    public static int Total(Facts f) => f.ExtensionApplies ? PresentedLessons.Length : Lessons.Length;
    public static string DisplayTitle(Lesson lesson, Facts f) => Number(lesson, f) + ". " + lesson.Heading;

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

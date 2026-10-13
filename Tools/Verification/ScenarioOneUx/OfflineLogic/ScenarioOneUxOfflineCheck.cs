// Offline check for the pure player-facing wording and rules behind the Scenario One UX gap patch: the lesson list, the
// help text and the advance guard. Compiles the PRODUCTION files that hold them (none uses UnityEngine) and runs on the
// plain .NET SDK; see run_ux_check.py.
//
// This proves wording, ordering and decision rules only. It does not prove that any screen renders, that a modal opens or
// closes, or that text fits at a given resolution: those need the Unity checks listed in
// Docs/Verification/ScenarioOneUxGap/Handoff.md.
//
// Prints SCENARIO_ONE_UX_PASS / SCENARIO_ONE_UX_FAIL.
using System;
using System.Collections.Generic;
using System.Linq;

public static class ScenarioOneUxOfflineCheck
{
    private static int failures, checks;

    public static int Main()
    {
        SavedProgressKeys();
        RoleBriefing();
        AdvanceWarning();
        FirstUseBudget();
        TeachingOrder();
        Console.WriteLine(failures == 0 ? $"SCENARIO_ONE_UX_PASS checks={checks}" : $"SCENARIO_ONE_UX_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // Every lesson step id is the key of a value saved on the player's device (CCF.Learning.v1.<id>). Reordering lessons is
    // fine; losing or renaming an id silently discards a tester's progress, so the set is pinned here.
    private static readonly string[] SavedStepIds =
    {
        "map.open", "map.select", "map.light", "map.waypoint", "map.return", "map.arrive", "map.inspectsite", "map.regeneration",
        "map.browse", "map.marks", "map.compare", "tree.inspect", "tree.read", "tree.crop", "fell.mark", "fell.plan", "fell.cost",
        "fell.approve", "fell.result", "prune.read", "prune.plan", "prune.approve", "prune.result", "deadwood.read", "deadwood.plan",
        "deadwood.result", "deadwood.visit", "plant.ground", "plant.stock", "plant.plan", "plant.execution", "plant.shelter",
        "plant.approve", "plant.result", "clear.read", "clear.plan", "clear.approve", "clear.result", "plan.open", "plan.read",
        "review.open", "review.read"
    };

    private static int Words(string text) => text.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    private static bool Has(string text, string part) => text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;

    // The first thing a beginner reads: who they are, what they are trying to do, that it takes years, and that there is no
    // single right answer. Short, and no detailed ecology or mechanics yet.
    private static void RoleBriefing()
    {
        string brief = MenuHelpView.Summary(MenuHelpView.Menu.WalkingHud);
        Check(brief != null, "the walking HUD has a short first-use text");
        if (brief == null) return;
        Check(Words(brief) <= 130, "the role briefing is short enough to read before play", Words(brief) + " words");
        Check(Has(brief, "plantation"), "briefing: the player manages a small plantation");
        Check(Has(brief, "continuous-cover"), "briefing: the objective is to begin converting toward continuous-cover management");
        Check(Has(brief, "over years") || Has(brief, "play out over years"), "briefing: decisions happen over years");
        Check(Has(brief, "keep") && Has(brief, "remove") && Has(brief, "Work Plan"), "briefing: choose trees to keep/remove and commission work");
        Check(Has(brief, "no single perfect prescription"), "briefing: the aim is not one perfect prescription");
        Check(Has(brief, "revisit"), "briefing: management is revisited as the forest changes");
        foreach (string jargon in new[] { "basal area", "DBH", "competition index", "browsing", "residual", "regeneration", "Hegyi" })
            Check(!Has(brief, jargon), "briefing does not front-load detail: " + jargon);
        Check(MenuHelpView.SummaryTitle(MenuHelpView.Menu.WalkingHud) != MenuHelpView.Title(MenuHelpView.Menu.WalkingHud), "the briefing has its own title");
        string full = MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud);
        Check(Has(full, "Your role") && Has(full, "repeated process") && Has(full, "not one thinning"), "the full walking help keeps the role and the repeated-process teaching");
        Check(Words(full) > Words(brief), "the full help is longer than the briefing");
    }

    // The year-wasting trap: planned work that was never approved is silently skipped when time advances.
    private static void AdvanceWarning()
    {
        AdvanceGuard.Decision none = AdvanceGuard.Evaluate(0, 0);
        Check(!none.Warn, "no planned work: advancing needs no question");
        AdvanceGuard.Decision unapproved = AdvanceGuard.Evaluate(3, 0);
        Check(unapproved.Warn, "planned work with nothing approved: the player is asked first");
        Check(unapproved.Headline == "You have planned work that has not been approved.", "the warning says the planned work has not been approved");
        Check(Has(unapproved.Detail, "3 planned tasks") && Has(unapproved.Detail, "pending"), "the warning counts the pending tasks");
        Check(Has(unapproved.Detail, "no work done"), "the warning says the year would pass with no work done");
        Check(Has(AdvanceGuard.Evaluate(1, 0).Detail, "1 planned task is"), "the warning reads correctly for a single task");
        Check(!AdvanceGuard.Evaluate(0, 4).Warn, "approved work and nothing pending: no question");
        Check(!AdvanceGuard.Evaluate(2, 1).Warn, "some work approved: the year does something, so no question");
        Check(!AdvanceGuard.Evaluate(-1, 0).Warn, "a negative count is treated as nothing pending");
        // Waiting must stay possible, and approving must stay the player's own click.
        Check(AdvanceGuard.AdvanceAnywayLabel == "Advance anyway", "the player can advance anyway");
        Check(AdvanceGuard.ReturnLabel == "Back to the plan" && AdvanceGuard.ApproveLabel == "Approve pending work", "the player can go back or approve");
        Check(Has(unapproved.Detail, "advance anyway") && Has(unapproved.Detail, "approve"), "the warning names both ways forward");
        Check(!Has(unapproved.Detail, "automatically"), "nothing is approved on the player's behalf");
    }

    // The packet's finding: roughly 770 words spread over five modal intros. First use must now be brief, with the full text
    // still reachable, and the Annual Review must not open a help screen over its own acknowledgement.
    private static void FirstUseBudget()
    {
        int firstUse = 0, full = 0;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
        {
            string brief = MenuHelpView.Summary(menu);
            string detail = MenuHelpView.Explanation(menu);
            full += Words(detail);
            if (brief == null) continue;
            firstUse += Words(brief);
            Check(Words(brief) <= 130, "first-use text is brief for " + menu, Words(brief) + " words");
            Check(Words(detail) > Words(brief), "the full help for " + menu + " is still more detailed than its first-use text");
        }
        Check(full > 600, "the detailed help is kept, not deleted", full + " words");
        Check(firstUse <= 300, "all first-use screens together stay short", firstUse + " words");
        Check(firstUse * 2 < full, "first use is less than half of the full help text", firstUse + " of " + full + " words");
        Check(MenuHelpView.Summary(MenuHelpView.Menu.AnnualReview) == null, "the Annual Review has no first-use help screen to stack over its acknowledgement");
        string prompt = MenuHelpView.AnnualReviewReadPrompt;
        Check(Has(prompt, "WORK DONE") && Has(prompt, "MONEY") && Has(prompt, "FOREST") && Has(prompt, "reassess") && Has(prompt, "walk back"),
            "the review itself tells the player what to read and to walk back and reassess");
        Check(Words(prompt) <= 50, "the in-review prompt is short", Words(prompt) + " words");
        Check(Has(MenuHelpView.Summary(MenuHelpView.Menu.TreeInspection) ?? "", "proposals") && Has(MenuHelpView.Summary(MenuHelpView.Menu.TreeInspection) ?? "", "Crop Tree"),
            "tree inspection first-use: marks are proposals, Crop Tree explained");
        Check(Has(MenuHelpView.Summary(MenuHelpView.Menu.StandMap) ?? "", "not what to do"), "stand map first-use: navigation aid, not remote management");
        Check(Has(MenuHelpView.Summary(MenuHelpView.Menu.WorkPlan) ?? "", "Approval alone changes nothing"), "work plan first-use: approval is not execution");
    }

    // First cycle: look at trees, choose a Crop Tree, mark, plan, approve, advance, read the review. The map is available but
    // must not be the first persistent "next lesson".
    private static void TeachingOrder()
    {
        string[] titles = LearningObjectivesView.Topics.Select(t => t.Title).ToArray();
        Check(titles.Length == 8, "eight lesson topics");
        Check(titles[0].EndsWith("Tree inspection and Crop Trees") && titles[1].EndsWith("Felling") && titles[2].EndsWith("Work Plan and Annual Review"),
            "the first topics follow the first cycle", string.Join(" | ", titles));
        Check(Array.FindIndex(titles, t => t.EndsWith("Stand Map")) == 3, "the Stand Map follows the first cycle");
        for (int i = 0; i < titles.Length; i++)
            Check(titles[i].StartsWith((i + 1) + ". "), "topic numbering matches position", titles[i]);

        var done = new HashSet<string>();
        var cues = new List<string>();
        for (LearningObjectivesView.Step step = LearningObjectivesView.NextCueStep(done.Contains); step != null && cues.Count < 60;
             step = LearningObjectivesView.NextCueStep(done.Contains))
        {
            cues.Add(step.Id);
            done.Add(step.Id);
        }
        Check(cues.Count == SavedStepIds.Length && cues.Distinct().Count() == cues.Count, "walking the cue visits every step exactly once", cues.Count + " cues");
        string[] firstCycle = { "tree.inspect", "tree.crop", "fell.mark", "fell.plan", "fell.approve", "fell.result", "plan.open", "review.open", "review.read" };
        Check(cues.Take(firstCycle.Length).SequenceEqual(firstCycle), "the first cues are the first cycle", string.Join(",", cues.Take(firstCycle.Length)));
        Check(!cues.Take(firstCycle.Length).Any(id => id.StartsWith("map.")), "no map step is among the first cues");
        Check(cues[firstCycle.Length] == "map.open", "map practice follows once the first cycle is done");
        // Reading acknowledgements never hold up the cue while something can still be done in the forest.
        int firstReading = cues.FindIndex(id => LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == id).Reading);
        int lastAction = cues.FindLastIndex(id => !LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == id).Reading);
        Check(lastAction < firstReading, "every action step is cued before any reading step");
        Check(LearningObjectivesView.CueText(LearningObjectivesView.NextCueStep(id => false)) == "Next lesson: Inspect a living tree · [O] Objectives", "the first cue reads as an invitation to inspect a tree");
        Check(LearningObjectivesView.CueText(null) == "Learning complete · revisit with O", "all done reads as complete");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "inspect trees, choose Crop Trees") && !Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "start with simple map steps"),
            "the walking help describes the new lesson order");
    }

    private static void SavedProgressKeys()
    {
        var ids = LearningObjectivesView.StepIds.ToList();
        Check(ids.Count == ids.Distinct().Count(), "step ids are unique");
        Check(ids.OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(SavedStepIds.OrderBy(x => x, StringComparer.Ordinal)),
            "the set of saved step ids is unchanged", string.Join(",", ids.Except(SavedStepIds).Concat(SavedStepIds.Except(ids))));
        foreach (LearningObjectivesView.Topic topic in LearningObjectivesView.Topics)
            foreach (LearningObjectivesView.Step step in topic.Steps)
                Check(step.Requires == null || ids.Contains(step.Requires), "prerequisite exists for " + step.Id);
        Check(LearningObjectivesView.PreferencePrefix == "CCF.Learning.v1.", "learning preference prefix is unchanged");
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
        {
            Check(!string.IsNullOrWhiteSpace(MenuHelpView.Title(menu)), "help title exists for " + menu);
            Check(!string.IsNullOrWhiteSpace(MenuHelpView.Explanation(menu)), "full help text exists for " + menu);
        }
    }
}

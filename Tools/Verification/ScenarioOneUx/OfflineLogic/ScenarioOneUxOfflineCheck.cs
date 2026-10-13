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

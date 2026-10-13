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

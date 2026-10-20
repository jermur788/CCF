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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class ScenarioOneUxOfflineCheck
{
    private static int failures, checks;

    public static int Main(string[] args)
    {
        SequentialLessons();
        SavedProgressKeys();
        RoleBriefing();
        AdvanceWarning();
        FirstUseBudget();
        TeachingOrder();
        ControlsAndTerminology();
        MarketWording();
        StaleCopyScan(args.Length > 0 ? args[0] : null);
        Console.WriteLine(failures == 0 ? $"SCENARIO_ONE_UX_PASS checks={checks}" : $"SCENARIO_ONE_UX_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void SequentialLessons()
    {
        var f = new ScenarioOneTeachingSequence.Facts();
        void Stage(int n, string why) => Check(ScenarioOneTeachingSequence.Current(f) == n, why);
        Stage(0, "access first"); f.Year = 9; Stage(0, "idle years never replace rack work");
        f.AccessOpen = true; Stage(1, "post-access observation before selection");
        f.CropTrees = 3; Stage(1, "early marks cannot bypass access review");
        f.AccessReviewed = true; f.CropTrees = 0; Stage(1, "positive selection required");
        f.CropTrees = 1; Stage(2, "one meaningful crop tree, no arbitrary quota");
        f.ReleasePlanned = true; Stage(3, "habitat decision before release execution");
        f.DeadwoodExecuted = true; Stage(3, "deadwood alone cannot replace selective release");
        f.ReleaseExecuted = true; Stage(4, "return to review after second intervention");
        f.ReleaseReviewed = true; Stage(4, "review chronology still authoritative");
        f.ReviewedTwoCycles = true; Stage(5, "next generation follows reviewed repeat management");
        f.BeechEstablished = true; Stage(5, "both existing introduced species goals retained");
        f.OakEstablished = true; f.PlantingYear = f.Year; Stage(6, "planting year not enough observation time");
        f.EstablishmentReviewed = true; Stage(6, "early review does not remove year gate");
        f.Year++; f.EstablishmentReviewed = false; Stage(6, "later annual results need acknowledgement");
        f.EstablishmentReviewed = true; Stage(7, "ongoing synthesis after later observation");
        f.Year += 5; Stage(7, "multiple extra years do not corrupt sequence");
        f.OakEstablished = false; Stage(5, "loss of introduced species gives useful recovery lesson");
        Check(ScenarioOneTeachingSequence.Lessons.Length == 8, "eight teaching lessons separate from scenario predicates");
        Check(ScenarioOneTeachingSequence.Lessons.Select(l => l.Id).Distinct().Count() == 8, "unique stable teaching IDs");
        foreach (var lesson in ScenarioOneTeachingSequence.Lessons)
        {
            Check(!string.IsNullOrWhiteSpace(lesson.Concept) && !string.IsNullOrWhiteSpace(lesson.Action), "why and action: " + lesson.Id);
            Check(!string.IsNullOrWhiteSpace(lesson.Caption), "visual brief: " + lesson.Id);
        }
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
        "review.open", "review.read",
        // Added by the rack-first revision (the earlier ids above are unchanged, so saved progress is kept).
        "rack.read", "rack.create", "rack.layout", "rack.approve", "rack.result", "rack.map"
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
        Check(Has(brief, "next generation") && Has(brief, "productive"), "briefing: productive woodland and its next generation");
        Check(Has(brief, "even-aged") && Has(brief, "same time"), "briefing: the starting stand is explained plainly");
        Check(Has(brief, "one step at a time"), "briefing: sequential learning");
        Check(Has(brief, "access racks") && Has(brief, "start"), "briefing: access racks are the first action");
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
        Check(AdvanceGuard.ReturnLabel == "Back to the plan" && AdvanceGuard.ApproveLabel == "Approve Work Plan", "the player can go back or approve");
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
        Check(LearningObjectivesView.GuidedTopicIndex(true, false, 0, id => false) == 0, "fresh forest teaches rack objective first");
        Check(LearningObjectivesView.GuidedTopicIndex(true, false, 3, id => true) == 0, "old device credits and empty years cannot skip rack work");
        Check(LearningObjectivesView.GuidedTopicIndex(true, true, 0, id => false) == 0, "objective 2 waits for annual advance even if rack flag is ready");
        Check(LearningObjectivesView.GuidedTopicIndex(true, true, 1, id => false) == 1, "harvested racks plus one annual advance activate objective 2");
        Check(titles.Length == 9, "nine lesson topics");
        Check(titles[0].EndsWith("Create access racks") && titles[1].EndsWith("Tree inspection and Crop Trees") && titles[2].EndsWith("Felling") && titles[3].EndsWith("Work Plan and Annual Review"),
            "the first topics are the racks, then the first cycle of selective marking", string.Join(" | ", titles));
        Check(Array.FindIndex(titles, t => t.EndsWith("Stand Map")) == 4, "the Stand Map follows the first cycle");
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
        string[] firstCycle = { "rack.create", "rack.layout", "rack.approve", "rack.result", "tree.inspect", "tree.crop", "fell.mark", "fell.plan", "fell.approve", "fell.result", "plan.open", "review.open", "review.read" };
        Check(cues.Take(firstCycle.Length).SequenceEqual(firstCycle), "the first cues are the first cycle", string.Join(",", cues.Take(firstCycle.Length)));
        Check(!cues.Take(firstCycle.Length).Any(id => id.StartsWith("map.")), "no map step is among the first cues");
        Check(cues[firstCycle.Length] == "map.open", "map practice follows once the first cycle is done");
        // Reading acknowledgements never hold up the cue while something can still be done in the forest.
        int firstReading = cues.FindIndex(id => LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == id).Reading);
        int lastAction = cues.FindLastIndex(id => !LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == id).Reading);
        Check(lastAction < firstReading, "every action step is cued before any reading step");
        Check(LearningObjectivesView.CueText(LearningObjectivesView.NextCueStep(id => false)) == "Next lesson: Create your first rack · [O] Objectives", "the first cue reads as an invitation to create a rack");
        Check(cues.IndexOf("rack.result") < cues.IndexOf("tree.inspect") && cues.IndexOf("rack.result") < cues.IndexOf("fell.mark"), "selective marking is taught after the racks");
        Check(LearningObjectivesView.CueText(null) == "Learning complete · revisit with O", "all done reads as complete");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "create extraction racks") && Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "selective felling follow once the racks are open") && !Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "start with simple map steps"),
            "the walking help describes the new lesson order");
    }

    // Core forestry interactions must be in the controls list, and the startup card and the in-game help must agree.
    private static void ControlsAndTerminology()
    {
        string[][] rows = ControlsReference.Rows;
        string all = string.Join(" | ", rows.Select(r => r[0] + " " + r[1]));
        // The startup-screen verification asserts these key labels and phrases.
        foreach (string key in new[] { "W A S D", "Mouse", "E", "M", "Tab", "O", "F1", "Esc", "F5 / F9", "F10" })
            Check(rows.Any(r => r[0] == key), "controls list keeps key " + key);
        foreach (string phrase in new[] { "Stand Map", "Work Plan", "Learning objectives", "Session menu" })
            Check(Has(all, phrase), "controls list still names " + phrase);
        Check(rows.Any(r => r[0] == "E" && Has(r[1], "inspect")), "inspect is documented");
        Check(rows.Any(r => r[0] == "Left-click" && Has(r[1], "inspect")), "left-click inspection is documented");
        Check(rows.Any(r => r[0] == "X" && Has(r[1], "felling")), "the fell mark is documented");
        Check(rows.Any(r => r[0] == "C" && Has(r[1], "Crop Tree") && Has(r[1], "competitors")), "the Crop Tree mark and competitor assessment are documented");
        Check(rows.Any(r => r[0] == "1 – 5" && Has(r[1], "competitors")), "the number keys are documented");
        Check(rows.Any(r => r[0] == "Tab" && Has(r[1], "Work Plan")) && rows.Any(r => r[0] == "M" && Has(r[1], "Stand Map")), "Work Plan and map keys are documented");
        Check(rows.All(r => r.Length == 2 && r[0].Length > 0 && r[1].Length > 0 && r[1].Length <= 110), "every control row has a key and a short description");
        var inKeysLine = new Dictionary<string, string>
        {
            { "E", "E or left-click" }, { "Left-click", "left-click" }, { "X", "X mark" }, { "C", "C mark" }, { "1 – 5", "1–5" }, { "G", "G planting" },
            { "U", "U plan" }, { "M", "M Stand Map" }, { "Tab", "Tab Work Plan" }, { "O", "O Learning" }, { "F1", "F1 Help" }, { "F5 / F9", "F5 / F9" }, { "F10", "F10 Session" }
        };
        foreach (var pair in inKeysLine)
            Check(rows.Any(r => r[0] == pair.Key) && ControlsReference.KeysLine.Contains(pair.Value), "the in-game keys line agrees with the controls list for " + pair.Key);
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), ControlsReference.KeysLine), "the full Walking HUD help includes the keys line");

        // Jargon is expanded where it first appears.
        string tree = MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection);
        int expanded = tree.IndexOf("competition index (CI)", StringComparison.Ordinal);
        Match firstCi = Regex.Match(tree, @"\bCI\b");
        Check(expanded >= 0 && firstCi.Success && firstCi.Index == expanded + "competition index (".Length, "CI is spelled out where it first appears in the inspection help");
        string plan = MenuHelpView.Explanation(MenuHelpView.Menu.WorkPlan);
        Check(Has(plan, "residual stand is what remains standing") && Has(plan, "basal area, the trunk cross-section area per hectare"), "residual stand and basal area are defined in the Work Plan help");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.WalkingHud), "browsing is deer eating the shoots of young trees"), "browsing is defined in the Walking HUD help");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.StandMap), "deer eating the shoots of young trees"), "browsing is defined in the Stand Map help");
        Check(Has(MenuHelpView.Explanation(MenuHelpView.Menu.TreeInspection), "left-click"), "inspection help mentions left-click");
    }

    // Oak and beech income depends on the saved forest's growth model, so the lesson must follow the active state and never
    // claim a market is universally present or absent.
    private static void MarketWording()
    {
        string present = LearningObjectivesView.MarketSentence(true), absent = LearningObjectivesView.MarketSentence(false);
        Check(present != absent, "the market sentence follows the active model");
        Check(Has(present, "oak and beech") && Has(present, "timber prices") && !Has(present, "no sale income"), "with the market active, oak and beech are priced");
        Check(Has(absent, "only Sitka spruce earns timber income") && Has(absent, "no sale income") && !Has(absent, "have timber prices"), "without the market, oak and beech earn nothing");
        LearningObjectivesView.Step cost = LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == "fell.cost");
        Check(!Has(cost.Text, "no broadleaf timber market is configured"), "the fixed lesson text no longer claims there is no broadleaf market");
        Check(LearningObjectivesView.StepText(cost, true).EndsWith(present) && LearningObjectivesView.StepText(cost, false).EndsWith(absent), "the harvest-cost lesson appends the sentence for the active forest");
        LearningObjectivesView.Step other = LearningObjectivesView.Topics.SelectMany(t => t.Steps).First(s => s.Id == "fell.approve");
        Check(LearningObjectivesView.StepText(other, true) == other.Text && LearningObjectivesView.StepText(other, false) == other.Text, "other lessons are unchanged by the market state");
    }

    // Obsolete player-facing guidance must not creep back into the shipped sources.
    private static void StaleCopyScan(string root)
    {
        if (root == null) { Console.WriteLine("SKIP stale-copy scan (no repository root given)"); return; }
        string[] obsolete =
        {
            "Open the Century Review in the Work Plan", "private first-cycle test", "Manage one cycle", "no broadleaf timber market is configured in this scenario",
            "2. Buy Beech and Sessile Oak saplings from the nursery", "4. Advance one ecological year to execute approved work", "start with simple map steps"
        };
        string folder = Path.Combine(root, "Assets", "ForestPrototype");
        string[] files = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories).Where(f => !f.Contains(Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar)).ToArray();
        Check(files.Length > 50, "the stale-copy scan found the production sources", files.Length + " files");
        foreach (string phrase in obsolete)
        {
            string hit = files.FirstOrDefault(f => File.ReadAllText(f).Contains(phrase));
            Check(hit == null, "obsolete guidance is gone: \"" + phrase + "\"", hit == null ? "" : Path.GetFileName(hit));
        }
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

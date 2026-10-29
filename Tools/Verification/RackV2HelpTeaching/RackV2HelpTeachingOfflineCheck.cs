using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// RACK-V2-10 offline check: Tree Marks help, its unlock rule and the Rack Planning V2 Objective 1 wording, compiled from the
// production pure-wording files on plain .NET. It reads the approved spec for the quoted §7, §28 and §31 sentences, so the copy
// cannot drift from the spec without failing. It proves wording, entry set, unlock rule and flow order only, not rendering.
public static class RackV2HelpTeachingOfflineCheck
{
    static int checks, failures;
    static void Check(bool condition, string message, string detail = "")
    {
        checks++;
        if (!condition) failures++;
        Console.WriteLine((condition ? "PASS " : "FAIL ") + message + (condition || detail.Length == 0 ? "" : " :: " + detail));
    }

    static bool Has(string text, string part) => text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
    static bool InOrder(string text, params string[] parts)
    {
        int at = -1;
        foreach (string part in parts)
        {
            int next = text.IndexOf(part, at + 1, StringComparison.OrdinalIgnoreCase);
            if (next < 0) return false;
            at = next;
        }
        return true;
    }

    static readonly string[] RackIds = { "rack.read", "rack.create", "rack.layout", "rack.approve", "rack.result", "rack.map" };

    // The block-quoted sentences of one spec section (lines starting with "> " between that heading and the next "## ").
    static List<string> Quotes(string[] spec, string heading)
    {
        var quotes = new List<string>();
        int start = Array.FindIndex(spec, line => line.StartsWith(heading, StringComparison.Ordinal));
        if (start < 0) return quotes;
        for (int i = start + 1; i < spec.Length && !spec[i].StartsWith("## ", StringComparison.Ordinal); i++)
            if (spec[i].StartsWith("> ", StringComparison.Ordinal) && !spec[i].StartsWith("> **", StringComparison.Ordinal))
                quotes.Add(spec[i].Substring(2).Trim());
        return quotes;
    }

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : ".";
        string[] spec = File.ReadAllLines(Path.Combine(root, "specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md"));

        // 1. Menu enum: Tree Marks is a new screen at the end; existing preference keys (the names) are unchanged.
        string[] names = Enum.GetNames(typeof(MenuHelpView.Menu));
        Check(string.Join(",", names) == "WalkingHud,TreeInspection,StandMap,WorkPlan,AnnualReview,Rack,TreeMarks",
            "help screens: existing names and order kept, TreeMarks appended", string.Join(",", names));
        Check(MenuHelpView.Title(MenuHelpView.Menu.TreeMarks).StartsWith("Tree Marks", StringComparison.Ordinal), "the screen is titled Tree Marks");
        Check(MenuHelpView.Summary(MenuHelpView.Menu.TreeMarks) == null, "first use shows the full Tree Marks table and disclosure, not a shorter text");

        // 2. Entry set: orange, red and blue only (spec §25, §28, §29).
        string[][] entries = MenuHelpView.TreeMarkEntries;
        Check(entries.Length == 3, "Tree Marks has exactly three entries", entries.Length.ToString());
        Check(entries.Length == 3 && entries[0][0] == "Orange vertical stripes" && entries[0][1] == "Rack / machine-path removal",
            "entry 1: Orange vertical stripes — Rack / machine-path removal");
        Check(entries.Length == 3 && entries[1][0] == "Red" && entries[1][1] == "Fell", "entry 2: Red — Fell");
        Check(entries.Length == 3 && entries[2][0] == "Blue" && entries[2][1] == "Crop Tree / future timber", "entry 3: Blue — Crop Tree / future timber");
        string marks = MenuHelpView.Explanation(MenuHelpView.Menu.TreeMarks);
        foreach (string[] entry in entries)
            Check(marks.Contains("• " + entry[0] + " — " + entry[1]), "the help lists " + entry[0]);
        Check(Regex.Matches(marks, "^• ", RegexOptions.Multiline).Count == 3, "the help lists no fourth mark");
        // The only other colour named is the source's "orange or pink", inside the orange convention sentence.
        string outsideSource = marks.Replace(MenuHelpView.TreeMarksOrangeText, "");
        foreach (string colour in new[] { "pink", "yellow", "white", "green", "purple", "violet", "black" })
            Check(!Has(outsideSource, colour), "no unused mark colour outside the source sentence: " + colour);
        foreach (string future in new[] { "deadwood", "habitat", "route-side", "retained", "boundary", "future marks" })
            Check(!Has(marks, future), "no future marking category: " + future);

        // 3. Spec §28 wording: the orange convention and the first-tree simplification disclosure, verbatim.
        List<string> s28 = Quotes(spec, "## 28.");
        Check(s28.Count == 2, "spec §28 has its two quoted sentences", s28.Count.ToString());
        if (s28.Count == 2)
        {
            Check(MenuHelpView.TreeMarksOrangeText == s28[0], "orange convention text equals spec §28", MenuHelpView.TreeMarksOrangeText);
            Check(MenuHelpView.TreeMarksSimplificationText == s28[1], "simplification disclosure equals spec §28", MenuHelpView.TreeMarksSimplificationText);
        }
        Check(marks.Contains(MenuHelpView.TreeMarksOrangeText) && marks.Contains(MenuHelpView.TreeMarksSimplificationText),
            "the Tree Marks help shows both §28 sentences");
        Check(Has(marks, "four sides") && Has(marks, "one spray"), "the help separates the source's four-sided stripes from the game's single spray (§24)");
        Check(Has(marks, "press P") && Has(marks, "3.5 m") && Has(marks, "left-click"), "the help names the real spray controls (P, 3.5 m, left-click)");
        string pRow = ControlsReference.RowsFor(true).First()[1];
        Check(ControlsReference.RowsFor(true).First()[0] == "P" && Has(pRow, "3.5 m") && Has(pRow, "left-click"), "the controls reference states the same spray reach and click");
        Check(Has(marks, "only one of these marks") && Has(marks, "approval"), "the help states mark exclusivity and that marking cuts nothing");

        // 4. Unlock rule: only a Rack Planning V2 world with a confirmed plan; context while unseen or with the spray equipped.
        Check(MenuHelpView.TreeMarksUnlocked(true, true), "unlocked: V2 world with a confirmed plan");
        Check(!MenuHelpView.TreeMarksUnlocked(true, false), "locked: V2 plan not yet confirmed (or reopened)");
        Check(!MenuHelpView.TreeMarksUnlocked(false, true), "locked: Legacy40/model-0 world");
        Check(!MenuHelpView.TreeMarksUnlocked(false, false), "locked: model-0 world without a plan");
        foreach (bool introduced in new[] { false, true })
            foreach (bool spray in new[] { false, true })
            {
                Check(!MenuHelpView.TreeMarksContext(false, introduced, spray), $"never the context while locked (seen={introduced}, spray={spray})");
                Check(MenuHelpView.TreeMarksContext(true, introduced, spray) == (!introduced || spray),
                    $"unlocked context (seen={introduced}, spray={spray}) = {!introduced || spray}");
            }

        // 5. Objective 1 copy, spec §31 and §7 sentences in the V2 objective.
        string concept = LearningObjectivesView.RackV2Concept;
        List<string> s31 = Quotes(spec, "## 31.");
        Check(s31.Count == 3, "spec §31 has its three quoted sentences", s31.Count.ToString());
        foreach (string quote in s31) Check(concept.Contains(quote), "Objective 1 explains (§31): " + quote);
        List<string> s7 = Quotes(spec, "## 7.");
        Check(s7.Count == 2 && concept.Contains(s7[0]) && !concept.Contains(s7[1]), "Objective 1 states the five-rack calibration as §7 asks, not the quota", string.Join(" | ", s7));
        Check(InOrder(concept, "machinery", "16–20 m", "depends on the stand", "five extraction racks", "Plan the racks on the map", "mark each rack entrance"),
            "Objective 1 why-it-matters order: purpose, spacing, conditional nature, five racks, plan then mark (§30 step 1)");
        string read = LearningObjectivesView.RackV2StepText("rack.read");
        Check(read != null && Has(read, "16–20 m") && Has(read, "30 m or more") && Has(read, "no universal rule") && Has(read, "this plantation only"),
            "rack reading gives §5.1's conditional examples and keeps the recommendation to this stand", read ?? "null");

        // 6. Objective 1 flow (§30 steps 2-7, D-064 OPEN-2: field marking before approval).
        string open = LearningObjectivesView.RackV2Instruction("Racks planned: 2 / 5", false, 0, 5, false);
        Check(open.StartsWith("Racks planned: 2 / 5. ") && InOrder(open, "Plan Racks", "Stand Map [M]", "Confirm Rack Plan") && !Has(open, "[P]"),
            "steps 2-5: unconfirmed plan says open the map, Plan Racks, Confirm Rack Plan", open);
        for (int marked = 0; marked < 5; marked++)
        {
            foreach (bool pending in new[] { true, false })
            {
                string field = LearningObjectivesView.RackV2Instruction($"Rack entrances marked: {marked} / 5", true, marked, 5, pending);
                Check(field.StartsWith($"Rack entrances marked: {marked} / 5. ") && InOrder(field, "Return to the forest", "first rack tree nearest the path", "entrance", "[P]", "3.5 m", "left-click")
                    && !Has(field, "approve"), $"step 6: confirmed, {marked} of 5 marked (work pending={pending}) says mark entrances, not approve", field);
            }
        }
        string done = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 5 / 5", true, 5, 5, true);
        Check(done.StartsWith("Rack entrances marked: 5 / 5. ") && InOrder(done, "approve", "Work Plan [Tab]", "advance one year") && !Has(done, "[P]"),
            "step 7: five entrances marked hands over to rack.approve", done);
        string approved = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 5 / 5", true, 5, 5, false);
        Check(approved.StartsWith("Rack entrances marked: 5 / 5. ") && Has(approved, "advance one year") && !Has(approved, "approve the"),
            "after approval the objective says advance one year", approved);
        string layout = LearningObjectivesView.RackV2StepText("rack.layout");
        Check(layout != null && InOrder(layout, "Plan five racks", "Confirm Rack Plan", "return to the forest", "mark each rack entrance", "[P]"), "rack.layout lesson: confirm, then mark", layout ?? "null");
        string approve = LearningObjectivesView.RackV2StepText("rack.approve");
        Check(approve != null && InOrder(approve, "entrances are marked", "Work Plan [Tab]", "Approve Work Plan"), "rack.approve lesson follows marking", approve ?? "null");
        Check(LearningObjectivesView.RackV2StepText("rack.result") == null && LearningObjectivesView.RackV2StepText("tree.inspect") == null,
            "rack.result and non-rack lessons keep their own text");
        string help = MenuHelpView.RackV2Explanation;
        Check(InOrder(help, "Stand Map [M]", "Plan Racks", "Confirm Rack Plan", "spray marker [P]", "approve", "advance one year"),
            "V2 Rack help follows the §30 order: plan, confirm, mark, approve, advance", help);
        Check(string.Join(",", LearningObjectivesView.Topics[0].Steps.Select(step => step.Id)) == "rack.read,rack.create,rack.layout,rack.approve,rack.result",
            "Objective 1 lesson ids and order unchanged (rack.approve then rack.result, OPEN-2)");
        Check(LearningObjectivesView.StepIds.Count() == LearningObjectivesView.StepIds.Distinct().Count() && LearningObjectivesView.StepIds.Count() == 48,
            "no lesson step added, removed or duplicated (saved progress keys stable)", LearningObjectivesView.StepIds.Count().ToString());

        // 7. Forbidden claims across every RACK-V2-10 surface.
        var surfaces = new Dictionary<string, string>
        {
            ["TreeMarks"] = marks,
            ["RackV2Explanation"] = help,
            ["RackV2Concept"] = concept,
            ["RackV2Practice"] = LearningObjectivesView.RackV2Practice,
            ["instruction.open"] = open,
            ["instruction.field"] = LearningObjectivesView.RackV2Instruction("Rack entrances marked: 0 / 5", true, 0, 5, true),
            ["instruction.done"] = done,
            ["instruction.approved"] = approved,
        };
        foreach (string id in RackIds)
            if (LearningObjectivesView.RackV2StepText(id) != null) surfaces["step." + id] = LearningObjectivesView.RackV2StepText(id);
        var forbidden = new[]
        {
            @"\b18\s?m\b", @"\bideal\b", @"\boptimal\b", @"\balways\b", @"best practice", @"standard practice", @"universally",
            @"five racks are (always )?required", @"require[sd]? five racks", @"CCF (stands|forests) (require|need)", @"\ball CCF\b", @"\bevery CCF\b",
            @"only the (first|entrance) tree is marked", @"authored rack plan", @"seven rows", @"\[B\]", @"Press B", @"Use B", @"racks \(B\)",
        };
        foreach (KeyValuePair<string, string> surface in surfaces)
        {
            string[] hits = forbidden.Where(pattern => Regex.IsMatch(surface.Value, pattern, RegexOptions.IgnoreCase)).ToArray();
            Check(hits.Length == 0, "no forbidden claim in " + surface.Key, string.Join(" ; ", hits));
            // "universal" may appear only as a denial ("no universal rule").
            bool universalOk = Regex.Matches(surface.Value, @"universal", RegexOptions.IgnoreCase).Cast<Match>()
                .All(m => m.Index >= 3 && surface.Value.Substring(m.Index - 3, 3).Equals("no ", StringComparison.OrdinalIgnoreCase));
            Check(universalOk, "\"universal\" appears only as \"no universal rule\" in " + surface.Key);
        }
        Check(Has(concept, "Scenario One uses that range as a guide for this plantation") && Has(read ?? "", "for this plantation only"),
            "the 16–20 m band is framed as this stand's guide, never a general rule");

        Console.WriteLine((failures == 0 ? "RACK_V2_HELP_TEACHING_OFFLINE_PASS" : "RACK_V2_HELP_TEACHING_OFFLINE_FAIL failures=" + failures + "") + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }
}

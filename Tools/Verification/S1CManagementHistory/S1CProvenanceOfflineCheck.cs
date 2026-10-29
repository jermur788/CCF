// Offline check for S1-C T-04 provenance labelling: the PRODUCTION ManagementHistoryText.cs classifiers and an audit of the
// AnnualReviewView source, compiled on the plain .NET SDK (no Unity). Nothing here is used by the game.
//
// Proves: the six labels are distinct and each has a plain-language meaning; the intervention and ecology/completion line
// classifiers give the documented class (a missing snapshot is NOT RECORDED, never a guessed value); no new label wording
// implies causality; and every history line the view builds is either tagged or on a reviewed allowlist of guidance text.
//
// Usage: run_t04_offline_check.py   Prints S1C_T04_PROVENANCE_* tokens.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class S1CProvenanceOfflineCheck
{
    private static int failures, checks;

    public static int Main()
    {
        Labels();
        InterventionClassification();
        EcologyClassification();
        CompletionClassification();
        Wording();
        ViewSourceAudit();
        Console.WriteLine(failures == 0
            ? $"S1C_T04_PROVENANCE_PASS checks={checks}"
            : $"S1C_T04_PROVENANCE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    private static void Labels()
    {
        var kinds = (HistoryProvenance[])Enum.GetValues(typeof(HistoryProvenance));
        Check(kinds.Length == 6, "six provenance classes");
        Check(kinds.Select(HistoryProvenanceText.Label).Distinct().Count() == 6, "labels are distinct");
        Check(kinds.Select(HistoryProvenanceText.Meaning).Distinct().Count() == 6 && kinds.All(k => HistoryProvenanceText.Meaning(k).Length > 10), "meanings are distinct and non-trivial");
        Check(kinds.All(k => HistoryProvenanceText.Legend.Contains(HistoryProvenanceText.Label(k))), "legend names every class");
        Check(HistoryProvenanceText.Label(HistoryProvenance.NotRecorded) == "NOT RECORDED" && HistoryProvenanceText.Label(HistoryProvenance.SessionOnly) == "SESSION-ONLY", "words match the spec");
    }

    private static ScenarioEcologicalSnapshot Snap(int year) => new ScenarioEcologicalSnapshot { year = year };

    private static void InterventionClassification()
    {
        var full = new InterventionSummary { Before = Snap(1), After = Snap(2), Latest = Snap(4) };
        Check(ManagementHistoryText.WorkProvenance == HistoryProvenance.Derived, "intervention work counts are grouped at display time: derived");
        Check(ManagementHistoryText.MoneyProvenance == HistoryProvenance.Derived, "intervention money is a derived aggregation");
        Check(ManagementHistoryText.DuringPlanProvenance(full) == HistoryProvenance.Derived, "stand change across the plan is derived when both snapshots exist");
        Check(ManagementHistoryText.SinceProvenance(full) == HistoryProvenance.Derived, "change since is derived when both snapshots exist");
        var noBefore = new InterventionSummary { Before = null, After = Snap(2), Latest = Snap(4) };
        Check(ManagementHistoryText.DuringPlanProvenance(noBefore) == HistoryProvenance.NotRecorded, "no before-snapshot is NOT RECORDED");
        Check(ManagementHistoryText.DuringPlan(noBefore).Contains("not available"), "...and the text invents no figures");
        var noLatest = new InterventionSummary { Before = Snap(1), After = Snap(2), Latest = null };
        Check(ManagementHistoryText.SinceProvenance(noLatest) == HistoryProvenance.NotRecorded, "no later snapshot is NOT RECORDED");
        var noAfter = new InterventionSummary { Before = Snap(1), After = null, Latest = null };
        Check(ManagementHistoryText.SinceProvenance(noAfter) == HistoryProvenance.NotRecorded && ManagementHistoryText.DuringPlanProvenance(noAfter) == HistoryProvenance.NotRecorded,
            "no after-snapshot makes both comparisons NOT RECORDED");
    }

    private static void EcologyClassification()
    {
        Check(HistoryProvenanceText.EcologyLine("Canopy 0.40 → 0.42 (+0.02); average forest-floor light 0.30 → 0.31.", true) == HistoryProvenance.Derived, "canopy change vs previous snapshot is derived");
        Check(HistoryProvenanceText.EcologyLine("Canopy 0.42; average forest-floor light 0.31.", false) == HistoryProvenance.Recorded, "canopy with no previous snapshot is just the recorded value");
        Check(HistoryProvenanceText.EcologyLine("Areas with young regeneration: 5 (was 3).", true) == HistoryProvenance.Derived, "regeneration with '(was n)' is derived");
        Check(HistoryProvenanceText.EcologyLine("Areas with young regeneration: 5.", false) == HistoryProvenance.Recorded, "regeneration alone is recorded");
        Check(HistoryProvenanceText.EcologyLine("Opened areas: 4 map squares with average forest-floor light 0.55 (whole stand 0.31).", true) == HistoryProvenance.Current, "opened areas read the live forest: current");
        Check(HistoryProvenanceText.EcologyLine("Planted trees: 3 growing, 1 joined the canopy layer, 0 lost.", false) == HistoryProvenance.Current, "planted-tree status reads live juveniles: current");
        Check(HistoryProvenanceText.EcologyLine("Something new the classifier has never seen", true) == null, "an unknown line is not given a guessed label");
    }

    private static void CompletionClassification()
    {
        Check(HistoryProvenanceText.CompletionLine("The Scenario One objectives were met in year 40.") == HistoryProvenance.Recorded, "completion year is recorded");
        Check(HistoryProvenanceText.CompletionLine("  Intervention 1 · year 3: 12 felled") == HistoryProvenance.Derived, "intervention work lines are derived counts");
        Check(HistoryProvenanceText.CompletionLine("You carried out 2 interventions:") == HistoryProvenance.Derived, "intervention count is derived");
        Check(HistoryProvenanceText.CompletionLine("No commissioned work was recorded before the objectives were met.") == HistoryProvenance.Derived, "absence of work is a derived statement");
        Check(HistoryProvenanceText.CompletionLine("Living trees at completion: 5 Oak.") == HistoryProvenance.Recorded, "per-species snapshot counts are recorded values");
        Check(HistoryProvenanceText.CompletionLine("Stand, start → year 40: living trees 1 → 2") == HistoryProvenance.Derived, "start-to-completion comparison is derived");
        Check(HistoryProvenanceText.CompletionLine("Money: cash 1.00 → 2.00") == HistoryProvenance.Derived, "completion money totals are derived");
        Check(HistoryProvenanceText.CompletionLine("3 tree deaths not caused by felling were recorded (self-thinning, windthrow or other biological death).") == HistoryProvenance.Derived, "death tally is derived");
        Check(HistoryProvenanceText.CompletionLine("This describes the forest and your decisions at the year the objectives were met. It is not a score.") == null, "closing explanation carries no label");
    }

    private static void Wording()
    {
        string[] banned = { "because", "caused", "due to", "led to", "thanks to", "as a result", "resulted in", "proves", "improved by", "worked" };
        var strings = new List<string> { HistoryProvenanceText.Legend, HistoryProvenanceText.SessionOnlyBrowsingMissing, HistoryProvenanceText.SessionOnlyBrowsingNote,
            HistoryProvenanceText.StormReconstructionNote };
        strings.AddRange(((HistoryProvenance[])Enum.GetValues(typeof(HistoryProvenance))).Select(HistoryProvenanceText.Meaning));
        foreach (string text in strings)
            Check(!banned.Any(word => text.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0), "no causal wording: " + (text.Length > 40 ? text.Substring(0, 40) : text));
        Check(ManagementHistoryText.NotCausalNote.Contains("not proof"), "existing non-causal note is kept");
    }

    private static string ReadSource(string relative)
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? throw new InvalidOperationException("CCF_REPO_ROOT not set");
        return File.ReadAllText(Path.Combine(root, relative));
    }

    // Every UiKit.Add with a body/muted class is either structural or guidance, never a history fact: this list is reviewed by hand.
    private static readonly string[] Allowed =
    {
        "AnnualReviewReadPrompt",
        "UiKit.Add(entry, \"L4 GUIDANCE", // explicit L4 management guidance; no historical fact
          // acknowledgement footer
        "Advance a year from the Work Plan", // empty-state guidance
        "Fallen trees stop contributing",   // storm model explanation
        "Salvage is optional",              // storm action guidance
        "UiKit.Add(head, WorkLabel",        // detail-entry title: its row carries Tag(head, Recorded)
        "UiKit.Add(c, line, \"body\")",      // fallback for a line the classifier does not know (the fixture asserts none occur)
        "An opening creates an opportunity", // guidance
        "UiKit.Add(box, heading",            // trend strip heading (the card is tagged DERIVED)
        "UiKit.Add(box, \"—\"",               // empty strip
        "Latest: {format",                   // last point of the DERIVED strip
        "UiKit.Add(card, m.TutorialHint",    // guidance, not history
        "An intervention is a work plan",    // definition
        "None yet. Inspect trees",           // empty-state guidance
        "UiKit.Add(card, line, style)",      // completion closing explanation
    };

    private static void ViewSourceAudit()
    {
        string ui = ReadSource("Assets/ForestPrototype/UI/AnnualReviewView.cs");
        var untagged = ui.Split('\n').Select(l => l.Trim())
            .Where(l => l.Contains("UiKit.Add(") && (l.Contains("\"body\"") || l.Contains("\"muted\"") || l.Contains("style)")))
            .Where(l => !Allowed.Any(a => l.Contains(a))).ToList();
        Check(untagged.Count == 0, "view audit: every body/muted history line is tagged or on the reviewed allowlist", string.Join(" || ", untagged.Select(l => l.Length > 90 ? l.Substring(0, 90) : l)));
        foreach (HistoryProvenance kind in Enum.GetValues(typeof(HistoryProvenance)))
            Check(ui.Contains("HistoryProvenance." + kind), "view audit: uses class " + kind);
        Check(ui.Contains("AddTagged(c, HistoryProvenance.SessionOnly") && ui.Contains("AddTagged(c, HistoryProvenance.Current, $\"Browsing pressure"),
            "view audit: browsing splits current pressure from the session-only last-year result");
        Check(Regex.Matches(ui, "HistoryProvenance\\.Reconstructed").Count >= 3 && ui.Contains("StormReconstructionNote"), "view audit: storm victim counts, volume and squares are reconstructed, with the reconstruction note");
        string storm = ui.Substring(ui.IndexOf("private void BuildStorm(", StringComparison.Ordinal));
        storm = storm.Substring(0, storm.IndexOf("private static string ChipStyle", StringComparison.Ordinal));
        Check(storm.Contains("HistoryProvenance.Recorded") && storm.Contains("cropTreesLost"), "view audit: Crop Trees lost stays with the recorded storm line");
        // Drill-down entries: title, facts, reason and missing values are separate, individually labelled rows.
        string item = ui.Substring(ui.IndexOf("private static void BuildDetailItem(", StringComparison.Ordinal));
        item = item.Substring(0, item.IndexOf("private static string WorkLabel(", StringComparison.Ordinal));
        Check(item.Contains("Tag(head, HistoryProvenance.Recorded)") && item.Contains("AddTagged(entry, HistoryProvenance.Recorded, string.Join") && item.Contains("AddTagged(entry, HistoryProvenance.NotRecorded, string.Join")
            && !item.Contains("Tag(head, HistoryProvenance.NotRecorded)") && !item.Contains("UiKit.Add(entry"), "view audit: detail entries use separate RECORDED title/facts rows and one NOT RECORDED row, no mixed chip");
        // Presentation counts are derived; only persisted scalars stay recorded.
        string work = ui.Substring(ui.IndexOf("private void BuildWork(", StringComparison.Ordinal));
        work = work.Substring(0, work.IndexOf("private static void AddHarvestSettlement(", StringComparison.Ordinal));
        Check(Regex.Matches(work, "HistoryProvenance\\.Derived").Count >= 7 && Regex.Matches(work, "HistoryProvenance\\.Recorded").Count == 2,
            "view audit: work-column counts are derived; only the regeneration-removal report scalar and failed event reasons stay recorded (2 recorded lines)", Regex.Matches(work, "HistoryProvenance\\.Recorded").Count.ToString());
        // Storm: the live remaining-deadwood line is gated to the latest review and is separate from the derived salvage line.
        Check(storm.Contains("if (!historical)") && storm.Contains("HistoryProvenance.Current, $\"{UiKit.F(summary.DeadwoodRemainingM3") && storm.Contains("HistoryProvenance.Derived, $\"{UiKit.F(summary.SalvagedVolumeM3")
            && !storm.Contains("m³ remains as deadwood now · "), "view audit: storm salvage (derived) and remaining deadwood now (current, latest review only) are separate lines");
        Check(!ui.Contains("UnityEngine.Input") && !ui.Contains("PlayerPrefs"), "view audit: no input or persistence added");
        Check(!Regex.IsMatch(ui, @"\bSaveData\b|\bCaptureSaveData\b|JsonUtility"), "view audit: nothing is written or serialised");
    }
}

// Offline check for S1-D T-03 (AC-2): the 40% teaching-footprint cap is hard only while the teaching sample is open.
// Compiles the PRODUCTION ScenarioOneTeachingCap.cs with ScenarioOneRackBay.cs and ScenarioOneRackBayText.cs on the plain .NET SDK.
// It proves the decision table and the call-site wiring by source text. It does not prove any Unity behaviour or rendering.
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class TeachingCapOfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string name, string detail = "")
    {
        checks++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }

    // An analysis with only the facts the cap decision reads. Everything the cap must NOT depend on is left unrelated on purpose.
    private static ScenarioOneRackBay.Analysis Bay(float removedShare, bool executedMixedRelease, int releaseYear = 1, bool applies = true)
    {
        var a = new ScenarioOneRackBay.Analysis { Applies = applies, RackIndex = 2, RackCount = 5, RemovedBasalShare = removedShare };
        if (executedMixedRelease)
        {
            a.ReleaseYear = releaseYear; a.MixedExecuted = true; a.SoldExecuted = 2; a.DeadwoodExecuted = 1; a.MinExecuted = 0.31f;
        }
        return a;
    }

    // The behaviour before T-03, kept here as the oracle for "pre-completion behaviour unchanged".
    private static bool OldHardBlock(ScenarioOneRackBay.Analysis bay, bool accessOpen) => bay != null && accessOpen && !bay.CanopyKept;

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? ".";
        DecisionTable();
        PreCompletionUnchanged();
        CompletionIndependentOfTheCap();
        StuckStateRow();
        Messages();
        CallSites(root);
        Console.WriteLine(failures == 0 ? "TEACHING_CAP_OFFLINE_PASS checks=" + checks + " NOT_UNITY_VERIFIED"
            : "TEACHING_CAP_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    // lesson active/closed x cap exceeded x cycle (first or later management cycle)
    private static void DecisionTable()
    {
        foreach (bool later in new[] { false, true })
        {
            string cycle = later ? "cycle2+" : "cycle1";
            // Active sample: the work has not been executed and reviewed. Closed: executed mixed release, results read.
            Check(ScenarioOneTeachingCap.Classify(Bay(0.20f, false), true, false) == ScenarioOneTeachingCap.Phase.WithinCap, "active, within cap -> WithinCap, " + cycle);
            Check(ScenarioOneTeachingCap.Classify(Bay(0.55f, false), true, false) == ScenarioOneTeachingCap.Phase.Hard, "active, over cap -> Hard, " + cycle);
            Check(ScenarioOneTeachingCap.Classify(Bay(0.20f, true), true, true) == ScenarioOneTeachingCap.Phase.WithinCap, "closed, within cap -> WithinCap, " + cycle);
            Check(ScenarioOneTeachingCap.Classify(Bay(0.55f, true), true, true) == ScenarioOneTeachingCap.Phase.Advisory, "closed, over cap -> Advisory (AC-2), " + cycle);
        }
        Check(ScenarioOneTeachingCap.Classify(null, true, true) == ScenarioOneTeachingCap.Phase.NotApplicable, "no rack bay -> NotApplicable");
        Check(ScenarioOneTeachingCap.Classify(Bay(0.9f, true, 1, false), true, true) == ScenarioOneTeachingCap.Phase.NotApplicable, "bay not applying -> NotApplicable");
        Check(ScenarioOneTeachingCap.Classify(Bay(0.9f, false), false, false) == ScenarioOneTeachingCap.Phase.NotApplicable, "racks not open -> NotApplicable");
        Check(ScenarioOneRackBay.MaxRemovedBasalShare == 0.40f, "threshold constant unchanged at 0.40");
        Check(Bay(0.40f, false).CanopyKept && !Bay(0.41f, false).CanopyKept, "CanopyKept boundary unchanged");
    }

    // Over a grid of (share, executed, reviewed, access): while the sample is not historically complete the answer equals the old rule.
    private static void PreCompletionUnchanged()
    {
        int rows = 0, mismatches = 0;
        foreach (float share in new[] { 0f, 0.2f, 0.4f, 0.4001f, 0.41f, 0.6f, 1f })
            foreach (bool executed in new[] { false, true })
                foreach (bool reviewed in new[] { false, true })
                    foreach (bool access in new[] { false, true })
                    {
                        var bay = Bay(share, executed);
                        if (ScenarioOneTeachingCap.SampleHistoricallyComplete(bay, reviewed)) continue;
                        rows++;
                        bool hard = ScenarioOneTeachingCap.Classify(bay, access, reviewed) == ScenarioOneTeachingCap.Phase.Hard;
                        if (hard != OldHardBlock(bay, access)) mismatches++;
                    }
        Check(rows > 0 && mismatches == 0, "pre-completion: Hard exactly matches the old predicate", rows + " rows, " + mismatches + " mismatches");
        Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(0.5f, true), false), "executed but results unread -> not complete");
        Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(0.5f, false), true), "results read but no executed release -> not complete");
        var soldOnly = Bay(0.5f, true); soldOnly.MixedExecuted = false;
        Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(soldOnly, true), "sold-only release (no deadwood) -> not complete");
        var thin = Bay(0.5f, true); thin.MinExecuted = 0.10f;
        Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(thin, true), "release below the lesson's own executed target -> not complete");
        var noYear = Bay(0.5f, true, -1);
        Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(noYear, true), "no release year -> not complete");
    }

    // Completion must not depend on the cap: exceeding it can neither complete nor reopen the activity.
    private static void CompletionIndependentOfTheCap()
    {
        foreach (float share in new[] { 0f, 0.3f, 0.4f, 0.5f, 0.99f })
        {
            Check(ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(share, true), true), "complete at removed share " + share);
            Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(share, false), true), "incomplete at removed share " + share);
        }
        // Later cycles: a newer release year with the results read stays complete; more executed removals do not reopen it.
        Check(ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(0.7f, true, 9), true), "year-9 release, results read -> complete");
    }

    // N-1 / spec section 17: executed excess reached before the sample is complete (e.g. through mortality shrinking the living
    // denominator after a sold-only release). T-03 leaves pre-completion behaviour unchanged, so this state STAYS hard.
    private static void StuckStateRow()
    {
        var soldOnlyDrift = Bay(0.47f, true); soldOnlyDrift.MixedExecuted = false; soldOnlyDrift.DeadwoodExecuted = 0;
        var phase = ScenarioOneTeachingCap.Classify(soldOnlyDrift, true, true);
        Check(phase == ScenarioOneTeachingCap.Phase.Hard,
            "STUCK-STATE ROW (spec 17): executed sold-only release drifted over the cap stays Hard; no player action can reduce executed excess");
    }

    private static void Messages()
    {
        var hard = ScenarioOneTeachingCap.HardMessage(Bay(0.52f, false));
        Check(hard.Contains("40%") && hard.Contains("52%") && hard.Contains("teaching footprint") && hard.Contains("the middle rack")
            && hard.Contains("Cancel/unmark") && hard.Contains("cumulative"), "hard message names rule, footprint, quantity and remedy", hard);
        var note = ScenarioOneTeachingCap.AdvisoryMessage(Bay(0.52f, true));
        Check(note.Contains("advisory") && note.Contains("52%") && note.Contains("40%") && note.Contains("does not block Approve Work Plan or Advance"),
            "advisory names the quantity and says it does not block", note);
        Check(!note.Contains("Cancel") && !note.Contains("must") && !note.Contains("should"), "advisory instructs nothing");
    }

    private static string Read(string root, string rel) => File.ReadAllText(Path.Combine(root, rel));

    private static void CallSites(string root)
    {
        string ui = Read(root, "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs");
        string work = Read(root, "Assets/ForestPrototype/UI/WorkPlanView.cs");
        string seq = Read(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs");
        string approve = Regex.Match(ui, @"public bool ApproveWorkPlan\(\)\s*\{.*?\n    \}", RegexOptions.Singleline).Value;
        string advance = Regex.Match(ui, @"public bool AdvanceFromWorkPlan\(\)\s*\{.*?\n    \}", RegexOptions.Singleline).Value;
        Check(approve.Contains("RackPlanProblem") && !approve.Contains("RackPlanAdvisory"), "ApproveWorkPlan gates on the hard problem only");
        Check(advance.Contains("RackPlanProblem") && !advance.Contains("RackPlanAdvisory"), "AdvanceFromWorkPlan gates on the hard problem only");
        string buttons = work.Substring(work.IndexOf("footer.Clear();", StringComparison.Ordinal));
        buttons = buttons.Substring(0, buttons.IndexOf("Close [Tab]", StringComparison.Ordinal));
        Check(buttons.Contains("rackProblem") && !buttons.Contains("rackAdvisory"), "Work Plan buttons enable on the hard problem only");
        Check(seq.Contains("ScenarioOneTeachingCap.Classify") && !seq.Contains("Teaching rack safeguard exceeded (40%"), "view uses the single cap policy");
        string cap = Read(root, "Assets/ForestPrototype/UI/ScenarioOneTeachingCap.cs");
        Check(!cap.Contains("using UnityEngine") && !Regex.IsMatch(cap, @"\bPlayerPrefs\b|\bSerializable\b"), "policy is UnityEngine-free and persists nothing");
        Check(Regex.Matches(cap, @"\bclass\b").Count == 1 && cap.Contains("public static class ScenarioOneTeachingCap") && !cap.Contains("Instance"),
            "one static class, no manager or singleton");
    }
}

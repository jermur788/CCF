#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using CCF.Forestry.WorkEconomy;
using System.Text.RegularExpressions;

// Disposable B2 gate: does the U4 completion guidance (A1, task/scenario-one-two-cycle-guidance-v1 @ a93a722) actually appear, with the right
// wording and direction, in the three intended states - and did no completion logic move?
//
// Tier 1 smoke. It is NOT the 170-check U4 qualification and runs none of its save/legacy/negative matrices. Helper blocks
// below are copied verbatim from Tools/Verification/U4/U4Qualification.cs (same fixtures, same plan builders), so the
// completion path here is the U4 "V1" path and its world hash can be compared with the recorded U4 value.
//
// Needs a graphical Editor (the Annual Review text is read from the real UI Toolkit panel). Run through
// Tools/Verification/Qualification/run_qualification.py --interactive --window 1280x800 with U4_OUT set.
public static class U4GuidanceSmoke
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "U4GuidanceSmoke.Begin")) return;
        new GameObject("U4 guidance smoke").AddComponent<U4GuidanceGate>();
    }
}

public sealed class U4GuidanceGate : MonoBehaviour
{
    private const string Sitka = "sitka-spruce", Oak = "sessile-oak", Beech = "beech";
    private const string OakItem = "sessile-oak-sapling", BeechItem = "beech-sapling";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestSaveData original;
    private string phase, outDir;
    private readonly List<string> failures = new List<string>();
    private readonly StringBuilder evidence = new StringBuilder();
    private int checks;

    // ---------- reporting ----------
    private void Expect(bool ok, string what)
    {
        checks++;
        string line = $"U4_{(ok ? "PASS" : "FAIL")} {what}";
        Debug.Log(line);
        evidence.AppendLine(line);
        if (!ok) failures.Add(what);
    }

    // A presentation finding: recorded and reported, but it does not turn the gate's hard result red.
    private int findings;
    private void Finding(bool ok, string what)
    {
        string line = $"U4_{(ok ? "FINDING_OK" : "FINDING_OPEN")} {what}";
        Debug.Log(line);
        evidence.AppendLine(line);
        if (!ok) findings++;
    }

    private void Note(string key, string value)
    {
        string line = $"U4_NOTE {key} {value}";
        Debug.Log(line);
        evidence.AppendLine(line);
    }

    private static void Need(bool ok, string what) { if (!ok) throw new InvalidOperationException("SETUP: " + what); }

    // ---------- driver ----------
    private IEnumerator Start()
    {
        yield return null; yield return null;
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        phase = "guidance-smoke";
        outDir = Environment.GetEnvironmentVariable("U4_OUT") ?? Path.Combine(Application.dataPath, "..", "Build", "U4Guidance");
        Directory.CreateDirectory(outDir);
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        try
        {
            Need(manager != null && ecology != null && saves != null && marking != null, "scene systems");
            original = saves.CaptureData();
            Note("head_geometry", StandGeometryModel.Label(ecology.StandGeometryModelVersion) + " growthModel=" + ecology.GrowthModelVersion
                + " rngModel=" + ecology.RngModelVersion + " regenModel=" + ecology.RegenerationModelVersion);
            Note("persistentDataPath", Application.persistentDataPath);
            stack.Push(Smoke());
        }
        catch (Exception e) { failure = e; }
        while (failure == null && stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        bool ok = failure == null && failures.Count == 0;
        if (failure != null) { Debug.LogError("U4_GUIDANCE_ABORT " + failure); evidence.AppendLine("U4_GUIDANCE_ABORT " + failure); }
        evidence.AppendLine((ok ? "U4_GUIDANCE_PASS " : "U4_GUIDANCE_FAIL ") + $"checks={checks} failed={failures.Count}");
        try { File.WriteAllText(Path.Combine(outDir, "evidence-guidance-smoke.txt"), evidence.ToString()); } catch (Exception e) { Debug.LogError(e); }
        Debug.Log(ok ? $"U4_GUIDANCE_PASS checks={checks}" : $"U4_GUIDANCE_FAIL failed=[{string.Join(" | ", failures)}] abort={failure}");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    // The three texts exactly as the game defines them (read from the production class, so the gate follows the source).
    private static string Text(CompletionGuidanceState s) => ScenarioOneCompletionGuidance.GuidanceText(s);
    // Whole-word patterns: "within" must not trip "thin". Any hit would mean the text names a kind of second intervention.
    private static readonly string[] Prescriptive = { @"\bfell", @"\bthin\b", @"\bthinning", @"\bprun", @"\bplant", @"\bharvest", @"\bcut\b", @"\boak", @"\bbeech", @"\bsitka", @"\bspecies", @"\bsell", @"\bdeadwood" };

    private static bool AnyText(IEnumerable<string> texts, string needle) => texts.Any(t => t.IndexOf(needle, StringComparison.Ordinal) >= 0);

    private IEnumerator Frames(int n = 3) { for (int i = 0; i < n; i++) yield return null; }

    // Opens the Annual Review and returns every visible text in the panel.
    private IEnumerator OpenReview(ScenarioOneUiRoot ui, List<string> into)
    {
        ui.CloseHelp();
        ui.ShowReview();
        yield return Frames();
        into.Clear();
        into.AddRange(VisibleTexts(ui.RootElement));
    }

    private string Joined(IEnumerable<string> texts) => string.Join(" // ", texts.Take(80));

    // "Visible" the way a player means it: displayed AND fully inside the visible part of its scroll area and of the panel.
    private static ScrollView ScrollAncestor(VisualElement e) { for (VisualElement p = e.parent; p != null; p = p.parent) if (p is ScrollView sv) return sv; return null; }

    private bool OnScreen(VisualElement e, VisualElement root)
    {
        if (!Displayed(e)) return false;
        Rect b = e.worldBound, panel = root.worldBound;
        if (b.yMin < panel.yMin - 1f || b.yMax > panel.yMax + 1f || b.xMin < panel.xMin - 1f || b.xMax > panel.xMax + 1f) return false;
        ScrollView sv = ScrollAncestor(e);
        if (sv == null) return true;
        Rect vp = sv.contentViewport.worldBound;
        return b.yMin >= vp.yMin - 1f && b.yMax <= vp.yMax + 1f;
    }

    // Finds every displayed text containing the needle, records whether one is fully on screen as the panel opens, scrolls the
    // scroll areas that hold them to the end, and requires one to be fully on screen afterwards (and before, if asked).
    private IEnumerator OnScreenCheck(ScenarioOneUiRoot ui, string needle, string label, string capture, bool requireBeforeScroll = false)
    {
        var found = ui.RootElement.Query<TextElement>().ToList().Where(t => Displayed(t) && !string.IsNullOrEmpty(t.text) && t.text.Contains(needle)).ToList();
        Expect(found.Count > 0, label + ": the text exists in the displayed panel (" + found.Count + " element(s))");
        bool before = found.Any(t => OnScreen(t, ui.RootElement));
        Note(label + "_onScreenWithoutScrolling", before.ToString());
        if (requireBeforeScroll) Expect(before, label + ": fully on screen without scrolling");
        foreach (ScrollView sv in found.Select(ScrollAncestor).Where(sc => sc != null).Distinct())
            sv.verticalScroller.value = sv.verticalScroller.highValue;
        yield return Frames();
        bool after = found.Any(t => OnScreen(t, ui.RootElement));
        Expect(after, label + ": fully on screen after scrolling the panel to its end");
        if (capture != null) { yield return new WaitForEndOfFrame(); ForceCapture(capture); }
    }

    // ============================================================ SMOKE ============================================================
    private IEnumerator Smoke()
    {
        ScenarioOneUiRoot ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Need(ui != null && ui.RootElement != null, "UI root present (needs a graphical Editor)");
        yield return Frames(2);
        Expect(ForestSaveData.CurrentVersion == 20, "save version is still 20");
        var texts = new List<string>();
        string tAck = Text(CompletionGuidanceState.AcknowledgeReview), tOne = Text(CompletionGuidanceState.ObjectivesMetOneCycle), tAnother = Text(CompletionGuidanceState.AnotherIntervention);
        Note("TEXT_state2_AcknowledgeReview", tAck);
        Note("TEXT_state1_ObjectivesMetOneCycle", tOne);
        Note("TEXT_state3_AnotherIntervention", tAnother);

        // ---- static content of the three texts ----
        Expect(tAck.Length > 0 && tOne.Length > 0 && tAnother.Length > 0 && tAck != tOne && tOne != tAnother && tAck != tAnother, "the three texts exist and differ");
        foreach (var (name, t) in new[] { ("state2", tAck), ("state1", tOne), ("state3", tAnother) })
            foreach (string word in Prescriptive)
                Expect(!Regex.IsMatch(t, word, RegexOptions.IgnoreCase), $"{name} text does not prescribe or name the second intervention (no match for {word})");
        Expect(Text(CompletionGuidanceState.None) == "", "no text for state None");

        string fresh = Snapshot();

        // ---- control: nothing before any plan ----
        Expect(manager.CompletionGuidance == CompletionGuidanceState.None, "Y0: no guidance before any Work Plan");
        Note("Y0_NextStepHint", manager.NextStepHint);
        Expect(manager.NextStepHint != tAck && manager.NextStepHint != tOne && manager.NextStepHint != tAnother, "Y0: the existing hint is untouched");

        // ============ PATH P (the U4 'V1' sequence): Plan A -> Y1 -> acknowledge -> idle to Y25 ============
        Note("PLAN_A", BuildPlanA());
        Need(manager.AdvanceYear() && Year == 1, "Plan A resolves into year 1: " + manager.Feedback);
        yield return Frames(2);
        Expect(manager.Interventions.Count == 1 && manager.AnnualReports.Count == 1 && !manager.AnnualReviewSeen, "Y1: one intervention, first review pending");
        // The first review keeps its own wording (guidance must not replace the tutorial step the player is on).
        Expect(ui.NeedsAnnualReview, "Y1: the first-review gate is active");
        Expect(manager.TutorialHint.StartsWith("Read WORK DONE", StringComparison.Ordinal), "Y1: tutorial hint is still the first-review wording: " + manager.TutorialHint);
        yield return OpenReview(ui, texts);
        Note("Y1_firstReview_visibleTexts", Joined(texts));
        Expect(!AnyText(texts, tAck) && !AnyText(texts, tOne) && !AnyText(texts, tAnother), "Y1: the first review shows none of the completion guidance");
        Button firstAck = FindAcknowledge(ui);
        Expect(firstAck != null && Displayed(firstAck), "Y1: the first-review acknowledgement control is shown");
        Note("Y1_firstReview_footerTexts", firstAck != null && firstAck.parent != null ? string.Join(" // ", firstAck.parent.Query<TextElement>().ToList().Select(t => t.text)) : "(no control)");

        manager.MarkAnnualReviewSeen();   // the U4 sequence, verbatim
        Expect(Acks().Count() == 1 && Acks().First().year == 1, "Y1: acknowledgement recorded in year 1");
        string s1a = Snapshot();

        // ---- STATE 3: review complete, waiting for a second plan ----
        Expect(manager.CompletionGuidance == CompletionGuidanceState.AnotherIntervention, "STATE 3 (Y1, reviewed, objectives not all met): AnotherIntervention");
        Expect(manager.NextStepHint == tAnother, "STATE 3: NextStepHint is exactly the state-3 text");
        Expect(manager.TutorialHint.Contains(tAnother), "STATE 3: TutorialHint (what the Work Plan header and objectives card show) contains it");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, "STATE 3: nothing left to acknowledge");
        yield return OpenReview(ui, texts);
        Note("STATE3_visibleTexts", Joined(texts));
        yield return new WaitForEndOfFrame(); ForceCapture("state3-annual-review-first-screen");
        Expect(AnyText(texts, tAnother), "STATE 3: the text is visible in the Annual Review");
        yield return OnScreenCheck(ui, tAnother, "STATE3_review", "state3-annual-review-scrolled");
        Expect(!AnyText(texts, tAck) && !AnyText(texts, tOne), "STATE 3: no other state's text is shown");
        Button b3 = FindAcknowledge(ui);
        Expect(b3 == null || !Displayed(b3), "STATE 3: the acknowledgement control is not offered");
        ui.CloseAll(); yield return Frames(2);
        ui.ShowWorkPlan(); ui.CloseHelp(); yield return Frames();
        var planTexts = VisibleTexts(ui.RootElement).ToList();
        Note("STATE3_workPlan_visibleTexts", Joined(planTexts));
        Expect(AnyText(planTexts, tAnother), "STATE 3: the text is visible in the Work Plan header");
        yield return OnScreenCheck(ui, tAnother, "STATE3_workPlan", null, requireBeforeScroll: true);
        yield return new WaitForEndOfFrame(); ForceCapture("state3-work-plan");
        ui.CloseAll(); yield return Frames(2);

        // ---- STATE 1: all eight objectives met, one reviewed plan ----
        yield return AdvanceTo(25, "state 1 idle to 25");
        Report("PATH P idle at Y25 (reviewed)");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active && manager.Interventions.Count == 1, "Y25: eight of eight objectives, Active, one intervention: " + ObjectiveLine());
        Expect(manager.CompletionGuidance == CompletionGuidanceState.ObjectivesMetOneCycle, "STATE 1 (Y25, reviewed, 8/8, one plan): ObjectivesMetOneCycle");
        Expect(manager.NextStepHint == tOne, "STATE 1: NextStepHint is exactly the state-1 text");
        yield return OpenReview(ui, texts);
        Note("STATE1_visibleTexts", Joined(texts));
        yield return new WaitForEndOfFrame(); ForceCapture("state1-annual-review-first-screen");
        Expect(AnyText(texts, tOne), "STATE 1: the text is visible in the Annual Review");
        yield return OnScreenCheck(ui, tOne, "STATE1_review", "state1-annual-review-scrolled");
        Expect(!AnyText(texts, tAck) && !AnyText(texts, tAnother), "STATE 1: no other state's text is shown");
        Expect(tOne.IndexOf("eight objectives", StringComparison.Ordinal) >= 0 && tOne.IndexOf("another", StringComparison.OrdinalIgnoreCase) >= 0,
            "STATE 1: says all eight objectives are met and that another cycle/intervention is still required");
        Note("STATE1_text_mentions_annual_review", (tOne.IndexOf("Annual Review", StringComparison.Ordinal) >= 0).ToString());
        Note("STATE1_STATE3_STATE2_mention_different_simulation_year", $"{tOne.Contains("different simulation year")}/{tAnother.Contains("different simulation year")}/{tAck.Contains("different simulation year")}");
        ui.CloseAll(); yield return Frames(2);
        var hud = VisibleTexts(ui.RootElement).ToList();
        Note("STATE1_hud_visibleTexts", Joined(hud));
        Note("STATE1_hud_shows_guidance", AnyText(hud, "second management decision").ToString() + " (the HUD was deliberately not changed by the guidance commit)");
        yield return new WaitForEndOfFrame(); ForceCapture("state1-hud");

        // ---- completion logic unchanged: the U4 'V1' path reproduces the recorded outcome AND world hash ----
        yield return RestoreJson(s1a, "V1");
        string b1 = BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 2, "V1 plan B resolves into year 2");
        Expect(manager.Interventions.Count == 2 && manager.Outcome == ScenarioOneOutcome.Active, "V1 Y2: two interventions, not complete yet (minimum-year objective)");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.None, "V1 Y2: reviewed two-cycle sequence exists, so no guidance");
        Expect(manager.NextStepHint != tAck && manager.NextStepHint != tOne && manager.NextStepHint != tAnother, "V1 Y2: the existing history hint shows again");
        yield return AdvanceTo(25, "V1");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, $"V1: COMPLETE at Y25 (outcome {manager.Outcome} year {manager.OutcomeYear})");
        string v1Hash = Hash();
        Note("V1_world_hash_Y25", v1Hash + " (U4 qualification recorded 572025058B227E92 on eb7fd3d)");
        Expect(v1Hash == "572025058B227E92", "V1: world hash at Y25 equals the recorded U4 value (completion logic and simulation unchanged)");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.None && manager.NextStepHint.StartsWith("Objectives met in year 25", StringComparison.Ordinal), "V1: completion keeps its own hint; no guidance after completion: " + manager.NextStepHint);

        // ============ PATH N2 (idle first year, review before the first plan): STATE 2, then the footer click ============
        yield return RestoreJson(fresh, "N2");
        Need(manager.AdvanceYear() && Year == 1, "N2 idle first year");
        yield return Frames(2);
        yield return OpenReview(ui, texts);
        Button firstAck2 = FindAcknowledge(ui);
        Expect(firstAck2 != null && Displayed(firstAck2), "N2 Y1: first-review control present");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.None, "N2 Y1: no plan yet, no guidance");
        ClickButton(firstAck2);   // the real UI handler, not the manager shortcut
        yield return Frames(2);
        Expect(Acks().Count() == 1 && Acks().First().year == 1, "N2: UI click records the first acknowledgement (year 1, before any plan)");
        ui.CloseAll(); yield return Frames(2);
        Note("PLAN_A_N2", BuildPlanA());
        Need(manager.AdvanceYear() && Year == 2, "N2 Plan A resolves in year 2");
        yield return Frames(2);
        Report("N2 after plan A (Y2)");

        // ---- STATE 2: first plan resolved, no review after it ----
        Expect(!ui.NeedsAnnualReview, "STATE 2: the first-review gate is not the active control");
        Expect(manager.NeedsCompletionReviewAcknowledgement, "STATE 2: footer acknowledgement is offered");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.AcknowledgeReview, "STATE 2 (Y2, plan resolved, review missing): AcknowledgeReview");
        Expect(manager.NextStepHint == tAck, "STATE 2: NextStepHint is exactly the state-2 text");
        yield return OpenReview(ui, texts);
        Note("STATE2_visibleTexts", Joined(texts));
        Expect(texts.Any(t => t == tAck), "STATE 2: the text is visible as the Annual Review footer prompt (exact element text)");
        yield return OnScreenCheck(ui, tAck, "STATE2_review", null, requireBeforeScroll: true);
        Button b2 = FindAcknowledge(ui);
        Expect(b2 != null && Displayed(b2), "STATE 2: the acknowledgement control is shown");
        Expect(b2 != null && tAck.Contains(b2.text), "STATE 2: the text names the button the player must press: '" + (b2 != null ? b2.text : "(missing)") + "'");
        Expect(!AnyText(texts, tOne) && !AnyText(texts, tAnother), "STATE 2: no other state's text is shown");
        yield return new WaitForEndOfFrame(); ForceCapture("state2-annual-review-footer");
        string n2Snapshot = Snapshot();
        ClickButton(b2);
        yield return Frames(3);
        Expect(Acks().Count() == 2 && Acks().Last().year == 2, "STATE 2: the click records the qualifying acknowledgement (year 2)");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, "STATE 2: footer need is gone after the click");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.AnotherIntervention, "after the click: STATE 3 (reviewed, objectives not all met)");
        yield return OpenReview(ui, texts);
        Expect(AnyText(texts, tAnother) && !texts.Any(t => t == tAck), "after the click: the review now shows the state-3 text and no longer the footer prompt");
        Button afterClick = FindAcknowledge(ui);
        Expect(afterClick == null || !Displayed(afterClick), "after the click: the control is hidden");
        ui.CloseAll(); yield return Frames(2);

        // ---- precedence: eight objectives met but the review is still missing -> acknowledge FIRST ----
        yield return RestoreJson(n2Snapshot, "N2 precedence");
        yield return AdvanceTo(25, "N2 idle to 25");
        Report("N2 idle at Y25 (review missing)");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active && manager.Interventions.Count == 1, "N2 Y25: eight of eight, Active, one plan");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.AcknowledgeReview, "PRECEDENCE: eight objectives met but unreviewed -> AcknowledgeReview comes before ObjectivesMetOneCycle");
        yield return OpenReview(ui, texts);
        Expect(texts.Any(t => t == tAck) && !AnyText(texts, tOne), "PRECEDENCE: the footer shows the acknowledge text, not the objectives-met text");
        Button b25 = FindAcknowledge(ui);
        Expect(b25 != null && Displayed(b25), "PRECEDENCE: the control is offered");
        if (b25 != null) ClickButton(b25);
        yield return Frames(3);
        Expect(manager.CompletionGuidance == CompletionGuidanceState.ObjectivesMetOneCycle, "PRECEDENCE: after the click the state is ObjectivesMetOneCycle");
        yield return OpenReview(ui, texts);
        Expect(AnyText(texts, tOne), "PRECEDENCE: and the objectives-met text is now visible");
        ui.CloseAll(); yield return Frames(2);
        // Second intervention after a reviewed first one completes (acknowledged AFTER the first plan, before the second).
        string b2Plan = BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 26, "N2 second plan resolves in year 26");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 26, $"N2: review then second plan completes at Y26 ({manager.Outcome}/{manager.OutcomeYear}); B = {b2Plan}");
        Expect(manager.CompletionGuidance == CompletionGuidanceState.None, "N2: no guidance once completed");
        yield return null;
    }

    // ---------- small helpers ----------
    private static string F(float v, string f = "0.###") => v.ToString(f, Inv);
    private int Year => manager.CurrentEcologicalYear;
    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));
    private string Snapshot() => JsonUtility.ToJson(saves.CaptureData());
    private string Hash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    private IEnumerator RestoreJson(string json, string label)
    {
        Expect(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), label + ": LoadData accepted");
        yield return null; yield return null;
    }

    private void EvaluateNow() => typeof(ScenarioOneManager).GetMethod("EvaluateProgress", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(manager, null);

    // The pre-U4 handler, verbatim: it sets only annualReviewSeen and records nothing.
    private void LegacyAcknowledge()
    {
        Need(manager.AnnualReports.Count > 0, "legacy acknowledgement needs a report");
        typeof(ScenarioOneManager).GetField("annualReviewSeen", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(manager, true);
    }

    private void ForceOutcome(ScenarioOneOutcome outcome, string reason)
        => typeof(ScenarioOneManager).GetMethod("SetOutcome", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(manager, new object[] { outcome, Year, reason });

    private IEnumerator AdvanceTo(int year, string label)
    {
        while (Year < year)
        {
            Need(manager.AdvanceYear(), $"{label}: advance to {year} failed at {Year}: {manager.Feedback}");
            if (Year % 5 == 0) yield return null;
        }
    }

    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);

    private static List<string> SelectCropTrees(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId)
        .OrderBy(id => id, StringComparer.Ordinal).ToList();

    private static List<ForestTree> SelectThinning(List<ForestTree> trees, List<string> cropIds, float fraction, out float removed)
    {
        var crop = new HashSet<string>(cropIds);
        List<ForestTree> crops = trees.Where(t => crop.Contains(t.TreeId)).ToList();
        float total = trees.Sum(BasalArea), target = total * fraction, taken = 0f;
        var result = new List<ForestTree>();
        foreach (ForestTree t in trees.Where(t => !crop.Contains(t.TreeId) && t.CanChop)
                     .OrderBy(t => crops.Count == 0 ? 0f : crops.Min(c => Vector3.Distance(c.transform.position, t.transform.position)))
                     .ThenBy(t => t.TreeId, StringComparer.Ordinal))
        {
            if (taken >= target) break;
            result.Add(t);
            taken += BasalArea(t);
        }
        removed = total > 0f ? taken / total : 0f;
        return result;
    }

    private bool TryPlantPair(int cell, string item, out Vector3 sheltered, out Vector3 exposed)
    {
        sheltered = exposed = Vector3.zero;
        Vector2 c = ecology.Cells[cell].Center;
        Vector3? first = null;
        for (int ix = -2; ix <= 2; ix++)
            for (int iz = -2; iz <= 2; iz++)
            {
                var p = new Vector3(c.x + ix * 0.9f, 0f, c.y + iz * 0.9f);
                if (ecology.GetCellIndex(p) != cell) continue;
                if (first.HasValue && Vector3.Distance(first.Value, p) < 0.8f) continue;
                bool shelterThis = !first.HasValue;
                if (!manager.TryDesignateExactPlanting(item, p, shelterThis ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor, shelterThis)) continue;
                if (!first.HasValue) { first = p; continue; }
                sheltered = first.Value; exposed = p;
                return true;
            }
        return false;
    }

    private bool TryPlantSingle(string item)
    {
        foreach (int cell in Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i))
        {
            Vector2 c = ecology.Cells[cell].Center;
            for (int ix = -2; ix <= 2; ix++)
                for (int iz = -2; iz <= 2; iz++)
                    if (manager.TryDesignateExactPlanting(item, new Vector3(c.x + ix * 0.9f + 0.3f, 0f, c.y + iz * 0.9f + 0.3f), WorkExecutionMethod.LandownerSimulated, true))
                        return true;
        }
        return false;
    }

    // ---------- the two kinds of plan used throughout ----------
    // Work Plan A: one thinning (crop-tree framework, 27 % basal area, deadwood/keep/sell mix), plus four Oak and four Beech
    // pairs. Everything is designated and approved in ONE planning session, so it resolves as ONE annual Work Plan.
    private string BuildPlanA()
    {
        List<ForestTree> living = Living();
        List<string> crop = SelectCropTrees(living);
        foreach (string id in crop) marking.Mark(living.First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        List<ForestTree> fell = SelectThinning(living, crop, 0.27f, out float fraction);
        int deadwoodN = Math.Min(4, fell.Count), keepN = Math.Min(3, Math.Max(0, fell.Count - deadwoodN));
        int added = 0;
        foreach (var (outcome, group) in new[] {
            (FellingMaterialOutcome.RetainAsFallenDeadwood, fell.Take(deadwoodN)),
            (FellingMaterialOutcome.KeepForUse, fell.Skip(deadwoodN).Take(keepN)),
            (FellingMaterialOutcome.SellAndExtract, fell.Skip(deadwoodN + keepN)) })
        {
            manager.PlanningFellingOutcome = outcome;
            foreach (ForestTree t in group) marking.Mark(t, TreeMarkType.Fell, false);
            added += manager.AddMarkedTreesToWorkPlan();
        }
        Need(added == fell.Count && fell.Count > 0, "work plan took every marked felling (" + added + "/" + fell.Count + ")");
        Need(manager.TryPurchaseStock(OakItem, 8) && manager.TryPurchaseStock(BeechItem, 8), "stock purchase: " + manager.Feedback);
        int[] bright = Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i).ToArray();
        int oakPairs = 0, beechPairs = 0;
        for (int k = 0; k < bright.Length && (oakPairs < 4 || beechPairs < 4); k++)
        {
            bool oak = oakPairs < 4;
            if (TryPlantPair(bright[k], oak ? OakItem : BeechItem, out _, out _)) { if (oak) oakPairs++; else beechPairs++; }
        }
        Need(oakPairs == 4 && beechPairs == 4, $"planting pairs designated oak={oakPairs} beech={beechPairs}");
        Need(manager.ApprovePendingWork(), "Plan A approval: " + manager.Feedback);
        return $"felled={fell.Count} basalAreaFraction={F(fraction)} oakPairs={oakPairs} beechPairs={beechPairs}";
    }

    // A one-order second plan: pruning on the first valid living Sitka (sorted by ID, so it is the same tree every run).
    private string BuildPlanBPrune()
    {
        foreach (ForestTree t in Living().Where(t => t.Species != null && t.Species.SpeciesId == Sitka))
            if (manager.TryDesignatePruning(t.TreeId))
            {
                Need(manager.ApprovePendingWork(), "pruning approval: " + manager.Feedback);
                return "PruneTree " + t.TreeId;
            }
        throw new InvalidOperationException("SETUP: no tree accepted a pruning order");
    }

    private string BuildPlanBThin()
    {
        List<ForestTree> standing = Living().Where(t => t.Species.SpeciesId == Sitka).ToList();
        List<ForestTree> second = SelectThinning(standing, marking.GetCropTreeIds(), 0.2f, out float fraction);
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (ForestTree t in second) marking.Mark(t, TreeMarkType.Fell, false);
        Need(manager.AddMarkedTreesToWorkPlan() == second.Count && second.Count > 0 && manager.ApprovePendingWork(), "second thinning: " + manager.Feedback);
        return $"FellTree x{second.Count} (basal area {F(fraction)})";
    }

    private string BuildPlanBPlant()
    {
        Need(manager.TryPurchaseStock(OakItem, 1) && TryPlantSingle(OakItem) && manager.ApprovePendingWork(), "second planting: " + manager.Feedback);
        return "PlantJuvenile sessile-oak x1";
    }

    // ---------- reading state ----------
    private IEnumerable<ScenarioManagementEvent> Acks() => manager.ManagementEvents.Where(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged);

    private string PlanSummary()
    {
        var years = manager.ManagementEvents.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved && e.outcome == ScenarioManagementOutcome.Succeeded)
            .GroupBy(e => e.year).OrderBy(g => g.Key);
        return string.Join("; ", years.Select(g => $"Y{g.Key}[" + string.Join(",", g.GroupBy(e => e.taskType + (string.IsNullOrEmpty(e.speciesId) || e.taskType == ScenarioWorkType.FellTree ? "" : "(" + e.speciesId + ")"))
            .Select(x => x.Key + "x" + x.Count())) + "]"));
    }

    private string AckSummary() => string.Join(",", Acks().Select(e => $"id{e.eventId}@Y{e.year}"));

    private string ObjectiveLine() => string.Join(" ", manager.Objectives.Select(o => $"{o.objectiveId}={F(o.currentValue)}/{F(o.targetValue)}:{(o.achieved ? "ok" : "no")}"));
    private bool AllObjectives() => manager.Objectives.Count == 8 && manager.Objectives.All(o => o.achieved);

    private void Report(string name)
        => Note("SCENARIO", $"{name} | year={Year} outcome={manager.Outcome} outcomeYear={manager.OutcomeYear} plans={PlanSummary()} acks={AckSummary()} "
            + $"eight={AllObjectives()} interventions={manager.Interventions.Count} needsFooterAck={manager.NeedsCompletionReviewAcknowledgement} seenFlag={manager.AnnualReviewSeen}");

    private static bool IsIncreasing(IReadOnlyList<ScenarioManagementEvent> events)
    {
        for (int i = 1; i < events.Count; i++) if (events[i].eventId <= events[i - 1].eventId) return false;
        return true;
    }

    // ============================================================ UI helpers (verbatim from U4Qualification) ============================================================
    private static IEnumerable<string> VisibleTexts(VisualElement root)
    {
        foreach (VisualElement e in root.Query<VisualElement>().ToList())
        {
            if (!(e is TextElement t) || string.IsNullOrWhiteSpace(t.text)) continue;
            bool shown = true;
            for (VisualElement p = e; p != null; p = p.parent)
                if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden) { shown = false; break; }
            if (shown) yield return t.text;
        }
    }

    private static void ClickButton(Button b)
    {
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        if (invoke == null) throw new InvalidOperationException("SETUP: Clickable.Invoke not found");
        invoke.Invoke(b.clickable, new object[] { null });
    }

    private Button FindAcknowledge(ScenarioOneUiRoot ui) => ui.RootElement.Q<Button>("annual-review-acknowledge");

    private bool Displayed(VisualElement e)
    {
        for (VisualElement p = e; p != null; p = p.parent) if (p.resolvedStyle.display == DisplayStyle.None) return false;
        return true;
    }

    private void ForceCapture(string name)
    {
        try
        {
            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(outDir, name + ".png"), tex.EncodeToPNG());
            Destroy(tex);
        }
        catch (Exception e) { Note("screenshot_failed", name + " " + e.Message); }
    }
}
#endif

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

// Disposable U4 qualification gate: Scenario One reviewed two-cycle completion (save v20, event 9).
//
// Everything that can be reached through the public gameplay handlers is reached that way: mark -> Work Plan ->
// purchase -> exact planting -> approve -> AdvanceYear -> MarkAnnualReviewSeen. Where a shape cannot be produced by
// play (for example two resolved plans in ONE year with a review between them), the fixture is built by editing a
// REAL captured save and is labelled SYNTHETIC in the evidence. Legacy (pre-event-9) saves are made by running the
// same play with the OLD acknowledgement handler (sets only annualReviewSeen), so event IDs stay continuous.
//
// Copy into Assets/ForestPrototype, run <UNITY> -executeMethod U4Qualification.Begin with env U4_PHASE and U4_OUT,
// then remove the copy and its .meta (Tools/Verification/Qualification/run_qualification.py does this).
// Saves go only to Application.persistentDataPath, which the runner isolates with XDG_CONFIG_HOME.
public static class U4Qualification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "U4Qualification.Begin")) return;
        new GameObject("U4 qualification").AddComponent<U4Gate>();
    }
}

public sealed class U4Gate : MonoBehaviour
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
        phase = Environment.GetEnvironmentVariable("U4_PHASE") ?? "core";
        outDir = Environment.GetEnvironmentVariable("U4_OUT") ?? Path.Combine(Application.dataPath, "..", "Build", "U4");
        Directory.CreateDirectory(outDir);
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        try
        {
            Need(manager != null && ecology != null && saves != null && marking != null, "scene systems");
            original = saves.CaptureData();
            Note("phase", phase);
            Note("head_geometry", StandGeometryModel.Label(ecology.StandGeometryModelVersion) + " growthModel=" + ecology.GrowthModelVersion
                + " rngModel=" + ecology.RngModelVersion + " regenModel=" + ecology.RegenerationModelVersion);
            Note("persistentDataPath", Application.persistentDataPath);
            stack.Push(Run());
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
        if (failure != null) { Debug.LogError("U4_VERIFY_ABORT " + failure); evidence.AppendLine("U4_VERIFY_ABORT " + failure); }
        evidence.AppendLine((ok ? "U4_VERIFY_PASS " : "U4_VERIFY_FAIL ") + $"phase={phase} checks={checks} failed={failures.Count} openFindings={findings}");
        try { File.WriteAllText(Path.Combine(outDir, "evidence-" + phase + ".txt"), evidence.ToString()); } catch (Exception e) { Debug.LogError(e); }
        Debug.Log(ok ? $"U4_VERIFY_PASS phase={phase} checks={checks} openFindings={findings}" : $"U4_VERIFY_FAIL phase={phase} failed=[{string.Join(" | ", failures)}] abort={failure}");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private IEnumerator Run()
    {
        switch (phase)
        {
            case "core": yield return Core(); break;
            case "saveA1": yield return SaveA1(); break;
            case "saveA2": yield return SaveA2(); break;
            case "saveA3": yield return SaveFinish("A"); break;
            case "saveB1": yield return SaveB1(); break;
            case "saveB2": yield return SaveFinish("B"); break;
            case "legacyMake": yield return LegacyMake(); break;
            case "legacyC": yield return LegacyC(); break;
            case "legacyD": yield return LegacyTerminal("D", ScenarioOneOutcome.Completed); break;
            case "legacyE": yield return LegacyTerminal("E", ScenarioOneOutcome.Failed); break;
            case "ui": yield return UiPhase(); break;
            default: throw new InvalidOperationException("unknown U4_PHASE " + phase);
        }
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

    // ============================================================ CORE ============================================================
    private IEnumerator Core()
    {
        // ---- identity ----
        Expect(ForestSaveData.CurrentVersion == 21, "ForestSaveData.CurrentVersion == 21");
        var enumValues = Enum.GetValues(typeof(ScenarioManagementEventType)).Cast<ScenarioManagementEventType>().ToDictionary(v => v.ToString(), v => (int)v);
        string[] earlier = { "StockPurchased", "OrderCreated", "OrderApproved", "OrderCancelled", "WorkResolved", "YearAdvanced", "ScenarioCompleted", "ScenarioFailed", "CenturyReviewed" };
        for (int i = 0; i < earlier.Length; i++) Expect(enumValues.TryGetValue(earlier[i], out int v) && v == i, $"enum {earlier[i]} == {i}");
        Expect(enumValues.TryGetValue("AnnualReviewAcknowledged", out int ackValue) && ackValue == 9 && enumValues.Count == 10, "enum AnnualReviewAcknowledged == 9 and no other values");
        Note("objectives_at_start", string.Join(",", manager.Objectives.Select(o => o.objectiveId)));
        Expect(manager.Objectives.Count == 8, "eight objectives exist unchanged (count)");
        Expect(Year == 0 && manager.Outcome == ScenarioOneOutcome.Active, "fresh Year 0, Active");
        string fresh = Snapshot();

        // ---- Work Plan A -> year 1 ----
        string planA = BuildPlanA();
        Note("PLAN_A", planA);
        Need(manager.AdvanceYear() && Year == 1, "Plan A resolves into year 1: " + manager.Feedback);
        yield return null;
        Expect(manager.Interventions.Count == 1, "exactly one intervention after Plan A");
        Expect(manager.AnnualReports.Count == 1 && !manager.AnnualReviewSeen, "first review pending");
        Report("after Plan A (Y1, not acknowledged)");
        string s1 = Snapshot();

        // ============ 1A: old shallow path (all eight objectives, ONE Work Plan, ONE felling, Oak, Beech, ONE acknowledgement) ============
        manager.MarkAnnualReviewSeen();
        ScenarioManagementEvent firstAck = Acks().Last();
        Expect(Acks().Count() == 1 && firstAck.year == 1 && firstAck.eventId > 0, $"acknowledgement recorded: id {firstAck.eventId} year {firstAck.year}");
        manager.MarkAnnualReviewSeen();
        Expect(Acks().Count() == 1, "second click at the same boundary is idempotent");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, "no footer acknowledgement needed straight after the acknowledgement");
        string s1a = Snapshot();
        yield return AdvanceTo(16, "idle to 16");
        string s16 = Snapshot();
        yield return AdvanceTo(24, "idle to 24");
        string s24 = Snapshot();
        yield return AdvanceTo(25, "idle to 25");
        Report("1A shallow at Y25");
        Expect(AllObjectives(), "1A: all eight original objectives satisfied at Y25 on the shallow path: " + ObjectiveLine());
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "1A: shallow path is NOT COMPLETE");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, "1A: nothing left to acknowledge (the guidance cannot be 'acknowledge again')");
        string objectiveTextShallow = string.Join(" // ", manager.Objectives.Select(ScenarioOneUiFacts.ObjectiveLine));
        string hint = manager.NextStepHint, tutorial = manager.TutorialHint;
        Note("1A_NextStepHint", hint);
        Note("1A_TutorialHint", tutorial);
        Note("1A_objective_lines", objectiveTextShallow);
        string[] mentions = { "second", "another", "two", "cycle", "reviewed", "acknowledge" };
        bool guidanceExplains = mentions.Any(w => (hint + " " + tutorial).IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
        Finding(guidanceExplains, "1A: player-facing hint/tutorial text explains that another reviewed management cycle is required (searched: " + string.Join("/", mentions) + ")");
        yield return AdvanceTo(30, "idle to 30");
        Expect(manager.Outcome == ScenarioOneOutcome.Active && AllObjectives(), "1A: still Active with eight of eight objectives at Y30");
        Expect(manager.Interventions.Count == 1, "1A: still one intervention");

        // ============ 1B: valid two-cycle paths ============
        // V1: shortest legitimate path: A (Y1) -> ack (Y1) -> prune plan resolves Y2 -> completion at the earliest the minimum-year objective allows.
        yield return RestoreJson(s1a, "V1");
        string b1 = BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 2, "V1 Plan B resolves into year 2");
        Expect(manager.Interventions.Count == 2, "V1: two interventions after Plan B");
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "V1: not complete at Y2 (minimum year objective unmet)");
        yield return AdvanceTo(25, "V1");
        Report("1B V1 prune at Y2");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, $"V1: COMPLETE at Y25 (outcome {manager.Outcome} year {manager.OutcomeYear})");
        Note("V1_work_types", $"A: {planA} | B: {b1}");
        CompletionChecks("V1");
        string v1Hash = Hash();
        Note("V1_world_hash_Y25", v1Hash);

        // V2: second plan is a thinning at Y16 (resolves Y17).
        yield return RestoreJson(s16, "V2");
        string b2 = BuildPlanBThin();
        Need(manager.AdvanceYear() && Year == 17, "V2 thinning resolves into year 17");
        yield return AdvanceTo(25, "V2");
        Report("1B V2 thinning designated Y16 resolved Y17");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, $"V2: COMPLETE at Y25 (outcome {manager.Outcome} year {manager.OutcomeYear}); B = {b2}");
        CompletionChecks("V2");

        // V3: second plan is a planting resolved exactly in the completion year.
        yield return RestoreJson(s24, "V3");
        string b3 = BuildPlanBPlant();
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "V3: Active with plan B approved but unresolved (approved-but-unresolved does not complete)");
        EvaluateNow();
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "V3: explicit EvaluateProgress with approved-but-unresolved plan stays Active");
        Need(manager.AdvanceYear() && Year == 25, "V3 plan resolves into year 25");
        Report("1B V3 planting resolved Y25");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, $"V3: COMPLETE at Y25, same year the second plan resolved; B = {b3}");
        CompletionChecks("V3");

        // V4: completion after Y25 (plan B approved when the world is already past the minimum year).
        yield return RestoreJson(s24, "V4");
        yield return AdvanceTo(25, "V4 idle");
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "V4: Y25 idle, Active");
        BuildPlanBPrune();
        EvaluateNow();
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "V4: approved-but-unresolved second plan at Y25 stays Active after EvaluateProgress");
        Need(manager.AdvanceYear() && Year == 26, "V4 resolves in year 26");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 26, $"V4: COMPLETE at Y26 (outcome {manager.Outcome} year {manager.OutcomeYear})");

        // ============ 1C: negative ordering cases ============
        // N1: two resolved plans in different years but NO acknowledgement anywhere (review never clicked).
        yield return RestoreJson(s1, "N1");
        BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 2, "N1 plan B");
        yield return AdvanceTo(25, "N1");
        Report("1C N1 two years, no review between (no acknowledgement at all)");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active && Acks().Count() == 0, "N1: eight objectives, two plans, no acknowledgement -> Active");
        Expect(manager.NeedsCompletionReviewAcknowledgement, "N1: footer acknowledgement is offered");
        // late acknowledgement after both plans cannot retroactively qualify them
        manager.MarkAnnualReviewSeen();
        EvaluateNow();
        Expect(Acks().Count() == 1 && Acks().Last().year == 25 && manager.Outcome == ScenarioOneOutcome.Active, "N1: a late acknowledgement (after both plans) does not retroactively complete");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, "N1: footer is gone once acknowledged");
        // ... but it does precede a genuinely later plan
        BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 26, "N1 third plan");
        Report("1C N1 recovery: late ack then third plan");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 26, "N1: acknowledgement followed by a later plan completes (recoverable, no dead end)");

        // N2: review only BEFORE the first plan (idle first year, acknowledge, then two plans, no review between them).
        yield return RestoreJson(fresh, "N2");
        Need(manager.AdvanceYear() && Year == 1, "N2 idle first year");
        manager.MarkAnnualReviewSeen();
        Expect(Acks().Count() == 1 && Acks().Last().year == 1, "N2: review acknowledged in year 1, before any plan");
        BuildPlanA();
        Need(manager.AdvanceYear() && Year == 2, "N2 Plan A resolves in year 2");
        BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 3, "N2 Plan B resolves in year 3");
        yield return AdvanceTo(25, "N2");
        Report("1C N2 review only before first plan");
        Expect(manager.Interventions.Count == 2, "N2: two interventions");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active, "N2: review only before the first plan -> Active");
        Expect(manager.NeedsCompletionReviewAcknowledgement, "N2: footer acknowledgement is offered (the only way forward)");
        string n2Active = Snapshot();
        Note("N2_hints_at_Y25", "NextStep=" + manager.NextStepHint + " | Tutorial=" + manager.TutorialHint);

        // N3: legacy boolean alone (annualReviewSeen == true, no event 9) with two plans.
        yield return RestoreJson(s1, "N3");
        LegacyAcknowledge();
        Expect(manager.AnnualReviewSeen && Acks().Count() == 0, "N3: legacy-shaped state (flag true, no acknowledgement event)");
        BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 2, "N3 plan B");
        yield return AdvanceTo(25, "N3");
        Report("1C N3 annualReviewSeen legacy boolean alone, two plans");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active, "N3: legacy boolean alone is no completion proof");
        Expect(manager.NeedsCompletionReviewAcknowledgement, "N3: fresh acknowledgement reachable");

        // N4: pending-only second plan (designated, never approved).
        yield return RestoreJson(s1a, "N4");
        foreach (ForestTree t in Living().Where(t => t.Species.SpeciesId == Sitka)) if (manager.TryDesignatePruning(t.TreeId)) break;
        Expect(manager.WorkOrders.Any(o => o.type == ScenarioWorkType.PruneTree && o.status == ScenarioWorkStatus.Pending), "N4: a pending (unapproved) pruning order exists");
        yield return AdvanceTo(25, "N4");
        Report("1C N4 pending-only second plan");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active && manager.Interventions.Count == 1, "N4: pending-only second plan -> Active, still one intervention");
        Expect(manager.WorkOrders.Any(o => o.type == ScenarioWorkType.PruneTree && o.IsOpen), "N4: the unresolved order is still open");

        // N5: empty plan (years advance with nothing approved) -- the idle path of 1A, restated against an explicit second 'plan' that has no orders.
        yield return RestoreJson(s1a, "N5");
        Expect(!manager.ApprovePendingWork(), "N5: approving an empty Work Plan is refused");
        yield return AdvanceTo(25, "N5");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active && manager.Interventions.Count == 1, "N5: empty/refused plan -> Active");

        // N6 (GAMEPLAY): a plan whose only order cannot succeed. Designate a pruning on a tree, then fell that tree in the same plan.
        yield return RestoreJson(s1a, "N6");
        {
            ForestTree target = Living().First(t => t.Species.SpeciesId == Sitka && manager.TryDesignatePruning(t.TreeId));
            marking.Mark(target, TreeMarkType.Fell, false);
            manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
            int added = manager.AddMarkedTreesToWorkPlan();
            bool approved = manager.ApprovePendingWork();
            Note("N6_setup", $"prune+fell on {target.TreeId}: felling added={added} approved={approved} feedback={manager.Feedback}");
            if (approved) Need(manager.AdvanceYear(), "N6 advance");
            yield return AdvanceTo(25, "N6");
            Report("1C N6 prune+fell on the same tree");
            Note("N6_failed_or_succeeded", string.Join(",", manager.ManagementEvents.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved && e.year == 2).Select(e => e.taskType + ":" + e.outcome)));
        }

        // ============ synthetic ordering fixtures (edited real saves) ============
        // S1: two plans in the SAME year with an acknowledgement between them.
        yield return SyntheticFromS24(s24, "SYN same-year two plans with ack between", data =>
        {
            int next = data.nextManagementEventId;
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = next++, year = 1, eventType = ScenarioManagementEventType.AnnualReviewAcknowledged });
            var order = new ScenarioOneWorkOrder { workOrderId = 900, resolvedYear = 1, status = ScenarioWorkStatus.Completed, type = ScenarioWorkType.PruneTree, cellIndex = 0 };
            data.workOrders.Add(order);
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = next++, year = 1, workOrderId = 900, eventType = ScenarioManagementEventType.WorkResolved, outcome = ScenarioManagementOutcome.Succeeded, taskType = ScenarioWorkType.PruneTree });
            data.nextManagementEventId = next;
        }, expectComplete: false);
        // S2: acknowledgement BEFORE the first plan only is covered by N2; here: second plan has only a FAILED resolution.
        yield return SyntheticFromS24(s24, "SYN failed-only second plan after ack", data =>
        {
            int next = data.nextManagementEventId;
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = next++, year = 1, eventType = ScenarioManagementEventType.AnnualReviewAcknowledged });
            var order = new ScenarioOneWorkOrder { workOrderId = 901, resolvedYear = 3, status = ScenarioWorkStatus.Failed, type = ScenarioWorkType.PruneTree, cellIndex = 0 };
            data.workOrders.Add(order);
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = next++, year = 3, workOrderId = 901, eventType = ScenarioManagementEventType.WorkResolved, outcome = ScenarioManagementOutcome.Failed, taskType = ScenarioWorkType.PruneTree });
            data.nextManagementEventId = next;
        }, expectComplete: false);
        // S3: ambiguous (duplicate) event IDs cannot prove chronology.
        yield return SyntheticFromS24(s24, "SYN duplicate event IDs", data =>
        {
            var ack = new ScenarioManagementEvent { eventId = data.managementEvents.Max(e => e.eventId), year = 1, eventType = ScenarioManagementEventType.AnnualReviewAcknowledged };
            data.managementEvents.Add(ack);
            var order = new ScenarioOneWorkOrder { workOrderId = 902, resolvedYear = 3, status = ScenarioWorkStatus.Completed, type = ScenarioWorkType.PruneTree, cellIndex = 0 };
            data.workOrders.Add(order);
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = data.managementEvents.Max(e => e.eventId), year = 3, workOrderId = 902, eventType = ScenarioManagementEventType.WorkResolved, outcome = ScenarioManagementOutcome.Succeeded, taskType = ScenarioWorkType.PruneTree });
        }, expectComplete: false);
        // S4 (control): the same synthetic construction with a valid ordering DOES complete, so the negatives above are not vacuous.
        yield return SyntheticFromS24(s24, "SYN control: valid ack then later plan", data =>
        {
            int next = data.nextManagementEventId;
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = next++, year = 1, eventType = ScenarioManagementEventType.AnnualReviewAcknowledged });
            var order = new ScenarioOneWorkOrder { workOrderId = 903, resolvedYear = 3, status = ScenarioWorkStatus.Completed, type = ScenarioWorkType.PruneTree, cellIndex = 0 };
            data.workOrders.Add(order);
            data.managementEvents.Add(new ScenarioManagementEvent { eventId = next++, year = 3, workOrderId = 903, eventType = ScenarioManagementEventType.WorkResolved, outcome = ScenarioManagementOutcome.Succeeded, taskType = ScenarioWorkType.PruneTree });
            data.nextManagementEventId = next;
        }, expectComplete: true);

        // ============ failed scenario stays terminal (GAMEPLAY: spend every euro on nursery stock) ============
        yield return RestoreJson(fresh, "FAILED");
        {
            long cash = manager.CashCents;
            // 550 a + 450 b == cash  =>  find integer a,b >= 0
            int a = -1, b = -1;
            for (int x = 0; x < 100 && a < 0; x++)
            {
                long rest = cash - 550L * x;
                if (rest >= 0 && rest % 450 == 0) { a = x; b = (int)(rest / 450); }
            }
            Need(a >= 0, "nursery spend can reach exactly zero cash");
            Need((a == 0 || manager.TryPurchaseStock(OakItem, a)) && (b == 0 || manager.TryPurchaseStock(BeechItem, b)), "spend all cash: " + manager.Feedback);
            Expect(manager.CashCents == 0, "FAILED: cash is exactly zero (oak " + a + ", beech " + b + ")");
            Need(manager.AdvanceYear(), "FAILED: advance into failure");
            Report("FAILED path");
            Expect(manager.Outcome == ScenarioOneOutcome.Failed, "FAILED: no cash and no approved work fails the scenario");
            int events = manager.ManagementEvents.Count;
            Expect(!manager.AdvanceYear(), "FAILED: further years refused");
            manager.MarkAnnualReviewSeen();
            EvaluateNow();
            Expect(manager.Outcome == ScenarioOneOutcome.Failed, "FAILED: acknowledgement and re-evaluation do not revive a failed scenario");
            Expect(!manager.TryDesignatePruning(Living().First().TreeId), "FAILED: no new plan can be made");
            string failedJson = Snapshot();
            yield return RestoreJson(failedJson, "FAILED reload");
            EvaluateNow();
            Expect(manager.Outcome == ScenarioOneOutcome.Failed, "FAILED: remains Failed after in-process reload");
            Note("FAILED_events", events + " events; reason=" + manager.OutcomeReason);
        }

        // ============ 1E: history / presentation, on the V1 completion ============
        yield return RestoreJson(s1a, "1E");
        BuildPlanBPrune();
        Need(manager.AdvanceYear(), "1E plan B");
        int interventionsBeforeLaterAck = manager.Interventions.Count;
        string histBefore = Fingerprint();
        Expect(interventionsBeforeLaterAck == 2, "1E: two interventions from two resolved plans");
        Expect(Acks().Count() == 1, "1E: one acknowledgement event");
        yield return AdvanceTo(25, "1E");
        string histAtCompletion = Fingerprint();
        Expect(manager.Outcome == ScenarioOneOutcome.Completed, "1E: complete at Y25");
        Expect(manager.Interventions.Count == 2, "1E: acknowledgement event is NOT counted as an intervention (still 2 after completion)");
        CompletionSummary summary = manager.CompletionReview;
        Expect(summary != null && summary.Interventions.Count >= 2, "1E: completion summary shows at least two real interventions (" + (summary?.Interventions.Count ?? -1) + ")");
        Expect(summary != null && summary.Interventions.Select(i => i.Ordinal).SequenceEqual(Enumerable.Range(1, summary.Interventions.Count)), "1E: intervention ordinals are 1..n");
        Expect(summary != null && summary.Interventions.Select(i => i.Year).SequenceEqual(summary.Interventions.Select(i => i.Year).OrderBy(y => y)), "1E: interventions in year order");
        if (summary != null)
        {
            var lines = ManagementHistoryText.Completion(summary, new HistoryTextContext());
            Note("1E_completion_text", string.Join(" // ", lines));
            Expect(lines.Count >= 5, "1E: completion wording produced");
        }
        Expect(IsIncreasing(manager.ManagementEvents), "1E: event IDs strictly increase");
        ScenarioManagementEvent ackEvent = Acks().Single();
        int idxAck = manager.ManagementEvents.ToList().IndexOf(ackEvent);
        var resolved = manager.ManagementEvents.Select((e, i) => (e, i)).Where(p => p.e.eventType == ScenarioManagementEventType.WorkResolved).ToList();
        Expect(resolved.Any(p => p.i < idxAck) && resolved.Any(p => p.i > idxAck), "1E: the acknowledgement lies between resolved plans in recorded order");
        string json = Snapshot();
        yield return RestoreJson(json, "1E reload");
        Expect(Fingerprint() == histAtCompletion, "1E: history identical after reload");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, "1E: still Completed (year 25) after reload");
        Expect(IsIncreasing(manager.ManagementEvents) && Acks().Count() == 1, "1E: event order and the single acknowledgement survive reload");
        Expect(Hash() == v1Hash, "1E: reloaded world hash equals the uninterrupted V1 run");
        Note("1E_history_before_B_resolution_vs_end", histBefore + " >>> " + histAtCompletion);
    }

    private string Fingerprint() => string.Join("|", manager.Interventions.Select(i =>
        $"{i.Ordinal}:{i.Year}:{i.Felled}:{i.Pruned}:{i.Planted}:{i.TasksFailed}:{i.CostCents}:{i.RevenueCents}:{i.Before?.year}:{i.After?.year}"))
        + "#" + string.Join(",", manager.ManagementEvents.Select(e => e.eventId + ":" + (int)e.eventType + "@" + e.year));

    private void CompletionChecks(string label)
    {
        Expect(manager.Interventions.Count >= 2, $"{label}: at least two real interventions (" + manager.Interventions.Count + ")");
        Expect(manager.CompletionReview != null && manager.CompletionReview.Interventions.Count >= 2, $"{label}: completion summary lists >= 2 interventions");
        Expect(Acks().Count() >= 1, $"{label}: an acknowledgement event exists");
        Expect(IsIncreasing(manager.ManagementEvents), $"{label}: event IDs strictly increase");
    }

    private IEnumerator SyntheticFromS24(string s24, string label, Action<ScenarioOneSaveData> edit, bool expectComplete)
    {
        yield return RestoreJson(s24, label);
        ForestSaveData full = saves.CaptureData();
        edit(full.scenarioOne);
        yield return RestoreJson(JsonUtility.ToJson(full), label + " edited");
        Need(Year == 24, label + ": at Y24");
        Need(manager.AdvanceYear() && Year == 25, label + ": advance to Y25");
        Report("SYNTHETIC " + label);
        Expect(AllObjectives(), label + ": eight objectives satisfied (isolates ordering)");
        Expect((manager.Outcome == ScenarioOneOutcome.Completed) == expectComplete, $"SYNTHETIC {label}: expected {(expectComplete ? "COMPLETE" : "NOT complete")}, got {manager.Outcome}");
    }

    // ============================================================ SAVE / RELAUNCH SEQUENCES ============================================================
    private string SaveFile => Path.Combine(Application.persistentDataPath, "forest-save.json");

    private void SaveToDisk(string label)
    {
        saves.Save();
        Expect(File.Exists(SaveFile), label + ": save file written to the isolated slot");
        Note(label + "_saved", $"year={Year} plans={PlanSummary()} acks={AckSummary()} hash={Hash()} bytes={new FileInfo(SaveFile).Length}");
    }

    private IEnumerator LoadFromDisk(string label)
    {
        Expect(saves.CanLoad(out string problem), label + ": CanLoad on the relaunched save" + (problem == null ? "" : " (" + problem + ")"));
        saves.Load();
        yield return null; yield return null;
        Note(label + "_loaded", $"year={Year} plans={PlanSummary()} acks={AckSummary()} hash={Hash()} outcome={manager.Outcome}");
    }

    private IEnumerator SaveA1()
    {
        if (File.Exists(SaveFile)) File.Delete(SaveFile);
        BuildPlanA();
        Need(manager.AdvanceYear() && Year == 1, "A1 Plan A");
        Expect(manager.Interventions.Count == 1 && Acks().Count() == 0, "A1: first intervention, not yet acknowledged");
        SaveToDisk("A1");
        yield break;
    }

    private IEnumerator SaveA2()
    {
        yield return LoadFromDisk("A2");
        Expect(Year == 1 && manager.Interventions.Count == 1 && Acks().Count() == 0 && !manager.AnnualReviewSeen, "A2: reloaded intervention, review still pending");
        manager.MarkAnnualReviewSeen();
        Expect(Acks().Count() == 1 && Acks().Last().year == 1, "A2: acknowledgement recorded after the reload");
        SaveToDisk("A2");
    }

    private IEnumerator SaveB1()
    {
        if (File.Exists(SaveFile)) File.Delete(SaveFile);
        BuildPlanA();
        Need(manager.AdvanceYear() && Year == 1, "B1 Plan A");
        manager.MarkAnnualReviewSeen();
        Expect(Acks().Count() == 1, "B1: acknowledged in the first process");
        SaveToDisk("B1");
        yield break;
    }

    // Final process of both sequences: load, second intervention, run to completion, compare with the uninterrupted run.
    private IEnumerator SaveFinish(string which)
    {
        yield return LoadFromDisk(which + "3");
        Expect(Year == 1 && Acks().Count() == 1 && manager.Interventions.Count == 1 && manager.Outcome == ScenarioOneOutcome.Active, which + ": reloaded state: first intervention + one acknowledgement, Active");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, which + ": no footer acknowledgement needed after reload");
        string b = BuildPlanBPrune();
        Need(manager.AdvanceYear() && Year == 2, which + " second intervention");
        yield return AdvanceTo(25, which + " finish");
        Report("SAVE SEQUENCE " + which);
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, $"{which}: COMPLETE at Y25 after relaunch(es); B = {b}");
        CompletionChecks(which);
        string hash = Hash();
        Note(which + "_final_world_hash_Y25", hash);
        string reference = Environment.GetEnvironmentVariable("U4_EXPECT_HASH");
        if (!string.IsNullOrEmpty(reference)) Expect(hash == reference, $"{which}: final world hash equals the uninterrupted run ({reference})");
    }

    // ============================================================ LEGACY FIXTURES ============================================================
    private IEnumerator LegacyMake()
    {
        string fixtures = Path.Combine(outDir, "fixtures");
        Directory.CreateDirectory(fixtures);
        BuildPlanA();
        Need(manager.AdvanceYear() && Year == 1, "legacy Plan A");
        LegacyAcknowledge();                       // exactly what a pre-U4 build did
        Expect(manager.AnnualReviewSeen && Acks().Count() == 0, "legacy fixture: flag true, no event 9");
        Expect(manager.NeedsCompletionReviewAcknowledgement, "legacy ACTIVE Y1: fresh acknowledgement is reachable");
        File.WriteAllText(Path.Combine(fixtures, "legacy-active-y1.json"), Snapshot());
        yield return AdvanceTo(25, "legacy to 25");
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active, "legacy fixture at Y25: eight objectives, still Active under U4 rules");
        File.WriteAllText(Path.Combine(fixtures, "legacy-active-y25.json"), Snapshot());
        ForceOutcome(ScenarioOneOutcome.Completed, "Scenario complete in year 25. Continue to the Century Review.");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed, "legacy COMPLETED fixture made");
        File.WriteAllText(Path.Combine(fixtures, "legacy-completed-y25.json"), Snapshot());
        // FAILED: restore the Y25 state and force failure the way the old evaluator worded it
        yield return RestoreJson(File.ReadAllText(Path.Combine(fixtures, "legacy-active-y25.json")), "legacy failed make");
        ForceOutcome(ScenarioOneOutcome.Failed, "No cash remains to hire a contractor; the management plan cannot continue.");
        File.WriteAllText(Path.Combine(fixtures, "legacy-failed-y25.json"), Snapshot());
        Note("legacy_fixtures", string.Join(",", Directory.GetFiles(fixtures).Select(Path.GetFileName)));
    }

    private void PlaceFixture(string name)
    {
        string src = Path.Combine(outDir, "fixtures", name);
        Need(File.Exists(src), "fixture " + name);
        Directory.CreateDirectory(Path.GetDirectoryName(SaveFile));
        File.Copy(src, SaveFile, true);
    }

    private IEnumerator LegacyC()
    {
        PlaceFixture("legacy-active-y1.json");
        yield return LoadFromDisk("C");
        Expect(Year == 1 && manager.Outcome == ScenarioOneOutcome.Active && manager.AnnualReviewSeen, "C: legacy ACTIVE v20 loads Active with annualReviewSeen == true");
        Expect(Acks().Count() == 0, "C: no acknowledgement event fabricated on load");
        Expect(manager.Interventions.Count == 1, "C: first intervention intact");
        Expect(manager.NeedsCompletionReviewAcknowledgement, "C: fresh acknowledgement remains reachable");
        EvaluateNow();
        Expect(manager.Outcome == ScenarioOneOutcome.Active, "C: re-evaluation does not complete");
        manager.MarkAnnualReviewSeen();
        Expect(Acks().Count() == 1 && Acks().Last().year == 1, "C: fresh acknowledgement is recorded");
        BuildPlanBPrune();
        Need(manager.AdvanceYear(), "C plan B");
        yield return AdvanceTo(25, "C");
        Report("1D C legacy ACTIVE -> fresh ack -> second cycle");
        Expect(manager.Outcome == ScenarioOneOutcome.Completed && manager.OutcomeYear == 25, "C: later second cycle completes (Y25)");
    }

    private IEnumerator LegacyTerminal(string tag, ScenarioOneOutcome expected)
    {
        PlaceFixture(expected == ScenarioOneOutcome.Completed ? "legacy-completed-y25.json" : "legacy-failed-y25.json");
        yield return LoadFromDisk(tag);
        Expect(manager.Outcome == expected, $"{tag}: legacy {expected} v20 loads as {expected}");
        EvaluateNow();
        Expect(manager.Outcome == expected, $"{tag}: stays {expected} after re-evaluation");
        Expect(!manager.NeedsCompletionReviewAcknowledgement, $"{tag}: no acknowledgement footer on a terminal scenario");
        if (expected == ScenarioOneOutcome.Completed)
        {
            Expect(manager.CompletionReview != null, "D: completion summary still produced for the legacy Completed save");
            Note("D_interventions_in_legacy_summary", (manager.CompletionReview?.Interventions.Count ?? -1).ToString());
        }
        Note(tag + "_outcome_year", $"{manager.Outcome}@{manager.OutcomeYear} reason={manager.OutcomeReason}");
        bool advanced = manager.AdvanceYear();
        Note(tag + "_advance_after_load", advanced.ToString());
        EvaluateNow();
        Expect(manager.Outcome == expected, $"{tag}: terminal outcome unchanged after another year/evaluation");
    }

    // ============================================================ UI (graphical Editor) ============================================================
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

    private IEnumerator UiPhase()
    {
        ScenarioOneUiRoot ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Need(ui != null && ui.RootElement != null, "UI root present (needs a graphical Editor)");
        yield return null; yield return null;
        string fixtures = Path.Combine(outDir, "fixtures");
        // --- legacy ACTIVE Y1: acknowledgement control reachable and working ---
        PlaceFixture("legacy-active-y1.json");
        yield return LoadFromDisk("UI-C");
        ui.ShowReview();
        yield return null; yield return null; yield return null;
        Button ack = FindAcknowledge(ui);
        Expect(ack != null, "UI: acknowledgement button exists in the Annual Review");
        Expect(ack != null && Displayed(ack), "UI: legacy ACTIVE save (flag true, no event 9) shows the acknowledgement control");
        Note("UI_legacy_active_review_texts", string.Join(" // ", VisibleTexts(ui.RootElement).Take(60)));
        Expect(!ui.NeedsAnnualReview, "UI: the first-review gate itself is not active for the legacy save (so only the footer route exists)");
        if (ack != null) ClickButton(ack);
        yield return null; yield return null;
        Expect(Acks().Count() == 1 && Acks().Last().year == 1, "UI: clicking the control records the acknowledgement event");
        ack = FindAcknowledge(ui);
        Expect(ack == null || !Displayed(ack), "UI: control disappears once acknowledged");
        // --- shallow Active at Y25 with all objectives met: what does the player read? ---
        PlaceFixture("legacy-active-y25.json");
        yield return LoadFromDisk("UI-shallow25");
        manager.MarkAnnualReviewSeen();
        ui.CloseHelp();
        ui.ShowReview();
        yield return null; yield return null; yield return null;
        Expect(AllObjectives() && manager.Outcome == ScenarioOneOutcome.Active, "UI shallow: eight of eight objectives, Active");
        var texts = VisibleTexts(ui.RootElement).ToList();
        string joined = string.Join(" // ", texts);
        Note("UI_shallow25_review_texts", joined);
        yield return new WaitForEndOfFrame();
        ForceCapture("ui-shallow25-review");
        string[] needles = { "second intervention", "another intervention", "second cycle", "another cycle", "two interventions", "two cycles", "required", "to complete", "completion", "acknowledge" };
        var hits = needles.Where(n => joined.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        Note("UI_shallow25_guidance_needles_found", hits.Count == 0 ? "NONE" : string.Join(",", hits));
        Finding(hits.Count > 0, "UI shallow: the Annual Review (help closed) tells the player why 8 of 8 objectives are not yet a completion (searched: " + string.Join("/", needles) + ")");
        ui.CloseAll();
        yield return null; yield return null;
        var hud = VisibleTexts(ui.RootElement).ToList();
        Note("UI_shallow25_hud_texts", string.Join(" // ", hud));
        Note("UI_shallow25_hud_mentions_objectives", string.Join(" | ", hud.Where(t => t.IndexOf("objective", StringComparison.OrdinalIgnoreCase) >= 0)));
        yield return new WaitForEndOfFrame();
        ForceCapture("ui-shallow25-hud");
        // --- N2 shape: review only before first plan; footer is the way forward ---
        yield return null;
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

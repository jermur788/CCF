#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Scenario 1 ecology viability diagnostic (Docs/Scenario1EcologyCompletion.md).
// Diagnostic only, not a gate: it reports, it does not assert outcomes.
// Copy into Assets/ForestPrototype, run
//   -executeMethod ScenarioOneEcologyViabilityDiagnostic.Begin
// then remove the copy and its .meta. Never writes the save slot.
//
// Schedules at Scenario One pressure 0.2, 40 years, live manager annual step:
//   A. do nothing
//   B. reasonable CCF plan: ~27% basal-area competitor-release thinning
//      (resolved Year 1), 8 oak + 8 beech planted in the brightest cells
//      (resolved Year 2): half by the landowner in live shelters, half by contractor,
//      ~20% second thinning resolved Year 12
//   C. B without shelters (same executors)
public static class ScenarioOneEcologyViabilityDiagnostic
{
    private const string Requested = "ScenarioOneEcologyViabilityDiagnostic.Requested";

    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("Scenario One viability diagnostic").AddComponent<ScenarioOneEcologyViabilityRunner>();
    }
}

public sealed class ScenarioOneEcologyViabilityRunner : MonoBehaviour
{
    private static readonly int[] Report = { 5, 10, 20, 25, 30, 40 };
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestSaveData original;
    private float pressure;

    private static string F(float v) => v.ToString("0.000", CultureInfo.InvariantCulture);

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator run = Run();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (saves != null && original != null)
        {
            ecology.Browsing.ClearProtection();
            ecology.Browsing.BackgroundPressure = pressure;
            saves.LoadData(original, false);
            yield return null; yield return null;
        }
        Debug.Log(failure == null ? "SCENARIO_ONE_VIABILITY_DIAGNOSTIC_DONE" : "SCENARIO_ONE_VIABILITY_DIAGNOSTIC_ERROR " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        original = saves.CaptureData();
        pressure = ecology.Browsing.BackgroundPressure;
        Debug.Log($"VIABILITY_SETUP pressure={F(pressure)} trees={Living().Count} year={ecology.EcologicalYear}");
        foreach (string schedule in new[] { "A-do-nothing", "B-plan-with-shelters", "C-plan-without-shelters" })
        {
            saves.LoadData(original, false);
            yield return null; yield return null;
            ecology.Browsing.ClearProtection();
            ecology.Browsing.BackgroundPressure = pressure;
            IEnumerator s = Simulate(schedule);
            while (s.MoveNext()) yield return s.Current;
        }
    }

    private IEnumerator Simulate(string schedule)
    {
        bool managed = !schedule.StartsWith("A");
        bool shelters = schedule.StartsWith("B");
        HashSet<string> originals = new HashSet<string>(Living().Select(t => t.TreeId));
        var sheltered = new HashSet<string>();
        var browseEvents = new Dictionary<string, int>();
        var positions = new List<(Vector3 position, bool shelter)>();
        if (managed)
        {
            List<ForestTree> living = Living();
            List<string> crop = Crop(living);
            foreach (string id in crop) marking.Mark(living.First(t => t.TreeId == id), TreeMarkType.CropTree, false);
            List<ForestTree> fell = Thin(living, crop, 0.27f);
            manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
            foreach (ForestTree t in fell.Take(4)) marking.Mark(t, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
            manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
            foreach (ForestTree t in fell.Skip(4)) marking.Mark(t, TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
            manager.ApprovePendingWork();
        }
        for (int year = 1; year <= 40; year++)
        {
            if (managed && ecology.EcologicalYear == 1)
            {
                // Planting designated in Year 1, resolved as Year 2.
                manager.TryPurchaseStock("sessile-oak-sapling", 8);
                manager.TryPurchaseStock("beech-sapling", 8);
                int[] bright = Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i).Take(8).ToArray();
                // Live planting: half by the owner with shelters (B) or without (C), half by contractor.
                for (int k = 0; k < bright.Length; k++)
                    PlantPair(bright[k], k < 4 ? "sessile-oak-sapling" : "beech-sapling", positions, shelters);
                manager.ApprovePendingWork();
            }
            if (managed && ecology.EcologicalYear == 11)
            {
                List<ForestTree> standing = Living().Where(t => t.Species != null && t.Species.SpeciesId == "sitka-spruce").ToList();
                foreach (ForestTree t in Thin(standing, marking.GetCropTreeIds(), 0.2f)) marking.Mark(t, TreeMarkType.Fell, false);
                manager.AddMarkedTreesToWorkPlan();
                manager.ApprovePendingWork();
            }
            if (!manager.AdvanceYear())
            {
                Debug.Log($"VIABILITY {schedule} advance stopped at {ecology.EcologicalYear}: {manager.Feedback}");
                yield break;
            }
            foreach (PlantedJuvenile j in manager.PlantedJuveniles)
                if (j.lastYearBrowsed && j.lastBrowseAssessmentYear == ecology.EcologicalYear)
                    browseEvents[j.juvenileId] = (browseEvents.TryGetValue(j.juvenileId, out int n) ? n : 0) + 1;
            if (Array.IndexOf(Report, ecology.EcologicalYear) >= 0)
                Emit(schedule, originals, positions, browseEvents);
            if (ecology.EcologicalYear % 5 == 0) yield return null;
        }
    }

    private void Emit(string schedule, HashSet<string> originals, List<(Vector3 position, bool shelter)> positions,
        Dictionary<string, int> browseEvents)
    {
        ScenarioEcologicalSnapshot s = manager.EcologicalSnapshots.Last();
        List<ForestTree> living = Living();
        int retained = living.Count(t => originals.Contains(t.TreeId));
        string Group(bool shelter, string species)
        {
            var group = manager.PlantedJuveniles.Where(j => j.speciesId == species
                && positions.Any(p => p.shelter == shelter && Vector2.Distance(new Vector2(p.position.x, p.position.z), new Vector2(j.position.x, j.position.z)) < 0.01f)).ToList();
            if (group.Count == 0) return "-";
            int alive = group.Count(j => j.alive), promoted = group.Count(j => !string.IsNullOrEmpty(j.promotedTreeId));
            int events = group.Sum(j => browseEvents.TryGetValue(j.juvenileId, out int n) ? n : 0);
            float meanH = group.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)).Select(j => j.heightMeters).DefaultIfEmpty(0f).Average();
            return $"{alive}/{group.Count}alive,{promoted}prom,{events}browse,h{F(meanH)}";
        }
        string objectives = ecology.EcologicalYear == 25
            ? " objectives=" + string.Join(",", manager.Objectives.Select(o => $"{o.objectiveId}:{(o.achieved ? "ok" : "no")}")) + " outcome=" + manager.Outcome
            : "";
        Debug.Log($"VIABILITY {schedule} y{ecology.EcologicalYear} cash={manager.CashCents} retainedOriginals={retained} canopy={F(s.meanCanopy)} light={F(s.meanLight)} "
            + $"regenCells={s.occupiedRegenerationCells} deadwoodM3={F(s.deadwoodVolumeM3)} planted={manager.PlantedJuveniles.Count(j => j.alive)} "
            + $"oak[sheltered {Group(true, "sessile-oak")} | exposed {Group(false, "sessile-oak")}] "
            + $"beech[sheltered {Group(true, "beech")} | exposed {Group(false, "beech")}]{objectives}");
    }

    private void PlantPair(int cell, string item, List<(Vector3 position, bool shelter)> positions, bool installShelters)
    {
        Vector2 c = ecology.Cells[cell].Center;
        int placed = 0;
        Vector3? first = null;
        for (int ix = -2; ix <= 2 && placed < 2; ix++)
        for (int iz = -2; iz <= 2 && placed < 2; iz++)
        {
            var p = new Vector3(c.x + ix * 0.9f, 0f, c.y + iz * 0.9f);
            if (ecology.GetCellIndex(p) != cell || (first.HasValue && Vector3.Distance(first.Value, p) < 0.8f)) continue;
            bool first0 = placed == 0;
            if (!manager.TryDesignateExactPlanting(item, p, first0 ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor,
                    first0 && installShelters)) continue;
            positions.Add((p, placed == 0));
            first = first ?? p;
            placed++;
        }
    }

    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);

    private static List<string> Crop(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId)
        .OrderBy(id => id, StringComparer.Ordinal).ToList();

    private static List<ForestTree> Thin(List<ForestTree> trees, List<string> cropIds, float fraction)
    {
        var crop = new HashSet<string>(cropIds);
        List<ForestTree> crops = trees.Where(t => crop.Contains(t.TreeId)).ToList();
        float target = trees.Sum(BasalArea) * fraction, taken = 0f;
        var result = new List<ForestTree>();
        foreach (ForestTree t in trees.Where(t => !crop.Contains(t.TreeId) && t.CanChop)
                     .OrderBy(t => crops.Count == 0 ? 0f : crops.Min(c => Vector3.Distance(c.transform.position, t.transform.position)))
                     .ThenBy(t => t.TreeId, StringComparer.Ordinal))
        {
            if (taken >= target) break;
            result.Add(t);
            taken += BasalArea(t);
        }
        return result;
    }
}
#endif

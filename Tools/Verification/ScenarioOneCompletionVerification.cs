#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Scenario 1 end-to-end completion harness (draft; Docs/Scenario1EcologyCompletion.md).
//
// Copy into Assets/ForestPrototype for an explicit batch run, then remove the
// copy and its .meta:  -executeMethod ScenarioOneCompletionVerification.Begin
// It never writes the save slot (in-memory CaptureData/LoadData only) and
// restores the scene world at the end. Designed for two-process execution:
// compare SCENARIO_ONE_COMPLETION_HASH between runs.
//
// Phases that need live work/economy or v15 APIs are reported as
// TODO-INTEGRATION and never counted as passed. Shelters are installed by a
// FIXTURE that follows the documented timing contract (installedYear = the
// planting order's resolution year) until the live protection adapter exists.
public static class ScenarioOneCompletionVerification
{
    private const string Requested = "ScenarioOneCompletionVerification.Requested";

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
        new GameObject("Scenario One completion gate").AddComponent<ScenarioOneCompletionGate>();
    }
}

public sealed class ScenarioOneCompletionGate : MonoBehaviour
{
    private const string Sitka = "sitka-spruce", Oak = "sessile-oak", Beech = "beech";
    private const string OakItem = "sessile-oak-sapling", BeechItem = "beech-sapling";
    private const float ThinningBasalAreaFraction = 0.27f;

    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestSaveData original;
    private float scenarioPressure;
    private readonly List<string> passed = new List<string>(), todo = new List<string>();
    private readonly StringBuilder evidence = new StringBuilder();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static string F(float v) => v.ToString("0.000", CultureInfo.InvariantCulture);

    private void Pass(string phase, string detail)
    {
        passed.Add(phase);
        string line = $"SCENARIO_ONE_COMPLETION_PHASE {phase} PASS {detail}";
        evidence.AppendLine(line);
        Debug.Log(line);
    }

    private void Todo(string phase, string what)
    {
        todo.Add(phase);
        Debug.Log($"SCENARIO_ONE_COMPLETION_PHASE {phase} TODO-INTEGRATION {what}");
    }

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        foreach (IEnumerator step in new[] { Verify(), Cleanup() })
        {
            while (true)
            {
                bool more; object current = null;
                try { more = step.MoveNext(); if (more) current = step.Current; }
                catch (Exception error) { failure = failure ?? error; break; }
                if (!more) break;
                yield return current;
            }
        }
        if (failure != null)
            Debug.LogError("SCENARIO_ONE_COMPLETION_VERIFY_FAIL " + failure);
        else if (todo.Count > 0)
            Debug.Log($"SCENARIO_ONE_COMPLETION_ENABLED_PHASES_PASS passed={string.Join(",", passed)} todoIntegration={string.Join(",", todo)}");
        else
            Debug.Log("SCENARIO_ONE_COMPLETION_VERIFY_PASS");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Verify()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        Check(manager != null && ecology != null && saves != null && marking != null && spawner != null, "scene systems missing");
        original = saves.CaptureData();
        scenarioPressure = ecology.Browsing.BackgroundPressure;

        // ---- P0 START ----
        List<ForestTree> living = Living();
        Check(manager.Definition != null && ecology.EcologicalYear == 0 && manager.Outcome == ScenarioOneOutcome.Active,
            "Scenario One is not at a fresh Year 0 (run with an isolated config; no save is auto-loaded)");
        Check(living.Count == 336 && living.All(t => t.Species != null && t.Species.SpeciesId == Sitka), "start is not 336 living Sitka");
        Check(manager.CashCents == manager.Definition.StartingCashCents, "starting cash differs from the definition");
        Check(scenarioPressure == manager.Definition.BackgroundBrowsePressure && scenarioPressure == 0.2f, "browse pressure is not the 0.2 Scenario One calibration");
        Check(!ecology.Browsing.HasProtection, "protection present at start");
        Pass("P0", $"trees=336 year=0 cash={manager.CashCents} pressure={F(scenarioPressure)} shelters=0");

        // ---- P1 INSPECT ----
        ForestTree sample = living.First(t => t.TreeId == "P0000");
        Check(sample.Height > 0f && sample.Diameter > 0f && sample.CrownRadius > 0f, "tree inspection data invalid");
        Vector3 groundPoint = CellCenter(0);
        string report = ecology.RegenerationReportLine(groundPoint);
        Check(report.Contains("light") && report.Contains("browsing low"), "ground report lacks light/browsing: " + report);
        RegenerationDiagnosis diagnosis = RegenerationDiagnostics.Diagnose(ecology, groundPoint, manager.PlantedJuveniles, spawner);
        Check(!string.IsNullOrEmpty(diagnosis.Summary()) && !string.IsNullOrEmpty(diagnosis.LightBand), "diagnosis produced no reason");
        Pass("P1", $"tree={sample.TreeId} h={F(sample.Height)} report='{report}' diagnosis='{diagnosis.Summary()}'");

        // ---- P2 PLAN (Year 0): crop trees, competitor-release thinning ----
        float initialCanopy = ecology.Cells.Average(c => c.Canopy);
        float[] initialLight = ecology.Cells.Select(c => c.Light).ToArray();
        List<string> cropIds = SelectCropTrees(living);
        foreach (string id in cropIds) marking.Mark(living.First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        List<ForestTree> fell = SelectThinning(living, cropIds, ThinningBasalAreaFraction, out float fellFraction);
        // The first four fellings stay as fallen deadwood; the rest are extracted.
        manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
        foreach (ForestTree t in fell.Take(4)) marking.Mark(t, TreeMarkType.Fell, false);
        int addedDeadwood = manager.AddMarkedTreesToWorkPlan();
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (ForestTree t in fell.Skip(4)) marking.Mark(t, TreeMarkType.Fell, false);
        int addedExtract = manager.AddMarkedTreesToWorkPlan();
        Check(addedDeadwood == 4 && addedDeadwood + addedExtract == fell.Count, "work plan did not take every marked felling");
        Check(manager.ApprovePendingWork(), "felling work could not be approved");
        HashSet<string> felledIds = new HashSet<string>(fell.Select(t => t.TreeId));
        HashSet<int> thinnedCells = new HashSet<int>(fell.Select(t => ecology.GetCellIndex(t.transform.position)));
        Pass("P2", $"crop={cropIds.Count} fell={fell.Count} basalAreaRemoved={F(fellFraction)} deadwoodOrders=4");
        Todo("P2-economy", "work-order grouping, contractor/owner choice and minimum-job charge belong to the live economy adapter");

        // ---- P3 RESOLVE (Year 1) ----
        long cashBefore = manager.CashCents;
        Check(manager.AdvanceYear() && ecology.EcologicalYear == 1, "Year-1 resolution failed: " + manager.Feedback);
        List<ForestTree> after = Living();
        Check(after.All(t => !felledIds.Contains(t.TreeId)), "a marked tree is still standing");
        Check(after.Count(t => t.Species.SpeciesId == Sitka) == 336 - fell.Count, "felling did not remove exactly the marked trees");
        Check(manager.DeadwoodRecords.Count >= 4, "retained fallen deadwood missing");
        Check(manager.WorkOrders.Count(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed) == fell.Count,
            "felling orders not all completed");
        Pass("P3", $"year=1 felled={fell.Count} deadwoodRecords={manager.DeadwoodRecords.Count} cashDelta={manager.CashCents - cashBefore}");
        Todo("P3-economy", "single ledger settlement and timber-yield batches are asserted once OpenCode's adapter is live");

        // ---- P5 PLANT / PROTECT (planted in Year 1, resolved as Year 2) ----
        int pairsPerSpecies = 4;
        Check(manager.TryPurchaseStock(OakItem, pairsPerSpecies * 2) && manager.TryPurchaseStock(BeechItem, pairsPerSpecies * 2),
            "planting stock purchase failed: " + manager.Feedback);
        int[] bright = Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i).Take(8).ToArray();
        var pairs = new List<(Vector3 sheltered, Vector3 exposed, string species)>();
        for (int k = 0; k < bright.Length && pairs.Count < pairsPerSpecies * 2; k++)
        {
            string species = pairs.Count < pairsPerSpecies ? Oak : Beech;
            string item = species == Oak ? OakItem : BeechItem;
            if (TryPlantPair(bright[k], item, out Vector3 a, out Vector3 b))
                pairs.Add((a, b, species));
        }
        Check(pairs.Count(p => p.species == Oak) == pairsPerSpecies && pairs.Count(p => p.species == Beech) == pairsPerSpecies,
            "could not designate planting pairs in the bright opened cells");
        Check(manager.ApprovePendingWork(), "planting work could not be approved");
        // FIXTURE shelters for half the planted trees, following the timing contract.
        int resolutionYear = ecology.EcologicalYear + 1;
        for (int i = 0; i < pairs.Count; i++)
            ecology.Browsing.Shelters.Add(new BrowseShelter { shelterId = "FIX-" + i, position = new Vector2(pairs[i].sheltered.x, pairs[i].sheltered.z),
                installedYear = resolutionYear, effectiveYears = 8 });
        Check(manager.AdvanceYear() && ecology.EcologicalYear == 2, "planting resolution failed: " + manager.Feedback);
        Check(manager.PlantedJuveniles.Count == pairs.Count * 2 && manager.PlantedJuveniles.All(j => j.plantingYear == resolutionYear),
            "planted juveniles missing or plantingYear differs from the resolution year");
        Check(manager.PlantedJuveniles.Where(j => IsSheltered(j, pairs)).All(j => j.lastBrowseAssessment.Protection == BrowseProtectionState.EffectiveShelter),
            "a fixture shelter did not protect its juvenile in the first step");
        Pass("P5", $"pairs={pairs.Count} resolutionYear={resolutionYear} sheltersEffectiveFirstStep=True");
        Todo("P5-live", "live shelter purchase/installation work order (OpenCode) must write installedYear = resolution year");

        // ---- P4 RESPOND (Year 6) ----
        for (int y = 0; y < 4; y++) Check(manager.AdvanceYear(), "annual advance failed: " + manager.Feedback);
        // Compare light gained since Year 0: unthinned cells include the road and
        // work clearing, which are already bright at the start.
        float thinnedGain = Enumerable.Range(0, ecology.CellCount).Where(thinnedCells.Contains)
            .Average(i => ecology.Cells[i].Light - initialLight[i]);
        float otherGain = Enumerable.Range(0, ecology.CellCount).Where(i => !thinnedCells.Contains(i))
            .Select(i => ecology.Cells[i].Light - initialLight[i]).DefaultIfEmpty(0f).Average();
        float thinnedLight = Enumerable.Range(0, ecology.CellCount).Where(thinnedCells.Contains).Average(i => ecology.Cells[i].Light);
        float canopy6 = ecology.Cells.Average(c => c.Canopy);
        Check(ecology.EcologicalYear == 6 && thinnedGain > 0f && thinnedGain > otherGain,
            $"thinned cells did not gain more light: {thinnedGain} vs {otherGain}");
        Check(canopy6 < initialCanopy, "canopy did not respond to thinning");
        Pass("P4", $"year=6 thinnedCells={thinnedCells.Count} lightGain thinned={F(thinnedGain)} unthinned={F(otherGain)} thinnedLight={F(thinnedLight)} canopy {F(initialCanopy)}->{F(canopy6)}");

        // ---- P8a: sheltered vs exposed pairs at Year 6, while still within browse reach ----
        int pairs6 = 0, taller6 = 0;
        float shelteredMean6 = 0f, exposedMean6 = 0f;
        foreach (var (sheltered, exposed, species) in pairs)
        {
            PlantedJuvenile s6 = At(sheltered), e6 = At(exposed);
            if (s6 == null || e6 == null || !s6.alive || !e6.alive || !string.IsNullOrEmpty(s6.promotedTreeId) || !string.IsNullOrEmpty(e6.promotedTreeId))
                continue;
            pairs6++;
            shelteredMean6 += s6.heightMeters; exposedMean6 += e6.heightMeters;
            Check(s6.heightMeters >= e6.heightMeters - 1e-5f, $"sheltered {species} shorter than its exposed pair at Year 6");
            if (s6.heightMeters > e6.heightMeters + 1e-5f) taller6++;
        }
        Check(pairs6 > 0 && taller6 > 0, "no Year-6 pair showed a browsing difference under pressure 0.2");
        string year6Pairs = $"year6Pairs={pairs6} shelteredTaller={taller6} meanHeight sheltered={F(shelteredMean6 / pairs6)} exposed={F(exposedMean6 / pairs6)}";

        // ---- P6 SAVE (in memory) ----
        ForestSaveData mid = saves.CaptureData();
        string midProtection = ProtectionJson();
        Check(mid.version == ForestSaveData.CurrentVersion && mid.version == 14, "save schema is not v14");
        Pass("P6", $"year=6 version={mid.version} inMemory=True shelters={ecology.Browsing.Shelters.Count}");

        // ---- P7 CONTINUE vs RESTORE ----
        IEnumerator toTwelve = AdvanceTo(12);
        while (toTwelve.MoveNext()) yield return toTwelve.Current;
        string continued = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(saves.LoadData(mid, false), "in-memory restore failed");
        yield return null; yield return null;
        RestoreProtection(midProtection);
        toTwelve = AdvanceTo(12);
        while (toTwelve.MoveNext()) yield return toTwelve.Current;
        string restored = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(continued == restored, $"continue vs restore differ: {continued} vs {restored}");
        Pass("P7", $"year=12 continued={continued} restored={restored}");

        // ---- P8 REGENERATION: sheltered vs exposed pairs (same cell, same light) ----
        int strictlyTaller = 0, compared = 0;
        var diagnosisCounts = new Dictionary<RegenerationLimit, int>();
        foreach (var (sheltered, exposed, species) in pairs)
        {
            PlantedJuvenile s = At(sheltered), e = At(exposed);
            Check(s != null && e != null, "pair juvenile missing");
            foreach (PlantedJuvenile j in new[] { s, e }.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)))
            {
                RegenerationLimit limit = RegenerationDiagnostics.Diagnose(ecology, j.position, manager.PlantedJuveniles, spawner).Limit;
                diagnosisCounts[limit] = diagnosisCounts.TryGetValue(limit, out int n) ? n + 1 : 1;
            }
            if (!s.alive || !e.alive || !string.IsNullOrEmpty(s.promotedTreeId) || !string.IsNullOrEmpty(e.promotedTreeId))
                continue;
            compared++;
            Check(s.heightMeters >= e.heightMeters - 1e-5f, $"sheltered {species} shorter than its exposed pair ({s.heightMeters} < {e.heightMeters})");
            if (s.heightMeters > e.heightMeters + 1e-5f) strictlyTaller++;
        }
        int oakPromotedSheltered = pairs.Where(p => p.species == Oak).Count(p => !string.IsNullOrEmpty(At(p.sheltered).promotedTreeId));
        int oakPromotedExposed = pairs.Where(p => p.species == Oak).Count(p => !string.IsNullOrEmpty(At(p.exposed).promotedTreeId));
        Check(oakPromotedSheltered >= oakPromotedExposed, "exposed oak out-recruited sheltered oak");
        Pass("P8", $"{year6Pairs} year=12 comparedPairs={compared} shelteredStrictlyTaller={strictlyTaller} oakPromoted sheltered={oakPromotedSheltered} exposed={oakPromotedExposed} "
            + "diagnosis=" + string.Join(",", diagnosisCounts.OrderBy(p => p.Key).Select(p => p.Key + ":" + p.Value)));

        // ---- P9 DECADES: second thinning at Year 12, continue to 40 ----
        List<ForestTree> standing = Living().Where(t => t.Species.SpeciesId == Sitka).ToList();
        List<ForestTree> second = SelectThinning(standing, marking.GetCropTreeIds(), 0.2f, out float secondFraction);
        foreach (ForestTree t in second) marking.Mark(t, TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() == second.Count && manager.ApprovePendingWork(), "second thinning could not be planned");
        int minimumYear = manager.Definition.MinimumCompletionYear;
        IEnumerator toCompletion = AdvanceTo(minimumYear);
        while (toCompletion.MoveNext()) yield return toCompletion.Current;
        string objectives = ObjectiveLine();
        ScenarioOneOutcome outcomeAtMinimum = manager.Outcome;
        IEnumerator toForty = AdvanceTo(40);
        while (toForty.MoveNext()) yield return toForty.Current;
        ScenarioEcologicalSnapshot y40 = manager.EcologicalSnapshots.Last();
        string finalHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Debug.Log($"SCENARIO_ONE_COMPLETION_OBJECTIVES year={minimumYear} outcome={outcomeAtMinimum} {objectives}");
        Debug.Log($"SCENARIO_ONE_COMPLETION_HASH {finalHash}");
        Check(outcomeAtMinimum == ScenarioOneOutcome.Completed && manager.OutcomeYear == minimumYear,
            $"reasonable CCF plan did not complete at Year {minimumYear}: {objectives}");
        Pass("P9", $"secondThinning={second.Count} basalArea={F(secondFraction)} completedYear={manager.OutcomeYear} y40 living={y40.livingTrees} "
            + $"canopy={F(y40.meanCanopy)} regenCells={y40.occupiedRegenerationCells} deadwood={F(y40.deadwoodVolumeM3)} hash={finalHash}");

        // ---- P10 COMPATIBILITY ----
        Todo("P10", "v14 -> v15 restore compatibility (fences/shelters persisted; Clear-then-restore contract) is completed by OpenCode/integrator");

        // ---- P11 NEGATIVE CONTROL: do nothing ----
        Check(saves.LoadData(original, false), "restore fresh world");
        yield return null; yield return null;
        ecology.Browsing.ClearProtection();
        ecology.Browsing.BackgroundPressure = scenarioPressure;
        IEnumerator idle = AdvanceTo(minimumYear);
        while (idle.MoveNext()) yield return idle.Current;
        string idleObjectives = ObjectiveLine();
        Check(manager.Outcome != ScenarioOneOutcome.Completed, "doing nothing completed Scenario One: " + idleObjectives);
        Pass("P11", $"year={ecology.EcologicalYear} outcome={manager.Outcome} {idleObjectives}");
    }

    private IEnumerator Cleanup()
    {
        if (saves == null || original == null) yield break;
        ecology.Browsing.ClearProtection();
        ecology.Browsing.BackgroundPressure = scenarioPressure;
        Check(saves.LoadData(original, false), "could not restore the captured scene world");
        yield return null; yield return null;
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == ScenarioReferenceArchive.WorldHash(original),
            "scene world not restored after the gate");
        Debug.Log("SCENARIO_ONE_COMPLETION_SCENE_RESTORED");
    }

    // ---------- plan helpers ----------

    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);

    // Largest stem per 10 m block: the retained crop-tree framework.
    private static List<string> SelectCropTrees(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId)
        .OrderBy(id => id, StringComparer.Ordinal).ToList();

    // Competitor release: nearest non-crop neighbours of crop trees first, until
    // the basal-area fraction is reached.
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
        var candidates = new List<Vector3>();
        for (int ix = -2; ix <= 2; ix++)
        for (int iz = -2; iz <= 2; iz++)
            candidates.Add(new Vector3(c.x + ix * 0.9f, 0f, c.y + iz * 0.9f));
        Vector3? first = null;
        foreach (Vector3 p in candidates)
        {
            if (ecology.GetCellIndex(p) != cell) continue;
            if (first.HasValue && Vector3.Distance(first.Value, p) < 0.8f) continue;
            if (!manager.TryDesignateExactPlanting(item, p)) continue;
            if (!first.HasValue) { first = p; continue; }
            sheltered = first.Value;
            exposed = p;
            return true;
        }
        return false;
    }

    private bool IsSheltered(PlantedJuvenile j, List<(Vector3 sheltered, Vector3 exposed, string species)> pairs)
        => pairs.Any(p => Vector2.Distance(new Vector2(p.sheltered.x, p.sheltered.z), new Vector2(j.position.x, j.position.z)) < 0.01f);

    private PlantedJuvenile At(Vector3 position) => manager.PlantedJuveniles.FirstOrDefault(j =>
        Vector2.Distance(new Vector2(j.position.x, j.position.z), new Vector2(position.x, position.z)) < 0.01f);

    private IEnumerator AdvanceTo(int year)
    {
        while (ecology.EcologicalYear < year)
        {
            Check(manager.AdvanceYear(), $"annual advance to {year} failed at {ecology.EcologicalYear}: {manager.Feedback}");
            if (ecology.EcologicalYear % 5 == 0) yield return null;
        }
    }

    private Vector3 CellCenter(int index) => new Vector3(ecology.Cells[index].Center.x, 0f, ecology.Cells[index].Center.y);

    private string ObjectiveLine() => string.Join(" ", manager.Objectives.Select(o =>
        $"{o.objectiveId}={o.currentValue.ToString("0.###", CultureInfo.InvariantCulture)}/{o.targetValue.ToString("0.###", CultureInfo.InvariantCulture)}:{(o.achieved ? "ok" : "no")}"));

    [Serializable]
    private sealed class ProtectionSnapshot
    {
        public List<BrowseShelter> shelters = new List<BrowseShelter>();
        public List<BrowseProtectedArea> areas = new List<BrowseProtectedArea>();
    }

    private string ProtectionJson()
    {
        var snapshot = new ProtectionSnapshot();
        snapshot.shelters.AddRange(ecology.Browsing.Shelters);
        snapshot.areas.AddRange(ecology.Browsing.ProtectedAreas);
        return JsonUtility.ToJson(snapshot);
    }

    // Documented restore contract: clear, then shelters, then protected areas.
    private void RestoreProtection(string json)
    {
        ProtectionSnapshot snapshot = JsonUtility.FromJson<ProtectionSnapshot>(json);
        ecology.Browsing.ClearProtection();
        ecology.Browsing.Shelters.AddRange(snapshot.shelters);
        ecology.Browsing.ProtectedAreas.AddRange(snapshot.areas);
    }
}
#endif

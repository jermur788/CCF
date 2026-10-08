using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public sealed partial class ScenarioOneManager
{
    private List<StormEventRecord> stormEvents = new List<StormEventRecord>();
    private readonly List<ScenarioDeadwoodRecord> pendingStormDeadwood = new List<ScenarioDeadwoodRecord>();
    private bool resolvingStorm;
    private readonly Dictionary<string, ForestTree> windVictims = new Dictionary<string, ForestTree>(StringComparer.Ordinal);
    private readonly Dictionary<int, StormEventRecord> stormByYear = new Dictionary<int, StormEventRecord>();
    private ScenarioWindthrowVisualBudget windVisualBudget;
    private double stormDeadwoodMilliseconds;
    public IReadOnlyList<StormEventRecord> StormEvents => stormEvents.AsReadOnly();

    private static List<StormEventRecord> CloneStormEvents(IEnumerable<StormEventRecord> records)
        => records == null ? new List<StormEventRecord>() : records.Select(record => new StormEventRecord
        { year = record.year, severity = record.severity, directionDegrees = record.directionDegrees, cropTreesLost = record.cropTreesLost }).ToList();

    public StormDamageSummary StormDamageInYear(int year)
    {
        StormEventRecord storm = stormEvents.FirstOrDefault(record => record.year == year);
        if (storm == null) return null;
        var victims = windVictims.Values.Where(tree => tree != null && tree.MortalityYear == year)
            .OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
        var ids = new HashSet<string>(victims.Select(tree => tree.TreeId), StringComparer.Ordinal);
        var records = deadwoodRecords.Where(record => ids.Contains(record.treeId)).ToArray();
        var summary = new StormDamageSummary { Event = CloneStormEvents(new[] { storm })[0], TreesLost = victims.Length,
            AffectedCells = victims.Select(tree => ecology.GetCellIndex(tree.transform.position)).Where(index => index >= 0).Distinct().Count(),
            OriginalVolumeM3 = records.Sum(record => record.originalVolumeM3), DeadwoodRemainingM3 = records.Sum(record => record.remainingVolumeM3),
            SalvagedVolumeM3 = workOrders.Where(order => order.type == ScenarioWorkType.SalvageDeadwood
                && order.status == ScenarioWorkStatus.Completed && ids.Contains(order.targetTreeId)).Sum(order => order.expectedVolumeM3) };
        if (victims.Length > 0)
        {
            foreach (ForestTree tree in victims) summary.Centre += tree.transform.position;
            summary.Centre /= victims.Length;
            summary.WaypointCell = ecology.GetCellIndex(victims.OrderBy(tree => (tree.transform.position - summary.Centre).sqrMagnitude)
                .ThenBy(tree => tree.TreeId, StringComparer.Ordinal).First().transform.position);
        }
        return summary;
    }

    public void BeginStormResolution(StormEventRecord storm, ForestTree[] victims)
    {
        if (resolvingStorm || stormEvents.Any(record => record.year == storm.year))
            throw new InvalidOperationException("A storm cannot resolve twice in one year.");
        if (victims.Any(tree => tree == null || !tree.IsLiving))
            throw new InvalidOperationException("Storm victims must still be living.");
        stormEvents.Add(CloneStormEvents(new[] { storm })[0]);
        stormByYear.Add(storm.year, stormEvents[stormEvents.Count - 1]);
        foreach (ForestTree victim in victims) windVictims[victim.TreeId] = victim;
        pendingStormDeadwood.Clear();
        stormDeadwoodMilliseconds = 0;
        resolvingStorm = true;
    }

    private void ClearStormVisualState()
    {
        windVictims.Clear();
        stormByYear.Clear();
        if (windVisualBudget != null) windVisualBudget.Clear();
    }

    private void RestoreStormVisualState()
    {
        ClearStormVisualState();
        if (stormEvents.Count == 0) return;
        foreach (StormEventRecord storm in stormEvents) stormByYear.Add(storm.year, storm);
        foreach (ForestTree tree in UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (tree.MortalityCause == "windthrow" && tree.IsBiologicallyDead) windVictims[tree.TreeId] = tree;
    }

    private bool TrySpawnWindthrowVisual(ScenarioDeadwoodRecord record, out string name)
    {
        name = "";
        if (!windVictims.TryGetValue(record.treeId, out ForestTree victim)
            || !stormByYear.TryGetValue(record.fallenYear, out StormEventRecord storm)) return false;
        WindthrowVisualCatalog catalog = WindthrowVisualCatalog.Load();
        if (catalog == null || catalog.freshRootPlate == null || catalog.weatheredRootPlate == null) return false;
        var root = new GameObject("Fallen Log " + record.deadwoodId);
        root.transform.SetParent(transform, false);
        var visual = root.AddComponent<ScenarioWindthrowVisual>();
        visual.Initialize(record, victim, StormWindthrow.FallBearing(storm, record.treeId, ecology.RngModelVersion, ecology.SimulationSeed), catalog);
        if (windVisualBudget == null) windVisualBudget = GetComponent<ScenarioWindthrowVisualBudget>() ?? gameObject.AddComponent<ScenarioWindthrowVisualBudget>();
        windVisualBudget.Register(visual);
        name = root.name;
        return true;
    }

    public void EndStormResolution(StormStepPerformance performance)
    {
        resolvingStorm = false;
        performance.DeadwoodMilliseconds = stormDeadwoodMilliseconds;
        var watch = Stopwatch.StartNew();
        foreach (ScenarioDeadwoodRecord record in pendingStormDeadwood)
            record.visualName = SpawnFallenLogVisual(record);
        pendingStormDeadwood.Clear();
        performance.VisualMilliseconds = watch.Elapsed.TotalMilliseconds;
    }
}

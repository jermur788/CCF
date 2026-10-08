using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public sealed class StormTreeEvaluation
{
    public ForestTree Tree;
    public StormTreeContext Context;
    public float Vulnerability, FailureChance;
    public bool Victim, WasCropTree;
    public int CellIndex;
}

public sealed class StormStepPerformance
{
    public int Candidates, Victims, CanopyRebuilds, SeedRebuilds;
    public double EvaluationMilliseconds, MortalityMilliseconds, RebuildMilliseconds,
        DeadwoodMilliseconds, VisualMilliseconds;
}

public sealed partial class ForestEcologyController
{
    [SerializeField] private int stormModelVersion;
    private StormCalibration stormCalibration = StormCalibration.ComparisonDefault();
    private StormEventRecord forcedStorm;
    private float[] stormLocalTop;
    private bool stormContextCurrent;
    private int canopyRebuildCount, seedRebuildCount;
    public int StormModelVersion
    {
        get => stormModelVersion;
        set { stormModelVersion = StormModel.Normalize(value); forcedStorm = null; }
    }
    public StormCalibration StormProfile => stormCalibration;
    public IReadOnlyList<StormTreeEvaluation> LastStormEvaluations { get; private set; } = Array.Empty<StormTreeEvaluation>();
    public StormStepPerformance LastStormPerformance { get; private set; } = new StormStepPerformance();

#if UNITY_EDITOR
    // Explicit harness-only override. These choices are recorded by calibration
    // evidence, not silently persisted as an unversioned world setting.
    public void UseStormCalibrationForVerification(StormCalibration calibration)
    {
        stormCalibration = calibration ?? throw new ArgumentNullException(nameof(calibration));
        InvalidateStormContext();
    }

    public void ForceStormNextYear(float severity, float directionDegrees)
    {
        if (stormModelVersion != StormModel.WindthrowV1 || !StormCalibration.Finite(severity)
            || severity <= 0f || severity > 1f || !StormCalibration.Finite(directionDegrees)
            || directionDegrees < 0f || directionDegrees >= 360f)
            throw new ArgumentException("Forced storm requires model1, bounded severity and a valid bearing.");
        forcedStorm = new StormEventRecord { year = ecologicalYear + 1, severity = severity, directionDegrees = directionDegrees };
    }
#endif

    private void InvalidateStormContext() => stormContextCurrent = false;

    private void BuildStormContext(ForestTree[] trees)
    {
        var buckets = new List<ForestTree>[CellCount];
        foreach (ForestTree tree in trees)
        {
            if (tree == null || !tree.IsLiving) continue;
            int index = GetCellIndex(tree.transform.position);
            if (index < 0) continue;
            if (buckets[index] == null) buckets[index] = new List<ForestTree>();
            buckets[index].Add(tree);
        }
        stormLocalTop = new float[CellCount];
        float standTop = TopHeight(trees.Where(tree => tree != null && tree.IsLiving).ToList());
        // One cached 3x3 neighbourhood per cell. No tree-by-global-tree scan.
        // It is a spatial shelter proxy [C/I], not a measured crown wind field.
        for (int index = 0; index < CellCount; index++)
        {
            var neighbours = new List<ForestTree>();
            int cx = index % cellsPerAxis, cz = index / cellsPerAxis;
            for (int z = Math.Max(0, cz - 1); z <= Math.Min(cellsPerAxis - 1, cz + 1); z++)
                for (int x = Math.Max(0, cx - 1); x <= Math.Min(cellsPerAxis - 1, cx + 1); x++)
                    if (buckets[z * cellsPerAxis + x] != null) neighbours.AddRange(buckets[z * cellsPerAxis + x]);
            stormLocalTop[index] = neighbours.Count > 0 ? TopHeight(neighbours) : standTop;
        }
        stormContextCurrent = true;
    }

    private static float TopHeight(List<ForestTree> trees)
    {
        if (trees.Count == 0) return 1f;
        trees.Sort((a, b) => { int height = b.Height.CompareTo(a.Height); return height != 0 ? height : StringComparer.Ordinal.Compare(a.TreeId, b.TreeId); });
        int count = Math.Max(1, (int)Math.Ceiling(trees.Count * .2));
        double sum = 0;
        for (int i = 0; i < count; i++) sum += trees[i].Height;
        return (float)(sum / count);
    }

    public float RecentOpeningAtTree(Vector3 position)
    {
        int index = GetCellIndex(position);
        if (index < 0) return 0f;
        int cx = index % cellsPerAxis, cz = index / cellsPerAxis;
        float strongest = 0f, half = cellSizeMeters * .5f;
        for (int z = Math.Max(0, cz - 1); z <= Math.Min(cellsPerAxis - 1, cz + 1); z++)
            for (int x = Math.Max(0, cx - 1); x <= Math.Min(cellsPerAxis - 1, cx + 1); x++)
            {
                ForestEcologyCell cell = cells[z * cellsPerAxis + x];
                float dx = Mathf.Max(0f, Mathf.Abs(position.x - cell.Center.x) - half);
                float dz = Mathf.Max(0f, Mathf.Abs(position.z - cell.Center.y) - half);
                float weight = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dz * dz) / cellSizeMeters);
                strongest = Mathf.Max(strongest, cell.RecentOpening * weight);
            }
        return strongest;
    }

    private StormTreeContext StormContext(ForestTree tree, int index)
    {
        TreeSpeciesDefinition definition = tree.Species != null ? tree.Species : ResolveSpecies();
        return new StormTreeContext(tree.Height, tree.Diameter, stormLocalTop[index], cells[index].Light,
            stormCalibration.Candidate == StormVulnerabilityCandidate.CurrentDiagnostic ? cells[index].RecentOpening : RecentOpeningAtTree(tree.transform.position), definition != null ? definition.StandWindSusceptibility : 1f,
            definition != null ? definition.WindOpeningWeight : 1f);
    }

    public float GetStormVulnerability(ForestTree tree)
    {
        if (tree == null || !tree.IsLiving) return 0f;
        int index = GetCellIndex(tree.transform.position);
        if (index < 0) return 0f;
        if (!stormContextCurrent) BuildStormContext(FindTrees());
        return StormWindthrow.Vulnerability(StormContext(tree, index), stormCalibration.Candidate, stormCalibration.SiteFactor);
    }

    // [C] qualitative bands for a relative model index, never probability.
    public static string StormExposureBand(float index) => index < 1.5f ? "Stable" : index < 3f ? "Watch" : "Exposed";
    public string GetStormExposureLabel(ForestTree tree)
        => StormExposureBand(GetStormVulnerability(tree)) + (stormModelVersion == StormModel.None ? " · storms off" : "");

    public string GetStormExposureExplanation(ForestTree tree)
    {
        if (tree == null || !tree.IsLiving) return "This tree no longer contributes to the living canopy.";
        int index = GetCellIndex(tree.transform.position);
        if (index < 0) return "Outside the modelled stand.";
        if (!stormContextCurrent) BuildStormContext(FindTrees());
        StormTreeContext context = StormContext(tree, index);
        var reasons = new List<string>();
        if (context.Height >= 20) reasons.Add("A tall crown increases modelled wind loading.");
        if (context.Height / Mathf.Max(.01f, context.DiameterCm / 100f) >= 80) reasons.Add("The stem is slender for its height.");
        else if (context.Height / Mathf.Max(.01f, context.DiameterCm / 100f) <= 65) reasons.Add("A stouter stem lowers relative vulnerability.");
        if (context.RecentOpening > .5f) reasons.Add("A recent nearby opening adds exposure; that effect fades with time.");
        else if (context.Light > .4f) reasons.Add("This established opening adds less exposure than a recent cut.");
        if (context.Height < context.LocalTopHeight * .8f) reasons.Add("Nearby taller trees provide shelter in this simplified model.");
        if (context.Height < 8) reasons.Add("Small stature lowers loading but does not make a living tree immune.");
        if (reasons.Count == 0) reasons.Add("Height, stem slenderness and nearby openings determine relative vulnerability.");
        return (stormModelVersion == StormModel.None ? "Storm events are off in this saved forest. " : "")
            + string.Join(" ", reasons) + " This is a relative model judgement, not a failure probability.";
    }

    public float ForecastStormPeak(ForestTree[] retained, IDictionary<int, float> lights, IDictionary<int, float> openings)
    {
        BuildStormContext(retained);
        float peak = 0;
        try
        {
            foreach (ForestTree tree in retained)
            {
                int index = GetCellIndex(tree.transform.position); if (index < 0) continue;
                StormTreeContext current = StormContext(tree, index);
                float light = lights.TryGetValue(index, out float proposedLight) ? proposedLight : current.Light;
                float recent = 0;
                int cx = index % cellsPerAxis, cz = index / cellsPerAxis;
                for (int z = Math.Max(0, cz - 1); z <= Math.Min(cellsPerAxis - 1, cz + 1); z++)
                    for (int x = Math.Max(0, cx - 1); x <= Math.Min(cellsPerAxis - 1, cx + 1); x++)
                    {
                        int neighbour = z * cellsPerAxis + x;
                        var cell = cells[neighbour];
                        float dx = Mathf.Max(0, Mathf.Abs(tree.transform.position.x - cell.Center.x) - cellSizeMeters * .5f);
                        float dz = Mathf.Max(0, Mathf.Abs(tree.transform.position.z - cell.Center.y) - cellSizeMeters * .5f);
                        float value = openings.TryGetValue(neighbour, out float proposed) ? proposed : cell.RecentOpening;
                        recent = Mathf.Max(recent, value * Mathf.Clamp01(1 - Mathf.Sqrt(dx * dx + dz * dz) / cellSizeMeters));
                    }
                peak = Mathf.Max(peak, StormWindthrow.Vulnerability(new StormTreeContext(current.Height, current.DiameterCm,
                    current.LocalTopHeight, light, recent, current.WindSusceptibility, current.OpeningWeight), stormCalibration.Candidate, stormCalibration.SiteFactor));
            }
        }
        finally { InvalidateStormContext(); }
        return peak;
    }

    // Pure preview: order is canonicalized and all inputs are captured before
    // any death. The shuffled-array determinism gate uses this same production path.
    public List<StormTreeEvaluation> EvaluateStorm(StormEventRecord storm, ForestTree[] treeOrder = null)
    {
        if (storm == null) throw new ArgumentNullException(nameof(storm));
        if (storm.year < 1 || !StormCalibration.Finite(storm.severity) || storm.severity <= 0 || storm.severity > 1
            || !StormCalibration.Finite(storm.directionDegrees) || storm.directionDegrees < 0 || storm.directionDegrees >= 360)
            throw new ArgumentException("Storm preview requires a valid year, bounded intensity and bearing.");
        ForestTree[] trees = (treeOrder ?? FindTrees()).Where(tree => tree != null && tree.IsLiving)
            .OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
        BuildStormContext(trees);
        var evaluations = new List<StormTreeEvaluation>(trees.Length);
        foreach (ForestTree tree in trees)
        {
            int index = GetCellIndex(tree.transform.position);
            if (index < 0) continue;
            StormTreeContext context = StormContext(tree, index);
            float vulnerability = StormWindthrow.Vulnerability(context, stormCalibration.Candidate, stormCalibration.SiteFactor);
            float chance = StormWindthrow.FailureChance(vulnerability, storm.severity, stormCalibration.ProbabilityTransform);
            evaluations.Add(new StormTreeEvaluation { Tree = tree, Context = context, CellIndex = index,
                WasCropTree = tree.IsCropTree, Vulnerability = vulnerability, FailureChance = chance,
                Victim = SimulationRandom.Roll(rngModelVersion, "WINDTHROW-v1-" + tree.TreeId, storm.year, simulationSeed) < chance });
        }
        return evaluations;
    }

    private void ResolveStormForYear()
    {
        LastStormEvaluations = Array.Empty<StormTreeEvaluation>();
        LastStormPerformance = new StormStepPerformance();
        if (stormModelVersion != StormModel.WindthrowV1) return;
        ScenarioOneManager manager = UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>();
        if (manager == null) throw new InvalidOperationException("Storm model1 requires Scenario One state.");
        StormEventRecord storm = forcedStorm ?? StormWindthrow.SampleEvent(stormCalibration, rngModelVersion, ecologicalYear, simulationSeed);
        forcedStorm = null;
        if (storm == null) return;
        if (storm.year != ecologicalYear) throw new InvalidOperationException("Forced storm belongs to a different annual step.");
        var watch = Stopwatch.StartNew();
        List<StormTreeEvaluation> evaluations = EvaluateStorm(storm);
        LastStormEvaluations = evaluations;
        LastStormPerformance.Candidates = evaluations.Count;
        LastStormPerformance.EvaluationMilliseconds = watch.Elapsed.TotalMilliseconds;
        var victims = evaluations.Where(value => value.Victim).ToList();
        storm.cropTreesLost = victims.Count(value => value.WasCropTree);
        LastStormPerformance.Victims = victims.Count;
        manager.BeginStormResolution(storm, victims.Select(value => value.Tree).ToArray());
        int oldCanopy = canopyRebuildCount, oldSeed = seedRebuildCount;
        BeginChangeBatch();
        watch.Restart();
        try
        {
            foreach (StormTreeEvaluation victim in victims)
            {
                if (!victim.Tree.ApplyMortality("windthrow", ecologicalYear))
                    throw new InvalidOperationException("Prevalidated storm victim was already dead.");
                cells[victim.CellIndex].RecentOpening = Mathf.Min(maxRecentOpeningPerCell, cells[victim.CellIndex].RecentOpening + 1f);
            }
        }
        finally
        {
            LastStormPerformance.MortalityMilliseconds = watch.Elapsed.TotalMilliseconds;
            watch.Restart();
            EndChangeBatch();
            LastStormPerformance.RebuildMilliseconds = watch.Elapsed.TotalMilliseconds;
            LastStormPerformance.CanopyRebuilds = canopyRebuildCount - oldCanopy;
            LastStormPerformance.SeedRebuilds = seedRebuildCount - oldSeed;
            InvalidateStormContext();
            manager.EndStormResolution(LastStormPerformance);
        }
    }
}

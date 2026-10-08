using System;
using System.Collections.Generic;
using UnityEngine;

// Storm version is independent of RNG, regeneration and growth versions.
public static class StormModel
{
    public const int None = 0;
    public const int WindthrowV1 = 1;
    public const int Latest = WindthrowV1;
    public static int Normalize(int value) => Mathf.Clamp(value, None, Latest);
}

[Serializable]
public sealed class StormEventRecord
{
    public int year;
    // [C] intensity, not measured wind speed or annual mortality probability.
    public float severity;
    // Bearing the wind blows towards: 0=north(+Z), 90=east(+X).
    public float directionDegrees;
    // Marks are erased by death, so this event aggregate cannot be reconstructed.
    // Victim identities, dimensions and death year remain in existing tree records.
    public int cropTreesLost;
}

public enum StormVulnerabilityCandidate { CurrentDiagnostic, HeightAndSlenderness, FullProposal, ReducedProposal }
public enum StormProbabilityTransform { Exponential, BoundedRational }

// Runtime calibration only. No profile or per-tree history is added to the save.
// New games keep stormModel0 while these choices await Manager review.
public sealed class StormCalibration
{
    public readonly float AnnualProbability;
    public readonly IReadOnlyList<float> Severities;
    public readonly IReadOnlyList<float> Weights;
    public readonly StormVulnerabilityCandidate Candidate;
    public readonly StormProbabilityTransform ProbabilityTransform;
    public readonly float SiteFactor;

    public StormCalibration(float annualProbability, float[] severities, float[] weights,
        StormVulnerabilityCandidate candidate = StormVulnerabilityCandidate.ReducedProposal,
        StormProbabilityTransform transform = StormProbabilityTransform.BoundedRational,
        float siteFactor = 1f)
    {
        if (!Finite(annualProbability) || annualProbability < 0f || annualProbability > .05f)
            throw new ArgumentOutOfRangeException(nameof(annualProbability));
        if (severities == null || weights == null || severities.Length != 3 || weights.Length != 3)
            throw new ArgumentException("A calibration profile needs three bounded intensity levels and weights.");
        float total = 0f;
        for (int i = 0; i < 3; i++)
        {
            if (!Finite(severities[i]) || severities[i] <= 0f || severities[i] > 1f || !Finite(weights[i]) || weights[i] < 0f)
                throw new ArgumentException("Invalid storm intensity or weight.");
            total += weights[i];
        }
        if (!Finite(total) || total <= 0f || !Finite(siteFactor) || siteFactor <= 0f
            || !Enum.IsDefined(typeof(StormVulnerabilityCandidate), candidate)
            || !Enum.IsDefined(typeof(StormProbabilityTransform), transform))
            throw new ArgumentException("Invalid storm calibration.");
        AnnualProbability = annualProbability;
        Severities = Array.AsReadOnly((float[])severities.Clone());
        Weights = Array.AsReadOnly((float[])weights.Clone());
        Candidate = candidate;
        ProbabilityTransform = transform;
        SiteFactor = siteFactor;
    }

    // Provisional comparison profile, not an accepted frequency/default decision.
    public static StormCalibration ComparisonDefault() => new StormCalibration(.02f,
        new[] { .02f, .06f, .18f }, new[] { 1f, 1f, 1f });
    public static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
}

public readonly struct StormTreeContext
{
    public readonly float Height, DiameterCm, LocalTopHeight, Light, RecentOpening, WindSusceptibility, OpeningWeight;
    public StormTreeContext(float height, float diameterCm, float localTopHeight, float light, float recentOpening,
        float windSusceptibility = 1f, float openingWeight = 1f)
    {
        Height = height; DiameterCm = diameterCm; LocalTopHeight = localTopHeight;
        Light = light; RecentOpening = recentOpening;
        WindSusceptibility = windSusceptibility; OpeningWeight = openingWeight;
    }
}

public static class StormWindthrow
{
    public static float Vulnerability(StormTreeContext context, StormVulnerabilityCandidate candidate, float siteFactor = 1f)
    {
        if (!Enum.IsDefined(typeof(StormVulnerabilityCandidate), candidate)
            || !StormCalibration.Finite(siteFactor) || siteFactor <= 0f)
            throw new ArgumentException("Vulnerability requires a known candidate and finite positive site factor.");
        double height = Math.Max(.01, context.Height), diameter = Math.Max(.01, context.DiameterCm / 100.0);
        double hd = height / diameter;
        double light = Mathf.Clamp01(context.Light), recent = Math.Max(0, context.RecentOpening);
        double index;
        if (candidate == StormVulnerabilityCandidate.CurrentDiagnostic)
            index = context.WindSusceptibility * (height / Math.Max(.05, context.DiameterCm / 100.0)) * (.25 + .75 * light) * (1 + context.OpeningWeight * recent);
        else if (candidate == StormVulnerabilityCandidate.HeightAndSlenderness)
            index = Square(height / 20) * hd / 75;
        else
        {
            double dominance = Math.Min(1, height / Math.Max(1, context.LocalTopHeight));
            index = Square(height / 20) * Square(dominance) * Square(hd / 70)
                * (1 + .6 * light) * (1 + .8 * recent);
            if (candidate == StormVulnerabilityCandidate.FullProposal)
                index *= Math.Max(0, Math.Min(1, (height - 8) / 6));
        }
        // Scenario One site and external edge are neutral. The explicit site
        // argument permits a later reviewed driver; no outside landscape is inferred.
        return (float)Math.Min(float.MaxValue, Math.Max(0, index * siteFactor));
    }

    public static float FailureChance(float vulnerability, float severity, StormProbabilityTransform transform)
    {
        if (!StormCalibration.Finite(vulnerability) || vulnerability < 0f
            || !StormCalibration.Finite(severity) || severity <= 0f || severity > 1f)
            throw new ArgumentException("Failure requires a finite relative index and bounded positive storm intensity.");
        if (!Enum.IsDefined(typeof(StormProbabilityTransform), transform))
            throw new ArgumentException("Unknown storm probability transform.");
        double load = (double)vulnerability * severity;
        double chance = transform == StormProbabilityTransform.Exponential ? 1 - Math.Exp(-load) : load / (1 + load);
        // Finite positive trees remain resistant rather than exactly immune;
        // float rounding cannot turn a very large finite index into certainty.
        return (float)Math.Max(0, Math.Min(.999999, chance));
    }

    public static StormEventRecord SampleEvent(StormCalibration calibration, int rngModel, int year, int seed)
    {
        if (SimulationRandom.Roll(rngModel, "STORM-OCCURS-v1", year, seed) >= calibration.AnnualProbability) return null;
        float total = calibration.Weights[0] + calibration.Weights[1] + calibration.Weights[2];
        float roll = SimulationRandom.Roll(rngModel, "STORM-SEVERITY-v1", year, seed) * total;
        int level = roll < calibration.Weights[0] ? 0 : roll < calibration.Weights[0] + calibration.Weights[1] ? 1 : 2;
        return new StormEventRecord { year = year, severity = calibration.Severities[level],
            directionDegrees = SimulationRandom.Roll(rngModel, "STORM-DIRECTION-v1", year, seed) * 360f };
    }

    public static float FallBearing(StormEventRecord storm, string treeId, int rngModel, int seed)
    {
        float angle = storm.directionDegrees + (SimulationRandom.Roll(rngModel, "WINDTHROW-BEARING-v1-" + treeId, storm.year, seed) * 2f - 1f) * 20f;
        return Mathf.Repeat(angle, 360f);
    }

    private static double Square(double value) => value * value;
}

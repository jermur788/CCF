using System;
using System.Collections.Generic;
using UnityEngine;

// One routed ambient layer selected from habitat indicators. Volume is a 0..1
// mix target computed from ecological state; the audio backend maps it to an
// AudioSource when clips are available. No clips are hard-wired here.
[Serializable]
public sealed class ScenarioSoundscapeLayer
{
    public string layerId = "";
    public string displayName = "";
    [Range(0f, 1f)] public float volume;
    public string driver = "";
}

// Habitat-driven soundscape routing computed from an ecological snapshot.
// Indicators are derived read-only from Forestry/management state; they never
// feed back into tree biology or management rules.
[Serializable]
public sealed class ScenarioSoundscapeState
{
    public int year;
    [Range(0f, 1f)] public float structuralDiversity;
    [Range(0f, 1f)] public float deadwoodHabitat;
    [Range(0f, 1f)] public float understoreyDiversity;
    [Range(0f, 1f)] public float canopyOpenness;
    [Range(0f, 1f)] public float regenerationActivity;
    public List<ScenarioSoundscapeLayer> layers = new List<ScenarioSoundscapeLayer>();
}

public static class ScenarioSoundscape
{
    // Shannon-style evenness across the six functional groups [D], 0..1.
    public static float UnderstoreyEvenness(ScenarioUnderstoreyCell cell)
    {
        if (cell == null)
            return 0f;
        float[] groups = { cell.mosses, cell.ferns, cell.grasses, cell.forbs, cell.shrubs, cell.fungi };
        float total = 0f;
        foreach (float value in groups)
            total += Mathf.Max(0f, value);
        if (total <= 0.0001f)
            return 0f;
        float entropy = 0f;
        foreach (float value in groups)
        {
            float p = Mathf.Max(0f, value) / total;
            if (p > 1e-5f)
                entropy -= p * Mathf.Log(p);
        }
        return Mathf.Clamp01(entropy / Mathf.Log(groups.Length));
    }

    public static ScenarioSoundscapeState Compute(ScenarioEcologicalSnapshot snapshot,
        IReadOnlyList<ScenarioUnderstoreyCell> understorey, IReadOnlyList<ScenarioDeadwoodRecord> deadwood)
    {
        var state = new ScenarioSoundscapeState { year = snapshot != null ? snapshot.year : 0 };
        if (snapshot == null)
            return state;

        // Structural diversity: species richness plus DBH spread proxy [D].
        int speciesCount = snapshot.species != null ? snapshot.species.Count : 0;
        float richness = Mathf.Clamp01(speciesCount / 4f);
        float dbhSpread = Mathf.Clamp01(snapshot.meanDbhCm / 40f);
        state.structuralDiversity = Mathf.Clamp01(richness * 0.6f + dbhSpread * 0.4f);

        // Deadwood habitat normalised to a working range [D].
        state.deadwoodHabitat = Mathf.Clamp01(snapshot.deadwoodHabitatValue / 2f);

        // Mean functional-group evenness across the stand.
        float evennessSum = 0f;
        int evennessCount = 0;
        if (understorey != null)
        {
            foreach (ScenarioUnderstoreyCell cell in understorey)
            {
                evennessSum += UnderstoreyEvenness(cell);
                evennessCount++;
            }
        }
        state.understoreyDiversity = evennessCount > 0 ? evennessSum / evennessCount : 0f;

        state.canopyOpenness = Mathf.Clamp01(snapshot.meanLight);
        state.regenerationActivity = snapshot.occupiedRegenerationCells /
            Mathf.Max(1, snapshot.species != null ? 64 : 64);

        state.layers.Add(Layer("woodland-birds", "Woodland birdsong",
            0.25f + 0.4f * state.structuralDiversity + 0.35f * state.understoreyDiversity,
            "structural diversity + understorey diversity"));
        state.layers.Add(Layer("canopy-wind", "Wind in the canopy",
            0.2f + 0.6f * Mathf.Clamp01(snapshot.meanCanopy),
            "canopy cover"));
        state.layers.Add(Layer("understorey-insects", "Understorey insects",
            0.1f + 0.5f * state.understoreyDiversity + 0.3f * state.canopyOpenness,
            "understorey diversity + light"));
        state.layers.Add(Layer("woodpeckers", "Woodpeckers and wood-boring beetles",
            0.15f + 0.85f * state.deadwoodHabitat,
            "deadwood habitat value"));
        state.layers.Add(Layer("litter-stillness", "Litter and fungi stillness",
            0.3f + 0.5f * Mathf.Clamp01(1f - state.canopyOpenness),
            "shade and moss/fungi cover"));
        return state;
    }

    private static ScenarioSoundscapeLayer Layer(string id, string displayName, float volume, string driver)
    {
        return new ScenarioSoundscapeLayer
        {
            layerId = id,
            displayName = displayName,
            volume = Mathf.Clamp01(volume),
            driver = driver
        };
    }
}

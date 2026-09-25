using System;
using UnityEngine;

// Abstract functional-group cover per ecology cell. These are diagnostic
// habitat proxies (0..1), not individual plant species or tree-regen cohorts.
[Serializable]
public sealed class ScenarioUnderstoreyCell
{
    public int cellIndex;
    public int lastUpdatedYear;
    public float mosses;
    public float ferns;
    public float grasses;
    public float forbs;
    public float shrubs;
    public float fungi;
}

public static class ScenarioOneUnderstorey
{
    // [D] Provisional, deterministic light/site-response shapes. Shade favours
    // mosses and litter fungi; intermediate light ferns; larger gaps herbaceous
    // groups and shrubs. No stochastic rolls or feedback into Forestry growth.
    public static ScenarioUnderstoreyCell Target(int index, ForestEcologyCell ecologyCell)
    {
        float light = Mathf.Clamp01(ecologyCell.Light);
        float site = Mathf.Clamp01(ecologyCell.SiteProductivity);
        float stability = Mathf.Clamp01(ecologyCell.SoilStability * ecologyCell.EstablishmentSuitability);
        float opening = Mathf.Clamp01(ecologyCell.RecentOpening / 2f);
        return new ScenarioUnderstoreyCell
        {
            cellIndex = index,
            mosses = Mathf.Clamp01((0.45f - light) / 0.45f) * site * (0.85f - 0.25f * opening),
            ferns = Mathf.Clamp01((light - 0.06f) / 0.24f)
                * Mathf.Clamp01((0.75f - light) / 0.45f) * stability * 0.75f,
            grasses = Mathf.Clamp01((light - 0.40f) / 0.40f) * (0.6f + 0.4f * opening) * site,
            forbs = Mathf.Clamp01((light - 0.32f) / 0.38f) * stability * (0.55f + 0.45f * opening),
            shrubs = Mathf.Clamp01((light - 0.50f) / 0.40f) * stability * 0.7f,
            fungi = Mathf.Clamp01(0.35f + 0.35f * Mathf.Clamp01(ecologyCell.Canopy))
                * (0.8f + 0.2f * site)
        };
    }

    public static ScenarioUnderstoreyCell Initially(int index, ForestEcologyCell cell, int year)
    {
        ScenarioUnderstoreyCell result = Target(index, cell);
        result.lastUpdatedYear = year;
        return result;
    }

    public static void Advance(ScenarioUnderstoreyCell state, ForestEcologyCell cell, int year,
        float colonisationRate, float lossRate)
    {
        if (state == null || cell == null || year <= state.lastUpdatedYear)
            return;
        ScenarioUnderstoreyCell target = Target(state.cellIndex, cell);
        state.mosses = Step(state.mosses, target.mosses, colonisationRate, lossRate);
        state.ferns = Step(state.ferns, target.ferns, colonisationRate, lossRate);
        state.grasses = Step(state.grasses, target.grasses, colonisationRate, lossRate);
        state.forbs = Step(state.forbs, target.forbs, colonisationRate, lossRate);
        state.shrubs = Step(state.shrubs, target.shrubs, colonisationRate, lossRate);
        state.fungi = Step(state.fungi, target.fungi, colonisationRate, lossRate);
        state.lastUpdatedYear = year;
    }

    private static float Step(float current, float target, float rise, float fall)
    {
        return Mathf.Clamp01(Mathf.Lerp(current, target, Mathf.Clamp01(target > current ? rise : fall)));
    }
}

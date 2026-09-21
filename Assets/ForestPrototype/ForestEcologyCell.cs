using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ForestRegenerationCohort
{
    public TreeSpeciesDefinition Species { get; private set; }
    public string SpeciesId => Species != null ? Species.SpeciesId : "";
    public float SeedRain;
    public float Density;
    public float Height;
    public int EstablishYear = -1;

    public ForestRegenerationCohort(TreeSpeciesDefinition species)
    {
        Species = species;
    }

    public void Restore(float density, float height, int establishYear)
    {
        Density = Mathf.Max(0f, density);
        Height = Mathf.Max(0f, height);
        EstablishYear = establishYear;
    }
}

public sealed class ForestEcologyCell
{
    public Vector2 Center;
    public float Canopy;
    public float Light;

    // Site state ([C] simple defaults; no detailed soil chemistry in this milestone).
    public float SiteProductivity = 1f;
    public float SoilStability = 1f;
    public float EstablishmentSuitability = 1f;

    private readonly List<ForestRegenerationCohort> regeneration = new List<ForestRegenerationCohort>();
    public IReadOnlyList<ForestRegenerationCohort> Regeneration => regeneration;
    public bool HasRegeneration => regeneration.Exists(c => c != null && c.Density > 0f);

    public ForestRegenerationCohort FindCohort(string speciesId)
    {
        if (string.IsNullOrEmpty(speciesId))
            return null;
        foreach (ForestRegenerationCohort cohort in regeneration)
            if (cohort != null && string.Equals(cohort.SpeciesId, speciesId, StringComparison.Ordinal))
                return cohort;
        return null;
    }

    public ForestRegenerationCohort GetOrCreateCohort(TreeSpeciesDefinition species)
    {
        if (species == null || string.IsNullOrEmpty(species.SpeciesId))
            return null;
        ForestRegenerationCohort existing = FindCohort(species.SpeciesId);
        if (existing != null)
            return existing;
        var cohort = new ForestRegenerationCohort(species);
        int index = regeneration.FindIndex(c => string.CompareOrdinal(c.SpeciesId, species.SpeciesId) > 0);
        if (index < 0) regeneration.Add(cohort);
        else regeneration.Insert(index, cohort);
        return cohort;
    }

    public void RemoveCohortIfEmpty(ForestRegenerationCohort cohort)
    {
        if (cohort != null && cohort.Density <= 0f && cohort.SeedRain <= 0f)
            regeneration.Remove(cohort);
    }

    public void ClearSeedRain()
    {
        for (int i = regeneration.Count - 1; i >= 0; i--)
        {
            regeneration[i].SeedRain = 0f;
            if (regeneration[i].Density <= 0f)
                regeneration.RemoveAt(i);
        }
    }

    public void ClearRegeneration()
    {
        regeneration.Clear();
    }

    public float SharedOccupancy
    {
        get
        {
            float occupancy = 0f;
            foreach (ForestRegenerationCohort cohort in regeneration)
                if (cohort != null && cohort.Species != null && cohort.Density > 0f)
                    occupancy += cohort.Density / Mathf.Max(0.01f, cohort.Species.RegenDensityMax);
            return occupancy;
        }
    }

    // With one occupied species this uses the original clamp expression exactly.
    // Shared-space arithmetic is introduced only when another cohort occupies the cell.
    public void AddDensityWithSharedCapacity(ForestRegenerationCohort target, float requestedDensity)
    {
        if (target == null || target.Species == null || requestedDensity <= 0f)
            return;
        float otherOccupancy = 0f;
        foreach (ForestRegenerationCohort cohort in regeneration)
        {
            if (cohort == null || cohort == target || cohort.Species == null || cohort.Density <= 0f)
                continue;
            otherOccupancy += cohort.Density / Mathf.Max(0.01f, cohort.Species.RegenDensityMax);
        }
        float maximum = target.Species.RegenDensityMax;
        if (otherOccupancy <= 0f)
        {
            target.Density = Mathf.Min(maximum, target.Density + requestedDensity);
            return;
        }
        float targetMaximum = Mathf.Max(0f, 1f - otherOccupancy) * maximum;
        target.Density = Mathf.Min(targetMaximum, target.Density + requestedDensity);
    }

    // Wind exposure from recent local removals (diagnostic; decays annually).
    public float RecentOpening;
}

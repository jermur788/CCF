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
    // Diagnostic provenance only; never consumed by growth, mortality or
    // reproduction. A planted cohort is otherwise an ordinary cohort.
    public RegenerationOrigin Origin = RegenerationOrigin.Natural;
    public int OriginYear = -1;
    // Browsing v1 diagnostic of the most recent annual step: expected fraction
    // of this cohort whose leaders were browsed. Not saved; the biological
    // consequence is already carried by Height and Density.
    public float LastBrowsedFraction;
    public int LastBrowseAssessmentYear = -1;

    public ForestRegenerationCohort(TreeSpeciesDefinition species)
    {
        Species = species;
    }

    public void Restore(float density, float height, int establishYear,
        RegenerationOrigin origin = RegenerationOrigin.Natural, int originYear = -1)
    {
        Density = Mathf.Max(0f, density);
        Height = Mathf.Max(0f, height);
        EstablishYear = establishYear;
        Origin = origin;
        // Older saves carry no origin year; fall back to the establishment year
        // so natural cohorts still expose a meaningful diagnostic value.
        OriginYear = originYear >= 0 ? originYear : establishYear;
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

    public bool RemoveCohort(ForestRegenerationCohort cohort)
    {
        return cohort != null && regeneration.Remove(cohort);
    }

    // Seed arrival per species for the current year (rebuilt annually, never
    // saved). Model 0 also keeps its legacy copy on the cohort record.
    private readonly SortedDictionary<string, float> seedRainBySpecies = new SortedDictionary<string, float>(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, float> SeedRainBySpecies => seedRainBySpecies;

    public float SeedRainFor(string speciesId)
    {
        return !string.IsNullOrEmpty(speciesId) && seedRainBySpecies.TryGetValue(speciesId, out float value) ? value : 0f;
    }

    public void SetSeedRain(string speciesId, float seedRain)
    {
        if (!string.IsNullOrEmpty(speciesId))
            seedRainBySpecies[speciesId] = seedRain;
    }

    // Total juvenile abundance of one species over all its bands.
    public float SpeciesDensity(string speciesId)
    {
        float total = 0f;
        foreach (ForestRegenerationCohort cohort in regeneration)
            if (cohort != null && cohort.Density > 0f && string.Equals(cohort.SpeciesId, speciesId, StringComparison.Ordinal))
                total += cohort.Density;
        return total;
    }

    // The tallest living band of a species (display and diagnosis).
    public ForestRegenerationCohort TallestBand(string speciesId, float minimumDensity = 0f)
    {
        ForestRegenerationCohort best = null;
        foreach (ForestRegenerationCohort cohort in regeneration)
            if (cohort != null && cohort.Density > 0f && cohort.Density >= minimumDensity
                && string.Equals(cohort.SpeciesId, speciesId, StringComparison.Ordinal)
                && (best == null || cohort.Height > best.Height))
                best = cohort;
        return best;
    }

    // ----- Regeneration model 1: age bands -----
    // A band is identified by species + origin + establishment year. Bands are
    // kept in ordinal species order, then origin, then establishment year.
    // Four bands at or above RegenerationModel.RepresentationThreshold, plus
    // at most one sub-threshold accumulator, per species + origin + cell.
    public const int MaxBandsPerSpeciesOrigin = 4;

    public ForestRegenerationCohort FindBand(string speciesId, RegenerationOrigin origin, int establishYear)
    {
        foreach (ForestRegenerationCohort cohort in regeneration)
            if (cohort != null && cohort.Origin == origin && cohort.EstablishYear == establishYear
                && string.Equals(cohort.SpeciesId, speciesId, StringComparison.Ordinal))
                return cohort;
        return null;
    }

    public List<ForestRegenerationCohort> Bands(string speciesId, RegenerationOrigin origin)
    {
        var bands = new List<ForestRegenerationCohort>();
        foreach (ForestRegenerationCohort cohort in regeneration)
            if (cohort != null && cohort.Origin == origin && string.Equals(cohort.SpeciesId, speciesId, StringComparison.Ordinal))
                bands.Add(cohort);
        return bands;
    }

    public void InsertBand(ForestRegenerationCohort band)
    {
        int index = regeneration.FindIndex(c => CompareBands(c, band) > 0);
        if (index < 0) regeneration.Add(band);
        else regeneration.Insert(index, band);
    }

    private static int CompareBands(ForestRegenerationCohort a, ForestRegenerationCohort b)
    {
        int order = string.CompareOrdinal(a.SpeciesId, b.SpeciesId);
        if (order != 0) return order;
        order = ((int)a.Origin).CompareTo((int)b.Origin);
        return order != 0 ? order : a.EstablishYear.CompareTo(b.EstablishYear);
    }

    // Model 1 admission: new abundance takes only free shared occupancy and
    // never reduces existing stock. Returns the accepted abundance.
    public float AdmitDensity(ForestRegenerationCohort target, float requestedDensity)
    {
        if (target == null || target.Species == null || requestedDensity <= 0f)
            return 0f;
        float free = Mathf.Max(0f, 1f - SharedOccupancy);
        float accepted = Mathf.Min(requestedDensity, free * Mathf.Max(0.01f, target.Species.RegenDensityMax));
        if (accepted <= 0f)
            return 0f;
        target.Density += accepted;
        return accepted;
    }

    public void ClearSeedRain()
    {
        seedRainBySpecies.Clear();
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
        seedRainBySpecies.Clear();
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

    // Normalized shared capacity: each species contributes density / RegenDensityMax.
    // A single-species cohort at its RegenDensityMax therefore reads occupancy 1,
    // even though the cohort's own density value is RegenDensityMax (e.g. 1.5 for
    // Beech). Density and occupancy are intentionally different quantities.
    // Wind exposure from recent local removals (diagnostic; decays annually).
    public float RecentOpening;
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Browsing & Protection v1 (Docs/BrowsingProtectionV1.md).
//
// Ecological conditions that decide how exposed juveniles are to large-deer
// leader browsing. This is ecology only: who builds, pays for or maintains a
// fence or shelter belongs to the work/economy system, which will later create
// and update these records. Nothing here is saved: pressure and protection are
// configuration in v1, so a save/load continues with identical conditions.

// One deer-fenced exclosure. An intact fence is an access barrier, not a
// percentage reduction [G, Teagasc]; a breach restores normal local pressure.
[Serializable]
public sealed class BrowseProtectedArea
{
    public string areaId = "";
    // Closed polygon in world XZ (metres). At least three points.
    public List<Vector2> polygon = new List<Vector2>();
    public int installedYear;
    // First ecological year in which the fence no longer excludes deer; -1 = never.
    public int breachedYear = -1;

    public bool ExcludesDeer(int year)
        => polygon != null && polygon.Count >= 3 && year >= installedYear && (breachedYear < 0 || year < breachedYear);

    public bool Contains(Vector2 point)
    {
        // Even-odd ray cast. Points exactly on an edge may resolve either way;
        // juvenile positions are not snapped to fence lines.
        bool inside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            Vector2 a = polygon[i], b = polygon[j];
            if ((a.y > point.y) != (b.y > point.y)
                && point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x)
                inside = !inside;
        }
        return inside;
    }
}

// One individual deer shelter on an exact-position planted juvenile.
// While effective it denies leader access [G/E transferred]; it ages out after
// an initial ~6-8 year working assumption [G] and can fail earlier.
[Serializable]
public sealed class BrowseShelter
{
    public string shelterId = "";
    // World XZ position of the protected stem.
    public Vector2 position;
    public int installedYear;
    // [G] Irish/British guidance treats shelters as a ~6-8 year establishment
    // measure. Growth/site dependent; a working assumption, not a measured life.
    public int effectiveYears = 8;
    // First ecological year in which the shelter is broken/displaced; -1 = never.
    public int failedYear = -1;

    public const float MatchRadiusM = 0.25f;

    public bool IsEffective(int year)
        => year >= installedYear && year < installedYear + Mathf.Max(0, effectiveYears)
           && (failedYear < 0 || year < failedYear);
}

public enum BrowseProtectionState
{
    None,
    InsideIntactFence,
    BreachedFence,
    EffectiveShelter,
    ExpiredShelter,
    FailedShelter
}

public enum BrowseExposureReason
{
    NoBrowsePressure,
    NotPalatable,
    AboveBrowseReach,
    Protected,
    Exposed
}

// Inspectable answer to "how exposed is this juvenile to browsing this year,
// and why". Every term is recomputable from current authoritative state.
public struct BrowseAssessment
{
    public float BackgroundPressure;
    public float Palatability;
    public float HeightVulnerability;
    // 0 = deer excluded, 1 = full access. Cohorts may be partially protected.
    public float ProtectionAccess;
    // Future understorey concealment hook; 1 in v1 (no concealment).
    public float VegetationExposure;
    public float Probability;
    public BrowseProtectionState Protection;
    public BrowseExposureReason Reason;

    public string PressureBand => BrowsingConditions.PressureBand(BackgroundPressure);
}

public sealed class BrowsingConditions
{
    private float backgroundPressure;

    // [S] Normalised 0-1 stand-level large-deer browse pressure. Not a deer
    // density: the browsing report advises against a density conversion.
    public float BackgroundPressure
    {
        get => backgroundPressure;
        set => backgroundPressure = Mathf.Clamp01(float.IsNaN(value) ? 0f : value);
    }

    public readonly List<BrowseProtectedArea> ProtectedAreas = new List<BrowseProtectedArea>();
    public readonly List<BrowseShelter> Shelters = new List<BrowseShelter>();

    public bool HasProtection => ProtectedAreas.Count > 0 || Shelters.Count > 0;

    public void ClearProtection()
    {
        ProtectedAreas.Clear();
        Shelters.Clear();
    }

    // [C] Field-style bands from the browsing report (0-0.3 / 0.3-0.65 / >0.65).
    // UI interpretation only; ecology always uses the scalar.
    public static string PressureBand(float pressure)
        => pressure <= 0f ? "none" : pressure < 0.3f ? "low" : pressure <= 0.65f ? "moderate" : "high";

    // Protection for a single exact-position stem: an effective shelter, or a
    // point inside an intact fence. Shelters are checked first because an
    // effective shelter protects regardless of the surrounding fence.
    public BrowseProtectionState ProtectionAt(Vector2 position, int year, out float access)
    {
        BrowseProtectionState state = BrowseProtectionState.None;
        foreach (BrowseShelter shelter in Shelters)
        {
            if (shelter == null || (shelter.position - position).sqrMagnitude > BrowseShelter.MatchRadiusM * BrowseShelter.MatchRadiusM)
                continue;
            if (shelter.IsEffective(year))
            {
                access = 0f;
                return BrowseProtectionState.EffectiveShelter;
            }
            if (year >= shelter.installedYear)
                state = shelter.failedYear >= 0 && year >= shelter.failedYear
                    ? BrowseProtectionState.FailedShelter : BrowseProtectionState.ExpiredShelter;
        }
        foreach (BrowseProtectedArea area in ProtectedAreas)
        {
            if (area == null || area.polygon == null || area.polygon.Count < 3 || year < area.installedYear || !area.Contains(position))
                continue;
            if (area.ExcludesDeer(year))
            {
                access = 0f;
                return BrowseProtectionState.InsideIntactFence;
            }
            if (state == BrowseProtectionState.None)
                state = BrowseProtectionState.BreachedFence;
        }
        access = 1f;
        return state;
    }

    // [S] Fraction of a square cell that deer can still reach. A cohort is
    // treated as spread uniformly over its cell, so a fence covering part of
    // the cell protects that fraction. Shelters do not apply to cohorts.
    public float CohortAccess(Vector2 cellCenter, float cellSize, int year)
    {
        if (ProtectedAreas.Count == 0)
            return 1f;
        const int samples = 5;
        int excluded = 0;
        for (int ix = 0; ix < samples; ix++)
        for (int iz = 0; iz < samples; iz++)
        {
            var point = new Vector2(
                cellCenter.x + ((ix + 0.5f) / samples - 0.5f) * cellSize,
                cellCenter.y + ((iz + 0.5f) / samples - 0.5f) * cellSize);
            foreach (BrowseProtectedArea area in ProtectedAreas)
            {
                if (area != null && area.ExcludesDeer(year) && area.Contains(point))
                {
                    excluded++;
                    break;
                }
            }
        }
        return 1f - excluded / (float)(samples * samples);
    }
}

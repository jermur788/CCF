using System.Collections.Generic;
using UnityEngine;

// Existing management footprints; no biological age/DBH cutoff is invented.
public readonly struct ClearanceFootprint
{
    public readonly Vector3 Center;
    public readonly float Radius, HalfCell;
    public readonly int CellIndex;
    public bool IsCircle => Radius > 0f;
    private ClearanceFootprint(Vector3 center, float radius, float halfCell, int cell)
    { Center = center; Radius = radius; HalfCell = halfCell; CellIndex = cell; }
    public static ClearanceFootprint Planting(Vector3 position)
        => new ClearanceFootprint(position, Mathf.Sqrt(1f / Mathf.PI), 0f, -1);
    public static ClearanceFootprint Cell(ForestEcologyController ecology, int index)
    {
        Vector2 p = ecology.Cells[index].Center;
        return new ClearanceFootprint(new Vector3(p.x, 0f, p.y), 0f, ecology.CellSizeMeters * .5f, index);
    }
    public bool Contains(Vector3 position)
    {
        if (IsCircle) return new Vector2(position.x - Center.x, position.z - Center.z).sqrMagnitude <= Radius * Radius;
        return position.x >= Center.x - HalfCell && position.x < Center.x + HalfCell
            && position.z >= Center.z - HalfCell && position.z < Center.z + HalfCell;
    }
    public string SizeLabel => IsCircle ? $"1 m² · radius {Radius:0.00} m" : $"{HalfCell * 2:0.#} × {HalfCell * 2:0.#} m · {HalfCell * HalfCell * 4:0.#} m²";
}

public sealed class ClearanceCohortTarget
{
    public int CellIndex;
    public ForestRegenerationCohort Cohort;
    public float RemainingFraction;
    public Vector3 DisplayPosition;
}

public sealed class ClearanceTargets
{
    public ClearanceFootprint Footprint;
    public readonly List<ClearanceCohortTarget> Cohorts = new List<ClearanceCohortTarget>();
    public readonly List<PlantedJuvenile> Juveniles = new List<PlantedJuvenile>();
    public readonly List<Vector3> GroundPlants = new List<Vector3>();
    public readonly List<ScenarioUnderstoreyCell> Understorey = new List<ScenarioUnderstoreyCell>();
    public float Density;
    public bool AlreadyTreated;
    // Cohorts are aggregate portions, not counted physical seedlings.
    public bool HasTargets => Cohorts.Count + Juveniles.Count + GroundPlants.Count + Understorey.Count > 0;
    public string Summary => $"{Cohorts.Count} young-tree group{(Cohorts.Count == 1 ? "" : "s")} · {Juveniles.Count} planted sapling{(Juveniles.Count == 1 ? "" : "s")} · {GroundPlants.Count} ground-plant patch{(GroundPlants.Count == 1 ? "" : "es")}";
}

public struct HabitatVegetationSite
{
    public int CellIndex, Plant;
    public bool IsRush;
    public HabitatVisualClass Kind;
    public Vector3 Position;
    public float Angle, Size;
    public bool IsCompeting => Kind != HabitatVisualClass.MossCarpet && Kind != HabitatVisualClass.DeadwoodFungi;
}

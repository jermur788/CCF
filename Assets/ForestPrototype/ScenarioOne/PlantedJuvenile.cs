using System;
using UnityEngine;

// A deliberately planted juvenile at an exact world position. Unlike natural
// regeneration cohorts (aggregated per ecology cell), planted juveniles retain
// individual identity, species, position, planting year, age/height and
// survival state. They use the local ecology cell for light and environmental
// effects but never appear at a random offset within the cell.
[Serializable]
public sealed class PlantedJuvenile
{
    public string juvenileId = "";
    public string speciesId = "";
    public Vector3 position;
    public int cellIndex = -1;
    public int plantingYear;
    public float ageYears;
    public float heightMeters;
    public bool alive = true;
    public string stockItemId = "";
    public string promotedTreeId = "";
    public bool legacyCohortManaged;
}

// Spatial clearance patch around a planting site. Only patches made in the
// current treatment year participate in a union: future seed rain recolonises.
[Serializable]
public sealed class PlantingClearancePatch
{
    public Vector3 center;
    public float radiusMeters = 0.564f; // 1 m² circle
    public int createdYear;
}

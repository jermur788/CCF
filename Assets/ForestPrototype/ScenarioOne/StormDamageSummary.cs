using UnityEngine;

// Reconstructed review data, never another saved history or victim list.
public sealed class StormDamageSummary
{
    public StormEventRecord Event;
    public int TreesLost, AffectedCells;
    public float OriginalVolumeM3, DeadwoodRemainingM3, SalvagedVolumeM3;
    public Vector3 Centre;
    public int WaypointCell = -1;
}

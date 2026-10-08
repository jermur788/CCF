using UnityEngine;

// References to existing authored assets only; no biological or saved state.
public sealed class WindthrowVisualCatalog : ScriptableObject
{
    public GameObject freshRootPlate;
    public GameObject weatheredRootPlate;
    public static WindthrowVisualCatalog Load() => Resources.Load<WindthrowVisualCatalog>("WindthrowVisualCatalog");
}

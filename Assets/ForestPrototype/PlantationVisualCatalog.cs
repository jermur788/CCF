using System;
using UnityEngine;

// Explicit engine-ready mapping of the delivered art, in Unity tree-local metres.
// No asset dimension is used to infer biology or work eligibility.
public sealed class PlantationVisualCatalog : ScriptableObject
{
    public PlantationBase[] bases;
    public PlantationLod[] recentScars, healedScars;
    public Material[] materials;
    public GameObject polePrefab, firstThinningPrefab;
    public static PlantationVisualCatalog Load() => Resources.Load<PlantationVisualCatalog>("PlantationVisualCatalog");
}

[Serializable] public sealed class PlantationPiece
{
    public Mesh mesh;
    public int[] materialSlots;
}
[Serializable] public sealed class PlantationLod { public PlantationPiece[] pieces; }
[Serializable] public sealed class PlantationCapsule
{
    public string segmentId;
    public Vector3 start, end;
    public float radius;
    public bool flexible;
}
[Serializable] public sealed class PlantationBranch
{
    public string id;
    public Bounds bounds;
    public float attachmentHeight;
    public PlantationLod[] lods;
    public Matrix4x4[] scarTransforms;
    public PlantationCapsule[] contacts;
}
[Serializable] public sealed class PlantationBase
{
    public string id;
    public float authoredHeight;
    public PlantationLod[] core;
    public PlantationBranch[] branches;
}

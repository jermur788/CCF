using UnityEngine;

// References to delivered art, not new biological defect or wetland state.
public sealed class RecentAssetVisualCatalog : ScriptableObject
{
    public GameObject[] grasses;
    public GameObject[] rushes;
    public GameObject[] bentStages;
    public GameObject[] cavityStages;
    public GameObject bilberryCover;
    public GameObject herbRosette;
    public GameObject herbFlowering;
    public GameObject groundMoss;
    public GameObject deadwoodMushroom;
    public GameObject deadwoodBracket;
    [Tooltip("Authored floor-dressing anchors in the playable stand, not soil/moisture observations.")]
    public Vector3[] rushDressingPositions;

    public static RecentAssetVisualCatalog Load() => Resources.Load<RecentAssetVisualCatalog>("RecentAssetVisualCatalog");
}

using UnityEngine;

// Explicit prefab references only. No ecology values, timing or populations
// are stored here or written back to the forest save.
public sealed class SectionFiveVisualCatalog : ScriptableObject
{
    public GameObject oakLitter;
    public GameObject beechLitter;
    public GameObject mixedLitter;
    public GameObject[] smallDeadwood;
    public GameObject shadeFern;
    public GameObject bracken;
    public GameObject bramble;
    public GameObject mossOnWood;
    public GameObject freshLog;
    public GameObject decayedLog;
    public GameObject sitkaStump;
    public GameObject beechSapling;

    public static SectionFiveVisualCatalog Load() => Resources.Load<SectionFiveVisualCatalog>("SectionFiveVisualCatalog");
}

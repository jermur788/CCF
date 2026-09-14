using UnityEngine;

public sealed class ForestEcologyController : MonoBehaviour
{
    [SerializeField] private int ecologicalYear;

    public int EcologicalYear => ecologicalYear;

    // One explicit step for editor, MCP and debug tooling. Nothing in normal
    // gameplay advances ecological time yet; one ecological year is not tied
    // to a game day or real-time minute.
    [ContextMenu("Advance one ecological year")]
    public void AdvanceOneYear()
    {
        ecologicalYear++;
    }
}

using UnityEngine;

// Authored visual states for one tree base model. The array order is
// Unpruned, Low recent, Low healed, Medium recent, Medium healed, High
// recent, High healed. Selection uses only authoritative pruning data
// (lift count and last lift year); mesh names never imply management history.
[System.Serializable]
public sealed class ForestTreeVisualBase
{
    public string label;
    public GameObject[] pruningStates = new GameObject[7];

    public GameObject PrefabFor(int pruningLifts, bool scarsHealed)
    {
        int index = pruningLifts <= 0
            ? 0
            : 1 + (Mathf.Clamp(pruningLifts, 1, 3) - 1) * 2 + (scarsHealed ? 1 : 0);
        return pruningStates != null && index >= 0 && index < pruningStates.Length
            ? pruningStates[index]
            : null;
    }
}

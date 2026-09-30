using UnityEngine;
using UnityEngine.InputSystem;

// Gallery-only controls; no saves, work orders or simulation are installed in
// this scene. Authored stage dimensions are displayed at their delivered size.
public sealed class RecentAssetReviewControls : MonoBehaviour
{
    private GUIStyle style;
    private int selectedLod = -1;
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        int next = selectedLod;
        if (keyboard.digit0Key.wasPressedThisFrame) next = -1;
        if (keyboard.digit1Key.wasPressedThisFrame) next = 0;
        if (keyboard.digit2Key.wasPressedThisFrame) next = 1;
        if (keyboard.digit3Key.wasPressedThisFrame) next = 2;
        if (next == selectedLod) return;
        selectedLod = next;
        foreach (LODGroup group in UnityEngine.Object.FindObjectsByType<LODGroup>(FindObjectsSortMode.None))
            group.ForceLOD(selectedLod);
    }

    private void OnGUI()
    {
        if (style == null) style = new GUIStyle(GUI.skin.label) { wordWrap = true };
        style.fontSize = Mathf.RoundToInt(18f * ForestHud.Scale);
        style.normal.textColor = Color.white;
        string text = "DELIVERED ASSET REVIEW — visual references only\n"
            + "WASD walk · Shift run · mouse look · Esc release cursor\n"
            + "[1] LOD0  [2] LOD1  [3] LOD2  [0] Auto — current: "
            + (selectedLod < 0 ? "Auto" : "LOD" + selectedLod);
        float width = Mathf.Min(780f * ForestHud.Scale, Screen.width - 32f);
        float height = style.CalcHeight(new GUIContent(text), width - 24f) + 24f;
        Rect rect = new Rect(16f, Screen.height - height - 16f, width, height);
        ForestHud.Panel(rect);
        GUI.Label(new Rect(rect.x + 12f, rect.y + 12f, width - 24f, height - 24f), text, style);
    }
}

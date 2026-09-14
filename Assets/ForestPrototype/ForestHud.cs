using UnityEngine;

// Shared IMGUI helpers so prompt panels look like the wood HUD: a solid
// opaque dark panel instead of the translucent default skin box, sized with
// the same resolution scale.
public static class ForestHud
{
    public static readonly Color PanelColor = new Color(0.06f, 0.09f, 0.05f, 0.97f);

    public static float Scale => Mathf.Max(1f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));

    public static void Panel(Rect rect)
    {
        Color previous = GUI.color;
        GUI.color = PanelColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
        GUI.color = Color.white;
    }
}

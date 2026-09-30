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

    // Solid 1x1 face for style backgrounds, so dark-panel controls keep
    // readable contrast instead of inheriting the light default skin.
    public static Texture2D Solid(Color color)
    {
        var texture = new Texture2D(1, 1) { name = "ForestHud solid" };
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}

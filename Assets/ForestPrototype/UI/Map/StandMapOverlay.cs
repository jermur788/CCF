using System;

// The colours of the analytical layers. A layer is a translucent wash laid over the base map, so the player never loses sight of the
// forest underneath, and the number behind a colour is read from the pointer or the selected-square card, not printed on the map.
//
// The ecology keeps its values per 5 m square, so each wash is built from one value per square. The presentation stretches the squares
// into a smooth field (the squares themselves are not drawn): a colour at the middle of a square is that square's value, and between
// two squares it is an interpolation, which is how a raster layer is normally shown. Plain numbers, checked offline.
public static class StandMapOverlayColours
{
    // Ground light 0 (dark) to 1 (open). The middle of the range is almost clear so the forest shows through; deep shade washes blue,
    // open ground washes warm. Under a closed plantation the light is near 0 everywhere, and the map says so.
    private static readonly float[] LightAt = { 0f, 0.12f, 0.30f, 0.50f, 0.75f, 1f };
    private static readonly MapColour[] LightColour =
    {
        new MapColour(12, 20, 54, 156), new MapColour(26, 40, 92, 114), new MapColour(64, 82, 100, 66),
        new MapColour(118, 122, 102, 26), new MapColour(226, 198, 112, 88), new MapColour(255, 220, 124, 150)
    };

    public static MapColour Light(float light)
    {
        float v = Math.Max(0f, Math.Min(1f, light));
        for (int i = 1; i < LightAt.Length; i++)
            if (v <= LightAt[i]) return Blend(LightColour[i - 1], LightColour[i], (v - LightAt[i - 1]) / (LightAt[i] - LightAt[i - 1]));
        return LightColour[LightColour.Length - 1];
    }

    // Regeneration in a square: nothing is clear. Otherwise a green that strengthens with the tallest cohort (3 m and over is full),
    // with planted juveniles in the square lifting it a little (as the earlier layer did). Height is the real simulation value.
    public static MapColour Regeneration(float tallestHeightM, bool hasCohort, bool hasPlanted)
    {
        if (!hasCohort && !hasPlanted) return new MapColour(0, 0, 0, 0);
        float v = Math.Max(0f, Math.Min(1f, tallestHeightM / 3f + (hasPlanted ? 0.3f : 0f)));
        return Blend(new MapColour(176, 226, 150, 54), new MapColour(88, 214, 96, 176), v);
    }

    // For a legend ramp: n colours from the low end to the high end.
    public static MapColour[] Ramp(MapOverlay overlay, int n)
    {
        var colours = new MapColour[n];
        for (int i = 0; i < n; i++)
        {
            float v = n > 1 ? (float)i / (n - 1) : 0f;
            colours[i] = overlay == MapOverlay.Light ? Light(v) : Regeneration(v * 3f, true, false);
        }
        return colours;
    }

    public static MapColour Blend(MapColour a, MapColour b, float t)
    {
        t = Math.Max(0f, Math.Min(1f, t));
        return new MapColour((byte)Math.Round(a.R + (b.R - a.R) * t), (byte)Math.Round(a.G + (b.G - a.G) * t),
            (byte)Math.Round(a.B + (b.B - a.B) * t), (byte)Math.Round(a.A + (b.A - a.A) * t));
    }
}

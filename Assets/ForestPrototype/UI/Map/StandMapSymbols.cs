using System.Collections.Generic;

// Which presentation a map is drawn for. The full Stand Map and the HUD minimap draw the same model with the same transform and
// the same symbols; they differ only in what is filtered out and in how big they are.
public enum MapDetail { Mini, Full }

// The analytical layer drawn over the base map. None is the ordinary property map. A layer never replaces the base map: it adds a
// translucent field (Light, Regeneration), the shelters (Browsing) or the marked trees (Marks) on top of it.
public enum MapOverlay { None, Light, Regeneration, Browsing, Marks }

public readonly struct MapColour
{
    public readonly byte R, G, B, A;
    public MapColour(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }
    public MapColour WithAlpha(byte a) => new MapColour(R, G, B, a);
}

// How one symbol looks. This table is the ONLY place a map symbol's colour, thickness, dash pattern or legend wording is defined,
// so the minimap cannot grow symbols of its own and a symbol cannot differ between the two presentations or the legend.
public sealed class MapSymbolStyle
{
    public MapSymbol Symbol;
    public MapColour Colour;
    // Real width on the ground in metres, and the least width in pixels so a thin feature stays visible on a small map.
    public float WidthM, MinPx;
    // Drawn on the minimap as well as on the full map. Everything is drawn on the full map.
    public bool InMini;
    // The colour named in words in the legend, and what the symbol means. Colour never carries the meaning alone: racks also differ
    // by line pattern and width.
    public string ColourWord, Meaning;
    // A dash pattern in pixels along the line (both 0 = solid). A dot is a short dash with a round cap.
    public float DashPx, GapPx;
    // A translucent fill under the stroke (a rack's corridor, the clearing).
    public bool HasFill;
    public MapColour Fill;
}

public static class StandMapSymbols
{
    // The map palette: a dark survey sheet with restrained forest greens and muted earth tones. The ground is the dark moss showing
    // between crowns; the canopy colours are the unlit base of each kind (the crown sprites shade them to look rounded).
    public static readonly MapColour Ground = new MapColour(70, 66, 50);
    public static readonly MapColour ConiferBase = new MapColour(30, 64, 50);
    public static readonly MapColour BeechBase = new MapColour(92, 128, 70);
    public static readonly MapColour OakBase = new MapColour(116, 128, 64);
    public static readonly MapColour OtherBroadleafBase = new MapColour(104, 140, 80);
    public static readonly MapColour DeadBase = new MapColour(100, 94, 84);
    public static readonly MapColour StumpColour = new MapColour(206, 188, 150);
    // The property outline colour (the ground and the forest picture are drawn by StandMapCanopyRaster, not filled by the canvas).
    public static readonly MapColour PropertyOutline = new MapColour(238, 238, 226, 235);

    public static MapColour CanopyBase(MapTreeKind kind)
    {
        switch (kind)
        {
            case MapTreeKind.Conifer: return ConiferBase;
            case MapTreeKind.Beech: return BeechBase;
            case MapTreeKind.Oak: return OakBase;
            default: return OtherBroadleafBase;
        }
    }

    private static readonly Dictionary<MapSymbol, MapSymbolStyle> styles = Build();

    private static Dictionary<MapSymbol, MapSymbolStyle> Build()
    {
        var table = new Dictionary<MapSymbol, MapSymbolStyle>();
        MapSymbolStyle Add(MapSymbol symbol, MapColour colour, float widthM, float minPx, bool inMini, string colourWord, string meaning, float dash = 0f, float gap = 0f)
        {
            var style = new MapSymbolStyle { Symbol = symbol, Colour = colour, WidthM = widthM, MinPx = minPx, InMini = inMini, ColourWord = colourWord, Meaning = meaning, DashPx = dash, GapPx = gap };
            table[symbol] = style;
            return style;
        }
        void Fill(MapSymbolStyle style, MapColour fill) { style.HasFill = true; style.Fill = fill; }

        Add(MapSymbol.Property, PropertyOutline, 0f, 2.2f, true, "white dashed line", "property boundary", 9f, 6f);
        Add(MapSymbol.Path, new MapColour(190, 158, 110, 240), 2.6f, 2f, true, "tan band", "forest path");
        Add(MapSymbol.PathCasing, new MapColour(26, 22, 14, 185), 2.6f, 3.4f, true, "dark edge", "edge of the forest path");
        Fill(Add(MapSymbol.Landmark, new MapColour(214, 196, 140, 205), 0f, 1.2f, true, "ochre patch", "clearing"), new MapColour(214, 190, 128, 58));

        Fill(Add(MapSymbol.RackToDo, new MapColour(228, 198, 122, 240), 0f, 1.7f, true, "dashed ochre line", "rack still to plan", 9f, 6f), new MapColour(228, 198, 122, 36));
        Fill(Add(MapSymbol.RackPlanned, new MapColour(238, 172, 78, 255), 0f, 2.6f, true, "solid amber line", "rack in the Work Plan"), new MapColour(238, 172, 78, 84));
        Fill(Add(MapSymbol.RackOpen, new MapColour(236, 228, 200, 235), 0f, 1.3f, true, "pale open strip", "rack harvested, corridor open"), new MapColour(236, 228, 200, 78));
        Fill(Add(MapSymbol.RackOther, new MapColour(242, 216, 156, 240), 0f, 2.1f, true, "dotted ochre line", "your rack, off the planned layout", 2f, 5f), new MapColour(242, 216, 156, 44));
        Fill(Add(MapSymbol.RackPreview, new MapColour(252, 252, 244, 255), 0f, 2.8f, false, "white dashed line", "rack you are planning", 8f, 5f), new MapColour(255, 255, 255, 72));
        Fill(Add(MapSymbol.RackSuggested, new MapColour(228, 218, 170, 170), 0f, 1.8f, false, "faint dashed line", "suggested rack", 9f, 6f), new MapColour(228, 218, 170, 22));
        Fill(Add(MapSymbol.RackPlayer, new MapColour(238, 172, 78, 255), 0f, 2.8f, false, "solid amber line", "your planned rack"), new MapColour(238, 172, 78, 84));
        // The band is the rack's real corridor width (RackGeometry.DefaultWidthM); its colour comes from the rack it belongs to.
        Add(MapSymbol.RackBand, new MapColour(0, 0, 0, 0), RackGeometry.DefaultWidthM, 4f, true, "pale strip", "rack corridor");

        Add(MapSymbol.GridLine, new MapColour(255, 255, 255, 30), 0f, 1f, false, "fine white line", "local grid");
        Add(MapSymbol.Waypoint, new MapColour(238, 168, 48), 0f, 1.5f, true, "amber diamond", "destination");
        Add(MapSymbol.Player, new MapColour(104, 164, 240, 118), 0f, 1.5f, true, "blue wedge", "which way you face");
        Add(MapSymbol.PlayerDot, new MapColour(104, 164, 240), 0f, 1.5f, true, "blue dot", "you");
        Add(MapSymbol.ScaleBar, new MapColour(232, 234, 224, 235), 0f, 1.6f, false, "bar", "scale");
        Add(MapSymbol.NorthArrow, new MapColour(232, 234, 224, 235), 0f, 1.4f, true, "arrow", "north");

        Add(MapSymbol.TreeMarkFell, new MapColour(220, 76, 62), 0f, 2.2f, false, "red ring", "marked to fell");
        Add(MapSymbol.TreeMarkCrop, new MapColour(78, 140, 236), 0f, 2.6f, false, "blue ring", "Crop Tree");
        Add(MapSymbol.Juvenile, new MapColour(170, 226, 128), 0f, 1.2f, false, "light green dot", "planted juvenile");
        Add(MapSymbol.Shelter, new MapColour(128, 182, 222), 0f, 1.8f, false, "blue ring", "shelter, still effective");
        Add(MapSymbol.ShelterLapsed, new MapColour(158, 158, 150), 0f, 1.8f, false, "grey ring", "shelter, expired or failed");
        return table;
    }

    public static MapSymbolStyle Get(MapSymbol symbol) => styles[symbol];

    public static IEnumerable<MapSymbolStyle> All => styles.Values;

    // The filter. Mini shows the symbols marked InMini; Full shows all of them.
    public static bool IsDrawn(MapSymbol symbol, MapDetail detail) => detail == MapDetail.Full || styles[symbol].InMini;

    // One line naming the drawn symbols, for a presentation that prints its key as a sentence.
    public static string Legend(MapDetail detail)
    {
        var parts = new List<string>();
        foreach (MapSymbol symbol in new[] { MapSymbol.PlayerDot, MapSymbol.Player, MapSymbol.RackToDo, MapSymbol.RackPlanned, MapSymbol.RackOpen, MapSymbol.RackOther, MapSymbol.RackPreview, MapSymbol.Path, MapSymbol.Waypoint })
            if (IsDrawn(symbol, detail))
                parts.Add(styles[symbol].ColourWord + ": " + styles[symbol].Meaning);
        return string.Join(" · ", parts);
    }
}

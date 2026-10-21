using System.Collections.Generic;

// Which presentation a map is drawn for. The full Stand Map and the HUD minimap draw the same model with the same transform and
// the same symbols; they differ only in what is filtered out and in how big they are.
public enum MapDetail { Mini, Full }

public readonly struct MapColour
{
    public readonly byte R, G, B, A;
    public MapColour(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }
}

// How one symbol looks. This table is the ONLY place a map symbol's colour, thickness or legend wording is defined, so the
// minimap cannot grow symbols of its own and a symbol cannot differ between the two presentations.
public sealed class MapSymbolStyle
{
    public MapSymbol Symbol;
    public MapColour Colour;
    // Real width on the ground in metres, and the least width in pixels so a thin feature stays visible on a small map.
    public float WidthM, MinPx;
    // Drawn on the minimap as well as on the full map. Everything is drawn on the full map.
    public bool InMini;
    // The colour named in words in the legend, and what the symbol means. Colour never carries the meaning alone.
    public string ColourWord, Meaning;
}

public static class StandMapSymbols
{
    // The text marks the full map's Access layer prints in a square. They stand for the same rack states as the drawn lines.
    public const string RackToDoMark = " ◇";
    public const string RackDoneMark = " ✓";

    // The backing of the minimap (the full map's backing is its cell grid) and the property outline of both.
    public static readonly MapColour MiniBackground = new MapColour(26, 44, 28);
    public static readonly MapColour PropertyOutline = new MapColour(176, 204, 160, 200);

    private static readonly Dictionary<MapSymbol, MapSymbolStyle> styles = Build();

    private static Dictionary<MapSymbol, MapSymbolStyle> Build()
    {
        var table = new Dictionary<MapSymbol, MapSymbolStyle>();
        void Add(MapSymbol symbol, MapColour colour, float widthM, float minPx, bool inMini, string colourWord, string meaning)
        {
            table[symbol] = new MapSymbolStyle { Symbol = symbol, Colour = colour, WidthM = widthM, MinPx = minPx, InMini = inMini, ColourWord = colourWord, Meaning = meaning };
        }
        Add(MapSymbol.Property, PropertyOutline, 0f, 1.5f, true, "pale outline", "edge of your woodland");
        Add(MapSymbol.Path, new MapColour(184, 150, 104, 205), 2.6f, 2f, true, "tan band", "forest path");
        Add(MapSymbol.Landmark, new MapColour(204, 214, 186, 220), 0f, 1.5f, true, "pale ring", "clearing");
        Add(MapSymbol.RackToDo, new MapColour(182, 118, 232), 1.2f, 3f, true, "purple line", "rack route still to plan");
        Add(MapSymbol.RackPlanned, new MapColour(150, 214, 140), 1.2f, 3f, true, "green line", "rack in the Work Plan");
        Add(MapSymbol.RackOpen, new MapColour(228, 214, 170, 230), 1.2f, 3f, true, "cream line", "rack open (harvested)");
        Add(MapSymbol.RackOther, new MapColour(206, 156, 246), 1.2f, 3f, true, "light purple line", "your rack, off the planned layout");
        Add(MapSymbol.RackPreview, new MapColour(255, 255, 255, 235), 1.2f, 3f, false, "white line", "rack you are planning");
        Add(MapSymbol.Waypoint, new MapColour(232, 206, 120), 0f, 1.5f, true, "gold diamond", "destination");
        Add(MapSymbol.Player, new MapColour(255, 255, 255), 0f, 1.5f, true, "white arrow", "you, facing this way");
        return table;
    }

    public static MapSymbolStyle Get(MapSymbol symbol) => styles[symbol];

    public static IEnumerable<MapSymbolStyle> All => styles.Values;

    // The filter. Mini shows the symbols marked InMini; Full shows all of them.
    public static bool IsDrawn(MapSymbol symbol, MapDetail detail) => detail == MapDetail.Full || styles[symbol].InMini;

    // One line naming the drawn symbols for the legend of a presentation.
    public static string Legend(MapDetail detail)
    {
        var parts = new List<string>();
        foreach (MapSymbol symbol in new[] { MapSymbol.Player, MapSymbol.RackToDo, MapSymbol.RackPlanned, MapSymbol.RackOpen, MapSymbol.RackOther, MapSymbol.RackPreview, MapSymbol.Path, MapSymbol.Waypoint })
            if (IsDrawn(symbol, detail))
                parts.Add(styles[symbol].ColourWord + ": " + styles[symbol].Meaning);
        return string.Join(" · ", parts);
    }
}

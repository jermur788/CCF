using System.Collections.Generic;

// What the legend of the full map lists. It is built from the model, so it names only what is actually on the map right now: a
// category with nothing to show (no planted juveniles yet, no standing dead) has no row, and nothing is listed for a feature the
// scenario does not have (water, contours, rock). The wording comes from the single symbol table, so the legend cannot disagree with
// what is drawn.
public enum LegendKind
{
    Line,      // a stroked line with the symbol's pattern
    Band,      // a corridor: fill with a line down the middle
    Patch,     // a filled area with an outline (the clearing)
    Dot,       // a small filled disc
    Ring,      // a ring
    Diamond,
    Wedge,     // you: dot and facing wedge
    Crown,     // a tree crown of one kind
    Ramp,      // a colour ramp for a layer (the ends are named in Text and RampNote)
    Stump
}

public sealed class LegendEntry
{
    public LegendKind Kind;
    public string Text;
    public MapSymbol Symbol;
    public MapTreeKind Crown;
    public MapTreeState CrownState;
    public MapOverlay Ramp;
    // For a ramp: what its ends mean, e.g. "dark ... open".
    public string RampNote = "";
}

public static class StandMapLegend
{
    public static List<LegendEntry> Entries(StandMapModel model, MapOverlay overlay)
    {
        var list = new List<LegendEntry>();
        if (model == null || !model.HasProperty) return list;
        void Symbol(LegendKind kind, MapSymbol symbol, string text) => list.Add(new LegendEntry { Kind = kind, Symbol = symbol, Text = text });
        string Meaning(MapSymbol symbol) => Capitalise(StandMapSymbols.Get(symbol).Meaning);

        if (model.HasPlayer) Symbol(LegendKind.Wedge, MapSymbol.Player, "You: position and facing");
        if (model.HasWaypoint) Symbol(LegendKind.Diamond, MapSymbol.Waypoint, "Destination");

        bool todo = false, planned = false, open = false, other = false, preview = false;
        foreach (MapRack rack in model.Racks)
            switch (rack.Symbol)
            {
                case MapSymbol.RackToDo: todo = true; break;
                case MapSymbol.RackPlanned: planned = true; break;
                case MapSymbol.RackOpen: open = true; break;
                case MapSymbol.RackOther: other = true; break;
                case MapSymbol.RackPreview: preview = true; break;
            }
        if (preview) Symbol(LegendKind.Band, MapSymbol.RackPreview, "Rack you are planning");
        if (todo) Symbol(LegendKind.Band, MapSymbol.RackToDo, "Rack still to plan");
        if (planned) Symbol(LegendKind.Band, MapSymbol.RackPlanned, "Rack in the Work Plan");
        if (open) Symbol(LegendKind.Band, MapSymbol.RackOpen, "Rack harvested, corridor open");
        if (other) Symbol(LegendKind.Band, MapSymbol.RackOther, "Your rack, off the planned layout");
        if (model.Paths.Count > 0) Symbol(LegendKind.Line, MapSymbol.Path, Meaning(MapSymbol.Path));
        if (model.Landmarks.Count > 0) Symbol(LegendKind.Patch, MapSymbol.Landmark, "Clearing (work area)");
        Symbol(LegendKind.Line, MapSymbol.Property, "Property boundary");
        Symbol(LegendKind.Line, MapSymbol.GridLine, "Local grid, " + StandMapGridPlan.StepM(model.SizeM) + " m (east, north)");

        // The forest: only the kinds that stand on the map, named by their species.
        bool[] kinds = new bool[4];
        bool dead = false, stump = false;
        foreach (MapTree tree in model.Trees)
        {
            if (tree.State == MapTreeState.Stump) { stump = true; continue; }
            if (tree.State == MapTreeState.StandingDead) { dead = true; continue; }
            kinds[(int)tree.Kind] = true;
        }
        for (int k = 0; k < 4; k++)
            if (kinds[k])
                list.Add(new LegendEntry { Kind = LegendKind.Crown, Crown = (MapTreeKind)k, Text = KindName(model, (MapTreeKind)k) });
        if (dead) list.Add(new LegendEntry { Kind = LegendKind.Crown, Crown = MapTreeKind.OtherBroadleaf, CrownState = MapTreeState.StandingDead, Text = "Standing dead tree" });
        if (stump) list.Add(new LegendEntry { Kind = LegendKind.Stump, Text = "Stump" });
        if (model.Juveniles.Count > 0) Symbol(LegendKind.Dot, MapSymbol.Juvenile, "Planted juvenile");

        switch (overlay)
        {
            case MapOverlay.Light:
                list.Add(new LegendEntry { Kind = LegendKind.Ramp, Ramp = MapOverlay.Light, Text = "Ground light", RampNote = "0 dark · 1 open" });
                break;
            case MapOverlay.Regeneration:
                list.Add(new LegendEntry { Kind = LegendKind.Ramp, Ramp = MapOverlay.Regeneration, Text = "Young trees (tallest)", RampNote = "none · 3 m and over" });
                break;
            case MapOverlay.Browsing:
                // A ring row only for a kind of shelter that is actually ringed (the draw plan rings every shelter, effective or lapsed).
                bool effective = false, lapsed = false;
                foreach (MapShelter shelter in model.Shelters) { if (shelter.Effective) effective = true; else lapsed = true; }
                if (effective) Symbol(LegendKind.Ring, MapSymbol.Shelter, Meaning(MapSymbol.Shelter));
                if (lapsed) Symbol(LegendKind.Ring, MapSymbol.ShelterLapsed, Meaning(MapSymbol.ShelterLapsed));
                break;
            case MapOverlay.Marks:
                // Likewise a mark row only when a living tree carries that mark (the draw plan rings living marked trees only).
                bool fell = false, crop = false;
                foreach (MapTree tree in model.Trees)
                {
                    if (tree.State != MapTreeState.Living) continue;
                    if (tree.Mark == MapTreeMark.Fell) fell = true;
                    else if (tree.Mark == MapTreeMark.Crop) crop = true;
                }
                if (fell) Symbol(LegendKind.Ring, MapSymbol.TreeMarkFell, Meaning(MapSymbol.TreeMarkFell));
                if (crop) Symbol(LegendKind.Ring, MapSymbol.TreeMarkCrop, Meaning(MapSymbol.TreeMarkCrop));
                break;
        }
        return list;
    }

    // "Sitka spruce" or "Beech" for a kind, from the species actually standing; a generic class name only when the source gave none.
    public static string KindName(StandMapModel model, MapTreeKind kind)
    {
        string named = model.KindLabels[(int)kind];
        if (!string.IsNullOrEmpty(named)) return named;
        switch (kind)
        {
            case MapTreeKind.Conifer: return "Conifer";
            case MapTreeKind.Beech: return "Beech";
            case MapTreeKind.Oak: return "Oak";
            default: return "Other broadleaf";
        }
    }

    private static string Capitalise(string text) => string.IsNullOrEmpty(text) ? text : char.ToUpperInvariant(text[0]) + text.Substring(1);
}

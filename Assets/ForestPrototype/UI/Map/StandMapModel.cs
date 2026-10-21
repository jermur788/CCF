using System;
using System.Collections.Generic;

// Everything the Scenario One map can show, as plain data in world metres (x east, z north). The full Stand Map and the HUD
// minimap both read ONE of these (built by StandMapSource from the live game) and neither keeps map geometry of its own, so
// a change to the underlying data - a rack confirmed, a waypoint set - appears on both from the same source.
//
// Plain numbers with no UnityEngine, like RackGeometry, so the shared map logic can be checked offline on plain .NET.

// The things the map can draw. The list is the shared symbology: StandMapSymbols gives each one its single style.
public enum MapSymbol
{
    Property,      // the property boundary
    Path,          // the forest path
    Landmark,      // the forest clearing
    RackToDo,      // a planned layout rack that still has trees to plan
    RackPlanned,   // a layout rack whose trees are in the Work Plan (or being harvested)
    RackOpen,      // a layout rack that has been harvested: an open corridor
    RackOther,     // a rack the player confirmed that is not on the planned layout
    RackPreview,   // the rack being planned on the Stand Map (full map only)
    Waypoint,      // the destination chosen on the Stand Map
    Player         // where you are and which way you face
}

public sealed class MapPath
{
    public readonly List<RackPoint> Points = new List<RackPoint>();
    public float WidthM = 2.6f;
}

public sealed class MapRack
{
    public RackPoint Start, End;
    public MapSymbol Symbol = MapSymbol.RackToDo;
    // 0-based layout rack, or -1 for a rack that is not on the planned layout.
    public int LayoutIndex = -1;
}

public sealed class MapLandmark
{
    public RackPoint At;
    public float RadiusM = 4.5f;
    public string Name = "";
}

public sealed class StandMapModel
{
    // The property square: south-west corner and side in world metres, and the ecology grid laid over it.
    public bool HasProperty;
    public float MinX, MinZ, SizeM;
    public int CellsPerAxis;
    public float CellSizeM;

    public readonly List<MapPath> Paths = new List<MapPath>();
    public readonly List<MapRack> Racks = new List<MapRack>();
    public readonly List<MapLandmark> Landmarks = new List<MapLandmark>();

    public bool HasPlayer;
    public RackPoint Player;
    // Compass heading, 0 = north, 90 = east, clockwise.
    public float PlayerHeadingDegrees;

    public bool HasWaypoint;
    public RackPoint Waypoint;

    // Changes whenever anything a presentation draws changes, so a presentation repaints only when it must.
    public int Version;

    public StandMapTransform TransformFor(float sidePx) => new StandMapTransform(MinX, MinZ, SizeM, sidePx);

    // A hash of everything drawn. The player is rounded to 10 cm and 1 degree, so standing still does not repaint.
    public int ContentHash()
    {
        unchecked
        {
            int h = 17;
            h = Mix(h, HasProperty ? 1 : 0); h = Mix(h, MinX); h = Mix(h, MinZ); h = Mix(h, SizeM); h = Mix(h, CellsPerAxis);
            foreach (MapPath path in Paths)
            {
                h = Mix(h, path.Points.Count); h = Mix(h, path.WidthM);
                foreach (RackPoint p in path.Points) { h = Mix(h, p.X); h = Mix(h, p.Z); }
            }
            foreach (MapLandmark l in Landmarks) { h = Mix(h, l.At.X); h = Mix(h, l.At.Z); h = Mix(h, l.RadiusM); }
            foreach (MapRack r in Racks)
            {
                h = Mix(h, (int)r.Symbol); h = Mix(h, r.LayoutIndex);
                h = Mix(h, r.Start.X); h = Mix(h, r.Start.Z); h = Mix(h, r.End.X); h = Mix(h, r.End.Z);
            }
            h = Mix(h, HasWaypoint ? 1 : 0);
            if (HasWaypoint) { h = Mix(h, Waypoint.X); h = Mix(h, Waypoint.Z); }
            h = Mix(h, HasPlayer ? 1 : 0);
            if (HasPlayer) { h = Mix(h, Player.X); h = Mix(h, Player.Z); h = Mix(h, (int)Math.Round(PlayerHeadingDegrees)); }
            return h;
        }
    }

    private static int Mix(int h, int value) { unchecked { return h * 31 + value; } }
    private static int Mix(int h, float value) { unchecked { return h * 31 + (int)Math.Round(value * 10.0); } }
}

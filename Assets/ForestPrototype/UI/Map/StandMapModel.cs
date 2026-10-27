using System;
using System.Collections.Generic;

// Everything the Scenario One map can show, as plain data in world metres (x east, z north). The full Stand Map and the HUD
// minimap both read ONE of these (built by StandMapSource from the live game) and neither keeps map geometry of its own, so
// a change to the underlying data - a rack confirmed, a waypoint set, a tree felled - appears on both from the same source.
//
// Plain numbers with no UnityEngine, like RackGeometry, so the shared map logic can be checked offline on plain .NET.

// The things the map can draw. The list is the shared symbology: StandMapSymbols gives each one its single style.
// The first ten are the Lane B symbols and keep their values; the rest were added by the professional map presentation.
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
    Player,        // which way you face (the wedge in front of the dot)
    PathCasing,    // the dark edge under the path, so it reads over any canopy
    RackBand,      // the true-width corridor a rack opens (2.4 m), under the rack's centre line
    GridLine,      // the local property grid (full map only)
    PlayerDot,     // exactly where you are
    ScaleBar,      // a length on the ground, labelled by the presentation
    NorthArrow,    // north is up; the arrow says so
    TreeMarkFell,  // a tree marked to fell (full map, Marks layer)
    TreeMarkCrop,  // a Crop Tree (full map, Marks layer)
    Juvenile,      // a planted juvenile, at its exact position (full map)
    Shelter,       // a browsing shelter that is still effective (full map, Browsing layer)
    ShelterLapsed  // a browsing shelter that has expired or failed (full map, Browsing layer)
}

// How a tree is drawn: a presentation class chosen from the species, not a simulation value. A species the map does not know
// is drawn as OtherBroadleaf rather than guessed to be something else.
public enum MapTreeKind : byte { Conifer, Beech, Oak, OtherBroadleaf }
public enum MapTreeState : byte { Living, StandingDead, Stump }
public enum MapTreeMark : byte { None, Fell, Crop }

// One tree at its exact world position. Size is the simulation's own crown radius, height and DBH; Seed is a stable number made
// from the tree's id that only varies how its crown is drawn, so two neighbouring crowns do not look stamped from one mould.
public struct MapTree
{
    public float X, Z;
    public float CrownRadiusM, HeightM, DbhCm;
    public MapTreeKind Kind;
    public MapTreeState State;
    public MapTreeMark Mark;
    public int Seed;
}

// A planted juvenile (individual identity and position, unlike natural regeneration which is only known per 5 m square).
public struct MapJuvenile
{
    public float X, Z, HeightM;
    public MapTreeKind Kind;
}

public struct MapShelter
{
    public float X, Z;
    public bool Effective;
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

// The few figures the map's information card states about the stand. Derived from the trees on the map, never typed in.
public struct StandMapStats
{
    public int LivingTrees, StandingDead, Stumps;
    public float MeanDbhCm, StemsPerHectare, BasalAreaM2PerHectare;
}

public sealed class StandMapModel
{
    // The property square: south-west corner and side in world metres, and the ecology grid laid over it.
    public bool HasProperty;
    public float MinX, MinZ, SizeM;
    public int CellsPerAxis;
    public float CellSizeM;
    public float AreaHectares => SizeM * SizeM / 10000f;

    public readonly List<MapPath> Paths = new List<MapPath>();
    public readonly List<MapRack> Racks = new List<MapRack>();
    public readonly List<MapLandmark> Landmarks = new List<MapLandmark>();

    // The forest. Sorted by height, shortest first, so a taller crown is drawn over a shorter one as it would be seen from above.
    public readonly List<MapTree> Trees = new List<MapTree>();
    public readonly List<MapJuvenile> Juveniles = new List<MapJuvenile>();
    public readonly List<MapShelter> Shelters = new List<MapShelter>();
    // What to call each tree kind in the legend, from the species actually standing ("" = none of that kind). Set by the source.
    public readonly string[] KindLabels = new string[4] { "", "", "", "" };

    // True when the scene has no terrain: the ground is a level plane and the map must not invent relief. (Scenario One has no
    // height data, so there is nothing to contour; this says so instead of drawing contours that mean nothing.)
    public bool GroundIsLevel = true;

    public bool HasPlayer;
    public RackPoint Player;
    // Compass heading, 0 = north, 90 = east, clockwise.
    public float PlayerHeadingDegrees;

    public bool HasWaypoint;
    public RackPoint Waypoint;

    // Three counters, so a presentation repaints only what changed:
    //   CanopyVersion   the trees' places, sizes, species or state (the canopy picture is redrawn only when this moves);
    //   StaticVersion   everything that does not move while you walk (property, grid inputs, path, racks, marks, juveniles, shelters);
    //   DynamicVersion  you, the destination and the rack being planned.
    // Version moves when any of them does (the one number a whole-map check compares).
    public int Version, CanopyVersion, StaticVersion, DynamicVersion;
    // Moves when any tree's mark changes (marks do not change the canopy picture, only the Marks layer).
    public int MarkVersion;

    public StandMapTransform TransformFor(float sidePx) => new StandMapTransform(MinX, MinZ, SizeM, sidePx);

    private int canopyHash = int.MinValue, markHash = int.MinValue, staticHash = int.MinValue, dynamicHash = int.MinValue;

    // Local property grid: metres east and north of the south-west corner. It is derived from the world coordinates, not invented.
    public float LocalEasting(float worldX) => worldX - MinX;
    public float LocalNorthing(float worldZ) => worldZ - MinZ;

    // Called by the source after it has re-read the trees. Returns true when the canopy picture needs redrawing.
    public bool StampTrees()
    {
        unchecked
        {
            int c = 17, m = 19;
            foreach (MapTree t in Trees)
            {
                c = Mix(c, t.X); c = Mix(c, t.Z); c = Mix(c, t.CrownRadiusM); c = Mix(c, (int)t.Kind); c = Mix(c, (int)t.State); c = Mix(c, t.Seed);
                m = Mix(m, (int)t.Mark);
            }
            c = Mix(c, Trees.Count);
            bool canopyChanged = c != canopyHash, markChanged = m != markHash;
            canopyHash = c; markHash = m;
            if (canopyChanged) CanopyVersion++;
            if (markChanged) MarkVersion++;
            return canopyChanged;
        }
    }

    // Moves the counters when anything drawn has changed. The player is rounded to 10 cm and 1 degree, so standing still does not repaint.
    public void Restamp()
    {
        int s = StaticContentHash(), d = DynamicContentHash();
        bool changed = false;
        if (s != staticHash) { staticHash = s; StaticVersion++; changed = true; }
        if (d != dynamicHash) { dynamicHash = d; DynamicVersion++; changed = true; }
        if (changed) Version++;
    }

    private int StaticContentHash()
    {
        unchecked
        {
            int h = 17;
            h = Mix(h, HasProperty ? 1 : 0); h = Mix(h, MinX); h = Mix(h, MinZ); h = Mix(h, SizeM); h = Mix(h, CellsPerAxis);
            h = Mix(h, GroundIsLevel ? 1 : 0);
            foreach (MapPath path in Paths)
            {
                h = Mix(h, path.Points.Count); h = Mix(h, path.WidthM);
                foreach (RackPoint p in path.Points) { h = Mix(h, p.X); h = Mix(h, p.Z); }
            }
            foreach (MapLandmark l in Landmarks) { h = Mix(h, l.At.X); h = Mix(h, l.At.Z); h = Mix(h, l.RadiusM); }
            foreach (MapRack r in Racks)
            {
                if (r.Symbol == MapSymbol.RackPreview) continue;
                h = Mix(h, (int)r.Symbol); h = Mix(h, r.LayoutIndex);
                h = Mix(h, r.Start.X); h = Mix(h, r.Start.Z); h = Mix(h, r.End.X); h = Mix(h, r.End.Z);
            }
            foreach (MapJuvenile j in Juveniles) { h = Mix(h, j.X); h = Mix(h, j.Z); h = Mix(h, (int)j.Kind); }
            foreach (MapShelter s in Shelters) { h = Mix(h, s.X); h = Mix(h, s.Z); h = Mix(h, s.Effective ? 1 : 0); }
            h = Mix(h, CanopyVersion); h = Mix(h, MarkVersion);
            return h;
        }
    }

    private int DynamicContentHash()
    {
        unchecked
        {
            int h = 23;
            foreach (MapRack r in Racks)
            {
                if (r.Symbol != MapSymbol.RackPreview) continue;
                h = Mix(h, r.LayoutIndex);
                h = Mix(h, r.Start.X); h = Mix(h, r.Start.Z); h = Mix(h, r.End.X); h = Mix(h, r.End.Z);
            }
            h = Mix(h, HasWaypoint ? 1 : 0);
            if (HasWaypoint) { h = Mix(h, Waypoint.X); h = Mix(h, Waypoint.Z); }
            h = Mix(h, HasPlayer ? 1 : 0);
            if (HasPlayer) { h = Mix(h, Player.X); h = Mix(h, Player.Z); h = Mix(h, (int)Math.Round(PlayerHeadingDegrees)); }
            return h;
        }
    }

    // One number for everything drawn (kept for callers that compare whole models).
    public int ContentHash() { unchecked { return StaticContentHash() * 31 + DynamicContentHash(); } }

    // The stand figures, from the living trees on the map. Cheap enough to ask for when a screen is built, not every frame.
    public StandMapStats ComputeStats()
    {
        var s = new StandMapStats();
        double dbhSum = 0, baSum = 0;
        foreach (MapTree t in Trees)
        {
            if (t.State == MapTreeState.Stump) { s.Stumps++; continue; }
            if (t.State == MapTreeState.StandingDead) { s.StandingDead++; continue; }
            s.LivingTrees++;
            dbhSum += t.DbhCm;
            baSum += Math.PI * (t.DbhCm / 200.0) * (t.DbhCm / 200.0);
        }
        float ha = AreaHectares;
        if (s.LivingTrees > 0) s.MeanDbhCm = (float)(dbhSum / s.LivingTrees);
        if (ha > 0f) { s.StemsPerHectare = s.LivingTrees / ha; s.BasalAreaM2PerHectare = (float)(baSum / ha); }
        return s;
    }

    private static int Mix(int h, int value) { unchecked { return h * 31 + value; } }
    private static int Mix(int h, float value) { unchecked { return h * 31 + (int)Math.Round(value * 10.0); } }
}

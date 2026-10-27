using System;

// A position inside the drawn map, in pixels or as a fraction of the map side. X grows to the east (right), Y grows to the
// south (down), the screen convention. The map is always drawn north-up.
public readonly struct MapPosition
{
    public readonly float X, Y;
    public MapPosition(float x, float y) { X = x; Y = y; }
}

// THE one world-to-map transform. The full Stand Map and the HUD minimap are two presentations of the same map, so every
// position on either of them comes from this struct and nowhere else: a world point has one fractional position inside the
// property square, and each presentation only multiplies it by its own side length.
//
// Plain numbers with no UnityEngine (like RackGeometry), so it can be checked offline on plain .NET
// (Tools/Verification/StandMap/OfflineLogic). World metres: x east, z north (the Unity X and Z axes), the same coordinates the
// ecology grid and the racks use. The map square is the property square; there is no second coordinate system.
public readonly struct StandMapTransform
{
    // South-west corner and side of the property square in world metres, and the side of the drawn map in pixels.
    public readonly float MinX, MinZ, SizeM, SidePx;

    public StandMapTransform(float minX, float minZ, float sizeM, float sidePx)
    {
        MinX = minX; MinZ = minZ; SizeM = sizeM; SidePx = sidePx;
    }

    public bool IsValid => SizeM > 0f && SidePx > 0f;

    public float PixelsPerMetre => SizeM > 0f ? SidePx / SizeM : 0f;

    public float MetresToPixels(float metres) => metres * PixelsPerMetre;

    // 0..1 across the property: X from the west edge, Y from the NORTH edge (so the north edge is the top of the map).
    public MapPosition WorldToFraction(RackPoint world)
    {
        if (SizeM <= 0f) return new MapPosition(0f, 0f);
        return new MapPosition((world.X - MinX) / SizeM, 1f - (world.Z - MinZ) / SizeM);
    }

    public MapPosition WorldToPixels(RackPoint world)
    {
        MapPosition f = WorldToFraction(world);
        return new MapPosition(f.X * SidePx, f.Y * SidePx);
    }

    public RackPoint PixelsToWorld(MapPosition pixels)
    {
        if (SidePx <= 0f) return new RackPoint(MinX, MinZ);
        return new RackPoint(MinX + pixels.X / SidePx * SizeM, MinZ + (1f - pixels.Y / SidePx) * SizeM);
    }

    // Where the top-left corner of a 5 m ecology cell sits and how big it is, derived from the SAME transform so the cell
    // buttons of the full map and everything drawn over them cannot drift apart. Cells are indexed z * perAxis + x, as in
    // ForestEcologyController.GetCellIndex.
    public MapPosition CellTopLeft(int index, int cellsPerAxis)
    {
        if (cellsPerAxis <= 0) return new MapPosition(0f, 0f);
        float cell = SizeM / cellsPerAxis;
        int x = index % cellsPerAxis, z = index / cellsPerAxis;
        // The cell's north-west corner in world metres.
        return WorldToPixels(new RackPoint(MinX + x * cell, MinZ + (z + 1) * cell));
    }

    public float CellSidePx(int cellsPerAxis) => cellsPerAxis > 0 ? SidePx / cellsPerAxis : 0f;

    // Compass heading (0 = north, 90 = east, clockwise) of a world direction on the ground plane.
    public static float HeadingOf(float dx, float dz) => (float)(Math.Atan2(dx, dz) * 180.0 / Math.PI);

    // The unit direction on the drawn map (pixels, Y down) for a compass heading: north is up the screen.
    public static MapPosition HeadingToDirection(float headingDegrees)
    {
        double r = headingDegrees * Math.PI / 180.0;
        return new MapPosition((float)Math.Sin(r), (float)-Math.Cos(r));
    }
}

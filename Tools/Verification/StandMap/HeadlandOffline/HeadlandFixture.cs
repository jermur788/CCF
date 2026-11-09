using System;
using System.Text.Json;

public static class HeadlandFixture
{
    public static string Bytes(object value) => JsonSerializer.Serialize(value, new JsonSerializerOptions { IncludeFields = true });

    public static StandMapModel Create()
    {
        var model = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
        var path = new MapPath { WidthM = 2.6f };
        path.Points.Add(new RackPoint(-2.436166f, -40f));
        path.Points.Add(new RackPoint(-2.436166f, -20f));
        model.Paths.Add(path);
        float[] positions = { -35.271175384521484f, -16.421764373779297f, 0.23823529481887817f, 15.793529510498047f, 35.23235321044922f };
        for (int index = 0; index < positions.Length; index++)
            model.Racks.Add(new MapRack
            {
                RackId = index + 1, Start = new RackPoint(-35.4f, positions[index]), End = new RackPoint(40f, positions[index]),
                Symbol = MapSymbol.RackPlayer, Selected = index == 0, FullOnly = index == 1
            });
        model.HasPlayer = true;
        model.Player = new RackPoint(1f, 2f);
        model.HasWaypoint = true;
        model.Waypoint = new RackPoint(3f, 4f);
        return model;
    }
}

public static class HeadlandFingerprint
{
    public static void Main()
    {
        StandMapModel model = HeadlandFixture.Create();
        foreach (float side in new[] { 128f, 196f, 520f, 880f })
            foreach (MapDetail detail in new[] { MapDetail.Mini, MapDetail.Full })
                foreach (MapOverlay overlay in Enum.GetValues<MapOverlay>())
                    Console.WriteLine(HeadlandFixture.Bytes(StandMapDrawPlan.Build(model, side, detail, overlay)));
        Console.WriteLine(HeadlandFixture.Bytes(StandMapLegend.Entries(model, MapOverlay.None)));
        Console.WriteLine(model.ContentHash());
    }
}

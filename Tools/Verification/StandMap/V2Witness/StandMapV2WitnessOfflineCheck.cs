using System;
using System.Collections.Generic;
using System.Linq;

// CCF #358 offline proof of the V2 witness logic on the REAL production draw-plan code (plain .NET, no Unity). It builds the same kind of
// model the V2 new game produces (two path pieces, a clearing, you, the existing headland + feeder, five suggested routes in
// MiniGuidance and nothing in Racks), draws it with StandMapDrawPlan at a minimap and a Stand Map size, then
//   1. confirms the witness PASSES on that honest plan with exactly 10 Mini-only items and 30 Mini-only points,
//   2. replays the HISTORIC StandMapParity.WholePlanParity counting on the same plans (matched 9 / missing 10 of 19, points 212 of 242),
//   3. proves the witness FAILS on defective plans (a missing shared item, a shifted point, a bogus or missing guidance item, ...),
//      including one the historic many-to-one match would accept.
// It proves the witness logic. It does not prove anything about the Unity canvases; the Editor runner supplies those.
public static class StandMapV2WitnessOfflineCheck
{
    static int passed, failed;
    static void Check(bool ok, string name, string detail = "") { if (ok) passed++; else failed++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " " + detail : "")); }

    const float MiniSide = 196f, FullSide = 688f;

    static StandMapModel V2NewGameModel(int suggestionCount, List<V2WitnessSuggestion> suggestions)
    {
        var m = new StandMapModel { HasProperty = true, MinX = -40f, MinZ = -40f, SizeM = 80f, CellsPerAxis = 16, CellSizeM = 5f };
        for (int piece = 0; piece < 2; piece++)
        {
            var path = new MapPath { WidthM = 2.6f };
            for (int i = 0; i < 52; i++) path.Points.Add(new RackPoint(-0.93f + 0.06f * i + 10f * piece, -14.75f + 0.5f * i - 5f * piece));
            m.Paths.Add(path);
        }
        m.Landmarks.Add(new MapLandmark { At = new RackPoint(-2.77f, 11f), RadiusM = 4.5f, Name = "Forest Clearing" });
        m.HasPlayer = true; m.Player = new RackPoint(-6.2f, 3.4f); m.PlayerHeadingDegrees = 37f;
        m.ExistingHeadlandSurfaces.Add(new MapHeadlandSurface(new RackPoint(-34.5f, -38f), new RackPoint(-34.5f, 38f), new RackPoint(-34.5f, -11f), new RackPoint(-1f, -11f), 2.6f, 3f));
        float[] zs = { -30f, -15f, 0f, 15f, 30f };
        for (int i = 0; i < suggestionCount; i++)
        {
            var s = new V2WitnessSuggestion { Entrance = new RackPoint(-34.5f, zs[i]), Far = new RackPoint(40f, zs[i]), WidthM = 2.4f };
            suggestions.Add(s);
            m.MiniGuidance.Add(new MapRack { Start = s.Entrance, End = s.Far, WidthM = s.WidthM, Symbol = MapSymbol.RackSuggested });
        }
        m.Restamp();
        return m;
    }

    static List<MapDrawItem> Plan(StandMapModel m, float side, MapDetail detail)
    {
        var plan = StandMapDrawPlan.BuildStatic(m, side, detail);
        plan.AddRange(StandMapDrawPlan.BuildDynamic(m, side, detail));
        return plan;
    }

    static MapDrawItem Clone(MapDrawItem i)
    {
        var c = (MapDrawItem)Activator.CreateInstance(typeof(MapDrawItem));
        c.Symbol = i.Symbol; c.Layer = i.Layer; c.Closed = i.Closed; c.Filled = i.Filled; c.WidthPx = i.WidthPx; c.Colour = i.Colour; c.Stroke = i.Stroke; c.StrokePx = i.StrokePx;
        c.DashPx = i.DashPx; c.GapPx = i.GapPx; c.World = i.World; c.Anchor = i.Anchor; c.SourceIndex = i.SourceIndex; c.Points = new List<MapPosition>(i.Points);
        return c;
    }
    static List<MapDrawItem> CloneAll(List<MapDrawItem> p) => p.Select(Clone).ToList();
    static void Shift(MapDrawItem i, int k, float dx, float dy) { i.Points[k] = new MapPosition(i.Points[k].X + dx, i.Points[k].Y + dy); }

    // The historic WholePlanParity counting (many-to-one first match), copied here only to show what it would have reported.
    static string HistoricCounts(List<MapDrawItem> mini, List<MapDrawItem> full, out int matched, out int missing)
    {
        bool Marker(MapSymbol s) => s == MapSymbol.Player || s == MapSymbol.PlayerDot || s == MapSymbol.Waypoint || s == MapSymbol.Landmark || s == MapSymbol.NorthArrow;
        matched = 0; missing = 0; int points = 0, expectedPoints = 0;
        foreach (MapDrawItem item in mini)
        {
            if (!Marker(item.Symbol)) expectedPoints += item.Points.Count;
            MapDrawItem other = full.FirstOrDefault(o => o.Symbol == item.Symbol && o.SourceIndex == item.SourceIndex && o.Points.Count == item.Points.Count);
            if (other == null) { missing++; continue; }
            matched++;
            if (!Marker(item.Symbol)) points += item.Points.Count;
        }
        return $"matched={matched} missing={missing} of {mini.Count} points={points} of {expectedPoints}";
    }

    static V2WitnessResult Run(StandMapModel m, List<V2WitnessSuggestion> s, List<MapDrawItem> mini, List<MapDrawItem> full)
        => StandMapV2WitnessLogic.EvaluateNotPlanning(m, s, 5, mini, MiniSide, full, FullSide);

    static bool Failing(V2WitnessResult r, string namePart) => !r.Ok && r.Lines.Any(l => l.StartsWith("V2WIT_FAIL") && l.Contains(namePart));

    public static int Main()
    {
        var suggestions = new List<V2WitnessSuggestion>();
        StandMapModel model = V2NewGameModel(5, suggestions);
        List<MapDrawItem> mini = Plan(model, MiniSide, MapDetail.Mini), full = Plan(model, FullSide, MapDetail.Full);

        // 1. The honest plan.
        V2WitnessResult honest = Run(model, suggestions, mini, full);
        foreach (string line in honest.Lines) Console.WriteLine("  " + line);
        foreach (string id in honest.Identities.Take(4)) Console.WriteLine("  " + id);
        Check(honest.Ok, "honest V2 new-game plan passes the witness", $"passed={honest.Passed} failed={honest.Failed}");
        Check(honest.MiniOnlyItems == 10 && honest.MiniOnlyPoints == 30, "honest plan: exactly 10 Mini-only items and 30 Mini-only points", $"items={honest.MiniOnlyItems} points={honest.MiniOnlyPoints}");
        Check(mini.Count == 19, "reconstructed minimap has 19 items (the count the historic run reported)", $"mini={mini.Count}");
        Check(mini.Count(i => i.Symbol == MapSymbol.RackBand && i.SourceIndex == -1 && i.Points.Count == 4) == 5 && mini.Count(i => i.Symbol == MapSymbol.RackSuggested && i.SourceIndex == -1 && i.Points.Count == 2) == 5,
            "the Mini-only items are 5 RackBand(4 pts) and 5 RackSuggested(2 pts), source index -1");
        Check(!full.Any(i => i.Symbol == MapSymbol.RackSuggested) && !full.Any(i => i.Symbol == MapSymbol.RackBand), "the Stand Map outside planning draws no suggested band or line");

        // 2. Replay of the historic counting on the same plans.
        string hist = HistoricCounts(mini, full, out int hm, out int hx);
        Console.WriteLine("  historic WholePlanParity counting on the reconstruction: " + hist);
        Check(hm == 9 && hx == 10 && mini.Count == 19, "historic counting reproduces matched=9 missing=10 of 19", hist);
        Check(hist.EndsWith("points=212 of 242"), "historic counting reproduces points=212 of 242 (30 fewer: 5x4 band + 5x2 line points)", hist);

        // 3. Mutations. Every one must FAIL the witness.
        void Mutant(string name, string expectFailureIn, Func<(StandMapModel, List<V2WitnessSuggestion>, List<MapDrawItem>, List<MapDrawItem>)> make)
        {
            var (m, s, mi, fu) = make();
            V2WitnessResult r = Run(m, s, mi, fu);
            Check(Failing(r, expectFailureIn), "mutant must FAIL: " + name, "failed=" + string.Join(" | ", r.Lines.Where(l => l.StartsWith("V2WIT_FAIL")).Select(l => l.Substring(11, Math.Min(60, l.Length - 11)))));
        }
        Mutant("a shared item (the path) is missing from the Stand Map", "MINI_ONLY_ITEMS_ARE_ONLY_GUIDANCE", () => { var f = CloneAll(full); f.Remove(f.First(i => i.Symbol == MapSymbol.Path && i.SourceIndex == 0)); return (model, suggestions, CloneAll(mini), f); });
        Mutant("one Stand Map path point is shifted by 0.01 of the map", "SHARED_POINTS_ARE_AT_THE_SAME_FRACTIONS", () => { var f = CloneAll(full); Shift(f.First(i => i.Symbol == MapSymbol.Path && i.SourceIndex == 0), 20, 0.01f * FullSide, 0f); return (model, suggestions, CloneAll(mini), f); });
        Mutant("one minimap path point is shifted by 0.01 of the map", "SHARED_POINTS_ARE_AT_THE_SAME_FRACTIONS", () => { var mi = CloneAll(mini); Shift(mi.First(i => i.Symbol == MapSymbol.PathCasing && i.SourceIndex == 1), 5, 0f, 0.01f * MiniSide); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("both canvases shifted together (agree with each other, wrong place)", "SHARED_GEOMETRY_IS_AT_THE_INDEPENDENTLY_COMPUTED_PLACE", () =>
        { var mi = CloneAll(mini); var f = CloneAll(full); for (int k = 0; k < 52; k++) { Shift(mi.First(i => i.Symbol == MapSymbol.Path && i.SourceIndex == 0), k, 0.02f * MiniSide, 0f); Shift(f.First(i => i.Symbol == MapSymbol.Path && i.SourceIndex == 0), k, 0.02f * FullSide, 0f); } return (model, suggestions, mi, f); });
        Mutant("an extra unexplained Mini-only item (a sixth bogus band)", "MINI_ONLY_COUNT_IS_TWO_PER_ROUTE", () => { var mi = CloneAll(mini); mi.Add(Clone(mi.First(i => i.Symbol == MapSymbol.RackBand))); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("a guidance centre line is missing from the minimap", "MINI_ONLY_COUNT_IS_TWO_PER_ROUTE", () => { var mi = CloneAll(mini); mi.Remove(mi.First(i => i.Symbol == MapSymbol.RackSuggested)); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("a guidance corridor band is missing from the minimap", "HAS_ITS_CORRIDOR_BAND", () => { var mi = CloneAll(mini); mi.Remove(mi.First(i => i.Symbol == MapSymbol.RackBand && i.SourceIndex == -1)); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("ten bogus Mini-only items with the right symbols and counts but the wrong places", "HAS_ITS_", () =>
        { var mi = CloneAll(mini); foreach (MapDrawItem i in mi.Where(x => x.SourceIndex == -1 && (x.Symbol == MapSymbol.RackBand || x.Symbol == MapSymbol.RackSuggested))) for (int k = 0; k < i.Points.Count; k++) Shift(i, k, 0f, 0.05f * MiniSide); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("one guidance line ends at the wrong place", "ROUTE_2_HAS_ITS_DASHED", () => { var mi = CloneAll(mini); Shift(mi.Where(i => i.Symbol == MapSymbol.RackSuggested).ElementAt(2), 1, 0f, 0.03f * MiniSide); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("a guidance line is solid, not dashed", "DASHED_CENTRE_LINE", () => { var mi = CloneAll(mini); mi.First(i => i.Symbol == MapSymbol.RackSuggested).DashPx = 0f; return (model, suggestions, mi, CloneAll(full)); });
        Mutant("the Stand Map also draws a suggested route outside planning", "DRAWS_NO_SUGGESTED_ROUTE", () => { var f = CloneAll(full); f.Add(Clone(mini.First(i => i.Symbol == MapSymbol.RackSuggested))); return (model, suggestions, CloneAll(mini), f); });
        Mutant("the minimap draws the existing headland stroke", "MINIMAP_DRAWS_NONE_OF_THE_EXISTING_HEADLAND", () => { var mi = CloneAll(mini); mi.Add(Clone(full.First(i => i.Symbol == MapSymbol.Path && i.SourceIndex == -1 && i.Points.Count == 2))); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("the minimap lacks the player dot", "SHARED_ITEMS_ARE_EXACTLY", () => { var mi = CloneAll(mini); mi.Remove(mi.First(i => i.Symbol == MapSymbol.PlayerDot)); return (model, suggestions, mi, CloneAll(full)); });
        Mutant("the Stand Map has an unexplained extra item", "STAND_MAP_ONLY_ITEMS_ARE_EXACTLY", () => { var f = CloneAll(full); f.Add(Clone(f.First(i => i.Symbol == MapSymbol.GridLine))); return (model, suggestions, CloneAll(mini), f); });
        Mutant("the Stand Map lacks a known Full-only item (a headland pad)", "STAND_MAP_ONLY_ITEMS_ARE_EXACTLY", () => { var f = CloneAll(full); f.Remove(f.Last(i => i.Symbol == MapSymbol.Path && i.SourceIndex == -1 && i.Points.Count == 20)); return (model, suggestions, CloneAll(mini), f); });
        Mutant("only four routes come from the foundation but the model holds five", "SUGGESTIONS_ARE_THE_FIVE", () => (model, suggestions.Take(4).ToList(), CloneAll(mini), CloneAll(full)));
        Mutant("model guidance differs from the foundation route", "GUIDANCE_3_EQUALS_FOUNDATION_ROUTE_3", () =>
        { var s2 = suggestions.Select(x => new V2WitnessSuggestion { Entrance = x.Entrance, Far = x.Far, WidthM = x.WidthM }).ToList(); s2[3].Far = new RackPoint(30f, 15f); return (model, s2, CloneAll(mini), CloneAll(full)); });
        // The historic many-to-one match accepts a duplicate; the one-to-one witness must not.
        {
            var mi = CloneAll(mini); mi.Add(Clone(mi.First(i => i.Symbol == MapSymbol.PathCasing && i.SourceIndex == 0)));
            HistoricCounts(mi, full, out int m2, out int x2);
            V2WitnessResult r = Run(model, suggestions, mi, CloneAll(full));
            Check(x2 == 10 && !r.Ok, "a duplicated shared Mini item is still invisible to the historic many-to-one counting but FAILS the one-to-one witness", $"historicMissing={x2}");
        }
        Console.WriteLine($"STANDMAP_V2_WITNESS_OFFLINE {(failed == 0 ? "PASS" : "DONE")} checks={passed} failed={failed}");
        return failed == 0 ? 0 : 1;
    }
}

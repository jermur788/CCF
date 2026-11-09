using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
#endif

// CCF #358: a V2-aware Mini-versus-Full Stand Map witness. It is NEW, bounded, verification-only code. It does not replace, edit or
// excuse Tools/Verification/StandMap/StandMapParity.cs, whose historic FAIL stays a FAIL. That older gate assumes the pre-V2 world
// (five pre-created layout racks, a minimap whose only text is "N", and EVERY minimap item present on the Stand Map).
//
// What this witness establishes, item by item and without subtracting a number:
//   1. The minimap items that have NO counterpart on the Stand Map are exactly the dashed V2 guidance: for each suggested route the
//      foundation returns, one RackBand (4 points) and one RackSuggested centre line (2 points), SourceIndex -1, each at the route's own
//      entrance/far fractions (computed here from the property bounds, not through StandMapTransform). Nothing else is Mini-only.
//   2. Everything else the minimap draws (property, path + casing, clearing, confirmed racks, you, destination, north arrow) has a
//      DISTINCT, one-to-one Stand Map counterpart at the same fractions of each canvas, and also at the independently computed place.
//   3. Everything that is on the Stand Map and not on the minimap is a known Full-only symbol in the expected number (grid, scale bar,
//      juveniles, the existing headland and feeder strokes and pads, draft racks); the minimap draws none of the headland.
//   4. The Stand Map outside planning mode draws no suggested route.
// Any other unmatched item, any shifted point, any missing guidance item is a failure. The logic below is plain C# (no UnityEngine) so
// StandMapV2WitnessOfflineCheck can prove it fails on defective plans; the Editor runner only supplies the REAL canvases' plans.
public sealed class V2WitnessSuggestion
{
    public RackPoint Entrance, Far;
    public float WidthM;
}

public sealed class V2WitnessResult
{
    public int Passed, Failed;
    public readonly List<string> Lines = new List<string>();
    public readonly List<string> Identities = new List<string>();
    public int MiniOnlyItems, MiniOnlyPoints, FullOnlyItems, MatchedItems;
    public bool Ok => Failed == 0;
}

public static class StandMapV2WitnessLogic
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    public const float PairTolerance = 1e-4f, IndependentTolerance = 2e-4f;

    private static string F(double v) => v.ToString("0.#####", Inv);

    private static void Check(V2WitnessResult r, string name, bool ok, string detail = "")
    {
        if (ok) r.Passed++; else r.Failed++;
        r.Lines.Add((ok ? "V2WIT_PASS " : "V2WIT_FAIL ") + name + (detail.Length > 0 ? " " + detail : ""));
    }

    // The independent expectation: x from the west edge, y from the NORTH edge, as a fraction of the property square.
    private static float FX(StandMapModel m, RackPoint p) => (p.X - m.MinX) / m.SizeM;
    private static float FY(StandMapModel m, RackPoint p) => 1f - (p.Z - m.MinZ) / m.SizeM;
    private static float Dist(float ax, float ay, float bx, float by) => (float)Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
    private static float PX(MapDrawItem i, int k, float side) => i.Points[k].X / side;
    private static float PY(MapDrawItem i, int k, float side) => i.Points[k].Y / side;
    private static bool IsMarker(MapSymbol s) => s == MapSymbol.Player || s == MapSymbol.PlayerDot || s == MapSymbol.Waypoint || s == MapSymbol.Landmark || s == MapSymbol.NorthArrow;
    private static string Key(MapDrawItem i) => i.Symbol + "#" + i.SourceIndex + "#" + i.Points.Count;
    private static string Describe(MapDrawItem i, float side)
        => $"{i.Symbol} idx={i.SourceIndex} pts={i.Points.Count} anchor=({F(i.Anchor.X / side)},{F(i.Anchor.Y / side)}) dash={F(i.DashPx)} layer={i.Layer}";

    // The shared-symbol items the model alone says the minimap must draw. This is computed from the MODEL, never from the plan under test.
    private static Dictionary<string, int> ExpectedSharedFromModel(StandMapModel m)
    {
        var e = new Dictionary<string, int>();
        void Add(MapSymbol s, int idx, int pts, int n) { if (n <= 0) return; string k = s + "#" + idx + "#" + pts; e[k] = (e.TryGetValue(k, out int c) ? c : 0) + n; }
        if (StandMapSymbols.IsDrawn(MapSymbol.Landmark, MapDetail.Mini))
            for (int i = 0; i < m.Landmarks.Count; i++) Add(MapSymbol.Landmark, i, 20, 1);
        for (int i = 0; i < m.Racks.Count; i++)
        {
            MapRack r = m.Racks[i];
            if (r.FullOnly || r.MiniOnly) continue;
            if (StandMapModel.IsDynamicRack(r.Symbol) || !StandMapSymbols.IsDrawn(r.Symbol, MapDetail.Mini)) continue;
            Add(MapSymbol.RackBand, i, 4, 1); Add(r.Symbol, i, 2, 1);
        }
        if (StandMapSymbols.IsDrawn(MapSymbol.Path, MapDetail.Mini))
            for (int i = 0; i < m.Paths.Count; i++)
                if (m.Paths[i].Points.Count >= 2) { Add(MapSymbol.PathCasing, i, m.Paths[i].Points.Count, 1); Add(MapSymbol.Path, i, m.Paths[i].Points.Count, 1); }
        Add(MapSymbol.Property, -1, 4, 1);
        if (StandMapSymbols.IsDrawn(MapSymbol.NorthArrow, MapDetail.Mini)) Add(MapSymbol.NorthArrow, -1, 4, 1);
        if (StandMapSymbols.IsDrawn(MapSymbol.Juvenile, MapDetail.Mini)) for (int i = 0; i < m.Juveniles.Count; i++) Add(MapSymbol.Juvenile, i, 20, 1);
        if (m.HasWaypoint && StandMapSymbols.IsDrawn(MapSymbol.Waypoint, MapDetail.Mini)) Add(MapSymbol.Waypoint, -1, 4, 1);
        if (m.HasPlayer && StandMapSymbols.IsDrawn(MapSymbol.Player, MapDetail.Mini)) { Add(MapSymbol.Player, -1, 8, 1); Add(MapSymbol.PlayerDot, -1, 20, 1); }
        if (StandMapSymbols.IsDrawn(MapSymbol.ScaleBar, MapDetail.Mini)) Add(MapSymbol.ScaleBar, -1, 4, 1);
        return e;
    }

    // What only the Stand Map may draw outside planning mode, by key, from the model alone.
    private static Dictionary<string, int> ExpectedFullOnlyFromModel(StandMapModel m)
    {
        var e = new Dictionary<string, int>();
        void Add(MapSymbol s, int idx, int pts, int n) { if (n <= 0) return; string k = s + "#" + idx + "#" + pts; e[k] = (e.TryGetValue(k, out int c) ? c : 0) + n; }
        int interior = StandMapGridPlan.InteriorOffsets(m.SizeM).Count;
        Add(MapSymbol.GridLine, -1, 2, interior * 2);
        if (!StandMapSymbols.IsDrawn(MapSymbol.ScaleBar, MapDetail.Mini)) Add(MapSymbol.ScaleBar, -1, 4, 1);
        if (!StandMapSymbols.IsDrawn(MapSymbol.Juvenile, MapDetail.Mini)) for (int i = 0; i < m.Juveniles.Count; i++) Add(MapSymbol.Juvenile, i, 20, 1);
        int surfaces = m.ExistingHeadlandSurfaces.Count;
        // Per surface: two casing strokes (2 pts) and two casing rings (20 pts), two fill strokes (2 pts) and two pad discs (20 pts); all SourceIndex -1.
        Add(MapSymbol.PathCasing, -1, 2, surfaces * 2); Add(MapSymbol.PathCasing, -1, 20, surfaces * 2);
        Add(MapSymbol.Path, -1, 2, surfaces * 2); Add(MapSymbol.Path, -1, 20, surfaces * 2);
        for (int i = 0; i < m.Racks.Count; i++)
        {
            MapRack r = m.Racks[i];
            if (r.MiniOnly || StandMapModel.IsDynamicRack(r.Symbol)) continue;
            bool miniDraws = !r.FullOnly && StandMapSymbols.IsDrawn(r.Symbol, MapDetail.Mini);
            if (!miniDraws) { Add(MapSymbol.RackBand, i, 4, 1); Add(r.Symbol, i, 2, 1); }
        }
        return e;
    }

    // OUTSIDE planning mode: guidance is in model.MiniGuidance and the Stand Map holds no suggestion.
    public static V2WitnessResult EvaluateNotPlanning(StandMapModel model, IReadOnlyList<V2WitnessSuggestion> suggestions, int expectedSuggestionCount,
        IReadOnlyList<MapDrawItem> mini, float miniSide, IReadOnlyList<MapDrawItem> full, float fullSide)
    {
        var r = new V2WitnessResult();
        int n = suggestions.Count;
        Check(r, "SUGGESTIONS_ARE_THE_FIVE_FOUNDATION_ROUTES", n == expectedSuggestionCount && n > 0, $"foundation={n} expected={expectedSuggestionCount}");
        Check(r, "MODEL_GUIDANCE_IS_THE_FOUNDATION_SUGGESTION", model.MiniGuidance.Count == n && !model.Racks.Any(x => x.Symbol == MapSymbol.RackSuggested), $"guidance={model.MiniGuidance.Count} racksSuggested={model.Racks.Count(x => x.Symbol == MapSymbol.RackSuggested)}");
        for (int i = 0; i < Math.Min(n, model.MiniGuidance.Count); i++)
        {
            MapRack g = model.MiniGuidance[i]; V2WitnessSuggestion s = suggestions[i];
            Check(r, $"GUIDANCE_{i}_EQUALS_FOUNDATION_ROUTE_{i}", g.Symbol == MapSymbol.RackSuggested && Math.Abs(g.Start.X - s.Entrance.X) < 1e-4f && Math.Abs(g.Start.Z - s.Entrance.Z) < 1e-4f
                && Math.Abs(g.End.X - s.Far.X) < 1e-4f && Math.Abs(g.End.Z - s.Far.Z) < 1e-4f && Math.Abs(g.WidthM - s.WidthM) < 1e-4f, $"{g.Start}->{g.End} w={F(g.WidthM)} vs {s.Entrance}->{s.Far} w={F(s.WidthM)}");
        }

        // 1. One-to-one matching on (symbol, source index, point count). Each Stand Map item can be claimed ONCE.
        var claimed = new bool[full.Count];
        var pairs = new List<KeyValuePair<MapDrawItem, MapDrawItem>>();
        var miniOnly = new List<MapDrawItem>();
        foreach (MapDrawItem item in mini)
        {
            int found = -1;
            for (int k = 0; k < full.Count && found < 0; k++)
                if (!claimed[k] && full[k].Symbol == item.Symbol && full[k].SourceIndex == item.SourceIndex && full[k].Points.Count == item.Points.Count) found = k;
            if (found < 0) { miniOnly.Add(item); continue; }
            claimed[found] = true; pairs.Add(new KeyValuePair<MapDrawItem, MapDrawItem>(item, full[found]));
        }
        var fullOnly = new List<MapDrawItem>();
        for (int k = 0; k < full.Count; k++) if (!claimed[k]) fullOnly.Add(full[k]);
        r.MatchedItems = pairs.Count; r.MiniOnlyItems = miniOnly.Count; r.FullOnlyItems = fullOnly.Count;
        r.MiniOnlyPoints = miniOnly.Sum(x => x.Points.Count);
        r.Lines.Add($"V2WIT_MEASURE counts mini={mini.Count} full={full.Count} matched={pairs.Count} miniOnly={miniOnly.Count} miniOnlyPoints={r.MiniOnlyPoints} fullOnly={fullOnly.Count}");

        // 2. The Mini-only set is exactly the guidance: per route one RackBand(-1, 4 pts) and one RackSuggested(-1, 2 pts), at that route's fractions.
        bool onlyKnownKinds = miniOnly.All(x => x.SourceIndex == -1 && ((x.Symbol == MapSymbol.RackBand && x.Points.Count == 4) || (x.Symbol == MapSymbol.RackSuggested && x.Points.Count == 2)));
        Check(r, "MINI_ONLY_ITEMS_ARE_ONLY_GUIDANCE_BANDS_AND_LINES", onlyKnownKinds, "unexpected=[" + string.Join("; ", miniOnly.Where(x => !(x.SourceIndex == -1 && ((x.Symbol == MapSymbol.RackBand && x.Points.Count == 4) || (x.Symbol == MapSymbol.RackSuggested && x.Points.Count == 2)))).Select(x => Describe(x, miniSide))) + "]");
        Check(r, "MINI_ONLY_COUNT_IS_TWO_PER_ROUTE", miniOnly.Count == 2 * n, $"miniOnly={miniOnly.Count} expected={2 * n}");
        Check(r, "MINI_ONLY_POINTS_ARE_SIX_PER_ROUTE", r.MiniOnlyPoints == 6 * n, $"points={r.MiniOnlyPoints} expected={6 * n} (4+2 per route)");
        var usedLine = new HashSet<MapDrawItem>(); var usedBand = new HashSet<MapDrawItem>();
        for (int i = 0; i < n; i++)
        {
            V2WitnessSuggestion s = suggestions[i];
            float ex = FX(model, s.Entrance), ey = FY(model, s.Entrance), fx = FX(model, s.Far), fy = FY(model, s.Far);
            MapDrawItem line = miniOnly.FirstOrDefault(x => x.Symbol == MapSymbol.RackSuggested && x.Points.Count == 2 && !usedLine.Contains(x)
                && Dist(PX(x, 0, miniSide), PY(x, 0, miniSide), ex, ey) < IndependentTolerance && Dist(PX(x, 1, miniSide), PY(x, 1, miniSide), fx, fy) < IndependentTolerance);
            Check(r, $"ROUTE_{i}_HAS_ITS_DASHED_CENTRE_LINE_AT_ITS_OWN_FRACTIONS_ON_THE_MINIMAP_ONLY", line != null && line.DashPx > 0f && line.GapPx > 0f, $"expected ({F(ex)},{F(ey)})->({F(fx)},{F(fy)})");
            if (line != null) { usedLine.Add(line); r.Identities.Add($"MINI_ONLY route={i} {Describe(line, miniSide)} <= foundation {s.Entrance}->{s.Far}"); }
            // Band: its end-edge midpoints are the route's entrance and far point; its width is the larger of the 2.4 m strip and the symbol's minimum pixels.
            float minPx = StandMapSymbols.Get(MapSymbol.RackBand).MinPx;
            float expectedWidth = Math.Max(s.WidthM / model.SizeM, minPx / miniSide);
            MapDrawItem band = miniOnly.FirstOrDefault(x => x.Symbol == MapSymbol.RackBand && x.Points.Count == 4 && !usedBand.Contains(x)
                && Dist((PX(x, 0, miniSide) + PX(x, 3, miniSide)) * 0.5f, (PY(x, 0, miniSide) + PY(x, 3, miniSide)) * 0.5f, ex, ey) < IndependentTolerance
                && Dist((PX(x, 1, miniSide) + PX(x, 2, miniSide)) * 0.5f, (PY(x, 1, miniSide) + PY(x, 2, miniSide)) * 0.5f, fx, fy) < IndependentTolerance);
            float bandWidth = band == null ? -1f : Dist(PX(band, 0, miniSide), PY(band, 0, miniSide), PX(band, 3, miniSide), PY(band, 3, miniSide));
            Check(r, $"ROUTE_{i}_HAS_ITS_CORRIDOR_BAND_AT_ITS_OWN_FRACTIONS_ON_THE_MINIMAP_ONLY", band != null && Math.Abs(bandWidth - expectedWidth) < 2e-4f, $"width={F(bandWidth)} expected={F(expectedWidth)}");
            if (band != null) { usedBand.Add(band); r.Identities.Add($"MINI_ONLY route={i} {Describe(band, miniSide)} width={F(bandWidth)} <= foundation {s.Entrance}->{s.Far}"); }
        }
        Check(r, "THE_STAND_MAP_DRAWS_NO_SUGGESTED_ROUTE_OUTSIDE_PLANNING", !full.Any(x => x.Symbol == MapSymbol.RackSuggested), $"count={full.Count(x => x.Symbol == MapSymbol.RackSuggested)}");

        // 3. The shared set is what the model says, in the expected number, and no minimap item draws the Stand Map's existing headland.
        var miniShared = new Dictionary<string, int>();
        foreach (KeyValuePair<MapDrawItem, MapDrawItem> p in pairs) { string k = Key(p.Key); miniShared[k] = (miniShared.TryGetValue(k, out int c) ? c : 0) + 1; }
        Dictionary<string, int> expectedShared = ExpectedSharedFromModel(model);
        string diff = DescribeDifference(expectedShared, miniShared);
        Check(r, "SHARED_ITEMS_ARE_EXACTLY_WHAT_THE_MODEL_SAYS_THE_MINIMAP_DRAWS", diff.Length == 0, diff);
        Check(r, "MINIMAP_DRAWS_NONE_OF_THE_EXISTING_HEADLAND", !mini.Any(x => (x.Symbol == MapSymbol.Path || x.Symbol == MapSymbol.PathCasing) && x.SourceIndex == -1)
            && mini.Count(x => x.Symbol == MapSymbol.Path) == model.Paths.Count(p => p.Points.Count >= 2), "");
        var fullOnlyActual = new Dictionary<string, int>();
        foreach (MapDrawItem x in fullOnly) { string k = Key(x); fullOnlyActual[k] = (fullOnlyActual.TryGetValue(k, out int c) ? c : 0) + 1; }
        string fullDiff = DescribeDifference(ExpectedFullOnlyFromModel(model), fullOnlyActual);
        Check(r, "STAND_MAP_ONLY_ITEMS_ARE_EXACTLY_THE_KNOWN_FULL_ONLY_SYMBOLS", fullDiff.Length == 0, fullDiff);
        foreach (MapDrawItem x in fullOnly) r.Identities.Add("FULL_ONLY " + Describe(x, fullSide));

        // 4. Shared geometry: same fractions on both canvases, and at the independently computed place.
        float worstPair = 0f; int pairedPoints = 0, expectedPoints = 0;
        foreach (KeyValuePair<MapDrawItem, MapDrawItem> p in pairs)
        {
            MapDrawItem a = p.Key, b = p.Value;
            if (!IsMarker(a.Symbol)) expectedPoints += a.Points.Count;
            if (!IsMarker(a.Symbol))
                for (int k = 0; k < a.Points.Count; k++) { worstPair = Math.Max(worstPair, Dist(PX(a, k, miniSide), PY(a, k, miniSide), PX(b, k, fullSide), PY(b, k, fullSide))); pairedPoints++; }
            if (a.Symbol != MapSymbol.NorthArrow) worstPair = Math.Max(worstPair, Dist(a.Anchor.X / miniSide, a.Anchor.Y / miniSide, b.Anchor.X / fullSide, b.Anchor.Y / fullSide));
        }
        Check(r, "SHARED_POINTS_ARE_AT_THE_SAME_FRACTIONS_ON_BOTH_CANVASES", worstPair < PairTolerance && pairedPoints == expectedPoints && pairedPoints > 0, $"points={pairedPoints}/{expectedPoints} worst={worstPair:0.0e+0}");
        float worstIndependent = 0f; int independentChecks = 0;
        foreach (KeyValuePair<MapDrawItem, MapDrawItem> p in pairs)
        {
            foreach (var side in new[] { new KeyValuePair<MapDrawItem, float>(p.Key, miniSide), new KeyValuePair<MapDrawItem, float>(p.Value, fullSide) })
            {
                MapDrawItem x = side.Key; float s = side.Value;
                if ((x.Symbol == MapSymbol.Path || x.Symbol == MapSymbol.PathCasing) && x.SourceIndex >= 0 && x.SourceIndex < model.Paths.Count)
                    for (int k = 0; k < x.Points.Count; k++) { RackPoint w = model.Paths[x.SourceIndex].Points[k]; worstIndependent = Math.Max(worstIndependent, Dist(PX(x, k, s), PY(x, k, s), FX(model, w), FY(model, w))); independentChecks++; }
                else if (x.Symbol == MapSymbol.Landmark && x.SourceIndex >= 0 && x.SourceIndex < model.Landmarks.Count)
                { RackPoint w = model.Landmarks[x.SourceIndex].At; worstIndependent = Math.Max(worstIndependent, Dist(x.Anchor.X / s, x.Anchor.Y / s, FX(model, w), FY(model, w))); independentChecks++; }
                else if (x.Symbol == MapSymbol.Player || x.Symbol == MapSymbol.PlayerDot)
                { worstIndependent = Math.Max(worstIndependent, Dist(x.Anchor.X / s, x.Anchor.Y / s, FX(model, model.Player), FY(model, model.Player))); independentChecks++; }
                else if (x.Symbol == MapSymbol.Waypoint)
                { worstIndependent = Math.Max(worstIndependent, Dist(x.Anchor.X / s, x.Anchor.Y / s, FX(model, model.Waypoint), FY(model, model.Waypoint))); independentChecks++; }
                else if (x.Symbol == MapSymbol.Property)
                {
                    var corners = new[] { new RackPoint(model.MinX, model.MinZ + model.SizeM), new RackPoint(model.MinX + model.SizeM, model.MinZ + model.SizeM), new RackPoint(model.MinX + model.SizeM, model.MinZ), new RackPoint(model.MinX, model.MinZ) };
                    for (int k = 0; k < x.Points.Count && k < 4; k++) { worstIndependent = Math.Max(worstIndependent, Dist(PX(x, k, s), PY(x, k, s), FX(model, corners[k]), FY(model, corners[k]))); independentChecks++; }
                }
                else if (x.Symbol == MapSymbol.RackBand && x.SourceIndex >= 0 && x.SourceIndex < model.Racks.Count)
                {
                    MapRack rack = model.Racks[x.SourceIndex];
                    worstIndependent = Math.Max(worstIndependent, Dist((PX(x, 0, s) + PX(x, 3, s)) * 0.5f, (PY(x, 0, s) + PY(x, 3, s)) * 0.5f, FX(model, rack.Start), FY(model, rack.Start)));
                    worstIndependent = Math.Max(worstIndependent, Dist((PX(x, 1, s) + PX(x, 2, s)) * 0.5f, (PY(x, 1, s) + PY(x, 2, s)) * 0.5f, FX(model, rack.End), FY(model, rack.End))); independentChecks += 2;
                }
                else if (x.SourceIndex >= 0 && x.SourceIndex < model.Racks.Count && x.Points.Count == 2 && x.Symbol == model.Racks[x.SourceIndex].Symbol)
                {
                    MapRack rack = model.Racks[x.SourceIndex];
                    worstIndependent = Math.Max(worstIndependent, Dist(PX(x, 0, s), PY(x, 0, s), FX(model, rack.Start), FY(model, rack.Start)));
                    worstIndependent = Math.Max(worstIndependent, Dist(PX(x, 1, s), PY(x, 1, s), FX(model, rack.End), FY(model, rack.End))); independentChecks += 2;
                }
            }
        }
        Check(r, "SHARED_GEOMETRY_IS_AT_THE_INDEPENDENTLY_COMPUTED_PLACE_ON_BOTH_CANVASES", worstIndependent < IndependentTolerance && independentChecks > 0, $"checks={independentChecks} worst={worstIndependent:0.0e+0}");
        return r;
    }

    private static string DescribeDifference(Dictionary<string, int> expected, Dictionary<string, int> actual)
    {
        var parts = new List<string>();
        foreach (string k in expected.Keys.Union(actual.Keys).OrderBy(x => x, StringComparer.Ordinal))
        {
            int e = expected.TryGetValue(k, out int a) ? a : 0, g = actual.TryGetValue(k, out int b) ? b : 0;
            if (e != g) parts.Add($"{k} expected={e} actual={g}");
        }
        return string.Join("; ", parts);
    }
}

#if UNITY_EDITOR
// The Editor runner: plays a fresh Scenario One game in a graphical Editor, reads what the REAL canvases last painted
// (StandMapCanvas.LastPlan on the walking HUD minimap and on the Stand Map), runs the logic above, and captures the real screen.
public static class StandMapV2Witness
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Vector2Int first = StandMapV2WitnessRunner.Sizes()[0];
        StandMapV2WitnessRunner.SetGameSize(first.x, first.y);
        EditorApplication.isPlaying = true;
    }

    // First-use screens are another harness's business; mark them seen so this one sees the HUD and the map directly.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "StandMapV2Witness.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "StandMapV2Witness.Begin"))
            new GameObject("Stand map V2 witness").AddComponent<StandMapV2WitnessRunner>();
    }
}

public sealed class StandMapV2WitnessRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private string output;
    private int shots, passed;
    private readonly List<string> failures = new List<string>(), evidence = new List<string>();

    private static string F(double v, string f = "0.#####") => v.ToString(f, Inv);
    private void Hard(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"V2WIT_RUNNER_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Note(string name, string detail) { evidence.Add(name + " " + detail); Debug.Log($"V2WIT_MEASURE {name} {detail}"); }

    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        System.Reflection.Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "V2Witness " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    public static Vector2Int[] Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        if (string.IsNullOrEmpty(text)) text = "1920x1080,1280x720";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (output != null) File.WriteAllLines(Path.Combine(output, "v2witness-evidence.txt"), evidence);
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"STANDMAP_V2_WITNESS_PASS shots={shots} checks={passed}"
            : $"STANDMAP_V2_WITNESS_FAIL shots={shots} checks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private static IEnumerator Settle(int frames = 6) { for (int i = 0; i < frames; i++) yield return null; }
    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }

    private IEnumerator Capture(string state)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
        File.WriteAllBytes(Path.Combine(output, state + "-" + Screen.width + "x" + Screen.height + ".png"), image.EncodeToPNG());
        Destroy(image);
        shots++;
    }

    private IEnumerator Invoke(Button button)
    {
        if (button == null) throw new InvalidOperationException("button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

    private Button PlanButton() => ui.Map.Root.Query<Button>().ToList().FirstOrDefault(b => b.text == "Plan Racks" || b.text.StartsWith("Planning Racks"));

    private void Report(string tag, V2WitnessResult result)
    {
        foreach (string line in result.Lines)
        {
            Debug.Log($"{line} [{tag}]");
            evidence.Add($"[{tag}] {line}");
            if (line.StartsWith("V2WIT_PASS")) passed++;
            else if (line.StartsWith("V2WIT_FAIL")) failures.Add($"[{tag}] {line}");
        }
        foreach (string id in result.Identities) { Debug.Log($"V2WIT_ITEM [{tag}] {id}"); evidence.Add($"[{tag}] {id}"); }
    }

    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        if (Application.isBatchMode) throw new InvalidOperationException("the V2 witness needs a graphical Editor");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/StandMap/evidence" : diag, "v2witness");
        Directory.CreateDirectory(output);
        yield return Wait(0.5f);
        if (ui.Help.IsOpen) { ui.CloseHelp(); yield return Settle(); }
        ui.CloseAll(); yield return Settle();
        ScenarioOneManager manager = ui.Manager;
        StandMapCanvas mini = ui.Minimap.Canvas, full = ui.Map.Canvas;
        Note("START", $"year={ui.Ecology.EcologicalYear} geometry={ui.Ecology.StandGeometryModelVersion} v2Active={manager.RackPlanningV2Active} confirmed={manager.RackPlanConfirmed} planRacks={manager.RackPlan.Count()} screen={Screen.width}x{Screen.height}");
        Hard("WORLD_IS_THE_V2_ENLARGED80_NEW_GAME", manager.RackPlanningV2Active && !manager.RackPlanConfirmed, $"v2={manager.RackPlanningV2Active} confirmed={manager.RackPlanConfirmed}");

        int sizeIndex = 0;
        foreach (Vector2Int size in Sizes())
        {
            string tag = size.x + "x" + size.y;
            if (sizeIndex++ > 0) { SetGameSize(size.x, size.y); yield return Wait(1.5f); }
            ui.CloseAll(); yield return Settle(12);
            StandMapModel model = ui.MapSource.Model;
            Hard($"{tag}_RACK_MODE_IS_OFF_AND_NO_SUGGESTION_IS_IN_RACKS", !ui.Map.RackMode && !ui.MapSource.ShowRackSuggestions && !model.Racks.Any(r => r.Symbol == MapSymbol.RackSuggested), $"rackMode={ui.Map.RackMode} show={ui.MapSource.ShowRackSuggestions}");
            Hard($"{tag}_MINIMAP_IS_SHOWN_AND_PAINTED_FROM_THE_CURRENT_MODEL", ui.Minimap.Visible && mini.PaintedVersion == model.Version, $"visible={ui.Minimap.Visible} version={mini.PaintedVersion}/{model.Version}");
            yield return Capture("hud-minimap");
            var miniPlan = mini.LastPlan.ToList(); float miniSide = mini.LastSidePx;
            ui.ShowMap(); yield return Settle(12);
            Hard($"{tag}_STAND_MAP_IS_PAINTED_FROM_THE_SAME_MODEL_VERSION", full.PaintedVersion == model.Version, $"version={full.PaintedVersion}/{model.Version}");
            var fullPlan = full.LastPlan.ToList(); float fullSide = full.LastSidePx;
            yield return Capture("stand-map-not-planning");
            var suggestions = manager.GetSuggestedRackLayout().Select(s => new V2WitnessSuggestion { Entrance = s.Entrance, Far = s.Far, WidthM = s.WidthM }).ToList();
            Note($"{tag}_PLAN_COUNTS", $"minimap={miniPlan.Count} full={fullPlan.Count} sides={F(miniSide)}/{F(fullSide)} foundationRoutes={suggestions.Count}");
            V2WitnessResult result = StandMapV2WitnessLogic.EvaluateNotPlanning(model, suggestions, 5, miniPlan, miniSide, fullPlan, fullSide);
            Report(tag + " not-planning", result);
            Note($"{tag}_NOT_PLANNING_SUMMARY", $"matched={result.MatchedItems} miniOnly={result.MiniOnlyItems} miniOnlyPoints={result.MiniOnlyPoints} fullOnly={result.FullOnlyItems} passed={result.Passed} failed={result.Failed}");
            File.WriteAllLines(Path.Combine(output, $"plan-items-{tag}.txt"),
                miniPlan.Select(i => "MINI " + i.Symbol + " idx=" + i.SourceIndex + " pts=" + i.Points.Count + " dash=" + F(i.DashPx)).Concat(fullPlan.Select(i => "FULL " + i.Symbol + " idx=" + i.SourceIndex + " pts=" + i.Points.Count + " dash=" + F(i.DashPx))));

            // Planning mode, informational: the same guidance moves into Racks (it is the Stand Map's business while planning). Measured, not gated.
            Button plan = PlanButton();
            if (plan != null && manager.RackPlanningV2Active)
            {
                yield return Invoke(plan); yield return Settle(12);
                var planningFull = full.LastPlan.ToList();
                Note($"{tag}_PLANNING", $"rackMode={ui.Map.RackMode} showSuggestions={ui.MapSource.ShowRackSuggestions} fullItems={planningFull.Count} fullRackSuggested={planningFull.Count(i => i.Symbol == MapSymbol.RackSuggested)} modelRacksSuggested={ui.MapSource.Model.Racks.Count(r => r.Symbol == MapSymbol.RackSuggested)} modelGuidance={ui.MapSource.Model.MiniGuidance.Count}");
                yield return Capture("stand-map-planning");
                Button off = PlanButton(); if (off != null) { yield return Invoke(off); yield return Settle(8); }
                Note($"{tag}_PLANNING_OFF", $"rackMode={ui.Map.RackMode} showSuggestions={ui.MapSource.ShowRackSuggestions}");
            }
            else Note($"{tag}_PLANNING", "plan button not found; planning-mode observation NOT RUN");
            ui.CloseAll(); yield return Settle(8);
        }
    }
}
#endif

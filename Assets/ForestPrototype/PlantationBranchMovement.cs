using System.Collections.Generic;
using UnityEngine;

// Presentation/gameplay contact only, separate from ecology and aim raycasts.
// A spatial grid limits queries to nearby tree instances. No Physics colliders.
public static class PlantationBranchMovement
{
    private const float CellSize = 4f;
    public const float ContactSpeedFactor = 0.35f; // [D] in-game calibration
    private const float SweepStep = 0.08f;
    private static readonly Dictionary<Vector2Int, HashSet<PlantationTreeVisual>> Grid =
        new Dictionary<Vector2Int, HashSet<PlantationTreeVisual>>();
    private static readonly Dictionary<PlantationTreeVisual, List<Vector2Int>> Membership =
        new Dictionary<PlantationTreeVisual, List<Vector2Int>>();
    private static readonly HashSet<PlantationTreeVisual> Candidates = new HashSet<PlantationTreeVisual>();
    public static int RegisteredTreeCount => Membership.Count;

    public static void Register(PlantationTreeVisual visual)
    {
        Unregister(visual);
        Bounds bounds = visual.ContactBounds;
        Vector2Int min = Cell(bounds.min), max = Cell(bounds.max);
        var cells = new List<Vector2Int>();
        for (int x = min.x; x <= max.x; x++)
            for (int z = min.y; z <= max.y; z++)
            {
                var cell = new Vector2Int(x, z);
                if (!Grid.TryGetValue(cell, out var set)) Grid[cell] = set = new HashSet<PlantationTreeVisual>();
                set.Add(visual); cells.Add(cell);
            }
        Membership[visual] = cells;
    }
    public static void Unregister(PlantationTreeVisual visual)
    {
        if (!Membership.TryGetValue(visual, out var cells)) return;
        foreach (Vector2Int cell in cells)
            if (Grid.TryGetValue(cell, out var set))
            {
                set.Remove(visual);
                if (set.Count == 0) Grid.Remove(cell);
            }
        Membership.Remove(visual);
    }
    private static Vector2Int Cell(Vector3 p) => new Vector2Int(Mathf.FloorToInt(p.x / CellSize), Mathf.FloorToInt(p.z / CellSize));

    public struct Contact
    {
        public bool touched;
        public int youngTrees, woodyBranches;
    }

    public static Contact Query(Vector3 foot, float height, float radius)
    {
        Candidates.Clear();
        Vector2Int min = Cell(foot - new Vector3(radius, 0, radius)), max = Cell(foot + new Vector3(radius, 0, radius));
        for (int x = min.x; x <= max.x; x++)
            for (int z = min.y; z <= max.y; z++)
                if (Grid.TryGetValue(new Vector2Int(x, z), out var set)) Candidates.UnionWith(set);
        var result = new Contact();
        Vector3 bottom = foot + Vector3.up * Mathf.Min(radius, height * 0.5f);
        Vector3 top = foot + Vector3.up * Mathf.Max(height - radius, height * 0.5f);
        foreach (PlantationTreeVisual visual in Candidates)
        {
            if (visual == null || !visual.isActiveAndEnabled || visual.Tree == null || visual.Tree.IsStump) continue;
            Bounds bounds = visual.ContactBounds;
            if (foot.x + radius < bounds.min.x || foot.x - radius > bounds.max.x
                || foot.z + radius < bounds.min.z || foot.z - radius > bounds.max.z
                || foot.y + height < bounds.min.y || foot.y > bounds.max.y) continue;
            int lastBranch = -1, branches = 0;
            for (int i = 0; i < visual.Contacts.Count; i++)
            {
                PlantationWorldCapsule capsule = visual.Contacts[i];
                float reach = radius + capsule.radius;
                if (foot.x < Mathf.Min(capsule.start.x, capsule.end.x) - reach
                    || foot.x > Mathf.Max(capsule.start.x, capsule.end.x) + reach
                    || foot.z < Mathf.Min(capsule.start.z, capsule.end.z) - reach
                    || foot.z > Mathf.Max(capsule.start.z, capsule.end.z) + reach
                    || top.y < Mathf.Min(capsule.start.y, capsule.end.y) - reach
                    || bottom.y > Mathf.Max(capsule.start.y, capsule.end.y) + reach) continue;
                float distance = SegmentDistanceSquared(bottom, top, capsule.start, capsule.end);
                if (distance > (radius + capsule.radius) * (radius + capsule.radius)) continue;
                result.touched = true;
                if (!capsule.flexible && capsule.branch != lastBranch)
                { lastBranch = capsule.branch; branches++; }
            }
            if (branches > 0 && visual.IsUnprunedYoung)
            { result.youngTrees++; result.woodyBranches += branches; }
        }
        return result;
    }

    public static bool DensePassage(Vector3 foot, float height, float radius, Vector3 direction)
    {
        return DensePassage(foot, height, radius, direction, Query(foot, height, radius));
    }

    private static bool DensePassage(Vector3 foot, float height, float radius, Vector3 direction, Contact centre)
    {
        if (centre.youngTrees < 2 || centre.woodyBranches < 3) return false;
        Vector3 side = Vector3.Cross(Vector3.up, direction.normalized) * (radius + 0.12f);
        // Both sides of the body-width route must also meet woody systems.
        // Conservative foliage envelopes never make a hard barrier by themselves.
        return Query(foot + side, height, radius * 0.65f).woodyBranches > 0
            && Query(foot - side, height, radius * 0.65f).woodyBranches > 0;
    }

    public static Vector3 Resolve(Vector3 foot, float height, float radius, Vector3 attempted)
    {
        Vector3 horizontal = new Vector3(attempted.x, 0, attempted.z);
        float budget = horizontal.magnitude;
        if (budget <= 0.00001f || Grid.Count == 0) return attempted;
        Vector3 direction = horizontal / budget;
        bool escaping = DensePassage(foot, height, radius, direction);
        float travelled = 0f;
        while (budget > 0.00001f)
        {
            float step = Mathf.Min(SweepStep, budget);
            Vector3 next = foot + direction * (travelled + step);
            Contact nextContact = Query(next, height, radius);
            bool dense = DensePassage(next, height, radius, direction, nextContact);
            if (dense && !escaping) break;
            // Once outside, do not allow entry into another dense patch.
            if (!dense) escaping = false;
            bool contact = nextContact.touched
                || Query(foot + direction * (travelled + step * 0.5f), height, radius).touched;
            float cost = contact ? step / ContactSpeedFactor : step;
            if (cost > budget) step *= budget / cost;
            travelled += step;
            budget -= Mathf.Min(cost, budget);
        }
        return direction * travelled + Vector3.up * attempted.y;
    }

    // Closest distance between two finite segments (including degenerate axes).
    public static float SegmentDistanceSquared(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        Vector3 d1 = q1 - p1, d2 = q2 - p2, r = p1 - p2;
        float a = Vector3.Dot(d1, d1), e = Vector3.Dot(d2, d2), f = Vector3.Dot(d2, r), s, t;
        if (a <= 1e-8f && e <= 1e-8f) return r.sqrMagnitude;
        if (a <= 1e-8f) { s = 0; t = Mathf.Clamp01(f / e); }
        else
        {
            float c = Vector3.Dot(d1, r);
            if (e <= 1e-8f) { t = 0; s = Mathf.Clamp01(-c / a); }
            else
            {
                float b = Vector3.Dot(d1, d2), denom = a * e - b * b;
                s = denom > 1e-8f ? Mathf.Clamp01((b * f - c * e) / denom) : 0;
                t = (b * s + f) / e;
                if (t < 0) { t = 0; s = Mathf.Clamp01(-c / a); }
                else if (t > 1) { t = 1; s = Mathf.Clamp01((b - c) / a); }
            }
        }
        return (p1 + d1 * s - p2 - d2 * t).sqrMagnitude;
    }
}

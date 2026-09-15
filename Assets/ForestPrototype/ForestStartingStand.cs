using System.Collections.Generic;
using UnityEngine;

// The canonical playable starting scenario: a conventional Irish Sitka
// plantation immediately before first thinning. Generated deterministically
// from a planting lattice at play start; the aggregated values here are
// scenario/calibration settings, not universal Sitka constants, and the
// density figures are validation anchors only - nothing in the simulation
// automatically thins toward a prescribed stocking.
//
// Structure: a planting lattice with a border margin; a fixed number of
// positions are omitted (failed establishment / early mortality) so the
// plantation reads as planted rather than computer-perfect. DBH classes come
// from local planting occupancy (dense neighbourhoods suppress), never from
// age: all crop trees share the canonical age. Competition, crowns, canopy,
// light and wind are then produced by the normal ecology systems.
public sealed class ForestStartingStand : MonoBehaviour
{
    [Header("Scenario: first-thinning Sitka plantation")]
    [SerializeField] private float standWidthMeters = 40f;
    [SerializeField] private float standDepthMeters = 40f;
    [SerializeField, Min(1)] private int canonicalAgeYears = 20;
    [SerializeField, Min(1)] private int targetTreeCount = 336;
    [Tooltip("20 x 20 planting lattice inside the stand borders - one slot more per axis than the 19 x 19 draft so the forest road and work-area clearances keep the living count at the 336-stem anchor.")]
    [SerializeField, Min(2)] private int latticePerAxis = 21;
    [Tooltip("[D] Spacing between planting rows/trees inside the lattice.")]
    [SerializeField, Min(0.5f)] private float latticeSpacingMeters = 1.9f;
    [Tooltip("[D] Mean DBH target at the canonical age (diagnostic anchor).")]
    [SerializeField, Min(5f)] private float meanDbhCm = 16f;
    [Tooltip("[D] DBH class bands: dominant / co-dominant, ordinary crop trees, suppressed.")]
    [SerializeField] private Vector2 dominantDbhCm = new Vector2(19f, 24f);
    [SerializeField] private Vector2 ordinaryDbhCm = new Vector2(15f, 19f);
    [SerializeField] private Vector2 suppressedDbhCm = new Vector2(8f, 14f);
    [Tooltip("[D] Neighbour counts (occupied planting slots within this radius) that adjust the vigour-field DBH: gap-edge slots get released, tight interiors get extra suppression.")]
    [SerializeField] private float neighbourScanRadiusMeters = 4.2f;
    [SerializeField] private int crowdedNeighbourThreshold = 11;
    [SerializeField] private int openNeighbourThreshold = 7;
    [Header("Clearings the plantation must not cover")]
    [Tooltip("[D] No-tree corridor half-width around every Dirt Path segment.")]
    [SerializeField, Min(0.5f)] private float pathCorridorRadiusMeters = 1.5f;
    [Tooltip("[D] No-tree radius around the work-area clearing, the player start and every built structure.")]
    [SerializeField, Min(1f)] private float clearingRadiusMeters = 2.2f;

    private void Awake()
    {
        // Fresh game only: a loaded save brings its own trees, and the old
        // serialized stand must have been removed from the scene by the
        // scenario switch.
        if (UnityEngine.Object.FindFirstObjectByType<ForestTree>() != null)
            return;
        Generate();
    }

    [ContextMenu("Regenerate starting stand (dev)")]
    public void Generate()
    {
        var spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        var ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        if (spawner == null || ecology == null)
        {
            Debug.LogError("ForestStartingStand needs a spawner and an ecology controller.", this);
            return;
        }

        int perAxis = Mathf.Max(2, latticePerAxis);
        int total = perAxis * perAxis;
        int toOmit = Mathf.Clamp(total - targetTreeCount, 0, total);
        float spacing = latticeSpacingMeters;
        float span = (perAxis - 1) * spacing;
        float originX = -span * 0.5f;
        float originZ = -span * 0.5f;

        // Clearance zones the plantation must respect: the forest road, the
        // work-area clearing, the player start and built structures. Path
        // planks get the road corridor radius; the rest get the clearing radius.
        var exclusionPoints = CollectClearancePoints();
        int clearanceOmitted = 0;

        // Slots with their final jittered world positions, so clearance is
        // tested against where the tree would actually stand, not the raw
        // lattice centre.
        var slots = new List<(int r, int c, Vector3 position, string id)>();
        for (int r = 0; r < perAxis; r++)
        for (int c = 0; c < perAxis; c++)
        {
            string id = $"P{r:D2}{c:D2}";
            Vector2 centre = new Vector2(originX + c * spacing, originZ + r * spacing);
            Vector2 jitter = FnvJitter(id);
            Vector3 position = new Vector3(
                Mathf.Clamp(centre.x + jitter.x, -(standWidthMeters * 0.5f - 0.6f), standWidthMeters * 0.5f - 0.6f),
                0f,
                Mathf.Clamp(centre.y + jitter.y, -(standDepthMeters * 0.5f - 0.6f), standDepthMeters * 0.5f - 0.6f));
            slots.Add((r, c, position, id));
        }

        // 1. Clearance omissions first (forest road, work area, structures);
        // they do not consume the mortality budget.
        var omitted = new HashSet<int>();
        for (int i = 0; i < slots.Count; i++)
            if (InClearanceZone(slots[i].position, exclusionPoints))
                omitted.Add(i);
        clearanceOmitted = omitted.Count;

        // 2. Mortality budget fills whatever omissions remain: the lowest-hash
        // non-clearance slots represent failed establishment / early mortality
        // while keeping the rows.
        int remainingOmit = Mathf.Max(0, toOmit - clearanceOmitted);
        if (remainingOmit > 0)
        {
            var byHash = new List<(int index, uint hash)>();
            for (int i = 0; i < slots.Count; i++)
                if (!omitted.Contains(i))
                    byHash.Add((i, FnvHash($"omit-{i}")));
            byHash.Sort((a, b) => a.hash.CompareTo(b.hash));
            for (int i = 0; i < Mathf.Min(remainingOmit, byHash.Count); i++)
                omitted.Add(byHash[i].index);
        }

        // 3. Occupancy grid first, so DBH classes see the real neighbourhoods.
        var occupied = new bool[perAxis, perAxis];
        for (int i = 0; i < slots.Count; i++)
            occupied[slots[i].r, slots[i].c] = !omitted.Contains(i);

        int spawned = 0;
        foreach (var slot in slots)
        {
            if (omitted.Contains(slot.r * perAxis + slot.c))
                continue;

            int neighbours = CountNeighbours(occupied, slot.r, slot.c, spacing, neighbourScanRadiusMeters);
            float dbh = DbhForClass(neighbours, slot.id, slot.position);
            float height = HeightForDbh(dbh, slot.id);
            var species = spawner.DefaultSpecies;
            float crown = species != null ? species.PotentialCrownRadiusM(dbh) : 1.6f;
            spawner.Spawn(slot.id, slot.position, canonicalAgeYears, dbh, height, crown);
            spawned++;
        }

        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
        Debug.Log($"STARTING_STAND: {spawned} stems planted ({spawned / (standWidthMeters * standDepthMeters / 10000f):F0} stems/ha), age {canonicalAgeYears}, clearance omissions {clearanceOmitted}, mortality omissions {Mathf.Max(0, toOmit - clearanceOmitted)}");
    }

    // Clearance anchors: the forest road planks, the work-area clearing, the
    // player start and every built structure. Trees never plant here.
    private List<KeyValuePair<Vector3, float>> CollectClearancePoints()
    {
        var points = new List<KeyValuePair<Vector3, float>>();
        foreach (var t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (t.name.StartsWith("Dirt Path"))
                points.Add(new KeyValuePair<Vector3, float>(t.position, pathCorridorRadiusMeters));
        }
        var clearing = GameObject.Find("Forest Clearing");
        if (clearing != null)
            points.Add(new KeyValuePair<Vector3, float>(clearing.transform.position, clearingRadiusMeters));
        var player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        if (player != null)
            points.Add(new KeyValuePair<Vector3, float>(player.transform.position, clearingRadiusMeters));
        foreach (var buildable in UnityEngine.Object.FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (buildable != null)
                points.Add(new KeyValuePair<Vector3, float>(buildable.transform.position, clearingRadiusMeters));
        return points;
    }

    private bool InClearanceZone(Vector3 worldPosition, List<KeyValuePair<Vector3, float>> exclusionPoints)
    {
        foreach (var pair in exclusionPoints)
            if (Vector2.Distance(new Vector2(worldPosition.x, worldPosition.z), new Vector2(pair.Key.x, pair.Key.z)) <= pair.Value)
                return true;
        return false;
    }

    // DBH from a smooth deterministic vigour field (so same-aged crop trees
    // vary widely through vigour and suppression, in spatial clusters that
    // the competition calculation can then confirm) plus a release bonus on
    // slots next to establishment gaps. Mean lands near the 16 cm anchor.
    private float DbhForClass(int neighbours, string id, Vector3 position)
    {
        float vigour = VigourAt(position);
        float dbh = Mathf.Lerp(suppressedDbhCm.x, dominantDbhCm.y, vigour);
        if (neighbours <= openNeighbourThreshold)
            dbh += 1.5f; // released by nearby establishment gaps
        else if (neighbours >= crowdedNeighbourThreshold)
            dbh -= 1.5f;
        uint hash = FnvHash($"dbh-{id}");
        float jitter = ((hash & 0xFFFF) / 65535f - 0.5f) * 1.4f;
        return Mathf.Clamp(dbh + jitter, suppressedDbhCm.x, dominantDbhCm.y);
    }

    // Low-frequency deterministic pattern in [0,1]: clusters of vigour and
    // suppression across the plantation, identical in every new game.
    private float VigourAt(Vector3 position)
    {
        float wave = Mathf.Sin(position.x * 0.41f + 1.3f) * Mathf.Sin(position.z * 0.33f + 0.7f);
        // Base 0.56 compensates the crowded-interior suppression so the stand
        // mean lands at the ~16 cm anchor.
        float vigour = 0.56f + 0.33f * wave;
        return Mathf.Clamp01(vigour);
    }

    private float HeightForDbh(float dbh, string id)
    {
        // 20-year crop anchor: ~12 m at 16 cm DBH; suppression narrows height
        // on top of the smaller DBH.
        uint hash = FnvHash($"h-{id}");
        float jitter = ((hash & 0xFF) / 255f - 0.5f) * 1.2f;
        return Mathf.Max(3.5f, 2f + 0.62f * dbh + jitter);
    }

    private int CountNeighbours(bool[,] occupied, int r, int c, float spacing, float radius)
    {
        int perAxis = occupied.GetLength(0);
        int count = 0;
        for (int dr = -3; dr <= 3; dr++)
        for (int dc = -3; dc <= 3; dc++)
        {
            if (dr == 0 && dc == 0)
                continue;
            int nr = r + dr, nc = c + dc;
            if (nr < 0 || nc < 0 || nr >= perAxis || nc >= perAxis || !occupied[nr, nc])
                continue;
            float distance = new Vector2(dr * spacing, dc * spacing).magnitude;
            if (distance <= radius)
                count++;
        }
        return count;
    }

    private static uint FnvHash(string text)
    {
        uint hash = 2166136261u;
        foreach (char ch in text)
        {
            hash ^= ch;
            hash *= 16777619u;
        }
        return hash;
    }

    private static Vector2 FnvJitter(string id)
    {
        uint hash = FnvHash($"j-{id}");
        float dx = ((hash & 0xFF) / 255f - 0.5f) * 0.9f;
        hash *= 16777619u;
        float dz = (((hash >> 8) & 0xFF) / 255f - 0.5f) * 0.9f;
        return new Vector2(dx, dz);
    }
}

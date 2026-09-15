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
    [Tooltip("19 x 19 planting lattice inside the stand borders.")]
    [SerializeField, Min(2)] private int latticePerAxis = 19;
    [Tooltip("[D] Spacing between planting rows/trees inside the lattice.")]
    [SerializeField, Min(0.5f)] private float latticeSpacingMeters = 2f;
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

        // Deterministic omissions: the lowest-hash lattice positions represent
        // failed establishment or early mortality while keeping the rows.
        var omitted = new HashSet<int>();
        if (toOmit > 0)
        {
            var byHash = new List<(int index, uint hash)>();
            for (int i = 0; i < total; i++)
                byHash.Add((i, FnvHash($"omit-{i}")));
            byHash.Sort((a, b) => a.hash.CompareTo(b.hash));
            for (int i = 0; i < toOmit; i++)
                omitted.Add(byHash[i].index);
        }

        // Occupancy grid first, so DBH classes see the real neighbourhoods.
        var occupied = new bool[perAxis, perAxis];
        for (int r = 0; r < perAxis; r++)
        for (int c = 0; c < perAxis; c++)
            occupied[r, c] = !omitted.Contains(r * perAxis + c);

        int spawned = 0;
        for (int r = 0; r < perAxis; r++)
        for (int c = 0; c < perAxis; c++)
        {
            if (!occupied[r, c])
                continue;
            string id = $"P{r:D2}{c:D2}";
            Vector2 centre = new Vector2(originX + c * spacing, originZ + r * spacing);
            // Small deterministic planting jitter - keeps visible rows without
            // looking surveyor-perfect.
            Vector2 jitter = FnvJitter(id);
            Vector3 position = new Vector3(
                Mathf.Clamp(centre.x + jitter.x, -(standWidthMeters * 0.5f - 0.6f), standWidthMeters * 0.5f - 0.6f),
                0f,
                Mathf.Clamp(centre.y + jitter.y, -(standDepthMeters * 0.5f - 0.6f), standDepthMeters * 0.5f - 0.6f));

            int neighbours = CountNeighbours(occupied, r, c, spacing, neighbourScanRadiusMeters);
            float dbh = DbhForClass(neighbours, id, position);
            float height = HeightForDbh(dbh, id);
            var species = spawner.DefaultSpecies;
            float crown = species != null ? species.PotentialCrownRadiusM(dbh) : 1.6f;
            spawner.Spawn(id, position, canonicalAgeYears, dbh, height, crown);
            spawned++;
        }

        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
        Debug.Log($"STARTING_STAND: {spawned} stems planted ({spawned / (standWidthMeters * standDepthMeters / 10000f):F0} stems/ha), age {canonicalAgeYears}");
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

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
        // A fresh game starts in the geometry the new-game policy names (Enlarged80; Legacy40 only for
        // verification replays). A loaded save brings its own geometry through ForestSaveController.LoadData.
        var ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology != null)
            ecology.ApplyStandGeometry(StandGeometryPolicy.NewGameModelForSession);
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
        if (ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80)
        {
            GenerateEnlarged80(spawner, ecology);
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
            spawner.Spawn(slot.id, spawner.DefaultSpecies, slot.position, canonicalAgeYears, dbh, height, crown);
            spawned++;
        }

        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
        Debug.Log($"STARTING_STAND: {spawned} stems planted ({spawned / (standWidthMeters * standDepthMeters / 10000f):F0} stems/ha), age {canonicalAgeYears}, clearance omissions {clearanceOmitted}, mortality omissions {Mathf.Max(0, toOmit - clearanceOmitted)}");
    }


    // Enlarged80 (D-056). The current 40 x 40 m plantation is kept as the central core with the SAME tree IDs,
    // positions and omission pattern (generated by exactly the Legacy40 rules); the outer plantation is added
    // around it on the same lattice spacing at the same stocking (about 2,100 stems/ha: 336 core + 1,008 outer =
    // 1,344 living trees on 0.64 ha). Outer IDs are "PO" + row + column of the enlarged lattice: they keep the "P"
    // prefix the objectives use to recognise original plantation trees, and are six characters long where
    // every legacy ID ("P0707") is five, so they can never collide. Omissions are deterministic hash rankings.
    // Neighbourhood-sensitive starting DBH is evaluated against the FULL enlarged occupancy, so the former
    // +-20 m property edge leaves no artificial ring of released trees inside the stand: positions and identity
    // are preserved, obsolete edge biology is not.
    private void GenerateEnlarged80(ForestTreeSpawner spawner, ForestEcologyController ecology)
    {
        int corePerAxis = Mathf.Max(2, latticePerAxis);
        int perAxis = 2 * corePerAxis - 1;
        int offset = corePerAxis / 2;
        float spacing = latticeSpacingMeters;
        // The Legacy40 generator clamps every core position to +-coreLimit (the plantation margin inside the old
        // property edge), so the core's outermost lattice lines sit ON that line. The outer plantation continues
        // from it at the same spacing, which keeps the planting spacing continuous across the former boundary and
        // ends the outer lattice exactly on the new property's margin.
        float coreLimit = standWidthMeters * 0.5f - 0.6f;
        float outerLimit = StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) * 0.5f - 0.6f;
        var exclusionPoints = CollectClearancePoints();

        // ---- Core: the Legacy40 generation, step for step ---------------------------------------------
        int coreTotal = corePerAxis * corePerAxis;
        int coreToOmit = Mathf.Clamp(coreTotal - targetTreeCount, 0, coreTotal);
        float coreSpan = (corePerAxis - 1) * spacing;
        float coreOriginX = -coreSpan * 0.5f;
        float coreOriginZ = -coreSpan * 0.5f;
        var coreSlots = new List<(int r, int c, Vector3 position, string id)>();
        for (int r = 0; r < corePerAxis; r++)
        for (int c = 0; c < corePerAxis; c++)
        {
            string id = $"P{r:D2}{c:D2}";
            Vector2 centre = new Vector2(coreOriginX + c * spacing, coreOriginZ + r * spacing);
            Vector2 jitter = FnvJitter(id);
            Vector3 position = new Vector3(
                Mathf.Clamp(centre.x + jitter.x, -(standWidthMeters * 0.5f - 0.6f), standWidthMeters * 0.5f - 0.6f),
                0f,
                Mathf.Clamp(centre.y + jitter.y, -(standDepthMeters * 0.5f - 0.6f), standDepthMeters * 0.5f - 0.6f));
            coreSlots.Add((r, c, position, id));
        }
        var coreOmitted = new HashSet<int>();
        for (int i = 0; i < coreSlots.Count; i++)
            if (InClearanceZone(coreSlots[i].position, exclusionPoints))
                coreOmitted.Add(i);
        int coreClearanceOmitted = coreOmitted.Count;
        int coreRemainingOmit = Mathf.Max(0, coreToOmit - coreClearanceOmitted);
        if (coreRemainingOmit > 0)
        {
            var byHash = new List<(int index, uint hash)>();
            for (int i = 0; i < coreSlots.Count; i++)
                if (!coreOmitted.Contains(i))
                    byHash.Add((i, FnvHash($"omit-{i}")));
            byHash.Sort((a, b) => a.hash.CompareTo(b.hash));
            for (int i = 0; i < Mathf.Min(coreRemainingOmit, byHash.Count); i++)
                coreOmitted.Add(byHash[i].index);
        }

        // ---- Outer plantation: every lattice slot outside the core block -------------------------------
        int target = StandGeometryModel.StartingTreeTarget(StandGeometryModel.Enlarged80);
        int coreLiving = coreSlots.Count - coreOmitted.Count;
        var outerSlots = new List<(int r, int c, Vector3 position, string id)>();
        for (int r = 0; r < perAxis; r++)
        for (int c = 0; c < perAxis; c++)
        {
            bool inCore = r >= offset && r < offset + corePerAxis && c >= offset && c < offset + corePerAxis;
            if (inCore)
                continue;
            string id = $"PO{r:D2}{c:D2}";
            Vector2 jitter = FnvJitter(id);
            Vector3 position = new Vector3(
                OuterAxis(c, jitter.x, offset, corePerAxis, spacing, coreOriginX, coreLimit, outerLimit), 0f,
                OuterAxis(r, jitter.y, offset, corePerAxis, spacing, coreOriginZ, coreLimit, outerLimit));
            outerSlots.Add((r, c, position, id));
        }
        var outerOmitted = new HashSet<int>();
        for (int i = 0; i < outerSlots.Count; i++)
            if (InClearanceZone(outerSlots[i].position, exclusionPoints))
                outerOmitted.Add(i);
        int outerClearanceOmitted = outerOmitted.Count;
        int outerToOmit = Mathf.Clamp(outerSlots.Count - Mathf.Max(0, target - coreLiving), 0, outerSlots.Count);
        int outerRemainingOmit = Mathf.Max(0, outerToOmit - outerClearanceOmitted);
        if (outerRemainingOmit > 0)
        {
            var byHash = new List<(int index, uint hash)>();
            for (int i = 0; i < outerSlots.Count; i++)
                if (!outerOmitted.Contains(i))
                    byHash.Add((i, Mix(FnvHash("omit-" + outerSlots[i].id))));
            byHash.Sort((a, b) => a.hash.CompareTo(b.hash));
            for (int i = 0; i < Mathf.Min(outerRemainingOmit, byHash.Count); i++)
                outerOmitted.Add(byHash[i].index);
        }

        // ---- Full-stand occupancy, then starting state for every living tree --------------------------
        var occupied = new bool[perAxis, perAxis];
        for (int i = 0; i < coreSlots.Count; i++)
            occupied[coreSlots[i].r + offset, coreSlots[i].c + offset] = !coreOmitted.Contains(i);
        for (int i = 0; i < outerSlots.Count; i++)
            occupied[outerSlots[i].r, outerSlots[i].c] = !outerOmitted.Contains(i);

        int spawned = 0;
        var species = spawner.DefaultSpecies;
        for (int i = 0; i < coreSlots.Count; i++)
        {
            if (coreOmitted.Contains(i))
                continue;
            var slot = coreSlots[i];
            SpawnPlanted(spawner, species, slot.id, slot.position, CountNeighbours(occupied, slot.r + offset, slot.c + offset, spacing, neighbourScanRadiusMeters));
            spawned++;
        }
        int outerSpawned = 0;
        for (int i = 0; i < outerSlots.Count; i++)
        {
            if (outerOmitted.Contains(i))
                continue;
            var slot = outerSlots[i];
            SpawnPlanted(spawner, species, slot.id, slot.position, CountNeighbours(occupied, slot.r, slot.c, spacing, neighbourScanRadiusMeters));
            spawned++;
            outerSpawned++;
        }

        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
        Debug.Log($"STARTING_STAND: {spawned} stems planted ({spawned / ecology.StandAreaHectares:F0} stems/ha), age {canonicalAgeYears}, Enlarged80 core {spawned - outerSpawned} + outer {outerSpawned}, core clearance omissions {coreClearanceOmitted}, core mortality omissions {coreRemainingOmit}, outer clearance omissions {outerClearanceOmitted}, outer mortality omissions {outerRemainingOmit}");
    }

    // Position along one axis of an outer-plantation slot, from its lattice line index (0..2*corePerAxis-2).
    // On an axis where the slot lies within the core block's range (the north/south/east/west bands) it uses the
    // core's own lattice line and legacy clamp, so those rows line up exactly with the core's rows and columns.
    // Beyond the core it continues outwards from the core's clamped outermost line at the lattice spacing.
    private static float OuterAxis(int index, float jitter, int offset, int corePerAxis, float spacing, float coreOrigin, float coreLimit, float outerLimit)
    {
        if (index >= offset && index < offset + corePerAxis)
            return Mathf.Clamp(coreOrigin + (index - offset) * spacing + jitter, -coreLimit, coreLimit);
        float line = index < offset
            ? -coreLimit - (offset - index) * spacing
            : coreLimit + (index - (offset + corePerAxis - 1)) * spacing;
        return Mathf.Clamp(line + jitter, -outerLimit, outerLimit);
    }

    private void SpawnPlanted(ForestTreeSpawner spawner, TreeSpeciesDefinition species, string id, Vector3 position, int neighbours)
    {
        float dbh = DbhForClass(neighbours, id, position);
        float height = HeightForDbh(dbh, id);
        float crown = species != null ? species.PotentialCrownRadiusM(dbh) : 1.6f;
        spawner.Spawn(id, species, position, canonicalAgeYears, dbh, height, crown);
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

    // FNV-1a alone clusters for IDs that share a long prefix ("PO31xx"), which would remove whole lattice rows
    // together and leave visible density bands. The outer plantation ranks its omissions on this mixed hash
    // (murmur3 finaliser) so they are deterministic AND spatially uniform. The Legacy40 core keeps the plain
    // FnvHash ranking exactly, to preserve its established omission pattern.
    private static uint Mix(uint hash)
    {
        hash ^= hash >> 16; hash *= 0x85ebca6bu;
        hash ^= hash >> 13; hash *= 0xc2b2ae35u;
        hash ^= hash >> 16;
        return hash;
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

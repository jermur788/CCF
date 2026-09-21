using System.Collections.Generic;
using UnityEngine;

public sealed class ForestEcologyController : MonoBehaviour
{
    [SerializeField] private int ecologicalYear;
    [SerializeField] private int simulationSeed = 20260914;
    [SerializeField] private float standSizeMeters = 40f;
    [SerializeField] private float cellSizeMeters = 5f;
    [Tooltip("[D] Maximum recorded recent opening per cell. A treatment that fells several trees in one cell counts once up to this cap, so a legitimate group opening stays serious without reading as catastrophic. Calibration from the spatial treatment experiments.")]
    [SerializeField, Min(1f)] private float maxRecentOpeningPerCell = 2f;
    [Tooltip("[D] Wind-risk band thresholds for the inspection card: low below the first, moderate below the second, high at or above it. Calibrated so an unthinned control stand (max ~7.8-8.6) reads moderate and a concentrated opening reads high.")]
    [SerializeField] private float windLabelLowAt = 5f;
    [SerializeField] private float windLabelHighAt = 12f;
    [SerializeField] private bool showDebugGrid;
    [SerializeField] private bool showSeedRain;
    [SerializeField] private bool logAnnualSummary;
    [SerializeField] private int debugCellIndex;
    [Tooltip("Time-lapse: while enabled, one ecological year passes every Seconds Per Year of real time. Toggled in-game with T, which cycles 30 -> 10 -> 2.5 s/year -> off. Uses the same deterministic annual step as everything else.")]
    [SerializeField] private bool timeLapseEnabled;
    [SerializeField, Min(0.5f)] private float timeLapseSecondsPerYear = 30f;
    [Tooltip("Optional visual-only seedling shown for the default species while its regeneration cohort grows (scaled by cohort height, removed on promotion). Pure display: the cohort state stays authoritative.")]
    [SerializeField] private GameObject seedlingVisualPrefab;
    private readonly Dictionary<int, GameObject> seedlingVisuals = new Dictionary<int, GameObject>();
    private bool seedlingVisualsDirty = true;
    private float timeLapseAccumulator;

    private ForestEcologyCell[] cells;
    private int cellsPerAxis;
    private TreeSpeciesDefinition species;
    private readonly Dictionary<ForestTree, float> competitionIndex = new Dictionary<ForestTree, float>();
    private readonly Dictionary<ForestTree, float> annualDbhGrowth = new Dictionary<ForestTree, float>();
    private readonly Dictionary<ForestTree, float> seedPotential = new Dictionary<ForestTree, float>();
    private sealed class MastState
    {
        public string Label = "normal";
        public float Multiplier = 1f;
    }
    private readonly Dictionary<string, MastState> mastBySpeciesId = new Dictionary<string, MastState>();
    private bool competitionCurrent;
    private string lastMastLabel = "normal";
    private float lastMastMultiplier = 1f;
    private GUIStyle timeLapseStyle;

    public int EcologicalYear => ecologicalYear;
    public int CellCount => cells != null ? cells.Length : 0;
    public int CellsPerAxis => cellsPerAxis;
    public float CellSizeMeters => cellSizeMeters;
    public ForestEcologyCell[] Cells => cells;
    public string LastMastLabel => lastMastLabel;
    public float LastMastMultiplier => lastMastMultiplier;
    public float MaxRecentOpeningPerCell => maxRecentOpeningPerCell;

    public string GetMastLabel(TreeSpeciesDefinition targetSpecies)
    {
        if (targetSpecies != null && mastBySpeciesId.TryGetValue(targetSpecies.SpeciesId, out MastState state))
            return state.Label;
        return "normal";
    }

    public float GetMastMultiplier(TreeSpeciesDefinition targetSpecies)
    {
        if (targetSpecies != null && mastBySpeciesId.TryGetValue(targetSpecies.SpeciesId, out MastState state))
            return state.Multiplier;
        return targetSpecies != null ? targetSpecies.MastNormalMultiplier : 1f;
    }

    public int SimulationSeed
    {
        get => simulationSeed;
        set => simulationSeed = value;
    }

    public bool ShowDebugGrid
    {
        get => showDebugGrid;
        set => showDebugGrid = value;
    }

    public bool ShowSeedRain
    {
        get => showSeedRain;
        set => showSeedRain = value;
    }

    public bool LogAnnualSummary
    {
        get => logAnnualSummary;
        set => logAnnualSummary = value;
    }

    public int DebugCellIndex
    {
        get => debugCellIndex;
        set => debugCellIndex = value;
    }

    private void Awake()
    {
        RebuildGrid();
    }

    private void Update()
    {
        if (seedlingVisualsDirty)
        {
            seedlingVisualsDirty = false;
            SyncSeedlingVisuals();
        }

        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null && keyboard.tKey.wasPressedThisFrame)
        {
            // T cycles: off -> 30 s/yr -> 10 s/yr -> 2.5 s/yr -> off, so the
            // multi-decade arc is watchable inside a minute or two.
            if (!timeLapseEnabled)
            {
                timeLapseEnabled = true;
                timeLapseSecondsPerYear = 30f;
            }
            else if (timeLapseSecondsPerYear > 10.5f)
                timeLapseSecondsPerYear = 10f;
            else if (timeLapseSecondsPerYear > 2.5f)
                timeLapseSecondsPerYear = 2.5f;
            else
                timeLapseEnabled = false;
            timeLapseAccumulator = 0f;
        }
        if (!timeLapseEnabled)
            return;
        timeLapseAccumulator += Time.deltaTime;
        while (timeLapseAccumulator >= timeLapseSecondsPerYear)
        {
            timeLapseAccumulator -= timeLapseSecondsPerYear;
            AdvanceOneYear();
        }
    }

    // Visual-only: one default-species seedling per regenerating cell, scaled to
    // the cohort's authoritative height. Never affects simulation state.
    private void SyncSeedlingVisuals()
    {
        if (cells == null)
            return;
        if (seedlingVisualPrefab == null)
            return;
        TreeSpeciesDefinition defaultSpecies = ResolveSpecies();
        string defaultSpeciesId = defaultSpecies != null ? defaultSpecies.SpeciesId : "";
        for (int i = 0; i < cells.Length; i++)
        {
            ForestRegenerationCohort cohort = cells[i] != null ? cells[i].FindCohort(defaultSpeciesId) : null;
            bool wanted = cohort != null && cohort.Density > 0f && !Mathf.Approximately(cohort.Height, 0f);
            seedlingVisuals.TryGetValue(i, out GameObject visual);
            if (wanted && visual == null)
            {
                Vector3 jitter = CellVisualJitter(i);
                Vector3 position = new Vector3(cells[i].Center.x + jitter.x, 0f, cells[i].Center.y + jitter.z);
                visual = Instantiate(seedlingVisualPrefab, position, Quaternion.Euler(0f, jitter.y, 0f), transform);
                visual.name = $"Seedling cell {i}";
                seedlingVisuals[i] = visual;
            }
            else if (!wanted && visual != null)
            {
                seedlingVisuals.Remove(i);
                if (visual != null)
                    Destroy(visual);
                continue;
            }
            if (wanted && visual != null)
            {
                float scale = Mathf.Clamp(cohort.Height, 0.15f, 3f);
                visual.transform.localScale = Vector3.one * scale;
                // The seedling sits on the ground regardless of parent scale.
                visual.transform.position = new Vector3(visual.transform.position.x, 0f, visual.transform.position.z);
            }
        }
    }

    // Stable per-cell offset so the representative seedling does not jump
    // between sessions, derived from the cell index only.
    private Vector3 CellVisualJitter(int index)
    {
        uint hash = 2166136261u ^ (uint)index;
        hash *= 16777619u;
        float dx = ((hash & 0xFF) / 255f - 0.5f) * (cellSizeMeters * 0.5f);
        hash *= 16777619u;
        float dz = ((hash & 0xFF) / 255f - 0.5f) * (cellSizeMeters * 0.5f);
        return new Vector3(dx, 0f, dz);
    }

    private void OnEnable()
    {
        ForestTree.Felled += OnTreeFelled;
    }

    private void OnDisable()
    {
        ForestTree.Felled -= OnTreeFelled;
    }

    private void OnTreeFelled(ForestTree tree)
    {
        if (cells != null && tree != null)
        {
            int index = GetCellIndex(tree.transform.position);
            if (index >= 0)
                cells[index].RecentOpening = Mathf.Min(cells[index].RecentOpening + 1f, maxRecentOpeningPerCell);
        }
        competitionCurrent = false;
        RecomputeCanopy();
        RecomputeSeedRain();
        seedlingVisualsDirty = true;
    }

    // One explicit step for editor, MCP and debug tooling. Nothing in normal
    // gameplay advances ecological time yet; one ecological year is not tied
    // to a game day or real-time minute.
    [ContextMenu("Advance one ecological year")]
    public void AdvanceOneYear()
    {
        TreeSpeciesDefinition s = ResolveSpecies();
        if (s == null || cells == null)
        {
            Debug.LogWarning("ForestEcologyController cannot advance a year without a species and a built grid.", this);
            return;
        }

        ecologicalYear++;
        var rng = new System.Random(unchecked(simulationSeed * 397) ^ ecologicalYear);

        // Annual order follows the Sitka report's sequence.
        UpdateCompetition(s);             // 1. competition from current neighbours
        competitionCurrent = true;
        RecomputeCanopy();                // 2. canopy/light from current crowns
        GrowAdults(s);                    // 3. adult DBH and height
        RelaxCrowns(s);                   // 4. crown relaxation toward competition-limited target
        RecomputeCanopy();                // 5. light reflects the new crowns
        GrowExistingRegeneration();       // 6. existing regeneration grows before new establishment
        UpdateAllMastStates(s, rng);      // 7. species-isolated mast state for this year
        ComputeSeedRain(s);               // 8. spatial seed dispersal (RecomputeSeedRain core)
        EstablishNewCohorts();            // 9. new establishment from seed x light x suitability
        PromoteCohorts(s, rng);           // 10. cohorts that reach tree size become individuals
        UpdateEstablishmentSuitability(); // 11. simple disturbance response
        DecayRecentOpening(s);            // 12. exposure decays with time
        LogSummary(s);                    // 13. diagnostics
        seedlingVisualsDirty = true;
    }

    public void RestoreEcologyState(int year, int seed)
    {
        ecologicalYear = Mathf.Max(0, year);
        simulationSeed = seed;
        competitionCurrent = false;
        timeLapseAccumulator = 0f;
        // Cells not present in the save must return to a clean state, or a second
        // load in the same session would inherit later regeneration.
        if (cells != null)
        {
            foreach (ForestEcologyCell cell in cells)
            {
                cell.ClearRegeneration();
                cell.RecentOpening = 0f;
                // Cells not present in the save return to fresh-grid semantics,
                // including the smoothed establishment suitability.
                cell.EstablishmentSuitability = 1f;
            }
        }
        RefreshMastForCurrentYear();
        ClearSeedlingVisuals();
        seedlingVisualsDirty = true;
    }

    // Full deterministic reset for test fixtures and replays: every piece of
    // state the annual update consumes returns to its fresh-play values.
    // Loaded saves use the narrower RestoreEcologyState path; this method is
    // the one that makes ApplyLifecycleFixture a complete reset.
    public void ResetForDeterministicRun()
    {
        ecologicalYear = 0;
        timeLapseAccumulator = 0f;
        competitionCurrent = false;
        competitionIndex.Clear();
        annualDbhGrowth.Clear();
        seedPotential.Clear();
        mastBySpeciesId.Clear();
        RebuildGrid();
        lastMastLabel = "normal";
        lastMastMultiplier = 1f;
        RefreshMastForCurrentYear();
    }

    // Competition is computed during the annual update; on demand (inspection)
    // it is computed lazily so the card is correct even before the first year.
    public void InvalidateCompetition()
    {
        competitionCurrent = false;
    }

    public void RestoreCellState(int index, float density, float height, int establishYear, float recentOpening, float establishmentSuitability = -1f)
    {
        if (cells == null || index < 0 || index >= cells.Length)
            return;
        ForestEcologyCell cell = cells[index];
        TreeSpeciesDefinition defaultSpecies = ResolveSpecies();
        if (defaultSpecies != null && (density > 0f || establishYear >= 0))
            cell.GetOrCreateCohort(defaultSpecies).Restore(density, height, establishYear);
        RestoreCellEnvironment(cell, recentOpening, establishmentSuitability);
        seedlingVisualsDirty = true;
    }

    public void RestoreCellState(int index, List<ForestRegenerationCohortSaveData> savedCohorts, float recentOpening, float establishmentSuitability = -1f)
    {
        if (cells == null || index < 0 || index >= cells.Length)
            return;
        ForestEcologyCell cell = cells[index];
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (savedCohorts != null && spawner != null)
        {
            savedCohorts.Sort((a, b) => string.CompareOrdinal(a != null ? a.speciesId : "", b != null ? b.speciesId : ""));
            foreach (ForestRegenerationCohortSaveData saved in savedCohorts)
            {
                if (saved == null || saved.density <= 0f)
                    continue;
                TreeSpeciesDefinition cohortSpecies = spawner.ResolveSpecies(saved.speciesId);
                if (cohortSpecies == null)
                {
                    Debug.LogWarning($"Unknown regeneration species '{saved.speciesId}' in cell {index}; cohort skipped.");
                    continue;
                }
                cell.GetOrCreateCohort(cohortSpecies).Restore(saved.density, saved.height, saved.establishYear);
            }
        }
        RestoreCellEnvironment(cell, recentOpening, establishmentSuitability);
        seedlingVisualsDirty = true;
    }

    private void RestoreCellEnvironment(ForestEcologyCell cell, float recentOpening, float establishmentSuitability)
    {
        cell.RecentOpening = Mathf.Clamp(recentOpening, 0f, maxRecentOpeningPerCell);
        // Suitability is a smoothed disturbance response: genuine short history,
        // so new saves persist it. Older saves reconstruct it deterministically
        // from the restored opening instead of inheriting the live world.
        cell.EstablishmentSuitability = establishmentSuitability >= 0f
            ? Mathf.Clamp01(establishmentSuitability)
            : Mathf.Clamp01(1f - 0.3f * cell.RecentOpening);
    }

    // Deterministically reproduces the mast roll for the current seed and year.
    public void RefreshMastForCurrentYear()
    {
        TreeSpeciesDefinition s = ResolveSpecies();
        if (s == null)
            return;
        var rng = new System.Random(unchecked(simulationSeed * 397) ^ ecologicalYear);
        UpdateAllMastStates(s, rng);
    }

    [ContextMenu("Rebuild ecology grid")]
    public void RebuildGrid()    {
        cellsPerAxis = Mathf.Max(1, Mathf.CeilToInt(standSizeMeters / cellSizeMeters));
        cells = new ForestEcologyCell[cellsPerAxis * cellsPerAxis];
        float origin = -standSizeMeters * 0.5f;
        for (int z = 0; z < cellsPerAxis; z++)
        for (int x = 0; x < cellsPerAxis; x++)
        {
            float centerX = origin + (x + 0.5f) * cellSizeMeters;
            float centerZ = origin + (z + 0.5f) * cellSizeMeters;
            cells[z * cellsPerAxis + x] = new ForestEcologyCell
            {
                Center = new Vector2(centerX, centerZ),
                Canopy = 1f,
                Light = 0f
            };
        }
        RecomputeCanopy();
        RecomputeSeedRain();
        ClearSeedlingVisuals();
        seedlingVisualsDirty = true;
    }

    // Visual-only: drop every seedling indicator (grid rebuild or save load).
    private void ClearSeedlingVisuals()
    {
        foreach (var visual in seedlingVisuals.Values)
            if (visual != null)
                Destroy(visual);
        seedlingVisuals.Clear();
    }

    // Simplified local crown influence: each living crown shades a cell by a
    // lateral falloff from its authoritative position, crown radius and height,
    // combined as fractional cover (1 - product of gaps). No global percentage.
    [ContextMenu("Recompute canopy and light")]
    public void RecomputeCanopy()
    {
        if (cells == null)
            return;

        ForestTree[] trees = FindTrees();
        foreach (ForestEcologyCell cell in cells)
        {
            float gap = 1f;
            foreach (ForestTree tree in trees)
            {
                if (tree == null || tree.IsStump)
                    continue;
                Vector2 treePosition = new Vector2(tree.transform.position.x, tree.transform.position.z);
                float distance = Vector2.Distance(cell.Center, treePosition);
                // Crown influence reaches half a cell beyond the crown so that
                // cells overlapping the crown respond, not only its centre.
                float reach = tree.CrownRadius * 1.5f + cellSizeMeters * 0.5f;
                float lateral = Mathf.Clamp01(1f - distance / reach);
                float vertical = Mathf.Clamp01(tree.Height / 8f);
                float influence = Mathf.Clamp01(lateral * vertical);
                gap *= 1f - influence;
            }
            float canopy = Mathf.Clamp01(1f - gap);
            cell.Canopy = canopy;
            cell.Light = 1f - canopy;
        }
    }

    [ContextMenu("Recompute seed rain")]
    public void RecomputeSeedRain()
    {
        TreeSpeciesDefinition s = ResolveSpecies();
        if (s == null || cells == null)
            return;
        ComputeSeedRain(s);
    }

    private static int CompareTreeIds(ForestTree a, ForestTree b)
    {
        return string.CompareOrdinal(a != null ? a.TreeId : "", b != null ? b.TreeId : "");
    }

    // Scene enumeration order is not guaranteed between sessions (and the
    // comments elsewhere already note it). Every annual update consumes trees
    // for accumulation - Hegyi competition sums, canopy gap products, growth,
    // crown relaxation and seed rain - and floating-point addition is not
    // associative, so the order must be stable. Ids are unique, so an ordinal
    // sort gives one deterministic snapshot per call; the cost at stand scale
    // is negligible against the value of reproducible runs.
    private ForestTree[] FindTrees()
    {
        ForestTree[] trees = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        System.Array.Sort(trees, CompareTreeIds);
        return trees;
    }

    public TreeSpeciesDefinition ResolveSpecies()
    {
        if (species != null)
            return species;
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (spawner != null && spawner.DefaultSpecies != null)
        {
            species = spawner.DefaultSpecies;
            return species;
        }
        foreach (ForestTree tree in FindTrees())
        {
            if (tree.Species != null)
            {
                species = tree.Species;
                return species;
            }
        }
        return null;
    }

    // Hegyi competition: CI = sum over nearby living neighbours of
    // (DBH_neighbour / DBH_target) / distance_m. Local positions only.
    private void UpdateCompetition(TreeSpeciesDefinition s)
    {
        competitionIndex.Clear();
        annualDbhGrowth.Clear();
        ForestTree[] trees = FindTrees();
        const float cutoffMeters = 20f; // [C] performance abstraction

        // Spatial pass v1: the pair loop used to fetch every neighbour's
        // transform.position from native code once per pair (~113k interop
        // calls a year at first-thinning stocking - the dominant profile
        // cost) and the 20 m cutoff pruned only ~1 pair in 5 inside this
        // 40 m stand, so a bucket grid cannot prune much at this scale.
        // Caching each tree's position and diameter once - in the same
        // sorted order the loops already consume - and early-outing on the
        // squared distance keeps every included pair, every operand and the
        // summation order identical, so competition indices stay bit-exact.
        int count = trees.Length;
        var positions = new Vector2[count];
        var diameters = new float[count];
        var alive = new bool[count];
        for (int i = 0; i < count; i++)
        {
            ForestTree tree = trees[i];
            if (tree == null || tree.IsStump)
                continue;
            Vector3 p = tree.transform.position;
            positions[i] = new Vector2(p.x, p.z);
            diameters[i] = tree.Diameter;
            alive[i] = true;
        }
        float cutoffSquared = cutoffMeters * cutoffMeters;

        for (int i = 0; i < count; i++)
        {
            if (!alive[i])
                continue;
            ForestTree target = trees[i];
            Vector2 targetPos = positions[i];
            float targetDiameter = diameters[i];
            float ci = 0f;
            for (int j = 0; j < count; j++)
            {
                if (j == i || !alive[j])
                    continue;
                float dx = positions[j].x - targetPos.x;
                float dz = positions[j].y - targetPos.y;
                float squared = dx * dx + dz * dz;
                // Slack band: the prune must never reject a pair the original
                // Vector2.Distance check accepted, because sqrt can round a
                // squared distance just above the cutoff down to the cutoff
                // itself. Borderline pairs fall through to the exact call, so
                // the final pair set and every summation operand stay
                // identical to the linear scan.
                if (squared > cutoffSquared * 1.00002f)
                    continue;
                float distance = Vector2.Distance(targetPos, positions[j]);
                if (distance > cutoffMeters)
                    continue;
                ci += (diameters[j] / Mathf.Max(1f, targetDiameter)) / Mathf.Max(0.5f, distance);
            }
            competitionIndex[target] = ci;
        }
        competitionCurrent = true;
    }

    // DBH responds to competition; height follows age/site and is deliberately
    // not multiplied by the same competition factor (thinning affects girth more).
    private void GrowAdults(TreeSpeciesDefinition s)
    {
        int yearsApplied = 1;
        ForestTree[] trees = FindTrees();
        foreach (ForestTree tree in trees)
        {
            if (tree == null || tree.IsStump)
                continue;
            TreeSpeciesDefinition treeSpecies = tree.Species != null ? tree.Species : s;
            float site = GetSiteProductivity(tree.transform.position);
            float ci = competitionIndex.TryGetValue(tree, out float value) ? value : 0f;

            float dbhPotential = treeSpecies.PotentialDbhGrowthCmPerYear * site *
                                 Mathf.Clamp01(1f - tree.Diameter / treeSpecies.MaxDbhCm);
            float dbhGrowth = dbhPotential * (1f / (1f + ci / treeSpecies.Ci50));
            float heightGrowth = treeSpecies.PotentialHeightGrowthMPerYear * site *
                                 Mathf.Clamp01(1f - tree.Height / treeSpecies.MaxHeightM);

            // Diagnostic integration of the existing DBH competition response.
            // Never feed this accumulated history back into growth in v1.
            tree.RecordSuppressionYear(1f - (1f / (1f + ci / treeSpecies.Ci50)));
            tree.ApplyGrowth(dbhGrowth * yearsApplied, heightGrowth * yearsApplied);
            tree.SetAgeYears(tree.AgeYears + yearsApplied);
            annualDbhGrowth[tree] = dbhGrowth;
        }
    }

    // Dynamic crown radius: potential from DBH, reduced by competition, reached
    // gradually by relaxation rather than snapping.
    private void RelaxCrowns(TreeSpeciesDefinition s)
    {
        ForestTree[] trees = FindTrees();
        foreach (ForestTree tree in trees)
        {
            if (tree == null || tree.IsStump)
                continue;
            TreeSpeciesDefinition treeSpecies = tree.Species != null ? tree.Species : s;
            float potential = treeSpecies.PotentialCrownRadiusM(tree.Diameter);
            float ci = competitionIndex.TryGetValue(tree, out float value) ? value : 0f;
            float factor = 1f / (1f + ci / treeSpecies.Ci50);
            float target = potential * factor;
            tree.RelaxCrownRadius(target, treeSpecies.CrownRelaxationPerYear);
        }
    }

    private List<TreeSpeciesDefinition> RegenerationSpecies(TreeSpeciesDefinition defaultSpecies)
    {
        var enabled = new List<TreeSpeciesDefinition>();
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (spawner != null)
            foreach (TreeSpeciesDefinition candidate in spawner.KnownSpecies)
                if (candidate != null && candidate.SupportsRegeneration)
                    enabled.Add(candidate);
        if (defaultSpecies != null && defaultSpecies.SupportsRegeneration &&
            !enabled.Exists(candidate => string.Equals(candidate.SpeciesId, defaultSpecies.SpeciesId, System.StringComparison.Ordinal)))
            enabled.Add(defaultSpecies);
        enabled.Sort((a, b) => string.CompareOrdinal(a.SpeciesId, b.SpeciesId));
        return enabled;
    }

    private static bool SameSpecies(TreeSpeciesDefinition a, TreeSpeciesDefinition b)
    {
        return a != null && b != null && string.Equals(a.SpeciesId, b.SpeciesId, System.StringComparison.Ordinal);
    }

    private void UpdateAllMastStates(TreeSpeciesDefinition defaultSpecies, System.Random defaultRng)
    {
        mastBySpeciesId.Clear();
        // Compatibility path: same seed and first random draw as the original Sitka-only model.
        if (defaultSpecies != null && defaultSpecies.SupportsRegeneration)
            UpdateMast(defaultSpecies, defaultRng, true);
        foreach (TreeSpeciesDefinition candidate in RegenerationSpecies(defaultSpecies))
        {
            if (SameSpecies(candidate, defaultSpecies))
                continue;
            UpdateMast(candidate, SpeciesRandom(candidate.SpeciesId, 0x4D415354u), false);
        }
    }

    private void UpdateMast(TreeSpeciesDefinition targetSpecies, System.Random rng, bool isDefault)
    {
        double roll = rng.NextDouble();
        var state = new MastState();
        if (roll < targetSpecies.MastGoodProbability)
        {
            state.Label = "good";
            state.Multiplier = targetSpecies.MastGoodMultiplier;
        }
        else if (roll < targetSpecies.MastGoodProbability + targetSpecies.MastPoorProbability)
        {
            state.Label = "poor";
            state.Multiplier = targetSpecies.MastPoorMultiplier;
        }
        else
        {
            state.Label = "normal";
            state.Multiplier = targetSpecies.MastNormalMultiplier;
        }
        mastBySpeciesId[targetSpecies.SpeciesId] = state;
        if (isDefault)
        {
            lastMastLabel = state.Label;
            lastMastMultiplier = state.Multiplier;
        }
    }

    private System.Random SpeciesRandom(string speciesId, uint phase)
    {
        uint hash = 2166136261u;
        string stableId = speciesId ?? "";
        for (int i = 0; i < stableId.Length; i++)
        {
            hash ^= stableId[i];
            hash *= 16777619u;
        }
        hash ^= phase;
        hash *= 16777619u;
        int seed = unchecked((simulationSeed * 397) ^ ecologicalYear ^ (int)hash);
        return new System.Random(seed);
    }

    private void ComputeSeedRain(TreeSpeciesDefinition defaultSpecies)
    {
        if (cells == null)
            return;
        foreach (ForestEcologyCell cell in cells)
            cell.ClearSeedRain();
        seedPotential.Clear();
        ForestTree[] trees = FindTrees();
        List<TreeSpeciesDefinition> enabled = RegenerationSpecies(defaultSpecies);
        // Default first preserves the exact Sitka accumulation path. Additional
        // species are ordinal and cannot consume or reorder Sitka operations.
        if (defaultSpecies != null && defaultSpecies.SupportsRegeneration)
            ComputeSpeciesSeedRain(defaultSpecies, trees);
        foreach (TreeSpeciesDefinition candidate in enabled)
            if (!SameSpecies(candidate, defaultSpecies))
                ComputeSpeciesSeedRain(candidate, trees);
    }

    private void ComputeSpeciesSeedRain(TreeSpeciesDefinition targetSpecies, ForestTree[] trees)
    {
        float mastMultiplier = GetMastMultiplier(targetSpecies);
        foreach (ForestTree tree in trees)
        {
            if (tree == null || tree.IsStump || !SameSpecies(tree.Species, targetSpecies))
                continue;
            float maturity = targetSpecies.Maturity(tree.AgeYears);
            float crown = Mathf.Clamp01(tree.CrownRadius / 4f);
            seedPotential[tree] = maturity * crown * targetSpecies.SeedPotentialPerMatureTree * mastMultiplier;
        }
        foreach (ForestEcologyCell cell in cells)
        {
            float seedRain = 0f;
            foreach (ForestTree tree in trees)
            {
                if (tree == null || tree.IsStump || !SameSpecies(tree.Species, targetSpecies))
                    continue;
                if (!seedPotential.TryGetValue(tree, out float potential) || potential <= 0f)
                    continue;
                Vector2 treePosition = new Vector2(tree.transform.position.x, tree.transform.position.z);
                float distance = Vector2.Distance(cell.Center, treePosition);
                if (distance > targetSpecies.SeedDispersalCutoffM)
                    continue;
                seedRain += potential * Mathf.Exp(-distance / targetSpecies.SeedDispersalScaleM);
            }
            if (seedRain > 0f)
                cell.GetOrCreateCohort(targetSpecies).SeedRain = seedRain;
        }
    }

    private void GrowExistingRegeneration()
    {
        foreach (ForestEcologyCell cell in cells)
        {
            // Cohorts are stored in ordinal SpeciesId order.
            for (int i = 0; i < cell.Regeneration.Count; i++)
            {
                ForestRegenerationCohort cohort = cell.Regeneration[i];
                TreeSpeciesDefinition cohortSpecies = cohort.Species;
                if (cohort.Density <= 0f || cohortSpecies == null)
                    continue;
                float response = cohortSpecies.JuvenileLightResponse(cell.Light);
                cohort.Height += cohortSpecies.RegenHeightGrowthMPerYear * response * cell.SiteProductivity;
                if (response < cohortSpecies.RegenPoorLightThreshold)
                    cohort.Density *= 1f - cohortSpecies.RegenMortalityUnderPoorLight;
                else
                    cell.AddDensityWithSharedCapacity(cohort, 0.05f);
                if (cohort.Density < 0.01f)
                {
                    cohort.Density = 0f;
                    cohort.Height = 0f;
                    cohort.EstablishYear = -1;
                }
            }
        }
    }

    private void EstablishNewCohorts()
    {
        foreach (ForestEcologyCell cell in cells)
        {
            // Snapshot because GetOrCreateCohort is not needed: seed computation
            // already created a cohort only where real seed rain exists.
            for (int i = 0; i < cell.Regeneration.Count; i++)
            {
                ForestRegenerationCohort cohort = cell.Regeneration[i];
                TreeSpeciesDefinition cohortSpecies = cohort.Species;
                if (cohort.SeedRain <= 0f || cohortSpecies == null)
                    continue;
                float seedFactor = 1f - Mathf.Exp(-cohort.SeedRain / cohortSpecies.SeedSaturationS50);
                float lightResponse = cohortSpecies.JuvenileLightResponse(cell.Light);
                float establishment = seedFactor * lightResponse * cell.EstablishmentSuitability;
                if (establishment <= 0.01f)
                    continue;
                if (cohort.EstablishYear < 0)
                {
                    cohort.EstablishYear = ecologicalYear;
                    cohort.Height = cohortSpecies.RegenInitialHeightM;
                }
                cell.AddDensityWithSharedCapacity(cohort, establishment * cohortSpecies.RegenDensityPerEstablishment);
            }
        }
    }

    private void PromoteCohorts(TreeSpeciesDefinition defaultSpecies, System.Random defaultRng)
    {
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (spawner == null)
            return;
        if (defaultSpecies != null && defaultSpecies.SupportsRegeneration)
            PromoteSpeciesCohorts(spawner, defaultSpecies, defaultRng, true);
        foreach (TreeSpeciesDefinition candidate in RegenerationSpecies(defaultSpecies))
        {
            if (SameSpecies(candidate, defaultSpecies))
                continue;
            PromoteSpeciesCohorts(spawner, candidate, SpeciesRandom(candidate.SpeciesId, 0x50524F4Du), false);
        }
    }

    private void PromoteSpeciesCohorts(ForestTreeSpawner spawner, TreeSpeciesDefinition cohortSpecies, System.Random rng, bool isDefault)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            ForestEcologyCell cell = cells[i];
            ForestRegenerationCohort cohort = cell.FindCohort(cohortSpecies.SpeciesId);
            if (cohort == null || cohort.Density <= 0f || cohort.Height < cohortSpecies.PromotionHeightM)
                continue;
            float offsetX = (float)(rng.NextDouble() - 0.5) * cellSizeMeters;
            float offsetZ = (float)(rng.NextDouble() - 0.5) * cellSizeMeters;
            int age = Mathf.Max(1, ecologicalYear - Mathf.Max(0, cohort.EstablishYear));
            float dbh = Mathf.Clamp(cohort.Height * 1.5f, 2f, 20f); // [C] young-tree proportion
            float crown = cohortSpecies.PotentialCrownRadiusM(dbh);
            string id = isDefault
                ? "R" + ecologicalYear + "-" + i
                : "R" + ecologicalYear + "-" + i + "-" + cohortSpecies.SpeciesId;
            Vector3 position = new Vector3(cell.Center.x + offsetX, 0f, cell.Center.y + offsetZ);
            ForestTree recruited = spawner.Spawn(id, cohortSpecies, position, age, dbh, cohort.Height, crown);
            if (recruited != null)
            {
                Debug.Log($"ECOLOGY recruited {id} ({cohortSpecies.SpeciesId}) at {position} height={cohort.Height:0.00} dbh={dbh:0.0}");
                cohort.Density = 0f;
                cohort.Height = 0f;
                cohort.EstablishYear = -1;
            }
        }
    }

    private void UpdateEstablishmentSuitability()
    {
        foreach (ForestEcologyCell cell in cells)
        {
            float target = Mathf.Clamp01(1f - 0.3f * Mathf.Clamp01(cell.RecentOpening));
            cell.EstablishmentSuitability = Mathf.Lerp(cell.EstablishmentSuitability, target, 0.2f);
        }
    }

    private void DecayRecentOpening(TreeSpeciesDefinition s)
    {
        float factor = Mathf.Pow(0.5f, 1f / s.WindThinningHalfLifeYears);
        foreach (ForestEcologyCell cell in cells)
            cell.RecentOpening *= factor;
    }

    // Diagnostic wind risk (calibration, not an annual mortality probability).
    public float GetWindRisk(ForestTree tree)
    {
        TreeSpeciesDefinition s = tree != null && tree.Species != null ? tree.Species : ResolveSpecies();
        if (s == null || tree == null || cells == null)
            return 0f;
        int index = GetCellIndex(tree.transform.position);
        if (index < 0)
            return 0f;
        ForestEcologyCell cell = cells[index];
        float slenderness = tree.Height / Mathf.Max(0.05f, tree.Diameter / 100f);
        float opening = 0.25f + 0.75f * cell.Light; // [D] not risk-free when unthinned
        float recent = 1f + s.WindOpeningWeight * cell.RecentOpening;
        return s.StandWindSusceptibility * slenderness * opening * recent;
    }

    public float GetCompetitionIndex(ForestTree tree)
    {
        if (!competitionCurrent)
        {
            TreeSpeciesDefinition s = ResolveSpecies();
            if (s != null)
                UpdateCompetition(s);
        }
        return tree != null && competitionIndex.TryGetValue(tree, out float value) ? value : 0f;
    }

    // Fraction of potential DBH growth withheld by the existing competition response.
    // This is a model diagnostic, not a measured physiological suppression index.
    public float GetCurrentSuppression(ForestTree tree)
    {
        if (tree == null || tree.IsStump) return 0f;
        TreeSpeciesDefinition s = tree.Species != null ? tree.Species : ResolveSpecies();
        if (s == null) return 0f;
        return Mathf.Clamp01(1f - 1f / (1f + GetCompetitionIndex(tree) / s.Ci50));
    }

    // Player-readable interpretation of the competition index. Thresholds are [D] calibration.
    public string GetCompetitionLabel(ForestTree tree)
    {
        float ci = GetCompetitionIndex(tree);
        if (ci < 1f) return "open / released";
        if (ci < 3f) return "moderate";
        return "crowded";
    }

    // Player-readable wind exposure band. Thresholds are [D] calibration.
    public string GetWindRiskLabel(ForestTree tree)
    {
        return WindRiskBandLabel(GetWindRisk(tree));
    }

    public string WindRiskBandLabel(float risk)
    {
        if (risk < windLabelLowAt) return "low";
        if (risk < windLabelHighAt) return "moderate";
        return "high";
    }

    public float GetAnnualDbhGrowth(ForestTree tree)
    {
        return tree != null && annualDbhGrowth.TryGetValue(tree, out float value) ? value : 0f;
    }

    public float GetSeedPotential(ForestTree tree)
    {
        return tree != null && seedPotential.TryGetValue(tree, out float value) ? value : 0f;
    }

    public float GetSeedRain(int cellIndex, TreeSpeciesDefinition targetSpecies)
    {
        if (cells == null || cellIndex < 0 || cellIndex >= cells.Length || targetSpecies == null)
            return 0f;
        ForestRegenerationCohort cohort = cells[cellIndex].FindCohort(targetSpecies.SpeciesId);
        return cohort != null ? cohort.SeedRain : 0f;
    }

    public float GetRegenerationDensity(int cellIndex, TreeSpeciesDefinition targetSpecies)
    {
        if (cells == null || cellIndex < 0 || cellIndex >= cells.Length || targetSpecies == null)
            return 0f;
        ForestRegenerationCohort cohort = cells[cellIndex].FindCohort(targetSpecies.SpeciesId);
        return cohort != null ? cohort.Density : 0f;
    }

    public float GetRegenerationHeight(int cellIndex, TreeSpeciesDefinition targetSpecies)
    {
        if (cells == null || cellIndex < 0 || cellIndex >= cells.Length || targetSpecies == null)
            return 0f;
        ForestRegenerationCohort cohort = cells[cellIndex].FindCohort(targetSpecies.SpeciesId);
        return cohort != null ? cohort.Height : 0f;
    }

    // Player-readable regeneration state for one cell, built only from data the
    // simulation already maintains. Classification names the binding constraint
    // (seed / light / site disturbance); no new ecological mechanism.
    public string RegenerationReportLine(Vector3 worldPosition)
    {
        TreeSpeciesDefinition s = ResolveSpecies();
        if (s == null || cells == null)
            return "";
        int index = GetCellIndex(worldPosition);
        if (index < 0)
            return "";
        ForestEcologyCell cell = cells[index];

        ForestRegenerationCohort defaultCohort = cell.FindCohort(s.SpeciesId);
        float defaultSeedRain = defaultCohort != null ? defaultCohort.SeedRain : 0f;
        float seedFactor = 1f - Mathf.Exp(-defaultSeedRain / s.SeedSaturationS50);
        float lightResponse = s.JuvenileLightResponse(cell.Light);
        string seedWord = seedFactor < 0.05f ? "none"
            : seedFactor < 0.3f ? "thin"
            : seedFactor < 0.7f ? "ok"
            : "plenty";

        var cohortText = new System.Text.StringBuilder();
        foreach (ForestRegenerationCohort cohort in cell.Regeneration)
        {
            if (cohort == null || cohort.Species == null || cohort.Density <= 0f)
                continue;
            if (cohortText.Length > 0) cohortText.Append(" · ");
            cohortText.Append($"{cohort.Species.DisplayName} {cohort.Density:0.00}/m2 @ {cohort.Height:0.00} m");
        }
        string regenText = cohortText.Length > 0 ? "regen " + cohortText : "no regeneration";

        string constraint;
        if (defaultCohort != null && defaultCohort.Density > 0f && lightResponse < s.RegenPoorLightThreshold)
            constraint = $"suppressed under shade (light response {lightResponse:0.00})";
        else
        {
            float smallest = Mathf.Min(seedFactor, Mathf.Min(lightResponse, cell.EstablishmentSuitability));
            if (seedFactor <= smallest + 0.001f && seedFactor < 0.3f)
                constraint = seedFactor < 0.05f
                    ? "seed-limited — no seed source within reach"
                    : $"seed-limited (thin seed rain, {seedWord})";
            else if (lightResponse <= smallest + 0.0001f && lightResponse < 0.3f)
                constraint = $"light-limited under canopy (response {lightResponse:0.00})";
            else if (cell.EstablishmentSuitability <= smallest + 0.0001f && cell.EstablishmentSuitability < 0.7f)
                constraint = $"site settling after disturbance ({cell.EstablishmentSuitability:0.00})";
            else if (cell.HasRegeneration)
                constraint = $"growing {s.RegenHeightGrowthMPerYear * lightResponse * cell.SiteProductivity:0.00} m/yr";
            else
                constraint = "establishment conditions good";
        }

        return $"Ground: {regenText} · light {cell.Light:0.00} · seed {seedWord} · suitability {cell.EstablishmentSuitability:0.00} · {constraint}";
    }

    public float GetSiteProductivity(Vector3 worldPosition)
    {
        int index = GetCellIndex(worldPosition);
        return index >= 0 ? cells[index].SiteProductivity : 1f;
    }

    public int GetCellIndex(Vector3 worldPosition)
    {
        if (cells == null)
            return -1;
        float origin = -standSizeMeters * 0.5f;
        int x = Mathf.FloorToInt((worldPosition.x - origin) / cellSizeMeters);
        int z = Mathf.FloorToInt((worldPosition.z - origin) / cellSizeMeters);
        if (x < 0 || z < 0 || x >= cellsPerAxis || z >= cellsPerAxis)
            return -1;
        return z * cellsPerAxis + x;
    }

    public int LivingTreeCount
    {
        get
        {
            int count = 0;
            foreach (ForestTree tree in FindTrees())
                if (tree != null && !tree.IsStump)
                    count++;
            return count;
        }
    }

    // One-line-per-metric stand summary for the starting scenario and any
    // point in time. Density figures are diagnostic anchors, never targets.
    public string StandDiagnostics()
    {
        var living = new List<ForestTree>();
        foreach (ForestTree tree in FindTrees())
            if (tree != null && !tree.IsStump)
                living.Add(tree);
        float areaHa = standSizeMeters * standSizeMeters / 10000f;
        if (living.Count == 0)
            return "stand empty";

        var dbhs = new List<float>();
        float ageSum = 0f, ciSum = 0f, basalArea = 0f;
        int suppressed = 0;
        foreach (ForestTree tree in living)
        {
            dbhs.Add(tree.Diameter);
            ageSum += tree.AgeYears;
            ciSum += GetCompetitionIndex(tree);
            basalArea += (tree.Diameter / 100f) * (tree.Diameter / 100f) * Mathf.PI / 4f;
        }
        dbhs.Sort();
        float meanDbh = 0f;
        foreach (float d in dbhs) meanDbh += d;
        meanDbh /= dbhs.Count;
        int suppressedCount = 0;
        foreach (ForestTree tree in living)
            if (GetCompetitionIndex(tree) > 6f) // [D] suppressed-band reference
                suppressedCount++;

        float lightSum = 0f; int darkCells = 0;
        foreach (ForestEcologyCell cell in cells)
        {
            if (cell == null) continue;
            lightSum += cell.Light;
            if (cell.Light < 0.15f) darkCells++;
        }
        float meanLight = cells != null && cells.Length > 0 ? lightSum / cells.Length : 0f;

        float ha = standSizeMeters * standSizeMeters / 10000f;
        var lines = new System.Text.StringBuilder();
        lines.AppendLine($"living stems: {living.Count}  ({living.Count / areaHa:F0} stems/ha)");
        lines.AppendLine($"basal area: {basalArea / areaHa:F1} m2/ha");
        lines.AppendLine($"DBH mean {meanDbh:F1} cm, median {dbhs[dbhs.Count / 2]:F1}, range {dbhs[0]:F0}-{dbhs[dbhs.Count - 1]:F0}");
        lines.AppendLine($"DBH histogram (2 cm bins): {DbhHistogram(dbhs)}");
        lines.AppendLine($"mean age: {ageSum / living.Count:F1} y");
        lines.AppendLine($"competition: mean CI {ciSum / living.Count:F2}; CI>4 {suppressedCount * 100f / living.Count:F0}% of stems");
        lines.AppendLine($"canopy light: mean {meanLight:F2}; cells <0.15: {darkCells * 100 / Mathf.Max(1, cells.Length)}%");
        return lines.ToString();
    }

    private string DbhHistogram(List<float> dbhs)
    {
        var bins = new SortedDictionary<int, int>();
        foreach (float d in dbhs)
        {
            int bin = Mathf.FloorToInt(d / 2f) * 2;
            bins.TryGetValue(bin, out int count);
            bins[bin] = count + 1;
        }
        var sb = new System.Text.StringBuilder();
        foreach (var pair in bins)
            sb.Append($"{pair.Key}-{pair.Key + 2}:{pair.Value} ");
        return sb.ToString().TrimEnd();
    }

    public float MeanDbhCm
    {
        get
        {
            float total = 0f;
            int count = 0;
            foreach (ForestTree tree in FindTrees())
            {
                if (tree == null || tree.IsStump)
                    continue;
                total += tree.Diameter;
                count++;
            }
            return count > 0 ? total / count : 0f;
        }
    }

    public int RegeneratingCellCount
    {
        get
        {
            if (cells == null)
                return 0;
            int count = 0;
            foreach (ForestEcologyCell cell in cells)
                if (cell.HasRegeneration)
                    count++;
            return count;
        }
    }

    public float MaxSeedRain
    {
        get
        {
            float max = 0f;
            if (cells == null)
                return max;
            TreeSpeciesDefinition defaultSpecies = ResolveSpecies();
            foreach (ForestEcologyCell cell in cells)
            {
                ForestRegenerationCohort cohort = defaultSpecies != null ? cell.FindCohort(defaultSpecies.SpeciesId) : null;
                if (cohort != null && cohort.SeedRain > max)
                    max = cohort.SeedRain;
            }
            return max;
        }
    }

    public float MaxWindRisk
    {
        get
        {
            float max = 0f;
            foreach (ForestTree tree in FindTrees())
            {
                if (tree == null || tree.IsStump)
                    continue;
                float risk = GetWindRisk(tree);
                if (risk > max)
                    max = risk;
            }
            return max;
        }
    }

    private void LogSummary(TreeSpeciesDefinition s)
    {
        if (!logAnnualSummary)
            return;
        Debug.Log($"ECOLOGY year={ecologicalYear} mast={lastMastLabel}({lastMastMultiplier:0.0})"
            + $" living={LivingTreeCount} meanDbh={MeanDbhCm:0.0}cm"
            + $" seedMax={MaxSeedRain:0.000} regenCells={RegeneratingCellCount}"
            + $" maxWind={MaxWindRisk:0.00}");
    }

    private void OnGUI()
    {
        if (timeLapseEnabled)
        {
            float hudScale = ForestHud.Scale;
            if (timeLapseStyle == null)
            {
                timeLapseStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.LowerLeft
                };
                timeLapseStyle.normal.textColor = new Color(0.75f, 0.95f, 0.75f);
            }
            timeLapseStyle.fontSize = Mathf.RoundToInt(18f * hudScale);
            GUI.Label(new Rect(18f, 16f + 92f * hudScale + 10f, 560f * hudScale, 26f * hudScale),
                $"Time-lapse: year {ecologicalYear} — 1 year / {timeLapseSecondsPerYear:0.#} s   [T] speed / stop", timeLapseStyle);
        }

        if (!showDebugGrid || cells == null || cellsPerAxis <= 0)
            return;

        const float mapSize = 180f;
        float cell = mapSize / cellsPerAxis;
        Rect origin = new Rect(Screen.width - mapSize - 20f, 20f, mapSize, mapSize);
        GUI.Box(new Rect(origin.x - 8f, origin.y - 26f, mapSize + 16f, mapSize + 260f), GUIContent.none);

        float seedMax = Mathf.Max(0.0001f, MaxSeedRain);
        TreeSpeciesDefinition defaultSpecies = ResolveSpecies();
        for (int z = 0; z < cellsPerAxis; z++)
        for (int x = 0; x < cellsPerAxis; x++)
        {
            ForestEcologyCell data = cells[z * cellsPerAxis + x];
            ForestRegenerationCohort defaultCohort = defaultSpecies != null ? data.FindCohort(defaultSpecies.SpeciesId) : null;
            float defaultSeedRain = defaultCohort != null ? defaultCohort.SeedRain : 0f;
            Color previous = GUI.color;
            GUI.color = showSeedRain
                ? Color.Lerp(new Color(0.1f, 0.1f, 0.15f), new Color(0.95f, 0.85f, 0.35f), Mathf.Clamp01(defaultSeedRain / seedMax))
                : Color.Lerp(new Color(0.95f, 0.9f, 0.45f), new Color(0.05f, 0.28f, 0.06f), data.Canopy);
            GUI.DrawTexture(new Rect(origin.x + x * cell, origin.y + (cellsPerAxis - 1 - z) * cell, cell - 1f, cell - 1f), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        string mode = showSeedRain ? "seed rain (normalized)" : "canopy / light";
        GUI.Label(new Rect(origin.x - 4f, origin.y - 22f, mapSize + 12f, 20f), $"CCF ecology {mode} — year {ecologicalYear} — {cellSizeMeters:0.#} m");

        var info = new System.Text.StringBuilder();
        info.AppendLine($"mast: {lastMastLabel} x{lastMastMultiplier:0.0}");
        info.AppendLine($"trees: {LivingTreeCount}  mean DBH {MeanDbhCm:0.0} cm");
        info.AppendLine($"seed max {MaxSeedRain:0.000}  regen cells {RegeneratingCellCount}");
        info.AppendLine($"max wind risk {MaxWindRisk:0.00}");
        if (debugCellIndex >= 0 && debugCellIndex < cells.Length)
        {
            ForestEcologyCell c = cells[debugCellIndex];
            info.AppendLine($"cell {debugCellIndex} ({c.Center.x:0},{c.Center.y:0})");
            info.AppendLine($"  light {c.Light:0.00} suitability {c.EstablishmentSuitability:0.00}");
            info.AppendLine($"  opening {c.RecentOpening:0.00} occupancy {c.SharedOccupancy:0.00}");
            foreach (ForestRegenerationCohort cohort in c.Regeneration)
                info.AppendLine($"  {cohort.SpeciesId}: seed {cohort.SeedRain:0.000} regen {cohort.Density:0.00} h {cohort.Height:0.00} m");
        }
        GUI.Label(new Rect(origin.x - 4f, origin.y + mapSize + 6f, mapSize + 12f, 240f), info.ToString());
    }
}

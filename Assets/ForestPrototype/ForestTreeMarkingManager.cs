using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Ecology-owned forestry planning: mark trees for harvest without felling them.
// Marking is deliberately separate from harvesting so a proposed treatment can
// be walked through and reconsidered before any cutting happens.
public sealed class ForestTreeMarkingManager : MonoBehaviour
{
    [SerializeField, Min(0.5f)] private float interactionDistance = 8f; // [D] planning reach, calibration
    [SerializeField] private Color markerColor = new Color(1f, 0.35f, 0.05f, 1f);

    private static readonly Dictionary<string, GameObject> MarkerObjects = new Dictionary<string, GameObject>();
    private static readonly HashSet<string> MarkedIds = new HashSet<string>();

    public static ForestTreeMarkingManager Instance { get; private set; }

    private Camera view;
    private Transform playerRoot;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private ForestTree aimedTree;
    private readonly List<ForestTree> markedTreeCache = new List<ForestTree>();
    private bool markedCacheDirty = true;
    private float nextStaleSweepTime;
    private string message = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle messageStyle;
    private GUIStyle counterStyle;
    private GUIStyle outcomeStyle;
    private Material markerMaterial;

    public int MarkedCount => MarkedIds.Count;

    // Treatment summary for the HUD: only trees still standing with a mark,
    // and their summed biological stem volume. Marking selects; it never fells.
    public int LivingMarkedCount { get; private set; }
    public float MarkedVolumeM3 { get; private set; }

    // Conditional consequence forecast for the pending treatment, rebuilt with
    // the summary sweep. Empty when nothing is marked.
    public string TreatmentOutcome { get; private set; }

    public List<string> GetMarkedIds()
    {
        return new List<string>(MarkedIds);
    }

    public bool IsMarked(ForestTree tree)
    {
        return tree != null && MarkedIds.Contains(tree.TreeId);
    }

    private void Awake()
    {
        Instance = this;
        // These statics survive play-mode sessions without a domain reload;
        // a fresh session must not inherit the previous session's pending marks.
        MarkedIds.Clear();
        MarkerObjects.Clear();
        view = Camera.main;
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        playerRoot = player != null ? player.transform : null;
    }

    private void OnEnable()
    {
        ForestTree.Felled += OnTreeFelled;
    }

    private void OnDisable()
    {
        ForestTree.Felled -= OnTreeFelled;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        UpdateMarkedSummary();

        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        aimedTree = null;
        if (view == null || Cursor.lockState != CursorLockMode.Locked)
            return;

        int hitCount = Physics.RaycastNonAlloc(view.transform.position, view.transform.forward, hitBuffer, interactionDistance);
        bool found = false;
        RaycastHit nearest = default;
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit candidate = hitBuffer[i];
            Transform candidateTransform = candidate.collider.transform;
            if (playerRoot != null && (candidateTransform == playerRoot || candidateTransform.IsChildOf(playerRoot)))
                continue;
            if (!found || candidate.distance < nearest.distance)
            {
                nearest = candidate;
                found = true;
            }
        }
        if (!found)
            return;

        ForestTree tree = nearest.collider.GetComponentInParent<ForestTree>();
        if (tree == null || tree.IsStump)
            return;

        aimedTree = tree;
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.mKey.wasPressedThisFrame)
            ToggleMark(tree);
    }

    private void OnTreeFelled(ForestTree tree)
    {
        // A fell is executed, so it leaves the pending harvest list.
        Unmark(tree, false);
    }

    public void ToggleMark(ForestTree tree)
    {
        if (tree == null || tree.IsStump)
            return;
        if (MarkedIds.Contains(tree.TreeId))
            Unmark(tree);
        else
            Mark(tree);
    }

    public void Mark(ForestTree tree, bool feedback = true)
    {
        if (tree == null || tree.IsStump)
            return;
        if (!MarkedIds.Add(tree.TreeId))
            return;
        CreateMarker(tree);
        markedCacheDirty = true;
        if (feedback)
            SetMessage($"Marked {tree.TreeId} for harvest ({MarkedIds.Count} marked)");
    }

    public void Unmark(ForestTree tree, bool feedback = true)
    {
        if (tree == null)
            return;
        if (!MarkedIds.Remove(tree.TreeId))
            return;
        RemoveMarker(tree.TreeId);
        markedCacheDirty = true;
        if (feedback)
            SetMessage($"Unmarked {tree.TreeId} ({MarkedIds.Count} marked)");
    }

    public void ClearAll()
    {
        var ids = new List<string>(MarkedIds);
        foreach (string id in ids)
            RemoveMarker(id);
        MarkedIds.Clear();
        markedCacheDirty = true;
    }

    // The HUD summary must reflect exactly the trees still standing with a mark.
    // Marks whose tree disappeared without a felling event are pruned by the sweep.
    private void UpdateMarkedSummary()
    {
        bool refresh = markedCacheDirty || Time.unscaledTime >= nextStaleSweepTime;
        if (refresh)
        {
            markedCacheDirty = false;
            nextStaleSweepTime = Time.unscaledTime + 0.5f;
            SweepStaleMarks();
            RebuildMarkedTreeCache();
            UpdateTreatmentOutcome();
        }

        int living = 0;
        float volume = 0f;
        foreach (ForestTree tree in markedTreeCache)
        {
            if (tree == null || tree.IsStump)
                continue;
            living++;
            volume += tree.BiologicalStemVolumeM3;
        }
        LivingMarkedCount = living;
        MarkedVolumeM3 = volume;

        UpdateTreatmentOutcome();
    }

    // Small player-facing forecast of what felling the marked trees would do:
    // retained volume, growth release of the retained trees, light in the opened
    // cells, the resulting wind peak and the opening intensity. Uses the same
    // formulas as the ecology simulation with the marked trees removed, so the
    // numbers are honest approximations of one year after the felling.
    private void UpdateTreatmentOutcome()
    {
        TreatmentOutcome = "";
        if (markedTreeCache.Count == 0)
            return;

        ForestEcologyController eco = Object.FindFirstObjectByType<ForestEcologyController>();
        TreeSpeciesDefinition species = eco != null ? eco.ResolveSpecies() : null;
        if (eco == null || eco.Cells == null || species == null)
            return;

        var marked = new HashSet<ForestTree>();
        foreach (ForestTree tree in markedTreeCache)
            if (tree != null && !tree.IsStump)
                marked.Add(tree);
        if (marked.Count == 0)
            return;

        var living = new List<ForestTree>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && !tree.IsStump)
                living.Add(tree);

        float cap = eco.MaxRecentOpeningPerCell;

        // Forecast per-cell opening: the marked fells add one step each, capped.
        var markedPerCell = new Dictionary<int, int>();
        var affected = new HashSet<int>();
        foreach (ForestTree tree in marked)
        {
            int index = eco.GetCellIndex(tree.transform.position);
            if (index < 0)
                continue;
            affected.Add(index);
            markedPerCell.TryGetValue(index, out int count);
            markedPerCell[index] = count + 1;
        }
        if (affected.Count == 0)
            return;

        var forecastOpening = new Dictionary<int, float>();
        foreach (int index in affected)
        {
            markedPerCell.TryGetValue(index, out int count);
            forecastOpening[index] = Mathf.Min(cap, eco.Cells[index].RecentOpening + count);
        }

        // Forecast light in the affected cells: the same fractional-cover model
        // as RecomputeCanopy, with the marked crowns removed.
        float lightNowSum = 0f, lightAfterSum = 0f;
        var forecastLight = new Dictionary<int, float>();
        foreach (int index in affected)
        {
            Vector2 center = eco.Cells[index].Center;
            float gap = 1f;
            foreach (ForestTree tree in living)
            {
                if (marked.Contains(tree))
                    continue;
                Vector2 position = new Vector2(tree.transform.position.x, tree.transform.position.z);
                float distance = Vector2.Distance(center, position);
                float reach = tree.CrownRadius * 1.5f + eco.CellSizeMeters * 0.5f;
                float lateral = Mathf.Clamp01(1f - distance / reach);
                float vertical = Mathf.Clamp01(tree.Height / 8f);
                gap *= 1f - Mathf.Clamp01(lateral * vertical);
            }
            // Light is the residual gap product (see RecomputeCanopy), so removing
            // crowns raises it.
            forecastLight[index] = gap;
            lightNowSum += eco.Cells[index].Light;
            lightAfterSum += gap;
        }
        float lightNow = lightNowSum / affected.Count;
        float lightAfter = lightAfterSum / affected.Count;

        // Competition and growth of the retained trees, current versus forecast.
        const float competitionCutoffM = 20f; // matches UpdateCompetition's [C] cutoff
        float growthNowSum = 0f, growthAfterSum = 0f;
        int remainingCount = 0;
        foreach (ForestTree tree in living)
        {
            if (marked.Contains(tree))
                continue;
            TreeSpeciesDefinition treeSpecies = tree.Species != null ? tree.Species : species;
            Vector2 position = new Vector2(tree.transform.position.x, tree.transform.position.z);
            float site = eco.GetSiteProductivity(tree.transform.position);
            float sizeFactor = Mathf.Clamp01(1f - tree.Diameter / treeSpecies.MaxDbhCm);
            float potential = treeSpecies.PotentialDbhGrowthCmPerYear * site * sizeFactor;

            float ciNow = eco.GetCompetitionIndex(tree);
            growthNowSum += potential * (1f / (1f + ciNow / treeSpecies.Ci50));

            float ciAfter = 0f;
            foreach (ForestTree neighbour in living)
            {
                if (neighbour == tree || marked.Contains(neighbour))
                    continue;
                Vector2 neighbourPosition = new Vector2(neighbour.transform.position.x, neighbour.transform.position.z);
                float distance = Vector2.Distance(position, neighbourPosition);
                if (distance > competitionCutoffM)
                    continue;
                ciAfter += (neighbour.Diameter / Mathf.Max(1f, tree.Diameter)) / Mathf.Max(0.5f, distance);
            }
            growthAfterSum += potential * (1f / (1f + ciAfter / treeSpecies.Ci50));
            remainingCount++;
        }
        float growthNow = remainingCount > 0 ? growthNowSum / remainingCount : 0f;
        float growthAfter = remainingCount > 0 ? growthAfterSum / remainingCount : 0f;
        float growthPercent = growthNow > 0.0001f ? (growthAfter - growthNow) / growthNow * 100f : 0f;

        // Wind peak for the retained trees under forecast light and opening.
        float windPeak = 0f;
        foreach (ForestTree tree in living)
        {
            if (marked.Contains(tree))
                continue;
            int index = eco.GetCellIndex(tree.transform.position);
            if (index < 0)
                continue;
            float light = forecastLight.TryGetValue(index, out float value) ? value : eco.Cells[index].Light;
            float opening = forecastOpening.TryGetValue(index, out float value2) ? value2 : eco.Cells[index].RecentOpening;
            float slenderness = tree.Height / Mathf.Max(0.05f, tree.Diameter / 100f);
            float exposure = 0.25f + 0.75f * light;
            float recent = 1f + species.WindOpeningWeight * opening;
            float risk = species.StandWindSusceptibility * slenderness * exposure * recent;
            if (risk > windPeak)
                windPeak = risk;
        }

        float keepVolume = 0f;
        foreach (ForestTree tree in living)
            if (!marked.Contains(tree))
                keepVolume += tree.BiologicalStemVolumeM3;

        float openingPeak = 0f;
        foreach (float value in forecastOpening.Values)
            if (value > openingPeak)
                openingPeak = value;

        TreatmentOutcome = $"If felled now: keep {keepVolume:0.0} m3 · growth {(growthPercent >= 0f ? "+" : "")}{growthPercent:0}% · gap light {lightNow:0.00} → {lightAfter:0.00}"
            + $" · wind peak {windPeak:0.0} ({eco.WindRiskBandLabel(windPeak)}) · opening {openingPeak:0.0} of {cap:0.#}";
    }

    private void SweepStaleMarks()
    {
        if (MarkedIds.Count == 0 && MarkerObjects.Count == 0)
            return;

        var livingById = new Dictionary<string, ForestTree>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree != null && !tree.IsStump && !string.IsNullOrEmpty(tree.TreeId))
                livingById[tree.TreeId] = tree;
        }

        var stale = new List<string>();
        foreach (string id in MarkedIds)
        {
            if (!livingById.TryGetValue(id, out ForestTree tree))
            {
                stale.Add(id);
                continue;
            }
            // A living marked tree always keeps its visual; recreate if it was lost.
            if (!MarkerObjects.TryGetValue(id, out GameObject marker) || marker == null)
                CreateMarker(tree);
        }

        foreach (string id in stale)
        {
            MarkedIds.Remove(id);
            RemoveMarker(id);
        }

        if (stale.Count > 0)
            markedCacheDirty = true;
    }

    private void RebuildMarkedTreeCache()
    {
        markedTreeCache.Clear();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || tree.IsStump || string.IsNullOrEmpty(tree.TreeId))
                continue;
            if (!MarkedIds.Contains(tree.TreeId))
                continue;
            markedTreeCache.Add(tree);
        }
    }

    // Used by save loading; marks are keyed by the persistent tree id.
    public void RestoreMarks(List<string> ids)
    {
        ClearAll();
        if (ids == null)
            return;
        var treesById = new Dictionary<string, ForestTree>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree != null && !string.IsNullOrEmpty(tree.TreeId))
                treesById[tree.TreeId] = tree;
        }
        foreach (string id in ids)
        {
            if (treesById.TryGetValue(id, out ForestTree tree) && !tree.IsStump)
                Mark(tree, false);
        }
    }

    private void CreateMarker(ForestTree tree)
    {
        if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject existing) && existing != null)
            return;

        var marker = new GameObject("Harvest Mark");
        marker.transform.SetParent(tree.transform, false);

        GameObject diamond = GameObject.CreatePrimitive(PrimitiveType.Cube);
        diamond.name = "Diamond";
        Destroy(diamond.GetComponent<Collider>());
        diamond.transform.SetParent(marker.transform, false);
        diamond.transform.localPosition = new Vector3(0f, tree.Height + 0.6f, 0f);
        diamond.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
        diamond.transform.localScale = Vector3.one * 0.45f;
        diamond.GetComponent<Renderer>().sharedMaterial = MarkerMaterial();

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Disc";
        Destroy(disc.GetComponent<Collider>());
        disc.transform.SetParent(marker.transform, false);
        disc.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        disc.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
        disc.GetComponent<Renderer>().sharedMaterial = MarkerMaterial();

        MarkerObjects[tree.TreeId] = marker;
    }

    private void RemoveMarker(string treeId)
    {
        if (MarkerObjects.TryGetValue(treeId, out GameObject marker))
        {
            if (marker != null)
                Destroy(marker);
            MarkerObjects.Remove(treeId);
        }
    }

    private Material MarkerMaterial()
    {
        if (markerMaterial != null)
            return markerMaterial;
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");
        markerMaterial = new Material(shader) { name = "HarvestMark" };
        markerMaterial.color = markerColor;
        return markerMaterial;
    }

    private void SetMessage(string text)
    {
        message = text;
        messageTimer = 2.5f;
    }

    private void OnGUI()
    {
        if (messageTimer > 0f && messageStyle == null)
        {
            messageStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            messageStyle.normal.textColor = new Color(1f, 0.85f, 0.55f);
        }
        if (messageTimer > 0f)
        {
            float hudScale = Mathf.Max(1f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));
            float messageWidth = Mathf.Min(920f, Screen.width - 32f);
            GUI.Box(new Rect(Screen.width * 0.5f - messageWidth * 0.5f, 16f + 92f * hudScale + 8f, messageWidth, 92f), message, messageStyle);
        }

        if (counterStyle == null)
        {
            counterStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft
            };
            counterStyle.normal.textColor = new Color(0.95f, 0.85f, 0.65f);
        }
        GUI.Label(new Rect(18f, Screen.height - 44f, 680f, 28f),
            $"Marked for harvest: {LivingMarkedCount} — {MarkedVolumeM3:0.0} m³   [M] mark / unmark", counterStyle);

        if (!string.IsNullOrEmpty(TreatmentOutcome))
        {
            if (outcomeStyle == null)
            {
                outcomeStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 17,
                    alignment = TextAnchor.LowerLeft
                };
                outcomeStyle.normal.textColor = new Color(0.8f, 0.88f, 0.95f);
            }
            GUI.Label(new Rect(18f, Screen.height - 74f, Screen.width - 36f, 24f), TreatmentOutcome, outcomeStyle);
        }

        if (aimedTree == null)
            return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            promptStyle.normal.textColor = Color.white;
        }
        string prompt = IsMarked(aimedTree) ? "[M] Unmark" : "[M] Mark for Harvest";
        GUI.Box(new Rect(Screen.width * 0.5f - 200f, Screen.height * 0.5f - 96f, 400f, 46f), prompt, promptStyle);
    }
}

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

// Crop Tree competitor assessment: keeps the competition breakdown for the
// inspected Crop Tree and the release summary for the current marks, and shows
// temporary numbered rings on the listed neighbours while that Crop Tree is
// inspected. Presentation only: it reads trees and marks, writes no tree,
// mark, save or ecology state, and recalculates only when the inspected tree,
// the marks or the ecological year change (checked four times a second).
public sealed class CompetitorAssessment
{
    // The rings and number labels can be switched off here if review finds
    // they read as instructions; the ranked list stays.
    public const bool ShowWorldMarkers = true;
    private const float RefreshInterval = 0.25f;
    private const float RingHeightM = 1.3f; // breast height, where DBH is measured

    private static readonly Color ListedColour = new Color(232f / 255f, 206f / 255f, 120f / 255f, 0.9f);
    private static readonly Color SelectedColour = new Color(1f, 1f, 1f, 1f);

    private readonly ScenarioOneUiRoot ui;
    private readonly VisualElement labelLayer;
    private readonly List<Label> labels = new List<Label>();
    private readonly List<LineRenderer> rings = new List<LineRenderer>();
    private GameObject markerRoot;
    private Material listedMaterial, selectedMaterial;

    private List<CompetitionTree> trees = new List<CompetitionTree>();
    private readonly List<ForestTree> sceneTrees = new List<ForestTree>();
    private string sceneSignature = "";
    private string reportTreeId = "";
    private float nextRefresh;

    public CropTreeCompetitionReport Report { get; private set; }
    public ForestTree ReportTree { get; private set; }
    public CropTreeReleaseSummary Summary { get; private set; }
    public float TargetCi50 { get; private set; }
    // 0-based index into the listed entries, or -1.
    public int SelectedIndex { get; private set; } = -1;
    // Changes whenever anything shown on the card changes.
    public int Version { get; private set; }

    public CompetitorAssessment(ScenarioOneUiRoot ui, VisualElement root)
    {
        this.ui = ui;
        labelLayer = UiKit.Box("layer");
        labelLayer.name = "competitor-tags";
        labelLayer.pickingMode = PickingMode.Ignore;
        root.Add(labelLayer);
    }

    public int ListedCount => Report == null ? 0 : Mathf.Min(CropTreeCompetition.ListedCount, Report.NeighbourCount);
    public CompetitorEntry Selected => Report != null && SelectedIndex >= 0 && SelectedIndex < ListedCount ? Report.Ranked[SelectedIndex] : null;

    // Called every frame by the UI root. inspected is null unless the walking
    // inspection card is visible.
    public void Update(ForestTree inspected)
    {
        if (Time.unscaledTime >= nextRefresh)
        {
            nextRefresh = Time.unscaledTime + RefreshInterval;
            RefreshIfChanged(inspected);
        }
        else if ((inspected != null ? inspected.TreeId : "") != reportTreeId)
            RefreshIfChanged(inspected);

        bool assessing = inspected != null && inspected.IsCropTree && Report != null && ReportTree == inspected;
        if (assessing) HandleSelectionKeys();
        else SelectedIndex = -1;
        UpdateRings(assessing);
    }

    // Labels follow the camera, so they are placed after it has moved.
    // Panels a label must not be drawn underneath (set by the UI root).
    public readonly List<VisualElement> OccludingPanels = new List<VisualElement>();

    public void LateUpdate(ForestTree inspected)
    {
        bool assessing = ShowWorldMarkers && inspected != null && inspected.IsCropTree && Report != null && ReportTree == inspected;
        Camera camera = Camera.main;
        IPanel panel = labelLayer.panel;
        for (int i = 0; i < labels.Count; i++)
        {
            bool visible = assessing && camera != null && panel != null && i < ListedCount;
            if (visible)
            {
                ForestTree tree = sceneTrees[Report.Ranked[i].TreeIndex];
                if (tree == null) { labels[i].style.display = DisplayStyle.None; continue; }
                Vector3 world = tree.transform.position + Vector3.up * (RingHeightM + 0.45f);
                Vector3 toPoint = world - camera.transform.position;
                visible = Vector3.Dot(toPoint, camera.transform.forward) > 0.2f;
                if (visible)
                {
                    Vector2 point = RuntimePanelUtils.CameraTransformWorldToPanel(panel, world, camera);
                    labels[i].style.left = point.x - 12f;
                    labels[i].style.top = point.y - 12f;
                    // A label under the card or side panel would show through it; turn to see that tree.
                    foreach (VisualElement occluder in OccludingPanels)
                        if (occluder != null && occluder.resolvedStyle.display == DisplayStyle.Flex && occluder.worldBound.Contains(point))
                            visible = false;
                }
            }
            labels[i].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void Clear()
    {
        SelectedIndex = -1;
        UpdateRings(false);
        foreach (Label label in labels) label.style.display = DisplayStyle.None;
    }

    public void Destroy()
    {
        if (markerRoot != null) Object.Destroy(markerRoot);
        if (listedMaterial != null) Object.Destroy(listedMaterial);
        if (selectedMaterial != null) Object.Destroy(selectedMaterial);
        markerRoot = null;
        rings.Clear();
    }

    private void RefreshIfChanged(ForestTree inspected)
    {
        ForestTreeMarkingManager marking = ui.Marking;
        ForestEcologyController eco = ui.Ecology;
        string signature = Signature(marking, eco);
        bool sceneChanged = signature != sceneSignature;
        if (sceneChanged)
        {
            sceneSignature = signature;
            BuildTrees();
            Summary = CropTreeCompetition.Summarise(trees);
            Version++;
        }

        string inspectedId = inspected != null ? inspected.TreeId : "";
        bool treeChanged = inspectedId != reportTreeId;
        if (!sceneChanged && !treeChanged)
            return;
        // A new tree starts unselected; a mark change keeps the selection,
        // because marks do not change any neighbour's contribution or rank.
        if (treeChanged) SelectedIndex = -1;
        reportTreeId = inspectedId;
        int index = inspected != null ? sceneTrees.IndexOf(inspected) : -1;
        if (index < 0)
        {
            Report = null;
            ReportTree = null;
        }
        else
        {
            Report = CropTreeCompetition.Analyse(trees, index);
            CropTreeCompetition.CountNearbyCropTrees(trees, Report, CropTreeCompetition.ListedCount);
            ReportTree = inspected;
            TargetCi50 = inspected.Species != null ? inspected.Species.Ci50 : 0f;
        }
        Version++;
    }

    // Marks, the ecological year and the identity of the living tree objects
    // describe everything the breakdown depends on between annual steps. The
    // object identities matter because loading a save replaces every tree
    // object without necessarily changing the year or the marks.
    private static string Signature(ForestTreeMarkingManager marking, ForestEcologyController eco)
    {
        var sb = new StringBuilder();
        sb.Append(eco != null ? eco.EcologicalYear : -1).Append('|');
        int living = 0;
        long identity = 17;
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (tree == null || !tree.IsLiving) continue;
            living++;
            identity += System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(tree) * 31L;
        }
        sb.Append(living).Append(':').Append(identity).Append('|');
        if (marking != null)
        {
            List<string> fell = marking.GetMarkedIds();
            List<string> crop = marking.GetCropTreeIds();
            fell.Sort(System.StringComparer.Ordinal);
            crop.Sort(System.StringComparer.Ordinal);
            sb.Append(string.Join(",", fell)).Append('|').Append(string.Join(",", crop));
        }
        return sb.ToString();
    }

    // Living trees in ordinal tree-id order, as ForestEcologyController sums them.
    private void BuildTrees()
    {
        sceneTrees.Clear();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (tree != null && tree.IsLiving)
                sceneTrees.Add(tree);
        sceneTrees.Sort((a, b) => string.CompareOrdinal(a.TreeId, b.TreeId));
        trees = new List<CompetitionTree>(sceneTrees.Count);
        foreach (ForestTree tree in sceneTrees)
        {
            Vector3 p = tree.transform.position;
            trees.Add(new CompetitionTree(tree.TreeId, new Vector2(p.x, p.z), tree.Diameter, tree.IsMarkedForFell, tree.IsCropTree));
        }
    }

    private void HandleSelectionKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || (ui.Player != null && ui.Player.IsPlantingMode))
            return;
        KeyControl[] keys = { keyboard.digit1Key, keyboard.digit2Key, keyboard.digit3Key, keyboard.digit4Key, keyboard.digit5Key };
        for (int i = 0; i < keys.Length && i < ListedCount; i++)
        {
            if (!keys[i].wasPressedThisFrame) continue;
            SelectedIndex = SelectedIndex == i ? -1 : i;
            Version++;
            return;
        }
    }

    private void UpdateRings(bool assessing)
    {
        int count = assessing && ShowWorldMarkers ? ListedCount : 0;
        if (count > 0) EnsureMarkers();
        for (int i = 0; i < rings.Count; i++)
        {
            ForestTree tree = i < count ? sceneTrees[Report.Ranked[i].TreeIndex] : null;
            bool show = tree != null;
            rings[i].gameObject.SetActive(show);
            if (!show) continue;
            bool selected = i == SelectedIndex;
            float radius = Mathf.Max(0.25f, tree.Diameter / 200f + (selected ? 0.25f : 0.15f));
            Vector3 centre = tree.transform.position + Vector3.up * RingHeightM;
            LineRenderer ring = rings[i];
            ring.startWidth = ring.endWidth = selected ? 0.07f : 0.035f;
            // URP Unlit ignores vertex colour, so each state has its own material.
            ring.sharedMaterial = selected ? selectedMaterial : listedMaterial;
            for (int p = 0; p < ring.positionCount; p++)
            {
                float angle = p * Mathf.PI * 2f / ring.positionCount;
                ring.SetPosition(p, centre + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
            }
        }
        for (int i = 0; i < labels.Count; i++)
        {
            bool selected = i == SelectedIndex;
            labels[i].EnableInClassList("competitor-tag-selected", selected);
        }
    }

    private void EnsureMarkers()
    {
        if (markerRoot == null)
        {
            markerRoot = new GameObject("Crop Tree competitor rings (runtime)");
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            listedMaterial = new Material(shader) { name = "Transient competitor ring", color = ListedColour };
            selectedMaterial = new Material(shader) { name = "Transient selected competitor ring", color = SelectedColour };
        }
        while (rings.Count < CropTreeCompetition.ListedCount)
        {
            var go = new GameObject("Competitor ring " + (rings.Count + 1));
            go.transform.SetParent(markerRoot.transform, false);
            var line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = listedMaterial;
            line.useWorldSpace = true;
            line.loop = true;
            line.positionCount = 24;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            rings.Add(line);
        }
        while (labels.Count < CropTreeCompetition.ListedCount)
        {
            Label label = UiKit.Text((labels.Count + 1).ToString(UiKit.Inv), "competitor-tag");
            label.pickingMode = PickingMode.Ignore;
            label.style.position = Position.Absolute;
            label.style.display = DisplayStyle.None;
            labelLayer.Add(label);
            labels.Add(label);
        }
    }
}

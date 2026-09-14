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
    private string message = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle messageStyle;
    private GUIStyle counterStyle;
    private Material markerMaterial;

    public int MarkedCount => MarkedIds.Count;

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
        if (feedback)
            SetMessage($"Unmarked {tree.TreeId} ({MarkedIds.Count} marked)");
    }

    public void ClearAll()
    {
        var ids = new List<string>(MarkedIds);
        foreach (string id in ids)
            RemoveMarker(id);
        MarkedIds.Clear();
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
        if (MarkerObjects.ContainsKey(tree.TreeId))
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
        GUI.Label(new Rect(18f, Screen.height - 44f, 420f, 28f), $"Marked trees: {MarkedIds.Count}   [M] mark / unmark", counterStyle);

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

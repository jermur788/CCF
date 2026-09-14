using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestBuildable : MonoBehaviour
{
    [SerializeField, Min(1)] private int woodCost = 8;
    [SerializeField, Min(0.5f)] private float interactionDistance = 3.5f;
    [SerializeField, Min(0f)] private float storageSearchRadius = 6f;
    // While this buildable stands, the player can carry this many extra wood units.
    [SerializeField, Min(0)] private int carriedWoodCapacityBonus = 0;
    // While this buildable stands, axe swings recover this many seconds faster.
    [SerializeField, Min(0)] private float swingCooldownReduction = 0f;
    // Plank-using buildables also require planks from a specific plank rack.
    [SerializeField, Min(0)] private int plankCost = 0;
    [SerializeField] private ForestWoodStorage plankSource;
    [SerializeField] private string displayName = "Forestry Workbench";
    [SerializeField] private string buildId = "workbench-01";
    [SerializeField] private ForestBuildable requiredBuildable;
    [SerializeField] private GameObject unbuiltVisual;
    [SerializeField] private GameObject builtVisual;

    private Camera view;
    private Transform playerRoot;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private readonly List<ForestWoodStorage> nearbyStorages = new List<ForestWoodStorage>();
    private bool isLooking;
    private bool isBuilt;
    private string message = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle messageStyle;

    private void Awake()
    {
        view = Camera.main;

        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        playerRoot = player != null ? player.transform : null;

        isBuilt = builtVisual != null && builtVisual.activeSelf;
        SetVisuals(isBuilt);
    }

    private void Update()
    {
        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        isLooking = false;
        if (isBuilt || view == null || Cursor.lockState != CursorLockMode.Locked)
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

        Transform hitTransform = nearest.collider.transform;
        if (hitTransform != transform && !hitTransform.IsChildOf(transform))
            return;

        isLooking = true;
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        bool interactPressed = (keyboard != null && keyboard.eKey.wasPressedThisFrame) ||
                               (mouse != null && mouse.leftButton.wasPressedThisFrame);
        if (interactPressed)
            TryBuild();
    }

    private void TryBuild()
    {
        if (!IsPrerequisiteMet)
        {
            SetMessage($"Requires {requiredBuildable.DisplayName}", 3f);
            return;
        }

        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        if (player == null)
        {
            SetMessage("Cannot build: player inventory is unavailable.", 3f);
            return;
        }

        int carried = player.CarriedWood;
        int stored = GatherNearbyStoredWood();
        if (carried + stored < woodCost)
        {
            SetMessage($"Not enough wood. Need {woodCost} (carried {carried}, nearby stored {stored}).", 3.5f);
            return;
        }

        if (plankCost > 0)
        {
            int planks = plankSource != null ? plankSource.StoredWood : 0;
            if (planks < plankCost)
            {
                SetMessage($"Need {plankCost} planks. Available: {planks}.", 3.5f);
                return;
            }
        }

        // Carried timber pays first; whatever is missing comes out of nearby racks.
        int fromPlayer = Mathf.Min(carried, woodCost);
        int fromStorage = woodCost - fromPlayer;
        if (fromPlayer > 0)
            player.TrySpendWood(fromPlayer);
        SpendStoredWood(fromStorage);
        if (plankCost > 0)
            plankSource.TakeStoredWood(plankCost);

        isBuilt = true;
        SetVisuals(true);
        string plankNote = plankCost > 0 ? $", {plankCost} planks" : "";
        string source = fromStorage <= 0
            ? $"all carried{plankNote}"
            : fromPlayer <= 0
                ? $"all stored{plankNote}"
                : $"{fromPlayer} carried, {fromStorage} stored{plankNote}";
        SetMessage($"{displayName} built. Wood -{woodCost}{plankNote}. ({source}).", 3.5f);
        Debug.Log($"FOREST_BUILD: {displayName} constructed for {woodCost} wood ({source}).", this);
    }

    private int GatherNearbyStoredWood()
    {
        nearbyStorages.Clear();
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>();
        int total = 0;
        foreach (ForestWoodStorage storage in storages)
        {
            if (storage == null || !storage.IsActive || storage.StoresPlanks || storage.StoredWood <= 0)
                continue;
            if (Vector3.Distance(transform.position, storage.transform.position) > storageSearchRadius)
                continue;
            nearbyStorages.Add(storage);
            total += storage.StoredWood;
        }
        return total;
    }

    private void SpendStoredWood(int amount)
    {
        int remaining = amount;
        foreach (ForestWoodStorage storage in nearbyStorages)
        {
            if (remaining <= 0)
                break;
            remaining -= storage.TakeStoredWood(remaining);
        }
    }

    private void SetVisuals(bool built)
    {
        if (unbuiltVisual != null)
            unbuiltVisual.SetActive(!built);
        if (builtVisual != null)
            builtVisual.SetActive(built);
    }

    public string BuildId => buildId;
    public string DisplayName => displayName;
    public bool IsBuilt => isBuilt;
    public int CarriedWoodCapacityBonus => carriedWoodCapacityBonus;
    public float SwingCooldownReduction => swingCooldownReduction;
    public bool HasPrerequisite => requiredBuildable != null;
    public bool IsPrerequisiteMet => requiredBuildable == null || requiredBuildable.IsBuilt;

    public void RestoreBuiltState(bool built)
    {
        isBuilt = built;
        SetVisuals(built);
    }

    private void SetMessage(string text, float duration)
    {
        message = text;
        messageTimer = duration;
    }

    private void OnGUI()
    {
        if (messageTimer > 0f)
        {
            if (messageStyle == null)
            {
                messageStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 36,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                messageStyle.normal.textColor = new Color(1f, 0.95f, 0.55f);
            }

            float hudScale = Mathf.Max(1f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));
            float messageWidth = Mathf.Min(920f, Screen.width - 32f);
            float messageY = 16f + 92f * hudScale + 8f;
            GUI.Box(new Rect(Screen.width * 0.5f - messageWidth * 0.5f, messageY, messageWidth, 92f), message, messageStyle);
        }

        if (!isLooking)
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

        string prompt = IsPrerequisiteMet
            ? (plankCost > 0
                ? $"[E] Build {displayName} ({woodCost} Wood + {plankCost} Planks)"
                : $"[E] Build {displayName} ({woodCost} Wood)")
            : $"Requires {requiredBuildable.DisplayName}";
        GUI.Box(new Rect(Screen.width * 0.5f - 240f, Screen.height * 0.5f + 40f, 480f, 58f), prompt, promptStyle);
    }
}

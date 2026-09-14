using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// First processing step: saws stored logs into planks. Logs are pulled from
// active wood racks close to the pit; planks accumulate in the plank rack.
public sealed class ForestSawPit : MonoBehaviour
{
    [SerializeField, Min(0.5f)] private float interactionDistance = 4f;
    [SerializeField, Min(0f)] private float logRackSearchRadius = 8f;
    [SerializeField] private string plankStorageId = "plank-rack-01";
    // [D] gameplay calibration for the first processing step.
    [SerializeField, Min(1)] private int logsPerBatch = 5;
    [SerializeField, Min(1)] private int planksPerBatch = 2;

    private Camera view;
    private Transform playerRoot;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private readonly List<ForestWoodStorage> nearbyLogStorages = new List<ForestWoodStorage>();
    private bool isLooking;
    private string message = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle messageStyle;

    private void Awake()
    {
        view = Camera.main;
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        playerRoot = player != null ? player.transform : null;
    }

    private bool IsActive
    {
        get
        {
            ForestBuildable buildable = GetComponent<ForestBuildable>();
            return buildable != null && buildable.IsBuilt;
        }
    }

    private void Update()
    {
        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        isLooking = false;
        if (!IsActive || view == null || Cursor.lockState != CursorLockMode.Locked)
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
        bool convertPressed = (keyboard != null && keyboard.eKey.wasPressedThisFrame) ||
                               (mouse != null && mouse.leftButton.wasPressedThisFrame);
        if (convertPressed)
            ConvertOneBatch();
    }

    private void ConvertOneBatch()
    {
        ForestWoodStorage plankStorage = FindPlankStorage();
        if (plankStorage == null)
        {
            SetMessage("No plank rack found.", 3f);
            return;
        }

        int available = GatherLogWood();
        if (available < logsPerBatch)
        {
            SetMessage($"Need {logsPerBatch} logs in a nearby rack. Stored: {available}.", 3f);
            return;
        }

        if (plankStorage.FreeCapacity < planksPerBatch)
        {
            SetMessage($"{plankStorage.DisplayName} full: {plankStorage.StoredWood} / {plankStorage.Capacity}", 3f);
            return;
        }

        SpendLogWood(logsPerBatch);
        plankStorage.AddStoredWood(planksPerBatch);
        SetMessage($"Sawed {logsPerBatch} logs into {planksPerBatch} planks ({plankStorage.StoredWood} planks stored).", 3f);
        Debug.Log($"FOREST_SAW: {logsPerBatch} logs -> {planksPerBatch} planks ({plankStorage.StoredWood} stored).", this);
    }

    private ForestWoodStorage FindPlankStorage()
    {
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>();
        foreach (ForestWoodStorage storage in storages)
        {
            if (storage != null && storage.StorageId == plankStorageId)
                return storage;
        }
        return null;
    }

    private int GatherLogWood()
    {
        nearbyLogStorages.Clear();
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>();
        int total = 0;
        foreach (ForestWoodStorage storage in storages)
        {
            if (storage == null || !storage.IsActive || storage.StoresPlanks || storage.StoredWood <= 0)
                continue;
            if (Vector3.Distance(transform.position, storage.transform.position) > logRackSearchRadius)
                continue;
            nearbyLogStorages.Add(storage);
            total += storage.StoredWood;
        }
        return total;
    }

    private void SpendLogWood(int amount)
    {
        int remaining = amount;
        foreach (ForestWoodStorage storage in nearbyLogStorages)
        {
            if (remaining <= 0)
                break;
            remaining -= storage.TakeStoredWood(remaining);
        }
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

        int logs = GatherLogWood();
        ForestWoodStorage plankStorage = FindPlankStorage();
        int planks = plankStorage != null ? plankStorage.StoredWood : 0;
        string prompt = $"[E] Saw {logsPerBatch} Logs into {planksPerBatch} Planks\nLogs nearby: {logs}   Planks: {planks}";
        GUI.Box(new Rect(Screen.width * 0.5f - 260f, Screen.height * 0.5f + 40f, 520f, 116f), prompt, promptStyle);
    }
}

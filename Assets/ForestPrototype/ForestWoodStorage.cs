using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestWoodStorage : MonoBehaviour
{
    [SerializeField] private string storageId = "log-rack-01";
    [SerializeField, Min(1)] private int capacity = 100;
    [SerializeField, Min(0.5f)] private float interactionDistance = 3.5f;
    [SerializeField] private string displayName = "Log Rack";
    // Plank racks hold sawn planks instead of wood units: the player never
    // hand-carries planks, so deposit/withdraw input is skipped for them.
    [SerializeField] private bool storesPlanks = false;
    [SerializeField] private ForestBuildable buildable;
    [SerializeField] private GameObject logFillLow;
    [SerializeField] private GameObject logFillMedium;
    [SerializeField] private GameObject logFillFull;

    private int storedWood;
    private Camera view;
    private Transform playerRoot;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private bool isLooking;
    private string message = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle messageStyle;

    public string StorageId => storageId;
    public string DisplayName => displayName;
    public int StoredWood => storedWood;
    public int Capacity => capacity;
    public int FreeCapacity => Mathf.Max(0, capacity - storedWood);
    public bool StoresPlanks => storesPlanks;
    public ForestBuildable Buildable => buildable;
    public bool IsActive => buildable != null && buildable.IsBuilt;

    private void Awake()
    {
        view = Camera.main;
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        playerRoot = player != null ? player.transform : null;
        RefreshFill();
    }

    private void Update()
    {
        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        isLooking = false;
        if (!IsActive || storesPlanks || view == null || Cursor.lockState != CursorLockMode.Locked)
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
        bool depositPressed = (keyboard != null && keyboard.eKey.wasPressedThisFrame) ||
                              (mouse != null && mouse.leftButton.wasPressedThisFrame);
        bool withdrawPressed = keyboard != null && keyboard.fKey.wasPressedThisFrame;
        if (depositPressed)
            DepositFromPlayer();
        else if (withdrawPressed)
            WithdrawToPlayer();
    }

    public void DepositFromPlayer()
    {
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        if (player == null)
        {
            SetMessage("Cannot store: player inventory is unavailable.", 3f);
            return;
        }
        if (player.CarriedWood <= 0)
        {
            SetMessage("No carried wood to store", 3f);
            return;
        }

        int transfer = Mathf.Min(player.CarriedWood, FreeCapacity);
        if (transfer <= 0)
        {
            SetMessage($"{displayName} full: {storedWood} / {capacity}", 3f);
            return;
        }

        player.TrySpendWood(transfer);
        storedWood += transfer;
        RefreshFill();
        SetMessage($"Stored {transfer} wood ({storedWood} / {capacity})", 3f);
    }

    public void WithdrawToPlayer()
    {
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        if (player == null)
        {
            SetMessage("Cannot withdraw: player inventory is unavailable.", 3f);
            return;
        }
        if (storedWood <= 0)
        {
            SetMessage($"{displayName} is empty", 3f);
            return;
        }

        int space = player.FreeWoodCapacity;
        if (space <= 0)
        {
            SetMessage("Cannot carry more wood", 3f);
            return;
        }

        int transfer = Mathf.Min(space, storedWood);
        storedWood -= transfer;
        player.TryAddWood(transfer);
        RefreshFill();
        SetMessage($"Withdrew {transfer} wood ({storedWood} / {capacity})", 3f);
    }

    public void RestoreStoredWood(int amount)
    {
        storedWood = Mathf.Clamp(amount, 0, capacity);
        RefreshFill();
    }

    // Construction pulls timber straight from an active rack; the buildable reports the result.
    public int TakeStoredWood(int amount)
    {
        int taken = Mathf.Clamp(amount, 0, storedWood);
        if (taken <= 0)
            return 0;
        storedWood -= taken;
        RefreshFill();
        return taken;
    }

    // Processing adds its output here; the caller reports the result.
    public int AddStoredWood(int amount)
    {
        int added = Mathf.Clamp(amount, 0, FreeCapacity);
        if (added <= 0)
            return 0;
        storedWood += added;
        RefreshFill();
        return added;
    }

    private void RefreshFill()
    {
        float fraction = capacity > 0 ? (float)storedWood / capacity : 0f;
        if (logFillLow != null)
            logFillLow.SetActive(fraction > 0f);
        if (logFillMedium != null)
            logFillMedium.SetActive(fraction > 1f / 3f);
        if (logFillFull != null)
            logFillFull.SetActive(fraction > 2f / 3f);
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
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            promptStyle.normal.textColor = Color.white;
        }

        string prompt = $"[E] Deposit Wood\n[F] Withdraw Wood\nStored: {storedWood} / {capacity}";
        GUI.Box(new Rect(Screen.width * 0.5f - 260f, Screen.height * 0.5f + 40f, 520f, 116f), prompt, promptStyle);
    }
}

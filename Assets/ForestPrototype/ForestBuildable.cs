using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestBuildable : MonoBehaviour
{
    [SerializeField, Min(1)] private int woodCost = 8;
    [SerializeField, Min(0.5f)] private float interactionDistance = 3.5f;
    [SerializeField] private string displayName = "Forestry Workbench";
    [SerializeField] private string buildId = "workbench-01";
    [SerializeField] private GameObject unbuiltVisual;
    [SerializeField] private GameObject builtVisual;

    private Camera view;
    private Transform playerRoot;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
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
        ForestPlayer player = view != null ? view.GetComponentInParent<ForestPlayer>() : null;
        if (player == null)
        {
            SetMessage("Cannot build: player inventory is unavailable.", 3f);
            return;
        }

        int wood = player.WoodCount;
        if (wood < woodCost)
        {
            SetMessage($"Not enough wood. Need {woodCost}, have {wood}.", 3f);
            return;
        }

        player.WoodCount = wood - woodCost;
        isBuilt = true;
        SetVisuals(true);
        SetMessage($"{displayName} built. Wood -{woodCost}.", 3.5f);
        Debug.Log($"FOREST_BUILD: {displayName} constructed for {woodCost} wood.", this);
    }

    private void SetVisuals(bool built)
    {
        if (unbuiltVisual != null)
            unbuiltVisual.SetActive(!built);
        if (builtVisual != null)
            builtVisual.SetActive(built);
    }

    public string BuildId => buildId;
    public bool IsBuilt => isBuilt;

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

        string prompt = $"[E] Build {displayName} ({woodCost} Wood)";
        GUI.Box(new Rect(Screen.width * 0.5f - 240f, Screen.height * 0.5f + 40f, 480f, 58f), prompt, promptStyle);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class ForestPlayer : MonoBehaviour
{
    [SerializeField] private Transform view;
    [SerializeField, Min(0.1f)] private float moveSpeed = 4f;
    [SerializeField, Min(0.1f)] private float runSpeed = 7f;
    [SerializeField, Min(0.1f)] private float jumpHeight = 1.2f;
    [SerializeField, Min(0.1f)] private float secondJumpHeight = 2f;
    private bool airJumpAvailable;
    private const float Gravity = 20f;
    [SerializeField, Min(0.01f)] private float mouseSensitivity = 0.12f;
    [SerializeField, Min(0.5f)] private float interactionDistance = 3.5f;
    [SerializeField, Range(18, 72)] private int promptFontSize = 36;
    [SerializeField, Min(0.1f)] private float swingCooldown = 0.45f;
    [SerializeField, Min(1)] private int maxCarriedWood = 20;
    [SerializeField] private int carriedWood = 0;
    private CharacterController controller;
    private float pitch;
    private float verticalSpeed;
    private bool isLookingAtTree;
    private ForestTree aimedTree;
    private bool isInspecting;
    private ForestTree inspectedTree;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private string inspectedTreeName = "";
    private float inspectedTreeHeight;
    private float inspectedTreeDiameter;
    private float lastChopTime = -1f;
    private float lookPixelsSinceRecentre;
    private float secondsSinceRecentre;
    private float chopImpactTimer;
    private Vector3 defaultCameraLocalPos = new Vector3(0f, 1.65f, 0f);
    private string lastHarvestMessage = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle cardStyle;
    private GUIStyle cardTitleStyle;
    private GUIStyle cardBodyStyle;
    private GUIStyle cardFooterStyle;
    private GUIStyle hudLabelStyle;
    private GUIStyle hudValueStyle;
    private int recentreGraceFrames;
    private GUIStyle notificationStyle;

    public int MaxCarriedWood => maxCarriedWood;
    public int CarriedWood => carriedWood;
    public int FreeWoodCapacity => Mathf.Max(0, maxCarriedWood - carriedWood);

    public bool CanCarryWood(int amount)
    {
        return amount >= 0 && carriedWood + amount <= maxCarriedWood;
    }

    public int TryAddWood(int amount)
    {
        if (amount <= 0)
            return 0;
        int added = Mathf.Min(amount, FreeWoodCapacity);
        carriedWood += added;
        return added;
    }

    public bool TrySpendWood(int amount)
    {
        if (amount <= 0 || carriedWood < amount)
            return false;
        carriedWood -= amount;
        return true;
    }

    // Used by save loading so legacy over-capacity saves keep every unit.
    public void RestoreCarriedWood(int amount)
    {
        carriedWood = Mathf.Max(0, amount);
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (view == null)
        {
            Debug.LogError("ForestPlayer requires a camera transform.", this);
            enabled = false;
        }
        else
        {
            defaultCameraLocalPos = view.localPosition;
        }
    }

    private void OnEnable() => SetCursor(true);
    private void OnDisable() => SetCursor(false);
    private void OnApplicationFocus(bool focused)
    {
        if (!focused) SetCursor(false);
    }

    private static void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        bool capturedThisFrame = false;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isInspecting)
                isInspecting = false;
            else
                SetCursor(false);
        }
        else if (Application.isFocused && mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            capturedThisFrame = Cursor.lockState != CursorLockMode.Locked;
            SetCursor(true);
        }

        bool interactPressed = (keyboard != null && keyboard.eKey.wasPressedThisFrame) ||
                               (mouse != null && mouse.leftButton.wasPressedThisFrame && !capturedThisFrame);
        bool harvestPressed = keyboard != null && keyboard.fKey.wasPressedThisFrame;

        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
        }

        if (chopImpactTimer > 0f)
        {
            chopImpactTimer -= Time.deltaTime;
            view.localPosition = defaultCameraLocalPos + Random.insideUnitSphere * 0.035f;
        }
        else if (view != null)
        {
            view.localPosition = defaultCameraLocalPos;
        }

        Vector2 movement = Vector2.zero;
        bool jumpPressed = false;
        bool running = false;
        if (Application.isFocused && Cursor.lockState == CursorLockMode.Locked)
        {
            if (keyboard != null)
            {
                movement.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
                movement.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
                movement = Vector2.ClampMagnitude(movement, 1f);
                jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
                running = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            }
            if (mouse != null && !capturedThisFrame)
            {
                // Mouse delta is already a per-frame displacement; do not multiply by deltaTime.
                Vector2 rawDelta = mouse.delta.ReadValue();
                bool inRecentreGrace = recentreGraceFrames > 0;
                if (inRecentreGrace)
                {
                    recentreGraceFrames--;
                    // A cursor warp is reported as a delta on the next frame or two;
                    // no real hand movement reaches that far in a single frame.
                    if (rawDelta.magnitude > 250f)
                        rawDelta = Vector2.zero;
                }

                Vector2 look = rawDelta * mouseSensitivity;
                transform.Rotate(0f, look.x, 0f);
                pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
                view.localRotation = Quaternion.Euler(pitch, 0f, 0f);
                lookPixelsSinceRecentre += rawDelta.magnitude;

                // On some Linux setups a locked cursor stops producing deltas once the
                // pointer reaches a screen edge. Recentre before that can happen so the
                // view can keep turning in one direction indefinitely; the periodic
                // recentre also recovers if the pointer starts a session at an edge.
                secondsSinceRecentre += Time.deltaTime;
                if (lookPixelsSinceRecentre >= Mathf.Min(Screen.width, Screen.height) * 0.25f ||
                    secondsSinceRecentre >= 2f)
                {
                    lookPixelsSinceRecentre = 0f;
                    secondsSinceRecentre = 0f;
                    mouse.WarpCursorPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
                    recentreGraceFrames = 2;
                }
            }
        }

        bool grounded = controller.isGrounded && verticalSpeed <= 0f;
        if (grounded)
        {
            verticalSpeed = -2f;
            airJumpAvailable = false;
        }
        if (jumpPressed)
        {
            if (grounded)
            {
                verticalSpeed = Mathf.Sqrt(2f * Gravity * jumpHeight);
                airJumpAvailable = true;
            }
            else if (airJumpAvailable)
            {
                // One fresh upward impulse per jump; landing resets the allowance.
                verticalSpeed = Mathf.Sqrt(2f * Gravity * secondJumpHeight);
                airJumpAvailable = false;
            }
        }
        verticalSpeed = Mathf.Max(verticalSpeed - Gravity * Time.deltaTime, -40f);
        float speed = running ? runSpeed : moveSpeed;
        Vector3 velocity = (transform.right * movement.x + transform.forward * movement.y) * speed;
        velocity.y = verticalSpeed;
        CollisionFlags collisions = controller.Move(velocity * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0 && verticalSpeed > 0f)
            verticalSpeed = 0f;

        UpdateTreeInspection(interactPressed, harvestPressed);
    }

    private void UpdateTreeInspection(bool interactPressed, bool harvestPressed)
    {
        isLookingAtTree = false;
        aimedTree = null;

        if (view == null || Cursor.lockState != CursorLockMode.Locked)
        {
            isInspecting = false;
            return;
        }

        // Cast a ray forward from the camera's eye position, ignoring the player's own collider
        int hitCount = Physics.RaycastNonAlloc(view.position, view.forward, hitBuffer, interactionDistance);
        bool found = false;
        RaycastHit nearest = default;
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit candidate = hitBuffer[i];
            Transform candidateTransform = candidate.collider.transform;
            if (candidateTransform == transform || candidateTransform.IsChildOf(transform))
                continue;
            if (!found || candidate.distance < nearest.distance)
            {
                nearest = candidate;
                found = true;
            }
        }

        if (found)
        {
            ForestTree tree = nearest.collider.GetComponentInParent<ForestTree>();
            if (tree != null)
            {
                isLookingAtTree = true;
                aimedTree = tree;
            }
        }

        // If inspecting, close card if player steps or looks too far away
        if (isInspecting)
        {
            if (inspectedTree == null || Vector3.Distance(view.position, inspectedTree.InteractionPoint) > interactionDistance + 1.5f)
            {
                isInspecting = false;
            }
            else if (harvestPressed && inspectedTree.CanChop)
            {
                ChopTree(inspectedTree);
            }
            else if (interactPressed)
            {
                isInspecting = false;
            }
        }
        else if (isLookingAtTree && aimedTree != null)
        {
            if (harvestPressed && aimedTree.CanChop)
            {
                ChopTree(aimedTree);
            }
            else if (interactPressed)
            {
                InspectTree(aimedTree);
            }
        }
    }

    private void InspectTree(ForestTree tree)
    {
        inspectedTree = tree;
        inspectedTreeName = tree.gameObject.name;
        inspectedTreeHeight = tree.Height;
        inspectedTreeDiameter = tree.Diameter;
        isInspecting = true;
    }

    private void ChopTree(ForestTree tree)
    {
        if (tree == null || !tree.CanChop) return;

        // Enforce axe swing cooldown for deliberate, physical rhythm
        if (Time.time < lastChopTime + swingCooldown) return;
        lastChopTime = Time.time;

        // Camera impact recoil
        chopImpactTimer = 0.12f;

        // The felling stroke needs room for the whole yield; partial collection
        // is not allowed, and the tree keeps its existing chop progress.
        int nextChops = tree.ChopProgress + 1;
        if (nextChops >= tree.ChopsRequired && !CanCarryWood(tree.WoodYield))
        {
            lastHarvestMessage = $"Need {tree.WoodYield} free wood capacity. Free space: {FreeWoodCapacity}.";
            messageTimer = 3.5f;
            return;
        }

        int currentChops = tree.AddChop();

        if (currentChops < tree.ChopsRequired)
        {
            lastHarvestMessage = $"Axe Chop! ({currentChops}/{tree.ChopsRequired})";
            messageTimer = 1.2f;
        }
        else
        {
            // Read the yield before falling, because falling shrinks the trunk
            int yield = tree.WoodYield;
            tree.Fell();
            TryAddWood(yield);

            lastHarvestMessage = $"Timber! +{yield} Wood collected from {tree.gameObject.name}";
            messageTimer = 3.5f;

            isInspecting = false;
        }
    }

    private void OnGUI()
    {
        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;

        // Scale the counter with the game resolution so a downscaled Game view stays readable.
        Matrix4x4 previousMatrix = GUI.matrix;
        float hudScale = Mathf.Max(1f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));
        GUI.matrix = previousMatrix * Matrix4x4.Scale(new Vector3(hudScale, hudScale, 1f));
        // Top-left wood inventory HUD
        float hudWidth = 380f;
        float hudHeight = 92f;
        Rect hudRect = new Rect(16f, 16f, hudWidth, hudHeight);
        // Keep inventory visible even when Escape or loss of focus releases the mouse.
        Color previousColor = GUI.color;
        GUI.color = new Color(0.06f, 0.09f, 0.05f, 0.95f);
        GUI.DrawTexture(hudRect, Texture2D.whiteTexture);
        GUI.color = Color.white;
        if (hudLabelStyle == null)
            hudLabelStyle = new GUIStyle(GUI.skin.label);
        // Re-applied every frame so a stale cached style can never render the counter dark or small.
        hudLabelStyle.fontSize = 22;
        hudLabelStyle.fontStyle = FontStyle.Normal;
        hudLabelStyle.alignment = TextAnchor.MiddleLeft;
        hudLabelStyle.normal.textColor = new Color(0.78f, 0.86f, 0.72f);
        GUI.Label(new Rect(hudRect.x + 28f, hudRect.y + 10f, hudWidth - 40f, 26f), "Carried Wood", hudLabelStyle);

        bool atCapacity = carriedWood >= maxCarriedWood;
        if (hudValueStyle == null)
            hudValueStyle = new GUIStyle(GUI.skin.label);
        hudValueStyle.fontSize = 40;
        hudValueStyle.fontStyle = FontStyle.Bold;
        hudValueStyle.alignment = TextAnchor.MiddleLeft;
        hudValueStyle.normal.textColor = atCapacity ? new Color(1f, 0.62f, 0.4f) : Color.white;
        GUI.Label(new Rect(hudRect.x + 28f, hudRect.y + 34f, hudWidth - 40f, 52f), $"{carriedWood} / {maxCarriedWood}", hudValueStyle);
        GUI.color = previousColor;
        GUI.matrix = previousMatrix;

        // Floating harvest notification, kept clear of the enlarged HUD
        if (messageTimer > 0f)
        {
            if (notificationStyle == null)
            {
                notificationStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 36,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                notificationStyle.normal.textColor = new Color(1f, 0.95f, 0.55f);
            }

            float noteWidth = Mathf.Min(800f, Screen.width - 32f);
            float noteHeight = 92f;
            float noteY = 16f + hudHeight * hudScale + 8f;
            GUI.Box(new Rect(centerX - noteWidth * 0.5f, noteY, noteWidth, noteHeight), lastHarvestMessage, notificationStyle);
        }

        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Small center reticle to help aiming
        GUI.Box(new Rect(centerX - 2f, centerY - 2f, 4f, 4f), GUIContent.none);

        if (isInspecting)
        {
            DrawInspectionCard(centerX, centerY);
        }
        else if (isLookingAtTree)
        {
            int size = promptFontSize >= 18 ? promptFontSize : 36;
            if (promptStyle == null || promptStyle.fontSize != size)
            {
                promptStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = size,
                    fontStyle = FontStyle.Bold,
                    wordWrap = false
                };
                promptStyle.normal.textColor = Color.white;
            }

            string promptText;
            if (aimedTree.CanChop)
            {
                int chops = aimedTree.ChopProgress;
                promptText = chops > 0
                    ? $"[E] Inspect  |  [F] Chop ({chops}/{aimedTree.ChopsRequired})"
                    : "[E] Inspect  |  [F] Chop Tree";
            }
            else
            {
                promptText = $"[E] Inspect {aimedTree.StageLabel}";
            }

            float width = Mathf.Max(340f, promptText.Length * size * 0.52f);
            float height = Mathf.Max(58f, size * 1.8f);
            GUI.Box(new Rect(centerX - width * 0.5f, centerY + 40f, width, height), promptText, promptStyle);
        }
    }

    private void DrawInspectionCard(float centerX, float centerY)
    {
        if (cardStyle == null)
        {
            cardStyle = new GUIStyle(GUI.skin.box);
            cardTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            cardTitleStyle.normal.textColor = new Color(0.95f, 0.85f, 0.45f);

            cardBodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                wordWrap = true,
                alignment = TextAnchor.UpperLeft
            };
            cardBodyStyle.normal.textColor = Color.white;

            cardFooterStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter
            };
            cardFooterStyle.normal.textColor = new Color(0.75f, 0.85f, 0.95f);
        }

        float cardWidth = 500f;
        float cardHeight = 250f;
        Rect cardRect = new Rect(centerX - cardWidth * 0.5f, centerY - cardHeight * 0.5f, cardWidth, cardHeight);
        GUI.Box(cardRect, GUIContent.none, cardStyle);

        GUILayout.BeginArea(new Rect(cardRect.x + 20f, cardRect.y + 16f, cardWidth - 40f, cardHeight - 32f));
        GUILayout.Label($"🌲 Tree Inspection — {inspectedTreeName}", cardTitleStyle);
        GUILayout.Space(10);
        string speciesName = inspectedTree != null && inspectedTree.Species != null
            ? inspectedTree.Species.FullName
            : "Unknown species";
        GUILayout.Label($"• Species: {speciesName}", cardBodyStyle);
        GUILayout.Label($"• Status: {inspectedTree.StageLabel}", cardBodyStyle);
        GUILayout.Label($"• Estimated Height: {inspectedTreeHeight:F1} m", cardBodyStyle);
        GUILayout.Label($"• Trunk Diameter: {inspectedTreeDiameter:F0} cm", cardBodyStyle);

        if (!inspectedTree.IsStump)
            GUILayout.Label($"• Standing Volume: {inspectedTree.BiologicalStemVolumeM3:F2} m³", cardBodyStyle);

        if (inspectedTree.IsStump)
        {
            GUILayout.Label("• CCF note: canopy gap created; future regeneration will grow from new individuals.", cardBodyStyle);
        }
        else
        {
            string ccfNote = inspectedTree.Stage == ForestTreeStage.Mature
                ? "• CCF Status: Mature canopy tree — candidate for selective single-tree thinning."
                : "• CCF Status: Young growing stock — retain for continuous crown cover.";
            GUILayout.Label(ccfNote, cardBodyStyle);

            if (inspectedTree.ChopProgress > 0)
            {
                GUILayout.Label($"• Chopping Progress: {inspectedTree.ChopProgress} / {inspectedTree.ChopsRequired} chops", cardBodyStyle);
            }

            GUILayout.Label($"• Potential Wood Yield: {inspectedTree.WoodYield} Wood", cardBodyStyle);
        }

        GUILayout.FlexibleSpace();
        GUILayout.Label(inspectedTree.CanChop
            ? "Press [F] to Chop   |   Press [E] to Close"
            : "Press [E], [Left Click], or step away to close", cardFooterStyle);
        GUILayout.EndArea();
    }
}

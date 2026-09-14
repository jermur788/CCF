using System.Collections.Generic;
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
    [SerializeField, Range(1, 10)] private int chopsRequired = 4;
    [SerializeField, Min(0.1f)] private float swingCooldown = 0.45f;
    [SerializeField] private int woodCount = 0;
    private CharacterController controller;
    private float pitch;
    private float verticalSpeed;
    private bool isLookingAtTree;
    private bool isLookingAtStump;
    private Transform aimedTrunkTransform;
    private bool isInspecting;
    private Transform inspectedTreeTransform;
    private string inspectedTreeName = "";
    private float inspectedTreeHeight;
    private float inspectedTreeDiameter;
    private readonly HashSet<Transform> harvestedTrees = new HashSet<Transform>();
    private readonly Dictionary<Transform, int> treeChopProgress = new Dictionary<Transform, int>();
    private float lastChopTime = -1f;
    private float chopImpactTimer;
    private Vector3 defaultCameraLocalPos = new Vector3(0f, 1.65f, 0f);
    private string lastHarvestMessage = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle cardStyle;
    private GUIStyle cardTitleStyle;
    private GUIStyle cardBodyStyle;
    private GUIStyle cardFooterStyle;
    private GUIStyle hudTextStyle;
    private GUIStyle notificationStyle;

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
                Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
                transform.Rotate(0f, look.x, 0f);
                pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
                view.localRotation = Quaternion.Euler(pitch, 0f, 0f);
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

    private bool IsStump(Transform trunk)
    {
        if (trunk == null) return false;
        Transform root = trunk.parent != null && trunk.parent.name.StartsWith("Tree") ? trunk.parent : trunk;
        return harvestedTrees.Contains(root) || trunk.localScale.y <= 0.25f;
    }

    private int GetChopCount(Transform trunk)
    {
        if (trunk == null) return 0;
        Transform root = trunk.parent != null && trunk.parent.name.StartsWith("Tree") ? trunk.parent : trunk;
        return treeChopProgress.TryGetValue(root, out int count) ? count : 0;
    }

    private void UpdateTreeInspection(bool interactPressed, bool harvestPressed)
    {
        isLookingAtTree = false;
        isLookingAtStump = false;
        aimedTrunkTransform = null;

        if (view == null || Cursor.lockState != CursorLockMode.Locked)
        {
            isInspecting = false;
            return;
        }

        // Cast a ray forward from the camera's eye position
        if (Physics.Raycast(view.position, view.forward, out RaycastHit hit, interactionDistance))
        {
            // Trees in ForestTest are structured as a parent "Tree X" with a child "Trunk" collider
            bool isTrunk = hit.collider != null && (hit.collider.name == "Trunk" ||
                (hit.collider.transform.parent != null && hit.collider.transform.parent.name.StartsWith("Tree")));
            if (isTrunk)
            {
                isLookingAtTree = true;
                aimedTrunkTransform = hit.collider.transform;
                isLookingAtStump = IsStump(aimedTrunkTransform);
            }
        }

        // If inspecting, close card if player steps or looks too far away
        if (isInspecting)
        {
            if (inspectedTreeTransform == null || Vector3.Distance(view.position, inspectedTreeTransform.position) > interactionDistance + 1.5f)
            {
                isInspecting = false;
            }
            else if (harvestPressed && !IsStump(inspectedTreeTransform))
            {
                ChopTree(inspectedTreeTransform);
            }
            else if (interactPressed)
            {
                isInspecting = false;
            }
        }
        else if (isLookingAtTree && aimedTrunkTransform != null)
        {
            if (harvestPressed && !isLookingAtStump)
            {
                ChopTree(aimedTrunkTransform);
            }
            else if (interactPressed)
            {
                InspectTree(aimedTrunkTransform);
            }
        }
    }

    private void InspectTree(Transform trunk)
    {
        inspectedTreeTransform = trunk;
        Transform root = trunk.parent != null && trunk.parent.name.StartsWith("Tree") ? trunk.parent : trunk;
        inspectedTreeName = root.name;

        // In this prototype, cylinder trunk localScale.y is half-height
        inspectedTreeHeight = trunk.localScale.y * 2f;
        // Trunk localScale.x is radius/diameter factor
        inspectedTreeDiameter = trunk.localScale.x * 100f;
        isInspecting = true;
    }

    private void ChopTree(Transform trunk)
    {
        if (trunk == null || IsStump(trunk)) return;

        // Enforce axe swing cooldown for deliberate, physical rhythm
        if (Time.time < lastChopTime + swingCooldown) return;
        lastChopTime = Time.time;

        Transform root = trunk.parent != null && trunk.parent.name.StartsWith("Tree") ? trunk.parent : trunk;
        int currentChops = GetChopCount(trunk) + 1;
        treeChopProgress[root] = currentChops;

        // Camera impact recoil
        chopImpactTimer = 0.12f;

        if (currentChops < chopsRequired)
        {
            lastHarvestMessage = $"Axe Chop! ({currentChops}/{chopsRequired})";
            messageTimer = 1.2f;
        }
        else
        {
            FellTree(trunk, root);
        }
    }

    private void FellTree(Transform trunk, Transform root)
    {
        treeChopProgress.Remove(root);
        harvestedTrees.Add(root);

        // Calculate wood yield proportional to tree height (3 to 10 logs)
        float currentHeight = trunk.localScale.y * 2f;
        int yield = Mathf.Clamp(Mathf.RoundToInt(currentHeight), 3, 10);
        woodCount += yield;

        lastHarvestMessage = $"Timber! +{yield} Wood collected from {root.name}";
        messageTimer = 3.5f;

        // Remove canopy leaves
        Transform canopy = root.Find("Canopy");
        if (canopy != null)
        {
            Destroy(canopy.gameObject);
        }

        // Reduce trunk to a realistic stump (~0.35m high)
        float stumpHeight = 0.35f;
        trunk.localScale = new Vector3(trunk.localScale.x * 1.15f, stumpHeight * 0.5f, trunk.localScale.z * 1.15f);
        trunk.localPosition = new Vector3(trunk.localPosition.x, stumpHeight * 0.5f, trunk.localPosition.z);

        if (!root.name.Contains("(Stump)"))
        {
            root.name += " (Stump)";
        }

        isInspecting = false;
        isLookingAtStump = true;
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
        float hudWidth = 190f;
        float hudHeight = 46f;
        Rect hudRect = new Rect(16f, 16f, hudWidth, hudHeight);
        // Keep inventory visible even when Escape or loss of focus releases the mouse.
        Color previousColor = GUI.color;
        GUI.color = new Color(0.06f, 0.09f, 0.05f, 0.95f);
        GUI.DrawTexture(hudRect, Texture2D.whiteTexture);
        GUI.color = Color.white;
        if (hudTextStyle == null)
        {
            hudTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            hudTextStyle.normal.textColor = Color.white;
        }
        GUI.Label(new Rect(hudRect.x + 14f, hudRect.y + 6f, hudWidth - 20f, 32f), $"Wood: {woodCount}", hudTextStyle);
        GUI.color = previousColor;
        GUI.matrix = previousMatrix;

        // Floating harvest notification
        if (messageTimer > 0f)
        {
            if (notificationStyle == null)
            {
                notificationStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                notificationStyle.normal.textColor = new Color(1f, 0.95f, 0.55f);
            }

            float noteWidth = 400f;
            float noteHeight = 46f;
            GUI.Box(new Rect(centerX - noteWidth * 0.5f, 30f, noteWidth, noteHeight), lastHarvestMessage, notificationStyle);
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
            if (isLookingAtStump)
            {
                promptText = "[E] Inspect Stump (Harvested)";
            }
            else
            {
                int chops = GetChopCount(aimedTrunkTransform);
                promptText = chops > 0
                    ? $"[E] Inspect  |  [F] Chop ({chops}/{chopsRequired})"
                    : $"[E] Inspect  |  [F] Chop Tree";
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

        bool isStump = IsStump(inspectedTreeTransform);
        GUILayout.BeginArea(new Rect(cardRect.x + 20f, cardRect.y + 16f, cardWidth - 40f, cardHeight - 32f));
        GUILayout.Label($"🌲 Tree Inspection — {inspectedTreeName}", cardTitleStyle);
        GUILayout.Space(10);
        GUILayout.Label("• Species: Scots Pine (Pinus sylvestris)", cardBodyStyle);

        if (isStump)
        {
            GUILayout.Label("• Status: Harvested tree stump", cardBodyStyle);
            GUILayout.Label("• CCF Ecology: Canopy gap created; promotes seed germination and natural succession.", cardBodyStyle);
            GUILayout.Label("• Wood Yield: Already harvested", cardBodyStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Press [E], [Left Click], or step away to close", cardFooterStyle);
        }
        else
        {
            GUILayout.Label($"• Estimated Height: {inspectedTreeHeight:F1} m", cardBodyStyle);
            GUILayout.Label($"• Trunk Diameter: {inspectedTreeDiameter:F0} cm", cardBodyStyle);

            string ccfNote = inspectedTreeHeight >= 4.5f
                ? "• CCF Status: Mature canopy tree — candidate for selective single-tree thinning."
                : "• CCF Status: Young growing stock — retain for continuous crown cover.";
            GUILayout.Label(ccfNote, cardBodyStyle);

            int chops = GetChopCount(inspectedTreeTransform);
            if (chops > 0)
            {
                GUILayout.Label($"• Chopping Progress: {chops} / {chopsRequired} chops", cardBodyStyle);
            }

            int estimatedYield = Mathf.Clamp(Mathf.RoundToInt(inspectedTreeHeight), 3, 10);
            GUILayout.Label($"• Potential Wood Yield: {estimatedYield} Wood", cardBodyStyle);

            GUILayout.FlexibleSpace();
            GUILayout.Label("Press [F] to Chop   |   Press [E] to Close", cardFooterStyle);
        }
        GUILayout.EndArea();
    }
}

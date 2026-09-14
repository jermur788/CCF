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
    private CharacterController controller;
    private float pitch;
    private float verticalSpeed;
    private bool isLookingAtTree;
    private GUIStyle promptStyle;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (view == null)
        {
            Debug.LogError("ForestPlayer requires a camera transform.", this);
            enabled = false;
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
            SetCursor(false);
        else if (Application.isFocused && mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            capturedThisFrame = Cursor.lockState != CursorLockMode.Locked;
            SetCursor(true);
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

        UpdateTreeInspection();
    }

    private void UpdateTreeInspection()
    {
        isLookingAtTree = false;
        if (view == null || Cursor.lockState != CursorLockMode.Locked) return;

        // Cast a ray forward from the camera's eye position
        if (Physics.Raycast(view.position, view.forward, out RaycastHit hit, interactionDistance))
        {
            // Trees in ForestTest are structured as a parent "Tree X" with a child "Trunk" collider
            bool isTrunk = hit.collider != null && (hit.collider.name == "Trunk" ||
                (hit.collider.transform.parent != null && hit.collider.transform.parent.name.StartsWith("Tree")));
            if (isTrunk)
            {
                isLookingAtTree = true;
            }
        }
    }

    private void OnGUI()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;

        // Small center reticle to help aiming
        GUI.Box(new Rect(centerX - 2f, centerY - 2f, 4f, 4f), GUIContent.none);

        if (isLookingAtTree)
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

            float width = Mathf.Max(260f, size * 9f);
            float height = Mathf.Max(58f, size * 1.8f);
            GUI.Box(new Rect(centerX - width * 0.5f, centerY + 40f, width, height), "Inspect Tree", promptStyle);
        }
    }
}

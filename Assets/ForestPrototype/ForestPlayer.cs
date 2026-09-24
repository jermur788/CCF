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
    // [D] gameplay calibration: biological stem volume converts to carried wood units here,
    // keeping Forestry's biological numbers separate from the survival economy.
    [SerializeField, Min(0.01f)] private float cubicMetersPerWoodUnit = 0.1f;
    [SerializeField] private int carriedWood = 0;
    [Header("Regeneration Uprooting")]
    [Tooltip("[D] Seconds required to pull up the sparsest regeneration cohort.")]
    [SerializeField, Min(0.1f)] private float uprootMinDuration = 1f;
    [Tooltip("[D] Seconds required to pull up a cohort at its species maximum density.")]
    [SerializeField, Min(0.1f)] private float uprootMaxDuration = 5f;
    private CharacterController controller;
    private float pitch;
    private float verticalSpeed;
    private bool isLookingAtTree;
    private ForestTree aimedTree;
    private bool isAimingGround;
    private Vector3 aimedSurfacePoint;
    private ForestEcologyController aimedEcology;
    private RegenerationQueryResult aimedRegeneration;
    private int aimedRegenerationCellIndex = -1;
    private string selectedRegenerationSpeciesId = "";
    private int selectedRegenerationIndex = -1;
    private bool isUprooting;
    private bool uprootNeedsRelease;
    private float uprootProgress;
    private float uprootDuration;
    private int uprootTargetCellIndex = -1;
    private string uprootTargetSpeciesId = "";
    private TreeSpeciesDefinition uprootTargetSpecies;
    private Vector3 uprootTargetPoint;
    private bool isInspecting;
    private ForestTree inspectedTree;
    private ForestEcologyController inspectedTreeEcology;
    private ForestTreeMarkingManager inspectedTreeMarking;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];
    private string inspectedTreeName = "";
    private float inspectedTreeHeight;
    private float inspectedTreeDiameter;
    private float lastChopTime = -1f;
    private float lookPixelsSinceRecentre;
    private float secondsSinceRecentre;
    private float secondsSinceEdgeStall;
    private int recentreGraceFrames;
    private float chopImpactTimer;
    private Vector3 defaultCameraLocalPos = new Vector3(0f, 1.65f, 0f);
    private string lastHarvestMessage = "";
    private float messageTimer;
    private GUIStyle promptStyle;
    private GUIStyle cardTitleStyle;
    private GUIStyle cardBodyStyle;
    private GUIStyle cardFooterStyle;
    private GUIStyle hudLabelStyle;
    private GUIStyle hudValueStyle;
    private GUIStyle notificationStyle;

    public int MaxCarriedWood => maxCarriedWood + BuiltCapacityBonus();
    // Read-only view for other systems (the marking HUD hides its prompt while
    // the inspection card covers the crosshair).
    public bool IsInspecting => isInspecting;
    public float EffectiveSwingCooldown => Mathf.Max(0.15f, swingCooldown - BuiltSwingCooldownReduction());

    // Built structures can raise the carrying limit and speed up axe recovery;
    // both bonuses are derived from built objects so they survive save/load
    // without their own save fields.
    private int BuiltCapacityBonus()
    {
        int bonus = 0;
        foreach (ForestBuildable buildable in Object.FindObjectsByType<ForestBuildable>())
        {
            if (buildable != null && buildable.IsBuilt)
                bonus += buildable.CarriedWoodCapacityBonus;
        }
        return bonus;
    }

    private float BuiltSwingCooldownReduction()
    {
        float reduction = 0f;
        foreach (ForestBuildable buildable in Object.FindObjectsByType<ForestBuildable>())
        {
            if (buildable != null && buildable.IsBuilt)
                reduction += buildable.SwingCooldownReduction;
        }
        return reduction;
    }
    public int CarriedWood => carriedWood;
    public int FreeWoodCapacity => Mathf.Max(0, MaxCarriedWood - carriedWood);

    public bool CanCarryWood(int amount)
    {
        return amount >= 0 && carriedWood + amount <= MaxCarriedWood;
    }

    // Survival-side conversion: biological stem volume -> carried wood units.
    // Floor 3 keeps very young trees worth a stroke; the ceiling 24 [D] lets
    // volume differentiate again (the old 3..10 clamp made every tree above
    // ~1 m3 give the same 10). A 24-unit trunk needs the Timber Sledge upgrade
    // (capacity 35) or a big empty inventory to collect in one stroke.
    private int TimberUnits(ForestTree tree)
    {
        return Mathf.Clamp(Mathf.RoundToInt(tree.BiologicalStemVolumeM3 / cubicMetersPerWoodUnit), 3, 24);
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

    private void OnEnable()
    {
        SetCursor(true);
        // Locking warps the pointer to the window centre; the resulting delta
        // spike must not rotate the view at the start of a session.
        recentreGraceFrames = 2;
    }
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
            // The lock warp produces a delta spike on the next frame or two;
            // suppress it so clicking to capture never throws the view.
            if (capturedThisFrame)
                recentreGraceFrames = 2;
        }

        bool interactPressed = (keyboard != null && keyboard.eKey.wasPressedThisFrame) ||
                               (mouse != null && mouse.leftButton.wasPressedThisFrame && !capturedThisFrame);
        bool harvestPressed = keyboard != null && keyboard.fKey.wasPressedThisFrame;
        bool plantPressed = keyboard != null && keyboard.gKey.wasPressedThisFrame;
        bool uprootHeld = keyboard != null && keyboard.uKey.isPressed;
        bool cycleRegenerationPressed = keyboard != null && keyboard.rKey.wasPressedThisFrame;

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
                    // A warp is reported as a delta on the next frame or two, and
                    // the OS can split it across frames, so every delta inside
                    // the grace window is discarded. Two frames is imperceptible.
                    rawDelta = Vector2.zero;
                }

                Vector2 look = rawDelta * mouseSensitivity;
                transform.Rotate(0f, look.x, 0f);
                pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
                view.localRotation = Quaternion.Euler(pitch, 0f, 0f);
                lookPixelsSinceRecentre += rawDelta.magnitude;

                // On some Linux setups a locked cursor stops producing deltas once the
                // pointer reaches a screen edge. Recentre only when that stall is
                // actually happening: the pointer has drifted to the edge AND the
                // deltas have gone quiet for a moment. The old unconditional
                // two-second warp yanked the cursor back to the game window even
                // while the player was reading or resting.
                secondsSinceRecentre += Time.deltaTime;
                Vector2 pointerPos = mouse.position.ReadValue();
                bool nearEdge = pointerPos.x < 24f || pointerPos.y < 24f ||
                                pointerPos.x > Screen.width - 24f || pointerPos.y > Screen.height - 24f;
                if (rawDelta.magnitude < 0.5f)
                    secondsSinceEdgeStall += Time.deltaTime;
                else
                    secondsSinceEdgeStall = 0f;
                bool crossedPixelBudget = lookPixelsSinceRecentre >= Mathf.Min(Screen.width, Screen.height) * 0.25f;
                if (crossedPixelBudget || (nearEdge && secondsSinceEdgeStall >= 1.5f && secondsSinceRecentre >= 1.5f))
                {
                    lookPixelsSinceRecentre = 0f;
                    secondsSinceRecentre = 0f;
                    secondsSinceEdgeStall = 0f;
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

        UpdateTreeInspection(interactPressed, harvestPressed, plantPressed,
            uprootHeld, cycleRegenerationPressed);
    }

    private void UpdateTreeInspection(bool interactPressed, bool harvestPressed, bool plantPressed,
        bool uprootHeld, bool cycleRegenerationPressed)
    {
        isLookingAtTree = false;
        aimedTree = null;
        isAimingGround = false;

        if (view == null || Cursor.lockState != CursorLockMode.Locked)
        {
            isInspecting = false;
            ClearAimedRegeneration();
            CancelUprooting();
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

        // Planting needs the aimed ground surface, not a tree: accept flat,
        // upward-facing non-tree, non-structure surfaces within reach.
        isAimingGround = found && !isLookingAtTree
            && Vector3.Dot(nearest.normal, Vector3.up) > 0.7f
            && nearest.collider.GetComponentInParent<ForestBuildable>() == null;
        if (isAimingGround)
        {
            aimedSurfacePoint = nearest.point;
            RefreshAimedRegeneration();
        }
        else
        {
            ClearAimedRegeneration();
        }

        if (cycleRegenerationPressed && isAimingGround && aimedRegeneration.Success
            && aimedRegeneration.Cohorts.Count > 1)
            CycleRegenerationSelection();
        UpdateUprooting(uprootHeld, Time.deltaTime);

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
        else if (plantPressed && isAimingGround)
        {
            PlantBeechAt(aimedSurfacePoint);
            RefreshAimedRegeneration();
        }
    }

    private void RefreshAimedRegeneration()
    {
        if (aimedEcology == null)
            aimedEcology = Object.FindFirstObjectByType<ForestEcologyController>();
        RegenerationQueryResult refreshed = aimedEcology != null
            ? aimedEcology.QueryRegeneration(aimedSurfacePoint)
            : RegenerationQueryResult.Failed(RegenerationQueryOutcome.OutsideEcologyArea,
                "No ecology is available.");

        int previousCell = aimedRegenerationCellIndex;
        string previousSpecies = selectedRegenerationSpeciesId;
        aimedRegeneration = refreshed;
        aimedRegenerationCellIndex = refreshed.CellIndex;

        if (!refreshed.Success || refreshed.Cohorts.Count == 0)
        {
            selectedRegenerationIndex = -1;
            selectedRegenerationSpeciesId = "";
            if (isUprooting)
                CancelUprooting();
            return;
        }

        int selected = -1;
        if (previousCell == refreshed.CellIndex && !string.IsNullOrEmpty(previousSpecies))
        {
            for (int i = 0; i < refreshed.Cohorts.Count; i++)
            {
                if (refreshed.Cohorts[i].SpeciesId == previousSpecies)
                {
                    selected = i;
                    break;
                }
            }
        }
        if (selected < 0)
            selected = 0;

        selectedRegenerationIndex = selected;
        selectedRegenerationSpeciesId = refreshed.Cohorts[selected].SpeciesId;
        if (isUprooting && (previousCell != refreshed.CellIndex
            || uprootTargetSpeciesId != selectedRegenerationSpeciesId))
            CancelUprooting();
    }

    private void ClearAimedRegeneration()
    {
        aimedRegeneration = default;
        aimedRegenerationCellIndex = -1;
        selectedRegenerationIndex = -1;
        selectedRegenerationSpeciesId = "";
    }

    private void CycleRegenerationSelection()
    {
        if (!aimedRegeneration.Success || aimedRegeneration.Cohorts.Count <= 1)
            return;
        selectedRegenerationIndex = (selectedRegenerationIndex + 1) % aimedRegeneration.Cohorts.Count;
        selectedRegenerationSpeciesId = aimedRegeneration.Cohorts[selectedRegenerationIndex].SpeciesId;
        CancelUprooting();
    }

    private bool TryGetSelectedRegeneration(out RegenerationCohortInfo selected)
    {
        selected = default;
        if (!aimedRegeneration.Success || selectedRegenerationIndex < 0
            || selectedRegenerationIndex >= aimedRegeneration.Cohorts.Count)
            return false;
        RegenerationCohortInfo candidate = aimedRegeneration.Cohorts[selectedRegenerationIndex];
        if (candidate.Species == null || candidate.Density <= 0f
            || candidate.SpeciesId != selectedRegenerationSpeciesId)
            return false;
        selected = candidate;
        return true;
    }

    private void UpdateUprooting(bool held, float deltaTime)
    {
        if (!held)
        {
            uprootNeedsRelease = false;
            CancelUprooting();
            return;
        }
        if (uprootNeedsRelease)
            return;
        if (isInspecting || isLookingAtTree || !isAimingGround || aimedEcology == null
            || !TryGetSelectedRegeneration(out RegenerationCohortInfo selected))
        {
            CancelUprooting();
            return;
        }

        if (!isUprooting)
        {
            float densityFraction = selected.Density / Mathf.Max(0.01f, selected.Species.RegenDensityMax);
            float minimum = Mathf.Max(0.1f, Mathf.Min(uprootMinDuration, uprootMaxDuration));
            float maximum = Mathf.Max(minimum, Mathf.Max(uprootMinDuration, uprootMaxDuration));
            uprootDuration = minimum + (maximum - minimum) * Mathf.Clamp01(densityFraction);
            uprootProgress = 0f;
            uprootTargetCellIndex = aimedRegenerationCellIndex;
            uprootTargetSpeciesId = selected.SpeciesId;
            uprootTargetSpecies = selected.Species;
            uprootTargetPoint = aimedSurfacePoint;
            isUprooting = true;
        }

        bool targetStillValid = aimedRegenerationCellIndex == uprootTargetCellIndex
            && selectedRegenerationSpeciesId == uprootTargetSpeciesId
            && selected.Species == uprootTargetSpecies;
        if (!targetStillValid)
        {
            CancelUprooting();
            return;
        }

        uprootProgress += Mathf.Max(0f, deltaTime);
        if (uprootProgress < uprootDuration)
            return;

        // Latch before calling Forestry so one continuous hold can complete
        // exactly once. A new cohort requires a fresh button press.
        uprootNeedsRelease = true;
        isUprooting = false;
        UprootingResult result = aimedEcology.TryUprootRegeneration(uprootTargetPoint, uprootTargetSpecies);
        lastHarvestMessage = result.Message;
        messageTimer = 3.5f;
        ResetUprootingProgress();
        RefreshAimedRegeneration();
    }

    private void CancelUprooting()
    {
        if (!isUprooting && uprootProgress <= 0f)
            return;
        isUprooting = false;
        ResetUprootingProgress();
    }

    private void ResetUprootingProgress()
    {
        uprootProgress = 0f;
        uprootDuration = 0f;
        uprootTargetCellIndex = -1;
        uprootTargetSpeciesId = "";
        uprootTargetSpecies = null;
        uprootTargetPoint = default;
    }

    // Player-facing Beech Planting v1: one explicit action, one planted
    // juvenile. The ecology (never a planting roll) decides survival; both
    // success and failure report the ecology's own player-readable reason.
    private void PlantBeechAt(Vector3 groundPoint)
    {
        ForestEcologyController ecology = Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology == null)
        {
            lastHarvestMessage = "No ecology to plant into.";
            messageTimer = 2.5f;
            return;
        }
        PlantingResult result = ecology.TryPlantBeech(groundPoint);
        lastHarvestMessage = result.Message;
        messageTimer = 3.5f;
    }

    private void InspectTree(ForestTree tree)
    {
        inspectedTree = tree;
        inspectedTreeName = tree.gameObject.name;
        inspectedTreeHeight = tree.Height;
        inspectedTreeDiameter = tree.Diameter;
        inspectedTreeEcology = Object.FindFirstObjectByType<ForestEcologyController>();
        inspectedTreeMarking = Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        isInspecting = true;
    }

    private void ChopTree(ForestTree tree)
    {
        if (tree == null || !tree.CanChop) return;

        // Enforce axe swing cooldown for deliberate, physical rhythm
        if (Time.time < lastChopTime + EffectiveSwingCooldown) return;
        lastChopTime = Time.time;

        // Camera impact recoil
        chopImpactTimer = 0.12f;

        // The felling stroke needs room for the whole yield; partial collection
        // is not allowed, and the tree keeps its existing chop progress.
        int nextChops = tree.ChopProgress + 1;
        int units = TimberUnits(tree);
        if (nextChops >= tree.ChopsRequired && !CanCarryWood(units))
        {
            lastHarvestMessage = $"Need {units} free wood capacity. Free space: {FreeWoodCapacity}.";
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
            tree.Fell();
            TryAddWood(units);

            lastHarvestMessage = $"Timber! +{units} Wood collected from {tree.gameObject.name}";
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

        bool atCapacity = carriedWood >= MaxCarriedWood;
        if (hudValueStyle == null)
            hudValueStyle = new GUIStyle(GUI.skin.label);
        hudValueStyle.fontSize = 40;
        hudValueStyle.fontStyle = FontStyle.Bold;
        hudValueStyle.alignment = TextAnchor.MiddleLeft;
        hudValueStyle.normal.textColor = atCapacity ? new Color(1f, 0.62f, 0.4f) : Color.white;
        GUI.Label(new Rect(hudRect.x + 28f, hudRect.y + 34f, hudWidth - 40f, 52f), $"{carriedWood} / {MaxCarriedWood}", hudValueStyle);
        GUI.color = previousColor;
        GUI.matrix = previousMatrix;

        // Floating harvest notification, kept clear of the enlarged HUD
        if (messageTimer > 0f)
        {
            float noteScale = ForestHud.Scale;
            if (notificationStyle == null)
            {
                notificationStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                notificationStyle.normal.textColor = new Color(1f, 0.95f, 0.55f);
            }
            notificationStyle.fontSize = Mathf.RoundToInt(36f * noteScale);

            float noteWidth = Mathf.Min(800f * noteScale, Screen.width - 32f);
            float noteHeight = 92f * noteScale;
            float noteY = 16f + hudHeight * hudScale + 8f;
            Rect noteRect = new Rect(centerX - noteWidth * 0.5f, noteY, noteWidth, noteHeight);
            ForestHud.Panel(noteRect);
            GUI.Label(noteRect, lastHarvestMessage, notificationStyle);
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
            int size = Mathf.RoundToInt((promptFontSize >= 18 ? promptFontSize : 36) * ForestHud.Scale);
            if (promptStyle == null || promptStyle.fontSize != size)
            {
                promptStyle = new GUIStyle(GUI.skin.label)
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

            float width = Mathf.Max(440f, promptText.Length * size * 0.52f + 48f);
            float height = Mathf.Max(64f, size * 2.0f);
            Rect promptRect = new Rect(centerX - width * 0.5f, centerY + 40f, width, height);
            ForestHud.Panel(promptRect);
            GUI.Label(promptRect, promptText, promptStyle);
        }
        else if (isAimingGround)
        {
            int size = Mathf.RoundToInt((promptFontSize >= 18 ? promptFontSize : 36) * ForestHud.Scale);
            if (promptStyle == null || promptStyle.fontSize != size)
            {
                promptStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = size,
                    fontStyle = FontStyle.Bold,
                    wordWrap = false
                };
                promptStyle.normal.textColor = Color.white;
            }

            string promptText = "[G] Plant Beech";
            if (TryGetSelectedRegeneration(out RegenerationCohortInfo selected))
            {
                string progress = isUprooting && uprootDuration > 0f
                    ? $"  {Mathf.Clamp01(uprootProgress / uprootDuration):P0}"
                    : "";
                string selection = aimedRegeneration.Cohorts.Count > 1
                    ? $"Selected: {selected.DisplayName}  |  [R] Cycle species\n"
                    : "";
                promptText = selection
                    + $"[Hold U] Pull up {selected.DisplayName} seedlings{progress}\n"
                    + "[G] Plant Beech";
            }
            float width = Mathf.Max(520f, promptText.Length * size * 0.28f + 48f);
            float height = Mathf.Max(64f, size * (promptText.Contains("\n") ? 3.8f : 2.0f));
            Rect promptRect = new Rect(centerX - width * 0.5f, centerY + 40f, width, height);
            ForestHud.Panel(promptRect);
            GUI.Label(promptRect, promptText, promptStyle);
        }
    }

    private void DrawInspectionCard(float centerX, float centerY)
    {
        if (cardTitleStyle == null)
        {
            cardTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            cardTitleStyle.normal.textColor = new Color(0.95f, 0.85f, 0.45f);

            cardBodyStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                alignment = TextAnchor.UpperLeft
            };
            cardBodyStyle.normal.textColor = Color.white;

            cardFooterStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter
            };
            cardFooterStyle.normal.textColor = new Color(0.75f, 0.85f, 0.95f);
        }

        float cardScale = ForestHud.Scale;
        // Re-applied every frame so a stale cached style can never render the card small.
        cardTitleStyle.fontSize = Mathf.RoundToInt(22f * cardScale);
        cardBodyStyle.fontSize = Mathf.RoundToInt(17f * cardScale);
        cardFooterStyle.fontSize = Mathf.RoundToInt(15f * cardScale);
        float cardWidth = 580f * cardScale;
        float cardHeight = 500f * cardScale;
        Rect cardRect = new Rect(centerX - cardWidth * 0.5f, centerY - cardHeight * 0.5f, cardWidth, cardHeight);
        ForestHud.Panel(cardRect);

        GUILayout.BeginArea(new Rect(cardRect.x + 20f, cardRect.y + 16f, cardWidth - 40f, cardHeight - 32f));
        GUILayout.Label($"🌲 Tree Inspection — {inspectedTreeName}", cardTitleStyle);
        GUILayout.Space(10);
        string speciesName = inspectedTree != null && inspectedTree.Species != null
            ? inspectedTree.Species.FullName
            : "Unknown species";
        bool markedForHarvest = inspectedTreeMarking != null && inspectedTreeMarking.IsMarked(inspectedTree);
        GUILayout.Label($"• Species: {speciesName}", cardBodyStyle);
        GUILayout.Label($"• Status: {(inspectedTree.IsStump ? "Harvested stump" : "Living tree")}{(markedForHarvest ? " — MARKED for harvest (M to unmark)" : "")}", cardBodyStyle);
        GUILayout.Label($"• Age: {inspectedTree.AgeYears} years", cardBodyStyle);
        if (!inspectedTree.IsStump)
            GUILayout.Label($"• Size: {inspectedTree.SizeClassLabel}", cardBodyStyle);
        GUILayout.Label($"• Recorded suppression: {inspectedTree.EquivalentSuppressedYears:F2} equivalent years", cardBodyStyle);
        GUILayout.Label($"• Estimated Height: {inspectedTree.Height:F1} m", cardBodyStyle);
        GUILayout.Label($"• Trunk Diameter: {inspectedTree.Diameter:F1} cm", cardBodyStyle);
        GUILayout.Label($"• Stem Volume: {inspectedTree.BiologicalStemVolumeM3:F2} m³", cardBodyStyle);

        if (inspectedTreeEcology != null && !inspectedTree.IsStump)
        {
            GUILayout.Label($"• Local crowding: {inspectedTreeEcology.GetCompetitionLabel(inspectedTree)} (CI {inspectedTreeEcology.GetCompetitionIndex(inspectedTree):0.0})", cardBodyStyle);
            GUILayout.Label($"• Current suppression: {inspectedTreeEcology.GetCurrentSuppression(inspectedTree):P0} of potential DBH growth", cardBodyStyle);
            float recentGrowth = inspectedTreeEcology.GetAnnualDbhGrowth(inspectedTree);
            if (recentGrowth > 0.0001f)
                GUILayout.Label($"• Recent DBH growth: {recentGrowth:F2} cm/year", cardBodyStyle);
            GUILayout.Label($"• Wind vulnerability: {inspectedTreeEcology.GetWindRiskLabel(inspectedTree)}", cardBodyStyle);
        }

        if (inspectedTree.IsStump)
        {
            GUILayout.Label("• CCF note: canopy gap created; future regeneration will grow from new individuals.", cardBodyStyle);
        }
        else
        {
            if (inspectedTree.Species != null)
            {
                float maturity = inspectedTree.Species.Maturity(inspectedTree.AgeYears);
                string reproduction = maturity <= 0.05f
                    ? "not yet seed-bearing"
                    : (maturity < 0.95f ? "maturing — some seed" : "seed-bearing");
                GUILayout.Label($"• Reproduction: {reproduction}", cardBodyStyle);
            }

            string ccfNote = "• CCF: consider crown cover, neighbours and wind exposure before thinning.";
            GUILayout.Label(ccfNote, cardBodyStyle);

            if (inspectedTree.ChopProgress > 0)
            {
                GUILayout.Label($"• Chopping Progress: {inspectedTree.ChopProgress} / {inspectedTree.ChopsRequired} chops", cardBodyStyle);
            }

            GUILayout.Label($"• Potential Wood Yield: {TimberUnits(inspectedTree)} Wood", cardBodyStyle);
        }

        GUILayout.FlexibleSpace();
        GUILayout.Label(inspectedTree.CanChop
            ? "Press [F] to Chop   |   Press [E] to Close"
            : "Press [E], [Left Click], or step away to close", cardFooterStyle);
        GUILayout.EndArea();
    }
}

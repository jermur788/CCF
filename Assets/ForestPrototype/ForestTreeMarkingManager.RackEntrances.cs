using UnityEngine;
using UnityEngine.InputSystem;

// Field communication of the authoritative v21 rack records (plan R-3). Orange never enters
// the loose Fell ID set or the saved per-tree enum. All three marks use MarkerObjects and
// the same marking manager's conflict checks; there is no second entrance-mark ledger.
public sealed partial class ForestTreeMarkingManager
{
    public const float RackSprayDistance = 3.5f; // [C] physical spray/use reach, not a forestry constant
    private const string RackEntranceMarkerName = "Orange Rack Entrance Mark";
    private ScenarioOneManager rackScenario;
    private Material orangePaint, cueMaterial;
    private GameObject entranceCue;
    private AudioSource sprayAudio;
    private AudioClip sprayClip;
    private bool sprayLockedLastFrame;
    private float sprayFeedbackUntil;

    public bool RackSprayEquipped { get; private set; }
    public bool RackSprayFeedbackActive => Time.unscaledTime < sprayFeedbackUntil;
    public int CueRackId { get; private set; }
    public string RackFieldStatus { get; private set; } = "";

    private ScenarioOneManager RackScenario
    {
        get
        {
            if (rackScenario == null) rackScenario = FindFirstObjectByType<ScenarioOneManager>();
            return rackScenario;
        }
    }

    public bool HasRackEntranceMark(ForestTree tree)
    {
        ScenarioOneManager scenario = RackScenario;
        if (tree == null || scenario == null || !scenario.RackPlanningV2Active) return false;
        // Read draft records too: reopening alone never erases a geometry-bound mark.
        foreach (RackSaveData rack in scenario.Racks)
            if (rack != null && rack.entranceMarked && rack.entranceTreeId == tree.TreeId) return true;
        return false;
    }

    private bool IsRackRemovalTree(ForestTree tree)
    {
        ScenarioOneManager scenario = RackScenario;
        if (tree == null || scenario == null || !scenario.RackPlanningV2Active) return false;
        foreach (RackSaveData rack in scenario.Racks)
            if (rack != null && rack.removalTreeIds != null && rack.removalTreeIds.Contains(tree.TreeId)) return true;
        return false;
    }

    // First unmarked, unworked rack by stable ID. Never redirect a broken rack to a nearby tree.
    public bool TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem)
    {
        rack = null; tree = null; problem = "";
        ScenarioOneManager scenario = RackScenario;
        if (scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed) return false;
        foreach (RackPlanRack candidate in scenario.ConfirmedRacks)
        {
            if (candidate.EntranceMarked || candidate.IsLocked) continue;
            rack = candidate;
            return scenario.TryGetRackEntranceTarget(candidate.Id, out tree, out problem);
        }
        return false;
    }

    public bool EquipRackSpray(bool equip)
    {
        ScenarioOneManager scenario = RackScenario;
        if (equip && (scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed
            || (inspectionPlayer != null && inspectionPlayer.IsPlantingMode)))
        {
            SetMessage("Confirm the rack plan and leave planting mode before equipping the spray marker.");
            return false;
        }
        RackSprayEquipped = equip;
        SetMessage(equip ? "Spray marker equipped · [Left-click] Spray · [P] Stow."
            : "Spray marker stowed.");
        return true;
    }

    private void UpdateRackSpray()
    {
        ScenarioOneManager scenario = RackScenario;
        bool available = scenario != null && scenario.RackPlanningV2Active && scenario.RackPlanConfirmed;
        bool locked = Cursor.lockState == CursorLockMode.Locked;
        if (!available || (inspectionPlayer != null && inspectionPlayer.IsPlantingMode)) RackSprayEquipped = false;
        if (available && locked && Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
            EquipRackSpray(!RackSprayEquipped);
        if (RackSprayEquipped && locked && sprayLockedLastFrame && Application.isFocused
            && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TrySprayRackEntrance();
        sprayLockedLastFrame = locked;

        bool target = TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem);
        RackFieldStatus = !available ? "" : $"Rack entrances marked {scenario.MarkedRackEntranceCount} / {ScenarioOneManager.RackPlanTargetCount}"
            + (!string.IsNullOrEmpty(problem) ? " · " + problem : "");
        CueRackId = target ? rack.Id : 0;
        if (target)
        {
            EnsureEntranceCue();
            if (entranceCue != null)
            {
                entranceCue.SetActive(true);
                entranceCue.transform.position = tree.transform.position + Vector3.up * Mathf.Min(2.6f, tree.Height * 0.75f);
                if (view != null) entranceCue.transform.rotation = Quaternion.LookRotation(view.transform.position - entranceCue.transform.position);
            }
        }
        else if (entranceCue != null) entranceCue.SetActive(false);
    }

    // Re-raycast at use time. Neither a cached tree nor a caller-supplied tree can satisfy aim/range.
    public RackPlanOperationResult TrySprayRackEntrance()
    {
        ScenarioOneManager scenario = RackScenario;
        RackPlanOperationResult result;
        if (!RackSprayEquipped || scenario == null || !scenario.RackPlanningV2Active || !scenario.RackPlanConfirmed
            || Cursor.lockState != CursorLockMode.Locked)
            result = RackPlanOperationResult.Invalid("Equip the spray marker [P] after confirming the rack plan.");
        else if (!TryRackSprayAim(out ForestTree tree))
            result = RackPlanOperationResult.Invalid("Aim at the entrance trunk and move within 3.5 m to spray.");
        else
        {
            RackPlanRack targetRack = null;
            foreach (RackPlanRack rack in scenario.ConfirmedRacks)
                if (rack.EntranceTreeId == tree.TreeId) { targetRack = rack; break; }
            result = targetRack == null
                ? RackPlanOperationResult.Invalid("That tree is not a rack entrance. Follow the entrance cue.")
                : scenario.MarkRackEntrance(targetRack.Id);
            if (result.Status == RackPlanOperationStatus.Applied)
            {
                CreateRackEntranceMarker(tree);
                PlaySprayFeedback();
                // Hide this rack's cue immediately; next Update projects the next real target.
                if (entranceCue != null) entranceCue.SetActive(false);
                CueRackId = 0;
            }
        }
        SetMessage(result.Message);
        return result;
    }

    private bool TryRackSprayAim(out ForestTree tree)
    {
        tree = null;
        if (view == null) return false;
        // RaycastAll avoids dropping the closest blocker when dense crowns fill a fixed buffer.
        RaycastHit[] hits = Physics.RaycastAll(view.transform.position, view.transform.forward, RackSprayDistance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        float nearest = float.MaxValue;
        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.collider.transform;
            if (playerRoot != null && (hitTransform == playerRoot || hitTransform.IsChildOf(playerRoot))) continue;
            if (hit.collider.GetComponentInParent<ScenarioHabitatVisuals>() != null
                || (hit.collider.GetComponentInParent<ForestEcologyController>() != null
                    && hit.collider.GetComponentInParent<ForestTree>() == null)) continue;
            if (hit.distance >= nearest) continue;
            nearest = hit.distance;
            tree = hit.collider.GetComponentInParent<ForestTree>();
        }
        return tree != null && tree.IsLiving;
    }

    public string RackSprayPrompt()
    {
        if (!RackSprayEquipped) return "";
        if (!TryRackSprayAim(out ForestTree tree)) return "Spray marker · aim at an entrance trunk within 3.5 m · [P] Stow";
        if (HasRackEntranceMark(tree)) return "Orange · entrance already marked · [P] Stow";
        ScenarioOneManager scenario = RackScenario;
        foreach (RackPlanRack rack in scenario.ConfirmedRacks)
        {
            if (rack.EntranceTreeId != tree.TreeId) continue;
            if (rack.IsLocked) return "Rack already harvested · entrance locked";
            if (tree.IsCropTree) return "Crop Tree · remove its blue designation before spraying";
            if (tree.IsMarkedForFell) return "Fell mark · remove its red designation before spraying";
            if (!scenario.TryGetRackEntranceTarget(rack.Id, out _, out string problem)) return problem;
            return $"Rack {rack.Id} entrance · [Left-click] Spray orange · [P] Stow";
        }
        return "Not a rack entrance · follow the entrance cue · [P] Stow";
    }

    private Material OrangePaint()
    {
        if (orangePaint == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null) orangePaint = new Material(shader) { name = "Rack entrance orange paint", color = new Color(0.95f, 0.28f, 0.025f) };
        }
        return orangePaint;
    }

    private void CreateRackEntranceMarker(ForestTree tree)
    {
        if (MarkerObjects.TryGetValue(tree.TreeId, out GameObject existing) && existing != null) return;
        Material paint = OrangePaint();
        if (paint == null) return;
        var marker = new GameObject(RackEntranceMarkerName);
        marker.transform.SetParent(tree.transform, false);
        // Four narrow curved patches wrap the trunk rather than becoming floating square signs.
        float radius = Mathf.Max(0.08f, tree.Diameter / 200f) + 0.012f;
        for (int side = 0; side < 4; side++)
        {
            var stripe = new GameObject("Orange vertical stripe " + (side + 1));
            stripe.transform.SetParent(marker.transform, false);
            const int segments = 6;
            var vertices = new Vector3[(segments + 1) * 2];
            var triangles = new int[segments * 6];
            for (int i = 0; i <= segments; i++)
            {
                float angle = (side * 90f - 12f + 24f * i / segments) * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * radius, z = Mathf.Cos(angle) * radius;
                vertices[i * 2] = new Vector3(x, 1.0f, z);
                vertices[i * 2 + 1] = new Vector3(x, Mathf.Min(1.85f, tree.Height * 0.75f), z);
                if (i == segments) continue;
                int a = i * 2, t = i * 6;
                triangles[t] = a; triangles[t + 1] = a + 2; triangles[t + 2] = a + 1;
                triangles[t + 3] = a + 2; triangles[t + 4] = a + 3; triangles[t + 5] = a + 1;
            }
            var mesh = new Mesh { name = "Rack entrance paint patch" };
            mesh.vertices = vertices; mesh.triangles = triangles; mesh.RecalculateNormals();
            stripe.AddComponent<MeshFilter>().sharedMesh = mesh;
            stripe.AddComponent<MeshRenderer>().sharedMaterial = paint;
        }
        MarkerObjects[tree.TreeId] = marker;
    }

    private void EnsureEntranceCue()
    {
        if (entranceCue != null) return;
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) return;
        cueMaterial = new Material(shader) { name = "Rack entrance cue", color = new Color(0.8f, 0.76f, 0.57f) };
        entranceCue = new GameObject("Next rack entrance cue");
        LineRenderer line = entranceCue.AddComponent<LineRenderer>();
        line.sharedMaterial = cueMaterial; line.useWorldSpace = false; line.loop = true;
        line.widthMultiplier = 0.018f; line.positionCount = 4;
        line.SetPositions(new[] { new Vector3(0f, 0.18f, 0f), new Vector3(0.13f, 0f, 0f), new Vector3(0f, -0.18f, 0f), new Vector3(-0.13f, 0f, 0f) });
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void PlaySprayFeedback()
    {
        sprayFeedbackUntil = Time.unscaledTime + 0.18f;
        if (sprayAudio == null)
        {
            sprayAudio = gameObject.AddComponent<AudioSource>();
            sprayAudio.playOnAwake = false; sprayAudio.spatialBlend = 0f; sprayAudio.volume = 0.15f;
            const int samples = 7938; // 180 ms, synthesized hiss, no external asset/licence or simulation RNG
            var data = new float[samples];
            uint noise = 9173;
            float previous = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                noise ^= noise << 13; noise ^= noise >> 17; noise ^= noise << 5;
                float value = (noise % 65536) / 32768f - 1f;
                data[i] = (value - previous) * 0.35f * Mathf.Sin(Mathf.PI * i / samples);
                previous = value;
            }
            sprayClip = AudioClip.Create("Short forestry spray", samples, 1, 44100, false);
            sprayClip.SetData(data, 0);
        }
        sprayAudio.PlayOneShot(sprayClip);
    }

    private void DisposeRackMarkingVisuals()
    {
        if (entranceCue != null) Destroy(entranceCue);
        if (orangePaint != null) Destroy(orangePaint);
        if (cueMaterial != null) Destroy(cueMaterial);
        if (sprayClip != null) Destroy(sprayClip);
    }
}

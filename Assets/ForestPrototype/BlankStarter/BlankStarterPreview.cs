using UnityEngine;
using UnityEngine.SceneManagement;

// Scene-owned authoring data, not a scenario identity or save profile.
[DefaultExecutionOrder(-100)]
public sealed class BlankStarterPreview : MonoBehaviour
{
    public const string Banner = "Development preview — unsaved";
    public const string SaveRefusal = "Development preview — unsaved. Historical saves are unavailable here.";

    [SerializeField] private float width = 30f;
    [SerializeField] private float depth = 30f;
    [SerializeField] private Vector2 spawn = new Vector2(0f, -5f);
    [SerializeField] private BoxCollider ground;
    [SerializeField] private ForestPlayer player;
    [SerializeField] private bool testOnly;
    [SerializeField, TextArea] private string introduction;
    [SerializeField, TextArea] private string objective;
    [SerializeField, TextArea] private string guidance;

    public string Title => testOnly ? "Blank Starter (TEST ONLY)" : "Blank Starter";
    public string Introduction => Slot(introduction, "[SCENARIO INTRODUCTION — DEV ONLY]");
    public string Objective => Slot(objective, "[SCENARIO OBJECTIVE — DEV ONLY]");
    public string Guidance => Slot(guidance, "[SCENARIO GUIDANCE — DEV ONLY]");
    public string FootprintError { get; private set; }
    public ForestPlayer Player => FootprintError == null ? player : null;

    public static BlankStarterPreview InScene(Scene scene)
    {
        // During Play, scene roots already exist while their Awake methods are running.
        if (!scene.IsValid() || (!Application.isPlaying && !scene.isLoaded)) return null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            // Even an inactive authoring marker must not silently enable historical persistence.
            BlankStarterPreview preview = root.GetComponentInChildren<BlankStarterPreview>(true);
            if (preview != null) return preview;
        }
        return null;
    }

    // Shared by the menu and save controller, before either even forms a disk operation.
    public static bool BlocksHistoricalSave(Scene callerScene)
    {
        Scene active = SceneManager.GetActiveScene();
        return InScene(active) != null || (callerScene != active && InScene(callerScene) != null);
    }

    private void Awake()
    {
        CharacterController controller = player != null ? player.GetComponent<CharacterController>() : null;
        if (!PositiveFinite(width) || !PositiveFinite(depth))
            FootprintError = "Invalid footprint: width and depth must be positive and finite. No ground or player spawned.";
        else if (ground == null || controller == null)
            FootprintError = "Invalid starter setup: ground and player must be assigned. No ground or player spawned.";
        else if (!float.IsFinite(spawn.x) || !float.IsFinite(spawn.y)
            || width * 0.5f - Mathf.Abs(spawn.x) < controller.radius
            || depth * 0.5f - Mathf.Abs(spawn.y) < controller.radius)
            FootprintError = "Invalid spawn: leave at least one controller radius inside the ground edge. No ground or player spawned.";

        if (FootprintError != null)
        {
            if (ground != null) ground.gameObject.SetActive(false);
            if (player != null) player.gameObject.SetActive(false);
            return;
        }

        // The scene's unit cube stays 20 cm thick, with its walkable surface at y = 0.
        ground.transform.SetPositionAndRotation(new Vector3(0f, -0.1f, 0f), Quaternion.identity);
        ground.transform.localScale = new Vector3(width, 0.2f, depth);
        player.transform.position = new Vector3(spawn.x, 0.05f, spawn.y);
    }

    public bool TryGetGroundBounds(out Bounds bounds)
    {
        if (FootprintError == null && ground != null && ground.enabled && ground.gameObject.activeInHierarchy)
        {
            bounds = ground.bounds;
            return true;
        }
        bounds = default;
        return false;
    }

    private static bool PositiveFinite(float value) => value > 0f && float.IsFinite(value);
    private static string Slot(string text, string marker) => string.IsNullOrWhiteSpace(text) ? marker : text;
}

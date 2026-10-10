using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Standalone session controls only. Forest/save behaviour stays in existing APIs.
// The startup/title screen (StandaloneSessionMenu.Startup.cs) is a second card on this same menu, so it
// shares the pause, input-isolation and restore logic below instead of duplicating it.
public sealed partial class StandaloneSessionMenu : MonoBehaviour
{
    [Serializable] public sealed class Identity { public string buildId; public string gitSha; }
    public static bool IsOpen { get; private set; }
    private UIDocument document;
    private PanelSettings settings;
    private VisualElement card;
    private ForestPlayer player;
    private ForestSaveController saves;
    private ScenarioOneUiRoot forestUi;
    private bool playerWasEnabled, savesWereEnabled, uiWasEnabled;
    private float previousTimeScale;
    private Identity identity;
    public Identity BuildIdentity => identity;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
#if UNITY_EDITOR
        // Editor Play Mode enters the stand directly, as it always has, so development and every verification
        // gate behave unchanged. Set CCF_STARTUP_SCREEN=1 to exercise the real startup screen in the Editor.
        // This check is compiled out of Players: a Player always shows the startup screen.
        if (Environment.GetEnvironmentVariable("CCF_STARTUP_SCREEN") != "1") return;
#endif
        var host = new GameObject("Standalone session menu");
        DontDestroyOnLoad(host);
        host.AddComponent<StandaloneSessionMenu>().ShowStartupScreen();
    }

    private IEnumerator Start()
    {
        var text = Resources.Load<TextAsset>("CCFBuildIdentity");
        identity = text != null ? JsonUtility.FromJson<Identity>(text.text) : new Identity { buildId = "UNSTAMPED DEVELOPMENT BUILD", gitSha = "unknown" };
        Debug.Log($"CCF_BUILD_ID {identity.buildId} git={identity.gitSha} Unity={Application.unityVersion}");
        Debug.Log($"CCF_DIAGNOSTICS os={SystemInfo.operatingSystem} cpu={SystemInfo.processorType} ramMB={SystemInfo.systemMemorySize} gpu={SystemInfo.graphicsDeviceName} graphics={SystemInfo.graphicsDeviceVersion} resolution={Screen.width}x{Screen.height} persistentDataPath={Application.persistentDataPath}");
        settings = ScriptableObject.CreateInstance<PanelSettings>();
        settings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ScenarioOneUiTheme");
        settings.sortingOrder = 100;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        settings.referenceResolution = new Vector2Int(1600, 900);
        document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = settings;
        var root = document.rootVisualElement;
        root.style.flexGrow = 1;
        root.style.justifyContent = Justify.Center;
        root.style.alignItems = Align.Center;
        root.style.backgroundColor = new Color(0.03f, 0.05f, 0.035f, 0.97f);
        card = new VisualElement(); card.style.width = 650; card.style.maxWidth = Length.Percent(95);
        card.style.paddingLeft = card.style.paddingRight = 24;
        root.Add(card);
        yield return null; yield return null;
        if (startupScreen) yield return StartCoroutine(OpenStartupWhenReady());
        else Open();
    }
    private void Update()
    {
        // The startup screen is left only through its own buttons, never by F10.
        if (!startupScreen && Keyboard.current != null && Keyboard.current.f10Key.wasPressedThisFrame)
        {
            if (IsOpen) Close(); else Open();
        }
        if (IsOpen) { UnityEngine.Cursor.lockState = CursorLockMode.None; UnityEngine.Cursor.visible = true; }
    }
    private static string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");
    public void Open()
    {
        if (IsOpen || FindFirstObjectByType<ScenarioOneManager>()?.ReferencePreviewActive == true) return;
        player = FindFirstObjectByType<ForestPlayer>(); saves = FindFirstObjectByType<ForestSaveController>();
        forestUi = FindFirstObjectByType<ScenarioOneUiRoot>();
        if (player == null || saves == null || forestUi == null || forestUi.RootElement == null) return;
        playerWasEnabled = player != null && player.enabled; savesWereEnabled = saves != null && saves.enabled;
        uiWasEnabled = forestUi != null && forestUi.enabled;
        previousTimeScale = Time.timeScale; Time.timeScale = 0;
        IsOpen = true;
        if (player != null) player.enabled = false;
        if (saves != null) saves.enabled = false;
        if (forestUi != null) { forestUi.enabled = false; forestUi.RootElement.style.display = DisplayStyle.None; }
        document.rootVisualElement.style.display = DisplayStyle.Flex;
        if (startupScreen) StartupCard(); else MainCard();
    }
    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false; startupScreen = false; Time.timeScale = previousTimeScale;
        document.rootVisualElement.style.display = DisplayStyle.None;
        if (saves != null) saves.enabled = savesWereEnabled;
        if (forestUi != null) { forestUi.RootElement.style.display = DisplayStyle.Flex; forestUi.enabled = uiWasEnabled; }
        if (player != null) player.enabled = playerWasEnabled;
    }
    private void Text(string text, int size = 20)
    {
        var label = new Label(text); label.style.whiteSpace = WhiteSpace.Normal;
        label.style.fontSize = size; label.style.color = Color.white; label.style.marginBottom = 12; card.Add(label);
    }
    private Button ActionButton(string text, System.Action action)
    {
        var button = new Button(action) { text = text }; button.style.fontSize = 22;
        button.style.height = 46; button.style.marginBottom = 8; card.Add(button);
        return button;
    }
    private void MainCard()
    {
        card.Clear(); Text("CCF · private first-cycle test", 30); Text(identity.buildId);
        Text("Manage one cycle, read the Annual Review, then walk and observe the changed forest. Choose your own forestry decisions.");
        ActionButton("Start a fresh Scenario One test…", () => Confirm("Discard current unsaved progress and start a fresh forest? Your saved file is retained until you choose Save.", Restart));
        ActionButton("Continue current forest [F10]", Close);
        if (File.Exists(SavePath)) ActionButton("Load your same-build save", () => { Close(); saves?.Load(); });
        ActionButton("Save current forest and return…", () => {
            if (File.Exists(SavePath)) Confirm("Replace your existing forest save with the current forest? Use saves only with this build.", SaveAndClose);
            else SaveAndClose();
        });
        Text("F1 Help · O Learning · M Map · Tab Work Plan · F5 Save · F9 Load · F10 this menu. Learning introductions remain remembered on this device.");
        ActionButton("Quit CCF…", () => Confirm("Quit without saving automatically? Use Save first if you want to keep current progress.", Quit));
    }
    private void Confirm(string prompt, System.Action action)
    {
        card.Clear(); Text(identity.buildId); Text(prompt);
        ActionButton("Confirm", action); ActionButton("Cancel", MainCard);
    }
    public void SaveAndClose() { Close(); saves?.Save(); }
    public void Restart()
    {
        Close(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("CCF_SESSION_FRESH_TEST existingSaveRetained=True");
    }
    public void Quit() { Debug.Log("CCF_SESSION_NORMAL_QUIT"); Application.Quit(0); }
    private void OnDestroy()
    {
        if (IsOpen) Close();
        if (settings != null) Destroy(settings);
    }
}

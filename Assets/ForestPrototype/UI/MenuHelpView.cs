using UnityEngine;
using UnityEngine.UIElements;

// Interface learning preferences are local to the player, not forest/save state.
public sealed partial class MenuHelpView
{
    public const string PreferencePrefix = "CCF.MenuHelp.v1.";
    private readonly ScenarioOneUiRoot ui;
    private readonly Label title, body;
    private readonly Button close;
    public VisualElement Root { get; }
    public bool IsOpen { get; private set; }
    public Menu CurrentMenu { get; private set; }

    public MenuHelpView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        Root.name = "menu-help";
        Root.style.display = DisplayStyle.None;
        VisualElement panel = UiKit.Box("panel", "menu-help-panel");
        // Opaque help stays readable over text-heavy map and review screens.
        panel.style.backgroundColor = new Color(14f / 255f, 22f / 255f, 16f / 255f, 1f);
        Root.Add(panel);
        title = UiKit.Add(panel, "", "title");
        var scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexShrink = 1;
        body = UiKit.Add(scroll, "", "body");
        body.name = "menu-help-text";
        panel.Add(scroll);
        UiKit.Add(panel, "Revisit this explanation with Help or F1. Introductions appear only once on this device.", "muted");
        close = UiKit.Button("", () => ui.CloseHelp(), true, "btn-primary");
        close.name = "menu-help-close";
        panel.Add(close);
    }

    public static bool Introduced(Menu menu) => PlayerPrefs.GetInt(PreferencePrefix + menu, 0) == 1;

    public void Introduce(Menu menu)
    {
        if (!IsOpen && !Introduced(menu)) Show(menu);
    }

    public void Show(Menu menu)
    {
        CurrentMenu = menu;
        IsOpen = true;
        title.text = Title(menu);
        body.text = Explanation(menu);
        close.text = menu == Menu.AnnualReview ? "Read the results [Esc]" : "Continue [Esc]";
        Root.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        if (!IsOpen) return;
        PlayerPrefs.SetInt(PreferencePrefix + CurrentMenu, 1);
        PlayerPrefs.Save();
        IsOpen = false;
        Root.style.display = DisplayStyle.None;
    }
}

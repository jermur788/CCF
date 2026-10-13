using UnityEngine;
using UnityEngine.UIElements;

// Interface learning preferences are local to the player, not forest/save state.
public sealed partial class MenuHelpView
{
    public const string PreferencePrefix = "CCF.MenuHelp.v1.";
    private readonly ScenarioOneUiRoot ui;
    private readonly Label title, body;
    private readonly ScrollView scroll;
    private readonly Button close, more;
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
        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexShrink = 1;
        body = UiKit.Add(scroll, "", "body");
        body.name = "menu-help-text";
        panel.Add(scroll);
        UiKit.Add(panel, "Help [F1] reopens the full explanation of any screen. First-use introductions appear only once on this device.", "muted");
        VisualElement buttons = UiKit.Box("row");
        more = UiKit.Button("More detail", ShowDetail);
        more.name = "menu-help-more";
        buttons.Add(more);
        close = UiKit.Button("", () => ui.CloseHelp(), true, "btn-primary");
        close.name = "menu-help-close";
        close.style.flexGrow = 1;
        buttons.Add(close);
        panel.Add(buttons);
    }

    public static bool Introduced(Menu menu) => PlayerPrefs.GetInt(PreferencePrefix + menu, 0) == 1;

    // First use of a screen: the short text where the screen has one.
    public void Introduce(Menu menu)
    {
        if (!IsOpen && !Introduced(menu)) Display(menu, true);
    }

    // F1 and the Help buttons: always the full explanation.
    public void Show(Menu menu) => Display(menu, false);

    private void Display(Menu menu, bool brief)
    {
        CurrentMenu = menu;
        IsOpen = true;
        string summary = brief ? Summary(menu) : null;
        title.text = summary != null ? SummaryTitle(menu) : Title(menu);
        body.text = summary ?? Explanation(menu);
        more.style.display = summary != null ? DisplayStyle.Flex : DisplayStyle.None;
        scroll.scrollOffset = Vector2.zero;
        close.text = menu == Menu.AnnualReview ? "Read the results [Esc]" : "Continue [Esc]";
        Root.style.display = DisplayStyle.Flex;
    }

    // Swaps the short first-use text for the full explanation without closing the screen.
    private void ShowDetail() => Display(CurrentMenu, false);

    public void Close()
    {
        if (!IsOpen) return;
        PlayerPrefs.SetInt(PreferencePrefix + CurrentMenu, 1);
        PlayerPrefs.Save();
        IsOpen = false;
        Root.style.display = DisplayStyle.None;
    }
}

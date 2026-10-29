using UnityEngine;
using UnityEngine.UIElements;

// Interface learning preferences are local to the player, not forest/save state.
//
// The help screen is a modal window: a dimmed backdrop that takes every pointer event, and inside it a title bar, a scrolling
// body and a button bar. First-use screens show a short text with a "More detail" toggle; F1 and the Help buttons show the
// full explanation directly.
public sealed partial class MenuHelpView
{
    public const string PreferencePrefix = "CCF.MenuHelp.v1.";
    public const string MoreDetailLabel = "More detail";
    public const string LessDetailLabel = "Less detail";
    private readonly ScenarioOneUiRoot ui;
    private readonly Label title, body;
    private readonly ScrollView scroll;
    private readonly Button close, more;
    private readonly VisualElement window;
    public ScenarioOneOnboardingView Onboarding { get; }
    public VisualElement Root { get; }
    public bool IsOpen { get; private set; }
    public Menu CurrentMenu { get; private set; }
    // True while the window shows the long explanation of a first-use screen that also has a short text (the toggle can go back).
    public bool ShowingDetail { get; private set; }

    public MenuHelpView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
        Root.name = "menu-help";
        Root.style.display = DisplayStyle.None;
        UiKit.MakeModal(Root);

        VisualElement panel = UiKit.Box("menu-help-panel");
        panel.name = "menu-help-window";
        window = panel;
        Root.Add(panel);

        VisualElement titleBar = UiKit.Box("menu-help-title");
        title = UiKit.Add(titleBar, "", "title");
        title.name = "menu-help-title";
        panel.Add(titleBar);

        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        scroll.AddToClassList("menu-help-body");
        scroll.style.flexShrink = 1;
        body = UiKit.Add(scroll, "", "body");
        body.name = "menu-help-text";
        panel.Add(scroll);

        VisualElement bar = UiKit.Box("menu-help-bar");
        more = UiKit.Button(MoreDetailLabel, ToggleDetail, true, "btn-ghost");
        more.name = "menu-help-more";
        bar.Add(more);
        close = UiKit.Button("", () => ui.CloseHelp(), true, "btn-primary");
        close.name = "menu-help-close";
        bar.Add(close);
        panel.Add(bar);
        Onboarding = new ScenarioOneOnboardingView(CloseWindow);
        Root.Add(Onboarding.Root);
    }

    public static bool Introduced(Menu menu) => PlayerPrefs.GetInt(PreferencePrefix + menu, 0) == 1;

    // Forgets which first-use screens this device has seen, so the next player meets them again. Local preferences only: the
    // forest and its save are untouched.
    public static void ResetProgress()
    {
        foreach (Menu menu in System.Enum.GetValues(typeof(Menu))) PlayerPrefs.DeleteKey(PreferencePrefix + menu);
        PlayerPrefs.Save();
    }

    // First use of a screen: the short text where the screen has one.
    public void Introduce(Menu menu)
    {
        // Walking teaching starts explicitly from Start New, never from a save load/context refresh.
        if (menu != Menu.WalkingHud && !IsOpen && !Introduced(menu)) Display(menu, true);
    }

    // F1 and the Help buttons: always the full explanation.
    public void Show(Menu menu) => Display(menu, false);

    // A fresh scenario gets its premise even on a device that has seen HUD help before. Session-only; no new preference/save field.
    public void ShowScenarioIntroduction()
    {
        CurrentMenu = Menu.WalkingHud;
        IsOpen = true;
        ShowingDetail = false;
        window.style.display = DisplayStyle.None;
        Root.style.display = DisplayStyle.Flex;
        Onboarding.Show();
        if (ui.Manager != null) ui.Manager.SetModalOpen(true);
    }

    // Continue/load must dismiss transient onboarding without advancing its pages.
    public void DismissScenarioIntroduction()
    {
        if (!Onboarding.IsOpen) return;
        Onboarding.Hide();
        CloseWindow();
    }

    private void Display(Menu menu, bool brief)
    {
        Onboarding.Hide();
        window.style.display = DisplayStyle.Flex;
        CurrentMenu = menu;
        IsOpen = true;
        string summary = brief ? Summary(menu) : null;
        ShowingDetail = false;
        Render(menu, summary);
        close.text = menu == Menu.AnnualReview ? "Read the results [Esc]" : "Continue [Esc]";
        Root.style.display = DisplayStyle.Flex;
        if (ui.Manager != null) ui.Manager.SetModalOpen(true);
    }

    // One place that decides what the window shows. The toggle exists only for first-use screens that have a short text.
    private void Render(Menu menu, string summary)
    {
        bool hasBrief = Summary(menu) != null && (summary != null || ShowingDetail);
        title.text = summary != null ? SummaryTitle(menu) : Title(menu);
        string text = summary ?? Explanation(menu);
        if (ui.Manager != null && ui.Manager.RackPlanningV2Active)
        {
            if (menu == Menu.Rack)
                text = "Plan extraction racks on the Stand Map [M]. Choose Plan Racks, create or edit the corridors, then choose Confirm Rack Plan. "
                    + "Confirmation adds pending removal work; review and approve it in the Work Plan [Tab], then advance one year. "
                    + "Racks are permanent machine-access routes for this and future thinnings. Individual Fell and Crop Tree marks remain forest decisions.";
            else if (menu == Menu.WalkingHud)
                text = text.Replace("racks (B)", "racks (Stand Map [M] → Plan Racks)")
                    .Replace(ControlsReference.KeysLine, ControlsReference.KeysLineFor(true));
        }
        body.text = text;
        more.style.display = hasBrief ? DisplayStyle.Flex : DisplayStyle.None;
        more.text = summary != null ? MoreDetailLabel : LessDetailLabel;
        scroll.scrollOffset = Vector2.zero;
    }

    // The same button opens the long explanation and, on a second press, goes back to the short text.
    private void ToggleDetail()
    {
        ShowingDetail = !ShowingDetail;
        Render(CurrentMenu, ShowingDetail ? null : Summary(CurrentMenu));
    }

    public void Close()
    {
        if (!IsOpen) return;
        if (Onboarding.IsOpen) { Onboarding.Next(); return; }
        CloseWindow();
    }

    private void CloseWindow()
    {
        PlayerPrefs.SetInt(PreferencePrefix + CurrentMenu, 1);
        PlayerPrefs.Save();
        IsOpen = false;
        ShowingDetail = false;
        Root.style.display = DisplayStyle.None;
        if (ui.Manager != null) ui.Manager.SetModalOpen(false);
    }
}

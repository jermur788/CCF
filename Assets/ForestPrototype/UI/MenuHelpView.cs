using System.Collections.Generic;
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
    // The extra parts of the scenario introduction (picture, three explanations, how to begin), removed whenever the window shows something else.
    private readonly List<VisualElement> introParts = new List<VisualElement>();
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
        if (!IsOpen && !Introduced(menu)) Display(menu, true);
    }

    // F1 and the Help buttons: always the full explanation.
    public void Show(Menu menu) => Display(menu, false);

    // A fresh scenario gets its premise even on a device that has seen HUD help before. Session-only; no new preference/save field.
    public void ShowScenarioIntroduction() => Display(Menu.WalkingHud, true);

    private void Display(Menu menu, bool brief)
    {
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
        bool introduction = menu == Menu.WalkingHud && summary != null;
        title.text = summary != null ? SummaryTitle(menu) : Title(menu);
        // The introduction puts the body text beside its picture; take it back first so every other screen finds it where it always was.
        if (body.parent != scroll) scroll.Insert(0, body);
        foreach (VisualElement part in introParts) part.RemoveFromHierarchy();
        introParts.Clear();
        window.EnableInClassList("menu-help-intro", introduction);
        if (introduction)
        {
            // The lead sits beside the picture of the stand and the passing of years, so the whole introduction fits the window without
            // scrolling; then three short explanations and how to begin. The words are ordinary labels, so they follow the shared type scale.
            body.text = IntroLead;
            VisualElement top = UiKit.Box("intro-top");
            scroll.Insert(0, top);
            top.Add(body);
            top.Add(new TeachingDiagram(-1, ""));
            introParts.Add(top);
            VisualElement sections = UiKit.Box("intro-sections");
            foreach (string[] section in IntroSections)
            {
                VisualElement block = UiKit.Box("intro-block");
                UiKit.Add(block, section[0], "intro-heading");
                UiKit.Add(block, section[1], "intro-text");
                sections.Add(block);
            }
            AddIntroPart(sections);
            AddIntroPart(UiKit.Text(IntroStartHeading, "intro-heading", "intro-start-heading"));
            AddIntroPart(UiKit.Text(IntroStart, "intro-start"));
        }
        else body.text = summary ?? Explanation(menu);
        more.style.display = hasBrief ? DisplayStyle.Flex : DisplayStyle.None;
        more.text = summary != null ? MoreDetailLabel : LessDetailLabel;
        scroll.scrollOffset = Vector2.zero;
    }

    private void AddIntroPart(VisualElement part)
    {
        scroll.Add(part);
        introParts.Add(part);
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
        PlayerPrefs.SetInt(PreferencePrefix + CurrentMenu, 1);
        PlayerPrefs.Save();
        IsOpen = false;
        ShowingDetail = false;
        Root.style.display = DisplayStyle.None;
        if (ui.Manager != null) ui.Manager.SetModalOpen(false);
    }
}

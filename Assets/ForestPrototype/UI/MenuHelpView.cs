using UnityEngine;
using UnityEngine.UIElements;

// Interface learning preferences are local to the player, not forest/save state.
public sealed class MenuHelpView
{
    public enum Menu { WalkingHud, TreeInspection, StandMap, WorkPlan, AnnualReview }
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

    public static string Title(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeInspection: return "Tree Inspection · understand one tree";
            case Menu.StandMap: return "Stand Map · find where to look";
            case Menu.WorkPlan: return "Work Plan · review and approve jobs";
            case Menu.AnnualReview: return "Annual Review · learn from the year";
            default: return "Walking HUD · your forest at a glance";
        }
    }

    public static string Explanation(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeInspection:
                return "Inspect a tree with E to understand its size, growth, competition and management state. Use this before deciding which trees to favour or remove.\n\n"
                    + "DBH is trunk diameter measured at breast height. Crown/light describes growing space and light; the light value here is measured at the tree's ground cell. Competition shows whether neighbours restrict growth.\n\n"
                    + "A Crop Tree is selected to retain and favour for future development. A Fell mark proposes removal. X and C change these marks; they do not execute work. You choose the trees—this screen does not select a correct answer.\n\n"
                    + "Close Help, then press E to close inspection and return to walking.";
            case Menu.StandMap:
                return "The Stand Map helps you find patterns across the forest that are difficult to see among the trees. Open it with M when looking for a site to inspect.\n\n"
                    + "Use Light for darker/brighter areas, Regeneration for young growth, Browsing / protection for browsing and shelter conditions, and Fell & crop marks for your marked trees.\n\n"
                    + "Select a cell, read its information, then choose Set Waypoint. Close the map with M, Esc or Back to forest. Follow the HUD direction and distance, then inspect the site directly before deciding what to do.\n\n"
                    + "The map helps you find where to look. It cannot mark, clear, plant or approve work remotely, and it does not make the forestry decision for you.";
            case Menu.WorkPlan:
                return "The Work Plan turns decisions made in the forest into jobs you can review and approve. Open it with Tab after marking trees or planting/clearance sites.\n\n"
                    + "Marked trees and tasks appear here. Review labour, material costs, cash after approved work and expected timber value. A contractor's minimum job charge can make a small harvest expensive. Some tasks let you choose contractor or landowner execution.\n\n"
                    + "Review or remove jobs, choose available executors and purchase nursery stock here. Approve pending work commits the plan; Advance one year resolves approved jobs and forest change. Approval alone does not immediately change the forest.\n\n"
                    + "Decide what should happen in the forest; review how it will happen and what it costs here. Use Tab or Back to forest to return.";
            case Menu.AnnualReview:
                return "After advancing time, the Annual Review shows what work happened and how the forest and finances changed. Read it before planning the next cycle.\n\n"
                    + "WORK DONE: what did I do, and did any jobs fail?\nMONEY: what did it cost or earn, and what cash remains?\nFOREST: how did growth, light, regeneration and habitat respond?\n\n"
                    + "Use these results to decide what to inspect next. This screen records outcomes; it does not choose or execute forestry work.\n\n"
                    + "Close Help and read the three sections. After your first results, choose I've read the annual results to unlock further year advances. Use Esc or Walk the forest to return and inspect the changes; Open Work Plan returns to planning.";
            default:
                return "The HUD shows the current year, cash, objectives and information about the ground or tree you are looking at. Current objective progress appears in the status panel.\n\n"
                    + "The forest is where you make management decisions. Look at ground to check light, regeneration and browsing conditions, or press E while looking at a tree to inspect it. Use these observations before marking work.\n\n"
                    + "A waypoint's direction and distance appear here when one is active. M opens the Stand Map; Tab opens the Work Plan to review and approve jobs. The HUD itself does not approve work.\n\n"
                    + "O opens Learning objectives: start with simple map steps, then revisit map layers and forestry lessons as you need them. Progress is remembered on this device, with no first-year deadline.\n\n"
                    + "Close this introduction to walk. Esc releases the mouse; click to resume looking around. Press F1 any time to revisit this screen's help.";
        }
    }
}

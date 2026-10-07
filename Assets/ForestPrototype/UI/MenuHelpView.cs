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
            case Menu.TreeInspection: return "Tree Inspection · judge one tree and its neighbours";
            case Menu.StandMap: return "Stand Map · find where to look";
            case Menu.WorkPlan: return "Work Plan · review how the work is done and what it costs";
            case Menu.AnnualReview: return "Annual Review · what happened, and where to look next";
            default: return "Your forest · continuous-cover forestry";
        }
    }

    // Teaching copy only: it explains existing mechanics and must not claim an
    // ecological effect the simulation does not model (D-020).
    public static string Explanation(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeInspection:
                return "Inspect a tree to judge its future, not just its size.\n\n"
                    + "DBH is the trunk's diameter at breast height (1.3 m). The crown is the tree's living branches and leaves; the light value here is measured at the ground in the tree's cell. "
                    + "Competition shows how strongly neighbours hold back this tree's growth: bigger, closer neighbours count most, small or distant ones count little.\n\n"
                    + "Start with a tree worth keeping. Press C to make a healthy tree with room to grow a Crop Tree: a tree you deliberately keep and give space to develop. "
                    + "Then inspect its neighbours to see which really compete with it. A tree being smaller or suppressed is not, on its own, a reason to remove it.\n\n"
                    + "X proposes a tree for felling. Marks are proposals: nothing is cut until you approve work and advance a year. You choose the trees; this screen never picks a correct answer. Close Help, then E closes inspection.";
            case Menu.StandMap:
                return "Use the Stand Map to find patterns that are hard to see while standing among the trees.\n\n"
                    + "Choose a layer (Light, Regeneration, Browsing / protection, Fell & crop marks), select a cell and read its information. Then Set waypoint, close the map with M, Esc or Back to forest, and follow the HUD direction and distance.\n\n"
                    + "When you arrive, inspect the actual trees and ground before deciding anything.\n\n"
                    + "The map helps you find where to look. It cannot mark, clear, plant or approve work, and it does not make the forestry decision for you.";
            case Menu.WorkPlan:
                return "In the forest you decide what should happen: which trees to keep, which to fell, where to plant or clear. "
                    + "The Work Plan reviews how that work will be carried out and what it will cost.\n\n"
                    + "Check each job's labour, materials and timber income. A harvest visit always costs at least the contractor's minimum charge, so a small harvest can cost more than it earns. "
                    + "Choose an executor where offered, buy nursery stock, and remove jobs you no longer want; add new jobs in the forest.\n\n"
                    + "Approve pending work commits the plan. Advance one year carries it out and grows the forest; approval alone changes nothing. Tab or Back to forest returns you to the trees.";
            case Menu.AnnualReview:
                return "The Annual Review records what actually happened this year. Read it as four questions:\n\n"
                    + "WORK DONE: what work did I do, and did any job fail?\nMONEY: what did it cost or earn?\nFOREST: how did the forest change?\nNEXT: what should I go and inspect?\n\n"
                    + "The figures point you to places; the forest itself is the evidence. Walk back to where you worked and look. "
                    + "One thinning does not finish continuous-cover forestry: trees keep growing, openings close again, and you will reassess and act again in later years.\n\n"
                    + "After your first results, choose I've read the annual results to unlock further years. Esc or Walk the forest returns you to the trees; Open Work Plan returns to planning.";
            default:
                return "You take over a Sitka spruce plantation planted 20 years ago. Your aim is continuous-cover forestry (CCF): managing the forest through repeated, selective work on individual trees, "
                    + "while always keeping tree cover and letting new trees establish. CCF is a process, not one thinning and not one ideal shape of forest. Judge each decision by the forest you leave behind.\n\n"
                    + "You make decisions here, in the forest. Look at the ground for light, young trees and browsing; press E on a tree to inspect it. Start with the trees you want to keep.\n\n"
                    + "The status panel shows the year, cash and how many forest objectives are met. M opens the Stand Map, Tab the Work Plan, O your lessons. F5 saves and F9 loads.\n\n"
                    + "Esc releases the mouse; click to look around again. Press F1 at any time to reopen this help.";
        }
    }
}

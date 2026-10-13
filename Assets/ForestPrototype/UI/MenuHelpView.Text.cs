// Wording for the first-use and F1 help screens. Kept free of UI Toolkit and PlayerPrefs so the text can be checked
// offline on plain .NET (Tools/Verification/ThreeSpecies/OfflineLogic). This is one half of MenuHelpView; the
// screen itself is in MenuHelpView.cs.
public sealed partial class MenuHelpView
{
    public enum Menu { WalkingHud, TreeInspection, StandMap, WorkPlan, AnnualReview }

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

    // The short text shown the first time a screen is met, or null when that screen has no short form and shows its full
    // Explanation instead. The first-use screens stay brief so they do not pile up in front of the forest; the full
    // Explanation is always one click ("More detail") or F1 away.
    public static string Summary(Menu menu)
    {
        switch (menu)
        {
            case Menu.WalkingHud:
                return "You manage a small Sitka spruce plantation. Your aim is to begin converting it toward continuous-cover forestry: a forest that keeps its tree cover while it renews gradually, rather than being cut all at once.\n\n"
                    + "Forestry decisions play out over years. You look at the trees, decide which to keep and which to remove, commission the work in the Work Plan, then let time pass and see how the forest responds. There is no single perfect prescription to find: expect to revisit your management as the forest changes.\n\n"
                    + "Start by looking around and pressing E on a tree to inspect it.";
            default:
                return null;
        }
    }

    public static string SummaryTitle(Menu menu)
    {
        switch (menu)
        {
            case Menu.WalkingHud: return "Your role · Scenario One";
            default: return Title(menu);
        }
    }

    public static string Explanation(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeInspection:
                return "Start with a tree worth keeping, then inspect its competitors. Inspect a tree with E to understand its size, growth, competition and management state. Use this before deciding which trees to favour or remove.\n\n"
                    + "DBH is trunk diameter measured at breast height. Crown/light describes growing space and light; the light value here is measured at the tree's ground cell. Competition shows whether neighbours restrict growth.\n\n"
                    + "A Crop Tree is selected to retain and favour for future development. A Fell mark proposes removal. X and C change these marks; they do not execute work. You choose the trees—this screen does not select a correct answer.\n\n"
                    + "Crop Trees are trees you choose to favour for the future. Nearby trees can reduce their growing space, but being small or suppressed does not by itself make a tree a problem. Inspect which neighbours contribute most to competition before deciding what to remove. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\n"
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
                    + "Decide what should happen in the forest; review how it will happen and what it costs here. The residual-stand summary describes what remains; it does not decide the forest structure for you. Use Tab or Back to forest to return.";
            case Menu.AnnualReview:
                return "After advancing time, the Annual Review shows what work happened and how the forest and finances changed. Read it before planning the next cycle.\n\n"
                    + "WORK DONE: what did I do, and did any jobs fail?\nMONEY: what did it cost or earn, and what cash remains?\nFOREST: how did growth, light, regeneration and habitat respond?\n\n"
                    + "One thinning does not finish continuous-cover forestry. Walk back, inspect the forest response and reassess before the next intervention. Openings can create an opportunity for regeneration; seed, light, browsing and vegetation affect the outcome. Use these results to decide what to inspect next. This screen records outcomes; it does not choose or execute forestry work.\n\n"
                    + "Close Help and read the three sections. After your first results, choose I've read the annual results to unlock further year advances. Use Esc or Walk the forest to return and inspect the changes; Open Work Plan returns to planning.";
            default:
                return "Your role: you manage a small Sitka spruce plantation and begin converting it toward continuous-cover forestry. Decisions play out over years, and you revisit them as the forest changes.\n\n"
                    + "Continuous-cover forestry is a repeated process of selective management while keeping tree cover, not one thinning. Start with trees worth keeping: the forest you leave behind matters. More felling is not automatically better.\n\n"
                    + "The HUD shows the current year, cash, objectives and information about the ground or tree you are looking at. Current objective progress appears in the status panel.\n\n"
                    + "The forest is where you make management decisions. Look at ground to check light, regeneration and browsing conditions, or press E while looking at a tree to inspect it. Use these observations before marking work.\n\n"
                    + "A waypoint's direction and distance appear here when one is active. M opens the Stand Map; Tab opens the Work Plan to review and approve jobs. The HUD itself does not approve work.\n\n"
                    + "O opens Learning objectives: start with simple map steps, view current forest success objectives, then revisit map layers and forestry lessons as you need them. Progress is remembered on this device, with no first-year deadline.\n\n"
                    + "Close this introduction to walk. Esc releases the mouse; click to resume looking around. Press F1 any time to revisit this screen's help.";
        }
    }
}

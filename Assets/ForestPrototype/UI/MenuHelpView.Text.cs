// Wording for the first-use and F1 help screens. Kept free of UI Toolkit and PlayerPrefs so the text can be checked
// offline on plain .NET (Tools/Verification/ThreeSpecies/OfflineLogic). This is one half of MenuHelpView; the
// screen itself is in MenuHelpView.cs.
public sealed partial class MenuHelpView
{
    public enum Menu { WalkingHud, TreeInspection, StandMap, WorkPlan, AnnualReview, Rack }

    public static string Title(Menu menu)
    {
        switch (menu)
        {
            case Menu.TreeInspection: return "Tree Inspection · understand one tree";
            case Menu.StandMap: return "Stand Map · find where to look";
            case Menu.WorkPlan: return "Work Plan · review and approve jobs";
            case Menu.AnnualReview: return "Annual Review · learn from the year";
            case Menu.Rack: return "Extraction racks · open machine routes";
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
                return "You are managing an even-aged Sitka spruce plantation: the trees were planted at the same time.\n\n"
                    + "Your goal is to guide it toward continuous-cover forestry: keeping tree cover as the woodland renews, while keeping it productive and making room for the next generation of trees.\n\n"
                    + "You will learn the management process one step at a time. Start by creating access racks so forestry work can take place through the stand.\n\n"
                    + "Close this introduction, then press O for your current objective, or B to choose the start of your first rack.";
            case Menu.Rack:
                return "Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Click a tree or ground point where the rack should start. The start stays fixed while you aim. The preview extends forward only. Click again to add it to the Work Plan; right-click reselects the start, and Esc cancels. Nothing is cut until you approve the plan.";
            case Menu.TreeInspection:
                return "Start with a tree worth keeping, then see what competes with it. The card shows its size and how crowded it is; press V for more detail. Press C to mark a Crop Tree (keep and favour) or X to propose felling. Marks are only proposals: nothing is cut until you approve work in the Work Plan.";
            case Menu.StandMap:
                return "The map helps you decide where to look, not what to do. Choose a layer, select a square to read it, set a waypoint, then walk there and inspect the site yourself. It cannot mark, clear, plant or approve anything remotely.";
            case Menu.WorkPlan:
                return "The Work Plan collects your marks and planting spots into jobs you can review and approve. Check each cost, then approve the plan. Approval alone changes nothing: Advance one year carries out the approved jobs, and jobs you have not approved wait.";
            default:
                // The Annual Review has no first-use screen: its short guidance sits in the review itself (AnnualReviewReadPrompt).
                return null;
        }
    }

    // Shown inside the Annual Review, beside the button that acknowledges the first results, instead of a help screen opening
    // on top of the review. The detailed explanation stays on Help [F1].
    public const string AnnualReviewReadPrompt = "Read WORK DONE, MONEY and FOREST. One thinning does not finish continuous-cover forestry: when you have read them, walk back, inspect how the forest responded and reassess before the next intervention.";

    public static string SummaryTitle(Menu menu)
    {
        switch (menu)
        {
            case Menu.WalkingHud: return "Scenario One — Transforming a Sitka Plantation";
            default: return Title(menu);
        }
    }

    public static string Explanation(Menu menu)
    {
        switch (menu)
        {
            case Menu.Rack:
                return "An extraction rack is a straight corridor of felled trees across the plantation. The harvesting machine drives along it, so the rest of the stand can be thinned later without driving through standing trees. Racks are permanent extraction routes: they stay open for this and future thinnings.\n\n"
                    + "Press B in the forest to create a rack. Click the intended starting tree or ground point: its ring and post mark the fixed origin. Aim to pivot the violet corridor forward from that point; the arrow shows the direction and violet rings mark the trees it would take. The mouse wheel adjusts the direction. Click or press Enter again to add it to the Work Plan; right-click reselects the start; Esc or B cancels. No work is added by choosing a start.\n\n"
                    + "This plantation follows an authored rack plan: five evenly spaced racks across the planted rows, with seven rows left standing between racks. The status line counts how many are in the Work Plan. After your first rack, the Stand Map can plan the rest: open it with M and choose Plan rack, which marks the squares of a planned rack that are still to do (◇) and those already in the Work Plan (✓).\n\n"
                    + "A rack keeps any Crop Tree on its line. Racks are felled in one contractor visit together with any other felling you have planned, so the visit minimum is shared. A rack is still ordinary felling work: approve it in the Work Plan [Tab], then advance a year.";
            case Menu.TreeInspection:
                return "Start with a tree worth keeping, then inspect its competitors. Inspect a tree with E (or left-click) to understand its size, growth, competition and management state. Use this before deciding which trees to favour or remove.\n\n"
                    + "DBH is trunk diameter measured at breast height. Competition shows whether neighbours restrict growth; the competition index (CI) is one number combining how large and how close the neighbouring trees are, and a higher number means more competition. Press V on the card for the rest: stem volume, light on the ground at the tree's base, recent growth, wind exposure and pruning.\n\n"
                    + "A Crop Tree is selected to retain and favour for future development. A Fell mark proposes removal. X and C change these marks; they do not execute work.\n\n"
                    + "Crop Trees are trees you choose to favour for the future. Nearby trees can reduce their growing space, but being small or suppressed does not by itself make a tree a problem. Inspect which neighbours contribute most to competition before deciding what to remove. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\n"
                    + "Close Help, then press E to close inspection and return to walking.";
            case Menu.StandMap:
                return "The Stand Map helps you find patterns across the forest that are difficult to see among the trees. Open it with M when looking for a site to inspect. The map is divided into squares of 5 m × 5 m, each named by a letter and a number.\n\n"
                    + "Use Light for darker/brighter areas, Regeneration for young growth, Browsing / protection for deer browsing (deer eating the shoots of young trees) and the shelters that protect planted saplings, and Fell & crop marks for your marked trees.\n\n"
                    + "Select a square, read its information, then choose Set Waypoint. Close the map with M, Esc or Back to forest. Follow the HUD direction and distance, then inspect the site directly before deciding what to do.\n\n"
                    + "The map helps you find where to look. It cannot mark, clear, plant or approve work remotely, and it does not make the forestry decision for you.";
            case Menu.WorkPlan:
                return "The Work Plan collects decisions made in the forest into jobs you can review and approve. Open it with Tab after marking trees or planting/clearance sites; your marks join the plan when you open it.\n\n"
                    + "Review labour, material costs, cash after approved work and expected timber value. A contractor's minimum job charge can make a small harvest expensive. Some tasks let you choose contractor or landowner execution.\n\n"
                    + "Review or remove jobs, choose available executors and purchase nursery stock here. Approve Work Plan commits the plan; Advance one year resolves approved jobs and forest change. Approval alone does not immediately change the forest.\n\n"
                    + "Decide what should happen in the forest; review how it will happen and what it costs here. The residual stand is what remains standing after the planned work, and its summary (including basal area, the trunk cross-section area per hectare) describes what remains; it does not decide the forest structure for you. Use Tab or Back to forest to return.";
            case Menu.AnnualReview:
                return "After advancing time, the Annual Review shows what work happened and how the forest and finances changed. Read it before planning the next cycle.\n\n"
                    + "WORK DONE: what did I do, and did any jobs fail?\nMONEY: what did it cost or earn, and what cash remains?\nFOREST: how did growth, light, regeneration and habitat respond?\n\n"
                    + "Average forest-floor light is the mean of the light reaching the ground over every 5 m × 5 m square of the stand, from 0 (dark) to 1 (open). Areas with young regeneration counts the squares where young trees are present; the scenario objective Young trees present asks for a minimum number of them. Neither number tells you which tree to fell.\n\n"
                    + "One thinning does not finish continuous-cover forestry. Walk back, inspect the forest response and reassess before the next intervention. Openings can create an opportunity for regeneration; seed, light, browsing and vegetation affect the outcome. Use these results to decide what to inspect next. This screen records outcomes; it does not choose or execute forestry work.\n\n"
                    + "Close Help and read the three sections. After your first results, choose I've read the annual results to unlock further year advances. Use Esc or Walk the forest to return and inspect the changes; Open Work Plan returns to planning.";
            default:
                return "Your role: you manage a small Sitka spruce plantation and begin converting it toward continuous-cover forestry. Decisions play out over years, and you revisit them as the forest changes.\n\n"
                    + "Continuous-cover forestry is a repeated process of selective management while keeping tree cover, not one thinning. Start with trees worth keeping: the forest you leave behind matters. More felling is not automatically better.\n\n"
                    + "The HUD shows the current year, cash, objectives and information about the ground or tree you are looking at. Current objective progress appears in the status panel.\n\n"
                    + "The forest is where you make management decisions. Look at ground to check light, regeneration and browsing conditions (browsing is deer eating the shoots of young trees), or press E (or left-click) while looking at a tree to inspect it. Use these observations before marking work.\n\n"
                    + "A waypoint's direction and distance appear here when one is active. M opens the Stand Map; Tab opens the Work Plan to review and approve jobs. The HUD itself does not approve work.\n\n"
                    + "O opens Learning objectives: the current forest success objectives, then short lessons in a suggested order. The first lessons follow a first cycle: create extraction racks, approve the work, advance a year and read the Annual Review. Inspecting trees, Crop Trees and selective felling follow once the racks are open. Map practice, pruning, deadwood, planting and clearance wait until you want them. Progress is remembered on this device, with no first-year deadline.\n\n"
                    + ControlsReference.KeysLine + "\n\n"
                    + "Close this introduction to walk. Esc releases the mouse; click to resume looking around. Press F1 any time to revisit this screen's help.";
        }
    }
}

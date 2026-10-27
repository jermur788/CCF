// The keys and mouse actions of Scenario One, in one place so the startup Controls / Help card and the in-game Walking HUD
// help cannot drift apart. Pure (no UnityEngine) so Tools/Verification/ScenarioOneUx can check it on plain .NET.
//
// Rows are {key label, what it does}. Keep each description short: the startup card shows them in a narrow list. The key
// labels W A S D, Mouse, E, M, Tab, O, F1, Esc, F5 / F9 and F10 are asserted by the startup-screen verification.
public static class ControlsReference
{
    public static readonly string[][] Rows =
    {
        new[] { "W A S D", "Walk. Hold Shift to run, Space to jump" },
        new[] { "Mouse", "Look around. Esc frees the mouse; click to take it back" },
        new[] { "E", "Inspect the tree or object you are looking at. V on the card shows more detail" },
        new[] { "Left-click", "Also inspects the tree you look at; after Esc the first click only retakes the mouse" },
        new[] { "B", "Access rack: click a start, aim forward, click to add; right-click reselects, Esc cancels" },
        new[] { "X", "Mark the tree for felling: a proposal only. X again removes the mark" },
        new[] { "C", "Mark it as a Crop Tree to keep and favour; its competitors appear on the card" },
        new[] { "1 – 5", "On a Crop Tree's card, show its competitors in the forest (in planting mode, 1 and 2 pick the stock)" },
        new[] { "G", "Planting mode: click to mark a planting spot (buy saplings in the Work Plan first)" },
        new[] { "U", "Plan clearance of competing vegetation where you are looking" },
        new[] { "M", "Stand Map: find where to look" },
        new[] { "Tab", "Work Plan: review, approve and advance a year" },
        new[] { "O", "Learning objectives and lessons" },
        new[] { "F1", "Help for the screen you are on" },
        new[] { "Esc", "Free the mouse, or close the open panel" },
        new[] { "F5 / F9", "Save / Load" },
        new[] { "F10", "Session menu: new scenario, save, load, quit" },
    };

    // The same controls as one line, for the full Walking HUD help.
    public const string KeysLine =
        "Keys: B extraction rack · E or left-click inspect a tree · V more detail · X mark for felling · C mark a Crop Tree · 1–5 show a Crop Tree's competitors · "
        + "G planting mode · U plan clearance · M Stand Map · Tab Work Plan · O Learning objectives · F1 Help · F5 / F9 Save / Load · F10 Session menu.";
}

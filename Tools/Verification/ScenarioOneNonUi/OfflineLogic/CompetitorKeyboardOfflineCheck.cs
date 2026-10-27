// Offline check for the competitor keyboard rules (UI/CompetitorKeyboardTarget.cs): choosing a competitor with 1-5, letting go, switching,
// keeping the same tree chosen when the list is rebuilt, and which row X may act on. Pure logic on plain .NET; the Unity gate
// CompetitorKeyboardVerification.cs checks the same behaviour against the live game and real key events.
//
// Prints NONUI_KEYBOARD_PASS / NONUI_KEYBOARD_FAIL.
using System;
using System.Collections.Generic;

public static class CompetitorKeyboardOfflineCheck
{
    private static int failures, checks;

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    public static int Run()
    {
        Toggle();
        Rebind();
        Target();
        Sequence();
        Load();
        Console.WriteLine(failures == 0 ? $"NONUI_KEYBOARD_PASS checks={checks}" : $"NONUI_KEYBOARD_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Toggle()
    {
        for (int i = 0; i < 5; i++)
            Check(CompetitorKeyboardTarget.Toggle(-1, i, 5) == i, "key " + (i + 1) + " chooses row " + (i + 1) + " from nothing");
        for (int i = 0; i < 5; i++)
            Check(CompetitorKeyboardTarget.Toggle(i, i, 5) == -1, "the same key " + (i + 1) + " again lets go");
        Check(CompetitorKeyboardTarget.Toggle(0, 3, 5) == 3, "another number switches the choice");
        Check(CompetitorKeyboardTarget.Toggle(3, 0, 5) == 0, "switching back works");
        Check(CompetitorKeyboardTarget.Toggle(-1, 3, 3) == -1, "a number with no row behind it chooses nothing");
        Check(CompetitorKeyboardTarget.Toggle(2, 4, 3) == 2, "a number with no row behind it keeps the existing choice");
        Check(CompetitorKeyboardTarget.Toggle(1, 1, 0) == 1, "an empty list cannot change a stale index");
        Check(CompetitorKeyboardTarget.Toggle(1, -1, 5) == 1, "a negative key index is ignored");
    }

    private static void Rebind()
    {
        var ids = new List<string> { "A", "B", "C", "D", "E" };
        Check(CompetitorKeyboardTarget.Rebind("C", ids) == 2, "the chosen tree is found where it is listed");
        var reordered = new List<string> { "C", "A", "B", "D", "E" };
        Check(CompetitorKeyboardTarget.Rebind("C", reordered) == 0, "the same tree stays chosen when it ranks differently");
        Check(CompetitorKeyboardTarget.Rebind("C", reordered) != CompetitorKeyboardTarget.Rebind("A", reordered), "different trees never share a row");
        var dropped = new List<string> { "A", "B", "D", "E", "F" };
        Check(CompetitorKeyboardTarget.Rebind("C", dropped) == -1, "a tree that is no longer listed lets go instead of pointing at its old row");
        Check(CompetitorKeyboardTarget.Rebind("", ids) == -1, "no remembered tree: nothing chosen");
        Check(CompetitorKeyboardTarget.Rebind(null, ids) == -1, "null remembered tree: nothing chosen");
        Check(CompetitorKeyboardTarget.Rebind("C", null) == -1, "no list: nothing chosen");
        Check(CompetitorKeyboardTarget.Rebind("C", new List<string>()) == -1, "empty list: nothing chosen");
    }

    private static void Target()
    {
        Check(CompetitorKeyboardTarget.TargetIndex(true, 2, 5) == 2, "X acts on the chosen row while the list is showing");
        Check(CompetitorKeyboardTarget.TargetIndex(false, 2, 5) == -1, "a closed list is never a target");
        Check(CompetitorKeyboardTarget.TargetIndex(true, -1, 5) == -1, "nothing chosen: X keeps acting on the aimed tree");
        Check(CompetitorKeyboardTarget.TargetIndex(true, 4, 3) == -1, "a stale index beyond the listed rows is never a target");
        Check(CompetitorKeyboardTarget.TargetIndex(true, 0, 0) == -1, "an empty list is never a target");
    }

    // A whole visit: open the list, choose, switch, let go, close, reopen on another tree, rebuild after a mark.
    private static void Sequence()
    {
        var listed = new List<string> { "A", "B", "C", "D", "E" };
        int selected = -1; string remembered = "";
        void Choose(int next) { selected = next; remembered = next >= 0 && next < listed.Count ? listed[next] : ""; }
        Choose(CompetitorKeyboardTarget.Toggle(selected, 2, listed.Count));
        Check(selected == 2 && remembered == "C" && CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == 2, "1: key 3 chooses C and X would act on C");
        Choose(CompetitorKeyboardTarget.Toggle(selected, 4, listed.Count));
        Check(selected == 4 && remembered == "E" && CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == 4, "2: key 5 switches to E and X now acts on E, not C");
        // A mark rebuilds the list: ranks are the same here, the choice must survive.
        Choose(CompetitorKeyboardTarget.Rebind(remembered, listed));
        Check(selected == 4 && remembered == "E", "3: marking E keeps E chosen after the list is rebuilt");
        // Another year: E is no longer among the five listed.
        listed = new List<string> { "A", "B", "C", "D", "F" };
        Choose(CompetitorKeyboardTarget.Rebind(remembered, listed));
        Check(selected == -1 && CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == -1, "4: a tree that fell out of the list is let go, so X cannot hit row 5's new tree");
        Choose(CompetitorKeyboardTarget.Toggle(selected, 0, listed.Count));
        Choose(CompetitorKeyboardTarget.Toggle(selected, 0, listed.Count));
        Check(selected == -1 && CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == -1, "5: the same key twice leaves competitor mode");
        // The list closes: the owner clears the choice (as CompetitorAssessment.Update does), whatever X is pressed afterwards.
        Choose(CompetitorKeyboardTarget.Toggle(selected, 1, listed.Count));
        bool showing = false;
        Check(CompetitorKeyboardTarget.TargetIndex(showing, selected, listed.Count) == -1, "6: with the list closed X never acts on a stale choice");
        Choose(-1);
        listed = new List<string> { "P", "Q", "R" };
        Choose(CompetitorKeyboardTarget.Rebind(remembered, listed));
        Check(selected == -1, "7: a list for another Crop Tree starts with nothing chosen");
    }

    // A save load or a new scenario replaces the world under the list. The owner (CompetitorAssessment) watches the scenario's
    // session version and clears the choice; the live gate rechecks the same cleanup through a real LoadData, including X afterwards
    // and Crop Tree protection (a marking-manager rule, not a selection rule).
    private static void Load()
    {
        var listed = new List<string> { "A", "B", "C", "D", "E" };
        int selected = -1; string remembered = "";
        int worldVersion = 7, chosenWorldVersion = worldVersion;
        void Choose(int next) { selected = next; remembered = next >= 0 && next < listed.Count ? listed[next] : ""; }
        void ResetIfNewWorld()
        {
            if (!CompetitorKeyboardTarget.WorldChanged(chosenWorldVersion, worldVersion)) return;
            chosenWorldVersion = worldVersion;
            selected = -1; remembered = ""; // what CompetitorAssessment.Update does on a world-version change
        }

        Choose(CompetitorKeyboardTarget.Toggle(selected, 2, listed.Count));
        Check(selected == 2 && remembered == "C", "1: key 3 chooses a competitor before the load");
        Check(CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == 2,
            "2: the chosen row is the active X target before the load");
        ResetIfNewWorld();
        Check(selected == 2 && remembered == "C", "3: the same world does not invalidate a live choice");

        worldVersion = 8; // the save is loaded
        Check(CompetitorKeyboardTarget.WorldChanged(chosenWorldVersion, worldVersion), "3: a load is a new world");
        ResetIfNewWorld();

        Check(selected == -1 && remembered == "",
            "4: a load clears the chosen row and the remembered tree id");
        Check(CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == -1,
            "5: after a load no competitor row is active for X");
        Check(CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) != 2,
            "6/7: X cannot act on the pre-load competitor and falls back to the aimed tree");
        Choose(CompetitorKeyboardTarget.Toggle(selected, 1, listed.Count));
        Check(selected == 1 && remembered == "B" && CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == 1,
            "8: a competitor can be chosen normally again after the load");
        Choose(-1);
        Check(CompetitorKeyboardTarget.TargetIndex(false, selected, listed.Count) == -1,
            "10: closing the list after a load lets the choice go, as before");
        Choose(CompetitorKeyboardTarget.Rebind(remembered, listed));
        Check(selected == -1 && CompetitorKeyboardTarget.TargetIndex(true, selected, listed.Count) == -1,
            "10: reopening competitor mode after a load starts with nothing chosen");
    }
}

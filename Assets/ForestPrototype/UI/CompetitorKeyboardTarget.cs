using System.Collections.Generic;

// The rules behind choosing a competitor with the number keys 1-5 and marking it with X, kept apart from Unity so they can be checked on
// plain .NET (Tools/Verification/ScenarioOneNonUi/OfflineLogic). The highlight on the tree, the row in the competitor list and the tree X
// acts on all come from one choice: the id of the listed tree the player picked. A row number alone could silently point at a different
// tree once the list is rebuilt, so the id is what is kept.
public static class CompetitorKeyboardTarget
{
    // A number key picks the listed row with that number; the same number again lets go. A number with no row behind it changes nothing.
    public static int Toggle(int selectedIndex, int pressedIndex, int listedCount)
    {
        if (pressedIndex < 0 || pressedIndex >= listedCount)
            return selectedIndex;
        return selectedIndex == pressedIndex ? -1 : pressedIndex;
    }

    // The list is rebuilt after a mark, a new year or a load. The same tree stays chosen wherever it now ranks; if it is no longer listed
    // nothing is chosen.
    public static int Rebind(string selectedTreeId, IReadOnlyList<string> listedIds)
    {
        if (string.IsNullOrEmpty(selectedTreeId) || listedIds == null)
            return -1;
        for (int i = 0; i < listedIds.Count; i++)
            if (listedIds[i] == selectedTreeId)
                return i;
        return -1;
    }

    // The row X acts on, or -1. Only while the list is showing for the inspected Crop Tree: with no list, or nothing chosen, X keeps
    // working on the tree under the reticle exactly as before.
    public static int TargetIndex(bool listShowing, int selectedIndex, int listedCount)
        => listShowing && selectedIndex >= 0 && selectedIndex < listedCount ? selectedIndex : -1;
}

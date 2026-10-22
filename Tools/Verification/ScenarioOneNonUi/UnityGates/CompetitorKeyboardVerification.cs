using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable live gate for the competitor keyboard targeting (CompetitorAssessment / CompetitorKeyboardTarget / ForestTreeMarkingManager
// .KeyboardTarget): SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1, item 2. It presses the real keys through the Input System: 1-5 choose
// a listed competitor of the inspected Crop Tree, X marks / unmarks that same tree for felling without the reticle on it, and the choice
// is let go when the list closes, another tree is inspected, planting starts or a save is loaded.
// Run with run_unity_harness.py in a graphical Editor with focus (--private-display --keep-focus), under the Unity lock; it stages and
// removes the copy and its .meta. It never writes the player's save file or the scene. The rules alone are also checked on plain .NET
// (Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py).
//
// Prints KEYBOARD_CHECK <what>, then KEYBOARD_PASS checks=N or KEYBOARD_FAIL <reason>.
public static class CompetitorKeyboardVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "CompetitorKeyboardVerification.Begin"))
            new GameObject("Disposable competitor keyboard verification").AddComponent<CompetitorKeyboardVerificationRunner>();
    }
}

public sealed class CompetitorKeyboardVerificationRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestPlayer player;
    private ForestTreeMarkingManager marks;
    private ForestSaveController saves;
    private int checks;

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) throw new InvalidOperationException(what + (detail.Length > 0 ? " [" + detail + "]" : ""));
        checks++;
        Debug.Log("KEYBOARD_CHECK " + what + (detail.Length > 0 ? " [" + detail + "]" : ""));
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 8; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        Debug.Log(failure == null ? "KEYBOARD_PASS checks=" + checks : "KEYBOARD_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    // ---- input helpers (the same synthetic key events the other Editor gates use) ----
    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private static readonly Key[] DigitKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5 };
    private static Key Digit(int number) => DigitKeys[number - 1];
    private IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }

    private static ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce")
        .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private static ForestTree ById(string id) => Trees().First(t => t.TreeId == id);

    // Stand 3 m south of a tree so the walking reticle picks it (as the other gates do), and wait for the aim to settle.
    private IEnumerator Aim(Func<ForestTree, bool> accept, Action<ForestTree> found)
    {
        foreach (ForestTree tree in Trees().Where(t => Mathf.Abs(t.transform.position.x) < 12f && Mathf.Abs(t.transform.position.z) < 12f).Where(accept).Take(60))
        {
            player.transform.position = tree.transform.position - new Vector3(0, 0, 3);
            yield return Frames(8);
            if (marks.AimedTree == tree) { found(tree); yield break; }
        }
        found(null);
    }

    private IEnumerator Inspect(ForestTree crop)
    {
        if (player.IsInspecting) yield return Press(Key.E);
        player.transform.position = crop.transform.position - new Vector3(0, 0, 3);
        yield return Frames(8);
        Check(marks.AimedTree == crop, "the reticle is on the Crop Tree before inspecting it", crop.TreeId);
        if (!crop.IsCropTree) yield return Press(Key.C);
        Check(crop.IsCropTree, "the tree is a Crop Tree", crop.TreeId);
        yield return Press(Key.E);
        yield return Frames(8);
        Check(player.IsInspecting && player.InspectedTree == crop, "the Crop Tree is being inspected", crop.TreeId);
        Check(ui.Competitors.ReportTree == crop && ui.Competitors.ListedCount == 5, "its competitor list shows five neighbours", "listed " + ui.Competitors.ListedCount);
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; player = ui.Player; marks = ui.Marking; saves = FindFirstObjectByType<ForestSaveController>();
        Check(ui != null && manager != null && player != null && marks != null && saves != null, "the scene has its UI, manager, player, marking manager and save controller");
        ui.CloseAll(); ui.CloseHelp();
        yield return Frames(4);
        Debug.Log($"KEYBOARD_MEASURE cursorLock={Cursor.lockState} playerEnabled={player.enabled} (X needs the cursor locked: run with focus)");

        // ---- a Crop Tree with its competitor list ----
        ForestTree cropA = null;
        yield return Aim(t => !t.IsCropTree, t => cropA = t);
        Check(cropA != null, "a tree can be aimed at");
        yield return Inspect(cropA);
        var snapshot = new HashSet<string>(marks.GetMarkedIds());

        // ---- 1-5 choose the listed competitor, X marks and unmarks that same tree, with the reticle on the Crop Tree ----
        for (int k = 1; k <= 5; k++)
        {
            yield return Press(Digit(k));
            Check(ui.Competitors.SelectedIndex == k - 1, "key " + k + " chooses row " + k, "index " + ui.Competitors.SelectedIndex);
            string id = ui.Competitors.Report.Ranked[k - 1].Id;
            Check(ui.Competitors.SelectedTree != null && ui.Competitors.SelectedTree.TreeId == id, "the highlighted tree is the tree behind row " + k, id);
            Check(marks.AimedTree == cropA, "the reticle is still on the Crop Tree, not on the competitor", "aimed " + (marks.AimedTree != null ? marks.AimedTree.TreeId : "none"));

            yield return Press(Key.X);
            yield return Frames(6);
            ForestTree chosen = ById(id);
            Check(chosen.IsMarkedForFell, "X marks the chosen competitor for felling without hovering it", id);
            Check(cropA.IsCropTree && !cropA.IsMarkedForFell, "the Crop Tree stays the Crop Tree (X did not act on the aimed tree)", cropA.TreeId);
            Check(player.IsInspecting && player.InspectedTree == cropA, "the Crop Tree stays the parent context", cropA.TreeId);
            Check(ui.Competitors.SelectedTree != null && ui.Competitors.SelectedTree.TreeId == id
                && ui.Competitors.Report.Ranked[ui.Competitors.SelectedIndex].Id == id, "the same tree stays chosen after the list is rebuilt, wherever it now ranks", id);

            yield return Press(Key.X);
            yield return Frames(6);
            Check(!ById(id).IsMarkedForFell && marks.GetMarkedIds().All(snapshot.Contains), "X again unmarks it", id);
            yield return Press(Digit(ui.Competitors.SelectedIndex + 1));       // let go before the next row
            Check(ui.Competitors.SelectedIndex == -1 && ui.Competitors.SelectedTree == null, "pressing the same number again lets go");
        }

        // ---- switching numbers moves the target ----
        yield return Press(Digit(2));
        string second = ui.Competitors.Report.Ranked[1].Id;
        yield return Press(Digit(4));
        string fourth = ui.Competitors.Report.Ranked[3].Id;
        Check(ui.Competitors.SelectedTree.TreeId == fourth, "switching from 2 to 4 moves the target", fourth);
        yield return Press(Key.X);
        yield return Frames(6);
        Check(ById(fourth).IsMarkedForFell && !ById(second).IsMarkedForFell, "X marks row 4's tree and not row 2's");
        yield return Press(Key.X);
        yield return Frames(6);
        yield return Press(Digit(ui.Competitors.SelectedIndex + 1));
        Check(marks.GetMarkedIds().All(snapshot.Contains), "everything is unmarked again");

        // ---- a Crop Tree is never silently turned into a felling mark: X refuses it, by keyboard and by mouse ----
        string protectedId = ui.Competitors.Report.Ranked[0].Id;
        ForestTree protectedTree = ById(protectedId);
        marks.Mark(protectedTree, TreeMarkType.CropTree, false);
        yield return Frames(8);
        int protectedRow = Enumerable.Range(0, ui.Competitors.ListedCount).First(i => ui.Competitors.Report.Ranked[i].Id == protectedId);
        yield return Press(Digit(protectedRow + 1));
        Check(ui.Competitors.SelectedTree != null && ui.Competitors.SelectedTree.TreeId == protectedId, "a competitor that is itself a Crop Tree can be chosen", protectedId);
        yield return Press(Key.X);
        yield return Frames(6);
        Check(protectedTree.IsCropTree && !protectedTree.IsMarkedForFell, "X by keyboard refuses a Crop Tree: the Crop Tree mark is kept", protectedId);
        Check(marks.TransientMessage.Contains("Crop Tree") && marks.TransientMessage.Contains("C"), "and the player is told to remove the Crop Tree mark first", marks.TransientMessage);
        yield return Press(Digit(ui.Competitors.SelectedIndex + 1));         // let go
        marks.Unmark(protectedTree, false);
        yield return Press(Key.E);                                            // leave inspection, then aim at the Crop Tree itself
        yield return Frames(6);
        yield return Aim(t => t == cropA, t => { });
        Check(marks.AimedTree == cropA && cropA.IsCropTree, "the reticle is on a Crop Tree with nothing chosen on the keyboard");
        yield return Press(Key.X);
        yield return Frames(6);
        Check(cropA.IsCropTree && !cropA.IsMarkedForFell, "X by mouse aim refuses a Crop Tree too: same rule on both paths", cropA.TreeId);
        Check(marks.TransientMessage.Contains("Crop Tree"), "with the same feedback", marks.TransientMessage);
        yield return Inspect(cropA);

        // ---- a chosen competitor does not outlive the list ----
        yield return Press(Digit(3));
        string third = ui.Competitors.Report.Ranked[2].Id;
        Check(ui.Competitors.SelectedTree != null && ui.Competitors.SelectedTree.TreeId == third, "row 3 is chosen", third);
        yield return Press(Key.E);                                  // leave inspection: the list closes
        yield return Frames(8);
        Check(!player.IsInspecting && ui.Competitors.SelectedTree == null, "closing the list lets the choice go");
        ForestTree other = null;
        yield return Aim(t => !t.IsCropTree && t.TreeId != third, t => other = t);
        Check(other != null, "another tree can be aimed at");
        yield return Press(Key.X);
        yield return Frames(6);
        Check(other.IsMarkedForFell && !ById(third).IsMarkedForFell, "with no list, X still marks the tree under the reticle and never the old choice", other.TreeId);
        yield return Press(Key.X);
        yield return Frames(6);
        Check(!other.IsMarkedForFell, "and X unmarks it", other.TreeId);

        // ---- inspecting another Crop Tree starts with nothing chosen ----
        yield return Inspect(cropA);
        yield return Press(Digit(1));
        Check(ui.Competitors.SelectedTree != null, "row 1 is chosen on the first Crop Tree");
        yield return Press(Key.E);
        yield return Frames(4);
        ForestTree cropB = null;
        yield return Aim(t => !t.IsCropTree && Vector3.Distance(t.transform.position, cropA.transform.position) > 6f, t => cropB = t);
        Check(cropB != null, "a second tree can be aimed at");
        yield return Inspect(cropB);
        Check(ui.Competitors.SelectedIndex == -1 && ui.Competitors.SelectedTree == null, "a different Crop Tree starts with no competitor chosen");
        yield return Press(Digit(2));
        Check(ui.Competitors.SelectedTree != null && ui.Competitors.SelectedTree.TreeId == ui.Competitors.Report.Ranked[1].Id, "the choice belongs to the Crop Tree being inspected now");

        // ---- planting mode lets go: the number keys pick a species there ----
        manager.TryPurchaseStock("sessile-oak-sapling", 1);
        yield return Press(Key.G);
        if (player.IsPlantingMode)
        {
            yield return Frames(4);
            Check(ui.Competitors.SelectedTree == null, "planting mode lets the competitor choice go");
            yield return Press(Key.Escape);
            yield return Frames(4);
            Check(ui.Competitors.SelectedIndex == -1, "leaving planting mode does not bring a stale choice back");
        }
        else Debug.Log("KEYBOARD_UNVERIFIED planting mode could not be entered; its guard is not exercised");

        // ---- a save loaded while a competitor is chosen leaves no stale target ----
        if (!player.IsInspecting) yield return Inspect(cropB);
        yield return Press(Digit(3));
        Check(ui.Competitors.SelectedTree != null, "row 3 is chosen before the save is loaded");
        var markedBefore = new HashSet<string>(marks.GetMarkedIds());
        ForestSaveData data = saves.CaptureData();
        saves.LoadData(data, false);
        yield return Frames(12);
        Check(ui.Competitors.SelectedTree == null && ui.Competitors.SelectedIndex == -1, "after a load nothing is chosen, so X has no stale competitor to act on");
        Check(marks.GetMarkedIds().All(markedBefore.Contains), "the load did not leave a stray mark behind");
    }
}

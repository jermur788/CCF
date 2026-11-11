#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class Ccf381 : MonoBehaviour
{
    [Serializable] public class Command
    {
        public string action;
        public string arg;
        public int width;
        public int height;
        public float x;
        public float y;
        public float z;
    }
    static string Output => Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
    ScenarioOneUiRoot ui;
    public static void Begin()
    {
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        string path = Application.persistentDataPath;
        string save = (string)typeof(ForestSaveController).GetProperty("SavePath", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        if (string.IsNullOrEmpty(config) || !path.StartsWith(config + "/", StringComparison.Ordinal) || !save.StartsWith(path + "/", StringComparison.Ordinal))
        {
            Debug.LogError("C381_FAIL unsafe actual persistent/save paths");
            EditorApplication.Exit(2);
            return;
        }
        PlayerPrefs.SetInt("CCF381_isolation", 369);
        PlayerPrefs.Save();
        if (!File.Exists(Path.Combine(path, "prefs"))) { EditorApplication.Exit(3); return; }
        File.WriteAllText(Path.Combine(Output, "paths.txt"), "persistent=" + path + "\nF5/F9=" + save + "\nprefs=" + Path.Combine(path, "prefs"));
        Debug.Log("C381_PATHS_PROVEN " + path);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetSize(1280, 720);
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("Ccf381.Begin")) new GameObject("Disposable CCF381").AddComponent<Ccf381>();
    }
    public static void SetSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "CCF381 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }
    static IEnumerator Pause(int frames = 25) { for (int frame = 0; frame < frames; frame++) yield return null; }
    static bool Visible(VisualElement element)
    {
        for (VisualElement ancestor = element; ancestor != null; ancestor = ancestor.parent)
            if (ancestor.resolvedStyle.display == DisplayStyle.None || ancestor.resolvedStyle.visibility == Visibility.Hidden) return false;
        return true;
    }
    void Dump(string stem)
    {
        string state = "year=" + ui.Manager.CurrentEcologicalYear + " objectiveIndex=" + ui.Learning.CurrentTopicIndex
            + " title=" + ui.Learning.CurrentTitle + " instruction=" + ui.Learning.CurrentInstruction + " progress=" + ui.Learning.CurrentProgress
            + " unread=" + ui.NeedsAnnualReview + " racks=" + ui.Manager.PlannedRackCount + " entrances=" + ui.Manager.MarkedRackEntranceCount
            + " player=" + ui.Player.transform.position + " aimed=" + (ui.Marking.AimedTree != null ? ui.Marking.AimedTree.TreeId : "none")
            + " orders="+ui.Manager.WorkOrders.Count+" crops="+FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(t=>t.IsCropTree)+" hash="+ScenarioReferenceArchive.WorldHash(FindFirstObjectByType<ForestSaveController>().CaptureData())+" context="+(ui.Player.InspectedTree==null?"none":ui.Map.Review.ContextFor(ui.Player.InspectedTree)?.Decision.ToString())+" help=" + ui.Help.IsOpen + " transient=" + ui.Marking.TransientMessage;
        File.WriteAllText(Path.Combine(Output, stem + "-state.txt"), state);
        File.WriteAllLines(Path.Combine(Output, stem + "-ui.tsv"), ui.RootElement.Query<VisualElement>().ToList().Where(Visible)
            .Select(element => element.GetType().Name + "\t" + element.name + "\t" + element.worldBound + "\t" + element.enabledInHierarchy
            + "\t" + string.Join(",", element.GetClasses()) + "\t" + (element is TextElement text ? text.text.Replace("\n", " | ") : "")));
        Debug.Log("C381_STATE " + stem + " " + state);
    }
    IEnumerator Capture(string stem)
    {
        yield return Pause();
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        string name = stem + "-" + Screen.width + "x" + Screen.height;
        int positive = image.GetPixels32().Count(pixel => pixel.r > 8 || pixel.g > 8 || pixel.b > 8);
        File.WriteAllBytes(Path.Combine(Output, name + ".png"), image.EncodeToPNG());
        Dump(name);
        Debug.Log("C381_CAPTURE " + name + " positive=" + positive + "/" + (image.width * image.height));
        if (positive < image.width * image.height / 100)
        {
            Debug.LogError("C381_FAIL black capture; stop");
            EditorApplication.Exit(4);
        }
        Destroy(image);
    }
    void CheckAnnualValues(string stem)
    {
        var lines = new List<string>();
        var entries = ui.RootElement.Query<VisualElement>(className: "annual-value-entry").ToList().Where(item => Visible(item) && item.worldBound.width > 1).ToList();
        if (entries.Count < 4) throw new InvalidOperationException("Fewer than four actual annual value rows");
        foreach (var entry in entries)
        {
            var name = entry.Q<Label>(className: "annual-value-name");
            var amount = entry.Q<Label>(className: "annual-value-amount");
            Vector2 nameSize = name.MeasureTextSize(name.text, name.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            Vector2 valueSize = amount.MeasureTextSize(amount.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined);
            bool fit = name.contentRect.width >= 200 && nameSize.y <= name.contentRect.height + 2
                && valueSize.x <= amount.contentRect.width + 2 && valueSize.y <= amount.contentRect.height + 2
                && name.worldBound.yMax <= amount.worldBound.yMin + 1
                && amount.worldBound.xMax <= entry.worldBound.xMax + 1;
            lines.Add(name.text + "\t" + name.worldBound + "\t" + nameSize + "\t" + amount.text + "\t" + amount.worldBound + "\t" + valueSize + "\t" + amount.resolvedStyle.unityTextAlign + "\tfit=" + fit);
            if (!fit) { File.WriteAllLines(Path.Combine(Output, stem + "-money-fit.tsv"), lines); Debug.LogError("C381_FAIL Annual value fit: " + lines.Last()); EditorApplication.Exit(7); return; }
        }
        File.WriteAllLines(Path.Combine(Output, stem + "-money-fit.tsv"), lines);
        Debug.Log("C381_MONEY_FIT rows=" + entries.Count + " " + stem);
    }

    void CheckTagged(string stem)
    {
        var rows = new List<string>();
        foreach(var entry in ui.RootElement.Query<VisualElement>(className: "annual-text-entry").ToList())
        {
            var body = entry.Q<Label>(className: "annual-text-body");
            if(body == null || body.worldBound.width < 1) continue;
            var natural = body.MeasureTextSize(body.text, body.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            bool fit = body.contentRect.width >= 180 && natural.y <= body.contentRect.height + 2;
            rows.Add(body.text + "\t" + body.worldBound + "\t" + natural + "\tfit=" + fit);
        }
        File.WriteAllLines(Path.Combine(Output, stem + "-tagged-census.tsv"), rows);
    }
    IEnumerator Aim(string identity)
    {
        ForestTree tree = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Single(item => item.TreeId == identity);
        ui.CloseAll(); ui.CloseHelp();
        foreach (Vector3 direction in new[] { Vector3.back, Vector3.forward, Vector3.right, Vector3.left })
        {
            ui.Player.transform.position = tree.transform.position + direction * 2.5f;
            ui.Player.LookToward(tree.transform.position + Vector3.up * 1.4f);
            Physics.SyncTransforms();
            yield return Pause(15);
            if (ui.Marking.AimedTree == tree) break;
        }
        Dump("aim-" + identity);
    }
    IEnumerator Execute(Command command)
    {
        Debug.Log("C381_INPUT " + JsonUtility.ToJson(command));
        switch (command.action)
        {
            case "candidates":
File.WriteAllLines(Path.Combine(Output,"candidates.txt"),ui.Map.Review.Session.LastCandidates.Candidates.Select(c=>c.TreeId));break;
case "scrollcard":
var lab=ui.RootElement.Query<Label>().ToList().First(t=>t.text==command.arg&&Visible(t));var sc=lab.GetFirstAncestorOfType<ScrollView>();sc.scrollOffset=new Vector2(0,lab.worldBound.yMin-sc.contentContainer.worldBound.yMin-40);break;
case "mousepick":
var mouse=UnityEngine.InputSystem.Mouse.current.position.ReadValue();var pp=RuntimePanelUtils.ScreenToPanel(ui.RootElement.panel,new Vector2(mouse.x,Screen.height-mouse.y));var pe=ui.RootElement.panel.Pick(pp);
File.WriteAllText(Path.Combine(Output,command.arg+"-mousepick.txt"),"mouse="+mouse+" panel="+pp+" picked="+pe?.name+" text="+(pe as TextElement)?.text+" enabled="+pe?.enabledInHierarchy);break;
case "target":
var label=ui.RootElement.Query<Label>().ToList().First(t=>t.text==command.arg&&Visible(t));
var card=label.GetFirstAncestorOfType<VisualElement>();
var v=EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
var rect=(Rect)v.GetType().GetProperty("targetInView",BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance).GetValue(v);
var pos=v.position; var b=label.worldBound;var rb=ui.RootElement.worldBound;
File.WriteAllText(Path.Combine(Output,"target.txt"),(pos.x+rect.x+(b.center.x/rb.width)*rect.width)+","+(pos.y+rect.y+(b.center.y/rb.height)*rect.height));break;
case "progress": yield return ResidualProgress381.Run(ui); break;
            case "size": SetSize(command.width, command.height); break;
            case "show":
                ui.CloseAll(); ui.CloseHelp();
                if (command.arg == "objectives") ui.ShowObjectives();
                if (command.arg == "map") ui.ShowMap();
                if (command.arg == "workplan") ui.ShowWorkPlan();
                if (command.arg == "review") ui.ShowReview();
                break;
            case "dismiss": ui.CloseHelp(); break;
            case "scroll":
                VisualElement target = ui.RootElement.Q(command.arg);
                if (target == null) throw new InvalidOperationException("Missing scroll target " + command.arg);
                ScrollView scroll = target.GetFirstAncestorOfType<ScrollView>();
                if (scroll == null) throw new InvalidOperationException("No ancestor ScrollView");
                scroll.scrollOffset = new Vector2(0, target.worldBound.yMin - scroll.contentContainer.worldBound.yMin + command.y);
                break;
            case "click":
                Button button = ui.RootElement.Q<Button>(command.arg) ?? ui.RootElement.Query<Button>().ToList().FirstOrDefault(item => item.text == command.arg && Visible(item));
                if (button == null || !Visible(button) || !button.enabledInHierarchy) throw new InvalidOperationException("Unavailable button " + command.arg);
                typeof(Clickable).GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(button.clickable, new object[] { null });
                break;
            case "capture": yield return Capture(command.arg); break;
            case "annualfit": yield return Capture(command.arg); CheckAnnualValues(command.arg); CheckTagged(command.arg); break;
            case "toggle":
                Toggle toggle = ui.RootElement.Q<Toggle>(command.arg);
                if (toggle == null) throw new InvalidOperationException("Missing toggle " + command.arg);
                toggle.value = command.width != 0; break;
            case "foldout":
                Foldout foldout = ui.RootElement.Query<Foldout>().ToList().First(item => item.text.Contains(command.arg) && Visible(item));
                foldout.value = command.width != 0; break;
            case "focus": EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView")).Focus(); break;
            case "pick":
                var point = new Vector2(command.x, command.y);
                var picked = ui.RootElement.panel.Pick(point);
                File.WriteAllText(Path.Combine(Output, command.arg + "-pick.txt"), picked == null ? "none" : picked.GetType().Name + " " + picked.name + " enabled=" + picked.enabledInHierarchy + " classes=" + string.Join(",", picked.GetClasses()));
                break;
            case "dump": Dump(command.arg); break;
            case "aim": yield return Aim(command.arg); break;
            case "position":
                ui.CloseAll(); ui.CloseHelp(); ui.Player.transform.position = new Vector3(command.x, command.y, command.z);
                ui.Player.LookToward(new Vector3(command.width, command.y + 1.3f, command.height)); Physics.SyncTransforms(); break;
            case "entrance":
                Debug.Log("C381_ENTRANCE production action " + ui.Manager.MarkRackEntrance(int.Parse(command.arg)).Status); break;
            case "trees":
                File.WriteAllLines(Path.Combine(Output, "trees.tsv"), FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
                    .OrderBy(tree => tree.TreeId).Select(tree => tree.TreeId + "\t" + tree.transform.position.x + "\t" + tree.transform.position.z
                    + "\t" + tree.IsLiving + "\t" + tree.IsCropTree + "\t" + tree.IsMarkedForFell));
                File.WriteAllText(Path.Combine(Output, "world.json"), JsonUtility.ToJson(FindFirstObjectByType<ForestSaveController>().CaptureData(), true));
                break;
            case "exit": Debug.Log("C381_SESSION_ENDED coverage evaluated by operator, not automatic PASS"); EditorApplication.Exit(0); break;
            default: throw new InvalidOperationException("Unknown command " + command.action);
        }
        yield return Pause();
    }
    IEnumerator Start()
    {
        yield return Pause(100);
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        if (ui == null) { EditorApplication.Exit(5); yield break; }
        if (ui.Help.Onboarding.IsOpen) { ui.Help.Onboarding.Next(); ui.Help.Onboarding.Next(); ui.Help.Onboarding.Next(); }
        ui.CloseAll(); ui.CloseHelp();
        Dump("initial");
        string commands = Path.Combine(Output, "commands");
        Directory.CreateDirectory(commands);
        File.WriteAllText(Path.Combine(Output, "READY"), "actual isolated fresh world ready");
        double deadline = Time.realtimeSinceStartupAsDouble + 3300;
        while (Time.realtimeSinceStartupAsDouble < deadline)
        {
            if (File.Exists(Path.Combine(Output, "CONTROLLED_STOP"))) { EditorApplication.Exit(6); yield break; }
            string path = Directory.GetFiles(commands, "*.json").OrderBy(value => value, StringComparer.Ordinal).FirstOrDefault();
            if (path == null) { yield return null; continue; }
            Command command = JsonUtility.FromJson<Command>(File.ReadAllText(path));
            File.Move(path, path + ".running");
            var stack = new Stack<IEnumerator>(); stack.Push(Execute(command));
            bool failed = false;
            while (stack.Count > 0)
            {
                object current = null; bool moved = false;
                try { moved = stack.Peek().MoveNext(); if (moved) current = stack.Peek().Current; }
                catch (Exception exception) { File.WriteAllText(path + ".error", exception.ToString()); Debug.LogWarning("C381_COMMAND_ERROR " + exception); failed = true; break; }
                if (!moved) stack.Pop();
                else if (current is IEnumerator nested) stack.Push(nested);
                else yield return current;
            }
            File.Move(path + ".running", path + (failed ? ".failed" : ".done"));
        }
        Debug.Log("C381_SESSION_ENDED finite time bound");
        EditorApplication.Exit(0);
    }
}
#endif

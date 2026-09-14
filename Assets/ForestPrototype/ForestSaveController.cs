using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestSaveController : MonoBehaviour
{
    private const string SaveFileName = "forest-save.json";

    private ForestPlayer player;
    private ForestTree[] trees;
    private ForestBuildable[] buildables;
    private string message = "";
    private float messageTimer;
    private GUIStyle messageStyle;

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    private void Awake()
    {
        player = Object.FindFirstObjectByType<ForestPlayer>();
        trees = Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
        buildables = Object.FindObjectsByType<ForestBuildable>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.f5Key.wasPressedThisFrame)
            Save();
        else if (keyboard.f9Key.wasPressedThisFrame)
            Load();
    }

    public void Save()
    {
        ForestSaveData data = new ForestSaveData();
        if (player != null)
            data.wood = player.WoodCount;

        foreach (ForestTree tree in trees)
        {
            if (tree == null) continue;
            data.trees.Add(new TreeSaveData
            {
                treeId = tree.TreeId,
                stage = (int)tree.Stage,
                stageTimer = tree.StageTimer,
                chopProgress = tree.ChopProgress
            });
        }

        foreach (ForestBuildable buildable in buildables)
        {
            if (buildable == null) continue;
            data.buildables.Add(new BuildableSaveData
            {
                buildId = buildable.BuildId,
                built = buildable.IsBuilt
            });
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        SetMessage($"Game saved: wood {data.wood}, trees {data.trees.Count}, objects {data.buildables.Count}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            SetMessage("No save found (press F5 to save)");
            return;
        }

        ForestSaveData data = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(SavePath));
        if (data == null)
        {
            SetMessage("Save file unreadable");
            return;
        }

        if (player != null)
            player.WoodCount = data.wood;

        Dictionary<string, ForestTree> treesById = new Dictionary<string, ForestTree>();
        foreach (ForestTree tree in trees)
        {
            if (tree != null && !string.IsNullOrEmpty(tree.TreeId))
                treesById[tree.TreeId] = tree;
        }
        if (data.trees != null)
        {
            foreach (TreeSaveData saved in data.trees)
            {
                if (treesById.TryGetValue(saved.treeId, out ForestTree tree))
                    tree.RestoreState((ForestTreeStage)saved.stage, saved.stageTimer, saved.chopProgress);
            }
        }

        if (data.buildables != null)
        {
            foreach (BuildableSaveData saved in data.buildables)
            {
                foreach (ForestBuildable buildable in buildables)
                {
                    if (buildable != null && buildable.BuildId == saved.buildId)
                        buildable.RestoreBuiltState(saved.built);
                }
            }
        }

        SetMessage("Game loaded");
    }

    private void SetMessage(string text)
    {
        message = text;
        messageTimer = 3f;
    }

    private void OnGUI()
    {
        if (messageTimer <= 0f)
            return;

        if (messageStyle == null)
        {
            messageStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            messageStyle.normal.textColor = new Color(0.7f, 1f, 0.7f);
        }

        float width = Mathf.Min(760f, Screen.width - 32f);
        GUI.Box(new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height - 116f, width, 88f), message, messageStyle);
    }
}

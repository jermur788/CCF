using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestSaveController : MonoBehaviour
{
    private const string SaveFileName = "forest-save.json";

    private string message = "";
    private float messageTimer;
    private GUIStyle messageStyle;

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

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
        ForestPlayer player = Object.FindFirstObjectByType<ForestPlayer>();
        ForestTree[] trees = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestBuildable[] buildables = Object.FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        ForestSaveData data = new ForestSaveData();
        if (player != null)
            data.wood = player.CarriedWood;

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

        foreach (ForestWoodStorage storage in storages)
        {
            if (storage == null) continue;
            data.storages.Add(new WoodStorageSaveData
            {
                storageId = storage.StorageId,
                storedWood = storage.StoredWood
            });
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        SetMessage($"Game saved (v{ForestSaveData.CurrentVersion}): wood {data.wood}, trees {data.trees.Count}, objects {data.buildables.Count}, storages {data.storages.Count}");
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

        // Saves written before versioning load as version 1. Future migrations belong here.
        if (data.version <= 0)
            data.version = 1;
        if (data.version > ForestSaveData.CurrentVersion)
            Debug.LogWarning($"Save version {data.version} is newer than supported version {ForestSaveData.CurrentVersion}; loading best-effort.");

        ForestPlayer player = Object.FindFirstObjectByType<ForestPlayer>();
        ForestTree[] trees = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestBuildable[] buildables = Object.FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (player != null)
            player.RestoreCarriedWood(data.wood);

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

        if (data.storages != null)
        {
            foreach (WoodStorageSaveData saved in data.storages)
            {
                foreach (ForestWoodStorage storage in storages)
                {
                    if (storage != null && storage.StorageId == saved.storageId)
                        storage.RestoreStoredWood(saved.storedWood);
                }
            }
        }

        ForestEcologyController ecology = Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology != null)
            ecology.RecomputeCanopy();

        SetMessage(data.version == ForestSaveData.CurrentVersion
            ? "Game loaded"
            : $"Game loaded (save v{data.version})");
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

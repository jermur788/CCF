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

        ForestEcologyController ecology = Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology != null)
        {
            data.ecologicalYear = ecology.EcologicalYear;
            data.simulationSeed = ecology.SimulationSeed;
        }

        ForestTreeMarkingManager marking = Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (marking != null)
            data.markedTreeIds = marking.GetMarkedIds();

        foreach (ForestTree tree in trees)
        {
            if (tree == null) continue;
            data.trees.Add(new TreeSaveData
            {
                treeId = tree.TreeId,
                stage = (int)tree.Stage,
                stageTimer = tree.StageTimer,
                chopProgress = tree.ChopProgress,
                hasSimulation = true,
                ageYears = tree.AgeYears,
                heightMeters = tree.Height,
                diameterCm = tree.Diameter,
                crownRadiusMeters = tree.CrownRadius,
                position = tree.transform.position
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

        if (ecology != null && ecology.Cells != null)
        {
            ForestEcologyCell[] ecologyCells = ecology.Cells;
            for (int i = 0; i < ecologyCells.Length; i++)
            {
                ForestEcologyCell cell = ecologyCells[i];
                if (cell == null) continue;
                // Seed rain is deterministic from trees + seed + year, so it is not saved.
                if (cell.RegenDensity <= 0f && cell.RegenEstablishYear < 0 && cell.RecentOpening <= 0f)
                    continue;
                data.cells.Add(new ForestCellSaveData
                {
                    index = i,
                    regenDensity = cell.RegenDensity,
                    regenHeight = cell.RegenHeight,
                    regenEstablishYear = cell.RegenEstablishYear,
                    recentOpening = cell.RecentOpening
                });
            }
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        SetMessage($"Game saved (v{ForestSaveData.CurrentVersion}): wood {data.wood}, trees {data.trees.Count}, objects {data.buildables.Count}, storages {data.storages.Count}, cells {data.cells.Count}");
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

        // Recruited trees that are not part of this save must go, or the same save
        // would diverge each time it is loaded.
        if (data.trees != null)
        {
            var savedIds = new HashSet<string>();
            foreach (TreeSaveData saved in data.trees)
                savedIds.Add(saved.treeId);
            foreach (ForestTree existing in trees)
            {
                if (existing != null && !string.IsNullOrEmpty(existing.TreeId) && !savedIds.Contains(existing.TreeId))
                    Destroy(existing.gameObject);
            }
        }

        if (data.trees != null)
        {
            ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
            foreach (TreeSaveData saved in data.trees)
            {
                if (treesById.TryGetValue(saved.treeId, out ForestTree tree))
                {
                    tree.RestoreState((ForestTreeStage)saved.stage, saved.stageTimer, saved.chopProgress);
                    if (data.version >= 3 && saved.hasSimulation)
                        tree.SetSimulationState(saved.ageYears, saved.heightMeters, saved.diameterCm, saved.crownRadiusMeters);
                }
                else if (data.version >= 3 && saved.hasSimulation && spawner != null)
                {
                    // A naturally recruited tree that is not in the scene yet.
                    ForestTree recruited = spawner.Spawn(saved.treeId, saved.position, saved.ageYears, saved.diameterCm, saved.heightMeters, saved.crownRadiusMeters);
                    if (recruited == null)
                    {
                        Debug.LogWarning($"Could not spawn recruited tree {saved.treeId} while loading.");
                        continue;
                    }
                    recruited.RestoreState((ForestTreeStage)saved.stage, saved.stageTimer, saved.chopProgress);
                }
            }
        }

        ForestTreeMarkingManager marking = Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (marking != null)
            marking.RestoreMarks(data.markedTreeIds);

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
        {
            if (data.version >= 3)
            {
                ecology.RestoreEcologyState(data.ecologicalYear, data.simulationSeed);
                if (data.cells != null)
                {
                    foreach (ForestCellSaveData saved in data.cells)
                        ecology.RestoreCellState(saved.index, saved.regenDensity, saved.regenHeight, saved.regenEstablishYear, saved.recentOpening);
                }
            }
            ecology.RecomputeCanopy();
            ecology.RecomputeSeedRain();
        }

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

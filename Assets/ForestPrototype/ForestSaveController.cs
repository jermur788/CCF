using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestSaveController : MonoBehaviour
{
    private const string SaveFileName = "forest-save.json";

    private string message = "";
    private float messageTimer;
    private GUIStyle messageStyle;

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
    // The new save is written here first and only swapped in once complete.
    private static string TempSavePath => SavePath + ".tmp";
    // The previous good save, kept by the swap so a bad save is recoverable.
    private static string BackupSavePath => SavePath + ".bak";

    private void Update()
    {
        if (messageTimer > 0f)
            messageTimer -= Time.deltaTime;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        ScenarioOneManager scenario = Object.FindFirstObjectByType<ScenarioOneManager>();
        if (scenario != null && scenario.ReferencePreviewActive)
            return; // previewing the reference must never replace the player's save

        if (keyboard.f5Key.wasPressedThisFrame)
            Save();
        else if (keyboard.f9Key.wasPressedThisFrame)
            Load();
    }

    public void Save()
    {
        ForestSaveData data = CaptureData();
        try
        {
            WriteSaveAtomically(JsonUtility.ToJson(data, true));
        }
        catch (System.Exception error) when (error is IOException || error is System.UnauthorizedAccessException)
        {
            Debug.LogError($"Saving to {SavePath} failed: {error}");
            SetMessage("Save failed: the file could not be written. Your previous save is unchanged.");
            return;
        }
        SetMessage($"Game saved (v{ForestSaveData.CurrentVersion}): wood {data.wood}, trees {data.trees.Count}, objects {data.buildables.Count}, storages {data.storages.Count}, cells {data.cells.Count}");
    }

    // Writing straight over the only save slot means a crash or full disk
    // mid-write leaves a half-written file and no good save. Instead the new
    // save goes to a temporary file, is flushed to disk, and then replaces
    // the old one in a single step; the old save is kept as a .bak copy.
    private static void WriteSaveAtomically(string json)
    {
        string temp = TempSavePath;
        try
        {
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.Write(json);
                writer.Flush();
                stream.Flush(true);
            }

            if (!File.Exists(SavePath))
            {
                File.Move(temp, SavePath);
                return;
            }
            try
            {
                File.Replace(temp, SavePath, BackupSavePath);
            }
            catch (System.PlatformNotSupportedException)
            {
                // Some platforms lack an atomic replace. Keep a backup, then copy.
                File.Copy(SavePath, BackupSavePath, true);
                File.Copy(temp, SavePath, true);
                File.Delete(temp);
            }
        }
        finally
        {
            // Never leave a stray temporary file behind after a failure.
            if (File.Exists(temp))
            {
                try { File.Delete(temp); }
                catch (IOException) { }
            }
        }
    }

    // The same authoritative capture powers local saves and verified reference
    // milestones without modifying the player's on-disk save slot.
    public ForestSaveData CaptureData()
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
            data.rngModelVersion = ecology.RngModelVersion;
        }

        ForestTreeMarkingManager marking = Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (marking != null)
        {
            data.markedTreeIds = marking.GetMarkedIds();
            data.cropTreeIds = marking.GetCropTreeIds();
        }

        ScenarioOneManager scenario = Object.FindFirstObjectByType<ScenarioOneManager>();
        if (scenario != null)
            data.scenarioOne = scenario.CaptureSaveData();

        foreach (ForestTree tree in trees)
        {
            if (tree == null) continue;
            data.trees.Add(new TreeSaveData
            {
                treeId = tree.TreeId,
                speciesId = tree.Species != null ? tree.Species.SpeciesId : "",
                stage = (int)tree.Stage,
                stageTimer = tree.StageTimer,
                chopProgress = tree.ChopProgress,
                hasSimulation = true,
                ageYears = tree.AgeYears,
                equivalentSuppressedYears = tree.EquivalentSuppressedYears,
                heightMeters = tree.SimulationHeightMeters,
                diameterCm = tree.SimulationDiameterCm,
                crownRadiusMeters = tree.SimulationCrownRadiusMeters,
                pruningLifts = tree.PruningLifts,
                crownBaseHeightM = tree.CrownBaseHeightM,
                lastPruningYear = tree.LastPruningYear,
                markType = (int)tree.MarkType,
                biologicallyDead = tree.IsBiologicallyDead,
                mortalityCause = tree.MortalityCause,
                mortalityYear = tree.MortalityYear,
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
                if (!cell.HasRegeneration && cell.RecentOpening <= 0f &&
                    Mathf.Approximately(cell.EstablishmentSuitability, 1f))
                    continue;
                var savedCell = new ForestCellSaveData
                {
                    index = i,
                    recentOpening = cell.RecentOpening,
                    establishmentSuitability = cell.EstablishmentSuitability
                };
                foreach (ForestRegenerationCohort cohort in cell.Regeneration)
                {
                    if (cohort == null || cohort.Density <= 0f || string.IsNullOrEmpty(cohort.SpeciesId))
                        continue;
                    savedCell.cohorts.Add(new ForestRegenerationCohortSaveData
                    {
                        speciesId = cohort.SpeciesId,
                        density = cohort.Density,
                        height = cohort.Height,
                        establishYear = cohort.EstablishYear,
                        origin = (int)cohort.Origin,
                        originYear = cohort.OriginYear
                    });
                }
                data.cells.Add(savedCell);
            }
        }

        return data;
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            SetMessage("No save found (press F5 to save)");
            return;
        }

        ForestSaveData data;
        try
        {
            data = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(SavePath));
        }
        catch (System.Exception error) when (error is System.ArgumentException || error is IOException
            || error is System.UnauthorizedAccessException)
        {
            // ArgumentException is JsonUtility's error for malformed JSON.
            Debug.LogWarning($"Could not read save {SavePath}: {error}");
            SetMessage(File.Exists(BackupSavePath)
                ? $"Save file unreadable. The previous save is kept as {SaveFileName}.bak."
                : "Save file unreadable.");
            return;
        }
        LoadData(data);
    }

    // Returns false, without changing the world, when the data is not a
    // usable save. Every check runs before the first change to the scene.
    public bool LoadData(ForestSaveData data, bool showMessage = true)
    {
        // Saves written before versioning load as version 1. Future migrations belong here.
        if (data != null && data.version <= 0)
            data.version = 1;

        ForestPlayer player = Object.FindFirstObjectByType<ForestPlayer>();
        ForestTree[] trees = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestBuildable[] buildables = Object.FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestEcologyController ecology = Object.FindFirstObjectByType<ForestEcologyController>();

        string problem = ForestSaveValidation.Validate(data, trees.Length,
            ecology != null ? ecology.CellCount : 0);
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (problem == null && data.trees.Count > 0 && spawner == null)
            problem = "the scene has no tree spawner to restore trees with";
        if (problem != null)
        {
            Debug.LogWarning($"Save rejected before loading: {problem}. The current forest was not changed.");
            if (showMessage)
                SetMessage($"Save not loaded: {problem}.");
            return false;
        }

        if (data.version > ForestSaveData.CurrentVersion)
            Debug.LogWarning($"Save version {data.version} is newer than supported version {ForestSaveData.CurrentVersion}; loading best-effort.");

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
                {
                    // Destroy completes at frame end. Hide/remove from active
                    // habitat queries now so a Year-0 reload cannot inherit
                    // litter from doomed Year-100 broadleaf trees.
                    existing.gameObject.SetActive(false);
                    Destroy(existing.gameObject);
                }
            }
        }

        if (data.trees != null)
        {
            foreach (TreeSaveData saved in data.trees)
            {
                bool legacySpecies = data.version < 7 || string.IsNullOrEmpty(saved.speciesId);
                TreeSpeciesDefinition savedSpecies = legacySpecies ? spawner.DefaultSpecies : spawner.ResolveSpecies(saved.speciesId);
                if (!legacySpecies && savedSpecies == null)
                {
                    Debug.LogWarning($"Unknown saved species '{saved.speciesId}' for tree {saved.treeId}; loading it as {spawner.DefaultSpecies?.SpeciesId ?? "default"}.");
                    savedSpecies = spawner.DefaultSpecies;
                }
                if (treesById.TryGetValue(saved.treeId, out ForestTree tree))
                {
                    tree.RestoreMortality(data.version >= 14 && saved.biologicallyDead,
                        saved.mortalityCause, saved.mortalityYear);
                    tree.SetSpecies(savedSpecies);
                    spawner.ApplySpeciesVisuals(tree, savedSpecies);
                    tree.RestoreState((ForestTreeStage)saved.stage, saved.stageTimer, saved.chopProgress);
                    tree.RestoreSuppressionHistory(data.version >= 6 ? saved.equivalentSuppressedYears : 0f);
                    tree.RestorePruningHistory(data.version >= 11 ? saved.pruningLifts : 0,
                        data.version >= 11 ? saved.crownBaseHeightM : 0f,
                        data.version >= 11 ? saved.lastPruningYear : -1);
                    tree.RestoreMark(data.version >= 13 ? (TreeMarkType)saved.markType : TreeMarkType.None);
                    if (data.version >= 3 && saved.hasSimulation)
                    {
                        tree.transform.position = saved.position;
                        tree.SetSimulationState(saved.ageYears, saved.heightMeters, saved.diameterCm, saved.crownRadiusMeters);
                    }
                }
                else if (data.version >= 3 && saved.hasSimulation)
                {
                    // A naturally recruited tree that is not in the scene yet.
                    ForestTree recruited = spawner.Spawn(saved.treeId, savedSpecies, saved.position, saved.ageYears, saved.diameterCm, saved.heightMeters, saved.crownRadiusMeters);
                    if (recruited == null)
                    {
                        Debug.LogWarning($"Could not spawn recruited tree {saved.treeId} while loading.");
                        continue;
                    }
                    recruited.RestoreState((ForestTreeStage)saved.stage, saved.stageTimer, saved.chopProgress);
                    recruited.RestoreMortality(data.version >= 14 && saved.biologicallyDead,
                        saved.mortalityCause, saved.mortalityYear);
                    recruited.RestoreSuppressionHistory(data.version >= 6 ? saved.equivalentSuppressedYears : 0f);
                    recruited.RestorePruningHistory(data.version >= 11 ? saved.pruningLifts : 0,
                        data.version >= 11 ? saved.crownBaseHeightM : 0f,
                        data.version >= 11 ? saved.lastPruningYear : -1);
                    recruited.RestoreMark(data.version >= 13 ? (TreeMarkType)saved.markType : TreeMarkType.None);
                }
            }
        }

        ForestTreeMarkingManager marking = Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        if (marking != null)
        {
            if (data.version >= 13 && data.trees != null)
            {
                // An early v13 draft accidentally put both colours in the Fell
                // ID list and could omit blue IDs before its cache refreshed.
                // The saved per-tree enum is authoritative for all v13 worlds.
                var fellIds = new List<string>();
                var cropIds = new List<string>();
                foreach (TreeSaveData saved in data.trees)
                {
                    if (saved == null || saved.stage == (int)ForestTreeStage.Stump
                        || (data.version >= 14 && saved.biologicallyDead))
                        continue;
                    if (saved.markType == (int)TreeMarkType.Fell) fellIds.Add(saved.treeId);
                    else if (saved.markType == (int)TreeMarkType.CropTree) cropIds.Add(saved.treeId);
                }
                marking.RestoreMarks(fellIds, cropIds);
            }
            else
                marking.RestoreMarks(data.markedTreeIds);
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

        if (ecology != null)
        {
            if (data.version >= 3)
            {
                ecology.RestoreEcologyState(data.ecologicalYear, data.simulationSeed, data.rngModelVersion);
                if (data.cells != null)
                {
                    foreach (ForestCellSaveData saved in data.cells)
                    {
                        if (data.version >= 8)
                            ecology.RestoreCellState(saved.index, saved.cohorts, saved.recentOpening, saved.establishmentSuitability);
                        else
                            ecology.RestoreCellState(saved.index, saved.regenDensity, saved.regenHeight, saved.regenEstablishYear, saved.recentOpening, saved.establishmentSuitability);
                    }
                }
            }
            ecology.RecomputeCanopy();
            ecology.RecomputeSeedRain();
        }

        // Management orders must validate against the restored ecology cells,
        // especially planted or naturally established cohorts.
        ScenarioOneManager scenario = Object.FindFirstObjectByType<ScenarioOneManager>();
        if (scenario != null)
            scenario.RestoreSaveData(data.version >= 10 ? data.scenarioOne : null);

        if (showMessage)
            SetMessage(data.version == ForestSaveData.CurrentVersion
                ? "Game loaded"
                : $"Game loaded (save v{data.version})");
        else
        {
            message = "";
            messageTimer = 0f;
        }
        return true;
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

        float hudScale = ForestHud.Scale;
        if (messageStyle == null)
        {
            messageStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            messageStyle.normal.textColor = new Color(0.7f, 1f, 0.7f);
        }
        messageStyle.fontSize = Mathf.RoundToInt(36f * hudScale);

        float width = Mathf.Min(760f * hudScale, Screen.width - 32f);
        Rect messageRect = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height - 116f * hudScale - 28f * hudScale, width, 88f * hudScale);
        ForestHud.Panel(messageRect);
        GUI.Label(messageRect, message, messageStyle);
    }
}

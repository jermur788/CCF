using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ForestSaveController : MonoBehaviour
{
    private string message = "";
    private float messageTimer;
    private GUIStyle messageStyle;

    private sealed class CapturedSource
    {
        internal readonly string Json;
        internal readonly ForestSaveSessionContext Context;
        internal readonly string PrimaryPath;
        internal CapturedSource(string json, ForestSaveSessionContext context)
        {
            Json = json;
            Context = context;
            PrimaryPath = context.GetSelectedPaths(Application.persistentDataPath).Primary;
        }
    }

    private static readonly ConditionalWeakTable<ForestSaveData, CapturedSource> captures = new ConditionalWeakTable<ForestSaveData, CapturedSource>();

    internal sealed class AdmittedSave
    {
        private readonly string json;
        private readonly ForestSaveSessionContext context;
        private readonly ForestSaveProfiles.Profile profile;
        private readonly string primaryPath;
        private ScenarioOneManager applyingManager;
        private bool applying;

        private AdmittedSave(string raw, ForestSaveSessionContext sourceContext)
        {
            json = raw;
            context = sourceContext;
            profile = context.SelectedProfile;
            primaryPath = context.GetSelectedPaths(Application.persistentDataPath).Primary;
        }

        internal static bool TryCreate(string json, out AdmittedSave admission, out string problem)
        {
            admission = null;
            ForestSaveSessionContext context = ForestSaveSessionContext.Current;
            context.GetSelectedPaths(Application.persistentDataPath);
            problem = ForestSaveValidation.ValidateSelectedJson(json, context.SelectedProfile);
            if (problem != null) return false;
            ForestSaveData data = JsonUtility.FromJson<ForestSaveData>(json);
            problem = ForestSaveValidation.ValidateGeometryJson(json, data)
                ?? ForestSaveValidation.ValidateStormJson(json, data)
                ?? ForestSaveValidation.ValidateCompetitionJson(json, data)
                ?? ForestSaveValidation.ValidateRackV2Json(json, data)
                ?? ForestSaveValidation.ValidateObjectiveHistoryJson(json, data)
                ?? ForestSaveValidation.ValidatePrescriptionsJson(json, data);
            if (problem != null) return false;
            // Second, typed check of the raw historical foreign-ID preflight, in case that streaming
            // reader stopped early on JSON the save parser still accepts. Only a nonempty, explicitly
            // different v10+ scenario ID refuses; absent/null/empty IDs and v1-v9 saves are unchanged.
            if (ReferenceEquals(context.SelectedProfile, ForestSaveProfiles.Historical) && data != null
                && data.version >= 10 && data.scenarioOne != null && !string.IsNullOrEmpty(data.scenarioOne.scenarioId)
                && !string.Equals(data.scenarioOne.scenarioId, context.SelectedProfile.ChapterId, System.StringComparison.Ordinal))
            {
                problem = "the save explicitly belongs to a different scenario";
                return false;
            }
            admission = new AdmittedSave(json, context);
            return true;
        }

        private bool MatchesCurrentContext()
        {
            return ReferenceEquals(context, ForestSaveSessionContext.Current)
                && ReferenceEquals(profile, context.SelectedProfile)
                && ForestSaveProfiles.CanUse(profile, out _)
                && primaryPath == context.GetSelectedPaths(Application.persistentDataPath).Primary;
        }

        internal bool TryPrepare(out ForestSaveData data, out string problem)
        {
            data = null;
            problem = "the admitted save no longer belongs to the selected context";
            if (!MatchesCurrentContext()) return false;
            ScenarioOneManager manager = Object.FindFirstObjectByType<ScenarioOneManager>();
            if (manager != null && (manager.Definition == null || manager.Definition.ScenarioId != profile.ChapterId))
            {
                problem = "the active scenario does not match the selected save profile";
                return false;
            }
            data = JsonUtility.FromJson<ForestSaveData>(json);
            if (data != null && data.version <= 0) data.version = 1;
            problem = FindLoadProblem(data,
                Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length,
                Object.FindFirstObjectByType<ForestEcologyController>(), Object.FindFirstObjectByType<ForestTreeSpawner>(), out _);
            return problem == null;
        }

        internal bool Apply(ForestSaveController controller, bool showMessage, System.Action onAdmitted = null)
            => Apply(controller, showMessage, onAdmitted, out _);

        // refusal is the exact player-readable message for a refusal, so callers can show it unchanged.
        internal bool Apply(ForestSaveController controller, bool showMessage, System.Action onAdmitted, out string refusal)
        {
            refusal = null;
            if (!TryPrepare(out ForestSaveData data, out string problem))
            {
                Debug.LogWarning("Save rejected before loading: " + problem);
                refusal = "Save not loaded: " + problem + ".";
                if (showMessage) controller.SetMessage(refusal);
                return false;
            }
            // The caller may now release paused input/UI. Apply uses this exact admitted snapshot;
            // it never rereads a file or accepts a caller-supplied replacement DTO after this point.
            onAdmitted?.Invoke();
            applyingManager = Object.FindFirstObjectByType<ScenarioOneManager>();
            applying = true;
            try { return controller.ApplyAdmittedData(data, this, showMessage); }
            finally { applying = false; applyingManager = null; }
        }

        internal bool TryGetManagementData(ScenarioOneManager manager, out ScenarioOneSaveData data, out int version)
        {
            data = null;
            version = 0;
            if (!applying || !ReferenceEquals(manager, applyingManager) || !MatchesCurrentContext()) return false;
            ForestSaveData payload = JsonUtility.FromJson<ForestSaveData>(json);
            version = payload.version <= 0 ? 1 : payload.version;
            data = version >= 10 ? payload.scenarioOne : null;
            return true;
        }
    }

    private static string SavePath => ForestSaveSessionContext.CurrentPaths.Primary;
    // The new save is written here first and only swapped in once complete.
    private static string TempSavePath => ForestSaveSessionContext.CurrentPaths.Temporary;
    // The previous good save, kept by the swap so a bad save is recoverable.
    private static string BackupSavePath => ForestSaveSessionContext.CurrentPaths.Backup;

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

    public void Save() => TrySave(null);

    internal bool TrySave(System.Action onAdmitted) => TrySave(onAdmitted, out _);

    // Menu teardown is permitted only after context, pure capture and semantic admission succeed.
    // On failure, refusal is the same message shown on the HUD, for a menu that has this controller disabled.
    internal bool TrySave(System.Action onAdmitted, out string refusal)
    {
        refusal = null;
        if (!CanOperateOnSelectedWorld(out string problem))
        {
            refusal = "Save refused: " + problem;
            SetMessage(refusal);
            return false;
        }
        ForestSaveData data = CaptureData();
        string json = JsonUtility.ToJson(data, true);
        if (!AdmittedSave.TryCreate(json, out AdmittedSave admission, out problem)
            || !admission.TryPrepare(out _, out problem))
        {
            refusal = "Save refused: " + problem;
            SetMessage(refusal);
            return false;
        }
        onAdmitted?.Invoke();
        try
        {
            WriteSaveAtomically(json);
        }
        catch (System.Exception error) when (error is IOException || error is System.UnauthorizedAccessException)
        {
            Debug.LogError($"Saving to {SavePath} failed: {error}");
            refusal = "Save failed: the file could not be written. Your previous save is unchanged.";
            SetMessage(refusal);
            return false;
        }
        SetMessage($"Game saved (v{ForestSaveData.CurrentVersion}): wood {data.wood}, trees {data.trees.Count}, objects {data.buildables.Count}, storages {data.storages.Count}, cells {data.cells.Count}");
        return true;
    }

    private static bool CanOperateOnSelectedWorld(out string problem)
    {
        ForestSaveSessionContext context = ForestSaveSessionContext.Current;
        if (!ForestSaveProfiles.CanUse(context.SelectedProfile, out problem)) return false;
        context.GetSelectedPaths(Application.persistentDataPath);
        ScenarioOneManager manager = Object.FindFirstObjectByType<ScenarioOneManager>();
        if (manager != null && manager.ReferencePreviewActive)
        {
            problem = "disk save/load is unavailable during reference preview";
            return false;
        }
        if (manager != null && (manager.Definition == null || manager.Definition.ScenarioId != context.SelectedProfile.ChapterId))
        {
            problem = "the active scenario does not match the selected save profile";
            return false;
        }
        return true;
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
        ForestSaveSessionContext context = ForestSaveSessionContext.Current;
        context.GetSelectedPaths(Application.persistentDataPath);
        ForestSaveData data = CaptureWorldData();
        captures.Add(data, new CapturedSource(JsonUtility.ToJson(data), context));
        return data;
    }

    private ForestSaveData CaptureWorldData()
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
            data.regenerationModel = ecology.RegenerationModelVersion;
            data.growthModel = ecology.GrowthModelVersion;
            data.stormModel = ecology.StormModelVersion;
            data.standGeometryModel = ecology.StandGeometryModelVersion;
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
        if (!TryLoad(null, out string problem)) SetMessage(problem);
    }

    // Read once at the action, then carry the immutable bytes through admission and caller teardown.
    // CanLoad is only an observation; it never grants permission to load a later file revision.
    internal bool TryLoad(System.Action onAdmitted, out string problem)
    {
        if (!TryPrepareDiskLoad(out AdmittedSave admission, out problem)) return false;
        if (admission.Apply(this, true, onAdmitted, out problem)) return true;
        // Keep Apply's specific reason (for example a scene change since the file was read).
        problem = problem ?? "Save not loaded.";
        return false;
    }

    public bool CanLoad(out string problem) => TryPrepareDiskLoad(out _, out problem);

    private static bool TryPrepareDiskLoad(out AdmittedSave admission, out string problem)
    {
        admission = null;
        if (!CanOperateOnSelectedWorld(out problem)) return false;
        if (!File.Exists(SavePath))
        {
            problem = "No saved forest on this device yet.";
            return false;
        }
        if (!TryReadSave(out admission, out problem)) return false;
        if (!admission.TryPrepare(out _, out string loadProblem))
        {
            problem = $"Save not loaded: {loadProblem}.";
            return false;
        }
        return true;
    }

    // File and JSON checks for the disk-load path. On failure "refusal" is the exact message Load() shows.
    private static bool TryReadSave(out AdmittedSave admission, out string refusal)
    {
        admission = null;
        refusal = null;
        try
        {
            string json = File.ReadAllText(SavePath);
            if (!AdmittedSave.TryCreate(json, out admission, out string problem))
            {
                refusal = "Save refused: " + problem;
                return false;
            }
        }
        catch (System.Exception error) when (error is System.ArgumentException || error is IOException
            || error is System.UnauthorizedAccessException)
        {
            // ArgumentException is JsonUtility's error for malformed JSON.
            Debug.LogWarning($"Could not read save {SavePath}: {error}");
            refusal = File.Exists(BackupSavePath)
                ? $"Save file unreadable. The previous save is kept as {Path.GetFileName(BackupSavePath)}."
                : "Save file unreadable.";
            return false;
        }
        return true;
    }

    internal bool TryPrepareReference(int year, out AdmittedSave admission, out ForestSaveData example)
    {
        admission = null;
        example = null;
        ForestSaveSessionContext.Current.GetSelectedPaths(Application.persistentDataPath);
        if (!ReferenceEquals(ForestSaveSessionContext.Current.SelectedProfile, ForestSaveProfiles.Historical)) return false;
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        ScenarioReferenceMilestone milestone = archive?.AtYear(year);
        ScenarioOneManager manager = Object.FindFirstObjectByType<ScenarioOneManager>();
        if (milestone == null || !milestone.verifiedFrozenWorld || manager == null
            || !archive.IdentityMatches(manager.Definition, Object.FindFirstObjectByType<ForestEcologyController>())) return false;
        TextAsset asset = Resources.Load<TextAsset>(ScenarioReferenceArchive.ResourceName);
        if (asset == null) return false;
        try
        {
            string text;
            using (var source = new MemoryStream(asset.bytes))
            using (var decoder = new GZipStream(source, CompressionMode.Decompress))
            using (var reader = new StreamReader(decoder, Encoding.UTF8))
                text = reader.ReadToEnd();
            JObject root = JObject.Parse(text);
            int milestoneIndex = archive.milestones.IndexOf(milestone);
            if ((int)root["milestones"][milestoneIndex]["year"] != year) return false;
            string raw = ExtractReferenceWorld(text, milestoneIndex);
            if (raw == null || ScenarioReferenceArchive.Hash(raw) != milestone.worldHash) return false;
            return AdmittedSave.TryCreate(raw, out admission, out _) && admission.TryPrepare(out example, out _);
        }
        catch (System.Exception error) when (error is IOException || error is Newtonsoft.Json.JsonException
            || error is System.ArgumentException || error is System.InvalidOperationException)
        {
            Debug.LogWarning("Reference source admission failed: " + error.Message);
            return false;
        }
    }

    private static string ExtractReferenceWorld(string text, int milestoneIndex)
    {
        int offset = 0;
        for (int milestone = 0; milestone <= milestoneIndex; milestone++)
        {
            int key = text.IndexOf("\"world\"", offset, System.StringComparison.Ordinal);
            if (key < 0) return null;
            int opening = text.IndexOf(':', key + 7) + 1;
            while (opening < text.Length && char.IsWhiteSpace(text[opening])) opening++;
            if (opening >= text.Length || text[opening] != '{') return null;
            var raw = new StringBuilder();
            bool quoted = false, escaped = false;
            int depth = 0;
            for (int position = opening; position < text.Length; position++)
            {
                char character = text[position];
                if (!quoted && char.IsWhiteSpace(character)) continue;
                raw.Append(character);
                if (character == '"' && !escaped) quoted = !quoted;
                if (!quoted)
                {
                    if (character == '{') depth++;
                    if (character == '}' && --depth == 0)
                    {
                        if (milestone == milestoneIndex) return raw.ToString();
                        offset = position + 1;
                        break;
                    }
                }
                escaped = quoted && character == '\\' ? !escaped : false;
            }
            if (depth != 0) return null;
        }
        return null;
    }

    // Every check that must pass before the first change to the scene. Returns null when the data can be loaded,
    // otherwise a short player-readable reason. Shared by LoadData and CanLoad.
    private static string FindLoadProblem(ForestSaveData data, int sceneTreeCount, ForestEcologyController ecology,
        ForestTreeSpawner spawner, out int geometry)
    {
        // Cell-indexed records are checked against the grid of the geometry the save was made in, never
        // against whatever grid is live: a Legacy40 save is not validated against an Enlarged80 world.
        geometry = StandGeometryModel.ForSave(data);
        int expectedCells = data != null && StandGeometryModel.IsKnown(geometry) ? StandGeometryModel.CellCount(geometry) : 0;
        string problem = ForestSaveValidation.Validate(data, sceneTreeCount, expectedCells);
        // Model-2 patches advance against the ecological cell at their center.
        // Reject an inapplicable center before loading rather than accepting a
        // state which would fail on the next annual step.
        if (problem == null && data.regenerationModel == RegenerationModel.Competition)
        {
            int saveGeometry = geometry;
            if (ecology == null) problem = "model 2 requires an active ecology grid";
            else if (data.scenarioOne.clearancePatches.Exists(p => StandGeometryModel.CellIndex(saveGeometry, p.center) < 0))
                problem = "model 2 local competitor center is outside the stand grid";
        }
        if (problem == null && data.trees.Count > 0 && spawner == null)
            problem = "the scene has no tree spawner to restore trees with";
        // Unknown saved species are refused here, never loaded as Sitka or silently dropped during the restore.
        if (problem == null && spawner != null)
            problem = ForestSaveValidation.ValidateSpecies(data, id => spawner.ResolveSpecies(id) != null);
        return problem;
    }

    // Returns false, without changing the world, when the data is not a
    // usable save. Every check runs before the first change to the scene.
    public bool LoadData(ForestSaveData data, bool showMessage = true)
    {
        if (!CanOperateOnSelectedWorld(out _)) return false;
        return RestoreCapturedData(data, showMessage);
    }

    // Internal same-session origin for preview return; public disk/typed loads stay blocked in preview.
    internal bool RestoreCapturedData(ForestSaveData data, bool showMessage = false)
    {
        if (!TryPrepareCapture(data, out AdmittedSave admission, out _)) return false;
        return admission.Apply(this, showMessage);
    }

    internal bool CanRestoreCapture(ForestSaveData data)
        => TryPrepareCapture(data, out _, out _);

    private static bool TryPrepareCapture(ForestSaveData data, out AdmittedSave admission, out string problem)
    {
        admission = null;
        problem = "original raw source or unchanged same-session capture required";
        if (data == null || !captures.TryGetValue(data, out CapturedSource source)
            || !ReferenceEquals(source.Context, ForestSaveSessionContext.Current)
            || !ForestSaveProfiles.CanUse(source.Context.SelectedProfile, out _)
            || source.PrimaryPath != source.Context.GetSelectedPaths(Application.persistentDataPath).Primary
            || source.Json != JsonUtility.ToJson(data))
        {
            Debug.LogWarning("Typed save refused: original raw source or unchanged same-session capture required.");
            return false;
        }
        if (!AdmittedSave.TryCreate(source.Json, out admission, out problem))
        {
            Debug.LogWarning("Captured save refused: " + problem);
            return false;
        }
        return admission.TryPrepare(out _, out problem);
    }

    private bool ApplyAdmittedData(ForestSaveData data, AdmittedSave admission, bool showMessage)
    {

        ForestPlayer player = Object.FindFirstObjectByType<ForestPlayer>();
        ForestTree[] trees = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestBuildable[] buildables = Object.FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestWoodStorage[] storages = Object.FindObjectsByType<ForestWoodStorage>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ForestEcologyController ecology = Object.FindFirstObjectByType<ForestEcologyController>();
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();

        int geometry = StandGeometryModel.ForSave(data);

        if (data.version > ForestSaveData.CurrentVersion)
            Debug.LogWarning($"Save version {data.version} is newer than supported version {ForestSaveData.CurrentVersion}; loading best-effort.");

        // First change to the world, after every check: put the ecology grid into the save's geometry so
        // every cell-indexed record below is restored onto the grid it was made in.
        if (ecology != null)
        {
            // An Enlarged80 world saved before the forest track's approach existed was planted across its line and keeps that stand: the
            // saved trees say which kind of world this is (no save field). Reconcile also covers a load that leaves the geometry unchanged.
            ecology.StandApproachExpected = StandApproachPath.SaveHasApproach(geometry, data.trees);
            ecology.ApplyStandGeometry(geometry);
            StandApproachPath.Reconcile(geometry, ecology.StandApproachExpected);
        }

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
                // No automatic migration: saves before v16 are always model 0.
                int regenerationModel = data.version >= 16 ? data.regenerationModel : RegenerationModel.Legacy;
                // No automatic migration: saves before v17 are always growth model 0.
                int growthModel = data.version >= 17 ? data.growthModel : GrowthModel.Legacy;
                ecology.RestoreEcologyState(data.ecologicalYear, data.simulationSeed, data.rngModelVersion, regenerationModel, growthModel,
                    data.version >= 19 ? data.stormModel : StormModel.None);
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
            scenario.RestoreAdmittedSaveData(admission);
        else if (ecology != null)
            ecology.Browsing.ClearProtection();

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

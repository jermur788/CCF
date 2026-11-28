using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;
using SitkaP1.Core.Saves;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // ---- Phase 1 Save v2 & Phase 2A-2 Save v3 (C-V3) -------------------------------------------------------------------

    // Lists every server-managed slot (v2 and v3). Valid self-contained v3 Forest Maker saves (Money On or Off) show
    // status="valid" and canLoad=true even when another stand is currently open (loading swaps the host atomically);
    // v2 saves require an exact identity match with the active built-in Money-Off stand. Built-in Economy sessions
    // surface SaveEconomyRequiresV3 while still listing loadable v3 Forest Maker slots.
    private Task ReadMultiSavesAsync(HttpContext context)
    {
        var slots = new List<MultiSlotInfo>();
        Phase1SaveExpectation? expectation = null;
        bool economyEnabled;
        bool isForestMaker;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            isForestMaker = active.Session.IsForestMaker;
            if (!economyEnabled)
                expectation = active.Session.SaveExpectation();
        }
        foreach (string slot in saveV3Slots.ListSlots())
        {
            var inspection = saveV3Slots.Inspect(slot);
            string status = "corrupt";
            string? refusal = inspection.PrimaryRefusal;
            string? label = null, savedUtc = null, models = null, experimentId = null;
            int? year = null;
            int? schemaVersion = inspection.PrimarySchemaVersion;
            bool? slotEconomyEnabled = null;
            if (!inspection.PrimaryExists)
            {
                status = "missing";
                refusal = "SlotMissing";
            }
            else if (inspection.PrimaryRefusal is null)
            {
                try
                {
                    if (inspection.PrimarySchemaVersion == SaveV3Format.SchemaVersionV3)
                    {
                        var loadedV3 = saveV3Slots.Load(slot, v2Expectation: null);
                        _ = SaveV3Codec.VerifyRecipeGenesis(loadedV3.V3!);
                        var header = loadedV3.Header;
                        status = "valid";
                        refusal = null;
                        label = header.Label;
                        year = header.EcologicalYear;
                        savedUtc = header.SavedUtc;
                        models = header.Models.ToString();
                        experimentId = header.Genesis.ExperimentId;
                        slotEconomyEnabled = loadedV3.V3!.GenesisRecipe?.EconomyEnabled;
                    }
                    else if (expectation is null)
                    {
                        var loadedV2 = saveV3Slots.Load(slot, v2Expectation: null);
                        var header = loadedV2.Header;
                        status = "incompatible";
                        refusal = "SaveEconomyRequiresV3";
                        label = header.Label;
                        year = header.EcologicalYear;
                        savedUtc = header.SavedUtc;
                        models = header.Models.ToString();
                        experimentId = header.Genesis.ExperimentId;
                    }
                    else
                    {
                        var loadedV2 = saveV3Slots.Load(slot, expectation);
                        var header = loadedV2.Header;
                        status = "valid";
                        refusal = null;
                        label = header.Label;
                        year = header.EcologicalYear;
                        savedUtc = header.SavedUtc;
                        models = header.Models.ToString();
                        experimentId = header.Genesis.ExperimentId;
                    }
                }
                catch (P1Refusal failure)
                {
                    status = IsIdentityRefusal(failure.Code) ? "incompatible" : "corrupt";
                    refusal = failure.Code;
                }
            }
            bool primaryVerifies = inspection.PrimaryExists && inspection.PrimaryRefusal is null;
            bool backupLoadable = inspection.BackupExists && inspection.BackupRefusal is null;
            slots.Add(new MultiSlotInfo(slot, label, year, savedUtc, models, status, status == "valid",
                inspection.BackupExists, backupLoadable, !primaryVerifies && backupLoadable, refusal,
                schemaVersion, experimentId, slotEconomyEnabled));
        }
        Notice? topRefusal = economyEnabled && !isForestMaker
            ? new Notice("SaveEconomyRequiresV3", EconomyBuiltInSaveBlockedNote)
            : null;
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiSavesPayload(slots.AsReadOnly(), topRefusal));
    }

    // Saves the current fenced session into a named slot: v2 for MA-1 / MA-1-MIXED, C-V3 for Forest Maker stands.
    private async Task MultiSaveAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSaveRequest>(context, MultiSaveMembers);
        if (request is null) return;
        string slot = request.Slot!;
        string label = string.IsNullOrEmpty(request.Label) ? slot : request.Label;
        try
        {
            MultiSavePayload? payload = null;
            Core.WoodlandImport.Inspection.BuiltInWoodlandEntry? inspectionBlock;
            using (saveV3Slots.LockSlot(slot))
            {
                var (exists, generation) = saveV3Slots.NextGeneration(slot);
                if (exists && !request.ReplaceExisting)
                    throw new P1Refusal("SlotExists",
                        "slot '" + slot + "' already holds a save; replacing it needs explicit confirmation");

                MultiSessionState capturedState;
                (int AnnualRevision, int WorkRevision, int PlanStateRevision) marker = default;
                int schemaVersion = 0;
                byte[] bytes = Array.Empty<byte>();
                Phase1SlotSaveResult? result = null;
                lock (multiHost.SyncLock)
                {
                    // Commit-time survey check (the body is already read). The slot write also stays inside this lock, so a
                    // save can never land on disk after a survey inspection has opened.
                    inspectionBlock = InspectionBlockerLocked();
                    capturedState = multiHost.Current;
                    if (inspectionBlock is null)
                    {
                        (schemaVersion, bytes) = capturedState.Session.CaptureSaveBytes(request.Fence!, label, generation);
                        marker = capturedState.Session.ProgressMarker();
                        result = saveV3Slots.Save(slot, bytes, request.ReplaceExisting);
                    }
                }

                if (result is not null)
                {
                    Interlocked.Increment(ref savesWritten);
                    multiHost.MarkPersisted(capturedState, marker);

                    var decoded = SaveV3Codec.DecodeAny(bytes);
                    var header = decoded.Header;
                    payload = new MultiSavePayload(new MultiSaveOutcome(result.SlotName,
                        result.Status == Phase1SlotCommitStatus.DurablyCommitted ? "durablyCommitted" : "durabilityUnconfirmed",
                        result.BackupUpdated, result.Checksum, result.Bytes, header.SaveGeneration, header.Label, header.EcologicalYear,
                        header.SavedUtc, schemaVersion), null);
                }
            }
            if (inspectionBlock is not null)
            {
                await WriteInspectionCommitRefusalAsync(context, inspectionBlock);
                return;
            }
            await WriteJsonAsync(context, StatusCodes.Status200OK, payload!);
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiSavePayload(null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Loads either a frozen Save v2 slot (into the matching built-in session) or a self-contained C-V3 Save v3 slot
    // (regenerating the stand from its embedded recipe, restoring all state + blocks, and atomically swapping the host).
    private async Task MultiLoadAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiLoadRequest>(context, MultiLoadMembers);
        if (request is null) return;
        var capturedState = multiHost.Current;
        MultiDecodedSave? loaded = null;
        P1Refusal? loadFailure = null;
        try
        {
            loaded = saveV3Slots.Load(request.Slot!, v2Expectation: null);
        }
        catch (P1Refusal refusal)
        {
            loadFailure = refusal;
        }

        MultiAreaLoadResult result;
        MultiAreaCompositeUiSnapshot snapshot;
        int totalRecordedYears;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        bool hasUnsavedProgress;
        string experimentBadge;
        MultiWoodlandRecipeDto? recipeDto;
        int genesisStems;

        lock (multiHost.SyncLock)
        {
            var activeState = multiHost.Current;
            if (!ReferenceEquals(activeState, capturedState))
            {
                result = new MultiAreaLoadResult(activeState.Session.ReadView(), 0, null,
                    "PlayerSessionMismatch", "the active session changed before load could commit; nothing changed");
            }
            else if (loadFailure is not null)
            {
                result = new MultiAreaLoadResult(activeState.Session.ReadView(), 0, null, loadFailure.Code, loadFailure.Message);
            }
            else if (loaded!.SchemaVersion == Phase1SaveFormat.SchemaVersion)
            {
                try
                {
                    result = activeState.Session.Restore(request.Fence!, loaded.V2!);
                    multiHost.MarkPersisted(activeState, activeState.Session.ProgressMarker());
                }
                catch (P1Refusal refusal)
                {
                    result = new MultiAreaLoadResult(activeState.Session.ReadView(), 0, null, refusal.Code, refusal.Message);
                }
            }
            else
            {
                // Self-contained Save v3 load: check fence + required unsaved confirmation (missing or false refuses), build candidate, then swap.
                try
                {
                    activeState.Session.RequireCurrentFence(request.Fence!);
                    if (activeState.HasUnsavedProgress && request.ConfirmDiscardUnsaved != true)
                        throw new P1Refusal("WoodlandUnsavedProgress", "Discard unsaved progress and load this saved woodland?");
                    var (restoredSession, restoredExperiment, loadResult) = MultiAreaContinuation.RestoreFromV3(loaded.V3!);
                    var candidate = MultiSessionHost.WrapRestoredV3(restoredSession, restoredExperiment);
                    multiHost.SwapLocked(candidate);
                    result = loadResult;
                }
                catch (P1Refusal refusal)
                {
                    result = new MultiAreaLoadResult(activeState.Session.ReadView(), 0, null, refusal.Code, refusal.Message);
                }
            }

            // A committed load replaces the playable woodland, so it also ends any survey inspection (same lock, same commit).
            if (result.RefusalCode is null) ClearInspectionLocked();

            var current = multiHost.Current;
            snapshot = current.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = current.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = current.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(current.Session, snapshot.View);
            hasUnsavedProgress = current.HasUnsavedProgress;
            experimentBadge = current.Badge;
            recipeDto = current.Recipe is null ? null : ProjectRecipeDto(current.Recipe);
            genesisStems = current.Session.GenesisStemCount;
        }

        var budget = ActiveStandBudget(snapshot.View.Areas.Count, genesisStems);
        var bounded = SerializeBoundedMultiView(snapshot, totalRecordedYears, reviewPreview: null, out byte[] rawBytes,
            economyEnabled, economy, budget, hasUnsavedProgress, recipeDto);
        var info = result.RefusalCode is null ? new MultiLoadInfo(result.DraftOrderCount, result.Checksum!) : null;
        await WriteBoundedJsonAsync(context, StatusOf(result.RefusalCode), new MultiLoadPayload(bounded.View, bounded.Scenes,
            MultiLabels, bounded.Stage4, bounded.Ecology, info, NoticeOf(result.RefusalCode, result.RefusalMessage),
            bounded.HistoryTruncated, bounded.TotalRecordedYears, bounded.ProjectionCompacted, bounded.Stage4Compacted,
            bounded.PayloadBudgetExceeded, economyEnabled, economy, hasUnsavedProgress, experimentBadge, recipeDto),
            budget, bounded.PayloadBudgetExceeded, rawBytes.Length);
    }

    private async Task MultiRecoverAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSlotRequest>(context, MultiRecoverMembers);
        if (request is null) return;
        try
        {
            Phase1SlotSaveResult? result = null;
            Core.WoodlandImport.Inspection.BuiltInWoodlandEntry? inspectionBlock;
            lock (multiHost.SyncLock)
            {
                // Commit-time survey check (the body is already read); the slot file changes only inside this lock.
                inspectionBlock = InspectionBlockerLocked();
                if (inspectionBlock is null) result = saveV3Slots.RecoverFromBackup(request.Slot!);
            }
            if (inspectionBlock is not null)
            {
                await WriteInspectionCommitRefusalAsync(context, inspectionBlock);
                return;
            }
            await WriteJsonAsync(context, StatusCodes.Status200OK,
                new MultiRecoverPayload(result!.SlotName, result.Checksum, result.Bytes, null));
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiRecoverPayload(request.Slot!, null, null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    private static bool IsIdentityRefusal(string code) =>
        code is "SaveModelMismatch" or "SaveGenesisMismatch" or "SaveLineageMismatch"
            or "SaveEconomyRequiresV3" or "SaveRequiredBlockMissing" or "SaveRecipeUnsupported";

    private sealed record MultiSaveRequest(MultiAreaFence? Fence, string? Slot, string? Label, bool ReplaceExisting);

    private sealed record MultiLoadRequest(MultiAreaFence? Fence, string? Slot, bool? ConfirmDiscardUnsaved = null);

    private sealed record MultiSlotRequest(string? Slot);

    private sealed record MultiSlotInfo(string Slot, string? Label, int? EcologicalYear, string? SavedUtc, string? Models,
        string Status, bool CanLoad, bool BackupExists, bool BackupLoadable, bool Recoverable, string? RefusalCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? SchemaVersion = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ExperimentId = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] bool? EconomyEnabled = null);

    private sealed record MultiSavesPayload(IReadOnlyList<MultiSlotInfo> Slots, Notice? Refusal);

    private sealed record MultiSaveOutcome(string Slot, string Status, bool BackupUpdated, string Checksum, long Bytes,
        long SaveGeneration, string Label, int EcologicalYear, string SavedUtc, int SchemaVersion = 2);

    private sealed record MultiSavePayload(MultiSaveOutcome? Saved, Notice? Refusal);

    private sealed record MultiLoadInfo(int DraftOrderCount, string Checksum);

    private sealed record MultiLoadPayload(object View, object Scenes,
        IReadOnlyDictionary<string, string> Labels, object? Stage4, MultiAreaEcologyDto? Ecology, MultiLoadInfo? Restored,
        Notice? Refusal, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null,
        bool HasUnsavedProgress = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ExperimentBadge = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiWoodlandRecipeDto? Recipe = null);

    private sealed record MultiRecoverPayload(string Slot, string? Checksum, long? Bytes, Notice? Refusal);
}

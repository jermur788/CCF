using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // ---- Phase 1 Save/Load ----------------------------------------------------------------------------------------------

    // The slot list. Every slot is read and fully verified here, and the page is told a state word, never a filesystem
    // path: "valid" (loadable into this exact session), "incompatible" (another forest, model or engine build), "corrupt"
    // (failed its own verification) or "missing" (listed but no longer present). A valid backup is reported separately, so
    // a corrupt primary with a good backup is never shown as a silent success or an empty slot.
    private Task ReadMultiSavesAsync(HttpContext context)
    {
        var slots = new List<MultiSlotInfo>();
        Phase1SaveExpectation expectation;
        try
        {
            expectation = CurrentMulti.Session.SaveExpectation();
        }
        catch (P1Refusal refusal)
        {
            // Economy-enabled sessions refuse Save v2 with SaveEconomyRequiresV3; surface cleanly without 500/stderr.
            return WriteJsonAsync(context, StatusCodes.Status200OK,
                new MultiSavesPayload(Array.Empty<MultiSlotInfo>(), new Notice(refusal.Code, refusal.Message)));
        }
        foreach (string slot in saveSlots.ListSlots())
        {
            var inspection = saveSlots.Inspect(slot);
            string status = "corrupt";
            string? refusal = inspection.PrimaryRefusal;
            string? label = null, savedUtc = null, models = null;
            int? year = null;
            if (!inspection.PrimaryExists)
            {
                status = "missing";
                refusal = "SlotMissing";
            }
            else if (inspection.PrimaryRefusal is null)
            {
                try
                {
                    var header = saveSlots.Load(slot, expectation).Envelope.Header;
                    status = "valid";
                    refusal = null;
                    label = header.Label;
                    year = header.EcologicalYear;
                    savedUtc = header.SavedUtc;
                    models = header.Models.ToString();
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
                inspection.BackupExists, backupLoadable, !primaryVerifies && backupLoadable, refusal));
        }
        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiSavesPayload(slots.AsReadOnly(), null));
    }

    // Save the current fenced forest into a named slot. The engine captures the whole root under its own lock; the strict
    // codec verifies the bytes; the slot store writes outside the lock with its atomic temp/backup/rename sequence. An
    // absent slot is always published no-clobber, and a slot that already holds a save is replaced only when the player
    // confirmed it (replaceExisting). A failed or indeterminate write is never reported as a save.
    private async Task MultiSaveAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSaveRequest>(context, MultiSaveMembers);
        if (request is null) return;
        string slot = request.Slot!;
        string label = string.IsNullOrEmpty(request.Label) ? slot : request.Label;
        try
        {
            MultiSavePayload payload;
            // The slot gate covers the generation read, the engine capture and the write so two saves of one slot
            // can neither interleave nor record the same generation, and is released before the response is awaited
            // (LockSlot holds a thread-affine Monitor, which must never span an await).
            using (saveSlots.LockSlot(slot))
            {
                var (exists, generation) = saveSlots.NextGeneration(slot);
                if (exists && !request.ReplaceExisting)
                    throw new P1Refusal("SlotExists",
                        "slot '" + slot + "' already holds a save; replacing it needs explicit confirmation");
                // Capture under multiSwapSync (so session swap cannot race with CaptureSave), then encode and write outside it.
                Phase1SaveEnvelope envelope;
                lock (multiSwapSync)
                {
                    var pair = CurrentMulti;
                    envelope = pair.Session.CaptureSave(request.Fence!, label, generation);
                }
                byte[] bytes = Phase1SaveCodec.Encode(envelope);
                var result = saveSlots.Save(slot, bytes, request.ReplaceExisting);
                Interlocked.Increment(ref savesWritten);
                var header = envelope.Header;
                payload = new MultiSavePayload(new MultiSaveOutcome(result.SlotName,
                    result.Status == Phase1SlotCommitStatus.DurablyCommitted ? "durablyCommitted" : "durabilityUnconfirmed",
                    result.BackupUpdated, result.Checksum, result.Bytes, header.SaveGeneration, header.Label, header.EcologicalYear,
                    header.SavedUtc), null);
            }
            await WriteJsonAsync(context, StatusCodes.Status200OK, payload);
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiSavePayload(null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // Load a slot: the store verifies the file and the exact identity, then the engine rebuilds, revalidates and publishes
    // the whole candidate root under one lock with a NEW session id. Any failure (stale fence, wrong forest, corrupt file,
    // illegal restored order) answers 409 with the code and the UNCHANGED current view, so the page never loses authority
    // over a session that was not replaced.
    private async Task MultiLoadAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiLoadRequest>(context, MultiLoadMembers);
        if (request is null) return;
        var capturedPair = CurrentMulti;
        Phase1DecodedSave? loaded = null;
        P1Refusal? loadFailure = null;
        try
        {
            loaded = saveSlots.Load(request.Slot!, capturedPair.Session.SaveExpectation());
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
        lock (multiSwapSync)
        {
            var activePair = CurrentMulti;
            if (!ReferenceEquals(activePair, capturedPair))
            {
                result = new MultiAreaLoadResult(activePair.Session.ReadView(), 0, null,
                    "PlayerSessionMismatch", "the active session changed before load could commit; nothing changed");
            }
            else if (loadFailure is not null)
            {
                result = new MultiAreaLoadResult(activePair.Session.ReadView(), 0, null, loadFailure.Code, loadFailure.Message);
            }
            else
            {
                try
                {
                    result = activePair.Session.Restore(request.Fence!, loaded!);
                }
                catch (P1Refusal refusal)
                {
                    result = new MultiAreaLoadResult(activePair.Session.ReadView(), 0, null, refusal.Code, refusal.Message);
                }
            }
            snapshot = activePair.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = activePair.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            economyEnabled = activePair.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(activePair.Session, snapshot.View);
        }
        var bounded = BoundSnapshotForResponse(snapshot, totalRecordedYears, economyEnabled, economy);
        var info = result.RefusalCode is null ? new MultiLoadInfo(result.DraftOrderCount, result.Checksum!) : null;
        await WriteJsonAsync(context, StatusOf(result.RefusalCode), new MultiLoadPayload(bounded.View, bounded.Scenes,
            MultiLabels, bounded.Stage4, bounded.Ecology, info, NoticeOf(result.RefusalCode, result.RefusalMessage),
            bounded.HistoryTruncated, bounded.TotalRecordedYears, bounded.ProjectionCompacted, bounded.Stage4Compacted,
            bounded.PayloadBudgetExceeded, economyEnabled, economy));
    }

    // Restore a slot's verified prior-good backup over its primary. This is a deliberate, separate player action and the
    // store refuses it while the primary still verifies (SlotPrimaryValid), so it can never roll a good newer save back.
    private async Task MultiRecoverAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiSlotRequest>(context, MultiRecoverMembers);
        if (request is null) return;
        try
        {
            var result = saveSlots.RecoverFromBackup(request.Slot!);
            await WriteJsonAsync(context, StatusCodes.Status200OK,
                new MultiRecoverPayload(result.SlotName, result.Checksum, result.Bytes, null));
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiRecoverPayload(request.Slot!, null, null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // A save made for another experiment, model selection or engine build is compatible-looking but not loadable here.
    private static bool IsIdentityRefusal(string code) =>
        code is "SaveModelMismatch" or "SaveGenesisMismatch" or "SaveLineageMismatch";

    // Phase 1 Save/Load request and response bodies. `replaceExisting` is required on every save so the page must state
    // whether the player confirmed replacing an existing slot.
    private sealed record MultiSaveRequest(MultiAreaFence? Fence, string? Slot, string? Label, bool ReplaceExisting);

    private sealed record MultiLoadRequest(MultiAreaFence? Fence, string? Slot);

    private sealed record MultiSlotRequest(string? Slot);

    private sealed record MultiSlotInfo(string Slot, string? Label, int? EcologicalYear, string? SavedUtc, string? Models,
        string Status, bool CanLoad, bool BackupExists, bool BackupLoadable, bool Recoverable, string? RefusalCode);

    private sealed record MultiSavesPayload(IReadOnlyList<MultiSlotInfo> Slots, Notice? Refusal);

    private sealed record MultiSaveOutcome(string Slot, string Status, bool BackupUpdated, string Checksum, long Bytes,
        long SaveGeneration, string Label, int EcologicalYear, string SavedUtc);

    private sealed record MultiSavePayload(MultiSaveOutcome? Saved, Notice? Refusal);

    private sealed record MultiLoadInfo(int DraftOrderCount, string Checksum);

    private sealed record MultiLoadPayload(object View, object Scenes,
        IReadOnlyDictionary<string, string> Labels, object? Stage4, MultiAreaEcologyDto? Ecology, MultiLoadInfo? Restored,
        Notice? Refusal, bool HistoryTruncated = false, int TotalRecordedYears = 0,
        bool ProjectionCompacted = false, bool Stage4Compacted = false, bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null);

    private sealed record MultiRecoverPayload(string Slot, string? Checksum, long? Bytes, Notice? Refusal);
}

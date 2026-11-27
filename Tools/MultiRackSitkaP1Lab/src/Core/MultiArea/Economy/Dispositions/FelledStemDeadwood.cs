namespace SitkaP1.Core;

// LeaveAsDeadwood (ENGINE HALF): the Economy settlement emits intents only. Appending them to MultiAreaStage4State.Deadwood
// is the follow-up wiring step inside Execute (outside this lane's fence); FelledStemDeadwood.Materialise does all the work.

/// <summary>
/// One felled stem that the plan leaves on site. Carries everything needed to build exactly one
/// <see cref="FallenDeadwoodRecord"/>; <see cref="VolumeM3"/> is <see cref="FallenDeadwoodRecord.StemVolumeM3"/> (float32),
/// the same form natural deaths use. Emitted only for <see cref="FellingDisposition.LeaveAsDeadwood"/> stems.
/// </summary>
internal sealed record FelledStemDeadwoodIntent(
    string AreaId,
    string TreeId,
    string SpeciesId,
    float XM,
    float ZM,
    float DbhCm,
    float HeightM,
    float VolumeM3,
    int EcologicalYear,
    int WorkRevision,
    string ExecutionKey,
    string PlanId);

internal static class FelledStemDeadwood
{
    /// <summary>The deadwood cause string for a felled stem left on site (the Fell death cause, never a natural-death cause).</summary>
    internal const string Cause = FellCause.Harvest;

    /// <summary>Builds the intent for one felled stem at the tree's current size.</summary>
    internal static FelledStemDeadwoodIntent IntentFor(string areaId, AnnualTree tree, string speciesId,
        int ecologicalYear, int workRevision, string executionKey, string planId)
    {
        ArgumentNullException.ThrowIfNull(tree);
        return new FelledStemDeadwoodIntent(areaId, tree.TreeId, speciesId, tree.XM, tree.ZM, tree.DbhCm, tree.HeightM,
            FallenDeadwoodRecord.StemVolumeM3(tree.DbhCm, tree.HeightM), ecologicalYear, workRevision, executionKey, planId);
    }

    /// <summary>
    /// Pure: turns intents (in the given order) into fallen-deadwood records numbered DW#### from
    /// <paramref name="nextDeadwoodNumber"/>, and returns the next free number. Rejects a repeated tree so a felled stem
    /// can never become two deadwood records.
    /// </summary>
    internal static (IReadOnlyList<FallenDeadwoodRecord> Records, int NextDeadwoodNumber) Materialise(
        IReadOnlyList<FelledStemDeadwoodIntent> intents, int nextDeadwoodNumber)
    {
        ArgumentNullException.ThrowIfNull(intents);
        if (nextDeadwoodNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(nextDeadwoodNumber), "deadwood numbering starts at 1");
        var seen = new HashSet<(string, string)>();
        var records = new FallenDeadwoodRecord[intents.Count];
        int next = nextDeadwoodNumber;
        for (int i = 0; i < intents.Count; i++)
        {
            var intent = intents[i];
            if (!seen.Add((intent.AreaId, intent.TreeId)))
                throw new InvalidOperationException("A felled stem cannot produce more than one deadwood record.");
            records[i] = new FallenDeadwoodRecord(FallenDeadwoodRecord.IdFor(next), intent.TreeId, intent.AreaId,
                intent.SpeciesId, intent.XM, intent.ZM, intent.EcologicalYear, Cause, intent.DbhCm, intent.HeightM, intent.VolumeM3);
            next = checked(next + 1);
        }
        return (Array.AsReadOnly(records), next);
    }
}

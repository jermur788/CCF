using SitkaP1.Core;

namespace SitkaP1.Ui;

/// <summary>
/// One immutable active <c>/multi</c> session state held by <see cref="MultiSessionHost"/>.
/// Published atomically behind a single reference swap so no reader can observe a half-replaced stand.
/// </summary>
internal sealed record MultiSessionState(
    string ExperimentKey,
    ForestMakerRecipe? Recipe,
    MultiAreaContinuation Session,
    MultiAreaWorkPlanAdapter Adapter,
    string Badge,
    (int AnnualRevision, int WorkRevision, int PlanStateRevision) PersistedMarker)
{
    internal bool HasUnsavedProgress => Session.ProgressMarker() != PersistedMarker;
}

/// <summary>
/// Single atomic owner of the active <c>/multi</c> woodland session (spec §7.2 / T-2A2-04).
/// Builds every candidate completely before swapping; any fence mismatch, unconfirmed unsaved-progress check,
/// or Core refusal leaves the existing session completely untouched.
/// </summary>
internal sealed class MultiSessionHost
{
    private readonly object sync = new();
    private MultiSessionState current;

    internal MultiSessionHost(MultiAreaWorkPlanAdapter initialAdapter, MultiAreaContinuation initialSession, ForestMakerRecipe? initialRecipe = null)
    {
        ArgumentNullException.ThrowIfNull(initialAdapter);
        ArgumentNullException.ThrowIfNull(initialSession);
        current = new MultiSessionState(
            initialAdapter.ExperimentKey,
            initialRecipe,
            initialSession,
            initialAdapter,
            initialAdapter.ExperimentBadge,
            initialSession.ProgressMarker());
    }

    internal object SyncLock => sync;

    internal MultiSessionState Current => Volatile.Read(ref current);

    internal bool HasUnsavedProgress()
    {
        lock (sync) return current.HasUnsavedProgress;
    }

    /// <summary>Records the committed progress marker as persisted (called after a durable Save or v2 Load).</summary>
    internal void MarkPersisted(MultiSessionState expectedState, (int AnnualRevision, int WorkRevision, int PlanStateRevision) marker)
    {
        lock (sync)
        {
            if (ReferenceEquals(current, expectedState))
                Volatile.Write(ref current, current with { PersistedMarker = marker });
        }
    }

    /// <summary>
    /// Atomically replaces the active session under <see cref="SyncLock"/>:
    /// checks the caller's fence first, enforces unsaved-progress confirmation, builds the complete candidate
    /// before swapping, and swaps the single reference only when every step succeeds.
    /// </summary>
    internal MultiSessionState AtomicReplace(
        MultiAreaFence fence,
        bool confirmDiscardUnsaved,
        Func<MultiSessionState, MultiSessionState> buildCandidate)
    {
        ArgumentNullException.ThrowIfNull(buildCandidate);
        lock (sync)
        {
            current.Session.RequireCurrentFence(fence);
            if (current.HasUnsavedProgress && !confirmDiscardUnsaved)
                throw new P1Refusal("WoodlandUnsavedProgress", "Discard unsaved progress and start a new woodland?");
            var candidate = buildCandidate(current);
            Volatile.Write(ref current, candidate);
            return candidate;
        }
    }

    /// <summary>Swaps directly to an already-built candidate (caller holds <see cref="SyncLock"/>).</summary>
    internal MultiSessionState SwapLocked(MultiSessionState candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        Volatile.Write(ref current, candidate);
        return candidate;
    }

    internal static MultiSessionState BuildBuiltIn(string experimentKey, bool economyEnabled)
    {
        var selection = economyEnabled ? MultiAreaModelSelection.Phase2WithEconomy : MultiAreaModelSelection.Phase2;
        var economyOptions = economyEnabled ? EconomyOptions.Default : null;
        if (string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1Key, StringComparison.Ordinal))
        {
            var experiment = MultiAreaGenerator.Generate();
            var session = new MultiAreaContinuation(experiment, selection, null, null, null, economyOptions);
            var adapter = new MultiAreaWorkPlanAdapter(session, experiment, MultiAreaDemoExperiments.Ma1Key, MultiAreaDemoExperiments.Ma1Badge);
            return new MultiSessionState(MultiAreaDemoExperiments.Ma1Key, null, session, adapter, MultiAreaDemoExperiments.Ma1Badge, session.ProgressMarker());
        }
        if (string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1MixedKey, StringComparison.Ordinal))
        {
            var (experiment, phase2Options, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(experiment, selection, null, null, phase2Options, economyOptions);
            var adapter = new MultiAreaWorkPlanAdapter(session, experiment, MultiAreaDemoExperiments.Ma1MixedKey, badge);
            return new MultiSessionState(MultiAreaDemoExperiments.Ma1MixedKey, null, session, adapter, badge, session.ProgressMarker());
        }
        throw new P1Refusal("MultiAreaUnknownExperiment", "supported built-in stands are ma-1 and ma-1-mixed");
    }

    /// <remarks><paramref name="siteMoisture"/> null or NeutralLegacy keeps every existing woodland exactly as before.</remarks>
    internal static MultiSessionState BuildForestMaker(ForestMakerRecipe recipe, bool economyEnabled, SiteMoistureSetting? siteMoisture = null)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        var selection = economyEnabled ? MultiAreaModelSelection.Phase2WithEconomy : MultiAreaModelSelection.Phase2;
        var economyOptions = economyEnabled ? EconomyOptions.Default : null;
        var (generated, session) = ForestMakerGenerator.CreateSession(recipe, selection, economy: economyOptions,
            siteMoisture: siteMoisture);
        var adapter = new MultiAreaWorkPlanAdapter(session, generated.Experiment, generated.Experiment.ExperimentId, session.ExperimentBadge);
        return new MultiSessionState(generated.Experiment.ExperimentId, generated.Recipe, session, adapter, session.ExperimentBadge, session.ProgressMarker());
    }

    internal static MultiSessionState BuildFreshFromActive(MultiSessionState basis, bool economyEnabled) =>
        basis.Recipe is not null
            ? BuildForestMaker(basis.Recipe, economyEnabled, basis.Session.ReadSiteMoisture().Setting)
            : BuildBuiltIn(basis.ExperimentKey, economyEnabled);

    internal static MultiSessionState WrapRestoredV3(MultiAreaContinuation session, MultiAreaExperiment experiment)
    {
        var adapter = new MultiAreaWorkPlanAdapter(session, experiment, experiment.ExperimentId, session.ExperimentBadge);
        return new MultiSessionState(experiment.ExperimentId, session.GenesisRecipe?.Recipe, session, adapter, session.ExperimentBadge, session.ProgressMarker());
    }
}

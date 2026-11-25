namespace SitkaP1.Core;

// CCF Multi 2D — Phase 1 Step 1 (spec multi-2d-phase1-time-history-core-v1 §4):
// Dual time pacing (PreviewNextReviewYear + AdvanceYears). Bounded multi-year stepping calls the existing authoritative
// single-year MultiAreaContinuation.Advance under the continuation's single lock without reimplementing annual biology,
// mutating on refusal, auto-approving/cancelling open work, or fabricating remainder records.

/// <summary>
/// Bounded stop reasons for 2D dual time pacing (exact six values; zero unmodelled 3D stop reasons).
/// </summary>
internal enum TimePacingStopReason
{
    /// <summary>All requested annual steps committed cleanly and DestinationYear equals RequestedTargetYear.</summary>
    ReachedReviewYear,
    /// <summary>Unexecuted work exists in the Work Plan (Pending or Approved of any order type). Zero mutation.</summary>
    WorkPlanOpen,
    /// <summary>Requested target year is not in [CurrentYear + 1, CurrentYear + MaxTicksPerPress]. Zero mutation.</summary>
    InvalidTargetYear,
    /// <summary>Every area has no living stems and no juvenile regeneration left to advance.</summary>
    StandCannotAdvance,
    /// <summary>CancellationToken was observed between completed annual steps; no partial year was committed.</summary>
    CancelledBetweenYears,
    /// <summary>The authoritative single-year Advance refused for another typed reason; pre-step root stayed intact.</summary>
    AnnualStepRefused,
}

/// <summary>Fixed constants and honesty labels for Phase 1 Step 1 dual time pacing (owner Option A, locked).</summary>
internal static class MultiAreaTimePacingConstants
{
    /// <summary>GAMEPLAY CALIBRATION (D-077): suggested review cadence gap in ecological years.</summary>
    internal const int SuggestedGapYears = 4;

    /// <summary>GAMEPLAY CALIBRATION (D-077): hard upper bound on annual ticks per AdvanceYears press.</summary>
    internal const int MaxTicksPerPress = 5;

    internal const string CalibrationNote =
        "GAMEPLAY CALIBRATION — 4-year suggested review gap and 5-year per-press cap are gameplay calibration for " +
        "pacing observation intervals, not universal Irish forestry advice or a compulsory thinning interval.";

    internal const string ObservationPeriodLabel =
        "Quiet observation period (pre-first-Fell) — advances up to 4 ecological years for stand monitoring; " +
        "not a completed silvicultural thinning cycle.";

    internal const string ReviewPeriodLabel =
        "Suggested management review period — 4-year GAMEPLAY CALIBRATION reassessment cadence anchored to last " +
        "recorded Fell year (navigation only; not a universal Irish forestry prescription).";

    internal const string OverdueReviewPeriodLabel =
        "Suggested management review period (anchor overdue) — 4-year GAMEPLAY CALIBRATION forward reassessment " +
        "window from current year (navigation only; not a universal Irish forestry prescription).";

    internal const string WorkPlanOpenLabel =
        "Work plan open — execute, cancel, or remove all pending or approved work before skipping ahead.";

    internal const string StandCannotAdvanceLabel =
        "Stand cannot advance — every area has no living stems and no juvenile regeneration remaining.";
}

/// <summary>Deterministic read-only preview of the next suggested management review year.</summary>
internal sealed record ReviewYearPreview(
    int CurrentYear,
    bool Eligible,
    TimePacingStopReason? StopReason,
    int? AnchorFellYear,
    bool IsObservationPeriod,
    int SuggestedTargetYear,
    int SuggestedGapYears,
    int MaxTicksPerPress,
    string CalibrationNote,
    string Label);

/// <summary>One genuinely committed annual step inside an <see cref="AdvanceYearsResult"/>.</summary>
internal sealed record AdvanceYearStepSummary(
    int YearBefore,
    int YearAfter,
    MultiAreaYearRecord YearRecord,
    MultiAreaAnnualResult AnnualResult);

/// <summary>Outcome of a bounded multi-year advance, returning every committed year and the fresh fence.</summary>
internal sealed record AdvanceYearsResult(
    int StartYear,
    int DestinationYear,
    int RequestedTargetYear,
    IReadOnlyList<AdvanceYearStepSummary> CompletedYears,
    bool ReachedTarget,
    TimePacingStopReason StopReason,
    MultiAreaFence FreshFence,
    bool IsObservationPeriod,
    int? AnchorFellYear,
    string CalibrationNote,
    string? StopDetail = null);

/// <summary>Pure deterministic cadence helpers over one immutable <see cref="MultiAreaRoot"/>.</summary>
internal static class MultiAreaTimePacing
{
    /// <summary>
    /// Latest <see cref="MultiAreaFellRecord.EcologicalYear"/> across all committed <c>root.Events[].Areas[].Fells</c>,
    /// or null before the first recorded Fell. Non-Fell work events never set a Fell anchor.
    /// </summary>
    internal static int? LatestRecordedFellYear(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        int? latest = null;
        foreach (var workEvent in root.Events)
        {
            foreach (var area in workEvent.Areas)
            {
                foreach (var fell in area.Fells)
                {
                    if (latest is null || fell.EcologicalYear > latest.Value)
                        latest = fell.EcologicalYear;
                }
            }
        }
        return latest;
    }

    /// <summary>True whenever any Pending or Approved order of any family exists on the committed root.</summary>
    internal static bool HasOpenWork(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        return root.HasUnexecutedWork
            || (root.Stage4?.ApprovedPrunes.Count ?? 0) > 0
            || (root.Stage5?.ApprovedClears.Count ?? 0) > 0
            || (root.Phase2?.ApprovedOrders.Count ?? 0) > 0;
    }

    /// <summary>Matches <see cref="MultiAreaContinuation.Advance"/>'s exact stand-advanceability condition.</summary>
    internal static bool CanAdvanceStand(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        return root.Areas.Any(area => area.State.Trees.Any(tree => tree.IsLiving))
            || (root.Stage5?.HasAbundance ?? false)
            || (root.Phase2?.HasJuveniles ?? false);
    }

    /// <summary>Computes the locked Option A deterministic preview over one committed root without mutation.</summary>
    internal static ReviewYearPreview Preview(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        int currentYear = root.EcologicalYear;
        int? anchorFellYear = LatestRecordedFellYear(root);
        bool isObservationPeriod = anchorFellYear is null;
        int maxTarget = checked(currentYear + MultiAreaTimePacingConstants.MaxTicksPerPress);

        int suggestedTargetYear;
        bool overdue = false;
        if (anchorFellYear is int anchor)
        {
            int candidate = checked(anchor + MultiAreaTimePacingConstants.SuggestedGapYears);
            if (candidate > currentYear)
            {
                suggestedTargetYear = Math.Min(candidate, maxTarget);
            }
            else
            {
                overdue = true;
                suggestedTargetYear = Math.Min(checked(currentYear + MultiAreaTimePacingConstants.SuggestedGapYears), maxTarget);
            }
        }
        else
        {
            suggestedTargetYear = Math.Min(checked(currentYear + MultiAreaTimePacingConstants.SuggestedGapYears), maxTarget);
        }

        if (HasOpenWork(root))
        {
            return new ReviewYearPreview(
                currentYear,
                Eligible: false,
                StopReason: TimePacingStopReason.WorkPlanOpen,
                AnchorFellYear: anchorFellYear,
                IsObservationPeriod: isObservationPeriod,
                SuggestedTargetYear: suggestedTargetYear,
                SuggestedGapYears: MultiAreaTimePacingConstants.SuggestedGapYears,
                MaxTicksPerPress: MultiAreaTimePacingConstants.MaxTicksPerPress,
                CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                Label: MultiAreaTimePacingConstants.WorkPlanOpenLabel);
        }

        if (!CanAdvanceStand(root))
        {
            return new ReviewYearPreview(
                currentYear,
                Eligible: false,
                StopReason: TimePacingStopReason.StandCannotAdvance,
                AnchorFellYear: anchorFellYear,
                IsObservationPeriod: isObservationPeriod,
                SuggestedTargetYear: suggestedTargetYear,
                SuggestedGapYears: MultiAreaTimePacingConstants.SuggestedGapYears,
                MaxTicksPerPress: MultiAreaTimePacingConstants.MaxTicksPerPress,
                CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                Label: MultiAreaTimePacingConstants.StandCannotAdvanceLabel);
        }

        string label = isObservationPeriod
            ? MultiAreaTimePacingConstants.ObservationPeriodLabel
            : overdue
                ? MultiAreaTimePacingConstants.OverdueReviewPeriodLabel
                : MultiAreaTimePacingConstants.ReviewPeriodLabel;

        return new ReviewYearPreview(
            currentYear,
            Eligible: true,
            StopReason: null,
            AnchorFellYear: anchorFellYear,
            IsObservationPeriod: isObservationPeriod,
            SuggestedTargetYear: suggestedTargetYear,
            SuggestedGapYears: MultiAreaTimePacingConstants.SuggestedGapYears,
            MaxTicksPerPress: MultiAreaTimePacingConstants.MaxTicksPerPress,
            CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
            Label: label);
    }
}

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Preview the next suggested management review year under the single root lock. Validates the caller's fence
    /// via <see cref="RequireFence"/> (throwing the existing stale-fence <see cref="P1Refusal"/> on mismatch) and
    /// never mutates state.
    /// </summary>
    internal ReviewYearPreview PreviewNextReviewYear(MultiAreaFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            return MultiAreaTimePacing.Preview(root);
        }
    }

    /// <summary>
    /// Advance strictly year by year toward <paramref name="requestedTargetYear"/> (1..5 years per press) under the
    /// single root lock, delegating every step to the authoritative single-year <see cref="Advance"/> with
    /// <c>consentToCancelWork: false</c>. Validates the initial fence via <see cref="RequireFence"/>; never mutates on
    /// refusal; checks <paramref name="cancellationToken"/> strictly between annual steps.
    /// </summary>
    internal AdvanceYearsResult AdvanceYears(MultiAreaFence fence, int requestedTargetYear,
        CancellationToken cancellationToken = default) =>
        AdvanceYearsCore(fence, requestedTargetYear, cancellationToken, afterYearCommitted: null);

    // Internal test-only seam invoked strictly AFTER one annual step commits and BEFORE the between-step cancellation check.
    internal AdvanceYearsResult AdvanceYears(MultiAreaFence fence, int requestedTargetYear,
        CancellationToken cancellationToken, Action<int>? afterYearCommitted) =>
        AdvanceYearsCore(fence, requestedTargetYear, cancellationToken, afterYearCommitted);

    private AdvanceYearsResult AdvanceYearsCore(MultiAreaFence fence, int requestedTargetYear,
        CancellationToken cancellationToken, Action<int>? afterYearCommitted)
    {
        lock (sync)
        {
            RequireFence(fence);
            int startYear = root.EcologicalYear;
            int? anchorFellYear = MultiAreaTimePacing.LatestRecordedFellYear(root);
            bool isObservationPeriod = anchorFellYear is null;

            if (MultiAreaTimePacing.HasOpenWork(root))
            {
                return new AdvanceYearsResult(
                    StartYear: startYear,
                    DestinationYear: startYear,
                    RequestedTargetYear: requestedTargetYear,
                    CompletedYears: Array.Empty<AdvanceYearStepSummary>(),
                    ReachedTarget: false,
                    StopReason: TimePacingStopReason.WorkPlanOpen,
                    FreshFence: CurrentFenceLocked(),
                    IsObservationPeriod: isObservationPeriod,
                    AnchorFellYear: anchorFellYear,
                    CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                    StopDetail: MultiAreaTimePacingConstants.WorkPlanOpenLabel);
            }

            long requestedDelta = (long)requestedTargetYear - startYear;
            if (requestedDelta <= 0L || requestedDelta > MultiAreaTimePacingConstants.MaxTicksPerPress)
            {
                string detail = requestedDelta <= 0L
                    ? FormattableString.Invariant($"requested target year {requestedTargetYear} must be greater than current year {startYear}")
                    : FormattableString.Invariant($"requested target year {requestedTargetYear} exceeds the maximum {MultiAreaTimePacingConstants.MaxTicksPerPress}-year step from year {startYear}");
                return new AdvanceYearsResult(
                    StartYear: startYear,
                    DestinationYear: startYear,
                    RequestedTargetYear: requestedTargetYear,
                    CompletedYears: Array.Empty<AdvanceYearStepSummary>(),
                    ReachedTarget: false,
                    StopReason: TimePacingStopReason.InvalidTargetYear,
                    FreshFence: CurrentFenceLocked(),
                    IsObservationPeriod: isObservationPeriod,
                    AnchorFellYear: anchorFellYear,
                    CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                    StopDetail: detail);
            }

            var completed = new List<AdvanceYearStepSummary>((int)requestedDelta);
            while (root.EcologicalYear < requestedTargetYear)
            {
                // Cancellation is honoured strictly between annual steps, never mid-step.
                if (cancellationToken.IsCancellationRequested)
                {
                    return new AdvanceYearsResult(
                        StartYear: startYear,
                        DestinationYear: root.EcologicalYear,
                        RequestedTargetYear: requestedTargetYear,
                        CompletedYears: Array.AsReadOnly(completed.ToArray()),
                        ReachedTarget: false,
                        StopReason: TimePacingStopReason.CancelledBetweenYears,
                        FreshFence: CurrentFenceLocked(),
                        IsObservationPeriod: isObservationPeriod,
                        AnchorFellYear: anchorFellYear,
                        CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                        StopDetail: "cancelled between annual steps");
                }

                var stepFence = CurrentFenceLocked();
                MultiAreaCommandResult step;
                try
                {
                    step = Advance(stepFence, consentToCancelWork: false);
                }
                catch (P1Refusal refusal)
                {
                    var stopReason = refusal.Code switch
                    {
                        "ContinuationNoLivingStems" => TimePacingStopReason.StandCannotAdvance,
                        "WorkPendingConsentRequired" => TimePacingStopReason.WorkPlanOpen,
                        _ => TimePacingStopReason.AnnualStepRefused,
                    };
                    return new AdvanceYearsResult(
                        StartYear: startYear,
                        DestinationYear: root.EcologicalYear,
                        RequestedTargetYear: requestedTargetYear,
                        CompletedYears: Array.AsReadOnly(completed.ToArray()),
                        ReachedTarget: false,
                        StopReason: stopReason,
                        FreshFence: CurrentFenceLocked(),
                        IsObservationPeriod: isObservationPeriod,
                        AnchorFellYear: anchorFellYear,
                        CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                        StopDetail: refusal.Code + ": " + refusal.Message);
                }

                var yearRecord = root.AnnualHistory[^1];
                completed.Add(new AdvanceYearStepSummary(yearRecord.YearBefore, yearRecord.YearAfter, yearRecord, step.Annual!));
                afterYearCommitted?.Invoke(yearRecord.YearAfter);
            }

            return new AdvanceYearsResult(
                StartYear: startYear,
                DestinationYear: root.EcologicalYear,
                RequestedTargetYear: requestedTargetYear,
                CompletedYears: Array.AsReadOnly(completed.ToArray()),
                ReachedTarget: true,
                StopReason: TimePacingStopReason.ReachedReviewYear,
                FreshFence: CurrentFenceLocked(),
                IsObservationPeriod: isObservationPeriod,
                AnchorFellYear: anchorFellYear,
                CalibrationNote: MultiAreaTimePacingConstants.CalibrationNote,
                StopDetail: null);
        }
    }

    // Caller holds sync.
    private MultiAreaFence CurrentFenceLocked() => new(sessionId, Token(root), root.PlanStateRevision);
}

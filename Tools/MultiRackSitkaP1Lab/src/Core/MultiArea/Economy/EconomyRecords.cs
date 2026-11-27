namespace SitkaP1.Core;

// Immutable Phase 2B Economy state, quote, approval, settlement, ledger, view, and history records.
// Published atomically inside MultiAreaRoot (null whenever EconomyModelVersion == Off).

/// <summary>Typed ledger entry category for every euro cent moved on Execute.</summary>
internal enum EconomyLedgerCategory
{
    HarvestingAndForwarding = 0,
    MinimumJobAdjustment = 1,
    ContractorLabour = 2,
    Materials = 3,
    TimberSale = 4,
}

/// <summary>One atomic financial ledger line. Positive = receipt, negative = external cost payment (ex VAT).</summary>
internal sealed record EconomyLedgerEntry(
    EconomyLedgerCategory Category,
    string ReferenceId,
    long CashDeltaCents,
    int EcologicalYear,
    int WorkRevision,
    string ExecutionKey,
    string PlanId);

/// <summary>One unmet economic precondition found while quoting a Work Plan.</summary>
internal sealed record EconomyQuoteIssue(string Code, string Message, long Required = 0L, long Available = 0L);

/// <summary>Grouped contractor harvest job quote/settlement for all Fell orders in one Work Plan.</summary>
internal sealed record EconomyHarvestQuote(
    HarvestThinningStage Stage,
    long StageRateCentsPerTonne,
    int FelledStemCount,
    long TotalStemVolumeCm3,
    long SaleVolumeCm3,
    long ResidualVolumeCm3,
    long HarvestedGreenGrams,
    long VariableHarvestCostCents,
    long MinimumJobFloorCents,
    long MinimumJobAdjustmentCents,
    long TotalHarvestContractorCostCents,
    long TimberSaleRevenueCents,
    IReadOnlyList<D062StemYield> Stems,
    IReadOnlyList<D062AssortmentSummary> Assortments)
{
    // ---- Phase 2B-2b felling dispositions (init-only; defaults keep every Phase 2B-1 construction unchanged) ----
    // Stems lists every graded felled stem; SaleVolumeCm3/ResidualVolumeCm3/Assortments/TimberSaleRevenueCents cover only
    // SellAndExtract stems, so TotalStemVolumeCm3 == Sale + Residual + RetainedStemVolumeCm3 + DeadwoodStemVolumeCm3.

    /// <summary>Per-stem disposition outcome, in the same order as <see cref="Stems"/>.</summary>
    public IReadOnlyList<FelledStemDispositionLine> StemDispositions { get; init; } = Array.Empty<FelledStemDispositionLine>();

    /// <summary>Exact engine-graded stem volume of KeepForUse stems (cm³).</summary>
    public long RetainedStemVolumeCm3 { get; init; }

    /// <summary>Exact engine-graded stem volume of LeaveAsDeadwood stems (cm³).</summary>
    public long DeadwoodStemVolumeCm3 { get; init; }

    /// <summary>Expected retained-timber delta of this job (physical, non-cash).</summary>
    public RetainedTimberTally RetainedTimber { get; init; } = RetainedTimberTally.Empty;

    /// <summary>Expected deadwood intents of this job (one per LeaveAsDeadwood stem, area then tree order).</summary>
    public IReadOnlyList<FelledStemDeadwoodIntent> DeadwoodIntents { get; init; } = Array.Empty<FelledStemDeadwoodIntent>();
}

/// <summary>One non-harvest Work Plan line (Prune, ClearUndergrowth, Plant, Shelter, or SpotClear).</summary>
internal sealed record EconomyNonHarvestLineQuote(
    EconomyOrderKey OrderKey,
    WorkExecutor Executor,
    int PersonMinutes,
    int OwnerMinutes,
    int ContractorMinutes,
    long ContractorLabourCents,
    string? MaterialId,
    long MaterialCostCents,
    long TotalExternalCostCents);

/// <summary>Pure, read-only Work Plan economic quote bound to one <see cref="PlanStateRevision"/> and committed root.</summary>
internal sealed record EconomyWorkPlanQuote(
    int PlanStateRevision,
    MultiAreaSourceToken Source,
    int EcologicalYear,
    long AvailableCashBeforeCents,
    int AvailableLandownerMinutesBefore,
    EconomyHarvestQuote? HarvestJob,
    IReadOnlyList<EconomyNonHarvestLineQuote> NonHarvestLines,
    long ContractorHarvestCostCents,
    long ContractorNonHarvestLabourCents,
    long MaterialCostCents,
    long TotalExternalCostCents,
    long UpfrontCashRequiredCents,
    long TimberSaleRevenueCents,
    long NetCashDeltaCents,
    int TotalOwnerMinutesRequired,
    int TotalContractorMinutes,
    IReadOnlyList<EconomyLedgerEntry> EstimatedLedger,
    IReadOnlyList<EconomyQuoteIssue> Issues,
    IReadOnlyList<string> EvidenceLabels)
{
    internal bool IsEligible => Issues.Count == 0;

    /// <summary>
    /// Phase 2B-2b: the resolved disposition of EVERY Fell order this quote priced (defaults included), sorted by key.
    /// Frozen into the approval with the quote, exactly like <see cref="EconomyApprovedQuote.FrozenOrderExecutors"/>.
    /// </summary>
    public IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> FellDispositions { get; init; } = FellingDispositionRules.EmptyResolved;
}

/// <summary>Immutable quote frozen into <see cref="MultiAreaEconomyState"/> by <c>Approve</c> (C-ECON).</summary>
internal sealed record EconomyApprovedQuote(
    string PlanId,
    string ExecutionKey,
    int QuotedAtPlanStateRevision,
    int ApprovedAtPlanStateRevision,
    IReadOnlyDictionary<EconomyOrderKey, WorkExecutor> FrozenOrderExecutors,
    EconomyWorkPlanQuote Quote)
{
    /// <summary>Phase 2B-2b: the felling dispositions frozen at Approve (carried inside the frozen quote).</summary>
    internal IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> FrozenFellDispositions => Quote.FellDispositions;
}

/// <summary>Immutable atomic settlement receipt committed together with physical Execute and returned on replay.</summary>
internal sealed record EconomySettlementReceipt(
    string ExecutionKey,
    string PlanId,
    int EcologicalYear,
    int WorkRevision,
    long CashBeforeCents,
    long TotalExternalCostCents,
    long TimberSaleRevenueCents,
    long NetCashDeltaCents,
    long CashAfterCents,
    int OwnerMinutesBefore,
    int OwnerMinutesConsumed,
    int OwnerMinutesAfter,
    EconomyHarvestQuote? HarvestJob,
    IReadOnlyList<EconomyNonHarvestLineQuote> NonHarvestLines,
    IReadOnlyList<EconomyLedgerEntry> LedgerEntries)
{
    /// <summary>Phase 2B-2b: retained (keep-for-use) timber added by this settlement, stamped with its execution identity. Never cash.</summary>
    public RetainedTimberTally RetainedTimberDelta { get; init; } = RetainedTimberTally.Empty;

    /// <summary>Phase 2B-2b: deadwood intents of this settlement (LeaveAsDeadwood stems), stamped with its execution identity.</summary>
    public IReadOnlyList<FelledStemDeadwoodIntent> DeadwoodIntents { get; init; } = Array.Empty<FelledStemDeadwoodIntent>();
}

/// <summary>Read-only year-end economy summary recorded by <see cref="ReservedPhase2BYearEndSummaryStep"/> before allowance reset.</summary>
internal sealed record EconomyYearEndSummary(
    int YearBefore,
    int YearAfter,
    long OpeningCashCents,
    long ClosingCashCents,
    long YearExternalCostCents,
    long YearTimberRevenueCents,
    long YearNetCashDeltaCents,
    int YearLandownerMinutesUsed,
    int YearLandownerMinutesRemainingBeforeReset,
    int AnnualLandownerAllowanceMinutesAfterReset,
    int HarvestPlansExecutedInYear,
    int TotalPlansSettledInYear);

/// <summary>
/// The Economy slice appended last on <see cref="MultiAreaRoot"/>. Always <c>null</c> when <see cref="EconomyModelVersion.Off"/>.
/// Published only via <see cref="MultiAreaContinuation"/>'s single root assignment under <c>sync</c>.
/// </summary>
internal sealed record MultiAreaEconomyState(
    EconomyOptions Options,
    long CashBalanceCents,
    int UsedLandownerMinutesThisYear,
    int RemainingLandownerMinutes,
    int ExecutedHarvestPlansCount,
    IReadOnlyDictionary<EconomyOrderKey, WorkExecutor> PendingOrderExecutors,
    EconomyApprovedQuote? ApprovedQuote,
    IReadOnlyList<EconomySettlementReceipt> Settlements,
    IReadOnlyList<EconomyLedgerEntry> Ledger,
    IReadOnlyList<EconomyYearEndSummary> YearEndSummaries)
{
    /// <summary>
    /// Phase 2B-2b: explicit non-default felling dispositions chosen on pending Fell orders. A choice is effective only for
    /// the exact order instance it was set on (see <see cref="FellingDispositionChoice"/>). Empty by default.
    /// </summary>
    public IReadOnlyDictionary<EconomyOrderKey, FellingDispositionChoice> PendingFellDispositions { get; init; } = FellingDispositionRules.EmptyChoices;

    /// <summary>Phase 2B-2b: cumulative retained (keep-for-use) timber balance. Physical, never cash. Empty by default.</summary>
    public RetainedTimberTally RetainedTimber { get; init; } = RetainedTimberTally.Empty;

    internal static MultiAreaEconomyState CreateInitial(EconomyOptions options)
    {
        var valid = options.RequireValid();
        return new MultiAreaEconomyState(
            valid,
            CashBalanceCents: valid.OpeningCashCents,
            UsedLandownerMinutesThisYear: 0,
            RemainingLandownerMinutes: valid.AnnualLandownerAllowanceMinutes,
            ExecutedHarvestPlansCount: 0,
            PendingOrderExecutors: new Dictionary<EconomyOrderKey, WorkExecutor>(),
            ApprovedQuote: null,
            Settlements: Array.Empty<EconomySettlementReceipt>(),
            Ledger: Array.Empty<EconomyLedgerEntry>(),
            YearEndSummaries: Array.Empty<EconomyYearEndSummary>());
    }
}

/// <summary>Revision-fenced (C-FENCE) read-only Economy snapshot of one committed root.</summary>
internal sealed record MultiAreaEconomyView(
    string SessionId,
    int PlanStateRevision,
    MultiAreaSourceToken Source,
    MultiAreaModelSelection Models,
    EconomyOptions Options,
    long CashBalanceCents,
    int UsedLandownerMinutesThisYear,
    int RemainingLandownerMinutes,
    int ExecutedHarvestPlansCount,
    IReadOnlyDictionary<EconomyOrderKey, WorkExecutor> PendingOrderExecutors,
    EconomyApprovedQuote? ApprovedQuote,
    IReadOnlyList<EconomySettlementReceipt> Settlements,
    IReadOnlyList<EconomyLedgerEntry> Ledger,
    IReadOnlyList<EconomyYearEndSummary> YearEndSummaries,
    IReadOnlyList<string> EvidenceLabels)
{
    /// <summary>Phase 2B-2b: explicit non-default dispositions of current pending Fell orders (missing = SellAndExtract).</summary>
    public IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> PendingFellDispositions { get; init; } = FellingDispositionRules.EmptyResolved;

    /// <summary>Phase 2B-2b: cumulative retained (keep-for-use) timber balance.</summary>
    public RetainedTimberTally RetainedTimber { get; init; } = RetainedTimberTally.Empty;
}

/// <summary>
/// Read-only per-year economic history readout. When Economy is Off, every money field reads <c>"NOT RECORDED"</c>
/// with <c>null</c> numeric cents — NEVER <c>€0</c> or <c>0</c>.
/// </summary>
internal sealed record EconomyYearHistory(
    int Year,
    bool EconomyActive,
    HistoryProvenance Provenance,
    string ProvenanceLabel,
    long? OpeningCashCents,
    long? ClosingCashCents,
    long? TotalExternalCostCents,
    long? TotalTimberRevenueCents,
    long? NetCashDeltaCents,
    int? LandownerMinutesUsed,
    string OpeningCashDisplay,
    string ClosingCashDisplay,
    string TotalExternalCostDisplay,
    string TotalTimberRevenueDisplay,
    string NetCashDeltaDisplay,
    string LandownerMinutesUsedDisplay,
    IReadOnlyList<EconomySettlementReceipt> Settlements,
    IReadOnlyList<EconomyLedgerEntry> Ledger,
    string Disclosure);

/// <summary>Pure read-only Economy history query over one immutable <see cref="MultiAreaRoot"/>.</summary>
internal static class EconomyHistoryQuery
{
    internal const string NotRecordedText = "NOT RECORDED";

    internal const string OffDisclosure =
        "Economy is OFF — cash balance, contractor costs, material costs, timber revenue, and landowner hours are NOT RECORDED (never fabricated as zero).";

    internal static EconomyYearHistory ReadYear(MultiAreaRoot root, int year)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (year < 0)
            throw new P1Refusal("HistoryYearInvalid", "history year must be non-negative");

        if (root.Economy is null)
        {
            return new EconomyYearHistory(
                Year: year,
                EconomyActive: false,
                Provenance: HistoryProvenance.NotRecorded,
                ProvenanceLabel: HistoryProvenanceCatalog.LabelOf(HistoryProvenance.NotRecorded),
                OpeningCashCents: null,
                ClosingCashCents: null,
                TotalExternalCostCents: null,
                TotalTimberRevenueCents: null,
                NetCashDeltaCents: null,
                LandownerMinutesUsed: null,
                OpeningCashDisplay: NotRecordedText,
                ClosingCashDisplay: NotRecordedText,
                TotalExternalCostDisplay: NotRecordedText,
                TotalTimberRevenueDisplay: NotRecordedText,
                NetCashDeltaDisplay: NotRecordedText,
                LandownerMinutesUsedDisplay: NotRecordedText,
                Settlements: Array.Empty<EconomySettlementReceipt>(),
                Ledger: Array.Empty<EconomyLedgerEntry>(),
                Disclosure: OffDisclosure);
        }

        var econ = root.Economy;
        var yearSettlements = econ.Settlements.Where(s => s.EcologicalYear == year).ToArray();
        var yearLedger = econ.Ledger.Where(e => e.EcologicalYear == year).ToArray();
        var yearEnd = econ.YearEndSummaries.FirstOrDefault(s => s.YearBefore == year);

        long totalCost = 0L;
        long totalRevenue = 0L;
        int ownerMinutes = 0;
        checked
        {
            foreach (var s in yearSettlements)
            {
                totalCost += s.TotalExternalCostCents;
                totalRevenue += s.TimberSaleRevenueCents;
                ownerMinutes += s.OwnerMinutesConsumed;
            }
        }
        long net = checked(totalRevenue - totalCost);

        long? openingCash = yearEnd?.OpeningCashCents
            ?? (yearSettlements.Length > 0 ? yearSettlements[0].CashBeforeCents
                : (year == root.EcologicalYear ? econ.CashBalanceCents : null));
        long? closingCash = yearEnd?.ClosingCashCents
            ?? (yearSettlements.Length > 0 ? yearSettlements[^1].CashAfterCents
                : (year == root.EcologicalYear ? econ.CashBalanceCents : null));

        return new EconomyYearHistory(
            Year: year,
            EconomyActive: true,
            Provenance: HistoryProvenance.Recorded,
            ProvenanceLabel: HistoryProvenanceCatalog.LabelOf(HistoryProvenance.Recorded),
            OpeningCashCents: openingCash,
            ClosingCashCents: closingCash,
            TotalExternalCostCents: totalCost,
            TotalTimberRevenueCents: totalRevenue,
            NetCashDeltaCents: net,
            LandownerMinutesUsed: ownerMinutes,
            OpeningCashDisplay: openingCash is long oc ? EconomyDefaults.FormatEuros(oc) : NotRecordedText,
            ClosingCashDisplay: closingCash is long cc ? EconomyDefaults.FormatEuros(cc) : NotRecordedText,
            TotalExternalCostDisplay: EconomyDefaults.FormatEuros(totalCost),
            TotalTimberRevenueDisplay: EconomyDefaults.FormatEuros(totalRevenue),
            NetCashDeltaDisplay: EconomyDefaults.FormatEuros(net),
            LandownerMinutesUsedDisplay: FormattableString.Invariant($"{ownerMinutes} min"),
            Settlements: Array.AsReadOnly(yearSettlements),
            Ledger: Array.AsReadOnly(yearLedger),
            Disclosure: EconomyEvidence.BroadleafD062);
    }
}

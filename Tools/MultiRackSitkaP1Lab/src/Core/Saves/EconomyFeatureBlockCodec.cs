using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

[assembly: InternalsVisibleTo("Phase1Save.Tests")]

namespace SitkaP1.Core.Saves;

/// <summary>
/// Phase 2B-2b Save v3 feature-block codec for <c>blocks.economy</c> (C-V3 contract).
/// Pure .NET, zero UI/HTTP dependencies, canonical compact UTF-8 JSON with fixed key order,
/// exact integer/bit encodings, and fail-closed validation on version, schema, invariants, and canonical bytes.
/// </summary>
internal sealed class EconomyFeatureBlockCodec : IFeatureBlockCodec
{
    private static readonly string[] RootKeys =
    {
        "options",
        "cashBalanceCents",
        "usedLandownerMinutesThisYear",
        "remainingLandownerMinutes",
        "executedHarvestPlansCount",
        "pendingOrderExecutors",
        "retainedTimber",
        "settlements",
        "ledger",
        "yearEndSummaries",
    };

    private static readonly string[] OptionsKeys =
    {
        "openingCashCents",
        "annualLandownerAllowanceMinutes",
        "contractorHourlyRateCents",
        "minimumHarvestJobCents",
    };

    private static readonly string[] PendingExecutorKeys =
    {
        "kind",
        "areaId",
        "targetKey",
        "executor",
    };

    private static readonly string[] RetainedTallyKeys =
    {
        "retainedTimberCm3",
        "bySpecies",
        "stems",
    };

    private static readonly string[] RetainedSpeciesKeys =
    {
        "speciesId",
        "retainedTimberCm3",
    };

    private static readonly string[] RetainedStemKeys =
    {
        "areaId",
        "treeId",
        "speciesId",
        "stemVolumeCm3",
        "ecologicalYear",
        "workRevision",
        "executionKey",
        "planId",
    };

    private static readonly string[] SettlementKeys =
    {
        "executionKey",
        "planId",
        "ecologicalYear",
        "workRevision",
        "cashBeforeCents",
        "totalExternalCostCents",
        "timberSaleRevenueCents",
        "netCashDeltaCents",
        "cashAfterCents",
        "ownerMinutesBefore",
        "ownerMinutesConsumed",
        "ownerMinutesAfter",
        "harvestJob",
        "nonHarvestLines",
        "ledgerEntries",
        "retainedTimberDelta",
        "deadwoodIntents",
    };

    private static readonly string[] HarvestJobKeys =
    {
        "stage",
        "stageRateCentsPerTonne",
        "felledStemCount",
        "totalStemVolumeCm3",
        "saleVolumeCm3",
        "residualVolumeCm3",
        "retainedStemVolumeCm3",
        "deadwoodStemVolumeCm3",
        "harvestedGreenGrams",
        "variableHarvestCostCents",
        "minimumJobFloorCents",
        "minimumJobAdjustmentCents",
        "totalHarvestContractorCostCents",
        "timberSaleRevenueCents",
        "stems",
        "assortments",
        "stemDispositions",
        "retainedTimber",
        "deadwoodIntents",
    };

    private static readonly string[] StemYieldKeys =
    {
        "treeId",
        "speciesId",
        "dbhMm",
        "totalHeightMm",
        "stemVolumeCm3",
        "saleVolumeCm3",
        "residualVolumeCm3",
        "harvestedGreenGrams",
        "timberRevenueCents",
        "hasPrunedSection",
        "prunedEndHeightMm",
        "logs",
    };

    private static readonly string[] AllocatedLogKeys =
    {
        "logId",
        "treeId",
        "speciesId",
        "specificationId",
        "assortment",
        "startHeightMm",
        "endHeightMm",
        "smallEndDiameterMm",
        "largeEndDiameterMm",
        "volumeCm3",
        "greenGrams",
        "unitPriceCentsPerTonne",
        "revenueCents",
        "quality",
    };

    private static readonly string[] AssortmentKeys =
    {
        "speciesId",
        "assortment",
        "pieceCount",
        "volumeCm3",
        "greenGrams",
        "unitPriceCentsPerTonne",
        "revenueCents",
    };

    private static readonly string[] StemDispositionKeys =
    {
        "areaId",
        "treeId",
        "speciesId",
        "disposition",
        "stemVolumeCm3",
        "harvestedGreenGrams",
        "creditedTimberRevenueCents",
        "retainedTimberCm3",
        "emitsDeadwoodIntent",
    };

    private static readonly string[] DeadwoodIntentKeys =
    {
        "areaId",
        "treeId",
        "speciesId",
        "xM",
        "zM",
        "dbhCm",
        "heightM",
        "volumeM3",
        "ecologicalYear",
        "workRevision",
        "executionKey",
        "planId",
    };

    private static readonly string[] NonHarvestLineKeys =
    {
        "kind",
        "areaId",
        "targetKey",
        "executor",
        "personMinutes",
        "ownerMinutes",
        "contractorMinutes",
        "contractorLabourCents",
        "materialId",
        "materialCostCents",
        "totalExternalCostCents",
    };

    private static readonly string[] LedgerEntryKeys =
    {
        "category",
        "referenceId",
        "cashDeltaCents",
        "ecologicalYear",
        "workRevision",
        "executionKey",
        "planId",
    };

    private static readonly string[] YearEndSummaryKeys =
    {
        "yearBefore",
        "yearAfter",
        "openingCashCents",
        "closingCashCents",
        "yearExternalCostCents",
        "yearTimberRevenueCents",
        "yearNetCashDeltaCents",
        "yearLandownerMinutesUsed",
        "yearLandownerMinutesRemainingBeforeReset",
        "annualLandownerAllowanceMinutesAfterReset",
        "harvestPlansExecutedInYear",
        "totalPlansSettledInYear",
    };

    public string BlockKey => SaveV3Format.EconomyBlockKey;

    public int CurrentBlockVersion => 1;

    public bool CanRead(int blockVersion) => blockVersion == 1;

    public string Encode(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (root.Economy is not { } econ)
            throw new P1Refusal("SaveRequiredBlockMissing", "cannot encode economy block: root has no Economy state");
        ValidateInvariants(econ);
        return EncodeCanonical(econ);
    }

    public MultiAreaRoot Decode(string canonicalBlockJson, int blockVersion, MultiAreaRoot restoredSoFar)
    {
        ArgumentNullException.ThrowIfNull(canonicalBlockJson);
        ArgumentNullException.ThrowIfNull(restoredSoFar);
        var econ = DecodeState(canonicalBlockJson, blockVersion);

        // Re-attach restored settlement receipts onto Executed records by ExecutionKey so replay history stays complete.
        IReadOnlyDictionary<string, MultiAreaExecutedWork> updatedExecuted = restoredSoFar.Executed;
        if (econ.Settlements.Count > 0 && restoredSoFar.Executed.Count > 0)
        {
            var settlementsByKey = new Dictionary<string, EconomySettlementReceipt>(StringComparer.Ordinal);
            foreach (var s in econ.Settlements)
            {
                if (!settlementsByKey.TryAdd(s.ExecutionKey, s))
                    throw new P1Refusal("SaveInconsistent", "economy block contains duplicate settlement executionKey: " + s.ExecutionKey);
            }
            var copy = new Dictionary<string, MultiAreaExecutedWork>(restoredSoFar.Executed, StringComparer.Ordinal);
            foreach (var (key, work) in restoredSoFar.Executed)
            {
                if (settlementsByKey.TryGetValue(key, out var receipt))
                    copy[key] = work with { Settlement = receipt };
            }
            updatedExecuted = copy;
        }

        return restoredSoFar with
        {
            Economy = econ,
            Executed = updatedExecuted,
        };
    }

    internal MultiAreaEconomyState DecodeState(string canonicalBlockJson, int blockVersion)
    {
        if (!CanRead(blockVersion))
            throw new P1Refusal("SaveBlockVersionUnsupported",
                $"economy block version {blockVersion} is not supported by this build");

        byte[] raw = Encoding.UTF8.GetBytes(canonicalBlockJson);
        var node = StrictJson.Parse(raw);
        var map = CanonicalKeys(node, "blocks.economy.data", RootKeys);

        var options = ReadOptions(map["options"]);
        long cashBalance = ReadI64(map["cashBalanceCents"], "economy.cashBalanceCents");
        int usedMinutes = ReadI32(map["usedLandownerMinutesThisYear"], "economy.usedLandownerMinutesThisYear");
        int remainingMinutes = ReadI32(map["remainingLandownerMinutes"], "economy.remainingLandownerMinutes");
        int harvestCount = ReadI32(map["executedHarvestPlansCount"], "economy.executedHarvestPlansCount");

        var pendingExecutors = new Dictionary<EconomyOrderKey, WorkExecutor>();
        EconomyOrderKey? prevExecKey = null;
        foreach (var item in J.Arr(map["pendingOrderExecutors"], "economy.pendingOrderExecutors").Items)
        {
            var em = CanonicalKeys(item, "economy.pendingOrderExecutor", PendingExecutorKeys);
            var kind = ReadEnum<EconomyOrderKind>(em["kind"], "pendingOrderExecutor.kind");
            string areaId = J.Str(em["areaId"], "pendingOrderExecutor.areaId");
            string targetKey = J.Str(em["targetKey"], "pendingOrderExecutor.targetKey");
            var exec = ReadEnum<WorkExecutor>(em["executor"], "pendingOrderExecutor.executor");
            var orderKey = new EconomyOrderKey(kind, areaId, targetKey);
            if (prevExecKey is { } prev && prev.CompareTo(orderKey) >= 0)
                throw new P1Refusal("SaveNotCanonical", "economy.pendingOrderExecutors must be strictly ordered by key");
            prevExecKey = orderKey;
            if (!pendingExecutors.TryAdd(orderKey, exec))
                throw new P1Refusal("SaveInconsistent", "duplicate key in economy.pendingOrderExecutors");
        }

        var retainedTimber = ReadRetainedTally(map["retainedTimber"], "economy.retainedTimber");
        var settlements = ReadList(map["settlements"], "economy.settlements", ReadSettlement);
        var ledger = ReadList(map["ledger"], "economy.ledger", ReadLedgerEntry);
        var yearEnds = ReadList(map["yearEndSummaries"], "economy.yearEndSummaries", ReadYearEndSummary);

        // Policy A: saved approvals never come back approved, and PendingFellDispositions starts empty rather than
        // binding onto rebuilt order instances (see Phase 2B-2b handoff).
        var decoded = new MultiAreaEconomyState(
            Options: options,
            CashBalanceCents: cashBalance,
            UsedLandownerMinutesThisYear: usedMinutes,
            RemainingLandownerMinutes: remainingMinutes,
            ExecutedHarvestPlansCount: harvestCount,
            PendingOrderExecutors: pendingExecutors,
            ApprovedQuote: null,
            Settlements: settlements,
            Ledger: ledger,
            YearEndSummaries: yearEnds)
        {
            PendingFellDispositions = FellingDispositionRules.EmptyChoices,
            RetainedTimber = retainedTimber,
        };

        ValidateInvariants(decoded);

        string canonical = EncodeCanonical(decoded);
        if (!string.Equals(canonical, canonicalBlockJson, StringComparison.Ordinal))
            throw new P1Refusal("SaveNotCanonical", "blocks.economy.data is not in canonical form");

        return decoded;
    }

    // ------------------------------------------------------------------------------------------------ canonical encoder

    internal static string EncodeCanonical(MultiAreaEconomyState econ)
    {
        var w = new Phase1JsonWriter();
        w.BeginObject();
        w.BeginObject("options")
            .I64("openingCashCents", econ.Options.OpeningCashCents)
            .Int("annualLandownerAllowanceMinutes", econ.Options.AnnualLandownerAllowanceMinutes)
            .I64("contractorHourlyRateCents", econ.Options.ContractorHourlyRateCents)
            .I64("minimumHarvestJobCents", econ.Options.MinimumHarvestJobCents)
            .EndObject();
        w.I64("cashBalanceCents", econ.CashBalanceCents)
            .Int("usedLandownerMinutesThisYear", econ.UsedLandownerMinutesThisYear)
            .Int("remainingLandownerMinutes", econ.RemainingLandownerMinutes)
            .Int("executedHarvestPlansCount", econ.ExecutedHarvestPlansCount);

        w.BeginArray("pendingOrderExecutors");
        foreach (var (key, exec) in econ.PendingOrderExecutors.OrderBy(kv => kv.Key))
        {
            w.BeginObject()
                .Str("kind", key.Kind.ToString())
                .Str("areaId", key.AreaId)
                .Str("targetKey", key.TargetKey)
                .Str("executor", exec.ToString())
                .EndObject();
        }
        w.EndArray();

        w.Key("retainedTimber");
        WriteRetainedTally(w, econ.RetainedTimber);

        w.BeginArray("settlements");
        foreach (var s in econ.Settlements)
            WriteSettlement(w, s);
        w.EndArray();

        w.BeginArray("ledger");
        foreach (var e in econ.Ledger)
            WriteLedgerEntry(w, e);
        w.EndArray();

        w.BeginArray("yearEndSummaries");
        foreach (var y in econ.YearEndSummaries)
        {
            w.BeginObject()
                .Int("yearBefore", y.YearBefore)
                .Int("yearAfter", y.YearAfter)
                .I64("openingCashCents", y.OpeningCashCents)
                .I64("closingCashCents", y.ClosingCashCents)
                .I64("yearExternalCostCents", y.YearExternalCostCents)
                .I64("yearTimberRevenueCents", y.YearTimberRevenueCents)
                .I64("yearNetCashDeltaCents", y.YearNetCashDeltaCents)
                .Int("yearLandownerMinutesUsed", y.YearLandownerMinutesUsed)
                .Int("yearLandownerMinutesRemainingBeforeReset", y.YearLandownerMinutesRemainingBeforeReset)
                .Int("annualLandownerAllowanceMinutesAfterReset", y.AnnualLandownerAllowanceMinutesAfterReset)
                .Int("harvestPlansExecutedInYear", y.HarvestPlansExecutedInYear)
                .Int("totalPlansSettledInYear", y.TotalPlansSettledInYear)
                .EndObject();
        }
        w.EndArray();
        w.EndObject();
        return w.ToString();
    }

    private static void WriteRetainedTally(Phase1JsonWriter w, RetainedTimberTally tally)
    {
        w.BeginObject()
            .I64("retainedTimberCm3", tally.RetainedTimberCm3)
            .BeginArray("bySpecies");
        foreach (var kv in tally.BySpecies.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            w.BeginObject()
                .Str("speciesId", kv.Value.SpeciesId)
                .I64("retainedTimberCm3", kv.Value.RetainedTimberCm3)
                .EndObject();
        }
        w.EndArray();
        w.BeginArray("stems");
        foreach (var st in tally.Stems)
        {
            w.BeginObject()
                .Str("areaId", st.AreaId)
                .Str("treeId", st.TreeId)
                .Str("speciesId", st.SpeciesId)
                .I64("stemVolumeCm3", st.StemVolumeCm3)
                .Int("ecologicalYear", st.EcologicalYear)
                .Int("workRevision", st.WorkRevision)
                .Str("executionKey", st.ExecutionKey)
                .Str("planId", st.PlanId)
                .EndObject();
        }
        w.EndArray().EndObject();
    }

    private static void WriteSettlement(Phase1JsonWriter w, EconomySettlementReceipt s)
    {
        w.BeginObject()
            .Str("executionKey", s.ExecutionKey)
            .Str("planId", s.PlanId)
            .Int("ecologicalYear", s.EcologicalYear)
            .Int("workRevision", s.WorkRevision)
            .I64("cashBeforeCents", s.CashBeforeCents)
            .I64("totalExternalCostCents", s.TotalExternalCostCents)
            .I64("timberSaleRevenueCents", s.TimberSaleRevenueCents)
            .I64("netCashDeltaCents", s.NetCashDeltaCents)
            .I64("cashAfterCents", s.CashAfterCents)
            .Int("ownerMinutesBefore", s.OwnerMinutesBefore)
            .Int("ownerMinutesConsumed", s.OwnerMinutesConsumed)
            .Int("ownerMinutesAfter", s.OwnerMinutesAfter);
        if (s.HarvestJob is null) w.Null("harvestJob");
        else { w.Key("harvestJob"); WriteHarvestJob(w, s.HarvestJob); }
        w.BeginArray("nonHarvestLines");
        foreach (var l in s.NonHarvestLines)
        {
            w.BeginObject()
                .Str("kind", l.OrderKey.Kind.ToString())
                .Str("areaId", l.OrderKey.AreaId)
                .Str("targetKey", l.OrderKey.TargetKey)
                .Str("executor", l.Executor.ToString())
                .Int("personMinutes", l.PersonMinutes)
                .Int("ownerMinutes", l.OwnerMinutes)
                .Int("contractorMinutes", l.ContractorMinutes)
                .I64("contractorLabourCents", l.ContractorLabourCents)
                .StrOrNull("materialId", l.MaterialId)
                .I64("materialCostCents", l.MaterialCostCents)
                .I64("totalExternalCostCents", l.TotalExternalCostCents)
                .EndObject();
        }
        w.EndArray();
        w.BeginArray("ledgerEntries");
        foreach (var e in s.LedgerEntries)
            WriteLedgerEntry(w, e);
        w.EndArray();
        w.Key("retainedTimberDelta");
        WriteRetainedTally(w, s.RetainedTimberDelta);
        WriteDeadwoodIntents(w, "deadwoodIntents", s.DeadwoodIntents);
        w.EndObject();
    }

    private static void WriteHarvestJob(Phase1JsonWriter w, EconomyHarvestQuote h)
    {
        w.BeginObject()
            .Str("stage", h.Stage.ToString())
            .I64("stageRateCentsPerTonne", h.StageRateCentsPerTonne)
            .Int("felledStemCount", h.FelledStemCount)
            .I64("totalStemVolumeCm3", h.TotalStemVolumeCm3)
            .I64("saleVolumeCm3", h.SaleVolumeCm3)
            .I64("residualVolumeCm3", h.ResidualVolumeCm3)
            .I64("retainedStemVolumeCm3", h.RetainedStemVolumeCm3)
            .I64("deadwoodStemVolumeCm3", h.DeadwoodStemVolumeCm3)
            .I64("harvestedGreenGrams", h.HarvestedGreenGrams)
            .I64("variableHarvestCostCents", h.VariableHarvestCostCents)
            .I64("minimumJobFloorCents", h.MinimumJobFloorCents)
            .I64("minimumJobAdjustmentCents", h.MinimumJobAdjustmentCents)
            .I64("totalHarvestContractorCostCents", h.TotalHarvestContractorCostCents)
            .I64("timberSaleRevenueCents", h.TimberSaleRevenueCents);

        w.BeginArray("stems");
        foreach (var st in h.Stems)
        {
            w.BeginObject()
                .Str("treeId", st.TreeId)
                .Str("speciesId", st.SpeciesId)
                .Int("dbhMm", st.DbhMm)
                .Int("totalHeightMm", st.TotalHeightMm)
                .I64("stemVolumeCm3", st.StemVolumeCm3)
                .I64("saleVolumeCm3", st.SaleVolumeCm3)
                .I64("residualVolumeCm3", st.ResidualVolumeCm3)
                .I64("harvestedGreenGrams", st.HarvestedGreenGrams)
                .I64("timberRevenueCents", st.TimberRevenueCents)
                .Bool("hasPrunedSection", st.HasPrunedSection)
                .Int("prunedEndHeightMm", st.PrunedEndHeightMm)
                .BeginArray("logs");
            foreach (var log in st.Logs)
            {
                w.BeginObject()
                    .Str("logId", log.LogId)
                    .Str("treeId", log.TreeId)
                    .Str("speciesId", log.SpeciesId)
                    .Str("specificationId", log.SpecificationId)
                    .Str("assortment", log.Assortment.ToString())
                    .Int("startHeightMm", log.StartHeightMm)
                    .Int("endHeightMm", log.EndHeightMm)
                    .Int("smallEndDiameterMm", log.SmallEndDiameterMm)
                    .Int("largeEndDiameterMm", log.LargeEndDiameterMm)
                    .I64("volumeCm3", log.VolumeCm3)
                    .I64("greenGrams", log.GreenGrams)
                    .I64("unitPriceCentsPerTonne", log.UnitPriceCentsPerTonne)
                    .I64("revenueCents", log.RevenueCents)
                    .Int("quality", (int)log.Quality)
                    .EndObject();
            }
            w.EndArray().EndObject();
        }
        w.EndArray();

        w.BeginArray("assortments");
        foreach (var a in h.Assortments)
        {
            w.BeginObject()
                .Str("speciesId", a.SpeciesId)
                .Str("assortment", a.Assortment.ToString())
                .Int("pieceCount", a.PieceCount)
                .I64("volumeCm3", a.VolumeCm3)
                .I64("greenGrams", a.GreenGrams)
                .I64("unitPriceCentsPerTonne", a.UnitPriceCentsPerTonne)
                .I64("revenueCents", a.RevenueCents)
                .EndObject();
        }
        w.EndArray();

        w.BeginArray("stemDispositions");
        foreach (var d in h.StemDispositions)
        {
            w.BeginObject()
                .Str("areaId", d.AreaId)
                .Str("treeId", d.TreeId)
                .Str("speciesId", d.SpeciesId)
                .Str("disposition", d.Disposition.ToString())
                .I64("stemVolumeCm3", d.StemVolumeCm3)
                .I64("harvestedGreenGrams", d.HarvestedGreenGrams)
                .I64("creditedTimberRevenueCents", d.CreditedTimberRevenueCents)
                .I64("retainedTimberCm3", d.RetainedTimberCm3)
                .Bool("emitsDeadwoodIntent", d.EmitsDeadwoodIntent)
                .EndObject();
        }
        w.EndArray();

        w.Key("retainedTimber");
        WriteRetainedTally(w, h.RetainedTimber);
        WriteDeadwoodIntents(w, "deadwoodIntents", h.DeadwoodIntents);
        w.EndObject();
    }

    private static void WriteDeadwoodIntents(Phase1JsonWriter w, string key, IReadOnlyList<FelledStemDeadwoodIntent> intents)
    {
        w.BeginArray(key);
        foreach (var i in intents)
        {
            w.BeginObject()
                .Str("areaId", i.AreaId)
                .Str("treeId", i.TreeId)
                .Str("speciesId", i.SpeciesId)
                .F32("xM", i.XM)
                .F32("zM", i.ZM)
                .F32("dbhCm", i.DbhCm)
                .F32("heightM", i.HeightM)
                .F32("volumeM3", i.VolumeM3)
                .Int("ecologicalYear", i.EcologicalYear)
                .Int("workRevision", i.WorkRevision)
                .Str("executionKey", i.ExecutionKey)
                .Str("planId", i.PlanId)
                .EndObject();
        }
        w.EndArray();
    }

    private static void WriteLedgerEntry(Phase1JsonWriter w, EconomyLedgerEntry e)
    {
        w.BeginObject()
            .Str("category", e.Category.ToString())
            .Str("referenceId", e.ReferenceId)
            .I64("cashDeltaCents", e.CashDeltaCents)
            .Int("ecologicalYear", e.EcologicalYear)
            .Int("workRevision", e.WorkRevision)
            .Str("executionKey", e.ExecutionKey)
            .Str("planId", e.PlanId)
            .EndObject();
    }

    // ------------------------------------------------------------------------------------------------ strict readers

    private static EconomyOptions ReadOptions(JNode node)
    {
        var m = CanonicalKeys(node, "economy.options", OptionsKeys);
        var opts = new EconomyOptions
        {
            OpeningCashCents = ReadI64(m["openingCashCents"], "options.openingCashCents"),
            AnnualLandownerAllowanceMinutes = ReadI32(m["annualLandownerAllowanceMinutes"], "options.annualLandownerAllowanceMinutes"),
            ContractorHourlyRateCents = ReadI64(m["contractorHourlyRateCents"], "options.contractorHourlyRateCents"),
            MinimumHarvestJobCents = ReadI64(m["minimumHarvestJobCents"], "options.minimumHarvestJobCents"),
        };
        return opts.RequireValid();
    }

    private static RetainedTimberTally ReadRetainedTally(JNode node, string what)
    {
        var m = CanonicalKeys(node, what, RetainedTallyKeys);
        long storedTotal = ReadI64(m["retainedTimberCm3"], what + ".retainedTimberCm3");
        if (storedTotal < 0L)
            throw new P1Refusal("SaveInconsistent", what + ".retainedTimberCm3 cannot be negative");

        var storedSpecies = ReadList(m["bySpecies"], what + ".bySpecies", x =>
        {
            var sm = CanonicalKeys(x, what + ".bySpecies[]", RetainedSpeciesKeys);
            string sp = J.Str(sm["speciesId"], "speciesId");
            long cm3 = ReadI64(sm["retainedTimberCm3"], "retainedTimberCm3");
            if (cm3 < 0L) throw new P1Refusal("SaveInconsistent", what + ".bySpecies volume cannot be negative");
            return new RetainedTimberAmount(sp, cm3);
        });
        for (int i = 1; i < storedSpecies.Count; i++)
        {
            if (string.CompareOrdinal(storedSpecies[i - 1].SpeciesId, storedSpecies[i].SpeciesId) >= 0)
                throw new P1Refusal("SaveNotCanonical", what + ".bySpecies must be unique and ordinal-sorted");
        }

        var stems = ReadList(m["stems"], what + ".stems", x =>
        {
            var st = CanonicalKeys(x, what + ".stems[]", RetainedStemKeys);
            long vol = ReadI64(st["stemVolumeCm3"], "stemVolumeCm3");
            if (vol < 0L) throw new P1Refusal("SaveInconsistent", what + ".stems volume cannot be negative");
            return new RetainedTimberStem(
                J.Str(st["areaId"], "areaId"),
                J.Str(st["treeId"], "treeId"),
                J.Str(st["speciesId"], "speciesId"),
                vol,
                ReadI32(st["ecologicalYear"], "ecologicalYear"),
                ReadI32(st["workRevision"], "workRevision"),
                J.Str(st["executionKey"], "executionKey"),
                J.Str(st["planId"], "planId"));
        });

        var rebuilt = RetainedTimberTally.FromStems(stems);
        if (rebuilt.RetainedTimberCm3 != storedTotal || rebuilt.BySpecies.Count != storedSpecies.Count)
            throw new P1Refusal("SaveInconsistent", what + " total or species breakdown does not match listed stems");
        foreach (var sp in storedSpecies)
        {
            if (!rebuilt.BySpecies.TryGetValue(sp.SpeciesId, out var actual) || actual.RetainedTimberCm3 != sp.RetainedTimberCm3)
                throw new P1Refusal("SaveInconsistent", what + ".bySpecies[" + sp.SpeciesId + "] does not match listed stems");
        }
        return rebuilt;
    }

    private static EconomySettlementReceipt ReadSettlement(JNode node)
    {
        var m = CanonicalKeys(node, "economy.settlement", SettlementKeys);
        var harvestJob = m["harvestJob"] is JNull ? null : ReadHarvestJob(m["harvestJob"]);
        var nonHarvest = ReadList(m["nonHarvestLines"], "settlement.nonHarvestLines", ReadNonHarvestLine);
        var entries = ReadList(m["ledgerEntries"], "settlement.ledgerEntries", ReadLedgerEntry);
        var retainedDelta = ReadRetainedTally(m["retainedTimberDelta"], "settlement.retainedTimberDelta");
        var intents = ReadList(m["deadwoodIntents"], "settlement.deadwoodIntents", ReadDeadwoodIntent);

        return new EconomySettlementReceipt(
            ExecutionKey: J.Str(m["executionKey"], "executionKey"),
            PlanId: J.Str(m["planId"], "planId"),
            EcologicalYear: ReadI32(m["ecologicalYear"], "ecologicalYear"),
            WorkRevision: ReadI32(m["workRevision"], "workRevision"),
            CashBeforeCents: ReadI64(m["cashBeforeCents"], "cashBeforeCents"),
            TotalExternalCostCents: ReadI64(m["totalExternalCostCents"], "totalExternalCostCents"),
            TimberSaleRevenueCents: ReadI64(m["timberSaleRevenueCents"], "timberSaleRevenueCents"),
            NetCashDeltaCents: ReadI64(m["netCashDeltaCents"], "netCashDeltaCents"),
            CashAfterCents: ReadI64(m["cashAfterCents"], "cashAfterCents"),
            OwnerMinutesBefore: ReadI32(m["ownerMinutesBefore"], "ownerMinutesBefore"),
            OwnerMinutesConsumed: ReadI32(m["ownerMinutesConsumed"], "ownerMinutesConsumed"),
            OwnerMinutesAfter: ReadI32(m["ownerMinutesAfter"], "ownerMinutesAfter"),
            HarvestJob: harvestJob,
            NonHarvestLines: nonHarvest,
            LedgerEntries: entries)
        {
            RetainedTimberDelta = retainedDelta,
            DeadwoodIntents = intents,
        };
    }

    private static EconomyHarvestQuote ReadHarvestJob(JNode node)
    {
        var m = CanonicalKeys(node, "settlement.harvestJob", HarvestJobKeys);
        var stems = ReadList(m["stems"], "harvestJob.stems", ReadStemYield);
        var assortments = ReadList(m["assortments"], "harvestJob.assortments", ReadAssortment);
        var dispositions = ReadList(m["stemDispositions"], "harvestJob.stemDispositions", ReadStemDisposition);
        var retained = ReadRetainedTally(m["retainedTimber"], "harvestJob.retainedTimber");
        var intents = ReadList(m["deadwoodIntents"], "harvestJob.deadwoodIntents", ReadDeadwoodIntent);

        return new EconomyHarvestQuote(
            Stage: ReadEnum<HarvestThinningStage>(m["stage"], "harvestJob.stage"),
            StageRateCentsPerTonne: ReadI64(m["stageRateCentsPerTonne"], "stageRateCentsPerTonne"),
            FelledStemCount: ReadI32(m["felledStemCount"], "felledStemCount"),
            TotalStemVolumeCm3: ReadI64(m["totalStemVolumeCm3"], "totalStemVolumeCm3"),
            SaleVolumeCm3: ReadI64(m["saleVolumeCm3"], "saleVolumeCm3"),
            ResidualVolumeCm3: ReadI64(m["residualVolumeCm3"], "residualVolumeCm3"),
            HarvestedGreenGrams: ReadI64(m["harvestedGreenGrams"], "harvestedGreenGrams"),
            VariableHarvestCostCents: ReadI64(m["variableHarvestCostCents"], "variableHarvestCostCents"),
            MinimumJobFloorCents: ReadI64(m["minimumJobFloorCents"], "minimumJobFloorCents"),
            MinimumJobAdjustmentCents: ReadI64(m["minimumJobAdjustmentCents"], "minimumJobAdjustmentCents"),
            TotalHarvestContractorCostCents: ReadI64(m["totalHarvestContractorCostCents"], "totalHarvestContractorCostCents"),
            TimberSaleRevenueCents: ReadI64(m["timberSaleRevenueCents"], "timberSaleRevenueCents"),
            Stems: stems,
            Assortments: assortments)
        {
            StemDispositions = dispositions,
            RetainedStemVolumeCm3 = ReadI64(m["retainedStemVolumeCm3"], "retainedStemVolumeCm3"),
            DeadwoodStemVolumeCm3 = ReadI64(m["deadwoodStemVolumeCm3"], "deadwoodStemVolumeCm3"),
            RetainedTimber = retained,
            DeadwoodIntents = intents,
        };
    }

    private static D062StemYield ReadStemYield(JNode node)
    {
        var m = CanonicalKeys(node, "harvestJob.stem", StemYieldKeys);
        var logs = ReadList(m["logs"], "stem.logs", x =>
        {
            var lm = CanonicalKeys(x, "stem.log", AllocatedLogKeys);
            int rawQuality = ReadI32(lm["quality"], "log.quality");
            if (rawQuality < 0 || rawQuality > 63)
                throw new P1Refusal("SaveEnumUnknown", "unknown StemQualityFlags value: " + rawQuality);
            return new D062AllocatedLog(
                J.Str(lm["logId"], "logId"),
                J.Str(lm["treeId"], "treeId"),
                J.Str(lm["speciesId"], "speciesId"),
                J.Str(lm["specificationId"], "specificationId"),
                ReadEnum<TimberAssortmentKind>(lm["assortment"], "log.assortment"),
                ReadI32(lm["startHeightMm"], "startHeightMm"),
                ReadI32(lm["endHeightMm"], "endHeightMm"),
                ReadI32(lm["smallEndDiameterMm"], "smallEndDiameterMm"),
                ReadI32(lm["largeEndDiameterMm"], "largeEndDiameterMm"),
                ReadI64(lm["volumeCm3"], "volumeCm3"),
                ReadI64(lm["greenGrams"], "greenGrams"),
                ReadI64(lm["unitPriceCentsPerTonne"], "unitPriceCentsPerTonne"),
                ReadI64(lm["revenueCents"], "revenueCents"),
                (StemQualityFlags)rawQuality);
        });
        return new D062StemYield(
            J.Str(m["treeId"], "treeId"),
            J.Str(m["speciesId"], "speciesId"),
            ReadI32(m["dbhMm"], "dbhMm"),
            ReadI32(m["totalHeightMm"], "totalHeightMm"),
            ReadI64(m["stemVolumeCm3"], "stemVolumeCm3"),
            ReadI64(m["saleVolumeCm3"], "saleVolumeCm3"),
            ReadI64(m["residualVolumeCm3"], "residualVolumeCm3"),
            ReadI64(m["harvestedGreenGrams"], "harvestedGreenGrams"),
            ReadI64(m["timberRevenueCents"], "timberRevenueCents"),
            ReadBool(m["hasPrunedSection"], "hasPrunedSection"),
            ReadI32(m["prunedEndHeightMm"], "prunedEndHeightMm"),
            logs);
    }

    private static D062AssortmentSummary ReadAssortment(JNode node)
    {
        var m = CanonicalKeys(node, "harvestJob.assortment", AssortmentKeys);
        return new D062AssortmentSummary(
            J.Str(m["speciesId"], "speciesId"),
            ReadEnum<TimberAssortmentKind>(m["assortment"], "assortment.assortment"),
            ReadI32(m["pieceCount"], "pieceCount"),
            ReadI64(m["volumeCm3"], "volumeCm3"),
            ReadI64(m["greenGrams"], "greenGrams"),
            ReadI64(m["unitPriceCentsPerTonne"], "unitPriceCentsPerTonne"),
            ReadI64(m["revenueCents"], "revenueCents"));
    }

    private static FelledStemDispositionLine ReadStemDisposition(JNode node)
    {
        var m = CanonicalKeys(node, "harvestJob.stemDisposition", StemDispositionKeys);
        return new FelledStemDispositionLine(
            J.Str(m["areaId"], "areaId"),
            J.Str(m["treeId"], "treeId"),
            J.Str(m["speciesId"], "speciesId"),
            ReadEnum<FellingDisposition>(m["disposition"], "stemDisposition.disposition"),
            ReadI64(m["stemVolumeCm3"], "stemVolumeCm3"),
            ReadI64(m["harvestedGreenGrams"], "harvestedGreenGrams"),
            ReadI64(m["creditedTimberRevenueCents"], "creditedTimberRevenueCents"),
            ReadI64(m["retainedTimberCm3"], "retainedTimberCm3"),
            ReadBool(m["emitsDeadwoodIntent"], "emitsDeadwoodIntent"));
    }

    private static FelledStemDeadwoodIntent ReadDeadwoodIntent(JNode node)
    {
        var m = CanonicalKeys(node, "deadwoodIntent", DeadwoodIntentKeys);
        return new FelledStemDeadwoodIntent(
            J.Str(m["areaId"], "areaId"),
            J.Str(m["treeId"], "treeId"),
            J.Str(m["speciesId"], "speciesId"),
            F32.FromBits(J.F32Bits(m["xM"], "xM")),
            F32.FromBits(J.F32Bits(m["zM"], "zM")),
            F32.FromBits(J.F32Bits(m["dbhCm"], "dbhCm")),
            F32.FromBits(J.F32Bits(m["heightM"], "heightM")),
            F32.FromBits(J.F32Bits(m["volumeM3"], "volumeM3")),
            ReadI32(m["ecologicalYear"], "ecologicalYear"),
            ReadI32(m["workRevision"], "workRevision"),
            J.Str(m["executionKey"], "executionKey"),
            J.Str(m["planId"], "planId"));
    }

    private static EconomyNonHarvestLineQuote ReadNonHarvestLine(JNode node)
    {
        var m = CanonicalKeys(node, "settlement.nonHarvestLine", NonHarvestLineKeys);
        var key = new EconomyOrderKey(
            ReadEnum<EconomyOrderKind>(m["kind"], "nonHarvestLine.kind"),
            J.Str(m["areaId"], "areaId"),
            J.Str(m["targetKey"], "targetKey"));
        return new EconomyNonHarvestLineQuote(
            key,
            ReadEnum<WorkExecutor>(m["executor"], "nonHarvestLine.executor"),
            ReadI32(m["personMinutes"], "personMinutes"),
            ReadI32(m["ownerMinutes"], "ownerMinutes"),
            ReadI32(m["contractorMinutes"], "contractorMinutes"),
            ReadI64(m["contractorLabourCents"], "contractorLabourCents"),
            J.StrOrNull(m["materialId"], "materialId"),
            ReadI64(m["materialCostCents"], "materialCostCents"),
            ReadI64(m["totalExternalCostCents"], "totalExternalCostCents"));
    }

    private static EconomyLedgerEntry ReadLedgerEntry(JNode node)
    {
        var m = CanonicalKeys(node, "economy.ledgerEntry", LedgerEntryKeys);
        return new EconomyLedgerEntry(
            ReadEnum<EconomyLedgerCategory>(m["category"], "ledgerEntry.category"),
            J.Str(m["referenceId"], "referenceId"),
            ReadI64(m["cashDeltaCents"], "cashDeltaCents"),
            ReadI32(m["ecologicalYear"], "ecologicalYear"),
            ReadI32(m["workRevision"], "workRevision"),
            J.Str(m["executionKey"], "executionKey"),
            J.Str(m["planId"], "planId"));
    }

    private static EconomyYearEndSummary ReadYearEndSummary(JNode node)
    {
        var m = CanonicalKeys(node, "economy.yearEndSummary", YearEndSummaryKeys);
        return new EconomyYearEndSummary(
            ReadI32(m["yearBefore"], "yearBefore"),
            ReadI32(m["yearAfter"], "yearAfter"),
            ReadI64(m["openingCashCents"], "openingCashCents"),
            ReadI64(m["closingCashCents"], "closingCashCents"),
            ReadI64(m["yearExternalCostCents"], "yearExternalCostCents"),
            ReadI64(m["yearTimberRevenueCents"], "yearTimberRevenueCents"),
            ReadI64(m["yearNetCashDeltaCents"], "yearNetCashDeltaCents"),
            ReadI32(m["yearLandownerMinutesUsed"], "yearLandownerMinutesUsed"),
            ReadI32(m["yearLandownerMinutesRemainingBeforeReset"], "yearLandownerMinutesRemainingBeforeReset"),
            ReadI32(m["annualLandownerAllowanceMinutesAfterReset"], "annualLandownerAllowanceMinutesAfterReset"),
            ReadI32(m["harvestPlansExecutedInYear"], "harvestPlansExecutedInYear"),
            ReadI32(m["totalPlansSettledInYear"], "totalPlansSettledInYear"));
    }

    // ------------------------------------------------------------------------------------------------ invariants

    private static void ValidateInvariants(MultiAreaEconomyState econ)
    {
        econ.Options.RequireValid();
        if (econ.CashBalanceCents < 0L)
            throw new P1Refusal("SaveInconsistent", "economy cashBalanceCents cannot be negative");
        if (econ.UsedLandownerMinutesThisYear < 0 || econ.RemainingLandownerMinutes < 0)
            throw new P1Refusal("SaveInconsistent", "economy landowner minutes cannot be negative");
        if (econ.UsedLandownerMinutesThisYear + econ.RemainingLandownerMinutes != econ.Options.AnnualLandownerAllowanceMinutes)
            throw new P1Refusal("SaveInconsistent", "economy used + remaining landowner minutes must equal annual allowance");
        if (econ.ExecutedHarvestPlansCount < 0)
            throw new P1Refusal("SaveInconsistent", "economy executedHarvestPlansCount cannot be negative");

        int harvestSettlementCount = 0;
        long totalRetainedFromSettlements = 0L;
        var expectedLedgerCount = 0;
        checked
        {
            foreach (var s in econ.Settlements)
            {
                if (s.CashBeforeCents < 0L || s.CashAfterCents < 0L || s.TotalExternalCostCents < 0L || s.TimberSaleRevenueCents < 0L)
                    throw new P1Refusal("SaveInconsistent", "settlement contains negative non-delta monetary amount");
                if (s.TimberSaleRevenueCents - s.TotalExternalCostCents != s.NetCashDeltaCents)
                    throw new P1Refusal("SaveInconsistent", "settlement netCashDeltaCents does not equal revenue minus external cost");
                if (s.CashBeforeCents + s.NetCashDeltaCents != s.CashAfterCents)
                    throw new P1Refusal("SaveInconsistent", "settlement cashAfterCents does not equal cashBefore + net");
                long sum = 0L;
                foreach (var e in s.LedgerEntries) sum += e.CashDeltaCents;
                if (sum != s.NetCashDeltaCents)
                    throw new P1Refusal("SaveInconsistent", "settlement ledgerEntries sum does not reconcile with netCashDeltaCents");
                if (s.HarvestJob is { } hj)
                {
                    harvestSettlementCount++;
                    long conserved = hj.SaleVolumeCm3 + hj.ResidualVolumeCm3 + hj.RetainedStemVolumeCm3 + hj.DeadwoodStemVolumeCm3;
                    if (conserved != hj.TotalStemVolumeCm3)
                        throw new P1Refusal("SaveInconsistent", "settlement harvestJob violates stem volume conservation");
                }
                totalRetainedFromSettlements += s.RetainedTimberDelta.RetainedTimberCm3;
                expectedLedgerCount += s.LedgerEntries.Count;
            }

            if (econ.ExecutedHarvestPlansCount != harvestSettlementCount)
                throw new P1Refusal("SaveInconsistent", "economy executedHarvestPlansCount does not match settled harvest plans");
            if (econ.RetainedTimber.RetainedTimberCm3 != totalRetainedFromSettlements)
                throw new P1Refusal("SaveInconsistent", "economy cumulative retainedTimber does not match settled deltas");
            if (econ.Ledger.Count != expectedLedgerCount)
                throw new P1Refusal("SaveInconsistent", "economy ledger count does not match settled ledger entries");

            long ledgerSum = 0L;
            foreach (var e in econ.Ledger) ledgerSum += e.CashDeltaCents;
            if (econ.Options.OpeningCashCents + ledgerSum != econ.CashBalanceCents)
                throw new P1Refusal("SaveInconsistent", "economy cashBalanceCents does not reconcile with openingCash + sum(ledger)");
        }
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static Dictionary<string, JNode> CanonicalKeys(JNode node, string what, string[] expectedKeys)
    {
        var map = J.Keys(node, what, expectedKeys);
        var obj = (JObj)node;
        for (int i = 0; i < expectedKeys.Length; i++)
        {
            if (!string.Equals(obj.Props[i].Key, expectedKeys[i], StringComparison.Ordinal))
                throw new P1Refusal("SaveNotCanonical", what + " keys are not in canonical order");
        }
        return map;
    }

    private static int ReadI32(JNode n, string what)
    {
        long v = J.Int(n, what);
        if (v < int.MinValue || v > int.MaxValue)
            throw new P1Refusal("NumberOutOfRange", what + " is outside the 32-bit integer range");
        return (int)v;
    }

    private static bool ReadBool(JNode n, string what) =>
        n is JBool b ? b.Value : throw new P1Refusal("SchemaInvalid", what + " must be true or false");

    private static long ReadI64(JNode n, string what)
    {
        var o = CanonicalKeys(n, what, new[] { "k", "b" });
        if (!string.Equals(J.Str(o["k"], what + ".k"), Phase1SaveFormat.Tags.I64, StringComparison.Ordinal))
            throw new P1Refusal("SaveTagInvalid", what + ".k must be \"" + Phase1SaveFormat.Tags.I64 + "\"");
        string hex = J.Str(o["b"], what + ".b");
        if (hex.Length != 16 || !hex.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'))
            throw new P1Refusal("FloatBitsInvalid", what + " bits must be exactly 16 lowercase hex digits");
        return unchecked((long)ulong.Parse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
    }

    private static TEnum ReadEnum<TEnum>(JNode n, string what) where TEnum : struct, Enum
    {
        string raw = J.Str(n, what);
        if (!Enum.TryParse<TEnum>(raw, ignoreCase: false, out var parsed) || !Enum.IsDefined(parsed))
            throw new P1Refusal("SaveEnumUnknown", $"unknown {typeof(TEnum).Name} value '{raw}' at {what}");
        return parsed;
    }

    private static IReadOnlyList<T> ReadList<T>(JNode n, string what, Func<JNode, T> read) =>
        Array.AsReadOnly(J.Arr(n, what).Items.Select(read).ToArray());
}

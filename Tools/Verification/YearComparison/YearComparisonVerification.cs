using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Readout = ScenarioOneManagementFollowonReadout;
using Mapper = ScenarioOneYearComparisonText;

// Offline fixture for the D-073 year-comparison text mapper (issue #415, task T-03).
// It compiles the REAL mapper and the REAL read-only helper against small test doubles of the history types, builds
// histories, runs them through the helper's Build and then through the mapper, and compares the card with exact text.
// No Unity, no scene, no save file. A PASS here says nothing about rendering, layout or Unity compilation.
//
// The first argument selects one group of checks (used by run_mutations.py); with no argument every group runs.
public static class YearComparisonVerification
{
    private static int checks;
    // Every card any check produced. The wording and label checks at the end read all of them.
    private static readonly List<Mapper.Card> seen = new List<Mapper.Card>();

    private static void Check(bool condition, string label)
    {
        checks++;
        if (!condition) throw new InvalidOperationException("FAILED " + label);
    }

    private static void Same(string actual, string expected, string label)
    {
        checks++;
        if (actual != expected)
            throw new InvalidOperationException("FAILED " + label + "\n  expected: " + expected + "\n  actual:   " + actual);
    }

    // ---------- history builders (test doubles from ManagementFollowonReadout/OfflineContractTypes.cs) ----------

    private static YearHistoryView Year(int year, ScenarioEcologicalSnapshot snapshot = null)
        => new YearHistoryView { Year = year, YearAdvancedRecorded = true, Snapshot = snapshot };

    private static ScenarioEcologicalSnapshot Snapshot(int year, int trees, float basal, float canopy, int regeneration, float deadwood)
        => new ScenarioEcologicalSnapshot
        {
            year = year, livingTrees = trees, basalAreaM2PerHa = basal, meanDbhCm = 21f, meanCanopy = canopy,
            meanLight = 0.35f, occupiedRegenerationCells = regeneration, deadwoodCount = 4, deadwoodVolumeM3 = deadwood
        };

    private static ScenarioSpeciesOutcome Species(string id, int living, int naturalCells, int plantedCells, int plantedTrees)
        => new ScenarioSpeciesOutcome
        {
            speciesId = id, livingTrees = living, basalAreaM2PerHa = 9f, regenerationCells = naturalCells,
            regenerationDensity = 0.5f, plantedRegenerationCells = plantedCells, plantedJuveniles = plantedTrees
        };

    private static ScenarioManagementEvent Event(int id, int orderId, int year, ScenarioWorkType type,
        ScenarioManagementOutcome outcome = ScenarioManagementOutcome.Succeeded)
        => new ScenarioManagementEvent
        {
            eventId = id, workOrderId = orderId, year = year, eventType = ScenarioManagementEventType.WorkResolved,
            taskType = type, outcome = outcome, prescriptionId = 8, targetTreeId = "T" + orderId, speciesId = "sitka-spruce"
        };

    private static ScenarioOneWorkOrder Order(int id, int year, ScenarioWorkType type, ScenarioManagementOutcome outcome)
        => new ScenarioOneWorkOrder
        {
            workOrderId = id, resolvedYear = year, type = type, prescriptionId = 8, prescriptionCoverageUnitId = "U-ALPHA",
            rackId = 0, targetTreeId = "T" + id, speciesId = "sitka-spruce",
            status = outcome == ScenarioManagementOutcome.Failed ? ScenarioWorkStatus.Failed : ScenarioWorkStatus.Completed
        };

    private static void Add(YearHistoryView year, ScenarioManagementEvent e, ScenarioOneWorkOrder order)
    {
        year.Events.Add(e);
        year.ResolvedWork.Add(new ResolvedWorkRecord { Event = e, Order = order, OrderStillExists = order != null });
    }

    // One resolved task whose event and retained order agree in every field, so the helper links it as Verified.
    private static void Work(YearHistoryView year, int eventId, ScenarioWorkType type,
        ScenarioManagementOutcome outcome = ScenarioManagementOutcome.Succeeded)
        => Add(year, Event(eventId, eventId + 1000, year.Year, type, outcome), Order(eventId + 1000, year.Year, type, outcome));

    private static void Cancel(YearHistoryView year, int eventId)
    {
        var e = new ScenarioManagementEvent
        {
            eventId = eventId, workOrderId = eventId + 1000, year = year.Year,
            eventType = ScenarioManagementEventType.OrderCancelled, taskType = ScenarioWorkType.FellTree,
            outcome = ScenarioManagementOutcome.Cancelled
        };
        year.Events.Add(e);
        year.Cancellations.Add(new HistoryCancellation { Event = e, WorkOrderId = e.workOrderId, Year = year.Year });
    }

    // Mirrors the fallback names of ScenarioOneUiFacts.SpeciesName (two of them start with a small letter).
    private static string Name(string id)
    {
        switch (id)
        {
            case "sitka-spruce": return "Sitka spruce";
            case "sessile-oak": return "sessile oak";
            case "beech": return "beech";
            default: return "Unknown species";
        }
    }

    private static Mapper.Card Show(int viewedYear, params YearHistoryView[] years)
        => Show(Readout.Build(years, viewedYear), viewedYear);

    private static Mapper.Card Show(Readout.Result result, int viewedYear)
    {
        Mapper.Card card = Mapper.Describe(result, viewedYear, Name);
        seen.Add(card);
        return card;
    }

    // ---------- card readers ----------

    private static Mapper.Row Row(Mapper.Card card, string key)
    {
        List<Mapper.Row> rows = card.Rows.Where(r => r.Key == key).ToList();
        if (rows.Count != 1) throw new InvalidOperationException("FAILED expected exactly one row '" + key + "', found " + rows.Count);
        return rows[0];
    }

    private static bool Has(Mapper.Card card, string key) => card.Rows.Any(r => r.Key == key);
    private static string Keys(Mapper.Card card) => string.Join(",", card.Rows.Select(r => r.Key));
    private static int Count(Mapper.Card card, Mapper.RowKind kind) => card.Rows.Count(r => r.Kind == kind);

    private static string Tag(HistoryProvenance? kind)
    {
        if (!kind.HasValue) return "";
        switch (kind.Value)
        {
            case HistoryProvenance.Recorded: return "[RECORDED] ";
            case HistoryProvenance.Derived: return "[DERIVED] ";
            case HistoryProvenance.NotRecorded: return "[NOT RECORDED] ";
            default: return "[" + kind.Value + "] ";
        }
    }

    // The card as tagged plain lines, the way the spec's layout sketch writes it.
    private static string Render(Mapper.Card card)
        => string.Join("\n", card.Rows.Select(r => Tag(r.Provenance) + r.Text));

    // Every field of every row, for "same input gives same output".
    private static string Dump(Mapper.Card card)
    {
        var text = new StringBuilder();
        text.Append(card.State).Append('|').Append(card.Visible).Append('|').Append(card.ViewedYear).Append('|').Append(card.LatestYear);
        foreach (Mapper.Row r in card.Rows)
            text.Append('\n').Append(string.Join("|", r.Kind, r.Key, r.Provenance, r.Text, r.Muted, r.Label, r.SelectedValue,
                r.LatestValue, r.Difference, r.ValueProvenance, r.DifferenceProvenance, r.SpeciesId, r.SpeciesName,
                r.FirstRowOfSpecies, r.MissingYear));
        return text.ToString();
    }

    // Every field of a helper result, for "the mapper does not change its input".
    private static string Fingerprint(Readout.Result r)
    {
        var text = new StringBuilder();
        text.Append(string.Join("|", r.SelectedYear, r.LatestCompletedYear, r.SelectedState, r.LatestState, r.ComparisonState));
        foreach (Readout.StandObservation o in new[] { r.Selected, r.Latest })
        {
            if (o == null) { text.Append("\nnull"); continue; }
            text.Append('\n').Append(string.Join("|", o.Year, o.LivingTrees, o.BasalAreaM2PerHa, o.MeanDbhCm, o.MeanCanopy, o.MeanLight,
                o.OccupiedRegenerationCells, o.DeadwoodCount, o.DeadwoodVolumeM3, o.SpeciesIdsUnique));
            foreach (Readout.SpeciesObservation s in o.Species)
                text.Append('\n').Append(string.Join("|", s.SpeciesId, s.LivingTrees, s.BasalAreaM2PerHa, s.NaturalRegenerationCells,
                    s.NaturalRegenerationDensity, s.PlantedRegenerationCells, s.ExactPlantedJuveniles));
        }
        if (r.Comparison == null) text.Append("\nnull");
        else
        {
            Readout.StandComparison c = r.Comparison;
            text.Append('\n').Append(string.Join("|", c.FromYear, c.ToYear, c.YearsBetween, c.LivingTreesChange,
                c.OccupiedRegenerationCellsChange, c.DeadwoodCountChange, c.BasalAreaM2PerHaChange, c.MeanDbhCmChange,
                c.MeanCanopyChange, c.MeanLightChange, c.DeadwoodVolumeM3Change));
            foreach (Readout.SpeciesComparison s in c.Species)
                text.Append('\n').Append(string.Join("|", s.SpeciesId, s.State, s.NaturalRegenerationCellsChange,
                    s.PlantedRegenerationCellsChange, s.ExactPlantedJuvenilesChange, s.NaturalRegenerationDensityChange,
                    s.LivingTreesChange, s.BasalAreaM2PerHaChange));
        }
        foreach (Readout.RecordedWork w in r.RecordedWork)
            text.Append('\n').Append(string.Join("|", w.Year, w.EventId, w.WorkOrderId, w.TaskType, w.Result, w.TargetTreeId,
                w.SpeciesId, w.CoverageUnitId, w.PrescriptionId, w.RackId, w.Link, w.Felling, w.GroupedHarvestMoney, w.FailureReason));
        foreach (Readout.ExecutedCoverageGroup g in r.ExecutedCoverage)
            text.Append('\n').Append(string.Join("|", g.ResolutionYear, g.PrescriptionId, g.CoverageUnitId,
                string.Join(",", g.EventIds), string.Join(",", g.WorkOrderIds)));
        foreach (Readout.PossibleTreatmentYear p in r.PossibleTreatmentYears)
            text.Append('\n').Append(string.Join("|", p.Year, p.CandidateFellEvents, p.RackOwnedFellEvents, p.UnclassifiedFellEvents));
        return text.ToString();
    }

    // ---------- the shared "clean" history: three completed years, work in each, complete and consistent records ----------

    private static YearHistoryView[] CleanHistory()
    {
        var year3 = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        year3.Snapshot.species.Add(Species("sitka-spruce", 90, 4, 1, 10));
        year3.Snapshot.species.Add(Species("sessile-oak", 30, 2, 0, 5));
        // Report and settlement present, so the helper sets GroupedHarvestMoney on the fell and salvage rows.
        year3.Report = new ScenarioAnnualReport();
        year3.HarvestSettlement = new GroupedHarvestSettlement();
        Work(year3, 301, ScenarioWorkType.FellTree);
        Work(year3, 302, ScenarioWorkType.FellTree);
        Work(year3, 303, ScenarioWorkType.PlantJuvenile);
        Work(year3, 304, ScenarioWorkType.PruneTree);
        Work(year3, 305, ScenarioWorkType.SalvageDeadwood);
        Work(year3, 306, ScenarioWorkType.RemoveRegeneration);
        Work(year3, 307, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Failed);
        Cancel(year3, 308);

        // A middle year with very different numbers: none of them may ever appear on the Year 3 card.
        var year5 = Year(5, Snapshot(5, 999, 77.7f, 0.99f, 888, 55.55f));
        year5.Snapshot.species.Add(Species("sitka-spruce", 777, 66, 55, 44));
        Work(year5, 501, ScenarioWorkType.FellTree);
        Work(year5, 502, ScenarioWorkType.FellTree);
        Work(year5, 503, ScenarioWorkType.FellTree);
        Work(year5, 504, ScenarioWorkType.PlantJuvenile, ScenarioManagementOutcome.Failed);
        Cancel(year5, 505);

        var year9 = Year(9, Snapshot(9, 112, 16.7f, 0.66f, 12, 2.25f));
        year9.Snapshot.species.Add(Species("sitka-spruce", 82, 6, 3, 8));
        year9.Snapshot.species.Add(Species("sessile-oak", 30, 2, 0, 5));
        Work(year9, 901, ScenarioWorkType.FellTree);
        return new[] { year3, year5, year9 };
    }

    private const string CleanYear3Card =
        "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
        + "[DERIVED] Recorded outcomes in Year 3: 2 tree(s) felled · 1 planted · 1 crop tree(s) pruned · 1 windthrow stem(s) salvaged · 1 young-tree removal job(s)\n"
        + "[DERIVED] 1 task(s) failed · 1 order(s) cancelled\n"
        + "[RECORDED] Stand recorded at the end of Year 3 and at the end of Year 9, 6 year(s) later.\n"
        + "End of Year 3 · End of Year 9 · Difference\n"
        + "[DERIVED] Standing trees: 120 → 112 (-8)\n"
        + "[DERIVED] Basal area: 14.2 m²/ha → 16.7 m²/ha (+2.5 m²/ha)\n"
        + "[DERIVED] Mean canopy cover: 62 % → 66 % (+4.0 points)\n"
        + "[DERIVED] Regeneration map squares (modelled): 7 → 12 (+5)\n"
        + "[DERIVED] Fallen deadwood: 3.50 m³ → 2.25 m³ (-1.25 m³)\n"
        + "By species\n"
        + "[DERIVED] Sessile oak: standing trees 30 → 30 (0)\n"
        + "[DERIVED] natural regeneration map squares (modelled) 2 → 2 (0)\n"
        + "[DERIVED] planted regeneration map squares (modelled) 0 → 0 (0)\n"
        + "[DERIVED] planted young trees (counted) 5 → 5 (0)\n"
        + "[DERIVED] Sitka spruce: standing trees 90 → 82 (-8)\n"
        + "[DERIVED] natural regeneration map squares (modelled) 4 → 6 (+2)\n"
        + "[DERIVED] planted regeneration map squares (modelled) 1 → 3 (+2)\n"
        + "[DERIVED] planted young trees (counted) 10 → 8 (-2)\n"
        + "These are recorded stand totals for two years. They do not show that Year 3's work caused the difference.\n"
        + "For what to inspect before deciding again, see the prior-treatment guidance in this review and walk the stand.";

    private const string ZeroOutcomesYear3 =
        "Recorded outcomes in Year 3: 0 tree(s) felled · 0 planted · 0 crop tree(s) pruned · 0 windthrow stem(s) salvaged · 0 young-tree removal job(s)";

    private static bool Run(string selection, string group) => selection == "all" || selection == group;

    public static int Main(string[] args)
    {
        string selection = args.Length > 0 ? args[0] : "all";
        try
        {
            if (Run(selection, "nocard")) NoCard();
            if (Run(selection, "latest")) LatestYear();
            if (Run(selection, "full")) FullComparison();
            if (Run(selection, "missing")) MissingSnapshots();
            if (Run(selection, "precedence")) CannotComparePrecedence();
            if (Run(selection, "nowork")) NoWorkRows();
            if (Run(selection, "yearfilter")) YearFilter();
            if (Run(selection, "ambiguous")) AmbiguousAndUnresolved();
            if (Run(selection, "linkage")) OrderLinkage();
            if (Run(selection, "species")) SpeciesRows();
            if (Run(selection, "units")) UnitsAndSigns();
            if (Run(selection, "stable")) StableAndReadOnly();
            // Last, so they read every card produced above.
            if (Run(selection, "wording")) WordingAndLabels();
        }
        catch (InvalidOperationException e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }
        if (checks == 0)
        {
            Console.Error.WriteLine("FAILED unknown group '" + selection + "': no assertion ran");
            return 2;
        }
        Console.WriteLine("YearComparison: PASS " + checks + " assertions (group: " + selection + ", cards checked: " + seen.Count + ").");
        return 0;
    }

    // FR-9 row 1 / AC-1.
    private static void NoCard()
    {
        Mapper.Card none = Show(3);
        Check(!none.Visible && none.State == Mapper.CardState.NoCompletedYear && none.Rows.Count == 0,
            "no completed year shows no card");

        // A year that was never completed (no report, no YearAdvanced record) is not a completed year.
        var open = new YearHistoryView { Year = 3, Snapshot = Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f) };
        Mapper.Card notCompleted = Show(3, open);
        Check(!notCompleted.Visible && notCompleted.Rows.Count == 0, "an uncompleted year shows no card");

        Mapper.Card missing = Mapper.Describe(null, 3, Name);
        Check(!missing.Visible && missing.State == Mapper.CardState.InvalidInput && missing.Rows.Count == 0,
            "no result shows no card");

        // A result built for Year 5 must never be shown under Year 3.
        Mapper.Card wrongYear = Show(Readout.Build(CleanHistory(), 5), 3);
        Check(!wrongYear.Visible && wrongYear.State == Mapper.CardState.InvalidInput && wrongYear.Rows.Count == 0,
            "a result built for another year shows no card");
    }

    // FR-9 row 2 / AC-2 (the card part; the acknowledgement footer needs Unity).
    private static void LatestYear()
    {
        Mapper.Card card = Show(9, CleanHistory());
        Check(card.Visible && card.State == Mapper.CardState.LatestYear && card.Rows.Count == 1, "latest year is a single row");
        Same(card.Rows[0].Text, "Year 9 is the latest completed year. Choose an earlier year to compare it with Year 9.",
            "latest year line exact wording");
        Check(card.Rows[0].Muted && card.Rows[0].Kind == Mapper.RowKind.Line && !card.Rows[0].Provenance.HasValue,
            "latest year line is muted and carries no label");
        Check(Count(card, Mapper.RowKind.StandMeasure) == 0 && Count(card, Mapper.RowKind.TableHeader) == 0
            && Count(card, Mapper.RowKind.Caveat) == 0 && !Has(card, "work-outcomes"), "latest year has no table, work lines or caveat");

        // Year 9 has work and a snapshot, and Year 9 appears twice. The latest-year line still comes first.
        YearHistoryView[] years = CleanHistory();
        Mapper.Card duplicated = Show(9, years[0], years[2], Year(9, Snapshot(9, 500, 50f, 0.5f, 50, 5f)));
        Same(Render(duplicated), "Year 9 is the latest completed year. Choose an earlier year to compare it with Year 9.",
            "latest year line wins over duplicate latest-year records");

        // Only one completed year: it is the latest, so there is nothing earlier to compare.
        Mapper.Card single = Show(3, CleanHistory()[0]);
        Same(Render(single), "Year 3 is the latest completed year. Choose an earlier year to compare it with Year 3.",
            "a single completed year is the latest year");
    }

    // FR-4, FR-6, FR-7, FR-8, FR-10, FR-11 / AC-3, AC-6, AC-12 (clean side).
    private static void FullComparison()
    {
        YearHistoryView[] years = CleanHistory();
        Mapper.Card card = Show(3, years);
        Check(card.Visible && card.State == Mapper.CardState.Compared && card.ViewedYear == 3 && card.LatestYear == 9,
            "earlier year with both snapshots is a comparison");
        Same(Render(card), CleanYear3Card, "full comparison card exact text and order");
        Same(Keys(card), "heading,work-outcomes,work-failed-cancelled,snapshots,table-header,stand-living-trees,stand-basal-area,"
            + "stand-canopy,stand-regeneration,stand-deadwood,species-heading,species-sessile-oak-standing-trees,"
            + "species-sessile-oak-natural-regeneration,species-sessile-oak-planted-regeneration,species-sessile-oak-planted-young-trees,"
            + "species-sitka-spruce-standing-trees,species-sitka-spruce-natural-regeneration,species-sitka-spruce-planted-regeneration,"
            + "species-sitka-spruce-planted-young-trees,caveat,guidance-pointer", "full comparison row keys in order");

        Mapper.Row header = Row(card, "table-header");
        Check(header.Kind == Mapper.RowKind.TableHeader && header.SelectedValue == "End of Year 3" && header.LatestValue == "End of Year 9"
            && header.Difference == "Difference" && header.ValueProvenance == HistoryProvenance.Recorded
            && header.DifferenceProvenance == HistoryProvenance.Derived, "table header names both years and labels the difference DERIVED");

        Mapper.Row trees = Row(card, "stand-living-trees");
        Check(trees.Label == "Standing trees" && trees.SelectedValue == "120" && trees.LatestValue == "112" && trees.Difference == "-8",
            "standing trees columns with a negative difference");
        Mapper.Row basal = Row(card, "stand-basal-area");
        Check(basal.Label == "Basal area" && basal.SelectedValue == "14.2 m²/ha" && basal.LatestValue == "16.7 m²/ha"
            && basal.Difference == "+2.5 m²/ha", "basal area columns keep m²/ha");
        Mapper.Row canopy = Row(card, "stand-canopy");
        Check(canopy.Label == "Mean canopy cover" && canopy.SelectedValue == "62 %" && canopy.LatestValue == "66 %"
            && canopy.Difference == "+4.0 points", "canopy columns are percent and points");
        Mapper.Row regeneration = Row(card, "stand-regeneration");
        Check(regeneration.Label == "Regeneration map squares (modelled)" && regeneration.SelectedValue == "7"
            && regeneration.LatestValue == "12" && regeneration.Difference == "+5", "regeneration columns are modelled map squares");
        Mapper.Row deadwood = Row(card, "stand-deadwood");
        Check(deadwood.Label == "Fallen deadwood" && deadwood.SelectedValue == "3.50 m³" && deadwood.LatestValue == "2.25 m³"
            && deadwood.Difference == "-1.25 m³", "deadwood columns keep m³ with a negative difference");
        Check(card.Rows.Where(r => r.Kind == Mapper.RowKind.StandMeasure).All(r => r.ValueProvenance == HistoryProvenance.Recorded
            && r.DifferenceProvenance == HistoryProvenance.Derived && r.Provenance == HistoryProvenance.Derived),
            "stand values are RECORDED and differences DERIVED");
        Check(Count(card, Mapper.RowKind.StandMeasure) == 5, "exactly five stand measures");

        Check(Row(card, "caveat").Kind == Mapper.RowKind.Caveat && Row(card, "caveat").Muted
            && Row(card, "guidance-pointer").Kind == Mapper.RowKind.GuidancePointer
            && card.Rows[card.Rows.Count - 2].Key == "caveat" && card.Rows[card.Rows.Count - 1].Key == "guidance-pointer",
            "the card ends with the caveat and the guidance pointer");

        // AC-12, clean side: on complete, consistent records the card's event counts equal the counts the existing
        // Work column makes from the same events (succeeded by task type; not succeeded = failed; cancellations).
        List<ResolvedWorkRecord> done = years[0].ResolvedWork.Where(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded).ToList();
        string workColumn = "Recorded outcomes in Year 3: " + done.Count(r => r.Event.taskType == ScenarioWorkType.FellTree) + " tree(s) felled · "
            + done.Count(r => r.Event.taskType == ScenarioWorkType.PlantJuvenile) + " planted · "
            + done.Count(r => r.Event.taskType == ScenarioWorkType.PruneTree) + " crop tree(s) pruned · "
            + done.Count(r => r.Event.taskType == ScenarioWorkType.SalvageDeadwood) + " windthrow stem(s) salvaged · "
            + done.Count(r => r.Event.taskType == ScenarioWorkType.RemoveRegeneration) + " young-tree removal job(s)";
        Same(Row(card, "work-outcomes").Text, workColumn, "clean history: card counts equal event counts made the Work-column way");
        Same(Row(card, "work-failed-cancelled").Text,
            years[0].ResolvedWork.Count(r => r.Event.outcome != ScenarioManagementOutcome.Succeeded) + " task(s) failed · "
            + years[0].Cancellations.Count + " order(s) cancelled", "clean history: failed and cancelled equal the Work-column counts");
        Check(!Has(card, "work-ambiguous") && !Has(card, "work-unresolved") && !Has(card, "work-event-only"),
            "clean history has no provenance warning");

        // A task type the card has no name for (not one of the five known types) is still counted, never dropped.
        var unnamed = new Readout.Result { SelectedYear = 3, LatestCompletedYear = 9, ComparisonState = Readout.ObservationState.NotRecorded };
        unnamed.RecordedWork.Add(new Readout.RecordedWork
        {
            Year = 3, EventId = 395, WorkOrderId = 1395, TaskType = (ScenarioWorkType)99,
            Result = Readout.Resolution.Succeeded, Link = Readout.OrderLink.Verified
        });
        Same(Row(Show(unnamed, 3), "work-outcomes").Text, ZeroOutcomesYear3 + " · 1 other task(s)",
            "a task type without a name is counted as other");
    }

    // FR-6, FR-9 rows 3 and 4 / AC-4.
    private static void MissingSnapshots()
    {
        YearHistoryView[] clean = CleanHistory();

        // Year 3 completed without a snapshot. Year 5 and Year 9 have snapshots; neither may stand in for Year 3.
        var bare3 = Year(3);
        Work(bare3, 301, ScenarioWorkType.FellTree);
        Mapper.Card selectedMissing = Show(3, bare3, clean[1], clean[2]);
        Check(selectedMissing.State == Mapper.CardState.SelectedSnapshotMissing, "selected snapshot missing state");
        Same(Render(selectedMissing),
            "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
            + "[DERIVED] Recorded outcomes in Year 3: 1 tree(s) felled · 0 planted · 0 crop tree(s) pruned · 0 windthrow stem(s) salvaged · 0 young-tree removal job(s)\n"
            + "[DERIVED] 0 task(s) failed · 0 order(s) cancelled\n"
            + "[NOT RECORDED] No stand snapshot was recorded for Year 3. Nothing is compared.",
            "selected snapshot missing: work lines then the exact line, and nothing else");
        Check(Count(selectedMissing, Mapper.RowKind.StandMeasure) == 0 && Count(selectedMissing, Mapper.RowKind.TableHeader) == 0
            && Count(selectedMissing, Mapper.RowKind.SpeciesMeasure) == 0 && Count(selectedMissing, Mapper.RowKind.Caveat) == 0,
            "selected snapshot missing shows no stand number from another year");

        // A snapshot stored under Year 3 but dated Year 4 is not Year 3's record.
        var misdated3 = Year(3, Snapshot(4, 444, 44.4f, 0.44f, 44, 4.44f));
        Mapper.Card misdated = Show(3, misdated3, clean[2]);
        Same(Render(misdated),
            "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
            + "[DERIVED] No recorded work-resolution or cancellation events for Year 3.\n"
            + "[NOT RECORDED] No stand snapshot was recorded for Year 3. Nothing is compared.",
            "a snapshot dated for another year is not shown as Year 3");

        // Year 9 completed without a snapshot. Year 3's own record is still shown; Year 5 is never used as "latest".
        Mapper.Card latestMissing = Show(3, clean[0], clean[1], Year(9));
        Check(latestMissing.State == Mapper.CardState.LatestSnapshotMissing, "latest snapshot missing state");
        Same(Render(latestMissing),
            "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
            + "[DERIVED] Recorded outcomes in Year 3: 2 tree(s) felled · 1 planted · 1 crop tree(s) pruned · 1 windthrow stem(s) salvaged · 1 young-tree removal job(s)\n"
            + "[DERIVED] 1 task(s) failed · 1 order(s) cancelled\n"
            + "[RECORDED] Stand recorded at the end of Year 3.\n"
            + "End of Year 3\n"
            + "[RECORDED] Standing trees: 120\n"
            + "[RECORDED] Basal area: 14.2 m²/ha\n"
            + "[RECORDED] Mean canopy cover: 62 %\n"
            + "[RECORDED] Regeneration map squares (modelled): 7\n"
            + "[RECORDED] Fallen deadwood: 3.50 m³\n"
            + "[NOT RECORDED] No stand snapshot was recorded for the latest year, Year 9. Nothing is compared.",
            "latest snapshot missing: Year A observation then the exact line");
        Check(latestMissing.Rows.All(r => r.LatestValue == "" && r.Difference == "" && !r.DifferenceProvenance.HasValue),
            "latest snapshot missing shows no latest value and no difference");
        Check(Count(latestMissing, Mapper.RowKind.Caveat) == 0 && Count(latestMissing, Mapper.RowKind.SpeciesMeasure) == 0,
            "latest snapshot missing has no species rows and no two-year caveat");

        // Both snapshots missing: each missing record is named, the viewed year first.
        Mapper.Card bothMissing = Show(3, Year(3), Year(9));
        Check(bothMissing.State == Mapper.CardState.SelectedSnapshotMissing, "both snapshots missing reports the selected year first");
        Same(Render(bothMissing),
            "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
            + "[DERIVED] No recorded work-resolution or cancellation events for Year 3.\n"
            + "[NOT RECORDED] No stand snapshot was recorded for Year 3. Nothing is compared.\n"
            + "[NOT RECORDED] No stand snapshot was recorded for the latest year, Year 9. Nothing is compared.",
            "both snapshots missing: both exact lines in order");
    }

    // FR-9 row 5 and the precedence rule / AC-4.
    private static void CannotComparePrecedence()
    {
        YearHistoryView[] clean = CleanHistory();

        // Year 3 appears twice and one copy has no snapshot. The helper reports CannotCompare AND SelectedState NotRecorded.
        var copy = Year(3);
        Readout.Result duplicateYear = Readout.Build(new[] { clean[0], copy, clean[2] }, 3);
        Check(duplicateYear.ComparisonState == Readout.ObservationState.CannotCompare
            && duplicateYear.SelectedState == Readout.ObservationState.NotRecorded, "fixture: duplicate year is CannotCompare with a missing selected snapshot");
        Mapper.Card card = Show(duplicateYear, 3);
        Check(Has(card, "cannot-compare") && !Has(card, "missing-selected") && !Has(card, "missing-latest"),
            "CannotCompare beats a missing snapshot");
        Check(card.State == Mapper.CardState.CannotCompare, "CannotCompare state");
        Same(Render(card),
            "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
            + "[DERIVED] " + ZeroOutcomesYear3 + "\n"
            + "[DERIVED] 0 task(s) failed · 0 order(s) cancelled\n"
            + "[NOT RECORDED] 8 work record(s) have ambiguous provenance and are not counted.\n"
            + "[NOT RECORDED] The records for these years are ambiguous, so no comparison is shown.",
            "CannotCompare: work lines then the exact line");
        Check(Count(card, Mapper.RowKind.StandMeasure) == 0 && Count(card, Mapper.RowKind.TableHeader) == 0
            && Count(card, Mapper.RowKind.Caveat) == 0, "CannotCompare shows no stand number");

        // Both snapshots recorded, but Year 9 lists the same species twice.
        var twice = Year(9, Snapshot(9, 112, 16.7f, 0.66f, 12, 2.25f));
        twice.Snapshot.species.Add(Species("sitka-spruce", 82, 6, 3, 8));
        twice.Snapshot.species.Add(Species("sitka-spruce", 1, 1, 1, 1));
        Mapper.Card duplicateSpecies = Show(3, Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f)), twice);
        Same(Render(duplicateSpecies),
            "YEAR 3 COMPARED WITH LATEST YEAR 9\n"
            + "[DERIVED] No recorded work-resolution or cancellation events for Year 3.\n"
            + "[NOT RECORDED] The records for these years are ambiguous, so no comparison is shown.",
            "duplicate species identity: no observation and no difference");

        // Latest year appears twice and the viewed year has no snapshot: still the ambiguous line, not the missing line.
        Mapper.Card duplicateLatest = Show(3, Year(3), clean[2], Year(9, Snapshot(9, 500, 50f, 0.5f, 50, 5f)));
        Check(Has(duplicateLatest, "cannot-compare") && !Has(duplicateLatest, "missing-selected") && !Has(duplicateLatest, "missing-latest")
            && Count(duplicateLatest, Mapper.RowKind.StandMeasure) == 0, "duplicate latest year beats a missing selected snapshot");

        // A hand-made result that claims both snapshots but carries no comparison is shown as not comparable, never as numbers.
        var broken = new Readout.Result
        {
            SelectedYear = 3, LatestCompletedYear = 9,
            SelectedState = Readout.ObservationState.Recorded, LatestState = Readout.ObservationState.Recorded,
            ComparisonState = Readout.ObservationState.Recorded,
            Selected = new Readout.StandObservation { Year = 3, LivingTrees = 120 },
            Latest = new Readout.StandObservation { Year = 9, LivingTrees = 112 }
        };
        Mapper.Card inconsistent = Show(broken, 3);
        Check(inconsistent.State == Mapper.CardState.CannotCompare && Has(inconsistent, "cannot-compare")
            && Count(inconsistent, Mapper.RowKind.StandMeasure) == 0, "a result without a usable comparison shows no numbers");
    }

    // FR-9 row 6: the notice is supplementary, so the stand comparison is still shown beside it.
    private static void NoWorkRows()
    {
        YearHistoryView[] clean = CleanHistory();
        var quiet3 = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Mapper.Card card = Show(3, quiet3, clean[1], clean[2]);
        Same(Row(card, "work-none").Text, "No recorded work-resolution or cancellation events for Year 3.", "no work rows exact line");
        Check(Row(card, "work-none").Provenance == HistoryProvenance.Derived, "no work rows line is DERIVED");
        Check(!Has(card, "work-outcomes") && !Has(card, "work-failed-cancelled"), "no work rows: no count lines");
        Check(card.State == Mapper.CardState.Compared && Count(card, Mapper.RowKind.StandMeasure) == 5 && Has(card, "caveat"),
            "no work rows is supplementary: the stand comparison is still shown");
        Same(card.Rows[1].Key, "work-none", "no work rows line sits where the work lines go");
    }

    // FR-4: RecordedWork holds every completed year; only the viewed year is counted.
    private static void YearFilter()
    {
        YearHistoryView[] years = CleanHistory();
        Readout.Result result = Readout.Build(years, 3);
        Check(result.RecordedWork.Count(w => w.Year == 3) == 8 && result.RecordedWork.Count(w => w.Year == 5) == 5
            && result.RecordedWork.Count(w => w.Year == 9) == 1, "fixture: the result holds work for three years");
        Mapper.Card year3 = Show(result, 3);
        Same(Row(year3, "work-outcomes").Text,
            "Recorded outcomes in Year 3: 2 tree(s) felled · 1 planted · 1 crop tree(s) pruned · 1 windthrow stem(s) salvaged · 1 young-tree removal job(s)",
            "only viewed-year rows are counted");
        Same(Row(year3, "work-failed-cancelled").Text, "1 task(s) failed · 1 order(s) cancelled", "only viewed-year failures and cancellations are counted");

        Mapper.Card year5 = Show(5, years);
        Same(Row(year5, "work-outcomes").Text,
            "Recorded outcomes in Year 5: 3 tree(s) felled · 0 planted · 0 crop tree(s) pruned · 0 windthrow stem(s) salvaged · 0 young-tree removal job(s)",
            "Year 5 counts only Year 5 rows");
        Same(Row(year5, "work-failed-cancelled").Text, "1 task(s) failed · 1 order(s) cancelled", "Year 5 failures and cancellations");
        Same(Row(year5, "heading").Text, "YEAR 5 COMPARED WITH LATEST YEAR 9", "Year 5 heading names both years");
        Same(Row(year5, "snapshots").Text, "Stand recorded at the end of Year 5 and at the end of Year 9, 4 year(s) later.", "Year 5 elapsed years");

        // Orders that were only created or approved (pending, approved, proposed work) are not resolution events.
        years = CleanHistory();
        years[0].Events.Add(new ScenarioManagementEvent { eventId = 390, workOrderId = 1390, year = 3, eventType = ScenarioManagementEventType.OrderCreated, taskType = ScenarioWorkType.FellTree });
        years[0].Events.Add(new ScenarioManagementEvent { eventId = 391, workOrderId = 1390, year = 3, eventType = ScenarioManagementEventType.OrderApproved, taskType = ScenarioWorkType.FellTree });
        Mapper.Card withPending = Show(3, years);
        Same(Render(withPending), CleanYear3Card, "created and approved orders never appear on the card");
    }

    // FR-5 / AC-5: ambiguous and unresolved rows.
    private static void AmbiguousAndUnresolved()
    {
        YearHistoryView latest = CleanHistory()[2];

        // Ambiguous only: two succeeded fells share one event id.
        var ambiguousOnly = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Add(ambiguousOnly, Event(310, 1310, 3, ScenarioWorkType.FellTree), Order(1310, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Succeeded));
        Add(ambiguousOnly, Event(310, 1311, 3, ScenarioWorkType.FellTree), Order(1311, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Succeeded));
        Mapper.Card card = Show(3, ambiguousOnly, latest);
        Same(Row(card, "work-outcomes").Text, ZeroOutcomesYear3, "ambiguous rows are excluded from every count");
        Same(Row(card, "work-failed-cancelled").Text, "0 task(s) failed · 0 order(s) cancelled", "ambiguous rows are not failed or cancelled either");
        Same(Row(card, "work-ambiguous").Text, "2 work record(s) have ambiguous provenance and are not counted.", "ambiguous exact sentence");
        Check(Row(card, "work-ambiguous").Provenance == HistoryProvenance.NotRecorded, "ambiguous warning is NOT RECORDED");
        Check(!Has(card, "work-unresolved") && !Has(card, "work-event-only"), "ambiguous only: no other warning");
        Check(card.State == Mapper.CardState.Compared, "ambiguous work does not hide a recorded stand comparison");

        // AC-12, malformed side: the Work column would count these two events as felled; the card does not, and says why.
        int workColumnFelled = ambiguousOnly.ResolvedWork.Count(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded
            && r.Event.taskType == ScenarioWorkType.FellTree);
        Check(workColumnFelled == 2 && Row(card, "work-outcomes").Text.Contains("0 tree(s) felled") && Has(card, "work-ambiguous"),
            "malformed history: the mismatch with event counts is explained by the warning");

        // Unresolved only: a resolution event with no outcome.
        var unresolvedOnly = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Add(unresolvedOnly, Event(320, 1320, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.None),
            Order(1320, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Succeeded));
        card = Show(3, unresolvedOnly, latest);
        Same(Row(card, "work-outcomes").Text, ZeroOutcomesYear3, "unresolved rows are excluded from every count");
        Same(Row(card, "work-unresolved").Text, "1 work record(s) have no recorded outcome.", "unresolved exact sentence");
        Check(Row(card, "work-unresolved").Provenance == HistoryProvenance.NotRecorded, "unresolved warning is NOT RECORDED");
        Check(!Has(card, "work-ambiguous") && !Has(card, "work-event-only"), "unresolved only: no other warning");

        // Both conditions on one row: event 330 is used twice and one use has no outcome. A third row is unresolved only.
        var mixed = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Add(mixed, Event(330, 1330, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.None),
            Order(1330, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Succeeded));
        Add(mixed, Event(330, 1331, 3, ScenarioWorkType.PruneTree), Order(1331, 3, ScenarioWorkType.PruneTree, ScenarioManagementOutcome.Succeeded));
        Add(mixed, Event(332, 1332, 3, ScenarioWorkType.PlantJuvenile, ScenarioManagementOutcome.None),
            Order(1332, 3, ScenarioWorkType.PlantJuvenile, ScenarioManagementOutcome.Succeeded));
        Work(mixed, 333, ScenarioWorkType.SalvageDeadwood);
        Readout.Result mixedResult = Readout.Build(new[] { mixed, latest }, 3);
        Check(mixedResult.RecordedWork.Count(w => w.Year == 3 && w.Link == Readout.OrderLink.AmbiguousIdentity
            && w.Result == Readout.Resolution.Unresolved) == 1, "fixture: one row is both ambiguous and unresolved");
        card = Show(mixedResult, 3);
        Same(Row(card, "work-ambiguous").Text, "2 work record(s) have ambiguous provenance and are not counted.",
            "a row that is ambiguous and unresolved is reported in the ambiguous group");
        Same(Row(card, "work-unresolved").Text, "1 work record(s) have no recorded outcome.",
            "a row that is ambiguous and unresolved is not counted again as unresolved");
        Same(Row(card, "work-outcomes").Text,
            "Recorded outcomes in Year 3: 0 tree(s) felled · 0 planted · 0 crop tree(s) pruned · 1 windthrow stem(s) salvaged · 0 young-tree removal job(s)",
            "only the clean row is counted beside ambiguous and unresolved rows");

        // The same rule on a hand-made result holding exactly one row with both conditions.
        var single = new Readout.Result { SelectedYear = 3, LatestCompletedYear = 9, ComparisonState = Readout.ObservationState.NotRecorded };
        single.RecordedWork.Add(new Readout.RecordedWork
        {
            Year = 3, EventId = 340, WorkOrderId = 1340, TaskType = ScenarioWorkType.FellTree,
            Result = Readout.Resolution.Unresolved, Link = Readout.OrderLink.AmbiguousIdentity
        });
        card = Show(single, 3);
        Check(Row(card, "work-ambiguous").Text == "1 work record(s) have ambiguous provenance and are not counted."
            && !Has(card, "work-unresolved"), "one row with both conditions appears once, in the ambiguous group only");
    }

    // FR-5 / AC-5: recorded outcomes without a consistent retained order.
    private static void OrderLinkage()
    {
        YearHistoryView latest = CleanHistory()[2];
        const string warningOne = "1 of the outcome(s) counted above are recorded event outcomes only; their original order or target linkage cannot be verified.";

        // MissingOrder: the event is recorded but its order is gone.
        var missingOrder = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Add(missingOrder, Event(350, 1350, 3, ScenarioWorkType.FellTree), null);
        Readout.Result result = Readout.Build(new[] { missingOrder, latest }, 3);
        Check(result.RecordedWork.Single(w => w.Year == 3).Link == Readout.OrderLink.MissingOrder, "fixture: MissingOrder link");
        Mapper.Card card = Show(result, 3);
        Check(Row(card, "work-outcomes").Text.Contains("1 tree(s) felled"), "a MissingOrder success still counts as a recorded event outcome");
        Same(Row(card, "work-event-only").Text, warningOne, "MissingOrder carries the linkage warning");
        Check(Row(card, "work-event-only").Provenance == HistoryProvenance.NotRecorded, "linkage warning is NOT RECORDED");
        Check(!Has(card, "work-ambiguous") && !Has(card, "work-unresolved"), "MissingOrder alone raises no other warning");

        // InconsistentOrder: the retained order names a different tree.
        var inconsistent = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        ScenarioOneWorkOrder otherTree = Order(1360, 3, ScenarioWorkType.PruneTree, ScenarioManagementOutcome.Succeeded);
        otherTree.targetTreeId = "OTHER";
        Add(inconsistent, Event(360, 1360, 3, ScenarioWorkType.PruneTree), otherTree);
        result = Readout.Build(new[] { inconsistent, latest }, 3);
        Check(result.RecordedWork.Single(w => w.Year == 3).Link == Readout.OrderLink.InconsistentOrder, "fixture: InconsistentOrder link");
        card = Show(result, 3);
        Check(Row(card, "work-outcomes").Text.Contains("1 crop tree(s) pruned"), "an InconsistentOrder success still counts as a recorded event outcome");
        Same(Row(card, "work-event-only").Text, warningOne, "InconsistentOrder carries the linkage warning");

        // Failed outcomes are covered too, and the warning counts every affected outcome once.
        var several = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Add(several, Event(370, 1370, 3, ScenarioWorkType.FellTree), null);
        Add(several, Event(371, 1371, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Failed), null);
        ScenarioOneWorkOrder wrongYear = Order(1372, 2, ScenarioWorkType.SalvageDeadwood, ScenarioManagementOutcome.Succeeded);
        Add(several, Event(372, 1372, 3, ScenarioWorkType.SalvageDeadwood), wrongYear);
        Work(several, 373, ScenarioWorkType.PlantJuvenile);
        Cancel(several, 374);
        card = Show(3, several, latest);
        Same(Row(card, "work-outcomes").Text,
            "Recorded outcomes in Year 3: 1 tree(s) felled · 1 planted · 0 crop tree(s) pruned · 1 windthrow stem(s) salvaged · 0 young-tree removal job(s)",
            "event-only outcomes are counted with verified ones");
        Same(Row(card, "work-failed-cancelled").Text, "1 task(s) failed · 1 order(s) cancelled", "event-only failure is counted as failed");
        Same(Row(card, "work-event-only").Text,
            "3 of the outcome(s) counted above are recorded event outcomes only; their original order or target linkage cannot be verified.",
            "the linkage warning counts succeeded and failed event-only outcomes");

        // A cancellation never has a retained order (the order was deleted), so it raises no linkage warning by itself.
        var cancelOnly = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        Cancel(cancelOnly, 380);
        card = Show(3, cancelOnly, latest);
        Same(Row(card, "work-failed-cancelled").Text, "0 task(s) failed · 1 order(s) cancelled", "a cancellation is counted as cancelled");
        Check(!Has(card, "work-event-only") && !Has(card, "work-ambiguous") && !Has(card, "work-unresolved"),
            "a cancellation alone raises no warning");

        // A record of the wrong event kind inside the resolved list has no trustworthy outcome: unresolved, not event-only.
        var wrongKind = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        ScenarioManagementEvent created = Event(385, 1385, 3, ScenarioWorkType.FellTree);
        created.eventType = ScenarioManagementEventType.OrderCreated;
        Add(wrongKind, created, Order(1385, 3, ScenarioWorkType.FellTree, ScenarioManagementOutcome.Succeeded));
        card = Show(3, wrongKind, latest);
        Check(Row(card, "work-outcomes").Text == ZeroOutcomesYear3 && Has(card, "work-unresolved") && !Has(card, "work-event-only"),
            "a non-resolution event is unresolved and never counted");
    }

    // FR-8 / AC-6.
    private static void SpeciesRows()
    {
        // Year 3 holds sessile oak and Sitka spruce; Year 9 holds Sitka spruce and beech.
        var year3 = Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f));
        year3.Snapshot.species.Add(Species("sessile-oak", 30, 2, 0, 5));
        year3.Snapshot.species.Add(Species("sitka-spruce", 90, 4, 1, 10));
        var year9 = Year(9, Snapshot(9, 112, 16.7f, 0.66f, 12, 2.25f));
        year9.Snapshot.species.Add(Species("sitka-spruce", 82, 6, 3, 8));
        year9.Snapshot.species.Add(Species("beech", 11, 12, 13, 14));
        Readout.Result result = Readout.Build(new[] { year3, year9 }, 3);
        Mapper.Card card = Show(result, 3);

        Mapper.Row heading = Row(card, "species-heading");
        Check(heading.Kind == Mapper.RowKind.SpeciesHeading && heading.Text == "By species" && !heading.Provenance.HasValue, "species heading");

        Mapper.Row beech = Row(card, "species-beech-not-recorded");
        Same(beech.Text, "Beech: not recorded for one of the two years; no difference shown. Missing for Year 3; recorded for Year 9.",
            "species missing on the selected side names Year A");
        Check(beech.Kind == Mapper.RowKind.SpeciesNotRecorded && beech.MissingYear == 3 && beech.Provenance == HistoryProvenance.NotRecorded
            && beech.SpeciesId == "beech" && beech.SpeciesName == "Beech", "selected-side missing species is structured as NOT RECORDED for Year 3");

        Mapper.Row oak = Row(card, "species-sessile-oak-not-recorded");
        Same(oak.Text, "Sessile oak: not recorded for one of the two years; no difference shown. Missing for Year 9; recorded for Year 3.",
            "species missing on the latest side names Year B");
        Check(oak.MissingYear == 9 && oak.Provenance == HistoryProvenance.NotRecorded, "latest-side missing species is structured as NOT RECORDED for Year 9");

        foreach (Mapper.Row missing in new[] { beech, oak })
            Check(missing.SelectedValue == "" && missing.LatestValue == "" && missing.Difference == ""
                && !missing.Text.Contains("11") && !missing.Text.Contains("30") && !missing.Text.Contains(" 0"),
                "a one-sided species shows no value, no zero and no difference (" + missing.SpeciesId + ")");
        Check(!card.Rows.Any(r => r.Key.StartsWith("species-beech-") && r.Kind == Mapper.RowKind.SpeciesMeasure)
            && !card.Rows.Any(r => r.Key.StartsWith("species-sessile-oak-") && r.Kind == Mapper.RowKind.SpeciesMeasure),
            "a one-sided species has no measure rows");

        // Four measures, four rows, four different pairs of numbers; nothing is added together.
        List<Mapper.Row> sitka = card.Rows.Where(r => r.SpeciesId == "sitka-spruce").ToList();
        Check(sitka.Count == 4 && sitka.All(r => r.Kind == Mapper.RowKind.SpeciesMeasure), "four separate measure rows per species");
        Same(string.Join("\n", sitka.Select(r => r.Text)),
            "Sitka spruce: standing trees 90 → 82 (-8)\n"
            + "natural regeneration map squares (modelled) 4 → 6 (+2)\n"
            + "planted regeneration map squares (modelled) 1 → 3 (+2)\n"
            + "planted young trees (counted) 10 → 8 (-2)", "four species measures exact text");
        Same(string.Join(";", sitka.Select(r => r.Label + "=" + r.SelectedValue + ">" + r.LatestValue + ">" + r.Difference)),
            "standing trees=90>82>-8;natural regeneration map squares (modelled)=4>6>+2;"
            + "planted regeneration map squares (modelled)=1>3>+2;planted young trees (counted)=10>8>-2",
            "four species measures as separate columns");
        Check(sitka[0].FirstRowOfSpecies && !sitka[1].FirstRowOfSpecies && !sitka[2].FirstRowOfSpecies && !sitka[3].FirstRowOfSpecies,
            "only the first row of a species starts with its name");
        // Sums a careless implementation might show: all regeneration squares (5, 9), squares plus planted trees (15, 17).
        Check(!sitka.Any(r => r.SelectedValue == "5" || r.LatestValue == "9" || r.SelectedValue == "15" || r.LatestValue == "17"),
            "species measures are never summed");

        Same(Keys(card).Substring(Keys(card).IndexOf("species-heading", StringComparison.Ordinal)),
            "species-heading,species-beech-not-recorded,species-sessile-oak-not-recorded,species-sitka-spruce-standing-trees,"
            + "species-sitka-spruce-natural-regeneration,species-sitka-spruce-planted-regeneration,species-sitka-spruce-planted-young-trees,"
            + "caveat,guidance-pointer", "species rows follow the helper's order, inside the card");

        // Without a name lookup the id is shown rather than a guessed name.
        Mapper.Card unnamed = Mapper.Describe(result, 3, null);
        seen.Add(unnamed);
        Check(Row(unnamed, "species-sitka-spruce-standing-trees").Text == "Sitka-spruce: standing trees 90 → 82 (-8)",
            "no name lookup: the species id is shown");

        // No species in either snapshot: no empty species section.
        Mapper.Card none = Show(3, Year(3, Snapshot(3, 120, 14.2f, 0.62f, 7, 3.5f)), Year(9, Snapshot(9, 112, 16.7f, 0.66f, 12, 2.25f)));
        Check(none.State == Mapper.CardState.Compared && !Has(none, "species-heading"), "no species records: no species section");
    }

    // FR-7.
    private static void UnitsAndSigns()
    {
        // Canopy is stored as a fraction: 0.5 is 50 %, and 0.5 → 0.47 is a fall of 3.0 percentage points.
        Mapper.Card falling = Show(3, Year(3, Snapshot(3, 100, 21.5f, 0.5f, 10, 1.5f)), Year(4, Snapshot(4, 100, 21.5f, 0.47f, 10, 1.5f)));
        Mapper.Row canopy = Row(falling, "stand-canopy");
        Check(canopy.SelectedValue == "50 %" && canopy.LatestValue == "47 %", "canopy fractions are shown as percent");
        Same(canopy.Difference, "-3.0 points", "canopy change is shown in percentage points with its sign");
        Same(canopy.Text, "Mean canopy cover: 50 % → 47 % (-3.0 points)", "canopy row exact text");
        Check(!falling.Rows.Any(r => r.Text.Contains("0.5") || r.Text.Contains("0.47") || r.Text.Contains("0.03")),
            "no raw canopy fraction is shown");

        // No change is a plain zero, with no sign, in every unit.
        Same(Row(falling, "stand-living-trees").Text, "Standing trees: 100 → 100 (0)", "unchanged whole number shows 0");
        Same(Row(falling, "stand-basal-area").Text, "Basal area: 21.5 m²/ha → 21.5 m²/ha (0.0 m²/ha)", "unchanged basal area shows 0.0");
        Same(Row(falling, "stand-regeneration").Text, "Regeneration map squares (modelled): 10 → 10 (0)", "unchanged regeneration shows 0");
        Same(Row(falling, "stand-deadwood").Text, "Fallen deadwood: 1.50 m³ → 1.50 m³ (0.00 m³)", "unchanged deadwood shows 0.00");
        Same(Row(falling, "snapshots").Text, "Stand recorded at the end of Year 3 and at the end of Year 4, 1 year(s) later.", "elapsed years are shown");

        // Every measure rising, and every measure falling.
        Mapper.Card rising = Show(2, Year(2, Snapshot(2, 80, 10.5f, 0.4f, 3, 0.75f)), Year(12, Snapshot(12, 95, 13.0f, 0.55f, 9, 2.0f)));
        Same(string.Join(";", rising.Rows.Where(r => r.Kind == Mapper.RowKind.StandMeasure).Select(r => r.Difference)),
            "+15;+2.5 m²/ha;+15.0 points;+6;+1.25 m³", "rising differences carry a plus sign and the right unit");
        Mapper.Card dropping = Show(2, Year(2, Snapshot(2, 95, 13.0f, 0.55f, 9, 2.0f)), Year(12, Snapshot(12, 80, 10.5f, 0.4f, 3, 0.75f)));
        Same(string.Join(";", dropping.Rows.Where(r => r.Kind == Mapper.RowKind.StandMeasure).Select(r => r.Difference)),
            "-15;-2.5 m²/ha;-15.0 points;-6;-1.25 m³", "falling differences carry a minus sign and the right unit");
        Same(Row(rising, "snapshots").Text, "Stand recorded at the end of Year 2 and at the end of Year 12, 10 year(s) later.", "ten elapsed years");

        // The helper's own values are what is shown: the displayed difference is the helper's, converted only for canopy.
        Readout.Result result = Readout.Build(new[] { Year(2, Snapshot(2, 80, 10.5f, 0.4f, 3, 0.75f)), Year(12, Snapshot(12, 95, 13.0f, 0.55f, 9, 2.0f)) }, 2);
        Check(Math.Abs(result.Comparison.MeanCanopyChange - 0.15f) < 0.0001f && result.Selected.MeanCanopy == 0.4f,
            "the helper's stored canopy values stay fractions");
    }

    // FR-13 (offline part), FR-14.
    private static void StableAndReadOnly()
    {
        YearHistoryView[] years = CleanHistory();
        Readout.Result result = Readout.Build(years, 3);
        string before = Fingerprint(result);
        Mapper.Card first = Show(result, 3);
        Mapper.Card second = Show(result, 3);
        Same(Dump(second), Dump(first), "a repeat call gives identical output");
        Same(Fingerprint(result), before, "the mapper does not change its input");
        Check(!ReferenceEquals(first, second) && !ReferenceEquals(first.Rows, second.Rows), "each call returns its own card");

        // The same saved history built again (as after a different kind of time advance) gives the same card.
        Mapper.Card rebuilt = Show(Readout.Build(CleanHistory(), 3), 3);
        Same(Dump(rebuilt), Dump(first), "the same history gives the same card");
        // The order in which the completed years are supplied does not matter.
        Mapper.Card reordered = Show(Readout.Build(new[] { years[2], years[0], years[1] }, 3), 3);
        Same(Dump(reordered), Dump(first), "the order of supplied years does not change the card");

        // Changing a returned card cannot change a later card or the input.
        first.Rows.Clear();
        first.State = Mapper.CardState.InvalidInput;
        Same(Dump(Show(result, 3)), Dump(second), "a changed card does not affect the next card");
        Same(Fingerprint(result), before, "the input is still unchanged after many calls");

        // The missing-snapshot and latest-year paths are read-only as well.
        Readout.Result latest = Readout.Build(years, 9);
        string latestBefore = Fingerprint(latest);
        Show(latest, 9);
        Readout.Result missing = Readout.Build(new[] { years[0], Year(9) }, 3);
        string missingBefore = Fingerprint(missing);
        Show(missing, 3);
        Check(Fingerprint(latest) == latestBefore && Fingerprint(missing) == missingBefore, "other states leave their input unchanged");
    }

    // FR-10, FR-11, FR-12 and the "not displayed" rules, read over every card the groups above produced.
    private static void WordingAndLabels()
    {
        // A card with everything on it, including harvest rows the helper flags as having grouped money.
        Readout.Result rich = Readout.Build(CleanHistory(), 3);
        Check(rich.RecordedWork.Any(w => w.Year == 3 && w.GroupedHarvestMoney) && rich.ExecutedCoverage.Count > 0
            && rich.PossibleTreatmentYears.Count > 0, "fixture: the result holds money flags, coverage groups and candidate years");
        Show(rich, 3);

        string[] money = { "€", "$", "£", "¢", "EUR", "cents", "money", "Money", "income", "revenue", "cost", "cash", "price", "settlement" };
        string[] causal = { "caused", "because", "due to", "led to", "result of" };
        string[] outOfScope = { "surviv", "hectare", "score", "interval", "U-ALPHA", "coverage", "candidate", "Candidate", "CANDIDATE",
            "prescription", "rack", "thinned", "major intervention" };
        const string pointer = "For what to inspect before deciding again, see the prior-treatment guidance in this review and walk the stand.";
        var moneyHits = new List<string>();
        var causalHits = new List<string>();
        var scopeHits = new List<string>();
        var fixedTextHits = new List<string>();
        var labelHits = new List<string>();
        var shapeHits = new List<string>();
        int rows = 0;
        foreach (Mapper.Card card in seen)
        {
            foreach (Mapper.Row row in card.Rows)
            {
                rows++;
                string all = string.Join(" | ", row.Text, row.Label, row.SelectedValue, row.LatestValue, row.Difference, row.SpeciesName);
                foreach (string word in money)
                    if (all.Contains(word)) moneyHits.Add("'" + word + "' in: " + all);
                foreach (string word in outOfScope)
                    if (all.Contains(word)) scopeHits.Add("'" + word + "' in: " + all);
                // The caveat is the one sentence allowed to use the word "caused", and only to deny it.
                string caveat = "These are recorded stand totals for two years. They do not show that Year " + card.ViewedYear + "'s work caused the difference.";
                if (row.Kind == Mapper.RowKind.Caveat)
                {
                    if (row.Text != caveat) fixedTextHits.Add(row.Text);
                }
                else
                {
                    foreach (string word in causal)
                        if (all.Contains(word)) causalHits.Add("'" + word + "' in: " + all);
                    if (row.Kind == Mapper.RowKind.GuidancePointer && row.Text != pointer) fixedTextHits.Add(row.Text);
                }
                string problem = LabelProblem(row);
                if (problem.Length > 0) labelHits.Add(problem + " in row '" + row.Key + "': " + all);
            }
            // The caveat and the pointer appear exactly when two years are compared, as the last two rows.
            bool compared = card.State == Mapper.CardState.Compared;
            if (Count(card, Mapper.RowKind.Caveat) != (compared ? 1 : 0) || Count(card, Mapper.RowKind.GuidancePointer) != (compared ? 1 : 0))
                shapeHits.Add("caveat presence for state " + card.State);
            if (!card.Visible && card.Rows.Count != 0) shapeHits.Add("a hidden card has rows");
        }
        Check(moneyHits.Count == 0, "no currency or money wording" + First(moneyHits));
        Check(causalHits.Count == 0, "no causal wording" + First(causalHits));
        Check(scopeHits.Count == 0, "no out-of-scope wording or undisplayed helper field" + First(scopeHits));
        Check(fixedTextHits.Count == 0, "fixed caveat and guidance pointer sentences" + First(fixedTextHits));
        Check(labelHits.Count == 0, "every row carries the right data-source label" + First(labelHits));
        Check(shapeHits.Count == 0, "caveat only with a comparison; hidden cards are empty" + First(shapeHits));
        Check(rows >= 21, "the sweeps read at least one full card (" + rows + " rows)");
    }

    private static string First(List<string> hits) => hits.Count == 0 ? "" : ": " + hits[0];

    // The data-source label each kind of row must carry (FR-10). Returns "" when the row is labelled correctly.
    private static string LabelProblem(Mapper.Row row)
    {
        HistoryProvenance? none = null;
        switch (row.Kind)
        {
            case Mapper.RowKind.Heading:
            case Mapper.RowKind.SpeciesHeading:
            case Mapper.RowKind.Caveat:
            case Mapper.RowKind.GuidancePointer:
                return row.Provenance == none ? "" : "a title, caveat or pointer must carry no label";
            case Mapper.RowKind.TableHeader:
                if (row.Provenance != none || row.ValueProvenance != HistoryProvenance.Recorded) return "table header labels";
                return (row.Difference == "Difference") == (row.DifferenceProvenance == HistoryProvenance.Derived) ? "" : "difference column label";
            case Mapper.RowKind.StandMeasure:
            case Mapper.RowKind.SpeciesMeasure:
                if (row.ValueProvenance != HistoryProvenance.Recorded) return "a copied snapshot value must be RECORDED";
                if (row.Difference.Length > 0)
                    return row.DifferenceProvenance == HistoryProvenance.Derived && row.Provenance == HistoryProvenance.Derived
                        ? "" : "a difference must be DERIVED";
                return row.DifferenceProvenance == none && row.Provenance == HistoryProvenance.Recorded ? "" : "a lone observation must be RECORDED";
            case Mapper.RowKind.SpeciesNotRecorded:
                return row.Provenance == HistoryProvenance.NotRecorded ? "" : "a missing species must be NOT RECORDED";
            case Mapper.RowKind.Line:
                switch (row.Key)
                {
                    case "latest-year": return row.Provenance == none ? "" : "the latest-year hint must carry no label";
                    case "work-outcomes":
                    case "work-failed-cancelled":
                    case "work-none": return row.Provenance == HistoryProvenance.Derived ? "" : "event counts must be DERIVED";
                    case "snapshots":
                    case "snapshot-selected-only": return row.Provenance == HistoryProvenance.Recorded ? "" : "a snapshot statement must be RECORDED";
                    case "work-ambiguous":
                    case "work-unresolved":
                    case "work-event-only":
                    case "missing-selected":
                    case "missing-latest":
                    case "cannot-compare": return row.Provenance == HistoryProvenance.NotRecorded ? "" : "a gap or unverified record must be NOT RECORDED";
                    default: return "unknown line key";
                }
            default:
                return "unknown row kind";
        }
    }
}

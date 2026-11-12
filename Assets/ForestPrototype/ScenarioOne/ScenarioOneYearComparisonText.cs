using System;
using System.Collections.Generic;
using System.Globalization;
// Short name for the read-only helper whose Build result this class turns into words.
using Readout = ScenarioOneManagementFollowonReadout;

// D-073 Annual Review card "Year A compared with latest Year B": player-facing wording only.
//
// Input: ONE result of ScenarioOneManagementFollowonReadout.Build, plus the year on screen. Output: the rows of the card
// in display order, each with its text and its data-source label. Nothing else is read: not the live forest, not the
// manager, not any other history. Nothing is stored and the input is never changed, so the same result always gives
// the same card.
//
// The rows are dated records and plain subtraction. They never say that work caused a difference, and they show no money.
// Spec: specs/scenario-one-multiyear-decision-outcome-feedback-v2 (FR-4 to FR-12). Strings only: no UI types here.
public static class ScenarioOneYearComparisonText
{
    // Which of the spec's presentations (FR-9) the card is in. Exactly one applies to a result.
    public enum CardState
    {
        NoCompletedYear,         // no card: nothing has been completed yet
        InvalidInput,            // no card: no result, or the result was built for a different year than the one on screen
        LatestYear,              // one muted line and nothing else
        CannotCompare,           // work lines, then "the records for these years are ambiguous"
        SelectedSnapshotMissing, // work lines, then "no stand snapshot for Year A" (and for Year B, if that is missing too)
        LatestSnapshotMissing,   // work lines, Year A's recorded stand, then "no stand snapshot for the latest year"
        Compared                 // work lines, both recorded stands, differences, species rows and the caveat
    }

    public enum RowKind
    {
        Heading,            // the card title
        Line,               // one sentence
        TableHeader,        // column titles of the stand table
        StandMeasure,       // one row of the stand table
        SpeciesHeading,     // title of the "By species" part, which the screen keeps closed until opened
        SpeciesMeasure,     // one of the four measures of one species
        SpeciesNotRecorded, // a species that one of the two years has no record for
        Caveat,             // the fixed "this is not cause and effect" sentence
        GuidancePointer     // the fixed pointer to the existing prior-treatment guidance card
    }

    public sealed class Row
    {
        public RowKind Kind;
        // Stable name for checks and UI element names (for example "stand-canopy"). Never shown to the player.
        public string Key = "";
        // The data-source label of the row as a whole. Null means the row carries no label (title, hint, caveat).
        // A row that shows two recorded values AND their difference is labelled Derived, as elsewhere in the review.
        public HistoryProvenance? Provenance;
        // The complete row as plain text. A screen that does not draw columns can show this alone.
        public string Text = "";
        // True for quiet rows (the latest-year hint, the caveat and the pointer).
        public bool Muted;

        // Column values, for TableHeader, StandMeasure and SpeciesMeasure rows. "" means there is nothing to show there.
        public string Label = "";
        public string SelectedValue = ""; // end of the viewed year
        public string LatestValue = "";   // end of the latest completed year
        public string Difference = "";    // latest minus viewed, with its sign
        public HistoryProvenance? ValueProvenance;      // label for SelectedValue and LatestValue
        public HistoryProvenance? DifferenceProvenance; // label for Difference

        // Species rows only.
        public string SpeciesId = "";
        public string SpeciesName = "";
        // True on the first row of a species: its Text starts with the species name. Later rows belong under it.
        public bool FirstRowOfSpecies;
        // SpeciesNotRecorded only: the year that has no record for the species, or -1 when the result cannot say which.
        public int MissingYear = -1;
    }

    public sealed class Card
    {
        public CardState State;
        // False means: add nothing to the review.
        public bool Visible;
        public int ViewedYear;
        public int LatestYear = -1;
        public readonly List<Row> Rows = new List<Row>();
    }

    // Builds the card for the year on screen. "speciesName" turns a species id into its display name; the game passes
    // ScenarioOneUiFacts.SpeciesName, which needs a scene object and so cannot be called from here. Null shows the id.
    public static Card Describe(Readout.Result result, int viewedYear, Func<string, string> speciesName)
    {
        var card = new Card { ViewedYear = viewedYear };
        // A result built for another year is never shown under this year's name.
        if (result == null || result.SelectedYear != viewedYear)
        {
            card.State = CardState.InvalidInput;
            return card;
        }
        card.LatestYear = result.LatestCompletedYear;
        if (result.LatestCompletedYear < 0)
        {
            card.State = CardState.NoCompletedYear;
            return card;
        }

        card.Visible = true;
        int yearA = viewedYear;
        int yearB = result.LatestCompletedYear;

        // The latest year has nothing later to be compared with. This comes before every other state.
        if (result.ComparisonState == Readout.ObservationState.LatestNotLater || yearA >= yearB)
        {
            card.State = CardState.LatestYear;
            Row hint = AddLine(card, RowKind.Line, "latest-year", null,
                $"Year {N(yearB)} is the latest completed year. Choose an earlier year to compare it with Year {N(yearB)}.");
            hint.Muted = true;
            return card;
        }

        AddLine(card, RowKind.Heading, "heading", null, $"YEAR {N(yearA)} COMPARED WITH LATEST YEAR {N(yearB)}");
        AddWorkLines(card, result, yearA);

        // A stand is shown only from the snapshot recorded for exactly that year. There is no fallback to a
        // neighbouring year, to zero or to today's forest.
        bool selectedRecorded = result.SelectedState == Readout.ObservationState.Recorded
            && result.Selected != null && result.Selected.Year == yearA;
        bool latestRecorded = result.LatestState == Readout.ObservationState.Recorded
            && result.Latest != null && result.Latest.Year == yearB;
        bool differencesRecorded = result.ComparisonState == Readout.ObservationState.Recorded && result.Comparison != null;

        // Order matters: ambiguous records first, then the viewed year's missing snapshot, then the latest year's.
        if (result.ComparisonState == Readout.ObservationState.CannotCompare)
        {
            card.State = CardState.CannotCompare;
            AddCannotCompareLine(card);
        }
        else if (!selectedRecorded)
        {
            card.State = CardState.SelectedSnapshotMissing;
            AddLine(card, RowKind.Line, "missing-selected", HistoryProvenance.NotRecorded,
                $"No stand snapshot was recorded for Year {N(yearA)}. Nothing is compared.");
            if (!latestRecorded) AddLatestMissingLine(card, yearB);
        }
        else if (!latestRecorded)
        {
            card.State = CardState.LatestSnapshotMissing;
            AddLine(card, RowKind.Line, "snapshot-selected-only", HistoryProvenance.Recorded,
                $"Stand recorded at the end of Year {N(yearA)}.");
            AddTableHeader(card, yearA, yearB, false);
            AddStandRows(card, result.Selected, null, null);
            AddLatestMissingLine(card, yearB);
        }
        else if (differencesRecorded)
        {
            card.State = CardState.Compared;
            AddLine(card, RowKind.Line, "snapshots", HistoryProvenance.Recorded,
                $"Stand recorded at the end of Year {N(yearA)} and at the end of Year {N(yearB)}, {N(result.Comparison.YearsBetween)} year(s) later.");
            AddTableHeader(card, yearA, yearB, true);
            AddStandRows(card, result.Selected, result.Latest, result.Comparison);
            AddSpeciesRows(card, result, yearA, yearB, speciesName);
            Row caveat = AddLine(card, RowKind.Caveat, "caveat", null,
                $"These are recorded stand totals for two years. They do not show that Year {N(yearA)}'s work caused the difference.");
            caveat.Muted = true;
            Row pointer = AddLine(card, RowKind.GuidancePointer, "guidance-pointer", null,
                "For what to inspect before deciding again, see the prior-treatment guidance in this review and walk the stand.");
            pointer.Muted = true;
        }
        else
        {
            // Both snapshots exist but the result holds no usable comparison. Show no numbers rather than guess.
            card.State = CardState.CannotCompare;
            AddCannotCompareLine(card);
        }
        return card;
    }

    // What the events of the viewed year record. These are counts of recorded event outcomes. They are not proof of
    // work on a particular tree, of a major intervention or of any effect on the stand.
    private static void AddWorkLines(Card card, Readout.Result result, int year)
    {
        int rowsForYear = 0, ambiguous = 0, unresolved = 0, failed = 0, cancelled = 0, eventOnly = 0;
        int felled = 0, planted = 0, pruned = 0, salvaged = 0, removals = 0, other = 0;
        foreach (Readout.RecordedWork work in result.RecordedWork)
        {
            // RecordedWork holds every completed year. Only the viewed year's rows belong on this card.
            if (work == null || work.Year != year) continue;
            rowsForYear++;
            // Fail closed: a row whose identity is ambiguous is never counted. A row that is also unresolved is
            // reported here only, so it is not counted twice.
            if (work.Link == Readout.OrderLink.AmbiguousIdentity) { ambiguous++; continue; }
            if (work.Result == Readout.Resolution.Unresolved) { unresolved++; continue; }
            // A cancelled order was deleted when it was cancelled, so it never has an order to link to.
            if (work.Result == Readout.Resolution.Cancelled) { cancelled++; continue; }

            // From here the row is a recorded succeeded or failed outcome. Without a consistent retained order it
            // still counts, but only as an event, and the card says so.
            if (work.Link == Readout.OrderLink.MissingOrder || work.Link == Readout.OrderLink.InconsistentOrder) eventOnly++;
            if (work.Result == Readout.Resolution.Failed) { failed++; continue; }
            switch (work.TaskType)
            {
                case ScenarioWorkType.FellTree: felled++; break;
                case ScenarioWorkType.PlantJuvenile: planted++; break;
                case ScenarioWorkType.PruneTree: pruned++; break;
                case ScenarioWorkType.SalvageDeadwood: salvaged++; break;
                case ScenarioWorkType.RemoveRegeneration: removals++; break;
                default: other++; break;
            }
        }

        if (rowsForYear == 0)
        {
            AddLine(card, RowKind.Line, "work-none", HistoryProvenance.Derived,
                $"No recorded work-resolution or cancellation events for Year {N(year)}.");
            return;
        }

        string outcomes = $"Recorded outcomes in Year {N(year)}: {N(felled)} tree(s) felled · {N(planted)} planted · {N(pruned)} crop tree(s) pruned"
            + $" · {N(salvaged)} windthrow stem(s) salvaged · {N(removals)} young-tree removal job(s)";
        // Only for a task type this card has no name for; the five known types never reach it.
        if (other > 0) outcomes += $" · {N(other)} other task(s)";
        AddLine(card, RowKind.Line, "work-outcomes", HistoryProvenance.Derived, outcomes);
        AddLine(card, RowKind.Line, "work-failed-cancelled", HistoryProvenance.Derived,
            $"{N(failed)} task(s) failed · {N(cancelled)} order(s) cancelled");
        if (ambiguous > 0)
            AddLine(card, RowKind.Line, "work-ambiguous", HistoryProvenance.NotRecorded,
                $"{N(ambiguous)} work record(s) have ambiguous provenance and are not counted.");
        if (unresolved > 0)
            AddLine(card, RowKind.Line, "work-unresolved", HistoryProvenance.NotRecorded,
                $"{N(unresolved)} work record(s) have no recorded outcome.");
        if (eventOnly > 0)
            AddLine(card, RowKind.Line, "work-event-only", HistoryProvenance.NotRecorded,
                $"{N(eventOnly)} of the outcome(s) counted above are recorded event outcomes only; their original order or target linkage cannot be verified.");
    }

    private static void AddCannotCompareLine(Card card)
    {
        AddLine(card, RowKind.Line, "cannot-compare", HistoryProvenance.NotRecorded,
            "The records for these years are ambiguous, so no comparison is shown.");
    }

    private static void AddLatestMissingLine(Card card, int yearB)
    {
        AddLine(card, RowKind.Line, "missing-latest", HistoryProvenance.NotRecorded,
            $"No stand snapshot was recorded for the latest year, Year {N(yearB)}. Nothing is compared.");
    }

    private static void AddTableHeader(Card card, int yearA, int yearB, bool bothYears)
    {
        var row = new Row
        {
            Kind = RowKind.TableHeader, Key = "table-header",
            SelectedValue = $"End of Year {N(yearA)}",
            ValueProvenance = HistoryProvenance.Recorded
        };
        if (bothYears)
        {
            row.LatestValue = $"End of Year {N(yearB)}";
            row.Difference = "Difference";
            row.DifferenceProvenance = HistoryProvenance.Derived;
        }
        row.Text = bothYears ? $"{row.SelectedValue} · {row.LatestValue} · {row.Difference}" : row.SelectedValue;
        card.Rows.Add(row);
    }

    // The five stand measures. "b" and "change" are null when only the viewed year's snapshot is shown.
    // Units are the helper's own, except canopy: it is stored as a fraction (0.62 means 62 %), so it is multiplied by
    // 100 here for display only, and its difference reads as percentage points.
    private static void AddStandRows(Card card, Readout.StandObservation a, Readout.StandObservation b, Readout.StandComparison change)
    {
        bool both = b != null && change != null;
        AddStandRow(card, "stand-living-trees", "Standing trees",
            N(a.LivingTrees),
            both ? N(b.LivingTrees) : "",
            both ? Signed(change.LivingTreesChange) : "");
        AddStandRow(card, "stand-basal-area", "Basal area",
            F(a.BasalAreaM2PerHa, "0.0") + " m²/ha",
            both ? F(b.BasalAreaM2PerHa, "0.0") + " m²/ha" : "",
            both ? Signed(change.BasalAreaM2PerHaChange, "0.0") + " m²/ha" : "");
        AddStandRow(card, "stand-canopy", "Mean canopy cover",
            F(a.MeanCanopy * 100f, "0") + " %",
            both ? F(b.MeanCanopy * 100f, "0") + " %" : "",
            both ? Signed(change.MeanCanopyChange * 100f, "0.0") + " points" : "");
        AddStandRow(card, "stand-regeneration", "Regeneration map squares (modelled)",
            N(a.OccupiedRegenerationCells),
            both ? N(b.OccupiedRegenerationCells) : "",
            both ? Signed(change.OccupiedRegenerationCellsChange) : "");
        AddStandRow(card, "stand-deadwood", "Fallen deadwood",
            F(a.DeadwoodVolumeM3, "0.00") + " m³",
            both ? F(b.DeadwoodVolumeM3, "0.00") + " m³" : "",
            both ? Signed(change.DeadwoodVolumeM3Change, "0.00") + " m³" : "");
    }

    private static void AddStandRow(Card card, string key, string label, string selected, string latest, string difference)
    {
        bool compared = latest.Length > 0;
        card.Rows.Add(new Row
        {
            Kind = RowKind.StandMeasure, Key = key, Label = label,
            SelectedValue = selected, LatestValue = latest, Difference = difference,
            ValueProvenance = HistoryProvenance.Recorded,
            DifferenceProvenance = compared ? HistoryProvenance.Derived : (HistoryProvenance?)null,
            Provenance = compared ? HistoryProvenance.Derived : HistoryProvenance.Recorded,
            Text = compared ? $"{label}: {selected} → {latest} ({difference})" : $"{label}: {selected}"
        });
    }

    // Four measures per species, always separate and never added together: they count different things
    // (standing trees, modelled map squares of natural and of planted regeneration, and exactly counted planted trees).
    private static void AddSpeciesRows(Card card, Readout.Result result, int yearA, int yearB, Func<string, string> speciesName)
    {
        if (result.Comparison.Species.Count == 0) return;
        AddLine(card, RowKind.SpeciesHeading, "species-heading", null, "By species");
        foreach (Readout.SpeciesComparison change in result.Comparison.Species)
        {
            if (change == null) continue;
            string id = change.SpeciesId ?? "";
            string name = DisplayName(id, speciesName);
            Readout.SpeciesObservation a = FindSpecies(result.Selected.Species, id);
            Readout.SpeciesObservation b = FindSpecies(result.Latest.Species, id);
            if (change.State != Readout.ObservationState.Recorded || a == null || b == null)
            {
                AddSpeciesNotRecorded(card, id, name, a != null, b != null, yearA, yearB);
                continue;
            }
            AddSpeciesMeasure(card, id, name, true, "standing-trees", "standing trees",
                a.LivingTrees, b.LivingTrees, change.LivingTreesChange);
            AddSpeciesMeasure(card, id, name, false, "natural-regeneration", "natural regeneration map squares (modelled)",
                a.NaturalRegenerationCells, b.NaturalRegenerationCells, change.NaturalRegenerationCellsChange);
            AddSpeciesMeasure(card, id, name, false, "planted-regeneration", "planted regeneration map squares (modelled)",
                a.PlantedRegenerationCells, b.PlantedRegenerationCells, change.PlantedRegenerationCellsChange);
            AddSpeciesMeasure(card, id, name, false, "planted-young-trees", "planted young trees (counted)",
                a.ExactPlantedJuveniles, b.ExactPlantedJuveniles, change.ExactPlantedJuvenilesChange);
        }
    }

    private static void AddSpeciesMeasure(Card card, string id, string name, bool first, string keyPart, string label,
        int selected, int latest, int difference)
    {
        string measure = $"{label} {N(selected)} → {N(latest)} ({Signed(difference)})";
        card.Rows.Add(new Row
        {
            Kind = RowKind.SpeciesMeasure, Key = "species-" + id + "-" + keyPart,
            SpeciesId = id, SpeciesName = name, FirstRowOfSpecies = first, Label = label,
            SelectedValue = N(selected), LatestValue = N(latest), Difference = Signed(difference),
            ValueProvenance = HistoryProvenance.Recorded,
            DifferenceProvenance = HistoryProvenance.Derived,
            Provenance = HistoryProvenance.Derived,
            Text = first ? name + ": " + measure : measure
        });
    }

    // A species that only one of the two snapshots holds. That is a missing record for one year: it is not a zero
    // and it does not mean the species was absent in both years.
    private static void AddSpeciesNotRecorded(Card card, string id, string name, bool inSelected, bool inLatest, int yearA, int yearB)
    {
        string text = name + ": not recorded for one of the two years; no difference shown.";
        int missingYear = -1;
        if (!inSelected && inLatest)
        {
            missingYear = yearA;
            text += $" Missing for Year {N(yearA)}; recorded for Year {N(yearB)}.";
        }
        else if (inSelected && !inLatest)
        {
            missingYear = yearB;
            text += $" Missing for Year {N(yearB)}; recorded for Year {N(yearA)}.";
        }
        card.Rows.Add(new Row
        {
            Kind = RowKind.SpeciesNotRecorded, Key = "species-" + id + "-not-recorded",
            SpeciesId = id, SpeciesName = name, FirstRowOfSpecies = true, MissingYear = missingYear,
            Provenance = HistoryProvenance.NotRecorded, Text = text
        });
    }

    private static Readout.SpeciesObservation FindSpecies(List<Readout.SpeciesObservation> list, string id)
    {
        foreach (Readout.SpeciesObservation species in list)
            if (species != null && string.Equals(species.SpeciesId, id, StringComparison.Ordinal)) return species;
        return null;
    }

    private static string DisplayName(string id, Func<string, string> speciesName)
    {
        string name = speciesName != null ? speciesName(id) : id;
        if (string.IsNullOrEmpty(name)) name = id;
        if (string.IsNullOrEmpty(name)) return "Unknown species";
        // Names such as "sessile oak" start a line here, so the first letter is shown as a capital.
        return char.ToUpperInvariant(name[0]) + name.Substring(1);
    }

    private static Row AddLine(Card card, RowKind kind, string key, HistoryProvenance? provenance, string text)
    {
        var row = new Row { Kind = kind, Key = key, Provenance = provenance, Text = text };
        card.Rows.Add(row);
        return row;
    }

    // Numbers are written the same way on every computer, whatever its language settings.
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string N(int value) => value.ToString(Inv);
    private static string F(float value, string format) => value.ToString(format, Inv);
    // A difference always shows its direction: "+3", "-2", and plain "0" when nothing changed.
    private static string Signed(int value) => value.ToString("+0;-0;0", Inv);
    private static string Signed(float value, string format) => value.ToString("+" + format + ";-" + format + ";" + format, Inv);
}

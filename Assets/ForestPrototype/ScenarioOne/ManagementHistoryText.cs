using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Player-facing wording for the derived management history (ScenarioInterventionHistory). Strings only: no UI types, no
// stored state. Money and species names are supplied by the caller so the same text works in the game and in the offline
// checks.
//
// The wording describes what was recorded after the work. It does not claim the work caused the change: canopy,
// regeneration and mortality also respond to growth, seed, light and weather in the same years.
public sealed class HistoryTextContext
{
    public Func<long, string> Money = cents => (cents / 100.0).ToString("0.00", CultureInfo.InvariantCulture);
    public Func<long, string> SignedMoney = cents => (cents > 0 ? "+" : cents < 0 ? "-" : "") + (Math.Abs(cents) / 100.0).ToString("0.00", CultureInfo.InvariantCulture);
    public Func<string, string> SpeciesName = id => id;
}

// Where a fact shown in the review or history comes from. Every history line carries exactly one of these so the player can
// tell what the save really holds from what was worked out afterwards. Nothing here is stored; it only labels presentation.
public enum HistoryProvenance
{
    Recorded,      // persisted when it happened (annual reports, events, orders, snapshots, objective achievements)
    Current,       // the live forest or live state now; never a statement about an earlier year
    Derived,       // computed at display time from recorded facts (deltas, trends, sums); not stored itself
    Reconstructed, // recovered from other records because the aggregate itself was never stored (storm victims)
    SessionOnly,   // exists only in this play session and is gone after save/load
    NotRecorded    // the save holds no such fact; it is never rebuilt from today's forest
}

public static class HistoryProvenanceText
{
    public static string Label(HistoryProvenance kind)
    {
        switch (kind)
        {
            case HistoryProvenance.Recorded: return "RECORDED";
            case HistoryProvenance.Current: return "CURRENT";
            case HistoryProvenance.Derived: return "DERIVED";
            case HistoryProvenance.Reconstructed: return "RECONSTRUCTED";
            case HistoryProvenance.SessionOnly: return "SESSION-ONLY";
            default: return "NOT RECORDED";
        }
    }

    public static string Meaning(HistoryProvenance kind)
    {
        switch (kind)
        {
            case HistoryProvenance.Recorded: return "saved when it happened";
            case HistoryProvenance.Current: return "the forest as it is now, not as it was in the year shown";
            case HistoryProvenance.Derived: return "calculated from recorded facts when you open this screen; not saved";
            case HistoryProvenance.Reconstructed: return "pieced together from other records; the total itself was never saved";
            case HistoryProvenance.SessionOnly: return "only known during this play session; gone after you save and load";
            default: return "the save holds no such fact, so none is shown";
        }
    }

    public const string Legend = "Labels: RECORDED saved when it happened · CURRENT live forest now · DERIVED calculated from records when shown · "
        + "RECONSTRUCTED pieced together from other records · SESSION-ONLY not saved · NOT RECORDED the save holds no such fact.";

    public const string SessionOnlyBrowsingMissing = "Last year's browsing result is not recorded this session. Browse results are not saved, so they are gone after a load.";
    public const string SessionOnlyBrowsingNote = "Session only: this browsing result is not saved and will not be recorded after you load a save.";
    public const string StormReconstructionNote = "Counts, volumes and map squares were rebuilt from tree-mortality and deadwood records; the storm itself saved only its year, severity, direction and Crop Tree losses.";

    // Classifies a line from ScenarioEcologyReviewLines by what it reads. Null means the line is not one of the known
    // review lines, so the caller must not guess a label for it. A snapshot value is recorded; a change against the previous
    // snapshot is derived; lines that read live ecology or live planted juveniles are current.
    public static HistoryProvenance? EcologyLine(string line, bool hasPreviousSnapshot)
    {
        if (line.StartsWith("Canopy ") || line.StartsWith("Areas with young regeneration"))
            return hasPreviousSnapshot ? HistoryProvenance.Derived : HistoryProvenance.Recorded;
        if (line.StartsWith("Opened areas") || line.StartsWith("Planted trees:"))
            return HistoryProvenance.Current;
        return null;
    }

    // Classifies a line from ManagementHistoryText.Completion the same way. Null (the closing explanation) carries no label.
    public static HistoryProvenance? CompletionLine(string line)
    {
        if (line.StartsWith("The Scenario One objectives were met") || line.StartsWith("You carried out") || line.StartsWith("  ")
            || line.StartsWith("No commissioned work") || line.StartsWith("Living trees at completion"))
            return HistoryProvenance.Recorded;
        if (line.StartsWith("Stand, start") || line.StartsWith("Money:") || line.Contains("not caused by felling"))
            return HistoryProvenance.Derived;
        return null;
    }
}

public static class ManagementHistoryText
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string F2(float v) => v.ToString("0.00", Inv);
    private static string F1(float v) => v.ToString("0.0", Inv);

    public static string Title(InterventionSummary s) => $"Intervention {s.Ordinal} · year {s.Year}";

    public static string Work(InterventionSummary s, HistoryTextContext c)
    {
        var parts = new List<string>();
        if (s.Felled > 0) parts.Add($"{s.Felled} felled");
        if (s.Salvaged > 0) parts.Add($"{s.Salvaged} windthrown stem{Plural(s.Salvaged)} salvaged");
        if (s.Pruned > 0) parts.Add($"{s.Pruned} pruned");
        if (s.Planted > 0)
        {
            string species = string.Join(", ", s.PlantedBySpecies.Select(p => $"{p.Value} {c.SpeciesName(p.Key)}"));
            parts.Add($"{s.Planted} planted ({species}" + (s.PlantedSheltered > 0 ? $"; {s.PlantedSheltered} sheltered" : "") + ")");
        }
        if (s.RegenerationRemoved > 0) parts.Add($"{s.RegenerationRemoved} map square{Plural(s.RegenerationRemoved)} cleared of competing growth");
        string text = parts.Count > 0 ? string.Join(" · ", parts) : "Work resolved";
        if (s.TasksFailed > 0)
            text += $" · {s.TasksFailed} task{Plural(s.TasksFailed)} could not be done" + (s.FailureReasons.Count > 0 ? $" ({s.FailureReasons[0]})" : "");
        return text;
    }

    public static string Money(InterventionSummary s, HistoryTextContext c)
    {
        string text = $"Cost {c.Money(s.CostCents)} · timber income {c.Money(s.RevenueCents)} · net {c.SignedMoney(s.NetCents)}";
        if (s.MinimumJobAdjustmentCents > 0)
            text += $" (the cost includes a {c.Money(s.MinimumJobAdjustmentCents)} small-job minimum)";
        return text;
    }

    // The state when the plan was made against the state after the work and one ecological year.
    public static string DuringPlan(InterventionSummary s)
    {
        if (s.Before == null || s.After == null)
            return "The stand records either side of this plan are not available.";
        return $"That year: living trees {s.Before.livingTrees} → {s.After.livingTrees} · canopy {F2(s.Before.meanCanopy)} → {F2(s.After.meanCanopy)} · "
            + $"average forest-floor light {F2(s.Before.meanLight)} → {F2(s.After.meanLight)} · areas with young regeneration {s.Before.occupiedRegenerationCells} → {s.After.occupiedRegenerationCells}.";
    }

    // What has been recorded since, up to the latest year.
    public static string Since(InterventionSummary s, HistoryTextContext c)
    {
        if (s.After == null || s.Latest == null)
            return "No later years recorded yet: advance time and walk the forest to see how it responds.";
        ScenarioEcologicalSnapshot a = s.After, l = s.Latest;
        var text = new System.Text.StringBuilder();
        text.Append($"By year {l.year}: living trees {a.livingTrees} → {l.livingTrees} · canopy {F2(a.meanCanopy)} → {F2(l.meanCanopy)} · "
            + $"areas with young regeneration {a.occupiedRegenerationCells} → {l.occupiedRegenerationCells} · fallen deadwood {F1(a.deadwoodVolumeM3)} → {F1(l.deadwoodVolumeM3)} m³.");
        var changes = new List<string>();
        foreach (ScenarioSpeciesOutcome now in l.species.Where(x => x != null).OrderBy(x => x.speciesId, StringComparer.Ordinal))
        {
            ScenarioSpeciesOutcome then = a.species.FirstOrDefault(x => x != null && x.speciesId == now.speciesId);
            int before = then != null ? then.livingTrees : 0;
            if (before != now.livingTrees) changes.Add($"{c.SpeciesName(now.speciesId)} {before} → {now.livingTrees}");
        }
        if (changes.Count > 0) text.Append(" Living trees by species: " + string.Join(", ", changes) + ".");
        if (s.OtherDeathsSince > 0)
            text.Append($" {s.OtherDeathsSince} tree death{Plural(s.OtherDeathsSince)} recorded since that were not felled (self-thinning, windthrow or other biological death).");
        return text.ToString();
    }

    // Work counts are the recorded task events; money is summed from several records; the stand lines compare two recorded
    // snapshots. The comparison is derived, and a missing snapshot is shown as not recorded rather than filled in.
    public static HistoryProvenance WorkProvenance => HistoryProvenance.Recorded;
    public static HistoryProvenance MoneyProvenance => HistoryProvenance.Derived;
    public static HistoryProvenance DuringPlanProvenance(InterventionSummary s)
        => s.Before == null || s.After == null ? HistoryProvenance.NotRecorded : HistoryProvenance.Derived;
    public static HistoryProvenance SinceProvenance(InterventionSummary s)
        => s.After == null || s.Latest == null ? HistoryProvenance.NotRecorded : HistoryProvenance.Derived;

    public const string NotCausalNote = "These are changes recorded after the work, not proof that the work caused them: growth, seed, light and weather act in the same years.";

    // The plan is the first one: tell the player what to look for, without telling them what to do.
    public static string FirstInterventionPrompt(InterventionSummary s)
        => $"You have carried out your first intervention (year {s.Year}). Before planning another, walk the forest and look at what changed: the opening you made, "
            + "what is regenerating, and how the trees you kept are growing. Everything you do from here stays in the Management history below, so you can compare each decision with what followed.";

    public static List<string> Completion(CompletionSummary x, HistoryTextContext c)
    {
        var lines = new List<string> { $"The Scenario One objectives were met in year {x.Year}." };
        if (x.Interventions.Count > 0)
        {
            lines.Add($"You carried out {x.Interventions.Count} intervention{Plural(x.Interventions.Count)}:");
            foreach (InterventionSummary i in x.Interventions)
                lines.Add($"  {Title(i)}: {Work(i, c)}");
        }
        else lines.Add("No commissioned work was recorded before the objectives were met.");
        if (x.Baseline != null && x.AtCompletion != null)
        {
            lines.Add($"Stand, start → year {x.AtCompletion.year}: living trees {x.Baseline.livingTrees} → {x.AtCompletion.livingTrees} · basal area {F1(x.Baseline.basalAreaM2PerHa)} → {F1(x.AtCompletion.basalAreaM2PerHa)} m²/ha · "
                + $"canopy {F2(x.Baseline.meanCanopy)} → {F2(x.AtCompletion.meanCanopy)} · areas with young regeneration {x.Baseline.occupiedRegenerationCells} → {x.AtCompletion.occupiedRegenerationCells} · "
                + $"fallen deadwood {F1(x.Baseline.deadwoodVolumeM3)} → {F1(x.AtCompletion.deadwoodVolumeM3)} m³.");
            var species = x.AtCompletion.species.Where(s => s != null && s.livingTrees > 0).OrderBy(s => s.speciesId, StringComparer.Ordinal)
                .Select(s => $"{s.livingTrees} {c.SpeciesName(s.speciesId)}").ToList();
            if (species.Count > 0) lines.Add("Living trees at completion: " + string.Join(", ", species) + ".");
        }
        lines.Add($"Money: cash {c.Money(x.StartingCashCents)} → {c.Money(x.ClosingCashCents)} · timber income {c.Money(x.TotalRevenueCents)} · work and materials {c.Money(x.TotalCostCents)} · lowest cash {c.Money(x.MinimumCashCents)}.");
        if (x.TotalOtherDeaths > 0)
            lines.Add($"{x.TotalOtherDeaths} tree death{Plural(x.TotalOtherDeaths)} not caused by felling were recorded (self-thinning, windthrow or other biological death).");
        lines.Add("This describes the forest and your decisions at the year the objectives were met. It is not a score. The Century Review (year 100) is separate and you can keep managing toward it.");
        return lines;
    }

    private static string Plural(int n) => n == 1 ? "" : "s";
}

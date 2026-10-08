using System.Collections.Generic;
using CCF.Forestry.TimberYield;
using CCF.Forestry.WorkEconomy;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 2 — tree inspection. Authoritative tree state and a causal
// diagnosis. It explains the tree's condition; it never names trees to fell.
public sealed class TreeInspectionView
{
    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly VisualElement body;
    private string shownKey = "";

    public TreeInspectionView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("panel", "inspect");
        Root.pickingMode = PickingMode.Ignore;
        body = new VisualElement();
        Root.Add(body);
    }

    public void Refresh(ForestTree tree)
    {
        ForestEcologyController eco = ui.Ecology;
        // Rebuild only when something visible can have changed.
        CompetitorAssessment competitors = ui.Competitors;
        string key = tree.TreeId + "|" + tree.MarkType + "|" + tree.IsCropTree + "|" + tree.PruningLifts + "|"
            + (eco != null ? eco.EcologicalYear : 0) + "|" + tree.IsStump + "|" + (competitors != null ? competitors.Version : 0);
        if (key == shownKey)
            return;
        shownKey = key;
        body.Clear();
        CropTreeCompetitionReport report = competitors != null && competitors.ReportTree == tree ? competitors.Report : null;
        bool assessing = tree.IsCropTree && !tree.IsStump && report != null;
        // The competitor table needs a little more width than the standard card.
        Root.style.width = assessing ? new StyleLength(440f) : new StyleLength(StyleKeyword.Null);

        string species = tree.Species != null ? tree.Species.DisplayName : "Unknown species";
        UiKit.Add(body, $"{species} · {tree.TreeId}", "title");
        UiKit.Add(body, "Origin: " + RegenerationDiagnostics.TreeOriginLabel(tree.TreeId), "muted");

        VisualElement marks = UiKit.Row(body, "row");
        if (tree.IsStump) UiKit.Chip(marks, "Harvested stump");
        else if (tree.IsMarkedForFell) UiKit.Chip(marks, "■ FELL mark", "chip-fell");
        else if (tree.IsCropTree) UiKit.Chip(marks, "◆ CROP TREE", "chip-crop");
        else UiKit.Chip(marks, "No mark");
        string shelter = eco != null ? RegenerationDiagnostics.ShelterLabel(eco, tree.transform.position) : "";
        if (!string.IsNullOrEmpty(shelter)) UiKit.Chip(marks, shelter);

        VisualElement grid = UiKit.Box("stat-grid");
        body.Add(grid);
        UiKit.Stat(grid, "DBH", UiKit.F(tree.Diameter, "0.0") + " cm");
        UiKit.Stat(grid, "Height", UiKit.F(tree.Height, "0.0") + " m");
        if (!assessing)
            UiKit.Stat(grid, "Stem volume", UiKit.F(tree.BiologicalStemVolumeM3, "0.000") + " m³");
        UiKit.Stat(grid, "Age", tree.AgeYears + " yr");
        if (!tree.IsStump && eco != null)
        {
            int cell = eco.GetCellIndex(tree.transform.position);
            float light = cell >= 0 ? eco.Cells[cell].Light : 0f;
            UiKit.Stat(grid, "Light at its cell (ground)", UiKit.F(light, "0.00") + " · " + RegenerationDiagnostics.LightBand(light));
            UiKit.Stat(grid, "Competition", $"{eco.GetCompetitionLabel(tree)} (CI {UiKit.F(eco.GetCompetitionIndex(tree), "0.0")})");
            float growth = eco.GetAnnualDbhGrowth(tree);
            UiKit.Stat(grid, "DBH growth last year", growth > 0.0001f ? UiKit.F(growth, "0.00") + " cm" : "not yet recorded");
            UiKit.Stat(grid, "Wind exposure", eco.GetWindRiskLabel(tree));
        }
        UiKit.Stat(grid, "Pruning", tree.PruningLifts > 0
            ? $"{tree.PruningLifts} lift(s), clear stem {UiKit.F(tree.CrownBaseHeightM, "0.0")} m" : "not pruned");
        if (tree.Species != null && !tree.IsStump)
        {
            float maturity = tree.Species.Maturity(tree.AgeYears);
            UiKit.Stat(grid, "Reproduction", maturity <= 0.05f ? "not yet seed-bearing" : maturity < 0.95f ? "maturing" : "seed-bearing");
        }

        if (!tree.IsStump && eco != null)
        {
            UiKit.Add(body, "Why", "heading");
            UiKit.Add(body, Diagnosis(tree, eco), "body");
        }

        if (assessing)
        {
            BuildCompetitors(report, competitors);
            UiKit.Add(body, "[1–5] Show in forest   [X] Fell   [C] Crop Tree   [E] Close", "muted");
            return;
        }
        if (!tree.IsStump && report != null && tree.IsLiving)
            UiKit.Add(body, $"Competition here comes from {report.NeighbourCount} tree{(report.NeighbourCount == 1 ? "" : "s")} within "
                + $"{UiKit.F(ForestEcologyController.HegyiCutoffMeters, "0")} m. If you want to keep this tree, mark it as a Crop Tree [C] to see which neighbours compete with it.", "muted");

        if (!tree.IsStump && tree.CanChop)
        {
            UiKit.Add(body, "If felled now — estimate", "heading");
            foreach (string line in TimberEstimate(tree))
                UiKit.Add(body, line, "body");
            UiKit.Add(body, "Approximate stem-shape model; final grading happens when the job is resolved.", "faint");
        }
        UiKit.Add(body, tree.CanChop ? "[X] Fell   [C] Crop Tree   [Tab] Work Plan   [E] Close" : "[E] Close", "muted");
    }

    // Ranked neighbours of an inspected Crop Tree and the release estimate for
    // the player's own Fell marks. Describes contribution; never names a tree to fell.
    private void BuildCompetitors(CropTreeCompetitionReport report, CompetitorAssessment competitors)
    {
        UiKit.Add(body, "Competitors of this Crop Tree", "heading");
        UiKit.Add(body, "Nearby trees that reduce its growing space. Being small or suppressed does not by itself make a tree a problem.", "muted");
        UiKit.Add(body, CropTreeCompetition.DistributionSentence(report), "body");
        int listed = competitors.ListedCount;
        if (listed > 0)
        {
            VisualElement head = UiKit.Box("table-row", "competitor-row");
            head.Add(UiKit.Text("#  Tree", "competitor-name", "faint"));
            head.Add(UiKit.Text("DBH", "competitor-num", "faint"));
            head.Add(UiKit.Text("Distance", "competitor-num", "faint"));
            head.Add(UiKit.Text("Share of competition", "competitor-share", "faint"));
            body.Add(head);
        }
        var alsoNear = new List<string>();
        for (int i = 0; i < listed; i++)
        {
            CompetitorEntry entry = report.Ranked[i];
            bool selected = i == competitors.SelectedIndex;
            string mark = entry.PlannedFell ? "  ■ Fell" : entry.CropTree ? "  ◆ Crop" : "";
            VisualElement row = UiKit.Box("table-row", "competitor-row");
            if (selected) row.AddToClassList("competitor-selected");
            row.Add(UiKit.Text($"{(selected ? "▶" : "")}{i + 1}  {entry.Id}{mark}", "competitor-name", "body"));
            row.Add(UiKit.Text(UiKit.F(entry.DbhCm, "0.0") + " cm", "competitor-num", "body"));
            row.Add(UiKit.Text(UiKit.F(entry.DistanceM, "0.0") + " m", "competitor-num", "body"));
            row.Add(UiKit.Text(Percent(entry.Share) + "  " + CropTreeCompetition.StrengthLabel(entry.Strength), "competitor-share", "body"));
            body.Add(row);
            if (entry.OtherCropTreesNearby > 0)
                alsoNear.Add($"#{i + 1} ({entry.OtherCropTreesNearby})");
        }
        if (report.NeighbourCount > listed)
            UiKit.Line(body, $"{report.NeighbourCount - listed} other nearby trees together", Percent(1f - report.ShareOfFirst(listed)), "body");
        if (alsoNear.Count > 0)
            UiKit.Add(body, "Also within 8 m of other Crop Trees: " + string.Join(", ", alsoNear), "muted");

        if (report.HasPlannedRemovals)
        {
            UiKit.Line(body, "Competition now → after your Fell marks",
                $"{UiKit.F(report.CompetitionNow, "0.00")} → {UiKit.F(report.CompetitionAfterPlanned, "0.00")} ({SignedPercent(report.ChangeFraction)})");
            UiKit.Add(body, $"{report.PlannedNeighbourCount} Fell-marked neighbour{(report.PlannedNeighbourCount == 1 ? "" : "s")} supply {Percent(report.PlannedShare)} of it. "
                + $"Growth held back: about {Percent(CropTreeCompetition.GrowthWithheld(report.CompetitionNow, competitors.TargetCi50))} now, "
                + $"{Percent(CropTreeCompetition.GrowthWithheld(report.CompetitionAfterPlanned, competitors.TargetCi50))} after. Estimate if nothing else changes.", "muted");
        }
        else
        {
            UiKit.Line(body, "Competition now", UiKit.F(report.CompetitionNow, "0.00"));
            UiKit.Add(body, "No neighbour is marked to fell. Marks you add with [X] show their effect here.", "muted");
        }
        UiKit.Add(body, "Shares come from the size and distance of trees within 8 m. Strength describes share only; it is not advice.", "faint");
    }

    private static string Percent(float fraction) => Mathf.RoundToInt(fraction * 100f).ToString(UiKit.Inv) + " %";

    private static string SignedPercent(float fraction)
    {
        int value = Mathf.RoundToInt(fraction * 100f);
        return (value > 0 ? "+" : value < 0 ? "−" : "") + Mathf.Abs(value).ToString(UiKit.Inv) + " %";
    }

    // Causal, non-prescriptive: names the limiting factor, not the remedy.
    private static string Diagnosis(ForestTree tree, ForestEcologyController eco)
    {
        float suppression = eco.GetCurrentSuppression(tree);
        string label = eco.GetCompetitionLabel(tree);
        if (label == "crowded")
            return $"Nearby stems are limiting this tree: about {Mathf.RoundToInt(suppression * 100f)}% of its potential diameter growth is withheld by competition.";
        if (label == "moderate")
            return $"Neighbours compete moderately: about {Mathf.RoundToInt(suppression * 100f)}% of potential diameter growth is withheld.";
        return "Little local competition: the tree is growing close to its potential diameter growth.";
    }

    private static IEnumerable<string> TimberEstimate(ForestTree tree)
    {
        if (tree.Species == null || tree.Species.SpeciesId != "sitka-spruce")
        {
            yield return "No timber market is configured for this species in Scenario One.";
            yield break;
        }
        double volume = tree.BiologicalStemVolumeM3;
        if (volume <= 0d || tree.Height <= 1.3f)
        {
            yield return "Too small to grade.";
            yield break;
        }
        StemMeasurements stem = MerchantableStemModel.FromMetres(tree.TreeId, tree.Species.SpeciesId, tree.Diameter, tree.Height,
            volume, "[S] Scenario One over-bark biological stem volume/diameter basis");
        stem.StemVolumeCm3 = (long)System.Math.Floor(volume * 1000000d);
        StandOperationYield result;
        try
        {
            result = TimberYieldCalculator.ResolveStandOperation("inspect-" + tree.TreeId,
                new[] { new StemYieldRequest { Stem = stem, DefaultDisposition = TimberDisposition.SellAndExtract, ExtractRetainedToRoadside = true } },
                TimberYieldDefaults.CreateSitka());
        }
        catch (System.Exception)
        {
            result = null;
        }
        if (result == null)
        {
            yield return "No estimate available.";
            yield break;
        }
        bool any = false;
        foreach (AssortmentYieldSummary product in result.Assortments)
        {
            if (product.VolumeCm3 <= 0) continue;
            any = true;
            yield return $"{product.Assortment}: {UiKit.M3(product.VolumeCm3)}";
        }
        if (result.ResidualVolumeCm3 > 0)
            yield return $"Tops and residue: {UiKit.M3(result.ResidualVolumeCm3)}";
        if (!any)
            yield return "Below merchantable size: residue only.";
    }
}

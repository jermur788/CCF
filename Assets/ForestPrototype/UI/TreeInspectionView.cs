using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.TimberYield;
using CCF.Forestry.WorkEconomy;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 2 — tree inspection. Authoritative tree state and a causal
// diagnosis. It explains the tree's condition; it never names trees to fell.
//
// The card keeps to what a decision needs: species, age, DBH, height, mark and how crowded the tree is. The rest (stem volume,
// light on the ground, growth, wind, pruning, reproduction) is one key away ([V] More detail), because the cursor is locked
// while inspecting and a panel cannot be clicked or scrolled. The felling estimate is the "If felled now" panel on the right.
public sealed class TreeInspectionView
{
    // Both side columns are this wide, so the status panel, the felling estimate and the competitor list line up on the right
    // and the card lines up with them on the left.
    public const float ColumnWidth = 400f;

    private readonly ScenarioOneUiRoot ui;
    // The left column: the card, and under it the optional detail card. Transparent; its two children are the panels.
    public VisualElement Root { get; }
    // Crop Tree competitor assessment, shown beside the card (right of the
    // screen, under the status panel) so neither panel has to scroll and the
    // centre of the view stays clear for aiming at the listed neighbours.
    public VisualElement CompetitorPanel { get; }
    public bool ShowsCompetitors { get; private set; }
    // The felling estimate takes the same slot for a tree that is not being assessed as a Crop Tree. Together with
    // the main card it would not fit one panel: the cursor is locked here, so a panel cannot scroll.
    public VisualElement EstimatePanel { get; }
    public bool ShowsEstimate { get; private set; }
    // [V] More detail. Presentation only, kept while the player moves from tree to tree.
    public bool ShowsDetail { get; private set; }
    private readonly VisualElement cardPanel, detailPanel;
    private readonly VisualElement body;
    private readonly VisualElement detailBody;
    private readonly VisualElement competitorBody;
    private readonly VisualElement estimateBody;
    private string shownKey = "";

    public TreeInspectionView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("inspect");
        Root.name = "inspect-column";
        Root.pickingMode = PickingMode.Ignore;
        // The card carries the selected-tree accent (ochre top/left edge) that matches the ring and outline on the tree.
        cardPanel = UiKit.Box("panel", "inspect-selected");
        cardPanel.name = "inspect-card";
        cardPanel.pickingMode = PickingMode.Ignore;
        body = new VisualElement { pickingMode = PickingMode.Ignore };
        cardPanel.Add(body);
        Root.Add(cardPanel);
        detailPanel = UiKit.Box("panel", "inspect-detail");
        detailPanel.name = "inspect-detail";
        detailPanel.pickingMode = PickingMode.Ignore;
        detailPanel.style.display = DisplayStyle.None;
        detailBody = new VisualElement { pickingMode = PickingMode.Ignore };
        detailPanel.Add(detailBody);
        Root.Add(detailPanel);

        CompetitorPanel = UiKit.Box("panel", "competitor-panel");
        CompetitorPanel.name = "competitor-panel";
        CompetitorPanel.pickingMode = PickingMode.Ignore;
        CompetitorPanel.style.display = DisplayStyle.None;
        CompetitorPanel.style.position = Position.Absolute;
        CompetitorPanel.style.right = 14;
        CompetitorPanel.style.top = 250;
        CompetitorPanel.style.width = ColumnWidth;
        competitorBody = new VisualElement();
        competitorBody.AddToClassList("competitor-body");
        CompetitorPanel.Add(competitorBody);

        EstimatePanel = UiKit.Box("panel", "estimate-panel");
        EstimatePanel.name = "inspect-estimate";
        EstimatePanel.pickingMode = PickingMode.Ignore;
        EstimatePanel.style.display = DisplayStyle.None;
        EstimatePanel.style.position = Position.Absolute;
        EstimatePanel.style.right = 14;
        EstimatePanel.style.top = 250;
        EstimatePanel.style.width = ColumnWidth;
        estimateBody = new VisualElement();
        EstimatePanel.Add(estimateBody);
    }

    // [V]: show or hide the detail card. Called by the UI root once per frame while a tree is being inspected.
    public void ToggleDetail()
    {
        ShowsDetail = !ShowsDetail;
        shownKey = "";
    }

    public void Refresh(ForestTree tree)
    {
        ForestEcologyController eco = ui.Ecology;
        // Rebuild only when something visible can have changed.
        CompetitorAssessment competitors = ui.Competitors;
        string rackPrompt = ui.Learning.RackInspectionPrompt(tree);
        string key = rackPrompt + "|" + tree.TreeId + "|" + tree.MarkType + "|" + tree.IsCropTree + "|" + tree.PruningLifts + "|"
            + (eco != null ? eco.EcologicalYear : 0) + "|" + (eco != null ? eco.StormModelVersion : 0) + "|" + tree.IsStump
            + "|" + (competitors != null ? competitors.Version : 0) + "|" + ShowsDetail;
        if (key == shownKey)
            return;
        shownKey = key;
        body.Clear();
        detailBody.Clear();
        competitorBody.Clear();
        estimateBody.Clear();
        ShowsEstimate = false;
        CropTreeCompetitionReport report = competitors != null && competitors.ReportTree == tree ? competitors.Report : null;
        bool assessing = tree.IsCropTree && !tree.IsStump && report != null;
        ShowsCompetitors = assessing;
        if (assessing)
            BuildCompetitors(report, competitors);

        string species = tree.Species != null ? tree.Species.DisplayName : "Unknown species";
        UiKit.Add(body, species, "title");
        UiKit.Add(body, $"{tree.TreeId} · {RegenerationDiagnostics.TreeOriginLabel(tree.TreeId)}", "muted");

        VisualElement marks = UiKit.Row(body, "row");
        if (tree.IsStump) UiKit.Chip(marks, "Harvested stump");
        else if (tree.IsMarkedForFell) UiKit.Chip(marks, "■ FELL mark", "chip-fell");
        else if (tree.IsCropTree) UiKit.Chip(marks, "◆ CROP TREE", "chip-crop");
        else UiKit.Chip(marks, "No mark");
        string shelter = eco != null ? RegenerationDiagnostics.ShelterLabel(eco, tree.transform.position) : "";
        if (!string.IsNullOrEmpty(shelter)) UiKit.Chip(marks, shelter);

        VisualElement grid = UiKit.Box("stat-grid");
        body.Add(grid);
        UiKit.Stat(grid, "Age", tree.AgeYears + " yr");
        UiKit.Stat(grid, "DBH", UiKit.F(tree.Diameter, "0.0") + " cm");
        UiKit.Stat(grid, "Height", UiKit.F(tree.Height, "0.0") + " m");
        if (!tree.IsStump && eco != null)
            UiKit.Stat(grid, "Growing space", Capitalise(eco.GetCompetitionLabel(tree)));
        if (!tree.IsStump && eco != null)
            UiKit.Add(body, Diagnosis(tree, eco), "body");

        if (!string.IsNullOrEmpty(rackPrompt)) UiKit.Add(body, rackPrompt, "body");
        BuildDetail(tree, eco, report);
        // During the opening rack stage the card does not advertise marking; it is introduced once the racks are open.
        string marking = ui.Manager != null && ui.Manager.RackFirstStageActive ? "" : "[X] Fell  [C] Crop Tree  ";

        if (assessing)
        {
            // A kept tree needs no felling estimate; its competitors are listed in the side panel.
            UiKit.Add(body, "Competitors are listed on the right.  [1–5] Show in forest  " + marking + "[V] " + (ShowsDetail ? "Less" : "More") + " detail  [E] Close", "inspect-hint");
            return;
        }

        if (!tree.IsStump && tree.CanChop)
        {
            ShowsEstimate = true;
            UiKit.Add(estimateBody, "If felled now", "heading");
            UiKit.Line(estimateBody, "Stem volume", UiKit.F(tree.BiologicalStemVolumeM3, "0.000") + " m³");
            foreach (string line in TimberEstimate(tree, ui.Manager != null && ui.Manager.BroadleafMarketActive))
                UiKit.Add(estimateBody, line, "body");
            UiKit.Add(estimateBody, "Approximate stem-shape model; final grading happens when the job is resolved.", "faint");
        }
        UiKit.Add(body, tree.CanChop
            ? marking + "[V] " + (ShowsDetail ? "Less" : "More") + " detail  [Tab] Work Plan  [E] Close"
            : "[V] " + (ShowsDetail ? "Less" : "More") + " detail  [E] Close", "inspect-hint");
    }

    // Everything that is not needed to decide: the second card, shown only on [V].
    private void BuildDetail(ForestTree tree, ForestEcologyController eco, CropTreeCompetitionReport report)
    {
        detailPanel.style.display = ShowsDetail ? DisplayStyle.Flex : DisplayStyle.None;
        if (!ShowsDetail) return;
        UiKit.Add(detailBody, "More detail", "heading");
        VisualElement grid = UiKit.Box("stat-grid");
        detailBody.Add(grid);
        UiKit.Stat(grid, "Stem volume", UiKit.F(tree.BiologicalStemVolumeM3, "0.000") + " m³");
        if (!tree.IsStump && eco != null)
        {
            int cell = eco.GetCellIndex(tree.transform.position);
            float light = cell >= 0 ? eco.Cells[cell].Light : 0f;
            UiKit.Stat(grid, "Light at ground level here", UiKit.F(light, "0.00") + " · " + RegenerationDiagnostics.LightBand(light));
            // "CI" is spelled out: the first thing a beginner meets here is an unexplained abbreviation otherwise.
            UiKit.Stat(grid, "Competition", $"{eco.GetCompetitionLabel(tree)} (competition index {UiKit.F(eco.GetCompetitionIndex(tree), "0.0")})");
            float growth = eco.GetAnnualDbhGrowth(tree);
            UiKit.Stat(grid, "DBH growth last year", growth > 0.0001f ? UiKit.F(growth, "0.00") + " cm" : "not yet recorded");
            UiKit.Stat(grid, "Wind exposure", eco.GetStormExposureLabel(tree));
        }
        UiKit.Stat(grid, "Pruning", tree.PruningLifts > 0
            ? $"{tree.PruningLifts} lift(s), clear stem {UiKit.F(tree.CrownBaseHeightM, "0.0")} m" : "not pruned");
        if (tree.Species != null && !tree.IsStump)
        {
            float maturity = tree.Species.Maturity(tree.AgeYears);
            UiKit.Stat(grid, "Reproduction", maturity <= 0.05f ? "not yet seed-bearing" : maturity < 0.95f ? "maturing" : "seed-bearing");
        }
        if (!tree.IsStump && eco != null)
            UiKit.Add(detailBody, eco.GetStormExposureExplanation(tree), "muted");
        if (!tree.IsStump && tree.Species != null && tree.Species.SpeciesId != "sitka-spruce")
            UiKit.Add(detailBody, ScenarioOneUiFacts.SharedSpeciesRulesNote + (eco != null && eco.StormModelVersion == StormModel.None ? " Storm events are off in this forest." : " Storm events are a simplified shared model, not a species wind-tolerance band."), "muted");
        if (!tree.IsStump && report != null && tree.IsLiving && !tree.IsCropTree)
            UiKit.Add(detailBody, $"Competition here comes from {report.NeighbourCount} tree{(report.NeighbourCount == 1 ? "" : "s")} within "
                + $"{UiKit.F(ForestEcologyController.HegyiCutoffMeters, "0")} m. To see which neighbours compete with it, mark it as a Crop Tree [C].", "muted");
    }

    private static string Capitalise(string text) => string.IsNullOrEmpty(text) ? text : char.ToUpperInvariant(text[0]) + text.Substring(1);

    // Ranked neighbours of an inspected Crop Tree and the release estimate for
    // the player's own Fell marks. Describes contribution; never names a tree to fell.
    private void BuildCompetitors(CropTreeCompetitionReport report, CompetitorAssessment competitors)
    {
        UiKit.Add(competitorBody, "Competitors of this Crop Tree", "heading");
        UiKit.Add(competitorBody, "Nearby trees that reduce its growing space. Being small or suppressed does not by itself make a tree a problem.", "muted");
        UiKit.Add(competitorBody, CropTreeCompetition.DistributionSentence(report), "body");
        int listed = competitors.ListedCount;
        if (listed > 0)
        {
            VisualElement head = UiKit.Box("table-row", "competitor-row");
            head.Add(UiKit.Text("#  Tree", "competitor-name", "faint"));
            head.Add(UiKit.Text("DBH", "competitor-num", "faint"));
            head.Add(UiKit.Text("Distance", "competitor-num", "faint"));
            head.Add(UiKit.Text("Share", "competitor-share", "faint")); // the full "Share of competition" never fitted this column on one line
            competitorBody.Add(head);
        }
        var alsoNear = new List<string>();
        for (int i = 0; i < listed; i++)
        {
            CompetitorEntry entry = report.Ranked[i];
            bool selected = i == competitors.SelectedIndex;
            string mark = entry.PlannedFell ? " ■ Fell" : entry.CropTree ? " ◆ Crop" : "";
            VisualElement row = UiKit.Box("table-row", "competitor-row");
            if (selected) row.AddToClassList("competitor-selected");
            row.Add(UiKit.Text($"{(selected ? "▶" : "")}{i + 1}  {entry.Id}{mark}", "competitor-name", "body"));
            row.Add(UiKit.Text(UiKit.F(entry.DbhCm, "0.0") + " cm", "competitor-num", "body"));
            row.Add(UiKit.Text(UiKit.F(entry.DistanceM, "0.0") + " m", "competitor-num", "body"));
            row.Add(UiKit.Text(Percent(entry.Share) + " " + CropTreeCompetition.StrengthLabel(entry.Strength), "competitor-share", "body"));
            competitorBody.Add(row);
            if (entry.OtherCropTreesNearby > 0)
                alsoNear.Add($"#{i + 1} ({entry.OtherCropTreesNearby})");
        }
        if (report.NeighbourCount > listed)
            UiKit.Line(competitorBody, $"{report.NeighbourCount - listed} other nearby trees together", Percent(1f - report.ShareOfFirst(listed)), "body");
        if (alsoNear.Count > 0)
            UiKit.Add(competitorBody, "Also within 8 m of other Crop Trees: " + string.Join(", ", alsoNear), "muted");

        if (report.HasPlannedRemovals)
        {
            UiKit.Line(competitorBody, "Competition now → after planned fells",
                $"{UiKit.F(report.CompetitionNow, "0.00")} → {UiKit.F(report.CompetitionAfterPlanned, "0.00")} ({SignedPercent(report.ChangeFraction)})");
            UiKit.Add(competitorBody, $"{report.PlannedNeighbourCount} planned-fell neighbour{(report.PlannedNeighbourCount == 1 ? " supplies" : "s supply")} {Percent(report.PlannedShare)} of it. "
                + $"Growth held back: about {Percent(CropTreeCompetition.GrowthWithheld(report.CompetitionNow, competitors.TargetCi50))} now, "
                + $"{Percent(CropTreeCompetition.GrowthWithheld(report.CompetitionAfterPlanned, competitors.TargetCi50))} after. Estimate if nothing else changes.", "muted");
        }
        else
        {
            UiKit.Line(competitorBody, "Competition now", UiKit.F(report.CompetitionNow, "0.00"));
            UiKit.Add(competitorBody, "No neighbour is planned to fell. Mark trees with [X] and add them to the Work Plan [Tab] to see their effect here.", "muted");
        }
        UiKit.Add(competitorBody, "Shares come from the size and distance of trees within 8 m. Strength describes share only; it is not advice.", "faint");
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

    private static IEnumerable<string> TimberEstimate(ForestTree tree, bool broadleafMarket)
    {
        string speciesId = tree.Species != null ? tree.Species.SpeciesId : "";
        bool sitka = speciesId == "sitka-spruce";
        TimberYieldConfiguration config = broadleafMarket ? TimberYieldDefaults.CreateScenarioOne() : TimberYieldDefaults.CreateSitka();
        if (!config.Specifications.Any(spec => spec.SpeciesId == speciesId))
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
        // Same grading input as the harvest quote: for Oak, the recorded pruning history is the stem's clear section.
        bool qualityDependent = config.Specifications.Any(spec => spec.SpeciesId == speciesId && spec.RequiredQuality != StemQualityFlags.None);
        if (qualityDependent)
            stem.QualitySections = ScenarioOneEconomyAdapter.PrunedSections(tree, stem.TotalHeightMm);
        StandOperationYield result;
        try
        {
            result = TimberYieldCalculator.ResolveStandOperation("inspect-" + tree.TreeId,
                new[] { new StemYieldRequest { Stem = stem, DefaultDisposition = TimberDisposition.SellAndExtract, ExtractRetainedToRoadside = true } },
                config);
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
            yield return $"{TimberAssortmentLabels.Label(product.Assortment)}: {UiKit.M3(product.VolumeCm3)}";
        }
        if (result.ResidualVolumeCm3 > 0)
            yield return $"Tops and residue: {UiKit.M3(result.ResidualVolumeCm3)}";
        if (!any)
            yield return "Below merchantable size: residue only.";
        if (sitka)
            yield break;
        // Oak and Beech: say why the grade is what it is, in terms of things the player can change or wait for.
        yield return ScenarioOneUiFacts.BroadleafQualityLimit + " Pruning changes which grades a log can reach; it does not set a price.";
        bool premium = result.Stems.SelectMany(stem => stem.Logs).Any(log => log.Assortment == TimberAssortment.VeneerPlanking || log.Assortment == TimberAssortment.Beam);
        if (speciesId == TimberYieldDefaults.OakSpeciesId)
        {
            if (tree.PruningLifts <= 0)
                yield return "Not pruned: the clear first-grade veneer/planking class is unavailable. Pruning a Crop Tree makes its pruned section eligible once it is large enough.";
            else
                yield return $"Pruned to {UiKit.F(tree.CrownBaseHeightM, "0.0")} m: logs inside that section can become eligible for first-grade veneer/planking once they are large enough.";
            if (!premium)
                yield return "Size thresholds are about 45 cm mid-log diameter for veneer/planking and 30 cm for beams. " + ScenarioOneUiFacts.PremiumOakNote;
        }
        else
            yield return "Beech here is graded to second-quality planking or firewood; it has no veneer, beam or fencing class.";
    }
}

using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.TimberYield;
using CCF.Forestry.WorkEconomy;
using UnityEngine;
using UnityEngine.UIElements;

// One selected-tree card. V cycles fixed pages because a locked cursor cannot scroll.
// All numbers are read from the existing tree/competition reports; no forestry state is changed here.
public sealed class TreeInspectionView
{
    public const float ColumnWidth = 440f;
    public const float EstimateWidth = ColumnWidth;
    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    // Retained as named content slots for verification; both live INSIDE the one card.
    public VisualElement CompetitorPanel { get; }
    public VisualElement EstimatePanel { get; }
    public bool ShowsCompetitors { get; private set; }
    public bool ShowsEstimate { get; private set; }
    public bool ShowsDetail => page > 0;
    public int PageCount { get; private set; } = 1;
    public int Page => page;
    private int page;
    private readonly VisualElement body;
    private string shownKey = "";
    private string shownTree = "";
    private List<string> timber = new List<string>();
    public string ControlHints { get; private set; } = "[V] Details · [E] Close";

    public TreeInspectionView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("panel", "inspect", "inspect-selected");
        Root.name = "inspect-column";
        Root.pickingMode = PickingMode.Ignore;
        body = UiKit.Box("inspection-page");
        Root.Add(body);
        CompetitorPanel = UiKit.Box("inspection-neighbours");
        CompetitorPanel.name = "competitor-panel";
        EstimatePanel = UiKit.Box("inspection-estimate");
        EstimatePanel.name = "inspect-estimate";
    }

    public static int NextPage(int current, int count, int direction = 1)
        => count <= 0 ? 0 : ((current + direction) % count + count) % count;

    public void ToggleDetail() => StepDetail(1);
    public void StepDetail(int direction)
    {
        page = NextPage(page, PageCount, direction);
        shownKey = "";
    }

    // Only real available ranks are advertised. Marking is still owned by the original handlers:
    // X acts on a chosen neighbour, otherwise the aimed trunk; C acts on the aimed trunk.
    public static string NeighbourHint(int count) => count <= 0 ? ""
        : (count == 1 ? "[1] Neighbour" : $"[1–{count}] Neighbours");

    public void Refresh(ForestTree tree)
    {
        ForestEcologyController eco = ui.Ecology;
        CompetitorAssessment competitors = ui.Competitors;
        string rackPrompt = ui.Learning.RackInspectionPrompt(tree);
        StandMapTreatmentReview treatmentReview = ui.Map != null ? ui.Map.Review : null;
        treatmentReview?.OnInspected(tree);
        ScenarioOneTreatmentReview.TreeContext reviewContext = treatmentReview != null ? treatmentReview.ContextFor(tree) : null;
        CropTreeCompetitionReport report = competitors != null && competitors.ReportTree == tree ? competitors.Report : null;
        bool assessing = tree.IsCropTree && !tree.IsStump && report != null;
        if (shownTree != tree.TreeId) { shownTree = tree.TreeId; page = 0; }
        // Overview, site, condition/wind, then every neighbour (three per page), draft and timber pages.
        int neighbourPages = assessing ? (report.NeighbourCount + 2) / 3 : 0;
        bool rackFirst = ui.Manager != null && ui.Manager.RackFirstStageActive;
        bool locked = UnityEngine.Cursor.lockState == CursorLockMode.Locked;
        int listed = assessing ? competitors.ListedCount : 0;
        ForestTree aimed = ui.Marking != null ? ui.Marking.AimedTree : null;
        ForestTree fellTarget = competitors != null && competitors.SelectedTree != null ? competitors.SelectedTree : aimed;
        bool markTarget = fellTarget != null && fellTarget.IsLiving && !fellTarget.IsCropTree;
        bool cropTarget = aimed != null && aimed.IsLiving;
        string treatment = ui.Marking != null ? ui.Marking.TreatmentOutcome ?? "" : "";
        string key = rackPrompt + "|" + tree.TreeId + "|" + tree.MarkType + "|" + tree.PruningLifts + "|"
            + (eco != null ? eco.EcologicalYear : 0) + "|" + (eco != null ? eco.StormModelVersion : 0) + "|" + tree.IsStump
            + "|" + (competitors != null ? competitors.Version : 0) + "|" + page + "|" + treatment
            + "|" + (reviewContext != null ? reviewContext.Key + reviewContext.Chip : "");
        bool rebuild = key != shownKey;
        if (rebuild)
        {
            timber = !tree.IsStump && tree.CanChop
                ? TimberEstimate(tree, ui.Manager != null && ui.Manager.BroadleafMarketActive).ToList() : new List<string>();
            int timberPages = (timber.Count + 1) / 2;
            PageCount = 4 + neighbourPages + timberPages;
            page = Mathf.Clamp(page, 0, PageCount - 1);
            shownKey = key;
        }
        ControlHints = (locked ? NeighbourHint(listed) + (listed > 0 ? " · " : "") : "")
            + $"[V] Next detail ({page + 1}/{PageCount}) · [Shift+V] Previous"
            + (!rackFirst && locked && markTarget ? " · [X] " + (fellTarget.IsMarkedForFell ? "Remove Fell mark " : "Mark to fell ") + fellTarget.TreeId : "")
            + (!rackFirst && locked && cropTarget ? " · [C] " + (aimed.IsCropTree ? "Unmark Crop Tree " : "Crop Tree ") + aimed.TreeId : "") + " · [E] Close";
        if (!string.IsNullOrEmpty(rackPrompt)) ControlHints += " · " + rackPrompt;
        if (!rebuild) return;
        body.Clear();
        CompetitorPanel.Clear();
        EstimatePanel.Clear();
        ShowsCompetitors = assessing;
        ShowsEstimate = timber.Count > 0;
        UiKit.Add(body, "SELECTED TREE", "eyebrow");
        UiKit.Add(body, tree.Species != null ? tree.Species.DisplayName : "Unknown species", "title");
        UiKit.Add(body, tree.TreeId + " · " + RegenerationDiagnostics.TreeOriginLabel(tree.TreeId), "muted");
        VisualElement marks = UiKit.Row(body, "row-wrap");
        if (tree.IsStump) UiKit.Chip(marks, "Harvested stump");
        else if (tree.IsMarkedForFell) UiKit.Chip(marks, "■ FELL mark", "chip-fell");
        else if (tree.IsCropTree) UiKit.Chip(marks, "◆ CROP TREE", "chip-crop");
        else UiKit.Chip(marks, "No mark");
        if (reviewContext != null) UiKit.Chip(marks, reviewContext.Chip, "chip-proposal");
        if (page == 0)
        {
            UiKit.Add(body, $"Age {tree.AgeYears} yr · DBH {UiKit.F(tree.Diameter, "0.0")} cm · {UiKit.F(tree.Height, "0.0")} m tall", "body");
            if (!tree.IsStump && eco != null) UiKit.Add(body, Diagnosis(tree, eco), "body");
            if (!string.IsNullOrEmpty(rackPrompt)) UiKit.Add(body, rackPrompt, "body");
            if (assessing) BuildNeighbours(report, competitors, 0, 3, false);
            UiKit.Add(body, "Your draft plan", "heading");
            if (assessing && report.HasPlannedRemovals)
                UiKit.Add(body, $"Competition index {UiKit.F(report.CompetitionNow, "0.00")} → {UiKit.F(report.CompetitionAfterPlanned, "0.00")} if valid open felling jobs are carried out. This is an estimate before execution.", "body");
            else UiKit.Add(body, "No valid open neighbour-felling jobs in this estimate. Open Work Plan to review unqueued marks; [V] shows draft effects.", "muted");
            if (!string.IsNullOrEmpty(treatment)) UiKit.Add(body, "Fell-mark forecast: " + treatment, "draft-warning");
            if (eco != null && eco.StormModelVersion == StormModel.None && !string.IsNullOrEmpty(treatment))
                UiKit.Add(body, "Storm events are off; exposure does not establish wind safety.", "faint");
            if (reviewContext != null && !string.IsNullOrEmpty(reviewContext.Line)) UiKit.Add(body, reviewContext.Line, "muted");
        }
        else if (page == 1) BuildSite(tree, eco);
        else if (page == 2) BuildCondition(tree, eco, report, reviewContext);
        else if (page < 3 + neighbourPages)
            BuildNeighbours(report, competitors, (page - 3) * 3, 3, true);
        else if (page == 3 + neighbourPages) BuildDraft(report, competitors, assessing, treatment);
        else
        {
            body.Add(EstimatePanel);
            UiKit.Add(EstimatePanel, "If felled now · timber estimate", "heading");
            int start = (page - 4 - neighbourPages) * 2;
            foreach (string line in timber.Skip(start).Take(2)) UiKit.Add(EstimatePanel, line, "body");
            UiKit.Add(EstimatePanel, "Approximate stem-shape model; final grading happens when the job is resolved. This is no instruction to fell.", "faint");
        }
        UiKit.Add(body, $"Page {page + 1} of {PageCount} · [V] Next · [Shift+V] Previous · overview follows last page", "inspect-hint");
        foreach (VisualElement element in Root.Query<VisualElement>().ToList()) element.pickingMode = PickingMode.Ignore;
    }

    private void BuildSite(ForestTree tree, ForestEcologyController eco)
    {
        UiKit.Add(body, "Site and growth", "heading");
        UiKit.Line(body, "Stem volume", UiKit.F(tree.BiologicalStemVolumeM3, "0.000") + " m³");
        if (tree.IsStump || eco == null) return;
        int cell = eco.GetCellIndex(tree.transform.position);
        float light = cell >= 0 ? eco.Cells[cell].Light : 0f;
        UiKit.Line(body, "Light at ground level", UiKit.F(light, "0.00") + " · " + RegenerationDiagnostics.LightBand(light));
        UiKit.Line(body, "Competition index", UiKit.F(eco.GetCompetitionIndex(tree), "0.00"));
        UiKit.Add(body, "Growing space: " + eco.GetCompetitionLabel(tree), "body");
        float growth = eco.GetAnnualDbhGrowth(tree);
        UiKit.Line(body, "DBH growth last year", growth > 0.0001f ? UiKit.F(growth, "0.00") + " cm" : "not yet recorded");
        string shelter = RegenerationDiagnostics.ShelterLabel(eco, tree.transform.position);
        if (!string.IsNullOrEmpty(shelter)) UiKit.Add(body, shelter, "body");
        if (cell >= 0) UiKit.Add(body, "Regeneration here: " + ScenarioOneUiFacts.RegenerationSummary(eco, cell, ui.Manager.PlantedJuveniles), "muted");
    }

    private void BuildCondition(ForestTree tree, ForestEcologyController eco, CropTreeCompetitionReport report,
        ScenarioOneTreatmentReview.TreeContext reviewContext)
    {
        UiKit.Add(body, "Condition and model limits", "heading");
        UiKit.Add(body, tree.PruningLifts > 0 ? $"Pruning: {tree.PruningLifts} lift(s), clear stem {UiKit.F(tree.CrownBaseHeightM, "0.0")} m" : "Not pruned", "body");
        if (!tree.IsStump && tree.Species != null)
        {
            float maturity = tree.Species.Maturity(tree.AgeYears);
            UiKit.Add(body, "Reproduction: " + (maturity <= 0.05f ? "not yet seed-bearing" : maturity < 0.95f ? "maturing" : "seed-bearing"), "body");
        }
        if (!tree.IsStump && eco != null)
        {
            UiKit.Add(body, "Wind exposure: " + eco.GetStormExposureLabel(tree), "body");
            UiKit.Add(body, eco.GetStormExposureExplanation(tree), "muted");
            if (eco.StormModelVersion == StormModel.None) UiKit.Add(body, "Storm events are off in this forest. This is not a wind-safety assessment.", "muted");
            if (tree.Species != null && tree.Species.SpeciesId != "sitka-spruce") UiKit.Add(body, ScenarioOneUiFacts.SharedSpeciesRulesNote, "muted");
        }
        if (reviewContext != null && !string.IsNullOrEmpty(reviewContext.Hint)) UiKit.Add(body, reviewContext.Hint, "muted");
        if (report != null) UiKit.Add(body, $"Competition uses {report.NeighbourCount} neighbours within {UiKit.F(ForestEcologyController.HegyiCutoffMeters, "0")} m. Size and distance determine each share; share is not advice to fell.", "faint");
    }

    private void BuildNeighbours(CropTreeCompetitionReport report, CompetitorAssessment competitors, int start, int count, bool detail)
    {
        body.Add(CompetitorPanel);
        UiKit.Add(CompetitorPanel, "Local competition · neighbour shares", "heading");
        int end = Mathf.Min(report.NeighbourCount, start + count);
        for (int i = start; i < end; i++)
        {
            CompetitorEntry entry = report.Ranked[i];
            string mark = entry.PlannedFell ? " ■ Open fell job" : entry.CropTree ? " ◆ Crop Tree" : "";
            VisualElement row = UiKit.Box("competition-line");
            if (i == competitors.SelectedIndex) row.AddToClassList("competitor-selected");
            UiKit.Line(row, $"{i + 1} · {entry.Id}{mark} · {UiKit.F(entry.DistanceM, "0.0")} m", Percent(entry.Share));
            if (detail)
            {
                string nearby = i < competitors.ListedCount ? $"near {entry.OtherCropTreesNearby} other Crop Tree(s)"
                    : "proximity to other Crop Trees not assessed at this rank";
                UiKit.Add(row, $"DBH {UiKit.F(entry.DbhCm, "0.0")} cm · index contribution {UiKit.F(entry.Contribution, "0.000")} · {CropTreeCompetition.StrengthLabel(entry.Strength)} share · {nearby}", "muted");
            }
            CompetitorPanel.Add(row);
        }
        if (report.NeighbourCount > end && start == 0)
            UiKit.Line(CompetitorPanel, $"Other neighbours ({report.NeighbourCount - end})", report.CompetitionNow > 0f ? Percent(Mathf.Clamp01(1f - report.ShareOfFirst(end))) : "No measured competition");
        if (report.NeighbourCount == 0) UiKit.Add(CompetitorPanel, "No neighbours inside the competition radius.", "body");
        UiKit.Add(CompetitorPanel, "Shares of the local competition index, not percentages of diameter growth. Rounded independently.", "faint");
    }

    private void BuildDraft(CropTreeCompetitionReport report, CompetitorAssessment competitors, bool assessing, string treatment)
    {
        UiKit.Add(body, "Your draft plan · before execution", "heading");
        if (assessing)
        {
            UiKit.Line(body, "Competition index now → with open felling jobs", $"{UiKit.F(report.CompetitionNow, "0.00")} → {UiKit.F(report.CompetitionAfterPlanned, "0.00")}");
            UiKit.Add(body, $"{report.PlannedNeighbourCount} neighbours with valid open felling jobs contribute {Percent(report.PlannedShare)} of the index. Potential diameter growth withheld: about {Percent(CropTreeCompetition.GrowthWithheld(report.CompetitionNow, competitors.TargetCi50))} now → {Percent(CropTreeCompetition.GrowthWithheld(report.CompetitionAfterPlanned, competitors.TargetCi50))} with those jobs. Estimate if nothing else changes; values are rounded, so small changes may not be visible.", "body");
        }
        else UiKit.Add(body, "No local Crop Tree release estimate is available for this selection.", "muted");
        UiKit.Add(body, string.IsNullOrEmpty(treatment) ? "No stand-wide Fell-mark forecast is currently available." : "Fell-mark forecast: " + treatment, "body");
        UiKit.Add(body, "The competition estimate uses valid open felling jobs (pending or approved). An unqueued Fell mark or MFM proposal adds no removal to that estimate. Open Work Plan to review and approve jobs; year advance executes approved work. Forecasts compare with the current forest, not a saved pre-mark baseline.", "muted");
    }

    private static string Percent(float fraction) => Mathf.RoundToInt(fraction * 100f).ToString(UiKit.Inv) + " %";

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

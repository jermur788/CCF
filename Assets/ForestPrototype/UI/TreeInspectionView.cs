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
        string key = tree.TreeId + "|" + tree.MarkType + "|" + tree.IsCropTree + "|" + tree.PruningLifts + "|"
            + (eco != null ? eco.EcologicalYear : 0) + "|" + tree.IsStump;
        if (key == shownKey)
            return;
        shownKey = key;
        body.Clear();

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
        if (tree.IsCropTree && !tree.IsStump)
            UiKit.Add(body, "Crop Tree: kept and given room to develop. Inspect its neighbours to see which really compete with it.", "muted");

        VisualElement grid = UiKit.Box("stat-grid");
        body.Add(grid);
        UiKit.Stat(grid, "DBH", UiKit.F(tree.Diameter, "0.0") + " cm");
        UiKit.Stat(grid, "Height", UiKit.F(tree.Height, "0.0") + " m");
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

        if (!tree.IsStump && tree.CanChop)
        {
            UiKit.Add(body, "If felled now — estimate", "heading");
            foreach (string line in TimberEstimate(tree))
                UiKit.Add(body, line, "body");
            UiKit.Add(body, "Approximate stem-shape model; final grading happens when the job is resolved.", "faint");
        }
        UiKit.Add(body, tree.CanChop ? "[C] Crop Tree   [X] Fell   [Tab] Work Plan   [E] Close   [F1] Help" : "[E] Close", "muted");
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

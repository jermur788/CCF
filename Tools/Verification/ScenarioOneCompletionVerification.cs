#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Scenario 1 end-to-end completion harness (draft; Docs/Scenario1EcologyCompletion.md).
//
// Copy into Assets/ForestPrototype for an explicit batch run, then remove the
// copy and its .meta:  -executeMethod ScenarioOneCompletionVerification.Begin
// It never writes the save slot (in-memory CaptureData/LoadData only) and
// restores the scene world at the end. Designed for two-process execution:
// compare SCENARIO_ONE_COMPLETION_HASH between runs.
//
// Final connected-system gate: live grouped harvest economy, timber yield,
// Contractor/LandownerSimulated planting with live shelters (timing contract),
// save v15 restore (protection replaced, never duplicated), Reference preview
// isolation and v14 compatibility. TODO-INTEGRATION reporting remains for any
// future phase that cannot yet be proven; none is used in the final gate.
public static class ScenarioOneCompletionVerification
{
    private const string Requested = "ScenarioOneCompletionVerification.Requested";

    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("Scenario One completion gate").AddComponent<ScenarioOneCompletionGate>();
    }
}

public sealed class ScenarioOneCompletionGate : MonoBehaviour
{
    private const string Sitka = "sitka-spruce", Oak = "sessile-oak", Beech = "beech";
    private const string OakItem = "sessile-oak-sapling", BeechItem = "beech-sapling";
    private const float ThinningBasalAreaFraction = 0.27f;

    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestSaveData original;
    private float scenarioPressure;
    private readonly List<string> passed = new List<string>(), todo = new List<string>();
    private readonly StringBuilder evidence = new StringBuilder();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static string F(float v) => v.ToString("0.000", CultureInfo.InvariantCulture);

    private void Pass(string phase, string detail)
    {
        passed.Add(phase);
        string line = $"SCENARIO_ONE_COMPLETION_PHASE {phase} PASS {detail}";
        evidence.AppendLine(line);
        Debug.Log(line);
    }

    private void Todo(string phase, string what)
    {
        todo.Add(phase);
        Debug.Log($"SCENARIO_ONE_COMPLETION_PHASE {phase} TODO-INTEGRATION {what}");
    }

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        foreach (IEnumerator step in new[] { Verify(), Cleanup() })
        {
            while (true)
            {
                bool more; object current = null;
                try { more = step.MoveNext(); if (more) current = step.Current; }
                catch (Exception error) { failure = failure ?? error; break; }
                if (!more) break;
                yield return current;
            }
        }
        if (failure != null)
            Debug.LogError("SCENARIO_ONE_COMPLETION_VERIFY_FAIL " + failure);
        else if (todo.Count > 0)
            Debug.Log($"SCENARIO_ONE_COMPLETION_ENABLED_PHASES_PASS passed={string.Join(",", passed)} todoIntegration={string.Join(",", todo)}");
        else
            Debug.Log("SCENARIO_ONE_COMPLETION_VERIFY_PASS");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Verify()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ScenarioProtectionVisuals visuals = FindFirstObjectByType<ScenarioProtectionVisuals>();
        Check(manager != null && ecology != null && saves != null && marking != null && spawner != null && visuals != null, "scene systems missing");
        // The playthrough runs the new-game default (RNG model 1). Setting
        // CCF_RNG_MODEL=0 replays legacy model 0, which reproduces the
        // pre-model-1 completion anchor 568922E1A6D73CDD.
        string forcedModel = Environment.GetEnvironmentVariable("CCF_RNG_MODEL");
        if (!string.IsNullOrEmpty(forcedModel))
            ecology.RngModelVersion = int.Parse(forcedModel, CultureInfo.InvariantCulture);
        // CCF_REGEN_MODEL=0 replays legacy single-cohort regeneration. Legacy
        // anchors: RNG 0 + regen 0 = 568922E1A6D73CDD; RNG 1 + regen 0 = 00479F18970F9926.
        string forcedRegeneration = Environment.GetEnvironmentVariable("CCF_REGEN_MODEL");
        if (!string.IsNullOrEmpty(forcedRegeneration))
            ecology.RegenerationModelVersion = int.Parse(forcedRegeneration, CultureInfo.InvariantCulture);
        Debug.Log($"SCENARIO_ONE_COMPLETION_RNG_MODEL {ecology.RngModelVersion} forced={(string.IsNullOrEmpty(forcedModel) ? "no" : "yes")}");
        // CCF_GROWTH_MODEL=0 replays legacy adult growth (no adult mortality);
        // with it, regeneration-model-1 games reproduce 6F84AF319D301F87 (v16 layout).
        string forcedGrowth = Environment.GetEnvironmentVariable("CCF_GROWTH_MODEL");
        if (!string.IsNullOrEmpty(forcedGrowth))
            ecology.GrowthModelVersion = int.Parse(forcedGrowth, CultureInfo.InvariantCulture);
        Debug.Log($"SCENARIO_ONE_COMPLETION_GROWTH_MODEL {ecology.GrowthModelVersion} forced={(string.IsNullOrEmpty(forcedGrowth) ? "no" : "yes")}");
        Debug.Log($"SCENARIO_ONE_COMPLETION_REGEN_MODEL {ecology.RegenerationModelVersion} forced={(string.IsNullOrEmpty(forcedRegeneration) ? "no" : "yes")}");
        original = saves.CaptureData();
        scenarioPressure = ecology.Browsing.BackgroundPressure;
        ScenarioOneDefinition definition = manager.Definition;

        // ---- P0 START ----
        List<ForestTree> living = Living();
        Check(definition != null && ecology.EcologicalYear == 0 && manager.Outcome == ScenarioOneOutcome.Active,
            "Scenario One is not at a fresh Year 0 (run with an isolated config; no save is auto-loaded)");
        Check(living.Count == 336 && living.All(t => t.Species != null && t.Species.SpeciesId == Sitka), "start is not 336 living Sitka");
        Check(manager.CashCents == definition.StartingCashCents && manager.CashCents == 1200000, "starting cash differs from the definition");
        Check(scenarioPressure == definition.BackgroundBrowsePressure && scenarioPressure == 0.2f, "browse pressure is not the 0.2 Scenario One calibration");
        Check(!ecology.Browsing.HasProtection && manager.Shelters.Count == 0, "protection present at start");
        Check(ForestSaveData.CurrentVersion == 19 && definition.MinimumHarvestJobCents == 250000 && manager.OwnerMinutesPerYear == 2400,
            "production conventions (v18, EUR 2,500 minimum, 2,400 owner min/yr)");
        Pass("P0", $"trees=336 year=0 cash={manager.CashCents} pressure={F(scenarioPressure)} shelters=0 save=v{ForestSaveData.CurrentVersion} minimum={definition.MinimumHarvestJobCents} ownerMin={manager.OwnerMinutesPerYear}");

        // ---- P1 INSPECT ----
        ForestTree sample = living.First(t => t.TreeId == "P0000");
        Check(sample.Height > 0f && sample.Diameter > 0f && sample.CrownRadius > 0f, "tree inspection data invalid");
        Check(RegenerationDiagnostics.TreeOriginLabel(sample.TreeId) == "original plantation", "inspection origin");
        Vector3 groundPoint = CellCenter(0);
        string report = ecology.RegenerationReportLine(groundPoint);
        Check(report.Contains("light") && report.Contains("browsing low"), "ground report lacks light/browsing: " + report);
        RegenerationDiagnosis diagnosis = RegenerationDiagnostics.Diagnose(ecology, groundPoint, manager.PlantedJuveniles, spawner);
        Check(!string.IsNullOrEmpty(diagnosis.Summary()) && !string.IsNullOrEmpty(diagnosis.LightBand), "diagnosis produced no reason");
        Pass("P1", $"tree={sample.TreeId} h={F(sample.Height)} report='{report}' diagnosis='{diagnosis.Summary()}'");

        // ---- P2 PLAN (Year 0): crop trees, competitor-release thinning, one grouped quote ----
        float initialCanopy = ecology.Cells.Average(c => c.Canopy);
        float[] initialLight = ecology.Cells.Select(c => c.Light).ToArray();
        List<string> cropIds = SelectCropTrees(living);
        foreach (string id in cropIds) marking.Mark(living.First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        List<ForestTree> fell = SelectThinning(living, cropIds, ThinningBasalAreaFraction, out float fellFraction);
        var deadwoodIds = new HashSet<string>(fell.Take(4).Select(t => t.TreeId));
        var keptIds = new HashSet<string>(fell.Skip(4).Take(3).Select(t => t.TreeId));
        int added = 0;
        foreach (var (outcome, group) in new[] { (FellingMaterialOutcome.RetainAsFallenDeadwood, fell.Take(4)),
                     (FellingMaterialOutcome.KeepForUse, fell.Skip(4).Take(3)), (FellingMaterialOutcome.SellAndExtract, fell.Skip(7)) })
        {
            manager.PlanningFellingOutcome = outcome;
            foreach (ForestTree t in group) marking.Mark(t, TreeMarkType.Fell, false);
            added += manager.AddMarkedTreesToWorkPlan();
        }
        Check(added == fell.Count, "work plan did not take every marked felling");
        ScenarioHarvestJob quote = manager.GetHarvestQuote();
        Check(quote.Eligible && quote.Orders.Count == fell.Count && quote.Orders.Select(o => o.harvestJobId).Distinct().Count() == 1,
            "felling orders are not one grouped commissioned job");
        Check(quote.Resolution.Ledger.Count(e => e.Category == LedgerCategory.MinimumJobAdjustment) <= 1 && quote.CostCents >= definition.MinimumHarvestJobCents,
            "minimum job charge applied more than once or not at all");
        Check(quote.TotalStemVolumeCm3 == quote.SoldVolumeCm3 + quote.RetainedVolumeCm3 + quote.DeadwoodVolumeCm3 + quote.ResidualVolumeCm3,
            "harvest quote does not conserve stem volume");
        Check(quote.Task.Timber.Length > 0 && quote.SoldVolumeCm3 > 0 && quote.RevenueCents > 0, "no timber products represented");
        Check(quote.Resolution.Quote.Timber.Where(v => deadwoodIds.Contains(v.Batch.SourceTreeId)).Sum(v => v.SaleRevenueCents) == 0,
            "deadwood stems valued for sale");
        // Owner harvest is unavailable (contractor-only), with a readable reason.
        ScenarioOneWorkOrder probe = quote.Orders[0];
        probe.executionMethod = WorkExecutionMethod.LandownerSimulated;
        manager.ApprovePendingWork();   // may approve the valid remainder
        Check(probe.status != ScenarioWorkStatus.Approved && (probe.validationMessage ?? "").Contains("contractor-only"),
            "owner harvest was not refused with a contractor-only reason: " + probe.validationMessage);
        probe.executionMethod = WorkExecutionMethod.Contractor;
        Check(manager.ApprovePendingWork(), "felling work could not be approved: " + manager.Feedback);
        ScenarioHarvestJob approved = manager.GetHarvestQuote(true);
        Check(manager.ReservedContractorCashCents == approved.CostCents && approved.Orders.Count == fell.Count, "cash not reserved once for the grouped job");
        HashSet<string> felledIds = new HashSet<string>(fell.Select(t => t.TreeId));
        HashSet<int> thinnedCells = new HashSet<int>(fell.Select(t => ecology.GetCellIndex(t.transform.position)));
        long minimumAdjustment = approved.Resolution.Quote.Costs.MinimumJobAdjustmentCents;
        Pass("P2", $"crop={cropIds.Count} fell={fell.Count} basalAreaRemoved={F(fellFraction)} deadwood=4 keepForUse=3 oneJob={approved.JobId} "
            + $"cost={approved.CostCents} revenue={approved.RevenueCents} minimumAdjustment={minimumAdjustment} sold={approved.SoldVolumeCm3} retained={approved.RetainedVolumeCm3} ownerHarvest=refused");

        // ---- P3 RESOLVE (Year 1) ----
        var fellCounts = new Dictionary<string, int>();
        Action<ForestTree> onFell = t => fellCounts[t.TreeId] = (fellCounts.TryGetValue(t.TreeId, out int n) ? n : 0) + 1;
        long cashBefore = manager.CashCents;
        float keptBefore = manager.RetainedTimberM3;
        long ledger = ForestryWorkCalculator.SumLedger(approved.Resolution.Ledger);
        ForestTree.Felled += onFell;
        bool advanced;
        try { advanced = manager.AdvanceYear(); } finally { ForestTree.Felled -= onFell; }
        Check(advanced && ecology.EcologicalYear == 1, "Year-1 resolution failed: " + manager.Feedback);
        Check(fellCounts.Count == fell.Count && fellCounts.All(p => felledIds.Contains(p.Key) && p.Value == 1), "a tree was not felled exactly once");
        Check(Living().All(t => !felledIds.Contains(t.TreeId)), "a marked tree is still standing");
        Check(manager.CashCents - cashBefore == ledger, $"cash change {manager.CashCents - cashBefore} != one job ledger {ledger}");
        Check(manager.ManagementEvents.Where(e => e.year == 1 && e.taskType == ScenarioWorkType.FellTree && e.eventType == ScenarioManagementEventType.WorkResolved)
            .Sum(e => e.cashDeltaCents) == ledger, "felling events do not reconcile to exactly one settlement");
        Check(Mathf.Abs(manager.RetainedTimberM3 - (keptBefore + approved.RetainedVolumeCm3 / 1000000f)) < 1e-5f, "retained material not deposited exactly once");
        Check(manager.DeadwoodRecords.Count == 4, "retained fallen deadwood missing");
        ScenarioAnnualReport year1 = manager.AnnualReports.Last();
        Check(year1.year == 1 && year1.harvestMinimumAdjustmentCents == minimumAdjustment && year1.timberSales.Count > 0
              && year1.closingCashCents == manager.CashCents, "annual economy report not populated");
        List<string> review1 = ScenarioEcologyReviewLines.Lines(manager.EcologicalSnapshots[manager.EcologicalSnapshots.Count - 2],
            manager.EcologicalSnapshots.Last(), ecology, manager.PlantedJuveniles);
        Check(review1.Count >= 3 && review1.Any(l => l.StartsWith("Browsing pressure low")), "annual ecology review lines missing");
        Pass("P3", $"year=1 felledOnce={fellCounts.Count} cashDelta={manager.CashCents - cashBefore} ledger={ledger} retainedM3={F(manager.RetainedTimberM3)} "
            + $"deadwoodRecords={manager.DeadwoodRecords.Count} products={year1.timberSales.Count} reviewLines={review1.Count}");

        // ---- P5 PLANT / PROTECT (designated in Year 1, resolved as Year 2) ----
        int pairsPerSpecies = 4;
        long cashBeforeStock = manager.CashCents;
        Check(manager.TryPurchaseStock(OakItem, pairsPerSpecies * 2) && manager.TryPurchaseStock(BeechItem, pairsPerSpecies * 2),
            "planting stock purchase failed: " + manager.Feedback);
        long stockCost = cashBeforeStock - manager.CashCents;
        int[] bright = Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i).ToArray();
        var pairs = new List<(Vector3 sheltered, Vector3 exposed, string species)>();
        for (int k = 0; k < bright.Length && pairs.Count < pairsPerSpecies * 2; k++)
        {
            string species = pairs.Count < pairsPerSpecies ? Oak : Beech;
            if (TryPlantPair(bright[k], species == Oak ? OakItem : BeechItem, out Vector3 a, out Vector3 b))
                pairs.Add((a, b, species));
        }
        Check(pairs.Count(p => p.species == Oak) == pairsPerSpecies && pairs.Count(p => p.species == Beech) == pairsPerSpecies,
            "could not designate planting pairs in the bright opened cells");
        List<ScenarioOneWorkOrder> plantOrders = manager.WorkOrders.Where(o => o.type == ScenarioWorkType.PlantJuvenile && o.IsOpen).ToList();
        List<ScenarioPlantingQuote> plantQuotes = plantOrders.Select(o => manager.GetPlantingQuote(o)).ToList();
        for (int i = 0; i < plantOrders.Count; i++)
        {
            ScenarioOneWorkOrder o = plantOrders[i]; ScenarioPlantingQuote q = plantQuotes[i];
            if (o.executionMethod == WorkExecutionMethod.LandownerSimulated)
                Check(o.installShelter && q.Eligible && q.OwnerMinutes == 3 && q.WorkCents == 0 && q.MaterialCents == 500, "owner sheltered planting quote");
            else
                Check(!o.installShelter && q.Eligible && q.OwnerMinutes == 0 && q.WorkCents > 0 && q.MaterialCents == 0, "contractor planting quote");
        }
        Check(manager.ApprovePendingWork(), "planting work could not be approved: " + manager.Feedback);
        int resolutionYear = ecology.EcologicalYear + 1;
        long cashBeforePlant = manager.CashCents;
        float keptBeforePlant = manager.RetainedTimberM3;
        Check(manager.AdvanceYear() && ecology.EcologicalYear == 2, "planting resolution failed: " + manager.Feedback);
        long plantLedger = plantQuotes.Sum(q => q.LedgerCashDelta);
        Check(manager.CashCents - cashBeforePlant == plantLedger, $"planting cash {manager.CashCents - cashBeforePlant} != ledgers {plantLedger}");
        Check(manager.GetStockQuantity(OakItem) == 0 && manager.GetStockQuantity(BeechItem) == 0, "stock not consumed exactly once");
        Check(manager.AnnualReports.Last().ownerMinutes == pairs.Count * 3, "owner minutes not recorded");
        Check(Mathf.Abs(manager.RetainedTimberM3 - keptBeforePlant) < 1e-6f, "retained material deposited again");
        Check(manager.PlantedJuveniles.Count == pairs.Count * 2 && manager.PlantedJuveniles.All(j => j.plantingYear == resolutionYear),
            "planted juveniles missing or plantingYear differs from the resolution year");
        Check(manager.Shelters.Count == pairs.Count && ecology.Browsing.Shelters.Count == pairs.Count
              && manager.Shelters.All(sh => sh.installedYear == resolutionYear && sh.effectiveYears == 8), "shelters not created once with the timing contract");
        Check(manager.PlantedJuveniles.Where(j => IsSheltered(j, pairs)).All(j => j.lastBrowseAssessment.Protection == BrowseProtectionState.EffectiveShelter && !j.lastYearBrowsed),
            "a live shelter did not protect its juvenile in the first step");
        visuals.RefreshNow();
        Check(visuals.EffectiveVisualCount == pairs.Count, "shelter tubes not visible immediately after planting");
        Pass("P5", $"pairs={pairs.Count} owner=sheltered contractor=unsheltered stockCost={stockCost} plantingLedger={plantLedger} ownerMinutes={manager.AnnualReports.Last().ownerMinutes} "
            + $"shelters={manager.Shelters.Count} installedYear={resolutionYear} visuals={visuals.EffectiveVisualCount}");

        // Browsing record from here on: shelters block browsing while effective; exposed oak can be browsed.
        int exposedOakBrowses = 0, protectedBrowses = 0;
        long minCash = manager.CashCents;
        void Track()
        {
            minCash = Math.Min(minCash, manager.CashCents);
            foreach (PlantedJuvenile j in manager.PlantedJuveniles.Where(j => j.lastBrowseAssessmentYear == ecology.EcologicalYear))
            {
                if (j.lastBrowseAssessment.Protection == BrowseProtectionState.EffectiveShelter && j.lastYearBrowsed) protectedBrowses++;
                if (j.speciesId == Oak && !IsSheltered(j, pairs) && j.lastYearBrowsed) exposedOakBrowses++;
            }
        }
        IEnumerator Advance(int year)
        {
            while (ecology.EcologicalYear < year)
            {
                Check(manager.AdvanceYear(), $"annual advance to {year} failed at {ecology.EcologicalYear}: {manager.Feedback}");
                Track();
                if (ecology.EcologicalYear % 5 == 0) yield return null;
            }
        }

        // ---- P4 RESPOND (Year 6) ----
        IEnumerator toSix = Advance(6);
        while (toSix.MoveNext()) yield return toSix.Current;
        float thinnedGain = Enumerable.Range(0, ecology.CellCount).Where(thinnedCells.Contains).Average(i => ecology.Cells[i].Light - initialLight[i]);
        float otherGain = Enumerable.Range(0, ecology.CellCount).Where(i => !thinnedCells.Contains(i)).Select(i => ecology.Cells[i].Light - initialLight[i]).DefaultIfEmpty(0f).Average();
        float canopy6 = ecology.Cells.Average(c => c.Canopy);
        Check(thinnedGain > 0f && thinnedGain > otherGain, $"thinned cells did not gain more light: {thinnedGain} vs {otherGain}");
        Check(canopy6 < initialCanopy, "canopy did not respond to thinning");
        Pass("P4", $"year=6 lightGain thinned={F(thinnedGain)} unthinned={F(otherGain)} canopy {F(initialCanopy)}->{F(canopy6)}");

        // ---- P8a: pairs at Year 6, while still inside browse reach ----
        int pairs6 = 0, taller6 = 0;
        var diagnosis6 = new Dictionary<RegenerationLimit, int>();
        foreach (var (sheltered, exposed, species) in pairs)
        {
            PlantedJuvenile s6 = At(sheltered), e6 = At(exposed);
            foreach (PlantedJuvenile j in new[] { s6, e6 }.Where(j => j != null && j.alive && string.IsNullOrEmpty(j.promotedTreeId)))
            {
                RegenerationLimit limit = RegenerationDiagnostics.Diagnose(ecology, j.position, manager.PlantedJuveniles, spawner).Limit;
                diagnosis6[limit] = diagnosis6.TryGetValue(limit, out int n) ? n + 1 : 1;
                if (j == s6 && j.heightMeters < spawner.ResolveSpecies(species).BrowseEscapeHeightM && limit != RegenerationLimit.LightLimited)
                    Check(limit == RegenerationLimit.Protected, "sheltered juvenile below reach not diagnosed Protected: " + limit);
            }
            if (s6 == null || e6 == null || !s6.alive || !e6.alive || !string.IsNullOrEmpty(s6.promotedTreeId) || !string.IsNullOrEmpty(e6.promotedTreeId))
                continue;
            pairs6++;
            Check(s6.heightMeters >= e6.heightMeters - 1e-5f, $"sheltered {species} shorter than its exposed pair at Year 6");
            if (s6.heightMeters > e6.heightMeters + 1e-5f) taller6++;
        }
        Check(pairs6 > 0 && taller6 > 0, "no Year-6 pair showed a browsing difference under pressure 0.2");

        // ---- P6 SAVE MID-PLAN (Year 6): approved grouped harvest + pending owner sheltered planting ----
        List<ForestTree> sitkaNow = Living().Where(t => t.Species.SpeciesId == Sitka && !marking.GetCropTreeIds().Contains(t.TreeId) && t.CanChop).ToList();
        manager.PlanningFellingOutcome = FellingMaterialOutcome.KeepForUse;
        foreach (ForestTree t in sitkaNow.Take(3)) marking.Mark(t, TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() == 3 && manager.ApprovePendingWork(), "mid-plan harvest not approved");
        Check(manager.TryPurchaseStock(OakItem, 1) && TryPlantSingle(OakItem, WorkExecutionMethod.LandownerSimulated, true), "mid-plan owner sheltered planting not designated");
        ForestSaveData mid = saves.CaptureData();
        string midJson = JsonUtility.ToJson(mid);
        Check(mid.version == ForestSaveData.CurrentVersion && mid.scenarioOne.shelters.Count == pairs.Count, "mid save is not current-version with shelters");
        Check(mid.scenarioOne.workOrders.Count(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Approved && o.harvestJobId >= 0
              && o.executionMethod == WorkExecutionMethod.Contractor) == 3, "approved grouped harvest not saved");
        Check(mid.scenarioOne.workOrders.Any(o => o.type == ScenarioWorkType.PlantJuvenile && o.status == ScenarioWorkStatus.Pending
              && o.installShelter && o.executionMethod == WorkExecutionMethod.LandownerSimulated), "pending owner sheltered planting not saved");
        Pass("P6", $"year=6 version={mid.version} inMemory=True shelters={mid.scenarioOne.shelters.Count} approvedHarvest=3 pendingOwnerShelter=1");

        // ---- P7 CONTINUE vs RESTORE (10 years) ----
        IEnumerator toSixteen = Advance(16);
        while (toSixteen.MoveNext()) yield return toSixteen.Current;
        string continued = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        int continuedShelters = ecology.Browsing.Shelters.Count;
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(midJson), false), "in-memory restore failed");
        yield return null; yield return null;
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(midJson), false), "repeat in-memory restore failed");
        yield return null; yield return null;
        Check(ecology.Browsing.Shelters.Count == pairs.Count && manager.Shelters.Count == pairs.Count, "restore duplicated protection");
        visuals.RefreshNow();
        Check(visuals.EffectiveVisualCount == pairs.Count, "shelter tubes not visible after load");
        // Reference preview: player shelters absent in the archive, restored on exit.
        if (ScenarioReferenceArchive.Load() != null)
        {
            Check(manager.TryBeginReferencePreview(50), "reference preview did not open");
            yield return null; yield return null;
            visuals.RefreshNow();
            Check(ecology.Browsing.Shelters.Count == 0 && visuals.VisualCount == 0, "player shelters visible in the archive preview");
            manager.EndReferencePreview();
            yield return null; yield return null;
            visuals.RefreshNow();
            Check(ecology.Browsing.Shelters.Count == pairs.Count && visuals.EffectiveVisualCount == pairs.Count, "player shelters not restored after preview");
        }
        IEnumerator again = Advance(16);
        while (again.MoveNext()) yield return again.Current;
        string restored = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(continued == restored && continuedShelters == ecology.Browsing.Shelters.Count, $"continue vs restore differ: {continued} vs {restored}");
        Pass("P7", $"years=6->16 continued={continued} restored={restored} shelters={ecology.Browsing.Shelters.Count} previewShelters=0->restored");

        // ---- P8 REGENERATION ----
        int regenInThinned = thinnedCells.Count(i => ecology.Cells[i].HasRegeneration);
        Check(protectedBrowses == 0, "a juvenile was browsed while its shelter was effective");
        Check(exposedOakBrowses > 0, "exposed oak was never browsed at pressure 0.2");
        Check(regenInThinned > 0, "no regeneration in the managed opening");
        int oakPromotedSheltered = pairs.Where(p => p.species == Oak).Count(p => !string.IsNullOrEmpty(At(p.sheltered)?.promotedTreeId));
        int oakPromotedExposed = pairs.Where(p => p.species == Oak).Count(p => !string.IsNullOrEmpty(At(p.exposed)?.promotedTreeId));
        Check(oakPromotedSheltered >= oakPromotedExposed, "exposed oak out-recruited sheltered oak");
        Pass("P8", $"year6Pairs={pairs6} shelteredTaller={taller6} browsesWhileSheltered=0 exposedOakBrowses={exposedOakBrowses} regenThinnedCells={regenInThinned} "
            + $"oakPromoted sheltered={oakPromotedSheltered} exposed={oakPromotedExposed} diagnosisY6=" + string.Join(",", diagnosis6.OrderBy(p => p.Key).Select(p => p.Key + ":" + p.Value)));

        // ---- P9 DECADES: second intervention at Year 16, continue to 30 ----
        List<ForestTree> standing = Living().Where(t => t.Species.SpeciesId == Sitka).ToList();
        List<ForestTree> second = SelectThinning(standing, marking.GetCropTreeIds(), 0.2f, out float secondFraction);
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (ForestTree t in second) marking.Mark(t, TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() == second.Count && manager.ApprovePendingWork(), "second intervention could not be planned: " + manager.Feedback);
        Check(manager.GetHarvestQuote(true).Orders.Count == second.Count, "second intervention not one grouped job");
        int minimumYear = definition.MinimumCompletionYear;
        IEnumerator toCompletion = Advance(minimumYear);
        while (toCompletion.MoveNext()) yield return toCompletion.Current;
        string objectives = ObjectiveLine();
        ScenarioOneOutcome outcomeAtMinimum = manager.Outcome;
        int completedYear = manager.OutcomeYear;
        IEnumerator toThirty = Advance(30);
        while (toThirty.MoveNext()) yield return toThirty.Current;
        ScenarioEcologicalSnapshot y30 = manager.EcologicalSnapshots.Last();
        string finalHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        string legacyLayoutHash = ScenarioReferenceArchive.LegacyV15WorldHash(saves.CaptureData()) ?? "n/a";
        string v16LayoutHash = ScenarioReferenceArchive.LegacyV16WorldHash(saves.CaptureData()) ?? "n/a";
        Debug.Log($"SCENARIO_ONE_COMPLETION_OBJECTIVES year={minimumYear} outcome={outcomeAtMinimum} {objectives}");
        Debug.Log($"SCENARIO_ONE_COMPLETION_CASH minimum={minCash} year30={manager.CashCents}");
        Debug.Log("SCENARIO_ONE_COMPLETION_V17_COMPAT_HASH " + (ScenarioReferenceArchive.LegacyV17WorldHash(saves.CaptureData()) ?? "n/a"));
        string v18LayoutHash = ScenarioReferenceArchive.LegacyV18WorldHash(saves.CaptureData()) ?? "n/a";
        Debug.Log("SCENARIO_ONE_COMPLETION_V18_COMPAT_HASH " + v18LayoutHash);
        if (ecology.RngModelVersion == 1 && ecology.RegenerationModelVersion == 2
            && ecology.GrowthModelVersion == 1 && ecology.StormModelVersion == 0)
            Check(v18LayoutHash == "702766DECE591E21", "accepted model2 completion retains its exact v18 world layout");
        Debug.Log($"SCENARIO_ONE_COMPLETION_HASH {finalHash} rngModel={ecology.RngModelVersion} regenerationModel={ecology.RegenerationModelVersion} growthModel={ecology.GrowthModelVersion} legacyV15LayoutHash={legacyLayoutHash} v16LayoutHash={v16LayoutHash}");
        Check(minCash >= 0, "cash went below zero");
        Check(outcomeAtMinimum == ScenarioOneOutcome.Completed && completedYear == minimumYear,
            $"reasonable CCF plan did not complete at Year {minimumYear}: {objectives}");
        Check(manager.Objectives.Count > 0 && ecology.EcologicalYear == 30, "objectives not evaluated to Year 30");
        Pass("P9", $"secondIntervention={second.Count} basalArea={F(secondFraction)} completedYear={completedYear} y30 living={y30.livingTrees} "
            + $"canopy={F(y30.meanCanopy)} regenCells={y30.occupiedRegenerationCells} deadwood={F(y30.deadwoodVolumeM3)} minCash={minCash} cash30={manager.CashCents} hash={finalHash}");

        // ---- P10 COMPATIBILITY: a v14-shaped save restores with Contractor defaults and no shelters ----
        ForestSaveData legacy = JsonUtility.FromJson<ForestSaveData>(midJson);
        legacy.version = 14;
        legacy.regenerationModel = RegenerationModel.Legacy;
        legacy.growthModel = GrowthModel.Legacy;
        Check(saves.LoadData(legacy, false), "v14-shaped save rejected");
        yield return null; yield return null;
        Check(ecology.Browsing.Shelters.Count == 0 && ecology.Browsing.ProtectedAreas.Count == 0 && manager.Shelters.Count == 0, "v14 restore kept protection");
        Check(manager.WorkOrders.All(o => o.executionMethod == WorkExecutionMethod.Contractor && !o.installShelter && o.harvestJobId == -1)
              && manager.OwnerMinutesUsedThisYear == 0, "v14 restore did not apply Contractor/no-shelter defaults");
        visuals.RefreshNow();
        Check(visuals.VisualCount == 0, "v14 restore shows shelter tubes");
        Check(ecology.Browsing.BackgroundPressure == scenarioPressure, "restore changed scenario browse pressure");
        for (int y = 0; y < 3; y++) Check(manager.AdvanceYear(), "v14-restored world cannot continue: " + manager.Feedback);
        Pass("P10", $"v14 restore year={ecology.EcologicalYear} shelters=0 executors=Contractor continued=3");

        // ---- P11 NEGATIVE CONTROL: do nothing ----
        Check(saves.LoadData(original, false), "restore fresh world");
        yield return null; yield return null;
        ecology.Browsing.BackgroundPressure = scenarioPressure;
        IEnumerator idle = AdvanceTo(minimumYear);
        while (idle.MoveNext()) yield return idle.Current;
        string idleObjectives = ObjectiveLine();
        Check(manager.Outcome != ScenarioOneOutcome.Completed, "doing nothing completed Scenario One: " + idleObjectives);
        Pass("P11", $"year={ecology.EcologicalYear} outcome={manager.Outcome} {idleObjectives}");
    }

    private bool TryPlantSingle(string item, WorkExecutionMethod method, bool shelter)
    {
        foreach (int cell in Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i))
        {
            Vector2 c = ecology.Cells[cell].Center;
            for (int ix = -2; ix <= 2; ix++)
            for (int iz = -2; iz <= 2; iz++)
                if (manager.TryDesignateExactPlanting(item, new Vector3(c.x + ix * 0.9f + 0.3f, 0f, c.y + iz * 0.9f + 0.3f), method, shelter))
                    return true;
        }
        return false;
    }

    private IEnumerator Cleanup()
    {
        if (saves == null || original == null) yield break;
        ecology.Browsing.ClearProtection();
        ecology.Browsing.BackgroundPressure = scenarioPressure;
        Check(saves.LoadData(original, false), "could not restore the captured scene world");
        yield return null; yield return null;
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == ScenarioReferenceArchive.WorldHash(original),
            "scene world not restored after the gate");
        Debug.Log("SCENARIO_ONE_COMPLETION_SCENE_RESTORED");
    }

    // ---------- plan helpers ----------

    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);

    // Largest stem per 10 m block: the retained crop-tree framework.
    private static List<string> SelectCropTrees(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId)
        .OrderBy(id => id, StringComparer.Ordinal).ToList();

    // Competitor release: nearest non-crop neighbours of crop trees first, until
    // the basal-area fraction is reached.
    private static List<ForestTree> SelectThinning(List<ForestTree> trees, List<string> cropIds, float fraction, out float removed)
    {
        var crop = new HashSet<string>(cropIds);
        List<ForestTree> crops = trees.Where(t => crop.Contains(t.TreeId)).ToList();
        float total = trees.Sum(BasalArea), target = total * fraction, taken = 0f;
        var result = new List<ForestTree>();
        foreach (ForestTree t in trees.Where(t => !crop.Contains(t.TreeId) && t.CanChop)
                     .OrderBy(t => crops.Count == 0 ? 0f : crops.Min(c => Vector3.Distance(c.transform.position, t.transform.position)))
                     .ThenBy(t => t.TreeId, StringComparer.Ordinal))
        {
            if (taken >= target) break;
            result.Add(t);
            taken += BasalArea(t);
        }
        removed = total > 0f ? taken / total : 0f;
        return result;
    }

    private bool TryPlantPair(int cell, string item, out Vector3 sheltered, out Vector3 exposed)
    {
        sheltered = exposed = Vector3.zero;
        Vector2 c = ecology.Cells[cell].Center;
        var candidates = new List<Vector3>();
        for (int ix = -2; ix <= 2; ix++)
        for (int iz = -2; iz <= 2; iz++)
            candidates.Add(new Vector3(c.x + ix * 0.9f, 0f, c.y + iz * 0.9f));
        Vector3? first = null;
        foreach (Vector3 p in candidates)
        {
            if (ecology.GetCellIndex(p) != cell) continue;
            if (first.HasValue && Vector3.Distance(first.Value, p) < 0.8f) continue;
            bool shelterThis = !first.HasValue;
            if (!manager.TryDesignateExactPlanting(item, p,
                    shelterThis ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor, shelterThis)) continue;
            if (!first.HasValue) { first = p; continue; }
            sheltered = first.Value;
            exposed = p;
            return true;
        }
        return false;
    }

    private bool IsSheltered(PlantedJuvenile j, List<(Vector3 sheltered, Vector3 exposed, string species)> pairs)
        => pairs.Any(p => Vector2.Distance(new Vector2(p.sheltered.x, p.sheltered.z), new Vector2(j.position.x, j.position.z)) < 0.01f);

    private PlantedJuvenile At(Vector3 position) => manager.PlantedJuveniles.FirstOrDefault(j =>
        Vector2.Distance(new Vector2(j.position.x, j.position.z), new Vector2(position.x, position.z)) < 0.01f);

    private IEnumerator AdvanceTo(int year)
    {
        while (ecology.EcologicalYear < year)
        {
            Check(manager.AdvanceYear(), $"annual advance to {year} failed at {ecology.EcologicalYear}: {manager.Feedback}");
            if (ecology.EcologicalYear % 5 == 0) yield return null;
        }
    }

    private Vector3 CellCenter(int index) => new Vector3(ecology.Cells[index].Center.x, 0f, ecology.Cells[index].Center.y);

    private string ObjectiveLine() => string.Join(" ", manager.Objectives.Select(o =>
        $"{o.objectiveId}={o.currentValue.ToString("0.###", CultureInfo.InvariantCulture)}/{o.targetValue.ToString("0.###", CultureInfo.InvariantCulture)}:{(o.achieved ? "ok" : "no")}"));
}
#endif

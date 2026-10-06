using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Reference Future v1 tooling. Copy into Assets/ForestPrototype,
// run one entry point in Editor batchmode, then remove the temporary Assets
// copy and generated .meta. Restores the user's save.
//
// Contract (Docs/ReferenceFutureContract.md): v1 is a frozen historical
// archive. Verification covers ARCHIVE integrity (original embedded data,
// never re-serialised through current save classes), PREVIEW of the frozen
// milestones, and CONTINUATION of the historical Year-50 save with the current
// ecology. Exact historical REPLAY by current code is not part of the contract.
//   Begin                    archive + preview + continuation gate (play mode)
//   ReportFrozen             archive integrity + frozen milestone report (no play mode; add -quit)
//   BeginAuthoringCandidate  author a NEW candidate with the current model
//                            (differences from v1 are diagnostics; Freeze
//                            refuses to overwrite v1)
public static class ScenarioReferenceVerification
{
#if UNITY_EDITOR
    private const string CandidatePath = "/tmp/opencode/reference-future-v1-candidate.json";
    private const string ResourceFolder = "Assets/ForestPrototype/ScenarioOne/Resources";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    public static void BeginAuthoringCandidate()
    {
        Begin();
    }

    // Explicit separate step: only freeze a candidate produced by the complete
    // simulation and independently reviewed. The archive and schedule become
    // data assets; no final forest is hand-constructed or changed here.
    public static void Freeze()
    {
        if (!File.Exists(CandidatePath))
            throw new InvalidOperationException("No verified reference candidate to freeze.");
        ScenarioReferenceArchive archive = JsonUtility.FromJson<ScenarioReferenceArchive>(
            File.ReadAllText(CandidatePath));
        ScenarioOneDefinition definition = AssetDatabase.LoadAssetAtPath<ScenarioOneDefinition>(
            "Assets/ForestPrototype/ScenarioOne/ScenarioOne.asset");
        if (archive == null || definition == null || archive.definitionVersion != definition.DefinitionVersion
            || archive.saveVersion != ForestSaveData.CurrentVersion
            || archive.scheduleHash != ScenarioReferenceArchive.Hash(JsonUtility.ToJson(archive.schedule))
            || archive.milestones == null || archive.milestones.Count != 4
            || archive.AtYear(0)?.worldHash != archive.startingStandHash
            || archive.AtYear(100)?.world?.scenarioOne?.outcome != ScenarioOneOutcome.Completed
            || archive.AtYear(100).world.scenarioOne.annualReports.Last().completedTasks != 0)
            throw new InvalidOperationException("Reference candidate is incomplete or incompatible; not frozen.");
        foreach (int year in new[] { 0, 20, 50, 100 })
        {
            ScenarioReferenceMilestone milestone = archive.AtYear(year);
            if (milestone?.world == null
                || ScenarioReferenceArchive.WorldHash(milestone.world) != milestone.worldHash)
                throw new InvalidOperationException("Reference Year " + year + " failed its full-world hash.");
        }
        ScenarioReferenceArchive existing = ScenarioReferenceArchive.Load();
        if (existing != null && (existing.startingStandHash != archive.startingStandHash
            || existing.scheduleHash != archive.scheduleHash
            || existing.AtYear(100)?.worldHash != archive.AtYear(100).worldHash))
            throw new InvalidOperationException("Reference Future v1 is frozen with different content; "
                + "author a new version rather than overwriting it.");

        Directory.CreateDirectory(ResourceFolder);
        File.WriteAllText(ResourceFolder + "/ScenarioOneReferenceScheduleV1.json",
            JsonUtility.ToJson(archive.schedule, true));
        byte[] json = Encoding.UTF8.GetBytes(JsonUtility.ToJson(archive, true));
        using (var memory = new MemoryStream())
        {
            using (var gzip = new GZipStream(memory, System.IO.Compression.CompressionLevel.Optimal, true))
                gzip.Write(json, 0, json.Length);
            File.WriteAllBytes(ResourceFolder + "/ScenarioOneReferenceFutureV1.bytes", memory.ToArray());
        }
        AssetDatabase.Refresh();
        ScenarioReferenceArchive loaded = ScenarioReferenceArchive.Load();
        if (loaded == null || loaded.AtYear(100)?.worldHash != archive.AtYear(100).worldHash)
            throw new InvalidOperationException("Frozen reference asset could not be reloaded.");
        Debug.Log($"REFERENCE_FUTURE_V1_FROZEN start={archive.startingStandHash} "
            + $"schedule={archive.scheduleHash} year100={archive.AtYear(100).worldHash}");
    }

    public static void ReportFrozen()
    {
        // Integrity is checked against the archive's original embedded JSON.
        // Re-hashing milestone.world here would serialise v12 data through the
        // current (v14) save classes, which adds fields and changes the hash.
        ReferenceArchiveIntegrity.Verify();
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        if (archive == null)
            throw new InvalidOperationException("Verified Reference Future v1 resource not found.");
        foreach (int year in new[] { 0, 20, 50, 100 })
        {
            ScenarioReferenceMilestone milestone = archive.AtYear(year);
            if (milestone?.world?.scenarioOne == null || !milestone.verifiedFrozenWorld)
                throw new InvalidOperationException("Frozen milestone year " + year + " is incompatible.");
            ScenarioOneSaveData state = milestone.world.scenarioOne;
            ScenarioEcologicalSnapshot stand = state.ecologicalSnapshots.Last();
            int Felled(ScenarioEcologicalTreatment treatment) => state.managementEvents.Count(entry =>
                entry.eventType == ScenarioManagementEventType.WorkResolved
                && entry.ecologicalTreatment == treatment && entry.outcome == ScenarioManagementOutcome.Succeeded);
            Debug.Log($"REFERENCE_FROZEN_YEAR_{year} trees={stand.livingTrees} "
                + $"sitka={stand.species.First(entry => entry.speciesId == "sitka-spruce").livingTrees} "
                + $"oak={stand.species.First(entry => entry.speciesId == "sessile-oak").livingTrees} "
                + $"beech={stand.species.First(entry => entry.speciesId == "beech").livingTrees} "
                + $"regen={stand.occupiedRegenerationCells} light={stand.meanLight:0.000} "
                + $"canopy={stand.meanCanopy:0.000} deadwood={stand.deadwoodVolumeM3:0.000} "
                + $"grass={stand.meanGrasses:0.000} fern={stand.meanFerns:0.000} "
                + $"pruned={Felled(ScenarioEcologicalTreatment.TreePruned)} "
                + $"extracted={Felled(ScenarioEcologicalTreatment.TreeFelledAndExtracted)} "
                + $"retained={Felled(ScenarioEcologicalTreatment.TreeRetainedAsDeadwood)} "
                + $"regenRemoved={Felled(ScenarioEcologicalTreatment.RegenerationRemoved)} "
                + $"events={state.managementEvents.Count} worldHash={milestone.worldHash}");
        }
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("Scenario Reference Verification").AddComponent<ScenarioReferenceVerificationRunner>();
    }
}

public sealed class ScenarioReferenceVerificationRunner : MonoBehaviour
{
    private string savePath;
    private byte[] backup;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        bool authoring = Array.Exists(Environment.GetCommandLineArgs(),
            arg => arg.EndsWith("ScenarioReferenceVerification.BeginAuthoringCandidate", StringComparison.Ordinal));
        IEnumerator run = authoring ? Run() : RunContract();
        while (true)
        {
            bool more;
            object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("REFERENCE_FUTURE_V1_PASS");
        else Debug.LogError("REFERENCE_FUTURE_V1_FAIL: " + failure);
        if (savePath != null)
        {
            if (backup != null) File.WriteAllBytes(savePath, backup);
            else if (File.Exists(savePath)) File.Delete(savePath);
        }
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Run()
    {
        ScenarioOneManager manager = FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestTreeMarkingManager marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        Check(manager != null && ecology != null && marking != null && spawner != null && saves != null,
            "scenario systems missing");
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) backup = File.ReadAllBytes(savePath);

        // Regression anchor: fresh stand must be exactly 336 Sitka.
        Check(ecology.LivingTreeCount == 336, $"fresh stand has {ecology.LivingTreeCount} trees, expected 336");
        List<ForestTree> fresh = AllLiving();
        Check(fresh.All(t => t.Species != null && t.Species.SpeciesId == "sitka-spruce"),
            "fresh stand has non-Sitka trees");
        ReferenceMilestone y0 = CaptureMilestone(manager, ecology, spawner, 0);
        Check(ecology.EcologicalYear == 0 && ecology.SimulationSeed == 20260914
            && manager.WorkOrders.Count == 0 && manager.CashCents == manager.Definition.StartingCashCents,
            "reference did not start from the fresh configured Year-0 scenario");

        ScenarioReferenceSchedule schedule = ScenarioReferenceRunner.BuildSchedule();
        Check(schedule.directives.Count > 10, "schedule has too few directives");
        ScenarioReferenceArchive frozen = ScenarioReferenceArchive.Load();
        Check(frozen != null && frozen.Matches(manager.Definition, ecology)
            && frozen.scheduleHash == ScenarioReferenceArchive.Hash(JsonUtility.ToJson(schedule)),
            "frozen reference and authored schedule are missing or incompatible with this scenario");
        manager.SetReferenceAuthoring(true);
        ScenarioReferenceSurvey survey = ScenarioReferenceRunner.Survey(manager, ecology,
            schedule.futureTreeStride);
        Check(survey.futureTreeIds.Count > 50, $"survey selected only {survey.futureTreeIds.Count} future trees");
        Check(survey.oakCandidateCells.Count > 0 && survey.beechCandidateCells.Count > 0,
            "survey found no broadleaf candidate cells");
        Debug.Log($"REFERENCE_SURVEY futureTrees={survey.futureTreeIds.Count} "
            + $"oakCells={survey.oakCandidateCells.Count} beechCells={survey.beechCandidateCells.Count} "
            + $"seed={schedule.simulationSeed} schedule={schedule.scheduleId}");

        var archive = new ScenarioReferenceArchive
        {
            referenceId = schedule.scheduleId,
            scenarioId = schedule.scenarioId,
            definitionVersion = manager.Definition.DefinitionVersion,
            saveVersion = ForestSaveData.CurrentVersion,
            simulationSeed = ecology.SimulationSeed,
            schedule = JsonUtility.FromJson<ScenarioReferenceSchedule>(JsonUtility.ToJson(schedule)),
            scheduleHash = ScenarioReferenceArchive.Hash(JsonUtility.ToJson(schedule)),
            futureTreeIds = survey.futureTreeIds.OrderBy(id => id, StringComparer.Ordinal).ToList()
        };
        archive.milestones.Add(CaptureFullWorld(saves, 0));
        archive.startingStandHash = archive.AtYear(0).worldHash;
        // Authoring diagnostics only: a candidate built by the current model and
        // schema is not expected to reproduce v1 (contract: no exact replay).
        if (frozen != null)
            Debug.Log($"REFERENCE_AUTHORING_VS_V1 year=0 candidate={archive.startingStandHash} "
                + $"v1={frozen.startingStandHash} identical={archive.startingStandHash == frozen.startingStandHash}");

        var milestones = new Dictionary<int, ReferenceMilestone>();
        var uninterruptedWorlds = new Dictionary<int, ScenarioReferenceMilestone>();
        var yearLog = new StringBuilder();
        float minimumAnnualCanopy = float.MaxValue;
        int largestVeryOpenPatch = 0;
        for (int year = 0; year < 100; year++)
        {
            int currentYear = ecology.EcologicalYear;
            ScenarioReferenceRunner.ExecuteYear(manager, ecology, marking, schedule, survey, currentYear);
            if (currentYear == 4)
            {
                int plantingOrders = manager.WorkOrders.Count(order => order.IsOpen
                    && order.type == ScenarioWorkType.PlantJuvenile);
                int removalOrders = manager.WorkOrders.Count(order => order.IsOpen
                    && order.type == ScenarioWorkType.RemoveRegeneration);
                Debug.Log($"REFERENCE_Y5_PLAN planting={plantingOrders} removal={removalOrders} "
                    + $"beechStock={manager.GetStockQuantity("beech-sapling")} oakStock={manager.GetStockQuantity("sessile-oak-sapling")}");
            }
            if (!manager.ApprovePendingWork())
            {
                // No valid pending work is acceptable in low-intervention years.
            }
            Check(manager.AdvanceYear(), $"year {currentYear + 1} did not advance");
            float canopy = ecology.Cells.Average(cell => cell.Canopy);
            int openPatch = LargestVeryOpenPatch(ecology);
            minimumAnnualCanopy = Mathf.Min(minimumAnnualCanopy, canopy);
            largestVeryOpenPatch = Mathf.Max(largestVeryOpenPatch, openPatch);
            Check(canopy >= 0.3f && openPatch <= 10,
                $"year {currentYear + 1} lost continuous cover (canopy {canopy:0.00}, "
                + $"contiguous very-open cells {openPatch})");
            if (currentYear + 1 == 5)
            {
                string reasons = string.Join(" | ", manager.WorkOrders.Where(order => order.resolvedYear == 5
                    && order.status == ScenarioWorkStatus.Failed)
                    .Select(order => order.validationMessage).Distinct());
                Debug.Log($"REFERENCE_Y5_RESULT completed={manager.AnnualReports.Last().completedTasks} "
                    + $"failed={manager.AnnualReports.Last().failedTasks} reasons={reasons}");
            }
            if (currentYear + 1 == 20 || currentYear + 1 == 50 || currentYear + 1 == 100)
            {
                milestones[currentYear + 1] = CaptureMilestone(manager, ecology, spawner, currentYear + 1);
                archive.milestones.Add(CaptureFullWorld(saves, currentYear + 1));
                if (frozen != null)
                    Debug.Log($"REFERENCE_AUTHORING_VS_V1 year={currentYear + 1} "
                        + $"candidate={archive.AtYear(currentYear + 1).worldHash} v1={frozen.AtYear(currentYear + 1)?.worldHash} "
                        + $"identical={archive.AtYear(currentYear + 1).worldHash == frozen.AtYear(currentYear + 1)?.worldHash}");
            }
            if (currentYear + 1 > 50)
                uninterruptedWorlds[currentYear + 1] = currentYear + 1 == 100
                    ? archive.AtYear(100) : CaptureFullWorld(saves, currentYear + 1);
            if (currentYear + 1 <= 5 || currentYear + 1 % 20 == 0 || currentYear + 1 == 100)
                yearLog.AppendLine(YearSummary(manager, ecology, currentYear + 1));
            yield return null;
        }

        Check(milestones.ContainsKey(20) && milestones.ContainsKey(50) && milestones.ContainsKey(100),
            "missing milestone captures");
        Check(ecology.EcologicalYear == 100, $"final year is {ecology.EcologicalYear}, expected 100");
        Check(manager.Outcome == ScenarioOneOutcome.Completed,
            "reference was not a successful Scenario One trajectory: " + manager.OutcomeReason);
        Check(manager.CenturyReview != null && manager.CenturyReview.year == 100,
            "Century Review not recorded at year 100");

        // Log metrics before acceptance checks.
        foreach (int milestoneYear in new[] { 20, 50, 100 })
        {
            ReferenceMilestone m = milestones[milestoneYear];
            Debug.Log($"REFERENCE_MILESTONE_{milestoneYear} {m.Describe()}");
        }

        // Reference acceptance criteria (from handoff). These describe a
        // credible trajectory, not the scenario's own completion objectives.
        ReferenceMilestone y100 = milestones[100];
        ReferenceMilestone y20 = milestones[20];
        ReferenceMilestone y50 = milestones[50];
        Check(y100.sitkaTrees > 50, $"Sitka dropped to {y100.sitkaTrees} at Year 100");
        Check(y50.plantedOak > 0 && y50.plantedBeech > 0,
            "planted Oak and Beech did not reach individual-tree stages by Year 50");
        Check(y100.oakTrees >= 3 && y100.beechTrees >= 3,
            "Oak or Beech lacked multiple individual trees at Year 100");
        Check(y100.naturalSitka >= 10 && y100.naturalBroadleaf >= 5,
            "natural regeneration did not contribute both Sitka and broadleaf individuals");
        Check(y100.originalSitka >= 50 && y100.olderOriginalSitka >= 20,
            "large older plantation Sitka were not retained");
        Check(y100.totalTrees > 100, $"Year 100 has only {y100.totalTrees} trees");
        Check(y100.deadwoodVolume > 0f, "no persistent deadwood at Year 100");
        Check(y100.sizeClasses >= 2, "insufficient size diversity at Year 100");
        Check(y100.youngTrees >= 10 && y100.maxAge - y100.minAge >= 50,
            "Year-100 age structure is still effectively even-aged");
        Check(y100.canopyCover > 0.3f, $"canopy dropped to {y100.canopyCover:0.00} at Year 100");
        Check(y100.meanLight < 0.8f, $"canopy fully opened at Year 100 (light {y100.meanLight:0.00})");
        Check(y100.brightCells >= 4 && y100.shadedCells >= 8 && y100.grassyCells >= 3,
            "the final stand has no light and understorey mosaic");
        Check(y20.sitkaTrees > 100, $"Sitka dropped too fast by Year 20: {y20.sitkaTrees}");
        Check(y50.canopyCover > 0.3f, $"canopy dropped below continuous cover by Year 50: {y50.canopyCover:0.00}");

        // Ecological improvement from Year 0.
        Check(y100.understoreyEvenness > y0.understoreyEvenness + 0.01f,
            $"understorey did not improve: {y0.understoreyEvenness:0.00} -> {y100.understoreyEvenness:0.00}");
        Check(y100.meanGrass + y100.meanForbs + y100.meanShrubs + y100.meanFerns
            > y0.meanGrass + y0.meanForbs + y0.meanShrubs + y0.meanFerns + 0.10f,
            "functional-group understorey did not change materially from Year 0");
        Check(y100.speciesRichness > y0.speciesRichness,
            $"species richness did not improve: {y0.speciesRichness} -> {y100.speciesRichness}");

        // Timber income across the century.
        float totalTimber = manager.AnnualReports.Sum(r => (float)r.timberRevenueCents);
        Check(totalTimber > 0f, "no timber income across the century");
        Check(manager.AnnualReports.Where(report => report.year <= 50).Sum(report => report.timberRevenueCents) > 0
            && manager.AnnualReports.Where(report => report.year > 50).Sum(report => report.timberRevenueCents) > 0,
            "timber production occurred in only one half of the century");
        Check(manager.WorkOrders.All(order => order.status != ScenarioWorkStatus.Failed),
            "the authored schedule produced failed contractor work");
        Check(manager.AnnualReports.Last().completedTasks == 0,
            "the reference used a final cosmetic Year-100 treatment");

        // Resume a second run from the frozen Year-50 full-world snapshot.
        // Compare the entire canonical Year-100 world, not just the manager's
        // reporting state. All treatment decisions still pass through work
        // orders and Forestry annual ecology on both sides of the reload.
        archive.oakPlantingCells = survey.oakPlantedCells.OrderBy(index => index).ToList();
        archive.beechPlantingCells = survey.beechPlantedCells.OrderBy(index => index).ToList();
        ScenarioReferenceMilestone finalMilestone = archive.AtYear(100);
        Check(finalMilestone != null && archive.AtYear(50) != null,
            "missing reference resume or final-world save");
        saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(archive.AtYear(50).world)));
        yield return null;
        yield return null;
        Check(ecology.EcologicalYear == 50
            && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == archive.AtYear(50).worldHash,
            "Year-50 milestone did not restore exactly");
        for (int year = 50; year < 100; year++)
        {
            ScenarioReferenceRunner.ExecuteYear(manager, ecology, marking, schedule, survey, ecology.EcologicalYear);
            manager.ApprovePendingWork();
            Check(manager.AdvanceYear(), $"replay year {year + 1} did not advance");
            yield return null;
            ScenarioReferenceMilestone replay = CaptureFullWorld(saves, year + 1);
            ScenarioReferenceMilestone expected = uninterruptedWorlds[year + 1];
            if (replay.worldHash != expected.worldHash)
            {
                ScenarioOneSaveData expectedScenario = expected.world.scenarioOne;
                ScenarioOneSaveData actualScenario = replay.world.scenarioOne;
                string expectedLast = string.Join(",", expectedScenario.managementEvents
                    .Skip(Mathf.Max(0, expectedScenario.managementEvents.Count - 4))
                    .Select(entry => entry.eventType.ToString()));
                string actualLast = string.Join(",", actualScenario.managementEvents
                    .Skip(Mathf.Max(0, actualScenario.managementEvents.Count - 4))
                    .Select(entry => entry.eventType.ToString()));
                Debug.LogError($"REFERENCE_REPLAY_EVENTS year={year + 1} "
                    + $"expectedCount={expectedScenario.managementEvents.Count} "
                    + $"actualCount={actualScenario.managementEvents.Count} "
                    + $"expectedOutcome={expectedScenario.outcome} actualOutcome={actualScenario.outcome} "
                    + $"expectedReview={expectedScenario.centuryReview?.year} "
                    + $"actualReview={actualScenario.centuryReview?.year} "
                    + $"expectedLast={expectedLast} actualLast={actualLast}");
                string a = JsonUtility.ToJson(expected.world);
                string b = JsonUtility.ToJson(replay.world);
                int differing = 0;
                while (differing < a.Length && differing < b.Length && a[differing] == b[differing])
                    differing++;
                int start = Mathf.Max(0, differing - 90);
                Debug.LogError($"REFERENCE_REPLAY_DIVERGENCE year={year + 1} offset={differing} "
                    + $"expectedHash={expected.worldHash} actualHash={replay.worldHash} "
                    + $"expected={a.Substring(start, Mathf.Min(180, a.Length - start))} "
                    + $"actual={b.Substring(start, Mathf.Min(180, b.Length - start))}");
                throw new InvalidOperationException("Reloaded reference diverged in year " + (year + 1));
            }
        }
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == finalMilestone.worldHash,
            "reloaded Year-50 run diverged from uninterrupted Year-100 world");
        Check(manager.CenturyReview != null && manager.CenturyReview.year == 100,
            "Century Review did not continue through Year-50 save/load");
        Debug.Log($"REFERENCE_REPLAY_PASS start={archive.startingStandHash} "
            + $"year50={archive.AtYear(50).worldHash} year100={finalMilestone.worldHash}");

        // Candidate archive remains outside Assets until independent review and
        // acceptance. Freeze it as a Resources asset only after those gates pass.
        File.WriteAllText("/tmp/opencode/reference-future-v1-candidate.json",
            JsonUtility.ToJson(archive, true));

        // Report.
        var report = new StringBuilder();
        report.AppendLine("=== REFERENCE FUTURE v1 — REPORT ===");
        report.AppendLine($"schedule={schedule.scheduleId} seed={schedule.simulationSeed}");
        report.AppendLine($"definition={archive.definitionVersion} saveVersion={archive.saveVersion} "
            + $"start={archive.startingStandHash} scheduleHash={archive.scheduleHash}");
        report.AppendLine($"futureTrees={survey.futureTreeIds.Count} directives={schedule.directives.Count}");
        foreach (int milestoneYear in new[] { 20, 50, 100 })
        {
            ReferenceMilestone m = milestones[milestoneYear];
            report.AppendLine($"--- YEAR {milestoneYear} ---");
            report.AppendLine(m.Describe());
        }
        report.AppendLine("--- ECONOMIC SUMMARY ---");
        report.AppendLine($"total timber revenue: {totalTimber / 100f:0.00} EUR");
        report.AppendLine($"total contractor cost: {manager.AnnualReports.Sum(r => (float)r.contractorCostCents) / 100f:0.00} EUR");
        report.AppendLine($"closing cash: {manager.CashCents / 100f:0.00} EUR");
        report.AppendLine($"completed tasks: {manager.AnnualReports.Sum(r => r.completedTasks)}");
        report.AppendLine($"failed tasks: {manager.AnnualReports.Sum(r => r.failedTasks)}");
        report.AppendLine($"minimum annual mean canopy: {minimumAnnualCanopy:0.00}; "
            + $"largest contiguous very-open patch: {largestVeryOpenPatch} of {ecology.CellCount} cells");
        foreach (ScenarioOneWorkOrder failed in manager.WorkOrders.Where(order => order.status == ScenarioWorkStatus.Failed))
            report.AppendLine($"failed #{failed.workOrderId} y{failed.resolvedYear} {failed.type} "
                + $"{failed.speciesId} cell={failed.cellIndex} tree={failed.targetTreeId}: {failed.validationMessage}");
        foreach (var treatment in manager.ManagementEvents.Where(entry =>
                     entry.eventType == ScenarioManagementEventType.WorkResolved
                     && entry.outcome == ScenarioManagementOutcome.Succeeded)
                     .GroupBy(entry => entry.ecologicalTreatment).OrderBy(group => group.Key))
            report.AppendLine($"treatment {treatment.Key}: {treatment.Count()}");
        report.AppendLine("--- MILESTONE SNAPSHOTS ---");
        report.AppendLine($"Year 0 trees={y0.totalTrees} species={y0.speciesRichness} canopy={y0.canopyCover:0.00}");
        report.AppendLine($"Year 20 trees={milestones[20].totalTrees} species={milestones[20].speciesRichness} canopy={milestones[20].canopyCover:0.00}");
        report.AppendLine($"Year 50 trees={milestones[50].totalTrees} species={milestones[50].speciesRichness} canopy={milestones[50].canopyCover:0.00}");
        report.AppendLine($"Year 100 trees={y100.totalTrees} species={y100.speciesRichness} canopy={y100.canopyCover:0.00}");
        foreach (ScenarioReferenceMilestone milestone in archive.milestones)
            report.AppendLine($"Year {milestone.year} worldHash={milestone.worldHash}");
        report.AppendLine("--- YEAR LOG (selected) ---");
        report.Append(yearLog);
        Debug.Log("REFERENCE_FUTURE_V1_REPORT\n" + report);

        Debug.Log($"REFERENCE_FUTURE_V1_DETAIL y20trees={milestones[20].totalTrees} "
            + $"y50trees={milestones[50].totalTrees} y100trees={y100.totalTrees} "
            + $"y100sitka={y100.sitkaTrees} y100oak={y100.oakTrees} y100beech={y100.beechTrees} "
            + $"y100deadwood={y100.deadwoodVolume:0.00} y100canopy={y100.canopyCover:0.00} "
            + $"timber={totalTimber / 100f:0.00}");
    }

    // ---------- Reference Future v1 contract gate ----------

    private IEnumerator RunContract()
    {
        ScenarioOneManager manager = FindFirstObjectByType<ScenarioOneManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestTreeMarkingManager marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        Check(manager != null && ecology != null && marking != null && saves != null, "scenario systems missing");
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) backup = File.ReadAllBytes(savePath);

        // A. ARCHIVE: original embedded data and pinned v1 anchors.
        ReferenceArchiveIntegrity.Verify();
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        Check(archive != null && archive.milestones.All(m => m.verifiedFrozenWorld)
            && archive.Matches(manager.Definition, ecology),
            "the archive loader did not verify every frozen milestone for this scenario");

        // B. PREVIEW: each frozen milestone is shown exactly as stored, and the
        // player's world returns unchanged afterwards.
        string playerHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        int playerYear = ecology.EcologicalYear;
        foreach (int year in new[] { 0, 20, 50, 100 })
        {
            Check(manager.TryBeginReferencePreview(year), "frozen milestone could not be previewed: Year " + year);
            yield return null;
            yield return null;
            Check(ecology.EcologicalYear == year, $"preview of Year {year} shows Year {ecology.EcologicalYear}");
            CheckLiveTreesMatch(archive.AtYear(year).world, "preview Year " + year);
            manager.EndReferencePreview();
            yield return null;
            yield return null;
            Check(ecology.EcologicalYear == playerYear
                && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == playerHash,
                "ending the Year-" + year + " preview did not restore the player's world");
            Debug.Log($"REFERENCE_PREVIEW_PASS year={year} trees={archive.AtYear(year).world.trees.Count} "
                + $"worldHash={archive.AtYear(year).worldHash}");
        }

        // C. CONTINUATION: the historical Year-50 save continues with the
        // current ecology; two independent continuations must agree exactly.
        // Divergence from the frozen Year-100 biology is reported, not asserted.
        ForestSaveData year50 = archive.AtYear(50).world;
        var continuedHashes = new List<string>();
        ForestSaveData continued = null;
        for (int run = 0; run < 2; run++)
        {
            saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(year50)), false);
            yield return null;
            yield return null;
            Check(ecology.EcologicalYear == 50, "historical Year-50 save did not load");
            CheckLiveTreesMatch(year50, "historical Year-50 load");
            var survey = new ScenarioReferenceSurvey
            {
                futureTreeIds = new HashSet<string>(archive.futureTreeIds, StringComparer.Ordinal),
                oakPlantedCells = new List<int>(archive.oakPlantingCells),
                beechPlantedCells = new List<int>(archive.beechPlantingCells)
            };
            ScenarioReferenceSchedule schedule = ScenarioReferenceRunner.BuildSchedule();
            for (int year = 50; year < 100; year++)
            {
                ScenarioReferenceRunner.ExecuteYear(manager, ecology, marking, schedule, survey, year);
                manager.ApprovePendingWork();
                Check(manager.AdvanceYear(), $"continuation {run + 1} stopped at year {year + 1}");
                yield return null;
            }
            continued = saves.CaptureData();
            continuedHashes.Add(ScenarioReferenceArchive.WorldHash(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(continued))));
        }
        Check(continuedHashes[0] == continuedHashes[1],
            $"historical continuation is not deterministic ({continuedHashes[0]} vs {continuedHashes[1]})");
        Check(continued.version == ForestSaveData.CurrentVersion && continued.ecologicalYear == 100
            && manager.CenturyReview != null && manager.CenturyReview.year == 100,
            "continuation did not reach a current-schema Year 100 with its Century Review");
        Check(continued.trees.Select(t => t.treeId).Distinct(StringComparer.Ordinal).Count() == continued.trees.Count,
            "continuation produced duplicate tree ids");
        Check(continued.trees.All(t => !t.biologicallyDead), "continuation applied tree mortality without an explicit cause");
        Dictionary<string, TreeSaveData> continuedTrees = continued.trees.ToDictionary(t => t.treeId);
        foreach (TreeSaveData tree in year50.trees)
            Check(continuedTrees.TryGetValue(tree.treeId, out TreeSaveData later)
                && later.speciesId == tree.speciesId && later.position == tree.position
                && (tree.stage != (int)ForestTreeStage.Stump || later.stage == (int)ForestTreeStage.Stump),
                "Year-50 tree identity, species, position or harvested state lost: " + tree.treeId);
        var continuedEvents = continued.scenarioOne.managementEvents.ToDictionary(entry => entry.eventId);
        foreach (var entry in year50.scenarioOne.managementEvents)
            Check(continuedEvents.TryGetValue(entry.eventId, out var later)
                && later.eventType == entry.eventType && later.year == entry.year,
                "historical management event lost or rewritten: " + entry.eventId);
        ForestSaveData frozen100 = archive.AtYear(100).world;
        int same = frozen100.trees.Count(t => continuedTrees.TryGetValue(t.treeId, out TreeSaveData c)
            && c.stage == t.stage && c.diameterCm == t.diameterCm && c.heightMeters == t.heightMeters);
        Debug.Log($"REFERENCE_CONTINUATION_DIVERGENCE (diagnostic, not a failure) frozenTrees={frozen100.trees.Count} "
            + $"continuedTrees={continued.trees.Count} identicalToFrozen={same}");
        Debug.Log($"REFERENCE_CONTINUATION_PASS year50=v{year50.version} year100=v{continued.version} "
            + $"deterministic=True continuedHash={continuedHashes[0]} legacyV15LayoutHash={ScenarioReferenceArchive.LegacyV15WorldHash(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(continued))) ?? "n/a"}");
        Debug.Log("REFERENCE_FUTURE_V1_CONTRACT_PASS archive=True preview=True continuation=True replay=NotRequired");
    }

    // Every saved tree is present exactly once with its saved identity,
    // species, position, stage and physical dimensions.
    private static void CheckLiveTreesMatch(ForestSaveData data, string context)
    {
        List<ForestTree> live = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        Check(live.Select(t => t.TreeId).Distinct(StringComparer.Ordinal).Count() == live.Count,
            context + ": duplicate tree ids in the scene");
        Dictionary<string, ForestTree> byId = live.ToDictionary(t => t.TreeId, StringComparer.Ordinal);
        Check(byId.Count == data.trees.Count, $"{context}: {byId.Count} scene trees for {data.trees.Count} saved");
        foreach (TreeSaveData saved in data.trees)
            Check(byId.TryGetValue(saved.treeId, out ForestTree tree)
                && tree.Species != null && tree.Species.SpeciesId == saved.speciesId
                && Mathf.Approximately(tree.transform.position.x, saved.position.x)
                && Mathf.Approximately(tree.transform.position.z, saved.position.z)
                && (int)tree.Stage == saved.stage && tree.AgeYears == saved.ageYears
                && tree.Diameter == saved.diameterCm && tree.SimulationHeightMeters == saved.heightMeters,
                $"{context}: tree {saved.treeId} not shown as stored");
    }

    private static ScenarioReferenceMilestone CaptureFullWorld(ForestSaveController saves, int year)
    {
        ForestSaveData data = saves.CaptureData();
        Check(data.ecologicalYear == year && data.version == ForestSaveData.CurrentVersion,
            "full-world capture has wrong year or incompatible save version");
        return new ScenarioReferenceMilestone
        {
            year = year,
            worldHash = ScenarioReferenceArchive.WorldHash(data),
            world = data
        };
    }

    private static ReferenceMilestone CaptureMilestone(ScenarioOneManager manager,
        ForestEcologyController ecology, ForestTreeSpawner spawner, int year)
    {
        var m = new ReferenceMilestone { year = year };
        List<ForestTree> trees = AllLiving();
        m.totalTrees = trees.Count;
        m.sitkaTrees = trees.Count(t => t.Species?.SpeciesId == "sitka-spruce");
        m.oakTrees = trees.Count(t => t.Species?.SpeciesId == "sessile-oak");
        m.beechTrees = trees.Count(t => t.Species?.SpeciesId == "beech");
        m.originalSitka = trees.Count(t => t.Species?.SpeciesId == "sitka-spruce"
            && t.TreeId.StartsWith("P", StringComparison.Ordinal));
        m.naturalSitka = trees.Count(t => t.Species?.SpeciesId == "sitka-spruce"
            && t.TreeId.StartsWith("R", StringComparison.Ordinal));
        m.naturalBroadleaf = trees.Count(t => t.TreeId.StartsWith("R", StringComparison.Ordinal)
            && (t.Species?.SpeciesId == "beech" || t.Species?.SpeciesId == "sessile-oak"));
        m.plantedOak = trees.Count(t => t.TreeId.StartsWith("PL", StringComparison.Ordinal)
            && t.Species?.SpeciesId == "sessile-oak");
        m.plantedBeech = trees.Count(t => t.TreeId.StartsWith("PL", StringComparison.Ordinal)
            && t.Species?.SpeciesId == "beech");
        m.olderOriginalSitka = trees.Count(t => t.TreeId.StartsWith("P", StringComparison.Ordinal)
            && t.Species?.SpeciesId == "sitka-spruce" && t.AgeYears >= 90);
        m.youngTrees = trees.Count(t => t.AgeYears < 45);
        m.minAge = trees.Count > 0 ? trees.Min(t => t.AgeYears) : 0;
        m.maxAge = trees.Count > 0 ? trees.Max(t => t.AgeYears) : 0;
        var sizeSet = new HashSet<string>();
        foreach (ForestTree t in trees)
            sizeSet.Add(t.SizeClass.ToString());
        m.sizeClasses = sizeSet.Count;

        float canopySum = 0f, lightSum = 0f;
        int regenCells = 0;
        var regenSpecies = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < ecology.CellCount; i++)
        {
            canopySum += ecology.Cells[i].Canopy;
            lightSum += ecology.Cells[i].Light;
            if (ecology.Cells[i].Light >= 0.3f) m.brightCells++;
            if (ecology.Cells[i].Light < 0.1f) m.shadedCells++;
            m.maximumLight = Mathf.Max(m.maximumLight, ecology.Cells[i].Light);
            if (ecology.Cells[i].HasRegeneration)
                regenCells++;
            foreach (ForestRegenerationCohort cohort in ecology.Cells[i].Regeneration)
            {
                if (cohort == null || cohort.Density <= 0f)
                    continue;
                regenSpecies.Add(cohort.SpeciesId);
                if (cohort.SpeciesId == "sitka-spruce")
                {
                    m.sitkaRegen++;
                    m.sitkaRegenDensity += cohort.Density;
                }
                else if (cohort.SpeciesId == "beech")
                    m.beechRegenDensity += cohort.Density;
                else if (cohort.SpeciesId == "sessile-oak")
                    m.oakRegenDensity += cohort.Density;
            }
        }
        m.canopyCover = canopySum / ecology.CellCount;
        m.meanLight = lightSum / ecology.CellCount;
        m.regenCells = regenCells;
        m.largestVeryOpenPatch = LargestVeryOpenPatch(ecology);
        var presentSpecies = new HashSet<string>(regenSpecies, StringComparer.Ordinal);
        foreach (ForestTree tree in trees)
            presentSpecies.Add(tree.Species?.SpeciesId ?? "unknown");
        m.speciesRichness = presentSpecies.Count;
        m.oakRegen = 0;
        m.beechRegen = 0;
        for (int i = 0; i < ecology.CellCount; i++)
        {
            if (ecology.Cells[i].FindCohort("sessile-oak") is { Density: > 0f }) m.oakRegen++;
            if (ecology.Cells[i].FindCohort("beech") is { Density: > 0f }) m.beechRegen++;
        }

        float deadwoodSum = 0f;
        foreach (ScenarioDeadwoodRecord record in manager.DeadwoodRecords)
            if (record != null)
                deadwoodSum += record.remainingVolumeM3;
        m.deadwoodVolume = deadwoodSum;
        m.deadwoodCount = manager.DeadwoodRecords.Count;

        if (manager.UnderstoreyCells != null && manager.UnderstoreyCells.Count > 0)
        {
            m.understoreyEvenness = manager.UnderstoreyCells.Average(c => ScenarioSoundscape.UnderstoreyEvenness(c));
            m.meanGrass = manager.UnderstoreyCells.Average(c => c.grasses);
            m.meanForbs = manager.UnderstoreyCells.Average(c => c.forbs);
            m.meanShrubs = manager.UnderstoreyCells.Average(c => c.shrubs);
            m.meanFerns = manager.UnderstoreyCells.Average(c => c.ferns);
            m.grassyCells = manager.UnderstoreyCells.Count(c => c.grasses > 0.1f);
        }
        if (manager.SoundscapeState != null && manager.SoundscapeState.layers != null)
        {
            foreach (ScenarioSoundscapeLayer layer in manager.SoundscapeState.layers)
                if (layer != null)
                    m.soundscape += $"{layer.layerId}={layer.volume:0.00} ";
        }
        return m;
    }

    private static string YearSummary(ScenarioOneManager manager, ForestEcologyController ecology, int year)
    {
        List<ForestTree> trees = AllLiving();
        int sitka = trees.Count(t => t.Species?.SpeciesId == "sitka-spruce");
        int oak = trees.Count(t => t.Species?.SpeciesId == "sessile-oak");
        int beech = trees.Count(t => t.Species?.SpeciesId == "beech");
        ScenarioAnnualReport last = manager.AnnualReports.LastOrDefault();
        return $"Y{year}: trees={trees.Count} sitka={sitka} oak={oak} beech={beech} "
            + $"canopy={ecology.Cells.Average(c => c.Canopy):0.00} "
            + $"tasks={(last?.completedTasks ?? 0)}/{(last?.failedTasks ?? 0)} "
            + $"timber={((last?.timberRevenueCents ?? 0) / 100f):0.00} "
            + $"deadwood={manager.DeadwoodRecords.Count}";
    }

    private static List<ForestTree> AllLiving()
    {
        return UnityEngine.Object.FindObjectsByType<ForestTree>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t != null && !t.IsStump && !string.IsNullOrEmpty(t.TreeId))
            .ToList();
    }

    private static int LargestVeryOpenPatch(ForestEcologyController ecology)
    {
        int axis = ecology.CellsPerAxis;
        bool[] visited = new bool[ecology.CellCount];
        int largest = 0;
        for (int i = 0; i < ecology.CellCount; i++)
        {
            if (visited[i] || ecology.Cells[i].Light < 0.8f) continue;
            var queue = new Queue<int>();
            queue.Enqueue(i);
            visited[i] = true;
            int patch = 0;
            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                patch++;
                int x = current % axis;
                int z = current / axis;
                if (x > 0) Visit(current - 1);
                if (x + 1 < axis) Visit(current + 1);
                if (z > 0) Visit(current - axis);
                if (z + 1 < axis) Visit(current + axis);
            }
            largest = Mathf.Max(largest, patch);

            void Visit(int index)
            {
                if (visited[index] || ecology.Cells[index].Light < 0.8f) return;
                visited[index] = true;
                queue.Enqueue(index);
            }
        }
        return largest;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

public sealed class ReferenceMilestone
{
    public int year;
    public int totalTrees;
    public int sitkaTrees;
    public int oakTrees;
    public int beechTrees;
    public int originalSitka;
    public int naturalSitka;
    public int naturalBroadleaf;
    public int plantedOak;
    public int plantedBeech;
    public int olderOriginalSitka;
    public int youngTrees;
    public int minAge;
    public int maxAge;
    public int sitkaRegen;
    public int oakRegen;
    public int beechRegen;
    public float sitkaRegenDensity;
    public float oakRegenDensity;
    public float beechRegenDensity;
    public int regenCells;
    public int speciesRichness;
    public int sizeClasses;
    public float canopyCover;
    public float meanLight;
    public float maximumLight;
    public int brightCells;
    public int shadedCells;
    public int largestVeryOpenPatch;
    public float deadwoodVolume;
    public int deadwoodCount;
    public float understoreyEvenness;
    public float meanGrass;
    public float meanForbs;
    public float meanShrubs;
    public float meanFerns;
    public int grassyCells;
    public string soundscape = "";

    public string Describe()
    {
        return $"trees={totalTrees} (sitka={sitkaTrees} oak={oakTrees} beech={beechTrees}) "
            + $"originalSitka={originalSitka} olderOriginal={olderOriginalSitka} naturalSitka={naturalSitka} "
            + $"plantedOak={plantedOak} plantedBeech={plantedBeech} naturalBroadleaf={naturalBroadleaf} "
            + $"ageRange={minAge}-{maxAge} youngTrees={youngTrees} "
            + $"sizeClasses={sizeClasses} richness={speciesRichness} "
            + $"canopy={canopyCover:0.00} light={meanLight:0.00} maxLight={maximumLight:0.00} "
            + $"brightCells={brightCells} shadedCells={shadedCells} "
            + $"largestVeryOpenPatch={largestVeryOpenPatch} "
            + $"regenCells={regenCells} sitkaRegen={sitkaRegen}/{sitkaRegenDensity:0.00} "
            + $"oakRegen={oakRegen}/{oakRegenDensity:0.00} "
            + $"beechRegen={beechRegen}/{beechRegenDensity:0.00} "
            + $"deadwood={deadwoodCount}/{deadwoodVolume:0.00}m³ "
            + $"understoreyEvenness={understoreyEvenness:0.00} "
            + $"grass={meanGrass:0.00} forb={meanForbs:0.00} shrub={meanShrubs:0.00} "
            + $"fern={meanFerns:0.00} grassyCells={grassyCells} "
            + $"soundscape[{soundscape}]";
    }
}

// ARCHIVE integrity for Reference Future v1, independent of the archive
// loader. Each milestone's "world" object and the "schedule" object are taken
// from the archive's original embedded JSON text, whitespace outside strings
// removed (the frozen hashes were computed on compact JSON), and hashed with
// the archive's own FNV-1a. Nothing is deserialised into current save classes
// and re-serialised. The pinned values are the frozen v1 anchors.
public static class ReferenceArchiveIntegrity
{
    private const string ArchivePath = "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes";
    private const string SchedulePath = "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json";
    private static readonly Dictionary<int, string> PinnedWorlds = new Dictionary<int, string>
    {
        { 0, "A564039D9B7CE31D" }, { 20, "F7DF7DAB53B6FD32" }, { 50, "D5E75D6D21D631AC" }, { 100, "7AD177B3CC2F73C7" }
    };
    private const string PinnedSchedule = "56C8B99FA1E8DDD1";

    public static void Verify()
    {
        string text;
        using (var compressed = new MemoryStream(File.ReadAllBytes(ArchivePath)))
        using (var decoder = new GZipStream(compressed, CompressionMode.Decompress))
        using (var json = new MemoryStream())
        {
            decoder.CopyTo(json);
            text = Encoding.UTF8.GetString(json.ToArray());
        }
        VerifyText(text, true);
        Debug.Log($"REFERENCE_ARCHIVE_INTEGRITY_PASS milestones=0,20,50,100 schedule={PinnedSchedule} "
            + $"archiveFileFnv={ScenarioReferenceArchive.Hash(Convert.ToBase64String(File.ReadAllBytes(ArchivePath)))}");

        // Negative control: one changed value inside the Year-100 embedded world
        // (in memory only) must be rejected.
        int lastWorld = text.LastIndexOf("\"world\"", StringComparison.Ordinal);
        int target = text.IndexOf("\"diameterCm\"", lastWorld, StringComparison.Ordinal);
        Require(lastWorld >= 0 && target > lastWorld, "negative control could not locate a Year-100 tree value");
        int digit = target + "\"diameterCm\"".Length;
        while (digit < text.Length && !char.IsDigit(text[digit])) digit++;
        string tampered = text.Substring(0, digit) + (text[digit] == '9' ? '8' : (char)(text[digit] + 1)) + text.Substring(digit + 1);
        bool rejected = false;
        try { VerifyText(tampered, false); }
        catch (InvalidOperationException) { rejected = true; }
        Require(rejected, "a changed Year-100 value was not detected");
        Debug.Log("REFERENCE_ARCHIVE_NEGATIVE_CONTROL_PASS tamperedYear100Value=detected");
    }

    private static void VerifyText(string text, bool log)
    {
        // Stored metadata (strings and ints only; schema-insensitive reads).
        ScenarioReferenceArchive stored = JsonUtility.FromJson<ScenarioReferenceArchive>(text);
        Require(stored != null && stored.referenceId == "reference-future-v1" && stored.saveVersion == 12
            && stored.milestones != null && stored.milestones.Count == 4, "archive metadata is not Reference Future v1");

        List<string> worlds = EmbeddedObjects(text, "world");
        Require(worlds.Count == stored.milestones.Count, $"found {worlds.Count} embedded worlds for {stored.milestones.Count} milestones");
        for (int i = 0; i < worlds.Count; i++)
        {
            ScenarioReferenceMilestone milestone = stored.milestones[i];
            string hash = ScenarioReferenceArchive.Hash(worlds[i]);
            Require(PinnedWorlds.TryGetValue(milestone.year, out string pinned), "unexpected milestone year " + milestone.year);
            Require(hash == milestone.worldHash && hash == pinned && milestone.world != null && milestone.world.version == 12,
                $"Year-{milestone.year} embedded world hash {hash} (stored {milestone.worldHash}, pinned {pinned})");
            if (log) Debug.Log($"REFERENCE_ARCHIVE_WORLD year={milestone.year} embeddedHash={hash} stored={milestone.worldHash} pinned=True version=12");
        }
        Require(stored.startingStandHash == PinnedWorlds[0], "starting-stand hash is not the Year-0 anchor");

        List<string> schedules = EmbeddedObjects(text, "schedule");
        Require(schedules.Count == 1, "archive must embed exactly one schedule");
        string scheduleHash = ScenarioReferenceArchive.Hash(schedules[0]);
        Require(scheduleHash == stored.scheduleHash && scheduleHash == PinnedSchedule,
            $"embedded schedule hash {scheduleHash} (stored {stored.scheduleHash}, pinned {PinnedSchedule})");
        string scheduleFile = Compact(File.ReadAllText(SchedulePath), 0, out _);
        Require(scheduleFile == schedules[0], "schedule resource file differs from the archive's embedded schedule");
    }

    // Compact JSON of every object value whose key is exactly "key", in order.
    private static List<string> EmbeddedObjects(string text, string key)
    {
        var result = new List<string>();
        string token = "\"" + key + "\"";
        int at = 0;
        while ((at = text.IndexOf(token, at, StringComparison.Ordinal)) >= 0)
        {
            int colon = at + token.Length;
            while (colon < text.Length && char.IsWhiteSpace(text[colon])) colon++;
            if (colon >= text.Length || text[colon] != ':') { at += token.Length; continue; }
            int open = colon + 1;
            while (open < text.Length && char.IsWhiteSpace(text[open])) open++;
            if (open < text.Length && text[open] == '{')
            {
                result.Add(Compact(text, open, out int end));
                at = end;
            }
            else at = open;
        }
        return result;
    }

    // Copies one balanced JSON value starting at 'start' (or the whole text if
    // it starts with whitespace before '{'), dropping whitespace outside strings.
    private static string Compact(string text, int start, out int end)
    {
        var sb = new StringBuilder();
        bool quoted = false, escaped = false;
        int depth = 0;
        int i = start;
        for (; i < text.Length; i++)
        {
            char c = text[i];
            if (!quoted && char.IsWhiteSpace(c)) continue;
            sb.Append(c);
            if (quoted)
            {
                if (escaped) escaped = false;
                else if (c == '\\') escaped = true;
                else if (c == '"') quoted = false;
                continue;
            }
            if (c == '"') quoted = true;
            else if (c == '{' || c == '[') depth++;
            else if ((c == '}' || c == ']') && --depth == 0) { i++; break; }
        }
        end = i;
        return sb.ToString();
    }

    private static void Require(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException("Reference archive integrity: " + message);
    }
}

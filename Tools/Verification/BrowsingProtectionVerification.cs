#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

// Browsing & Protection v1 calibration and verification gate
// (Docs/BrowsingProtectionV1.md). Copy into Assets/ForestPrototype for an
// explicit batch run, then remove the copy and its generated .meta:
//   -executeMethod BrowsingProtectionVerification.Begin
// It never writes the save slot: the scene world is captured in memory first
// and restored at the end.
public static class BrowsingProtectionVerification
{
    private const string Requested = "BrowsingProtectionVerification.Requested";

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
        new GameObject("Browsing protection gate").AddComponent<BrowsingProtectionGate>();
    }
}

public sealed class BrowsingProtectionGate : MonoBehaviour
{
    private static readonly float[] Lights = { 0.08f, 0.18f, 0.35f, 0.7f };
    private static readonly string[] LightNames = { "poor", "marginal", "moderate", "strong" };
    private static readonly float[] Pressures = { 0f, 0.2f, 0.5f, 0.85f };
    private static readonly string[] PressureNames = { "none", "low", "moderate", "high" };
    private static readonly string[] Protections = { "none", "shelter", "fence" };
    private static readonly int[] Snapshots = { 5, 10, 20, 40 };
    private const int Years = 40;
    private const int Individuals = 60;
    private const int MatrixBaseYear = 200;

    private ForestEcologyController ecology;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestTreeSpawner spawner;
    private ForestSaveData original;
    private readonly StringBuilder measured = new StringBuilder();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static string F(float v) => v.ToString("0.000", CultureInfo.InvariantCulture);

    private void Emit(string line)
    {
        measured.AppendLine(line);
        Debug.Log(line);
    }

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator run = Verify();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        IEnumerator cleanup = Cleanup();
        while (true)
        {
            bool more; object current = null;
            try { more = cleanup.MoveNext(); if (more) current = cleanup.Current; }
            catch (Exception error) { failure = failure ?? error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("BROWSING_PROTECTION_VERIFY_PASS");
        else Debug.LogError("BROWSING_PROTECTION_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private readonly Dictionary<string, object> generatorSettings = new Dictionary<string, object>();
    private float originalStandSize;

    private static readonly string[] GeneratorFields =
        { "standWidthMeters", "standDepthMeters", "latticePerAxis", "targetTreeCount", "pathCorridorRadiusMeters", "clearingRadiusMeters" };

    private IEnumerator Cleanup()
    {
        if (ecology == null || saves == null || original == null) yield break;
        ecology.Browsing.BackgroundPressure = 0f;
        ecology.Browsing.ClearProtection();
        DestroyHarnessTrees();
        // The performance section resizes the stand; restore the scene's grid
        // and generator configuration before reloading the captured world.
        ForestStartingStand generator = FindFirstObjectByType<ForestStartingStand>();
        if (generator != null)
            foreach (var pair in generatorSettings) Field(typeof(ForestStartingStand), pair.Key).SetValue(generator, pair.Value);
        Field(typeof(ForestEcologyController), "standSizeMeters").SetValue(ecology, originalStandSize);
        ecology.RebuildGrid();
        Check(saves.LoadData(original, false), "could not restore the captured scene world");
        yield return null;
        yield return null;
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == ScenarioReferenceArchive.WorldHash(original),
            "scene world not restored after the gate");
        Debug.Log("BROWSE_SCENE_RESTORED_PASS");
    }

    private IEnumerator Verify()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        Check(ecology != null && manager != null && saves != null && spawner != null, "scene controllers missing");
        original = saves.CaptureData();
        originalStandSize = (float)Field(typeof(ForestEcologyController), "standSizeMeters").GetValue(ecology);
        ForestStartingStand sceneGenerator = FindFirstObjectByType<ForestStartingStand>();
        if (sceneGenerator != null)
            foreach (string name in GeneratorFields) generatorSettings[name] = Field(typeof(ForestStartingStand), name).GetValue(sceneGenerator);
        Check(ecology.Browsing.BackgroundPressure == 0f && !ecology.Browsing.HasProtection,
            "production ForestTest must start with browsing off");

        TreeSpeciesDefinition sitka = spawner.ResolveSpecies("sitka-spruce");
        TreeSpeciesDefinition oak = spawner.ResolveSpecies("sessile-oak");
        TreeSpeciesDefinition beech = spawner.ResolveSpecies("beech");
        Check(sitka != null && oak != null && beech != null, "species missing");
        Emit($"BROWSE_PARAMETERS cap={JuvenileEcologyRules.BrowseProbabilityCap} incrementLoss={JuvenileEcologyRules.BrowseHeightIncrementLoss} "
            + $"extraMortality={JuvenileEcologyRules.BrowseExtraMortality} "
            + string.Join(" ", new[] { sitka, oak, beech }.Select(s =>
                $"{s.SpeciesId}=pal{s.BrowsePalatability}/full{s.BrowseFullVulnerabilityHeightM}/escape{s.BrowseEscapeHeightM}")));
        Check(oak.BrowsePalatability == 1f && beech.BrowsePalatability == 0.5f && sitka.BrowsePalatability == 0.15f,
            "species palatability differs from the documented v1 values");

        VerifyNeutralRule(new[] { sitka, oak, beech });
        VerifyRuleUnits(oak, sitka);
        Emit("BROWSE_RULE_UNITS_PASS");

        // Matrix through the real cohort and exact-individual adapters at fixed light.
        var results = new Dictionary<string, Outcome>();
        foreach (TreeSpeciesDefinition species in new[] { sitka, oak, beech })
        for (int l = 0; l < Lights.Length; l++)
        {
            for (int p = 0; p < Pressures.Length; p++)
            foreach (string protection in Protections)
            {
                Outcome o = RunCondition(species, Lights[l], Pressures[p], protection, null);
                string key = Key(species, l, p, protection);
                results[key] = o;
                Emit($"BROWSE_MATRIX {key} " + o.Describe());
            }
            yield return null;
        }
        Debug.Log($"BROWSE_MATRIX_HASH {Fnv64(string.Join("\n", results.OrderBy(r => r.Key, StringComparer.Ordinal).Select(r => r.Key + r.Value.Describe())))}");
        VerifyOutcomes(results, sitka, oak, beech);
        yield return null;

        VerifyEquivalence(oak);
        yield return null;
        VerifyExperiments(oak);
        yield return null;

        IEnumerator production = VerifyProductionContinuity(oak, beech);
        while (production.MoveNext()) yield return production.Current;
        IEnumerator lifecycle = VerifyLifecycle();
        while (lifecycle.MoveNext()) yield return lifecycle.Current;
        IEnumerator perf = MeasurePerformance(oak, sitka);
        while (perf.MoveNext()) yield return perf.Current;

        Sensitivity(oak, beech, sitka);
        Debug.Log($"BROWSE_MEASUREMENT_HASH {Fnv64(measured.ToString().Replace("\r", ""), excludeTiming: true)}");
    }

    // ---------- 1. Neutral equivalence ----------

    private static void VerifyNeutralRule(TreeSpeciesDefinition[] species)
    {
        foreach (TreeSpeciesDefinition s in species)
        foreach (float light in new[] { 0f, 0.05f, 0.1f, 0.18f, 0.25f, 0.55f, 1f })
        foreach (float height in new[] { 0.15f, 0.6f, 1.5f, 3f })
        {
            float a = height, b = height;
            JuvenileEcologyRules.GrowHeight(ref a, s, light, 0.8f);
            JuvenileEcologyRules.GrowHeight(ref b, s, light, 0.8f, 0f);
            Check(a == b, "zero browsing changed height growth");
            Check(JuvenileEcologyRules.SurvivalResponse(s, light) == JuvenileEcologyRules.SurvivalResponse(s, light, 0f),
                "zero browsing changed survival");
            foreach (float roll in new[] { 0f, 0.3f, 0.79f, 0.81f, 0.999f })
                Check(JuvenileEcologyRules.Survives(s, light, roll) == JuvenileEcologyRules.Survives(s, light, roll, false),
                    "unbrowsed individual survival changed");
            Check(JuvenileEcologyRules.AssessBrowse(s, height, 0f, 1f, BrowseProtectionState.None).Probability == 0f,
                "zero pressure produced a browse probability");
            // Protection never adds growth: a browsed fraction only lowers the increment.
            float browsed = height;
            JuvenileEcologyRules.GrowHeight(ref browsed, s, light, 0.8f, 1f);
            Check(browsed <= a, "browsing increased growth");
        }
        Debug.Log("BROWSE_NEUTRAL_RULE_PASS");
    }

    private void VerifyRuleUnits(TreeSpeciesDefinition oak, TreeSpeciesDefinition sitka)
    {
        Check(JuvenileEcologyRules.BrowseHeightVulnerability(oak, 1.0f) == 1f
              && Mathf.Abs(JuvenileEcologyRules.BrowseHeightVulnerability(oak, 1.5f) - 0.5f) < 1e-4f
              && JuvenileEcologyRules.BrowseHeightVulnerability(oak, 1.8f) == 0f, "oak height taper");
        Check(JuvenileEcologyRules.BrowseHeightVulnerability(sitka, 0.5f) == 1f
              && JuvenileEcologyRules.BrowseHeightVulnerability(sitka, 0.8f) == 0f, "Sitka height taper");
        BrowseAssessment a = JuvenileEcologyRules.AssessBrowse(oak, 0.5f, 1f, 1f, BrowseProtectionState.None);
        Check(a.Probability == JuvenileEcologyRules.BrowseProbabilityCap && a.Reason == BrowseExposureReason.Exposed, "cap");
        Check(JuvenileEcologyRules.AssessBrowse(oak, 0.5f, 0.6f, 0f, BrowseProtectionState.InsideIntactFence).Reason
              == BrowseExposureReason.Protected, "protected reason");
        Check(JuvenileEcologyRules.AssessBrowse(oak, 2.5f, 0.6f, 1f, BrowseProtectionState.None).Reason
              == BrowseExposureReason.AboveBrowseReach, "escaped reason");

        var c = new BrowsingConditions();
        var area = new BrowseProtectedArea { areaId = "F1", installedYear = 10, breachedYear = 15,
            polygon = new List<Vector2> { new Vector2(0, 0), new Vector2(10, 0), new Vector2(10, 10), new Vector2(0, 10) } };
        c.ProtectedAreas.Add(area);
        Check(c.ProtectionAt(new Vector2(5, 5), 9, out float access9) == BrowseProtectionState.None && access9 == 1f, "fence before install");
        Check(c.ProtectionAt(new Vector2(5, 5), 12, out float access12) == BrowseProtectionState.InsideIntactFence && access12 == 0f, "intact fence");
        Check(c.ProtectionAt(new Vector2(5, 5), 15, out float access15) == BrowseProtectionState.BreachedFence && access15 == 1f, "breached fence");
        Check(c.ProtectionAt(new Vector2(12, 5), 12, out float outside) == BrowseProtectionState.None && outside == 1f, "outside fence");
        // A fence covering exactly half of a 5 m cell protects half the cohort.
        Check(Mathf.Abs(c.CohortAccess(new Vector2(10f, 5f), 5f, 12) - 0.6f) < 0.11f, "half-cell cohort access");
        Check(c.CohortAccess(new Vector2(5f, 5f), 5f, 12) == 0f && c.CohortAccess(new Vector2(5f, 5f), 5f, 16) == 1f, "cohort fence/breach");
        c.Shelters.Add(new BrowseShelter { shelterId = "S1", position = new Vector2(20, 20), installedYear = 3, effectiveYears = 8, failedYear = -1 });
        c.Shelters.Add(new BrowseShelter { shelterId = "S2", position = new Vector2(30, 30), installedYear = 3, effectiveYears = 8, failedYear = 6 });
        Check(c.ProtectionAt(new Vector2(20.1f, 20f), 10, out _) == BrowseProtectionState.EffectiveShelter, "shelter effective");
        Check(c.ProtectionAt(new Vector2(20.1f, 20f), 11, out float expired) == BrowseProtectionState.ExpiredShelter && expired == 1f, "shelter expiry");
        Check(c.ProtectionAt(new Vector2(30f, 30f), 6, out float failed) == BrowseProtectionState.FailedShelter && failed == 1f, "shelter failure");
        Check(c.ProtectionAt(new Vector2(20.5f, 20f), 5, out _) == BrowseProtectionState.None, "shelter radius");
    }

    // ---------- 2. Matrix ----------

    private sealed class Outcome
    {
        public readonly float[] CohortHeight = new float[Snapshots.Length];
        public readonly float[] CohortDensity = new float[Snapshots.Length];
        public readonly float[] Survival = new float[Snapshots.Length];
        public readonly float[] MeanHeight = new float[Snapshots.Length];
        public readonly float[] Promoted = new float[Snapshots.Length];
        public readonly float[] MeanBrowseEvents = new float[Snapshots.Length];
        public readonly float[] RepeatedBrowsed = new float[Snapshots.Length];
        public int CohortPromotionYear = -1;
        public float MedianPromotionYear = -1f;

        public string Describe()
        {
            var b = new StringBuilder();
            for (int i = 0; i < Snapshots.Length; i++)
                b.Append($" y{Snapshots[i]}[cohort h={F(CohortHeight[i])} d={F(CohortDensity[i])} | ind surv={F(Survival[i])} h={F(MeanHeight[i])} prom={F(Promoted[i])} browse={F(MeanBrowseEvents[i])} rep3={F(RepeatedBrowsed[i])}]");
            b.Append($" cohortPromotionYear={CohortPromotionYear} medianPromotionYear={MedianPromotionYear.ToString("0.0", CultureInfo.InvariantCulture)}");
            return b.ToString();
        }
    }

    private static string Key(TreeSpeciesDefinition s, int l, int p, string protection)
        => $"{s.SpeciesId}/{LightNames[l]}/{PressureNames[p]}/{protection}";

    // Optional per-year light override (reclosure experiment); otherwise fixed light.
    private Outcome RunCondition(TreeSpeciesDefinition species, float light, float pressure, string protection,
        Func<int, float> lightByYear, int breachYear = -1, int shelterYears = 8, int years = Years)
    {
        foreach (ForestEcologyCell cell in ecology.Cells) cell.ClearRegeneration();
        DestroyHarnessTrees();
        int cohortCell = 9, individualCell = 18;
        ForestEcologyCell cCell = ecology.Cells[cohortCell], iCell = ecology.Cells[individualCell];
        SetYear(MatrixBaseYear);

        ecology.Browsing.BackgroundPressure = pressure;
        ecology.Browsing.ClearProtection();
        var positions = new List<Vector3>();
        for (int i = 0; i < Individuals; i++)
        {
            // Deterministic spread inside the individual cell (stays inside its 5 m square).
            float ox = ((i % 8) - 3.5f) * 0.55f, oz = ((i / 8) - 3.5f) * 0.55f;
            var position = new Vector3(iCell.Center.x + ox, 0f, iCell.Center.y + oz);
            Check(ecology.GetCellIndex(position) == individualCell, "harness juvenile left its cell");
            positions.Add(position);
        }
        if (protection == "fence")
        {
            foreach (int index in new[] { cohortCell, individualCell })
            {
                Vector2 c = ecology.Cells[index].Center;
                ecology.Browsing.ProtectedAreas.Add(new BrowseProtectedArea
                {
                    areaId = "fence-" + index, installedYear = MatrixBaseYear, breachedYear = breachYear,
                    polygon = new List<Vector2> { c + new Vector2(-3, -3), c + new Vector2(3, -3), c + new Vector2(3, 3), c + new Vector2(-3, 3) }
                });
            }
        }
        else if (protection == "shelter")
        {
            for (int i = 0; i < positions.Count; i++)
                ecology.Browsing.Shelters.Add(new BrowseShelter
                {
                    shelterId = "shelter-" + i, position = new Vector2(positions[i].x, positions[i].z),
                    installedYear = MatrixBaseYear, effectiveYears = shelterYears
                });
        }

        // Natural cohort: newly established seedlings. Individuals: nursery stock.
        ForestRegenerationCohort cohort = cCell.GetOrCreateCohort(species);
        cohort.Restore(0.5f, species.RegenInitialHeightM, MatrixBaseYear, RegenerationOrigin.Natural, MatrixBaseYear);
        ScenarioOneSaveData state = JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(original.scenarioOne));
        state.interactionSchemaVersion = 2;
        state.plantedJuveniles.Clear();
        for (int i = 0; i < positions.Count; i++)
            state.plantedJuveniles.Add(new PlantedJuvenileSaveData
            {
                juvenileId = "PJ" + (8000 + i).ToString("0000"), speciesId = species.SpeciesId, position = positions[i],
                cellIndex = individualCell, plantingYear = MatrixBaseYear, ageYears = 3, heightMeters = 0.6f, alive = true
            });
        manager.RestoreSaveData(state);

        var outcome = new Outcome();
        var events = new Dictionary<string, int>();
        var promotionYear = new Dictionary<string, int>();
        bool cohortDone = false;
        int snapshot = 0;
        for (int y = 1; y <= years; y++)
        {
            SetYear(MatrixBaseYear + y);
            float l = lightByYear != null ? lightByYear(y) : light;
            foreach (ForestEcologyCell cell in new[] { cCell, iCell }) { cell.Light = l; cell.SiteProductivity = 1f; }
            Invoke(ecology, "GrowExistingRegeneration");
            Invoke(manager, "AdvancePlantedJuveniles");
            foreach (PlantedJuvenile j in manager.PlantedJuveniles)
            {
                if (j.lastYearBrowsed && j.lastBrowseAssessmentYear == MatrixBaseYear + y)
                    events[j.juvenileId] = (events.TryGetValue(j.juvenileId, out int n) ? n : 0) + 1;
                if (!string.IsNullOrEmpty(j.promotedTreeId) && !promotionYear.ContainsKey(j.juvenileId))
                    promotionYear[j.juvenileId] = y;
            }
            if (!cohortDone && cohort.Density > 0f && JuvenileEcologyRules.CanPromote(species, cohort.Height, l))
            {
                outcome.CohortPromotionYear = y;
                cohortDone = true;
            }
            if (snapshot < Snapshots.Length && y == Snapshots[snapshot])
            {
                List<PlantedJuvenile> all = manager.PlantedJuveniles.ToList();
                List<PlantedJuvenile> growing = all.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)).ToList();
                outcome.CohortHeight[snapshot] = cohort.Density > 0f ? cohort.Height : 0f;
                outcome.CohortDensity[snapshot] = cohort.Density;
                outcome.Survival[snapshot] = all.Count(j => j.alive) / (float)all.Count;
                outcome.MeanHeight[snapshot] = growing.Count > 0 ? growing.Average(j => j.heightMeters) : 0f;
                outcome.Promoted[snapshot] = all.Count(j => !string.IsNullOrEmpty(j.promotedTreeId)) / (float)all.Count;
                outcome.MeanBrowseEvents[snapshot] = (float)all.Average(j => events.TryGetValue(j.juvenileId, out int n) ? n : 0);
                outcome.RepeatedBrowsed[snapshot] = all.Count(j => events.TryGetValue(j.juvenileId, out int n) && n >= 3) / (float)all.Count;
                snapshot++;
            }
        }
        if (promotionYear.Count * 2 >= Individuals)
        {
            List<int> ordered = promotionYear.Values.OrderBy(v => v).ToList();
            outcome.MedianPromotionYear = ordered[Individuals / 2 - 1];
        }
        DestroyHarnessTrees();
        return outcome;
    }

    private void VerifyOutcomes(Dictionary<string, Outcome> r, TreeSpeciesDefinition sitka, TreeSpeciesDefinition oak, TreeSpeciesDefinition beech)
    {
        int Y(int year) => Array.IndexOf(Snapshots, year);
        foreach (TreeSpeciesDefinition s in new[] { sitka, oak, beech })
        for (int l = 0; l < Lights.Length; l++)
        {
            Outcome baseline = r[Key(s, l, 0, "none")];
            for (int p = 0; p < Pressures.Length; p++)
            {
                // (1)/(D) Protection removes browsing only: an intact fence gives
                // the browse-free trajectory exactly, never anything better.
                Outcome fenced = r[Key(s, l, p, "fence")];
                Check(fenced.Describe() == baseline.Describe(),
                    $"fenced outcome differs from the no-browsing baseline: {Key(s, l, p, "fence")}");
                // Shelter protects individuals for its working life; cohorts are not sheltered.
                Outcome sheltered = r[Key(s, l, p, "shelter")];
                Check(sheltered.Promoted[Y(40)] >= r[Key(s, l, p, "none")].Promoted[Y(40)] - 1e-6f,
                    $"shelter worsened promotion: {Key(s, l, p, "shelter")}");
            }
            // (5) Low pressure stays close to the baseline.
            Outcome low = r[Key(s, l, 1, "none")];
            Check(low.Survival[Y(20)] >= baseline.Survival[Y(20)] - 0.1f, $"low pressure survival collapse {Key(s, l, 1, "none")}");
            if (baseline.MedianPromotionYear > 0f)
                Check(low.MedianPromotionYear > 0f && low.MedianPromotionYear <= baseline.MedianPromotionYear + 2f,
                    $"low pressure delayed promotion by more than 2 years {Key(s, l, 1, "none")}");
        }

        // (1)/(9) Poor light: protection never turns shade into recruitment.
        // Protected outcomes can never beat the browse-free baseline (checked
        // exactly above for fences); where shade alone prevents recruitment
        // (oak, Sitka at 0.08 light) protection cannot rescue it. Shade-tolerant
        // beech slowly recruits at this light under the existing light model.
        foreach (TreeSpeciesDefinition s in new[] { sitka, oak })
        foreach (int p in new[] { 0, 3 })
        foreach (string protection in Protections)
        {
            Outcome poor = r[Key(s, 0, p, protection)];
            Check(poor.Promoted[Y(40)] == 0f && poor.CohortPromotionYear < 0,
                $"poor light recruited despite shade: {Key(s, 0, p, protection)}");
        }
        foreach (int p in new[] { 1, 2, 3 })
        foreach (string protection in Protections)
            Check(r[Key(beech, 0, p, protection)].Promoted[Y(40)] <= r[Key(beech, 0, 0, "none")].Promoted[Y(40)] + 1e-6f,
                $"protection beat the browse-free baseline for shaded beech: {Key(beech, 0, p, protection)}");

        Outcome oakStrongNone = r[Key(oak, 3, 0, "none")];
        Outcome oakStrongHigh = r[Key(oak, 3, 3, "none")];
        Outcome oakStrongModerate = r[Key(oak, 3, 2, "none")];
        Outcome oakStrongHighShelter = r[Key(oak, 3, 3, "shelter")];
        // (2) Good light + high pressure substantially delays/prevents recruitment.
        Check(oakStrongHigh.Promoted[Y(10)] <= 0.5f * oakStrongNone.Promoted[Y(10)],
            "high pressure did not substantially delay strong-light oak");
        Check(oakStrongHigh.MeanBrowseEvents[Y(10)] >= 3f && oakStrongHigh.RepeatedBrowsed[Y(10)] >= 0.5f,
            "high pressure did not produce repeated browsing of oak");
        // (6) Not an instant-death switch.
        Check(oakStrongHigh.Survival[Y(10)] >= 0.5f, "high pressure behaved like instant death");
        // (3) Protection improves outcomes under meaningful pressure.
        Check(oakStrongHighShelter.Promoted[Y(10)] > oakStrongHigh.Promoted[Y(10)] + 0.25f
              && r[Key(oak, 3, 3, "fence")].Promoted[Y(10)] > oakStrongHigh.Promoted[Y(10)] + 0.25f,
            "protection did not improve high-pressure oak");
        Check(oakStrongModerate.Promoted[Y(10)] < oakStrongNone.Promoted[Y(10)]
              || oakStrongModerate.MedianPromotionYear > oakStrongNone.MedianPromotionYear,
            "moderate pressure had no effect on strong-light oak");
        // (4) Protection does not guarantee promotion: marginal-light oak stays
        // below promotion light, so even fenced oak cannot recruit.
        Check(r[Key(oak, 1, 3, "fence")].Promoted[Y(40)] == 0f, "fenced marginal-light oak recruited");
        // (7) Species ordering under identical high pressure, strong light, 10 years.
        float Delay(TreeSpeciesDefinition s) => r[Key(s, 3, 0, "none")].Promoted[Y(10)] - r[Key(s, 3, 3, "none")].Promoted[Y(10)];
        float Events(TreeSpeciesDefinition s) => r[Key(s, 3, 3, "none")].MeanBrowseEvents[Y(10)];
        Check(Events(oak) > Events(beech) && Events(beech) > Events(sitka), "browse event ordering oak > beech > Sitka broken");
        Check(Delay(oak) >= Delay(beech) && Delay(beech) >= Delay(sitka) - 1e-6f, "recruitment delay ordering oak >= beech >= Sitka broken");
        Emit($"BROWSE_OUTCOMES strongLight oak promoted10 none={F(oakStrongNone.Promoted[Y(10)])} moderate={F(oakStrongModerate.Promoted[Y(10)])} "
            + $"high={F(oakStrongHigh.Promoted[Y(10)])} highShelter={F(oakStrongHighShelter.Promoted[Y(10)])} "
            + $"highSurvival10={F(oakStrongHigh.Survival[Y(10)])} highEvents10={F(oakStrongHigh.MeanBrowseEvents[Y(10)])} "
            + $"events10 oak={F(Events(oak))} beech={F(Events(beech))} sitka={F(Events(sitka))}");
        Debug.Log("BROWSE_REQUIRED_OUTCOMES_PASS");
    }

    // ---------- 3. Natural/planted equivalence ----------

    private void VerifyEquivalence(TreeSpeciesDefinition oak)
    {
        // Same inputs: one natural cohort and many exact individuals for one year.
        foreach (float light in new[] { 0.12f, 0.55f })
        {
            const int n = 4000;
            const float pressure = 0.6f, h0 = 0.6f;
            foreach (ForestEcologyCell cell in ecology.Cells) cell.ClearRegeneration();
            ecology.Browsing.BackgroundPressure = pressure;
            ecology.Browsing.ClearProtection();
            SetYear(MatrixBaseYear + 1);
            ForestEcologyCell c = ecology.Cells[9];
            c.Light = light; c.SiteProductivity = 1f;
            ForestRegenerationCohort natural = c.GetOrCreateCohort(oak);
            natural.Restore(0.5f, h0, MatrixBaseYear, RegenerationOrigin.Natural, MatrixBaseYear);
            ForestEcologyCell legacyCell = ecology.Cells[10];
            legacyCell.Light = light; legacyCell.SiteProductivity = 1f;
            ForestRegenerationCohort legacy = legacyCell.GetOrCreateCohort(oak);
            legacy.Restore(0.5f, h0, MatrixBaseYear - 3, RegenerationOrigin.Planted, MatrixBaseYear);
            Invoke(ecology, "GrowExistingRegeneration");
            Check(natural.Height == legacy.Height && natural.Density == legacy.Density, "natural/legacy planted cohort browsing differs");

            float p = JuvenileEcologyRules.AssessBrowse(oak, h0, pressure, 1f, BrowseProtectionState.None).Probability;
            double s = JuvenileEcologyRules.SurvivalResponse(oak, light);
            int browsed = 0, alive = 0, aliveBrowsed = 0;
            double sumHeight = 0;
            for (int i = 0; i < n; i++)
            {
                string id = "EQ" + i.ToString("00000");
                bool b = JuvenileEcologyRules.RealiseBrowse(p, ecology.RngModelVersion, id, ecology.EcologicalYear, ecology.SimulationSeed);
                float h = h0;
                JuvenileEcologyRules.GrowHeight(ref h, oak, light, 1f, b ? 1f : 0f);
                bool survives = JuvenileEcologyRules.Survives(oak, light,
                    SimulationRandom.Roll(ecology.RngModelVersion, id, ecology.EcologicalYear, ecology.SimulationSeed), b);
                if (b) browsed++;
                if (survives) { alive++; if (b) aliveBrowsed++; }
                sumHeight += h;
            }
            double meanHeight = sumHeight / n;
            double expectedIncrement = natural.Height - h0;
            double increment = meanHeight - h0;
            double fullIncrement = oak.RegenHeightGrowthMPerYear * oak.JuvenileLightResponse(light);
            double sePb = Math.Sqrt(p * (1 - p) / n);
            Check(Math.Abs(browsed / (double)n - p) < 4 * sePb + 1e-9, $"individual browse rate {browsed / (double)n} vs p {p}");
            Check(Math.Abs(increment - expectedIncrement) <= fullIncrement * JuvenileEcologyRules.BrowseHeightIncrementLoss * 4 * sePb + 1e-6,
                $"individual mean increment {increment} vs cohort {expectedIncrement}");
            double expectedSurvival = s * (1 - p * JuvenileEcologyRules.BrowseExtraMortality);
            double seS = Math.Sqrt(expectedSurvival * (1 - expectedSurvival) / n) + 1e-9;
            Check(Math.Abs(alive / (double)n - expectedSurvival) < 4 * seS + 1e-6, $"individual survival {alive / (double)n} vs expected {expectedSurvival}");
            // Browse and survival rolls are independent draws.
            double survivalGivenBrowsed = browsed > 0 ? aliveBrowsed / (double)browsed : 1;
            double expectedGivenBrowsed = s * (1 - JuvenileEcologyRules.BrowseExtraMortality);
            Check(browsed == 0 || Math.Abs(survivalGivenBrowsed - expectedGivenBrowsed) < 4 * Math.Sqrt(expectedGivenBrowsed * (1 - expectedGivenBrowsed) / browsed) + 0.01,
                "browse and survival rolls are correlated");
            Emit($"BROWSE_EQUIVALENCE light={F(light)} p={F(p)} indBrowsed={F(browsed / (float)n)} cohortIncrement={expectedIncrement:0.0000} indIncrement={increment:0.0000} "
                + $"indSurvival={alive / (double)n:0.0000} expectedSurvival={expectedSurvival:0.0000} survivalGivenBrowsed={survivalGivenBrowsed:0.0000}");
        }
        Debug.Log("BROWSE_SHARED_RESPONSE_EQUIVALENCE_PASS");
    }

    // ---------- 4. Protection lifetime / failure / reclosure experiments ----------

    private void VerifyExperiments(TreeSpeciesDefinition oak)
    {
        // Fence breach, strong light, high pressure. An early breach exposes
        // juveniles still below escape height; a late breach finds them
        // escaped, so they keep the full benefit of early protection.
        Outcome early = RunCondition(oak, 0.7f, 0.85f, "fence", null, MatrixBaseYear + 3, years: 20);
        Outcome late = RunCondition(oak, 0.7f, 0.85f, "fence", null, MatrixBaseYear + 8, years: 20);
        Outcome fenced = RunCondition(oak, 0.7f, 0.85f, "fence", null, years: 20);
        Outcome open = RunCondition(oak, 0.7f, 0.85f, "none", null, years: 20);
        Emit("BROWSE_EXPERIMENT fenceBreachYear3 " + early.Describe());
        Emit("BROWSE_EXPERIMENT fenceBreachYear8 " + late.Describe());
        Check(early.MeanBrowseEvents[1] > 0f && fenced.MeanBrowseEvents[2] == 0f, "early breach did not expose short juveniles");
        Check(early.Promoted[1] >= open.Promoted[1] && early.Promoted[1] <= fenced.Promoted[1], "early breach outcome not between open and intact");
        Check(late.Describe() == fenced.Describe(), "late breach affected juveniles that had already escaped browse height");

        // Shelter expiry: marginal light keeps oak short beyond the shelter life.
        Outcome shelterShort = RunCondition(oak, 0.35f, 0.85f, "shelter", null, shelterYears: 3, years: 20);
        Outcome shelterLong = RunCondition(oak, 0.35f, 0.85f, "shelter", null, shelterYears: 40, years: 20);
        Emit("BROWSE_EXPERIMENT shelter3yr " + shelterShort.Describe());
        Emit("BROWSE_EXPERIMENT shelter40yr " + shelterLong.Describe());
        Check(shelterShort.MeanBrowseEvents[2] > 0f && shelterLong.MeanBrowseEvents[2] == 0f, "shelter expiry did not restore exposure");
        Check(shelterLong.Promoted[2] >= shelterShort.Promoted[2], "longer shelter life worsened recruitment");

        // Canopy reclosure: good light for 3 years, then shade. Fenced juveniles still fail.
        Outcome reclosure = RunCondition(oak, 0.7f, 0.85f, "fence", y => y <= 3 ? 0.7f : 0.08f, years: 40);
        Emit("BROWSE_EXPERIMENT reclosureFenced " + reclosure.Describe());
        Check(reclosure.Promoted[3] == 0f && reclosure.CohortPromotionYear < 0, "protected juveniles recruited under reclosed canopy");
        Debug.Log("BROWSE_EXPERIMENTS_PASS");
    }

    // ---------- 5. Production annual step: determinism and save/load continuity ----------

    private IEnumerator VerifyProductionContinuity(TreeSpeciesDefinition oak, TreeSpeciesDefinition beech)
    {
        // Neutral control first: the same load/continue procedure with browsing off.
        IEnumerator neutral = Continuity(oak, beech, 0f);
        while (neutral.MoveNext()) yield return neutral.Current;
        IEnumerator browsed = Continuity(oak, beech, 0.7f);
        while (browsed.MoveNext()) yield return browsed.Current;
        Debug.Log("BROWSE_SAVELOAD_DETERMINISM_PASS");
    }

    private IEnumerator Continuity(TreeSpeciesDefinition oak, TreeSpeciesDefinition beech, float pressure)
    {
        string A = null, B = null, C = null;
        string endJson = null, loadedJson = null;
        for (int pass = 0; pass < 2; pass++)
        {
            Check(saves.LoadData(original, false), "restore world");
            // Load destroys trees absent from the save at frame end.
            yield return null;
            yield return null;
            SetupProductionBrowsing(oak, beech, pressure);
            for (int y = 0; y < 8; y++) { ecology.AdvanceOneYear(); Invoke(manager, "AdvancePlantedJuveniles"); }
            yield return null;
            ForestSaveData mid = saves.CaptureData();
            for (int y = 0; y < 7; y++) { ecology.AdvanceOneYear(); Invoke(manager, "AdvancePlantedJuveniles"); }
            ForestSaveData endData = saves.CaptureData();
            string end = ScenarioReferenceArchive.WorldHash(endData);
            if (pass == 0) A = end; else { B = end; endJson = JsonUtility.ToJson(endData, true); }
            if (pass == 1)
            {
                // Load the mid-run save (conditions are configuration, not saved) and continue.
                Check(saves.LoadData(mid, false), "load mid save");
                yield return null;
                yield return null;
                for (int y = 0; y < 7; y++) { ecology.AdvanceOneYear(); Invoke(manager, "AdvancePlantedJuveniles"); }
                ForestSaveData loadedData = saves.CaptureData();
                C = ScenarioReferenceArchive.WorldHash(loadedData);
                loadedJson = JsonUtility.ToJson(loadedData, true);
            }
            yield return null;
        }
        int browsedNow = manager.PlantedJuveniles.Count(j => j.lastYearBrowsed);
        Emit($"BROWSE_PRODUCTION pressure={F(pressure)} passA={A} passB={B} saveLoadContinuation={C} plantedBrowsedLastYear={browsedNow}");
        if (A != C)
        {
            string[] x = endJson.Split('\n'), y2 = loadedJson.Split('\n');
            int shown = 0;
            for (int i = 0; i < Math.Min(x.Length, y2.Length) && shown < 12; i++)
                if (x[i] != y2[i]) { Debug.Log($"BROWSE_CONTINUITY_DIFF line={i} uninterrupted={x[i].Trim()} loaded={y2[i].Trim()}"); shown++; }
            if (x.Length != y2.Length) Debug.Log($"BROWSE_CONTINUITY_DIFF lengths {x.Length} vs {y2.Length}");
        }
        Check(A == B, "production run is not deterministic at pressure " + pressure);
        Check(A == C, "save/load changed the continuation at pressure " + pressure);
        ecology.Browsing.BackgroundPressure = 0f;
        ecology.Browsing.ClearProtection();
    }

    private void SetupProductionBrowsing(TreeSpeciesDefinition oak, TreeSpeciesDefinition beech, float pressure)
    {
        ecology.Browsing.BackgroundPressure = pressure;
        ecology.Browsing.ClearProtection();
        ScenarioOneSaveData state = JsonUtility.FromJson<ScenarioOneSaveData>(JsonUtility.ToJson(original.scenarioOne));
        state.interactionSchemaVersion = 2;
        state.plantedJuveniles.Clear();
        // Planted oak/beech along the existing work clearing; half receive shelters.
        ForestEcologyCell brightest = ecology.Cells.OrderByDescending(c => c.Light).ThenBy(c => c.Center.x).First();
        for (int i = 0; i < 12; i++)
        {
            var position = new Vector3(brightest.Center.x + (i % 4 - 1.5f) * 1.1f, 0f, brightest.Center.y + (i / 4 - 1f) * 1.1f);
            TreeSpeciesDefinition s = i % 3 == 2 ? beech : oak;
            state.plantedJuveniles.Add(new PlantedJuvenileSaveData
            {
                juvenileId = "PJ" + (7000 + i).ToString("0000"), speciesId = s.SpeciesId, position = position,
                cellIndex = ecology.GetCellIndex(position), plantingYear = ecology.EcologicalYear, ageYears = 3,
                heightMeters = 0.6f, alive = true
            });
            if (i % 2 == 0 && pressure > 0f)
                ecology.Browsing.Shelters.Add(new BrowseShelter { shelterId = "PS" + i, position = new Vector2(position.x, position.z),
                    installedYear = ecology.EcologicalYear + 1, effectiveYears = 8 });
        }
        manager.RestoreSaveData(state);
    }

    // ---------- 6. Canonical lifecycle with browsing off and on ----------

    private IEnumerator VerifyLifecycle()
    {
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        Check(hashMethod != null, "lifecycle hash entry point missing");
        string Run(float pressure)
        {
            ForestStandScenarios.ApplyLifecycleFixture();
            ecology.RngModelVersion = SimulationRandom.LegacyModel;
            ecology.Browsing.BackgroundPressure = pressure;
            ecology.Browsing.ClearProtection();
            for (int year = 0; year < 80; year++) ecology.AdvanceOneYear();
            return (string)hashMethod.Invoke(null, new object[] { ecology });
        }
        string neutral = Run(0f);
        yield return null;
        string onA = Run(0.6f);
        yield return null;
        string onB = Run(0.6f);
        ecology.Browsing.BackgroundPressure = 0f;
        Emit($"BROWSE_LIFECYCLE neutral={neutral} browsing0.6A={onA} browsing0.6B={onB}");
        Check(neutral == "BFC55473C1506067", "neutral browsing changed the canonical lifecycle: " + neutral);
        Check(onA == onB, "browsing lifecycle not deterministic");
        Debug.Log("BROWSE_NEUTRAL_LIFECYCLE_PASS");
    }

    // ---------- 7. Performance ----------

    private IEnumerator MeasurePerformance(TreeSpeciesDefinition oak, TreeSpeciesDefinition sitka)
    {
        ForestStartingStand generator = FindFirstObjectByType<ForestStartingStand>();
        Check(generator != null, "stand generator missing");
        foreach (float size in new[] { 40f, 80f, 100f })
        {
            BuildStand(generator, size);
            yield return null;
            // Stress: an oak and a Sitka cohort in every cell.
            foreach (ForestEcologyCell cell in ecology.Cells)
            {
                cell.GetOrCreateCohort(oak).Restore(0.5f, 0.5f, ecology.EcologicalYear, RegenerationOrigin.Natural, ecology.EcologicalYear);
                cell.GetOrCreateCohort(sitka).Restore(0.5f, 0.4f, ecology.EcologicalYear, RegenerationOrigin.Natural, ecology.EcologicalYear);
            }
            double regenOff = TimeRegen(0f, false), regenOn = TimeRegen(0.6f, true);
            var watch = Stopwatch.StartNew();
            ecology.Browsing.BackgroundPressure = 0f; ecology.Browsing.ClearProtection();
            for (int i = 0; i < 3; i++) ecology.AdvanceOneYear();
            double yearOff = watch.Elapsed.TotalMilliseconds / 3;
            AddQuarterFence(size);
            ecology.Browsing.BackgroundPressure = 0.6f;
            watch.Restart();
            for (int i = 0; i < 3; i++) ecology.AdvanceOneYear();
            double yearOn = watch.Elapsed.TotalMilliseconds / 3;
            ecology.Browsing.BackgroundPressure = 0f; ecology.Browsing.ClearProtection();
            Debug.Log($"BROWSE_TIMING size={size} cells={ecology.CellCount} regenStepOffMs={regenOff:0.000} regenStepOnMs={regenOn:0.000} "
                + $"annualStepOffMs={yearOff:0.0} annualStepOnMs={yearOn:0.0}");
            yield return null;
        }
        Debug.Log("BROWSE_PERFORMANCE_MEASURED");
    }

    private double TimeRegen(float pressure, bool fence)
    {
        ecology.Browsing.BackgroundPressure = pressure;
        ecology.Browsing.ClearProtection();
        if (fence) AddQuarterFence(Mathf.Sqrt(ecology.CellCount) * 5f);
        var snapshot = ecology.Cells.Select(c => c.Regeneration.Select(r => (r, r.Density, r.Height)).ToList()).ToList();
        var watch = Stopwatch.StartNew();
        for (int i = 0; i < 50; i++)
        {
            Invoke(ecology, "GrowExistingRegeneration");
            foreach (var cell in snapshot) foreach (var item in cell) { item.r.Density = item.Density; item.r.Height = item.Height; }
        }
        return watch.Elapsed.TotalMilliseconds / 50;
    }

    private void AddQuarterFence(float size)
    {
        float h = size * 0.5f;
        ecology.Browsing.ProtectedAreas.Add(new BrowseProtectedArea
        {
            areaId = "perf-fence", installedYear = 0,
            polygon = new List<Vector2> { new Vector2(-h, -h), new Vector2(0, -h), new Vector2(0, 0), new Vector2(-h, 0) }
        });
    }

    private void BuildStand(ForestStartingStand generator, float size)
    {
        DestroyAllTrees();
        Field(typeof(ForestEcologyController), "standSizeMeters").SetValue(ecology, size);
        ecology.ResetForDeterministicRun();
        float spacing = (float)Field(typeof(ForestStartingStand), "latticeSpacingMeters").GetValue(generator);
        Field(typeof(ForestStartingStand), "standWidthMeters").SetValue(generator, size);
        Field(typeof(ForestStartingStand), "standDepthMeters").SetValue(generator, size);
        Field(typeof(ForestStartingStand), "latticePerAxis").SetValue(generator, Mathf.FloorToInt((size - 2f) / spacing) + 1);
        Field(typeof(ForestStartingStand), "targetTreeCount").SetValue(generator, Mathf.RoundToInt(2100f * size * size / 10000f));
        Field(typeof(ForestStartingStand), "pathCorridorRadiusMeters").SetValue(generator, 0f);
        Field(typeof(ForestStartingStand), "clearingRadiusMeters").SetValue(generator, 0f);
        generator.Generate();
    }

    // ---------- 8. Sensitivity (diagnostic; mirrors the shared rule's arithmetic) ----------

    private void Sensitivity(TreeSpeciesDefinition oak, TreeSpeciesDefinition beech, TreeSpeciesDefinition sitka)
    {
        // Expected-value juvenile from 0.6 m in strong light (0.7), 40 years.
        // Reports years to promotion height and survival under alternative
        // increment-loss / mortality / palatability values. Not production.
        foreach (TreeSpeciesDefinition s in new[] { oak, beech, sitka })
        foreach (float loss in new[] { 0.6f, 0.8f, 0.9f, 1f })
        foreach (float mortality in new[] { 0.02f, 0.04f, 0.08f })
        {
            var line = new StringBuilder($"BROWSE_SENSITIVITY {s.SpeciesId} loss={F(loss)} mortality={F(mortality)}");
            foreach (float pressure in new[] { 0.2f, 0.5f, 0.85f })
            {
                float h = 0.6f; double survival = 1; int years = -1;
                for (int y = 1; y <= 40 && years < 0; y++)
                {
                    float p = Mathf.Min(JuvenileEcologyRules.BrowseProbabilityCap,
                        pressure * s.BrowsePalatability * JuvenileEcologyRules.BrowseHeightVulnerability(s, h));
                    h += s.RegenHeightGrowthMPerYear * s.JuvenileLightResponse(0.7f) * (1f - p * loss);
                    survival *= JuvenileEcologyRules.SurvivalResponse(s, 0.7f) * (1 - p * mortality);
                    if (h >= s.PromotionHeightM) years = y;
                }
                line.Append($" p{F(pressure)}:years={years},surv={survival:0.000}");
            }
            Emit(line.ToString());
        }
    }

    // ---------- helpers ----------

    private void SetYear(int year) => Field(typeof(ForestEcologyController), "ecologicalYear").SetValue(ecology, year);

    private static FieldInfo Field(Type type, string name) =>
        type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new MissingFieldException(type.Name, name);

    private static void Invoke(object target, string name)
    {
        MethodInfo method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Check(method != null, "Missing verification entry point " + name);
        method.Invoke(target, null);
    }

    private static void DestroyHarnessTrees()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t.TreeId != null && t.TreeId.StartsWith("PL-PJ8", StringComparison.Ordinal))
                DestroyImmediate(t.gameObject);
    }

    private static void DestroyAllTrees()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
    }

    private static string Fnv64(string text, bool excludeTiming = false)
    {
        ulong hash = 14695981039346656037UL;
        foreach (string line in text.Split('\n'))
        {
            if (excludeTiming && line.StartsWith("BROWSE_TIMING", StringComparison.Ordinal)) continue;
            foreach (char c in line) { hash ^= c; hash *= 1099511628211UL; }
            hash ^= '\n'; hash *= 1099511628211UL;
        }
        return hash.ToString("X16");
    }
}
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable in-Editor gate for the Scenario Two species-model foundation (SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-V1).
//
// What it adds over the offline check (Tools/Verification/ScenarioTwoSpeciesFoundation/run_species_foundation_check.py): it uses
// the REAL ScriptableObject, the REAL ForestTreeSpawner registration, the REAL ForestEcologyController annual step and the REAL
// seed-rain code. It answers one question: can pine, birch and alder now be represented without silently inheriting Sitka
// behaviour, with age-phased growth and per-species dispersal, while Sitka / Oak / Beech are unchanged?
//
//   U1  the three new assets LOAD as declared and are not simulation-ready (HOLD), the established three are ready and unchanged
//   U2  a minimally defined new species is not ready, not Reference-safe, not browse-proof-safe, does not disperse
//   U3  the spawner registers only simulation-ready species (unready ones never reach KnownSpecies or ResolveSpecies)
//   U4  age phases change the adult increment across the phase boundary in the real GrowAdults; no phases = unchanged law
//   U5  seed rain: planting-only produces none; a wind species follows exp(-d/scale) with ITS OWN cutoff; wind+water uses the
//       wind kernel only and is not limited to 30 m
//   U6  authored-only external colonisation: with no parent of a species in the stand, that species never appears
//   U7  browse: a declared non-zero palatability gives a non-zero browse probability
//
// Fixture species use MADE-UP numbers to exercise the rules. They are NOT species values and are never saved.
//
// STATUS: run in Unity 6000.6.0f1; see the task handoff for the exact candidate and evidence.
// It does not replace the existing Scenario One gates; run those for the "unchanged" requirement (see
// Docs/ScenarioTwoSpeciesFoundation.md, Verification).
//
// Copy into Assets/ForestPrototype, run with
//   CCF_STAND_GEOMETRY=1 <UNITY> -batchmode -nographics -executeMethod ScenarioTwoSpeciesFoundationVerification.Begin
// Enlarged80 is required by the seed-distance fixture (Legacy40 cannot reach beyond 30 m).
// then remove the copy and its .meta. Saves are captured/restored in memory only; the scene is never saved.
public static class ScenarioTwoSpeciesFoundationVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "ScenarioTwoSpeciesFoundationVerification.Begin"))
            new GameObject("Scenario Two species foundation verification").AddComponent<ScenarioTwoSpeciesFoundationVerificationRunner>();
    }
}

public sealed class ScenarioTwoSpeciesFoundationVerificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string AssetFolder = "Assets/ForestPrototype/Species/ScenarioTwo/";

    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestSaveData original;
    private ForestSpeciesVisualSet[] originalSets;
    private TreeSpeciesDefinition sitka, oak, beech;
    private readonly List<string> failures = new List<string>();
    private readonly List<TreeSpeciesDefinition> fixtures = new List<TreeSpeciesDefinition>();
    private int passed;
    private HashSet<string> keep = new HashSet<string>();
    private float lastDd, lastDh;

    private static string F(double v, string f = "0.###") => v.ToString(f, Inv);

    private void Pass(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"S2_SPECIES_FOUNDATION_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }

    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    // ---------------------------------------------------------------- fixtures

    private static void Field(object target, string name, object value)
    {
        FieldInfo field = typeof(TreeSpeciesDefinition).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null) throw new InvalidOperationException("TreeSpeciesDefinition has no field " + name);
        field.SetValue(target, value);
    }

    // A fully declared fixture: every area Set/Placeholder and no Sitka default left in the guarded fields. Made-up numbers.
    private TreeSpeciesDefinition Fixture(string id, NaturalSeedDispersal dispersal = NaturalSeedDispersal.Wind,
        float scale = 19f, float cutoff = 150f)
    {
        TreeSpeciesDefinition s = ScriptableObject.CreateInstance<TreeSpeciesDefinition>();
        fixtures.Add(s);
        Field(s, "speciesId", id); Field(s, "displayName", id);
        Field(s, "growingSpaceDemand", GrowingSpaceDemandClass.Intermediate);
        Field(s, "growthState", SpeciesParameterState.Placeholder);
        Field(s, "potentialDbhGrowthCmPerYear", 0.5f); Field(s, "potentialHeightGrowthMPerYear", 0.5f);
        Field(s, "maxDbhCm", 80f); Field(s, "maxHeightM", 30f);
        Field(s, "crownRadiusIntercept", 0.5f); Field(s, "crownRadiusPerCmDbh", 0.1f);
        Field(s, "browseState", SpeciesParameterState.Set); Field(s, "browsePalatability", 0.5f);
        Field(s, "juvenileLightState", SpeciesParameterState.Placeholder); Field(s, "useDistinctJuvenileLightResponses", true);
        Field(s, "naturalSeedDispersal", dispersal); Field(s, "seedKernelState", SpeciesParameterState.Placeholder);
        Field(s, "seedDispersalScaleM", scale); Field(s, "seedDispersalCutoffM", cutoff);
        Field(s, "maturityOnsetYears", 15f); Field(s, "maturityFullYears", 15f);
        Field(s, "mastGoodProbability", 0f); Field(s, "mastPoorProbability", 0f);
        Field(s, "mastGoodMultiplier", 1f); Field(s, "mastNormalMultiplier", 1f); Field(s, "mastPoorMultiplier", 1f);
        return s;
    }

    private void RegisterInSpawner(params TreeSpeciesDefinition[] species)
    {
        FieldInfo field = typeof(ForestTreeSpawner).GetField("speciesVisualSets", BindingFlags.NonPublic | BindingFlags.Instance);
        var sets = new List<ForestSpeciesVisualSet>(originalSets ?? new ForestSpeciesVisualSet[0]);
        foreach (TreeSpeciesDefinition s in species)
            sets.Add(new ForestSpeciesVisualSet { species = s });
        field.SetValue(spawner, sets.ToArray());
    }

    private void RestoreSpawner()
    {
        typeof(ForestTreeSpawner).GetField("speciesVisualSets", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(spawner, originalSets);
    }

    // ---------------------------------------------------------------- stand helpers

    private void ResetStand(int growth)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growth;
        keep = new HashSet<string>();
    }

    private void Spawn(string id, TreeSpeciesDefinition s, Vector3 position, int age, float dbh, float height, float crown)
    {
        spawner.Spawn(id, s, position, age, dbh, height, crown);
        keep.Add(id);
    }

    private void Finish()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (!keep.Contains(t.TreeId)) DestroyImmediate(t.gameObject);
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    // One isolated 3 x 3 grid (spacing above the Hegyi cutoff, so competition is exactly 0); one annual step; the centre tree's increment.
    private IEnumerator OneYearIncrement(TreeSpeciesDefinition s, int age)
    {
        ResetStand(2);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                Spawn("G" + (i * 3 + j).ToString("00"), s, new Vector3((i - 1) * 12f, 0f, (j - 1) * 12f), age, 10f, 8f, s.PotentialCrownRadiusM(10f));
        Finish();
        yield return null;
        ForestTree probe = Living().First(t => t.TreeId == "G04");
        float d0 = probe.Diameter, h0 = probe.Height;
        e.AdvanceOneYear();
        probe = Living().First(t => t.TreeId == "G04");
        lastDd = probe.Diameter - d0;
        lastDh = probe.Height - h0;
    }

    private static float Distance(ForestEcologyCell cell, Vector3 tree) => Vector2.Distance(cell.Center, new Vector2(tree.x, tree.z));

    // ---------------------------------------------------------------- the checks

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        FieldInfo setsField = typeof(ForestTreeSpawner).GetField("speciesVisualSets", BindingFlags.NonPublic | BindingFlags.Instance);
        originalSets = (ForestSpeciesVisualSet[])setsField.GetValue(spawner);
        sitka = spawner.ResolveSpecies("sitka-spruce");
        oak = spawner.ResolveSpecies("sessile-oak");
        beech = spawner.ResolveSpecies("beech");
        Pass("SPECIES_RESOLVE", sitka != null && oak != null && beech != null, "Scenario One species resolve through the spawner");
        original = saves.CaptureData();

        // ---- U1: assets
#if UNITY_EDITOR
        TreeSpeciesDefinition pine = AssetDatabase.LoadAssetAtPath<TreeSpeciesDefinition>(AssetFolder + "ScotsPine.asset");
        TreeSpeciesDefinition birch = AssetDatabase.LoadAssetAtPath<TreeSpeciesDefinition>(AssetFolder + "DownyBirch.asset");
        TreeSpeciesDefinition alder = AssetDatabase.LoadAssetAtPath<TreeSpeciesDefinition>(AssetFolder + "CommonAlder.asset");
#else
        TreeSpeciesDefinition pine = null, birch = null, alder = null;
#endif
        Pass("U1_NEW_ASSETS_LOAD", pine != null && birch != null && alder != null, "ScriptableObjects import and deserialize");
        if (pine != null && birch != null && alder != null)
        {
            Pass("U1_IDS_AND_DENSITY_CLASS",
                pine.SpeciesId == "scots-pine" && birch.SpeciesId == "downy-birch" && alder.SpeciesId == "common-alder"
                && pine.GrowingSpaceDemand == GrowingSpaceDemandClass.Intermediate && birch.GrowingSpaceDemand == GrowingSpaceDemandClass.Intermediate
                && alder.GrowingSpaceDemand == GrowingSpaceDemandClass.Intermediate, "Intermediate, never Reference");
            Pass("U1_NOT_SIMULATION_READY_BECAUSE_OF_HOLD",
                !pine.IsSimulationReady && !birch.IsSimulationReady && !alder.IsSimulationReady
                && !pine.ConfigurationReport.HasErrors && !birch.ConfigurationReport.HasErrors && !alder.ConfigurationReport.HasErrors,
                $"pine=[{string.Join(",", pine.ConfigurationReport.HoldAreas)}] birch=[{string.Join(",", birch.ConfigurationReport.HoldAreas)}] alder=[{string.Join(",", alder.ConfigurationReport.HoldAreas)}]");
            Pass("U1_PINE_IS_PLANTING_ONLY", pine.SeedDispersal == NaturalSeedDispersal.None && pine.SupportsRegeneration && !pine.DispersesSeedNaturally, "no natural dispersal");
            Pass("U1_BIRCH_USES_THE_CLOSED_STAND_SCALE",
                birch.SeedDispersal == NaturalSeedDispersal.Wind && birch.DispersesSeedNaturally && Mathf.Approximately(birch.SeedDispersalScaleM, 19f)
                && birch.MaturityOnsetYears == 15f && birch.BrowsePalatability == 0.5f, $"scale={birch.SeedDispersalScaleM} cutoff={birch.SeedDispersalCutoffM}");
            Pass("U1_ALDER_DECLARES_WIND_AND_WATER_WITH_THE_KERNEL_ON_HOLD",
                alder.HasWaterDispersalCapability && alder.SeedDispersal == NaturalSeedDispersal.WindAndWater && !alder.DispersesSeedNaturally
                && alder.SeedKernelState == SpeciesParameterState.Hold && alder.BrowsePalatability == 0.2f
                && alder.SeedDispersalCutoffM != 30f && !SpeciesConfigurationPolicy.WaterDispersalPropagationImplemented, "no 30 m cutoff, water propagation not implemented");
            Pass("U1_NO_PHASE_TABLE_IS_SUPPLIED", !pine.HasGrowthAgePhases && !birch.HasGrowthAgePhases && !alder.HasGrowthAgePhases, "growth values are HOLD");
        }
        Pass("U1_ESTABLISHED_SPECIES_ARE_READY_AND_UNCHANGED",
            sitka.IsSimulationReady && oak.IsSimulationReady && beech.IsSimulationReady
            && sitka.IsEstablishedScenarioOneSpecies && oak.IsEstablishedScenarioOneSpecies && beech.IsEstablishedScenarioOneSpecies
            && sitka.GrowingSpaceDemand == GrowingSpaceDemandClass.Reference && oak.GrowingSpaceDemand == GrowingSpaceDemandClass.High
            && beech.GrowingSpaceDemand == GrowingSpaceDemandClass.Intermediate
            && sitka.DispersesSeedNaturally && oak.DispersesSeedNaturally && beech.DispersesSeedNaturally == beech.SupportsRegeneration
            && !sitka.HasGrowthAgePhases && !oak.HasGrowthAgePhases && !beech.HasGrowthAgePhases
            && sitka.BrowsePalatability == 0.15f && oak.BrowsePalatability == 1f && beech.BrowsePalatability == 0.5f,
            $"sitka={sitka.GrowingSpaceDemand} oak={oak.GrowingSpaceDemand} beech={beech.GrowingSpaceDemand}");

        // ---- U2: a minimally defined species
        TreeSpeciesDefinition minimal = ScriptableObject.CreateInstance<TreeSpeciesDefinition>();
        fixtures.Add(minimal);
        Field(minimal, "speciesId", "s2-minimal");
        SpeciesConfigurationReport minimalReport = minimal.ConfigurationReport;
        Pass("U2_MINIMAL_SPECIES_IS_NOT_READY_AND_HAS_ERRORS", !minimal.IsSimulationReady && minimalReport.HasErrors, $"{minimalReport.Errors.Count} errors");
        Pass("U2_MINIMAL_SPECIES_DEFAULTS_ARE_REJECTED_NOT_USED",
            minimal.GrowingSpaceDemand == GrowingSpaceDemandClass.Reference && minimalReport.Errors.Any(x => x.Contains("reserved for sitka-spruce"))
            && minimal.BrowsePalatability == 0f && minimalReport.Errors.Any(x => x.Contains("browse-proof"))
            && minimalReport.Errors.Any(x => x.Contains("growthState")), "Reference, browse 0 and Sitka-like growth are all refused");
        Pass("U2_MINIMAL_SPECIES_DOES_NOT_DISPERSE", !minimal.DispersesSeedNaturally && minimal.SeedDispersal == NaturalSeedDispersal.Unspecified, "no generic dispersal rule applies");

        // ---- U3: registration
        TreeSpeciesDefinition ready = Fixture("s2-ready");
        Pass("U3_FULLY_DECLARED_FIXTURE_IS_READY", ready.IsSimulationReady, string.Join(" | ", ready.ConfigurationReport.Errors.Concat(ready.ConfigurationReport.HoldAreas)));
        string[] beforeIds = spawner.KnownSpecies.Select(s => s.SpeciesId).ToArray();
        var unready = new List<TreeSpeciesDefinition> { minimal };
        if (pine != null) { unready.Add(pine); unready.Add(birch); unready.Add(alder); }
        RegisterInSpawner(unready.ToArray());
        string[] unreadyIds = spawner.KnownSpecies.Select(s => s.SpeciesId).ToArray();
        Pass("U3_UNREADY_SPECIES_ARE_NOT_REGISTERED",
            unreadyIds.SequenceEqual(beforeIds) && spawner.ResolveSpecies("s2-minimal") == null && spawner.ResolveSpecies("scots-pine") == null
            && spawner.ResolveSpecies("downy-birch") == null && spawner.ResolveSpecies("common-alder") == null,
            $"known before=[{string.Join(",", beforeIds)}] after adding unready=[{string.Join(",", unreadyIds)}]");
        RegisterInSpawner(ready);
        Pass("U3_READY_SPECIES_IS_REGISTERED", spawner.KnownSpecies.Any(s => s.SpeciesId == "s2-ready") && spawner.ResolveSpecies("s2-ready") == ready, "registered and resolvable");
        RestoreSpawner();
        FieldInfo defaultField = typeof(ForestTreeSpawner).GetField("defaultSpecies", BindingFlags.NonPublic | BindingFlags.Instance);
        TreeSpeciesDefinition originalDefault = (TreeSpeciesDefinition)defaultField.GetValue(spawner);
        try
        {
            defaultField.SetValue(spawner, minimal);
            Pass("U3_UNREADY_DEFAULT_CANNOT_BYPASS_RESOLUTION_WITH_AN_EMPTY_ID",
                spawner.ResolveSpecies(null) == null && spawner.ResolveSpecies("") == null
                && spawner.ResolveSpecies(minimal.SpeciesId) == null
                && !spawner.KnownSpecies.Contains(minimal), "null, empty and explicit IDs all reject the unready default");
            defaultField.SetValue(spawner, ready);
            Pass("U3_READY_DEFAULT_RESOLVES_WITH_AN_EMPTY_ID",
                spawner.ResolveSpecies(null) == ready && spawner.ResolveSpecies("") == ready,
                "the default fallback remains available for a ready species");
        }
        finally
        {
            defaultField.SetValue(spawner, originalDefault);
        }

        // ---- U4: age phases in the real adult growth
        TreeSpeciesDefinition phased = Fixture("s2-phased");
        Field(phased, "growthAgePhases", new[] { new GrowthAgePhase(0f, 1f, 1f), new GrowthAgePhase(10f, 0.5f, 0.25f) });
        TreeSpeciesDefinition control = Fixture("s2-control");
        RegisterInSpawner(phased, control);
        yield return OneYearIncrement(phased, 9);
        float dd9 = lastDd, dh9 = lastDh;
        yield return OneYearIncrement(phased, 10);
        float dd10 = lastDd, dh10 = lastDh;
        yield return OneYearIncrement(phased, 40);
        float dd40 = lastDd, dh40 = lastDh;
        yield return OneYearIncrement(control, 9);
        float cd9 = lastDd, ch9 = lastDh;
        yield return OneYearIncrement(control, 40);
        float cd40 = lastDd, ch40 = lastDh;
        Pass("U4_PHASES_CHANGE_THE_INCREMENT_ACROSS_THE_BOUNDARY",
            dd9 > 0f && Mathf.Abs(dd9 / dd10 - 2f) < 1e-3f && Mathf.Abs(dh9 / dh10 - 4f) < 1e-3f && Mathf.Abs(dd10 - dd40) < 1e-6f && Mathf.Abs(dh10 - dh40) < 1e-6f,
            $"age9 dD={F(dd9, "0.0000")} dH={F(dh9, "0.0000")}  age10 dD={F(dd10, "0.0000")} dH={F(dh10, "0.0000")}  age40 dD={F(dd40, "0.0000")} dH={F(dh40, "0.0000")}");
        Pass("U4_NO_PHASES_IS_THE_UNCHANGED_FIXED_INCREMENT_LAW",
            cd9 > 0f && BitConverter.SingleToInt32Bits(cd9) == BitConverter.SingleToInt32Bits(cd40) && BitConverter.SingleToInt32Bits(ch9) == BitConverter.SingleToInt32Bits(ch40)
            && Mathf.Abs(cd9 - dd9) < 1e-6f && Mathf.Abs(ch9 - dh9) < 1e-6f,
            $"control age9 dD={F(cd9, "0.0000")} age40 dD={F(cd40, "0.0000")}; phase 0 factors are 1, so the phased species matches the control before age 10");
        RestoreSpawner();

        // ---- U5: seed rain
        TreeSpeciesDefinition plantOnly = Fixture("s2-plant-only", NaturalSeedDispersal.None);
        TreeSpeciesDefinition wind30 = Fixture("s2-wind-30", NaturalSeedDispersal.Wind, 19f, 30f);
        TreeSpeciesDefinition windWater = Fixture("s2-wind-water", NaturalSeedDispersal.WindAndWater, 15f, 100f);
        RegisterInSpawner(plantOnly, wind30, windWater);
        var rains = new Dictionary<string, float[]>();
        float[] dist = null; float farthest = 0f;
        foreach (TreeSpeciesDefinition s in new[] { plantOnly, wind30, windWater, sitka })
        {
            ResetStand(2);
            Vector3 at = Vector3.zero;
            Spawn("PARENT", s, at, 60, 30f, 20f, 4f);
            Finish();
            yield return null;
            e.AdvanceOneYear();
            ForestEcologyCell[] cells = e.Cells;
            rains[s.SpeciesId] = cells.Select(c => c.SeedRainFor(s.SpeciesId)).ToArray();
            if (dist == null)
            {
                dist = cells.Select(c => Distance(c, at)).ToArray();
                farthest = dist.Max();
            }
        }
        Pass("U5_STAND_IS_LARGE_ENOUGH_FOR_THE_DISTANCE_TESTS", farthest > 40f, $"farthest cell {F(farthest, "0.0")} m from the parent");
        Pass("U5_PLANTING_ONLY_PRODUCES_NO_SEED_RAIN", rains["s2-plant-only"].All(r => r == 0f), $"sum={F(rains["s2-plant-only"].Sum())}");
        Pass("U5_SITKA_STILL_SEEDS", rains["sitka-spruce"].Any(r => r > 0f), $"sum={F(rains["sitka-spruce"].Sum())}");
        int near = Array.IndexOf(dist, dist.Min()), far = Array.IndexOf(dist, dist.Max());
        float[] w30 = rains["s2-wind-30"], ww = rains["s2-wind-water"];
        Pass("U5_WIND_SPECIES_HONOURS_ITS_OWN_CUTOFF_NOT_A_GLOBAL_ONE", w30[near] > 0f && w30[far] == 0f && dist[far] > 30f, $"near={F(w30[near], "0.0000")} far={F(w30[far], "0.0000")} at {F(dist[far], "0.0")} m (cutoff 30 configured on this fixture only)");
        float expectedRatio = Mathf.Exp(-(dist[far] - dist[near]) / 15f);
        Pass("U5_WIND_AND_WATER_USES_THE_WIND_KERNEL_AND_REACHES_BEYOND_30_M",
            ww[far] > 0f && dist[far] > 30f && Mathf.Abs(ww[far] / ww[near] - expectedRatio) < 1e-3f * Mathf.Max(1f, expectedRatio),
            $"ratio far/near={F(ww[far] / ww[near], "0.00000")} expected exp(-(dFar-dNear)/15)={F(expectedRatio, "0.00000")}; water propagation implemented={SpeciesConfigurationPolicy.WaterDispersalPropagationImplemented}");

        // ---- U6: authored-only external colonisation
        TreeSpeciesDefinition absent = Fixture("s2-absent-parent", NaturalSeedDispersal.Wind, 19f, 150f);
        RegisterInSpawner(absent);
        ResetStand(2);
        Spawn("SITKA-PARENT", sitka, Vector3.zero, 60, 40f, 25f, 4f);
        Finish();
        yield return null;
        bool noAppearance = true; string appearanceDetail = "10 years, no parent of the species in the stand";
        for (int year = 0; year < 10; year++)
        {
            e.AdvanceOneYear();
            foreach (ForestEcologyCell c in e.Cells)
            {
                ForestRegenerationCohort cohort = c.FindCohort("s2-absent-parent");
                if (c.SeedRainFor("s2-absent-parent") > 0f || (cohort != null && cohort.Density > 0f)) noAppearance = false;
            }
            if (Living().Any(t => t.Species != null && t.Species.SpeciesId == "s2-absent-parent")) noAppearance = false;
            foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (!keep.Contains(t.TreeId) && (t.Species == null || t.Species.SpeciesId != "s2-absent-parent")) DestroyImmediate(t.gameObject);
        }
        Pass("U6_NO_SPONTANEOUS_EXTERNAL_COLONISATION", noAppearance, appearanceDetail);
        RestoreSpawner();

        // ---- U7: browse
        BrowseAssessment assessed = JuvenileEcologyRules.AssessBrowse(ready, 0.5f, 1f, 1f, BrowseProtectionState.None);
        Pass("U7_DECLARED_PALATABILITY_GIVES_A_NON_ZERO_BROWSE_PROBABILITY", ready.BrowsePalatability > 0f && assessed.Probability > 0f && Mathf.Abs(assessed.Probability - 0.5f) < 1e-5f, $"probability={F(assessed.Probability)}");
        if (birch != null && alder != null)
            Pass("U7_BIRCH_AND_ALDER_ASSET_PALATABILITY_IS_NON_ZERO", birch.BrowsePalatability > 0f && alder.BrowsePalatability > 0f && alder.BrowsePalatability < birch.BrowsePalatability,
                $"birch={birch.BrowsePalatability} alder={alder.BrowsePalatability} (alder below birch)");
    }

    private IEnumerator Start()
    {
        yield return null; yield return null;
        string failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Execute());
        while (stack.Count > 0)
        {
            bool more = false; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception ex) { failure = ex.ToString(); }
            if (failure != null) break;
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        try
        {
            RestoreSpawner();
            foreach (TreeSpeciesDefinition s in fixtures) if (s != null) DestroyImmediate(s);
            if (original != null) { manager.EndReferencePreview(); saves.LoadData(original, false); }
        }
        catch (Exception ex) { failure = failure ?? ex.ToString(); }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"S2_SPECIES_FOUNDATION_VERIFY_PASS checks={passed}" : $"S2_SPECIES_FOUNDATION_VERIFY_FAIL {failure} checks=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}

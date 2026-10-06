using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable verification for regeneration model 1 (age bands) and its model
// policy. Copy into Assets/ForestPrototype, run with
// -executeMethod RegenerationModelVerification.Begin, then remove the copy and
// its .meta. It may load/capture saves in memory but never writes the save slot.
//
// Juvenile abundance is relative abundance (occupancy); nothing here converts
// it to stems. Fixtures drive the production stage methods with controlled light.
public static class RegenerationModelVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "RegenerationModelVerification.Begin"))
            new GameObject("Regeneration model verification").AddComponent<RegenerationModelVerificationRunner>();
    }
}

public sealed class RegenerationModelVerificationRunner : MonoBehaviour
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string Neutral0 = "BFC55473C1506067", Normal0 = "3485B6630C9EA448";

    private ForestEcologyController e;
    private ForestStartingStand stand;
    private ForestTreeSpawner spawner;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka, beech, oak;
    private string dir;
    private int years;
    private readonly List<string> failures = new List<string>();
    private int passed;

    private static string F(float f) => f.ToString("R", Inv);
    private void Check(bool ok, string what)
    {
        if (ok) return;
        failures.Add(what);
        Debug.LogError("REGEN_MODEL_CHECK_FAIL " + what);
    }
    private void Pass(string invariant, bool ok, string detail)
    {
        Check(ok, invariant + " " + detail);
        if (ok) passed++;
        Debug.Log($"REGEN_MODEL_{(ok ? "PASS" : "FAIL")} {invariant} {detail}");
    }
    private void Call(string name, params object[] args) => typeof(ForestEcologyController).GetMethod(name, Private).Invoke(e, args);
    private void SetField(string name, object value) => typeof(ForestEcologyController).GetField(name, Private).SetValue(e, value);
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    // ---------- controlled fixture world ----------

    private void DestroyAllTrees()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
    }

    private void EmptyWorld(int model, float light, float browsePressure = 0f)
    {
        DestroyAllTrees();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = model;
        e.Browsing.BackgroundPressure = browsePressure;
        e.Browsing.ClearProtection();
        e.RecomputeCanopy();
        e.RecomputeSeedRain(); // registers regeneration species; no trees, so no seed
        foreach (ForestEcologyCell c in e.Cells) { c.ClearRegeneration(); c.Light = light; }
    }

    private void NextYear() => SetField("ecologicalYear", e.EcologicalYear + 1);
    private void Grow() => Call("GrowExistingRegeneration");
    private void Establish() => Call("EstablishNewCohorts");
    private void Promote() => Call("PromoteCohorts", e.ResolveSpecies(),
        SimulationRandom.Create(e.RngModelVersion, e.SimulationSeed, e.EcologicalYear, 0));

    private ForestRegenerationCohort Band(int cell, TreeSpeciesDefinition s, RegenerationOrigin origin, int year, float density, float height)
    {
        var band = new ForestRegenerationCohort(s);
        band.Restore(density, height, year, origin, origin == RegenerationOrigin.Planted ? Mathf.Max(0, year) : year);
        e.Cells[cell].InsertBand(band);
        return band;
    }

    private static string Describe(IEnumerable<ForestRegenerationCohort> bands) => string.Join(";", bands.Select(b =>
        $"{b.SpeciesId}/{b.Origin}/y{b.EstablishYear}/d{F(b.Density)}/h{F(b.Height)}"));

    private RegenerationSpeciesAccount Account(TreeSpeciesDefinition s) => e.LastRegenerationAccount.For(s.SpeciesId);

    // ---------- invariants ----------

    private void Invariants()
    {
        // NO_SEED_NO_NEW_ABUNDANCE: existing stock survives/grows in height but
        // its abundance never rises without seed, at any light.
        foreach (float light in new[] { 0.9f, 0.3f, 0.05f })
        {
            EmptyWorld(RegenerationModel.AgeBands, light);
            ForestRegenerationCohort b = Band(0, sitka, RegenerationOrigin.Natural, 0, 0.2f, sitka.RegenInitialHeightM);
            bool rose = false; float previous = b.Density;
            for (int y = 0; y < 10; y++)
            {
                NextYear(); Grow(); Establish();
                float now = e.Cells[0].SpeciesDensity("sitka-spruce");
                if (now > previous) rose = true;
                previous = now;
            }
            int bands = e.Cells.Sum(c => c.Regeneration.Count);
            Pass("NO_SEED_NO_NEW_ABUNDANCE", !rose && bands <= 1, $"light={F(light)} start=0.2 after10={F(previous)} bands={bands}");
        }
        // Model 0 retains its legacy infill (compatibility, not a pass condition of model 1).
        {
            EmptyWorld(RegenerationModel.Legacy, 0.9f);
            e.Cells[0].GetOrCreateCohort(sitka).Restore(0.2f, sitka.RegenInitialHeightM, 0);
            for (int y = 0; y < 10; y++) { NextYear(); Grow(); }
            Debug.Log($"REGEN_MODEL_INFO MODEL0_LEGACY_INFILL_RETAINED start=0.2 after10={F(e.Cells[0].SpeciesDensity("sitka-spruce"))}");
        }

        // NEW_RECRUIT_STARTS_AT_AGE_ZERO / NEW_RECRUIT_GETS_INITIAL_HEIGHT.
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            for (int y = 0; y < 9; y++) NextYear();
            ForestRegenerationCohort old = Band(0, sitka, RegenerationOrigin.Natural, 0, 0.1f, 1.2f);
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            Establish();
            ForestRegenerationCohort recruit = e.Cells[0].FindBand("sitka-spruce", RegenerationOrigin.Natural, e.EcologicalYear);
            Pass("NEW_RECRUIT_STARTS_AT_AGE_ZERO", recruit != null && recruit.EstablishYear == e.EcologicalYear && recruit.OriginYear == e.EcologicalYear
                && old.EstablishYear == 0 && old.Density == 0.1f, $"year={e.EcologicalYear} bands=[{Describe(e.Cells[0].Regeneration)}]");
            Pass("NEW_RECRUIT_GETS_INITIAL_HEIGHT", recruit != null && recruit.Height == sitka.RegenInitialHeightM && old.Height == 1.2f,
                $"recruitHeight={(recruit != null ? F(recruit.Height) : "none")} initial={F(sitka.RegenInitialHeightM)} olderBandHeight={F(old.Height)}");
        }

        // NATURAL_RECRUIT_REMAINS_NATURAL: after a planted band promotes, later
        // natural recruitment is Natural with its own age and height.
        {
            EmptyWorld(RegenerationModel.AgeBands, 1f);
            NextYear();
            Band(0, sitka, RegenerationOrigin.Planted, -3, 0.2f, sitka.PromotionHeightM + 0.5f);
            int treesBefore = Living().Length;
            Promote();
            bool promoted = Living().Length == treesBefore + 1 && e.Cells[0].Regeneration.Count == 0;
            NextYear();
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            Establish();
            ForestRegenerationCohort recruit = e.Cells[0].Regeneration.FirstOrDefault();
            Pass("NATURAL_RECRUIT_REMAINS_NATURAL", promoted && recruit != null && recruit.Origin == RegenerationOrigin.Natural
                && recruit.EstablishYear == e.EcologicalYear && recruit.Height == sitka.RegenInitialHeightM,
                $"plantedPromoted={promoted} after=[{Describe(e.Cells[0].Regeneration)}]");
        }

        // PLANTED_RECRUIT_REMAINS_PLANTED: planting creates a Planted band; later
        // natural recruitment forms a separate Natural band; nothing relabels.
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            NextYear();
            PlantingResult planting = e.TryPlantJuvenile(beech, new Vector3(e.Cells[0].Center.x, 0f, e.Cells[0].Center.y));
            ForestRegenerationCohort plantedBand = e.Cells[0].Regeneration.FirstOrDefault();
            string before = Describe(e.Cells[0].Regeneration);
            for (int y = 0; y < 6; y++)
            {
                NextYear(); Grow();
                e.Cells[0].SetSeedRain("beech", 100f);
                Establish();
            }
            var planted = e.Cells[0].Bands("beech", RegenerationOrigin.Planted);
            var natural = e.Cells[0].Bands("beech", RegenerationOrigin.Natural);
            Pass("PLANTED_RECRUIT_REMAINS_PLANTED", planting.Success && planted.Count == 1 && planted[0] == plantedBand
                && planted[0].EstablishYear == 1 - e.PlantedJuvenileAgeYears && natural.All(n => n.EstablishYear > 1),
                $"planted={before} after=[{Describe(e.Cells[0].Regeneration)}]");
        }

        // CAPACITY_REJECTION_DOES_NOT_REMOVE_EXISTING_STOCK (over-capacity cell).
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            NextYear();
            ForestRegenerationCohort a = Band(0, sitka, RegenerationOrigin.Natural, 0, sitka.RegenDensityMax * 0.7f, 0.3f);
            ForestRegenerationCohort b = Band(0, beech, RegenerationOrigin.Natural, 0, beech.RegenDensityMax * 0.7f, 0.3f);
            float aBefore = a.Density, bBefore = b.Density;
            RegenerationSpeciesAccount acc = Account(sitka);
            float req0 = acc.EstablishmentRequested, acc0 = acc.EstablishmentAccepted, rej0 = acc.CapacityRejected;
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            Establish();
            float requested = acc.EstablishmentRequested - req0, accepted = acc.EstablishmentAccepted - acc0, rejected = acc.CapacityRejected - rej0;
            Pass("CAPACITY_REJECTION_DOES_NOT_REMOVE_EXISTING_STOCK", a.Density == aBefore && b.Density == bBefore && accepted == 0f
                && requested > 0f && Mathf.Abs(requested - accepted - rejected) < 1e-6f,
                $"occupancy=1.4 sitka {F(aBefore)}->{F(a.Density)} beech {F(bBefore)}->{F(b.Density)} requested={F(requested)} accepted={F(accepted)} rejected={F(rejected)}");
        }

        // FULL_REJECTION_CREATES_NO_COHORT.
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            NextYear();
            Band(0, beech, RegenerationOrigin.Natural, 0, beech.RegenDensityMax, 0.5f);
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            Establish();
            Pass("FULL_REJECTION_CREATES_NO_COHORT", e.Cells[0].Bands("sitka-spruce", RegenerationOrigin.Natural).Count == 0,
                $"after=[{Describe(e.Cells[0].Regeneration)}]");
        }

        // BAND_MERGE_*: four natural bands (years 0,2,3,7) plus a planted band;
        // recruitment in year 8 overflows. Closest gaps tie at 1 ((2,3) and
        // (7,8)); the oldest pair (2,3) merges.
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            for (int y = 0; y < 8; y++) NextYear();
            Band(0, sitka, RegenerationOrigin.Natural, 0, 0.10f, 2.0f);
            Band(0, sitka, RegenerationOrigin.Natural, 2, 0.20f, 1.0f);
            Band(0, sitka, RegenerationOrigin.Natural, 3, 0.10f, 0.7f);
            Band(0, sitka, RegenerationOrigin.Natural, 7, 0.05f, 0.2f);
            ForestRegenerationCohort plantedBand = Band(0, sitka, RegenerationOrigin.Planted, 4, 0.10f, 0.9f);
            string plantedBefore = Describe(new[] { plantedBand });
            float naturalBefore = e.Cells[0].Bands("sitka-spruce", RegenerationOrigin.Natural).Sum(b => b.Density);
            RegenerationSpeciesAccount acc = Account(sitka);
            float acc0 = acc.EstablishmentAccepted; int merges0 = acc.BandMerges;
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            Establish();
            float accepted = acc.EstablishmentAccepted - acc0;
            var natural = e.Cells[0].Bands("sitka-spruce", RegenerationOrigin.Natural);
            float naturalAfter = natural.Sum(b => b.Density);
            ForestRegenerationCohort merged = natural.FirstOrDefault(b => b.EstablishYear == 2 || b.EstablishYear == 3);
            // Expected merged state: abundance-weighted height and year (0.2 @ y2, 0.1 @ y3 -> 2.333 -> 2).
            bool mergedOk = merged != null && merged.EstablishYear == 2 && Mathf.Abs(merged.Density - 0.3f) < 1e-6f
                && Mathf.Abs(merged.Height - (1.0f * 0.2f + 0.7f * 0.1f) / 0.3f) < 1e-5f;
            Pass("BAND_MERGE_CONSERVES_ABUNDANCE", accepted > 0f && Mathf.Abs(naturalAfter - (naturalBefore + accepted)) < 1e-6f && mergedOk
                && acc.BandMerges - merges0 == 1, $"naturalBefore={F(naturalBefore)} accepted={F(accepted)} naturalAfter={F(naturalAfter)} bands=[{Describe(natural)}]");
            Pass("BAND_MERGE_PRESERVES_ORIGIN", natural.All(b => b.Origin == RegenerationOrigin.Natural)
                && Describe(e.Cells[0].Bands("sitka-spruce", RegenerationOrigin.Planted)) == plantedBefore,
                $"planted={Describe(e.Cells[0].Bands("sitka-spruce", RegenerationOrigin.Planted))}");
            // Keep recruiting for 12 more years: never more than four bands per species + origin.
            int maxBands = 0;
            for (int y = 0; y < 12; y++)
            {
                NextYear();
                foreach (ForestEcologyCell c in e.Cells) c.ClearSeedRain();
                e.Cells[0].SetSeedRain("sitka-spruce", 100f);
                Grow(); Establish();
                maxBands = Math.Max(maxBands, e.Cells[0].Bands("sitka-spruce", RegenerationOrigin.Natural).Count);
            }
            Pass("MAX_BAND_COUNT_RESPECTED", maxBands <= ForestEcologyCell.MaxBandsPerSpeciesOrigin,
                $"max={maxBands} limit={ForestEcologyCell.MaxBandsPerSpeciesOrigin} final=[{Describe(e.Cells[0].Regeneration)}]");
        }

        // PROMOTION_REMOVES_ONLY_PROMOTED_BAND / YOUNGER_BANDS_SURVIVE_PROMOTION.
        {
            EmptyWorld(RegenerationModel.AgeBands, 1f);
            for (int y = 0; y < 10; y++) NextYear();
            float h = sitka.PromotionHeightM;
            Band(0, sitka, RegenerationOrigin.Natural, 0, 1.0f, h + 0.5f);
            ForestRegenerationCohort second = Band(0, sitka, RegenerationOrigin.Natural, 3, 0.3f, h + 0.3f);
            ForestRegenerationCohort young = Band(0, sitka, RegenerationOrigin.Natural, 6, 0.2f, 0.5f);
            string secondBefore = Describe(new[] { second }), youngBefore = Describe(new[] { young });
            int treesBefore = Living().Length;
            Promote();
            ForestTree[] created = Living().Where(t => t.TreeId.StartsWith("R")).ToArray();
            var remaining = e.Cells[0].Regeneration.ToList();
            Pass("PROMOTION_REMOVES_ONLY_PROMOTED_BAND", Living().Length == treesBefore + 1 && remaining.Count == 2
                && remaining.All(b => b.EstablishYear != 0) && created.Length == 1 && created[0].AgeYears == e.EcologicalYear,
                $"trees+={Living().Length - treesBefore} createdAge={(created.Length > 0 ? created[0].AgeYears : -1)} remaining=[{Describe(remaining)}]");
            Pass("YOUNGER_BANDS_SURVIVE_PROMOTION", Describe(new[] { second }) == secondBefore && Describe(new[] { young }) == youngBefore
                && remaining.Contains(second) && remaining.Contains(young), $"second={secondBefore} young={youngBefore}");
            NextYear();
            Promote();
            Debug.Log($"REGEN_MODEL_INFO NEXT_YEAR_PROMOTES_NEXT_OLDEST remaining=[{Describe(e.Cells[0].Regeneration)}] trees={Living().Length - treesBefore}");
            Check(e.Cells[0].Regeneration.Count == 1 && e.Cells[0].Regeneration[0] == young, "second-oldest band did not promote next year");
        }

        // CLEARANCE_RELEASES_CAPACITY (viability): a full cell rejects arrival;
        // production ApplyClearance removes every band; the next year's arrival
        // is admitted as a new age-0 band.
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            NextYear();
            Band(0, sitka, RegenerationOrigin.Natural, 0, sitka.RegenDensityMax * 0.6f, 1.0f);
            Band(0, sitka, RegenerationOrigin.Natural, 1, sitka.RegenDensityMax * 0.4f, 0.4f);
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            RegenerationSpeciesAccount acc = Account(sitka);
            float a0 = acc.EstablishmentAccepted;
            Establish();
            float blocked = acc.EstablishmentAccepted - a0;
            ClearanceTargets targets = manager.QueryClearance(ClearanceFootprint.Cell(e, 0), e.EcologicalYear);
            typeof(ScenarioOneManager).GetMethod("ApplyClearance", Private).Invoke(manager, new object[] { targets, e.EcologicalYear });
            int bandsAfterClearance = e.Cells[0].Regeneration.Count;
            NextYear();
            e.Cells[0].SetSeedRain("sitka-spruce", 100f);
            float a1 = acc.EstablishmentAccepted;
            Establish();
            float released = acc.EstablishmentAccepted - a1;
            ForestRegenerationCohort recruit = e.Cells[0].FindBand("sitka-spruce", RegenerationOrigin.Natural, e.EcologicalYear);
            Pass("CLEARANCE_RELEASES_CAPACITY", blocked == 0f && targets.Cohorts.Count == 2 && bandsAfterClearance == 0 && released > 0f
                && recruit != null && recruit.Height == sitka.RegenInitialHeightM,
                $"acceptedWhenFull={F(blocked)} clearedBands={targets.Cohorts.Count} removed={F(targets.Density)} bandsAfter={bandsAfterClearance} acceptedNextYear={F(released)}");
        }

        // EXTINCTION_THRESHOLD_ACCOUNTED: boundary behaviour and accounting.
        {
            EmptyWorld(RegenerationModel.AgeBands, 0.9f);
            NextYear();
            float survival = (float)JuvenileEcologyRules.SurvivalResponse(sitka, 0.9f, 0f);
            ForestRegenerationCohort atLimit = Band(0, sitka, RegenerationOrigin.Natural, 0, 0.01f / survival + 1e-6f, 0.3f);
            ForestRegenerationCohort below = Band(1, sitka, RegenerationOrigin.Natural, 0, 0.0099f, 0.3f);
            RegenerationSpeciesAccount acc = Account(sitka);
            float t0 = acc.ThresholdExtinction;
            float belowAfterSurvival = (float)(below.Density * survival);
            Grow();
            float extinguished = acc.ThresholdExtinction - t0;
            bool kept = e.Cells[0].Regeneration.Contains(atLimit) && atLimit.Density >= 0.01f;
            bool removed = e.Cells[1].Regeneration.Count == 0;
            // No recruitment follows an extinction without seed.
            for (int y = 0; y < 5; y++) { NextYear(); Establish(); }
            bool noRebirth = e.Cells[1].Regeneration.Count == 0;
            Pass("EXTINCTION_THRESHOLD_ACCOUNTED", kept && removed && noRebirth && Mathf.Abs(extinguished - belowAfterSurvival) < 1e-7f,
                $"survival={F(survival)} keptAt={F(atLimit.Density)} removedBelow extinguished={F(extinguished)} expected={F(belowAfterSurvival)} noRebirth={noRebirth}");
        }
    }

    // ---------- save/load and policy ----------

    private ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));

    private string RegenerationState() => string.Join("\n", e.Cells.Select((c, i) => i + ":" + Describe(c.Regeneration)));

    private IEnumerator SaveLoadAndPolicy()
    {
        // New game default.
        Pass("POLICY_NEW_GAME_MODEL1", ScenarioOneManager.NewGameRegenerationModel == RegenerationModel.AgeBands
            && original.regenerationModel == RegenerationModel.AgeBands && original.version == 16,
            $"newGame={ScenarioOneManager.NewGameRegenerationModel} capturedModel={original.regenerationModel} version={original.version}");

        // SAVE_LOAD_PRESERVES_ALL_BANDS: multi-band cells through the real save path.
        Check(saves.LoadData(Clone(original), false), "load new game");
        yield return null;
        foreach (ForestEcologyCell c in e.Cells) c.ClearRegeneration();
        Band(5, sitka, RegenerationOrigin.Natural, -2, 0.3f, 0.9f);
        Band(5, sitka, RegenerationOrigin.Natural, -1, 0.2f, 0.5f);
        Band(5, sitka, RegenerationOrigin.Planted, -3, 0.4f, 0.8f);
        Band(5, beech, RegenerationOrigin.Natural, 0, 0.1f, 0.15f);
        Band(9, oak, RegenerationOrigin.Natural, -4, 0.0105f, 0.4f);
        string before = RegenerationState();
        ForestSaveData data = saves.CaptureData();
        string json = JsonUtility.ToJson(data);
        string validation = ForestSaveValidation.Validate(JsonUtility.FromJson<ForestSaveData>(json), 0, e.CellCount);
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), "multi-band load");
        yield return null;
        string after = RegenerationState();
        int sitkaRecords = data.cells.Single(c => c.index == 5).cohorts.Count(c => c.speciesId == "sitka-spruce");
        Pass("SAVE_LOAD_PRESERVES_ALL_BANDS", before == after && validation == null && sitkaRecords == 3 && e.RegenerationModelVersion == 1,
            $"sitkaRecordsInCell5={sitkaRecords} validation={validation ?? "ok"} identical={before == after}");

        // Validation rejects a duplicate band key and a fifth band; model 0 keeps historical leniency.
        ForestSaveData duplicate = JsonUtility.FromJson<ForestSaveData>(json);
        ForestCellSaveData cell5 = duplicate.cells.Single(c => c.index == 5);
        cell5.cohorts.Add(JsonUtility.FromJson<ForestRegenerationCohortSaveData>(JsonUtility.ToJson(cell5.cohorts.First(c => c.speciesId == "sitka-spruce"))));
        string dupProblem = ForestSaveValidation.Validate(duplicate, 0, e.CellCount);
        ForestSaveData tooMany = JsonUtility.FromJson<ForestSaveData>(json);
        for (int k = 0; k < 4; k++)
            tooMany.cells.Single(c => c.index == 5).cohorts.Add(new ForestRegenerationCohortSaveData { speciesId = "beech", density = 0.01f, height = 0.15f, establishYear = 10 + k });
        string manyProblem = ForestSaveValidation.Validate(tooMany, 0, e.CellCount);
        Pass("SAVE_VALIDATION_BAND_KEYS", dupProblem != null && manyProblem != null, $"duplicate=[{dupProblem}] fifthBand=[{manyProblem}]");

        // Deterministic continuation around the threshold: save at year Y, continue
        // 10 years, compare with the uninterrupted run (oak band starts at 0.0105).
        string mid = JsonUtility.ToJson(saves.CaptureData());
        for (int y = 0; y < 10; y++) { Check(manager.AdvanceYear(), "continuation advance"); if (y % 5 == 4) yield return null; }
        string uninterrupted = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(mid), false), "mid reload");
        yield return null;
        for (int y = 0; y < 10; y++) { Check(manager.AdvanceYear(), "reloaded advance"); if (y % 5 == 4) yield return null; }
        string resumed = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Pass("SAVE_LOAD_DETERMINISTIC_AROUND_THRESHOLD", uninterrupted == resumed, $"uninterrupted={uninterrupted} resumed={resumed}");

        // Missing field -> model 0; pre-v16 saves -> model 0 even if a field is present.
        string stripped = JsonUtility.ToJson(Clone(original)).Replace("\"regenerationModel\":1,", "");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(stripped), false), "stripped load");
        yield return null;
        bool missingIsZero = e.RegenerationModelVersion == RegenerationModel.Legacy;
        ForestSaveData v15 = Clone(original); v15.version = 15; v15.regenerationModel = 1;
        Check(saves.LoadData(v15, false), "v15 load");
        yield return null;
        bool v15IsZero = e.RegenerationModelVersion == RegenerationModel.Legacy;
        ForestSaveData explicitZero = Clone(original); explicitZero.regenerationModel = 0;
        Check(saves.LoadData(explicitZero, false), "explicit model-0 load");
        yield return null;
        bool explicitIsZero = e.RegenerationModelVersion == RegenerationModel.Legacy;
        Pass("POLICY_OLD_SAVES_MODEL0", missingIsZero && v15IsZero && explicitIsZero && !stripped.Contains("regenerationModel"),
            $"missingField={missingIsZero} v15WithField={v15IsZero} explicit0={explicitIsZero}");

        // Reference Future v1 previews are model 0; the player's model returns afterwards.
        Check(saves.LoadData(Clone(original), false), "restore new game");
        yield return null;
        bool previewZero = true, restored = true;
        foreach (int year in new[] { 0, 20, 50, 100 })
        {
            Check(manager.TryBeginReferencePreview(year), "preview " + year);
            yield return null; yield return null;
            previewZero &= e.RegenerationModelVersion == RegenerationModel.Legacy;
            manager.EndReferencePreview();
            yield return null; yield return null;
            restored &= e.RegenerationModelVersion == RegenerationModel.AgeBands;
        }
        Pass("POLICY_REFERENCE_V1_MODEL0", previewZero && restored, $"previewModel0={previewZero} playerModelRestored={restored}");
    }

    // ---------- lifecycle anchors ----------

    private IEnumerator Anchors()
    {
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        Check(hashMethod != null, "lifecycle hash entry point missing");
        float scenarioPressure = manager.Definition.BackgroundBrowsePressure;
        var results = new Dictionary<string, string>();
        foreach ((string label, int rng, int regen) in new[] { ("rng0_regen0", 0, 0), ("rng1_regen1", 1, 1), ("rng1_regen1_repeat", 1, 1) })
            foreach (float pressure in new[] { 0f, scenarioPressure })
            {
                ForestStandScenarios.ApplyLifecycleFixture();
                e.RngModelVersion = rng;
                e.RegenerationModelVersion = regen;
                e.Browsing.BackgroundPressure = pressure;
                e.Browsing.ClearProtection();
                for (int year = 0; year < 80; year++) { e.AdvanceOneYear(); if (year % 10 == 9) yield return null; }
                results[label + (pressure == 0f ? "_neutral" : "_normal")] = (string)hashMethod.Invoke(null, new object[] { e });
            }
        e.Browsing.BackgroundPressure = scenarioPressure;
        Pass("LEGACY_ANCHORS_MODEL0", results["rng0_regen0_neutral"] == Neutral0 && results["rng0_regen0_normal"] == Normal0,
            $"neutral={results["rng0_regen0_neutral"]} normal={results["rng0_regen0_normal"]}");
        Pass("MODEL1_DETERMINISTIC", results["rng1_regen1_neutral"] == results["rng1_regen1_repeat_neutral"]
            && results["rng1_regen1_normal"] == results["rng1_regen1_repeat_normal"],
            $"neutral={results["rng1_regen1_neutral"]} normal={results["rng1_regen1_normal"]} repeat=identical");
        Debug.Log($"REGEN_MODEL_ANCHORS_MODEL1 neutral={results["rng1_regen1_neutral"]} normal={results["rng1_regen1_normal"]}");
    }

    // ---------- long-run funnels, model 0 vs model 1 (production ledger) ----------

    private void Build(float removal, int model)
    {
        DestroyAllTrees();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = model;
        e.Browsing.BackgroundPressure = manager.Definition.BackgroundBrowsePressure;
        e.Browsing.ClearProtection();
        stand.Generate();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = model;
        e.InvalidateCompetition();
        ForestTree[] ordered = Living().OrderBy(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        e.BeginChangeBatch();
        foreach (ForestTree t in ordered.Take(Mathf.RoundToInt(ordered.Length * removal))) t.Fell();
        e.EndChangeBatch();
    }

    private sealed class Totals
    {
        public double Arrival, Requested, Accepted, Rejected, Infill, Contraction, Light, Browse, Threshold, Exported;
        public int Trees, Bands, Merges;
        public void Add(RegenerationSpeciesAccount a)
        {
            Arrival += a.SeedArrival; Requested += a.EstablishmentRequested; Accepted += a.EstablishmentAccepted;
            Rejected += a.CapacityRejected; Infill += a.InfillAccepted; Contraction += a.CapacityContraction;
            Light += a.LightLoss; Browse += a.BrowseLoss; Threshold += a.ThresholdExtinction; Exported += a.PromotionExported;
            Trees += a.ExactTreesCreated; Bands += a.BandsCreated; Merges += a.BandMerges;
        }
    }

    private IEnumerator Funnels(StreamWriter w)
    {
        int[] horizons = { 10, 25, 50, 100 };
        var repeatCheck = new Dictionary<string, string>();
        foreach ((string label, float removal) in new[] { ("unthinned", 0f), ("remove_smallest_20", 0.2f), ("remove_smallest_60", 0.6f) })
            foreach (int model in new[] { 0, 1, 1 })
            {
                bool repeat = model == 1 && repeatCheck.ContainsKey(label);
                Build(removal, model);
                var totals = new SortedDictionary<string, Totals>(StringComparer.Ordinal);
                var line = new System.Text.StringBuilder();
                for (int y = 1; y <= years; y++)
                {
                    e.AdvanceOneYear();
                    foreach (var pair in e.LastRegenerationAccount.Species)
                    {
                        if (!totals.TryGetValue(pair.Key, out Totals t)) totals[pair.Key] = t = new Totals();
                        t.Add(pair.Value);
                    }
                    if (horizons.Contains(y))
                    {
                        ForestTree[] living = Living();
                        foreach (var pair in totals)
                        {
                            Totals t = pair.Value;
                            float standing = e.Cells.Sum(c => c.SpeciesDensity(pair.Key));
                            int bands = e.Cells.Sum(c => c.Regeneration.Count(r => r.SpeciesId == pair.Key && r.Density > 0f));
                            int occupied = e.Cells.Count(c => c.SpeciesDensity(pair.Key) > 0f);
                            int recruits = living.Count(tr => tr.Species.SpeciesId == pair.Key && (tr.TreeId.StartsWith("R") || tr.TreeId.StartsWith("PL")));
                            string row = string.Join(",", label, model, y, pair.Key, t.Arrival.ToString("0.###", Inv), t.Requested.ToString("0.####", Inv),
                                t.Accepted.ToString("0.####", Inv), t.Rejected.ToString("0.####", Inv), t.Infill.ToString("0.####", Inv),
                                t.Contraction.ToString("0.####", Inv), t.Light.ToString("0.####", Inv), t.Browse.ToString("0.####", Inv),
                                t.Threshold.ToString("0.####", Inv), t.Exported.ToString("0.####", Inv), t.Trees, t.Bands, t.Merges,
                                standing.ToString("0.####", Inv), bands, occupied, recruits, living.Length);
                            line.AppendLine(row);
                            if (!repeat) w.WriteLine(row);
                        }
                    }
                    yield return null;
                    if (y % 20 == 0) { yield return Resources.UnloadUnusedAssets(); GC.Collect(); }
                }
                if (model == 1)
                {
                    if (!repeat) repeatCheck[label] = line.ToString();
                    else Check(repeatCheck[label] == line.ToString(), $"model-1 funnel {label} not deterministic on repeat");
                }
                w.Flush();
            }
        Pass("MODEL1_FUNNEL_REPEAT_IDENTICAL", failures.All(f => !f.Contains("not deterministic on repeat")), "treatments=3 years=" + years);
    }

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        stand = FindFirstObjectByType<ForestStartingStand>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Check(e != null && stand != null && spawner != null && manager != null && saves != null, "scene systems");
        sitka = spawner.ResolveSpecies("sitka-spruce"); beech = spawner.ResolveSpecies("beech"); oak = spawner.ResolveSpecies("sessile-oak");
        Check(sitka != null && beech != null && oak != null, "species");
        dir = Environment.GetEnvironmentVariable("CCF_DIAG_DIR") ?? Path.Combine(Application.temporaryCachePath, "regeneration-model");
        years = int.TryParse(Environment.GetEnvironmentVariable("CCF_REGEN_YEARS"), out int parsed) ? parsed : 100;
        Directory.CreateDirectory(dir);
        original = saves.CaptureData();

        yield return SaveLoadAndPolicy();
        Check(saves.LoadData(Clone(original), false), "restore before fixtures");
        yield return null;
        Invariants();
        yield return Anchors();
        using (var w = new StreamWriter(Path.Combine(dir, "model_funnels.csv")))
        {
            w.WriteLine("treatment,regeneration_model,horizon,species,seed_arrival,establishment_requested,establishment_accepted,capacity_rejected,legacy_infill,capacity_contraction,light_loss,browse_loss,threshold_extinction,promotion_exported,exact_trees_created,bands_created,band_merges,remaining_abundance,remaining_bands,occupied_cells,recruited_trees_alive,living_trees");
            yield return Funnels(w);
        }
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
        try { if (original != null) { manager.EndReferencePreview(); saves.LoadData(original, false); } }
        catch (Exception ex) { failure = failure ?? ex.ToString(); }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"REGENERATION_MODEL_VERIFY_PASS invariants={passed}"
            : $"REGENERATION_MODEL_VERIFY_FAIL {failure} checks=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}

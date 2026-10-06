using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable regeneration-accounting evidence (task/regeneration-population-budget).
// Copy into Assets/ForestPrototype, run with -executeMethod RegenerationBudgetEvidence.Begin,
// then remove the copy and its .meta. Production code is not modified.
//
// The annual pipeline is split at its existing private method boundaries. Two
// regeneration stages (GrowExistingRegeneration, EstablishNewCohorts) are run as
// instrumented copies of the production code so their internal losses can be
// separated; every treatment is then replayed through the unmodified
// AdvanceOneYear and the full world state must be identical at 10/25/50/100
// years, which proves the copies are faithful. All other stages are production.
//
// Regeneration model: pinned to 0 (legacy). This harness measures the BEFORE
// state; its instrumented copies reproduce the legacy stage code. Model 1 is
// verified by RegenerationModelVerification.
//
// Units: regeneration "density" is the production relative-abundance value
// (normalised occupancy via RegenDensityMax). It is NOT stems/ha and no
// conversion to a count is made anywhere in this harness.
public static class RegenerationBudgetEvidence
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
        if (Environment.GetCommandLineArgs().Any(a => a == "RegenerationBudgetEvidence.Begin"))
            new GameObject("Regeneration budget evidence").AddComponent<RegenerationBudgetRunner>();
    }
}

public sealed class RegenerationBudgetRunner : MonoBehaviour
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly int[] Horizons = { 10, 25, 50, 100 };

    private ForestEcologyController e;
    private ForestStartingStand stand;
    private ForestTreeSpawner spawner;
    private ScenarioOneManager manager;
    private string dir;
    private int years;
    private StreamWriter facts;
    private readonly List<string> failures = new List<string>();

    private sealed class Row
    {
        public string Treatment, Species, Origin = "";
        public int Year, Cell, EstYearBefore = -1, EstYearAfter = -1, InheritedAge = -1, Trees;
        public float Light, Arrival, Before, LightLoss, BrowseLoss, InfillRequested, InfillAccepted, InfillContraction,
            ThresholdLoss, EstRequested, EstAccepted, EstRejected, EstContraction, BeforePromotion, Exported, After,
            HeightBefore, HeightAfter, InheritedHeight;
        public bool MergedIntoOlder, AgeInitialisedWithZeroAccepted, SubthresholdArrival;
        public float Residual => After - (Before - LightLoss - BrowseLoss + InfillAccepted - InfillContraction
            - ThresholdLoss + EstAccepted - EstContraction - Exported);
    }

    private static string F(float f) => f.ToString("R", Inv);
    private void Fact(string line) { facts.WriteLine(line); Debug.Log("REGEN_BUDGET " + line); }
    private void Check(bool ok, string what) { if (!ok) { failures.Add(what); Debug.LogError("REGEN_BUDGET_CHECK_FAIL " + what); } }
    private void Call(string name, params object[] args) => typeof(ForestEcologyController).GetMethod(name, Private).Invoke(e, args);
    private void SetField(string name, object value) => typeof(ForestEcologyController).GetField(name, Private).SetValue(e, value);
    private TreeSpeciesDefinition Species(string id) => spawner.ResolveSpecies(id);
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    // ---------- world set-up ----------

    private void DestroyAllTrees()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
    }

    // Actual Scenario One starting-stand generator, RNG model 1, stable seed;
    // the given fraction of the smallest stems is felled at year 0.
    private void Build(float removal)
    {
        ClearManagerJuveniles();
        DestroyAllTrees();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.Legacy; // BEFORE baseline: legacy regeneration
        e.GrowthModelVersion = GrowthModel.Legacy;
        stand.Generate();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.Legacy; // BEFORE baseline: legacy regeneration
        e.GrowthModelVersion = GrowthModel.Legacy;
        e.InvalidateCompetition();
        ForestTree[] ordered = Living().OrderBy(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        e.BeginChangeBatch();
        foreach (ForestTree t in ordered.Take(Mathf.RoundToInt(ordered.Length * removal))) t.Fell();
        e.EndChangeBatch();
    }

    private List<PlantedJuvenile> ManagerJuveniles() => manager == null ? null
        : (List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles", Private).GetValue(manager);

    private void ClearManagerJuveniles() => ManagerJuveniles()?.Clear();

    private string State()
    {
        var b = new StringBuilder();
        b.Append(e.EcologicalYear).Append('|').Append(e.RngModelVersion).Append('|').Append(e.LastMastLabel).Append('|').Append(F(e.LastMastMultiplier)).AppendLine();
        foreach (ForestTree t in Living())
            b.Append(t.TreeId).Append('|').Append(F(t.transform.position.x)).Append('|').Append(F(t.transform.position.z)).Append('|')
             .Append(F(t.Diameter)).Append('|').Append(F(t.Height)).Append('|').Append(F(t.CrownRadius)).Append('|').Append(t.AgeYears).AppendLine();
        foreach (ForestEcologyCell c in e.Cells)
        {
            b.Append(F(c.Light)).Append('|').Append(F(c.EstablishmentSuitability)).Append('|').Append(F(c.RecentOpening));
            foreach (ForestRegenerationCohort r in c.Regeneration)
                b.Append('|').Append(r.SpeciesId).Append(':').Append(F(r.Density)).Append(':').Append(F(r.Height)).Append(':')
                 .Append(r.EstablishYear).Append(':').Append(F(r.SeedRain)).Append(':').Append(r.Origin).Append(':').Append(r.OriginYear);
            b.AppendLine();
        }
        return b.ToString();
    }

    private static string Hash(string text)
    {
        using (SHA256 sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").Substring(0, 16);
    }

    // ---------- traced annual step ----------

    private Row RowFor(Dictionary<string, Row> rows, string treatment, int year, int cell, string species)
    {
        string key = cell + "|" + species;
        if (!rows.TryGetValue(key, out Row row))
        {
            row = new Row { Treatment = treatment, Year = year, Cell = cell, Species = species, Light = e.Cells[cell].Light };
            rows[key] = row;
        }
        return row;
    }

    // Instrumented copy of ForestEcologyController.GrowExistingRegeneration.
    // Operation order and float/double conversions are identical to production.
    private void TracedGrowExisting(Dictionary<string, Row> rows, string treatment, int year)
    {
        ForestEcologyCell[] cells = e.Cells;
        for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
        {
            ForestEcologyCell cell = cells[cellIndex];
            for (int i = 0; i < cell.Regeneration.Count; i++)
            {
                ForestRegenerationCohort cohort = cell.Regeneration[i];
                TreeSpeciesDefinition cohortSpecies = cohort.Species;
                if (cohort.Density <= 0f || cohortSpecies == null)
                    continue;
                Row row = RowFor(rows, treatment, year, cellIndex, cohortSpecies.SpeciesId);
                row.Light = cell.Light;
                row.Before = cohort.Density;
                row.HeightBefore = cohort.Height;
                row.EstYearBefore = cohort.EstablishYear;
                float browsed = e.Browsing.BackgroundPressure > 0f
                    ? e.AssessCohortBrowse(cellIndex, cohortSpecies, cohort.Height).Probability : 0f;
                cohort.LastBrowsedFraction = browsed;
                cohort.LastBrowseAssessmentYear = year;
                float growthResponse = JuvenileEcologyRules.LightResponse(cohortSpecies, cell.Light);
                JuvenileEcologyRules.GrowHeight(ref cohort.Height, cohortSpecies, cell.Light, cell.SiteProductivity, browsed);
                double lightSurvival = JuvenileEcologyRules.SurvivalResponse(cohortSpecies, cell.Light);
                double survival = JuvenileEcologyRules.SurvivalResponse(cohortSpecies, cell.Light, browsed);
                float before = cohort.Density;
                cohort.Density = (float)(cohort.Density * survival);
                // Attribution only: light loss first, then the additional browse loss.
                float lightOnly = (float)(before * lightSurvival);
                row.LightLoss = before - lightOnly;
                row.BrowseLoss = lightOnly - cohort.Density;
                bool recoverDensity = cohortSpecies.UsesDistinctJuvenileLightResponses
                    ? lightSurvival >= 0.999999f
                    : growthResponse >= cohortSpecies.RegenPoorLightThreshold && lightSurvival == 1d;
                if (recoverDensity)
                {
                    float pre = cohort.Density;
                    cell.AddDensityWithSharedCapacity(cohort, 0.05f);
                    row.InfillRequested = 0.05f;
                    float delta = cohort.Density - pre;
                    if (delta >= 0f) row.InfillAccepted = delta; else row.InfillContraction = -delta;
                }
                if (cohort.Density < 0.01f)
                {
                    row.ThresholdLoss = cohort.Density;
                    cohort.Density = 0f;
                    cohort.Height = 0f;
                    cohort.EstablishYear = -1;
                }
            }
        }
    }

    // Instrumented copy of ForestEcologyController.EstablishNewCohorts.
    private void TracedEstablish(Dictionary<string, Row> rows, string treatment, int year)
    {
        ForestEcologyCell[] cells = e.Cells;
        for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
        {
            ForestEcologyCell cell = cells[cellIndex];
            for (int i = 0; i < cell.Regeneration.Count; i++)
            {
                ForestRegenerationCohort cohort = cell.Regeneration[i];
                TreeSpeciesDefinition cohortSpecies = cohort.Species;
                if (cohort.SeedRain <= 0f || cohortSpecies == null)
                    continue;
                Row row = RowFor(rows, treatment, year, cellIndex, cohortSpecies.SpeciesId);
                row.Arrival = cohort.SeedRain;
                float seedFactor = 1f - Mathf.Exp(-cohort.SeedRain / cohortSpecies.SeedSaturationS50);
                float lightResponse = cohortSpecies.JuvenileEstablishmentResponse(cell.Light);
                float establishment = seedFactor * lightResponse * cell.EstablishmentSuitability;
                if (establishment <= 0.01f)
                {
                    row.SubthresholdArrival = true;
                    continue;
                }
                int yearBefore = cohort.EstablishYear;
                float heightBefore = cohort.Height;
                bool initialised = false;
                if (cohort.EstablishYear < 0 && cohort.Origin != RegenerationOrigin.Planted)
                {
                    cohort.EstablishYear = year;
                    cohort.Height = cohortSpecies.RegenInitialHeightM;
                    cohort.OriginYear = year;
                    initialised = true;
                }
                float requested = establishment * cohortSpecies.RegenDensityPerEstablishment;
                float pre = cohort.Density;
                cell.AddDensityWithSharedCapacity(cohort, requested);
                float delta = cohort.Density - pre;
                row.EstRequested = requested;
                if (delta >= 0f) row.EstAccepted = delta; else row.EstContraction = -delta;
                row.EstRejected = requested - Mathf.Max(0f, delta);
                if (initialised && delta <= 0f) row.AgeInitialisedWithZeroAccepted = true;
                // New recruits added to a cohort that already has an older age/height.
                if (!initialised && delta > 0f && pre > 0f && yearBefore >= 0 && yearBefore < year)
                {
                    row.MergedIntoOlder = true;
                    row.InheritedAge = year - yearBefore;
                    row.InheritedHeight = heightBefore;
                }
            }
        }
    }

    private Dictionary<string, Row> TraceYear(string treatment)
    {
        TreeSpeciesDefinition s = e.ResolveSpecies();
        int y = e.EcologicalYear + 1;
        SetField("ecologicalYear", y);
        System.Random rng = SimulationRandom.Create(e.RngModelVersion, e.SimulationSeed, y, 0);
        Call("UpdateCompetition", s); SetField("competitionCurrent", true); e.RecomputeCanopy();
        Call("GrowAdults", s); Call("RelaxCrowns", s); e.RecomputeCanopy();

        var rows = new Dictionary<string, Row>();
        TracedGrowExisting(rows, treatment, y);
        Call("UpdateAllMastStates", s, rng);
        Call("ComputeSeedRain", s);
        TracedEstablish(rows, treatment, y);

        for (int i = 0; i < e.Cells.Length; i++)
            foreach (ForestRegenerationCohort c in e.Cells[i].Regeneration)
                if (c.Density > 0f) RowFor(rows, treatment, y, i, c.SpeciesId).BeforePromotion = c.Density;
        HashSet<string> oldIds = new HashSet<string>(Living().Select(t => t.TreeId));
        Call("PromoteCohorts", s, rng);
        foreach (ForestTree t in Living().Where(t => !oldIds.Contains(t.TreeId)))
        {
            int cell = e.GetCellIndex(t.transform.position);
            Check(cell >= 0, $"{treatment} y{y} promoted tree {t.TreeId} outside the grid");
            if (cell < 0) continue;
            Row row = RowFor(rows, treatment, y, cell, t.Species.SpeciesId);
            row.Trees++;
        }
        Call("UpdateEstablishmentSuitability"); Call("DecayRecentOpening", s); Call("LogSummary", s);
        SetField("seedlingVisualsDirty", true);

        foreach (Row row in rows.Values)
        {
            ForestRegenerationCohort c = e.Cells[row.Cell].FindCohort(row.Species);
            row.After = c != null ? c.Density : 0f;
            row.HeightAfter = c != null ? c.Height : 0f;
            row.EstYearAfter = c != null ? c.EstablishYear : -1;
            row.Origin = c != null ? c.Origin.ToString() : "";
            if (row.Trees > 0) row.Exported = row.BeforePromotion - row.After;
        }
        return rows;
    }

    private const string LedgerHeader = "treatment,year,cell,species,light,seed_arrival_relative,subthreshold_arrival,density_before,light_loss,browse_loss,"
        + "infill_requested,infill_accepted,infill_contraction,infill_unsourced,threshold_loss,establishment_requested,establishment_accepted,"
        + "establishment_rejected,establishment_contraction,merged_into_older,inherited_age,inherited_height,age_initialised_zero_accepted,"
        + "density_before_promotion,exported_relative,exact_trees_created,density_after,height_before,height_after,establish_year_before,"
        + "establish_year_after,origin,identity_residual";

    private static string LedgerLine(Row r) => string.Join(",", r.Treatment, r.Year, r.Cell, r.Species, F(r.Light), F(r.Arrival),
        r.SubthresholdArrival ? 1 : 0, F(r.Before), F(r.LightLoss), F(r.BrowseLoss), F(r.InfillRequested), F(r.InfillAccepted),
        F(r.InfillContraction), r.InfillAccepted > 0f && r.Arrival <= 0f ? 1 : 0, F(r.ThresholdLoss), F(r.EstRequested), F(r.EstAccepted),
        F(r.EstRejected), F(r.EstContraction), r.MergedIntoOlder ? 1 : 0, r.InheritedAge, F(r.InheritedHeight),
        r.AgeInitialisedWithZeroAccepted ? 1 : 0, F(r.BeforePromotion), F(r.Exported), r.Trees, F(r.After), F(r.HeightBefore),
        F(r.HeightAfter), r.EstYearBefore, r.EstYearAfter, r.Origin, F(r.Residual));

    // ---------- funnels ----------

    private sealed class Funnel
    {
        public double Arrival, Requested, Accepted, Rejected, LightLoss, BrowseLoss, Infill, InfillUnsourced, Contraction,
            Threshold, Exported, MaxResidual;
        public int Trees, Merges, ZeroAcceptInit, InheritedAgeSum, RejectionEvents, EstablishmentEvents;
        public void Add(Row r)
        {
            Arrival += r.Arrival; Requested += r.EstRequested; Accepted += r.EstAccepted; Rejected += r.EstRejected;
            LightLoss += r.LightLoss; BrowseLoss += r.BrowseLoss; Infill += r.InfillAccepted;
            if (r.InfillAccepted > 0f && r.Arrival <= 0f) InfillUnsourced += r.InfillAccepted;
            Contraction += r.InfillContraction + r.EstContraction; Threshold += r.ThresholdLoss; Exported += r.Exported;
            Trees += r.Trees; if (r.MergedIntoOlder) { Merges++; InheritedAgeSum += r.InheritedAge; }
            if (r.AgeInitialisedWithZeroAccepted) ZeroAcceptInit++;
            if (r.EstRequested > 0f) { EstablishmentEvents++; if (r.EstRejected > 0f) RejectionEvents++; }
            MaxResidual = Math.Max(MaxResidual, Math.Abs(r.Residual));
        }
    }

    private IEnumerator RunTreatment(string label, float removal, StreamWriter ledger, StreamWriter funnels, StreamWriter hashes)
    {
        Build(removal);
        Fact($"TREATMENT {label} removal={F(removal)} livingStart={Living().Length} rngModel={e.RngModelVersion} seed={e.SimulationSeed} browsePressure={F(e.Browsing.BackgroundPressure)}");
        var cumulative = new Dictionary<string, Funnel>();
        var traced = new Dictionary<int, string>();
        for (int y = 1; y <= years; y++)
        {
            Dictionary<string, Row> rows = TraceYear(label);
            foreach (Row r in rows.Values.OrderBy(r => r.Cell).ThenBy(r => r.Species, StringComparer.Ordinal))
            {
                ledger.WriteLine(LedgerLine(r));
                if (!cumulative.TryGetValue(r.Species, out Funnel f)) cumulative[r.Species] = f = new Funnel();
                f.Add(r);
            }
            if (Horizons.Contains(y) && y <= years)
            {
                string state = State();
                traced[y] = Hash(state);
                ForestTree[] living = Living();
                foreach (var pair in cumulative.OrderBy(p => p.Key, StringComparer.Ordinal))
                {
                    Funnel f = pair.Value;
                    double standing = e.Cells.Sum(c => (double)(c.FindCohort(pair.Key)?.Density ?? 0f));
                    int recruitsAlive = living.Count(t => t.Species.SpeciesId == pair.Key && (t.TreeId.StartsWith("R") || t.TreeId.StartsWith("PL")));
                    int occupied = e.Cells.Count(c => (c.FindCohort(pair.Key)?.Density ?? 0f) > 0f);
                    funnels.WriteLine(string.Join(",", label, y, pair.Key, f.Arrival.ToString("R", Inv), f.Requested.ToString("R", Inv),
                        f.Accepted.ToString("R", Inv), f.Rejected.ToString("R", Inv), f.EstablishmentEvents, f.RejectionEvents,
                        f.LightLoss.ToString("R", Inv), f.BrowseLoss.ToString("R", Inv), f.Infill.ToString("R", Inv),
                        f.InfillUnsourced.ToString("R", Inv), f.Contraction.ToString("R", Inv), f.Threshold.ToString("R", Inv),
                        f.Exported.ToString("R", Inv), f.Trees, f.Merges, f.Merges > 0 ? (f.InheritedAgeSum / (double)f.Merges).ToString("0.00", Inv) : "0",
                        f.ZeroAcceptInit, standing.ToString("R", Inv), occupied, recruitsAlive, living.Length, f.MaxResidual.ToString("R", Inv)));
                    Check(f.MaxResidual < 1e-5, $"{label} y{y} {pair.Key} ledger identity residual {f.MaxResidual}");
                }
                funnels.Flush(); ledger.Flush();
                hashes.WriteLine($"{label},traced,{y},{traced[y]}");
            }
            yield return null;
            if (y % 10 == 0) { yield return Resources.UnloadUnusedAssets(); GC.Collect(); }
        }

        // Faithfulness: replay through the unmodified production annual API.
        Build(removal);
        for (int y = 1; y <= years; y++)
        {
            e.AdvanceOneYear();
            if (traced.TryGetValue(y, out string expected))
            {
                string actual = Hash(State());
                hashes.WriteLine($"{label},ordinary,{y},{actual}");
                Check(actual == expected, $"{label} traced pipeline differs from AdvanceOneYear at year {y}");
                Fact($"FAITHFUL {label} year={y} traced={expected} ordinary={actual} identical={actual == expected}");
            }
            yield return null;
            if (y % 10 == 0) { yield return Resources.UnloadUnusedAssets(); GC.Collect(); }
        }
        hashes.Flush();
    }

    // ---------- fixtures (production stage methods, controlled light) ----------

    private void EmptyWorld(float light)
    {
        ClearManagerJuveniles();
        DestroyAllTrees();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.Legacy; // BEFORE baseline: legacy regeneration
        e.GrowthModelVersion = GrowthModel.Legacy;
        e.RecomputeCanopy();
        foreach (ForestEcologyCell c in e.Cells) { c.ClearRegeneration(); c.Light = light; }
    }

    private void NextYear() => SetField("ecologicalYear", e.EcologicalYear + 1);

    private void Fixtures(StreamWriter np)
    {
        TreeSpeciesDefinition sitka = Species("sitka-spruce"), beech = Species("beech"), oak = Species("sessile-oak");
        Check(sitka != null && beech != null && oak != null, "fixture species resolve");
        foreach (TreeSpeciesDefinition s in new[] { sitka, beech, oak })
            Fact($"SPECIES {s.SpeciesId} regenDensityMax={F(s.RegenDensityMax)} densityPerEstablishment={F(s.RegenDensityPerEstablishment)} initialHeight={F(s.RegenInitialHeightM)} promotionHeight={F(s.PromotionHeightM)} promotionMinLight={F(s.PromotionMinimumLight)} distinctLight={s.UsesDistinctJuvenileLightResponses}");

        // FX1 no seed, no cohort: nothing may appear.
        EmptyWorld(0.9f);
        for (int y = 0; y < 5; y++) { NextYear(); Call("GrowExistingRegeneration"); Call("EstablishNewCohorts"); }
        float total = e.Cells.Sum(c => c.Regeneration.Sum(r => r.Density));
        Fact($"FX1_NO_SEED_NO_COHORT years=5 totalDensity={F(total)} result={(total == 0f ? "PASS no unsourced creation" : "FAIL")}");

        // FX2 no seed, existing cohort: surviving stock vs unsourced infill, separately.
        foreach (float light in new[] { 0.9f, 0.3f, 0.05f })
        {
            EmptyWorld(light);
            ForestRegenerationCohort c = e.Cells[0].GetOrCreateCohort(sitka);
            c.Restore(0.2f, sitka.RegenInitialHeightM, 0);
            float start = c.Density, survivorOnly = c.Density;
            var trace = new StringBuilder();
            for (int y = 1; y <= 10; y++)
            {
                NextYear();
                float pre = c.Density;
                Call("GrowExistingRegeneration");
                survivorOnly = (float)(survivorOnly * JuvenileEcologyRules.SurvivalResponse(sitka, light, c.LastBrowsedFraction));
                trace.Append($" y{y}={F(c.Density)}");
            }
            Fact($"FX2_NO_SEED_EXISTING_COHORT light={F(light)} start={F(start)} after10={F(c.Density)} survivorsOnlyCounterfactual={F(survivorOnly)} unsourcedGain={F(c.Density - survivorOnly)} seedArrival=0 trace:{trace}");
        }

        // FX3 late recruitment into an older cohort inherits its age and height.
        EmptyWorld(0.9f);
        for (int y = 0; y < 8; y++) NextYear();
        {
            ForestRegenerationCohort c = e.Cells[0].GetOrCreateCohort(sitka);
            c.Restore(0.1f, 1.2f, 0);
            NextYear();
            c.SeedRain = 100f;
            float pre = c.Density;
            Call("EstablishNewCohorts");
            Fact($"FX3_LATE_RECRUIT_INHERITS year={e.EcologicalYear} densityBefore={F(pre)} densityAfter={F(c.Density)} accepted={F(c.Density - pre)} establishYearAfter={c.EstablishYear} heightAfter={F(c.Height)} newRecruitAgeRecorded={e.EcologicalYear - c.EstablishYear} newRecruitHeightRecorded={F(c.Height)} expectedNewRecruitHeight={F(sitka.RegenInitialHeightM)}");
            Check(c.Density > pre && c.EstablishYear == 0, "FX3 expected merge into older cohort");
        }

        // FX4 full capacity rejection still initialises age/height of an empty cohort.
        EmptyWorld(0.9f);
        NextYear();
        {
            ForestRegenerationCohort occupant = e.Cells[0].GetOrCreateCohort(beech);
            occupant.Restore(beech.RegenDensityMax, 0.5f, 0);
            ForestRegenerationCohort target = e.Cells[0].GetOrCreateCohort(sitka);
            target.SeedRain = 100f;
            Call("EstablishNewCohorts");
            Fact($"FX4_FULL_REJECTION_AGE_INIT occupancyBefore=1 sitkaDensityAfter={F(target.Density)} sitkaEstablishYear={target.EstablishYear} sitkaHeight={F(target.Height)} defect={(target.Density == 0f && target.EstablishYear >= 0)}");
        }

        // FX5 capacity: full cohort rejects; over-capacity state contracts existing stock.
        EmptyWorld(0.9f);
        NextYear();
        {
            ForestRegenerationCohort full = e.Cells[0].GetOrCreateCohort(sitka);
            full.Restore(sitka.RegenDensityMax, 0.3f, 0);
            full.SeedRain = 100f;
            float pre = full.Density;
            Call("EstablishNewCohorts");
            Fact($"FX5A_FULL_REJECTION before={F(pre)} after={F(full.Density)} accepted={F(full.Density - pre)}");
            ForestRegenerationCohort a = e.Cells[1].GetOrCreateCohort(sitka), b = e.Cells[1].GetOrCreateCohort(beech);
            a.Restore(sitka.RegenDensityMax * 0.7f, 0.3f, 0); b.Restore(beech.RegenDensityMax * 0.7f, 0.3f, 0);
            float occ = e.Cells[1].SharedOccupancy, aPre = a.Density;
            e.Cells[1].AddDensityWithSharedCapacity(a, 0.01f);
            Fact($"FX5B_OVERCAPACITY_CONTRACTION occupancyBefore={F(occ)} requested=0.01 sitkaBefore={F(aPre)} sitkaAfter={F(a.Density)} change={F(a.Density - aPre)}");
        }

        // FX6 promotion compression: very different abundances each export to exactly one tree.
        EmptyWorld(1f);
        NextYear();
        {
            float[] densities = { 0.011f, sitka.RegenDensityMax * 0.5f, sitka.RegenDensityMax };
            for (int i = 0; i < densities.Length; i++)
                e.Cells[i].GetOrCreateCohort(sitka).Restore(densities[i], sitka.PromotionHeightM + 0.5f, 0);
            HashSet<string> before = new HashSet<string>(Living().Select(t => t.TreeId));
            Call("PromoteCohorts", e.ResolveSpecies(), SimulationRandom.Create(e.RngModelVersion, e.SimulationSeed, e.EcologicalYear, 0));
            ForestTree[] created = Living().Where(t => !before.Contains(t.TreeId)).ToArray();
            for (int i = 0; i < densities.Length; i++)
            {
                int trees = created.Count(t => e.GetCellIndex(t.transform.position) == i);
                float residual = e.Cells[i].FindCohort("sitka-spruce")?.Density ?? 0f;
                Fact($"FX6_PROMOTION_COMPRESSION cell={i} abundanceBefore={F(densities[i])} occupancyFraction={F(densities[i] / sitka.RegenDensityMax)} exactTrees={trees} residualAbundance={F(residual)} exported={F(densities[i] - residual)}");
            }
        }

        // FX6B planted-origin record keeps its origin after promotion and labels later natural recruits.
        EmptyWorld(1f);
        NextYear();
        {
            ForestRegenerationCohort c = e.Cells[0].GetOrCreateCohort(sitka);
            c.Restore(0.2f, sitka.PromotionHeightM + 0.5f, -3, RegenerationOrigin.Planted, 0);
            Call("PromoteCohorts", e.ResolveSpecies(), SimulationRandom.Create(e.RngModelVersion, e.SimulationSeed, e.EcologicalYear, 0));
            string afterPromotion = $"density={F(c.Density)} origin={c.Origin} originYear={c.OriginYear} establishYear={c.EstablishYear}";
            NextYear();
            c.SeedRain = 100f;
            Call("EstablishNewCohorts");
            Fact($"FX6B_ORIGIN_AFTER_PROMOTION afterPromotion[{afterPromotion}] afterNaturalArrival[density={F(c.Density)} origin={c.Origin} establishYear={c.EstablishYear} height={F(c.Height)}] defect={(c.Density > 0f && c.Origin == RegenerationOrigin.Planted)}");
        }

        // FX7 save capability: two same-species age bands in one cell through the real restore path.
        EmptyWorld(0.5f);
        {
            var saved = new List<ForestRegenerationCohortSaveData>
            {
                new ForestRegenerationCohortSaveData { speciesId = "sitka-spruce", density = 0.3f, height = 0.2f, establishYear = 9 },
                new ForestRegenerationCohortSaveData { speciesId = "sitka-spruce", density = 0.4f, height = 1.5f, establishYear = 2 },
            };
            e.RestoreCellState(0, saved, 0f, 1f);
            var restored = e.Cells[0].Regeneration.Where(r => r.SpeciesId == "sitka-spruce").ToList();
            Fact($"FX7_SAVE_TWO_BANDS savedRecords=2 restoredCohorts={restored.Count} restored=[{string.Join(";", restored.Select(r => $"density={F(r.Density)} height={F(r.Height)} establishYear={r.EstablishYear}"))}] bandsPreserved={restored.Count == 2}");
        }

        NaturalVersusPlanted(np, sitka, beech, oak);
    }

    // Same species, light and browse pressure: one natural cohort (expected
    // fractions, plus production infill) versus 1000 exact planted juveniles
    // through the production planted path (AdvancePlantedJuveniles, model 1).
    private void NaturalVersusPlanted(StreamWriter np, params TreeSpeciesDefinition[] species)
    {
        List<PlantedJuvenile> planted = ManagerJuveniles();
        if (planted == null) { Fact("NP_SKIPPED no ScenarioOneManager"); return; }
        float[] lights = { 0.03f, 0.15f, 0.5f, 0.9f };
        foreach (TreeSpeciesDefinition s in species)
            foreach (float light in lights)
            {
                EmptyWorld(light);
                const int cell = 27, n = 1000;
                ForestRegenerationCohort c = e.Cells[cell].GetOrCreateCohort(s);
                float start = s.RegenDensityMax * 0.2f;
                c.Restore(start, s.RegenInitialHeightM, 0);
                Vector2 centre = e.Cells[cell].Center;
                for (int i = 0; i < n; i++)
                {
                    float ox = ((i % 40) / 40f - 0.5f) * e.CellSizeMeters * 0.9f, oz = ((i / 40) / 25f - 0.5f) * e.CellSizeMeters * 0.9f;
                    planted.Add(new PlantedJuvenile { juvenileId = $"NP{s.SpeciesId}{i:0000}", speciesId = s.SpeciesId, cellIndex = cell,
                        position = new Vector3(centre.x + ox, 0f, centre.y + oz), heightMeters = s.RegenInitialHeightM, ageYears = 0f });
                }
                float noInfill = start;
                int cohortPromotedYear = -1;
                for (int y = 1; y <= 20; y++)
                {
                    NextYear();
                    foreach (ForestEcologyCell other in e.Cells) other.Light = light;
                    float pre = c.Density;
                    Call("GrowExistingRegeneration");
                    noInfill = (float)(noInfill * JuvenileEcologyRules.SurvivalResponse(s, light, c.LastBrowsedFraction));
                    typeof(ScenarioOneManager).GetMethod("AdvancePlantedJuveniles", Private).Invoke(manager, null);
                    if (cohortPromotedYear < 0 && c.Density > 0f && JuvenileEcologyRules.CanPromote(s, c.Height, light)) cohortPromotedYear = y;
                    var alive = planted.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)).ToList();
                    int promoted = planted.Count(j => !string.IsNullOrEmpty(j.promotedTreeId));
                    np.WriteLine(string.Join(",", s.SpeciesId, F(light), F(e.Browsing.BackgroundPressure), y,
                        F(c.Density / start), F(noInfill / start), F(c.Height), F(c.LastBrowsedFraction),
                        F(alive.Count / (float)n), F(promoted / (float)n), alive.Count > 0 ? F(alive.Average(j => j.heightMeters)) : "",
                        F(alive.Count(j => j.lastYearBrowsed) / Mathf.Max(1f, alive.Count))));
                }
                np.Flush();
                planted.Clear();
            }
    }

    // ---------- clearance / understorey causal trace ----------

    private IEnumerator ClearanceTrace(StreamWriter ct)
    {
        if (manager == null) { Fact("CT_SKIPPED no ScenarioOneManager"); yield break; }
        const int warmup = 15, follow = 10;
        int selected = -1;
        string startHash = null;
        foreach (string branch in new[] { "control", "clear_cell", "cover_only" })
        {
            Build(0.6f);
            for (int y = 0; y < warmup; y++) { e.AdvanceOneYear(); if (y % 5 == 0) yield return null; }
            string hash = Hash(State());
            if (startHash == null)
            {
                startHash = hash;
                float best = 0f;
                for (int i = 0; i < e.Cells.Length; i++)
                {
                    float d = e.Cells[i].Regeneration.Sum(r => r.Density);
                    if (d > best) { best = d; selected = i; }
                }
                Fact($"CT_START year={e.EcologicalYear} selectedCell={selected} density={F(best)} stateHash={hash}");
            }
            Check(hash == startHash, "CT branch start states differ");
            if (branch == "clear_cell")
            {
                ClearanceTargets targets = manager.QueryClearance(ClearanceFootprint.Cell(e, selected), e.EcologicalYear);
                float occupancyBefore = e.Cells[selected].SharedOccupancy;
                typeof(ScenarioOneManager).GetMethod("ApplyClearance", Private).Invoke(manager, new object[] { targets, e.EcologicalYear });
                Fact($"CT_CLEARANCE cell={selected} removedRelativeDensity={F(targets.Density)} cohorts={targets.Cohorts.Count} plantedKilled={targets.Juveniles.Count} understoreyCellsReset={targets.Understorey.Count} occupancyBefore={F(occupancyBefore)} occupancyAfter={F(e.Cells[selected].SharedOccupancy)}");
            }
            else if (branch == "cover_only")
            {
                int changed = 0;
                foreach (ScenarioUnderstoreyCell u in manager.UnderstoreyCells)
                {
                    if (u == null) continue;
                    u.ferns = u.grasses = u.forbs = u.shrubs = 0f; changed++;
                }
                Fact($"CT_COVER_ONLY understoreyCellsZeroed={changed} cohortsUntouched=true");
            }
            var annual = new StringBuilder();
            for (int y = 1; y <= follow; y++)
            {
                Dictionary<string, Row> rows = TraceYear("ct_" + branch);
                foreach (Row r in rows.Values.Where(r => r.Cell == selected))
                    ct.WriteLine(LedgerLine(r));
            }
            string endRegen = Hash(string.Join("\n", e.Cells.Select(c => string.Join("|", c.Regeneration.Select(r => r.SpeciesId + ":" + F(r.Density) + ":" + F(r.Height) + ":" + r.EstablishYear)))));
            Fact($"CT_END branch={branch} year={e.EcologicalYear} selectedCellDensity={F(e.Cells[selected].Regeneration.Sum(r => r.Density))} regenerationHash={endRegen}");
            ct.Flush();
            yield return null;
        }
    }

    // ---------- driver ----------

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        stand = FindFirstObjectByType<ForestStartingStand>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        dir = Environment.GetEnvironmentVariable("CCF_DIAG_DIR") ?? Path.Combine(Application.temporaryCachePath, "regeneration-budget");
        years = int.TryParse(Environment.GetEnvironmentVariable("CCF_REGEN_YEARS"), out int parsed) ? parsed : 100;
        Directory.CreateDirectory(dir);
        Check(e != null && stand != null && spawner != null, "scene systems");
        using (facts = new StreamWriter(Path.Combine(dir, "facts.txt")))
        using (var np = new StreamWriter(Path.Combine(dir, "natural_vs_planted.csv")))
        using (var ledger = new StreamWriter(Path.Combine(dir, "ledger.csv")))
        using (var funnels = new StreamWriter(Path.Combine(dir, "funnels.csv")))
        using (var hashes = new StreamWriter(Path.Combine(dir, "state_hashes.csv")))
        using (var ct = new StreamWriter(Path.Combine(dir, "clearance_trace.csv")))
        {
            Fact($"ENV engine={Application.unityVersion} years={years} managerPresent={manager != null}");
            np.WriteLine("species,light,browse_pressure,year,cohort_relative_abundance,cohort_without_infill_counterfactual,cohort_height,cohort_browsed_fraction,planted_alive_fraction,planted_promoted_fraction,planted_mean_height,planted_browsed_fraction");
            Fixtures(np);
            yield return null;
            ledger.WriteLine(LedgerHeader);
            funnels.WriteLine("treatment,horizon,species,seed_arrival_relative_sum,establishment_requested,establishment_accepted,establishment_rejected,establishment_events,rejection_events,light_loss,browse_loss,infill_accepted,infill_unsourced,capacity_contraction,threshold_loss,exported_relative,exact_trees_created,merges_into_older,mean_inherited_age,age_initialised_zero_accepted,standing_relative_abundance,occupied_cells,recruited_trees_alive,living_trees,max_identity_residual");
            hashes.WriteLine("treatment,pipeline,year,state_hash");
            foreach ((string label, float removal) in new[] { ("unthinned", 0f), ("remove_smallest_20", 0.2f), ("remove_smallest_60", 0.6f) })
                yield return RunTreatment(label, removal, ledger, funnels, hashes);
            ct.WriteLine(LedgerHeader);
            yield return ClearanceTrace(ct);
            Fact(failures.Count == 0 ? "REGEN_BUDGET_EVIDENCE_PASS" : "REGEN_BUDGET_EVIDENCE_FAIL " + string.Join("; ", failures));
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
        if (failure != null) Debug.LogError("REGEN_BUDGET_EVIDENCE_FAIL " + failure);
        else if (failures.Count > 0) Debug.LogError("REGEN_BUDGET_EVIDENCE_FAIL checks=" + failures.Count);
        Debug.Log("REGEN_BUDGET_DONE");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null && failures.Count == 0 ? 0 : 1);
#endif
    }
}

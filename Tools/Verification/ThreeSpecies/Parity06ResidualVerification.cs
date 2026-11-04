#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable PARITY-06 residual checks. Run alone, with an isolated config and
// explicit geometry; never saves an asset, scene, or player save slot.
public static class Parity06ResidualVerification
{
    public static void Begin()
    {
        const string path = "Assets/ForestPrototype/ScenarioOne/ScenarioOne.asset";
        byte[] before = File.ReadAllBytes(path);
        var warnings = new List<string>();
        Application.LogCallback capture = (message, trace, type) =>
        {
            if (type == LogType.Warning || type == LogType.Error || type == LogType.Exception)
                warnings.Add(message);
        };
        Application.logMessageReceived += capture;
        ScenarioOneDefinition definition;
        try { definition = AssetDatabase.LoadAssetAtPath<ScenarioOneDefinition>(path); }
        finally { Application.logMessageReceived -= capture; }
        if (definition == null || warnings.Count != 0 || !before.SequenceEqual(File.ReadAllBytes(path)))
            throw new InvalidOperationException("ScenarioOne.asset load warnings/byte change: " + string.Join(";", warnings));
        Debug.Log("PARITY06_ASSET_LOAD_PASS warnings=0 bytesUnchanged=True");
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("Parity06ResidualVerification.Begin"))
            new GameObject("PARITY-06 residual gate").AddComponent<Parity06ResidualGate>();
    }
}

public sealed class Parity06ResidualGate : MonoBehaviour
{
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeSpawner spawner;
    private ScenarioOneManager manager;
    private ForestSaveData original;
    private int checks;

    private void Check(bool ok, string message)
    {
        checks++;
        Debug.Log("PARITY06_RESIDUAL_" + (ok ? "PASS " : "FAIL ") + message);
        if (!ok) throw new InvalidOperationException(message + " / " + manager.Feedback);
    }
    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
    private ForestTree Tree(string id) => FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Single(t => t.TreeId == id && t.IsLiving);
    private ForestTree Spawn(string id, string species) => spawner.Spawn(id, spawner.ResolveSpecies(species), new Vector3(1, 0, 1), 110, 60, 24, 5);

    private IEnumerator Start()
    {
        yield return null;
        saves = FindFirstObjectByType<ForestSaveController>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        original = saves.CaptureData();
        string hash = ScenarioReferenceArchive.WorldHash(original);
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object next = null;
            try { more = stack.Peek().MoveNext(); if (more) next = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (next is IEnumerator nested) stack.Push(nested); else yield return next;
        }
        if (!saves.LoadData(original, false)) failure = failure ?? new Exception("original restore failed");
        yield return null; yield return null;
        if (ScenarioReferenceArchive.WorldHash(saves.CaptureData()) != hash) failure = failure ?? new Exception("original hash changed");
        if (failure == null) Debug.Log("PARITY06_RESIDUAL_VERIFY_PASS checks=" + checks + " worldRestored=True noSaveSlotWrites=True");
        else Debug.LogError("PARITY06_RESIDUAL_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Reset(int model = GrowthModel.SpeciesAware)
    {
        Check(saves.LoadData(Clone(original), false), "reset baseline");
        yield return null; yield return null;
        manager.InitializeNewScenario(); ecology.GrowthModelVersion = model;
    }

    private void Visual(ForestTree tree, string expected)
    {
        Check(tree.ActiveVisualSource.Contains(expected), tree.TreeId + " visual=" + tree.ActiveVisualSource + " expected=" + expected);
        Check(tree.transform.Cast<Transform>().Count(t => t.name == "PolishedVisual" && t.gameObject.activeInHierarchy) == 1
            && tree.transform.Find("PolishedVisual").GetComponentsInChildren<Renderer>().Any(r => r.enabled), "one active rendered pruning model");
    }

    private IEnumerator Verify()
    {
        long sitkaRevenue = -1, cost = -1;
        string prices = null;
        foreach (int model in new[] { GrowthModel.Legacy, GrowthModel.SiteClassDensity, GrowthModel.SpeciesAware })
        {
            yield return Reset(model);
            var marking = FindFirstObjectByType<ForestTreeMarkingManager>();
            foreach (string species in new[] { "sitka-spruce", "sessile-oak", "beech" })
                marking.Mark(Spawn("P06-PRICE-" + species, species), TreeMarkType.Fell, false);
            manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
            Check(manager.AddMarkedTreesToWorkPlan() == 3, "price fixture orders model=" + model);
            var job = manager.GetHarvestQuote();
            var sitkaTimber = job.Resolution.Quote.Timber.Where(v => v.Batch.SpeciesId == "sitka-spruce");
            long revenue = sitkaTimber.Sum(v => v.SaleRevenueCents);
            string currentPrices = string.Join(";", sitkaTimber.Select(v => v.Batch.Assortment + ":" + v.SelectedRoadsideCentsPerTonne));
            if (model == GrowthModel.Legacy) { sitkaRevenue = revenue; prices = currentPrices; cost = job.CostCents; }
            Check(revenue > 0 && revenue == sitkaRevenue && prices == currentPrices && job.CostCents == cost,
                "Sitka revenue/prices and generic job cost exact across models 0/1/2 model=" + model);
            Check(model == GrowthModel.SpeciesAware ? !job.HasUnmarketedSpecies : job.HasUnmarketedSpecies
                && job.Resolution.Quote.Timber.Where(v => v.Batch.SpeciesId != "sitka-spruce").Sum(v => v.SaleRevenueCents) == 0,
                "market behaviour model=" + model);
        }

        // Explicit compatibility for all three Sitka lifts with the model-2 gate on.
        yield return Reset();
        var sitka = Spawn("P06-SITKA", "sitka-spruce");
        Check(sitka.TryPrune(2.5f, 0, true) == null && sitka.CanPrune(5, 4, true) != null
            && sitka.TryPrune(5, 5, true) == null && sitka.TryPrune(6.5f, 10, true) == null
            && sitka.TryPrune(7, 15, true) != null, "Sitka three lifts/five-year interval unchanged with gate on");

        foreach (string species in new[] { "sessile-oak", "beech" })
        {
            // A genuine old pending order is captured on model 1 and then loaded
            // as model 2, reproducing the obsolete target without bypassing validation.
            yield return Reset(GrowthModel.SiteClassDensity);
            var tree = Spawn("P06-OLD", species);
            Check(manager.TryDesignatePruning(tree.TreeId), "legacy pending designation");
            var pending = saves.CaptureData(); pending.growthModel = GrowthModel.SpeciesAware;
            Check(pending.scenarioOne.workOrders.Single().targetCrownBaseHeightM == 2.5f, "obsolete first target fixture");
            Check(saves.LoadData(Clone(pending), false), "obsolete pending save loads");
            yield return null; yield return null;
            long cash = manager.CashCents;
            Check(!manager.ApprovePendingWork() && !string.IsNullOrEmpty(manager.WorkOrders.Single().validationMessage), "obsolete pending target rejected by normal approval");
            Check(manager.CashCents == cash && manager.AdvanceYear() && manager.CashCents == cash && Tree("P06-OLD").PruningLifts == 0,
                "ineligible pending work charges nothing and changes no pruning history");
            // Previously approved old orders must also fail safely at resolution.
            pending.scenarioOne.workOrders.Single().status = ScenarioWorkStatus.Approved;
            Check(saves.LoadData(Clone(pending), false), "obsolete approved save loads");
            yield return null; yield return null;
            cash = manager.CashCents;
            Check(manager.AdvanceYear() && manager.CashCents == cash && manager.WorkOrders.Single().status == ScenarioWorkStatus.Failed
                && Tree("P06-OLD").PruningLifts == 0, "obsolete approved resolution fails without charging");

            // Preserve old one/two/three-lift history exactly, including crowns
            // that follow the former schedule; do not silently migrate them.
            foreach (int lifts in new[] { 1, 2, 3 })
            {
                yield return Reset(); tree = Spawn("P06-HISTORY", species);
                float crownBase = lifts == 1 ? 2.5f : lifts == 2 ? 5f : 6.5f;
                tree.RestorePruningHistory(lifts, crownBase, 0);
                ForestSaveData history = saves.CaptureData();
                Check(saves.LoadData(Clone(history), false), "historical lifts=" + lifts + " load");
                yield return null; yield return null;
                tree = Tree("P06-HISTORY");
                Check(tree.PruningLifts == lifts && tree.CrownBaseHeightM == crownBase && tree.LastPruningYear == 0
                    && tree.CrownRadius == 5, "history/crown/year/radius exact " + species + " lifts=" + lifts);
                Check(manager.Definition.NextPruningTargetHeightM(tree, true) == (lifts == 1 ? 6f : -1f), "one lift proceeds to 6 m; two/three lifts capped");
                Check(tree.CanPrune(6, 3, true) == (lifts == 1 ? null : $"Already pruned {lifts} times (maximum 2)."), "restored history eligibility");
            }

            yield return Reset(); tree = Spawn("P06-VISUAL", species);
            Visual(tree, "Unpruned");
            Check(tree.TryPrune(3.5f, 0, true) == null && Mathf.Abs(tree.CrownRadius - 4.6f) < 0.00001f, "first lift real radius loss");
            Visual(tree, "LowPruned_Recent");
            for (int i = 0; i < 3; i++) { ecology.AdvanceOneYear(); yield return null; }
            float before = tree.CrownRadius;
            Check(tree.TryPrune(6, 3, true) == null && Mathf.Abs(tree.CrownRadius - before * .92f) < .00001f, "second lift real radius loss");
            Visual(tree, "MediumPruned_Recent");
            before = tree.CrownRadius;
            ecology.AdvanceOneYear();
            var index = (Dictionary<ForestTree, float>)typeof(ForestEcologyController)
                .GetField("competitionIndex", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(ecology);
            float ci = index.TryGetValue(tree, out float value) ? value : 0;
            float target = tree.Species.PotentialCrownRadiusM(tree.Diameter) / (1 + ci / tree.Species.Ci50) * .92f * .92f;
            float expected = Mathf.Max(.1f, Mathf.Lerp(before, target, tree.Species.CrownRelaxationPerYear));
            Check(tree.IsLiving && Mathf.Abs(tree.CrownRadius - expected) < .00001f && Mathf.Abs(tree.PrunedCrownTargetFactor - .8464f) < .00001f,
                "real annual ecology relaxation retains two-lift crown factor " + species + " expected=" + expected + " actual=" + tree.CrownRadius);
            Visual(tree, "MediumPruned_Recent");
            Check(!manager.TryDesignatePruning(tree.TreeId), "visual/ecology refresh does not unlock a third lift");
        }
    }
}
#endif

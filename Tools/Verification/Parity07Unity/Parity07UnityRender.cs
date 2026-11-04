#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

// Disposable rendered qualification for PARITY-07 player-facing species truth copy.
//
// Opens Assets/Scenes/ForestTest.unity, starts a new scenario through the real startup menu, closes first-use help, then
// drives the real ScenarioOneUiRoot / TreeInspectionView / WorkPlanView / LearningObjectivesView at 1280x720 and 1920x1080.
// Every asserted string is read back from the rendered UI Toolkit labels (no synthetic text), every state is captured as a
// PNG and every panel is audited for horizontal containment, vertical containment (fixed panels) and clipped text.
//
// Fixtures: extra Oak/Beech/Sitka trees are spawned in memory through ForestTreeSpawner and pruned through the production
// ForestTree.TryPrune; nothing is written to the player's save, and the world is restored from an in-memory snapshot and
// its hash compared at the end.
//
// Install by copying this file into Assets/ForestPrototype/ (run_gate.py does this), run, then remove it with its .meta.
// The runtime installer only acts for the exact -executeMethod argument, so it can never auto-start in a build.
//
// LIMITATION: callback/API driven through the real UI state and renderer, not OS-native keyboard or mouse input.
public static class Parity07Unity
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Parity07UnityRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "Parity07Unity.Begin")) return;
        new GameObject("PARITY-07 rendered qualification").AddComponent<Parity07UnityRunner>();
    }
}

public sealed class Parity07UnityRunner : MonoBehaviour
{
    private const string Limitation = "Callback/API driven: no OS-native keyboard, mouse click or hover input was delivered.";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly Vector2Int[] RequiredSizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };

    // Literal expected copy. Deliberately not read from ScenarioOneUiFacts, so a regression in the constants is caught.
    private const string QualityLimit = "Oak and Beech grade here depends on size and pruning history in this version; fork and defect shape is not modelled.";
    private const string PremiumLead = "Premium Oak is only a possible late outcome: it needs a large, pruned, favourably grown tree with little competition, and it is not promised or required.";
    private const string Thresholds = "Size thresholds are about 45 cm mid-log diameter for veneer/planking and 30 cm for beams.";
    private const string NotPruned = "Not pruned: the clear first-grade veneer/planking class is unavailable.";
    private const string BeechClass = "Beech here is graded to second-quality planking or firewood; it has no veneer, beam or fencing class.";
    private const string SharedNote = "Site suitability, canopy shading and wind exposure use the same simplified rules for all three species. Site and wind responses are not calibrated per species; canopy shading still reflects species-specific crown size and crown adjustment.";
    private const string StormsOff = "Storm events are off in this forest.";
    private const string StormsOn = "Storm events are a simplified shared model, not a species wind-tolerance band.";
    private const string NoMarket = "No timber market is configured for this species in Scenario One.";
    private const string WorkPlanPruneOak = "Where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade; Beech has no such class. Fork and defect shape is not modelled.";
    private const string WorkPlanPruneLead = "Pruning is a retained-tree timber-quality treatment and sets no price by itself.";
    private const string TopicOak = "where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade, while Sitka and Beech have no such premium here, and fork or defect shape is not modelled";
    private const string StepLifts = "Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart). When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above.";
    private const string VeneerLabel = "First-grade veneer / planking:";
    private const string BeamLabel = "Beam:";

    // Wording that would claim measured form or a guaranteed premium (FR-9 / review N2).
    private static readonly Regex[] Prohibited =
    {
        new Regex(@"straight enough", RegexOptions.IgnoreCase), new Regex(@"large,\s*straight", RegexOptions.IgnoreCase),
        new Regex(@"size and form", RegexOptions.IgnoreCase), new Regex(@"\bgood form\b|\bwell[- ]formed\b|\bstraight stems?\b", RegexOptions.IgnoreCase),
        new Regex(@"guarantee[ds]?\b[^"".]{0,40}(?:premium|veneer|oak)|(?:premium|veneer|oak)[^"".]{0,40}\bguarantee|will (?:reach|become) (?:premium|veneer)", RegexOptions.IgnoreCase),
        new Regex(@"applies no timber-price premium for pruning", RegexOptions.IgnoreCase),
    };

    [Serializable] public sealed class CaptureRecord { public string name, path, textPath; public int width, height; }

    [Serializable] public sealed class Audit
    {
        public string harness = "Parity07UnityRunner";
        public string limitation = Limitation;
        public string verdict;
        public bool pass;
        public int failures, passes, shots;
        public string hashBefore, hashAfter;
        public List<CaptureRecord> captures = new List<CaptureRecord>();
        public List<string> notes = new List<string>();
        public List<string> errors = new List<string>();
    }

    private string output;
    private Vector2Int size;
    private int shots, passes, failures;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private StandaloneSessionMenu menu;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeSpawner spawner;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private ForestSaveData original;
    private string hashBefore, hashAfter;
    private int fixtureCounter;
    private readonly List<string> notes = new List<string>();
    private readonly List<string> errors = new List<string>();
    private readonly List<CaptureRecord> captures = new List<CaptureRecord>();

    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object gameSize = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "PARITY-07 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        PropertyInfo gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show();
        view.maximized = true;
        view.Focus(); view.Repaint();
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "parity07-unity");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "texts"));
        Application.logMessageReceived += OnLogMessage;
        CheckBlankClassifier();
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Play());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }

        // Restore the in-memory world whether or not a state failed, then prove nothing leaked into it.
        if (saves != null && original != null)
        {
            ClearInspection();
            if (ui != null) { ui.CloseHelp(); ui.CloseAll(); }
            bool restored = false;
            try { restored = saves.LoadData(JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(original)), false); }
            catch (Exception error) { failure = failure ?? error; }
            yield return null; yield return null;
            try
            {
                hashAfter = WorldHash();
                Check(restored && hashAfter == hashBefore, "WORLD_RESTORED_AFTER_FIXTURES", "before=" + hashBefore + " after=" + hashAfter);
            }
            catch (Exception error) { failure = failure ?? error; }
        }
        if (errors.Count > 0)
        {
            failures++;
            Debug.Log("PARITY07_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4)));
        }
        bool pass = failure == null && failures == 0;
        Application.logMessageReceived -= OnLogMessage;
        WriteReports(pass, failure);
        Debug.Log(pass
            ? $"PARITY07_UNITY_HARNESS_PASS shots={shots} passes={passes} failures=0"
            : $"PARITY07_UNITY_HARNESS_FAIL shots={shots} passes={passes} failures={failures} exception={failure}");
        Debug.Log("PARITY07_LIMITATION " + Limitation);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(pass ? 0 : 1);
    }

    private IEnumerator Play()
    {
        for (int frame = 0; frame < 900 && (menu == null || !StandaloneSessionMenu.IsOpen); frame++)
        {
            if (menu == null) menu = UnityEngine.Object.FindFirstObjectByType<StandaloneSessionMenu>();
            yield return null;
        }
        if (menu == null || !StandaloneSessionMenu.IsOpen) throw new InvalidOperationException("the startup screen did not open");
        for (int frame = 0; frame < 180 && !Application.isFocused; frame++) yield return null;
        InvokePrivate(menu, "StartupCard");
        yield return Settle(8);
        menu.StartNewScenario();
        for (int frame = 0; frame < 600 && (ui == null || ui.Help == null || !ui.Help.IsOpen); frame++)
        {
            if (ui == null) ui = UnityEngine.Object.FindFirstObjectByType<ScenarioOneUiRoot>();
            yield return null;
        }
        if (ui == null || ui.Help == null || !ui.Help.IsOpen) throw new InvalidOperationException("Scenario One UI/first-use help missing after Start New");
        ui.CloseHelp();
        yield return Settle(20);
        player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        ecology = ui.Ecology;
        spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        if (player == null || saves == null || ecology == null || spawner == null || ui.Manager == null)
            throw new InvalidOperationException("player, save controller, ecology, spawner or manager missing");
        startPosition = player.transform.position;
        startRotation = player.transform.rotation;
        original = saves.CaptureData();
        hashBefore = WorldHash();
        notes.Add(Limitation);
        notes.Add("World hash before fixtures: " + hashBefore);
        notes.Add($"New-world models at start: growth={ecology.GrowthModelVersion} storm={ecology.StormModelVersion} broadleafMarket={ui.Manager.BroadleafMarketActive}");

        // The harness states the models it relies on instead of assuming what Start New produced.
        ecology.GrowthModelVersion = GrowthModel.SpeciesAware;
        ecology.StormModelVersion = StormModel.None;
        ResetInspectionCache();
        Check(ui.Manager.BroadleafMarketActive, "FIXTURE_BROADLEAF_MARKET_ACTIVE_FOR_NEW_WORLD");

        ForestTree oakUnpruned = Fixture("sessile-oak", 22f, 15f, 0);
        ForestTree oakLargeUnpruned = Fixture("sessile-oak", 60f, 29f, 1);
        ForestTree oakPrunedSmall = Fixture("sessile-oak", 22f, 15f, 2);
        PruneProduction(oakPrunedSmall);
        ForestTree beech = Fixture("beech", 30f, 20f, 3);
        ForestTree sitka = Fixture("sitka-spruce", 30f, 20f, 4);
        ForestTree oakPrunedLarge = null;
        foreach (float dbh in new[] { 50f, 60f, 70f, 80f, 90f })
        {
            ForestTree candidate = Fixture("sessile-oak", dbh, Mathf.Min(34f, 18f + dbh * 0.2f), 5);
            PruneProduction(candidate);
            oakPrunedLarge = candidate;
            if (HasPremiumProduct(EstimateLines(candidate, true))) break;
        }
        yield return Settle(10);
        Check(oakUnpruned != null && !HasPremiumProduct(EstimateLines(oakUnpruned, true)), "FIXTURE_SMALL_UNPRUNED_OAK_HAS_NO_PREMIUM_PRODUCT");
        Check(!HasPremiumProduct(EstimateLines(oakPrunedSmall, true)), "FIXTURE_SMALL_PRUNED_OAK_HAS_NO_PREMIUM_PRODUCT");
        Check(HasPremiumProduct(EstimateLines(oakPrunedLarge, true)), "FIXTURE_LARGE_PRUNED_OAK_REACHES_VENEER_OR_BEAM",
            "dbh=" + oakPrunedLarge.Diameter.ToString("0", Inv) + " lines=" + string.Join(" | ", EstimateLines(oakPrunedLarge, true)));

        // 1. Unpruned Oak (below thresholds) and unpruned large Oak.
        yield return InspectionState("oak-unpruned", oakUnpruned, false, c => OakEstimate(c, oakUnpruned, "Not pruned"));
        yield return InspectionState("oak-unpruned-large", oakLargeUnpruned, false, c => OakEstimate(c, oakLargeUnpruned, "Not pruned"));
        // 2. Pruned Oak below and above the size thresholds.
        yield return InspectionState("oak-pruned-below-thresholds", oakPrunedSmall, false, c => OakEstimate(c, oakPrunedSmall, "Pruned to 3.5 m"));
        yield return InspectionState("oak-pruned-above-thresholds", oakPrunedLarge, false, c => OakEstimate(c, oakPrunedLarge, "Pruned to 3.5 m"));
        // 3. Beech.
        yield return InspectionState("beech", beech, false, c =>
        {
            Expect(c.estimate, QualityLimit, "BEECH_QUALITY_LIMIT");
            Expect(c.estimate, BeechClass, "BEECH_NO_VENEER_BEAM_FENCING_CLASS");
            Forbid(c.estimate, "Premium Oak", "BEECH_NO_PREMIUM_OAK_NOTE");
            Forbid(c.estimate, "Not pruned", "BEECH_NO_OAK_PRUNING_LINE");
        });
        yield return InspectionState("sitka", sitka, false, c =>
        {
            Forbid(c.estimate, QualityLimit, "SITKA_NO_BROADLEAF_QUALITY_LIMIT");
            Forbid(c.estimate, "Premium Oak", "SITKA_NO_PREMIUM_OAK_NOTE");
        });

        // 4. More detail [V]: storms off / on for Oak and Beech, Sitka (note is broadleaf-only in production), old growth model.
        foreach ((string label, int storm) in new[] { ("storms-off", StormModel.None), ("storms-on", StormModel.Latest) })
        {
            ecology.StormModelVersion = storm;
            ResetInspectionCache();
            string expectedTail = storm == StormModel.None ? StormsOff : StormsOn;
            string otherTail = storm == StormModel.None ? StormsOn : StormsOff;
            foreach ((string speciesName, ForestTree tree) in new[] { ("oak", oakUnpruned), ("beech", beech) })
            {
                yield return InspectionState($"detail-{speciesName}-{label}", tree, true, c =>
                {
                    Expect(c.detail, SharedNote + " " + expectedTail, "SHARED_NOTE_AND_STORM_TAIL_" + speciesName + "_" + label);
                    Expect(c.detail, "Site and wind responses are not calibrated per species", "SITE_WIND_NOT_CALIBRATED_" + speciesName + "_" + label);
                    Expect(c.detail, "canopy shading still reflects species-specific crown size and crown adjustment", "CANOPY_SHADING_SPECIES_SPECIFIC_" + speciesName + "_" + label);
                    Forbid(c.detail, otherTail, "WRONG_STORM_TAIL_ABSENT_" + speciesName + "_" + label);
                    Forbid(c.detail, "they are not calibrated per species", "OLD_OVERBROAD_NOTE_ABSENT_" + speciesName + "_" + label);
                });
            }
            yield return InspectionState($"detail-sitka-{label}", sitka, true, c =>
            {
                Forbid(c.detail, "Site suitability, canopy shading and wind exposure use the same simplified rules", "SITKA_HAS_NO_SHARED_BROADLEAF_NOTE_" + label);
                Expect(c.detail, "More detail", "SITKA_DETAIL_CARD_OPEN_" + label);
            });
        }
        // Older forest (growth model before species-aware): no Oak/Beech timber market, same shared-rules note.
        ecology.StormModelVersion = StormModel.None;
        ecology.GrowthModelVersion = GrowthModel.SiteClassDensity;
        ResetInspectionCache();
        Check(!ui.Manager.BroadleafMarketActive, "OLD_GROWTH_MODEL_HAS_NO_BROADLEAF_MARKET");
        yield return InspectionState("detail-oak-old-growth-model", oakUnpruned, true, c =>
        {
            Expect(c.estimate, NoMarket, "OLD_MODEL_OAK_NO_TIMBER_MARKET");
            Forbid(c.estimate, "Premium Oak", "OLD_MODEL_NO_PREMIUM_OAK_NOTE");
            Expect(c.detail, SharedNote + " " + StormsOff, "OLD_MODEL_SHARED_NOTE");
        });
        ecology.GrowthModelVersion = GrowthModel.SpeciesAware;
        ResetInspectionCache();
        SetDetail(false);

        // 5. Work Plan: harvest card (broadleaf market active, then an older forest) and Pruning card.
        yield return WorkPlanStates();

        // 6. Learning objectives -> 6. Pruning topic and prune.read.
        yield return LearningStates();

        CleanupPresentation();
        yield return Settle(10);
    }

    // ---------------------------------------------------------------- inspection

    private struct Seen { public string card, detail, estimate, all; }

    private IEnumerator InspectionState(string name, ForestTree tree, bool detail, Action<Seen> checks)
    {
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ResetInspectionCache();
            SetDetail(detail);
            yield return Inspect(tree);
            CloseAutoHelp();
            yield return Settle(8);
            Check(player.InspectedTree == tree, "TREE_SELECTED_" + name);
            VisualElement card = ui.Inspection.Root.Q("inspect-card"), detailPanel = ui.Inspection.Root.Q("inspect-detail");
            var c = new Seen
            {
                card = TextOf(card), detail = detail ? TextOf(detailPanel) : "",
                estimate = ui.Inspection.ShowsEstimate ? TextOf(ui.Inspection.EstimatePanel) : "",
            };
            c.all = c.card + "\n" + c.detail + "\n" + c.estimate;
            Check(Visible(card), "INSPECT_CARD_VISIBLE_" + name);
            Check(ui.Inspection.ShowsEstimate, "FELLING_ESTIMATE_PANEL_SHOWN_" + name);
            if (detail) Check(Visible(detailPanel), "DETAIL_CARD_VISIBLE_" + name); else Check(!ui.Inspection.ShowsDetail, "DETAIL_CARD_CLOSED_" + name);
            Check(string.IsNullOrEmpty(Regex.Match(c.all, @"\bNaN\b|\bInfinity\b").Value), "NO_NAN_OR_INFINITY_" + name);
            checks(c);
            ForbidProhibited(c.all, name);
            yield return Shot(name, c.all, card, detail ? detailPanel : null, ui.Inspection.ShowsEstimate ? ui.Inspection.EstimatePanel : null);
            // The two stacked left-column cards must also fit the screen (the cursor is locked, so they cannot scroll).
            VisualElement bottom = detail ? detailPanel : card;
            Check(bottom.worldBound.yMax <= ui.Inspection.Root.worldBound.yMax + 2f, "LEFT_COLUMN_FITS_SCREEN_" + name,
                $"bottom={bottom.worldBound.yMax.ToString("0", Inv)} column={ui.Inspection.Root.worldBound} root={ActiveRoot().worldBound}");
            VisualElement prompt = ActiveRoot().Q(className: "hud-prompt")?.Q(className: "hud-prompt-box");
            Check(!Visible(prompt) || !prompt.worldBound.Overlaps(ui.Inspection.Root.worldBound), "INSPECTION_CLEARS_CENTRAL_PROMPT_" + name,
                $"prompt={prompt?.worldBound} column={ui.Inspection.Root.worldBound}");
            Check(!Visible(prompt) || !Visible(ui.Inspection.EstimatePanel) || !prompt.worldBound.Overlaps(ui.Inspection.EstimatePanel.worldBound), "ESTIMATE_CLEARS_CENTRAL_PROMPT_" + name,
                $"prompt={prompt?.worldBound} estimate={ui.Inspection.EstimatePanel.worldBound}");
            ClearInspection();
            yield return Settle(6);
        }
    }

    private void OakEstimate(Seen c, ForestTree tree, string pruningLine)
    {
        Expect(c.estimate, QualityLimit, "OAK_QUALITY_LIMIT");
        Expect(c.estimate, "Pruning changes which grades a log can reach; it does not set a price.", "OAK_PRUNING_DOES_NOT_SET_PRICE");
        if (pruningLine == "Not pruned")
            Expect(c.estimate, NotPruned, "OAK_NOT_PRUNED_LINE");
        else
            Expect(c.estimate, "Pruned to " + tree.CrownBaseHeightM.ToString("0.0", Inv) + " m: logs inside that section can become eligible for first-grade veneer/planking once they are large enough.", "OAK_PRUNED_ELIGIBLE_LINE");
        bool product = c.estimate.Contains(VeneerLabel) || c.estimate.Contains(BeamLabel);
        if (product)
        {
            Forbid(c.estimate, "Premium Oak is only a possible late outcome", "PREMIUM_NOTE_ABSENT_WHEN_VENEER_OR_BEAM_PRESENT");
            Forbid(c.estimate, Thresholds, "THRESHOLD_LINE_ABSENT_WHEN_VENEER_OR_BEAM_PRESENT");
        }
        else
        {
            Expect(c.estimate, Thresholds, "OAK_SIZE_THRESHOLDS_LINE");
            Expect(c.estimate, PremiumLead, "PREMIUM_OAK_NOTE_WHEN_NO_VENEER_OR_BEAM");
        }
        notes.Add($"{size.x}x{size.y} oak estimate tree={tree.TreeId} dbh={tree.Diameter.ToString("0.0", Inv)} product={product}");
    }

    private static IEnumerable<string> EstimateLines(ForestTree tree, bool market)
    {
        MethodInfo estimate = typeof(TreeInspectionView).GetMethod("TimberEstimate", BindingFlags.NonPublic | BindingFlags.Static);
        if (estimate == null) throw new MissingMethodException("TreeInspectionView.TimberEstimate");
        return ((IEnumerable<string>)estimate.Invoke(null, new object[] { tree, market })).ToList();
    }

    private static bool HasPremiumProduct(IEnumerable<string> lines)
        => lines.Any(l => l.StartsWith(VeneerLabel, StringComparison.Ordinal) || l.StartsWith(BeamLabel, StringComparison.Ordinal));

    private ForestTree Fixture(string speciesId, float dbh, float height, int slot)
    {
        fixtureCounter++;
        Vector3 at = startPosition + new Vector3(10f + 3f * (slot % 6), 0f, 10f + 3f * (fixtureCounter % 4));
        float ground = Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(at) + Terrain.activeTerrain.transform.position.y : at.y;
        at.y = ground;
        TreeSpeciesDefinition species = spawner.ResolveSpecies(speciesId);
        if (species == null) throw new InvalidOperationException("species not resolvable: " + speciesId);
        ForestTree tree = spawner.Spawn("P07-FIX-" + fixtureCounter.ToString("00", Inv) + "-" + speciesId, species, at, 70, dbh, height, 4f);
        if (tree == null) throw new InvalidOperationException("fixture spawn failed: " + speciesId);
        return tree;
    }

    // Production pruning path (ForestTree.TryPrune, same rule the Work Plan executes), in memory only.
    private void PruneProduction(ForestTree tree)
    {
        string problem = tree.TryPrune(3.5f, ecology.EcologicalYear, ui.Manager.BroadleafMarketActive);
        Check(problem == null, "FIXTURE_PRODUCTION_PRUNE_" + tree.TreeId, problem ?? "ok");
    }

    private void SetDetail(bool wanted)
    {
        if (ui.Inspection.ShowsDetail != wanted) ui.Inspection.ToggleDetail();
    }

    // TreeInspectionView only rebuilds when its key changes; market/storm/growth changes made by this harness are not in it.
    private void ResetInspectionCache()
        => typeof(TreeInspectionView).GetField("shownKey", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(ui.Inspection, "");

    // ---------------------------------------------------------------- Work Plan

    private IEnumerator WorkPlanStates()
    {
        ForestTreeMarkingManager marking = ui.Marking;
        ForestTree fellOak = Fixture("sessile-oak", 40f, 24f, 0), fellBeech = Fixture("beech", 40f, 24f, 1), cropOak = Fixture("sessile-oak", 24f, 16f, 2);
        marking.Mark(fellOak, TreeMarkType.Fell, false);
        marking.Mark(fellBeech, TreeMarkType.Fell, false);
        marking.Mark(cropOak, TreeMarkType.CropTree, false);
        ui.Manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        int added = ui.Manager.AddMarkedTreesToWorkPlan();
        Check(added >= 2, "WORKPLAN_FELL_ORDERS_ADDED", "added=" + added + " feedback=" + ui.Manager.Feedback);
        int pruneOrders = ui.Manager.BatchPruneCropTrees();
        Check(pruneOrders >= 1, "WORKPLAN_PRUNE_ORDER_ADDED_BY_PRODUCTION_PATH", "added=" + pruneOrders + " feedback=" + ui.Manager.Feedback);
        notes.Add($"Work Plan fixture: fell orders={added} prune orders={pruneOrders} year={ui.Manager.CurrentEcologicalYear}");

        foreach ((string label, int growth) in new[] { ("broadleaf-market", GrowthModel.SpeciesAware), ("old-growth-model", GrowthModel.SiteClassDensity) })
        {
            ecology.GrowthModelVersion = growth;
            bool market = ui.Manager.BroadleafMarketActive;
            foreach (Vector2Int target in Sizes())
            {
                yield return UseSize(target);
                ui.ShowWorkPlan();
                yield return Settle(20);
                CloseAutoHelp();
                yield return Settle(8);
                Check(ui.Manager.WorkPlanOpen, "WORKPLAN_OPEN_" + label);
                VisualElement modal = ui.RootElement.Q(className: "modal-workplan");
                string full = TextOf(modal);

                // Harvest card.
                Label harvestHead = modal.Query<Label>().ToList().FirstOrDefault(l => l.text != null && l.text.StartsWith("Harvest and salvage job", StringComparison.Ordinal));
                VisualElement harvest = Card(harvestHead);
                Check(harvest != null, "WORKPLAN_HARVEST_CARD_PRESENT_" + label);
                string harvestText = harvest != null ? TextOf(harvest) : "";
                string marketNote = "If this visit includes Oak or Beech: " + QualityLimit + " " + PremiumLead;
                if (market) Expect(harvestText, marketNote, "WORKPLAN_HARVEST_BROADLEAF_QUALIFICATION_" + label);
                else
                {
                    Forbid(harvestText, "If this visit includes Oak or Beech", "WORKPLAN_HARVEST_NO_BROADLEAF_NOTE_WITHOUT_MARKET_" + label);
                    Expect(harvestText, "No timber market is configured for some of these stems' species", "WORKPLAN_HARVEST_UNMARKETED_NOTE_" + label);
                }
                if (harvest != null) ScrollTo(modal, harvest);
                yield return Settle(6);
                yield return Shot("workplan-harvest-" + label, harvestText, modal, harvest);

                // Pruning card.
                Label pruneHead = modal.Query<Label>().ToList().FirstOrDefault(l => l.text != null && l.text.StartsWith("Pruning ·", StringComparison.Ordinal));
                VisualElement prune = Card(pruneHead);
                Check(prune != null, "WORKPLAN_PRUNING_CARD_PRESENT_" + label);
                string pruneText = prune != null ? TextOf(prune) : "";
                Expect(pruneText, WorkPlanPruneLead, "WORKPLAN_PRUNING_LEAD_" + label);
                Expect(pruneText, WorkPlanPruneOak, "WORKPLAN_PRUNING_OAK_GRADE_QUALIFIED_BY_MARKET_" + label);
                Forbid(pruneText, "For Oak the recorded pruned section", "WORKPLAN_PRUNING_NO_UNQUALIFIED_OAK_CLAIM_" + label);
                if (prune != null) ScrollTo(modal, prune);
                yield return Settle(6);
                yield return Shot("workplan-pruning-" + label, pruneText, modal, prune);
                ForbidProhibited(full, "workplan-" + label);
                ui.CloseAll();
                yield return Settle(8);
            }
        }
        ecology.GrowthModelVersion = GrowthModel.SpeciesAware;
        ui.CloseAll();
    }

    // ---------------------------------------------------------------- Learning objectives

    private IEnumerator LearningStates()
    {
        ecology.GrowthModelVersion = GrowthModel.SpeciesAware;
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ui.ShowObjectives();
            yield return Settle(16);
            CloseAutoHelp();
            ui.Learning.Refresh(true);
            yield return Settle(6);
            VisualElement modal = ui.RootElement.Q(className: "modal-objectives");
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives && Visible(modal), "OBJECTIVES_OPEN");
            Foldout library = ui.Learning.Root.Query<Foldout>().ToList().FirstOrDefault(f => f.text != null && f.text.StartsWith("All learning topics", StringComparison.Ordinal));
            Check(library != null, "LEARNING_LIBRARY_FOLDOUT_PRESENT");
            if (library == null) { ui.CloseAll(); continue; }
            library.value = true;
            yield return Settle(4);
            Foldout topic = library.Query<Foldout>().ToList().FirstOrDefault(f => f.text == "6. Pruning");
            Check(topic != null, "LEARNING_PRUNING_TOPIC_PRESENT");
            if (topic == null) { ui.CloseAll(); continue; }
            topic.value = true;
            yield return Settle(10);
            string topicText = TextOf(topic);
            Expect(topicText, TopicOak, "TOPIC_6_OAK_GRADE_QUALIFIED_BY_MARKET");
            Expect(topicText, StepLifts, "PRUNE_READ_QUALIFIED_TWO_LIFT_WORDING");
            Expect(topicText, "Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart)", "PRUNE_READ_SITKA_THREE_LIFT_DISTINCTION");
            Expect(topicText, "Understand eligibility", "PRUNE_READ_STEP_TITLE_PRESENT");
            Forbid(topicText, "Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart. All are", "PRUNE_READ_NO_UNQUALIFIED_TWO_LIFT");
            Forbid(topicText, "for Oak the recorded pruned section", "TOPIC_6_NO_UNQUALIFIED_OAK_CLAIM");
            ForbidProhibited(topicText, "learning-pruning");
            Label stepTitle = topic.Query<Label>().ToList().FirstOrDefault(l => l.text != null && l.text.EndsWith("Understand eligibility", StringComparison.Ordinal));
            ScrollTo(modal, stepTitle != null ? Card(stepTitle) : topic);
            yield return Settle(8);
            yield return Shot("learning-pruning-prune-read", topicText, modal, topic);
            ui.CloseAll();
            yield return Settle(8);
        }
    }

    // ---------------------------------------------------------------- capture and audit

    private IEnumerator Shot(string name, string text, params VisualElement[] panels)
    {
        // A selected Game tab is not enough: the native Editor window must be visible/focused too (runner --keep-focus).
        EditorWindow game = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
        game.Focus(); game.Repaint();
        yield return new WaitForSecondsRealtime(0.3f);
        yield return new WaitForEndOfFrame();
        Check(Application.isFocused && EditorWindow.focusedWindow == game, "GAME_VIEW_FOCUSED_" + name);
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) { Fail("SHOT_" + name, "no rendered capture"); yield break; }
        Check(image.width == size.x && image.height == size.y, "CAPTURE_SIZE_" + name, $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        Check(!LooksBlank(image), "SHOT_NOT_BLANK_" + name, "capture must be opaque with visible colour variance; transparent RGB garbage is not a rendered surface");
        string file = $"{name}-{size.x}x{size.y}.png", textFile = $"texts/{name}-{size.x}x{size.y}.txt";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        File.WriteAllText(Path.Combine(output, textFile), text ?? "");
        captures.Add(new CaptureRecord { name = name, path = file, textPath = textFile, width = image.width, height = image.height });
        Destroy(image);
        shots++;
        foreach (VisualElement panel in panels) if (panel != null) AuditPanel(name, panel);
        Debug.Log($"PARITY07_SHOT {file} {size.x}x{size.y}");
    }

    private void AuditPanel(string state, VisualElement panel)
    {
        VisualElement root = ActiveRoot();
        Check(Visible(panel), "PANEL_VISIBLE_" + state + "_" + Describe(panel));
        if (root == null) { Fail("ACTIVE_ROOT_PRESENT_" + state, "no UI root"); return; }
        string tag = state + "_" + Describe(panel);
        Check(HorizontalContained(panel.worldBound, root.worldBound, 2f), "PANEL_CONTAINED_HORIZONTALLY_" + tag, $"panel={panel.worldBound} root={root.worldBound}");
        // Cards inside a ScrollView legitimately extend past the modal; fixed panels (inspection column) must fit vertically.
        bool inScroll = panel.GetFirstAncestorOfType<ScrollView>() != null || panel.Q<ScrollView>() != null;
        if (!inScroll)
            Check(panel.worldBound.yMin >= root.worldBound.yMin - 2f && panel.worldBound.yMax <= root.worldBound.yMax + 2f, "PANEL_CONTAINED_VERTICALLY_" + tag, $"panel={panel.worldBound} root={root.worldBound}");
        var bad = new List<string>();
        int checkedLabels = 0;
        foreach (TextElement label in panel.Query<TextElement>().ToList())
        {
            if (!Visible(label) || string.IsNullOrWhiteSpace(label.text)) continue;
            // Labels scrolled out of the viewport are not on screen; only on-screen text can be clipped.
            if (!OnScreen(label, panel, root)) continue;
            checkedLabels++;
            Rect box = label.worldBound;
            if (box.xMin < panel.worldBound.xMin - 2f || box.xMax > panel.worldBound.xMax + 2f)
                bad.Add($"outside panel \"{Short(label.text)}\" x={box.xMin.ToString("0", Inv)}..{box.xMax.ToString("0", Inv)}");
            if (label.GetFirstAncestorOfType<ScrollView>() == null &&
                (box.yMin < panel.worldBound.yMin - 2f || box.yMax > panel.worldBound.yMax + 2f ||
                 box.yMin < root.worldBound.yMin - 2f || box.yMax > root.worldBound.yMax + 2f))
                bad.Add($"outside fixed panel vertically \"{Short(label.text)}\" y={box.yMin.ToString("0", Inv)}..{box.yMax.ToString("0", Inv)} panel={panel.worldBound}");
            bool noWrap = label.resolvedStyle.whiteSpace == WhiteSpace.NoWrap;
            Vector2 natural = noWrap
                ? label.MeasureTextSize(label.text, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined)
                : label.MeasureTextSize(label.text, box.width, VisualElement.MeasureMode.Exactly, 0f, VisualElement.MeasureMode.Undefined);
            if (noWrap && natural.x > box.width + 1f) bad.Add($"cut one-line \"{Short(label.text)}\" text={natural.x.ToString("0", Inv)} box={box.width.ToString("0", Inv)}");
            if (!noWrap && natural.y > box.height + 2f) bad.Add($"cut wrapped \"{Short(label.text)}\" text={natural.y.ToString("0", Inv)} box={box.height.ToString("0", Inv)}");
        }
        Check(bad.Count == 0, "TEXT_FITS_PANEL_" + tag, bad.Count == 0 ? "labels=" + checkedLabels : string.Join(" | ", bad.Take(4)));
    }

    // Labels scrolled out of a ScrollView viewport are not on screen, so only visible text can be clipped.
    private static bool OnScreen(VisualElement label, VisualElement panel, VisualElement root)
    {
        ScrollView scroll = label.GetFirstAncestorOfType<ScrollView>();
        Rect box = label.worldBound;
        if (scroll == null) return true; // Fixed-panel text outside the viewport is a failure, not text to skip.
        Rect view = scroll.contentViewport.worldBound;
        return box.yMin >= view.yMin - 2f && box.yMax <= view.yMax + 2f;
    }

    private static string Describe(VisualElement element)
        => string.IsNullOrEmpty(element.name) ? string.Join("-", element.GetClasses().Take(2)) : element.name;

    // ---------------------------------------------------------------- text helpers

    private static string Normalise(string text) => Regex.Replace(text ?? "", @"\s+", " ").Trim();

    private static string TextOf(VisualElement root)
    {
        if (root == null) return "";
        var lines = new List<string>();
        foreach (TextElement label in root.Query<TextElement>().ToList())
        {
            // Collapsed foldouts keep their content in the tree; only text a player could see counts as shown copy.
            if (string.IsNullOrWhiteSpace(label.text) || label.resolvedStyle.display == DisplayStyle.None || !AncestorsDisplayed(label, root)) continue;
            lines.Add(Normalise(label.text));
        }
        return string.Join("\n", lines);
    }

    private static bool AncestorsDisplayed(VisualElement element, VisualElement stop)
    {
        for (VisualElement p = element; p != null; p = p.parent)
        {
            if (p.resolvedStyle.display == DisplayStyle.None) return false;
            if (p == stop) break;
        }
        return true;
    }

    private static VisualElement Card(VisualElement from)
    {
        for (VisualElement p = from; p != null; p = p.parent)
            if (p.ClassListContains("card")) return p;
        return null;
    }

    private static void ScrollTo(VisualElement modal, VisualElement target)
    {
        ScrollView scroll = modal != null ? modal.Q<ScrollView>() : null;
        if (scroll != null && target != null) scroll.ScrollTo(target);
    }

    private void Expect(string haystack, string phrase, string name)
        => Check(Normalise(haystack).Contains(Normalise(phrase)), name, "expected \"" + Short(phrase) + "\"");

    private void Forbid(string haystack, string phrase, string name)
        => Check(!Normalise(haystack).Contains(Normalise(phrase)), name, "must not contain \"" + Short(phrase) + "\"");

    private void ForbidProhibited(string text, string name)
    {
        foreach (Regex rx in Prohibited)
        {
            Match m = rx.Match(text ?? "");
            Check(!m.Success, "NO_PROHIBITED_FORM_OR_PREMIUM_CLAIM_" + name, m.Success ? "matched \"" + m.Value + "\" by " + rx : "");
        }
    }

    // ---------------------------------------------------------------- scene helpers

    private IEnumerator Inspect(ForestTree tree)
    {
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        if (inspect == null) throw new InvalidOperationException("ForestPlayer.InspectTree not found");
        Vector3 camera = tree.transform.position + new Vector3(0f, 0f, -3.2f);
        float ground = Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(camera) + Terrain.activeTerrain.transform.position.y : camera.y;
        player.transform.position = new Vector3(camera.x, ground + 0.1f, camera.z);
        player.LookToward(tree.transform.position + Vector3.up * 1.5f);
        for (int i = 0; i < 12; i++)
        {
            Cursor.lockState = CursorLockMode.Locked;
            player.LookToward(tree.transform.position + Vector3.up * 1.5f);
            inspect.Invoke(player, new object[] { tree });
            yield return null;
        }
        yield return Settle(4);
    }

    private void ClearInspection()
    {
        if (player == null) return;
        typeof(ForestPlayer).GetField("isInspecting", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(player, false);
    }

    private void CleanupPresentation()
    {
        ClearInspection();
        SetDetail(false);
        ui.CloseHelp();
        ui.CloseAll();
        player.transform.position = startPosition;
        player.transform.rotation = startRotation;
    }

    private void CloseAutoHelp()
    {
        if (ui != null && ui.Help != null && ui.Help.IsOpen) ui.CloseHelp();
    }

    private VisualElement ActiveRoot() => ui != null ? ui.RootElement : null;

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++) yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y, $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private static bool Visible(VisualElement element)
    {
        if (element == null) return false;
        for (VisualElement p = element; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return element.worldBound.width > 0f && element.worldBound.height > 0f;
    }

    private static bool HorizontalContained(Rect inner, Rect outer, float tolerance)
        => inner.xMin >= outer.xMin - tolerance && inner.xMax <= outer.xMax + tolerance;

    private static string Short(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        text = text.Replace("\r", " ").Replace("\n", " ");
        return text.Length > 70 ? text.Substring(0, 70) + "…" : text;
    }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        Debug.Log((ok ? "PARITY07_CHECK_PASS " : "PARITY07_CHECK_FAIL ") + $"[{size.x}x{size.y}] {name}" + (string.IsNullOrEmpty(detail) ? "" : " " + detail));
    }

    private void Fail(string name, string detail) => Check(false, name, detail);

    private static IEnumerator Settle(int frames)
    {
        for (int i = 0; i < frames; i++) yield return null;
    }

    private void CheckBlankClassifier()
    {
        var texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        var pixels = new Color32[128 * 128];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32((byte)(i % 256), (byte)(i / 128), 90, 0);
        texture.SetPixels32(pixels); texture.Apply();
        Check(LooksBlank(texture), "BLANK_CHECK_REJECTS_TRANSPARENT_COLOUR_NOISE");
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(90, 90, 90, 255);
        texture.SetPixels32(pixels); texture.Apply();
        Check(LooksBlank(texture), "BLANK_CHECK_REJECTS_OPAQUE_UNIFORM_IMAGE");
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32((byte)(i % 256), (byte)(i / 128), 90, 255);
        texture.SetPixels32(pixels); texture.Apply();
        Check(!LooksBlank(texture), "BLANK_CHECK_ACCEPTS_OPAQUE_VARIED_IMAGE");
        Destroy(texture);
    }

    private static bool LooksBlank(Texture2D image)
    {
        Color32[] pixels = image.GetPixels32();
        if (pixels.Length == 0) return true;
        int opaque = 0;
        foreach (Color32 pixel in pixels) if (pixel.a >= 250) opaque++;
        // Real gameplay fills this framebuffer. Hidden RGB in transparent pixels must never qualify it.
        if (opaque < pixels.Length * 0.99f) return true;
        int step = Mathf.Max(1, pixels.Length / 4000), different = 0, samples = 0;
        double sum = 0, sumSquares = 0;
        Color32 first = pixels[0];
        for (int i = 0; i < pixels.Length; i += step)
        {
            Color32 pixel = pixels[i];
            if (Mathf.Abs(pixel.r - first.r) + Mathf.Abs(pixel.g - first.g) + Mathf.Abs(pixel.b - first.b) > 24) different++;
            double light = (pixel.r + pixel.g + pixel.b) / 3.0;
            sum += light; sumSquares += light * light; samples++;
        }
        double variance = sumSquares / samples - Math.Pow(sum / samples, 2);
        return different < 40 || variance < 4.0;
    }

    private string WorldHash()
    {
        if (saves == null) throw new InvalidOperationException("save controller missing for world hash");
        return ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (errors.Count < 40) errors.Add(type + ": " + condition);
    }

    private static void InvokePrivate(object target, string method)
    {
        MethodInfo info = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new MissingMethodException(target.GetType().Name, method);
        info.Invoke(target, null);
    }

    private static IEnumerable<Vector2Int> Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        var parsed = new List<Vector2Int>();
        if (!string.IsNullOrEmpty(text))
        {
            foreach (string part in text.Split(','))
            {
                string[] pair = part.Trim().Split('x');
                if (pair.Length != 2 || !int.TryParse(pair[0], out int width) || !int.TryParse(pair[1], out int height)) continue;
                if (width > 0 && height > 0) parsed.Add(new Vector2Int(width, height));
            }
        }
        foreach (Vector2Int required in RequiredSizes) if (!parsed.Contains(required)) parsed.Add(required);
        return parsed.Distinct().ToArray();
    }

    private void WriteReports(bool pass, Exception failure)
    {
        var audit = new Audit
        {
            verdict = pass ? "PARITY07_UNITY_HARNESS_PASS" : "PARITY07_UNITY_HARNESS_FAIL",
            pass = pass, failures = failures, passes = passes, shots = shots,
            hashBefore = hashBefore, hashAfter = hashAfter, captures = captures, notes = notes, errors = errors,
        };
        if (failure != null) audit.notes.Add("Exception: " + failure);
        File.WriteAllText(Path.Combine(output, "parity07-result.json"), JsonUtility.ToJson(audit, true));
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
    }
}
#endif

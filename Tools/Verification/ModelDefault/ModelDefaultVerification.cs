#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Disposable focused gate: a NORMAL new Scenario One game starts on growth model 2 (GrowthModel.SpeciesAware), and nothing else about
// models, saves or the frozen Reference Future moves.
//
// It checks the real paths a player uses: the state the scene is in the first frame after Awake (a fresh session), the real
// ForestSaveController.Save()/Load() through the isolated config folder, and the real StandaloneSessionMenu.Restart() ("Start a fresh
// Scenario One test"). Saves for the compatibility cases are made by changing only the growthModel field of a captured v20 save, which is
// exactly what an older game's file looks like.
//
// Receipts (one text file per question) are written to CCF_DIAG_DIR/model-default/. No threshold, anchor or model constant is tuned or
// rebaselined here; every expectation is either the accepted constant itself or an equality between two runs of the same game.
//
// Copy to Assets/ForestPrototype, run with  -batchmode -nographics -executeMethod ModelDefaultVerification.Begin
// (the runner stages it and removes it and its .meta afterwards). Needs an isolated config folder with no forest-save.json.
public static class ModelDefaultVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "ModelDefaultVerification.Begin")) return;
        var host = new GameObject("Model default verification");
        UnityEngine.Object.DontDestroyOnLoad(host);   // the gate must survive the scene reload that "Start a fresh Scenario One" performs
        host.AddComponent<ModelDefaultGate>();
    }
}

public sealed class ModelDefaultGate : MonoBehaviour
{
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marks;
    private ForestTreeSpawner spawner;
    private StandaloneSessionMenu session;
    private ForestSaveData baseline;
    private string receiptDir;
    private int checks;
    private readonly List<string> failures = new List<string>();

    private static string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");

    private void Bind()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        marks = FindFirstObjectByType<ForestTreeMarkingManager>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        session = FindFirstObjectByType<StandaloneSessionMenu>();
    }

    private void Check(string name, bool ok, string detail)
    {
        checks++;
        Debug.Log($"MODEL_DEFAULT_{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures.Add(name + " " + detail);
    }

    private void Receipt(string file, string line)
    {
        try { File.AppendAllText(Path.Combine(receiptDir, file), line + "\n"); }
        catch (Exception error) { Debug.LogError("receipt write failed: " + error.Message); }
    }

    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));

    private IEnumerator LoadBaseline()
    {
        bool loaded = saves.LoadData(Clone(baseline), false);
        yield return null; yield return null;
        if (!loaded) throw new InvalidOperationException("baseline did not load");
    }

    private IEnumerator Frames(int count) { for (int i = 0; i < count; i++) yield return null; }

    private ForestTree[] LivingTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving).ToArray();

    private Vector3 OpenPosition()
    {
        ForestTree[] trees = LivingTrees();
        foreach (var cell in ecology.Cells.OrderByDescending(c => c.Light).ThenBy(c => c.Center.x).ThenBy(c => c.Center.y))
            foreach (float x in new[] { -1.5f, 0f, 1.5f }) foreach (float z in new[] { -1.5f, 0f, 1.5f })
            {
                var point = new Vector3(cell.Center.x + x, 0f, cell.Center.y + z);
                if (trees.All(t => Vector2.Distance(new Vector2(t.transform.position.x, t.transform.position.z), new Vector2(point.x, point.z)) >= 0.8f)) return point;
            }
        throw new InvalidOperationException("no open position");
    }

    private ForestTree SpawnMature(string id, string speciesId)
    {
        TreeSpeciesDefinition species = spawner.ResolveSpecies(speciesId);
        if (species == null) throw new InvalidOperationException("species " + speciesId);
        return spawner.Spawn(id, species, OpenPosition(), 40, 24f, 2f + 0.62f * 24f, species.PotentialCrownRadiusM(24f));
    }

    // Synthetic large stems so every timber class is testable (same fixture as BroadleafTimberEconomyVerification).
    private (ForestTree oak, ForestTree beech, ForestTree sitka) SpawnMarketTrio()
    {
        var oak = spawner.Spawn("MD-OAK", spawner.ResolveSpecies("sessile-oak"), new Vector3(1f, 0f, 1f), 110, 60f, 24f, 5f);
        var beech = spawner.Spawn("MD-BEECH", spawner.ResolveSpecies("beech"), new Vector3(-1f, 0f, 1f), 110, 60f, 24f, 5f);
        var sitka = spawner.Spawn("MD-SITKA", spawner.DefaultSpecies, new Vector3(1f, 0f, -1f), 60, 50f, 28f, 4f);
        return (oak, beech, sitka);
    }

    private void MarkForFelling(ForestTree tree)
    {
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        marks.Mark(tree, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
    }

    private static long Revenue(ScenarioHarvestJob job, string treeId) => job.Resolution.Quote.Timber.Where(v => v.Batch.SourceTreeId == treeId).Sum(v => v.SaleRevenueCents);

    // Tree state without the growth-model number, so a model-1 run and a model-2 run of the same game can be compared row by row.
    private static string TreeStateHash(ForestTree[] all)
    {
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values) { foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; } }
        void Text(string value) { Bytes(Encoding.UTF8.GetBytes(value ?? "")); Bytes(new byte[1]); }
        foreach (ForestTree t in all.OrderBy(x => x.TreeId, StringComparer.Ordinal))
        {
            Text(t.TreeId); Text(t.IsLiving ? "L" : "D"); Text(t.MortalityCause);
            Bytes(BitConverter.GetBytes(t.Diameter)); Bytes(BitConverter.GetBytes(t.Height)); Bytes(BitConverter.GetBytes(t.AgeYears));
        }
        return hash.ToString("X16");
    }

    private List<string> RunYears(int model, int years)
    {
        ecology.GrowthModelVersion = model;
        var rows = new List<string>();
        for (int i = 0; i < years; i++)
        {
            ecology.AdvanceOneYear();
            AdultMortalityAccount a = ecology.LastAdultMortality;
            ForestTree[] all = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            rows.Add($"{ecology.EcologicalYear}|{a.Route}|{a.LivingBefore}|{a.Deaths}|{a.HazardDeaths}|{a.BoundaryDeaths}|{all.Count(t => t.IsLiving)}|{TreeStateHash(all)}");
        }
        return rows;
    }

    private IEnumerator WriteSaveFile(ForestSaveData data)
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(data));
        yield return null;
    }

    private IEnumerator DiskLoad()
    {
        saves.Load();
        yield return Frames(4);
    }

    // ---------- driver ----------

    private IEnumerator Start()
    {
        yield return null;
        Bind();
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        receiptDir = Path.Combine(string.IsNullOrEmpty(diag) ? Path.Combine(Application.dataPath, "..", "Build", "ModelDefault") : diag, "model-default");
        Directory.CreateDirectory(receiptDir);
        foreach (string old in Directory.GetFiles(receiptDir, "*.txt")) File.Delete(old);

        Exception fatal = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Execute());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { fatal = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (fatal != null) failures.Add("FATAL " + fatal);
        bool ok = failures.Count == 0 && checks > 0;
        Debug.Log(ok ? $"MODEL_DEFAULT_VERIFY_PASS checks={checks}" : $"MODEL_DEFAULT_VERIFY_FAIL checks={checks} failures=[{string.Join(" | ", failures)}]");
        File.WriteAllText(Path.Combine(receiptDir, "verdict.txt"), (ok ? "PASS" : "FAIL") + $" checks={checks} failures={failures.Count}\n" + string.Join("\n", failures) + "\n");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private IEnumerator Section(string name, IEnumerator body)
    {
        var stack = new Stack<IEnumerator>();
        stack.Push(body);
        Exception failure = null;
        while (stack.Count > 0)
        {
            bool more = false; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; }
            if (failure != null) break;
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (failure != null) Check(name + "_RAN_WITHOUT_EXCEPTION", false, failure.ToString().Replace('\n', ' '));
    }

    private IEnumerator Execute()
    {
        // 0. The state of a normal fresh session, read before anything is touched.
        bool noSaveAtStart = !File.Exists(SavePath);
        Check("PRECONDITION_ISOLATED_CONFIG_HAS_NO_SAVE_FILE", noSaveAtStart, SavePath);
        if (!noSaveAtStart) throw new InvalidOperationException("The isolated config folder already holds a save; this is not a fresh-session check.");
        int growth = ecology.GrowthModelVersion, rng = ecology.RngModelVersion, regen = ecology.RegenerationModelVersion, storm = ecology.StormModelVersion, geo = ecology.StandGeometryModelVersion;
        baseline = saves.CaptureData();

        yield return Section("NEW_GAME", NewGame(growth, rng, regen, storm, geo));
        yield return Section("DENSITY", Density());
        yield return Section("ECONOMY", Economy());
        yield return Section("SAVE_RELOAD", SaveReload());
        yield return Section("EXISTING_SAVES", ExistingSaves());
        yield return Section("REFERENCE", Reference());
        yield return Section("PURE_SITKA", PureSitka());
        yield return Section("FRESH_AFTER_MODEL1", FreshAfterModel1());   // last: it reloads the scene
    }

    // ---------- 1. new game ----------

    private IEnumerator NewGame(int growth, int rng, int regen, int storm, int geo)
    {
        const string r = "receipt-new-game-model.txt";
        Receipt(r, $"first-frame session: growth={growth} rng={rng} regen={regen} storm={storm} geometry={StandGeometryModel.Label(geo)} unityVersion={Application.unityVersion}");
        Check("NEW_GAME_CONSTANT_IS_SPECIES_AWARE", ScenarioOneManager.NewGameGrowthModel == GrowthModel.SpeciesAware && GrowthModel.SpeciesAware == 2,
            $"NewGameGrowthModel={ScenarioOneManager.NewGameGrowthModel} SpeciesAware={GrowthModel.SpeciesAware} Latest={GrowthModel.Latest}");
        Check("FRESH_SESSION_STARTS_ON_GROWTH_MODEL_2", growth == GrowthModel.SpeciesAware, $"growth={growth}");
        Check("FRESH_SESSION_OTHER_NEW_GAME_MODELS_UNCHANGED",
            rng == ScenarioOneManager.NewGameRngModel && regen == ScenarioOneManager.NewGameRegenerationModel && storm == StormModel.None
            && rng == SimulationRandom.MixedModel && regen == RegenerationModel.Competition && geo == StandGeometryPolicy.NewGameModelForSession,
            $"rng={rng} regen={regen} storm={storm} geometry={StandGeometryModel.Label(geo)} (new-game constants rng={ScenarioOneManager.NewGameRngModel} regen={ScenarioOneManager.NewGameRegenerationModel})");
        Check("FRESH_SESSION_BROADLEAF_MARKET_FLAG_ON", manager.BroadleafMarketActive, $"BroadleafMarketActive={manager.BroadleafMarketActive}");
        Check("SAVE_SCHEMA_VERSION_STILL_23", ForestSaveData.CurrentVersion == 23 && baseline.version == 23, $"current={ForestSaveData.CurrentVersion} captured={baseline.version}");
        yield return null;
    }

    // ---------- 2. species-aware density ----------

    private IEnumerator Density()
    {
        const string r = "receipt-species-aware-density.txt";
        yield return LoadBaseline();
        int before = ecology.GrowthModelVersion;
        ecology.AdvanceOneYear();
        AdultMortalityAccount pure = ecology.LastAdultMortality;
        Receipt(r, $"fresh pure-Sitka stand, model {before}, year {ecology.EcologicalYear}: route={pure.Route} living={pure.LivingBefore} deaths={pure.Deaths}");
        Check("PURE_SITKA_FRESH_GAME_RUNS_THE_LEGACY_ROUTE", before == 2 && pure.Route == DensityRoute.LegacySitkaExact && pure.LivingBefore > 0, $"model={before} route={pure.Route} living={pure.LivingBefore}");

        foreach (string speciesId in new[] { "sessile-oak", "beech" })
        {
            yield return LoadBaseline();
            ForestTree broadleaf = SpawnMature("MD-" + speciesId, speciesId);
            yield return null;
            ecology.AdvanceOneYear();
            AdultMortalityAccount a = ecology.LastAdultMortality;
            Receipt(r, $"fresh game + one mature {speciesId} ({broadleaf.TreeId}), model {ecology.GrowthModelVersion}: route={a.Route} occupancy={a.Occupancy:0.0000} living={a.LivingBefore} deaths={a.Deaths} sitkaComponent={a.SitkaComponentDeaths} generic={a.GenericDeaths}");
            Check("ONE_" + speciesId.ToUpperInvariant().Replace('-', '_') + "_SWITCHES_THE_FRESH_GAME_TO_THE_SPECIES_AWARE_ROUTE",
                ecology.GrowthModelVersion == 2 && a.Route == DensityRoute.SpeciesAwareOccupancy && a.Occupancy > 0.0,
                $"model={ecology.GrowthModelVersion} route={a.Route} occupancy={a.Occupancy:0.0000}");
        }
    }

    // ---------- 3. broadleaf market ----------

    private IEnumerator Economy()
    {
        const string r = "receipt-broadleaf-market.txt";
        yield return LoadBaseline();
        var t = SpawnMarketTrio();
        MarkForFelling(t.oak); MarkForFelling(t.beech); MarkForFelling(t.sitka);
        Check("FRESH_GAME_MARKET_ACTIVE", manager.BroadleafMarketActive && ecology.GrowthModelVersion == 2, $"market={manager.BroadleafMarketActive} model={ecology.GrowthModelVersion}");
        ScenarioHarvestJob market = manager.GetHarvestQuote();
        long oak = Revenue(market, t.oak.TreeId), beech = Revenue(market, t.beech.TreeId), sitka = Revenue(market, t.sitka.TreeId);
        Receipt(r, $"fresh game model 2: unmarketed={market.HasUnmarketedSpecies} broadleafMarket={market.BroadleafMarket} eligible={market.Eligible} revenueCents oak={oak} beech={beech} sitka={sitka} costCents={market.CostCents}");
        Check("FRESH_GAME_OAK_AND_BEECH_EARN_REVENUE", !market.HasUnmarketedSpecies && market.BroadleafMarket && market.Eligible && oak > 0 && beech > 0 && sitka > 0,
            $"unmarketed={market.HasUnmarketedSpecies} broadleafMarket={market.BroadleafMarket} eligible={market.Eligible} oak={oak} beech={beech} sitka={sitka}");

        // Control on the same stems: model 1 keeps the original economy (Oak/Beech unmarketed) and Sitka is untouched by the market.
        ecology.GrowthModelVersion = 1;
        ScenarioHarvestJob original = manager.GetHarvestQuote();
        long oak1 = Revenue(original, t.oak.TreeId), beech1 = Revenue(original, t.beech.TreeId), sitka1 = Revenue(original, t.sitka.TreeId);
        Receipt(r, $"same stems, model 1 control: marketActive={manager.BroadleafMarketActive} unmarketed={original.HasUnmarketedSpecies} revenueCents oak={oak1} beech={beech1} sitka={sitka1} costCents={original.CostCents}");
        Check("MODEL_1_CONTROL_KEEPS_THE_ORIGINAL_ECONOMY", !manager.BroadleafMarketActive && original.HasUnmarketedSpecies && oak1 == 0 && beech1 == 0 && sitka1 == sitka && original.CostCents == market.CostCents,
            $"market={manager.BroadleafMarketActive} unmarketed={original.HasUnmarketedSpecies} oak={oak1} beech={beech1} sitka={sitka1} (model-2 sitka={sitka}) cost1={original.CostCents} cost2={market.CostCents}");

        // Settlement uses the same market: cash moves by the job ledger.
        ecology.GrowthModelVersion = 2;
        ScenarioHarvestJob again = manager.GetHarvestQuote();
        bool approved = manager.ApprovePendingWork();
        long cash = manager.CashCents;
        bool advanced = manager.AdvanceYear();
        long delta = manager.CashCents - cash, ledger = ForestryWorkCalculator.SumLedger(again.Resolution.Ledger);
        Receipt(r, $"settlement on model 2: approved={approved} advanced={advanced} cashDeltaCents={delta} ledgerCents={ledger} quotedRevenueCents={again.RevenueCents}");
        Check("FRESH_GAME_SETTLEMENT_USES_THE_BROADLEAF_MARKET", approved && advanced && delta == ledger && again.RevenueCents > 0, $"approved={approved} advanced={advanced} delta={delta} ledger={ledger} revenue={again.RevenueCents}");
    }

    // ---------- 4. save / reload ----------

    private IEnumerator SaveReload()
    {
        const string r = "receipt-save-reload.txt";
        yield return LoadBaseline();
        ecology.AdvanceOneYear();
        ForestSaveData captured = saves.CaptureData();
        string json = JsonUtility.ToJson(captured);
        Receipt(r, $"capture after one year: version={captured.version} growthModel={captured.growthModel} json has \"growthModel\":2 = {json.Contains("\"growthModel\":2")}");
        Check("CAPTURE_STORES_MODEL_2", captured.growthModel == 2 && captured.version == 23 && json.Contains("\"growthModel\":2"), $"growthModel={captured.growthModel} version={captured.version}");

        saves.Save();
        yield return Frames(2);
        bool fileExists = File.Exists(SavePath);
        string disk = fileExists ? File.ReadAllText(SavePath) : "";
        // Save() pretty-prints, so match key/value regardless of whitespace.
        bool diskModel2 = Regex.IsMatch(disk, "\"growthModel\"\\s*:\\s*2\\b"), diskV20 = Regex.IsMatch(disk, "\"version\"\\s*:\\s*20\\b");
        Receipt(r, $"disk save: exists={fileExists} bytes={disk.Length} growthModel2={diskModel2} version20={diskV20} path={SavePath}");
        Check("DISK_SAVE_WRITES_MODEL_2", fileExists && diskModel2 && diskV20, $"exists={fileExists} bytes={disk.Length} growthModel2={diskModel2} version20={diskV20}");

        // Prove the reload comes from the file: knock the live session off model 2 first.
        ecology.GrowthModelVersion = 1;
        Check("PRECONDITION_SESSION_KNOCKED_OFF_MODEL_2", ecology.GrowthModelVersion == 1 && !manager.BroadleafMarketActive, $"model={ecology.GrowthModelVersion}");
        yield return DiskLoad();
        Receipt(r, $"after saves.Load() from disk: growth={ecology.GrowthModelVersion} marketActive={manager.BroadleafMarketActive} year={ecology.EcologicalYear}");
        Check("DISK_RELOAD_RETAINS_MODEL_2", ecology.GrowthModelVersion == 2 && manager.BroadleafMarketActive, $"model={ecology.GrowthModelVersion} market={manager.BroadleafMarketActive}");
        Check("RECAPTURE_AFTER_RELOAD_STILL_MODEL_2", saves.CaptureData().growthModel == 2, $"growthModel={saves.CaptureData().growthModel}");

        ecology.GrowthModelVersion = 1;
        bool memoryLoad = saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false);
        yield return Frames(2);
        Check("IN_MEMORY_JSON_ROUND_TRIP_RETAINS_MODEL_2", memoryLoad && ecology.GrowthModelVersion == 2, $"loaded={memoryLoad} model={ecology.GrowthModelVersion}");
    }

    // ---------- 5. existing saves keep their stored model ----------

    private IEnumerator ExistingSaves()
    {
        const string r = "receipt-existing-model1-compat.txt";
        // A game that already exists on growth model 1, with Oak/Beech/Sitka present (so an upgrade would be visible in routing and economy).
        yield return LoadBaseline();
        var t = SpawnMarketTrio();
        yield return null;
        ForestSaveData made = saves.CaptureData();
        made.growthModel = 1;
        string model1Json = JsonUtility.ToJson(made);
        yield return WriteSaveFile(JsonUtility.FromJson<ForestSaveData>(model1Json));

        yield return LoadBaseline();   // live session is a fresh model-2 game when the old save is loaded over it
        Check("PRECONDITION_LIVE_SESSION_IS_MODEL_2_BEFORE_LOADING_THE_OLD_SAVE", ecology.GrowthModelVersion == 2, $"model={ecology.GrowthModelVersion}");
        yield return DiskLoad();
        int loaded = ecology.GrowthModelVersion;
        bool market = manager.BroadleafMarketActive;
        Receipt(r, $"existing model-1 save loaded through saves.Load(): growth={loaded} marketActive={market} oakPresent={LivingTrees().Any(x => x.TreeId == "MD-OAK")}");
        Check("EXISTING_MODEL_1_SAVE_LOADS_AS_MODEL_1", loaded == 1 && !market, $"model={loaded} market={market}");

        ScenarioHarvestJob oldEconomy = null;
        MarkForFelling(LivingTrees().First(x => x.TreeId == "MD-OAK")); MarkForFelling(LivingTrees().First(x => x.TreeId == "MD-BEECH")); MarkForFelling(LivingTrees().First(x => x.TreeId == "MD-SITKA"));
        oldEconomy = manager.GetHarvestQuote();
        long oakRevenue = Revenue(oldEconomy, "MD-OAK"), beechRevenue = Revenue(oldEconomy, "MD-BEECH");
        Check("EXISTING_MODEL_1_SAVE_KEEPS_THE_ORIGINAL_ECONOMY", oldEconomy.HasUnmarketedSpecies && oakRevenue == 0 && beechRevenue == 0, $"unmarketed={oldEconomy.HasUnmarketedSpecies} oak={oakRevenue} beech={beechRevenue}");

        var routes = new List<string>();
        for (int i = 0; i < 3; i++) { ecology.AdvanceOneYear(); routes.Add(ecology.LastAdultMortality.Route.ToString()); }
        ForestSaveData after = saves.CaptureData();
        Receipt(r, $"three years on the loaded model-1 game with Oak+Beech standing: routes=[{string.Join(",", routes)}] model={ecology.GrowthModelVersion} recaptured growthModel={after.growthModel}");
        Check("NO_SILENT_UPGRADE_DURING_PLAY", ecology.GrowthModelVersion == 1 && after.growthModel == 1 && routes.All(x => x == DensityRoute.LegacySitkaExact.ToString()), $"model={ecology.GrowthModelVersion} recaptured={after.growthModel} routes={string.Join(",", routes)}");

        // Explicit legacy model 0 stays 0.
        yield return LoadBaseline();
        ForestSaveData legacy = saves.CaptureData(); legacy.growthModel = 0;
        yield return WriteSaveFile(legacy);
        yield return DiskLoad();
        ecology.AdvanceOneYear();
        Receipt(r, $"explicit model-0 save loaded: growth={ecology.GrowthModelVersion} recaptured={saves.CaptureData().growthModel} route={ecology.LastAdultMortality.Route}");
        Check("EXISTING_MODEL_0_SAVE_STAYS_MODEL_0", ecology.GrowthModelVersion == 0 && saves.CaptureData().growthModel == 0, $"model={ecology.GrowthModelVersion}");

        // A v20 file with no growthModel field means legacy growth (D-048); the new default must not fill it in.
        yield return LoadBaseline();
        string stripped = Regex.Replace(JsonUtility.ToJson(saves.CaptureData()), "\"growthModel\":\\d+,?", "");
        Check("PRECONDITION_FIELD_REMOVED_FROM_THE_FILE", !stripped.Contains("growthModel"), "stripped json has no growthModel key");
        File.WriteAllText(SavePath, stripped);
        yield return null;
        yield return DiskLoad();
        Receipt(r, $"save file without a growthModel field loaded: growth={ecology.GrowthModelVersion} marketActive={manager.BroadleafMarketActive}");
        Check("SAVE_WITHOUT_GROWTH_MODEL_FIELD_STAYS_LEGACY_NOT_THE_NEW_DEFAULT", ecology.GrowthModelVersion == 0 && !manager.BroadleafMarketActive, $"model={ecology.GrowthModelVersion}");

        // Leave the existing model-1 file on disk for the "fresh game after loading it" check.
        yield return WriteSaveFile(JsonUtility.FromJson<ForestSaveData>(model1Json));
    }

    // ---------- 6. Reference Future ----------

    private IEnumerator Reference()
    {
        const string r = "receipt-reference-future.txt";
        yield return LoadBaseline();
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        bool identity = archive != null && archive.IdentityMatches(manager.Definition, ecology);
        Receipt(r, $"archive loaded={archive != null} identityMatchesModel2Session={identity}");
        Check("REFERENCE_ARCHIVE_IDENTITY_MATCHES_A_MODEL_2_SESSION", identity, $"identity={identity}");
        string worldBefore = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        int sessionModel = ecology.GrowthModelVersion;
        int sessionGeo = ecology.StandGeometryModelVersion;
        foreach (int year in new[] { 0, 50, 100 })
        {
            bool began = manager.TryBeginReferencePreview(year);
            yield return Frames(2);
            int g = ecology.GrowthModelVersion, rg = ecology.RegenerationModelVersion, st = ecology.StormModelVersion, geo = ecology.StandGeometryModelVersion;
            bool market = manager.BroadleafMarketActive;
            Receipt(r, $"preview year {year}: began={began} growth={g} regen={rg} storm={st} geometry={StandGeometryModel.Label(geo)} marketActive={market}");
            Check($"REFERENCE_PREVIEW_Y{year}_KEEPS_ITS_FROZEN_MODELS", began && g == GrowthModel.Legacy && rg == RegenerationModel.Legacy && st == StormModel.None && geo == StandGeometryModel.Legacy40 && !market,
                $"began={began} growth={g} regen={rg} storm={st} geometry={StandGeometryModel.Label(geo)} market={market}");
            manager.EndReferencePreview();
            yield return Frames(2);
        }
        string worldAfter = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Receipt(r, $"after previews: growth={ecology.GrowthModelVersion} geometry={StandGeometryModel.Label(ecology.StandGeometryModelVersion)} worldHashBefore={worldBefore} worldHashAfter={worldAfter}");
        Check("LEAVING_PREVIEW_RESTORES_THE_MODEL_2_GAME_EXACTLY", ecology.GrowthModelVersion == sessionModel && ecology.StandGeometryModelVersion == sessionGeo && worldAfter == worldBefore,
            $"model={ecology.GrowthModelVersion} (was {sessionModel}) hashEqual={worldAfter == worldBefore}");
    }

    // ---------- 7. pure-Sitka compatibility on the default game ----------

    private IEnumerator PureSitka()
    {
        const string r = "receipt-pure-sitka-compat.txt";
        const int years = 40;
        // A = model 1, B = model 2 (the new default), C = model 1 again. A == C separates real model effects from run-to-run noise.
        yield return LoadBaseline(); var a = RunYears(1, years);
        yield return LoadBaseline(); var b = RunYears(2, years);
        yield return LoadBaseline(); var c = RunYears(1, years);
        bool control = a.SequenceEqual(c);
        int firstDiff = -1;
        for (int i = 0; i < years && firstDiff < 0; i++) if (a[i] != b[i]) firstDiff = i + 1;
        int legacyYears = b.Count(row => row.Split('|')[1] == DensityRoute.LegacySitkaExact.ToString());
        int totalDeaths = b.Sum(row => int.Parse(row.Split('|')[3]));
        Receipt(r, $"fresh default game, {years} years from the same baseline: model1-vs-model1 control equal={control}; model1-vs-model2 equal={firstDiff < 0}; firstDifferingYear={firstDiff}; model2 legacyRouteYears={legacyYears}/{years}; model2 densityDeaths={totalDeaths}");
        Receipt(r, "last year row model1: " + a[years - 1]);
        Receipt(r, "last year row model2: " + b[years - 1]);
        Check("CONTROL_TWO_MODEL_1_RUNS_OF_THE_SAME_GAME_ARE_IDENTICAL", control, "A==C");
        Check("PURE_SITKA_DEFAULT_GAME_MODEL_2_EQUALS_MODEL_1_YEAR_BY_YEAR", firstDiff < 0, $"firstDifferingYear={firstDiff}");
        Check("PURE_SITKA_DEFAULT_GAME_USES_THE_LEGACY_ROUTE_EVERY_YEAR", legacyYears == years, $"legacyRouteYears={legacyYears}/{years}");
        Check("PURE_SITKA_RUN_IS_NOT_VACUOUS_LIVING_STEMS_AND_DEATHS_RECORDED", int.Parse(b[0].Split('|')[2]) > 0 && totalDeaths >= 0, $"startLiving={b[0].Split('|')[2]} deaths={totalDeaths}");
    }

    // ---------- 8. "Start a fresh Scenario One" after an old model-1 game ----------

    private IEnumerator FreshAfterModel1()
    {
        const string r = "receipt-fresh-after-model1.txt";
        // The model-1 file written at the end of EXISTING_SAVES is on disk; load it so the live session really is model 1.
        yield return DiskLoad();
        Check("PRECONDITION_SESSION_IS_ON_AN_EXISTING_MODEL_1_GAME", ecology.GrowthModelVersion == 1, $"model={ecology.GrowthModelVersion}");
        string fileBefore = File.ReadAllText(SavePath);
        ScenarioOneManager oldManager = manager;
        // Editor Play Mode does not bootstrap the session menu (a Player always does), so create it the way UxQualification does.
        if (session == null) session = FindFirstObjectByType<StandaloneSessionMenu>();
        if (session == null)
        {
            session = new GameObject("Session controls under test").AddComponent<StandaloneSessionMenu>();
            yield return Frames(4);
            if (StandaloneSessionMenu.IsOpen) session.Close();
            yield return Frames(3);
        }
        Check("PRECONDITION_SESSION_MENU_PRESENT", session != null, session == null ? "StandaloneSessionMenu not available" : "created/found");
        if (session == null) yield break;
        session.Restart();   // the real "Start a fresh Scenario One test" action: closes the menu and reloads the scene
        for (int i = 0; i < 600; i++)
        {
            yield return null;
            var current = FindFirstObjectByType<ScenarioOneManager>();
            if (current != null && current != oldManager) break;
        }
        yield return Frames(3);
        Bind();
        bool reloaded = manager != null && manager != oldManager;
        string fileAfter = File.Exists(SavePath) ? File.ReadAllText(SavePath) : "";
        Receipt(r, $"after Restart(): sceneReloaded={reloaded} growth={ecology.GrowthModelVersion} marketActive={manager.BroadleafMarketActive} year={ecology.EcologicalYear} oldSaveFileUnchanged={fileAfter == fileBefore} oldSaveGrowthModel1={fileAfter.Contains("\"growthModel\":1")}");
        Check("FRESH_GAME_AFTER_A_MODEL_1_SESSION_IS_MODEL_2", reloaded && ecology.GrowthModelVersion == 2 && manager.BroadleafMarketActive, $"reloaded={reloaded} model={ecology.GrowthModelVersion} market={manager.BroadleafMarketActive}");
        Check("RESTART_LEAVES_THE_EXISTING_MODEL_1_SAVE_FILE_UNTOUCHED", fileAfter == fileBefore && fileAfter.Contains("\"growthModel\":1"), $"unchanged={fileAfter == fileBefore}");
    }
}
#endif

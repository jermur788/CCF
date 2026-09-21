using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class CCFIntegrationVerificationTemp
{
#if UNITY_EDITOR
    public static void BeginMixedScaffold()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MixedSpeciesTest.unity");
        EditorApplication.isPlaying = true;
    }

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        new GameObject("CCF Integration Verification Temp").AddComponent<CCFIntegrationVerificationRunnerTemp>();
    }
}

public sealed class CCFIntegrationVerificationRunnerTemp : MonoBehaviour
{
    private string savePath;
    private byte[] previousSave;
    private bool hadPreviousSave;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        try
        {
            savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
            hadPreviousSave = File.Exists(savePath);
            if (hadPreviousSave) previousSave = File.ReadAllBytes(savePath);
            VerifyFreshStand();
        }
        catch (Exception ex) { failure = ex; }

        if (failure == null)
        {
            IEnumerator interactions = VerifyInteractionsAndSaveLoad();
            while (true)
            {
                bool more = false;
                object current = null;
                try
                {
                    more = interactions.MoveNext();
                    if (more) current = interactions.Current;
                }
                catch (Exception ex) { failure = ex; }
                if (failure != null || !more) break;
                yield return current;
            }
        }

        if (failure == null)
        {
            try { VerifyLifecycle(); VerifySuppression(); }
            catch (Exception ex) { failure = ex; }
        }

        if (failure == null) Debug.Log("INTEGRATION_VERIFY_PASS");
        else Debug.LogError("INTEGRATION_VERIFY_FAIL: " + failure);
        RestoreUserSave();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private void VerifyFreshStand()
    {
        ForestTree[] trees = Trees();
        Require(trees.Length == 336, $"fresh tree count {trees.Length}, expected 336");
        Require(trees.All(t => t.TreeId.StartsWith("P", StringComparison.Ordinal)), "fresh stand contains a non-P id");
        Require(trees.Select(t => t.TreeId).Distinct().Count() == 336, "fresh stand has duplicate ids");
        float meanDbh = trees.Average(t => t.Diameter);
        float basalArea = trees.Sum(t => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f)) / 0.16f;
        Require(Mathf.Abs(meanDbh - 15.6f) < 0.35f, $"mean DBH {meanDbh:F3}");
        Require(Mathf.Abs(trees.Length / 0.16f - 2100f) < 0.01f, "stocking mismatch");
        Require(Mathf.Abs(basalArea - 41f) < 2.0f, $"basal area {basalArea:F3}");

        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        Require(player != null && player.GetComponent<CharacterController>() != null, "player/controller missing");
        Require(Mathf.Abs(player.transform.position.x) < 0.5f && Mathf.Abs(player.transform.position.z) < 20f && player.transform.position.y >= 0f,
            $"player start is off the forest road: {player.transform.position}");

        foreach (ForestTree tree in trees)
        {
            foreach (Transform path in FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (path.name.StartsWith("Dirt Path", StringComparison.Ordinal))
                    Require(Planar(tree.transform.position, path.position) > 1.5f, $"tree {tree.TreeId} intersects road clearance");
        }
        var clearing = GameObject.Find("Forest Clearing");
        if (clearing != null)
            foreach (ForestTree tree in trees) Require(Planar(tree.transform.position, clearing.transform.position) > 2.2f, $"tree {tree.TreeId} intersects clearing");
        foreach (ForestTree tree in trees) Require(Planar(tree.transform.position, player.transform.position) > 2.2f, $"tree {tree.TreeId} intersects player start");
        foreach (ForestBuildable buildable in FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            foreach (ForestTree tree in trees) Require(Planar(tree.transform.position, buildable.transform.position) > 2.2f, $"tree {tree.TreeId} intersects {buildable.name}");

        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        Require(spawner != null && ecology != null, "spawner/ecology missing");
        Require(PrivateField<GameObject>(spawner, "visualPrefab") != null, "mature Sitka visual missing");
        Require(PrivateField<GameObject>(spawner, "poleVisualPrefab") != null, "pole Sitka visual missing");
        Require(PrivateField<GameObject>(ecology, "seedlingVisualPrefab") != null, "seedling Sitka visual missing");
        Require(PrivateField<GameObject>(spawner, "stumpPrefab") != null, "stump visual missing");
        Debug.Log($"VERIFY_FRESH_PASS trees=336 stemsHa=2100 meanDbh={meanDbh:F3} basalAreaHa={basalArea:F3} player={player.transform.position}");
    }

    private IEnumerator VerifyInteractionsAndSaveLoad()
    {
        ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
        ForestTreeMarkingManager marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        Require(player != null && marking != null && ecology != null && saves != null && spawner != null, "interaction systems missing");

        ForestTree target = Trees().OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo chop = typeof(ForestPlayer).GetMethod("ChopTree", BindingFlags.Instance | BindingFlags.NonPublic);
        Require(inspect != null && chop != null, "player interaction entry points missing");
        inspect.Invoke(player, new object[] { target });
        Require(player.IsInspecting, "tree inspection did not activate");
        marking.Mark(target, false);
        Require(marking.IsMarked(target), "tree marking failed");

        player.RestoreCarriedWood(0);
        int expectedWood = Mathf.Clamp(Mathf.RoundToInt(target.BiologicalStemVolumeM3 / 0.10f), 3, 24);
        for (int i = 0; i < target.ChopsRequired; i++)
        {
            chop.Invoke(player, new object[] { target });
            yield return new WaitForSeconds(player.EffectiveSwingCooldown + 0.05f);
        }
        Require(target.IsStump, "tree did not fell after required chops");
        Require(player.CarriedWood == expectedWood, $"wood award {player.CarriedWood}, expected {expectedWood}");
        Require(!marking.IsMarked(target), "felled tree remained marked");
        chop.Invoke(player, new object[] { target });
        Require(player.CarriedWood == expectedWood, "resources awarded more than once");

        int yearBefore = ecology.EcologicalYear;
        ecology.AdvanceOneYear();
        Require(ecology.EcologicalYear == yearBefore + 1, "ecological year did not advance");
        Require(ecology.Cells != null && ecology.Cells.Length > 0, "ecology grid missing");
        Require(ecology.Cells.All(c => c != null && IsFinite(c.Canopy) && IsFinite(c.Light) && IsFinite(c.SitkaSeedRain)), "canopy/light/seed rain contains invalid values");

        int savedYear = ecology.EcologicalYear;
        string felledId = target.TreeId;
        int savedWood = player.CarriedWood;
        var savedHistory = Trees().ToDictionary(t => t.TreeId, t => t.EquivalentSuppressedYears);
        saves.Save();
        Require(File.Exists(savePath), "save file was not written");

        ForestTree remove = Trees().First(t => t.TreeId != felledId);
        string removedId = remove.TreeId;
        Destroy(remove.gameObject);
        ForestTree intruder = spawner.Spawn("R999999", Vector3.zero, 2, 2f, 2f, 0.5f);
        Require(intruder != null, "could not create transient recruit");
        player.RestoreCarriedWood(0);
        ecology.AdvanceOneYear();
        yield return null;

        saves.Load();
        yield return null;
        yield return null;
        ForestTree[] loaded = Trees();
        Require(loaded.Length == 336, $"loaded count {loaded.Length}, expected 336");
        Require(loaded.Select(t => t.TreeId).Distinct().Count() == 336, "duplicate ids after load");
        Require(loaded.Count(t => t.TreeId.StartsWith("P", StringComparison.Ordinal)) == 336, "P population was not restored exactly");
        Require(loaded.All(t => t.TreeId != "R999999"), "transient recruit survived load");
        Require(loaded.Any(t => t.TreeId == removedId), "missing saved P tree was not respawned");
        Require(loaded.Single(t => t.TreeId == felledId).IsStump, "felled state did not restore");
        Require(player.CarriedWood == savedWood, "inventory did not restore");
        Require(ecology.EcologicalYear == savedYear, "ecological year did not restore");
        Require(loaded.All(t => t.EquivalentSuppressedYears == savedHistory[t.TreeId]), "history did not restore exactly on existing/respawned trees");
        string currentSave = File.ReadAllText(savePath);
        string currentVersionToken = $"\"version\": {ForestSaveData.CurrentVersion}";
        Require(currentSave.Contains(currentVersionToken),
            $"saved JSON did not contain expected version token {currentVersionToken}");
        string legacySave = currentSave.Replace(currentVersionToken, "\"version\": 5");
        File.WriteAllText(savePath, legacySave);
        saves.Load();
        Require(Trees().All(t => t.EquivalentSuppressedYears == 0f), "legacy history not reset to zero");
        Debug.Log($"VERIFY_INTERACTIONS_SAVELOAD_PASS felled={felledId} wood={savedWood} year={savedYear} trees={loaded.Length}");
    }


    private void VerifySuppression()
    {
        var ecology = FindFirstObjectByType<ForestEcologyController>();
        var spawner = FindFirstObjectByType<ForestTreeSpawner>();
        foreach (var tree in Trees()) DestroyImmediate(tree.gameObject);
        ecology.ResetForDeterministicRun();
        var small = spawner.Spawn("PTEST0", Vector3.zero, 20, 8f, 10f, 1f);
        var large = spawner.Spawn("PTEST1", new Vector3(1f,0f,0f), 20, 35f, 15f, 2f);
        ecology.InvalidateCompetition();
        Require(small.AgeYears == large.AgeYears && small.SizeClass != large.SizeClass, "age and size coupled");
        Require(small.Species.Maturity(small.AgeYears) == large.Species.Maturity(large.AgeYears), "size affects reproduction");
        float suppression = ecology.GetCurrentSuppression(small);
        Require(suppression > 0f, "neighbour did not suppress tree");
        ecology.AdvanceOneYear();
        float slowGrowth = ecology.GetAnnualDbhGrowth(small);
        Require(Mathf.Abs(small.EquivalentSuppressedYears - suppression) < 0.000001f, "annual history integral wrong");
        float history = small.EquivalentSuppressedYears;
        float dbh = small.Diameter;
        int age = small.AgeYears;
        large.Fell();
        Require(ecology.GetCurrentSuppression(small) == 0f, "release does not remove current competition");
        Require(small.Diameter == dbh && small.AgeYears == age && small.EquivalentSuppressedYears == history, "release changed past state");
        ecology.AdvanceOneYear();
        Require(small.AgeYears == age + 1 && small.EquivalentSuppressedYears == history, "release reset age/history");
        Require(ecology.GetAnnualDbhGrowth(small) > slowGrowth, "release did not improve subsequent growth");
        Require(large.AgeYears == 21, "stump continued ageing");
        // History is diagnostic: same starting physical state and neighbours,
        // different accumulated history must produce the same next growth.
        float releasedGrowth = ecology.GetAnnualDbhGrowth(small);
        small.SetSimulationState(age, 10f, dbh, 1f);
        small.RestoreSuppressionHistory(100f);
        ecology.AdvanceOneYear();
        Require(ecology.GetAnnualDbhGrowth(small) == releasedGrowth, "history penalizes growth twice");
        Require(small.EquivalentSuppressedYears == 100f, "released history changed");
        Debug.Log("VERIFY_SUPPRESSION_PASS sameAgeDifferentSize=True release=True history=True noSecondPenalty=True");
    }

    private void VerifyLifecycle()
    {
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        string hashA = RunLifecycle(ecology, out int recruitsA, out int age30A);
        string hashB = RunLifecycle(ecology, out int recruitsB, out int age30B);
        Require(recruitsA == 30 && recruitsB == 30, $"lifecycle recruits A={recruitsA} B={recruitsB}");
        Require(age30A == 30 && age30B == 30, $"age-30 recruits A={age30A} B={age30B}");
        Require(hashA == "7E39B70A14959FAD", "baseline ecology changed: " + hashA);
        Require(hashA == hashB, $"lifecycle hashes differ A={hashA} B={hashB}");
        Debug.Log($"VERIFY_LIFECYCLE_PASS years=80 recruits=30 age30=30 hashA={hashA} hashB={hashB} match=True configuration=Editor");
    }

    private static string RunLifecycle(ForestEcologyController ecology, out int recruits, out int age30)
    {
        ForestStandScenarios.ApplyLifecycleFixture();
        for (int year = 1; year <= 80; year++) ecology.AdvanceOneYear();
        ForestTree[] trees = Trees();
        recruits = trees.Count(t => !t.IsStump && t.TreeId.StartsWith("R", StringComparison.Ordinal));
        age30 = trees.Count(t => !t.IsStump && t.TreeId.StartsWith("R", StringComparison.Ordinal) && t.AgeYears >= 30);
        return StateHash();
    }

    private static string StateHash()
    {
        ulong hash = 14695981039346656037UL;
        List<ForestTree> trees = new List<ForestTree>(Trees());
        trees.Sort((a, b) => string.CompareOrdinal(a.TreeId ?? "", b.TreeId ?? ""));
        HashString(ref hash, "TREES:" + trees.Count);
        foreach (ForestTree tree in trees)
        {
            HashString(ref hash, tree.TreeId ?? "");
            HashString(ref hash, ((int)tree.Stage).ToString());
            HashString(ref hash, tree.AgeYears.ToString());
            HashFloat(ref hash, tree.Height);
            HashFloat(ref hash, tree.Diameter);
            HashFloat(ref hash, tree.CrownRadius);
            HashFloat(ref hash, tree.transform.position.x);
            HashFloat(ref hash, tree.transform.position.z);
        }
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        if (ecology != null)
        {
            HashString(ref hash, "YEAR:" + ecology.EcologicalYear);
            HashString(ref hash, "MAST:" + ecology.LastMastLabel);
            HashFloat(ref hash, ecology.LastMastMultiplier);
            if (ecology.Cells != null)
            {
                HashString(ref hash, "CELLS:" + ecology.Cells.Length);
                foreach (ForestEcologyCell cell in ecology.Cells)
                {
                    if (cell == null) { HashFloat(ref hash, -1f); continue; }
                    HashFloat(ref hash, cell.Canopy);
                    HashFloat(ref hash, cell.Light);
                    HashFloat(ref hash, cell.SitkaSeedRain);
                    HashFloat(ref hash, cell.RegenDensity);
                    HashFloat(ref hash, cell.RegenHeight);
                    HashFloat(ref hash, cell.RegenEstablishYear);
                    HashFloat(ref hash, cell.RecentOpening);
                    HashFloat(ref hash, cell.EstablishmentSuitability);
                }
            }
        }
        return hash.ToString("X16");
    }

    private static void HashFloat(ref ulong hash, float value) => HashBytes(ref hash, BitConverter.GetBytes(value));
    private static void HashString(ref ulong hash, string value)
    {
        HashBytes(ref hash, Encoding.UTF8.GetBytes(value ?? ""));
        HashBytes(ref hash, new byte[1]);
    }
    private static void HashBytes(ref ulong hash, byte[] bytes)
    {
        foreach (byte value in bytes) { hash ^= value; hash *= 1099511628211UL; }
    }

    private void RestoreUserSave()
    {
        if (string.IsNullOrEmpty(savePath)) return;
        if (hadPreviousSave) File.WriteAllBytes(savePath, previousSave);
        else if (File.Exists(savePath)) File.Delete(savePath);
    }

    private static ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    private static float Planar(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    private static T PrivateField<T>(object instance, string name) where T : class
        => instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(instance) as T;
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

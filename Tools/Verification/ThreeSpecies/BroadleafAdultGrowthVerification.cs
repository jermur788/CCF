using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable in-Editor gate: Oak and Beech adult growth under ACTIVE growth models (audit gap F).
//
// The existing Oak/Beech gates pin GrowthModel.Legacy. This gate runs the same species under growth
// models 0, 1 and 2 and records and asserts the CURRENT contract, so a later change to Oak or Beech
// growth (Phase 2/3 if evidence is supplied) is a visible, deliberate difference and not an accident:
//   - height follows the species law  dH = potential x site x (1 - H/Max)  with no competition term;
//   - DBH follows the Hegyi response, so crowded trees grow less than isolated ones;
//   - neither depends on the growth-model number while no density mortality acts;
//   - deterministic replay and save/load continuation.
// Oak and Beech have DIFFERENT accepted adult constants (below); they are asserted, not just reported.
//
// PARITY-04 strengthening (documentation/verification only; no production change). The gate now also asserts, from the
// LIVE loaded species assets and the running ecology:
//   - the accepted adult constants, exactly (Oak, Beech, and Sitka as the protected comparator). Oak/Beech are
//     GAMEPLAY CALIBRATION (asset tag [C]), not empirical Irish broadleaf growth constants, and the growing-space classes
//     are a weak, provisional ordinal ranking (Oak High, Beech Intermediate, Sitka Reference);
//   - the production site scalar is exactly 1.0 in every ecology cell and at every sampled position (the species-site
//     response is dormant and neutral; the dormant seam itself is asserted by Tools/Verification/SiteInfrastructure and
//     is deliberately NOT referenced here: that check forbids any other verification tool from using it);
//   - canopy and light are bit-identical across species for identical crown geometry and state (species may differ only
//     through crown geometry/relaxation);
//   - the Hegyi cutoff is exactly 8 m and a neighbour just inside (7.99 m) is counted, just outside (8.01 m) is not,
//     identically for every target/neighbour species pair;
//   - Oak/Beech heights at ages 50/75/100 are PRINTED as diagnostics. Beech sits below its provisional band at ages 50 and
//     100: that is the accepted provisional calibration limitation (PARITY-02 U-11), not an empirical pass/fail target.
//
// STATUS: strengthened in a non-Unity session and checked only by the repository's non-Unity tooling. The strengthened
// version has NOT YET RUN IN UNITY. Its expected pass marker is BROADLEAF_GROWTH_VERIFY_PASS with checks=30 (by source count:
// 26 asserting + 4 informational); an earlier version of this gate recorded 14/14 on a different lineage and is not comparable.
//
// Copy into Assets/ForestPrototype, run with
//   CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -executeMethod BroadleafAdultGrowthVerification.Begin
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
public static class BroadleafAdultGrowthVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "BroadleafAdultGrowthVerification.Begin"))
            new GameObject("Broadleaf adult growth verification").AddComponent<BroadleafAdultGrowthVerificationRunner>();
    }
}

public sealed class BroadleafAdultGrowthVerificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka, oak, beech;
    private readonly List<string> failures = new List<string>();
    private int passed;

    private static string F(double v, string f = "0.###") => v.ToString(f, Inv);
    private void Pass(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"BROADLEAF_GROWTH_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Check(bool ok, string what) { if (!ok) { failures.Add(what); Debug.LogError("BROADLEAF_GROWTH_CHECK_FAIL " + what); } }
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private string TreeStateHash()
    {
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values) { foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; } }
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(t => t.TreeId, StringComparer.Ordinal))
        {
            Bytes(System.Text.Encoding.UTF8.GetBytes(t.TreeId ?? "")); Bytes(new byte[] { (byte)(t.IsLiving ? 1 : 0) });
            Bytes(BitConverter.GetBytes(t.Diameter)); Bytes(BitConverter.GetBytes(t.Height)); Bytes(BitConverter.GetBytes(t.AgeYears));
        }
        return hash.ToString("X16");
    }

    // Fixture control: the nine grid trees are the only stems the question is about. After about 40 years of age the parents seed the
    // sparse stand and natural recruits promote into it; they would add competition (the closed-form law assumes CI 0 at 12 m
    // spacing), change Living() and re-order the probe tree. Each annual step therefore removes every stem that is not a grid tree.
    // This leaves the adult-growth checks exactly as written; it only stops the fixture being polluted.
    private HashSet<string> fixtureIds = new HashSet<string>();
    private int recruitsRemoved;

    private void Step()
    {
        e.AdvanceOneYear();
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (!fixtureIds.Contains(t.TreeId)) { recruitsRemoved++; DestroyImmediate(t.gameObject); }
    }

    // 3 x 3 grid. Spacing above the 8 m Hegyi cutoff gives every tree a competition index of exactly 0.
    private void Grid(int growth, TreeSpeciesDefinition s, float spacing, float dbh = 5.25f, float height = 3.5f, int age = 12)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growth;
        string prefix = s == oak ? "O" : s == beech ? "B" : "S";
        fixtureIds = new HashSet<string>(Enumerable.Range(0, 9).Select(k => prefix + k.ToString("00")));
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                spawner.Spawn(prefix + (i * 3 + j).ToString("00"), s, new Vector3((i - 1) * spacing, 0f, (j - 1) * spacing), age, dbh, height, s.PotentialCrownRadiusM(dbh));
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    // The documented law, restated independently of the controller.
    private static (float h, float d) Law(TreeSpeciesDefinition s, float h, float d, float site, float ci)
    {
        float dg = s.PotentialDbhGrowthCmPerYear * site * Mathf.Clamp01(1f - d / s.MaxDbhCm) * (1f / (1f + ci / s.Ci50));
        float hg = s.PotentialHeightGrowthMPerYear * site * Mathf.Clamp01(1f - h / s.MaxHeightM);
        return (h + hg, d + dg);
    }

    // ---- PARITY-04 strengthening: live assertions of the accepted adult model (verification only) -------------------------

    // Accepted adult calibration. Oak/Beech: GAMEPLAY CALIBRATION (asset tag [C]; the search-grid centre of the pre-registered
    // Oak/Beech calibration), NOT empirical Irish growth constants. Sitka is the protected comparator and must not move.
    private struct AcceptedAdult
    {
        public string Id;
        public float Pd, Ph, Dmax, Hmax, Ci50;
        public GrowingSpaceDemandClass Demand;
    }

    private static readonly AcceptedAdult[] AcceptedAdults =
    {
        new AcceptedAdult { Id = "sitka-spruce", Pd = 1.2f, Ph = 0.45f, Dmax = 120f, Hmax = 35f, Ci50 = 5f, Demand = GrowingSpaceDemandClass.Reference },
        new AcceptedAdult { Id = "sessile-oak", Pd = 0.8f, Ph = 0.65f, Dmax = 120f, Hmax = 42.5f, Ci50 = 5f, Demand = GrowingSpaceDemandClass.High },
        new AcceptedAdult { Id = "beech", Pd = 0.9f, Ph = 0.75f, Dmax = 120f, Hmax = 42.5f, Ci50 = 6.5f, Demand = GrowingSpaceDemandClass.Intermediate },
    };

    // The live loaded assets must carry exactly the accepted constants, the accepted growing-space class and no age-phase table.
    private void AssertAcceptedAdultConstants()
    {
        foreach (AcceptedAdult a in AcceptedAdults)
        {
            TreeSpeciesDefinition s = spawner.ResolveSpecies(a.Id);
            bool exact = s != null && s.PotentialDbhGrowthCmPerYear == a.Pd && s.PotentialHeightGrowthMPerYear == a.Ph
                && s.MaxDbhCm == a.Dmax && s.MaxHeightM == a.Hmax && s.Ci50 == a.Ci50
                && s.GrowingSpaceDemand == a.Demand && !s.HasGrowthAgePhases;
            string live = s == null ? "species not resolved"
                : $"Pd={F(s.PotentialDbhGrowthCmPerYear)} Ph={F(s.PotentialHeightGrowthMPerYear)} Dmax={F(s.MaxDbhCm)} Hmax={F(s.MaxHeightM)} Ci50={F(s.Ci50)} class={s.GrowingSpaceDemand} agePhases={s.HasGrowthAgePhases}";
            Pass($"{a.Id}_ACCEPTED_ADULT_CONSTANTS_EXACT", exact,
                $"live [{live}] accepted [Pd={F(a.Pd)} Ph={F(a.Ph)} Dmax={F(a.Dmax)} Hmax={F(a.Hmax)} Ci50={F(a.Ci50)} class={a.Demand} agePhases=False] "
                + (a.Id == "sitka-spruce" ? "(protected comparator)" : "(GAMEPLAY CALIBRATION, not empirical; weak provisional class)"));
        }
    }

    // The site scalar the growth path multiplies by must be exactly 1.0 everywhere: the species-site response is dormant and neutral.
    private void AssertSiteNeutral(string when)
    {
        ForestEcologyCell[] all = e.Cells;
        int cells = 0, nonNeutral = 0, positionNotOne = 0;
        if (all != null)
            foreach (ForestEcologyCell c in all)
            {
                cells++;
                if (c.SiteProductivity != 1f) nonNeutral++;
                if (e.GetSiteProductivity(new Vector3(c.Center.x, 0f, c.Center.y)) != 1f) positionNotOne++;
            }
        Pass($"SITE_SCALAR_EXACTLY_1_{when}", cells > 0 && nonNeutral == 0 && positionNotOne == 0,
            $"cells={cells} cellsNotExactlyOne={nonNeutral} positionLookupsNotExactlyOne={positionNotOne}");
    }

    private void ClearTrees()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
    }

    // The same reset sequence Grid uses, for fixtures that do not need Grid's nine-tree layout.
    private void ResetFixtureWorld()
    {
        ClearTrees();
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = 2;
    }

    private int[] canopyProbeBits;

    // Two trees with the SAME ids, positions, size and crown whatever the species; only the species definition differs.
    // Records the canopy and light of every ecology cell as raw float bits.
    private IEnumerator CanopyProbe(TreeSpeciesDefinition s, float dbh, float height, float crown, int age)
    {
        ResetFixtureWorld();
        spawner.Spawn("K00", s, new Vector3(0f, 0f, 0f), age, dbh, height, crown);
        spawner.Spawn("K01", s, new Vector3(9f, 0f, 3f), age, dbh, height, crown);
        e.InvalidateCompetition();
        e.RecomputeCanopy();
        yield return null;
        e.RecomputeCanopy();
        Check(Living().Length == 2, $"canopy probe {s.SpeciesId}: expected exactly 2 living trees, found {Living().Length}");
        ForestEcologyCell[] cells = e.Cells;
        canopyProbeBits = new int[cells.Length * 2];
        for (int i = 0; i < cells.Length; i++)
        {
            canopyProbeBits[2 * i] = BitConverter.SingleToInt32Bits(cells[i].Canopy);
            canopyProbeBits[2 * i + 1] = BitConverter.SingleToInt32Bits(cells[i].Light);
        }
    }

    // Canopy and light have no species term: identical geometry and state must give bit-identical results for every species.
    // Species may differ only through crown geometry/relaxation (the inputs), which this probe holds equal.
    private IEnumerator CanopySpeciesNeutrality()
    {
        const float dbh = 30f, height = 18f, crown = 4.2f;
        const int age = 40;
        var perSpecies = new List<int[]>();
        foreach (TreeSpeciesDefinition s in new[] { sitka, oak, beech })
        {
            yield return CanopyProbe(s, dbh, height, crown, age);
            perSpecies.Add(canopyProbeBits);
        }
        bool sameLength = perSpecies.All(b => b.Length == perSpecies[0].Length);
        int differing = 0, shaded = 0;
        if (sameLength)
            for (int i = 0; i < perSpecies[0].Length; i++)
            {
                if (perSpecies[1][i] != perSpecies[0][i] || perSpecies[2][i] != perSpecies[0][i]) differing++;
                if (i % 2 == 0 && BitConverter.Int32BitsToSingle(perSpecies[0][i]) > 0f) shaded++;
            }
        Pass("CANOPY_LIGHT_IDENTICAL_ACROSS_SPECIES_FOR_IDENTICAL_GEOMETRY", sameLength && differing == 0 && shaded > 0,
            $"cells={perSpecies[0].Length / 2} shadedCells={shaded} differingValues={differing} (sitka vs oak vs beech; crown {F(crown)} m, height {F(height)} m, DBH {F(dbh)} cm)");

        // Sensitivity control: the probe must notice a real geometry change, or the identity above would prove nothing.
        yield return CanopyProbe(oak, dbh, height, crown + 1f, age);
        int sensitivity = 0;
        for (int i = 0; i < canopyProbeBits.Length && i < perSpecies[1].Length; i++)
            if (canopyProbeBits[i] != perSpecies[1][i]) sensitivity++;
        Pass("CANOPY_PROBE_IS_SENSITIVE_TO_CROWN_GEOMETRY", sensitivity > 0, $"differingValues={sensitivity} when the Oak crown grows by 1 m");

        // Report: the only species-specific canopy inputs.
        Pass("CROWN_GEOMETRY_REPORT", true,
            $"potential crown radius at DBH 30 cm: sitka={F(sitka.PotentialCrownRadiusM(30f))} m oak={F(oak.PotentialCrownRadiusM(30f))} m beech={F(beech.PotentialCrownRadiusM(30f))} m; "
            + $"relaxation per year: sitka={F(sitka.CrownRelaxationPerYear)} oak={F(oak.CrownRelaxationPerYear)} beech={F(beech.CrownRelaxationPerYear)} (the only permitted species differences)");
    }

    // The Hegyi neighbourhood is exactly 8 m and species-neutral: a neighbour just inside counts, just outside does not,
    // identically for every target/neighbour species pair.
    private IEnumerator HegyiBoundary()
    {
        Pass("HEGYI_CUTOFF_EXACTLY_8_M", ForestEcologyController.HegyiCutoffMeters == 8f,
            $"cutoff={F(ForestEcologyController.HegyiCutoffMeters)} m");
        const float dbh = 20f, inside = 7.99f, outside = 8.01f;
        float expected = ForestEcologyController.HegyiTerm(dbh, dbh, inside);
        var insideValues = new List<float>();
        var outsideValues = new List<float>();
        foreach (TreeSpeciesDefinition target in new[] { sitka, oak, beech })
            foreach (TreeSpeciesDefinition neighbour in new[] { sitka, oak, beech })
            {
                ResetFixtureWorld();
                ForestTree t = spawner.Spawn("H00", target, Vector3.zero, 40, dbh, 15f, 3f);
                ForestTree n = spawner.Spawn("H01", neighbour, new Vector3(inside, 0f, 0f), 40, dbh, 15f, 3f);
                yield return null;
                Check(Living().Length == 2, $"hegyi probe {target.SpeciesId}/{neighbour.SpeciesId}: expected exactly 2 living trees, found {Living().Length}");
                e.InvalidateCompetition();
                insideValues.Add(e.GetCompetitionIndex(t));
                n.transform.position = new Vector3(outside, 0f, 0f);
                e.InvalidateCompetition();
                outsideValues.Add(e.GetCompetitionIndex(t));
            }
        bool insideIdentical = insideValues.Count == 9
            && insideValues.All(v => BitConverter.SingleToInt32Bits(v) == BitConverter.SingleToInt32Bits(insideValues[0]));
        Pass("HEGYI_INSIDE_8M_COUNTED_IDENTICALLY_FOR_EVERY_SPECIES_PAIR",
            insideIdentical && insideValues[0] > 0f && Mathf.Abs(insideValues[0] - expected) < 1e-6f,
            $"pairs={insideValues.Count} CI with a neighbour at {F(inside)} m = {F(insideValues[0], "0.#####")} (expected {F(expected, "0.#####")})");
        Pass("HEGYI_OUTSIDE_8M_EXCLUDED_FOR_EVERY_SPECIES_PAIR", outsideValues.Count == 9 && outsideValues.All(v => v == 0f),
            $"pairs={outsideValues.Count} CI with a neighbour at {F(outside)} m = {F(outsideValues.Max(), "0.#####")} (must be exactly 0)");
    }

    // Provisional [C] height bands used when the Oak/Beech constants were calibrated (Tools/Calibration/OakBeechGrowth/kernel_controls.py):
    // Oak 20-25 m at age 50 and 30-35 m at age 100; Beech 25-30 m at age 50 and 35-40 m at age 100. DIAGNOSTIC ONLY.
    private static string HeightBandNote(string label, int age, float height)
    {
        float lo, hi;
        if (age == 50) { lo = label == "OAK" ? 20f : 25f; hi = label == "OAK" ? 25f : 30f; }
        else if (age == 100) { lo = label == "OAK" ? 30f : 35f; hi = label == "OAK" ? 35f : 40f; }
        else return "no provisional band at this age";
        string position = height < lo ? $"{F(lo - height, "0.00")} m BELOW" : height > hi ? $"{F(height - hi, "0.00")} m ABOVE" : "inside";
        string accepted = label == "BEECH" && height < lo
            ? " ACCEPTED_PROVISIONAL_HEIGHT_BAND_MISS (PARITY-02 U-11): a calibration limitation, not a defect, not retuned"
            : "";
        return $"provisional band {F(lo, "0")}-{F(hi, "0")} m: {position}{accepted}";
    }

    // Heights at ages 50/75/100 from a promoted stem (3.5 m, 5.25 cm, age about 10) in an isolated fixture (CI 0, site 1), PRINTED as
    // diagnostics. They are not empirical pass/fail targets; the only assertion is that the printed engine value is the species law's value.
    private IEnumerator HeightDiagnostics(TreeSpeciesDefinition s, string label)
    {
        Grid(2, s, 12f, 5.25f, 3.5f, 10);
        yield return null;
        ForestTree probe = Living()[4];
        float lh = probe.Height, ld = probe.Diameter, maxDiff = 0f;
        while (probe.AgeYears < 100)
        {
            Step();
            (lh, ld) = Law(s, lh, ld, 1f, 0f);
            maxDiff = Mathf.Max(maxDiff, Mathf.Abs(lh - probe.Height));
            int age = probe.AgeYears;
            if (age == 50 || age == 75 || age == 100)
                Debug.Log($"ADULT_HEIGHT_DIAGNOSTIC {label} age={age} height={F(probe.Height, "0.00")} m ({HeightBandNote(label, age, probe.Height)}) [diagnostic only; not an empirical pass/fail target; GAMEPLAY CALIBRATION]");
            if (age % 10 == 0) yield return null;
        }
        Check(Living().Length == 9, $"{label} height diagnostic: sparse stand lost a tree or gained a recruit the fixture did not remove (living={Living().Length})");
        Pass($"{label}_HEIGHT_DIAGNOSTIC_MATCHES_SPECIES_LAW", maxDiff < 1e-3f, $"maxDiff={F(maxDiff, "0.#####")} m over ages 11-100 against the restated law (CI 0, site 1)");
    }

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        sitka = spawner.ResolveSpecies("sitka-spruce");
        oak = spawner.ResolveSpecies("sessile-oak");
        beech = spawner.ResolveSpecies("beech");
        Check(sitka != null && oak != null && beech != null, "species resolve");
        original = saves.CaptureData();

        // Oak and Beech carry different accepted adult constants. This line only REPORTS the live values of both (the assertion
        // that they are the accepted values is AssertAcceptedAdultConstants below), so a future divergence is a recorded decision.
        bool shared = oak.PotentialDbhGrowthCmPerYear == beech.PotentialDbhGrowthCmPerYear && oak.MaxDbhCm == beech.MaxDbhCm
            && oak.PotentialHeightGrowthMPerYear == beech.PotentialHeightGrowthMPerYear && oak.MaxHeightM == beech.MaxHeightM && oak.Ci50 == beech.Ci50;
        Pass("OAK_BEECH_ADULT_PARAMETERS_REPORT", true,
            $"sharedAdultParameters={shared} oak[dbhPot={F(oak.PotentialDbhGrowthCmPerYear)} heightPot={F(oak.PotentialHeightGrowthMPerYear)} maxH={F(oak.MaxHeightM)} maxD={F(oak.MaxDbhCm)} ci50={F(oak.Ci50)}] "
            + $"beech[dbhPot={F(beech.PotentialDbhGrowthCmPerYear)} heightPot={F(beech.PotentialHeightGrowthMPerYear)} maxH={F(beech.MaxHeightM)} maxD={F(beech.MaxDbhCm)} ci50={F(beech.Ci50)}]");
        AssertAcceptedAdultConstants();
        AssertSiteNeutral("SCENE_START");

        foreach (TreeSpeciesDefinition s in new[] { oak, beech })
        {
            string label = s == oak ? "OAK" : "BEECH";
            var heights = new Dictionary<int, float[]>();
            var dbhs = new Dictionary<int, float[]>();
            var hashes = new Dictionary<int, string>();
            foreach (int growth in new[] { 0, 1, 2 })
            {
                Grid(growth, s, 12f);
                yield return null;
                var h = new float[61]; var d = new float[61];
                ForestTree probe = Living()[4];
                h[0] = probe.Height; d[0] = probe.Diameter;
                for (int y = 1; y <= 60; y++)
                {
                    Step();
                    h[y] = probe.Height; d[y] = probe.Diameter;
                    if (y % 10 == 0) yield return null;
                }
                heights[growth] = h; dbhs[growth] = d; hashes[growth] = TreeStateHash();
                Check(Living().Length == 9, $"{label} growth {growth}: sparse stand lost a tree or gained a recruit the fixture did not remove (living={Living().Length})");
            }
            // Closed-form law with ci = 0 and site = 1 (the production site scalar).
            float lh = 3.5f, ld = 5.25f; bool lawOk = true;
            for (int y = 1; y <= 60; y++)
            {
                (lh, ld) = Law(s, lh, ld, 1f, 0f);
                if (Mathf.Abs(lh - heights[1][y]) > 1e-3f || Mathf.Abs(ld - dbhs[1][y]) > 1e-3f) lawOk = false;
            }
            Pass($"{label}_FOLLOWS_SPECIES_LAW_UNDER_GROWTH1", lawOk, $"H20={F(heights[1][20], "0.00")} H40={F(heights[1][40], "0.00")} H60={F(heights[1][60], "0.00")} D60={F(dbhs[1][60], "0.0")} (ci 0, site 1)");
            bool sameAcrossModels = Enumerable.Range(0, 61).All(y => heights[0][y] == heights[1][y] && heights[1][y] == heights[2][y] && dbhs[0][y] == dbhs[1][y] && dbhs[1][y] == dbhs[2][y]);
            Pass($"{label}_GROWTH_INDEPENDENT_OF_GROWTH_MODEL_WHEN_NO_DENSITY", sameAcrossModels, $"hash0={hashes[0]} hash1={hashes[1]} hash2={hashes[2]}");
            bool monotone = Enumerable.Range(1, 60).All(y => heights[2][y] > heights[2][y - 1] && dbhs[2][y] > dbhs[2][y - 1] && heights[2][y] <= s.MaxHeightM);
            Pass($"{label}_MONOTONE_AND_BOUNDED", monotone, $"maxHeight={s.MaxHeightM}");

            // Competition lowers DBH, not height.
            Grid(2, s, 12f); yield return null;
            for (int y = 0; y < 30; y++) Step();
            ForestTree isolated = Living()[4];
            float isolatedD = isolated.Diameter, isolatedH = isolated.Height;
            Grid(2, s, 2.5f); yield return null;
            for (int y = 0; y < 30; y++) Step();
            ForestTree crowdedCentre = Living().FirstOrDefault(t => t.TreeId.EndsWith("04"));
            Pass($"{label}_COMPETITION_REDUCES_DBH_NOT_HEIGHT", crowdedCentre != null && crowdedCentre.Diameter < isolatedD - 0.05f && Mathf.Abs(crowdedCentre.Height - isolatedH) < 1e-3f,
                crowdedCentre == null ? "centre tree died" : $"isolated D={F(isolatedD)} H={F(isolatedH)} crowded D={F(crowdedCentre.Diameter)} H={F(crowdedCentre.Height)}");

            // Deterministic replay and save/load continuation.
            string first = null;
            for (int run = 0; run < 2; run++)
            {
                Grid(2, s, 3.5f); yield return null;
                for (int y = 0; y < 25; y++) Step();
                string hash = TreeStateHash();
                if (run == 0) first = hash; else Pass($"{label}_REPLAY_IDENTICAL", hash == first, $"hash={hash}");
            }
            Grid(2, s, 3.5f); yield return null;
            for (int y = 0; y < 10; y++) Step();
            string mid = JsonUtility.ToJson(saves.CaptureData());
            for (int y = 0; y < 15; y++) Step();
            string uninterrupted = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
            Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(mid), false), label + " mid load");
            yield return null;
            for (int y = 0; y < 15; y++) Step();
            string resumed = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
            Pass($"{label}_SAVE_LOAD_CONTINUATION", uninterrupted == resumed, $"uninterrupted={uninterrupted} resumed={resumed}");
            Pass($"{label}_FIXTURE_RECRUITS_REMOVED_REPORT", true, $"recruitsRemovedSoFar={recruitsRemoved} (natural regeneration promoted into the sparse stand and was removed each year; the checks above are unchanged)");
            // PARITY-04: the site scalar is still exactly 1.0 after the fixture runs, then the Oak/Beech height diagnostics (printed only).
            AssertSiteNeutral(label + "_AFTER_FIXTURE_RUNS");
            yield return HeightDiagnostics(s, label);
        }

        // PARITY-04: shared canopy/light and the shared 8 m Hegyi neighbourhood, species-neutral for identical inputs.
        yield return CanopySpeciesNeutrality();
        yield return HegyiBoundary();

        // Sitka under growth model 2 still follows the Class III envelope (the Sitka-only height gate is unchanged).
        Grid(2, sitka, 12f, 20f, 12f, 25); yield return null;
        ForestTree s0 = Living()[4]; float expected = SitkaGrowthModel.NextHeight(s0.Height, s0.AgeYears);
        Step();
        Pass("SITKA_HEIGHT_STILL_CLASS_III_UNDER_GROWTH2", Mathf.Abs(Living()[4].Height - expected) < 1e-4f, $"expected={F(expected, "0.0000")} got={F(Living()[4].Height, "0.0000")}");
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
        Debug.Log(ok ? $"BROADLEAF_GROWTH_VERIFY_PASS checks={passed}" : $"BROADLEAF_GROWTH_VERIFY_FAIL {failure} checks=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}

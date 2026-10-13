using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// Disposable, read-only per-year probe for completion measurements. Active only when CCF_DENSITY_PROBE_FILE is set,
// so staging it next to a gate changes nothing unless asked.
//
// It hooks the controller's own per-year "ECOLOGY year=" log line, which is written at the very end of every annual
// step. A frame-polling probe would miss years that a gate advances inside one frame (the completion gate yields only
// every fifth year); a log hook runs synchronously inside the step, so every year is recorded.
//
// Each row carries a tree-state hash that does not include the growth-model number, so a growth-model-1 run and a
// growth-model-2 run of the same playthrough can be compared row by row; the first differing row is the first
// divergence.
public static class DensityProbe
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        string file = Environment.GetEnvironmentVariable("CCF_DENSITY_PROBE_FILE");
        if (string.IsNullOrEmpty(file)) return;
        new GameObject("Density probe").AddComponent<DensityProbeBehaviour>().Configure(file);
    }
}

public sealed class DensityProbeBehaviour : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string Header = "year,growthModel,geometry,route,livingBefore,densityDeaths,hazard,boundary,occupancy,sitkaRd,pressure,livingAfter,nSitka,nBeech,nOak,nOther,baM2ha,treeStateHash";
    private string file;
    private ForestEcologyController e;
    private bool writing;

    public void Configure(string path)
    {
        file = path;
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, Header + "\n");
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() { Application.logMessageReceived += OnLog; }
    private void OnDisable() { Application.logMessageReceived -= OnLog; }

    private void OnLog(string condition, string stackTrace, LogType type)
    {
        if (writing || type != LogType.Log || condition == null || !condition.StartsWith("ECOLOGY year=", StringComparison.Ordinal)) return;
        writing = true;
        try { Record(); }
        catch (Exception ex) { File.AppendAllText(file, "PROBE_ERROR " + ex.Message.Replace('\n', ' ') + "\n"); }
        finally { writing = false; }
    }

    private static string F(double v, string f) => v.ToString(f, Inv);

    private void Record()
    {
        if (e == null) e = FindFirstObjectByType<ForestEcologyController>();
        if (e == null) return;
        AdultMortalityAccount a = e.LastAdultMortality;
        ForestTree[] all = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        ForestTree[] living = all.Where(t => t.IsLiving).ToArray();
        int nSitka = 0, nBeech = 0, nOak = 0, nOther = 0;
        double ba = 0.0;
        foreach (ForestTree t in living)
        {
            string id = t.Species != null ? t.Species.SpeciesId : "";
            if (id == "sitka-spruce") nSitka++; else if (id == "beech") nBeech++; else if (id == "sessile-oak") nOak++; else nOther++;
            ba += Math.PI / 4.0 * (t.Diameter / 100.0) * (t.Diameter / 100.0);
        }
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values) { foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; } }
        void Text(string value) { Bytes(Encoding.UTF8.GetBytes(value ?? "")); Bytes(new byte[1]); }
        foreach (ForestTree t in all)
        {
            Text(t.TreeId); Text(t.IsLiving ? "L" : "D"); Text(t.MortalityCause);
            Bytes(BitConverter.GetBytes(t.Diameter)); Bytes(BitConverter.GetBytes(t.Height)); Bytes(BitConverter.GetBytes(t.AgeYears));
        }
        Text("YEAR:" + e.EcologicalYear);
        string geo = e.StandGeometryModelVersion == StandGeometryModel.Legacy40 ? "L40" : "E80";
        string row = $"{e.EcologicalYear},{e.GrowthModelVersion},{geo},{a.Route},{a.LivingBefore},{a.Deaths},{a.HazardDeaths},{a.BoundaryDeaths},{F(a.Occupancy, "0.0000")},{F(a.RelativeDensity, "0.0000")},{F(a.DensityPressure, "0.0000")},{living.Length},{nSitka},{nBeech},{nOak},{nOther},{F(ba / e.StandAreaHectares, "0.00")},{hash:X16}";
        File.AppendAllText(file, row + "\n");
    }
}

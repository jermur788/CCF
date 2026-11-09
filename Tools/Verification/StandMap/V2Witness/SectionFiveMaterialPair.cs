#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// CCF #358: a bounded, verification-only before/after render of the Section Five flora materials.
//
// Unity (the installed URP BaseShaderGUI) rewrites seven tracked Section Five .mat files whenever an Editor session ends; the new bytes add
// _MainTex (= _BaseMap), set _AlphaToMask on the two leaf materials and round-trip _Color through float. This probe renders the SAME flora, from the SAME
// cameras, at the SAME resolution and quality, in the SAME frame, twice with the committed (HEAD) material state and once with an in-memory
// copy of each material that carries the generated values. It never writes an asset: the generated state is a runtime Material clone built from the
// generated .mat bytes that an earlier Editor session produced (parsed here, not typed in), so no production material is edited to fabricate the
// comparison. Order per view: committed, generated, committed again, then the live Editor-session state; committed-vs-committed is the noise floor, so a non-zero HEAD-vs-GENERATED difference can be told
// from render noise.
//
// Needs a graphical Editor. Environment: CCF_MATPAIR_HEAD_DIR (the seven committed .mat files, named <Material>.mat), CCF_MATPAIR_GEN_DIR (the seven
// generated ones), CCF_DIAG_DIR (evidence folder), CCF_UI_SIZES (resolutions, default 1920x1080,1280x720).
public static class SectionFiveMaterialPair
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "SectionFiveMaterialPair.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "SectionFiveMaterialPair.Begin"))
            new GameObject("Section five material pair").AddComponent<SectionFiveMaterialPairRunner>();
    }
}

public sealed class SectionFiveMaterialPairRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly string[] MaterialNames = { "Berry_Calyx", "Bilberry_Berries", "Bilberry_Leaves", "Bilberry_Stems", "Forb_Flower_Centre", "Forb_Leaves", "Forb_Stems" };
    private static readonly string[] AllowedDifferences = { "_MainTex", "_AlphaToMask", "_Color" };
    private string output;
    private int shots, passed;
    private readonly List<string> failures = new List<string>(), evidence = new List<string>();

    private sealed class MatData
    {
        public readonly Dictionary<string, string> Textures = new Dictionary<string, string>();
        public readonly Dictionary<string, float> Floats = new Dictionary<string, float>();
        public readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>();
        public string Sha256;
    }

    private void Hard(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"MATPAIR_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Note(string name, string detail) { evidence.Add(name + " " + detail); Debug.Log($"MATPAIR_MEASURE {name} {detail}"); }
    private static string F(double v) => v.ToString("0.#########", Inv);
    private static bool Same(Color a, Color b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
    private static bool Near(Color a, Color b) => Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a) <= 1e-6f;

    // A tiny reader for the three kinds of saved property a Unity .mat lists. It reads only what it needs and ignores everything else.
    private static MatData Parse(string path)
    {
        var d = new MatData();
        byte[] bytes = File.ReadAllBytes(path);
        using (var sha = SHA256.Create()) d.Sha256 = BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        string[] lines = File.ReadAllText(path).Split('\n');
        string section = "", pendingTexture = null;
        foreach (string raw in lines)
        {
            string line = raw.TrimEnd('\r');
            string t = line.Trim();
            if (t == "m_TexEnvs:") { section = "tex"; continue; }
            if (t == "m_Ints:" || t.StartsWith("m_Ints:")) { section = "int"; continue; }
            if (t == "m_Floats:") { section = "float"; continue; }
            if (t == "m_Colors:") { section = "color"; continue; }
            if (t.StartsWith("m_BuildTextureStacks")) { section = ""; continue; }
            if (section == "tex")
            {
                if (t.StartsWith("- _") && t.EndsWith(":")) { pendingTexture = t.Substring(2, t.Length - 3); continue; }
                if (t.StartsWith("m_Texture:") && pendingTexture != null)
                {
                    int g = t.IndexOf("guid: ", StringComparison.Ordinal);
                    d.Textures[pendingTexture] = g < 0 ? "" : t.Substring(g + 6, 32);
                    pendingTexture = null;
                }
            }
            else if (section == "float" && t.StartsWith("- _"))
            {
                int c = t.IndexOf(':'); d.Floats[t.Substring(2, c - 2)] = float.Parse(t.Substring(c + 1).Trim(), Inv);
            }
            else if (section == "color" && t.StartsWith("- _"))
            {
                int c = t.IndexOf(':'); string name = t.Substring(2, c - 2); string body = t.Substring(c + 1).Trim().Trim('{', '}');
                float[] v = new float[4];
                foreach (string part in body.Split(',')) { string[] kv = part.Split(':'); int idx = "rgba".IndexOf(kv[0].Trim()[0]); if (idx >= 0) v[idx] = float.Parse(kv[1].Trim(), Inv); }
                d.Colors[name] = new Color(v[0], v[1], v[2], v[3]);
            }
        }
        return d;
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (output != null) File.WriteAllLines(Path.Combine(output, "matpair-evidence.txt"), evidence);
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"MATPAIR_CAPTURE_PASS shots={shots} checks={passed}"
            : $"MATPAIR_CAPTURE_FAIL shots={shots} checks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private static Vector2Int[] Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        if (string.IsNullOrEmpty(text)) text = "1920x1080,1280x720";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    private static Bounds BoundsOf(IEnumerable<Renderer> renderers)
    {
        bool first = true; Bounds b = new Bounds();
        foreach (Renderer r in renderers) { if (first) { b = r.bounds; first = false; } else b.Encapsulate(r.bounds); }
        return b;
    }

    private Color32[] Render(Camera cam, int w, int h, string pngName)
    {
        var rt = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture previous = RenderTexture.active; RenderTexture.active = rt;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0); tex.Apply();
        RenderTexture.active = previous; cam.targetTexture = null; RenderTexture.ReleaseTemporary(rt);
        Color32[] pixels = tex.GetPixels32();
        if (pngName != null) { File.WriteAllBytes(Path.Combine(output, pngName), tex.EncodeToPNG()); shots++; }
        Destroy(tex);
        return pixels;
    }

    private static void Compare(Color32[] a, Color32[] b, out int differing, out int maxChannel, out double meanAbs)
    {
        differing = 0; maxChannel = 0; long sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            int dr = Mathf.Abs(a[i].r - b[i].r), dg = Mathf.Abs(a[i].g - b[i].g), db = Mathf.Abs(a[i].b - b[i].b);
            int m = Mathf.Max(dr, Mathf.Max(dg, db));
            if (m > 0) differing++;
            if (m > maxChannel) maxChannel = m;
            sum += dr + dg + db;
        }
        meanAbs = sum / (3.0 * a.Length);
    }

    private IEnumerator Verify()
    {
        if (Application.isBatchMode) throw new InvalidOperationException("the material pair needs a graphical Editor");
        string headDir = Environment.GetEnvironmentVariable("CCF_MATPAIR_HEAD_DIR"), genDir = Environment.GetEnvironmentVariable("CCF_MATPAIR_GEN_DIR");
        if (string.IsNullOrEmpty(headDir) || string.IsNullOrEmpty(genDir)) throw new InvalidOperationException("CCF_MATPAIR_HEAD_DIR / CCF_MATPAIR_GEN_DIR not set");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/MatPair" : diag, "matpair");
        Directory.CreateDirectory(output);
        yield return new WaitForSeconds(0.5f);
        ScenarioOneUiRoot ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        if (ui != null) { if (ui.Help.IsOpen) ui.CloseHelp(); ui.CloseAll(); }
        yield return null; yield return null;

        // ---- what the HEAD and the generated bytes say ----
        var head = new Dictionary<string, MatData>(); var gen = new Dictionary<string, MatData>();
        foreach (string name in MaterialNames)
        {
            head[name] = Parse(Path.Combine(headDir, name + ".mat")); gen[name] = Parse(Path.Combine(genDir, name + ".mat"));
            var keys = new List<string>();
            foreach (var kv in head[name].Textures) if (!gen[name].Textures.TryGetValue(kv.Key, out string g) || g != kv.Value) keys.Add(kv.Key);
            foreach (var kv in head[name].Floats) if (!gen[name].Floats.TryGetValue(kv.Key, out float f) || f != kv.Value) keys.Add(kv.Key);
            foreach (var kv in head[name].Colors) if (!gen[name].Colors.TryGetValue(kv.Key, out Color c) || !Same(c, kv.Value)) keys.Add(kv.Key);
            Note("PARSED_" + name, $"headSha256={head[name].Sha256} generatedSha256={gen[name].Sha256} differingProperties=[{string.Join(",", keys)}]");
            Hard($"{name}_GENERATED_DIFFERS_ONLY_IN_THE_THREE_KNOWN_PROPERTIES", keys.Count > 0 && keys.All(k => AllowedDifferences.Contains(k)), string.Join(",", keys));
            Hard($"{name}_GENERATED_HAS_NO_PROPERTY_HEAD_LACKS", gen[name].Textures.Keys.All(k => head[name].Textures.ContainsKey(k)) && gen[name].Floats.Keys.All(k => head[name].Floats.ContainsKey(k)) && gen[name].Colors.Keys.All(k => head[name].Colors.ContainsKey(k)));
        }

        // ---- the flora the game itself uses, staged at a fixed place in the live world ----
        RecentAssetVisualCatalog catalog = RecentAssetVisualCatalog.Load();
        Hard("CATALOG_HAS_THE_THREE_FLORA_PREFABS", catalog != null && catalog.bilberryCover != null && catalog.herbRosette != null && catalog.herbFlowering != null);
        Camera main = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
        Vector3 origin = ui != null && ui.Player != null ? ui.Player.transform.position : Vector3.zero;
        Vector3 anchor = new Vector3(origin.x + 8f, origin.y + 50f, origin.z + 8f);
        float groundY = Physics.Raycast(anchor, Vector3.down, out RaycastHit hit, 200f) ? hit.point.y : origin.y;
        Note("STAGE", $"origin=({F(origin.x)},{F(origin.y)},{F(origin.z)}) groundY={F(groundY)} mainCamera={(main != null ? main.name : "none")}");
        GameObject[] prefabs = { catalog.bilberryCover, catalog.herbRosette, catalog.herbFlowering };
        var instances = new List<GameObject>();
        float x = origin.x + 8f;
        foreach (GameObject prefab in prefabs)
        {
            GameObject inst = Instantiate(prefab, new Vector3(x, groundY, origin.z + 8f), Quaternion.identity);
            inst.name = "MATPAIR " + prefab.name;
            foreach (LODGroup lod in inst.GetComponentsInChildren<LODGroup>()) lod.ForceLOD(0);
            instances.Add(inst);
            x += 1.6f;
        }
        yield return null; yield return null;

        // ---- the live materials, and two in-memory states built from the committed and the generated bytes ----
        // Run 1 showed that Unity has ALREADY applied the leaf changes (_MainTex, _AlphaToMask) to the in-memory material at session start, so the live
        // material is not a reliable "committed" baseline. Both states are therefore built explicitly from the parsed .mat bytes, and the live material is
        // rendered as a third state so what the Editor session itself draws is on record too.
        var liveByName = new Dictionary<string, Material>(); var headByName = new Dictionary<string, Material>(); var genByName = new Dictionary<string, Material>();
        foreach (GameObject inst in instances)
            foreach (Renderer r in inst.GetComponentsInChildren<Renderer>())
                foreach (Material m in r.sharedMaterials)
                {
                    if (m == null) continue;
                    string asset = AssetDatabase.GetAssetPath(m);
                    foreach (string name in MaterialNames)
                        if (asset.EndsWith("/" + name + ".mat", StringComparison.Ordinal)) liveByName[name] = m;
                }
        Hard("ALL_SEVEN_MATERIALS_ARE_USED_BY_THE_STAGED_FLORA", MaterialNames.All(n => liveByName.ContainsKey(n)), "found=" + string.Join(",", liveByName.Keys));
        string TextureGuid(Material m, string key) { Texture t = m.GetTexture(key); return t != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(t)) : ""; }
        Material Build(Material live, MatData state, string label)
        {
            var clone = new Material(live) { name = live.name + " (" + label + ", in memory)" };
            foreach (string key in AllowedDifferences)
            {
                if (state.Colors.TryGetValue(key, out Color c) && clone.HasProperty(key)) clone.SetColor(key, c);
                if (state.Floats.TryGetValue(key, out float f) && clone.HasProperty(key)) clone.SetFloat(key, f);
                if (state.Textures.TryGetValue(key, out string guid) && clone.HasProperty(key))
                    clone.SetTexture(key, string.IsNullOrEmpty(guid) ? null : AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            return clone;
        }
        bool Equals(Material m, MatData state, out string detail)
        {
            bool ok = true; var parts = new List<string>();
            foreach (string key in AllowedDifferences)
            {
                if (state.Colors.TryGetValue(key, out Color c) && m.HasProperty(key)) { bool e = Near(m.GetColor(key), c); ok &= e; parts.Add($"{key}:{(e ? "=" : "!=")}"); }
                if (state.Floats.TryGetValue(key, out float f) && m.HasProperty(key)) { bool e = m.GetFloat(key) == f; ok &= e; parts.Add($"{key}:{(e ? "=" : "!=")}"); }
                if (state.Textures.TryGetValue(key, out string guid) && m.HasProperty(key)) { bool e = TextureGuid(m, key) == guid; ok &= e; parts.Add($"{key}:{(e ? "=" : "!=")}"); }
            }
            detail = string.Join(" ", parts); return ok;
        }
        foreach (string name in MaterialNames)
        {
            if (!liveByName.TryGetValue(name, out Material live)) continue;
            // Where the Editor session's own in-memory material stands, property by property (exact comparison, no tolerance).
            bool liveIsHead = Equals(live, head[name], out string headDetail), liveIsGen = Equals(live, gen[name], out string genDetail);
            Note("LIVE_STATE_" + name, $"equalsCommittedBytes={liveIsHead} [{headDetail}] equalsGeneratedBytes={liveIsGen} [{genDetail}] (every property listed is one of the three that differ or all parsed _Color/_AlphaToMask/_MainTex entries)");
            Material h = Build(live, head[name], "committed state"), g = Build(live, gen[name], "generated state");
            Hard($"{name}_IN_MEMORY_COMMITTED_STATE_EQUALS_THE_COMMITTED_FILE", Equals(h, head[name], out string d1), d1);
            Hard($"{name}_IN_MEMORY_GENERATED_STATE_EQUALS_THE_GENERATED_FILE", Equals(g, gen[name], out string d2), d2);
            // Where the files differ in a texture or a float (the two leaf materials) the two in-memory states must differ too. The _Color changes are ~3e-8,
            // below what a material can read back, so for those materials this is measured and recorded, not asserted.
            bool textureOrFloatDiffers = head[name].Textures.Any(kv => !gen[name].Textures.TryGetValue(kv.Key, out string gg) || gg != kv.Value) || head[name].Floats.Any(kv => !gen[name].Floats.TryGetValue(kv.Key, out float ff) || ff != kv.Value);
            bool statesDiffer = !Equals(h, gen[name], out string d3);
            if (textureOrFloatDiffers) Hard($"{name}_THE_TWO_STATES_DIFFER_WHERE_THE_FILES_DIFFER_IN_A_TEXTURE_OR_FLOAT", statesDiffer, d3);
            else Note("COLOUR_ONLY_" + name, $"the files differ only in _Color float round-trip; in-memory states differ={statesDiffer} [{d3}]");
            if (head[name].Colors.TryGetValue("_Color", out Color pc) && gen[name].Colors.TryGetValue("_Color", out Color qc))
                Note("COLOUR_RAW_" + name, $"parsedCommitted=({pc.r:R},{pc.g:R},{pc.b:R}) parsedGenerated=({qc.r:R},{qc.g:R},{qc.b:R}) live=({live.GetColor("_Color").r:R},{live.GetColor("_Color").g:R},{live.GetColor("_Color").b:R}) committedState=({h.GetColor("_Color").r:R},{h.GetColor("_Color").g:R},{h.GetColor("_Color").b:R}) generatedState=({g.GetColor("_Color").r:R},{g.GetColor("_Color").g:R},{g.GetColor("_Color").b:R})");
            headByName[name] = h; genByName[name] = g;
        }

        // ---- the render rig: one camera, a fixed pose per view, the game's own camera settings ----
        var rig = new GameObject("MATPAIR camera");
        Camera cam = rig.AddComponent<Camera>();
        if (main != null)
        {
            cam.CopyFrom(main);
            UniversalAdditionalCameraData from = main.GetComponent<UniversalAdditionalCameraData>(), to = cam.GetUniversalAdditionalCameraData();
            if (from != null && to != null) { to.renderPostProcessing = from.renderPostProcessing; to.antialiasing = from.antialiasing; to.renderShadows = from.renderShadows; to.volumeLayerMask = from.volumeLayerMask; to.volumeTrigger = rig.transform; }
        }
        cam.enabled = false; cam.fieldOfView = 40f; cam.nearClipPlane = 0.05f;
        Note("CAMERA", $"copiedFrom={(main != null ? main.name : "none")} fov={F(cam.fieldOfView)} msaaAllowed={cam.allowMSAA} urpMsaa={UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline?.name} qualityAA={QualitySettings.antiAliasing} quality={QualitySettings.names[QualitySettings.GetQualityLevel()]}");

        var views = new List<(string name, Bounds bounds)> { ("overview", BoundsOf(instances.SelectMany(i => i.GetComponentsInChildren<Renderer>()))) };
        foreach (GameObject inst in instances) views.Add((inst.name.Replace("MATPAIR ", ""), BoundsOf(inst.GetComponentsInChildren<Renderer>())));
        var renderers = instances.SelectMany(i => i.GetComponentsInChildren<Renderer>()).ToList();
        var original = renderers.ToDictionary(r => r, r => r.sharedMaterials);
        // state: 0 = the live material as the Editor session holds it, 1 = committed state, 2 = generated state
        void Apply(int state)
        {
            foreach (Renderer r in renderers)
            {
                Material[] src = original[r]; var swapped = new Material[src.Length];
                for (int i = 0; i < src.Length; i++)
                {
                    Material m = src[i]; string key = null;
                    if (m != null) { string asset = AssetDatabase.GetAssetPath(m); key = MaterialNames.FirstOrDefault(n => asset.EndsWith("/" + n + ".mat", StringComparison.Ordinal)); }
                    swapped[i] = key == null || state == 0 ? m : state == 1 ? headByName[key] : genByName[key];
                }
                r.sharedMaterials = swapped;
            }
        }

        foreach (Vector2Int size in Sizes())
        {
            string res = size.x + "x" + size.y;
            foreach ((string view, Bounds b) in views)
            {
                Vector3 centre = b.center; float radius = Mathf.Max(0.05f, b.extents.magnitude);
                float distance = radius / Mathf.Sin(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 1.05f;
                Vector3 direction = new Vector3(0.0f, 0.55f, -1.0f).normalized;
                cam.transform.position = centre + direction * distance; cam.transform.LookAt(centre);
                cam.aspect = (float)size.x / size.y;
                // One warm-up render so shadow and light setup is built before the ones that count; all in the same frame.
                Apply(1); Render(cam, size.x, size.y, null);
                Apply(1); Color32[] h1 = Render(cam, size.x, size.y, $"{view}-committed-{res}.png");
                Apply(2); Color32[] g1 = Render(cam, size.x, size.y, $"{view}-generated-{res}.png");
                Apply(1); Color32[] h2 = Render(cam, size.x, size.y, null);
                Apply(0); Color32[] l1 = Render(cam, size.x, size.y, $"{view}-live-editor-state-{res}.png");
                Compare(h1, h2, out int noiseDiff, out int noiseMax, out double noiseMean);
                Compare(h1, g1, out int effDiff, out int effMax, out double effMean);
                Compare(l1, h1, out int liveHeadDiff, out int liveHeadMax, out double liveHeadMean);
                Compare(l1, g1, out int liveGenDiff, out int liveGenMax, out double liveGenMean);
                Note($"VIEW_{view}_{res}", $"pixels={h1.Length} noiseFloor(committed-vs-committed2): differing={noiseDiff} maxChannel={noiseMax} meanAbs={F(noiseMean)} | effect(committed-vs-generated): differing={effDiff} maxChannel={effMax} meanAbs={F(effMean)} | live-vs-committed: differing={liveHeadDiff} maxChannel={liveHeadMax} | live-vs-generated: differing={liveGenDiff} maxChannel={liveGenMax}");
                Hard($"{view}_{res}_RENDER_IS_REPEATABLE_SO_THE_COMPARISON_IS_VALID", noiseDiff == 0, $"noiseDiffPixels={noiseDiff}");
                if (effDiff > 0) Debug.Log($"MATPAIR_EFFECT {view} {res} generated state changes {effDiff} pixels (max channel difference {effMax}, mean {F(effMean)})");
                else Debug.Log($"MATPAIR_NO_EFFECT {view} {res} generated state renders pixel-identical to the committed state");
                yield return null;
            }
        }
        Apply(0);
        Destroy(rig); foreach (GameObject inst in instances) Destroy(inst);
        yield return null;
    }
}
#endif

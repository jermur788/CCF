using System.Collections.Generic;
using UnityEngine;

// Darker, more natural Sitka needle colour, applied at runtime to a shared copy of the delivered needle material.
//
// The delivered material SS_Benchmark_Needles (Art/SitkaPlantation02 and Art/SitkaMatureBenchmark) multiplies its needle texture
// by white; the texture averages a pale, slightly blue-grey green (about sRGB 84, 111, 86), which reads as washed out under the
// sky light. Rather than edit the delivered assets (their bytes are pinned by the delivery manifests), one tinted copy of each
// source material is made the first time it is needed and every renderer shares it, so the SRP Batcher and instancing still
// group the trees exactly as before. Presentation only: no tree, ecology, save or economy state is read or written.
public static class SitkaFoliage
{
    public const string NeedleMaterialName = "SS_Benchmark_Needles";

    // Multiplies the material's base colour (white): needles about sRGB (52, 80, 48), a dark conifer green.
    public static readonly Color Tint = new Color(0.62f, 0.72f, 0.56f, 1f);

    private static readonly Dictionary<Material, Material> Darkened = new Dictionary<Material, Material>();

    // The material a needle renderer should use: a shared darkened copy for the delivered needle material, otherwise the same one.
    public static Material Darken(Material source)
    {
        if (source == null || source.name != NeedleMaterialName)
            return source;
        if (Darkened.TryGetValue(source, out Material copy) && copy != null)
            return copy;
        copy = new Material(source) { name = source.name + " (darker foliage)" };
        Multiply(copy, "_BaseColor");
        Multiply(copy, "_Color");
        Darkened[source] = copy;
        return copy;
    }

    // Swaps the needle material on every renderer under a visual (the mature models; the plantation model binds its own).
    public static void Apply(GameObject visual)
    {
        if (visual == null)
            return;
        foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>(true))
        {
            Material[] shared = renderer.sharedMaterials;
            bool changed = false;
            for (int i = 0; i < shared.Length; i++)
            {
                Material darker = Darken(shared[i]);
                if (darker != shared[i]) { shared[i] = darker; changed = true; }
            }
            if (changed)
                renderer.sharedMaterials = shared;
        }
    }

    private static void Multiply(Material material, string property)
    {
        if (material.HasProperty(property))
            material.SetColor(property, material.GetColor(property) * Tint);
    }
}

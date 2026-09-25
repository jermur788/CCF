using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ScenarioSoundscapeClipBinding
{
    public string layerId = "";
    public AudioClip clip;
}

// Optional audio backend for the habitat-driven mix. Clips can be assigned by
// layer ID without changing biology or routing; empty bindings stay silent.
[DisallowMultipleComponent]
public sealed class ScenarioOneSoundscapePlayer : MonoBehaviour
{
    [SerializeField] private List<ScenarioSoundscapeClipBinding> clips = new List<ScenarioSoundscapeClipBinding>();
    [SerializeField, Range(0f, 1f)] private float masterVolume = 0.2f;
    [SerializeField, Min(0.05f)] private float fadeSeconds = 2f;

    private readonly Dictionary<string, AudioSource> sources = new Dictionary<string, AudioSource>(StringComparer.Ordinal);
    private readonly Dictionary<string, float> targets = new Dictionary<string, float>(StringComparer.Ordinal);

    public void Route(ScenarioSoundscapeState state)
    {
        foreach (string key in new List<string>(targets.Keys))
            targets[key] = 0f;
        if (state == null || state.layers == null)
            return;
        foreach (ScenarioSoundscapeLayer layer in state.layers)
        {
            if (layer == null || string.IsNullOrEmpty(layer.layerId))
                continue;
            targets[layer.layerId] = Mathf.Clamp01(layer.volume) * masterVolume;
            if (sources.ContainsKey(layer.layerId))
                continue;
            ScenarioSoundscapeClipBinding binding = clips.Find(item =>
                item != null && item.layerId == layer.layerId && item.clip != null);
            if (binding == null)
                continue;
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            source.volume = 0f;
            source.clip = binding.clip;
            source.Play();
            sources.Add(layer.layerId, source);
        }
    }

    private void Update()
    {
        foreach (KeyValuePair<string, AudioSource> pair in sources)
        {
            if (pair.Value == null)
                continue;
            float target = targets.TryGetValue(pair.Key, out float volume) ? volume : 0f;
            pair.Value.volume = Mathf.MoveTowards(pair.Value.volume, target,
                Time.unscaledDeltaTime * masterVolume / Mathf.Max(0.05f, fadeSeconds));
        }
    }
}

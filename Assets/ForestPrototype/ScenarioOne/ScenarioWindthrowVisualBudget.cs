using System.Collections.Generic;
using UnityEngine;

// One distance/budget pass for all fallen crowns, rather than one update/query
// per victim. Root plates and cheap authored stems retain their own LODs.
public sealed class ScenarioWindthrowVisualBudget : MonoBehaviour
{
    public const int MaximumCrowns = 20;
    public const float CrownDistanceMeters = 35f;
    private readonly List<ScenarioWindthrowVisual> visuals = new List<ScenarioWindthrowVisual>();
    private float nextUpdate;
    public int ActiveCrowns { get; private set; }
    public void Register(ScenarioWindthrowVisual visual) => visuals.Add(visual);
    public void Clear() { visuals.Clear(); ActiveCrowns = 0; }
    private void Update()
    {
        if (Time.unscaledTime < nextUpdate) return;
        nextUpdate = Time.unscaledTime + .5f;
        Camera camera = Camera.main;
        if (camera == null) return;
        RefreshAt(camera.transform.position);
    }
    public void RefreshAt(Vector3 viewer)
    {
        visuals.RemoveAll(visual => visual == null);
        visuals.Sort((a, b) => { int distance = (a.transform.position - viewer).sqrMagnitude.CompareTo((b.transform.position - viewer).sqrMagnitude);
            return distance != 0 ? distance : string.CompareOrdinal(a.TreeId, b.TreeId); });
        ActiveCrowns = 0;
        foreach (ScenarioWindthrowVisual visual in visuals)
        {
            bool near = ActiveCrowns < MaximumCrowns && (visual.transform.position - viewer).sqrMagnitude <= CrownDistanceMeters * CrownDistanceMeters;
            visual.SetCrownVisible(near);
            if (visual.HasCrown) ActiveCrowns++;
        }
    }
}

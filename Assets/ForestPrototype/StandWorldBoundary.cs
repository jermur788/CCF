using System.Collections.Generic;
using UnityEngine;

// Puts the physical property (ground, boundary ridges, camera range) into the geometry the ecology grid is in.
// The scene is authored as Legacy40 and stays that way on disk: this component records the authored transforms
// the first time it runs and restores them exactly for Legacy40, so Legacy40 worlds are bit-identical to before.
// For Enlarged80 it scales the same five objects to the 80 x 80 m property. It is added at runtime by
// ForestEcologyController (like other scenario components) and follows ForestEcologyController.StandGeometryApplied,
// so new-game setup, load, Reference preview and verification all use the one geometry-application path.
// There is no decorative forest outside the authoritative ecology bounds.
public sealed class StandWorldBoundary : MonoBehaviour
{
    private struct Authored
    {
        public Transform Transform;
        public Vector3 Position;
        public Vector3 Scale;
    }

    private const string GroundName = "Ground";
    private static readonly string[] RidgeNames = { "North Ridge", "South Ridge", "East Ridge", "West Ridge" };

    // The ground texture tiles over the box's UVs (Forest Ground.mat: 16 tiles over the authored 40 m). Keep the
    // same metres per tile when the box grows, per renderer, without touching the shared material asset.
    private const float AuthoredGroundTiles = 16f;
    private const float AuthoredGroundMeters = 40f;

    private ForestEcologyController ecology;
    private readonly Dictionary<string, Authored> authored = new Dictionary<string, Authored>();
    private Camera viewCamera;
    private float authoredFarClip;
    private MaterialPropertyBlock groundBlock;
    private int appliedModel = -1;

    private void Awake()
    {
        ecology = GetComponent<ForestEcologyController>();
        if (ecology == null)
            ecology = FindFirstObjectByType<ForestEcologyController>();
        Record(GroundName);
        foreach (string ridge in RidgeNames)
            Record(ridge);
        viewCamera = Camera.main;
        authoredFarClip = viewCamera != null ? viewCamera.farClipPlane : 0f;
        if (ecology != null)
        {
            ecology.StandGeometryApplied += Apply;
            Apply(ecology.StandGeometryModelVersion);
        }
    }

    private void OnDestroy()
    {
        if (ecology != null)
            ecology.StandGeometryApplied -= Apply;
    }

    private void Record(string objectName)
    {
        GameObject found = GameObject.Find(objectName);
        if (found == null)
        {
            Debug.LogWarning($"StandWorldBoundary: scene object '{objectName}' is missing; its geometry will not follow the stand.", this);
            return;
        }
        authored[objectName] = new Authored { Transform = found.transform, Position = found.transform.position, Scale = found.transform.localScale };
    }

    public void Apply(int model)
    {
        if (model == appliedModel)
            return;
        appliedModel = model;
        float size = StandGeometryModel.StandSizeMeters(model);
        float half = size * 0.5f;
        bool legacy = model == StandGeometryModel.Legacy40;

        if (authored.TryGetValue(GroundName, out Authored ground) && ground.Transform != null)
        {
            ground.Transform.position = ground.Position;
            ground.Transform.localScale = legacy ? ground.Scale : new Vector3(size, ground.Scale.y, size);
            ApplyGroundTiling(ground.Transform, legacy ? 0f : AuthoredGroundTiles * size / AuthoredGroundMeters);
        }
        // Ridges keep their authored height, thickness and the 2 m corner overlap; they move to the property edge.
        Place("North Ridge", legacy, new Vector3(0f, 0f, half), new Vector3(size + 2f, 0f, 0f), true);
        Place("South Ridge", legacy, new Vector3(0f, 0f, -half), new Vector3(size + 2f, 0f, 0f), true);
        Place("East Ridge", legacy, new Vector3(half, 0f, 0f), new Vector3(0f, 0f, size), false);
        Place("West Ridge", legacy, new Vector3(-half, 0f, 0f), new Vector3(0f, 0f, size), false);

        // The whole property must be visible from any point in it (diagonal of the stand plus a margin).
        if (viewCamera != null && authoredFarClip > 0f)
            viewCamera.farClipPlane = legacy ? authoredFarClip : Mathf.Max(authoredFarClip, size * 1.5f * 1.4143f);
    }

    private void Place(string ridgeName, bool legacy, Vector3 edgePosition, Vector3 length, bool alongX)
    {
        if (!authored.TryGetValue(ridgeName, out Authored ridge) || ridge.Transform == null)
            return;
        if (legacy)
        {
            ridge.Transform.position = ridge.Position;
            ridge.Transform.localScale = ridge.Scale;
            return;
        }
        ridge.Transform.position = new Vector3(edgePosition.x, ridge.Position.y, edgePosition.z);
        ridge.Transform.localScale = alongX
            ? new Vector3(length.x, ridge.Scale.y, ridge.Scale.z)
            : new Vector3(ridge.Scale.x, ridge.Scale.y, length.z);
    }

    private void ApplyGroundTiling(Transform ground, float tiles)
    {
        Renderer renderer = ground.GetComponent<Renderer>();
        if (renderer == null)
            return;
        if (tiles <= 0f)
        {
            renderer.SetPropertyBlock(null);
            return;
        }
        if (groundBlock == null)
            groundBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(groundBlock);
        groundBlock.SetVector("_BaseMap_ST", new Vector4(tiles, tiles, 0f, 0f));
        groundBlock.SetVector("_MainTex_ST", new Vector4(tiles, tiles, 0f, 0f));
        renderer.SetPropertyBlock(groundBlock);
    }
}

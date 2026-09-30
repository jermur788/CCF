#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RecentAssetReviewVerification
{
    private const string Requested = "CCF.RecentAssetReviewVerification";
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestryAssetReview.unity");
        EditorPrefs.SetBool(Requested, true);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("Review startup gate").AddComponent<RecentAssetReviewGate>();
    }
}

public sealed class RecentAssetReviewGate : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        Exception failure = null;
        try
        {
            ForestPlayer player = FindFirstObjectByType<ForestPlayer>();
            Camera camera = Camera.main;
            CharacterController body = player != null ? player.GetComponent<CharacterController>() : null;
            Require(player != null && player.enabled && camera != null && camera.transform.IsChildOf(player.transform)
                && body != null && body.enabled, "Review walking/camera did not initialize");
            Require((body.Move(Vector3.down * 0.3f) & CollisionFlags.Below) != 0,
                "Review player did not stand on collision-enabled ground");
            Require(FindObjectsByType<LODGroup>(FindObjectsSortMode.None).Length == 11,
                "Review specimens missing at runtime");
            Require(FindObjectsByType<Renderer>(FindObjectsSortMode.None).All(renderer =>
                renderer.sharedMaterials.All(material => material != null && material.shader != null)),
                "Review contains missing material/shader");
            Require(FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Length == 0
                && FindFirstObjectByType<ForestEcologyController>() == null
                && FindFirstObjectByType<ForestSaveController>() == null,
                "Review specimens entered authoritative simulation or saves");
            Require(FindFirstObjectByType<RecentAssetReviewControls>() != null,
                "Review LOD controls unavailable");
            Debug.Log("RECENT_ASSET_REVIEW_PLAY_PASS walkingGround=True camera=True specimens=11 forestRecords=0");
        }
        catch (Exception error) { failure = error; Debug.LogError("RECENT_ASSET_REVIEW_PLAY_FAIL: " + error); }
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private static void Require(bool success, string message)
    {
        if (!success) throw new InvalidOperationException(message);
    }
}
#endif

using System.Collections.Generic;
using UnityEngine;

// The forest track's approach from outside the 80 m property (Enlarged80 only), so it no longer begins out of nothing in the middle of
// the plantation. Like StandWorldBoundary, the scene stays the authored Legacy40 baseline on disk: this adds the extra pieces at run time
// for Enlarged80 and removes them again for Legacy40, so Legacy40 worlds and the frozen Reference Future are untouched.
//
// What it adds (all under one object, "Stand Approach"):
//   * "Dirt Path X000"...: more track planks from the authored south end to the property edge. They share the "Dirt Path" prefix on
//     purpose: the starting stand keeps trees clear of every such plank and the Stand Map draws them, so the world and the map agree
//     without either being changed. "X" sorts after the authored "Dirt Path 00".."51", so the map reads them as one more line.
//   * an opening in the south boundary ridge where the track meets it (ShapeSouthRidge, called by StandWorldBoundary), and beyond the
//     opening a short walled cutting with ground and the rest of the track, so the track is seen arriving from outside the property.
//   * an invisible blocker across the opening. The cutting is scenery: the player stays inside the ecology bounds as before.
//
// The planks are made inside ForestEcologyController.ApplyStandGeometry, before anything reads them, so a new game plants around them
// and a load or Reference preview sees the right world whichever object wakes first. The route itself is StandApproachRoute.
public static class StandApproachPath
{
    private const string RootName = "Stand Approach";
    private const string TemplatePlankName = "Dirt Path 00";
    private const string InsidePlankPrefix = "Dirt Path X";
    private const string OutsidePlankPrefix = "Approach Track ";
    private const string EastRidgePieceName = "South Ridge (east of gate)";
    private const float MetresPerGroundTile = 2.5f;

    public static void Apply(int geometryModel)
    {
        GameObject existing = GameObject.Find(RootName);
        if (geometryModel != StandGeometryModel.Enlarged80)
        {
            Remove(existing);
            return;
        }
        // Run time only: building these in Edit Mode would leave extra objects in the authored scene.
        if (existing != null || !Application.isPlaying)
            return;
        GameObject template = GameObject.Find(TemplatePlankName);
        if (template == null)
            return;

        float edgeZ = -StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) * 0.5f;
        List<StandApproachRoute.Point> route = StandApproachRoute.Build(
            template.transform.position.x, template.transform.position.z, template.transform.eulerAngles.y * Mathf.Deg2Rad);
        var root = new GameObject(RootName);

        // The track. The first point is the authored end itself, so the map's two lines meet; it sits a hair higher so the two
        // planks do not fight over the same pixels.
        int inside = 0, outside = 0;
        for (int i = 0; i < route.Count; i++)
        {
            StandApproachRoute.Point p = route[i];
            bool isInside = StandApproachRoute.InsideProperty(p, edgeZ);
            GameObject plank = Object.Instantiate(template, root.transform);
            plank.name = isInside ? InsidePlankPrefix + (inside++).ToString("D3") : OutsidePlankPrefix + (outside++).ToString("D3");
            plank.transform.SetPositionAndRotation(
                new Vector3(p.X, template.transform.position.y + (i == 0 ? 0.001f : 0f), p.Z),
                Quaternion.Euler(0f, p.BearingRadians * Mathf.Rad2Deg, 0f));
            plank.transform.localScale = template.transform.localScale;
        }

        BuildCutting(root.transform, StandApproachRoute.GateCentreX(route, edgeZ), edgeZ);
    }

    // The centre of the opening in the south ridge, or false when there is no track to meet it.
    public static bool TryGetGate(out float centreX)
    {
        centreX = 0f;
        GameObject template = GameObject.Find(TemplatePlankName);
        if (template == null)
            return false;
        float edgeZ = -StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) * 0.5f;
        centreX = StandApproachRoute.GateCentreX(StandApproachRoute.Build(
            template.transform.position.x, template.transform.position.z, template.transform.eulerAngles.y * Mathf.Deg2Rad), edgeZ);
        return true;
    }

    // Called by StandWorldBoundary right after it has put the south ridge back at full length for Enlarged80: the authored ridge becomes
    // the west piece and a copy the east piece, with the opening between them.
    public static void ShapeSouthRidge(Transform southRidge)
    {
        GameObject root = GameObject.Find(RootName);
        if (southRidge == null || root == null || !TryGetGate(out float gateX))
            return;
        float half = StandApproachRoute.GateWidthM * 0.5f;
        float min = southRidge.position.x - southRidge.localScale.x * 0.5f;
        float max = southRidge.position.x + southRidge.localScale.x * 0.5f;
        GameObject east = GameObject.Find(EastRidgePieceName);
        if (east == null)
        {
            east = Object.Instantiate(southRidge.gameObject, root.transform);
            east.name = EastRidgePieceName;
        }
        SetSpan(southRidge, min, gateX - half);
        SetSpan(east.transform, gateX + half, max);
    }

    // The walled cutting beyond the opening: ground, two side walls and an end wall in the boundary ridge's own material, and the
    // invisible blocker that keeps the player inside the property.
    private static void BuildCutting(Transform parent, float gateX, float edgeZ)
    {
        float width = StandApproachRoute.GateWidthM, half = width * 0.5f;
        float length = StandApproachRoute.ForecourtLengthM, thickness = StandApproachRoute.RidgeThicknessM;
        GameObject ground = GameObject.Find("Ground");
        GameObject ridge = GameObject.Find("East Ridge");

        if (ground != null)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Approach Ground";
            floor.transform.SetParent(parent, false);
            floor.transform.position = new Vector3(gateX, ground.transform.position.y, edgeZ - length * 0.5f);
            floor.transform.localScale = new Vector3(width, ground.transform.localScale.y, length);
            Object.Destroy(floor.GetComponent<Collider>());
            Renderer floorRenderer = floor.GetComponent<Renderer>();
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (floorRenderer != null && groundRenderer != null)
            {
                floorRenderer.sharedMaterial = groundRenderer.sharedMaterial;
                var block = new MaterialPropertyBlock();
                var tiling = new Vector4(width / MetresPerGroundTile, length / MetresPerGroundTile, 0f, 0f);
                block.SetVector("_BaseMap_ST", tiling);
                block.SetVector("_MainTex_ST", tiling);
                floorRenderer.SetPropertyBlock(block);
            }
        }

        if (ridge != null)
        {
            float wallZ = edgeZ - thickness * 0.5f - length * 0.5f;
            float y = ridge.transform.position.y, height = ridge.transform.localScale.y;
            AddWall(parent, ridge, "Approach Wall West", new Vector3(gateX - half - thickness * 0.5f, y, wallZ), new Vector3(thickness, height, length));
            AddWall(parent, ridge, "Approach Wall East", new Vector3(gateX + half + thickness * 0.5f, y, wallZ), new Vector3(thickness, height, length));
            AddWall(parent, ridge, "Approach Wall End", new Vector3(gateX, y, edgeZ - thickness - length), new Vector3(width + 2f * thickness, height, thickness));
        }

        var blocker = new GameObject("Approach Gate Blocker");
        blocker.transform.SetParent(parent, false);
        blocker.transform.position = new Vector3(gateX, 2.5f, edgeZ);
        var box = blocker.AddComponent<BoxCollider>();
        box.size = new Vector3(width, 5f, thickness);
    }

    private static void AddWall(Transform parent, GameObject ridge, string wallName, Vector3 position, Vector3 scale)
    {
        GameObject wall = Object.Instantiate(ridge, parent);
        wall.name = wallName;
        wall.transform.position = position;
        wall.transform.localScale = scale;
    }

    private static void SetSpan(Transform ridge, float min, float max)
    {
        ridge.position = new Vector3((min + max) * 0.5f, ridge.position.y, ridge.position.z);
        ridge.localScale = new Vector3(max - min, ridge.localScale.y, ridge.localScale.z);
    }

    // Switched off first, then destroyed at the end of the frame: a second Apply in the same frame (a geometry change back and forth)
    // must not find the old pieces still standing, nor have them queried by the starting stand or the map.
    private static void Remove(GameObject root)
    {
        if (root == null)
            return;
        root.SetActive(false);
        root.name = RootName + " (removed)";
        if (Application.isPlaying)
            Object.Destroy(root);
        else
            Object.DestroyImmediate(root);
    }
}

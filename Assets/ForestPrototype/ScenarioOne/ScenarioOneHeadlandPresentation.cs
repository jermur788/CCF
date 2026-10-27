using System.Collections.Generic;
using UnityEngine;

// The track is only a ground surface. The existing ground supplies the collider;
// no invisible road walls or extra raised collision geometry are introduced.
public sealed class ScenarioOneHeadlandPresentation : MonoBehaviour
{
    private ScenarioOneManager scenario;
    private GameObject surface;

    private void Start()
    {
        scenario = UnityEngine.Object.FindFirstObjectByType<ScenarioOneManager>();
        Reconcile();
    }

    private void LateUpdate() => Reconcile();

    private void Reconcile()
    {
        bool active = scenario != null && scenario.RackPlanningV2Active;
        if (active && surface == null)
            surface = CreateSurface();
        if (surface != null && surface.activeSelf != active)
            surface.SetActive(active);
    }

    private GameObject CreateSurface()
    {
        GameObject template = GameObject.Find("Dirt Path 00");
        if (template == null || template.GetComponent<Renderer>() == null)
            return null;
        float y = template.transform.position.y + template.transform.localScale.y * 0.5f + 0.001f;
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        float x = ScenarioOneHeadlandGeometry.HeadlandX;
        float south = ScenarioOneHeadlandGeometry.SouthZ, north = ScenarioOneHeadlandGeometry.NorthZ;
        float feeder = ScenarioOneHeadlandGeometry.FeederZ, path = ScenarioOneHeadlandGeometry.PathX;
        float radius = ScenarioOneHeadlandGeometry.Width * 0.5f;
        AddRectangle(vertices, uv, triangles, x - radius, x + radius, south, north, y);
        AddDisc(vertices, uv, triangles, x, south, radius, y + 0.0001f);
        AddDisc(vertices, uv, triangles, x, north, radius, y + 0.0001f);
        AddRectangle(vertices, uv, triangles, x, path, feeder - radius, feeder + radius, y + 0.0002f);
        AddDisc(vertices, uv, triangles, x, feeder, ScenarioOneHeadlandGeometry.PadRadius, y + 0.0004f);
        AddDisc(vertices, uv, triangles, path, feeder, ScenarioOneHeadlandGeometry.PadRadius, y + 0.0004f);

        var root = new GameObject("Scenario One V2 Headland");
        root.transform.SetParent(transform, false);
        var mesh = new Mesh { name = "Scenario One V2 Headland Surface" };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        root.AddComponent<MeshFilter>().sharedMesh = mesh;
        root.AddComponent<MeshRenderer>().sharedMaterial = template.GetComponent<Renderer>().sharedMaterial;
        return root;
    }

    private static void AddVertex(List<Vector3> vertices, List<Vector2> uv, float x, float z, float y)
    {
        vertices.Add(new Vector3(x, y, z));
        uv.Add(new Vector2(x / 2.5f, z / 2.5f));
    }

    private static void AddRectangle(List<Vector3> vertices, List<Vector2> uv, List<int> triangles,
        float minX, float maxX, float minZ, float maxZ, float y)
    {
        int first = vertices.Count;
        AddVertex(vertices, uv, minX, minZ, y); AddVertex(vertices, uv, minX, maxZ, y);
        AddVertex(vertices, uv, maxX, maxZ, y); AddVertex(vertices, uv, maxX, minZ, y);
        triangles.Add(first); triangles.Add(first + 1); triangles.Add(first + 2);
        triangles.Add(first); triangles.Add(first + 2); triangles.Add(first + 3);
    }

    private static void AddDisc(List<Vector3> vertices, List<Vector2> uv, List<int> triangles,
        float x, float z, float radius, float y)
    {
        const int segments = 48;
        int centre = vertices.Count;
        AddVertex(vertices, uv, x, z, y);
        for (int i = 0; i < segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            AddVertex(vertices, uv, x + radius * Mathf.Cos(angle), z + radius * Mathf.Sin(angle), y);
        }
        for (int i = 0; i < segments; i++)
        {
            triangles.Add(centre); triangles.Add(centre + 1 + (i + 1) % segments); triangles.Add(centre + 1 + i);
        }
    }

    private void OnDestroy()
    {
        if (surface != null)
        {
            MeshFilter filter = surface.GetComponent<MeshFilter>();
            if (filter != null && filter.sharedMesh != null)
                Destroy(filter.sharedMesh);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class MeshSplit : MonoBehaviour
{
    public struct GridCoordinates
    {
        private static readonly float precision = 100f;

        public int x;

        public int y;

        public int z;

        public GridCoordinates(float x, float y, float z)
        {
            this.x = Mathf.RoundToInt(x * precision);
            this.y = Mathf.RoundToInt(y * precision);
            this.z = Mathf.RoundToInt(z * precision);
        }

        public static implicit operator GridCoordinates(Vector3 v)
        {
            return new GridCoordinates((int)v.x, (int)v.y, (int)v.z);
        }

        public static implicit operator Vector3(GridCoordinates i)
        {
            return new Vector3(i.x, i.y, i.z);
        }

        public override string ToString()
        {
            return $"({(float)x / precision},{(float)y / precision},{(float)z / precision})";
        }
    }

    private readonly bool drawGrid;

    private Mesh baseMesh;

    private MeshRenderer baseRenderer;

    [Range(0.1f, 64f)]
    public float gridSize = 16f;

    public bool axisX = true;

    public bool axisY = true;

    public bool axisZ = true;

    public int renderLayerIndex;

    public string renderLayerName = "Default";

    public bool useSortingLayerFromThisMesh = true;

    public bool useStaticSettingsFromThisMesh = true;

    private Vector3[] baseVerticles;

    private int[] baseTriangles;

    private Vector2[] baseUvs;

    private Vector3[] baseNormals;

    private Dictionary<GridCoordinates, List<int>> triDictionary;

    [HideInInspector]
    public List<GameObject> childen = new List<GameObject>();

    private void MapTrianglesToGridNodes()
    {
        triDictionary = new Dictionary<GridCoordinates, List<int>>();
        GridCoordinates key = default(GridCoordinates);
        for (int i = 0; i < baseTriangles.Length; i += 3)
        {
            Vector3 vector = (baseVerticles[baseTriangles[i]] + baseVerticles[baseTriangles[i + 1]] + baseVerticles[baseTriangles[i + 2]]) / 3f;
            vector.x = Mathf.Round(vector.x / gridSize) * gridSize;
            vector.y = Mathf.Round(vector.y / gridSize) * gridSize;
            vector.z = Mathf.Round(vector.z / gridSize) * gridSize;
            key = new GridCoordinates((!axisX) ? 0f : vector.x, (!axisY) ? 0f : vector.y, (!axisZ) ? 0f : vector.z);
            if (!triDictionary.ContainsKey(key))
            {
                triDictionary.Add(key, new List<int>());
            }
            triDictionary[key].Add(baseTriangles[i]);
            triDictionary[key].Add(baseTriangles[i + 1]);
            triDictionary[key].Add(baseTriangles[i + 2]);
        }
    }

    public void Split()
    {
        DestroyChildren();
        if (GetComponent<MeshFilter>() == null)
        {
            UnityEngine.Debug.LogError("Mesh Filter Component is missing.");
            return;
        }
        if (GetUsedAxisCount() < 1)
        {
            UnityEngine.Debug.LogError("You have to choose at least 1 axis.");
            return;
        }
        baseMesh = GetComponent<MeshFilter>().sharedMesh;
        baseRenderer = GetComponent<MeshRenderer>();
        if ((bool)baseRenderer)
        {
            baseRenderer.enabled = false;
        }
        baseVerticles = baseMesh.vertices;
        baseTriangles = baseMesh.triangles;
        baseUvs = baseMesh.uv;
        baseNormals = baseMesh.normals;
        MapTrianglesToGridNodes();
        foreach (GridCoordinates key in triDictionary.Keys)
        {
            CreateMesh(key, triDictionary[key]);
        }
    }

    public void CreateMesh(GridCoordinates gridCoordinates, List<int> dictionaryTriangles)
    {
        GameObject gameObject = new GameObject();
        gameObject.name = "SubMesh " + gridCoordinates;
        gameObject.transform.SetParent(base.transform);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localRotation = base.transform.localRotation;
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
        component.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        if (!useSortingLayerFromThisMesh)
        {
            component.sortingLayerName = renderLayerName;
            component.sortingOrder = renderLayerIndex;
        }
        else if ((bool)baseRenderer)
        {
            component.sortingLayerName = baseRenderer.sortingLayerName;
            component.sortingOrder = baseRenderer.sortingOrder;
        }
        if (useStaticSettingsFromThisMesh)
        {
            gameObject.isStatic = base.gameObject.isStatic;
        }
        List<Vector3> list = new List<Vector3>();
        List<int> list2 = new List<int>();
        List<Vector2> list3 = new List<Vector2>();
        List<Vector3> list4 = new List<Vector3>();
        for (int i = 0; i < dictionaryTriangles.Count; i += 3)
        {
            list.Add(baseVerticles[dictionaryTriangles[i]]);
            list.Add(baseVerticles[dictionaryTriangles[i + 1]]);
            list.Add(baseVerticles[dictionaryTriangles[i + 2]]);
            list2.Add(i);
            list2.Add(i + 1);
            list2.Add(i + 2);
            list3.Add(baseUvs[dictionaryTriangles[i]]);
            list3.Add(baseUvs[dictionaryTriangles[i + 1]]);
            list3.Add(baseUvs[dictionaryTriangles[i + 2]]);
            list4.Add(baseNormals[dictionaryTriangles[i]]);
            list4.Add(baseNormals[dictionaryTriangles[i + 1]]);
            list4.Add(baseNormals[dictionaryTriangles[i + 2]]);
        }
        childen.Add(gameObject);
        Mesh mesh = new Mesh();
        mesh.name = gridCoordinates.ToString();
        mesh.vertices = list.ToArray();
        mesh.triangles = list2.ToArray();
        mesh.uv = list3.ToArray();
        mesh.normals = list4.ToArray();
        MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
        component2.mesh = mesh;
    }

    private int GetUsedAxisCount()
    {
        return (axisX ? 1 : 0) + (axisY ? 1 : 0) + (axisZ ? 1 : 0);
    }

    public void Clear()
    {
        DestroyChildren();
        GetComponent<MeshRenderer>().enabled = true;
    }

    private void DestroyChildren()
    {
        for (int i = 0; i < childen.Count; i++)
        {
            UnityEngine.Object.DestroyImmediate(childen[i]);
        }
        childen.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        MeshFilter component = GetComponent<MeshFilter>();
        if (!drawGrid || !component || !component.sharedMesh)
        {
            return;
        }
        Bounds bounds = component.sharedMesh.bounds;
        Vector3 extents = bounds.extents;
        float num = Mathf.Ceil(extents.x) + gridSize;
        Vector3 extents2 = bounds.extents;
        float num2 = Mathf.Ceil(extents2.y) + gridSize;
        Vector3 extents3 = bounds.extents;
        float num3 = Mathf.Ceil(extents3.z) + gridSize;
        for (float num4 = 0f - num3; num4 <= num3; num4 += gridSize)
        {
            for (float num5 = 0f - num2; num5 <= num2; num5 += gridSize)
            {
                for (float num6 = 0f - num; num6 <= num; num6 += gridSize)
                {
                    Vector3 center = base.transform.position + new Vector3(num6, num5, num4);
                    Gizmos.DrawWireCube(center, gridSize * base.transform.localScale);
                }
            }
        }
    }
}

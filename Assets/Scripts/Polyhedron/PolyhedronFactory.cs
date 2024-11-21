using UnityEngine;

public class PolyhedronFactory : MonoBehaviour
{
    public Polyhedron polyhedronTemplate;

    public void Start()
    {
        Polyhedron polyhedron = GenerateRandomPolyhedron();

        if (polyhedron != null)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();

            polyhedron.initialing();

            mesh.vertices = polyhedron.vertices.ToArray();
            mesh.triangles = polyhedron.triangles.ToArray();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
        else
        {
            Debug.LogError("err");
        }
    }

    public Polyhedron GenerateRandomPolyhedron()
    {
        Polyhedron polyhedron = ScriptableObject.CreateInstance<Polyhedron>();

        // 生成随机顶点
        for (int i = 0; i < Random.Range(4, 20); i++)
        {
            polyhedron.AddPoint(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            );
        }

        return polyhedron;
    }
}

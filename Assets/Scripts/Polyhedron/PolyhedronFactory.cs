using UnityEngine;

public class PolyhedronFactory : MonoBehaviour
{
    public Polyhedron polyhedronTemplate;

    public void Start()
    {
        if (gameObject.GetComponent<Rigidbody>() != null)
            return;
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

            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true; 
        }
        else
        {
            Debug.LogError("err");
        }
    }

    public Polyhedron GenerateRandomPolyhedron()
    {
        Polyhedron polyhedron = ScriptableObject.CreateInstance<Polyhedron>();

        for (int i = 0; i < Random.Range(4,12); i++)
        {
            polyhedron.AddPoint(
                Random.Range(-1f, 1f), 
                Random.Range(0f, 1f),
                Random.Range(-0.7f, 1.5f)   
            );
        }
        return polyhedron;
    }
}

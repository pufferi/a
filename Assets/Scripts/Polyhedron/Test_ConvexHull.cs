using System.Collections.Generic;
using UnityEngine;
using Habrador_Computational_Geometry;

public class Test_ConvexHull : MonoBehaviour
{
    public HashSet<MyVector3> points = new HashSet<MyVector3>();
    void AddPoint(float x, float y, float z)
    {
        MyVector3 newPoint = new MyVector3(x, y, z);
        points.Add(newPoint);
    }

    void Start()
    {
        AddPoint(0, 0, 0);
        AddPoint(1, 0, 0);
        AddPoint(0, 1, 0);
        AddPoint(0, 0, 1);
        AddPoint(1, 1, 1);
        AddPoint(0.5f, 1, 1);

        if (points == null || points.Count < 4)
        {
            Debug.LogError("4 points needed");
            return;
        }

        // 计算 3D 凸包
        HalfEdgeData3 hull = _ConvexHull.Iterative_3D(points, true);

        if (hull != null)
        {
            Mesh mesh = new Mesh();

            // 提取顶点
            List<Vector3> vertices = new List<Vector3>();
            Dictionary<HalfEdgeVertex3, int> vertexIndexMapping = new Dictionary<HalfEdgeVertex3, int>();

            int index = 0;
            foreach (var vertex in hull.verts)
            {
                vertices.Add(new Vector3(vertex.position.x, vertex.position.y, vertex.position.z));
                vertexIndexMapping[vertex] = index++;
            }

            // 提取三角形
            List<int> triangles = new List<int>();
            foreach (var face in hull.faces)
            {
                HalfEdge3 edge = face.edge;
                do
                {
                    triangles.Add(vertexIndexMapping[edge.v]);
                    edge = edge.nextEdge;
                } while (edge != face.edge);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
        else
        {
            Debug.LogError("err");
        }
    }
}

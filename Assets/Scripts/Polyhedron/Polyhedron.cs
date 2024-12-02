using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Polyhedron", menuName = "ScriptableObjects/Polyhedron", order = 1)]
public class Polyhedron : ScriptableObject
{
    
    public HashSet<MyVector3> points = new HashSet<MyVector3>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
   //meshCenter应该放在这里，，，好像不用Mesh,我有现成的points,,
   //然后我的playerMOvement的限制距离也要拿这个meshCenter计算，，，或者再算一个max距离，，
    public static Vector3 meshCenter = Vector3.zero;

    public void AddPoint(float x, float y, float z)
    {
        MyVector3 newPoint = new MyVector3(x, y, z);
        points.Add(newPoint);
    }

    public Mesh initialing()
    {
        Mesh mesh = new Mesh();
        if (points.Count < 4)
        {
            Debug.LogError("4 points needed");
            return null;
        }

        // 计算 3D 凸包
        HalfEdgeData3 hull = _ConvexHull.Iterative_3D(points, true);

        if (hull != null)
        {

            // 提取顶点
            Dictionary<HalfEdgeVertex3, int> vertexIndexMapping = new Dictionary<HalfEdgeVertex3, int>();

            int index = 0;
            foreach (var vertex in hull.verts)
            {
                vertices.Add(new Vector3(vertex.position.x, vertex.position.y, vertex.position.z));
                vertexIndexMapping[vertex] = index++;
            }

            // 提取三角形
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

        }
        else
        {
            Debug.LogError("err");
        }
        return mesh;
    }



}

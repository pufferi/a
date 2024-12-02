using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 MeshCenter;

    public void Grab()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 center = GetMeshCenter();

        // 质心
        GameObject grabCenter = new GameObject("GrabCenter");
        grabCenter.transform.position = center;
        transform.SetParent(grabCenter.transform);

        rb.isKinematic = true; // 使物品在抓取时不受物理影响
        grabCenter.transform.SetParent(GameObject.FindWithTag("MainCamera").transform);
    }

    public void Release()
    {
        rb.isKinematic = false; // 使物品在释放时恢复物理影响
        transform.SetParent(null); 
    }


    private Vector3 GetMeshCenter()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return transform.position;

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        Vector3 center = Vector3.zero;
        foreach (Vector3 vertex in vertices)
        {
            center += vertex;
        }
        center /= vertices.Length;

        // 转换到世界坐标系
        center = transform.TransformPoint(center);

        return center;
    }

}

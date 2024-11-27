using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    private Rigidbody rb;

    public void Grab()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 center = GetMeshCenter();

        // ���������������Ϊ�µĸ�����ʹ������λ��Ϊץȡ��
        GameObject grabCenter = new GameObject("GrabCenter");
        grabCenter.transform.position = center;
        transform.SetParent(grabCenter.transform);

        rb.isKinematic = true; // ʹ��Ʒ��ץȡʱ��������Ӱ��
        grabCenter.transform.SetParent(GameObject.FindWithTag("MainCamera").transform);
    }

    public void Release()
    {
        rb.isKinematic = false; // ʹ��Ʒ���ͷ�ʱ�ָ�����Ӱ��
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

        // ת������������ϵ
        center = transform.TransformPoint(center);

        return center;
    }
}

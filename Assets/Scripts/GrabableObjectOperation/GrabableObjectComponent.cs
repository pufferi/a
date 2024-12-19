using UnityEngine;
using UnityEngine.XR;

public class GrabableObjectComponent : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 MeshCenter;
    private GameObject grabCenter;
    public LayerMask groundLayer;
    public float minimumHeightAboveGround = 0.1f; // �������������С�߶�

    private void Update()
    {
        AvoidingObjectPenetratingTheFloor();
    }

    private void AvoidingObjectPenetratingTheFloor()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return;
        if(transform.position.y<-0.2)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //Vector3 bottomCenter = GetColliderBottomCenter(collider);
        ////���bottomCenter�ȵذ尫0.2m����ô����Ʒ�ĸ߶ȵ����ɺ͵ذ�һ����
        //if (bottomCenter.y < -0.3)
        //{
        //    
        //    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //}
    }

    //private Vector3 GetColliderBottomCenter(Collider collider)
    //{
    //    if (collider is BoxCollider boxCollider)
    //    {
    //        return boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0);
    //    }
    //    else if (collider is SphereCollider sphereCollider)
    //    {
    //        return sphereCollider.bounds.center - new Vector3(0, sphereCollider.radius, 0);
    //    }
    //    else if (collider is CapsuleCollider capsuleCollider)
    //    {
    //        return capsuleCollider.bounds.center - new Vector3(0, capsuleCollider.height / 2, 0);
    //    }
    //    else if (collider is MeshCollider meshCollider)
    //    {
    //        return meshCollider.bounds.center - new Vector3(0, meshCollider.bounds.extents.y, 0);
    //    }
    //    else
    //    {
    //        return collider.bounds.center - new Vector3(0, collider.bounds.extents.y, 0);
    //    }
    //}

    public void Grab()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 center = GetMeshCenter();

        // ����ץȡ����
        grabCenter = new GameObject("GrabCenter");
        grabCenter.transform.position = center;
        transform.SetParent(grabCenter.transform);

        rb.isKinematic = true;
        grabCenter.transform.SetParent(GameObject.FindWithTag("MainCamera").transform);
    }

    public void Release()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        if (grabCenter != null)
        {
            Destroy(grabCenter);
            grabCenter = null;
        }
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

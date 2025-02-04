using UnityEngine;
using UnityEngine.XR;

public class GrabableObjectComponent : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 MeshCenter;
    private GameObject grabCenter;
    public LayerMask groundLayer;
    public LayerMask Layer_DontTouchPlayer;

    private void Update()
    {
        AvoidingObjectPenetratingTheFloor();
    }

    //private void AvoidingObjectPenetratingTheFloor()
    //{
    //    Collider collider = GetComponent<Collider>();
    //    if (collider == null) return;

    //    RaycastHit hit;
    //    float distanceToGround = collider.bounds.extents.y*0.4f;
    //    Vector3 origin = transform.position;//- Vector3.up * collider.bounds.extents.y*0.5f;

    //    if (Physics.Raycast(origin, Vector3.down, out hit, distanceToGround))
    //    {
    //        //Debug.Log("okokokookkkkkkrrr");
    //        float offset = distanceToGround - hit.distance;
    //        transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
    //    }
    //}
    private void AvoidingObjectPenetratingTheFloor()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return;

        RaycastHit hit;
        float distanceToGround = collider.bounds.extents.y * 0.3f;
        Vector3 origin = transform.position;

        if (Physics.Raycast(origin, Vector3.down, out hit, distanceToGround))
        {
            if (hit.collider.tag == "ground") 
            {
                float offset = distanceToGround - hit.distance;
                transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
            }

            //Debug.Log("okokokookkkkkkrrr");
        }
    }



    public void Grab()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 center = GetMeshCenter();

        grabCenter = new GameObject("GrabCenter");
        grabCenter.transform.position = center;
        transform.SetParent(grabCenter.transform);
        int layer = Mathf.RoundToInt(Mathf.Log(Layer_DontTouchPlayer.value, 2));
        gameObject.layer = layer;


        rb.isKinematic = true;
        grabCenter.transform.SetParent(GameObject.FindWithTag("MainCamera").transform);
    }

    public void Release()
    {
        gameObject.layer = 0;

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

        // 转换到世界坐标系
        center = transform.TransformPoint(center);
        return center;
    }
}
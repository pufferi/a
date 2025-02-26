using System.Collections.Generic;
using UnityEngine;

public class GrabableObjectComponent : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 MeshCenter;
    public GameObject grabCenter;
    public LayerMask groundLayer;
    public LayerMask Layer_DontTouchPlayer;
    public int objID;
    public int groupID = -1; // separate object

    private void Start()
    {
        GrabableObejectGroupingManager.Instance.AssignObjectID(this);
    }

    private void Update()
    {
        //AdjustHeightIfBelowGround();
    }

    private void AdjustHeightIfBelowGround()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
            return;

        // 计算物体的最低点
        Vector3 lowestPoint = transform.position - new Vector3(0, collider.bounds.extents.y, 0);

        // 发射一条从最低点向下的射线，以检测是否接触地面
        RaycastHit hit;
        if (Physics.Raycast(lowestPoint, Vector3.down, out hit, 0.1f, groundLayer))
        {
            // 计算需要提升的高度
            float offset = hit.distance - collider.bounds.extents.y;
            if (offset < 0)
            {
                Vector3 newPosition = transform.position;
                newPosition.y += -offset;
                transform.position = newPosition;
            }
        }
    }

 
  public void Grab()
    {
        rb = GetComponent<Rigidbody>();

        grabCenter = new GameObject("GrabCenter");
        // Removed setting the parent to MainCamera to prevent unwanted transformations
        // grabCenter.transform.SetParent(GameObject.FindWithTag("MainCamera").transform);
        grabCenter.transform.position = this.transform.position;

        // Use a FixedJoint instead of setting isKinematic
        FixedJoint joint = grabCenter.AddComponent<FixedJoint>();
        joint.connectedBody = rb;

        int layer = Mathf.RoundToInt(Mathf.Log(Layer_DontTouchPlayer.value, 2));
        gameObject.layer = layer;

        if (this.objID < 0)
            return;
        List<GrabableObjectComponent> AllConnect = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(this);
        foreach (var obj in AllConnect)
            obj.gameObject.layer = layer;
    }

    public virtual void Release()
    {
        if (grabCenter != null)
        {
            FixedJoint joint = grabCenter.GetComponent<FixedJoint>();
            if (joint != null)
            {
                Destroy(joint);
            }
            Destroy(grabCenter);
            grabCenter = null;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        int originalLayer = Mathf.RoundToInt(Mathf.Log(Layer_DontTouchPlayer.value, 2));
        gameObject.layer = originalLayer;

        if (this.objID >= 0)
        {
            List<GrabableObjectComponent> allConnected = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(this);
            foreach (var obj in allConnected)
            {
                obj.gameObject.layer = originalLayer;
            }
        }

        // Additional release functionality if needed
    }



}

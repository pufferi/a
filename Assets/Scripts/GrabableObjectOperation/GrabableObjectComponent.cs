using System.Collections.Generic;
using UnityEngine;

public class GrabableObjectComponent : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 MeshCenter;
    private GameObject grabCenter;
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

        // �����������͵�
        Vector3 lowestPoint = transform.position - new Vector3(0, collider.bounds.extents.y, 0);

        // ����һ������͵����µ����ߣ��Լ���Ƿ�Ӵ�����
        RaycastHit hit;
        if (Physics.Raycast(lowestPoint, Vector3.down, out hit, 0.1f, groundLayer))
        {
            // ������Ҫ�����ĸ߶�
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
        transform.SetParent(grabCenter.transform);
        int layer = Mathf.RoundToInt(Mathf.Log(Layer_DontTouchPlayer.value, 2));
        gameObject.layer = layer;
        rb.isKinematic = true;
        grabCenter.transform.SetParent(GameObject.FindWithTag("MainCamera").transform);

        if (this.objID < 0)
            return;
        List<GrabableObjectComponent> AllConnect = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(this);
        foreach (var obj in AllConnect)
            obj.gameObject.layer = layer;
    }

    public virtual void Release()
    {
        gameObject.layer = 0;

        rb.isKinematic = false;
        transform.SetParent(null);
        if (grabCenter != null)
        {
            Destroy(grabCenter);
            grabCenter = null;
        }
        if (this.objID < 0)
            return;

        List<GrabableObjectComponent> AllConnect = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(this);
        foreach (var obj in AllConnect)
            obj.gameObject.layer = 0;
    }
}
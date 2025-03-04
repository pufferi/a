﻿using System.Collections.Generic;
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
        if (transform.position.y < -0.5f)
        {
            transform.position = new Vector3(transform.position.x, -0.3f, transform.position.z);
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
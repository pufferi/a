using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishRod : GrabableObjectComponent
{
    public bool isRodInHand = false;

    public Vector3 rodRotation=Vector3.zero;

    public Transform hand;

    public Vector3 positionOffset = Vector3.zero;

    public PlayerGrabItems playerGrabItems;


    private void Start()
    {
        this.objID = -2;
    }
    

}

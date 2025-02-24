using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InFishGameArea : MonoBehaviour
{
    private bool inArea = false;

    public GameObject fishingTip0;
    public GameObject fishingTip1;

    private int tipTime = 5;
    public PlayerGrabItems playerGrabItems;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerGrabItems.grabbedObject != null && playerGrabItems.grabbedObject.objID == -2)
        {
            if (tipTime > 0)
            {
                fishingTip0.SetActive(true);
                tipTime--;
            }
            else fishingTip1.SetActive(true);
            inArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fishingTip0.SetActive(false);
            fishingTip1.SetActive(false);
            inArea = false;
        }
    }

    public bool IsInArea()
    {
        return inArea;
    }
}

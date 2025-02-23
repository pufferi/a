using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InFishGameArea : MonoBehaviour
{
    private bool inArea = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inArea = false;
        }
    }

    public bool IsInArea()
    {
        return inArea;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObjectCollection : MonoBehaviour
{
    public Dictionary<string, int> grabableObjectDictionary;

    void Start()
    {
        grabableObjectDictionary = new Dictionary<string, int>();
    }

   
}

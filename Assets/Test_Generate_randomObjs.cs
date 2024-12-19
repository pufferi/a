using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Generate_randomObjs : MonoBehaviour
{
    public GrabableObjectGenerator grabableObjectGenerator;
    int i = 0;
    void Update()
    {
        Create();
    }

    void Create()
    {
        for (; i < 10; i++)
        {
            GameObject obj = grabableObjectGenerator.GetObject();
        }
    }
}

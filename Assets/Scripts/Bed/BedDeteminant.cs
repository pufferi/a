using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedDeteminant : MonoBehaviour
{
    public List<BedDeterminTrigger> bedTrigger = new List<BedDeterminTrigger>();

    public TaskListController tlm;

    void Start()
    {
        foreach (BedDeterminTrigger bdt in GetComponentsInChildren<BedDeterminTrigger>())
        {
            bedTrigger.Add(bdt);
        }
    }

    private void Update()
    {
        if(IsBed())
        {
            tlm.CompleteTask(0);
        }
    }


    public bool IsBed()
    {
        int triggeredNum = 0;
        foreach (BedDeterminTrigger bdt in bedTrigger)
        {
            if (bdt.isTriggered)
            {
                triggeredNum++;
                if (triggeredNum == bedTrigger.Count)
                {
                    return true;
                }
            }

        }
        return false;
    }
}

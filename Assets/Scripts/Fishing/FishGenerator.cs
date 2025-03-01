using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FishEntry
{
    public GameObject fishObject;
    public int index;
}

public class FishGenerator : MonoBehaviour
{
    //for generate special fish
    private int _caughtFishCount = -1;

    [SerializeField]
    private List<FishEntry> fishEntries;

    private Dictionary<GameObject, int> fishDictionary;

    public GrabableObjectGenerator grabableObjectGenerator;
    public Material mat;

    public TaskListController tlm;

    void Start()
    {
        fishDictionary = new Dictionary<GameObject, int>();
        foreach (var entry in fishEntries)
        {
            fishDictionary.Add(entry.fishObject, entry.index);
        }
    }

    private void GenerateGarbage(int fishSize)
    {
        GameObject obj = grabableObjectGenerator.GetObject(new Vector3(0, 0.5f, 0), Vector3.one * fishSize, mat);
    }

    public void GenerateFish(int fishSize)
    {
        if(fishSize == -1)
        {
            tlm.CompleteTask("Try Fishing");
        }

        _caughtFishCount++;
        foreach (var kvp in fishDictionary)
        {
            if (kvp.Value == _caughtFishCount)
            {
                kvp.Key.transform.position = new Vector3(0, 0.5f, 0);
                kvp.Key.SetActive(true);
                return;
            }
        }
        GenerateGarbage(fishSize);
    }
}

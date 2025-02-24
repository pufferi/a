using UnityEngine;
using UnityEngine.InputSystem;

public class GarbageBin : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;

    private GrabableObjectGenerator grabableObjectGenerator;

    private void Start()
    {
        grabableObjectGenerator = FindObjectOfType<GrabableObjectGenerator>();
    }

    public void DestroyOldGarbageAndRegenerateGarbageInGarbageBin()
    {
        // Destroy old garbage
        foreach (Transform child in transform)
        {
            grabableObjectGenerator.ReturnObject(child.gameObject);
            Destroy(child.gameObject);
        }
        // Regenerate garbage
        GenerateGarbage();
    }

    private void GenerateGarbage()
    {
        // Implementation for generating new garbage
    }
}

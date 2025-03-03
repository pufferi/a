using UnityEngine;

public class BedDeterminTrigger : MonoBehaviour
{
    public bool isTriggered = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isTriggered = false;
        }
    }
}

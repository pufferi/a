using UnityEngine;

public class BedDeterminTrigger : MonoBehaviour
{
    public bool isTriggered = false;
    public GrabableObjectComponent Bedbase;

    public Material defaultMat;
    public Material triggeredMat;


    private void Update()
    {
        if(isTriggered)
        {
            GetComponent<Renderer>().material = triggeredMat;
        }
        else
        {
            GetComponent<Renderer>().material = defaultMat;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        int bedBaseGroupId = Bedbase.groupID;
        if (other.CompareTag("Item"))
        {
            if (other.GetComponent<GrabableObjectComponent>().groupID == bedBaseGroupId)
            {
                isTriggered = true;
            }
            else
                isTriggered = false;
        }
        else
            isTriggered = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}

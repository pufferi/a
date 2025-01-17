using UnityEngine;

public class InAreaChecker : MonoBehaviour
{
    public bool inArea { get; private set; }



    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inArea = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inArea = false;
        }
    }

}

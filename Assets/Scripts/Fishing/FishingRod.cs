using UnityEngine;
using UnityEngine.XR;
using static UnityEditor.Experimental.GraphView.GraphView;
public class FishingRod : MonoBehaviour
{
    //private Vector3 grabPositionOffset = new Vector3(-0.5f,-1.25f,-1.42f);
    //private Vector3 grabRotationOffset = new Vector3(-80.7f, 40, 27.4f);
    public Transform hand;
    public PlayerGrabFishingRod playerGrabFishingRod;



    public LayerMask Layer_DontTouchPlayer;
    public Camera playerCam;//track the rotation


    public void Grab()
    {
        Debug.Log("Grabbing the fishing rod");  
        transform.SetParent(hand);

        this.GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        if (playerGrabFishingRod.fishingRod != null)
        {
            transform.localPosition = Vector3.zero;
            //transform.localRotation = Quaternion.Euler(grabRotationOffset);
            //transform.rotation = playerCam.transform.rotation * Quaternion.Euler(grabRotationOffset);
            transform.rotation = playerCam.transform.rotation;
        }
        
    }

    public void Release()
    {
        Debug.Log("Releasing the fishing rod");
        transform.SetParent(null);
        this.transform.position= hand.position;
        this.GetComponent<Collider>().enabled = true;



    }
}

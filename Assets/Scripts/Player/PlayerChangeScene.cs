using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChangeScene : MonoBehaviour
{
    public Transform destination;

    public GameObject Player;

    public PlayerGrabItems playerGrabItems;


    public InputActionAsset inputActions;

    private InputAction changeSceneAction;

    public bool inRegion;


    public void ActivateChangeSceneFunction()
    {

        changeSceneAction.performed += OnSceneChange;
    }

    public void InactivateChangeSceneFunction()
    {

        changeSceneAction.performed -= OnSceneChange;
    }


    private void Awake()
    {
        var ConversationMap = inputActions.FindActionMap("Player");
        changeSceneAction = ConversationMap.FindAction("Interact");
        changeSceneAction.Enable();
        ActivateChangeSceneFunction();
    }
    private void OnSceneChange(InputAction.CallbackContext context)
    {
        if (inRegion) 
        {
            //if(playerGrabItems.grabbedObject!=null)
                //playerGrabItems.grabbedObject.transform.position = destination.position;
            Player.transform.position = destination.position; 
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = true;
            playerGrabItems.isPlayerInDoorArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = false;
            playerGrabItems.isPlayerInDoorArea = false ;
        }
    }

    
}

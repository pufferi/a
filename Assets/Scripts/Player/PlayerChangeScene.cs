using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChangeScene : MonoBehaviour
{
    public Transform destination;

    public GameObject Player;

    public InputActionAsset inputActions;

    private InputAction changeSceneAction;

    private bool inRegion;



    private void Awake()
    {
        var ConversationMap = inputActions.FindActionMap("Player");
        changeSceneAction = ConversationMap.FindAction("Interact");
        changeSceneAction.Enable();
        changeSceneAction.performed += OnSceneChange;
    }
    private void OnSceneChange(InputAction.CallbackContext context)
    {
        if(inRegion)Player.transform.position = destination.position;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = false;
        }
    }

    
}

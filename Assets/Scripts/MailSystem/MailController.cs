using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class MailController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction MailBoxOpenAndClose;
    private bool isMailBoxOpen = false;
    public GameObject MailBoxCanvas;
    public MailHandler mailHandler;

    private void Start()
    {
        var PlayerMap = inputActions.FindActionMap("Player");
        MailBoxOpenAndClose = PlayerMap.FindAction("Mail");
        MailBoxOpenAndClose.performed += OnMailBoxOpenAndClosePerformed;
    }

    

    private void OnMailBoxOpenAndClosePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("MailBoxOpenAndClose triggered. isMailBoxOpen: " + isMailBoxOpen);
        if (isMailBoxOpen)
        {
            MailBoxCanvas.SetActive(false);
            isMailBoxOpen=false;
            PlayerStateManager.Instance.PlayerMoveUnlock();
            PlayerStateManager.Instance.PlayerViewUnlock("all");
        }
        else
        {
            mailHandler.SelectChild(0);
            MailBoxCanvas.SetActive(true);
            isMailBoxOpen = true;
            PlayerStateManager.Instance.PlayerMoveLock();
            PlayerStateManager.Instance.PlayerViewLock("all");
        }
    }
   

}

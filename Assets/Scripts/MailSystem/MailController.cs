using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MailController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction MailBoxOpenAndClose;
    private bool isMailBoxOpen = false;
    public GameObject MailBoxCanvas;
    public PlayerMovement playerMovement;

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
            playerMovement.playerStill=false;
        }
        else
        {
            MailBoxCanvas.SetActive(true);
            isMailBoxOpen = true;
            playerMovement.playerStill = true;
        }
    }
   
    private void OnDestroy()
    {
        if (MailBoxOpenAndClose != null)
        {
            MailBoxOpenAndClose.performed -= OnMailBoxOpenAndClosePerformed;
        }
    }

}

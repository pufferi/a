using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Click2Start : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction clickAction;
            
    public CursorController cursorController;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Others");

        clickAction = playerMap.FindAction("Click");

        clickAction.Enable();
        clickAction.performed += onClick;
    }

    private void onClick(InputAction.CallbackContext context)
    {
        cursorController.Cursorlock();
        gameObject.SetActive(false);
    }
}

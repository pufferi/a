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
        Debug.Log("Clicked");
        clickAction.performed -= onClick;
        cursorController.Cursorlock();
        gameObject.SetActive(false);
        clickAction.Disable();
    }
}

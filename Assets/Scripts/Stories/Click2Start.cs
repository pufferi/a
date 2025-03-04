using UnityEngine;
using UnityEngine.InputSystem;

public class Click2Start : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction clickAction;
            
    public CursorController cursorController;

    public GameObject PlayerStory_1;

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
        PlayerStory_1.SetActive(true);
        gameObject.SetActive(false);
        clickAction.Disable();
    }
}

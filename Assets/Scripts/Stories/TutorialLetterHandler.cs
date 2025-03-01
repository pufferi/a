using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialLetterHandler : MonoBehaviour
{
    public GameObject AlianLetter;
    public InputActionAsset inputActions;
    //private InputAction openLetterAction;
    private bool isLetterOpen;

    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        //openLetterAction = playerMap.FindAction("Tutorial");
        //openLetterAction.Enable();

        //openLetterAction.performed += ToggleLetter; 
    }

    private void OnDestroy()
    {
       // openLetterAction.performed -= ToggleLetter;
    }

    private void ToggleLetter(InputAction.CallbackContext context)
    {
        isLetterOpen = !isLetterOpen;
        AlianLetter.SetActive(isLetterOpen);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class BasicUiController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction settingsAction;

    [SerializeField]
    private GameObject settingsCanvas;
    [SerializeField]
    private CursorController cursorController;


    private void Start()
    {
        var otherMap = inputActions.FindActionMap("Others");
        settingsAction = otherMap.FindAction("Settings");

        settingsAction.Enable();//���Ҫ����{�ڷ���ҳ������enable,��ҳ����de}

    }

    void Update() 
    { 
        if (settingsAction.triggered) 
        {
            if (!settingsCanvas.activeSelf) { settingsCanvas.SetActive(true); cursorController.CursorUnlock(); }
            else { settingsCanvas.SetActive(false); cursorController.Cursorlock(); }
        }
    }

}

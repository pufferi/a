using UnityEngine;
using UnityEngine.InputSystem;

public class ShowBedBuildProgress : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction showBuildProcessAction;

    private bool _isTriggered = false;

    public GameObject BedMesh;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        showBuildProcessAction = playerMap.FindAction("CheckBedProgress");
        showBuildProcessAction.Enable();
        showBuildProcessAction.performed += OnShowProgress;
    }

    private void OnShowProgress(InputAction.CallbackContext context)
    {
        if (!_isTriggered)
        {
            _isTriggered = true;
            BedMesh.SetActive(true);
        }
        else
        {
            _isTriggered = false;
            BedMesh.SetActive(false);
        }
    }

}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    public float sensX = 90f;
    public float sensY = 90f;
    public Transform playerOrientation;
    public InputActionAsset inputActions;
    public float damping = 10f;
    public float deadZone = 0.05f;

    private InputAction lookAction;
    private Vector2 lookInput;
    private Vector2 currentInput;
    private float xRotation;
    private float yRotation;

    public bool playerViewLockX = false;
    public bool playerViewLockY = false;

    public PlayerGrabItems playerGrabItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var playerMap = inputActions.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
        lookAction.performed += OnLook;
        lookAction.canceled += OnLookCanceled; 
        lookAction.Enable();
    }

    private void OnDestroy()
    {
        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLookCanceled;
        lookAction.Disable();
    }

    private void Update()
    {
        currentInput = Vector2.Lerp(currentInput, lookInput, Time.deltaTime * damping);

        if (Mathf.Abs(currentInput.x) < deadZone && Mathf.Abs(currentInput.y) < deadZone)
        {
            return;
        }

        float mouseX = currentInput.x * Time.deltaTime * sensX;
        float mouseY = currentInput.y * Time.deltaTime * sensY;

        if (!playerViewLockX)
        {
            yRotation += mouseX;
            yRotation %= 360; 

            if (!playerViewLockY)
            {
                if (playerGrabItems != null && !playerGrabItems.CanItemGoDown() && mouseY < 0)
                {
                    mouseY = 0;
                }
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            }

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (playerViewLockX && playerViewLockY)
            return;
        lookInput = context.ReadValue<Vector2>();
    }


    // prevent camera from moving when mouse is still
    public void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    public void LookAtSomeWhere(Vector3 someWhere)
    {
        Vector3 direction = someWhere - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
        playerOrientation.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        xRotation = rotation.eulerAngles.x;
        yRotation = rotation.eulerAngles.y;
    }
}

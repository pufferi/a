using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public float sensX = 100f;
    public float sensY = 100f;
    public Transform playerOrientation;
    public InputActionAsset inputActions;
    public float damping = 10f; // ����ϵ��
    public float deadZone = 0.05f; // ������Χ

    private InputAction lookAction;
    private Vector2 lookInput;
    private Vector2 currentInput;
    private float xRotation;
    private float yRotation;

    void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
        lookAction.performed += OnLook;
        lookAction.Enable();
    }

    



    void OnDestroy()
    {
        lookAction.performed -= OnLook;
        lookAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("before"+  currentInput.x+"  "+currentInput.y);
        // ʹ�� Vector2.Lerp �𽥼�С����ֵ
        currentInput = Vector2.Lerp(currentInput, lookInput, Time.deltaTime * damping);
        //Debug.Log("after" + currentInput.x + "  " + currentInput.y);

        // �������ֵ�Ƿ���������Χ��
        if (Mathf.Abs(currentInput.x) < deadZone && Mathf.Abs(currentInput.y) < deadZone)
        {
            return; 
        }

        float mouseX = currentInput.x * Time.deltaTime * sensX;
        float mouseY = currentInput.y * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}

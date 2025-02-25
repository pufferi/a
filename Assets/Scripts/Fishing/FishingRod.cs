using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FishingRod : GrabableObjectComponent
{
    private Vector3 DefaultgrabPosition = new Vector3(-3.65f, -0.35f, 1.46f);
    private Vector3 DefaultgrabRotation = new Vector3(-86.7f, 153, 54.8f);

    public Vector3 RotateTarget= new Vector3(-4.377f,0.381f,1.762f);
    public Vector3 axis = Vector3.up;

    public InputActionAsset inputActions;

    private InputAction fishRodMoveAction;

    private bool _isInFishingGame = false;

    public float angle = 0;

    void Start()
    {
        this.objID = -2;
    }

    private void Awake()//not start..mystery
    {
        var fishMap = inputActions.FindActionMap("FishGame");
        fishRodMoveAction = fishMap.FindAction("FishRodMove");

        fishRodMoveAction.Enable();
    }

    private void Update()
    {
        // AvoidingObjectPenetratingTheFloor();
        //还没写这个

        if (_isInFishingGame)
        {
            Vector2 mouseMovement = fishRodMoveAction.ReadValue<Vector2>();
            float mouseY = mouseMovement.y;
            angle = mouseY;
            //Debug.Log(angle);
            RotateObjectAroundPoint(RotateTarget, axis, angle);
        }
    }

    private void RotateObjectAroundPoint(Vector3 target, Vector3 axis, float angle)
    {
        float rotationSpeed = 7.0f; //
        transform.RotateAround(target, axis, angle * rotationSpeed * Time.deltaTime);
    }


    public void StartFishingGame_PlacingTheFishRod()
    {
        this.transform.position = DefaultgrabPosition;
        this.transform.rotation = Quaternion.Euler(DefaultgrabRotation);
        this.GetComponent<Rigidbody>().isKinematic = true; 
        _isInFishingGame = true;
    }

    public void EndFishingGame()
    {
        //this.GetComponent<Rigidbody>().isKinematic = false;
        _isInFishingGame = false;
    }
}

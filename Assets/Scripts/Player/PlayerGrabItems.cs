using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerGrabItems : MonoBehaviour
{

    public Transform hand;
    private float grabDistance = 1.5f;
    public float rotationSpeed = 10;
    private string targetTag = "Item";
    private float rayDistance = 0.4f;

    public GrabableObjectComponent grabbedObject { get;  set; }
    public bool isPlayerInDoorArea = false;
    public InputActionAsset inputActions;

    private InputAction grabAction;
    private InputAction rotateLRAction;
    private InputAction rotateUDAction;
    private InputAction jointAction;
    private InputAction unjointAction;
    private InputAction itemForwardAction;
    private InputAction autoAttachAction;


    public float ScrollSen = 0.05f;

    private Camera playerCamera;

    private bool isGrbbedNormalItem = false;//not a fishRod or something else
    //special item id
    //fishingRod : -2



    private List<GrabableObjectComponent> AllConnectsOfCurrentGrabbedObj;

    private void Start()
    {
        playerCamera = Camera.main;
        var playerMap = inputActions.FindActionMap("Player");

        grabAction = playerMap.FindAction("Interact");
        rotateLRAction = playerMap.FindAction("RotateLR");
        rotateUDAction = playerMap.FindAction("RotateUD");
        jointAction = playerMap.FindAction("FixJoints");
        unjointAction = playerMap.FindAction("UnfixJoints");
        itemForwardAction = playerMap.FindAction("ItemForward");
        autoAttachAction = playerMap.FindAction("AutoAttach");

        grabAction.Enable();
        rotateLRAction.Enable();
        rotateUDAction.Enable();
        jointAction.Enable();
        unjointAction.Enable();
        itemForwardAction.Enable();
        autoAttachAction.Enable();

        grabAction.performed += OnGrab;
        jointAction.performed += OnJoint;
        unjointAction.performed += OnUnJoint;
        itemForwardAction.performed += OnItemMovingForward;
        //autoAttachAction.performed += OnAutoAttach;

        directions = GenerateDirections(numberOfDirections);
    }

    void Update()
    {
        if (isGrbbedNormalItem && grabbedObject != null)//normalItem means can stick
            CheckIfCanStickAndChangeMaterial();


        if (!rotateUDAction.IsPressed() && rotateLRAction.IsPressed() && grabbedObject != null)
        {
            float scrollValue = rotationSpeed * Time.deltaTime;
            grabbedObject.transform.Rotate(Vector3.up, scrollValue, Space.World);
        }
        else if (rotateUDAction.IsPressed() && rotateLRAction.IsPressed() && grabbedObject != null)
        {
            float scrollValue = rotationSpeed * Time.deltaTime;
            grabbedObject.transform.Rotate(Vector3.left, scrollValue, Space.World);
        }
    }


    bool canGrabbedObjStick = false;
    private RaycastHit closestHit = new RaycastHit();

    private RaycastHit lastRaycasHit;

    public Material greenMet;
    public Material yellowMet;
    // Ensure only one object has the yellow material
    private Material originalMat_Grabbed;
    private Material originalMat_Target;
    private GameObject LastHitObject;


    private void CheckIfCanStickAndChangeMaterial()//Get closestHit
    {
        canGrabbedObjStick = false;
        float closestDistance = 3f;

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(grabbedObject.transform.position, direction, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject.gameObject && hit.collider.gameObject.layer == 0)//item,not itself,and not connect to itself
                {
                    float distance = Vector3.Distance(grabbedObject.transform.position, hit.point);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestHit = hit;
                        canGrabbedObjStick = true;
                    }
                }

            }
        }
        Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
        if (canGrabbedObjStick)
        {
            grabbedObjectRenderer.material = greenMet;

            Renderer hitRenderer = closestHit.collider.GetComponent<Renderer>();

            if (LastHitObject != null && LastHitObject.GetComponent<Renderer>() != null)//&& LastHitObject.GetComponent<Renderer>().material == yellowMet)
                LastHitObject.GetComponent<Renderer>().material = originalMat_Target;


            if (hitRenderer != null)
            {

                originalMat_Target = hitRenderer.material;

                hitRenderer.material = yellowMet;
                LastHitObject = closestHit.collider.gameObject;// Ensure only one object has the yellow material

            }

            lastRaycasHit = closestHit;
        }
        else
        {
            grabbedObjectRenderer.material = originalMat_Grabbed;
            if (lastRaycasHit.collider != null)
            {
                Renderer lastRenderer = lastRaycasHit.collider.GetComponent<Renderer>();
                if (lastRenderer != null && originalMat_Target != null)
                {
                    lastRenderer.material = originalMat_Target;
                }
                originalMat_Target = null;
                LastHitObject = null;
            }

        }

    }



    public LayerMask Layer_DontTouchRay;

    private void OnGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance, ~Layer_DontTouchRay))
            {
                GrabableObjectComponent grabbable = hit.collider.GetComponent<GrabableObjectComponent>();
                if (grabbable != null)
                {
                    Debug.Log(grabbable);
                    if (grabbable.objID >= 0)
                        isGrbbedNormalItem = true;
                    else // is fish rod or something
                    {
                        isGrbbedNormalItem = false;
                        grabbedObject = grabbable;
                        grabbedObject.Grab();
                        return;
                    }
                    grabbedObject = grabbable;
                    Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
                    originalMat_Grabbed = grabbedObjectRenderer.material;
                    grabbedObject.Grab();
                    AllConnectsOfCurrentGrabbedObj = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(grabbable);
                }
            }
        }
        else if (!isPlayerInDoorArea)
        {
            if (!isGrbbedNormalItem)
            {
                grabbedObject.Release();
                grabbedObject = null;
                return;
            }
            Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
            grabbedObjectRenderer.material = originalMat_Grabbed;
            grabbedObject.Release();
            grabbedObject = null;
            AllConnectsOfCurrentGrabbedObj = null;
        }
    }


    public void Grab(GrabableObjectComponent obj)
    {
        if (grabbedObject == null)
        {
            GrabableObjectComponent grabbable = obj;
            if (grabbable != null)
            {
                //Debug.Log(grabbable);
                if (grabbable.objID >= 0)
                    isGrbbedNormalItem = true;
                else // is fish rod or something
                {
                    isGrbbedNormalItem = false;
                    grabbedObject = grabbable;
                    grabbedObject.Grab();
                    return;
                }
                grabbedObject = grabbable;
                Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
                originalMat_Grabbed = grabbedObjectRenderer.material;
                grabbedObject.Grab();
                AllConnectsOfCurrentGrabbedObj = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(grabbable);
            }
        }
    }

    public void Release()
    {
        if (grabbedObject != null && !isPlayerInDoorArea)
        {
            if (!isGrbbedNormalItem)
            {
                grabbedObject.Release();
                grabbedObject = null;
                return;
            }
            Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
            grabbedObjectRenderer.material = originalMat_Grabbed;
            grabbedObject.Release();
            grabbedObject = null;
            AllConnectsOfCurrentGrabbedObj = null;
        }
    }

    private List<Vector3> GenerateDirections(int numberOfDirections)
    {
        List<Vector3> directions = new List<Vector3>();

        for (int i = 0; i < numberOfDirections; i++)
        {
            float theta = Mathf.Acos(1 - 2 * (i + 0.5f) / numberOfDirections);
            float phi = Mathf.PI * (1 + Mathf.Sqrt(5)) * (i + 0.5f);

            float x = Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = Mathf.Cos(theta);

            directions.Add(new Vector3(x, y, z).normalized);
        }

        return directions;
    }




    private List<Vector3> directions;
    private int numberOfDirections = 300;


    private void OnJoint(InputAction.CallbackContext context)
    {
        Debug.Log("OnJoint");
        if (canGrabbedObjStick)
        {
            GrabableObjectComponent hitObject = closestHit.rigidbody.GetComponent<GrabableObjectComponent>();
            if (hitObject == null || GrabableObejectGroupingManager.Instance.IsConnect(grabbedObject, hitObject))
                return;

            Debug.Log("Detected closest object with tag: " + targetTag);

            FixedJoint joint = grabbedObject.AddComponent<FixedJoint>();
            joint.connectedBody = closestHit.rigidbody;
            Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
            grabbedObjectRenderer.material = originalMat_Grabbed;
            grabbedObject.Release();

            Renderer hitRenderer = closestHit.collider.GetComponent<Renderer>();
            if (hitRenderer != null)
            {
                hitRenderer.material = originalMat_Target;
                originalMat_Target = null;
            }


            GrabableObejectGroupingManager.Instance.AssignGroupID(grabbedObject, hitObject);
            grabbedObject = null;
        }
    }

    private void OnUnJoint(InputAction.CallbackContext context)
    {
        Debug.Log("OnUnJoint");
        FixedJoint[] joints = grabbedObject.GetComponents<FixedJoint>();
        if (joints.Length > 0)
        {
            foreach (FixedJoint joint in joints)
            {
                Destroy(joint);
            }
        }

        FixedJoint[] allJoints = FindObjectsOfType<FixedJoint>();
        foreach (FixedJoint j in allJoints)
        {
            if (j.connectedBody == grabbedObject.GetComponent<Rigidbody>())
            {
                Destroy(j);
            }
        }
        if (lastRaycasHit.collider != null)
        {
            Renderer lastRenderer = lastRaycasHit.collider.GetComponent<Renderer>();
            if (lastRenderer != null && originalMat_Target != null)
            {
                lastRenderer.material = originalMat_Target;
                originalMat_Target = null;
            }
        }

        foreach (GrabableObjectComponent connect in AllConnectsOfCurrentGrabbedObj)
            connect.gameObject.layer = 0;


        GrabableObejectGroupingManager.Instance.UnassignGroupID(grabbedObject);
    }


    private void OnItemMovingForward(InputAction.CallbackContext context)
    {
        if (Keyboard.current[Key.LeftCtrl].isPressed)
            return;
        if (grabbedObject != null)
        {
            float scrollValue = context.ReadValue<float>() * 0.001f;
            Vector3 direction = (grabbedObject.transform.position - hand.position).normalized;

            float distance = Vector3.Distance(grabbedObject.transform.position, hand.position);
            float minDistance = 0.7f;

            if (distance < minDistance && scrollValue < 0)
            {
                Debug.Log("Too close");
            }
            else
                grabbedObject.transform.position += direction * scrollValue;
        }
    }

}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerGrabItems : MonoBehaviour
{
    public Transform hand;
    private float grabDistance =1.7f;
    public float rotationSpeed = 10;
    private string targetTag = "Item";
    private float rayDistance = 0.8f;

    public GrabableObjectComponent grabbedObject { get; private set; }
    public bool isPlayerInDoorArea=false;
    public InputActionAsset inputActions;

    private InputAction grabAction;
    private InputAction rotateLRAction;
    private InputAction rotateUDAction;
    private InputAction jointAction;
    private InputAction unjointAction;
    private InputAction itemForwardAction;
    private InputAction autoAttachAction;

    public Material greenMet;
    public Material yellowMet;
    //public Material Met;
    //public Material targetMet;

    public float ScrollSen = 0.05f;

    private Camera playerCamera;

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
        if (grabbedObject != null) 
        {
            CheckIfCanStickAndChangeMaterial();
        }


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
    private Material originalGrabbedObjMet;

    private Material targetObjOriginMet;
    private RaycastHit lastRaycasHit;

    private void CheckIfCanStickAndChangeMaterial()
    {
        canGrabbedObjStick = false;
        float closestDistance = 3f;

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(grabbedObject.transform.position, direction, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject.gameObject)
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
            if (hitRenderer != null)
            {
                // 保存目标对象的原始材质
                if (targetObjOriginMet == null)
                {
                    targetObjOriginMet = hitRenderer.material;
                }

                // 将材质更改为 yellowMet
                hitRenderer.material = yellowMet;
            }

            // 记录最近一次射线击中信息
            lastRaycasHit = closestHit;
        }
        else
        {
            grabbedObjectRenderer.material = originalGrabbedObjMet;

            // 还原目标对象的原始材质
            if (lastRaycasHit.collider != null)
            {
                Renderer lastRenderer = lastRaycasHit.collider.GetComponent<Renderer>();
                if (lastRenderer != null && targetObjOriginMet != null)
                {
                    lastRenderer.material = targetObjOriginMet;
                    targetObjOriginMet = null; // 重置原始材质变量
                }
            }
        }
    }


    private void OnGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance))
            {
                GrabableObjectComponent grabbable = hit.collider.GetComponent<GrabableObjectComponent>();
                if (grabbable != null)
                {
                    grabbedObject = grabbable;
                    Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
                    originalGrabbedObjMet=grabbedObjectRenderer.material;
                    grabbedObject.Grab();
                }
            }
        }
        else if(!isPlayerInDoorArea)
        {
            Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
            grabbedObjectRenderer.material = originalGrabbedObjMet;
            grabbedObject.Release();
            grabbedObject = null;
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
    private int numberOfDirections = 100;


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
            grabbedObjectRenderer.material = originalGrabbedObjMet;
            grabbedObject.Release();

            Renderer hitRenderer = closestHit.collider.GetComponent<Renderer>();
            if (hitRenderer != null)
            {
                hitRenderer.material = targetObjOriginMet;
                targetObjOriginMet = null;
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
            if (lastRenderer != null && targetObjOriginMet != null)
            {
                lastRenderer.material = targetObjOriginMet;
                targetObjOriginMet = null;
            }
        }

        GrabableObejectGroupingManager.Instance.UnassignGroupID(grabbedObject);
    }

    //private void OnJoint(InputAction.CallbackContext context)
    //{
    //    Debug.Log("OnJoint");
    //    if (canGrabbedObjStick)
    //    {
    //        GrabableObjectComponent hitObject = closestHit.rigidbody.GetComponent<GrabableObjectComponent>();
    //        if (hitObject == null|| GrabableObejectGroupingManager.Instance.IsConnect(grabbedObject, hitObject))
    //            return;

    //        Debug.Log("Detected closest object with tag: " + targetTag);

    //        FixedJoint joint = grabbedObject.AddComponent<FixedJoint>();
    //        joint.connectedBody = closestHit.rigidbody;
    //        Renderer grabbedObjectRenderer = grabbedObject.GetComponent<Renderer>();
    //        grabbedObjectRenderer.material = originalGrabbedObjMet;
    //        grabbedObject.Release();
    //        //grouping
    //        GrabableObejectGroupingManager.Instance.AssignGroupID(grabbedObject, hitObject);
    //        grabbedObject = null;

    //    }
    //}



    //private void OnUnJoint(InputAction.CallbackContext context)
    //{
    //    Debug.Log("OnUnJoint");
    //    FixedJoint[] joints = grabbedObject.GetComponents<FixedJoint>();
    //    if (joints.Length > 0)
    //    {
    //        foreach (FixedJoint joint in joints)
    //            Destroy(joint);
    //    }

    //    FixedJoint[] allJoints = FindObjectsOfType<FixedJoint>();
    //    foreach (FixedJoint j in allJoints)
    //    {
    //        if (j.connectedBody == grabbedObject.GetComponent<Rigidbody>())
    //            Destroy(j);
    //    }
    //    //ungrouping
    //    GrabableObejectGroupingManager.Instance.UnassignGroupID(grabbedObject);
    //}


    private void OnItemMovingForward(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
        {
            float scrollValue = context.ReadValue<float>() * 0.001f;
            Vector3 direction = (grabbedObject.transform.position - hand.position).normalized;

            float distance = Vector3.Distance(grabbedObject.transform.position, hand.position);
            float minDistance = 0.5f;

            if (distance < minDistance && scrollValue < 0)
            {
                Debug.Log("Too close");
            }
            else
                grabbedObject.transform.position += direction * scrollValue;
        }
    }




    //private void OnAutoAttach(InputAction.CallbackContext context)
    //{
    //    if (grabbedObject != null)
    //    {
    //        foreach (Vector3 direction in directions)
    //        {
    //            RaycastHit hit;
    //            if (Physics.Raycast(grabbedObject.transform.position, direction, out hit, rayDistance))
    //            {
    //                if (hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject)
    //                {
    //                    Debug.Log("Detected object with tag: " + targetTag + " in direction: " + direction);
    //                    GameObject targetObject = hit.collider.gameObject;
    //                    AttachToSurface(grabbedObject.gameObject, targetObject);
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}



    //private void AttachToSurface(GameObject grabbedObject, GameObject targetObject)
    //{
    //    Mesh grabbedMesh = grabbedObject.GetComponent<MeshFilter>().mesh;
    //    Mesh targetMesh = targetObject.GetComponent<MeshFilter>().mesh;

    //    Vector3 grabbedCenter, grabbedNormal, targetCenter, targetNormal;
    //    GetClosestFace(grabbedMesh, grabbedObject.transform, targetObject.transform.position, out grabbedCenter, out grabbedNormal);
    //    GetClosestFace(targetMesh, targetObject.transform, grabbedObject.transform.position, out targetCenter, out targetNormal);

    //    if (!AreNormalsWithinAngle(grabbedNormal, targetNormal, 30)) return;

    //    Quaternion rotationToParallel = Quaternion.FromToRotation(targetNormal, grabbedNormal);

    //    grabbedObject.transform.rotation = rotationToParallel * targetObject.transform.rotation;
    //}

    //private static bool AreNormalsWithinAngle(Vector3 normal1, Vector3 normal2, float maxAngleDegrees)
    //{
    //    float maxAngleRadians = maxAngleDegrees * Mathf.Deg2Rad;

    //    float dotProduct = Vector3.Dot(normal1.normalized, normal2.normalized);

    //    float cosAngle = Mathf.Cos(maxAngleRadians);

    //    return dotProduct >= cosAngle;
    //}


    //private void GetClosestFace(Mesh mesh, Transform transform, Vector3 referencePosition, out Vector3 faceCenter, out Vector3 normal)
    //{
    //    faceCenter = Vector3.zero;
    //    normal = Vector3.zero;
    //    float minDistance = float.MaxValue;

    //    Vector3[] vertices = mesh.vertices;
    //    Vector3[] normals = mesh.normals;
    //    int[] triangles = mesh.triangles;

    //    for (int i = 0; i < triangles.Length; i += 3)
    //    {
    //        Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
    //        Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
    //        Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

    //        Vector3 currentNormal = transform.TransformDirection(normals[triangles[i]]);
    //        Vector3 currentCenter = (v0 + v1 + v2) / 3.0f;

    //        float distance = Vector3.Distance(currentCenter, referencePosition);

    //        if (distance < minDistance)
    //        {
    //            minDistance = distance;
    //            faceCenter = currentCenter;
    //            normal = currentNormal;
    //        }
    //    }
    //}


}


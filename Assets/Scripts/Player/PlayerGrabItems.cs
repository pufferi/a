using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrabItems : MonoBehaviour
{
    public Transform hand;
    public float grabDistance = 3.0f;
    public float rotationSpeed = 5;
    public string targetTag = "Item";
    public float rayDistance = 3f;

    private GrabObjects grabbedObject;
    public InputActionAsset inputActions;

    private InputAction grabAction;
    private InputAction rotateLRAction;
    private InputAction rotateUDAction;
    private InputAction jointAction;
    private InputAction itemForwardAction;
    private InputAction autoAttachAction;


    public float ScrollSen=0.05f;

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
        var playerMap = inputActions.FindActionMap("Player");

        grabAction = playerMap.FindAction("Grab");
        rotateLRAction = playerMap.FindAction("RotateLR");
        rotateUDAction = playerMap.FindAction("RotateUD");
        jointAction = playerMap.FindAction("FixJoints");
        itemForwardAction = playerMap.FindAction("ItemForward");
        autoAttachAction = playerMap.FindAction("AutoAttach");

        grabAction.Enable();
        rotateLRAction.Enable();
        rotateUDAction.Enable();
        jointAction.Enable();
        itemForwardAction.Enable();
        autoAttachAction.Enable();

        grabAction.performed += OnGrab;
        //rotateLRAction.performed += OnLRRotate;
        //rotateUDAction.performed += OnUDRotate;
        jointAction.performed += OnJoint;
        itemForwardAction.performed += OnItemMoingForward;
        autoAttachAction.performed += OnAutoAttach;
    }

    void Update()
    {
        // 检测R键是否被按住
        if (!rotateUDAction.IsPressed()&&rotateLRAction.IsPressed() && grabbedObject != null)
        {
            float scrollValue = rotationSpeed * Time.deltaTime;
            grabbedObject.transform.Rotate(Vector3.up, scrollValue, Space.World);
        }
        //R+rightclick
        else if(rotateUDAction.IsPressed() && rotateLRAction.IsPressed() && grabbedObject != null)
        {
            float scrollValue = rotationSpeed * Time.deltaTime;
            grabbedObject.transform.Rotate(Vector3.left, scrollValue, Space.World);
        }
    }


    private void OnGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance))
            {
                GrabObjects grabbable = hit.collider.GetComponent<GrabObjects>();
                if (grabbable != null)
                {
                    grabbedObject = grabbable;
                    grabbedObject.Grab();
                }
            }
        }
        else
        {
            grabbedObject.Release();
            grabbedObject = null;
        }
    }




    //在考虑鼠标上下滚动触发物体旋转还是前后移动
    //这里要设置成Axis
    //private void OnLRRotate(InputAction.CallbackContext context)
    //{
    //    if (grabbedObject != null)
    //    {
    //        float scrollValue = context.ReadValue<float>() * ScrollSen; // 旋转物体
    //        grabbedObject.transform.Rotate(Vector3.up, scrollValue, Space.World);
    //    }
    //    else
    //    {
    //        //啥也不做
    //    }
    //}
    //private void OnUDRotate(InputAction.CallbackContext context)
    //{
    //    if (grabbedObject != null)
    //    {
    //        float scrollValue = context.ReadValue<float>() * ScrollSen; // 旋转物体
    //        grabbedObject.transform.Rotate(Vector3.left, scrollValue, Space.World);
    //    }
    //    else
    //    {
    //        //啥也不做
    //    }
    //}


    //是不是要排除掉Player相对gragbbedObj的位置
    private Vector3[] directions = {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            Vector3.up,
            Vector3.down
        };


    //还缺取消joint的部分
    private void OnJoint(InputAction.CallbackContext context)
    {
        Debug.Log("onnonJOint");
        if (grabbedObject != null)
        {
            foreach (Vector3 direction in directions)
            {
                RaycastHit hit;
                if (Physics.Raycast(grabbedObject.transform.position, direction, out hit, rayDistance))
                {
                    if (hit.collider.CompareTag(targetTag)&&hit.collider.gameObject!=grabbedObject)
                    {
                        Debug.Log("Detected object with tag: " + targetTag + " in direction: " + direction);
                        // 添加 FixedJoint
                        FixedJoint joint = grabbedObject.AddComponent<FixedJoint>();
                        joint.connectedBody = hit.rigidbody;
                        grabbedObject.Release(); 
                        grabbedObject = null;
                        break; 
                    }
                }
            }
        }
        else
        {
            //啥也不做
        }
    }


    private void OnItemMoingForward(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
        {
            float scrollValue = context.ReadValue<float>() * 0.001f;
            Vector3 direction = (grabbedObject.transform.position - hand.position).normalized;

            float distance = Vector3.Distance(grabbedObject.transform.position, hand.position);
            float minDistance = 0.8f; // 设置物体与玩家之间的最小距离

            if (distance > minDistance)
            {
                grabbedObject.transform.position += direction * scrollValue;
            }
            else
            {
                Debug.Log("Too close to the player, pausing movement.");
            }
        }
    }


    //只是转向贴近这个面
    private void OnAutoAttach(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
        {
            foreach (Vector3 direction in directions)
            {
                RaycastHit hit;
                if (Physics.Raycast(grabbedObject.transform.position, direction, out hit, rayDistance))
                {
                    if (hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject)
                    {
                        Debug.Log("Detected object with tag: " + targetTag + " in direction: " + direction);
                        GameObject targetObject = hit.collider.gameObject;
                        AttachToSurface(grabbedObject.gameObject, targetObject);
                        break;
                    }
                }
            }
        }
    }

   

    private void AttachToSurface(GameObject grabbedObject, GameObject targetObject)
    {
        Mesh grabbedMesh = grabbedObject.GetComponent<MeshFilter>().mesh;
        Mesh targetMesh = targetObject.GetComponent<MeshFilter>().mesh;

        Vector3 grabbedCenter, grabbedNormal, targetCenter, targetNormal;
        GetClosestFace(grabbedMesh, grabbedObject.transform, targetObject.transform.position, out grabbedCenter, out grabbedNormal);
        GetClosestFace(targetMesh, targetObject.transform, grabbedObject.transform.position, out targetCenter, out targetNormal);

        //还要设置法线夹角不能差太大，，
        if (!AreNormalsWithinAngle(grabbedNormal, targetNormal, 30)) return;

        // 计算旋转，使两个法线平行
        Quaternion rotationToParallel = Quaternion.FromToRotation(targetNormal, grabbedNormal);

        // 应用旋转并调整位置
        grabbedObject.transform.rotation = rotationToParallel * targetObject.transform.rotation;
        //grabbedObject.transform.position = grabbedCenter + grabbedNormal * Vector3.Distance(grabbedCenter, targetCenter);
    }

    private static bool AreNormalsWithinAngle(Vector3 normal1, Vector3 normal2, float maxAngleDegrees)
    {
        // 将角度转换为弧度
        float maxAngleRadians = maxAngleDegrees * Mathf.Deg2Rad;

        // 计算法线向量的点积
        float dotProduct = Vector3.Dot(normal1.normalized, normal2.normalized);

        // 计算两个向量之间夹角的余弦值
        float cosAngle = Mathf.Cos(maxAngleRadians);

        // 比较余弦值
        return dotProduct >= cosAngle;
    }


    private void GetClosestFace(Mesh mesh, Transform transform, Vector3 referencePosition, out Vector3 faceCenter, out Vector3 normal)
    {
        faceCenter = Vector3.zero;
        normal = Vector3.zero;
        float minDistance = float.MaxValue;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            Vector3 currentNormal = transform.TransformDirection(normals[triangles[i]]);
            Vector3 currentCenter = (v0 + v1 + v2) / 3.0f;

            float distance = Vector3.Distance(currentCenter, referencePosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                faceCenter = currentCenter;
                normal = currentNormal;
            }
        }
    }


}

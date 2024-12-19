//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.InputSystem;
//using System;
//using Unity.VisualScripting;

//public class JointController : MonoBehaviour
//{
//    private List<Vector3> directions;
//    private int numberOfDirections = 100;
//    private float rayDistance = 3f;
//    private string targetTag = "Item";

//    private GameObject grabbedObject;
//    private UnionFind unionFind;

//    public PlayerGrabItems playerGrabItems;
//    public InputActionAsset inputActions;
//    private InputAction jointAction;
//    private InputAction unjointAction;


//    void Start()
//    {
//        unionFind = new UnionFind();
//        directions = GenerateDirections(numberOfDirections);


//        var playerMap = inputActions.FindActionMap("Player");

//        jointAction = playerMap.FindAction("FixJoints");
//        unjointAction = playerMap.FindAction("UnfixJoints");

//        jointAction.Enable();
//        unjointAction.Enable();

//        jointAction.performed += OnJoint;
//        unjointAction.performed += OnUnJoint;
//    }

//    //private void Update()
//    //{
//    //    grabbedObject=playerGrabItems.gameObject;
//    //}

//    private List<Vector3> GenerateDirections(int numberOfDirections)
//    {
//        List<Vector3> directions = new List<Vector3>();
//        for (int i = 0; i < numberOfDirections; i++)
//        {
//            float theta = Mathf.Acos(1 - 2 * (i + 0.5f) / numberOfDirections);
//            float phi = Mathf.PI * (1 + Mathf.Sqrt(5)) * (i + 0.5f);

//            float x = Mathf.Sin(theta) * Mathf.Cos(phi);
//            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
//            float z = Mathf.Cos(theta);

//            directions.Add(new Vector3(x, y, z).normalized);
//        }
//        return directions;
//    }

//    private void Update()
//    {
//        if (playerGrabItems != null)
//        {
//            grabbedObject = playerGrabItems.gameObject;
//        }
//        else
//        {
//            grabbedObject = null;
//        }
//    }

//    private void OnJoint(InputAction.CallbackContext context)
//    {
//        Debug.Log("OnJoint");
//        if (grabbedObject != null)
//        {
//            Rigidbody grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();
//            if (grabbedRigidbody == null)
//            {
//                Debug.LogError("grabbedObject does not have a Rigidbody component.");
//                return;
//            }

//            RaycastHit closestHit = new RaycastHit();
//            bool found = false;
//            float closestDistance = 1f;

//            foreach (Vector3 direction in directions)
//            {
//                RaycastHit hit;
//                Debug.Log("Casting ray in direction: " + direction);
//                if (Physics.Raycast(grabbedObject.transform.position, direction, out hit, rayDistance))
//                {
//                    Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
//                    if (hit.collider != null)
//                    {
//                        Debug.Log("Hit object tag: " + hit.collider.tag);
//                    }

//                    if (hit.collider != null && hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject)
//                    {
//                        float distance = Vector3.Distance(grabbedObject.transform.position, hit.point);
//                        Debug.Log("Hit object distance: " + distance);

//                        if (distance < closestDistance)
//                        {
//                            closestDistance = distance;
//                            closestHit = hit;
//                            found = true;
//                        }
//                    }
//                }
//                else
//                {
//                    Debug.Log("Raycast did not hit anything in direction: " + direction);
//                }
//            }

//            if (found)
//            {
//                if (closestHit.rigidbody == null)
//                {
//                    Debug.LogError("The closest hit object does not have a Rigidbody component.");
//                    return;
//                }

//                if (!unionFind.IsConnected(grabbedRigidbody, closestHit.rigidbody))
//                {
//                    FixedJoint joint = grabbedObject.AddComponent<FixedJoint>();
//                    joint.connectedBody = closestHit.rigidbody;
//                    unionFind.Union(grabbedRigidbody, closestHit.rigidbody);
//                    grabbedObject = null;
//                }
//            }
//            else
//            {
//                Debug.Log("No valid hit found");
//            }
//        }
//        else
//        {
//            Debug.LogError("No grabbed object found");
//        }
//    }

//    //private void OnJoint(InputAction.CallbackContext context)
//    //{
//    //    Debug.Log("OnJoint");
//    //    if (grabbedObject != null)
//    //    {
//    //        if (grabbedObject.GetComponent<Rigidbody>() == null)
//    //        {
//    //            Debug.LogError("grabbedObject does not have a Rigidbody component.");
//    //            return;
//    //        }

//    //        RaycastHit closestHit = new RaycastHit();
//    //        bool found = false;
//    //        float closestDistance = Mathf.Infinity;

//    //        foreach (Vector3 direction in directions)
//    //        {
//    //            Debug.Log(direction);
//    //            RaycastHit hit;
//    //            if (Physics.Raycast(grabbedObject.transform.position, direction, out hit, rayDistance))
//    //            {
//    //                if (hit.collider!= null) Debug.Log(hit.collider.tag);
//    //                if (hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject)
//    //                {
//    //                    float distance = Vector3.Distance(grabbedObject.transform.position, hit.point);
//    //                    if (distance < closestDistance)
//    //                    {
//    //                        closestDistance = distance;
//    //                        closestHit = hit;
//    //                        found = true;
//    //                    }
//    //                }
//    //            }
//    //        }

//    //        if (found)
//    //        {
//    //            if (closestHit.rigidbody == null)
//    //            {
//    //                Debug.LogError("The closest hit object does not have a Rigidbody component.");
//    //                return;
//    //            }

//    //            if (!unionFind.IsConnected(grabbedObject.GetComponent<Rigidbody>(), closestHit.rigidbody))
//    //            {
//    //                FixedJoint joint = grabbedObject.AddComponent<FixedJoint>();
//    //                joint.connectedBody = closestHit.rigidbody;
//    //                unionFind.Union(grabbedObject.GetComponent<Rigidbody>(), closestHit.rigidbody);
//    //                grabbedObject = null;
//    //            }
//    //        }
//    //        else
//    //        {
//    //            Debug.Log("No valid hit found");
//    //        }
//    //    }
//    //    else
//    //    {
//    //        Debug.LogError("No grabbed object found");
//    //    }
//    //}

//    private void OnUnJoint(InputAction.CallbackContext context)
//    {
//        Debug.Log("OnUnJoint");
//        if (grabbedObject != null)
//        {
//            FixedJoint[] joints = grabbedObject.GetComponents<FixedJoint>();
//            if (joints.Length > 0)
//            {
//                foreach (FixedJoint joint in joints)
//                {
//                    if (joint.connectedBody != null)
//                    {
//                        unionFind.Find(joint.connectedBody);
//                    }
//                    Destroy(joint);
//                }
//            }
//            FixedJoint[] allJoints = FindObjectsOfType<FixedJoint>();
//            foreach (FixedJoint j in allJoints)
//            {
//                if (j.connectedBody == grabbedObject.GetComponent<Rigidbody>())
//                {
//                    unionFind.Find(j.connectedBody);
//                    Destroy(j);
//                }
//            }
//        }
//        else
//        {
//            Debug.Log("No grabbed object found");
//        }
//    }


//    //private void OnJoint(InputAction.CallbackContext context)
//    //{
//    //    Debug.Log("OnJoint");
//    //    if (grabbedObject != null)
//    //    {
//    //        RaycastHit closestHit = new RaycastHit();
//    //        bool found = false;
//    //        float closestDistance = Mathf.Infinity;
//    //        foreach (Vector3 direction in directions)
//    //        {
//    //            RaycastHit hit;
//    //            if (Physics.Raycast(grabbedObject.transform.position, direction, out hit, rayDistance))
//    //            {
//    //                if (hit.collider.CompareTag(targetTag) && hit.collider.gameObject != grabbedObject.gameObject)
//    //                {
//    //                    float distance = Vector3.Distance(grabbedObject.transform.position, hit.point);
//    //                    if (distance < closestDistance)
//    //                    {
//    //                        closestDistance = distance;
//    //                        closestHit = hit;
//    //                        found = true;
//    //                    }
//    //                }
//    //            }
//    //        }
//    //        Debug.Log(unionFind.IsConnected(grabbedObject.GetComponent<Rigidbody>(), closestHit.rigidbody));
//    //        if (found && !unionFind.IsConnected(grabbedObject.GetComponent<Rigidbody>(), closestHit.rigidbody))
//    //        {
//    //            FixedJoint joint = grabbedObject.AddComponent<FixedJoint>();
//    //            joint.connectedBody = closestHit.rigidbody;
//    //            unionFind.Union(grabbedObject.GetComponent<Rigidbody>(), closestHit.rigidbody);
//    //            grabbedObject = null;
//    //        }
//    //    }
//    //    else
//    //        Debug.Log("NNNNNNNNNNNNNNNNNNNNN");

//    //}




//    //private void OnUnJoint(InputAction.CallbackContext context)
//    //{
//    //    Debug.Log("OnUnJoint");
//    //    // 直接
//    //    FixedJoint[] joints = grabbedObject.GetComponents<FixedJoint>();
//    //    if (joints.Length > 0)
//    //    {
//    //        foreach (FixedJoint joint in joints)
//    //        {
//    //            unionFind.Find(joint.connectedBody); 
//    //            Destroy(joint);
//    //        }
//    //    }
//    //    // 间接
//    //    FixedJoint[] allJoints = FindObjectsOfType<FixedJoint>();
//    //    foreach (FixedJoint j in allJoints)
//    //    {
//    //        if (j.connectedBody == grabbedObject.GetComponent<Rigidbody>())
//    //        {
//    //            unionFind.Find(j.connectedBody); 
//    //            Destroy(j);
//    //        }
//    //    }
//    //}
//}

////public class UnionFind
////{
////    private Dictionary<Rigidbody, Rigidbody> parent;
////    private Dictionary<Rigidbody, int> rank;

////    public UnionFind()
////    {
////        parent = new Dictionary<Rigidbody, Rigidbody>();
////        rank = new Dictionary<Rigidbody, int>();
////    }

////    public Rigidbody Find(Rigidbody x)
////    {
////        if (!parent.ContainsKey(x))
////        {
////            parent[x] = x;
////            rank[x] = 0;
////        }
////        if (parent[x] != x)
////        {
////            parent[x] = Find(parent[x]);
////        }
////        return parent[x];
////    }

////    public void Union(Rigidbody x, Rigidbody y)
////    {
////        if (x == null) return;
////        if (y == null) return;
////        Rigidbody rootX = Find(x);
////        Rigidbody rootY = Find(y);

////        if (rootX != rootY)
////        {
////            if (rank[rootX] > rank[rootY])
////            {
////                parent[rootY] = rootX;
////            }
////            else if (rank[rootX] < rank[rootY])
////            {
////                parent[rootX] = rootY;
////            }
////            else
////            {
////                parent[rootY] = rootX;
////                rank[rootX]++;
////            }
////        }
////    }

////    public bool IsConnected(Rigidbody x, Rigidbody y)
////    {
////        if (x == null || y == null) return false;
////        return Find(x) == Find(y);
////    }
////}

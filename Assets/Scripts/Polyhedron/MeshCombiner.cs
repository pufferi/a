using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeshCombiner : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction ConbineMeshAction;

    public PlayerGrabItems playerGrabItems;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");

        ConbineMeshAction = playerMap.FindAction("ConbineMesh");

        ConbineMeshAction.Enable();
        ConbineMeshAction.performed += OnMeshConbine;
    }


    List<MeshFilter> meshFilters = new List<MeshFilter>();
    HashSet<GameObject> visited = new HashSet<GameObject>();


    //private void TraverseJoints(GameObject obj)
    //{
    //    if (visited.Contains(obj)) return;
    //    visited.Add(obj);

    //    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
    //    if (meshFilter != null)
    //    {
    //        meshFilters.Add(meshFilter);
    //    }

    //    Joint[] joints = obj.GetComponents<Joint>();
    //    foreach (Joint joint in joints)
    //    {
    //        if (joint.connectedBody != null)
    //        {
    //            TraverseJoints(joint.connectedBody.gameObject);
    //        }
    //    }
    //}


    //private void GetMeshFilters(GameObject obj)
    //{
    //    TraverseJoints(obj);
    //}

    public void OnMeshConbine(InputAction.CallbackContext context)
    {
        //MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        GameObject obj;
        if(playerGrabItems.grabbedObject != null) 
            obj = playerGrabItems.grabbedObject.gameObject;
        else return;
        //GetMeshFilters(obj);
        Dictionary<Material, List<CombineInstance>> materialToMesh = new Dictionary<Material, List<CombineInstance>>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            Renderer renderer = meshFilter.GetComponent<Renderer>();
            Mesh mesh = meshFilter.sharedMesh;
            Material[] materials = renderer.sharedMaterials;

            for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
            {
                Material material = materials[subMesh];
                if (!materialToMesh.ContainsKey(material))
                {
                    materialToMesh[material] = new List<CombineInstance>();
                }

                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = mesh,
                    transform = meshFilter.transform.localToWorldMatrix,
                    subMeshIndex = subMesh
                };
                materialToMesh[material].Add(combineInstance);
            }
        }

        List<Mesh> meshes = new List<Mesh>();
        List<Material> newMaterials = new List<Material>();
        foreach (var kvp in materialToMesh)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(kvp.Value.ToArray(), true, true);
            meshes.Add(combinedMesh);
            newMaterials.Add(kvp.Key);
        }

        Mesh combinedResult = new Mesh();
        combinedResult.subMeshCount = meshes.Count;
        CombineInstance[] finalCombine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            finalCombine[i].mesh = meshes[i];
            finalCombine[i].transform = Matrix4x4.identity;
            finalCombine[i].subMeshIndex = 0; 
        }

        combinedResult.CombineMeshes(finalCombine, false, false);

        //MeshFilter resultFilter = gameObject.AddComponent<MeshFilter>();
        //resultFilter.mesh = combinedResult;

        //MeshRenderer resultRenderer = gameObject.AddComponent<MeshRenderer>();
        //resultRenderer.materials = newMaterials.ToArray();

        // 创建一个新的 GameObject 来保存合并后的网格
        GameObject combinedObject = new GameObject("CombinedMesh");
        MeshFilter meshFilterCombined = combinedObject.AddComponent<MeshFilter>();
        meshFilterCombined.mesh = combinedResult;

        MeshRenderer meshRendererCombined = combinedObject.AddComponent<MeshRenderer>();
        meshRendererCombined.material = GetComponentInChildren<MeshRenderer>().sharedMaterial;

        // 删除旧的 GameObject
        //foreach (var objj in visited)
        //{
        //    Destroy(objj);
        //}

        Debug.Log("Mesh combination and replacement complete!");
    }

}

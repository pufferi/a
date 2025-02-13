using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GrabableObejectGroupingManager;

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


    public void OnMeshConbine(InputAction.CallbackContext context)
    {
        GameObject obj;
        if (playerGrabItems.grabbedObject != null)
            obj = playerGrabItems.grabbedObject.gameObject;
        else return;

        // 使用 GetAllConnectObjects 获取所有连接的对象
        List<GrabableObjectComponent> connectedObjects = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(playerGrabItems.grabbedObject);

        Dictionary<Material, List<CombineInstance>> materialToMesh = new Dictionary<Material, List<CombineInstance>>();

        // 遍历所有连接的对象的 MeshFilter
        foreach (GrabableObjectComponent grabObject in connectedObjects)
        {
            MeshFilter[] filters = grabObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in filters)
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

        // 创建一个新的 GameObject 来保存合并后的网格
        GameObject combinedObject = new GameObject("CombinedMesh");
        MeshFilter meshFilterCombined = combinedObject.AddComponent<MeshFilter>();
        meshFilterCombined.mesh = combinedResult;

        MeshRenderer meshRendererCombined = combinedObject.AddComponent<MeshRenderer>();
        meshRendererCombined.material = GetComponentInChildren<MeshRenderer>().sharedMaterial;

        Debug.Log("Mesh combination and replacement complete!");
    }
}

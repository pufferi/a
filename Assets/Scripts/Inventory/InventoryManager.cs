using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public Button[] inventoryButtons=new Button[6];
    //public InventoryItemInfo[] inventorySlotsInfo = new InventoryItemInfo[6];
    public bool[] isInventorySlotEmpty = new bool[6];


    public InputActionAsset inputActions;

    private InputAction StoreItemAction;

    public PlayerGrabItems playerGrabItems;

    public int currentSlot = 0;

    public Color selectedColor = Color.green; 
    public Color defaultColor = Color.white;

    [SerializeField]
    private InventoryIconCapturer _inventoryIconCapturer;

    [SerializeField]
    private GameObject _parent;

    [SerializeField]
    private GameObject _GrabableObjs;

    private string _prefabPath = "Assets/Resources/Inventory/InventoryPrefebs/";
    private string _inventoryIconPath = "Assets/Resources/Inventory/InventoryItemIcons/";

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        StoreItemAction = playerMap.FindAction("StoreItem");
        StoreItemAction.Enable();
        StoreItemAction.performed += OnStoreOrDropItem;

        UpdateButtonColors();

        var scrollAction = playerMap.FindAction("ScrollInventory");
        scrollAction.Enable();
        scrollAction.performed += OnScrollInventory;

        for(int i = 0; i < 6; i++)
        {
            isInventorySlotEmpty[i] = true;
        }
    }

    private string GetInventoryName()
    {
        return "iv_" + currentSlot;
    }
    private void UpdateButtonColors()
    {
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            ColorBlock colors = inventoryButtons[i].colors;
            if (i == currentSlot)
            {
                colors.normalColor = selectedColor;
            }
            else
            {
                colors.normalColor = defaultColor;
            }
            inventoryButtons[i].colors = colors;
        }
    }




    private void OnStoreOrDropItem(InputAction.CallbackContext context)
    {
        GrabableObjectComponent itemInHand = playerGrabItems.grabbedObject;
        if (itemInHand == null&& isInventorySlotEmpty[currentSlot])
            return;

        if (isInventorySlotEmpty[currentSlot])
        {
            string slotName=GetInventoryName();
            _inventoryIconCapturer.CaptureIcon(slotName);

            Image image = inventoryButtons[currentSlot].GetComponent<Image>();

            string fileName = _inventoryIconPath + slotName + ".png";
            if (File.Exists(fileName))
            {
                byte[] fileData = File.ReadAllBytes(fileName);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
            }

            isInventorySlotEmpty[currentSlot] = false;

            List<GrabableObjectComponent> AllComponent = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(itemInHand);
            AllComponent.Add(itemInHand);
            GameObject wholeObj = new GameObject(slotName);
            wholeObj.transform.SetParent(_parent.transform);
            foreach (var obj in AllComponent)
            {
                Debug.Log(obj.gameObject.GetComponent<MeshRenderer>().material);
                obj.transform.SetParent(wholeObj.transform);
            }
            
            //PrefabUtility.SaveAsPrefabAsset(wholeObj, _prefabPath+slotName+".prefab");


            SavePrefabWithMeshes(wholeObj, _prefabPath + slotName + ".prefab");



            Destroy(wholeObj);
            
            Debug.Log("Stored item in slot " + currentSlot);

        }

        else
        {
            // 不需要前缀 'Assets/' 和后缀 '.prefab'
            string prefabPath = "Inventory/InventoryPrefebs/" + GetInventoryName();
            GameObject dropped = Resources.Load<GameObject>(prefabPath);

            if (dropped != null)
            {
                GameObject instantiatedObj = Instantiate(dropped, Vector3.zero, Quaternion.identity);
                instantiatedObj.transform.position = playerGrabItems.hand.position + new Vector3(1, 1, 1);
                instantiatedObj.transform.SetParent(_GrabableObjs.transform);
                Debug.Log("success!!!!!!!!");
            }
            else
            {
                Debug.LogError("Failed to load prefab at path: " + prefabPath);
            }

            // 删除 prefab
            string fullPath = Path.Combine(Application.dataPath, "Resources", prefabPath + ".prefab");
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log("Prefab file deleted successfully.");
            }
            else
            {
                Debug.Log("Prefab file not found.");
            }

            // 删除 ImageIcon
            string iconPath = _inventoryIconPath + GetInventoryName() + ".png";
            if (File.Exists(iconPath))
            {
                File.Delete(iconPath);
            }

            isInventorySlotEmpty[currentSlot] = true;
        }


    }



    void SavePrefabWithMeshes(GameObject wholeObj, string prefabPath)
    {
        // 遍历所有子对象，确保 MeshFilter 和 MeshRenderer 都已正确分配
        foreach (Transform child in wholeObj.transform)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshFilter == null)
            {
                Debug.LogError("MeshFilter missing in child: " + child.name);
                meshFilter = child.gameObject.AddComponent<MeshFilter>();
            }

            if (meshFilter.mesh == null)
            {
                Debug.Log("mesh是空的，为什么为什么");
                // 假设您有一个默认的 Mesh 资源
                Mesh defaultMesh = Resources.Load<Mesh>("Path/To/DefaultMesh");
                if (defaultMesh != null)
                {
                    meshFilter.mesh = defaultMesh;
                    Debug.Log("Assigned default Mesh to child: " + child.name);
                }
                else
                {
                    Debug.LogError("Failed to load default Mesh for child: " + child.name);
                }
            }

            if (meshRenderer == null)
            {
                Debug.LogError("MeshRenderer missing in child: " + child.name);
                meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
            }

            if (meshRenderer.material == null)
            {
                // 假设您有一个默认的材质资源
                Material defaultMaterial = Resources.Load<Material>("Path/To/DefaultMaterial");
                if (defaultMaterial != null)
                {
                    meshRenderer.material = defaultMaterial;
                    Debug.Log("Assigned default Material to child: " + child.name);
                }
                else
                {
                    Debug.LogError("Failed to load default Material for child: " + child.name);
                }
            }
        }

        // 保存预制件
        PrefabUtility.SaveAsPrefabAsset(wholeObj, prefabPath);
        Debug.Log("Prefab saved successfully at path: " + prefabPath);
    }

    private void OnScrollInventory(InputAction.CallbackContext context)
    {
        
        float scrollValue = context.ReadValue<Vector2>().y;

        if (scrollValue > 0.2f)
        {
            currentSlot = (currentSlot - 1 + inventoryButtons.Length) % inventoryButtons.Length;
        }
        else if (scrollValue < 0.2f)
        {
            currentSlot = (currentSlot + 1) % inventoryButtons.Length;
        }

        UpdateButtonColors();
    }


}

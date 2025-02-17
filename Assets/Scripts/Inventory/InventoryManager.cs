using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public Button[] inventoryButtons=new Button[6];
    public InventoryItemInfo[] inventorySlotsInfo = new InventoryItemInfo[6];


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
        if (itemInHand == null&& IsInventorySlotEmpty(currentSlot))
            return;

        if (IsInventorySlotEmpty(currentSlot))
        {
            string slotName=GetInventoryName();
            _inventoryIconCapturer.CaptureIcon(slotName);

            Image image = inventoryButtons[currentSlot].GetComponent<Image>();

            string fileName = "Assets/Resources/Inventory/InventoryItemIcons/"+ slotName + ".png";
            if (File.Exists(fileName))
            {
                byte[] fileData = File.ReadAllBytes(fileName);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
            }

            
            InventoryItemInfo newItemInfo = new InventoryItemInfo
            {
                obj = itemInHand.gameObject,
                name = slotName,
            };
            List<GrabableObjectComponent> AllComponent = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(itemInHand);
            AllComponent.Add(itemInHand);
            GameObject wholeObj = new GameObject(slotName);
            wholeObj.transform.SetParent(_parent.transform);
            foreach(var obj in AllComponent)
                obj.transform.SetParent(wholeObj.transform);

            PrefabUtility.SaveAsPrefabAsset(wholeObj, _prefabPath+slotName+".prefab");
            Destroy(wholeObj);
            inventorySlotsInfo[currentSlot] = newItemInfo;
            Debug.Log("Stored item in slot " + currentSlot);

        }

        else
        {
            Debug.Log("qwertyuiop[");
            GameObject dropped = new GameObject("dropped");
            Image image = inventoryButtons[currentSlot].GetComponent<Image>();
            image.sprite = null;
            dropped.transform.SetParent(_GrabableObjs.transform);
            dropped = Resources.Load<GameObject>(_prefabPath+ GetInventoryName() + ".prefab");
            if (dropped != null)
            {
                Instantiate(dropped, Vector3.zero, Quaternion.identity);
            }
        }
    }

    private bool IsInventorySlotEmpty(int slotIndex)
    {
        return inventorySlotsInfo[slotIndex] == null;
    }

    private void OnScrollInventory(InputAction.CallbackContext context)
    {
        if (Keyboard.current[Key.LeftCtrl].isPressed)
            return;
        float scrollValue = context.ReadValue<float>();

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

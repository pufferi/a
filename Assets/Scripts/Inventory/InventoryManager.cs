using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;


public class InventoryManager : MonoBehaviour
{
    public Button[] inventoryButtons = new Button[6];
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
    private GameObject _GrabableObjs; // This is just an entry in the Hierarchy, convenient for managing these objects later

    private string _prefabPath = "Assets/Resources/Inventory/InventoryPrefebs/";
    private string _inventoryIconPath = "Assets/Resources/Inventory/InventoryItemIcons/";

    public Camera playerCam;

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

        for (int i = 0; i < 6; i++)
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

        if (itemInHand == null && isInventorySlotEmpty[currentSlot])
            return;

        bool isItemInHandNormal = false;
        
        if (isInventorySlotEmpty[currentSlot]) // store
        {
            if (itemInHand.objID >= 0)
                isItemInHandNormal = true;

            string slotName = GetInventoryName();
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

            if (isItemInHandNormal)
            {
                List<GrabableObjectComponent> AllComponent = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(itemInHand);
                AllComponent.Add(itemInHand);
                GameObject wholeObj = new GameObject(slotName);
                wholeObj.transform.SetParent(_parent.transform);

                foreach (var obj in AllComponent)
                {
                    Debug.Log(obj.gameObject.GetComponent<MeshRenderer>().material);
                    obj.transform.SetParent(wholeObj.transform);
                }

                wholeObj.SetActive(false);
            }
            else
            {
                itemInHand.name = slotName;
                itemInHand.transform.SetParent(_parent.transform);
                itemInHand.gameObject.SetActive(false);
            }
            Debug.Log("Stored item in slot " + currentSlot);
            playerGrabItems.grabbedObject = null;
        }
        else // drop
        {
            
            string slotName = GetInventoryName();
            Transform storedItem = _parent.transform.Find(slotName);

            if (storedItem != null)
            {
                //check if isItemInHandNormal
                if (storedItem.GetComponent<Rigidbody>()==null)
                    isItemInHandNormal=true;

                storedItem.gameObject.SetActive(true);
                storedItem.position = new Vector3(
                    playerGrabItems.hand.position.x + playerCam.transform.forward.x * 0.5f,
                    Mathf.Max(storedItem.position.y, playerGrabItems.hand.position.y + playerCam.transform.forward.y * 0.5f),
                    playerGrabItems.hand.position.z + playerCam.transform.forward.z * 0.5f
                );
                

                //storedItem.SetParent(_GrabableObjs.transform);
                if (!isItemInHandNormal)
                {
                    storedItem.GetComponent<Rigidbody>().isKinematic = false;
                    storedItem.name = "obj_" + storedItem.GetComponent<GrabableObjectComponent>().objID;
                    storedItem.SetParent(_GrabableObjs.transform);
                }

                // This foreach loop caused me a lot of trouble..........
                //else
                //{
                //    Debug.Log(storedItem.childCount);
                //    foreach (Transform child in storedItem)
                //    {
                //        Debug.Log("childis here");
                //        child.GetComponent<Rigidbody>().isKinematic = false;
                //        child.SetParent(_GrabableObjs.transform);
                //    }
                //    //Destroy(storedItem.gameObject);
                //}
                else
                {
                    List<Transform> children = new List<Transform>();
                    foreach (Transform child in storedItem)
                    {
                        children.Add(child);
                    }

                    foreach (Transform child in children)
                    {
                        Debug.Log("child is here");
                        child.GetComponent<Rigidbody>().isKinematic = false;
                        child.SetParent(_GrabableObjs.transform);
                        Debug.Log("child    " + child.transform.position);
                        child.position = new Vector3(
                    playerGrabItems.hand.position.x + playerCam.transform.forward.x * 0.5f,
                    Mathf.Max(storedItem.position.y, playerGrabItems.hand.position.y + playerCam.transform.forward.y * 0.5f),
                    playerGrabItems.hand.position.z + playerCam.transform.forward.z * 0.5f
                );

                    }

                    Destroy(storedItem.gameObject);
                }

                Image image = inventoryButtons[currentSlot].GetComponent<Image>();
                image.sprite = null;
                Debug.Log("Dropped item from slot " + currentSlot);
            }
            else
            {
                Debug.LogError("Failed to find stored item in slot: " + currentSlot);
            }

            string iconPath = _inventoryIconPath + slotName + ".png";
            if (File.Exists(iconPath))
            {
                File.Delete(iconPath);
            }

            isInventorySlotEmpty[currentSlot] = true;
        }
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

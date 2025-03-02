using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryIconCapturer : MonoBehaviour
{
    public Camera captureCamera;
    public GameObject targetObject;
    public LayerMask Layer_Capture;
    private int _width = 512;
    private int _height = 512;
    //public InputActionAsset inputActions;

    //private InputAction shootAction;

    public PlayerGrabItems playerGrabItems;
    private void Start()
    {
        //var playerMap = inputActions.FindActionMap("Player");

        //shootAction = playerMap.FindAction("Shoot");

        //shootAction.Enable();
        //shootAction.performed += OnCapture;
    }

    public void CaptureIcon(string SlotName)
    {
        GrabableObjectComponent grabbed = playerGrabItems.grabbedObject;
        if (grabbed == null)
            return;
        List<GrabableObjectComponent> connects = GrabableObejectGroupingManager.Instance.GetAllConnectObjects(grabbed);
        //string ItemName = grabbed.name;
        string ItemName = SlotName;

        grabbed.gameObject.layer = LayerMask.NameToLayer("Layer_Capture");
        if(connects != null)
            foreach (var connect in connects)
            {
                connect.gameObject.layer = LayerMask.NameToLayer("Layer_Capture");
            }


        Camera tempCamera = new GameObject("Layer_Capture").AddComponent<Camera>();
        tempCamera.CopyFrom(captureCamera);
        tempCamera.clearFlags = CameraClearFlags.SolidColor;
        tempCamera.backgroundColor = new Color(0, 0, 0, 0);
        tempCamera.cullingMask = 1 << LayerMask.NameToLayer("Layer_Capture");

        RenderTexture renderTexture = new RenderTexture(_width, _height, 24, RenderTextureFormat.ARGB32);
        tempCamera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(_width, _height, TextureFormat.ARGB32, false);

        tempCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
        screenshot.Apply();

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(tempCamera.gameObject);

        byte[] bytes = screenshot.EncodeToPNG();
        //\Resources\Inventory\InventoryItemIcons

        string folderPath = Application.dataPath + "/InventoryItemIcons";
        if (!Directory.Exists(folderPath))
        {
            // 创建文件夹
            Directory.CreateDirectory(folderPath);
            Debug.Log("Folder created at: " + folderPath);
        }
        string newFilePath = Path.Combine(folderPath, ItemName + ".png");


        Debug.Log("THE PATH OF MY STORED IMAGE IS:     ");
        Debug.Log(newFilePath);


        System.IO.File.WriteAllBytes(newFilePath, bytes);
        Debug.Log("AND DOES IT EXIST?    " + System.IO.File.Exists(newFilePath));

        Debug.Log("Screenshot saved");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TestMail : MonoBehaviour
{
    public MailHandler mailHandler;

    string path = "Assets/Images/Mails/frEUrd 03,11.png";

    string mailName = "frEUrd 03,11";
    string mailName2 = "Community Administrator 03,10";

    public Image image;
    void Start()
    {
        mailHandler.CreatMail(mailName);
        mailHandler.CreatMail(mailName2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //public void OpenMail(string fileName)
    //{
    //    image.gameObject.SetActive(true);
    //    fileName = path;
    //    if (File.Exists(fileName))
    //    {
    //        加载PNG文件并转换为Texture2D
    //        byte[] fileData = File.ReadAllBytes(fileName);
    //        Texture2D tex = new Texture2D(2, 2);
    //        tex.LoadImage(fileData);

    //        创建Sprite并设置到Image组件
    //       Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    //        image.sprite = sprite;
    //    }
    //    else
    //    {
    //        Debug.LogError("File not found at: " + fileName);
    //    }
    //}
}

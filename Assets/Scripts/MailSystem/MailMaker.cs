using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MailMaker : MonoBehaviour
{

    public string nameTo;
    public string nameFrom;
    //public string closingPhrase;
    public string date;

    [TextArea]
    public string content;

    public Image background;

    public Color backGroundColor = Color.white;

    private string file = "Assets/Images/Mails/";

    [SerializeField]
    private TextMeshProUGUI Name;
    [SerializeField]
    private TextMeshProUGUI Date;
    [SerializeField]
    private TextMeshProUGUI Content;

    void Start()
    {
        Name.text = nameTo+",";
        Date.text = nameFrom+"\n\n"+date;
        Content.text = content;
        background.color= backGroundColor;
        string mailName=nameFrom+" "+date+".png";

        CaptureMail(file+mailName);
    }

    

    void CaptureMail(string fileName)
    {
        UnityEngine.ScreenCapture.CaptureScreenshot(fileName);
    }

}

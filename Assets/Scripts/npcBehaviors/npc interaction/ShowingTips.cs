using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingTips : MonoBehaviour
{
    private GameObject PunctTip;
    private GameObject PunctTipInstance;

    void Start()
    {
        PunctTip = Resources.Load<GameObject>("punct");
    }

    public void ShowPunctTip(Transform parent)
    {
        if (PunctTipInstance == null)
        {
            PunctTipInstance = Instantiate(PunctTip, parent);
            PunctTipInstance.SetActive(true);
            PunctTip.transform.position = new Vector3(PunctTip.transform.position.x, PunctTip.transform.position.y, PunctTip.transform.position.z);
        }
    }

    public void HidePuctTip()
    {
        if (PunctTipInstance != null)
        {
            Destroy(PunctTipInstance);
            PunctTipInstance = null;
        }
    }
}

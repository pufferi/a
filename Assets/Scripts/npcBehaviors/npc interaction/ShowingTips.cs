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

    float OffsetOnYPos = 0;
    public void ShowPunctTip(Transform parent, float OffsetOnYPos = 0)
    {
        if (PunctTipInstance == null)
        {
            this.OffsetOnYPos = OffsetOnYPos;
            PunctTipInstance = Instantiate(PunctTip, parent);
            PunctTipInstance.SetActive(true);
            PunctTip.transform.position = new Vector3(PunctTip.transform.position.x, PunctTip.transform.position.y+OffsetOnYPos, PunctTip.transform.position.z);
        }
    }

    public void HidePuctTip()
    {
        if (PunctTipInstance != null)
        {
            Destroy(PunctTipInstance);
            PunctTip.transform.position = new Vector3(PunctTip.transform.position.x, PunctTip.transform.position.y - OffsetOnYPos, PunctTip.transform.position.z);
            PunctTipInstance = null;
        }
    }
}

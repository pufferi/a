using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform _content;
    
    [SerializeField]
    private GameObject _contentPrefab;



    [Space(10)]
    [Header("Scroll View Events")]

    [SerializeField]
    private MailButtonEvent _eventMailClicked;

    [SerializeField]
    private MailButtonEvent _eventMailOnSelected;

    [SerializeField]
    private MailButtonEvent _eventMailOnSubmit;



    [Space(10)]
    [Header("Default Selected Index")]

    [SerializeField]
    private int _defaultSelectedIndex = 0;


    [Space(10)]
    [Header("Test")]
    [SerializeField]
    private int _testButtonCount = 1;


    private void Start()
    {
        if (_testButtonCount > 0)
        {
            TestCreatMail(_testButtonCount);
            UpdataButtonNavigation();
        }

        SelectChild(_defaultSelectedIndex);
    }

    public void SelectChild(int index)
    {
        int childCount=_content.transform.childCount;

        if (index >= childCount)
            return;

        GameObject childObject=_content.transform.GetChild(index).gameObject;
        MailSlotButton mail = childObject.GetComponent<MailSlotButton>();
        mail.ObtainSelectionFocus();

    }

    private void UpdataButtonNavigation()
    {
        MailSlotButton[] children=_content.transform.GetComponentsInChildren<MailSlotButton>();

        if (children.Length < 2)
            return;
        MailSlotButton mail;
        Navigation navigation;
        for (int i = 0; i < children.Length; i++) 
        {
            mail = children[i];
            navigation = mail.gameObject.GetComponent<Button>().navigation;

            navigation.selectOnUp = GetNavigationUp(i, children.Length);
            navigation.selectOnDown = GetNavigationDown(i, children.Length);

            mail.gameObject.GetComponent<Button>().navigation = navigation;
        }

    }

    private Selectable GetNavigationDown(int indexCurrent, int length)
    {
        MailSlotButton mail;
        if (indexCurrent == length - 1)
        {
            mail=_content.transform.GetChild(0).GetComponent<MailSlotButton>();
        }
        else
        {
            mail=_content.transform.GetChild(indexCurrent+1).GetComponent<MailSlotButton>();
        }
        return mail.GetComponent<Selectable>();
    }

    private Selectable GetNavigationUp(int indexCurrent, int length)
    {
        MailSlotButton mail;
        if (indexCurrent == 0)
        {
            mail = _content.transform.GetChild(length-1).GetComponent<MailSlotButton>();
        }
        else
        {
            mail = _content.transform.GetChild(indexCurrent - 1).GetComponent<MailSlotButton>();
        }
        return mail.GetComponent<Selectable>();
    }

    private void TestCreatMail(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreatMail("NAme                " + i);
        }
    }
    private void CreatMail(string mailInformation)
    {
        GameObject obj;
        MailSlotButton mail;
        obj = Instantiate(_contentPrefab, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(_content.transform);
        obj.transform.localPosition =new Vector3();
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localRotation = Quaternion.Euler(new Vector3());
        obj.name = mailInformation;


        mail= obj.GetComponent<MailSlotButton>();
        mail.MailInformationText = mailInformation;

        mail.OnSelectEvent.AddListener((ItemButtom) => { HandleEventEmailOnSelect(mail); });
        mail.OnClickEvent.AddListener((ItemButtom) => { HandleEventEmailOnClick(mail); });
        mail.OnSubmitEvent.AddListener((ItemButtom) => { HandleEventEmailOnSubmit(mail); });
    }

    private void HandleEventEmailOnSubmit(MailSlotButton mail)
    {
        _eventMailOnSubmit.Invoke(mail);
    }

    private void HandleEventEmailOnClick(MailSlotButton mail)
    {
        _eventMailClicked.Invoke(mail);
    }

    private void HandleEventEmailOnSelect(MailSlotButton mail)
    {
        AutoScrollView autoScrollView=GetComponent<AutoScrollView>();
        autoScrollView.HandleOnSelectChange(mail.gameObject);

        _eventMailOnSelected.Invoke(mail);
    }
}

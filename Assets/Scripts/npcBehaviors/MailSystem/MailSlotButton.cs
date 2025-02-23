using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class MailSlotButton : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField]
    private TMP_Text _mailInformation;

    [SerializeField]
    private MailButtonEvent _onSelectEvent; 
    
    [SerializeField]
    private MailButtonEvent _onSubmitEvent;

    [SerializeField]
    private MailButtonEvent _onClickEvent;

    //public string mailName="";


    public MailButtonEvent OnSelectEvent { get => _onSelectEvent; set => _onSelectEvent = value; }
    public MailButtonEvent OnSubmitEvent { get => _onSubmitEvent; set => _onSubmitEvent = value; }
    public MailButtonEvent OnClickEvent { get => _onClickEvent; set => _onClickEvent = value; }
    public string MailInformationText { get => _mailInformation.text; set => _mailInformation.text = value; }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClickEvent.Invoke(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _onSelectEvent.Invoke(this);
    }


    public void OnSubmit(BaseEventData eventData)
    {
        _onSubmitEvent.Invoke(this);
    }

    public void ObtainSelectionFocus()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);

        _onSelectEvent.Invoke(this);
    }


}

[System.Serializable]
public class MailButtonEvent: UnityEvent<MailSlotButton> { }

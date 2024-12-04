using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeginnerTutorialProcess : MonoBehaviour
{
    [SerializeField]
    private DialogueConversations conversation1;

    [SerializeField]
    private DialogueConversations conversation2;

    private bool isConversation1triggered;

    private bool isConversation2triggered;

    private bool isSwatterMade;

    private bool isSwatterGavetoFrog;

    public ShowingTips ShowingTipsController;
    public InputActionAsset inputActions;
    private InputAction talkAction;

    private bool inRegion;

    public GameObject Web; // ²ÔÓ¬ÅÄµÄÍø
    public GameObject Bar;

    private Rigidbody webRigdbody;
    private Rigidbody barRigdbody;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        talkAction = playerMap.FindAction("Talk");
        talkAction.performed += OnTalkAction;
        talkAction.Enable();

        webRigdbody = Web.GetComponent<Rigidbody>();
        barRigdbody = Bar.GetComponent<Rigidbody>();
    }

    private void OnTalkAction(InputAction.CallbackContext context)
    {
        if (inRegion&&!isConversation1triggered)
        {
            isConversation1triggered = true;
            ShowingTipsController.HidePuctTip();
            conversation1.onDialogueEnd += OnConversation1Complete;
            StartCoroutine(StartDialogue(conversation1));
            Debug.Log("Okkkkk");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = true;
            if (!isConversation1triggered) ShowingTipsController.ShowPunctTip(gameObject.transform);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = false;
            ShowingTipsController.HidePuctTip();
        }
    }

    private void OnConversation1Complete()
    {
        Debug.Log("Dialogue has ended.");
        webRigdbody.isKinematic = false;
        barRigdbody.isKinematic = false;
    }

    private IEnumerator StartDialogue(DialogueConversations conversation)
    {
        DialogueManager.instance.StartDialogue(conversation.dialogue);
       yield return null;
    }
}

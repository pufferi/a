using Dialogue;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrogStory_1 : MonoBehaviour
{
    [SerializeField]
    private DialogueConversations conversation1;

    [SerializeField]
    private DialogueConversations conversation2;


    public Transform FrogTransform;
    public Transform FrogHandPos;

    private bool isConversation1triggered;

    private bool isConversation2triggered;

    public ShowingTips ShowingTipsController;
    public InputActionAsset inputActions;
    private InputAction talkAction;

    private bool inRegion;
    private bool isSwatterMade = false;
    private bool isSwatterInHand = false;

    public GameObject Web; // 苍蝇拍的网
    public GameObject Bar;

    private Rigidbody webRigidbody;
    private Rigidbody barRigidbody;

    public NPCBehaviorController npcbehaviour;

    public TaskListController tlm;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        talkAction = playerMap.FindAction("Interact");
        talkAction.performed += OnTalk1Action;
        talkAction.Enable();
        webRigidbody = Web.GetComponent<Rigidbody>();
        barRigidbody = Bar.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!isSwatterMade)
            isSwatterMade = IsWebAndBarConnected();
        else
            isSwatterInHand = IsSwatterInHand();
    }

    private void OnTalk1Action(InputAction.CallbackContext context)
    {
        if (inRegion&&!isConversation1triggered)
        {
            isConversation1triggered = true;
            ShowingTipsController.HidePuctTip();
            conversation1.dialogue.onDialogueEnd += OnConversation1Complete;
            StartCoroutine(StartDialogue(conversation1));
            talkAction.performed -= OnTalk1Action;
            talkAction.performed += OnTalk2Action;
        }
    }
    private void OnTalk2Action(InputAction.CallbackContext context)
    {
        if (inRegion && !isConversation2triggered&&isSwatterInHand)
        {
            //拿走苍蝇拍应该要setParent,取消kinematic
            isConversation2triggered = true;
            ShowingTipsController.HidePuctTip();
            conversation2.dialogue.onDialogueEnd += OnConversation2Complete;
            StartCoroutine(StartDialogue(conversation2));
            talkAction.performed -= OnTalk2Action;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = true;
            if (!isConversation1triggered||(IsSwatterInHand() && !isConversation2triggered)) ShowingTipsController.ShowPunctTip(gameObject.transform);
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
        webRigidbody.isKinematic = false;
        barRigidbody.isKinematic = false;
        tlm.AddTask("help the frog to fix the swatter");
    }


    private void OnConversation2Complete()
    {
        // 青蛙走掉
        Vector3 destination = new Vector3(100, FrogTransform.position.y, 50);
        Debug.Log(FrogTransform == null);

        npcbehaviour.Move(FrogTransform, destination, 30f);
        tlm.CompleteTask("help the frog to fix the swatter");

    }



    private IEnumerator StartDialogue(DialogueConversations conversation)
    {
        DialogueManager.instance.StartDialogue(conversation.dialogue);
       yield return null;
    }

    private bool IsWebAndBarConnected()
    {
        Joint[] webJoints = webRigidbody.GetComponents<Joint>();
        Joint[] barJoints = barRigidbody.GetComponents<Joint>();

        bool isConnected = false;

        foreach (var webJoint in webJoints)
        {
            if (webJoint.connectedBody == barRigidbody)
            {
                isConnected = true;
                break;
            }
        }

        if (!isConnected)
        {
            foreach (var barJoint in barJoints)
            {
                if (barJoint.connectedBody == webRigidbody)
                {
                    isConnected = true;
                    break;
                }
            }
        }
        return isConnected;
    }

    private bool IsSwatterInHand()
    {
        if (Web != null && Web.transform.parent != null && Web.transform.parent.name == "GrabCenter") return true;
        if (Bar != null && Bar.transform.parent != null && Bar.transform.parent.name == "GrabCenter") return true;
        return false;
    }


}

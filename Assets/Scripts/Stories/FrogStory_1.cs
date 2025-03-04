using Dialogue;
using System;
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

    private bool _isConversation1triggered;

    private bool _isConversation2triggered;

    public GameObject punctTip;
    public InputActionAsset inputActions;
    private InputAction talkAction;

    private bool inRegion;
    private bool isSwatterMade = false;
    private bool isSwatterInHand = false;

    public GameObject Web;//web of swatter
    public GameObject Bar;

    private Rigidbody webRigidbody;
    private Rigidbody barRigidbody;

    public NPCBehaviorController npcbehaviour;

    public TaskListController tlm;
    public MailHandler mailHandler;
    public GameObject bee;

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
        if (inRegion && !_isConversation1triggered)
        {
            _isConversation1triggered = true;

            punctTip.SetActive(false);
            conversation1.dialogue.onDialogueEnd += OnConversation1Complete;
            StartCoroutine(StartDialogue(conversation1));
            talkAction.performed -= OnTalk1Action;
            talkAction.performed += OnTalk2Action;
        }
    }
    private void OnTalk2Action(InputAction.CallbackContext context)
    {
        if (inRegion && !_isConversation2triggered && isSwatterInHand)
        {
            _isConversation2triggered = true;
            punctTip.SetActive(false);
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
            if (!_isConversation1triggered || (IsSwatterInHand() && !_isConversation2triggered))
                punctTip.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRegion = false;
            punctTip.SetActive(false);
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
        Web.transform.SetParent(FrogHandPos);
        webRigidbody.isKinematic = true;

        barRigidbody.isKinematic = false;
        Bar.transform.SetParent(FrogHandPos);

        Vector3 destination = new Vector3(100, FrogTransform.position.y, 50);

        npcbehaviour.Move(FrogTransform, destination, 30f);
        tlm.CompleteTask("help the frog to fix the swatter");
        mailHandler.CreatMail("Green Frog 03,11");
        mailHandler.SelectChild(0);
        bee.SetActive(true);
        StartCoroutine(DestroyAfterDelay(gameObject, 2f));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
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

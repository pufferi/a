using Dialogue;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeeStory_1 : MonoBehaviour
{
    [SerializeField]
    private DialogueConversations conversation1;

    [SerializeField]
    private DialogueConversations conversation2;


    private bool isConversation1triggered;

    private bool isConversation2triggered;

    public ShowingTips ShowingTipsController;
    public InputActionAsset inputActions;
    private InputAction talkAction;

    private bool _inRegion;
    private bool _isMokaInHand = false;


    public GameObject mokaTop;
    public GameObject mokaBottom;
    public GameObject tree;
    public GameObject puffer;

    private NpcMoveWithNavMesh _beeMove;

    public PlayerGrabItems playerGrabItems;

    public Transform BeeTransform;

    public TaskListController tlm;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        talkAction = playerMap.FindAction("Interact");
        talkAction.performed += OnTalk1Action;
        talkAction.Enable();
        _beeMove=this.GetComponent<NpcMoveWithNavMesh>();
        _beeMove.NpcStartMoveWithPlayer();
    }
    private void Update()
    {
        _isMokaInHand = IsMokaInHand();
    }

    private void OnTalk1Action(InputAction.CallbackContext context)
    {
        if (_inRegion&&!isConversation1triggered)
        {
            isConversation1triggered = true;
            ShowingTipsController.HidePuctTip();
            conversation1.dialogue.onDialogueEnd += OnConversation1Complete;
            _beeMove.NpcStopMove();
            StartCoroutine(StartDialogue(conversation1));
            talkAction.performed -= OnTalk1Action;
            talkAction.performed += OnTalk2Action;
        }
    }
    private void OnTalk2Action(InputAction.CallbackContext context)
    {
        if (_inRegion && !isConversation2triggered && _isMokaInHand)
        {
            isConversation2triggered = true;
            ShowingTipsController.HidePuctTip();
            conversation2.dialogue.onDialogueEnd += OnConversation2Complete;
            _beeMove.NpcStopMove();
            StartCoroutine(StartDialogue(conversation2));
            talkAction.performed -= OnTalk2Action;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _inRegion = true;
            if (!isConversation1triggered || (IsMokaInHand() && !isConversation2triggered)) 
                ShowingTipsController.ShowPunctTip(gameObject.transform);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _inRegion = false;
            ShowingTipsController.HidePuctTip();
        }
    }

    private void OnConversation1Complete()
    {
        _beeMove.NpcStopMoveWithPlayer();
        _beeMove.NpcStartMove();
        tlm.AddTask("Find the moka pot for the bee");
    }


    private void OnConversation2Complete()
    {
        tree.SetActive(true);
        _beeMove.NpcStartMove();

        tree.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 5f, this.transform.position.z);
        tlm.CompleteTask("Find the moka pot for the bee");
        puffer.SetActive(true);
        this.enabled = false;
    }



    private IEnumerator StartDialogue(DialogueConversations conversation)
    {
        DialogueManager.instance.StartDialogue(conversation.dialogue);
       yield return null;
    }

    private bool IsMokaTopAndBottomConnected()
    {
        return mokaTop.GetComponent<GrabableObjectComponent>().groupID == mokaBottom.GetComponent<GrabableObjectComponent>().groupID;
    }

    private bool IsMokaInHand()
    {
        if (playerGrabItems.grabbedObject == null)
            return false;
        int id= playerGrabItems.grabbedObject.GetComponent<GrabableObjectComponent>().objID;
        if (id == mokaTop.GetComponent<GrabableObjectComponent>().objID||id== mokaBottom.GetComponent<GrabableObjectComponent>().objID)
            if(IsMokaTopAndBottomConnected())return true;
        return false;
    }


}

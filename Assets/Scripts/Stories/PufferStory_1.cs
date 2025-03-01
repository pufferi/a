using Dialogue;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PufferStory_1 : MonoBehaviour
{
    [SerializeField]
    private DialogueConversations conversation1;

    private bool isConversation1triggered;


    public InputActionAsset inputActions;
    private InputAction talkAction;

    private bool _inRegion;

    public GameObject fishRod;
    public GameObject punctTip;

    private NpcMoveWithNavMesh _pufferMove;

    public PlayerGrabItems playerGrabItems;

    public Transform PufferTransform;

    public TaskListController tlm;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        talkAction = playerMap.FindAction("Interact");
        talkAction.performed += OnTalk1Action;
        talkAction.Enable();
        _pufferMove = this.GetComponent<NpcMoveWithNavMesh>();
    }
    private void Update()
    {
     
    }

    private void OnTalk1Action(InputAction.CallbackContext context)
    {
        if (_inRegion && !isConversation1triggered)
        {
            isConversation1triggered = true;
            punctTip.SetActive(false);
            conversation1.dialogue.onDialogueEnd += OnConversation1Complete;
            _pufferMove.NpcStopMove();
            StartCoroutine(StartDialogue(conversation1));
            talkAction.performed -= OnTalk1Action;
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!isConversation1triggered && other.gameObject.tag == "Player")
        {
            punctTip.SetActive(true);

            _inRegion = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _inRegion = false;
            punctTip.SetActive(false);
        }
    }

    private void OnConversation1Complete()
    {
        _pufferMove.NpcStartMove();
        tlm.AddTask("Try Fishing");
        fishRod.SetActive(true);
        fishRod.transform.position = PufferTransform.position;
        this.enabled = false;
    }



    private IEnumerator StartDialogue(DialogueConversations conversation)
    {
        DialogueManager.instance.StartDialogue(conversation.dialogue);
        yield return null;
    }


}

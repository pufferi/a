using Dialogue;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStory_1 : MonoBehaviour
{
    [SerializeField]
    private DialogueConversations conversation1;
    [SerializeField]
    private DialogueConversations conversation2;
    [SerializeField]
    private DialogueConversations conversation3;

    public TaskListController tlc;

    private bool isConversation1triggered=false;

    private PlayerChangeScene playerChangeScene;

    //public Transform SofaBase;
    public float SofaBaseRadius;

    public Transform AlienLetter;
    public float AlienLetterRadius;

    public GameObject TutorialCanvas;

    public GameObject tip1;
    public GameObject tip2;

    private bool hasPickedLetter = false;
    private bool hasTriggeredBaseConversation=false;
    private bool hasTriggeredLetterConversation=false;

    public InAreaChecker inSofaAreaChecker;
    public InAreaChecker inLetterAreaChecker;


    public InputActionAsset inputActions;
    private InputAction InteractAction;

    public MailHandler mailHandler;

    public GameObject BaseTrigger;


    void Start()
    {
        playerChangeScene = GetComponent<PlayerChangeScene>();
        playerChangeScene.InactivateChangeSceneFunction();
        PlayerStateManager.Instance.PlayerMoveLock();

        var playerMap = inputActions.FindActionMap("Player");
        InteractAction = playerMap.FindAction("Interact");
        InteractAction.performed += ToggleLetter;
        InteractAction.Enable();

        mailHandler.CreatMail("Community Administrator 03,10");

        ShowDialogue1();
    }

    private void Update()
    {
        if (hasPickedLetter&& !hasTriggeredBaseConversation)
        {
            if (inSofaAreaChecker.inArea)
            {
                Destroy(tip2);
                hasTriggeredBaseConversation = true;
                conversation3.dialogue.onDialogueEnd += OnConversation3Complete;
                StartCoroutine(StartDialogue(conversation3));
            }
        }
    }

    private void ToggleLetter(InputAction.CallbackContext context)
    {
        if (inLetterAreaChecker.inArea&& !hasTriggeredLetterConversation) 
        {
            conversation2.dialogue.onDialogueEnd += OnConversation2Complete;
            StartCoroutine(StartDialogue(conversation2));
            hasTriggeredLetterConversation=true;
        }
    }

    private void ShowDialogue1()
    {
        if (!isConversation1triggered)
        {
            isConversation1triggered = true;
            conversation1.dialogue.onDialogueEnd += OnConversation1Complete;
            StartCoroutine(StartDialogue(conversation1));
        }
    }


    private void OnConversation1Complete()
    {
        PlayerStateManager.Instance.PlayerMoveUnlock();
    }


    private void OnConversation2Complete()
    {
        AlienLetter.gameObject.SetActive(false);
        Destroy(tip1);
        tip2.SetActive(true);
        hasPickedLetter = true;
        mailHandler.CreatMail("frEUrd 03,11");
    }
    private void OnConversation3Complete()
    {
        tlc.AddTask("Gather some materials and build a bed on the base.");
        BaseTrigger.SetActive(false);
        playerChangeScene.ActivateChangeSceneFunction();
    }

    private IEnumerator StartDialogue(DialogueConversations conversation)
    {
        DialogueManager.instance.StartDialogue(conversation.dialogue);
        yield return null;
    }

}

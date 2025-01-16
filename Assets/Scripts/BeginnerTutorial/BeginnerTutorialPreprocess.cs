using Dialogue;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeginnerTutorialPreprocess : MonoBehaviour
{
    [SerializeField]
    private DialogueConversations conversation1;
    [SerializeField]
    private DialogueConversations conversation2;
    [SerializeField]
    private DialogueConversations conversation3;

    public TaskListController tlm;

    private bool isConversation1triggered=false;

    private PlayerChangeScene playerChangeScene;

    public Transform SofaBase;
    public float SofaBaseRadius;

    public Transform AlienLetter;
    public float AlienLetterRadius;

    public GameObject TutorialCanvas;

    public GameObject tip1;
    public GameObject tip2;
    public string targetTag = "Player"; // 物体A的标签
    private bool inLetterRegion;
    private bool inSofaBaseRegion;
    private bool hasPickedLetter = false;
    private bool hasTriggeredBaseConversation=false;
    private bool hasTriggeredLetterConversation=false;

    public PlayerMovement playerMovement;


    public InputActionAsset inputActions;
    private InputAction InteractAction;



    void Start()
    {
        playerChangeScene = GetComponent<PlayerChangeScene>();
        playerChangeScene.InactivateChangeSceneFunction();

        playerMovement.playerStill = true;

        var playerMap = inputActions.FindActionMap("Player");
        InteractAction = playerMap.FindAction("Interact");
        InteractAction.performed += ToggleLetter;
        InteractAction.Enable();
        ShowDialogue1();
    }

    private void Update()
    {

        if (!hasPickedLetter) { AlienLetterProcess(); }

        if (hasPickedLetter&& !hasTriggeredBaseConversation)
        {
            Collider[] hitColliders = Physics.OverlapSphere(SofaBase.position, SofaBaseRadius);
            inSofaBaseRegion = false;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag(targetTag))
                {
                    inSofaBaseRegion = true;
                    break;
                }
            }
            if (inSofaBaseRegion)
            {
                Destroy(tip2);
                hasTriggeredBaseConversation = true;
                conversation3.dialogue.onDialogueEnd += OnConversation3Complete;
                StartCoroutine(StartDialogue(conversation3));
            }
        }
    }

    private void AlienLetterProcess()
    {
        Collider[] hitColliders = Physics.OverlapSphere(AlienLetter.position, AlienLetterRadius);
        inLetterRegion = false;
        int i = 0;
        foreach (var hitCollider in hitColliders)
        {
            i++;
            if (hitCollider.CompareTag(targetTag))
            {
                inLetterRegion = true;
                return;
            }
        }
        if (i == hitColliders.Length) inLetterRegion = false;
    }

    private void ToggleLetter(InputAction.CallbackContext context)
    {
        if (inLetterRegion&& !hasTriggeredLetterConversation) 
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
        playerMovement.playerStill = false;
    }


    private void OnConversation2Complete()
    {
        AlienLetter.gameObject.SetActive(false);
        Destroy(tip1);
        tip2.SetActive(true);
        hasPickedLetter = true;
    }
    private void OnConversation3Complete()
    {
        tlm.AddTask("Gather some materials and build a sofa on the base.");
        playerChangeScene.ActivateChangeSceneFunction();
    }
    private IEnumerator StartDialogue(DialogueConversations conversation)
    {
        DialogueManager.instance.StartDialogue(conversation.dialogue);
        yield return null;
    }

}

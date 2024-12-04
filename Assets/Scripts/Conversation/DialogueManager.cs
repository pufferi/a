using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Player;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance;
        public Image characterIcon;
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI dialogueArea;
        public GameObject DialogueCanvas;
        private Queue<DialogueLine> lines = new Queue<DialogueLine>();
        public float typingSpeed;
        public Transform npcIconPosition;
        public Transform playerIconPosition;
        private bool isTyping = false;

        public InputActionAsset inputActions;
        private InputAction leftclickAction;

        public PlayerMovement playermovement;//¿ØÖÆÍæ¼ÒÔÝÍ£

        public TypingSpeedSliderHandler TypingSpeedSliderHandler;

        private void Awake()
        {
            if (instance == null) instance = this;
            dialogueArea.text = "";

            var ConversationMap = inputActions.FindActionMap("Conversation");
            leftclickAction = ConversationMap.FindAction("LeftClick");
            leftclickAction.Enable();
        }

        void Update()
        {
            typingSpeed = TypingSpeedSliderHandler.TypingSpeed;
            if (lines.Count != 0)
            {
                if (leftclickAction.triggered)
                {
                    if (dialogueArea.text == lines.Peek().dialogueline)
                    {
                        lines.Dequeue();
                        DisplayNextDialogueLine();
                    }
                    else
                    {
                        if (isTyping)
                        {
                            isTyping = false;
                            dialogueArea.text = lines.Peek().dialogueline;
                        }
                    }
                }
            }
            else
            {
                EndDialogue();
            }
        }

        public void DisplayNextDialogueLine()
        {
            if (lines.Count == 0)
            {
                EndDialogue();
                return;
            }
            DialogueLine currentLine = lines.Peek();
            characterIcon.sprite = currentLine.character.icon;
            characterName.text = currentLine.character.characterName;

            // Adjust the position of the character icon based on the character's name
            if (currentLine.character.characterName == "Player")
            {
                characterIcon.rectTransform.position = playerIconPosition.position;
            }
            else
            {
                characterIcon.rectTransform.position = npcIconPosition.position;
            }
            isTyping = true;
            dialogueArea.text = "";
            StartCoroutine(TypeSentence(currentLine));
        }

        public void StartDialogue(Dialogue dialogue)
        {
            DialogueCanvas.SetActive(true);
            lines.Clear();
            foreach (DialogueLine dialogueline in dialogue.dialoguelines)
            {
                lines.Enqueue(dialogueline);
            }
            DisplayNextDialogueLine();
        }


        private IEnumerator TypeSentence(DialogueLine currentLine)
        {
            dialogueArea.text = ""; 
            foreach (char c in currentLine.dialogueline.ToCharArray())
            {
                if (!isTyping)
                {
                    dialogueArea.text = currentLine.dialogueline;
                    yield break;
                }

                dialogueArea.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
        }



        void EndDialogue()
        {
            DialogueCanvas.SetActive(false);
            playermovement.playerStill = false;
            // Any additional actions after dialogue ends
        }
    }
}

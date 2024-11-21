




////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;
////using UnityEngine.UI;
////using TMPro;

////public class DialogueManager : MonoBehaviour
////{
////    public static DialogueManager instance;
////    public Image characterIcon;
////    public TextMeshProUGUI characterName;
////    public TextMeshProUGUI dialogueArea;
////    public GameObject DialogueCanvas;
////    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
////    public float typingSpeed;
////    public Transform npcIconPosition;
////    public Transform playerIconPosition;

////    private void Awake()
////    {
////        Debug.Log("start");
////        if (instance == null) instance = this;
////        dialogueArea.text = "";
////    }

////    void Update()
////    {
////        if (lines.Count != 0)
////        {
////            if (Input.GetMouseButtonDown(0))
////            {
////                if (dialogueArea.text == lines.Peek().dialogueline)
////                {
////                    lines.Dequeue();
////                    DisplayNextDialogueLine();
////                }
////                else
////                {
////                    StopAllCoroutines();
////                    dialogueArea.text = lines.Peek().dialogueline;
////                }
////            }
////        }
////        else
////        {
////            EndDialogue();
////        }
////    }
////    public void DisplayNextDialogueLine()
////    {
////        if (lines.Count == 0)
////        {
////            EndDialogue();
////            return;
////        }
////        DialogueLine currentLine = lines.Peek();
////        characterIcon.sprite = currentLine.character.icon;
////        characterName.text = currentLine.character.characterName;

////        // Adjust the position of the character icon based on the character's name
////        if (currentLine.character.characterName == "Player") // Adjust this to match your player's name
////        {
////            characterIcon.rectTransform.position = playerIconPosition.position;
////        }
////        else // NPC
////        {
////            characterIcon.rectTransform.position = npcIconPosition.position;
////        }

////        Debug.Log("change character Name");
////        StopAllCoroutines();
////        StartCoroutine(TypeSentence(currentLine));
////    }
////    public IEnumerator StartDialogue(Dialogue dialogue)
////    {
////        DialogueCanvas.SetActive(true);
////        lines.Clear();
////        foreach (DialogueLine dialogueline in dialogue.dialoguelines)
////        {
////            lines.Enqueue(dialogueline);
////        }
////        yield return DisplayNextDialogueLineCoroutine();
////    }

////    private IEnumerator DisplayNextDialogueLineCoroutine()
////    {
////        while (lines.Count > 0)
////        {
////            DialogueLine currentLine = lines.Dequeue();
////            characterIcon.sprite = currentLine.character.icon;
////            characterName.text = currentLine.character.characterName;

////            // Adjust the position of the character icon based on the character's name
////            if (currentLine.character.characterName == "Player")
////            {
////                characterIcon.rectTransform.position = playerIconPosition.position;
////            }
////            else
////            {
////                characterIcon.rectTransform.position = npcIconPosition.position;
////            }

////            Debug.Log("change character Name");
////            yield return StartCoroutine(TypeSentence(currentLine));
////            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
////        }
////        EndDialogue();
////    }

////    private IEnumerator TypeSentence(DialogueLine currentLine)
////    {
////        dialogueArea.text = "";
////        foreach (char c in currentLine.dialogueline.ToCharArray())
////        {
////            dialogueArea.text += c;
////            yield return new WaitForSeconds(typingSpeed);
////        }
////    }

////    void EndDialogue()
////    {
////        DialogueCanvas.SetActive(false);
////    }
////}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class DialogueManager : MonoBehaviour
//{
//    public static DialogueManager instance;
//    public Image characterIcon;
//    public TextMeshProUGUI characterName;
//    public TextMeshProUGUI dialogueArea;
//    public GameObject DialogueCanvas;
//    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
//    public float typingSpeed;
//    public Transform npcIconPosition;
//    public Transform playerIconPosition;

//    private void Awake()
//    {
//        Debug.Log("start");
//        if (instance == null) instance = this;
//        dialogueArea.text = "";
//    }

//    void Update()
//    {
//        if (lines.Count != 0)
//        {
//            if (Input.GetMouseButtonDown(0))
//            {
//                if (dialogueArea.text == lines.Peek().dialogueline)
//                {
//                    lines.Dequeue();
//                    DisplayNextDialogueLine();
//                }
//                else
//                {
//                    StopAllCoroutines();
//                    dialogueArea.text = lines.Peek().dialogueline;
//                }
//            }
//        }
//        else
//        {
//            EndDialogue();
//        }
//    }

//    public void DisplayNextDialogueLine()
//    {
//        if (lines.Count == 0)
//        {
//            EndDialogue();
//            return;
//        }
//        DialogueLine currentLine = lines.Peek();
//        characterIcon.sprite = currentLine.character.icon;
//        characterName.text = currentLine.character.characterName;
//        // Adjust the position of the character icon based on the character's name
//        if (currentLine.character.characterName == "Player")
//        {
//            characterIcon.rectTransform.position = playerIconPosition.position;
//        }
//        else
//        {
//            characterIcon.rectTransform.position = npcIconPosition.position;
//        }
//        Debug.Log("change character Name");
//        StopAllCoroutines();
//        StartCoroutine(TypeSentence(currentLine));
//    }

//    public IEnumerator StartDialogue(Dialogue dialogue)
//    {
//        DialogueCanvas.SetActive(true);
//        lines.Clear();
//        foreach (DialogueLine dialogueline in dialogue.dialoguelines)
//        {
//            lines.Enqueue(dialogueline);
//        }
//        yield return DisplayNextDialogueLineCoroutine();
//    }

//    private IEnumerator DisplayNextDialogueLineCoroutine()
//    {
//        while (lines.Count > 0)
//        {
//            DialogueLine currentLine = lines.Dequeue();
//            characterIcon.sprite = currentLine.character.icon;
//            characterName.text = currentLine.character.characterName;
//            // Adjust the position of the character icon based on the character's name
//            if (currentLine.character.characterName == "Player")
//            {
//                characterIcon.rectTransform.position = playerIconPosition.position;
//            }
//            else
//            {
//                characterIcon.rectTransform.position = npcIconPosition.position;
//            }
//            Debug.Log("change character Name");
//            yield return StartCoroutine(TypeSentence(currentLine));
//            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
//        }
//        EndDialogue();
//    }

//    private IEnumerator TypeSentence(DialogueLine currentLine)
//    {
//        dialogueArea.text = "";
//        foreach (char c in currentLine.dialogueline.ToCharArray())
//        {
//            dialogueArea.text += c;
//            yield return new WaitForSeconds(typingSpeed);
//        }
//    }

//    void EndDialogue()
//    {
//        DialogueCanvas.SetActive(false);
//        // Any additional actions after dialogue ends
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
        private Coroutine typingCoroutine;

        private void Awake()
        {
            Debug.Log("start");
            if (instance == null) instance = this;
            dialogueArea.text = "";
        }

        void Update()
        {
            if (lines.Count != 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (dialogueArea.text == lines.Peek().dialogueline)
                    {
                        lines.Dequeue();
                        DisplayNextDialogueLine();
                    }
                    //else
                    //{
                    //    if (typingCoroutine != null)
                    //    {
                    //        StopCoroutine(typingCoroutine);
                    //    }
                    //    dialogueArea.text = lines.Peek().dialogueline;
                    //}
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
            Debug.Log("change character Name");

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeSentence(currentLine));
        }

        public IEnumerator StartDialogue(Dialogue dialogue)
        {
            DialogueCanvas.SetActive(true);
            lines.Clear();
            foreach (DialogueLine dialogueline in dialogue.dialoguelines)
            {
                lines.Enqueue(dialogueline);
            }
            yield return DisplayNextDialogueLineCoroutine();
        }

        private IEnumerator DisplayNextDialogueLineCoroutine()
        {
            while (lines.Count > 0)
            {
                DialogueLine currentLine = lines.Dequeue();
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
                //Debug.Log("change character Name");
                yield return StartCoroutine(TypeSentence(currentLine));
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }
            EndDialogue();
        }

        private IEnumerator TypeSentence(DialogueLine currentLine)
        {
            dialogueArea.text = "";
            foreach (char c in currentLine.dialogueline.ToCharArray())
            {
                dialogueArea.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        void EndDialogue()
        {
            DialogueCanvas.SetActive(false);
            // Any additional actions after dialogue ends
        }
    }
}
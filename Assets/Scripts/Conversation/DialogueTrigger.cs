using Dialogue;
using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueConversations dialogueConversations;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (dialogueConversations != null)
            {
                Debug.Log("Tiggereddd");

                dialogueConversations.dialogue.onDialogueEnd += OnDialogueComplete;

                // Start the dialogue
                StartCoroutine(StartDialogue());
            }
        }
    }

    

    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1f);

        DialogueManager.instance.StartDialogue(dialogueConversations.dialogue);
        dialogueConversations.gameObject.SetActive(true);
    }

    private void OnDialogueComplete()
    {
        Debug.Log("Dialogue has ended. You can now perform other actions.");
    }
}

using Dialogue;
using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueConversations dialogueConversations;

    public void OnTriggerEnter(Collider other)
    {
        // Assuming you have already assigned the DialogueConversations component via the inspector or programmatically
        if (dialogueConversations != null)
        {
            // Hook up the onDialogueEnd action to a custom method
            dialogueConversations.onDialogueEnd += OnDialogueComplete;

            // Start the dialogue
            StartCoroutine(StartDialogue());
        }
    }

    

    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1f); // Optional delay before starting the dialogue
        dialogueConversations.gameObject.SetActive(true);
    }

    private void OnDialogueComplete()
    {
        Debug.Log("Dialogue has ended. You can now perform other actions.");
    }
}

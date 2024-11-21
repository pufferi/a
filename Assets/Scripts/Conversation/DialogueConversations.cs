using System.Collections;
using UnityEngine;

namespace Dialogue
{
    public class DialogueConversations : MonoBehaviour
    {
        public Dialogue dialogue;
        public System.Action onDialogueEnd;


        private void OnDialogueEnd()
        {
            Debug.Log("Ending Dialogue");
            gameObject.SetActive(false);
            onDialogueEnd?.Invoke();
        }
    }
}
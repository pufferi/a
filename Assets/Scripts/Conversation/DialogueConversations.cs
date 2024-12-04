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
            Debug.Log("IIIIIHJDHHDWHDJSJHDSBHD");
            onDialogueEnd?.Invoke();
        }
    }
}
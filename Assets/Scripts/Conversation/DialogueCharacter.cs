using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueCharacter
    {
        public string characterName = "";
        public Sprite icon = null;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public DialogueCharacter character;
        [TextArea(0, 6)]
        public string dialogueline;

    }

    [System.Serializable]
    public class Dialogue
    {
        public List<DialogueLine> dialoguelines = new List<DialogueLine>();
        public System.Action onDialogueEnd;
        private void OnDialogueEnd()
        {
            onDialogueEnd?.Invoke();
        }
    }
}
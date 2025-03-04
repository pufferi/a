using System.Collections;
using TMPro;
using UnityEngine;

public class TempEnding : MonoBehaviour
{
    public TaskListController taskListController;
    public TextMeshProUGUI message;

    private void Update()
    {
        if (taskListController.GetCompletedTaskCount() == 4)
        {
            StartCoroutine(ShowEndText());
        }
    }
    private IEnumerator ShowEndText()
    {
        message.text= "Awesome! You've completed all the tasks. Thank you for experiencing my game.";
        yield return new WaitForSeconds(6);
        message.text = "You can press the ESC key and exit the game.";
        yield return new WaitForSeconds(4);
        message.text = string.Empty;
        this.enabled = false;
    }
}

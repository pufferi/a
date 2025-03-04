using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskListController : MonoBehaviour
{
    public GameObject taskPrefab; 
    public Transform taskListContainer; 
    private List<GameObject> tasks = new List<GameObject>(); 

    public IEnumerator co_AddTask(string taskDescription)
    {
        GameObject newTask = Instantiate(taskPrefab, taskListContainer);
        RectTransform rectTransform = newTask.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        newTask.GetComponent<TMP_Text>().text = taskDescription;
        tasks.Add(newTask);
        yield return null;
    }

    public void AddTask(string taskDescription)
    {
        GameObject newTask = Instantiate(taskPrefab, taskListContainer);
        RectTransform rectTransform = newTask.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(140, 30); 
        newTask.GetComponent<TMP_Text>().text = taskDescription;
        tasks.Add(newTask);
    }

    public IEnumerator co_completeTask(int taskIndex)
    {
        CompleteTask(taskIndex);
        yield return null;
    }
    public void CompleteTask(int taskIndex)
    {
        if (taskIndex >= 0 && taskIndex < tasks.Count)
        {
            TMP_Text taskText = tasks[taskIndex].GetComponent<TMP_Text>();
            taskText.fontStyle = FontStyles.Strikethrough; 
            taskText.color = Color.gray; 
        }
    }
    public void CompleteTask(string taskDescription)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            TMP_Text taskText = tasks[i].GetComponent<TMP_Text>();
            if (taskText.text == taskDescription)
            {
                taskText.fontStyle = FontStyles.Strikethrough;
                taskText.color = Color.gray;
                break;
            }
        }
    }


    public void RemoveTask(int taskIndex)
    {
        if (taskIndex >= 0 && taskIndex < tasks.Count)
        {
            Destroy(tasks[taskIndex]);
            tasks.RemoveAt(taskIndex);
        }
    }
    public void ClearAllTask()
    {
        foreach (GameObject task in tasks)
        {
            Destroy(task); 
        }
        tasks.Clear(); 
    }

    public int GetCompletedTaskCount()
    {
        int completedTaskCount = 0;
        foreach (GameObject task in tasks)
        {
            TMP_Text taskText = task.GetComponent<TMP_Text>();
            if (taskText.fontStyle == FontStyles.Strikethrough && taskText.color == Color.gray)
            {
                completedTaskCount++;
            }
        }
        return completedTaskCount;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviorController : MonoBehaviour
{
    public void Move(Transform npcTransform,Vector3 target, float speed)
    {
        StartCoroutine(MoveTowards(npcTransform,target, speed));
    }
    private IEnumerator MoveTowards(Transform npcTransform,Vector3 target, float speed)
    {
        while (Vector3.Distance(npcTransform.position, target) > 0.01f)
        {
            npcTransform.position = Vector3.MoveTowards(npcTransform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        npcTransform.position = target;
    }
}

using System.Collections;
using UnityEngine;

public class CatMove : MonoBehaviour
{
    public BoxCollider moveRistriction;
    private float restrictionX0, restrictionY0, restrictionX1, restrictionY1;

    private bool isCatMoving = false;
    public float movingSpeed = 0.3f;
    //public Animator catAnimator;
    void Start()
    {
        restrictionX0 =moveRistriction.center.x- moveRistriction.size.x/2f;
        restrictionY0= moveRistriction.size.y-moveRistriction.size.y/2f;
        restrictionX1 = moveRistriction.center.x + moveRistriction.size.x / 2f;
        restrictionY1 = moveRistriction.size.y + moveRistriction.size.y / 2f;
        StartCatMove();
    }

    private void Update()
    {
        if (!isCatMoving) { StartCatMove(); }

    }


    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(
            Random.Range(restrictionX0, restrictionX1),
            transform.position.y, //up
            Random.Range(restrictionY0, restrictionY1)
        );
    }
        private void StartCatMove()
    {
        StartCoroutine(co_CatMove());
    }

    IEnumerator co_CatMove()
    {
        isCatMoving = true;
        Vector3 targetPos = GenerateRandomPosition();
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            Debug.Log("Targ   " + targetPos);
            Debug.Log("tf      "+transform.position);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movingSpeed*Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 5));
        isCatMoving=false;
    }
}

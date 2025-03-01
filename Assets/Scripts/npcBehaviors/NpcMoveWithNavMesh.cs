using UnityEngine;
using UnityEngine.AI;

public class NpcMoveWithNavMesh : MonoBehaviour
{
    //the scrpit is for controlling npc's radom move

    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float speed = 3.5f;

    private NavMeshAgent agent;
    private float timer;

    // fix the problem when my model's facing the wrong way, or goes up side down
    public float XRotationOffset = -90f;
    public float YRotationOffset = 0f;
    public float ZRotationOffset = 90f;

    public float YPositionOffset = 0f;

    public float rotationSpeed = 50f;

    private bool shouldNpcMoveWithPlayer=false;

    public Transform playerTransform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        timer = wanderTimer;
    }


    void Update()
    {
        if (shouldNpcMoveWithPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > 3.0f) // Adjust the stopping distance as needed
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
            else
                agent.isStopped = true;
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                willNpcSpin = WillTheNpcSpin();
                Debug.Log(agent.velocity.sqrMagnitude);
                timer = 0;
            }
        }

        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity);
            if (!willNpcSpin)
            {
                transform.rotation = Quaternion.Euler(XRotationOffset, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z + ZRotationOffset);
            }
            else
            {
                fishSpinAngle += fishSpinSpeed;
                transform.rotation = Quaternion.Euler(XRotationOffset, targetRotation.eulerAngles.y + fishSpinAngle, targetRotation.eulerAngles.z + ZRotationOffset);
            }
        }
        else
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(XRotationOffset, currentRotation.y, currentRotation.z);
        }

        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + YPositionOffset, currentPosition.z);
    }

   

    public void NpcStartMoveWithPlayer()
    {
        shouldNpcMoveWithPlayer = true;
    }

    public void NpcStopMoveWithPlayer()
    {
        shouldNpcMoveWithPlayer = false;
    }

    public void NpcStopMove()
    {
        agent.isStopped = true;
    }


    public void NpcStartMove()
    {
        agent.isStopped = false;
    }




    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        agent.speed = speed;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

        return navHit.position;
    }


    //for spinning fish
    bool willNpcSpin = false;
    float fishSpinAngle = 0;
    float fishSpinSpeed = 10;
    private bool WillTheNpcSpin()
    {
        if (this.tag != "Fish")
            return false;

        int randomValue = Random.Range(0, 20);
        if (randomValue == 1)
        {
            fishSpinAngle = 0;
            return true;
        }
        return false;
    }
    //yes u have a 1 in 20 chance see a spinning fish when the fish move
    //本来是一条bug让鱼转了起来，但椰壳说这是转转收购了咸鱼，我觉得好笑爆了，所以就保留了这个功能

}

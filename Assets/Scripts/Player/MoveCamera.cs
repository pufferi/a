using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraTransform;
    void Update()
    {
        transform.position = cameraTransform.position;
    }
}

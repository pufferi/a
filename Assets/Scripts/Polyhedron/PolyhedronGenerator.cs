using UnityEngine;

public class PolyhedronGenerator : MonoBehaviour
{
    public PolyhedronFactory polyhedronFactory;


    void Start()
    {
        if (gameObject.GetComponent<Rigidbody>()!=null)
            return;
        if (polyhedronFactory != null)
        {
            Polyhedron randomPolyhedron = polyhedronFactory.GenerateRandomPolyhedron();
            if (randomPolyhedron == null)
            {
                Debug.LogError("Generated Polyhedron is null");
                return;
            }

            Mesh mesh = randomPolyhedron.initialing();
            if (mesh == null)
            {
                Debug.LogError("Initialized Mesh is null");
                return;
            }

            Transform transform = GetComponent<Transform>();

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();

            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            rb.isKinematic = false;

            transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        }
        else
        {
            Debug.LogError("PolyhedronFactory Œ¥…Ë÷√");
        }
    }

}

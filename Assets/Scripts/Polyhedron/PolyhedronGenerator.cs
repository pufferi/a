using UnityEngine;

public class PolyhedronGenerator : MonoBehaviour
{
    public PolyhedronFactory polyhedronFactory;

    void Start()
    {
        if (polyhedronFactory != null)
        {
            Polyhedron randomPolyhedron = polyhedronFactory.GenerateRandomPolyhedron();
            // Debug.Log("顶点数 " + randomPolyhedron.vertices.Length);
            Mesh mesh=randomPolyhedron.initialing();

            Transform transform = GetComponent<Transform>();
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 设置连续碰撞检测模式

            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            rb.isKinematic = false;

            // 确保物体不与地面重叠
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        }
        else
        {
            Debug.LogError("PolyhedronFactory 未设置");
        }
    }
}

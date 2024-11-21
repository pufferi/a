using UnityEngine;

public class PolyhedronGenerator : MonoBehaviour
{
    public PolyhedronFactory polyhedronFactory;

    void Start()
    {
        if (polyhedronFactory != null)
        {
            Polyhedron randomPolyhedron = polyhedronFactory.GenerateRandomPolyhedron();
            //Debug.Log("∂•µ„ ˝   " + randomPolyhedron.vertices.Length);
            randomPolyhedron.initialing();
            Transform transform=GetComponent<Transform>();
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        else
        {
            Debug.LogError("PolyhedronFactory Œ¥…Ë÷√");
        }
    }
}

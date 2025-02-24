using UnityEngine;
using UnityEngine.Pool;

public class GrabableObjectGenerator : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 50;
    public int maxPoolSize = 500;

    private ObjectPool<GameObject> objectPool;

    void Start()
    {
        objectPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: poolSize,
            maxSize: maxPoolSize
        );
    }

    public GameObject GetObject()
    {
        return objectPool.Get();
    }

    public GameObject GetObject(Vector3 position, Vector3 scaleSize, Material material)
    {
        GameObject obj = objectPool.Get();
        obj.transform.position = position;
        obj.transform.localScale = scaleSize;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        return obj;
    }
    
    public void ReturnObject(GameObject obj)
    {
        objectPool.Release(obj);
    }
}


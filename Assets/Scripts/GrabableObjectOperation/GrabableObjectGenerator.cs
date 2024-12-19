using UnityEngine;
using UnityEngine.Pool;

public class GrabableObjectGenerator : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 10;

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
            maxSize: poolSize
        );
    }

    public GameObject GetObject()
    {
        return objectPool.Get();
    }

    public void ReturnObject(GameObject obj)
    {
        objectPool.Release(obj);
    }
}

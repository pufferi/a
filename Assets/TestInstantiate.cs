using UnityEngine;
using UnityEditor;

public class TestInstantiate : MonoBehaviour
{
    public GameObject a;

    //public Material material;
    int i = 0;

    void Start()
    {
    }

    //private void Update()
    //{
    //    if(a!=null&&i==0)
    //    {
    //        string prefabPath = "Assets/Prefabs/" + a.name + ".prefab";
    //        //a.GetComponent<Renderer>().material = material;

    //        PrefabUtility.SaveAsPrefabAsset(a, prefabPath);
    //        //a.GetComponent<MeshFilter>().mesh = a.GetComponent<GrabableObjectComponent>().Amesh;
    //        //a.GetComponent<MeshRenderer>().material = a.GetComponent<GrabableObjectComponent>().Amat;

    //        Debug.Log("Prefab saved at path: " + prefabPath);
    //        i++;
    //    }
    //}
}

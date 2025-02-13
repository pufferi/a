using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GrabableObejectGroupingManager : MonoBehaviour
{
    private static GrabableObejectGroupingManager _instance;
    public static GrabableObejectGroupingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GrabableObejectGroupingManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GrabableObejectGroupingStateChecker");
                    _instance = singletonObject.AddComponent<GrabableObejectGroupingManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        pendingObjectID = 0;
    }


    //------------------------------------------------------------------
    public struct objConnectionInfo
    {
        public GrabableObjectComponent obj;
        public List<GrabableObjectComponent> connectObjs;

        public objConnectionInfo(GrabableObjectComponent obj)
        {
            this.obj=obj;
            connectObjs = new List<GrabableObjectComponent>();
        }
    }

    LinkedList<objConnectionInfo> graph = new LinkedList<objConnectionInfo>();
    Dictionary<int ,objConnectionInfo> objId_objInfo = new Dictionary<int ,objConnectionInfo>();

    private Queue<int>reusedObjID=new Queue<int>();
    private int pendingObjectID;

    public void AssignObjectID(GrabableObjectComponent Gobj)
    {
        int id;

        if (reusedObjID.Count > 0)
            id = reusedObjID.Dequeue();
        else
            id = pendingObjectID++;
        //objectDic.Add(id, Gobj);
        objConnectionInfo info = new objConnectionInfo(Gobj);
        graph.AddLast(info);
        objId_objInfo.Add(id, info);
        Gobj.objID=id;
    }


    public void UnassignObjectID(GrabableObjectComponent Gobj)
    {
        objConnectionInfo info = objId_objInfo[Gobj.objID];
        UnassignGroupID(Gobj);
        graph.Remove(info);
        reusedObjID.Enqueue(Gobj.objID);
        Gobj.objID = -1;
    }
    
    public void AssignGroupID(GrabableObjectComponent a, GrabableObjectComponent b)
    {
        objId_objInfo[a.objID].connectObjs.Add(b);
        objId_objInfo[b.objID].connectObjs.Add(a);
    }   
    
    public void UnassignGroupID(GrabableObjectComponent Gobj)
    {
        int id=Gobj.objID;
        objConnectionInfo ObjInfo = objId_objInfo[id];
        foreach (var connetObj in ObjInfo.connectObjs)
        {
            objId_objInfo[connetObj.objID].connectObjs.Remove(Gobj);
        }
        ObjInfo.connectObjs.Clear();
    }

    public List<GrabableObjectComponent> GetNeighborObjects(GrabableObjectComponent Gobj)
    {
       return objId_objInfo[Gobj.objID].connectObjs;
    }

    public List<GrabableObjectComponent> GetAllConnectObjects(GrabableObjectComponent Gobj)
    {
        List<GrabableObjectComponent> result = new List<GrabableObjectComponent>();
        HashSet<GrabableObjectComponent> visited = new HashSet<GrabableObjectComponent>();

        void dfs(GrabableObjectComponent current)
        {
            visited.Add(current);
            result.Add(current);
            foreach (var neighbor in objId_objInfo[current.objID].connectObjs)
            {
                if (!visited.Contains(neighbor))
                {
                    dfs(neighbor);
                }
            }
        }

        dfs(Gobj);
        return result;
    }



    public bool IsConnect(GrabableObjectComponent a, GrabableObjectComponent b)
    {
        HashSet<GrabableObjectComponent> visited = new HashSet<GrabableObjectComponent>();

        bool dfs(GrabableObjectComponent current)
        {
            if (current == b)
                return true;

            visited.Add(current);

            if (!objId_objInfo.ContainsKey(current.objID))
            {
                return false; 
            }

            foreach (var neighbor in objId_objInfo[current.objID].connectObjs)
            {
                if (!visited.Contains(neighbor))
                {
                    if (dfs(neighbor))
                        return true;
                }
            }
            return false;
        }
        return dfs(a);
    }

}

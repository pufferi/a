using System.Collections.Generic;
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
        pendingGroupID= 0;  
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
    private Queue<int>reusedGroupID=new Queue<int>();
    private int pendingObjectID;
    private int pendingGroupID;


    public void AssignObjectID(GrabableObjectComponent Gobj)
    {
        int id;

        if (reusedObjID.Count > 0)
            id = reusedObjID.Dequeue();
        else
            id = pendingObjectID++;
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
        int groupId_a=a.groupID;
        int groupId_b=b.groupID;
        if (groupId_a == -1 && groupId_b == -1)//1粘1
        {
            a.groupID = GetGroupNum();
            b.groupID = a.groupID;
        }
        else if (a.groupID == -1) //1粘多
        {
            a.groupID=b.groupID;
        }
        else if(b.groupID == -1)//多粘1
        {
            b.groupID=a.groupID;
        }
        else//多粘多
        {
            if(a.groupID >b.groupID)
            {
                a.groupID=b.groupID;
                List<GrabableObjectComponent>aConnects= GetAllConnectObjects(a);
                foreach (var aConnect in aConnects)
                    aConnect.groupID = b.groupID;
                reusedGroupID.Enqueue(groupId_a);
            }
            else
            {
                b.groupID = a.groupID;
                List<GrabableObjectComponent> bConnects = GetAllConnectObjects(b);
                foreach (var bConnect in bConnects)
                    bConnect.groupID = a.groupID;
                reusedGroupID.Enqueue(groupId_b);
            }
        }

    }   

   
    
    public void UnassignGroupID(GrabableObjectComponent Gobj)
    {
        int id=Gobj.objID;
        objConnectionInfo ObjInfo = objId_objInfo[id];
        reusedGroupID.Enqueue(Gobj.groupID);

        foreach (var connetObj in ObjInfo.connectObjs)
        {
            objId_objInfo[connetObj.objID].connectObjs.Remove(Gobj);
            if (objId_objInfo[connetObj.objID].connectObjs.Count == 0)
                connetObj.groupID = -1;
            else
            {
                List<GrabableObjectComponent> AllLinkedObjsToConnectObj = GetAllConnectObjects(connetObj);
                int pendingId = GetGroupNum();
                connetObj.groupID=pendingId;
                foreach(var obj in AllLinkedObjsToConnectObj)
                {
                    obj.groupID = pendingId;    
                }
            }
        }

        Gobj.groupID = -1;
        ObjInfo.connectObjs.Clear();
    }

    private int GetGroupNum()
    {
        if (reusedGroupID.Count == 0)
            return pendingGroupID++;
        return reusedGroupID.Dequeue();
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
        if(a.groupID==-1&&b.groupID==-1)
            return false;
        return a.groupID==b.groupID;
    }

}

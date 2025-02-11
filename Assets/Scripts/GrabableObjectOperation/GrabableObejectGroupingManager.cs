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
        pendingGroupID = 0;
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

    private Dictionary<int ,int>groupingStatement=new Dictionary<int, int>();//<groupID,objID>
    //private Dictionary<int , GrabableObjectComponent>objectDic= new Dictionary<int , GrabableObjectComponent>();
    private Dictionary<int , List<GrabableObjectComponent>>groups=new Dictionary<int , List<GrabableObjectComponent>>();//groupID,group
    private Queue<int>reusedGroupID=new Queue<int>();
    private Queue<int>reusedObjID=new Queue<int>();
    private int pendingObjectID;
    private int pendingGroupID;

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
        //objectDic.Remove(Gobj.objID);
        objConnectionInfo info = objId_objInfo[Gobj.objID];
        graph.Remove(info);
        reusedObjID.Enqueue(Gobj.objID);
        Gobj.objID = -1;
    }
    
    public void AssignGroupID(GrabableObjectComponent a, GrabableObjectComponent b)
    {
        if (a.groupID == -1 && b.groupID == -1)
        {
            int groupId;
            if (reusedGroupID.Count > 0)
                groupId = reusedGroupID.Dequeue();
            else
                groupId = pendingGroupID++;
            a.groupID = groupId;
            b.groupID = groupId;
            groups.Add(groupId, new List<GrabableObjectComponent> { a, b });
        }
        else if (a.groupID == b.groupID)
            return;
        else if (a.groupID == -1 && b.groupID != -1)
        {
            int groupId = b.groupID;
            a.groupID = groupId;
            groups[groupId].Add(a);
        }
        else if (a.groupID != -1 && b.groupID == -1)
        {
            int groupId = a.groupID;
            b.groupID = groupId;
            groups[groupId].Add(b);
        }
        else
        {
            if(a.groupID < b.groupID)
            {
                int groupId = a.groupID;
                List<GrabableObjectComponent> groupB = groups[b.groupID];
                groups[a.groupID].AddRange(groupB);
                groups.Remove(b.groupID);
                foreach(GrabableObjectComponent component in groupB)
                    component.groupID = groupId;
            }
            else
            {
                int groupId=b.groupID;
                List<GrabableObjectComponent>groupA = groups[a.groupID];
                groups[b.groupID].AddRange(groupA);
                groups.Remove(a.groupID);
                foreach (GrabableObjectComponent component in groupA)
                    component.groupID = groupId;
            }
        }
    }   
    
    public void UnassignGroupID(GrabableObjectComponent Gobj)
    {
        List<GrabableObjectComponent> connectObjs = GetConnectObjects(Gobj);
        if (connectObjs.Count == 0)
            return;
        if(connectObjs.Count == 1)
        {
            reusedGroupID.Enqueue(Gobj.groupID);
            groups.Remove(Gobj.groupID);
            Gobj.groupID = -1;
            reusedGroupID.Enqueue(connectObjs[0].groupID);
            groups.Remove(connectObjs[0].groupID);
            connectObjs[0].groupID = -1;
        }
        else
        {
            groups[Gobj.groupID].Remove(Gobj);
            Gobj.groupID = -1;
        }
    }

    public List<GrabableObjectComponent> GetConnectObjects(GrabableObjectComponent Gobj)
    {
        List<GrabableObjectComponent>connectObjs=groups[Gobj.groupID];
        connectObjs.Remove(Gobj);
        return connectObjs;
    }

    public bool IsConnect(GrabableObjectComponent a, GrabableObjectComponent b)
    {
        if (a.groupID == -1 || b.groupID == -1) 
            return false;
        return a.groupID == b.groupID;
    }
    public bool IsConnect(GameObject a,GameObject b)
    {
        return IsConnect(a.GetComponent<GrabableObjectComponent>(),b.GetComponent<GrabableObjectComponent>());
    }
}

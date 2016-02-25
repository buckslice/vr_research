using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncTransformation : SyncScript
{
    public int networkID;
    public bool useLocalValues = false;
    public bool SyncPosition = true;
    public bool SyncRotation = true;
    public bool SyncScale = true;
    [Tooltip("minimum magnitude difference in the transform needed to send info over network")]
    public float epsilon = 0.01f;
    
    GameClient client;
    GameServer server;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Vector3 lastScale;

    private Vector3 curPos;
    private Quaternion curRot;
    private Vector3 curScale;

    int bufsize;
    
    // Use this for initialization
	void Start ()
    {
        bufsize = sizeof(int) + sizeof(float) * 3 + sizeof(float) * 4; //ID + vector3 + quaternion
        bufsize = Mathf.NextPowerOfTwo(bufsize) * 2; //for some reason just the next power isn't enough room.
        client = Object.FindObjectOfType<GameClient>();
        server = Object.FindObjectOfType<GameServer>();
        if (client)
            client.addID(networkID, this);
        if (server)
            server.addID(networkID, this);
        SetCurTransform();
        SetLastTransform();
	}
	
	// Update is called once per frame
	void Update ()
    {
        SetCurTransform();
        if (!receiving && ValuesHaveChanged())
        {
 //           Debug.Log("Sending data from " + name);
            Send();
            SetLastTransform();
        }
	}

    bool VectorsEqual(Vector3 v1, Vector3 v2)
    {
        float mag = (v1 - v2).sqrMagnitude;
        return mag < epsilon;
    }

    bool QuaternionsEqual(Quaternion q1, Quaternion q2)
    {
        return Mathf.Abs(Quaternion.Dot(q1,q2)) > 1 - epsilon;
    }

    bool ValuesHaveChanged()
    {
        bool vecsEqual = VectorsEqual(curPos, lastPos);
        bool rotsEqual = QuaternionsEqual(curRot, lastRot);
        bool scalesEqual = VectorsEqual(curScale, lastScale);
        return !(vecsEqual && rotsEqual && scalesEqual);
    }

    void SetCurTransform()
    {
        curPos = useLocalValues ? transform.localPosition : transform.position;
        curRot = useLocalValues ? transform.localRotation : transform.rotation;
        curScale = useLocalValues ? transform.localScale : transform.lossyScale;
    }

    void SetLastTransform()
    {
        lastPos = curPos;
        lastRot = curRot;
        lastScale = curScale;
    }

    override public void Send()
    {
        Packet p = new Packet(bufsize);
        p.Write(networkID);
        p.Write(curPos);
        p.Write(curRot);
        p.Write(curScale);
        if (client)
            client.SendPacket(p, QosType.Unreliable);
        if (server)
            server.SendPacket(p, QosType.Unreliable);
    }

    override public void Receive(Packet p)
    {
        if (!receiving)
            return;
        //Debug.Log(transform.name + " Receiving");
        Vector3 pos = p.ReadVector3();
        Quaternion rot = p.ReadQuaternion();
        Vector3 scl = p.ReadVector3();
        if(useLocalValues)
        {
            if (SyncPosition)
                transform.localPosition = pos;
            if (SyncRotation)
                transform.localRotation = rot;
            if (SyncScale)
                transform.localScale = scl;
        }
        else
        {
            if (SyncPosition)
                transform.position = pos;
            if (SyncRotation)
                transform.rotation = rot;
            if (SyncScale)
                transform.localScale = scl;
            //need to figure out a non-computationally expensive way to set scale globally
        }
    }
}

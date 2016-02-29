using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

abstract public class SyncScript : MonoBehaviour 
{
    public bool receiving;
    abstract public void Send();
    abstract public void Receive(Packet p);
    abstract public void Receive(Vector3 pos, Quaternion rot, Vector3 scl);
}

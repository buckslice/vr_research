using UnityEngine;
using System.Collections;

public class GrabPlayer : MonoBehaviour {
    GameClient client;
    void Start()
    {
        client = Transform.FindObjectOfType<GameClient>();
    }
    
    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Player" && other.transform.parent != this.transform)
        {
            if (client)
                client.sendMessage("Parent: " + GetComponent<SyncTransformation>().networkID + " Child: " + other.transform.GetComponent<SyncTransformation>().networkID);
            other.transform.parent = transform;
            ArtificialGravity gravity = other.gameObject.GetComponent<ArtificialGravity>();
            gravity.SwitchDownObject(this.transform);
        }
    }
}

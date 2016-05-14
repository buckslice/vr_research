using UnityEngine;
using System.Collections;

public class GrabPlayer : MonoBehaviour {
    
    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Player")
        {
            other.transform.parent = transform;
        }
    }
}

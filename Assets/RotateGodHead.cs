using UnityEngine;
using System.Collections;

public class RotateGodHead : MonoBehaviour
{
    Transform thehead;
	void Start ()
    {
        thehead = GameObject.Find("InvisibleGodEyes").transform;
	}
	
	void LateUpdate ()
    {
        transform.position = thehead.position;
        transform.rotation = thehead.rotation;
	}
}

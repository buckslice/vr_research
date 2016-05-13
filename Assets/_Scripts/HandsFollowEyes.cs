using UnityEngine;
using System.Collections;

public class HandsFollowEyes : MonoBehaviour {
    Transform godEyes;
    float distance;
	// Use this for initialization
	void Start () {
        godEyes = GameObject.Find("GodEyes").transform;
        distance = Vector3.Distance(godEyes.position,transform.position);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = godEyes.position + godEyes.forward * distance;
	}
}

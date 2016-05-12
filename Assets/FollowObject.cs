using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
    public Transform obj;
	void Update () {
        transform.position = obj.position;
        transform.rotation = obj.rotation;
	}
}

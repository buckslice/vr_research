using UnityEngine;
using System.Collections;

public class ArtificialGravity : MonoBehaviour {
    float gravity = 9.8f;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 gravDir = -transform.up.normalized;
        rb.velocity += gravity * gravDir * Time.deltaTime;
	}
}

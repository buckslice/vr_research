using UnityEngine;
using System.Collections;

public class ArtificialGravity : MonoBehaviour
{
    public Transform downObject;
    float gravity;
    Rigidbody rb;
    Vector3 gravDir;
	// Use this for initialization
	void Start () {
        gravity = Physics.gravity.y;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (downObject)
            gravDir = downObject.up.normalized;
        else
            gravDir = transform.up.normalized;
        rb.velocity += gravity * gravDir * Time.deltaTime;
	}
}

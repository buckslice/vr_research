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

    public void SwitchDownObject(Transform newDownObject)
    {
        Debug.Log("Switching down object to " + newDownObject.name);
        transform.rotation = newDownObject.rotation;
        Vector3 relativePos = newDownObject.InverseTransformPoint(transform.position);
        if(relativePos.y < 0)
        {
            Debug.Log("I'm below the object. Adjusting");
            relativePos.y = 2f;
            transform.position = newDownObject.TransformPoint(Vector3.up*2f); //go to 2 units above center of object.
        }
        downObject = newDownObject;
    }
}

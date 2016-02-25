using UnityEngine;
using System.Collections;

public class WASDMove : MonoBehaviour {
    public float speed = 1f;
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.forward * Time.deltaTime * speed;
        else if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * Time.deltaTime * speed;
        else if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.back * Time.deltaTime * speed;
        else if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * Time.deltaTime * speed;
	}
}

using UnityEngine;
using System.Collections;

public class WASDMove : MonoBehaviour {
    public float speed = 3f;
    public float turnSpeed = 50f;
    SyncTransformation syncT;
    bool canMove = true;
    void Start()
    {
        syncT = GetComponent<SyncTransformation>();
        if (syncT && syncT.receiving)
            canMove = false;
    }
	// Update is called once per frame
	void Update () {
        if (!canMove)
            return;
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.up * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.down * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.down * Time.deltaTime * turnSpeed);
	}
}

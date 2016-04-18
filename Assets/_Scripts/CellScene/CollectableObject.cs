using UnityEngine;
using System.Collections;

public class CollectableObject : MonoBehaviour {

    private Rigidbody rb;
    private Vector3 centerOfCell = Vector3.zero;
    private float cellRadius = 25.0f;
    private float speed;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        speed = Random.value * 2.0f + 2.0f;
        rb.velocity = Random.onUnitSphere * speed;
    }

    void FixedUpdate() {
        if ((transform.position - centerOfCell).sqrMagnitude > cellRadius* cellRadius) {
            rb.velocity = Vector3.Reflect(rb.velocity, (centerOfCell - transform.position).normalized).normalized * speed;
        }
    }
}

using UnityEngine;
using System.Collections;

public class RotatePlayerHead : MonoBehaviour
{
    Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
    }
	// Update is called once per frame
	void Update ()
    {
        transform.localRotation = cam.localRotation;
	}
}

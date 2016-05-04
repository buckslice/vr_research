using UnityEngine;
using System.Collections;

public class MoleculeBehavior : MonoBehaviour
{
	public Vector3 destination = Vector3.zero;
	public float speed = 1f;
	private bool destinationReached = false;
	
	// Update is called once per frame
	void Update () {
		if(!destinationReached && Vector3.Distance (transform.localPosition, destination) < .1f * speed)
			destinationReached = true;
		if(!destinationReached)
			transform.localPosition -= (transform.localPosition - destination).normalized * speed * Time.deltaTime;
		if(destinationReached)
			Destroy(this.gameObject);
	}
}

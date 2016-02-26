using UnityEngine;
using System.Collections;

public class StartChoice : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S)) {
            gameObject.AddComponent<GameServer>();
            Destroy(this);
        } else if (Input.GetKeyDown(KeyCode.C)) {
            gameObject.AddComponent<GameClient>();
            Destroy(this);
        }
	}
}

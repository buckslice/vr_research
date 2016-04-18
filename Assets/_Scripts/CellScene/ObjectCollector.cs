using UnityEngine;
using System.Collections;

public class ObjectCollector : MonoBehaviour {

    public int collected = 0;

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Collectable") {
            Destroy(col.gameObject);

            collected++;

        }
    }
}

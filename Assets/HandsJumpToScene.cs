using UnityEngine;
using System.Collections;

public class HandsJumpToScene : MonoBehaviour {
    public GameObject theScene;
    private Renderer sceneRend;
    private Vector3 sceneBounds;
    private Vector3 origPos;
	// Use this for initialization
	void Start () {
        origPos = transform.localPosition;
        sceneBounds = GetBounds(theScene);
        sceneRend = theScene.GetComponent<Renderer>();
	}
	
    Vector3 GetBounds(GameObject obj)
    {
        Renderer[] bs = GetComponentsInChildren<Renderer>(obj);
        Bounds b = bs[0].bounds;
        int len = bs.Length;
        for (int i = 1; i < len; ++i)
        {
            b.Encapsulate(bs[i].bounds);
        }
        return b.size;
    }

	// Update is called once per frame
	void Update () {
        if (sceneRend.enabled)
        {
            transform.position = theScene.transform.position + origPos;
        }
        else
            transform.localPosition = origPos;
	}
}

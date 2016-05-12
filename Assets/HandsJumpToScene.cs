using UnityEngine;
using System.Collections;

public class HandsJumpToScene : MonoBehaviour {
    public GameObject theScene;
    public bool isLeft;
    private Renderer sceneRend;
    private Vector3 sceneBounds;
    private Vector3 origPos;
    private Vector3 newPos;
    private Vector3 origScale;
    private Vector3 newScale;
    private Vector3 leftOffset = Vector3.zero;
	// Use this for initialization
	void Start () {
        origPos = transform.localPosition;
        origScale = transform.localScale;
        sceneBounds = GetBounds(theScene);
        newScale = origScale * sceneBounds.magnitude * 2;
        newPos = origPos * sceneBounds.magnitude * 2;
        newPos.z += sceneBounds.magnitude*5;
        sceneRend = theScene.GetComponent<Renderer>();
        if (isLeft)
            leftOffset = Vector3.left * origPos.x;
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
            transform.localScale = newScale;
            transform.position = theScene.transform.position - newPos;
        }
        else
        {
            transform.localScale = origScale;
            transform.localPosition = origPos;
        }
	}
}

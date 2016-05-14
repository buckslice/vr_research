using UnityEngine;
using System.Collections;

public class CopyAllChildTransforms : MonoBehaviour
{
    public Transform parentToCopy;
    public bool hideTheirMeshes = true;
    public bool disableTheirColliders = true;
    public bool killTheirRigidbodies = true;

    Transform[] theirChildTransforms;
    Transform[] myChildTransforms;
    int len;
    // Use this for initialization
    void Start () {
        theirChildTransforms = parentToCopy.GetComponentsInChildren<Transform>();
        myChildTransforms = transform.GetComponentsInChildren<Transform>();
        len = theirChildTransforms.Length;
        if (hideTheirMeshes)
            HideMeshes();
        if (disableTheirColliders)
            DisableColliders();
        if (killTheirRigidbodies)
            KillRigidbodies();
    }
	
	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < len; ++i)
        {
            myChildTransforms[i].localScale = theirChildTransforms[i].localScale;
            myChildTransforms[i].position = theirChildTransforms[i].position;
            myChildTransforms[i].rotation = theirChildTransforms[i].rotation;
        }
	}

    void HideMeshes()
    {
        Renderer[] theirMeshes = parentToCopy.GetComponentsInChildren<Renderer>();
        int meshLen = theirMeshes.Length;
        for (int i = 0; i < meshLen; ++i)
            theirMeshes[i].enabled = false;
    }

    void DisableColliders()
    {
        Collider[] theirColliders = parentToCopy.GetComponentsInChildren<Collider>();
        int meshLen = theirColliders.Length;
        for (int i = 0; i < meshLen; ++i)
            theirColliders[i].enabled = false;
    }
    void KillRigidbodies()
    {
        Rigidbody[] theirRBs = parentToCopy.GetComponentsInChildren<Rigidbody>();
        int meshLen = theirRBs.Length;
        for (int i = 0; i < meshLen; ++i)
            Destroy(theirRBs[i]);
    }
}

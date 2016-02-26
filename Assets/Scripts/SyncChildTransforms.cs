using UnityEngine;
using System.Collections;

public class SyncChildTransforms : MonoBehaviour 
{
    //[Header("Network IDs")]
    [Tooltip("Script sets packet IDs starting at this value, and iterates forward for each child")]
    public int startID = 0;
    [Tooltip("Don't bother setting this. It's just so you know what the last ID is once it's set by the script")]
    public int endID;
    private int id;
    //[Header("SyncTransform variables")]
    public bool receiving;
    public bool useLocalValues = true;
    public bool SyncPosition = true;
    public bool SyncRotation = true;
    public bool SyncScale = true;

    //uses DepthFirstSearch to add SyncTransform script to each child
    [ContextMenu("Add SyncTransforms to all children")]
	public void AddSyncTransforms () 
    {
        id = startID;
        Transform[] children = GetComponentsInChildren<Transform>();
        Debug.Log("Length: " + children.Length);
        for (int i = 0; i < children.Length; ++ i)
        {
            SyncTransformation sync = children[i].gameObject.AddComponent<SyncTransformation>();
            sync.receiving = receiving;
            sync.useLocalValues = useLocalValues;
            sync.SyncPosition = SyncPosition;
            sync.SyncRotation = SyncRotation;
            sync.SyncScale = SyncScale;
            sync.networkID = id++;
        }
        endID = id;
	}

    [ContextMenu("Remove SyncTransforms from all children")]
    public void RemoveSyncTransforms()
    {
        SyncTransformation[] syncs = transform.GetComponentsInChildren<SyncTransformation>();
        Debug.Log("Sync Length: " + syncs.Length);
        foreach (SyncTransformation s in syncs)
            DestroyImmediate(s);
        endID = startID;
    }
}

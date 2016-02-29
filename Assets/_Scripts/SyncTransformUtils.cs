using UnityEngine;
using System.Collections;

public class SyncTransformUtils : MonoBehaviour 
{
    [Header("Network IDs")]
    [Tooltip("AddSyncTransforms sets packet IDs starting at this value, and iterates forward for each child")]
    public int startID = 0;
    [Tooltip("Don't bother setting this. It's just so you know what the last ID is once it's set by the AddSyncTransfroms")]
    public int endID;
    private int id;
    [Header("SyncTransform variables")]
    public bool receiving;
    public bool useLocalValues = true;
    public bool SyncPosition = true;
    public bool SyncRotation = true;
    public bool SyncScale = true;
    public float epsilon = 0.01f;

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

    [ContextMenu("Log the total count of SyncTransforms in the scene")]
    public void CountSyncTransformsInScene()
    {
        SyncTransformation[] syncs = Object.FindObjectsOfType<SyncTransformation>();
        int lowestID = int.MaxValue;
        int highestID = 0;
        foreach(SyncTransformation s in syncs)
        {
            if (s.networkID < lowestID)
                lowestID = s.networkID;
            if (s.networkID > highestID)
                highestID = s.networkID;
        }

        Debug.Log("Number of syncs: " + syncs.Length);
        Debug.Log("Lowest ID: " + lowestID);
        Debug.Log("Highest ID: " + highestID);
    }

    [ContextMenu("Update the epsilon of each SyncTransform in the scene")]
    public void UpdateEpsilons()
    {
        SyncTransformation[] syncs = Object.FindObjectsOfType<SyncTransformation>();
        foreach (SyncTransformation s in syncs)
            s.epsilon = epsilon;
    }
}

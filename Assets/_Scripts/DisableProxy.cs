using UnityEngine;
using System.Collections;

public class DisableProxy : MonoBehaviour {
    public GameObject realHandLeft;
    public GameObject realHandRight;
    public GameObject proxyHandLeft;
    public GameObject proxyHandRight;

    bool valuesSet;
	// Use this for initialization
	void Start () {
        valuesSet = ValsAreSet();
        if (!valuesSet)
            Debug.Log("DisableProxy: Set real and proxy hands so this script works");
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (valuesSet)
        {
            proxyHandLeft.SetActive(realHandLeft.activeSelf);
            proxyHandRight.SetActive(realHandRight.activeSelf);
        }
        else
            valuesSet = ValsAreSet();
	}

    bool ValsAreSet()
    {
        return !(!realHandLeft || !realHandRight || !proxyHandLeft || !proxyHandRight);
    }
}

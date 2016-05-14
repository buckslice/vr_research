using UnityEngine;
using System.Collections;

public class DisableProxy : MonoBehaviour {
    public GameObject realHandLeft;
    public GameObject realHandRight;
    public GameObject proxyHandLeft;
    public GameObject proxyHandRight;
    public Vector3 oldPosLeft;
    public Vector3 oldPosRight;
    private Vector3 newPosLeft;
    private Vector3 newPosRight;
    bool amOld = false;

    bool valuesSet;
	// Use this for initialization
	void Start () {
        newPosLeft = proxyHandLeft.transform.localPosition;
        newPosRight = proxyHandRight.transform.localPosition;

        valuesSet = ValsAreSet();
        if (!valuesSet)
            Debug.Log("DisableProxy: Set real and proxy hands so this script works");
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (Input.GetMouseButtonDown(0))
            ToggleHandPlacement();
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

    public void ToggleHandPlacement()
    {
        proxyHandLeft.transform.localPosition = amOld ? newPosLeft : oldPosLeft;
        proxyHandRight.transform.localPosition = amOld ? newPosRight : oldPosRight;
        amOld = !amOld;
    }
}

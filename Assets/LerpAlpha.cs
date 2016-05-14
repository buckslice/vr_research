using UnityEngine;
using System.Collections;

public class LerpAlpha : MonoBehaviour
{
    public Transform head;
    public float MaxDistance = 160f;
    public float MinDistance = 60f;
    private float totalDistance;
    private Renderer rend;
    private Color maxColor;
    private Color minColor;
	void Start ()
    {
        totalDistance = MaxDistance + MinDistance;
        rend = GetComponent<Renderer>();
        maxColor = rend.material.color;
        minColor = maxColor;
        minColor.a = 0f;

    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            Debug.Log(Vector3.Distance(transform.position, head.position));
        if (rend.enabled)
            rend.material.color = Color.Lerp(minColor, maxColor, (Vector3.Distance(transform.position, head.position) - MinDistance) / (MaxDistance - MinDistance));
        
	}
}

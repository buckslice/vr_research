using UnityEngine;
using System.Collections;

public class CellWallUpdate : MonoBehaviour {
    public Color color = Color.white;
    public float cellRadius = 25.0f;
    public float rippleScale = 1.0f;
    public bool fadeWhenClose = true;

    private MeshRenderer mr;

    // Use this for initialization
    void Start() {
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update() {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_RippleScale", rippleScale);

        if (fadeWhenClose) {
            float d = Vector3.Magnitude(Camera.main.transform.position - transform.position);
            float min = cellRadius * 2.0f;
            float max = min + 20.0f;
            float t = (d - min) / (min - max);
            float a = Mathf.Lerp(0.1f, 0.5f, 1.0f - t);
            //Color c = mr.material.GetColor("_Color");
            color.a = a;
        }
        mpb.SetColor("_Color", color);

        mr.SetPropertyBlock(mpb);
    }
}

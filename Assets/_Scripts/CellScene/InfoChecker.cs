using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfoChecker : MonoBehaviour {
    public float rayCastDistance = 20.0f;

    private RectTransform panel;
    private Text infoText;
    private Text nameText;

    private Transform cam;
    private ParticleSystem ps;
    //private float gazeTimer;
    private IEnumerator currentShow = null;

    // Use this for initialization
    void Start() {
        GameObject go = GameObject.Find("Canvas");
        if (go) {
            cam = Camera.main.transform;
            ps = cam.Find("Particles").GetComponent<ParticleSystem>();

            panel = go.transform.Find("Panel").GetComponent<RectTransform>();
            panel.gameObject.SetActive(false);

            infoText = panel.Find("Info").GetComponent<Text>();
            nameText = panel.Find("Name").GetComponent<Text>();
        } else {
            Debug.LogWarning("Can't find CANVAS for LookInfo script!!! Deleting self...");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update() {

        //Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //if (input.sqrMagnitude < 0.01f) {
        //    gazeTimer += Time.deltaTime;
        //    if (gazeTimer > 0.5f) {
        //        checkForInfo();
        //        gazeTimer = 0.0f;
        //    }
        //} else {
        //    gazeTimer = 0.0f;
        //}

        RaycastHit info;
        if (Physics.Raycast(cam.position, cam.forward, out info, rayCastDistance)) {
            if (Input.GetMouseButtonDown(0)) {
                ps.transform.position = info.point - cam.forward * 0.1f;
                ps.transform.rotation = Quaternion.LookRotation(info.normal);
                LookInfo lookInfo = info.collider.GetComponent<LookInfo>();
                if (lookInfo) {
                    if (currentShow != null) {
                        StopCoroutine(currentShow);
                    }
                    currentShow = showText(lookInfo);
                    StartCoroutine(currentShow);
                    Debug.Log("clicked");
                }
            }
        } else {
            ps.transform.position = cam.position + cam.forward * rayCastDistance;
            ps.transform.rotation = Quaternion.Euler(cam.forward);
        }

        ParticleSystem.EmissionModule em = ps.emission;
        em.enabled = Input.GetMouseButton(0);

    }


    IEnumerator showText(LookInfo info) {
        panel.gameObject.SetActive(true);

        nameText.text = info.title;
        infoText.text = info.text;

        yield return new WaitForSeconds(5.0f);

        panel.gameObject.SetActive(false);
    }
}

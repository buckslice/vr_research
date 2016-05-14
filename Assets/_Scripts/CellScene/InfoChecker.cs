using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfoChecker : MonoBehaviour {
    public float rayCastDistance = 20.0f;

    public Object textPrefab;

    private GameObject textObject;
    private TextMesh textMesh;

    private Transform cam;
    //private ParticleSystem ps;
    //private float gazeTimer;
    private IEnumerator currentShow = null;

    // Use this for initialization
    void Start() {
        if (textPrefab) {
            textObject = (GameObject)Instantiate(textPrefab, Vector3.up * 1000.0f, Quaternion.identity);
            textMesh = textObject.transform.Find("Text").GetComponent<TextMesh>();
            textObject.SetActive(false);

            cam = Camera.main.transform;
        } else {
            Debug.LogWarning("InfoChecker.cs missing textObject prefab");
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
                //ps.transform.position = info.point - cam.forward * 0.1f;
                //ps.transform.rotation = Quaternion.LookRotation(info.normal);

                LookInfo lookInfo = info.collider.GetComponent<LookInfo>();
                if (lookInfo) {
                    textObject.transform.position = info.point - cam.forward * 0.1f;
                    textObject.transform.rotation = Quaternion.LookRotation(-info.normal);
                    if (currentShow != null) {
                        StopCoroutine(currentShow);
                    }
                    currentShow = showText(lookInfo);
                    StartCoroutine(currentShow);
                }
            }
        } else {
            //ps.transform.position = cam.position + cam.forward * rayCastDistance;
            //ps.transform.rotation = Quaternion.Euler(cam.forward);
        }

        //ParticleSystem.EmissionModule em = ps.emission;
        //em.enabled = Input.GetMouseButton(0);

    }


    IEnumerator showText(LookInfo info) {
        textObject.SetActive(true);

        textMesh.text = info.text;
        //nameText.text = info.title;
        //infoText.text = info.text;

        yield return new WaitForSeconds(5.0f);

        textObject.SetActive(false);
    }
}

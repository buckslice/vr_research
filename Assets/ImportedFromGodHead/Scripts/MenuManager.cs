using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{

    private Transform cam;

    private GameObject serverCube;
    private GameObject clientCube;
    private ParticleSystem ps;
    private ParticleSystem.EmissionModule serverps;
    private ParticleSystem.EmissionModule clientps;
    private IPKeyPad keyPad;
    private OVRScreenFade ovrFade;
    public string ipAddress;
    private bool fading = false;

    void Awake()
    {
        cam = Camera.main.transform;
        ovrFade = cam.GetComponent<OVRScreenFade>();
        serverCube = GameObject.Find("ServerCube");
        clientCube = GameObject.Find("ClientCube");

        //keyPad = UnityEngine.Object.FindObjectOfType<IPKeyPad>();
        serverps = serverCube.GetComponent<ParticleSystem>().emission;
        clientps = clientCube.GetComponent<ParticleSystem>().emission;

    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
        string lookedAt = NameOfLookedAt();
        if (lookedAt == serverCube.name) 
        {
            spinTransform(serverCube.transform);
            serverps.enabled = true;

            if (!fading && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                StartCoroutine(FadeOutThenLoad("VRScene"));
        } 
        else
            serverps.enabled = false;

        if (lookedAt == clientCube.name) 
        {
            spinTransform(clientCube.transform);
            clientps.enabled = true;

            if (!fading && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                StartCoroutine(FadeOutThenLoad("ARScene"));
        } 
        else
            clientps.enabled = false;
    }

    private IEnumerator FadeOutThenLoad(string levelname)
    {
        fading = true;
        yield return StartCoroutine(ovrFade.FadeOut());
        fading = false;
        SceneManager.LoadScene(levelname);
    }

    private string NameOfLookedAt()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.position, cam.forward, out hit))
            return "";
        return hit.collider.name;
    }

    private void spinTransform(Transform tform) 
    {
        float t = Time.deltaTime;
        tform.Rotate(new Vector3(t * 80.0f, t * 100.0f, t * 40.0f));
    }
}

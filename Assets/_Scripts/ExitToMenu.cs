using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour {
    public string menuName = "menu";
    private bool fading = false;
    private OVRScreenFade ovrFade;

	void Start () {
        ovrFade = Camera.main.GetComponent<OVRScreenFade>();
        if (!ovrFade)
            Debug.Log("Attach OVRScreenFade script to " + Camera.main.name);
	}
	
	void Update () {
        if (!fading && Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(FadeOutThenLoad(menuName));
	}


    private IEnumerator FadeOutThenLoad(string levelname)
    {
        fading = true;
        yield return StartCoroutine(ovrFade.FadeOut());
        fading = false;
        SceneManager.LoadScene(levelname);
    }
}

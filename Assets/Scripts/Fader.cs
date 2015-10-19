using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {

	public string level;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.N)) {
			StartCoroutine("ChangeLevel");
		}
	}

	IEnumerator ChangeLevel () {
		float fadeTime = GameObject.Find("GM").GetComponent<FadeBehavior>().BeginFade(1);
		yield return new WaitForSeconds(fadeTime);
		if (level != null) {
			Application.LoadLevel(level);
		} else {
			Application.LoadLevel(Application.loadedLevel + 1);
		}
	}
}

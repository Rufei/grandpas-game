using UnityEngine;
using System.Collections;

public class TalkScript : MonoBehaviour {

	// Use this for initialization
	private TextMesh label;
	private float alpha;
	public bool isFadingOut;
	private float fadeRate = 1.0f;

	public int scriptNumberPrev = 0;
	public int scriptNumber = 0;

	void Start () {
		
		label = gameObject.GetComponent<TextMesh>();
		label.text = newText (0);
		label.color = new Color (label.color.r, label.color.g, label.color.b, 0.0f);
		isFadingOut = false;
	}

	// Update is called once per frame
	void Update () {

		/*if (Input.GetMouseButtonDown (0)) {
			scriptNumber++;
		}*/

		fadeOut ();
		updateText ();
	}

	public void incrementText(){
		scriptNumber++;
	}

	void fadeOut(){
		if (isFadingOut) {
			label.color = new Color (label.color.r, label.color.g, label.color.b, label.color.a - Time.deltaTime * fadeRate);
		} else {
			label.color = new Color (label.color.r, label.color.g, label.color.b, label.color.a + Time.deltaTime * fadeRate);
		}

		float tempCap = label.color.a;
		tempCap = Mathf.Min (Mathf.Max (tempCap, 0.0f), 1.0f);

		label.color = new Color (label.color.r, label.color.g, label.color.b, tempCap);
	}

	void updateText(){
		if (scriptNumber == scriptNumberPrev) {
			isFadingOut = false;
		}
		if (scriptNumber != scriptNumberPrev) {
			isFadingOut = true;

			if(label.color.a < 0.01f){
				label.text = newText(scriptNumber);
				scriptNumberPrev = scriptNumber;
				isFadingOut = false;
			}
		}
	}

	string newText(int number){
		switch(number){
		case(0):
			return "'Sit and play with me.'";

		case(1):
			return "'Slow down some and talk.'";

		case(2):
			return "'We can enjoy this time.'"; 
		case(3):
			return "'Before we are called for dinner.'"; 
		case(4):
			return "'And maybe even once more after.'";
		default:
			return "'...'";
		}
	}

}

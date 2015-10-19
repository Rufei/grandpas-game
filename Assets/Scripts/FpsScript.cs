using UnityEngine;
using System.Collections;

public class FpsScript : MonoBehaviour {

	TextMesh label;

	// Use this for initialization
	void Start () {
		label = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = "F:" + (int)(1.0f / Time.deltaTime);
	}
}

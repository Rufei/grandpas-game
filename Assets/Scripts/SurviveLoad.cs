using UnityEngine;
using System.Collections;

public class SurviveLoad : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad(gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class LookAtScript : MonoBehaviour {
	public Transform target;
	// Use this for initialization
	public Vector3 cameraOrigin;

	public Vector3 LookAtTarget;


	void Start () {
		cameraOrigin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
	}

	
	// Update is called once per frame
	void Update () {
		//transform.LookAt(target);
		//transform.LookAt(new Vector3(10.5f,0.5f,7.5f));
		Vector3 mousePos = new Vector3 (Input.mousePosition.x-(Screen.width/2), Input.mousePosition.y-(Screen.height/2), Input.mousePosition.z);

		transform.position = new Vector3 (cameraOrigin.x+(mousePos.x*(0.0027f)), cameraOrigin.y+(mousePos.y*(-0.01f)+0.9f), cameraOrigin.z);

		LookAtTarget = new Vector3 (10.5f + (mousePos.x * 0.01f), 0.5f, 7.5f + (mousePos.y * 0.01f));

		transform.LookAt(new Vector3(LookAtTarget.x,LookAtTarget.y,LookAtTarget.z));
	}


	Vector3 getLookAtTarget(){
		return new Vector3(LookAtTarget.x,LookAtTarget.y,LookAtTarget.z);
	}

}

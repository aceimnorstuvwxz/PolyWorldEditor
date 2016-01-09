using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("v") || Input.GetMouseButton(1)) {
			
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			Debug.Log("x"+x.ToString() + " y"+y.ToString());
			float rotate_scale = 10f;
			//			transform.Rotate(new Vector3(y*rotate_scale, -x*rotate_scale,0));
			
			transform.RotateAround(Vector3.zero, transform.right, -y*rotate_scale);
			transform.RotateAround(Vector3.zero, Vector3.up, x*rotate_scale);
		}



		float scoll = Input.GetAxis ("Mouse ScrollWheel");
		float scoll_scale = 1f;
		transform.position = transform.position + (transform.forward) * transform.position.magnitude * scoll * scoll_scale;
	}
}

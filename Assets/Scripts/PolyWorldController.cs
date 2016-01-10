using UnityEngine;
using System.Collections;

public class PolyWorldController : MonoBehaviour {

	public GameObject poly_obj_fab;

	void Start () {
	
	}
	
	void Update () {
	
	}

	public void NewPolyObject(int id)
	{
		Debug.Log ("NewPolyObject");

		GameObject obj = Instantiate (poly_obj_fab) as GameObject;
		obj.transform.SetParent (transform);
		obj.transform.localPosition = Vector3.zero;
	}
}

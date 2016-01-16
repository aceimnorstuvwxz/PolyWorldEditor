using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<TransformSmoother> ().SetLocalPosition (Vector3.one,5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

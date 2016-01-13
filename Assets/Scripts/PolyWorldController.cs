using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyWorldController : MonoBehaviour {

	public GameObject poly_obj_fab;

	private Dictionary<int, GameObject> _polyObjects;

	void Start () {

		_polyObjects = new Dictionary<int, GameObject> ();
	
	}
	
	void Update () {
	
	}

	public void NewPolyObject(int id)
	{
		Debug.Log ("NewPolyObject " + id.ToString());

		GameObject obj = Instantiate (poly_obj_fab) as GameObject;
		obj.transform.SetParent (transform);
		obj.transform.localPosition = Vector3.zero;

		_polyObjects.Add (id, obj);
	}

	public void DeletePolyObject(int id)
	{
		Debug.Log ("DeletePolyObject " + id.ToString());

		var obj = _polyObjects [id];
		_polyObjects.Remove (id);
		Destroy (obj);

	}

	public List<GameObject> GetSelectedGameObjects(List<int> keys)
	{
		List< GameObject> golist = new List<GameObject> ();
		foreach (int id in keys) {
			golist.Add(_polyObjects[id]);
		}
		return golist;
	}

	public void SetObjectSelection(int id, bool isSelected)
	{
		_polyObjects [id].layer = isSelected ? LayerMask.NameToLayer ("PolyObjectSelected") : 0;
	}
}

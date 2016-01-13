using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyWorldController : MonoBehaviour {
	public GameObject runtime_translation;

	public GameObject poly_obj_fab;

	private Dictionary<int, GameObject> _polyObjects;
	private List<int> _selectedObects;

	enum PresettedObjectType { Cube, Floor, Point, Sphere};

	void Start () {

		_polyObjects = new Dictionary<int, GameObject> ();
		_selectedObects = new List<int> ();
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

	public List<GameObject> GetSelectedGameObjects()
	{
		List< GameObject> golist = new List<GameObject> ();
		foreach (int id in _selectedObects) {
			golist.Add(_polyObjects[id]);
		}
		return golist;
	}

	public void SetObjectSelection(int id, bool isSelected)
	{
		_polyObjects [id].layer = isSelected ? LayerMask.NameToLayer ("PolyObjectSelected") : 0;
		_polyObjects [id].GetComponent<PolyObjectController> ()._selected = isSelected;
		if (isSelected && !_selectedObects.Contains (id)) {
			_selectedObects.Add (id);
		} 

		if (!isSelected && _selectedObects.Contains (id)) {
			_selectedObects.Remove(id);
		}
	}

	public void OnClickAddCube()
	{
		foreach (int id in _selectedObects) {
			var go = _polyObjects[id];
			go.GetComponent<PolyObjectController>().AddCube();
			Debug.Log("add cube ");
		}
	}

	public void RefreshMaterial(List<int> materials)
	{
		foreach (GameObject go in _polyObjects.Values) {
			go.GetComponent<PolyObjectController>().RefreshMesh();
		}
	}

	public void RefreshSelection()
	{
		var selectedGos = GetSelectedGameObjects ();
		runtime_translation.GetComponent<RuntimeTranslation> ().SetTargetGameObjects (selectedGos);
		List<GameObject> r = new List<GameObject>();
		foreach(var go in _polyObjects.Values) {
			r.Add(go);
		}
		runtime_translation.GetComponent<RuntimeTranslation> ().SetAllObjects (r);
	}
}

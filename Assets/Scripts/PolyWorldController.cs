using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PolyWorldController : MonoBehaviour, IRuntimeTranslationCallBack{
	public GameObject runtime_translation;

	public GameObject poly_obj_fab;

	private Dictionary<int, GameObject> _polyObjects;
	private List<int> _selectedObects;


	public enum PresetType { Sphere, Cube, Floor}; // floor = plane

	void Start () {

		_polyObjects = new Dictionary<int, GameObject> ();
		_selectedObects = new List<int> ();

		runtime_translation.GetComponent<RuntimeTranslation> ().SetCallBack (this);

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

	public void DuplicateObject(int srcId, int desId)
	{
		Debug.Log ("DuplicateObject src" + srcId.ToString() + " des" + desId.ToString());
		
		GameObject obj = Instantiate (poly_obj_fab) as GameObject;
		obj.transform.SetParent (transform);

		GameObject srcObj = _polyObjects [srcId];

		obj.transform.localPosition = srcObj.transform.localPosition + Vector3.one;
		obj.transform.localRotation = srcObj.transform.localRotation;
		obj.transform.localScale = srcObj.transform.localScale;

		obj.GetComponent<PolyObjectController> ().DuplicateFrom (srcObj.GetComponent<PolyObjectController> ());

		_polyObjects.Add (desId, obj);
	}

	public void DeletePolyObject(int id)
	{
		Debug.Log ("DeletePolyObject " + id.ToString());

		var obj = _polyObjects [id];
		_polyObjects.Remove (id);
		_selectedObects.Remove (id);
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
		_polyObjects [id].GetComponent<PolyObjectController> ().SetSelection(isSelected);
		if (isSelected && !_selectedObects.Contains (id)) {
			_selectedObects.Add (id);
		} 

		if (!isSelected && _selectedObects.Contains (id)) {
			_selectedObects.Remove(id);
		}
	}

	public void OnClickAddCube()
	{
//		foreach (int id in _selectedObects) {
//			var go = _polyObjects[id];
//			go.GetComponent<PolyObjectController>().AddCube();
//			Debug.Log("add cube ");
//		}
	}

	public void AddPreset(PresetType t, int value, float fillrate)
	{
//		Debug.Log ("add preset");
		/*
		switch (t) {
		case PresetType.Cube:
			AddPresetCubeAst(value);
			break;
			
		case PresetType.Sphere:
			AddPresetSphereAst(value);
			break;
			
		case PresetType.Floor:
			AddPresetFloorAst(value);
			break;

		default:
			Debug.Log("unimplemented preset type" + t.ToString());
			Debug.Assert(false);
		}
		*/

		foreach (int id in _selectedObects) {
			var go = _polyObjects[id];
			go.GetComponent<PolyObjectController>().AddPreset(t, value, fillrate);
		}
	}

	public void ClearA()
	{
		foreach (int id in _selectedObects) {
			var go = _polyObjects[id];
			go.GetComponent<PolyObjectController>().ClearA();
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

	public void SetObjectVisibility(int id, bool isShow)
	{
		var go = _polyObjects [id];
		go.GetComponent<MeshRenderer> ().enabled = isShow;
	}

	public void OnRTEnter()
	{
		Debug.Log ("On rt enter");
		foreach (var go in _polyObjects.Values) {
			go.GetComponent<PolyObjectController>().SetTranslation(true);
		}
	}

	public void OnRTExit()
	{
		Debug.Log ("on rt exit");
		
		foreach (var go in _polyObjects.Values) {
			go.GetComponent<PolyObjectController>().SetTranslation(false);
		}
	}

	public Vector3 GetCameraFocusPosition()
	{
		if (_selectedObects.Count > 0) {
			int id = _selectedObects [_selectedObects.Count - 1];
			return _polyObjects [id].transform.position;
		} else {
			return Vector3.zero;
		}
	}
}

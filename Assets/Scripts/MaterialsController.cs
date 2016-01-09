using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialsController : MonoBehaviour {

	private int _materialIndex = 0;

	public GameObject material_fab;
	private GameObject _content;

	private Dictionary<int, GameObject> _materialsDict;

	private List<int> _selectedMaterials;

	// Use this for initialization
	void Start () {
		_content = GameObject.Find ("MaterialsContent");
		_materialsDict = new Dictionary<int, GameObject> ();
		_selectedMaterials = new List<int> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnNewMaterial()
	{
		Debug.Log ("new material");


		int materialId = _materialIndex++;

		
		var material = Instantiate (material_fab) as GameObject;
		material.transform.SetParent (_content.transform);
		//		var rect = layer.GetComponent<RectTransform> ();
		//		_lineHeight = rect.sizeDelta.y * 1.2f;
		//		rect.localPosition = new Vector3 (0, -_lineHeight * _layerDict.Count, 0);
		
		_materialsDict.Add (materialId, material);


		var materialController = material.GetComponent<MaterialLineController> ();
		materialController.material_id = materialId;
		
		RefreshContentLayout ();
		
		SelectMaterial (materialId);

	}

	void RefreshContentLayout()
	{
		int heightCount = 0;
		float lineHeight = 0;
		foreach (GameObject obj in _materialsDict.Values) {
			var rect = obj.GetComponent<RectTransform> ();
			lineHeight = rect.sizeDelta.y * 1.2f;
			rect.localPosition = new Vector3 (0, -lineHeight * heightCount, 0);
			heightCount++;
		}
		
		var contentRect = _content.GetComponent<RectTransform> ();
		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, _materialsDict.Count * lineHeight);
	}

	public void SelectMaterial(int materialId)
	{
		
		if (Input.GetKey ("right shift") || Input.GetKey ("left shift")) {
			//multiple selection
		} else {
			// de-select old ones
			foreach (int id in _selectedMaterials) {
				_materialsDict [id].GetComponent<MaterialLineController> ().SetSelection (false);
			}
			_selectedMaterials.Clear();
		}
		
		// select new
		if (!_selectedMaterials.Contains (materialId)) {
			_selectedMaterials.Add(materialId);
			_materialsDict [materialId].GetComponent<MaterialLineController> ().SetSelection (true);
		}

	}

	public void OnCombineMaterial()
	{

	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayersController : MonoBehaviour {

	public GameObject layer_line_fab;

	private GameObject _content;
	private int _layerIndex = 0;
	private PolyWorldController _polyWorldController;



//	private List<GameObject> _layerList;
	private Dictionary<int, GameObject> _layerDict;
	private List<int> _selectedLayers;

	// Use this for initialization
	void Start () {
		_content = GameObject.Find ("LayersContent");
		_layerDict = new Dictionary<int, GameObject> ();
		_selectedLayers = new List<int> ();
		_polyWorldController = GameObject.Find ("PolyWorldSpace").GetComponent<PolyWorldController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void OnClickNewLayer()
	{
		Debug.Log ("new layer");

		// tell other to make a new layer, 
		// will 
		int layerId = _layerIndex++;

		var layer = Instantiate (layer_line_fab) as GameObject;
		layer.transform.SetParent (_content.transform);

		_layerDict.Add (layerId, layer);

		var layerLine = layer.GetComponent<LayerLineController> ();
		layerLine.layer_id = layerId;

		RefreshContentLayout ();

		_polyWorldController.NewPolyObject (layerId);
		
		SelectLayer (layerId);
	}

	void RefreshContentLayout()
	{
		int heightCount = 0;
		float lineHeight = 0;
		foreach (GameObject obj in _layerDict.Values) {
			var rect = obj.GetComponent<RectTransform> ();
			lineHeight = rect.sizeDelta.y * 1.2f;
			rect.localPosition = new Vector3 (0, -lineHeight * heightCount, 0);

			heightCount++;
		}

		var contentRect = _content.GetComponent<RectTransform> ();
		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, _layerDict.Count * lineHeight);
	}

	public void OnClickDeleteLayer()
	{
		Debug.Log ("delete layer");

		// delete all selected
		foreach (int id in _selectedLayers) {
			var desObj = _layerDict[id];
			_layerDict.Remove(id);
			Destroy(desObj);

			
			_polyWorldController.DeletePolyObject(id);
		}
		_selectedLayers.Clear ();

		// reposition all
		RefreshContentLayout ();

	}

	public void SelectLayer(int layerId)
	{
		if (Input.GetKey ("right shift") || Input.GetKey ("left shift")) {
			//multiple selection
		} else {
			// de-select old ones
			foreach (int id in _selectedLayers) {
				_layerDict [id].GetComponent<LayerLineController> ().SetSelection (false);
				_polyWorldController.SetObjectSelection(id, false);
			}
			_selectedLayers.Clear();
		}

		// select new
		if (!_selectedLayers.Contains (layerId)) {
			_selectedLayers.Add(layerId);
			_layerDict [layerId].GetComponent<LayerLineController> ().SetSelection (true);
			_polyWorldController.SetObjectSelection(layerId, true);
		}

		_polyWorldController.RefreshSelection ();

	}
	
}

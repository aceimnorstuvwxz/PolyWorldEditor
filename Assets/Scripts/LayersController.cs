using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayersController : MonoBehaviour {

	public GameObject _layerFab;

	private GameObject _content;
	private int _layerIndex = 0;


//	private List<GameObject> _layerList;
	private Dictionary<int, GameObject> _layerDict;
	private List<int> _selectedLayers;

	// Use this for initialization
	void Start () {
		_content = GameObject.Find ("LayersContent");
		_layerDict = new Dictionary<int, GameObject> ();
		_selectedLayers = new List<int> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	private float _lineHeight = 0;
	public void OnClickNewLayer()
	{
		Debug.Log ("new layer");

		// tell other to make a new layer, 
		// will 
		int layerId = _layerIndex++;

		var layer = Instantiate (_layerFab) as GameObject;
		layer.transform.SetParent (_content.transform);
		var rect = layer.GetComponent<RectTransform> ();
		_lineHeight = rect.sizeDelta.y * 1.2f;
		rect.localPosition = new Vector3 (0, -_lineHeight * _layerDict.Count, 0);

		_layerDict.Add (layerId, layer);

		var layerLine = layer.GetComponent<LayerLineController> ();
		layerLine.layer_id = layerId;

		RefreshContentSize ();

		SelectLayer (layerId);
	}

	void RefreshContentSize()
	{
		var rect = _content.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (rect.sizeDelta.x, _layerDict.Count * _lineHeight);
	}

	public void OnClickDeleteLayer()
	{
		Debug.Log ("delete layer");
	}

	public void SelectLayer(int layerId)
	{

		if (Input.GetKey ("right shift") || Input.GetKey ("left shift")) {
			//multiple selection
		} else {
			// de-select old ones
			foreach (int id in _selectedLayers) {
				_layerDict [id].GetComponent<LayerLineController> ().SetSelection (false);
			}
			_selectedLayers.Clear();
		}

		// select new
		if (!_selectedLayers.Contains (layerId)) {
			_selectedLayers.Add(layerId);
			_layerDict [layerId].GetComponent<LayerLineController> ().SetSelection (true);
		}
	}
}

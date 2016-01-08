using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayersController : MonoBehaviour {

	public GameObject _layerFab;

	private GameObject _content;
	private int _layerIndex = 0;


	private List<GameObject> _layerList;

	// Use this for initialization
	void Start () {
		_content = GameObject.Find ("LayersContent");
		_layerList = new List<GameObject> ();
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
		rect.localPosition = new Vector3 (0, -_lineHeight * _layerList.Count, 0);

		_layerList.Add (layer);
		RefreshContentSize ();
	}

	void RefreshContentSize()
	{
		var rect = _content.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (rect.sizeDelta.x, _layerList.Count * _lineHeight);
	}

	public void OnClickDeleteLayer()
	{
		Debug.Log ("delete layer");
	}
}

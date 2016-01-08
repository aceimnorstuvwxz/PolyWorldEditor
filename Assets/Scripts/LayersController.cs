using UnityEngine;
using System.Collections;

public class LayersController : MonoBehaviour {

	public GameObject _layerFab;

	private GameObject _content;
	private int _layerIndex = 0;

	// Use this for initialization
	void Start () {
		_content = GameObject.Find ("LayersContent");
		Debug.Assert (_content);
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

		var newLayerLine = Instantiate (_layerFab) as GameObject;
		Debug.Assert (newLayerLine.transform != null);
		newLayerLine.transform.SetParent (_content.transform);

	}

	public void OnClickDeleteLayer()
	{
		Debug.Log ("delete layer");
	}
}

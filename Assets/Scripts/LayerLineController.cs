using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LayerLineController : MonoBehaviour {

	public int layer_id = 0;

	private LayersController _layersController;


	// Use this for initialization
	void Start () {
		_layersController = GameObject.Find ("LayersCanvas").GetComponent<LayersController> ();
		GetComponent<Outline> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickSelection() 
	{
		Debug.Log ("select layer id=" + layer_id.ToString ());

		_layersController.SelectLayer (layer_id);
	}

	public void SetSelection(bool isSelected)
	{
		GetComponent<Outline> ().enabled = isSelected;
	}
}

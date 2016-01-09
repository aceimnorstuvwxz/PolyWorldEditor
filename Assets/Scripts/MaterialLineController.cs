using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaterialLineController : MonoBehaviour {

	public int material_id;


	private MaterialsController _materialsController;
	// Use this for initialization
	void Start () {
	
		_materialsController = GameObject.Find ("MaterialsCanvas").GetComponent<MaterialsController> ();
		Debug.Assert (_materialsController != null);
		GetComponent<Outline> ().enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickSelection()
	{
		Debug.Log ("select material id=" + material_id.ToString ());
		_materialsController.SelectMaterial (material_id);
	}

	public void SetSelection(bool isSelected)
	{
//		Debug.Log ("set selection mat"+material_id.ToString() + isSelected.ToString());
		GetComponent<Outline> ().enabled = isSelected;
	}

	public void SetColor(Color color)
	{
		GetComponent<Image> ().color = color;
		GetComponentInChildren<Text> ().color = new Color (1f - color.r, 1f - color.g, 1f - color.b, 1f);
	}

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorState : MonoBehaviour {


	public bool is_add = true;
	public float emission_per_second = 5f;
	public float emission_wait_time = 0.2f;


	public GameObject text_preset_value;
	public GameObject text_preset_fillrate;
	public GameObject drop_preset_types;

	public GameObject light_object;

	private Text _textEmit;

	private PolyWorldController _polyWorldController;
	private BrushController _brushController;

	private int _presetValue = 1;
	private float _presetFillrate = 1f;

	void Start()
	{
		_textEmit = GameObject.Find ("TextEmit").GetComponent<Text> ();
		_textEmit.text = emission_per_second.ToString ();

		_polyWorldController = GameObject.Find ("PolyWorldSpace").GetComponent<PolyWorldController> ();
		_brushController = GameObject.Find ("Brush").GetComponent<BrushController> ();
	}


	public void  OnEmitValueChanged(float radio)
	{
		emission_per_second = Mathf.Lerp (1, 20, radio);
		emission_wait_time = 1f / emission_per_second;
		_textEmit.text = emission_per_second.ToString ();

	}

	public void OnPresetValueChange(float value)
	{
		int iv = (int)value;

		text_preset_value.GetComponent<Text> ().text = iv.ToString ();

		_presetValue = iv;
	}

	public void OnPresetFillrateChange(float value)
	{
		_presetFillrate = value;
		text_preset_fillrate.GetComponent<Text> ().text = _presetFillrate.ToString ();
	}

	public void OnClickAddPreset()
	{
		int v = drop_preset_types.GetComponent<Dropdown> ().value;

		Debug.Log ("add preset " + v.ToString() + " " + _presetValue.ToString());


		PolyWorldController.PresetType t = v == 0 ? PolyWorldController.PresetType.Sphere :
			v == 1 ? PolyWorldController.PresetType.Cube : 
				PolyWorldController.PresetType.Floor;
		_polyWorldController.AddPreset (t, _presetValue, _presetFillrate);
	}

	public void OnClickBtnClear()
	{
		_polyWorldController.ClearA();
	}

	public void OnBackgroundChange(float value)
	{
		Camera.main.backgroundColor = new Color (value, value, value, 1);
	}

	public void OnLightChange(float value)
	{
		light_object.GetComponent<Light>().color = new Color (value, value, value, 1);
	}

	public void OnSelectBrushShape(int value)
	{
		_brushController.SetBrushShape (value == 0 ? BrushController.BrushShape.Cylinder :
		                               value == 1 ? BrushController.BrushShape.Sphere :
		                               BrushController.BrushShape.Cube);
	}

	private float _brushWidth = 1f;
	private float _brushHeight = 1f;
	public GameObject text_brush_width;
	public GameObject text_brush_height;

	public void OnBrushWidthChange(float value)
	{
		_brushWidth = value;
		_brushController.SetBrushSize (_brushWidth, _brushHeight);
		text_brush_width.GetComponent<Text> ().text = ((int)(value)).ToString ();
	}

	public void OnBrushHeightChange(float value)
	{
		_brushHeight = value;
		_brushController.SetBrushSize (_brushWidth, _brushHeight);
		text_brush_height.GetComponent<Text> ().text = ((int)(value)).ToString ();
	}
}

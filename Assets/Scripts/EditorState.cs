using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorState : MonoBehaviour {


	public bool is_add = true;
	public float emission_per_second = 5f;
	public float emission_wait_time = 0.2f;


	public GameObject text_preset_value;
	public GameObject drop_preset_types;

	private Text _textEmit;

	private PolyWorldController _polyWorldController;

	private int _presetValue = 1;

	void Start()
	{
		_textEmit = GameObject.Find ("TextEmit").GetComponent<Text> ();
		_textEmit.text = emission_per_second.ToString ();

		_polyWorldController = GameObject.Find ("PolyWorldSpace").GetComponent<PolyWorldController> ();
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

	public void OnClickAddPreset()
	{
		int v = drop_preset_types.GetComponent<Dropdown> ().value;

		Debug.Log ("add preset " + v.ToString() + " " + _presetValue.ToString());


		PolyWorldController.PresetType t = v == 0 ? PolyWorldController.PresetType.Sphere :
			v == 1 ? PolyWorldController.PresetType.Cube : 
				PolyWorldController.PresetType.Floor;
		_polyWorldController.AddPreset (t, _presetValue);
	}

	public void OnClickBtnClear()
	{
		_polyWorldController.ClearA();
	}
}

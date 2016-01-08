using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorState : MonoBehaviour {


	public bool is_add = true;
	public float emission_per_second = 5f;
	public float emission_wait_time = 0.2f;





	private Text _textEmit;

	void Start()
	{
		_textEmit = GameObject.Find ("TextEmit").GetComponent<Text> ();
		_textEmit.text = emission_per_second.ToString ();
	}


	public void  OnEmitValueChanged(float radio)
	{
		emission_per_second = Mathf.Lerp (1, 20, radio);
		emission_wait_time = 1f / emission_per_second;
		_textEmit.text = emission_per_second.ToString ();

	}
}

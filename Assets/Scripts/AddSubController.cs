using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddSubController : MonoBehaviour {

	public Color ADD_COLOR = Color.green;
	public Color SUB_COLOR = Color.yellow;
	private EditorState _editorState;

	void Start()
	{
		_editorState = GameObject.Find ("UICanvas").GetComponent<EditorState> ();
		Refresh ();
	}

	void Refresh() 
	{
		GetComponent<Image> ().color = _editorState.is_add ? ADD_COLOR : SUB_COLOR;
		GetComponentInChildren<Text> ().text = _editorState.is_add ? "ADD" : "SUB";
	}

	void Update()
	{
		if (Input.GetKeyDown ("s")) {
			OnClick();
		}
		if (Input.GetKeyUp ("s")) {
			OnClick();
		}
	}

	public void OnClick()
	{
		_editorState.is_add = !_editorState.is_add;
		Refresh ();
	}
}

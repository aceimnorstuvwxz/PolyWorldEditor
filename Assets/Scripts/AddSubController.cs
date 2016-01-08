using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddSubController : MonoBehaviour {

	public Color ADD_COLOR = Color.green;
	public Color SUB_COLOR = Color.yellow;
	public int a = 0;

	private bool _isAdd = true;

	void Start()
	{
		Refresh ();
	}

	public bool IsAdd() {
		return true;

	}

	void Refresh() 
	{
		Color col = _isAdd ? ADD_COLOR : SUB_COLOR;

		GetComponent<Image> ().color = col;

		var tex = GetComponentInChildren<Text> ();
//		tex.color = col;
		tex.text = _isAdd ? "ADD" : "SUB";
	}

	public void OnClick()
	{
		_isAdd = !_isAdd;
		Refresh ();
	}
}

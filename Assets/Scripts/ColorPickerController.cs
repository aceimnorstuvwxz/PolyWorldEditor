using UnityEngine;
using System.Collections;

public class ColorPickerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnSetColor(Color color)
	{
		Debug.Log ("on set color, " + color.ToString ());
	}

	public Color OnGetColor()
	{
		Debug.Log ("OnGetColor, ");
		return Color.red;

	}
}

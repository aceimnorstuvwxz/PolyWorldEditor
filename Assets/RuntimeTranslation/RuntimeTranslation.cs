using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RuntimeTranslation : MonoBehaviour {
	public GameObject btn_move;
	public GameObject btn_rotate;
	public GameObject btn_scale;
	
	// Use this for initialization
	void Start () {
		SetBtnSelection (btn_move, false);
		SetBtnSelection (btn_rotate, false);
		SetBtnSelection (btn_scale, false);
	}
	
	// Update is called once per frame
	void Update () {
		// escape to exit any edit mode
		if (Input.GetKeyDown ("escape")) {
			SetBtnSelection (btn_move, false);
			SetBtnSelection (btn_rotate, false);
			SetBtnSelection (btn_scale, false);
		}

		// update gizmo's scale, so when camera move, it always has the same vision size!!!
		// TODO
	}

	void SetBtnSelection(GameObject btn, bool isSelected)
	{
		btn.GetComponent<Outline> ().enabled = isSelected;
		btn.GetComponent<Image> ().color = isSelected ? Color.gray : Color.white;
	}

	bool GetBtnSelection(GameObject btn)
	{
		return btn.GetComponent<Outline> ().enabled;
	}
	
	public void OnClickBtnMove()
	{

		Debug.Log ("OnClickBtnMove");

		{
			var theBtn = btn_move;
			if (GetBtnSelection (theBtn)) {
				SetBtnSelection (theBtn, false);
				// toggle off
			} else {
				// toggle on, and off others
				// first, off all
				SetBtnSelection(btn_move, false);
				SetBtnSelection(btn_rotate, false);
				SetBtnSelection(btn_scale, false);
				// on the one
				SetBtnSelection(theBtn, true);
			}
		}
	}

	public void OnClickBtnRotate()
	{
		Debug.Log ("OnClickBtnRotate");
		{
			var theBtn = btn_rotate;
			if (GetBtnSelection (theBtn)) {
				SetBtnSelection (theBtn, false);
				// toggle off
			} else {
				// toggle on, and off others
				// first, off all
				SetBtnSelection(btn_move, false);
				SetBtnSelection(btn_rotate, false);
				SetBtnSelection(btn_scale, false);
				// on the one
				SetBtnSelection(theBtn, true);
			}
		}
	}

	public void OnClickBtnScale()
	{
		Debug.Log ("OnClickBtnScale");
		{
			var theBtn = btn_scale;
			if (GetBtnSelection (theBtn)) {
				SetBtnSelection (theBtn, false);
				// toggle off
			} else {
				// toggle on, and off others
				// first, off all
				SetBtnSelection(btn_move, false);
				SetBtnSelection(btn_rotate, false);
				SetBtnSelection(btn_scale, false);
				// on the one
				SetBtnSelection(theBtn, true);
			}
		}
	}

	public void SetPosition(Vector3 position)
	{

	}
}

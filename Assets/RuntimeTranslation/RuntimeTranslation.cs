using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RuntimeTranslation : MonoBehaviour {
	public GameObject btn_move;
	public GameObject btn_rotate;
	public GameObject btn_scale;

	public GameObject gizmo_move;
	public GameObject gizmo_rotate;
	public GameObject gizmo_scale;

	enum RTT {MOVE, ROTATE, SCALE};
	
	// Use this for initialization
	void Start () {
		SetBtnSelection(RTT.MOVE, false);
		SetBtnSelection(RTT.SCALE, false);
		SetBtnSelection(RTT.ROTATE, false);
	}
	
	// Update is called once per frame
	void Update () {
		// escape to exit any edit mode
		if (Input.GetKeyDown ("escape")) {
			SetBtnSelection(RTT.MOVE, false);
			SetBtnSelection(RTT.SCALE, false);
			SetBtnSelection(RTT.ROTATE, false);
		}

		// update gizmo's scale, so when camera move, it always has the same vision size!!!
		// TODO
	}

	GameObject RTT2Btn(RTT t)
	{
		return t == RTT.MOVE ? btn_move :
			t == RTT.ROTATE ? btn_rotate : btn_scale;
	}

	GameObject RTT2Gizmo(RTT t)
	{
		return t == RTT.MOVE ? gizmo_move :
			t == RTT.ROTATE ? gizmo_rotate : gizmo_scale;
	}

	void SetBtnSelection(RTT t, bool isSelected)
	{
		var btn = RTT2Btn (t);
		btn.GetComponent<Outline> ().enabled = isSelected;
		btn.GetComponent<Image> ().color = isSelected ? Color.gray : Color.white;

		var gizmo = RTT2Gizmo (t);
		gizmo.SetActive (isSelected);
	}

	bool GetBtnSelection(RTT t)
	{
		var btn = RTT2Btn (t);
		return btn.GetComponent<Outline> ().enabled;
	}
	
	public void OnClickBtnMove()
	{

		Debug.Log ("OnClickBtnMove");

		{
			var theBtn = RTT.MOVE;
			if (GetBtnSelection (theBtn)) {
				SetBtnSelection (theBtn, false);
				// toggle off
			} else {
				// toggle on, and off others
				// first, off all
				SetBtnSelection(RTT.MOVE, false);
				SetBtnSelection(RTT.SCALE, false);
				SetBtnSelection(RTT.ROTATE, false);
				// on the one
				SetBtnSelection(theBtn, true);
			}
		}
	}

	public void OnClickBtnRotate()
	{
		Debug.Log ("OnClickBtnRotate");
		{
			var theBtn = RTT.ROTATE;
			if (GetBtnSelection (theBtn)) {
				SetBtnSelection (theBtn, false);
				// toggle off
			} else {
				// toggle on, and off others
				// first, off all
				SetBtnSelection(RTT.MOVE, false);
				SetBtnSelection(RTT.SCALE, false);
				SetBtnSelection(RTT.ROTATE, false);
				// on the one
				SetBtnSelection(theBtn, true);
			}
		}
	}

	public void OnClickBtnScale()
	{
		Debug.Log ("OnClickBtnScale");
		{
			var theBtn = RTT.SCALE;
			if (GetBtnSelection (theBtn)) {
				SetBtnSelection (theBtn, false);
				// toggle off
			} else {
				// toggle on, and off others
				// first, off all
				SetBtnSelection(RTT.MOVE, false);
				SetBtnSelection(RTT.SCALE, false);
				SetBtnSelection(RTT.ROTATE, false);
				// on the one
				SetBtnSelection(theBtn, true);
			}
		}
	}

	public void SetPosition(Vector3 position)
	{

	}
}

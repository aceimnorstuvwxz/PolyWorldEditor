using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RuntimeTranslation : MonoBehaviour {
	public GameObject test_target;
	public GameObject btn_move;
	public GameObject btn_rotate;
	public GameObject btn_scale;

	public GameObject btn_global_local;

	public GameObject gizmo_move;
	public GameObject gizmo_rotate;
	public GameObject gizmo_scale;


	private List<GameObject> _targetObjects;
	private GameObject _mainTargetObject;
	private bool _isCurrentGlobal = true;

	enum RTT {MOVE, ROTATE, SCALE, NONE};

	private RTT _currentWorkingState = RTT.NONE;
	
	// Use this for initialization
	void Start () {
		SetBtnSelection(RTT.MOVE, false);
		SetBtnSelection(RTT.SCALE, false);
		SetBtnSelection(RTT.ROTATE, false);

		if (test_target != null) {
			List<GameObject> l = new List<GameObject>();
			l.Add(test_target);
			SetTargetGameObjects(l);
		}

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


		// do real work!
		if (_mainTargetObject != null && _currentWorkingState != RTT.NONE) {
			UpdateTranslation();
		}
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

		if (isSelected) {
			_currentWorkingState = t;
		} else {
			if (_currentWorkingState == t) {
				_currentWorkingState = RTT.NONE;
			}
		}
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

	public void OnClickBtnGlobalLocal()
	{
		_isCurrentGlobal = !_isCurrentGlobal;
		btn_global_local.GetComponentInChildren<Text>().text = _isCurrentGlobal ? "Global" : "Local";

		RefreshGizmoGlocalLocal ();
	}

	void RefreshGizmoGlocalLocal()
	{
		if (_isCurrentGlobal) {
			gizmo_move.transform.rotation = Quaternion.identity;
			gizmo_rotate.transform.rotation = Quaternion.identity;
			gizmo_scale.transform.rotation = Quaternion.identity;
		} else {
			if (_mainTargetObject != null) {
				Quaternion rot = _mainTargetObject.transform.rotation;
				
				gizmo_move.transform.rotation = rot;
				gizmo_rotate.transform.rotation = rot;
				gizmo_scale.transform.rotation = rot;
			}
		}
	}

	public void SetTargetGameObjects(List<GameObject> targets)
	{
		_targetObjects = targets;
		if (targets.Count > 0) {
			_mainTargetObject = targets [0];
		} else {
			_mainTargetObject = null;
			Debug.Log("empty controll target list");
		}

		RefreshGizmoGlocalLocal ();
	}

	void UpdateTranslation()
	{
		RaycastHit hit;

		int gizmoLayer = LayerMask.NameToLayer ("RTGizmo");
		int mask = (1 << gizmoLayer);

		if (!Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, mask)) {
			return;
		}


		string tag = hit.collider.gameObject.tag;
		Debug.Log (tag);
	}

}

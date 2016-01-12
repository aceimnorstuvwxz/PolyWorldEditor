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
	enum RTA {R,G,B,C};

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

	private bool _mouseTouching = false;
	private RTA _mouseTouchingAxis = RTA.B;
	private Vector3 _hitNormal = Vector3.one;
	private Vector3 _hitPosition = Vector3.one;
	void UpdateTranslation()
	{

		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;

			int gizmoLayer = LayerMask.NameToLayer ("RTGizmo");
			int mask = (1 << gizmoLayer);

			if (!Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, mask)) {
				return;
			}


			string tag = hit.collider.gameObject.tag;
			Debug.Log (tag);
			_mouseTouching = true;
			_mouseTouchingAxis = tag == "RT_R" ? RTA.R :
				tag == "RT_G" ? RTA.G :
					tag == "RT_B" ? RTA.B : RTA.C;

			_hitNormal = hit.normal;
			_hitPosition = hit.point;
		}

		if (Input.GetMouseButtonUp (0)) {
			_mouseTouching = false;
		}

		if (_mouseTouching) {
			float dx = Input.GetAxis("Mouse X");
			float dy = Input.GetAxis("Mouse Y");
			Debug.Log("mouse move "+dx.ToString() + " " + dy.ToString());

			KissAss(dx, dy);

		}
	}


	void KissAss(float dx, float dy)
	{
		if (_currentWorkingState == RTT.MOVE) {
			
			//project to screen space, then dot product, scalre radio!!

			Vector3 movDir = _mouseTouchingAxis == RTA.R ? gizmo_move.transform.right :
				_mouseTouchingAxis == RTA.G ? gizmo_move.transform.up : gizmo_move.transform.forward;

			Vector3 srcPoint = gizmo_move.transform.position;
			Vector3 dirPoint = srcPoint + movDir;


			Vector3 screenSrcPoint = Camera.main.WorldToScreenPoint(srcPoint);
			Vector3 screenDirPoint = Camera.main.WorldToScreenPoint(dirPoint);
			Debug.Log(screenSrcPoint.ToString());
			Debug.Log(screenDirPoint.ToString());

			Vector3 screenSpaceDir3 = screenDirPoint-screenSrcPoint;
			Vector2 screenSpaceDir = new Vector2(screenSpaceDir3.x, screenSpaceDir3.y);
			Vector2 screenMov = new Vector2(dx,dy);

			float mag = screenSpaceDir.magnitude;
			float radio = mag == 0f ? 0f : (Vector2.Dot(screenMov, screenSpaceDir) / mag);

			Vector3 resMoveDiff = radio * movDir;
			gizmo_move.transform.position = gizmo_move.transform.position + resMoveDiff;
			foreach(GameObject go in _targetObjects) {
				go.transform.position = go.transform.position + resMoveDiff;
			}

		} else if (_currentWorkingState == RTT.ROTATE) {

			// find the hit normal vector, cross product with the rotate axis, the result is tangent vector,
			// project that to screen space, then mouse move diff dot product with it, all done!

			Vector3 rotateRollAxis = _mouseTouchingAxis == RTA.R ? gizmo_rotate.transform.right :
				_mouseTouchingAxis == RTA.G ? gizmo_rotate.transform.up : gizmo_rotate.transform.forward;
			Vector3 tangentDir = Vector3.Cross(rotateRollAxis, _hitNormal).normalized;


			Vector3 worldSrcPoint = _hitPosition;
			Vector3 worldDesPoint = _hitPosition + tangentDir;

			Vector3 screenSrcPoint = Camera.main.WorldToScreenPoint(worldSrcPoint);
			Vector3 screenDesPoint = Camera.main.WorldToScreenPoint(worldDesPoint);

			Vector2 screenTangentDir = new Vector2(screenDesPoint.x - screenSrcPoint.x, screenDesPoint.y - screenSrcPoint.y);
			float mag = Vector2.Dot(new Vector2(dx, dy), screenTangentDir);

			float degreeSpeed = 180f / Screen.height;

			float rotateDegree = degreeSpeed * mag;

			gizmo_rotate.transform.RotateAround(gizmo_rotate.transform.position, rotateRollAxis, rotateDegree);
			foreach(GameObject go in _targetObjects) {
				go.transform.RotateAround(gizmo_rotate.transform.position, rotateRollAxis, rotateDegree);
			}

		} else if (_currentWorkingState == RTT.SCALE) {

			// no global scaling, all is local
			// when [shift], scale in all axises!

			Vector3 movDir = _mouseTouchingAxis == RTA.R ? gizmo_scale.transform.right :
				_mouseTouchingAxis == RTA.G ? gizmo_scale.transform.up : 
					_mouseTouchingAxis == RTA.B ? gizmo_scale.transform.forward : gizmo_scale.transform.up;
			
			Vector3 srcPoint = gizmo_scale.transform.position;
			Vector3 dirPoint = srcPoint + movDir;
			
			Vector3 screenSrcPoint = Camera.main.WorldToScreenPoint(srcPoint);
			Vector3 screenDirPoint = Camera.main.WorldToScreenPoint(dirPoint);
			Debug.Log(screenSrcPoint.ToString());
			Debug.Log(screenDirPoint.ToString());
			
			Vector3 screenSpaceDir3 = screenDirPoint-screenSrcPoint;
			Vector2 screenSpaceDir = new Vector2(screenSpaceDir3.x, screenSpaceDir3.y);
			Vector2 screenMov = new Vector2(dx,dy);
			
			float mag = screenSpaceDir.magnitude;
			float radio = mag == 0f ? 0f : (Vector2.Dot(screenMov, screenSpaceDir) / mag);
			float base_ment = 50f;
			float scale = (base_ment + radio * mag)/base_ment;

			Vector3 scaleVect = new Vector3(_mouseTouchingAxis == RTA.R ? scale : 1f , 
			                                _mouseTouchingAxis == RTA.G ? scale : 1f , 
			                                _mouseTouchingAxis == RTA.B ? scale : 1f );
			if (_mouseTouchingAxis == RTA.C || Input.GetKey("left shift")|| Input.GetKey("right shift") ) {
				scaleVect = Vector3.one *scale;
			}

			Vector3 oldScale = gizmo_scale.transform.localScale;

			gizmo_scale.transform.localScale =  Vector3.Scale(oldScale, scaleVect);
			foreach(GameObject go in _targetObjects) {
				go.transform.localScale = Vector3.Scale(go.transform.localScale, scaleVect);
			}

		}
	}

}

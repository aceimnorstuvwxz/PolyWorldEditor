using UnityEngine;
using System.Collections;

public class TransformSmoother : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		if (_aniMoving == true) {
			transform.localPosition += _step*Time.deltaTime;
			_pastTime += Time.deltaTime;
			if (_pastTime >= _moveTime) {
				_aniMoving = false;
			}
		}
	}

	private float _moveTime;
	private Vector3 _step;
	private bool _aniMoving = false;
	private float _pastTime;

	public void SetLocalPosition(Vector3 p, float t)
	{
		_moveTime = t;
		_step = (p - transform.localPosition) / _moveTime;
		_pastTime = 0;
		_aniMoving = true;
	}
}

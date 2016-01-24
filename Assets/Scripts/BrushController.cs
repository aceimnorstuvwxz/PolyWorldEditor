using UnityEngine;
using System.Collections;

public class BrushController : MonoBehaviour {

	public GameObject goc_cylinder;
	public GameObject goc_sphere;
	public GameObject goc_cube;

	private PolyObjectController _targetPolyObject;
	public enum BrushMode { Normal, SharpFall, SoftFall };
	public enum BrushShape { Cylinder, Sphere, Cube};

	private BrushMode _brushMode = BrushMode.Normal;
	private BrushShape _brushShape = BrushShape.Cylinder;

	public void SetTargetPolyObject(GameObject obj)
	{
		transform.SetParent (obj.transform);
		_targetPolyObject = obj.GetComponent<PolyObjectController> ();
	}

	public void SetBrushMode(BrushMode m)
	{
		_brushMode = m;
		//TODO
	}

	public void SetBrushShape(BrushShape s)
	{
		_brushShape = s;
		//TODO
	}

	public void SetBrushSize(int size, int height)
	{
		// sphere's height, will make it flat or not
		var newScale = new Vector3 (size / 1f, height / 1f, size / 1f);
		goc_cylinder.transform.localScale = newScale;
		goc_cylinder.transform.localScale = newScale;
		goc_cylinder.transform.localScale = newScale;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_targetPolyObject != null && _targetPolyObject.IsEditable ()) {
			MouseBrush();
		}
	}

	void HelpSetColidderPosition(Vector3 localPosition, Vector3 localNormal)
	{
		var q = Quaternion.FromToRotation (new Vector3 (0, 1, 0), localNormal);

		goc_cube.transform.localPosition = localPosition;
		goc_cube.transform.localRotation = q;

		goc_cylinder.transform.localPosition = localPosition;
		goc_cylinder.transform.localRotation = q;

		goc_sphere.transform.localPosition = localPosition;
		goc_sphere.transform.localRotation = q;
	}

	void MouseBrush()
	{
		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer ("PolyObjectSelected");
		if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask))
			return;
		
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null)
			return;
		
		Mesh mesh = meshCollider.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
		Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
		Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
		Transform hitTransform = hit.collider.transform;
		p0 = hitTransform.TransformPoint(p0);
		p1 = hitTransform.TransformPoint(p1);
		p2 = hitTransform.TransformPoint(p2);
		Debug.DrawLine(p0, p1);
		Debug.DrawLine(p1, p2);
		Debug.DrawLine(p2, p0);
		Debug.DrawRay(hit.point, hit.normal*10f);

		
		Vector3 localPoint = transform.InverseTransformPoint (hit.point);
		Vector3 localNormal = transform.InverseTransformDirection (hit.normal);
		HelpSetColidderPosition (localPoint, localNormal);
		
//		if (_shouldEmit && Input.GetMouseButton(0)) {
//			_shouldEmit = false;
//			
//			if (_editorState.is_add) {
//				AddBrush(localPoint, localNormal, 0);
//			} else {
//				SubBrush(localPoint, localNormal, 0);
//			}
//		}

	}
}

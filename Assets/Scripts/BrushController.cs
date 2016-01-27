using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrushController : MonoBehaviour {

	public GameObject goc_cylinder;
	public GameObject goc_sphere;
	public GameObject goc_cube;


	public GameObject go_ex_x0;
	public GameObject go_ex_x1;
	public GameObject go_ex_y0;
	public GameObject go_ex_y1;
	public GameObject go_ex_z0;
	public GameObject go_ex_z1;



	public GameObject go_extruder;

	private Collider cld_cylinder;
	private Collider cld_sphere;
	private Collider cld_cube;

	private PolyObjectController _targetPolyObject;
	public enum BrushMode { Normal, SharpFall, SoftFall };
	public enum BrushShape { Cylinder, Sphere, Cube};

	private BrushMode _brushMode = BrushMode.Normal;
	private BrushShape _brushShape = BrushShape.Cylinder;

	private EditorState _editorState;
	private Dictionary<IntVector3, int> _brushVoxelPoolBefore;
	private Dictionary<IntVector3, int> _brushVoxelPoolAfter;

	private Dictionary<IntVector3, int > _extrudeVoxelPool;

	
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

	private GameObject _currentShapeGo;
	public void SetBrushShape(BrushShape s)
	{
		_brushShape = s;

		goc_cylinder.SetActive (s == BrushShape.Cylinder);
		goc_sphere.SetActive (s == BrushShape.Sphere);
		goc_cube.SetActive (s == BrushShape.Cube);

		_currentShapeGo = s == BrushShape.Cube ? goc_cube :
			s == BrushShape.Sphere ? goc_sphere : goc_cylinder;
	}

	private float _brushWidth = 1f;
	private float _brushHeight = 1f;

	public void SetBrushSize(float width, float height)
	{
		// sphere's height, will make it flat or not

		_brushWidth = width;
		_brushHeight = height;

		var newScale = new Vector3 (width, height, width);
		goc_cylinder.transform.localScale = newScale;
		goc_sphere.transform.localScale = newScale;
		goc_cube.transform.localScale = newScale;
	}

	void Start () 
	{
		_extrudeVoxelPool =  new Dictionary<IntVector3, int> (new IntVector3.EqualityComparer ());
		_brushVoxelPoolAfter = new Dictionary<IntVector3, int> (new IntVector3.EqualityComparer ());
		_brushVoxelPoolBefore = new Dictionary<IntVector3, int> (new IntVector3.EqualityComparer ());

		_editorState = GameObject.Find ("UICanvas").GetComponent<EditorState> ();

		cld_cube = goc_cube.GetComponent<MeshCollider> ();
		cld_cylinder = goc_cylinder.GetComponent<MeshCollider> ();
		cld_sphere = goc_sphere.GetComponent<MeshCollider> ();


		SetBrushShape (BrushShape.Cylinder);
	}
	
	void Update () 
	{
		if (_targetPolyObject != null &&
		    _targetPolyObject.IsEditable ()) 
		{
			MouseExtrudeSelection();
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

	private IntVector3 _extrudeDirection = new IntVector3(0,0,0);
	void MouseExtrudeSelection()
	{
		if (_editorState.GetEditMode () == EditorState.EditMode.BRUSH)
			return;

		if (Input.GetMouseButtonDown (0)) {
			if (Doge.IsMouseOn (go_ex_x0)) {
				_extrudeDirection = new IntVector3 (1, 0, 0);
				Debug.Log ("x0");
			}
			if (Doge.IsMouseOn (go_ex_x1)) {
				_extrudeDirection = new IntVector3 (-1, 0, 0);
				Debug.Log ("x1");
			}
			if (Doge.IsMouseOn (go_ex_y0)) {
				_extrudeDirection = new IntVector3 (0, 1, 0);
				Debug.Log ("y0");
			}
			if (Doge.IsMouseOn (go_ex_y1)) {
				_extrudeDirection = new IntVector3 (0, -1, 0);
				Debug.Log ("y1");
				
			}
			if (Doge.IsMouseOn (go_ex_z0)) {
				_extrudeDirection = new IntVector3 (0, 0, 1);
				Debug.Log ("z0");
				
			}
			if (Doge.IsMouseOn (go_ex_z1)) {
				_extrudeDirection = new IntVector3 (0, 0, -1);
				Debug.Log ("z1");
				
			}
		}
	}
	void MouseBrush()
	{

		go_extruder.SetActive (_editorState.GetEditMode() == EditorState.EditMode.EXTRUDE);

		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer ("PolyObjectSelected");
		bool mouseHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, layerMask);
		_currentShapeGo.SetActive (mouseHit);

		if (!mouseHit)
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

		IntVector3 centerVoxel = IntVector3.FromFloat (localPoint);
		int len = (int)Mathf.Ceil (Mathf.Sqrt (_brushWidth * _brushWidth + _brushHeight * _brushHeight) * 0.5f);
//		Debug.Log ("len" + len.ToString () + centerVoxel.ToString());
		Collider cld = _brushShape == BrushShape.Cube ? cld_cube :
			_brushShape == BrushShape.Cylinder ? cld_cylinder : cld_sphere;
		var outside = Camera.main.transform.position; //new Vector3 (300, 300, 300);

		if (_editorState.GetEditMode () == EditorState.EditMode.BRUSH) {
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButton (0)) {
				for (int x = -len; x <= len; x++) {
					for (int y = -len; y <= len; y++) {
						for (int z = -len; z <= len; z++) {
							Vector3 underTestPoint = transform.TransformPoint (centerVoxel.ToFloat () + new Vector3 (x, y, z));

							if (Doge.IsColliderContainPoint (outside, underTestPoint, cld)) {
								var key = new IntVector3 (centerVoxel.x + x, centerVoxel.y + y, centerVoxel.z + z);
								if (_brushVoxelPoolBefore.ContainsKey (key)) {
									_brushVoxelPoolBefore.Remove (key);
								} 
								_brushVoxelPoolAfter.Add (key, 0);
								/*
								if (_editorState.is_add){
									_targetPolyObject.AddEditSpacePoint (centerVoxel.x + x, centerVoxel.y + y, centerVoxel.z + z);
								} else {
									_targetPolyObject.DeleteEditSpacePoint (centerVoxel.x + x, centerVoxel.y + y, centerVoxel.z + z);
								}
								*/
							}
						}
					}
				}

				foreach (var k in _brushVoxelPoolBefore.Keys) {
					_targetPolyObject.ConfigEditSpacePoint (k, _editorState.is_add);
				}

				_brushVoxelPoolBefore.Clear ();
				var tmpAfter = _brushVoxelPoolAfter;
				_brushVoxelPoolAfter = _brushVoxelPoolBefore;
				_brushVoxelPoolBefore = tmpAfter;
			
				_targetPolyObject.RefreshMesh ();
			}

			if (Input.GetMouseButtonUp (0)) {
				foreach (var k in _brushVoxelPoolBefore.Keys) {
					_targetPolyObject.ConfigEditSpacePoint (k, _editorState.is_add);
				}
				_brushVoxelPoolBefore.Clear ();
				_brushVoxelPoolAfter.Clear ();
				_targetPolyObject.RefreshMesh ();
			}
		} else {
			// ** extrude **
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButton (0)) {
				for (int x = -len; x <= len; x++) {
					for (int y = -len; y <= len; y++) {
						for (int z = -len; z <= len; z++) {
							Vector3 underTestPoint = transform.TransformPoint (centerVoxel.ToFloat () + new Vector3 (x, y, z));
							
							if (Doge.IsColliderContainPoint (outside, underTestPoint, cld)) {
								var key = new IntVector3 (centerVoxel.x + x, centerVoxel.y + y, centerVoxel.z + z);

								_extrudeVoxelPool[key] = 1;
							}
						}
					}
				}
			}

			RefreshExtruderPosition();
			
		}
	}

	void RefreshExtruderPosition()
	{
		if (_extrudeVoxelPool.Count > 0) {
			Vector3 sum = Vector3.zero;
			foreach (var ip in _extrudeVoxelPool.Keys) {
				sum += ip.ToFloat ();
			}
			var p = sum / _extrudeVoxelPool.Count;
			go_extruder.transform.localPosition = p;
		}
	}

	public void OnExtrude(int len)
	{
		if (_editorState.GetEditMode() == EditorState.EditMode.EXTRUDE) {
			
			var newPool = new Dictionary<IntVector3, int> (new IntVector3.EqualityComparer ());
			Debug.Log("extrude " + len.ToString());

			foreach(var pp in _extrudeVoxelPool.Keys) {
				var nVoxel = new IntVector3(pp.x + _extrudeDirection.x, 
				                            pp.y + _extrudeDirection.y,
				                            pp.z + _extrudeDirection.z);
				if (_editorState.is_add) {
					_targetPolyObject.CopyVoxel(pp, nVoxel);
				} else {
					_targetPolyObject.DeleteVoxel(pp);
				}
				newPool.Add(nVoxel, 1);
			}
			
			_targetPolyObject.RefreshMesh ();
			
			_extrudeVoxelPool = newPool;
			RefreshExtruderPosition ();
		}
	}
}

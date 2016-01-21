using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyObjectSegment : MonoBehaviour {

	public static int poly_object_segment_width = 20;

	public PolyObjectController _parentController;

	private int _count;

	public IntVector3 _segmentIndex;

	private int[,,] _editSpace; // 0 ? -> not solid, 1-N ->solid, with material index

	private MarchingCubesEngine _marchingCubesEngine;

	private bool _dirty = true;

	public void Init()
	{
		_marchingCubesEngine = GameObject.Find ("MarchingCubesEngine").GetComponent<MarchingCubesEngine> ();
		_editSpace = new int[poly_object_segment_width + 1, poly_object_segment_width + 1, poly_object_segment_width + 1];
		_count = 0;
	}

	void Start () 
	{
	}
	
	void Update () 
	{
	
	}

	public void SetVoxelPoint(IntVector3 relativePosition, int material)
	{
//		Debug.Log ("" + relativePosition.ToString ());
		int old = _editSpace[relativePosition.x, relativePosition.y, relativePosition.z];
		if (old == 0 && material > 0) {
			_count ++;
		}
		if (old > 0 && material == 0) {
			_count --;
		}
		if (old != material) {
			_dirty = true;
		}
		_editSpace [relativePosition.x, relativePosition.y, relativePosition.z] = material;
	}
	/*
	public void SetAdditiveVoxelPoint(IntVector3 relativePosition, int material)
	{
		int old 
		_editSpace [relativePosition.x, relativePosition.y, relativePosition.z] = material;
	}*/

	public int GetVoxelPoint(IntVector3 relativePosition)
	{
		return _editSpace [relativePosition.x, relativePosition.y, relativePosition.z];
	}

	public bool IsEmpty() 
	{
		return _count == 0;
	}

	public void RefreshMesh()
	{
		if (!_dirty) {
			return;
		}

		_dirty = false;
		if (_count == 0) {
			_parentController.DeleteSegment(_segmentIndex);
		} else {
			Debug.Log ("seg refresh mesh");
			var ret = _marchingCubesEngine.Marching (_editSpace, _segmentIndex.multi (poly_object_segment_width));
		
			Mesh mesh = new Mesh ();
			var meshFilter = GetComponent<MeshFilter> ();
			meshFilter.mesh = mesh;
			mesh.vertices = ret.vertices.ToArray ();
			mesh.triangles = ret.triangles.ToArray ();
			//		mesh.uv = _uvs.ToArray();
			mesh.RecalculateNormals ();
			mesh.colors = ret.colors.ToArray ();
		
			var meshColider = GetComponent<MeshCollider> ();
			meshColider.sharedMesh = mesh;
		}
	}
}

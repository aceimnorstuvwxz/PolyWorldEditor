using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyObjectSegment : MonoBehaviour {

	public static int poly_object_segment_width = 20;

	private int _count;

	public IntVector3 _segmentIndex;

	private int[,,] _editSpace; // 0 ? -> not solid, 1-N ->solid, with material index
	
	void Start () 
	{
		_editSpace = new int[poly_object_segment_width + 1, poly_object_segment_width + 1, poly_object_segment_width + 1];
		_count = 0;
	}
	
	void Update () 
	{
	
	}

	public void SetVoxelPoint(IntVector3 relativePosition, int material)
	{
		int old = _editSpace[relativePosition.x, relativePosition.y, relativePosition.z];
		if (old == 0 && material > 0) {
			_count ++;
		}
		if (old > 0 && material == 0) {
			_count --;
		}
		_editSpace [relativePosition.x, relativePosition.y, relativePosition.z] = material;
	}

	public void SetAdditiveVoxelPoint(IntVector3 relativePosition, int material)
	{
		_editSpace [relativePosition.x, relativePosition.y, relativePosition.z] = material;
	}

	public int GetVoxelPoint(IntVector3 relativePosition)
	{
		return _editSpace [relativePosition.x, relativePosition.y, relativePosition.z];
	}

	public bool IsEmpty() 
	{
		return _count == 0;
	}

	private List<Vector3> _vertices; //vertices
	//	private List<Vector2> _uvs;
	private List<int> _triangles; //index
	private List<Color> _colors;
	private MarchingCubes _marchingCubes  = new MarchingCubes();

	public void RefreshMesh()
	{
		_vertices = new List<Vector3> ();
		_triangles = new List<int> ();
		_colors = new List<Color> ();
		
		
		//marching cubes
		for (int x = 0; x < poly_object_segment_width; x++) {
			for (int y = 0; y < poly_object_segment_width; y++) {
				for (int z = 0; z <poly_object_segment_width; z++) {
//					MarchPerCube(x,y,z);
				}
			}
		}
		
		//		setCurrentMeshFilter ();
		
		Mesh mesh = new Mesh();
		var meshFilter = GetComponent<MeshFilter> ();
		meshFilter.mesh = mesh;
		mesh.vertices = _vertices.ToArray();
		mesh.triangles = _triangles.ToArray();
		//		mesh.uv = _uvs.ToArray();
		mesh.RecalculateNormals();
		mesh.colors = _colors.ToArray ();
		
		Debug.Log ("TerrainFab vertices = " + _vertices.Count.ToString());
		
		var meshColider = GetComponent<MeshCollider> ();
		meshColider.sharedMesh = mesh;
	}
}

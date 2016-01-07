using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Demo : MonoBehaviour {

	private int EDITOR_SPACE_HALF_WIDTH = 20; //500->11G memory, 100 11G/125=500M
//
//	private List<Vector3> _vertices;
//	private List<Vector2> _uvs;
//	private List<int> _triangles;

	private bool[,,] _editSpace;

	void SetEditSpacePoint(int x, int y, int z, bool value)
	{
		_editSpace [x + EDITOR_SPACE_HALF_WIDTH, y + EDITOR_SPACE_HALF_WIDTH, z + EDITOR_SPACE_HALF_WIDTH] = value;
	}

	int GetEditSpacePoint(int x, int y, int z)
	{
		return _editSpace [x + EDITOR_SPACE_HALF_WIDTH, y + EDITOR_SPACE_HALF_WIDTH, z + EDITOR_SPACE_HALF_WIDTH] ? 1:0;
	}


	// VoxelPoint, the meta points of marching cubes
	class VoxelPoint
	{
		// direction relation is same to unity scene editor
		public int x;
		public int y;
		public int z;

		public VoxelPoint(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public class EqualityComparer : IEqualityComparer<VoxelPoint> {
			
			public bool Equals(VoxelPoint a, VoxelPoint b) {
				return a.x == b.x && a.y == b.y && a.z == b.z;
			}
			
			public int GetHashCode(VoxelPoint a) {
				int c = a.x ^ a.y ^ a.z;
				return c.GetHashCode();
			}
		}
		
		public Vector3 toVector()
		{
			return new Vector3 ((float)x, (float)y, (float)z); //to Unity's axises
		}
	}



	void Start () {
		int width = 2 * EDITOR_SPACE_HALF_WIDTH + 1;
		_editSpace = new bool[width, width, width];	
	}
	
	// Update is called once per frame
	void Update () {
		MouseSelection ();
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
		for (int x = -EDITOR_SPACE_HALF_WIDTH; x <= EDITOR_SPACE_HALF_WIDTH-1; x++) {
			for (int y = -EDITOR_SPACE_HALF_WIDTH; y <= EDITOR_SPACE_HALF_WIDTH-1; y++) {
				for (int z = -EDITOR_SPACE_HALF_WIDTH; z <= EDITOR_SPACE_HALF_WIDTH-1; z++) {
					MarchPerCube(x,y,z);
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

	void MarchPerCube(int x, int y, int z)
	{
		int caseValue = 0;
		
		caseValue = caseValue * 2 + GetEditSpacePoint (x + 1, y, z + 1);//  _voxels[x+1,y+1,h];//v7
		caseValue = caseValue * 2 + GetEditSpacePoint (x + 1, y + 1, z + 1); // _voxels[x+1,y+1,h+1];//v6
		caseValue = caseValue * 2 + GetEditSpacePoint (x, y + 1, z + 1); //_voxels[x,y+1,h+1];//v5
		caseValue = caseValue * 2 + GetEditSpacePoint (x, y, z + 1);// _voxels[x,y+1,h];//v4
		caseValue = caseValue * 2 + GetEditSpacePoint (x + 1, y, z);//_voxels[x+1,y,h];//v3
		caseValue = caseValue * 2 + GetEditSpacePoint (x + 1, y + 1, z);//_voxels[x+1,y,h+1];//v2
		caseValue = caseValue * 2 + GetEditSpacePoint (x, y + 1, z);//_voxels[x,y,h+1];//v1
		caseValue = caseValue * 2 + GetEditSpacePoint (x, y, z);//_voxels[x,y,h];//v0
		
		int[,] caseTriangles = _marchingCubes.getCaseTriangles (caseValue);
		for (int i = 0; i < caseTriangles.GetLength(0); i++) {
			int edgeA = caseTriangles[i,0];
			int edgeB = caseTriangles[i,1];
			int edgeC = caseTriangles[i,2];

			AddTriangle(Edge2Position(edgeA, x, y, z), Edge2Position(edgeB, x, y, z), Edge2Position(edgeC, x, y, z));
		}
	}

	Vector3 Edge2Position(int edgeNumber, int originX, int originY, int originZ) {
		int dx;
		int dy;
		int dh;
		dx = dy = dh = 0;
		switch (edgeNumber) {
		case 0:
			dx = 0;
			dy = 0;
			dh = 1;
			break;
		case 1:
			dx = 1;
			dy = 0;
			dh = 2;
			break;
		case 2:
			dx = 2;
			dy = 0;
			dh = 1;
			break;
		case 3:
			dx = 1;
			dy = 0;
			dh = 0;
			break;
		case 4:
			dx = 0;
			dy = 2;
			dh = 1;
			break;
		case 5:
			dx = 1;
			dy = 2;
			dh = 2;
			break;
		case 6:
			dx = 2;
			dy = 2;
			dh = 1;
			break;
		case 7:
			dx = 1;
			dy = 2;
			dh = 0;
			break;
		case 8:
			dx = 0;
			dy = 1;
			dh = 0;
			break;
		case 9:
			dx = 0;
			dy = 1;
			dh = 2;
			break;
		case 10:
			dx = 2;
			dy = 1;
			dh = 2;
			break;
		case 11:
			dx = 2;
			dy = 1;
			dh = 0;
			break;
		default:
			Debug.Assert(false);
			break;
		}
		
		return new Vector3 (originX + dx*0.5f, originY + dh*0.5f, originZ + dy*0.5f);
	}

	void AddVertex(Vector3 node) {
		_vertices.Add (node);
//		_uvs.Add (node.toUV (_voxels.GetLength(0), _voxels.GetLength(2)));
		_colors.Add (Color.blue);
	}
	
	void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
	{
		if (_vertices.Count > 64990) {
			Debug.Log("vertice count out ");
			Debug.Assert(false);
		}

		
		_triangles.Add (_vertices.Count);
		AddVertex(a);
		
		_triangles.Add (_vertices.Count);
		AddVertex(b);
		
		_triangles.Add (_vertices.Count);
		AddVertex(c);

	}


	public void AddPlane ()
	{
		int planeYPos = 0;

		for (int x = -EDITOR_SPACE_HALF_WIDTH; x <= EDITOR_SPACE_HALF_WIDTH; x++) {
			for (int z = -EDITOR_SPACE_HALF_WIDTH; z <= EDITOR_SPACE_HALF_WIDTH; z++) {
				SetEditSpacePoint(x,planeYPos,z,true);
			}
		}
	}

	public void AddCube()
	{
		Debug.Log ("AddCube");
		int cubeHalfWidth = 5;

		for (int x = -cubeHalfWidth; x<= cubeHalfWidth; x++) {
			for (int y = -cubeHalfWidth; y <= cubeHalfWidth; y++) {
				for (int z = -cubeHalfWidth; z <= cubeHalfWidth; z++){
					SetEditSpacePoint(x,y,z,true);
				}
			}
		}

	}

	
	void MouseSelection()
	{
		RaycastHit hit;
		if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
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
//		Debug.Log ("working");

		Debug.DrawRay(hit.point, hit.normal*10f);
//		hit.point
		if (Input.GetMouseButton (0)) {
			AddBrush(hit.point, hit.normal);
		}
	}

	void AddBrush(Vector3 point, Vector3 normal)
	{
		// test, only the cloest point
		int x = Mathf.RoundToInt (point.x);
		int y = Mathf.RoundToInt (point.y);
		int z = Mathf.RoundToInt (point.z);
		if (GetEditSpacePoint (x, y, z) == 1) {
//			AddBrush(point + 0.1f*normal, normal);
		} else {
			SetEditSpacePoint (x, y, z, true);
			RefreshMesh ();
		}
	}
}

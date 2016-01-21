using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyObjectController : MonoBehaviour {

	public GameObject poly_object_segment_fab;

	public bool use_lerp_color = true;

	public Material mat_normal;
	public Material mat_translation;
	public Material mat_out_line;
	
	private int EDITOR_SPACE_HALF_WIDTH = 20; //500->11G memory, 100 11G/125=500M
	
	private int[,,] _editSpace; // 0 ? -> not solid, 1-N ->solid, with material index

	private Dictionary<IntVector3, GameObject> _segments;
	
	private EditorState _editorState;

	int MyDivision(int a, int b)
	{
		int s = a / b;
		if (a < 0 && a % b != 0) {
			s--;
		}
		return s;
	}

	IntVector3 WorldPosition2SegmentIndex(int x, int y, int z)
	{
		return new IntVector3 (MyDivision (x, PolyObjectSegment.poly_object_segment_width),
		                      MyDivision (y, PolyObjectSegment.poly_object_segment_width),
		                      MyDivision (z, PolyObjectSegment.poly_object_segment_width));
	}

	int MyModulo(int a, int b) 
	{
		int s = a % b;
		if (s < 0) {
			s += b;
		}
		return s;
	}

	IntVector3 WorldPosition2SegmentPosition(int x, int y, int z)
	{
		
		return new IntVector3 (MyModulo (x, PolyObjectSegment.poly_object_segment_width),
		                       MyModulo (y, PolyObjectSegment.poly_object_segment_width),
		                       MyModulo (z, PolyObjectSegment.poly_object_segment_width));
	}

	void ProcNewSegment(IntVector3 index)
	{
		GameObject segment = Instantiate(poly_object_segment_fab) as GameObject;
		_segments[index] = segment;
		segment.transform.SetParent(transform);
		var seg = segment.GetComponent<PolyObjectSegment>();
		seg._segmentIndex = index;
		seg.Init();
	}

	void SetEditSpacePoint(int x, int y, int z, int value)
	{
//		_editSpace [x + EDITOR_SPACE_HALF_WIDTH, y + EDITOR_SPACE_HALF_WIDTH, z + EDITOR_SPACE_HALF_WIDTH] = value;
		IntVector3 segmentIndex = WorldPosition2SegmentIndex (x, y, z);
		IntVector3 voxelPoint = WorldPosition2SegmentPosition (x, y, z);

		if (value > 0 && !_segments.ContainsKey (segmentIndex)) {
			ProcNewSegment(segmentIndex);
		}

		if ((value == 0 && _segments.ContainsKey (segmentIndex)) ||
		    (value > 0)) {
			var seg = _segments[segmentIndex].GetComponent<PolyObjectSegment>();
			seg.SetVoxelPoint(voxelPoint, value);
		}


		// aditive points

		bool ix = x % PolyObjectSegment.poly_object_segment_width == 0;
		bool iy = y % PolyObjectSegment.poly_object_segment_width == 0;
		bool iz = z % PolyObjectSegment.poly_object_segment_width == 0;

		// single 
		if (ix) {
			IntVector3 index = new IntVector3(segmentIndex.x - 1, segmentIndex.y, segmentIndex.z);
			IntVector3 point = new IntVector3(PolyObjectSegment.poly_object_segment_width,
			                                  voxelPoint.y,
			                                  voxelPoint.z);
			SetEditSpacePointAditive(index, point, value);
		}
		if (iy) {
			IntVector3 index = new IntVector3(segmentIndex.x, segmentIndex.y - 1, segmentIndex.z);
			IntVector3 point = new IntVector3(voxelPoint.x,
			                                  PolyObjectSegment.poly_object_segment_width,
			                                  voxelPoint.z);
			SetEditSpacePointAditive(index, point, value);
		}
		if (iz) {
			IntVector3 index = new IntVector3(segmentIndex.x, segmentIndex.y, segmentIndex.z - 1);
			IntVector3 point = new IntVector3(voxelPoint.x,
			                                  voxelPoint.y,
			                                  PolyObjectSegment.poly_object_segment_width);
			SetEditSpacePointAditive(index, point, value);
		}

		//double

		if (ix && iy) {
			IntVector3 index = new IntVector3(segmentIndex.x - 1, segmentIndex.y - 1, segmentIndex.z);
			IntVector3 point = new IntVector3(PolyObjectSegment.poly_object_segment_width,
			                                  PolyObjectSegment.poly_object_segment_width,
			                                  voxelPoint.z);
			SetEditSpacePointAditive(index, point, value);
		}

		if (iy && iz) {
			IntVector3 index = new IntVector3(segmentIndex.x, segmentIndex.y - 1, segmentIndex.z - 1);
			IntVector3 point = new IntVector3(voxelPoint.x, 
			                                  PolyObjectSegment.poly_object_segment_width,
			                                  PolyObjectSegment.poly_object_segment_width);
			SetEditSpacePointAditive(index, point, value);
		}

		if (ix && iz) {
			IntVector3 index = new IntVector3(segmentIndex.x - 1, segmentIndex.y, segmentIndex.z - 1);
			IntVector3 point = new IntVector3(PolyObjectSegment.poly_object_segment_width,
			                                  voxelPoint.y,
			                                  PolyObjectSegment.poly_object_segment_width);
			SetEditSpacePointAditive(index, point, value);
		}

		//trible
		if (ix && iy && iz) {
			
			IntVector3 index = new IntVector3(segmentIndex.x - 1, segmentIndex.y - 1, segmentIndex.z - 1);
			IntVector3 point = new IntVector3(PolyObjectSegment.poly_object_segment_width, 
			                                  PolyObjectSegment.poly_object_segment_width,
			                                  PolyObjectSegment.poly_object_segment_width);
			SetEditSpacePointAditive(index, point, value);
		}
	}

	void SetEditSpacePointAditive(IntVector3 index, IntVector3 point, int value) 
	{
		if (value > 0 && !_segments.ContainsKey (index)) {
			ProcNewSegment(index);
		}

		if (_segments.ContainsKey (index)) {
			var seg = _segments[index].GetComponent<PolyObjectSegment>();
			seg.SetAdditiveVoxelPoint(point, value);
		}
	}
	
	int GetEditSpacePoint(int x, int y, int z)
	{
		return _editSpace [x + EDITOR_SPACE_HALF_WIDTH, y + EDITOR_SPACE_HALF_WIDTH, z + EDITOR_SPACE_HALF_WIDTH];
	}

	int IsEditSpacePointSolid(int x, int y, int z)
	{
		return _editSpace [x + EDITOR_SPACE_HALF_WIDTH, y + EDITOR_SPACE_HALF_WIDTH, z + EDITOR_SPACE_HALF_WIDTH] > 0 ? 1 : 0;
	}

	public bool _selected = false;

	private RuntimeTranslation _runtimeTranslation;

	private ColorPicker _colorPicker;
	
	private MaterialsController _materialsController;
	private MeshRenderer _meshRenderer;

	private MarchingCubesEngine _marchingCubesEngine;
	void Start () 
	{
		_segments = new Dictionary<IntVector3, GameObject> (new IntVector3.EqualityComparer ());
		_marchingCubesEngine = GameObject.Find ("MarchingCubesEngine").GetComponent<MarchingCubesEngine> ();
		Debug.Assert (_marchingCubesEngine != null);
		_colorPicker = GameObject.Find ("ColorPicker").GetComponent<ColorPicker> ();
		_runtimeTranslation = GameObject.Find ("RuntimeTranslation").GetComponent<RuntimeTranslation> ();
		_editorState = GameObject.Find ("UICanvas").GetComponent<EditorState> ();
		_materialsController = GameObject.Find ("MaterialsCanvas").GetComponent<MaterialsController> ();
		Debug.Assert (_materialsController != null);

		int width = 2 * EDITOR_SPACE_HALF_WIDTH + 1;
		_editSpace = new int[width, width, width];
		
		if (use_lerp_color) {
			GenNearestVoxelPointForEdgePointTable ();
		} else {
			GenNearestVoxelPointTable ();
		}

		_meshRenderer = GetComponent<MeshRenderer> ();
	}
	
	
	void Update () 
	{
		if (_selected && !_runtimeTranslation.IsActive() && !_colorPicker.IsActive() && _meshRenderer.enabled) {
			EmissionCalc ();
			MouseBrush ();
		}
	}
	
	
	private List<Vector3> _vertices; //vertices
	//	private List<Vector2> _uvs;
	private List<int> _triangles; //index
	private List<Color> _colors;
	private MarchingCubes _marchingCubes  = new MarchingCubes();
	
	public void RefreshMesh()
	{
		/*
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
		}*/

		foreach (var go in _segments.Values) {
			go.GetComponent<PolyObjectSegment>().RefreshMesh();
		}


		var ret = _marchingCubesEngine.Marching (_editSpace, new IntVector3 (-EDITOR_SPACE_HALF_WIDTH,-EDITOR_SPACE_HALF_WIDTH,-EDITOR_SPACE_HALF_WIDTH));
		
		Mesh mesh = new Mesh();
		var meshFilter = GetComponent<MeshFilter> ();
		meshFilter.mesh = mesh;
		mesh.vertices = ret.vertices.ToArray ();//_vertices.ToArray();
		mesh.triangles = ret.triangles.ToArray ();// _triangles.ToArray();
		//		mesh.uv = _uvs.ToArray();
		mesh.RecalculateNormals();
		mesh.colors = ret.colors.ToArray ();//_colors.ToArray ();
		
//		Debug.Log ("TerrainFab vertices = " + _vertices.Count.ToString());
		
		var meshColider = GetComponent<MeshCollider> ();
		meshColider.sharedMesh = mesh;
	}
	
	int GetVoxelPointMaterial(int voxelRelativeIndex, int x, int y, int z)
	{
		IntVector3 diffPos = VoxelPointPosition(voxelRelativeIndex);
		int materialIndex = GetEditSpacePoint(x + diffPos.x, y + diffPos.y, z + diffPos.z);
		return materialIndex;
	}
	
	void MarchPerCube(int x, int y, int z)
	{
		int caseValue = 0;
		
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x + 1, y, z + 1);//  _voxels[x+1,y+1,h];//v7
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x + 1, y + 1, z + 1); // _voxels[x+1,y+1,h+1];//v6
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x, y + 1, z + 1); //_voxels[x,y+1,h+1];//v5
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x, y, z + 1);// _voxels[x,y+1,h];//v4
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x + 1, y, z);//_voxels[x+1,y,h];//v3
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x + 1, y + 1, z);//_voxels[x+1,y,h+1];//v2
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x, y + 1, z);//_voxels[x,y,h+1];//v1
		caseValue = caseValue * 2 + IsEditSpacePointSolid (x, y, z);//_voxels[x,y,h];//v0
		
		int[,] caseTriangles = _marchingCubes.getCaseTriangles (caseValue);
		for (int i = 0; i < caseTriangles.GetLength(0); i++) {
			int edgeA = caseTriangles[i,0];
			int edgeB = caseTriangles[i,1];
			int edgeC = caseTriangles[i,2];
			
			if (!use_lerp_color) {
				// color mode, connected triangle in one voxel space will has same color, the color is to the nearest voxel point.
				int nearestVoxelPointInde =  _nearestVoxelPointTable[caseValue,i];
				int materialIndex = GetVoxelPointMaterial(nearestVoxelPointInde, x, y, z);
				
				AddTriangle(Edge2Position(edgeA, x, y, z), 
				            Edge2Position(edgeB, x, y, z), 
				            Edge2Position(edgeC, x, y, z), 
				            materialIndex, 
				            materialIndex, 
				            materialIndex);
				
			} else {
				// color mode, every single edge point will find its own color, the color is to the nearest voxel point.
				AddTriangle(Edge2Position(edgeA, x, y, z),
				            Edge2Position(edgeB, x, y, z), 
				            Edge2Position(edgeC, x, y, z),
				            GetVoxelPointMaterial(_nearestVoxelPointForEdgePointTable[caseValue, edgeA], x, y, z),
				            GetVoxelPointMaterial(_nearestVoxelPointForEdgePointTable[caseValue, edgeB], x, y, z),
				            GetVoxelPointMaterial(_nearestVoxelPointForEdgePointTable[caseValue, edgeC], x, y, z));
				
			}
			
		}
	}
	
	IntVector3 VoxelPointPosition(int voxelPointIndex) {
		int x, y, z;
		x = y = z = 0;
		switch (voxelPointIndex) {
		case 0:
			break;
		case 1:
			y = 1;
			break;
		case 2:
			x = y = 1;
			break;
		case 3:
			x = 1;
			break;
		case 4:
			z = 1;
			break;
		case 5:
			y = z = 1;
			break;
		case 6:
			x = y = z = 1;
			break;
		case 7:
			x = z = 1;
			break;
		}
		return new IntVector3 (x, y, z);
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
	
	void AddVertex(Vector3 node, Color co) {
		_vertices.Add (node);
		//		_uvs.Add (node.toUV (_voxels.GetLength(0), _voxels.GetLength(2)));
		_colors.Add (co);
	}
	
	Color GetMatColor(int mat)
	{
		return _materialsController.GetMaterialColor (mat);
	}
	
	void AddTriangle(Vector3 a, Vector3 b, Vector3 c, int matA, int matB, int matC)
	{
		if (_vertices.Count > 64990) {
			Debug.Log("vertice count out ");
			Debug.Assert(false);
			return;
		}
		
		_triangles.Add (_vertices.Count);
		AddVertex(a,GetMatColor(matA));
		
		_triangles.Add (_vertices.Count);
		AddVertex (b, GetMatColor (matB));
		
		_triangles.Add (_vertices.Count);
		AddVertex(c, GetMatColor(matC));
		
	}

	public void AddPreset(PolyWorldController.PresetType t, int value, float fillrate)
	{
		Debug.Log ("aa");
		switch (t) {
			
		case PolyWorldController.PresetType.Cube:
			AddCube(value, fillrate);
			break;
			
		case PolyWorldController.PresetType.Sphere:
			AddSphere(value, fillrate);
			break;
			
		case PolyWorldController.PresetType.Floor://plane
			AddFloor(value, fillrate);
			break;

		}
	}

	public void ClearA()
	{
		Debug.Log ("0");
		int cubeHalfWidth = EDITOR_SPACE_HALF_WIDTH;
		Debug.Assert (_materialsController != null);
		
		int mat = _materialsController.GetBrushMaterial ();
		
		for (int x = -cubeHalfWidth; x<= cubeHalfWidth; x++) {
			for (int y = -cubeHalfWidth; y <= cubeHalfWidth; y++) {
				for (int z = -cubeHalfWidth; z <= cubeHalfWidth; z++){
					SetEditSpacePoint(x,y,z,0);
				}
			}
		}
		
		RefreshMesh ();
	}
	
	void AddCube(int r, float fillrate)
	{
		Debug.Log ("AddCube"+fillrate.ToString());
		int cubeHalfWidth = r;
		Debug.Assert (_materialsController != null);

		int mat = _materialsController.GetBrushMaterial ();
		
		for (int x = -cubeHalfWidth; x<= cubeHalfWidth; x++) {
			for (int y = -cubeHalfWidth; y <= cubeHalfWidth; y++) {
				for (int z = -cubeHalfWidth; z <= cubeHalfWidth; z++){
					if (Random.value < fillrate) 
						SetEditSpacePoint(x,y,z,mat);
				}
			}
		}

		RefreshMesh ();
	}

	void AddSphere(int r, float fillrate)
	{
		Debug.Log ("Add sphere");
		int cubeHalfWidth = r;
		Debug.Assert (_materialsController != null);
		
		int mat = _materialsController.GetBrushMaterial ();
		
		for (int x = -cubeHalfWidth; x<= cubeHalfWidth; x++) {
			for (int y = -cubeHalfWidth; y <= cubeHalfWidth; y++) {
				for (int z = -cubeHalfWidth; z <= cubeHalfWidth; z++){
					if ( x*x + y*y + z*z <= r*r) {
						if (Random.value < fillrate) 
							SetEditSpacePoint(x,y,z,mat);
					}
				}
			}
		}
		
		RefreshMesh ();
	}

	void AddFloor(int r, float fillrate)
	{
		Debug.Log ("Add floor");
		int cubeHalfWidth = r;
		Debug.Assert (_materialsController != null);
		
		int mat = _materialsController.GetBrushMaterial ();
		
		for (int x = -cubeHalfWidth; x<= cubeHalfWidth; x++) {
//			for (int y = -cubeHalfWidth; y <= cubeHalfWidth; y++) {
				for (int z = -cubeHalfWidth; z <= cubeHalfWidth; z++){
					if (Random.value < fillrate) 
						SetEditSpacePoint(x,0,z,mat);
				}
//			}
		}
		
		RefreshMesh ();
	}


	
	
	private float _emissionWaitTime = 0;
	private bool _shouldEmit = false;
	void EmissionCalc()
	{
		if (Input.GetMouseButtonDown (0)) {
			_emissionWaitTime = _editorState.emission_wait_time;
			_shouldEmit = true;
		} else if (Input.GetMouseButton (0)) {
			_emissionWaitTime -= Time.deltaTime;
			if (_emissionWaitTime < 0) {
				_emissionWaitTime = _editorState.emission_wait_time;
				_shouldEmit = true;
			}
		}
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
		
		if (_shouldEmit && Input.GetMouseButton(0)) {
			_shouldEmit = false;
			
			if (_editorState.is_add) {
				AddBrush(localPoint, localNormal, 0);
			} else {
				SubBrush(localPoint, localNormal, 0);
			}
		}
	}
	
	void AddBrush(Vector3 srcPoint, Vector3 normal, float extand)
	{
		if (extand > 1)
			return;
		
		// test, only the cloest point
		Vector3 point = srcPoint + normal * extand;
		int x = Mathf.RoundToInt (point.x);
		int y = Mathf.RoundToInt (point.y);
		int z = Mathf.RoundToInt (point.z);
		if (IsEditSpacePointSolid (x, y, z) == 1) {
			AddBrush(srcPoint, normal, extand+0.1f);
		} else {
			SetEditSpacePoint (x, y, z, _materialsController.GetBrushMaterial());
			RefreshMesh ();
		}
	}
	
	void SubBrush(Vector3 srcPoint, Vector3 normal, float extand)
	{
		if (extand < -1)
			return;
		
		// test, only the cloest point
		Vector3 point = srcPoint + normal * extand;
		int x = Mathf.RoundToInt (point.x);
		int y = Mathf.RoundToInt (point.y);
		int z = Mathf.RoundToInt (point.z);
		if (IsEditSpacePointSolid (x, y, z) == 0) {
			SubBrush(srcPoint, normal, extand-0.1f);
		} else {
			SetEditSpacePoint (x, y, z, 0);
			RefreshMesh ();
		}
	}
	
	
	private int[,] _nearestVoxelPointTable = new int[256,5];
	
	void GenNearestVoxelPointTable()
	{
		for (int caseValue = 0; caseValue <256; caseValue++) {
			int[,] caseTriangles = _marchingCubes.getCaseTriangles (caseValue);
			
			int[,] groupTrignales = new int[4,14];
			
			for (int i = 0; i <4; i++) {
				groupTrignales[i,0] = 0;
				groupTrignales[i,1] = 0;
				
			}
			
			int groupNum = 0;
			
			for (int i = 0; i < caseTriangles.GetLength(0); i++) {
				int edgeA = caseTriangles[i,0];
				int edgeB = caseTriangles[i,1];
				int edgeC = caseTriangles[i,2];
				
				bool newGroup =  true;
				int group2put = 0;
				for (int j = 0; j < groupNum; j++) {
					for (int k = 0; k < groupTrignales[j,1]; k++) {
						int v = groupTrignales[j,k+2];
						if (edgeA == v || edgeB == v || edgeC == v) {
							newGroup = false;
							group2put = j;
							break;
						}
					}
				}
				
				if (newGroup) {
					groupTrignales[groupNum,1] = 3;
					groupTrignales[groupNum,2] = edgeA;
					groupTrignales[groupNum,3] = edgeB;
					groupTrignales[groupNum,4] = edgeC;
					groupNum++;
				} else {
					{
						int value = edgeA;
						bool toPut = true;
						for (int j = 0; j < groupTrignales[group2put,1]; j++) {
							if (groupTrignales[group2put,j+2] == value) {
								toPut = false;
								break;
							}
						}
						if (toPut) {
							groupTrignales[group2put,groupTrignales[group2put,1]+2] = value;
							groupTrignales[group2put,1]++;
						}
					}
					{
						int value = edgeB;
						bool toPut = true;
						for (int j = 0; j < groupTrignales[group2put,1]; j++) {
							if (groupTrignales[group2put,j+2] == value) {
								toPut = false;
								break;
							}
						}
						if (toPut) {
							groupTrignales[group2put,groupTrignales[group2put,1]+2] = value;
							groupTrignales[group2put,1]++;
						}
					}
					{
						int value = edgeB;
						bool toPut = true;
						for (int j = 0; j < groupTrignales[group2put,1]; j++) {
							if (groupTrignales[group2put,j+2] == value) {
								toPut = false;
								break;
							}
						}
						if (toPut) {
							groupTrignales[group2put,groupTrignales[group2put,1]+2] = value;
							groupTrignales[group2put,1]++;
						}
					}
				}
			}
			
			
			// calc per group 
			for (int i = 0; i < groupNum; i++) {
				Vector3 sum = Vector3.zero;
				for (int j = 0; j < groupTrignales[i,1]; j++) {
					sum += Edge2Position(groupTrignales[i,2+j],0,0,0);
				}
				Vector3 center = sum/(groupTrignales[i,1] * 1f);
				
				int caseValueIndex = 1;
				int minVoxelPoint = 0;
				float minDistance = 100000f;
				for (int j = 0; j < 8; j++) {
					caseValueIndex = 1 << j;
					if ((caseValueIndex & caseValue) > 0) {
						IntVector3 voxelPosInt =  VoxelPointPosition(j);
						float distance = (voxelPosInt.ToFloat() - center).sqrMagnitude;
						if (distance < minDistance) {
							minDistance = distance;
							minVoxelPoint = j;
						}
					}
				}
				groupTrignales[i,0] = minVoxelPoint;
			}
			
			for (int i = 0; i < caseTriangles.GetLength(0); i++) {
				int edgeA = caseTriangles[i,0];
				/*
				int edgeB = caseTriangles[i,1];
				int edgeC = caseTriangles[i,2];
				
				Vector3 center = ( Edge2Position(edgeA,0,0,0) + Edge2Position(edgeB, 0,0,0) + Edge2Position(edgeC, 0,0,0))/3f;
				int caseValueIndex = 1;
				int minVoxelPoint = 0;
				float minDistance = 100000f;
				for (int j = 0; j < 8; j++) {
					caseValueIndex = 1 << j;
					if ((caseValueIndex & caseValue) > 0) {
						IntVector3 voxelPosInt =  VoxelPointPosition(j);
						float distance = (voxelPosInt.ToFloat() - center).sqrMagnitude;
						if (distance < minDistance) {
							minDistance = distance;
							minVoxelPoint = j;
						}
					}
				}*/
				
				for (int j = 0; j < groupNum; j++) {
					for (int k = 0; k < groupTrignales[j,1]; k++) {
						if (edgeA == groupTrignales[j, 2+k]) {
							_nearestVoxelPointTable[caseValue,i] = groupTrignales[j,0];
							break;
						}
					}
				}
			}
		}
	}
	
	private int[,] _nearestVoxelPointForEdgePointTable = new int[256,12];
	private void GenNearestVoxelPointForEdgePointTable()
	{
		
		for (int caseValue = 0; caseValue <256; caseValue++) {
			for (int edge = 0; edge < 12; edge++) {
				Vector3 edgePosition = Edge2Position(edge,0,0,0);
				int minVoxel = -1;
				float minDistance = 1000;
				for (int voxel = 0; voxel < 8; voxel++) {
					int keyvalue = 1 << voxel;
					if ((caseValue & keyvalue) > 0) {
						IntVector3 voxelPosition = VoxelPointPosition(voxel);
						float dist = (voxelPosition.ToFloat()-edgePosition).sqrMagnitude;
						if (dist < minDistance) {
							minDistance = dist;
							minVoxel = voxel;
						}
					}
				}
				_nearestVoxelPointForEdgePointTable[caseValue, edge] = minVoxel;
			}
		}
	}

	private bool _flagShowOutline = false;
	private bool _flagTranslation = false;
	void RefreshMaterialSetting()
	{
		var render = GetComponent<MeshRenderer> ();
		if (_flagTranslation) {
			GetComponent<MeshRenderer> ().materials = new Material[]{mat_translation};
		} else if (_flagShowOutline) {
			GetComponent<MeshRenderer> ().materials = new Material[]{mat_normal, mat_out_line};
		} else {
			GetComponent<MeshRenderer> ().materials = new Material[]{mat_normal};
		}
	}

	public void SetSelection(bool isSelected)
	{
		_selected = isSelected;
		_flagShowOutline = isSelected;
		RefreshMaterialSetting ();
	}

	public void SetTranslation(bool isTranslation)
	{
		_flagTranslation = isTranslation;
		RefreshMaterialSetting ();
	}

	
	private PolyObjectController _dupliOther;
	public void DuplicateFrom(PolyObjectController other)
	{
		// at this time, Start() may not be called!
		_dupliOther = other;
		Invoke ("DuplicateFromAst", 0.1f);
	}
	
	void DuplicateFromAst()
	{
		PolyObjectController other = _dupliOther;
		Debug.Assert (_editSpace != null);
		for (int x = 0; x < other._editSpace.GetLength(0); x++) {
			for (int y = 0; y < other._editSpace.GetLength(1); y++) {
				for (int z =0; z < other._editSpace.GetLength(2); z++) {
					_editSpace [x, y, z] = other._editSpace [x, y, z];
				}
			}
		}
		RefreshMesh ();
	}
}

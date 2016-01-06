using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// as the terrain self
public class TerrainGenerator : MonoBehaviour {

	public GameObject terrainFab;

	public int width_para_n = 5; //2^N + 1 per edge
	public float reduce_rate = 0.7f; //random value scope reduce radio
	public float init_height_left_bottom = 0f;
	public float init_height_left_top = 10f;
	public float init_height_right_top = 10f;
	public float init_height_right_bottom = 0f;
	public float init_random_scope = 10f;
	public string random_seed = "arisecbf";
	public float center_deep_scale = 2f;
	public bool smooth_normal = false;

	private float [,] _heightMap;
	private float _minHeight;
	private Hmp _minHeightPosition;
	private float _maxHeight;
	private int[,,] _voxels; // 0 -empty 1-fill, convient for bit shift in case matching
	private int[,] _voxelHeights;
	private System.Random _randomGen;

	private Dictionary<VoxelNode, int> _voxelPointIndexTable; //vertic's index lookup
	private List<Vector3> _vertices; //vertices
	private List<Vector2> _uvs;
	private List<int> _triangles; //index

	private List<GameObject> _childTerrains;
	private GameObject _currentChildTerrain;
	private MeshFilter _currentMeshFilter;

	class Hmp //height map point
	{
		public Hmp(int x, int y){
			this.x = x;
			this.y = y;
		}

		public int x;
		public int y;
	}

	class VoxelNode
	{
		public int x;
		public int y;
		public int h;

		public VoxelNode(int x, int y, int h) {
			this.x = x;
			this.y = y;
			this.h = h;
		}
		public class EqualityComparer : IEqualityComparer<VoxelNode> {
			
			public bool Equals(VoxelNode a, VoxelNode b) {
				return a.x == b.x && a.y == b.y && a.h == b.h;
			}
			
			public int GetHashCode(VoxelNode a) {
				int c = a.x ^ a.y ^ a.h;
				return c.GetHashCode();
			}
		}

		public Vector3 toVector()
		{
			return new Vector3 ((float)x, (float)h, (float)y); //to Unity's axises
		}

		public Vector2 toUV(int width, int hMax)
		{
			float v = h * 1.0f / hMax;
			float u = x * 1.0f / width;
			return new Vector2 (u, v);
		}

	}


	private bool _flagCenterDown = true;
	void generateHeightMap()
	{
		_randomGen = new System.Random(random_seed.GetHashCode());

		int width = 1;
		for (int i = 0; i < width_para_n; i++) {
			width = width*2;
		}
		width += 1;

		_heightMap = new float[width,width];
		_heightMap [0, 0] = init_height_left_bottom;
		_heightMap [0,width - 1] = init_height_left_top;
		_heightMap [width - 1,0] = init_height_right_bottom;
		_heightMap [width - 1,width - 1] = init_height_right_top;

		_minHeight = 100000;
		_maxHeight = -100000;


		int currentStep = width - 1;
		_flagCenterDown = true;
		float currentRandomScope = init_random_scope;
		for (int i = 0; i < width_para_n; i++) {
			for (int x = 0; x < width-1; x += currentStep) {
				for ( int y = 0; y < width-1; y += currentStep) {
					diamond(new Hmp(x,y), new Hmp(x, y+currentStep), new Hmp(x+currentStep, y), new Hmp(x+currentStep, y+currentStep),currentRandomScope);
				}
			}
			int halfStep = currentStep/2;
			for (int y = 0; y < width; y += halfStep) {
				for (int x = y%(currentStep) == 0 ? halfStep : 0; x < width; x+= currentStep) {
//					Debug.Log(x.ToString() + " " +y.ToString() + " " + halfStep.ToString());
					square(new Hmp(x-halfStep,y), new Hmp(x+halfStep, y), new Hmp(x, y-halfStep), new Hmp(x, y+halfStep),currentRandomScope);
				}
			}
			currentStep = currentStep/2;
			currentRandomScope = currentRandomScope*reduce_rate;
		}


	}

	float getHmpv(Hmp p) {
		return _heightMap [p.x, p.y];
	}
	void setHmpv(Hmp p, float v) {
		_heightMap [p.x, p.y] = v;
		if (v < _minHeight) {
			_minHeight = v;
			_minHeightPosition = p;
		}
		_maxHeight = Mathf.Max (_maxHeight, v);
	}
	bool isLegelHmp(Hmp p) {
		int width = _heightMap.GetLength (0);
		return p.x >= 0 && p.x < width && p.y >= 0 && p.y < width;
	}

	void diamond(Hmp lb, Hmp lt,Hmp rb, Hmp rt, float randomScope)
	{
		int centerX = (lb.x + rt.x) / 2;
		int centerY = (lb.y + rt.y) / 2;
		Hmp center = new Hmp (centerX, centerY);
		float centerValue = 0.25f * (getHmpv (lb) + getHmpv (lt) + getHmpv (rb) + getHmpv (rt)) + 
			(_flagCenterDown ? -((float)(_randomGen.NextDouble()+0.5f)*randomScope)*center_deep_scale : (float)((_randomGen.NextDouble()-0.5)*2)*randomScope);
		_flagCenterDown = false;
		setHmpv (center, centerValue);

		/*
		float nextRandomScope = randomScope * reduce_rate;
		// left square
		square (new Hmp (lb.x + lb.x - center.x, center.y), center, lb, lt, nextRandomScope);

		// right square
		square (center, new Hmp (rb.x + rb.x - center.x, center.y), rb, rt, nextRandomScope);

		// bottom square
		square (lb, rb, new Hmp (center.x, lb.y + lb.y - center.y), center, nextRandomScope);

		// top square
		square (lt, rt, center, new Hmp (center.x, lt.y + lt.y - center.y), nextRandomScope);
		*/
	}

	void square(Hmp left, Hmp right, Hmp bottom, Hmp top,  float randomScope)
	{
		int cnt = 0;
		float amount = 0;
		if (isLegelHmp (left)) {
			amount += getHmpv(left);
			cnt ++;
		} 
		if (isLegelHmp (right)) {
			amount += getHmpv(right);
			cnt ++;
		} 
		if (isLegelHmp (bottom)) {
			amount += getHmpv(bottom);
			cnt ++;
		} 
		if (isLegelHmp (top)) {
			amount += getHmpv(top);
			cnt ++;
		} 

		float centerValue = amount / cnt + (float)((_randomGen.NextDouble () - 0.5)*2) * randomScope;
		Hmp center = new Hmp (top.x, left.y);
		setHmpv (center, centerValue);

	}

	void generateVoxelMesh() 
	{
		// using Height map && minHeight/maxHeight, using 1 as step!!


		//new voxels
		float totalHeight = (_maxHeight - _minHeight);
		int voxelHeight = (int)totalHeight + 3;
		int width = _heightMap.GetLength (0);
		_voxels = new int[width,width,voxelHeight]; // x,y,height
		_voxelHeights = new int[width, width];

		//heightmap to voxels
		for (int y = 0; y < width; y++) {
			for (int x = 0; x < width; x++) {
				float v = getHmpv(new Hmp(x,y));
				int height = (int)(v-_minHeight);
				_voxelHeights[x,y] = height;
				for (int i = 0; i < voxelHeight; i++) {
					_voxels[x,y,i] = (i <= height) ? 1 : 0;
				}
			}
		}


		//marching cubes
		for (int y = 0; y < _voxels.GetLength(1)-1; y++) {
			for (int x = 0; x < _voxels.GetLength(0)-1; x++) {
				for (int h = 0; h < _voxels.GetLength(2)-1; h++) {
					marchPerCube(x,y,h);
				}
			}
		}

		setCurrentMeshFilter ();
	}

	private MarchingCubes _marchingCubes  = new MarchingCubes();
	void marchPerCube(int x, int y, int h)
	{
		int caseValue = 0;

		caseValue = caseValue*2 + _voxels[x+1,y+1,h];//v7
		caseValue = caseValue*2 + _voxels[x+1,y+1,h+1];//v6
		caseValue = caseValue*2 + _voxels[x,y+1,h+1];//v5
		caseValue = caseValue*2 + _voxels[x,y+1,h];//v4
		caseValue = caseValue*2 + _voxels[x+1,y,h];//v3
		caseValue = caseValue*2 + _voxels[x+1,y,h+1];//v2
		caseValue = caseValue*2 + _voxels[x,y,h+1];//v1
		caseValue = caseValue*2 + _voxels[x,y,h];//v0

		int[,] caseTriangles = _marchingCubes.getCaseTriangles (caseValue);
		for (int i = 0; i < caseTriangles.GetLength(0); i++) {
			int edgeA = caseTriangles[i,0];
			int edgeB = caseTriangles[i,1];
			int edgeC = caseTriangles[i,2];

			addTriangle(edge2voxelNode(edgeA, x, y, h), edge2voxelNode(edgeB, x, y, h), edge2voxelNode(edgeC, x, y, h));
		}
	}

	VoxelNode edge2voxelNode(int edgeNumber, int originX, int originY, int originH) {
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

		return new VoxelNode (originX * 2 + dx, originY * 2 + dy, originH * 2 + dh);
	}

	void addVertex(VoxelNode node) {
		_vertices.Add (node.toVector ());
		_uvs.Add (node.toUV (_voxels.GetLength(0), _voxels.GetLength(2)));
	}

	void addTriangle(VoxelNode a, VoxelNode b, VoxelNode c)
	{
		if (_vertices.Count > 64990) {
			nextChildMesh();
		}
		if (smooth_normal) {
			//a
			if (!_voxelPointIndexTable.ContainsKey (a)) {
				_voxelPointIndexTable.Add (a, _vertices.Count);
				addVertex(a);
			}

			//b
			if (!_voxelPointIndexTable.ContainsKey (b)) {
				_voxelPointIndexTable.Add (b, _vertices.Count);
				addVertex(b);

			}

			//c
			if (!_voxelPointIndexTable.ContainsKey (c)) {
				_voxelPointIndexTable.Add (c, _vertices.Count);
				addVertex(c);

			}
			_triangles.Add (_voxelPointIndexTable [a]);
			_triangles.Add (_voxelPointIndexTable [b]);
			_triangles.Add (_voxelPointIndexTable [c]);
		} else {
			_triangles.Add (_vertices.Count);
			addVertex(a);
			
			_triangles.Add (_vertices.Count);
			addVertex(b);
			
			_triangles.Add (_vertices.Count);
			addVertex(c);
		}
	}

	// call this by outer to generate the terrain, time costly!
	public void generateTerrain(){
		generateHeightMap ();
		generateVoxelMesh ();

		terrainFab.GetComponent<MeshRenderer> ().sharedMaterial.mainTexture = (Resources.Load ("TerrainTexture/" + ((int)(UnityEngine.Random.Range (0, 9.5f))).ToString ()) as Texture);
	}


	void nextChildMesh(){
		setCurrentMeshFilter ();

		_voxelPointIndexTable = new Dictionary<VoxelNode, int> ();
		_vertices = new List<Vector3> ();
		_triangles = new List<int> ();
		_uvs = new List<Vector2> ();


		_currentChildTerrain = (GameObject)Instantiate(terrainFab, Vector3.zero, Quaternion.identity);
		_currentChildTerrain.transform.parent = gameObject.transform;
		_currentMeshFilter = _currentChildTerrain.GetComponent<MeshFilter> ();
		_childTerrains.Add (_currentChildTerrain);
	}

	void setCurrentMeshFilter(){
		if (_currentMeshFilter != null) {
			Mesh mesh = new Mesh();
			_currentMeshFilter.mesh = mesh;
			mesh.vertices = _vertices.ToArray();
			mesh.triangles = _triangles.ToArray();
			mesh.uv = _uvs.ToArray();
			mesh.RecalculateNormals();
			
			Debug.Log ("TerrainFab vertices = " + _vertices.Count.ToString());
		}
	}

	// Use this for initialization
	void Start () {
		_childTerrains = new List<GameObject> ();
		
		nextChildMesh ();
	}

	// get deepest position
	public Vector3 getValleyPosition()
	{
		int center = _voxels.GetLength (0)/2;
		int h = 0;
		for (;; h++) {
			if (_voxels[center,center,h] == 0 )
			{
				break;
			}
		}
		h += 4;
		return new Vector3(center * 2f, h*2f, center*2f);
	}

	public Vector3 nextFlyingPosition()
	{
		int rbegin = (int)(_voxelHeights.GetLength (0) * 0.3);
		int rend = (int)(_voxelHeights.GetLength (0) * 0.7);
		int x = _randomGen.Next (rbegin, rend);
		int y = _randomGen.Next (rbegin, rend);
		return new Vector3 (x * 2f, (_voxelHeights [x, y]+5) * 2f, y * 2f);
	}

	public Vector3 nextFlyingDirection()
	{
		return (new Vector3 ((float)_randomGen.NextDouble(), 0f, (float)_randomGen.NextDouble())).normalized;
	}

	public float getTerrainWidth()
	{
		return _voxels.GetLength(0)*2f;
	}

	/*
	public bool CheckSolid(Vector3 p)
	{
		int x = Convert.ToInt32 (p.x * 0.5f);
		int y = Convert.ToInt32 (p.z * 0.5f);
		int h = Convert.ToInt32 (p.y * 0.5f);
		return _voxels [x, y, h] != 0;
	}*/
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos() {
//		if (_heightMap != null) {
//			int width = _heightMap.GetLength (0);
//			int height = width;
//			for (int x = 0; x < width; x ++) {
//				for (int y = 0; y < height; y ++) {
//					float v = getHmpv(new Hmp(x,y));
////					Debug.Log("v"+v.ToString());
//					Gizmos.color = new Color((v+5)/10f,1-(v+5)/10f,0,1);
//					Vector3 pos = new Vector3(-width/2 + x + .5f, v, -height/2 + y+.5f);
//					Gizmos.DrawCube(pos,Vector3.one/2f);
//				}
//			}
//		}

//		if (_voxels != null) {
//			for (int x = 0; x < _voxels.GetLength(0); x++) {
//				for (int y = 0; y < _voxels.GetLength(1); y++) {
//					for (int h = 0; h < _voxels.GetLength(2); h++) {
//
//						if (_voxels[x,y,h] == 1) {
//						Gizmos.color = Color.gray;
//						Vector3 pos = new Vector3(x*2, h*2, y*2);
//						Gizmos.DrawSphere(pos, 0.25f);
//						}
//					}
//				}
//			}
//
//		}

	}
}

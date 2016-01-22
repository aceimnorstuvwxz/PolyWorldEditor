using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class MarchingCubesEngine: MonoBehaviour 
{
	private MaterialsController _materialsController;

	// tmp data
	private List<Vector3> _vertices; //vertices
		private List<Vector2> _uvs;
	private List<int> _triangles; //index
	private List<Color> _colors;
	private IntVector3 _origin;
	private int[,,] _editSpace; // 0 ? -> not solid, 1-N ->solid, with material index


	private MarchingCubes _marchingCubes  = new MarchingCubes();

	void Start () 
	{
		GenNearestVoxelPointForEdgePointTable ();
		_materialsController = GameObject.Find ("MaterialsCanvas").GetComponent<MaterialsController> ();

		Debug.Log ("init MarchingCubesEngine");
	}

	public class Result
	{
		public List<Vector3> vertices; //vertices
		public List<Vector2> uvs;
		public List<int> triangles; //index
		public List<Color> colors; // TODO tiaoseban
	};


	public Result Marching(int[,,] space, IntVector3 origin)
	{
		
		_vertices = new List<Vector3> ();
		_triangles = new List<int> ();
		_colors = new List<Color> ();
		_uvs = new List<Vector2> ();

		_origin = origin;
		_editSpace = space;

		for (int x = 0; x < space.GetLength(0)-1 ; x++) {
			for (int y = 0; y < space.GetLength(1)-1; y++) {
				for (int z = 0; z < space.GetLength(2)-1; z++) {
					MarchPerCube(x,y,z);
				}
			}
		}

		Result res = new Result ();
		res.vertices = _vertices;
		res.triangles = _triangles;
		res.colors = _colors;
		res.uvs = _uvs;

		_vertices = null;
		_triangles = null;
		_colors = null;
		_uvs = null;
		_editSpace = null;

		return res;
	}

	int IsEditSpacePointSolid(int x, int y, int z)
	{
		return _editSpace [x, y, z] > 0 ? 1 : 0;
	}

	int GetEditSpacePoint(int x, int y, int z)
	{
		return _editSpace [x, y, z];
	}

	int GetVoxelPointMaterial(int voxelRelativeIndex, int x, int y, int z)
	{
		IntVector3 diffPos = VoxelPointPosition(voxelRelativeIndex);
		int materialIndex = GetEditSpacePoint(x + diffPos.x, y + diffPos.y, z + diffPos.z);
		return materialIndex;
	}

	void AddVertex(Vector3 node, Vector2 uv) {
		_vertices.Add (node + _origin.ToFloat());
		_uvs.Add (uv);
		_colors.Add (Color.green);
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
		AddVertex (a, _materialsController.GetMaterialPaletteUV(matA));
		
		_triangles.Add (_vertices.Count);
		AddVertex (b, _materialsController.GetMaterialPaletteUV (matB));
		
		_triangles.Add (_vertices.Count);
		AddVertex (c, _materialsController.GetMaterialPaletteUV (matC));
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
			
//			if (!use_lerp_color) {
//				// color mode, connected triangle in one voxel space will has same color, the color is to the nearest voxel point.
//				int nearestVoxelPointInde =  _nearestVoxelPointTable[caseValue,i];
//				int materialIndex = GetVoxelPointMaterial(nearestVoxelPointInde, x, y, z);
//				
//				AddTriangle(Edge2Position(edgeA, x, y, z), 
//				            Edge2Position(edgeB, x, y, z), 
//				            Edge2Position(edgeC, x, y, z), 
//				            materialIndex, 
//				            materialIndex, 
//				            materialIndex);
//
//			} else {
				// color mode, every single edge point will find its own color, the color is to the nearest voxel point.
				AddTriangle(Edge2Position(edgeA, x, y, z),
				            Edge2Position(edgeB, x, y, z), 
				            Edge2Position(edgeC, x, y, z),
				            GetVoxelPointMaterial(_nearestVoxelPointForEdgePointTable[caseValue, edgeA], x, y, z),
				            GetVoxelPointMaterial(_nearestVoxelPointForEdgePointTable[caseValue, edgeB], x, y, z),
				            GetVoxelPointMaterial(_nearestVoxelPointForEdgePointTable[caseValue, edgeC], x, y, z));
				
//			}
			
		}
	}

	Vector3 Edge2Position(int edgeNumber, int originX, int originY, int originZ) 
	{
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

	IntVector3 VoxelPointPosition(int voxelPointIndex) 
	{
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
}

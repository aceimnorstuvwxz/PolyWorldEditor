using UnityEngine;
using System.Collections;

public class IntVector3
{
	// direction relation is same to unity scene editor
	public int x;
	public int y;
	public int z;
	
	public IntVector3(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	public Vector3 ToFloat() {
		return new Vector3 (x * 1f, y * 1f, z * 1f);
	}

	public IntVector3 multi(int p)
	{
		return new IntVector3(x * p, y * p, z * p);
	}
}

public class Fonda 
{

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

	public static IntVector3 FromFloat(Vector3 point)
	{
		return new IntVector3 (Mathf.RoundToInt (point.x), Mathf.RoundToInt (point.y), Mathf.RoundToInt (point.z));
	}

	public IntVector3 multi(int p)
	{
		return new IntVector3(x * p, y * p, z * p);
	}

	public class EqualityComparer : IEqualityComparer<IntVector3> {
		
		public bool Equals(IntVector3 a, IntVector3 b) {
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}
		
		public int GetHashCode(IntVector3 a) {
			int c = a.x ^ a.y ^ a.z;
			return c.GetHashCode();
		}
	}

	public string ToString()
	{
		return "" + x.ToString () + ", " + y.ToString () + ", " + z.ToString ();
	}
}

public class Fonda 
{

}

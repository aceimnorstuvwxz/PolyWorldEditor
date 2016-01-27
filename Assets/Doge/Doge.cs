using UnityEngine;
using System.Collections;

public class Doge {


	public static bool IsMouseOn(GameObject go)
	{
		RaycastHit hit;
		Collider cld = go.GetComponent<MeshCollider> ();
		return cld.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);
	}

	public static bool ColliderLineCast(Collider collider, Vector3 src, Vector3 des, out RaycastHit hit)
	{
		return  collider.Raycast (new Ray (src, des-src), out hit, (des - src).magnitude);
	}

	//http://answers.unity3d.com/questions/163864/test-if-point-is-in-collider.html
	public static bool IsColliderContainPoint(Vector3 outsidePoint, Vector3 underTestPoint, Collider collider)
	{
		{
			// convex shape, this is ok!
			RaycastHit hit;
			return ColliderLineCast(collider, outsidePoint, underTestPoint, out hit) && 
				!ColliderLineCast(collider, underTestPoint, outsidePoint, out hit);
		}


		/*
		Vector3 Point;
		Vector3 Start = outsidePoint; // This is defined to be some arbitrary point far away from the collider.
		Vector3 Goal = underTestPoint; // This is the point we want to determine whether or not is inside or outside the collider.
		Vector3 Direction = (Goal-Start).normalized; // This is the direction from start to goal.
		int Itterations = 0; // If we know how many times the raycast has hit faces on its way to the target and back, we can tell through logic whether or not it is inside.
		Point = Start;

//		Debug.DrawRay(Goal, new Vector3(0,1,0)*100);
//		Debug.DrawRay(Goal, new Vector3(1,0,0)*100);


		while(Point != Goal) // Try to reach the point starting from the far off point.  This will pass through faces to reach its objective.
		{
			RaycastHit hit;
			if(ColliderLineCast(collider, Point, Goal, out hit)) // Progressively move the point forward, stopping everytime we see a new plane in the way.
			{

//				Debug.DrawRay(Goal, new Vector3(0,1,0)*100);
				Itterations ++;
				Point = hit.point + (Direction/100.0f); // Move the Point to hit.point and push it forward just a touch to move it through the skin of the mesh (if you don't push it, it will read that same point indefinately).
			}
			else
			{
				Point = Goal; // If there is no obstruction to our goal, then we can reach it in one step.
			}
			if (Itterations > 10) break;
		}
		while(Point != Start) // Try to return to where we came from, this will make sure we see all the back faces too.
		{
			RaycastHit hit;
			if(ColliderLineCast(collider, Point, Start, out hit))
			{
//				Debug.DrawRay(Goal, new Vector3(0,1,0)*100);
				Itterations ++;
				Point = hit.point + (-Direction/100.0f);
			}
			else
			{
				Point = Start;
			}
			if (Itterations > 10) break;
		}

		return (Itterations % 2) == 1;*/
	}


}

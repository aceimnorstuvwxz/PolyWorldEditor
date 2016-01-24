using UnityEngine;
using System.Collections;

public class Doge {


	//http://answers.unity3d.com/questions/163864/test-if-point-is-in-collider.html
	public static bool IsColliderContainPoint(Vector3 outsidePoint, Vector3 underTestPoint, Collider collider)
	{

		Vector3 Point;
		Vector3 Start = new Vector3(0,100,0); // This is defined to be some arbitrary point far away from the collider.
		Vector3 Goal = underTestPoint; // This is the point we want to determine whether or not is inside or outside the collider.
		Vector3 Direction = Goal-Start; // This is the direction from start to goal.
		Direction.Normalize();
		int Itterations = 0; // If we know how many times the raycast has hit faces on its way to the target and back, we can tell through logic whether or not it is inside.
		Point = Start;

		while(Point != Goal) // Try to reach the point starting from the far off point.  This will pass through faces to reach its objective.
		{
			RaycastHit hit;
			if( collider.Raycast( new Ray(Point, Goal), out hit, Mathf.Infinity)) // Progressively move the point forward, stopping everytime we see a new plane in the way.
			{
				Itterations ++;
				Point = hit.point + (Direction/100.0f); // Move the Point to hit.point and push it forward just a touch to move it through the skin of the mesh (if you don't push it, it will read that same point indefinately).
			}
			else
			{
				Point = Goal; // If there is no obstruction to our goal, then we can reach it in one step.
			}
		}
		while(Point != Start) // Try to return to where we came from, this will make sure we see all the back faces too.
		{
			RaycastHit hit;
			if( Physics.Linecast(Point, Start, out hit))
			{
				Itterations ++;
				Point = hit.point + (-Direction/100.0f);
			}
			else
			{
				Point = Start;
			}
		}

		return Itterations % 2 == 1;
	}

}

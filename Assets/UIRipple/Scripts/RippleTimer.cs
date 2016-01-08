//this script will cause a Ripple at a specific rate

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (UIRipple))]
public class RippleTimer : MonoBehaviour {

	/// <summary> 
	/// The Ripple's Offset
	/// </summary>
	public Vector2 Offset;

	/// <summary> 
	/// The Rate the Ripple will appear
	/// </summary>
	public float Rate;

	//just stores the time;
	float T;

	/// <summary> 
	//List of Colors that will be used
	/// <summary> 
	public List<Color> Colors = new List<Color>();
	
	//the index of color we are using from the list
	int ColorIndex = 0;

	// Update is called once per frame
	void Update () 
	{
		//current time - time of last Ripple >= Ripple
		if (Time.time - T >= Rate)
		{
			//create Ripple
			GetComponent<UIRipple>().CreateRipple(Offset);
			//set new time
			T = Time.time;

			//Change the colors, if there are colors to change to 
			if (Colors.Count > 0)
			{
				GetComponent<UIRipple>().StartColor = Colors[ColorIndex];
				GetComponent<UIRipple>().EndColor = Colors[ColorIndex];

				ColorIndex += 1;

				//loop back if at the end of the color list
				ColorIndex = ColorIndex % Colors.Count;
			}
		}
	}
}

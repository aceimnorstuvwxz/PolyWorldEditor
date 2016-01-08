//this script will animate the ripple

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ripple : MonoBehaviour {

	//parameters of the Ripple
	public float Speed;
	public float MaxSize;
	public Color StartColor;
	public Color EndColor;

	// Use this for initialization
	void Start () 
	{
		//set the size and the color
		transform.localScale = new Vector3(0f,0f,0f);
		GetComponent<Image>().color = new Color(StartColor.r,StartColor.g,StartColor.b,1f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//lerp the scale and the color
		transform.localScale = Vector3.Lerp (transform.localScale,new Vector3(MaxSize,MaxSize,MaxSize),Time.deltaTime * Speed);
		GetComponent<Image>().color = Color.Lerp (GetComponent<Image>().color,new Color(EndColor.r,EndColor.g,EndColor.b,0f),Time.deltaTime * Speed);

		//destroy at the end of life
		if (transform.localScale.x >= MaxSize * 0.995f)
		{
			Destroy(gameObject);
		}
	}
}

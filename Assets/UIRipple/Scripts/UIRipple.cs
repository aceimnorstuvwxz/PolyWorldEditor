//this script will create the ripples when clicked

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof (Mask))]
[RequireComponent (typeof (Image))]
public class UIRipple : MonoBehaviour {
	
	/// <summary> 
	/// the Sprite that will render
	/// </summary>
	public Sprite ShapeSprite;
	
	/// <summary> 
	/// the speed at which the ripple will grow
	/// </summary>
	[Range(0.25f,5f)]
	public float Speed = 1f;

	/// <summary> 
	/// If true the MaxSize will be set automatically
	/// </summary>
	public bool AutomaticMaxSize = true;

	/// <summary> 
	/// The Maximum Size of the Ripple
	/// </summary>
	public float MaxSize = 4f;
	
	/// <summary> 
	/// Start Color of Ripple
	/// </summary>
	public Color StartColor = new Color(1f,1f,1f,1f);

	/// <summary> 
	/// End Color of Ripple
	/// </summary>
	public Color EndColor = new Color(1f,1f,1f,1f);

	/// <summary> 
	/// If true the Ripple will only appear if you click on the UI Element
	/// </summary>
	public bool OnUIOnly = true;

	/// <summary> 
	/// If true Ripples will appear on the top of all other children in the UI Element 
	/// </summary>
	public bool RenderOnTop = false;

	/// <summary> 
	/// If true the Ripple will start at the center of the UI Element
	/// </summary>
	public bool StartAtCenter = false;

	void Awake()
	{
		//automatically set the MaxSize if needed
		if (AutomaticMaxSize)
		{
			RectTransform RT = gameObject.transform as RectTransform;
			MaxSize = (RT.rect.width > RT.rect.height)? 4f * ((float)Mathf.Abs(RT.rect.width)/(float)Mathf.Abs(RT.rect.height)) :4f * ((float)Mathf.Abs(RT.rect.height)/(float)Mathf.Abs(RT.rect.width));

			if (float.IsNaN(MaxSize))
			{
				MaxSize = (transform.localScale.x > transform.localScale.y)? 4f * transform.localScale.x :4f * transform.localScale.y;
			}
		}

		MaxSize = Mathf.Clamp(MaxSize,0.5f,1000f);
	}
	

	// Update is called once per frame
	void Update () 
	{
		//if the mouse button is down
		if (Input.GetMouseButtonDown(0))
		{
			//if UI only..
			if (OnUIOnly) 
			{
				//and the mouse is over the UIElement
				if (IsOnUIElement(Input.mousePosition))
				{
					//create the Ripple
					CreateRipple(Input.mousePosition);
				}
			}
			else
			{
				//create the Ripple
				CreateRipple(Input.mousePosition);
			}

		}
	}

	//this will create the Ripple
	public void CreateRipple(Vector2 Position)
	{
		//create the GameObject and add components
		GameObject ThisRipple = new GameObject();
		ThisRipple.AddComponent<Ripple>();
		ThisRipple.AddComponent<Image>();
		ThisRipple.GetComponent<Image>().sprite = ShapeSprite;
		ThisRipple.name = "Ripple";

		//set the parent
		ThisRipple.transform.SetParent(gameObject.transform);

		//rearrange the children if needed 
		if (!RenderOnTop)
		{ThisRipple.transform.SetAsFirstSibling();}

		//set the Ripple at the correct location
		if (StartAtCenter)
		{ThisRipple.transform.localPosition = new Vector2(0f,0f);}
		else
		{ThisRipple.transform.position = Position;}

		//set the parameters in the Ripple
		ThisRipple.GetComponent<Ripple>().Speed = Speed;
		ThisRipple.GetComponent<Ripple>().MaxSize = MaxSize;
		ThisRipple.GetComponent<Ripple>().StartColor = StartColor;
		ThisRipple.GetComponent<Ripple>().EndColor = EndColor;
	}



	public bool IsOnUIElement(Vector2 Position)
	{
		//if the point is within the bounds of the UIElement
		if (
			gameObject.transform.position.x + (GetComponent<RectTransform>().rect.width/2f) >= Position.x
			&& gameObject.transform.position.x - (GetComponent<RectTransform>().rect.width/2f)  <= Position.x
			&& gameObject.transform.position.y + (GetComponent<RectTransform>().rect.height/2f)  >= Position.y
			&& gameObject.transform.position.y - (GetComponent<RectTransform>().rect.height/2f)  <= Position.y
			)
		{
			return true; //return true
		}
		else
		{
			return false; //if not return false
		}
	}

}

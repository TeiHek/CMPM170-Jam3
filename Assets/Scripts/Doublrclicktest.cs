using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Doublrclicktest : MonoBehaviour
{
    public UnityEvent DoubleClick;
    public Transform units;
	float timerForDoubleClick = 0.0f;
	float delay = 0.3f;
	bool isDoubleClick = false;

	void Update()
	{
		if (isDoubleClick == true)
		{
			timerForDoubleClick += Time.deltaTime;
		}


		if (timerForDoubleClick >= delay)
		{
			timerForDoubleClick = 0.0f;
			isDoubleClick = false;
		}

	}

	
	void OnMouseOver()
	{
		if (Input.GetButtonDown("fire1") && isDoubleClick == false)
		{
			Debug.Log ("Mouse clicked once");
			isDoubleClick = true;
		}
	}

	void OnMouseDown()
	{
		if (isDoubleClick == true && timerForDoubleClick < delay)
		{
			Debug.Log("ISWORKING!!!!");
		}
	}
}
